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
    /// Formatted Process Program Request (H -> E)
    /// </summary>
    public class S7F25 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        public string PPID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string UNITID;
        [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string SUNITID;
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string PPTYPE;

        public S7F25() 
        {
            StreamFunction = "S7F25";
            W_Bit = 1;
        }
    }
}
