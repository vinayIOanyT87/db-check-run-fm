namespace TransactionFields
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using AjaxControlToolkit;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	
	public class FuelCardFG : FieldGenerator, IHeaderField
	{
		#region Public constants
		public const string ERR_MSG_001 = "Invalid Card : {0}.";

		public const string ERR_MSG_002 = "Card {0} is inactive.";

		public const string ERR_MSG_003 = "Card {0} is expired";
		#endregion

		#region Override Properties
		/// <summary>
		/// This property returns the field ID
		/// </summary>
		public override string FieldID
		{
			get { return "FuelCardID"; }
		}
		#endregion

		#region Override methods
		/// <summary>
		/// This method will generate the actual web control. In this case, the
		/// FMCompanyTextBox control is being generated.
		/// </summary>
		/// <param name="editable">Sets the control to be editable or not.</param>
		public override void Generate ( bool editable )
		{
			const int LocalMaxColumn = 50;

			if ( this.transContext.EnableAutoComplete )
			{
				var fieldValue = this.GetDataValue();
				var value = fieldValue == null ? string.Empty : fieldValue.ToString();

				var control = new FMAutoComplete
				{
					// ID cannot contain a period or jQuery will not activate the control
					ID				= this.ID.Replace( ".", string.Empty ),
					Columns			= LocalMaxColumn,
					FieldKey		= this.FieldID,
					CssClass		= "formfield",
					MaxLength		= LocalMaxColumn,
					Text			= value,
					Enabled			= editable,
					ClientAutoPost	= true,
					CallbackAddress = "TransactionDetail.aspx/GetFuelCardAutoComplete"
				};

				control.TextChanged += this.TextChanged;

				cell.Controls.Add( control );
			}
			else if ( !transContext.aliasClass.UseComboxControls )
			{
				var updatePanel = new UpdatePanel
				                  {
					                  UpdateMode = UpdatePanelUpdateMode.Conditional,
					                  ID = this.ID + "Panel"
				                  };

				var textBoxButtonCombo = new FMFuelCardTextBox ( );
				this.cell.Controls.Add ( textBoxButtonCombo );

				updatePanel.ContentTemplateContainer.Controls.Add(textBoxButtonCombo);
				this.cell.Controls.Add(updatePanel);

				textBoxButtonCombo.ID			= this.ID;
				textBoxButtonCombo.MaxLength	= LocalMaxColumn;
				textBoxButtonCombo.Columns		= LocalMaxColumn;
				textBoxButtonCombo.AutoPostBack = true;
				textBoxButtonCombo.BackColor	= this.VarecBkgrndReadOnlyGray;
				textBoxButtonCombo.Enabled		= editable;
				textBoxButtonCombo.ToolTip = this.displayName;

				object fieldValue = GetDataValue ( );

				if (fieldValue != null)
				{
					textBoxButtonCombo.Text = fieldValue.ToString ( );
				}

				textBoxButtonCombo.TextChanged += this.TextChanged;
			}

			else
			{
				var updatePanel = new UpdatePanel { UpdateMode = UpdatePanelUpdateMode.Conditional, ID = this.ID + "Panel" };
				var comboBox = new FMComboBox ( );

				updatePanel.ContentTemplateContainer.Controls.Add(comboBox);
				this.cell.Controls.Add(updatePanel);
				comboBox.ToolTip = this.displayName;

				comboBox.ID					= this.ID;
				comboBox.MaxLength			= 0;
				comboBox.AutoPostBack		= true;
				comboBox.Enabled			= editable;
				comboBox.CssClass			= "formfield txFieldComboBox";
				comboBox.RenderMode			= ComboBoxRenderMode.Block;
				comboBox.AutoCompleteMode	= ComboBoxAutoCompleteMode.Suggest;
				comboBox.DropDownStyle		= ComboBoxStyle.DropDownList;

				if (transContext.aliasClass.PermitNonReferenceData)
				{
					comboBox.DropDownStyle = ComboBoxStyle.DropDown;
				}

				else
				{
					comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				}

				comboBox.ItemInsertLocation = ComboBoxItemInsertLocation.OrdinalText;

				comboBox.TextChanged += this.TextChanged;
				comboBox.ItemInserted += this.ItemInserted;

				if (!cell.Page.IsPostBack || transContext.reload)
				{
					object fieldValue = GetDataValue ( );
					comboBox.Items.Add ( new ListItem ( string.Empty, string.Empty ) );
					bool itemInList = false;

					var cards = this.GetEntries();

					foreach (FuelCardClass fuelCard in cards)
					{
						comboBox.Items.Add ( new ListItem ( fuelCard.ID, fuelCard.ID ) );

						if (!this.TransFieldConfiguration.FileFound)
						{
							if (fuelCard.ID.Length > comboBox.TextBoxCntrl.Columns && fuelCard.ID.Length < LocalMaxColumn)
							{
								comboBox.MaxLength = fuelCard.ID.Length;
								comboBox.TextBoxCntrl.MaxLength = fuelCard.ID.Length;
								comboBox.TextBoxCntrl.Columns = fuelCard.ID.Length;
							}
						}

						if (fieldValue != null && fieldValue.ToString ( ) == fuelCard.ID)
						{
							comboBox.SelectedIndex = comboBox.Items.Count - 1;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString (CultureInfo.InvariantCulture);
							comboBox.Text = fieldValue.ToString ( );
							itemInList = true;
						}
					}

					if (!itemInList && fieldValue != null && fieldValue.ToString ( ) != string.Empty)
					{
						if (!this.TransFieldConfiguration.FileFound)
						{
							if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length < LocalMaxColumn)
							{
								comboBox.MaxLength = fieldValue.ToString().Length;
								comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
								comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
							}
						}

						int insertIndex = 0;
						foreach (ListItem item in comboBox.Items)
						{
							if (item.Text != string.Empty && item.Text.CompareTo(fieldValue.ToString()) > 0)
							{
								comboBox.Items.Insert(insertIndex, new ListItem(fieldValue.ToString(), fieldValue.ToString()));
								break;
							}

							insertIndex++;
						}

						if (insertIndex == comboBox.Items.Count)
						{
							comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));
						}

						comboBox.SelectedIndex = insertIndex;
						comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
						comboBox.Text = fieldValue.ToString();
					}
				}

				if (comboBox.TextBoxCntrl.MaxLength == 0)
				{
					comboBox.MaxLength = LocalMaxColumn;
					comboBox.TextBoxCntrl.MaxLength = LocalMaxColumn;
					comboBox.TextBoxCntrl.Columns = LocalMaxColumn;
				}
			}
		}

		/// <summary>
		/// This method will get a list of fuel cards.
		/// </summary>
		/// <returns>Collection of fuel cards.</returns>
		public List<FuelCardClass> GetEntries()
		{
			var fuelCardCollection = FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(x => x.EnumerateFuelCards(transContext.security));

			var cardList = new List<FuelCardClass>();

			var timeConverter = new SiteTimeConverter(this.transContext.accountingSite.CurrentSite);

			foreach (FuelCardClass card in fuelCardCollection)
			{
				if ( card.Status == FuelCardClass.Statuses.ACTIVE && 
				     (!card.ExpirationDate.HasValue || card.ExpirationDate.Value > timeConverter.Now()))
				{
					cardList.Add( card );
				}
			}

			return cardList;
		}

		/// <summary>
		/// The purpose of this method is to give auto complete controls access to the 
		/// entries determination logic of this field generator.  Intended for use by
		/// WebMethod on transaction detail page.
		/// </summary>
		/// <param name="startsWith"></param>
		/// <param name="maxRows"></param>
		/// <returns></returns>
		public List<string> GetBaseEntries( string startsWith, int maxRows )
		{
			var timeConverter = new SiteTimeConverter(this.transContext.accountingSite.CurrentSite);
			DateTimeOffset currentDateTime = timeConverter.Now();

			// Only gets fuel cards with active status (0).
			var dataSet = FMChannelHelper.MakeCall<IFuelCards, DataSet>(x => x.EnumerateFuelCardsForAutoComplete(transContext.security));

			var cardList = new List<string>();

			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				int itemCount = 0;
				DataTable table = dataSet.Tables[0];

				foreach(DataRow row in table.Rows)
				{
					string fuelCardId				= row.IsNull("ID") ? string.Empty : (string)row["ID"];
					DateTimeOffset? expirationDate	= row.IsNull("ExpirationDate") ? null : (DateTimeOffset?)row["ExpirationDate"];

					if (string.IsNullOrEmpty(fuelCardId) || expirationDate == null || currentDateTime > expirationDate.Value)
					{
						continue;
					}

					if (fuelCardId.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase))
					{
						cardList.Add(fuelCardId);
						itemCount++;
					}
		
					if (itemCount >= maxRows)
					{
						break;
					}
				}
			}

			return cardList;
		}

		/// <summary>
		/// This method is an override method that will return the contents of the control
		/// control.
		/// </summary>
		/// <param name="control">The fuel card ID web control.</param>
		/// <returns>Returns the fuel card ID value.</returns>
		public override object GetNewValue ( WebControl control )
		{
			var updatePanel = control.Controls[0] as UpdatePanel;

			if ( transContext.EnableAutoComplete )
			{
				TextBox textBox;

				if ( updatePanel != null )
				{
					textBox = updatePanel.ContentTemplateContainer.Controls[0] as FMCompanyTextBox;
				}
				else
				{
					textBox = (TextBox) control.Controls[0];
				}

				if (textBox != null)
				{
					return textBox.Text;
				}
			}
			
			if ( !this.transContext.aliasClass.UseComboxControls )
			{
				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMFuelCardTextBox;

					if (textBoxButtonCombo != null)
					{
						return textBoxButtonCombo.Text;
					}
				}

				return string.Empty;
			}

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

				string textValue = string.Empty;

				if (comboBox != null)
				{
					TextBox textBox = comboBox.TextBoxCntrl;

					if (textBox != null)
					{
						textValue = textBox.Text;
					}
				}

				return textValue;
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will return the fuel card ID from the transaction data object.
		/// </summary>
		/// <param name="transaction">Transaction data object.</param>
		/// <returns>Fuel card ID as an object.</returns>
		public object GetDataValue ( TransactionDO transaction )
		{
			return transaction.FuelCardID;
		}

		/// <summary>
		/// This method will return the fuel card ID from the transaction data object.
		/// </summary>
		/// <param name="transaction">Transaction data object.</param>
		/// <returns>Fuel card ID</returns>
		public string GetDataText ( TransactionDO transaction )
		{
			return transaction.FuelCardID;
		}

		/// <summary>
		/// This method will set the new value in the transaction data object.
		/// </summary>
		/// <param name="newValue">New value.</param>
		public virtual void SetValue ( object newValue )
		{
			Guid fuelCardGuid = Guid.Empty;

			trans.FuelCardID = string.Empty;

			if (newValue is string && newValue as string != string.Empty)
			{
				fuelCardGuid =
					FMChannelHelper.MakeCall<IFuelCards, Guid>(x => x.GetIdentityGuid(transContext.security, newValue as string));

				if (fuelCardGuid == Guid.Empty)
				{
					this.RenderErrorMessage(string.Format(ERR_MSG_001, newValue as string));
				}

				else
				{
					var fuelCard =
						FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(x => x.Get(transContext.security, fuelCardGuid, false));

					if (fuelCard.Status != FuelCardClass.Statuses.ACTIVE)
					{
						this.RenderErrorMessage(string.Format(ERR_MSG_002, fuelCard.ID));
					}

					else
					{

						var timeConverter = new SiteTimeConverter(this.transContext.accountingSite.CurrentSite);

						if (fuelCard.ExpirationDate.HasValue && timeConverter.Now() > fuelCard.ExpirationDate.Value)
						{
							this.RenderErrorMessage(string.Format(ERR_MSG_003, fuelCard.ID));
						}

						else
						{
							trans.FuelCardID = newValue as string;
						}
					}
				}
			}

			trans.FuelCardGuid = fuelCardGuid;
			object fieldValue = GetDataValue ( );

			if ( this.transContext.EnableAutoComplete )
			{
				TextBox textBox;
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if ( updatePanel != null )
				{
					updatePanel.Update();
					textBox = (TextBox) updatePanel.ContentTemplateContainer.Controls[0];
				}
				else
				{
					textBox = (TextBox) cell.Controls[0];
				}

				textBox.Text = fieldValue.ToString();
			}
			else if ( !transContext.aliasClass.UseComboxControls )
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMFuelCardTextBox;

					if (fieldValue != null)
					{
						if (textBoxButtonCombo != null)
						{
							textBoxButtonCombo.Text = fieldValue.ToString();
						}
					}
					else
					{
						if (textBoxButtonCombo != null)
						{
							textBoxButtonCombo.Text = string.Empty;
						}
					}
				}
			}
			else
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					updatePanel.Update();
					var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

					if (comboBox != null)
					{
						comboBox.Clear ( );
						comboBox.MaxLength = 0;
						comboBox.TextBoxCntrl.Columns = 0;
						comboBox.TextBoxCntrl.MaxLength = 0;
						comboBox.Items.Add ( new ListItem ( string.Empty, string.Empty ) );

						bool itemInList = false;

						var cards = this.GetEntries();

						foreach (FuelCardClass fuelCard in cards)
						{
							if (fuelCard.Status != FuelCardClass.Statuses.ACTIVE)
							{
								continue;
							}

							comboBox.Items.Add ( new ListItem ( fuelCard.ID, fuelCard.ID ) );

							if (!this.TransFieldConfiguration.FileFound)
							{
								if (fuelCard.ID.Length > comboBox.TextBoxCntrl.Columns && fuelCard.ID.Length < 50)
								{
									comboBox.MaxLength = fuelCard.ID.Length;
									comboBox.TextBoxCntrl.MaxLength = fuelCard.ID.Length;
									comboBox.TextBoxCntrl.Columns = fuelCard.ID.Length;
								}
							}

							if (fieldValue != null && fieldValue.ToString ( ) == fuelCard.ID)
							{
								comboBox.SelectedIndex = comboBox.Items.Count - 1;
								comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString (CultureInfo.InvariantCulture);
								comboBox.Text = fieldValue.ToString ( );
								itemInList = true;
							}
						}

						if (!itemInList && fieldValue != null && fieldValue.ToString ( ) != string.Empty)
						{
							if (!this.TransFieldConfiguration.FileFound)
							{
								if (fieldValue.ToString().Length > comboBox.TextBoxCntrl.Columns && fieldValue.ToString().Length < 50)
								{
									comboBox.MaxLength = fieldValue.ToString().Length;
									comboBox.TextBoxCntrl.MaxLength = fieldValue.ToString().Length;
									comboBox.TextBoxCntrl.Columns = fieldValue.ToString().Length;
								}
							}

							int insertIndex = 0;
							foreach (ListItem item in comboBox.Items)
							{
								if (item.Text != string.Empty && item.Text.CompareTo(fieldValue.ToString()) > 0)
								{
									comboBox.Items.Insert(insertIndex, new ListItem(fieldValue.ToString(), fieldValue.ToString()));
									break;
								}

								insertIndex++;
							}

							if (insertIndex == comboBox.Items.Count)
							{
								comboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));
							}

							comboBox.SelectedIndex = insertIndex;
							comboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
							comboBox.Text = fieldValue.ToString();
						}

						if (comboBox.TextBoxCntrl.MaxLength == 0)
						{
							comboBox.MaxLength = 50;
							comboBox.TextBoxCntrl.MaxLength = 50;
							comboBox.TextBoxCntrl.Columns = 50;
						}
					}
				}
			}

			// Calls the on change event if one is registered.
			OnFieldChanged ( );
		}

		/// <summary>
		/// This method will set the new value of the control into the transaction
		/// data object.
		/// </summary>
		/// <param name="transaction">Transaction data object.</param>
		/// <param name="newValue">New value from user entry.</param>
		public virtual void SetDataValue ( TransactionDO transaction, object newValue )
		{
			// Sets the value in the control and sets the Transaction Guid.
			this.SetValue ( newValue );

			if (transaction.FuelCardGuid == Guid.Empty)
			{
				if (transContext.aliasClass.TransactionFieldCollection.Find ( "DestinationRegistrationID1" ) != null)
				{
					var destinationFG1 = fieldGenerator.GetFieldGenerator ( "DestinationRegistrationID1" ) as DestinationEquipmentFG;
					
					if (destinationFG1 != null)
					{
						destinationFG1.SetValue ( transaction.DestinationEQ1.RegistrationID );
					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find ( "DestinationRegistrationID2" ) != null)
				{
					var destinationFG2 = fieldGenerator.GetFieldGenerator ( "DestinationRegistrationID2" ) as DestinationEquipmentFG;
					
					if (destinationFG2 != null)
					{
						destinationFG2.SetValue ( transaction.DestinationEQ2.RegistrationID );
					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find ( "DestinationRegistrationID3" ) != null)
				{
					var destinationFG3 = fieldGenerator.GetFieldGenerator ( "DestinationRegistrationID3" ) as DestinationEquipmentFG;
					
					if (destinationFG3 != null)
					{
						destinationFG3.SetValue ( transaction.DestinationEQ3.RegistrationID );
					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("ManagerID") != null)
				{
					var managerFG = fieldGenerator.GetFieldGenerator("ManagerID") as CompanyTextButtonGenerator;

					if (managerFG != null)
					{
						managerFG.SetValue(this.trans.ManagerID);
					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("OwnerID") != null)
				{
					var ownerFG = fieldGenerator.GetFieldGenerator("OwnerID") as CompanyTextButtonGenerator;

					if (ownerFG != null)
					{
						ownerFG.SetValue(this.trans.OwnerID);
					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("ShipperID") != null)
				{
					var shipperFG = fieldGenerator.GetFieldGenerator("ShipperID") as CompanyTextButtonGenerator;

					if (shipperFG != null)
					{
						shipperFG.SetValue(this.trans.ShipperID);
					}
				}
				else
				{
					trans.ShipperID = string.Empty;
					trans.ShipperCode = string.Empty;
					trans.ShipperCompanyGuid = Guid.Empty;
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("BillToID") != null)
				{
					var billToFG = fieldGenerator.GetFieldGenerator("BillToID") as CompanyTextButtonGenerator;

					if (billToFG != null)
					{
						billToFG.SetValue(this.trans.BillToID);
					}
				}
				else
				{
					trans.BillToID = string.Empty;
					trans.BillToCode = string.Empty;
					trans.BillToCompanyGuid = Guid.Empty;
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("ShipToID") != null)
				{
					var shipToFG = fieldGenerator.GetFieldGenerator("ShipToID") as CompanyTextButtonGenerator;

					if (shipToFG != null)
					{
						shipToFG.SetValue(this.trans.ShipToID);
					}
				}
				else
				{
					trans.ShipToID = string.Empty;
					trans.ShipToCode = string.Empty;
					trans.ShipToCompanyGuid = Guid.Empty;
				}
			}
			else
			{
				FuelCardClass fuelCard = FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(
																	 x =>  x.Get(transContext.security, transaction.FuelCardGuid, true ));

				this.PopulateDependentFields(transaction, fuelCard);
			}
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// This method will populate the dependent fields when there is a fuel card GUID.
		/// </summary>
		/// <param name="transaction">Contains the transaction data object.</param>
		/// <param name="fuelCard">Contains the fuel card data object.</param>
		protected void PopulateDependentFields(TransactionDO transaction, FuelCardClass fuelCard)
		{
			if (transContext.aliasClass.TransactionFieldCollection.Find("ManagerID") != null)
			{
				var managerFG = fieldGenerator.GetFieldGenerator("ManagerID") as CompanyTextButtonGenerator;

				if (managerFG != null && string.IsNullOrEmpty(fuelCard.ManagerID) == false)
				{
					managerFG.SetValue(fuelCard.ManagerID);
				}
			}
			else
			{
				transaction.ManagerID = fuelCard.ManagerID;
				transaction.ManagerCode = fuelCard.ManagerCode;
				transaction.ManagerCompanyGuid = fuelCard.ManagerGuid;
			}

			if (transContext.aliasClass.TransactionFieldCollection.Find("OwnerID") != null)
			{
				var ownerFG = fieldGenerator.GetFieldGenerator("OwnerID") as CompanyTextButtonGenerator;

				if (ownerFG != null && string.IsNullOrEmpty(fuelCard.OwnerID) == false)
				{
					ownerFG.SetValue(fuelCard.OwnerID);
				}
			}
			else
			{
				transaction.OwnerID = fuelCard.OwnerID;
				transaction.OwnerCode = fuelCard.OwnerCode;
				transaction.OwnerCompanyGuid = fuelCard.OwnerGuid;
			}

			if (transContext.aliasClass.TransactionFieldCollection.Find("ShipperID") != null)
			{
				var shipperFG = fieldGenerator.GetFieldGenerator("ShipperID") as CompanyTextButtonGenerator;

				if (shipperFG != null && string.IsNullOrEmpty(fuelCard.ShipperID) == false)
				{
					shipperFG.SetValue(fuelCard.ShipperID);
				}
			}
			else
			{
				transaction.ShipperID = fuelCard.ShipperID;
				transaction.ShipperCode = fuelCard.ShipperCode;
				transaction.ShipperCompanyGuid = fuelCard.ShipperGuid;
			}

			if (transContext.aliasClass.TransactionFieldCollection.Find("BillToID") != null)
			{
				var billToFG = fieldGenerator.GetFieldGenerator("BillToID") as CompanyTextButtonGenerator;

				if (billToFG != null && string.IsNullOrEmpty(fuelCard.BillToID) == false)
				{
					billToFG.SetValue(fuelCard.BillToID);
				}
			}
			else
			{
				transaction.BillToID = fuelCard.BillToID;
				transaction.BillToCode = fuelCard.BillToCode;
				transaction.BillToCompanyGuid = fuelCard.BillToGuid;
			}

			if (transContext.aliasClass.TransactionFieldCollection.Find("ShipToID") != null)
			{
				var shipToFG = fieldGenerator.GetFieldGenerator("ShipToID") as CompanyTextButtonGenerator;

				if (shipToFG != null && string.IsNullOrEmpty(fuelCard.ShipToID) == false)
				{
					shipToFG.SetValue(fuelCard.ShipToID);
				}
			}
			else
			{
				transaction.ShipToID = fuelCard.ShipToID;
				transaction.ShipToCode = fuelCard.ShipToCode;
				transaction.ShipToCompanyGuid = fuelCard.ShipToGuid;
			}

			// Populate the card expiration date with the expiration date in the fuel card.
			if (transContext.aliasClass.TransactionFieldCollection.Find("CardExpiration") != null)
			{
				var cardExpirationFG = fieldGenerator.GetFieldGenerator("CardExpiration") as CardExpirationFG;

				if (cardExpirationFG != null)
				{
					cardExpirationFG.SetDataValue(transaction, fuelCard.ExpirationDate);
					cardExpirationFG.SetDisplayValue(fuelCard.ExpirationDate);
				}
			}
			else
			{
				transaction.PaymentInfo.CreditCardExpiration = fuelCard.ExpirationDate;
			}

			// Populate the card name with the Provider field in the fuel card.
			if (transContext.aliasClass.TransactionFieldCollection.Find("CardName") != null)
			{
				var cardNameFG = fieldGenerator.GetFieldGenerator("CardName") as CardNameFG;

				if (cardNameFG != null && string.IsNullOrEmpty(fuelCard.Provider) == false)
				{
					cardNameFG.SetDataValue(transaction, fuelCard.Provider);
					cardNameFG.SetDisplayValue(fuelCard.Provider);
				}
			}
			else
			{
				transaction.PaymentInfo.CreditCardName = fuelCard.Provider;
			}

			// Populate the card name with the Provider field in the fuel card.
			if (transContext.aliasClass.TransactionFieldCollection.Find("CardType") != null)
			{
				var cardTypeFG = fieldGenerator.GetFieldGenerator("CardType") as CardTypeFG;

				if (cardTypeFG != null && string.IsNullOrEmpty(fuelCard.FuelCardTypeApplicationStringID) == false)
				{
					// The fuel card type comes from the application string.
					cardTypeFG.SetDataValue(transaction, fuelCard.FuelCardTypeApplicationStringID);
					cardTypeFG.SetDisplayValue(fuelCard.FuelCardTypeApplicationStringID);
				}
			}
			else
			{
				// The fuel card type comes from the application string.
				transaction.PaymentInfo.CreditCardType = fuelCard.FuelCardTypeApplicationStringID;
			}

			// Transactions in which the Destination Equipment may be associated with a FuelCard
			if (transContext.aliasClass.TransTypeID == TransactionTypes.T5_PrimaryDisbursement ||
				transContext.aliasClass.TransTypeID == TransactionTypes.T6_SecondaryDisbursement)
			{
				if (transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID1") != null)
				{
					var destinationFG1 = fieldGenerator.GetFieldGenerator("DestinationRegistrationID1") as DestinationEquipmentFG;

					if (destinationFG1 != null)
					{
						if (fuelCard.EquipmentCollection.Count == 1
								 && this.transContext.aliasClass.IncludeType(true, 1, fuelCard.EquipmentCollection[0].Type))
						{
							destinationFG1.SetValue(fuelCard.EquipmentCollection[0].ID);
						}

						else if (fuelCard.EquipmentCollection.Count > 0)
						{
							if (fuelCard.EquipmentCollection.Find(T => T.ID.Equals(transaction.DestinationEQ1.RegistrationID)) == null)
							{
								destinationFG1.SetValue(string.Empty);
							}
							else
							{
								destinationFG1.SetValue(transaction.DestinationEQ1.RegistrationID);
							}
						}
						else
						{
							destinationFG1.SetValue(string.Empty);
						}
					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID2") != null)
				{
					var destinationFG2 = fieldGenerator.GetFieldGenerator("DestinationRegistrationID2") as DestinationEquipmentFG;

					if (destinationFG2 != null)
					{
						if (fuelCard.EquipmentCollection.Count == 1
						         && this.transContext.aliasClass.IncludeType(true, 2, fuelCard.EquipmentCollection[0].Type))
						{
							destinationFG2.SetValue(fuelCard.EquipmentCollection[0].ID);
						}

						else if (fuelCard.EquipmentCollection.Count > 0)
						{
							if (fuelCard.EquipmentCollection.Find(T => T.ID.Equals(transaction.DestinationEQ2.RegistrationID)) == null)
							{
								destinationFG2.SetValue(string.Empty);
							}
							else
							{
								destinationFG2.SetValue(transaction.DestinationEQ2.RegistrationID);
							}
						}
						else
						{
							destinationFG2.SetValue(string.Empty);
						}
					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID3") != null)
				{
					var destinationFG3 = fieldGenerator.GetFieldGenerator("DestinationRegistrationID3") as DestinationEquipmentFG;

					if (destinationFG3 != null)
					{
						if (fuelCard.EquipmentCollection.Count == 1
								 && this.transContext.aliasClass.IncludeType(true, 3, fuelCard.EquipmentCollection[0].Type))
						{
							destinationFG3.SetValue(fuelCard.EquipmentCollection[0].ID);
						}

						else if (fuelCard.EquipmentCollection.Count > 0)
						{
							if (fuelCard.EquipmentCollection.Find(T => T.ID.Equals(transaction.DestinationEQ3.RegistrationID)) == null)
							{
								destinationFG3.SetValue(string.Empty);
							}
							else
							{
								destinationFG3.SetValue(transaction.DestinationEQ3.RegistrationID);
							}
						}
						else
						{
							destinationFG3.SetValue(string.Empty);
						}
					}
				}
			}

			// Transactions in which the Source Equipment may be associated with a FuelCard
			if (transContext.aliasClass.TransTypeID == TransactionTypes.T3_PrimaryDefuel
				|| transContext.aliasClass.TransTypeID == TransactionTypes.T4_SecondaryDefuel)
			{
				if (transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID1") != null)
				{
					var sourceFG1 = fieldGenerator.GetFieldGenerator("SourceRegistrationID1") as SourceEquipmentFG;

					if (sourceFG1 != null)
					{
						if (fuelCard.EquipmentCollection.Count == 1
								 && this.transContext.aliasClass.IncludeType(false, 1, fuelCard.EquipmentCollection[0].Type))
						{
							sourceFG1.SetValue(fuelCard.EquipmentCollection[0].ID);
						}

						else if (fuelCard.EquipmentCollection.Count > 0)
						{
							if (fuelCard.EquipmentCollection.Find(T => T.ID.Equals(transaction.SourceEQ1.RegistrationID)) == null)
							{
								sourceFG1.SetValue(string.Empty);
							}
							else
							{
								sourceFG1.SetValue(transaction.SourceEQ1.RegistrationID);
							}
						}
						else
						{
							sourceFG1.SetValue(string.Empty);
						}

					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID2") != null)
				{
					var sourceFG2 = fieldGenerator.GetFieldGenerator("SourceRegistrationID2") as SourceEquipmentFG;

					if (sourceFG2 != null)
					{
						if (fuelCard.EquipmentCollection.Count == 1
							 && this.transContext.aliasClass.IncludeType(false, 2, fuelCard.EquipmentCollection[0].Type))
						{
							sourceFG2.SetValue(fuelCard.EquipmentCollection[0].ID);
						}

						else if (fuelCard.EquipmentCollection.Count > 0)
						{
							if (fuelCard.EquipmentCollection.Find(T => T.ID.Equals(transaction.SourceEQ2.RegistrationID)) == null)
							{
								sourceFG2.SetValue(string.Empty);
							}
							else
							{
								sourceFG2.SetValue(transaction.SourceEQ2.RegistrationID);
							}
						}
						else
						{
							sourceFG2.SetValue(string.Empty);
						}
					}
				}

				if (transContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID3") != null)
				{
					var sourceFG3 = fieldGenerator.GetFieldGenerator("SourceRegistrationID3") as SourceEquipmentFG;

					if (sourceFG3 != null)
					{
						if (fuelCard.EquipmentCollection.Count == 1
								 && this.transContext.aliasClass.IncludeType(false, 3, fuelCard.EquipmentCollection[0].Type))
						{
							sourceFG3.SetValue(fuelCard.EquipmentCollection[0].ID);
						}

						else if (fuelCard.EquipmentCollection.Count > 0)
						{
							if (fuelCard.EquipmentCollection.Find(T => T.ID.Equals(transaction.SourceEQ3.RegistrationID)) == null)
							{
								sourceFG3.SetValue(string.Empty);
							}
							else
							{
								sourceFG3.SetValue(transaction.SourceEQ3.RegistrationID);
							}
						}
						else
						{
							sourceFG3.SetValue(string.Empty);
						}
					}
				}
			}
		}

		/// <summary>
		/// This method handles a text change event.
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event argument list.</param>
		protected void TextChanged ( object sender, EventArgs e )
		{
			if ( this.transContext.EnableAutoComplete )
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;
				var textBox = updatePanel != null ? updatePanel.ContentTemplateContainer.Controls[0] as TextBox : cell.Controls[0] as TextBox;

				if (textBox != null)
				{
					this.SetDataValue( this.trans, textBox.Text );
				}
			}
			else if ( !transContext.aliasClass.UseComboxControls )
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBoxButtonCombo = updatePanel.ContentTemplateContainer.Controls[0] as FMFuelCardTextBox;
				
					if (textBoxButtonCombo != null)
					{
						this.SetDataValue ( this.trans, textBoxButtonCombo.Text );
					}
				}
			}
			else
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
                    var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

					if (comboBox != null)
					{
						this.SetDataValue ( this.trans, comboBox.Text );
					}
				}
			}
		}

		/// <summary>
		/// This method sets up a text change event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Insert event argument list.</param>
		protected void ItemInserted ( object sender, ComboBoxItemInsertEventArgs e )
		{
			var updatePanel = cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

				if (comboBox != null)
				{
					comboBox.TextBoxCntrl.TextChanged += this.TextChanged;
				}
			}
		}
		#endregion
	}
}
