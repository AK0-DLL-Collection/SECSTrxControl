using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S1F76 : SXFY
    {
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string LINE_ID;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 4)]
        public string EQPT_ID;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string PORT_ID;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string CST_ID;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
        public string IS_EMPTY;

        public S1F76() 
        {
            StreamFunction = "S1F76";
        }
    }
}
