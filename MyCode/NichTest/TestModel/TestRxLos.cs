using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NichTest
{
    public class TestRxLos : ITest
    {
        public bool BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            //get the current test channel
            int channel = ConditionParaByTestPlan.Channel;
            try
            { 
                //get equipment object
                Attennuator attennuator = (Attennuator)equipments["ATTENNUATOR"];

                ////change to Rx path, no need this switch, otherwise it can't run parallel test
                //if (equipments.Keys.Contains("NA_OPTICALSWITCH"))
                //{
                //    OpticalSwitch opticalSwitch = (OpticalSwitch)equipments["NA_OPTICALSWITCH"];
                //    opticalSwitch.CheckEquipmentRole(2, (byte)channel);
                //}

                lock (attennuator)
                {
                    Log.SaveLogToTxt("Start to do RxLos test");

                    //get input parameters
                    double step = Convert.ToDouble(inPara["LOSADTUNESTEP"]);//调整步长: 0.3
                    bool isDetailedTest = Convert.ToBoolean(inPara["ISLOSDETAIL"]);//是否测具体值?: true

                    // open apc
                    Log.SaveLogToTxt("Close apc for module.");
                    dut.CloseAndOpenAPC(Convert.ToByte(DUT.APCMODE.IBAISandIMODON));

                    //get spec
                    double LosDMax = -14, LosAMin = -30;
                    double startAttValue = -6;
                    attennuator.AttnValue(startAttValue.ToString());
                    dut.ChkRxLos(channel);//清理Luanch

                    //test losA
                    double losA = 0, losD = 0;
                    bool isLosA = false, isLosD = true;
                    int i = 0;
                    if (isDetailedTest)
                    {
                        //detailed test
                        startAttValue = LosDMax;
                        int count = Convert.ToInt16((LosDMax + 1 - LosAMin) / step);
                        do
                        {
                            attennuator.AttnValue(startAttValue.ToString());
                            isLosA = dut.ChkRxLos(channel);
                            Thread.Sleep(100);
                            isLosA = dut.ChkRxLos(channel);
                            if (isLosA == false)
                            {
                                startAttValue -= step;
                                i++;
                            }

                            losA = startAttValue;
                        } while (isLosA == false && startAttValue >= LosAMin && i < count);
                    }
                    else
                    {
                        //quickly test
                        startAttValue = LosAMin;
                        attennuator.AttnValue(startAttValue.ToString(), 1);
                        Thread.Sleep(300);
                        isLosA = dut.ChkRxLos(channel);
                        losA = startAttValue;
                    }

                    if (isLosA == false)
                    {
                        Log.SaveLogToTxt("failed to assert los");
                    }
                    else
                    {
                        Log.SaveLogToTxt("successful to assert los");
                    }
                    Log.SaveLogToTxt("LosA = " + losA);

                    //test losD
                    if (isDetailedTest)
                    {
                        //detailed test
                        startAttValue = losA;
                        i = 0;
                        do
                        {
                            attennuator.AttnValue(startAttValue.ToString());
                            isLosD = dut.ChkRxLos(channel);
                            Thread.Sleep(100);
                            isLosD = dut.ChkRxLos(channel);
                            if (isLosD == true)
                            {
                                startAttValue += step;
                                i++;
                            }

                            losD = startAttValue;
                        } while (isLosD == true && startAttValue <= LosDMax && i < 30);
                    }
                    else
                    {
                        //quickly test
                        startAttValue = LosDMax;
                        attennuator.AttnValue(startAttValue.ToString(), 1);
                        Thread.Sleep(50);
                        isLosD = dut.ChkRxLos(channel);
                        losD = startAttValue;
                    }

                    if (isLosD == true)
                    {
                        Log.SaveLogToTxt("failed to deassert los");
                    }
                    else
                    {
                        Log.SaveLogToTxt("successful to deassert los");
                    }

                    //calculate losH
                    double losH = losD - losA;
                    Log.SaveLogToTxt("LosA = " + losA);
                    Log.SaveLogToTxt("LosD = " + losD);
                    Log.SaveLogToTxt("LosH = " + losH.ToString("f3"));
                    Log.SaveLogToTxt("End rxlos test for channel " + channel + "\r\n");
                    return true;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Failed rxlos test for channel " + channel + "\r\n");
                return false;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
