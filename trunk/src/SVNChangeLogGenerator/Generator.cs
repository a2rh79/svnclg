// Copyright © CharlieFirpo 2009 <Firpo.Charlie@googlemail.com>

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

        public string GenerateChangeLog(XmlDocument log, Dictionary<ArgumentManager.Args, object> arguments)
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
                if ((bool)arguments[ArgumentManager.Args.swap])
                    clEntries.Reverse();
            }

            if (arguments.ContainsKey(ArgumentManager.Args.iswap))
            {
                if ((bool)arguments[ArgumentManager.Args.iswap])
                {
                    foreach (ChangelogEntry cle in clEntries)
                    {
                        cle.Msg.Reverse();
                    }
                }
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
                        if (msg.Count > 0)
                            cle.Msg.Add(msg);
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
                    logEntry.SetAttribute("paths", cle.Paths);
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
            int textWidth = 70;

            if (m_Arguments.ContainsKey(ArgumentManager.Args.textWidth))
            {
                textWidth = (int)m_Arguments[ArgumentManager.Args.textWidth];
            }

            foreach (string s in messages)
            {
                if (s.Trim()[0] == Convert.ToChar(m_Arguments[ArgumentManager.Args.escapeChar]))
                {
                    continue;
                }

                if (textWidth == -1)    // nowrap
                {
                    retVal.Add(s);
                }
                else
                {
                    retVal.AddRange(WrapText(s, textWidth));
                }
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

        private static List<string> WrapText(string text, int width)
        {
            text = text.Replace(Environment.NewLine, " ");

            string[] words = text.Split(' ');
            string line = String.Empty;
            List<string> wrappedText = new List<string>();

            foreach (string word in words)
            {
                if (line.Length + word.Length <= width)
                {
                    line += word + " ";
                }
                else
                {
                    wrappedText.Add(line.Trim());
                    line = word + " ";
                }
            }
            wrappedText.Add(line.Trim());

            return wrappedText;
        }
    }
}