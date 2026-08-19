// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabularView.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TabularView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    //using System.Reflection;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.Services;
    using System.Web.UI;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.ServiceRequests;

    using FMControls;

    using FMCore;

    /// <summary>
	///    Code behind for TabularView page.
	/// </summary>
	public partial class TabularView : FMDispatchFormBase, ICallbackEventHandler
	{
		#region Constants and Fields

		/// <summary>
		///    Key for saving transaction object during cancel comment prompting operations.
		/// </summary>
		private const string TabularViewTransactionSessionKey = "TabularView_Transaction";

		/// <summary>
		///    The result of the most recent client request for transaction data.
		/// </summary>
		private DispatchTransactionDO clientTransactionDO = new DispatchTransactionDO();

		#endregion

/*
		/// <summary>
		/// Clears the specified property of the passed in object.  If the property name contains a dot
		/// then it is assumed that the property of a nested object should be cleared.  The nested object
		/// is specified by the substring of the property name preceding the dot.  The property to clear
		/// is specified by the substring of the property name following the dot.  For example the
		/// specified property name "PaymentInfo.BillTo" indicates that the BillTo property of the
		/// nested PaymentInfo object should be cleared.
		/// </summary>
		/// <param name="obj">The object containing the property to clear</param>
		/// <param name="propertyName">The name of the property to clear</param>
		/// <returns>True if the specified property value cleared successfully</returns>
		static bool ClearObjectProperty(object obj, string propertyName)
		{
			bool result = true;
			try
			{
				Type objType = obj.GetType();

				int dotIndex = propertyName.IndexOf('.');
				if (dotIndex > -1)
				{
					string basePropertyName = propertyName.Substring(0, dotIndex);
					string nestedPropertyName = propertyName.Substring(dotIndex + 1);
					PropertyInfo baseProperty = objType.GetProperty(basePropertyName);
					if (baseProperty != null)
					{
						object nestedObject = baseProperty.GetValue(obj);
						return ClearObjectProperty(nestedObject, nestedPropertyName);
					}
				}

				PropertyInfo property = objType.GetProperty(propertyName);
				if (property != null)
				{
					if (property.PropertyType == typeof(string))
					{
						property.SetValue(obj, string.Empty);
					}
					else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(float))
					{
						property.SetValue(obj, 0.0);
					}
					else if (property.PropertyType == typeof(long) || property.PropertyType == typeof(int)
							|| property.PropertyType == typeof(short) || property.PropertyType == typeof(byte))
					{
						property.SetValue(obj, 0);
					}
					else if (property.PropertyType == typeof(bool))
					{
						property.SetValue(obj, false);
					}
					else if (property.PropertyType == typeof(Guid))
					{
						property.SetValue(obj, Guid.Empty);
					}
					else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						property.SetValue(obj, null);
					}
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}
*/

		#region Properties

		/// <summary>
		///    Gets or sets the transaction cache.
		/// </summary>
		protected List<TransactionDO> TransactionCache { get; set; }

		#endregion

		#region Explicit Interface Methods

		/// <summary>
		///    Returns the callback result to the client.
		/// </summary>
		/// <returns>The callback result string</returns>
		string ICallbackEventHandler.GetCallbackResult()
		{
			var serializer = new JavaScriptSerializer();
			string jsonTransactionDO = serializer.Serialize(this.clientTransactionDO);
			return jsonTransactionDO;
		}

		/// <summary>
		///    Handles the callback event raised by the client.
		/// </summary>
		/// <param name="eventArgument">The event argument</param>
		void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
		{
			var serializer = new JavaScriptSerializer();
			var clientRequest = serializer.Deserialize<ClientRequestParams>(eventArgument);
			this.clientTransactionDO =
				FMChannelHelper.MakeCall<IDispatchRequests, DispatchTransactionDO>(
					x =>
					x.EnumerateTransactions(
						this.Security,
						clientRequest.TopVersion,
						clientRequest.BeginDate,
						clientRequest.EndDate,
						clientRequest.Status,
						clientRequest.RequestName));
		}

		#endregion

		#region Protected ASPX Methods

		/// <summary>
		///    Page_Load event handler for page.
		/// </summary>
		/// <param name="sender">The sender parameter</param>
		/// <param name="e">The event args parameter</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.GenerateClientCallbackFunction();
				this.GenerateCopyCallbackFunction();
				this.GenerateCancelCallbackFunction();
				this.GenerateUnCancelCallbackFunction();

				if (!this.IsPostBack)
				{
					this.InitializeCommonProperties(this.hiddenFields, PageType.TabularView);

					if (this.useDataDictionary)
					{
						this.TranslateContextMenuItems();
					}
				}
				else
				{
					this.RestoreCommonProperties(this.hiddenFields);

					this.ParseCustomEventArguments();
				}

				// A postback always clears the toolbar controls so create toolbar each time the page is loaded
				this.CreateToolbar();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		///    Gets the transaction cached.
		/// </summary>
		/// <param name="transId">The trans id.</param>
		/// <returns>A transaction data object.</returns>
		private TransactionDO GetTransactionCached(string transId)
		{
			if (this.TransactionCache == null)
			{
				this.TransactionCache = new List<TransactionDO>();
			}

			foreach (TransactionDO trans in this.TransactionCache)
			{
				if (trans.TransID == transId)
				{
					return trans;
				}
			}

			TransactionDO transaction = this.GetTransaction(transId);
			this.TransactionCache.Add(transaction);
			return transaction;
		}

		/// <summary>
		///    Cancels the transaction.
		/// </summary>
		private void CancelTransaction()
		{
			var transaction = this.Session[TabularViewTransactionSessionKey] as TransactionDO;

			if (transaction == null)
			{
				throw new ApplicationException("Transaction could not be found in session.");
			}

			// Cancel the transaction
			transaction.Notes += " - " + this.CancelCommentText.Value;
			transaction.Status = TransactionStatus.Cancelled;

			foreach (LineItemDO lineItem in transaction.LineItems)
			{
				lineItem.Status = TransactionStatus.Cancelled;
				lineItem.Quantity = new QuantityDO(0, 0, 0, 0);

				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					subLineItem.Status = TransactionStatus.Cancelled;
				}
			}

			this.SaveTransaction(transaction);
		}

		/// <summary>
		///    Creates the dispatch tabular view custom toolbar.
		/// </summary>
		private void CreateToolbar()
		{
			//var toolbarInfoMap = new Dictionary<string, ToolbarInfo>
			//	{
			//		// Add the default toolbar command buttons in the desired display order
			//		{ "Request", new ToolbarInfo(true, "toolStripRequestRefuelButton.Image.png", null) },
			//		{ "Transient", new ToolbarInfo(true, "toolStripTransientButton.Image.png", null) },
			//		{ "Fast Log", new ToolbarInfo(true, "toolStripFastLogButton.Image.png", null) },
			//		{ "Fast Log Fillstand", new ToolbarInfo(true, "toolStripFastLogFillstandButton.Image.png", null) },
			//		{ "Relog", new ToolbarInfo(true, "toolStripCopyButton.Image.png", null) },
			//		{ "Dispatch", new ToolbarInfo(true, "toolStripDispatchButton.Image.png", null) },
			//		{ "Control Log", new ToolbarInfo(true, "toolStripControlLogButton.Image.png", null) },
			//		{ "Standby", new ToolbarInfo(true, "toolStripStandbyButton.Image.png", null) },
			//		{ "Flight Line Status", new ToolbarInfo(true, "toolStripFlightLineButton.Image.png", "Flight Line") },
			//		{ "Dispatchers List", new ToolbarInfo(true, null, null) },
			//		{ "Cancel", new ToolbarInfo(true, "toolStripCancelButton.Image.png", null) },
			//		{ "Refresh", new ToolbarInfo(true, null, null) },
			//		{ "Arrival", new ToolbarInfo(false, null, null) },
			//		{ "Change Operator Status", new ToolbarInfo(false, null, null) },
			//		{ "Evacuate", new ToolbarInfo(false, null, null) },
			//		{ "Release To Accounting", new ToolbarInfo(false, null, null) },
			//		{ "Fillstand Completion", new ToolbarInfo(false, null, null) },
			//		{ "Help", new ToolbarInfo(false, "toolStripHelpButton.Image.png", null) },
			//		{ "Optional Times", new ToolbarInfo(false, null, null) },
			//		{ "Query Writer", new ToolbarInfo(false, null, null) },
			//		{ "Recirculation", new ToolbarInfo(false, null, null) },
			//		{ "Reports", new ToolbarInfo(false, null, null) },
			//		{ "Service Completion", new ToolbarInfo(false, null, null) },
			//		{ "Start Of Service", new ToolbarInfo(false, null, null) },
			//		{ "Stop Of Service", new ToolbarInfo(false, null, null) },
			//		{ "Total And Average", new ToolbarInfo(false, null, null) },
			//		{ "Uncancel", new ToolbarInfo(false, null, null) }
			//	};




			CustomToolbarType toolbarType = FMChannelHelper.MakeCall<ICustomToolbars, CustomToolbarType>(
									commands => commands.EnumerateToolbarTypeById(this.Security, "Dispatch Tabular View"));

			Dictionary<string, ToolbarInfo> toolbarInfoMap =
				FMChannelHelper.MakeCall<ICustomToolbarCommands, CustomToolbarCommandTypeList>(
					x => x.EnumerateCommandTypes(this.Security, toolbarType.LookupIndex)).ToDictionary(x =>x.Id, x=> new ToolbarInfo(x.IsDefault, x.ImageSource, null));

			// A toolbar command button is either a standard command defined in the toolbar info map
			// or a transaction alias command contained in the custom toolbar command list.
			var buttons = new List<ButtonInfo>();

			// Get toolbar button data once and save to hidden field
			if (!this.IsPostBack)
			{
				// Get the Tabular View Custom Toolbar
				var customToolbar = new CustomToolbarClass { ID = toolbarType.Id};

				FMChannelHelper.MakeCall<ICustomToolbars>(
					customToolbars =>
						{
							Guid customToolbarGuid = customToolbars.GetIdentityGuidById(
								this.Security, customToolbar.ID, this.DispatchConfigurationGuid);
							if (customToolbarGuid != Guid.Empty)
							{
								customToolbar = customToolbars.Get(this.Security, customToolbarGuid);
							}
						});

				if (customToolbar.IdentityGuid != Guid.Empty)
				{
					// Populate the command button list with the custom set of toolbar commands
					foreach (CustomToolbarCommandClass toolbarCommand in customToolbar.ToolbarCommandList)
					{
						bool isTransactionAlias = toolbarCommand.TransactionAliasGuid != Guid.Empty;
						buttons.Add(new ButtonInfo(toolbarCommand.ID, isTransactionAlias));
					}
				}
				else
				{
					// Enumerate the custom toolbar types
					var customToolbarTypes = FMChannelHelper.MakeCall<ICustomToolbars, List<CustomToolbarType>>(
						customToolbars => customToolbars.EnumerateToolbarTypes(this.Security));

					foreach (CustomToolbarType toolbarClass in customToolbarTypes)
					{
						if ( toolbarClass.Id == customToolbar.ID)
						{
							var defaultCommands =
								FMChannelHelper.MakeCall<ICustomToolbarCommands, CustomToolbarCommandTypeList>(
									commands => commands.EnumerateDefaultCommandTypes(this.Security, toolbarClass.LookupIndex));

							// Populate the command button list with the default set of toolbar commands
							foreach (CustomToolbarCommandType toolbarItem in defaultCommands)
							{
								if (toolbarItem.IsDefault)
								{
									buttons.Add(new ButtonInfo(toolbarItem.Id, false));
								}
							}

							break;
						}
					}
				}

				var serializer = new JavaScriptSerializer();
				this.hiddenFields.JsonToolbarButtonList = serializer.Serialize(buttons);
			}
			else
			{
				var serializer = new JavaScriptSerializer();
				buttons = serializer.Deserialize<List<ButtonInfo>>(this.hiddenFields.JsonToolbarButtonList);
			}

			// Must match the tabindex of the VehicleSelect control specified in the ASPX file
			short lastTabIndex = 5;

			// Create the toolbar command buttons
			string id = string.Empty;
			bool firstButton = true;
			foreach (ButtonInfo button in buttons)
			{
				if (button.IsTransactionAlias)
				{
					string aliasName = button.CommandName;
					string text = aliasName + CustomToolbarCommandClass.TransactionAliasDesignator;
					string aliasNameNoSpaces = aliasName.Replace(" ", string.Empty);

					id = aliasNameNoSpaces + "TransactionAliasButton";
					const string CssClass = "buttonStyle";
					string onClick = "TabularViewLib.TransactionAliasButtonOnClick('" + aliasName + "')";
					var toolbarButton = new FMToolbarButton(null, text, id, CssClass, onClick, ++lastTabIndex);
					this.toolBarTabular.Controls.Add(toolbarButton);
				}
				else
				{
					string commandName = button.CommandName;
					string commandNameNoSpaces = commandName.Replace(" ", string.Empty);
					string img = toolbarInfoMap[commandName].SourceImage;
					if (img != null)
					{
						img = "images/" + img;
					}

					string text = toolbarInfoMap[commandName].CustomText;
					if (string.IsNullOrEmpty(text))
					{
						text = this.GetTranslatedText(commandName);
					}
					else
					{
						text = this.GetTranslatedText(text);
					}

					id = commandNameNoSpaces + "Button";
					const string CssClass = "buttonStyle";
					string onClick = "TabularViewLib." + id + "OnClick()";
					var toolbarButton = new FMToolbarButton(img, text, id, CssClass, onClick, ++lastTabIndex);
					this.toolBarTabular.Controls.Add(toolbarButton);
				}

				if (firstButton)
				{
					firstButton = false;
					this.toolBarTabular.FirstButtonTabIndex = lastTabIndex.ToString(CultureInfo.InvariantCulture);
				}
			}
			this.toolBarTabular.LastButtonId = id;
			this.toolBarTabular.LastButtonTabIndex = lastTabIndex.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///    Generates the cancel callback function.
		/// </summary>
		private void GenerateCancelCallbackFunction()
		{
			string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "CancelMenuItemClick");

			// Create script block
			string scriptBlock = @"
						function endCancelDialogGetResponse()
						{
							$(this).dialog('close');
							" + postBackString + @";
						}

						TabularViewLib.CancelButtonOnClick = function () {
							var numRows = TabularViewLib.selectedRows.length;
							if (numRows < 1) {
								return;
							}
	
							// Check for completed or cancelled transactions
							for (var index = 0; index < numRows; ++index) {
								var rowNum = TabularViewLib.selectedRows[index];
								if (TabularViewLib.data[rowNum].Status == 'Completed' || TabularViewLib.data[rowNum].Status == 'Cancelled') {
									alert('Completed or Cancelled requests cannot be cancelled.  Operation will be aborted.');
									return;
								}
							}

							$('#CancelConfirmationDialog').dialog(
							{
								autoOpen: false,
								modal: true,
								width: 450,
								height: 200,
								buttons: {
									'Yes': endCancelDialogGetResponse,
									'No' : function() {$(this).dialog('close'); }
								}
							});

							$('#CancelConfirmationDialog').dialog('open');
						};
					";

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "CancelMenuItemEvent", scriptBlock, true);
		}

		/// <summary>
		///    Generates the client callback function.
		/// </summary>
		private void GenerateClientCallbackFunction()
		{
			string callbackReference = this.Page.ClientScript.GetCallbackEventReference(
				this, "arg", "TabularViewLib.ReceiveServerData", string.Empty);

			string callbackScript = "TabularViewLib.CallServer = function(arg, context) {" + callbackReference + "; };";

			this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TabularViewLib.CallServer", callbackScript, true);
		}

		/// <summary>
		///    Generates the copy callback function.
		/// </summary>
		private void GenerateCopyCallbackFunction()
		{
			string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "RelogMenuItemClick");

			// Create script block
			string scriptBlock = @"
						TabularViewLib.RelogButtonOnClick = function()
						{
							" + postBackString + @";
						}
					";

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "RelogMenuItemEvent", scriptBlock, true);
		}

		/// <summary>
		///    Generates the cancel callback function.
		/// </summary>
		private void GenerateUnCancelCallbackFunction()
		{
			string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "UnCancelMenuItemClick");

			// Create script block
			string scriptBlock = @"
						function endUnCancelDialogGetResponse()
						{
							$(this).dialog('close');
							" + postBackString + @";
						}

						TabularViewLib.UncancelButtonOnClick = function () {
							var numRows = TabularViewLib.selectedRows.length;
							if (numRows < 1) {
								return;
							}
	
							// Check for cancelled transaction status
							for (var index = 0; index < numRows; ++index) {
								var rowNum = TabularViewLib.selectedRows[index];
								if (TabularViewLib.data[rowNum].Status != 'Cancelled') {
									alert('Only Cancelled requests can be Uncancelled!  Operation will be aborted!');
									return;
								}
							}

							$('#UnCancelConfirmationDialog').dialog(
							{
								autoOpen: false,
								modal: true,
								width: 450,
								height: 200,
								buttons: {
									'Yes': endUnCancelDialogGetResponse,
									'No' : function() {$(this).dialog('close'); }
								}
							});

							$('#UnCancelConfirmationDialog').dialog('open');
						};
					";

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "UnCancelMenuItemEvent", scriptBlock, true);
		}

		/// <summary>
		///    Parses custom event arguments and dispatches the messages to handler routines as appropriate.
		/// </summary>
		private void ParseCustomEventArguments()
		{
			try
			{
				string arguments = this.Request.GetQueryOrFormValue("__EVENTARGUMENT");
				if (arguments != null)
				{
					if (arguments.Equals("RelogMenuItemClick", StringComparison.InvariantCultureIgnoreCase))
					{
						this.RelogTransaction();
						return;
					}

					if (arguments.Equals("CancelMenuItemClick", StringComparison.InvariantCultureIgnoreCase))
					{
						this.LoopStorage.Value = "0";
						this.TransactionCache = new List<TransactionDO>();
						this.StartCancelTransactions();
						return;
					}

					if (arguments.Equals("UnCancelMenuItemClick", StringComparison.InvariantCultureIgnoreCase))
					{
						this.TransactionCache = new List<TransactionDO>();
						this.UnCancelTransactions();
						return;
					}

					if (arguments.Equals("CommentOkClickEvent", StringComparison.InvariantCultureIgnoreCase))
					{
						this.CancelTransaction();
						this.StartCancelTransactions();
						return;
					}

					if (arguments.Equals("CommentCancelClickEvent", StringComparison.InvariantCultureIgnoreCase))
					{
						this.Session.Remove(TabularViewTransactionSessionKey);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Prompts for cancellation comment.
		/// </summary>
		/// <param name="transaction">
		///    The transaction.
		/// </param>
		private void PromptForCancellationComment(TransactionDO transaction)
		{
			// Get the postback script name
			string postBackString = this.Page.ClientScript.GetPostBackEventReference(this, "CommentOkClickEvent");
			string postBackCancelString = this.Page.ClientScript.GetPostBackEventReference(this, "CommentCancelClickEvent");

			// Create script block
			string scriptBlock = @"
						function endDialogGetResponse()
						{
							$('#CancelCommentText').val( $('#CancelCommentTextBox').text() );
							$(this).dialog('close');
							" + postBackString + @";
						}

						function endDialogCancelResponse()
						{
							$(this).dialog('close');
							" + postBackCancelString + @";
						}

						function CancelCommentDialogFunction() {
							$('#CancelCommentDialog').dialog(
							{
								autoOpen: false,
								modal: true,
								width: 500,
								height: 325,
								buttons: {
									'OK': endDialogGetResponse,
									'Cancel' : endDialogCancelResponse
								}
							});

							$('#CancelCommentTextBox').width(450);
							$('#CancelCommentTextBox').height(150);

							$('#CancelCommentHeading').text( $('#CancelCommentHeading').text() + '" + transaction.Alias + @"');

							$('#CancelCommentDialog').dialog('open');
						}

						window.setTimeout('CancelCommentDialogFunction()', 100);
					";

			this.ClientScript.RegisterClientScriptBlock(this.GetType(), "CancelCommentScriptBlock", scriptBlock, true);
		}

		/// <summary>
		/// This method handles the relogging of a transactions.
		/// </summary>
		private void RelogTransaction()
		{
			try
			{
				if (this.Security.HasRight(RIGHT.MODIFY_DISPATCH) == false)
				{
					return;
				}

				var sr = new CopyTransactionsSR
				{
					Security = this.Security
				};

				var transactionResults = new List<SaveTransactionsResultDO>();

				string transactionIds = this.RequestGridSelection.Value;
				List<TransactionDO> transactionArray = this.ParseTransactionIds(transactionIds);

				// Make sure that only refuel and defuel requests were selected to be copied
				if (transactionArray.Find(transaction => transaction.TransTypeID == TransactionTypes.T7_FillStand) != null)
				{
					throw new ApplicationException("Cannot relog fillstand requests.");
				}

				FMChannelHelper.MakeCall<ICopyTransactionsProcessor>(
					copyTransactionsProcessor =>
					{
						foreach (TransactionDO transactionDO in transactionArray)
						{
							string transID = transactionDO.TransID;
							sr.TransactionIds.Add(transID);

							// Each transaction type may need to request a different type of
							// document number.
							switch (transactionDO.TransTypeID)
							{
								case TransactionTypes.T5_PrimaryDisbursement:
								case TransactionTypes.T25_Shipment:
									sr.DocumentTypes.Add(DOCUMENT_TYPE.MANUAL_BOL);
									break;
								case TransactionTypes.T17_Order:
								case TransactionTypes.T18_SupplyOrder:
									sr.DocumentTypes.Add(DOCUMENT_TYPE.ORDER);
									break;
								default:
									sr.DocumentTypes.Add(DOCUMENT_TYPE.TRANSACTION);
									break;
							}

							SaveTransactionsResultDO resultDo = copyTransactionsProcessor.Process(sr);
							sr.TransactionIds.Clear();

							// Collect errors/warnings if there were any.
							if (resultDo.Results.Count > 0)
							{
								transactionResults.Add(resultDo);
							}
						}
					});

				if (transactionResults.Count > 0)
				{
					bool foundErrors = false;
					string msg = string.Empty;

					foreach (SaveTransactionsResultDO resultDo in transactionResults)
					{
						foreach (TransactionValidationResult validationResult in resultDo.Results)
						{
							if (validationResult.IsValid == false)
							{
								msg = "Save transaction Errors";
								msg = msg + "! ";

								foreach(string error in validationResult.ErrorList)
								{
									msg += "\n\r" + error;
									foundErrors = true;
								}
							}
						}
					}

					if (foundErrors)
					{
						throw new Exception(msg);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Starts the cancel transaction process.
		/// </summary>
		private void StartCancelTransactions()
		{
			string[] transactionIds = this.RequestGridSelection.Value.Split(',');

			int index = Convert.ToInt32(this.LoopStorage.Value);

			// Check to see if we are already done
			if (index >= transactionIds.Length)
			{
				const string Script = @"
					function setCancelFilter()
					{
						$('#StatusSelect').val('Cancelled');
					}				

					window.setTimeout('setCancelFilter()', 100);
				";

				this.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetCancelFilter", Script, true);
				return;
			}

			// Get the transaction
			TransactionDO transaction = this.GetTransactionCached(transactionIds[index]);
			if (transaction == null)
			{
				throw new ApplicationException("Could not find transaction: " + transactionIds[index]);
			}

			// Issue error if transaction is completed.  We cannot cancel completed transactions.
			if (transaction.Status == TransactionStatus.Cancelled)
			{
				throw new ApplicationException("Completed requests cannot be cancelled.");
			}

			// Save the transaction in session for return
			this.Session[TabularViewTransactionSessionKey] = transaction;

			// Set up a comment dialog prompt
			this.PromptForCancellationComment(transaction);

			this.LoopStorage.Value = (index + 1).ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///    Translates the context menu items.
		/// </summary>
		private void TranslateContextMenuItems()
		{
			// Translate context menu items.
			FMChannelHelper.MakeCall<IDataDictionariesClass>(
				dict =>
				{
					this.ArrivedItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.ArrivedItem.InnerText);
					this.StartedItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.StartedItem.InnerText);
					this.StoppedItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.StoppedItem.InnerText);
					this.CompletedItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.CompletedItem.InnerText);
					this.FillstandCompleteItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.FillstandCompleteItem.InnerText);
					this.RelogItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.RelogItem.InnerText);
					this.CancelItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.CancelItem.InnerText);
					this.UncancelItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.UncancelItem.InnerText);
					this.QualityResultsItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.QualityResultsItem.InnerText);
					this.TrainingAssignmentsItem.InnerText = dict.Get(this.Security.LoginSiteGuid, this.TrainingAssignmentsItem.InnerText);
				});
		}

		/// <summary>
		///    UnCancels the transaction.
		/// </summary>
		private void UnCancelTransactions()
		{
			string[] transactionIds = this.RequestGridSelection.Value.Split(',');

			foreach (string transactionId in transactionIds)
			{
				// Get the transaction
				TransactionDO transaction = this.GetTransactionCached(transactionId);
				if (transaction == null)
				{
					throw new ApplicationException("Transaction could not be found.");
				}

				// Skip transactions that are not cancelled
				if (transaction.Status != TransactionStatus.Cancelled)
				{
					continue;
				}

				foreach (LineItemDO lineItem in transaction.LineItems)
				{
					lineItem.Status = TransactionStatus.Requested;
				}

				transaction.Notes += "- Transaction uncanceled - status set to requested.";
				transaction.Status = TransactionStatus.Requested;

				this.SaveTransaction(transaction);
			}
		}

		private static SecurityClass CheckWebMethodSecurity( string token )
		{
			var security = (SecurityClass) HttpContext.Current.Session["Security"];

			var tokenGuid = new Guid( token );

			if ( security == null || security.Token != tokenGuid )
			{
				security = FMChannelHelper.MakeCall<ISites, SecurityClass>( x => x.GetSecurity( token ) );
			}

			if ( security.Token != tokenGuid )
			{
				throw new FMInsufficientRightsException();
			}

			return security;
		}

		[WebMethod]
		public static object EnumerateOperatorStatus(string securityToken)
		{
			var security = CheckWebMethodSecurity(securityToken);

			var allDrivers =
				FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
					x => x.EnumerateByRole(security, PERSON_ROLE.LOADER_ROLE));

			// Get the list of equipment to offer for selection
			EquipmentCollectionClass equipmentList =
				FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>( x => x.EnumerateBySource( security ) );

			var list = new List<object>();

			foreach ( EquipmentClass equipment in equipmentList )
			{
				// Add equipment to the registration selection control only if it has not been assigned to a personnel record
				if ( !equipment.IsAssignedToPersonnel )
				{
				    // ReSharper disable once RedundantAnonymousTypePropertyName
					list.Add(new {ID = equipment.ID, IdentityGuid = equipment.MasterRecordGuid});
				}
			}

			var value = new { EquipmentList = list, StatusList = new List<OperatorStatusData>() };

			foreach (var person in allDrivers)
			{
				string color = "blue";
				if (person.LockedOut)
				{
					color = "red";
				}
				else if (person.Status == PersonClass.STATUS.STB)
				{
					color = "gray";
				}

				value.StatusList.Add(new OperatorStatusData
				          {
					          PersonGuid = person.IdentityGuid,
							  FullName = person.FullName,
							  StatusCode = person.StatusText,
							  EquipmentID = person.AssignedEquipmentID,
							  EquipmentGuid = person.AssignedEquipmentGuid,
							  LockedOut = person.LockedOut,
							  EmployeeID = person.ID,
							  ForeColor = color
				          });
			}

			return value;
		}

		[WebMethod]
		public static void SetOperatorHomeStatus(string securityToken, List<string> guids)
		{
			var security = CheckWebMethodSecurity(securityToken);

			FMChannelHelper.MakeCall<IPersonnel>(
				personnel =>
				{
					foreach (var guidText in guids)
					{
						var guid = new Guid(guidText);

						var person = personnel.Get(security, guid);

						person.Status = PersonClass.STATUS.In;
						person.AssignedEquipmentID = string.Empty;
						person.AssignedEquipmentGuid = Guid.Empty;

						personnel.Modify(security, DATA_TYPE.AUTOMIC, person);
					}
				});
		}

		[WebMethod]
		public static void SetOperatorOutStatus( string securityToken, List<string> guids )
		{
			var security = CheckWebMethodSecurity( securityToken );

			FMChannelHelper.MakeCall<IPersonnel>(
				personnel =>
				{
					foreach ( var guidText in guids )
					{
						var guid = new Guid( guidText );

						var person = personnel.Get( security, guid );

						person.Status = PersonClass.STATUS.Out;
						person.AssignedEquipmentID = string.Empty;
						person.AssignedEquipmentGuid = Guid.Empty;

						personnel.Modify( security, DATA_TYPE.AUTOMIC, person );
					}
				} );
		}

		[WebMethod]
		public static void SetOperatorStandbyStatus( string securityToken, string personGuidText, string equipmentGuidText )
		{
			var security = CheckWebMethodSecurity( securityToken );

			if (string.IsNullOrEmpty(personGuidText) == false && string.IsNullOrEmpty(equipmentGuidText) == false)
			{
				FMChannelHelper.MakeCall<IPersonnel>(
					personnel =>
					{
						var personGuid = new Guid(personGuidText);
						var equipmentGuid = new Guid(equipmentGuidText);

						var person = personnel.Get(security, personGuid);

						var equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(security, equipmentGuid));

						person.AssignedEquipmentGuid = equipmentGuid;
						person.AssignedEquipmentID = equipment.ID;
						person.Status = PersonClass.STATUS.STB;

						personnel.Modify(security, DATA_TYPE.AUTOMIC, person);
					});
			}
		}

		#endregion

		public class OperatorStatusData
		{
			public Guid PersonGuid { get; set; }
			public string FullName { get; set; }
			public string StatusCode { get; set; }
			public string EquipmentID { get; set; }
			public bool LockedOut { get; set; }
			public string EmployeeID { get; set; }
			public string ForeColor { get; set; }
			public Guid EquipmentGuid { get; set; }
		}

		/// <summary>
		///    The client request params.
		/// </summary>
		public struct ClientRequestParams
		{
			/// <summary>
			///    The begin date.
			/// </summary>
			public string BeginDate;

			/// <summary>
			///    The end date.
			/// </summary>
			public string EndDate;

			/// <summary>
			///    The request name.
			/// </summary>
			public string RequestName;

			/// <summary>
			///    The status.
			/// </summary>
			public string Status;

			/// <summary>
			///    The top version.
			/// </summary>
			public string TopVersion;
		}

		/// <summary>
		///    Structure containing the toolbar button command name and a flag indicating
		///    whether or not the toolbar button command is a transaction alias command
		/// </summary>
		public struct ButtonInfo
		{
			/// <summary>
			///    The toolbar button command name
			/// </summary>
			public string CommandName;

			/// <summary>
			///    True if command is a transaction alias command
			/// </summary>
			public bool IsTransactionAlias;

			/// <summary>
			///    Initializes a new instance of the ButtonInfo struct.
			/// </summary>
			/// <param name="name">The toolbar button command name</param>
			/// <param name="isAlias">True if command is a transaction alias command</param>
			public ButtonInfo(string name, bool isAlias)
			{
				this.CommandName = name;
				this.IsTransactionAlias = isAlias;
			}
		}

		/// <summary>
		///    Dictionary value item used in the generation of the custom toolbar
		/// </summary>
		public struct ToolbarInfo
		{
			/// <summary>
			///    The custom text to display for the toolbar command name
			/// </summary>
			public string CustomText;

			/// <summary>
			///    True if toolbar command is a default command
			/// </summary>
			public bool DefaultCommand;

			/// <summary>
			///    The filename of the toolbar buttom image
			/// </summary>
			public string SourceImage;

			/// <summary>
			///    Initializes a new instance of the <see cref="ToolbarInfo" /> struct.
			/// </summary>
			/// <param name="defaultCommand">True indicates the toolbar command is a default command</param>
			/// <param name="sourceImage">The filename of the toolbar buttom image</param>
			/// <param name="customText">The custom text to display for the toolbar command name</param>
			public ToolbarInfo(bool defaultCommand, string sourceImage, string customText)
			{
				this.DefaultCommand = defaultCommand;
				this.SourceImage = sourceImage;
				this.CustomText = customText;
			}
		}
	}
}