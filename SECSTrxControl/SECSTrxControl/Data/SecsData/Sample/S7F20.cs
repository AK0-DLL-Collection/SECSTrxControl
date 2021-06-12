using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// Current EPPD Data
    /// </summary>
    public class S7F20 : SXFY
    {
        /// <summary>
        /// Equipment ID or Unit ID or Sub-Unit ID
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string UNITID;
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string PPTYPE;
        [SecsElement(Index = 3, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, ListElementLength = 40)]
        public string[] PPIDLIST;
        [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string VERSION;
        //public PPIDItem[] PPIDLIST;

        public S7F20()
        {
            StreamFunction = "S7F20";
            W_Bit = 0;
        }

        //public class PPIDItem : SXFY
        //{
        //    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        //    public string PPID;
        //}
    }
}
