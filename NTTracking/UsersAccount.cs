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


namespace NTTracking
{
    public partial class UsersAccount : Form
    {
        

        public UsersAccount()
        {
            InitializeComponent();
        }

        private void UsersAccount_Load(object sender, EventArgs e)
        {
          
        }


        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void guna2ShadowPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {

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
            this.Hide();
            UsersAccount dash = new UsersAccount();
            dash.ShowDialog();
            this.Close();
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png; *.jpg; *.jpeg; *.gif; *.bmp)|*.png; *.jpg; *.jpeg; *.gif; *.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog.FileName;
                byte[] imageBytes = File.ReadAllBytes(imagePath);

                SaveImageToDatabase(imageBytes);
            }
        }

        private void SaveImageToDatabase(byte[] imageBytes)
        {
            using (MySqlConnection connection = new MySqlConnection("Data Source=localhost;Initial Catalog=ntdbtracking;username=root;password=;"))
            {
                try
                {
                    connection.Open();

                    string query = "UPDATE accounts SET ProfilePicture = @imageData WHERE UserId = @userId";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        // Assuming you have a textbox for UserId, replace "YourUserIdTextBox" with the actual textbox name.
                        /*cmd.Parameters.Add("@userId", MySqlDbType.Int32).Value = int.Parse();
                        cmd.Parameters.Add("@imageData", MySqlDbType.Blob).Value = imageBytes;*/

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Image uploaded and saved to database successfully!");
                        }
                        else
                        {
                            MessageBox.Show("Failed to save image to database.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
    }
 }
        
    

