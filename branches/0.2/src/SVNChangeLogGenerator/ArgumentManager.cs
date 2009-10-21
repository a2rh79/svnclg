using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SVNChangeLogGenerator
{
    class ArgumentManager
    {
        public enum Args
        {
            repo,                   // string
            output,                 // string
            ranges,                 // List<ChangeLogEntry>
            swap,                   // bool
            additionalSvnArgs,      // List<string>
            svnPath,                // string
            escapeChar,             // char
            textWidth               // int
        }

        public static Dictionary<Args, object> ProcessArgs(string[] args)
        {
            Dictionary<Args, object> arguments = new Dictionary<Args, object>();
            List<ChangelogEntry> ranges = new List<ChangelogEntry>();
            List<string> additionalSvnArgs = new List<string>();

            LoadConfigFromFile(arguments);

            // override config with command line arguments
            for (int i = 0; i < args.Length; i++)
            {
                string[] tokens = args[i].Split('=');

                switch (tokens[0].ToLower())
                {
                    case "repo":
                        if (!String.IsNullOrEmpty(tokens[1]))
                        {
                            arguments[Args.repo] = tokens[1];
                        }
                        break;
                    case "output":
                        if (!String.IsNullOrEmpty(tokens[1]))
                        {
                            arguments[Args.output] = tokens[1];
                        }
                        break;
                    case "range":
                        if (!String.IsNullOrEmpty(tokens[1]))
                        {
                            string version = String.Empty;
                            if (args.Length > (i + 1))
                            {
                                string[] ver = args[i + 1].Split('=');
                                if (ver[0] == "version" && !String.IsNullOrEmpty(ver[1]))
                                {
                                    version = ver[1];
                                }
                            }
                            ranges.AddRange(GetChangeLogEntries(tokens[1], version));
                        }
                        break;
                    case "version":
                        break;
                    case "swap":
                        arguments[Args.swap] = true;
                        break;
                    case "help":
                        Help.ShowHelp();
                        break;
                    case "nowrap":
                        arguments[Args.textWidth] = -1;
                        break;
                    default:
                        additionalSvnArgs.Add(tokens[0]);
                        break;
                }
            }

            if (ranges.Count > 0)
            {
                arguments.Add(Args.ranges, ranges);
            }
            if (additionalSvnArgs.Count > 0)
            {
                arguments.Add(Args.additionalSvnArgs, additionalSvnArgs);
            }

            return arguments;
        }

        private static void LoadConfigFromFile(Dictionary<Args, object> arguments)
        {
            string svnPath = System.Configuration.ConfigurationManager.AppSettings["svnPath"];
            if (File.Exists(svnPath))
            {
                arguments.Add(Args.svnPath, svnPath);
            }
            string repoPath = System.Configuration.ConfigurationManager.AppSettings["repoPath"];
            arguments.Add(Args.repo, repoPath);
            string escChar = System.Configuration.ConfigurationManager.AppSettings["escapeChar"];
            arguments.Add(Args.escapeChar, Convert.ToChar(escChar));
            int textWidth;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["textWidth"], out textWidth);
            arguments.Add(Args.textWidth, (int)textWidth);
        }

        private static List<ChangelogEntry> GetChangeLogEntries(string token, string version)
        {
            // Get Range
            string[] ranges = token.Split(',');
            string[] versions = version.Split(',');
            List<ChangelogEntry> entries = new List<ChangelogEntry>();

            for (int i = 0; i < ranges.Length; i++)
            {
                string[] subTokens = ranges[i].Split('-');
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
                else if (subTokens[0] == "all" || subTokens[0] == "...")
                {
                    cle.StartRevision = -1;
                }

                // Get Version
                if (i < versions.Length)
                {
                    string[] ver = versions[i].Split('.');

                    cle.Version.AddRange(ver);
                }

                entries.Add(cle);
            }

            return entries;
        }
    }
}
