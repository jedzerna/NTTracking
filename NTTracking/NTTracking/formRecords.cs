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
        public string id;
        public string username;
        public formRecords()
        {
            
            InitializeComponent();
        }
        private void formRecords_Load(object sender, EventArgs e)
        {
            list = null;
            info = null;
            trackid = "";
            recordslist();
        }
        formRecordsList list;
        formRecordInfo info;
        private void recordslist()
        {
            guna2Button2.Visible = false;
            panel1.Controls.Clear();
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
                panel1.Controls.Clear();
            //formRecordInfo info = new formRecordInfo();
           info = new formRecordInfo
            {
                trackid = trackid,
                id = id,
                username = username,
                Height = panel1.Height,
                Width = panel1.Width
            };

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
        public string trackid;
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
            recordslist();
        }
    }
}
