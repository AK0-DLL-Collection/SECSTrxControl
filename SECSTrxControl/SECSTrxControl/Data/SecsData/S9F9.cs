using com.mirle.ibg3k0.stc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.Data.SecsData
{
    public class S9F9 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 10)]
        public string SHEAD;

        public S9F9() 
        {
            StreamFunction = "S9F9";
            W_Bit = 0;
        }
    }
}
