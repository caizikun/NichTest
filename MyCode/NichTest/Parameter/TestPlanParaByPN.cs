using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    public struct TestPlanParaByPN
    {
        public static int ID;
        public static int PID;
        public static string ItemName;//名称
        public static string FwVersion;//软件版本号
        public static string HwVersion;// 硬件版本号
        public static byte DUT_USB_Port;//USB端口
        public static bool IsInitialChip;//是否初始化芯片
        public static bool IsInitialEEPROM;//是否初始化EEPROM
        public static bool IgnoreRecordCoef;//是否不备份系数
        public static bool IsCheckSN;//是否检查SN
        public static bool IsCheckFW;//是否检查FW版本号
        public static bool IsCheckPN;//是否检查品名
        public static bool IsSkip;//是否跳过TestPlan
        public static string Description;//描述
        public static int Version;//修订号
        public static string SN;

        public static void SetValue(DataTable dataTable_TestPlan)
        {
            DataRow dr = dataTable_TestPlan.Rows[0];

            ID = Convert.ToInt32(dr["ID"]);
            PID = Convert.ToInt32(dr["PID"]);
            ItemName = dr["ItemName"].ToString();
            FwVersion = dr["SWVersion"].ToString();
            HwVersion = dr["HWVersion"].ToString();
            DUT_USB_Port = Convert.ToByte(dr["USBPort"]);
            IsInitialChip = Convert.ToBoolean(dr["IsChipInitialize"]);
            IsInitialEEPROM = Convert.ToBoolean(dr["IsEEPROMInitialize"]);
            IgnoreRecordCoef = Convert.ToBoolean(dr["IgnoreBackupCoef"]);
            IsCheckSN = Convert.ToBoolean(dr["SNCheck"]);
            IsCheckFW = Convert.ToBoolean(dr["SWCheck"]);
            IsCheckPN = Convert.ToBoolean(dr["PNCheck"]);
            IsSkip = Convert.ToBoolean(dr["IgnoreFlag"]);
            Description = dr["ItemDescription"].ToString();
            Version = Convert.ToInt32(dr["Version"]);           
        }
    }
}
