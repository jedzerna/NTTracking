using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace NTTracking
{
    public partial class formRecords : Form
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
        public string id;
        public string username;
        public string recorddate = "";
        public formRecords()
        {
            
            InitializeComponent();
        }
        private void formRecords_Load(object sender, EventArgs e)
        {
            list = null;
            info = null;
            //MessageBox.Show(trackid.Trim());
            if (trackid.Trim() != "")
            {
                label14.Text = recorddate;
                recordinfo();

                UserDashboard user = (UserDashboard)Application.OpenForms["UserDashboard"];
                user.trackid = "";
            }
            else
            {
                recordslist();
            }
        }
        formRecordsList list;
        formRecordInfo info;
        private void recordslist()
        {
            guna2Button2.Visible = false;
            //panel1.Controls.Clear();
            list = new formRecordsList(this);
            list.id = id;
            list.username = username;
            list.Height = panel1.Height;
            list.Width = panel1.Width;
            list.TopLevel = false;
            panel1.Controls.Add(list);
            panel1.AutoScroll = false;
            list.BringToFront();
            list.Show();
        }
        public void recordinfo()
        {
            try
            {
                guna2Button2.Visible = true;
                //panel1.Controls.Clear();
                //formRecordInfo info = new formRecordInfo();
                info = new formRecordInfo();

                info.trackid = trackid;
                info.id = id;
                info.username = username;
                info.Height = panel1.Height;
                info.Width = panel1.Width;
         

            info.TopLevel = false;
            panel1.Controls.Add(info);
            panel1.AutoScroll = false;
            info.BringToFront();
                info.Show();
                //MessageBox.Show("");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
        public string trackid = "";
        private void label1_TextChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(trackid);
            //if ()
            //{

            //}
            //recordinfo();
            //if (label1.Text != "" && trackid != "")
            //{
            //    recordinfo();
            //}
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            guna2Button2.Visible = false;
            label14.Text = "Records";
            if (list == null)
            {
                recordslist();
            }
            else
            {
                list.BringToFront();
                list.Show();
            }
        }
        public event EventHandler<string> TextUpdated;
        public void OnTextUpdated(string newText)
        {
            // Trigger the event to notify Form1 about the updated text
            TextUpdated?.Invoke(this, newText);
        }

        public void UpdateLabelText(string newText)
        {
            // Update the label text in Form2
            label14.Text = newText;
        }
    }
}
