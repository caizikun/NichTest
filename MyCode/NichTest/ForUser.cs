using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace NichTest
{
    public class ForUser
    {
        private Dictionary<string, IEquipment> usedEquipments;
        private Dictionary<string, ITest> testItemsObject;
        private IFactory myFactory;
        private DataTable dataTable_Spec;
        private ConfigXmlIO myXml;
        private DataIO myDataIO;        
        //equipment
        private PowerSupply supply;
        private Thermocontroller tempControl;
        private Attennuator attennuator;
        private DUT dut;
        //testdata table
        private TxOTable txoTable;
        private RxOTable rxoTable;
        private DataRow drOfTxOTable;
        private DataRow drOfRxOTable;

        public string GetIP()
        {
            string hostname = System.Net.Dns.GetHostName(); //主机
            System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(hostname);//网卡IP地址集合   
            string IP = "";
            for (int i = 0; i < ipEntry.AddressList.Length; i++)
            {
                if (ipEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IP = ipEntry.AddressList[i].ToString();
                    // IP4
                    break;
                }
            }
            return IP;
        }

        public string[] GetProductionFamily(ConfigXmlIO configXml, ref DataIO dataIO, ref DataTable dataTable_Family)
        {
            try
            {                
                this.myXml = configXml;
                if (myXml.DatabaseType.ToUpper() == "LOCATIONDATABASE")
                {
                    myDataIO = new LocalDatabase(myXml.DatabasePath);
                }
                else//SqlDatabase
                {
                    myDataIO = new SqlDatabase(myXml.DatabasePath, myXml.DbName, myXml.Username, myXml.PWD);
                }

                if (myDataIO.OpenDatabase(true))
                {
                    dataIO = this.myDataIO;
                    string dataBaseTable = "GlobalProductionType";
                    string expression = "select * from " + dataBaseTable + " Where IgnoreFlag='false' order by ID";
                    dataTable_Family = myDataIO.GetDataTable(expression, dataBaseTable);
                    return dataTable_Family.AsEnumerable().Select(d => d.Field<string>("ItemName")).ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public void GetTestPlanParaByPN(int ID_TestPlan)
        {            
            string expression = "Select* from TopoTestPlan where ID=" + ID_TestPlan;
            DataTable dataTable_TestPlan = myDataIO.GetDataTable(expression, "TopoTestPlan");
            if (dataTable_TestPlan == null)
            {
                throw new Exception("No test plan para");
            }
            else
            {
                TestPlanParaByPN.SetValue(dataTable_TestPlan);
            }            
        }

        public void GetSpec(int ID_TestPlan)
        {
            try
            {
                DataTable dt = new DataTable();
                string expression = "Select* from TopoPNSpecsParams left join GlobalSpecs on TopoPNSpecsParams.SID=GlobalSpecs.ID  where TopoPNSpecsParams.PID=" + ID_TestPlan + "order by TopoPNSpecsParams.id";
                string dataBaseTable = "TopoPNSpecsParams";
                dt = myDataIO.GetDataTable(expression, dataBaseTable);
                dt.Columns.Add("FullName");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["FullName"] = dt.Rows[i]["ItemName"].ToString() + "(" + dt.Rows[i]["Unit"].ToString() + ")";
                }
                this.dataTable_Spec = dt;
            }
            catch
            {
                this.dataTable_Spec = null;
            }
        }

        public void CreatFolderPath(string[] folderPath)
        {
            FolderPath.SetValue(folderPath);
        }

        public bool Initial()
        {
            try
            {
                myFactory = new NateFactory();
                DataTable dataTable_Equipment;
                DataTable equipmentLits = this.GetEquipmentInfo(out dataTable_Equipment);
                usedEquipments = new Dictionary<string, IEquipment>();
                for (int i = 0; i < equipmentLits.Rows.Count; i++)
                {                    
                    //create equipment object
                    string equipmentName = equipmentLits.Rows[i]["Name"].ToString().ToUpper();
                    string equipmentFullName = equipmentLits.Rows[i]["FullName"].ToString();
                    string equipmentType = equipmentLits.Rows[i]["TYPE"].ToString().ToUpper();
                    IEquipment equipment = myFactory.CreateEquipment(equipmentName);

                    //config offset
                    if (equipmentFullName.Contains("SCOPE"))
                    {
                        Log.SaveLogToTxt("Config offset for equipment " + equipmentName.ToLower());
                        string[] settingArray = myXml.ScopeOffset.Split(',');
                        for (int j = 1; j < settingArray.Length + 1; j++)
                        {
                            double offset = Convert.ToDouble(settingArray[j - 1]);
                            equipment.ConfigOffset(j, offset);
                        }
                    }

                    if (equipmentFullName.Contains("ATT"))
                    {
                        Log.SaveLogToTxt("Config offset for equipment " + equipmentName.ToLower());
                        string[] settingArray = myXml.AttennuatorOffset.Split(',');
                        for (int j = 1; j < settingArray.Length + 1; j++)
                        {
                            double offset = Convert.ToDouble(settingArray[j - 1]);
                            equipment.ConfigOffset(j, offset);
                        }
                    }

                    if (equipmentFullName.Contains("POWERSUPPLY"))
                    {
                        Log.SaveLogToTxt("Config offset for equipment " + equipmentName.ToLower());
                        double offset = Convert.ToDouble(myXml.VccOffset);
                        equipment.ConfigOffset(1, offset);
                    }

                    //get para of the current equipment
                    Log.SaveLogToTxt("Get parameter for equipment " + equipmentName.ToLower());
                    string dataTable = "TopoEquipmentParameter";
                    string expression = "select * from " + dataTable + " where PID= " + equipmentLits.Rows[i]["id"].ToString() + " order by ID";
                    //DataTable dt = myDataIO.GetDataTable(expression, dataTable);
                    DataRow[] drs = dataTable_Equipment.Select("id='" + equipmentLits.Rows[i]["id"].ToString() + "'");
                    Dictionary<string, string> currentEquipmentPara = new Dictionary<string, string>();
                    foreach (DataRow dr in drs)
                    {
                        string name = dr["ItemName"].ToString().Trim().ToUpper();
                        string value = dr["ItemValue"].ToString().Trim().ToUpper();
                        currentEquipmentPara.Add(name, value);
                    }
                    string role = dataTable_Equipment.Rows[i]["Role"].ToString().Trim().ToUpper();// 0=NA,1=TX,2=RX
                    currentEquipmentPara.Add("ROLE", role);

                    Log.SaveLogToTxt("Initial equipment " + equipmentName.ToLower());
                    if (equipment.Initial(currentEquipmentPara) && equipment.Configure(1))
                    {
                        usedEquipments.Add(equipmentType, equipment);
                    }
                    else
                    {
                        Log.SaveLogToTxt("Failed to initial equipment " + equipmentName.ToLower());
                        return false;
                    }
                    
                    currentEquipmentPara = null;
                    equipment.OutPutSwitch(false);
                }
                Log.SaveLogToTxt("Initial equipment successfully.");
                return true;
            }
            catch
            {
                Log.SaveLogToTxt("Failed to initial equipment.");
                return false;
            }
        }

        public bool ParallelInitial()
        {
            try
            {
                myFactory = new NateFactory();
                DataTable dataTable_Equipment;
                DataTable equipmentLits = this.GetEquipmentInfo(out dataTable_Equipment);
                usedEquipments = new Dictionary<string, IEquipment>();
                bool  result = true;
                Parallel.For(0, equipmentLits.Rows.Count, (int i, ParallelLoopState pls) =>
                    {
                        //get para of the current equipment
                        string dataTable = "TopoEquipmentParameter";
                        string expression = "select * from " + dataTable + " where PID= " + equipmentLits.Rows[i]["id"].ToString() + " order by ID";
                        //DataTable dt = myDataIO.GetDataTable(expression, dataTable);
                        DataRow[] drs = dataTable_Equipment.Select("id='" + equipmentLits.Rows[i]["id"].ToString() + "'");
                        Dictionary<string, string> currentEquipmentPara = new Dictionary<string, string>();
                        foreach (DataRow dr in drs)
                        {
                            string name = dr["ItemName"].ToString().Trim().ToUpper();
                            string value = dr["ItemValue"].ToString().Trim().ToUpper();
                            currentEquipmentPara.Add(name, value);
                        }

                        string role = dataTable_Equipment.Rows[i]["Role"].ToString().Trim().ToUpper();// 0=NA,1=TX,2=RX
                        currentEquipmentPara.Add("ROLE", role);
                        //create equipment object
                        string equipmentName = equipmentLits.Rows[i]["Name"].ToString().ToUpper();
                        string equipmentFullName = equipmentLits.Rows[i]["FullName"].ToString();
                        string equipmentType = equipmentLits.Rows[i]["TYPE"].ToString().ToUpper();
                        IEquipment equipment = myFactory.CreateEquipment(equipmentName);

                        //config offset
                        if (equipmentFullName.Contains("SCOPE"))
                        {
                            string[] settingArray = myXml.ScopeOffset.Split(',');
                            for (int j = 1; j < settingArray.Length + 1; j++)
                            {
                                double offset = Convert.ToDouble(settingArray[j - 1]);
                                equipment.ConfigOffset(j, offset);
                            }
                        }

                        if (equipmentFullName.Contains("ATT"))
                        {
                            string[] settingArray = myXml.AttennuatorOffset.Split(',');
                            for (int j = 1; j < settingArray.Length + 1; j++)
                            {
                                double offset = Convert.ToDouble(settingArray[j - 1]);
                                equipment.ConfigOffset(j, offset);
                            }
                        }

                        if (equipmentFullName.Contains("POWERSUPPLY"))
                        {
                            double offset_VCC = Convert.ToDouble(myXml.VccOffset);
                            double offset_ICC = Convert.ToDouble(myXml.IccOffset);
                            equipment.ConfigOffset(1, offset_VCC, offset_ICC);
                        }

                        if (equipmentFullName.Contains("NA_OPTICALSWITCH"))
                        {
                            Log.SaveLogToTxt("Failed.");
                            Log.SaveLogToTxt("It can not parallel initial, due to Tx/Rx have common equipment NA_OPTICALSWITCH.");
                            result = false;
                            pls.Break();
                        }

                        if (equipment.Initial(currentEquipmentPara) && equipment.Configure(1))
                        {
                            usedEquipments.Add(equipmentType, equipment);
                            result = true;
                        }
                        else
                        {
                            result = false;
                            pls.Break();
                            //log.AdapterLogString(3, equipmentFullName + "Configure Error");
                            //Exception ex = new Exception(equipmentFullName + "Configure Error");
                            //throw ex;
                        }

                        currentEquipmentPara = null;
                        equipment.OutPutSwitch(false);
                    });
                return result;
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.ToString());
                return false;
            }
        }

        private DataTable GetEquipmentInfo(out DataTable dataTable_Equipment)
        {
            try
            {
                Log.SaveLogToTxt("Try to get equipment infomation from server.");

                String expression = "SELECT GlobalAllEquipmentList.ItemType AS Type,GlobalAllEquipmentList.ItemName AS Name,TopoEquipment.Role AS Role,TopoEquipment.ID AS ID, GlobalAllEquipmentParamterList.ItemName AS ItemName,TopoEquipmentParameter.ItemValue AS ItemValue" +
                " FROM GlobalAllEquipmentList,TopoEquipment ,TopoEquipmentParameter,GlobalAllEquipmentParamterList  where " +
                " (TopoEquipment.GID=GlobalAllEquipmentList.ID and TopoEquipment.ID=TopoEquipmentParameter.PID and TopoEquipmentParameter.GID=GlobalAllEquipmentParamterList.ID)and TopoEquipment.PID=" + TestPlanParaByPN.ID;
                dataTable_Equipment = myDataIO.GetDataTable(expression, "TopoEquipment");
                if (dataTable_Equipment != null)
                {
                    dataTable_Equipment.Columns.Add("FullName");

                    for (int i = 0; i < dataTable_Equipment.Rows.Count; i++)
                    {
                        string role = "_NA";
                        switch (dataTable_Equipment.Rows[i]["Role"].ToString().ToUpper())// 0=NA,1=TX,2=RX
                        {
                            case "2":
                                role = "_RX";
                                break;
                            case "1":
                                role = "_TX";
                                break;
                            default:
                                role = "_NA";
                                break;
                        }
                        dataTable_Equipment.Rows[i]["FullName"] = dataTable_Equipment.Rows[i]["Name"].ToString().Trim().ToUpper() + role + "_" + dataTable_Equipment.Rows[i]["Type"].ToString().ToUpper();
                    }
                    DataTable equipList = dataTable_Equipment.Clone();
                    DataView dv = dataTable_Equipment.DefaultView;
                    DataTable dt = dv.ToTable(true, "FullName");
                    string[] num = dt.AsEnumerable().Select(d => d.Field<string>("FullName")).ToArray();
                    for (int i = 0; i < num.Length; i++)
                    {
                        DataRow[] drarray = dataTable_Equipment.Select("FullName='" + num[i].ToString() + "'");
                        DataRow dr = equipList.NewRow();
                        for (int j = 0; j < equipList.Columns.Count; j++)
                        {
                            dr[j] = drarray[0][j];
                        }
                        equipList.Rows.Add(dr);
                    }
                    return equipList;
                }
                return null;
            }
            catch
            {
                Log.SaveLogToTxt("Failed to get equipment infomation from server.");
                dataTable_Equipment = null;
                return null;
            }
        }    

        public bool ReadyForTest(out string dut_SN, out string dut_FW)
        {
            dut_SN = "";
            dut_FW = "";
            string TestStartTime = myDataIO.GetCurrTime().ToString("yyyy/MM/dd HH:mm:ss");
            try
            {
                supply = null;
                tempControl = null;
                attennuator = null;
                foreach (string key in usedEquipments.Keys)
                {
                    if (key == "POWERSUPPLY")
                    {
                        supply = (PowerSupply)this.usedEquipments[key];
                    }

                    if (key == "THERMOCONTROLLER")
                    {
                        tempControl = (Thermocontroller)this.usedEquipments[key];
                    }

                    if (key == "ATTENNUATOR")
                    {
                        attennuator = (Attennuator)this.usedEquipments[key];
                    } 
                }                

                // 获得芯片control 地址信息
                Log.SaveLogToTxt("Try to get chip control address from server.");
                string table = "GlobalManufactureChipsetControl";
                string expression = "select * from " + table + " where PID= " + GlobalParaByPN.ID + " order by ID";
                ChipControlByPN dataTable_ChipControlByPN = new ChipControlByPN(myDataIO.GetDataTable(expression, table));

                // 获得芯片初始化赋值信息
                Log.SaveLogToTxt("Try to get chip default value from server.");
                table = "GlobalManufactureChipsetInitialize";
                expression = "select * from " + table + " where PID= " + GlobalParaByPN.ID + " order by ID";
                ChipDefaultValueByPN dataTable_ChipDefaultValueByPN = new ChipDefaultValueByPN(myDataIO.GetDataTable(expression, table));

                // 获得模块配置EEPROM初始化赋值信息
                Log.SaveLogToTxt("Try to get EEPROM default value from server.");
                table = "TopoManufactureConfigInit";
                expression = "select * from " + table + " where PID= " + TestPlanParaByPN.ID + " order by ID";
                EEPROMDefaultValueByTestPlan dataTable_EEPROMDefaultValueByTestPlan = new EEPROMDefaultValueByTestPlan(myDataIO.GetDataTable(expression, table));

                // 获得模块系数表信息
                Log.SaveLogToTxt("Try to get module map from server.");
                table = "GlobalManufactureCoefficients";
                expression = "select * from " + table + " where PID= " + GlobalParaByPN.MCoefsID + " order by ID";
                DUTCoeffControlByPN dataTable_DUTCoeffControlByPN = new DUTCoeffControlByPN(myDataIO.GetDataTable(expression, table));

                dut = myFactory.CreateDUT(GlobalParaByPN.Family);
                dut.Initial(dataTable_ChipControlByPN, dataTable_ChipDefaultValueByPN, dataTable_EEPROMDefaultValueByTestPlan, dataTable_DUTCoeffControlByPN);

                Log.SaveLogToTxt("Enable full function of module.");
                supply.OutPutSwitch(true);
                if (!dut.FullFunctionEnable())
                {
                    Log.SaveLogToTxt("Faield to enable full function of module.");
                    return false;
                }

                //check SN
                for (int i = 0; i < 3; i++)
                {
                    dut_SN = dut.ReadSN();

                    if(Algorithm.CheckSerialNumberFormat(dut_SN))
                    {
                        Log.SaveLogToTxt("Read module' serial number is " + dut_SN);
                        break;
                    }

                    if (i == 2)
                    {
                        Log.SaveLogToTxt("Failed to read module' serial number.");
                        return false;
                    }
                }

                //check FW
                for (int i = 0; i < 3; i++)
                {
                    dut_FW = dut.ReadFW();

                    if (dut_FW.Length != 4)
                    {
                        Log.SaveLogToTxt("Failed to read module's firmware.");
                        return false;
                    }

                    if (dut_FW != "0000" && dut_FW != "FFFF")
                    {
                        if (TestPlanParaByPN.IsCheckFW)
                        {
                            if (dut_FW != TestPlanParaByPN.FwVersion)
                            {
                                Log.SaveLogToTxt("Module's firmware does not math.");
                                break;
                            }
                        }
                        break;
                    }
                }

                return true;
            }
            catch
            {
                Log.SaveLogToTxt("Failed to prepare to test. Please check network.");
                return false;
            }
        }

        public bool BeginTest()
        {
            try
            {
                Log.SaveLogToTxt("Begin to test...");
                Log.SaveLogToTxt("Power on all equipments.");
                foreach (string key in this.usedEquipments.Keys)
                {
                    this.usedEquipments[key].OutPutSwitch(true);
                }
                Log.SaveLogToTxt("Enable full function for module.");
                dut.FullFunctionEnable();

                Log.SaveLogToTxt("Try to get test condition from server.");
                testItemsObject = new Dictionary<string, ITest>();
                DataTable dataTable_Condition = this.BuildConditionTable();

                //create testdata table
                if (TestPlanParaByPN.ItemName.Contains("TR"))
                {
                    txoTable = new TxOTable();
                    rxoTable = new RxOTable();
                }
                else if (TestPlanParaByPN.ItemName.Contains("TX"))
                {
                    txoTable = new TxOTable();
                }
                else if (TestPlanParaByPN.ItemName.Contains("RX"))
                {
                    rxoTable = new RxOTable();
                }
                else
                {
                    Log.SaveLogToTxt("Test plan is not for function test.");
                }

                string beginTime = DateTime.Now.ToString();

                for (int row = 0; row < dataTable_Condition.Rows.Count; row++)//遍历测试环境条件
                {
                    if (txoTable != null)
                    {
                        //create testdata table new row, set the default value for new row, excepte for ID column
                        drOfTxOTable = txoTable.NewRow();
                        for (int i = 1; i < txoTable.Columns.Count; i++)
                        {
                            drOfTxOTable[i] = Algorithm.MyNaN;
                        }
                    }

                    if (rxoTable != null)
                    {
                        drOfRxOTable = rxoTable.NewRow();
                        for (int i = 1; i < rxoTable.Columns.Count; i++)
                        {
                            drOfRxOTable[i] = Algorithm.MyNaN;
                        }
                    }

                    DataRow dr = dataTable_Condition.Rows[row];
                    ConditionParaByTestPlan.SetValue(dr);

                    Log.SaveLogToTxt("Begin to config environment.");
                    Log.SaveLogToTxt("Temp = " + ConditionParaByTestPlan.Temp + " VCC = " + ConditionParaByTestPlan.VCC.ToString("f3") + " Channel = " + ConditionParaByTestPlan.Channel);
                    //myDataIO.WriterLog(ctrlType_Condition, SNID, "", StrStartTime, StrStartTime, Convert.ToSingle(StrCurrentTemp), Convert.ToSingle(StrCurrentVcc), CurrentChannel, false, out CurrentLogId);
                    if (!this.ConfigEnvironment(ConditionParaByTestPlan.Temp, ConditionParaByTestPlan.VCC, ConditionParaByTestPlan.Channel, 
                        ConditionParaByTestPlan.TempOffset, ConditionParaByTestPlan.TempWaitingTimes, GlobalParaByPN.OverLoadPoint))
                    {
                        Log.SaveLogToTxt("Failed to config environment.");
                        return false;
                    }
                    ConditionParaByTestPlan.LastTemp = ConditionParaByTestPlan.Temp;

                    Log.SaveLogToTxt("Try to get test model list under this condition.");
                    string table = "TopoTestModel";
                    string expression = "select B.ItemName,A.*,C.* , A.ID AS TestmodelID,c.ItemName as AppType from  TopoTestModel A,GlobalAllTestModelList B,GlobalAllAppModelList C where C.ID=B.PID AND A.PID=" 
                        + ConditionParaByTestPlan.ID + " and A.IgnoreFlag=0  AND A.GID=B.ID order by SEQ ASC";
                    DataTable dataTable_TestItems = myDataIO.GetDataTable(expression, table);// 获得TestModelList
                    DataTable dataTable_TestItemsPara = GetCurrentConditionPrameter(dataTable_TestItems);
                    if (ConditionParaByTestPlan.CtrlType == 2)
                    {
                        supply.OutPutSwitch(false);
                        supply.OutPutSwitch(true);
                        dut.FullFunctionEnable();
                    }
                    

                    for (int i = 0; i < dataTable_TestItems.Rows.Count; i++)// 遍历Condition中的TestModel
                    {
                        
                        DataRow dr_TestItem = dataTable_TestItems.Rows[i];
                        string ID_TestItem = dr_TestItem["TestmodelID"].ToString();
                        string name_TestItem = dr_TestItem["ItemName"].ToString();
                        string type_TestItem = dr_TestItem["AppType"].ToString();
                        bool isFailedBreak_TestItem = Convert.ToBoolean(dr_TestItem["Failbreak"]);                        

                        DataRow[] drs = dataTable_TestItemsPara.Select("TestmodelId='" + ID_TestItem + "'");
                        Dictionary<string, string> inPara_TestItem = new Dictionary<string, string>();
                        Log.SaveLogToTxt("Try to get test parameter for " + name_TestItem + ".");
                        for (int j = 0; j < drs.Length; j++)
                        {
                            inPara_TestItem.Add(drs[j]["ItemName"].ToString().Trim().ToUpper(), drs[j]["ItemValue"].ToString().Trim().ToUpper());
                            Log.SaveLogToTxt(drs[j]["ItemName"].ToString() + " " + drs[j]["ItemValue"].ToString());
                        }
                        Log.SaveLogToTxt("Test " + name_TestItem + "...");
                        if (!RunTestItem(name_TestItem, ID_TestItem, type_TestItem, inPara_TestItem))
                        {
                            //bool result_TestItem = false;
                        }
                        GC.Collect();
                    }

                    if (drOfTxOTable != null)
                    {
                        drOfTxOTable["Family"] = GlobalParaByPN.Family;
                        drOfTxOTable["PartNumber"] = GlobalParaByPN.PN;
                        drOfTxOTable["SerialNumber"] = TestPlanParaByPN.SN;
                        drOfTxOTable["Channel"] = ConditionParaByTestPlan.Channel;
                        drOfTxOTable["Temp"] = ConditionParaByTestPlan.Temp;
                        drOfTxOTable["Station"] = "TXO";
                        drOfTxOTable["Time"] = beginTime;
                        drOfTxOTable["Status"] = 0;
                        txoTable.Rows.Add(drOfTxOTable);
                    }

                    if (drOfRxOTable != null)
                    {
                        drOfRxOTable["Family"] = GlobalParaByPN.Family;
                        drOfRxOTable["PartNumber"] = GlobalParaByPN.PN;
                        drOfRxOTable["SerialNumber"] = TestPlanParaByPN.SN;
                        drOfRxOTable["Channel"] = ConditionParaByTestPlan.Channel;
                        drOfRxOTable["Temp"] = ConditionParaByTestPlan.Temp;
                        drOfRxOTable["Station"] = "RXO";
                        drOfRxOTable["Time"] = beginTime;
                        drOfRxOTable["Status"] = 0;
                        rxoTable.Rows.Add(drOfRxOTable);
                    }
                    drOfTxOTable = null;
                    drOfRxOTable = null;
                }

                //Save test data to xml file
                if (txoTable != null)
                {
                    txoTable.WriteXml("TxOData", FilePath.TxODataXml);
                }

                if (rxoTable != null)
                {
                    rxoTable.WriteXml("RxOData", FilePath.RxODataXml);
                }
                txoTable = null;
                rxoTable = null;

                foreach (string key in testItemsObject.Keys)
                {
                    if (!testItemsObject[key].SaveTestData())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                Log.SaveLogToTxt("Failed to test.");
                return false;
            }
        }

        public bool ParallelBeginTest()
        {
            try
            {
                Log.SaveLogToTxt("Begin to test...");
                Log.SaveLogToTxt("Power on all equipments.");
                foreach (string key in this.usedEquipments.Keys)
                {
                    this.usedEquipments[key].OutPutSwitch(true);
                }
                Log.SaveLogToTxt("Enable full function for module.");
                dut.FullFunctionEnable();

                Log.SaveLogToTxt("Try to get test condition from server.");
                testItemsObject = new Dictionary<string, ITest>();
                DataTable dataTable_Condition = this.BuildConditionTable();

                //create testdata table
                if (TestPlanParaByPN.ItemName.Contains("TR"))
                {
                    txoTable = new TxOTable();
                    rxoTable = new RxOTable();
                }
                else if (TestPlanParaByPN.ItemName.Contains("TX"))
                {
                    txoTable = new TxOTable();
                }
                else if (TestPlanParaByPN.ItemName.Contains("RX"))
                {
                    rxoTable = new RxOTable();
                }
                else
                {
                    Log.SaveLogToTxt("Test plan is not for function test.");
                }

                string beginTime = DateTime.Now.ToString();

                for (int row = 0; row < dataTable_Condition.Rows.Count; row++)//遍历测试环境条件
                {
                    if (txoTable != null)
                    {
                        //create testdata table new row, set the default value for new row, excepte for ID column
                        drOfTxOTable = txoTable.NewRow();
                        for (int i = 1; i < txoTable.Columns.Count; i++)
                        {
                            drOfTxOTable[i] = Algorithm.MyNaN;
                        }                        
                    }

                    if (rxoTable != null)
                    {
                        drOfRxOTable = rxoTable.NewRow();
                        for (int i = 1; i < rxoTable.Columns.Count; i++)
                        {
                            drOfRxOTable[i] = Algorithm.MyNaN;
                        }
                    }

                    DataRow dr = dataTable_Condition.Rows[row];
                    ConditionParaByTestPlan.SetValue(dr);

                    Log.SaveLogToTxt("Begin to config environment.");
                    Log.SaveLogToTxt("Temp = " + ConditionParaByTestPlan.Temp + " VCC = " + ConditionParaByTestPlan.VCC.ToString("f3") + " Channel = " + ConditionParaByTestPlan.Channel);
                    //myDataIO.WriterLog(ctrlType_Condition, SNID, "", StrStartTime, StrStartTime, Convert.ToSingle(StrCurrentTemp), Convert.ToSingle(StrCurrentVcc), CurrentChannel, false, out CurrentLogId);
                    if (!this.ConfigEnvironment(ConditionParaByTestPlan.Temp, ConditionParaByTestPlan.VCC, ConditionParaByTestPlan.Channel,
                        ConditionParaByTestPlan.TempOffset, ConditionParaByTestPlan.TempWaitingTimes, GlobalParaByPN.OverLoadPoint))
                    {
                        Log.SaveLogToTxt("Failed to config environment.");
                        return false;
                    }
                    ConditionParaByTestPlan.LastTemp = ConditionParaByTestPlan.Temp;

                    Log.SaveLogToTxt("Try to get test model list under this condition.");
                    string table = "TopoTestModel";
                    string expression = "select B.ItemName,A.*,C.* , A.ID AS TestmodelID,c.ItemName as AppType from  TopoTestModel A,GlobalAllTestModelList B,GlobalAllAppModelList C where C.ID=B.PID AND A.PID="
                        + ConditionParaByTestPlan.ID + " and A.IgnoreFlag=0  AND A.GID=B.ID order by SEQ ASC";
                    DataTable dataTable_TestItems = myDataIO.GetDataTable(expression, table);// 获得TestModelList
                    DataTable dataTable_TestItemsPara = GetCurrentConditionPrameter(dataTable_TestItems);
                    if (ConditionParaByTestPlan.CtrlType == 2)
                    {
                        supply.OutPutSwitch(false);
                        supply.OutPutSwitch(true);
                        dut.FullFunctionEnable();
                        //parallel test for FMT
                        Parallel.For(0, dataTable_TestItems.Rows.Count, (int i, ParallelLoopState pls) =>// 遍历Condition中的TestModel
                        { 
                            DataRow dr_TestItem = dataTable_TestItems.Rows[i];
                            string ID_TestItem = dr_TestItem["TestmodelID"].ToString();
                            string name_TestItem = dr_TestItem["ItemName"].ToString();
                            string type_TestItem = dr_TestItem["AppType"].ToString();
                            bool isFailedBreak_TestItem = Convert.ToBoolean(dr_TestItem["Failbreak"]);

                            DataRow[] drs = dataTable_TestItemsPara.Select("TestmodelId='" + ID_TestItem + "'");
                            Dictionary<string, string> inPara_TestItem = new Dictionary<string, string>();
                            Log.SaveLogToTxt("Try to get test parameter for " + name_TestItem + ".");
                            for (int j = 0; j < drs.Length; j++)
                            {
                                inPara_TestItem.Add(drs[j]["ItemName"].ToString().Trim().ToUpper(), drs[j]["ItemValue"].ToString().Trim().ToUpper());
                                Log.SaveLogToTxt(drs[j]["ItemName"].ToString() + " " + drs[j]["ItemValue"].ToString());
                            }
                            Log.SaveLogToTxt("Test " + name_TestItem + "...");
                            if (!RunTestItem(name_TestItem, ID_TestItem, type_TestItem, inPara_TestItem))
                            {
                                //bool result_TestItem = false;
                                //pls.Break();
                            }
                            //GC.Collect();
                        });
                    }
                    else
                    {
                        //onebyone test for adjust
                        for (int i = 0; i < dataTable_TestItems.Rows.Count; i++)// 遍历Condition中的TestModel
                        {

                            DataRow dr_TestItem = dataTable_TestItems.Rows[i];
                            string ID_TestItem = dr_TestItem["TestmodelID"].ToString();
                            string name_TestItem = dr_TestItem["ItemName"].ToString();
                            string type_TestItem = dr_TestItem["AppType"].ToString();
                            bool isFailedBreak_TestItem = Convert.ToBoolean(dr_TestItem["Failbreak"]);

                            DataRow[] drs = dataTable_TestItemsPara.Select("TestmodelId='" + ID_TestItem + "'");
                            Dictionary<string, string> inPara_TestItem = new Dictionary<string, string>();
                            Log.SaveLogToTxt("Try to get test parameter for " + name_TestItem + ".");
                            for (int j = 0; j < drs.Length; j++)
                            {
                                inPara_TestItem.Add(drs[j]["ItemName"].ToString(), drs[j]["ItemValue"].ToString());
                                Log.SaveLogToTxt(drs[j]["ItemName"].ToString() + " " + drs[j]["ItemValue"].ToString());
                            }
                            Log.SaveLogToTxt("Test " + name_TestItem + "...");
                            if (!RunTestItem(name_TestItem, ID_TestItem, type_TestItem, inPara_TestItem))
                            {
                                //bool result_TestItem = false;
                            }
                            GC.Collect();
                        }
                    }

                    if (drOfTxOTable != null)
                    {
                        drOfTxOTable["Family"] = GlobalParaByPN.Family;
                        drOfTxOTable["PartNumber"] = GlobalParaByPN.PN;
                        drOfTxOTable["SerialNumber"] = TestPlanParaByPN.SN;
                        drOfTxOTable["Channel"] = ConditionParaByTestPlan.Channel;
                        drOfTxOTable["Temp"] = ConditionParaByTestPlan.Temp;
                        drOfTxOTable["Station"] = "TXO";
                        drOfTxOTable["Time"] = beginTime;
                        drOfTxOTable["Status"] = 0;
                        txoTable.Rows.Add(drOfTxOTable);
                    }

                    if (drOfRxOTable != null)
                    {
                        drOfRxOTable["Family"] = GlobalParaByPN.Family;
                        drOfRxOTable["PartNumber"] = GlobalParaByPN.PN;
                        drOfRxOTable["SerialNumber"] = TestPlanParaByPN.SN;
                        drOfRxOTable["Channel"] = ConditionParaByTestPlan.Channel;
                        drOfRxOTable["Temp"] = ConditionParaByTestPlan.Temp;
                        drOfRxOTable["Station"] = "RXO";
                        drOfRxOTable["Time"] = beginTime;
                        drOfRxOTable["Status"] = 0;
                        rxoTable.Rows.Add(drOfRxOTable);
                    }                    
                    drOfTxOTable = null;
                    drOfRxOTable = null;
                }

                //Save test data to xml file
                if (txoTable != null)
                {
                    txoTable.WriteXml("TxOData", FilePath.TxODataXml);
                }

                if (rxoTable != null)
                {
                    rxoTable.WriteXml("RxOData", FilePath.RxODataXml);
                }
                txoTable = null;
                rxoTable = null;               

                foreach (string key in testItemsObject.Keys)
                {
                    if (!testItemsObject[key].SaveTestData())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                Log.SaveLogToTxt(ex.StackTrace);
                Log.SaveLogToTxt("Failed to test.");
                return false;
            }
        }

        private DataTable BuildConditionTable()
        {
            string table = "TopoTestControl";
            string expression = "select * from " + table + " where PID=" + TestPlanParaByPN.ID + " and IgnoreFlag=0" + " order by SEQ ASC";
            DataTable dt = myDataIO.GetDataTable(expression, table);// 获得环境的DataTable

            //---------------------------组件全新的DtMyConditionDataTable
            DataTable conditionTable = dt.Clone();
            for (int row = 0; row < dt.Rows.Count; row++)
            {                
                if (dt.Rows[row]["Channel"].ToString() == "0")
                {
                    for (int i = 0; i < GlobalParaByPN.TotalChCount; i++)
                    {
                        DataRow dr = conditionTable.NewRow();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            dr[j] = dt.Rows[row][j];
                        }
                        dr["Channel"] = i + 1;
                        conditionTable.Rows.Add(dr);
                    }
                }
                else
                {
                    DataRow dr = conditionTable.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dr[i] = dt.Rows[row][i];
                    }
                    conditionTable.Rows.Add(dr);
                }                
            }
            
            //---------------------------给DtMyConditionDataTable中的Condition 排序 
            for (int row = 0; row < conditionTable.Rows.Count; row++)
            {
                conditionTable.Rows[row]["SEQ"] = row + 1;
            }

            return conditionTable;
        }

        private bool ConfigEnvironment(double temp, double voltage, byte channel, double tempOffset, int tempWaitTime, float RxOverload)
        {
            bool result = true;
            if (tempControl != null)
            {
                if (temp != ConditionParaByTestPlan.LastTemp)
                {
                    result = result && ConfigTemp(temp, tempOffset);
                    Log.SaveLogToTxt("keep temp " + tempWaitTime + " s.");
                    Thread.Sleep(tempWaitTime * 1000);
                }      
            }

            if (attennuator != null)
            {
                result = result && attennuator.SetAllChannnel_RxOverLoad(RxOverload);
                Log.SaveLogToTxt("set rx overload");
            }

            foreach (string key in this.usedEquipments.Keys)
            {
                this.usedEquipments[key].ChangeChannel(channel);
            }

            if (!supply.ConfigVoltageCurrent(voltage))
            {
                return false;
            }
            return result;
        }

        private bool ConfigTemp(double temp, double tempOffset)
        {
            int i;
            temp = temp + tempOffset;

            tempControl.SetPointTemp(temp);
            double CurrentTemp = Convert.ToDouble(tempControl.ReadCurrentTemp());
            i = 0;            
            while (Math.Abs(CurrentTemp - temp) > 0.5)
            {
                Thread.Sleep(2000);
                CurrentTemp = Convert.ToDouble(tempControl.ReadCurrentTemp());
                i++;
                if (i > 100)
                {
                    Log.SaveLogToTxt("Failed to config temp.");
                    //MessageBox.Show("无法调整到当前温度");
                    return false;
                }
            }
            
            return true;
        }

        private DataTable GetCurrentConditionPrameter(DataTable dtTestModelList)
        {
            DataTable dt = new DataTable();
            if (dtTestModelList.Rows.Count > 0)
            {
                string SS = "";
                for (int i = 0; i < dtTestModelList.Rows.Count; i++)
                {
                    SS += dtTestModelList.Rows[i]["id"].ToString();
                    if (i < dtTestModelList.Rows.Count - 1)
                    {
                        SS += ",";
                    }
                }

                //String StrMaxid=dtTestModelList.Rows[dtTestModelList.Rows.Count-1]["ID"].ToString();
                //String StrMinid = dtTestModelList.Rows[0]["ID"].ToString();
                string Str = "SELECT  TopoTestParameter.PID as TestmodelId,* FROM TopoTestParameter,GlobalTestModelParamterList Where  TopoTestParameter.GID=GlobalTestModelParamterList.ID and  (TopoTestParameter.PID in ( " + SS + "))";
                dt = myDataIO.GetDataTable(Str, "TopoTestParameter");// 获得环境的DataTable
            }
            return dt;
        }

        private bool RunTestItem(string name, string ID, string type, Dictionary<string, string> inPara)
        {
            name = name.Trim();
            if (!this.testItemsObject.Keys.Contains(name))
            {
                ITest testItem = myFactory.CreateTestItem(name);
                this.testItemsObject.Add(name, testItem);  
            }

            Dictionary<string, double> result = this.testItemsObject[name].BeginTest(dut, this.usedEquipments, inPara);

            if (result == null)
            {
                return false;
            }

            foreach(string key in result.Keys)
            {
                if (drOfTxOTable != null)
                {
                    if (txoTable.Columns.Contains(key))
                    {
                        drOfTxOTable[key] = result[key];
                    }                    
                }

                if (drOfRxOTable != null)
                {
                    if (rxoTable.Columns.Contains(key))
                    {
                        drOfRxOTable[key] = result[key];
                    }
                }                
            }
            return Convert.ToBoolean(result["Result"]);
        } 
    }
}
