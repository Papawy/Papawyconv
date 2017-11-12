using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papawyconv
{
    class Quaternions
    {
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


        public static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static class Matrix
        {
            public static double[,] MakeRotationFromQuaternion(double x, double y, double z, double w)
            {
                double x2 = x + x;
                double y2 = y + y;
                double z2 = z + z;

                double xx = x * x2;
                double xy = x * y2;
                double xz = x * z2;

                double yy = y * y2;
                double yz = y * z2;
                double zz = z * z2;

                double wx = w * x2;
                double wy = w * y2;
                double wz = w * z2;

                double[,] result = {
                    {1-(yy + zz), xy - wz, xz + wy, 0},
                    {xy+wz, 1 - (xx + zz), yz - wx, 0},
                    {xz-wy, yz+wx, 1-(xx+yy), 0},
                    {0, 0, 0, 1}
                };

                return result;
            }
        }

        public static class JS
        {
            public static EulerRot ToEulerAngles(double x, double y, double z, double w)
            {
                double[,] matRot = Matrix.MakeRotationFromQuaternion(x, y, z, w);

                EulerRot result = new EulerRot();

                result.y = Math.Asin(Clamp(matRot[0, 2], -1, 1));

                if (Math.Abs(matRot[0, 2]) < 0.99999999)
                {
                    result.x = Math.Atan2(-matRot[1, 2], matRot[2, 2]);
                    result.z = Math.Atan2(-matRot[0, 1], matRot[0, 0]);
                }
                else
                {
                    result.x = Math.Atan2(matRot[2, 1], matRot[2, 2]);
                    result.z = 0;
                }

                result.x = RadianToDegree(result.x);
                result.y = RadianToDegree(result.y);
                result.z = RadianToDegree(result.z);

                return result;
            }
        }

        public static class MTA
        {
            public static double[,] IdMat = {
                    { 1, 0, 0},
                    { 0, 1, 0},
                    { 0, 0, 1} };

            public static double Clamp(double value, double min, double max)
            {
                return (value < min) ? min : (value > max) ? max : value;
            }

            static public double[,] To3x3Mat(double x, double y, double z, double w)
            {
                double[,] mat = new double[3, 3];

                double[,] symmat = {
                    { (-(y*y)-(z*z)), x*y, x*z },
                    { x*y, (-(x*x)-(z*z)), y*z },
                    { x*z, y*z, (-(x*x)-(y*y))} };

                double[,] antisymmat = {
                    { 0, -z, y },
                    { z, 0, -x },
                    { -y, x, 0} };

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
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

                retval.x = RadianToDegree(Math.Asin(Clamp(matrix[1, 2], -1, 1)));
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
    }
}
