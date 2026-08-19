using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.BusinessInterfaces;

namespace DispatchPrototype
{
	public partial class RecirculationForm : FMBaseForm
	{
		SiteClass site = null;
		TransactionDO transaction = null;
		SecurityClass security = null;

		public string TransID
		{
			set
			{
				TransactionSR transactionSR = new TransactionSR ( );
				transactionSR.Security = security;
				transactionSR.TransID = value;

				FMChannelFactory<ITransactionProcessor> transactionProcessorClient = new FMChannelFactory<ITransactionProcessor> ( );
				ITransactionProcessor client = transactionProcessorClient.CreateProxy ( );
				transaction = client.Process ( transactionSR );
			}
		}

		public RecirculationForm ( )
		{
			GetSecurity ( );

			InitializeComponent ( );

			security = AppDomain.CurrentDomain.GetData ( "Security" ) as SecurityClass;
			if (security == null)
			{
				throw new Exception ( "Security not in AppDomain" );
			}

			FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
			ISites sites = sitesClient.CreateProxy ( );

			site = sites.Get(security, security.SiteGuid, false, false, false);
			// set the initial controls
			InitializeDialogDisplay ( );

		}

		private void InitializeDialogDisplay ( )
		{
			StartTimeSelection.CustomFormat = "MM/dd/yyyy  -  hh:mm:ss tt";
			StartTimeSelection.ShowCheckBox = false;
			StartTimeSelection.ShowUpDown = true;
			StartTimeSelection.Value = System.DateTime.Now;
			StopTimeSelection.CustomFormat = "MM/dd/yyyy  -  hh:mm:ss tt";
			StopTimeSelection.ShowCheckBox = false;
			StopTimeSelection.ShowUpDown = true;
			StopTimeSelection.Value = System.DateTime.Now;

			// load the available transactions for type 12
			PopulateEquipmentIDDropDownList ( );
			PopulateOperatorDropDownList ( );
			PopulateProductDropDownList ( );
			PopulateTransactionDescriptionDropDownList ( );

			NetVolumeTextBox.Text = "0";
			GrossVolumetextBox.Text = "0";

			// MOD specific functionality (IGO 2010-Sep-15)
			if (TargetCustomer.MOD == base.GetTargetCustomer ( ))
			{
				// Hardcode the transaction type to "Miscellaneous"
				this.TransactionTypeLabel.Text = "Miscellaneous";

				// Always remove the following controls
				this.ServiceBranchLabel.Visible = false;
				this.ServiceBranchtextBox.Visible = false;
				this.CardNumberLabel.Visible = false;
				this.CardNumbertextBox.Visible = false;
				this.IssuePointNumberLabel.Visible = false;
				this.IssuePointNumbertextBox.Visible = false;
				this.IssuePointLabel.Visible = false;
				this.IssuePointtextBox.Visible = false;
				this.SerialNumberLabel.Visible = false;
				this.SerialNumbertextBox.Visible = false;
			}
		}

		private void SetDialogTransactionData ( )
		{
			if (transaction.RouteSchedule.FST != null)
				StartTimeSelection.Value = transaction.RouteSchedule.FST.Value.DateTime;

			if (transaction.TimeEnd != null)
				StopTimeSelection.Value = transaction.TimeEnd.Value.DateTime;

			RegistrationIDDropDownList.SelectedIndex = RegistrationIDDropDownList.FindString ( transaction.SourceEQ1.RegistrationID );

			OperatorDropDownList.SelectedIndex = OperatorDropDownList.FindString ( transaction.OperatorID );

			foreach (LineItemDO lineItemDO in transaction.LineItems)
			{
				if (lineItemDO.Product != null)
				{
					ProductDropDownList.SelectedIndex = ProductDropDownList.FindString ( lineItemDO.Product );
					GrossVolumetextBox.Text = lineItemDO.Quantity.Gross.ToString ( site.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.VOLUME ) );
					NetVolumeTextBox.Text = lineItemDO.Quantity.Net.ToString ( site.GetNumberFormatInfo ( SITE_VARIABLE_TYPE.VOLUME ) );
					break;
				}
			}

			TransactionDescription.SelectedIndex = TransactionDescription.FindString ( (string) transaction.UserData["Transaction Aliases User Data 2"] );

			SerialNumbertextBox.Text = (string) transaction.UserData["Transaction Aliases User Data 4"];
			IssuePointtextBox.Text = (string) transaction.IssuePoint;
			IssuePointNumbertextBox.Text = (string) transaction.IssuePointNumber;
			ServiceBranchtextBox.Text = (string) transaction.UserData["Transaction Aliases User Data 19"];
			CardNumbertextBox.Text = transaction.FuelCardID;
			MemotextBox.Text = transaction.Notes;
		}

		private void PopulateEquipmentIDDropDownList ( )
		{
			List<EquipmentClass> EquipmentCollection = new List<EquipmentClass> ( );

			FMChannelFactory<IEquipments> equipmentsClient = new FMChannelFactory<IEquipments> ( );
			IEquipments Equipments = equipmentsClient.CreateProxy ( );

			RegistrationIDDropDownList.Items.Clear ( );
			EquipmentCollection = Equipments.EnumerateManagedEquipment ( security );
			RegistrationIDDropDownList.DataSource = EquipmentCollection;
		}

		private void PopulateOperatorDropDownList ( )
		{
			List<PersonClass> PersonCollection = new List<PersonClass> ( );

			FMChannelFactory<IPersonnel> personnelClient = new FMChannelFactory<IPersonnel> ( );
			IPersonnel Personnel = personnelClient.CreateProxy ( );

			OperatorDropDownList.Items.Clear ( );
			// operators should be limited to drivers
			PersonCollection = Personnel.EnumerateByRole ( security, PERSON_ROLE.DRIVER_ROLE );
			OperatorDropDownList.DataSource = PersonCollection;
		}

		private void PopulateProductDropDownList ( )
		{
			List<ProductClass> ProductCollection = new List<ProductClass> ( );

			FMChannelFactory<IProducts> productsClient = new FMChannelFactory<IProducts> ( );
			IProducts Products = productsClient.CreateProxy ( );

			ProductDropDownList.Items.Clear ( );
			ProductCollection = Products.Enumerate ( security );
			ProductDropDownList.DataSource = ProductCollection;
		}

		private void PopulateTransactionDescriptionDropDownList ( )
		{
			TransactionDescription.Items.Clear ( );

			TransactionDescription.Items.Add ( "Maintenance" );
			TransactionDescription.Items.Add ( "Quality Control" );
			TransactionDescription.Items.Add ( "Simulation Dry Run" );
			TransactionDescription.Items.Add ( "Hose Pressure Test" );
			TransactionDescription.Items.Add ( "Other" );
			TransactionDescription.SelectedItem = TransactionDescription.Items[0];
		}

		private void OnCloseClicked ( object sender, EventArgs e )
		{
			Close ( );
		}

		private void label5_Click ( object sender, EventArgs e )
		{

		}

		private void OnNetVolumeTextBoxKeyPress ( object sender, KeyPressEventArgs e )
		{
			e.Handled = VerifyNumberIsValid ( e, NetVolumeTextBox );
		}

		private void OnGrossVolumeTextBoxKeyPress ( object sender, KeyPressEventArgs e )
		{
			e.Handled = VerifyNumberIsValid ( e, GrossVolumetextBox );
		}

		private bool VerifyNumberIsValid ( KeyPressEventArgs e, TextBox CurrentTextBox )
		{
			if (char.IsDigit ( e.KeyChar ))
				return false;

			// check for only one decimal point
			if (e.KeyChar == '.' && CurrentTextBox.Text.Contains ( "." ) == false)
				return false;

			if (e.KeyChar == 8) // backspace
				return false;

			return true;
		}

		private void OnCardNumberTextChanged ( object sender, EventArgs e )
		{
		}

		private void OnServiceBranchTextChanged ( object sender, EventArgs e )
		{
		}

		private void OnIssuePointTextChanged ( object sender, EventArgs e )
		{
		}

		private void OnIssuePointNumberTextChanged ( object sender, EventArgs e )
		{
		}

		private void OnSerialNumberTextChanged ( object sender, EventArgs e )
		{
		}

		private void OperatorDropDownList_SelectedIndexChanged ( object sender, EventArgs e )
		{

		}

		private void OnApplyClick ( object sender, EventArgs eArgs )
		{
			try
			{
				if (!VolumesAreGreaterThenZero ( ))
					return;
				SaveTransactionToDataBase ( );
				// remove the data from the designated boxes
				NetVolumeTextBox.Text = "0";
				GrossVolumetextBox.Text = "0";
				ServiceBranchtextBox.Text = "";
				CardNumbertextBox.Text = "";
				IssuePointNumbertextBox.Text = "";
				IssuePointtextBox.Text = "";
				SerialNumbertextBox.Text = "";
			}
			catch (Exception e)
			{
				MessageBox.Show ( e.ToString ( ) );
			}
		}

		private void SaveTransactionToDataBase ( )
		{
			bool NewTransaction = false;
			TransactionDO Transaction = null;
			if (transaction != null)
			{
				Transaction = transaction;
			}
			else
			{
				NewTransaction = true;
				Transaction = new TransactionDO ( );
				Transaction.TransID = FuelsManagerId.NewId ( );
			}
			if (Transaction == null)
				throw new Exception ( "Transaction Creation Failure" );

			SiteTimeConverter timeConverter = new SiteTimeConverter ( site );

			Transaction.TransTypeID = TransactionTypes.T12_InventoryNotAffected;
			Transaction.Site = security.SiteID;
			Transaction.SiteGuid = security.SiteGuid;

			FMChannelFactory<ICompanies> companiesClient = new FMChannelFactory<ICompanies> ( );
			ICompanies Companies = companiesClient.CreateProxy ( );

			CompanyCollectionClass managerCollection = (CompanyCollectionClass) Companies.EnumerateByRole ( security, COMPANY_ROLE.MANAGER, ByGroupCompanies: false, bLocalize: true );
			if (managerCollection.Count == 0)
				throw new Exception ( "No Manager" );

			if (managerCollection.Count > 1)
				throw new Exception ( "Multiple Managers" );

			Transaction.ManagerID = managerCollection[0].ID;
			Transaction.ManagerCode = managerCollection[0].Code;
			Transaction.ManagerCompanyGuid = managerCollection[0].IdentityGuid;

			CompanyCollectionClass ownerCollection = (CompanyCollectionClass) Companies.EnumerateByRole ( security, COMPANY_ROLE.OWNER, ByGroupCompanies: false, bLocalize: true );
			if (ownerCollection.Count == 0)
				throw new Exception ( "No Owner" );

			if (ownerCollection.Count > 1)
				throw new Exception ( "Multiple Owner" );

			Transaction.OwnerID = ownerCollection[0].ID;
			Transaction.OwnerCode = ownerCollection[0].Code;
			Transaction.OwnerCompanyGuid = ownerCollection[0].IdentityGuid;

			Transaction.Status = TransactionStatus.Completed;
			foreach (LineItemDO LineItem in Transaction.LineItems)
			{
				LineItem.Status = TransactionStatus.Completed;
			}

			InventoryDateSR inventoryDateSR = new InventoryDateSR ( );
			inventoryDateSR.Security = Security;
			inventoryDateSR.CurrentSiteGuid = Security.SiteGuid;

			FMChannelFactory<IInventoryDateProcessor> inventoryDateProcessorClient = new FMChannelFactory<IInventoryDateProcessor> ( );
			IInventoryDateProcessor inventoryDateProcessor = inventoryDateProcessorClient.CreateProxy ( );

			InventoryDateDO inventoryDateDO = inventoryDateProcessor.Process ( inventoryDateSR );

			Transaction.InventoryDate = inventoryDateDO.InventoryDate;
			Transaction.TransactionDateTime = timeConverter.Now ( );
			Transaction.OriginApplication = TransactionOrigin.Dispatch;
			Transaction.TimeEnd = StopTimeSelection.Value;
			Transaction.RouteSchedule.FST = StartTimeSelection.Value;

			Transaction.RequestedDateTime = timeConverter.Now ( );

			EquipmentClass Equipment = new EquipmentClass ( );
			Equipment = (EquipmentClass) RegistrationIDDropDownList.SelectedItem;

			Transaction.SourceEQ1.RegistrationID = Equipment.ID;
			Transaction.SourceEQ1.SerialNumber = Equipment.SerialNumber;
			Transaction.SourceEQ1.EquipmentGuid = Equipment.IdentityGuid;
			Transaction.SourceEQ1.EquipmentModel = Equipment.Model;
			Transaction.SourceEQ1.EquipmentType = EquipmentTypeClass.TypeID ( Equipment.Type );
			Transaction.SourceEQ1.CompanyEquipmentID = Equipment.CompanyEquipmentID;

			LineItemDO lineItemDO = null;
			if (NewTransaction == true)
			{
				lineItemDO = new LineItemDO ( );
			}
			else
			{
				foreach (LineItemDO lineItemTrans in transaction.LineItems)
				{
					if (lineItemTrans != null)
					{
						lineItemDO = lineItemTrans;
						break;
					}
				}
			}
			if (lineItemDO == null)
				throw new Exception ( "Invalid Transaction" );

			lineItemDO.Status = TransactionStatus.Completed;

			//			lineItemDO.DestinationEQ.EquipmentModel = Equipment.Model;
			//			lineItemDO.DestinationEQ.EquipmentType = EquipmentTypeClass.TypeID(Equipment.Type);
			//			lineItemDO.DestinationEQ.RegistrationID = Equipment.ID;
			//			lineItemDO.DestinationEQ.SerialNumber = Equipment.SerialNumber;
			//			lineItemDO.DestinationEQ.EquipmentIndex = new VInteger(Equipment.Index);
			//			lineItemDO.DestinationEQ.CompanyEquipmentID = Equipment.CompanyEquipmentID;

			ProductClass Product = (ProductClass) ProductDropDownList.SelectedItem;
			lineItemDO.Product = Product.ID;
			lineItemDO.ProductCode = Product.Code;
			lineItemDO.ProductGuid = Product.IdentityGuid;

			lineItemDO.Quantity = new QuantityDO ( );
			lineItemDO.Quantity.Gross = System.Convert.ToDouble ( GrossVolumetextBox.Text );
			lineItemDO.Quantity.Net = System.Convert.ToDouble ( NetVolumeTextBox.Text );

			if (NewTransaction == true)
				Transaction.LineItems.Add ( lineItemDO );

			Transaction.Notes = MemotextBox.Text;
			Transaction.Number03 = System.Convert.ToDouble ( GrossVolumetextBox.Text );

			Transaction.TransactionDateTime = DateTime.UtcNow;

			PersonClass person = (PersonClass) OperatorDropDownList.SelectedItem;
			Transaction.OperatorID = OperatorDropDownList.Text;
			Transaction.OperatorPersonnelGuid = person.IdentityGuid;

			Transaction.UserData["Transaction Aliases User Data 1"] = "U.S. Gallons";
			Transaction.UserData["Transaction Aliases User Data 2"] = TransactionDescription.Text;
			Transaction.UserData["Transaction Aliases User Data 4"] = SerialNumbertextBox.Text;
			Transaction.IssuePoint = IssuePointtextBox.Text;
			Transaction.IssuePointNumber = IssuePointNumbertextBox.Text;
			Transaction.UserData["Transaction Aliases User Data 19"] = ServiceBranchtextBox.Text;
			Transaction.FuelCardID = CardNumbertextBox.Text;

			FMChannelFactory<ITransactionAliases> transactionAliasesClient = new FMChannelFactory<ITransactionAliases> ( );
			ITransactionAliases aliases = transactionAliasesClient.CreateProxy ( );

			TransactionAliasClass TransactionAlias = new TransactionAliasClass ( );

			TransactionAlias = aliases.Get(security, aliases.GetIdentityGuid(security, TransactionTypeLabel.Text), false);
			Transaction.Alias = TransactionAlias.ID;
			Transaction.TransactionAliasGuid = TransactionAlias.IdentityGuid;
			Transaction.TransTypeID = TransactionAlias.TransTypeID;

			SaveTransaction ( Transaction );

		}

		private void OnSaveAndCloseClicked ( object sender, EventArgs eArgs )
		{
			try
			{
				if (!VolumesAreGreaterThenZero ( ))
					return;
				SaveTransactionToDataBase ( );
				Close ( );
			}
			catch (Exception e)
			{
				MessageBox.Show ( e.ToString ( ) );
			}
		}

		private void OnLoad ( object sender, EventArgs e )
		{
			if (transaction != null)
				SetDialogTransactionData ( );

			SaveandClosebutton.Enabled = Security.HasRight ( RIGHT.MODIFY_DISPATCH );
			// if this is being edited then disable the apply button as requested
			if (transaction != null)
				Applybutton.Enabled = false;
			else
				Applybutton.Enabled = Security.HasRight ( RIGHT.MODIFY_DISPATCH );

		}

		private bool VolumesAreGreaterThenZero ( )
		{
			if (string.IsNullOrEmpty ( NetVolumeTextBox.Text ) ||
				Convert.ToInt32 ( NetVolumeTextBox.Text ) <= 0)
			{
				MessageBox.Show ( "Net Volume Must Be Greater Then 0." );
				NetVolumeTextBox.Focus ( );
				return false;
			}
			if (string.IsNullOrEmpty ( GrossVolumetextBox.Text ) ||
				Convert.ToInt32 ( GrossVolumetextBox.Text ) <= 0)
			{
				MessageBox.Show ( "Gross Volume Must Be Greater Then 0." );
				GrossVolumetextBox.Focus ( );
				return false;
			}
			return true;
		}
	}
}
