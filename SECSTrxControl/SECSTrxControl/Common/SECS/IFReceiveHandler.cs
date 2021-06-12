using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.stc.Common.SECS
{
    public interface IFReceiveHandler
    {
        SXFY processHandler(SECSHandler secsHandler, 
            int lID, QSACTIVEXLib.EVENT_ID lMsgID, int S, int F, int W_Bit,
            int ulSystemBytes, Object rawData, Object head, string pEventText);

    }


    public class SampleReceiveHandler : IFReceiveHandler
    {
        public SXFY processHandler(SECSHandler secsHandler,
            int lID, QSACTIVEXLib.EVENT_ID lMsgID, int S, int F, int W_Bit,
            int ulSystemBytes, Object rawData, Object head, string pEventText) 
        {
            return null;
        }
    }

}
