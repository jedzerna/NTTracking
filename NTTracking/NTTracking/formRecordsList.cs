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
        private void formRecordsList_Load(object sender, EventArgs e)
        {
            dataGridView2.DataSource = db.GetAllPreviousRecord(id);
        }
        formRecords dasha = (formRecords)Application.OpenForms["formRecords"];
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string newData = dataGridView2.CurrentRow.Cells["Column1"].Value.ToString();

            parentForm.trackid = dataGridView2.CurrentRow.Cells["Column1"].Value.ToString();
            parentForm.recordinfo();
        }
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
