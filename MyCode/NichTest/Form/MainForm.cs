using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;

namespace NichTest
{
    public partial class MainForm : Form
    {
        private string address_IP;
        private ForUser user;
        private TextBox[] myTextBox;
        private DataTable dataTable_Family; 
        private int ID_PN;
        private int ID_Family;
        private ConfigXmlIO myXml;
        private DataIO myDataIO;
        private CancellationTokenSource tokenSource;
        private LogForm logForm;

        /// <summary>
        /// 禁止使用还原按钮的方法
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch(m.WParam.ToInt32())
            {
                //click restore button
                case 0xF120:
                    m.WParam = (IntPtr)0xF030;
                    break;

                //click title panel
                case 0xF122:
                    m.WParam = IntPtr.Zero;
                    break;
            }
            base.WndProc(ref m);
        }

        public MainForm()
        {
            try
            {
                InitializeComponent();

                //FilePath.LogFile = Application.StartupPath + @"\Log\" + "Initial_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                user = new ForUser();

                this.RenameTextBox();
                this.LoadConfigXmlInfo();
                if (myXml.DataBaseUserLever == "1")
                {
                    if (MessageBox.Show("Do you want to Use Location Database?", "Database Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        OpenFileDialog openFileDialog1 = new OpenFileDialog();
                        openFileDialog1.InitialDirectory = "D:\\Patch";
                        openFileDialog1.Filter = "All files (*.accdb)|*.accdb|All files (*.*)|*.* ";
                        openFileDialog1.FilterIndex = 1;
                        openFileDialog1.RestoreDirectory = true;
                        myXml.DatabaseType = "LocationDatabase";
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            myXml.DatabasePath = openFileDialog1.FileName;
                        }
                    }
                    else
                    {
                        myXml.DatabaseType = "SqlDatabase";
                    }
                }
                else
                {
                    myXml.DatabaseType = "SqlDatabase";
                }
            }
            catch
            {
                var result = MessageBox.Show("缺少Config文件，请确认，并重启软件。", "错误", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        private void RenameTextBox()
        {
            myTextBox = new TextBox[14];
            myTextBox[0] = this.txtPMOffsetCh1;
            myTextBox[1] = this.txtPMOffsetCh2;
            myTextBox[2] = this.txtPMOffsetCh3;
            myTextBox[3] = this.txtPMOffsetCh4;
            myTextBox[4] = this.txtLigSourceCh1;
            myTextBox[5] = this.txtLigSourceCh2;
            myTextBox[6] = this.txtLigSourceCh3;
            myTextBox[7] = this.txtLigSourceCh4;
            myTextBox[8] = this.txtLigSourceERCh1;
            myTextBox[9] = this.txtLigSourceERCh2;
            myTextBox[10] = this.txtLigSourceERCh3;
            myTextBox[11] = this.txtLigSourceERCh4;
            myTextBox[12] = this.txtVCCOffset;
            myTextBox[13] = this.txtICCOffset;
        }

        private void LoadConfigXmlInfo()
        {
            try
            {
                FilePath.ConfigXml = Application.StartupPath + "\\Config.xml";
                myXml = new ConfigXmlIO(FilePath.ConfigXml);

                //-------------------ScopeOffset
                string[] settingArray = myXml.ScopeOffset.Split(',');
                for (int i = 0; i < 4; i++)
                {
                    myTextBox[i].Text = settingArray[i];                    
                }

                //------------------AttOffset
                settingArray = myXml.AttennuatorOffset.Split(',');
                for (int i = 0; i < 4; i++)
                {
                    myTextBox[i + 4].Text = settingArray[i];
                }

                //------------------LightSource
                settingArray = myXml.LightSourceEr.Split(',');
                for (int i = 0; i < 4; i++)
                {
                    myTextBox[i + 8].Text = settingArray[i];
                }

                myTextBox[12].Text = myXml.VccOffset;
                myTextBox[13].Text = myXml.IccOffset;
            }
            catch(Exception ex)
            {
                var result = MessageBox.Show("导入Config文件出错，请确认，并重启软件。", "错误", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        private void SaveConfigXmlInfo()
        {
            try
            {
                //----------------Scope                
                string StrScopeOffsetArray = myTextBox[0].Text;
                for (int i = 1; i < 4; i++)
                {
                    StrScopeOffsetArray += "," + myTextBox[i].Text.ToString();
                }
                myXml.ScopeOffset = StrScopeOffsetArray;

                //---------------------AttOffset                
                string StrAttOffsetArray = myTextBox[4].Text;
                for (int i = 1; i < 4; i++)
                {
                    StrAttOffsetArray += "," + myTextBox[i + 4].Text.ToString();
                }
                myXml.AttennuatorOffset = StrAttOffsetArray;

                //-----------------------------LightSourceER  
                string StrErArray = myTextBox[8].Text;

                for (int i = 1; i < 4; i++)
                {
                    StrErArray += "," + myTextBox[i + 8].Text.ToString();
                }
                myXml.LightSourceEr = StrErArray;
                GlobalParaByPN.OpticalSourceERArray = StrErArray;
                //---------------------IccOffset & PsOffset           

                double Vccoffset = Convert.ToDouble(myTextBox[12].Text);
                // pflowControl.Iccoffset = Convert.ToDouble(IccOffsetArray[0]);
                myXml.VccOffset = Vccoffset.ToString();
                string icc = myTextBox[13].Text.Trim();
                if (icc == "" || icc == null)
                {
                    icc = "0";
                }
                //pflowControl.pGlobalParameters.StrEvbCurrent = IccOffset.Text.Trim();
                myXml.IccOffset = icc;
            }
            catch
            {
                return;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                //load dataGridViewTestData
                this.dataGridViewTestData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                this.dataGridViewTestData.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                this.dataGridViewTestData.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                this.dataGridViewTestData.AllowUserToAddRows = false;
                TestData dt = new TestData();
                FilePath.TestDataXml = Application.StartupPath + @"\TestData.xml";                
                dt.ReadXml("TestData", FilePath.TestDataXml);
                dt.Columns.Remove("Family");
                dt.Columns.Remove("Current");
                dt.Columns.Remove("Temp");
                dt.Columns.Remove("Time");

                this.dataGridViewTestData.DataSource = dt;
                this.dataGridViewTestData.Columns["Result"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                this.btnInitial.Enabled = false;
                this.btnStart.Enabled = false;

                //get Production Family
                string[] member = user.GetProductionFamily(myXml, ref myDataIO, ref dataTable_Family);
                this.comboBoxFamily.Items.Clear();
                if (member == null)
                {
                    var result = MessageBox.Show("不能连接服务器，是否继续？", "错误", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result == DialogResult.No)
                    {
                        Application.Exit();
                    }
                    return;
                }
                foreach (string one in member)
                {
                    this.comboBoxFamily.Items.Add(one);
                }
                //check IP address   
                address_IP = user.GetIP();
                if (address_IP == "")
                {                
                    this.comboBoxFamily.Text = "";
                    this.comboBoxFamily.Enabled = false;
                }
            }
            catch
            {
                var result = MessageBox.Show("连接服务器或导入Config文件出错，请确认，并重启软件。", "错误", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }  

        private delegate void UpdateControl(bool taskResult);

        private void btnInitial_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.comboBoxTestPlan.Text == "")
                {
                    MessageBox.Show("Select Production Name");
                    return;
                }
                this.btnInitial.Enabled = false;
                this.btnStart.Enabled = false;
                this.btnTraceView.Enabled = true;
                this.groupBoxCalibratioon.Enabled = false;
                this.groupBoxConfig.Enabled = false;
                this.groupBoxStatus.BackColor = Color.Yellow;
                this.labelStatus.Text = "正在初始化...";

                if(logForm!=null)
                {
                    logForm.Close();
                    logForm.Dispose();
                }

                Log.ReportRecord += new Report(UpdateLogForm);
                logForm = new LogForm();
                logForm.Show();

                FilePath.LogFile = Application.StartupPath + @"\Log\" + "Initial_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                Log.SaveLogToTxt("Begin to initial...");

                this.SaveConfigXmlInfo();
                Log.SaveLogToTxt("Saved calibration data to config file.");
                //创建存放眼图和数据的文件夹位置
                string time = DateTime.Now.ToString("yyyy-MM-dd");
                string[] folderPath = new string[6];
                folderPath[0] = Application.StartupPath + @"\EyeDiagram\" + this.comboBoxFamily.Text.ToUpper() + "\\" + this.comboBoxPN.Text.ToUpper() + "\\" + this.comboBoxTestPlan.Text.ToUpper() + "\\" + time + "\\OptEyeDiagram\\";
                folderPath[1] = Application.StartupPath + @"\EyeDiagram\" + this.comboBoxFamily.Text.ToUpper() + "\\" + this.comboBoxPN.Text.ToUpper() + "\\" + this.comboBoxTestPlan.Text.ToUpper() + "\\" + time + "\\ElecEyeDiagram\\";
                folderPath[2] = Application.StartupPath + @"\EyeDiagram\" + this.comboBoxFamily.Text.ToUpper() + "\\" + this.comboBoxPN.Text.ToUpper() + "\\" + this.comboBoxTestPlan.Text.ToUpper() + "\\" + time + "\\Polarity\\";
                folderPath[3] = Application.StartupPath + @"\Log\";
                folderPath[4] = Application.StartupPath + @"\TestData\xml\";
                folderPath[5] = Application.StartupPath + @"\TestData\backup\";
                user.CreatFolderPath(folderPath);

                //上传以前未上传的测试数据到数据库
                DirectoryInfo theFolder = new DirectoryInfo(FolderPath.TestDataPath);
                foreach (FileInfo nextFile in theFolder.GetFiles())
                {
                    UploadTestData(nextFile.FullName);
                }

                tokenSource = new CancellationTokenSource();
                Task<bool> task;
                if (this.checkBoxParallelTest.Checked)
                {
                    task = Task.Factory.StartNew<bool>(() => (user.ParallelInitial()), tokenSource.Token);
                }
                else
                {
                    task = Task.Factory.StartNew<bool>(() => (user.Initial()), tokenSource.Token);
                }
                Task cwt = task.ContinueWith(t =>
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new UpdateControl(delegate 
                            {
                                this.UpdateControlAfterInitial(task.Result);

                            }), task.Result);
                    }
                    else
                    {
                        this.UpdateControlAfterInitial(task.Result);
                    }                    
                });
            }
            catch(Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                Log.SaveLogToTxt("Failed to initial. Please check equipments infomation.");
                this.btnInitial.Enabled = true;
                this.btnTraceView.Enabled = false;
                this.groupBoxCalibratioon.Enabled = true;
                this.groupBoxConfig.Enabled = true;
                this.groupBoxStatus.BackColor = Color.LightCoral;
            }           
        }

        private void UpdateLogForm()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate 
                { 
                    if (logForm == null || logForm.IsDisposed)
                    {
                        logForm = new LogForm();
                    }
                    logForm.ChangeText();
                }
                ));
            }
            else
            {
                if (logForm == null || logForm.IsDisposed)
                {
                    logForm = new LogForm();
                }
                logForm.ChangeText();
            }
        }

        private void UpdateControlAfterInitial(bool initialResult)
        {
            if (initialResult == true)
            {          
                this.btnStart.Enabled = true;
                this.groupBoxStatus.BackColor = Color.Lime;
            }
            else
            {
                this.groupBoxStatus.BackColor = Color.LightCoral;
            }
            this.btnInitial.Enabled = true;
            this.btnStart.Enabled = true;
            this.btnTraceView.Enabled = false;
            this.groupBoxCalibratioon.Enabled = true;
            this.groupBoxConfig.Enabled = true;
            this.labelStatus.Text = "初始化完成";
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnInitial.Enabled = false;
                this.btnStart.Enabled = false;
                this.btnTraceView.Enabled = true;
                this.groupBoxCalibratioon.Enabled = false;
                this.groupBoxConfig.Enabled = false;
                this.groupBoxStatus.BackColor = Color.Yellow;
                this.labelStatus.Text = "正在测试...";
                Thread.Sleep(50);
                
                string SN, FW;
                string path = "ReadyForTest" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                FilePath.LogFile = Application.StartupPath + @"\Log\" + path;
                
                if (user.ReadyForTest(out SN, out FW))
                {
                    this.labelSN.Text = SN;
                    this.labelFW.Text = FW;

                    string logFileName = SN + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    FilePath.LogFile = FolderPath.LogPath + logFileName;
                    string rxoFileName = SN + "_RxO_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                    FilePath.RxODataXml = FolderPath.TestDataPath + rxoFileName;
                    string txoFileName = SN + "_TxO_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                    FilePath.TxODataXml = FolderPath.TestDataPath + txoFileName;

                    TestPlanParaByPN.SN = SN;
                                       
                    tokenSource = new CancellationTokenSource();
                    Task<bool> task;
                    if (this.checkBoxParallelTest.Checked)
                    {
                        task = Task.Factory.StartNew<bool>(() => (user.ParallelBeginTest()), tokenSource.Token);
                    }
                    else
                    {
                        task = Task.Factory.StartNew<bool>(() => (user.BeginTest()), tokenSource.Token);
                    }

                    Task cwt = task.ContinueWith(t => 
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new UpdateControl(delegate
                            {
                                //Save test data to server
                                DirectoryInfo theFolder = new DirectoryInfo(FolderPath.TestDataPath);
                                foreach (FileInfo nextFile in theFolder.GetFiles())
                                {
                                    UploadTestData(nextFile.FullName);
                                }

                                this.UpdateControlAfterTest(task.Result);
                            }), task.Result);
                        }
                        else
                        {
                            //Save test data to server
                            DirectoryInfo theFolder = new DirectoryInfo(FolderPath.TestDataPath);
                            foreach (FileInfo nextFile in theFolder.GetFiles())
                            {
                                UploadTestData(nextFile.FullName);
                            }
                            this.UpdateControlAfterTest(task.Result);
                        }
                    });
                }
                else
                {
                    this.labelStatus.Text = "测试完成";
                    this.btnInitial.Enabled = true;
                    this.btnStart.Enabled = true;
                    this.btnTraceView.Enabled = false;
                    this.groupBoxCalibratioon.Enabled = true;
                    this.groupBoxConfig.Enabled = true;
                    this.groupBoxStatus.BackColor = Color.LightCoral;
                }
            }
            catch
            {
                //Log.SaveLogToTxt("Failed to test. Please check communucation and test model config.");
                this.btnInitial.Enabled = true;
                this.btnStart.Enabled = true;
                this.groupBoxCalibratioon.Enabled = true;
                this.groupBoxConfig.Enabled = true;
                this.groupBoxStatus.BackColor = Color.LightCoral;
            }
        }

        private void UploadTestData(string xmlfile)
        {
            try
            {
                if(!xmlfile.Contains(".xml"))
                {
                    return;
                }

                //Save test data to server
                if (xmlfile.Contains("TxO"))
                {
                    TxOTable dtTxO = new TxOTable();
                    dtTxO.ReadXml("TxOData", xmlfile);
                    //save to excel
                    dtTxO.SaveTableToExcel(xmlfile.Replace("xml", "xls"));

                    Log.SaveLogToTxt("upload test data: " + xmlfile);
                    if (this.UploadTestDataToServer(dtTxO, "select * from my_databases.txo") == true)
                    {
                        try
                        {
                            File.Move(xmlfile, FolderPath.BackupTestDataPath + Path.GetFileName(xmlfile));
                        }
                        catch
                        {
                            MessageBox.Show("移动测试数据到备份文件夹失败，你可以继续测试。");
                        }
                    }
                    dtTxO = null;
                }

                if (xmlfile.Contains("RxO"))
                {
                    RxOTable dtRxO = new RxOTable();
                    dtRxO.ReadXml("RxOData", xmlfile);
                    //save to excel
                    dtRxO.SaveTableToExcel(xmlfile.Replace("xml", "xls"));

                    Log.SaveLogToTxt("upload test data: " + xmlfile);
                    if (this.UploadTestDataToServer(dtRxO, "select * from my_databases.rxo") == true)
                    {
                        try
                        {
                            File.Move(xmlfile, FolderPath.BackupTestDataPath + Path.GetFileName(xmlfile));
                        }
                        catch
                        {
                            MessageBox.Show("移动测试数据到备份文件夹失败，你可以继续测试。");
                        }
                    }
                    dtRxO = null;
                }
            }
            catch
            {
                MessageBox.Show("上传以前未上传的测试数据到数据库失败，请检测网络。当然你也可以离线测试。");
            }
        }

        private bool UploadTestDataToServer(DataTable table, string command)
        {
            try
            {
                if (table == null)
                {
                    return true;
                }

                Log.SaveLogToTxt("Upload test data to server...");
                string mysqlconCommand = "Database=my_databases;Data Source=localhost;User Id=root;Password=abc@123;pooling=false;CharSet=utf8;port=3306";
                MySqlConnection mycon = new MySqlConnection();
                mycon.ConnectionString = mysqlconCommand;
                MySqlTransaction tr;
                mycon.Open();
                tr = mycon.BeginTransaction(IsolationLevel.RepeatableRead);
                try
                {
                    //string mysqlCommand = "select * from my_databases.quickcheck_testdata";///
                    string mysqlCommand = command;
                    MySqlCommand cmd = new MySqlCommand(mysqlCommand, mycon);
                    MySqlDataAdapter mda = new MySqlDataAdapter(cmd);
                    MySqlCommandBuilder cb = new MySqlCommandBuilder(mda);

                    mda.SelectCommand.Transaction = tr;

                    if (table.GetErrors() != null)
                    {
                        //upload test data to mysql database
                        mda.Update(table);
                        tr.Commit();
                        Log.SaveLogToTxt("Upload test data to server sucessfully.");
                        return true;
                    }
                    Log.SaveLogToTxt("Test data is abnormal.");
                    return false;
                }
                catch
                {
                    tr.Rollback();//回滚（出错的时候)
                    Log.SaveLogToTxt("Failed to upload test data to MySQL.");
                    return false;
                }
                finally
                {
                    mycon.Close();
                }
            }
            catch (Exception ex)
            {
                Log.SaveLogToTxt(ex.Message);
                Log.SaveLogToTxt("Failed to communicate server.");
                return false;
            }
        }

        private void UpdateControlAfterTest(bool testResult)
        {
            if (testResult == true)
            {
                TestData dt = new TestData();
                dt.ReadXml("TestData", FilePath.TestDataXml);
                dt.Columns.Remove("Family");
                dt.Columns.Remove("Current");
                dt.Columns.Remove("Temp");
                dt.Columns.Remove("Time");
                this.dataGridViewTestData.DataSource = dt;
                this.dataGridViewTestData.Columns["Result"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                this.groupBoxStatus.BackColor = Color.Lime;
            }
            else
            {
                this.groupBoxStatus.BackColor = Color.LightCoral;
            }
            this.btnInitial.Enabled = true;
            this.btnStart.Enabled = true;
            this.btnTraceView.Enabled = false;
            this.groupBoxCalibratioon.Enabled = true;
            this.groupBoxConfig.Enabled = true;
            this.labelStatus.Text = "测试完成";
        }

        private void comboBoxFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.comboBoxPN.Items.Clear();
                this.comboBoxTestPlan.Items.Clear();
                this.comboBoxStation.Text = "";
                //Get the ID of Current Product Family 
                string productFamlily = this.comboBoxFamily.SelectedItem.ToString();
                DataRow[] dr = dataTable_Family.Select("ItemName='" + productFamlily + "'");
                ID_Family = Convert.ToInt32(dr[0]["ID"].ToString());

                //Fit ProductionName Combox
                string expression = "Select* from GlobalProductionName where PID=" + ID_Family + " and IgnoreFlag='false' order by id";
                string dataBaseTable = "GlobalProductionName";
                DataTable dt = myDataIO.GetDataTable(expression, dataBaseTable);
                this.comboBoxPN.Items.Clear();
                this.comboBoxPN.Text = "";
                //this.comboBoxPN.Refresh();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    comboBoxPN.Items.Add(dt.Rows[i]["PN"].ToString());
                }
                this.comboBoxTestPlan.Items.Clear();
            }
            catch
            {
                MessageBox.Show("Faied to get production name list.");   
            }
        }

        private void comboBoxPN_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.comboBoxTestPlan.Items.Clear();

                if (this.comboBoxFamily.Text != null)
                {
                     
                    string productPN = this.comboBoxPN.SelectedItem.ToString();
                    string expression = "Select* from GlobalProductionName where GlobalProductionName.pid=" + ID_Family + " and GlobalProductionName.IgnoreFlag='false' and GlobalProductionName.PN='" + productPN + "' order by GlobalProductionName.id";
                    DataTable dataTable_PN = myDataIO.GetDataTable(expression, "GlobalProductionName");
                    DataRow[] dr = dataTable_PN.Select("PN='" + productPN + "'");
                    ID_PN = Convert.ToInt32(dr[0]["ID"].ToString());
                    GlobalParaByPN.SetValue(dataTable_PN, productPN);
                    GlobalParaByPN.Family = this.comboBoxFamily.Text;

                    expression = "Select* from TopoTestPlan where IgnoreFlag='false' and PID=" + ID_PN + " order by id";
                    DataTable dt = myDataIO.GetDataTable(expression, "GlobalProductionName");
                    this.comboBoxTestPlan.Items.Clear();
                    this.comboBoxTestPlan.Text = "";
                    this.comboBoxStation.Text = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        comboBoxTestPlan.Items.Add(dt.Rows[i]["ItemName"].ToString());
                    }
                }
            }            
            catch
            {
                MessageBox.Show("Faied to get test plan list."); 
            }
        }

        private void comboBoxTestPlan_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.comboBoxPN.Text != "")
                {
                    //Get the ID of Current Testplan                    
                    string testPlan = this.comboBoxTestPlan.SelectedItem.ToString();
                    string expression = "Select* from TopoTestPlan where TopoTestPlan.pid=" + ID_PN + " and TopoTestPlan.IgnoreFlag='false' and TopoTestPlan.ItemName='" + testPlan + "' order by TopoTestPlan.id";
                    DataTable dt = myDataIO.GetDataTable(expression, "GlobalProductionName");
                    int ID_TestPlan = Convert.ToInt32(dt.Rows[0]["id"]);

                    user.GetTestPlanParaByPN(ID_TestPlan);
                    user.GetSpec(ID_TestPlan);
                    this.comboBoxStation.Text = "";
                }
            }
            catch
            {
                MessageBox.Show("Faied to get parameters and spec."); 
            }
        }

        private void comboBoxStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxPN.Text != "")
            {
                string station = this.comboBoxStation.SelectedItem.ToString();
                GlobalParaByPN.Station = station;

                this.btnInitial.Enabled = true;
                MessageBox.Show("确认是该站点:" + station + "?");
            }            
        }

        private void HelpHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("已授权，欢迎使用。");       
        }

        private void toolStripBtnStop_Click(object sender, EventArgs e)
        {
            try
            {
                tokenSource.Cancel();
            }
            catch
            {
                return;
            }
        }

        private void testPlanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://10.160.46.72:8080/");
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            SearchForm sf = new SearchForm();
            sf.Show(); 
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TestData dt = new TestData();
                dt.ReadXml("TestData", FilePath.TestDataXml);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("无测试记录！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string folderPath = Application.StartupPath + @"\TestData\";
                FolderBrowserDialog dilog = new FolderBrowserDialog();
                dilog.Description = "请选择文件夹";
                var result = dilog.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    folderPath = dilog.SelectedPath;
                }

                string fileExcelPath = folderPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FilePath.SaveTableToExcel(dt, fileExcelPath);
            }
            catch
            {
                MessageBox.Show("Save test data failed.");
            }
        }

        private void CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalculatorForm cf = new CalculatorForm();
            cf.Show();
        }

        private void MineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MineClearanceForm mc = new MineClearanceForm();
            mc.Show();
        }

        private void btnTraceView_Click(object sender, EventArgs e)
        {
            if (logForm == null || logForm.IsDisposed)
            {
                logForm = new LogForm();
                logForm.Show();
            }
            else
            {
                logForm.WindowState = FormWindowState.Normal;
                logForm.Show();
                logForm.Activate();
            }
        }

        private void ToolStripMenuItemClearLog_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("确定删除日志文件？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                string folder = Application.StartupPath + @"\Log\";
                FolderPath.ClearFolder(folder);
                MessageBox.Show("成功删除！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ToolStripMenuItemClearExcelFile_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("确定删除excel数据文件？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                string folder = Application.StartupPath + @"\TestData\xls\";
                FolderPath.ClearFolder(folder);
                MessageBox.Show("成功删除！");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ToolStripMenuItemClearXmlFile_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("确定删除xml数据文件？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                string folder = Application.StartupPath + @"\TestData\xml\";
                FolderPath.ClearFolder(folder);
                MessageBox.Show("成功删除！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ToolStripMenuItemClearEyeFile_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("确定删除眼图文件？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                string folder = Application.StartupPath + @"\EyeDiagram\";
                FolderPath.ClearFolder(folder);
                MessageBox.Show("成功删除！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ToolStripMenuItemClearBackup_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("确定删除备份数据？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                string folder = Application.StartupPath + @"\TestData\backup\";
                FolderPath.ClearFolder(folder);
                MessageBox.Show("成功删除！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ToolStripMenuItemClearAllData_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("确定删除所有数据文件？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                string[] folders = new string[5];
                folders[0] = Application.StartupPath + @"\Log\";
                folders[1] = Application.StartupPath + @"\EyeDiagram\";
                folders[2] = Application.StartupPath + @"\TestData\xml\";
                folders[3] = Application.StartupPath + @"\TestData\backup\";
                folders[4] = Application.StartupPath + @"\TestData\xls\";

                foreach (string folder in folders)
                {
                    FolderPath.ClearFolder(folder);
                }
                MessageBox.Show("成功删除！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ToolStripMenuItemLog_Click(object sender, EventArgs e)
        {
            if (this.btnTraceView.Enabled == true)
            {
                this.btnTraceView_Click(sender, e);
            }
        }
    }
}
