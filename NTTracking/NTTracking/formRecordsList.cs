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
            load();
        }
        private void load()
        {
            totalRecords = db.GetTotalRecords(id);
            UpdatePageInformation();
            int startIndex = (currentPage - 1) * pageSize;
            DataTable dt = db.GetAllPreviousRecord(id, startIndex, pageSize);
            
            dataGridView2.DataSource = db.GetAllPreviousRecord(id, startIndex, pageSize);
           
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
            //formRecords rec = (formRecords)Application.OpenForms["formRecords"];
            parentForm.label14.Text = "Records for "+dataGridView2.CurrentRow.Cells["records"].Value.ToString();

            parentForm.trackid = dataGridView2.CurrentRow.Cells["Column1"].Value.ToString();
            parentForm.recordinfo();
        }
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

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
    }
}
