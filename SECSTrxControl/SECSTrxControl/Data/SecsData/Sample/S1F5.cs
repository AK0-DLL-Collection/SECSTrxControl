using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    /// <summary>
    /// Formatted Status Request
    /// </summary>
    public class S1F5 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string SFCD;

        public S1F5()
        {
            StreamFunction = "S1F5";
            W_Bit = 1;
        }
    }
}
