using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// Lot(Glass) Information Download (H -> E)
    /// </summary>
    public class S2F103 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 8)]
        public string MODULEID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string PTID;
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string LOTID;
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string CSTID;
        [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string PPID;
        [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 10)]
        public string OPERID;
        [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string WOID;
        [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string PRODID;
        [SecsElement(Index = 9, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string PRODCATE;
        [SecsElement(Index = 10, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string LOTJUDGE;
        [SecsElement(Index = 11, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string LSORTTYPE;
        [SecsElement(Index = 12, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string QTY;
        [SecsElement(Index = 13, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string THICKNESS;
        [SecsElement(Index = 14, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string ESFLAG;
        [SecsElement(Index = 15, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 25)]
        public string SLOTMAP;
        //****************************************************************************************************//
        //   1. <A[08] MODULEID>
        //	 2. <A[03] PTID>
        //	 3. <A[20] LOTID>
        //	 4. <A[20] CSTID>
        //	 5. <A[20] PPID>
        //	 6. <A[10] OPERID>
        //	 7. <A[20] WOID>
        //	 8. <A[20] PRODID>
        //	 9. <A[04] PRODCATE>
        //10. <A[01] LOTJUDGE>
        //11. <A[01] LSORTTYPE>
        //12. <A[02] QTY>
        //13. <A[05] THICKNESS>
        //14. <A[01] ESFLAG>
        //15. <A[25] SLOTMAP> 
        //****************************************************************************************************//
        [SecsElement(Index = 16)]
        public GLASSITEM[] GLASITEMS;   //. L, a        ※ a = number of Glass count

        [SecsElement(Index = 17)]
        public GLORDER[] GLASITEMS1;

        [SecsElement(Index = 18, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, ListElementLength = 20)]
        public string[] PAIRLOTID;


        //*************************************初始化*******************************************************//
        public S2F103()
        {
            StreamFunction = "S2F103";
            W_Bit = 1;
        }
        //************************************************************************************************// 

        public class GLASSITEM : SXFY               // 1. L, 8
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string SLOTNO;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string GLSID;
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GLSJUDGE;
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string GSORTTYPE;
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string SMPLFLAG;
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string RWKCNT;
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PAIRGLSID;
            [SecsElement(Index = 8)]    //8. L, n    ※ n = Number of Cell count
            public CELL[] GLASITEMS;   //. L, a        ※ a = number of Glass count

            //[SecsElement(Index = 8, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, ListElementLength = 0)]

            public class CELL : SXFY
            {
                [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string CELLID;
                [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
                public string CELLJUDGE;
            }

        }



        public class GLORDER : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 11)]
            public string GLASSORDER;
            [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, ListElementLength = 3)]
            public string[] SLOTNO;
            //public SOLOTNO[] GLASITEMS2;

            //public class SOLOTNO : SXFY
            //{
            //    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            //    public string SLOTNO;
            //}
        }

    }
}