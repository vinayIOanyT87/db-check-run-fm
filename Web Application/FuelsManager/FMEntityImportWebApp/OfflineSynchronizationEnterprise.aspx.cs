// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfflineSynchronizationEnterprise.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OfflineSynchronizationEnterprise type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
    using System;
    using System.IO;
    using System.Web;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FuelsManager.FMWebApp;

    public partial class OfflineSynchronizationEnterprise : FMFormBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the SyncOfflineServiceState object from Session.
        /// </summary>
        private SyncServiceStateDO SyncServiceState
        {
            get
            {
	            if (this.Session[PageSessionKeyConstants.SYNC_OFFLINE_SERVICE_STATE] != null 
					&& this.Session[PageSessionKeyConstants.SYNC_OFFLINE_SERVICE_STATE] is SyncServiceStateDO)
                {
                    return (SyncServiceStateDO)this.Session[PageSessionKeyConstants.SYNC_OFFLINE_SERVICE_STATE];
                }

	            return null;
            }
	        set
            {
                this.Session.Add(PageSessionKeyConstants.SYNC_OFFLINE_SERVICE_STATE, value);
            }
        }
        #endregion Properties

 
        #region Methods and Operators

        /// <summary>
        ///    This method will either enable or disable controls.  It is called by
        ///    the individual tabs associated to the site form.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableControls(bool enable)
        {
            if (this.Security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
            {
                this.ImportBtn.Enabled = enable;
            }
            else
            {
                this.ImportBtn.Enabled = false;
            }
        }

        /// <summary>
        /// Update all object(s) in session with any data the user has entered on the page
        /// </summary>
        public void UpdateData()
        {
            //this.SiteSyncSettingsPage.UpdateData();
        }

        /// <summary>
        /// Populate the fields on the screen with data
        /// </summary>
        private void UpdateView()
        {
            try
            {
                // Determine the current state of synchronization
                //SyncOfflineServiceStateDO syncServiceState = FMChannelHelper.MakeCall<ISynchronizationServices, SyncOfflineServiceStateDO>((config) => { return config.Get(Security); });
                //this.SyncServiceState = syncServiceState;

                //if (null == this.SyncServiceState)
                    //this.SyncServiceState = new SyncOfflineServiceStateDO();
            }
            catch (Exception ex)
            {
	            this.ErrorHandler(ex);
            }
        }
        #endregion Methods and Operators

        #region Page Events and Overrides
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                if (!this.Security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION))
                {
                    throw new Exception("Synchronization Access Denied");
                }

                if (!this.Page.IsPostBack)
                {
                    this.UpdateView();
                }
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
        #endregion Page Events and Overrides

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