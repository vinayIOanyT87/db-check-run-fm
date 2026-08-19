// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelRequestForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// The Fuel Request form is used by dispatch personnel to enter data about fuel service requests
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.Linq;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.ServiceModel;

	using AjaxControlToolkit;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;
	using FMBusinessObjects.Exceptions;

	using FMControls;

    using FMCore;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Represents Session variables that can be accessed by the Fuel Request Form and its various tabs
	/// </summary>
	public static class FuelRequestFormSession
	{
		/// <summary>
		/// Represents the site we're currently logged into
		/// </summary>
		public static SiteClass SessionSite
		{
			get
			{
				if (HttpContext.Current.Session["FuelRequestSite"] is SiteClass)
				{
					return HttpContext.Current.Session["FuelRequestSite"] as SiteClass;
				}
				
				return new SiteClass();
			}

			set
			{
				HttpContext.Current.Session.Add("FuelRequestSite", value);
			}
		}

		/// <summary>
		/// Represents the transaction alias that corresponds to the type of request
		/// </summary>
		public static TransactionAliasClass SessionTransactionAlias
		{
			get
			{
				if (HttpContext.Current.Session["FuelRequestTransactionAlias"] is TransactionAliasClass)
				{
					return HttpContext.Current.Session["FuelRequestTransactionAlias"] as TransactionAliasClass;
				}
				
				return new TransactionAliasClass();
			}

			set
			{
				HttpContext.Current.Session.Add("FuelRequestTransactionAlias", value);
			}
		}

		/// <summary>
		/// Represents the Dispatch Configuration, which tells us things like whether certain times (start, stop) should be ignored
		/// </summary>
		public static DispatchConfigurationClass SessionDispatchConfiguration
		{
			get
			{
				if (HttpContext.Current.Session["FuelRequestDispatchConfiguration"] is DispatchConfigurationClass)
				{
					return HttpContext.Current.Session["FuelRequestDispatchConfiguration"] as DispatchConfigurationClass;
				}
				
				return new DispatchConfigurationClass();
			}

			set
			{
				HttpContext.Current.Session.Add("FuelRequestDispatchConfiguration", value);
			}
		}

		/// <summary>
		/// Represents the transaction that we loaded or created when the form opened
		/// </summary>
		public static TransactionDO SessionTransaction
		{
			get
			{
				if (HttpContext.Current.Session["FuelRequestTransaction"] is TransactionDO)
				{
					return HttpContext.Current.Session["FuelRequestTransaction"] as TransactionDO;
				}
				
				return new TransactionDO { SubmittedToAccounting = false };
			}

			set
			{
				HttpContext.Current.Session.Add("FuelRequestTransaction", value);
			}
		}

		/// <summary>
		/// Represents the request subtype selected (e.g. Refuel, defuel, fill, return to bulk)
		/// </summary>
		public static string SessionFuelRequestSubType
		{
			get
			{
				if (HttpContext.Current.Session["FuelRequestSubType"] is string)
				{
					return HttpContext.Current.Session["FuelRequestSubType"] as string;
				}
				
				return string.Empty;
			}

			set
			{
				HttpContext.Current.Session.Add("FuelRequestSubType", value);
			}
		}

		/// <summary>
		/// True if the user requested to complete the transaction 
		/// </summary>
		public static bool SessionCompletingTransaction
		{
			get
			{
				if (HttpContext.Current.Session["FuelRequestCompletingTransaction"] is bool)
				{
					return (bool)HttpContext.Current.Session["FuelRequestCompletingTransaction"];
				}
				
				return false;
			}

			set
			{
				HttpContext.Current.Session.Add("FuelRequestCompletingTransaction", value);
			}
		}
	}

	/// <summary>
	/// A base class which all tabs that are on the Fuel Request Form inherit from.
	/// The base class contains methods and properties that are used by more than one tab page.
	/// </summary>
	public class FuelRequestFormPageBase : FMUserControlBase
	{
		#region Page Properties

		/// <summary>
		/// Returns the form that contains the user control. This is used to access methods and properties
		/// on the parent form as well as on other tabs
		/// </summary>
		public FuelRequestForm ParentForm
		{
			get
			{
				return GetParentForm(this);
			}
		}
		#endregion

		#region Page Methods
		/// <summary>
		/// Walk up the hierarchy of controls until we find the fuel request form
		/// </summary>
		/// <param name="control">The current control</param>
		/// <returns>The parent Fuel Request Form. An exception will be thrown if it's not found.</returns>
		private static FuelRequestForm GetParentForm(Control control)
		{
			while (control != null)
			{
				if (control is FuelRequestForm)
				{
					return control as FuelRequestForm;
				}

				control = control.Parent;
			}
			
			throw new Exception("Could not locate the parent fuel request form");
		}

		/// <summary>
		/// Used to load equipment data from the equipment enumeration results since it returns a data set
		/// </summary>
		/// <param name="set">Contains data returned from enumerating equipment</param>
		/// <param name="equipmentCollection">Will be populated with equipment records</param>
		public void LoadEquipment(DataSet set, EquipmentCollectionClass equipmentCollection)
		{
			if (set != null && set.Tables.Count != 0)
			{
				foreach (DataRow row in set.Tables[0].Rows)
				{
					EquipmentClass equipment = new EquipmentClass(FuelRequestFormSession.SessionSite)
					                           {
						                           IdentityGuid = DataObject.getValue<Guid>(row["EquipmentGuid"], Guid.Empty),
						                           MasterRecordGuid = DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty),
						                           SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty),
						                           ID = DataObject.getValue<string>(row["ID"], string.Empty),
						                           Xref = DataObject.getValue<string>(row["Xref"], string.Empty),
						                           FuelingType = (FUELING_TYPES)DataObject.getValue<short>(row["FuelingType"], 0)
					                           };

					equipmentCollection.Add(equipment);
				}
			}
		}

		/// <summary>
		/// Add a blank entry to a combo box to represent no selection
		/// </summary>
		/// <param name="comboBox">The combo box to add a blank entry to</param>
		/// <param name="hasGuidValue">Indicates whether the blank entry should have a value of Guid.empty or the empty string. 
		/// If true, it will be Guid.Empty. Defaults to true.</param>
		public void AddBlankComboBoxEntry(FMComboBox comboBox, bool hasGuidValue = true)
		{
			if (hasGuidValue)
			{
				comboBox.Items.Insert(0, new ListItem(string.Empty, Guid.Empty.ToString()));
			}
			else
			{
				comboBox.Items.Insert(0, new ListItem(string.Empty, string.Empty));
			}
		}
		#endregion

		#region Page Events
		/// <summary>
		/// When the user adds a new item to a combo box by typing in a value (and not selecting an existing one),
		/// set the focus on the control so we don't lose it during postback
		/// </summary>
		/// <param name="sender">The control that had an item inserted</param>
		/// <param name="e">The parameter is not used</param>
		protected void ComboBoxItemInserted(object sender, ComboBoxItemInsertEventArgs e)
		{
			try
			{
				if (sender is FMComboBox)
				{
					FMComboBox comboBox = sender as FMComboBox;
					GetParentForm(this).SetFocusOnControl(comboBox);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion
	}

	/// <summary>
	/// The Fuel Request form is used by dispatch personnel to enter data about fuel service requests
	/// </summary>
	public partial class FuelRequestForm : FMFormBase
	{
		#region Constants
		// These constants represent the different form titles displayed. There's one for each type of request, 
		// and a couple more used when the user wants to complete an existing request
		private const string FuelRequestTitle = "Fuel Request";
		private const string FillStandRequestTitle = "Fill Stand Request";
		private const string TransientFuelRequestTitle = "Transient Fuel Request";
		private const string FastLogFuelRequestTitle = "Fast Log Fuel Request";
		private const string FastLogFillStandRequestTitle = "Fast Log Fill Stand Request";
		private const string FuelRequestCompletionTitle = "Fuel Request Completion";
		private const string FillStandRequestCompletionTitle = "Fill Stand Completion";

		// Constants for page request parameters that can be provided by whoever wants to display the dialog
		public const string TransactionGuidRequestParameterName = "TransactionGuid";
		public const string TransientRequestParameterName = "Transient";
		public const string FillStandRequestParameterName = "FillStand";
		public const string FastLogRequestParameterName = "FastLog";
		public const string FastLogFillStandRequestParameterName = "FastLogFillStand";
		public const string CompletionModeRequestParameterName = "CompletionMode";

		// Constants for the types of transactions we create. 
		public const TransactionTypes RefuelTransactionType = TransactionTypes.T6_SecondaryDisbursement;
		public const TransactionTypes DefuelTransactionType = TransactionTypes.T4_SecondaryDefuel;
		public const TransactionTypes FillStandTransactionType = TransactionTypes.T7_FillStand;
		public const TransactionTypes ReturnToBulkTransactionType = TransactionTypes.T10_Unload;

		// Constants for aliases
		public const string FuelRequestTransactionAlias = "Sale";
		public const string DefuelRequestTransactionAlias = "Defuel";
		#endregion

		/// <summary>
		/// The type of fuel request which corresponds to the current transaction
		/// </summary>
		private FuelRequestType requestType = FuelRequestType.Unknown;

		#region Session Properties
		/// <summary>
		/// Used to display a message if we are re-opening the form
		/// to display a new transaction instead of closing it.
		/// </summary>
		public string SessionAlertMessage
		{
			get
			{
				if (this.Session["FuelRequestAlertMessage"] is string)
				{
					return this.Session["FuelRequestAlertMessage"] as string;
				}
				
				return string.Empty;
			}

			set
			{
				this.Session.Add("FuelRequestAlertMessage", value);
			}
		}
		#endregion

		#region Form Properties
		/// <summary>
		/// Represents the product (aka Grade) selected on the Service Request tab.
		/// </summary>
		public Guid SelectedProduct
		{
			get
			{
				return this.IsFuelRequest ? this.FuelRequestServiceRequestPage.SelectedProduct : this.FuelRequestFillStandServiceRequestPage.SelectedProduct;
			}
		}

		/// <summary>
		/// Represents the product (aka Grade) selected on the Service Request tab.
		/// </summary>
		public bool RequestCancelled
		{
			get
			{
				return this.IsFuelRequest ? this.FuelRequestServiceRequestPage.RequestCancelled : this.FuelRequestFillStandServiceRequestPage.RequestCancelled;
			}
		}

		/// <summary>
		/// The Issue Point Number displayed on the Additional Data tab
		/// </summary>
		public string IssuePointNumber
		{
			get
			{
				return this.FuelRequestAdditionalDataPage.IssuePointNumber;
			}

			set
			{
				this.FuelRequestAdditionalDataPage.IssuePointNumber = value;
			}
		}

		/// <summary>
		/// The Issue Point displayed on the Additional Data tab
		/// </summary>
		public string IssuePoint
		{
			get
			{
				return this.FuelRequestAdditionalDataPage.IssuePoint;
			}

			set
			{
				this.FuelRequestAdditionalDataPage.IssuePoint = value;
			}
		}

		/// <summary>
		/// Represents the Registration ID combo box on the Detail tab 
		/// </summary>
		public FMComboBox DetailRegistrationIDComboBox
		{
			get
			{
				return this.FuelRequestDetailPage.DetailRegistrationIDComboBox;
			}
		}

		/// <summary>
		/// If the form should be read only, disable the buttons so users can't save things
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
		/// True if this is a Regular (refuel or defuel) request, a Transient request, or a Fast Log request
		/// </summary>
		public bool IsFuelRequest
		{
			get
			{
				if (this.requestType == FuelRequestType.Unknown)
				{
					throw new Exception("Fuel Request Type is not set");
				}

				return this.requestType != FuelRequestType.FillStand && this.requestType != FuelRequestType.FastLogFillStand;
			}
		}

		/// <summary>
		/// True if this either one of the Fast log requests (Fast Log or Fast Log Fill Stand)
		/// </summary>
		public bool IsFastLogOrFastLogFillStand
		{
			get
			{
				if (this.requestType == FuelRequestType.Unknown)
				{
					throw new Exception("Fuel Request Type is not set");
				}

				return this.requestType == FuelRequestType.FastLog || this.requestType == FuelRequestType.FastLogFillStand;
			}
		}

		/// <summary>
		/// The type of fuel request which corresponds to the current transaction
		/// </summary>
		public FuelRequestType RequestType
		{
			get { return this.requestType; }
		}

		/// <summary>
		/// This is used to udpate the quantity text box on the Fuel Request Detail form.
		/// </summary>
		public string UpdateQuantityOnDetailForm
		{
			set { this.FuelRequestDetailPage.Quantity = value; }
		}

		public bool QuantityEnabledOnDetailForm
		{
			set { this.FuelRequestDetailPage.QuantityEnabled = value; }
		}
		#endregion

		#region Form Dialog Display and Behavior
		/// <summary>
		/// Show a dialog to the user with yes and no buttons. If the user says yes, click the button provided.
		/// </summary>
		/// <param name="confirmationMessage">The message to show</param>
		/// <param name="button">The button to click if the user says yes</param>
		public void ShowConfirmationDialog(string confirmationMessage, FMButton button)
		{
			ScriptManager.RegisterStartupScript(this, this.GetType(), "ConfirmDialog", "ShowConfirmationDialogAndClickButton('" + HttpUtility.JavaScriptStringEncode(confirmationMessage) + "','" + button.ID + "');", true);
		}

		/// <summary>
		/// Show a message to the user on top of the form displaying the specified text.
		/// </summary>
		/// <param name="alertMessage">The message to show</param>
		public void ShowAlert(string alertMessage)
		{
			ScriptManager.RegisterStartupScript(this, this.GetType(), "AlertScript", "ShowAlertDialog('" + HttpUtility.JavaScriptStringEncode(alertMessage) + "');", true);
		}

		/// <summary>
		/// Set the focus on the specified control, also changing tabs if necessary.
		/// You must provide the index of the tab the control is on
		/// </summary>
		/// <param name="control">The control to set focus on</param>
		public void SetFocusOnControl(Control control)
		{
			int tabIndex = this.GetTabIndex(control);

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
				this, this.GetType(), "FocusScript", "SetFocus('" + HttpUtility.JavaScriptStringEncode(controlID) + "','" + HttpUtility.JavaScriptStringEncode(tabIndex.ToString(CultureInfo.InvariantCulture)) + "');", true);
		}

		/// <summary>
		/// Determine the tab index that the specified control is on.
		/// This allows us to avoid hard-coding tab indexes
		/// </summary>
		/// <param name="control">The control to find the tab index of</param>
		/// <returns>The tab index that the specified control is on. Will throw an exception if the tab page index is not found</returns>
		private int GetTabIndex(Control control)
		{
			if (control == null)
			{
				throw new Exception("Could not determine the tab index of the current tab page");
			}

			if (control is TabPanel)
			{
				int tabIndex = this.tcFuelRequest.Tabs.IndexOf(control);

				if (tabIndex == -1)
				{
					throw new Exception("Could not determine the tab index of the current tab page");
				}

				return tabIndex;
			}
			
			return this.GetTabIndex(control.Parent);
		}
		#endregion

		#region Form Methods
		/// <summary>
		/// Retrieve the site we're logged into, we use it primarily for time conversion
		/// </summary>
		private void LoadSite()
		{
			FuelRequestFormSession.SessionSite = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));
		}

		/// <summary>
		/// Retrieve the dispatch configuration, which tells us things like whether certain times (start, stop) should be ignored
		/// </summary>
		private void LoadDispatchConfiguration()
		{
			FuelRequestFormSession.SessionDispatchConfiguration = FMChannelHelper.MakeCall<IDispatchConfigurations, DispatchConfigurationClass>(dispatchConfigurations =>
			{
				bool entityAssigned;
				Guid dispatchConfigurationGuid = dispatchConfigurations.GetIdentityGuidBySiteIdAndAssigned(
						this.Security, this.Security.SiteGuid, DispatchConfigurationClass.DefaultId, true, out entityAssigned);

				if (dispatchConfigurationGuid != Guid.Empty)
				{
					return dispatchConfigurations.Get(this.Security, dispatchConfigurationGuid);
				}

				return new DispatchConfigurationClass();
			});
		}

		/// <summary>
		/// Set the form into the appropriate request mode (Fast Log, Transient, etc)
		/// by looking at the page request parameter provided which indicates the type of fuel request 
		/// </summary>
		private void DetermineRequestType()
		{
			FuelRequestFormSession.SessionCompletingTransaction = false;

			bool isFillStand = false;

			if (this.Request.GetQueryOrFormValue(TransientRequestParameterName) != null
				&& this.Request.GetQueryOrFormValue(TransientRequestParameterName).ToLower() == bool.TrueString.ToLower())
			{
				this.titleLabel.Text = TransientFuelRequestTitle;
				this.requestType = FuelRequestType.Transient;
			}
			else if (this.Request.GetQueryOrFormValue(FillStandRequestParameterName) != null
				&& this.Request.GetQueryOrFormValue(FillStandRequestParameterName).ToLower() == bool.TrueString.ToLower())
			{
				isFillStand = true;
				this.titleLabel.Text = FillStandRequestTitle;
				this.requestType = FuelRequestType.FillStand;
			}
			else if (this.Request.GetQueryOrFormValue(FastLogRequestParameterName) != null
				&& this.Request.GetQueryOrFormValue(FastLogRequestParameterName).ToLower() == bool.TrueString.ToLower())
			{
				this.titleLabel.Text = FastLogFuelRequestTitle;
				this.requestType = FuelRequestType.FastLog;
			}
			else if (this.Request.GetQueryOrFormValue(FastLogFillStandRequestParameterName) != null
				&& this.Request.GetQueryOrFormValue(FastLogFillStandRequestParameterName).ToLower() == bool.TrueString.ToLower())
			{
				isFillStand = true;
				this.titleLabel.Text = FastLogFillStandRequestTitle;
				this.requestType = FuelRequestType.FastLogFillStand;
			}
			else
			{
				this.titleLabel.Text = FuelRequestTitle;
				this.requestType = FuelRequestType.RequestFuel;
			}

			// Determine if the user wants to complete the transaction. We'll later set the transaction status to completed if this is the case.
			if (this.Request.GetQueryOrFormValue(CompletionModeRequestParameterName) != null
				&& this.Request.GetQueryOrFormValue(CompletionModeRequestParameterName).ToLower() == bool.TrueString.ToLower())
			{
				FuelRequestFormSession.SessionCompletingTransaction = true;

				this.Title = this.IsFuelRequest ? FuelRequestCompletionTitle : FillStandRequestCompletionTitle;
			}

			// Use either the regular service request page or the fill stand service request page depending on the 
			// type of request. Additionally, the Contact tab is not visible for fill stand requests
			if (isFillStand)
			{
				if (this.tcFuelRequest.Tabs.Contains(this.tpServiceRequestPage))
				{
					this.tcFuelRequest.Tabs.Remove(this.tpServiceRequestPage);
				}

				if (this.tcFuelRequest.Tabs.Contains(this.tpContactPage))
				{
					this.tcFuelRequest.Tabs.Remove(this.tpContactPage);
				}
			}
			else
			{
				if (this.tcFuelRequest.Tabs.Contains(this.tpFillStandServiceRequestPage))
				{
					this.tcFuelRequest.Tabs.Remove(this.tpFillStandServiceRequestPage);
				}
			}
		}

		/// <summary>
		/// Use the request subtype selected on the Service Request tab to determine which transaction alias
		/// to use
		/// </summary>
		/// <param name="requestSubType">The request sub type selected on the Service Request tab, e.g. Defuel or Fill</param>
		public void DetermineTransactionAlias(string requestSubType)
		{
			try
			{
				TransactionTypes transactionType = TransactionTypes.T_Maximum;

				if (requestSubType == FuelRequestSR.ReturnToBulkRequestSubType || requestSubType == FuelRequestSR.PartialReturnToBulkSubType)
				{
					transactionType = ReturnToBulkTransactionType;
				}
				else if (requestSubType == FuelRequestSR.FillRequestSubType
					|| requestSubType == FuelRequestSR.PartialFillRequestSubType)
				{
					transactionType = FillStandTransactionType;
				}
				else if (requestSubType == FuelRequestSR.DefuelRequestSubType)
				{
					transactionType = DefuelTransactionType;
				}
				else if (requestSubType == FuelRequestSR.RefuelRequestSubType)
				{
					transactionType = RefuelTransactionType;
				}
				else
				{
					throw new Exception("Unknown Request Type: " + requestSubType);
				}

				// Get all of the transaction aliases that are configured for Dispatch
				TransactionAliasNameCollectionClass transactionAliases = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
					transactionAliasesServiceClass => transactionAliasesServiceClass.EnumerateDispatchAliasNames(this.Security));

				// Find the transaction alias which has a transaction alias type which corresponds to the request type specified on the form
				TransactionAliasNameClass matchingAliasName = transactionAliases.Find(aliasName => aliasName.TransTypeID == transactionType);

				// If we found a match, get the entire transaction alias.
				// We need the entire alias because it contains the user data field configuration, which determines the fields displayed
				// on the Additional Data tab.
				if (matchingAliasName != null)
				{
					TransactionAliasClass transactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
						transactionAliasesServiceClass => transactionAliasesServiceClass.Get(this.Security, matchingAliasName.IdentityGuid, false));

					if (transactionAlias == null)
					{
						throw new Exception("Could not determine the transaction alias to use for the corresponding request type: " + requestSubType);
					}

					FuelRequestFormSession.SessionTransactionAlias = transactionAlias;
					FuelRequestFormSession.SessionFuelRequestSubType = requestSubType;

					// Since the fields that appear on the Additional Data tab depend on the transaction alias, 
					// reconfigure them now
					this.FuelRequestAdditionalDataPage.SetUserDataControls();
				}
				else
				{
					// We couldn't find a transaction alias configured for dispatch with the correponding transaction alias type
					throw new Exception("Could not determine the transaction alias to use for the corresponding request type: " + requestSubType);
				}
			}
			catch (Exception)
			{
				this.ReadOnly = true;
				throw;
			}
		}

		/// <summary>
		/// Populate the Registration ID on the Detail tab, and also the 
		/// Registration ID on the Fill Stand Service Request tab.
		/// This is called from other tabs because we sometimes filter the 
		/// vehicles available in the Registration ID based on values input on other tabs
		/// </summary>
		public void PopulateRegistrationIDComboBoxes()
		{
			if (!this.IsFuelRequest)
			{
				this.FuelRequestFillStandServiceRequestPage.PopulateRegistrationID();
			}

			this.FuelRequestDetailPage.PopulateRegistrationID();
		}

		/// <summary>
		/// Update the variance displayed on the Detail tab for Fill Stand requests.
		/// The variance depends on the equipment records involved in the transaction
		/// </summary>
		/// <param name="transaction">Contains transaction data used when calculating the variance</param>
		public bool UpdateVariance(TransactionDO transaction)
		{
			return this.FuelRequestDetailPage.UpdateVariance(transaction);
		}

		#endregion

		#region Form Events

		/// <summary>
		/// When the form is initialized, determine the type of request (transient, fast log, etc).
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.DetermineRequestType();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when the page loads. 
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Page.IsPostBack == false)
				{
					this.ReadOnly = !this.Security.HasRight(RIGHT.MODIFY_DISPATCH);

					FuelRequestFormSession.SessionTransaction = null;
					FuelRequestFormSession.SessionTransactionAlias = null;
					FuelRequestFormSession.SessionFuelRequestSubType = string.Empty;

					// If we need to display a variance warning, do so now.
					// The user may have pressed the apply button, in which case 
					// We redirect to this page. The warning hangs out in session.
					// If the user hit OK the message will be displayed before the form is closed.
					if (!string.IsNullOrEmpty(this.SessionAlertMessage))
					{
						this.ShowAlert(this.SessionAlertMessage);
						this.SessionAlertMessage = string.Empty;
					}

					this.LoadSite();
					this.LoadDispatchConfiguration();

					string requestSubType;

					// If the user provided a transaction identifier, load that transaction
					// Otherwise, create a new one
					if (this.Request.GetQueryOrFormValue(TransactionGuidRequestParameterName) != null)
					{
						Guid transactionGuid;
						Guid.TryParse(this.Request.GetQueryOrFormValue(TransactionGuidRequestParameterName), out transactionGuid);

						TransactionSR sr = new TransactionSR
							                   {
								                   Security = this.Security,
								                   TransactionGuid = transactionGuid,
								                   AllowCrossSiteTransactions = true,
								                   ConvertUnits = true
							                   };

						TransactionDO matchingTransaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(processor => processor.Process(sr));

						if (matchingTransaction != null)
						{
							FuelRequestFormSession.SessionTransaction = matchingTransaction;
						}
						else
						{
							throw new Exception("Could not find the transaction identified by the transaction guid: " + transactionGuid);
						}

						if (matchingTransaction.Number02 != null)
						{
							this.requestType = (FuelRequestType) Convert.ToInt32(matchingTransaction.Number02.Value);
						}

						// Use the transaction type to determine the request sub type (Refuel, defuel, etc).
						if (this.IsFuelRequest)
						{
							if (matchingTransaction.TransTypeID == RefuelTransactionType)
							{
								requestSubType = FuelRequestSR.RefuelRequestSubType;
							}
							else if (matchingTransaction.TransTypeID == DefuelTransactionType)
							{
								requestSubType = FuelRequestSR.DefuelRequestSubType;
							}
							else
							{
								throw new Exception("Unrecognized Transaction Alias");
							}
						}
						else
						{
							if (matchingTransaction.TransTypeID == FillStandTransactionType)
							{
								LineItemDO lineItem = matchingTransaction.LineItems.Find(matchingLineItem => matchingLineItem.DeleteFlag == false);

								if (lineItem != null && lineItem.PartialFill.HasValue && lineItem.PartialFill.Value)
								{
									requestSubType = FuelRequestSR.PartialFillRequestSubType;
								}
								else
								{
									requestSubType = FuelRequestSR.FillRequestSubType;
								}
							}
							else if (matchingTransaction.TransTypeID == ReturnToBulkTransactionType)
							{
								requestSubType = FuelRequestSR.ReturnToBulkRequestSubType;
							}
							else
							{
								throw new Exception("Unrecognized Transaction Alias");
							}
						}
					}
					else
					{
						TransactionDO newTransaction = new TransactionDO { SubmittedToAccounting = false };
						newTransaction.LineItems.Add(new LineItemDO());

						newTransaction.Site = this.Security.SiteID;
						newTransaction.SiteGuid = this.Security.SiteGuid;

						if (this.requestType == FuelRequestType.RequestFuel
							|| this.requestType == FuelRequestType.Transient
							|| this.requestType == FuelRequestType.FillStand)
						{
							newTransaction.Status = TransactionStatus.Requested;

							foreach (LineItemDO lineItem in newTransaction.LineItems)
							{
								lineItem.Status = TransactionStatus.Requested;
							}
						}
						else
						{
							newTransaction.Status = TransactionStatus.Completed;

							foreach (LineItemDO lineItem in newTransaction.LineItems)
							{
								lineItem.Status = TransactionStatus.Completed;
							}
						}

						// Set the request sub type to the intial value depending on the type of request
						requestSubType = this.IsFuelRequest ? FuelRequestSR.RefuelRequestSubType : FuelRequestSR.FillRequestSubType;

						FuelRequestFormSession.SessionTransaction = newTransaction;
					}

					this.DetermineTransactionAlias(requestSubType);

					this.DisplayTransaction(FuelRequestFormSession.SessionTransaction);
				}
				else
				{
					if (FuelRequestFormSession.SessionTransaction != null)
					{
						TransactionDO existingTransactionDO = FuelRequestFormSession.SessionTransaction;

						if (existingTransactionDO.Number02 != null)
						{
							this.requestType = (FuelRequestType) Convert.ToInt32(existingTransactionDO.Number02.Value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion

		#region Form Button Control Events
		/// <summary>
		/// Save the changes and close the window.
		/// Since we may show a confirmation dialog, the actual save is done by the HiddenOKButton
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				TransactionDO transaction = FuelRequestFormSession.SessionTransaction;

				if (this.ValidateTransactionData(transaction))
				{
					bool showConfirmFuelAdditiveDialog = this.ShowConfirmFuelAdditiveDialog();
					bool showConfirmUseLocationDialog = this.ShowConfirmOutOfServiceLocationDialog();
					bool showCancellationWarningDialog = this.ShowCancellationWarningDialog(transaction);

					if (showCancellationWarningDialog || showConfirmFuelAdditiveDialog|| showConfirmUseLocationDialog)
					{
						if (showCancellationWarningDialog)
						{
							this.ShowConfirmationDialog("Once an operation is canceled it cannot be uncanceled. Are you sure you want to cancel this job?", this.HiddenOKButton);
						}
						else if (showConfirmUseLocationDialog)
						{
							this.ShowConfirmationDialog("This location is out of service.  Continue anyway?", this.HiddenOKButton);
						}
						else
						{
							this.ShowConfirmationDialog("The Aircraft requires a fuel additive but the Refueler does not have the additive. Create the transaction anyway?", this.HiddenOKButton);
						}
					}
					else
					{
						this.HiddenOkButtonClick(sender, e);
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Save the changes and clear the window
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void ApplyButtonClick(object sender, EventArgs e)
		{
			try
			{
				TransactionDO transaction = FuelRequestFormSession.SessionTransaction;

				if (this.ValidateTransactionData(transaction))
				{
					bool showConfirmFuelAdditiveDialog = this.ShowConfirmFuelAdditiveDialog();
					bool showConfirmUseLocationDialog = this.ShowConfirmOutOfServiceLocationDialog();
					bool showCancellationWarningDialog = this.ShowCancellationWarningDialog(transaction);

					if (showCancellationWarningDialog || showConfirmFuelAdditiveDialog || showConfirmUseLocationDialog)
					{
						// Since the confirm fuel additive dialog only shows for fast logs, it's not possible that we need to show both dialogs
						// because fast logs aren't cancelled.
						if (showCancellationWarningDialog)
						{
							this.ShowConfirmationDialog("Once an operation is canceled it cannot be un-canceled. Are you sure you want to cancel this job?", this.HiddenApplyButton);
						}
						else if (showConfirmUseLocationDialog)
						{
							this.ShowConfirmationDialog("This location is out of service.  Continue anyway?", this.HiddenApplyButton);
						}
						else
						{
							this.ShowConfirmationDialog("The Aircraft requires a fuel additive but the Refueler does not have the additive. Create the transaction anyway?", this.HiddenApplyButton);
						}
					}
					else
					{
						this.HiddenApplyButtonClick(sender, e);
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// The hidden OK button is what actually saves the transaction and closes the form after the user presses the OK button.
		/// We use a hidden button so that we can issue client-side confirmation dialogs, and if the user presses Yes on the confirmation dialog, 
		/// click the hidden button through javascript
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void HiddenOkButtonClick(object sender, EventArgs e)
		{
			try
			{
				// Save the Transaction
				this.SaveTransactionData(FuelRequestFormSession.SessionTransaction);

				string alertToShowBeforeWindowClose = this.SessionAlertMessage;

				this.SessionAlertMessage = string.Empty;

				// Close the form. Also returns a value of 1 to tell tabularView.js that the transaction was completed
				if (!string.IsNullOrEmpty(alertToShowBeforeWindowClose))
				{
					this.ClientScript.RegisterStartupScript(this.GetType(), 
						"CloseScript", 
						"window.returnValue='1';ShowAlertAndClose('" + HttpUtility.JavaScriptStringEncode(alertToShowBeforeWindowClose) + "', '');", true);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(this.GetType(), "CloseScript", "window.returnValue='1';window.close();", true);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// The hidden apply button is what actually saves the transaction and opens the next transaction after the user presses the apply button
		/// We use a hidden button so that we can issue client-side confirmation dialogs, and if the user presses Yes on the confirmation dialog, 
		/// click the hidden button through javascript
		/// </summary>
		/// <param name="sender">The parameter is not used.</param>
		/// <param name="e">The parameter is not used.</param>
		protected void HiddenApplyButtonClick(object sender, EventArgs e)
		{
			try
			{
				// Save
				this.SaveTransactionData(FuelRequestFormSession.SessionTransaction);

				string redirectUrl = "FuelRequestForm.aspx";

				// Clear the controls by reloading the form
				switch (this.requestType)
				{
					case FuelRequestType.FastLog:
						redirectUrl += "?" + FastLogRequestParameterName + "=true";
						break;
					case FuelRequestType.FastLogFillStand:
						redirectUrl += "?" + FastLogFillStandRequestParameterName + "=true";
						break;
					case FuelRequestType.Transient:
						redirectUrl += "?" + TransientRequestParameterName + "=true";
						break;
					case FuelRequestType.FillStand:
						redirectUrl += "?" + FillStandRequestParameterName + "=true";
						break;
				}

				// Since the Service Completion was selected, the data on the page
				// should remain. Therefore, do not redirect.
				if (FuelRequestFormSession.SessionCompletingTransaction)
				{
					return;
				}

				this.Redirect(redirectUrl);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}
		#endregion

		#region Transaction Record Display and Creation
		/// <summary>
		/// Display the form using data from a FuelsManager transaction record.
		/// </summary>
		/// <param name="transaction">The transaction record to display</param>
		private void DisplayTransaction(TransactionDO transaction)
		{
			// If the transaction has been submitted to accounting it can't be modified
			bool formIsReadOnly = transaction.SubmittedToAccounting == true 
								|| transaction.Status == TransactionStatus.Cancelled 
								|| transaction.Status == TransactionStatus.Posted;

			// If the site does not own the transaction it can't be modified
			formIsReadOnly = formIsReadOnly || (transaction.SiteGuid != this.Security.SiteGuid);

			if (formIsReadOnly)
			{
				this.ReadOnly = true;
			}
		}

		/// <summary>
		/// Set values in a FuelsManager transaction record using data from the tabs on the form
		/// </summary>
		/// <param name="transaction">The FuelsManager Transaction record to populate with data</param>
		private void SaveTransactionData(TransactionDO transaction)
		{
			try
			{
				SiteClass site = FuelRequestFormSession.SessionSite;
				TransactionAliasClass transactionAlias = FuelRequestFormSession.SessionTransactionAlias;

				DateTimeOffset siteDateTime = TimeConverter.Now(site);
				string originalTransactionNotes = transaction.Notes;
				bool transactionOriginallyCompleted = false;

				transaction.Alias = transactionAlias.ID;
				transaction.TransTypeID = transactionAlias.TransTypeID;
				transaction.TransactionAliasGuid = transactionAlias.MasterRecordGuid;

				if (transaction.TransactionGuid == Guid.Empty)
				{
					var documentNumberGenerator = new DocumentNumberGenerator(this.Security);
					transaction.DocumentNumber = documentNumberGenerator.GetNextDocumentNumber(transactionAlias.TransTypeID);

					transaction.InventoryDate = siteDateTime.DateTime;
					transaction.TransactionDateTime = siteDateTime;
					transaction.OriginApplication = this.DetermineOriginApplication();

					transaction.RequestedDateTime = siteDateTime;

					if (this.IsFastLogOrFastLogFillStand || FuelRequestFormSession.SessionCompletingTransaction)
					{
						transaction.TimeOut = siteDateTime;
					}

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
														managerCollection.Count,
														managerNames);

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

						string errorMsg = string.Format("Multiple owners are not allowed. {0} owners were found. They are {1}.",
														ownerCollection.Count,
														ownerNames);

						throw new Exception(errorMsg);
					}

					transaction.OwnerID = ownerCollection[0].ID;
					transaction.OwnerCode = ownerCollection[0].Code;
					transaction.OwnerCompanyGuid = ownerCollection[0].MasterRecordGuid;

					transaction.Number02 = (double)this.requestType;

					transaction.TransID = FuelsManagerId.NewId();
				}
				else if (transaction.Status == TransactionStatus.Completed)
				{
					transactionOriginallyCompleted = true;
				}

				if (FuelRequestFormSession.SessionCompletingTransaction)
				{
					transaction.Status = TransactionStatus.Completed;

					foreach (LineItemDO lineItemToComplete in transaction.LineItems)
					{
						lineItemToComplete.Status = TransactionStatus.Completed;
					}
				}

				ProductClass selectedProduct;

				if (this.IsFuelRequest)
				{
					this.FuelRequestServiceRequestPage.SaveTransactionData(transaction, out selectedProduct);
					this.FuelRequestDetailPage.SaveTransactionData(transaction);
					this.FuelRequestAdditionalDataPage.SaveTransactionData(transaction);
					this.FuelRequestContactPage.SaveTransactionData(transaction);
				}
				else
				{
					this.FuelRequestFillStandServiceRequestPage.SaveTransactionData(transaction, out selectedProduct);
					this.FuelRequestDetailPage.SaveTransactionData(transaction);
					this.FuelRequestAdditionalDataPage.SaveTransactionData(transaction);
				}

				int currentConsecutiveOosVariance = 0;

				if (!this.IsFuelRequest && !transactionOriginallyCompleted)
				{
					this.FuelRequestFillStandServiceRequestPage.DisplayVarianceWarning( transaction,
																						originalTransactionNotes,
																						out currentConsecutiveOosVariance);
				}

				LineItemDO lineItem = transaction.LineItems.Find((matchingLineItem) => matchingLineItem.DeleteFlag == false);

				UnitsHelperClass unitsHelper = new UnitsHelperClass(this.Security, site, transactionAlias, selectedProduct);
				unitsHelper.SetUnits(transaction, ProductType.ComponentProduct);
				unitsHelper.SetUnits(lineItem, ProductType.ComponentProduct, selectedProduct);
				
				FuelRequestSR serviceRequest = new FuelRequestSR
												   {
													   Transaction = transaction,
													   RequestType = this.requestType,
													   RequestSubType = FuelRequestFormSession.SessionFuelRequestSubType,
													   CurrentConsecutiveOOSVariance = currentConsecutiveOosVariance,
													   TransactionOriginallyCompleted = transactionOriginallyCompleted
												   };

				FuelRequestResult result = FMChannelHelper.MakeCall<IFuelRequestProcessor, FuelRequestResult>(
												fuelRequestProcessor => fuelRequestProcessor.Process(this.Security, serviceRequest));

				this.SessionAlertMessage = result.AlertMessage;
			}
			catch (FaultException<SaveTransactionsException> e)
			{
				System.Text.StringBuilder msg = new System.Text.StringBuilder(e.Message + "\n");
				foreach (var transactionValidationResult in e.Detail.Results)
				{
					foreach (var err in transactionValidationResult.ErrorList)
					{
						msg.Append(err + "\n");
					}
				}
				throw new Exception(msg.ToString());

			}
			catch (Exception e)
			{
				throw e;

			}
		}
		#endregion

		#region Transaction Data Validation
		/// <summary>
		/// Examine data on all of the tabs to make sure that the values provided are the values
		/// expected
		/// </summary>
		/// <param name="transaction">Represents the transaction object</param>
		/// <returns>True if everything is OK, false otherwise</returns>
		private bool ValidateTransactionData(TransactionDO transaction)
		{
			if (this.IsFuelRequest)
			{
				if (!this.FuelRequestServiceRequestPage.ValidateTransactionData(transaction)
					|| !this.FuelRequestDetailPage.ValidateTransactionData(transaction))
				{
					return false;
				}
			}
			else
			{
				if (!this.FuelRequestFillStandServiceRequestPage.ValidateTransactionData(transaction)
					|| !this.FuelRequestDetailPage.ValidateTransactionData(transaction))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Determine if the user has cancelled the transaction. We show a warning dialog if this is the case.
		/// </summary>
		/// <param name="transaction">The transaction record to examine</param>
		/// <returns>True if the user cancelled the transaction</returns>
		public bool ShowCancellationWarningDialog(TransactionDO transaction)
		{
			if (this.RequestCancelled &&
				(transaction.TransactionGuid == Guid.Empty || (transaction.TransactionGuid != Guid.Empty && transaction.Status != TransactionStatus.Cancelled)))
			{
				return true;
			}

			return false;
		}



		/// <summary>
		/// Determine if the user picked an aircraft which requires an additive
		/// but the service vehicle doesn't have the additive. 
		/// </summary>
		/// <returns>True if the user picked an aircraft which requires an additive
		/// but the service vehicle doesn't have the additive. </returns>
		public bool ShowConfirmFuelAdditiveDialog()
		{
			// For some reason this is only shown for Fast Log requests
			if (this.requestType == FuelRequestType.FastLog)
			{
				if (this.FuelRequestServiceRequestPage.AircraftGuid != Guid.Empty)
				{
					EquipmentClass aircraft = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
									equipments => equipments.Get(this.Security, this.FuelRequestServiceRequestPage.AircraftGuid));

					if (aircraft != null && this.FuelRequestDetailPage.EquipmentGuid != Guid.Empty)
					{
						EquipmentClass equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
														equipments => equipments.Get(this.Security, this.FuelRequestDetailPage.EquipmentGuid));

						if (equipment != null && equipment.FuelingType == FUELING_TYPES.REFUELER)
						{
							if (aircraft.FuelAdditiveFlag && !equipment.FuelAdditiveFlag)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Determine if the user picked a location that is out of service
		/// </summary>
		/// <returns>True if the user picked a location that is out of service </returns>
		public bool ShowConfirmOutOfServiceLocationDialog()
		{
			// For some reason this is only shown for Fast Log requests
			if (this.requestType == FuelRequestType.FillStand || this.requestType == FuelRequestType.FastLogFillStand)
			{
				if (this.FuelRequestFillStandServiceRequestPage.LocationGuid != Guid.Empty)
				{
					EquipmentClass location = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
									equipments => equipments.Get(this.Security, this.FuelRequestFillStandServiceRequestPage.LocationGuid));

					if (location != null )
					{
						return !location.InServiceFlag;
					}
				}
			}

			return false;
		}
		#endregion


		private TransactionOrigin DetermineOriginApplication()
		{
			return this.IsEnterprise ? TransactionOrigin.DispatchEnterprise : TransactionOrigin.Dispatch;
		}
	}
}