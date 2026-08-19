// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentGeneralPage type.
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
	///    Summary description for EquipmentGeneralPage.
	/// </summary>
	public partial class EquipmentGeneralPage : EquipmentPageBase
	{
		#region Constants and Fields
		protected SiteClass CurrentSite;
        #endregion

        #region Events Available to Parent

        public delegate void EquipmentTypeChangedDelegate(object sender, EquipmentTypeChangedEventArgs args);

        public event EquipmentTypeChangedDelegate EquipmentTypeChanged;

        #endregion


        protected void InitializeIdFields()
		{
			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
			{
				this.RefIDRequiredSymbol.Visible = true;
				this.RefIDTextbox.MaxLength = 4;
				this.IDTextbox.MaxLength = 8;
				this.RegistryJavaScriptToCreateRefID();
			}
		}

		#region Public Methods and Operators
		public void UpdateData()
		{
			this.Equipment.ID					= this.IDTextbox.Text;
			this.Equipment.SerialNumber			= this.SerialNumberTextbox.Text;
			this.Equipment.TruckCardNumber		= this.CardTextbox.Text;
			this.Equipment.LockedOut			= this.LockedOutCheckBox.Checked;
			this.Equipment.LockedOutReason		= this.LockedOutReasonTextbox.Text;
			this.Equipment.CompanyEquipmentID	= this.CompanyEquipmentIDTextBox.Text;
            this.Equipment.ScullyRequired       = this.ScullyRequiredCheckBox.Checked;

            if (this.RefIDTextbox.Text.Trim().Length == 0)
			{
				string id = this.Equipment.ID.Trim();
				if (id.Length < 4)
				{
					this.RefIDTextbox.Text = id;
				}
				else
				{
					this.RefIDTextbox.Text = id.Substring(id.Length - 4, 4);
				}
			}

			this.Equipment.Xref = this.RefIDTextbox.Text;
			this.Equipment.Make = this.MakeTextbox.Text;
			this.Equipment.Model = this.ModelTextbox.Text;
			this.Equipment.Description = this.DescriptionTextbox.Text;
			if (!this.Equipment.IsMultiCompartment)
			{
				this.Equipment.Volume = this.VolumeTextbox.Text;
				this.Equipment.Capacity = this.CapacityTextbox.Text;
				this.Equipment.SafeFill = this.SafeFillTextbox.Text;
			}

			try
			{
				if (this.YearTextbox.Text.Trim().Length == 0)
				{
					this.Equipment.Year = 0;
				}
				else
				{
					this.Equipment.Year = Convert.ToInt32(this.YearTextbox.Text);
				}
			}
			catch
			{
				throw new Exception("Invalid entry for Equipment Year.");
			}

			if (this.CompanyTextBox.Text == this.GetTranslatedText("{Unassigned}"))
			{
				this.Equipment.CompanyID = "{Unassigned}";
				this.Equipment.CompanyGuid = Guid.Empty;
			}
			else
			{
				this.Equipment.CompanyID = this.CompanyTextBox.Text;
				Guid companyMasterGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
						x => x.GetMasterRecordGuid(this.Security, this.Equipment.CompanyID));

				if (companyMasterGuid != Guid.Empty)
				{
					this.Equipment.CompanyGuid = companyMasterGuid;
				}
			}

			if (this.ProductTextBox.Text == this.GetTranslatedText("{Unassigned}"))
			{
				this.Equipment.ProductID = "{Unassigned}";
				this.Equipment.ProductGuid = Guid.Empty;
			}
			else
			{
				this.Equipment.ProductID = this.ProductTextBox.Text;
				Guid productMasterGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
						x => x.GetMasterRecordGuidFromID(this.Security, this.Equipment.ProductID));

                if (productMasterGuid != Guid.Empty)
				{
                    this.Equipment.ProductGuid = productMasterGuid;
				}
			}

			if (this.FuelCardTextBox.Text == this.GetTranslatedText("{Unassigned}"))
			{
				this.Equipment.FuelCardID = "{Unassigned}";
				this.Equipment.FuelCardGuid = Guid.Empty;
			}
			else
			{
				this.Equipment.FuelCardID = this.FuelCardTextBox.Text;
				Guid fuelCardGuid = FMChannelHelper.MakeCall<IFuelCards, Guid>(
						x => x.GetIdentityGuid(this.Security, this.Equipment.FuelCardID));

				if (fuelCardGuid != Guid.Empty)
				{
					this.Equipment.FuelCardGuid = fuelCardGuid;
				}
			}

			if (this.AssetTrackingDeviceTextBox.Text == this.GetTranslatedText("{Unassigned}"))
			{
				this.Equipment.AssetTrackingDeviceID = "{Unassigned}";
				this.Equipment.AssetTrackingDeviceGuid = Guid.Empty;
			}
			else
			{
				this.Equipment.AssetTrackingDeviceID = this.AssetTrackingDeviceTextBox.Text;
				Guid assetTrackingDeviceGuid = FMChannelHelper.MakeCall<IAssetTrackingDevices, Guid>(
						x => x.GetIdentityGuid(this.Security, this.Equipment.AssetTrackingDeviceID));


				if (assetTrackingDeviceGuid != Guid.Empty)
				{
					this.Equipment.AssetTrackingDeviceGuid = assetTrackingDeviceGuid;
				}
			}

			Guid equipmentTypeGuid = Guid.Parse(this.EquipmentTypeDropDownList.SelectedValue);
			var equipmentTypeClass = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
					x => x.Get(this.Security, equipmentTypeGuid));

			this.Equipment.SetEquipmentType(equipmentTypeClass);

			if (this.EquipmentTypeDropDownList.SelectedIndex == 0)
			{
				this.Equipment.Type = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;
			}

			this.Equipment.FuelingType = (FUELING_TYPES)Convert.ToInt16(this.FuelingTypeDropDownList.SelectedValue);

            // Only set the hidden date if the hidden check box is checked and there isn't already a value
            if (this.HiddenCheckBox.Checked && !this.Equipment.HiddenDate.HasValue)
            {
                this.Equipment.HiddenDate = DateTimeOffset.Now;
            }
            else if (!this.HiddenCheckBox.Checked)
            {
                this.Equipment.HiddenDate = null;
            }
		}
		#endregion

		#region Methods
		protected void AssignButtonCommand(object sender, CommandEventArgs e)
		{
		}

		protected ListItemCollection EnumerateFuelTypes()
		{
			var fuelTypesColl = new ListItemCollection();
			Array fuelTypeValues = Enum.GetValues(typeof(FUELING_TYPES));
			foreach (int fuelTypeValue in fuelTypeValues)
			{
				var item = new ListItem(Enum.GetName(typeof(FUELING_TYPES), fuelTypeValue), fuelTypeValue.ToString(CultureInfo.InvariantCulture));
				if (item.Text == "NONE")
				{
					item.Text = "";
				}

				fuelTypesColl.Add(item);
			}

			return fuelTypesColl;
		}

		protected void EquipmentTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			string redirectString = "EquipmentForm.aspx";

			try
			{
				((EquipmentForm)this.Page).UpdateData();

				Guid equipmentTypeGuid = Guid.Parse(this.EquipmentTypeDropDownList.SelectedValue);

				EquipmentTypeClass equipmentType = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
						x => x.Get(this.Security, equipmentTypeGuid));

				this.Equipment.SetEquipmentType(equipmentType);

				// When changing type dump the compartment collection if the new type doesn't support
				// compartments
				if (this.Equipment.CompartmentCollection.Count != 0 && !this.Equipment.IsMultiCompartment)
				{
					this.Equipment.CompartmentCollection = new EquipmentCollectionClass();
				}

				this.Equipment.Make = equipmentType.Make;
				this.Equipment.Model = equipmentType.Model;
				this.Equipment.Year = equipmentType.Year;

				// Set capacity and safe fill volume units so SI values are converted properly
				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false, bGetAssociatedAliases: false));
				equipmentType.SICapacity.Units = site.VolumeUnits;
				equipmentType.SISafeFill.Units = site.VolumeUnits;

				this.Equipment.Capacity = equipmentType.Capacity;
				this.Equipment.SafeFill = equipmentType.SafeFill;

				if (equipmentType.Attribute == EQUIPMENT_TYPE.AIRCRAFT_TYPE)
				{
					this.Equipment.ProductGuid = equipmentType.Product;

					ProductClass prod = FMChannelHelper.MakeCall<IProducts, ProductClass>(
							x => x.Get(this.Security, this.Equipment.ProductGuid));

					if (prod != null)
					{
						this.Equipment.ProductID = prod.ID;
					}
				}

                //For refueling types (tanker, stationary cart, or hydrant cart) automatically create meter with ID that is the same as the 
                //EquipmentID
                //if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
                //{
                    if (this.Equipment.Type == EQUIPMENT_TYPE.TANKER_TYPE || this.Equipment.Type == EQUIPMENT_TYPE.HYDRANT_CART_TYPE
                        || this.Equipment.Type == EQUIPMENT_TYPE.STATIONARY_CART_TYPE)
                    {
                        this.Equipment.FuelingType = FUELING_TYPES.REFUELER;
                        //raise the event
                        //send true if meter should be automatically changed, false if not
                        EquipmentTypeChangedEventArgs args = new EquipmentTypeChangedEventArgs
                                                             {
                                                                 AutoCreateMeter = true,
                                                                 EquipmentID = this.IDTextbox.Text
                                                             };
                        this.EquipmentTypeChanged?.Invoke(this, args);
                    }
                    else
                    {
                        this.Equipment.FuelingType = FUELING_TYPES.NONE;
                        //raise the event
                        //send true if meter should be automatically changed, false if not
                        EquipmentTypeChangedEventArgs args = new EquipmentTypeChangedEventArgs
                                                             {
                                                                 AutoCreateMeter = false,
                                                                 EquipmentID = this.IDTextbox.Text
                                                             };
                        this.EquipmentTypeChanged?.Invoke(this, args);
                    }
                //}

                if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()))
				{
					if (this.Equipment.Type == EQUIPMENT_TYPE.TRAILER_TYPE || this.Equipment.Type == EQUIPMENT_TYPE.HYDRANT_CART_TYPE)
					{
						this.Equipment.FuelingType = FUELING_TYPES.REFUELER;
					}
					else
					{
						this.Equipment.FuelingType = FUELING_TYPES.NONE;
					}
				}

				var fmForm = (FMFormBase)this.Page;
				if (fmForm != null && fmForm.IsFromDispatch)
				{
					redirectString += "?DispatchEdit=" + fmForm.DispatchEntityGuid;
				}

				else if (fmForm != null && fmForm.IsFromQueryWriter)
				{
					redirectString += "?QueryEdit=" + fmForm.QueryEntityGuid;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			var equipmentForm = this.Page as EquipmentForm;
		    if (equipmentForm != null)
		    {
		        this.Session["EquipmentFormTabIndex"] = equipmentForm.ActiveTabIndex;
		    }

		    this.Redirect(redirectString);
		}

		protected void FuelingTypeSelectedIndexChanged(object sender, EventArgs e)
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
                bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
                if ((this.Equipment.IdentityGuid.Equals(Guid.Empty))
					 || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid)))
				{
					this.LockedOutReasonTextbox.Enabled = true;
				}
                else if (this.VersionSpecificFields != null)
                {
					this.LockedOutReasonTextbox.Enabled = this.VersionSpecificFields.Contains("LockedOutReason");
				}

				this.Equipment._LockedOutDate.Value = TimeConverter.Today();
				this.LockedOutDateTextbox.Text = this.Equipment.LockedOutDate;
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
				if (!this.Page.IsPostBack)
				{
					this.IDTextbox.Text = this.Equipment.ID;
					this.DescriptionTextbox.Text = this.Equipment.Description;
					EquipmentTypeCollectionClass equipmentTypeCollection = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>(
							x => x.Enumerate(this.Security, null, null));

					var newTypeItem = new ListItem("{Unassigned}", Guid.Empty.ToString());
					this.EquipmentTypeDropDownList.Items.Add(newTypeItem);
					this.EquipmentTypeDropDownList.SelectedIndex = 0;

					foreach (EquipmentTypeClass equipmentType in equipmentTypeCollection)
					{
						// Skip Compartments, only accessible through Trailers
						if (equipmentType.Attribute == EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
						{
							continue;
						}

						newTypeItem = new ListItem(equipmentType.ID, equipmentType.IdentityGuid.ToString());
						this.EquipmentTypeDropDownList.Items.Add(newTypeItem);

						if (equipmentType.IdentityGuid == this.Equipment.EquipmentTypeGuid)
						{
							this.EquipmentTypeDropDownList.SelectedIndex = this.EquipmentTypeDropDownList.Items.Count - 1;
							this.CompanyTextBox.Role = equipmentType.CompanyRoleAssignmentConstraint.ToString();
						}
					}

					this.ProductTextBox.Text				= this.Equipment.ProductID;
					this.FuelCardTextBox.Text				= this.Equipment.FuelCardID;
					this.AssetTrackingDeviceTextBox.Text	= this.Equipment.AssetTrackingDeviceID;
					this.RefIDTextbox.Text					= this.Equipment.Xref;
					this.InitializeIdFields();
					this.RefIDRequiredSymbol.Visible = FMChannelHelper.MakeCall<IHardwareKey, bool>(
							x => x.IsDescKey());

					if (this.RefIDTextbox.Text.Trim().Length == 0)
					{
						string id = this.Equipment.ID.Trim();
						if (id.Length < 4)
						{
							this.RefIDTextbox.Text = id;
						}
						else
						{
							this.RefIDTextbox.Text = id.Substring(id.Length - 4, 4);
						}
					}

					this.LockedOutCheckBox.Checked = this.Equipment.LockedOut;
                    this.ScullyRequiredCheckBox.Checked = this.Equipment.ScullyRequired;
                    DateTimeFormatInfo dateFormat = DateTimeFormatInfo.CurrentInfo;
					if (this.Security != null)
					{
						SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
								x => x.GetBasic(this.Security, this.Security.SiteGuid));

					    DateTimeFormatInfo d = site?.GetDateTimeFormatInfo();
					    if (d != null)
					    {
					        dateFormat = d;
					    }
					}

					this.Equipment._LockedOutDate.Format = dateFormat;

					// Always disable Reason Testbox on page load. 
					this.LockedOutReasonTextbox.Enabled = false;

					if (this.Equipment.LockedOut)
					{
						this.LockedOutReasonTextbox.Text = this.Equipment.LockedOutReason;
						this.LockedOutDateTextbox.Text = this.Equipment.LockedOutDate;
					}

					if (this.Equipment.CompanyID == "{Unassigned}")
					{
						this.CompanyTextBox.Text = this.GetTranslatedText("{Unassigned}");
					}
					else
					{
						this.CompanyTextBox.Text = this.Equipment.CompanyID;
					}

					this.CompanyEquipmentIDTextBox.Text = this.Equipment.CompanyEquipmentID;
					this.FuelingTypeDropDownList.DataBind();
					foreach (ListItem x in this.FuelingTypeDropDownList.Items)
					{
						if (x.Value == ((int)this.Equipment.FuelingType).ToString(CultureInfo.InvariantCulture))
						{
							this.FuelingTypeDropDownList.SelectedIndex = (int)this.Equipment.FuelingType;
							break;
						}
					}

                    this.HiddenCheckBox.Checked = this.Equipment.HiddenDate.HasValue;

					if (this.Equipment == null)
					{
						return;
					}

					this.UpdateViewBasedOnEquipmentType();
					this.SetFieldAccessibilityForChildRecordVersion();
				}
				else
				{
					this.InitializeIdFields();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UnassignButtonCommand(object sender, CommandEventArgs e)
		{
		}

		protected void UpdateViewBasedOnEquipmentType()
		{
			this.VolumeTextbox.Text = this.Equipment.Volume;
			this.CapacityTextbox.Text = this.Equipment.Capacity;
			this.SafeFillTextbox.Text = this.Equipment.SafeFill;

			bool pipeline = (this.Equipment.Type == EQUIPMENT_TYPE.PIPELINE_TYPE);

			if (!pipeline)
			{
				this.MakeTextbox.Text = this.Equipment.Make;
				this.ModelTextbox.Text = this.Equipment.Model;
				this.YearTextbox.Text = (this.Equipment.Year == 0 ? string.Empty : this.Equipment.Year.ToString());
				this.SerialNumberTextbox.Text = this.Equipment.SerialNumber;
				this.CardTextbox.Text = this.Equipment.TruckCardNumber;
			}

			// If not a pipeline type, then make the fields visible.
			this.MakeLabel.Visible = !pipeline;
			this.MakeTextbox.Visible = !pipeline;
			this.ModelLabel.Visible = !pipeline;
			this.ModelTextbox.Visible = !pipeline;
			this.YearLabel.Visible = !pipeline;
			this.YearTextbox.Visible = !pipeline;
			this.SerialNumberTextbox.Visible = !pipeline;
			this.SerialNumLabel.Visible = !pipeline;
			this.CardTextbox.Visible = !pipeline;

			this.CapacityTextbox.Visible = !this.Equipment.IsMultiCompartment;
			this.SafeFillTextbox.Visible = !this.Equipment.IsMultiCompartment;
			this.CapacityFmLabel.Visible = !this.Equipment.IsMultiCompartment;
			this.SafeFillFmLabel.Visible = !this.Equipment.IsMultiCompartment;

			this.CompanyTextBox.Role = COMPANY_ROLE.CARRIER.ToString();
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		protected void RegistryJavaScriptToCreateRefID()
		{

			string strJScript =
			    $@" function PopulateRefID() {{
                    var EquipmentIDTextBox = document.getElementById('{
			        this.IDTextbox.ClientID}');
                    var RefIDTextBox = document.getElementById('{
			        this.RefIDTextbox.ClientID
			        }');
                    var jstrRefID = RefIDTextBox.value;               
                     var id = document.getElementById('{
			        this.IDTextbox.ClientID
			        }').value;
                     if (id.length < 4) {{
                      RefIDTextBox.value = id;
                     }}
                     else {{
                      RefIDTextBox.value = id.substr(id.length - 4);
                     }}                   
                   }}";

			System.Web.UI.ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "FieldError", strJScript, true);
			this.IDTextbox.Attributes.Add("onblur", "PopulateRefID()");

		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);

			if ((this.Equipment.IdentityGuid.Equals(Guid.Empty)
				  || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid))
				  || (this.VersionSpecificFields == null)))
			{
				return;
			}

			this.IDTextbox.Enabled = (this.IDTextbox.Enabled 
											  && this.VersionSpecificFields.Contains("ID"));
			this.DescriptionTextbox.Enabled = (this.DescriptionTextbox.Enabled 
														  && this.VersionSpecificFields.Contains("Description"));
			this.EquipmentTypeDropDownList.Enabled = false;
			//Cannot change the Equipment Type of a child record version, because it is used as a filter field in the definition of the Equipment entity segment template.
			this.MakeTextbox.Enabled = (this.MakeTextbox.Enabled 
												 && this.VersionSpecificFields.Contains("Make"));
			this.ModelTextbox.Enabled = (this.ModelTextbox.Enabled 
												  && this.VersionSpecificFields.Contains("Model"));
			this.YearTextbox.Enabled = (this.YearTextbox.Enabled 
												 && this.VersionSpecificFields.Contains("Year"));
			this.SerialNumberTextbox.Enabled = (this.SerialNumberTextbox.Enabled
															&& this.VersionSpecificFields.Contains("SerialNumber"));
			this.FuelingTypeDropDownList.Enabled = (this.FuelingTypeDropDownList.Enabled 
																 && this.VersionSpecificFields.Contains("FuelingType"));
			this.VolumeTextbox.Enabled = (this.VolumeTextbox.Enabled 
													&& this.VersionSpecificFields.Contains("Volume"));
			this.CapacityTextbox.Enabled = (this.CapacityTextbox.Enabled 
													  && this.VersionSpecificFields.Contains("Capacity"));
			this.SafeFillTextbox.Enabled = (this.SafeFillTextbox.Enabled 
													  && this.VersionSpecificFields.Contains("SafeFill"));
			this.ProductTextBox.Enabled = (this.ProductTextBox.Enabled 
                                                     && this.VersionSpecificFields.Contains("ProductGuid"));
			this.FuelCardTextBox.Enabled = (this.FuelCardTextBox.Enabled 
                                                      && this.VersionSpecificFields.Contains("FuelCardGuid"));
			this.RefIDTextbox.Enabled = (this.RefIDTextbox.Enabled 
												  && this.VersionSpecificFields.Contains("Xref"));
			this.LockedOutCheckBox.Enabled = (this.LockedOutCheckBox.Enabled 
														 && this.VersionSpecificFields.Contains("LockedOut"));
			this.LockedOutReasonTextbox.Enabled = (this.LockedOutReasonTextbox.Enabled 
																&& this.VersionSpecificFields.Contains("LockedOutReason"));
			this.LockedOutDateTextbox.Enabled = (this.LockedOutDateTextbox.Enabled 
															 && this.VersionSpecificFields.Contains("LockedOutDate"));
            this.ScullyRequiredCheckBox.Enabled = (this.ScullyRequiredCheckBox.Enabled 
                                                         && this.VersionSpecificFields.Contains("ScullyRequired"));
            this.CardTextbox.Enabled = (this.CardTextbox.Enabled 
												 && this.VersionSpecificFields.Contains("TruckCardNumber"));
			this.CompanyTextBox.Enabled = (this.CompanyTextBox.Enabled 
                                                     && this.VersionSpecificFields.Contains("CompanyGuid"));
			this.CompanyEquipmentIDTextBox.Enabled = (this.CompanyEquipmentIDTextBox.Enabled 
																&& this.VersionSpecificFields.Contains("CompanyEquipmentID"));
            this.HiddenCheckBox.Enabled = this.HiddenCheckBox.Enabled 
                && this.VersionSpecificFields.Contains("HiddenDate");
		}
		#endregion
	}

    /// <summary>
    /// This class contains the event args to be passed when raising the EquipmentTypeChanged event
    /// </summary>
    public class EquipmentTypeChangedEventArgs : EventArgs
    {
        public bool AutoCreateMeter { get; set; }
        public string EquipmentID { get; set; }
    }
}