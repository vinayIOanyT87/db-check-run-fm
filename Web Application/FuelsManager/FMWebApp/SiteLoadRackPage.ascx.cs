// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteLoadRackPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SiteLoadRackPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Security;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FMControls;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	///    Summary description for SiteLoadRackPage.
	/// </summary>
	public partial class SiteLoadRackPage : FMUserControlBase
	{
		#region Constants and Fields

		protected Label DriverTimeOutPeriodUnitsLabel;

		protected FMLabel DriverTimeoutPeriodUnitsLabel;

		#endregion

		#region Public Methods and Operators

		public void UpdateData()
		{
			var site = (SiteClass)this.Session["Site"];

			site.InhibitAccessAfterHours = this.InhibitAccessAfterHoursCheckBox.Checked;
			site.InhibitMultipleCardIns = this.InhibitMultipleCardInsCheckBox.Checked;
			site.AccessCardInRequired = this.AccessCardinRequiredCheckBox.Checked;
			site.CheckSiteNumber = this.CheckSiteNumberCheckBox.Checked;
			site.PromptForCustomerCard = this.PromptForCustomerCardCheckBox.Checked;
			site.PromptForTractorOrTanker = this.PromptForTractorOrTankerCheckBox.Checked;
			site.PromptForFirstTrailer = this.PromptForFirstTrailerCheckBox.Checked;
			site.PromptForSecondTrailer = this.PromptForSecondTrailerCheckBox.Checked;
            site.PromptForThirdTrailer = this.PromptForThirdTrailerCheckBox.Checked;
			site.PromptForCompartment = this.PromptForCompartmentCheckBox.Checked;
            site.PromptForTransactionCompletion = this.PromptForTransactionCompletionCheckBox.Checked;
			site.EnforceDriverEquipmentMatch = this.EnforceDriverEquipmentMatchCheckBox.Checked;
			site.UseCompanyEquipmentIdentifiers = this.UseCompanyEquipmentIdentifiersCheckBox.Checked;
			site.LoadByNet = this.LoadByNetCheckBox.Checked;
			site.PromptForShipmentNumber = this.PromptForShipmentNumberCheckBox.Checked;
			site.ListEquipment = this.ListEquipmentCheckBox.Checked;
			site.PromptForReturns = this.PromptForReturnCheckBox.Checked;
			site.PromptForTruckCard = this.PromptForTruckCardCheckBox.Checked;
			site.UseShortCardNumber = this.UseShortCardNumberCheckBox.Checked;
			site.MaximumLoadAmount = this.MaximumLoadAmountTextBox.Text;
			site.MaximumLoadTime = this.MaximumLoadTimeTextBox.Text;
			site.MaximumIdleTime = this.MaximumIdleTimeTextBox.Text;
			site.MaximumFlushAmount = this.MaximumFlushAmountTextBox.Text;
			site.MaximumMeterProvingAmount = this.MaximumMeterProvingAmountTextBox.Text;
			site.MaximumReturnsAmount = this.MaximumReturnsAmountTextBox.Text;
			site.MaximumNumberOfActiveArms = this.MaximumNumberOfActiveArmsTextBox.Text;
			site.DriverTimeoutPeriod = this.DriverTimeoutPeriodTextBox.Text;
			site.DriverWarningPeriod = this.DriverWarningPeriodTextBox.Text;
			site.MaximumPrompts = this.MaximumPromptsTextBox.Text;
			site.StartingShortCardNumber = this.StartingShortCardNumberTextBox.Text;
			site.MaximumVehicleWeight = this.MaximumVehicleWeightTextBox.Text;
            site.InhibitCustomerConfirmationPrompt = this.InhibitCustomerConfirmationPromptCheckBox.Checked;
            site.RequireTrailerScully = this.RequireTrailerScullyCheckBox.Checked;
            site.CardInTimeout = this.CardInTimeoutTextBox.Text;
            site.InhibitLoadRackCardIns = this.InhibitLoadOffLoadMultipleCardIns.Checked;
			site.EnforceSalesOrderLimit = this.EnforceSalesOrderLimit.Checked;

			site.ExcessVarianceCount = this.VarianceCountTextBox.Text;
			site.ExcessVarianceTolerance = this.VarianceTolaranceTextBox.Text;
			site.SecondaryStorageFillMethod = (SiteClass.FILL_METHOD)Convert.ToInt32(this.FillMethodDropDownList.SelectedValue);
			site.MaximumProductTemperature = this.MaximumProductTemperatureTextBox.Text;

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

		[SecurityCritical]
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var site = (SiteClass)this.Session["Site"];

				var localsecurity = new SecurityClass
			                            {
			                                UserID = this.Security.UserID,
			                                UserGuid = this.Security.UserGuid,
			                                Password = this.Security.Password,
			                                Token = this.Security.Token,
			                                SiteID = this.Security.SiteID,
			                                SiteGuid = this.Security.SiteGuid,
			                                LoginSiteID = this.Security.LoginSiteID,
			                                LoginSiteGuid = this.Security.LoginSiteGuid
			                            };

			    // copy relevant memeber into local security
                localsecurity.CloneRights(this.Security);

				if (!this.Page.IsPostBack)
				{
					this.InhibitAccessAfterHoursCheckBox.Checked = site.InhibitAccessAfterHours;
					this.InhibitMultipleCardInsCheckBox.Checked = site.InhibitMultipleCardIns;
					this.AccessCardinRequiredCheckBox.Checked = site.AccessCardInRequired;
					this.CheckSiteNumberCheckBox.Checked = site.CheckSiteNumber;
					this.PromptForCustomerCardCheckBox.Checked = site.PromptForCustomerCard;
					this.PromptForTractorOrTankerCheckBox.Checked = site.PromptForTractorOrTanker;
					this.PromptForFirstTrailerCheckBox.Checked = site.PromptForFirstTrailer;
					this.PromptForSecondTrailerCheckBox.Checked = site.PromptForSecondTrailer;
                    this.PromptForThirdTrailerCheckBox.Checked = site.PromptForThirdTrailer;
					this.PromptForCompartmentCheckBox.Checked = site.PromptForCompartment;
                    this.PromptForTransactionCompletionCheckBox.Checked = site.PromptForTransactionCompletion;
					this.EnforceDriverEquipmentMatchCheckBox.Checked = site.EnforceDriverEquipmentMatch;
					this.UseCompanyEquipmentIdentifiersCheckBox.Checked = site.UseCompanyEquipmentIdentifiers;
					this.LoadByNetCheckBox.Checked = site.LoadByNet;
					this.PromptForShipmentNumberCheckBox.Checked = site.PromptForShipmentNumber;
					this.ListEquipmentCheckBox.Checked = site.ListEquipment;
					this.PromptForReturnCheckBox.Checked = site.PromptForReturns;
					this.PromptForTruckCardCheckBox.Checked = site.PromptForTruckCard;
					this.UseShortCardNumberCheckBox.Checked = site.UseShortCardNumber;
					this.MaximumLoadTimeTextBox.Text = site.MaximumLoadTime;
					this.MaximumIdleTimeTextBox.Text = site.MaximumIdleTime;
					this.MaximumNumberOfActiveArmsTextBox.Text = site.MaximumNumberOfActiveArms;
					this.DriverTimeoutPeriodTextBox.Text = site.DriverTimeoutPeriod;
					this.DriverWarningPeriodTextBox.Text = site.DriverWarningPeriod;
					this.MaximumPromptsTextBox.Text = site.MaximumPrompts;
					this.StartingShortCardNumberTextBox.Text = site.StartingShortCardNumber;
                    this.InhibitCustomerConfirmationPromptCheckBox.Checked = site.InhibitCustomerConfirmationPrompt;
                    this.RequireTrailerScullyCheckBox.Checked = site.RequireTrailerScully;
                    this.CardInTimeoutTextBox.Text = site.CardInTimeout;
                    this.InhibitLoadOffLoadMultipleCardIns.Checked = site.InhibitLoadRackCardIns;
					this.EnforceSalesOrderLimit.Checked = site.EnforceSalesOrderLimit;
					this.VarianceCountTextBox.Text = site.ExcessVarianceCount;
					this.VarianceTolaranceTextBox.Text = site.ExcessVarianceTolerance;
					this.FillMethodDropDownList.Items.Add(
						new ListItem(SiteClass.FillMethodID(SiteClass.FILL_METHOD.ACTUAL), ((int)SiteClass.FILL_METHOD.ACTUAL).ToString()));
					this.FillMethodDropDownList.Items.Add(
						new ListItem(
							SiteClass.FillMethodID(SiteClass.FILL_METHOD.SAFEFILL), ((int)SiteClass.FILL_METHOD.SAFEFILL).ToString()));
					this.FillMethodDropDownList.SelectedValue = ((int)site.SecondaryStorageFillMethod).ToString();
				}

					// Must UpdateData every post back
				else
				{
					this.UpdateData();
				}

				//if(localsecurity.LoginSiteIndex == Site.Index) //sjiang: there is no need to verify
				{
					site._MaximumLoadAmount.Format.NumberDecimalDigits = site._VolumeDecimalPlaces;
					site._MaximumFlushAmount.Format.NumberDecimalDigits = site._VolumeDecimalPlaces;
					site._MaximumMeterProvingAmount.Format.NumberDecimalDigits = site._VolumeDecimalPlaces;
					site._MaximumReturnsAmount.Format.NumberDecimalDigits = site._VolumeDecimalPlaces;
					site._MaximumVehicleWeight.Format.NumberDecimalDigits = site._MassDecimalPlaces;
					site._MaximumProductTemperature.Format.NumberDecimalDigits = site._TemperatureDecimalPlaces;

					site._MaximumLoadAmount.Units = site.VolumeUnits;
					site._MaximumFlushAmount.Units = site.VolumeUnits;
					site._MaximumMeterProvingAmount.Units = site.VolumeUnits;
					site._MaximumReturnsAmount.Units = site.VolumeUnits;
					site._MaximumVehicleWeight.Units = site.MassUnits;
					site._MaximumProductTemperature.Units = site.TemperatureUnits;
				}

				this.MaximumLoadAmountTextBox.Text = site.MaximumLoadAmount;
				this.MaximumFlushAmountTextBox.Text = site.MaximumFlushAmount;
				this.MaximumMeterProvingAmountTextBox.Text = site.MaximumMeterProvingAmount;
				this.MaximumReturnsAmountTextBox.Text = site.MaximumReturnsAmount;
				this.MaximumVehicleWeightTextBox.Text = site.MaximumVehicleWeight;
				this.MaximumProductTemperatureTextBox.Text = site.MaximumProductTemperature;

				string abbrevString = EngineeringUnits.GetUnitAbbreviation(site._MaximumReturnsAmount.Units);
				this.MaxLoadAmountUnitsLabel.Text = abbrevString;
				this.MaxFlushAmountUnitsLabel.Text = abbrevString;
				this.MaxMeterProvingAmountUnitsLabel.Text = abbrevString;
				this.MaxReturnsAmountUnitsLabel.Text = abbrevString;
				abbrevString = EngineeringUnits.GetUnitAbbreviation(site._MaximumVehicleWeight.Units);
				this.MaxVehicleWeightUnitsLabel.Text = abbrevString;
				abbrevString = EngineeringUnits.GetUnitAbbreviation(site._MaximumProductTemperature.Units);
				this.MaxProductTempUnitsLabel.Text = abbrevString;
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
		}

		#endregion
	}
}