using com.mirle.ibg3k0.stc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.Data.SecsData
{
    public class S9F1 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 10)]
        public string MHEAD;

        public S9F1()
        {
            StreamFunction = "S9F1";
            W_Bit = 0;
        }
    }
}
