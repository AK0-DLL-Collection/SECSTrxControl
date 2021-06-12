using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Common.SECS;
using com.mirle.ibg3k0.stc.Data.SecsData;
using com.mirle.ibg3k0.stc.Data.SecsData.Sample;
using NLog;
using System.Diagnostics;

namespace SECSTrxControl
{
    class Program
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static Object _lock = new object();
        static void fun_1()
        {
            Console.WriteLine("in fun_1");
            lock (_lock)
            {
                Console.WriteLine("in fun_1 lock 1");
                lockMethod();
                Console.WriteLine("in fun_1 lock 2");
            }
            Console.WriteLine("in fun_1 exist lock");
        }

        static void lockMethod()
        {
            lock (_lock)
            {
                Console.WriteLine("in Lock Method.");
            }
        }

        static void Main(string[] args)
        {
            //fun_1();
            string slotMap = "1000110010";
            string str = new string(slotMap.Reverse().ToArray());
            Console.WriteLine("slotMap: {0}", slotMap);
            Console.WriteLine("str: {0}", str);

            //S1F6_SFCD1 s1f6_sfcd1 = new S1F6_SFCD1()
            //{
            //    PORTID = "P1",
            //    SFCD = "A",
            //    SFCD_1 = new S1F6_SFCD1.SFCD_1_T()
            //    {
            //        CRST = "A",
            //        EQST = "B",
            //        SARY = new S1F6_SFCD1.SITEM[2]
            //    },
            //    SARY_2 = new S1F6_SFCD1.SITEM[2]
            //};
            //s1f6_sfcd1.SFCD_1.SARY[0] = new S1F6_SFCD1.SITEM()
            //{
            //    INDEX = "1",
            //    KEY = "K1"
            //};
            //s1f6_sfcd1.SFCD_1.SARY[1] = new S1F6_SFCD1.SITEM()
            //{
            //    INDEX = "2",
            //    KEY = "K2"
            //};
            //s1f6_sfcd1.SARY_2[0] = new S1F6_SFCD1.SITEM()
            //{
            //    INDEX = "0",
            //    KEY = "K0"
            //};
            //s1f6_sfcd1.SARY_2[1] = new S1F6_SFCD1.SITEM()
            //{
            //    INDEX = "1",
            //    KEY = "Y1"
            //};
            //             TrxSECS.ReturnCode rplyCode = secsAgent.TrxSECS.replySECS(s1f6_sfcd1);

            //             //secsAgent.addSECSReceivedHandler("S1F76", (_sender, _e) => onReceiveSecsHandler(_sender, _e));
            //             S6F11_CEID104 s6f11_ceid104 = new S6F11_CEID104()
            //             {
            //                 DATAID = "A012",
            //                 INFO = new S6F11_CEID104.RPTINFO()
            //                 {
            //                     STATUS = new S6F11_CEID104.RPTSTAT()
            //                     {
            //                         STATINFO = new S6F11_CEID104.STATITEM()
            //                         {
            //                             CRST = "1",
            //                             EQST = "5"
            //                         }
            //                     },
            //                     MODE = new S6F11_CEID104.RPTMODE()
            //                     {
            //                         MODEINFO = new S6F11_CEID104.MODEITEM() 
            //                         {
            //                            OPERMODE = "PR",
            //                            OPERMODEDESC = "Printer Mode"
            //                         }
            //                     }
            //                 }
            //             };

            //    //         secsAgent.addSECSReceivedHandler("S6F11", (_sender, _e) => 
            //    //             replyHandler<S6F11_CEID104>(_sender, _e, s => (s.CEID == "104")));
            //             secsAgent.addSECSReceivedHandler("S6F11", (_sender, _e) =>
            //                  replyHandler<S6F11_CEID104>(_sender, _e, null));
            //             Console.ReadLine();
            ////             TrxSECS.ReturnCode 
            //                 rplyCode = TrxSECS.ReturnCode.SendDataFail;
            //             rplyCode = secsAgent.TrxSECS.replySECS(s6f11_ceid104);
            //             Console.WriteLine("Reply Result: {0}", rplyCode);


            Program prg = new Program();
            prg.doTestSecs();


            Console.ReadLine();
        }

        SECSAgent secsAgent = null;
        void doTestSecs()
        {
            var ipAdr = System.Net.Dns.GetHostAddresses("redis.ohxc.mirle.com.tw");
            string localIP = "127.0.0.1";
            string remoteIP = "127.0.0.1";
            int localPort = 7000;
            int remotePort = 6000;
            int t3Timeout = 30;
            int t5Timeout = 7;
            int t6Timeout = 50;
            int t7Timeout = 5;
            int t8Timeout = 5;
            //             QSACTIVEXLib.HSMS_COMM_MODE connectMode = HSMS_COMM_MODE.HSMS_ACTIVE_MODE;
            SECSAgent.SECS_AGENT_COMM_MODE connectMode = SECSAgent.SECS_AGENT_COMM_MODE.HSMS_PASSIVE_MODE;
            //SECSAgent.SECS_AGENT_COMM_MODE connectMode = SECSAgent.SECS_AGENT_COMM_MODE.HSMS_PASSIVE_MODE;
            //int timeoutSec = 30;
            int retryCount = 0;
            string TX_LOG_PATH = @"N,D:\SECSLog\";
            //string TX_LOG_PATH = @"";
            //

            //


            //             SampleReceiveHandler recHandler = new SampleReceiveHandler();
            secsAgent = new SECSAgent("ABC", 1, localIP, localPort, remoteIP, remotePort, connectMode,
                t3Timeout, t5Timeout, t6Timeout, t7Timeout, t8Timeout, 10, TX_LOG_PATH,
                //timeoutSec, 
                retryCount, SECSAgent.PROCESS_RECEIVE_ORDER.Normal, null, null, checkDataValue);

            secsAgent.start();

            SXFY abortSecs = null;
            Boolean result = false;
            string rtnMsg = string.Empty;
            TrxSECS.ReturnCode rtnCode = TrxSECS.ReturnCode.Normal;
            SECSAgent.FormatExceptionMonitor.addEventHandler("SECSFormatExceptionMonitor",
                SECSFormatExceptionMonitor.getPropertyName(() => SECSAgent.FormatExceptionMonitor.FormatExceptionItem),
                exceptionFormatMonitor);
            //            secsAgent.addSECSDisconnectedHandler(disconnectedHandler);
            //            secsAgent.addSECSConnectedHandler(connectedHandler);
            //secsAgent.addSystemErrorReceivedHandler(s9f1Receive);
            //secsAgent.addSECSReceivedHandler("S6F3", s6f3Receive);

            //            S1F1 s1f1 = new S1F1();
            //            S1F2 s1f2;
            //       //     rtnCode = secsAgent.TrxSECS.sendRecv<S1F2>(s1f1, out s1f2, out abortSecs, out rtnMsg, null);

            //            Console.ReadLine();
            //s7f25TEST();
            //sendS6F103(secsAgent);
            //Thread.Sleep(500);
            //while (true)
            //{
            //    sendS6F3(secsAgent);
            //    Console.ReadLine();
            //}
            //            //secsAgent.setT3(5);
            //            //secsAgent.stop();
            //            //secsAgent.start();

            secsAgent.addSECSReceivedHandler("S2F41", s2f41Receive);
            secsAgent.addSECSConnectedHandler(Connection);
            ////            secsAgent.addSECSReceivedHandler("S1F6", s1f6Receive);
            //            #region Test Format
            ////            S1F6_SFCD4 s1f6 = new S1F6_SFCD4()
            ////            {
            ////                RPTINFO = new S1F6_SFCD4.RPTITEM[2]
            ////            };
            ////            s1f6.RPTINFO[0] = new S1F6_SFCD4.RPTITEM()
            ////            {
            ////                UNITID = "IND",
            ////                UNITST = "R",
            //////                GLSINFO = new S1F6_SFCD4.RPTITEM.GLSITEM[1]
            ////            };
            ////            //s1f6.RPTINFO[0].GLSINFO[0] = new S1F6_SFCD4.RPTITEM.GLSITEM() 
            ////            //{
            ////            //    GLSID = "G000001",
            ////            //    USLOTNO = "1"
            ////            //};
            ////            s1f6.RPTINFO[1] = new S1F6_SFCD4.RPTITEM()
            ////            {
            ////                UNITID = "1APP01-TRC",
            ////                UNITST = "R",
            ////                GLSINFO = new S1F6_SFCD4.RPTITEM.GLSITEM[1]
            ////            };
            ////            s1f6.RPTINFO[1].GLSINFO[0] = new S1F6_SFCD4.RPTITEM.GLSITEM()
            ////            {
            ////                GLSID = "G000022",
            ////                USLOTNO = "2"
            ////            };
            ////            result = secsAgent.replySECSMsg(s1f6);
            ////            Console.WriteLine("S1F1 rtnCode:{0}", rtnCode);
            //            S1F5 s1f5 = new S1F5() 
            //            {
            //                SFCD = "4"
            //            };
            //            S1F6_SFCD4 s1f6 = null;
            //            rtnCode = secsAgent.TrxSECS.sendRecv<S1F6_SFCD4>(s1f5, out s1f6, out abortSecs, out rtnMsg, null);
            //            Console.WriteLine("S1F6 rtnCode:{0}", rtnCode);
            //            S1F2_Empty s1f2_empty = new S1F2_Empty();
            //            result = secsAgent.replySECSMsg(s1f2_empty);
            //            Console.WriteLine("S1F2_Empty rtnCode:{0}", rtnCode);
            //            secsAgent.addSECSReceivedHandler("S1F3", s1f3Receive);
            secsAgent.addSECSReceivedHandler("S1F1", s1f1Receive);
            secsAgent.addSECSReceivedHandler("S1F11", s1f11Receive);
            //     //       secsAgent.addSECSReceivedHandler("S1F17", s1f17Receive);
            //secsAgent.addSECSReceivedHandler("S6F11", s6f11Receive);

            //            #endregion Test Format
            //            #region Test Abort
            //            //S1F1 s1f1 = new S1F1();
            //            //S1F2 s1f2 = null;
            ////            SXFY abortSecs = null;
            // //           rtnCode = secsAgent.TrxSECS.sendRecv<S1F2>(s1f1, out s1f2, out abortSecs, out rtnMsg, null);
            ////            Console.WriteLine("S1F1 rtnCode:{0}", rtnCode);

            ////            secsAgent.setT3(10);
            // //           secsAgent.setDeviceID(5);
            ////            secsAgent.stop();
            ////            secsAgent.refreshConnection();
            ////            secsAgent.refreshConnection();
            ////            secsAgent.start();
            ////            Thread.Sleep(1000);
            ////            Console.ReadLine();
            //            S5F103 s5f103 = new S5F103()
            //            {
            //                UNITS = new S5F103.UNITITEM[1]
            //            };
            //            s5f103.UNITS[0] = new S5F103.UNITITEM()
            //            {
            //                UNITID = "N2"
            //            };
            //            S5F104 s5f104 = null;
            ////            Console.ReadLine();
            // //           rtnCode = secsAgent.TrxSECS.sendRecv<S5F104>(s5f103, out s5f104, out abortSecs, out rtnMsg, null);
            // //           Console.WriteLine("S5F103 rtnCode:{0}", rtnCode);
            //            #endregion Test Abort

            Console.ReadLine();

            #region mark
            //        #region Test S1F1 Header Only
            //        S1F1 s1f1 = new S1F1();
            //        rtnCode = secsAgent.TrxSECS.replySECS(s1f1);
            //        Console.WriteLine("rtnCode = {0}", rtnCode);
            //        #endregion

            //        #region Test S1F6
            //        S1F5 s1f5 = new S1F5() 
            //        {
            //            SFCD = "R"
            //        };
            //        S1F6_SFCD1 s1f6_1 = null;
            //        rtnCode = secsAgent.TrxSECS.sendRecv<S1F6_SFCD1>(s1f5, out s1f6_1, out rtnMsg, null);
            //        Console.WriteLine("S1F5 Result : {0}", rtnCode);

            //        s1f6_1 = new S1F6_SFCD1()
            //        {
            //            SFCD = "2",
            //            SFCD_1 = new S1F6_SFCD1.SFCD_1_T()
            //            {
            //                CRST = "AA",
            //                EQST = "RUN",
            //                SARY = new S1F6_SFCD1.SITEM[3]
            //            },
            //            PORTID = "P1",
            //            SARY_2 = new S1F6_SFCD1.SITEM[2]
            //        };
            //        s1f6_1.SFCD_1.SARY[0] = new S1F6_SFCD1.SITEM()
            //        {
            //            INDEX = "1",
            //            KEY = "K1"
            //        };
            //        s1f6_1.SFCD_1.SARY[1] = new S1F6_SFCD1.SITEM()
            //        {
            //            INDEX = "2",
            //            KEY = "K2"
            //        };
            //        s1f6_1.SFCD_1.SARY[2] = new S1F6_SFCD1.SITEM()
            //        {
            //            INDEX = "3",
            //            KEY = "K3"
            //        };
            //        s1f6_1.SARY_2[0] = new S1F6_SFCD1.SITEM()
            //        {
            //            INDEX = "5",
            //            KEY = "K5"
            //        };
            //        s1f6_1.SARY_2[1] = new S1F6_SFCD1.SITEM()
            //        {
            //            INDEX = "6",
            //            KEY = "K6"
            //        };
            //        string s1f6_str = s1f6_1.toSECSString();
            //        Console.WriteLine("rtnStr={0}", s1f6_str);
            //        rtnCode = secsAgent.TrxSECS.replySECS(s1f6_1);
            //        Console.WriteLine("S1F6 Result : {0}", rtnCode);
            //        #endregion Test S1F6

            //        #region Test S5F5
            //        S5F5 s5f5 = new S5F5() 
            //        {
            //            ALLUNITALAM = new S5F5.ALARMS[2]
            //        };
            //        s5f5.ALLUNITALAM[0] = new S5F5.ALARMS() 
            //        {
            //            UNITID = "N01",
            //            ALIDS = new string[]{ "A01", "B02", "C03"}
            //        };
            //        s5f5.ALLUNITALAM[1] = new S5F5.ALARMS()
            //        {
            //            UNITID = "N02",
            //            ALIDS = new string[] { "D01", "B01", "F05", "U08" }
            //        };
            //        //rtnCode = secsAgent.TrxSECS.replySECS(s5f5);
            //        S5F6 s5f6 = null;
            //        rtnCode = secsAgent.TrxSECS.sendRecv<S5F6>(s5f5, out s5f6, out rtnMsg, null);
            //        Console.WriteLine("S5F5 Result : {0}", rtnCode);
            //        //S5F6 s5f6 = new S5F6() 
            //        //{
            //        //    RPTDATA = new S5F6.REPORTITEM[3]
            //        //};
            //        //s5f6.RPTDATA[0] = new S5F6.REPORTITEM() 
            //        //{
            //        //    UNITID = "N01",
            //        //    ALARMS = new S5F6.REPORTITEM.ALARM[2]
            //        //};
            //        //s5f6.RPTDATA[0].ALARMS[0] = new S5F6.REPORTITEM.ALARM() { ALCD="H", ALID="A001", ALTX="Test A001" };
            //        //s5f6.RPTDATA[0].ALARMS[1] = new S5F6.REPORTITEM.ALARM() { ALCD = "H", ALID = "B005", ALTX = "Test B005" };
            //        //s5f6.RPTDATA[1] = new S5F6.REPORTITEM()
            //        //{
            //        //    UNITID = "N03",
            //        //    ALARMS = new S5F6.REPORTITEM.ALARM[1]
            //        //};
            //        //s5f6.RPTDATA[1].ALARMS[0] = new S5F6.REPORTITEM.ALARM() { ALCD = "L", ALID = "D006", ALTX = "Test D006" };
            //        //s5f6.RPTDATA[2] = new S5F6.REPORTITEM()
            //        //{
            //        //    UNITID = "N08",
            //        //    ALARMS = new S5F6.REPORTITEM.ALARM[3]
            //        //};
            //        //s5f6.RPTDATA[2].ALARMS[0] = new S5F6.REPORTITEM.ALARM() { ALCD = "L", ALID = "S003", ALTX = "Test S003" };
            //        //s5f6.RPTDATA[2].ALARMS[1] = new S5F6.REPORTITEM.ALARM() { ALCD = "H", ALID = "K002", ALTX = "Test K002" };
            //        //s5f6.RPTDATA[2].ALARMS[2] = new S5F6.REPORTITEM.ALARM() { ALCD = "L", ALID = "L001", ALTX = "Test L001" };
            //        //rtnCode = secsAgent.TrxSECS.replySECS(s5f6);
            //        //Console.WriteLine("S5F6 Result : {0}", rtnCode);
            //        #endregion Test S5F5

            //        #region Test S1F11
            //        S1F11 s1f11 = new S1F11() 
            //        {
            //            SVID = new string[]{"B", "T"}
            //        };
            //   //     result = secsAgent.replySECSMsg(s1f11);
            //        S1F12 s1f12 = null;
            //        rtnCode = secsAgent.TrxSECS.sendRecv<S1F12>(s1f11, out s1f12, out rtnMsg, null);
            //        Console.WriteLine("S1F11 Result : {0}", rtnCode);
            //        //S1F12 s1f12 = new S1F12() 
            //        //{
            //        //    SVIDS = new S1F12.SVITEM[3]
            //        //};
            //        //s1f12.SVIDS[0] = new S1F12.SVITEM() { SVID = "AA", SVNAME = "NAME-A"};
            //        //s1f12.SVIDS[1] = new S1F12.SVITEM() { SVID = "AB", SVNAME = "NAME-B" };
            //        //s1f12.SVIDS[2] = new S1F12.SVITEM() { SVID = "AC", SVNAME = "NAME-C" };
            //        //result = secsAgent.replySECSMsg(s1f12);
            //        //Console.WriteLine("S1F12 Result : {0}", result);
            //        #endregion

            //        #region Test S1F3
            //        S1F3 s1f3 = new S1F3() 
            //        {
            //            SVID = new string[]{"3A", "8H"}
            //        };
            //        //Boolean result = secsAgent.replySECSMsg(s1f3);
            //        S1F4 rplyS1f4 = null;
            //        rtnCode = secsAgent.TrxSECS.sendRecv<S1F4>(s1f3, out rplyS1f4, out rtnMsg, null);
            //        Console.WriteLine("S1F3 Result : {0}", rtnCode);
            //        #endregion Test S1F3

            //        #region Test S7F19
            //        S7F19 s7f19 = new S7F19() 
            //        {
            //            UNITID = "N1",
            //            PPTYPE = "E"
            //        };
            //        S7F20 s7f20 = null;
            //        string msg = string.Empty;
            //        rtnCode = secsAgent.TrxSECS.sendRecv<S7F20>(s7f19, out s7f20, out msg, null);
            //        Console.WriteLine("rtnCode = {0}", rtnCode);
            //        s7f20 = new S7F20() 
            //        {
            //            UNITID = "N1",
            //            PPTYPE = "E",
            //            PPIDLIST = new string[]{"R01", "R02", "R03"},
            //            VERSION = "V0.01"
            //        };
            //        //s7f20.PPIDLIST[0] = new S7F20.PPIDItem() { PPID = "R01" };
            //        //s7f20.PPIDLIST[1] = new S7F20.PPIDItem() { PPID = "R02" };
            //        //s7f20.PPIDLIST[2] = new S7F20.PPIDItem() { PPID = "R03" };
            //        result = secsAgent.replySECSMsg(s7f20);
            //        Console.WriteLine("reuslt = {0}", result);
            //        #endregion Test S7F19


            //        S2F31 s2f31 = new S2F31()
            //        {
            //            TIME = "20140828155900"
            //        };
            //        S2F32 s2f32 = null;

            //        Console.ReadLine();
            //        rtnCode = secsAgent.TrxSECS.sendRecv<S2F32>(s2f31, out s2f32, out rtnMsg, null);
            //        Console.WriteLine("rtnCode = {0}", rtnCode);
            //        Console.ReadLine();
            //        ///

            //        S1F75 s1f75_1 = (S1F75)SEUtility.GetDefault(typeof(S1F75));
            //        s7f19 = new S7F19() 
            //        {
            //            SECSAgentName = "ABC"
            //        };
            //        result = secsAgent.replySECSMsg(s7f19);
            //        Console.WriteLine("Result = {0}", result);

            //        secsAgent.addSECSReceivedHandler("S7F19", (_sender, _e) =>
            //             onReceiveSecsHandler(_sender, _e));

            //        secsAgent.addSECSReceivedHandler("S6F11", (_sender, _e) =>
            //             onReceiveSecsHandler(_sender, _e));

            //        S1F75 s1f75 = new S1F75()
            //        {
            //            LINE_ID = "LN-01",
            //            EQPT_ID = "PCLN1",
            //            PORT_ID = "P1",
            //            SystemByte = 1002,
            //            W_Bit = 1
            //        };

            //        S1F75 s1f75_2 = new S1F75()
            //        {
            //            LINE_ID = "LN-02",
            //            EQPT_ID = "PCLN2",
            //            PORT_ID = "P2",
            //            SystemByte = 1003,
            //            W_Bit = 1
            //        };

            ////        ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(doSendS1F75), s1f75);
            ////        ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(doSendS1F75), s1f75_2);
            #endregion
        }


        protected void sendS6F103(SECSAgent agent)
        {
            S6F103 s6f103 = new S6F103()
            {
                CSTENDFLAG = "1",
                CSTID = "CST01",
                GLSITEMS = new S6F103.GLSITEM[2]
            };
            s6f103.GLSITEMS[0] = new S6F103.GLSITEM()
            {
                DUMUSEDCNT = "0",
                GLSID = "GLS01",
                GLSGRADE = "G",
                GLSIDTYPE = "L",
                GLSJUDGE = "A",
                GLSST = "4",
                GLSSIZE = "800*100",
                GLSTHK = "A",
                UNITITEMS = new S6F103.GLSITEM.UNITITEM[1]
            };
            s6f103.GLSITEMS[0].UNITITEMS[0] = new S6F103.GLSITEM.UNITITEM()
            {
                UNITID = "CVD",
                SUNITIDS = new string[2] { "TC1", "LLU" }
            };
            Console.WriteLine(s6f103.toSECSString());
            agent.replySECSMsg(s6f103);
        }

        protected void sendS6F3(SECSAgent agent)
        {
            List<S6F3_CEID500_501.DVDATA> dvDataList = new List<S6F3_CEID500_501.DVDATA>();
            var data = new S6F3_CEID500_501.DVDATA()
            {
                DVNAME = "ABC",
                SUBITEMS = new S6F3_CEID500_501.SUBITEM[1]
            };
            data.SUBITEMS[0] = new S6F3_CEID500_501.SUBITEM()
            {
                SITENAME = "G",
                DV = "DEF"
            };
            for (int i = 0; i < 1; i++)
                dvDataList.Add(data);

            S6F3_CEID500_501 s6f3 = new S6F3_CEID500_501()
            {
                CEID = SECSConst.CEID_Glass_Process_Data,//"500",
                DATA = new S6F3_CEID500_501.PROCDATA()
                {
                    //                                        UNITID = unit_id,
                    UNITID = "1AEW01-WEE",
                    OPERID = "OPER01",
                    LOTID = "LOTID-0116-A",
                    PRODID = "NORMALPROD",
                    PPID = "P001",
                    GLSID = "GLASS01",
                    CSTID = "CSTID",
                    DVDATAS = dvDataList.ToArray()
                },
                SECSAgentName = "ABC"
            };
            agent.replySECSMsg(s6f3);
        }

        protected void disconnectedHandler(object sender, SECSEventArgs e)
        {
            string agentName = e.secsHandler.Agent.Name;
            string eventText = e.MsgID.ToString();
            Console.WriteLine("[Agent Name:{0}][EventText:{1}]  Disconnected !!!!!!!!!!!!!!!!", agentName, eventText);
        }

        protected void connectedHandler(object sender, SECSEventArgs e)
        {
            string agentName = e.secsHandler.Agent.Name;
            string eventText = e.MsgID.ToString();
            Console.WriteLine("[Agent Name:{0}][EventText:{1}] Connected !!!!!!!!!!!!!!!!", agentName, eventText);
        }

        protected void exceptionFormatMonitor(object sender, PropertyChangedEventArgs e)
        {
            string message = SECSAgent.FormatExceptionMonitor.FormatExceptionItem.Message;
            Console.WriteLine(message);
        }

        public com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT checkDataValue(string name, string value)
        {
            com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT result =
                SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Recognize;

            if (name.Trim().Equals("CRST"))
            {
                //SECSConst.CRST
                if (!SECSConst.CRST.Contains(value.Trim()))
                {
                    return SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format;
                }
            }

            return result;
        }

        // public com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT checkDataValue(Type type, SXFY sxfy) 
        // {
        //     com.mirle.ibg3k0.stc.Common.SECS.SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT result = 
        //         SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Recognize;

        ////     SXFY tmpSXFY = (SXFY)Activator.CreateInstance(type);
        //     FieldInfo[] fields = SEUtility.GetSeceElementFields(sxfy.GetType());
        //     for (int ix = 0; ix < fields.Length; ++ix) 
        //     {
        //         FieldInfo field = fields[ix];
        //         if (field.FieldType.IsArray && field.FieldType.GetElementType().BaseType == typeof(SXFY)) 
        //         {
        //             SXFY[] subArray = (SXFY[])field.GetValue(sxfy);
        //             for (int index = 0; index < subArray.Length; ++index) 
        //             {
        //                 SXFY eleSxfy = subArray[index];
        //                 result = checkDataValue(eleSxfy.GetType(), eleSxfy);
        //                 if (result == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format) 
        //                 {
        //                     return result;
        //                 }
        //             }
        //         }
        //         else if (field.FieldType.IsArray && field.FieldType.GetElementType() == typeof(string)) 
        //         {
        //             string[] subArray = (string[])field.GetValue(sxfy);
        //             for (int index = 0; index < subArray.Length; ++index)
        //             {
        //                 string eleStr = subArray[index];
        //                 //result = checkDataValue(eleStr.GetType(), eleStr);
        //                 if (result == SECSAgent.SECS_STREAM_FUNCTION_CHECK_RESULT.Illegal_Data_Value_Format)
        //                 {
        //                     return result;
        //                 }
        //             }
        //         }
        //         else if (field.FieldType.BaseType == typeof(SXFY))
        //         {

        //         }
        //         else 
        //         {

        //         }
        //         String name = field.Name;
        //         if (name.Trim().Equals("CRST")) 
        //         {
        //             object value = field.GetValue(sxfy);

        //         }
        //     }

        //     return result;
        // } 

        protected void s1f3Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S1F3 s1f3 = ((S1F3)e.secsHandler.Parse<S1F3>(e));
                Console.WriteLine("s1f3[{0}]", s1f3.SystemByte);

                logger.Debug("S1F3");

            }
            catch (Exception ex)
            {
                logger.ErrorException("s2f41Receive Exception!", ex);
            }
        }

        protected void s1f6Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S1F6 s1f6 = ((S1F6)e.secsHandler.Parse<S1F6>(e));
                Console.WriteLine("s1f6[{0}] SFCD[{1}]", s1f6.SystemByte, s1f6.SFCD);
                S1F6_SFCD4 s1f6_sfcd4 = ((S1F6_SFCD4)e.secsHandler.Parse<S1F6_SFCD4>(e));
                Console.WriteLine("s1f6[{0}] SFCD[{1}]", s1f6.SystemByte, s1f6_sfcd4.toSECSString());
                logger.Debug("S1F3");

            }
            catch (Exception ex)
            {
                logger.ErrorException("s2f41Receive Exception!", ex);
            }
        }

        protected void Connection(object sender, SECSEventArgs e)
        {

        }

        protected void s2f41Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S2F41_RCMD1_5 s2f41 = ((S2F41_RCMD1_5)e.secsHandler.Parse<S2F41_RCMD1_5>(e));
                Console.WriteLine("s6f11[{0}]", s2f41.SystemByte);
                logger.Debug("S6F11");

            }
            catch (Exception ex)
            {
                logger.ErrorException("s2f41Receive Exception!", ex);
            }
        }

        protected void s6f3Receive(object sender, SECSEventArgs e)
        {
            try
            {
                //     S6F3 s6f3 = ((S6F3)e.secsHandler.Parse<S6F3>(e.RawData, e.SystemBytes, e.W_Bit));
                //     S6F3_CEID500_501 s6f3_CEID500_501 = ((S6F3_CEID500_501)e.secsHandler.Parse<S6F3_CEID500_501>(e.RawData, e.SystemBytes, e.W_Bit));
                S6F3 s6f3 = ((S6F3)e.secsHandler.Parse<S6F3>(e));
                Console.WriteLine("S6F3[{0}]", s6f3.SystemByte);
                if (s6f3.CEID == "500")
                {
                    DateTime startTime = DateTime.Now;
                    S6F3_CEID500_501 s6f3_CEID500_501 = ((S6F3_CEID500_501)e.secsHandler.Parse<S6F3_CEID500_501>(e));
                    double totalMs = DateTime.Now.Subtract(startTime).TotalMilliseconds;
                    Console.WriteLine("s6f3_CEID500_501[{0}][Milliseconds:{1}]", s6f3_CEID500_501.SystemByte, totalMs);
                }

                logger.Debug("S6F3");

            }
            catch (Exception ex)
            {
                logger.ErrorException("s6f3Receive Exception!", ex);
            }
        }

        protected void s2f103Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S2F103 s6f11 = ((S2F103)e.secsHandler.Parse<S2F103>(e));

            }
            catch (Exception ex)
            {
                logger.ErrorException("s6f11Receive Exception!", ex);
            }
        }

        protected void s6f11Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S6F11 s6f11 = ((S6F11)e.secsHandler.Parse<S6F11>(e));
                Console.WriteLine("s6f11[{0}]", s6f11.SystemByte);
                logger.Debug("S6F11");
                if (s6f11.CEID.Trim().Equals("324"))
                {
                    S6F11_CEID321_326 s6f11_324 = ((S6F11_CEID321_326)e.secsHandler.Parse<S6F11_CEID321_326>(e));
                    Console.WriteLine(s6f11_324.toSECSString());
                    logger.Debug("S6F11 CEID 324");
                }
                else if (s6f11.CEID.Trim().Equals("401"))
                {
                    S6F11_CEID401 s6f11_401 = ((S6F11_CEID401)e.secsHandler.Parse<S6F11_CEID401>(e));
                    Console.WriteLine(s6f11_401.toSECSString());
                    logger.Debug("S6F11 CEID 401");
                }
            }
            catch (Exception ex)
            {
                logger.ErrorException("s6f11Receive Exception!", ex);
            }
        }

        //protected void s7f25TEST()
        //{
        //    try
        //    {

        //        //List<S6F3_CEID500_501.DVDATA> dvDataList = new List<S6F3_CEID500_501.DVDATA>();
        //        //var data = new S6F3_CEID500_501.DVDATA()
        //        //{
        //        //    DVNAME = "ABC",
        //        //    SUBITEMS = new S6F3_CEID500_501.SUBITEM[1]
        //        //};
        //        //data.SUBITEMS[0] = new S6F3_CEID500_501.SUBITEM()
        //        //{
        //        //    SITENAME = "G",
        //        //    DV = "DEF"
        //        //};
        //        //for (int i = 0; i < 1; i++)
        //        //    dvDataList.Add(data);


        //        S7F26.CODEITEM[] arrayCODEITEM = new S7F26.CODEITEM[10];
        //        for (int i = 0; i < arrayCODEITEM.Count(); i++)
        //        {

        //            S7F26.CODEITEM.PARMITEM[] attayPARMITEM = new S7F26.CODEITEM.PARMITEM[10];
        //            for (int j = 0; j < attayPARMITEM.Count(); j++)
        //            {
        //                attayPARMITEM[j] = new S7F26.CODEITEM.PARMITEM()
        //                {
        //                    PPARMNAME = "123" + j,
        //                    PPARMVALUE = "456" + j
        //                };
        //            }
        //            arrayCODEITEM[i] = new S7F26.CODEITEM()
        //            {
        //                CCODE = "123" + i,
        //                RCPSTEP = "" + i,
        //                UNITID = "" + i,
        //                SUNITID = "" + i,
        //                PARMITEMS = attayPARMITEM
        //            };
        //        }
        //        S7F26 S7F26 = new com.mirle.ibg3k0.stc.Data.SecsData.Sample.S7F26()
        //        {
        //            PPID = "111",
        //            PPTYPE = "P",
        //            MDLN = "666666",
        //            SOFTREV = "666",
        //            LCTIME = "20170426",
        //            CODEITMES = arrayCODEITEM
        //        };

        //        object rawData = null;

        //        S7F26.parser2RawData(ref rawData, secsAgent.getQSWrapper());
        //        //SECSHandler handler = secsAgent.getSecsHandler();
        //        SECSEventArgs eventArgs = new SECSEventArgs(0, 1,
        //                QSACTIVEXLib.EVENT_ID.QS_EVENT_RECV_MSG, 7, 26, 0, 132, rawData, null, "", handler);
        //        Stopwatch sw = new Stopwatch();
        //        Console.WriteLine("Start Parse");
        //        sw.Start();
        //        //S7F26 = (S7F26)handler.Parse<S7F26>(eventArgs);
        //        Console.WriteLine(sw.ElapsedMilliseconds + "ms");
        //        Console.WriteLine("END Parse");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.ErrorException("s6f11Receive Exception!", ex);
        //    }
        //}

        protected void s1f1Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S1F1 s1f1 = ((S1F1)e.secsHandler.Parse<S1F1>(e));
                Console.WriteLine("s1f1[{0}]", s1f1.SystemByte);
                logger.Debug("S1F1");
            }
            catch (Exception ex)
            {
                logger.ErrorException("s1f1Receive Exception!", ex);
            }
        }
        protected void s9f1Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S9F1 s1f1 = ((S9F1)e.secsHandler.Parse<S9F1>(e));
                Console.WriteLine("s1f1[{0}]", s1f1.SystemByte);
                logger.Debug("S1F1");
            }
            catch (Exception ex)
            {
                logger.ErrorException("s1f1Receive Exception!", ex);
            }
        }

        protected void s2f17Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S2F17 s1f11 = ((S2F17)e.secsHandler.Parse<S2F17>(e));
                Console.WriteLine("s1f11[{0}]", s1f11.SystemByte);
                logger.Debug("S1F11");
            }
            catch (Exception ex)
            {
                logger.ErrorException("s1f11Receive Exception!", ex);
            }
        }

        protected void s1f17Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S1F17 s1f17 = ((S1F17)e.secsHandler.Parse<S1F17>(e));
                Console.WriteLine("S1F17[{0}]", s1f17.SystemByte);
                logger.Debug("S1F17");
            }
            catch (Exception ex)
            {
                logger.ErrorException("s1f17Receive Exception!", ex);
            }
        }

        protected void s1f11Receive(object sender, SECSEventArgs e)
        {
            try
            {
                S1F11 s1f11 = ((S1F11)e.secsHandler.Parse<S1F11>(e));
                Console.WriteLine("s1f11[{0}]", s1f11.SystemByte);
                logger.Debug("S1F11");
            }
            catch (Exception ex)
            {
                logger.ErrorException("s1f11Receive Exception!", ex);
            }
        }

        void doSendS1F75(Object inSecs)
        {
            S1F75 s1f75 = inSecs as S1F75;
            S1F76 s1f76 = null;
            string rtnMsg = null;
            S2F21 s2f21 = null;
            SXFY abortSecs = null;
            TrxSECS.ReturnCode rtnCode = secsAgent.TrxSECS.sendRecv<S1F76>(s1f75, out s1f76, out abortSecs, out rtnMsg,
                s => (s.SystemByte == s1f75.SystemByte));
            //TrxSECS.ReturnCode rtnCode = secsAgent.TrxSECS.sendRecv<S1F76, S2F21>(s1f75, out s1f76, out rtnMsg, 
            //    s => (s.SystemByte == s1f75.SystemByte),
            //    out s2f21, 30, null);
            if (rtnCode == TrxSECS.ReturnCode.Normal)
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!Receive suceess!");
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!Stream Function:{0} ; System Byte:{1} ; W Bit:{2} ; LineID:{3}",
                    s1f76.StreamFunction, s1f76.SystemByte, s1f76.W_Bit, s1f76.LINE_ID);
            }
            else
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!Receive fail [{0}]", rtnCode);
            }
        }

        static void replyHandler<TSource>(object sender, SECSEventArgs e,
            Expression<Func<TSource, bool>> predicate)
        {
            TSource tmpSXFY = (TSource)((Object)e.secsHandler.Parse<TSource>(e));

            TSource sxfy = tmpSXFY;
            if (predicate != null)
            {
                List<TSource> list = new List<TSource>();
                Type type = typeof(TSource).GetType();
                list.Add((TSource)tmpSXFY);
                sxfy = list.AsQueryable().Where(predicate).SingleOrDefault();
            }

            if (sxfy == null)
            {
                return;
            }
            SXFY recvSXFY = (SXFY)(Object)tmpSXFY;
            System.Console.WriteLine("StreamFunction:{0} ; System Byte:{1} ; W Bit:{2}",
                recvSXFY.StreamFunction, recvSXFY.SystemByte, recvSXFY.W_Bit);

        }

        static void onReceiveSecsHandler(object sender, SECSEventArgs e)
        {
            Console.WriteLine("Event Text:{0}", e.EventText);
            int S = e.S;
            int F = e.F;
            Console.WriteLine("S = {0}, F = {1}", S, F);
            S7F19 s7f19 = null;
            s7f19 = ((S7F19)e.secsHandler.Parse<S7F19>(e));
            Console.WriteLine("SF = {0}", s7f19.StreamFunction);
            //Do Something
            S6F11 s6f11 = ((S6F11)e.secsHandler.Parse<S6F11>(e));
            string ceid = s6f11.CEID;
            Console.WriteLine("CEID:{0}", ceid);
            if (ceid.Trim().Equals("104"))
            {
                S6F11_CEID104 s6f11_CEID106 =
                    ((S6F11_CEID104)e.secsHandler.Parse<S6F11_CEID104>(e));
                Console.WriteLine("Oper Mode:{0}", s6f11_CEID106.INFO.MODE.MODEINFO.OPERMODE);
            }
        }

        //        static void Main_00(string[] args)
        //        {
        //            Program prg = new Program();
        //            logger.Info("Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);
        //            SampleReceiveHandler recHandler = new SampleReceiveHandler();
        //            int l_SystemBytes = 0;
        //            SECSAgent secsAgent = new SECSAgent(recHandler);
        //            secsAgent.start();
        // //           secsAgent.stop();

        //            //SECSAgent secsAgent2 = new SECSAgent();
        //            //secsAgent2.start();
        //            //secsAgent2.stop();

        //            //SECSAgent secsAgent3 = new SECSAgent();
        //            //secsAgent3.start();
        //            //secsAgent3.stop();

        //            QSACTIVEXLib.QSWrapper qsWrapper = secsAgent.getQSWrapper();//new QSACTIVEXLib.QSWrapper();
        ////            qsWrapper.Start();
        //            #region 測試
        //            //SECSHandler handler = new SECSHandler(qsWrapper);
        //            //handler.Parse<S1F6_SFCD1>("");

        //            S7F71 s7f71 = new S7F71() 
        //            {
        //                cstid = "C002",
        //                eqptid = "CLEN",
        //                inlineid = "L01",
        //                portid = "P01",
        //                runmode = "M"
        //            };
        //            string rtnStr = s7f71.toSECSString();
        //            Console.WriteLine("rtnStr={0}", rtnStr);

        //            S1F6_SFCD1 s1f6_1 = new S1F6_SFCD1()
        //            {
        //                SFCD = "2",
        //                SFCD_1 = new S1F6_SFCD1.SFCD_1_T() 
        //                {
        //                    CRST = "AA",
        //                    EQST = "RUN",
        //                    SARY = new S1F6_SFCD1.SITEM[3]
        //                },
        //                PORTID = "P1",
        //                SARY_2 = new S1F6_SFCD1.SITEM[2]
        //            };
        //            s1f6_1.SFCD_1.SARY[0] = new S1F6_SFCD1.SITEM() {
        //                INDEX = "1",
        //                KEY = "K1"
        //            };
        //            s1f6_1.SFCD_1.SARY[1] = new S1F6_SFCD1.SITEM()
        //            {
        //                INDEX = "2",
        //                KEY = "K2"
        //            };
        //            s1f6_1.SFCD_1.SARY[2] = new S1F6_SFCD1.SITEM()
        //            {
        //                INDEX = "3",
        //                KEY = "K3"
        //            };
        //            s1f6_1.SARY_2[0] = new S1F6_SFCD1.SITEM() 
        //            {
        //                INDEX = "5",
        //                KEY = "K5"
        //            };
        //            s1f6_1.SARY_2[1] = new S1F6_SFCD1.SITEM()
        //            {
        //                INDEX = "6",
        //                KEY = "K6"
        //            };
        //            rtnStr = s1f6_1.toSECSString();
        //            Console.WriteLine("rtnStr={0}", rtnStr);
        //            #endregion
        //            Object rawData = null;
        //            s1f6_1.parser2RawData(ref rawData, qsWrapper);
        //            if (rawData.GetType().IsArray) 
        //            {
        //                Console.WriteLine("rawData is Array!");
        //            }
        //            //qsWrapper.SendSECSIIMessage(1, 6, 1, ref l_SystemBytes, rawData);
        //            Boolean isSuccess = secsAgent.replySECSMsg(s1f6_1);
        //            Console.WriteLine("Reply Success:{0}", isSuccess);
        //       //     secsAgent.start();
        //            //S1F6 s1f6 = (S1F6)handler.Parse<S1F6>(rawData);
        //            //Console.WriteLine("SFCD:{0}", s1f6.SFCD);
        //            //S1F6_SFCD1 s1f6_sfcd1 = (S1F6_SFCD1)handler.Parse<S1F6_SFCD1>(rawData);
        //            //Object rawData_2 = null;
        //            //s1f6_sfcd1.parser2RawData(ref rawData_2, qsWrapper);
        //            //qsWrapper.SendSECSIIMessage(1, 6, 1, ref l_SystemBytes, rawData_2);
        //            //if (s7f71.GetType().BaseType == typeof(SXFY)) 
        //            //{
        //            //    Console.WriteLine("Type is SXFY!");
        //            //}

        //            //FieldInfo[] fields = null;
        //     //       secsAgent.secsReceive += new EventHandler<SECSEventArgs>(prg.onReceiveSecs);

        //            Console.ReadLine();

        //        }

        private void onReceiveSecs(object sender, SECSEventArgs e)
        {
            //e.EventText;
            Console.WriteLine("Event Text:{0}", e.EventText);
            int S = e.S;
            int F = e.F;
            switch (S)
            {
                case 1:
                    break;
                case 2:
                    S2_Handler(e);
                    break;
                default:
                    Console.WriteLine("Not Match Handler!");
                    break;
            }
            //e.secsHandler.Parse<
        }

        private void S2_Handler(SECSEventArgs e)
        {
            int S = e.S;
            int F = e.F;
            if (F == 21)
            {
                logger.Info("Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);
                S2F21 s2f21 = (S2F21)e.secsHandler.Parse<S2F21>(e);
                Console.WriteLine("Port ID:{0}", s2f21.PORTID);
            }
        }

    }
}
