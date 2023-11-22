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
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace NTTracking
{
    public partial class Register : Form
    {

        public Register()
        {
            InitializeComponent();
            InitializeTabOrder();
        }

        private void InitializeTabOrder()
        {
            first_name_txt.TabIndex = 1;
            last_name_txt.TabIndex = 2;
            username_txt.TabIndex = 3;
            position_dropdown.TabIndex = 4;
            email_txt.TabIndex = 5;
            pass_txt.TabIndex = 6;
            confirm_pass_txt.TabIndex = 7;
            register_btn.TabIndex = 8;
            LoginTextLink.TabIndex = 9;
        }
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
            position_dropdown.Items.AddRange(new object[]
            {
                "Software Engineer",
                "Junior Software Engineer",
                "Associate Software Engineer",
                "Senior Software Engineer",
                "IT Support Engineer",
                "Intern",
                "Pre-editor - Journal",
                "Pre-editor - Book",
                "Copyeditor - Journal",
                "Copyeditor - Book",
                "Quality Assurance",
                "Proof Collator",
                "Supervisor",
                "Manager",
                "VP",
                "President"
            });

            pictureBox2.Controls.Add(position_dropdown);
            position_dropdown.Location = new Point(17, 307);
            position_dropdown.BackColor = Color.Transparent;
            pictureBox2.Controls.Add(last_name_txt);
            last_name_txt.Location = new Point(17, 242);
            last_name_txt.BackColor = Color.Transparent;
            pictureBox2.Controls.Add(department_txt);
            department_txt.Location = new Point(17, 366);
            department_txt.BackColor = Color.Transparent;
            pictureBox2.TabStop = true; // Set tab stop for PictureBox
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

            // Validate first_name_txt, last_name_txt, and username_txt
            if (!IsValidName(first_name_txt.Text) || !IsValidName(last_name_txt.Text))
            {
                MessageBox.Show("Invalid characters in First Name or Last Name. Please use only letters." + br + br + "Registration Failed!");
            }
            else if (!IsValidUsername(username_txt.Text))
            {
                MessageBox.Show("Invalid characters in Username. Please use only letters and clear whitespaces." + br + br + "Registration Failed!");
            }
            else if (!IsValidEmail(email_txt.Text))
            {
                MessageBox.Show("Invalid email address. Please enter a valid email." + br + br + "Registration Failed!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(first_name_txt.Text) || string.IsNullOrWhiteSpace(last_name_txt.Text) || string.IsNullOrWhiteSpace(department_txt.Text) ||
                     string.IsNullOrWhiteSpace(username_txt.Text) || string.IsNullOrWhiteSpace(pass_txt.Text) || string.IsNullOrWhiteSpace(email_txt.Text))
            {
                MessageBox.Show("All Fields are required!" + br + br + "Registration Failed!");
            }
            else if (pass_txt.Text != confirm_pass_txt.Text)
            {
                MessageBox.Show("Password does not match!" + br + br + "Registration Failed!");
            }
            else
            {
                MessageBox.Show("Password does not match!" + br + br + "Registration Failed!");
                registerT = new Thread(register);
                registerT.Start();
            }
        }

        private bool IsValidName(string input)
        {
            // Use regular expression to allow only letters and spaces
            Regex regex = new Regex("^[a-zA-Z ]+$");
            return regex.IsMatch(input);
        }

        private bool IsValidUsername(string input)
        {
            // Use regular expression to allow only letters
            Regex regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(input);
        }

        private bool IsValidEmail(string email)
        {
            // Use regular expression for email validation
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }

        private void register()
        {
            try
            {
                // UI updates
                guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                {
                    guna2ProgressIndicator1.Visible = true;
                    guna2ProgressIndicator1.Start();
                    guna2Button1.Enabled = false;
                });

                // Database connection string
                string connectionString = "Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password=;";


                // SQL query
                string query = "INSERT INTO accounts(user_image, first_name, last_name, position, department, username, password, email, type, status)" +
                    "VALUES (@user_image, @first_name_txt, @last_name_txt, @position_dropdown, @department_txt, @username_txt, @pass_txt, @email_txt, 'User', 'Pending');";


                // Check if USERNAME already exists
                if (IsUsernameExists(username_txt.Text, connectionString))
                {
                    MessageBox.Show("Username already exists. Registration Failed!");
                    return; // Exit the registration process if USERNAME already exists
                }


                // Check if EMAIL already exists
                if (IsEmailExists(email_txt.Text, connectionString))
                {
                    MessageBox.Show("Email already exists. Registration Failed!");
                    return; // Exit the registration process if EMAIL already exists
                }


                // Database operations
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@user_image", DBNull.Value);
                        command.Parameters.AddWithValue("@first_name_txt", first_name_txt.Text.Trim());
                        command.Parameters.AddWithValue("@last_name_txt", last_name_txt.Text.Trim());
                        command.Parameters.AddWithValue("@position_dropdown", selected);
                        command.Parameters.AddWithValue("@department_txt", department_txt.Text.Trim());
                        command.Parameters.AddWithValue("@username_txt", username_txt.Text.Trim());
                        command.Parameters.AddWithValue("@pass_txt", pass_txt.Text.Trim());
                        command.Parameters.AddWithValue("@email_txt", email_txt.Text.Trim());

                        connection.Open();

                        // Execute the query
                        command.ExecuteNonQuery();

                        // Display success message
                        StringBuilder successMessage = new StringBuilder();
                        successMessage.AppendLine("Registration Successful!");
                        successMessage.AppendLine();
                        successMessage.AppendLine("Please contact your direct manager to activate your account.");
                        MessageBox.Show(successMessage.ToString());

                        success = "1";
                        registerT = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // UI updates
                guna2ProgressIndicator1.BeginInvoke((Action)delegate ()
                {
                    guna2ProgressIndicator1.Visible = false;
                    guna2ProgressIndicator1.Stop();
                    guna2Button1.Enabled = true;
                });
            }
        }



        private bool IsUsernameExists(string username, string connectionString)
        {
            // SQL query to check if the USERNAME already exists
            string query = "SELECT COUNT(*) FROM accounts WHERE username = @username";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    connection.Open();

                    // Execute the query
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // If count is greater than 0, the email already exists
                    return count > 0;
                }
            }
        }


        private bool IsEmailExists(string email, string connectionString)
        {
            // SQL query to check if the EMAIL already exists
            string query = "SELECT COUNT(*) FROM accounts WHERE email = @email";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();

                    // Execute the query
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // If count is greater than 0, the email already exists
                    return count > 0;
                }
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

        private void label11_Click(object sender, EventArgs e)
        {

        }



        private void Register_combo(object sender, EventArgs e)
        {
            // Populate the positions in the combo box

        }

        private void department_txt_TextChanged(object sender, EventArgs e)
        {

        }


        private void department_txt_Load(object sender, EventArgs e)
        {

        }
        private string selected;
        private void position_dropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the selected item is not null
            if (position_dropdown.SelectedItem != null)
            {
                selected = position_dropdown.SelectedItem.ToString();
                // Get the selected item from the combo box
                string selectedPosition = position_dropdown.SelectedItem.ToString();

                // Set the department_txt based on the selected position
                if (selectedPosition == "Software Engineer" ||
                    selectedPosition == "Junior Software Engineer" ||
                    selectedPosition == "Associate Software Engineer" ||
                    selectedPosition == "Senior Software Engineer" ||
                    selectedPosition == "IT Support Engineer")
                {
                    department_txt.Text = "IT Department";
                }
                else if (selectedPosition == "Pre-editor - Journal" ||
                         selectedPosition == "Pre-editor - Book" ||
                         selectedPosition == "Copyeditor - Journal" ||
                         selectedPosition == "Copyeditor - Book" ||
                         selectedPosition == "Proof Collator" ||
                         selectedPosition == "Quality Assurance")
                {
                    department_txt.Text = "Production";
                }
                else if (selectedPosition == "Supervisor" ||
                         selectedPosition == "Manager" ||
                         selectedPosition == "VP" ||
                         selectedPosition == "President")
                {
                    department_txt.Text = "Management";
                }
                else
                {
                    // Handle other positions or set a default value for the department_txt
                    department_txt.Text = "Unknown Department";
                }
            }
        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }

        private void Register_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Register_MouseDown(object sender, MouseEventArgs e)
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

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void first_name_txt_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Tab)
            //{
            //    last_name_txt.Focus();
            //}
        }
    }
}
