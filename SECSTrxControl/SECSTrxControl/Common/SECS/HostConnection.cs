//*********************************************************************************
//      HostConnection.cs
//*********************************************************************************
// File Name: HostConnection.cs
// Description: Host Connection抽象類別
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/05/30    Hayes Chen     N/A            N/A     Initial Release
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.stc.Common.SECS
{
    public abstract class HostConnection
    {
        protected SECSAgent secsAgent;
        public string strmessage = "";
        //abstract public bool replySECSMsg(XmlDocument sendDoc);
        abstract public bool replySECSMsg(SXFY sendObj);
        abstract public void replyTimeout();
        abstract public void startToListen();
        abstract public void stopToListen();

        abstract public void setT3(int t3Timeout);
        abstract public void setT5(int t5Timeout);
        abstract public void setT6(int t6Timeout);
        abstract public void setT7(int t7Timeout);
        abstract public void setT8(int t8Timeout);
        abstract public void setLinkTestTimer(int linkTestTimer);
        abstract public void setDeviceID(int deviceID);
//        abstract public void refreshConnection();
        abstract public void destroy();
    }
}
