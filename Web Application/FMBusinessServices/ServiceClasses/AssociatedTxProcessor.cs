// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociatedTxProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AssociatedTxProcessorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// note: [ServiceContract] is inherited from IAssociatedTxProcessor
	/// </summary>
	public class AssociatedTxProcessorClass : IAssociatedTxProcessor
	{
		#region Private data members
		private AssociatedTxListDO associatedList;
		private AssociatedTxSR associatedSR;
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public AssociatedTxProcessorClass()
		{
			this.associatedList = null;
			this.associatedSR = null;
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		public AssociatedTxListDO Process(AssociatedTxSR inAssociatedSR)
		{
			this.associatedSR = inAssociatedSR;
			this.associatedList = new AssociatedTxListDO();

			switch (associatedSR.RequestType)
			{
				case AssociatedTxSR.RequestTypes.GetAssociatedTransactions:
					this.GetAssociatedTransactions();
					break;

				case AssociatedTxSR.RequestTypes.GetAssociatedTransactionDetails:
					this.GetAssociatedTransactionDetails(associatedSR.TransactionLineItemGuid);
					break;

				case AssociatedTxSR.RequestTypes.GetAvailableTransactions:
					this.GetAvailableTransactions();
					break;

				case AssociatedTxSR.RequestTypes.GetAssociatedAndAvailableTransactions:
					this.GetAssociatedTransactions();
					this.GetAvailableTransactions();
					break;

				case AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions:
					this.GetAssociatedParentTransactions();
					break;

				default:
					break;
			}

			return associatedList;
		}

		/// <summary>
		/// This method will get the available transactions that can be associated. This is based on the filters and project
		/// type (i.e. ADF).
		/// </summary>
		private void GetAvailableTransactions()
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				string sqlPart_enforce_1to1 = "AND l.TransactionLineItemGuid NOT IN (" +
								 "SELECT LinkedTransactionLineItemGuid FROM tblTransactionLinks tl1 LEFT JOIN tblTransactions t1 ON " +
								 "tl1.OriginalTransID = t1.TransID WHERE t1.TransactionAliasGuid = @TransactionAliasGuid2) ";

				string sql = "SELECT 0 AS Associated, t.TransID, t.AliasName, t.TransactionAliasGuid, t.LookupTransTypeIndex, t.SubType, t.Site, t.SiteGuid, " +
								"t.TransReferenceID, t.InventoryDate, t.ShipToID, t.ShipToCode, t.ShipToCompanyGuid, t.SupplierID, t.SupplierCode, " +
								"t.SupplierCompanyGuid, t.CreatedDate, t.CreatedBy, t.RequestedDeliveryDate, t.UpdatedDate, t.UpdatedBy, t.TransDateTime, " +
								"t.TransVersion, t.SCACCode, t.CardNumber, t.ShipmentNumber, t.ShipperID, t.ShipperCode, t.ShipperCompanyGuid, t.OwnerID, " +
								"t.OwnerCode, t.OwnerCompanyGuid, t.ManagerID, t.ManagerCode, t.ManagerCompanyGuid, t.CarrierID, t.CarrierCode, t.CarrierCompanyGuid, t.ConjoinTransID, " +
								"t.ReversedTransID, t.LinkedDocumentNumber, t.ReversalType, t.PONumber, t.TimeIn, t.TimeOut, t.TimeEnd, t.RoutingID, t.TicketSource, " +
								"t.LoadID, t.LookupTransactionStatusIndex, t.BillToID, t.BillToCode, t.BillToCompanyGuid, t.DriverIdentificationNumber, t.CreditAmount, t.CardExpiration, " +
								"t.CardName, t.CardType, t.CashAmount, t.RouteOriginationDate, t.InternationalRouteIndicator, t.PreviousRoutingID, " +
								"(SELECT IATAID FROM tblIATA WHERE t.FinalStationIATAGuid = IATAGuid) AS FinalStation, " +
								"(SELECT IATAID FROM tblIATA WHERE t.PreviousStationIATAGuid = IATAGuid) AS PreviousStation, " +
								"(SELECT IATAID FROM tblIATA WHERE t.NextStationIATAGuid = IATAGuid) AS NextStation, " +
								"(SELECT IATAID FROM tblIATA WHERE t.OriginStationIATAGuid = IATAGuid) AS OriginStation, " +
								"t.ShippingDocumentNumber, t.DocumentNumber, t.STD, t.ETD, t.STA, t.ETA, " +
								"t.SFT, t.FST, t.EstimatedFuelingDuration, t.DeleteFlag, t.TicketMode, t.DestinationRegistrationID1, t.DestinationSerialNumber1, " +
								"t.DestinationEquipmentType1, t.DestinationEquipmentModel1, t.DestinationCompanyEquipmentID1, t.Destination1EquipmentGuid, " +
								"t.DestinationRegistrationID2, t.DestinationSerialNumber2, t.DestinationEquipmentType2, t.DestinationEquipmentModel2, t.DestinationCompanyEquipmentID2, " +
								"t.Destination2EquipmentGuid, t.DestinationRegistrationID3, t.DestinationSerialNumber3, t.DestinationEquipmentType3, t.DestinationEquipmentModel3, " +
								"t.DestinationCompanyEquipmentID3, t.Destination3EquipmentGuid, t.SourceRegistrationID1, t.SourceSerialNumber1, t.SourceEquipmentType1, " +
								"t.SourceEquipmentModel1, t.SourceCompanyEquipmentID1, t.Source1EquipmentGuid, t.SourceRegistrationID2, t.SourceSerialNumber2, " +
								"t.SourceEquipmentType2, t.SourceEquipmentModel2, t.SourceCompanyEquipmentID2, t.Source2EquipmentGuid, t.SourceRegistrationID3, t.SourceSerialNumber3, " +
								"t.SourceEquipmentType3, t.SourceEquipmentModel3, t.SourceCompanyEquipmentID3, t.Source3EquipmentGuid, t.OperatorID, t.OperatorPersonnelGuid, " +
								"t.EffectiveDate, t.ExpirationDate, t.ScheduledDate, t.AutoComplete, t.Flag01, t.Flag02, t.Flag03, t.Flag04, t.Flag05, t.Flag06, " +
								"t.FuelAdditiveFlag, t.IssuePoint, t.IssuePointNumber, t.RadioNumber, t.GateID, t.GateGuid, " +
								"t.Number01, t.Number02, t.Number03, t.Number04, t.Number05, t.Number06, t.ContactFirstName, t.ContactSurname, t.Date01, t.Date02, t.Date03, t.Date04, " +
								"t.LegacyNumber, t.Country, t.ContactInfo, t.AssociatedDocNumber, t.AssociatedCLIN, t.SubmittedToAccounting, t.LookupOriginApplicationIndex, t.FuelCardGuid, " +
								"t.FuelCardID, t.AssociatedTransportOrderNumber, t.RequestedDateTime, t.DispatchedDateTime, t.ErrorFlag, t.TransactionGuid, " +
								"l.TransactionLineItemGuid, l.Product, l.DeliveryLocation, " +
								"l.GrossQuantity, l.Tax1 AS Excise, l.Tax2 AS GST, l.Tax3 AS Markup, l.ProductPrice, l.LookupTransactionStatusIndex AS LineItemStatus, l.CurrencyGuid, " +
								"l.RequestedDateTime AS LineItemRequestedDateTime, l.AlternativeNetVolume " +
								"FROM  tblTransactions t JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid " +
									"AND l.Product = @Product  ";

				if (this.associatedSR.ProjectType == AssociatedTxSR.ProjectTypes.ADF)
				{
					if (this.associatedSR.TransTypeID != TransactionTypes.T8_Receipt)
					{
						// receipts associated demands, but demands must not have linkage restriction'
						sql += sqlPart_enforce_1to1;
					}
				}
				else
				{
					sql += sqlPart_enforce_1to1;
				}

				sql += "WHERE ((t.SiteGuid = @SiteGuid) OR (t.SiteGuid IN " +
										"(SELECT ChildSiteGuid FROM map.tblSiteToSite WHERE " +
										"ParentSiteGuid = @SiteGuid))) " +
                                      "AND t.TransactionAliasGuid IN " +
                                      "(SELECT txa._MasterRecordGuid FROM map.tblAssociatedTransactionAliases ata INNER JOIN tblTransactionAliases txa ON txa.TransactionAliasGuid = ata.ChildTransactionAliasGuid" +
                                      " WHERE ata.ParentTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @TransactionAliasGuid, @SiteGuid)) " +
									  "AND ((t.ManagerID = @manager) OR (@manager IS NULL)) " +
									  "AND ((t.OwnerID = @owner) OR (@owner IS NULL)) " +
									  "AND ((t.SupplierID = @supplier) OR (@supplier IS NULL)) " +
									  "AND ((t.PONumber = @poNumber) OR (@poNumber IS NULL)) " +
									  "AND ((t.ShipToID = @shipTo) OR (@shipTo IS NULL)) " +
									  "AND ((t.BillToID = @billTo) OR (@billTo IS NULL)) " +
									  "AND ((t.DocumentNumber like @docNumber + '%') OR (@docNumber IS NULL)) " +
								 " AND t.DeleteFlag = 0 AND l.DeleteFlag = 0 AND l.LookupQualityIndex = 1 ";

				// See if a date filter is needed
				if (this.associatedSR.DateFilter != AssociatedTxSR.DateFilters.None)
				{
					// Both begin and end date must be provided
					if ((this.associatedSR.StartDate != null) && (this.associatedSR.EndDate != null))
					{
						if (this.associatedSR.DateFilter == AssociatedTxSR.DateFilters.InventoryDate)
						{
							sql += "AND t.InventoryDate BETWEEN @startDate AND @endDate ";
						}
						else
						{
							sql += "AND t.TransDateTime BETWEEN @startDate AND @endDate ";
						}


						// Add the date parameters to the parameter collection
						cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
						cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);

						cmd.Parameters["@StartDate"].Value = associatedSR.StartDate;
						cmd.Parameters["@EndDate"].Value = associatedSR.EndDate;
					}
				}

				cmd.Parameters.Add("@Product", SqlDbType.NVarChar, 30);
				cmd.Parameters["@Product"].Value = associatedSR.Product;

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = associatedSR.CurrentSiteGuid;

				cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TransactionAliasGuid"].Value = associatedSR.TransactionAliasGuid;

				cmd.Parameters.Add("@Manager", SqlDbType.NVarChar, 100);

				if (!string.IsNullOrEmpty(associatedSR.Manager))
				{
					cmd.Parameters["@Manager"].Value = associatedSR.Manager;
				}
				else
				{
					cmd.Parameters["@Manager"].Value = DBNull.Value;
				}

				cmd.Parameters.Add("@Owner", SqlDbType.NVarChar, 100);

				if (!string.IsNullOrEmpty(associatedSR.Owner))
				{
					cmd.Parameters["@Owner"].Value = associatedSR.Owner;
				}
				else
				{
					cmd.Parameters["@Owner"].Value = DBNull.Value;
				}

				cmd.Parameters.Add("@Supplier", SqlDbType.NVarChar, 100);

				if (!string.IsNullOrEmpty(associatedSR.Supplier))
				{
					cmd.Parameters["@Supplier"].Value = associatedSR.Supplier;
				}
				else
				{
					cmd.Parameters["@Supplier"].Value = DBNull.Value;
				}

				cmd.Parameters.Add("@PONumber", SqlDbType.NVarChar, 30);

				if (!string.IsNullOrEmpty(associatedSR.PONumber))
				{
					cmd.Parameters["@PONumber"].Value = associatedSR.PONumber;
				}
				else
				{
					cmd.Parameters["@PONumber"].Value = DBNull.Value;
				}

				cmd.Parameters.Add("@ShipTo", SqlDbType.NVarChar, 100);

				if (!string.IsNullOrEmpty(associatedSR.ShipTo))
				{
					cmd.Parameters["@ShipTo"].Value = associatedSR.ShipTo;
				}
				else
				{
					cmd.Parameters["@ShipTo"].Value = DBNull.Value;
				}

				cmd.Parameters.Add("@BillTo", SqlDbType.NVarChar, 100);

				if (!string.IsNullOrEmpty(associatedSR.BillTo))
				{
					cmd.Parameters["@BillTo"].Value = associatedSR.BillTo;
				}
				else
				{
					cmd.Parameters["@BillTo"].Value = DBNull.Value;
				}

				cmd.Parameters.Add("@DocNumber", SqlDbType.NVarChar, 30);
				if (!string.IsNullOrEmpty(associatedSR.DocumentNumber))
				{
					cmd.Parameters["@DocNumber"].Value = associatedSR.DocumentNumber;
				}
				else
				{
					cmd.Parameters["@DocNumber"].Value = DBNull.Value;
				}

				cmd.Parameters.Add("@TransactionAliasGuid2", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TransactionAliasGuid2"].Value = associatedSR.TransactionAliasGuid;

				// The ADF project requires filtering on possible associated transactions based on if a currency
				// unit has been used. Filter associated transaction on a currency unit other than Australian dollar.
				if (this.associatedSR.ProjectType == AssociatedTxSR.ProjectTypes.ADF)
				{
					if (this.associatedSR.CurrencyGuid != Guid.Empty)
					{
						sql += " AND l.CurrencyGuid = @CurrencyGuid ";

						cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
						cmd.Parameters["@CurrencyGuid"].Value = associatedSR.CurrencyGuid;

					}
					else
					{
						sql += " AND l.CurrencyGuid IS NULL ";
					}
				}

				sql += " ORDER BY t.InventoryDate ";

				cmd.CommandText = sql;

				// Populate the dataset
				associatedList.AvailableTransactions = this.consolidatedDA.GetDataSet(cmd, this.associatedSR.Security);
			}
		}

		private void GetAssociatedTransactions()
		{
			// Make sure a list of associated trans ID's was provided
			if (associatedSR.AssociatedTransactionIDs == null ||
				associatedSR.AssociatedTransactionIDs.Count == 0)
			{
				return;
			}

			// Prepare the where clause
			ArrayList transIds = new ArrayList();
			ArrayList lineIds = new ArrayList();

			foreach (object obj in associatedSR.AssociatedTransactionIDs)
			{
				AssociatedTxDO associatedIds = (AssociatedTxDO)obj;
				if (associatedIds.TransID != null && associatedIds.TransID.Trim().Length > 0)
					transIds.Add(associatedIds.TransID);

				if (associatedIds.TransactionLineItemGuid != Guid.Empty)
					lineIds.Add(associatedIds.TransactionLineItemGuid);
			}

			string sql =
				"SELECT " +
					"1 AS Associated, t.*, l.TransactionLineItemGuid, l.Product, l.DeliveryLocation, " +
				"l.GrossQuantity, l.Tax1 AS Excise, l.Tax2 AS GST, l.Tax3 AS Markup, l.ProductPrice, l.LookupTransactionStatusIndex AS LineItemStatus, " +
				"l.CurrencyGuid, l.RequestedDateTime AS LineItemRequestedDateTime, l.AlternativeNetVolume " +
			"FROM " +
				"tblTransactions t JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid ";

			using (SqlCommand cmd = new SqlCommand())
			{
				ArrayList transIDParamNames = new ArrayList();
				ArrayList lineIDParamNames = new ArrayList();

				for (int i = 0; i < transIds.Count; i++)
				{
					string paramName = "@TransID" + i.ToString();
					cmd.Parameters.Add(paramName, SqlDbType.NVarChar, 64);
					cmd.Parameters[paramName].Value = transIds[i];

					transIDParamNames.Add(paramName);
				}

				for (int i = 0; i < lineIds.Count; i++)
				{
					string paramName = "@LineID" + i.ToString();
					cmd.Parameters.Add(paramName, SqlDbType.UniqueIdentifier);
					cmd.Parameters[paramName].Value = lineIds[i];

					lineIDParamNames.Add(paramName);
				}

				if (transIDParamNames.Count > 0)
				{
					sql += " WHERE " +
							"t.TransID IN ({0}) ";

					string[] paramStrings = transIDParamNames.ToArray(typeof(string)) as string[];
					sql = string.Format(sql, string.Join(",", paramStrings));
				}

				if (lineIDParamNames.Count > 0)
				{
					sql += "AND l.TransactionLineItemGuid IN ({0}) ";
					string[] paramStrings = lineIDParamNames.ToArray(typeof(string)) as string[];
					sql = string.Format(sql, string.Join(",", paramStrings));
				}

				sql += "ORDER BY " +
					"t.InventoryDate";

				cmd.CommandText = sql;
				associatedList.AssociatedTransactions = this.consolidatedDA.GetDataSet(cmd, this.associatedSR.Security);
			}
		}

		private void GetAssociatedParentTransactions()
		{
			// Make sure a list of associated trans ID's was provided
			if (associatedSR.TransID == null)
			{
				return;
			}

			string sql = "SELECT " +
				"1 AS Associated, t.*, l.TransactionLineItemGuid, l.Product, l.DeliveryLocation, k.LinkedTransactionLineItemGuid, " +
				"l.GrossQuantity, l.Tax1 AS Excise, l.Tax2 AS GST, l.Tax3 AS Markup, l.ProductPrice, l.LookupTransactionStatusIndex AS LineItemStatus, " +
				"l.CurrencyGuid " +
				"FROM " +
				"tblTransactions t JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid JOIN " +
				"tblTransactionLinks k ON k.OriginalTransID = t.TransID WHERE LinkedTransID = @TransID ";

			// for ADF project only, ensures we are pulling up the values from the right line item if defined
			HardwareKeyClass hardwareKey = new HardwareKeyClass();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 64);
				cmd.Parameters["@TransID"].Value = associatedSR.TransID;

				if ((hardwareKey.IsADFKey() == true) && (associatedSR.TransactionLineItemGuid != Guid.Empty))
				{
					sql += "AND LinkedTransactionLineItemGuid = @TransactionLineItemGuid ";
					sql += "AND l.TransactionLineItemGuid = k.TransactionLineItemGuid ";

					cmd.Parameters.Add("@TransactionLineItemGuid", SqlDbType.UniqueIdentifier);
					cmd.Parameters["@TransactionLineItemGuid"].Value = associatedSR.TransactionLineItemGuid;
				}

				sql += "ORDER BY  t.InventoryDate";

				cmd.CommandText = sql;

				associatedList.AssociatedTransactions = this.consolidatedDA.GetDataSet(cmd, this.associatedSR.Security);
			}
		}


		private void GetAssociatedTransactionDetails(Guid transactionLineItemGuid)
		{
			// Make sure a list of associated trans ID's was provided
			if (associatedSR.AssociatedTransactionIDs == null ||
				associatedSR.AssociatedTransactionIDs.Count == 0)
			{
				return;
			}

			// Prepare the IN clause
			string lineItemGuids = "";

			foreach (object obj in associatedSR.AssociatedTransactionIDs)
			{
				AssociatedTxDO associatedIds = (AssociatedTxDO)obj;
				if (associatedIds.TransactionLineItemGuid != Guid.Empty)
					lineItemGuids += associatedIds.TransactionLineItemGuid + ",";
			}

			// Remove trailing comma
			if (lineItemGuids.Length > 0)
				lineItemGuids = lineItemGuids.Substring(0, lineItemGuids.Length - 1);

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "usp_GetAssociatedTransactionDetails";
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add("@TransactionLineItemGuidList", SqlDbType.VarChar);
				cmd.Parameters["@TransactionLineItemGuidList"].Value = lineItemGuids;

				cmd.Parameters.Add("@TransactionAliasGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TransactionAliasGuid"].Value = associatedSR.TransactionAliasGuid;

				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@SiteGuid"].Value = associatedSR.Security.SiteGuid;

				associatedList.AssociatedTransactions = this.consolidatedDA.GetDataSet(cmd, this.associatedSR.Security);
			}
		}
	}
}
