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
            recordslist();
        }
        formRecordsList list;
        private void recordslist()
        {
            list = new formRecordsList();
            list.id = id;
            list.username = username;
            list.Height = panel1.Height;
            list.Width = panel1.Width;
            panel1.Controls.Clear();
            list.TopLevel = false;
            panel1.Controls.Add(list);
            panel1.AutoScroll = false;
            list.BringToFront();
            list.Show();
        }
    }
}
