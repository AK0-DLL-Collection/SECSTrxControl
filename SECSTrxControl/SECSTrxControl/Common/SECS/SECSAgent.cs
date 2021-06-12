//*********************************************************************************
//      SECSAgent.cs
//*********************************************************************************
// File Name: SECSAgent.cs
// Description: SECS Agent
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/05/30    Hayes Chen     N/A            N/A     Initial Release
// 2014/09/20    Hayes Chen     N/A            A0.01   加上S9FY系列的監聽
// 2014/10/27    Hayes Chen     N/A            A0.02   增加Receive Queue
// 2014/11/01    Hayes Chen     N/A            A0.03   增加指定特定的Stream Function才需要放入Receive Queue
// 2015/08/31    Kevin Wei      N/A            A0.04   增加T3 Time Out的事件註冊。
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Data.SecsData;
using com.mirle.ibg3k0.stc.License;
using NLog;
using QSACTIVEXLib;

namespace com.mirle.ibg3k0.stc.Common.SECS
{
    public class SECSAgent : ILicenseKeyProtect
    {
        private static string LicenseCopyRight =
            "MIRLE automation software license check fail! \n" +
                "This software will be stop running. \n" +
                "Please contact MIRLE for this authority issue. \n" +
                "copyright. MIRLE.3K0";
        private static Logger trxLogger = LogManager.GetLogger("SECSTrxLogger");
        private static Logger SXFYLogger = NLog.LogManager.GetLogger("SXFYLogger");
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static SECSFormatExceptionMonitor FormatExceptionMonitor = new SECSFormatExceptionMonitor();


        public enum SECS_AGENT_COMM_MODE
        {
            HSMS_ACTIVE_MODE = HSMS_COMM_MODE.HSMS_ACTIVE_MODE,
            HSMS_PASSIVE_MODE = HSMS_COMM_MODE.HSMS_PASSIVE_MODE
        }

        public enum PROCESS_RECEIVE_ORDER           //A0.02
        {
            Normal = 0,
            Queue = 1
        }

        public enum SECS_STREAM_FUNCTION_CHECK_RESULT
        {
            Recognize = 0,
            Unrecognized_Stream_Type = 1,
            Unrecognized_Function_Type = 2,
            Illegal_Data_Value_Format = 3
        }

        public enum SECS_PARSE_RESULT
        {
            OK = 0,
            Over_Size_Limit = 1
        }

        //public event EventHandler<SECSEventArgs> secsReceive;
        private Dictionary<string, EventHandler<SECSEventArgs>> RealSecsReceivedDic = new Dictionary<string, EventHandler<SECSEventArgs>>();
        private Dictionary<string, EventHandler<SECSEventArgs>> RealSecsDisconnetedDic = new Dictionary<string, EventHandler<SECSEventArgs>>();
        private Dictionary<string, EventHandler<SECSEventArgs>> RealSecsConnetedDic = new Dictionary<string, EventHandler<SECSEventArgs>>();
        private Dictionary<string, EventHandler<SECSEventArgs>> RealSystemErrorReceivedDic = new Dictionary<string, EventHandler<SECSEventArgs>>();    //A0.01
        public event EventHandler<SECSEventArgs> secsT3Timeout;
        private HostConnection hostConnection;

        public string Name { get; private set; }
        public string LocalIP { get; private set; }
        public string RemoteIP { get; private set; }
        public int LocalPort { get; private set; }
        public int RemotePort { get; private set; }
        public int T3Timeout { get; private set; }
        public int T5Timeout { get; private set; }
        public int T6Timeout { get; private set; }
        public int T7Timeout { get; private set; }
        public int T8Timeout { get; private set; }
        public int LinkTestTimer { get; private set; }
        public int DeviceID { get; private set; }
        public SECS_AGENT_COMM_MODE ConnectMode { get; private set; }
        public PROCESS_RECEIVE_ORDER ProcReceiveOrder { get; private set; }       //A0.02
        public List<string> StreamFunctionProcByReceiveQueue { get; private set; }  //A0.03

        public int TimeoutSec { get { return T3Timeout; } }
        public int RetryCount { get; private set; }
        public string TX_LOG_PATH { get; private set; }

        private Boolean isSelected = false;
        public Boolean IsSelected { get { return isSelected; } }

        public TrxSECS TrxSECS
        {
            get { return new TrxSECS(this); }
        }

        public delegate SECS_STREAM_FUNCTION_CHECK_RESULT SFTypeCheck(int S, int F);
        private SFTypeCheck sfTypeCheck;
        public delegate SECS_STREAM_FUNCTION_CHECK_RESULT DataIllegalCheck(string name, string value);
        private DataIllegalCheck dataIllegalCheck;

        public ILicenseKey LicenseKey { get { return licenseKey; } }
        private ILicenseKey licenseKey = null;

        private TriggerEventQueue triggerEventQueue = null;   //A0.02

        public SECSAgent(string name, int deviceID, string localIP, int localPort, string remoteIP, int remotePort, SECS_AGENT_COMM_MODE connectMode,
            int t3Timeout, int t5Timeout, int t6Timeout, int t7Timeout, int t8Timeout, int linkTestTimer,
            string TX_LOG_PATH, /*int timeoutSec,*/
            int retryCount) :
            this(name, deviceID, localIP, localPort, remoteIP, remotePort, connectMode,
            t3Timeout, t5Timeout, t6Timeout, t7Timeout, t8Timeout, linkTestTimer, TX_LOG_PATH, retryCount,
            PROCESS_RECEIVE_ORDER.Normal, null,
            null, null)
        {
            //sfTypeCheck = typeCheckFunction;
        }

        public SECSAgent(string name, int deviceID, string localIP, int localPort, string remoteIP, int remotePort, SECS_AGENT_COMM_MODE connectMode,
            int t3Timeout, int t5Timeout, int t6Timeout, int t7Timeout, int t8Timeout, int linkTestTimer,
            string TX_LOG_PATH, /*int timeoutSec,*/
            int retryCount, PROCESS_RECEIVE_ORDER procReceiveOrder,
            List<string> streamFunctionProcByReceiveQueue,              //A0.03
            SFTypeCheck typeCheckFunction, DataIllegalCheck dataIllegalCheck)
        {
            //            licenseKey = new TrialLicenseKey(this);
            //            licenseKey = new SimpleLicenseKey(this);
            licenseKey = new AutomationLicenseKey(this);
            licenseKey.checkValidation();
            //            triggerEventQueue = new TriggerEventQueue(name, licenseKey);    //A0.02
            triggerEventQueue = new TriggerEventQueue(name);    //A0.02
            Name = name;
            LocalIP = localIP;
            RemoteIP = remoteIP;
            LocalPort = localPort;
            RemotePort = remotePort;
            ConnectMode = connectMode;
            DeviceID = deviceID;
            this.TX_LOG_PATH = TX_LOG_PATH;
            //            this.TimeoutSec = t3Timeout;// timeoutSec;
            this.RetryCount = retryCount;
            T3Timeout = t3Timeout;
            T5Timeout = t5Timeout;
            T6Timeout = t6Timeout;
            T7Timeout = t7Timeout;
            T8Timeout = t8Timeout;
            LinkTestTimer = linkTestTimer;
            this.ProcReceiveOrder = procReceiveOrder;           //A0.02
            this.StreamFunctionProcByReceiveQueue = streamFunctionProcByReceiveQueue;       //A0.03
            hostConnection = new SECSConnection(this, LocalIP, LocalPort, RemoteIP, RemotePort, (HSMS_COMM_MODE)ConnectMode,
                T3Timeout, T5Timeout, T6Timeout, T7Timeout, T8Timeout, LinkTestTimer, DeviceID);
            sfTypeCheck = typeCheckFunction;
            this.dataIllegalCheck = dataIllegalCheck;

        }
        /// <summary>
        /// 將SECS Agent進行 refresh。
        /// 將對該SECS Driver 執行 1.Stop Driver、2.Destroy、3.New Instance、4.Start Driver
        /// </summary>
        public void refreshConnection()
        {
            stop();
            hostConnection.destroy();
            hostConnection = null;
            hostConnection = new SECSConnection(this, LocalIP, LocalPort, RemoteIP, RemotePort, (HSMS_COMM_MODE)ConnectMode,
                T3Timeout, T5Timeout, T6Timeout, T7Timeout, T8Timeout, LinkTestTimer, DeviceID);
            start();
        }

        public void setT3(int t3Timeout)
        {
            T3Timeout = t3Timeout;
            hostConnection.setT3(t3Timeout);
        }

        public void setT5(int t5Timeout)
        {
            T5Timeout = t5Timeout;
            hostConnection.setT5(t5Timeout);
        }

        public void setT6(int t6Timeout)
        {
            T6Timeout = t6Timeout;
            hostConnection.setT6(t6Timeout);
        }

        public void setT7(int t7Timeout)
        {
            T7Timeout = t7Timeout;
            hostConnection.setT7(t7Timeout);
        }

        public void setT8(int t8Timeout)
        {
            T8Timeout = t8Timeout;
            hostConnection.setT8(t8Timeout);
        }

        /// <summary>
        /// 設置發送Link Test的間隔時間。
        /// Link Test為當雙方SECS在無進行Transaction時，將會進行發送給對方的訊號，用以確保雙方保持連線。
        /// </summary>
        /// <param name="linkTestTimer"></param>
        public void setLinkTestTimer(int linkTestTimer)
        {
            LinkTestTimer = linkTestTimer;
            hostConnection.setLinkTestTimer(linkTestTimer);
        }

        public void setDeviceID(int deviceID)
        {
            DeviceID = deviceID;
            hostConnection.setDeviceID(deviceID);
        }

        //public void refreshConnection() 
        //{
        //    hostConnection.refreshConnection();
        //}

        /// <summary>
        /// 實作於ILicenseKeyProtect，使其呈現在License失效後，所需要的紀錄/提示。
        /// </summary>
        /// <param name="msg"></param>
        public void invalidLicense(string msg)
        {
            //try
            //{
            //    stop();
            //}
            //catch { }
            //throw new LicenseKeyException(msg);
            SEUtility.SystemEventLog(msg, System.Diagnostics.EventLogEntryType.Warning);
            System.Windows.Forms.MessageBox.Show(msg, "SECS Agent Copyright");
        }
        /// <summary>
        /// 實作於ILicenseKeyProtect，使其在License失效後，停止該Agent的服務。
        /// </summary>
        public void stopSystemByLicenseCheck()
        {
            try
            {
                stop();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            finally
            {
                SEUtility.SystemEventLog(LicenseCopyRight, System.Diagnostics.EventLogEntryType.Warning);
                System.Windows.Forms.MessageBox.Show(LicenseCopyRight, "SECS Agent Copyright");
            }
        }
        /// <summary>
        /// 注入檢查Strame tpye、Function message id是否符合的Function，
        /// 用來在收到Stream function時，確認是否要回應
        /// Unrecognized Stream Type或Unrecognized Function Type。
        /// </summary>
        /// <param name="sfTypeCheck">檢查Stream Function是否符合的Function</param>
        public void injectSFTypeCheck(SFTypeCheck sfTypeCheck)
        {
            this.sfTypeCheck = sfTypeCheck;
        }
        /// <summary>
        /// 注入檢查Data format是否符合的Function，
        /// 用來在收到Stream function時，確認是否要回應
        /// Illegal Data。
        /// </summary>
        /// <param name="dataIllegalCheck">檢查Data format是否符合的Function</param>
        public void injectDataIllegalCheck(DataIllegalCheck dataIllegalCheck)
        {
            this.dataIllegalCheck = dataIllegalCheck;
        }
        /// <summary>
        /// 進行Strame tpye、Function message id的檢查，
        /// </summary>
        /// <param name="S">Stream Type</param>
        /// <param name="F">Function message id</param>
        /// <returns></returns>
        public SECS_STREAM_FUNCTION_CHECK_RESULT checkStreamFunctionType(int S, int F)
        {
            if (sfTypeCheck == null)
            {
                return SECS_STREAM_FUNCTION_CHECK_RESULT.Recognize;
            }
            return sfTypeCheck(S, F);
        }
        /// <summary>
        /// 進行Data format的檢查
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sxfy"></param>
        /// <returns></returns>
        public SECS_STREAM_FUNCTION_CHECK_RESULT checkIllegalDataValueFormat(Type type, SXFY sxfy)
        {
            com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT result =
                SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Recognize;
            if (dataIllegalCheck == null)
            {
                return result;
            }

            FieldInfo[] fields = SEUtility.GetSeceElementFields(sxfy.GetType());
            for (int ix = 0; ix < fields.Length; ++ix)
            {
                FieldInfo field = fields[ix];
                if (field.FieldType.IsArray && field.FieldType.GetElementType().BaseType == typeof(SXFY))
                {
                    SXFY[] subArray = (SXFY[])field.GetValue(sxfy);
                    if (subArray == null)
                    {
                        continue;
                    }
                    for (int index = 0; index < subArray.Length; ++index)
                    {
                        SXFY eleSxfy = subArray[index];
                        result = checkIllegalDataValueFormat(eleSxfy.GetType(), eleSxfy);
                        if (result == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format)
                        {
                            return result;
                        }
                    }
                }
                else if (field.FieldType.IsArray && field.FieldType.GetElementType() == typeof(string))
                {
                    string[] subArray = (string[])field.GetValue(sxfy);
                    if (subArray == null)
                    {
                        continue;
                    }
                    for (int index = 0; index < subArray.Length; ++index)
                    {
                        string eleStr = subArray[index];
                        result = dataIllegalCheck(field.Name, eleStr);
                        if (result == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format)
                        {
                            return result;
                        }
                    }
                }
                else if (field.FieldType.BaseType == typeof(SXFY))
                {
                    SXFY subSxfy = (SXFY)field.GetValue(sxfy);
                    if (subSxfy == null)
                    {
                        continue;
                    }
                    result = checkIllegalDataValueFormat(subSxfy.GetType(), subSxfy);
                    if (result == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format)
                    {
                        return result;
                    }
                }
                else if (field.FieldType == typeof(string))
                {
                    string value = (String)field.GetValue(sxfy);
                    if (value == null)
                    {
                        continue;
                    }
                    result = dataIllegalCheck(field.Name, value);
                    if (result == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 啟動SECS Agent
        /// </summary>
        public void start()
        {
            licenseKey.checkValidation();
            hostConnection.startToListen();
        }

        /// <summary>
        /// 停止SECS Agent
        /// </summary>
        public void stop()
        {
            hostConnection.stopToListen();
        }
        /// <summary>
        /// 取得QSWrapper的執行個體。
        /// </summary>
        /// <returns></returns>
        public QSWrapper getQSWrapper()
        {
            return (hostConnection as SECSConnection).QSWrapper;
        }

        /// <summary>
        /// 註冊監聽收到的StreamFunction
        /// </summary>
        /// <param name="streamFunction">註冊的StreamFunction。(ex.SXFY)</param>
        /// <param name="handler"></param>
        public void addSECSReceivedHandler(string streamFunction, EventHandler<SECSEventArgs> handler)
        {
            lock (RealSecsReceivedDic)
            {
                if (RealSecsReceivedDic.ContainsKey(streamFunction.Trim()))
                {
                    RealSecsReceivedDic[streamFunction.Trim()] += handler;
                }
                else
                {
                    RealSecsReceivedDic.Add(streamFunction.Trim(), handler);
                }
            }
        }

        public static readonly int SystemErrorStreamNumber = 9;
        /// <summary>
        /// A0.01
        /// System Error (S9FY) 的監聽
        /// </summary>
        /// <param name="handler"></param>
        public void addSystemErrorReceivedHandler(EventHandler<SECSEventArgs> handler)
        {
            lock (RealSystemErrorReceivedDic)
            {
                if (RealSystemErrorReceivedDic.ContainsKey(SystemErrorStreamNumber.ToString()))
                {
                    RealSystemErrorReceivedDic[SystemErrorStreamNumber.ToString()] += handler;
                }
                else
                {
                    RealSystemErrorReceivedDic.Add(SystemErrorStreamNumber.ToString(), handler);
                }
            }
        }
        /// <summary>
        /// 移除System error的監聽
        /// </summary>
        /// <param name="handler"></param>
        public void removeSystemErrorReceivedHandler(EventHandler<SECSEventArgs> handler)
        {
            lock (RealSystemErrorReceivedDic)
            {
                if (RealSystemErrorReceivedDic.ContainsKey(SystemErrorStreamNumber.ToString()))
                {
                    RealSystemErrorReceivedDic[SystemErrorStreamNumber.ToString()] -= handler;
                }
            }
        }
        /// <summary>
        /// 移除StreamFunction的監聽
        /// </summary>
        /// <param name="streamFunction"></param>
        /// <param name="handler"></param>
        public void removeSECSReceivedHandler(string streamFunction, EventHandler<SECSEventArgs> handler)
        {
            lock (RealSecsReceivedDic)
            {
                if (RealSecsReceivedDic.ContainsKey(streamFunction.Trim()))
                {
                    RealSecsReceivedDic[streamFunction.Trim()] -= handler;
                }
            }
        }
        /// <summary>
        /// 註冊監聽SECS Disconnected
        /// </summary>
        /// <param name="handler"></param>
        public void addSECSDisconnectedHandler(EventHandler<SECSEventArgs> handler)
        {
            lock (RealSecsDisconnetedDic)
            {
                if (RealSecsDisconnetedDic.ContainsKey(DisconnectedEventName.Trim()))
                {
                    RealSecsDisconnetedDic[DisconnectedEventName.Trim()] += handler;
                }
                else
                {
                    RealSecsDisconnetedDic.Add(DisconnectedEventName.Trim(), handler);
                }
            }
        }
        /// <summary>
        /// 移除監聽SECS Disconnected
        /// </summary>
        /// <param name="handler"></param>
        public void removeSECSDisconnectedHandler(EventHandler<SECSEventArgs> handler)
        {
            lock (RealSecsDisconnetedDic)
            {
                if (RealSecsDisconnetedDic.ContainsKey(DisconnectedEventName.Trim()))
                {
                    RealSecsDisconnetedDic[DisconnectedEventName.Trim()] -= handler;
                }
            }
        }
        /// <summary>
        /// 註冊監聽SECS Connected
        /// </summary>
        /// <param name="handler"></param>
        public void addSECSConnectedHandler(EventHandler<SECSEventArgs> handler)
        {
            lock (RealSecsConnetedDic)
            {
                if (RealSecsConnetedDic.ContainsKey(ConnectedEventName.Trim()))
                {
                    RealSecsConnetedDic[ConnectedEventName.Trim()] += handler;
                }
                else
                {
                    RealSecsConnetedDic.Add(ConnectedEventName.Trim(), handler);
                }
            }
        }
        /// <summary>
        /// 移除監聽SECS Connected
        /// </summary>
        /// <param name="handler"></param>
        public void removeSECSConnectedHandler(EventHandler<SECSEventArgs> handler)
        {
            lock (RealSecsConnetedDic)
            {
                if (RealSecsConnetedDic.ContainsKey(ConnectedEventName.Trim()))
                {
                    RealSecsConnetedDic[ConnectedEventName.Trim()] -= handler;
                }
            }
        }


        private static readonly string DisconnectedEventName = "DisconnectedEventName";
        private static readonly string ConnectedEventName = "ConnectedEventName";
        /// <summary>
        /// 通知SECS Disconnected
        /// </summary>
        /// <param name="e"></param>
        public void onSECSDisconnected(SECSEventArgs e)
        {
            isSelected = false;
            if (licenseKey != null) { licenseKey.checkValidation(); }
            EventHandler<SECSEventArgs> tmpEventHandler = null;
            lock (RealSecsDisconnetedDic)
            {
                if (RealSecsDisconnetedDic.ContainsKey(DisconnectedEventName.Trim()))
                {
                    tmpEventHandler = RealSecsDisconnetedDic[DisconnectedEventName];
                }
            }
            if (tmpEventHandler != null)
            {
                ChangeEventStruct eventStruct = new ChangeEventStruct()
                {
                    EventHandler = tmpEventHandler,
                    EventArgs = e
                };
                triggerEvent(eventStruct);
                //                triggerEventQueue.onNotify(eventStruct);        //A0.02
                //A0.02                ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
            }
        }
        /// <summary>
        /// 通知SECS Connected
        /// </summary>
        /// <param name="e"></param>
        public void onSECSConnected(SECSEventArgs e)
        {
            isSelected = true;
            if (licenseKey != null) { licenseKey.checkValidation(); }
            EventHandler<SECSEventArgs> tmpEventHandler = null;
            lock (RealSecsConnetedDic)
            {
                if (RealSecsConnetedDic.ContainsKey(ConnectedEventName.Trim()))
                {
                    tmpEventHandler = RealSecsConnetedDic[ConnectedEventName];
                }
            }
            if (tmpEventHandler != null)
            {
                ChangeEventStruct eventStruct = new ChangeEventStruct()
                {
                    EventHandler = tmpEventHandler,
                    EventArgs = e
                };
                triggerEvent(eventStruct);
                //                triggerEventQueue.onNotify(eventStruct);            //A0.02
                //A0.02                ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
            }
        }
        /// <summary>
        /// 通知Receive stream function
        /// </summary>
        /// <param name="streamFunction">通知的Stream Function</param>
        /// <param name="e"></param>
        public void onSECSReceive(string streamFunction, SECSEventArgs e)
        {
            EventHandler<SECSEventArgs> tmpEventHandler = null;
            lock (RealSecsReceivedDic)
            {
                if (RealSecsReceivedDic.ContainsKey(streamFunction.Trim()))
                {
                    tmpEventHandler = RealSecsReceivedDic[streamFunction];
                }
            }
            if (tmpEventHandler != null)
            {
                ChangeEventStruct eventStruct = new ChangeEventStruct()
                {
                    EventHandler = tmpEventHandler,
                    EventArgs = e
                };
                triggerEvent(eventStruct);
                //                triggerEventQueue.onNotify(eventStruct);        //A0.02
                //A0.02                ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
            }
        }

        /// <summary>
        /// A0.01
        /// 觸發System Error Stream Function的事件
        /// </summary>
        /// <param name="streamFunction"></param>
        /// <param name="e"></param>
        public void onSystemErrorReceive(string streamFunction, SECSEventArgs e)
        {
            EventHandler<SECSEventArgs> tmpEventHandler = null;
            lock (RealSystemErrorReceivedDic)
            {
                if (RealSystemErrorReceivedDic.ContainsKey(SystemErrorStreamNumber.ToString()))
                {
                    tmpEventHandler = RealSystemErrorReceivedDic[SystemErrorStreamNumber.ToString()];
                }
            }
            if (tmpEventHandler != null)
            {
                ChangeEventStruct eventStruct = new ChangeEventStruct()
                {
                    EventHandler = tmpEventHandler,
                    EventArgs = e
                };
                triggerEvent(eventStruct);
                //                triggerEventQueue.onNotify(eventStruct);            //A0.02
                //A0.02                ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
            }
        }
        /// <summary>
        /// 當有事件需要通知時(SECSDisconnected、SECSConnected、SECSReceive、SystemErrorReceive)
        /// 統一由此處通知。
        /// </summary>
        /// <param name="eventStruct"></param>
        private void triggerEvent(ChangeEventStruct eventStruct)
        {
            if (ProcReceiveOrder == PROCESS_RECEIVE_ORDER.Queue)
            {
                //A0.03
                //StreamFunctionProcByReceiveQueue
                //eventStruct.EventArgs.S
                string streamFunction = String.Format("S{0}F{1}",
                    eventStruct.EventArgs.S, eventStruct.EventArgs.F);
                if (eventStruct.EventArgs.F % 2 == 0)   //F是偶數，表示是EQ的回覆
                {
                    try
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Exception:");
                    }
                }
                else                                    //F是奇數，表示是EQ主動發出
                {
                    if (StreamFunctionProcByReceiveQueue == null ||
                        (StreamFunctionProcByReceiveQueue != null && StreamFunctionProcByReceiveQueue.Count == 0) ||
                        (StreamFunctionProcByReceiveQueue != null
                                && StreamFunctionProcByReceiveQueue.Contains(streamFunction)))      //A0.03
                    {
                        triggerEventQueue.onNotify(eventStruct);
                    }
                    else
                    {
                        try
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Exception:");
                        }
                    }
                }
            }
            else if (ProcReceiveOrder == PROCESS_RECEIVE_ORDER.Normal)
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
            else
            {
                try
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception:");
                }
            }
        }

        private void triggerEventHandler(Object obj)
        {
            //            licenseKey.checkValidation();
            ChangeEventStruct eventStruct = obj as ChangeEventStruct;
            if (eventStruct.EventHandler != null)
            {
                SXFYLogger.Debug(
                        "Begin Trigger Event Handler: [ID:{0}][name:{1}][S{2}F{3}][SystemByte:{4}]",
                        eventStruct.EventArgs.ID, eventStruct.EventArgs.secsHandler.Agent.Name,
                        eventStruct.EventArgs.S, eventStruct.EventArgs.F, eventStruct.EventArgs.SystemBytes);
                DateTime startTime = DateTime.Now;
                eventStruct.EventHandler(this, eventStruct.EventArgs);
                double totalMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
                SXFYLogger.Debug(
                        "Complete Trigger Event Handler: [ID:{0}][name:{1}][S{2}F{3}][SystemByte:{4}][Total Milliseconds:{5}]",
                        eventStruct.EventArgs.ID, eventStruct.EventArgs.secsHandler.Agent.Name,
                        eventStruct.EventArgs.S, eventStruct.EventArgs.F, eventStruct.EventArgs.SystemBytes, totalMS);
                //trxLogger.Debug("Complete Trigger Event Handler: [ID:{0}][name:{1}][F:{2}][S:{3}][SystemByte:{4}][Total Milliseconds:{5}]",
                //    eventStruct.EventArgs.ID,
                //    Name, eventStruct.EventArgs.F, eventStruct.EventArgs.S, eventStruct.EventArgs.SystemBytes, totalMS);
            }
        }

        //A0.04 Start
        protected static Object T3TimeoutErrorLock = new object();
        /// <summary>
        /// 監聽T3TimeOut
        /// </summary>
        /// <param name="handler"></param>
        public void addSECST3TimeOutHandler(EventHandler<SECSEventArgs> handler)
        {
            lock (T3TimeoutErrorLock)
            {
                secsT3Timeout += handler;
            }
        }
        /// <summary>
        /// 移除監聽T3TimeOut
        /// </summary>
        /// <param name="handler"></param>
        public void removeSECST3TimeOutHandler(EventHandler<SECSEventArgs> handler)
        {
            lock (T3TimeoutErrorLock)
            {
                secsT3Timeout -= handler;
            }
        }
        //A0.04 End

        /// <summary>
        /// 通知T3 TimeOut
        /// </summary>
        /// <param name="e"></param>
        public void onSECST3Timeout(SECSEventArgs e)
        {
            if (secsT3Timeout != null)
            {
                //secsT3Timeout(this, e);
                ChangeEventStruct eventStruct = new ChangeEventStruct()
                {
                    EventHandler = secsT3Timeout,
                    EventArgs = e
                };
                triggerEvent(eventStruct);
                //                triggerEventQueue.onNotify(eventStruct);            //A0.02
                //A0.02                ThreadPool.QueueUserWorkItem(new WaitCallback(triggerEventHandler), eventStruct);
            }
        }

        /**
         * 回傳SECS 訊息
         * */
        public bool replySECSMsg(SXFY obj)
        {
            return hostConnection.replySECSMsg(obj);
        }

        public void replyTimeout()
        {
            hostConnection.replyTimeout();
        }

        public string getMessage()
        {
            return hostConnection.strmessage;
        }

    }

    public class ChangeEventStruct
    {
        public EventHandler<SECSEventArgs> EventHandler { get; set; }
        public SECSEventArgs EventArgs { get; set; }
    }

    //A0.02 Begin
    /// <summary>
    /// 用來進行Receive By Queue的處理。
    /// (即會按照接收到recive enevt進行處理)
    /// </summary>
    public class TriggerEventQueue
    {
        private static Logger trxLogger = LogManager.GetLogger("SECSTrxLogger");
        private static Logger SXFYLogger = NLog.LogManager.GetLogger("SXFYLogger");
        private static Logger triggerEventLogger = LogManager.GetLogger("TriggerEventQueue");
        private readonly object _eventSyncRoot = new object();
        private readonly Queue<WaitCallback> _eventDelegateQueue = new Queue<WaitCallback>();
        //        private ILicenseKey licenseKey = null;
        private String name = string.Empty;
        private WorkKeyFlag flag = new WorkKeyFlag();
        private Object _lock = new Object();
        public TriggerEventQueue(string name/*, ILicenseKey licenseKey*/)
        {
            this.name = name;
            //            this.licenseKey = licenseKey;
            handlerEvent += handler;
        }

        public void onNotify(ChangeEventStruct _eventStruct)
        {
            WaitCallback eventDelegate = delegate(object obj)
            {
                ChangeEventStruct eventStruct = _eventStruct;
                if (eventStruct.EventHandler != null)
                {
                    //triggerEventLogger.Debug("Begin Trigger Event Handler: [ID:{0}][name:{1}][F:{2}][S:{3}][SystemByte:{4}]",
                    //    eventStruct.EventArgs.ID,
                    //    name, eventStruct.EventArgs.F, eventStruct.EventArgs.S, eventStruct.EventArgs.SystemBytes);
                    SXFYLogger.Debug(
                        "Begin Trigger Event Handler By Queue: [ID:{0}][name:{1}][S{2}F{3}][SystemByte:{4}]",
                        eventStruct.EventArgs.ID, eventStruct.EventArgs.secsHandler.Agent.Name,
                        eventStruct.EventArgs.S, eventStruct.EventArgs.F, eventStruct.EventArgs.SystemBytes);
                    DateTime startTime = DateTime.Now;
                    eventStruct.EventHandler(this, eventStruct.EventArgs);
                    double totalMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
                    SXFYLogger.Debug(
                        "Complete Trigger Event Handler By Queue: [ID:{0}][name:{1}][S{2}F{3}][SystemByte:{4}][Total Milliseconds:{5}]",
                        eventStruct.EventArgs.ID, eventStruct.EventArgs.secsHandler.Agent.Name,
                        eventStruct.EventArgs.S, eventStruct.EventArgs.F, eventStruct.EventArgs.SystemBytes, totalMS);
                    //triggerEventLogger.Debug("End Trigger Event Handler: [ID:{0}][name:{1}][F:{2}][S:{3}][SystemByte:{4}]",
                    //    eventStruct.EventArgs.ID,
                    //    name, eventStruct.EventArgs.F, eventStruct.EventArgs.S, eventStruct.EventArgs.SystemBytes);
                }
            };
            lock (_eventDelegateQueue)
            {
                _eventDelegateQueue.Enqueue(eventDelegate);
            }
            if (flag.Compare(WorkKeyFlag.UNSET))
            {
                ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(doWork), null);
            }
        }

        private void doWork(Object wKey)
        {
            if (flag.CompareAndSet(WorkKeyFlag.UNSET, WorkKeyFlag.SET))
            {
                trxLogger.Debug("TriggerEventQueue doWork is Running.[Thread ID{0}]", Thread.CurrentThread.ManagedThreadId);
                while (true)
                {
                    try
                    {
                        //                        lock (_lock)
                        //                        {
                        WaitCallback eventDelegate = null;
                        lock (_eventDelegateQueue)
                        {
                            if (_eventDelegateQueue.Count <= 0)
                            {
                                break;
                            }
                            //                            WaitCallback eventDelegate = _eventDelegateQueue.Dequeue();
                            eventDelegate = _eventDelegateQueue.Dequeue();
                        }
                        HandlerDelgate tmpHandlerEvent = handlerEvent;
                        if (tmpHandlerEvent != null && eventDelegate != null)
                        {

                            tmpHandlerEvent(eventDelegate);

                        }
                        //                            ThreadPool.QueueUserWorkItem(eventDelegate);
                        //                        }
                        //work.doWork(workKey, workItem);
                    }
                    catch (Exception ex)
                    {
                        triggerEventLogger.WarnException("Do Work Exception !", ex);
                    }
                }
                flag.CompareAndSet(WorkKeyFlag.SET, WorkKeyFlag.UNSET);
            }
            else
            {

            }
        }

        public delegate void HandlerDelgate(WaitCallback eventDelegate);
        public event HandlerDelgate handlerEvent;
        private void handler(WaitCallback eventDelegate)
        {
            eventDelegate(null);
        }

        //public void onNotify_other(ChangeEventStruct _eventStruct)
        //{
        //    licenseKey.checkValidation();
        //    WaitCallback eventDelegate = delegate(object obj)
        //    {
        //        ChangeEventStruct eventStruct = _eventStruct;
        //        if (eventStruct.EventHandler != null)
        //        {
        //            eventStruct.EventHandler(this, eventStruct.EventArgs);
        //        }
        //    };
        //    lock (_eventDelegateQueue)
        //    {
        //        _eventDelegateQueue.Enqueue(eventDelegate);
        //    }
        //    //Run
        //    WaitCallback handlerDelegate = delegate(object state)
        //    {
        //        lock (_eventSyncRoot)
        //        {
        //            WaitCallback runEventDelegate = null;
        //            lock (_eventDelegateQueue)
        //            {
        //                if (_eventDelegateQueue.Count > 0)
        //                {
        //                    runEventDelegate = _eventDelegateQueue.Dequeue();
        //                }
        //            }

        //            if (runEventDelegate != null)
        //            {
        //                runEventDelegate(null);
        //            }
        //        }
        //    };
        //    ThreadPool.QueueUserWorkItem(handlerDelegate);
        //}

    }

    public class WorkKeyFlag
    {
        public static readonly int SET = 1;
        public static readonly int UNSET = 0;

        //public int EventPoint { get; set; }
        private int eventPoint;
        public WorkKeyFlag()
        {
            eventPoint = UNSET;
        }

        public int Exchange(int update)
        {
            return Interlocked.Exchange(ref eventPoint, update);
        }

        public bool Compare(int expect)
        {
            return Interlocked.CompareExchange(ref eventPoint, eventPoint, expect) == expect;
        }

        public bool CompareAndSet(int expect, int update)
        {
            return Interlocked.CompareExchange(ref eventPoint, update, expect) == expect;
        }
    }
    //A0.02 End

    public class SECSFormatExceptionMonitor : PropertyChangedVO
    {
        private SECSFormatExceptionItem formatExceptionItem;
        public SECSFormatExceptionItem FormatExceptionItem
        {
            get { return formatExceptionItem; }
            set
            {
                formatExceptionItem = value;
                OnPropertyChanged(getPropertyName(() => this.FormatExceptionItem));
            }
        }

        public static string getPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }
    }

    public class SECSFormatExceptionItem
    {
        public SECSFormatException FormatException { get; set; }
        public string Message { get; set; }
    }
}
