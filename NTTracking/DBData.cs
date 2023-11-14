using Guna.UI2.WinForms.Suite;
using MySql.Data.MySqlClient;
using NTTracking.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace NTTracking
{
    internal class DBData
    {
        private MySqlConnection connection;
        private string connectionString;

        public DBData()
        {
            // Initialize the connection string
            //connectionString = $"Server=13.127.54.40;Port=3306;Database=ntdbtracking;User=admin;Password=admin;";
            connectionString = $"Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password=";

            // using (MySqlConnection con = new MySqlConnection("Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password="))
            // Create a new MySqlConnection using the connection string
            connection = new MySqlConnection(connectionString);
        }

        public bool OpenConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                // Handle any connection error
                // You can log or display an error message here
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                // Handle any connection error
                // You can log or display an error message here
                return false;
            }
        }

        public bool TimeIn(string userid, DateTime datetimein)
        {
            if (this.OpenConnection())
            {
                try
                {
                    //string timein = datetimein.ToString(@"HH\:mm\:ss");
                    //string datein = datetimein.ToString("dd-MM-yyyy");
                    // Use parameterized query to avoid SQL injection
                    string query = "INSERT INTO tbltrackrecords (userid, timein,datein,timeout) VALUES (@userid, @timein, @datein, @timeout)";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@timein", datetimein.ToString(@"HH\:mm\:ss"));
                    cmd.Parameters.AddWithValue("@timeout", DBNull.Value);
                    //DateTime dateinDate = DateTime.Parse(datein);
                    cmd.Parameters.AddWithValue("@datein", datetimein.ToString("yyyy-MM-dd"));

                    cmd.ExecuteNonQuery();


                    this.CloseConnection();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("MySQL Exception: " + ex.Message);
                    MessageBox.Show(ex.Message);
                    this.CloseConnection();
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    // Handle other exceptions
                    Console.WriteLine("Exception: " + ex.Message);
                    this.CloseConnection();
                    return false;
                }
            }
            return false;
        }


        public bool TimeOut(string id, DateTime datetimeout)
        {
            if (this.OpenConnection())
            {
                try
                {
                    // Use parameterized query to avoid SQL injection
                    string query = "UPDATE tbltrackrecords SET id=@id,timeout=@timeout,dateout=@dateout WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@timeout", datetimeout.ToString(@"HH\:mm\:ss"));
                    //DateTime dateoutD = DateTime.Parse(dateout);
                    cmd.Parameters.AddWithValue("@dateout", datetimeout.ToString("yyyy-MM-dd"));



                    cmd.ExecuteNonQuery();


                    this.CloseConnection();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("MySQL Exception: " + ex.Message);
                    this.CloseConnection();
                    return false;
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Console.WriteLine("Exception: " + ex.Message);
                    this.CloseConnection();
                    return false;
                }
            }
            return false;
        }
        public int GetHighestId(string userid, DateTime datein)
        {
            int highestId = 0;

            if (this.OpenConnection())
            {
                string querya = "SELECT MAX(id) FROM tbltrackrecords WHERE datein = @datein AND timeout IS NULL AND userid = @userid";

                using (MySqlCommand cmda = new MySqlCommand(querya, connection))
                {
                    //MessageBox.Show(datein.ToString());
                    //DateTime dateinDate = DateTime.Parse(datein);
                    cmda.Parameters.AddWithValue("@datein", datein.ToString("yyyy-MM-dd"));
                    cmda.Parameters.AddWithValue("@userid", userid);

                    using (MySqlDataReader readera = cmda.ExecuteReader())
                    {
                        if (readera.Read() && !readera.IsDBNull(0))
                        {
                            highestId = readera.GetInt32(0);
                        }
                    }
                }

                this.CloseConnection();
            }

            return highestId;
        }
        public class MyData
        {
            public string YourProperty1 { get; set; } // Replace with actual property type
            public string YourProperty2 { get; set; } // Replace with actual property type
                                                      // Add more properties as needed
        }
        public List<MyData> RetrieveData(string id)
        {
            List<MyData> dataList = new List<MyData>();

            if (this.OpenConnection())
            {
                try
                {
                    string query = "SELECT * FROM tbltrackrecords WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MyData data = new MyData
                            {
                                // Map database columns to your data type properties
                                YourProperty1 = reader["timein"].ToString(),
                                YourProperty2 = reader["userid"].ToString(),
                            };

                            dataList.Add(data);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Handle retrieval error
                    // Log or display an error message here
                }
                finally
                {
                    this.CloseConnection();
                }
            }

            return dataList;
        }

     
        public int GetAnomalies(string userid)
        {
            DateTime recentDate = DateTime.Now.AddDays(-1);
            DateTime firstDate = DateTime.Now;
            string recentDateF = recentDate.ToString("yyyy-MM-dd");
            string firstDateF = firstDate.ToString("yyyy") + firstDate.ToString("MM")  + "-" + "-01";

            int highestId = 0;

            if (this.OpenConnection())
            {
                string query = "SELECT COUNT(*) FROM tbltrackrecords WHERE userid = @userid AND datein BETWEEN @startDate AND @endDate AND timeout IS NULL"; // Replace with your actual table name

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@startDate", firstDateF); // Set your start date parameter
                    cmd.Parameters.AddWithValue("@endDate", recentDateF);
                    int rowCount = Convert.ToInt32(cmd.ExecuteScalar()); // Use ExecuteScalar to get the count directly

                    highestId = rowCount;
                }

                this.CloseConnection();
            }

            return highestId;
        }
        public string CalculateTimeDifference(string userid,  DateTime dateto)
        {
            TimeSpan totalTimeDifference = TimeSpan.Zero;
            if (this.OpenConnection())
            {
                string datefrom = DateTime.Now.ToString("yyyy") + "-" + DateTime.Now.ToString("MM") + "-" + "01";
                //MessageBox.Show(userid);
                string query = "SELECT timein, timeout FROM tbltrackrecords WHERE userid=@userid AND dateout BETWEEN @startDate AND @endDate AND timeout IS NOT NULL";
                MySqlCommand command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@userid", userid);
                DateTime datefromD = DateTime.Parse(datefrom);
                command.Parameters.AddWithValue("@startDate", datefromD.ToString("yyyy-MM-dd"));
                //DateTime datetoD = DateTime.Parse(dateto);
                command.Parameters.AddWithValue("@endDate", dateto.ToString("yyyy-MM-dd"));

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);



                    foreach (DataRow row in dataTable.Rows)
                    {
                        TimeSpan startTime = (TimeSpan)row["timein"];
                        TimeSpan endTime = (TimeSpan)row["timeout"];
                        TimeSpan timeDifference = endTime - startTime;


                        totalTimeDifference += timeDifference;
                    }

                }
                this.CloseConnection();
            }

            return totalTimeDifference.ToString();
        }
        public DataTable GetPreviousRecord(string userid)
        {
            DataTable dt = new DataTable();
            DataTable fdt = new DataTable();
            fdt.Columns.Add("Records",typeof(DateTime));
            fdt.Columns.Add("Image", typeof(Bitmap));
            if (this.OpenConnection())
            {
                string query = "SELECT DATE_FORMAT(datein, '%Y-%m-%d %H:%i:%s') AS datein, " +
                  "DATE_FORMAT(dateout, '%Y-%m-%d %H:%i:%s') AS dateout, timeout " +
                  "FROM tbltrackrecords WHERE userid = @userid ORDER BY id DESC LIMIT 10";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userid", userid);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow frow = fdt.NewRow();
                        //MessageBox.Show(DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy"));
                        //MessageBox.Show(DateTime.Now.ToString("MM/dd/yyyy"));
                        if (row["timeout"] is DBNull || string.IsNullOrEmpty(row["timeout"].ToString()) && DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy") == DateTime.Now.ToString("MM/dd/yyyy"))
                        {
                            string dat = row["datein"].ToString();
                            string formattedDate = dat.Remove(dat.Length - 9);
                            //.Show(formattedDate);
                            //frow["Records"] = DateTime.Parse(formattedDate).ToString("MM/dd/yyyy");
                            frow["Records"] = DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy");              //frow["Records"] = DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy");
                            frow["Image"] = new Bitmap(Properties.Resources.circlered2);
                        }
                        else if (row["timeout"] is DBNull || string.IsNullOrEmpty(row["timeout"].ToString()) && DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy") != DateTime.Now.ToString("MM/dd/yyyy"))
                        {
                            frow["Records"] = DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy");
                            frow["Image"] = new Bitmap(Properties.Resources.circlered2);
                        }
                        else
                        {
                            frow["Records"] = DateTime.Parse(row["dateout"].ToString()).ToString("MM/dd/yyyy");
                            frow["Image"] = new Bitmap(Properties.Resources.circlegreen2);
                        }

                        fdt.Rows.Add(frow);
                    }

                    this.CloseConnection();
                }
            }
            return fdt;
        }
        public DataTable GetAllPreviousRecord(string userid)
        {
            DataTable dt = new DataTable();
            DataTable fdt = new DataTable();
            fdt.Columns.Add("id");
            fdt.Columns.Add("Records");
            fdt.Columns.Add("Image", typeof(Bitmap));
            if (this.OpenConnection())
            {
                string query = "SELECT DATE_FORMAT(datein, '%Y-%m-%d %H:%i:%s') AS datein, " +
                  "DATE_FORMAT(dateout, '%Y-%m-%d %H:%i:%s') AS dateout, timeout,id " +
                  "FROM tbltrackrecords WHERE userid = @userid ORDER BY id DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@userid", userid);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow frow = fdt.NewRow();
                        //MessageBox.Show(DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy"));
                        //.Show(row["id"].ToString());
                        frow["id"] = row["id"].ToString();
                        if (row["timeout"] is DBNull || string.IsNullOrEmpty(row["timeout"].ToString()) && DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy") == DateTime.Now.ToString("MM/dd/yyyy"))
                        {
                            frow["Records"] = DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy");
                            frow["Image"] = new Bitmap(Properties.Resources.circlered2);
                        }
                        else if (row["timeout"] is DBNull || string.IsNullOrEmpty(row["timeout"].ToString()) && DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy") != DateTime.Now.ToString("MM/dd/yyyy"))
                        {
                            frow["Records"] = DateTime.Parse(row["datein"].ToString()).ToString("MM/dd/yyyy");
                            frow["Image"] = new Bitmap(Properties.Resources.circlered2);
                        }
                        else
                        {
                            frow["Records"] = DateTime.Parse(row["dateout"].ToString()).ToString("MM/dd/yyyy");
                            frow["Image"] = new Bitmap(Properties.Resources.circlegreen2);
                        }

                        fdt.Rows.Add(frow);
                    }

                    this.CloseConnection();
                }
            }
            return fdt;
        }
        public bool AddTaskRunning(string userid, string taskid, string description)
        {
            if (this.OpenConnection())
            {
                try
                {
                    string query = "INSERT INTO tbltaskrunning (userid,taskid, description, date,time, status) VALUES (@userid,@taskid, @description, @date,@time, @status)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userid", userid);
                        cmd.Parameters.AddWithValue("@taskid", taskid);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@status", "Opening");

                        cmd.ExecuteNonQuery();
                    }
                    this.CloseConnection();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("MySQL Exception: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Console.WriteLine("Exception: " + ex.Message);
                }
                finally
                {
                    this.CloseConnection();
                }
            }
            return false;
        }

        public bool CloseTaskRunning(string userid, string taskid, string description)
        {
            if (this.OpenConnection())
            {
                try
                {
                    string query = "INSERT INTO tbltaskrunning (userid, taskid,description, date,time, status) VALUES (@userid, @taskid,@description, @date,@time, @status)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userid", userid);
                        cmd.Parameters.AddWithValue("@taskid", taskid);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@status", "Closing");

                        cmd.ExecuteNonQuery();
                    }
                    this.CloseConnection();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("MySQL Exception: " + ex.Message);
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Console.WriteLine("Exception: " + ex.Message);
                }
                finally
                {
                    this.CloseConnection();
                }
            }
            return false;
        }

        public class TrackData
        {
            public string timein { get; set; } 
            public string datein { get; set; }
            public string timebreakin { get; set; }
            public string datebreakin { get; set; }
            public string timebreakout { get; set; }
            public string datebreakout { get; set; }
            public string timeout { get; set; }
            public string dateout { get; set; }
            public string totalhours { get; set; }
        }
        public List<TrackData> RetrieveTrackData(string taskid)
        {
            List<TrackData> dataList = new List<TrackData>();

            if (this.OpenConnection())
            {
                try
                {
                    string query = "SELECT " +
                                    "DATE_FORMAT(timein, '%H:%i:%s') AS timein, " +
                                    "DATE_FORMAT(datein, '%Y-%m-%d') AS datein, " +
                                    "DATE_FORMAT(timebreakin, '%H:%i:%s') AS timebreakin, " +
                                    "DATE_FORMAT(datebreakin, '%Y-%m-%d') AS datebreakin, " +
                                    "DATE_FORMAT(timebreakout, '%H:%i:%s') AS timebreakout, " +
                                    "DATE_FORMAT(datebreakout, '%Y-%m-%d') AS datebreakout, " +
                                    "DATE_FORMAT(timeout, '%H:%i:%s') AS timeout, " +
                                    "DATE_FORMAT(dateout, '%Y-%m-%d') AS dateout " +
                                    "FROM tbltrackrecords WHERE id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", taskid);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TimeSpan startTime = TimeSpan.Parse(ParseTime(reader["timein"]));
                            TimeSpan endTime = TimeSpan.Zero;
                            if (reader["timeout"] == DBNull.Value || reader["timeout"].ToString().Trim() == "")
                            {
                                endTime = TimeSpan.Zero;
                            }
                            else
                            {
                                endTime = TimeSpan.Parse(ParseTime(reader["timeout"]));
                            }
                            TimeSpan timeDifference = TimeSpan.Zero;
                            if (endTime != TimeSpan.Zero)
                            {
                                 timeDifference = endTime - startTime;
                            }
                            else
                            {
                                 timeDifference = TimeSpan.Zero;
                            }
                            TrackData data = new TrackData
                            {
                                timein = ParseTime(reader["timein"]),
                                datein = ParseDate(reader["datein"]),
                            timebreakin = ParseTime(reader["timebreakin"]),
                                datebreakin = ParseDate(reader["datebreakin"]),
                                timebreakout = ParseTime(reader["timebreakout"]),
                                datebreakout = ParseDate(reader["datebreakout"]),
                                timeout = ParseTime(reader["timeout"]),
                                dateout = ParseDate(reader["dateout"]),
                                // Map database columns to your data type properties
                                //timein = (TimeSpan)reader["timein"],
                                //datein = DateTime.Parse(reader["datein"].ToString()),
                                //timebreakin = (TimeSpan)reader["timebreakin"],
                                //datebreakin = DateTime.Parse(reader["datebreakin"].ToString()),
                                //timebreakout = (TimeSpan)reader["timebreakout"],
                                //datebreakout = DateTime.Parse(reader["datebreakout"].ToString()),
                                //timeout = (TimeSpan)reader["timeout"],
                                //dateout = DateTime.Parse(reader["dateout"].ToString()),
                                totalhours = timeDifference.ToString(),

                            };

                            dataList.Add(data);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Handle retrieval error
                    // Log or display an error message here
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    // Handle retrieval error
                    // Log or display an error message here
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.CloseConnection();
                }
            }

            return dataList;
        }
        private static string ParseTime(object dbValue)
        {
            if (dbValue != DBNull.Value && DateTime.TryParse(dbValue.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result.ToString("HH:mm:ss");
            }
            else
            {
                // Handle the case where the date/time value cannot be parsed.
                // This could be logging an error or setting a default value, for example.
                return null; // or some default value
            }
        }
        private static string ParseDate(object dbValue)
        {
            if (dbValue != DBNull.Value && DateTime.TryParse(dbValue.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result.ToString("MM/dd/yyyy");
            }
            else
            {
                // Handle the case where the date/time value cannot be parsed.
                // This could be logging an error or setting a default value, for example.
                return null; // or some default value
            }
        }
    }
}

