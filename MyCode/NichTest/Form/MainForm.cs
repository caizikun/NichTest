using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace NichTest
{
    public partial class MainForm : Form
    {
        string address_IP;

        private ForUser user;

        private TextBox[] myTextBox;

        private DataTable dataTable_Family;        

        private int ID_PN;

        private int ID_Family;

        private ConfigXmlIO myXml;

        private DataIO myDataIO;

        CancellationTokenSource tokenSource;

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

                FilePath.LogFile = Application.StartupPath + @"\Log\" + "Initial_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                user = new ForUser();

                this.RenameTextBox();
                Log.SaveLogToTxt("Load config...");
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
                Log.SaveLogToTxt("Done.");
            }
            catch
            {
                Log.SaveLogToTxt("Filed to load config.");
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
                Log.SaveLogToTxt("form load error");
                Log.SaveLogToTxt(ex.ToString());
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
                Log.SaveLogToTxt("Save XML file error!");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                Log.SaveLogToTxt("Load test data recording...");
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
                Log.SaveLogToTxt("Done.");

                this.btnInitial.Enabled = false;
                this.btnStart.Enabled = false;

                //get Production Family
                Log.SaveLogToTxt("Get production family information from server.");
                string[] member = user.GetProductionFamily(myXml, ref myDataIO, ref dataTable_Family);
                this.comboBoxFamily.Items.Clear();
                if (member == null)
                {
                    Log.SaveLogToTxt("Can't connect to server.");
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
                Log.SaveLogToTxt("Done.");
                //check IP address   
                address_IP = user.GetIP();
                if (address_IP == "")
                {
                    Log.SaveLogToTxt("Can't tet IP address.");                    
                    this.comboBoxFamily.Text = "";
                    this.comboBoxFamily.Enabled = false;
                }
            }
            catch
            {
                Log.SaveLogToTxt("Error. Please check network and 'TestData.xml' file");
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
                this.groupBoxCalibratioon.Enabled = false;
                this.groupBoxConfig.Enabled = false;
                this.groupBoxStatus.BackColor = Color.Yellow;
                this.labelStatus.Text = "正在初始化...";
                Log.SaveLogToTxt("Begin to initial...");

                this.SaveConfigXmlInfo();
                Log.SaveLogToTxt("Saved calibration data to config file.");
                //创建存放眼图的文件夹位置
                string time = DateTime.Now.ToString("yyyy-MM-dd");
                string[] folderPath = new string[3];
                folderPath[0] = Application.StartupPath + @"\EyeDiagram\" + this.comboBoxFamily.Text.ToUpper() + "\\" + this.comboBoxPN.Text.ToUpper() + "\\" + this.comboBoxTestPlan.Text.ToUpper() + "\\" + time + "\\OptEyeDiagram\\";
                folderPath[1] = Application.StartupPath + @"\EyeDiagram\" + this.comboBoxFamily.Text.ToUpper() + "\\" + this.comboBoxPN.Text.ToUpper() + "\\" + this.comboBoxTestPlan.Text.ToUpper() + "\\" + time + "\\ElecEyeDiagram\\";
                folderPath[2] = Application.StartupPath + @"\EyeDiagram\" + this.comboBoxFamily.Text.ToUpper() + "\\" + this.comboBoxPN.Text.ToUpper() + "\\" + this.comboBoxTestPlan.Text.ToUpper() + "\\" + time + "\\Polarity\\";
                user.CreatFolderPath(folderPath);

                tokenSource = new CancellationTokenSource();
                //Task<bool> task = Task.Factory.StartNew<bool>(() => (user.Initial()), tokenSource.Token);
                Task<bool> task = Task.Factory.StartNew<bool>(() => (user.ParallelInitial()), tokenSource.Token);
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
            catch
            {
                Log.SaveLogToTxt("Failed to initial. Please check equipments infomation.");
                this.btnInitial.Enabled = true;
                this.groupBoxCalibratioon.Enabled = true;
                this.groupBoxConfig.Enabled = true;
                this.groupBoxStatus.BackColor = Color.LightCoral;
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

                    string fileName = SN + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                    FilePath.LogFile = Application.StartupPath + @"\Log\" + fileName;

                    TestPlanParaByPN.SN = SN;
                                       
                    tokenSource = new CancellationTokenSource();
                    Task<bool> task = Task.Factory.StartNew<bool>(() => (user.ParallelBeginTest()), tokenSource.Token);
                    Task cwt = task.ContinueWith(t => 
                    {
                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(new UpdateControl(delegate
                            {
                                this.UpdateControlAfterTest(task.Result);

                            }), task.Result);
                        }
                        else
                        {
                            this.UpdateControlAfterTest(task.Result);
                        }
                    });
                }
                else
                {
                    this.labelStatus.Text = "测试完成";
                    this.btnInitial.Enabled = true;
                    this.btnStart.Enabled = true;
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
                Log.SaveLogToTxt("Get production name list from server, according selected family " + productFamlily +".");  

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
                Log.SaveLogToTxt("Done.");  
            }
            catch
            {
                Log.SaveLogToTxt("Faied to get production name list.");   
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
                    Log.SaveLogToTxt("Get test plan list from server, according its PN " + productPN + "."); 
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
                    Log.SaveLogToTxt("Done.");  
                }
            }            
            catch
            {
                Log.SaveLogToTxt("Faied to get test plan list."); 
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

                    Log.SaveLogToTxt("Get parameters and spec of test plan " + testPlan + "."); 
                    user.GetTestPlanParaByPN(ID_TestPlan);
                    user.GetSpec(ID_TestPlan);
                    Log.SaveLogToTxt("Done.");
                    this.comboBoxStation.Text = "";
                }
            }
            catch
            {
                Log.SaveLogToTxt("Faied to get parameters and spec."); 
            }
        }

        private void comboBoxStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxPN.Text != "")
            {
                string station = this.comboBoxStation.SelectedItem.ToString();
                GlobalParaByPN.Station = station;

                this.btnInitial.Enabled = true;
                Log.SaveLogToTxt("Selected test station " + station + ".");
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
                Log.SaveLogToTxt("Save test data failed.");
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
    }
}
