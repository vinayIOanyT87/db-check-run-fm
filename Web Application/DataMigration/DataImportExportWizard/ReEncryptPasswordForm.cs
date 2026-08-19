// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReEncryptPasswordForm.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReEncryptPasswordForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Crypt;

    using DataImportExportWizard.Constants;
    using DataImportExportWizard.DataAccess;
    using DataImportExportWizard.Interfaces;
    using DataImportExportWizard.InternalClasses;
    using DataImportExportWizard.InternalClasses.LogClient;

    public partial class ReEncryptPasswordForm : Form, IMigrationForm
    {
        private bool autoRunEnabled = false;

        private bool autoCloseEnabled = false;

        private CapicomCrypt encryptorCapi = new CapicomCrypt();
        private AESCrypt encryptorAes = new AESCrypt();

        private static readonly byte[] Seed = (new Guid("1488AE9C-6813-49AE-AF08-155A53D99CE6")).ToByteArray();
        private static readonly byte[] DummyData = (new Guid("4BE74006-F456-4399-86C5-03613D7FB234")).ToByteArray();

        /// <summary>
        /// The logger.
        /// </summary>
        private Logger loggerInstance;

        /// <summary>
        /// The loggerInstance.
        /// </summary>
        public Logger LoggerInstance
        {
            get
            {
                return this.loggerInstance;
            }

            set
            {
                this.loggerInstance = value;
            }
        }

        public bool IsProcessing { get; private set; }

        public ReEncryptPasswordForm()
        {
            this.InitializeComponent();
        }

        #region Form Controls - Event Handlers
        private void StartButton_Click(object sender, EventArgs e)
        {
            this.ProcessReEncryption();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion Form Controls - Event Handlers

        #region Form Event Handlers
        private void ReEncryptPasswordForm_Load(object sender, EventArgs e)
        {
            this.loggerInstance = new Logger(string.Format("{0}_AESEncrypt", StringConstants.ApplicationShortName));
        }
        private void ReEncryptPasswordForm_Shown(object sender, EventArgs e)
        {
            try
            {
                if (this.autoRunEnabled)
                {
                    Application.DoEvents();
                    this.ProcessReEncryption();

                    if (this.autoCloseEnabled)
                    {
                        this.Close();
                    }
                }
            }
            catch (IOException ioException)
            {
                MessageBox.Show(
                    @"An error occurred while attempting to re-encrypt persisted data." + @"The error is:"
                    + ioException.ToString());
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    @"An error occurred while attempting to re-encrypt persisted data." + @"The error is:"
                    + exception.ToString());
            }
            finally
            {
                this.ExitButton.Enabled = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.loggerInstance.Dispose();
            this.loggerInstance = null;
            base.OnFormClosing(e);
        }
        #endregion Form Event Handlers

        #region Public Methods
        /// <summary>
        /// The run.
        /// </summary>
        public void EnableAutoRun(bool autoCloseFlag)
        {
            this.autoCloseEnabled = autoCloseFlag;
            this.autoRunEnabled = true;

            this.StartButton.Visible = false;

            if (this.autoCloseEnabled)
            {
                this.ExitButton.Visible = false;
            }
            else
            {
                this.ExitButton.Location = new Point(337, 386);
            }
        }
        #endregion Public Methods

        #region Logging Methods
        /// <summary>
        /// The report status.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void ReportStatus(string message)
        {
            this.loggerInstance.Info(message);
            this.statusProcessingUserControl.UpdateStatus(message);
        }

        /// <summary>
        /// The report status line.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private void ReportStatusLine(string message)
        {
            this.loggerInstance.Info(message);
            this.statusProcessingUserControl.UpdateStatusLine(message);
        }
        #endregion Logging Methods

        #region Main Encryption Processing Method
        /// <summary>
        /// The process re encryption.
        /// </summary>
        private void ProcessReEncryption()
        {
            if (this.IsProcessing)
            {
                return;
            }

            this.IsProcessing = true;

            DAService da = new DAService();

            try
            {
                this.ProcessSystemSettings(da);

                this.ProcessSyncClientConfiguration(da);

                this.ProcessUsers(da);

				this.ProcessPersonnelPIN(da);

				this.ReportStatusLine(string.Empty);
                this.ReportStatusLine("Encryption Completed...");
                this.ReportStatusLine(string.Empty);
            }
            catch (Exception e)
            {
                this.loggerInstance.Critical("*** Error Detected ***");
                this.loggerInstance.Critical(e.Message);
                this.ReportStatusLine("*** Error Detected ***");
                this.ReportStatusLine(e.Message);

                throw;
            }
            finally
            {
                this.IsProcessing = false;
            }
        }
        #endregion Main Encryption Processing Method

        #region Sub Encryption Processing Methods
        /// <summary>
        /// The process system settings.
        /// </summary>
        /// <param name="da">
        /// The da.
        /// </param>
        private void ProcessSystemSettings(DAService da)
        {
            this.ReportStatusLine("Processing System Settings");

            DataSet ds = da.ExecuteSql(DAService.DatabaseName, @"SELECT * FROM [dbo].[tblSystemSettings]");

            if (ds.Tables.Count == 1)
            {
                bool changesMade = false;

                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    string decrypted = string.Empty;

                    if (this.TryDecryptColumnWithCapicom(dr, "ReportServerPassword", false, out decrypted))
                    {
                        dr["ReportServerPassword"] = this.EncryptColumnWithAes(decrypted, Guids.SiteAdminGuid);
                    }

                    if (dr.RowState != DataRowState.Unchanged)
                    {
                        changesMade = true;
                    }
                }

                if (changesMade)
                {
                    dt.AcceptChanges();
                }
            }
        }

        /// <summary>
        /// The process sync client configuration.
        /// </summary>
        /// <param name="da">
        /// The da.
        /// </param>
        private void ProcessSyncClientConfiguration(DAService da)
        {
            this.ReportStatusLine("Processing Sync Client Configuration");

            DataSet ds = da.ExecuteSql(DAService.DatabaseName, @"SELECT * FROM [dbo].[tblSyncClientConfiguration]");

            if (ds.Tables.Count == 1)
            {
                bool changesMade = false;

                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    string decrypted = string.Empty;

                    if (this.TryDecryptColumnWithCapicom(dr, "ServerAuthPassword", false, out decrypted))
                    {
                        dr["ServerAuthPassword"] = this.EncryptColumnWithAes(decrypted, Guids.SiteAdminGuid);
                    }

                    decrypted = string.Empty;

                    if (this.TryDecryptColumnWithCapicom(dr, "FMAuthPassword", false, out decrypted))
                    {
                        dr["FMAuthPassword"] = this.EncryptColumnWithAes(decrypted, Guids.SiteAdminGuid);
                    }
                    
                    if (dr.RowState != DataRowState.Unchanged)
                    {
                        changesMade = true;
                    }
                }

                if (changesMade)
                {
                    dt.AcceptChanges();
                }
            }
        }

        /// <summary>
        /// The process users.
        /// </summary>
        /// <param name="da">
        /// The da.
        /// </param>
        private void ProcessUsers(DAService da)
        {
            this.ReportStatusLine("Processing users...");

            DataSet sites = da.GetSites();

            if (sites.Tables.Count > 0)
            {
                foreach (DataRow site in sites.Tables[0].Rows)
                {
                    string siteId = this.GetString(site["ID"]);
                    Guid siteGuid = da.GetSiteGuid(siteId);

                    DataSet ds = new DataSet();

                    using (var connection = new SqlConnection())
                    {
                        connection.ConnectionString = DAService.GetConnectionString(DAService.DatabaseName);

                        try
                        {
                            // Open the database connection.
                            connection.Open();

                            if (connection.State == ConnectionState.Open)
                            {
                                bool changesMade = false;

                                // Create and configure a new command.                       
                                SqlCommand cmd = new SqlCommand(
                                    @"SELECT * FROM [dbo].[tblUsers] WHERE SiteGuid = @Guid");
                                cmd.Connection = connection;
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@Guid", siteGuid);

                                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                                adapter.TableMappings.Add("tblUsers", "tblUsers");
                                adapter.UpdateCommand = this.GetUpdateUsersCommand();
                                adapter.UpdateCommand.Connection = connection;

                                adapter.UpdateCommand.Parameters.Add("@Password", SqlDbType.VarBinary, 256).SourceColumn = "Password";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory1", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory1";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory2", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory2";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory3", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory3";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory4", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory4";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory5", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory5";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory6", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory6";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory7", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory7";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory8", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory8";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory9", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory9";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory10", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory10";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory11", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory11";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory12", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory12";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory13", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory13";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory14", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory14";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory15", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory15";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory16", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory16";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory17", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory17";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory18", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory18";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory19", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory19";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory20", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory20";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory21", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory21";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory22", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory22";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory23", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory23";
                                adapter.UpdateCommand.Parameters.Add("@PasswordHistory24", SqlDbType.VarBinary, 256).SourceColumn = "PasswordHistory24";

                                SqlParameter workParam = adapter.UpdateCommand.Parameters.Add(
                                    "@UserGuid", SqlDbType.UniqueIdentifier);

                                workParam.SourceColumn = "UserGuid";
                                workParam.SourceVersion = DataRowVersion.Current;

                                adapter.Fill(ds, "tblUsers");

                                if (ds.Tables.Count > 0)
                                {
                                    this.ReportStatus(string.Format("Processing users for '{0}': ", siteId));

                                    DataTable dt = ds.Tables[0];

                                    int userCount = 0;
                                    int updateCount = 0;

                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        userCount++;

                                        string decrypted = string.Empty;

                                        if (this.TryDecryptColumnWithCapicom(dr, "Password", false, out decrypted))
                                        {
                                            dr["Password"] = this.EncryptColumnWithAes(decrypted, siteGuid);
                                        }

                                        for (int historyCount = 1; historyCount <= 24; historyCount++)
                                        {
                                            string columnName = "PasswordHistory" + historyCount.ToString();
                                            decrypted = string.Empty;

                                            if (this.TryDecryptColumnWithCapicom(dr, columnName, false, out decrypted))
                                            {
                                                dr[columnName] = this.EncryptColumnWithAes(decrypted, siteGuid);
                                            }
                                        }

                                        if (dr.RowState != DataRowState.Unchanged)
                                        {
                                            updateCount++;
                                            changesMade = true;
                                        }
                                    }

                                    this.ReportStatusLine(string.Format("{0} /{1} (detected / updated)", userCount, updateCount));
                                }

                                if (changesMade)
                                {
                                    adapter.Update(ds, "tblUsers");
                                }
                            }
                        }
                        catch (SqlException e)
                        {
                            this.ReportStatusLine("ProcessUsers: " + e.Message);
                            Trace.WriteLine(string.Format("ProcessUsers: {0}", e.Message));
                        }
                        catch (Exception ex)
                        {
                            this.ReportStatusLine("ProcessUsers: " + ex.Message);
                            Trace.WriteLine(string.Format("ProcessUsers: {0}", ex.Message));
                        }

                        connection.Close();

                        // At the end of the using block Dispose() calls Close().
                    }
                }
            }
        }

		/// <summary>
		/// The process PINs for Personnel Records.  These may not be encrypted in previous versions so we need to detect this and encrypt them.
		/// </summary>
		/// <param name="da">
		/// The da.
		/// </param>
		private void ProcessPersonnelPIN(DAService da)
		{
			this.ReportStatusLine("Processing Personnel PINs...");

			DataSet sites = da.GetSites();

			if (sites.Tables.Count > 0)
			{
				foreach (DataRow site in sites.Tables[0].Rows)
				{
					string siteId = this.GetString(site["ID"]);
					Guid siteGuid = da.GetSiteGuid(siteId);

					DataSet ds = new DataSet();

					using (var connection = new SqlConnection())
					{
						connection.ConnectionString = DAService.GetConnectionString(DAService.DatabaseName);

						try
						{
							// Open the database connection.
							connection.Open();

							if (connection.State == ConnectionState.Open)
							{
								bool changesMade = false;

								// Create and configure a new command.                       
								SqlCommand cmd = new SqlCommand(
									@"SELECT * FROM [dbo].[tblPersonnel] WHERE SiteGuid = @Guid");
								cmd.Connection = connection;
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@Guid", siteGuid);

								SqlDataAdapter adapter = new SqlDataAdapter(cmd);
								adapter.TableMappings.Add("tblPersonnel", "tblPersonnel");
								adapter.UpdateCommand = this.GetUpdatePersonnelCommand();
								adapter.UpdateCommand.Connection = connection;

								adapter.UpdateCommand.Parameters.Add("@PINNumber", SqlDbType.VarBinary, 256).SourceColumn = "PINNumber";

								SqlParameter workParam = adapter.UpdateCommand.Parameters.Add(
									"@PersonnelGuid", SqlDbType.UniqueIdentifier);

								workParam.SourceColumn = "PersonnelGuid";
								workParam.SourceVersion = DataRowVersion.Current;

								adapter.Fill(ds, "tblPersonnel");

								if (ds.Tables.Count > 0)
								{
									this.ReportStatus(string.Format("Processing personnel PINs for '{0}': ", siteId));

									DataTable dt = ds.Tables[0];

									int personnelCount = 0;
									int updateCount = 0;

									foreach (DataRow dr in dt.Rows)
									{
										personnelCount++;

										string decrypted = string.Empty;

										if (this.TryDecryptColumnWithCapicom(dr, "PINNumber", true, out decrypted))
										{
											dr["PINNumber"] = this.EncryptColumnWithAes(decrypted, siteGuid);
										}
										else
										{
											if (dr["PINNumber"] != DBNull.Value)
											{
												byte[] plainTextBytes = ((byte[])dr["PINNumber"]);
												string plainTextPIN = Encoding.Unicode.GetString(plainTextBytes);

												dr["PINNumber"] = this.EncryptColumnWithAes(plainTextPIN, siteGuid);
											}

										}

										if (dr.RowState != DataRowState.Unchanged)
										{
											updateCount++;
											changesMade = true;
										}
									}

									this.ReportStatusLine(string.Format("{0} /{1} (detected / updated)", personnelCount, updateCount));
								}

								if (changesMade)
								{
									adapter.Update(ds, "tblPersonnel");
								}
							}
						}
						catch (SqlException e)
						{
							this.ReportStatusLine("ProcessPersonnelPIN: " + e.Message);
							Trace.WriteLine(string.Format("ProcessPersonnelPIN: {0}", e.Message));
						}
						catch (Exception ex)
						{
							this.ReportStatusLine("ProcessPersonnelPIN: " + ex.Message);
							Trace.WriteLine(string.Format("ProcessPersonnelPIN: {0}", ex.Message));
						}

						connection.Close();

						// At the end of the using block Dispose() calls Close().
					}
				}
			}
		}
		#endregion Sub Encryption Processing Methods

		#region Utility Methods for Encrypting and Decrypting
		/// <summary>
		/// The try decrypt column with capicom.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <param name="columnName">
		/// The column name.
		/// </param>
		/// <param name="decrypted">
		/// The decrypted.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool TryDecryptColumnWithCapicom(DataRow row, string columnName, bool detectOnlyFlag, out string decrypted)
        {
            bool isCapicom = false;
            decrypted = string.Empty;

            try
            {
                if (row[columnName] != DBNull.Value)
                {
                    decrypted = this.DecryptColumnWithCapicom(row[columnName]);
                    isCapicom = true;
                }
            }
            catch (Exception e)
            {
				if (!detectOnlyFlag)
				{
					this.loggerInstance.Warn(string.Format(@"Failed to decrypt data with Capicom API: {0}", e.Message));
				}
                isCapicom = false;
            }

            return isCapicom;
        }

        /// <summary>
        /// The encrypt column with AES.
        /// </summary>
        /// <param name="plainText">
        /// The plain text data that needs to be encrypted.
        /// </param>
        /// <param name="siteGuid">
        /// The GUID value is used as the key when encrypting with AESCrypt.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private byte[] EncryptColumnWithAes(string plainText, Guid siteGuid)
        {
            try
            {
                if (!string.IsNullOrEmpty(plainText))
                {
                    return this.EncodeAes(plainText, siteGuid);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        /// <summary>
        /// The decrypt column with capicom.
        /// </summary>
        /// <param name="columnData">
        /// The column data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DecryptColumnWithCapicom(object columnData)
        {
            if (!this.IsNull(columnData))
            {
                return this.DecodeCapi((byte[])columnData);
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the CAPI key.
        /// </summary>
        /// <returns>
        /// The <see cref="PasswordKey"/>.
        /// </returns>
        private PasswordKey GetCapiKey()
        {
            string passwordKeySeed = "Mellon";
            return new PasswordKey(Encoding.UTF8.GetBytes(passwordKeySeed));
        }

        /// <summary>
        /// The decode CAPI.
        /// </summary>
        /// <param name="encodedData">
        /// The encoded data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DecodeCapi(byte[] encodedData)
        {
            PasswordKey key = this.GetCapiKey();
            byte[] plainTextBytes = this.encryptorCapi.Decrypt(encodedData, key);
            key.Dispose();

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        /// <summary>
        /// The encode CAPI.
        /// </summary>
        /// <param name="plaintextData">
        /// The plaintext data.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        /// </returns>
        public byte[] EncodeCapi(string plaintextData)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plaintextData);

            PasswordKey key = this.GetCapiKey();
            byte[] encryptedBytes = this.encryptorCapi.Encrypt(plainTextBytes, key);
            key.Dispose();

            return encryptedBytes;
        }

        /// <summary>
        /// The get AES encryption key.
        /// </summary>
        /// <param name="siteGuid">
        /// The site GUID.
        /// </param>
        /// <returns>
        /// The <see cref="AESKey"/>.
        /// </returns>
        private AESKey GetAesKey(Guid siteGuid)
        {
            byte[] newSeed = new byte[ReEncryptPasswordForm.Seed.Length + ReEncryptPasswordForm.DummyData.Length];
            Buffer.BlockCopy(ReEncryptPasswordForm.Seed, 0, newSeed, 0, ReEncryptPasswordForm.Seed.Length);
            Buffer.BlockCopy(ReEncryptPasswordForm.DummyData, 0, newSeed, ReEncryptPasswordForm.Seed.Length, ReEncryptPasswordForm.DummyData.Length);
            return new AESKey(newSeed, siteGuid.ToByteArray());
        }

        /// <summary>
        /// The decode.
        /// </summary>
        /// <param name="encodedData">
        /// The encoded data.
        /// </param>
        /// <param name="siteGuid">
        /// The site GUID.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string DecodeAes(byte[] encodedData, Guid siteGuid)
        {
            using (AESKey key = this.GetAesKey(siteGuid))
            {
                return this.encryptorAes.DecryptToText(encodedData, key);
            }
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="plaintextData">
        /// The plaintext data.
        /// </param>
        /// <param name="siteGuid">
        /// The site GUID.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        /// </returns>
        public byte[] EncodeAes(string plaintextData, Guid siteGuid)
        {
            using (AESKey key = this.GetAesKey(siteGuid))
            {
                return this.encryptorAes.Encrypt(plaintextData, key);
            }
        }
        #endregion Utility Methods for Encrypting and Decrypting

        #region Utility Methods for Reading / Writing Database Values
        /// <summary>
        /// This method will determine if the row has a null value. If so, then the method will return an empty string.
        /// </summary>
        /// <param name="columnData">
        /// The column data to check.
        /// </param>
        /// <returns>
        /// The object as a string or String.Empty if the row is null.
        /// </returns>
        public string GetString(object columnData)
        {
            if (this.IsNull(columnData))
            {
                return string.Empty;
            }

            return (string)columnData;
        }

        /// <summary>
        /// Tests if the object is null.
        /// </summary>
        /// <param name="columnData">
        /// The object to test.
        /// </param>
        /// <returns>
        /// Boolean indicating if object is null.
        /// </returns>
        public bool IsNull(object columnData)
        {
            return columnData == DBNull.Value;
        }

        /// <summary>
        /// The get update users command.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        private SqlCommand GetUpdateUsersCommand()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE [dbo].[tblUsers] SET ");
            sql.Append("Password = @Password, ");
            sql.Append("PasswordHistory1 = @PasswordHistory1, ");
            sql.Append("PasswordHistory2 = @PasswordHistory2, ");
            sql.Append("PasswordHistory3 = @PasswordHistory3, ");
            sql.Append("PasswordHistory4 = @PasswordHistory4, ");
            sql.Append("PasswordHistory5 = @PasswordHistory5, ");
            sql.Append("PasswordHistory6 = @PasswordHistory6, ");
            sql.Append("PasswordHistory7 = @PasswordHistory7, ");
            sql.Append("PasswordHistory8 = @PasswordHistory8, ");
            sql.Append("PasswordHistory9 = @PasswordHistory9, ");
            sql.Append("PasswordHistory10 = @PasswordHistory10, ");
            sql.Append("PasswordHistory11 = @PasswordHistory11, ");
            sql.Append("PasswordHistory12 = @PasswordHistory12, ");
            sql.Append("PasswordHistory13 = @PasswordHistory13, ");
            sql.Append("PasswordHistory14 = @PasswordHistory14, ");
            sql.Append("PasswordHistory15 = @PasswordHistory15, ");
            sql.Append("PasswordHistory16 = @PasswordHistory16, ");
            sql.Append("PasswordHistory17 = @PasswordHistory17, ");
            sql.Append("PasswordHistory18 = @PasswordHistory18, ");
            sql.Append("PasswordHistory19 = @PasswordHistory19, ");
            sql.Append("PasswordHistory20 = @PasswordHistory20, ");
            sql.Append("PasswordHistory21 = @PasswordHistory21, ");
            sql.Append("PasswordHistory22 = @PasswordHistory22, ");
            sql.Append("PasswordHistory23 = @PasswordHistory23, ");
            sql.Append("PasswordHistory24 = @PasswordHistory24 ");
            sql.Append("WHERE UserGuid = @UserGuid ");

            return new SqlCommand(sql.ToString());
        }

		/// <summary>
		/// The get update Personnel command.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlCommand"/>.
		/// </returns>
		private SqlCommand GetUpdatePersonnelCommand()
		{
			StringBuilder sql = new StringBuilder();

			sql.Append("UPDATE [dbo].[tblPersonnel] SET ");
			sql.Append("PINNumber = @PINNumber ");
			sql.Append("WHERE PersonnelGuid = @PersonnelGuid ");

			return new SqlCommand(sql.ToString());
		}
		#endregion Utility Methods for Reading / Writing Database Values
	}
}
