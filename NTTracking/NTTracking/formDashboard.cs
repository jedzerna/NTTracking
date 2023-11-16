﻿using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NTTracking
{
    public partial class formDashboard : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleparam = base.CreateParams;
                handleparam.ExStyle |= 0x02000000;
                return handleparam;
            }

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.DoubleBuffered = true;
        }
        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }

        public string username;
        public string id;
        private DateTime startTime;
        DBData db = new DBData();
        //UserDashboard userdash;
        private Thread showappsThread;
        public formDashboard()
        {
            InitializeComponent();
        }
        private UserDashboard userd = null;
        public formDashboard(Form callingForm)
        {
            userd = callingForm as UserDashboard;

            InitializeComponent();
        }

        private void formDashboard_Load(object sender, EventArgs e)
        {
                refreshdata();
            dataGridView1.ClearSelection();
            ChangeControlStyles(dataGridView2, ControlStyles.OptimizedDoubleBuffer, true);
            this.dataGridView2.ColumnHeadersDefaultCellStyle.SelectionBackColor = this.dataGridView2.ColumnHeadersDefaultCellStyle.BackColor;

            ChangeControlStyles(dataGridView1, ControlStyles.OptimizedDoubleBuffer, true);
            this.dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = this.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor;
           timer1.Start();
        }
        private void ChangeControlStyles(Control ctrl, ControlStyles flag, bool value)
        {
            MethodInfo method = ctrl.GetType().GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
                method.Invoke(ctrl, new object[] { flag, value });
        }
        int highestId = 0;

        public void refreshdata()
        {
            label7.Text = db.GetAnomalies(id).ToString();
            dataGridView2.DataSource = db.GetPreviousRecord(id);
            label10.Text = db.CalculateTimeDifference(id, DateTime.Now);
            //if (label7.IsHandleCreated)
            //{
            //    label7.BeginInvoke((Action)delegate ()
            //    {
            //        label7.Text = db.GetAnomalies(id).ToString();
            //        dataGridView2.DataSource = db.GetPreviousRecord(id);
            //        string datefrom = "01-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy");
            //        label10.Text = db.CalculateTimeDifference(datefrom, DateTime.Now.ToString("dd-MM-yyyy"));
            //    });
            //}
            //else
            //{
            //    // Handle not created yet, subscribe to HandleCreated event
            //    label7.HandleCreated += (sender, e) =>
            //    {
            //        label7.BeginInvoke((Action)delegate ()
            //        {
            //            label7.Text = db.GetAnomalies(id).ToString();
            //            dataGridView2.DataSource = db.GetPreviousRecord(id);
            //            string datefrom = "01-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy");
            //            label10.Text = db.CalculateTimeDifference(datefrom, DateTime.Now.ToString("dd-MM-yyyy"));
            //        });
            //    };
            //}
        }

        private void LoadProcessesOnUIThread()
        {
            try
            {

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("Software");
                dt.Columns.Add("ProcessID");

                HashSet<int> processIds = new HashSet<int>();

                Process[] processes = Process.GetProcesses();

                foreach (var process in processes)
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle) && !processIds.Contains(process.Id))
                    {
                        DataRow row = dt.NewRow();
                        row["Software"] = process.MainWindowTitle.Trim();
                        row["ProcessID"] = process.Id.ToString().Trim();
                        dt.Rows.Add(row);

                        // Add the Process ID to the HashSet to prevent duplicates
                        processIds.Add(process.Id);
                    }
                }
                dt.AcceptChanges();
                dt = RemoveDuplicateRows(dt, "ProcessID");
                dt.DefaultView.Sort = "Software ASC";

                Thread.Sleep(2000);
                if (dataGridView1.Rows.Count == 0)
                {

                    foreach (DataRow dtrow in dt.Rows)
                    {
                        dataGridView1.BeginInvoke((Action)delegate ()
                        {
                            if (showappsThread != null)
                            {
                                if (userd.timer1.Enabled == true)
                                {
                                    string desc = dtrow["Software"].ToString().Trim();


                                    int a = dataGridView1.Rows.Add();
                                    dataGridView1.Rows[a].Cells["ProcessID"].Value = dtrow["ProcessID"].ToString().Trim();
                                    dataGridView1.Rows[a].Cells["Software"].Value = desc;
                                }
                            }
                        });
                    }
                }
                Thread.Sleep(1000);
                //checking if task is still exist
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    bool found = false;
                    foreach (DataRow dtrow in dt.Rows)
                    {
                        if (showappsThread != null)
                        {
                            if (dtrow["ProcessID"].ToString() == row.Cells["ProcessID"].Value.ToString() && dtrow["Software"].ToString() == row.Cells["Software"].Value.ToString())
                            {
                                found = true;
                                break;
                            }
                        }

                    }
                    if (found == false)
                    {
                        dataGridView1.BeginInvoke((Action)delegate ()
                        {
                            if (showappsThread != null)
                            {
                                if (userd.timer1.Enabled == true)
                                {
                                    dataGridView1.Rows.Remove(row);
                                }
                            }
                        });
                    }
                }

                //add new task
                foreach (DataRow dtrow in dt.Rows)
                {
                    bool found = false;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (showappsThread != null)
                        {
                            if (dtrow["ProcessID"].ToString() == row.Cells["ProcessID"].Value.ToString() && dtrow["Software"].ToString() == row.Cells["Software"].Value.ToString())
                            {
                                found = true;
                                break;
                            }
                        }

                    }
                    if (found == false)
                    {
                            dataGridView1.BeginInvoke((Action)delegate ()
                            {
                                if (showappsThread != null)
                                {
                                    if (userd.timer1.Enabled == true)
                                    {
                                        string desc = dtrow["Software"].ToString().Trim();
                                        //db.AddTaskRunning(id, desc);

                                        int a = dataGridView1.Rows.Add();
                                        dataGridView1.Rows[a].Cells["ProcessID"].Value = dtrow["ProcessID"].ToString().Trim();
                                        dataGridView1.Rows[a].Cells["Software"].Value = desc;
                                    }
                                }
                            });
                    }
                }

                showappsThread = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                showappsThread = null;
            }




        }
        public DataTable RemoveDuplicateRows(DataTable dataTable, string columnToCheck)
        {
            // Create a new DataTable with the same structure
            DataTable distinctTable = dataTable.Clone();

            // Create a HashSet to keep track of seen values in the specified column
            HashSet<string> seenValues = new HashSet<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                string value = row[columnToCheck].ToString();

                if (!seenValues.Contains(value))
                {
                    // Add the row to the distinctTable and HashSet
                    distinctTable.ImportRow(row);
                    seenValues.Add(value);
                }
            }

            return distinctTable;
        }
        bool connection = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (db.OpenConnection())
                {
                    db.CloseConnection();
                }
                else
                {
                    connection = false;

                    timer1.Enabled = false;
                    timer1.Stop();
                    return;
                }
                startTime = DateTime.Now;
                string data1 = id;
                string data3 = startTime.ToString("dd-MM-yyyy");
                highestId = db.GetHighestId(data1, startTime);
                if (highestId > 0)
                {
                    if (showappsThread == null)
                    {
                        showappsThread = new Thread(LoadProcessesOnUIThread);
                        showappsThread.Start();
                    }
                }
                else
                {
                    if (showappsThread == null)
                    {
                        if (dataGridView1.Rows.Count != 0)
                        {
                            dataGridView1.Rows.Clear();
                        }
                    }
                }
                if (userd != null && userd.reload != null && userd.reload == "2")
                {
                    refreshdata();
                    userd.reload = "";
                }else if (userd != null && userd.reload != null && userd.reload == "1")
                {
                    refreshdata();
                    userd.reload = "";
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void guna2ShadowPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            UserDashboard user = (UserDashboard)Application.OpenForms["UserDashboard"];

            user.recorddate = "Records for " + DateTime.Parse(dataGridView2.CurrentRow.Cells["records"].Value.ToString()).ToString("MMM/dd/yyyy");
            // UserDashboard user = new UserDashboard();
            user.trackid = dataGridView2.CurrentRow.Cells["Column1"].Value.ToString().Trim();
            user.guna2Button2_Click(sender,e);
           // guna2Button2
        }
    }
}
