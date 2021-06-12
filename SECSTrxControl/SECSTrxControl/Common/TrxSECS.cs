//*********************************************************************************
//      TrxSECS.cs
//*********************************************************************************
// File Name: TrxSECS.cs
// Description: Transaction For SECS
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/05/30    Hayes Chen     N/A            N/A     Initial Release
// 2014/08/05    Hayes Chen     N/A            A0.01   於Send Recv的Receive Handler加上強制比較SystemByte一定要相同
// 2014/09/05    Hayes Chen     N/A            A0.02   增加監聽Abort Function
// 2014/09/20    Hayes Chen     N/A            A0.03   增加監聽Syste Error Function
// 2014/12/10    Hayes Chen     N/A            A0.04   增加中斷Conversation Timeout的等待
// 2016/03/09    Hayes Chen     N/A            A0.05   將Thread.Sleep的用法改成SpinWait()
//**********************************************************************************
//**********************************************************************************
// 2018/03/21    Kevin Wei      N/A            B0.01   修正當有註冊replySystemErrorHandler時，
//                                                     但設備回報S9FX以後，程式會當掉的問題。
//                                                     並加入replySystemErrorHandler、replyAbortHandler
//                                                     的Exception Handle。
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common.SECS;
using com.mirle.ibg3k0.stc.Data.SecsData;
using NLog;
using System.Threading;

namespace com.mirle.ibg3k0.stc.Common
{
    /// <summary>
    /// 此類別用於SECS格式的通訊：
    /// 1. 單一SECS Message傳送
    /// 2. 進行SECS的通訊交握
    /// 3. 進行SECS的通訊交握，並且接續進行對話計時
    /// </summary>
    public class TrxSECS
    {
        private static Logger trxLogger = LogManager.GetLogger("SECSTrxLogger");
        private int monitorIntervalMilliseconds = 100;
        private SECSAgent secsAgent = null;
        private Boolean enable_tx_log = true;
        private string TX_LOG_PATH = "";
        Boolean hasRtn = false;
        Boolean hasAbort = false;               //A0.02
        Boolean hasSystemError = false;         //A0.03
        SXFY recvSXFY = null;
        SXFY abortSXFY = null;                  //A0.02
        SXFY systemErrorSXFY = null;            //A0.03

        /// <summary>
        /// TrxSECS建構式
        /// </summary>
        /// <param name="agent"></param>
        public TrxSECS(SECSAgent agent)
        {
            secsAgent = agent;
            TX_LOG_PATH = secsAgent.TX_LOG_PATH;
        }

        /// <summary>
        /// 傳送SECS Message
        /// </summary>
        /// <param name="inSecs"></param>
        /// <returns></returns>
        public ReturnCode replySECS(SXFY inSecs)
        {
            Boolean sendResult = false;
            try
            {
                sendResult = secsAgent.replySECSMsg(inSecs);
                //                writeTXLog(inSecs, TX_LOG_PATH);
                if (!sendResult)
                {
                    return ReturnCode.SendDataFail;
                }
            }
            catch (Exception ex)
            {
                trxLogger.Error(ex, "Exception:");
            }
            return ReturnCode.Normal;
        }

        /// <summary>
        /// 用於進行SECS的交握
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="inSecs"></param>
        /// <param name="outSecs"></param>
        /// <param name="rtnMsg"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ReturnCode sendRecv<TSource>(SXFY inSecs, out TSource outSecs, /*string receiveSF,*/ out SXFY abortSecs,
            out string rtnMsg,
            Expression<Func<TSource, bool>> predicate)
        {
            return sendRecv<TSource>(inSecs, out outSecs/*, receiveSF*/, out abortSecs, out rtnMsg, secsAgent.TimeoutSec,
                secsAgent.RetryCount, predicate);
        }

        /// <summary>
        /// 用於進行SECS的交握，並且於交握完成後，進行接續的對話（等待Host的訊息）。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <param name="inSecs"></param>
        /// <param name="outSecs"></param>
        /// <param name="rtnMsg"></param>
        /// <param name="predicate"></param>
        /// <param name="convSecs"></param>
        /// <param name="convTimeoutSec"></param>
        /// <param name="convPredicate"></param>
        /// <returns></returns>
        public ReturnCode sendRecv<TSource, TSource2>(SXFY inSecs, out TSource outSecs, out SXFY abortSesc,
            out string rtnMsg,
            Expression<Func<TSource, bool>> predicate,
            out TSource2 convSecs, long convTimeoutSec, Expression<Func<TSource2, bool>> convPredicate)
        {
            return sendRecv<TSource, TSource2>(inSecs, out outSecs, out abortSesc, out rtnMsg, secsAgent.TimeoutSec,
                secsAgent.RetryCount, predicate, out convSecs, convTimeoutSec, convPredicate);
        }

        /// <summary>
        /// 用於進行SECS的交握，並且於交握完成後，進行接續的對話（等待Host的訊息）。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <param name="inSecs"></param>
        /// <param name="outSecs"></param>
        /// <param name="rtnMsg"></param>
        /// <param name="predicate"></param>
        /// <param name="convSecs"></param>
        /// <param name="convTimeoutSec"></param>
        /// <param name="convPredicate"></param>
        /// <returns></returns>
        public ReturnCode sendRecv<TSource, TSource2>(SXFY inSecs, out TSource outSecs, out SXFY abortSesc,
            out string rtnMsg,
            Expression<Func<TSource, bool>> predicate,
            out TSource2 convSecs, long convTimeoutSec, Expression<Func<TSource2, bool>> convPredicate, IBreakConvWait breakConvWait)
        {
            this.breakWaitConv = breakConvWait;
            return sendRecv<TSource, TSource2>(inSecs, out outSecs, out abortSesc, out rtnMsg, secsAgent.TimeoutSec,
                secsAgent.RetryCount, predicate, out convSecs, convTimeoutSec, convPredicate);
        }


        /// <summary>
        /// 用於進行SECS的交握
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="inSecs"></param>
        /// <param name="outSecs"></param>
        /// <param name="rtnMsg"></param>
        /// <param name="timeoutSec"></param>
        /// <param name="retryCnt"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ReturnCode sendRecv<TSource>(SXFY inSecs, out TSource outSecs/*, string receiveSF*/, out SXFY abortSesc,
            out string rtnMsg,
            long timeoutSec, int retryCnt, Expression<Func<TSource, bool>> predicate)
        {
            Object convSecs = null;
            return sendRecv<TSource, Object>(inSecs, out outSecs, out abortSesc, out rtnMsg, timeoutSec, retryCnt, predicate,
                false, out convSecs, 0, null);
        }

        // public ReturnCode sendRecv<TSource>(SXFY inSecs, out TSource outSecs/*, string receiveSF*/, out string rtnMsg,
        //     long timeoutSec, int retryCnt, Expression<Func<TSource, bool>> predicate) 
        // {
        //     hasRtn = false;
        //     outSecs = (TSource)SEUtility.GetDefault(typeof(TSource));
        //     rtnMsg = string.Empty;
        //     Boolean sendResult = false;
        //     string receiveSF = (outSecs as SXFY).StreamFunction;
        ////     result = secsAgent.replySECSMsg(inSecs);
        ////     if (!result) 
        ////     {
        ////         return ReturnCode.SendDataFail;
        ////     }
        //     //secsAgent.secsReceive += (_sender, _e) => replyHandler<TSource>(_sender, _e, predicate);
        //     try
        //     {
        //         secsAgent.addSECSReceivedHandler(receiveSF,
        //             (_sender, _e) => replyHandler<TSource>(_sender, _e, predicate));
        //         int count = 0;
        //         Boolean isTryAgain = false;
        //         do
        //         {
        //             isTryAgain = false;
        //             sendResult = secsAgent.replySECSMsg(inSecs);
        //             writeTXLog(inSecs, TX_LOG_PATH);
        //             if (!sendResult)
        //             {
        //                 return ReturnCode.SendDataFail;
        //             }
        //             long currentValueTime = getNow();

        //             while (!hasRtn)
        //             {
        //                 long second = (getNow() - currentValueTime) / 1000;
        //                 if (second >= timeoutSec)
        //                 {
        //                     if (++count >= retryCnt)
        //                     {
        //                         //secsAgent.secsReceive -= (_sender, _e) => replyHandler<TSource>(_sender, _e, predicate);
        //                         //secsAgent.removeSECSReceivedHandler((outSecs as SXFY).StreamFunction,
        //                         //    (_sender, _e) => replyHandler<TSource>(_sender, _e, predicate));
        //                         return ReturnCode.Timeout;
        //                     }
        //                     isTryAgain = true;
        //                     break;
        //                 }
        //                 System.Threading.Thread.Sleep(100);
        //             }
        //         } while (isTryAgain);
        //         outSecs = (TSource)(Object)recvSXFY;
        //         writeTXLog(recvSXFY, TX_LOG_PATH);
        //     }
        //     catch (Exception ex)
        //     {
        //         trxLogger.ErrorException("", ex);
        //     }
        //     finally 
        //     {
        //         secsAgent.removeSECSReceivedHandler(receiveSF,
        //                         (_sender, _e) => replyHandler<TSource>(_sender, _e, predicate));
        //     }
        //     return ReturnCode.Normal;
        // }

        /// <summary>
        /// 用於進行SECS的交握，並且於交握完成後，進行接續的對話（等待Host的訊息）。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <param name="inSecs"></param>
        /// <param name="outSecs"></param>
        /// <param name="rtnMsg"></param>
        /// <param name="timeoutSec"></param>
        /// <param name="retryCnt"></param>
        /// <param name="predicate"></param>
        /// <param name="convSecs"></param>
        /// <param name="convTimeoutSec"></param>
        /// <param name="convPredicate"></param>
        /// <returns></returns>
        public ReturnCode sendRecv<TSource, TSource2>(SXFY inSecs, out TSource outSecs, out SXFY abortSesc,
            out string rtnMsg,
            long timeoutSec, int retryCnt, Expression<Func<TSource, bool>> predicate,
            out TSource2 convSecs, long convTimeoutSec, Expression<Func<TSource2, bool>> convPredicate)
        {
            return sendRecv<TSource, TSource2>(inSecs, out outSecs, out abortSesc, out rtnMsg, timeoutSec, retryCnt, predicate,
                true, out convSecs, convTimeoutSec, convPredicate);
        }

        /// <summary>
        /// 用於進行SECS的交握，並且可以指定是否於交握完成後，還進行接續的對話（等待Host的訊息）。
        /// </summary>
        /// <typeparam name="TSource">指定Receive的SECS型別</typeparam>
        /// <typeparam name="TSource2">指定接續談話Receive的SECS型別</typeparam>
        /// <param name="inSecs">傳送給Host的SCES Message</param>
        /// <param name="outSecs">接收到的SECS Message</param>
        /// <param name="rtnMsg">回傳額外資訊</param>
        /// <param name="timeoutSec">指定Timeout秒數(T3 Timeout)</param>
        /// <param name="retryCnt">指定重試次數</param>
        /// <param name="predicate">指定接收SECS的內容條件檢查</param>
        /// <param name="continueConversation">指定此交握，是否於交握結束後，接續進行對話</param>
        /// <param name="convSecs">從Host接收到的對話SECS Message</param>
        /// <param name="convTimeoutSec">指定接續對話的Timeout秒數</param>
        /// <param name="convPredicate">指定接續對話SECS的內容條件檢查</param>
        /// <returns></returns>
        public ReturnCode sendRecv<TSource, TSource2>(SXFY inSecs, out TSource outSecs, out SXFY abortSecs,
            out string rtnMsg,
            long timeoutSec, int retryCnt, Expression<Func<TSource, bool>> predicate,
            Boolean continueConversation, out TSource2 convSecs, long convTimeoutSec,
            Expression<Func<TSource2, bool>> convPredicate)
        {
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            hasRtn = false;
            hasAbort = false;           //A0.02
            outSecs = (TSource)SEUtility.GetDefault(typeof(TSource));
            convSecs = (TSource2)SEUtility.GetDefault(typeof(TSource2));
            abortSecs = null;

            EventHandler<SECSEventArgs> handler = null;
            EventHandler<SECSEventArgs> convReceiveHandler = null;
            EventHandler<SECSEventArgs> abortHandler = null;            //A0.02
            EventHandler<SECSEventArgs> systemErrorHandler = null;      //A0.03

            rtnMsg = string.Empty;
            Boolean sendResult = false;
            ReturnCode convRtnCode = ReturnCode.Normal;
            string receiveSF = (outSecs as SXFY).StreamFunction;
            string convReceiveSF = string.Empty;
            string abortSF = inSecs.getAbortFunctionName();
            try
            {
                handler = delegate (Object _sender, SECSEventArgs _e)
                {
                    replyHandler<TSource>(_sender, _e, predicate, ref inSecs);
                };
                systemErrorHandler = delegate (Object _sender, SECSEventArgs _e)             //A0.03
                {
                    replySystemErrorHandler(_sender, _e, ref inSecs);                       //A0.03
                };
                trxLogger.Debug("Create Handler[threadID:{0}]", threadID);
                secsAgent.addSECSReceivedHandler(receiveSF, handler);
                secsAgent.addSystemErrorReceivedHandler(systemErrorHandler);                //A0.03
                trxLogger.Debug("Add Handler To List[threadID:{0}]", threadID);
                //A0.02 Begin
                if (inSecs.CanAbort)
                {
                    abortHandler = delegate (Object _sender, SECSEventArgs _e)
                    {
                        replyAbortHandler(_sender, _e, ref inSecs);
                    };
                    trxLogger.Debug("Create Abort Handler[threadID:{0}]", threadID);
                    secsAgent.addSECSReceivedHandler(abortSF, abortHandler);
                }
                //A0.02 End
                //                secsAgent.addSECSReceivedHandler(receiveSF,
                //                    (_sender, _e) => replyHandler<TSource>(_sender, _e, predicate));
                if (continueConversation)
                {
                    convReceiveSF = (convSecs as SXFY).StreamFunction;
                    convReceiveHandler = delegate (Object _sender, SECSEventArgs _e)
                    {
                        conversationHandler<TSource2>(_sender, _e, convPredicate);
                    };
                    secsAgent.addSECSReceivedHandler(convReceiveSF, convReceiveHandler);
                    //                    secsAgent.addSECSReceivedHandler(convReceiveSF,
                    //                            (_sender, _e) => conversationHandler<TSource2>(_sender, _e, convPredicate));
                }
                int count = 0;
                Boolean isTryAgain = false;
                do
                {
                    isTryAgain = false;
                    trxLogger.Debug("Begin to reply SECS[threadID:{0}]", threadID);
                    sendResult = secsAgent.replySECSMsg(inSecs);
                    trxLogger.Debug("End to reply SECS[threadID:{0}]", threadID);
                    //                    writeTXLog(inSecs, TX_LOG_PATH);
                    if (!sendResult)
                    {
                        return ReturnCode.SendDataFail;
                    }
                    long currentValueTime = getNow();

                    while (!hasRtn)
                    {
                        long second = (getNow() - currentValueTime) / 1000;
                        if (second >= timeoutSec)
                        {
                            trxLogger.Debug("Over Time![threadID:{0}]", threadID);
                            if (count++ >= retryCnt)
                            {
                                trxLogger.Debug("Return Timeout![threadID:{0}]", threadID);
                                return ReturnCode.Timeout;
                            }
                            isTryAgain = true;
                            break;
                        }

                        //A0.05 System.Threading.Thread.Sleep(monitorIntervalMilliseconds);
                        SpinWait.SpinUntil(() => false, monitorIntervalMilliseconds);

                        trxLogger.Debug("Loop Watch[second:{0}][threadID:{1}]", second, threadID);
                    }
                    trxLogger.Debug("is retry? [{0}][threadID:{1}]", isTryAgain, threadID);
                } while (isTryAgain);
                trxLogger.Debug("Detect Result 1[threadID:{0}]", threadID);
                if (inSecs.CanAbort && hasAbort)                        //A0.02
                {
                    //Receive Abort Stream Function
                    abortSecs = this.abortSXFY;                         //A0.02
                    //                    writeAbortTXLog(abortSF, TX_LOG_PATH);              //A0.02
                    trxLogger.Debug("Return Abort[threadID:{0}]", threadID);
                    return ReturnCode.Abort;                            //A0.02
                }
                else if (hasSystemError)                                //A0.03
                {
                    inSecs.SystemErrorStreamFunction = systemErrorSXFY; //A0.03
                    trxLogger.Debug("Return System Error[threadID:{0}]", threadID);
                    return ReturnCode.SystemError;                      //A0.03
                }
                else
                {
                    trxLogger.Debug("Receive Reply 1[threadID:{0}]", threadID);
                    outSecs = (TSource)(Object)recvSXFY;
                    //                    writeTXLog(recvSXFY, TX_LOG_PATH);
                    if (continueConversation)
                    {
                        convRtnCode = conversationReceive<TSource2>(out convSecs, convTimeoutSec, convPredicate, false);
                        if (convRtnCode == ReturnCode.ConversationTimeout ||
                            convRtnCode == ReturnCode.BreakcConversationWait  /*A0.04*/)
                        {
                            return convRtnCode;
                        }
                    }
                    trxLogger.Debug("Receive Reply 2[threadID:{0}]", threadID);
                }
                trxLogger.Debug("Detect Result 2[threadID:{0}]", threadID);
            }
            catch (Exception ex)
            {
                trxLogger.ErrorException("sendRecv Exception!", ex);
            }
            finally
            {
                trxLogger.Debug("Remove Handler 1[threadID:{0}]", threadID);
                secsAgent.removeSECSReceivedHandler(receiveSF, handler);
                //                secsAgent.removeSECSReceivedHandler(receiveSF,
                //                                (_sender, _e) => replyHandler<TSource>(_sender, _e, predicate));
                secsAgent.removeSystemErrorReceivedHandler(systemErrorHandler);     //A0.03
                secsAgent.removeSECSReceivedHandler(abortSF, abortHandler);         //A0.02
                if (continueConversation)
                {
                    secsAgent.removeSECSReceivedHandler(convReceiveSF, convReceiveHandler);
                    //                    secsAgent.removeSECSReceivedHandler(convReceiveSF,
                    //                            (_sender, _e) => conversationHandler<TSource2>(_sender, _e, convPredicate));
                }
                trxLogger.Debug("Remove Handler 2[threadID:{0}]", threadID);
            }
            trxLogger.Debug("Return Normal[threadID:{0}]", threadID);
            return ReturnCode.Normal;
        }

        /// <summary>
        /// 取得目前當下系統時間
        /// </summary>
        /// <returns></returns>
        protected long getNow()
        {
            return System.DateTime.Now.Ticks / 10000;
        }

        /// <summary>
        /// A0.02 
        /// 加上Abort Stream Function的監聽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="inSecs"></param>
        protected virtual void replyAbortHandler(object sender, SECSEventArgs e, ref SXFY inSecs)
        {
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            lock (handlerLock)
            {
                try //B0.01
                {
                    if (hasRtn || !inSecs.CanAbort)
                    {
                        return;
                    }
                    if (inSecs.SystemByte != e.SystemBytes)
                    {
                        return;
                    }
                    //
                    SXFY tmpSXFY = e.secsHandler.Parse<SXFY>(e);
                    trxLogger.Debug("Receive Abort Stream Function SXF0");
                    abortSXFY = tmpSXFY;
                    abortSXFY.StreamFunction = inSecs.getAbortFunctionName();
                    //
                    hasAbort = true;
                    hasRtn = true;
                }
                catch (Exception ex)
                {
                    trxLogger.Warn("in reply abort Handler Exception[threadID:{0}][e.SystemBytes:{1}]:{2}", threadID, e.SystemBytes, ex);
                }

            }
        }

        /// <summary>
        /// A0.03
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="inSecs"></param>
        protected virtual void replySystemErrorHandler(object sender, SECSEventArgs e, ref SXFY inSecs)
        {
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            lock (handlerLock)
            {
                try //B0.01
                {
                    if (hasRtn)
                    {
                        return;
                    }
                    if (inSecs.SystemByte != e.SystemBytes)
                    {
                        return;
                    }

                    //B0.01 SXFY tmpSXFY = e.secsHandler.Parse<SXFY>(e);
                    //B0.01 Start 
                    SXFY tmpSXFY = null;
                    if (e.S == SECSAgent.SystemErrorStreamNumber)
                    {
                        switch (e.F)
                        {
                            case 1: tmpSXFY = e.secsHandler.Parse<S9F1>(e); break;
                            case 3: tmpSXFY = e.secsHandler.Parse<S9F3>(e); break;
                            case 5: tmpSXFY = e.secsHandler.Parse<S9F5>(e); break;
                            case 7: tmpSXFY = e.secsHandler.Parse<S9F7>(e); break;
                            case 9: tmpSXFY = e.secsHandler.Parse<S9F9>(e); break;
                            default: tmpSXFY = e.secsHandler.Parse<SXFY>(e); break;
                        }
                    }
                    else
                    {
                        tmpSXFY = e.secsHandler.Parse<SXFY>(e);
                    }
                    //B0.01 Start

                    trxLogger.Debug("Receive System Error Stream Function S9FY");
                    systemErrorSXFY = tmpSXFY;
                    systemErrorSXFY.StreamFunction = string.Format("S{0}F{1}", e.S, e.F);
                    hasSystemError = true;
                    hasRtn = true;
                }
                catch (Exception ex)
                {
                    trxLogger.Warn("in reply system error Handler Exception[threadID:{0}][e.SystemBytes:{1}]:{2}", threadID, e.SystemBytes, ex);
                }
            }
        }

        private object handlerLock = new object();
        protected virtual void replyHandler<TSource>(object sender, SECSEventArgs e,
            Expression<Func<TSource, bool>> predicate, ref SXFY inSecs)
        {
            int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            trxLogger.Debug("in reply Handler[threadID:{0}][SystemByte:{1}]", threadID, e.SystemBytes);
            lock (handlerLock)
            {
                try
                {
                    if (hasRtn)
                    {
                        return;
                    }
                    if (inSecs.SystemByte != e.SystemBytes)     //A0.01
                    {
                        return;
                    }
                    trxLogger.Debug("has return 1[threadID:{0}][e.SystemBytes:{1}]", threadID, e.SystemBytes);
                    TSource tmpSXFY = (TSource)((Object)e.secsHandler.Parse<TSource>(e));

                    TSource sxfy = tmpSXFY;
                    if (predicate != null)
                    {
                        List<TSource> list = new List<TSource>();
                        Type type = typeof(TSource).GetType();
                        list.Add((TSource)tmpSXFY);
                        //                    sxfy = list.AsQueryable().Where(predicate).SingleOrDefault();
                        sxfy = list.AsQueryable().Where(predicate).SingleOrDefault();
                    }
                    trxLogger.Debug("has return 2[threadID:{0}][e.SystemBytes:{1}]", threadID, e.SystemBytes);
                    if (sxfy == null)
                    {
                        return;
                    }
                    recvSXFY = (SXFY)(Object)tmpSXFY;
                    hasAbort = false;       //A0.02
                    hasRtn = true;
                    trxLogger.Debug("has return 3[threadID:{0}][e.SystemBytes:{1}]", threadID, e.SystemBytes);
                }
                catch (Exception ex)
                {
                    trxLogger.Warn("in reply Handler Exception[threadID:{0}][e.SystemBytes:{1}]:{2}", threadID, e.SystemBytes, ex);
                }
            }
        }

        protected static Object _lockObj = new object();
        protected static void writeAbortTXLog(string abortSF, string logPath)
        {
            if (abortSF == null || abortSF.Trim().Length == 0)
            {
                return;
            }
            trxLogger.Info(abortSF);
            lock (_lockObj)
            {
                string postfix = string.Concat("_", DateTime.Now.ToString("yyyyMMddHHmmssfffff"));
                try
                {
                    bool IsExists = System.IO.Directory.Exists(logPath);
                    if (!IsExists)
                    {
                        System.IO.Directory.CreateDirectory(logPath);
                    }
                    using (StreamWriter sw =
                            new StreamWriter(String.Concat(logPath, "\\",
                                abortSF, postfix, ".xml")))
                    {
                        sw.Write(abortSF);
                    }
                }
                catch
                {
                    trxLogger.Warn(String.Concat("Failed to write TX Log: ", abortSF,
                        postfix, ".xml"));
                }
            }
        }
        protected static void writeTXLog(SXFY inTrx, string logPath)
        {
            if (inTrx == null) { return; }
            string trxSecsStr = inTrx.toSECSString();
            trxLogger.Info(trxSecsStr);
            lock (_lockObj)
            {
                string postfix = string.Concat("_", DateTime.Now.ToString("yyyyMMddHHmmssfffff"));
                try
                {
                    bool IsExists = System.IO.Directory.Exists(logPath);
                    if (!IsExists)
                    {
                        System.IO.Directory.CreateDirectory(logPath);
                    }
                    using (StreamWriter sw =
                            new StreamWriter(String.Concat(logPath, "\\",
                                inTrx.StreamFunction, postfix, ".xml")))
                    {
                        sw.Write(trxSecsStr);
                    }
                }
                catch
                {
                    trxLogger.Warn(String.Concat("Failed to write TX Log: ", inTrx.StreamFunction,
                        postfix, ".xml"));
                }
            }
        }

        /// <summary>
        /// SECS通訊結果代碼
        /// </summary>
        public enum ReturnCode
        {
            /// <summary>
            /// 成功
            /// </summary>
            Normal = 0,
            /// <summary>
            /// T3 Timeout
            /// </summary>
            Timeout = 1000,
            /// <summary>
            /// 訊息傳送失敗
            /// </summary>
            SendDataFail = 2000,
            /// <summary>
            /// 連續對話Timeout
            /// </summary>
            ConversationTimeout = 3000,
            /// <summary>
            /// 收到Abort Stream Function
            /// </summary>
            Abort = 4000,
            /// <summary>
            /// 收到System Error Stream Function(S9FY)
            /// </summary>
            SystemError = 5000,
            /// <summary>
            /// 中斷連續對話的等待
            /// </summary>
            BreakcConversationWait = 6000,
        }

        /// <summary>
        /// 指定接續進行談話(等待Host傳遞訊息)，並限定於特定時間內完成，否則回傳「ConversationTimeout」結果
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="outSecs"></param>
        /// <param name="conversationTimeoutSec"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ReturnCode conversationReceive<TSource>(out TSource outSecs, long conversationTimeoutSec,
            Expression<Func<TSource, bool>> predicate)
        {
            return conversationReceive<TSource>(out outSecs, conversationTimeoutSec, predicate, true);
        }

        private ReturnCode conversationReceive<TSource>(out TSource outSecs, long conversationTimeoutSec,
            Expression<Func<TSource, bool>> predicate, Boolean registerHandler)
        {
            outSecs = (TSource)SEUtility.GetDefault(typeof(TSource));
            string receiveSF = string.Empty;
            EventHandler<SECSEventArgs> convReceiveHandler = null;
            try
            {
                receiveSF = (outSecs as SXFY).StreamFunction;
                if (registerHandler)
                {
                    convReceiveHandler = delegate (Object _sender, SECSEventArgs _e)
                    {
                        conversationHandler<TSource>(_sender, _e, predicate);
                    };
                    secsAgent.addSECSReceivedHandler(receiveSF, convReceiveHandler);
                    //                    secsAgent.addSECSReceivedHandler(receiveSF,
                    //                        (_sender, _e) => conversationHandler<TSource>(_sender, _e, predicate));
                }

                long currentValueTime = getNow();
                while (!hasReceiveConv && !isBreakWaitConv())               //A0.04
                //A0.04                while (!hasReceiveConv)
                {
                    long second = (getNow() - currentValueTime) / 1000;
                    if (second >= conversationTimeoutSec)
                    {
                        return ReturnCode.ConversationTimeout;
                    }
                    //A0.05 System.Threading.Thread.Sleep(monitorIntervalMilliseconds);
                    SpinWait.SpinUntil(() => false, monitorIntervalMilliseconds); //A0.05
                }
                if (!hasReceiveConv && isBreakWaitConv())
                {
                    return ReturnCode.BreakcConversationWait;           //A0.04
                }
                hasReceiveConv = false;
                outSecs = (TSource)(Object)conversationSECS;
                //                writeTXLog(conversationSECS, TX_LOG_PATH);
            }
            catch (Exception ex)
            {
                trxLogger.ErrorException("", ex);
            }
            finally
            {
                if (registerHandler)
                {
                    secsAgent.removeSECSReceivedHandler(receiveSF, convReceiveHandler);
                    //                    secsAgent.removeSECSReceivedHandler(receiveSF,
                    //                                    (_sender, _e) => conversationHandler<TSource>(_sender, _e, predicate));
                }
            }
            return ReturnCode.Normal;
        }

        private Boolean isBreakWaitConv()
        {
            if (breakWaitConv == null) { return false; }
            return breakWaitConv.breakConvWait();
        }

        private SXFY conversationSECS = null;
        private Boolean hasReceiveConv = false;
        private IBreakConvWait breakWaitConv = null;             //A0.04
        //public Boolean BreakWaitConv 
        //{
        //    set { breakWaitConv = value; }
        //    get { return breakWaitConv; }
        //}

        private object conversationLock = new object();
        protected virtual void conversationHandler<TSource>(object sender, SECSEventArgs e, Expression<Func<TSource, bool>> predicate)
        {
            lock (conversationLock)
            {
                int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
                try
                {
                    if (hasReceiveConv)
                    {
                        return;
                    }
                    TSource tmpSXFY = (TSource)((Object)e.secsHandler.Parse<TSource>(e));

                    TSource sxfy = tmpSXFY;
                    try
                    {
                        if (predicate != null)
                        {
                            List<TSource> list = new List<TSource>();
                            Type type = typeof(TSource).GetType();
                            list.Add((TSource)tmpSXFY);
                            sxfy = list.AsQueryable().Where(predicate).SingleOrDefault();
                        }
                    }
                    catch (Exception ex)
                    {
                        trxLogger.Error(ex, "Exception:");
                    }

                    if (sxfy == null)
                    {
                        return;
                    }
                    conversationSECS = (SXFY)(Object)tmpSXFY;
                    hasReceiveConv = true;
                }
                catch (Exception ex)
                {
                    trxLogger.Warn("in conversation Handler Exception[threadID:{0}][e.SystemBytes:{1}]:{2}", threadID, e.SystemBytes, ex);
                }
            }
        }

    }
}
