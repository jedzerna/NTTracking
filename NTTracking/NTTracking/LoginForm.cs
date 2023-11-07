using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;

namespace NTTracking
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        Thread loginThread;
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            loginThread = new Thread(login);
            loginThread.Start();
        }
        private void login()
        {
            guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
            {
                guna2ProgressIndicator1.Visible = true;
                guna2ProgressIndicator1.Start();
                guna2Button1.Enabled = false;
            });
            try
            {

                using (MySqlConnection con = new MySqlConnection("Server=13.127.54.40;Port=3306;Database=ntdbtracking;User=admin;Password=admin;"))
                //using (MySqlConnection con = new MySqlConnection("Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password="))
                {
                    con.Open();
                    con.Close();

                    MySqlCommand cmd = new MySqlCommand("SELECT username, password FROM accounts WHERE BINARY username = @Username AND BINARY password = @Password", con);

                    // Add parameters to the query
                    cmd.Parameters.AddWithValue("@Username", guna2TextBox1.Text);
                    cmd.Parameters.AddWithValue("@Password", guna2TextBox2.Text);


                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {

                            con.Close();

                            con.Open();
                            String query1 = "SELECT * FROM accounts where username like '" + guna2TextBox1.Text.Trim() + "'";
                            MySqlCommand cmd2 = new MySqlCommand(query1, con);
                            MySqlDataReader dr1 = cmd2.ExecuteReader();

                            UserDashboard f = new UserDashboard();
                            if (dr1.Read())
                            {
                                f.username = guna2TextBox1.Text;
                                f.id = (dr1["id"].ToString());


                            }
                            dr1.Close();


                            con.Close();
                            this.Hide();

                            f.ShowDialog();
                            this.Close();

                            loginThread.Abort();
                        }
                        catch (Exception ex)
                        {
                            guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                            {
                                guna2ProgressIndicator1.Visible = false;
                                guna2ProgressIndicator1.Stop();
                                guna2Button1.Enabled = true;
                            });
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                            {
                                guna2ProgressIndicator1.Visible = false;
                                guna2ProgressIndicator1.Stop();
                                guna2Button1.Enabled = true;
                            });
                            if (con != null)
                            {
                                con.Dispose();
                            }
                            if (cmd != null)
                            {
                                cmd.Dispose();
                            }
                            loginThread.Abort();
                        }
                    }
                    else
                    {
                        con.Close();
                        guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                        {
                            guna2ProgressIndicator1.Visible = false;
                            guna2ProgressIndicator1.Stop();
                            guna2Button1.Enabled = true;
                        });
                        MessageBox.Show("Invalid Login please check username and password");
                    } 

                }
            }
            catch (Exception ex)
            {
                guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                {
                    guna2ProgressIndicator1.Visible = false;
                    guna2ProgressIndicator1.Stop();
                    guna2Button1.Enabled = true;
                });
                MessageBox.Show(ex.Message);
            }
        }

        private void guna2ProgressIndicator1_Click(object sender, EventArgs e)
        {

        }
    }
}
