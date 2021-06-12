//*********************************************************************************
//      SECSConnection.cs
//*********************************************************************************
// File Name: SECSConnection.cs
// Description: SECS Connection
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/05/30    Hayes Chen     N/A            N/A     Initial Release
// 2015/08/28    Kevin Wei      N/A            A0.01    暫時將會把SECS內容記錄成.XML檔的部分，先拿掉。
// 2015/08/31    Kevin Wei      N/A            A0.02    增加T3 TimeOut的通知
// 2015/11/02    Kevin Wei      N/A            A0.03    修正再發生DISCONNECTED並非直接startToListen
//                                                      ，而是會先停止T5的時間後再執行refreshConnection。
// 2015/11/10    Kevin Wei      N/A            A0.04    由於使用A0.03的寫法會導致影響到PLC Read的掃描週期(他會跟著T5時間延後)，
//                                                      因此借此修正。
// 2016/01/12    Kevin Wei      N/A            A0.05    在Fun:doSendSECSIIMessage，增加Try Catch。
// 2016/08/22    Kevin Wei      N/A            A0.06    加入對SECS各個操作的Return Code做紀錄。
// 2017/04/12    Kevin Wei      N/A            A0.07    修改SECS斷線後的處理機制。原本的會在斷線後就先關掉SECS的連線，但這樣會導致Keeping Time
//                                                      的功能失效，現在修改成開始計時T5時間，若超過這時間還沒連回來才進行重啟的動作。
// 2016/03/09    Kevin Wei      N/A            A0.08    將Thread.Sleep的用法改成SpinWait()
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Data.SecsData;
using NLog;
using QSACTIVEXLib;
using System.IO;

namespace com.mirle.ibg3k0.stc.Common.SECS
{
    public class SECSConnection : HostConnection
    {
        private static Logger logger = NLog.LogManager.GetLogger("SECSTrxLogger");
        private static Logger SXFYLogger = NLog.LogManager.GetLogger("SXFYLogger");
        //        private int deviceID = 0;
        private QSACTIVEXLib.QSWrapper qsWrapper;
        public QSACTIVEXLib.QSWrapper QSWrapper { get { return qsWrapper; } }
        private SECSHandler secsHandler = null;

        private QSACTIVEXLib.HSMS_COMM_MODE connect_Mode = QSACTIVEXLib.HSMS_COMM_MODE.HSMS_ACTIVE_MODE;
        private QSACTIVEXLib.COMMMODE comm_Mode = QSACTIVEXLib.COMMMODE.HSMS_MODE;
        private int deviceID = 0;
        private int linkTestPeriod = 60;
        private int logEnable = 1;
        private int flowControlEnable = 0;
        private int localPort = 7000;
        private int remotePort = 7000;
        private string localIP = "";
        private string remoteIP = "";
        private int t3Timeout = 30;
        private int t5Timeout = 5;
        private int t6Timeout = 5;//5;
        private int t7Timeout = 5;//5;
        private int t8Timeout = 5;//5;
        private _IQSWrapperEvents_QSEventEventHandler qsHandler = null;

        public SECSConnection(SECSAgent secsAgent, string localIP, int localPort, string remoteIP, int remotePort,
            QSACTIVEXLib.HSMS_COMM_MODE connectMode,
            int t3Timeout, int t5Timeout, int t6Timeout, int t7Timeout, int t8Timeout,
            int linkTestTimer, int deviceID)
        {
            logger.Debug("Create SECSConnection");
            logger.Info("Create SECSConnection Start,[Name:{0}]", secsAgent.Name); //A0.06
            this.secsAgent = secsAgent;
            this.connect_Mode = connectMode;
            this.deviceID = deviceID;
            this.localPort = localPort;
            this.remotePort = remotePort;
            this.localIP = localIP;
            this.remoteIP = remoteIP;
            this.t3Timeout = t3Timeout;
            this.t5Timeout = t5Timeout;
            this.t6Timeout = t6Timeout;
            this.t7Timeout = t7Timeout;
            this.t8Timeout = t8Timeout;
            this.linkTestPeriod = linkTestTimer;
            secsHandler = new SECSHandler(secsAgent);
            qsHandler = new QSACTIVEXLib._IQSWrapperEvents_QSEventEventHandler(QSEventHandler);
            createQSWrapper();
            #region
            //qsWrapper = new QSACTIVEXLib.QSWrapper();
            //qsWrapper.HSMS_Connect_Mode = connectMode;// QSACTIVEXLib.HSMS_COMM_MODE.HSMS_ACTIVE_MODE;
            ////qsWrapper.HSMS_Connect_Mode = QSACTIVEXLib.HSMS_COMM_MODE.HSMS_PASSIVE_MODE;
            //qsWrapper.lCOMM_Mode = QSACTIVEXLib.COMMMODE.HSMS_MODE;
            //qsWrapper.lDeviceID = deviceID;
            //this.deviceID = deviceID;
            //qsWrapper.lLinkTestPeriod = 60;
            //qsWrapper.lLogEnable = 1;
            //qsWrapper.lFlowControlEnable = 0;
            //qsWrapper.nLocalPort = localPort;
            //qsWrapper.nRemotePort = remotePort;
            //qsWrapper.szLocalIP = localIP;
            //qsWrapper.szRemoteIP = remoteIP;
            //qsWrapper.T3 = t3Timeout;
            //qsWrapper.T5 = t5Timeout;//5;
            //qsWrapper.T6 = t6Timeout;//5;
            //qsWrapper.T7 = t7Timeout;//5;
            //qsWrapper.T8 = t8Timeout;//5;
            //secsHandler = new SECSHandler(secsAgent);
            //qsWrapper.QSEvent += new QSACTIVEXLib._IQSWrapperEvents_QSEventEventHandler(QSEventHandler);
            #endregion
            logger.Info("Create SECSConnection End,[Name:{0}]", secsAgent.Name);        //A0.06
            Console.WriteLine("Create SECSConnection [Name:{0}]", secsAgent.Name);
        }

        private void createQSWrapper()
        {
            if (qsWrapper != null)
            {
                try
                {
                    qsWrapper.QSEvent -= qsHandler;

                    int actionResult = -1;                                         //A0.06
                    actionResult = qsWrapper.Stop();
                    logger.Info("SECS QS, Device ID:{0} Stop action,Result:{1}"    //A0.06
                        , deviceID                                                 //A0.06
                        , actionResult);                                           //A0.06

                    actionResult = qsWrapper.Destroy();
                    logger.Info("SECS QS, Device ID:{0} Destroy action,Result:{1}" //A0.06
                        , deviceID                                                 //A0.06
                        , actionResult);                                           //A0.06

                    qsWrapper = null;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
            qsWrapper = new QSACTIVEXLib.QSWrapper();
            qsWrapper.HSMS_Connect_Mode = connect_Mode;
            qsWrapper.lCOMM_Mode = comm_Mode;
            qsWrapper.lDeviceID = deviceID;
            qsWrapper.lLinkTestPeriod = linkTestPeriod;
            qsWrapper.lLogEnable = logEnable;
            qsWrapper.lFlowControlEnable = flowControlEnable;
            qsWrapper.nLocalPort = localPort;
            qsWrapper.nRemotePort = remotePort;
            qsWrapper.szLocalIP = localIP;
            qsWrapper.szRemoteIP = remoteIP;
            qsWrapper.T3 = t3Timeout;
            qsWrapper.T5 = t5Timeout;
            qsWrapper.T6 = t6Timeout;
            qsWrapper.T7 = t7Timeout;
            qsWrapper.T8 = t8Timeout;
            qsWrapper.QSEvent += qsHandler;
        }

        #region Set Parameter

        private void doSetT3(object obj)
        {
            int val = (int)obj;
            qsWrapper.T3 = val;
        }
        public override void setT3(int t3Timeout)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(doSetT3), t3Timeout);
        }

        private void doSetT5(object obj)
        {
            int val = (int)obj;
            qsWrapper.T5 = val;
        }
        public override void setT5(int t5Timeout)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(doSetT5), t5Timeout);
        }

        private void doSetT6(object obj)
        {
            int val = (int)obj;
            qsWrapper.T6 = val;
        }
        public override void setT6(int t6Timeout)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(doSetT6), t6Timeout);
        }

        private void doSetT7(object obj)
        {
            int val = (int)obj;
            qsWrapper.T7 = val;
        }
        public override void setT7(int t7Timeout)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(doSetT7), t7Timeout);
        }

        private void doSetT8(object obj)
        {
            int val = (int)obj;
            qsWrapper.T8 = val;
        }
        public override void setT8(int t8Timeout)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(doSetT8), t8Timeout);
        }

        private void doSetLinkTestTimer(object obj)
        {
            int val = (int)obj;
            qsWrapper.lLinkTestPeriod = val;
        }

        public override void setLinkTestTimer(int linkTestTimer)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(doSetLinkTestTimer), linkTestTimer);
        }

        private void doSetDeviceID(object obj)
        {
            int val = (int)obj;
            qsWrapper.lDeviceID = val;
        }
        public override void setDeviceID(int deviceID)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(doSetDeviceID), deviceID);
        }
        #endregion Set Parameter

        //private void writeSECSLog(Object rawData)
        //{//SECSEventArgs eventArgs
        //    //logger.Info(SEUtility.secs2String(rawData, ref qsWrapper));
        //    logger.Info("writeSECSLog Begin Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);
        //    logger.Info(secsHandler.secs2String(rawData));
        //    logger.Info("writeSECSLog End Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);
        //}

        private void writeSECSLog(Object obj)
        {
            try
            {
                SECSEventArgs eventArgs = obj as SECSEventArgs;
                Object rawData = eventArgs.RawData;
                int S = eventArgs.S;
                int F = eventArgs.F;
                StringBuilder sb = new StringBuilder();
                logger.Debug(string.Format("writeSECSLog Begin Thread ID:{0}", Thread.CurrentThread.ManagedThreadId));
                DateTime startTime = DateTime.Now;
                sb.AppendLine(string.Format("[Time:{0}][Device ID:{1}][Event:{2}][StringFunction:S{3}F{4}][SystemByte:{5}]",
                    eventArgs.getEventTime(), eventArgs.DeviceID, eventArgs.MsgID, S, F, eventArgs.SystemBytes));
                String secsStr = secsHandler.secs2String(rawData);
                sb.AppendLine(secsStr);
                double totalMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
                logger.Debug(string.Format("writeSECSLog End Thread ID:{0}[Total Milliseconds:{1}]",
                    Thread.CurrentThread.ManagedThreadId, totalMS));
                //logger.Info(sb.ToString());
                SXFYLogger.Info(sb.ToString());
                string file_postfix = string.Concat("_", DateTime.Now.ToString("yyyyMMddHHmmssfffff"));
                string streamFunctionName = string.Format("S{0}F{1}", S, F);

                //A0.01  try
                //A0.01  {
                //A0.01      bool IsExists = System.IO.Directory.Exists(secsAgent.TX_LOG_PATH);
                //A0.01      if (!IsExists)
                //A0.01      {
                //A0.01          System.IO.Directory.CreateDirectory(secsAgent.TX_LOG_PATH);
                //A0.01      }
                //A0.01      using (StreamWriter sw =
                //A0.01              new StreamWriter(String.Concat(secsAgent.TX_LOG_PATH, "\\",
                //A0.01                  streamFunctionName, file_postfix, ".xml")))
                //A0.01      {
                //A0.01          sw.Write(sb.ToString());
                //A0.01      }
                //A0.01  }
                //A0.01  catch
                //A0.01  {
                //A0.01      logger.Warn(String.Concat("Failed to write TX Log: ", streamFunctionName,
                //A0.01          file_postfix, ".xml"));
                //A0.01  }

            }
            catch (Exception ex)
            {
                SXFYLogger.ErrorException("writeSECSLog Exception ! ", ex);
            }
        }

        private void QSEventHandler(int lID, QSACTIVEXLib.EVENT_ID lMsgID, int S, int F, int W_Bit,
            int ulSystemBytes, Object rawData, Object head, string pEventText)
        {
            string stream = "S" + S + "F" + F;
            int receiveDeviceID = deviceID;
            int[] headAry = head as int[];
            if (headAry.Length > 2)
            {
                receiveDeviceID = (headAry[0] * 10) + headAry[1];
            }

            logger.Debug("QSEventHandler [Device ID:{0}][lID:{1}][S:{2}; F:{3}][Name:{4}][lMsgID:{5}][W_Bit:{6}][ulSystemBytes:{7}][pEventText:{8}]",
                receiveDeviceID, lID, S, F, secsAgent.Name, lMsgID, W_Bit, ulSystemBytes, pEventText);

            if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_RECV_MSG)          //收到SECS
            {
                //logger.Debug(SEUtility.secs2String(rawData, ref qsWrapper));

                //轉換SECS為SXFY Object
                logger.Debug("Receive S:{0}; F:{1}; SysteByte:{2}; Device ID:{3}", S, F, ulSystemBytes, receiveDeviceID);
                SECSEventArgs eventArgs = new SECSEventArgs(lID, receiveDeviceID,
                    lMsgID, S, F, W_Bit, ulSystemBytes, rawData, head, pEventText, secsHandler);
                ThreadPool.QueueUserWorkItem(new WaitCallback(writeSECSLog), eventArgs);
                //如果是System Error類型的Stream Number，就不必往下執行、檢查
                if (S == SECSAgent.SystemErrorStreamNumber)
                {
                    secsAgent.onSystemErrorReceive(stream, eventArgs);
                    return;
                }
                #region 檢查Device ID、Stream Type、Funtion Type
                if (receiveDeviceID != deviceID && S != SECSAgent.SystemErrorStreamNumber)
                {
                    logger.Warn("Device ID is Error! [Local Device ID:{0}][Remote Device ID:{1}]", deviceID, receiveDeviceID);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(sendS9F1), eventArgs);
                    return;
                }
                SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT checkResult =
                    secsAgent.checkStreamFunctionType(S, F);
                if (checkResult == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Unrecognized_Stream_Type)
                {
                    //Send Stream Type Error (S9F3)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(sendS9F3), eventArgs);
                    return;
                }
                else if (checkResult == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Unrecognized_Function_Type)
                {
                    //Send Function Type Error (S9F5)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(sendS9F5), eventArgs);
                    return;
                }
                #endregion 檢查Device ID、Stream Type、Funtion Type

                secsAgent.onSECSReceive(stream, eventArgs);
            }
            else if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_SEND_MSG)     //送出SECS
            {
                //logger.Debug(SEUtility.secs2String(rawData, ref qsWrapper));
                //logger.Debug(secsHandler.secs2String(rawData));
                logger.Debug("Send S:{0}; F:{1}; SysteByte:{2}", S, F, ulSystemBytes);
                //                ThreadPool.QueueUserWorkItem(new WaitCallback(writeSECSLog), rawData);
                SECSEventArgs eventArgs = new SECSEventArgs(lID, deviceID,
                    lMsgID, S, F, W_Bit, ulSystemBytes, rawData, head, pEventText, secsHandler);
                ThreadPool.QueueUserWorkItem(new WaitCallback(writeSECSLog), eventArgs);
            }
            else if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_CONNECTED)
            {
                logger.Info("Connected to the passive entity");
                SECSEventArgs eventArgs = new SECSEventArgs(lID, deviceID,
                    lMsgID, S, F, W_Bit, ulSystemBytes, rawData, head, pEventText, secsHandler);
                secsAgent.onSECSConnected(eventArgs);
            }
            else if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_DISCONNECTED)     //斷線
            {
                SECSEventArgs eventArgs = new SECSEventArgs(lID, deviceID,
                    lMsgID, S, F, W_Bit, ulSystemBytes, rawData, head, pEventText, secsHandler);
                secsAgent.onSECSDisconnected(eventArgs);
                if (qsWrapper.HSMS_Connect_Mode == QSACTIVEXLib.HSMS_COMM_MODE.HSMS_ACTIVE_MODE)
                {
                    //' Operating in HSMS-SS Active Mode
                    logger.Info("DISCONNECTE:Try to establish connection...");
                    //' Retry connection
                    //qsWrapper.Start();
                    //startToListen();
                }
                else
                {
                    //' Operating in HSMS-SS Passive Mode
                    logger.Info("Remote client disconnected...");
                }

                //A0.03 startToListen();

                //A0.04 logger.Info("Refresh connection after [{0}] sec...,Device ID:[{1}]", this.t5Timeout, this.deviceID);   //A0.03
                //A0.04 Thread.Sleep(this.t5Timeout * 1000);                                                                   //A0.03
                //A0.04 secsAgent.refreshConnection();                                                                         //A0.03
                //A0.04 logger.Info("Refresh connection finish!!!,Device ID:[{0}]", this.deviceID);                            //A0.03

                reStartToListen();//A0.04
            }
            else if (lMsgID == QSACTIVEXLib.EVENT_ID.QS_EVENT_REPLY_TIMEOUT)    //Timeout
            {
                logger.Info("T3 Time out - [SystemByte:{0}]", ulSystemBytes);
                SECSEventArgs eventArgs = new SECSEventArgs(lID, deviceID,
                    lMsgID, S, F, W_Bit, ulSystemBytes, rawData, head, pEventText, secsHandler);
                ThreadPool.QueueUserWorkItem(new WaitCallback(sendS9F9), eventArgs);
                secsAgent.onSECST3Timeout(eventArgs); //A0.02
            }
        }

        #region S9FY
        /// <summary>
        /// Transaction Timer Timeout
        /// </summary>
        /// <param name="obj"></param>
        private void sendS9F9(Object obj)
        {
            SECSEventArgs eventArgs = obj as SECSEventArgs;
            string rcvSF = string.Format("S{0}F{1}", eventArgs.S, eventArgs.F);
            S9F9 s9f9 = new S9F9()
            {
                SECSAgentName = secsAgent.Name,
                SystemByte = eventArgs.SystemBytes,
                //                SHEAD = rcvSF
            };
            int[] headAry = eventArgs.Head as int[];
            s9f9.SHEAD = SEUtility.binary2String(headAry);
            TrxSECS.ReturnCode rtnCode = secsAgent.TrxSECS.replySECS(s9f9);
            if (rtnCode != TrxSECS.ReturnCode.Normal)
            {
                logger.Error("Send S9F9 Fail.");
            }
        }

        /// <summary>
        /// Unrecognized Function Type
        /// </summary>
        /// <param name="obj"></param>
        private void sendS9F5(Object obj)
        {
            SECSEventArgs eventArgs = obj as SECSEventArgs;
            string rcvSF = string.Format("S{0}F{1}", eventArgs.S, eventArgs.F);
            S9F5 s9f5 = new S9F5()
            {
                SECSAgentName = secsAgent.Name,
                SystemByte = eventArgs.SystemBytes,
                //                MHEAD = rcvSF
            };
            int[] headAry = eventArgs.Head as int[];
            s9f5.MHEAD = SEUtility.binary2String(headAry);
            TrxSECS.ReturnCode rtnCode = secsAgent.TrxSECS.replySECS(s9f5);
            if (rtnCode != TrxSECS.ReturnCode.Normal)
            {
                logger.Error("Send S9F5 Fail.");
            }
        }

        private void sendS9F1(Object obj)
        {
            SECSEventArgs eventArgs = obj as SECSEventArgs;
            string rcvSF = string.Format("S{0}F{1}", eventArgs.S, eventArgs.F);
            S9F1 s9f1 = new S9F1()
            {
                SECSAgentName = secsAgent.Name,
                SystemByte = eventArgs.SystemBytes,
                //                MHEAD = rcvSF
            };
            int[] headAry = eventArgs.Head as int[];
            s9f1.MHEAD = SEUtility.binary2String(headAry);
            TrxSECS.ReturnCode rtnCode = secsAgent.TrxSECS.replySECS(s9f1);
            if (rtnCode != TrxSECS.ReturnCode.Normal)
            {
                logger.Error("Send S9F1 Fail.");
            }
        }

        /// <summary>
        /// Unrecognized Stream Type
        /// </summary>
        /// <param name="obj"></param>
        private void sendS9F3(Object obj)
        {
            SECSEventArgs eventArgs = obj as SECSEventArgs;
            string rcvSF = string.Format("S{0}F{1}", eventArgs.S, eventArgs.F);
            S9F3 s9f3 = new S9F3()
            {
                SECSAgentName = secsAgent.Name,
                SystemByte = eventArgs.SystemBytes,
                //                MHEAD = rcvSF
            };
            int[] headAry = eventArgs.Head as int[];
            s9f3.MHEAD = SEUtility.binary2String(headAry);
            TrxSECS.ReturnCode rtnCode = secsAgent.TrxSECS.replySECS(s9f3);
            if (rtnCode != TrxSECS.ReturnCode.Normal)
            {
                logger.Error("Send S9F3 Fail.");
            }
        }
        #endregion S9FY

        private class DataItemStruct
        {
            public Object RawData { get; set; }
            public SXFY SendObj { get; set; }
            public Object ItemData { get; set; }
            //public QSACTIVEXLib.QSWrapper QsWrapper { get; set; }
            public EventWaitHandle eventWaitHandle = new ManualResetEvent(false);
        }

        private void doSendSECSIIMessage(Object obj)
        {
            try//A0.05
            {
                DataItemStruct diStruct = obj as DataItemStruct;
                Object rawData = diStruct.RawData;
                SXFY sendObj = diStruct.SendObj;
                qsWrapper.SendSECSIIMessage(sendObj.getS(), sendObj.getF(), sendObj.W_Bit, ref sendObj.SystemByte, rawData);
                diStruct.eventWaitHandle.Set();
            }
            catch (Exception ex)//A0.05
            {
                logger.Error(ex, "Exception:");
            }

        }

        public override bool replySECSMsg(SXFY sendObj)
        {
            Object rawData = null;
            try
            {
                sendObj.parser2RawData(ref rawData, qsWrapper);
                //                SXFYLogger.Debug(secsHandler.secs2String(rawData));
                SEUtility.showRawData(rawData);
                DataItemStruct dataItemStruct = new DataItemStruct()
                {
                    RawData = rawData,
                    SendObj = sendObj
                };
                ThreadPool.QueueUserWorkItem(doSendSECSIIMessage, dataItemStruct);
                dataItemStruct.eventWaitHandle.WaitOne();
                //                qsWrapper.SendSECSIIMessage(sendObj.getS(), sendObj.getF(), sendObj.W_Bit, ref sendObj.SystemByte, rawData);
            }
            catch (Exception ex)
            {
                logger.Warn("SendSECSIIMessage Error:{0}", ex);
                return false;
            }
            return true;
        }

        //Modify Start
        public EventWaitHandle reStartToListenWaitHandle = new ManualResetEvent(false);
        private void doReStartToListen(Object obj)
        {
            logger.Info("Beging doReStartToListen SECS Connection ,Device ID:{0}", deviceID); //A0.06
            SpinWait.SpinUntil(() => secsAgent.IsSelected, this.t5Timeout * 1000); //A0.07
            if (!secsAgent.IsSelected)                                             //A0.07
            {                                                                      //A0.07
                createQSWrapper();
                //A0.06 SpinWait.SpinUntil(() => false, this.t5Timeout * 1000); //A0.08
                //A0.08 Thread.Sleep(this.t5Timeout * 1000);
                int actionResult = -1;                                                            //A0.06

                //A0.06 qsWrapper.Initialize();
                actionResult = qsWrapper.Initialize();
                logger.Info("SECS QS, Device ID:{0} Initialize action,Result:{1}"                 //A0.06
                    , deviceID                                                                    //A0.06
                    , actionResult);                                                              //A0.06

                ////A0.06 qsWrapper.Start();
                actionResult = qsWrapper.Start();
                logger.Info("SECS QS, Device ID:{0} Start action,Result:{1}"                      //A0.06
                    , deviceID                                                                    //A0.06
                    , actionResult);                                                              //A0.06
            }                                                                      //A0.07
            logger.Info("End doReStartToListen SECS Connection ,Device ID:{0}", deviceID);    //A0.06

            reStartToListenWaitHandle.Set();
        }

        /**
        * 重新聆聽 //A0.04
        * */
        public void reStartToListen()
        {
            reStartToListenWaitHandle.Reset();
            secsAgent.LicenseKey.checkValidation();
            logger.Info("Refresh connection after [{0}] sec...,Device ID:[{1}]", this.t5Timeout, this.deviceID);
            ThreadPool.QueueUserWorkItem(doReStartToListen, null);
            reStartToListenWaitHandle.WaitOne();
            logger.Info("Refresh connection finish!!!,Device ID:[{0}]", this.deviceID);
        }
        //Modify End

        public EventWaitHandle StartToListenWaitHandle = new ManualResetEvent(false);
        private void doStartToListen(Object obj)
        {
            //DataItemStruct diStruct = obj as DataItemStruct;
            int actionResult = -1;                                              //A0.06
            ////A0.06 qsWrapper.Initialize();
            actionResult = qsWrapper.Initialize();                              //A0.06
            logger.Info("SECS QS, Device ID:{0} Initialize action,Result:{1}"   //A0.06
                , deviceID                                                      //A0.06
                , actionResult);                                                //A0.06
            ////A0.06 qsWrapper.Start();
            actionResult = qsWrapper.Start();                                   //A0.06
            logger.Info("SECS QS, Device ID:{0} Start action,Result:{1}"        //A0.06
                , deviceID                                                      //A0.06
                , actionResult);                                                //A0.06

            StartToListenWaitHandle.Set();
        }

        /**
        * 開始聆聽
        * */
        public override void startToListen()
        {
            StartToListenWaitHandle.Reset();
            secsAgent.LicenseKey.checkValidation();
            //Console.Out.WriteLine(string.Format("startToListen[{0}]", this.secsAgent.Name));
            //   ThreadPool.QueueUserWorkItem(doStartToListen, null);
            //qsWrapper.Initialize();
            //qsWrapper.Start();
            ThreadPool.QueueUserWorkItem(doStartToListen, null);
            StartToListenWaitHandle.WaitOne();
        }

        public EventWaitHandle StopToListenWaitHandle = new ManualResetEvent(false);
        private void doStopToListen(Object obj)
        {
            int actionResult = -1;                                          //A0.06
            //A0.06 qsWrapper.Stop();
            actionResult = qsWrapper.Stop();                                //A0.06
            logger.Info("SECS QS, Device ID:{0} Stop action,Result:{1}"     //A0.06
                , deviceID                                      //A0.06
                , actionResult);                                //A0.06
            //            qsWrapper.Destroy();
            StopToListenWaitHandle.Set();
        }

        /**
         * 結束鈴聽動作
         * */
        public override void stopToListen()
        {
            StopToListenWaitHandle.Reset();
            Console.Out.WriteLine(string.Format("stopToListen", this.secsAgent.Name));
            ThreadPool.QueueUserWorkItem(doStopToListen, null);
            //qsWrapper.Stop();
            StopToListenWaitHandle.WaitOne();
        }

        public EventWaitHandle DestroyWaitHandle = new ManualResetEvent(false);
        private void doDestroy(Object obj)
        {
            int actionResult = -1;                                              //A0.06
            //A0.06 qsWrapper.Destroy();
            actionResult = qsWrapper.Destroy();                                 //A0.06
            logger.Info("SECS QS, Device ID:{0} Destroy action,Result:{1}"      //A0.06
                , deviceID                                                      //A0.06
                , actionResult);                                                //A0.06
            DestroyWaitHandle.Set();
        }

        public override void destroy()
        {
            //qsWrapper.Destroy();
            DestroyWaitHandle.Reset();
            ThreadPool.QueueUserWorkItem(doDestroy, null);
            DestroyWaitHandle.WaitOne();
        }

        //public EventWaitHandle RefreshWaitHandle = new ManualResetEvent(false);
        //private void doRefreshConnection(Object obj) 
        //{
        //    createQSWrapper();
        //    RefreshWaitHandle.Set();
        //}

        //public override void refreshConnection() 
        //{
        //    RefreshWaitHandle.Reset();
        //    ThreadPool.QueueUserWorkItem(doRefreshConnection, null);
        //    RefreshWaitHandle.WaitOne();
        //}

        /**
         * T3 Timeout
         * */
        override public void replyTimeout()
        {
            ThreadPool.QueueUserWorkItem(doSendT3Timeout, null);
            //Object OutputRawData = null;
            //Object head = new int[10];
            //int ulOutputSystemBytes = 0;

            //qsWrapper.DataItemOut(OutputRawData, 10, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, head);
            //qsWrapper.SendSECSIIMessage(9, 9, 0, ref ulOutputSystemBytes, OutputRawData);
            //logger.Info("T3 Time out");
        }

        private void doSendT3Timeout(Object obj)
        {
            Object OutputRawData = null;
            Object head = new int[10];
            int ulOutputSystemBytes = 0;
            qsWrapper.DataItemOut(OutputRawData, 10, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, head);
            qsWrapper.SendSECSIIMessage(9, 9, 0, ref ulOutputSystemBytes, OutputRawData);
        }
    }
}
