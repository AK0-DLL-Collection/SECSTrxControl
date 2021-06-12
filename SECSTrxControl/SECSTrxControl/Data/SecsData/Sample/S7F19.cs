using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// Current EPPD Request (H->E)
    /// </summary>
    public class S7F19 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string UNITID;
        /// <summary>
        /// Process Program Type
        /// E: Equipment
        /// U: Unit
        /// S: Sub-Unit
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string PPTYPE;

        public S7F19()
        {
            StreamFunction = "S7F19";
            W_Bit = 1;
        }
    }
}
