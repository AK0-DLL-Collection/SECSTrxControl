//*********************************************************************************
//      SecsElement.cs
//*********************************************************************************
// File Name: SecsElement.cs
// Description: SECS Element Attribute
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

namespace com.mirle.ibg3k0.stc.Common
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class SecsElement : System.Attribute
    {
        public SecsElementType Type { get; set; }
        public int Length{get;set;}
        public int Index { get; set; }
        public SecsElementType ListElementType { get; set; }
        public int ListElementLength { get; set; }
        public Boolean ListSpreadOut { get; set; }
        public Boolean ValidateFormat { get; set; }
//        public Boolean FixValue { get; set; }

        public SecsElement() 
        {
            ListSpreadOut = false;
            ValidateFormat = true;
//            FixValue = false;
        }

        public enum SecsElementType 
        {
            TYPE_ASCII = 1,
            TYPE_JIS8 = 2,
            TYPE_BINARY = 3,
            TYPE_1_BYTE_SIGNED_INTEGER = 4,
            TYPE_2_BYTE_SIGNED_INTEGER = 5,
            TYPE_4_BYTE_SIGNED_INTEGER = 6,
            TYPE_1_BYTE_UNSIGNED_INTEGER = 7,
            TYPE_2_BYTE_UNSIGNED_INTEGER = 8,
            TYPE_4_BYTE_UNSIGNED_INTEGER = 9,
            TYPE_4_BYTE_FLOATING_POINT_NUMBER = 10,
            TYPE_8_BYTE_FLOATING_POINT_NUMBER = 11,
            TYPE_BOOLEAN = 12,
            TYPE_LIST = 13,
            NOT_USED = 14                           //Not Secs Element
        }

        public static string converToTypeID(SecsElementType type) 
        {
            switch (type) 
            {
                case SecsElementType.TYPE_ASCII:
                    return SecsElementTypeID.TYPE_ASCII;
                case SecsElementType.TYPE_JIS8:
                    return SecsElementTypeID.TYPE_JIS8;
                case SecsElementType.TYPE_BINARY:
                    return SecsElementTypeID.TYPE_BINARY;
                case SecsElementType.TYPE_1_BYTE_SIGNED_INTEGER:
                    return SecsElementTypeID.TYPE_1_BYTE_SIGNED_INTEGER;
                case SecsElementType.TYPE_2_BYTE_SIGNED_INTEGER:
                    return SecsElementTypeID.TYPE_2_BYTE_SIGNED_INTEGER;
                case SecsElementType.TYPE_4_BYTE_SIGNED_INTEGER:
                    return SecsElementTypeID.TYPE_4_BYTE_SIGNED_INTEGER;
                case SecsElementType.TYPE_1_BYTE_UNSIGNED_INTEGER:
                    return SecsElementTypeID.TYPE_1_BYTE_UNSIGNED_INTEGER;
                case SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER:
                    return SecsElementTypeID.TYPE_2_BYTE_UNSIGNED_INTEGER;
                case SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER:
                    return SecsElementTypeID.TYPE_4_BYTE_UNSIGNED_INTEGER;
                case SecsElementType.TYPE_4_BYTE_FLOATING_POINT_NUMBER:
                    return SecsElementTypeID.TYPE_4_BYTE_FLOATING_POINT_NUMBER;
                case SecsElementType.TYPE_8_BYTE_FLOATING_POINT_NUMBER:
                    return SecsElementTypeID.TYPE_8_BYTE_FLOATING_POINT_NUMBER;
                case SecsElementType.TYPE_BOOLEAN:
                    return SecsElementTypeID.TYPE_BOOLEAN;
                case SecsElementType.TYPE_LIST:
                    return SecsElementTypeID.TYPE_LIST;
                default:
                    return SecsElementTypeID.TYPE_ASCII;
            }
        }

        public static QSACTIVEXLib.SECSII_DATA_TYPE convertToSECSIIDataType(SecsElementType type) 
        {
            switch (type)
            {
                case SecsElementType.TYPE_ASCII:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
                case SecsElementType.TYPE_JIS8:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.JIS_TYPE;
                case SecsElementType.TYPE_BINARY:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.BINARY_TYPE;
                case SecsElementType.TYPE_1_BYTE_SIGNED_INTEGER:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.INT_1_TYPE;
                case SecsElementType.TYPE_2_BYTE_SIGNED_INTEGER:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.INT_2_TYPE;
                case SecsElementType.TYPE_4_BYTE_SIGNED_INTEGER:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.INT_4_TYPE;
                case SecsElementType.TYPE_1_BYTE_UNSIGNED_INTEGER:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.UINT_1_TYPE;
                case SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.UINT_2_TYPE;
                case SecsElementType.TYPE_4_BYTE_UNSIGNED_INTEGER:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.UINT_4_TYPE;
                case SecsElementType.TYPE_4_BYTE_FLOATING_POINT_NUMBER:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.FT_4_TYPE;
                case SecsElementType.TYPE_8_BYTE_FLOATING_POINT_NUMBER:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.FT_8_TYPE;
                case SecsElementType.TYPE_BOOLEAN:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.BOOLEAN_TYPE;
                case SecsElementType.TYPE_LIST:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.LIST_TYPE;
                default:
                    return QSACTIVEXLib.SECSII_DATA_TYPE.ASCII_TYPE;
            }
        }

    }

    public static class SecsElementTypeID
    {
        public static readonly string TYPE_ASCII = "A";
        public static readonly string TYPE_JIS8 = "J";
        public static readonly string TYPE_BINARY = "B";
        public static readonly string TYPE_1_BYTE_SIGNED_INTEGER = "I1";
        public static readonly string TYPE_2_BYTE_SIGNED_INTEGER = "I2";
        public static readonly string TYPE_4_BYTE_SIGNED_INTEGER = "I4";
        public static readonly string TYPE_1_BYTE_UNSIGNED_INTEGER = "U1";
        public static readonly string TYPE_2_BYTE_UNSIGNED_INTEGER = "U2";
        public static readonly string TYPE_4_BYTE_UNSIGNED_INTEGER = "U4";
        public static readonly string TYPE_4_BYTE_FLOATING_POINT_NUMBER = "F4";
        public static readonly string TYPE_8_BYTE_FLOATING_POINT_NUMBER = "F8";
        public static readonly string TYPE_BOOLEAN = "Boolean";
        public static readonly string TYPE_LIST = "L";
    }
}
