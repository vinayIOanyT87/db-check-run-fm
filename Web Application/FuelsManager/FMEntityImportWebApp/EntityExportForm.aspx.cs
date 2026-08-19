// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityExportForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityExportForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
    using System;
    using System.Collections;
    using System.Web.UI.WebControls;

    using Accounting;

    using EntityImportExport;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FMControls;

    public partial class EntityExportForm : AccountingWebFormView
    {
        #region Constants and Fields

        protected FMLabel ExportFileLable;

        private const string ERR_MSG_001 = "Select a file to export";

        private const string ERR_MSG_002 = "Export file is empty";

        private AccountingSite accountingSite;

        private ExcelExport excelExport;

        #endregion

        #region Methods

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();

            // Want to ignore the disabling of inputs on post backs.
            this.IgnoreInputDisable = true;

            base.OnInit(e);
            this.Initialize();
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
                this.LoadExportTypes();

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
            this.ExportTypeDropdown.Enabled = enable;
            this.ExportProductsCB.Enabled = enable;
            this.ExportPersonnelCB.Enabled = enable;
            this.ExportEquipmentCB.Enabled = enable;
            this.ExportCompaniesCB.Enabled = enable;
            this.ExportStandingOffersCB.Enabled = enable;
            this.ExportFuelCardCB.Enabled = enable;
            this.ExportIATACodesCB.Enabled = enable;
            this.ExportEquipmentTypesCB.Enabled = enable;
            this.ExportAssignmentsCB.Enabled = enable;
            this.ExportPointsCB.Enabled = enable;
            this.ExportPointTemplatesCB.Enabled = enable;
            this.ExportPointCategoriesCB.Enabled = enable;
            this.ExportPointTypesCB.Enabled = enable;
            this.ExportPointTagsCB.Enabled = enable;
            this.ExportBtn.Enabled = enable;
            this.IncludeStrapTablesCB.Enabled = enable;
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ClearChkBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OnCommandClearChkBtn);
            this.ExportBtn.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OnCommandExportBtn);
        }

        /// <summary>
        ///    This method will load the export types into the export type
        ///    dropdown list.
        /// </summary>
        private void LoadExportTypes()
        {
            var exportTypes = new ArrayList();
            var exportType = new ListItem();

            exportType.Text = "Excel";
            exportType.Value = "EXCEL";
            exportTypes.Add(exportType);

            this.ExportTypeDropdown.DataSource = exportTypes;
            this.ExportTypeDropdown.DataTextField = "Text";
            this.ExportTypeDropdown.DataValueField = "Value";
            this.ExportTypeDropdown.DataBind();
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
        ///    This method handles the Export button being pressed event. It will process
        ///    the exporting of entity data from an excel spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandExportBtn(object sender, CommandEventArgs e)
        {
            try
            {
                bool okToSave = true;

                this.Visibility(true);

                this.excelExport = new ExcelExport(base.security, this.accountingSite.CurrentSite);

                // Set which entities are to be exported
                this.excelExport.ExportCompanies = this.ExportCompaniesCB.Checked;
                this.excelExport.ExportEquipment = this.ExportEquipmentCB.Checked;
                this.excelExport.ExportPersonnel = this.ExportPersonnelCB.Checked;
                this.excelExport.ExportProducts = this.ExportProductsCB.Checked;
                this.excelExport.ExportStandingOffers = this.ExportStandingOffersCB.Checked;
                this.excelExport.ExportFuelCard = this.ExportFuelCardCB.Checked;
                this.excelExport.ExportIATACodes = this.ExportIATACodesCB.Checked;
                this.excelExport.ExportEquipmentTypes = this.ExportEquipmentTypesCB.Checked;
                this.excelExport.ExportAssignments = this.ExportAssignmentsCB.Checked;
                this.excelExport.ExportPoints = this.ExportPointsCB.Checked;
                this.excelExport.ExportPointTemplates = this.ExportPointTemplatesCB.Checked;
                this.excelExport.ExportPointCategories = this.ExportPointCategoriesCB.Checked;
                this.excelExport.ExportPointTypes = this.ExportPointTypesCB.Checked;
                this.excelExport.ExportPointTags = this.ExportPointTagsCB.Checked;
					 if (this.excelExport.ExportPointTemplates)
					 {
						 this.excelExport.IncludeStrapTables = true;
					 }
					 else
					 {
					    this.excelExport.IncludeStrapTables = this.IncludeStrapTablesCB.Checked;
					 }
					 if (AreAnyEntitiesSelectedForExport() == false)
                {
                    throw new Exception("No entity types selected for export.");
                }

                try
                {
                    this.excelExport.Export();

                    if (this.excelExport.ImportException.HasException)
                    {
                        string critical = this.excelExport.ImportException.CriticalMessage;
                        string error = this.excelExport.ImportException.ErrorMessage;
                        string warning = this.excelExport.ImportException.WarningMessage;
                        string info = this.excelExport.ImportException.InfoMessage;

                        string message = "";

                        if (critical != null && critical.Length > 0)
                        {
                            message += critical;
                            message += "\n";
                            okToSave = false;
                        }

                        if (error != null && error.Length > 0)
                        {
                            message += error;
                            message += "\n";
                            okToSave = false;
                        }

                        if (warning != null && warning.Length > 0)
                        {
                            message += warning;
                            message += "\n";
                        }

                        if (info != null && info.Length > 0)
                        {
                            message += info;
                        }

                        this.ResultsTB.Text = message;
                    }

                    // only save if there are no critical or error messages
                    if (okToSave)
                    {
                        this.Response.ClearContent();
                        this.Response.ClearHeaders();
                        this.Response.ContentType = "text/xml";
                        this.Response.AddHeader("Content-Disposition", "attachment; filename=FMEntityExport.xml");
                        this.Response.AddHeader("cache-control", "private, max-age=0");
                        this.Response.Write(this.excelExport.ExcelXMLDocument.Replace("\n","&#10;")); //newline encoding for MS Excel XML document
                        this.Response.Flush();
                        this.Response.SuppressContent = true;
                    }
                }
                catch (ImportExportException impExptExcept)
                {
                    if (impExptExcept.HasException)
                    {
                        this.ResultsTB.Text = impExptExcept.CriticalMessage;
                        this.ResultsTB.Text = impExptExcept.ErrorMessage;
                    }
                }
            }
            catch (Exception except)
            {
                base.ErrorHandler(except);
            }
        }

        private bool AreAnyEntitiesSelectedForExport()
        {
            return (this.excelExport.ExportCompanies
               || this.excelExport.ExportEquipment
               || this.excelExport.ExportPersonnel
               || this.excelExport.ExportProducts
               || this.excelExport.ExportStandingOffers
               || this.excelExport.ExportFuelCard
               || this.excelExport.ExportIATACodes
               || this.excelExport.ExportEquipmentTypes
               || this.excelExport.ExportAssignments
               || this.excelExport.ExportPoints
               || this.excelExport.ExportPointTemplates
   || this.excelExport.ExportPointCategories
   || this.excelExport.ExportPointTypes
   || this.excelExport.ExportPointTags);
        }

        /// <summary>
        ///    This method checks or unchecks all the checkbox controls.
        /// </summary>
        /// <param name="Checked"></param>
        private void SetAllCheckboxState(bool Checked)
        {
            this.ExportProductsCB.Checked = Checked;
            this.ExportPersonnelCB.Checked = Checked;
            this.ExportEquipmentCB.Checked = Checked;
            this.ExportCompaniesCB.Checked = Checked;
            this.ExportStandingOffersCB.Checked = Checked;
            this.ExportFuelCardCB.Checked = Checked;
            this.ExportIATACodesCB.Checked = Checked;
            this.ExportEquipmentTypesCB.Checked = Checked;
            this.ExportAssignmentsCB.Checked = Checked;
            this.ExportPointTemplatesCB.Checked = Checked;
            this.ExportPointCategoriesCB.Checked = Checked;
            this.ExportPointTypesCB.Checked = Checked;
            
            if (!Checked)
            { 
               this.ExportPointsCB.Checked = Checked;
               this.ExportPointTagsCB.Checked = Checked;
            }
            else
            { 
               this.ExportPointsCB.Checked = !Checked;
               this.IncludeStrapTablesCB.Enabled = this.ExportPointsCB.Checked;
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