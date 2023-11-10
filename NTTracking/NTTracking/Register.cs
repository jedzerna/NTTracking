using Guna.UI2.WinForms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace NTTracking
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm reg = new LoginForm();
            reg.ShowDialog();
            this.Close();
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void guna2ProgressIndicator1_Click(object sender, EventArgs e)
        {

        }

        Thread registerT;

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            var br = Environment.NewLine;
            if (first_name_txt.Text == "" || last_name_txt.Text == "" || position_txt.Text == "" || username_txt.Text == "" || pass_txt.Text == "" || email_txt.Text == "")
            {
                MessageBox.Show("All Fields are required!" + br + br + br + "Registration Failed!");
            }
            else if (pass_txt.Text != confirm_pass_txt.Text)
            {
                MessageBox.Show("Password does not match!" + br + br + br + "Registration Failed!");
            }
            else
            {
                registerT = new Thread(register);
                registerT.Start();
            }
        }

        private void register()
        {
            try
            {
                guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                {
                    guna2ProgressIndicator1.Visible = true;
                    guna2ProgressIndicator1.Start();
                    guna2Button1.Enabled = false;
                });

                String MyConnection = "Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password=;";

                String MySqlQuery = "INSERT INTO accounts(first_name, last_name, position, username, password, email, status) VALUES ('" +first_name_txt.Text.Trim()+ "', '" + last_name_txt.Text.Trim() + "', '"+position_txt.Text.Trim()+"', '"+username_txt.Text.Trim()+"', '"+pass_txt.Text.Trim()+"', '"+email_txt.Text.Trim()+"', 'Pending');";
                MySqlConnection MyConnection2 = new MySqlConnection(MyConnection);
                MySqlCommand RegCommand = new MySqlCommand(MySqlQuery, MyConnection2);

                MySqlDataReader MyReader;

                MyConnection2.Open();
                MyReader = RegCommand.ExecuteReader();

                if (true)
                {
                    var br = Environment.NewLine;
                    MessageBox.Show("Registration Successful!"+ br + br + br +"Please contact your direct manager to activate your account.");
                    success = "1";
                    registerT = null;
                }
                

                while (MyReader.Read())
                {
                    
                }

                MyConnection2.Close();

                // Add parameters to the query
                //cmd.Parameters.AddWithValue("@Username", first_name_txt.Text);
                //cmd.Parameters.AddWithValue("@Password", pass_txt.Text);

                //MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                //DataTable dt = new DataTable();
                //da.Fill(dt);

            }
            catch (Exception ex)
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
        string success = "";
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (success != "")
            {
                timer1.Stop();
                timer1.Enabled = false;
                this.Hide();
                LoginForm reg = new LoginForm();
                reg.ShowDialog();
                this.Close();
            }
        }
    }
}
