using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class S2F21 : SXFY
    {
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string LDNAME;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 2)]
        public string PORTID;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string CSTTYPE;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 6)]
        public string CSTID;
        [SecsElement(Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 5)]
        public string CMD;

        public S2F21() 
        {
            StreamFunction = "S2F21";
        }
    }
}
