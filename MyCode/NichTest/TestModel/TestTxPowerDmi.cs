using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NichTest
{
    public class TestTxPowerDmi : ITest
    {
        public bool BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            //get the current test channel
            int channel = ConditionParaByTestPlan.Channel;
            try
            { 
                //get equipment object
                Scope scope = (Scope)equipments["SCOPE"];

                lock (scope)
                {                    
                    Log.SaveLogToTxt("Start to do TxPowerDmi test for channel " + channel);

                    //prepare environment
                    if (scope.SetMaskAlignMethod(1) &&
                    scope.SetMode(0) &&
                    scope.MaskONOFF(false) &&
                    scope.SetRunTilOff() &&
                    scope.RunStop(true) &&
                    scope.OpenOpticalChannel(true) &&
                    scope.RunStop(true) &&
                    scope.ClearDisplay() &&
                    scope.AutoScale(1)
                    )
                    {
                        Log.SaveLogToTxt("PrepareEnvironment OK!");
                    }
                    else
                    {
                        Log.SaveLogToTxt("PrepareEnvironment Fail!");
                        return false;
                    }

                    //test dmi
                    Log.SaveLogToTxt("Read DCA TxPower");
                    double txDCAPowerDmi = scope.GetAveragePowerdBm();
                    Log.SaveLogToTxt("txDCAPowerDmi = " + txDCAPowerDmi.ToString("f2"));
                    Log.SaveLogToTxt("Read DUT Txpower");
                    double txPowerDmi = dut.ReadDmiTxP(channel);
                    Log.SaveLogToTxt("txPowerDmi = " + txPowerDmi.ToString("f2"));
                    double txDmiPowerErr = txPowerDmi - txDCAPowerDmi;
                    Log.SaveLogToTxt("txDmiPowerErr = " + txDmiPowerErr.ToString("f2"));

                    //test disable power
                    //have to lock this object, due to it can't run parallel test with TestIBiasDmi
                    //as this will disable TxPower, ibiasDmi will be defected.
                    lock (dut)
                    {
                        dut.SetSoftTxDis();
                        scope.ClearDisplay();
                        double txDisablePower = scope.GetAveragePowerdBm();
                        Log.SaveLogToTxt("txDisablePower = " + txDisablePower.ToString());
                        Thread.Sleep(200);
                        if (!dut.TxAllChannelEnable())
                        {
                            scope.ClearDisplay();
                            Thread.Sleep(200);
                            return false;
                        }
                    }
                    scope.ClearDisplay();
                    Thread.Sleep(200);
                    Log.SaveLogToTxt("End TxPowerDmi test " + channel + "\r\n");
                    return true;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Failed TxPowerDmi test for channel " + channel + "\r\n");
                return false;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
