using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cudafy;
namespace Cudafy1
{
    [Cudafy(eCudafyType.Struct)]
    public struct ImageStruct
    {
        public byte[,] Image;
        // true for frame false for database
        public bool Staus;
        public int ImageID;
        public bool Matched;
        
    }
    public struct ImageList
    {
        public ImageStruct[] Images;
    }
}
