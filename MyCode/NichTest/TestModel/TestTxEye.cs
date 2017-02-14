using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    class TestTxEye : ITest
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
                    Log.SaveLogToTxt("Start to do TxEye test for channel " + channel);

                    //prepare environment
                    if (scope.SetMaskAlignMethod(1) && scope.SetMode(0) && scope.MaskONOFF(false) &&
                      scope.SetRunTilOff() && scope.RunStop(true) && scope.OpenOpticalChannel(true) &&
                      scope.RunStop(true) && scope.ClearDisplay() && scope.AutoScale())
                    {
                        Log.SaveLogToTxt("PrepareEnvironment OK!");
                    }
                    else
                    {
                        Log.SaveLogToTxt("PrepareEnvironment Fail!");
                        return null;
                    }

                    // open apc
                    Log.SaveLogToTxt("Close apc for module.");
                    dut.CloseAndOpenAPC(Convert.ToByte(DUT.APCMODE.IBAISandIMODON));

                    //Algorithm.GetSpec(specParameters, "MASKMARGIN(%)", 0, out MaskSpecMax, out MaskSpecMin);
                    double MaskSpecMax = 255, MaskSpecMin = 0;

                    //TxEye test
                    Dictionary<string, double> outData = new Dictionary<string, double>();
                    bool flagMask = false;
                    scope.OpticalEyeTest(ref outData, 1);

                    if (outData["MASKMARGIN(%)"] > MaskSpecMax || outData["MASKMARGIN(%)"] < MaskSpecMin)
                    {
                        flagMask = false;
                    }
                    else
                    {
                        flagMask = true;
                    }

                    //retest
                    if (!flagMask)//后三次测试有一次失败就不在测试
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            scope.OpticalEyeTest(ref outData);

                            if (outData["MASKMARGIN(%)"] > MaskSpecMax || outData["MASKMARGIN(%)"] < MaskSpecMin)
                            {
                                break;
                            }
                        }
                    }

                    //save testdata
                    Dictionary<string, double> dic = new Dictionary<string, double>();
                    foreach (string key in outData.Keys)
                    {
                        Log.SaveLogToTxt(key + " = " + outData[key].ToString("f2"));
                        dic.Add(key, outData[key]);
                    }
                    Log.SaveLogToTxt("End tx eye test for channel " + channel + "\r\n");                                     
                    
                    dic.Add("Result", 1);
                    return dic;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Failed tx eye test for channel " + channel + "\r\n");
                return null;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
