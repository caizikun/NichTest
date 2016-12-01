using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    public class TestIcc: ITest
    {
        public bool BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            PowerSupply supply = (PowerSupply)equipments["POWERSUPPLY"];
            Log.SaveLogToTxt("Start to do DMI_ICC test");
            Log.SaveLogToTxt("Try to get current of powersupply");
            double current = supply.GetCurrent();            
            Log.SaveLogToTxt("Get ICC is " + current.ToString("f3"));                
            Log.SaveLogToTxt("End DMI_ICC test");
            return true;
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
