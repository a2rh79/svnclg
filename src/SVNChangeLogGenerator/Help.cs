using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVNChangeLogGenerator
{
    class Help
    {
        public static void ShowHelp()
        {
            Console.WriteLine("Usage: SvnClg.exe <Command>=<Parameter>");
            Console.WriteLine();
            Console.WriteLine("Note:");
            Console.WriteLine("SVN log entries that should be ignored when generating the ChangeLog can be marked by a preceding special character.");
            Console.WriteLine("By default this is a minus ('-'). It can be changed in the .config file (key=\"escapeChar\").");
            Console.WriteLine("This way you can easily sort out the entries you don't want to have in the ChangeLog? when committing changes.");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine("repo             - Path to the repository. Use: repo=svn://path/to/repo");
            Console.WriteLine("output/o         - Output file path. Use: output=D:\\ChangeLog.txt");
            Console.WriteLine("svnpath          - Path to the svn.exe. This is not needed if svn.exe is already registered within the OS. Use: svnpath=C:\\path\\to\\svn.exe");
            Console.WriteLine("range/r          - A range of revisions that shall be merged to one log entry. Multiple ranges can be separated by commas. Use: range=x-y or range=all or range=... or r=x-y,x2-y2,...");
            Console.WriteLine("                   Parameters:");
            Console.WriteLine("                   x-y         - Specifies a range from revision x to revision y");
            Console.WriteLine("                   all / ...   - Merges all revisions to one entry");
            Console.WriteLine("version/v        - The version of the preceding range. Use: version=1.0.0 or version=1.0.* or v=1.0.5*");
            Console.WriteLine("                   Parameters:");
            Console.WriteLine("                   *         - will be automatically replaced by a 0 and will be incremented by 1 with each given range, i.e. 1.1.0, 1.1.1, ... ");
            Console.WriteLine("                   x*        - The version number will be incremented by 1, starting with the given number x");
            Console.WriteLine("swap             - Allows to swap the order of the entries. Use: swap=true");
            Console.WriteLine("                   Parameters:");
            Console.WriteLine("                   true         - The order will be swapped");
            Console.WriteLine("                   false        - The order will not be swapped");
            Console.WriteLine("innerswap/iswap  - Allows to swap the order of the log messages within a changelog entry. Use: iswap=true");
            Console.WriteLine("                   Parameters:");
            Console.WriteLine("                   true         - The order will be swapped");
            Console.WriteLine("                   false        - The order will not be swapped");
            Console.WriteLine("textwidth/tw     - Specifies the text width. A value of -1 means the text will not be wrapped. Use: tw=70");
            Console.WriteLine("escapeChar/esc   - Specifies the character that's used to ignore particular SVN log entries. Use: esc=-");
            Console.WriteLine("help             - Shows this help page");
            Console.WriteLine();
            Console.WriteLine("Note: The \"version\" command only works with a preceding \"range\" command.");
            Console.WriteLine();
            Console.WriteLine(@"Example: <location>\SvnClg.exe repo=http://svnclg.googlecode.com/svn/trunk/ o=D:\ChangeLog.txt r=1-100,101-200 v=1.0.0,2.0.0");
            Console.WriteLine();
            Console.WriteLine("Config file SvnClg.exe.config:");
            Console.WriteLine("Most of the above commands can be specified as default values in the .config file.");
            Console.WriteLine("These default settings will be overwritten when the accordant command line arguments are used.");
            Console.WriteLine();
            Console.WriteLine("For more detailed help please visit the project page: http://code.google.com/p/svnclg/");
            Console.WriteLine();
            Console.WriteLine("For help with StringTemplate, please refer to http://www.StringTemplate.org");

            Environment.Exit(1);
        }

    }
}
