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

            [Option("app", HelpText = "Append lines in the artconf generated file from an IDE conversion or the pawn file in an IPL conversion.", DefaultValue = false)]
            public bool AppendArtConf { get; set; }

            [Option("ipl", HelpText = "The IPL file to convert. Work better with an IDE conversion in the same execution.", DefaultValue = "")]
            public string IPLFile { get; set; }

            [Option("pwn", HelpText = "The pawn file that will be generated. Need an IPL file.", DefaultValue = "streamer.pwn")]
            public string Pawnfile { get; set; }

            [Option("acc", HelpText = "For an IPL conversion, accept objects without custom SAMP ID.", DefaultValue = false)]
            public bool AcceptWithoutCustomID { get; set; }

            [Option("nolod", HelpText = "Remove LOD objects, they are sometimes no needed.", DefaultValue = false)]
            public bool NoLod { get; set; }

            [Option("dir", HelpText = "Set a custom directory for models during an IDE conversion.", DefaultValue = "")]
            public string ModelsDir { get; set; }

            [Option("addx", HelpText = "Add x units to all objects positions. Usefull for entire maps placement. Only during an IPL conversion.", DefaultValue = 0.0)]
            public double AddX { get; set; }

            [Option("addy", HelpText = "Add y units to all objects positions. Usefull for entire maps placement. Only during an IPL conversion.", DefaultValue = 0.0)]
            public double AddY { get; set; }

            [Option("addz", HelpText = "Add z units to all objects positions. Usefull for entire maps placement. Only during an IPL conversion.", DefaultValue = 0.0)]
            public double AddZ { get; set; }

            [Option("drawdist", HelpText = "Set a drawdistance for all objects. Take effect during an IPL conversion.", DefaultValue = -1)]
            public double DrawDistance { get; set; }

            [Option("streamdist", HelpText = "Set a streamdistance for all objects. Take effect during an IPL conversion.", DefaultValue = -1)]
            public double StreamDistance { get; set; }

            [Option("quat", HelpText = "false = MTA Usefull funcs QuatsToEuler method | true = three.js QuasToEuler method.", DefaultValue = false)]
            public bool QuatOpt { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                HelpText help = new HelpText
                {
                    Heading = new HeadingInfo("Papawyconf", "0.1.7"),
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

                Utils.QuatOpt = opt.QuatOpt;

                IdeConverter.IDEConvertResult ideResult = null;

                if(opt.IDEFile != "")
                {
                    ideResult = IdeConverter.ToArtconf(opt.IDEFile, opt.AppendArtConf, opt.ArtconfFile, opt.NoLod, opt.ModelsDir);
                }

                if(opt.IPLFile != "")
                {
                    IplConverter.IPLConvertOptions iplopt = new IplConverter.IPLConvertOptions();

                    iplopt.moddx = opt.AddX;
                    iplopt.moddy = opt.AddY;
                    iplopt.moddz = opt.AddZ;

                    iplopt.drawd = opt.DrawDistance;
                    iplopt.streamd = opt.StreamDistance;

                    IplConverter.ToCreateDynamicObject(opt.IPLFile, opt.Pawnfile, ideResult, opt.AppendArtConf, opt.AcceptWithoutCustomID, iplopt);
                }
            }
            else
            {
                Console.WriteLine(opt.GetUsage());
            }

            
        }
    }
}
