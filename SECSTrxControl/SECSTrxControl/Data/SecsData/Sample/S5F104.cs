using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// Current Alarm Set List Data (H <- E)
    ///  The order of alarm list should be the latest order. 
    /// </summary>
    public class S5F104 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public RPTITEM[] RPTDATA;

        public S5F104()
        {
            StreamFunction = "S5F104";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
                ListElementLength = 10)]
            public string[] ALARMIDS;
        }
    }
}
