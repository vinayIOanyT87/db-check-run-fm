using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FMBusinessObjects.DataObjects;

namespace PasswordResetToolGUI
{
    public partial class passwordReset : Form
    {
        public passwordReset()
        {
            InitializeComponent();
            this.AcceptButton = button1;
            this.FormClosing += PasswordReset_FormClosing1;
        }

        private void PasswordReset_FormClosing1(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            byte[] encryptedPassword = null;
            Guid siteGuid = new Guid();
            string userID = textBox1.Text;


            if (textBox3.Text != textBox2.Text)
            {
               label4.Text = "Passwords do not match";
                return;
            }

            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
            {
                label4.Text = "Fields may not be left blank";
                return;
            }

            string password = textBox3.Text;

            // Running SQL Query
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //Encrypt new password
                    string retrieveSiteGuid = "SELECT SiteGuid FROM dbo.tblUsers WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(@retrieveSiteGuid, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                siteGuid = Guid.Parse(reader["SiteGuid"].ToString());
                                encryptedPassword = UserClass.encode(password, siteGuid);
                            }
                            else
                            {
                                label4.Text = "User not found";
                                return;
                            }
                        }
                    }

                    //Set Password to encoded password and set the ForcePasswordReset flag to true
                    string sqlScript = @"
                        UPDATE dbo.tblUsers
                        SET Password = CAST(@encryptedPassword AS VARBINARY), ChangePassword = 1
                        WHERE UserID = @UserID;
                    ";

                    using (SqlCommand command = new SqlCommand(sqlScript, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@encryptedPassword", encryptedPassword);

                        int affectedRows = command.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            label4.Text = "Password was successfully updated";
                            textBox1.Text = "";
                            textBox2.Text = "";
                            textBox3.Text = "";
                        }
                        else
                        {
                            label4.Text = "User not found";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.Source == ".Net SqlClient Data Provider")
                    label4.Text = ex.Message;
                else
                    throw ex;
            }


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void passwordReset_Load(object sender, EventArgs e)
        {

        }

    }
}
