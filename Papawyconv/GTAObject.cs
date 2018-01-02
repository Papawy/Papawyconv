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
        public int SAMPID = -1000;

        public string ModelName = "";
        public string DffName = "";
        public string TxdName = "";

        public uint MeshCount = 1;

        public int VirtualWord = -1;
        public uint InteriorID = 0;

        public double DrawDist = 0;
        public double StreamDist = 0;

        public double posX;
        public double posY;
        public double posZ;

        public double rotX;
        public double rotY;
        public double rotZ;

        public int LOD = -1;

        public UInt32 IDEFlags = 0;

        public bool Timed = false;

        public int TimeOn = 0;
        public int TimeOff = 0;

        public void Clone(GTAObject other, bool clonePos = true)
        {
            this.IsMapObject = other.IsMapObject;

            this.LegacyID = other.LegacyID;
            this.SAMPID = other.SAMPID;

            this.ModelName = other.ModelName;
            this.DffName = other.DffName;
            this.TxdName = other.TxdName;

            this.MeshCount = other.MeshCount;
            this.VirtualWord = other.VirtualWord;
            this.InteriorID = other.InteriorID;

            this.DrawDist = other.DrawDist;
            this.StreamDist = other.StreamDist;

            if(clonePos)
            {
                this.posX = other.posX;
                this.posY = other.posY;
                this.posZ = other.posZ;

                this.rotX = other.rotX;
                this.rotY = other.rotY;
                this.rotZ = other.rotZ;
            }

        }
    }
}
