// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Common.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Common type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADFWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	public class Common
	{
		#region Constants
		public static string USERDATA_SUPPLIERINVOICENUMBER_KEY = "TAUD3";
		public static string USERDATA_SECTION_KEY				= "TAUD5";
		public static string USERDATA_COSTCENTRE_KEY			= "TAUD19";
		public static string USERDATA_ACCTCODE_KEY				= "TAUD13";
		public static string USERDATA_ACTIONREQUIRED_KEY		= "TAUD6";
		public static string USERDATA_ROMANNUMBER_KEY			= "TAUD8";
		public static string SECTION_KEY						= "Software\\Varec\\ADFCustomData";
		public static string SECTION_VALUE						= "Sections";
		public static string DDL_ALL							= "{All}";
		public static short DF_COMMERCIAL_PRECISION				= 5;
		public static string ADFWEBAPP_URL						= "../ADFWebApp/";

		// different types of fields
		public enum FieldControls
		{
			// header fields
			TRANSACTIONSTATUS,
			DOCUMENTNUMBER,
			TOTALPRICEAMOUNT,
			TOTALPRICEWITHTAX,
			TOTALONCOST,
			USERDATA3,
			BILL_TO,
			FROM_BILL_TO,
			TO_BILL_TO,
			FROM_SHIPTO,
			TO_SHIPTO,
			SHIPTO,
			ROSUPPLIER,
			TOTALFOREIGNPRICE,
			ADFTRANSACTIONDATETIME,
			TOTALEXCISE,
			TOTALGST,
			TOTALMARKUP,
			SHIPPER,
			COUNTRY,
			DESTINATIONEQUIPMENTFG1,
			SOURCEEQUIPMENTFG1,

			TRANSACTIONDATE_DAY,
			TRANSACTIONDATE_MONTH,
			TRANSACTIONDATE_YEAR,
			TRANSACTIONDATE_SETBUTTON,
			TRANSACTIONDATE_HOUR,
			TRANSACTIONDATE_MINUTE,
			TRANSACTIONDATE_SECOND,
			TRANSACTIONDATE_AMPM,

			INVENTORYDATE_DAY,
			INVENTORYDATE_MONTH,
			INVENTORYDATE_YEAR,
			INVENTORYDATE_SETBUTTON,

			DATE3_DAY,
			DATE3_MONTH,
			DATE3_YEAR,
			DATE3_SETBUTTON,
			DATE3_HOUR,
			DATE3_MINUTE,
			DATE3_SECOND,
			DATE3_AMPM,

			USERDATA01,
			USERDATA02,
			USERDATA03,
			USERDATA04,
			USERDATA05,
			USERDATA06,
			USERDATA07,
			USERDATA08,
			USERDATA09,
			USERDATA10,
			USERDATA11,
			USERDATA12,
			USERDATA13,
			USERDATA14,
			USERDATA15,
			USERDATA16,
			USERDATA17,
			USERDATA18,
			USERDATA19,
			USERDATA20,
			USERDATA21,
			USERDATA22,
			USERDATA23,
			USERDATA24,

			// line item fields
			LINEITEM_GROSSQUANTITY,
			LINEITEM_ALTERNATIVEUNITS,
			LINEITEM_ALTERNATIVEGROSSQUANTITY,
			LINEITEM_PRODUCTPRICE,
			LINEITEM_TRANSACTIONSTATUS,
			LINEITEM_TRANSACTIONQUALITY,
			LINEITEM_SELECTEDQUALITY,
			LINEITEM_TOTALPRICEWITHTAX,
			LINEITEM_TOTALVALUE,
			LINEITEM_TAX1,
			LINEITEM_TAX2,
			LINEITEM_TAX3,
			LINEITEM_DELIVERYLOCATION,
			LINEITEM_REQUESTEDBY,
			LINEITEM_NUMBER2,
			LINEITEM_NUMBER3,
			LINEITEM_NUMBER4,
			LINEITEM_NUMBER5,
			LINEITEM_NUMBER6,
			LINEITEM_RECEIPTVARIANCE,
			LINEITEM_TEMPERATURE,
			LINEITEM_DENSITY,
			LINEITEM_VCF,
			LINEITEM_NETQUANTITY,
			LINEITEM_ALTERNATIVENETVOLUME,
			LINEITEM_TOTALPRICEAMOUNT,
			LINEITEM_PRODUCT,
			LINEITEM_FROM_PRODUCT,
			LINEITEM_TO_PRODUCT,
			LINEITEM_NON_DOMESTIC_PRICE,
			LINEITEM_FOREIGN_CURRENCY,
			LINEITEM_CURRENCYUNITLABEL,
			LINEITEM_TOTALFOREIGNPRICE,
			LINEITEM_METERSTART,
			LINEITEM_METERSTARTTIME,
			LINEITEM_METERSTOP,
			LINEITEM_METERSTOPTIME,
			LINEITEM_TOTALONCOST,
			LINEITEM_STORAGELOCATIONID,
			LINEITEM_TOSTORAGELOCATIONID,
			LINEITEM_FROMSTORAGELOCATIONID,
			LINEITEM_REQUESTEDDELIVERYDATE,
			LINEITEM_ONCOST,
			LINEITEM_REQUESTEDDATETIME,

			// Line Item user data fields
			LINEITEM_USERDATA01,
			LINEITEM_USERDATA02,
			LINEITEM_USERDATA03,
			LINEITEM_USERDATA04,
			LINEITEM_USERDATA05,
			LINEITEM_USERDATA06,
			LINEITEM_USERDATA07,
			LINEITEM_USERDATA08,
			LINEITEM_USERDATA09,
			LINEITEM_USERDATA10,
			LINEITEM_USERDATA11,
			LINEITEM_USERDATA12,
			LINEITEM_USERDATA13,
			LINEITEM_USERDATA14,
			LINEITEM_USERDATA15,
			LINEITEM_USERDATA16,
			LINEITEM_USERDATA17,
			LINEITEM_USERDATA18,
			LINEITEM_USERDATA19,
			LINEITEM_USERDATA20,
			LINEITEM_USERDATA21,
			LINEITEM_USERDATA22,
			LINEITEM_USERDATA23,
			LINEITEM_USERDATA24
		}

		// different types of field groups
		public enum FieldGroups
		{
			// header fields
			INVENTORYDATE,
			TRANSACTIONDATE
		}

		protected static Hashtable controlTable = new Hashtable ( )
		{
			// header fields
			{FieldControls.TRANSACTIONSTATUS,      "TransactionFields.TransactionStatusFG"},
			{FieldControls.DOCUMENTNUMBER,         "TransactionFields.DocumentNumberFG"},
			{FieldControls.TOTALPRICEAMOUNT,       "TransactionFields.TotalPriceAmountFG"},
			{FieldControls.TOTALPRICEWITHTAX,      "TransactionFields.TotalPriceWithTaxFG"},
			{FieldControls.TOTALONCOST,            "TransactionFields.TotalOnCostFG"},
			{FieldControls.USERDATA3,              "TransactionFields.UserDataTextFGTAUD3"},
			{FieldControls.BILL_TO,                "TransactionFields.BillToFG"},
			{FieldControls.FROM_BILL_TO,           "TransactionFields.FromBillToFG"},
			{FieldControls.TO_BILL_TO,             "TransactionFields.ToBillToFG"},
			{FieldControls.ROSUPPLIER,             "TransactionFields.ROSupplierFG"},
			{FieldControls.TOTALFOREIGNPRICE,      "TransactionFields.TotalForeignPriceFG"},
			{FieldControls.ADFTRANSACTIONDATETIME, "TransactionFields.ADFTransactionDateTimeFG DateTime"},
			{FieldControls.TOTALEXCISE,            "TransactionFields.TotalExciseFG"},
			{FieldControls.TOTALGST,               "TransactionFields.TotalGSTFG"},
			{FieldControls.TOTALMARKUP,            "TransactionFields.TotalMarkupFG"},
			{FieldControls.FROM_SHIPTO,            "TransactionFields.FromShipToFG"},
			{FieldControls.TO_SHIPTO,              "TransactionFields.ToShipToFG"},
			{FieldControls.SHIPTO,                 "TransactionFields.ShipToFG"},
			{FieldControls.SHIPPER,                "TransactionFields.ShipperFG"},
			{FieldControls.COUNTRY,                "TransactionFields.CountryFG"},
			{FieldControls.DESTINATIONEQUIPMENTFG1,"TransactionFields.DestinationEquipmentFG1"},
			{FieldControls.SOURCEEQUIPMENTFG1,     "TransactionFields.SourceEquipmentFG1"},
			
			// JS20100427
			{FieldControls.INVENTORYDATE_DAY, "TransactionFields.InventoryDateFG Date Month"},
			{FieldControls.INVENTORYDATE_MONTH, "TransactionFields.InventoryDateFG Date Day"},
			{FieldControls.INVENTORYDATE_YEAR, "TransactionFields.InventoryDateFG Date Year"}, 
			{FieldControls.INVENTORYDATE_SETBUTTON, "TransactionFields.InventoryDateFG Date SetButton"},
			{FieldControls.TRANSACTIONDATE_DAY, "TransactionFields.TransactionDateTimeFG DateTime Date Day"},
			{FieldControls.TRANSACTIONDATE_MONTH, "TransactionFields.TransactionDateTimeFG DateTime Date Month"},
			{FieldControls.TRANSACTIONDATE_YEAR, "TransactionFields.TransactionDateTimeFG DateTime Date Year"},
			{FieldControls.TRANSACTIONDATE_SETBUTTON, "TransactionFields.TransactionDateTimeFG DateTime Date SetButton"},
			{FieldControls.TRANSACTIONDATE_HOUR, "TransactionFields.TransactionDateTimeFG DateTime Time Hour"},
			{FieldControls.TRANSACTIONDATE_MINUTE, "TransactionFields.TransactionDateTimeFG DateTime Time Minute"},
			{FieldControls.TRANSACTIONDATE_SECOND, "TransactionFields.TransactionDateTimeFG DateTime Time Second"},
			{FieldControls.TRANSACTIONDATE_AMPM, "TransactionFields.TransactionDateTimeFG DateTime Time AM/PM"},

			// JS20100508
			{FieldControls.DATE3_DAY,       "TransactionFields.Date03FG DateTime Date Day"},
			{FieldControls.DATE3_MONTH,     "TransactionFields.Date03FG DateTime Date Month"},
			{FieldControls.DATE3_YEAR,      "TransactionFields.Date03FG DateTime Date Year"},
			{FieldControls.DATE3_SETBUTTON, "TransactionFields.Date03FG DateTime Date SetButton"},
			{FieldControls.DATE3_HOUR,      "TransactionFields.Date03FG DateTime Time Hour"},
			{FieldControls.DATE3_MINUTE,    "TransactionFields.Date03FG DateTime Time Minute"},
			{FieldControls.DATE3_SECOND,    "TransactionFields.Date03FG DateTime Time Second"},
			{FieldControls.DATE3_AMPM,      "TransactionFields.Date03FG DateTime Time AM/PM"},

			{FieldControls.USERDATA01, "TransactionFields.UserDataListFGTAUD1"},
			{FieldControls.USERDATA02, "TransactionFields.UserDataListFGTAUD2"},
			{FieldControls.USERDATA03, "TransactionFields.UserDataListFGTAUD3"},
			{FieldControls.USERDATA04, "TransactionFields.UserDataListFGTAUD4"},
			{FieldControls.USERDATA05, "TransactionFields.UserDataListFGTAUD5"},
			{FieldControls.USERDATA06, "TransactionFields.UserDataListFGTAUD6"},
			{FieldControls.USERDATA07, "TransactionFields.UserDataListFGTAUD7"},
			{FieldControls.USERDATA08, "TransactionFields.UserDataListFGTAUD8"},
			{FieldControls.USERDATA09, "TransactionFields.UserDataListFGTAUD9"},
			{FieldControls.USERDATA10, "TransactionFields.UserDataListFGTAUD10"},
			{FieldControls.USERDATA11, "TransactionFields.UserDataListFGTAUD11"},
			{FieldControls.USERDATA12, "TransactionFields.UserDataListFGTAUD12"},
			{FieldControls.USERDATA13, "TransactionFields.UserDataListFGTAUD13"},
			{FieldControls.USERDATA14, "TransactionFields.UserDataListFGTAUD14"},
			{FieldControls.USERDATA15, "TransactionFields.UserDataListFGTAUD15"},
			{FieldControls.USERDATA16, "TransactionFields.UserDataListFGTAUD16"},
			{FieldControls.USERDATA17, "TransactionFields.UserDataListFGTAUD17"},
			{FieldControls.USERDATA18, "TransactionFields.UserDataListFGTAUD18"},
			{FieldControls.USERDATA19, "TransactionFields.UserDataListFGTAUD19"},
			{FieldControls.USERDATA20, "TransactionFields.UserDataListFGTAUD20"},
			{FieldControls.USERDATA21, "TransactionFields.UserDataListFGTAUD21"},
			{FieldControls.USERDATA22, "TransactionFields.UserDataListFGTAUD22"},
			{FieldControls.USERDATA23, "TransactionFields.UserDataListFGTAUD23"},
			{FieldControls.USERDATA24, "TransactionFields.UserDataListFGTAUD24"},

			// line item fields
			{FieldControls.LINEITEM_GROSSQUANTITY,            "TransactionFields.LineItemGrossQuantityFG"},
			{FieldControls.LINEITEM_ALTERNATIVEUNITS,         "TransactionFields.LineItemAlternativeUnitsFG"},
			{FieldControls.LINEITEM_ALTERNATIVEGROSSQUANTITY, "TransactionFields.LineItemAlternativeGrossVolumeFG"},
			{FieldControls.LINEITEM_PRODUCTPRICE,             "TransactionFields.LineItemProductPriceFG"},
			{FieldControls.LINEITEM_TRANSACTIONSTATUS,        "TransactionFields.LineItemTransactionStatusFG"},
			{FieldControls.LINEITEM_TRANSACTIONQUALITY,       "TransactionFields.LineItemTransactionQualityFG"},
			{FieldControls.LINEITEM_SELECTEDQUALITY,          "TransactionFields.LineItemSelectedQualityFG"},
			{FieldControls.LINEITEM_TOTALPRICEWITHTAX,        "TransactionFields.LineItemTotalPriceWithTaxFG"},
			{FieldControls.LINEITEM_TOTALVALUE,               "TransactionFields.LineItemTotalValueFG"},
			{FieldControls.LINEITEM_TAX1,                     "TransactionFields.LineItemTax1FG"},
			{FieldControls.LINEITEM_TAX2,                     "TransactionFields.LineItemTax2FG"},
			{FieldControls.LINEITEM_TAX3,                     "TransactionFields.LineItemTax3FG"},
			{FieldControls.LINEITEM_DELIVERYLOCATION,         "TransactionFields.LineItemDeliveryLocationFG"},
			{FieldControls.LINEITEM_REQUESTEDBY,              "TransactionFields.LineItemRequestedByFG"},
			{FieldControls.LINEITEM_NUMBER2,                  "TransactionFields.LineItemNumber02FG"},
			{FieldControls.LINEITEM_NUMBER3,                  "TransactionFields.LineItemNumber03FG"},
			{FieldControls.LINEITEM_NUMBER4,                  "TransactionFields.LineItemNumber04FG"},
			{FieldControls.LINEITEM_NUMBER5,                  "TransactionFields.LineItemNumber05FG"},
			{FieldControls.LINEITEM_NUMBER6,                  "TransactionFields.LineItemNumber06FG"},
			{FieldControls.LINEITEM_RECEIPTVARIANCE,          "TransactionFields.LineItemReceiptVarianceFG"},
			{FieldControls.LINEITEM_TEMPERATURE,              "TransactionFields.LineItemTemperatureFG"},
			{FieldControls.LINEITEM_DENSITY,                  "TransactionFields.LineItemDensityFG"},
			{FieldControls.LINEITEM_VCF,                      "TransactionFields.LineItemVCF_FG"},
			{FieldControls.LINEITEM_NETQUANTITY,              "TransactionFields.LineItemNetQuantityFG"},
			{FieldControls.LINEITEM_ALTERNATIVENETVOLUME,     "TransactionFields.LineItemAlternativeNetVolumeFG"},
			{FieldControls.LINEITEM_TOTALPRICEAMOUNT,         "TransactionFields.LineItemTotalPriceAmountFG"},
			{FieldControls.LINEITEM_PRODUCT,                  "TransactionFields.LineItemProductFG"},
			{FieldControls.LINEITEM_FROM_PRODUCT,             "TransactionFields.LineItemFromProductFG"},
			{FieldControls.LINEITEM_TO_PRODUCT,               "TransactionFields.LineItemToProductFG"},
			{FieldControls.LINEITEM_NON_DOMESTIC_PRICE,       "TransactionFields.LineItemNonDomesticPriceFG"},
			{FieldControls.LINEITEM_FOREIGN_CURRENCY,         "TransactionFields.LineItemCurrencyUnitFG"},
			{FieldControls.LINEITEM_CURRENCYUNITLABEL,        "TransactionFields.LineItemCurrencyUnitLabelFG"},
			{FieldControls.LINEITEM_TOTALFOREIGNPRICE,        "TransactionFields.LineItemTotalForeignPriceFG"},
			{FieldControls.LINEITEM_METERSTART,               "TransactionFields.LineItemMeterStartFG"},
			{FieldControls.LINEITEM_METERSTARTTIME,           "TransactionFields.LineItemMeterStartTimeFG"},
			{FieldControls.LINEITEM_METERSTOP,                "TransactionFields.LineItemMeterStopFG"},
			{FieldControls.LINEITEM_METERSTOPTIME,            "TransactionFields.LineItemMeterStopTimeFG"},
			{FieldControls.LINEITEM_TOTALONCOST,              "TransactionFields.LineItemTotalOnCostFG"},
			{FieldControls.LINEITEM_STORAGELOCATIONID,        "TransactionFields.LineItemStorageLocationFG"},
			{FieldControls.LINEITEM_FROMSTORAGELOCATIONID,    "TransactionFields.LineItemFromStorageLocationFG"},
			{FieldControls.LINEITEM_TOSTORAGELOCATIONID,      "TransactionFields.LineItemToStorageLocationFG"},
			{FieldControls.LINEITEM_REQUESTEDDELIVERYDATE,     "TransactionFields.LineItemRequestedDeliveryDateFG"},
			{FieldControls.LINEITEM_REQUESTEDDATETIME,        "TransactionFields.LineItemRequestedDateTimeFG DateTime"},
			{FieldControls.LINEITEM_ONCOST,     "TransactionFields.LineItemOnCostFG"},
			
			// Line Item UserData fields
			{FieldControls.LINEITEM_USERDATA01, "TransactionFields.UserDataTextFGTALUD1"},
			{FieldControls.LINEITEM_USERDATA02, "TransactionFields.UserDataTextFGTALUD2"},
			{FieldControls.LINEITEM_USERDATA03, "TransactionFields.UserDataTextFGTALUD3"},
			{FieldControls.LINEITEM_USERDATA04, "TransactionFields.UserDataTextFGTALUD4"},
			{FieldControls.LINEITEM_USERDATA05, "TransactionFields.UserDataTextFGTALUD5"},
			{FieldControls.LINEITEM_USERDATA06, "TransactionFields.UserDataTextFGTALUD6"},
			{FieldControls.LINEITEM_USERDATA07, "TransactionFields.UserDataTextFGTALUD7"},
			{FieldControls.LINEITEM_USERDATA08, "TransactionFields.UserDataTextFGTALUD8"},
			{FieldControls.LINEITEM_USERDATA09, "TransactionFields.UserDataTextFGTALUD9"},
			{FieldControls.LINEITEM_USERDATA10, "TransactionFields.UserDataTextFGTALUD10"},
			{FieldControls.LINEITEM_USERDATA11, "TransactionFields.UserDataTextFGTALUD11"},
			{FieldControls.LINEITEM_USERDATA12, "TransactionFields.UserDataTextFGTALUD12"},
			{FieldControls.LINEITEM_USERDATA13, "TransactionFields.UserDataTextFGTALUD13"},
			{FieldControls.LINEITEM_USERDATA14, "TransactionFields.UserDataTextFGTALUD14"},
			{FieldControls.LINEITEM_USERDATA15, "TransactionFields.UserDataTextFGTALUD15"},
			{FieldControls.LINEITEM_USERDATA16, "TransactionFields.UserDataTextFGTALUD16"},
			{FieldControls.LINEITEM_USERDATA17, "TransactionFields.UserDataTextFGTALUD17"},
			{FieldControls.LINEITEM_USERDATA18, "TransactionFields.UserDataTextFGTALUD18"},
			{FieldControls.LINEITEM_USERDATA19, "TransactionFields.UserDataTextFGTALUD19"},
			{FieldControls.LINEITEM_USERDATA20, "TransactionFields.UserDataTextFGTALUD20"},
			{FieldControls.LINEITEM_USERDATA21, "TransactionFields.UserDataTextFGTALUD21"},
			{FieldControls.LINEITEM_USERDATA22, "TransactionFields.UserDataTextFGTALUD22"},
			{FieldControls.LINEITEM_USERDATA23, "TransactionFields.UserDataTextFGTALUD23"},
			{FieldControls.LINEITEM_USERDATA24, "TransactionFields.UserDataTextFGTALUD24"}
		};

		protected static Hashtable groupControlTable = new Hashtable ( )
		{
			{
				FieldGroups.INVENTORYDATE, new List<FieldControls>()
				{
					FieldControls.INVENTORYDATE_YEAR,
					FieldControls.INVENTORYDATE_MONTH,
					FieldControls.INVENTORYDATE_DAY,
					FieldControls.INVENTORYDATE_SETBUTTON
				}
			},
			{

				FieldGroups.TRANSACTIONDATE, new List<FieldControls>()
				{
					FieldControls.TRANSACTIONDATE_YEAR,
					FieldControls.TRANSACTIONDATE_MONTH,
					FieldControls.TRANSACTIONDATE_DAY,
					FieldControls.TRANSACTIONDATE_SETBUTTON,
					FieldControls.TRANSACTIONDATE_HOUR,
					FieldControls.TRANSACTIONDATE_MINUTE,
					FieldControls.TRANSACTIONDATE_SECOND,
					FieldControls.TRANSACTIONDATE_AMPM
				}
			}
		};

		protected static Hashtable fieldFnTable = new Hashtable ( )
		{
			{typeof(TextBox), new SetControlState(Common.SetTextboxState)},
			{typeof(DropDownList), new SetControlState(Common.SetDropdownState)},
			{typeof(HtmlSelect), new SetControlState(Common.SetHtmlSelectState)},
			{typeof(FMProductTextBox), new SetControlState(Common.SetFMProductTextBoxState)},
			{typeof(FMCompanyTextBox), new SetControlState(Common.SetFMCompanyTextBoxState)},
			{typeof(FMCalendarSetLinkButton), new SetControlState(Common.SetCalendarLinkButtonState)}
		};

		protected static Hashtable fieldValueFnTable = new Hashtable ( )
		{
			{typeof(TextBox), new SetControlValue(Common.SetTextboxValue)}
		};

		#endregion // Constants

		public static List<FieldControls> GetFieldControlsFromGroup ( FieldGroups a_group )
		{
			if (!groupControlTable.Contains ( a_group ))
			{
				return null; // failsafe
			}

			List<FieldControls> controlList = groupControlTable[a_group] as List<FieldControls>;

			return controlList;
		}

		public static SetControlState GetControlDelegate ( Type a_type )
		{
			SetControlState returnVal = null;

			if (Common.fieldFnTable.Contains ( a_type ))
			{
				returnVal = Common.fieldFnTable[a_type] as SetControlState;
			}

			return returnVal;
		}

		// field names
		public static string FieldControlName ( FieldControls a_control )
		{
			string result = null;

			if (controlTable.ContainsKey ( a_control ))
			{
				result = controlTable[a_control] as string;
			}

			return result;
		}

		public delegate TransactionFilterSR FilterBuilderDelegate ( ICustomContext a_context );
		public delegate bool InlineFilterDelegate ( TransactionDO a_trans );
		public delegate ICustomContext GetContext ( );
		public delegate ICustomContext LoadToContext ( ref Object a_context );
		public delegate void StoreContext ( Object a_context );

		public static void RefreshPreProcessing (	ref FMControls.FMDataGrid a_grid,
													GetContext a_getContextDelegate,
													LoadToContext a_loadToContextDelegate,
													StoreContext a_storeContextDelegate )
		{

			// go back to first page when keyword changed because the currently selected page may no longer exist
			a_grid.CurrentPageIndex = 0;
			ICustomContext context = a_getContextDelegate ( );
			Object temp = context;
			context = (ICustomContext) a_loadToContextDelegate ( ref temp );
			context = temp as ICustomContext;
			a_storeContextDelegate ( context );
		}

		public static void PageChangePreProcessing ( ref FMControls.FMDataGrid a_grid, DataGridPageChangedEventArgs e )
		{
			if (a_grid.EditItemIndex > -1)
			{
				return;
			}

			a_grid.CurrentPageIndex = e.NewPageIndex;
		}

		public static TransactionDOCollection EnumerateByContext ( ICustomContext a_context, 
																	SecurityClass a_security, 
																	AccountingSite a_accountingSite,
																	FilterBuilderDelegate a_filterBuilder, 
																	InlineFilterDelegate a_inlineFilter )
		{
			TransactionDOCollection results = new TransactionDOCollection ( );

			TransactionFilterSR filterSR = a_filterBuilder ( a_context );

			FMChannelFactory<ITransactionFilterProcessor> txFilterProcessorClient = new FMChannelFactory<ITransactionFilterProcessor> ( );
			ITransactionFilterProcessor txFilterProcessor = txFilterProcessorClient.CreateProxy ( );

			GetTransactionDO ds = txFilterProcessor.Process ( filterSR );

			// manually filter the rest
			if (ds != null)
			{
				for (int i = 0; i < ds.TransactionDataSet.Tables[0].Rows.Count; ++i)
				{
					string transID = (string) ds.TransactionDataSet.Tables[0].Rows[i]["TransID"];

					// retrieve the transaction in question
					TransactionSR tranSR = new TransactionSR ( );
					tranSR.Security = a_security;
					tranSR.AccountingSite = a_accountingSite;
					tranSR.TransID = transID;

					TransactionDO trans = null;
					try
					{
						FMChannelFactory<ITransactionProcessor> txProcessorClient = new FMChannelFactory<ITransactionProcessor> ( );
						ITransactionProcessor txProcessor = txProcessorClient.CreateProxy ( );
						trans = txProcessor.Process ( tranSR );
					}
					catch (Exception)
					{
						continue;
					}

					if (trans != null)
					{
						if (a_inlineFilter ( trans ))
						{
							results.Add ( trans );
						}
					} // matches criterias defined in delegate
				} // transaction is not null
			} // recurse through the DS for transactions

			return results;
		}

		public enum FilterTarget : short
		{
			USER,
			SECTION,
			INVOICE_ACCOUNT_CODE,
			INVOICE_COST_CENTRE_CODE
		}

		public static string GetValueForFiltering ( FMDropDownList a_list )
		{
			string result = "";

			ListItem item = a_list.SelectedItem;
			if (item != null)
			{
				if (!item.Text.Equals ( Common.DDL_ALL ))
				{
					result = item.Text;
				}
			}

			return result;
		}

		public static void PopulateFilterDropDownList ( SecurityClass a_security, ref FMDropDownList a_list, FilterTarget a_target, bool a_wildcard )
		{
			a_list.Items.Clear ( );

			if (a_wildcard)
			{
				a_list.Items.Add ( Common.DDL_ALL );
				a_list.SelectByText ( Common.DDL_ALL );
			}

			switch (a_target)
			{
				case FilterTarget.USER:
					{
						FMChannelFactory<IUsers> usersClient = new FMChannelFactory<IUsers> ( );
						IUsers users = usersClient.CreateProxy ( );

						// give view user right to be able to pull the users
						SecurityClass tempSecurity = a_security;
						tempSecurity.RightCollection.Add ( RIGHT.VIEW_USERS );

						UserCollectionClass col = users.Enumerate ( tempSecurity );

						for (int nextRight = tempSecurity.RightCollection.Count - 1; nextRight >= 0; --nextRight)
						{
							if (tempSecurity.RightCollection[nextRight] == RIGHT.VIEW_USERS)
							{
								tempSecurity.RightCollection.RemoveAt ( nextRight );
								break;
							}
						}

						foreach (UserClass user in col)
						{
							a_list.Items.Add ( user.ID );
						}
						break;
					}
				case FilterTarget.SECTION:
					{
						PopulateFromUserDataListValues ( a_security, ref a_list, "INVOICE", USERDATA_SECTION_KEY );

						break;
					}
				case FilterTarget.INVOICE_ACCOUNT_CODE:
					{
						PopulateFromUserDataListValues ( a_security, ref a_list, "INVOICE", USERDATA_ACCTCODE_KEY );

						break;
					}
				case FilterTarget.INVOICE_COST_CENTRE_CODE:
					{
						PopulateFromUserDataListValues ( a_security, ref a_list, "INVOICE", USERDATA_COSTCENTRE_KEY );

						break;
					}
				default:
					throw new Exception ( "Unknown target " + Enum.GetName ( typeof ( FilterTarget ), a_target ) );
			}
		}

		protected static void PopulateFromUserDataListValues ( SecurityClass a_security, ref FMDropDownList a_list, string a_aliasName, string a_fieldName )
		{
			FMChannelFactory<ITransactionAliases> aliasesClient = new FMChannelFactory<ITransactionAliases> ( );
			ITransactionAliases alises = aliasesClient.CreateProxy ( );

			TransactionAliasCollectionClass col = alises.EnumerateByTransTypeID ( a_security, TransactionTypes.T21_AccountPayableInvoice );

			foreach (TransactionAliasClass alias in col)
			{
				if (alias.ID.ToUpper ( ).Equals ( a_aliasName ))
				{
					// get the alias user data fields
					TransactionAliasClass cAlias = alises.Get ( a_security, alias.IdentityGuid, false );

					for (int nextAlias = 0; nextAlias < cAlias.UserDataFieldCollection.Count; ++nextAlias)
					{
						UserDataFieldClass field = (UserDataFieldClass) cAlias.UserDataFieldCollection.Item ( nextAlias );

						if (field.ID.Equals ( a_fieldName ))
						{
							foreach (UserDataListValueClass value in field.UserDataListValueCollection)
							{
								a_list.Items.Add ( value.ID.Trim ( ) );
							}
							break;
						}
					}
					break;
				}
			}
		}

		public static SetControlValue GetControlValueDelegate ( Type a_type )
		{
			SetControlValue returnVal = null;

			if (Common.fieldValueFnTable.Contains ( a_type ))
			{
				returnVal = Common.fieldValueFnTable[a_type] as SetControlValue;
			}

			return returnVal;
		}

		public static void SetFieldValue ( Object a_value, ref Control a_ctrl )
		{
			Type controlType = a_ctrl.GetType ( );

			SetControlValue del = Common.GetControlValueDelegate ( controlType );
			if (del != null)
			{
				del ( a_value, ref a_ctrl );
			}
		}

		public delegate void SetControlValue ( Object a_value, ref Control a_ctrl );
		public static void SetTextboxValue ( Object a_value, ref Control a_ctrl )
		{
			TextBox tb = a_ctrl as TextBox;
			if (tb != null)
			{
				tb.Text = a_value.ToString ( );
			}
		}

		public static void EnableDisableField ( bool a_enable, ref Control a_ctrl )
		{
			Type controlType = a_ctrl.GetType ( );

			SetControlState del = Common.GetControlDelegate ( controlType );
			if (del != null)
			{
				del ( a_enable, ref a_ctrl );
			}
		}

		public delegate void SetControlState ( bool a_enable, ref Control a_ctrl );
		public static void SetTextboxState ( bool a_enable, ref Control a_ctrl )
		{
			TextBox tb = a_ctrl as TextBox;

			if (tb != null)
			{
				//tb.Enabled = a_enable;
				tb.BackColor = a_enable ? System.Drawing.Color.White : System.Drawing.Color.LightGray;
				tb.ReadOnly = !a_enable;

				if (a_ctrl.ID.ToUpper ( ).Contains ( "ProductPrice" ))
				{
					// product price needs to be enabled/disabled due to many javascript functions setting them as not readonly
					tb.Enabled = a_enable;
				}

				a_ctrl = tb;
			}
		}

		public static void SetDropdownState ( bool a_enable, ref Control a_ctrl )
		{
			DropDownList ddl = a_ctrl as DropDownList;

			if (ddl != null)
			{
				ddl.Enabled = a_enable;
				ddl.BackColor = a_enable ? System.Drawing.Color.White : System.Drawing.Color.LightGray;
			}
		}

		public static void SetHtmlSelectState ( bool a_enable, ref Control a_ctrl )
		{
			HtmlSelect select = a_ctrl as HtmlSelect;

			if (select != null)
			{
				select.Disabled = !a_enable;
			}
		}

		/// <summary>
		/// This method is a delegate to handle disabling the FM Company Text Box control.
		/// </summary>
		/// <param name="enabled"></param>
		/// <param name="inControl"></param>
		public static void SetFMCompanyTextBoxState ( bool enabled, ref Control inControl )
		{
			FMCompanyTextBox control = inControl as FMCompanyTextBox;

			if (control != null)
			{
				control.ReadOnly = !enabled;
			}
		}

		/// <summary>
		/// This method is a delegate to handle disabling the FM Product Text Box control.
		/// </summary>
		/// <param name="enabled"></param>
		/// <param name="inControl"></param>
		public static void SetFMProductTextBoxState ( bool enabled, ref Control inControl )
		{
			FMProductTextBox control = inControl as FMProductTextBox;

			if (control != null)
			{
				control.ReadOnly = !enabled;
			}
		}

		public static void SetCalendarLinkButtonState ( bool a_enable, ref Control a_ctrl )
		{
			FMCalendarSetLinkButton btn = a_ctrl as FMCalendarSetLinkButton;

			if (btn != null)
			{
				btn.Enabled = false;
			}
		}

		public static Control FindControlFrom ( Control a_ctrl, FieldControls a_id )
		{
			string fieldName = Common.FieldControlName ( a_id );
			Control ctrl = a_ctrl.FindControl ( fieldName );

			return ctrl;
		}

		public static bool TransactionImpactsInventory ( TransactionDO a_trans )
		{
			switch (a_trans.TransTypeID)
			{
				case TransactionTypes.T7_FillStand:
				case TransactionTypes.T9_Request:
				case TransactionTypes.T10_Unload:
				case TransactionTypes.T11_ConsumerTransfer:
				case TransactionTypes.T12_InventoryNotAffected:
				case TransactionTypes.T17_Order:
				case TransactionTypes.T18_SupplyOrder:
				case TransactionTypes.T19_EndOfDay:
				case TransactionTypes.T20_EndOfMonth:
				case TransactionTypes.T21_AccountPayableInvoice:
				case TransactionTypes.T22_AccountReceivableInvoice:
				case TransactionTypes.T23_StorageTransfer:
				case TransactionTypes.T14_PhysicalInventory:
					return false;
			}

			return true;
		}

		public static LineItemDO AggregateLineItemValues ( SecurityClass security, TransactionDO trans, int itemIndex )
		{
			FMChannelFactory<IAccountingSites> accountingSitesClient = new FMChannelFactory<IAccountingSites> ( );
			IAccountingSites accountingSites = accountingSitesClient.CreateProxy ( );

			AccountingSite accountingSite = accountingSites.LoadSiteInfo(security, security.SiteGuid);

			FMChannelFactory<IPriceCalculatorInvoker> priceInvokerClient = new FMChannelFactory<IPriceCalculatorInvoker> ( );
			IPriceCalculatorInvoker invoker = priceInvokerClient.CreateProxy ( );

			if (itemIndex < 0 && trans.LineItems.Count <= itemIndex)
			{
				return null; // failsafe
			}

			LineItemDO li = trans.LineItems[itemIndex] as LineItemDO;

			// find all the line item IDs to aggregate from
			ArrayList lineItemGuids = new ArrayList ( );
			foreach (AssociatedTxDO atx in li.AssociatedTransactions)
			{
				lineItemGuids.Add ( atx.TransactionLineItemGuid );
			}

			// quick aggregation
			if (trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_FUEL_ORDER ) ||
			   trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice ||
			   trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				// create the aggregation service request
				TxAggregationSR aggregationSr = new TxAggregationSR ( );
				aggregationSr.Security = security;
				aggregationSr.ParentTransTypeID = trans.TransTypeID;
				aggregationSr.AtxLineItemGuids = lineItemGuids;

				FMChannelFactory<ITxAggregationProcessor> txAggProcessorClient = new FMChannelFactory<ITxAggregationProcessor> ( );
				ITxAggregationProcessor txAggProcessor = txAggProcessorClient.CreateProxy ( );
				TxAggregationDO aggregatedDo = txAggProcessor.Process ( aggregationSr );

				if (aggregatedDo != null)
				{
					double oncostAmount = 0.0;
					try
					{
						oncostAmount = double.Parse ( li.UserData["TALUD14"].ToString ( ) );
					}
					catch (Exception) { }

					switch (trans.TransTypeID)
					{
						// supply order only aggregates the quantity
						case TransactionTypes.T18_SupplyOrder:
							if (li.Quantity.Gross == 0)
								li.Quantity.Gross = aggregatedDo.Quantity;
							break;
						// invoices aggregate most things
						case TransactionTypes.T21_AccountPayableInvoice:
							li.Quantity.Gross = aggregatedDo.Quantity;
							li.Tax1 = aggregatedDo.Excise;
							li.Tax2 = aggregatedDo.Gst;
							li.Tax3 = aggregatedDo.Margin;
							li.TotalValue = aggregatedDo.TotalValue + oncostAmount;
							li.TotalPriceWithTax = aggregatedDo.TotalPriceWithTax + oncostAmount;
							li.UserData["TALUD3"] = aggregatedDo.TotalForeignPrice.ToString ( );
							break;
						case TransactionTypes.T22_AccountReceivableInvoice:
							li.Quantity.Gross = -aggregatedDo.Quantity; // reverse the quantity since sales are outgoings
							li.Tax1 = aggregatedDo.Excise;
							li.Tax2 = aggregatedDo.Gst;
							li.Tax3 = aggregatedDo.Margin;
							li.Number01 = aggregatedDo.Number01;
							li.Number02 = aggregatedDo.Number02;
							li.Number03 = aggregatedDo.Number03;
							li.Number04 = aggregatedDo.Number04;
							li.Number05 = aggregatedDo.Number05;
							li.Number06 = aggregatedDo.Number06;
							li.TotalValue = aggregatedDo.TotalValue;
							li.TotalPriceWithTax = aggregatedDo.TotalPriceWithTax;
							li.UserData["TALUD14"] = aggregatedDo.OnCost.ToString ( "N" );
							break;
					}
				}
				else
				{
					li.Quantity.Gross = 0;
					li.Quantity.Net = 0;
					li.Quantity.Mass = 0;
					li.Quantity.Package = 0;
					li.Tax1 = null;
					li.Tax2 = null;
					li.Tax3 = null;
					li.Number01 = null;
					li.Number02 = null;
					li.Number03 = null;
					li.Number04 = null;
					li.Number05 = null;
					li.Number06 = null;
					li.TotalValue = 0;
					li.TotalPriceWithTax = 0;
					li.UserData["TALUD14"] = "";
				}
			}

			// for receipts, we pull the fuel order supplier
			if (trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_RECEIPT ))
			{
				if (li.AssociatedTransactions.Count > 0)
				{
					AssociatedTxDO assocTx = li.AssociatedTransactions[0] as AssociatedTxDO;

					AssociatedTxSR sr = new AssociatedTxSR ( );
					sr.Security = security;
					sr.TransactionLineItemGuid = assocTx.TransactionLineItemGuid;
					sr.TransID = assocTx.TransID;
					sr.RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions;

					FMChannelFactory<IAssociatedTxProcessor> assocTxProcessorClient = new FMChannelFactory<IAssociatedTxProcessor> ( );
					IAssociatedTxProcessor assocTxProcessor = assocTxProcessorClient.CreateProxy ( );

					AssociatedTxListDO result = assocTxProcessor.Process ( sr );

					foreach (DataRow dr in result.AssociatedTransactions.Tables[0].Rows)
					{
						AssociatedTxDO demandParentTrans = new AssociatedTxDO ( );
						demandParentTrans.Load ( dr );

						if (demandParentTrans.TransactionAlias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_FUEL_ORDER ) &&
							demandParentTrans.LinkedTransactionLineItemGuid == assocTx.TransactionLineItemGuid &&
							demandParentTrans.Product.ToUpper ( ).Equals ( assocTx.Product.ToUpper ( ) ))
						{
							// set the supplier and work out the supplier index
							trans.SupplierID = demandParentTrans.SupplierID;

							FMChannelFactory<ICompanies> companiesClient = new FMChannelFactory<ICompanies> ( );
							ICompanies companies = companiesClient.CreateProxy ( );
							trans.SupplierCompanyGuid = companies.GetIdentityGuid ( security, trans.SupplierID );
						}
					}
				}
			}

			// JS20100907 WI-14875 allow fuel orders to pull in delivery date from demand
			else if (trans.Alias.ToUpper ( ).Equals ( TransactionDetail.ALIAS_FUEL_ORDER ))
			{
				if (li.AssociatedTransactions.Count > 0)
				{
					AssociatedTxDO demandTransaction = li.AssociatedTransactions[0] as AssociatedTxDO;

					if (demandTransaction.LineItemRequestedDateTime != null && li.RequestedDateTime == null)
					{
						FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
						ISites sites = sitesClient.CreateProxy ( );

						SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);
						SiteTimeConverter converter = new SiteTimeConverter ( site );
						DateTime requestedDateTime = converter.ConvertToSiteTime ( demandTransaction.LineItemRequestedDateTime.Value );

						li.RequestedDateTime = requestedDateTime;
					}
				}
			}

			return li;
		}
	}
}
