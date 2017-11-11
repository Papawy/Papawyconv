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

        /*static public void QuatToEuler(double qrotx, double qroty, double qrotz, double qrotw, ref double rotx, ref double roty, ref double rotz)
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
        }*/

        public static class Quaternions
        {
            public static double[,] IdMat= {
                    { 1, 0, 0},
                    { 0, 1, 0},
                    { 0, 0, 1} };

            public struct EulerRot
            {
                public double x;
                public double y;
                public double z;
            }

            public static double DegreeToRadian(double angle)
            {
                return Math.PI * angle / 180.0;
            }

            public static double RadianToDegree(double angle)
            {
                return angle * (180.0 / Math.PI);
            }

            static public double[,] To3x3Mat(double x, double y, double z, double w)
            {
                double[,] mat = new double[3,3];

                double[,] symmat = { 
                    { (-(y*y)-(z*z)), x*y, x*z },
                    { x*y, (-(x*x)-(z*z)), y*z },
                    { x*z, y*z, (-(x*x)-(y*y))} };

                double[,] antisymmat = {
                    { 0, -z, y },
                    { z, 0, -x },
                    { -y, x, 0} };

                for (int i = 0; i<3; i++)
                {
                    for(int j = 0; j<3; j++)
                    {
                        mat[i, j] = IdMat[i, j] + (2 * symmat[i, j]) + (2 * w * antisymmat[i, j]);
                    }
                }

                return mat;
            }

            static public EulerRot GetEulerAnglesFromMatrix(double[,] matrix)
            {
                double nz1, nz2, nz3;

                nz3 = Math.Sqrt(matrix[1, 0] * matrix[1, 0] + matrix[1, 1] * matrix[1, 1]);
                nz1 = (-matrix[1, 0] * matrix[1, 2]) / nz3;
                nz2 = (-matrix[1, 1] * matrix[1, 2]) / nz3;

                double vx = nz1 * matrix[0, 0] + nz2 * matrix[0, 1] + nz3 * matrix[0, 2];
                double vz = nz1 * matrix[2, 0] + nz2 * matrix[2, 1] + nz3 * matrix[2, 2];

                EulerRot retval = new EulerRot();

                retval.x = RadianToDegree(Math.Asin(matrix[1, 2]));
                retval.y = -RadianToDegree(Math.Atan2(vx, vz));
                retval.z = -RadianToDegree(Math.Atan2(matrix[1, 0], matrix[1, 1]));

                return retval;
            }

            static public EulerRot ToEulerAngles(double x, double y, double z, double w)
            {
                double[,] mat = To3x3Mat(x, y, z, w);
                return GetEulerAnglesFromMatrix(mat);
            }

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
