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
    class IdeConverter
    {
        public class IDEConvertResult
        {
            public List<GTAObject> Objects = new List<GTAObject>();
            public int LastSAMPID = -1000;
            public int ErrorCount = 0;
        }

        public static IDEConvertResult ToArtconf(string file, bool appendArtconf, string artconfName = "artconfig.txt", bool noLod = false, string dir = "")
        {
            StreamReader ideReader = null;
            StreamWriter artconfWriter = null;

            int sampidCount = -1000;

            try
            {
                if (appendArtconf)
                {
                    sampidCount = Utils.GetLatestIDFromArtconf(artconfName);
                    if (sampidCount == -1)
                        sampidCount = -1000;
                    else
                        sampidCount -= 1;
                }

                ideReader = new StreamReader(file);
                if (appendArtconf)
                {
                    if(File.Exists(artconfName))
                        artconfWriter = new StreamWriter(artconfName, true);
                    else
                    {
                        artconfWriter = new StreamWriter(artconfName);
                    }

                }
                else
                    artconfWriter = new StreamWriter(artconfName);
            }
            catch
            {
                Console.WriteLine("ERROR : error while openning IDE file. Check your paths !");
                return null;
            }

            if (Utils.VerboseOpt)
                Console.WriteLine($"Reading and parsing {file} ...");

            IDEConvertResult result = new IDEConvertResult();

            bool insideObjsSect = false;

            uint lineCount = 0;

            while (!ideReader.EndOfStream)
            {
                string line = ideReader.ReadLine();
                lineCount += 1;

                if (line == null)
                    continue;

                if (line.StartsWith("#"))
                    continue;

                if (line == "objs")
                {
                    insideObjsSect = true;
                    continue;
                }

                if (insideObjsSect && line == "end")
                    insideObjsSect = false;

                if (insideObjsSect == false)
                    continue;

                string[] lineParams = line.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (lineParams.Length < 5 || lineParams.Length > 8)
                    continue;

                try
                {

                    if (noLod)
                        if (lineParams[1].StartsWith("lod") || lineParams[1].StartsWith("LOD"))
                            continue;

                    GTAObject tmpObj = new GTAObject();

                    tmpObj.LegacyID = tmpObj.LegacyID = UInt32.Parse(lineParams[0]);

                    tmpObj.ModelName = lineParams[1];

                    tmpObj.DffName = lineParams[1] + ".dff";
                    tmpObj.TxdName = lineParams[2] + ".txd";

                    switch(lineParams.Length)
                    {
                        case 5:
                            {
                                tmpObj.DrawDist = double.Parse(lineParams[3]);
                                tmpObj.IDEFlags = UInt32.Parse(lineParams[4]);
                                break;
                            }

                        case 6:
                            {
                                tmpObj.MeshCount = uint.Parse(lineParams[3]);
                                tmpObj.DrawDist = double.Parse(lineParams[4]);
                                tmpObj.IDEFlags = UInt32.Parse(lineParams[5]);
                                break;
                            }

                        case 7:
                            {
                                tmpObj.MeshCount = uint.Parse(lineParams[3]);
                                tmpObj.DrawDist = double.Parse(lineParams[4]);
                                tmpObj.IDEFlags = UInt32.Parse(lineParams[6]);
                                break;
                            }

                        case 8:
                            {
                                tmpObj.MeshCount = uint.Parse(lineParams[3]);
                                tmpObj.DrawDist = double.Parse(lineParams[4]);
                                tmpObj.IDEFlags = UInt32.Parse(lineParams[7]);
                                break;
                            }
                        default:
                            break;
                    }

                    result.Objects.Add(tmpObj);
                }
                catch(Exception e)
                {
                    result.ErrorCount += 1;
                    if(Utils.VerboseOpt)
                        Console.WriteLine($"\t[IDE] Error at line {lineCount}.\n\tExcept : {e.Message}");
                }
            }

            ideReader.Close();

            if (Utils.VerboseOpt)
                Console.WriteLine($"\tFound {result.Objects.Count} objects. Errors : {result.ErrorCount}\n");

            

            if (Utils.VerboseOpt)
                Console.Write($"Writing SAMP {artconfName} ...");
            
            foreach (GTAObject obj in result.Objects)
            {
                obj.SAMPID = sampidCount;
                artconfWriter.WriteLine($"AddSimpleModel(-1, 19379, {sampidCount}, \"{(dir == "" ? "" : dir+"/")}{obj.DffName}\", \"{(dir == "" ? "" : dir + "/")}{obj.TxdName}\");");
                sampidCount -= 1;
            }

            result.LastSAMPID = sampidCount;

            artconfWriter.Close();

            if (Utils.VerboseOpt)
                Console.Write(" Done !\n\n");

            return result;
        }
    }
}
