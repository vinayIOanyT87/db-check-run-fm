/******************************************************************************

	FILE NAME:		ProductVolumeCorrectionPage.ascx.cs


	PURPOSE:			Implementation of ProductVolumeCorrectionPage


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		05/15/2007	W.Gray		7.1.0.1 - Changed to display LoginSite.DensityUnits
										and LoginSite.TemperatureUnits (CSI 4752)

		08/06/2007	W.Gray		7.1.1.2 - Changed to correct CSI 5027
 
		09/02/2009  A.Coker	WI 6323 - Replaced setting of Standard Temperature Text Box
										ReadOnly property with setting of Enable property. This will
										prevent taking focus and user from mistaking that the field is accepting values
										when actually is not.
	
														

*******************************************************************************/
using System;
using System.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FuelsManager.FMWebApp
{
	/// <summary>
	/// Summary description for ProductVolumeCorrectionForm.
	/// </summary>
	public partial class ProductVolumeCorrectionPage : ProductPageBase
	{
		
		private SiteClass CurrentSite{ get{ return ((ProductForm) Page).CurrentSite;}}

	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (! Page.IsPostBack) 
				{
					UpdateView();
					SetFieldAccessibilityForChildRecordVersion();
				}
			}	
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{	

		}
		#endregion

		public void UpdateData()
		{
			try
			{
				this.Product._VcfModuleSettings.ForceVcfTo4Digits = this.ForceVcfTo4DigitsYesRadioButton.Checked;
				this.Product._VcfModuleSettings.UseHydrometerCorrection = this.UseHydrometerCorrectionYesRadioButton.Checked;
				this.Product._VcfModuleSettings.UseProductObservedDensity = this.UseProductObservedDensityYesRadioButton.Checked;
				this.Product._VcfModuleSettings.K[0] = Convert.ToDouble(K0TextBox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.Product._VcfModuleSettings.K[1] = Convert.ToDouble(K1TextBox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.Product._VcfModuleSettings.K[2] = Convert.ToDouble(K2TextBox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.Product._VcfModuleSettings.K[3] = Convert.ToDouble(K3TextBox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.Product._VcfModuleSettings.K[4] = Convert.ToDouble(K4TextBox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.Product._VcfModuleSettings.Alpha = Convert.ToDouble(AlphaTextBox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));

				if (this.Product.StandardDensity != StandardDensityTextbox.Text)
				{
					this.Product.StandardDensity = StandardDensityTextbox.Text;
				}

				var temperatureFormatInfo = CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE);
				temperatureFormatInfo.NumberDecimalDigits = Product.TemperatureDecimalPlaces;

				this.Product._VcfModuleSettings.BaseTemperature.Value = Convert.ToDouble(StandardTemperatureTextbox.Text, temperatureFormatInfo);
				this.Product._VcfModuleSettings.AlternateTemperature.Value = Convert.ToDouble(AlternateTemperatureTextbox.Text, temperatureFormatInfo);

				var pressureFormatInfo = CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.PRESSURE);
				pressureFormatInfo.NumberDecimalDigits = Product.PressureDecimalPlaces;

				this.Product._VcfModuleSettings.AlternateBasePressure.Value = Convert.ToDouble(AlternatePressureTextbox.Text, pressureFormatInfo);

				this.Product._VcfModuleSettings.CorrectionMethodType = VcfModuleSettings.GetCorrectionTypeMajor(
					this.StandardsOrganizationDropDownList.SelectedValue
					, this.StandardRevisionDropDownList.SelectedValue
					, this.TemperatureStandardDropdownlist.SelectedValue);

				this.Product._VcfModuleSettings.CorrectionMethodSpecific = VcfModuleSettings.GetCorrectionTypeMinor(
					this.StandardsOrganizationDropDownList.SelectedValue
					, this.StandardRevisionDropDownList.SelectedValue
					, this.CommodityTableDropdownlist.SelectedValue
					, this.TemperatureStandardDropdownlist.SelectedValue);

				this.Product.ApplyStandardDensity = ApplyStandardDensityCheckBox.Checked;
				this.Product.ApplyVolumeCorrection = ApplyVolumeCorrectionCheckBox.Checked;

			}
			catch (Exception e)
			{
				string msg = "Product Volume Correction Page - " + e.Message;
				Exception e1 = new Exception(msg);
				//		ErrorHandler(e1);
			}
		}
		
		protected void StandardsOrganizationDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			var standardRevision = this.StandardRevisionDropDownList.SelectedValue;
			this.StandardRevisionDropDownList.Items.Clear();
			foreach (var Item in VcfModuleSettings.GetStandardsAndRevisions(StandardsOrganizationDropDownList.SelectedValue))
			{
				this.StandardRevisionDropDownList.Items.Add(new ListItem(Item.Key, Item.Value));
				if(Item.Value == standardRevision)
				{
					this.StandardRevisionDropDownList.SelectedValue = standardRevision;
				}
			}

			if (!Page.IsPostBack)
			{
				this.StandardRevisionDropDownList.SelectedValue = VcfModuleSettings.GetStandardRevision(Product._VcfModuleSettings.CorrectionMethodType, Product._VcfModuleSettings.CorrectionMethodSpecific);
			}

			this.StandardRevisionDropDownList_SelectedIndexChanged(null,null);
		}

		[SecurityCritical]
		protected void StandardRevisionDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			var commodityTable = this.CommodityTableDropdownlist.SelectedValue;
			this.CommodityTableDropdownlist.Items.Clear();
			foreach (var Item in VcfModuleSettings.GetCommoditiesOrTables(this.StandardsOrganizationDropDownList.SelectedValue, this.StandardRevisionDropDownList.SelectedValue))
			{
				this.CommodityTableDropdownlist.Items.Add(new ListItem(Item.Key, Item.Value));
				if(commodityTable == Item.Value)
				{
					this.CommodityTableDropdownlist.SelectedValue = commodityTable;
				}
			}

			if (!Page.IsPostBack)
			{
				this.CommodityTableDropdownlist.SelectedValue = VcfModuleSettings.GetCommodityOrTable(Product._VcfModuleSettings.CorrectionMethodSpecific);
			}

			this.CommodityTableDropDownList_SelectedIndexChanged(null, null);

			if (this.StandardRevisionDropDownList.SelectedValue == "Commodity (2004)")
			{
				this.AlternateTemperatureTextbox.ReadOnly = false;
				this.AlternatePressureTextbox.ReadOnly = false;
			}
			else
			{
				this.AlternateTemperatureTextbox.ReadOnly = true;
				this.AlternatePressureTextbox.ReadOnly = true;

				double zero = 0.0;

				var temperatureFormatInfo = CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE);
				temperatureFormatInfo.NumberDecimalDigits = this.Product.TemperatureDecimalPlaces;

				this.AlternateTemperatureTextbox.Text = zero.ToString("N", temperatureFormatInfo);

				var pressureFormatInfo = CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.PRESSURE);
				pressureFormatInfo.NumberDecimalDigits = this.Product.PressureDecimalPlaces;

				this.AlternatePressureTextbox.Text = zero.ToString("N", pressureFormatInfo);
			}
		}

		protected void CommodityTableDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			var temperatureStandard = this.TemperatureStandardDropdownlist.SelectedValue;
			this.TemperatureStandardDropdownlist.Items.Clear();
			foreach (var Item in VcfModuleSettings.GetStandardTemperatures(this.StandardsOrganizationDropDownList. SelectedValue, this.StandardRevisionDropDownList.SelectedValue, this.CommodityTableDropdownlist.SelectedValue))
			{
				this.TemperatureStandardDropdownlist.Items.Add(new ListItem(Item.Key, Item.Value));
				if(Item.Value == temperatureStandard)
				{
					this.TemperatureStandardDropdownlist.SelectedValue = temperatureStandard;
				}
			}

			if (!Page.IsPostBack)
			{
				var standardTemperature = VcfModuleSettings.GetStandardTemperature(Product._VcfModuleSettings.CorrectionMethodType, Product._VcfModuleSettings.CorrectionMethodSpecific);
				if(string.IsNullOrEmpty(standardTemperature))
				{
					if (Product._VcfModuleSettings.BaseTemperature.Value == 60)
					{
						standardTemperature = "60 °F";
					}
					else
					{
						standardTemperature = "15 °C";
					}
				}

				this.TemperatureStandardDropdownlist.SelectedValue = standardTemperature;
			}

			TemperatureStandardDropDownList_SelectedIndexChanged(null, null);

			bool readonlyKFactors;

			if (this.CommodityTableDropdownlist.SelectedValue == "K-Factors")
			{
				readonlyKFactors = false;
			}
			else
			{
				readonlyKFactors = true;
			}

			this.K0TextBox.ReadOnly = readonlyKFactors;
			this.K1TextBox.ReadOnly = readonlyKFactors;
			this.K2TextBox.ReadOnly = readonlyKFactors;
			this.K3TextBox.ReadOnly = readonlyKFactors;
			this.K4TextBox.ReadOnly = readonlyKFactors;

			if(readonlyKFactors)
			{
				this.K0TextBox.Text = "0";
				this.K1TextBox.Text = "0";
				this.K2TextBox.Text = "0";
				this.K3TextBox.Text = "0";
				this.K4TextBox.Text = "0";
			}

			if (this.CommodityTableDropdownlist.SelectedValue == "Alpha 60 Supplied"
			|| this.CommodityTableDropdownlist.SelectedValue == "6C"
			|| this.CommodityTableDropdownlist.SelectedValue == "54C")
			{
				this.AlphaTextBox.ReadOnly = false; 
			}
			else
			{
				this.AlphaTextBox.ReadOnly = true;
				this.AlphaTextBox.Text = "0";
			}

		}

		protected void TemperatureStandardDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			var standardTemp = this.TemperatureStandardDropdownlist.SelectedValue;
			if (string.IsNullOrEmpty(standardTemp))
			{
				standardTemp = "60 °F";
			}

			var standardTempDisabled = true;
			var temperatureUnits = standardTemp;
			if (standardTemp.Length > 2)
			{
				var res = standardTemp.Split(new char[] {' '});
				standardTemp = res[0];
				temperatureUnits = res[1];
			}
			else
			{
				standardTempDisabled = false;

				if (this.Product._VcfModuleSettings.BaseTemperature.Value != 0)
				{
					var tempFormatInfo = CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE);
					standardTemp = this.Product._VcfModuleSettings.BaseTemperature.Value.ToString("N", tempFormatInfo);
				}
				else
				{
					if (temperatureUnits == "°C")
					{
						standardTemp = "15";
					}
					else
					{
						standardTemp = "60";
					}
				}
			}

			var temperatureFormatInfo = CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE);
			temperatureFormatInfo.NumberDecimalDigits = this.Product.TemperatureDecimalPlaces;

			this.StandardTemperatureTextbox.Text = Convert.ToDouble(standardTemp, temperatureFormatInfo).ToString("N", temperatureFormatInfo);
			this.StandardTemperatureUnitsLabel.Text = temperatureUnits;
			this.StandardTemperatureTextbox.ReadOnly = standardTempDisabled;
			this.AlternateTemperatureUnitsLabel.Text = temperatureUnits;
		}


		public void ValidateDataToUI()
		{
			string msg = "";
			try
			{
				SIDouble d = new SIDouble();
				d.Units = Product.DensityUnits;
				msg = "Product Volume Correction Page - Standard Density : ";
				d.SIValue = Product._StandardDensity.SIValue;
				double val = d.Value;

				d.Units = Product.TemperatureUnits;
				msg = "Product Volume Correction Page - Standard Temperature : ";
				d.SIValue = Product._VcfModuleSettings.BaseTemperature.Value;
				val = d.Value;

				msg = "Product Volume Correction Page - Alternate Temperature : ";
				d.SIValue = Product._VcfModuleSettings.AlternateTemperature.Value;
				val = d.Value;

				d.Units = Product.PressureUnits;
				msg = "Product Volume Correction Page - Alternate Pressure : ";
				d.SIValue = Product._VcfModuleSettings.AlternateBasePressure.Value;
				val = d.Value;
			}
			catch (Exception e)
			{
				throw new Exception(msg + e.Message);
			}
		}

		public void ValidateDataFromUI()
		{
			string msg = "";
			try
			{
				SIDouble d = new SIDouble();
                if (StandardDensityTextbox.Text.Length > 0)
                {
                    d.Units = Product.DensityUnits;
                    msg = "Product Volume Correction Page - Standard Density : ";
                    d.Value = Double.Parse(StandardDensityTextbox.Text);
                }

                if (StandardTemperatureTextbox.Text.Length > 0)
                {
                    d.Units = Product.TemperatureUnits;
				msg = "Product Volume Correction Page - Standard Temperature : ";
				d.Value = Double.Parse(StandardTemperatureTextbox.Text);
                }

                if (AlternateTemperatureTextbox.Text.Length > 0)
                {
                    msg = "Product Volume Correction Page - Alternate Temperature : ";
				    d.Value = Double.Parse(AlternateTemperatureTextbox.Text);
                }

                if (AlternatePressureTextbox.Text.Length > 0)
                {
                    d.Units = Product.PressureUnits;
				    msg = "Product Volume Correction Page - Alternate Pressure : ";
				    d.Value = Double.Parse(AlternatePressureTextbox.Text);
                }
            }
            catch (Exception e)
			{
				throw new Exception(msg + e.Message);
			}

		}

		public void UpdateView()
		{
			try
			{
				var standardsOrganization = this.StandardsOrganizationDropDownList.SelectedValue;
				this.StandardsOrganizationDropDownList.Items.Clear();
				foreach (var Item in VcfModuleSettings.GetStandardsOrganizations())
				{
					this.StandardsOrganizationDropDownList.Items.Add(new ListItem(Item.Key, Item.Value));
					if(Item.Value == standardsOrganization)
					{
						this.StandardsOrganizationDropDownList.SelectedValue = standardsOrganization;
					}
				}

				if (!Page.IsPostBack)
				{
					this.StandardsOrganizationDropDownList.SelectedValue = VcfModuleSettings.GetStandardsOrganization(Product._VcfModuleSettings.CorrectionMethodType);
				}

				this.StandardsOrganizationDropDownList_SelectedIndexChanged(null, null);

				this.ForceVcfTo4DigitsYesRadioButton.Checked = this.Product._VcfModuleSettings.ForceVcfTo4Digits;
				this.UseHydrometerCorrectionYesRadioButton.Checked = this.Product._VcfModuleSettings.UseHydrometerCorrection;
				this.UseProductObservedDensityYesRadioButton.Checked = this.Product._VcfModuleSettings.UseProductObservedDensity;

				this.ForceVcfTo4DigitsNoRadioButton.Checked = !this.Product._VcfModuleSettings.ForceVcfTo4Digits;
				this.UseHydrometerCorrectionNoRadioButton.Checked = !this.Product._VcfModuleSettings.UseHydrometerCorrection;
				this.UseProductObservedDensityNoRadioButton.Checked = !this.Product._VcfModuleSettings.UseProductObservedDensity;

				this.K0TextBox.Text = Product._VcfModuleSettings.K[0].ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.K1TextBox.Text = Product._VcfModuleSettings.K[1].ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.K2TextBox.Text = Product._VcfModuleSettings.K[2].ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.K3TextBox.Text = Product._VcfModuleSettings.K[3].ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.K4TextBox.Text = Product._VcfModuleSettings.K[4].ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.AlphaTextBox.Text = Product._VcfModuleSettings.Alpha.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));

				this.ApplyStandardDensityCheckBox.Checked = Product.ApplyStandardDensity;
				this.ApplyVolumeCorrectionCheckBox.Checked = Product.ApplyVolumeCorrection;

				UnitsHelperClass unitsHelper = new UnitsHelperClass(Security, CurrentSite, null, Product);
				string AbbrevString = EngineeringUnits.GetUnitAbbreviation(unitsHelper.PressureUnits);
				this.AlternatePressureUnitsLabel.Text = AbbrevString;

				AbbrevString = EngineeringUnits.GetUnitAbbreviation(unitsHelper.DensityUnits);
				StandardDensityUnitsLabel.Text = AbbrevString;

				var temperatureFormatInfo = CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE);
				temperatureFormatInfo.NumberDecimalDigits = this.Product.TemperatureDecimalPlaces;

				this.StandardTemperatureTextbox.Text = this.Product._VcfModuleSettings.BaseTemperature.Value.ToString("N", temperatureFormatInfo);
				this.AlternateTemperatureTextbox.Text = this.Product._VcfModuleSettings.AlternateTemperature.Value.ToString("N",temperatureFormatInfo);

				var pressureFormatInfo = CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.PRESSURE);
				pressureFormatInfo.NumberDecimalDigits = this.Product.PressureDecimalPlaces;

				this.AlternatePressureTextbox.Text = this.Product._VcfModuleSettings.AlternateBasePressure.Value.ToString("N", pressureFormatInfo);

				this.UpdateSDensityFromPackageSize();
			}
			catch (Exception e)
			{
				string msg = "Product Volume Correction Page - " + e.Message;
				throw new Exception(msg);
			}
		}

		[SecurityCritical]
		public void UpdateSDensityFromPackageSize()
		{
			if (Product._MassPackageSize.SIValue != 0.0 && Product._VolumePackageSize.SIValue != 0.0)
			{
				Product.StandardDensity = (Math.Round(Product._MassPackageSize.SIValue / Product._VolumePackageSize.SIValue, Product.DensityDecimalPlaces, MidpointRounding.AwayFromZero)).ToString();
				StandardDensityTextbox.Enabled = false;
			}

			StandardDensityTextbox.Text = Product.StandardDensity;
		}



		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
			if ((this.Product.IdentityGuid.Equals(Guid.Empty)
				|| (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))
				|| (this.VersionSpecificFields == null)))
			{
				return;
			}
			this.StandardsOrganizationDropDownList.Enabled = (this.StandardsOrganizationDropDownList.Enabled 
                                            && this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.StandardRevisionDropDownList.Enabled = (this.StandardRevisionDropDownList.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.CommodityTableDropdownlist.Enabled = (this.CommodityTableDropdownlist.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.TemperatureStandardDropdownlist.Enabled = (this.TemperatureStandardDropdownlist.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.ForceVcfTo4DigitsYesRadioButton.Enabled = (this.ForceVcfTo4DigitsYesRadioButton.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.UseHydrometerCorrectionYesRadioButton.Enabled = (this.UseHydrometerCorrectionYesRadioButton.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.UseProductObservedDensityYesRadioButton.Enabled = (this.UseProductObservedDensityYesRadioButton.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.ForceVcfTo4DigitsNoRadioButton.Enabled = (this.ForceVcfTo4DigitsNoRadioButton.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.UseHydrometerCorrectionNoRadioButton.Enabled = (this.UseHydrometerCorrectionNoRadioButton.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
            this.UseProductObservedDensityNoRadioButton.Enabled = (this.UseProductObservedDensityNoRadioButton.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.K0TextBox.Enabled = (this.K0TextBox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.K1TextBox.Enabled = (this.K1TextBox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.K2TextBox.Enabled = (this.K2TextBox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.K3TextBox.Enabled = (this.K3TextBox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.K4TextBox.Enabled = (this.K4TextBox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.AlphaTextBox.Enabled = (this.AlphaTextBox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.StandardDensityTextbox.Enabled = (this.StandardDensityTextbox.Enabled 
											&& this.VersionSpecificFields.Contains("StandardDensity"));
			this.StandardTemperatureTextbox.Enabled = (this.StandardTemperatureTextbox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.AlternateTemperatureTextbox.Enabled = (this.AlternateTemperatureTextbox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.AlternatePressureTextbox.Enabled = (this.AlternatePressureTextbox.Enabled 
											&& this.VersionSpecificFields.Contains("VcfModuleSettings"));
			this.ApplyStandardDensityCheckBox.Enabled = (this.AlternatePressureTextbox.Enabled 
											&& this.VersionSpecificFields.Contains("ApplyStandardDensity"));
			this.ApplyVolumeCorrectionCheckBox.Enabled = (this.AlternatePressureTextbox.Enabled 
											&& this.VersionSpecificFields.Contains("ApplyVolumeCorrection"));

		}
	}
}
