using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace NTTracking
{
    public partial class UsersAccount : Form
    {
        private const string ConnectionString = "Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password=;";

        public string id;
        private bool IsEditMode = false;
        private bool isDarkMode = false;
        private DBData db = new DBData();
        private string v = Environment.NewLine; // Move this outside of methods

        public UsersAccount()
        {
            InitializeComponent();
            ApplyTheme();
        }
        bool loading = false;
        private void UsersAccount_Load(object sender, EventArgs e)
        {
            loading = true;
            guna2ComboBox1.Items.AddRange(new object[]
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


            LoadData();
            loading = false;
        }
        private void LoadData()
        {
            List<DBData.AccountData> retrievedata = db.RetrieveAccountData(id);
            if (retrievedata.Count > 0)
            {
                guna2TextBox5.Text = retrievedata[0].FirstName.ToString();
                guna2TextBox6.Text = retrievedata[0].LastName.ToString();
                guna2ComboBox1.Text = retrievedata[0].Position.ToString();
                guna2TextBox2.Text = retrievedata[0].Email.ToString();

                //guna2TextBox4.Text = retrievedata[0].Department.ToString();
                guna2TextBox3.Text = retrievedata[0].PhoneNo.ToString();
                guna2TextBox10.Text = retrievedata[0].Username.ToString();
                guna2TextBox11.Text = "***********";

                label7.Text = retrievedata[0].Position.ToString();
                label8.Text = $"{retrievedata[0].FirstName} {retrievedata[0].LastName}";

                DisableTextFields();

            }
        }



        private void DisableTextFields()
        {
            guna2TextBox5.ReadOnly = true;
            guna2TextBox6.ReadOnly = true;
            guna2TextBox4.ReadOnly = true;
            guna2TextBox11.ReadOnly = true;
            guna2TextBox2.ReadOnly = true;
            guna2TextBox3.ReadOnly = true;
            guna2TextBox10.ReadOnly = true;
            guna2TextBox12.ReadOnly = true;
            guna2TextBox13.ReadOnly = true;


        }

  

        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;

            if (IsEditMode)
            {
                EnableTextFields();
            }
            else
            {
                SaveData();
                DisableTextFields();
                LoadData();
            }
        }
        private void EnableTextFields() // Corrected method name
        {
            guna2TextBox5.ReadOnly = false;
            guna2TextBox6.ReadOnly = false;
            guna2TextBox4.ReadOnly = false;
            guna2TextBox11.ReadOnly = true;
            guna2TextBox2.ReadOnly = false;
            guna2TextBox3.ReadOnly = false;
            guna2TextBox10.ReadOnly = false;
            guna2TextBox12.ReadOnly = false;
            guna2TextBox13.ReadOnly = false;
        }



        // Modify your btnSave_Click method
        private void SaveData()
        {
            if (guna2TextBox12.Text != guna2TextBox13.Text)
            {
                MessageBox.Show("Password doesn't match...");
                return;
            }
            string editedFirstName = guna2TextBox5.Text;
            string editedLastName = guna2TextBox6.Text;
            string editedPosition = guna2ComboBox1.Text;
            string editedDepartment = guna2TextBox4.Text;
            string editedEmail = guna2TextBox2.Text;
            string editedPhoneNo = guna2TextBox3.Text;
            string editedUsername = guna2TextBox10.Text;
            /*string editedPassword = guna2TextBox11.Text;*/

            //Check if the password change
            string editedPassword = guna2TextBox12.Text;

            bool isSaved = db.SaveEditedData(id, editedFirstName, editedLastName, editedPosition, editedDepartment, editedEmail, editedPhoneNo,
                editedUsername, editedPassword);

            if (isSaved)
            {
                MessageBox.Show("Data saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to save data. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            ToggleEditMode();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (IsEditMode)
            {
                SaveData();
                ToggleEditMode();
            }
        }
        /// <summary>
        /// DarkTheme
        /// </summary>
    private void ApplyTheme()
        {
            if (isDarkMode)
            {
                //Dark mode
                this.BackColor = Color.FromArgb(30, 30, 30);
                foreach (Control control in this.Controls)
                {
                    ApplyDarkThemeToControl(control);
                }
            }
            else
            {
                // Light mode
                this.BackColor = Color.White;
                foreach (Control control in this.Controls)
                {
                    ApplyLightThemeToControl(control);
                }
            }
        }

        private void ApplyDarkThemeToControl(Control control)
        {
            if (control is Button)
            {
                control.BackColor = Color.FromArgb(50, 50, 50);
                control.ForeColor = Color.White;
            }
            else if (control is TextBox)
            {
                control.BackColor = Color.FromArgb(40, 40, 40);
                control.ForeColor = Color.White;
            }
        }

        private void ApplyLightThemeToControl(Control control)
        {
            if (control is Button)
            {
                control.BackColor = SystemColors.Window;
                control.ForeColor = SystemColors.ControlText;

            }

        }







        private void label6_Click(object sender, EventArgs e)
        {

        }


        private void guna2ShadowPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        /*private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            byte[] imageData = db.RetrieveProfileImage(id); // Assuming you have a method in your DBData class to retrieve the image

            if (imageData != null && imageData.Length > 0)
            {
                // Convert the byte array to an Image
                Image profileImage = ByteArrayToImage(imageData);

                // Display the image in the guna2CirclePictureBox1
                guna2CirclePictureBox1.Image = profileImage;
            }
            else
            {
                // If there's no image data, you can set a default image or handle it accordingly
                guna2CirclePictureBox1.Image = null; // Set a default image or handle accordingly
            }
        }*/

        // Convert a byte array to Image
        private Image ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        private void guna2VTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2CirclePictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            this.Hide();
            UsersAccount dash = new UsersAccount();
            dash.ShowDialog();
            this.Close();
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            //this.Hide();
            //UsersAccount dash = new UsersAccount();
            //dash.id = id;
            //dash.ShowDialog();
            //this.Close();

            guna2ShadowPanel3.Visible = false;
            guna2ShadowPanel2.Visible = true;
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            //this.Hide();
            //UsersAccount dash = new UsersAccount();
            //dash.id = id;
            //dash.ShowDialog();
            //this.Close();
            guna2ShadowPanel3.Visible = true;
            guna2ShadowPanel2.Visible = false;

        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {

            guna2ShadowPanel4.Visible = true;
            guna2ShadowPanel3.Visible = false;

        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2ShadowPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox6_TextChanged(object sender, EventArgs e)
        {
            {
                if (ValidateInput())
                {
                    ToggleEditMode();
                }
            }

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            /*ToggleEditMode();*/
        }

        private void label6_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
                ToggleEditMode();
        }

        private bool ValidateInput()
        {
            bool isValid = true;  // Assume input is valid by default
            if (!loading)
            {

                // Validate first name and last name
                if (!IsValidName(guna2TextBox5.Text) || !IsValidName(guna2TextBox6.Text))
                {
                    MessageBox.Show("Invalid characters in First Name or Last Name. Please use only letters." + v + v + "Edit Failed!");
                    isValid = false;
                }
                else if (!IsValidUsername(guna2TextBox10.Text))
                {
                    MessageBox.Show("Invalid characters in Username. Please use only letters and clear whitespaces." + v + v + "Edit Failed!");
                }
                else if (!IsValidEmail(guna2TextBox2.Text) && guna2TextBox2.Text != "")
                {
                    MessageBox.Show("Invalid email address." + v + v + "Edit Failed!");
                    isValid = false;
                }
                else if (string.IsNullOrWhiteSpace(guna2TextBox5.Text) || string.IsNullOrWhiteSpace(guna2TextBox6.Text) || string.IsNullOrWhiteSpace(guna2TextBox4.Text) ||
                         string.IsNullOrWhiteSpace(guna2TextBox10.Text) || string.IsNullOrWhiteSpace(guna2TextBox12.Text) || string.IsNullOrWhiteSpace(guna2TextBox2.Text))
                {
                    MessageBox.Show("All Fields are required!" + v + v + "Edit Failed!");
                    isValid = false;
                }
                else if (guna2TextBox12.Text != guna2TextBox13.Text)
                {
                    MessageBox.Show("Password does not match!" + v + v + "Edit Failed!");
                    isValid = false;
                }

            }
            return isValid;
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
        private bool IsValidEmail(string input)
        {
            // Use regular expression to allow only letters
            Regex regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(input);
        }




        private void guna2Button6_Click(object sender, EventArgs e)
         {
            if (ValidateInput())
            {
                ToggleEditMode();
            }
               
         }
       

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the selected item is not null
            if (guna2ComboBox1.SelectedItem != null)
            {
                /* selected = guna2ComboBox1.SelectedItem.ToString();*/
                // Get the selected item from the combo box
                string selectedPosition = guna2ComboBox1.SelectedItem.ToString();

                // Set the department_txt based on the selected position
                if (selectedPosition == "Software Engineer" ||
                    selectedPosition == "Junior Software Engineer" ||
                    selectedPosition == "Associate Software Engineer" ||
                    selectedPosition == "Senior Software Engineer" ||
                    selectedPosition == "IT Support Engineer")
                {
                    guna2TextBox4.Text = "IT Department";
                }
                else if (selectedPosition == "Pre-editor - Journal" ||
                         selectedPosition == "Pre-editor - Book" ||
                         selectedPosition == "Copyeditor - Journal" ||
                         selectedPosition == "Copyeditor - Book" ||
                         selectedPosition == "Proof Collator" ||
                         selectedPosition == "Quality Assurance")
                {
                    guna2TextBox4.Text = "Production";
                }
                else if (selectedPosition == "Supervisor" ||
                         selectedPosition == "Manager" ||
                         selectedPosition == "VP" ||
                         selectedPosition == "President")
                {
                    guna2TextBox4.Text = "Management";
                }
                else
                {
                    // Handle other positions or set a default value for the department_txt
                    guna2TextBox4.Text = "Unknown Department";
                }
            }
        }

        private void guna2TextBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label6_Click_2(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ToggleEditMode();
        }

        private void guna2Button3_Click_2(object sender, EventArgs e)
        {

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)

        {
        }


        private void guna2Button12_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click_3(object sender, EventArgs e)
        {
            ToggleEditMode();
        }
        //valida




        private void guna2Button5_Click(object sender, EventArgs e)
        {
            String imageLocation = "";
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "jpg files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files(*.*)|*.*";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    imageLocation = dialog.FileName;

                    image1.ImageLocation = imageLocation;
                }
            }
            catch (Exception) { 
            MessageBox.Show("An error occured", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            {

            }
            }
        }
        /// <summary>
        /// validation
        

        private void guna2ShadowPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2VProgressBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void guna2ContainerControl2_Click(object sender, EventArgs e)
        {

        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            isDarkMode = guna2ToggleSwitch1.Checked;
            ApplyTheme();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleProgressBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }
    }

}

