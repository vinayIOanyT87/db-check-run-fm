// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelRequestFillStandServiceRequestPage.ascx.cs" company="Varec, Inc.">
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

	/// <summary>
	/// <para>
	/// Represents the Service Request tab on the Fuel Request Form that is displayed
	/// for Fill Stand requests (Fill, Partial Fill, Return to Bulk). The tab contains information
	/// about the fuel service vehicle involved in the transaction.
	/// </para>
	/// <para>
	/// A different Service Request tab is displayed for Refuel or Defuel requests.
	/// </para>
	/// </summary>
	public partial class FuelRequestFillStandServiceRequestPage : FuelRequestFormPageBase
	{
		#region Page Properties

		/// <summary>
		/// The product (grade) selected on the fill stand Service Request tab page
		/// </summary>
		public Guid SelectedProduct
		{
			get
			{
				Guid productGuid;

				if (this.GradeComboBox.SelectedItem != null && Guid.TryParse(this.GradeComboBox.SelectedItem.Value, out productGuid))
				{
					return productGuid;
				}

				return Guid.Empty;
			}
		}

		/// <summary>
		/// Indicates whether the Request Cancelled check box is checked on the 
		/// Fill Stand Service Request tab page
		/// </summary>
		public bool RequestCancelled
		{
			get
			{
				return this.RequestCancelledCheckBox.Checked;
			}
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
					this.PopulateControls();

					this.DisplayTransaction(FuelRequestFormSession.SessionTransaction);
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
		/// When the ref code value is changed, filter the available values in the 
		/// Registration ID box to those with the selected Xref.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void RefCodeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				string selectedRefCode = string.Empty;
				string selectedRegistrationID = string.Empty;

				if (this.RefCodeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RefCodeComboBox.SelectedItem.Text))
				{
					selectedRefCode = this.RefCodeComboBox.SelectedItem.Text;
				}

				if (this.RegistrationIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text))
				{
					selectedRegistrationID = this.RegistrationIDComboBox.SelectedItem.Text;
				}

				EquipmentCollectionClass equipmentCollection = new EquipmentCollectionClass();

				EQUIPMENT_TYPE[] types = (EQUIPMENT_TYPE[])Enum.GetValues(typeof(EQUIPMENT_TYPE));

				var dataSet = FMChannelHelper.MakeCall<IEquipments, DataSet>(
						equipments => equipments.EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(this.Security, types, null, null, null, true));

				this.LoadEquipment(dataSet, equipmentCollection);

				this.RegistrationIDComboBox.SelectedIndex = -1;

				// If the user selected a ref code, filter the equipment records in the RegistrationID box to those with the ref code 
				// that was selected
				if (!string.IsNullOrEmpty(selectedRefCode))
				{
					List<EquipmentClass> filteredAircraft = equipmentCollection.FindAll(aircraft => aircraft.Xref == selectedRefCode);
					this.RegistrationIDComboBox.DataSource = filteredAircraft;
				}
				else
				{
					this.RegistrationIDComboBox.DataSource = equipmentCollection;
				}

				this.RegistrationIDComboBox.DataBind();
				this.AddBlankComboBoxEntry(this.RegistrationIDComboBox);

				if (!string.IsNullOrEmpty(selectedRegistrationID))
				{
					this.RegistrationIDComboBox.SelectByText(selectedRegistrationID);

					// If the equipment that was selected in the registration id combo box has been filtered out because it 
					// doesn't have the selected xref, the select above will fail. If it does, then we need to 
					// blank out any values derived from the record
					if (this.RegistrationIDComboBox.SelectedItem == null || string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text))
					{
						this.TypeTextBox.Text = string.Empty;
						this.ParentForm.IssuePoint = string.Empty;
						this.ParentForm.IssuePointNumber = string.Empty;
						this.ParentForm.DetailRegistrationIDComboBox.SelectedValue = Guid.Empty.ToString();
					}
				}
				else
				{
					this.TypeTextBox.Text = string.Empty;
					this.ParentForm.IssuePoint = string.Empty;
					this.ParentForm.IssuePointNumber = string.Empty;
					this.RegistrationIDComboBox.SelectedValue = Guid.Empty.ToString();
					this.ParentForm.DetailRegistrationIDComboBox.SelectedValue = Guid.Empty.ToString();
				}

				this.ParentForm.UpdateVariance(FuelRequestFormSession.SessionTransaction);

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.RefCodeComboBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When a value is selected in the registration id combo box, 
		/// select the corresponding values in the ref code combo box on this tab
		/// and the registration id combo box on the detail tab.
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void RegistrationIDComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.RegistrationIDComboBox.SelectedItem != null)
				{
					Guid equipmentGuid;

					if (Guid.TryParse(this.RegistrationIDComboBox.SelectedItem.Value, out equipmentGuid))
					{
						this.ParentForm.DetailRegistrationIDComboBox.SelectedValue = equipmentGuid.ToString();
					}
				}

				string equipmentType = string.Empty;
				string issuePoint = string.Empty;
				string issuePointNumber = string.Empty;

				if (this.RegistrationIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Value))
				{
					Guid equipmentGuid;
					Guid.TryParse(this.RegistrationIDComboBox.SelectedItem.Value, out equipmentGuid);

					if (equipmentGuid != Guid.Empty)
					{
						EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
													equipments => equipments.Get(this.Security, equipmentGuid));

						equipmentType = equipment.EqTypeName;
						issuePoint = equipment.IssPt;
						issuePointNumber = equipment.IssPtNum;

						if (!string.IsNullOrEmpty(equipment.Xref))
						{
							this.RefCodeComboBox.SelectByText(equipment.Xref);
						}
					}
				}

				this.TypeTextBox.Text = equipmentType;
				this.ParentForm.IssuePoint = issuePoint;
				this.ParentForm.IssuePointNumber = issuePointNumber;

				this.ParentForm.UpdateVariance(FuelRequestFormSession.SessionTransaction);

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.RegistrationIDComboBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the request type changes, update the variance and determine the request sub type
		/// (fill, partial fill, return to bulk)
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void RequestTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.ParentForm.DetermineTransactionAlias(this.RequestTypeComboBox.SelectedItem.Text);

				// We must re-populate the controls since some things depend on the type of request,
				// most importantly whether or not the combo boxes should allow manual entry of data
				this.PopulateControls();

				// Always update the variance so it gets removed for partial fills
				this.ParentForm.UpdateVariance(FuelRequestFormSession.SessionTransaction);

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.RequestTypeComboBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion

		#region Page Combo Box Control Population
		/// <summary>
		/// Fill controls on the tab with initial values and combo box items
		/// </summary>
		public void PopulateControls()
		{
			this.PopulateRegistrationID();
			this.PopulateGrade();
			this.PopulateLocation();
		}

		/// <summary>
		/// Populate the registration ID and ref code boxes with equipment records.
		/// </summary>
		public void PopulateRegistrationID()
		{
			string previouslySelectedRegistrationIDText = string.Empty;

			// If a value is selected, remember the value selected before we reload the equipment records to display 
			// in the list so that we can select the same value again.
			if (this.RegistrationIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text))
			{
				previouslySelectedRegistrationIDText = this.RegistrationIDComboBox.SelectedItem.Text;
			}

			var equipmentCollection = new EquipmentCollectionClass();

			var types = (EQUIPMENT_TYPE[])Enum.GetValues(typeof(EQUIPMENT_TYPE));

			var dataSet = FMChannelHelper.MakeCall<IEquipments, DataSet>(
				equipments => equipments.EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(this.Security, types, null, null, null, true));

			this.LoadEquipment(dataSet, equipmentCollection);

			var filteredXrefResults = new List<EquipmentClass>();

			var sortedByIDResults = equipmentCollection.OrderBy(equipment => equipment.ID).ToList<EquipmentClass>();

			// sort the collection by Xref and remove any duplicates
			var sortedByXrefResults = equipmentCollection.OrderBy(equipment => equipment.Xref).ToList<EquipmentClass>();

			foreach (EquipmentClass equipment in sortedByXrefResults)
			{
				if (!string.IsNullOrEmpty(equipment.Xref) 
					&& filteredXrefResults.Find(matchingEquipment => matchingEquipment.Xref == equipment.Xref) == null)
				{
					filteredXrefResults.Add(equipment);
				}
			}

			this.RefCodeComboBox.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
			this.RegistrationIDComboBox.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;

			this.RefCodeComboBox.SelectedIndex = -1;
			this.RefCodeComboBox.DataSource = filteredXrefResults;
			this.RefCodeComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.RefCodeComboBox);

			this.RegistrationIDComboBox.SelectedIndex = -1;
			this.RegistrationIDComboBox.DataSource = sortedByIDResults;
			this.RegistrationIDComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.RegistrationIDComboBox);

			if (!string.IsNullOrEmpty(previouslySelectedRegistrationIDText))
			{
				this.RegistrationIDComboBox.SelectByText(previouslySelectedRegistrationIDText);

				string equipmentType = string.Empty;
				string refID = string.Empty;

				if (this.RegistrationIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Value))
				{
					Guid equipmentGuid;
					Guid.TryParse(this.RegistrationIDComboBox.SelectedItem.Value, out equipmentGuid);

					if (equipmentGuid != Guid.Empty)
					{
						EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
														equipments => equipments.Get(this.Security, equipmentGuid));

						equipmentType = equipment.EqTypeName;
						refID = equipment.Xref;
					}
				}

				this.RefCodeComboBox.SelectByText(refID);
				this.TypeTextBox.Text = equipmentType;
			}
			else
			{
				this.RefCodeComboBox.SelectedValue = Guid.Empty.ToString();
				this.RegistrationIDComboBox.SelectedValue = Guid.Empty.ToString();
			}
		}

		/// <summary>
		/// Populate the location combo box with managed fill stand equipment records
		/// </summary>
		private void PopulateLocation()
		{
			string previouslySelectedLocationText = string.Empty;

			// If a value is selected, remember the value selected before we reload the equipment records to display 
			// in the list so that we can select the same value again.
			if (this.LocationComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.LocationComboBox.SelectedItem.Text))
			{
				previouslySelectedLocationText = this.LocationComboBox.SelectedItem.Text;
			}

			this.LocationComboBox.DataSource = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
													equipments => equipments.EnumerateByManagedFillstand(this.Security));

			this.LocationComboBox.SelectedIndex = -1;
			this.LocationComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.LocationComboBox);

			if (!string.IsNullOrEmpty(previouslySelectedLocationText))
			{
				this.LocationComboBox.SelectByText(previouslySelectedLocationText);
			}
			else
			{
				this.LocationComboBox.SelectedValue = Guid.Empty.ToString();
			}
		}

		public Guid LocationGuid
		{
			get
			{
				Guid locationGuid = Guid.Empty;

				if (this.LocationComboBox.SelectedItem != null && Guid.TryParse(this.LocationComboBox.SelectedItem.Value, out locationGuid))
				{
					return locationGuid;
				}

				return Guid.Empty;
			}

		}

		/// <summary>
		/// Fill the grade combo box with component products
		/// </summary>
		private void PopulateGrade()
		{
			string previouslySelectedGradeText = string.Empty;

			// If a value is selected, remember the value selected before we reload the records to display 
			// in the list so that we can select the same value again.
			if (this.GradeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.GradeComboBox.SelectedItem.Text))
			{
				previouslySelectedGradeText = this.GradeComboBox.SelectedItem.Text;
			}

			this.GradeComboBox.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;

			this.GradeComboBox.SelectedIndex = -1;

			this.GradeComboBox.DataSource = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
												products => products.EnumerateByType(this.Security, ProductType.ComponentProduct));

			this.GradeComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.GradeComboBox);

			if (!string.IsNullOrEmpty(previouslySelectedGradeText))
			{
				this.GradeComboBox.SelectByText(previouslySelectedGradeText);
			}
			else
			{
				this.GradeComboBox.SelectedValue = Guid.Empty.ToString();
			}
		}
		#endregion

		#region Transaction Record Display and Creation
		/// <summary>
		/// Use the controls on the form to display data from a FuelsManager transaction record.
		/// </summary>
		/// <param name="transaction">The transaction record to display</param>
		public void DisplayTransaction(TransactionDO transaction)
		{
			LineItemDO lineItem = transaction.LineItems.Find(matchingLineItem => matchingLineItem.DeleteFlag == false);

			// Disable the request type box for existing transactions
			if ((transaction.Status == TransactionStatus.Completed
				|| transaction.Status == TransactionStatus.Posted) && (transaction.TransactionGuid != Guid.Empty))
			{
				this.RequestTypeComboBox.Enabled = false;
			}

			this.RequestTypeComboBox.SelectByText(FuelRequestFormSession.SessionFuelRequestSubType);

			// Vehicle Group
			var fuelingVehicle = (transaction.TransTypeID == FuelRequestForm.FillStandTransactionType) 
										? transaction.DestinationEQ1 : transaction.SourceEQ1;

			if (fuelingVehicle != null && fuelingVehicle.EquipmentGuid != Guid.Empty)
			{
				this.RegistrationIDComboBox.SelectedValue = fuelingVehicle.EquipmentGuid.ToString();

				EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
												equipments => equipments.Get(this.Security, fuelingVehicle.EquipmentGuid));

				if (!string.IsNullOrEmpty(equipment.Xref))
				{
					this.RefCodeComboBox.SelectByText(equipment.Xref);
				}

				this.TypeTextBox.Text = equipment.TypeClass;
			}
			else
			{
				this.RegistrationIDComboBox.SelectedValue = Guid.Empty.ToString();
				this.RefCodeComboBox.SelectedValue = Guid.Empty.ToString();
			}

			if (lineItem != null && lineItem.ProductGuid != Guid.Empty)
			{
				this.GradeComboBox.SelectedValue = lineItem.ProductGuid.ToString();
			}

			string locationText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_07, out locationText);

			if (!string.IsNullOrEmpty(locationText))
			{
				this.LocationComboBox.SelectByText(locationText);
			}
			else
			{
				this.LocationComboBox.SelectedValue = Guid.Empty.ToString();
			}

			// Request Group
			this.RequestedByTextBox.Text = transaction.ContactSurname;

			if (transaction.TransactionGuid != Guid.Empty)
			{
				if (transaction.Status == TransactionStatus.Cancelled)
				{
					this.RequestCancelledCheckBox.Checked = true;
				}

				if (FuelRequestFormSession.SessionCompletingTransaction
					|| transaction.Status == TransactionStatus.Completed
					|| transaction.Status == TransactionStatus.Posted
					|| transaction.Status == TransactionStatus.Cancelled)
				{
					this.RequestCancelledCheckBox.Enabled = false;
				}
			}
			else
			{
				this.RequestCancelledCheckBox.Checked = false;
				this.RequestCancelledCheckBox.Enabled = false;
			}

			this.CommentsTextBox.Text = transaction.Notes;
		}

		/// <summary>
		/// Set values in a FuelsManager transaction record using data from the controls on the page
		/// </summary>
		/// <param name="transaction">The FuelsManager Transaction record to populate with data</param>
		/// <param name="product">Will be set to the product selected in the Grade combo box</param>
		public void SaveTransactionData(TransactionDO transaction, out ProductClass product)
		{
			product = null;

			LineItemDO lineItem = transaction.LineItems.Find(matchingLineItem => matchingLineItem.DeleteFlag == false);

			lineItem.PartialFill = this.RequestTypeComboBox.SelectedItem != null
				&& (this.RequestTypeComboBox.SelectedItem.Text == FuelRequestSR.PartialFillRequestSubType || this.RequestTypeComboBox.SelectedItem.Text == FuelRequestSR.PartialReturnToBulkSubType);

			if (this.RefCodeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RefCodeComboBox.SelectedItem.Text))
			{
				transaction.UserData18 = this.RefCodeComboBox.SelectedItem.Text;
			}

			EquipmentDO equipmentDO = (transaction.TransTypeID == FuelRequestForm.FillStandTransactionType) 
											? transaction.DestinationEQ1 : transaction.SourceEQ1;
			EquipmentDO lineItemEquipmentDO = (transaction.TransTypeID == FuelRequestForm.FillStandTransactionType) 
											? lineItem.DestinationEQ : lineItem.SourceEQ;

			equipmentDO.EquipmentGuid = Guid.Empty;
			equipmentDO.RegistrationID = string.Empty;
			equipmentDO.EquipmentType = string.Empty;

			lineItemEquipmentDO.EquipmentGuid = Guid.Empty;
			lineItemEquipmentDO.RegistrationID = string.Empty;
			lineItemEquipmentDO.EquipmentType = string.Empty;

			if (this.RegistrationIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text))
			{
				equipmentDO.RegistrationID = this.RegistrationIDComboBox.SelectedItem.Text;
				lineItemEquipmentDO.RegistrationID = this.RegistrationIDComboBox.SelectedItem.Text;

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

			if (this.GradeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.GradeComboBox.SelectedItem.Text))
			{
				lineItem.Product = this.GradeComboBox.SelectedItem.Text;
				lineItem.ProductGuid = Guid.Empty;

				Guid productGuid;
				Guid.TryParse(this.GradeComboBox.SelectedItem.Value, out productGuid);

				if (productGuid != Guid.Empty)
				{
					product = FMChannelHelper.MakeCall<IProducts, ProductClass>(products => products.Get(this.Security, productGuid));

					lineItem.ProductType = ProductClass.ProductTypeID(ProductType.ComponentProduct);
					lineItem.ProductCode = product.Code;
					lineItem.ProductGuid = product.MasterRecordGuid;
				}
			}

			if (this.LocationComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.LocationComboBox.SelectedItem.Text))
			{
				transaction.UserData7 = this.LocationComboBox.SelectedItem.Text;
			}
			else
			{
				transaction.UserData7 = string.Empty;
			}

			if (this.RequestCancelledCheckBox.Checked)
			{
				transaction.Status = TransactionStatus.Cancelled;

				foreach (LineItemDO lineItemToCancel in transaction.LineItems)
				{
					lineItemToCancel.Status = TransactionStatus.Cancelled;
				}
			}

			transaction.Notes = this.CommentsTextBox.Text;
			transaction.ContactSurname = this.RequestedByTextBox.Text;
		}
		#endregion

		#region Transaction Data Validation
		/// <summary>
		/// Apply any data validation checks to fields on the tab
		/// </summary>
		/// <param name="transaction">Represents the transaction object</param>
		/// <returns>True if the checks pass and everything is OK. 
		/// False if a problem with the data was detected</returns>
		public bool ValidateTransactionData(TransactionDO transaction)
		{
			if (!this.CheckRequiredFields(transaction))
			{
				return false;
			}

			if (!ParentForm.UpdateVariance(transaction))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Check to make sure required fields were provided and issue an alert if they weren't
		/// </summary>
		/// <param name="transaction">Represents the transaction object</param>
		/// <returns>True if required fields were provided</returns>
		private bool CheckRequiredFields(TransactionDO transaction)
		{
			FuelRequestType requestType = this.ParentForm.RequestType;

			if (this.GradeComboBox.SelectedItem == null || string.IsNullOrEmpty(this.GradeComboBox.SelectedItem.Text))
			{
				this.ParentForm.ShowAlert("Grade must be provided");
				this.ParentForm.SetFocusOnControl(this.GradeComboBox);
				return false;
			}

			if (this.RegistrationIDComboBox.SelectedItem == null || string.IsNullOrEmpty(this.RegistrationIDComboBox.SelectedItem.Text))
			{
				if (requestType == FuelRequestType.FastLogFillStand
					|| transaction.Status == TransactionStatus.Dispatched
					|| transaction.Status == TransactionStatus.Arrived
					|| transaction.Status == TransactionStatus.Started
					|| transaction.Status == TransactionStatus.Stopped
					|| transaction.Status == TransactionStatus.Completed
					|| FuelRequestFormSession.SessionCompletingTransaction)
				{
					this.ParentForm.ShowAlert("Registration ID must be provided");
					this.ParentForm.SetFocusOnControl(this.RegistrationIDComboBox);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// If necessary, issue a warning about the variance being out of tolerance.
		/// If the variance is out of tolerance for three consecutive actions, comments must be provided
		/// </summary>
		/// <param name="transaction">The transaction record to examine</param>
		/// <param name="originalNotes">Used to make sure that the notes aren't the same originally present on the transaction</param>
		/// <param name="currentConsecutiveOosVariance">The current number of times the equipment has been out of tolerance. After we save the transaction we examine this
		/// value again</param>
		public void DisplayVarianceWarning(TransactionDO transaction, string originalNotes, out int currentConsecutiveOosVariance)
		{
			currentConsecutiveOosVariance = 0;
			LineItemDO lineItem = transaction.LineItems.Find((matchingLineItem) => matchingLineItem.DeleteFlag == false);

			if (string.IsNullOrEmpty(transaction.Notes)
				 && FuelRequestFormSession.SessionFuelRequestSubType != FuelRequestSR.PartialFillRequestSubType
				 && transaction.Status != TransactionStatus.Cancelled)
			{
				EquipmentClass fillstandEquipment;

				// If this is a return to bulk request, the fueling vehicle is the source equipment record.
				// If this is a fill or partial fill, the fueling vehicle is the destination equipment record.
				bool isReturnToBulk = FuelRequestFormSession.SessionFuelRequestSubType == FuelRequestSR.ReturnToBulkRequestSubType;

				if (!isReturnToBulk)
				{
					fillstandEquipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(equipments => equipments.Get(this.Security, lineItem.DestinationEQ.EquipmentGuid));
				}
				else
				{
					fillstandEquipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(equipments => equipments.Get(this.Security, lineItem.SourceEQ.EquipmentGuid));
				}

				if (fillstandEquipment != null && fillstandEquipment.IdentityGuid != Guid.Empty)
				{
					currentConsecutiveOosVariance = fillstandEquipment.Consecutive_OOS_Variance;

					double safeFill;
					double volume;

					double.TryParse(fillstandEquipment.SafeFill, out safeFill);
					double.TryParse(fillstandEquipment.Volume, out volume);

					double localVariance;
					double tolerance;

					// Calculate the variance
					if (isReturnToBulk)
					{
						localVariance = lineItem.Quantity.NetInventoryChange - volume;
						tolerance = Math.Abs(localVariance / volume * 100.0);
					}
					else
					{
						// if the volume added will fill above capacity then we do a different calculation
						if ((lineItem.Quantity.NetInventoryChange + volume) > safeFill)
						{
							localVariance = safeFill - (lineItem.Quantity.NetInventoryChange + volume);
						}
						else
						{
							localVariance = (safeFill - volume) - lineItem.Quantity.NetInventoryChange;
						}

						tolerance = Math.Abs(localVariance / safeFill * 100.0);
					}

					// Verify that the variance has not changed direction before displaying the message
					if ((fillstandEquipment.Consecutive_OOS_Variance == -2 && localVariance < 0)
						 || (fillstandEquipment.Consecutive_OOS_Variance == 2 && localVariance > 0))
					{
						// Check the tolerance - if we will be at three once we save the transaction, we need to
						// require a comment.
						if (tolerance >= 2 && (string.IsNullOrEmpty(transaction.Notes) || transaction.Notes == originalNotes))
						{
							this.ParentForm.SetFocusOnControl(this.CommentsTextBox);
							throw new Exception("Deviation is >= 2% for three consecutive actions. Comment field is required");
						}
					}
				}
			}
		}
		#endregion
	}
}