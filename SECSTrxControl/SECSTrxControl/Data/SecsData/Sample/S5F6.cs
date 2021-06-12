using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// List Alarm Data(LAD)
    /// </summary>
    public class S5F6 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public REPORTITEM[] RPTDATA;

        public S5F6()
        {
            StreamFunction = "S5F6";
            W_Bit = 0;
        }

        public class REPORTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2)]
            public ALARM[] ALARMS;

            public class ALARM : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                public string ALCD;
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
                public string ALID;
                [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 80)]
                public string ALTX;
            }

        }

    }
}
