using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S1F2 : SXFY
    {
        /// <summary>
        /// Equipment Model Type
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string MDLN;
        /// <summary>
        /// Software revision code
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string SOFTREV;

        public S1F2()
        {
            StreamFunction = "S1F2";
            W_Bit = 0;
        }
    }

    public class S1F2_Empty : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public string[] EMPTY;

        public S1F2_Empty()
        {
            StreamFunction = "S1F2";
            W_Bit = 0;
            IsBaseType = true;
            //EMPTY = new string[0];
        }

    }
}
