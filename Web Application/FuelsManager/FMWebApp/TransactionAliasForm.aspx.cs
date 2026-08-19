// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
	using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Web.UI.WebControls;

    using AjaxControlToolkit;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;

    using FMControls;
    using FMCore;
    using FMDepedencyManager;
    using FuelsManager.Interfaces;

    /// <summary>
    ///    Summary description for TransactionAliasForm.
    /// </summary>
    public partial class TransactionAliasForm : FMAutoSubmitFormBase
	{
        #region Public Properties

        /// <summary>
        ///    Add comments for this section...
        /// </summary>
        public List<string> VersionSpecificFields { get; set; }
        public FMTabContainer MainTabControl { get; set; }

        #endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the transaction alias form.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls(bool enable)
		{
			TransactionAliasClass TransactionAlias;
			TransactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			if (this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES) && (this.Security.SiteGuid == TransactionAlias.SiteGuid)
				 || TransactionAlias.SiteGuid == Guid.Empty)
			{
				this.OK.Enabled = enable;
				this.New.Enabled = enable;
			}
			this.Cancel.Enabled = enable;

			this.tcTransactionAliasTabs.HeaderEnabled = enable;
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			this.MainTabControl = this.tcTransactionAliasTabs;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("Status");

				base.GetSecurity();

				TransactionAliasClass transactionAlias;

				if (this.Page.IsPostBack == false)
				{
					transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

					if (transactionAlias == null)
					{
						// Get TransactionAlias
						if (this.Session["IdentityGuid"] != null)
						{
							transactionAlias =
								FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
									x => x.Get(this.Security, Guid.Parse((string)this.Session["IdentityGuid"]), false));
						}
						else
						{
							transactionAlias = new TransactionAliasClass();
							transactionAlias.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;

							// GKendall - Defaulting new alias to have all statuses assigned
							var Statuses = (int[])Enum.GetValues(typeof(TransactionStatus));
							foreach (int Status in Statuses)
							{
								transactionAlias.AssignedStatuses.Add(Status);
							}
						}

						this.Session["TransactionAlias"] = transactionAlias;
						this.Session["TransactionAliasOriginalID"] = transactionAlias.ID;
					}
					
					if (transactionAlias != null)
					{
                  this.GetRecordVersioningFields();
                  if (!this.Security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
                        || (transactionAlias.SiteGuid != Guid.Empty &&
                           this.Security.SiteGuid != transactionAlias.SiteGuid &&
                           (this.VersionSpecificFields == null || this.VersionSpecificFields.Count == 0))
                     )
                  {
                        this.OK.Enabled = false;
                        this.New.Enabled = false;
                  }

                  // Adding the name of the alias that's being configured to the Page header
                  this.labHeader.Text = this.GetTitleLabelText(this.labHeader.Text, transactionAlias.ID);
					}

					// Apply the Data dictionary to the tab headers
					foreach (TabPanel tab in this.tcTransactionAliasTabs.Tabs)
					{
						tab.HeaderText = this.GetTranslatedText(tab.HeaderText);
					}

               var config = FMServiceLocator.GetInstance<IFuelManagerConfigurationFactory>().GetConfig();
               if (config.EnableAjaxTransactionScreen)
               {
                  tpFieldPlacement.Visible = true;
               }
				}
				else
				{
					if (this.Session["TransactionAlias"] == null)
					{
						throw new Exception("TransactionAlias not in Session");
					}

					transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

					if ( this.Request.GetQueryOrFormValue( "__EVENTARGUMENT" ) == "FIELDORDERPOSTBACK" )
					{
						this.tcTransactionAliasTabs.ActiveTabIndex = 4;
						this.tcTransactionAliasTabs.ActiveTab = this.tpFieldOrderPage;
                  string InitialHelpKeyJS = "window.parent.OverrideKey = 'FMWebApp/TransactionAliasFieldOrderPage.ascx'; ";
                  this.Page.ClientScript.RegisterStartupScript(this.GetType(), "HelpMapping", InitialHelpKeyJS, true);
               }

					this.VersionSpecificFields = this.Session["TransactionAliasVersionSpecificFields"] as List<string>;
				}

				if (transactionAlias != null)
				{
					if (transactionAlias.TransTypeID == TransactionTypes.T17_Order
					|| transactionAlias.TransTypeID == TransactionTypes.T18_SupplyOrder)
					{
						this.tpAssociationsPage.Visible = false;
						transactionAlias.AssociatedAliases.Clear();
					}
					else
					{
						transactionAlias.AssociatedTransactionAliasGuid = Guid.Empty;
						transactionAlias.AssociatedAlias = string.Empty;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("TransactionAlias");

			this.Redirect("TransactionAliasesForm.aspx");
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);
			this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
			this.New.Command += new System.Web.UI.WebControls.CommandEventHandler(this.New_Command);
		}

		private void New_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

				if (transactionAlias.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ITransactionAliases>(x => x.Modify(this.Security, transactionAlias));
				}
				else
				{
					FMChannelHelper.MakeCall<ITransactionAliases>(x => x.Add(this.Security, transactionAlias));
				}

				if (transactionAlias.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.RefreshTransactionSecurityRightsCache(ref this.Security)
																);

					this.Session["Security"] = this.Security;

					try
					{
						if (UsingLoadRack)
						{
							ILoadRackManager loadRackManager = this.GetLoadRackManager();
							loadRackManager.Modify(this.Security, typeof(TransactionAliasClass), transactionAlias.IdentityGuid);
						}
					}
					catch (SocketException socketExcept)
					{
						if (socketExcept.ErrorCode != 10061)
						{
							throw socketExcept;
						}
					}
				}

				transactionAlias.ID = "";
				transactionAlias.IdentityGuid = Guid.Empty;
				transactionAlias.MasterRecordGuid = Guid.Empty;

				this.Session.Remove("TransactionAlias");
				this.ucFMMenuBar.Refresh();
				this.Session.Remove("TransactionAliasOriginalID");
				this.Session.Add("TransactionAlias",transactionAlias);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("TransactionAliasForm.aspx");
		}



		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

				if (transactionAlias.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ITransactionAliases>(x => x.Modify(this.Security, transactionAlias));
				}
				else
				{
					FMChannelHelper.MakeCall<ITransactionAliases>(x => x.Add(this.Security, transactionAlias));
				}

				if (transactionAlias.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.RefreshTransactionSecurityRightsCache(ref this.Security)
																);

					this.Session["Security"] = this.Security;

					try
					{
						if (UsingLoadRack)
						{
							ILoadRackManager loadRackManager = this.GetLoadRackManager();
							loadRackManager.Modify(this.Security, typeof(TransactionAliasClass), transactionAlias.IdentityGuid);
						}
					}
					catch (SocketException socketExcept)
					{
						if (socketExcept.ErrorCode != 10061)
						{
							throw socketExcept;
						}
					}
				}

				this.Session.Remove("TransactionAlias");
				this.ucFMMenuBar.Refresh();
				this.Session.Remove("TransactionAliasOriginalID");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("TransactionAliasesForm.aspx");
		}


        private void GetRecordVersioningFields()
        {
            this.VersionSpecificFields = new List<string>();
            TransactionAliasClass transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);
            this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] = this.VersionSpecificFields;

            if ((transactionAlias == null) 
                || (transactionAlias.IdentityGuid.Equals(Guid.Empty)) 
                || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
            {
                return;
            }
            string flcMode = FieldLevelConfigClass.FLCModeGSOnly;
            if (currentSiteOwnsRecordVersion)
                flcMode = FieldLevelConfigClass.FLCModeVSandGS;

            try
            {
                this.VersionSpecificFields = FMChannelHelper.MakeCall<IEntityToSiteMaps, List<string>>(
                                                                x =>
                                                                x.GetRecordVersioningFields(this.Security, transactionAlias.EntityType, transactionAlias.MasterRecordGuid, flcMode)
                                                           );

                this.Session["TransactionAliasVersionSpecificFields"] = this.VersionSpecificFields;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
            if (this.VersionSpecificFields == null)
            {
                this.VersionSpecificFields = new List<string>();
            }
            this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] = this.VersionSpecificFields;
        }

		#endregion
	}

	/// <summary>
	///    Page base for gaining access to company object
	/// </summary>
	public class TransactionAliasPageBase : FMUserControlBase
	{
		protected FMTabContainer GetTabControl()
		{
			return ((TransactionAliasForm)this.Page).MainTabControl;
		}

        //protected List<string> VersionSpecificFields => ((TransactionAliasForm)this.Page).VersionSpecificFields;
    }
}
