//*********************************************************************************
//      SECSHandler.cs
//*********************************************************************************
// File Name: SECSHandler.cs
// Description: SECS Handler
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/05/30    Hayes Chen     N/A            N/A     Initial Release
// 2014/08/01    Hayes Chen     N/A            A0.01   Fix Bug: 已進入List處理程序，就不應該再pass下一個List
// 2014/08/14    Hayes Chen     N/A            A0.02   當只有Header的StreamFunction，也一樣要建立StreamFunction物件
// 2014/08/31    Hayes Chen     N/A            A0.03   加上轉換String Array的功能
// 2014/09/01    Hayes Chen     N/A            A0.04   加上SXFY Class直接Spread Out Field的情況
// 2014/09/05    Hayes Chen     N/A            A0.05   增加格式錯誤的Exception處理
// 2014/11/11    Hayes Chen     N/A            A0.06   不使用另一執行序執行轉換
//**********************************************************************************
//**********************************************************************************
// 2017/04/14    Kevin Wei      N/A            B0.01   修改在非"ListElement"還使用"ListElementType"，因此將ListElementType改為Type
// 2017/07/11    Kevin Wei      N/A            B0.02   修正在收到不正確的Filed Count時，不會回復S9F7的問題。
// 2017/07/21    Kevin Wei      N/A            B0.03   增加I2、I4、U1、U2、U4的格式轉換。
// 2017/07/21    Kevin Wei      N/A            B0.04   增加判斷如果是UNKNOWN的型別，則忽略確認(回復S9F7)的部分。
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common.SECS;
using com.mirle.ibg3k0.stc.Data.SecsData;
using NLog;

namespace com.mirle.ibg3k0.stc.Common
{

    public class SECSHandler
    {
        private static Logger trxLogger = LogManager.GetLogger("SECSTrxLogger");
        private static Logger SXFYConvertLogger = NLog.LogManager.GetLogger("SXFYConvertLogger");
        private SECSAgent agent = null;
        public SECSAgent Agent { get { return agent; } }
        public SECSHandler(SECSAgent agent)
        {
            this.agent = agent;
        }

        public SXFY Parse<T>(SECSEventArgs e, T target_obj)
        {
            return Parse<T>(e);
        }
        public SXFY Parse<T>(SECSEventArgs e)
        {
            SXFY tmp = (SXFY)Activator.CreateInstance(typeof(T));
            if (tmp.IsBaseType && e.BaseSXFYData != null && typeof(T).Name == e.BaseSXFYData.GetType().Name)
                return e.BaseSXFYData;
            if (!tmp.IsBaseType && e.ExtendSXFYData != null && typeof(T).Name == e.ExtendSXFYData.GetType().Name)
            {
                return e.ExtendSXFYData;
            }
            string rcvSF = string.Format("S{0}F{1}", e.S, e.F);
            try
            {
                SXFY rtnSXFY = Parse<T>(e.RawData, e.SystemBytes, e.W_Bit, rcvSF, e.Head);
                if (rtnSXFY.IsBaseType)
                {
                    e.BaseSXFYData = rtnSXFY;
                }
                else
                {
                    e.ExtendSXFYData = rtnSXFY;
                }
                return rtnSXFY;
            }
            catch (SECSFormatException sfex)
            {
                trxLogger.Warn("Format Error. Send S9F7.[Exception:{0}]", sfex);
                string message = "Illegal Data Value Format. Send S9F7.";
                SECSAgent.FormatExceptionMonitor.FormatExceptionItem = new SECSFormatExceptionItem()
                {
                    FormatException = new SECSFormatException(message),
                    Message = message
                };
                trxLogger.Warn(message);
                S9F7 s9f7 = new S9F7()
                {
                    SECSAgentName = Agent.Name,
                    SystemByte = e.SystemBytes,
                };
                int[] headAry = e.Head as int[];
                s9f7.MHEAD = SEUtility.binary2String(headAry);
                agent.replySECSMsg(s9f7);
                throw sfex;
            }
        }

        protected SXFY Parse<T>(Object rawData, int systemByte, int w_Bit, string streamFunction, Object head)
        {
            SXFYConvertLogger.Debug("Start Parse from raw Data to SXFY[SystemByte:{0}]", systemByte);
            DateTime startTime = DateTime.Now;
            SXFY sxfy = null;
            try
            {
                int offset = 0;
                int length = 0;
                int fieldIndex = 0;
                if (!SEUtility.isSXFYType(typeof(T)))
                {
                    throw new Exception("Invalid Type!");
                }
                Type t = rawData.GetType();
                Boolean isListSpreadOut = SEUtility.isListSpreadOutSXFY(typeof(T));        //A0.04

                sxfy = (SXFY)Activator.CreateInstance(typeof(T));
                sxfy.SECSAgentName = Agent.Name;
                sxfy.SystemByte = systemByte;
                sxfy.W_Bit = w_Bit;

                if (t.IsArray == false || ((Array)rawData).Length == 0)
                {
                    return sxfy;
                }
                length = ((Array)rawData).Length;
                FieldInfo[] fields = SEUtility.GetSeceElementFields(sxfy.GetType());
                if (!isListSpreadOut)
                {
                    //必須先將最外層的「L」取出
                    QSACTIVEXLib.SECSII_DATA_TYPE lItemType = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;

                    lItemType = agent.getQSWrapper().GetDataItemType(ref rawData, offset);

                    int lItemNum = 0;
                    try
                    {
                        if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE)
                        {
                            Object itemData = null;
                            offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemNum, ref itemData);
                            if (fields.Length != lItemNum && !sxfy.IsBaseType)
                            {
                                trxLogger.Info(string.Format("Stream Function[{0}] Format Error.[{1}][System Byte:{2}]",
                                    sxfy.StreamFunction, "Filed Count is not match!", sxfy.SystemByte));
                                throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[{1}][System Byte:{2}]",
                                    sxfy.StreamFunction, "Filed Count is not match!", sxfy.SystemByte)); //A0.05
                            }
                        }
                        else
                        {
                            //Format Error.
                            throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}]",
                                sxfy.StreamFunction, sxfy.SystemByte)); //A0.05
                        }
                    }
                    catch (Exception ex)
                    {

                        trxLogger.ErrorException("Parse Exception!", ex);
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}]",
                                sxfy.StreamFunction, sxfy.SystemByte));
                    }
                }
                else
                {
                    //如果是ListSpreadOut情況，則裡面的Field不可能有多個。
                    if (fields.Length > 1)
                    {
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[{1}][System Byte:{2}]",
                        sxfy.StreamFunction, "Filed Count must be only one!", sxfy.SystemByte)); //A0.05
                    }
                }
                try
                {
                    SXFYConvertLogger.Info("Start Call parseFromSECS[SystemByte:{0}]", systemByte);
                    parseFromSECS(typeof(T).Name, ref sxfy, ref fieldIndex, ref offset, ref length, rawData, systemByte);
                    SXFYConvertLogger.Info("Finish Call parseFromSECS[SystemByte:{0}]", systemByte);
                }
                catch (SECSFormatException sfex)
                {
                    throw sfex;
                }
            }
            catch (SECSFormatException sfex)
            {
                if (sxfy.ValidateFormat)
                {
                    //SECSAgent.FormatExceptionMonitor.FormatExceptionItem = new SECSFormatExceptionItem()
                    //{
                    //    FormatException = sfex,
                    //    Message = sfex.Message
                    //};
                    ////Format Error. (S9F7)
                    //trxLogger.WarnException("Format Error. Send S9F7.", sfex);
                    //S9F7 s9f7 = new S9F7()
                    //{
                    //    SystemByte = systemByte,
                    //};
                    //int[] headAry = head as int[];
                    //s9f7.MHEAD = SEUtility.binary2String(headAry);
                    //agent.replySECSMsg(s9f7);
                    throw sfex;
                }
            }
            //To check data format
            com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT dataValueChkResult =
                agent.checkIllegalDataValueFormat(typeof(T), sxfy);
            if (dataValueChkResult == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format)
            {
                string message = "Illegal Data Value Format. Send S9F7.";
                SECSAgent.FormatExceptionMonitor.FormatExceptionItem = new SECSFormatExceptionItem()
                {
                    FormatException = new SECSFormatException(message),
                    Message = message
                };
                trxLogger.Warn(message);
                S9F7 s9f7 = new S9F7()
                {
                    SECSAgentName = Agent.Name,
                    SystemByte = systemByte,
                };
                int[] headAry = head as int[];
                s9f7.MHEAD = SEUtility.binary2String(headAry);
                agent.replySECSMsg(s9f7);
                throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}]",
                                sxfy.StreamFunction, sxfy.SystemByte));
            }
            double totalMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            SXFYConvertLogger.Debug("Finish Parse from raw Data to SXFY[SystemByte:{0}][Total Milliseconds:{1}]", systemByte, totalMS);
            return sxfy;
        }

        //        protected SXFY Parse<T>(Object rawData, int systemByte, int w_Bit, string streamFunction, Object head) 
        //        {
        //            SXFYConvertLogger.Debug("Start Parse from raw Data to SXFY[SystemByte:{0}]", systemByte);
        //            DateTime startTime = DateTime.Now;
        //            SXFY sxfy = null;
        //            try
        //            {
        //                int offset = 0;
        //                int length = 0;
        //                int fieldIndex = 0;
        //                if (!SEUtility.isSXFYType(typeof(T)))
        //                //            if (typeof(T).BaseType != typeof(SXFY)) 
        //                {
        //                    throw new Exception("Invalid Type!");
        //                }
        //                Type t = rawData.GetType();
        //                Boolean isListSpreadOut = SEUtility.isListSpreadOutSXFY(typeof(T));        //A0.04
        //                //A0.02 Begin
        //                sxfy = (SXFY)Activator.CreateInstance(typeof(T));
        //                sxfy.SystemByte = systemByte;
        //                sxfy.W_Bit = w_Bit;
        //                //A0.02 End
        //                if (t.IsArray == false || ((Array)rawData).Length == 0)
        //                {
        //                    return sxfy;
        //                }
        //                //
        //                //A0.02            sxfy = (SXFY)Activator.CreateInstance(typeof(T));
        //                //A0.02            sxfy.SystemByte = systemByte;
        //                //A0.02            sxfy.W_Bit = w_Bit;
        //                length = ((Array)rawData).Length;
        //                FieldInfo[] fields = SEUtility.GetSeceElementFields(sxfy.GetType());
        //                if (!isListSpreadOut)
        //                {
        //                    //必須先將最外層的「L」取出
        //                    QSACTIVEXLib.SECSII_DATA_TYPE lItemType = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;

        ////A0.06                    lItemType = getDataItemType(ref rawData, offset);
        //                    lItemType = agent.getQSWrapper().GetDataItemType(ref rawData, offset);

        //                    int lItemNum = 0;
        //                    try
        //                    {
        //                        if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE)
        //                        {
        //                            Object itemData = null;
        ////A0.06                            offset = dataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemNum, ref itemData);
        //                            offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemNum, ref itemData);
        //                            if (fields.Length != lItemNum && !sxfy.IsBaseType)
        //                            {
        //                                trxLogger.Info(string.Format("Stream Function[{0}] Format Error.[{1}][System Byte:{2}]",
        //                                    sxfy.StreamFunction, "Filed Count is not match!", sxfy.SystemByte));
        //                                throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[{1}][System Byte:{2}]",
        //                                    sxfy.StreamFunction, "Filed Count is not match!", sxfy.SystemByte)); //A0.05
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //Format Error.
        //                            //throw new SECSFormatException("");
        //                            throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}]",
        //                                sxfy.StreamFunction, sxfy.SystemByte)); //A0.05
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                        trxLogger.ErrorException("Parse Exception!", ex);
        //                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}]",
        //                                sxfy.StreamFunction, sxfy.SystemByte));
        ////                        return sxfy;
        //                    }
        //                }
        //                else
        //                {
        //                    //如果是ListSpreadOut情況，則裡面的Field不可能有多個。
        //                    if (fields.Length > 1)
        //                    {
        //                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[{1}][System Byte:{2}]",
        //                        sxfy.StreamFunction, "Filed Count must be only one!", sxfy.SystemByte)); //A0.05
        //                    }
        //                }
        //                //
        //                try
        //                {
        //                    //
        ////A0.06                    ParseSECSParameterStruct parseSECSParaStruct = new ParseSECSParameterStruct() 
        ////A0.06                    {
        ////A0.06                        FieldIndex = fieldIndex,
        ////A0.06                        Length = length,
        ////A0.06                        Offset = offset,
        ////A0.06                        RawData = rawData,
        ////A0.06                        Sxfy = sxfy,
        ////A0.06                        SystemByte = systemByte,
        ////A0.06                        StreamFunctionName = typeof(T).Name
        ////A0.06                    };
        //                    SXFYConvertLogger.Debug("Start Call parseFromSECS[SystemByte:{0}]", systemByte);
        ////A0.06                    ThreadPool.QueueUserWorkItem(parseFromSECS, parseSECSParaStruct);
        //                    parseFromSECS(typeof(T).Name, ref sxfy, ref fieldIndex, ref offset, ref length, rawData, systemByte);
        ////A0.06                    parseSECSParaStruct.eventWaitHandle.WaitOne();
        ////A0.06                    sxfy = parseSECSParaStruct.Sxfy;
        //                    SXFYConvertLogger.Debug("Finish Call parseFromSECS[SystemByte:{0}]", systemByte);
        ////A0.06                    if (parseSECSParaStruct.hasException) 
        ////A0.06                    {
        ////A0.06                        throw parseSECSParaStruct.SecsFormatException;
        ////A0.06                    }
        //                    //
        ////                    parseFromSCES(typeof(T).Name, ref sxfy, ref fieldIndex, ref offset, ref length, rawData, systemByte);
        //                }
        //                catch (SECSFormatException sfex)
        //                {
        //                    ////Format Error. (S9F7)
        //                    //trxLogger.WarnException("Format Error. Send S9F7.", sfex);
        //                    //S9F7 s9f7 = new S9F7()
        //                    //{
        //                    //    SystemByte = systemByte
        //                    //};
        //                    //agent.replySECSMsg(s9f7);
        //                    throw sfex;
        //                }
        //            }
        //            catch (SECSFormatException sfex) 
        //            {
        //                if (sxfy.ValidateFormat)
        //                {
        //                    SECSAgent.FormatExceptionMonitor.FormatExceptionItem = new SECSFormatExceptionItem()
        //                    {
        //                        FormatException = sfex,
        //                        Message = sfex.Message
        //                    };
        //                    //Format Error. (S9F7)
        //                    trxLogger.WarnException("Format Error. Send S9F7.", sfex);
        //                    S9F7 s9f7 = new S9F7()
        //                    {
        //                        SystemByte = systemByte,
        ////                        MHEAD = streamFunction
        //                    };
        //                    int[] headAry = head as int[];
        //                    s9f7.MHEAD = SEUtility.binary2String(headAry);
        //                    agent.replySECSMsg(s9f7);
        //                }
        //            }
        //            //To check data format
        //            com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT dataValueChkResult = 
        //                agent.checkIllegalDataValueFormat(typeof(T), sxfy);
        //            if (dataValueChkResult == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format) 
        //            {
        //                string message = "Illegal Data Value Format. Send S9F7.";
        //                SECSAgent.FormatExceptionMonitor.FormatExceptionItem = new SECSFormatExceptionItem()
        //                {
        //                    FormatException = new SECSFormatException(message),
        //                    Message = message
        //                };
        //                trxLogger.Warn(message);
        //                S9F7 s9f7 = new S9F7()
        //                {
        //                    SystemByte = systemByte,
        ////                    MHEAD = streamFunction
        //                };
        //                int[] headAry = head as int[];
        //                s9f7.MHEAD = SEUtility.binary2String(headAry);
        //                agent.replySECSMsg(s9f7);
        //            }
        //            double totalMS = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        //            SXFYConvertLogger.Debug("Finish Parse from raw Data to SXFY[SystemByte:{0}][Total Milliseconds:{1}]", systemByte, totalMS);
        //            return sxfy;
        //        }

        private class ParseSECSParameterStruct
        {
            public string StreamFunctionName { get; set; }
            public SXFY Sxfy { get; set; }
            public int FieldIndex { get; set; }
            public int Offset { get; set; }
            public int Length { get; set; }
            public Object RawData { get; set; }
            public int SystemByte { get; set; }
            public EventWaitHandle eventWaitHandle = new ManualResetEvent(false);
            public SECSFormatException SecsFormatException { get; set; }
            public Boolean hasException { get; set; }

            public ParseSECSParameterStruct()
            {
                hasException = false;
            }
        }

        private class DataItemStruct
        {
            public Object RawData { get; set; }
            public int Offset { get; set; }
            public QSACTIVEXLib.SECSII_DATA_TYPE ItemType { get; set; }
            public int ItemNum { get; set; }
            public Object ItemData { get; set; }
            public int SkipItem { get; set; }
            public EventWaitHandle eventWaitHandle = new ManualResetEvent(false);

        }
        private void doGetDataItemType(Object obj)
        {
            DataItemStruct diStruct = obj as DataItemStruct;
            diStruct.ItemType = agent.getQSWrapper().GetDataItemType(diStruct.RawData, diStruct.Offset);
            //diStruct.WaitHandle.Set();
            diStruct.eventWaitHandle.Set();
        }
        private QSACTIVEXLib.SECSII_DATA_TYPE getDataItemType(ref Object rawData, int offset)
        {
            DataItemStruct dataItemStruct = new DataItemStruct()
            {
                RawData = rawData,
                Offset = offset
            };
            ThreadPool.QueueUserWorkItem(doGetDataItemType, dataItemStruct);
            dataItemStruct.eventWaitHandle.WaitOne();
            QSACTIVEXLib.SECSII_DATA_TYPE lItemType = dataItemStruct.ItemType;
            return lItemType;
        }
        private void doDataItemIn(Object obj)
        {
            DataItemStruct diStruct = obj as DataItemStruct;
            Object rawData = diStruct.RawData;
            Object itemData = diStruct.ItemData;
            int itemNum = 0;
            diStruct.Offset = agent.getQSWrapper().DataItemIn(ref rawData, diStruct.Offset, diStruct.ItemType,
                                       out itemNum, ref itemData);
            diStruct.ItemNum = itemNum;
            diStruct.ItemData = itemData;
            diStruct.eventWaitHandle.Set();
        }
        private int dataItemIn(ref Object rawData, int offset, QSACTIVEXLib.SECSII_DATA_TYPE itemType, out int itemNum, ref Object itemData)
        {
            SXFYConvertLogger.Debug("Begin dataItemIn [Offset:{0}][Thread:{1}]", offset, Thread.CurrentThread.ManagedThreadId);
            itemNum = 0;
            DataItemStruct dataItemStruct = new DataItemStruct()
            {
                RawData = rawData,
                Offset = offset,
                ItemType = itemType,
                ItemData = itemData
            };
            ThreadPool.QueueUserWorkItem(doDataItemIn, dataItemStruct);
            dataItemStruct.eventWaitHandle.WaitOne();
            itemNum = dataItemStruct.ItemNum;
            itemData = dataItemStruct.ItemData;
            offset = dataItemStruct.Offset;
            SXFYConvertLogger.Debug("Finish dataItemIn [Offset:{0}][Thread:{1}]", offset, Thread.CurrentThread.ManagedThreadId);
            return offset;
        }
        private void doDataItemInSkip(Object obj)
        {
            //offset = agent.getQSWrapper().DataItemInSkip(rawData, offset, 1);
            DataItemStruct diStruct = obj as DataItemStruct;
            Object rawData = diStruct.RawData;
            diStruct.Offset = agent.getQSWrapper().DataItemInSkip(ref rawData, diStruct.Offset, diStruct.SkipItem);
            diStruct.eventWaitHandle.Set();
        }
        private int dataItemSkip(ref Object rawData, int offset, int skipItem)
        {
            DataItemStruct dataItemStruct = new DataItemStruct()
            {
                RawData = rawData,
                Offset = offset,
                SkipItem = skipItem
            };
            ThreadPool.QueueUserWorkItem(doDataItemInSkip, dataItemStruct);
            dataItemStruct.eventWaitHandle.WaitOne();
            offset = dataItemStruct.Offset;
            return offset;
        }

        private void parseFromSECS(Object parseSECSParameter)
        {
            ParseSECSParameterStruct psStruct = parseSECSParameter as ParseSECSParameterStruct;
            try
            {
                string streamFunctionName = psStruct.StreamFunctionName;
                SXFY sxfy = psStruct.Sxfy;
                int fieldIndex = psStruct.FieldIndex;
                int offset = psStruct.Offset;
                int length = psStruct.Length;
                Object rawData = psStruct.RawData;
                int systemByte = psStruct.SystemByte;
                SXFYConvertLogger.Debug("Start Do parseFromSECS[SystemByte:{0}]", systemByte);
                parseFromSECS(streamFunctionName, ref sxfy, ref fieldIndex, ref offset, ref length, rawData, systemByte);
                psStruct.Sxfy = sxfy;
                psStruct.FieldIndex = fieldIndex;
                psStruct.Offset = offset;
                psStruct.Length = length;
                SXFYConvertLogger.Debug("Finish Do parseFromSECS[SystemByte:{0}]", systemByte);
            }
            catch (SECSFormatException sfex)
            {
                psStruct.hasException = true;
                psStruct.SecsFormatException = sfex;
            }
            psStruct.eventWaitHandle.Set();
        }

        private void parseFromSECS(string streamFunctionName,
            ref SXFY sxfy, ref int fieldIndex, ref int offset, ref int length, Object rawData, int systemByte)
        {
            FieldInfo[] fields = null;
            try
            {
                fields = SEUtility.GetSeceElementFields(sxfy.GetType());
            }
            catch
            {
                fields = new FieldInfo[0];
            }
            if (fieldIndex >= fields.Length)
            {
                return;
            }
            FieldInfo field = fields[fieldIndex];
            while (field.FieldType.IsInterface)
            {
                ++fieldIndex;
                if (fieldIndex >= fields.Length)
                {
                    return;
                }
                field = fields[fieldIndex];
            }
            QSACTIVEXLib.SECSII_DATA_TYPE lItemType;
            int lItemNum = 0;
            Object ItemData = null;
            lItemType = agent.getQSWrapper().GetDataItemType(ref rawData, offset);
            //            lItemType = getDataItemType(ref rawData, offset);

            SecsElement secsElement = SEUtility.getSecsElementAttr(field);
            if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE)
            {
                Object itemData = null;
                offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE,
                                       out lItemNum, ref itemData);
                //                offset = dataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemNum, ref itemData);
                //往裡面走訪
                if (field.FieldType.IsArray && field.FieldType.GetElementType().BaseType == typeof(SXFY))
                {
                    try
                    {
                        Type subType = field.FieldType.GetElementType();
                        SXFY[] subArray = (SXFY[])Array.CreateInstance(subType, lItemNum);
                        field.SetValue(sxfy, (subArray));
                        for (int index = 0; index < lItemNum; ++index)
                        {
                            subArray[index] = (SXFY)Activator.CreateInstance(subType);
                        }
                        for (int index = 0; index < lItemNum; ++index)
                        {
                            int subSXFYFieldIndex = 0;
                            SXFY subSxFy = subArray[index];
                            QSACTIVEXLib.SECSII_DATA_TYPE subItemType = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;

                            subItemType = agent.getQSWrapper().GetDataItemType(ref rawData, offset);
                            //                            subItemType = getDataItemType(ref rawData, offset);

                            int subItemNum = 0;
                            if (subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE)
                            {
                                itemData = null;
                                offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE,
                                       out subItemNum, ref itemData);
                                //                                offset = dataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out subItemNum, ref itemData);
                            }
                            parseFromSECS(streamFunctionName, ref subSxFy, ref subSXFYFieldIndex, ref offset, ref length, rawData, systemByte);
                        }
                    }
                    catch (Exception ex)
                    {
                        trxLogger.ErrorException("parseFromSCES Exception!", ex);
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]", streamFunctionName, field.Name)); //A0.05
                    }
                }
                else if (field.FieldType.IsArray && field.FieldType.GetElementType() == typeof(string))
                {
                    try
                    {
                        //A0.03 Begin
                        string[] subArray = new string[lItemNum];
                        for (int index = 0; index < lItemNum; ++index)
                        {
                            subArray[index] = string.Empty;
                        }
                        for (int index = 0; index < lItemNum; ++index)
                        {
                            //subArray[index]
                            QSACTIVEXLib.SECSII_DATA_TYPE subItemType = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
                            subItemType = agent.getQSWrapper().GetDataItemType(ref rawData, offset);
                            //                            subItemType = getDataItemType(ref rawData, offset);
                            if (subItemType != SecsElement.convertToSECSIIDataType(secsElement.ListElementType))
                            {
                                throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                                    streamFunctionName, field.Name));
                            }
                            int elementLength = 0;
                            offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, subItemType,
                                       out elementLength, ref ItemData);
                            //                            offset = dataItemIn(ref rawData, offset, subItemType, out elementLength, ref ItemData);
                            if (subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE)
                            {
                                if (ItemData != null && secsElement.ListElementLength < (ItemData as string).Trim().Length)
                                {
                                    throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                                    streamFunctionName, field.Name));
                                }
                                subArray[index] = ItemData as string;
                            }
                            //B0.03 else if (subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE)
                            else if (subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE    //B0.03
                                    || subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_2_TYPE  //B0.03
                                    || subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE  //B0.03
                                    || subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE //B0.03
                                    || subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE //B0.03
                                    || subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE)//B0.03
                            {
                                //subArray[index] = Convert.ToString(ItemData);
                                int[] U1 = (int[])ItemData;
                                subArray[index] = Convert.ToString(U1[0]);
                            }
                        }
                        field.SetValue(sxfy, subArray);
                        //A0.03 End
                    }
                    catch (Exception ex)
                    {
                        trxLogger.ErrorException("parseFromSCES Exception!", ex);
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]", streamFunctionName, field.Name)); //A0.05
                    }
                }
                else if (field.FieldType.BaseType == typeof(SXFY))
                {
                    Type subType = field.FieldType;
                    SXFY subElement = (SXFY)Activator.CreateInstance(subType);
                    try
                    {
                        field.SetValue(sxfy, subElement);
                    }
                    catch (Exception ex)
                    {
                        trxLogger.ErrorException("parseFromSCES Exception!", ex);
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]", streamFunctionName, field.Name)); //A0.05
                    }
                    QSACTIVEXLib.SECSII_DATA_TYPE subItemType = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
                    subItemType = agent.getQSWrapper().GetDataItemType(ref rawData, offset);
                    //                    subItemType = getDataItemType(ref rawData, offset);
                    int subItemNum = 0;
                    //A0.01                    if (subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE)
                    //A0.01                    {
                    //A0.01                        itemData = null;
                    //A0.01                        offset = dataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out subItemNum, ref itemData);
                    //A0.01                    }
                    //B0.02 Start
                    if (subItemType == QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE)
                    {
                        FieldInfo[] fieldsTemp = SEUtility.GetSeceElementFields(subElement.GetType());
                        if (fieldsTemp.Length != lItemNum)
                        {
                            trxLogger.Info(string.Format("Stream Function[{0}] Format Error.[{1}][System Byte:{2}]",
                                sxfy.StreamFunction, "Filed Count is not match!", sxfy.SystemByte));
                            throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[{1}][System Byte:{2}]",
                                sxfy.StreamFunction, "Filed Count is not match!", sxfy.SystemByte)); //A0.05

                        }
                    }
                    //B0.02 End
                    int subSXFYFieldIndex = 0;
                    parseFromSECS(streamFunctionName, ref subElement, ref subSXFYFieldIndex, ref offset, ref length, rawData, systemByte);
                }
                else
                {
                    //SECSFormatException
                    trxLogger.Error("Stream Function[{0}] Format Error.");
                    throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}][Field:{2}]",
                        streamFunctionName, sxfy.SystemByte, field.Name)); //A0.05
                }
            }
            else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE)
            {
                try
                {
                    //B0.01 if (lItemType != SecsElement.convertToSECSIIDataType(secsElement.ListElementType))
                    if (lItemType != SecsElement.convertToSECSIIDataType(secsElement.Type)//B0.01
                        && QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE != SecsElement.convertToSECSIIDataType(secsElement.Type)) //B0.04
                    {
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                            streamFunctionName, field.Name));
                    }
                    offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE,
                                       out lItemNum, ref ItemData);
                    //                    offset = dataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, out lItemNum, ref ItemData);

                    if (ItemData != null && secsElement.Length < (ItemData as string).Trim().Length)
                    {
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                        streamFunctionName, field.Name));
                    }
                    field.SetValue(sxfy, ItemData);
                }
                catch (Exception ex)
                {
                    trxLogger.ErrorException("parseFromSCES Exception!", ex);
                    throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}][Field:{2}]",
                        streamFunctionName, sxfy.SystemByte, field.Name)); //A0.05
                }
            }
            else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE)
            {
                try
                {
                    //B0.01 if (lItemType != SecsElement.convertToSECSIIDataType(secsElement.ListElementType))
                    if (lItemType != SecsElement.convertToSECSIIDataType(secsElement.Type)  //B0.01
                        && QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE != SecsElement.convertToSECSIIDataType(secsElement.Type))//B0.04
                    {
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                            streamFunctionName, field.Name));
                    }
                    offset = agent.getQSWrapper().DataItemIn(ref rawData, offset,
                        QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, out lItemNum, ref ItemData);
                    string bStr = "";
                    int[] tData = (int[])ItemData;
                    for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                    {
                        bStr = bStr + " 0x" + Convert.ToString(tData[intIndex], 16).ToUpper();
                    }
                    field.SetValue(sxfy, bStr);
                }
                catch (Exception ex)
                {
                    trxLogger.ErrorException("parseFromSCES Exception!", ex);
                    throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}][Field:{2}]",
                        streamFunctionName, sxfy.SystemByte, field.Name)); //A0.05
                }
            }
            //B0.04 Start
            else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE)
            {
                try
                {
                    if (lItemType != SecsElement.convertToSECSIIDataType(secsElement.Type)
                        && QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE != SecsElement.convertToSECSIIDataType(secsElement.Type)) //A0.09
                    {
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                            streamFunctionName, field.Name));
                    }
                    offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE,
                                       out lItemNum, ref ItemData);
                    int[] U1 = (int[])ItemData;

                    //field.SetValue(sxfy, U1[0].ToString());
                    field.SetValue(sxfy, U1 != null ? string.Join(",", U1) : null);
                }
                catch (Exception ex)
                {
                    trxLogger.ErrorException("parseFromSCES Exception!", ex);
                    throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}][Field:{2}]",
                        streamFunctionName, sxfy.SystemByte, field.Name)); //A0.05
                }
            }
            else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE)
            {
                try
                {
                    if (lItemType != SecsElement.convertToSECSIIDataType(secsElement.Type)
                        && QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE != SecsElement.convertToSECSIIDataType(secsElement.Type)) //A0.09
                    {
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                            streamFunctionName, field.Name));
                    }
                    offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE,
                                       out lItemNum, ref ItemData);

                    int[] U2 = (int[])ItemData;
                    //field.SetValue(sxfy, U2[0].ToString());
                    field.SetValue(sxfy, U2 != null ? string.Join(",", U2) : null);
                }
                catch (Exception ex)
                {
                    trxLogger.ErrorException("parseFromSCES Exception!", ex);
                    throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}][Field:{2}]",
                        streamFunctionName, sxfy.SystemByte, field.Name)); //A0.05
                }
            }
            else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE)
            {
                try
                {
                    if (lItemType != SecsElement.convertToSECSIIDataType(secsElement.Type)
                        && QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE != SecsElement.convertToSECSIIDataType(secsElement.Type)) //A0.09
                    {
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                            streamFunctionName, field.Name));
                    }
                    offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE,
                                       out lItemNum, ref ItemData);

                    int[] U4 = (int[])ItemData;
                    //field.SetValue(sxfy, U4 == null ? string.Empty : U4[0].ToString()); //2015/10/08 修正By Kevin
                    field.SetValue(sxfy, U4 != null ? string.Join(",", U4) : null); //2015/10/08 修正By Kevin
                }
                catch (Exception ex)
                {
                    trxLogger.ErrorException("parseFromSCES Exception!", ex);
                    throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}][Field:{2}]",
                        streamFunctionName, sxfy.SystemByte, field.Name)); //A0.05
                }
            }
            else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE)
            {
                try
                {
                    if (lItemType != SecsElement.convertToSECSIIDataType(secsElement.Type)
                        && QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE != SecsElement.convertToSECSIIDataType(secsElement.Type)) //A0.09
                    {
                        throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[Field:{1}]",
                            streamFunctionName, field.Name));
                    }
                    offset = agent.getQSWrapper().DataItemIn(ref rawData, offset, QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE,
                                       out lItemNum, ref ItemData);

                    int[] i = (int[])ItemData;
                    //Boolean b = i[0] == 0 ? false : true;
                    //field.SetValue(sxfy, b.ToString());
                    field.SetValue(sxfy, i);
                }
                catch (Exception ex)
                {
                    trxLogger.ErrorException("parseFromSCES Exception!", ex);
                    throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}][Field:{2}]",
                        streamFunctionName, sxfy.SystemByte, field.Name)); //A0.05
                }
            }
            //B0.04 End
            else
            {
                //A0.05                offset = dataItemSkip(ref rawData, offset, 1);
                throw new SECSFormatException(string.Format("Stream Function[{0}] Format Error.[System Byte:{1}][Field:{2}]",
                    streamFunctionName, sxfy.SystemByte, field.Name)); //A0.05
            }
            ++fieldIndex;
            parseFromSECS(streamFunctionName, ref sxfy, ref fieldIndex, ref offset, ref length, rawData, systemByte);
        }

        private void AppendIndent(ref StringBuilder mySb, int myIndentLevel)
        {
            for (int i = 0; i < myIndentLevel; i++)
            {
                mySb.Append("  ");
            }
        }

        public string secs2String(object myRawData)
        {
            SXFYConvertLogger.Debug("Begin secs2String");
            StringBuilder strSecsMessage = new StringBuilder();
            //object myRawData = objData.RawData;
            int[] myStack = new int[10];
            int myTestCount; // 2013.09.16
            int myStackPtr;
            int lOffset;
            int lItemNum;
            object ItemData = null;
            int lLength;
            QSACTIVEXLib.SECSII_DATA_TYPE lItemType;
            string DisplayString;
            //int i;

            // Verify whether the input data is an array or not
            System.Array myArray = myRawData as System.Array;
            if (myArray != null)
                lLength = myArray.Length;
            else
                return strSecsMessage.ToString();

            if (myArray.Length == 0)
                return strSecsMessage.ToString();

            myTestCount = 0;
            myStackPtr = 0;
            lOffset = 0;

            //int lOffset1 = 0;

            try
            {
                while (lOffset < lLength)
                {
                    if (myStackPtr > 0)
                    {
                        if (myStack[myStackPtr - 1] > 0)
                        {
                            myStack[myStackPtr - 1] = myStack[myStackPtr - 1] - 1;
                        }
                        else
                        {
                            myStackPtr = myStackPtr - 1;
                            if (myStackPtr == 0)
                            {
                                // force show end. Normal message should not run to here

                                strSecsMessage.Append("> \r\n");

                                strSecsMessage.Append(". \r\n");
                                return strSecsMessage.ToString();
                            }

                            while (myStack[myStackPtr] == 0) // (if) 表示L下為0個
                            {
                                // 09.16
                                myTestCount = 0;

                                AppendIndent(ref strSecsMessage, myStackPtr);
                                strSecsMessage.Append(">" + "\r\n");
                                myStackPtr = myStackPtr - 1;
                            }
                            myStackPtr = myStackPtr + 1;
                            myStack[myStackPtr - 1] = myStack[myStackPtr - 1] - 1;
                        }
                    }
                    lItemType = agent.getQSWrapper().GetDataItemType(myRawData, lOffset);
                    //                    lItemType = getDataItemType(ref myRawData, lOffset);
                    if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE)
                    {
                        lItemNum = 99;

                        object objTemp1 = null;
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemNum, ref objTemp1);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemNum, ref objTemp1);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, out lItemNum, ref objTemp1);
                        // Display the data item
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        strSecsMessage.Append("<L[" + lItemNum + "]" + "\r\n");
                        // 2013.09.16
                        int intL = myStackPtr + 1;
                        //Test Log                        strSecsMessage.Append("<L Length = \"" + lItemNum + "\" ID = \"L" + intL + "\">");
                        // Increase the indent level
                        myStack[myStackPtr] = lItemNum; // 記錄有每個L包含的元素數
                        myStackPtr = myStackPtr + 1;
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        strSecsMessage.Append("<A[" + lItemNum + "] " + (char)34 + ItemData + (char)34 + ">" + "\r\n");
                        // 2013.09.16
                        myTestCount = myTestCount + 1;
                        //int intL = myTestCount - myStackPtr;
                        //Test Log                        strSecsMessage.Append("<A Length = \"" + lItemNum + "\" ID = \"" + myTestCount + "\">" + ItemData + "</A>");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.JIS_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.JIS_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.JIS_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.JIS_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        strSecsMessage.Append("<J[" + lItemNum + "] " + (char)34 + ItemData + (char)34 + ">" + "\r\n");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";

                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " 0x" + Convert.ToString(tData[intIndex], 16).ToUpper();
                        }
                        strSecsMessage.Append("<B[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";

                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " 0x" + Convert.ToString(tData[intIndex], 16).ToUpper();
                        }
                        strSecsMessage.Append("<Boolean[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";
                        uint[] tData = (uint[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        strSecsMessage.Append("<U1[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";
                        uint[] tData = (uint[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        strSecsMessage.Append("<U2[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                        // 2013.09.16
                        myTestCount = myTestCount + 1;
                        //Test Log                        strSecsMessage.Append("<U2 Length = \"" + lItemNum + "\" ID = \"" + myTestCount + "\">" + DisplayString + "</U2>");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";
                        uint[] tData = (uint[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        strSecsMessage.Append("<U4[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                        // 2013.09.16
                        myTestCount = myTestCount + 1;
                        //Test Log                        strSecsMessage.Append("<U4 Length = \"" + lItemNum + "\" ID = \"" + myTestCount + "\">" + DisplayString + "</U4>");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";
                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        strSecsMessage.Append("<I1[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_2_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_2_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_2_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_2_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";
                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        strSecsMessage.Append("<I2[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";
                        int[] tData = (int[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        strSecsMessage.Append("<I4[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.FT_4_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.FT_4_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.FT_4_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.FT_4_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";
                        Single[] tData = (Single[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        strSecsMessage.Append("<F4[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                    }
                    else if (lItemType == QSACTIVEXLib.SECSII_DATA_TYPE.FT_8_TYPE)
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.FT_8_TYPE, out lItemNum, ref ItemData);
                        lOffset = agent.getQSWrapper().DataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.FT_8_TYPE, out lItemNum, ref ItemData);
                        //                        lOffset = dataItemIn(ref myRawData, lOffset, QSACTIVEXLib.SECSII_DATA_TYPE.FT_8_TYPE, out lItemNum, ref ItemData);
                        AppendIndent(ref strSecsMessage, myStackPtr);
                        DisplayString = "";
                        double[] tData = (double[])ItemData;

                        for (int intIndex = 0; intIndex <= (lItemNum - 1); intIndex++)
                        {
                            DisplayString = DisplayString + " " + Convert.ToString(tData[intIndex]);
                        }
                        strSecsMessage.Append("<F8[" + lItemNum + "]" + DisplayString + ">" + "\r\n");
                    }
                    else
                    {
                        //                        lOffset = agent.getQSWrapper().DataItemInSkip(ref myRawData, lOffset, 1);
                        lOffset = agent.getQSWrapper().DataItemInSkip(ref myRawData, lOffset, 1);
                        //                        lOffset = dataItemSkip(ref myRawData, lOffset, 1);
                    }
                }

            }
            catch (Exception ex)
            {
                trxLogger.ErrorException("secs2String has error!", ex);
            }

            while (myStackPtr > 0)
            {
                myStackPtr = myStackPtr - 1;
                //Test Log                strSecsMessage.Append("</L>");
                AppendIndent(ref strSecsMessage, myStackPtr);
                strSecsMessage.Append(">" + "\r\n");
            }
            SXFYConvertLogger.Debug("Finish secs2String");
            return strSecsMessage.ToString();
        }

    }
}
