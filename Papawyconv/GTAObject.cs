using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papawyconv
{
    public class GTAObject
    {
        public bool IsMapObject = false;

        public uint LegacyID = 0;
        public int SAMPID = -2000;

        public string ModelName = "";
        public string DffName = "";
        public string TxdName = "";

        public uint MeshCount = 1;

        public int VirtualWord = -1;
        public uint InteriorID = 0;

        public float DrawDist = 0;

        public double posX;
        public double posY;
        public double posZ;

        public double rotX;
        public double rotY;
        public double rotZ;

        public int LOD = -1;

        public UInt32 IDEFlags = 0;
    }
}
