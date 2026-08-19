using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using ENCRYPTUTILITIESLib;

namespace FMBackupUtilityConfiguration
{
    public partial class LoginDialogForm : Form
    {
        private static string sUserID; // SQL Server user ID.       
        private bool bLogin = false;
        int iNumofTry = 0;
        int iThreshold = -1;
        public String strWhere;
        String strUpdatedDate;

        public LoginDialogForm()
        {
            InitializeComponent();
        }
        
        public bool IsLoggedIn
        {
            get { return bLogin; }
        }
/*
        public string UserName
        {
            get { return tbUserName.Text; }
        }
        
        public string Password
        {
            get { return tbPassword.Text; }
        }
*/
        public static string getConnectionString()
        {
            sUserID = "FMDService";
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder("Data Source = localhost; Initial Catalog = ConsolidatedDB;");
            connectionString.Add("Integrated Security", "false");
            connectionString.Add("Network Library", "dbmssocn");
            connectionString.Add("pwd", getDBPassword());
            connectionString.Add("User ID", sUserID);
//            connectionString.AsynchronousProcessing = true;
            return connectionString.ToString();
        }

        public static string getDBPassword()
        {
            // Algorithm is to take a SHA-1 hash of the bytes of the ASCII representation
            // of the user ID followed by the bytes of the salt "{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}"
            ASCIIEncoding encoding = new ASCIIEncoding();
            SHA1 sha = new SHA1CryptoServiceProvider();

            // Split out for obfuscation purposes
            // Probably something more thorough required later

            //Updated to ensure that UserID is always uppercase.
            StringBuilder newData = new StringBuilder(sUserID.ToUpper());
            newData.Append('{');
            newData.Append('0');
            newData.Append('1');
            newData.Append('A');
            newData.Append('F');
            newData.Append('E');
            newData.Append('B');
            newData.Append('D');
            newData.Append('3');
            newData.Append('-');
            newData.Append('7');
            newData.Append('8');
            newData.Append('C');
            newData.Append('D');
            newData.Append('-');
            newData.Append('4');
            newData.Append('B');
            newData.Append('1');
            newData.Append('5');
            newData.Append('-');
            newData.Append('A');
            newData.Append('B');
            newData.Append('9');
            newData.Append('B');
            newData.Append('-');
            newData.Append('F');
            newData.Append('4');
            newData.Append('A');
            newData.Append('A');
            newData.Append('1');
            newData.Append('C');
            newData.Append('0');
            newData.Append('E');
            newData.Append('2');
            newData.Append('D');
            newData.Append('9');
            newData.Append('B');
            newData.Append('}');
            byte[] userIDBytes = encoding.GetBytes(newData.ToString());
            //byte[]	saltBytes = encoding.GetBytes("{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}");

            byte[] pwdBytes = sha.ComputeHash(userIDBytes);

            newData.Length = 0;
            foreach (byte pwdByte in pwdBytes)
            {
                newData.Append(pwdByte.ToString("x2")); // x indicates hexidecimal integer, 2 (the precision) is
                // the minimum number of digits.  Output will be zero
                // padded on the left as necessary
            }
            return newData.ToString();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            iThreshold = -1;
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        SqlCommand Archive = connection.CreateCommand();
                        iNumofTry++;
                        strWhere = " WHERE UserID = '";
                        strWhere += this.tbUserName.Text.Trim();
                        strWhere += "' AND SiteIndex = 1 ";

                        strUpdatedDate = ", UpdatedDate = GETDATE(), UpdatedBy = 'FMDAdmin' ";

                        String strSQL;
                        strSQL = "SELECT AccountLockoutThreshold FROM tblSites WHERE SiteIndex = 1";
                        Archive.CommandText = strSQL;
                        iThreshold = (int)Archive.ExecuteScalar();

                        strSQL = "SELECT Disabled FROM tblUsers";
                        strSQL += strWhere;
                        Archive.CommandText = strSQL;
								try
								{
									bool bLockout = (bool)Archive.ExecuteScalar();

									if (bLockout)
									{
										connection.Close();
										MessageBox.Show("This account has been locked out.\n Please check with your adminstrator.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
										UpdateAccountStatus();
										DummyLogin(false);
										this.Close();
										return;
									}
								}
								catch (NullReferenceException)
								{
									// Null Reference Exception is expected here.
									// If we ever get the chance to revamp it, logic must
									// be changed to not expect exceptions for non-exceptional events.
									bLogin = false;

									// Keep going, let it fall through and get caught by the next block
								}
                    
                        StringEncrypt2Class StringEncrypt2Class = new StringEncrypt2Class();
                        Array EnteredPassword = StringEncrypt2Class.Encrypt(this.tbPassword.Text.Trim());

                        strSQL = "SELECT tblUsers.Password ";
                        strSQL += "FROM tblUsers JOIN tblSites ON tblUsers.SiteIndex = tblSites.SiteIndex ";
                        strSQL += " WHERE tblUsers.UserID = '";
                        strSQL += this.tbUserName.Text.Trim();
                        strSQL += "' AND tblUsers.SiteIndex = 1 ";
                        strSQL += "AND tblUsers.Disabled = 0 AND tblUsers.UserIndex IN ( SELECT tblUsers.UserIndex ";
                        strSQL += "FROM tblUsers JOIN tblUserGroupMap ON tblUserGroupMap.UserIndex = tblUsers.UserIndex ";
                        strSQL += "WHERE tblUserGroupMap.DeleteFlag = 0 AND tblUserGroupMap.GroupIndex IN (";
                        strSQL += "SELECT tblUserGroupMap.GroupIndex ";
                        strSQL += "FROM tblGroupRightsMap JOIN tblUserGroupMap ON tblUserGroupMap.GroupIndex = tblGroupRightsMap.GroupIndex ";
                        strSQL += "where tblGroupRightsMap.RightIndex IN ( ";
                        strSQL += "SELECT tblGroupRightsMap.RightIndex ";
                        strSQL += "FROM tblGroupRightsMap JOIN tblGroupRights ON tblGroupRights.RightIndex = tblGroupRightsMap.RightIndex ";
                        strSQL += "WHERE tblGroupRights.RightID = 'Modify System Configuration')))";

                        Archive.CommandText = strSQL;

								try
								{
									byte[] Password = (byte[])Archive.ExecuteScalar();
									if (Password != null && (Password.Length == EnteredPassword.GetLength(0)))
									{
										for (int i = 0; i < Password.Length; i++)
										{
											if (Password[i] != (byte)EnteredPassword.GetValue(i))
											{
												bLogin = false;
												break;
											}
											bLogin = true;
										}
									}
								}
								catch (NullReferenceException)
								{
									// Null Reference Exception is expected here.
									// If we ever get the chance to revamp it, logic must
									// be changed to not expect exceptions for non-exceptional events.
									bLogin = false;
								}

                        if (bLogin)
                        {
                            strSQL = "SELECT tblUsers.UserID FROM tblUsers JOIN tblSites ON tblUsers.SiteIndex = tblSites.SiteIndex";
                            strSQL += " WHERE tblUsers.ForcePWDChangeFlag <> 1 AND tblUsers.UserID = '";
                            strSQL += this.tbUserName.Text.Trim();
                            strSQL += "' AND tblUsers.SiteIndex = 1 AND ";
                            strSQL += " DATEADD(day, tblSites.MaxPasswordAge, tblUsers.LastPasswordChange) > GETDATE()";

                            Archive.CommandText = strSQL;
                            Object obj = Archive.ExecuteScalar();
                            if (obj == null)
                            {
                                String strMsg;
                                strMsg = "Your password must be changed before it may be used. You must change your password in the FuelsManager Defense application. ";                                
                                MessageBox.Show(strMsg, "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                bLogin = false;
                            }
                            else
                            {
                                strSQL = "UPDATE tblUsers SET FailedLogins = 0 ";
                                strSQL += strUpdatedDate;
                                strSQL += ", LastLoginDate = GETDATE() FROM tblUsers";
                                strSQL += strWhere;
                                Archive.CommandText = strSQL;
                                Archive.ExecuteScalar();
                            }
                            connection.Close();
                            DummyLogin(true);
                            LogoutUpdate();
                            this.Close();
                            return;
                        }
                        else
                        {
                            connection.Close();
                            UpdateAccountStatus();
                            DummyLogin(false);
                            if (iNumofTry >= iThreshold)
                            {
                                MessageBox.Show("Login Fails", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                                return;
                            }
                        }
                    }
                    else
                        System.Diagnostics.Trace.WriteLine("Could not open Database.");
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("Connection Dababase Failed. {0}", ex.Message));
                }

                MessageBox.Show("Login Fails", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);               
                this.tbUserName.Clear();
                this.tbPassword.Clear();
                this.tbUserName.Focus();
            }
        }

        private void UpdateAccountStatus()
        {
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        SqlCommand Archive = connection.CreateCommand();
                        String strSQL;
                        strSQL = "SELECT FailedLogins FROM tblUsers";
                        strSQL += strWhere;
                        Archive.CommandText = strSQL;
                        int iFailedLogins = (int)Archive.ExecuteScalar();

                        iFailedLogins++;
                        String s = Convert.ToString(iFailedLogins);
                        strSQL = "UPDATE tblUsers SET FailedLogins = ";
                        strSQL += s;
                        if (iFailedLogins >= iThreshold)
                            strSQL += ", Disabled = 1, DisabledDate = GETDATE()";
                        strSQL += strUpdatedDate;
                        strSQL += "FROM tblUsers";
                        strSQL += strWhere;
                        Archive.CommandText = strSQL;
                        Archive.ExecuteScalar();
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("Could not open Database.");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("Connection Dababase Failed. {0}", ex.Message));
                }
                connection.Close();
            }
            return;
        }

        private void DummyLogin(bool blogin)
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder("Data Source = localhost; Initial Catalog = ConsolidatedDB;");
            connectionString.Add("Integrated Security", "false");
            connectionString.Add("Network Library", "dbmssocn");
            sUserID = this.tbUserName.Text.Trim();
            if (blogin)
            {
                connectionString.Add("pwd", getDBPassword());
            }
            else
                connectionString.Add("pwd", "");
            connectionString.Add("User ID", sUserID);
            //connectionString.AsynchronousProcessing = true;
            using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
            {
                try
                {
                    connection.Open();
                    connection.Close();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("Connection Dababase Failed. {0}", ex.Message));
                }
            }
        }

        private void LogoutUpdate()
        {
            using (SqlConnection connection = new SqlConnection(getConnectionString()))
            {
                try
                {
                    connection.Open();
                    if (connection.State == ConnectionState.Open)
                    {
                        SqlCommand Archive = connection.CreateCommand();
                        String strSQL = "UPDATE tblUsers SET LastLogoffDate = GETDATE(), UpdatedDate = GETDATE(), UpdatedBy = 'FMDAdmin' ";
                        strSQL += "WHERE UserID = '";
                        strSQL += this.tbUserName.Text.Trim();
                        strSQL += "' AND SiteIndex = 1 ";
                        Archive.CommandText = strSQL;
                        Archive.ExecuteScalar();
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("Could not open Database.");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("Connection Dababase Failed. {0}", ex.Message));
                }
                connection.Close();
            }  
        }
    }
}