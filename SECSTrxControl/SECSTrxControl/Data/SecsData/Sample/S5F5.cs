using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// List Alarms Data Request(LAR) (H -> E)
    /// </summary>
    public class S5F5 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public ALARMS[] ALLUNITALAM;

        public S5F5()
        {
            StreamFunction = "S5F5";
            W_Bit = 1;
        }

        public class ALARMS : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
                ListElementLength = 10)]
            public string[] ALIDS;
        }
    }
}
