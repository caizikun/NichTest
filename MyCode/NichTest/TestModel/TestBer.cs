using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace NichTest
{
    public class TestBer: ITest
    {
        public Dictionary<string, double> BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            //get the current test channel
            int channel = ConditionParaByTestPlan.Channel;
            try
            {  
                //get equipment object
                Attennuator attennuator = (Attennuator)equipments["ATTENNUATOR"];
                ErrorDetector ed = (ErrorDetector)equipments["ERRORDETECTOR"];

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
                    Log.SaveLogToTxt("Start to do sensitivity test");

                    //get input parameters
                    ArrayList erList = Algorithm.StringtoArraylistDeletePunctuations(GlobalParaByPN.OpticalSourceERArray, new char[] { ',' });
                    double senAlignRxPwr = Convert.ToDouble(inPara["CSENALIGNRXPWR(DBM)"]);//自动对齐时的入射光: -7
                    double senStartingRxPwr = Convert.ToDouble(inPara["CSENSTARTINGRXPWR(DBM)"]);//目标点搜索起始点,开始灵敏度测试时设定接收端光功率: -13
                    double searchTargetBerUL = Convert.ToDouble(inPara["SEARCHTARGETBERUL"]);//第一点误码率的上限: 	0.00001
                    double searchTargetBerLL = Convert.ToDouble(inPara["SEARCHTARGETBERLL"]);//第一点误码率的下限: 0.00000001
                    double searchTargetBerRxpowerUL = Convert.ToDouble(inPara["SEARCHTARGETRXPOWERUL"]);//寻找误码率点的光功率下限: -15
                    double searchTargetBerRxpowerLL = Convert.ToDouble(inPara["SEARCHTARGETRXPOWERLL"]);//寻找误码率点的光功率上限: -10
                    double coefCsenSubStep = Convert.ToDouble(inPara["COEFCSENSUBSTEP(DBM)"]);//推算灵敏度找误码点时减小光功率的步长: 0.3
                    double coefCsenAddStep = Convert.ToDouble(inPara["COEFCSENADDSTEP(DBM)"]);//推算灵敏度找误码点时增大光功率的步长: 0.5
                    bool isBerQuickTest = inPara["ISBERQUICKTEST"].Trim().ToUpper() == "FALSE" ? false : true;//快速测试还是具体值?
                    bool isOpticalSourceUnitOMA = inPara["ISOPTICALSOURCEUNITOMA"].Trim().ToUpper() == "FALSE" ? false : true;//光源是否是OMA输入: false
                    double searchTargetBerStep = Convert.ToDouble(inPara["SEARCHTARGETSTEP"]);//搜寻目标值步长: 0.5
                    byte searchTargetBerMethod = Convert.ToByte(inPara["SEARCHTARGETBERMETHOD"]);//搜寻第一点的方法0=二分法,1=Step: 0
                    double ber_ERP = Convert.ToDouble(inPara["BER_ERP"]);//目标误码率:	1E-12

                    //change unit from OMA to dBm
                    if (isOpticalSourceUnitOMA)
                    {
                        senAlignRxPwr = Algorithm.CalculateFromOMAtoDBM(senAlignRxPwr, Convert.ToDouble(erList[channel - 1]));
                        senStartingRxPwr = Algorithm.CalculateFromOMAtoDBM(senStartingRxPwr, Convert.ToDouble(erList[channel - 1]));
                        searchTargetBerRxpowerUL = Algorithm.CalculateFromOMAtoDBM(searchTargetBerRxpowerUL, Convert.ToDouble(erList[channel - 1]));
                        searchTargetBerRxpowerLL = Algorithm.CalculateFromOMAtoDBM(searchTargetBerRxpowerLL, Convert.ToDouble(erList[channel - 1]));
                    }

                    // open apc
                    Log.SaveLogToTxt("Close apc for module.");
                    dut.CloseAndOpenAPC(Convert.ToByte(DUT.APCMODE.IBAISandIMODON));

                    //auto align
                    Log.SaveLogToTxt("Set atten value.");
                    attennuator.AttnValue(senAlignRxPwr.ToString());
                    Log.SaveLogToTxt("Auto alaign...");
                    if (!ed.AutoAlign(true))
                    {
                        Log.SaveLogToTxt("Auto align failed");
                        return null;
                    }
                    Log.SaveLogToTxt("Auto align successfully");

                    //search the points to calulate sensitivity
                    double sensitivity = Algorithm.MyNaN;
                    double currentBer;
                    if (isBerQuickTest == true)
                    {
                        //QuickTest(attennuator, ed);
                        //quickly search, direct to test sensitivity, not to search points
                        attennuator.AttnValue(senStartingRxPwr.ToString());
                        currentBer = ed.RapidErrorRate();
                        Log.SaveLogToTxt("SetAtten=" + senStartingRxPwr.ToString());
                        Log.SaveLogToTxt("QUICBER=" + currentBer.ToString());
                        if (currentBer >= 1)
                        {
                            sensitivity = 1;
                            return null;
                        }

                        if (currentBer <= ber_ERP)
                        {
                            sensitivity = senStartingRxPwr;
                        }
                        else
                        {
                            sensitivity = currentBer;
                            Log.SaveLogToTxt("AttPoint=" + senStartingRxPwr.ToString() + ber_ERP.ToString());
                        }
                        //calculate the OMA sensitivity
                        double sensitivity_OMA = Algorithm.CalculateOMA(sensitivity, Convert.ToDouble(erList[channel - 1]));

                        //save testdata
                        Dictionary<string, double> dictionary = new Dictionary<string, double>();
                        dictionary.Add("Sensitivity", sensitivity);
                        dictionary.Add("Sensitivity_OMA", sensitivity_OMA);
                        dictionary.Add("Result", 1);
                        return dictionary;
                    }

                    //detailed search points
                    ArrayList serchAttPoints = new ArrayList();
                    ArrayList serchRxDmiPoints = new ArrayList();
                    ArrayList serchBerPoints = new ArrayList();
                    byte count = 0;
                    double currentInputPower = senStartingRxPwr;
                    //first: search the first point
                    double firstPoint = 0;
                    switch (searchTargetBerMethod)
                    {
                        case 1:
                            //attPoint = StepSearchTargetPoint(csenStartingRxPwr, searchTargetBerRxpowerLL, searchTargetBerRxpowerUL, testBerStruct.SearchTargetBerLL, testBerStruct.SearchTargetBerUL, tempAtten, tempED, out serchAttPoints, out serchRxDmidPoints, out serchBerPoints);                    
                            //search the first point by step                                       
                            while (currentInputPower <= searchTargetBerRxpowerUL && currentInputPower >= searchTargetBerRxpowerLL && count <= 20)
                            {
                                attennuator.AttnValue(currentInputPower.ToString());
                                double rxDmiPower = dut.ReadDmiRxP(channel);
                                currentBer = ed.RapidErrorRate();
                                serchAttPoints.Add(senStartingRxPwr);
                                serchRxDmiPoints.Add(rxDmiPower);
                                serchBerPoints.Add(currentBer);

                                if (currentBer < searchTargetBerLL)// 误码太小->光太大->减小光
                                {
                                    currentInputPower -= searchTargetBerStep;
                                    count++;
                                }
                                else if (currentBer > searchTargetBerUL)// 误码太大->光太小->加大入射光
                                {
                                    currentInputPower += searchTargetBerStep;
                                    count++;
                                }
                                else
                                {
                                    firstPoint = currentInputPower;
                                    break;
                                }
                            }
                            break;

                        default:
                            //attPoint = binarySearchTargetPoint(testBerStruct.CsenStartingRxPwr, testBerStruct.SearchTargetBerRxpowerLL, testBerStruct.SearchTargetBerRxpowerUL, testBerStruct.SearchTargetBerLL, testBerStruct.SearchTargetBerUL, tempAtten, tempED, out serchAttPoints, out serchRxDmidPoints, out serchBerPoints);
                            //search the first point by binary search
                            double low = searchTargetBerRxpowerLL;
                            double high = searchTargetBerRxpowerUL;
                            attennuator.AttnValue(((high + low) / 2).ToString(), 1);
                            currentBer = ed.RapidErrorRate();

                            while (low <= high && count <= 20)
                            {
                                if (Math.Abs(low - high) < 0.2)
                                {
                                    break;
                                }
                                double mid = (high + low) / 2;
                                attennuator.AttnValue(mid.ToString(), 1);

                                double rxDmiPower = dut.ReadDmiRxP(channel);
                                currentBer = ed.RapidErrorRate();
                                serchAttPoints.Add(mid);
                                serchRxDmiPoints.Add(rxDmiPower);
                                serchBerPoints.Add(currentBer);

                                if (currentBer < searchTargetBerLL)
                                {
                                    high = mid;
                                    count++;
                                }
                                else if (currentBer > searchTargetBerUL)
                                {
                                    low = mid;
                                    count++;
                                }
                                else
                                {
                                    firstPoint = mid;
                                    break;
                                }
                            }
                            break;
                    }//completed to search the first point

                    //second: search other points
                    //define BER of the final point
                    double terminativeBer = 0;
                    if (ber_ERP < searchTargetBerLL)
                    {
                        if (ber_ERP > 1E-12)//如果是大于E-12 ,并且比第一点的下限要小 ,那么就以目标值为下限
                        {
                            terminativeBer = ber_ERP;
                        }
                        else////如果是小于E-12 ,并且比第一点的下限要小 ,那么就以-为11下限
                        {
                            terminativeBer = 1E-11;
                        }

                        // terminativeBer = targetBer * 100;
                    }
                    else if (ber_ERP > searchTargetBerUL)
                    {
                        // terminativeBer = targetBer * 1E-1;
                        terminativeBer = ber_ERP;
                    }
                    attennuator.AttnValue(firstPoint.ToString(), 0);
                    Thread.Sleep(1500);
                    double point = firstPoint;
                    int i = 0;
                    bool done = false;
                    ArrayList points = new ArrayList();
                    ArrayList ber = new ArrayList();
                    do
                    {
                        attennuator.AttnValue(point.ToString(), 0);
                        Thread.Sleep(1000);
                        double rxDmiPower = dut.ReadDmiRxP(channel);
                        currentBer = ed.RapidErrorRate();

                        Log.SaveLogToTxt("RxInputPower=" + point);
                        Log.SaveLogToTxt("RxDmiPower=" + rxDmiPower.ToString("f2"));
                        Log.SaveLogToTxt("CurrentBer=" + currentBer.ToString());

                        if (ber_ERP < searchTargetBerLL)
                        {
                            done = currentBer > terminativeBer;
                        }
                        else if (ber_ERP > searchTargetBerUL)
                        {
                            done = currentBer < terminativeBer;
                        }

                        if (done == true && currentBer != 0)
                        {
                            points.Add(point);
                            ber.Add(currentBer);
                        }

                        if (currentBer > ber_ERP)
                        {
                            point += coefCsenAddStep;
                        }
                        else
                        {
                            point -= coefCsenSubStep;
                        }
                        i++;
                    } while (i < 8 && done);//completed to search points

                    //calculate the sensitivity
                    byte minCount = (byte)Math.Min(points.Count, ber.Count);
                    double[] pointsArray = new double[minCount];
                    double[] berArray = new double[minCount];
                    for (int j = 0; j < minCount; j++)
                    {
                        pointsArray[j] = double.Parse(points[j].ToString());
                        berArray[j] = double.Parse(ber[j].ToString());
                        Log.SaveLogToTxt("attPoints[ " + j + "] " + points[j].ToString() + "  " + "berPoints[ " + j + "] " + ber[j].ToString());
                    }
                    double slope, intercept;
                    Algorithm.LinearRegression(Algorithm.Getlog10(Algorithm.GetNegative(Algorithm.Getlog10(berArray))), pointsArray, out slope, out intercept);
                    sensitivity = slope + (intercept * Math.Log10(Math.Log10(ber_ERP) * (-1)));

                    if (double.IsNaN(sensitivity) || double.IsInfinity(sensitivity))
                    {
                        sensitivity = Algorithm.MyNaN;
                    }
                    sensitivity = Math.Round(sensitivity, 2);
                    Log.SaveLogToTxt("sensitivity = " + sensitivity.ToString());

                    //calculate the OMA sensitivity
                    double sensOMA = Algorithm.CalculateOMA(sensitivity, Convert.ToDouble(erList[channel - 1]));
                    Log.SaveLogToTxt("OMA sensitivity = " + sensOMA.ToString("f2"));
                    Log.SaveLogToTxt("End sensitivity test for channel " + channel + "\r\n");

                    //save testdata
                    Dictionary<string, double> dic = new Dictionary<string, double>();
                    dic.Add("Sensitivity", sensitivity);
                    dic.Add("Sensitivity_OMA", sensOMA);
                    dic.Add("Result", 1);
                    return dic;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Failed sensitivity test for channel " + channel + "\r\n");
                return null;
            }
        }

        public bool SaveTestData()
        {
            return true;
        }
    }
}
