using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S2F41 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string RCMD;

        public S2F41()
        {
            StreamFunction = "S2F41";
            IsBaseType = true;
        }
    }

    public class S2F41_RCMD1_5 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string RCMD;
        [SecsElement(Index = 2)]
        public CMD CMDINOF;

        public S2F41_RCMD1_5()
        {
            StreamFunction = "S2F41";
            StreamFunctionName = "HOST Command Send (HCS)";
            W_Bit = 1;
        }

        public class CMD : SXFY
        {
            [SecsElement(Index = 1)]
            public PORT PORTINFO;
            [SecsElement(Index = 2)]
            public CST CSTINFO;
            [SecsElement(Index = 3)]
            public LOT LOTINFO;

            public class PORT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
                public string NAME = "PTID";
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string PTID;
            }
            public class CST : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
                public string NAME = "CSTID";
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string CSTID;
            }
            public class LOT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
                public string NAME = "LOTID";
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string LOTID;
            }
        }
    }

    public class S2F41_RCMD6 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string RCMD;
        [SecsElement(Index = 2)]
        public CMD CMDINOF;

        public S2F41_RCMD6()
        {
            StreamFunction = "S2F41";
            RCMD = SECSConst.RCMD_Operator_Call;
            CanAbort = false;
            W_Bit = 1;
        }

        public class CMD : SXFY
        {
            [SecsElement(Index = 1)]
            public OPCALL CALLINFO;

            public class OPCALL : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
                public string NAME = "OPCALL";
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 80)]
                public string MESSAGE;
            }
        }
    }

    public class S2F41_RCMD7 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string RCMD;
        [SecsElement(Index = 2)]
        public CMD CMDINOF;

        public S2F41_RCMD7()
        {
            StreamFunction = "S2F41";
            RCMD = SECSConst.RCMD_Mask_CST_Cancel;
            W_Bit = 1;
        }

        public class CMD : SXFY
        {
            [SecsElement(Index = 1)]
            public PORT PORTINFO;
            [SecsElement(Index = 2)]
            public CST CSTINFO;
            [SecsElement(Index = 3)]
            public UNIT UNITINFO;
            [SecsElement(Index = 4)]
            public SUBUNIT SUNITINFO;

            public class PORT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
                public string NAME = "PTID";
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string PTID;
            }

            public class CST : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
                public string NAME = "CSTID";
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string CSTID;
            }

            public class UNIT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
                public string NAME = "UNITID";
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string UNITID;
            }

            public class SUBUNIT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 7)]
                public string NAME = "SUNITID";
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string SUNITID;
            }
        }
    }
}
