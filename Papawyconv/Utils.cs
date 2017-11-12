using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Papawyconv
{
    public static class Utils
    {
        public static bool VerboseOpt = false;

        public static bool QuatOpt = false;

        static public int GetLatestIDFromArtconf(string artconfFile)
        {
            if(VerboseOpt)
                Console.WriteLine("Detecting Last SAMP ID from the artconfig.txt ...");

            string[] lines = { "" };

            try
            {
                lines = File.ReadAllLines(artconfFile);
            }
            catch
            {
                return -1;
            }
            

            Regex addSimpleModelRegex = new Regex(@"^(?:AddSimpleModel\()([0-9A-Za-z\-.,_\""\s\/\\]+)\)[;]");

            List<int> sampIds = new List<int>();

            foreach (string line in lines.Reverse<string>())
            {

                if (addSimpleModelRegex.IsMatch(line))
                {
                    Match rgxMatch;
                    rgxMatch = addSimpleModelRegex.Match(line);
                    
                    if (rgxMatch.Groups.Count == 2)
                    {
                        try
                        {
                            string[] lineParams = rgxMatch.Groups[1].Value.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            sampIds.Add(int.Parse(lineParams[2]));
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            sampIds.Sort(delegate (int x, int y)
            {
                if (x == y) return 0;
                if (x > y) return 1;
                else return 0;
            });

            if (VerboseOpt)
            {
                Console.WriteLine($"\tDone ! {(sampIds.Count != 0 ? "Last ID : "+sampIds[0]+"\n" : "\n")}");
                Console.Out.Flush();
            }

            return sampIds.Count != 0 ? sampIds[0] : -2000;
        }
    }
}
