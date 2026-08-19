namespace MigrationTool
{
    using FMBusinessObjects.DataObjects;
    using MirgrationToolProcessing;
    using System;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    public partial class MigrationToolForm : Form
    {
        private void InitializePersonnelToolTab()
        {
            this.ClearToolCheckboxes();
            this.LoadMigrationVersionComboBox();
            this.SetMigrateButton();

            // Temporary
            this.QueriesToolCb.Enabled = false;
        }

        /// <summary>
        /// This method loads the version combo box.
        /// </summary>
        private void LoadMigrationVersionComboBox()
        {
            var item = new ListItem
            {
                Text = MigrationVersionNone, 
                Value = MigrationVersionNoneIndex.ToString()
            };
            this.PersonnelMigrationVersionCB.Items.Add(item);

            item = new ListItem
            {
                Text = MigrationVersion753ToFmV12, 
                Value = MigrationVersion753ToFmV12Index.ToString()
            };
            this.PersonnelMigrationVersionCB.Items.Add(item);

            this.PersonnelMigrationVersionCB.SelectedIndex = 0;
        }

        /// <summary>
        /// This method will un-check all the tool checkboxes.
        /// </summary>
        private void ClearToolCheckboxes()
        {
            this.EquipmentToolCb.Checked            = false;
            this.PersonnelToolCb.Checked            = false;
            this.StationsToolCb.Checked             = false;
            this.FootnoteToolCb.Checked             = false;
            this.QueriesToolCb.Checked              = false;
            this.EquipmentTypeToolCb.Checked        = false;
            this.QualificationToolCb.Checked        = false;
            this.ApplicationStringToolCb.Checked    = false;
        }

        /// <summary>
        /// This method enables/disables the start migration button based on the 
        /// selected value.
        /// </summary>
        private void SetMigrateButton()
        {
            this.PersonnelStartMigrationBtn.Enabled = false;

            bool anyToolsChecked = this.EquipmentToolCb.Checked | this.PersonnelToolCb.Checked | this.StationsToolCb.Checked
                                    | this.FootnoteToolCb.Checked | this.QueriesToolCb.Checked | this.EquipmentTypeToolCb.Checked
                                    | this.QualificationToolCb.Checked | this.ApplicationStringToolCb.Checked | this.FootnoteToolCb.Checked;

            // None = 0
            if (this.PersonnelMigrationVersionCB.SelectedIndex > 0
                && string.IsNullOrEmpty(this.PersonnelSourceSiteIdTb.Text) == false
                && string.IsNullOrEmpty(this.PersonnelTargetSiteIdTb.Text) == false
                && anyToolsChecked)
            {
                this.PersonnelStartMigrationBtn.Enabled = true;
            }
        }

        /// <summary>
        /// This method will handle the migration combo box on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonnelMigrationCbOnChange(object sender, EventArgs e)
        {
            this.SetMigrateButton();
        }

        /// <summary>
        /// This method will handle the source site ID text box on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonnelSourceSiteIdOnChange(object sender, EventArgs e)
        {
            this.SetMigrateButton();

            if(this.StationsToolCb.Checked)
            {
                this.PersonnelTargetSiteIdTb.Text = this.PersonnelSourceSiteIdTb.Text;
            }
        }

        /// <summary>
        /// This method will handle the target site ID text box on change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonnelTargetSiteIdOnChange(object sender, EventArgs e)
        {
            this.SetMigrateButton();
        }

        /// <summary>
        /// This method handles the personnel clear button on click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonnelClearBtnOnClick(object sender, EventArgs e)
        {
            this.PersonnelResultsTb.Text = string.Empty;
        }

        /// <summary>
        /// This method will hanble the start migration on click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonnelStartMigrationBtnOnClick(object sender, EventArgs e)
        {
            this.PersonnelClearBtnOnClick(null, null);

            this.StartMigrationConfirmation();
        }

        /// <summary>
        /// This method will display a confirmation dialog confirming the user wants to 
        /// start the migration.
        /// </summary>
        private void StartMigrationConfirmation()
        {
            string confirmationMessage = "Is the Destination Database backed up?" + Environment.NewLine
                                        + "'Yes' to continue migration or 'No' to stop.";
            DialogResult dialogResult = MessageBox.Show(confirmationMessage, "Confirm Start Migration", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                switch (this.PersonnelMigrationVersionCB.SelectedIndex)
                {
                    case MigrationVersion753ToFmV12Index:
                        this.Start753ToV12Migration();
                        break;
                }
            }
        }

        /// <summary>
        /// This method starts the 7.5.3 to FM v12 migration.
        /// </summary>
        private void Start753ToV12Migration()
        {
            this.PersonnelResultsTb.Text = string.Empty;

            // We want to migrate application strings.
            if (this.ApplicationStringToolCb.Checked)
            {
                var applicationStringMigrationProcess = new ApplicationStringProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text
                };

                this.WriteResultMessage("---- Started Application String migration process... ----");

                applicationStringMigrationProcess.MigrationProcess();

                if (applicationStringMigrationProcess.MessageFlag)
                {
                    this.WriteResultMessage(applicationStringMigrationProcess.Message);
                    this.WriteResultMessage("---- Completed Application String migration. ----");
                }
            }

            // We want to migrate the qualification.
            if (this.QualificationToolCb.Checked)
            {
                var qualificationMigrationProcess = new QualificationProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text
                };

                this.WriteResultMessage("---- Started Qualifications migration process... ----");

                qualificationMigrationProcess.MigrationProcess();

                if (qualificationMigrationProcess.MessageFlag)
                {
                    this.WriteResultMessage(qualificationMigrationProcess.Message);
                    this.WriteResultMessage("---- Completed Qualifications migration. ----");
                }
            }

            // Equipment types migration
            if (this.EquipmentTypeToolCb.Checked)
            {
                var equipmentTypeMigrationProcess = new EquipmentTypesProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text, 
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text
                };

                this.WriteResultMessage("---- Started Equipment Types migration process... ----");

                equipmentTypeMigrationProcess.MigrationProcess();

                if (equipmentTypeMigrationProcess.MessageFlag)
                {
                    this.WriteResultMessage(equipmentTypeMigrationProcess.Message);
                    this.WriteResultMessage("---- Completed Equipment Types migration. ----");
                }
            }

            if (this.EquipmentToolCb.Checked)
            {
                // Map the User Data Field configuration for Equipment.
                var userDataFieldProcessing = new UserDataFieldProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text,
                    UserDataEntityType = ENTITY_TYPE.EQUIPMENT
                };

                this.WriteResultMessage("---- Started Equipment User Data Fields migration process... ----");
                userDataFieldProcessing.MigrationProcess();

                if (userDataFieldProcessing.MessageFlag)
                {
                    this.WriteResultMessage(userDataFieldProcessing.Message);
                    this.WriteResultMessage("---- Completed Equipment User Data Field migration. ----");
                }

                // Equipment migration.
                var equipmentMigrationProcess = new EquipmentProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text
                };

                this.WriteResultMessage("---- Started Equipment migration process... ----");

                equipmentMigrationProcess.MigrationProcess();

                if (equipmentMigrationProcess.MessageFlag)
                {
                    this.WriteResultMessage(equipmentMigrationProcess.Message);
                    this.WriteResultMessage("---- Completed Equipment migration. ----");
                }
            }

            if (this.PersonnelToolCb.Checked)
            {
                // Map the User Data Field configuration for Personnel.
                var userDataFieldProcessing = new UserDataFieldProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text,
                    UserDataEntityType = ENTITY_TYPE.PERSONNEL
                };

                this.WriteResultMessage("---- Started Personnel User Data Fields migration process... ----");
                userDataFieldProcessing.MigrationProcess();

                if (userDataFieldProcessing.MessageFlag)
                {
                    this.WriteResultMessage(userDataFieldProcessing.Message);
                    this.WriteResultMessage("---- Completed Personnel User Data Field migration. ----");
                }

                // Start migrating the Personnel data.
                var personnelMigrationProcess = new PersonnelProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text
                };

                this.WriteResultMessage("---- Started Personnel migration process... ----");
                personnelMigrationProcess.MigrationProcess();

                if (personnelMigrationProcess.MessageFlag)
                {
                    this.WriteResultMessage(personnelMigrationProcess.Message);
                    this.WriteResultMessage("---- Completed Personnel migration. ----");
                }
            }

            if (this.StationsToolCb.Checked)
            {
                if (this.PersonnelSourceSiteIdTb.Text != this.PersonnelTargetSiteIdTb.Text)
                {
                    this.WriteResultMessage("Error: To migrate Stations, the Source Site ID and Target Site ID must be the same.");
                    return;
                }

                // Map OPC Connections
                var opcConnectionProcessing = new OpcConnectionProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text
                };

                this.WriteResultMessage("---- Started OPC Connection migration process... ----");
                opcConnectionProcessing.MigrationProcess();

                if (opcConnectionProcessing.MessageFlag)
                {
                    this.WriteResultMessage(opcConnectionProcessing.Message);
                    this.WriteResultMessage("---- Completed OPC Connection migration. ----");
                }

                // Map Stations
                var stationProcessing = new StationProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text
                };

                this.WriteResultMessage("---- Started Station migration process... ----");
                stationProcessing.MigrationProcess();

                if (stationProcessing.MessageFlag)
                {
                    this.WriteResultMessage(stationProcessing.Message);
                    this.WriteResultMessage("---- Completed Station migration. ----");
                }
            }

            // We want to migrate footnotes.
            if (this.FootnoteToolCb.Checked)
            {
                var footnoteMigrationProcess = new FootnoteProcessing753ToV12(dbConfigurationDo)
                {
                    SourceSiteId = this.PersonnelSourceSiteIdTb.Text,
                    TargetSiteId = this.PersonnelTargetSiteIdTb.Text
                };

                this.WriteResultMessage("---- Started Footnote migration process... ----");

                footnoteMigrationProcess.MigrationProcess();

                if (footnoteMigrationProcess.MessageFlag)
                {
                    this.WriteResultMessage(footnoteMigrationProcess.Message);
                    this.WriteResultMessage("---- Completed Footnote migration. ----");
                }
            }
        }

        private void WriteResultMessage(string message)
        {
            this.PersonnelResultsTb.Text = this.PersonnelResultsTb.Text + Environment.NewLine + message;
        }

        /// <summary>
        /// This method handles the select tool checkboxes event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectToolsCheckboxOnClick(object sender, System.EventArgs e)
        {
            this.PersonnelTargetSiteIdTb.Enabled = true;
            this.EnableDisableToolsCb(true);
            this.SetMigrateButton();
        }

        /// <summary>
        /// This method handles the Equipment Type tool checkbox event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EquipmentTypeCheckboxOnClick(object sender, System.EventArgs e)
        {
            this.PersonnelTargetSiteIdTb.Enabled = true;
            this.EnableDisableToolsCb(true);

            if(this.EquipmentTypeToolCb.Checked)
            {
                this.PersonnelTargetSiteIdTb.Text = "SiteAdmin";
                this.PersonnelTargetSiteIdTb.Enabled = false;

                this.ClearToolCheckboxes();
                this.EquipmentTypeToolCb.Checked = true;

                this.EnableDisableToolsCb(false);
                this.EquipmentTypeToolCb.Enabled = true;
            }

            this.SetMigrateButton();
        }

        /// <summary>
        /// This method enables or disables the tools checkboxes.
        /// </summary>
        /// <param name="setting">True for enabled or False for disabled.</param>
        private void EnableDisableToolsCb(bool setting)
        {
            this.EquipmentToolCb.Enabled            = setting;
            this.EquipmentTypeToolCb.Enabled        = setting;
            this.PersonnelToolCb.Enabled            = setting;
            this.StationsToolCb.Enabled             = setting;
            this.FootnoteToolCb.Enabled             = setting;
            this.QualificationToolCb.Enabled        = setting;
            this.ApplicationStringToolCb.Enabled    = setting;
            //this.QueriesToolCb.Enabled              = setting;
        }

        /// <summary>
        /// This method handles the Stations tool checkbox event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StationToolsCheckboxOnClick(object sender, System.EventArgs e)
        {
            this.PersonnelTargetSiteIdTb.Enabled = true;
            this.EnableDisableToolsCb(true);

            if (this.StationsToolCb.Checked)
            {
                this.PersonnelTargetSiteIdTb.Text = this.PersonnelSourceSiteIdTb.Text;
                this.PersonnelTargetSiteIdTb.Enabled = false;

                this.ClearToolCheckboxes();
                this.StationsToolCb.Checked = true;

                this.EnableDisableToolsCb(false);
                this.StationsToolCb.Enabled = true;
            }

            this.SetMigrateButton();
        }
    }
}
