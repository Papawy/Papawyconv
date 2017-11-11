using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;

using CommandLine;
using CommandLine.Text;

namespace Papawyconv
{
    class Program
    {
        class Options
        {
            [Option('v', "verbose", HelpText = "Print details during execution.")]
            public bool Verbose { get { return Utils.VerboseOpt; } set { Utils.VerboseOpt = value; } }

            [Option("ide", HelpText = "The IDE file to convert.", DefaultValue = "")]
            public string IDEFile { get; set; }

            [Option("art", HelpText = "The artconf file that will be generated. Need an IDE file.", DefaultValue = "artconfig.txt")]
            public string ArtconfFile { get; set; }

            [Option("app", HelpText = "Append lines in the artconf generated file from an IDE conversion.", DefaultValue = false)]
            public bool AppendArtConf { get; set; }

            [Option("ipl", HelpText = "The IPL file to convert. Work better with an IDE conversion in the same execution.", DefaultValue = "")]
            public string IPLFile { get; set; }

            [Option("pwn", HelpText = "The pawn file that will be generated. Need an IPL file.", DefaultValue = "streamer.pwn")]
            public string Pawnfile { get; set; }

            [Option("acc", HelpText = "For an IPL conversion, accept objects without custom SAMP ID.", DefaultValue = false)]
            public bool AcceptWithoutCustomID { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                HelpText help = new HelpText
                {
                    Heading = new HeadingInfo("Papawyconf", "0.1.3"),
                    AdditionalNewLineAfterOption = true,
                    AddDashesToOption = true
                };
                help.AddPreOptionsLine("Apache Licence 2.0");
                help.AddPreOptionsLine("Usage: papawyconf --ide ideFile < --art artconf.txt >");
                help.AddPreOptionsLine("Usage: papawyconf --ipl ideFile < --pwn streamer.pwn --acc >");
                help.AddOptions(this);
                return help;
            }

        }

        static void Main(string[] args)
        {
            Options opt = new Options();

            if(CommandLine.Parser.Default.ParseArguments(args, opt))
            {
                if(opt.IDEFile == "" && opt.IPLFile == "")
                {
                    Console.WriteLine(opt.GetUsage());
                    return;
                }

                IdeConverter.IDEConvertResult ideResult = null;

                if(opt.IDEFile != "")
                {
                    ideResult = IdeConverter.ToArtconf(opt.IDEFile, opt.AppendArtConf, opt.ArtconfFile);
                }

                if(opt.IPLFile != "")
                {
                    IplConverter.ToCreateDynamicObject(opt.IPLFile, opt.Pawnfile, ideResult, opt.AcceptWithoutCustomID);
                }
            }
            else
            {
                Console.WriteLine(opt.GetUsage());
            }

            
        }
    }
}
