using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using Google.Protobuf;
using Guna.UI2.WinForms;
using WindowsInput;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static Guna.UI2.Native.WinApi;
using static NTTracking.DBData;
using static NTTracking.Model.TrackData;
using static System.TimeZoneInfo;
using System.Data.SqlClient;
using System.Web.Management;
using MySqlX.XDevAPI.Relational;
using System.Security.Policy;

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
        public string position;
        public string trackid ="";
        public string recorddate ="";
        public string id;
        public Image img;
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

        private formRecordsList List;

        public UserDashboard(formRecordsList form1)
        {
            InitializeComponent();
            List = form1;
        }
        int highestId = 0;
        private DataTable appcategories = new DataTable();
        private void UserDashboard_Load(object sender, EventArgs e)
        {
            appcategories = db.GetAppCategories();

            SuspendLayout();
            if (img == null)
            {
                pictureisnull();
            }
            else
            {
                guna2CirclePictureBox1.Image = img;

                Image imggg = resizeImage((Image)img, new Size(35, 35));
                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
                guna2CirclePictureBox2.Image = ByteToImage(xByte);
            }
            guna2CirclePictureBox1.Controls.Add(pictureBox1);
            pictureBox1.Location = new Point(73, 63);
            pictureBox1.BackColor = Color.Transparent;



            guna2CirclePictureBox2.Controls.Add(pictureBox2);
            pictureBox2.Location = new Point(12, 12);
            pictureBox2.BackColor = Color.Transparent;

            startTime = DateTime.Now;
            string data1 = id;
            string data3 = startTime.ToString("dd-MM-yyyy");
            highestId = db.GetHighestId(data1, startTime);
            if (highestId > 0)
            {
                // Do ; with highestId
                loaddata();

                startTracking();
            }
            showappsThread = null;

            label1.Text = username;
            label4.Text = position;
            guna2Button1_Click(sender, e);
            ResumeLayout();
        }
        private void pictureisnull()
        {
            string inputString = username.Trim();

            if (!string.IsNullOrEmpty(inputString)) // Check if the string is not empty
            {
                char firstLetter = inputString[0];
                string resourceName = firstLetter.ToString().ToLower(); 
                if (Properties.Resources.ResourceManager.GetObject(resourceName) != null)
                {
                    guna2CirclePictureBox1.Image = (Image)Properties.Resources.ResourceManager.GetObject(resourceName);

                    Image imggg = resizeImage((Image)Properties.Resources.ResourceManager.GetObject(resourceName), new Size(35, 35));
                    ImageConverter _imageConverter = new ImageConverter();
                    byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
                    guna2CirclePictureBox2.Image = ByteToImage(xByte);
                }
                else
                {
                    guna2CirclePictureBox1.Image = Properties.Resources.profilep;

                    Image imggg = resizeImage((Image)Properties.Resources.profilep, new Size(35, 35));
                    ImageConverter _imageConverter = new ImageConverter();
                    byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
                    guna2CirclePictureBox2.Image = ByteToImage(xByte);
                }
            }
            else
            {
                guna2CirclePictureBox1.Image = Properties.Resources.profilep;
                Image imggg = resizeImage((Image)Properties.Resources.profilep, new Size(35, 35));
                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(imggg, typeof(byte[]));
                guna2CirclePictureBox2.Image = ByteToImage(xByte);
            }
        }
        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
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
            highestId = db.GetHighestId(data1, startTime);
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
            guna2Button6.Enabled = true;
            guna2Button9.Enabled = true;
            guna2Button4.Enabled = false;
            guna2Button11.Enabled = false;

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
            try
            {
                m_GlobalHook.MouseMove += GlobalHookOnMouseMove;
                m_GlobalHook.MouseClick += GlobalHookOnMouseClick;
                Application.Run();
            }
            catch
            {

            }
        }
        public DataTable RemoveDuplicateRows(DataTable dataTable, string columnId, string columnName)
        {
            // Create a new DataTable with the same structure
            DataTable distinctTable = dataTable.Clone();

            // Create a HashSet to keep track of seen values in the specified columns
            HashSet<string> seenValues = new HashSet<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                string valueId = row[columnId].ToString();
                string valueName = row[columnName].ToString();

                // Concatenate values from both columns
                string concatenatedValues = $"{valueId}_{valueName}";

                if (!seenValues.Contains(concatenatedValues))
                {
                    // Add the row to the distinctTable and HashSet
                    distinctTable.ImportRow(row);
                    seenValues.Add(concatenatedValues);
                }
            }

            return distinctTable;
        }
        public class ProcessInfo
        {
            public string Software { get; set; }
        }
        public class ProcessInfoTemp
        {
            public string Software { get; set; }
        }
        List<ProcessInfo> dataList = new List<ProcessInfo>();
        private void LoadProcessesOnUIThread()
        {
            List<ProcessInfoTemp> dataListT = new List<ProcessInfoTemp>();
            dataListT.Clear();
            HashSet<int> processIds = new HashSet<int>();

            Process[] processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle) && !processIds.Contains(process.Id))
                {
                    ProcessInfoTemp newRowT = new ProcessInfoTemp
                    {
                        Software = process.MainWindowTitle.Trim(),
                    };

                    dataListT.Add(newRowT);
                    processIds.Add(process.Id);

                }
            }

            dataListT = dataListT.GroupBy(x => x.Software)
                                 .Select(group => group.First())
                                 .ToList();

            foreach (ProcessInfoTemp itemT in dataListT)
            {

                bool exist = false;
                foreach (ProcessInfo item in dataList)
                {
                    if (item.Software.Trim() == itemT.Software.Trim())
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    string catid = "6";
                    foreach (DataRow rowprocess in appcategories.Rows)
                    {
                        if (itemT.Software.Contains(rowprocess["name"].ToString()))
                        {
                            catid = rowprocess["id"].ToString();
                            break; // Break the loop once a match is found
                        }
                    }
                    if (db.AddTaskRunning(id, highestId.ToString(), itemT.Software, catid))
                    {
                        ProcessInfo newRow = new ProcessInfo
                        {
                            Software = itemT.Software
                        };
                        dataList.Add(newRow);
                    }

                }
            }
            List<ProcessInfo> itemsToRemove = new List<ProcessInfo>();
            foreach (ProcessInfo item in dataList)
            {
                bool exist = false;
                foreach (ProcessInfoTemp itemT in dataListT)
                {
                    if (item.Software.Trim() == itemT.Software.Trim())
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist == false)
                {
                    if (db.CloseTaskRunning(id, highestId.ToString(), item.Software))
                    {
                        itemsToRemove.Add(item);
                    }
                }
            }
            foreach (ProcessInfo itemToRemove in itemsToRemove)
            {
                dataList.Remove(itemToRemove);
            }

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
            showappsThread = null;
        
            Application.Exit();
        }
        DBData db = new DBData();
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (db.OpenConnection())
            {
                startTime = DateTime.Now;
                // Specify the data you want to insert
                string data1 = id; 

                // Insert the data into the database
                if (db.TimeIn(data1, startTime))
                {
                    formDashboard fdash = (formDashboard)Application.OpenForms["formDashboard"];
                    fdash.refreshdata();
                    fdash.EnableTimer(); 
                    fdash.showappsThread = null;
                    //refreshdata();
                    startTracking();
                }
                db.CloseConnection();
            }
            Cursor.Current = Cursors.Default;
        }
        bool connection = false;
        private void connectionlost()
        {

            guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
            {
                guna2ProgressIndicator1.Start();
                guna2ProgressIndicator1.Visible = true;
            });
        }
        private void connected()
        {

                loading = null;
                guna2ProgressIndicator1.Stop();
                guna2ProgressIndicator1.Visible = false;

        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (db.OpenConnection() == true)
            {
                connected();
            }
        }
        Thread loading;
        private void timer1_Tick(object sender, EventArgs e)
        {

            try
            {

                TimeSpan elapsedTime = DateTime.Now - startTime;
                label3.Text = elapsedTime.Hours.ToString("00") + "\n" +
                             elapsedTime.Minutes.ToString("00") + "\n" +
                            elapsedTime.Seconds.ToString("00");
                label6.Text = elapsedTime.Hours.ToString("00") + ":" +
                                 elapsedTime.Minutes.ToString("00") + ":" +
                                elapsedTime.Seconds.ToString("00");
                if (showappsThread == null && timer1.Enabled == true)
                {
                    showappsThread = new Thread(LoadProcessesOnUIThread);
                    showappsThread.Start();
                }
                if (idlechecking >= 120)
                {
                    idlechecking++;
                    pictureBox1.Image = Properties.Resources.circlered2;
                    pictureBox2.Image = Properties.Resources.circlered2;
                }
                else if (idlechecking >= 40)
                {
                    idlechecking++;
                    pictureBox1.Image = Properties.Resources.circlewaiting;
                    pictureBox2.Image = Properties.Resources.circlewaiting;
                }
                else
                {
                    idlechecking++;
                    pictureBox1.Image = Properties.Resources.circlegreen2;
                    pictureBox2.Image = Properties.Resources.circlegreen2;
                }
            }
            catch
            {

            
                    connection = false;
                    loading = new Thread(connectionlost);
                    loading.Start();
                    timer1.Enabled = false;
                    timer1.Stop();
                    timer2.Start();
                    return;
                
            }
        }
        int idlechecking = 0;
        string keyboardchar;
        string mousepoint;
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //try
            //{

                    //startTime = DateTime.Now;
                    // Specify the data you want to insert
                    string data1 = highestId.ToString();
                    string data2 = DateTime.Now.ToString(@"HH\:mm\:ss");
                    string data3 = DateTime.Now.ToString("dd-MM-yyyy");

                    // Insert the data into the database
                    if (db.TimeOut(data1, DateTime.Now, id))
            {
                formDashboard fdash = (formDashboard)Application.OpenForms["formDashboard"];
                fdash.StopTimer();
                fdash.showappsThread = null;
                fdash.refreshdata();
                //refreshdata();
                KeyboardHook.Unhook();
                        m_GlobalHook.MouseMove -= GlobalHookOnMouseMove;
                        m_GlobalHook.MouseClick -= GlobalHookOnMouseClick;

                        timer1.Stop();
                        timer1.Enabled = false;
                        eventThread = null;
                showappsThread = null;
                        label3.Text = "0:00";
                        label6.Text = "0:00";
                        guna2Button6.Enabled = false;
                        guna2Button9.Enabled = false;
                guna2Button4.Enabled = true;
                        guna2Button11.Enabled = true;
            }

            Cursor.Current = Cursors.Default;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }

        private void transparentPictureBox2_Click(object sender, EventArgs e)
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
            Cursor.Current = Cursors.WaitCursor;
            //changefill();
            if (formNum != "1")
            {
                if (dashboard != null)
                {
                    formNum = "1";
                    dashboard.BringToFront();
                    dashboard.Show();
                }
                else
                {
                    formNum = "1";
                    dashboard = new formDashboard(this);
                    dashboard.id = id;
                    dashboard.username = username;
                    dashboard.Height = panel1.Height;
                    dashboard.Width = panel1.Width;
                    //panel1.Controls.Clear();
                    dashboard.TopLevel = false;
                    panel1.Controls.Add(dashboard);
                    panel1.AutoScroll = false;
                    dashboard.BringToFront();
                    dashboard.Show();
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            guna2Transition1.HideSync(guna2ShadowPanel1);
            guna2ShadowPanel1.Visible = false;


            guna2Transition2.ShowSync(guna2ShadowPanel2);
            guna2ShadowPanel2.Visible = true;

            panel1.SetBounds(117, 46, 774, 543);
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {

            guna2Transition2.HideSync(guna2ShadowPanel2);
            guna2ShadowPanel2.Visible = false;

            panel1.SetBounds(188, 48, 774, 543);

            guna2Transition1.ShowSync(guna2ShadowPanel1);
            guna2ShadowPanel1.Visible = true;



        }

        public formRecords records;
        public void guna2Button2_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (formNum != "2")
            {
                if (records != null)
                {
                    formNum = "2";
                    records.BringToFront();
                    records.Show();
                }
                else
                {
                    formNum = "2";
                    records = new formRecords();
                    records.id = id;
                    records.username = username;
                    records.trackid = trackid;
                    records.recorddate = recorddate;
                    records.Height = panel1.Height;
                    records.Width = panel1.Width;
                    //panel1.Controls.Clear();
                    records.TopLevel = false;
                    panel1.Controls.Add(records);
                    panel1.AutoScroll = false;
                    records.BringToFront();
                    records.Show();
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            guna2Button6_Click(sender,e);
        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {

            guna2Button4_Click(sender, e);
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (guna2TextBox2.Text != "")
            {
                if (DateTime.TryParse(guna2TextBox2.Text, out DateTime date))
                {
                    label5.Visible = false;
                    guna2TextBox2.BorderColor = Color.FromArgb(94, 148, 255);
                    guna2TextBox2.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
                    guna2TextBox2.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
                }
                else
                {
                    label5.Visible = true;
                    guna2TextBox2.FocusedState.BorderColor = Color.Maroon;
                    guna2TextBox2.HoverState.BorderColor = Color.Maroon;
                    guna2TextBox2.BorderColor = Color.Maroon;
                }
            }
            else
            {
                label5.Visible = false;
                guna2TextBox2.BorderColor = Color.FromArgb(94, 148, 255);
                guna2TextBox2.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
                guna2TextBox2.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            }
            formRecordsList dasha = (formRecordsList)Application.OpenForms["formRecordsList"];
            dasha.label2.Text = guna2TextBox2.Text;
        }

        private void guna2TextBox2_Enter(object sender, EventArgs e)
        {
            //records = null;
            guna2Button2_Click(sender,e);
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {

        }

        private void UserDashboard_SizeChanged(object sender, EventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)
            //{
            //    this.FormBorderStyle = FormBorderStyle.None;
            //}
            //else
            //{
            //    this.FormBorderStyle = FormBorderStyle.Sizable;
            //}
        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }
    }
}
