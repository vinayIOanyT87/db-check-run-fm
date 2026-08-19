namespace Dispatch
{
	using System;
	using System.Windows.Forms;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class RecirculationForm : FMBaseForm
	{
		private readonly SiteClass site;
		private TransactionDO transaction;
		private readonly SecurityClass security;

		public string TransID
		{
			set
			{
				this.transaction = FMChannelHelper.MakeCall<IClientDispatchService, TransactionDO>(x => x.GetTransactionByTransID( security, value));
			}
		}

		public RecirculationForm()
		{
			this.GetSecurity();

			this.InitializeComponent();

			this.security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;
			if (this.security == null)
			{
				throw new Exception("Security not in AppDomain");
			}

			this.site = FMChannelHelper.MakeCall<IClientDispatchService, SiteClass>(x => x.GetSite(this.security, this.security.SiteGuid));

			// set the initial controls
			this.InitializeDialogDisplay();
		}

		private void InitializeDialogDisplay()
		{
			this.StartTimeSelection.CustomFormat = "MM/dd/yyyy  -  hh:mm:ss tt";
			this.StartTimeSelection.ShowCheckBox = false;
			this.StartTimeSelection.ShowUpDown = true;
			this.StartTimeSelection.Value = DateTime.Now;
			this.StopTimeSelection.CustomFormat = "MM/dd/yyyy  -  hh:mm:ss tt";
			this.StopTimeSelection.ShowCheckBox = false;
			this.StopTimeSelection.ShowUpDown = true;
			this.StopTimeSelection.Value = DateTime.Now;

			// load the available transactions for type 12
			this.PopulateEquipmentIDDropDownList();
			this.PopulateOperatorDropDownList();
			this.PopulateProductDropDownList();
			this.PopulateTransactionDescriptionDropDownList();

			this.NetVolumeTextBox.Text = "0";
			this.GrossVolumetextBox.Text = "0";
		}

		private void SetDialogTransactionData()
		{
			if (this.transaction.RouteSchedule.FST != null)
			{
				this.StartTimeSelection.Value = this.transaction.RouteSchedule.FST.Value.DateTime;
			}

			if (this.transaction.TimeEnd != null)
			{
				this.StopTimeSelection.Value = this.transaction.TimeEnd.Value.DateTime;
			}

			this.RegistrationIDDropDownList.SelectedIndex = 
								this.RegistrationIDDropDownList.FindString(this.transaction.SourceEQ1.RegistrationID);
			this.OperatorDropDownList.SelectedIndex = this.OperatorDropDownList.FindString(this.transaction.OperatorID);

			foreach (LineItemDO lineItemDO in this.transaction.LineItems)
			{
				if (lineItemDO.Product != null)
				{
					this.ProductDropDownList.SelectedIndex = this.ProductDropDownList.FindString(lineItemDO.Product);
					this.GrossVolumetextBox.Text = lineItemDO.Quantity.Gross.ToString(this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
					this.NetVolumeTextBox.Text = lineItemDO.Quantity.Net.ToString(this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
					break;
				}
			}

			if (this.transaction.UserData.ContainsKey("TAUD2"))
			{
				this.TransactionDescription.SelectedIndex = this.TransactionDescription.FindString((string)this.transaction.UserData["TAUD2"]);
			}

			if (this.transaction.UserData.ContainsKey("TAUD4"))
			{
				this.SerialNumbertextBox.Text = this.transaction.UserData["TAUD4"];
			}
			this.IssuePointtextBox.Text = this.transaction.IssuePoint;
			this.IssuePointNumbertextBox.Text = this.transaction.IssuePointNumber;
			if (this.transaction.UserData.ContainsKey("TAUD19"))
			{
				this.ServiceBranchtextBox.Text = this.transaction.UserData["TAUD19"];
			}
			this.CardNumbertextBox.Text = this.transaction.FuelCardID;
			this.MemotextBox.Text = this.transaction.Notes;
		}

		private void PopulateEquipmentIDDropDownList()
		{
			this.RegistrationIDDropDownList.Items.Clear();

			// once again we change our minds during testing we now want all managed equipment to be displayed
			EquipmentCollectionClass equipmentCollection = FMChannelHelper.MakeCall<IClientDispatchService, EquipmentCollectionClass>(
				x => x.EnumerateManagedEquipment(this.security));

			this.RegistrationIDDropDownList.DataSource = equipmentCollection;
		}

		private void PopulateOperatorDropDownList()
		{
			this.OperatorDropDownList.Items.Clear();

			// operators should be limited to drivers
			PersonCollectionClass personCollection = FMChannelHelper.MakeCall<IClientDispatchService, PersonCollectionClass>(
				x => x.EnumeratePersonnelByRole(this.security, PERSON_ROLE.LOADER_ROLE));

			this.OperatorDropDownList.DataSource = personCollection;
		}

		private void PopulateProductDropDownList()
		{
			this.ProductDropDownList.Items.Clear();
			ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IClientDispatchService, ProductCollectionClass>(
																x => x.EnumerateProducts(this.security));
			this.ProductDropDownList.DataSource = productCollection;
		}

		private void PopulateTransactionDescriptionDropDownList()
		{
			this.TransactionDescription.Items.Clear();

			this.TransactionDescription.Items.Add("Maintenance");
			this.TransactionDescription.Items.Add("Quality Control");
			this.TransactionDescription.Items.Add("Simulation Dry Run");
			this.TransactionDescription.Items.Add("Hose Pressure Test");
			this.TransactionDescription.Items.Add("Other");
			this.TransactionDescription.SelectedItem = this.TransactionDescription.Items[0];
		}

		private void OnCloseClicked(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Label5Click(object sender, EventArgs e)
		{

		}

		private void OnNetVolumeTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = this.VerifyNumberIsValid(e, this.NetVolumeTextBox);
		}

		private void OnGrossVolumeTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = this.VerifyNumberIsValid(e, this.GrossVolumetextBox);
		}

		private bool VerifyNumberIsValid(KeyPressEventArgs e, TextBox currentTextBox)
		{
			if (char.IsDigit(e.KeyChar))
			{
				return false;
			}

			// check for only one decimal point
			if (e.KeyChar == '.' && currentTextBox.Text.Contains(".") == false)
			{
				return false;
			}

			if (e.KeyChar == 8) // backspace
			{
				return false;
			}

			return true;
		}

		private void OnCardNumberTextChanged(object sender, EventArgs e)
		{
		}

		private void OnServiceBranchTextChanged(object sender, EventArgs e)
		{
		}

		private void OnIssuePointTextChanged(object sender, EventArgs e)
		{
		}

		private void OnIssuePointNumberTextChanged(object sender, EventArgs e)
		{
		}

		private void OnSerialNumberTextChanged(object sender, EventArgs e)
		{
		}

		private void OperatorDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void OnApplyClick(object sender, EventArgs eArgs)
		{
			try
			{
				if (!this.VolumesAreGreaterThenZero())
				{
					return;
				}

				this.SaveTransactionToDataBase();

				// remove the data from the designated boxes
				this.NetVolumeTextBox.Text = "0";
				this.GrossVolumetextBox.Text = "0";
				this.ServiceBranchtextBox.Text = string.Empty;
				this.CardNumbertextBox.Text = string.Empty;
				this.IssuePointNumbertextBox.Text = string.Empty;
				this.IssuePointtextBox.Text = string.Empty;
				this.SerialNumbertextBox.Text = string.Empty;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void SaveTransactionToDataBase()
		{
			bool newTransaction = false;
			TransactionDO transactionDO;

			if (this.transaction != null)
			{
				transactionDO = this.transaction;
			}
			else
			{
				newTransaction = true;
				transactionDO = new TransactionDO { TransID = FuelsManagerId.NewId() };
			}

			if (transactionDO == null)
			{
				throw new Exception("Transaction Creation Failure");
			}

			var timeConverter = new SiteTimeConverter(this.site);

			transactionDO.TransTypeID = TransactionTypes.T12_InventoryNotAffected;
			transactionDO.Site = this.security.SiteID;
			transactionDO.SiteGuid = this.security.SiteGuid;

			CompanyCollectionClass managerCollection = FMChannelHelper.MakeCall<IClientDispatchService, CompanyCollectionClass>(
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

				string errorMsg = String.Format("Multiple managers are not allowed. {0} managers were found. They are {1}.", 
												managerCollection.Count, 
												strMgrs);

				throw new Exception(errorMsg);
			}

			transactionDO.ManagerID = managerCollection[0].ID;
			transactionDO.ManagerCode = managerCollection[0].Code;
			transactionDO.ManagerCompanyGuid = managerCollection[0].MasterRecordGuid;

			CompanyCollectionClass ownerCollection = FMChannelHelper.MakeCall<IClientDispatchService, CompanyCollectionClass>(
						x => x.EnumerateCompanyByRole(this.security, COMPANY_ROLE.OWNER));

			if (ownerCollection.Count == 0)
			{
				throw new Exception("No Owner");
			}

			if (ownerCollection.Count > 1)
			{
				throw new Exception("Multiple Owner");
			}

			transactionDO.OwnerID = ownerCollection[0].ID;
			transactionDO.OwnerCode = ownerCollection[0].Code;
			transactionDO.OwnerCompanyGuid = ownerCollection[0].MasterRecordGuid;

			transactionDO.Status = TransactionStatus.Completed;

			foreach (LineItemDO lineItem in transactionDO.LineItems)
			{
				lineItem.Status = TransactionStatus.Completed;
			}

			var inventoryDateSR = new InventoryDateSR { Security = this.Security, CurrentSiteGuid = this.Security.SiteGuid };

			var inventoryDateDO =
				FMChannelHelper.MakeCall<IClientDispatchService, InventoryDateDO>(x => x.ProcessInventoryDateServiceRequest(inventoryDateSR));

			transactionDO.InventoryDate = inventoryDateDO.InventoryDate;
			transactionDO.TransactionDateTime = timeConverter.Now();
			transactionDO.OriginApplication = TransactionOrigin.Dispatch;
			transactionDO.SubmittedToAccounting = false;
			transactionDO.TimeEnd = this.StopTimeSelection.Value;
			transactionDO.RouteSchedule.FST = this.StartTimeSelection.Value;

			transactionDO.RequestedDateTime = timeConverter.Now();

			var equipment = (EquipmentClass) this.RegistrationIDDropDownList.SelectedItem;

			transactionDO.SourceEQ1.RegistrationID = equipment.ID;
			transactionDO.SourceEQ1.SerialNumber = equipment.SerialNumber;
			transactionDO.SourceEQ1.EquipmentGuid = equipment.MasterRecordGuid;
			transactionDO.SourceEQ1.EquipmentModel = equipment.Model;
			transactionDO.SourceEQ1.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
			transactionDO.SourceEQ1.CompanyEquipmentID = equipment.CompanyEquipmentID;

			LineItemDO lineItemDO = null;

			if (newTransaction)
			{
				lineItemDO = new LineItemDO();
			}
			else
			{
				foreach (LineItemDO lineItemTrans in this.transaction.LineItems)
				{
					if (lineItemTrans != null)
					{
						lineItemDO = lineItemTrans;
						break;
					}
				}
			}

			if (lineItemDO == null)
			{
				throw new Exception("Invalid Transaction");
			}

			lineItemDO.Status = TransactionStatus.Completed;

			var product = (ProductClass) this.ProductDropDownList.SelectedItem;
			lineItemDO.Product = product.ID;
			lineItemDO.ProductCode = product.Code;
			lineItemDO.ProductGuid = product.MasterRecordGuid;

			lineItemDO.Quantity = new QuantityDO
			                      {
				                      Gross = Convert.ToDouble(this.GrossVolumetextBox.Text),
				                      Net = Convert.ToDouble(this.NetVolumeTextBox.Text)
			                      };

			if (newTransaction)
			{
				transactionDO.LineItems.Add(lineItemDO);
			}

			transactionDO.Notes = this.MemotextBox.Text;
			transactionDO.Number03 = Convert.ToDouble(this.GrossVolumetextBox.Text);

			transactionDO.TransactionDateTime = DateTime.UtcNow;

			var person = (PersonClass) this.OperatorDropDownList.SelectedItem;
			transactionDO.OperatorID = this.OperatorDropDownList.Text;
			transactionDO.OperatorPersonnelGuid = person.MasterRecordGuid;
			transactionDO.OperatorName = person.FullName;

			transactionDO.UserData["TAUD1"] = "U.S. Gallons";
			transactionDO.UserData["TAUD2"] = this.TransactionDescription.Text;
			transactionDO.UserData["TAUD4"] = this.SerialNumbertextBox.Text;
			transactionDO.IssuePoint = this.IssuePointtextBox.Text;
			transactionDO.IssuePointNumber = this.IssuePointNumbertextBox.Text;
			transactionDO.UserData["TAUD19"] = this.ServiceBranchtextBox.Text;
			transactionDO.FuelCardID = this.CardNumbertextBox.Text;

			var transactionAlias =
				FMChannelHelper.MakeCall<IClientDispatchService, TransactionAliasClass>(
					x => x.GetTransactionAliasFromAliasId(this.security, this.TransactionTypeLabel.Text, false));

			transactionDO.Alias = transactionAlias.ID;
			transactionDO.TransactionAliasGuid = transactionAlias.MasterRecordGuid;
			transactionDO.TransTypeID = transactionAlias.TransTypeID;

			this.SaveTransaction(transactionDO);
		}

		private void OnSaveAndCloseClicked(object sender, EventArgs eArgs)
		{
			try
			{
				if (!this.VolumesAreGreaterThenZero())
					return;
				this.SaveTransactionToDataBase();
				this.Close();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString());
			}
		}

		private void OnLoad(object sender, EventArgs e)
		{
			if (this.transaction != null)
			{
				this.SetDialogTransactionData();
			}

			this.SaveandClosebutton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

			// if this is being edited then disable the apply button as requested
			if (this.transaction != null)
			{
				this.Applybutton.Enabled = false;

				// The transaction has been released to accounting and cannot be modified.
				if (this.transaction.SubmittedToAccounting == true)
				{
					this.SaveandClosebutton.Enabled = false;
				}
			}
			else
			{
				this.Applybutton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
			}
		}

		private bool VolumesAreGreaterThenZero()
		{
			if (string.IsNullOrEmpty(this.NetVolumeTextBox.Text) ||
				Convert.ToInt32(this.NetVolumeTextBox.Text) <= 0)
			{
				MessageBox.Show("Net Volume Must Be Greater Then 0.");
				this.NetVolumeTextBox.Focus();
				return false;
			}

			if (string.IsNullOrEmpty(this.GrossVolumetextBox.Text) ||
				Convert.ToInt32(this.GrossVolumetextBox.Text) <= 0)
			{
				MessageBox.Show("Gross Volume Must Be Greater Then 0.");
				this.GrossVolumetextBox.Focus();
				return false;
			}

			return true;
		}
	}
}
