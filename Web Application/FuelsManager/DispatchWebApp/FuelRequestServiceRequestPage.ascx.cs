// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelRequestServiceRequestPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Represents the Service Request tab on the Fuel Request Form that is displayed 
// for refuel and defuel requests, including fast log and transient requests. 
// This tab contains information about the aircraft, 
// the product requested, and billing information such as the company (DoDAAC).
// 
// A different Service Request tab is displayed for Fill Stand requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// <para>
	/// Represents the Service Request tab on the Fuel Request Form that is displayed 
	/// for refuel and defuel requests, including fast log and transient requests. 
	/// This tab contains information about the aircraft, 
	/// the product requested, and billing information such as the company (DoDAAC).
	/// </para>
	/// <para>
	/// A different Service Request tab is displayed for Fill Stand requests.
	/// </para>
	/// </summary>
	public partial class FuelRequestServiceRequestPage : FuelRequestFormPageBase
	{

		private bool handleActivityComboBoxEvents = true;

		private bool handleAircraftIDComboBoxEvents = true;
		private bool handleGradeComboBoxEvents = true;
		//private bool handleRegistrationIDComboBoxEvents;

		#region Page Properties
		/// <summary>
		/// The product currently selected in the grade combo box on the Refuel and Defuel Service Request tab page
		/// </summary>
		public Guid SelectedProduct
		{
			get
			{
				Guid productGuid = Guid.Empty;

				if (this.GradeComboBox.SelectedItem != null && Guid.TryParse(this.GradeComboBox.SelectedItem.Value, out productGuid))
				{
					return productGuid;
				}

				return Guid.Empty;
			}
		}

		/// <summary>
		/// Indicates whether the request cancelled box on the tab page is checked
		/// </summary>
		public bool RequestCancelled
		{
			get
			{
				return this.RequestCancelledCheckBox.Checked;
			}
		}

		/// <summary>
		/// Identifies the aircraft record selected by the user 
		/// </summary>
		public Guid AircraftGuid
		{
			get
			{
				Guid aircraftGuid = Guid.Empty;

				if (this.AircraftIDComboBox.SelectedItem != null && Guid.TryParse(this.AircraftIDComboBox.SelectedItem.Value, out aircraftGuid))
				{
					return aircraftGuid;
				}

				return Guid.Empty;
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
				if (this.Page.IsPostBack == false)
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

		#region Page Combo Box Control Population
		/// <summary>
		/// Fill controls on the tab with initial values and combo box items
		/// </summary>
		private void PopulateControls()
		{
			this.PopulateRefIDAndAircraftID();
			this.PopulateGrade();
			this.PopulateActivity();
			this.PopulateSignalCode();
			this.PopulateUseCode();
		}

		/// <summary>
		/// Populate the reference code and aircraft ID combo boxes with a list
		/// of aircraft. 
		/// </summary>
		private void PopulateRefIDAndAircraftID()
		{

			//breaks - SelectedProduct aircraft asdfasdf, then Selected activity 1234, throws error
			bool allowComboBoxTextEntry = this.ParentForm.RequestType == FuelRequestType.Transient || FuelRequestFormSession.SessionTransactionAlias.PermitNonReferenceData;

			var aircraftTypes = new EQUIPMENT_TYPE[] { EQUIPMENT_TYPE.AIRCRAFT_TYPE };

			EquipmentCollectionClass equipmentCollection = new EquipmentCollectionClass();

			if (this.ParentForm.RequestType != FuelRequestType.Transient)
			{
				// If a fuel card is selected in the Activity combo box, select aircraft records that
				// have the fuel card assigned to them
				Guid fuelCardGuid = Guid.Empty;

				if (this.ActivityComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.ActivityComboBox.SelectedItem.Value))
				{
					Guid.TryParse(this.ActivityComboBox.SelectedItem.Value, out fuelCardGuid);
				}

				object fuelCardGuidObject = fuelCardGuid == Guid.Empty ? null : (object)fuelCardGuid;

				// If a product is selected in the Grade combo box, select aircraft records that
				// have the product assigned or no assigned product
				Guid productGuid = this.SelectedProduct;
				object productGuidObject = productGuid == Guid.Empty ? null : (object)productGuid;

				var equipmentDataSet = FMChannelHelper.MakeCall<IEquipments, DataSet>(equipments => equipments.EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(this.Security, aircraftTypes, null, fuelCardGuidObject, productGuidObject, null));

				this.LoadEquipment(equipmentDataSet, equipmentCollection);
			}
			else
			{
				// We don't load aircraft for transient requests because the transient aircraft won't be in our system anyway.
				// Additionally, the user is not allowed to select a ref ID when entering a transient request
				// However, if this was an existing transient transaction we do need to make sure that the 
				// aircraft ID recorded is an item in the combo box - otherwise, we won't be able to select it when displaying the transaction.
				this.RefIDComboBox.Enabled = false;

				


			}

			// If we're supporting transient requests, allow the user to type text into the 
			// Aircraft ID combo box
			this.AircraftIDComboBox.DropDownStyle = allowComboBoxTextEntry ? AjaxControlToolkit.ComboBoxStyle.DropDown : AjaxControlToolkit.ComboBoxStyle.DropDownList;
			this.RefIDComboBox.DropDownStyle = allowComboBoxTextEntry ? AjaxControlToolkit.ComboBoxStyle.DropDown : AjaxControlToolkit.ComboBoxStyle.DropDownList;

			EquipmentClass selectedAircraft = null;

			if (this.AircraftIDComboBox.SelectedItem != null || this.RefIDComboBox.SelectedItem != null
				//&& !string.IsNullOrEmpty(this.AircraftIDComboBox.SelectedItem.Text)
				)
			{
				Guid selectedAircraftGuid = Guid.Empty;

				if (AircraftIDComboBox.SelectedItem != null)
				{
					Guid.TryParse(this.AircraftIDComboBox.SelectedItem.Value, out selectedAircraftGuid);
				}


				// If there's a value selected in the combo box, the aircraft is the currently selected value
				// Find it in the list of aircraft equipment records we retrieved
				if ( selectedAircraftGuid != Guid.Empty)
				{
					selectedAircraft = equipmentCollection.Find(aircraft => aircraft.MasterRecordGuid == selectedAircraftGuid);

					if (selectedAircraft == null)
					{
						equipmentCollection.Add(FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, selectedAircraftGuid)));
					}

					selectedAircraft = equipmentCollection.Find(x => x.MasterRecordGuid == selectedAircraftGuid);
				}
				else
				{
					// An invalid Guid indicates a manually entered record, which should only happen for transient requests. 
					selectedAircraft = new EquipmentClass { ID = string.Empty };

					if (this.AircraftIDComboBox.SelectedItem != null)
					{
						selectedAircraft.ID = this.AircraftIDComboBox.SelectedItem.Text;
					}

					if (this.RefIDComboBox.SelectedItem != null)
					{
						selectedAircraft.Xref = this.RefIDComboBox.SelectedItem.Text;
					}
				}
			}
			else if (FuelRequestFormSession.SessionTransaction != null)
			{
				// If the transaction is not a new transaction, make sure that the 
				// aircraft that was used appears in the combo box.
				EquipmentDO equipmentDO = (FuelRequestFormSession.SessionFuelRequestSubType == FuelRequestSR.RefuelRequestSubType) ? FuelRequestFormSession.SessionTransaction.DestinationEQ1 : FuelRequestFormSession.SessionTransaction.SourceEQ1;

				if (equipmentDO.EquipmentGuid == Guid.Empty)
				{

					// An invalid Guid indicates a manually entered record
					selectedAircraft = new EquipmentClass { ID = string.Empty };


					if (this.AircraftIDComboBox.Text.Length == 0)
					{
						selectedAircraft.ID = equipmentDO.RegistrationID;
					}

					if (this.RefIDComboBox.Text.Length == 0 && this.ParentForm.RequestType != FuelRequestType.Transient)
					{
						selectedAircraft.Xref = FuelRequestFormSession.SessionTransaction.UserData18;
					}
				}
				else
				{
					selectedAircraft = equipmentCollection.Find(aircraft => aircraft.MasterRecordGuid == equipmentDO.EquipmentGuid);
					if (selectedAircraft == null)
					{
						equipmentCollection.Add(FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, equipmentDO.EquipmentGuid)));
					}

					selectedAircraft = equipmentCollection.Find(x => x.MasterRecordGuid == equipmentDO.EquipmentGuid);
				}
			}
			

			// Sort the aircraft by Xref and remove any duplicates
			List<EquipmentClass> sortedResults = equipmentCollection.OrderBy(equipment => equipment.Xref).ToList<EquipmentClass>();
			List<EquipmentClass> filteredResults = new List<EquipmentClass>();

			foreach (EquipmentClass equipment in sortedResults)
			{
				if (!string.IsNullOrEmpty(equipment.Xref)
					&& filteredResults.Find(matchingEquipment => matchingEquipment.Xref == equipment.Xref) == null)
				{
					filteredResults.Add(equipment);
				}
			}

			string fuelRequestEnteredText = this.RefIDComboBox.Text;
			string aircraftEnteredText = this.AircraftIDComboBox.Text;

			// Refresh the lists. The combo boxes are pretty picky - sometimes
			// when DataBind() is called you'll get an ArgumentOutOfRangeException unless
			// you manually clear out the items and set the selectedValue = null
			this.RefIDComboBox.Clear();
			this.RefIDComboBox.SelectedValue = null;
			this.RefIDComboBox.DataSource = filteredResults;
			this.RefIDComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.RefIDComboBox);

			this.AircraftIDComboBox.Clear();
			this.AircraftIDComboBox.SelectedValue = null;
			this.AircraftIDComboBox.DataSource = equipmentCollection;
			this.AircraftIDComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.AircraftIDComboBox);


			// Restore the user's previous selection now
			if (selectedAircraft != null)
			{
				this.RefIDComboBox.SelectByText(selectedAircraft.Xref);
				this.AircraftIDComboBox.SelectByText(selectedAircraft.ID);

				// We must add manually entered aircraft to the list because they will be removed when the lists are refreshed
				// We only want to do this if the selected aircraft has no identity guid (it's transient), 
				// and we are allowing manual entry of records
				if ((this.AircraftIDComboBox.SelectedItem == null || string.IsNullOrEmpty(this.AircraftIDComboBox.SelectedItem.Text))
					&& !string.IsNullOrEmpty(selectedAircraft.ID)
					&& selectedAircraft.IdentityGuid == Guid.Empty && allowComboBoxTextEntry)
				{
					if (this.AircraftIDComboBox.Items.FindByText(selectedAircraft.ID) == null)
					{
						this.AircraftIDComboBox.Items.Add(new ListItem(selectedAircraft.ID, selectedAircraft.ID));
					}

					this.AircraftIDComboBox.SelectByText(selectedAircraft.ID);
				}

				if ((this.RefIDComboBox.SelectedItem == null || string.IsNullOrEmpty(this.RefIDComboBox.SelectedItem.Text))
					&& !string.IsNullOrEmpty(selectedAircraft.Xref)
					&& selectedAircraft.IdentityGuid == Guid.Empty && allowComboBoxTextEntry)
				{
					if (!string.IsNullOrEmpty(selectedAircraft.Xref) && this.RefIDComboBox.Items.FindByText(selectedAircraft.Xref) == null)
					{
						this.RefIDComboBox.Items.Add(new ListItem(selectedAircraft.Xref, selectedAircraft.Xref));
					}

					this.RefIDComboBox.SelectByText(selectedAircraft.Xref);
				}
			}
			else
			{

				// The user had no value previously selected. Make sure the blank entries are selected again.
				this.RefIDComboBox.SelectedValue = Guid.Empty.ToString();
				this.AircraftIDComboBox.SelectedValue = Guid.Empty.ToString();
			}

			if (RefIDComboBox.SelectedValue == Guid.Empty.ToString() && fuelRequestEnteredText.Length > 0)
			{
				RefIDComboBox.Text = fuelRequestEnteredText;
			}

			if (AircraftIDComboBox.SelectedValue == Guid.Empty.ToString() && aircraftEnteredText.Length > 0)
			{
				AircraftIDComboBox.Text = aircraftEnteredText;
			}
		}

		/// <summary>
		/// Add component products to the list of products in the Grade combo box
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

			this.GradeComboBox.DataSource = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(products => products.EnumerateByType(this.Security, ProductType.ComponentProduct));

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

		private void PopulateAssociatedActivityFields()
		{
			if (this.ActivityComboBox.SelectedItem != null)
			{
				// If we find a fuel card record, use the data from it to set the billing information
				Guid fuelCardGuid = Guid.Empty;

				if (Guid.TryParse(this.ActivityComboBox.SelectedItem.Value, out fuelCardGuid))
				{
					if (fuelCardGuid != Guid.Empty)
					{
						FuelCardClass fuelCard = FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(
													fuelCards => fuelCards.Get(this.Security, fuelCardGuid, false));

						this.UseCodeComboBox.SelectedValue = fuelCard.UserData2;
						this.SignalCodeComboBox.SelectedValue = fuelCard.UserData1;
						this.FundCodeTextBox.Text = fuelCard.UserData3;
						this.BOSTextBox.Text = fuelCard.UserData4;
						this.RPTTECAPCTextBox.Text = fuelCard.UserData5;
						this.DODAACTextBox.Text = fuelCard.ShipToID;

						this.SuppDODAACTextBox.Text = fuelCard.ShipToID != fuelCard.BillToID ? fuelCard.BillToID : string.Empty;
					}
					else
					{
						this.UseCodeComboBox.SelectedValue = string.Empty;
						this.SignalCodeComboBox.SelectedValue = string.Empty;
						this.FundCodeTextBox.Text = string.Empty;
						this.BOSTextBox.Text = string.Empty;
						this.RPTTECAPCTextBox.Text = string.Empty;
						this.DODAACTextBox.Text = string.Empty;
						this.SuppDODAACTextBox.Text = string.Empty;
					}
				}
			}

			if (handleActivityComboBoxEvents)
			{
				handleAircraftIDComboBoxEvents = false;
				this.PopulateRefIDAndAircraftID();
				handleAircraftIDComboBoxEvents = true;
			}
		}

		/// <summary>
		/// Add fuel cards to the list of fuel cards in the Activity combo box
		/// </summary>
		private void PopulateActivity()
		{
			string previouslySelectedActivityText = string.Empty;

			// If a value is selected, remember the value selected before we reload the records to display 
			// in the list so that we can select the same value again.
			if (this.ActivityComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.ActivityComboBox.SelectedItem.Text))
			{
				previouslySelectedActivityText = this.ActivityComboBox.SelectedItem.Text;
			}

			if (FuelRequestFormSession.SessionTransactionAlias.PermitNonReferenceData)
			{
				this.ActivityComboBox.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDown;
			}
			else
			{
				this.ActivityComboBox.DropDownStyle = AjaxControlToolkit.ComboBoxStyle.DropDownList;
			}

			FuelCardCollectionClass fuelCardCollection = new FuelCardCollectionClass();
			fuelCardCollection.Add(new FuelCardClass(){ID = string.Empty, IdentityGuid = Guid.Empty});

			fuelCardCollection.AddRange(FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(fuelCards => fuelCards.EnumerateFuelCards(this.Security)));

			this.ActivityComboBox.SelectedIndex = -1;
			this.ActivityComboBox.SelectedValue = Guid.Empty.ToString();
			this.ActivityComboBox.DataSource = fuelCardCollection;

			this.ActivityComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.ActivityComboBox);

			if (!string.IsNullOrEmpty(previouslySelectedActivityText))
			{
				this.ActivityComboBox.SelectByText(previouslySelectedActivityText);
			}
			else
			{
				this.ActivityComboBox.SelectedValue = Guid.Empty.ToString();
			}
		}

		/// <summary>
		/// Fill the Use Code combo box with values configured for the fuel card user data 2 field
		/// </summary>
		private void PopulateUseCode()
		{
			string previouslySelectedUseCodeText = string.Empty;

			// If a value is selected, remember the value selected before we reload the records to display 
			// in the list so that we can select the same value again.
			if (this.UseCodeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.UseCodeComboBox.SelectedItem.Text))
			{
				previouslySelectedUseCodeText = this.UseCodeComboBox.SelectedItem.Text;
			}

			UserDataFieldClass useCodeUserData = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldClass>(userDataFields =>
			{
				Guid identityGuid = userDataFields.GetIdentityGuid(this.Security, ENTITY_TYPE.FUEL_CARD, Guid.Empty, 1, false);
				return userDataFields.Get(this.Security, identityGuid, ENTITY_TYPE.FUEL_CARD);
			});

			this.UseCodeComboBox.Enabled = string.Compare(useCodeUserData.DisplayName, "Use Code", StringComparison.OrdinalIgnoreCase) == 0;

			this.UseCodeComboBox.SelectedIndex = -1;
			this.UseCodeComboBox.DataSource = useCodeUserData.UserDataListValueCollection;
			this.UseCodeComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.UseCodeComboBox, false);

			if (!string.IsNullOrEmpty(previouslySelectedUseCodeText))
			{
				this.UseCodeComboBox.SelectByText(previouslySelectedUseCodeText);
			}
			else
			{
				this.UseCodeComboBox.SelectedValue = string.Empty;
			}
		}

		/// <summary>
		/// Fill the Signal Code combo box with values configured for the fuel card user data 1 field
		/// </summary>
		private void PopulateSignalCode()
		{
			string previouslySelectedSignalCodeText = string.Empty;

			// If a value is selected, remember the value selected before we reload the records to display 
			// in the list so that we can select the same value again.
			if (this.SignalCodeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.SignalCodeComboBox.SelectedItem.Text))
			{
				previouslySelectedSignalCodeText = this.SignalCodeComboBox.SelectedItem.Text;
			}

			UserDataFieldClass signalCodeUserData = FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldClass>(userDataFields =>
			{
				Guid identityGuid = userDataFields.GetIdentityGuid(this.Security, ENTITY_TYPE.FUEL_CARD, Guid.Empty, 0, false);
				return userDataFields.Get(this.Security, identityGuid, ENTITY_TYPE.FUEL_CARD);
			});

			this.SignalCodeComboBox.Enabled = string.Compare(signalCodeUserData.DisplayName, "Sig. Code", StringComparison.OrdinalIgnoreCase) == 0;

			this.SignalCodeComboBox.SelectedIndex = -1;
			this.SignalCodeComboBox.DataSource = signalCodeUserData.UserDataListValueCollection;
			this.SignalCodeComboBox.DataBind();
			this.AddBlankComboBoxEntry(this.SignalCodeComboBox, false);

			if (!string.IsNullOrEmpty(previouslySelectedSignalCodeText))
			{
				this.SignalCodeComboBox.SelectByText(previouslySelectedSignalCodeText);
			}
			else
			{
				this.SignalCodeComboBox.SelectedValue = string.Empty;
			}
		}
		#endregion

		#region Page Control Events
		/// <summary>
		/// When the Ref ID changes, filter the available aircraft in the Aircraft ID combo box
		/// to those that have the same Ref ID
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void RefIDComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{

				if (!this.handleAircraftIDComboBoxEvents)
				{
					return;
				}

				if (ParentForm.RequestType == FuelRequestType.Transient &&
					string.IsNullOrEmpty(RefIDComboBox.Text))
				{
					// For transients, an empty ref code should not result in the
					// selection of an aircraft ID - it only means that the ref code
					// is not known.
					return;
				}

				// Filter the aircraft combo based on the selection from this one.  Only aircraft with 
				// the same ref code should be listed.
				string selectedRefID = string.Empty;

				if (this.RefIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RefIDComboBox.SelectedItem.Text))
				{
					selectedRefID = this.RefIDComboBox.SelectedItem.Text;
				}


				EQUIPMENT_TYPE[] aircraftTypes = { EQUIPMENT_TYPE.AIRCRAFT_TYPE };

				DataSet dataSet = null;
				EquipmentCollectionClass equipmentCollection = new EquipmentCollectionClass();

				// If a fuel card is selected in the Activity combo box, select aircraft records that
				// have the fuel card assigned to them
				Guid fuelCardGuid = Guid.Empty;

				if (this.ActivityComboBox.SelectedItem != null)
				{
					Guid.TryParse(this.ActivityComboBox.SelectedItem.Value, out fuelCardGuid);
				}

				object fuelCardGuidObject = fuelCardGuid == Guid.Empty ? null : (object)fuelCardGuid;

				// If a product is selected in the Grade combo box, select aircraft records that
				// have the product assigned or no assigned product
				Guid productGuid = this.SelectedProduct;
				object productGuidObject = productGuid == Guid.Empty ? null : (object)productGuid;

				dataSet = FMChannelHelper.MakeCall<IEquipments, DataSet>(equipments => equipments.EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(this.Security, aircraftTypes, null, fuelCardGuidObject, productGuidObject, null));

				this.LoadEquipment(dataSet, equipmentCollection);

				if (!string.IsNullOrEmpty(selectedRefID))
				{
					List<EquipmentClass> filteredAircraft = equipmentCollection.FindAll(aircraft => aircraft.Xref == selectedRefID);
					this.AircraftIDComboBox.DataSource = filteredAircraft;

					if (filteredAircraft.Count > 0)
					{
						this.AircraftIDComboBox.SelectedIndex = 0;
					}
				}
				else
				{
					this.AircraftIDComboBox.DataSource = equipmentCollection;
				}

				this.AircraftIDComboBox.DataBind();


				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.RefIDComboBox);

				this.AircraftIDComboBoxSelectedIndexChanged(null, null);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the aircraft ID combo box selection changes, 
		/// set the appropriate values in the Activity, Grade, and MDS (model) controls,
		/// using values from the aircraft record
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void AircraftIDComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (!this.handleAircraftIDComboBoxEvents)
				{
					return;
				}


				if (this.AircraftIDComboBox.SelectedItem != null)
				{
					Guid aircraftGuid = Guid.Empty;

					if (Guid.TryParse(this.AircraftIDComboBox.SelectedItem.Value, out aircraftGuid))
					{
						if (aircraftGuid != Guid.Empty)
						{
							this.handleActivityComboBoxEvents = false;
							this.handleGradeComboBoxEvents = false;

							EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
															equipments => equipments.Get(this.Security, aircraftGuid));

							if (equipment.FuelCardGuid != Guid.Empty)
							{
								this.ActivityComboBox.SelectedValue = equipment.FuelCardGuid.ToString();
							}
							else
							{
								this.ActivityComboBox.SelectedValue = Guid.Empty.ToString();
							}

							this.ActivityComboBox_SelectedIndexChanged(null,null);

							try
							{
								if (equipment.ProductGuid != Guid.Empty)
								{
									this.GradeComboBox.SelectedValue = equipment.ProductGuid.ToString();
								}
							}
							catch (ArgumentOutOfRangeException)
							{
								this.ParentForm.ShowAlert("Could not find the product that corresponds to aircraft " 
															+ equipment.ID + ". Are you sure that it is a component product?");
								this.GradeComboBox.SelectedValue = Guid.Empty.ToString();
							}

							this.GradeComboBoxSelectedIndexChanged(null,null);

							this.ParentForm.PopulateRegistrationIDComboBoxes();

							this.MDSTextBox.Text = equipment.Model;
							this.FuelAdditiveCheckBox.Checked = equipment.FuelAdditiveFlag;
							this.CardNumberTextBox.Text = equipment.UserData10;

							if (!string.IsNullOrEmpty(equipment.Xref))
							{
								this.RefIDComboBox.SelectByText(equipment.Xref);
							}
							else
							{
								this.RefIDComboBox.SelectedValue = Guid.Empty.ToString();
							}

							this.handleAircraftIDComboBoxEvents = false;
							this.RefIDComboBoxSelectedIndexChanged(null,null);
							this.handleAircraftIDComboBoxEvents = true;

							this.handleActivityComboBoxEvents = true;
							this.handleGradeComboBoxEvents = true;
						}
						else
						{
							// The user selected the empty record, so blank out any fields we previously determined based on the selection
							this.MDSTextBox.Text = string.Empty;
							this.FuelAdditiveCheckBox.Checked = false;
							this.CardNumberTextBox.Text = string.Empty;
							this.RefIDComboBox.SelectedValue = Guid.Empty.ToString();
							this.ActivityComboBox.SelectedValue = Guid.Empty.ToString();
							this.GradeComboBox.SelectedValue = Guid.Empty.ToString();
						}
					}

					// Else the Guid isn't valid - the user must have selected a record they entered manually
				}


				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.AircraftIDComboBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the grade (product) changes, repopulate the available aircraft
		/// and fueling vehicles since the product can filter the equipment available
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void GradeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{

				if (!this.handleGradeComboBoxEvents)
				{
					return;
				}

				handleAircraftIDComboBoxEvents = false;
				handleActivityComboBoxEvents = false;

				this.PopulateRefIDAndAircraftID();
				this.ParentForm.PopulateRegistrationIDComboBoxes();

				handleAircraftIDComboBoxEvents = true;
				handleActivityComboBoxEvents = true;

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.GradeComboBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the Activity (fuel card) changes, populate controls on the screen
		/// with data from the fuel card. 
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void ActivityComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				// Populate all the associated activity fields.
				this.PopulateAssociatedActivityFields();


				if (handleActivityComboBoxEvents)
				{
					handleAircraftIDComboBoxEvents = false;
					// Repopulate the available aircraft
					// since the fuel card filters the available aircraft.
					this.PopulateRefIDAndAircraftID();

					handleAircraftIDComboBoxEvents = true;
				}

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.ActivityComboBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the request type changes, repopulate the aircraft and fueling vehicle controls
		/// since they depend on the request type
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void RequestTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.RequestTypeComboBox.SelectedItem != null)
				{
					this.ParentForm.DetermineTransactionAlias(this.RequestTypeComboBox.SelectedItem.Text);

					// We must re-populate the controls since some things depend on the type of request,
					// most importantly whether or not the combo boxes should allow manual entry of data
					this.PopulateControls();

					this.ParentForm.PopulateRegistrationIDComboBoxes();
				}

				// Set the focus on the control so we don't lose focus during postback
				this.ParentForm.SetFocusOnControl(this.RequestTypeComboBox);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// This method handles the request checkbox change event.
		/// </summary>
		/// <param name="sender">The sending object.</param>
		/// <param name="e">The event arguments.</param>
		protected void RequestCancelledCheckboxChecked(object sender, EventArgs e)
		{
			if (this.RequestCancelledCheckBox.Checked)
			{
				this.ParentForm.UpdateQuantityOnDetailForm = "0";
				this.ParentForm.QuantityEnabledOnDetailForm = false;
			}
			else
			{
				{
					// The quantity text box is only enabled when completing a transaction or for fast logs
					this.ParentForm.QuantityEnabledOnDetailForm = FuelRequestFormSession.SessionCompletingTransaction
					                                              || this.ParentForm.IsFastLogOrFastLogFillStand;
				}
			}
		}
		#endregion

		#region Transaction Data Validation
		/// <summary>
		/// Apply any data validation checks to fields on the tab
		/// </summary>
		/// <param name="transaction">Represents the transaction object, which we use to determine things like if the record is new or not</param>
		/// <returns>True if the checks pass and everything is OK. 
		/// False if a problem with the data was detected</returns>
		public bool ValidateTransactionData(TransactionDO transaction)
		{
			if (!this.CheckRequiredFields(transaction)
				|| !this.ValidateRptTecApcLength()
				|| !this.ValidateBosLength())
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check to make sure required fields were provided and issue an alert if they weren't
		/// </summary>
		/// <param name="transaction">Represents the transaction object, which we use to determine things like if the record is new or not</param>
		/// <returns>True if required fields were provided</returns>
		private bool CheckRequiredFields(TransactionDO transaction)
		{
			FuelRequestType requestType = this.ParentForm.RequestType;

			if ((this.RefIDComboBox.SelectedItem == null || string.IsNullOrEmpty(this.RefIDComboBox.SelectedItem.Text))
				&& requestType != FuelRequestType.Transient)
			{
				this.ParentForm.ShowAlert("Ref ID must be provided");
				this.ParentForm.SetFocusOnControl(this.RefIDComboBox);
				return false;
			}

			if (this.AircraftIDComboBox.SelectedItem == null || string.IsNullOrEmpty(this.AircraftIDComboBox.SelectedItem.Text))
			{
				this.ParentForm.ShowAlert("Aircraft ID must be provided");
				this.ParentForm.SetFocusOnControl(this.AircraftIDComboBox);
				return false;
			}

			if (this.GradeComboBox.SelectedItem == null || string.IsNullOrEmpty(this.GradeComboBox.SelectedItem.Text))
			{
				this.ParentForm.ShowAlert("Grade must be provided");
				this.ParentForm.SetFocusOnControl(this.GradeComboBox);
				return false;
			}

			if (string.IsNullOrEmpty(this.DODAACTextBox.Text))
			{
				// For transient requests, the user is allowed to enter a record without a dodacc but they must have a dodacc when editing
				// It also seems that providing a card number will suffice
				if ((requestType != FuelRequestType.Transient || transaction.TransactionGuid != Guid.Empty)
					&& string.IsNullOrEmpty(this.CardNumberTextBox.Text))
				{
					this.ParentForm.ShowAlert("DoDAAC must be provided");
					this.ParentForm.SetFocusOnControl(this.DODAACTextBox);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Make sure the BOS text field is three characters in length
		/// </summary>
		/// <returns>True if the BOS field length is three and everything is OK. 
		/// False otherwise</returns>
		private bool ValidateBosLength()
		{
			if (!string.IsNullOrWhiteSpace(this.BOSTextBox.Text)
				&& this.BOSTextBox.Text.Length != 3)
			{
				this.ParentForm.ShowAlert("BOS must be three characters in length");
				this.ParentForm.SetFocusOnControl(this.BOSTextBox);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check the length of the RTP/TEC/APC field, which 
		/// is either three or four depending on the DoDAAC field
		/// </summary>
		/// <returns>True if the RTP/TEC/APC field length is the correct length.
		/// False otherwise</returns>
		private bool ValidateRptTecApcLength()
		{
			string rptTecApc = this.RPTTECAPCTextBox.Text;

			string doDACC = this.DODAACTextBox.Text;

			int requiredLength = 3; // Default for the airforce

			if (!string.IsNullOrEmpty(doDACC))
			{
				// DoDAACs that begin with W or S indicate the army. DoDAAC that begin with S, N, Q, V, M, R, or U indicate the Navy.
				if (doDACC.StartsWith("W", StringComparison.InvariantCultureIgnoreCase)
					|| doDACC.StartsWith("S", StringComparison.InvariantCultureIgnoreCase)
					|| doDACC.StartsWith("N", StringComparison.InvariantCultureIgnoreCase)
					|| doDACC.StartsWith("Q", StringComparison.InvariantCultureIgnoreCase)
					|| doDACC.StartsWith("V", StringComparison.InvariantCultureIgnoreCase)
					|| doDACC.StartsWith("M", StringComparison.InvariantCultureIgnoreCase)
					|| doDACC.StartsWith("R", StringComparison.InvariantCultureIgnoreCase)
					|| doDACC.StartsWith("U", StringComparison.InvariantCultureIgnoreCase))
				{
					requiredLength = 4;
				}

				if (!string.IsNullOrWhiteSpace(rptTecApc) && rptTecApc.Length != requiredLength)
				{
					this.ParentForm.ShowAlert("RPT/TEC/APC must be " + requiredLength.ToString(CultureInfo.InvariantCulture) 
											+ " characters in length");
					this.ParentForm.SetFocusOnControl(this.RPTTECAPCTextBox);

					return false;
				}
			}

			return true;
		}
		#endregion

		#region Transaction Record Display and Creation
		/// <summary>
		/// Use the controls on the page to display data from a FuelsManager transaction record.
		/// </summary>
		/// <param name="transaction">The transaction record to display</param>
		public void DisplayTransaction(TransactionDO transaction)
		{
			LineItemDO lineItem = transaction.LineItems.Find(matchingLineItem => matchingLineItem.DeleteFlag == false);

			// Disable the request type box for existing transactions
			if (transaction.TransactionGuid != Guid.Empty)
			{
				//this.RequestTypeComboBox.Enabled = false;

				if ((transaction.Alias == FuelRequestForm.FuelRequestTransactionAlias || 
					transaction.Alias == FuelRequestForm.DefuelRequestTransactionAlias)
						&& transaction.Status != TransactionStatus.Completed 
						&& transaction.Status != TransactionStatus.Cancelled)
				{
					this.RequestTypeComboBox.Enabled = true;
				}
				else
				{
					this.RequestTypeComboBox.Enabled = false;
				}
			}

			this.RequestTypeComboBox.SelectByText(FuelRequestFormSession.SessionFuelRequestSubType);

			// Aircraft Data Group

			// Try to set the aircraft ID, ref ID, and MDS (model) based on the equipment information available on the transaction
			EquipmentDO aircraft = transaction.TransTypeID == FuelRequestForm.RefuelTransactionType ? transaction.DestinationEQ1 : transaction.SourceEQ1;

			if (aircraft != null)
			{
				this.MDSTextBox.Text = aircraft.EquipmentModel;

				string refID;

				transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_18, out refID);
				this.RefIDComboBox.SelectByText(refID);

				if (aircraft.EquipmentGuid != Guid.Empty)
				{
					this.AircraftIDComboBox.SelectedValue = aircraft.EquipmentGuid.ToString();
				}
				else if (!string.IsNullOrEmpty(aircraft.RegistrationID))
				{
					// Transient aircraft are not created in our system, so we have to select the value using the registration ID only
					this.AircraftIDComboBox.SelectByText(aircraft.RegistrationID);
				}
			}

			if (lineItem != null && lineItem.ProductGuid != Guid.Empty)
			{
				this.GradeComboBox.SelectedValue = lineItem.ProductGuid.ToString();
			}

			if (transaction.FuelCardGuid != Guid.Empty)
			{
				this.ActivityComboBox.SelectedValue = transaction.FuelCardGuid.ToString();
			}

			this.FuelAdditiveCheckBox.Checked = transaction.FuelAdditiveFlag;

			string locationText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_07, out locationText);
			this.LocationTextBox.Text = locationText;

			// Request Information Group
			this.RequestedByTextBox.Text = transaction.ContactSurname;

			if (transaction.TransactionGuid != Guid.Empty)
			{
				if (transaction.Status == TransactionStatus.Cancelled)
				{
					this.RequestCancelledCheckBox.Checked = true;
				}

				if (transaction.Status == TransactionStatus.Completed
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

			// Billing Information Group
			this.DODAACTextBox.Text = transaction.ShipToID;

			if (transaction.ShipToID != transaction.BillToID)
			{
				this.SuppDODAACTextBox.Text = transaction.BillToID;
			}

			string bosText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_19, out bosText);
			this.BOSTextBox.Text = bosText;

			string useCodeText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_21, out useCodeText);
			this.UseCodeComboBox.SelectedValue = useCodeText;

			string signalCodeText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_20, out signalCodeText);
			this.SignalCodeComboBox.SelectedValue = signalCodeText;

			string fundCodeText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_05, out fundCodeText);
			this.FundCodeTextBox.Text = fundCodeText;

			this.CardNumberTextBox.Text = transaction.PaymentInfo == null ? string.Empty : transaction.PaymentInfo.CreditCardNumber;

			string rptTECAPCText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_03, out rptTECAPCText);
			this.RPTTECAPCTextBox.Text = rptTECAPCText;

			// The combo boxes that identify aircraft must be repopulated for existing transactions, 
			// because we just set values that filter values available in the aircraft boxes, like product and fuel card
			if (transaction.TransactionGuid != Guid.Empty)
			{
				this.PopulateRefIDAndAircraftID();
			}
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

			if (this.RefIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.RefIDComboBox.SelectedItem.Text))
			{
				transaction.UserData18 = this.RefIDComboBox.SelectedItem.Text;
			}

			if (this.AircraftIDComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.AircraftIDComboBox.SelectedItem.Text))
			{
				EquipmentDO equipmentDO = (transaction.TransTypeID == FuelRequestForm.RefuelTransactionType) ? transaction.DestinationEQ1 : transaction.SourceEQ1;
				EquipmentDO lineItemEquipmentDO = (transaction.TransTypeID == FuelRequestForm.RefuelTransactionType) ? lineItem.DestinationEQ : lineItem.SourceEQ;

				equipmentDO.RegistrationID = this.AircraftIDComboBox.SelectedItem.Text;
				lineItemEquipmentDO.RegistrationID = this.AircraftIDComboBox.SelectedItem.Text;

				Guid equipmentGuid;
				Guid.TryParse(this.AircraftIDComboBox.SelectedItem.Value, out equipmentGuid);

				if (equipmentGuid != Guid.Empty)
				{
					EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(equipments => equipments.Get(this.Security, equipmentGuid));

					equipmentDO.SerialNumber = equipment.SerialNumber;
					equipmentDO.EquipmentType = equipment.EqTypeName;
					equipmentDO.EquipmentGuid = equipment.MasterRecordGuid;
					equipmentDO.EquipmentModel = this.MDSTextBox.Text;

					lineItemEquipmentDO.SerialNumber = equipment.SerialNumber;
					lineItemEquipmentDO.EquipmentType = equipment.EqTypeName;
					lineItemEquipmentDO.EquipmentGuid = equipment.MasterRecordGuid;
					lineItemEquipmentDO.EquipmentModel = this.MDSTextBox.Text;
				}
				else
				{
					equipmentDO.EquipmentGuid = Guid.Empty;
					equipmentDO.SerialNumber = string.Empty;
					equipmentDO.EquipmentType = string.Empty;
					equipmentDO.EquipmentModel = this.MDSTextBox.Text;

					lineItemEquipmentDO.EquipmentGuid = Guid.Empty;
					lineItemEquipmentDO.SerialNumber = string.Empty;
					lineItemEquipmentDO.EquipmentType = string.Empty;
					lineItemEquipmentDO.EquipmentModel = this.MDSTextBox.Text;
				}
			}

			// When an activity (Fuel Card) is selected, use information from it to populate company information
			transaction.FuelCardID = string.Empty;
			transaction.FuelCardGuid = Guid.Empty;

			if (this.ActivityComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.ActivityComboBox.SelectedItem.Text))
			{
				transaction.FuelCardID = this.ActivityComboBox.SelectedItem.Text;

				Guid fuelCardGuid;
				Guid.TryParse(this.ActivityComboBox.SelectedItem.Value, out fuelCardGuid);

				if (fuelCardGuid != Guid.Empty)
				{
					transaction.FuelCardGuid = fuelCardGuid;

					FuelCardClass fuelCard = FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(fuelCards => fuelCards.Get(this.Security, fuelCardGuid, false));

					if (fuelCard.ManagerGuid != Guid.Empty && !string.IsNullOrEmpty(fuelCard.ManagerID))
					{
						transaction.ManagerID = fuelCard.ManagerID;
						transaction.ManagerCode = fuelCard.ManagerCode;
						transaction.ManagerCompanyGuid = fuelCard.ManagerGuid;
					}

					if (fuelCard.OwnerGuid != Guid.Empty && !string.IsNullOrEmpty(fuelCard.OwnerID))
					{
						transaction.OwnerID = fuelCard.OwnerID;
						transaction.OwnerCode = fuelCard.OwnerCode;
						transaction.OwnerCompanyGuid = fuelCard.OwnerGuid;
					}

					if (fuelCard.ShipperGuid != Guid.Empty && !string.IsNullOrEmpty(fuelCard.ShipperID))
					{
						transaction.ShipperID = fuelCard.ShipperID;
						transaction.ShipperCode = fuelCard.ShipperCode;
						transaction.ShipperCompanyGuid = fuelCard.ShipperGuid;
					}
				}
			}

			if (this.GradeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.GradeComboBox.SelectedItem.Text))
			{
				lineItem.Product = this.GradeComboBox.SelectedItem.Text;
				lineItem.ProductGuid = Guid.Empty;

				Guid productGuid = this.SelectedProduct;

				if (productGuid != Guid.Empty)
				{
					product = FMChannelHelper.MakeCall<IProducts, ProductClass>(products => products.Get(this.Security, productGuid));

					lineItem.ProductType = ProductClass.ProductTypeID(ProductType.ComponentProduct);
					lineItem.ProductCode = product.Code;
					lineItem.ProductGuid = product.MasterRecordGuid;

					transaction.Flag01 = product.UserData2.ToUpper() == "YES"; //Aviation
					transaction.Flag02 = product.UserData1.ToUpper() == "YES"; //Capitalize
				}
			}

			transaction.ShipToCompanyGuid = Guid.Empty;
			transaction.ShipToCode = string.Empty;
			transaction.ShipToID = string.Empty;

			if (!string.IsNullOrEmpty(this.DODAACTextBox.Text))
			{
				transaction.ShipToID = this.DODAACTextBox.Text;

				Guid shipToGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
									companies => companies.GetIdentityGuid(this.Security, transaction.ShipToID));

				if (shipToGuid != Guid.Empty)
				{
					CompanyClass shipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
												companies => companies.Get(this.Security, shipToGuid, false));

					transaction.ShipToID = shipTo.ID;
					transaction.ShipToCode = shipTo.Code;
					transaction.ShipToCompanyGuid = shipTo.MasterRecordGuid;
				}
			}

			transaction.BillToID = string.Empty;
			transaction.BillToCode = string.Empty;
			transaction.BillToCompanyGuid = Guid.Empty;

			if (!string.IsNullOrEmpty(this.SuppDODAACTextBox.Text))
			{
				transaction.BillToID = this.SuppDODAACTextBox.Text;

				Guid billToGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
										companies => companies.GetIdentityGuid(this.Security, transaction.BillToID));

				if (billToGuid != Guid.Empty)
				{
					CompanyClass billTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
												companies => companies.Get(this.Security, billToGuid, false));

					transaction.BillToID = billTo.ID;
					transaction.BillToCode = billTo.Code;
					transaction.BillToCompanyGuid = billTo.MasterRecordGuid;
				}
			}

			transaction.PaymentInfo.CreditCardNumber = this.CardNumberTextBox.Text;

			transaction.FuelAdditiveFlag = this.FuelAdditiveCheckBox.Checked;

			if (this.RequestCancelledCheckBox.Checked)
			{
				transaction.Status = TransactionStatus.Cancelled;

				foreach (LineItemDO lineItemToCancel in transaction.LineItems)
				{
					lineItemToCancel.Status = TransactionStatus.Cancelled;
					lineItemToCancel.Quantity = new QuantityDO(0, 0, 0, 0);

					foreach (SubLineItemDO subLineItem in lineItemToCancel.SubLineItems)
					{
						subLineItem.Status = TransactionStatus.Cancelled;
					}
				}
			}

			transaction.Notes = this.CommentsTextBox.Text;
			transaction.ContactSurname = this.RequestedByTextBox.Text;

			transaction.UserData3 = this.RPTTECAPCTextBox.Text;
			transaction.UserData5 = this.FundCodeTextBox.Text;
			transaction.UserData7 = this.LocationTextBox.Text;
			transaction.UserData19 = this.BOSTextBox.Text;

			if (this.SignalCodeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.SignalCodeComboBox.SelectedItem.Text))
			{
				transaction.UserData20 = this.SignalCodeComboBox.SelectedItem.Text;
			}
			else
			{
				transaction.UserData20 = string.Empty;
			}

			if (this.UseCodeComboBox.SelectedItem != null && !string.IsNullOrEmpty(this.UseCodeComboBox.SelectedItem.Text))
			{
				transaction.UserData21 = this.UseCodeComboBox.SelectedItem.Text;
			}
			else
			{
				transaction.UserData21 = string.Empty;
			}
		}
		#endregion
	}
}