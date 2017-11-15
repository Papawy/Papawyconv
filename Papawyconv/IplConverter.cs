using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papawyconv
{
    class IplConverter
    {

        public class IPLConvertResult
        {
            public List<GTAObject> MapObjects = new List<GTAObject>();
            public int ErrorCount = 0;
            public int ObjectsWithoutCustomID = 0;
        }

        public class IPLConvertOptions
        {
            public double moddx = 0.0;
            public double moddy = 0.0;
            public double moddz = 0.0;
            public double drawd = -1;
            public double streamd = -1;
        }

        public static IPLConvertResult ToCreateDynamicObject(string file, string outputPwn, IdeConverter.IDEConvertResult ideResult, bool appendPawnFile = false, bool acceptWithoutCustomID = false, IPLConvertOptions options = null)
        {
            StreamReader iplReader = null;
            StreamWriter streamWriter = null;

            try
            {
                iplReader = new StreamReader(file);
                streamWriter = new StreamWriter(outputPwn, appendPawnFile);
            }
            catch
            {
                Console.WriteLine("ERROR : error while openning IPL file. Check your paths !");
                return null;
            }

            if (Utils.VerboseOpt)
                Console.WriteLine($"Reading and parsing {file} ...");

            bool insideInstSect = false;

            IPLConvertResult result = new IPLConvertResult();

            uint lineCount = 0;

            while (!iplReader.EndOfStream)
            {
                string line = iplReader.ReadLine();
                lineCount += 1;

                if (line == null)
                    continue;

                if (line == "inst")
                {
                    insideInstSect = true;
                    continue;
                }

                if (insideInstSect && line == "end")
                    insideInstSect = false;

                if (insideInstSect == false)
                    continue;

                string[] lineParams = line.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (lineParams.Length != 11)
                    continue;

                try
                {
                    uint LegacyID = UInt32.Parse(lineParams[0]);

                    GTAObject tmpObj = null ;

                    if (ideResult != null)
                    {
                        var res = ideResult.Objects.Find(obj => obj.LegacyID == LegacyID);
                        if(res != null)
                        {
                            tmpObj = new GTAObject();
                            tmpObj.Clone(res);
                        }
                    }

                    if (tmpObj == null)
                    {
                        result.ObjectsWithoutCustomID += 1;

                        if(ideResult != null)
                            if(!acceptWithoutCustomID)
                                continue;

                        tmpObj = new GTAObject();

                        tmpObj.LegacyID = LegacyID;

                        tmpObj.ModelName = lineParams[1];
                        tmpObj.DffName = lineParams[1] + ".dff";
                        tmpObj.TxdName = lineParams[1] + ".txd";

                        tmpObj.DrawDist = 0;

                        tmpObj.SAMPID = (int)LegacyID;
                    }

                    tmpObj.InteriorID = UInt32.Parse(lineParams[2]);

                    tmpObj.posX = double.Parse(lineParams[3], CultureInfo.InvariantCulture) + (options == null ? 0 : options.moddx);
                    tmpObj.posY = double.Parse(lineParams[4], CultureInfo.InvariantCulture) + (options == null ? 0 : options.moddy);
                    tmpObj.posZ = double.Parse(lineParams[5], CultureInfo.InvariantCulture) + (options == null ? 0 : options.moddz);

                    double tmpRotX = float.Parse(lineParams[6], CultureInfo.InvariantCulture);
                    double tmpRotY = float.Parse(lineParams[7], CultureInfo.InvariantCulture);
                    double tmpRotZ = float.Parse(lineParams[8], CultureInfo.InvariantCulture);
                    double tmpRotW = float.Parse(lineParams[9], CultureInfo.InvariantCulture);

                    // Utils.QuatToEuler(tmpRotX, tmpRotY, tmpRotZ, tmpRotW, ref tmpObj.rotX, ref tmpObj.rotY, ref tmpObj.rotZ);
                    Quaternions.EulerRot objRot = new Quaternions.EulerRot();
                    if (Utils.QuatOpt)
                        objRot = Quaternions.JS.ToEulerAngles(tmpRotX, tmpRotY, tmpRotZ, tmpRotW);
                    else
                        objRot = Quaternions.MTA.ToEulerAngles(tmpRotX, tmpRotY, tmpRotZ, tmpRotW);


                    tmpObj.rotX = objRot.x;
                    tmpObj.rotY = objRot.y;
                    tmpObj.rotZ = objRot.z;

                    tmpObj.IsMapObject = true;

                    if(options != null)
                    {
                        if (options.drawd != -1)
                            tmpObj.DrawDist = options.drawd;
                        if (options.streamd != -1)
                            tmpObj.StreamDist = options.streamd;
                    }

                    if (result.MapObjects.FindAll(obj => obj.SAMPID == tmpObj.SAMPID
                            && obj.posX == tmpObj.posX
                            && obj.posY == tmpObj.posY
                            && obj.posZ == tmpObj.posZ 
                            && obj.rotX == tmpObj.rotX
                            && obj.rotY == tmpObj.rotY
                            && obj.rotZ == tmpObj.rotZ).Count > 0)
                    {
                        Console.WriteLine($"\tIPL Duplicate at line {lineCount}.");
                    }

                    result.MapObjects.Add(tmpObj);
                }
                catch(Exception e)
                {
                    result.ErrorCount += 1;
                    if(Utils.VerboseOpt)
                        Console.WriteLine($"\t[IDE] Error at line {lineCount}.\n\tExcept : {e.Message}");
                }
            }

            iplReader.Close();

            if (Utils.VerboseOpt)
            {
                Console.WriteLine($"\tFound {result.MapObjects.FindAll(obj => obj.IsMapObject == true).Count} map objects.\n\tErrors : {result.ErrorCount}\n\tObjects without custom IDs : {result.ObjectsWithoutCustomID}");
                if(ideResult != null)
                    Console.WriteLine($"\t{ideResult.Objects.FindAll(obj => obj.IsMapObject == false).Count} objects are not on the map !");
            }

            if (Utils.VerboseOpt)
                Console.Write($"\nWriting SAMP Pawn Streamer Code in {outputPwn} ...");

            foreach (GTAObject obj in result.MapObjects)
            {
                streamWriter.WriteLine($"CreateDynamicObject({obj.SAMPID}, {obj.posX.ToString("F", CultureInfo.InvariantCulture)}, {obj.posY.ToString("F", CultureInfo.InvariantCulture)}, {obj.posZ.ToString("F", CultureInfo.InvariantCulture)}, " +
                    $"{obj.rotX.ToString("F", CultureInfo.InvariantCulture)}, {obj.rotY.ToString("F", CultureInfo.InvariantCulture)}, {obj.rotZ.ToString("F", CultureInfo.InvariantCulture)}, -1, {obj.InteriorID}, -1," +
                    $"{(obj.StreamDist == 0 ? "STREAMER_OBJECT_SD" : obj.StreamDist.ToString("F2", CultureInfo.InvariantCulture))}, {(obj.DrawDist == 0 ? "STREAMER_OBJECT_DD" : obj.DrawDist.ToString("F2", CultureInfo.InvariantCulture))});" +
                    $" // {obj.ModelName}");
            }

            streamWriter.Close();

            if (Utils.VerboseOpt)
                Console.Write(" Done !\n");

            return result;
        }
    }
}
