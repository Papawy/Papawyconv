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

        static public void QuatToEuler(double qrotx, double qroty, double qrotz, double qrotw, ref double rotx, ref double roty, ref double rotz)
        {
            // roll (x-axis rotation)
            double sinr = +2.0 * (qrotw * qrotx + qroty * qrotz);
            double cosr = +1.0 - 2.0 * (qrotx * qrotx + qroty * qroty);
            rotx = Math.Atan2(sinr, cosr);

            // pitch (y-axis rotation)
            double sinp = +2.0 * (qrotw * qroty - qrotz * qrotx);
            if (Math.Abs(sinp) >= 1)

                roty = (Math.PI / 2) * Math.Sign(sinp); // use 90 degrees if out of range
            else
                roty = Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny = +2.0 * (qrotw * qrotz + qrotx * qroty);
            double cosy = +1.0 - 2.0 * (qroty * qroty + qrotz * qrotz);
            rotz = Math.Atan2(siny, cosy);
        }

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
            

            Regex addSimpleModelRegex = new Regex(@"^(?:AddSimpleModel\()([0-9A-Za-z\-.,_\""\s]+)\)[;]");

            List<int> sampIds = new List<int>();

            foreach(string line in lines.Reverse<string>())
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

            if(VerboseOpt)
            {
                Console.WriteLine($"\tDone ! Last ID : {sampIds[0]}\n");
                Console.Out.Flush();
            }
                
            return sampIds.Count != 0 ? sampIds[0] : -2000;
        }
    }
}
