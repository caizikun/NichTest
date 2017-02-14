using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class TestIcc: ITest
    {
        public Dictionary<string, double> BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            try
            {
                PowerSupply supply = (PowerSupply)equipments["POWERSUPPLY"];
                Log.SaveLogToTxt("Start to do DMI_ICC test");
                Log.SaveLogToTxt("Try to get current of powersupply");
                double current = supply.GetCurrent();
                Log.SaveLogToTxt("Get ICC is " + current.ToString("f3"));
                Log.SaveLogToTxt("End DMI_ICC test" + "\r\n");
                //save testdata
                Dictionary<string, double> dictionary = new Dictionary<string, double>();
                dictionary.Add("Current", current);
                dictionary.Add("Result", 1);
                return dictionary;
            }
            catch
            {
                Log.SaveLogToTxt("Failed DMI_ICC test.");
                return null;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
