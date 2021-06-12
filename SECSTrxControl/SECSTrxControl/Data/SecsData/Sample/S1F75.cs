using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S1F75 : SXFY
    {
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string LINE_ID;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string EQPT_ID;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string PORT_ID;

        public S1F75() 
        {
            StreamFunction = "S1F75";
        }
    }
}
