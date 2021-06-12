using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S7F71 : SXFY
    {
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
        public string inlineid;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
        public string eqptid;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string portid;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string cstid;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string runmode;
        public S7F71() 
        {
            StreamFunction = "S7F71";
        }
    }
}
