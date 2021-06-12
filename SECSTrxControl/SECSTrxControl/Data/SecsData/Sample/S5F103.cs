using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// Current Alarm Set List Request (H -> E)
    /// </summary>
    public class S5F103 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public UNITITEM[] UNITS;

        public S5F103()
        {
            StreamFunction = "S5F103";
            W_Bit = 1;
        }

        public class UNITITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
        }
    }
}
