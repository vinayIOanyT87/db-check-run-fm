// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityImportForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityImportForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Xml;

    using Accounting;

    using EntityImportExport;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    public partial class EntityImportForm : AccountingWebFormView
    {
        #region Constants and Fields

        private const string ERR_MSG_001 = "Select a file to import";

        private const string ERR_MSG_002 = "Import file is empty";

        private const string ERR_MSG_003 = "Select an entity type to import";

        private AccountingSite accountingSite;

        private ExcelImport excelImport;

        #endregion

        #region Methods

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            base.Initialize();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get site information.
            this.accountingSite =
               FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
                  accountingSites => accountingSites.LoadSiteInfoNoCompanies(this.security, this.security.SiteGuid));

            // Check permissions
            this.CheckPermissions();

            if (this.Page.IsPostBack == false)
            {
                this.Visibility(false);
                this.LoadImportTypes();

                // Always set the check boxes to checked
                this.SetAllCheckboxState(true);
            }
        }

        /// <summary>
        ///    This method will check permissions to ensure the user can execute
        ///    import/export.
        /// </summary>
        private void CheckPermissions()
        {
            if (base.security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == false)
            {
                this.EnableControls(false);
            }
            else
            {
                this.EnableControls(true);
            }
        }

        /// <summary>
        ///    This method enables and disables the controls.
        /// </summary>
        /// <param name="enable"></param>
        private void EnableControls(bool enable)
        {
            this.ImportTypeDropdown.Enabled = enable;
            this.ImportProductsCB.Enabled = enable;
            this.ImportPersonnelCB.Enabled = enable;
            this.ImportEquipmentCB.Enabled = enable;
            this.ImportCompaniesCB.Enabled = enable;
            this.ImportStandingOfferCB.Enabled = enable;
            this.ImportFuelCardCB.Enabled = enable;
            this.ImportIATACodesCB.Enabled = enable;
            this.ImportBtn.Enabled = enable;
            this.ImportEquipmentTypesCB.Enabled = enable;
            this.ImportAssignmentsCB.Enabled = enable;
            this.ImportPointsCB.Enabled = enable;
            this.ImportPointTemplatesCB.Enabled = enable;
            this.ImportPointCategoriesCB.Enabled = enable;
            this.ImportPointTypesCB.Enabled = enable;
            this.ImportPointTagsCB.Enabled = enable;
            this.IncludeStrapTablesCB.Enabled = enable;
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ClearChkBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OnCommandClearChkBtn);
            this.ImportBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OnCommandImportBtn);
        }

        /// <summary>
        ///    This method will load the import types into the import type
        ///    dropdown list.
        /// </summary>
        private void LoadImportTypes()
        {
            var importTypes = new ArrayList();
            var importType = new ListItem();

            importType.Text = "Excel";
            importType.Value = "EXCEL";
            importTypes.Add(importType);

            this.ImportTypeDropdown.DataSource = importTypes;
            this.ImportTypeDropdown.DataTextField = "Text";
            this.ImportTypeDropdown.DataValueField = "Value";
            this.ImportTypeDropdown.DataBind();
        }

        /// <summary>
        ///    This method handles the Clear button being pressed event. It will clear all check boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandClearChkBtn(object sender, CommandEventArgs e)
        {
            this.SetAllCheckboxState(false);
        }

        /// <summary>
        ///    This method handles the Import button being pressed event. It will process
        ///    the importing of entity data from an excel spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandImportBtn(object sender, CommandEventArgs e)
        {
            try
            {

                if (!this.ImportCompaniesCB.Checked
                && !this.ImportEquipmentCB.Checked
                && !this.ImportPersonnelCB.Checked
                && !this.ImportProductsCB.Checked
                && !this.ImportStandingOfferCB.Checked
                && !this.ImportFuelCardCB.Checked
                && !this.ImportIATACodesCB.Checked
                && !this.ImportEquipmentTypesCB.Checked
                && !this.ImportAssignmentsCB.Checked
                && !this.ImportPointsCB.Checked
                && !this.ImportPointTemplatesCB.Checked
                && !this.ImportPointCategoriesCB.Checked
    && !this.ImportPointTypesCB.Checked
    && !this.ImportPointTagsCB.Checked
    )
                {
                    throw new Exception(ERR_MSG_003);
                }



                if (this.Request.Files.AllKeys.Length == 0)
                {
                    throw new Exception(ERR_MSG_001);
                }
                else
                {
                    HttpPostedFile file = this.Request.Files[0];

                    if ((file.FileName == "") || (file.ContentLength == 0))
                    {
                        throw new Exception(ERR_MSG_001);
                    }
                    else
                    {
                        this.Visibility(true);

                        var document = new XmlDocument();

                        document.XmlResolver = null;

                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.XmlResolver = null;
                        settings.DtdProcessing = DtdProcessing.Prohibit;
                        XmlReader reader = null;
                        try
                        {
                            reader = XmlReader.Create(file.InputStream, settings);
                            document.Load(reader);
                        }
                        catch (XmlException exception)
                        {

                            throw new FMXmlException(exception);
                        }
                        finally
                        {
                            if (reader != null)
                                reader.Close();
                        }

                        if (document == null)
                        {
                            this.ResultsTB.Text = ERR_MSG_002;
                        }
                        else
                        {
                            this.excelImport = new ExcelImport(base.security, this.accountingSite.CurrentSite, document);

                            // Set which entities are to be imported
                            this.excelImport.ImportCompanies = this.ImportCompaniesCB.Checked;
                            this.excelImport.ImportEquipment = this.ImportEquipmentCB.Checked;
                            this.excelImport.ImportPersonnel = this.ImportPersonnelCB.Checked;
                            this.excelImport.ImportProducts = this.ImportProductsCB.Checked;
                            this.excelImport.ImportStandingOffers = this.ImportStandingOfferCB.Checked;
                            this.excelImport.ImportFuelCard = this.ImportFuelCardCB.Checked;
                            this.excelImport.ImportIATACodes = this.ImportIATACodesCB.Checked;
                            this.excelImport.ImportEquipmentTypes = this.ImportEquipmentTypesCB.Checked;
                            this.excelImport.ImportAssignments = this.ImportAssignmentsCB.Checked;
                            this.excelImport.ImportPoints = this.ImportPointsCB.Checked;
                            this.excelImport.ImportPointTemplates = this.ImportPointTemplatesCB.Checked;
                            this.excelImport.ImportPointCategories = this.ImportPointCategoriesCB.Checked;
                            this.excelImport.ImportPointTypes = this.ImportPointTypesCB.Checked;
                            this.excelImport.ImportPointTags = this.ImportPointTagsCB.Checked;
							       this.excelImport.IncludeStrapTables = this.IncludeStrapTablesCB.Checked;

							   try
							   {
                                this.excelImport.StartImport();

                                if (this.excelImport.ImportException.HasException)
                                {
                                    string critical = this.excelImport.ImportException.CriticalMessage;
                                    string error = this.excelImport.ImportException.ErrorMessage;
                                    string warning = this.excelImport.ImportException.WarningMessage;
                                    string info = this.excelImport.ImportException.InfoMessage;

                                    string message = "";
                                    if (critical != null && critical.Length > 0)
                                    {
                                        message = critical;
                                    }
                                    if (message.Length > 0)
                                    {
                                        message += "\n";
                                    }
                                    if (error != null && error.Length > 0)
                                    {
                                        message += error;
                                    }
                                    if (message.Length > 0)
                                    {
                                        message += "\n";
                                    }
                                    if (warning != null && warning.Length > 0)
                                    {
                                        message += warning;
                                    }
                                    if (message.Length > 0)
                                    {
                                        message += "\n";
                                    }
                                    if (info != null && info.Length > 0)
                                    {
                                        message += info;
                                    }

                                    this.ResultsTB.Text = message;
                                }
                            }
                            catch (ImportExportException impExptExcept)
                            {
                                if (impExptExcept.HasException)
                                {
                                    this.ResultsTB.Text = impExptExcept.CriticalMessage;
                                    this.ResultsTB.Text += "\n" + impExptExcept.ErrorMessage;
                                }
                            }
                            catch (Exception ex)
                            {
                                while (ex.InnerException != null)
                                {
                                    ex = ex.InnerException;
                                }

                                string errorMessage = ex.Message;

                                if (this.security != null
                                    && (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"]))
                                {
                                    errorMessage = GetDataDictionaryValueByKey(this.security.LoginSiteGuid, ex.Message);
                                }
                                else
                                {
                                    errorMessage = new DataDictionaryCollectionClass()[ex.Message];
                                }

                                this.ResultsTB.Text = errorMessage;
                            }
                        }
                    }
                }
            }
            catch (Exception except)
            {
                base.ErrorHandler(except);
            }
        }

        /// <summary>
        ///    This method checks or unchecks all the checkbox controls.
        /// </summary>
        /// <param name="Checked"></param>
        private void SetAllCheckboxState(bool Checked)
        {
            this.ImportProductsCB.Checked = Checked;
            this.ImportPersonnelCB.Checked = Checked;
            this.ImportEquipmentCB.Checked = Checked;
            this.ImportCompaniesCB.Checked = Checked;
            this.ImportStandingOfferCB.Checked = Checked;
            this.ImportFuelCardCB.Checked = Checked;
            this.ImportIATACodesCB.Checked = Checked;
            this.ImportEquipmentTypesCB.Checked = Checked;
            this.ImportAssignmentsCB.Checked = Checked;
            this.ImportPointsCB.Checked = Checked;
            this.ImportPointTemplatesCB.Checked = Checked;
            this.ImportPointCategoriesCB.Checked = Checked;
            this.ImportPointTypesCB.Checked = Checked;
            this.ImportPointTagsCB.Checked = false;

            if (this.ImportPointsCB.Checked)
            {
               this.IncludeStrapTablesCB.Checked = false;
               this.IncludeStrapTablesCB.Enabled = true;
            }
            else
            {
	            this.IncludeStrapTablesCB.Checked = false;
	            this.IncludeStrapTablesCB.Enabled = false;
            }
		}

        /// <summary>
        ///    This method will turn visibility on or off for the results.
        /// </summary>
        /// <param name="visible"></param>
        private void Visibility(bool visible)
        {
            this.ResultsTB.Visible = visible;
            this.ResultsTB.Text = "";
            this.ResultsLabel.Visible = visible;
        }

        #endregion
    }
}