using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    class TestIBiasDmi : ITest
    {
        public bool BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            //get the current test channel
            int channel = ConditionParaByTestPlan.Channel;
            try
            {
                //have to lock this object, due to it can't run parallel test with TestTxPowerDmi
                //as TestTxPowerDmi will disable TxPower, ibiasDmi will be defected.
                lock (dut)
                {
                    Log.SaveLogToTxt("Start to do IBiasDMI test for channel " + channel);
                    // open apc
                    Log.SaveLogToTxt("Close apc for module.");
                    dut.CloseAndOpenAPC(Convert.ToByte(DUT.APCMODE.IBAISandIMODON));
                    // read IBiasDMI
                    Log.SaveLogToTxt("Try to readDmiBias of DUT");
                    double ibiasDmi = dut.ReadDmiBias(channel);
                    Log.SaveLogToTxt("Read IBiasDMI is " + ibiasDmi.ToString("f3"));
                    Log.SaveLogToTxt("End DMI_IBias test for channel " + channel + "\r\n");
                    return true;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Failed DMI_IBias test for channel " + channel + "\r\n");
                return false;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
