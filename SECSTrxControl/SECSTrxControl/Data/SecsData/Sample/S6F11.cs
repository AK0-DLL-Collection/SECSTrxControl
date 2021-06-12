using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// Event Report Send
    /// </summary>
    public class S6F11 : SXFY
    {


        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;

        public S6F11()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
            IsBaseType = true;
        }
    }

    /// <summary>
    /// Event Report Send: Operation mode Status Change
    /// </summary>
    public class S6F11_CEID104 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;


        public S6F11_CEID104()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
            CEID = SECSConst.CEID_Oper_Mode_Status_Change;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTMODE MODE;
        }

        public class RPTSTAT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "100";
            [SecsElement(Index = 2)]
            public STATITEM STATINFO;
        }

        public class STATITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CRST;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EQST;
        }

        public class RPTMODE : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "104";
            [SecsElement(Index = 2)]
            public MODEITEM MODEINFO;
        }

        public class MODEITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string OPERMODE;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string OPERMODEDESC;
        }
    }

    /// <summary>
    /// Event Report Send: Unit Status Change
    /// </summary>
    public class S6F11_CEID105 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID105()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
            CEID = SECSConst.CEID_Unit_Status_Change;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTUNIT UNIT;
        }

        public class RPTSTAT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "100";
            [SecsElement(Index = 2)]
            public STATITEM STATINFO;
        }

        public class STATITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CRST;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EQST;
        }

        public class RPTUNIT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "105";
            [SecsElement(Index = 2)]
            public UNITITEM UNITINFO;
        }

        public class UNITITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string UNITST;
        }

    }

    /// <summary>
    /// Event Report Send: Sub-Unit Status Change
    /// </summary>
    public class S6F11_CEID106 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID106()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
            CEID = SECSConst.CEID_Sub_Unit_Status_Change;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTUNIT UNIT;
        }

        public class RPTSTAT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "100";
            [SecsElement(Index = 2)]
            public STATITEM STATINFO;
        }

        public class STATITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CRST;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EQST;
        }

        public class RPTUNIT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "106";
            [SecsElement(Index = 2)]
            public UNITITEM UNITINFO;
        }

        public class UNITITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string UNITST;
            [SecsElement(Index = 3)]
            public SUBUNITITEM SUBUNIT;
        }

        public class SUBUNITITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string SUBITID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string SUNITST;
        }

    }

    /// <summary>
    /// Event Report Send: Material State Change
    /// </summary>
    public class S6F11_CEID108 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID108()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTMATER MATER;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTMATER : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "108";
                [SecsElement(Index = 2)]
                public MATERITEM MATERINFO;

                public class MATERITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string UNITID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
                    public string MATERIALID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string MATERIALST;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string MATERIALTYPE;
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Equipment Constant Change
    /// </summary>
    public class S6F11_CEID109 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID109()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTECID ECID;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTECID : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "109";
                [SecsElement(Index = 2)]
                public ECIDITEM[] ECIDLIST;

                public class ECIDITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
                    public string ECID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
                    public string ECV;
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Ready To Start
    /// </summary>
    public class S6F11_CEID110 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID110()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
            CEID = SECSConst.CEID_Ready_To_Start;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public STARTINFO STARTCMD;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }

            }

            public class STARTINFO : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "110";
                [SecsElement(Index = 2)]
                public PORTINFO PORT;

                public class PORTINFO : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string PTID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PTTYPE;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PTUSETYPE;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string TRSMODE;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CSTID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string LOTID;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string SLOTMAP;
                }
            }
        }




    }

    /// <summary>
    /// Event Report Send: Control State Change & Equipment Status Change
    /// </summary>
    public class S6F11_CEID111_114 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID111_114()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
        }

        public class RPTSTAT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "100";
            [SecsElement(Index = 2)]
            public STATITEM STATINFO;
        }

        public class STATITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CRST;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EQST;
        }

    }

    /// <summary>
    /// Event Report Send: Operator Confirm Event about Operator Call Command
    /// </summary>
    public class S6F11_CEID118 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID118()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTMSG MSG;

            public class RPTMSG : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "118";
                [SecsElement(Index = 2)]
                public MSGITEM MSGINFO;

                public class MSGITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 80)]
                    public string MESSAGE;
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Port Status & Transfer Mode Change
    /// </summary>
    public class S6F11_CEID200_209 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID200_209()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT EQSTATUS;
            [SecsElement(Index = 2)]
            public PORTSTAT PORTSTATUS;
        }

        public class RPTSTAT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "100";
            [SecsElement(Index = 2)]
            public STATITEM STATINFO;
        }

        public class STATITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CRST;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EQST;
        }

        public class PORTSTAT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "200";
            [SecsElement(Index = 2)]
            public PORTSTATITEM PORTSTATINFO;
        }

        public class PORTSTATITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string PTID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PTTYPE;
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PTUSETYPE;
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string TRSMODE;
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CSTID;
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string SLOTMAP;
        }

    }

    /// <summary>
    /// Event Report Send: Crate Port Status & Transfer Mode Change
    /// </summary>
    public class S6F11_CEID210_219 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID210_219()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT EQSTATUS;
            [SecsElement(Index = 2)]
            public PORTSTAT PORTSTATUS;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class PORTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "210";
                [SecsElement(Index = 2)]
                public PORTSTATITEM PORTSTATINFO;

                public class PORTSTATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string PTID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PTTYPE;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PTUSETYPE;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string TRSMODE;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CRATEID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
                    public string GLSTHK;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string GLSSIZE;
                    [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string MAKER;
                    [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string CRATEQTY;
                }
            }


        }
    }

    /// <summary>
    /// Event Report Send: Mask Cassette Port Status & Transfer Mode Change
    /// </summary>
    public class S6F11_CEID220_229 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID220_229()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT EQSTATUS;
            [SecsElement(Index = 2)]
            public PORTSTAT PORTSTATUS;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class PORTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "220";
                [SecsElement(Index = 2)]
                public PORTSTATITEM PORTSTATINFO;

                public class PORTSTATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string PTID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PTTYPE;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PTUSETYPE;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string TRSMODE;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CSTID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string SLOTMAP;
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Process status change report
    /// </summary>
    public class S6F11_CEID301_307 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID301_307()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public PROCINFO GLS;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class PROCINFO : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "301";
                [SecsElement(Index = 2)]
                public DATA ITEM;

                public class DATA : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string LOTID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string PTID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PTTYPE;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PTUSETYPE;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CSTID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                    public string PPID;
                }
            }
        }

    }

    /// <summary>
    /// Event Report Send: Last Glass Process Start
    /// </summary>
    public class S6F11_CEID311 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID311()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
            CEID = SECSConst.CEID_Last_Glass_Process_Start;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTGLS GLS;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTGLS : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "311";
                [SecsElement(Index = 2)]
                public GLSITEM GLSINFO;

                public class GLSITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string PTID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string LOTID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string GLSID;
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Glass Out/In
    /// </summary>
    public class S6F11_CEID321_326 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID321_326()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTGLS GLS;
        }

        public class RPTSTAT : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "100";
            [SecsElement(Index = 2)]
            public STATITEM STATINFO;
        }

        public class STATITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string CRST;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EQST;
        }

        public class RPTGLS : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string RPTID = "321";
            [SecsElement(Index = 2)]
            public GLSITEM GLSINFO;
        }

        public class GLSITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string SUNITID;
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string IPTID;
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string OPTID;
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string ICSTID;
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string OCSTID;
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string PPID;
            [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string FSLOTNO;
            [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string TSLOTNO;
            [SecsElement(Index = 11, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string GLSID;
            [SecsElement(Index = 12, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSJUDGE;
            [SecsElement(Index = 13, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSGRADE;
            [SecsElement(Index = 14, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string SSLOTNO;
            [SecsElement(Index = 15, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string MASKID;
        }
    }

    /// <summary>
    /// Event Report Send: Cassette Out/In Unit
    /// </summary>
    public class S6F11_CEID327_328 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID327_328()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTCST CST;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTCST : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "327";
                [SecsElement(Index = 2)]
                public CSTITEM CSTINFO;

                public class CSTITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string UNITID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string SUNITID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string LOTID;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string LOTJUDGE;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CSTID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string IPTID;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string OPTID;
                    [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                    public string PPID;
                }
            }
        }




    }

    /// <summary>
    /// Event Report Send: Glass Scrap / Unscrap
    /// </summary>
    public class S6F11_CEID331_332 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID331_332()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTSCRAP SCRAP;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTSCRAP : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "331";
                [SecsElement(Index = 2)]
                public SCRAPITEM SCRAPINFO;

                public class SCRAPITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string UNITID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string LOTID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string PTID;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CSTID;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string SLOTNO;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string GLSID;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string GLSJUDGE;
                    [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
                    public string SCRAPCODE;
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Glass Turn
    /// </summary>
    public class S6F11_CEID334 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID334()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTGLS GLS;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTGLS : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "334";
                [SecsElement(Index = 2)]
                public GLSITEM GLSINFO;

                public class GLSITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string UNITID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string LOTID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string PTID;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CSTID;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string SLOTNO;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string GLSID;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string GLSJUDGE;
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Mask Out/In (Evaporation, Mask AMHS EQ only)
    /// </summary>
    public class S6F11_CEID341_348 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID341_348()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTMASK MASK;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTMASK : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "341";
                [SecsElement(Index = 2)]
                public MASKITEM MASKINFO;

                public class MASKITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string UNITID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string SUNITID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
                    public string MASKID;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                    public string PPID;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string IPTID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string OPTID;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string ICSTID;
                    [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string OCSTID;
                    [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string FSLOTNO;
                    [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string TSLOTNO;
                    [SecsElement(Index = 11, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string SSLOTNO;
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Process Program or Recipe Change
    /// </summary>
    public class S6F11_CEID401 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID401()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTPPID RPTPPIDDATA;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTPPID : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "401";
                [SecsElement(Index = 2)]
                public PPIDITEM PPIDINFO;

                public class PPIDITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                    public string PPID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string PPTYPE;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string PPCINFO;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 14)]
                    public string LCTIME;
                    //                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    //                    public string UNITID;
                    [SecsElement(Index = 5)]
                    public RECIPE[] RECIPEIFO;

                    public class RECIPE : SXFY
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                        public string CCODE;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                        public string RCPSTEP;
                        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                        public string UNITID;
                        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                        public string SUNITID;
                        [SecsElement(Index = 5)]
                        public PPARM[] PPARMINFO;

                        public class PPARM : SXFY
                        {
                            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                            public string PPARMNAME;
                            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                            public string PPARMVALUE;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Event Report Send: Assemble Complete Event
    /// </summary>
    public class S6F11_CEID411 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID411()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTLTPSGLS RPTLTPSGLSDATA;
            [SecsElement(Index = 3)]
            public RPTENCPGLS RPTENCPGLSDATA;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            /// <summary>
            /// LTPS Glass Information
            /// </summary>
            public class RPTLTPSGLS : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "411";
                [SecsElement(Index = 2)]
                public GLSITEM GLSINFO;

                public class GLSITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string UNITID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string LOTID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CSTID;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string SLOTNO;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string GLSID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string RGLSID;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string GLSJUDGE;
                    [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string GLSGRADE;
                }
            }

            /// <summary>
            /// Encap Glass Information
            /// </summary>
            public class RPTENCPGLS : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "413";
                [SecsElement(Index = 2)]
                public GLSITEM GLSINFO;

                public class GLSITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string PAIRLOTID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string PAIRCSTID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                    public string PAIRSLOTNO;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string PAIRGLSID;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string PAIRRGLSID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string PAIRGLSJUDGE;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string PAIRGLSGRADE;
                }
            }

        }
    }

    /// <summary>
    /// Event Report Send: Glass Cut Process
    /// </summary>
    public class S6F11_CEID412 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID412()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTPPID RPTPPIDDATA;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTPPID : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "412";
                [SecsElement(Index = 2)]
                public GLSITEM GLSINFO;

                public class GLSITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string PTID;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string CSTID;
                    [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string LOTID;
                    [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
                    public string PPID;
                    [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string OPERID;
                    [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string PRODID;
                    [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string LOTJUDGE;
                    [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                    public string SLOTNO;
                    [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string GLSID;
                    [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string GLSTYPE;
                    [SecsElement(Index = 11, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string GLSJUDGE;
                    [SecsElement(Index = 12)]
                    public CUTGLS[] CUTGLSINFO;

                    public class CUTGLS : SXFY
                    {
                        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                        public string CUTGLSID;
                        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                        public string CUTGLSJUDGE;
                        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                        public string CUTGLSGRADE;
                        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                        public string CUTGLSX;
                        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
                        public string CUTGLSY;
                    }
                }
            }
        }
    }

    /// <summary>
    ///  Report Send: Sorting Job Event
    /// </summary>
    public class S6F11_CEID431_436 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string DATAID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 3)]
        public RPTINFO INFO;

        public S6F11_CEID431_436()
        {
            StreamFunction = "S6F11";
            W_Bit = 1;
        }

        public class RPTINFO : SXFY
        {
            [SecsElement(Index = 1)]
            public RPTSTAT STATUS;
            [SecsElement(Index = 2)]
            public RPTSORT RPTSORTDATA;

            public class RPTSTAT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "100";
                [SecsElement(Index = 2)]
                public STATITEM STATINFO;

                public class STATITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string CRST;
                    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
                    public string EQST;
                }
            }

            public class RPTSORT : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
                public string RPTID = "431";
                [SecsElement(Index = 2)]
                public SORTITEM SORTINFO;

                public class SORTITEM : SXFY
                {
                    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                    public string SORTERJOBID;
                }
            }
        }
    }
}
