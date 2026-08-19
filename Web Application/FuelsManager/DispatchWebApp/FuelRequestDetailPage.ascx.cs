// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelRequestDetailPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Represents the Detail tab on the Fuel Request Form.
// The Detail tab contains information about the service vehicle
// involved in the request and the times that key events in the lifecycle of
// the request occurred.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	/// <summary>
	/// Represents the Detail tab on the Fuel Request Form.
	/// The Detail tab contains information about the service vehicle
	/// involved in the request and the times that key events in the lifecycle of
	/// the request occurred.
	/// </summary>
	public partial class FuelRequestDetailPage : FuelRequestFormPageBase
	{
		#region Page Properties
		/// <summary>
		/// Identifies the equipment selected on the detail tab. This is the vehicle providing the fueling service
		/// </summary>
		public Guid EquipmentGuid
		{
			get
			{
				Guid equipmentGuid;

				if (this.RegistrationIDComboBox.SelectedItem != null && Guid.TryParse(this.RegistrationIDComboBox.SelectedItem.Value, out equipmentGuid))
				{
					return equipmentGuid;
				}

				return Guid.Empty;
			}
		}

		/// <summary>
		/// Gets the equipment container on the detail tab. This is the vehicle providing the fueling service
		/// </summary>
		public FMComboBox DetailRegistrationIDComboBox
		{
			get
			{
				return this.RegistrationIDComboBox;
			}
		}

		/// <summary>
		/// Gets and sets the quantity value in the quantity text box.
		/// </summary>
		public string Quantity
		{
			get { return this.QuantityTextBox.Text; }
			set { this.QuantityTextBox.Text = value; }
		}

		public bool QuantityEnabled
		{
			set { this.QuantityTextBox.Enabled = value; }
			get { return this.QuantityTextBox.Enabled; }
		}
		#endregion

		#region Page Methods
		/// <summary>
		/// Update the variance displayed on the form using data
		/// from the fueling vehicle record
		/// </summary>
		/// <param name="transaction">Contains transaction data used when calculating the variance</param>
		public bool UpdateVariance(TransactionDO transaction)
		{
			string quantity = string.Empty;

			if (this.QuantityTextBox.Enabled && !string.IsNullOrEmpty(this.QuantityTextBox.Text))
			{
				quantity = this.QuantityTextBox.Text;
			}

			if (this.RegistrationIDComboBox.SelectedItem == null
				|| string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text)
				|| string.IsNullOrEmpty(quantity)
				|| FuelRequestFormSession.SessionFuelRequestSubType == FuelRequestSR.PartialFillRequestSubType)
			{
				this.DifferentialPressureAndVarianceTextBox.Text = string.Empty;
				return true;
			}

			Guid equipmentGuid;
			Guid.TryParse(this.RegistrationIDComboBox.SelectedItem.Value, out equipmentGuid);

			if (equipmentGuid == Guid.Empty)
			{
				this.DifferentialPressureAndVarianceTextBox.Text = string.Empty;
				return true;
			}

			// Get the equipment identified in the Registration ID control
			EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(equipments => equipments.Get(this.Security, equipmentGuid));

			double equipmentCapacity;
			double equipmentVolume;
			double quantityVolume;

			// Make sure the quantities we'll be working with are actual numbers
			if (!double.TryParse(equipment.Capacity, out equipmentCapacity))
			{
				this.ParentForm.ShowAlert("Invalid equipment capacity");
				this.DifferentialPressureAndVarianceTextBox.Text = string.Empty;
				return false;
			}

			if (!double.TryParse(equipment.Volume, out equipmentVolume))
			{
				this.ParentForm.ShowAlert("Invalid equipment volume");
				this.DifferentialPressureAndVarianceTextBox.Text = string.Empty;
				return false;
			}

			if (!double.TryParse(quantity, out quantityVolume))
			{
				this.ParentForm.ShowAlert("Invalid quantity");
				this.DifferentialPressureAndVarianceTextBox.Text = string.Empty;
				return false;
			}

			double variance;

			LineItemDO lineItem = transaction.LineItems.Find((matchingLineItem) => matchingLineItem.DeleteFlag == false);

			if (transaction.TransactionGuid == Guid.Empty
				|| transaction.Number01 == null
				|| lineItem.Quantity == null)
			{
				if (FuelRequestFormSession.SessionFuelRequestSubType == FuelRequestSR.ReturnToBulkRequestSubType)
				{
					// If it's a return to bulk the variance is difference between the volume added
					// and the volume of the equipment
					variance = quantityVolume - equipmentVolume;
				}
				else
				{
					// if the volume added will fill above capacity then we do a different calculation
					if ((equipmentVolume + quantityVolume) > equipmentCapacity)
					{
						variance = equipmentCapacity - (equipmentVolume + quantityVolume);
					}
					else
					{
						// the variance is the volume of the equipment and volume added subtracted from the capacity of the equipment
						variance = equipmentCapacity - equipmentVolume - quantityVolume;
					}
				}
			}
			else
			{
				variance = transaction.Number01.Value;
				double previousVolume = lineItem.Quantity.Gross;

				variance += quantityVolume - previousVolume;
			}

			this.DifferentialPressureAndVarianceTextBox.Text = variance.ToString(FuelRequestFormSession.SessionSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

			return true;
		}
		#endregion

		#region Page Events
		/// <summary>
		/// Fires when the page loads. If it's not a post back, display the transaction
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					bool isFuelRequest = this.ParentForm.IsFuelRequest;

					// We display either a "Differential Pressure" label or a "Variance" label on the tab, depending
					// on the type of request. If it's a refuel or defuel, we display the Differential Pressure,
					// if it's a Fill, Partial Fill, or Return to Bulk we display the Variance
					this.DifferentialPressureLabel.Visible = isFuelRequest;
					this.VarianceLabel.Visible = !isFuelRequest;

					// The quantity text box is only enabled when completing a transaction or for fast logs
					this.QuantityTextBox.Enabled = FuelRequestFormSession.SessionCompletingTransaction || this.ParentForm.IsFastLogOrFastLogFillStand;

					// The detail tab's registration id is only visible for refuel and defuel requests. 
					// That's because the fueling vehicle indicated by the registration ID is already displayed on the fill stand Service Request tab
					this.RegistrationIDComboBox.Enabled = isFuelRequest;

					this.PopulateControls();

					this.DisplayTransaction(FuelRequestFormSession.SessionTransaction);

					// If the user wants to complete the transaction, set focus on the quantity, and empty the field
					// so the user can enter a new quantity. 
					if (FuelRequestFormSession.SessionCompletingTransaction)
					{
						this.QuantityTextBox.Text = "";
						this.ParentForm.SetFocusOnControl(this.QuantityTextBox);
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion

		#region Page Control Events
		/// <summary>
		/// When the quantity text box gets a new value,
		/// update the variance displayed on the tab.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void QuantityTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				// The variance is only displayed for fill stand requests
				if (!this.ParentForm.IsFuelRequest)
				{
					this.UpdateVariance(FuelRequestFormSession.SessionTransaction);
				}

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.QuantityTextBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the user picks a different equipment in the Registration ID control,
		/// Set the issue point values on the additional data tab.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void RegistrationIDComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.RegistrationIDComboBox.SelectedItem != null)
				{
					Guid equipmentGuid;

					if (Guid.TryParse(this.RegistrationIDComboBox.SelectedItem.Value, out equipmentGuid))
					{
						if (equipmentGuid != Guid.Empty)
						{
							EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
															equipments => equipments.Get(this.Security, equipmentGuid));

							this.ParentForm.IssuePoint = equipment.IssPt;
							this.ParentForm.IssuePointNumber = equipment.IssPtNum;
						}
						else
						{
							this.ParentForm.IssuePoint = string.Empty;
							this.ParentForm.IssuePointNumber = string.Empty;
						}
					}
				}

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.RegistrationIDComboBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}


		/// <summary>
		/// When the check box that represents whether the dispatched time should be ignored becomes checked,
		/// disable the dispatched time control. When it's unchecked, enable the control.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void IgnoreDispatchDateTimeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.DispatchDateTimeControl.Enabled = !this.IgnoreDispatchDateTimeCheckBox.Checked;
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the check box that represents whether the arrival time should be ignored becomes checked,
		/// disable the arrival time control. When it's unchecked, enable the control.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void IgnoreArrivalDateTimeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.ArrivalDateTimeControl.Enabled = !this.IgnoreArrivalDateTimeCheckBox.Checked;

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.IgnoreArrivalDateTimeCheckBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the check box that represents whether the start time should be ignored becomes checked,
		/// disable the start time control. When it's unchecked, enable the control.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void IgnoreStartDateTimeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.StartDateTimeControl.Enabled = !this.IgnoreStartDateTimeCheckBox.Checked;

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.IgnoreStartDateTimeCheckBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the check box that represents whether the stop time should be ignored becomes checked,
		/// disable the stop time control. When it's unchecked, enable the control.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void IgnoreStopDateTimeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.StopDateTimeControl.Enabled = !this.IgnoreStopDateTimeCheckBox.Checked;

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.IgnoreStopDateTimeCheckBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion

		#region Page Combo Box and Date Control Population
		/// <summary>
		/// Fill controls on the tab with initial values and combo box items
		/// </summary>
		private void PopulateControls()
		{
			this.PopulateOperator();
			this.PopulateRegistrationID();
			this.PopulateDateTimeControls();
		}

		/// <summary>
		/// Fill the operator combo box list with driver personnel
		/// </summary>
		public void PopulateOperator()
		{
			string previouslySelectedOperatorText = string.Empty;

			// If a value is selected, remember the value selected before we reload the records to display 
			// in the list so that we can select the same value again.
			if (this.OperatorComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.OperatorComboBox.SelectedItem.Text))
			{
				previouslySelectedOperatorText = this.OperatorComboBox.SelectedItem.Text;
			}

			this.OperatorComboBox.SelectedIndex = -1;
			this.OperatorComboBox.DataSource = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(personnel => personnel.EnumerateByRole(this.Security, PERSON_ROLE.LOADER_ROLE));

			this.OperatorComboBox.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
			
			this.OperatorComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.OperatorComboBox);

			if (!string.IsNullOrEmpty(previouslySelectedOperatorText))
			{
				this.OperatorComboBox.SelectByText(previouslySelectedOperatorText);
			}
			else
			{
				this.OperatorComboBox.SelectedValue = Guid.Empty.ToString();
			}
		}

		/// <summary>
		/// Fill the registration id combo box control with fueling equipment
		/// </summary>
		public void PopulateRegistrationID()
		{
			string previouslySelectedText = string.Empty;
			bool source = true;		

			if (this.ParentForm.RequestType == FuelRequestType.FastLogFillStand 
				|| this.ParentForm.RequestType == FuelRequestType.FillStand)
			{
				if ((FuelRequestFormSession.SessionFuelRequestSubType != FuelRequestSR.ReturnToBulkRequestSubType) && (FuelRequestFormSession.SessionFuelRequestSubType != FuelRequestSR.PartialReturnToBulkSubType))
				{
					source = false;
				}
			}
			else if (FuelRequestFormSession.SessionFuelRequestSubType == FuelRequestSR.DefuelRequestSubType)
			{
				source = false;
			}

			// If a value is selected, remember the value selected before we reload the equipment records to display 
			// in the list so that we can select the same value again.
			if (this.RegistrationIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text))
			{
				previouslySelectedText = this.RegistrationIDComboBox.SelectedItem.Text;
			}

			var equipmentCollection = new EquipmentCollectionClass();
			bool isFuelRequest = this.ParentForm.IsFuelRequest;
			var equipmentSecondaryStorageCollectionCache = new EquipmentCollectionClass();

			var dataSet = FMChannelHelper.MakeCall<IEquipments, DataSet>(
				x => x.EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(this.Security, null, null, null, null, true));

			this.LoadEquipment(dataSet, equipmentSecondaryStorageCollectionCache);

			if (this.ParentForm.SelectedProduct != Guid.Empty)
			{
				var query = equipmentSecondaryStorageCollectionCache;

				//specific one
				query.Where(dr => dr.ProductGuid == this.ParentForm.SelectedProduct);

				foreach (EquipmentClass eq in query)
				{
					equipmentCollection.Add(eq);
				}
			}
			else
			{
				foreach (EquipmentClass eq in equipmentSecondaryStorageCollectionCache)
				{
					equipmentCollection.Add(eq); //add all back
				}
			}

			// If there is a selected item, make sure it is in the list even if it does
			// not meet the criteria.  The user may have dispatched the equipment despite
			// warnings about the equipment not meeting the request parameters.
			if (string.IsNullOrEmpty(previouslySelectedText) == false)
			{
				if (equipmentCollection.Find(x => x.ID == previouslySelectedText) == null)
				{
					Guid equipmentGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.GetIdentityGuid(this.Security, previouslySelectedText));

					if (equipmentGuid != Guid.Empty)
					{
						EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, equipmentGuid));
						equipmentCollection.Add(equipment);
					}
				}
			}
			else if (FuelRequestFormSession.SessionTransaction != null)
			{
				EquipmentDO equipmentDO = (source) ? 
					FuelRequestFormSession.SessionTransaction.SourceEQ1 : FuelRequestFormSession.SessionTransaction.DestinationEQ1;

				if (equipmentDO != null)
				{
					EquipmentClass selectedItem = equipmentCollection.Find(x => x.MasterRecordGuid == equipmentDO.EquipmentGuid);

					// If we did not find the equipment in the list, we still want it to show up.  So, go
					// get the equipment and add it to the list
					if (selectedItem == null)
					{
						var equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
													x => x.Get(this.Security, equipmentDO.EquipmentGuid));

						if (equipment.MasterRecordGuid == equipmentDO.EquipmentGuid)
						{
							equipmentCollection.Add(equipment);
							previouslySelectedText = equipment.ID;
						}
					}
				}
			}

			List<EquipmentClass> sortedResults;

			if (isFuelRequest)
			{
				// sort the collection by ID
				sortedResults = equipmentCollection.OrderBy(equipment => equipment.ID).ToList();
			}
			else
			{
				// sort the collection by Xref
				sortedResults = equipmentCollection.OrderBy(equipment => equipment.Xref).ToList();
			}

			this.RegistrationIDComboBox.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;

			this.RegistrationIDComboBox.SelectedIndex = -1;
			this.RegistrationIDComboBox.DataSource = sortedResults;
			this.RegistrationIDComboBox.DataBind();

			this.AddBlankComboBoxEntry(this.RegistrationIDComboBox);

			if (!string.IsNullOrEmpty(previouslySelectedText))
			{
				this.RegistrationIDComboBox.SelectByText(previouslySelectedText);
			}
			else
			{
				this.RegistrationIDComboBox.SelectedValue = Guid.Empty.ToString();
			}
		}

		/// <summary>
		/// Fill the date time controls with their default value, which is right now.
		/// </summary>
		private void PopulateDateTimeControls()
		{
			SiteClass site = FuelRequestFormSession.SessionSite;

			DateTimeOffset siteDateTime = TimeConverter.Now(site);

			this.RequestDateTimeControl.FormatInfo = site.GetDateTimeFormatInfo();
			this.RequestDateTimeControl.Text = siteDateTime.ToString();

			this.DispatchDateTimeControl.FormatInfo = site.GetDateTimeFormatInfo();
			this.DispatchDateTimeControl.Text = siteDateTime.ToString();

			this.ArrivalDateTimeControl.FormatInfo = site.GetDateTimeFormatInfo();
			this.ArrivalDateTimeControl.Text = siteDateTime.ToString();

			this.StartDateTimeControl.FormatInfo = site.GetDateTimeFormatInfo();
			this.StartDateTimeControl.Text = siteDateTime.ToString();

			this.StopDateTimeControl.FormatInfo = site.GetDateTimeFormatInfo();
			this.StopDateTimeControl.Text = siteDateTime.ToString();

			this.CompletionDateTimeControl.FormatInfo = site.GetDateTimeFormatInfo();
			this.CompletionDateTimeControl.Text = siteDateTime.ToString();
			
			if (FuelRequestFormSession.SessionTransaction != null
			    && FuelRequestFormSession.SessionTransaction.TransactionGuid != Guid.Empty)
			{
				if (FuelRequestFormSession.SessionTransaction.RequestedDateTime != null)
				{
					this.RequestDateTimeControl.Text = FuelRequestFormSession.SessionTransaction.RequestedDateTime.Value.ToString();
				}

				if (FuelRequestFormSession.SessionTransaction.DispatchedDateTime != null)
				{
					this.DispatchDateTimeControl.Text = FuelRequestFormSession.SessionTransaction.DispatchedDateTime.Value.ToString();
				}

				if (FuelRequestFormSession.SessionTransaction.TimeIn != null)
				{
					this.ArrivalDateTimeControl.Text = FuelRequestFormSession.SessionTransaction.TimeIn.Value.ToString();
				}

				if (FuelRequestFormSession.SessionTransaction.RouteSchedule.FST != null)
				{
					this.StartDateTimeControl.Text = FuelRequestFormSession.SessionTransaction.RouteSchedule.FST.Value.ToString();
				}

				if (FuelRequestFormSession.SessionTransaction.TimeEnd != null)
				{
					this.StopDateTimeControl.Text = FuelRequestFormSession.SessionTransaction.TimeEnd.Value.ToString();
				}

				if (FuelRequestFormSession.SessionTransaction.TimeOut != null)
				{
					this.CompletionDateTimeControl.Text = FuelRequestFormSession.SessionTransaction.TimeOut.Value.ToString();
				}
			}

			this.IgnoreDispatchDateTimeCheckBox.Checked = false;
			this.DispatchDateTimeControl.Enabled = true;

			DispatchConfigurationClass dispatchConfiguration = FuelRequestFormSession.SessionDispatchConfiguration;

			this.IgnoreArrivalDateTimeCheckBox.Checked = !dispatchConfiguration.UseArrivalTime;
			this.ArrivalDateTimeControl.Enabled = dispatchConfiguration.UseArrivalTime;

			this.IgnoreStartDateTimeCheckBox.Checked = !dispatchConfiguration.UseStartTime;
			this.StartDateTimeControl.Enabled = dispatchConfiguration.UseStartTime;

			this.IgnoreStopDateTimeCheckBox.Checked = !dispatchConfiguration.UseStopTime;
			this.StopDateTimeControl.Enabled = dispatchConfiguration.UseStopTime;
		}
		#endregion

		#region Transaction Record Display and Creation
		/// <summary>
		/// Use the controls on the form to display data from a FuelsManager transaction record.
		/// </summary>
		/// <param name="transaction">The transaction record to display</param>
		public void DisplayTransaction(TransactionDO transaction)
		{
			LineItemDO lineItem = transaction.LineItems.Find((matchingLineItem) => matchingLineItem.DeleteFlag == false);

			// Detail Group
			if (lineItem != null && lineItem.Quantity != null)
			{
				this.QuantityTextBox.Text = lineItem.Quantity.Gross.ToString(FuelRequestFormSession.SessionSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
			}

			EquipmentDO fuelingVehicle = null;

			if (this.ParentForm.IsFuelRequest)
			{
				fuelingVehicle = (transaction.TransTypeID == FuelRequestForm.RefuelTransactionType) ? transaction.SourceEQ1 : transaction.DestinationEQ1;
			}
			else
			{
				fuelingVehicle = (transaction.TransTypeID == FuelRequestForm.FillStandTransactionType) ? transaction.DestinationEQ1 : transaction.SourceEQ1;
			}

			if (fuelingVehicle != null && fuelingVehicle.EquipmentGuid != Guid.Empty)
			{
				this.RegistrationIDComboBox.SelectedValue = fuelingVehicle.EquipmentGuid.ToString();
			}
			else
			{
				this.RegistrationIDComboBox.SelectedValue = Guid.Empty.ToString();
			}

			if (this.ParentForm.IsFuelRequest)
			{
				string differentialPressureText;
				transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_10, out differentialPressureText);
				this.DifferentialPressureAndVarianceTextBox.Text = differentialPressureText;
				this.DifferentialPressureAndVarianceTextBox.Enabled = true;
			}
			else
			{
				// if this is a fill stand request, we're displaying the variance rather than the differential pressure.
				this.DifferentialPressureAndVarianceTextBox.Text = transaction.Number01.HasValue ? transaction.Number01.ToString() : string.Empty;
				this.DifferentialPressureAndVarianceTextBox.Enabled = false;
			}

			this.RadioNumberTextBox.Text = transaction.RadioNumber;

			if (transaction.OperatorPersonnelGuid != Guid.Empty)
			{
				this.OperatorComboBox.SelectedValue = transaction.OperatorPersonnelGuid.ToString();
			}

			// Service History Group
			if (transaction.TransactionGuid != Guid.Empty)
			{
				this.IgnoreDispatchDateTimeCheckBox.Enabled = false;
				this.IgnoreArrivalDateTimeCheckBox.Enabled = false;
				this.IgnoreStartDateTimeCheckBox.Enabled = false;
				this.IgnoreStopDateTimeCheckBox.Enabled = false;

				if (transaction.DispatchedDateTime.HasValue)
				{
					this.IgnoreDispatchDateTimeCheckBox.Checked = false;
					this.DispatchDateTimeControl.Enabled = true;
					this.DispatchDateTimeControl.Text = transaction.DispatchedDateTime.ToString();
				}
				else
				{
					this.IgnoreDispatchDateTimeCheckBox.Checked = true;
					this.DispatchDateTimeControl.Enabled = false;
				}

				if (transaction.TimeIn.HasValue)
				{
					this.IgnoreArrivalDateTimeCheckBox.Checked = false;
					this.ArrivalDateTimeControl.Enabled = true;
					this.ArrivalDateTimeControl.Text = transaction.TimeIn.ToString();
				}
				else
				{
					this.IgnoreArrivalDateTimeCheckBox.Checked = true;
					this.ArrivalDateTimeControl.Enabled = false;
				}

				if (transaction.RouteSchedule.FST.HasValue)
				{
					this.IgnoreStartDateTimeCheckBox.Checked = false;
					this.StartDateTimeControl.Enabled = true;
					this.StartDateTimeControl.Text = transaction.RouteSchedule.FST.ToString();
				}
				else
				{
					this.IgnoreStartDateTimeCheckBox.Checked = true;
					this.StartDateTimeControl.Enabled = false;
				}

				if (transaction.TimeEnd.HasValue)
				{
					this.IgnoreStopDateTimeCheckBox.Checked = false;
					this.StopDateTimeControl.Enabled = true;
					this.StopDateTimeControl.Text = transaction.TimeEnd.ToString();
				}
				else
				{
					this.IgnoreStopDateTimeCheckBox.Checked = true;
					this.StopDateTimeControl.Enabled = false;
				}

				if (transaction.TimeOut.HasValue)
				{
					this.CompletionDateTimeControl.Text = transaction.TimeOut.ToString();
				}

				if (transaction.RequestedDateTime.HasValue)
				{
					this.RequestDateTimeControl.Text = transaction.RequestedDateTime.ToString();
				}
			}
		}

		/// <summary>
		/// Set values in a FuelsManager transaction record using data from the controls on the page
		/// </summary>
		/// <param name="transaction">The FuelsManager Transaction record to populate with data</param>
		public void SaveTransactionData(TransactionDO transaction)
		{
			LineItemDO lineItem = transaction.LineItems.Find((matchingLineItem) => matchingLineItem.DeleteFlag == false);

			transaction.RadioNumber = this.RadioNumberTextBox.Text;

			if (this.ParentForm.IsFuelRequest)
			{
				transaction.UserData10 = this.DifferentialPressureAndVarianceTextBox.Text;
			}
			else
			{
				double variance;

				if (double.TryParse(this.DifferentialPressureAndVarianceTextBox.Text, out variance))
				{
					transaction.Number01 = variance;
				}
				else
				{
					transaction.Number01 = null;
				}
			}

			double quantity;

			if (this.QuantityTextBox.Enabled && double.TryParse(this.QuantityTextBox.Text, out quantity))
			{
				lineItem.Quantity.Gross = quantity;
				lineItem.Quantity.Net = quantity;
				transaction.UserData1 = "U.S. Gallons";
			}

			// For refuels, the quantity should be recorded as a negative number. That's because we're dispensing fuel 
			// rather than receiving it.
			if (transaction.TransTypeID == FuelRequestForm.RefuelTransactionType)
			{
				lineItem.Quantity.Gross *= -1;
				lineItem.Quantity.Net *= -1;
			}

			DateTimeOffset requestedDateTime = this.RequestDateTimeControl.CurrentValue;

			transaction.RequestedDateTime = requestedDateTime;

			transaction.InventoryDate = requestedDateTime.Date;
			
			if (!this.IgnoreDispatchDateTimeCheckBox.Checked)
			{
				if (transaction.Status == TransactionStatus.Dispatched
					|| transaction.Status == TransactionStatus.Arrived
					|| transaction.Status == TransactionStatus.Started
					|| transaction.Status == TransactionStatus.Stopped
					|| transaction.Status == TransactionStatus.Completed)
				{
					transaction.DispatchedDateTime = this.DispatchDateTimeControl.CurrentValue;
				}
			}
			else
			{
				transaction.DispatchedDateTime = null;
			}

			if (!this.IgnoreArrivalDateTimeCheckBox.Checked)
			{
				if (transaction.Status == TransactionStatus.Arrived
					|| transaction.Status == TransactionStatus.Started
					|| transaction.Status == TransactionStatus.Stopped
					|| transaction.Status == TransactionStatus.Completed)
				{
					transaction.TimeIn = this.ArrivalDateTimeControl.CurrentValue;
				}
			}
			else
			{
				transaction.TimeIn = null;
			}

			if (!this.IgnoreStartDateTimeCheckBox.Checked)
			{
				if (transaction.Status == TransactionStatus.Started
					|| transaction.Status == TransactionStatus.Stopped
					|| transaction.Status == TransactionStatus.Completed)
				{
					transaction.RouteSchedule.FST = this.StartDateTimeControl.CurrentValue;
				}
			}
			else
			{
				transaction.RouteSchedule.FST = null;
			}

			if (!this.IgnoreStopDateTimeCheckBox.Checked)
			{
				if (transaction.Status == TransactionStatus.Stopped
					|| transaction.Status == TransactionStatus.Completed)
				{
					transaction.TimeEnd = this.StopDateTimeControl.CurrentValue;
				}
			}
			else
			{
				transaction.TimeEnd = null;
			}

			if (transaction.Status == TransactionStatus.Completed)
			{
				transaction.TimeOut = this.CompletionDateTimeControl.CurrentValue;

				// Set arrival time if the user has set to ignore. This will ensure the
				// time will not change when the page is reopened.
				if (this.IgnoreArrivalDateTimeCheckBox.Checked && transaction.TimeIn == null)
				{
					transaction.TimeIn = transaction.TimeOut;
				}

				// Set start time if the user has set to ignore. This will ensure the
				// time will not change when the page is reopened.
				if (this.IgnoreStartDateTimeCheckBox.Checked && transaction.RouteSchedule.FST == null)
				{
					transaction.RouteSchedule.FST = transaction.TimeOut;
				}

				// Set stop time if the user has set to ignore. This will ensure the
				// time will not change when the page is reopened.
				if (this.IgnoreStopDateTimeCheckBox.Checked && transaction.TimeEnd == null)
				{
					transaction.TimeEnd = transaction.TimeOut;
				}
			}

			transaction.OperatorID = string.Empty;
			transaction.OperatorName = string.Empty;
			transaction.OperatorPersonnelGuid = Guid.Empty;

			if (this.OperatorComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.OperatorComboBox.SelectedItem.Text))
			{
				transaction.OperatorID = this.OperatorComboBox.SelectedItem.Text;
				transaction.OperatorName = this.OperatorComboBox.SelectedItem.Text;
				transaction.OperatorPersonnelGuid = Guid.Empty;

				Guid personnelGuid;

				Guid.TryParse(this.OperatorComboBox.SelectedItem.Value, out personnelGuid);

				if (personnelGuid != Guid.Empty)
				{
					PersonClass theOperator = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(personnel => personnel.Get(this.Security, Guid.Parse(this.OperatorComboBox.SelectedItem.Value)));

					if (theOperator.IdentityGuid != Guid.Empty)
					{
						transaction.OperatorID = theOperator.ID;
						transaction.OperatorName = theOperator.FullName;
						transaction.OperatorPersonnelGuid = theOperator.MasterRecordGuid;
					}
				}
			}

			// Only save the Registration ID if this is a fuel request.
			// If this is a Fill Stand request, the registration ID box is a duplicate of the one on the Service Request tab
			// and the value is saved there.
			if (this.ParentForm.IsFuelRequest)
			{
				EquipmentDO equipmentDO = (transaction.TransTypeID == FuelRequestForm.RefuelTransactionType) 
													? transaction.SourceEQ1 : transaction.DestinationEQ1;
				EquipmentDO lineItemEquipmentDO = (transaction.TransTypeID == FuelRequestForm.RefuelTransactionType) 
													? lineItem.SourceEQ : lineItem.DestinationEQ;

				equipmentDO.EquipmentGuid = Guid.Empty;
				equipmentDO.RegistrationID = string.Empty;
				equipmentDO.EquipmentType = string.Empty;

				lineItemEquipmentDO.EquipmentGuid = Guid.Empty;
				lineItemEquipmentDO.RegistrationID = string.Empty;
				lineItemEquipmentDO.EquipmentType = string.Empty;

				if (this.RegistrationIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text))
				{
					equipmentDO.RegistrationID = this.RegistrationIDComboBox.SelectedItem.Text;
					lineItemEquipmentDO.RegistrationID = equipmentDO.RegistrationID;

					Guid equipmentGuid;

					Guid.TryParse(this.RegistrationIDComboBox.SelectedItem.Value, out equipmentGuid);

					if (equipmentGuid != Guid.Empty)
					{
						EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
															equipments => equipments.Get(this.Security, equipmentGuid));

						equipmentDO.SerialNumber = equipment.SerialNumber;
						equipmentDO.EquipmentType = equipment.EqTypeName;
						equipmentDO.EquipmentGuid = equipment.MasterRecordGuid;

						lineItemEquipmentDO.SerialNumber = equipment.SerialNumber;
						lineItemEquipmentDO.EquipmentType = equipment.EqTypeName;
						lineItemEquipmentDO.EquipmentGuid = equipment.MasterRecordGuid;
					}
				}
			}
		}
		#endregion

		#region Transaction Data Validation
		/// <summary>
		/// Apply any data validation checks to fields on the Detail tab page
		/// </summary>
		/// <param name="transaction">Represents the transaction object, which contains some information
		/// we need when applying edit rules</param>
		/// <returns>True if the checks pass and everything is OK. 
		/// False if a problem with the data was detected</returns>
		public bool ValidateTransactionData(TransactionDO transaction)
		{
			if (!this.CheckRequiredFields(transaction)
				|| !this.ValidateTimes())
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check to make sure required fields were provided and issue an alert if they weren't
		/// </summary>
		/// <param name="transaction">Represents the transaction object, which contains some information
		/// we need when applying edit rules</param>
		/// <returns>True if required fields were provided</returns>
		private bool CheckRequiredFields(TransactionDO transaction)
		{
			FuelRequestType requestType = this.ParentForm.RequestType;

			bool completingTransaction = FuelRequestFormSession.SessionCompletingTransaction;

			if (this.OperatorComboBox.SelectedItem == null || string.IsNullOrEmpty(this.OperatorComboBox.SelectedItem.Text))
			{
				if (this.ParentForm.IsFastLogOrFastLogFillStand
					|| transaction.Status == TransactionStatus.Dispatched
					|| transaction.Status == TransactionStatus.Arrived
					|| transaction.Status == TransactionStatus.Started
					|| transaction.Status == TransactionStatus.Stopped
					|| transaction.Status == TransactionStatus.Completed
					|| completingTransaction)
				{
					this.ParentForm.ShowAlert("Operator must be provided");
					this.ParentForm.SetFocusOnControl(this.OperatorComboBox);
					return false;
				}
			}

			if (this.ParentForm.IsFuelRequest)
			{
				if (this.RegistrationIDComboBox.SelectedItem == null || string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text))
				{
					if (requestType == FuelRequestType.FastLog
						|| transaction.Status == TransactionStatus.Dispatched
						|| transaction.Status == TransactionStatus.Arrived
						|| transaction.Status == TransactionStatus.Started
						|| transaction.Status == TransactionStatus.Stopped
						|| transaction.Status == TransactionStatus.Completed
						|| completingTransaction)
					{
						this.ParentForm.ShowAlert("Registration ID must be provided");
						this.ParentForm.SetFocusOnControl(this.RegistrationIDComboBox);
						return false;
					}
				}
			}

			double quantity = 0;
			if ((transaction.Status == TransactionStatus.Completed || completingTransaction) && !double.TryParse(this.QuantityTextBox.Text, out quantity))
			{
				this.ParentForm.ShowAlert("A valid Quantity must be provided when completing a transaction");
				this.ParentForm.SetFocusOnControl(this.QuantityTextBox);
				return false;
			}

			// Ignore quantity of zero if the Request Cancel Checkbox is checked. It is required to be 
			// zero in this case.  Otherwise, display an error.
			if (this.ParentForm.RequestCancelled == false)
			{
				if ((transaction.Status == TransactionStatus.Completed || completingTransaction) && quantity <= 0)
				{
					this.ParentForm.ShowAlert("Quantity must be greater than zero when completing a transaction");
					this.ParentForm.SetFocusOnControl(this.QuantityTextBox);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Make sure that the times provided on the tab are sequential 
		/// </summary>
		/// <returns>True if the times are sequential and everything is OK.
		/// False if there is a problem with the data</returns>
		public bool ValidateTimes()
		{
			Dictionary<string, FMDateTime> dateTimesToValidate = new Dictionary<string, FMDateTime>();

			dateTimesToValidate.Add("Request Time", this.RequestDateTimeControl);

			if (!this.IgnoreDispatchDateTimeCheckBox.Checked)
			{
				dateTimesToValidate.Add("Dispatch Time", this.DispatchDateTimeControl);
			}

			if (!this.IgnoreArrivalDateTimeCheckBox.Checked)
			{
				dateTimesToValidate.Add("Arrival Time", this.ArrivalDateTimeControl);
			}

			if (!this.IgnoreStartDateTimeCheckBox.Checked)
			{
				dateTimesToValidate.Add("Start Time", this.StartDateTimeControl);
			}

			if (!this.IgnoreStopDateTimeCheckBox.Checked)
			{
				dateTimesToValidate.Add("Stop Time", this.StopDateTimeControl);
			}

			if (!string.IsNullOrEmpty(this.CompletionDateTimeLabel.Text))
			{
				dateTimesToValidate.Add("Completion Time", this.CompletionDateTimeControl);
			}

			// The times are sequential - all must be equal to or later than the previous ones
			foreach (KeyValuePair<string, FMDateTime> dateTimeControl in dateTimesToValidate)
			{
				DateTimeOffset dateTimeValue;

				// Make sure the date time is a valid date. 
				if (!DateTimeOffset.TryParse(dateTimeControl.Value.Text, dateTimeControl.Value.FormatInfo, System.Globalization.DateTimeStyles.None, out dateTimeValue))
				{
					this.ParentForm.ShowAlert(dateTimeControl.Key + " must be provided and must be a valid date");
					this.ParentForm.SetFocusOnControl(dateTimeControl.Value);
					return false;
				}

				foreach (KeyValuePair<string, FMDateTime> earlierDateTimeControl in dateTimesToValidate)
				{
					if (dateTimeControl.Key == earlierDateTimeControl.Key)
					{
						break;
					}

					if (dateTimeControl.Value.CurrentValue < earlierDateTimeControl.Value.CurrentValue)
					{
						this.ParentForm.ShowAlert(dateTimeControl.Key + " must be later than " + earlierDateTimeControl.Key);
						this.ParentForm.SetFocusOnControl(dateTimeControl.Value);
						return false;
					}
				}
			}

			// The request date must be before the operational lock date 
			if (!this.ParentForm.IsFastLogOrFastLogFillStand && this.ParentForm.IsFuelRequest)
			{
				DateTimeOffset operationalLockDate = FuelRequestFormSession.SessionSite._OperationalLockDate.Value;

				if (this.RequestDateTimeControl.CurrentValue < operationalLockDate)
				{
					const string ErrorMessage = "Request date cannot be before current lock out date";

					this.ParentForm.ShowAlert(ErrorMessage);
					this.ParentForm.SetFocusOnControl(this.RequestDateTimeControl);
					return false;
				}
			}

			return true;
		}
		#endregion
	}
}