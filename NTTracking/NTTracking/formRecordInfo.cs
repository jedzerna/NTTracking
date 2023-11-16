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
    public partial class formRecordInfo : Form
    {
        public string id;
        public string username;
        public string trackid;
        DBData db = new DBData();
        public formRecordInfo()
        {
            InitializeComponent();
        }
        DataTable dataTable = new DataTable();
        private int pageSize = 10; // Number of records to show per page
        private int currentPage = 1;
        private int totalRecords = 0;
        private void formRecordInfo_Load(object sender, EventArgs e)
        {
            load();
            dataTable = db.GetTasks(id, trackid);

            LoadData();
            dataGridView2.DataSource = db.TotalEachCategories(id, trackid);
            //dataGridView2.DataSource = db.GetSessions(id,trackid);
        }
        private void LoadData()
        {
            try
            {
                // Assuming dataTable is populated somewhere in your code
                // For demonstration purposes, let's assume it's filled with dummy data
               // FillDummyData();

                // Calculate the total number of records
                totalRecords = dataTable.Rows.Count;

                // Calculate the starting index for the current page
                int startIndex = (currentPage - 1) * pageSize;

                // Use LINQ to paginate the DataTable
                var pageData = dataTable.AsEnumerable()
                    .Skip(startIndex)
                    .Take(pageSize)
                    .CopyToDataTable();

                // Bind the paginated DataTable to a DataGridView or any other UI control
                dataGridView1.DataSource = pageData;

                // Update the page label and button states
                UpdatePageInformation();
            }
            catch (Exception ex)
            {
               
            }
        }
        //private void FillDummyData()
        //{
        //    // Fill the dataTable with some dummy data for demonstration purposes
        //    dataTable.Columns.Add("ID", typeof(int));
        //    dataTable.Columns.Add("Name", typeof(string));

        //    for (int i = 1; i <= 100; i++)
        //    {
        //        dataTable.Rows.Add(i, $"Name {i}");
        //    }
        //}

        private void UpdatePageInformation()
        {
            // Calculate the total number of pages
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Display page information
            label8.Text = $"{currentPage}/{totalPages}";

            guna2Button2.Visible = currentPage > 1;
            guna2Button1.Visible = currentPage < totalPages;
        }
        private void load()
        {
            if (trackid != "")
            {
                List<DBData.TrackData> dataList = db.RetrieveTrackData(trackid.ToString());
                if (dataList.Count > 0)
                {
                   
                    label10.Text = dataList[0].totalhours;
                    label4.Text = dataList[0].timein.ToString();
                    if (dataList[0].timeout == null)
                    {
                        label1.Text = "00:00";
                        label7.Text = "1";
                    }
                    else
                    {
                        label1.Text = dataList[0].timeout.ToString();
                    }

                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void guna2ShadowPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2ShadowPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadData();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            currentPage++;
            LoadData();
        }
    }
}
