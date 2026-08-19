using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Configuration;
using FMBusinessObjects.DataObjects;

namespace PasswordResetTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Retrieve connection string and userID
            string configPath = @"C:\Program Files (x86)\FuelsManager\FMBusinessServices\Web.config";
            ExeConfigurationFileMap configurationFileMap = new ExeConfigurationFileMap();
            configurationFileMap.ExeConfigFilename = configPath;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configurationFileMap, ConfigurationUserLevel.None);

            string connectionString = config.AppSettings.Settings["ConnectionString"].Value;

            string userID = args.Length > 0 ? args[0] :string.Empty;

            // check for passCode
            string adminPassCode = "5834Varec";
            bool checkPassCode = true;
            while(checkPassCode)
            {
                Console.WriteLine("Please enter the admin passcode: ");
                string inputPassCode = Console.ReadLine();
                if(inputPassCode.Equals(adminPassCode))
                {
                    checkPassCode = false;
                }
                else
                {
                    Console.WriteLine("Incorrect admin passcode, please try again,");
                }
            }




            // if the User ID is not provided, we can just allow the user to provide it manually
            if(string.IsNullOrEmpty(userID) )
            {
                Console.WriteLine("Please enter your User ID: ");
                userID = Console.ReadLine();
            }

            //prompt user for new password to enter and check for password comparions

            string newPassword = " ";
            bool passwordCheck = true;
            while(passwordCheck)
            {
                Console.WriteLine("Please enter your new password: ");
                newPassword = Console.ReadLine();
                Console.WriteLine("Please re-enter your new password: ");
                string inputPassword = Console.ReadLine();
                if( inputPassword.Equals(newPassword))
                {
                    passwordCheck = false;
                }
                else
                {
                    Console.WriteLine("The passwords do not match please try again,");
                }
            }

            byte[] encryptedPassword = null;
            Guid siteGuid = new Guid();

            // Running SQL Query
            try
            {
               using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    //Encrypt new password
                    string retrieveSiteGuid = "SELECT SiteGuid FROM dbo.tblUsers WHERE UserID = @UserID";
                    using (SqlCommand command = new SqlCommand(@retrieveSiteGuid, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        using(SqlDataReader reader =await command.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                siteGuid = Guid.Parse(reader["SiteGuid"].ToString());
                                encryptedPassword = UserClass.encode(newPassword, siteGuid);
                            }
                            else
                            {
                                Console.WriteLine("User not found");
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

                    using(SqlCommand command = new SqlCommand(sqlScript, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@encryptedPassword", encryptedPassword);

                        int affectedRows = await command.ExecuteNonQueryAsync();

                        if(affectedRows > 0 )
                        {
                            Console.WriteLine($"Password was successfully updated");
                        }
                        else
                        {
                            Console.WriteLine("No user was found");
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nDone. Press enter.");
            Console.ReadLine();
        }
    }
}
