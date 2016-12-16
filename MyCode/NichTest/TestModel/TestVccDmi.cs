using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class TestVccDmi: ITest
    {
        public bool BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
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
                return true;
            }
            catch
            {
                Log.SaveLogToTxt("Failed DMI_VCC test.");
                return false;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
