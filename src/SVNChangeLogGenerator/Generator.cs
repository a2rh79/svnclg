﻿// Copyright © CharlieFirpo 2009 <Firpo.Charlie@googlemail.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Antlr.StringTemplate;

namespace SVNChangeLogGenerator
{
    class Generator
    {
        private StringTemplateGroup m_ClGrp = new StringTemplateGroup("ChangeLogGroup", (string)null);
        private Dictionary<ArgumentManager.Args, object> m_Arguments = new Dictionary<ArgumentManager.Args, object>();

        public string GenerateLog(XmlDocument log, Dictionary<ArgumentManager.Args, object> arguments)
        {
            XmlNodeList logEntries = log.GetElementsByTagName("logentry");
            List<ChangelogEntry> clEntries;
            m_Arguments = arguments;

            if (arguments.ContainsKey(ArgumentManager.Args.ranges))
            {
                clEntries = (List<ChangelogEntry>)arguments[ArgumentManager.Args.ranges];
            }
            else
            {
                clEntries = new List<ChangelogEntry>();
            }
            
            foreach (XmlNode node in logEntries)
            {
                string revisionString = node.Attributes.GetNamedItem("revision").Value;
                int revision;
                int.TryParse(revisionString, out revision);

                if (arguments.ContainsKey(ArgumentManager.Args.ranges))
                {
                    foreach (ChangelogEntry cle in clEntries)
                    {
                        if ((revision >= cle.StartRevision && revision <= cle.EndRevision) || cle.StartRevision == -1)
                        {
                            PopulateChangeLogEntry(node, cle, revision);
                            break;
                        }
                    }
                }
                else
                {
                    ChangelogEntry cle = new ChangelogEntry();

                    cle.StartRevision = revision;
                    PopulateChangeLogEntry(node, cle, revision);
                    clEntries.Add(cle);
                }
            }

            if (arguments.ContainsKey(ArgumentManager.Args.swap))
            {
                clEntries.Reverse();
            }

            string changeLog = SetStringTemplate(clEntries);

            return changeLog;
        }

        private void PopulateChangeLogEntry(XmlNode node, ChangelogEntry cle, int revision)
        {
            cle.Revisions.Add(revision);

            foreach (XmlNode childNode in node.ChildNodes)
            {
                string nodeName = childNode.Name;
                string innerText = childNode.InnerText;

                switch (nodeName)
                {
                    case "author":
                        if (!cle.Authors.Contains(innerText))
                            cle.Authors.Add(innerText);
                        break;
                    case "date":
                        string dateString = ConvertDate(innerText);
                        if (!cle.Dates.Contains(dateString))
                            cle.Dates.Add(dateString);
                        break;
                    case "msg":
                        List<string> msg = FormatMessage(innerText);
                        cle.Msg.AddRange(msg);
                        break;
                    case "paths":
                        GetPaths(childNode, cle);
                        break;
                }
            }
        }

        private string SetStringTemplate(List<ChangelogEntry> clEntries)
        {
            StringTemplate changeLog = new StringTemplate("$logEntry; separator=\"\\n\"$\r\nGenerated with Subversion Changelog Generator $currentVersion$\r\nCopyright © CharlieFirpo 2009 <Firpo.Charlie@googlemail.com>");
            changeLog.SetAttribute("currentVersion", Program.AssemblyVersion);

            foreach (ChangelogEntry cle in clEntries)
            {
                if (cle.Msg.Count > 0)
                {
                    StringTemplate logEntry = m_ClGrp.GetInstanceOf("Templates/ChangeLog");
                    logEntry.SetAttribute("date", cle.Dates[0]);
                    logEntry.SetAttribute("version", cle.VersionString);
                    logEntry.SetAttribute("revisions", cle.Revisions);
                    logEntry.SetAttribute("msg", cle.Msg);
                    logEntry.SetAttribute("path", cle.Paths);
                    logEntry.SetAttribute("authors", cle.Authors);
                    changeLog.SetAttribute("logEntry", logEntry);
                }
            }

            return changeLog.ToString();
        }

        private List<string> FormatMessage(string msg)
        {
            string[] messages = msg.Trim().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<string> retVal = new List<string>();

            foreach (string s in messages)
            {
                if (s.Trim()[0] == (char)m_Arguments[ArgumentManager.Args.escapeChar])
                {
                    continue;
                }

                retVal.Add(s.Trim());
            }

            return retVal;
        }

        private void GetPaths(XmlNode childNode, ChangelogEntry cle)
        {
            foreach (XmlNode node in childNode.ChildNodes)
            {
                if (node.Name == "path")
                {
                    SvnPath path = new SvnPath();
                    path.Name = node.InnerText;

                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        switch (attr.Name)
                        { 
                            case "kind":
                                path.Kind = attr.Value;
                                break;
                            case "action":
                                path.Actions.Add(attr.Value);
                                break;
                            case "copyfrom-path":
                                path.FromPath = attr.Value;
                                break;
                            case "copyfrom-rev":
                                path.FromRevision = attr.Value;
                                break;
                        }
                    }

                    if (!cle.Paths.Contains(path))
                    {
                        cle.Paths.Add(path);
                    }
                    else
                    {
                        int index = cle.Paths.IndexOf(path);
                        if (!cle.Paths[index].Actions.Contains(path.Actions[0]))
                        {
                            cle.Paths[index].Actions.AddRange(path.Actions);
                        }
                    }
                }
            }
        }

        private string ConvertDate(string date)
        {
            DateTime dt = DateTime.Parse(date);

            return dt.ToShortDateString() + " " + dt.ToShortTimeString();
        }
    }
}