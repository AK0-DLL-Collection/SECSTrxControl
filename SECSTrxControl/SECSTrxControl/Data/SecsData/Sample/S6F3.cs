using com.mirle.ibg3k0.stc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S6F3 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;

        public S6F3()
        {
            StreamFunction = "S6F3";
            W_Bit = 0;
            IsBaseType = true;
        }
    }

    public class S6F3_CEID500_501 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
        public string CEID;
        [SecsElement(Index = 2)]
        public PROCDATA DATA;

        public S6F3_CEID500_501()
        {
            StreamFunction = "S6F3";
            W_Bit = 0;
        }

        public class PROCDATA : SXFY
        {
            /// <summary>
            /// 如果是Lot Process Data，則留空白
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string UNITID;
            /// <summary>
            /// 如果是Lot Process Data，則留空白
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string SUNITID;
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CSTID;
            /// <summary>
            /// 如果是Lot Process Data，則留空白
            /// </summary>
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string GLSID;
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string OPERID;
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string PRODID;
            [SecsElement(Index = 8, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string PPID;
            [SecsElement(Index = 9)]
            public DVDATA[] DVDATAS;
        }

        public class DVDATA : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string DVNAME;
            [SecsElement(Index = 2)]
            public SUBITEM[] SUBITEMS;
        }

        public class SUBITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string SITENAME;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string DV;
        }

    }
}
