namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Security;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	///     Summary description for ProductUnitsPage.
	/// </summary>
	public partial class ProductUnitsPage : ProductPageBase
	{
		#region Constants and Fields
		protected SiteClass CurrentSite;
		#endregion

		#region Public Methods and Operators

		public void UpdateData()
		{
			this.Product.VolumeUnits = (EngineeringUnit)Convert.ToInt32(this.VolumeUnitsDropDownList.SelectedValue);
			this.Product.VolumeDecimalPlaces =
				Byte.Parse(
					(string.IsNullOrEmpty(this.VolumeDecimalPlacesTextbox.Text) ? "0" : this.VolumeDecimalPlacesTextbox.Text));
			this.Product.TemperatureUnits = (EngineeringUnit)Convert.ToInt32(this.TemperatureUnitsDropDownList.SelectedValue);
			this.Product.TemperatureDecimalPlaces =
				Byte.Parse(
					(string.IsNullOrEmpty(this.TemperatureDecimalPlacesTextbox.Text) ? "0" : this.TemperatureDecimalPlacesTextbox.Text));
			this.Product.DensityUnits = (EngineeringUnit)Convert.ToInt32(this.DensityUnitsDropDownList.SelectedValue);
			this.Product.DensityDecimalPlaces =
				Byte.Parse(
					(string.IsNullOrEmpty(this.DensityDecimalPlacesTextbox.Text) ? "0" : this.DensityDecimalPlacesTextbox.Text));
			this.Product.MassUnits = (EngineeringUnit)Convert.ToInt32(this.MassUnitsDropDownList.SelectedValue);
			this.Product.MassDecimalPlaces =
				Byte.Parse((string.IsNullOrEmpty(this.MassDecimalPlacesTextbox.Text) ? "0" : this.MassDecimalPlacesTextbox.Text));
			this.Product.LevelUnits = (EngineeringUnit)Convert.ToInt32(this.LevelUnitsDropDownList.SelectedValue);
			this.Product.LevelDecimalPlaces =
				Byte.Parse((string.IsNullOrEmpty(this.LevelDecimalPlacesTextbox.Text) ? "0" : this.LevelDecimalPlacesTextbox.Text));
			this.Product.FlowUnits = (EngineeringUnit)Convert.ToInt32(this.FlowUnitsDropDownList.SelectedValue);
			this.Product.FlowDecimalPlaces =
				Byte.Parse((string.IsNullOrEmpty(this.FlowDecimalPlacesTextbox.Text) ? "0" : this.FlowDecimalPlacesTextbox.Text));
			this.Product.PressureUnits = (EngineeringUnit)Convert.ToInt32(this.PressureUnitsDropDownList.SelectedValue);
			this.Product.PressureDecimalPlaces =
				Byte.Parse(
					(string.IsNullOrEmpty(this.PressureDecimalPlacesTextbox.Text) ? "0" : this.PressureDecimalPlacesTextbox.Text));
			this.Product.VolumePackageSize = string.IsNullOrEmpty(this.VolumePackageSizeTextbox.Text)
				? "0"
				: this.VolumePackageSizeTextbox.Text;
			this.Product.MassPackageSize = string.IsNullOrEmpty(this.MassPackageSizeTextbox.Text)
				? "0"
				: this.MassPackageSizeTextbox.Text;
		}
		#endregion

		#region Methods
		protected void MassDecimalPlacesTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Product.MassDecimalPlaces =
					Byte.Parse((string.IsNullOrEmpty(this.MassDecimalPlacesTextbox.Text) ? "0" : this.MassDecimalPlacesTextbox.Text));
				if (!string.IsNullOrEmpty(this.Product.MassPackageSize))
				{
					this.Product._MassPackageSize.Format.NumberDecimalDigits = this.Product.MassDecimalPlaces;
					this.MassPackageSizeTextbox.Text = this.Product.MassPackageSize;
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void MassPackageSizeTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.MassPackageSizeTextbox.Text == "")
				{
					this.Product.MassPackageSize = "0";
				}
				else
				{
					this.Product.MassPackageSize = this.MassPackageSizeTextbox.Text;
				}

				var volumeCorrectionPage =
					(ProductVolumeCorrectionPage)this.Page.FindControl("tcProductTabs")
							.FindControl("tpVolumeCorrectionPage")
							.FindControl("ProductVolumeCorrectionPage");
				volumeCorrectionPage.UpdateSDensityFromPackageSize();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.CurrentSite =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

				if (!this.Page.IsPostBack)
				{
					this.InitializeUnitsDropDownList(this.LevelUnitsDropDownList,
													EngineeringUnit.FmlFtIn8Th,
													EngineeringUnit.FmlMile,
													this.Product.LevelUnits);
					this.InitializeUnitsDropDownList(this.VolumeUnitsDropDownList,
													EngineeringUnit.FmvCm3,
													EngineeringUnit.FmvMsFt3,
													this.Product.VolumeUnits);
					this.InitializeUnitsDropDownList(this.TemperatureUnitsDropDownList,
													EngineeringUnit.FmtDegC,
													EngineeringUnit.FmtDegR,
													this.Product.TemperatureUnits);
					this.InitializeUnitsDropDownList(this.DensityUnitsDropDownList,
													EngineeringUnit.FmdGcm3,
													EngineeringUnit.FmdSTnYd3,
													this.Product.DensityUnits);
					this.InitializeUnitsDropDownList(this.MassUnitsDropDownList,
													EngineeringUnit.FmmGram,
													EngineeringUnit.FmmMlbs,
													this.Product.MassUnits);
					this.InitializeUnitsDropDownList(this.FlowUnitsDropDownList,
													EngineeringUnit.FmvfCcMin,
													EngineeringUnit.FmvfKlDay,
													this.Product.FlowUnits);
					this.InitializeUnitsDropDownList(this.PressureUnitsDropDownList,
													EngineeringUnit.FmpPa,
													EngineeringUnit.FmpAtm,
													this.Product.PressureUnits);

					this.LevelUnitsDropDownList.Items.Insert(0, new ListItem("{Site}", "0"));
					this.VolumeUnitsDropDownList.Items.Insert(0, new ListItem("{Site}", "0"));
					this.TemperatureUnitsDropDownList.Items.Insert(0, new ListItem("{Site}", "0"));
					this.DensityUnitsDropDownList.Items.Insert(0, new ListItem("{Site}", "0"));
					this.MassUnitsDropDownList.Items.Insert(0, new ListItem("{Site}", "0"));
					this.FlowUnitsDropDownList.Items.Insert(0, new ListItem("{Site}", "0"));
					this.PressureUnitsDropDownList.Items.Insert(0, new ListItem("{Site}", "0"));

					this.LevelDecimalPlacesTextbox.Text = this.Product.LevelDecimalPlaces.ToString();
					this.VolumeDecimalPlacesTextbox.Text = this.Product.VolumeDecimalPlaces.ToString();
					this.TemperatureDecimalPlacesTextbox.Text = this.Product.TemperatureDecimalPlaces.ToString();
					this.DensityDecimalPlacesTextbox.Text = this.Product.DensityDecimalPlaces.ToString();
					this.MassDecimalPlacesTextbox.Text = this.Product.MassDecimalPlaces.ToString();
					this.FlowDecimalPlacesTextbox.Text = this.Product.FlowDecimalPlaces.ToString();
					this.PressureDecimalPlacesTextbox.Text = this.Product.PressureDecimalPlaces.ToString();
					this.VolumePackageSizeTextbox.Text = this.Product.VolumePackageSize;
					this.MassPackageSizeTextbox.Text = this.Product.MassPackageSize;

					this.LevelUnitsDropDownListSelectedIndexChanged(null, null);
					this.VolumeUnitsDropDownListSelectedIndexChanged(null, null);
					this.TemperatureUnitsDropDownListSelectedIndexChanged(null, null);
					this.DensityUnitsDropDownListSelectedIndexChanged(null, null);
					this.MassUnitsDropDownListSelectedIndexChanged(null, null);
					this.FlowUnitsDropDownListSelectedIndexChanged(null, null);
					this.PressureUnitsDropDownListSelectedIndexChanged(null, null);
					this.VolumeDecimalPlacesTextChanged(null, null);
					this.MassDecimalPlacesTextChanged(null, null);
					this.VolumePackageSizeTextChanged(null, null);
					this.MassPackageSizeTextChanged(null, null);
					this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void VolumeDecimalPlacesTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Product.VolumeDecimalPlaces =
					Byte.Parse(
						(string.IsNullOrEmpty(this.VolumeDecimalPlacesTextbox.Text) ? "0" : this.VolumeDecimalPlacesTextbox.Text));
				
				if (!string.IsNullOrEmpty(this.Product.VolumePackageSize))
				{
					this.Product._VolumePackageSize.Format.NumberDecimalDigits = this.Product.VolumeDecimalPlaces;
					this.VolumePackageSizeTextbox.Text = this.Product.VolumePackageSize;
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void VolumePackageSizeTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.VolumePackageSizeTextbox.Text == string.Empty)
				{
					this.Product.VolumePackageSize = "0";
				}
				else
				{
					this.Product.VolumePackageSize = this.VolumePackageSizeTextbox.Text;
				}

				var volumeCorrectionPage =
					(ProductVolumeCorrectionPage)this.Page.FindControl("tcProductTabs")
							.FindControl("tpVolumeCorrectionPage")
							.FindControl("ProductVolumeCorrectionPage");

				volumeCorrectionPage.UpdateSDensityFromPackageSize();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		private void DensityUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			EngineeringUnit units = this.Product.DensityUnits;

			try
			{
				bool siteUnitsSelected = this.DensityUnitsDropDownList.SelectedIndex == 0;
				this.DensityDecimalPlacesTextbox.Enabled = !siteUnitsSelected;

				bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
                if (!((this.Product.IdentityGuid.Equals(Guid.Empty))
					  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
                      || (this.VersionSpecificFields == null))
                   )
				{
					this.DensityDecimalPlacesTextbox.Enabled = this.DensityDecimalPlacesTextbox.Enabled
					                                           && this.VersionSpecificFields.Contains("DensityDecimalPlaces");
				}

				// Change decimal places only on postback, which means user actually changed the dropdown selection
				if (this.Page.IsPostBack)
				{
					if (siteUnitsSelected)
					{
						this.DensityDecimalPlacesTextbox.Text = this.CurrentSite.DensityDecimalPlaces;
					}
					else
					{
						this.DensityDecimalPlacesTextbox.Text = this.Product.DensityDecimalPlaces.ToString();
					}
				}

				var volumeCorrectionPage =
					(ProductVolumeCorrectionPage)this.Page.FindControl("tcProductTabs")
							.FindControl("tpVolumeCorrectionPage")
							.FindControl("ProductVolumeCorrectionPage");

				var productAlarmsPage =
					(ProductAlarmsPage)this.Page.FindControl("tcProductTabs")
							.FindControl("tpAlarmsPage")
							.FindControl("ProductAlarmsPage");

				if (sender != null)
				{
					productAlarmsPage.ValidateDataFromUI();
					volumeCorrectionPage.ValidateDataFromUI();
					productAlarmsPage.UpdateData();
					volumeCorrectionPage.UpdateData();
				}

				this.Product.DensityUnits = (EngineeringUnit)Convert.ToInt32(this.DensityUnitsDropDownList.SelectedValue);

				productAlarmsPage.ValidateDataToUI();
				volumeCorrectionPage.ValidateDataFromUI();
				volumeCorrectionPage.UpdateView();
				productAlarmsPage.UpdateView();
			}
			catch (Exception ex)
			{
				if (this.Product.DensityUnits != units)
				{
					//Revert back
					this.Product.DensityUnits = units;
				}
				this.DensityUnitsDropDownList.SelectedValue = ((int)units).ToString(CultureInfo.InvariantCulture);
				this.ErrorHandler(ex);
			}
		}

		private void FlowUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			bool siteUnitsSelected = this.FlowUnitsDropDownList.SelectedIndex == 0;
			this.FlowDecimalPlacesTextbox.Enabled = !siteUnitsSelected;

			bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
            if (!((this.Product.IdentityGuid.Equals(Guid.Empty))
				  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
                  || (this.VersionSpecificFields == null))
               )
			{
				this.FlowDecimalPlacesTextbox.Enabled = this.FlowDecimalPlacesTextbox.Enabled
				                                        && this.VersionSpecificFields.Contains("FlowDecimalPlaces");
			}

			// Change decimal places only on postback, which means user actually changed the dropdown selection
			if (this.Page.IsPostBack)
			{
				if (siteUnitsSelected)
				{
					this.FlowDecimalPlacesTextbox.Text = this.CurrentSite.FlowDecimalPlaces;
				}
				else
				{
					this.FlowDecimalPlacesTextbox.Text = this.Product.FlowDecimalPlaces.ToString();
				}
			}

			this.Product.FlowUnits = (EngineeringUnit)Convert.ToInt32(this.FlowUnitsDropDownList.SelectedValue);
		}

		private void InitializeComponent()
		{
			this.LevelUnitsDropDownList.SelectedIndexChanged		+= this.LevelUnitsDropDownListSelectedIndexChanged;
			this.VolumeUnitsDropDownList.SelectedIndexChanged		+= this.VolumeUnitsDropDownListSelectedIndexChanged;
			this.TemperatureUnitsDropDownList.SelectedIndexChanged	+= this.TemperatureUnitsDropDownListSelectedIndexChanged;
			this.DensityUnitsDropDownList.SelectedIndexChanged		+= this.DensityUnitsDropDownListSelectedIndexChanged;
			this.MassUnitsDropDownList.SelectedIndexChanged			+= this.MassUnitsDropDownListSelectedIndexChanged;
			this.FlowUnitsDropDownList.SelectedIndexChanged			+= this.FlowUnitsDropDownListSelectedIndexChanged;
			this.PressureUnitsDropDownList.SelectedIndexChanged		+= this.PressureUnitsDropDownListSelectedIndexChanged;
		}

		private void LevelUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			bool siteUnitsSelected = this.LevelUnitsDropDownList.SelectedIndex == 0;
			this.LevelDecimalPlacesTextbox.Enabled = !siteUnitsSelected;

			bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
            if (!((this.Product.IdentityGuid.Equals(Guid.Empty))
				  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
                  || (this.VersionSpecificFields == null))
               )
			{
				this.LevelDecimalPlacesTextbox.Enabled = this.LevelDecimalPlacesTextbox.Enabled
				                                         && this.VersionSpecificFields.Contains("LevelDecimalPlaces");
			}

			// Change decimal places only on postback, which means user actually changed the dropdown selection
			if (this.Page.IsPostBack)
			{
				if (siteUnitsSelected)
				{
					this.LevelDecimalPlacesTextbox.Text = this.CurrentSite.LevelDecimalPlaces;
				}
				else
				{
					this.LevelDecimalPlacesTextbox.Text = this.Product.LevelDecimalPlaces.ToString();
				}
			}

			this.Product.LevelUnits = (EngineeringUnit)Convert.ToInt32(this.LevelUnitsDropDownList.SelectedValue);
		}

		[SecurityCritical]
		private void MassUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			bool siteUnitsSelected = this.MassUnitsDropDownList.SelectedIndex == 0;
			this.MassDecimalPlacesTextbox.Enabled = !siteUnitsSelected;
			this.MassPackageSizeTextbox.Enabled = !siteUnitsSelected;

			bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
            if (!((this.Product.IdentityGuid.Equals(Guid.Empty))
				  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
                  || (this.VersionSpecificFields == null))
               )
			{
				this.MassDecimalPlacesTextbox.Enabled = this.MassDecimalPlacesTextbox.Enabled
				                                        && this.VersionSpecificFields.Contains("MassDecimalPlaces");
				this.MassPackageSizeTextbox.Enabled = this.MassPackageSizeTextbox.Enabled
				                                      && this.VersionSpecificFields.Contains("MassPackageSize");
			}

			// Change decimal places only on postback, which means user actually changed the dropdown selection
			if (this.Page.IsPostBack)
			{
				if (siteUnitsSelected)
				{
					this.MassDecimalPlacesTextbox.Text = this.CurrentSite.MassDecimalPlaces;
				}
				else
				{
					this.MassDecimalPlacesTextbox.Text = this.Product.MassDecimalPlaces.ToString();
				}
			}

			this.Product.MassUnits = (EngineeringUnit)Convert.ToInt32(this.MassUnitsDropDownList.SelectedValue);
			
			if (this.Product._MassPackageSize.Value != 0)
			{
				this.MassPackageSizeTextbox.Text = this.Product.MassPackageSize;
			}

			if (!this.MassPackageSizeTextbox.Enabled)
			{
				this.MassPackageSizeTextbox.Text = "";
				this.MassPackageSizelbl.Text = this.GetTranslatedText("N/A");
			}
			else
			{
				this.MassPackageSizelbl.Text = this.MassUnitsDropDownList.SelectedItem.Text;
			}
		}

		private void PressureUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			EngineeringUnit units = this.Product.PressureUnits;

			try
			{
				bool siteUnitsSelected = this.PressureUnitsDropDownList.SelectedIndex == 0;
				this.PressureDecimalPlacesTextbox.Enabled = !siteUnitsSelected;

				bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
                if (!((this.Product.IdentityGuid.Equals(Guid.Empty))
					  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
                      || (this.VersionSpecificFields == null))
                   )
				{
					this.PressureDecimalPlacesTextbox.Enabled = this.PressureDecimalPlacesTextbox.Enabled
					                                            && this.VersionSpecificFields.Contains("PressureDecimalPlaces");
				}

				// Change decimal places only on postback, which means user actually changed the dropdown selection
				if (this.Page.IsPostBack)
				{
					if (siteUnitsSelected)
					{
						this.PressureDecimalPlacesTextbox.Text = this.CurrentSite.PressureDecimalPlaces;
					}
					else
					{
						this.PressureDecimalPlacesTextbox.Text = this.Product.PressureDecimalPlaces.ToString();
					}
				}

				this.Product.PressureUnits = (EngineeringUnit)Convert.ToInt32(this.PressureUnitsDropDownList.SelectedValue);
				var volumeCorrectionPage =
					(ProductVolumeCorrectionPage)this.Page.FindControl("tcProductTabs")
							.FindControl("tpVolumeCorrectionPage")
							.FindControl("ProductVolumeCorrectionPage");

				if (sender != null)
				{
					volumeCorrectionPage.ValidateDataFromUI();
					volumeCorrectionPage.UpdateData();
				}

				this.Product.PressureUnits = (EngineeringUnit)Convert.ToInt32(this.PressureUnitsDropDownList.SelectedValue);

				volumeCorrectionPage.ValidateDataFromUI();
				volumeCorrectionPage.UpdateView();
			}
			catch (Exception ex)
			{
				if (this.Product.PressureUnits != units)
				{
					//Revert back
					this.Product.PressureUnits = units;
				}

				this.PressureUnitsDropDownList.SelectedValue = ((int)units).ToString(CultureInfo.InvariantCulture);
				this.ErrorHandler(ex);
			}
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

			this.LevelUnitsDropDownList.Enabled = (this.LevelUnitsDropDownList.Enabled && this.VersionSpecificFields.Contains("LevelUnitIndex"));
			this.LevelDecimalPlacesTextbox.Enabled = (this.LevelDecimalPlacesTextbox.Enabled && this.VersionSpecificFields.Contains("LevelDecimalPlaces"));
			this.VolumeUnitsDropDownList.Enabled = (this.VolumeUnitsDropDownList.Enabled && this.VersionSpecificFields.Contains("VolumeUnitIndex"));
			this.VolumeDecimalPlacesTextbox.Enabled = (this.VolumeDecimalPlacesTextbox.Enabled && this.VersionSpecificFields.Contains("VolumeDecimalPlaces"));
			this.TemperatureUnitsDropDownList.Enabled = (this.TemperatureUnitsDropDownList.Enabled && this.VersionSpecificFields.Contains("TemperatureUnitIndex"));
			this.TemperatureDecimalPlacesTextbox.Enabled = (this.TemperatureDecimalPlacesTextbox.Enabled && this.VersionSpecificFields.Contains("TemperatureDecimalPlaces"));
			this.DensityUnitsDropDownList.Enabled = (this.DensityUnitsDropDownList.Enabled && this.VersionSpecificFields.Contains("DensityUnitIndex"));
			this.DensityDecimalPlacesTextbox.Enabled = (this.DensityDecimalPlacesTextbox.Enabled && this.VersionSpecificFields.Contains("DensityDecimalPlaces"));
			this.MassUnitsDropDownList.Enabled = (this.MassUnitsDropDownList.Enabled && this.VersionSpecificFields.Contains("MassUnitIndex"));
			this.MassDecimalPlacesTextbox.Enabled = (this.MassDecimalPlacesTextbox.Enabled && this.VersionSpecificFields.Contains("MassDecimalPlaces"));
			this.FlowUnitsDropDownList.Enabled = (this.FlowUnitsDropDownList.Enabled && this.VersionSpecificFields.Contains("FlowUnitIndex"));
			this.FlowDecimalPlacesTextbox.Enabled = (this.FlowDecimalPlacesTextbox.Enabled && this.VersionSpecificFields.Contains("FlowDecimalPlaces"));
			this.PressureUnitsDropDownList.Enabled = (this.PressureUnitsDropDownList.Enabled && this.VersionSpecificFields.Contains("PressureUnitIndex"));
			this.PressureDecimalPlacesTextbox.Enabled = (this.PressureDecimalPlacesTextbox.Enabled && this.VersionSpecificFields.Contains("PressureDecimalPlaces"));
			this.VolumePackageSizeTextbox.Enabled = (this.VolumePackageSizeTextbox.Enabled && this.VersionSpecificFields.Contains("PVolumePackageSize"));
			this.MassPackageSizeTextbox.Enabled = (this.MassPackageSizeTextbox.Enabled && this.VersionSpecificFields.Contains("MassPackageSize"));
		}

		private void TemperatureUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			EngineeringUnit units = this.Product.TemperatureUnits;

			try
			{
				bool siteUnitsSelected = this.TemperatureUnitsDropDownList.SelectedIndex == 0;
				this.TemperatureDecimalPlacesTextbox.Enabled = !siteUnitsSelected;

				bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
                if (!((this.Product.IdentityGuid.Equals(Guid.Empty))
					  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
                      || (this.VersionSpecificFields == null))
                   )
				{
					this.TemperatureDecimalPlacesTextbox.Enabled = this.TemperatureDecimalPlacesTextbox.Enabled
					                                               && this.VersionSpecificFields.Contains("TemperatureDecimalPlaces");
				}

				// Change decimal places only on postback, which means user actually changed the dropdown selection
				if (this.Page.IsPostBack)
				{
					if (siteUnitsSelected)
					{
						this.TemperatureDecimalPlacesTextbox.Text = this.CurrentSite.TemperatureDecimalPlaces;
					}
					else
					{
						this.TemperatureDecimalPlacesTextbox.Text = this.Product.TemperatureDecimalPlaces.ToString();
					}
				}

				var volumeCorrectionPage =
					(ProductVolumeCorrectionPage)this.Page.FindControl("tcProductTabs")
							.FindControl("tpVolumeCorrectionPage")
							.FindControl("ProductVolumeCorrectionPage");

				var productAlarmsPage =
					(ProductAlarmsPage)this.Page.FindControl("tcProductTabs").FindControl("tpAlarmsPage").FindControl("ProductAlarmsPage");

				if (sender != null)
				{
					productAlarmsPage.ValidateDataFromUI();
					volumeCorrectionPage.ValidateDataFromUI();
					productAlarmsPage.UpdateData();
					volumeCorrectionPage.UpdateData();
				}

				this.Product.TemperatureUnits = (EngineeringUnit)Convert.ToInt32(this.TemperatureUnitsDropDownList.SelectedValue);

				productAlarmsPage.ValidateDataToUI();
				volumeCorrectionPage.ValidateDataFromUI();
				volumeCorrectionPage.UpdateView();
				productAlarmsPage.UpdateView();
			}
			catch (Exception ex)
			{
				if (this.Product.TemperatureUnits != units)
				{
					//Revert back
					this.Product.TemperatureUnits = units;
				}

				this.TemperatureUnitsDropDownList.SelectedValue = ((int)units).ToString(CultureInfo.InvariantCulture);
				this.ErrorHandler(ex);
			}
		}

		[SecurityCritical]
		private void VolumeUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			bool siteUnitsSelected = this.VolumeUnitsDropDownList.SelectedIndex == 0;
			this.VolumeDecimalPlacesTextbox.Enabled = !siteUnitsSelected;
			this.VolumePackageSizeTextbox.Enabled = !siteUnitsSelected;

			bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
            if (!((this.Product.IdentityGuid.Equals(Guid.Empty))
				  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)) 
                  || (this.VersionSpecificFields == null))
               )
			{
				this.VolumeDecimalPlacesTextbox.Enabled = this.VolumeDecimalPlacesTextbox.Enabled
				                                          && this.VersionSpecificFields.Contains("VolumeDecimalPlaces");
				this.VolumePackageSizeTextbox.Enabled = this.VolumePackageSizeTextbox.Enabled
				                                        && this.VersionSpecificFields.Contains("VolumePackageSize");
			}

			// Change decimal places only on postback, which means user actually changed the dropdown selection
			if (this.Page.IsPostBack)
			{
				if (siteUnitsSelected)
				{
					this.VolumeDecimalPlacesTextbox.Text = this.CurrentSite.VolumeDecimalPlaces;
				}
				else
				{
					this.VolumeDecimalPlacesTextbox.Text = this.Product.VolumeDecimalPlaces.ToString();
				}
			}

			this.Product.VolumeUnits = (EngineeringUnit)Convert.ToInt32(this.VolumeUnitsDropDownList.SelectedValue);

			if (this.Product._VolumePackageSize.Value != 0)
			{
				this.VolumePackageSizeTextbox.Text = this.Product.VolumePackageSize;
			}

			if (!this.VolumePackageSizeTextbox.Enabled)
			{
				this.VolumePackageSizeTextbox.Text = "";
				this.VolumePackageSizelbl.Text = this.GetTranslatedText("N/A");
			}
			else
			{
				this.VolumePackageSizelbl.Text = this.VolumeUnitsDropDownList.SelectedItem.Text;
			}
		}
		#endregion
	}
}