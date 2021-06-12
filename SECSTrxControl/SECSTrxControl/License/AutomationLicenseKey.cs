﻿//*********************************************************************************
//      AutomationLicenseKey.cs
//*********************************************************************************
// File Name: AutomationLicenseKey.cs
// Description: AutomationLicenseKey
//
//(c) Copyright 2015, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2015/06/02    Kevin Wei      N/A            A0.01   修正Check Validation確認的方式。由原本藉由LogIn/LogOut的方式確認，
//                                                     改為由Encrypt/Decrypt的方式做確認，並記錄Log。
// 2015/06/09    Kevin Wei      N/A            A0.02   允許當Key Pro內部設定"協助功能->遠端桌面"，並未勾選時，
//                                                     若此時"HaspStatus的Return Code為TerminalServiceDetected"，還是要算登入成功。
// 2015/06/09    Kevin Wei      N/A            A0.03   由原本的1分鐘確認一次，改為10分鐘確認一次"ValidationCheck"。
// 2015/08/18    Kevin Wei      N/A            A0.04   增加當尚未到要執行License Check時(即已超過間隔時間10分鐘)，就不會再次執行。
// 2015/08/26    Kevin Wei      N/A            A0.05   將License Check的Check方式，由Log In/Log Out改為"encrypt"的比較方式。
//**********************************************************************************
using Aladdin.HASP;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.License
{
    public class AutomationLicenseKey : ILicenseKey
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //A0.03 private static int intervalCheckSec = 60;//20;
        private static int intervalCheckSec = 600;//20; A0.03
        private static int totalTrialSec = 3600;//60;

        private static string LicenseKeyTrialVersion =
            "This is a trial version, " +
            "The software will be stopped after 60 mins. \n" +
            "Please contact MIRLE for this authority issue. \n" +
            "copyright. MIRLE.3K0";

        private static int dataSize = 80;
        private static string vendorCode =
            "LE+M1JkdQDfpg4Vpk5A5oKyz6xhjY8zICF/A4LDd4vxqKLkLrbvQeEyXVAV/QESylxDxVbSTxR4Ha2AK" +
            "kJzX1jkZdok18GASOz/5zP3kCWuK4/buzwCYuAHyiiSs5C7P0aGW0gwXM3f9tyPO1WOVu8LEFfulUf2q" +
            "WYf5zplPmk/LAzQzqXl8pzgeeMn2WZ7kNV3rtlNvo/Q+n4NdjeQjU2uTz+yrGBghszCfYsZjk+zL4VDN" +
            "Y1qS5ny1TLOZFDE411FdkHMAUONIhkrXHYX6OPYmxB9dXqpkLp7mDyAyAihB3U2EUFo4jXm2xMj5n8yt" +
            "Rnrph/NTCgWYjIK6rE9Xv8tl6H3xJ6GLeQBUWDVMAbNYt6So4IThF7OiNmxrILv+RTgJnPJ14xbdQR6M" +
            "lgHDJliqZWPkjp+RXttmy8q15yxaD9Rqd28VGZuH4xZtXrpHJybNR1LOCAZSPUDnLd6V12miMU/r+rXB" +
            "5O+pmcBJ1Fsrdc5UvCJE0fwmOr3ukPYAO/WILg1OgIjpI20+EYfGTiMVKviIdGtpG/id1+CU7TJkaRFd" +
            "aAhBIibLItsv8Ge71jZzkDDnjKvsraVzD/XfiVanOP2GpLXZMvIXq/5/G8zdbnrs5D6VX4UfFKUbJAfm" +
            "Gc/gsp1JyDUzaxXVmoe+mN82qROHTkXoBIol2RD4rpSrxOglWp8VpweyYLTgARQOpblyEgI9GiUURw38" +
            "shENLLXRgCtERZmRYu7upay0JyraHVMGVPrS8pQxaF6eBqOz3+1oYLTgo9+k1sb7LMDlcOeIv5I0DRhb" +
            "8/t4yNMumX+ZNw2a8eTM3JIpv2ceZ//FrR1YWJaNAC50Wgb3MfLaOG5FoKAkfAwuhCNMcN2+q88ZExh3" +
            "iLDoel0kgOkWZM2MTuhdp1TS/VnDSLTIxiwZS0jC2UktAJHm2XrX7lMoHuq0j3e19MG8OaJLyMHAZ0un" +
            "P5n5cdxju9bHCMuOyPnYCg==";

        private static readonly string encryptString = "copyright. MIRLE.3K0"; //A0.01
        private string encryptedString = string.Empty;                         //A0.01

        private HaspFeature feature;
        private Hasp hasp = null;
        private HaspFile file;

        private byte[] data = new byte[dataSize];

        private ILicenseKeyProtect agent;
        private string localhostname;

        private Boolean checkFailFlag = false;
        private Boolean CheckFailFlag
        {
            get { return checkFailFlag; }
            set
            {
                if (value == checkFailFlag)
                {
                    return;
                }
                if (checkFailFlag == false && value == true)
                {
                    //檢查失敗
                    //                    agent.invalidLicense("License Key is Invalidation !");
                    agent.invalidLicense(LicenseKeyTrialVersion);
                    checkFailTime = DateTime.Now;
                    //啟動 trialTimer 
                    startTrialTimer();
                }
                else if (checkFailFlag == true && value == false)
                {
                    stopTrialTimer();
                }
                checkFailFlag = value;
            }
        }
        private DateTime checkFailTime;
        private Timer trialTimer;
        private Timer checkTimer;

        public AutomationLicenseKey(ILicenseKeyProtect agent)
        {
            this.agent = agent;
            localhostname = Environment.UserDomainName;
            feature = HaspFeature.FromFeature(1);
            hasp = new Hasp(feature);
            file = hasp.GetFile(HaspFileId.ReadOnly);
            //
            startCheckTimer();
        }

        private async void startCheckTimer()
        {
            await Task.Delay(10);
            if (checkTimer != null)
            {
                checkTimer.Change(Timeout.Infinite, Timeout.Infinite);
                checkTimer.Dispose();
                checkTimer = null;
            }
            checkTimer = new Timer(new TimerCallback(checkValidation), null, 0, intervalCheckSec * 1000);
        }

        private async void stopCheckTimer()
        {
            await Task.Delay(10);
            if (checkTimer != null)
            {
                checkTimer.Change(Timeout.Infinite, Timeout.Infinite);
                checkTimer.Dispose();
                checkTimer = null;
            }
        }

        private async void startTrialTimer()
        {
            await Task.Delay(10);
            if (trialTimer != null)
            {
                trialTimer.Change(Timeout.Infinite, Timeout.Infinite);
                trialTimer.Dispose();
                trialTimer = null;
            }
            trialCount = 0;
            trialTimer = new Timer(new TimerCallback(trialTimeOut), null, 0, totalTrialSec * 1000);
        }

        private async void stopTrialTimer()
        {
            await Task.Delay(10);
            if (trialTimer != null)
            {
                trialTimer.Change(Timeout.Infinite, Timeout.Infinite);
                trialTimer.Dispose();
                trialTimer = null;
            }
            trialCount = 0;
        }

        private int trialCount = 0;
        private void trialTimeOut(Object obj)
        {
            if (trialCount++ == 0) { return; }
            //停止系統運作
            agent.stopSystemByLicenseCheck();
            //
            //            stopTrialTimer();

        }

        public String getHostname()
        {
            return localhostname;
        }

        public DateTime getExpirationDate()
        {
            return DateTime.Now;
        }

        private long syncCheckPoint = 0;
        private Boolean isValidationCheck()
        {
            if (System.Threading.Interlocked.Exchange(ref syncCheckPoint, 1) == 0)
            {
                try
                {
                    if (hasp.IsLoggedIn())
                    {
                        //hasp.Logout();
                        ReportStatus("Logout", hasp.Logout());      //A0.01
                    }
                    HaspStatus status = hasp.Login(vendorCode);
                    ReportStatus("Login", status);                  //A0.01

                    if (HaspStatus.StatusOk != status && HaspStatus.AlreadyLoggedIn != status 
                        && HaspStatus.TerminalServiceDetected != status) //A0.02 
                    {
                        try
                        {
                            CheckFailFlag = true;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Exception:");
                        }
                        return false;
                    }
                    //else 
                    //{
                    //    status = file.Read(data, 0, data.Length);
                    //    if (HaspStatus.StatusOk == status)
                    //    {
                    //        string result = System.Text.Encoding.UTF8.GetString(data);
                    //        Console.WriteLine("Data:[{0}]", result);
                    //    }
                    //}
                    CheckFailFlag = false;
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncCheckPoint, 0);
                }
            }
            return true;
        }

        public bool isValidation()
        {
            return isValidationCheck();
        }

        /// <summary>
        /// 此FUN 要等到GVO現場的Key皆改為允許遠端控制後，再進行使用。//A0.05
        /// </summary>
        /// <param name="obj"></param>
        private void checkValidation(Object obj)
        {
            //A0.01 Add Beging (1/2)
            try
            {
                bool checkResult = false;
                if (hasp.IsLoggedIn())
                {
                    checkResult = isValidationCheckByEncryptDecrypt();
                    if (!checkResult)
                    {
                        isValidationCheck();
                    }
                }
                else
                {
                    checkResult = isValidationCheck();
                    if (checkResult)
                    {
                        encryptedString = doEncrypt(encryptString);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            //A0.01 Add End (1/2)

            //A0.01   if (!isValidationCheck())
            //A0.01   {
            //A0.01       //A0.01   
            //A0.01   }
        }

        /// <summary>
        /// 在現場尚未將KeyPro中的"允許遠端桌面"設定開啟時，先使用使FUN。//A0.05
        /// </summary>
        /// <param name="obj"></param>
        //private void checkValidation(Object obj)
        //{
        //    if (!isValidationCheck())
        //    {
        //        //
        //    }
        //}

        DateTime preCheckTime = DateTime.Now;//A0.04
        private long syncCheckOverPoint = 0; //A0.04
        /// <summary>
        /// A0.04
        /// </summary>
        /// <returns></returns>
        private Boolean isOverCheckIntervalTime()
        {
            bool isOver = false;
            if (System.Threading.Interlocked.Exchange(ref syncCheckOverPoint, 1) == 0)
            {
                try
                {
                    if (preCheckTime.AddMilliseconds(intervalCheckSec * 1000) < DateTime.Now)
                    {
                        preCheckTime = DateTime.Now;
                        isOver = true;
                    }
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncCheckOverPoint, 0);
                }
            }
            return isOver;
        }

        public void checkValidation()
        {
            if (!isOverCheckIntervalTime()) return; //A0.04
            checkValidation(null);
        }



        //A0.01 Add Beging (2/2)
        private long syncCheckByEncryptDecryptPoint = 0;
        protected bool isValidationCheckByEncryptDecrypt()
        {
            if (System.Threading.Interlocked.Exchange(ref syncCheckByEncryptDecryptPoint, 1) == 0)
            {
                try
                {
                    bool checkResult = true;
                    string checkEncryptedString = string.Empty;
                    // sanity check
                    if ((hasp == null) || !hasp.IsLoggedIn())
                        return false;

                    checkEncryptedString = doEncrypt(encryptString);

                    if (checkEncryptedString == null || encryptedString == null) return false;

                    checkResult = checkEncryptedString.Trim().Equals(encryptedString.Trim(), StringComparison.OrdinalIgnoreCase);
                    Verbose(string.Format("Validation Check:[{0}] compare with [{1}],Result : {2}", checkEncryptedString, encryptedString, checkResult));
                    return checkResult;
                    //if (checkEncryptedString.Trim().Equals(encryptedString.Trim(), StringComparison.OrdinalIgnoreCase))
                    //{
                    //    Verbose(string.Format("Validation Check:[{0}] compare with [{1}],Result : {2}", checkEncryptedString, encryptedString, true));
                    //    return true;
                    //}
                    //else
                    //{
                    //    Verbose(string.Format("Validation Check:[{0}] compare with [{1}],Result : {2}", checkEncryptedString, encryptedString, false));
                    //    return false;
                    //}
                }
                finally
                {
                    System.Threading.Interlocked.Exchange(ref syncCheckByEncryptDecryptPoint, 0);
                }
            }
            return true;
        }

        protected string doEncrypt(string encryptObj)
        {
            byte[] data = UTF8Encoding.Default.GetBytes(encryptObj);
            HaspStatus status = hasp.Encrypt(data);
            ReportStatus("Encrypt", status);
            string encryptReturn = string.Empty;
            if (status == HaspStatus.StatusOk
                || status == HaspStatus.TerminalServiceDetected
                || status == HaspStatus.InvalidHandle)
            {
                return UTF8Encoding.Default.GetString(data);
            }
            else
            {
                return string.Empty;
            }
        }

        protected void ReportStatus(string action, HaspStatus status)
        {
            logger.Info(string.Format("SECSTrxControl, HASP Action:[{0}] ,HaspStatus:[{1}]",
                                             action,
                                             status.ToString()));
        }

        private void Verbose(string msg)
        {
            if (null == msg)
                return;
            logger.Info("SECSTrxControl," + msg);
        }
        //A0.01 Add End (2/2)

    }
}
