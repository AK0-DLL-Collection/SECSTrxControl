using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S1F17 : SXFY
    {

        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string CRST;         //R: On-line

        public S1F17()
        {
            StreamFunction = "S1F17";
            W_Bit = 1;
        }
    }
}
