// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountingOperationsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AccountingOperationsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using System.Web;
    using System.Web.Services;
    using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FuelsManager.Accounting;

    /// <summary>
    ///   Code behind for Accounting Operations Form.
    /// </summary>
    public partial class AccountingOperationsForm : AccountingWebFormView
	{
		#region Constants and Fields

		private const string ConfirmProcessScript1 = @"
			<script type='text/javascript'>
			<!--
			ConfirmProcess(";

		private const string ConfirmProcessScript2 = @"); 
			//-->
			</script>
			";

        private ProductIrdoCollectionClass allProductsIrdoCollection;

		private AccountingSite accountingSite;

		private bool closeoutDisableFlag;

	    public string InlineSessionID
	    {
	        get
	        {
	            if (HttpContext.Current.Session.IsCookieless)
	            {
	                return "/(S(" + this.Session.SessionID + "))/";
	            }
	            return "";
	        }
	    }

	    public string StartDateText { get; set; } = "Start Date:";
	    public string StopDateText { get; set; } = "Stop Date:";
	    public string SpecifyStartDate { get; set; } = "Specify Start Date:";
	    public string Complete { get; set; } = "Finished!";
	    public string NumberOfTransactions { get; set; } = "Number of transactions to post to Enterprise:";
        #endregion

        #region Methods

        /// <summary>
        /// Closeouts all button_ confirmed closeout.
        /// </summary>
        protected void CloseoutAllButtonConfirmedCloseout()
		{
			if (this.Session["AllProductsIRDOCollection"] != null)
			{
                this.allProductsIrdoCollection = (ProductIrdoCollectionClass)this.Session["AllProductsIRDOCollection"];

                var closeoutSR = new CloseoutSR
				{
					Security = this.security,
					Site = this.security.SiteID,
					CurrentSiteGuid = this.security.SiteGuid,
					ManagerName = this.managerTextBox.Text,
					ManagerCompanyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
						companies => companies.GetMasterRecordGuid(this.security, this.managerTextBox.Text)),
					CloseoutCommand = CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP2,
					InventoryDate = this.CloseoutDate.CurrentValue.Date,
					AllProductsIrdoCollection = (ProductIrdoCollectionClass)this.Session["AllProductsIRDOCollection"]
				};

                CloseoutDO closeoutDO;
                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.CloseoutAllStartEvent));
                try
				{
					closeoutDO = FMChannelHelper.MakeCall<ICloseoutProcessor, CloseoutDO>(closeoutProcessor =>
					{
						((IClientChannel)closeoutProcessor).OperationTimeout = new TimeSpan(0, 30, 0);
						return closeoutProcessor.Process(closeoutSR);
					});

					if (closeoutDO.Closeouterror )
                    {
                        this.DisplayErrorDialog("[Error in creating a closeout transactions] - " + closeoutDO.Closeouterrtext + " \n");
                    }
                }
                catch (Exception exception)
                {
                    this.DisplayErrorDialog("[Error in creating a closeout transactions] - " + exception.Message + " \n");
                }

                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.security, TransactionAlarmEventDO.CloseoutAllEndEvent));

                closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP3;
                try
                {
                    closeoutDO = FMChannelHelper.MakeCall<ICloseoutProcessor,CloseoutDO>(x => x.Process(closeoutSR));
                    if (closeoutDO.Closeouterror == false)
                    {
                        // Let the user know that the closeout was successful by displaying a message
                        const string SuccessMessage = "Closeout successful.";
                        this.ClientScript.RegisterStartupScript(this.GetType(), "CloseoutSuccessScript", "alert('" + SuccessMessage + "');", true);
                    }
                }
                catch (Exception exception)
                {
                    this.DisplayErrorDialog("[Error in signaling closeout all complete] - " + exception.Message);
                }
			}
		}

		/// <summary>
		/// Displays the error dialog.
		/// </summary>
		/// <param name="errorMessage">
		/// The error message. 
		/// </param>
		protected void DisplayErrorDialog(string errorMessage)
		{
			string errMsg = "An Error has occurred!";

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errMsg = errorMessage;
			}

			this.RenderErrorMessage(errMsg);
		}

		/// <summary>
		/// Handles the Click event of the HiddenButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void HiddenButtonClick(object sender, EventArgs e)
		{
			this.CloseoutAllButtonConfirmedCloseout();
		}

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">
		/// The <see cref="System.EventArgs"/> instance containing the event data. 
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
			this.Initialize();
		}

		/// <summary>
		/// Handles the Load event of the AccountingOperationsForm control.
		/// </summary>
		/// <param name="sender">
		/// The source of the event. 
		/// </param>
		/// <param name="e">
		/// The <see cref="System.EventArgs"/> instance containing the event data. 
		/// </param>
		private void AccountingOperationsFormLoad(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				// Whether to disable the closeout buttons.
				this.GetCloseoutDisableFlag();

				this.accountingSite =
					FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
						x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

				// Initialize Collection 
                this.allProductsIrdoCollection = new ProductIrdoCollectionClass();

				if (this.Page.IsPostBack == false)
				{
					// Initialze the Manager control
					List<CompanyClass> companycollection =
						FMChannelHelper.MakeCall<ICompanies, List<CompanyClass>>(
							x => x.EnumerateByRole(this.security, COMPANY_ROLE.MANAGER, true, true));

					if (companycollection.Count == 1)
					{
						this.managerTextBox.Text = companycollection[0].ID;
                        this.managerTextBox2.Text = companycollection[0].ID;
					}

				    this.productTextBox.Text = "{All}";
				}

				// enable closeout button on page load
				if (this.closeoutDisableFlag == false)
				{
					this.CloseoutAllButton.Enabled = this.security.HasRight(RIGHT.PERFORM_CLOSEOUT);
				}
				else
				{
					this.CloseoutAllButton.Enabled = false;
				}

				this.StartDateText = this.GetTranslatedText(this.StartDateText);
				this.StopDateText = this.GetTranslatedText(this.StopDateText);
				this.SpecifyStartDate = this.GetTranslatedText(this.SpecifyStartDate);
				this.Complete = this.GetTranslatedText(this.Complete);
				this.NumberOfTransactions = this.GetTranslatedText(this.NumberOfTransactions);
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		/// This method will get the flag that makes the closeout buttons always disabled from the
		/// configuration settings in the database.
		/// </summary>
		private void GetCloseoutDisableFlag()
		{
			this.closeoutDisableFlag = false;

			string disableFlagStr = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_ForceCloseoutButtonDisable));

			if (string.IsNullOrEmpty(disableFlagStr) == false && disableFlagStr.ToUpper().Equals("TRUE"))
			{
				this.closeoutDisableFlag = true;
			}
		}

		/// <summary>
		/// Handles the Click event of the CloseoutAllButton control.
		/// </summary>
		/// <param name="sender">
		/// The source of the event. 
		/// </param>
		/// <param name="e">
		/// The <see cref="System.EventArgs"/> instance containing the event data. 
		/// </param>
		private void CloseoutAllButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (this.ValidateCloseoutDateControl() && this.ValidateManagerControl())
				{
                    var sr = new CloseoutSR
                    {
	                    Site = this.security.SiteID,
	                    CurrentSiteGuid = this.security.SiteGuid,
	                    ManagerCompanyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
											companies => companies.GetMasterRecordGuid(this.security, this.managerTextBox.Text)),
	                    ManagerName = this.managerTextBox.Text,
	                    Security = this.security,
	                    InventoryDate = this.CloseoutDate.CurrentValue.Date,
	                    CloseoutCommand = CloseoutSR.CloseoutType.CLOSEOUT_ALL_PRODUCTS_BY_STEPS_STEP1
                    };

					CloseoutDO closeoutResponse = FMChannelHelper.MakeCall<ICloseoutProcessor, CloseoutDO>(x => 
                    {
                        // Allow 15 minutes to complete closeout pre-processing.
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        ((System.ServiceModel.IClientChannel)x).OperationTimeout = new TimeSpan(0, 15, 0);
                        return x.Process(sr);
                    });

                    if (closeoutResponse.Closeouterror)
                    {
                        this.DisplayErrorDialog(closeoutResponse.Closeouterrtext);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(closeoutResponse.Confirmtext))
                        {
                            // If all products are closed out then display a message to the user
                            throw new Exception("All products are closed out for the date and manager selected");
                        }

                        // add product collection to session
                        this.Session["AllProductsIRDOCollection"] = closeoutResponse.Nonclosedproductsirdocollection;

                        this.Page.ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "ConfirmProcessScript_Key",
                            ConfirmProcessScript1 + "\"" + HttpUtility.JavaScriptStringEncode(closeoutResponse.Confirmtext) + "\"" + ConfirmProcessScript2);
                    }
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///   Required method for Designer support - do not modify
		///   the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CloseoutAllButton.Click += this.CloseoutAllButtonClick;
			this.Load += this.AccountingOperationsFormLoad;
		}

		/// <summary>
		/// This method will validate that the closeout date is valid.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool ValidateCloseoutDateControl()
		{
			// Check for an empty date
			if (string.IsNullOrEmpty(this.CloseoutDate.Text))
			{
				this.DisplayErrorDialog("Cannot have an empty date.");
				return false;
			}

			// Make sure the date provided is valid
            try
            {
                DateTime localDate;
                localDate = this.CloseoutDate.CurrentValue.Date;
            }
            catch (System.FormatException)
            {
                this.DisplayErrorDialog(string.Format("{0} is not a valid date.", this.CloseoutDate.Text.ToString(CultureInfo.InvariantCulture)));
                return false;
            }
            return true;
		}

		/// <summary>
		/// Validates the manager control.
		/// </summary>
		/// <returns>True if the manager selection if valid.</returns>
		private bool ValidateManagerControl()
		{
			bool breturn = false;

			try
			{
				// Check for an empty date
				if (0 == this.managerTextBox.Text.Length)
				{
					this.DisplayErrorDialog("Must select a manager.");
				}
				else
				{
					breturn = true;
				}
			}
			catch (FormatException fe)
			{
				this.DisplayErrorDialog(fe.Message);
			}

			return breturn;
		}

		#endregion

	}
}