// Copyright © CharlieFirpo 2009 <Firpo.Charlie@googlemail.com>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace SVNChangeLogGenerator
{
    class Program
    {
        public static string AssemblyVersion { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        static void Main(string[] args)
        {

             // commands: range=continue version=1.0.*/1.0.*x
            try
            {
                string repoPath = "";
                string outputPath = "";
                List<string> additionalSvnArgs = new List<string>();
                Dictionary<string, object> additionalClgArgs = new Dictionary<string, object>();
                SvnExe svnExe = new SvnExe();

                Console.WriteLine();
                Console.WriteLine("Subversion Changelog Generator " + AssemblyVersion);
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine();

                GetArgs(args, ref repoPath, ref outputPath, additionalSvnArgs, additionalClgArgs);

                XmlDocument log = svnExe.GetLog(repoPath, additionalSvnArgs);

                Console.WriteLine("Generating Changelog...");

                Generator clGen = new Generator();
                string outputLog = clGen.GenerateLog(log, additionalClgArgs);

                File.WriteAllText(outputPath, outputLog);
            
                Console.WriteLine("Finished.");
                Console.WriteLine();
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void GetArgs(string[] args, ref string repoPath, ref string outputPath, List<string> additionalSvnArgs, Dictionary<string, object> additionalClgArgs)
        {
            List<ChangelogEntry> ranges = new List<ChangelogEntry>();

            for (int i = 0; i < args.Length; i++ )
            {
                string[] tokens = args[i].Split('=');

                switch (tokens[0].ToLower())
                {
                    case "repo":
                        if (!String.IsNullOrEmpty(tokens[1]))
                        {
                            repoPath = tokens[1];
                        }
                        break;
                    case "output":
                        if (!String.IsNullOrEmpty(tokens[1]))
                        {
                            outputPath = tokens[1];
                        }
                        break;
                    case "range":
                        if (!String.IsNullOrEmpty(tokens[1]))
                        {
                            string version = String.Empty;
                            if (args.Length > (i + 1))
                            {
                                version = args[i + 1];
                            }
                            ranges.Add(GetRangeAndVersion(tokens[1], version));
                        }
                        break;
                    case "version":
                        break;
                    case "swap":
                        additionalClgArgs.Add("Swap", true);
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    default:
                        additionalSvnArgs.Add(tokens[0]);
                        break;
                }
            }

            if (ranges.Count > 0)
            {
                additionalClgArgs.Add("Ranges", ranges);
            }
        }

        private static ChangelogEntry GetRangeAndVersion(string token, string version)
        {
            // Get Range
            string[] subTokens = token.Split('-');
            ChangelogEntry cle = new ChangelogEntry();

            int start;
            int.TryParse(subTokens[0], out start);
            cle.StartRevision = Math.Abs(start);
            if (subTokens.Length > 1)
            {
                int end;
                int.TryParse(subTokens[1], out end);
                cle.EndRevision = end;
            }
            else if (subTokens[0] == "all")
            {
                cle.StartRevision = -1;
            }

            // Get Version
            string[] ver = version.Split('=');
            if (ver[0] == "version")
            {
                if (!String.IsNullOrEmpty(ver[1]))
                {
                    cle.Version = ver[1];
                }
            }

            return cle;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: SvnClg.exe <Command>=<Parameter>");
            Console.WriteLine();
            Console.WriteLine("Note: Only messages beginning with an asterisk (*) will be considered as changelog entries!");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine("repo         - Path to the repository");
            Console.WriteLine("output       - Output file path");
            Console.WriteLine("range        - A range of revisions that shall be merged to one log entry");
            Console.WriteLine("               Parameters:");
            Console.WriteLine("               x-y         - Specifies a range from revision x to revision y");
            Console.WriteLine("               all         - Merges all revisions to one entry");
            //Console.WriteLine("     continue    - Merges all revisions since the last time SvnClg.exe was executed (stored in *.cfg)");
            Console.WriteLine("version      - The version of the preceding range");
            Console.WriteLine("swap         - Swaps the order of the entries");
            Console.WriteLine();
            Console.WriteLine("Note: The \"version\" command only works with a preceding \"range\" command.");
            Console.WriteLine();
            Console.WriteLine(@"e.g: SvnClg.exe repo=svn://MyRepo/trunk/Myproject output=D:\ChangeLog.txt range=1-100 version=1.0.0 range=101-200 version=2.0.0 swap");
            Console.WriteLine();
            Console.WriteLine("For help with StringTemplate, please refer to www.StringTemplate.org");
            
            Environment.Exit(1);
        }
    }
}
