using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    public struct GlobalParaByPN
    {
        public static int ID;
        public static int PID;
        public static string PN;//品名
        public static string ItemName;//描述
        public static byte TotalChCount;//总通道数
        public static byte TotalVccCount;//总电压数
        public static byte TotalTempCount;//总温度数，传感器类型
        public static int MCoefsID;//系数组
        public static bool IgnoreFlag;//
        public static bool isOldDriver;//new or old
        public static byte TecPresent;//是否存在TEC
        public static byte CoupleType;//耦合类型
        public static byte APCType;//APC类型
        public static byte BER_EXP;//误码率        
        public static byte MaxRate;//最大速率
        public static string Publish_PN;//正式品名
        public static string NickName;//别名
        public static string IbiasFormula;//Ibias公式
        public static string ImodFormula;//Imod公式
        public static bool UsingCelsiusTemp;//是否用摄氏温度
        public static float OverLoadPoint;//接收饱和入射光
        public static string OpticalSourceERArray;//OpticalSourceER

        //add
        public static string Station;//当前站点
        public static string Family;

        public static void SetValue(DataTable dataTable_PN, string selectedProductPN)
        {
            DataRow[] dr = dataTable_PN.Select("PN='" + selectedProductPN + "'");
            ID = Convert.ToInt32(dr[0]["ID"]);
            PID = Convert.ToInt32(dr[0]["PID"]);
            PN = dr[0]["PN"].ToString();
            ItemName = dr[0]["ItemName"].ToString();
            TotalChCount = Convert.ToByte(dr[0]["Channels"]);
            TotalVccCount = Convert.ToByte(dr[0]["Voltages"]);
            TotalTempCount = Convert.ToByte(dr[0]["Tsensors"]);
            MCoefsID = Convert.ToInt32(dr[0]["MCoefsID"]);
            IgnoreFlag = Convert.ToBoolean(dr[0]["IgnoreFlag"]);
            isOldDriver = (Convert.ToByte(dr[0]["OldDriver"]) == 0);
            TecPresent = Convert.ToByte(dr[0]["TEC_Present"]);
            CoupleType = Convert.ToByte(dr[0]["Couple_Type"]);
            APCType = Convert.ToByte(dr[0]["APC_Type"]);
            BER_EXP = Convert.ToByte(dr[0]["BER"]);
            MaxRate = Convert.ToByte(dr[0]["MaxRate"]);
            Publish_PN = dr[0]["Publish_PN"].ToString();
            NickName = dr[0]["NickName"].ToString();
            IbiasFormula = dr[0]["IbiasFormula"].ToString();
            ImodFormula = dr[0]["IModFormula"].ToString();
            UsingCelsiusTemp = Convert.ToBoolean(dr[0]["UsingCelsiusTemp"]);
            OverLoadPoint = Convert.ToSingle(dr[0]["RxOverLoaddBm"]);            
        }
        //public static double CurrentVcc;// 当前的电压值

        //public static double CurrentTemp;//当前的实际温度（C）  

        //public static byte CurrentChannel; //当前通道，添加到相应的数组下标  

        //public static String CurrentSN;        

        //public static Int32 IbiasRegistDacValueMax;

        //public static Int32 ImodRegistDacValueMax;

        //public static string StrEvbCurrent;        

        //public string StrPathOEyeDiagram;//眼图存放路径

        //public string StrPathEEyeDiagram;//眼图存放路径

        //public string StrPathPolarityEyeDiagram;//极性眼图存放路径
    }
}
