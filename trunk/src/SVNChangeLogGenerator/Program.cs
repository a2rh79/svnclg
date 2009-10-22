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
            try
            {
                SvnExe svnExe = new SvnExe();

                Console.WriteLine();
                Console.WriteLine("Subversion Changelog Generator " + AssemblyVersion);
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine();

                Dictionary<ArgumentManager.Args, object> arguments = ArgumentManager.ProcessArgs(args);

                XmlDocument log = svnExe.GetLog(arguments);

                Console.WriteLine("Generating Changelog...");

                Generator clGen = new Generator();
                string outputLog = clGen.GenerateChangeLog(log, arguments);

                File.WriteAllText((string)arguments[ArgumentManager.Args.output], outputLog);
            
                Console.WriteLine("Finished.");
                Console.WriteLine();
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}