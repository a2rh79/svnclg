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
