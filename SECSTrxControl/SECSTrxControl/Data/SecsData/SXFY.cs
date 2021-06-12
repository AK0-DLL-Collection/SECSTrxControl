//*********************************************************************************
//      SXFY.cs
//*********************************************************************************
// File Name: SXFY.cs
// Description: SECS Base Class
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/05/30    Hayes Chen     N/A            N/A     Initial Release
// 2014/08/31    Hayes Chen     N/A            A0.01   加上考慮String Array的情況
// 2014/09/01    Hayes Chen     N/A            A0.02   加上SXFY Class直接Spread Out Field的情況
// 2014/09/05    Hayes Chen     N/A            A0.03   加上指定是否允許Abort Stream Function Command
// 2015/03/24    Hayes Chen     N/A            A0.04   在Parallel.For內加上Sleep，故意讓CPU多出空檔，
//                                                     使Parallel.For可以動態再增加Thread進行處理，提昇整體處理速度
// 2015/06/11    Kevin Wei      N/A            A0.05   將Parser2RawData中Theard的設定:MaxtTreadCount、SleepCount、SleepTime設成Config。
// 2016/03/09    Kevin Wei      N/A            A0.06   將Thread.Sleep的用法改成SpinWait()
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using NLog;

namespace com.mirle.ibg3k0.stc.Data.SecsData
{
    public class SXFY
    {
        private static Logger logger = NLog.LogManager.GetLogger("SECSTrxLogger");
        private static Logger SXFYConvertLogger = NLog.LogManager.GetLogger("SXFYConvertLogger");

        /// <summary>
        /// SECS Agent Name
        /// </summary>
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public string SECSAgentName;
        /// <summary>
        /// Stream Function
        /// </summary>
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public string StreamFunction;
        /// <summary>
        /// Stream Function Name
        /// </summary>
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public string StreamFunctionName;
        /// <summary>
        /// Wait Bit
        /// </summary>
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public int W_Bit;
        /// <summary>
        /// System Byte
        /// </summary>
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public int SystemByte;
        /// <summary>
        /// Can Abort By SXF0
        /// </summary>
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public Boolean CanAbort = true;                             //A0.03
        /// <summary>
        /// Is Base Type (Don't check field count)
        /// </summary>
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public Boolean IsBaseType = false;
        /// <summary>
        /// 驗証格式(預設是需要驗証)
        /// </summary>
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public Boolean ValidateFormat = true;
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        public SXFY SystemErrorStreamFunction;

        public int Parser2RawData_MaxThread = 1;   //A0.05
        public int Parser2RawData_SleepCount = 50;   //A0.05
        public int Parser2RawData_SleepTime = 100;  //A0.05

        public int getS()
        {
            int S = 0;
            try
            {
                string[] streams = StreamFunction.Split(new String[] { "S", "F" }, System.StringSplitOptions.RemoveEmptyEntries);
                S = Convert.ToInt32(streams[0]);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return S;
        }

        public int getF()
        {
            int F = 0;
            try
            {
                string[] streams = StreamFunction.Split(new String[] { "S", "F" }, System.StringSplitOptions.RemoveEmptyEntries);
                F = Convert.ToInt32(streams[1]);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return F;
        }

        /// <summary>
        /// A0.03
        /// 取得對應用於Abort的Stream Function。
        /// 如果此SXFY不允許Abort，將會回傳空白字串。
        /// </summary>
        /// <returns></returns>
        public string getAbortFunctionName()
        {
            string rtnStr = string.Empty;
            if (CanAbort)
            {
                int abortS = getS();
                int abortF = 0;
                rtnStr = string.Format("S{0}F{1}", Convert.ToString(abortS), Convert.ToString(abortF));
            }
            return rtnStr;
        }

        //public static readonly String STREAM_FUNCTION_KEY = "sf_id";

        private static readonly String CHAR_LESS_THAN = "<";
        private static readonly String CHAR_LARGER_THAN = ">";
        private static readonly String CHAR_LEFT_BRACKETS = "[";
        private static readonly String CHAR_RIGHT_BRACKETS = "]";
        private static readonly String CHAR_DOUBLE_QUOTATION_MARKS = new string(new char[] { '"' });//"\"";
        private static readonly String CHAR_BREAK = " ";
        private static readonly String CHAR_DOT = ".";

        private static readonly String CHAR_EQUAL = "=";

        public object getField(string fieldName)
        {
            FieldInfo field = this.GetType().GetField(fieldName);
            return field.GetValue(this);
        }

        public void parser2RawData(ref Object rawData, QSACTIVEXLib.QSWrapper qsWrapper)
        {
            DataItemStruct diStruct = new DataItemStruct()
            {
                RawData = rawData,
                QsWrapper = qsWrapper
            };
            ThreadPool.QueueUserWorkItem(doParser2RawData, diStruct);
            diStruct.eventWaitHandle.WaitOne();
            rawData = diStruct.RawData;
        }

        private void doParser2RawData(Object diStructObj)
        {
            DataItemStruct diStruct = diStructObj as DataItemStruct;
            Object rawData = diStruct.RawData;
            QSACTIVEXLib.QSWrapper qsWrapper = diStruct.QsWrapper;
            //            ParseSizeCtr parseSizeCtr = new ParseSizeCtr();
            DateTime startTime = DateTime.Now;
            doParser2RawData(ref rawData, qsWrapper/*, parseSizeCtr*/);
            double costTime = DateTime.Now.Subtract(startTime).TotalMilliseconds;
            //SXFYConvertLogger.Debug("Do Parser SXFT[{0}](SystemByte:{1})(SECSAgentName:{2}) To Raw Data cost Milliseconds:[{3}]",
            //    StreamFunction, SystemByte, SECSAgentName, costTime);
            SXFYConvertLogger.Info("Do Parser SXFT[{0}](SystemByte:{1})(SECSAgentName:{2}) To Raw Data cost Milliseconds:[{3}]",   //A0.05
                StreamFunction, SystemByte, SECSAgentName, costTime);                                                              //A0.05
            diStruct.RawData = rawData;
            diStruct.QsWrapper = qsWrapper;
            diStruct.eventWaitHandle.Set();
        }
        //[SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        //public readonly static int Parse_Size_Limit = 300;
        //[SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        //private class ParseSizeCtr 
        //{
        //    private Object _lock_ParseSize = new object();
        //    public int parse_size { get; private set; }
        //    public int resetParseSize()
        //    {
        //        lock (_lock_ParseSize)
        //        {
        //            parse_size = 0;
        //            return parse_size;
        //        }
        //    }
        //    public int increaseParseSize()
        //    {
        //        lock (_lock_ParseSize)
        //        {
        //            ++parse_size;
        //            if (parse_size > SXFY.Parse_Size_Limit) 
        //            {

        //            }
        //            return parse_size;
        //        }
        //    }
        //}


        private void doParser2RawData(ref Object rawData, QSACTIVEXLib.QSWrapper qsWrapper/*, ParseSizeCtr parseSizeCtr*/)
        {
            SXFYConvertLogger.Debug("Start parser2RawData[SystemByte:{0}]", SystemByte);
            FieldInfo[] fields = null;
            try
            {
                fields = SEUtility.GetSeceElementFields(this.GetType());
                SXFYConvertLogger.Debug("Get Fields, Count[{0}][SystemByte:{1}]", fields.Length, SystemByte);
            }
            catch (Exception ex)
            {
                fields = new FieldInfo[0];
                logger.WarnException("parser2RawData GetSeceElementFields Exception!", ex);
                SXFYConvertLogger.WarnException("parser2RawData GetSeceElementFields Exception![SystemByte:" + SystemByte + "]", ex);
            }
            try
            {
                int fCnt = fields.Length;
                if (fCnt == 0)
                {
                    Object nullItem = null;
                    SXFYConvertLogger.Debug("Begin dataItemOut, fCnt=0 , [SystemByte:{0}]", SystemByte);
                    qsWrapper.DataItemOut(ref rawData, 0, QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE, ref nullItem);
                    //                    dataItemOut(ref rawData, 0, QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE, ref nullItem, ref qsWrapper);
                    SXFYConvertLogger.Debug("End dataItemOut, fCnt=0 , [SystemByte:{0}]", SystemByte);
                    return;
                }
                Boolean isListSpreadOut = SEUtility.isListSpreadOutSXFY(this.GetType());        //A0.02

                Object dataItem = null;
                if (!isListSpreadOut)       //A0.02
                {
                    SXFYConvertLogger.Debug("Begin dataItemOut, isListSpreadOut=false , [SystemByte:{0}]", SystemByte);
                    qsWrapper.DataItemOut(ref rawData, fCnt, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem);
                    //                    dataItemOut(ref rawData, fCnt, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem, ref qsWrapper);
                    SXFYConvertLogger.Debug("End dataItemOut, isListSpreadOut=false , [SystemByte:{0}]", SystemByte);
                }
                //
                string data = null;
                SXFY subArray = null;
                logger.Debug("fields.Length={0}", fields.Length);
                for (int i = 0; i < fields.Length; ++i)
                {
                    Object obj = fields[i].GetValue(this);
                    if (fields[i].FieldType.IsInterface) { continue; }
                    string fieldName = fields[i].Name;
                    if (fields[i].FieldType.IsArray)
                    {
                        //                        qsWrapper.DataItemOut(ref rawData, (obj as Array).Length, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, new Object());
                        if (obj == null)
                        {
                            Type eleType = fields[i].FieldType;
                            Object[] args = new object[1];
                            args[0] = 0;
                            SXFYConvertLogger.Debug("Begin CreateInstance, Object is null[SystemByte:{0}]", SystemByte);
                            obj = Activator.CreateInstance(eleType, args);
                            SXFYConvertLogger.Debug("End CreateInstance, Object is null[SystemByte:{0}]", SystemByte);
                        }
                        if (obj.GetType().GetElementType() == typeof(String))
                        {
                            //A0.01 Begin
                            SecsElement attr = SEUtility.getSecsElementAttr(fields[i]);
                            dataItem = new object();
                            SXFYConvertLogger.Debug("Begin dataItemOut, obj element type is string[SystemByte:{0}]", SystemByte);
                            qsWrapper.DataItemOut(ref rawData, (obj as Array).Length, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem);
                            //                            dataItemOut(ref rawData, (obj as Array).Length, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem, ref qsWrapper);
                            SXFYConvertLogger.Debug("End dataItemOut, obj element type is string[SystemByte:{0}]", SystemByte);
                            for (int j = 0; j < (obj as Array).Length; ++j)
                            {
                                data = (obj as Array).GetValue(j) as string;
                                if (data == null/* || data.Length == 0*/)
                                {
                                    logger.Debug("Data is Empty");
                                    data = string.Empty;
                                    //                            continue; 
                                }
                                int elementLength = attr.ListElementLength;
                                Object tmpObj = data.PadRight(elementLength, ' ');
                                QSACTIVEXLib.SECSII_DATA_TYPE dataTyp = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
                                dataTyp = SecsElement.convertToSECSIIDataType(attr.ListElementType);
                                int length = attr.ListElementLength;
                                SXFYConvertLogger.Debug("Begin dataItemOut, obj element type is string, do element[SystemByte:{0}]", SystemByte);
                                //                                parseSizeCtr.increaseParseSize();
                                qsWrapper.DataItemOut(ref rawData, length, dataTyp, ref tmpObj);
                                //                                dataItemOut(ref rawData, length, dataTyp, ref tmpObj, ref qsWrapper);
                                SXFYConvertLogger.Debug("End dataItemOut, obj element type is string, do element[SystemByte:{0}]", SystemByte);
                            }
                            //A0.01 End
                        }
                        else
                        {
                            dataItem = new object();
                            SXFYConvertLogger.Debug("Begin dataItemOut, obj type is list[SystemByte:{0}]", SystemByte);
                            qsWrapper.DataItemOut(ref rawData, (obj as Array).Length, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem);
                            //                            dataItemOut(ref rawData, (obj as Array).Length, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem, ref qsWrapper);
                            SXFYConvertLogger.Debug("End dataItemOut, obj type is list[SystemByte:{0}]", SystemByte);

                            int length = (obj as Array).Length;
                            if (length > 10)
                            {
                                DateTime startTime = DateTime.Now;
                                SXFYConvertLogger.Debug("Begin Parallel.For parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
                                SXFYConvertLogger.Info("Parser2RawData_MaxThread:[{0}],Parser2RawData_SleepCount:[{1}],Parser2RawData_SleepTime[{2}]"
                                    , Parser2RawData_MaxThread, Parser2RawData_SleepCount, Parser2RawData_SleepTime);
                                List<KeyValuePair<int, byte[]>> rawDataItemList = new List<KeyValuePair<int, byte[]>>();
                                //A0.05 Parallel.For(0, (obj as Array).Length, j =>
                                Parallel.For(0, (obj as Array).Length, new ParallelOptions { MaxDegreeOfParallelism = Parser2RawData_MaxThread }, j =>  //A0.05 
                                {
                                    DateTime startParallelTime = DateTime.Now;
                                    SXFYConvertLogger.Debug("Begin Parallel.For[j:{0}][SystemByte:{1}]", j, SystemByte);
                                    SXFY tmpSubArray = (SXFY)(obj as Array).GetValue(j);
                                    Object tmpRawData = null;
                                    if (tmpSubArray != null)
                                    {
                                        SXFYConvertLogger.Debug("Begin parser2RawData for subArray, subArray[fieldName:{0}][j:{1}] not null[SystemByte:{2}]",
                                            fieldName, j, SystemByte);
                                        tmpSubArray.doParser2RawData(ref tmpRawData, qsWrapper);
                                        SXFYConvertLogger.Debug("End parser2RawData for subArray, subArray[fieldName:{0}][j:{1}] not null[SystemByte:{2}]",
                                            fieldName, j, SystemByte);
                                    }
                                    else
                                    {
                                        Type eleType = obj.GetType().GetElementType();
                                        tmpSubArray = (SXFY)Activator.CreateInstance(eleType);
                                        SXFYConvertLogger.Debug("Begin parser2RawData for subArray, subArray is null[SystemByte:{0}]", SystemByte);
                                        tmpSubArray.doParser2RawData(ref tmpRawData, qsWrapper);
                                        SXFYConvertLogger.Debug("End parser2RawData for subArray, subArray is null[SystemByte:{0}]", SystemByte);
                                    }
                                    if (tmpRawData != null)
                                    {
                                        byte[] tmpRawDataAry = (tmpRawData as byte[]);
                                        lock (rawDataItemList)
                                        {
                                            rawDataItemList.Add(new KeyValuePair<int, byte[]>(j, tmpRawDataAry));
                                        }
                                    }
                                    double totalMS = DateTime.Now.Subtract(startParallelTime).TotalMilliseconds;
                                    SXFYConvertLogger.Debug("End Parallel.For[j:{0}][SystemByte:{1}][totalMilliseconds:{2}]", j, SystemByte, totalMS);
                                    //每5次就delay 10 ms，讓CPU得以空檔，使Parallel.For可再動態增加Thread提昇總處理速度
                                    //A0.05 if (j % 5 == 0) { Thread.Sleep(100); }   //A0.04 
                                    if (j != 0 && j % Parser2RawData_SleepCount == 0)
                                    {
                                        double costTime = DateTime.Now.Subtract(startTime).TotalMilliseconds;
                                        SXFYConvertLogger.Info("Do Parser Proc Data To Raw Data cost Milliseconds:[{0}]/[{1}] items",
                                             costTime, Parser2RawData_SleepCount);
                                        //A0.06 Thread.Sleep(Parser2RawData_SleepTime);
                                        SpinWait.SpinUntil(() => false, Parser2RawData_SleepTime); //A0.06
                                        startTime = DateTime.Now;
                                    } //A0.05 
                                });
                                SXFYConvertLogger.Debug("End Parallel.For parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
                                SXFYConvertLogger.Debug("Begin Sort KeyValuePair for subArray, subArray not null[SystemByte:{0}]", SystemByte);
                                rawDataItemList = rawDataItemList.OrderBy(o => o.Key).ToList();
                                byte[] rawDataAry = (rawData as byte[]);
                                List<byte> tmpList = (rawDataAry == null ? new List<byte>() : rawDataAry.ToList());

                                foreach (KeyValuePair<int, byte[]> pair in rawDataItemList)
                                {
                                    tmpList.AddRange(pair.Value.ToList());
                                }
                                rawData = tmpList.ToArray();
                                SXFYConvertLogger.Debug("End Sort KeyValuePair for subArray, subArray not null[SystemByte:{0}]", SystemByte);
                            }
                            else
                            {
                                for (int j = 0; j < (obj as Array).Length; ++j)
                                {
                                    subArray = (SXFY)(obj as Array).GetValue(j);
                                    if (subArray != null)
                                    {
                                        SXFYConvertLogger.Debug("Begin parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
                                        subArray.doParser2RawData(ref rawData, qsWrapper);
                                        SXFYConvertLogger.Debug("End parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
                                    }
                                    else
                                    {
                                        Type eleType = obj.GetType().GetElementType();
                                        subArray = (SXFY)Activator.CreateInstance(eleType);
                                        SXFYConvertLogger.Debug("Begin parser2RawData for subArray, subArray is null[SystemByte:{0}]", SystemByte);
                                        subArray.doParser2RawData(ref rawData, qsWrapper);
                                        SXFYConvertLogger.Debug("End parser2RawData for subArray, subArray is null[SystemByte:{0}]", SystemByte);
                                    }
                                }
                            }
                        }
                    }
                    else if (fields[i].FieldType == typeof(String))
                    {
                        data = obj as String;
                        if (data == null/* || data.Length == 0*/)
                        {
                            logger.Debug("Data is Empty");
                            data = string.Empty;
                            //                            continue; 
                        }
                        SecsElement attr = SEUtility.getSecsElementAttr(fields[i]);
                        if (attr == null) { continue; }
                        if (attr.Type == SecsElement.SecsElementType.NOT_USED) { continue; }
                        logger.Debug("Type:{0}, Length:{1}",
                                    SecsElement.converToTypeID(attr.Type), attr.Length);
                        string type = SecsElement.converToTypeID(attr.Type);
                        int length = attr.Length;
                        QSACTIVEXLib.SECSII_DATA_TYPE dataTyp = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
                        dataTyp = SecsElement.convertToSECSIIDataType(attr.Type);
                        if (dataTyp == QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE)
                        {
                            string[] bSAry = data.Split(new string[] { " 0x" }, length, StringSplitOptions.RemoveEmptyEntries);
                            int[] bAry = new int[length];
                            Object bAryObj = null;
                            for (int ix = 0; ix < bSAry.Length; ++ix)
                            {
                                int bVal = 0;
                                try { bVal = Convert.ToInt32(bSAry[ix], 16); }
                                catch { }
                                bAry[ix] = bVal;
                            }
                            bAryObj = bAry;
                            SXFYConvertLogger.Debug("Begin dataItemOut, this field is binary[SystemByte:{0}]", SystemByte);
                            qsWrapper.DataItemOut(ref rawData, length, dataTyp, ref bAryObj);
                            SXFYConvertLogger.Debug("End dataItemOut, this field is binary[SystemByte:{0}]", SystemByte);
                        }
                        else
                        {
                            data = data.PadRight(length, ' ');
                            obj = (Object)data;
                            //                        if (attr.Type == SecsElement.SecsElementType.TYPE_ASCII)
                            //                        {
                            //                            dataTyp = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
                            //                        }
                            //                            qsWrapper.DataItemOut(ref rawData, length, dataTyp, ref obj);
                            SXFYConvertLogger.Debug("Begin dataItemOut, this field is string[SystemByte:{0}]", SystemByte);
                            qsWrapper.DataItemOut(ref rawData, length, dataTyp, ref obj);
                            //                        dataItemOut(ref rawData, length, dataTyp, ref obj, ref qsWrapper);
                            SXFYConvertLogger.Debug("End dataItemOut, this field is string[SystemByte:{0}]", SystemByte);
                        }
                    }
                    else if (fields[i].FieldType.BaseType == typeof(SXFY))
                    {
                        subArray = (obj as SXFY);
                        if (subArray != null)
                        {
                            SXFYConvertLogger.Debug("Begin parser2RawData for subArray, this field is SXFY, subArray not null[SystemByte:{0}]", SystemByte);
                            subArray.doParser2RawData(ref rawData, qsWrapper);
                            SXFYConvertLogger.Debug("End parser2RawData for subArray, this field is SXFY, subArray not null[SystemByte:{0}]", SystemByte);
                        }
                        else
                        {
                            Type eleType = obj.GetType();
                            subArray = (SXFY)Activator.CreateInstance(eleType);
                            SXFYConvertLogger.Debug("Begin parser2RawData for subArray, this field is SXFY, subArray is null[SystemByte:{0}]", SystemByte);
                            subArray.doParser2RawData(ref rawData, qsWrapper);
                            SXFYConvertLogger.Debug("End parser2RawData for subArray, this field is SXFY, subArray is null[SystemByte:{0}]", SystemByte);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SXFYConvertLogger.Debug("Conver Exception![SystemByte:" + SystemByte + "]", ex);
                logger.WarnException("parser2RawData Exception!", ex);
            }
            SXFYConvertLogger.Debug("Finish parser2RawData[SystemByte:{0}]", SystemByte);
        }

        //        public void parser2RawData(ref Object rawData, QSACTIVEXLib.QSWrapper qsWrapper)
        //        {
        //            SXFYConvertLogger.Debug("Start parser2RawData[SystemByte:{0}]", SystemByte);
        //            FieldInfo[] fields = null;
        //            try
        //            {
        //                //                fields = this.GetType().GetFields(
        //                //                     BindingFlags.Instance | BindingFlags.Public);
        //                fields = SEUtility.GetSeceElementFields(this.GetType());
        //                SXFYConvertLogger.Debug("Get Fields, Count[{0}][SystemByte:{1}]", fields.Length, SystemByte);
        //            }
        //            catch(Exception ex)
        //            {
        //                fields = new FieldInfo[0];
        //                logger.WarnException("parser2RawData GetSeceElementFields Exception!", ex);
        //                SXFYConvertLogger.WarnException("parser2RawData GetSeceElementFields Exception![SystemByte:"+ SystemByte +"]", ex);
        //            }
        //            try
        //            {
        //                int fCnt = fields.Length;
        //                if (fCnt == 0) 
        //                {
        //                    Object nullItem = null;
        //                    SXFYConvertLogger.Debug("Begin dataItemOut, fCnt=0 , [SystemByte:{0}]", SystemByte);
        //                    dataItemOut(ref rawData, 0, QSACTIVEXLib.SECSII_DATA_TYPE.UNKNOWN_TYPE, ref nullItem, ref qsWrapper);
        //                    SXFYConvertLogger.Debug("End dataItemOut, fCnt=0 , [SystemByte:{0}]", SystemByte);
        //                    return;
        //                }
        //                Boolean isListSpreadOut = SEUtility.isListSpreadOutSXFY(this.GetType());        //A0.02
        //                //
        //                #region 計算數量
        //                //int fCnt = 0;
        ////                int fCnt = fields.Length;
        //                //for (int i = 0; i < fields.Length; ++i)
        //                //{
        //                //    if (fields[i].FieldType.IsInterface) { continue; }
        //                //    if (fields[i].FieldType.IsArray && fields[i].FieldType.GetElementType().BaseType == typeof(SXFY)) 
        //                //    {
        //                //        ++fCnt;
        //                //    }else
        //                //    if (fields[i].FieldType.BaseType == typeof(SXFY))
        //                //    {
        //                //        ++fCnt;
        //                //    }
        //                //    else
        //                //    {
        //                //        System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fields[i]);
        //                //        SecsElement attr = null;
        //                //        foreach (System.Attribute a in attrs)
        //                //        {
        //                //            if (a is SecsElement)
        //                //            {
        //                //                attr = (SecsElement)a;
        //                //                break;
        //                //            }
        //                //        }
        //                //        if (attr == null) { continue; }
        //                //        if (attr.Type == SecsElement.SecsElementType.NOT_USED) { continue; }
        //                //        ++fCnt;
        //                //    }
        //                //}
        //                #endregion
        //                //                qsWrapper.DataItemOut(ref rawData, fCnt, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, null);
        //                Object dataItem = null;
        //                if (!isListSpreadOut)       //A0.02
        //                {
        //                    SXFYConvertLogger.Debug("Begin dataItemOut, isListSpreadOut=false , [SystemByte:{0}]", SystemByte);
        //                    dataItemOut(ref rawData, fCnt, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem, ref qsWrapper);
        //                    SXFYConvertLogger.Debug("End dataItemOut, isListSpreadOut=false , [SystemByte:{0}]", SystemByte);
        //                }
        //                //
        //                string data = null;
        //                SXFY subArray = null;
        //                logger.Debug("fields.Length={0}", fields.Length);
        //                for (int i = 0; i < fields.Length; ++i)
        //                {
        //                    Object obj = fields[i].GetValue(this);
        //                    if (fields[i].FieldType.IsInterface) { continue; }

        //                    if (fields[i].FieldType.IsArray)
        //                    {
        //                        //                        qsWrapper.DataItemOut(ref rawData, (obj as Array).Length, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, new Object());
        //                        if (obj == null) 
        //                        {
        //                            Type eleType = fields[i].FieldType;
        //                            Object[] args = new object[1];
        //                            args[0] = 0;
        //                            SXFYConvertLogger.Debug("Begin CreateInstance, Object is null[SystemByte:{0}]", SystemByte);
        //                            obj = Activator.CreateInstance(eleType, args);
        //                            SXFYConvertLogger.Debug("End CreateInstance, Object is null[SystemByte:{0}]", SystemByte);
        //                        }
        //                        if (obj.GetType().GetElementType() == typeof(String))
        //                        {
        //                            //A0.01 Begin
        //                            SecsElement attr = SEUtility.getSecsElementAttr(fields[i]);
        //                            dataItem = new object();
        //                            SXFYConvertLogger.Debug("Begin dataItemOut, obj element type is string[SystemByte:{0}]", SystemByte);
        //                            dataItemOut(ref rawData, (obj as Array).Length, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem, ref qsWrapper);
        //                            SXFYConvertLogger.Debug("End dataItemOut, obj element type is string[SystemByte:{0}]", SystemByte);
        //                            for (int j = 0; j < (obj as Array).Length; ++j) 
        //                            {
        //                                data = (obj as Array).GetValue(j) as string;
        //                                if (data == null/* || data.Length == 0*/)
        //                                {
        //                                    logger.Debug("Data is Empty");
        //                                    data = string.Empty;
        //                                    //                            continue; 
        //                                }
        //                                int elementLength = attr.ListElementLength;
        //                                Object tmpObj = data.PadRight(elementLength, ' ');
        //                                QSACTIVEXLib.SECSII_DATA_TYPE dataTyp = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
        //                                dataTyp = SecsElement.convertToSECSIIDataType(attr.ListElementType);
        //                                int length = attr.ListElementLength;
        //                                SXFYConvertLogger.Debug("Begin dataItemOut, obj element type is string, do element[SystemByte:{0}]", SystemByte);
        //                                dataItemOut(ref rawData, length, dataTyp, ref tmpObj, ref qsWrapper);
        //                                SXFYConvertLogger.Debug("End dataItemOut, obj element type is string, do element[SystemByte:{0}]", SystemByte);
        //                            }
        //                            //A0.01 End
        //                        }
        //                        else 
        //                        {
        //                            dataItem = new object();
        //                            SXFYConvertLogger.Debug("Begin dataItemOut, obj type is list[SystemByte:{0}]", SystemByte);
        //                            dataItemOut(ref rawData, (obj as Array).Length, QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE, ref dataItem, ref qsWrapper);
        //                            SXFYConvertLogger.Debug("End dataItemOut, obj type is list[SystemByte:{0}]", SystemByte);

        //                            int length = (obj as Array).Length;
        //                            if(length > 10){
        //                                SXFYConvertLogger.Debug("Begin Parallel.For parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
        //                                List<KeyValuePair<int, byte[]>> rawDataItemList = new List<KeyValuePair<int, byte[]>>();
        //                                Parallel.For(0, (obj as Array).Length, j => 
        //                                {
        //                                    SXFYConvertLogger.Debug("Parallel.For[j:{0}][SystemByte:{1}]", j, SystemByte);
        //                                    SXFY tmpSubArray = (SXFY)(obj as Array).GetValue(j);
        //                                    Object tmpRawData = null;
        //                                    if (tmpSubArray != null)
        //                                    {
        //                                        SXFYConvertLogger.Debug("Begin parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
        //                                        tmpSubArray.parser2RawData(ref tmpRawData, qsWrapper);
        //                                        SXFYConvertLogger.Debug("End parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
        //                                    }
        //                                    else
        //                                    {
        //                                        Type eleType = obj.GetType().GetElementType();
        //                                        tmpSubArray = (SXFY)Activator.CreateInstance(eleType);
        //                                        SXFYConvertLogger.Debug("Begin parser2RawData for subArray, subArray is null[SystemByte:{0}]", SystemByte);
        //                                        tmpSubArray.parser2RawData(ref tmpRawData, qsWrapper);
        //                                        SXFYConvertLogger.Debug("End parser2RawData for subArray, subArray is null[SystemByte:{0}]", SystemByte);
        //                                    }
        //                                    if (tmpRawData != null)
        //                                    {
        //                                        byte[] tmpRawDataAry = (tmpRawData as byte[]);
        //                                        lock (rawDataItemList)
        //                                        {
        //                                            rawDataItemList.Add(new KeyValuePair<int, byte[]>(j, tmpRawDataAry));
        //                                        }
        //                                    }
        //                                });
        //                                SXFYConvertLogger.Debug("End Parallel.For parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
        //                                SXFYConvertLogger.Debug("Begin Sort KeyValuePair for subArray, subArray not null[SystemByte:{0}]", SystemByte);
        //                                rawDataItemList = rawDataItemList.OrderBy(o => o.Key).ToList();
        //                                byte[] rawDataAry = (rawData as byte[]);
        //                                List<byte> tmpList = (rawDataAry == null ? new List<byte>() : rawDataAry.ToList());

        //                                foreach (KeyValuePair<int, byte[]> pair in rawDataItemList)
        //                                {
        //                                    tmpList.AddRange(pair.Value.ToList());
        //                                }
        //                                rawData = tmpList.ToArray();
        //                                SXFYConvertLogger.Debug("End Sort KeyValuePair for subArray, subArray not null[SystemByte:{0}]", SystemByte);
        //                            }else{
        //                                for (int j = 0; j < (obj as Array).Length; ++j)
        //                                {
        //                                    subArray = (SXFY)(obj as Array).GetValue(j);
        //                                    if (subArray != null)
        //                                    {
        //                                        SXFYConvertLogger.Debug("Begin parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
        //                                        subArray.parser2RawData(ref rawData, qsWrapper);
        //                                        SXFYConvertLogger.Debug("End parser2RawData for subArray, subArray not null[SystemByte:{0}]", SystemByte);
        //                                    }
        //                                    else 
        //                                    {
        //                                        Type eleType = obj.GetType().GetElementType();
        //                                        subArray = (SXFY)Activator.CreateInstance(eleType);
        //                                        SXFYConvertLogger.Debug("Begin parser2RawData for subArray, subArray is null[SystemByte:{0}]", SystemByte);
        //                                        subArray.parser2RawData(ref rawData, qsWrapper);
        //                                        SXFYConvertLogger.Debug("End parser2RawData for subArray, subArray is null[SystemByte:{0}]", SystemByte);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                    else if (fields[i].FieldType == typeof(String))
        //                    {
        //                        data = obj as String;
        //                        if (data == null/* || data.Length == 0*/) 
        //                        { 
        //                            logger.Debug("Data is Empty");
        //                            data = string.Empty;
        ////                            continue; 
        //                        }
        //                        //System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fields[i]);
        //                        //SecsElement attr = null;
        //                        //foreach (System.Attribute a in attrs)
        //                        //{
        //                        //    if (a is SecsElement)
        //                        //    {
        //                        //        attr = (SecsElement)a;
        //                        //        break;
        //                        //    }
        //                        //}
        //                        SecsElement attr = SEUtility.getSecsElementAttr(fields[i]);
        //                        if (attr == null) { continue; }
        //                        if (attr.Type == SecsElement.SecsElementType.NOT_USED) { continue; }
        //                        logger.Debug("Type:{0}, Length:{1}",
        //                                    SecsElement.converToTypeID(attr.Type), attr.Length);
        //                        string type = SecsElement.converToTypeID(attr.Type);
        //                        int length = attr.Length;
        //                        data = data.PadRight(length, ' ');
        //                        obj = (Object)data;
        //                        QSACTIVEXLib.SECSII_DATA_TYPE dataTyp = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
        //                        dataTyp = SecsElement.convertToSECSIIDataType(attr.Type);
        ////                        if (attr.Type == SecsElement.SecsElementType.TYPE_ASCII)
        ////                        {
        ////                            dataTyp = QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
        ////                        }
        //                        //                            qsWrapper.DataItemOut(ref rawData, length, dataTyp, ref obj);
        //                        SXFYConvertLogger.Debug("Begin dataItemOut, this field is string[SystemByte:{0}]", SystemByte);
        //                        dataItemOut(ref rawData, length, dataTyp, ref obj, ref qsWrapper);
        //                        SXFYConvertLogger.Debug("End dataItemOut, this field is string[SystemByte:{0}]", SystemByte);
        //                    }
        //                    else if (fields[i].FieldType.BaseType == typeof(SXFY))
        //                    {
        //                        subArray = (obj as SXFY);
        //                        if (subArray != null)
        //                        {
        //                            SXFYConvertLogger.Debug("Begin parser2RawData for subArray, this field is SXFY, subArray not null[SystemByte:{0}]", SystemByte);
        //                            subArray.parser2RawData(ref rawData, qsWrapper);
        //                            SXFYConvertLogger.Debug("End parser2RawData for subArray, this field is SXFY, subArray not null[SystemByte:{0}]", SystemByte);
        //                        }
        //                        else 
        //                        {
        //                            Type eleType = obj.GetType();
        //                            subArray = (SXFY)Activator.CreateInstance(eleType);
        //                            SXFYConvertLogger.Debug("Begin parser2RawData for subArray, this field is SXFY, subArray is null[SystemByte:{0}]", SystemByte);
        //                            subArray.parser2RawData(ref rawData, qsWrapper);
        //                            SXFYConvertLogger.Debug("End parser2RawData for subArray, this field is SXFY, subArray is null[SystemByte:{0}]", SystemByte);
        //                        }
        //                    }
        //                }
        //            }
        //            catch(Exception ex)
        //            {
        //                SXFYConvertLogger.Debug("Conver Exception![SystemByte:"+ SystemByte +"]", ex);
        //                logger.WarnException("parser2RawData Exception!", ex);
        //            }
        //            SXFYConvertLogger.Debug("Finish parser2RawData[SystemByte:{0}]", SystemByte);
        //        }

        public string toSECSString()
        {
            string tabSpace = "";
            StringWriter sw = new StringWriter();
            //First
            //System.Attribute[] attrs = System.Attribute.GetCustomAttributes(property);
            //        sw.Write(CHAR_LESS_THAN);                   //<
            //        sw.Write(SecsElementTypeID.TYPE_LIST);      //L
            //        sw.Write(CHAR_LEFT_BRACKETS);               //[
            //        sw.Write((obj as Array).Length);            //長度
            //        sw.Write(CHAR_RIGHT_BRACKETS);              //]
            Boolean isListSpreadOut = SEUtility.isListSpreadOutSXFY(this.GetType());
            toSECSElement(tabSpace, sw);
            if (!isListSpreadOut)
            {
                //hayes test..20140903                sw.Write(CHAR_LARGER_THAN);                 //>
            }
            //sw.Write(CHAR_DOT);
            sw.Flush();
            return sw.ToString();
        }
        [SecsElement(Type = SecsElement.SecsElementType.NOT_USED)]
        static readonly string Space = "  ";
        protected void toSECSElement(string tabSpace, StringWriter sw)
        {
            tabSpace += Space;
            FieldInfo[] fields = null;
            Boolean isListSpreadOut = false;
            try
            {
                isListSpreadOut = SEUtility.isListSpreadOutSXFY(this.GetType());
                //                fields = this.GetType().GetFields(
                //                     BindingFlags.Instance | BindingFlags.Public);
                fields = SEUtility.GetSeceElementFields(this.GetType());
                //properties = this.GetType().GetProperties(
                //     BindingFlags.Instance | BindingFlags.Public);
            }
            catch
            {
                fields = new FieldInfo[0];
                //properties = new PropertyInfo[0];
            }
            try
            {
                #region 計算數量
                //int fCnt = 0;
                int fCnt = fields.Length;
                //for (int i = 0; i < fields.Length; ++i) 
                //{
                //    if (fields[i].FieldType.IsInterface) { continue; }
                //    if (fields[i].FieldType.BaseType == typeof(SXFY))
                //    {
                //        ++fCnt;
                //    }
                //    else 
                //    {
                //        System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fields[i]);
                //        SecsElement attr = null;
                //        foreach (System.Attribute a in attrs)
                //        {
                //            if (a is SecsElement)
                //            {
                //                attr = (SecsElement)a;
                //                break;
                //            }
                //        }
                //        if (attr == null) { continue; }
                //        if (attr.Type == SecsElement.SecsElementType.NOT_USED) { continue; }
                //        ++fCnt;
                //    }
                //}
                #endregion

                if (fCnt == 0)
                {
                    return;
                }

                if (!isListSpreadOut)
                {
                    //Test Begin
                    sw.Write(tabSpace + CHAR_LESS_THAN);                   //<
                    sw.Write(SecsElementTypeID.TYPE_LIST);      //L
                    sw.Write(CHAR_LEFT_BRACKETS);               //[
                    sw.Write(fCnt);            //長度
                    sw.WriteLine(CHAR_RIGHT_BRACKETS);              //]
                    //Test End
                }

                string data = null;
                SXFY subArray = null;

                for (int i = 0; i < fields.Length; ++i)
                {
                    Object obj = fields[i].GetValue(this);
                    if (fields[i].FieldType.IsInterface) { continue; }

                    if (fields[i].FieldType.IsArray)
                    {
                        if (obj.GetType().GetElementType() == typeof(String))
                        {
                            SecsElement secsEle = SEUtility.getSecsElementAttr(fields[i]);
                            if (secsEle.Type == SecsElement.SecsElementType.NOT_USED)
                            {
                                continue;
                            }
                            int length = secsEle.ListElementLength;
                            string type = SecsElement.converToTypeID(secsEle.ListElementType);

                            sw.Write(tabSpace + Space + CHAR_LESS_THAN);                   //<
                            sw.Write(SecsElementTypeID.TYPE_LIST);      //L
                            sw.Write(CHAR_LEFT_BRACKETS);               //[
                            sw.Write((obj as Array).Length);            //長度
                            sw.WriteLine(CHAR_RIGHT_BRACKETS);              //]
                            for (int j = 0; j < (obj as Array).Length; ++j)
                            {
                                data = (obj as Array).GetValue(j) as string;
                                //                                if (data == null || data.Length == 0) continue;
                                if (data == null || data.Length == 0)
                                {
                                    data = string.Empty;
                                }
                                //
                                //int length = attr.Length;
                                sw.Write(tabSpace + Space + Space + CHAR_LESS_THAN);                   //<
                                sw.Write(type);                             //L
                                sw.Write(CHAR_LEFT_BRACKETS);               //[
                                sw.Write(length);                           //長度
                                sw.Write(CHAR_RIGHT_BRACKETS);              //]
                                sw.Write(CHAR_BREAK);
                                sw.Write(CHAR_DOUBLE_QUOTATION_MARKS);      //"
                                sw.Write(data);                             // Value
                                sw.Write(CHAR_DOUBLE_QUOTATION_MARKS);      //"
                                sw.WriteLine(CHAR_LARGER_THAN);                 //>
                                //
                                //subArray = (SXFY)(obj as Array).GetValue(j);
                                //if (subArray != null)
                                //{
                                //    subArray.toSECSElement(sw);
                                //}
                            }
                            sw.WriteLine(tabSpace + CHAR_LARGER_THAN);                 //>
                        }
                        else
                        {
                            sw.Write(tabSpace + Space + CHAR_LESS_THAN);                   //<
                            sw.Write(SecsElementTypeID.TYPE_LIST);      //L
                            sw.Write(CHAR_LEFT_BRACKETS);               //[
                            sw.Write((obj as Array).Length);            //長度
                            sw.WriteLine(CHAR_RIGHT_BRACKETS);              //]
                            for (int j = 0; j < (obj as Array).Length; ++j)
                            {
                                subArray = (SXFY)(obj as Array).GetValue(j);
                                if (subArray != null)
                                {
                                    subArray.toSECSElement(tabSpace + Space, sw);
                                }
                            }
                            sw.WriteLine(tabSpace + Space + CHAR_LARGER_THAN);                 //>
                        }

                        //                        continue;
                    }
                    else

                        if (fields[i].FieldType == typeof(String))
                        {
                            data = obj as String;
                            //                        if (data == null || data.Length == 0) continue;
                            if (data == null || data.Length == 0)
                            {
                                data = string.Empty;
                            }
                            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fields[i]);
                            SecsElement attr = null;
                            foreach (System.Attribute a in attrs)
                            {
                                if (a is SecsElement)
                                {
                                    attr = (SecsElement)a;
                                    break;
                                }
                            }
                            if (attr == null) { continue; }
                            if (attr.Type == SecsElement.SecsElementType.NOT_USED) { continue; }
                            System.Console.WriteLine("Type:{0}, Length:{1}",
                                        SecsElement.converToTypeID(attr.Type), attr.Length);
                            string type = SecsElement.converToTypeID(attr.Type);
                            int length = attr.Length;
                            sw.Write(tabSpace + Space + CHAR_LESS_THAN);                   //<
                            sw.Write(type);                             //L
                            sw.Write(CHAR_LEFT_BRACKETS);               //[
                            sw.Write(length);                           //長度
                            sw.Write(CHAR_RIGHT_BRACKETS);              //]
                            sw.Write(CHAR_BREAK);

                            //sw.Write(fields[i].Name);
                            //sw.Write(CHAR_BREAK);
                            //sw.Write(CHAR_EQUAL);
                            //sw.Write(CHAR_BREAK);
                            
                            sw.Write(CHAR_DOUBLE_QUOTATION_MARKS);      //"
                            sw.Write(data);                             // Value
                            sw.Write(CHAR_DOUBLE_QUOTATION_MARKS);      //"
                            sw.Write(CHAR_LARGER_THAN);                 //>
                            //sw.WriteLine(CHAR_LARGER_THAN);                 //>

                            sw.Write(CHAR_BREAK);
                            sw.Write(CHAR_LEFT_BRACKETS);               //[
                            sw.Write(fields[i].Name);
                            sw.WriteLine(CHAR_RIGHT_BRACKETS);              //]

                            //                        continue;
                        }
                        else

                            if (fields[i].FieldType.BaseType == typeof(SXFY))
                            {
                                //System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fields[i]);
                                //SecsElement attr = null;
                                //foreach (System.Attribute a in attrs)
                                //{
                                //    if (a is SecsElement)
                                //    {
                                //        attr = (SecsElement)a;
                                //    }
                                //}
                                //if (attr == null) { continue; }
                                //System.Console.WriteLine("Type:{0}, Length:{1}",
                                //            SecsElement.converToTypeID(attr.Type), attr.Length);
                                //string type = SecsElement.converToTypeID(attr.Type);
                                //int length = attr.Length;

                                //           sw.Write(CHAR_LESS_THAN);                   //<
                                //           sw.Write(type);                             //L
                                //           sw.Write(CHAR_LEFT_BRACKETS);               //[
                                //           sw.Write(length);                           //長度
                                //           sw.Write(CHAR_RIGHT_BRACKETS);              //]

                                subArray = (obj as SXFY);
                                if (subArray != null)
                                {
                                    subArray.toSECSElement(tabSpace, sw);
                                }
                                //           sw.Write(CHAR_LARGER_THAN);                 //>

                                //continue;
                            }

                }
                //Test Begin
                if (!isListSpreadOut)
                {
                    sw.WriteLine(tabSpace + CHAR_LARGER_THAN);                 //>
                }
                //Test End
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                Console.WriteLine(ex);
            }
        }

        //////////////

        private class DataItemStruct
        {
            public Object RawData { get; set; }
            public int Offset { get; set; }
            public QSACTIVEXLib.SECSII_DATA_TYPE ItemType { get; set; }
            public int ItemNum { get; set; }
            public Object ItemData { get; set; }
            public int SkipItem { get; set; }
            public int Length { get; set; }
            public EventWaitHandle eventWaitHandle = new ManualResetEvent(false);
            public QSACTIVEXLib.QSWrapper QsWrapper { get; set; }
            public com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_PARSE_RESULT parse_result { get; set; }
            //private AutoResetEvent _WaitHandle;
            //public AutoResetEvent WaitHandle
            //{
            //    get { return _WaitHandle; }
            //    set { _WaitHandle = value; }
            //}
        }
        private void doGetDataItemType(Object obj)
        {
            DataItemStruct diStruct = obj as DataItemStruct;
            diStruct.ItemType = diStruct.QsWrapper.GetDataItemType(diStruct.RawData, diStruct.Offset);
            //diStruct.WaitHandle.Set();
            diStruct.eventWaitHandle.Set();
        }
        private QSACTIVEXLib.SECSII_DATA_TYPE getDataItemType(ref Object rawData, int offset, ref QSACTIVEXLib.QSWrapper qsWrapper)
        {
            DataItemStruct dataItemStruct = new DataItemStruct()
            {
                RawData = rawData,
                Offset = offset,
                QsWrapper = qsWrapper
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
            diStruct.Offset = diStruct.QsWrapper.DataItemIn(ref rawData, diStruct.Offset, diStruct.ItemType,
                                       out itemNum, ref itemData);
            diStruct.ItemNum = itemNum;
            diStruct.ItemData = itemData;
            diStruct.eventWaitHandle.Set();
        }
        private int dataItemIn(ref Object rawData, int offset, QSACTIVEXLib.SECSII_DATA_TYPE itemType, out int itemNum, ref Object itemData, ref QSACTIVEXLib.QSWrapper qsWrapper)
        {
            itemNum = 0;
            DataItemStruct dataItemStruct = new DataItemStruct()
            {
                RawData = rawData,
                Offset = offset,
                ItemType = itemType,
                ItemData = itemData,
                QsWrapper = qsWrapper
            };
            ThreadPool.QueueUserWorkItem(doDataItemIn, dataItemStruct);
            dataItemStruct.eventWaitHandle.WaitOne();
            itemNum = dataItemStruct.ItemNum;
            itemData = dataItemStruct.ItemData;
            offset = dataItemStruct.Offset;
            logger.Debug("**********dataItemIn[Thread:{0}][itemType:{1}][offset:{2}][itemData:{3}][RawData:{4}]",
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                itemType, offset, itemData,
                SEUtility.rawData2String(rawData));
            return offset;
        }
        private void doDataItemInSkip(Object obj)
        {
            //offset = agent.getQSWrapper().DataItemInSkip(rawData, offset, 1);
            DataItemStruct diStruct = obj as DataItemStruct;
            Object rawData = diStruct.RawData;
            diStruct.Offset = diStruct.QsWrapper.DataItemInSkip(ref rawData, diStruct.Offset, diStruct.SkipItem);
            diStruct.eventWaitHandle.Set();
        }
        private int dataItemSkip(ref Object rawData, int offset, int skipItem, ref QSACTIVEXLib.QSWrapper qsWrapper)
        {
            DataItemStruct dataItemStruct = new DataItemStruct()
            {
                RawData = rawData,
                Offset = offset,
                SkipItem = skipItem,
                QsWrapper = qsWrapper
            };
            ThreadPool.QueueUserWorkItem(doDataItemInSkip, dataItemStruct);
            dataItemStruct.eventWaitHandle.WaitOne();
            offset = dataItemStruct.Offset;
            return offset;
        }
        private void doDataItemOut(Object obj)
        {
            DataItemStruct diStruct = obj as DataItemStruct;
            Object rawData = diStruct.RawData;
            Object itemData = diStruct.ItemData;
            diStruct.QsWrapper.DataItemOut(ref rawData, diStruct.Length, diStruct.ItemType, ref itemData);
            diStruct.ItemData = itemData;
            diStruct.RawData = rawData;
            diStruct.eventWaitHandle.Set();
        }
        private void dataItemOut(ref Object rawData, int length, QSACTIVEXLib.SECSII_DATA_TYPE itemType, ref Object itemData, ref QSACTIVEXLib.QSWrapper qsWrapper)
        {
            DataItemStruct dataItemStruct = new DataItemStruct()
            {
                RawData = rawData,
                Length = length,
                ItemType = itemType,
                ItemData = itemData,
                QsWrapper = qsWrapper
            };
            SXFYConvertLogger.Debug("**********Begin doDataItemOut dataItemOut[Thread:{0}][itemType:{1}][length:{2}][itemData:{3}][SystemByte:{4}]",
                 System.Threading.Thread.CurrentThread.ManagedThreadId,
                 itemType, length, itemData, SystemByte);
            ThreadPool.QueueUserWorkItem(doDataItemOut, dataItemStruct);
            dataItemStruct.eventWaitHandle.WaitOne();
            rawData = dataItemStruct.RawData;
            itemData = dataItemStruct.ItemData;
            SXFYConvertLogger.Debug("**********End doDataItemOut dataItemOut[Thread:{0}][itemType:{1}][length:{2}][itemData:{3}][SystemByte:{4}]",
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                itemType, length, itemData, SystemByte);
            logger.Debug("**********dataItemOut[Thread:{0}][itemType:{1}][length:{2}][itemData:{3}][RawData:{4}]",
                System.Threading.Thread.CurrentThread.ManagedThreadId,
                itemType, length, itemData,
                SEUtility.rawData2String(rawData));
        }

    }
}
