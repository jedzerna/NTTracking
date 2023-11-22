using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NTTracking
{
    public partial class formRecordsList : Form
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
        DBData db = new DBData();
        public formRecordsList()
        {
            InitializeComponent();
        }
        private formRecords parentForm;

        public formRecordsList(formRecords form1)
        {
            InitializeComponent();
            parentForm = form1;
        }
        private int pageSize = 17; // Number of records to show per page
        private int currentPage = 1;
        private int totalRecords = 0;
        private void formRecordsList_Load(object sender, EventArgs e)
        {
            UserDashboard user = (UserDashboard)Application.OpenForms["UserDashboard"];
      
            load();
        }
        private void load()
        {
            if (label2.Text != "")
            {
                if (DateTime.TryParse(label2.Text, out DateTime date))
                {
                    //
                    string dates = date.ToString("yyyy-MM-dd");
                    //label3.Text = $"The date is valid: {dates}";
                    totalRecords = db.GetTotalRecords(id, dates);
                    UpdatePageInformation();
                    int startIndex = (currentPage - 1) * pageSize;
                    dataGridView2.DataSource = db.GetAllPreviousRecord(id, startIndex, pageSize, dates);
                }
            }
            else
            {
                //currentPage = 1;
                totalRecords = db.GetTotalRecords(id, "");
                UpdatePageInformation();
                int startIndex = (currentPage - 1) * pageSize;
                dataGridView2.DataSource = db.GetAllPreviousRecord(id, startIndex, pageSize, "");
            }
            if (dataGridView2.Rows.Count == 0)
            {
                label1.Text = "No Records Found";
            }
           
        }
        private void UpdatePageInformation()
        {
            // Calculate the total number of pages
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Display page information
            label1.Text = $"{currentPage}/{totalPages}";

            // Enable or disable buttons based on current page
            guna2Button2.Visible = currentPage > 1;
            guna2Button1.Visible = currentPage < totalPages;
        }
        formRecords dasha = (formRecords)Application.OpenForms["formRecords"];
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;
            //formRecords rec = (formRecords)Application.OpenForms["formRecords"];
            parentForm.label14.Text = "Records for " + dataGridView2.CurrentRow.Cells["records"].Value.ToString();

            parentForm.trackid = dataGridView2.CurrentRow.Cells["Column1"].Value.ToString();
            parentForm.recordinfo();
            Cursor.Current = Cursors.Default;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            currentPage++;
            load();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                load();
            }
        }

        private void label2_TextChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            load();
            //if (label2.Text != "")
            //{
            //    if (DateTime.TryParse(label2.Text, out DateTime date))
            //    {
            //        label1.Text = "No Records";
            //    }
            //    else
            //    {
            //        label1.Text = "No Records";
            //    }
            //}
        }
    }
}
