﻿using System;
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
using Gma.System.MouseKeyHook;
using Guna.UI2.WinForms;
using WindowsInput;
using static NTTracking.DBData;
using static NTTracking.Model.TrackData;

namespace NTTracking
{
    public partial class UserDashboard : Form
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

        private string formNum;
        public string username;
        public string id;
        private Thread eventThread;
        private Thread showappsThread;
        private DateTime startTime;
        formDashboard dashboard;
        private bool canProcessClick = true; // Indicates whether a click event can be processed
        private System.Windows.Forms.Timer clickTimer; // Timer to reset the flag
        public UserDashboard()
        {
            InitializeComponent();
            // Initialize the Windows Forms Timer
            clickTimer = new System.Windows.Forms.Timer();
            clickTimer.Interval = 1000; // Adjust this interval as needed (in milliseconds)
            clickTimer.Tick += (s, e) => canProcessClick = true;
        }
        int highestId = 0;
        private void UserDashboard_Load(object sender, EventArgs e)
        {
            SuspendLayout();
            guna2CirclePictureBox1.Controls.Add(pictureBox1);
            pictureBox1.Location = new Point(69, 62);
            pictureBox1.BackColor = Color.Transparent;
            startTime = DateTime.Now;
            string data1 = id;
            string data3 = startTime.ToString("dd-MM-yyyy");
            highestId = db.GetHighestId(data1, data3);
            if (highestId > 0)
            {
                // Do ; with highestId
                loaddata();

                startTracking();
            }
            showappsThread = null;

            label1.Text = username;
            guna2Button1_Click(sender, e);
            ResumeLayout();
        }
        private void PictureBoxForeground_Paint(object sender, PaintEventArgs e)
        {
            // Draw the top PictureBox with transparency.
            using (var brush = new SolidBrush(Color.FromArgb(128, Color.White)))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }
        }
        private void ChangeControlStyles(Control ctrl, ControlStyles flag, bool value)
        {
            MethodInfo method = ctrl.GetType().GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
                method.Invoke(ctrl, new object[] { flag, value });
        }
        private void loaddata()
        {
            DBData dbs = new DBData();
            List<DBData.MyData> dataList = dbs.RetrieveData(highestId.ToString());
            if (dataList.Count > 0)
            {
                DateTime elapsedTime = DateTime.Parse(dataList[0].YourProperty1);
                startTime = elapsedTime;
            }
        }
        private void startTracking()
        {
            timer1.Enabled = true;
            timer1.Start();
            startTime = DateTime.Now;
            string data1 = id;
            string data3 = startTime.ToString("dd-MM-yyyy");
            highestId = db.GetHighestId(data1, data3);
            //MessageBox.Show(highestId.ToString());
            KeyboardHook.SetHook();
            KeyboardHook.KeyDown += KeyDownEventHandler;
            KeyboardHook.KeyUp += KeyUpEventHandler;
            m_GlobalHook = Hook.GlobalEvents();
            if (showappsThread == null && timer1.Enabled == true)
            {
                showappsThread = new Thread(LoadProcessesOnUIThread);
                showappsThread.Start();
            }
            eventThread = new Thread(EventHandlingThread);
            eventThread.Start();

            loaddata();
            guna2Button6.Visible = true;
            guna2Button4.Visible = false;

        }
        private IKeyboardMouseEvents m_GlobalHook;

        private void KeyDownEventHandler(object sender, KeyEventArgs e)
        {
            // Handle key down events here
            //Console.WriteLine("Key Down: " + e.KeyCode);
            //guna2TextBox2.Text += e.KeyCode.ToString();

            idlechecking = 0;
        }

        private void KeyUpEventHandler(object sender, KeyEventArgs e)
        {
            idlechecking = 0;
        }
        private void EventHandlingThread()
        {
            m_GlobalHook.MouseMove += GlobalHookOnMouseMove;
            m_GlobalHook.MouseClick += GlobalHookOnMouseClick;
            Application.Run();
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
        private void LoadProcessesOnUIThread()
        {
            //Thread.Sleep(3000);
            //MessageBox.Show("dawd");
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Software");
            dt.Columns.Add("ProcessID");


            DataTable dtf = new DataTable();
            dtf.Clear();
            dtf.Columns.Add("Software");
            dtf.Columns.Add("ProcessID");

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
                    processIds.Add(process.Id);
                }
            }
            dt = RemoveDuplicateRows(dt, "ProcessID");
            dt.AcceptChanges();
            foreach (DataRow row in dt.Rows)
            {
                bool exist = false;
                foreach (DataRow rowf in dtf.Rows)
                {
                    if (rowf["Software"].ToString() == row["Software"].ToString() && rowf["ProcessID"].ToString() == row["ProcessID"].ToString())
                    {
                        exist = true; break;
                    }
                }
                if (exist == false)
                {
                    DataRow rown = dtf.NewRow();
                    rown["Software"] = row["Software"].ToString();
                    rown["ProcessID"] = row["ProcessID"].ToString();
                    dtf.Rows.Add(rown);
                    dtf.AcceptChanges();
                }
            }
            foreach (DataRow rowf in dtf.Rows)
            {
                bool exist = false;
                foreach (DataRow row in dt.Rows)
                {
                    if (rowf["Software"].ToString() == row["Software"].ToString() && rowf["ProcessID"].ToString() == row["ProcessID"].ToString())
                    {
                        exist = true; break;
                    }
                    

                }

                if (exist == false)
                {
                    rowf.Delete();
                    dtf.AcceptChanges();
                }
            }
            dtf.DefaultView.Sort = "Software ASC";
            dtf.AcceptChanges();
            Thread.Sleep(2000);
            dataGridView1.BeginInvoke((Action)delegate ()
            {
                dataGridView1.DataSource = dtf;
            });
            showappsThread = null;

        }
        private void GlobalHookOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!canProcessClick)
                return;

            canProcessClick = false; // Disable further processing
            clickTimer.Start(); // Start the timer
            // Handle global mouse move events
            int x = e.X;
            int y = e.Y;

            idlechecking = 0;
        }
        private void GlobalHookOnMouseClick(object sender, MouseEventArgs e)
        {
            // Handle global mouse click events
            MouseButtons button = e.Button;
            int x = e.X;
            int y = e.Y;
            idlechecking = 0;
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ShadowPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void UserDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            /* KeyboardHook.Unhook();
             base.OnFormClosing(e);
             m_GlobalHook.MouseMove -= GlobalHookOnMouseMove;
             m_GlobalHook.MouseClick -= GlobalHookOnMouseClick;

             // Stop and join the event thread to exit cleanly
             eventThread.Abort();
             eventThread.Join();*/
        }
        DBData db = new DBData();
        private void guna2Button4_Click(object sender, EventArgs e)
        {

            if (db.OpenConnection())
            {
                startTime = DateTime.Now;
                // Specify the data you want to insert
                string data1 = id; 
                string data2 = startTime.ToString(@"HH\:mm\:ss");
                string data3 = startTime.ToString("dd-MM-yyyy");

                // Insert the data into the database
                if (db.TimeIn(data1, data2, data3))
                {

                    //refreshdata();
                    startTracking();
                }
                db.CloseConnection();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          
                TimeSpan elapsedTime = DateTime.Now - startTime;
                label3.Text = elapsedTime.Hours.ToString("00") + "\n" +
                             elapsedTime.Minutes.ToString("00") + "\n" +
                            elapsedTime.Seconds.ToString("00");

            if (showappsThread == null && timer1.Enabled == true)
            {
                showappsThread = new Thread(LoadProcessesOnUIThread);
                showappsThread.Start();
            }
            if (idlechecking >= 1000)
            {
                //idlechecking = 0;
                idlechecking++;
                //label4.Text = idlechecking.ToString();
                pictureBox1.Image = Properties.Resources.circlered2;
            }
            else if (idlechecking >= 400)
            {
                //idlechecking = 0;
                idlechecking++;
                //label4.Text = idlechecking.ToString();
                pictureBox1.Image = Properties.Resources.circlewaiting;
            }
            else
            {
                idlechecking++;
                //label4.Text = idlechecking.ToString();
                pictureBox1.Image = Properties.Resources.circlegreen2;
            }
        }
        int idlechecking = 0;
        string keyboardchar;
        string mousepoint;
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (db.OpenConnection())
            {
                //startTime = DateTime.Now;
                // Specify the data you want to insert
                string data1 = highestId.ToString();
                string data2 = DateTime.Now.ToString(@"HH\:mm\:ss");
                string data3 = DateTime.Now.ToString("dd-MM-yyyy");

                // Insert the data into the database
                if (db.TimeOut(data1, data2, data3))
                {
                    //refreshdata();
                    KeyboardHook.Unhook();
                    m_GlobalHook.MouseMove -= GlobalHookOnMouseMove;
                    m_GlobalHook.MouseClick -= GlobalHookOnMouseClick;

                    timer1.Stop();
                    timer1.Enabled = false;
                    eventThread = null;
                    showappsThread = null;
                    label3.Text = "0:00";
                    guna2Button6.Visible = false;
                    guna2Button4.Visible = true;
                }
                db.CloseConnection();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }

        private void transparentPictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
                    }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            //changefill();
            formNum = "1";
            //guna2Button6.FillColor = Color.FromArgb(56, 163, 255);
            dashboard = new formDashboard(this);
            dashboard.id = id;
            dashboard.username = username;
            dashboard.Height = panel1.Height;
            dashboard.Width = panel1.Width;
            panel1.Controls.Clear();
            dashboard.TopLevel = false;
            panel1.Controls.Add(dashboard);
            panel1.AutoScroll = false;
            dashboard.BringToFront();
            dashboard.Show();
        }
    }
}
