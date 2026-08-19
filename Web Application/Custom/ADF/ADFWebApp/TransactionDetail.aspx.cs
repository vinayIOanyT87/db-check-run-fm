/// <summary>
///	File name:	TransactionDetail.cs
///	Purpose:	   The ADF Transaction Detail page is derived from the Accounting Transaction Detail page
///	            and implements custom features for ADF.
///	            
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 2009.  
///					This file shall not be copied or reproduced in any form 
///					without the express written consent of Varec.
///
///	Author(s):	Jack Shen and Richard Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///      Date:          By:                  Reason:
///      ----------     -------------------- ----------------------------------
///      2010-06-15		W.Gray					WI Reason for the change
///		
/// </summary>
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

using FMControls;
using Interop.FMUtil;
using TransactionFields;
using EngineeringUnitsLibrary;

using Accounting;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.ServiceRequests;

namespace ADFWebApp
{
	public partial class TransactionDetail : TransactionDetailBase, IDataDictionary
	{
		#region Protected Attributes
		protected bool bTransIDBeingLoaded = false;
		protected string customRedirect = null;
		protected bool disableAll = false;
		#endregion // Attributes

		#region Private data members
		public string errorMsg = "";
		public const string ALIAS_ADJUSTMENT = "ADJUSTMENT";
		public const string BULK_PURCHASE_ORDER = "BULK PURCHASE ORDER";
		public const string ALIAS_COMMERCIAL = "COMMERCIAL";
		public const string ALIAS_CONSUMER_TRANSFER = "CONSUMER TRANSFER";
		public const string ALIAS_DEFUEL = "DEFUEL";
		public const string ALIAS_DEMAND_AVIATION = "DEMAND (AVIATION)";
		public const string ALIAS_DEMAND_GROUND = "DEMAND (GROUND)";
		public const string ALIAS_DEMAND_MARINE = "DEMAND (MARINE)";
		public const string ALIAS_DIRECT_FUEL_PURCHASE = "DIRECT FUEL PURCHASE";
		public const string ALIAS_DISPOSAL = "DISPOSAL";
		public const string ALIAS_DUE_OUTS = "DUE OUTS";
		public const string ALIAS_FILL_STAND = "FILL STAND";
		public const string ALIAS_FQC = "FQC";
		public const string ALIAS_FUEL_ORDER = "FUEL ORDER";
		public const string ALIAS_ISSUE_AVIATION = "ISSUE (AVIATION)";
		public const string ALIAS_ISSUE_GROUND = "ISSUE (GROUND)";
		public const string ALIAS_ISSUE_MARINE = "ISSUE (MARINE)";
		public const string ALIAS_PAYMENT = "PAYMENT";
		public const string ALIAS_PHYSICAL_INVENTORY = "PHYSICAL INVENTORY";
		public const string ALIAS_RECEIPT = "RECEIPT";
		public const string ALIAS_RECOVERY = "RECOVERY";
		public const string ALIAS_REGRADE = "REGRADE";
		public const string ALIAS_RETURN = "RETURN";
		public const string ALIAS_RETURN_TO_BULK = "RETURN TO BULK";
		public const string ALIAS_SALE_AVIATION = "SALE (AVIATION)";
		public const string ALIAS_SALE_GROUND = "SALE (GROUND)";
		public const string ALIAS_SALE_MARINE = "SALE (MARINE)";
		public const string ALIAS_SHIPMENT = "SHIPMENT";
		public const string ALIAS_TANK_TO_TANK = "TANK TO TANK";
		#endregion

		#region Public Constants
		public static string CUSTOM_DISABLE_FLAG = "disableAll";
		public static string CUSTOM_REDIRECT_PARAM = "CustomRedirect";
		public static string SESSION_TRANSACTION_OBJECT = "ADFTransactionDetail.TransactionObject";
		#endregion // Constants

		#region Constructor
		public TransactionDetail ( )
			: base ( )
		{
		}
		#endregion // Constructor

		#region Data Dictionary
		string[] IDataDictionary.Keys ( SecurityClass a_security )
		{
			// JS20100915 WI-17454 add this as a dictionary key for receipt association
			string[] keys = 
			{
				"Alternative Net Volume"
			};
			return keys;
		}
		#endregion // Data Dictionary

		#region Must Overrides

		protected override void OnInit ( EventArgs e )
		{
			base.OnInit ( e );
		}
		protected bool canModify ( )
		{
			return security.HasModifyTransactionRightByAliasName ( trans.Alias );
		}

		/// <summary>
		/// This is the main entry point for the ADF Transaction Detail page. It overrides the
		/// base class implementation.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void Page_Load ( object sender, EventArgs e )
		{
			if (Request.Params[CUSTOM_DISABLE_FLAG] != null)
			{
				this.disableAll = bool.Parse ( Request.Params[CUSTOM_DISABLE_FLAG] );
			}

			// CCP-045 since ADF transaction detail don't have delete button, have to initialise it so that base behaviour does not crash
			if (null == base.DeleteButton)
			{
				base.DeleteButton = new FMDeleteButton ( );
			}

			// JS20100921 This is a quick fix for the DF/Commercial UOM not being carried through defect
			if (trans.Alias.ToUpper ( ).Equals ( ALIAS_COMMERCIAL ) ||
			   trans.Alias.ToUpper ( ).Equals ( ALIAS_DIRECT_FUEL_PURCHASE ))
			{
				try
				{
					string uomValue = Request.Params["TransactionFields.LineItemAlternativeGrossVolumeFG"];
					( trans.LineItems[0] as LineItemDO ).AlternativeGrossVolume = double.Parse ( uomValue );
					string fieldName = Common.FieldControlName ( Common.FieldControls.LINEITEM_ALTERNATIVEGROSSQUANTITY );
					TextBox tb = FieldTable.FindControl ( fieldName ) as TextBox;

					if (tb != null)
					{
						tb.Text = uomValue;
					}
				}
				catch (Exception)
				{
				}
			}


			// JS20100805 WI-16449 check for changes in customer and product type and reset other fields accordingly
			HandleFieldStateChange ( );

			base.Page_Load ( sender, e );

			// JS20101015 This line forces redirection to the ADF custom invoice summary instead of core which doesn't meet CCP-042 req
			if (trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				customRedirect = "../ADFWebApp/InvoicePaymentSummary.aspx?mode=" + (int) InvoicePaymentMode.INVOICE;
			}
			else
			{
				customRedirect = Request.Params[TransactionDetail.CUSTOM_REDIRECT_PARAM];
			}

			// Disable fields based on transaction aliases and rights.
			this.ClientSideDisableFieldsBasedOnRights ( );

			// Hide the combine button for ADF
			CombineBtn.Visible = false;
		}

		protected override void SetLineItemCurrencyFields ( CurrencyClass currency, LineItemDO lineItem )
		{
			// JS20100827 WI-17114 for sales, do not let foreign currency influence fuel price, related to
			// change request WI-17161
			if (!trans.Alias.ToUpper ( ).Contains ( "SALE" ))
			{
				base.SetLineItemCurrencyFields ( currency, lineItem );
			}
		}

		protected void HandleFieldStateChange ( )
		{
			// JS20100805 WI-16449 check for changes in customer and product type and reset other fields accordingly
			try
			{
				FMChannelFactory<ITransactionAliases> aliasesClient = new FMChannelFactory<ITransactionAliases> ( );
				ITransactionAliases aliases = aliasesClient.CreateProxy ( );

				TransactionAliasClass alias = aliases.Get ( security, trans.TransactionAliasGuid, false );

				// header stuff
				string customerFieldName = Common.FieldControlName ( Common.FieldControls.SHIPTO );
				string toCustomerFieldName = Common.FieldControlName ( Common.FieldControls.TO_SHIPTO );
				string fromCustomerFieldName = Common.FieldControlName ( Common.FieldControls.FROM_SHIPTO );
				string shipperFieldName = Common.FieldControlName ( Common.FieldControls.SHIPPER );

				bool shipToChanged = false;
				bool toShipTochanged = false;
				bool fromShipTochanged = false;

				string shipToID = trans.ShipToID;
				if (shipToID == null)
				{
					shipToID = "";
				}

				// in the following try/catch blocks, we catch out of range errors, or 
				// subsequently null references, this is quicker than
				// having to find if the field name keys exists.
				try 
				{ 
					shipToChanged = !Request.Params[customerFieldName].Equals ( shipToID ); 
				}
				catch (Exception) 
				{ 
				}

				if (trans.GetType ( ) == typeof ( ConsumerTransferDO ))
				{
					string toShipToID = ( trans as ConsumerTransferDO ).ToShipToID;
					if (toShipToID == null)
					{
						toShipToID = "";
					}

					try 
					{ 
						toShipTochanged = !Request.Params[toCustomerFieldName].Equals ( toShipToID ); 
					}
					catch (Exception) 
					{
					}

					string fromShipToID = ( trans as ConsumerTransferDO ).ShipToID;
					if (fromShipToID == null)
					{
						fromShipToID = "";
					}

					try 
					{ 
						fromShipTochanged = !Request.Params[fromCustomerFieldName].Equals ( fromShipToID ); 
					}
					catch (Exception) 
					{ 
					}
				}

				List<Common.FieldControls> clearList = new List<Common.FieldControls> ( );
				if (shipToChanged)
				{
					clearList.Add ( Common.FieldControls.DESTINATIONEQUIPMENTFG1 );
					clearList.Add ( Common.FieldControls.SOURCEEQUIPMENTFG1 );
					clearList.Add ( Common.FieldControls.LINEITEM_PRODUCT );
				}

				if (toShipTochanged)
				{
					clearList.Add ( Common.FieldControls.DESTINATIONEQUIPMENTFG1 );
					if (trans.GetType ( ) == typeof ( ConsumerTransferDO ))
						clearList.Add ( Common.FieldControls.LINEITEM_PRODUCT );
				}

				if (fromShipTochanged)
				{
					clearList.Add ( Common.FieldControls.SOURCEEQUIPMENTFG1 );
				}

				if (shipToChanged &&
				   ( trans.Alias.ToUpper ( ).Equals ( ALIAS_DEFUEL ) || trans.Alias.ToUpper ( ).Equals ( ALIAS_RETURN_TO_BULK ) ))
				{
					clearList.Add ( Common.FieldControls.SOURCEEQUIPMENTFG1 );
				}

				foreach (Common.FieldControls fieldControl in clearList)
				{
					EquipmentDO equipment = null;
					switch (fieldControl)
					{
						case Common.FieldControls.SOURCEEQUIPMENTFG1:
							if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_SALE_AVIATION ))
							{
								equipment = trans.SourceEQ1;
							}
							break;
						case Common.FieldControls.DESTINATIONEQUIPMENTFG1:
							if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_DEFUEL ))
							{
								equipment = trans.DestinationEQ1;
							}
							break;
						case Common.FieldControls.LINEITEM_PRODUCT:
							( trans.LineItems[0] as LineItemDO ).Product = "";
							( trans.LineItems[0] as LineItemDO ).ProductCode = "";
							(trans.LineItems[0] as LineItemDO).ProductGuid = Guid.Empty;

							string fieldName = Common.FieldControlName ( fieldControl );
							FMProductTextBox productCtrl = FieldTable.FindControl ( fieldName ) as FMProductTextBox;
							if (productCtrl != null)
							{
								productCtrl.Text = "";
							}
							break;
					}

					if (equipment != null)
					{
						string assetFieldName = Common.FieldControlName ( fieldControl );
						FMEquipmentTextBox equipmentCtrl = FieldTable.FindControl ( assetFieldName ) as FMEquipmentTextBox;

						if (equipmentCtrl != null)
						{
							equipmentCtrl.Text = "";

							equipment.RegistrationID = "";
							equipment.EquipmentGuid = Guid.Empty;
						}
					}
				}

				if (!alias.MultipleLineItems)
				{
					string productFieldName = Common.FieldControlName ( Common.FieldControls.LINEITEM_PRODUCT );
					string fromProductFieldName = Common.FieldControlName ( Common.FieldControls.LINEITEM_FROM_PRODUCT );
					string toProductFieldName = Common.FieldControlName ( Common.FieldControls.LINEITEM_TO_PRODUCT );

					bool productChanged = false;
					bool fromProductChanged = false;
					bool toProductChanged = false;

					try 
					{ 
						productChanged = !Request.Params[productFieldName].Equals ( ( trans.LineItems[0] as LineItemDO ).Product == null ? "" : ( trans.LineItems[0] as LineItemDO ).Product ); 
					}
					catch (Exception) 
					{ 
					}

					if (trans.GetType ( ) == typeof ( RegradeDO ))
					{
						try { fromProductChanged = !Request.Params[fromProductFieldName].Equals ( ( trans.LineItems[0] as RegradeLineItemDO ).Product == null ? "" : ( trans.LineItems[0] as RegradeLineItemDO ).Product ); }
						catch (Exception) { }
						try { toProductChanged = !Request.Params[toProductFieldName].Equals ( ( trans.LineItems[0] as RegradeLineItemDO ).ToProduct == null ? "" : ( trans.LineItems[0] as RegradeLineItemDO ).ToProduct ); }
						catch (Exception) { }
					}

					clearList = new List<Common.FieldControls> ( );
					if (productChanged)
					{
						clearList.Add ( Common.FieldControls.LINEITEM_STORAGELOCATIONID );

						if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_DEFUEL ))
						{
							clearList.Add ( Common.FieldControls.DESTINATIONEQUIPMENTFG1 );
						}

						if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_SALE_AVIATION ))
						{
							clearList.Add ( Common.FieldControls.SOURCEEQUIPMENTFG1 );
						}
					}

					if (fromProductChanged)
					{
						clearList.Add ( Common.FieldControls.LINEITEM_FROMSTORAGELOCATIONID );
					}

					if (toProductChanged)
					{
						clearList.Add ( Common.FieldControls.LINEITEM_TOSTORAGELOCATIONID );
					}

					if (productChanged && trans.Alias.ToUpper ( ).Equals ( ALIAS_TANK_TO_TANK ))
					{
						clearList.Add ( Common.FieldControls.LINEITEM_TOSTORAGELOCATIONID );
						clearList.Add ( Common.FieldControls.LINEITEM_FROMSTORAGELOCATIONID );
					}

					foreach (Common.FieldControls fieldControl in clearList)
					{
						if (fieldControl == Common.FieldControls.LINEITEM_STORAGELOCATIONID ||
						   fieldControl == Common.FieldControls.LINEITEM_TOSTORAGELOCATIONID ||
						   fieldControl == Common.FieldControls.LINEITEM_FROMSTORAGELOCATIONID)
						{
							// product was changed, clear tank number
							string tankFieldName = Common.FieldControlName ( fieldControl );
							FMTankTextBox tankCtrl = FieldTable.FindControl ( tankFieldName ) as FMTankTextBox;

							if (tankCtrl != null)
							{
								tankCtrl.Text = "";

								switch (fieldControl)
								{
									case Common.FieldControls.LINEITEM_STORAGELOCATIONID:
										( trans.LineItems[0] as LineItemDO ).StorageLocationID = "";
										(trans.LineItems[0] as LineItemDO).StorageLocationTankGuid = Guid.Empty;
										break;
									case Common.FieldControls.LINEITEM_TOSTORAGELOCATIONID:
										if (trans.GetType ( ) == typeof ( RegradeDO ))
										{
											( trans.LineItems[0] as RegradeLineItemDO ).ToStorageLocation = "";
											(trans.LineItems[0] as RegradeLineItemDO).ToStorageLocationTankGuid = Guid.Empty;
										}
										break;
									case Common.FieldControls.LINEITEM_FROMSTORAGELOCATIONID:
										if (trans.GetType ( ) == typeof ( RegradeDO ))
										{
											( trans.LineItems[0] as RegradeLineItemDO ).StorageLocationID = "";
											(trans.LineItems[0] as RegradeLineItemDO).StorageLocationTankGuid = Guid.Empty;
										}
										break;
								}
							}
						}
						else if (fieldControl == Common.FieldControls.DESTINATIONEQUIPMENTFG1 || 
								fieldControl == Common.FieldControls.SOURCEEQUIPMENTFG1)
						{
							string equipmentFieldName = Common.FieldControlName ( fieldControl );
							FMEquipmentTextBox equipmentCtrl = FieldTable.FindControl ( equipmentFieldName ) as FMEquipmentTextBox;

							EquipmentDO equipment = null;
							switch (fieldControl)
							{
								case Common.FieldControls.SOURCEEQUIPMENTFG1:
									if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_ISSUE_AVIATION ))
									{
										equipment = trans.SourceEQ1;
									}
									break;
								case Common.FieldControls.DESTINATIONEQUIPMENTFG1:

									if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_FILL_STAND ))
									{
										equipment = trans.DestinationEQ1;
									}
									break;
							}

							if (equipmentCtrl != null && equipment != null)
							{
								equipmentCtrl.Text = "";

								if (equipment != null)
								{
									equipment.RegistrationID = "";
									equipment.EquipmentGuid = Guid.Empty;
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				base.ErrorHandler ( e );
			}
		}

		protected override void BindControls ( )
		{
			try
			{
				// JS20100902 WI-17457 In ADF, we need to aggregate prior to price calculation due to price list entry (aka standing offer) dependency
				if (trans.Alias.ToUpper ( ).Equals ( ALIAS_FUEL_ORDER ))
				{
					for (int i = 0; i < trans.LineItems.Count; ++i)
					{
						AggregateAssociatedTxValues ( i, false );
					}
				}

				// WI-15699 This defect was caused by the removal of price calculation from base BindControls() so one must be used 
				// in ADFWebApp to ensure virtual fields are calculated on view or edit of a transaction.
				FMChannelFactory<IPriceCalculatorInvoker> priceInvokerClient = new FMChannelFactory<IPriceCalculatorInvoker> ( );
				IPriceCalculatorInvoker priceCalculator = priceInvokerClient.CreateProxy ( );

				priceCalculator.Calculate(security, trans);
			}
			catch (Exception)
			{
				// Do nothing. The price calculator error is being caught in RetrieveLineItems() method
				// and the error is being displayed there.  We need to look into why we need to call
				// the price calculator here! Since someone added throw exceptions in the price calculator
				// this try/catch is necessary.
			}

			base.BindControls ( );
		}
		protected override void EnableFieldTable ( bool Enable, bool a_ignoreCloseoutStatus )
		{
			bool closeoutStatus = this.trans.PartialCloseout;

			// JS20100427 physical inventories can never be reversed
			//this.NewLineItemButton.Enabled = !closeoutStatus;
			//this.SetReverseButton(!closeoutStatus);
			//this.SetReverseUpdateButton(!closeoutStatus);
			this.SaveButton.Enabled = true; // save button always enabled regardless of closeout (so that the roman number can be saved)

			bool hasBeenAssociated = false;

			// checks if the currently viewed demand is associated by another transaction (i.e. receipt or fuel order)
			if (this.trans.TransTypeID == TransactionTypes.T9_Request && this.trans.TransVersion != 0)
			{
				try
				{
					AssociatedTxSR sr = new AssociatedTxSR ( );
					sr.Security = base.security;
					sr.RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions;
					sr.TransID = base.trans.TransID;

					FMChannelFactory<IAssociatedTxProcessor> assocTxProcessorClient = new FMChannelFactory<IAssociatedTxProcessor> ( );
					IAssociatedTxProcessor assocTxProcessor = assocTxProcessorClient.CreateProxy ( );

					AssociatedTxListDO atxList = assocTxProcessor.Process ( sr );
					hasBeenAssociated = ( atxList.AssociatedTransactions.Tables[0].Rows.Count > 0 );
				}
				catch (Exception e)
				{
					base.ErrorHandler ( e );
				}
			}

			base.EnableFieldTable ( Enable && !hasBeenAssociated, a_ignoreCloseoutStatus );

			// roman number field on sales and receipts will only be read only when there is something in there
			if (base.trans.TransTypeID == TransactionTypes.T8_Receipt || // receipt
				base.trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement // sale and issue
				)
			{
				if (string.IsNullOrEmpty ( base.trans.UserData3 ))
				{
					// name of the roman number field
					string userDataFieldName = Common.FieldControlName ( Common.FieldControls.USERDATA3 );
					Control userDataCtrl = base.FieldTable.FindControl ( userDataFieldName );
					if (userDataCtrl != null)
					{
						Common.GetControlDelegate ( userDataCtrl.GetType ( ) ) ( true, ref userDataCtrl );
					}
				}
			}
		}

		protected override void Page_PreRender ( object sender, EventArgs e )
		{
			// Set some of the direct fuel purchase or commercial fields since they are virtual.
			this.SetDirectFuelPurchaseAndCommercialPriceFields ( );

			if (Session[Flag05FG.CLIENT_SIDE_SCRIPT_FLAG05] != null)
			{
				this.ClientScript.RegisterStartupScript ( this.GetType ( ),
														Flag05FG.CLIENT_SIDE_KEY_FLAG05,
														Session[Flag05FG.CLIENT_SIDE_SCRIPT_FLAG05] as string );
			}

			if (Session[Flag06FG.CLIENT_SIDE_SCRIPT_FLAG06] != null)
			{
				this.ClientScript.RegisterStartupScript ( this.GetType ( ),
														Flag06FG.CLIENT_SIDE_KEY_FLAG06,
														Session[Flag06FG.CLIENT_SIDE_SCRIPT_FLAG06] as string );
			}

			base.Page_PreRender ( sender, e );

			if (Request.Params.Get ( "__MYEVENTTARGET" ) == "ASSOCIATIONS_CHANGED")
			{
				try
				{
					AggregateAssociatedTxValues ( System.Convert.ToInt32 ( Request.Params.Get ( "__MYEVENTARGUMENT" ) ), true );
				}
				catch
				{
				}
			}

			// Set the default states for specific fields on each transactions.
			this.SetDefaultFieldStates ( );
			if (!canModify ( ))
			{
				DisableButtonsForEditing ( );
				this.CloseButton.Enabled = true;
			}
		}
		#endregion // Must Overrides

		#region Event Overrides

		protected override bool Save ( )
		{
			TransactionDO origTransaction = null;
			TransactionDO savingTransaction = base.trans;

			// get the originanl transaction, we need this to compare certain differences such as deciding
			// weather or not to update the transaction date time, also used for WAC to determine the WAC
			// modification rules, and checks in changes in usability
			try
			{
				if (this.trans.TransVersion != 0)
				{
					origTransaction = this.LoadTransaction ( this.trans.TransID ); // JS20091125
				}
			}
			catch (Exception)
			{
				origTransaction = null; // original transaction do not exist
			}

			// if new transaction, then should set transaction date time
			SiteTimeConverter converter = new SiteTimeConverter ( transContext.accountingSite.CurrentSite );
			if (origTransaction == null)
			{
				// need to convert to site time because TransactionDO defaults to storing datetime objects in site
				// time format then convert it on DB save/load
				base.trans.TransactionDateTime = converter.ConvertToSiteTime ( DateTime.UtcNow );
			}

			// for defuels and returns, if just set to usable then set the transaction and inventory dates to now
			if (base.trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_DEFUEL ) ||
			   base.trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_RETURN ))
			{
				// generate a list of line item ID's where they were originally usable
				List<long> origUsables = new List<long> ( );
				if (origTransaction != null)
				{
					foreach (LineItemDO li in origTransaction.LineItems)
					{
						if (li.Quality == TransactionQuality.Usable)
						{
							origUsables.Add ( li.TransactionLineItemGuid );
						}
					}
				}

				// now look at the current transaction line items and only change dates if the original transaction was not usable
				// but is now.
				foreach (LineItemDO li in trans.LineItems)
				{
					if (li.Quality == TransactionQuality.Usable && !origUsables.Contains ( li.TransactionLineItemGuid ))
					{
						// a line item was changed to usable, set transaction and inventory dates
						base.trans.TransactionDateTime = converter.ConvertToSiteTime ( DateTime.UtcNow );
						base.trans.InventoryDate = base.trans.TransactionDateTime.Value;

						break;
					}
				}
			}

			bool success = base.Save ( );

			if (success)
			{
				// only save the WAC if the transaction has been saved successfully
				this.SaveWAC ( savingTransaction, origTransaction );

				base.trans = this.LoadTransaction ( trans.TransID );
				Session[TransKey] = trans;

				// for these transactions, the fuel price fields defaults to empty. The WAC will need to get forced in.
				if (trans.Alias.ToUpper ( ).Equals ( ALIAS_FILL_STAND ) ||
				   trans.Alias.ToUpper ( ).Equals ( ALIAS_TANK_TO_TANK ) ||
				   trans.Alias.ToUpper ( ).Equals ( ALIAS_RETURN_TO_BULK ))
				{
					// force transaction price for these single line item transactions
					if (trans.LineItems.Count == 1)
					{
						string producePriceName = Common.FieldControlName ( Common.FieldControls.LINEITEM_PRODUCTPRICE );
						TextBox tb = FieldTable.FindControl ( producePriceName ) as TextBox;
						if (tb != null && ( trans.LineItems[0] as LineItemDO ).ProductPrice != null)
						{
							tb.Text = ( trans.LineItems[0] as LineItemDO ).ProductPrice.Value.ToString ( "N" );
						}
					}
				}

				// need to release now-deleted previously-associated transactions (if any)
				List<Guid> curLineItemGuids = new List<Guid> ( );
				List<Guid> delLineItemGuids = new List<Guid> ( );
				if (origTransaction != null)
				{
					foreach (LineItemDO li in base.trans.LineItems)
					{
						curLineItemGuids.Add ( li.TransactionLineItemGuid );
					}
					foreach (LineItemDO li in origTransaction.LineItems)
					{
						if (!curLineItemGuids.Contains ( li.TransactionLineItemGuid ))
						{
							// record the line item ID that has been deleted
							delLineItemGuids.Add ( li.TransactionLineItemGuid );
						}
					}

					if (delLineItemGuids.Count > 0)
					{
						try
						{
							TransactionLinkSR sr = new TransactionLinkSR ( );
							sr.Security = base.security;
							sr.OriginalLineItemGuids = delLineItemGuids;
							sr.PerformAction = TransactionLinkSR.Action.DELETE_LINEITEM_LINKS;

							FMChannelFactory<ITransactionLinkProcessor> linkClient = new FMChannelFactory<ITransactionLinkProcessor> ( );
							ITransactionLinkProcessor linkProcessor = linkClient.CreateProxy ( );
							linkProcessor.Process ( sr );
						}
						catch (Exception ex)
						{
							base.ErrorHandler ( ex );
						}
					}
				}
			}

			return success;
		}

		protected override void ReverseProcessing ( )
		{
			string origTransID = trans.TransID;

			// JS20100601 WI-14886 must re-calculate for WAC changes.
			trans.TransVersion = 0;
			trans.ReversalType = TransactionDO.Reversal;
			trans.TransID = FuelsManagerId.NewId ( );

			// must reverse the quantities to emulate the "old" price calculation behaviour before all the
			// TransactionDetail structural changes were implemented
			foreach (LineItemDO lineItem in trans.LineItems)
			{
				lineItem.Quantity.GrossInventoryChange *= -1;
				lineItem.Quantity.NetInventoryChange *= -1;
			}

			FMChannelFactory<IPriceCalculatorInvoker> invokerClient = new FMChannelFactory<IPriceCalculatorInvoker> ( );
			IPriceCalculatorInvoker invoker = invokerClient.CreateProxy ( );
			invoker.Calculate(this.security, trans);

			// restore origTransID, see comments before price calculation
			trans.TransID = origTransID;

			// restore values because base will redo it, see comments before price calculation
			foreach (LineItemDO lineItem in trans.LineItems)
			{
				lineItem.Quantity.GrossInventoryChange *= -1;
				lineItem.Quantity.NetInventoryChange *= -1;
			}

			base.ReverseProcessing ( );
		}

		protected override void EnterKeyButton_Click ( object sender, EventArgs e )
		{
			NewButton.Enabled = NewButton.Enabled && transContext.mode != TransactionContext.Mode.Add;

			// restore values because base will redo it, see comments before price calculation
			foreach (LineItemDO lineItem in trans.LineItems)
			{
				lineItem.Quantity.GrossInventoryChange *= -1;
				lineItem.Quantity.NetInventoryChange *= -1;
			}
		}

		/// <summary>
		/// This method will handle the Save button event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void SaveButton_Click ( object sender, EventArgs e )
		{
			string errorMsg = this.ValidateLineItemFields ( );
			if (errorMsg.Length > 0)
			{
				base.HandleFieldError ( new Exception ( errorMsg ) );
				return;
			}

			// Must retrieve the data from the page prior to performing any field checking.
			base.noSaveErrors = RetrieveDataFromPage ( );

			if (base.noSaveErrors)
			{
				// copied directly from base, can't use the event because we need the return value of SaveButtonProcessing
				base.noSaveErrors = true;

				// CCP-043 Inventory date must be the same as the transaction date
				if (base.trans.TransactionDateTime != null)
				{
					base.trans.InventoryDate = base.trans.TransactionDateTime.Value;
				}

				// JS20100520 If current transaction is an invoice, must ensure currency units are consistent
				if (base.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
				{
					Guid currencyGuid = Guid.Empty;
					bool firstOne = true;
					foreach (LineItemDO lineItemDO in base.trans.LineItems)
					{
						if (firstOne)
						{
							currencyGuid = lineItemDO.CurrencyGuid;
							firstOne = false;
						}
						else if (lineItemDO.CurrencyGuid != Guid.Empty &&
						   ( lineItemDO.CurrencyGuid != currencyGuid ))
						{
							base.ErrorHandler ( new Exception ( "all line items must have the same currency unit" ) );
							return;
						}
					}
				}

				// JS20100427 If current transaction is a physical inventory then must enforce uniqueness on site/tank
				if (base.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory)
				{
					ArrayList tankProductList = new ArrayList ( );

					ExistTransactionAssetSR existSr = new ExistTransactionAssetSR ( );
					existSr.Security = security;
					existSr.SiteGuid = security.SiteGuid;
					existSr.AliasName = trans.Alias;
					existSr.InventoryDate = trans.InventoryDate;

					foreach (LineItemDO lineItemDO in base.trans.LineItems)
					{
						// client check
						string key = lineItemDO.Product + " " + lineItemDO.StorageLocationID;
						if (tankProductList.Contains ( key ))
						{
							throw new Exception ( "There is already another line item with the same fuel type and tank" );
						}

						tankProductList.Add ( key );

						// server check
						existSr.Product = lineItemDO.Product;
						existSr.Tank = lineItemDO.StorageLocationID;
						existSr.TransactionLineItemGuid = (int) lineItemDO.TransactionLineItemGuid;

						try
						{
							FMChannelFactory<IExistTransactionAssetProcessor> existTransClient = new FMChannelFactory<IExistTransactionAssetProcessor> ( );
							IExistTransactionAssetProcessor existTransProcessor = existTransClient.CreateProxy ( );

							IntegerDO duplicateCount = existTransProcessor.Process ( existSr ) as IntegerDO;

							if (duplicateCount.Value > 0)
							{
								throw new Exception ( "There is already another line item with the same fuel type and tank" );
							}
						}
						catch (Exception ex)
						{
							base.ErrorHandler ( ex );
							return;
						}
					}
				}

				// This check will determine if the direct fuel purchase number is unique
				// only for type 12 transactions (i.e. Direct Fuel Purchase and Commercial).
				if (this.IsDirectFuelPurchaseUnique ( ) == false)
				{
					Exception exception = new Exception ( this.errorMsg );
					base.ErrorHandler ( exception );
				}
				else
				{
					bool fuelPurchaseValidationOk = this.UpdateDirectFuelPurchaseCommercialFields ( );

					if (fuelPurchaseValidationOk == false)
					{
						Exception exception = new Exception ( this.errorMsg );
						base.ErrorHandler ( exception );
					}
					else
					{
						if (this.SaveProcessing ( sender ) == true)
						{
							this.PerformDrawdown ( );

							// this.RegenerateControls(true);
						}
					}
				}
			}
			else
			{
				EventLog eventLog = new EventLog ( "Application", ".", "FuelsManager" );
				eventLog.WriteEntry ( "TransactionDetail.aspx: Error retrieving data from transaction page", EventLogEntryType.Warning );
			}
		}

		/// <summary>
		/// This method overrides the base class save processing method. The main difference is that 
		/// the data is already retrieved from the page.
		/// </summary>
		/// <param name="sender"></param>
		/// <returns></returns>
		protected override bool SaveProcessing ( object sender )
		{
			// Save the data to the database
			if (this.noSaveErrors == true)
			{
				bool successfulSave = Save ( );

				if (successfulSave == true)
				{
					this.transContext.mode = TransactionFields.TransactionContext.Mode.Edit;
					Session.Add ( TransactionDetailBase.ModeKey, this.transContext.mode );
					this.SetButtons ( );
				}
				else
				{
					return false;
				}
			}

			return this.noSaveErrors;
		}

		/// <summary>
		/// This method handles the New Button click event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void NewButton_Click ( object sender, EventArgs e )
		{
			this.SaveButton_Click ( sender, e );

			// Process the new button for ADF specific implementation. It will create
			// a new transaction data object.
			this.ADFNewButtonProcess ( );
		}

		protected string ValidateLineItemFields ( )
		{
			string returnVal = "";

			try
			{

				if (base.LineItemDataGrid.SelectedItem != null)
				{
					int editIndex = base.LineItemDataGrid.SelectedIndex;
					if (base.trans.LineItems.Count > editIndex && // defensive programming
							( base.trans.TransTypeID == TransactionTypes.T8_Receipt ||
							base.trans.Alias.ToUpper ( ).Equals ( ALIAS_FUEL_ORDER ) )
						)
					{
						// JS20110210 Removal of code for RAAF Pearce
						// CCP-042 Receipts - must have a demand associated
						/*LineItemDO lineItem = base.trans.LineItems[editIndex] as LineItemDO;
						returnVal = lineItem.AssociatedTransactions.Count > 0 ? "" :
							"Line item must have a demand associated.";*/

						// JS20101005 WI-18235 cannot enter zero quantity
						string grossVolumeName = Common.FieldControlName ( Common.FieldControls.LINEITEM_GROSSQUANTITY );
						TextBox grossTextBox = base.LineItemDataGrid.SelectedItem.FindControl ( grossVolumeName ) as TextBox;
						if (grossTextBox != null)
							returnVal = ( grossTextBox.Text.Length == 0 || grossTextBox.Text == "0" ) ?
								  "The receipt quantity cannot be zero." : "";
					}

					// CCP-042 Demands - cannot apply line item if delivery location is set to none
					else if (base.trans.TransTypeID == TransactionTypes.T9_Request)
					{
						string dellocName = Common.FieldControlName ( Common.FieldControls.LINEITEM_DELIVERYLOCATION );
						HtmlSelect ddl = base.LineItemDataGrid.SelectedItem.FindControl ( dellocName ) as HtmlSelect;

						if (ddl != null)
						{
							int selIndex = ddl.SelectedIndex;
							returnVal = ddl.Items[selIndex].Text.ToUpper ( ).Equals ( "NONE" ) ?
								"Please select a value other than None for delivery location." : "";
						}
					}
				}
			}
			catch (Exception)
			{ }

			return returnVal;
		}

		protected override void LineItemDataGrid_UpdateCommand ( object source, DataGridCommandEventArgs e )
		{
			// preprocessing - perform some update on ADF specific field requirements
			string errorMsg = this.ValidateLineItemFields ( );
			if (errorMsg.Length > 0)
			{
				base.HandleFieldError ( new Exception ( errorMsg ) );
				return;
			}

			base.LineItemDataGrid_UpdateCommand ( source, e );
		}

		protected override bool RetrieveLineItem ( DataGridItem item )
		{
			bool ok = base.RetrieveLineItem ( item );
			if (ok)
			{
				int lineItemIndex;
				int sublineItemIndex;

				GetItemIndices ( item, out lineItemIndex, out sublineItemIndex );

				LineItemDO lineItem = trans.LineItems[lineItemIndex] as LineItemDO;
				foreach (WebControl control in item.Controls)
				{
					if (control.ID == null)
					{
						continue;
					}

					// Check to see if this is line item user data 14 (on-cost)
					if (control.ID.StartsWith ( "TALUD14" ) && control.Controls.Count > 0)
					{
						TextBox tb = (TextBox) control.Controls[0];
						if (tb != null)
						{
							double oncost = 0.0;

							// convert to number first so we can format it properly
							try
							{
								oncost = double.Parse ( tb.Text );
							}
							catch (Exception)
							{
								oncost = 0.0;
							}
							lineItem.UserData["TALUD14"] = oncost.ToString ( "N" );
						}
					}
				}
			}

			return ok;
		}

		protected override void LineItem_ItemDataBound ( object sender, System.Web.UI.WebControls.DataGridItemEventArgs e )
		{
			base.LineItem_ItemDataBound ( sender, e );

			if (e.Item.ItemIndex != -1)
			{
				// do custom single demand association button
				FMElipseButton assocTrans = e.Item.FindControl ( "btnAddAssocTx" ) as FMElipseButton;
				FMElipseButton assocSingleTrans = e.Item.FindControl ( "btnAddAssocSingleTx" ) as FMElipseButton;

				int lineItemIndex = 0;
				int sublineItemIndex = 0;
				GetItemIndices ( e.Item, out lineItemIndex, out sublineItemIndex );
				LineItemDO LineItem = (LineItemDO) base.trans.LineItems[lineItemIndex];

				// prevent user from deleting a line item is the transaction or line item is marked as complete
				LinkButton DeleteButton = (LinkButton) e.Item.FindControl ( "DeleteButton" );
				if (( DeleteButton != null && base.trans.TransVersion != 0 &&
					( base.trans.Status == TransactionStatus.Completed || LineItem.Status == TransactionStatus.Completed ) )
					|| ( base.trans.TransTypeID == TransactionTypes.T8_Receipt && LineItem.Quality == TransactionQuality.Usable && LineItem.TransactionLineItemGuid >= 0 )
					|| base.trans.PartialCloseout
				   || this.HasParentAssociations ( base.trans, LineItem )
					)
				{
					DeleteButton.Enabled = false ||
					   ( base.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory ) && base.trans.InventoryDate.Date.Equals ( DateTime.UtcNow.Date );
					assocTrans.Enabled = false;
					assocSingleTrans.Enabled = false;
				}

				// fuel order uses single select...
				if (assocSingleTrans != null &&
					( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_FUEL_ORDER ) || base.trans.TransTypeID == TransactionTypes.T8_Receipt )
					)
				{
					assocSingleTrans.Visible = true;

					assocSingleTrans.OnClick = "AssociateSingleTx(" + e.Item.ItemIndex.ToString ( ) + ")";

					if (assocTrans != null)
					{
						assocTrans.Visible = false;
					}
				}
				// for all other transactions...
				else if (assocTrans != null)
				{
					assocTrans.Visible = true;

					if (assocSingleTrans != null)
					{
						assocSingleTrans.Visible = false;
					}
				}
			}
		}

		protected override void NewLineItemButton_Click ( object sender, EventArgs e )
		{
			if (this.RetrieveDataFromPage ( ) == false)
			{
				return;
			}

			// Disable all buttons for line item edit processing.
			// JS20100511 moved this up here to avoid race problems
			this.DisableButtonsForEditing ( );

			LineItemDO lineItem;
			if (( trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade ) ||
				( trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade ))
			{
				lineItem = new RegradeLineItemDO ( );
			}
			else if (trans.TransTypeID == TransactionTypes.T23_StorageTransfer)
			{
				lineItem = new StorageTransferLineItemDO ( );
			}
			else
			{
				lineItem = new LineItemDO ( );

			}

			//The code used to generate a negative line item ID is to indicate that lineItem is not yet stored in database.
			//It was used to differentiate newly added line items when viewing the associated 
			//transactions for a specific line.
			//We now generate a new Guid and set a flag
			lineItem.TransactionLineItemGuid = Guid.NewGuid(); // was -(trans.LineItems.Count + 1);
			lineItem.IsNewLineItem = true;

			if (base.OrderReferenceID != "" && this.OrderReferenceID != null)
			{
				// Set the product
				lineItem.Product = this.OrderProduct;
				lineItem.ProductCode = this.OrderProductCode;
				lineItem.ProductGuid = OrderProductIndex;
			}

			if (this.trans.TransTypeID == TransactionTypes.T8_Receipt)
			{
				// JS20100203 CCP-042
				lineItem.Quality = TransactionQuality.Quarantined;
			}

			if (( this.trans.TransTypeID == TransactionTypes.T17_Order ) ||
			   ( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_FUEL_ORDER ) ))
			{
				// JS20100203 CCP-042
				lineItem.Status = TransactionStatus.InProgress;
			}
			else
			{
				if (this.transContext.DefaultStatus != -1)
				{
					lineItem.Status = (TransactionStatus) this.transContext.DefaultStatus;
				}

			}

			// vthompson - 8/12/2008
			// For an AR invoice auto-populate the line item's invoice number
			if (trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
				ISites sites = sitesClient.CreateProxy ( );

				lineItem.InvoiceNumber = sites.GetNextInvoiceNumber ( this.security );
			}

			Session[TransactionDetailBase.SESSION_LINE_ITEM_OBJECT] = lineItem;

			trans.LineItems.Add ( lineItem );

			LineItemDataGrid.SelectedIndex = LineItemDataGrid.Items.Count;
			LineItemDataGrid.EditItemIndex = LineItemDataGrid.Items.Count;
			lineItemGridGenerator.Bind ( );
			// always ignore closeout status regardless of inc
			EnableFieldTable ( false, false );

			Session.Add ( TransactionDetailBase.SESSION_LINE_ITEM_ADDED, LineItemDataGrid.EditItemIndex );
		}

		protected override void CloseButtonClick ( object sender, EventArgs e )
		{
			this.Close ( );
		}

		#endregion // Event Overrides

		#region Other Overrides

		protected override bool RetrieveDataFromPage ( )
		{
			bool result = base.RetrieveDataFromPage ( );

			string onCostString = "";
			if (transContext.aliasClass.MultipleLineItems == false
				 && trans.LineItems.Count > 0)
			{
				LineItemDO lineItem = trans.LineItems[0] as LineItemDO;

				//TextBox txtOnCost = (TextBox)FieldTable.FindControl("TransactionFields.UserDataTextFGTALUD14");
				TextBox txtOnCost = (TextBox) FieldTable.FindControl ( Common.FieldControlName ( Common.FieldControls.LINEITEM_ONCOST ) );
				if (txtOnCost != null)
				{
					//lineItem.UserData["TALUD14"] = txtOnCost.Text;
					onCostString = txtOnCost.Text;
				}
			}

			if (transContext.aliasClass.MultipleLineItems == false
				&& trans.LineItems.Count > 0)
			{
				LineItemDO lineItem = trans.LineItems[0] as LineItemDO;

				//TextBox txtOnCost = (TextBox)FieldTable.FindControl("TransactionFields.UserDataTextFGTALUD14");
				TextBox txtOnCost = (TextBox) FieldTable.FindControl ( Common.FieldControlName ( Common.FieldControls.LINEITEM_ONCOST ) );
				if (txtOnCost != null)
				{
					//string onCostStr = lineItem.UserData["TALUD14"] as string;
					if (!string.IsNullOrEmpty ( onCostString ))
					{
						try
						{
							lineItem.UserData["TALUD14"] = onCostString;
							txtOnCost.Text = double.Parse ( onCostString ).ToString ( "F", CultureInfo.InvariantCulture );
						}
						catch (Exception)
						{
							txtOnCost.Text = "0.00";
						}
					}
				}
			}

			return result;
		}

		protected bool HasAssociations ( TransactionDO a_trans, LineItemDO a_lineItem )
		{
			return HasParentAssociations ( a_trans, a_lineItem ) || a_lineItem.AssociatedTransactions.Count > 0;
		}

		protected bool TransHasParentAssociations ( TransactionDO a_trans )
		{
			bool returnVal = false;

			if (a_trans.TransVersion != 0)
			{
				foreach (LineItemDO li in a_trans.LineItems)
				{
					if (HasParentAssociations ( a_trans, li ))
					{
						returnVal = true;
						break;
					}
				}
			}

			return returnVal;
		}

		protected bool HasParentAssociations ( TransactionDO a_trans, LineItemDO a_lineItem )
		{
			bool returnVal = false;

			// check transaction has been saved
			if (a_trans.TransVersion != 0 && a_lineItem.TransactionLineItemGuid >= 0)
			{
				// have to do it every time because another user could be accessing it
				try
				{
					AssociatedTxSR sr = new AssociatedTxSR ( );
					sr.Security = base.security;
					sr.TransID = base.trans.TransID;
					sr.TransactionLineItemGuid = (int) a_lineItem.TransactionLineItemGuid;
					sr.RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions;

					FMChannelFactory<IAssociatedTxProcessor> assocTxProcessorClient = new FMChannelFactory<IAssociatedTxProcessor> ( );
					IAssociatedTxProcessor assocTxProcessor = assocTxProcessorClient.CreateProxy ( );

					AssociatedTxListDO results = assocTxProcessor.Process ( sr );

					if (results.AssociatedTransactions.Tables != null)
					{
						if (results.AssociatedTransactions.Tables.Count <= 0)
						{
							returnVal = false;
						}
						else if (results.AssociatedTransactions.Tables[0].Rows.Count > 0)
						{
							returnVal = true;
						}
					}

					// for invoices, extra check it's associated to a bulk payment
					if (a_trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
					{
						FMChannelFactory<IBulkPaymentInvoiceMappings> bpInvMappingClient = new FMChannelFactory<IBulkPaymentInvoiceMappings> ( );
						IBulkPaymentInvoiceMappings mappings = bpInvMappingClient.CreateProxy ( );
						BulkPaymentInvoiceMappingClass mapping = mappings.EnumerateByInvoiceTransID ( security, a_trans.TransID );

						if (mapping != null)
						{
							returnVal = true;
						}
					}
				}
				catch (Exception e)
				{
					base.ErrorHandler ( e );
				}
			}

			return returnVal;
		}

		protected void SetDefaultFieldStates ( )
		{
			bool isNewReverse = false;

			// workout the current datetime
			SiteTimeConverter converter = new SiteTimeConverter ( transContext.accountingSite.CurrentSite );
			DateTime now = converter.ConvertToSiteTime ( DateTime.UtcNow );

			// ensures the inventory and transaction dates match on screen
			if (trans != null)
			{
				// new reversals should not have fields disabled
				if (trans.ReversalType.Equals ( "U" ) && transContext.mode == TransactionContext.Mode.Add)
				{
					isNewReverse = true;
				}

				if (isNewReverse && trans.TransactionDateTime != null)
				{
					base.trans.TransactionDateTime = now;
				}

				if (trans.TransactionDateTime != null)
				{
					trans.InventoryDate = trans.TransactionDateTime.Value;
				}
			}

			string adfDateTimeName = Common.FieldControlName ( Common.FieldControls.ADFTRANSACTIONDATETIME );
			FMDateTime adfDateTimeCtrl = FieldTable.FindControl ( adfDateTimeName ) as FMDateTime;
			if (adfDateTimeCtrl != null)
			{
				adfDateTimeCtrl.Text = transContext.accountingSite.FormatDateTime (
					  trans.TransactionDateTime == null ? now : trans.TransactionDateTime.Value );
			}

			// set line item delete buttons
			for (int i = 0; i < base.LineItemDataGrid.Items.Count; ++i)
			{
				LineItemDO li = base.trans.LineItems[i] as LineItemDO;

				LinkButton DeleteButton = (LinkButton) base.LineItemDataGrid.Items[i].FindControl ( "DeleteButton" );
				if (( DeleteButton != null && base.trans.TransVersion != 0 &&
					( base.trans.Status == TransactionStatus.Completed || li.Status == TransactionStatus.Completed ) )
					|| ( base.trans.TransTypeID == TransactionTypes.T8_Receipt && li.Quality == TransactionQuality.Usable && li.TransactionLineItemGuid >= 0 )
					|| base.trans.PartialCloseout
				   || this.HasParentAssociations ( base.trans, li )
					)
				{
					DeleteButton.Enabled = false ||
					   ( base.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory ) && base.trans.InventoryDate.Date.Equals ( now.Date );
				}
			}

			// only able to delete a physical inventory of current date
			bool deleteButtonVisible = ( base.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory )
					&& base.trans.InventoryDate.Date.Equals ( now.Date );
			base.DeleteButton.Visible = deleteButtonVisible;

			// ------------- SET UP CONTROL LOOKUP TABLES -------------

			// <FieldControl, Control> - maps field control with the actual control instance
			Hashtable controlTable = new Hashtable ( );
			// <FieldControl, VBool> - maps field control and whether or not it will be enabled
			Hashtable controlStateTable = new Hashtable ( );

			int editIndex = LineItemDataGrid.SelectedIndex;

			// find and retrieve all the control handles on the transaction
			foreach (Common.FieldControls control in Enum.GetValues ( typeof ( Common.FieldControls ) ))
			{
				controlTable[control] = base.FieldTable.FindControl ( Common.FieldControlName ( control ) );
				if (controlTable[control] == null && base.LineItemDataGrid.EditItemIndex != -1)
				{
					controlTable[control] = base.LineItemDataGrid.Items[base.LineItemDataGrid.EditItemIndex]
							.FindControl ( Common.FieldControlName ( control ) );
				}

				controlStateTable[control] = null;
			}

			List<string> assocTxButtonList = new List<string> ( ) { "btnAddAssocTx", "btnAddAssocSingleTx" };

			// ------------- FIGURE OUT CONTROL STATES -------------

			// for all transactions, make transaction datetime read only
			//Control transDateTimeCtrl = controlTable[Common.FieldControls.TRANSACTIONDATE] as Control;
			List<Common.FieldGroups> groupList = new List<Common.FieldGroups> ( )
			{
				Common.FieldGroups.TRANSACTIONDATE,
				Common.FieldGroups.INVENTORYDATE
			};
			foreach (Common.FieldGroups fieldGroup in groupList)
			{
				foreach (Common.FieldControls fieldControl in Common.GetFieldControlsFromGroup ( fieldGroup ))
				{
					controlStateTable[fieldControl] = false;
				}
			}

			// for all transactions, these fields should be read only (except bulk purchase order which should be unchanged)
			if (!trans.Alias.ToUpper ( ).Equals ( TransactionDetail.BULK_PURCHASE_ORDER ))
			{
				controlStateTable[Common.FieldControls.DOCUMENTNUMBER] = false;
				controlStateTable[Common.FieldControls.TOTALPRICEAMOUNT] = false;
				controlStateTable[Common.FieldControls.TOTALPRICEWITHTAX] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TOTALPRICEAMOUNT] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TOTALPRICEWITHTAX] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TOTALVALUE] = false;
				controlStateTable[Common.FieldControls.TOTALONCOST] = false;
				controlStateTable[Common.FieldControls.ROSUPPLIER] = false;
				controlStateTable[Common.FieldControls.TOTALFOREIGNPRICE] = false;
				controlStateTable[Common.FieldControls.LINEITEM_CURRENCYUNITLABEL] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TOTALFOREIGNPRICE] = false;
				// WI-15503 prevents total GST and Excise fields from being overwritten
				controlStateTable[Common.FieldControls.TOTALEXCISE] = false;
				controlStateTable[Common.FieldControls.TOTALGST] = false;
				controlStateTable[Common.FieldControls.TOTALMARKUP] = false;
			}

			// fuel price should be disabled on many transactions
			if (trans.Alias.ToUpper ( ).Contains ( "ISSUE" ) ||
				trans.Alias.ToUpper ( ).Contains ( "SALE" ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_SHIPMENT ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_DEFUEL ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_RETURN ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_CONSUMER_TRANSFER ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_ADJUSTMENT ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_REGRADE ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_DISPOSAL ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_FQC ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_TANK_TO_TANK ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_RETURN_TO_BULK ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_PHYSICAL_INVENTORY ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_RECEIPT ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_FILL_STAND ))
			{
				controlStateTable[Common.FieldControls.LINEITEM_PRODUCTPRICE] = false;
			}

			// WI-14910 Mike et al no longer want this, but wanted this code commented just in case they change their mind.
			// if any transaction is cancelled or its line items cancelled, then it cannot be uncancelled
			/* if (base.trans.Status == TransactionStatus.Cancelled && base.trans.TransVersion != 0)
			 {
				controlStateTable[Common.FieldControls.TRANSACTIONSTATUS] = new VBool(false);
			 }
			 if (editIndex >= 0 && trans.LineItems.Count > editIndex)
			 {
				if ((trans.LineItems[editIndex] as LineItemDO).Status == TransactionStatus.Cancelled)
				{
				   controlStateTable[Common.FieldControls.LINEITEM_TRANSACTIONSTATUS] = new VBool(false);
				}
			 }*/

			// if a line item has child associations, then you cannot modify the fuel type of this transaction
			if (editIndex >= 0 && trans.LineItems.Count > editIndex)
			{
				LineItemDO li = trans.LineItems[editIndex] as LineItemDO;
				if (li.AssociatedTransactions.Count > 0)
				{
					controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
				}

				// if has parent associations then the fuel type and quantity should be locked
				if (HasParentAssociations ( base.trans, li ))
				{
					controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
					controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					controlStateTable[Common.FieldControls.LINEITEM_NETQUANTITY] = false;
					controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVEGROSSQUANTITY] = false;
					controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVENETVOLUME] = false;
				}
			}

			// for receipts, do not allow user the change fuel price
			if (base.trans.TransTypeID == TransactionTypes.T8_Receipt)
			{
				NewLineItemButton.Enabled = base.trans.LineItems.Count <= 0;

				//controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = new VBool(false);
				controlStateTable[Common.FieldControls.LINEITEM_RECEIPTVARIANCE] = false;
				controlStateTable[Common.FieldControls.LINEITEM_FOREIGN_CURRENCY] = false;
				controlStateTable[Common.FieldControls.LINEITEM_NON_DOMESTIC_PRICE] = false;

				string roSupplierName = Common.FieldControlName ( Common.FieldControls.ROSUPPLIER );
				TextBox roSupplierTextBox = base.FieldTable.FindControl ( roSupplierName ) as TextBox;
				if (roSupplierTextBox != null)
				{
					roSupplierTextBox.Text = base.trans.SupplierID;
				}

				// receipts have transaction association column
				LineItemDataGrid.Columns[3].Visible = true;

				if (editIndex >= 0 && trans.LineItems.Count > editIndex)
				{
					Hashtable valueTable = new Hashtable ( );
					LineItemDO lineItem = trans.LineItems[editIndex] as LineItemDO;

					valueTable[Common.FieldControls.LINEITEM_TOTALVALUE] = lineItem.TotalValue.ToString ( "N" );

					if (lineItem.Quality == TransactionQuality.Usable)
					{
						valueTable[Common.FieldControls.LINEITEM_TRANSACTIONQUALITY] = false;
					}

					if (( lineItem.Quality == TransactionQuality.Usable && lineItem.TransactionLineItemGuid >= 0 ) ||
						base.trans.PartialCloseout ||
					   HasParentAssociations ( base.trans, lineItem ) // when line item is associated then it should not be editable
						)
					{
						controlStateTable[Common.FieldControls.LINEITEM_TRANSACTIONQUALITY] = false;
						controlStateTable[Common.FieldControls.LINEITEM_SELECTEDQUALITY] = false;
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCTPRICE] = false;
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
						controlStateTable[Common.FieldControls.LINEITEM_NETQUANTITY] = false;

						// JS20100827 WI-14868 commercials and df are exceptions
						if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_COMMERCIAL ) &&
						 !trans.Alias.ToUpper ( ).Equals ( ALIAS_DIRECT_FUEL_PURCHASE ))
						{
							controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVEUNITS] = false;
							controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVEGROSSQUANTITY] = false;
							controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVENETVOLUME] = false;
						}

						if (trans.TransTypeID != TransactionTypes.T3_PrimaryDefuel &&
						   trans.TransTypeID != TransactionTypes.T4_SecondaryDefuel)
						{
							controlStateTable[Common.FieldControls.LINEITEM_TEMPERATURE] = false;
							controlStateTable[Common.FieldControls.LINEITEM_DENSITY] = false;
							controlStateTable[Common.FieldControls.LINEITEM_VCF] = false;
						}
					}

					//if (lineItem.Status == TransactionStatus.Completed)
					//{
					//    valueTable[Common.FieldControls.LINEITEM_TRANSACTIONSTATUS] = new VBool(false);
					//}
				}
			}

			// disable quantity fields from being edited after a transaction is saved and is complete
			// also prevent user from switching back to a status other than complete
			if (( ( base.trans.TransVersion != 0 && base.trans.Status == TransactionStatus.Completed ) || base.trans.PartialCloseout ) &&
				// JS20100428 physical inventories on the current day can still be modified
				!( base.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory && base.trans.InventoryDate.Date.Equals ( now.Date ) ) &&
				// JS20100611 does not affect bulk purchase order (which remains unaffected by CCP-042)
			   !trans.Alias.ToUpper ( ).Equals ( TransactionDetail.BULK_PURCHASE_ORDER ) &&
			   !trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_COMMERCIAL ) &&
			   !trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_DIRECT_FUEL_PURCHASE )
			   )
			{
				if (!isNewReverse)
				{
					controlStateTable[Common.FieldControls.TRANSACTIONSTATUS] = false;
					controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					controlStateTable[Common.FieldControls.LINEITEM_TAX1] = false;
					controlStateTable[Common.FieldControls.LINEITEM_TAX2] = false;
					controlStateTable[Common.FieldControls.LINEITEM_TAX3] = false;

					// JS20100508 added as per M.Hogendoorn's WI assigned to me
					controlStateTable[Common.FieldControls.LINEITEM_NETQUANTITY] = false;
					controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;

					if (trans.TransTypeID != TransactionTypes.T3_PrimaryDefuel &&
						trans.TransTypeID != TransactionTypes.T4_SecondaryDefuel)
					{
						controlStateTable[Common.FieldControls.LINEITEM_TEMPERATURE] = false;
						controlStateTable[Common.FieldControls.LINEITEM_DENSITY] = false;
						controlStateTable[Common.FieldControls.LINEITEM_VCF] = false;
					}

					// JS20100827 WI-14868 commercials are exceptions for UOM related
					if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_COMMERCIAL ) &&
					 !trans.Alias.ToUpper ( ).Equals ( ALIAS_DIRECT_FUEL_PURCHASE ))
					{
						controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVENETVOLUME] = false;
						controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVEUNITS] = false;
						controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVEGROSSQUANTITY] = false;
					}

					if (base.LineItemDataGrid.EditItemIndex != -1)
					{
						controlStateTable[Common.FieldControls.LINEITEM_TRANSACTIONSTATUS] = false;

						foreach (string assocTxButtonName in assocTxButtonList)
						{
							FMControls.FMElipseButton buttonCtrl =
								(FMControls.FMElipseButton) LineItemDataGrid.Items[base.LineItemDataGrid.EditItemIndex].FindControl ( assocTxButtonName );
							if (buttonCtrl != null)
							{
								buttonCtrl.Enabled = false;
							}
						}
					}
				}
			}

			// CCP-043 Disable quantity field
			// CCP-042 Fuel Orders have some special functionalities on association
			if (base.trans.Alias.ToUpper ( ).Equals ( ALIAS_FUEL_ORDER ))
			{
				// no associated transactions, so fuel price should be enabled

				//controlStateTable[Common.FieldControls.LINEITEM_PRODUCTPRICE] = new VBool(true);

				GetFuelOrderReceiptedLineItemsDO lineItemReceipted = null;
				try
				{
					GetFuelOrderReceiptedLineItemsSR fuelOrderLineItemReceiptedSR = new GetFuelOrderReceiptedLineItemsSR ( );
					fuelOrderLineItemReceiptedSR.Security = security;
					fuelOrderLineItemReceiptedSR.TransID = trans.TransID;

					lineItemReceipted = FMChannelHelper.MakeCall<IGetFuelOrderReceiptedLineItemsProcessor, GetFuelOrderReceiptedLineItemsDO>(
																	 x =>
																	 x.Process ( fuelOrderLineItemReceiptedSR )
																);
				}
				catch (Exception e)
				{
					base.ErrorHandler ( e );
				}

				if (lineItemReceipted != null && lineItemReceipted.GetResult ( ).Count > 0)
				{
					List<long> receiptedIndexTable = lineItemReceipted.GetResult ( );

					// go through all the fuel order line items
					for (int i = 0; i < trans.LineItems.Count; ++i)
					{
						// if the current line item has an associated usable receipt, then should
						// prevent it from being removed
						LineItemDO lineItem = trans.LineItems[i] as LineItemDO;
						if (lineItem != null && receiptedIndexTable.Contains ( lineItem.TransactionLineItemGuid ))
						{
							LinkButton deleteButton = base.LineItemDataGrid.Items[i].FindControl ( "DeleteButton" ) as LinkButton;
							if (deleteButton != null)
								deleteButton.Enabled = false;

							// if currently editing a line item, disable the quantities
							if (LineItemDataGrid.EditItemIndex == i)
							{
								controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
								controlStateTable[Common.FieldControls.LINEITEM_PRODUCTPRICE] = false;
							}
						}
					}
				}
			}

			if (base.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice ||
			   base.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				// invoice tax items always aggregated and not edited
				controlStateTable[Common.FieldControls.LINEITEM_TAX1] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TAX2] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TAX3] = false;

				// aggregate foreign currency & oncost totals
				double totalForeign = 0.0;
				double totalOnCost = 0.0;
				foreach (LineItemDO li in trans.LineItems)
				{
					if (li.UserData.ContainsKey ( "TALUD3" ))
					{
						try
						{
							totalForeign += double.Parse ( li.UserData["TALUD3"].ToString ( ) );
						}
						catch (Exception) { }

						try
						{
							totalOnCost += double.Parse ( li.UserData["TALUD14"].ToString ( ) );
						}
						catch (Exception) { }
					}
				}

				string foreignTotalStr = Common.FieldControlName ( Common.FieldControls.TOTALFOREIGNPRICE );
				TextBox tbForeignTotal = FieldTable.FindControl ( foreignTotalStr ) as TextBox;
				if (tbForeignTotal != null)
				{
					tbForeignTotal.Text = totalForeign.ToString ( "N" );
				}
				string oncostTotalStr = Common.FieldControlName ( Common.FieldControls.TOTALONCOST );
				TextBox tbOncostTotal = FieldTable.FindControl ( oncostTotalStr ) as TextBox;

				if (tbOncostTotal != null)
				{
					tbOncostTotal.Text = totalOnCost.ToString ( "N" );
				}

				if (editIndex >= 0 && trans.LineItems.Count > editIndex)
				{
					if (base.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
					{
						LineItemDO lineItem = base.trans.LineItems[editIndex] as LineItemDO;

						// WI-14861 currency unit editability is dependent on transactions associated to prevent discrepancies between 
						// currency unit and associated transactions
						if (editIndex == 0)
						{
							controlStateTable[Common.FieldControls.LINEITEM_FOREIGN_CURRENCY] = lineItem.AssociatedTransactions.Count == 0;
						}
						// WI-14861 for subsequent lineitems, disable currency unit selection and force it to be whatever the first 
						// line item is, or if there are more than 
						else
						{
							LineItemDO firstLineItem = base.trans.LineItems[0] as LineItemDO;
							Guid selectedCurrencyGuid = firstLineItem.CurrencyGuid;

							if (selectedCurrencyGuid != Guid.Empty)
							{
								string currencyUnitStr = Common.FieldControlName ( Common.FieldControls.LINEITEM_FOREIGN_CURRENCY );
								string displayName = FMChannelHelper.MakeCall<ICurrencies, string>(
																	 x =>
																	 x.Get ( this.security, selectedCurrencyGuid ).UnitDisplayName
																);

								HtmlSelect ddl = LineItemDataGrid.SelectedItem.FindControl ( currencyUnitStr ) as HtmlSelect;

								if (ddl != null)
								{
									ListItem selItem = ddl.Items.FindByText ( displayName );
									int selIndex = ddl.Items.IndexOf ( selItem );
									if (selIndex >= 0)
									{
										ddl.SelectedIndex = selIndex;
									}
								}
							}
							controlStateTable[Common.FieldControls.LINEITEM_FOREIGN_CURRENCY] = false;
						}

					}

					// JS20100915 WI-16799 recovery quantity
					controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
				}
			}

			// demand transaction, disable demand status and set it to requested by default for new items
			if (base.trans.TransTypeID == TransactionTypes.T9_Request)
			{
				// demands do not have transaction column
				LineItemDataGrid.Columns[3].Visible = false;

				if (( base.trans.LineItems.Count > 0 ) && ( base.trans.LineItems.Count > editIndex ) && ( editIndex >= 0 ))
				{
					LineItemDO lineItemDO = base.trans.LineItems[editIndex] as LineItemDO;

					string statusControlName = Common.FieldControlName ( Common.FieldControls.LINEITEM_TRANSACTIONSTATUS );
					HtmlSelect ddl = base.LineItemDataGrid.SelectedItem.FindControl ( statusControlName ) as HtmlSelect;

					// default line item status to "Requested" on creation
					if (ddl != null && lineItemDO.TransactionLineItemGuid == Guid.Empty)
					{
						int count = 0;
						foreach (ListItem item in ddl.Items)
						{
							if (item.Text.ToUpper ( ).Equals ( "REQUESTED" ))
							{
								break;
							}
							++count;
						}
						ddl.SelectedIndex = count;

						ddl.Disabled = lineItemDO.TransactionLineItemGuid < 0;
					}

					// demand requested by will be automatically set by the current user modifying it and cannot be modified manually
					string requestedByControlName = Common.FieldControlName ( Common.FieldControls.LINEITEM_REQUESTEDBY );
					TextBox requestedByCtrl = base.LineItemDataGrid.SelectedItem.FindControl ( requestedByControlName ) as TextBox;
					if (requestedByCtrl != null)
					{
						if (requestedByCtrl.Text.Length == 0)
						{
							requestedByCtrl.Text = base.security.UserID;
						}
						Control ctrl = requestedByCtrl;
						Common.EnableDisableField ( false, ref ctrl );
					}
				}
			}

			if (trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_SHIPMENT ))
			{
				try
				{
					string shipFromFieldname = Common.FieldControlName ( Common.FieldControls.SHIPPER );
					FMCompanyTextBox tbShipper = FieldTable.FindControl ( shipFromFieldname ) as FMCompanyTextBox;
					if (string.IsNullOrEmpty ( tbShipper.Text ))
					{
						tbShipper.Text = security.SiteID;
					}
				}
				catch (Exception)
				{
					// JS20100802 WI-14887
				}
			}

			// JS20100803 WI-16552
			HtmlSelect countryCtrl = FieldTable.FindControl ( Common.FieldControlName ( Common.FieldControls.USERDATA03 ) ) as HtmlSelect;
			FMCompanyTextBox customerCtrl = FieldTable.FindControl ( Common.FieldControlName ( Common.FieldControls.SHIPTO ) ) as FMCompanyTextBox;

			if (countryCtrl != null && customerCtrl != null)
			{
				// only fill the country if nothing is already selected
				if (countryCtrl.SelectedIndex <= 0 && !string.IsNullOrEmpty ( customerCtrl.Text ))
				{
					FMChannelFactory<ICompanies> companiesClient = new FMChannelFactory<ICompanies>();
					ICompanies companies = companiesClient.CreateProxy ( );
					string customerName = customerCtrl.Text;
					Guid companyGuid = companies.GetIdentityGuid ( security, customerName );

					if (companyGuid != Guid.Empty)
					{
						CompanyClass company = companies.Get(security, companyGuid);
						if (null != company)
						{
							ListItem listItem = countryCtrl.Items.FindByText ( company.Country.ToUpper ( ) );
							if (listItem != null)
							{
								countryCtrl.SelectedIndex = countryCtrl.Items.IndexOf ( listItem );
							}
						}
					}
				}
			}

			// for recovery transaction, disable the actual financial fields
			if (base.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				controlStateTable[Common.FieldControls.LINEITEM_NUMBER2] = false;
				controlStateTable[Common.FieldControls.LINEITEM_NUMBER3] = false;
				controlStateTable[Common.FieldControls.LINEITEM_NUMBER4] = false;
				controlStateTable[Common.FieldControls.LINEITEM_NUMBER5] = false;
				controlStateTable[Common.FieldControls.LINEITEM_NUMBER6] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TOTALONCOST] = false;
			}

			// JS20100721 WI-14868 Extra field states required -- START --
			if (transContext.mode != TransactionContext.Mode.Add)
			{
				controlStateTable[Common.FieldControls.BILL_TO] = false;
				controlStateTable[Common.FieldControls.FROM_BILL_TO] = false;
				controlStateTable[Common.FieldControls.TO_BILL_TO] = false;
				controlStateTable[Common.FieldControls.TO_SHIPTO] = false;
				controlStateTable[Common.FieldControls.FROM_SHIPTO] = false;

				if (!trans.Alias.ToUpper ( ).Equals ( ALIAS_COMMERCIAL ) &&
					!trans.Alias.ToUpper ( ).Equals ( ALIAS_DIRECT_FUEL_PURCHASE ))
				{
					controlStateTable[Common.FieldControls.SHIPTO] = false;
				}

				// JS20100820 WI-14868 meter start/stop only for certain transactions
				if (trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement ||
					trans.TransTypeID == TransactionTypes.T6_SecondaryDisbursement ||
					trans.TransTypeID == TransactionTypes.T3_PrimaryDefuel ||
					trans.TransTypeID == TransactionTypes.T4_SecondaryDefuel)
				{
					controlStateTable[Common.FieldControls.LINEITEM_METERSTART] = false;
					controlStateTable[Common.FieldControls.LINEITEM_METERSTARTTIME] = false;
					controlStateTable[Common.FieldControls.LINEITEM_METERSTOP] = false;
					controlStateTable[Common.FieldControls.LINEITEM_METERSTOPTIME] = false;
				}

				// JS20100820 WI-14868 defuel/returns are exceptions
				if (( trans.TransTypeID != TransactionTypes.T3_PrimaryDefuel &&
					 trans.TransTypeID != TransactionTypes.T4_SecondaryDefuel &&
					 trans.TransTypeID != TransactionTypes.T14_PhysicalInventory &&
					 trans.TransTypeID != TransactionTypes.T8_Receipt ) ||
					// if is physical inventory, then temp/density/vcf can be modified only for current day transactions
				   ( trans.TransTypeID == TransactionTypes.T14_PhysicalInventory && !trans.InventoryDate.Equals ( now.Date ) ) ||
					// if is a receipt, check quantity is usable first
				   ( trans.TransTypeID == TransactionTypes.T8_Receipt && ( trans.LineItems[0] as LineItemDO ).Quality == TransactionQuality.Usable )
				   )
				{
					controlStateTable[Common.FieldControls.LINEITEM_TEMPERATURE] = false;
					controlStateTable[Common.FieldControls.LINEITEM_DENSITY] = false;
					controlStateTable[Common.FieldControls.LINEITEM_VCF] = false;
				}

				// JS20100827 WI-16848
				if (trans.TransTypeID == TransactionTypes.T3_PrimaryDefuel ||
				   trans.TransTypeID == TransactionTypes.T4_SecondaryDefuel)
				{
					if (trans.LineItems.Count > 0)
					{
						if (( trans.LineItems[0] as LineItemDO ).Quality == TransactionQuality.Usable)
						{
							controlStateTable[Common.FieldControls.LINEITEM_TRANSACTIONQUALITY] = false;
						}
					}
				}
			}

			// on shipments, the ship from site can be disabled at all times
			if (trans.Alias.ToUpper ( ).Equals ( ALIAS_SHIPMENT ))
			{
				controlStateTable[Common.FieldControls.SHIPPER] = false;
			}

			// JS20100722 WI-15511 Force reversals to have disabled fields appear the same as others
			if (trans.ReversalType == TransactionDO.Reversal ||
			   trans.ReversalType == TransactionDO.ReversalWithUpdate)
			{
				if (trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement)
				{
					controlStateTable[Common.FieldControls.LINEITEM_NUMBER2] = false;
					controlStateTable[Common.FieldControls.LINEITEM_NUMBER3] = false;
					controlStateTable[Common.FieldControls.LINEITEM_NUMBER4] = false;
					controlStateTable[Common.FieldControls.LINEITEM_NUMBER5] = false;
					controlStateTable[Common.FieldControls.LINEITEM_NUMBER6] = false;
				}
				controlStateTable[Common.FieldControls.LINEITEM_TAX1] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TAX2] = false;
				controlStateTable[Common.FieldControls.LINEITEM_TAX3] = false;
			}

			// JS20100721 WI-14868 Extra field states required -- END --

			// JS20100628 WI-15511 Does not apply for reversed and reversal transactions
			if (!( trans.ReversalType == TransactionDO.Original ||
				trans.ReversalType == TransactionDO.Reversal ||
				trans.ReversalType == TransactionDO.ReversalWithUpdate ))
			{
				// Disable fields based on transactions and if the transaction not in <add mode JS20100624 WI-15511>
				if (base.transContext.mode != TransactionContext.Mode.Add)
				{
					if (( base.trans.Alias.ToUpper ( ).Contains ( "ISSUE" ) == true )
					   || ( base.trans.Alias.ToUpper ( ).Contains ( "SALE" ) == true )
					   || ( base.trans.Alias.ToUpper ( ).Contains ( "DEFUEL" ) == true )
					   || ( base.trans.Alias.ToUpper ( ).Contains ( "RETURN" ) == true ))
					{
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
						controlStateTable[Common.FieldControls.BILL_TO] = false;
						controlStateTable[Common.FieldControls.LINEITEM_TAX1] = true;
						controlStateTable[Common.FieldControls.LINEITEM_TAX2] = true;
						controlStateTable[Common.FieldControls.LINEITEM_TAX3] = true;
					}
					else if (base.trans.Alias.ToUpper ( ).Contains ( "DISPOSAL" ) == true)
					{
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					}
					else if (( base.trans.TransTypeID == TransactionTypes.T1_PrimaryAdjustment )
							|| ( base.trans.TransTypeID == TransactionTypes.T2_SecondaryAdjustment ))
					{
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					}
					else if (base.trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade)
					{
						controlStateTable[Common.FieldControls.LINEITEM_TO_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_FROM_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					}
					else if (base.trans.TransTypeID == TransactionTypes.T23_StorageTransfer)
					{
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					}
					else if (base.trans.TransTypeID == TransactionTypes.T7_FillStand)
					{
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					}
					else if (base.trans.TransTypeID == TransactionTypes.T10_Unload)
					{
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					}
				}
				else
				{
					if (( base.trans.Alias.ToUpper ( ).Contains ( "ISSUE" ) == true ) ||
						( base.trans.Alias.ToUpper ( ).Contains ( "SALE" ) == true ))
					{
						controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = true;
						controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = true;
					}
				}

				NewButton.Enabled = NewButton.Enabled && transContext.mode != TransactionContext.Mode.Add;
			}

			// Ensure the Product, Gross Quantity, and Product Pricefields remain editable for Direct 
			// Fuel Purchase and Commercial aliases.
			this.SetCommercialDirectFuelPurchaseFieldStates ( controlStateTable );

			// Loop through all the controls and set the state.
			this.EnableDisableFieldControls ( controlTable, controlStateTable );
		}
		/// <summary>
		/// This method will ensure that the Product, Gross Quantity, and Product Price
		/// fields remain editable for Direct Fuel Purchase and Commercial aliases.
		/// </summary>
		/// <param name="controlStateTable"></param>
		private void SetCommercialDirectFuelPurchaseFieldStates ( Hashtable controlStateTable )
		{
			bool hasInvoiceAssociated = this.HasInvoiceAssociation ( );
			bool hasAustraliaSelected = trans.UserData3 == null ? false : trans.UserData3.Equals ( "AUSTRALIA" );

			if (( base.trans.TransTypeID == TransactionTypes.T12_InventoryNotAffected )
			   && ( ( base.trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_DIRECT_FUEL_PURCHASE ) == true )
				  || ( base.trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_COMMERCIAL ) == true ) ))
			{
				controlStateTable[Common.FieldControls.LINEITEM_TAX1] = true;
				controlStateTable[Common.FieldControls.LINEITEM_TAX2] = true;

				if (hasInvoiceAssociated == true)
				{
					controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = false;
					controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = false;
					controlStateTable[Common.FieldControls.LINEITEM_PRODUCTPRICE] = false;
					controlStateTable[Common.FieldControls.LINEITEM_TAX1] = false;
					controlStateTable[Common.FieldControls.LINEITEM_TAX2] = false;
					controlStateTable[Common.FieldControls.LINEITEM_USERDATA02] = false;
					controlStateTable[Common.FieldControls.LINEITEM_USERDATA03] = false;
					controlStateTable[Common.FieldControls.LINEITEM_NON_DOMESTIC_PRICE] = false;
					controlStateTable[Common.FieldControls.LINEITEM_FOREIGN_CURRENCY] = false;
				}
				else
				{
					controlStateTable[Common.FieldControls.LINEITEM_PRODUCT] = true;
					controlStateTable[Common.FieldControls.LINEITEM_PRODUCTPRICE] = true;

					// JS20100913 WI-17509 Allow UOM to disable gross on load
					bool uomSelected = ( trans.LineItems[0] as LineItemDO ).AlternativeUnits != null;
					controlStateTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = !uomSelected;
					controlStateTable[Common.FieldControls.LINEITEM_ALTERNATIVEGROSSQUANTITY] = uomSelected;
				}
			}
		}

		/// <summary>
		/// This method is used to identify if a Direct Fuel Purchase or Commercial transaction
		/// has an associated Invoice. It will return true if there is an association. Otherwise,
		/// it returns false.
		/// </summary>
		/// <returns></returns>
		private bool HasInvoiceAssociation ( )
		{
			bool hasInvoiceAssociation = false;

			if (( base.trans.TransTypeID == TransactionTypes.T12_InventoryNotAffected )
			   && ( ( base.trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_DIRECT_FUEL_PURCHASE ) == true )
				  || ( base.trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_COMMERCIAL ) == true ) ))
			{
				try
				{
					AssociatedTxSR sr = new AssociatedTxSR ( );
					sr.Security = base.security;
					sr.TransID = base.trans.TransID;
					sr.TransactionLineItemGuid = 0;
					sr.RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions;

					FMChannelFactory<IAssociatedTxProcessor> assocTxClient = new FMChannelFactory<IAssociatedTxProcessor> ( );
					IAssociatedTxProcessor assocTxProcessor = assocTxClient.CreateProxy ( );

					AssociatedTxListDO associatedTxListDO = assocTxProcessor.Process ( sr );

					if (( associatedTxListDO.AssociatedTransactions.Tables != null ) && 
						( associatedTxListDO.AssociatedTransactions.Tables.Count > 0 ))
					{
						DataTable table = associatedTxListDO.AssociatedTransactions.Tables[0];
						if (table.Rows.Count > 0)
						{
							foreach (DataRow row in table.Rows)
							{
								TransactionTypes transTypeID = row.IsNull ( "TransTypeID" ) ? TransactionTypes.T_Maximum : (TransactionTypes) row["TransTypeID"];

								if (transTypeID == TransactionTypes.T21_AccountPayableInvoice)
								{
									hasInvoiceAssociation = true;
									break;
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					base.ErrorHandler ( e );
				}
			}

			return hasInvoiceAssociation;
		}

		/// <summary>
		/// This method is a helper to loop through all the controls in the table and set
		/// the state.
		/// </summary>
		/// <param name="controlTable"></param>
		/// <param name="controlStateTable"></param>
		private void EnableDisableFieldControls ( Hashtable controlTable, Hashtable controlStateTable )
		{
			foreach (Common.FieldControls fieldControl in controlTable.Keys)
			{
				Control ctrl = controlTable[fieldControl] as Control;
				if (null == ctrl)
				{
					continue;
				}

				bool? enable = controlStateTable[fieldControl] as bool?;
				if (enable != null)
				{
					Common.EnableDisableField ( enable.Value, ref ctrl );
				}
			}
		}

		protected override void AggregateAssociatedTxValues ( int itemIndex, bool setControls )
		{
			LineItemDO li = trans.LineItems[itemIndex] as LineItemDO;

			LineItemDO result = Common.AggregateLineItemValues ( security, trans, itemIndex );
			li = result;

			// set the controls if applicable
			if (setControls)
			{
				if (base.trans.Alias.ToUpper ( ).Equals ( ALIAS_FUEL_ORDER ) ||
				 base.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice ||
				 base.trans.TransTypeID == TransactionTypes.T8_Receipt ||
				 base.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
				{
					li.WacCalculated = false;

					double oncost = 0.0;
					try
					{
						oncost = double.Parse ( li.UserData["TALUD14"].ToString ( ) );
					}
					catch (Exception) { }

					// prepare the field data
					Hashtable controlTable = new Hashtable ( )
					{
						{Common.FieldControls.LINEITEM_PRODUCTPRICE, li.ProductPrice == null ? 0.0 : li.ProductPrice.Value},
						{Common.FieldControls.LINEITEM_TOTALPRICEWITHTAX, li.TotalPriceWithTax},
						{Common.FieldControls.LINEITEM_TOTALVALUE, li.TotalValue},
						{Common.FieldControls.LINEITEM_TAX1, li.Tax1 == null ? 0.0 : li.Tax1.Value},
						{Common.FieldControls.LINEITEM_TAX2, li.Tax2 == null ? 0.0 : li.Tax2.Value},
						{Common.FieldControls.LINEITEM_TAX3, li.Tax3 == null ? 0.0 : li.Tax3.Value},
						{Common.FieldControls.LINEITEM_NUMBER2, li.Number02 == null ? 0.0 : li.Number02.Value},
						{Common.FieldControls.LINEITEM_NUMBER3, li.Number03 == null ? 0.0 : li.Number03.Value},
						{Common.FieldControls.LINEITEM_NUMBER4, li.Number04 == null ? 0.0 : li.Number04.Value},
						{Common.FieldControls.LINEITEM_NUMBER5, li.Number05 == null ? 0.0 : li.Number05.Value},
						{Common.FieldControls.LINEITEM_NUMBER6, li.Number06 == null ? 0.0 : li.Number06.Value},
						{Common.FieldControls.LINEITEM_TOTALONCOST, oncost}
					};

					if (base.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
					{
						double totalForeignPrice = 0.0;

						try
						{
							totalForeignPrice = double.Parse ( li.UserData["TALUD3"].ToString ( ) );
						}
						catch (Exception) { }

						try
						{
							double foreignPrice = double.Parse ( li.NonDomesticPrice.Value.ToString ( ) );
							// JS20100622 WI-15432
							controlTable[Common.FieldControls.LINEITEM_NON_DOMESTIC_PRICE] = foreignPrice;
						}
						catch (Exception) { }

						controlTable[Common.FieldControls.LINEITEM_USERDATA03] = totalForeignPrice;
					}

					if (!trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_RECEIPT ))
					{
						controlTable[Common.FieldControls.LINEITEM_GROSSQUANTITY] = li.Quantity.GrossInventoryChange;
					}

					foreach (Common.FieldControls key in controlTable.Keys)
					{
						string fieldName = Common.FieldControlName ( key );
						TextBox tb = this.LineItemDataGrid.Items[itemIndex].FindControl ( fieldName ) as TextBox;
						if (tb != null)
						{
							tb.Text = ( (double) controlTable[key] ).ToString ( "N" );
						}
					}

					// JS20100907 WI-14875 set the requested delivery date control
					if (trans.Alias.ToUpper ( ).Equals ( ALIAS_FUEL_ORDER ) && li.RequestedDateTime != null)
					{
						string fieldName = Common.FieldControlName ( Common.FieldControls.LINEITEM_REQUESTEDDATETIME );
						FMDateTime ctrl = LineItemDataGrid.Items[itemIndex].FindControl ( fieldName ) as FMDateTime;
						if (ctrl != null)
							ctrl.Text = transContext.accountingSite.FormatDateTime ( li.RequestedDateTime.Value );
					}
				}
			}
		}

		protected override bool IsTransactionEditable
		{
			get
			{
				if (this.disableAll)
				{
					return false;
				}

				bool returnVal = base.IsTransactionEditable;

				return returnVal;
			}
		}

		protected override bool ConvertAlternateVolumeToGrossVolume ( LineItemDO lineItem )
		{
			bool returnVal = base.ConvertAlternateVolumeToGrossVolume ( lineItem );

			if (( lineItem.AlternativeUnits != null ) && ( ( (int) lineItem.AlternativeUnits.Value ) > 0 ))
			{
				TextBox grossVolumeTextBox = FieldTable.FindControl ( "TransactionFields.LineItemGrossQuantityFG" ) as TextBox;
				if (lineItem.Quantity != null && lineItem.AlternativeGrossVolume == null)
				{
					//Set the alternate gross using gross value conversion if the gross field already has a value but the alternate gross doesn't.
					lineItem.AlternativeGrossVolume = 0;
				}
			}

			return returnVal;
		}

		protected override void SetReverseButton ( bool editable )
		{
			if (this.security.HasRight ( RIGHT.MODIFY_TRANSACTION_DATA ) == false)
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			if (this.security.HasRight ( RIGHT.PERFORM_REVERSE_TRANSACTION ) == false)
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			if (this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				if (!security.HasModifyTransactionRightByAliasName ( trans.Alias ))
				{
					this.ReverseButton.Enabled = false;
					return;
				}
			}

			if (this.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				if (!security.HasModifyTransactionRightByAliasName ( trans.Alias ))
				{
					this.ReverseButton.Enabled = false;
					return;
				}
			}

			// JS20100428 physical inventories cannot be reversed
			if (base.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory)
			{
				base.ReverseButton.Enabled = false;
			}

			else if (this.disableAll)
			{
				base.ReverseButton.Enabled = false;
			}

			// only updates and originals can be reversed
			else if (trans.ReversalType != TransactionDO.None
			&& trans.ReversalType != TransactionDO.Update)
			{
				this.ReverseButton.Enabled = false;
			}

			// unsaved transactions cannot be enabled
			else if (transContext.mode == TransactionContext.Mode.Add)
			{
				base.ReverseButton.Enabled = false;
			}
			// WI-15266 these transactions can never be reversed, neither can transactions with parent associations
			else if (trans.TransTypeID == TransactionTypes.T9_Request ||
			   trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice ||
			   trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice ||
			   trans.TransTypeID == TransactionTypes.T18_SupplyOrder ||
			   TransHasParentAssociations ( trans ))
			{
				base.ReverseButton.Enabled = false;
			}
			else if (!string.IsNullOrEmpty ( trans.SubType ) && trans.SubType == TransactionDO.CREDIT)
			{
				// if product is a credit then prevent reversal
				base.ReverseButton.Enabled = false;
			}

			// otherwise always enabled
			else
			{
				base.ReverseButton.Enabled = true;
			}
		}

		protected override void SetReverseUpdateButton ( bool editable )
		{
			if (this.security.HasRight ( RIGHT.MODIFY_TRANSACTION_DATA ) == false)
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			if (this.security.HasRight ( RIGHT.PERFORM_REVERSE_TRANSACTION ) == false)
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			if (this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				if (!security.HasModifyTransactionRightByAliasName ( trans.Alias ))
				{
					this.ReverseUpdateButton.Enabled = false;
					return;
				}
			}

			if (this.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				if (!security.HasModifyTransactionRightByAliasName ( trans.Alias ))
				{
					this.ReverseUpdateButton.Enabled = false;
					return;
				}
			}

			if (base.trans.TransTypeID == TransactionTypes.T14_PhysicalInventory)
			{
				base.ReverseUpdateButton.Enabled = false;
			}

			else if (this.disableAll
			|| transContext.mode == TransactionContext.Mode.Add
			|| base.trans.TransTypeID == TransactionTypes.T8_Receipt)
			{
				base.ReverseUpdateButton.Enabled = false;
			}

			else if (trans.ReversalType != TransactionDO.None
			&& trans.ReversalType != TransactionDO.Update)
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}
			// WI-15266 these transactions can never be reversed, neither can transactions with parent associations
			else if (trans.TransTypeID == TransactionTypes.T9_Request ||
			   trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice ||
			   trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice ||
			   trans.TransTypeID == TransactionTypes.T18_SupplyOrder ||
			   TransHasParentAssociations ( trans ))
			{
				base.ReverseUpdateButton.Enabled = false;
			}
			else if (!string.IsNullOrEmpty ( trans.SubType ) && trans.SubType == TransactionDO.CREDIT)
			{
				// if product is a credit then prevent reversal
				base.ReverseUpdateButton.Enabled = false;
			}

			// otherwise always enabled WI
			else
			{
				base.ReverseUpdateButton.Enabled = true;
			}
		}

		#endregion // Other Overrides

		#region WAC Processing
		protected void SaveWAC ( TransactionDO a_savingTransaction, TransactionDO origTransaction )
		{
			// JS20100722 Prevents certain transactions to ever impact the WAC
			if (trans.Alias.ToUpper ( ).Equals ( ALIAS_COMMERCIAL ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_DIRECT_FUEL_PURCHASE ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_FUEL_ORDER ) ||
				trans.Alias.ToUpper ( ).Equals ( ALIAS_RECOVERY ) ||
				trans.Alias.ToUpper ( ).Contains ( "DEMAND" ))
			{
				return; // failsafe
			}

			try
			{
				// need to check if we saved an update, if so then there should be an equivalent reverse
				// retrieve the update and reverse, calculate their wac and resave.
				// assuming reverse can't be modified (which it can't), we retrieve it only on new update
				if (a_savingTransaction.ReversalType == TransactionDO.Update && null == origTransaction)
				{
					// JS20100621 NOTE: if you are to change this so that it runs concurrently with transaction
					// save, then look for tag [SKAFTSV001] label to change as well
					TransactionDO reverseTrans = null;
					TransactionDO originalTrans = null;
					TransactionSR reverseSR = new TransactionSR ( );
					reverseSR.Security = this.security;
					reverseSR.TransID = this.trans.ReversedTransID;

					try
					{
						FMChannelFactory<ITransactionProcessor> txProcessorClient = new FMChannelFactory<ITransactionProcessor> ( );
						ITransactionProcessor txProcessor = txProcessorClient.CreateProxy ( );

						reverseTrans = txProcessor.Process ( reverseSR ) as TransactionDO;
						// we also need the original transaction for the old WAC value
						if (reverseTrans != null)
						{
							originalTrans = this.LoadTransaction ( reverseTrans.ReversedTransID );
						}
					}
					catch (Exception e)
					{
						base.ErrorHandler ( e );
					}

					if (reverseTrans != null)
					{
						// set the WAC value of this update to the orignal WAC
						foreach (LineItemDO li in a_savingTransaction.LineItems)
						{
							li.WacCalculated = false;
							foreach (LineItemDO oli in originalTrans.LineItems)
							{
								if (oli.TransactionLineItemGuid == li.TransactionLineItemGuid)
								{
									if (oli.Tax4 != null)
									{
										li.Tax4 = oli.Tax4;
									}
								}
							}
						}

						// force recalculation of WAC for reverse transaction
						foreach (LineItemDO li in reverseTrans.LineItems)
						{
							li.WacCalculated = false;
							foreach (LineItemDO oli in originalTrans.LineItems)
							{
								if (oli.TransactionLineItemGuid == li.TransactionLineItemGuid)
								{
									li.Tax4 = oli.Tax4;
								}
							}
						}

						// resave these transactions
						try
						{
							SaveTransactionsSR sr = new SaveTransactionsSR ( );
							SaveWeightedAverageCostsSR wacSR = new SaveWeightedAverageCostsSR ( this.security );

							sr.CurrentSiteGuid = this.security.SiteGuid;
							sr.Security = this.security;
							wacSR.Security = this.security;

							FMChannelFactory<ISaveTransactionsProcessor> saveTxClient = new FMChannelFactory<ISaveTransactionsProcessor> ( );
							ISaveTransactionsProcessor saveTxProcessor = saveTxClient.CreateProxy ( );

							FMChannelFactory<IPriceCalculatorInvoker> priceInvokerClient = new FMChannelFactory<IPriceCalculatorInvoker> ( );
							IPriceCalculatorInvoker invoker = priceInvokerClient.CreateProxy ( );

							// need to do these individually, the update transaction WAC will be affected by the first reversal, so we
							// must save reversal first, update the WAC, then repeat with the update
							reverseTrans.TransVersion = 0;
							
							// add the update quantity to work with code at [SKAFTSV001] does NOT work with multi-line items (currently there are none)
							if (a_savingTransaction.LineItems.Count > 0 && reverseTrans.LineItems.Count > 0)
							{
								SetVolumeSigns ( a_savingTransaction, false );
								( reverseTrans.LineItems[0] as LineItemDO ).UserData["TALUD24"] =
								   ( a_savingTransaction.LineItems[0] as LineItemDO ).Quantity.GrossInventoryChange.ToString();
							}

							invoker.Calculate ( security, reverseTrans );

							// [SKAFTSV001] restore to unused
							if (a_savingTransaction.LineItems.Count > 0 && reverseTrans.LineItems.Count > 0)
							{
								// defensive programming
								( reverseTrans.LineItems[0] as LineItemDO ).UserData["TALUD24"] = null;
							}

							sr.Transactions.Clear ( );
							reverseTrans.TransVersion = 0;
							sr.Transactions.Add ( reverseTrans );
							SaveTransactionsResultDO resultDO = saveTxProcessor.SaveTransactions ( sr );
							CheckForAndDisplayWarningMessagesInternal ( resultDO );

							// save the WAC
							reverseTrans = this.LoadTransaction ( reverseTrans.TransID );
							wacSR = this.BuildWeightedAverageCostsSR ( this.security, reverseTrans, origTransaction, true );

							FMChannelFactory<ISaveWeightedAverageCostsProcessor> wacClient = new FMChannelFactory<ISaveWeightedAverageCostsProcessor> ( );
							ISaveWeightedAverageCostsProcessor wacProcessor = wacClient.CreateProxy ( );

							CustomResultDO wacResult = wacProcessor.Process ( wacSR ) as CustomResultDO;

							// now do the update part
							a_savingTransaction.TransVersion = 0;

							invoker.Calculate (security, a_savingTransaction);

							sr.Transactions.Clear ( );
							a_savingTransaction.TransVersion = 0;
							sr.Transactions.Add ( a_savingTransaction );
							SetVolumeSigns ( a_savingTransaction, false );

							resultDO = saveTxProcessor.SaveTransactions ( sr );
							SetVolumeSigns ( a_savingTransaction, true );
							CheckForAndDisplayWarningMessagesInternal ( resultDO );

							// save the WAC
							a_savingTransaction = this.LoadTransaction ( a_savingTransaction.TransID );
							wacSR = this.BuildWeightedAverageCostsSR ( this.security, a_savingTransaction, origTransaction, true );

							wacResult = wacProcessor.Process ( wacSR );
						}
						catch (Exception e)
						{
							base.ErrorHandler ( e );
						}
					}
				}
				else
				{
					// JS20091123 Need to check if we need to update the WAC with any of the line item changes
					SaveWeightedAverageCostsSR wacSR = null;
					TransactionDO savedTransaction = a_savingTransaction;

					if (savedTransaction != null)
					{
						wacSR = this.BuildWeightedAverageCostsSR ( this.security, savedTransaction, origTransaction );

						if (wacSR.WeightedAverageCosts.Count > 0)
						{
							// only attempt to write WAC if there is something to be saved
							FMChannelFactory<ISaveWeightedAverageCostsProcessor> wacClient = new FMChannelFactory<ISaveWeightedAverageCostsProcessor> ( );
							ISaveWeightedAverageCostsProcessor wacProcessor = wacClient.CreateProxy ( );

							CustomResultDO wacResult = wacProcessor.Process ( wacSR );
						}
					}
					// else means transaction wasn't saved properly
				}
			}
			catch (Exception e)
			{
				base.ErrorHandler ( e ); // rethrow, helps with debugging
			}
		}

		protected SaveWeightedAverageCostsSR BuildWeightedAverageCostsSR ( SecurityClass a_security, TransactionDO a_trans, TransactionDO a_origTrans )
		{
			return this.BuildWeightedAverageCostsSR ( a_security, a_trans, a_origTrans, false );
		}

		protected SaveWeightedAverageCostsSR BuildWeightedAverageCostsSR ( SecurityClass a_security, TransactionDO a_trans, TransactionDO a_origTrans, bool a_force )
		{
			SaveWeightedAverageCostsSR result = new SaveWeightedAverageCostsSR ( a_security );

			FMChannelFactory<ISaveWeightedAverageCostsProcessor> wacClient = new FMChannelFactory<ISaveWeightedAverageCostsProcessor> ( );
			ISaveWeightedAverageCostsProcessor saveWeightedAverageCostsProcessor = wacClient.CreateProxy ( );

			foreach (LineItemDO lineItemDO in a_trans.LineItems)
			{
				if (saveWeightedAverageCostsProcessor.ShouldWacUpdate ( a_trans, lineItemDO, a_origTrans ) || a_force)
				{
					// CCP-043 WAC should always update regardless of whether or not it's the same
					WeightedAverageCostDO wac = new WeightedAverageCostDO ( );

					wac.CreatedBy = a_security.UserID;
					wac.CreatedDate = DateTime.UtcNow;
					wac.UpdatedBy = wac.CreatedBy;
					wac.UpdatedDate = wac.CreatedDate;
					wac.IsManualOverride = false;
					wac.Notes = " ";
					wac.SiteGuid = a_trans.SiteGuid;
					wac.Source = a_trans.TransID;
					wac.WeightedAverageCostGuid = Guid.Empty;
					wac.InventoryDate = a_trans.InventoryDate;

					if (a_trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade ||
						a_trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade)
					{
						// regrades work a bit different, tax4 keeps the WAC used (i.e. source fuel) where tax5 stores
						// the new WAC calculated for the current fuel

						// create the WAC data object for destination fuel
						wac.ProductGuid = (lineItemDO as RegradeLineItemDO).ProductGuid;
						if (lineItemDO.Tax5 != null)
						{
							wac.WacValue = (double) lineItemDO.Tax5.Value;
						}

						// create the WAC data object for source fuel
						WeightedAverageCostDO wac2 = new WeightedAverageCostDO ( );

						wac2.CreatedBy = a_security.UserID;
						wac2.CreatedDate = DateTime.UtcNow;
						wac2.UpdatedBy = wac2.CreatedBy;
						wac2.UpdatedDate = wac2.CreatedDate;
						wac2.IsManualOverride = false;
						wac2.Notes = " ";
						wac2.SiteGuid = a_trans.SiteGuid;
						wac2.Source = a_trans.TransID;
						wac2.WeightedAverageCostGuid = Guid.Empty;
						wac2.InventoryDate = a_trans.InventoryDate;

						wac2.ProductGuid = (lineItemDO as RegradeLineItemDO).ProductGuid;

						if (lineItemDO.Tax4 != null)
						{
							wac2.WacValue = (double) lineItemDO.Tax4.Value;
						}

						result.WeightedAverageCosts.Add ( wac2 );
					}
					else
					{
						wac.ProductGuid = lineItemDO.ProductGuid;

						if (lineItemDO.Tax4 != null)
						{
							wac.WacValue = (double) lineItemDO.Tax4.Value;
						}
					}

					result.WeightedAverageCosts.Add ( wac );
				}
			}

			return result;
		}
		#endregion // WAC Processing

		#region Copy Methods
		/// <summary>
		/// This method will override the base class load transaction with specific
		/// ADF functionality.
		/// </summary>
		/// <param name="transID"></param>
		/// <returns></returns>
		protected override TransactionDO LoadTransaction ( string transID )
		{
			TransactionSR sr = new TransactionSR ( );
			sr.Security = this.security;
			sr.TransID = transID;

			try
			{
				TransactionDO trans = null;
				// vthompson 10-27-2008
				// Changed this to only call the load rack if the site is not a site group
				if (accountingSite.CurrentSite.SiteGroup)
				{
					sr.AccountingSite = this.accountingSite;

					trans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process ( sr )
																);


					bTransIDBeingLoaded = false;
					return trans;
				}
				else
				{
					if (UsingLoadRack)
					{
						FMBusinessObjects.Interfaces.ILoadRackManager LoadRackManager = GetLoadRackManager();
						trans = LoadRackManager.AccountingRequest(sr);
						bTransIDBeingLoaded = true;
						return trans;
					}
					// alternateLoad
				}
			}
			// vthompson 10/15/2008
			// Originally the exception message was checked to determine if connectivity to the load rack service
			// failed.  The message changed in .NET 2.0 so this design was changed.
			// bschaal 1/07/09
			// the above comment is incorrect. The change made here prevents the ability to edit a bol when the loadrack service is running.
			// .NET 2.0 has nothing to do with this. This code has been changed back
			catch (System.Net.Sockets.SocketException)
			{
				// alternateLoad
			}
			catch (Exception except)
			{
				if (!except.Message.Contains ( "No connection could be made because the target machine actively refused it" ) &&
				   !except.Message.Contains ( "Not Loading" ) &&
				   !except.Message.Contains ( "Requested Service not found" ))
				{
					throw ( except );
				}

				sr.AccountingSite = this.accountingSite;

				trans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process ( sr )
																);

				bTransIDBeingLoaded = false;
				return trans;
			}
			// alternateLoad
			sr.AccountingSite = this.accountingSite;

			trans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process(sr)
																);
			;
			bTransIDBeingLoaded = false;
			return trans;
		}

		/// <summary>
		/// This method overrides the base class method with ADF project specific
		/// functionality.
		/// </summary>
		protected override void Close ( )
		{
			AssociatedTxContext associatedTxContext = Session["AssociatedTxContext"] as AssociatedTxContext;
			OrderAssociatedTxContext orderContext = Session["OrderAssociatedTxContext"] as OrderAssociatedTxContext;
			SupplyOrderAssociatedTxContext supplyOrderContext = Session["SupplyOrderAssociatedTxContext"] as SupplyOrderAssociatedTxContext;
			TransactionDetailList transDetailList = Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList;

			string returnPage;

			if (Request.Params["QueryEditItem"].DefaultIfNull ( "" ).Equals ( "" ) == false)
			{
				returnPage = "..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning";
			}
			else if (associatedTxContext != null)
			{
				returnPage = "../Accounting/AssociatedTxSummary.aspx";
			}
			else if (transDetailList != null && string.IsNullOrEmpty ( transDetailList.ReturnURL ) == false
				// JS20101001 WI-18144 check the right alias is being used, see the defect for more details for why this is needed
			   && transDetailList.ReturnURL.ToUpper ( ).Contains ( trans.Alias.ToUpper ( ) ))
			{
				returnPage = transDetailList.ReturnURL;
				Session.Remove ( TransactionDetailList.TransactionDetailListKey );
			}
			else
			{
				base.UpdateTransDetailList ( );

				// Build URL for transferring to the transaction list page.
				int row = this.trans.TransactionDateTime.Value.Day - 1;
				returnPage = "../Accounting/TransactionList.aspx?Row=";
				returnPage = returnPage + row + "&Column=" + this.trans.Alias;

				if (this.trans.TransTypeID == TransactionTypes.T17_Order)
				{
					returnPage = "..\\OrderEntryWebApp\\OrderSummary.aspx";
				}
				else if (this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder)
				{
					returnPage = "..\\SupplyOrderWebApp\\SupplyOrderSummary.aspx";
				}
				else if (this.trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
				{
					Page.Session.Add ( "InvoiceSummaryType", "21" );
					returnPage = "..\\ADFWebApp\\InvoicePaymentSummary.aspx?mode=" + (int) InvoicePaymentMode.INVOICE;
				}
				else if (this.trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
				{
					Page.Session.Add ( "InvoiceSummaryType", "22" );
					returnPage = "..\\InvoiceWebApp\\InvoiceSummary.aspx";
				}
			}

			base.Reset ( );
			Session.Remove ( "allAssociatedTransactionsBeforeTransactionEdit" );

			// JS20101001 WI-18144 Always remove the transaction summary return key no matter what
			Session.Remove ( TransactionDetailList.TransactionDetailListKey );

			TransactionListContext transactionListContext = Session["TransactionListContext"] as TransactionListContext;
			if (transactionListContext == null)
			{
				transactionListContext = new TransactionListContext ( );
			}

			if (transactionListContext.TransactionListReturnURL == "")
			{
				transactionListContext.Site = base.security.SiteID;
				transactionListContext.Month = trans.InventoryDate.ToString ( "MMM yyyy" );
				transactionListContext.Manager = trans.ManagerID;

				if (trans.LineItems.Count > 0)
				{
					transactionListContext.Product = ( (LineItemDO) trans.LineItems[0] ).Product;
				}

				transactionListContext.ReturnURL = Request.ApplicationPath + "/FMWebApp/FuelsManagerForm.aspx";
				transactionListContext.TransactionListReturnURL = transactionListContext.ReturnURL;
				transactionListContext.Owner = trans.OwnerID;

				Session["TransactionListContext"] = transactionListContext;
			}

			// custom redirect
			if (this.customRedirect != null)
			{
				this.Redirect ( this.customRedirect );
			}
			else
			{
				this.Redirect ( returnPage );
			}
		}
		#endregion // Copy Methods

		#region Private methods
		/// <summary>
		/// This method will return true if the direct fuel purchase number is unique within a 
		/// given site.  Otherwise, it will return false. For transactions other than type 12, true
		/// is always returned.
		/// </summary>
		/// <returns></returns>
		private bool IsDirectFuelPurchaseUnique ( )
		{
			bool isUnique = true;
			this.errorMsg = "";

			if (base.transContext.mode == TransactionContext.Mode.Edit)
			{
				return isUnique;
			}

			if (base.trans.TransTypeID == TransactionTypes.T12_InventoryNotAffected)
			{
				if (( base.trans.PONumber != null ) && ( base.trans.PONumber.Length > 0 ))
				{
					try
					{
						FMChannelFactory<ITFMSServices> tfmsClient = new FMChannelFactory<ITFMSServices> ( );
						ITFMSServices tfmsBll = tfmsClient.CreateProxy ( );
						isUnique = tfmsBll.IsDirectPurchaseNumberUnique ( base.security, base.trans.PONumber );

						if (isUnique == false)
						{
							this.errorMsg = "Direct Fuel Purchase Number '" + trans.PONumber + "' must be unique.";
						}
					}
					catch (Exception)
					{
						isUnique = false;
						this.errorMsg = "Database error; could not determine if the Direct Fuel Purchase Number '" +
										trans.PONumber + "' is unique.";
					}
				}
				else
				{
					isUnique = false;
					this.errorMsg = "Direct Fuel Purchase Number is required";
				}
			}

			return isUnique;
		}

		/// <summary>
		/// This method will set some of the Direct Fuel Purchase and Commercial price fields.
		/// </summary>
		private void SetDirectFuelPurchaseAndCommercialPriceFields ( )
		{
			if (( base.trans.TransTypeID == TransactionTypes.T12_InventoryNotAffected )
			   && ( ( base.trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_COMMERCIAL ) == true )
				  || ( base.trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_DIRECT_FUEL_PURCHASE ) == true ) ))
			{
				if (( base.trans.LineItems != null ) && ( base.trans.LineItems.Count > 0 ))
				{
					LineItemDO lineItemDO = base.trans.LineItems[0] as LineItemDO;

					if (lineItemDO != null)
					{
						string totalPriceAUD = lineItemDO.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_02] as string;
						string totalForeignPrice = lineItemDO.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_03] as string;

						string formatString = "{0:0.";
						for (int i = 0; i < Common.DF_COMMERCIAL_PRECISION; ++i)
						{
							formatString += "0";
						}
						formatString += "}";

						if (string.IsNullOrEmpty ( totalPriceAUD ) == false)
						{
							TextBox totalPriceControl = base.FieldTable.FindControl ( "TransactionFields.UserDataTextFGTALUD2" ) as TextBox;

							if (totalPriceControl != null)
							{
								double price = 0.0;
								try
								{
									price = double.Parse ( totalPriceAUD );
								}
								catch (Exception) { }

								totalPriceControl.Text = String.Format ( formatString, price );
							}
						}

						if (string.IsNullOrEmpty ( totalForeignPrice ) == false)
						{
							TextBox totalForeignPriceControl = base.FieldTable.FindControl ( "TransactionFields.UserDataTextFGTALUD3" ) as TextBox;

							if (totalForeignPriceControl != null)
							{
								double price = 0.0;
								try
								{
									price = double.Parse ( totalForeignPrice );
								}
								catch (Exception) { }

								totalForeignPriceControl.Text = String.Format ( formatString, price );
							}
						}

						if (lineItemDO.Tax1 != null)
						{
							TextBox exciseControl = base.FieldTable.FindControl ( "TransactionFields.LineItemTax1FG" ) as TextBox;

							if (exciseControl != null)
							{
								exciseControl.Text = String.Format ( formatString, lineItemDO.Tax1.Value );
							}
						}

						if (lineItemDO.Tax2 != null)
						{
							TextBox gstControl = base.FieldTable.FindControl ( "TransactionFields.LineItemTax2FG" ) as TextBox;

							if (gstControl != null)
							{
								gstControl.Text = String.Format ( formatString, lineItemDO.Tax2.Value );
							}
						}

						if (lineItemDO.ProductPrice != null)
						{
							string priceName = Common.FieldControlName ( Common.FieldControls.LINEITEM_PRODUCTPRICE );
							TextBox priceControl = base.FieldTable.FindControl ( priceName ) as TextBox;

							if (priceControl != null)
							{
								priceControl.Text = String.Format ( formatString, lineItemDO.ProductPrice.Value );
							}
						}
					}
				}
			}
		}

		private bool UpdateDirectFuelPurchaseCommercialFields ( )
		{
			TFMSDO tfmsDO = new TFMSDO ( );
			return UpdateDirectFuelPurchaseCommercialFields ( ref tfmsDO, base.trans );
		}

		/// <summary>
		/// This method will update the Direct Fuel Purchase and Commercial fields
		/// prior to saving.
		/// </summary>
		public bool UpdateDirectFuelPurchaseCommercialFields ( ref TFMSDO tfmsDO, TransactionDO a_trans )
		{
			bool successful = true;

			if (( a_trans.TransTypeID == TransactionTypes.T12_InventoryNotAffected )
			   && ( ( a_trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_DIRECT_FUEL_PURCHASE ) == true )
				  || ( a_trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_COMMERCIAL ) == true ) ))
			{
				if (( a_trans.LineItems != null ) && ( a_trans.LineItems.Count > 0 ))
				{
					LineItemDO lineItemDO = a_trans.LineItems[0] as LineItemDO;

					if (lineItemDO != null)
					{
						DFPCommercialCurrencyValidation dfpCommercialCurrencyValidation = new DFPCommercialCurrencyValidation ( base.security );

						dfpCommercialCurrencyValidation.Source = DFPCommercialCurrencyValidation.Sources.GUI;

						tfmsDO.DateTime = a_trans.InventoryDate;
						tfmsDO.Excise = null;
						tfmsDO.ForeignCurrencyPrice = null;
						tfmsDO.ForeignCurrencyUnit = "";
						tfmsDO.FuelPriceAUD = null;
						tfmsDO.GST = null;
						tfmsDO.Quantity = null;
						tfmsDO.TotalForeignCurrencyPrice = null;
						tfmsDO.TotalPriceAUD = null;

						// Set the quantity to the gross quantity
						if (lineItemDO.AlternativeGrossVolume != null)
						{
							// JS20100914 WI-17681
							tfmsDO.Quantity = lineItemDO.AlternativeGrossVolume.Value;
						}
						else if (lineItemDO.Quantity != null)
						{
							tfmsDO.Quantity = lineItemDO.Quantity.GrossInventoryChange;
						}

						// Set GST tax
						if (( lineItemDO.Tax2 != null ) && ( lineItemDO.Tax2 != null ))
						{
							tfmsDO.GST = lineItemDO.Tax2.Value;
						}

						// Set Excise tax
						if (( lineItemDO.Tax1 != null ) && ( lineItemDO.Tax1 != null ))
						{
							tfmsDO.Excise = lineItemDO.Tax1.Value;
						}

						// Set foreign currency price
						if (( lineItemDO.NonDomesticPrice != null ) && ( lineItemDO.NonDomesticPrice != null ))
						{
							tfmsDO.ForeignCurrencyPrice = lineItemDO.NonDomesticPrice.Value;
						}

						// Set currency unit
						if ((lineItemDO.CurrencyGuid != Guid.Empty) && (lineItemDO.CurrencyGuid != Guid.Empty))
						{
							dfpCommercialCurrencyValidation.CurrencyGuid = lineItemDO.CurrencyGuid;
						}

						// Set the quantity to the gross quantity
						if (lineItemDO.AlternativeUnits != null && lineItemDO.AlternativeGrossVolume != null)
						{
							// JS20100908 WI-17510 if UOM present, then use its quantity
							tfmsDO.Quantity = lineItemDO.AlternativeGrossVolume.Value;
						}
						else if (lineItemDO.Quantity != null)
						{
							tfmsDO.Quantity = lineItemDO.Quantity.GrossInventoryChange;
						}

						// Set total fuel price AUD
						string totalFuelPrice = lineItemDO.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_02] as string;
						if (string.IsNullOrEmpty ( totalFuelPrice ) == false)
						{
							try
							{
								tfmsDO.TotalPriceAUD = Convert.ToDouble ( totalFuelPrice );
							}
							catch (Exception)
							{
								// do nothing.
							}
						}

						// Only perform pricing validation if one of the fields (Foreign Price, Foreign Currency Unit,
						// Total Foreign Price, Fuel Price AUD, or Total Fuel Price AUD) have a value. If not,
						// then ignore validation.
						if (( ( tfmsDO.ForeignCurrencyPrice == null )
						   && ( lineItemDO.CurrencyGuid == Guid.Empty )
						   && ( tfmsDO.TotalForeignCurrencyPrice == null )
						   && ( tfmsDO.TotalPriceAUD == null )
						   && ( tfmsDO.FuelPriceAUD == null ) ) == false)
						{
							// If the currency unit is not set that means the we should validate for
							// domestic currency rules.
							if ((lineItemDO.CurrencyGuid == Guid.Empty)
								|| (lineItemDO.CurrencyGuid == Guid.Empty)
							   || ( lineItemDO.CurrencyGuid == Guid.Empty ))
							{
								Guid supplierGuid = Guid.Empty;
								dfpCommercialCurrencyValidation.ClearErrorMessage ( );

								if (a_trans.SupplierCompanyGuid != Guid.Empty)
								{
									supplierGuid = a_trans.SupplierCompanyGuid;
								}

								Guid productGuid = lineItemDO.ProductGuid;
								successful = dfpCommercialCurrencyValidation.DomesticCurrencyValidation(tfmsDO, supplierGuid, productGuid);

								if (successful == false)
								{
									this.errorMsg = dfpCommercialCurrencyValidation.ErrorMsg;
								}
							}
							// If the currency unit is set that means that we should validate for
							// foreign currency rules.
							else
							{
								dfpCommercialCurrencyValidation.ClearErrorMessage ( );
								successful = dfpCommercialCurrencyValidation.ForeignCurrencyValidation ( tfmsDO );

								if (successful == false)
								{
									this.errorMsg = dfpCommercialCurrencyValidation.ErrorMsg;
								}
							}

							if (successful == true)
							{
								this.TransDataValidationUpdate ( tfmsDO, lineItemDO );
							}
						}
					}
				}
			}

			return successful;
		}

		/// <summary>
		/// This method will update the transaction line item pricing information
		/// after the pricing validation.
		/// </summary>
		/// <param name="tfmsDO"></param>
		/// <param name="lineItemDO"></param>
		private void TransDataValidationUpdate ( TFMSDO tfmsDO, LineItemDO lineItemDO )
		{
			if (tfmsDO.FuelPriceAUD != null)
			{
				TextBox domesticPriceCntrl = base.FieldTable.FindControl ( "TransactionFields.LineItemProductPriceFG" ) as TextBox;
				lineItemDO.ProductPrice = tfmsDO.FuelPriceAUD.Value;

				if (domesticPriceCntrl != null)
				{
					domesticPriceCntrl.Text = tfmsDO.FuelPriceAUD.Value.ToString ( );
				}
			}

			if (tfmsDO.ForeignCurrencyPrice != null)
			{
				TextBox nonDomesticPriceCntrl = base.FieldTable.FindControl ( "TransactionFields.LineItemNonDomesticPriceFG" ) as TextBox;
				lineItemDO.NonDomesticPrice = tfmsDO.ForeignCurrencyPrice.Value;

				// Must set the control or an error is generated by the Base Transaction Detail
				// validations.
				if (nonDomesticPriceCntrl != null)
				{
					nonDomesticPriceCntrl.Text = tfmsDO.ForeignCurrencyPrice.Value.ToString ( );
				}
			}

			if (tfmsDO.GST != null)
			{
				lineItemDO.Tax2 = tfmsDO.GST.Value;
			}

			if (tfmsDO.Excise != null)
			{
				lineItemDO.Tax1 = tfmsDO.Excise.Value;
			}

			if (tfmsDO.TotalPriceAUD != null)
			{
				lineItemDO.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_02] = tfmsDO.TotalPriceAUD.ToString ( );
			}

			if (tfmsDO.TotalForeignCurrencyPrice != null)
			{
				lineItemDO.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_03] = tfmsDO.TotalForeignCurrencyPrice.ToString ( );
			}
		}

		/// <summary>
		/// This method will set the fields that need to be disabled on the client side based
		/// on transaction alias and security rights.
		/// </summary>
		private void ClientSideDisableFieldsBasedOnRights ( )
		{
			if (( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_DIRECT_FUEL_PURCHASE ) == true )
			   || ( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_COMMERCIAL ) == true ))
			{
				if (base.security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA ) == false)
				{
					// The field right are made up of either F (no rights) or T (has rights)
					// plus the field name. Each right/field name is divided by a "|".
					// The value is stored in a hidden field that is used by the ADF custom script
					// on the client side.
					FieldsAndRights.Value = "FLineItemTax1FG|FLineItemTax2FG";
				}
			}
		}

		/// <summary>
		/// This method will override the InitTransaction base method and implement BSME specific functions
		/// when the New Button is pressed. Certain BSME transactions need to retain certain fields when
		/// the new button is pressed.
		/// </summary>
		protected override void InitTransaction ( )
		{
			if (base.trans != null)
			{
				// JS20100629 WI-15694, do not retain reversal type on new
				trans.ReversalType = TransactionDO.None;

				if (( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_SALE_AVIATION ) == true ) ||
					( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_SALE_GROUND ) == true ) ||
					( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_SALE_MARINE ) == true ))
				{
					// Saves certain fields and repopulates them. It calls the base class
					// InitTransaction().
					this.NewButtonSaleTxProcessing ( );
				}
				// JS20100622 WI-15399 so that the transaction is initialised and not use the old one if not one of the above transactions
				else
				{
					// JS20100706 WI-15841 For receipts, clear the supplier on new
					if (trans.TransTypeID == TransactionTypes.T8_Receipt)
					{
						base.trans.SupplierCode = base.trans.SupplierID = "";
						base.trans.SupplierCompanyGuid = Guid.Empty;
					}

					trans.TransVersion = 0;
					base.InitTransaction ( );
				}
			}
			else
			{
				base.InitTransaction ( );

				FMChannelFactory<ITransactionAliases> aliasesClient = new FMChannelFactory<ITransactionAliases> ( );
				ITransactionAliases aliases = aliasesClient.CreateProxy ( );
				TransactionAliasClass alias = aliases.Get ( security, aliases.GetIdentityGuid ( security, transAlias ), true );

				// JS20100803 WI-16550 forces disposals not to accept fuel from summary page
				if (!alias.MultipleLineItems && alias.ID.ToUpper ( ).Equals ( ALIAS_DISPOSAL ))
				{
					LineItemDO lineItem = trans.LineItems[0] as LineItemDO;
					if (lineItem != null)
					{
						lineItem.Product = null;
						lineItem.ProductCode = null;
						lineItem.ProductType = ProductClass.ProductTypeID ( PRODUCT_TYPE.COMPONENT_PRODUCT );
						lineItem.ProductGuid = Guid.Empty;
					}
				}
			}
		}

		/// <summary>
		/// This method handles the processing for the new button being pressed.
		/// </summary>
		protected void ADFNewButtonProcess ( )
		{
			if (( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_SALE_AVIATION ) == true ) ||
				( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_SALE_GROUND ) == true ) ||
				( base.trans.Alias.ToUpper ( ).Equals ( ALIAS_SALE_MARINE ) == true ))
			{
				Session.Remove ( TransactionDetailList.TransactionDetailListKey );
				Session.Remove ( "AssociatedTxContext" );
				Session.Remove ( "allAssociatedTransactionsBeforeTransactionEdit" );

				//Create new transaction.
				this.InitTransaction ( );

				base.transContext.mode = TransactionFields.TransactionContext.Mode.Add;

				Session[TransactionDetailBase.ModeKey] = base.transContext.mode;
				Session[TransactionDetailBase.TransKey] = base.trans;

				base.SetButtons ( );

				bool reload = true;
				base.RegenerateControls ( reload );
			}
			else
			{
				// The default new button processing.
				base.NewButtonProcess ( );
			}
		}

		/// <summary>
		/// This method will save certain Sale transaction fields and repopulate the fields
		/// with the new transaction. It calls the base class InitTransaction method.
		/// </summary>
		private void NewButtonSaleTxProcessing ( )
		{
			string billToID = base.trans.BillToID;
			Guid billToGuid = base.trans.BillToCompanyGuid;
			string billToCode = base.trans.BillToCode;

			string shipToID = base.trans.ShipToID;
			Guid shipToGuid = base.trans.ShipToCompanyGuid;
			string shipToCode = base.trans.ShipToCode;

			base.InitTransaction ( );

			base.trans.BillToID = billToID;
			base.trans.BillToCompanyGuid = billToGuid;
			base.trans.BillToCode = billToCode;

			base.trans.ShipToID = shipToID;
			base.trans.ShipToCompanyGuid = shipToGuid;
			base.trans.ShipToCode = shipToCode;

			FMCompanyTextBox billToCompanyTB = base.FieldTable.FindControl ( "TransactionFields.BillToFG" ) as FMCompanyTextBox;
			FMCompanyTextBox shipToCompanyTB = base.FieldTable.FindControl ( "TransactionFields.ShipToFG" ) as FMCompanyTextBox;

			if (billToCompanyTB != null)
			{
				billToCompanyTB.Text = base.trans.BillToID;
			}

			if (shipToCompanyTB != null)
			{
				shipToCompanyTB.Text = base.trans.ShipToID;
			}
		}
		#endregion
	}
}



