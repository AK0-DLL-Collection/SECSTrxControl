using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.Data.SecsData.Sample
{
    public class SECSConst
    {
        #region CRST
        public static readonly string CRST_Off_Line = "O";
        public static readonly string CRST_On_Line_Remote = "R";
        public static readonly string CRST_On_Line_Local = "L";
        public static readonly string[] CRST = 
        {
            CRST_Off_Line, 
            CRST_On_Line_Remote,
            CRST_On_Line_Local
        };
        #endregion CRST

        #region EQST
        public static readonly string EQST_IDLE = "I";
        public static readonly string EQST_RUN = "R";
        public static readonly string EQST_DOWN = "D";
        public static readonly string EQST_MAINTENANCE = "M";
        public static readonly string EQST_OTHER = "O";
        #endregion EQST

        #region TRSMODE
        public static readonly string TRSMODE_AUTO = "1";
        public static readonly string TRSMODE_Manual = "2";
        #endregion TRSMODE

        #region PPTYPE
        public static readonly string PPTYPE_Equipment = "E";
        public static readonly string PPTYPE_Unit = "U";
        public static readonly string PPTYPE_Sub_Unit = "S";
        #endregion PPTYPE

        #region ONLACK
        public static readonly string ONLACK_Accepted = "0";
        public static readonly string ONLACK_Not_Accepted = "1";
        public static readonly string ONLACK_Already_On_Line_Local = "2";
        public static readonly string ONLACK_Already_On_Line_Remote = "3";
        #endregion ONLACK

        #region SFCD
        public static readonly string SFCD_Equipment_Status_Request = "1";
        public static readonly string SFCD_Port_Status_Request = "2";
        public static readonly string SFCD_Operation_Mode_Request = "3";
        public static readonly string SFCD_Unit_Status_Request = "4";
        public static readonly string SFCD_Sub_Unit_Status_Request = "5";
        public static readonly string SFCD_Mask_Status_Request = "6";
        public static readonly string SFCD_Material_Status_Request = "7";
        public static readonly string SFCD_Sorter_Job_List_Request = "8";
        public static readonly string SFCD_Crate_Port_Status_Request = "9";
        #endregion SFCD

        #region CIACK
        public static readonly string CIACK_Accepted = "0";
        public static readonly string CIACK_Busy = "1";
        public static readonly string CIACK_CSTID_is_Invalid = "2";
        public static readonly string CIACK_PPID_is_Invalid = "3";
        public static readonly string CIACK_SLOT_Information_Mismatch = "4";
        public static readonly string CIACK_Already_Received = "5";
        public static readonly string CIACK_PAIR_LOT_Mismatch = "6";
        public static readonly string CIACK_PROD_ID_Invalid = "7";
        public static readonly string CIACK_Glass_Type_Invalid = "8";
        public static readonly string CIACK_Other_Error = "9";
        #endregion CIACK

        #region RCMD
        //RCMD Begin
        //Start, Cancel, Abort, Pause, Resume
        public static readonly string RCMD_Start = "1";
        public static readonly string RCMD_Cancel = "2";
        public static readonly string RCMD_Abort = "3";
        public static readonly string RCMD_Pause = "4";
        public static readonly string RCMD_Resume = "5";
        //Operator Call
        public static readonly string RCMD_Operator_Call = "6";
        //Mask CST Cancel
        public static readonly string RCMD_Mask_CST_Cancel = "7";
        //RCMD End
        #endregion RCMD

        #region HCACK
        public static readonly string HCACK_Accepted = "0";
        public static readonly string HCACK_Error = "1";
        #endregion HCACK

        #region ACK
        public static readonly string ACK_Accepted = "0";
        public static readonly string ACK_Not_Accepted = "1";
        #endregion ACK

        #region CEID
        /// <summary>
        /// 用來代表所有的CEID（於Enable、Disable All CEID時會使用到）。
        /// </summary>
        public static readonly string CEID_ALL_CEID = "000";
        //CEID Remark Begin
        public static readonly string CEID_Oper_Mode_Status_Change = "104";
        public static readonly string CEID_Unit_Status_Change = "105";
        public static readonly string CEID_Sub_Unit_Status_Change = "106";
        //Material State Change
        public static readonly string CEID_Material_State_Change = "108";
        //Equipment Constant Change
        public static readonly string CEID_Equipment_Constant_Change = "109";
        //Ready To Start
        public static readonly string CEID_Ready_To_Start = "110";
        //Control State Change & Equipment Status Change
        public static readonly string CEID_Control_State_Change_OFF_LINE = "111";
        public static readonly string CEID_Control_State_Change_ON_LINE_LOCAL = "112";
        public static readonly string CEID_Control_State_Change_ON_LINE_REMOTE = "113";
        public static readonly string CEID_Equipment_Status_Change = "114";
        //Operator Confirm Event about Operator Call Command
        public static readonly string CEID_Operator_Confirm = "118";
        //Port Status & Transfer Mode Change (CEID 200 ~ 209)
        public static readonly string CEID_Load_Request = "200";
        public static readonly string CEID_Pre_Load_Complete = "201";
        public static readonly string CEID_Load_Complete = "202";
        public static readonly string CEID_Unload_Request = "203";
        public static readonly string CEID_Unload_Complete = "204";
        public static readonly string CEID_Port_Diable_Changed = "205";
        public static readonly string CEID_Port_Enable_Changed = "206";
        public static readonly string CEID_Port_Type_Changed = "207";
        public static readonly string CEID_Port_Use_Type_Changed = "208";
        public static readonly string CEID_Transfer_Mode_Changed = "209";
        //Crate Port Status & Transfer Mode Change (CEID 210 ~ 219)
        public static readonly string CEID_Crate_Port_Load_Request = "210";
        public static readonly string CEID_Remained_Glass_Count_of_Crate_Report = "211";
        public static readonly string CEID_Crate_Port_Load_Complete = "212";
        public static readonly string CEID_Crate_Port_Unload_Request = "213";
        public static readonly string CEID_Crate_Port_Unload_Complete = "214";
        public static readonly string CEID_Crate_Port_Port_Disabled = "215";        //The Port must be empty before the event happens
        public static readonly string CEID_Crate_Port_Port_Enabled = "216";         //The port must be empty before the event happens
        public static readonly string CEID_Crate_Port_Type_Changed = "217";
        public static readonly string CEID_Crate_Port_Use_Type_Change = "218";
        public static readonly string CEID_Crate_Port_Transfer_Mode_Change = "219"; //AGV or MGV / Use of Port Type
        //Mask Cassette Port Status & Transfer Mode Change (CEID 220 ~ 229)
        public static readonly string CEID_Mask_Cassette_Port_Load_Request = "220";
        public static readonly string CEID_Mask_Cassette_Port_Pre_Load_Complete = "221";
        public static readonly string CEID_Mask_Cassette_Port_Load_Complete = "222";
        public static readonly string CEID_Mask_Cassette_Port_Unload_Request = "223";
        public static readonly string CEID_Mask_Cassette_Port_Unload_Complete = "224";
        public static readonly string CEID_Mask_Cassette_Port_Disable_Changed = "225";
        public static readonly string CEID_Mask_Cassette_Port_Enable_Changed = "226";
        public static readonly string CEID_Mask_Cassette_Port_Type_Changed = "227";
        public static readonly string CEID_Mask_Cassette_Port_Use_Type_Changed = "228";
        public static readonly string CEID_Mask_Cassette_Port_Transfer_Mode_Changed = "229";
        //Process Status (CEID 301 ~ 305)
        public static readonly string CEID_Process_Start = "301";
        public static readonly string CEID_Process_Cancel = "304";
        public static readonly string CEID_Process_Abort = "305";
        public static readonly string CEID_Process_Pause = "306";
        public static readonly string CEID_Process_Resume = "307";
        //Last Glass Process Start
        public static readonly string CEID_Last_Glass_Process_Start = "311";
        //Glass Out/In
        public static readonly string CEID_Glass_Out_By_Indexer_Port = "321";
        public static readonly string CEID_Glass_In_By_Indexer_Port = "322";
        public static readonly string CEID_Glass_Out_By_Unit = "323";
        public static readonly string CEID_Glass_In_By_Unit = "324";
        public static readonly string CEID_Glass_Out_By_Sub_Unit = "325";
        public static readonly string CEID_Glass_In_By_Sub_Unit = "326";
        //Cassette Out/In Unit
        public static readonly string CEID_Cassette_In_By_Unit = "327";
        public static readonly string CEID_Cassette_Out_By_Unit = "328";
        //Glass Scrap / Unscrap
        public static readonly string CEID_Glass_Scrap = "331";
        public static readonly string CEID_Glass_UnScrap = "332";
        //Glass Turn
        public static readonly string CEID_Glass_Turn = "334";
        //Mask Out/In (Evaporation, Mask AMHS EQ only)
        public static readonly string CEID_Mask_Out_By_Indexer = "341";
        public static readonly string CEID_Mask_In_By_Indexer = "342";
        public static readonly string CEID_Mask_Out_By_Unit = "343";
        public static readonly string CEID_Mask_In_By_Unit = "344";
        public static readonly string CEID_Mask_Out_By_Sub_Unit = "345";
        public static readonly string CEID_Mask_In_By_Sub_Unit = "346";
        public static readonly string CEID_Mask_In_Line = "347";
        public static readonly string CEID_Mask_Out_Line = "348";
        //Process Program or Recipe Change
        public static readonly string CEID_Proc_Prog_or_Recipe_Change = "401";
        //Assemble Complete Event
        public static readonly string CEID_Assemble_Complete_Event = "411";
        //Glass Cut Process
        public static readonly string CEID_Glass_Cut_Process = "412";
        //Sorting Job Event
        public static readonly string CEID_Sorting_Job_Process_Start = "431";
        public static readonly string CEID_Sorting_Job_Process_End = "432";
        public static readonly string CEID_Sorting_Job_Cancel_Begin = "433";
        public static readonly string CEID_Sorting_Job_Cancel_End = "434";
        public static readonly string CEID_Sorting_Job_Abort_Begin = "435";
        public static readonly string CEID_Sorting_Job_Abort_End = "436";
        //Process Data
        public static readonly string CEID_Glass_Process_Data = "500";
        public static readonly string CEID_Lot_Process_Data = "501";
        //CEID Remark End
        #region CEID Array
        public static readonly string[] CEID_ARRAY = 
        {
            CEID_Oper_Mode_Status_Change,
              CEID_Oper_Mode_Status_Change,
          CEID_Unit_Status_Change,
          CEID_Sub_Unit_Status_Change,
        //Material State Change
          CEID_Material_State_Change,
        //Equipment Constant Change
          CEID_Equipment_Constant_Change,
        //Ready To Start
          CEID_Ready_To_Start,
        //Control State Change & Equipment Status Change
          CEID_Control_State_Change_OFF_LINE,
          CEID_Control_State_Change_ON_LINE_LOCAL,
          CEID_Control_State_Change_ON_LINE_REMOTE,
          CEID_Equipment_Status_Change,
        //Operator Confirm Event about Operator Call Command
          CEID_Operator_Confirm,
        //Port Status & Transfer Mode Change (CEID 200 ~ 209)
          CEID_Load_Request,
          CEID_Pre_Load_Complete,
          CEID_Load_Complete,
          CEID_Unload_Request,
          CEID_Unload_Complete,
          CEID_Port_Diable_Changed,
          CEID_Port_Enable_Changed,
          CEID_Port_Type_Changed,
          CEID_Port_Use_Type_Changed,
          CEID_Transfer_Mode_Changed ,
        //Crate Port Status & Transfer Mode Change (CEID 210 ~ 219)
          CEID_Crate_Port_Load_Request,
          CEID_Remained_Glass_Count_of_Crate_Report ,
          CEID_Crate_Port_Load_Complete,
          CEID_Crate_Port_Unload_Request,
          CEID_Crate_Port_Unload_Complete,
          CEID_Crate_Port_Port_Disabled,        //The Port must be empty before the event happens
          CEID_Crate_Port_Port_Enabled,        //The port must be empty before the event happens
          CEID_Crate_Port_Type_Changed,
          CEID_Crate_Port_Use_Type_Change ,
          CEID_Crate_Port_Transfer_Mode_Change,//AGV or MGV / Use of Port Type
        //Mask Cassette Port Status & Transfer Mode Change (CEID 220 ~ 229)
          CEID_Mask_Cassette_Port_Load_Request,
          CEID_Mask_Cassette_Port_Pre_Load_Complete,
          CEID_Mask_Cassette_Port_Load_Complete,
          CEID_Mask_Cassette_Port_Unload_Request,
          CEID_Mask_Cassette_Port_Unload_Complete,
          CEID_Mask_Cassette_Port_Disable_Changed,
          CEID_Mask_Cassette_Port_Enable_Changed ,
          CEID_Mask_Cassette_Port_Type_Changed ,
          CEID_Mask_Cassette_Port_Use_Type_Changed ,
          CEID_Mask_Cassette_Port_Transfer_Mode_Changed,
        //Process Status (CEID 301 ~ 305)
          CEID_Process_Start,
          CEID_Process_Cancel,
          CEID_Process_Abort,
          CEID_Process_Pause,
          CEID_Process_Resume ,
        //Last Glass Process Start
          CEID_Last_Glass_Process_Start,
        //Glass Out/In
          CEID_Glass_Out_By_Indexer_Port,
          CEID_Glass_In_By_Indexer_Port,
          CEID_Glass_Out_By_Unit,
          CEID_Glass_In_By_Unit ,
          CEID_Glass_Out_By_Sub_Unit ,
          CEID_Glass_In_By_Sub_Unit,
        //Cassette Out/In Unit
          CEID_Cassette_In_By_Unit,
          CEID_Cassette_Out_By_Unit,
        //Glass Scrap / Unscrap
          CEID_Glass_Scrap,
          CEID_Glass_UnScrap,
        //Glass Turn
          CEID_Glass_Turn,
        //Mask Out/In (Evaporation, Mask AMHS EQ only)
          CEID_Mask_Out_By_Indexer,
          CEID_Mask_In_By_Indexer,
          CEID_Mask_Out_By_Unit,
          CEID_Mask_In_By_Unit,
          CEID_Mask_Out_By_Sub_Unit,
          CEID_Mask_In_By_Sub_Unit ,
          CEID_Mask_In_Line,
          CEID_Mask_Out_Line,
        //Process Program or Recipe Change
          CEID_Proc_Prog_or_Recipe_Change,
        //Assemble Complete Event
          CEID_Assemble_Complete_Event,
        //Glass Cut Process
          CEID_Glass_Cut_Process,
        //Sorting Job Event
          CEID_Sorting_Job_Process_Start,
          CEID_Sorting_Job_Process_End,
          CEID_Sorting_Job_Cancel_Begin,
          CEID_Sorting_Job_Cancel_End,
          CEID_Sorting_Job_Abort_Begin ,
          CEID_Sorting_Job_Abort_End,
        //Process Data
          CEID_Glass_Process_Data,
          CEID_Lot_Process_Data
        };
        #endregion CEID Array
        #endregion CEID

        #region ACKC6
        public static readonly string ACKC6_Accepted = "0";
        public static readonly string ACKC6_NotAccepted = "1";
        #endregion ACKC6

        #region TIACK
        public static readonly string TIACK_Accepted = "0";
        public static readonly string TIACK_Error_not_done = "1";
        #endregion TIACK

        #region OFLACK
        public static readonly string OFLACK_Accepted = "0";
        public static readonly string OFLACK_Not_Accepted = "1";
        #endregion OFLACK

        #region EAC
        public static readonly string EAC_Accept = "0";
        public static readonly string EAC_Denied_At_Least_one_constant_does_not_exist = "1";
        public static readonly string EAC_Denied_Busy = "2";
        public static readonly string EAC_Denied_At_least_one_constant_out_of_range = "3";
        public static readonly string EAC_Other_equipment_specific_error = "4";
        #endregion EAC

        #region TIAACK
        public static readonly string TIAACK_Everything_correct = "0";
        public static readonly string TIAACK_Too_many_SVIDs = "1";
        public static readonly string TIAACK_No_more_traces_allowed = "2";
        public static readonly string TIAACK_Invalid_period = "3";
        public static readonly string TIAACK_Equipment_specified_error = "4";
        #endregion TIAACK

        #region ERACK
        public static readonly string ERACK_Accepted = "0";
        public static readonly string ERACK_Denied_At_least_one_CEID_dose_not_exist = "1";
        public static readonly string ERACK_Other_Errors = "2";
        #endregion ERACK

        #region ACKC5
        public static readonly string ACKC5_Accepted = "0";
        public static readonly string ACKC5_Not_Accepted = "1";
        public static readonly string ACKC5_Not_Exist_ALID = "2";
        #endregion ACKC5

        #region ACKC7
        public static readonly string ACKC7_Accepted = "0";
        public static readonly string ACKC7_Not_Accepted = "1";
        public static readonly string ACKC7_Unit_ID_is_not_exist = "2";
        public static readonly string ACKC7_PPTYPE_is_not_match = "3";
        public static readonly string ACKC7_PPID_is_not_match = "4";
        #endregion ACKC7

        #region ACKC10
        public static readonly string ACKC10_Accepted = "0";
        public static readonly string ACKC10_Not_Accepted = "1";
        #endregion ACKC10

        #region CEED
        public static readonly string CEED_Enable = "0";
        public static readonly string CEED_Disable = "1";
        #endregion CEED

        #region ALED
        public static readonly string ALED_Enable = "0";
        public static readonly string ALED_Disable = "1";
        #endregion ALED

        #region PPCINFO
        /// <summary>
        /// A new PPID is created and registered
        /// </summary>
        public static readonly string PPCINFO_Created = "1";
        /// <summary>
        /// Some parameters of a PPID are modified
        /// </summary>
        public static readonly string PPCINFO_Modified = "2";
        /// <summary>
        /// Any PPID is deleted
        /// </summary>
        public static readonly string PPCINFO_Deleted = "3";
        /// <summary>
        /// Equipment sets up any PPID which different from current PPID
        /// </summary>
        public static readonly string PPCINFO_Changed = "4";
        #endregion PPCINFO

        #region ALST
        public static readonly string ALST_SET = "1";
        public static readonly string ALST_CLEAR = "2";
        #endregion ALST

        #region ALCD
        public static readonly string ALCD_Light_Alarm = "1";
        public static readonly string ALCD_Serious_Alarm = "2";
        #endregion ALCD

        //#region PPTYPE
        //public static readonly string PPTYPE_Equipment = "E";
        //public static readonly string PPTYPE_Unit = "U";
        //public static readonly string PPTYPE_SubUnit = "S";
        //#endregion PPTYPE
    }
}
