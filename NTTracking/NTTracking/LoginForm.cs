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
using System.IO;

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
            timer2.Start();
        }
        Thread loginT;
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            loginT = new Thread(login);
            loginT.Start();
            //login();
        }
        

        //using (MySqlConnection con = new MySqlConnection("Server=13.127.54.40;Port=3306;Database=ntdbtracking;User=admin;Password=admin;"))
        //using (MySqlConnection con = new MySqlConnection("Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password=;"))
        private void login()
        {
            try
            {
                guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                {
                    guna2ProgressIndicator1.Visible = true;
                    guna2ProgressIndicator1.Start();
                    guna2Button1.Enabled = false;
                });
                using (MySqlConnection con = new MySqlConnection("Server=13.127.54.40;Port=3306;Database=ntdbtracking;User=admin;Password=admin;"))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand("SELECT username, password FROM accounts WHERE BINARY username = @Username AND BINARY password = @Password", con);

                    // Add parameters to the query
                    cmd.Parameters.AddWithValue("@Username", guna2TextBox1.Text);
                    cmd.Parameters.AddWithValue("@Password", guna2TextBox2.Text);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        con.Close();
                        con.Open();
                        String query1 = "SELECT * FROM accounts where username like @Username";
                        MySqlCommand cmd2 = new MySqlCommand(query1, con);
                        cmd2.Parameters.AddWithValue("@Username", guna2TextBox1.Text.Trim());

                        using (MySqlDataReader dr1 = cmd2.ExecuteReader())
                        {
                            if (dr1.Read())
                            {
                                if (dr1["user_image"] != DBNull.Value)
                                {
                                    byte[] imageBytes = (byte[])dr1["user_image"];
                                    img = ByteArrayToImage(imageBytes);
                                }
                                else
                                {
                                    // Handle the case when user_image is null in the database
                                    img = null;
                                }
                                id = dr1["id"].ToString();
                                position = dr1["position"].ToString();
                            }
                            dr1.Close();
                        }

                        con.Close();
                    }
                    else
                    {
                        guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                        {
                            guna2ProgressIndicator1.Visible = false;
                            guna2ProgressIndicator1.Stop();
                            guna2Button1.Enabled = true;
                        });
                        MessageBox.Show("Invalid Login. Please check the username and password.");
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                {
                    guna2ProgressIndicator1.Visible = false;
                    guna2ProgressIndicator1.Stop();
                    guna2Button1.Enabled = true;
                });
            }

        }
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
        private string id = "";
        private string position;
        private Image img;
        private void openDash()
        {
            timer2.Stop();
            timer2.Enabled = false;
            timer1.Stop();
            timer1.Enabled = false;
            UserDashboard f = new UserDashboard();
            f.img = img;
            f.username = guna2TextBox1.Text;
            f.id = id;
            f.position = position;
            this.Hide();
            f.ShowDialog();
            this.Close();
        }

        private void guna2ProgressIndicator1_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("");
            if (id!= "")
            {
                openDash();
            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

            //guna2Transition1.ShowSync(pictureBox2);
            //pictureBox2.Visible = true;


            //guna2Transition2.HideSync(pictureBox4);
            //pictureBox4.Visible = false;
            this.Hide();
            Register reg = new Register();
            reg.ShowDialog();
            this.Close();
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                guna2Button1_Click(sender,e);
                e.Handled = true;
            }
        }

        private void guna2TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                guna2Button1_Click(sender, e);
                e.Handled = true;
            }
        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private string[] animationTexts = {
    "Guarding● ",
    "Guarding your● ",
    "Guarding your path,● ",
    "Guarding your path, monitoring● ",
    "Guarding your path, monitoring with● ",
    "Guarding your path, monitoring with precision",
    "Guarding your path, monitoring with precision.",
    "Guarding your path, monitoring with precision..",
    "Guarding your path, monitoring with precision...",
    "Guarding your path, monitoring with precision....",
    ""
};
        private string[] animationTexts1 = {
    "Securely● ",
    "Securely monitor● ",
    "Securely monitor and● ",
    "Securely monitor and track● ",
    "Securely monitor and track with● ",
    "Securely monitor and track with confidence.",
    "Securely monitor and track with confidence..",
    "Securely monitor and track with confidence...",
    "Securely monitor and track with confidence....",
    ""
};

        private int animationIndex = 0;
        private Random random = new Random();
        int randomNumber = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (label7.Text == "")
            {
               randomNumber = random.Next(1, 3);
            }
            if (randomNumber == 1)
            {
                label7.Text = animationTexts[animationIndex];
                if (animationIndex == 0)
                {
                    guna2Transition1.ShowSync(label7);
                    label7.Visible = true;
                }
                if (animationTexts.Length - 2 == animationIndex)
                {
                    guna2Transition1.HideSync(label7);
                    label7.Visible = false;
                }
                animationIndex = (animationIndex + 1) % animationTexts.Length;
            }else if (randomNumber == 2)
            {
                label7.Text = animationTexts1[animationIndex];
                if (animationIndex == 0)
                {
                    guna2Transition1.ShowSync(label7);
                    label7.Visible = true;
                }
                if (animationTexts1.Length - 2 == animationIndex)
                {
                    guna2Transition1.HideSync(label7);
                    label7.Visible = false;
                }
                animationIndex = (animationIndex + 1) % animationTexts1.Length;
            }
        }
    }
}
