// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProductGeneralPage.ascx.cs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	///    Summary description for ProductGeneralPage.
	/// </summary>
	public partial class ProductGeneralPage : ProductPageBase
	{
		#region Constants and Fields
		protected SiteClass CurrentSite;
		#endregion

		#region Public Methods and Operators
		public void UpdateData()
		{
			this.Product.ID = this.IDTextbox.Text;
			this.Product.Code = this.CodeTextbox.Text;
			this.Product.Description = this.DescriptionTextbox.Text;
			this.Product.OctaneNumber = this.OctaneTextbox.Text;
			this.Product.ReidVaporPressure = this.ReidVaporPressureTextbox.Text;
			this.Product.LoadRackDisplayText = this.LoadRackDisplayTextbox.Text;
			this.Product.VaporRecovery = this.VaporRecoveryCheckBox.Checked;
			
			if (this.VarianceToleranceTextbox.Text.Trim() == string.Empty)
			{
				throw new Exception("Variance Tolerance required.");
			}

            this.Product.VarianceTolerance = Convert.ToDouble(	this.VarianceToleranceTextbox.Text, 
																this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
			this.Product.DielectricTolerance	= Convert.ToDouble(this.DielectricToleranceTextbox.Text, this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
			this.Product.LoadByWeight			= this.LoadByWeightCheckBox.Checked;
			this.Product.HazardousMaterial		= this.HazardousMaterialCheckBox.Checked;
			this.Product.InhibitAccounting		= this.InhibitAccountingCheckBox.Checked;

			// Only set the hidden date if the hidden check box is checked and there isn't already a value
			if (this.HiddenCheckBox.Checked && !this.Product.HiddenDate.HasValue)
			{
				this.Product.HiddenDate = DateTimeOffset.Now;
			}
			else if (!this.HiddenCheckBox.Checked)
			{
				this.Product.HiddenDate = null;
			}

			this.Product.PIDXCode = this.PIDXCodeTextbox.Text;
			this.Product.PIDXFamilyCode = this.PIDXFamilyCodeTextbox.Text;
			this.Product.ContaminationPromptLoadRackText = this.ContaminationPromptLoadRackTextTextBox.Text;
			this.Product.LockedOut = this.LockedOutCheckBox.Checked;
			this.Product.LockedOutReason = this.LockedOutReasonTextbox.Text;
			this.Product.TrackingProductID = this.TrackingProductTextBox.Text;
			this.Product.IsEthanol = this.IsEthanolCheckBox.Checked;

			if (this.Product.TrackingProductID == this.GetTranslatedText("{None}"))
			{
				this.Product.TrackingProductGuid = Guid.Empty;
			}
			else
			{
                ProductClass trackingProd = FMChannelHelper.MakeCall<IProducts, ProductClass>(
                                                                     x =>
                                                                     x.GetByID(this.Security, this.Product.TrackingProductID)
                                                                );
                this.Product.TrackingProductGuid = trackingProd.MasterRecordGuid;
			}

			this.Product.Price = this.PriceTextBox.Text;

            switch (this.ProductClassification.SelectedIndex)
            {
                case 0:
                    this.Product.AviationFuel = false;
                    this.Product.GroundFuel = false;
                    break;
                case 1:
                    this.Product.AviationFuel = true;
                    this.Product.GroundFuel = false;
                    break;
                case 2:
                    this.Product.AviationFuel = false;
                    this.Product.GroundFuel = true;
                    break;
            }

			//this.Product.AviationFuel = this.IsAviationFuelCheckBox.Checked;
			this.Product.TaxCode = this.TaxCodeTextBox.Text;
            this.Product.AutomaticCloseout = this.AutomaticCloseoutCheckBox.Checked; 
		}
		#endregion

		#region Methods
		protected void LoadRackDisplayTextboxTextChanged(object sender, EventArgs e)
		{
		}

		protected void LockedOutCheckBoxCheckedChanged(object sender, EventArgs e)
		{
            if (!this.LockedOutCheckBox.Checked)
            {
                this.LockedOutDateTextbox.Text = string.Empty;
                this.LockedOutReasonTextbox.Text = string.Empty;
                this.LockedOutReasonTextbox.Enabled = false;
            }
            else
            {
                bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
                if (this.Product.IdentityGuid.Equals(Guid.Empty)
                    || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid)))
                {
                    this.LockedOutReasonTextbox.Enabled = true;
                }
                else if (this.VersionSpecificFields != null)
                {
                    this.LockedOutReasonTextbox.Enabled = this.VersionSpecificFields.Contains("LockedOutReason");
                }

                this.Product._LockedOutDate.Value = TimeConverter.Today();
                this.LockedOutDateTextbox.Text = this.Product.LockedOutDate;
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
				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				if (!this.Page.IsPostBack)
				{
					this.IDTextbox.Text = this.Product.ID;
					this.CodeTextbox.Text = this.Product.Code;
					this.DescriptionTextbox.Text = this.Product.Description;

					// ProductTypeDropDownList
					for (var productType = ProductType.ComponentProduct; productType < ProductType.MaxProduct; productType++)
					{
						if (productType == ProductType.AdditizedProduct)
						{
							continue;
						}

						var newTypeItem = new ListItem(	ProductClass.ProductTypeID(productType), 
														((int)productType).ToString(CultureInfo.InvariantCulture));
						this.ProductTypeDropDownList.Items.Add(newTypeItem);
						
						if (this.Product.ProductType == productType)
						{
							this.ProductTypeDropDownList.SelectedIndex = this.ProductTypeDropDownList.Items.Count - 1;
						}
					}

					this.OctaneTextbox.Text = this.Product.OctaneNumber;
					this.ReidVaporPressureTextbox.Text = this.Product.ReidVaporPressure;
					this.LoadRackDisplayTextbox.Text = this.Product.LoadRackDisplayText;
					this.VaporRecoveryCheckBox.Checked = this.Product.VaporRecovery;
					
					if (this.Product.ProductType == ProductType.AdditiveProduct)
					{
						this.VaporRecoveryCheckBox.Enabled = false;
					}

					this.VarianceToleranceTextbox.Text =
					this.Product.VarianceTolerance.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
					this.DielectricToleranceTextbox.Text				= this.Product.DielectricTolerance.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
					this.LoadByWeightCheckBox.Checked					= this.Product.LoadByWeight;
					this.HazardousMaterialCheckBox.Checked				= this.Product.HazardousMaterial;
					this.InhibitAccountingCheckBox.Checked				= this.Product.InhibitAccounting;
					this.HiddenCheckBox.Checked							= this.Product.HiddenDate.HasValue;
					this.PIDXCodeTextbox.Text							= this.Product.PIDXCode;
					this.PIDXFamilyCodeTextbox.Text						= this.Product.PIDXFamilyCode;
					this.ContaminationPromptLoadRackTextTextBox.Text	= this.Product.ContaminationPromptLoadRackText;
					this.LockedOutCheckBox.Checked						= this.Product.LockedOut;
					this.IsEthanolCheckBox.Checked							= this.Product.IsEthanol;

					if (this.Product.ProductType == ProductType.BlendProduct)
					{
						this.IsEthanolCheckBox.Enabled = false;
					}


					// WI#9676 - Lockedout reason box should always be disabled on initial page load
					this.LockedOutReasonTextbox.Enabled = false;

					if (this.Product.LockedOut)
					{
						this.LockedOutReasonTextbox.Text = this.Product.LockedOutReason;
						this.LockedOutDateTextbox.Text = this.Product.LockedOutDate;
					}

					this.TrackingProductTextBox.Text = this.GetTranslatedText(this.Product.TrackingProductID);
					this.PriceTextBox.Text = this.Product.Price;

                    if (this.Product.AviationFuel && !this.Product.GroundFuel)
                    {
                        this.ProductClassification.SelectedIndex = 1;
                    }
                    else if (this.Product.GroundFuel && !this.Product.AviationFuel)
                    {
                        this.ProductClassification.SelectedIndex = 2;
                    }
                    else
                    {
                        this.ProductClassification.SelectedIndex = 0;
                    }

					//this.IsAviationFuelCheckBox.Checked = this.Product.AviationFuel;
					this.TaxCodeTextBox.Text = this.Product.TaxCode;
                    this.AutomaticCloseoutCheckBox.Checked = this.Product.AutomaticCloseout;
                    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ProductTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				((ProductForm)this.Page).UpdateData();

				this.Product.ProductType = (ProductType)Convert.ToInt32(this.ProductTypeDropDownList.SelectedValue);
				this.Product.ComponentCollection.Clear();

				if (this.Product.ProductType == ProductType.AdditiveProduct)
				{
					this.Product.VaporRecovery = false;
				}
				else
				{
					this.VaporRecoveryCheckBox.Enabled = true;
				}

				if (this.Product.ProductType == ProductType.BlendProduct)
				{
					this.Product.IsEthanol = false;
				}

				this.Session.Remove("Page");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("ProductForm.aspx");
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

      private void SetFieldAccessibilityForChildRecordVersion()
      {
         bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);

         if ( this.Product.IdentityGuid.Equals(Guid.Empty)
               || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))
               || (this.VersionSpecificFields == null))
         {
               return;
         }

         this.IDTextbox.Enabled = (this.IDTextbox.Enabled && this.VersionSpecificFields.Contains("ID"));
         this.CodeTextbox.Enabled = (this.CodeTextbox.Enabled && this.VersionSpecificFields.Contains("ProductCode"));
         this.DescriptionTextbox.Enabled = (this.DescriptionTextbox.Enabled && this.VersionSpecificFields.Contains("Description"));
         this.LoadRackDisplayTextbox.Enabled = (this.LoadRackDisplayTextbox.Enabled && this.VersionSpecificFields.Contains("LoadRackDisplayText"));
         this.ProductTypeDropDownList.Enabled = (this.ProductTypeDropDownList.Enabled && this.VersionSpecificFields.Contains("ProductType"));
         this.ReidVaporPressureTextbox.Enabled = (this.ReidVaporPressureTextbox.Enabled && this.VersionSpecificFields.Contains("ReidVaporPressure"));
         this.OctaneTextbox.Enabled = (this.OctaneTextbox.Enabled && this.VersionSpecificFields.Contains("OctaneNumber"));
         this.LockedOutCheckBox.Enabled = (this.LockedOutCheckBox.Enabled && this.VersionSpecificFields.Contains("LockedOut"));
         this.LockedOutReasonTextbox.Enabled = (this.LockedOutReasonTextbox.Enabled && this.VersionSpecificFields.Contains("LockedOutReason"));
         this.LockedOutDateTextbox.Enabled = (this.LockedOutDateTextbox.Enabled && this.VersionSpecificFields.Contains("LockedOutDate"));
         this.VaporRecoveryCheckBox.Enabled = (this.VaporRecoveryCheckBox.Enabled && this.VersionSpecificFields.Contains("VaporRecovery"));
         this.VarianceToleranceTextbox.Enabled = (this.VarianceToleranceTextbox.Enabled && this.VersionSpecificFields.Contains("VarianceTolerance"));
			this.DielectricToleranceTextbox.Enabled = (this.DielectricToleranceTextbox.Enabled && this.VersionSpecificFields.Contains("DielectricTolerance"));
			this.LoadByWeightCheckBox.Enabled = (this.LoadByWeightCheckBox.Enabled && this.VersionSpecificFields.Contains("LoadByWeight"));
         this.HazardousMaterialCheckBox.Enabled = (this.HazardousMaterialCheckBox.Enabled && this.VersionSpecificFields.Contains("HazardousMaterial"));
         this.PIDXCodeTextbox.Enabled = (this.PIDXCodeTextbox.Enabled && this.VersionSpecificFields.Contains("PIDXCode"));
         this.PIDXFamilyCodeTextbox.Enabled = (this.PIDXFamilyCodeTextbox.Enabled && this.VersionSpecificFields.Contains("PIDXFamilyCode"));
         this.ContaminationPromptLoadRackTextTextBox.Enabled = (this.ContaminationPromptLoadRackTextTextBox.Enabled && this.VersionSpecificFields.Contains("ContaminationPromptLoadRackText"));
         this.InhibitAccountingCheckBox.Enabled = (this.InhibitAccountingCheckBox.Enabled && this.VersionSpecificFields.Contains("InhibitAccounting"));
			this.IsEthanolCheckBox.Enabled = (this.IsEthanolCheckBox.Enabled && this.VersionSpecificFields.Contains("IsEthanol"));
			this.TrackingProductTextBox.Enabled = (this.TrackingProductTextBox.Enabled && this.VersionSpecificFields.Contains("TrackingProductGuid"));
         this.ProductClassification.Enabled = (this.ProductClassification.Enabled && this.VersionSpecificFields.Contains("AviationFuelFlag"));
         this.PriceTextBox.Enabled = (this.PriceTextBox.Enabled && this.VersionSpecificFields.Contains("Price"));
         this.TaxCodeTextBox.Enabled = (this.TaxCodeTextBox.Enabled && this.VersionSpecificFields.Contains("TaxCode"));
         this.HiddenCheckBox.Enabled = this.HiddenCheckBox.Enabled && this.VersionSpecificFields.Contains("HiddenDate");
         this.AutomaticCloseoutCheckBox.Enabled = this.AutomaticCloseoutCheckBox.Enabled && this.VersionSpecificFields.Contains("AutomaticCloseout");
      }
		#endregion
	}
}