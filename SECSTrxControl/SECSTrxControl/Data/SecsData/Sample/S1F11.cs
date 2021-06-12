using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S1F11 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
            ListElementLength = 5)]
        public string[] SVID;

        public S1F11()
        {
            StreamFunction = "S1F11";
            W_Bit = 1;
        }
    }
}
