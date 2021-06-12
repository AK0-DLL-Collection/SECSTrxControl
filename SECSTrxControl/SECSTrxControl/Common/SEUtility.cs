//*********************************************************************************
//      SEUtility.cs
//*********************************************************************************
// File Name: SEUtility.cs
// Description: Common Function Utility 
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Data.SecsData;
using System.Diagnostics;

namespace com.mirle.ibg3k0.stc.Common
{
    public class SEUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SystemEventLog(string msg, EventLogEntryType type)
        {
            try
            {
                string src_name = "SECS Agent";
                if (!EventLog.SourceExists(src_name))
                {
                    EventLog.CreateEventSource(src_name, src_name);
                }
                EventLog eLog = new EventLog();
                eLog.Source = src_name;
                eLog.WriteEntry(msg, type);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }

        public static Boolean isSXFYType(Type type)
        {
            if (type == null)
            {
                return false;
            }
            Type baseType = type;
            while (baseType != null)
            {
                if (baseType == typeof(SXFY))
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }

        public static object GetDefault(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
            return null;
        }

        public static Boolean isListSpreadOutSXFY(Type type)
        {
            FieldInfo[] fields = SEUtility.GetSeceElementFields(type);
            if (fields.Length > 1)
            {
                return false;
            }
            foreach (FieldInfo field in fields)
            {
                SecsElement attr = getSecsElementAttr(field);
                if (attr == null) { continue; }
                if (attr.ListSpreadOut)
                {
                    return true;
                }
            }
            return false;
        }

        public static FieldInfo[] GetSeceElementFields(Type type)
        {
            List<FieldInfo> rtnList = new List<FieldInfo>();
            FieldInfo[] tmpFieldAry = null;
            tmpFieldAry = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

            List<KeyValuePair<int, FieldInfo>> fieldList = new List<KeyValuePair<int, FieldInfo>>();
            foreach (FieldInfo field in tmpFieldAry)
            {
                if (field.FieldType.IsInterface) { continue; }
                if (field.FieldType.IsArray && field.FieldType.GetElementType().BaseType == typeof(SXFY))
                {
                    SecsElement attr = getSecsElementAttr(field);
                    if (attr != null && attr.Type == SecsElement.SecsElementType.NOT_USED) { continue; }
                    KeyValuePair<int, FieldInfo> pair =
                        new KeyValuePair<int, FieldInfo>((attr == null ? 0 : attr.Index), field);
                    fieldList.Add(pair);
                }
                else if (field.FieldType.BaseType == typeof(SXFY))
                {
                    SecsElement attr = getSecsElementAttr(field);
                    if (attr != null && attr.Type == SecsElement.SecsElementType.NOT_USED) { continue; }
                    KeyValuePair<int, FieldInfo> pair =
                        new KeyValuePair<int, FieldInfo>((attr == null ? 0 : attr.Index), field);
                    fieldList.Add(pair);
                }
                else
                {
                    SecsElement attr = getSecsElementAttr(field);
                    if (attr == null || attr.Type == SecsElement.SecsElementType.NOT_USED) { continue; }
                    KeyValuePair<int, FieldInfo> pair = new KeyValuePair<int, FieldInfo>(attr.Index, field);
                    fieldList.Add(pair);
                }
            }
            fieldList = fieldList.OrderBy(o => o.Key).ToList();
            foreach (KeyValuePair<int, FieldInfo> pair in fieldList)
            {
                rtnList.Add(pair.Value);
            }
            return rtnList.ToArray();
        }

        public static SecsElement getSecsElementAttr(FieldInfo fieldInfo)
        {
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fieldInfo);
            SecsElement attr = null;
            foreach (System.Attribute a in attrs)
            {
                if (a is SecsElement)
                {
                    attr = (SecsElement)a;
                    break;
                }
            }
            return attr;
        }

        //private static Dictionary<string, SecsElement> td = new Dictionary<string, SecsElement>();
        //public static SecsElement getSecsElementAttr(FieldInfo fieldInfo)
        //{

        //    SecsElement attr = null;
        //    string s = fieldInfo.DeclaringType.ToString();

        //    if (td.ContainsKey(s))
        //    {
        //        attr = td[s];
        //    }
        //    else
        //    {
        //        System.Attribute[] attrs = System.Attribute.GetCustomAttributes(fieldInfo, false);
        //        foreach (System.Attribute a in attrs)
        //        {
        //            if (a is SecsElement)
        //            {

        //                attr = (SecsElement)a;
        //                td.Add(s, attr);
        //                break;
        //            }
        //        }
        //    }

        //    return attr;
        //}


        public static string rawData2String(Object rawData)
        {
            Byte[] ary = rawData as Byte[];
            StringBuilder sb = new StringBuilder();
            foreach (Byte val in ary)
            {
                sb.Append(val).Append(", ");
            }
            return sb.ToString();
        }

        public static void showRawData(Object rawData)
        {
            logger.Info("*****************************RawData:{0}", rawData2String(rawData));
        }

        public static string binary2String(int[] binary)
        {
            string bStr = "";
            for (int intIndex = 0; intIndex <= (binary.Length - 1); intIndex++)
            {
                bStr = bStr + " 0x" + Convert.ToString(binary[intIndex], 16).ToUpper();
            }
            return bStr;
        }

    }
}
