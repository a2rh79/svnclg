// Copyright © CharlieFirpo 2009 <Firpo.Charlie@googlemail.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace SVNChangeLogGenerator
{
    class SvnExe
    {
        private string m_SvnPath = "svn.exe";
        private readonly string m_SvnLogArguments = "log --xml -v ";

        public XmlDocument GetLog(Dictionary<ArgumentManager.Args, object> arguments)
        {
            Process svnProc = new Process();

            if (arguments.ContainsKey(ArgumentManager.Args.svnPath))
            {
                m_SvnPath = (string)arguments[ArgumentManager.Args.svnPath];
            }

            svnProc.StartInfo.FileName = m_SvnPath;
            svnProc.StartInfo.Arguments = GetLogArguments(arguments);
            svnProc.StartInfo.RedirectStandardInput = true;
            svnProc.StartInfo.RedirectStandardOutput = true;
            svnProc.StartInfo.RedirectStandardError = true;
            svnProc.StartInfo.UseShellExecute = false;
            svnProc.StartInfo.CreateNoWindow = true;

            Console.WriteLine("Starting Subversion...");

            svnProc.Start();

            Console.WriteLine("Retrieving Log...");

            string output = svnProc.StandardOutput.ReadToEnd();

            svnProc.WaitForExit();
            svnProc.Close();
            svnProc.Dispose();

            XmlDocument log = new XmlDocument();
            log.InnerXml = output;

            return log;
        }

        private string GetLogArguments(Dictionary<ArgumentManager.Args, object> arguments)
        {
            string repoPath = (string)arguments[ArgumentManager.Args.repo];
            string args = m_SvnLogArguments + repoPath;

            if (arguments.ContainsKey(ArgumentManager.Args.additionalSvnArgs))
            {
                List<string> additionalSvnArgs = (List<string>)arguments[ArgumentManager.Args.additionalSvnArgs];

                foreach (string addSvnArg in additionalSvnArgs)
                {
                    args += " " + addSvnArg;
                }
            }

            return args;
        }
    }
}
