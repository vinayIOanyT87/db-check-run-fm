// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecirculationForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// The Recirculation Form is used by dispatch personnel to enter data about Recirculations of fuel.
// Recirculations do not affect the inventory, since the fuel isn't being consumed, just recirculated
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Linq;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

    using FMCore;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// The Recirculation Form is used by dispatch personnel to enter data about Recirculations of fuel.
	/// Recirculations do not affect the inventory, since the fuel isn't being consumed, just recirculated
	/// </summary>
	public partial class RecirculationForm : FMFormBase
	{
		#region Constants
		/// <summary>
		/// Identifies the name of the page request parameter you can provide to load an existing record
		/// </summary>
		public const string TransactionGuidRequestParameterName = "TransactionGuid";

		/// <summary>
		/// The type of transaction alias that should be used for recirculation transactions
		/// </summary>
		private const TransactionTypes RecirculationTransactionType = TransactionTypes.T12_InventoryNotAffected;
		#endregion

		#region Session Properties
		/// <summary>
		/// Represents the transaction that we loaded or created when the form opened
		/// </summary>
		public TransactionDO SessionTransaction
		{
			get
			{
				if (this.Session["RecirculationTransaction"] is TransactionDO)
				{
					return this.Session["RecirculationTransaction"] as TransactionDO;
				}
				
				return new TransactionDO { SubmittedToAccounting = false };
			}

			set
			{
				this.Session.Add("RecirculationTransaction", value);
			}
		}

		/// <summary>
		/// Represents the site we're currently logged into
		/// </summary>
		public SiteClass SessionSite
		{
			get
			{
				if (this.Session["RecirculationSite"] is SiteClass)
				{
					return this.Session["RecirculationSite"] as SiteClass;
				}
				
				return new SiteClass();
			}

			set
			{
				this.Session.Add("RecirculationSite", value);
			}
		}

		#endregion

		#region Form Properties and Methods

		/// <summary>
		/// If the form is read only the OK and Apply buttons should be disabled
		/// </summary>
		private bool ReadOnly
		{
			set
			{
				this.OKButton.Enabled = !value;
				this.ApplyButton.Enabled = !value;
			}
		}

		/// <summary>
		/// Show a message to the user on top of the form displaying the specified text.
		/// </summary>
		/// <param name="alertMessage">The message to show</param>
		public void ShowAlert(string alertMessage)
		{
			ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertScript", 
											"ShowAlertDialog('" + HttpUtility.JavaScriptStringEncode(alertMessage) + "');", true);
		}

		/// <summary>
		/// Set the focus on the specified control, also changing tabs if necessary.
		/// You must provide the index of the tab the control is on
		/// </summary>
		/// <param name="control">The control to set focus on</param>
		public void SetFocusOnControl(Control control)
		{
			string controlID = control.ClientID;

			// To set focus properly on combo boxes, we must alter the client id
			// The name of the combo box's text box is the normal name + (name of control)_(TextBox)
			// e.g. tcFuelRequest_tpServiceRequestPage_FuelRequestServiceRequestPage_AircraftIDComboBox_AircraftIDComboBox_TextBox
			if (control is FMComboBox)
			{
				controlID += "_" + control.ID + "_TextBox";
			}
			else if (control is FMDateTime)
			{
				// Setting focus on the FMDateTime control requires adding " Date Month" to the control name,
				// that way we set focus on the month text box
				controlID += " Date Month";
			}

			ScriptManager.RegisterStartupScript(
				this, this.GetType(), "FocusScript", "SetFocus('" + HttpUtility.JavaScriptStringEncode(controlID) + "');", true);
		}

		/// <summary>
		/// Figure out which transaction alias we should use when creating Recirculation transactions.
		/// </summary>
		/// <returns>The transaction alias to use for recirculation transactions. This method will throw if a corresponding
		/// transaction alias is not found</returns>
		public TransactionAliasClass DetermineTransactionAlias()
		{
			try
			{
				// Get all of the transaction aliases that are configured for Dispatch
				TransactionAliasNameCollectionClass transactionAliases = 
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
								transactionAliasesServiceClass => transactionAliasesServiceClass.EnumerateDispatchAliasNames(this.Security));

				// Find the transaction alias which has a transaction alias type which corresponds to the transaction alias type to use
				// for recirculatioms
				TransactionAliasNameClass matchingAliasName = transactionAliases.Find(
												aliasName => aliasName.TransTypeID == RecirculationTransactionType);

				if (matchingAliasName != null)
				{
					TransactionAliasClass transactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
						transactionAliasesServiceClass => 
									transactionAliasesServiceClass.Get(this.Security, matchingAliasName.IdentityGuid, false));

					if (transactionAlias == null)
					{
						// We couldn't find a transaction alias configured for dispatch with the corresponding transaction alias type
						throw new Exception("Could not determine the transaction alias to use for recirculation transactions");
					}

					return transactionAlias;
				}

				// We couldn't find a transaction alias configured for dispatch with the corresponding transaction alias type
				throw new Exception("Could not determine the transaction alias to use for recirculation transactions");
			}
			catch (Exception)
			{
				this.ReadOnly = true;
				throw;
			}
		}

		/// <summary>
		/// Retrieve the site we're logged into, we use it primarily for time conversion
		/// </summary>
		private void LoadSite()
		{
			this.SessionSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
									sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));
		}
		#endregion


		#region Form Events

		/// <summary>
		/// When the form loads, populate the controls and display the existing transaction record, if a transaction guid was provided
		/// If a guid was not provided, create a new transaction record
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!Page.IsPostBack)
				{
					this.ReadOnly = !this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

					this.LoadSite();

					this.PopulateControls();

					// If the user provided a transaction identifier, load that transaction
					// Otherwise, create a new one
					if (this.Request.GetQueryOrFormValue(FuelRequestForm.TransactionGuidRequestParameterName) != null)
					{
						Guid transactionGuid;
						Guid.TryParse(this.Request.GetQueryOrFormValue(TransactionGuidRequestParameterName), 
																		out transactionGuid);

						TransactionSR sr = new TransactionSR
											   {
												   Security = this.Security,
												   TransactionGuid = transactionGuid,
												   AllowCrossSiteTransactions = true,
												   ConvertUnits = true
											   };

						TransactionDO matchingTransaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																			processor => processor.Process(sr));

						if (matchingTransaction != null)
						{
							this.SessionTransaction = matchingTransaction;
						}
						else
						{
							throw new Exception("Could not find the transaction identified by the transaction guid: " 
												+ transactionGuid);
						}
					}
					else
					{
						TransactionDO newTransaction = new TransactionDO { SubmittedToAccounting = false };
						newTransaction.LineItems.Add(new LineItemDO());

						newTransaction.Site = this.Security.SiteID;
						newTransaction.SiteGuid = this.Security.SiteGuid;

						this.SessionTransaction = newTransaction;
					}

					this.DisplayTransaction(this.SessionTransaction);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the OK button is clicked, save the data and close the form
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (!this.ValidateTransactionData())
				{
					return;
				}

				TransactionDO transaction = this.SessionTransaction;

				this.SaveTransactionData(transaction);

				this.ClientScript.RegisterStartupScript(this.GetType(), "CloseScript", "window.close();", true);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the user presses Apply, save the data and then open the form again to clear out the existing data
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void ApplyButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (!this.ValidateTransactionData())
				{
					return;
				}

				TransactionDO transaction = this.SessionTransaction;

				this.SaveTransactionData(transaction);

				this.Redirect("RecirculationForm.aspx");
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion

		#region Form Control Population
		/// <summary>
		/// Fill the drop down lists and datetime controls on the form with data
		/// </summary>
		private void PopulateControls()
		{
			// Populate the Operator drop down list with drivers
			this.OperatorDropDownList.DataSource = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
													personnel => personnel.EnumerateByRole(this.Security, PERSON_ROLE.LOADER_ROLE));

			this.OperatorDropDownList.DataBind();
			this.OperatorDropDownList.Items.Add(new ListItem(string.Empty, Guid.Empty.ToString()));

			// Populate products.
			this.ProductDropDownList.DataSource = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
													products => products.Enumerate(this.Security));

			this.ProductDropDownList.DataBind();
			this.ProductDropDownList.Items.Add(new ListItem(string.Empty, Guid.Empty.ToString()));

			// Populate RegistrationID with equipment records
			this.RegistrationIDDropDownList.DataSource = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
															equipments => equipments.EnumerateManagedEquipment(this.Security));

			this.RegistrationIDDropDownList.DataBind();
			this.RegistrationIDDropDownList.Items.Add(new ListItem(string.Empty, Guid.Empty.ToString()));

			SiteClass site = this.SessionSite;
			DateTimeOffset siteDateTime = TimeConverter.Now(site);

			this.StartDateTimeControl.FormatInfo = site.GetDateTimeFormatInfo();
			this.StartDateTimeControl.Text = siteDateTime.ToString();

			this.StopDateTimeControl.FormatInfo = site.GetDateTimeFormatInfo();
			this.StopDateTimeControl.Text = siteDateTime.ToString();
		}
		#endregion

		#region Transaction Record Display and Creation
		/// <summary>
		/// Use the controls on the page to display data from a FuelsManager transaction record.
		/// </summary>
		/// <param name="transaction">The transaction record to display</param>
		private void DisplayTransaction(TransactionDO transaction)
		{
			SiteClass site = this.SessionSite;

			// By attempting to determine the transaction alias to use now, we 
			// can display a message to the user before they enter any data
			// if there is no configured Recirculation transaction alias
			this.DetermineTransactionAlias();

			// The transaction cannot be modified if it's been submitted to accounting, or if the site that is viewing the transaction
			// is not the site that owns it
			if (transaction.SubmittedToAccounting == true || transaction.SiteGuid != this.Security.SiteGuid)
			{
				this.ReadOnly = true;
			}

			// The apply button is disabled for existing transactions.
			if (transaction.TransactionGuid != Guid.Empty)
			{
				this.ApplyButton.Enabled = false;
			}

			string recirculationType;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_02, out recirculationType);
			this.TypeDropDownList.SelectByText(recirculationType);

			this.RegistrationIDDropDownList.SelectedValue = transaction.SourceEQ1.EquipmentGuid != Guid.Empty 
															? transaction.SourceEQ1.EquipmentGuid.ToString() : Guid.Empty.ToString();

			this.StartDateTimeControl.Text = transaction.RouteSchedule.FST.HasValue ? transaction.RouteSchedule.FST.ToString() : string.Empty;

			this.OperatorDropDownList.SelectedValue = transaction.OperatorPersonnelGuid != Guid.Empty 
														? transaction.OperatorPersonnelGuid.ToString() : Guid.Empty.ToString();

			this.StopDateTimeControl.Text = transaction.TimeEnd.HasValue ? transaction.TimeEnd.ToString() : string.Empty;

			LineItemDO lineItem = transaction.LineItems.Find(matchingLineItem => matchingLineItem.DeleteFlag == false);

			if (lineItem != null && lineItem.ProductGuid != Guid.Empty)
			{
				this.ProductDropDownList.SelectedValue = lineItem.ProductGuid.ToString();
			}
			else
			{
				this.ProductDropDownList.SelectedValue = Guid.Empty.ToString();
			}

			string bosText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_19, out bosText);
			this.BOSTextBox.Text = bosText;

			this.CardNumberTextBox.Text = transaction.FuelCardID;

			this.IssuePointNumberTextBox.Text = transaction.IssuePointNumber;
			this.IssuePointTextBox.Text = transaction.IssuePoint;

			string serialNumberText;
			transaction.UserData.TryGetValue(TransactionDO.USER_DATA_KEY_04, out serialNumberText);
			this.SerialNumberTextBox.Text = serialNumberText;

			if (lineItem != null)
			{
				this.NetVolumeTextBox.Text = lineItem.Quantity.Net.ToString(site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
				this.GrossVolumeTextBox.Text = lineItem.Quantity.Gross.ToString(site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
			}

			this.MemoTextBox.Text = transaction.Notes;
		}


		private TransactionOrigin DetermineOriginApplication()
		{
			return this.IsEnterprise ? TransactionOrigin.DispatchEnterprise : TransactionOrigin.Dispatch;
		}

		/// <summary>
		/// Set values in a FuelsManager transaction record using data from the controls on the page
		/// </summary>
		/// <param name="transaction">The FuelsManager Transaction record to populate with data</param>
		private void SaveTransactionData(TransactionDO transaction)
		{
			SiteClass site = this.SessionSite;
			DateTimeOffset siteDateTime = TimeConverter.Now(site);

			var transactionAlias = this.DetermineTransactionAlias();
			var documentNumberGenerator = new DocumentNumberGenerator(this.Security);

			if (transaction.TransactionGuid == Guid.Empty)
			{
				transaction.Alias = transactionAlias.ID;
				transaction.TransTypeID = transactionAlias.TransTypeID;
				transaction.TransactionAliasGuid = transactionAlias.MasterRecordGuid;

				transaction.DocumentNumber = documentNumberGenerator.GetNextDocumentNumber(transactionAlias.TransTypeID);

				transaction.InventoryDate = siteDateTime.DateTime;
				transaction.TransactionDateTime = siteDateTime;



				transaction.OriginApplication = this.DetermineOriginApplication();

				transaction.RequestedDateTime = siteDateTime;

				CompanyCollectionClass managerCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
											companies => companies.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, false, false));

				if (managerCollection.Count == 0)
				{
					throw new Exception("No Manager");
				}

				if (managerCollection.Count > 1)
				{
					string managerNames = string.Join<string>(", ", managerCollection.Select<CompanyClass, string>(company => company.ID));

					string errorMsg = string.Format("Multiple managers are not allowed. {0} managers were found. They are {1}.",
													managerCollection.Count, managerNames);

					throw new Exception(errorMsg);
				}

				transaction.ManagerID = managerCollection[0].ID;
				transaction.ManagerCode = managerCollection[0].Code;
				transaction.ManagerCompanyGuid = managerCollection[0].MasterRecordGuid;

				CompanyCollectionClass ownerCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
										companies => companies.EnumerateByRole(this.Security, COMPANY_ROLE.OWNER, false, false));

				if (ownerCollection.Count == 0)
				{
					throw new Exception("No Owner");
				}

				if (ownerCollection.Count > 1)
				{
					string ownerNames = string.Join<string>(", ", ownerCollection.Select<CompanyClass, string>(company => company.ID));

					string errorMsg = string.Format("Multiple owners are not allowed. {0} owners were found. They are {1}.", ownerCollection.Count, ownerNames);

					throw new Exception(errorMsg);
				}

				transaction.OwnerID = ownerCollection[0].ID;
				transaction.OwnerCode = ownerCollection[0].Code;
				transaction.OwnerCompanyGuid = ownerCollection[0].MasterRecordGuid;

				transaction.TransID = FuelsManagerId.NewId();
				transaction.Status = TransactionStatus.Completed;

				foreach (LineItemDO lineItemToComplete in transaction.LineItems)
				{
					lineItemToComplete.Status = TransactionStatus.Completed;
				}
			}

			LineItemDO lineItem = transaction.LineItems.Find(matchingLineItem => matchingLineItem.DeleteFlag == false);

			transaction.UserData2 = this.TypeDropDownList.SelectedItem.Text;

			if (!string.IsNullOrEmpty(this.StartDateTimeControl.Text))
			{
				transaction.RouteSchedule.FST = this.StartDateTimeControl.CurrentValue;
			}

			if (!string.IsNullOrEmpty(this.StopDateTimeControl.Text))
			{
				transaction.TimeEnd = this.StopDateTimeControl.CurrentValue;
			}

			EquipmentDO equipmentDO = transaction.SourceEQ1;
			EquipmentDO lineItemEquipmentDO = transaction.SourceEQ1;

			equipmentDO.EquipmentGuid = Guid.Empty;
			equipmentDO.RegistrationID = string.Empty;
			equipmentDO.EquipmentType = string.Empty;

			lineItemEquipmentDO.EquipmentGuid = Guid.Empty;
			lineItemEquipmentDO.RegistrationID = string.Empty;
			lineItemEquipmentDO.EquipmentType = string.Empty;

			if (this.RegistrationIDDropDownList.SelectedItem != null 
				&& !string.IsNullOrEmpty(this.RegistrationIDDropDownList.SelectedItem.Text))
			{
				equipmentDO.RegistrationID = this.RegistrationIDDropDownList.SelectedItem.Text;
				lineItemEquipmentDO.RegistrationID = equipmentDO.RegistrationID;

				Guid equipmentGuid;
				Guid.TryParse(this.RegistrationIDDropDownList.SelectedItem.Value, out equipmentGuid);

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

			ProductClass product = null;

			if (this.ProductDropDownList.SelectedItem != null && !string.IsNullOrEmpty(this.ProductDropDownList.SelectedItem.Text))
			{
				lineItem.Product = this.ProductDropDownList.SelectedItem.Text;
				lineItem.ProductGuid = Guid.Empty;

				Guid productGuid;
				Guid.TryParse(this.ProductDropDownList.SelectedItem.Value, out productGuid);

				if (productGuid != Guid.Empty)
				{
					product = FMChannelHelper.MakeCall<IProducts, ProductClass>(products => products.Get(this.Security, productGuid));

					lineItem.ProductGuid = product.MasterRecordGuid;
				}
			}

			transaction.OperatorID = string.Empty;
			transaction.OperatorName = string.Empty;
			transaction.OperatorPersonnelGuid = Guid.Empty;

			if (this.OperatorDropDownList.SelectedItem != null && !string.IsNullOrEmpty(this.OperatorDropDownList.SelectedItem.Text))
			{
				transaction.OperatorID = this.OperatorDropDownList.SelectedItem.Text;
				transaction.OperatorName = this.OperatorDropDownList.SelectedItem.Text;
				transaction.OperatorPersonnelGuid = Guid.Empty;

				Guid personnelGuid;
				Guid.TryParse(this.OperatorDropDownList.SelectedItem.Value, out personnelGuid);

				if (personnelGuid != Guid.Empty)
				{
					PersonClass theOperator = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
										personnel => personnel.Get(this.Security, Guid.Parse(this.OperatorDropDownList.SelectedItem.Value)));

					if (theOperator.IdentityGuid != Guid.Empty)
					{
						transaction.OperatorID = theOperator.ID;
						transaction.OperatorName = theOperator.FullName;
						transaction.OperatorPersonnelGuid = theOperator.MasterRecordGuid;
					}
				}
			}

			transaction.UserData1 = "U.S. Gallons";

			transaction.UserData4			= this.SerialNumberTextBox.Text;
			transaction.IssuePoint			= this.IssuePointTextBox.Text;
			transaction.IssuePointNumber	= this.IssuePointNumberTextBox.Text;
			transaction.UserData19			= this.BOSTextBox.Text;

			transaction.FuelCardID = this.CardNumberTextBox.Text;
			transaction.FuelCardGuid = Guid.Empty;

			double grossVolume;

			if (double.TryParse(this.GrossVolumeTextBox.Text, out grossVolume))
			{
				lineItem.Quantity.Gross = grossVolume;

				// The old form also populated Number03 with the gross volume
				transaction.Number03 = grossVolume;
			}

			double netVolume;

			if (double.TryParse(this.NetVolumeTextBox.Text, out netVolume))
			{
				lineItem.Quantity.Net = netVolume;
			}

			transaction.Notes = this.MemoTextBox.Text;

			UnitsHelperClass unitsHelper = new UnitsHelperClass(this.Security, site, transactionAlias, product);
			unitsHelper.SetUnits(transaction, ProductType.ComponentProduct);
			unitsHelper.SetUnits(lineItem, ProductType.ComponentProduct, product);

			SaveTransactionsSR saveTransactionServiceRequest = new SaveTransactionsSR
				                                                   {
					                                                   Security = this.Security,
					                                                   ConvertUnits = true
				                                                   };

			saveTransactionServiceRequest.Transactions.Add(transaction);
			saveTransactionServiceRequest.CurrentSiteGuid = transaction.SiteGuid;

			FMChannelHelper.MakeCall<ISaveTransactionsProcessor>(
										transactionClient => transactionClient.SaveTransactions(saveTransactionServiceRequest));
		}
		#endregion

		#region Data Validation
		/// <summary>
		/// Check the data input on the form to make sure that required fields are present
		/// and the data is valid
		/// </summary>
		/// <returns>True if everything is OK, false otherwise</returns>
		private bool ValidateTransactionData()
		{
			if (!this.CheckRequiredFields()
				|| !this.ValidateVolumesAreGreaterThanZero()
				|| !this.CheckDateTimes())
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Make sure that all required fields are present
		/// </summary>
		/// <returns>True if all required fields are present, false otherwise</returns>
		private bool CheckRequiredFields()
		{
			if (this.TypeDropDownList.SelectedItem == null || string.IsNullOrEmpty(this.TypeDropDownList.SelectedItem.Text))
			{
				this.ShowAlert("Recirculation Type must be provided");
				this.SetFocusOnControl(this.TypeDropDownList);
				return false;
			}

			if (this.ProductDropDownList.SelectedItem == null || string.IsNullOrEmpty(this.ProductDropDownList.SelectedItem.Text))
			{
				this.ShowAlert("Product must be provided");
				this.SetFocusOnControl(this.ProductDropDownList);
				return false;
			}

			if (this.OperatorDropDownList.SelectedItem == null || string.IsNullOrEmpty(this.OperatorDropDownList.SelectedItem.Text))
			{
				this.ShowAlert("Operator must be provided");
				this.SetFocusOnControl(this.OperatorDropDownList);
				return false;
			}

			if (this.RegistrationIDDropDownList.SelectedItem == null || string.IsNullOrEmpty(this.RegistrationIDDropDownList.SelectedItem.Text))
			{
				this.ShowAlert("Registration ID must be provided");
				this.SetFocusOnControl(this.RegistrationIDDropDownList);
				return false;
			}

			if (string.IsNullOrEmpty(this.StartDateTimeControl.Text))
			{
				this.ShowAlert("Start Time must be provided");
				this.SetFocusOnControl(this.StartDateTimeControl);
				return false;
			}

			if (string.IsNullOrEmpty(this.StopDateTimeControl.Text))
			{
				this.ShowAlert("Stop Time must be provided");
				this.SetFocusOnControl(this.StopDateTimeControl);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Make sure the start time is before or equal to the stop time
		/// </summary>
		/// <returns>True if the start time is before or equal to the stop time</returns>
		private bool CheckDateTimes()
		{
			if (!string.IsNullOrEmpty(this.StartDateTimeControl.Text) && !string.IsNullOrEmpty(this.StopDateTimeControl.Text))
			{
				DateTimeOffset startTime = this.StartDateTimeControl.CurrentValue;
				DateTimeOffset stopTime = this.StopDateTimeControl.CurrentValue;

				if (stopTime < startTime)
				{
					this.ShowAlert("Stop Time must be equal to or later than Start Time");
					this.SetFocusOnControl(this.StopDateTimeControl);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Make sure the volumes are provided and greater than zero
		/// </summary>
		/// <returns>True if the volumes are provided and valid numbers greater than zero. False otherwise</returns>
		private bool ValidateVolumesAreGreaterThanZero()
		{
			double grossVolume;

			if (string.IsNullOrEmpty(this.GrossVolumeTextBox.Text))
			{
				this.ShowAlert("Gross volume must be provided");
				this.SetFocusOnControl(this.GrossVolumeTextBox);
				return false;
			}

			if (!double.TryParse(this.GrossVolumeTextBox.Text, out grossVolume))
			{
				this.ShowAlert("Gross volume must be a valid number");
				this.SetFocusOnControl(this.GrossVolumeTextBox);
				return false;
			}

			if (grossVolume <= 0)
			{
				this.ShowAlert("Gross volume must be greater than zero");
				this.SetFocusOnControl(this.GrossVolumeTextBox);
				return false;
			}

			double netVolume;

			if (string.IsNullOrEmpty(this.NetVolumeTextBox.Text))
			{
				this.ShowAlert("Net volume must be provided");
				this.SetFocusOnControl(this.NetVolumeTextBox);
				return false;
			}

			if (!double.TryParse(this.NetVolumeTextBox.Text, out netVolume))
			{
				this.ShowAlert("Net volume must be a valid number");
				this.SetFocusOnControl(this.NetVolumeTextBox);
				return false;
			}

			if (netVolume <= 0)
			{
				this.ShowAlert("Net volume must be greater than zero");
				this.SetFocusOnControl(this.NetVolumeTextBox);
				return false;
			}

			return true;
		}
		#endregion
	}
}