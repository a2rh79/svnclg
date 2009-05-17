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

        public XmlDocument GetLog(string repoPath, IEnumerable<string> additionalSvnArgs)
        {
            Process svnProc = new Process();

            svnProc.StartInfo.FileName = m_SvnPath;
            svnProc.StartInfo.Arguments = GetLogArguments(repoPath, additionalSvnArgs);
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

        private string GetLogArguments(string repoPath, IEnumerable<string> additionalSvnArgs)
        {
            string args =  m_SvnLogArguments + repoPath;

            foreach (string addSvnArg in additionalSvnArgs)
            {
                args += " " + addSvnArg;
            }

            return args;
        }
    }
}
