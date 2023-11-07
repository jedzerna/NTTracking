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

namespace NTTracking
{
    public partial class LoginForm : Form
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
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            login();
        }
        private void login()
        {
            try
            {

                using (MySqlConnection con = new MySqlConnection("Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password="))
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
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            if (con != null)
                            {
                                con.Dispose();
                            }
                            if (cmd != null)
                            {
                                cmd.Dispose();
                            }
                        }
                    }
                    else
                    {
                        con.Close();
                        MessageBox.Show("Invalid Login please check username and password");
                    } 

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
