using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// Formatted Status Data
    /// </summary>
    public class S1F6 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD;

        public S1F6()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
            IsBaseType = true;
        }
    }

    /// <summary>
    /// Equipment Status Request
    /// </summary>
    public class S1F6_SFCD1 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Equipment_Status_Request;
        [SecsElement(Index = 2)]
        public RPTITEM RPTINFO;

        public S1F6_SFCD1()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CRST;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EQST;
        }
    }

    /// <summary>
    /// Port Status Request (Not Crate Port)
    /// </summary>
    public class S1F6_SFCD2 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Port_Status_Request;
        [SecsElement(Index = 2)]
        public RPTITEM[] RPTINFO;

        public S1F6_SFCD2()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        /// <summary>
        /// If one cassette has more than two lots, ‘LOTID’, ‘PPID’ and ‘LOTST’ should be empty.
        /// </summary>
        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string PTID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PTTYPE;
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PTUSETYPE;
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string TRSMODE;
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string PTST;
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CSTID;
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string PPID;
            [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string SLOTINFO;
        }
    }

    /// <summary>
    /// Operation Status Request
    /// </summary>
    public class S1F6_SFCD3 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Operation_Mode_Request;
        [SecsElement(Index = 2)]
        public RPTITEM RPTINFO;

        public S1F6_SFCD3()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string OPERMODE;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string OPERMODEDESC;
        }
    }

    /// <summary>
    /// Unit Status Request
    /// </summary>
    public class S1F6_SFCD4 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Unit_Status_Request;
        [SecsElement(Index = 2)]
        public RPTITEM[] RPTINFO;

        public S1F6_SFCD4()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string UNITST;
            [SecsElement(Index = 3)]
            public GLSITEM[] GLSINFO;

            public class GLSITEM : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
                public string USLOTNO;
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string GLSID;
            }
        }
    }

    /// <summary>
    /// Sub-Unit Status Request
    /// </summary>
    public class S1F6_SFCD5 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Sub_Unit_Status_Request;
        [SecsElement(Index = 2)]
        public RPTITEM[] RPTINFO;

        public S1F6_SFCD5()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2)]
            public SUBUNITITEM[] SUBUNITINFO;

            public class SUBUNITITEM : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string SUNITID;
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                public string SUNITST;
                [SecsElement(Index = 3)]
                public GLSITEM[] GLSINFO;

                public class GLSITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string SSLOTNO;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string GLSID;
                }

            }
        }
    }

    /// <summary>
    /// Mask Status Request
    /// </summary>
    public class S1F6_SFCD6 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Mask_Status_Request;
        [SecsElement(Index = 2)]
        public RPTITEM[] RPTINFO;

        public S1F6_SFCD6()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2)]
            public MASKITEM[] MASKINFO;
            [SecsElement(Index = 3)]
            public SUBUNITITEM[] SUBUNITINFO;

            public class MASKITEM : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
                public string MASKID;
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                public string MASKST;
                [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
                public string MASKUSECNT;
            }

            public class SUBUNITITEM : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string SUNITID;
                [SecsElement(Index = 2)]
                public MASKITEM[] MASKINFO;

                public class MASKITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
                    public string SUNITMASKID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string SUNITMASKST;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
                    public string SUNITMASKUSECNT;
                }

            }
        }
    }

    /// <summary>
    /// Material Status Request
    /// </summary>
    public class S1F6_SFCD7 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Material_Status_Request;
        [SecsElement(Index = 2)]
        public RPTITEM[] RPTINFO;

        public S1F6_SFCD7()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string MATERIALTYPE;
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string MATERIALID;
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string MATERIALST;
        }
    }

    /// <summary>
    /// Sorter Job List Request
    /// </summary>
    public class S1F6_SFCD8 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Sorter_Job_List_Request;
        [SecsElement(Index = 2)]
        public RPTITEM RPTINFO;

        public S1F6_SFCD8()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string SORTERJOBID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string SORTERJOBST;

        }
    }

    /// <summary>
    /// Crate Port Status Request
    /// </summary>
    public class S1F6_SFCD9 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD = SECSConst.SFCD_Crate_Port_Status_Request;
        [SecsElement(Index = 2)]
        public RPTITEM[] RPTINFO;

        public S1F6_SFCD9()
        {
            StreamFunction = "S1F6";
            W_Bit = 0;
        }

        public class RPTITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string PTID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PTTYPE;
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PTUSETYPE;
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string TRSMODE;
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string PTST;
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CSTID;
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CRATEID;
        }
    }

}
