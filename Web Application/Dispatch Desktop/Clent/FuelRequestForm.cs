namespace Dispatch
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

    using System.Reflection;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

    using FMCore;
	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class FuelRequestForm : FMBaseForm
	{
		public enum REQUEST_TYPE
		{
			/// <summary>
			/// The initial setting, which we should never get when the page is in use.
			/// </summary>
			Unknown = 0,

			/// <summary>
			/// A regular fuel request, typically entered in real time with dispatching personnel. Either a Refuel or Defuel.
			/// </summary>
			RequestFuel = 1,

			/// <summary>
			/// A fuel request for an aircraft not stationed at your site. Either a Refuel or Defuel.
			/// </summary>
			Transient = 2,

			/// <summary>
			/// A fuel request where all information is already known, typically entered after the request is complete. Either a Refuel or Defuel.
			/// </summary>
			FastLog = 3,

			/// <summary>
			/// A Fill, a Partial Fill, or a Return to Bulk
			/// </summary>
			FillStand = 4,

			/// <summary>
			/// A Fill, a Partial Fill, or a Return to Bulk
			/// </summary>
			FastLogFillStand = 5
		};

		public TransactionDO transaction;
		public TransactionDO lastTransaction;
		public string EquipmentRefID = string.Empty;
		public bool TransactionWillBeSetToCompleted = false;

		private DateTime? startingRequestDate;
		private REQUEST_TYPE requestType;
		private TransactionAliasClass transactionAlias;

		private readonly SecurityClass security;
		private readonly SiteClass site;
		private bool handleActivityComboBoxEvents;
		private bool handleAircraftIDComboBoxEvents;
		private bool handleGradeComboBoxEvents;
		private bool handleRegistrationIDComboBoxEvents;
		private readonly string fuelRequestTransactionAlias;
		private readonly string defuelRequestTransactionAlias;
		private readonly string fillStandTransactionAlias;
		private readonly string returnToBulkTransactionAlias;
		private string lastTransactionMemo = string.Empty; // this is used to ensure the user enters a comment after a third consequtive > 2%
		private int iLoopCounter;
		private bool transactionInitialyCompleted;
		private ProductCollectionClass productCollectionCache;
		private EquipmentCollectionClass equipmentSecondaryStorageCollectionCache;
		private EquipmentCollectionClass equipmentActivityCollectionCache;
		private bool invalidRegistrationID;
		private TransactionProcessingListClass transToProcess;

		EquipmentCollectionClass equipmentCollection = new EquipmentCollectionClass();

		/// <summary>
		/// This is not suitable to determining when the dialog is completing all transaction types.  It 
		/// helps with some specific situations but not all.
		/// </summary>
		public bool CompletionMode
		{
			get;
			set;
		}

		private const string DeviationCommentRequiredMessage = "Deviation is >= 2% for three consecutive actions.\r\n Comment field is required";
		private bool isNewRequestForm;
		private readonly DateTime operationLockDate;

		public class TransactionProcessingListClass
		{
			protected string[] TransIDList;
			protected string[] TransXRefList;
			protected int CurrentTransIndex;

			public string FormDialogHeader
			{
				get;
				protected set;
			}

			public TransactionProcessingListClass(string[] transIDList, string[] transXRefList, string formDialogText)
			{
				this.TransIDList = transIDList;
				this.TransXRefList = transXRefList;
				this.CurrentTransIndex = 0;
				this.FormDialogHeader = formDialogText;
			}

			public bool HasAnotherTransaction
			{
				get
				{
					return this.CurrentTransIndex < this.TransIDList.Length - 1;
				}
			}

			public string GetNextTransactionID()
			{
				string toRet = null;

				if (this.HasAnotherTransaction)
				{
					toRet = this.TransIDList[++this.CurrentTransIndex];
				}

				return toRet;
			}

			public string CurrentTransID
			{
				get
				{
					return this.TransIDList[this.CurrentTransIndex];
				}
			}

			public string CurrentTransXRefID
			{
				get
				{
					return this.TransXRefList[this.CurrentTransIndex];
				}
			}
		}

		public TransactionProcessingListClass TransToProcess
		{
			set
			{
				this.transToProcess = value;
			}
		}

		public string TransID
		{
			set
			{
				var transactionSR = new TransactionSR { Security = this.security, TransID = value, ConvertUnits = true };

				this.transaction = FMChannelHelper.MakeCall<IClientDispatchService, TransactionDO>(x => x.ProcessTransactionTransactionServiceRequest(transactionSR));

				if (this.transaction.Number02 == null)
				{
					if (this.transactionAlias.ID == this.returnToBulkTransactionAlias 
						|| this.transactionAlias.ID == this.fillStandTransactionAlias)
					{
						this.transaction.Number02 = Convert.ToDouble(REQUEST_TYPE.FastLogFillStand);
					}
					else
					{
						this.transaction.Number02 = Convert.ToDouble(REQUEST_TYPE.FastLog);
					}
				}

				this.RequestType = (REQUEST_TYPE) Convert.ToInt32(this.transaction.Number02.Value);

				if (this.transaction.Status == TransactionStatus.Completed)
				{
					this.CompletionMode = true;
					this.transactionInitialyCompleted = true;
				}
				else
				{
					this.transactionInitialyCompleted = false;
				}

				this.transIDTextBox.Text = value;
			}
		}

		public string DialogHeaderText
		{
			set
			{
				this.Text = value;
			}
		}

		public REQUEST_TYPE RequestType
		{
			set
			{
				this.requestType = value;

				if (this.requestType == REQUEST_TYPE.FastLogFillStand || this.requestType == REQUEST_TYPE.FillStand)
				{
					this.fuelingServiceRequestTabControl.TabPages.Remove(this.fuelRequestTabPage);
					this.fuelingServiceRequestTabControl.TabPages.Remove(this.contactTabPage);
				}
				else
				{
					this.fuelingServiceRequestTabControl.TabPages.Remove(this.fillStandTabPage);
				}

				switch (this.requestType)
				{
					case REQUEST_TYPE.FastLog:
						this.Text = "Fast Log Fuel Request";
						this.CompletionMode = true;
						break;
					case REQUEST_TYPE.RequestFuel:
						this.Text = "Fuel Request";
						break;
					case REQUEST_TYPE.Transient:
						this.Text = "Transient Fuel Request";
						break;
					case REQUEST_TYPE.FillStand:
						this.Text = "Fill Stand Request";
						break;
					case REQUEST_TYPE.FastLogFillStand:
						this.Text = "Fast Log Fill Stand Request";
						this.CompletionMode = true;
						break;
					default:
						throw new Exception("Unknown Request Type");
				}

				this.GetTransactionAlias();
			}
		}


		public FuelRequestForm(DateTime lockDate)
		{
			this.InitializeComponent();

			this.GetSecurity();
			this.security = this.Security;

			this.CompletionMode = false;

			this.operationLockDate = lockDate;

			this.site = FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(this.security, this.security.SiteGuid));

			this.fuelRequestTransactionAlias = ConfigurationManager.AppSettings["FuelRequestTransactionAlias"];

			if (string.IsNullOrEmpty(this.fuelRequestTransactionAlias))
			{
				throw new Exception("FuelRequestTransactionAlias not in AppSettings");
			}

			this.defuelRequestTransactionAlias = ConfigurationManager.AppSettings["DefuelRequestTransactionAlias"];

			if (string.IsNullOrEmpty(this.defuelRequestTransactionAlias))
			{
				throw new Exception("DefuelRequestTransactionAlias not in AppSettings");
			}

			this.fillStandTransactionAlias = ConfigurationManager.AppSettings["FillStandTransactionAlias"];

			if (string.IsNullOrEmpty(this.fillStandTransactionAlias))
			{
				throw new Exception("FillStandTransactionAlias not in AppSettings");
			}

			this.returnToBulkTransactionAlias = ConfigurationManager.AppSettings["ReturnToBulkTransactionAlias"];

			if (string.IsNullOrEmpty(this.defuelRequestTransactionAlias))
			{
				throw new Exception("DefuelRequestTransactionAlias not in AppSettings");
			}

			this.fillStandLocationComboBox.TextChanged += this.FillStandLocationComboBoxTextChanged;
		}

		private void PopulateActivityComboBox()
		{
			if (this.activityComboBox.Items.Count == 0) // For reloading dont want to hit the db again
			{
				var fuelCardCollection =
					FMChannelHelper.MakeCall<IClientDispatchService, FuelCardCollectionClass>(x => x.EnumerateFuelCards(this.security));

				if (this.transactionAlias.PermitNonReferenceData
				    && ConfigurationManager.AppSettings["PermitNonReferenceFuelCardData"] == "true")
				{
					this.activityComboBox.DropDownStyle = ComboBoxStyle.DropDown;
				}
				else
				{
					this.activityComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				}

				this.activityComboBox.DataSource = fuelCardCollection;
				this.activityComboBox.SelectedIndex = -1;
			}
		}

		/// <summary>
		/// Populates the aircraft ID combo box.
		/// </summary>
		private void PopulateAircraftIDComboBox()
		{
			var selectedItem = this.aircraftIDComboBox.SelectedItem as EquipmentClass;

			Guid? fuelCardGuid = null;

			if (this.activityComboBox.SelectedValue != null && (Guid) this.activityComboBox.SelectedValue != Guid.Empty)
			{
				fuelCardGuid = (Guid) this.activityComboBox.SelectedValue;
			}

			EQUIPMENT_TYPE[] types = { EQUIPMENT_TYPE.AIRCRAFT_TYPE };

			if (this.requestType != REQUEST_TYPE.Transient)
			{
				// Used for speeding up the form
				if (this.equipmentActivityCollectionCache == null)
				{
					DataSet set =
						FMChannelHelper.MakeCall<IClientDispatchService, DataSet>(
							x => x.EnumerateEquipmentByTypesCompanyFuelCardProductAndSecondaryStorage1(this.security, types, null));

					this.equipmentActivityCollectionCache = new EquipmentCollectionClass();
					this.LoadEquipment(set, this.equipmentActivityCollectionCache);
				}

				if (this.fuelRequestGradeComboBox.SelectedValue is Guid
					&& (Guid) this.fuelRequestGradeComboBox.SelectedValue != Guid.Empty)
				{
					Guid productGuid = ((ProductClass) this.fuelRequestGradeComboBox.SelectedItem).MasterRecordGuid;

					// null or specific product
					var query =
						this.equipmentActivityCollectionCache.Where(dr => (dr.ProductGuid == Guid.Empty || dr.ProductGuid == productGuid));

					if (fuelCardGuid == Guid.Empty)
					{
						query = query.Where(dr => dr.FuelCardGuid == Guid.Empty); // null fuelcard
					}
					else if (fuelCardGuid == null)
					{
						query = query.Where(dr => dr.FuelCardGuid != Guid.Empty); // not nulls
					}
					else
					{
						query = query.Where(dr => dr.FuelCardGuid == fuelCardGuid); // specific one
					}

					this.equipmentCollection = new EquipmentCollectionClass();

					foreach (EquipmentClass eq in query)
					{
						this.equipmentCollection.Add(eq);
					}
				}
				else
				{
					var query = this.equipmentActivityCollectionCache.AsQueryable();

					if (fuelCardGuid == Guid.Empty)
					{
						query = query.Where(dr => dr.FuelCardGuid == Guid.Empty); // nulls
					}
					else if (fuelCardGuid == null)
					{
						query = query.Where(dr => dr.FuelCardGuid != Guid.Empty); // not nulls
					}
					else
					{
						query = query.Where(dr => dr.FuelCardGuid == fuelCardGuid); // specific one
					}

					this.equipmentCollection = new EquipmentCollectionClass();

					foreach (EquipmentClass eq in query)
					{
						this.equipmentCollection.Add(eq);
					}
				}
			}
			else
			{
				this.fuelRequestRefCodeComboBox.Enabled = false;
			}

			if (this.transactionAlias.PermitNonReferenceData
				&& ConfigurationManager.AppSettings["PermitNonReferenceDestinationEquipmentData"] == "true")
			{
				this.fuelRequestRefCodeComboBox.DropDownStyle = ComboBoxStyle.DropDown;
				this.aircraftIDComboBox.DropDownStyle = ComboBoxStyle.DropDown;
			}
			else
			{
				this.fuelRequestRefCodeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				this.aircraftIDComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			}

			if (selectedItem != null)
			{
				selectedItem = this.equipmentCollection.Find(x => x.IdentityGuid == selectedItem.IdentityGuid);
			}
			else if (this.transaction != null)
			{
				EquipmentDO equipmentDO = (this.transaction.Alias == this.fuelRequestTransactionAlias) ? this.transaction.DestinationEQ1 : this.transaction.SourceEQ1;

				// if the user did not add it to the database no record will be returned just set the text
				if (equipmentDO.EquipmentGuid == Guid.Empty)
				{
					if (this.aircraftIDComboBox.Text.Length == 0)
						this.aircraftIDComboBox.Text = equipmentDO.RegistrationID;

					if (this.fuelRequestRefCodeComboBox.Text.Length == 0 && this.requestType != REQUEST_TYPE.Transient)
					{
						this.fuelRequestRefCodeComboBox.Text = this.transaction.UserData18;
					}
				}
				else
				{
					selectedItem = this.equipmentCollection.Find(x => x.MasterRecordGuid == equipmentDO.EquipmentGuid);
					
					if (selectedItem == null)
					{
						this.equipmentCollection.Add(
							FMChannelHelper.MakeCall<IClientDispatchService, EquipmentClass>(x => x.GetEquipment(this.security, equipmentDO.EquipmentGuid)));

						selectedItem = this.equipmentCollection.Find(x => x.MasterRecordGuid == equipmentDO.EquipmentGuid);
					}
				}
			}

			string fuelRequestEnteredText = this.fuelRequestRefCodeComboBox.Text;
			string aircraftEnteredText = this.aircraftIDComboBox.Text;

			// we need to reset the combo box parameters each time to preclude a malfunction.
			// If the datasource has no items in the collection the display member and value member will be reset
			this.fuelRequestRefCodeComboBox.DataSource = null;
			this.fuelRequestRefCodeComboBox.Items.Clear();
			this.fuelRequestRefCodeComboBox.DisplayMember = "XRef";
			this.fuelRequestRefCodeComboBox.ValueMember = "IdentityGuid";
			this.aircraftIDComboBox.DataSource = null;
			this.aircraftIDComboBox.Items.Clear();
			this.aircraftIDComboBox.DisplayMember = "ID";
			this.aircraftIDComboBox.ValueMember = "IdentityGuid";

			// Only add unique Xrefs to the combo
			var sortedCollection = from E in this.equipmentCollection
								   orderby E.Xref
								   select E;

			var filteredCollection = new EquipmentCollectionClass();

			foreach (EquipmentClass aircraft in sortedCollection)
			{
				if (this.FindXREF(filteredCollection, aircraft.Xref) == false)
				{
					filteredCollection.Add(aircraft);
				}
			}

			this.fuelRequestRefCodeComboBox.DataSource = filteredCollection;
			this.aircraftIDComboBox.DataSource = this.equipmentCollection;

			if (selectedItem != null)
			{
				this.fuelRequestRefCodeComboBox.Text = selectedItem.Xref;
				this.aircraftIDComboBox.SelectedItem = selectedItem;
			}
			else
			{
				this.fuelRequestRefCodeComboBox.SelectedIndex = -1;
				this.aircraftIDComboBox.SelectedIndex = -1;
			}

			// if after the reload there is no selection but the user had entered text restore it here
			if (this.fuelRequestRefCodeComboBox.SelectedIndex == -1 && fuelRequestEnteredText.Length > 0)
			{
				this.fuelRequestRefCodeComboBox.Text = fuelRequestEnteredText;
			}

			if (this.aircraftIDComboBox.SelectedIndex == -1 && aircraftEnteredText.Length > 0)
			{
				this.aircraftIDComboBox.Text = aircraftEnteredText;
			}
		}

		private bool FindXREF(EquipmentCollectionClass filteredCollection, string xRef)
		{
			foreach (EquipmentClass equipment in filteredCollection)
			{
				if (equipment.Xref.Equals(xRef))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Populates the grade combo box.
		/// </summary>
		private void PopulateGradeComboBox()
		{
			ComboBox comboBox = this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage) 
										? this.fillStandGradeComboBox : this.fuelRequestGradeComboBox;

			var selectedItem = comboBox.SelectedItem as ProductClass;

			if (this.productCollectionCache == null)
			{
				this.productCollectionCache = new ProductCollectionClass();

				var set = FMChannelHelper.MakeCall<IClientDispatchService, DataSet>(
					x => x.EnumerateProductsByType(this.security, ProductType.ComponentProduct));

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					var product = new ProductClass(this.site);
					product.Load(set);
					this.productCollectionCache.Add(product);
					table.Rows.RemoveAt(0);
				}
			}

			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

			if (selectedItem != null)
			{
				selectedItem = this.productCollectionCache.Find(x => x.IdentityGuid == selectedItem.IdentityGuid);
			}
			else if (this.transaction != null)
			{
				var searchGuid = this.transaction.LineItems[0].ProductGuid;
				selectedItem = this.productCollectionCache.Find(x => x.MasterRecordGuid == searchGuid);
			}

			comboBox.DataSource = this.productCollectionCache;

			if (selectedItem != null)
			{
				comboBox.SelectedItem = selectedItem;
			}
			else
			{
				comboBox.SelectedIndex = -1;
			}
		}

		public void SetRequestTypeComboBox()
		{
			if (this.transaction != null)
			{
				if ((this.transaction.Alias == this.fuelRequestTransactionAlias
				     || this.transaction.Alias == this.defuelRequestTransactionAlias)
				    && this.transaction.Status != TransactionStatus.Completed
				    && this.transaction.Status != TransactionStatus.Cancelled)
				{
					this.fuelRequestRequestTypeComboBox.Enabled = true;

				}
				else
				{
					this.fuelRequestRequestTypeComboBox.Enabled = false;
				}

				if (this.transaction.Alias == this.fuelRequestTransactionAlias)
				{
					this.fuelRequestRequestTypeComboBox.SelectedItem = "Refuel";
				}
				else if (this.transaction.Alias == this.defuelRequestTransactionAlias)
				{
					this.fuelRequestRequestTypeComboBox.SelectedItem = "Defuel";
				}
				else if (this.transaction.Alias == this.fillStandTransactionAlias)
				{
					this.fillStandRequestTypeComboBox.Items.Remove("Return To Bulk");
					this.fillStandRequestTypeComboBox.Items.Remove("Partial Return To Bulk");

					if (this.transaction.LineItems[0].PartialFill == null
						|| !this.transaction.LineItems[0].PartialFill.Value)
					{
						this.fillStandRequestTypeComboBox.SelectedItem = "Fill";
					}
					else
					{
						this.fillStandRequestTypeComboBox.SelectedItem = "Partial Fill";
					}
				}

				else if (this.transaction.Alias == this.returnToBulkTransactionAlias)
				{
					this.fillStandRequestTypeComboBox.Items.Remove("Fill");
					this.fillStandRequestTypeComboBox.Items.Remove("Partial Fill");

					if (this.transaction.LineItems[0].PartialFill == null
						|| !this.transaction.LineItems[0].PartialFill.Value)
					{
						this.fillStandRequestTypeComboBox.SelectedItem = "Return To Bulk";
					}
					else
					{
						this.fillStandRequestTypeComboBox.SelectedItem = "Partial Return To Bulk";
					}
					this.fillStandRequestTypeComboBox.Enabled = false;
				}
				else
				{
					throw new Exception("Unrecognized Transaction Alias - " + this.transaction.Alias);
				}
			}
			else
			{
				if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage))
				{
					this.fillStandRequestTypeComboBox.SelectedItem = "Fill";
				}
				else if (this.lastTransaction != null && this.lastTransaction.Alias == this.defuelRequestTransactionAlias)
				{
					this.fuelRequestRequestTypeComboBox.SelectedItem = "Defuel";
				}
				else
				{
					this.fuelRequestRequestTypeComboBox.SelectedItem = "Refuel";
				}
			}

			this.ConfigureAdditionalDataFields();
		}

		private void GetTransactionAlias()
		{
			if (this.transaction != null)
			{
				this.transactionAlias =
					FMChannelHelper.MakeCall<IClientDispatchService, TransactionAliasClass>(
						x => x.GetTransactionAliasFromAliasGuid(this.security, this.transaction.TransactionAliasGuid, true));
			}
			else
			{
				string aliasID = null;

				switch (this.requestType)
				{
					case REQUEST_TYPE.RequestFuel:
					case REQUEST_TYPE.Transient:
					case REQUEST_TYPE.FastLog:
						{
							if (this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Refuel")
							{
								aliasID = this.fuelRequestTransactionAlias;
							}
							else
							{
								aliasID = this.defuelRequestTransactionAlias;
							}
							break;
						}

					case REQUEST_TYPE.FillStand:
					case REQUEST_TYPE.FastLogFillStand:
						{
							string selectedItem = this.fillStandRequestTypeComboBox.SelectedItem as string;
							if (selectedItem == "Return To Bulk" || selectedItem == "Partial Return To Bulk")
							{
								aliasID = this.returnToBulkTransactionAlias;
							}
							else
							{
								aliasID = this.fillStandTransactionAlias;
							}
							break;
						}
				}

				if (string.IsNullOrEmpty(aliasID))
				{
					throw new Exception("Invalid Transaciton Alias");
				}

				this.transactionAlias = FMChannelHelper.MakeCall<IClientDispatchService, TransactionAliasClass>(
					x => x.GetTransactionAliasFromAliasId(this.security, aliasID, true));
			}

			if (this.transactionAlias.MultipleLineItems)
			{
				throw new Exception("Multiplie Line Item Alias is not supported - " + this.transactionAlias.ID);
			}
		}

		/// <summary>
		/// KeyPress Handler for EDIPI field of Defuel/Refuel transactions
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void OnEdipiTextboxKeyPress(Object sender, KeyPressEventArgs eventArgs)
		{
			char inputChar = eventArgs.KeyChar;

			if (!char.IsDigit(inputChar) && !char.IsControl(inputChar))
			{
				eventArgs.Handled = true;
			}
		}

		/// <summary>
		/// This method will be called from ConfigureAdditionalDataFields. 
		/// It disables the EDIPI field(line itme user data field 1) for Defule and Refuel transaction.
		/// </summary>
		private void ConfigureCustomAdditionalDataFields()
		{
			const int EdipiMaxLength = 10;
			TextBox edipiTextBox;

			if (this.FindEdipiTextBox(out edipiTextBox))
			{
				if ((this.transaction != null) &&
					(this.transaction.Status == TransactionStatus.Completed 
					|| this.transaction.Status == TransactionStatus.Posted 
					|| this.transaction.Status == TransactionStatus.Cancelled)
					)
				{

					edipiTextBox.Enabled = false;
				}
				edipiTextBox.MaxLength = EdipiMaxLength;

				// added KeyPress event to make it digit only
				edipiTextBox.KeyPress -= this.OnEdipiTextboxKeyPress; // in case we got call multiple times
				edipiTextBox.KeyPress += this.OnEdipiTextboxKeyPress;
			}
		}

		private void ConfigureAdditionalDataFields()
		{
			Label[] userDataLabel = this.UserDataLabels;
			TextBox[] userDataTextBox = this.UserDataTextBoxes;
			ComboBox[] userDataComboBox = this.UserDataComboBoxes;

			int index = 0;

			foreach (var fieldClass in this.transactionAlias.LineItemUserDataFieldCollection)
			{
				var lineItemUserDataField = (UserDataFieldClass)fieldClass;

				userDataLabel[index].Text = lineItemUserDataField.DisplayName;
				userDataLabel[index].Visible = true;
				userDataTextBox[index].Enabled = lineItemUserDataField.UserDataType == USER_DATA_TYPE.TEXT;
				userDataTextBox[index].Visible = lineItemUserDataField.UserDataType == USER_DATA_TYPE.TEXT;
				userDataComboBox[index].Enabled = lineItemUserDataField.UserDataType != USER_DATA_TYPE.TEXT;
				userDataComboBox[index].Visible = lineItemUserDataField.UserDataType != USER_DATA_TYPE.TEXT;

				if (lineItemUserDataField.UserDataType == USER_DATA_TYPE.LIST)
				{
					userDataComboBox[index].DataSource = lineItemUserDataField.UserDataListValueCollection;
				}

				index++;
			}

			this.NoAdditionalDataFieldsLabel.Visible = (index == 0);

			for (; index < 24; index++)
			{
				userDataLabel[index].Visible = false;
				userDataTextBox[index].Enabled = false;
				userDataTextBox[index].Visible = false;
				userDataComboBox[index].Enabled = false;
				userDataComboBox[index].Visible = false;
			}

			this.ConfigureCustomAdditionalDataFields();
		}

		/// <summary>
		/// This method populates the Registration ID into the combo box.
		/// </summary>
		private void PopulateRegistrationIDComboBox()
		{
			var selectedItem = this.detailRegistrationIDComboBox.SelectedItem as EquipmentClass;
			bool source = true;

			if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage))
			{
				if ((this.fillStandRequestTypeComboBox.SelectedItem as string != "Return To Bulk") &&
					(this.fillStandRequestTypeComboBox.SelectedItem as string != "Partial Return To Bulk"))
				{
					source = false;
				}
			}
			else if (this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Defuel")
			{
				source = false;
			}

			// Used for speeding up the form
			if (this.equipmentSecondaryStorageCollectionCache == null 
				|| this.equipmentSecondaryStorageCollectionCache.Count == 0)
			{
				var set = FMChannelHelper.MakeCall<IClientDispatchService, DataSet>(
							x => x.EnumerateEquipmentByTypesCompanyFuelCardProductAndSecondaryStorage1(this.security, null, true ));

				this.equipmentSecondaryStorageCollectionCache = new EquipmentCollectionClass();
				this.LoadEquipment(set, this.equipmentSecondaryStorageCollectionCache);
			}

			var equipmentCollectionClass = new EquipmentCollectionClass();

			if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fuelRequestTabPage)
				&& this.fuelRequestGradeComboBox.SelectedValue is int
				&& (int) this.fuelRequestGradeComboBox.SelectedValue != 0)
			{
				Guid? productGuid = null;

				if (this.fuelRequestGradeComboBox.SelectedIndex >= 0)
				{
					productGuid = new Guid(this.fuelRequestGradeComboBox.SelectedValue.ToString());
				}

				var query = this.equipmentSecondaryStorageCollectionCache.AsQueryable();

				if (productGuid == Guid.Empty)
				{
					//nulls
					query = query.Where(dr => dr.ProductGuid == Guid.Empty);
				}
				else if (productGuid == null)
				{
					//not nulls
					query = query.Where(dr => dr.ProductGuid != Guid.Empty);
				}
				else
				{
					//specific one
					query = query.Where(dr => dr.ProductGuid == productGuid);
				}

				foreach (EquipmentClass eq in query)
				{
					equipmentCollectionClass.Add(eq);
				}
			}
			else
			{
				foreach (EquipmentClass eq in this.equipmentSecondaryStorageCollectionCache)
				{
					equipmentCollectionClass.Add(eq); //add all back
				}
			}

			// Set the style of the drop down based on whether we need to allow manual entry
			if (this.transactionAlias.PermitNonReferenceData
			    && ConfigurationManager.AppSettings["PermitNonReferenceSourceEquipmentData"] == "true")
			{
				this.detailRegistrationIDComboBox.DropDownStyle = ComboBoxStyle.DropDown;
			}
			else
			{
				this.detailRegistrationIDComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			}

			// If there is a selected item, make sure it is in the list even if it does
			// not meet the criteria.  The user may have dispatched the equipment despite
			// warnings about the equipment not meeting the request parameters.
			if (selectedItem != null)
			{
				if (equipmentCollectionClass.Find(x => x.IdentityGuid == selectedItem.IdentityGuid) == null)
				{
					equipmentCollectionClass.Add(selectedItem);
				}
			}
			else if (this.transaction != null)
			{
				EquipmentDO equipmentDO = (source) ? this.transaction.SourceEQ1 : this.transaction.DestinationEQ1;

				if (equipmentDO != null)
				{
					selectedItem = equipmentCollectionClass.Find(x => x.MasterRecordGuid == equipmentDO.EquipmentGuid);

					// If we did not find the equipment in the list, we still want it to show up.  So, go
					// get the equipment and add it to the list
					if (selectedItem == null)
					{
						var equipment = FMChannelHelper.MakeCall<IClientDispatchService, EquipmentClass>(
													x => x.GetEquipment(this.Security, equipmentDO.EquipmentGuid));

						if (equipment.MasterRecordGuid == equipmentDO.EquipmentGuid)
						{
							equipmentCollectionClass.Add(equipment);
							selectedItem = equipment;
						}
					}
				}
			}

			// sort the collection
			var sortedCollection = from equipmentClass in equipmentCollectionClass
								   orderby equipmentClass.ID
								   select equipmentClass;

			var sortedEquipmentCollection = new EquipmentCollectionClass();

			foreach (var equipmentClass in sortedCollection)
			{
				sortedEquipmentCollection.Add(equipmentClass);
			}

			this.detailRegistrationIDComboBox.DisplayMember = "ID";
			this.detailRegistrationIDComboBox.DataSource = sortedEquipmentCollection;

			if (selectedItem != null)
			{
				this.detailRegistrationIDComboBox.SelectedItem = selectedItem;
				this.detailRegistrationIDComboBox.Text = selectedItem.ID;
			}
			else
			{
				this.detailRegistrationIDComboBox.SelectedIndex = -1;
			}

			if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage))
			{
				if (this.transactionAlias.PermitNonReferenceData
					&& ConfigurationManager.AppSettings["PermitNonReferenceSourceEquipmentData"] == "true")
				{
					this.fillStandRefCodeComboBox.DropDownStyle = ComboBoxStyle.DropDown;
					this.fillStandRegistrationIDComboBox.DropDownStyle = ComboBoxStyle.DropDown;
				}
				else
				{
					this.fillStandRefCodeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
					this.fillStandRegistrationIDComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				}

				// sort by ref code
				var sortedList = from equipmentClass in sortedEquipmentCollection
								 orderby equipmentClass.Xref
								 select equipmentClass;

				var sortedByXRef = new EquipmentCollectionClass();

				foreach (var equipmentClass in sortedList)
				{
					sortedByXRef.Add(equipmentClass);
				}

				this.fillStandRefCodeComboBox.DataSource = sortedByXRef;
				this.fillStandRegistrationIDComboBox.DataSource = sortedEquipmentCollection;

				if (selectedItem != null)
				{
					this.fillStandRefCodeComboBox.SelectedItem = selectedItem;
					this.fillStandRegistrationIDComboBox.SelectedItem = selectedItem;
					this.typeTextBox.Text = selectedItem.TypeClass;
				}
				else
				{
					this.fillStandRefCodeComboBox.SelectedIndex = -1;
					this.fillStandRegistrationIDComboBox.SelectedIndex = -1;
				}

				this.detailRegistrationIDComboBox.Enabled = false;
			}
		}


		private void PopulateUseCodeComboBox()
		{
			if (this.useCodeComboBox.Items.Count == 0) //for reloading dont want to hit the db again
			{
				var fuelCard = new FuelCardClass();

				var userCodeGuid =
					FMChannelHelper.MakeCall<IClientDispatchService, Guid>(
							x => x.GetUserDataFieldsIdentityGuid(this.security, fuelCard.EntityType, Guid.Empty, 1, false));

				if (userCodeGuid == Guid.Empty)
				{
					throw new Exception("Use Code User Data Field not found");
				}

				UserDataFieldClass userDataField =
					FMChannelHelper.MakeCall<IClientDispatchService, UserDataFieldClass>(
								x => x.GetUserDataField(this.security, userCodeGuid, fuelCard.EntityType));

				if (userDataField.DisplayName != "Use Code")
				{
					throw new Exception("Fuel Card User Data Field 2 not configured for Use Code");
				}

				this.useCodeComboBox.DataSource = userDataField.UserDataListValueCollection;
				this.useCodeComboBox.SelectedIndex = -1;
			}
		}

		private void PopulateSignalCodeComboBox()
		{
			if (this.signalCodeComboBox.Items.Count == 0) //for reloading dont want to hit the db again
			{
				var fuelCard = new FuelCardClass();

				FMChannelHelper.MakeCall<IClientDispatchService>(
					userDataFields =>
					{
						var signalCodeGuid = userDataFields.GetUserDataFieldsIdentityGuid(this.security, fuelCard.EntityType, Guid.Empty, 0, false);

						if (signalCodeGuid == Guid.Empty)
						{
							throw new Exception("Sig. Code User Data Field not found");
						}

						UserDataFieldClass userDataField = userDataFields.GetUserDataField(this.security, signalCodeGuid, fuelCard.EntityType);

						if (userDataField.DisplayName != "Sig. Code")
						{
							throw new Exception("Fuel Card User Data Field 1 not configured for Sig. Code");
						}

						this.signalCodeComboBox.DataSource = userDataField.UserDataListValueCollection;
						this.signalCodeComboBox.SelectedIndex = -1;
					});
			}
		}

		private void PopulateOperatorComboBox()
		{
			if (this.operatorComboBox.Items.Count == 0) //for reloading dont want to hit the db again
			{
				var personCollection = new PersonCollectionClass();

				var set = FMChannelHelper.MakeCall<IClientDispatchService, DataSet>(x => x.EnumeratePersonByRole(this.security, PERSON_ROLE.LOADER_ROLE));
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					var person = new PersonClass(this.site);
					person.Load(set);
					personCollection.Add(person);
					table.Rows.RemoveAt(0);
				}

				if (this.transactionAlias.PermitNonReferenceData
				    && ConfigurationManager.AppSettings["PermitNonReferencePersonnelData"] == "true")
				{
					this.operatorComboBox.DropDownStyle = ComboBoxStyle.DropDown;
				}
				else
				{
					this.operatorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
				}

				this.operatorComboBox.DataSource = personCollection;
				this.operatorComboBox.SelectedIndex = -1;
			}
		}

		private void LoadFillStandForm()
		{
			this.differentialPressureAndVarianceLabel.Text = "Variance";
			this.differentialPressureAndVarianceTextBox.Enabled = false;

			this.PopulateGradeComboBox();
			this.PopulateRegistrationIDComboBox();
			this.PopulateOperatorComboBox();

			if (this.transaction != null)
			{
				if (this.transaction.Status == TransactionStatus.Cancelled)
				{
					this.fillStandRequestCancelledCheckBox.Checked = true;
				}

				if (this.transaction.Status == TransactionStatus.Completed
					|| this.transaction.Status == TransactionStatus.Posted)
				{
					this.fillStandRequestCancelledCheckBox.Enabled = false;
					this.fillStandRequestTypeComboBox.Enabled = false;
				}

				this.fillStandRequestedByTextBox.Text = this.transaction.ContactSurname;
				this.fillStandLocationTextBox.Text = this.transaction.UserData7;
				this.radioNumberTextBox.Text = this.transaction.UserData8;

				this.fillStandCommentTextBox.Text = this.transaction.Notes;

				if (this.transaction.LineItems != null && this.transaction.LineItems.Count > 0)
				{
					LineItemDO lineItem = this.transaction.LineItems[0];

					if (lineItem.Quantity != null)
					{
						this.quantityTextBox.Text = lineItem.Quantity.Gross.ToString(this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
						this.quantityTextBox2.Text = lineItem.Quantity.Gross.ToString(this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
					}

					TextBox[] userDataTextBox = this.UserDataTextBoxes;
					ComboBox[] userDataComboBox = this.UserDataComboBoxes;

					int index = 0;
					foreach (var fieldClass in this.transactionAlias.LineItemUserDataFieldCollection)
					{
						var lineItemUserDataField = (UserDataFieldClass)fieldClass;
						var key = lineItemUserDataField.EntityType + " User Data " + (lineItemUserDataField.Number + 1);

						if (this.transaction.LineItems[0].UserData.ContainsKey(key))
						{

							if (lineItemUserDataField.UserDataType == USER_DATA_TYPE.LIST)
							{
								userDataComboBox[index].SelectedText = this.transaction.LineItems[0].UserData[key];

							}
							else
							{
								int userDataCount = this.transaction.LineItems[0].UserData.Count;

								if ((lineItemUserDataField.Number + 1) <= userDataCount)
								{
									userDataTextBox[index].Text = this.transaction.LineItems[0].UserData[key];
								}
							}

						}
						index++;
					}
				}

				if (this.transaction.OperatorPersonnelGuid != Guid.Empty)
				{
					PersonClass person =
						FMChannelHelper.MakeCall<IClientDispatchService, PersonClass>(x => x.GetPerson(this.security, this.transaction.OperatorPersonnelGuid));

					this.operatorComboBox.SelectedIndex = this.operatorComboBox.FindString(person.FullName);
				}

				if (this.transaction.Number01 != null)
				{
					this.differentialPressureAndVarianceTextBox.Text = this.transaction.Number01.Value.ToString(CultureInfo.InvariantCulture);
				}
			}

			this.handleGradeComboBoxEvents = true;
			this.handleRegistrationIDComboBoxEvents = true;

			// if this is a service or fillstand completetion set the tab to the detail
			if (this.Text.Contains("Fillstand Completion"))
			{
				this.quantityLabel.Visible = false;
				this.quantityTextBox.Visible = false;
				this.quantityTextBox.Enabled = false;

				this.quantityLabel2.Visible = true;
				this.quantityTextBox2.Visible = true;
				this.quantityTextBox2.Enabled = true;

			}

			if (this.Text.Contains("Fillstand Completion")
				|| (this.requestType == REQUEST_TYPE.FastLogFillStand && (this.transaction == null || this.transaction.Status != TransactionStatus.Completed)))
			{
				this.fillStandLocationTextBox.Visible = false;
				this.fillStandLocationComboBox.Visible = true;
				this.typeTextBox.Enabled = false;
				this.PopulateFillStandLocationComboBox();
			}
			else if (this.Text.Contains("Fast Log Fill Stand"))
			{
				this.typeTextBox.Enabled = false;
			}
		}

		private void PopulateFillStandLocationComboBox()
		{
			this.equipmentCollection =
				FMChannelHelper.MakeCall<IClientDispatchService, EquipmentCollectionClass>(x => x.EnumerateByManagedFillstand(this.security));

			this.fillStandLocationComboBox.DisplayMember = "ID";
			this.fillStandLocationComboBox.DataSource = this.equipmentCollection;
			this.fillStandLocationComboBox.Text = this.fuelRequestLocationTextBox.Text;
		}

		private void LoadFuelRequestForm()
		{
			this.fuelRequestRequestTypeComboBox.SelectedIndexChanged	-= this.RequestTypeComboBoxSelectedIndexChanged;
			this.fuelRequestGradeComboBox.SelectedIndexChanged			-= this.FuelRequestGradeComboBoxSelectedIndexChanged;
			this.fuelRequestRefCodeComboBox.SelectedIndexChanged		-= this.FuelRequestRefCodeComboBoxSelectedIndexChanged;
			this.aircraftIDComboBox.SelectedIndexChanged				-= this.AircraftIDComboBoxSelectedIndexChanged;
			this.aircraftIDComboBox.SelectedIndexChanged				-= this.AircraftIDComboBoxSelectedIndexChanged;
			this.activityComboBox.SelectedIndexChanged					-= this.ActivityComboBoxSelectedIndexChanged;

			this.differentialPressureAndVarianceLabel.Text = "Differential Pressure";

			if (this.equipmentCollection == null)
			{
				this.equipmentCollection = new EquipmentCollectionClass();
			}

			this.PopulateUseCodeComboBox();
			this.PopulateSignalCodeComboBox();
			this.PopulateActivityComboBox();
			this.PopulateGradeComboBox();
			this.PopulateAircraftIDComboBox();

			this.PopulateRegistrationIDComboBox();
			this.PopulateOperatorComboBox();

			if (this.transaction != null)
			{
				if (this.transaction.Status == TransactionStatus.Cancelled)
				{
					this.fuelRequestRequestCancelledCheckBox.Checked = true;
				}

				if (this.transaction.Status == TransactionStatus.Completed || this.transaction.Status == TransactionStatus.Posted)
				{
					this.fuelRequestRequestCancelledCheckBox.Enabled = false;
				}

				this.dodaccTextBox.Text = this.transaction.ShipToID;

				if (this.transaction.ShipToID != this.transaction.BillToID)
				{
					this.suppDODACCTextBox.Text = this.transaction.BillToID;
				}

				this.fuelRequestRequestedByTextBox.Text = this.transaction.ContactSurname;
				this.contactTextBox.Text				= this.transaction.ContactFirstName;
				this.phoneTextBox.Text					= this.transaction.ContactInfo;
				this.cardNumberTextBox.Text				= this.transaction.PaymentInfo.CreditCardNumber;
				this.fundCodeTextBox.Text				= this.transaction.UserData5;

				this.rptTecTextBox.Text = this.transaction.UserData3;

				if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fuelRequestTabPage))
				{
					this.fuelRequestLocationTextBox.Text = this.transaction.UserData7;
				}
				else
				{
					this.fillStandLocationTextBox.Text = this.transaction.UserData7;
				}

				this.radioNumberTextBox.Text						= this.transaction.UserData8;
				this.differentialPressureAndVarianceTextBox.Text	= this.transaction.UserData10;
				this.address1TextBox.Text							= this.transaction.UserData11;
				this.emailTextBox.Text								= this.transaction.UserData12;
				this.cityTextBox.Text								= this.transaction.UserData15;
				this.stateTextBox.Text								= this.transaction.UserData17;
				this.bosComboBox.Text								= this.transaction.UserData19;
				this.signalCodeComboBox.Text						= this.transaction.UserData20;
				this.useCodeComboBox.Text							= this.transaction.UserData21;
				this.zipTextBox.Text								= this.transaction.UserData22;
				this.memoTextBox.Text								= this.transaction.UserData23;
				this.faxTextBox.Text								= this.transaction.UserData24;

				this.fuelRequestCommentTextBox.Text = this.transaction.Notes;

				if (this.transaction.FuelCardGuid != Guid.Empty)
				{
					// While we are loading the transaction we don't want to get the selected index changed event
					// as it will clobber values we just loaded in controls from values saved in the transaction.
					bool prevHandleActivity = this.handleActivityComboBoxEvents;
					this.handleActivityComboBoxEvents = false;

					// Update the index
					this.activityComboBox.SelectedValue = this.transaction.FuelCardGuid;

					// Rehandle the activity event if needed
					this.handleActivityComboBoxEvents = prevHandleActivity;
				}

				if (this.transaction.LineItems != null && this.transaction.LineItems.Count > 0)
				{
					LineItemDO lineItem = this.transaction.LineItems[0];

					if (lineItem.Quantity != null)
					{
						this.quantityTextBox.Text = lineItem.Quantity.Gross.ToString(this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
						this.quantityTextBox2.Text = this.quantityTextBox.Text;
					}

					TextBox[] userDataTextBox = this.UserDataTextBoxes;
					ComboBox[] userDataComboBox = this.UserDataComboBoxes;

					int index = 0;

					foreach (var fieldClass in this.transactionAlias.LineItemUserDataFieldCollection)
					{
						var lineItemUserDataField = (UserDataFieldClass)fieldClass;
						var key = "TALUD" + (lineItemUserDataField.Number + 1);

						if (this.transaction.LineItems[0].UserData.ContainsKey(key))
						{
							if (lineItemUserDataField.UserDataType == USER_DATA_TYPE.LIST)
							{
								userDataComboBox[index].SelectedText = this.transaction.LineItems[0].UserData[key];
							}
							else
							{
								int userDataCount = this.transaction.LineItems[0].UserData.Count;

								if ((lineItemUserDataField.Number + 1) <= userDataCount)
								{
									userDataTextBox[index].Text = this.transaction.LineItems[0].UserData[key];
								}
							}
						}

						index++;
					}
				}

				EquipmentDO equipmentDO = (this.transaction.Alias == this.fuelRequestTransactionAlias) 
												? this.transaction.DestinationEQ1 : this.transaction.SourceEQ1;

				if (equipmentDO != null && equipmentDO.EquipmentGuid != Guid.Empty)
				{
					this.mdsTextBox.Text = equipmentDO.EquipmentModel;
					this.aircraftIDComboBox.SelectedValue = equipmentDO.EquipmentGuid;

					if (this.requestType == REQUEST_TYPE.Transient)
					{
						var equipment =
							FMChannelHelper.MakeCall<IClientDispatchService, EquipmentClass>(x => x.GetEquipment(this.Security, equipmentDO.EquipmentGuid));

						this.aircraftIDComboBox.Text = equipment.ID;
						this.fuelRequestRefCodeComboBox.Text = equipment.Xref;
					}
				}
				else
				{
					// if there is an equipment model in the transaction that is not in our database populate the mds field with this data
					if (this.transaction.SourceEQ1 != null &&
						 !string.IsNullOrEmpty(this.transaction.SourceEQ1.EquipmentModel) &&
						 (this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Defuel" ||
						 this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Return To Bulk"))
					{
						this.mdsTextBox.Text = this.transaction.SourceEQ1.EquipmentModel;
					}
					else if (this.transaction.DestinationEQ1 != null &&
						 !string.IsNullOrEmpty(this.transaction.DestinationEQ1.EquipmentModel) &&
						 this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Refuel")
					{
						this.mdsTextBox.Text = this.transaction.DestinationEQ1.EquipmentModel;
					}
				}

				if (this.transaction.OperatorPersonnelGuid != Guid.Empty)
				{
					PersonClass person =
						FMChannelHelper.MakeCall<IClientDispatchService, PersonClass>(x => x.GetPerson(this.security, this.transaction.OperatorPersonnelGuid));

					this.operatorComboBox.SelectedIndex = this.operatorComboBox.FindString(person.FullName);
				}

				this.fuelAdditiveCheckBox.Checked = this.transaction.Flag04;

				// if this is a service or fillstand completetion set the tab to the detail
				if (this.Text.Contains("Fillstand Completion"))
				{
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.quantityLabel.Visible = false;
					this.quantityTextBox.Visible = false;
					this.quantityTextBox.Enabled = false;

					this.quantityLabel2.Visible = true;
					this.quantityTextBox2.Visible = true;
					this.quantityTextBox2.Enabled = this.CompletionMode;
				}
				else if (this.Text.Contains("Service Completion"))
				{
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
				}

				// The transaction has been released to accounting and cannot be modified.
				if (this.transaction.SubmittedToAccounting == true)
				{
					this.okButton.Enabled = false;
					this.applyButton.Enabled = false;
				}
			}

			this.fuelRequestRequestTypeComboBox.SelectedIndexChanged	+= this.RequestTypeComboBoxSelectedIndexChanged;
			this.fuelRequestGradeComboBox.SelectedIndexChanged			+= this.FuelRequestGradeComboBoxSelectedIndexChanged;
			this.fuelRequestRefCodeComboBox.SelectedIndexChanged		+= this.FuelRequestRefCodeComboBoxSelectedIndexChanged;
			this.aircraftIDComboBox.SelectedIndexChanged				+= this.AircraftIDComboBoxSelectedIndexChanged;
			this.activityComboBox.SelectedIndexChanged					+= this.ActivityComboBoxSelectedIndexChanged;

			this.handleActivityComboBoxEvents		= true;
			this.handleAircraftIDComboBoxEvents		= true;
			this.handleGradeComboBoxEvents			= true;
			this.handleRegistrationIDComboBoxEvents = true;
		}


		private void FuelRequestFormLoad(object sender, EventArgs e)
		{
			try
			{
				bool resetTime = true;

				if (this.transToProcess != null)
				{
					this.TransID = this.transToProcess.CurrentTransID;

					// This must come after setting TransID otherwise it gets overwritten
					this.DialogHeaderText = this.transToProcess.FormDialogHeader + this.transToProcess.CurrentTransXRefID; 
				}

				this.isNewRequestForm = (this.transaction == null);

				if (this.isNewRequestForm)
				{
					this.AcceptButton = this.applyButton;
				}
				else
				{
					this.AcceptButton = this.okButton;
				}

				this.okButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
				this.applyButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

				this.quantityTextBox.Enabled = this.CompletionMode;
				this.quantityTextBox2.Enabled = this.CompletionMode;

				if (sender == null)
				{
					// Reset the time pickers if the form is actually be loaded for the first time 
					// (Sender will be null if we are refreshing the form)
					resetTime = false;
				}

				this.ResetTimePickers(resetTime);

				if (!this.isNewRequestForm)
				{
					this.ignoreDispatchTimeCheckBox.Enabled = false;
					this.ignoreArrivalTimeCheckBox.Enabled = false;
					this.ignoreStartTimeCheckBox.Enabled = false;
					this.ignoreStopTimeCheckBox.Enabled = false;
				}

				this.SetRequestTypeComboBox();

				if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage))
				{
					this.LoadFillStandForm();
					this.fillStandRefCodeComboBox.Select();
				}
				else
				{
					this.LoadFuelRequestForm();
					this.fuelRequestRefCodeComboBox.Select();
				}

				if (this.transaction != null)
				{
					if (this.transaction.DispatchedDateTime != null)
					{
						this.ignoreDispatchTimeCheckBox.Checked = false;
						this.dispatchDateTimePicker.Value = this.transaction.DispatchedDateTime.Value.DateTime;
						this.DispatchdatePicker.Value = this.transaction.DispatchedDateTime.Value.DateTime;
					}
					else
					{
						this.ignoreDispatchTimeCheckBox.Checked = true;
					}

					if (this.transaction.TimeIn != null)
					{
						this.ignoreArrivalTimeCheckBox.Checked = false;
						this.arrivalDateTimePicker.Value = this.transaction.TimeIn.Value.DateTime;
						this.ArrivaldatePicker.Value = this.transaction.TimeIn.Value.DateTime;
					}
					else
					{
						this.ignoreArrivalTimeCheckBox.Checked = true;
					}

					if (this.transaction.RouteSchedule.FST != null)
					{
						this.ignoreStartTimeCheckBox.Checked = false;
						this.startDateTimePicker.Value = this.transaction.RouteSchedule.FST.Value.DateTime;
						this.StartdatePicker.Value = this.transaction.RouteSchedule.FST.Value.DateTime;
					}
					else
					{
						this.ignoreStartTimeCheckBox.Checked = true;
					}

					if (this.transaction.TimeEnd != null)
					{
						this.ignoreStopTimeCheckBox.Checked = false;
						this.stopDateTimePicker.Value = this.transaction.TimeEnd.Value.DateTime;
						this.StopdatePicker.Value = this.transaction.TimeEnd.Value.DateTime;
					}
					else
					{
						this.ignoreStopTimeCheckBox.Checked = true;
					}

					if (this.transaction.TimeOut != null)
					{
						this.completionDateTimePicker.Value = this.transaction.TimeOut.Value.DateTime;
						this.CompletiondatePicker.Value = this.transaction.TimeOut.Value.DateTime;
					}

					if (this.transaction.RequestedDateTime != null)
					{
						this.requestDateDateTimePicker.Value = this.transaction.RequestedDateTime.Value.DateTime;
						this.requestDateTimePicker.Value = this.transaction.RequestedDateTime.Value.DateTime;
					}

					// populate the serial number, iss pt, isspt num and gross gal fields on the detail tab
					this.serialnumbertextBox.Text = this.transaction.UserData4;
					this.isspttextBox.Text = this.transaction.IssuePoint;
					this.issptnumtextBox.Text = this.transaction.IssuePointNumber;

					var number03 = this.transaction.Number03;

					if (number03 != null)
					{
						this.grossgaltextBox.Text = number03.Value.ToString(CultureInfo.InvariantCulture);
					}

					this.fuelRequestLocationTextBox.Enabled = true;
					this.fuelRequestRequestedByTextBox.Enabled = true;
					this.fuelRequestCommentTextBox.Enabled = true;
				}
				else
				{
					if (resetTime)
					{
						this.SetTimeCheckboxToConfig();
					}
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
			}

			this.SetControlEnabledDisabled();
		}


		private void SetTimeCheckboxToConfig()
		{
			this.ignoreDispatchTimeCheckBox.Checked = false;
			this.ignoreArrivalTimeCheckBox.Checked = false;
			this.ignoreStartTimeCheckBox.Checked = false;
			this.ignoreStopTimeCheckBox.Checked = false;

			string useArrivalTime = ConfigurationManager.AppSettings["Use Arrival Time"];
			string useStartTime = ConfigurationManager.AppSettings["Use Start Time"];
			string useStopTime = ConfigurationManager.AppSettings["Use Stop Time"];

			if (useArrivalTime != null)
			{
				this.ignoreArrivalTimeCheckBox.Checked = !Convert.ToBoolean(useArrivalTime);
			}

			if (useStartTime != null)
			{
				this.ignoreStartTimeCheckBox.Checked = !Convert.ToBoolean(useStartTime);
			}

			if (useStopTime != null)
			{
				this.ignoreStopTimeCheckBox.Checked = !Convert.ToBoolean(useStopTime);
			}
		}

		private void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (!this.VerifyBosLength())
				{
					return;
				}

				if (!this.VerifyTecLength())
				{
					return;
				}

				if (!this.VerifyTimeAreCorrect())
				{
					return;
				}

				if (FMChannelHelper.MakeCall<IClientDispatchService, bool>(this.VerifyAircfartSetupIsCorrect) == false)
				{
					return;
				}

				if (!this.VerifyVolumeIsCorrect())
				{
					return;
				}

				// Defuel/Refuel only
				if (!this.VerifyEdipiNumber())
				{
					return;
				}

				if (!this.VerifyFillStandLocation())
				{
					return;
				}

				this.SaveTransaction();
				this.lastTransaction = this.transaction;
				this.EquipmentRefID = this.detailRegistrationIDComboBox.Text;

				if (this.transToProcess != null && this.transToProcess.HasAnotherTransaction)
				{
					this.ResetForm(true);
					this.transToProcess.GetNextTransactionID();
					this.FuelRequestFormLoad(null, null);
					this.FuelRequestFormActivated(null, null);

					return;
				}

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);

				if (this.invalidRegistrationID)
				{
					this.invalidRegistrationID = false;
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.detailRegistrationIDComboBox.DroppedDown = true;
				}
			}
		}

		private void CancelButtonClick(object sender, EventArgs e)
		{
			if (this.transToProcess != null && this.transToProcess.HasAnotherTransaction)
			{
				TransactionDO lastSavedTransaction = this.lastTransaction;
				this.ResetForm(true);

				// This is because ResetForm will set it, but this was a cancel,so we dont want to
				this.lastTransaction = lastSavedTransaction; 
				this.transToProcess.GetNextTransactionID();
				this.FuelRequestFormLoad(null, null);
				this.FuelRequestFormActivated(null, null);
				this.DialogResult = DialogResult.None;

				return;
			}

			this.DialogResult = DialogResult.Cancel;
		}

		private void SaveFillStandTransaction()
		{
			string selectedItem = this.fillStandRequestTypeComboBox.SelectedItem as string;
			this.transaction.LineItems[0].PartialFill = selectedItem == "Partial Fill" || selectedItem == "Partial Return To Bulk";

			if (!string.IsNullOrEmpty(this.fillStandRegistrationIDComboBox.Text))
			{
				EquipmentDO equipmentDO = (this.transaction.Alias == this.fillStandTransactionAlias) 
													? this.transaction.DestinationEQ1 : this.transaction.SourceEQ1;
				EquipmentDO lineItemEquipmentDO = (this.transaction.Alias == this.fillStandTransactionAlias) 
													? this.transaction.LineItems[0].DestinationEQ : this.transaction.LineItems[0].SourceEQ;

				equipmentDO.RegistrationID = this.fillStandRegistrationIDComboBox.Text;
				lineItemEquipmentDO.RegistrationID = this.fillStandRegistrationIDComboBox.Text;

				FMChannelHelper.MakeCall<IClientDispatchService>(
					equipments =>
					{
						var equipmentGuid = equipments.GetEquipmentGuidById(this.security, equipmentDO.RegistrationID);

						if (equipmentGuid != Guid.Empty)
						{
							EquipmentClass equipment = equipments.GetEquipment(this.security, equipmentGuid);
							equipmentDO.SerialNumber = equipment.SerialNumber;
							equipmentDO.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
							equipmentDO.EquipmentGuid = equipment.MasterRecordGuid;

							lineItemEquipmentDO.SerialNumber = equipment.SerialNumber;
							lineItemEquipmentDO.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
							lineItemEquipmentDO.EquipmentGuid = equipment.MasterRecordGuid;
						}
					});
			}
			else
			{
				if (this.requestType == REQUEST_TYPE.FastLogFillStand
				|| this.transaction.Status == TransactionStatus.Dispatched
				|| this.transaction.Status == TransactionStatus.Arrived
				|| this.transaction.Status == TransactionStatus.Started
				|| this.transaction.Status == TransactionStatus.Stopped
				|| this.transaction.Status == TransactionStatus.Completed)
				{
					this.invalidRegistrationID = true;
					throw new Exception("Registration ID is required.");
				}
			}

			if (!string.IsNullOrEmpty(this.fillStandGradeComboBox.Text))
			{
				this.transaction.LineItems[0].Product = this.fillStandGradeComboBox.Text;

				FMChannelHelper.MakeCall<IClientDispatchService>(
					products =>
					{
						var productGuid = products.GetProductGuidById(this.security, this.transaction.LineItems[0].Product);

						if (productGuid != Guid.Empty)
						{
							ProductClass product = products.GetProduct(this.security, productGuid);
							this.transaction.LineItems[0].ProductType = ProductClass.ProductTypeID(product.ProductType);
							this.transaction.LineItems[0].ProductCode = product.Code;
							this.transaction.LineItems[0].ProductGuid = product.MasterRecordGuid;
						}
					});
			}
			else
			{
				throw new Exception("Invalid Grade ID");
			}

			if (string.IsNullOrEmpty(this.operatorComboBox.Text) == false)
			{
				var oper = (PersonClass) this.operatorComboBox.SelectedItem;

				if (oper != null)
				{
					this.transaction.OperatorID = oper.ID;
					this.transaction.OperatorName = oper.FullName;
				}
				else
				{
					this.transaction.OperatorID = this.operatorComboBox.Text;
					this.transaction.OperatorName = this.operatorComboBox.Text;
				}

				var operatorGuid =
					FMChannelHelper.MakeCall<IClientDispatchService, Guid>(personnel => personnel.GetPersonGuidById(this.security, this.transaction.OperatorID));

				if (operatorGuid != Guid.Empty)
				{
					this.transaction.OperatorPersonnelGuid = operatorGuid;
				}
			}
			else
			{
				if (this.requestType == REQUEST_TYPE.FastLogFillStand
					|| this.transaction.Status == TransactionStatus.Dispatched
					|| this.transaction.Status == TransactionStatus.Arrived
					|| this.transaction.Status == TransactionStatus.Started
					|| this.transaction.Status == TransactionStatus.Stopped
					|| this.transaction.Status == TransactionStatus.Completed)
				{
					throw new Exception("Invalid Operator");
				}
			}

			if (this.fillStandRequestCancelledCheckBox.Checked)
			{
				this.transaction.Status = TransactionStatus.Cancelled;

				foreach (LineItemDO lineItem in this.transaction.LineItems)
				{
					lineItem.Status = TransactionStatus.Cancelled;
					lineItem.Quantity = new QuantityDO(0, 0, 0, 0);
				}
			}

			this.transaction.Notes = this.fillStandCommentTextBox.Text;
			this.transaction.ContactSurname = this.fillStandRequestedByTextBox.Text;

			if (this.quantityTextBox.Enabled && this.quantityTextBox.Text != string.Empty)
			{
				this.transaction.LineItems[0].Quantity.Gross = Convert.ToDouble(this.quantityTextBox.Text);
				this.transaction.LineItems[0].Quantity.Net = this.transaction.LineItems[0].Quantity.Gross;
				this.transaction.UserData1 = "U.S. Gallons";
			}
			else if (this.quantityTextBox2.Enabled && this.quantityTextBox2.Text != string.Empty)
			{
				this.transaction.LineItems[0].Quantity.Gross = Convert.ToDouble(this.quantityTextBox2.Text);
				this.transaction.LineItems[0].Quantity.Net = this.transaction.LineItems[0].Quantity.Gross;
				this.transaction.UserData1 = "U.S. Gallons";
			}
			else
			{
				if (this.transaction.Status == TransactionStatus.Completed)
				{
					throw new Exception("Invalid Quantity");
				}
			}

			this.transaction.UserData7 = this.fillStandLocationTextBox.Text;
			this.transaction.UserData8 = this.radioNumberTextBox.Text;

			TextBox[] userDataTextBox = this.UserDataTextBoxes;
			ComboBox[] userDataComboBox = this.UserDataComboBoxes;

			int index = 0;

			foreach (var fieldClass in this.transactionAlias.LineItemUserDataFieldCollection)
			{
				var lineItemUserDataField = (UserDataFieldClass)fieldClass;

				if (lineItemUserDataField.UserDataType == USER_DATA_TYPE.LIST)
				{
					this.transaction.LineItems[0].UserData[
						lineItemUserDataField.EntityType + " User Data " + (lineItemUserDataField.Number + 1)] =
										userDataComboBox[index].SelectedText;
				}
				else
				{
					this.transaction.LineItems[0].UserData[lineItemUserDataField.EntityType + " User Data " 
										+ (lineItemUserDataField.Number + 1)] = userDataTextBox[index].Text;
				}

				index++;
			}

			if (string.IsNullOrEmpty(this.differentialPressureAndVarianceTextBox.Text) == false)
			{
				double variance = Convert.ToDouble(this.differentialPressureAndVarianceTextBox.Text);
				this.transaction.Number01 = variance;
			}
			else
			{
				this.transaction.Number01 = null;
			}
		}

		private void SaveFuelRequestTransaction()
		{
			if (string.IsNullOrEmpty(this.fuelRequestRefCodeComboBox.Text)
				&& this.requestType != REQUEST_TYPE.Transient)
			{
				throw new NullReferenceException("Ref ID cannot be blank.");
			}

			if (this.fuelRequestRefCodeComboBox.Text != null)
			{
				this.transaction.UserData18 = this.fuelRequestRefCodeComboBox.Text;
			}

			if (string.IsNullOrEmpty(this.aircraftIDComboBox.Text) == false)
			{
				EquipmentDO equipmentDO = (this.transaction.Alias == this.fuelRequestTransactionAlias)
											  ? this.transaction.DestinationEQ1
											  : this.transaction.SourceEQ1;

				EquipmentDO lineItemEquipmentDO = (this.transaction.Alias == this.fuelRequestTransactionAlias)
													  ? this.transaction.LineItems[0].DestinationEQ
													  : this.transaction.LineItems[0].SourceEQ;

				equipmentDO.RegistrationID = this.aircraftIDComboBox.Text;
				lineItemEquipmentDO.RegistrationID = this.aircraftIDComboBox.Text;

				FMChannelHelper.MakeCall<IClientDispatchService>(
					equipments =>
					{
						Guid equipmentGuid = equipments.GetEquipmentGuidById(this.security, equipmentDO.RegistrationID);

						if (equipmentGuid != Guid.Empty)
						{
							EquipmentClass equipment = equipments.GetEquipment(this.security, equipmentGuid);
							equipmentDO.SerialNumber = equipment.SerialNumber;
							equipmentDO.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
							equipmentDO.EquipmentGuid = equipment.MasterRecordGuid;
							equipmentDO.EquipmentModel = this.mdsTextBox.Text;

							lineItemEquipmentDO.SerialNumber = equipment.SerialNumber;
							lineItemEquipmentDO.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
							lineItemEquipmentDO.EquipmentGuid = equipment.MasterRecordGuid;
							lineItemEquipmentDO.EquipmentModel = this.mdsTextBox.Text;
						}
						else
						{
							// Don't have the equipment yet.  Must at least save the MDS
							equipmentDO.EquipmentModel = this.mdsTextBox.Text;
							lineItemEquipmentDO.EquipmentModel = this.mdsTextBox.Text;
						}
					});
			}
			else
			{
				throw new Exception("Invalid Aircraft ID");
			}

			// When Activity Is selected attempt to fill in Manager, Owner and Shipper.
			this.transaction.FuelCardID = this.activityComboBox.Text;
			this.transaction.FuelCardGuid = Guid.Empty;

			FMChannelHelper.MakeCall<IClientDispatchService>(
				fuelCards =>
				{
					if (!string.IsNullOrEmpty(this.transaction.FuelCardID))
					{
						Guid fuelCardGuid = fuelCards.GetFuelCardGuidById(this.security, this.transaction.FuelCardID);

						if (fuelCardGuid != Guid.Empty)
						{
							this.transaction.FuelCardGuid = fuelCardGuid;

							FuelCardClass fuelCard = fuelCards.GetFuelCard(this.security, fuelCardGuid);

							FMChannelHelper.MakeCall<IClientDispatchService>(
								companies =>
								{
									if (fuelCard.ManagerGuid != Guid.Empty)
									{
										CompanyClass manager = companies.GetCompany(this.security, fuelCard.ManagerGuid);
										this.transaction.ManagerID = manager.ID;
										this.transaction.ManagerCode = manager.Code;
										this.transaction.ManagerCompanyGuid = manager.MasterRecordGuid;
									}

									if (fuelCard.OwnerGuid != Guid.Empty)
									{
										CompanyClass owner = companies.GetCompany(this.security, fuelCard.OwnerGuid);
										this.transaction.OwnerID = owner.ID;
										this.transaction.OwnerCode = owner.Code;
										this.transaction.OwnerCompanyGuid = owner.MasterRecordGuid;
									}

									if (fuelCard.ShipperGuid != Guid.Empty)
									{
										CompanyClass shipper = companies.GetCompany(this.security, fuelCard.ShipperGuid);
										this.transaction.ShipperID = shipper.ID;
										this.transaction.ShipperCode = shipper.Code;
										this.transaction.ShipperCompanyGuid = shipper.MasterRecordGuid;
									}
								});
						}
					}
				});

			if (!string.IsNullOrEmpty(this.detailRegistrationIDComboBox.Text))
			{
				EquipmentDO equipmentDO = (this.transaction.Alias == this.fuelRequestTransactionAlias) 
													? this.transaction.SourceEQ1 : this.transaction.DestinationEQ1;
				EquipmentDO lineItemEquipmentDO = (this.transaction.Alias == this.fuelRequestTransactionAlias) 
													? this.transaction.LineItems[0].SourceEQ : this.transaction.LineItems[0].DestinationEQ;

				equipmentDO.RegistrationID = this.detailRegistrationIDComboBox.Text;
				lineItemEquipmentDO.RegistrationID = equipmentDO.RegistrationID;

				FMChannelHelper.MakeCall<IClientDispatchService>(
					equipments =>
					{
						Guid equipmentGuid = equipments.GetEquipmentGuidById(this.security, equipmentDO.RegistrationID);

						if (equipmentGuid != Guid.Empty)
						{
							EquipmentClass equipment = equipments.GetEquipment(this.security, equipmentGuid);
							equipmentDO.SerialNumber = equipment.SerialNumber;
							equipmentDO.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
							equipmentDO.EquipmentGuid = equipment.MasterRecordGuid;

							lineItemEquipmentDO.SerialNumber = equipment.SerialNumber;
							lineItemEquipmentDO.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
							lineItemEquipmentDO.EquipmentGuid = equipment.MasterRecordGuid;
						}
					});
			}
			else
			{
				if (this.requestType == REQUEST_TYPE.FastLog
					|| this.transaction.Status == TransactionStatus.Dispatched
					|| this.transaction.Status == TransactionStatus.Arrived
					|| this.transaction.Status == TransactionStatus.Started
					|| this.transaction.Status == TransactionStatus.Stopped
					|| this.transaction.Status == TransactionStatus.Completed)
				{
					this.invalidRegistrationID = true;
					throw new Exception("Registration ID is required.");
				}
			}

			if (!string.IsNullOrEmpty(this.fuelRequestGradeComboBox.Text))
			{
				this.transaction.LineItems[0].Product = this.fuelRequestGradeComboBox.Text;

				FMChannelHelper.MakeCall<IClientDispatchService>(
					products =>
					{
						Guid productGuid = products.GetProductGuidById(this.security, this.transaction.LineItems[0].Product);
						if (productGuid != Guid.Empty)
						{
							ProductClass product = products.GetProduct(this.security, productGuid);
							this.transaction.LineItems[0].ProductType = ProductClass.ProductTypeID(product.ProductType);
							this.transaction.LineItems[0].ProductCode = product.Code;
							this.transaction.LineItems[0].ProductGuid = product.MasterRecordGuid;
						}
					});
			}
			else
			{
				throw new Exception("Invalid Grade ID");
			}

			if (!string.IsNullOrEmpty(this.operatorComboBox.Text))
			{
				var oper = (PersonClass) this.operatorComboBox.SelectedItem;
				if (oper != null)
				{
					this.transaction.OperatorID = oper.ID;
					this.transaction.OperatorName = oper.FullName;
				}
				else
				{
					this.transaction.OperatorID = this.operatorComboBox.Text;
					this.transaction.OperatorName = this.operatorComboBox.Text;
				}

				FMChannelHelper.MakeCall<IClientDispatchService>(
					personnel =>
					{
						Guid operatorGuid = personnel.GetPersonGuidById(this.security, this.transaction.OperatorID);

						if (operatorGuid != Guid.Empty)
						{
							var person = personnel.GetPerson(this.security, operatorGuid);
							this.transaction.OperatorPersonnelGuid = person.MasterRecordGuid;
						}
					});
			}
			else
			{
				if (this.requestType == REQUEST_TYPE.FastLog 
					|| this.transaction.Status == TransactionStatus.Dispatched
					|| this.transaction.Status == TransactionStatus.Arrived 
					|| this.transaction.Status == TransactionStatus.Started
					|| this.transaction.Status == TransactionStatus.Stopped 
					|| this.transaction.Status == TransactionStatus.Completed)
				{
					throw new Exception("Invalid Operator");
				}
			}

			this.transaction.PaymentInfo.CreditCardNumber = this.cardNumberTextBox.Text;

			if (!this.SetShipToInformation()
				&& string.IsNullOrEmpty(this.transaction.PaymentInfo.CreditCardNumber)
				&& this.requestType != REQUEST_TYPE.Transient)
			{
				throw new Exception("Invalid DODACC");
			}

			this.SetBillToInformation();
			this.transaction.Flag04 = this.fuelAdditiveCheckBox.Checked;

			if (this.fuelRequestRequestCancelledCheckBox.Checked)
			{
				this.transaction.Status = TransactionStatus.Cancelled;

				foreach (var lineItem in this.transaction.LineItems)
				{
					lineItem.Status = TransactionStatus.Cancelled;
					lineItem.Quantity = new QuantityDO(0, 0, 0, 0);
				}
			}

			this.transaction.Notes = this.fuelRequestCommentTextBox.Text;
			this.transaction.ContactSurname = this.fuelRequestRequestedByTextBox.Text;
			this.transaction.ContactFirstName = this.contactTextBox.Text;
			this.transaction.ContactInfo = this.phoneTextBox.Text;

			if (this.quantityTextBox.Enabled && this.quantityTextBox.Text != "")
			{
				this.transaction.LineItems[0].Quantity.Gross = Convert.ToDouble(this.quantityTextBox.Text);
				this.transaction.LineItems[0].Quantity.Net = this.transaction.LineItems[0].Quantity.Gross;
				this.transaction.UserData1 = "U.S. Gallons";
			}
			else if (this.quantityTextBox2.Enabled && this.quantityTextBox2.Text != string.Empty)
			{
				this.transaction.LineItems[0].Quantity.Gross = Convert.ToDouble(this.quantityTextBox2.Text);
				this.transaction.LineItems[0].Quantity.Net = this.transaction.LineItems[0].Quantity.Gross;
				this.transaction.UserData1 = "U.S. Gallons";
			}
			else
			{
				if (this.transaction.Status == TransactionStatus.Completed)
				{
					throw new Exception("Invalid Quantity");
				}
			}

			EquipmentDO eqDO = (this.transaction.Alias == this.fuelRequestTransactionAlias) 
										? this.transaction.SourceEQ1 : this.transaction.DestinationEQ1;

			if (eqDO.EquipmentGuid != Guid.Empty)
			{
				EquipmentClass eq = FMChannelHelper.MakeCall<IClientDispatchService, EquipmentClass>(x => x.GetEquipment(this.Security, eqDO.EquipmentGuid));

				// These well be transfered from the boxes to the transaction later in SaveTransaction()
				if (string.IsNullOrEmpty(this.serialnumbertextBox.Text))
				{
					this.serialnumbertextBox.Text = eq.SerialNumber;
				}

				if (string.IsNullOrEmpty(this.isspttextBox.Text))
				{
					this.isspttextBox.Text = eq.IssPt;
				}

				if (string.IsNullOrEmpty(this.issptnumtextBox.Text))
				{
					this.issptnumtextBox.Text = eq.IssPtNum;
				}
			}

			this.transaction.UserData3 = this.rptTecTextBox.Text;
			this.transaction.UserData5 = this.fundCodeTextBox.Text;
			this.transaction.UserData7 = this.fuelRequestLocationTextBox.Text;
			this.transaction.UserData8 = this.radioNumberTextBox.Text;
			this.transaction.UserData10 = this.differentialPressureAndVarianceTextBox.Text;
			this.transaction.UserData11 = this.address1TextBox.Text;
			this.transaction.UserData12 = this.emailTextBox.Text;
			this.transaction.UserData15 = this.cityTextBox.Text;
			this.transaction.UserData17 = this.stateTextBox.Text;
			this.transaction.UserData19 = this.bosComboBox.Text;
			this.transaction.UserData20 = this.signalCodeComboBox.Text;
			this.transaction.UserData21 = this.useCodeComboBox.Text;
			this.transaction.UserData22 = this.zipTextBox.Text;
			this.transaction.UserData24 = this.faxTextBox.Text;
			this.transaction.UserData23 = this.memoTextBox.Text;

			TextBox[] userDataTextBox = this.UserDataTextBoxes;
			ComboBox[] userDataComboBox = this.UserDataComboBoxes;

			int index = 0;

			foreach (var fieldClass in this.transactionAlias.LineItemUserDataFieldCollection)
			{
				var lineItemUserDataField = (UserDataFieldClass)fieldClass;

				if (lineItemUserDataField.UserDataType == USER_DATA_TYPE.LIST)
				{
					this.transaction.LineItems[0].UserData["TALUD" + (lineItemUserDataField.Number + 1)] = 
																			userDataComboBox[index].SelectedText;
				}
				else
				{
					this.transaction.LineItems[0].UserData["TALUD" + (lineItemUserDataField.Number + 1)] = userDataTextBox[index].Text;
				}

				index++;
			}
		}

		private void SaveTransaction()
		{
			var timeConverter = new SiteTimeConverter(this.site);

			if (this.transaction == null)
			{
				this.transaction = new TransactionDO
				                   {
					                   TransID				= FuelsManagerId.NewId(),
					                   Site					= this.security.SiteID,
					                   SiteGuid				= this.security.SiteGuid,
					                   Alias				= this.transactionAlias.ID,
					                   TransTypeID			= this.transactionAlias.TransTypeID,
					                   TransactionAliasGuid = this.transactionAlias.MasterRecordGuid,
					                   DocumentNumber		= this.GenerateDocumentNumbers(this.transactionAlias.TransTypeID)
				                   };

				this.transaction.LineItems.Add(new LineItemDO());

				var inventoryDateSR = new InventoryDateSR
										  {
											  Security = this.Security,
											  CurrentSiteGuid = this.Security.SiteGuid
										  };

				var inventoryDateDO =
					FMChannelHelper.MakeCall<IClientDispatchService, InventoryDateDO>(x => x.ProcessInventoryDateServiceRequest(inventoryDateSR));

				this.transaction.InventoryDate = inventoryDateDO.InventoryDate;
				this.transaction.TransactionDateTime = timeConverter.Now();
				this.transaction.OriginApplication = TransactionOrigin.Dispatch;
				this.transaction.SubmittedToAccounting = false;

				if (this.requestType == REQUEST_TYPE.RequestFuel
				|| this.requestType == REQUEST_TYPE.Transient
				|| this.requestType == REQUEST_TYPE.FillStand)
				{
					this.transaction.Status = TransactionStatus.Requested;

					foreach (LineItemDO lineItemDO in this.transaction.LineItems)
					{
						lineItemDO.Status = TransactionStatus.Requested;
					}
				}
				else
				{
					this.transaction.Status = TransactionStatus.Completed;

					foreach (LineItemDO lineItemDO in this.transaction.LineItems)
					{
						lineItemDO.Status = TransactionStatus.Completed;
					}

					this.transaction.TimeOut = timeConverter.Now();
				}

				var managerCollection =
					FMChannelHelper.MakeCall<IClientDispatchService, CompanyCollectionClass>(
						x => x.EnumerateCompanyByRole(this.security, COMPANY_ROLE.MANAGER));

				if (managerCollection.Count == 0)
				{
					throw new Exception("No Manager");
				}

				if (managerCollection.Count > 1)
				{
					string strMgrs = string.Empty;

					foreach (CompanyClass manager in managerCollection)
					{
						if (strMgrs.Length > 0)
						{
							strMgrs += ", ";
						}
						strMgrs += string.Format("{0}", manager.Name);
					}

					string errorMsg = string.Format("Multiple managers are not allowed. {0} managers were found. They are {1}.", 
													managerCollection.Count, 
													strMgrs);

					throw new Exception(errorMsg);
				}

				this.transaction.ManagerID = managerCollection[0].ID;
				this.transaction.ManagerCode = managerCollection[0].Code;
				this.transaction.ManagerCompanyGuid = managerCollection[0].MasterRecordGuid;

				var ownerCollection =
					FMChannelHelper.MakeCall<IClientDispatchService, CompanyCollectionClass>(
						x => x.EnumerateCompanyByRole(this.security, COMPANY_ROLE.OWNER));

				if (ownerCollection.Count == 0)
				{
					throw new Exception("No Owner");
				}

				if (ownerCollection.Count > 1)
				{
					throw new Exception("Multiple Owners");
				}

				this.transaction.OwnerID = ownerCollection[0].ID;
				this.transaction.OwnerCode = ownerCollection[0].Code;
				this.transaction.OwnerCompanyGuid = ownerCollection[0].MasterRecordGuid;
				this.transaction.Number02 = Convert.ToDouble(this.requestType);
			}

			if (this.TransactionWillBeSetToCompleted)
			{
				this.transaction.Status = TransactionStatus.Completed;

				foreach (LineItemDO lineItemDO in this.transaction.LineItems)
				{
					lineItemDO.Status = TransactionStatus.Completed;
				}

				this.transaction.RequestedDateTime = timeConverter.Now();
				this.transaction.TimeOut = timeConverter.Now();
			}

			if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage))
			{
				this.SaveFillStandTransaction();
			}
			else
			{
				this.SaveFuelRequestTransaction();
			}

			this.transaction.RequestedDateTime = 
						this.ConvertToDateTimeOffset(this.requestDateDateTimePicker.Value, this.requestDateTimePicker.Value);

			// Requested date could be in the past.  
			DateTime requestedDate = this.requestDateDateTimePicker.Value;
			this.transaction.InventoryDate = new DateTime(requestedDate.Year, requestedDate.Month, requestedDate.Day);

			if (this.ignoreDispatchTimeCheckBox.Checked == false)
			{
				if (this.transaction.Status == TransactionStatus.Dispatched 
					|| this.transaction.Status == TransactionStatus.Arrived
				    || this.transaction.Status == TransactionStatus.Started 
					|| this.transaction.Status == TransactionStatus.Stopped
				    || this.transaction.Status == TransactionStatus.Completed)
				{
					this.transaction.DispatchedDateTime = 
								this.ConvertToDateTimeOffset(this.DispatchdatePicker.Value, this.dispatchDateTimePicker.Value);
				}
			}
			else
			{
				this.transaction.DispatchedDateTime = null;
			}

			if (this.ignoreArrivalTimeCheckBox.Checked == false)
			{
				if (this.transaction.Status == TransactionStatus.Arrived 
					|| this.transaction.Status == TransactionStatus.Started
				    || this.transaction.Status == TransactionStatus.Stopped
				    || this.transaction.Status == TransactionStatus.Completed)
				{
					this.transaction.TimeIn = this.ConvertToDateTimeOffset(this.ArrivaldatePicker.Value, this.arrivalDateTimePicker.Value);
				}
			}
			else
			{
				this.transaction.TimeIn = null;
			}

			if (this.ignoreStartTimeCheckBox.Checked == false)
			{
				if (this.transaction.Status == TransactionStatus.Started
					|| this.transaction.Status == TransactionStatus.Stopped
					|| this.transaction.Status == TransactionStatus.Completed)
				{
					this.transaction.RouteSchedule.FST = 
									this.ConvertToDateTimeOffset(this.StartdatePicker.Value, this.startDateTimePicker.Value);
				}
			}
			else
			{
				this.transaction.RouteSchedule.FST = null;
			}

			if (this.ignoreStopTimeCheckBox.Checked == false)
			{
				if (this.transaction.Status == TransactionStatus.Stopped || this.transaction.Status == TransactionStatus.Completed)
				{
					this.transaction.TimeEnd = this.ConvertToDateTimeOffset(this.StopdatePicker.Value, this.stopDateTimePicker.Value);
				}
			}
			else
			{
				this.transaction.TimeEnd = null;
			}

			if (this.transaction.Status == TransactionStatus.Completed)
			{
				this.transaction.TimeOut = this.ConvertToDateTimeOffset(this.CompletiondatePicker.Value, this.completionDateTimePicker.Value);

				// Set arrival time if the user has set to ignore. This will ensure the
				// time will not change when the page is reopened.
				if (this.ignoreArrivalTimeCheckBox.Checked && this.transaction.TimeIn == null)
				{
					this.transaction.TimeIn = this.transaction.TimeOut;
				}

				// Set start time if the user has set to ignore. This will ensure the
				// time will not change when the page is reopened.
				if (this.ignoreStartTimeCheckBox.Checked && this.transaction.RouteSchedule.FST == null)
				{
					this.transaction.RouteSchedule.FST = this.transaction.TimeOut;
				}

				// Set stop time if the user has set to ignore. This will ensure the
				// time will not change when the page is reopened.
				if (this.ignoreStopTimeCheckBox.Checked && this.transaction.TimeEnd == null)
				{
					this.transaction.TimeEnd = this.transaction.TimeOut;
				}
			}

			if (this.transaction.Alias == this.fuelRequestTransactionAlias)
			{
				this.transaction.LineItems[0].Quantity.Gross *= -1;
				this.transaction.LineItems[0].Quantity.Net *= -1;
			}

			LineItemDO lineItem = this.transaction.LineItems[0];
			EquipmentClass fillstandEquipment;

			// if this is a rtb then the sourceeq gets set and the destinationeq will be blank
			bool thisIsArtb = this.fillStandRequestTypeComboBox.SelectedItem as string == "Return To Bulk";

			bool thirdConsequtivePromptIssueAndPassed = false;

			// used to determine if we need to tell the user again
			int currentConsecutiveOosVariance = 0;	

			// we only check the variance on a fillstand and rtb the first time the data is entered. If we are editing the record then
			// we do not check the variance even if the operator has chenged the quantity
			// Good
			if ((this.requestType == REQUEST_TYPE.FillStand || this.requestType == REQUEST_TYPE.FastLogFillStand
				&& string.IsNullOrEmpty(this.transaction.Notes))
				 && (string) this.fillStandRequestTypeComboBox.SelectedItem != "Partial Fill" &&
				 this.transaction.Status != TransactionStatus.Cancelled &&
				 this.transactionInitialyCompleted == false)
			{
				fillstandEquipment = null;

				FMChannelHelper.MakeCall<IClientDispatchService>(
					equipments =>
					{
						if (thisIsArtb == false)
						{
							fillstandEquipment = equipments.GetEquipment(this.Security, lineItem.DestinationEQ.EquipmentGuid);
						}
						else
						{
							fillstandEquipment = equipments.GetEquipment(this.Security, lineItem.SourceEQ.EquipmentGuid);
						}
					});

				currentConsecutiveOosVariance = fillstandEquipment.Consecutive_OOS_Variance;

				if (fillstandEquipment != null)
				{
					double safeFill = Convert.ToDouble(fillstandEquipment.SafeFill);
					double volume = Convert.ToDouble(fillstandEquipment.Volume);

					double localVariance;
					double tolerance;

					if (thisIsArtb)
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

					// verify that the variance has not changed direction before issueing the message
					if ((fillstandEquipment.Consecutive_OOS_Variance == -2 && localVariance < 0)
						 || (fillstandEquipment.Consecutive_OOS_Variance == 2 && localVariance > 0))
					{
						// Check the tolerance - if we will be at three once we save the transaction, we need to
						// require a comment.
						if (tolerance >= 2
							&& (string.IsNullOrEmpty(this.transaction.Notes) 
							|| this.lastTransactionMemo.ToUpper() == this.transaction.Notes.ToUpper()))
						{
							this.lastTransactionMemo = this.transaction.Notes;

							if (this.requestType == REQUEST_TYPE.FillStand || this.requestType == REQUEST_TYPE.FastLogFillStand)
							{
								this.fuelingServiceRequestTabControl.SelectedTab = this.fillStandTabPage;
								this.fillStandCommentTextBox.Focus();
							}

							throw new ApplicationException(DeviationCommentRequiredMessage);
						}
						
						thirdConsequtivePromptIssueAndPassed = true;
					}
				}
			}

			this.transaction.UserData4 = this.serialnumbertextBox.Text;
			this.transaction.IssuePoint = this.isspttextBox.Text;
			this.transaction.IssuePointNumber = this.issptnumtextBox.Text;

			if (!string.IsNullOrEmpty(this.grossgaltextBox.Text))
			{
				this.transaction.Number03 = Convert.ToDouble(this.grossgaltextBox.Text);
			}

			this.SaveTransaction(this.transaction);

			if (this.transaction.TransactionGuid == Guid.Empty)
			{
				TransactionDO localTransaction = this.GetTransaction(this.transaction.TransID);
				this.transaction.TransactionGuid = localTransaction.TransactionGuid;
			}

			this.lastTransactionMemo = string.Empty;

			// Check the variance - we need a warning message if this is the first occurance of a variance
			// over 2%.  This must be done after the transaction has been saved since the variance count
			// is calculated by a trigger on the line item table.
			// if this is a rtb then the sourceeq gets set and the destinationeq will be blank
			if ((thirdConsequtivePromptIssueAndPassed == false &&
					  (this.requestType == REQUEST_TYPE.FillStand ||
					  this.requestType == REQUEST_TYPE.FastLogFillStand))
				 && (string) this.fillStandRequestTypeComboBox.SelectedItem != "Partial Fill" &&
				 this.transaction.Status != TransactionStatus.Cancelled &&
				 this.transactionInitialyCompleted == false)
			{
				fillstandEquipment = null;

				Guid equipGuid;

				if (thisIsArtb == false)
				{
					equipGuid = lineItem.DestinationEQ.EquipmentGuid;

				}
				else
				{
					equipGuid = lineItem.SourceEQ.EquipmentGuid;
				}

				fillstandEquipment =
						FMChannelHelper.MakeCall<IClientDispatchService, EquipmentClass>(
													x => x.GetEquipment(this.Security, equipGuid));

				if (fillstandEquipment != null)
				{
					if (currentConsecutiveOosVariance != fillstandEquipment.Consecutive_OOS_Variance)
					{
						int checkInterval = (fillstandEquipment.Consecutive_OOS_Variance % 3);

						if (checkInterval == 1 || checkInterval == -1)
						{
							// First occurrence of 2% loss.  First occurrence of 2% gain.
							string message = string.Format("First occurrence of 2% {0}.", (checkInterval > 0) ? "gain" : "loss");

							MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.OK);
							this.transaction.TransactionNoteGuid = 
								this.SaveTransactionNote(this.transaction.TransactionGuid, message, this.transaction.Notes);
						}
						else if (checkInterval == 2  || checkInterval == -2)
						{
							string message = string.Format("Second occurrence of 2% {0}.", (fillstandEquipment.Consecutive_OOS_Variance > 0) ? "gain" : "loss");

							MessageBox.Show(this, message, "Dispatch", MessageBoxButtons.OK);
							this.transaction.TransactionNoteGuid =
								this.SaveTransactionNote(this.transaction.TransactionGuid, message, this.transaction.Notes);
						}
					}
				}
			}

			if (this.transaction.Alias == this.fuelRequestTransactionAlias)
			{
				this.transaction.LineItems[0].Quantity.Gross *= -1;
				this.transaction.LineItems[0].Quantity.Net *= -1;
			}
		}

		/// <summary>
		/// This method will convert a Date Time value into a Date Time Offset setting the
		/// offset value.
		/// </summary>
		/// <param name="dateValue">Date portion of the Date Time.</param>
		/// <param name="timeValue">Time portion of the Date Time.</param>
		/// <returns>Returns a date time offset.</returns>
		private DateTimeOffset ConvertToDateTimeOffset(DateTime dateValue, DateTime timeValue)
		{
			var timeConverter = new SiteTimeConverter(this.site);

			var dateTimeValue = new DateTime(dateValue.Year,
											dateValue.Month,
											dateValue.Day,
											timeValue.Hour,
											timeValue.Minute,
											timeValue.Second);

			TimeSpan offsetValue = timeConverter.Now().Offset;
			var newDateTimeOffset = new DateTimeOffset(dateTimeValue, offsetValue);

			return newDateTimeOffset;
		}

		private void PerformDefuelSaleSwitchIfNeeded()
		{
			if (this.transaction != null)
			{
				if (this.transaction.Alias == this.defuelRequestTransactionAlias 
					&& this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Refuel")
				{
					this.transaction.Alias = this.fuelRequestTransactionAlias;

					var saleGuid = FMChannelHelper.MakeCall<IClientDispatchService, Guid>(x => x.GetTransactionAliasMasterRecordGuid(this.Security, "Sale"));

					this.transaction.TransactionAliasGuid = saleGuid;
					this.transaction.TransTypeID = TransactionTypes.T6_SecondaryDisbursement;

					this.transaction.DestinationEQ1 = this.transaction.SourceEQ1;
					this.transaction.LineItems[0].DestinationEQ = this.transaction.LineItems[0].SourceEQ;

					this.transaction.SourceEQ1 = new EquipmentDO
												{
													EquipmentModel = string.Empty,
													EquipmentType = string.Empty,
													RegistrationID = string.Empty,
													EquipmentRefID = string.Empty
												};

					this.transaction.LineItems[0].SourceEQ = new EquipmentDO
					                                         {
						                                         EquipmentModel = string.Empty,
						                                         EquipmentType = string.Empty,
						                                         EquipmentRefID = string.Empty,
						                                         RegistrationID = string.Empty
					                                         };

					this.detailRegistrationIDComboBox.Text = string.Empty;
					this.detailRegistrationIDComboBox.SelectedIndex = -1;

					this.operatorComboBox.Text = string.Empty;
					this.operatorComboBox.SelectedIndex = -1;
				}
				else if (this.transaction.Alias == this.fuelRequestTransactionAlias 
						&& this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Defuel")
				{
					this.transaction.Alias = this.defuelRequestTransactionAlias;

					var defuelGuid = FMChannelHelper.MakeCall<IClientDispatchService, Guid>(x => x.GetTransactionAliasMasterRecordGuid(this.Security, "Defuel"));

					this.transaction.TransactionAliasGuid = defuelGuid;
					this.transaction.TransTypeID = TransactionTypes.T4_SecondaryDefuel;

					this.transaction.SourceEQ1 = this.transaction.DestinationEQ1;
					this.transaction.LineItems[0].SourceEQ = this.transaction.LineItems[0].DestinationEQ;

					this.transaction.DestinationEQ1 = new EquipmentDO
					                                  {
						                                  EquipmentModel = string.Empty,
						                                  EquipmentType = string.Empty,
						                                  EquipmentRefID = string.Empty,
						                                  RegistrationID = string.Empty
					                                  };

					this.transaction.LineItems[0].DestinationEQ = new EquipmentDO
					                                              {
						                                              EquipmentModel = string.Empty,
						                                              EquipmentType = string.Empty,
						                                              EquipmentRefID = string.Empty,
						                                              RegistrationID = string.Empty
					                                              };

					this.detailRegistrationIDComboBox.Text = string.Empty;
					this.detailRegistrationIDComboBox.SelectedIndex = -1;

					this.operatorComboBox.Text = string.Empty;
					this.operatorComboBox.SelectedIndex = -1;
				}
			}
		}

		private void ApplyButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (!this.VerifyBosLength())
				{
					return;
				}

				if (!this.VerifyTecLength())
				{
					return;
				}

				if (!this.VerifyTimeAreCorrect())
				{
					return;
				}

				if (FMChannelHelper.MakeCall<IClientDispatchService, bool>(this.VerifyAircfartSetupIsCorrect) == false)
				{
					return;
				}

				if (!this.VerifyVolumeIsCorrect())
				{
					return;
				}

				if (!this.VerifyFillStandLocation())
				{
					return;
				}

				this.SaveTransaction();
				this.ResetForm(this.isNewRequestForm);

				if (this.isNewRequestForm)
				{
					this.FuelRequestFormLoad(null, null);
				}
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}

			if (this.requestType == REQUEST_TYPE.FastLog|| this.requestType == REQUEST_TYPE.FastLogFillStand)
			{
				this.fuelingServiceRequestTabControl.SelectedTab = this.fuelRequestTabPage;
			}
		}

		private void ResetForm(bool clearValues)
		{

			this.fuelRequestRequestTypeComboBox.SelectedIndexChanged -= this.RequestTypeComboBoxSelectedIndexChanged;
			this.fuelRequestGradeComboBox.SelectedIndexChanged -= this.FuelRequestGradeComboBoxSelectedIndexChanged;
			this.fuelRequestRefCodeComboBox.SelectedIndexChanged -= this.FuelRequestRefCodeComboBoxSelectedIndexChanged;
			this.aircraftIDComboBox.SelectedIndexChanged -= this.AircraftIDComboBoxSelectedIndexChanged;
			this.activityComboBox.SelectedIndexChanged -= this.ActivityComboBoxSelectedIndexChanged;
			this.fillStandRequestTypeComboBox.SelectedIndexChanged -= this.FillStandRequestTypeComboBoxSelectedIndexChanged;
			this.fillStandLocationComboBox.SelectedIndexChanged -= this.FillStandLocationComboBoxSelectedIndexChanged;
			this.fillStandRegistrationIDComboBox.SelectedIndexChanged -= this.FillStandRegistrationIDComboBoxSelectedIndexChanged;
			this.fillStandRegistrationIDComboBox.TextChanged -= this.FillStandRegistrationIDComboBoxTextChanged;
			this.fillStandGradeComboBox.SelectedIndexChanged -= this.FillStandGradeComboBoxSelectedIndexChanged;
			this.fillStandRefCodeComboBox.SelectedIndexChanged -= this.FillStandRefCodeComboBoxSelectedIndexChanged;
			this.detailRegistrationIDComboBox.SelectedIndexChanged -= this.DetailRegistrationIDComboBoxSelectedIndexChanged;

			if (clearValues)
			{
				this.lastTransaction = this.transaction;
				this.transaction = null;
				this.ReInitialize();
				this.fuelRequestRefCodeComboBox.Select();
			}

			this.SetRequestTypeComboBox();

			this.fuelRequestRequestTypeComboBox.SelectedIndexChanged += this.RequestTypeComboBoxSelectedIndexChanged;
			this.fuelRequestGradeComboBox.SelectedIndexChanged += this.FuelRequestGradeComboBoxSelectedIndexChanged;
			this.fuelRequestRefCodeComboBox.SelectedIndexChanged += this.FuelRequestRefCodeComboBoxSelectedIndexChanged;
			this.aircraftIDComboBox.SelectedIndexChanged += this.AircraftIDComboBoxSelectedIndexChanged;
			this.activityComboBox.SelectedIndexChanged += this.ActivityComboBoxSelectedIndexChanged;
			this.fillStandRequestTypeComboBox.SelectedIndexChanged += this.FillStandRequestTypeComboBoxSelectedIndexChanged;
			this.fillStandLocationComboBox.SelectedIndexChanged += this.FillStandLocationComboBoxSelectedIndexChanged;
			this.fillStandRegistrationIDComboBox.SelectedIndexChanged += this.FillStandRegistrationIDComboBoxSelectedIndexChanged;
			this.fillStandRegistrationIDComboBox.TextChanged += this.FillStandRegistrationIDComboBoxTextChanged;
			this.fillStandGradeComboBox.SelectedIndexChanged += this.FillStandGradeComboBoxSelectedIndexChanged;
			this.fillStandRefCodeComboBox.SelectedIndexChanged += this.FillStandRefCodeComboBoxSelectedIndexChanged;
			this.detailRegistrationIDComboBox.SelectedIndexChanged += this.DetailRegistrationIDComboBoxSelectedIndexChanged;
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the activityComboBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void ActivityComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{

			if (this.activityComboBox.SelectedIndex > -1
				&& this.activityComboBox.SelectedValue is Guid
				&& (Guid) this.activityComboBox.SelectedValue != Guid.Empty)
			{
				var cardGuid = (Guid) this.activityComboBox.SelectedValue;

				var fuelCard =
					FMChannelHelper.MakeCall<IClientDispatchService, FuelCardClass>(x => x.GetFuelCard(this.security, cardGuid));

				this.useCodeComboBox.Text = fuelCard.UserData2.TrimToMaxLength(1);
				this.signalCodeComboBox.Text = fuelCard.UserData1.TrimToMaxLength(1);
				this.fundCodeTextBox.Text = fuelCard.UserData3.TrimToMaxLength(this.fundCodeTextBox.MaxLength);
				this.bosComboBox.Text = fuelCard.UserData4.TrimToMaxLength(this.bosComboBox.MaxLength);
				this.rptTecTextBox.Text = fuelCard.UserData5.TrimToMaxLength(this.rptTecTextBox.MaxLength);
				this.dodaccTextBox.Text = fuelCard.ShipToID;

				if (fuelCard.ShipToID != fuelCard.BillToID)
				{
					this.suppDODACCTextBox.Text = fuelCard.BillToID;
				}
				else
				{
					this.suppDODACCTextBox.Text = string.Empty;
				}
			}
			else
			{
				if (this.requestType != REQUEST_TYPE.Transient)
				{
					this.useCodeComboBox.Text = string.Empty;
					this.signalCodeComboBox.Text = string.Empty;
					this.fundCodeTextBox.Text = string.Empty;
					this.bosComboBox.Text = string.Empty;
					this.rptTecTextBox.Text = string.Empty;
					this.dodaccTextBox.Text = string.Empty;
					this.suppDODACCTextBox.Text = string.Empty;
				}
			}

			if (this.handleActivityComboBoxEvents)
			{
				this.handleAircraftIDComboBoxEvents = false;
				this.PopulateAircraftIDComboBox();
				this.handleAircraftIDComboBoxEvents = true;
			}
		}

		private void AircraftIDComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.handleAircraftIDComboBoxEvents)
			{
				return;
			}

			if (this.aircraftIDComboBox.SelectedValue is Guid
				&& (Guid) this.aircraftIDComboBox.SelectedValue != Guid.Empty)
			{
				this.handleActivityComboBoxEvents = false;
				this.handleGradeComboBoxEvents = false;

				var aircraftGuid = (Guid) this.aircraftIDComboBox.SelectedValue;

				EquipmentClass equipment = FMChannelHelper.MakeCall<IClientDispatchService, EquipmentClass>(x => x.GetEquipment(this.security, aircraftGuid));

				if (equipment.FuelCardGuid != Guid.Empty)
				{
					this.activityComboBox.SelectedValue = equipment.FuelCardGuid;
				}
				else
				{
					this.activityComboBox.SelectedValue = -1;
				}

				if (equipment.ProductGuid != Guid.Empty)
				{
					var productList = (ProductCollectionClass) this.fuelRequestGradeComboBox.DataSource;
					ProductClass soonToBeSelectedItem = productList.Find(x => x.MasterRecordGuid == equipment.ProductGuid);
					this.fuelRequestGradeComboBox.SelectedValue = soonToBeSelectedItem.IdentityGuid;
				}
				else
				{
					this.fuelRequestGradeComboBox.SelectedIndex = -1;
				}

				this.PopulateRegistrationIDComboBox();

				this.mdsTextBox.Text = equipment.Model;
				this.fuelAdditiveCheckBox.Checked = equipment.FuelAdditiveFlag;

				if (!string.IsNullOrEmpty(equipment.UserData10))
				{
					this.cardNumberTextBox.Text = equipment.UserData10;
				}
				else
				{
					this.cardNumberTextBox.Text = string.Empty;
				}

				this.handleActivityComboBoxEvents = true;
				this.handleGradeComboBoxEvents = true;
			}
			else
			{
				this.mdsTextBox.Text = string.Empty;
			}

			if (this.aircraftIDComboBox.SelectedIndex >= 0)
			{
				var equipment = (EquipmentClass) this.aircraftIDComboBox.SelectedItem;

				this.handleAircraftIDComboBoxEvents = false;

				// If there is equipment select it here or just leave the current text
				if (!string.IsNullOrEmpty(equipment.Xref))
				{
					this.fuelRequestRefCodeComboBox.SelectedIndex = this.fuelRequestRefCodeComboBox.FindStringExact(equipment.Xref);
				}

				this.handleAircraftIDComboBoxEvents = true;
			}
		}

		private void RequestTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			this.GetTransactionAlias();
			this.ConfigureAdditionalDataFields();
			this.PopulateRegistrationIDComboBox();

			this.handleAircraftIDComboBoxEvents = false;
			this.PopulateAircraftIDComboBox();
			this.handleAircraftIDComboBoxEvents = true;

			this.PerformDefuelSaleSwitchIfNeeded();
		}

		private Label[] UserDataLabels
		{
			get
			{
				Label[] userDataLabel ={this.userData1Label,this.userData2Label,this.userData3Label,
												this.userData4Label,this.userData5Label,this.userData6Label,
												this.userData7Label,this.userData8Label,this.userData9Label,
												this.userData10Label,this.userData11Label,this.userData12Label,
												this.userData13Label,this.userData14Label,this.userData15Label,
												this.userData16Label,this.userData17Label,this.userData18Label,
												this.userData19Label,this.userData20Label,this.userData21Label,
												this.userData22Label,this.userData23Label,this.userData24Label};

				return userDataLabel;
			}
		}

		private TextBox[] UserDataTextBoxes
		{
			get
			{
				TextBox[] userDataTextBox ={this.userData1TextBox,this.userData2TextBox,this.userData3TextBox,
												this.userData4TextBox,this.userData5TextBox,this.userData6TextBox,
												this.userData7TextBox,this.userData8TextBox,this.userData9TextBox,
												this.userData10TextBox,this.userData11TextBox,this.userData12TextBox,
												this.userData13TextBox,this.userData14TextBox,this.userData15TextBox,
												this.userData16TextBox,this.userData17TextBox,this.userData18TextBox,
												this.userData19TextBox,this.userData20TextBox,this.userData21TextBox,
												this.userData22TextBox,this.userData23TextBox,this.userData24TextBox};

				return userDataTextBox;
			}
		}

		private ComboBox[] UserDataComboBoxes
		{
			get
			{
				ComboBox[] userDataComboBox ={this.userData1ComboBox,this.userData2ComboBox,this.userData3ComboBox,
												this.userData4ComboBox,this.userData5ComboBox,this.userData6ComboBox,
												this.userData7ComboBox,this.userData8ComboBox,this.userData9ComboBox,
												this.userData10ComboBox,this.userData11ComboBox,this.userData12ComboBox,
												this.userData13ComboBox,this.userData14ComboBox,this.userData15ComboBox,
												this.userData16ComboBox,this.userData17ComboBox,this.userData18ComboBox,
												this.userData19ComboBox,this.userData20ComboBox,this.userData21ComboBox,
												this.userData22ComboBox,this.userData23ComboBox,this.userData24ComboBox};

				return userDataComboBox;
			}
		}

		private void FillStandRequestTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			// Always update the variance so it gets removed for partial fills
			this.UpdateVariance();

			if (this.transactionAlias != null 
				&& this.transactionAlias.ID == this.fillStandTransactionAlias
			    && ((string)this.fillStandRequestTypeComboBox.SelectedItem == "Fill"
			        || (string)this.fillStandRequestTypeComboBox.SelectedItem == "Partial Fill"))
			{
				return;
			}

			this.GetTransactionAlias();
			this.ConfigureAdditionalDataFields();
		}

		private void FuelRequestGradeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.handleGradeComboBoxEvents)
			{
				return;
			}

			this.handleAircraftIDComboBoxEvents = false;
			this.handleActivityComboBoxEvents = false;
			this.PopulateAircraftIDComboBox();
			this.PopulateRegistrationIDComboBox();
			this.handleAircraftIDComboBoxEvents = true;
			this.handleActivityComboBoxEvents = true;
		}

		private void UpdateVariance()
		{
			string quantity = string.Empty;

			if (this.quantityTextBox.Enabled && string.IsNullOrEmpty(this.quantityTextBox.Text) == false)
			{
				quantity = this.quantityTextBox.Text;
			}
			else if (this.quantityTextBox2.Enabled && string.IsNullOrEmpty(this.quantityTextBox2.Text) == false)
			{
				quantity = this.quantityTextBox2.Text;
			}

			if (this.fillStandRequestTypeComboBox.SelectedItem as string == "Partial Fill"
				|| this.fillStandRegistrationIDComboBox.SelectedValue == null
				|| string.IsNullOrEmpty(quantity))
			{
				this.differentialPressureAndVarianceTextBox.Text = string.Empty;
				return;
			}

			var fillStandGuid = (Guid)this.fillStandRegistrationIDComboBox.SelectedValue;

			EquipmentClass equipment =
				FMChannelHelper.MakeCall<IClientDispatchService, EquipmentClass>(x => x.GetEquipment(this.security, fillStandGuid));

			double equipmentCapacity;
			double equipmentVolume;
			double volume = 0.0;

			try
			{
				equipmentCapacity = Convert.ToDouble(equipment.Capacity);
			}
			catch
			{
				MessageBox.Show(this, "Invalid equipment capacity", this.Text);
				this.differentialPressureAndVarianceTextBox.Text = string.Empty;
				return;
			}

			try
			{
				equipmentVolume = Convert.ToDouble(equipment.Volume);
			}
			catch
			{
				MessageBox.Show(this, "Invalid equipment volume", this.Text);
				this.differentialPressureAndVarianceTextBox.Text = string.Empty;
				return;
			}

			try
			{
				if (string.IsNullOrEmpty(quantity) == false)
				{
					volume = Convert.ToDouble(quantity);
				}
			}
			catch
			{
				MessageBox.Show(this, "Invalid quantity", this.Text);
				this.differentialPressureAndVarianceTextBox.Text = string.Empty;
				return;
			}

			double variance;

			if (this.transaction == null
				|| this.transaction.Number01 == null
				|| this.transaction.LineItems[0].Quantity == null)
			{

				if (this.fillStandRequestTypeComboBox.SelectedItem as string == "Return To Bulk")
				{
					variance = volume - equipmentVolume;
				}
				else
				{
					// if the volume added will fill above capacity then we do a different calculation
					if ((equipmentVolume + volume) > equipmentCapacity)
					{
						variance = equipmentCapacity - (equipmentVolume + volume);
					}
					else
					{
						variance = (equipmentCapacity - equipmentVolume) - volume;
					}
				}
			}

			else
			{
				variance = this.transaction.Number01.Value;
				double previousVolume = this.transaction.LineItems[0].Quantity.Gross;

				variance += volume - previousVolume;
			}

			this.differentialPressureAndVarianceTextBox.Text = variance.ToString(this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
		}

		private void QuantityTextBoxTextChanged(object sender, EventArgs e)
		{
			if (!this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage))
			{
				return;
			}

			this.UpdateVariance();
		}

		private void FillStandGradeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.handleGradeComboBoxEvents)
				return;

			this.PopulateRegistrationIDComboBox();
		}

		private void FillStandRegistrationIDComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.handleRegistrationIDComboBoxEvents)
			{
				return;
			}

			this.detailRegistrationIDComboBox.SelectedItem = this.fillStandRegistrationIDComboBox.SelectedItem;
			this.fillStandRefCodeComboBox.SelectedItem = this.fillStandRegistrationIDComboBox.SelectedItem;

			var equipment = (EquipmentClass) this.fillStandRegistrationIDComboBox.SelectedItem;

			if (equipment != null)
			{
				this.typeTextBox.Text = equipment.TypeClass;
			}
			else
			{
				this.typeTextBox.Text = string.Empty;
			}

			this.UpdateVariance();
		}

		private void FuelRequestRefCodeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.handleAircraftIDComboBoxEvents)
			{
				return;
			}

			if ((this.requestType == REQUEST_TYPE.Transient) &&
				 (string.IsNullOrEmpty(this.fuelRequestRefCodeComboBox.Text)))
			{
				// For transients, an empty ref code should not result in the
				// selection of an aircraft ID - it only means that the ref code
				// is not known.
				return;
			}

			// Filter the aircraft combo based on the selection from this one.  Only aircraft with 
			// the same ref code should be listed.
			var equipment = (EquipmentClass) this.fuelRequestRefCodeComboBox.SelectedItem;

			if (equipment != null)
			{

				var aircraft = from equipmentClass in this.equipmentCollection
					where equipmentClass.Xref.Equals(equipment.Xref)
					select equipmentClass;

				var newCollection = new EquipmentCollectionClass();

				foreach (var craft in aircraft)
				{
					newCollection.Add(craft);
				}

				this.aircraftIDComboBox.DataSource = newCollection;

				if (newCollection.Count > 0)
				{
					this.aircraftIDComboBox.SelectedIndex = 0;
				}
			}
			else
			{
				this.aircraftIDComboBox.DataSource = this.equipmentCollection;
			}

			this.fuelRequestLocationTextBox.Enabled = true;
			this.fuelRequestRequestedByTextBox.Enabled = true;
			this.fuelRequestCommentTextBox.Enabled = true;
		}

		private void FillStandRefCodeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.handleRegistrationIDComboBoxEvents)
			{
				return;
			}

			this.fillStandRegistrationIDComboBox.SelectedItem = this.fillStandRefCodeComboBox.SelectedItem;
			this.detailRegistrationIDComboBox.SelectedItem = this.fillStandRegistrationIDComboBox.SelectedItem;

			var equipment = (EquipmentClass) this.fillStandRefCodeComboBox.SelectedItem;

			if (equipment != null)
			{
				this.typeTextBox.Text = equipment.TypeClass;
			}
			else
			{
				this.typeTextBox.Text = string.Empty;
			}

			this.UpdateVariance();
		}

		private void FillStandRegistrationIDComboBoxTextChanged(object sender, EventArgs e)
		{
			if (!this.handleRegistrationIDComboBoxEvents)
			{
				return;
			}

			this.detailRegistrationIDComboBox.SelectedItem = this.fillStandRegistrationIDComboBox.SelectedItem;
			this.detailRegistrationIDComboBox.Text = this.fillStandRegistrationIDComboBox.Text;
		}

		private void RequestCancelledCheckBoxOnCheckedChanged(object sender, EventArgs e)
		{
			
			if (transaction == null && fuelRequestRequestCancelledCheckBox.Checked == true)
			{
				DisplayCancellationWarning(fuelRequestRequestCancelledCheckBox);
			}
			else if (transaction != null 
					&& fuelRequestRequestCancelledCheckBox.Checked == true
			        && transaction.Status != TransactionStatus.Cancelled)
			{
				DisplayCancellationWarning(fuelRequestRequestCancelledCheckBox);
			}

			// set quantity to 0 if it is being cancelled. Per bug 40277 Dispatch Cancel text box 
			if (fuelRequestRequestCancelledCheckBox.Checked == true)
			{
				this.quantityTextBox.Text = "0";
				this.quantityTextBox2.Text = "0";
				this.quantityTextBox.Enabled = false;
			}
			else
			{
				if (CompletionMode)
				{
					this.quantityTextBox.Enabled = true;
				}
			}
		}

		private void DisplayCancellationWarning(CheckBox selectedCheckBox)
		{
			const string Message = "Once an operation is canceled it cannot be un-canceled.\nAre you sure you want to cancel this job?";
			DialogResult result = MessageBox.Show(this, Message, "Fuel Request Form", MessageBoxButtons.YesNo);

			if (result == DialogResult.No)
			{
				selectedCheckBox.Checked = false;
			}
			else if (result == DialogResult.Yes)
			{
				// Quantity must be set to zero on cancellation.
				this.quantityTextBox.Text = "0";

				// set the focus to the comment field on the dialog. this is non standard but it is what they DOD wants
				this.fuelRequestCommentTextBox.Focus();
			}
		}

		private void FillStandRequestConcelledCheckBoxOnCheckedChanged(object sender, EventArgs e)
		{
			if (this.transaction == null && this.fillStandRequestCancelledCheckBox.Checked)
			{
				this.DisplayCancellationWarning(this.fillStandRequestCancelledCheckBox);
			}
			else if (this.transaction != null 
					&& this.fillStandRequestCancelledCheckBox.Checked 
					&& this.transaction.Status != TransactionStatus.Cancelled)
			{
				this.DisplayCancellationWarning(this.fillStandRequestCancelledCheckBox);
			}
		}

		private void SetControlEnabledDisabled()
		{
			// this routine sets the state of the displayed controls based on the data provided
			// if this is a new transaction disable the cancelled check box
			if (this.transaction == null)
			{
				this.fuelRequestRequestCancelledCheckBox.Checked = false;
				this.fuelRequestRequestCancelledCheckBox.Enabled = false;
				this.fillStandRequestCancelledCheckBox.Checked = false;
				this.fillStandRequestCancelledCheckBox.Enabled = false;
			}
			else
			{
				// if the transaction is cancelled disable the controls to prevent changes
				if (this.transaction.Status == TransactionStatus.Cancelled ||
					 this.transaction.Status == TransactionStatus.Posted)
				{
					this.okButton.Enabled = false;
					this.applyButton.Enabled = false;

					if (this.requestType == REQUEST_TYPE.FastLogFillStand)
					{
						this.fillStandRequestCancelledCheckBox.Enabled = false;
					}
					else
					{
						this.fuelRequestRequestCancelledCheckBox.Enabled = false;
					}
				}
			}
		}

		private void IgnoreDispatchTimeCheckBoxCheckChanged(object sender, EventArgs e)
		{
			if (this.ignoreDispatchTimeCheckBox.Checked)
			{
				this.dispatchDateTimePicker.Enabled = false;
				this.DispatchdatePicker.Enabled = false;
			}
			else
			{
				this.dispatchDateTimePicker.Enabled = true;
				this.DispatchdatePicker.Enabled = true;
			}
		}

		private void IgnoreArrivalTimeCheckBoxCheckChanged(object sender, EventArgs e)
		{
			if (this.ignoreArrivalTimeCheckBox.Checked)
			{
				this.arrivalDateTimePicker.Enabled = false;
				this.ArrivaldatePicker.Enabled = false;
			}
			else
			{
				this.arrivalDateTimePicker.Enabled = true;
				this.ArrivaldatePicker.Enabled = true;
			}
		}

		private void IgnoreStartTimeCheckBoxCheckChanged(object sender, EventArgs e)
		{
			if (this.ignoreStartTimeCheckBox.Checked)
			{
				this.startDateTimePicker.Enabled = false;
				this.StartdatePicker.Enabled = false;
			}
			else
			{
				this.startDateTimePicker.Enabled = true;
				this.StartdatePicker.Enabled = true;
			}
		}

		private void IgnoreStopTimeCheckBoxCheckChanged(object sender, EventArgs e)
		{
			if (this.ignoreStopTimeCheckBox.Checked)
			{
				this.stopDateTimePicker.Enabled = false;
				this.StopdatePicker.Enabled = false;
			}
			else
			{
				this.stopDateTimePicker.Enabled = true;
				this.StopdatePicker.Enabled = true;
			}

		}

		private void QuantityTextBox2TextChanged(object sender, EventArgs e)
		{
			if (!this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage))
			{
				return;
			}

			this.UpdateVariance();
		}

		private void RequestDateDateTimePickerValueChanged(object sender, EventArgs e)
		{
			// check if the value is before the lockout date and not allow it
			if (this.requestType != REQUEST_TYPE.FastLog
				 && this.requestType != REQUEST_TYPE.FastLogFillStand)
			{
				if (this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Refuel" ||
					 this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Defuel")
				{
					if (this.requestDateDateTimePicker.Value < this.operationLockDate)
					{
						const string ErrorMessage = "Request date can not be before current lock out date";

						// we get this update twice but only want to show the message once.
						if (this.iLoopCounter == 0)
						{
							MessageBox.Show(ErrorMessage);
							this.iLoopCounter = 1;
						}
						else if (this.iLoopCounter > 0)
						{
							this.iLoopCounter = 0;
						}

						this.requestDateDateTimePicker.Value = this.operationLockDate;
					}
				}
			}

			if (this.requestType == REQUEST_TYPE.FastLog || this.requestType == REQUEST_TYPE.FastLogFillStand)
			{
				this.startingRequestDate = this.requestDateDateTimePicker.Value;
			}

			// the only thing we want to do hear is reset the date values but not the time values
			this.requestDateTimePicker.Value = this.CreateNewDate(this.requestDateDateTimePicker.Value, this.requestDateTimePicker.Value);
			/*
							dispatchDateTimePicker.Value = CreateNewDate(requestDateDateTimePicker.Value, dispatchDateTimePicker.Value);
							DispatchdatePicker.Value = dispatchDateTimePicker.Value;

							arrivalDateTimePicker.Value = CreateNewDate(requestDateDateTimePicker.Value, arrivalDateTimePicker.Value);
							ArrivaldatePicker.Value = arrivalDateTimePicker.Value;

							startDateTimePicker.Value = CreateNewDate(requestDateDateTimePicker.Value, startDateTimePicker.Value);
							StartdatePicker.Value = startDateTimePicker.Value;

							stopDateTimePicker.Value = CreateNewDate(requestDateDateTimePicker.Value, stopDateTimePicker.Value);
							StopdatePicker.Value = stopDateTimePicker.Value;

							completionDateTimePicker.Value = CreateNewDate(requestDateDateTimePicker.Value, completionDateTimePicker.Value);
							CompletiondatePicker.Value = completionDateTimePicker.Value;
			 */
		}

		private DateTime CreateNewDate(DateTime dateTime1, DateTime dateTime2)
		{
			return new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, dateTime2.Hour, dateTime2.Minute, dateTime2.Second);
		}

		private void CompletionDateTimePickerValueChanged(object sender, EventArgs e)
		{
			// do not allow a completion date earlier then the lock out date OperationLockDate
			if (this.requestType != REQUEST_TYPE.FastLog
				 && this.requestType != REQUEST_TYPE.FastLogFillStand)
			{
				if (this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Refuel" ||
					 this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Defuel")
				{
					if (this.completionDateTimePicker.Value < this.operationLockDate)
					{
						this.completionDateTimePicker.Value = this.operationLockDate;
					}
				}
			}
		}

		void FillStandLocationComboBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.fillStandLocationTextBox.Text = this.fillStandLocationComboBox.Text;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void FillStandLocationComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.fillStandLocationTextBox.Text = this.fillStandLocationComboBox.Text;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private bool SetShipToInformation()
		{
			if (!string.IsNullOrEmpty(this.dodaccTextBox.Text))
			{
				this.transaction.ShipToID = this.dodaccTextBox.Text;
				var shipToGuid =
					FMChannelHelper.MakeCall<IClientDispatchService, Guid>(x => x.GetCompanyGuidById(this.security, this.transaction.ShipToID));
				
				if (shipToGuid != Guid.Empty)
				{
					CompanyClass shipTo = FMChannelHelper.MakeCall<IClientDispatchService, CompanyClass>(x => x.GetCompany(this.security, shipToGuid));
					this.transaction.ShipToID = shipTo.ID;
					this.transaction.ShipToCode = shipTo.Code;
					this.transaction.ShipToCompanyGuid = shipTo.MasterRecordGuid;
				}
			}
			else
			{
				// for transients the user is allowed to enter a record without a dodacc but they must have a dodacc when editing
				if (this.requestType == REQUEST_TYPE.Transient && this.isNewRequestForm)
				{
					return true;
				}
				
				return false;
			}
			
			return true;
		}

		private void SetBillToInformation()
		{
			if (!string.IsNullOrEmpty(this.suppDODACCTextBox.Text))
			{
				this.transaction.BillToID = this.suppDODACCTextBox.Text;

				var billToGuid =
					FMChannelHelper.MakeCall<IClientDispatchService, Guid>(x => x.GetCompanyGuidById(this.security, this.transaction.BillToID));

				if (billToGuid != Guid.Empty)
				{
					CompanyClass billTo = FMChannelHelper.MakeCall<IClientDispatchService, CompanyClass>(x => x.GetCompany(this.security, billToGuid));
					this.transaction.BillToID = billTo.ID;
					this.transaction.BillToCode = billTo.Code;
					this.transaction.BillToCompanyGuid = billTo.MasterRecordGuid;
				}
			}
		}

		private bool VerifyFillStandLocation()
		{
			if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fillStandTabPage))
			{

				var tmpEquip = fillStandLocationComboBox.SelectedItem as EquipmentClass;
				if (tmpEquip != null && !tmpEquip.InServiceFlag)
				{
					DialogResult result = MessageBox.Show(this,
													"This location is out of service.  Continue anyway?",
													"Fuel Request",
													MessageBoxButtons.YesNo,
													MessageBoxIcon.Warning);

					if (result == DialogResult.No)
					{
						return false;

					}
				}
			}

			return true;
		}

		private bool VerifyAircfartSetupIsCorrect(IClientDispatchService equipments)
		{
			// Check if a fueladditive is required and is selected
			// if not prompt the user for confirmation
			if (this.requestType == REQUEST_TYPE.FastLog)
			{
				if (this.aircraftIDComboBox.SelectedValue is Guid
					&& (Guid) this.aircraftIDComboBox.SelectedValue != Guid.Empty)
				{
					var equipmentGuid = (Guid) this.aircraftIDComboBox.SelectedValue;
					EquipmentClass equipmentaircraft = equipments.GetEquipment(this.security, equipmentGuid);

					if (equipmentaircraft != null && !string.IsNullOrEmpty(this.detailRegistrationIDComboBox.Text))
					{
						equipmentGuid = equipments.GetEquipmentGuidById(this.security, this.detailRegistrationIDComboBox.Text);

						if (equipmentGuid != Guid.Empty)
						{
							EquipmentClass equipment = equipments.GetEquipment(this.security, equipmentGuid);

							if (equipment != null &&
								 equipment.FuelingType == FUELING_TYPES.REFUELER)
							{
								if (equipment.FuelAdditiveFlag == false &&
									 equipmentaircraft.FuelAdditiveFlag)
								{
									DialogResult result = MessageBox.Show(this,
										 "Attention, The Aircraft requires a fuel additive but the Refueler does not have the additive. Create transaction anyway?",
										 "Fastlog",
										 MessageBoxButtons.YesNo,
										 MessageBoxIcon.Warning);

									if (result == DialogResult.No)
									{
										return false;
									}
								}
							}
						}
					}
				}
			}

			return true;
		}

		private bool VerifyVolumeIsCorrect()
		{
			if (this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Refuel" ||
				 this.fuelRequestRequestTypeComboBox.SelectedItem as string == "Defuel")
			{
				// The quantityTextBox will be enabled at this point when we are completing an item.  Before we are
				// completing a request the quantity may remain zero (and in fact may have to since the textbox is
				// disabled.
				if (this.quantityTextBox.Enabled)
				{
					double quantity = Convert.ToDouble(this.quantityTextBox.Text.DefaultIfNullOrEmpty("0"));

					// Allow a quantity of zero to be saved if the user is cancelling the transaction.
					// Otherwise, display an error message.
					if (quantity <= 0.0 && this.fuelRequestRequestCancelledCheckBox.Checked == false)
					{
						MessageBox.Show("Transaction cannot be saved with a quantity of zero.");
						this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
						this.quantityTextBox.Focus();
						return false;
					}
				}
			}

			return true;
		}

		private bool VerifyTecLength()
		{
			int length = 3; //default for airforce

			if (this.fuelingServiceRequestTabControl.TabPages.Contains(this.fuelRequestTabPage))
			{

				//logic copied from BSMEWebApp BSMECustomeScript.js to mimic what the web page is doing.
				if (this.dodaccTextBox.Text.Length > 0)
				{
					string branch = this.dodaccTextBox.Text.Substring(0, 1);

					if ((branch.ToUpper() == "W") || (branch.ToUpper() == "C") || (branch.ToUpper() == "A"))
					{
						//"ARMY";
						length = 4;
					}

					if ((branch.ToUpper() == "N")
						|| (branch.ToUpper() == "Q")
						|| (branch.ToUpper() == "V")
						|| (branch.ToUpper() == "M")
						|| (branch.ToUpper() == "R")
						|| (branch.ToUpper() == "U"))
					{
						length = 4;
						//"NAVY";
					}
				}

				if (this.rptTecTextBox.Text.Trim().Length != 0 && this.rptTecTextBox.Text.Trim().Length != length)
				{
					MessageBox.Show(String.Format("RPT/TEC/APC must be {0} characters in length.", length));
					this.fuelingServiceRequestTabControl.SelectedTab = this.fuelRequestTabPage;
					this.rptTecTextBox.Focus();
					
					return false;
				}
			}

			return true;
		}

		private bool VerifyBosLength()
		{
			// Default for airforce
			const int Length = 3; 

			if (this.bosComboBox.Text.Trim().Length != 0
				&& this.bosComboBox.Text.Trim().Length != Length)
			{
				MessageBox.Show(String.Format("BOS must be {0} characters in length.", Length));
				this.fuelingServiceRequestTabControl.SelectedTab = this.fuelRequestTabPage;
				this.bosComboBox.Focus();
				
				return false;
			}

			return true;
		}

		/// <summary>
		/// Verify EDIPI number in Defuel and Refuel transaction is valid
		/// </summary>
		/// <returns></returns>
		private bool VerifyEdipiNumber()
		{
			const long EdipiMinValue = 1000000000;
			const long EdipiMaxValue = 9999999999;

			bool isValid = true; // by default, for other transactions
			TextBox edipiTextBox;

			// if we found the text box, it is implied that it is Defuel/Refuel
			if (this.FindEdipiTextBox(out edipiTextBox))
			{
				string edipiString = edipiTextBox.Text;

				if (string.IsNullOrEmpty(edipiString) == false)
				{
					long edipiNumber;
					isValid = false;

					if (long.TryParse(edipiString, out edipiNumber))
					{
						isValid = (edipiNumber >= EdipiMinValue) && (edipiNumber <= EdipiMaxValue);
					}
				}
			}

			return isValid;
		}

		/// <summary>
		/// Find EDIPI number in Defuel/Refuel transaction.
		/// </summary>
		/// <param name="edipiTextBox"></param>
		/// <returns></returns>
		private bool FindEdipiTextBox(out TextBox edipiTextBox)
		{
			edipiTextBox = null;
			const int EdipiInternalIndex = 23;

			if ((this.transactionAlias != null) &&
				 (this.transactionAlias.ID == this.defuelRequestTransactionAlias 
				 || this.transactionAlias.ID == this.fuelRequestTransactionAlias))
			{
				// Find the EDIPI field
				int actualEdipiIndex = -1;

				for (int idx = 0; idx < this.transactionAlias.LineItemUserDataFieldCollection.Count; idx++)
				{
					var currentField = this.transactionAlias.LineItemUserDataFieldCollection[idx];

					if (currentField.Number == EdipiInternalIndex)
					{
						actualEdipiIndex = idx;
						break;
					}
				}

				if (actualEdipiIndex >= 0)
				{
					edipiTextBox = this.UserDataTextBoxes[actualEdipiIndex];
				}
			}

			return edipiTextBox != null;
		}

		private void FuelRequestFormActivated(object sender, EventArgs e)
		{
			if (this.Text.Contains("Fillstand Completion"))
			{
				this.quantityTextBox2.Focus();
			}
			else if (this.Text.Contains("Service Completion"))
			{
				this.quantityTextBox.Focus();
			}
		}

		private void DetailRegistrationIDComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			//detailRegistrationIDComboBox
			var selectedEquipmentItem = this.detailRegistrationIDComboBox.SelectedItem as EquipmentClass;
			
			if (selectedEquipmentItem != null)
			{
				this.isspttextBox.Text = selectedEquipmentItem.IssPt;
				this.issptnumtextBox.Text = selectedEquipmentItem.IssPtNum;
			}
			else
			{
				this.isspttextBox.Text = string.Empty;
				this.issptnumtextBox.Text = string.Empty;
			}
		}

		private bool VerifyTimeAreCorrect()
		{

			if (!this.CheckDispatchedTime())
			{
				return false;
			}

			if (!this.CheckArrivalTime())
			{
				return false;
			}

			if (!this.CheckStartTime())
			{
				return false;
			}

			if (!this.CheckStopTime())
			{
				return false;
			}

			if (!this.CheckCompletionTime())
			{
				return false;
			}

			return true;
		}

		private bool CheckDispatchedTime()
		{
			if (this.ignoreDispatchTimeCheckBox.Checked)
			{
				return true;
			}

			if ((this.DispatchdatePicker.Value.Date + this.dispatchDateTimePicker.Value.TimeOfDay) 
				< (this.requestDateDateTimePicker.Value.Date + this.requestDateTimePicker.Value.TimeOfDay))
			{
				MessageBox.Show("Dispatch time must be later than request time.");
				this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
				this.dispatchDateTimePicker.Focus();
				
				return false;
			}

			return true;
		}

		private bool CheckArrivalTime()
		{
			if (this.ignoreArrivalTimeCheckBox.Checked)
			{
				return true;
			}

			if (this.ignoreDispatchTimeCheckBox.Checked == false)
			{
				if ((this.ArrivaldatePicker.Value.Date + this.arrivalDateTimePicker.Value.TimeOfDay) 
					< (this.DispatchdatePicker.Value.Date + this.dispatchDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Arrival time must be later than dispatch Time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.arrivalDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if ((this.ArrivaldatePicker.Value.Date + this.arrivalDateTimePicker.Value.TimeOfDay) 
				< (this.requestDateDateTimePicker.Value.Date + this.requestDateTimePicker.Value.TimeOfDay))
			{
				MessageBox.Show("Arrival Time must be later than request time.");
				this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
				this.arrivalDateTimePicker.Focus();
				
				return false;
			}

			return true;
		}

		private bool CheckStartTime()
		{
			if (this.ignoreStartTimeCheckBox.Checked)
			{
				return true;
			}

			if (this.ignoreArrivalTimeCheckBox.Checked == false)
			{

				if ((this.StartdatePicker.Value.Date + this.startDateTimePicker.Value.TimeOfDay) 
					< (this.ArrivaldatePicker.Value.Date + this.arrivalDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Start time must be later than arrival time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.startDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if (this.ignoreDispatchTimeCheckBox.Checked == false)
			{
				if ((this.StartdatePicker.Value.Date + this.startDateTimePicker.Value.TimeOfDay) 
					< (this.DispatchdatePicker.Value.Date + this.dispatchDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Start Time must be later than dispatch time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.startDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if ((this.StartdatePicker.Value.Date + this.startDateTimePicker.Value.TimeOfDay) 
				< (this.requestDateDateTimePicker.Value.Date + this.requestDateTimePicker.Value.TimeOfDay))
			{
				MessageBox.Show("Start time must be later than request time.");
				this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
				this.startDateTimePicker.Focus();
				return false;
			}

			return true;
		}

		private bool CheckStopTime()
		{
			if (this.ignoreStopTimeCheckBox.Checked)
			{
				return true;
			}

			if (this.ignoreStartTimeCheckBox.Checked == false)
			{
				if ((this.StopdatePicker.Value.Date + this.stopDateTimePicker.Value.TimeOfDay) 
					< (this.StartdatePicker.Value.Date + this.startDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Stop time must be later than start time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.stopDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if (this.ignoreArrivalTimeCheckBox.Checked == false)
			{
				if ((this.StopdatePicker.Value.Date + this.stopDateTimePicker.Value.TimeOfDay) 
					< (this.ArrivaldatePicker.Value.Date + this.arrivalDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Stop time must be later than arrival time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.stopDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}
			if (this.ignoreDispatchTimeCheckBox.Checked == false)
			{
				if ((this.StopdatePicker.Value.Date + this.stopDateTimePicker.Value.TimeOfDay) 
					< (this.DispatchdatePicker.Value.Date + this.dispatchDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Stop time must be later than dispatch Time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.stopDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if ((this.StopdatePicker.Value.Date + this.stopDateTimePicker.Value.TimeOfDay) 
				< (this.requestDateDateTimePicker.Value.Date + this.requestDateTimePicker.Value.TimeOfDay))
			{
				MessageBox.Show("Stop time must be later than request Time.");
				this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
				this.stopDateTimePicker.Focus();
				
				return false;
			}

			return true;
		}

		private bool CheckCompletionTime()
		{
			if (this.ignoreStopTimeCheckBox.Checked == false)
			{
				if ((this.CompletiondatePicker.Value.Date + this.completionDateTimePicker.Value.TimeOfDay) 
					< (this.StopdatePicker.Value.Date + this.stopDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Completion time must be later than stop time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.completionDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if (this.ignoreStartTimeCheckBox.Checked == false)
			{
				if ((this.stopDateTimePicker.Value.Date + this.stopDateTimePicker.Value.TimeOfDay) 
					< (this.StartdatePicker.Value.Date + this.startDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Completion time must be later than start time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.completionDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if (this.ignoreArrivalTimeCheckBox.Checked == false)
			{
				if ((this.CompletiondatePicker.Value.Date + this.completionDateTimePicker.Value.TimeOfDay) 
					< (this.ArrivaldatePicker.Value.Date + this.arrivalDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Completion time must be later than arrival time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.completionDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if (this.ignoreDispatchTimeCheckBox.Checked == false)
			{
				if ((this.CompletiondatePicker.Value.Date + this.completionDateTimePicker.Value.TimeOfDay) 
					< (this.DispatchdatePicker.Value.Date + this.dispatchDateTimePicker.Value.TimeOfDay))
				{
					MessageBox.Show("Completion time must be later than dispatch time.");
					this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
					this.completionDateTimePicker.Focus();
					
					return false;
				}
				
				return true;
			}

			if ((this.CompletiondatePicker.Value.Date + this.completionDateTimePicker.Value.TimeOfDay) 
				< (this.requestDateDateTimePicker.Value.Date + this.requestDateTimePicker.Value.TimeOfDay))
			{
				MessageBox.Show("Completion time must be later than request time.");
				this.fuelingServiceRequestTabControl.SelectedTab = this.detailTabPage;
				this.stopDateTimePicker.Focus();
				
				return false;
			}

			return true;
		}

		private void ReInitialize()
		{
			this.rptTecTextBox.Text = string.Empty;
			this.cardNumberTextBox.Text = string.Empty;
			this.fundCodeTextBox.Text = string.Empty;
			this.signalCodeComboBox.SelectedIndex = -1;
			this.useCodeComboBox.SelectedIndex = -1;
			//this.bosComboBox.SelectedIndex=-1;
			this.bosComboBox.Text = string.Empty;
			this.suppDODACCTextBox.Text = string.Empty;
			this.dodaccTextBox.Text = string.Empty;
			this.fuelRequestRequestCancelledCheckBox.Checked = false;
			this.fuelRequestCommentTextBox.Text = string.Empty;
			this.fuelRequestRequestTypeComboBox.SelectedIndex = -1;
			this.fuelRequestRequestedByTextBox.Text = string.Empty;
			this.activityComboBox.SelectedIndex = -1;
			this.activityComboBox.Text = string.Empty;
			this.fuelAdditiveCheckBox.Checked = false;
			this.fuelRequestGradeComboBox.SelectedIndex = -1;
			this.fuelRequestGradeComboBox.Text = string.Empty;
			this.fuelRequestLocationTextBox.Text = string.Empty;
			this.mdsTextBox.Text = string.Empty;
			this.aircraftIDComboBox.SelectedIndex = -1;
			this.aircraftIDComboBox.Text = string.Empty;
			this.fuelRequestRefCodeComboBox.SelectedIndex = -1;
			this.fuelRequestRefCodeComboBox.Text = string.Empty;
			this.fillStandRequestCancelledCheckBox.Checked = false;
			this.fillStandCommentTextBox.Text = string.Empty;
			this.fillStandRequestTypeComboBox.SelectedIndex = -1;
			this.fillStandRequestedByTextBox.Text = string.Empty;
			this.fillStandLocationComboBox.SelectedIndex = -1;
			this.quantityTextBox2.Text = string.Empty;
			this.fillStandLocationTextBox.Text = string.Empty;
			this.typeTextBox.Text = string.Empty;
			this.fillStandRegistrationIDComboBox.SelectedIndex = -1;
			this.fillStandGradeComboBox.SelectedIndex = -1;
			this.fillStandRefCodeComboBox.SelectedIndex = -1;
			this.radioNumberTextBox.Text = string.Empty;
			this.differentialPressureAndVarianceTextBox.Text = string.Empty;
			this.operatorComboBox.SelectedIndex = -1;
			this.detailRegistrationIDComboBox.SelectedIndex = -1;
			this.quantityTextBox.Text = string.Empty;
			this.issptnumtextBox.Text = string.Empty;
			this.isspttextBox.Text = string.Empty;
			this.grossgaltextBox.Text = string.Empty;
			this.serialnumbertextBox.Text = string.Empty;
			this.transIDTextBox.Text = string.Empty;

			this.userData24ComboBox.SelectedIndex = -1;
			this.userData23ComboBox.SelectedIndex = -1;
			this.userData22ComboBox.SelectedIndex = -1;
			this.userData21ComboBox.SelectedIndex = -1;
			this.userData20ComboBox.SelectedIndex = -1;
			this.userData19ComboBox.SelectedIndex = -1;
			this.userData18ComboBox.SelectedIndex = -1;
			this.userData17ComboBox.SelectedIndex = -1;
			this.userData16ComboBox.SelectedIndex = -1;
			this.userData15ComboBox.SelectedIndex = -1;
			this.userData14ComboBox.SelectedIndex = -1;
			this.userData13ComboBox.SelectedIndex = -1;
			this.userData12ComboBox.SelectedIndex = -1;
			this.userData11ComboBox.SelectedIndex = -1;
			this.userData10ComboBox.SelectedIndex = -1;
			this.userData9ComboBox.SelectedIndex = -1;
			this.userData8ComboBox.SelectedIndex = -1;
			this.userData7ComboBox.SelectedIndex = -1;
			this.userData6ComboBox.SelectedIndex = -1;
			this.userData5ComboBox.SelectedIndex = -1;
			this.userData4ComboBox.SelectedIndex = -1;
			this.userData3ComboBox.SelectedIndex = -1;
			this.userData2ComboBox.SelectedIndex = -1;
			this.userData1ComboBox.SelectedIndex = -1;

			this.userData24TextBox.Text = string.Empty;
			this.userData21TextBox.Text = string.Empty;
			this.userData18TextBox.Text = string.Empty;
			this.userData15TextBox.Text = string.Empty;
			this.userData12TextBox.Text = string.Empty;
			this.userData9TextBox.Text = string.Empty;
			this.userData23TextBox.Text = string.Empty;
			this.userData20TextBox.Text = string.Empty;
			this.userData17TextBox.Text = string.Empty;
			this.userData14TextBox.Text = string.Empty;
			this.userData11TextBox.Text = string.Empty;
			this.userData8TextBox.Text = string.Empty;
			this.userData22TextBox.Text = string.Empty;
			this.userData19TextBox.Text = string.Empty;
			this.userData16TextBox.Text = string.Empty;
			this.userData13TextBox.Text = string.Empty;
			this.userData10TextBox.Text = string.Empty;
			this.userData7TextBox.Text = string.Empty;
			this.userData6TextBox.Text = string.Empty;
			this.userData5TextBox.Text = string.Empty;
			this.userData3TextBox.Text = string.Empty;
			this.userData2TextBox.Text = string.Empty;
			this.userData4TextBox.Text = string.Empty;
			this.userData1TextBox.Text = string.Empty;

			//this.contactTabPage = new System.Windows.Forms.TabPage();
			this.emailTextBox.Text = string.Empty;
			this.memoTextBox.Text = string.Empty;
			this.faxTextBox.Text = string.Empty;
			this.phoneTextBox.Text = string.Empty;
			this.zipTextBox.Text = string.Empty;
			this.stateTextBox.Text = string.Empty;
			this.cityTextBox.Text = string.Empty;
			this.address1TextBox.Text = string.Empty;
			this.contactTextBox.Text = string.Empty;
			//this.okButton = new System.Windows.Forms.Button();
			//this.cancelButton = new System.Windows.Forms.Button();
			//this.applyButton = new System.Windows.Forms.Button();
			this.comboBox1.SelectedIndex = -1;
			this.checkBox1.Checked = false;
			this.comboBox2.SelectedIndex = -1;
			this.textBox1.Text = string.Empty;
			this.textBox2.Text = string.Empty;
			this.comboBox3.SelectedIndex = -1;
			this.comboBox4.SelectedIndex = -1;
			this.comboBox5.SelectedIndex = -1;
			this.checkBox2.Checked = false;
			this.comboBox6.SelectedIndex = -1;
			this.textBox3.Text = string.Empty;
			this.textBox4.Text = string.Empty;
			this.comboBox7.SelectedIndex = -1;
			this.comboBox8.SelectedIndex = -1;

			this.ResetTimePickers(false);
		}

		private void ResetTimePickers(Boolean resetDateTime)
		{
			this.isNewRequestForm = (this.transaction == null);

			var timeConverter = new SiteTimeConverter(this.site);

			if ((this.requestType != REQUEST_TYPE.FastLog && this.requestType != REQUEST_TYPE.FastLogFillStand) || (!this.isNewRequestForm))
			{
				this.requestDateDateTimePicker.Enabled = false;
			}

			// format date controls based on site configuration (IGO 2010-Aug-13)
			this.GetSiteDateTimeFormatInfo();
			this.requestDateDateTimePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.DispatchdatePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.ArrivaldatePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.StartdatePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.StopdatePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortDatePattern;
			this.CompletiondatePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortDatePattern;

			this.requestDateTimePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortTimePattern;
			this.dispatchDateTimePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortTimePattern;
			this.arrivalDateTimePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortTimePattern;
			this.startDateTimePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortTimePattern;
			this.stopDateTimePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortTimePattern;
			this.completionDateTimePicker.CustomFormat = this.SiteDateTimeFormatInfo.ShortTimePattern;

			DateTime siteDateTime = timeConverter.Now().DateTime;

			// If the request type is a fast log of a fast log fillstand and the apply button
			// is clicked, the calling dialog will set the starting request date so we can
			// retain it.  Once the dialog is closed, the date is lost and will be set to the
			// current date when they open it again.
			if ((this.requestType == REQUEST_TYPE.FastLog || this.requestType == REQUEST_TYPE.FastLogFillStand)
				&& this.startingRequestDate != null)
			{
				this.requestDateDateTimePicker.Value = this.startingRequestDate.Value;
			}
			else
			{
				this.requestDateDateTimePicker.Value = siteDateTime;
			}

			//if (ResetDateTime) sjiang: In case all request forms do not need to reset data time, use this line of code and remove below.
			if (resetDateTime ||
				(!(this.requestType == REQUEST_TYPE.FastLog || this.requestType == REQUEST_TYPE.FastLogFillStand)))
			{
				this.requestDateTimePicker.Value = siteDateTime;

				this.dispatchDateTimePicker.Value = siteDateTime;
				this.DispatchdatePicker.Value = siteDateTime;

				this.arrivalDateTimePicker.Value = siteDateTime;
				this.ArrivaldatePicker.Value = siteDateTime;

				this.startDateTimePicker.Value = siteDateTime;
				this.StartdatePicker.Value = siteDateTime;

				this.stopDateTimePicker.Value = siteDateTime;
				this.StopdatePicker.Value = siteDateTime;

				this.completionDateTimePicker.Value = siteDateTime;
				this.CompletiondatePicker.Value = siteDateTime;

				this.SetTimeCheckboxToConfig();
			}
		}
	}
}
