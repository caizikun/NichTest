using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace NichTest
{
    public class QuickCheckTest : ITest
    {
        private TestData testData;
        
        public QuickCheckTest()
        {
            testData = new TestData();
        }

        public Dictionary<string, double> BeginTest(DUT dut, Dictionary<string, IEquipment> equipments, Dictionary<string, string> inPara)
        {
            try
            {
                //get the current test channel
                int channel = ConditionParaByTestPlan.Channel;
                Log.SaveLogToTxt("Start to do quick check test for channel " + channel);
                //get equipment object
                PowerSupply supply = (PowerSupply)equipments["POWERSUPPLY"];
                Attennuator attennuator = (Attennuator)equipments["ATTENNUATOR"];
                OpticalSwitch opticalSwitch = (OpticalSwitch)equipments["OPTICALSWITCH"];
                PowerMeter powerMeter = (PowerMeter)equipments["POWERMETER"];
                //get in parameters
                string[] BiasDACs = inPara["BIASDACS"].Split(',');
                string[] ModDACs = inPara["MODDACS"].Split(',');
                double ratio = Convert.ToDouble(inPara["RATIO"]);
                double U_Ref = Convert.ToDouble(inPara["UREF"]);
                double resolution = Convert.ToDouble(inPara["RESOLUTION"]);
                double R_rssi = Convert.ToDouble(inPara["RRSSI"]);

                //read current of power supply
                double current = supply.GetCurrent();

                // close apc
                Log.SaveLogToTxt("Close apc for module.");
                dut.CloseAndOpenAPC(Convert.ToByte(DUT.APCMODE.IBAISandIMODOFF));

                //disable attennuator and Tx to read Rx/TxDarkADC
                Log.SaveLogToTxt("Shut down attennuator and disable Tx to read Tx/RxDark ADC for channel " + channel);
                attennuator.OutPutSwitch(false);                
                ushort RxDarkADC = dut.ReadADC(DUT.NameOfADC.RXPOWERADC, channel);
                dut.SetSoftTxDis();
                ushort TxDarkADC = dut.ReadADC(DUT.NameOfADC.TXPOWERADC, channel);
                Log.SaveLogToTxt("TxDarkADC is " + TxDarkADC);
                Log.SaveLogToTxt("RxDarkADC is " + RxDarkADC);

                //enable attennuator and Tx
                Log.SaveLogToTxt("Power on attennuator and set value to 0");
                attennuator.OutPutSwitch(true);
                attennuator.SetAttnValue(0);
                Log.SaveLogToTxt("Light on all Tx channel.");
                dut.TxAllChannelEnable();
                                
                ConfigXmlIO myXml = new ConfigXmlIO(FilePath.ConfigXml);
                //get scope/powermeter's offset
                string[] offsetArray = myXml.ScopeOffset.Split(',');
                double offset = Convert.ToDouble(offsetArray[channel-1]); 
                //get attenator's ofset
                string[] lightSourceArray = myXml.AttennuatorOffset.Split(',');
                double lightSource = Convert.ToDouble(lightSourceArray[channel-1]);
                //calculate RxRes
                double RxRes = dut.CalRxRes(lightSource, channel, ratio, U_Ref, resolution, R_rssi);
                Log.SaveLogToTxt("Calculate RES of Rx is " + RxRes.ToString("f3"));
                
                //set default Bias/ModDAC
                Log.SaveLogToTxt("Set BiasDAC is " + BiasDACs[channel - 1]);
                Log.SaveLogToTxt("Set ModDAC is " + ModDACs[channel - 1]);
                dut.WriteChipDAC(DUT.NameOfChipDAC.BIASDAC, channel, BiasDACs[channel - 1]);
                dut.WriteChipDAC(DUT.NameOfChipDAC.MODDAC, channel, ModDACs[channel - 1]);
                //change to related change, and read Tx power
                opticalSwitch.ChangeChannel(channel);
                double TxP = powerMeter.ReadPower(channel) + offset;
                Log.SaveLogToTxt("Get Tx power is " + TxP.ToString("f3"));

                //read SN again and check its format
                string dut_SN = dut.ReadSN();
                if (!Algorithm.CheckSerialNumberFormat(dut_SN))
                {
                    Log.SaveLogToTxt("Module' serial number is not correct");
                    return null;
                }

                Log.SaveLogToTxt("Again. Read module' serial number is " + dut_SN);
                
                //save test data to TestData class
                DataRow dr = testData.NewRow();
                dr["Family"] = GlobalParaByPN.Family;
                dr["PartNumber"] = GlobalParaByPN.PN;
                dr["SerialNumber"] = dut_SN;
                dr["Channel"] = channel;
                dr["Current"] = current.ToString("f3");
                dr["Temp"] = ConditionParaByTestPlan.Temp;
                dr["Station"] = GlobalParaByPN.Station;
                dr["Time"] = DateTime.Now;
                dr["TxDarkADC"] = TxDarkADC;
                dr["TxPower"] = TxP.ToString("f3");
                dr["RxDarkADC"] = RxDarkADC;
                dr["RxRes"] = RxRes.ToString("f3");
                dr["DeltaTxDarkADC"] = -99999;
                dr["DeltaTxPower"] = "-99999";
                dr["DeltaRxDarkADC"] = -99999;
                dr["DeltaRxRes"] = "-99999";
                dr["Result"] = -1;

                if (GlobalParaByPN.Station == "PreModule")
                {
                    dr["Result"] = 0;
                }
                else
                {
                    Log.SaveLogToTxt("Try to get test data of pre module station to calculate delta.");
                    //conect to mysql database to get test recording of pre module station
                    string mysqlconCommand = "Database=my_databases;Data Source=localhost;User Id=root;Password=abc@123;pooling=false;CharSet=utf8;port=3306";
                    MySqlConnection mycon = new MySqlConnection();
                    mycon.ConnectionString = mysqlconCommand;
                    mycon.Open();

                    string table = "my_databases.quickcheck_testdata";
                    string experisson = "SELECT * FROM my_databases.quickcheck_testdata where PartNumber = '" + GlobalParaByPN.PN + "' and Serialnumber = '" +
                        dut_SN + "' and Channel = " + channel + " and Temp = " + ConditionParaByTestPlan.Temp + " and Station = 'PreModule' order by ID";

                    MySqlDataAdapter da = new MySqlDataAdapter(experisson, mycon);
                    MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
                    DataSet ds = new DataSet(table);
                    da.Fill(ds, table);
                    DataTable dt = ds.Tables[table];
                    mycon.Close();

                    if (dt.Rows.Count != 0)
                    {
                        //calculate delta: post - pre
                        dr["DeltaTxDarkADC"] = Convert.ToInt32(dr["TxDarkADC"]) - Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["TxDarkADC"]);
                        dr["DeltaTxPower"] = (Convert.ToDouble(dr["TxPower"]) - Convert.ToDouble(dt.Rows[dt.Rows.Count - 1]["TxPower"])).ToString("f3");
                        dr["DeltaRxDarkADC"] = Convert.ToInt32(dr["RxDarkADC"]) - Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["RxDarkADC"]);
                        double orgRxRes = Convert.ToDouble(dt.Rows[dt.Rows.Count - 1]["RxRes"]);
                        //orgRxRes can't be zero
                        dr["DeltaRxRes"] = (orgRxRes == 0) ? "-99999" : (100.0 * (Convert.ToDouble(dr["RxRes"]) - orgRxRes) / orgRxRes).ToString("f2") + "%";
                        dr["Result"] = 0;
                        Log.SaveLogToTxt("Sucessfully get test data of pre module station.");
                        Log.SaveLogToTxt("DeltaTxDarkADC is " + dr["DeltaTxDarkADC"]);
                        Log.SaveLogToTxt("DeltaTxPower is " + dr["DeltaTxPower"]);
                        Log.SaveLogToTxt("DeltaRxDarkADC is " + dr["DeltaRxDarkADC"]);
                        Log.SaveLogToTxt("DeltaRxRes is " + dr["DeltaRxRes"]);
                    }
                    else
                    {
                        Log.SaveLogToTxt("Failed to get test data of pre module station.");
                        //return false;
                    }                    
                }
                testData.Rows.Add(dr);
                dr = null;
                Log.SaveLogToTxt("End quick check test for channel " + channel + "\r\n");

                //save testdata
                Dictionary<string, double> dic = new Dictionary<string, double>();
                dic.Add("Result", 1);
                return dic;
            }
            catch
            {
                Log.SaveLogToTxt("Failed quickcheck test.");
                return null;
            }
        }

        public bool SaveTestData()
        {
            Log.SaveLogToTxt("Upload test data to server...");
            string mysqlconCommand = "Database=my_databases;Data Source=localhost;User Id=root;Password=abc@123;pooling=false;CharSet=utf8;port=3306";
            MySqlConnection mycon = new MySqlConnection();
            mycon.ConnectionString = mysqlconCommand;
            try
            {
                mycon.Open();
                string mysqlCommand = "select * from my_databases.quickcheck_testdata";
                MySqlCommand cmd = new MySqlCommand(mysqlCommand, mycon);
                MySqlDataAdapter mda = new MySqlDataAdapter(cmd);
                MySqlCommandBuilder cb = new MySqlCommandBuilder(mda);
                MySqlTransaction tr;
                tr = mycon.BeginTransaction(IsolationLevel.RepeatableRead);
                mda.SelectCommand.Transaction = tr;

                if (testData.GetErrors() != null)
                {
                    //upload test data to mysql database
                    mda.Update(testData);
                    tr.Commit();
                    Log.SaveLogToTxt("Upload test data to server sucessfully.");

                    Log.SaveLogToTxt("Save test data to xml...");
                    bool xmlResul = this.SaveToXml();
                    Log.SaveLogToTxt("Save test data to xml " + (xmlResul ? "sucessfully" : "failed").ToString());
                    return true;
                }
                Log.SaveLogToTxt("Test data is abnormal.");
                return false;
            }
            catch(Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                Log.SaveLogToTxt("Failed to upload test data to MySQL.");
                return false;
            }
            finally
            {
                mycon.Close();
            }
        }

        private bool SaveToXml()
        {
            try
            { 
                //save the new test data to xml file.
                TestData data = new TestData();
                data.ReadXml("TestData", FilePath.TestDataXml);

                int delta = testData.Rows.Count - data.Rows.Count;

                if (delta == 0)
                {
                    testData.WriteXml("TestData", FilePath.TestDataXml);
                    return true;
                }

                for (int row = 0; row < data.Rows.Count; row++)
                {
                    for (int column = 1; column < data.Columns.Count; column++)
                    {
                        if (delta > 0)
                        {
                            data.Rows[row][column] = testData.Rows[row + delta][column];
                        }
                        else
                        {
                            if (row < -1 * delta)
                            {
                                data.Rows[row][column] = data.Rows[row + testData.Rows.Count][column];
                            }
                            else
                            {
                                data.Rows[row][column] = testData.Rows[row + delta][column];
                            }
                        }
                    }
                }
                data.WriteXml("TestData", FilePath.TestDataXml);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
