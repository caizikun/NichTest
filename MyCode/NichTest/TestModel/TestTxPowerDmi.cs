using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NichTest
{
    public class TestTxPowerDmi : ITest
    {
        public Dictionary<string, double> BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            //get the current test channel
            int channel = ConditionParaByTestPlan.Channel;
            try
            { 
                //get equipment object
                Scope scope = (Scope)equipments["SCOPE"];

                //change to Tx path, if have this equipment, it can not run parallel test, just to do one by one.
                //due to it is the common equipment between Tx and Rx.
                if (equipments.Keys.Contains("NA_OPTICALSWITCH"))
                {
                    Log.SaveLogToTxt("It can not parallel initial, due to Tx/Rx have common equipment NA_OPTICALSWITCH.");
                    Log.SaveLogToTxt("have to test one by one.");
                    OpticalSwitch opticalSwitch = (OpticalSwitch)equipments["NA_OPTICALSWITCH"];
                    opticalSwitch.CheckEquipmentRole(1, (byte)channel);
                }

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
                        return null;
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

                    //save testdata
                    Dictionary<string, double> dictionary = new Dictionary<string, double>();
                    dictionary.Add("DmiTxPowerErr", txDmiPowerErr);

                    //test disable power
                    //have to lock this object, due to it can't run parallel test with TestIBiasDmi
                    //as this will disable TxPower, ibiasDmi will be defected.
                    lock (dut)
                    {
                        dut.SetSoftTxDis();
                        scope.ClearDisplay();
                        double txDisablePower = scope.GetAveragePowerdBm();
                        Log.SaveLogToTxt("txDisablePower = " + txDisablePower.ToString());
                        dictionary.Add("TxDisablePower", txDisablePower);
                        Thread.Sleep(200);
                        if (!dut.TxAllChannelEnable())
                        {
                            scope.ClearDisplay();
                            Thread.Sleep(200);
                            return null;
                        }
                    }
                    scope.ClearDisplay();
                    Thread.Sleep(200);
                    Log.SaveLogToTxt("End TxPowerDmi test " + channel + "\r\n"); 
                    
                    dictionary.Add("Result", 1);
                    return dictionary;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Failed TxPowerDmi test for channel " + channel + "\r\n");
                return null;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
