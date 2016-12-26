using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NichTest
{
    class TestTxEye : ITest
    {
        public bool BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            //get the current test channel
            int channel = ConditionParaByTestPlan.Channel;
            try
            {                
                //get equipment object
                Scope scope = (Scope)equipments["SCOPE"];

                ////change to Tx path, no need this switch, otherwise it can't run parallel test
                //if (equipments.Keys.Contains("NA_OPTICALSWITCH"))
                //{
                //    OpticalSwitch opticalSwitch = (OpticalSwitch)equipments["NA_OPTICALSWITCH"];
                //    opticalSwitch.CheckEquipmentRole(1, (byte)channel);
                //}

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
                        return false;
                    }

                    // open apc
                    Log.SaveLogToTxt("Close apc for module.");
                    dut.CloseAndOpenAPC(Convert.ToByte(DUT.APCMODE.IBAISandIMODON));

                    //Algorithm.GetSpec(specParameters, "MASKMARGIN(%)", 0, out MaskSpecMax, out MaskSpecMin);
                    double MaskSpecMax = 255, MaskSpecMin = -255;

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

                    foreach (string key in outData.Keys)
                    {
                        Log.SaveLogToTxt(key + " = " + outData[key].ToString("f2"));
                    }
                    Log.SaveLogToTxt("End tx eye test for channel " + channel + "\r\n");
                    return true;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Failed tx eye test for channel " + channel + "\r\n");
                return false;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
