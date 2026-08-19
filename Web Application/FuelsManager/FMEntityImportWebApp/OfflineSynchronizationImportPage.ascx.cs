// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfflineSynchronizationImportPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OfflineSynchronizationImportPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
    using System;
    using System.IO;
    using System.Security;
    using System.Web;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;

    using FuelsManager.FMWebApp;

	/// <summary>
    /// Code behind for OfflineSynchronizationImportPage that allows a user to import the synchronization download file returned by the enterprise synchronization service.
    /// </summary>
    public partial class OfflineSynchronizationImportPage : FMUserControlBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the Client Sync Configuration object from Session.
        /// </summary>
        private SyncClientConfigurationDO SessionSyncClientConfig
        {
            get
            {
	            if (this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS] != null && this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS] is SyncClientConfigurationDO)
                {
                    return (SyncClientConfigurationDO)this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS];
                }
	            
				return null;
            }
	        set
            {
                this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS, value);
            }
        }
        #endregion Properties

        #region Public Methods and Operators
        /// <summary>
        /// The update data.
        /// </summary>
        /// <exception cref="Exception">
        /// </exception>
        public void UpdateData()
        {
            try
            {
                var syncClientConfig = this.SessionSyncClientConfig;

                if (null == syncClientConfig)
                {
                    syncClientConfig = new SyncClientConfigurationDO();
                    this.SessionSyncClientConfig = syncClientConfig;

                    // Reference the one in the session.
                    syncClientConfig = this.SessionSyncClientConfig;
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        /// <summary>
        /// The update view.
        /// </summary>
        public void UpdateView()
        {
            try
            {
                var syncClientConfig = (SyncClientConfigurationDO)this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS];

                if (null != syncClientConfig)
                {

                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }
        #endregion Public Methods and Operators

        #region Page Event Handlers and Overrides
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
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        [SecurityCritical]
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.Page.IsPostBack)
                {
                    this.UpdateView();
                }

                // If a successful complete synchronization has already taken place, then we will not allow the user to change the synchronization Site/SiteGroup ID.
                // This prevents nodes from cycling through Sites/SiteGroups and pulling down all data from the enterprise.
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ImportBtn.Command += this.ImportBtnClickCommand;
        }
        #endregion Page Event Handlers and Overrides

        #region Control  Event Handlers
        /// <summary>
        /// Import button click event handler
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ImportBtnClickCommand(object sender, EventArgs e)
        {
            try
            {
                // We need to extract the file and start importing the data.
                if (this.Request.Files.AllKeys.Length == 0)
                {
                    throw new Exception("Missinge import file.");
                }
	            
				HttpPostedFile file = this.Request.Files[0];

	            if ((file.FileName == string.Empty) || (file.ContentLength == 0))
	            {
		            throw new Exception("Missing or empty import file.");
	            }

	            this.ResultsTB.Visible = true;
	            this.ResultsTB.Text = string.Empty;
	            this.ResultsLabel.Visible = true;

	            // Dump the import file to disk so the synchronization service can pick it up.
	            MemoryStream document = new MemoryStream();

	            try
	            {
		            // Tell the synchronization service to process the import file
		            //
	            }
	            catch (Exception ex)
	            {
		            while (ex.InnerException != null)
		            {
			            ex = ex.InnerException;
		            }

		            string errorMessage = ex.Message;

		            if (this.Security != null
		                && (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"]))
		            {
			            errorMessage =
				            FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.Security.LoginSiteGuid, ex.Message));
		            }

		            this.ResultsTB.Text = errorMessage;
	            }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }
        #endregion Control  Event Handlers
    }
}