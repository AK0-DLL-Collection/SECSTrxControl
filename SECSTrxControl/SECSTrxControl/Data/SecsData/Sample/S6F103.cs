using com.mirle.ibg3k0.stc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S6F103 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string PTID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string PTUSETYPE;
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string PTTYPE;
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string CSTID;
        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string QTY;
        [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CRATEQTY;
        [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string SLOTSEL;
        [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string SLOTMAP;
        [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string CSTENDFLAG;
        [SecsElement(Index = 10)]
        public GLSITEM[] GLSITEMS;

        public S6F103()
        {
            StreamFunction = "S6F103";
            W_Bit = 1;
        }

        public class GLSITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string OPERID;
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PRODID;
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string LOTJUDGE;
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSST;
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string SLOTNO;
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string GLSID;
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string PPID;
            [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string RGLSID;
            [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSTYPE;
            [SecsElement(Index = 11, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSIDTYPE;
            [SecsElement(Index = 12, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSJUDGE;
            [SecsElement(Index = 13, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSGRADE;
            [SecsElement(Index = 14, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
            public string PAIRSLOTNO;
            [SecsElement(Index = 15, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRPRODID;
            [SecsElement(Index = 16, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRPRODTYPE;
            [SecsElement(Index = 17, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRGLSID;
            [SecsElement(Index = 18, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRRGLSID;
            [SecsElement(Index = 19, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string PAIRGLSJUDGE;
            [SecsElement(Index = 20, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string PAIRGLSGRADE;
            [SecsElement(Index = 21, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string WORKORDER;
            [SecsElement(Index = 22, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CRATEID;
            [SecsElement(Index = 23, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string MAKER;
            [SecsElement(Index = 24, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
            public string GLSTHK;
            [SecsElement(Index = 25, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSSIZE;
            [SecsElement(Index = 26, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string SMPLFLAG;
            [SecsElement(Index = 27, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string RWKCNT;
            [SecsElement(Index = 28, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
            public string DUMUSEDCNT;
            [SecsElement(Index = 29, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 30)]
            public string MASKID;
            [SecsElement(Index = 30)]
            public UNITITEM[] UNITITEMS;
            [SecsElement(Index = 31, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PROBERID;
            [SecsElement(Index = 32, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GCFLAG;
            [SecsElement(Index = 33, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
            public string GCUNIT;
            [SecsElement(Index = 34, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string EVASMPLFLAG;
            [SecsElement(Index = 35, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string PANELJUDGE;
            [SecsElement(Index = 36, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string ARRAYREPAIRTYPE;
            [SecsElement(Index = 37, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 800)]
            public string LCVDREPAIRTYPE;

            public class UNITITEM : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string UNITID;
                [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII,
                        ListElementLength = 20)]
                public string[] SUNITIDS;


                //[SecsElement(Index = 2)]
                //public SUBUNITITEM[] SUBUNITITEMS;

                //public class SUBUNITITEM : SXFY 
                //{
                //    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                //    public string SUNITID;
                //}
            }
        }
    }
}
