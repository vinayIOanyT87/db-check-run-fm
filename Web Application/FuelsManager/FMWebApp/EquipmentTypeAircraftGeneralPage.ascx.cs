// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentTypeAircraftGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentTypeAircraftGeneralPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// The equipment type aircraft general page.
	/// </summary>
	public partial class EquipmentTypeAircraftGeneralPage : FMUserControlBase
	{
		#region Properties
		/// <summary>
		/// Gets the equipment type.
		/// </summary>
		protected EquipmentTypeClass EquipmentType => ((EquipmentTypeDetailsForm)this.Page).EquipmentType;

	    #endregion

		#region Public Methods and Operators
		/// <summary>
		/// The update data.
		/// </summary>
		/// <exception cref="Exception">
		/// Invalid entry for Equipment Year exception.
		/// </exception>
		public void UpdateData()
		{
			var equipmentType = this.Session["SelectedEquipmentType"] as EquipmentTypeClass;

			if (equipmentType != null)
			{
				equipmentType.ID					= this.EquipmentTypeIDTextbox.Text;
				equipmentType.Model					= this.ModelTextbox.Text;
				equipmentType.Make					= this.MakeTextbox.Text;
				equipmentType.CustomerDesignator	= this.CustomerDesignatorTextBox.Text;
				equipmentType.ServiceTime			= this.ServiceTimeTextbox.Text;
				equipmentType.AllowFuelingByWeight	= this.AllowFuelingByWeightCheckbox.Checked;
				equipmentType.VolumeDecimalPlaces	= this.VolumeDecimalPlacesTextBox.Text;
				equipmentType.MassDecimalPlaces		= this.MassDecimalPlacesTextBox.Text;
				equipmentType.Description			= this.DescriptionTextbox.Text;

				try
				{
					if (this.YearTextbox.Text.Trim() == string.Empty)
					{
						equipmentType.Year = 0;
					}
					else
					{
						equipmentType.Year = Convert.ToInt32(this.YearTextbox.Text);
					}
				}
				catch
				{
					throw new Exception("Invalid entry for Equipment Year.");
				}

				equipmentType.Attribute							= (EQUIPMENT_TYPE)Convert.ToInt32(this.AttributeDropDownList.SelectedValue);
				equipmentType.CompanyRoleAssignmentConstraint	= (COMPANY_ROLE)Convert.ToInt32(this.CompanyRoleDropDownList.SelectedValue);
				equipmentType.MassUnits							= (EngineeringUnit)Convert.ToInt32(this.MassUnitsDropDownList.SelectedValue);
				equipmentType.VolumeUnits						= (EngineeringUnit)Convert.ToInt32(this.VolumeUnitsDownList.SelectedValue);

				equipmentType.FuelServiceToleranceMaxType = this.rbFuelServiceMaxVolueAlias.Checked ? TOLERANCE_TYPE.Volume : TOLERANCE_TYPE.Mass;

				if (this.rbFuelServiceToleranceVolumeAlias.Checked)
				{
					equipmentType.FuelServiceToleranceType = TOLERANCE_TYPE.Volume;
				}
				else if (this.rbFuelServiceTolerancePercentageAlias.Checked)
				{
					equipmentType.FuelServiceToleranceType = TOLERANCE_TYPE.Percentage;
				}
				else
				{
					equipmentType.FuelServiceToleranceType = TOLERANCE_TYPE.Mass;
				}

				if (this.rbTankToTankVoluemAlias.Checked)
				{
					equipmentType.TankToTankToleranceType = TOLERANCE_TYPE.Volume;
				}
				else if (this.rbTankToTankPercentageAlias.Checked)
				{
					equipmentType.TankToTankToleranceType = TOLERANCE_TYPE.Percentage;
				}
				else
				{
					equipmentType.TankToTankToleranceType = TOLERANCE_TYPE.Mass;
				}

				if (this.rbWingToWingVolumeAlias.Checked)
				{
					equipmentType.WingToWingToleranceType = TOLERANCE_TYPE.Volume;
				}
				else if (this.rbWingToWingPercentageAlias.Checked)
				{
					equipmentType.WingToWingToleranceType = TOLERANCE_TYPE.Percentage;
				}
				else
				{
					equipmentType.WingToWingToleranceType = TOLERANCE_TYPE.Mass;
				}

				equipmentType.WingToWingToleranceValue = this.WingToWingValueTextBox.Text;
				equipmentType.TankToTankToleranceValue = this.TankToTankValueTextBox.Text;
				equipmentType.FuelServiceToleranceValue = this.FuelServiceToleranceTextBox.Text;
				equipmentType.FuelServiceToleranceMaxValue = this.FuelServiceMaxTextBox.Text;

				if (this.ProductDropDownList.SelectedIndex <= 0)
				{
					equipmentType.Product = Guid.Empty;
				}
				else
				{
					equipmentType.Product = Guid.Parse(this.ProductDropDownList.SelectedItem.Value);
				}
			}
		}
		#endregion


		#region Methods
		/// <summary>
		/// The allow fueling by weight checkbox checked changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void AllowFuelingByWeightCheckboxCheckedChanged(object sender, EventArgs e)
		{
			this.UpdateData();
			this.UpdateView(false);
		}

		/// <summary>
		/// The apply data dictionary.
		/// </summary>
		protected void ApplyDataDictionary()
		{
			this.ToleranceLabel.Text						= this.GetTranslatedText("Tolerance:");
			this.MaxTypeLabel.Text							= this.GetTranslatedText("Max Type:");
			this.MaxLabel.Text								= this.GetTranslatedText("Max:");
			this.EquipmentClassLabel.Text					= this.GetTranslatedText("Equipment Class:");
			this.EquipmentTypeIdLabel.Text					= this.GetTranslatedText("Equipment Type ID:");
			this.DescriptionLabel.Text						= this.GetTranslatedText("Description:");
			this.ModelLabel.Text							= this.GetTranslatedText("Model:");
			this.MakeLabel.Text								= this.GetTranslatedText("Make:");
			this.YearLabel.Text								= this.GetTranslatedText("Year:");
			this.CustomerDesignatorLabel.Text				= this.GetTranslatedText("Customer Designator:");
			this.ServiceTimeLabel.Text						= this.GetTranslatedText("Service Time:");
			this.UnitsLabel.Text							= this.GetTranslatedText("Units:");
			this.DecimalPlacesLabel.Text					= this.GetTranslatedText("Decimal Places:");
			this.MassLabel.Text								= this.GetTranslatedText("Mass:");
			this.VolumeLabel.Text							= this.GetTranslatedText("Volume:");
			this.ProductLabel.Text							= this.GetTranslatedText("Product:");
			this.WingToWingToleranceLabel.Text				= this.GetTranslatedText("Wing-To-Wing Tolerance:");
			this.TankToTankToleranceLabel.Text				= this.GetTranslatedText("Tank-To-Tank Tolerance:");
			this.FuelServiceToleranceLabel.Text				= this.GetTranslatedText("Fuel Service Tolerance:");
			this.rbWingToWingMassAlias.Text					= this.GetTranslatedText("Mass");
			this.rbWingToWingVolumeAlias.Text				= this.GetTranslatedText("Volume");
			this.rbWingToWingPercentageAlias.Text			= this.GetTranslatedText("Percentage");
			this.rbTankToTankMassAlias.Text					= this.GetTranslatedText("Mass");
			this.rbTankToTankVoluemAlias.Text				= this.GetTranslatedText("Volume");
			this.rbTankToTankPercentageAlias.Text			= this.GetTranslatedText("Percentage");
			this.rbFuelServiceToleranceMassAlias.Text		= this.GetTranslatedText("Mass");
			this.rbFuelServiceToleranceVolumeAlias.Text		= this.GetTranslatedText("Volume");
			this.rbFuelServiceTolerancePercentageAlias.Text = this.GetTranslatedText("Percentage");
			this.rbFuelServiceMaxMassAlias.Text				= this.GetTranslatedText("Mass");
			this.rbFuelServiceMaxVolueAlias.Text			= this.GetTranslatedText("Volume");
		}

		/// <summary>
		/// The attribute drop down list selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void AttributeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			const string RedirectString = "EquipmentTypeDetailsForm.aspx";

			try
			{
				((EquipmentTypeDetailsForm)this.Page).UpdateData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect(RedirectString);
		}

		/// <summary>
		/// The enumerate products.
		/// </summary>
		protected void EnumerateProducts()
		{
			var productItems = new ListItemCollection();

			ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
			foreach (ProductClass product in productCollection)
			{
				if (product.ProductType == ProductType.AdditiveProduct)
				{
					continue;
				}

				var newProductItem = new ListItem(product.ID, product.MasterRecordGuid.ToString());

				foreach (ListItem existingProductItem in productItems)
				{
					if (string.Compare(existingProductItem.Text, newProductItem.Text, StringComparison.Ordinal) > 0)
					{
						int index = productItems.IndexOf(existingProductItem);
						productItems.Insert(index, newProductItem);
						newProductItem = null;
						break;
					}
				}

				if (newProductItem != null)
				{
					productItems.Add(newProductItem);
				}
			}
			
			foreach (ListItem productDropDownItem in productItems)
			{
				this.ProductDropDownList.Items.Add(productDropDownItem);
			}

			if (this.EquipmentType.Product != Guid.Empty)
			{
				string productGuidStr = this.EquipmentType.Product.ToString();

				foreach (ListItem prdct in this.ProductDropDownList.Items)
				{
					if (prdct.Value == productGuidStr)
					{
						this.ProductDropDownList.SelectByText(prdct.Text);
						break;
					}
				}
			}
		}

		/// <summary>
		/// The equipment type ID textbox text changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void EquipmentTypeIDTextboxTextChanged(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// The mass units drop down list selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void MassUnitsDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.VolumeUnitsDownListSelectedIndexChanged(sender, e);
		}

		/// <summary>
		/// The page load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.Page.IsPostBack)
			{
				this.UpdateView(true);
				this.EquipmentTypeIDTextbox.Focus();
			}
		}

		/// <summary>
		/// The set fueling by weight visibility.
		/// </summary>
		/// <param name="visible">
		/// The a visible.
		/// </param>
		protected void SetFuelingByWeightVisibility(bool visible)
		{
			this.TankToTankTolerancePanel.Visible = visible;
			this.WingToWingTolerancePanel.Visible = visible;
			this.FuelServicePanel.Visible = visible;
			this.ToleranceLabel.Visible = visible;
			this.MaxTypeLabel.Visible = visible;
			this.MaxLabel.Visible = visible;
			this.EquipmentClassLabel.Visible = visible;
			this.UnitsLabel.Visible = visible;
			this.DecimalPlacesLabel.Visible = visible;
			this.MassLabel.Visible = visible;
			this.VolumeLabel.Visible = visible;
			this.WingToWingToleranceLabel.Visible = visible;
			this.TankToTankToleranceLabel.Visible = visible;
			this.FuelServiceToleranceLabel.Visible = visible;
			this.rbWingToWingMassAlias.Visible = visible;
			this.rbWingToWingVolumeAlias.Visible = visible;
			this.rbWingToWingPercentageAlias.Visible = visible;
			this.rbTankToTankMassAlias.Visible = visible;
			this.rbTankToTankVoluemAlias.Visible = visible;
			this.rbTankToTankPercentageAlias.Visible = visible;
			this.rbFuelServiceToleranceMassAlias.Visible = visible;
			this.rbFuelServiceToleranceVolumeAlias.Visible = visible;
			this.rbFuelServiceTolerancePercentageAlias.Visible = visible;
			this.rbFuelServiceMaxMassAlias.Visible = visible;
			this.rbFuelServiceMaxVolueAlias.Visible = visible;
			this.VolumeDecimalPlacesTextBox.Visible = visible;
			this.MassDecimalPlacesTextBox.Visible = visible;
			this.MassUnitsDropDownList.Visible = visible;
			this.VolumeUnitsDownList.Visible = visible;
			this.rbFuelServiceToleranceVolumeAlias.Visible = visible;
			this.rbFuelServiceTolerancePercentageAlias.Visible = visible;
			this.rbFuelServiceToleranceMassAlias.Visible = visible;
			this.rbFuelServiceMaxVolueAlias.Visible = visible;
			this.rbFuelServiceMaxMassAlias.Visible = visible;
			this.rbTankToTankVoluemAlias.Visible = visible;
			this.rbTankToTankPercentageAlias.Visible = visible;
			this.rbTankToTankMassAlias.Visible = visible;
			this.rbWingToWingVolumeAlias.Visible = visible;
			this.rbWingToWingPercentageAlias.Visible = visible;
			this.rbWingToWingMassAlias.Visible = visible;
			this.WingToWingValueTextBox.Visible = visible;
			this.TankToTankValueTextBox.Visible = visible;
			this.FuelServiceToleranceTextBox.Visible = visible;
			this.FuelServiceMaxTextBox.Visible = visible;
		}

		/// <summary>
		/// The update view.
		/// </summary>
		/// <param name="populateDropDowns">
		/// The populate drop downs.
		/// </param>
		protected void UpdateView(bool populateDropDowns)
		{
			this.EquipmentTypeIDTextbox.Text		= this.EquipmentType.ID;
			this.DescriptionTextbox.Text			= this.EquipmentType.Description;
			this.ModelTextbox.Text					= this.EquipmentType.Model;
			this.MakeTextbox.Text					= this.EquipmentType.Make;
			this.YearTextbox.Text					= this.EquipmentType.Year == 0 ? string.Empty : this.EquipmentType.Year.ToString();
			this.CustomerDesignatorTextBox.Text		= this.EquipmentType.CustomerDesignator;
			this.ServiceTimeTextbox.Text			= this.EquipmentType.ServiceTime;
			this.VolumeDecimalPlacesTextBox.Text	= this.EquipmentType.VolumeDecimalPlaces;
			this.MassDecimalPlacesTextBox.Text		= this.EquipmentType.MassDecimalPlaces;

			if (populateDropDowns)
			{
				this.EnumerateProducts();

				for (EQUIPMENT_TYPE i = 0; i < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; i++)
				{
					if (EQUIPMENT_TYPE.COMPARTMENT_TYPE == i)
					{
						continue;
					}
					this.AttributeDropDownList.Items.Add(new ListItem(this.GetTranslatedText(EquipmentTypeClass.TypeID(i)), ((int)i).ToString()));
				}

				this.AttributeDropDownList.SelectedValue = ((int) this.EquipmentType.Attribute).ToString();

				for (COMPANY_ROLE role = 0; role < COMPANY_ROLE.MAX_COMPANY_ROLE; role++)
				{
					if (role == COMPANY_ROLE.MAX_COMPANY_ROLE)
					{
						continue;
					}

					this.CompanyRoleDropDownList.Items.Add(new ListItem(this.GetTranslatedText(CompanyRoleMapClass.RoleID(role)),((int) role).ToString()));

					if (role == this.EquipmentType.CompanyRoleAssignmentConstraint)
					{
						this.CompanyRoleDropDownList.SelectedIndex = this.CompanyRoleDropDownList.Items.Count - 1;
					}
				}

				this.CompanyRoleDropDownList.Items.Insert(0, new ListItem(this.GetTranslatedText("{Any}"), ((int)COMPANY_ROLE.MAX_COMPANY_ROLE).ToString()));

				// Need to replace ENGINEERING_UNIT.FMM_Gram with selection if available
				this.InitializeUnitsDropDownList(
					this.MassUnitsDropDownList, EngineeringUnit.FmmGram, EngineeringUnit.FmmMlbs, this.EquipmentType.MassUnits);

				// Need to replace ENGINEERING_UNIT.FMV_CM3 with selection if available
				this.InitializeUnitsDropDownList(
					this.VolumeUnitsDownList, EngineeringUnit.FmvCm3, EngineeringUnit.FmvKl, this.EquipmentType.VolumeUnits);
			}

			switch (this.EquipmentType.FuelServiceToleranceType)
			{
				case TOLERANCE_TYPE.Volume:
					this.rbFuelServiceToleranceVolumeAlias.Checked = true;
					break;
				case TOLERANCE_TYPE.Percentage:
					this.rbFuelServiceTolerancePercentageAlias.Checked = true;
					break;
				default:
					this.rbFuelServiceToleranceMassAlias.Checked = true;
					break;
			}

			switch (this.EquipmentType.FuelServiceToleranceMaxType)
			{
				case TOLERANCE_TYPE.Volume:
					this.rbFuelServiceMaxVolueAlias.Checked = true;
					break;
				default:
					this.rbFuelServiceMaxMassAlias.Checked = true;
					break;
			}

			switch (this.EquipmentType.TankToTankToleranceType)
			{
				case TOLERANCE_TYPE.Volume:
					this.rbTankToTankVoluemAlias.Checked = true;
					break;
				case TOLERANCE_TYPE.Percentage:
					this.rbTankToTankPercentageAlias.Checked = true;
					break;
				default:
					this.rbTankToTankMassAlias.Checked = true;
					break;
			}

			switch (this.EquipmentType.WingToWingToleranceType)
			{
				case TOLERANCE_TYPE.Volume:
					this.rbWingToWingVolumeAlias.Checked = true;
					break;
				case TOLERANCE_TYPE.Percentage:
					this.rbWingToWingPercentageAlias.Checked = true;
					break;
				default:
					this.rbWingToWingMassAlias.Checked = true;
					break;
			}

			this.WingToWingValueTextBox.Text			= this.EquipmentType.WingToWingToleranceValue;
			this.TankToTankValueTextBox.Text			= this.EquipmentType.TankToTankToleranceValue;
			this.FuelServiceToleranceTextBox.Text		= this.EquipmentType.FuelServiceToleranceValue;
			this.FuelServiceMaxTextBox.Text				= this.EquipmentType.FuelServiceToleranceMaxValue;
			this.AllowFuelingByWeightCheckbox.Checked	= this.EquipmentType.AllowFuelingByWeight;

			this.SetFuelingByWeightVisibility(this.EquipmentType.AllowFuelingByWeight);
			this.ApplyDataDictionary();

			this.rbFuelServiceMaxVolueAlias.Enabled = this.rbFuelServiceTolerancePercentageAlias.Checked;
			this.rbFuelServiceMaxMassAlias.Enabled	= this.rbFuelServiceTolerancePercentageAlias.Checked;
			this.FuelServiceMaxTextBox.Enabled		= this.rbFuelServiceTolerancePercentageAlias.Checked;
		}

		/// <summary>
		/// The volume units down list selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void VolumeUnitsDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateData();
			this.UpdateView(false);
		}
		#endregion

		/// <summary>
		/// The fuel service tolerance percentage alias checked changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void RbFuelServiceTolerancePercentageAliasCheckedChanged(object sender, EventArgs e)
		{
			this.UpdateData();
			this.UpdateView(false);
		}

		/// <summary>
		/// The fuel service tolerance volume alias checked changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void RbFuelServiceToleranceVolumeAliasCheckedChanged(object sender, EventArgs e)
		{
			this.UpdateData();
			this.UpdateView(false);
		}

		/// <summary>
		/// The fuel service tolerance mass alias checked changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void RbFuelServiceToleranceMassAliasCheckedChanged(object sender, EventArgs e)
		{
			this.UpdateData();
			this.UpdateView(false);
		}

		public void SetReadOnly()
		{
			DisableControls();
		}
	}
}