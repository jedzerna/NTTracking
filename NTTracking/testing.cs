using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NTTracking
{
    public partial class testing : Form
    {
        private Thread showappsThread;
        public testing()
        {
            InitializeComponent();
        }

        private void testing_Load(object sender, EventArgs e)
        {
           timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (showappsThread == null)
            {
                showappsThread = new Thread(LoadProcessesOnUIThread);
                showappsThread.Start();
            }
        }
        public class ProcessInfo
        {
            public string Software { get; set; }
        }
        public class ProcessInfoTemp
        {
            public string Software { get; set; }
        }
        DBData db = new DBData();
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

                    if (db.AddTaskRunning("1", "26", itemT.Software, catid))
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
                    if (db.CloseTaskRunning("1", "26", item.Software))
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
    }
}
