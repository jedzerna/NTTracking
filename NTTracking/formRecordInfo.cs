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
        private void formRecordInfo_Load(object sender, EventArgs e)
        {
            load();
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
                    }
                    else
                    {
                        label1.Text = dataList[0].timeout.ToString();
                    }

                    if (dataList[0].timebreakin.ToString() == "00:00:00")
                    {
                        label12.Text = "00:00";
                    }
                    else
                    {
                        label12.Text = dataList[0].timebreakin.ToString();
                    }
                    if (dataList[0].timebreakout.ToString() == "00:00:00")
                    {
                        label3.Text = "00:00";
                    }
                    else
                    {
                        label3.Text = dataList[0].timebreakout.ToString();
                    }
                }
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
