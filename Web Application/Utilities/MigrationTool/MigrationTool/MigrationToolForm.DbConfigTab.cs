namespace MigrationTool
{
    using MigrationToolBusinessObjects;
    using MigrationToolBusinessObjects.Handlers;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Configuration;
    using System.Windows.Forms;

    public partial class MigrationToolForm : Form
    {
        #region Data members
        private const string SourceDbConnectionSecurityKey  = "SourceDbConnectionSecurity";
        private const string SourceDbConnectionTimeoutKey   = "SourceDbConnectionTimeout";
        private const string SourceDbConnectionDbServerKey  = "SourceDbConnectionDbServer";
        private const string SourceDbConnectionDbNameKey    = "SourceDbConnectionDbName";
        private const string TargetDbConnectionSecurityKey  = "TargetDbConnectionSecurity";
        private const string TargetDbConnectionTimeoutKey   = "TargetDbConnectionTimeout";
        private const string TargetDbConnectionDbServerKey  = "TargetDbConnectionDbServer";
        private const string TargetDbConnectionDbNameKey    = "TargetDbConnectionDbName";

        private DbConfigurationDO dbConfigurationDo;
        private SaveFileDialog saveDbConfigFileDialog;
        private OpenFileDialog openDbConfigFileDialog;
        #endregion

        private void InitializeDbConnectionTab()
        {
            this.readDbConfiguration = true;
            this.dbConfigurationDo = new DbConfigurationDO();
            this.AppConfigForDbConnectionInfo();
        }

        /// <summary>
        /// This method will read the keys from the app config file.
        /// </summary>
        private void AppConfigForDbConnectionInfo()
        {
            if(this.readDbConfiguration == false)
            {
                return;
            }

            this.readDbConfiguration = false;

            this.dbConfigurationDo.SourceDbConnectionSecurity = ConfigurationManager.AppSettings[SourceDbConnectionSecurityKey];
            this.dbConfigurationDo.SourceDbConnectionTimeout  = ConfigurationManager.AppSettings[SourceDbConnectionTimeoutKey];
            this.dbConfigurationDo.SourceDbConnectionDbServer = ConfigurationManager.AppSettings[SourceDbConnectionDbServerKey];
            this.dbConfigurationDo.SourceDbConnectionDbName   = ConfigurationManager.AppSettings[SourceDbConnectionDbNameKey];

            this.dbConfigurationDo.TargetDbConnectionSecurity = ConfigurationManager.AppSettings[TargetDbConnectionSecurityKey];
            this.dbConfigurationDo.TargetDbConnectionTimeout  = ConfigurationManager.AppSettings[TargetDbConnectionTimeoutKey];
            this.dbConfigurationDo.TargetDbConnectionDbServer = ConfigurationManager.AppSettings[TargetDbConnectionDbServerKey];
            this.dbConfigurationDo.TargetDbConnectionDbName   = ConfigurationManager.AppSettings[TargetDbConnectionDbNameKey];

            // Source
            if(string.IsNullOrEmpty(this.dbConfigurationDo.SourceDbConnectionSecurity) == false)
            {
                this.SourceSecurityTb.Text = this.dbConfigurationDo.SourceDbConnectionSecurity;
            }

            if (string.IsNullOrEmpty(this.dbConfigurationDo.SourceDbConnectionTimeout) == false)
            {
                int timeoutValue;

                if (int.TryParse(this.dbConfigurationDo.SourceDbConnectionTimeout, out timeoutValue) == false)
                {
                    this.SourceConnectTimeoutSpinner.Value = 60;
                }
                else
                {
                    this.SourceConnectTimeoutSpinner.Value = timeoutValue;
                }
            }
            else
            {
                this.SourceConnectTimeoutSpinner.Value = 60;
                this.dbConfigurationDo.SourceDbConnectionTimeout = "60";
            }

            if (string.IsNullOrEmpty(this.dbConfigurationDo.SourceDbConnectionDbServer) == false)
            {
                this.SourceDbServerTb.Text = this.dbConfigurationDo.SourceDbConnectionDbServer;
            }

            if (string.IsNullOrEmpty(this.dbConfigurationDo.SourceDbConnectionDbName) == false)
            {
                this.SourceDbNameTb.Text = this.dbConfigurationDo.SourceDbConnectionDbName;
            }

            // Target
            if (string.IsNullOrEmpty(this.dbConfigurationDo.TargetDbConnectionSecurity) == false)
            {
                this.TargetSecurityTb.Text = this.dbConfigurationDo.TargetDbConnectionSecurity;
            }

            if (string.IsNullOrEmpty(this.dbConfigurationDo.TargetDbConnectionTimeout) == false)
            {
                int timeoutValue;

                if (int.TryParse(this.dbConfigurationDo.TargetDbConnectionTimeout, out timeoutValue) == false)
                {
                    this.TargetConnectTimeoutSpinner.Value = 60;
                }
                else
                {
                    this.TargetConnectTimeoutSpinner.Value = timeoutValue;
                }
            }
            else
            {
                this.TargetConnectTimeoutSpinner.Value = 60;
                this.dbConfigurationDo.TargetDbConnectionTimeout = "60";
            }

            if (string.IsNullOrEmpty(this.dbConfigurationDo.TargetDbConnectionDbServer) == false)
            {
                this.TargetDbServerTb.Text = this.dbConfigurationDo.TargetDbConnectionDbServer;
            }

            if (string.IsNullOrEmpty(this.dbConfigurationDo.TargetDbConnectionDbName) == false)
            {
                this.TargetDbNameTb.Text = this.dbConfigurationDo.TargetDbConnectionDbName;
            }
        }

        /// <summary>
        /// This method handles the enabling of the save button.
        /// </summary>
        private void EnableDbConfigBtn()
        {
            this.DbConfigSaveBtn.Enabled = true;
        }

        /// <summary>
        /// This method handles the disabling of the save button.
        /// </summary>
        private void DisableDbConfigBtn()
        {
            this.DbConfigSaveBtn.Enabled = false;
        }

        /// <summary>
        /// This method set the source test connection button to
        /// enable only if all the source DB connection fields are
        /// populated.
        /// </summary>
        private void SetSourceTextConnectionBtn()
        {
            this.SourceTestConnectionBtn.Enabled = false;

            if (string.IsNullOrEmpty(this.SourceDbNameTb.Text) == false
                && string.IsNullOrEmpty(this.SourceDbServerTb.Text) == false
                && string.IsNullOrEmpty(this.SourceSecurityTb.Text) == false
                && this.SourceConnectTimeoutSpinner.Value > 0)
            {
                this.SourceTestConnectionBtn.Enabled = true;
            }
        }

        /// <summary>
        /// This method set the target test connection button to
        /// enable only if all the source DB connection fields are
        /// populated.
        /// </summary>
        private void SetTargetTextConnectionBtn()
        {
            this.TargetTestConnectionBtn.Enabled = false;

            if (string.IsNullOrEmpty(this.TargetDbNameTb.Text) == false
                && string.IsNullOrEmpty(this.TargetDbServerTb.Text) == false
                && string.IsNullOrEmpty(this.TargetSecurityTb.Text) == false)
            {
                this.TargetTestConnectionBtn.Enabled = true;
            }
        }
        #region File Dialog Events
        /// <summary>
        /// This method will open a save dialog for the database configuration tab.
        /// </summary>
        private void OpenDbConfigSaveDialog()
        {
            if (this.saveDbConfigFileDialog == null)
            {
                this.saveDbConfigFileDialog = new SaveFileDialog();
                this.saveDbConfigFileDialog.FileOk += this.SaveDbConfigFileDialogOkEvent;
            }

            this.saveDbConfigFileDialog.FileName = FileHandler.DbConfigurationFileName;
            this.saveDbConfigFileDialog.ShowDialog();
        }

        /// <summary>
        /// This method will read the save DB connection configuration file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void LoadConfigurationOnClickBtn(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            if (this.openDbConfigFileDialog == null)
            {
                this.openDbConfigFileDialog = new OpenFileDialog();
            }

            // Set filter options and filter index.
            this.openDbConfigFileDialog.Filter = "XML Files (.xml)|*.xml";
            this.openDbConfigFileDialog.FilterIndex = 1;

            this.openDbConfigFileDialog.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            this.openDbConfigFileDialog.ShowDialog();

            try
            {
                var fileHandler = new FileHandler();
                this.dbConfigurationDo = fileHandler.ReadDbConnectionConfigurationDataFromFile(this.openDbConfigFileDialog);

                if (this.dbConfigurationDo != null)
                {
                    this.SourceDbServerTb.Text              = this.dbConfigurationDo.SourceDbConnectionDbServer;
                    this.SourceDbNameTb.Text                = this.dbConfigurationDo.SourceDbConnectionDbName;
                    this.SourceSecurityTb.Text              = this.dbConfigurationDo.SourceDbConnectionSecurity;
                    this.SourceConnectTimeoutSpinner.Value  = this.dbConfigurationDo.SourceDbConnectTimeoutInt;

                    this.TargetDbServerTb.Text              = this.dbConfigurationDo.TargetDbConnectionDbServer;
                    this.TargetDbNameTb.Text                = this.dbConfigurationDo.TargetDbConnectionDbName;
                    this.TargetSecurityTb.Text              = this.dbConfigurationDo.TargetDbConnectionSecurity;
                    this.TargetConnectTimeoutSpinner.Value  = this.dbConfigurationDo.TargetDbConnectTimeoutInt;
                }
            }
            catch (Exception ex)
            {
                string errMessage = "Error reading Migration Tool DB Configuration file. " + ex.Message;

                MessageBox.Show(
                                errMessage,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// This method will handle the OK event on the save dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveDbConfigFileDialogOkEvent(object sender, EventArgs e)
        {
            FileHandler fileHandler = new FileHandler();
            fileHandler.SaveDbConfigToFile(this.saveDbConfigFileDialog, this.dbConfigurationDo);
        }
        #endregion

        #region Db Connection Configuration events
        /// <summary>
        /// This method handles the save database configuration tab event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveDbConfigOnClickBtn(object sender, EventArgs e)
        {
            this.OpenDbConfigSaveDialog();
        }

        /// <summary>
        /// This method handles the Exit on click button event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DbConfigExitOnClickBtn(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Exit Migration Tool", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// This method will test the source database connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceTestConnectionOnClickBtn(object sender, EventArgs e)
        {
            // Example: "Persist Security Info=False;Integrated Security=SSPI;database=FuelsManagerQaEnterpriseDB;server=GMPC4G3\SQLSERVER2019;Connect Timeout=60"
            var connectionDA = new MigrationDatabaseDAClass
            {
                ConnectionString = this.SourceSecurityTb.Text
                                            + ";database=" + this.SourceDbNameTb.Text
                                            + ";server=" + this.SourceDbServerTb.Text
                                            + ";Connect Timeout=" + this.SourceConnectTimeoutSpinner.Value
            };

            try
            {
                connectionDA.TestConnection();
                this.SourceTestConnectionTb.Text = "Successful";
            }
            catch(Exception ex)
            {
                this.SourceTestConnectionTb.Text = "Failed: " + ex.Message;
            }
        }

        /// <summary>
        /// This method will test the target database connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetTestConnectionOnClickBtn(object sender, EventArgs e)
        {
            // Example: "Persist Security Info=False;Integrated Security=SSPI;database=FuelsManagerQaEnterpriseDB;server=GMPC4G3\SQLSERVER2019;Connect Timeout=60"
            var connectionDA = new MigrationDatabaseDAClass
            {
                ConnectionString = this.TargetSecurityTb.Text
                                            + ";database=" + this.TargetDbNameTb.Text
                                            + ";server=" + this.TargetDbServerTb.Text
                                            + ";Connect Timeout=" + this.TargetConnectTimeoutSpinner.Value
            };

            try
            {
                connectionDA.TestConnection();
                this.TargetTestConnectionTb.Text = "Successful";
            }
            catch (Exception ex)
            {
                this.TargetTestConnectionTb.Text = "Failed: " + ex.Message;
            }
        }

        /// <summary>
        /// This method handles the text on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceDbServerOnChange(object sender, EventArgs e)
        {
            if (this.dbConfigurationDo != null)
            {
                this.SourceConnectTimeoutSpinner.Value = this.dbConfigurationDo.SourceDbConnectTimeoutInt;
                this.TargetConnectTimeoutSpinner.Value = this.dbConfigurationDo.TargetDbConnectTimeoutInt;
            }

            if (this.dbConfigurationDo != null)
            {
                this.dbConfigurationDo.SourceDbConnectionDbServer = this.SourceDbServerTb.Text;
            }
            
            this.SetSourceTextConnectionBtn();
        }

        /// <summary>
        /// This method handles the text on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceDbNameOnChange(object sender, EventArgs e)
        {
            if (this.dbConfigurationDo != null)
            {
                this.dbConfigurationDo.SourceDbConnectionDbName = this.SourceDbNameTb.Text;
            }

            this.SetSourceTextConnectionBtn();
        }

        /// <summary>
        /// This method handles the text on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceSecurityOnChange(object sender, EventArgs e)
        {
            if (this.dbConfigurationDo != null)
            {
                this.dbConfigurationDo.SourceDbConnectionSecurity = this.SourceSecurityTb.Text;
            }

            this.SetSourceTextConnectionBtn();
        }

        /// <summary>
        /// This method handles the source connection timeout spinner on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SourceConnectSpinnerOnChange(object sender, System.EventArgs e)
        {
            // Inforce timeout to be 10 seconds or greater.
            if (this.SourceConnectTimeoutSpinner.Value < 10)
            {
                this.SourceConnectTimeoutSpinner.Value = 60;
            }

            if (this.dbConfigurationDo != null)
            {
                this.dbConfigurationDo.SourceDbConnectionTimeout = this.SourceConnectTimeoutSpinner.Value.ToString();
            }
        }

        /// <summary>
        /// This method handles the text on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetDbServerOnChange(object sender, EventArgs e)
        {
            if (this.dbConfigurationDo != null)
            {
                this.dbConfigurationDo.TargetDbConnectionDbServer = this.TargetDbServerTb.Text;
            }

            this.SetTargetTextConnectionBtn();
        }

        /// <summary>
        /// This method handles the text on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetDbNameOnChange(object sender, EventArgs e)
        {
            if (this.dbConfigurationDo != null)
            {
                this.dbConfigurationDo.TargetDbConnectionDbName = this.TargetDbNameTb.Text;
            }

            this.SetTargetTextConnectionBtn();
        }

        /// <summary>
        /// This method handles the text on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetSecurityOnChange(object sender, EventArgs e)
        {        
            if (this.dbConfigurationDo != null)
            {
                this.dbConfigurationDo.TargetDbConnectionSecurity = this.TargetSecurityTb.Text;
            }

            this.SetTargetTextConnectionBtn();
        }

        /// <summary>
        /// This method handles the target connection timeout spinner on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetConnectSpinnerOnChange(object sender, System.EventArgs e)
        {
            // Inforce timeout to be 10 seconds or greater.
            if (this.TargetConnectTimeoutSpinner.Value < 10)
            {
                this.TargetConnectTimeoutSpinner.Value = 60;
            }

            if (this.dbConfigurationDo != null)
            {
                this.dbConfigurationDo.TargetDbConnectionTimeout = this.TargetConnectTimeoutSpinner.Value.ToString();
            }
        }
        #endregion
    }
}
