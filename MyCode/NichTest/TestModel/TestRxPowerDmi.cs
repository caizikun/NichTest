using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace NichTest
{
    public class TestRxPowerDmi : ITest
    {
        public Dictionary<string, double> BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            //get the current test channel
            int channel = ConditionParaByTestPlan.Channel;
            try
            {
                //get equipment object
                Attennuator attennuator = (Attennuator)equipments["ATTENNUATOR"];

                //change to Rx path, if have this equipment, it can not run parallel test, just to do one by one.
                //due to it is the common equipment between Tx and Rx.
                if (equipments.Keys.Contains("NA_OPTICALSWITCH"))
                {
                    Log.SaveLogToTxt("It can not parallel initial, due to Tx/Rx have common equipment NA_OPTICALSWITCH.");
                    Log.SaveLogToTxt("have to test one by one.");
                    OpticalSwitch opticalSwitch = (OpticalSwitch)equipments["NA_OPTICALSWITCH"];
                    opticalSwitch.CheckEquipmentRole(2, (byte)channel);
                }

                lock (attennuator)
                {                    
                    Log.SaveLogToTxt("Start to do RxPowerDmi test for channel " + channel);

                    //get input para
                    ArrayList rxInputPower = Algorithm.StringtoArraylistDeletePunctuations(inPara["RXPOWERARRLIST(DBM)"], new char[] { ',' });
                    if (rxInputPower == null)
                    {
                        return null;
                    }
                    
                    //set different point to get rxPowerDmi
                    attennuator.AttnValue(rxInputPower[0].ToString(), 1);
                    Thread.Sleep(3000);
                    double[] rxPowerDmiArray = new double[rxInputPower.Count];
                    double[] rxPowerErrArray = new double[rxInputPower.Count];
                    double[] rxPowerErrRawArray = new double[rxInputPower.Count];
                    for (byte i = 0; i < rxInputPower.Count; i++)
                    {
                        attennuator.AttnValue(rxInputPower[i].ToString(), 1);
                        rxPowerDmiArray[i] = dut.ReadDmiRxP(channel);
                        rxPowerErrArray[i] = Math.Abs(Convert.ToDouble(rxInputPower[i].ToString()) - rxPowerDmiArray[i]);
                        rxPowerErrRawArray[i] = Convert.ToDouble(rxInputPower[i].ToString()) - rxPowerDmiArray[i];
                        Log.SaveLogToTxt("rxInputPower[" + i.ToString() + "]: " + rxInputPower[i].ToString() + " rxPowerDmiArray[" + i.ToString("f3") + "]: " + rxPowerDmiArray[i].ToString() + " rxPowerErrArray[" + i.ToString() + "]: " + rxPowerErrArray[i].ToString("f3"));
                    }

                    //calculate delta and get the max
                    byte maxIndex;
                    Algorithm.SelectMaxValue(ArrayList.Adapter(rxPowerErrArray), out maxIndex);
                    double maxErr = rxPowerErrRawArray[maxIndex];
                    double errMaxPoint = Convert.ToDouble(rxInputPower[maxIndex].ToString());
                    Log.SaveLogToTxt("ErrMaxPoint = " + errMaxPoint.ToString() + "  MaxErr = " + maxErr.ToString("f3"));

                    //get rxPowerDmi without any input optical power
                    attennuator.OutPutSwitch(false);
                    Thread.Sleep(2000);
                    double rxNopticalPoint = dut.ReadDmiRxP(channel);
                    Log.SaveLogToTxt("RxNopticalPoint=" + rxNopticalPoint.ToString());
                    attennuator.OutPutSwitch(true);

                    Log.SaveLogToTxt("End RxPowerDmi test for channel " + channel + "\r\n");

                    //save testdata
                    Dictionary<string, double> dictionary = new Dictionary<string, double>();
                    dictionary.Add("DmiRxPWRErr", maxErr);
                    dictionary.Add("DmiRxNOptical", rxNopticalPoint);
                    dictionary.Add("Result", 1);
                    return dictionary;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Failed DMI_ICC test for channel " + channel + "\r\n");
                return null;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
