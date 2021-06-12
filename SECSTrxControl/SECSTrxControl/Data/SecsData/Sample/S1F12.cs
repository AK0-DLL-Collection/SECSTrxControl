using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S1F12 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public SVITEM[] SVIDS;

        public S1F12()
        {
            StreamFunction = "S1F12";
            W_Bit = 0;
        }

        public class SVITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
            public string SVID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string SVNAME;
        }
    }
}
