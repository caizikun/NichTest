using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NichTest
{
    public struct ConditionParaByTestPlan
    {
        public static int ID;
        public static string SEQ;
        public static byte Channel;
        public static double Temp;
        public static double LastTemp = Algorithm.MyNaN;
        public static double VCC;
        public static string ItemName;
        public static int CtrlType;
        public static int TempWaitingTimes;
        public static double TempOffset;

        public static void SetValue(DataRow dr)
        {
            ID = Convert.ToInt32(dr["ID"]);// 记录当前ConditionID号码
            SEQ= dr["SEQ"].ToString();
            Channel = Convert.ToByte(dr["Channel"]);
            Temp = Convert.ToDouble(dr["Temp"]);
            VCC = Convert.ToDouble(dr["Vcc"]);
            ItemName = dr["ItemName"].ToString();
            CtrlType = Convert.ToInt32(dr["CtrlType"]);
            TempWaitingTimes = Convert.ToInt32(dr["TempWaitTimes"]);
            TempOffset = Convert.ToDouble(dr["TempOffset"]);
        }
    }
}
