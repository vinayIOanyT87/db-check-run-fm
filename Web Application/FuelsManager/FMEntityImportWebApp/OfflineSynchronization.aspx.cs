// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfflineSynchronization.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OfflineSynchronization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
    using System;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FuelsManager.FMWebApp;

	/// <summary>
    ///    Summary description for SiteForm.
    /// </summary>
    public partial class OfflineSynchronization : FMAutoSubmitFormBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Client Sync Configuration object from Session.
        /// </summary>
        private SyncClientConfigurationDO SessionSyncClientConfig
        {
/*
            get
            {
                return this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS] as SyncClientConfigurationDO;
            }
*/
	        set
            {
                this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS, value);
            }
        }
        #endregion Properties

         #region Methods and Operators

        /// <summary>
        /// Update all object(s) in session with any data the user has entered on the page
        /// </summary>
        public void UpdateData()
        {
            //this.PeriodicSyncSettingsPage.UpdateData();
        }

        /// <summary>
        /// Populate the fields on the screen with data
        /// </summary>
        private void UpdateView()
        {
            try
            {
                // Get the stored Client Synchronization Settings
                SyncClientConfigurationDO syncClientConfig = FMChannelHelper.MakeCall<ISyncClientConfigurations, SyncClientConfigurationDO>(config => config.Get(this.Security));
                syncClientConfig.Changed = false;

                this.SessionSyncClientConfig = syncClientConfig;
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }
        #endregion Methods and Operators

        #region Page Events and Overrides
        /// <summary>
        /// Overrides the virtual <code>OnInit</code> framework method.  Any special page specific initialization can be performed here.
        /// </summary>
        /// <param name="e">
        /// Event arguments of type <see cref="EventArgs"/> for the <code>OnInit</code> method.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tcOfflineSyncTabs.ActiveTabChanged += this.TcOfflineSyncTabsActiveTabChanged;
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="Exception">
        /// Throws an exception for Insufficient Rights if the user does not have ANY View rights for Client, Server and Periodic Synchronization Settings.
        /// </exception>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                if (!this.Security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
                {
                    throw new Exception("Insufficient Rights");
                }

                if (!this.Page.IsPostBack)
                {
                    // this.Session.Remove(PageSessionKeyConstants.SYNC_CONFIG_SITE_SETTINGS_MODIFIED);

                    this.UpdateView();
                }

                this.tpExportToEnterprise.Visible = false;
                this.tpImportFromEnterprise.Visible = false;

                if (this.Security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
                {
                    this.tpExportToEnterprise.Visible = true;
                    this.tpExportToEnterprise.HeaderText = this.GetTranslatedText("Export To Enterprise");
                    this.tpImportFromEnterprise.Visible = true;
                    this.tpImportFromEnterprise.HeaderText = this.GetTranslatedText("Import From Enterprise");
                }

                // We could be coming back from another page
                if (this.Session["OfflineSynchronizationConfig.TabIndex"] != null)
                {
                    this.tcOfflineSyncTabs.ActiveTabIndex = (int)this.Session["OfflineSynchronizationConfig.TabIndex"];
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }
        #endregion Page Events and Overrides

        #region Control  Event Handlers
        /// <summary>
        /// Updates the current tab index for this page in the session.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// Event arguments.
        /// </param>
        private void TcOfflineSyncTabsActiveTabChanged(object sender, EventArgs e)
        {
            this.Session["OfflineSynchronizationConfig.TabIndex"] = this.tcOfflineSyncTabs.ActiveTabIndex;
        }
        #endregion Control  Event Handlers
    }
}