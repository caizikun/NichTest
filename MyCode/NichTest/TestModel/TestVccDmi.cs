using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class TestVccDmi: ITest
    {
        public Dictionary<string, double> BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            try
            {
                PowerSupply supply = (PowerSupply)equipments["POWERSUPPLY"];
                Log.SaveLogToTxt("Start to do DMI_VCC test");
                Log.SaveLogToTxt("Try to get woltage of powersupply");
                double voltage = supply.GetVoltage();
                Log.SaveLogToTxt("Get VCC is " + voltage.ToString("f3"));
                Log.SaveLogToTxt("Try to read DmiVcc of DUT");
                double delta = dut.ReadDmiVcc() - ConditionParaByTestPlan.VCC;
                Log.SaveLogToTxt("Calculate delta of VCC is " + delta.ToString("f3"));
                Log.SaveLogToTxt("End DMI_VCC test" + "\r\n");

                //save testdata
                Dictionary<string, double> dictionary = new Dictionary<string, double>();
                dictionary.Add("DmiVccErr", delta);
                dictionary.Add("Result", 1);
                return dictionary;
            }
            catch
            {
                Log.SaveLogToTxt("Failed DMI_VCC test.");
                return null;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
