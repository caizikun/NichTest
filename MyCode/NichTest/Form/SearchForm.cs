using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Threading.Tasks;

namespace NichTest
{
    public partial class SearchForm : Form
    {
        private delegate void SavaData(DataTable taskResult);

        private const byte QSFP_TotalChannel = 4;

        private const byte CFP_TotalChannel = 4;

        private const byte SFP_TotalChannel = 1;

        public SearchForm()
        {
            InitializeComponent();
        }

        private void txtListSN_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int length = this.txtListSN.Lines.Length;
                if (this.txtListSN.Lines[length - 1].Length != 12)
                {
                    this.txtListSN.Lines[length - 1].Remove(0);
                };

                this.txtListSN.Refresh();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.txtListSN.Text == "")
                {
                    return;
                }
                this.btnSearch.Enabled = false;

                string groupSN = "";
                string experisson = "";
                string table = "";
                if(this.radioButtonTxO.Checked)
                {
                    table = "my_databases.txo";
                }

                if(this.radioButtonRxO.Checked)
                {
                    table = "my_databases.rxo";
                }

                if(this.radioButtonQuickCheck.Checked)
                {
                    table = "my_databases.quickcheck_testdata";
                }

                if (this.radioButtonByGroup.Checked)
                {
                    groupSN = this.txtListSN.Text + "%";
                    groupSN = groupSN.Replace("\r\n", "%");
                    experisson = "SELECT * FROM " + table + " where Serialnumber like '" +
                        groupSN + "' order by ID";
                }
                else if (this.radioButtonBySN.Checked)
                {

                    for (int i = 0; i < this.txtListSN.Lines.Length; i++)
                    {
                        if (i == this.txtListSN.Lines.Length - 1)
                        {
                            groupSN += "'" + this.txtListSN.Lines[i] + "'";
                        }
                        else
                        {
                            groupSN += "'" + this.txtListSN.Lines[i] + "',";
                        }
                    }
                    experisson = "SELECT * FROM " + table + " where Serialnumber in (" +
                        groupSN + ") order by ID";
                }
                else
                {
                    return;
                }

                bool isGettingLatestData = true;
                if(this.radioButtonFullData.Checked)
                {
                    isGettingLatestData = false;
                }
                
                Task<DataTable> task = Task.Factory.StartNew<DataTable>(() =>
                {
                    //get all recording test data
                    string mysqlconCommand = "Database=my_databases;Data Source=localhost;User Id=root;Password=abc@123;pooling=false;CharSet=utf8;port=3306";
                    MySqlConnection mycon = new MySqlConnection();
                    mycon.ConnectionString = mysqlconCommand;
                    mycon.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(experisson, mycon);
                    MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
                    DataSet ds = new DataSet(table);
                    da.Fill(ds, table);
                    DataTable dt = ds.Tables[table];
                    mycon.Close();

                    if (isGettingLatestData == false)
                    {
                        return dt;
                    }

                    int count = dt.Rows.Count;
                    DataTable newdt = dt.Clone();
                    newdt.Clear();
                    string expression = "";
                    DataRow[] drs;

                    if (table == "my_databases.quickcheck_testdata")
                    {
                        //get the last test recording by total channel and station                        
                        int recordingCount = 1;
                        if (dt.Rows[count - 1]["Family"].ToString().Contains("QSFP"))
                        {
                            recordingCount = QSFP_TotalChannel;
                        }
                        else if (dt.Rows[count - 1]["Family"].ToString().Contains("CFP"))
                        {
                            recordingCount = CFP_TotalChannel;
                        }
                        else if (dt.Rows[count - 1]["Family"].ToString().Contains("SFP"))
                        {
                            recordingCount = SFP_TotalChannel;
                        }
                        else
                        {
                            recordingCount = count;
                        }                        
                        
                        string[] stations = { "PreModule", "PreTCT", "PostTCT", "PostBurnIn"};
                        for (int i = 0; i < stations.Length; i++)
                        {
                            expression = "Station = '" + stations[i] + "'";
                            drs = dt.Select(expression);
                            if (drs.Length == 0)
                            {
                                continue;
                            }
                            for (int j = recordingCount; j > 0; j--)
                            {
                                newdt.Rows.Add(drs[drs.Length - j].ItemArray);                                
                            }
                            drs = null;
                        }
                        return newdt;
                    }
                    
                    //get the last test recording by comparing test time
                    DateTime lastTime = DateTime.MinValue;
                    for (int i = 0; i < count; i++)
                    {
                        DateTime time = Convert.ToDateTime(dt.Rows[i]["Time"]);
                        if (time >= lastTime)
                        {
                            lastTime = time;
                        }
                    }
                    expression = "Time = '" + lastTime.ToString() + "'";
                    drs = dt.Select(expression);
                    if (drs == null || drs.Length == 0)
                    {
                        return null;
                    }

                    for (int i = 0; i < drs.Length; i++)
                    {
                        newdt.Rows.Add(drs[i].ItemArray);
                    }                    
                    drs = null;
                    return newdt;                                                        
                });

                Task cwt = task.ContinueWith(t =>
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new SavaData(delegate
                        {
                            this.SaveDataToLoc(task.Result);

                        }), task.Result);
                    }
                    else
                    {
                        this.SaveDataToLoc(task.Result);
                    }
                });                
            }
            catch
            {
                return;
            }
            finally
            {
                this.btnSearch.Enabled = true;
            }
        }        

        private void SaveDataToLoc(DataTable testDataTable)
        {
            try
            {
                if (testDataTable == null || testDataTable.Rows.Count == 0)
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
                FilePath.SaveTableToExcel(testDataTable, fileExcelPath);
            }
            catch
            {
                return;
            }
            finally
            {
                this.btnSearch.Enabled = true;
            }
        }         

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            this.txtListSN.Text = "";
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("①批量： 输入SN前缀。\r\n" + "②按SN： 一行一个SN。输入回车换行。");
        }

        private void txtListSN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
            }
        }    
    }
}
