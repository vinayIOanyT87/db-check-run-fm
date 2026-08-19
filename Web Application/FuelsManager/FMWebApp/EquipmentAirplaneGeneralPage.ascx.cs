// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentAirplaneGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentAirplaneGeneralPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    public partial class EquipmentAirplaneGeneralPage : EquipmentPageBase
	{
		#region Constants and Fields
		protected bool AllowFuelingByWeight;
		#endregion

		#region Public Methods and Operators

		public void UpdateData()
		{
			this.Equipment.ID = this.IDTextbox.Text;
			this.Equipment.SerialNumber = this.SerialNumberTextbox.Text;
			this.Equipment.CompanyEquipmentID = this.SerialNumberTextbox.Text;
			this.Equipment.Make = this.MakeTextbox.Text;
			this.Equipment.Model = this.ModelTextbox.Text;
			this.Equipment.Description = this.DescriptionTextbox.Text;
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
				throw new Exception(this.GetTranslatedText("Invalid entry for Equipment Year."));
			}

			if (this.CompanyTextBox.Text == this.GetTranslatedText("{Unassigned}"))
			{
				this.Equipment.CompanyID = "{Unassigned}";
				this.Equipment.CompanyGuid = Guid.Empty;
			}
			else
			{
				this.Equipment.CompanyID = this.CompanyTextBox.Text;
				Guid companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, this.Equipment.CompanyID)
																);

				if (companyGuid != Guid.Empty)
				{
					this.Equipment.CompanyGuid = companyGuid;
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
				Guid productMasterRecordGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetMasterRecordGuidFromID(this.Security, this.Equipment.ProductID)
																);

				if (productMasterRecordGuid != Guid.Empty)
				{
					this.Equipment.ProductGuid = productMasterRecordGuid;
				}
			}
			if (!this.AllowFuelingByWeight)
			{
				this.Equipment.FuelCardID = this.FuelCardTextBox.Text;
				if (this.FuelCardTextBox.Text == this.GetTranslatedText("{Unassigned}"))
				{
					this.Equipment.FuelCardID = "{Unassigned}";
					this.Equipment.FuelCardGuid = Guid.Empty;
				}
				else
				{
					this.Equipment.FuelCardID = this.FuelCardTextBox.Text;
					Guid fuelCardGuid = FMChannelHelper.MakeCall<IFuelCards, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, this.Equipment.FuelCardID)
																);

					if (fuelCardGuid != Guid.Empty)
					{
						this.Equipment.FuelCardGuid = fuelCardGuid;
					}
				}
				this.Equipment.TruckCardNumber = this.CardTextbox.Text;
			}

			Guid equipmentTypeGuid = Guid.Parse(this.EquipmentTypeDropDownList.SelectedValue);
			var equipmentTypeClass = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
																	 x =>
																	 x.Get(this.Security, equipmentTypeGuid)
																);

			this.Equipment.SetEquipmentType(equipmentTypeClass);

			if (this.EquipmentTypeDropDownList.SelectedIndex == 0)
			{
				this.Equipment.Type = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE;
			}

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

		protected void ApplyDataDictionary()
		{
			this.YearLabel.Text = this.GetTranslatedText("Year:");
			this.ModelLabel.Text = this.GetTranslatedText("Model:");
			this.MakeLabel.Text = this.GetTranslatedText("Make:");
			this.TypeLabel.Text = this.GetTranslatedText("Type:");
			this.DescriptionLabel.Text = this.GetTranslatedText("Description:");
			this.TailIdLabel.Text = this.GetTranslatedText("Tail ID:");
			this.ShipNumLabel.Text = this.GetTranslatedText("Ship Number:");
			this.ConsumerLabel.Text = this.GetTranslatedText("Consumer:");
			this.ProductLabel.Text = this.GetTranslatedText("Product:");
			this.FuelCardLabel.Text = this.GetTranslatedText("Fuel Card:");
			this.CardLabel.Text = this.GetTranslatedText("Card:");
		}

		protected void EquipmentTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			string redirectString = "EquipmentForm.aspx";

			try
			{
				((EquipmentForm)this.Page).UpdateData();

				Guid equipmentTypeGuid = Guid.Parse(this.EquipmentTypeDropDownList.SelectedValue);
				EquipmentTypeClass equipmentType;
				{
					equipmentType = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
																	 x =>
																	 x.Get(this.Security, equipmentTypeGuid)
																);
				}
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
				this.Equipment.Capacity = equipmentType.Capacity;
				this.Equipment.SafeFill = equipmentType.SafeFill;
				this.Equipment.ProductGuid = equipmentType.Product;
				{
					ProductClass prod = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(this.Security, this.Equipment.ProductGuid)
																);
					if (prod != null)
					{
						this.Equipment.ProductID = prod.ID;
					}
				}

				var fmForm = (FMFormBase)this.Page;
				if (fmForm.IsFromDispatch)
				{
					redirectString += "?DispatchEdit=" + fmForm.DispatchEntityGuid;
				}

				else if (fmForm.IsFromQueryWriter)
				{
					redirectString += "?QueryEdit=" + fmForm.QueryEntityGuid;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect(redirectString);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.IDTextbox.Text = this.Equipment.ID;
					this.DescriptionTextbox.Text = this.Equipment.Description;
					var equipmentTypeCollection = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security, null, null)
																);

					var newTypeItem = new ListItem(this.GetTranslatedText("{Unassigned}"), Guid.Empty.ToString());
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

					this.ProductTextBox.Text = this.Equipment.ProductID;
					this.FuelCardTextBox.Text = this.Equipment.FuelCardID;
					this.CardTextbox.Text = this.Equipment.TruckCardNumber;

					if (this.Equipment.CompanyID == "{Unassigned}")
					{
						this.CompanyTextBox.Text = this.GetTranslatedText("{Unassigned}");
					}
					else
					{
						this.CompanyTextBox.Text = this.Equipment.CompanyID;
					}

                    this.HiddenCheckBox.Checked = this.Equipment.HiddenDate.HasValue;

					if (this.Equipment == null)
					{
						return;
					}

					this.UpdateViewBasedOnEquipmentType();
					this.SetFieldAccessibilityForChildRecordVersion();

					if (this.EquipmentTypeDropDownList.SelectedItem.Text.ToUpper() == "AIRCRAFT")
					{
						this.Page.ClientScript.RegisterStartupScript(this.GetType(), "HelpKey", "var CurrentHelpKey =\"FMWebApp/EquipmentAirplaneGeneralPage.ascx\";", true);
					}
				}
				else
				{
					this.ShowByAllowFuelingByWeight();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ShowByAllowFuelingByWeight()
		{
			this.AllowFuelingByWeight =
				FMChannelHelper.MakeCall<IEquipmentTypes, bool>(
						x =>
						x.Get(this.Security, x.GetIdentityGuid(this.Security, this.Equipment.TypeClass)).AllowFuelingByWeight
				);

			bool aVisible = !this.AllowFuelingByWeight;
			this.FuelCardLabel.Visible = aVisible;
			this.FuelCardTextBox.Visible = aVisible;
			this.CardLabel.Visible = aVisible;
			this.CardTextbox.Visible = aVisible;
		}

		protected void UpdateViewBasedOnEquipmentType()
		{
			this.MakeTextbox.Text = this.Equipment.Make;
			this.ModelTextbox.Text = this.Equipment.Model;
			this.YearTextbox.Text = (this.Equipment.Year == 0 ? "" : this.Equipment.Year.ToString());
			this.SerialNumberTextbox.Text = this.Equipment.SerialNumber;
			this.IDTextbox.Text = this.Equipment.ID;
			this.DescriptionTextbox.Text = this.Equipment.Description;
			this.CompanyTextBox.Text = this.Equipment.CompanyID;
			this.ProductTextBox.Text = this.Equipment.ProductID;
			this.FuelCardTextBox.Text = this.Equipment.FuelCardID;
			this.CardTextbox.Text = this.Equipment.TruckCardNumber;
			this.ApplyDataDictionary();
			this.ShowByAllowFuelingByWeight();
		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
			if (this.Equipment.IdentityGuid.Equals(Guid.Empty)
			    || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid))
			    || (this.VersionSpecificFields == null))
			{
				return;
			}
			this.IDTextbox.Enabled = (this.IDTextbox.Enabled && this.VersionSpecificFields.Contains("ID"));
			this.DescriptionTextbox.Enabled = (this.DescriptionTextbox.Enabled && this.VersionSpecificFields.Contains("Description"));
			this.EquipmentTypeDropDownList.Enabled = false;
				//Cannot change the Equipment Type of a child record version, because it is used as a filter field in the definition of the Equipment entity segment template.
			this.MakeTextbox.Enabled = (this.MakeTextbox.Enabled && this.VersionSpecificFields.Contains("Make"));
			this.ModelTextbox.Enabled = (this.ModelTextbox.Enabled && this.VersionSpecificFields.Contains("Model"));
			this.YearTextbox.Enabled = (this.YearTextbox.Enabled && this.VersionSpecificFields.Contains("Year"));
			this.SerialNumberTextbox.Enabled = (this.SerialNumberTextbox.Enabled && this.VersionSpecificFields.Contains("SerialNumber"));
			this.ProductTextBox.Enabled = (this.ProductTextBox.Enabled && this.VersionSpecificFields.Contains("Product"));
			this.FuelCardTextBox.Enabled = (this.FuelCardTextBox.Enabled && this.VersionSpecificFields.Contains("Fuel Card"));
			this.CardTextbox.Enabled = (this.CardTextbox.Enabled && this.VersionSpecificFields.Contains("TruckCardNumber"));
			this.CompanyTextBox.Enabled = (this.CompanyTextBox.Enabled && this.VersionSpecificFields.Contains("Company"));
            this.HiddenCheckBox.Enabled = this.HiddenCheckBox.Enabled && this.VersionSpecificFields.Contains("HiddenDate");
		}

		#endregion
	}
}