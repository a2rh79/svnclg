using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVNChangeLogGenerator
{
    class ArgumentManager
    {
        public enum Args
        {
            repo,
            output,
            range,
            version,
            swap,
            help,
            ranges,
            additionalSvnArgs
        }

        public static Dictionary<Args, object> ProcessArgs(string[] args)
        {
            Dictionary<Args, object> arguments = new Dictionary<Args, object>();
            List<ChangelogEntry> ranges = new List<ChangelogEntry>();
            List<string> additionalSvnArgs = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                string[] tokens = args[i].Split('=');

                switch (tokens[0].ToLower())
                {
                    case "repo":
                        if (!String.IsNullOrEmpty(tokens[1]))
                        {
                            arguments.Add(Args.repo, tokens[1]);
                        }
                        break;
                    case "output":
                        if (!String.IsNullOrEmpty(tokens[1]))
                        {
                            arguments.Add(Args.output, tokens[1]);
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
                        arguments.Add(Args.swap, true);
                        break;
                    case "help":
                        Help.ShowHelp();
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
                else if (subTokens[0] == "all")
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
