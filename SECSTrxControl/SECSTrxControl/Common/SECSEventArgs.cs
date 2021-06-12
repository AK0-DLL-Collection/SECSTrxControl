//*********************************************************************************
//      SECSEventArgs.cs
//*********************************************************************************
// File Name: SECSEventArgs.cs
// Description: SECS Event Args.
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/05/30    Hayes Chen     N/A            N/A     Initial Release
// 2014/11/06    Hayes Chen     N/A            A0.01   增加Cache SXFY
// 2021/06/07    Kevin Wei      N/A            A0.02   將Raw Data的資料，複製一份出來避免影響已在轉譯的內容
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.stc.Common
{
    public class SECSEventArgs : EventArgs
    {
        public int ID { get; private set; }
        public QSACTIVEXLib.EVENT_ID MsgID { get; private set; }
        public int S { get; private set; }
        public int F { get; private set; }
        public int W_Bit { get; private set; }
        public int SystemBytes { get; private set; }
        public Object RawData { get; private set; }
        public Object Head { get; private set; }
        public string EventText { get; private set; }
        public SECSHandler secsHandler { get; private set; }
        public int DeviceID { get; private set; }
        public SXFY BaseSXFYData { get; set; }                //A0.01
        public SXFY ExtendSXFYData { get; set; }              //A0.01
        public DateTime EventTime { get; private set; }

        public SECSEventArgs(int lID, int deviceID, QSACTIVEXLib.EVENT_ID lMsgID, int S, int F, int W_Bit,
            int ulSystemBytes, Object rawData, Object head, string pEventText, SECSHandler secsHandler)
        {
            this.ID = lID;
            this.DeviceID = deviceID;
            this.MsgID = lMsgID;
            this.S = S;
            this.F = F;
            this.W_Bit = W_Bit;
            this.SystemBytes = ulSystemBytes;
            //A0.02 this.RawData = rawData;
            this.RawData = CloneByteArray(rawData);//A0.02
            this.Head = head;
            this.EventText = pEventText;
            this.secsHandler = secsHandler;
            this.EventTime = DateTime.Now;
        }
        object CloneByteArray(object rawData)
        {
            if (rawData is Array)
            {
                Array source_array = rawData as Array;
                int lenth = source_array.Length;
                byte[] new_array = new byte[lenth];
                source_array.CopyTo(new_array, 0);
                return new_array;
            }
            else
            {
                return rawData;
            }
        }

        public string getEventTime()
        {
            return EventTime.ToString("yyyy-MM-dd HH:mm:ss.fffff");
        }

    }
}
