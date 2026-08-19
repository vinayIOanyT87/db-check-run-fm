// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetTransactionProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GetTransactionProcessorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;

namespace FMBusinessServices.ServiceClasses
{
	using System.Diagnostics;
	using System.Linq;

	public class GetTransactionProcessorClass : IGetTransactionProcessor
	{
		#region Methods
		public bool IsTransactionDuplicate(Guid transactionGuid, string aliasName, SecurityClass security)
		{
			bool isDuplicate = false;

			using (SqlCommand cmd = new SqlCommand())
			{
				if (string.IsNullOrEmpty(aliasName) == true)
				{
					cmd.CommandText = "SELECT TransID FROM tblTransactions WHERE TransactionGuid = @TransactionGuid";
				}
				else
				{
					cmd.CommandText = "SELECT TransID FROM tblTransactions WHERE AliasName = @AliasName AND TransactionGuid = @TransactionGuid";
				}

				SqlParameter parm = null;

				if (string.IsNullOrEmpty(aliasName) == false)
				{
					parm = new SqlParameter("@AliasName", SqlDbType.NVarChar, 50)
					{
						Value = aliasName
					};
					cmd.Parameters.Add(parm);
				}

				parm = new SqlParameter("@TransactionGuid", SqlDbType.UniqueIdentifier)
				{
					Value = transactionGuid
				};
				cmd.Parameters.Add(parm);

				try
				{
					ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();
					DataSet dataSet = consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						if (dataSet.Tables[0].Rows.Count > 0)
						{
							isDuplicate = true;
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error identifying duplicate transaction. " + ex.Message);
				}
			}

			return isDuplicate;
		}

		public GetTransactionDO Process(GetTransactionSR getTransactionSR)
		{
			DataSet dataSet = null;

			string AliasName = "";
			int TransTypeID = 0;
			DateTimeOffset BeginDate = TimeConverter.MinFMDate;
			DateTimeOffset EndDate = TimeConverter.MaxFMDate;
			DateTimeOffset InventoryDate = TimeConverter.Today();
			string ManagerID = "";
			string OwnerID = "";
			string ShipperID = "";
			string BillToID = "";
			string ShipToID = "";
			string carrierID = "";
			string Location = "";
			string TransactionStatus = "";
			string DocumentNumber = "";
			Guid OperatorPersonnelGuid = Guid.Empty;
			string sql = string.Empty;

			GetTransactionDO getTransactionDO = new GetTransactionDO();

			ConsolidatedDAClass dal = new ConsolidatedDAClass();

			switch (getTransactionSR.Request)
			{
				case GetTransactionRequest.GET_TRANSACTION_TYPE_AND_ALIAS:
					return this.GetTransTypeIdAndAliasName(getTransactionSR);

				case GetTransactionRequest.SITE_TYPEID_ALIAS_STATUS_LOCATION_LINEITEMSTATUS:
					{
						AliasName = getTransactionSR.AliasName;
						TransTypeID = (int)getTransactionSR.TransTypeID;
						Location = getTransactionSR.Location;
						TransactionStatus = getTransactionSR.Status;

						using (SqlCommand cmd = new SqlCommand())
						{
							sql = "SELECT * FROM tblTransactions" +
								" WHERE SiteGuid = @SiteGuid" +
								" AND LookupTransTypeIndex = @LookupTransTypeIndex ";

							if (!string.IsNullOrEmpty(AliasName))
							{
								sql += " AND AliasName = @AliasName ";

								cmd.Parameters.Add("@AliasName", SqlDbType.NVarChar, 32);
								cmd.Parameters["@AliasName"].Value = AliasName;
							}

							sql += " AND LookupTransactionStatusIndex = @LookupTransactionStatusIndex " +
								" AND (DeleteFlag = NULL OR DeleteFlag = 0)" +
								" AND ((SELECT COUNT(*) FROM tblTransactionLineItems WHERE TransactionGuid = tblTransactions.TransactionGuid AND LookupTransactionStatusIndex = @LineItemStatus AND LoadingLocationID = @Location) > 0)";

							cmd.CommandText = sql;

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);
							cmd.Parameters.Add("@LookupTransactionStatusIndex", SqlDbType.Int);
							cmd.Parameters.Add("@LineItemStatus", SqlDbType.Int);
							cmd.Parameters.Add("@Location", SqlDbType.NVarChar, 30);

							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@LookupTransTypeIndex"].Value = TransTypeID;
							cmd.Parameters["@LookupTransactionStatusIndex"].Value = TransactionStatus;
							cmd.Parameters["@LineItemStatus"].Value = getTransactionSR.LineItemStatus;
							cmd.Parameters["@Location"].Value = Location;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}

				case GetTransactionRequest.SITE_TYPEID_ALIAS_TRANSDATE_COMPANIES:
					{
						TransactionStatus = getTransactionSR.Status;
						AliasName = getTransactionSR.AliasName;
						TransTypeID = (int)getTransactionSR.TransTypeID;
						BeginDate = getTransactionSR.BeginningDate;
						EndDate = getTransactionSR.EndingDate;
						ManagerID = getTransactionSR.ManagerID;
						OwnerID = getTransactionSR.OwnerID;
						ShipperID = getTransactionSR.ShipperID;
						BillToID = getTransactionSR.BillToID;
						ShipToID = getTransactionSR.ShipToID;
						carrierID = getTransactionSR.CarrierID;
						Location = getTransactionSR.LocationID;
						string Product = getTransactionSR.Product;
						using (SqlCommand cmd = new SqlCommand())
						{
							cmd.CommandText = "usp_BOLSummaryList " +
								"@AliasName," +
								"@LookupTransTypeIndex," +
								"@BeginDate, " +
								"@EndDate," +
								"@ManagerID," +
								"@OwnerID," +
								"@ShipperID," +
								"@BillToID," +
								"@ShipToID," +
								"@CarrierID," +
								"@DocumentNumber," +
								"@LookupTransactionStatusIndex," +
								"''," +
								"@LoginSiteGuid," +
								"@SiteGuid," +
								"@UserGuid," +
								"@Location," +
								"@Product," +
								"@DestinationSerialNumber1," +
								"@DestinationSerialNumber2," +
								"@DestinationSerialNumber3";

							cmd.Parameters.Add("@AliasName", SqlDbType.NVarChar, 32);
							cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);
							cmd.Parameters.Add("@BeginDate", SqlDbType.DateTimeOffset);
							cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);
							cmd.Parameters.Add("@ManagerID", SqlDbType.NVarChar, 100);
							cmd.Parameters.Add("@OwnerID", SqlDbType.NVarChar, 100);
							cmd.Parameters.Add("@ShipperID", SqlDbType.NVarChar, 100);
							cmd.Parameters.Add("@BillToID", SqlDbType.NVarChar, 100);
							cmd.Parameters.Add("@ShipToID", SqlDbType.NVarChar, 100);
							cmd.Parameters.Add("@CarrierID", SqlDbType.NVarChar, 100);
							cmd.Parameters.Add("@DocumentNumber", SqlDbType.NVarChar, 30);
							cmd.Parameters.Add("@LookupTransactionStatusIndex", SqlDbType.NVarChar, 2);
							cmd.Parameters.Add("@LoginSiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@Location", SqlDbType.NVarChar, 30);
							cmd.Parameters.Add("@Product", SqlDbType.NVarChar, 30);
							cmd.Parameters.Add("@DestinationSerialNumber1", SqlDbType.NVarChar, 10);
							cmd.Parameters.Add("@DestinationSerialNumber2", SqlDbType.NVarChar, 10);
							cmd.Parameters.Add("@DestinationSerialNumber3", SqlDbType.NVarChar, 10);

							cmd.Parameters["@AliasName"].Value = AliasName;
							cmd.Parameters["@LookupTransTypeIndex"].Value = TransTypeID;
							cmd.Parameters["@BeginDate"].Value = BeginDate;
							cmd.Parameters["@EndDate"].Value = EndDate;
							cmd.Parameters["@ManagerID"].Value = ManagerID;
							cmd.Parameters["@OwnerID"].Value = OwnerID;
							cmd.Parameters["@ShipperID"].Value = ShipperID;
							cmd.Parameters["@BillToID"].Value = BillToID;
							cmd.Parameters["@ShipToID"].Value = ShipToID;
							cmd.Parameters["@CarrierID"].Value = carrierID;
							cmd.Parameters["@DocumentNumber"].Value = DocumentNumber;
							cmd.Parameters["@LookupTransactionStatusIndex"].Value = TransactionStatus;
							cmd.Parameters["@LoginSiteGuid"].Value = getTransactionSR.Security.LoginSiteGuid;
							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@DestinationSerialNumber1"].Value = getTransactionSR.DestinationSerialNumber1;
							cmd.Parameters["@DestinationSerialNumber2"].Value = getTransactionSR.DestinationSerialNumber2;
							cmd.Parameters["@DestinationSerialNumber3"].Value = getTransactionSR.DestinationSerialNumber3;

							//if the user guid is empty, pass in NULL for the UserGuid parameter.
							if (getTransactionSR.Security.UserGuid == Guid.Empty)
							{
								cmd.Parameters["@UserGuid"].Value = DBNull.Value;
							}
							else
							{
								cmd.Parameters["@UserGuid"].Value = getTransactionSR.Security.UserGuid;
							}

							cmd.Parameters["@Location"].Value = Location;
							cmd.Parameters["@Product"].Value = Product;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}

				case GetTransactionRequest.SITE_TYPEID_ALIAS_DOCUMENTNUMBER:
					{
						TransactionStatus = getTransactionSR.Status ?? string.Empty;
						TransTypeID = (int)getTransactionSR.TransTypeID;
						DocumentNumber = getTransactionSR.DocumentNumber;
						break;
					}

				case GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID:
				case GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS:
				case GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS_INVENTORYDATE:
					{
						TransTypeID = (int)getTransactionSR.TransTypeID;
						BeginDate = getTransactionSR.BeginningDate;
						EndDate = getTransactionSR.EndingDate;
						TransactionStatus = getTransactionSR.Status;
						OperatorPersonnelGuid = getTransactionSR.OperatorPersonnelGuid;

						using (SqlCommand cmd = new SqlCommand())
						{
							sql = "SELECT * FROM tblTransactions A" +
									" WHERE SiteGuid = @SiteGuid" +
									" AND TransDateTime >= @BeginDate" +
									" AND TransDateTime <= @EndDate" +
									" AND LookupTransTypeIndex = @LookupTransTypeIndex " +
									" AND LookupTransactionStatusIndex = @LookupTransactionStatusIndex " +
									" AND OperatorPersonnelGuid = @OperatorPersonnelGuid " +
									" AND (DeleteFlag = NULL OR DeleteFlag = 0)";

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);
							cmd.Parameters.Add("@LookupTransactionStatusIndex", SqlDbType.Int);
							cmd.Parameters.Add("@BeginDate", SqlDbType.DateTimeOffset);
							cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);
							cmd.Parameters.Add("@OperatorPersonnelGuid", SqlDbType.UniqueIdentifier);

							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@LookupTransTypeIndex"].Value = TransTypeID;
							cmd.Parameters["@LookupTransactionStatusIndex"].Value = TransactionStatus;
							cmd.Parameters["@BeginDate"].Value = BeginDate.DateTime;
							cmd.Parameters["@EndDate"].Value = EndDate.DateTime;
							cmd.Parameters["@OperatorPersonnelGuid"].Value = OperatorPersonnelGuid;

							if (getTransactionSR.Request == GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS_INVENTORYDATE)
							{
								InventoryDate = getTransactionSR.InventoryDate;

								sql += " AND InventoryDate = @InventoryDate";

								cmd.Parameters.Add("@InventoryDate", SqlDbType.Date);
								cmd.Parameters["@InventoryDate"].Value = InventoryDate.Date;
							}

							if (getTransactionSR.Request == GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS
							|| getTransactionSR.Request == GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS_INVENTORYDATE)
							{
								sql += " AND ((SELECT COUNT(*) FROM tblTransactionLineItems WHERE TransactionGuid = A.TransactionGuid AND LookupTransactionStatusIndex = @LineItemStatus";
								sql += ") > 0)";

								cmd.Parameters.Add("@LineItemStatus", SqlDbType.Int);
								cmd.Parameters["@LineItemStatus"].Value = getTransactionSR.LineItemStatus;
							}

							cmd.CommandText = sql;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}

				case GetTransactionRequest.SITE_TYPEID_SHIPMENTNUMBER:
					{
						TransTypeID = (int)getTransactionSR.TransTypeID;
						Location = getTransactionSR.Location;
						TransactionStatus = getTransactionSR.Status;
						string ShipmentNumber = getTransactionSR.ShipmentNumber;

						using (SqlCommand cmd = new SqlCommand())
						{
							cmd.CommandText = "SELECT * FROM tblTransactions" +
								" WHERE SiteGuid = @SiteGuid " +
								" AND LookupTransTypeIndex = @LookupTransTypeIndex " +
								" AND LookupTransactionStatusIndex = @LookupTransactionStatusIndex " +
								" AND ShipmentNumber = @ShipmentNumber " +
								" AND (DeleteFlag = NULL OR DeleteFlag = 0)";

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);
							cmd.Parameters.Add("@LookupTransactionStatusIndex", SqlDbType.Int);
							cmd.Parameters.Add("@ShipmentNumber", SqlDbType.NVarChar, 30);

							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@LookupTransTypeIndex"].Value = TransTypeID;
							cmd.Parameters["@LookupTransactionStatusIndex"].Value = TransactionStatus;
							cmd.Parameters["@ShipmentNumber"].Value = ShipmentNumber;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}

				case GetTransactionRequest.SITE_GET_EOD_TRANSACTIONS:
					{
						TransTypeID = (int)getTransactionSR.TransTypeID;

						using (SqlCommand cmd = new SqlCommand())
						{
							cmd.CommandText = "SELECT * FROM tblTransactions" +
								" WHERE SiteGuid = @SiteGuid" +
								" AND LookupTransTypeIndex = @LookupTransTypeIndex" +
								" AND (DeleteFlag = NULL OR DeleteFlag = 0)";

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);

							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@LookupTransTypeIndex"].Value = TransTypeID;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}

				case GetTransactionRequest.SITE_TYPEID_ALIAS_TRANSDATE_RECEIVINGEQUIPMENT_ISSPT_ISSPTNUM_SERIAL:
					return this.getTransactionsByAliasTransDateTimeReceivingEquipmentUserData131404(getTransactionSR);

				case GetTransactionRequest.CUSTOM_INTERFACE_QUERY:
					return this.GetTransaction_getTransactionsByCustomQuery(getTransactionSR);


				case GetTransactionRequest.SITE_TYPEID_REVERSEDTRANSID:
					{
						TransTypeID = (int)getTransactionSR.TransTypeID;

						using (SqlCommand cmd = new SqlCommand())
						{
							cmd.CommandText = "SELECT * FROM tblTransactions" +
									" WHERE SiteGuid = @SiteGuid" +
									" AND LookupTransTypeIndex = @LookupTransTypeIndex " +
									" AND ReversedTransID = @ReversedTransID " +
									" AND (DeleteFlag = NULL OR DeleteFlag = 0)";

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@LookupTransTypeIndex", SqlDbType.SmallInt);
							cmd.Parameters.Add("@ReversedTransID", SqlDbType.NVarChar, 64);

							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@LookupTransTypeIndex"].Value = TransTypeID;
							cmd.Parameters["@ReversedTransID"].Value = getTransactionSR.ReversedTransID;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}
				case GetTransactionRequest.SITE_MANAGER_PRODUCT_UNPOSTED_ISSUE:
					{
						using (SqlCommand cmd = new SqlCommand())
						{
							cmd.CommandText = "exec usp_FindUnpostedBols @SiteGuid, @ManagerID, @StartDate, @EndDate, @ProductID";

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@ManagerID", SqlDbType.NVarChar, 30);
							cmd.Parameters.Add("@StartDate", SqlDbType.DateTime);
							cmd.Parameters.Add("@EndDate", SqlDbType.DateTime);
							cmd.Parameters.Add("@ProductID", SqlDbType.NVarChar, 30);

							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@ManagerID"].Value = getTransactionSR.ManagerID;
							cmd.Parameters["@StartDate"].Value = getTransactionSR.BeginningDate.DateTime;
							cmd.Parameters["@EndDate"].Value = getTransactionSR.EndingDate.DateTime;
							cmd.Parameters["@ProductID"].Value = getTransactionSR.Product;
							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}

				case GetTransactionRequest.SITE_TYPEID_STATUS_REF_NUM:
					return this.GetTransaction_GetTransactionShipmentOrReceipt(getTransactionSR);
				case GetTransactionRequest.SITE_ORIGINSTATION_FINALSTATION_SHIPTOID_ROUTINGID_ROUTEORIGINATIONDATE:
					{
						using (SqlCommand cmd = new SqlCommand())
						{
							// 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
							cmd.CommandText = "SELECT TOP(1) tblTransactions.TransactionGuid, tblTransactions.TransID, tblTransactions.LookupTransactionStatusIndex " +
									" FROM tblTransactions " +
									" WHERE tblTransactions.SiteGuid = @SiteGuid" +
									" AND tblTransactions.OriginStationIATAID = @OriginStationIATAID" +
									" AND tblTransactions.FinalStationIATAID = @FinalStationIATAID" +
									" AND tblTransactions.ShipToID = @ShipToID" +
									" AND tblTransactions.RoutingID = @RoutingID" +
									" AND tblTransactions.RouteOriginationDate = @RouteOriginationDate" +
									" AND (tblTransactions.DeleteFlag = 0 OR tblTransactions.DeleteFlag IS NULL) " +
									" ORDER BY tblTransactions.CreatedDate DESC ";

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@OriginStationIATAID", SqlDbType.NVarChar, 50);
							cmd.Parameters.Add("@FinalStationIATAID", SqlDbType.NVarChar, 50);
							cmd.Parameters.Add("@ShipToID", SqlDbType.NVarChar, 100);
							cmd.Parameters.Add("@RoutingID", SqlDbType.NVarChar, 30);
							cmd.Parameters.Add("@RouteOriginationDate", SqlDbType.DateTimeOffset);

							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@OriginStationIATAID"].Value = getTransactionSR.OriginStationIATAID;
							cmd.Parameters["@FinalStationIATAID"].Value = getTransactionSR.FinalStationIATAID;
							cmd.Parameters["@ShipToID"].Value = getTransactionSR.ShipToID;
							cmd.Parameters["@RoutingID"].Value = getTransactionSR.RoutingID;
							cmd.Parameters["@RouteOriginationDate"].Value = getTransactionSR.RouteOriginationDate;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}
				case GetTransactionRequest.SITE_ORIGINSTATION_SHIPTOID_DESTINATIONSERIALNUMBER1_ETD:
					{
						using (SqlCommand cmd = new SqlCommand())
						{
							cmd.CommandText = "SELECT TOP(1) tblTransactions.TransactionGuid, " +
								" tblTransactions.TransID, tblTransactions.LookupTransactionStatusIndex " +
								" FROM tblTransactions" +
								" WHERE tblTransactions.SiteGuid = @SiteGuid" +
								" AND tblTransactions.OriginStationIATAID = @OriginStationIATAID" +
								" AND tblTransactions.DestinationSerialNumber1 = @ShipNumber" +
								" AND tblTransactions.ShipToID = @ShipToID" +
								" AND tblTransactions.ETD >= @ETD" +
								" AND (tblTransactions.DeleteFlag = 0 OR tblTransactions.DeleteFlag IS NULL) " +
								" ORDER BY tblTransactions.ETD DESC, tblTransactions.CreatedDate DESC ";

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@OriginStationIATAID", SqlDbType.NVarChar, 50);
							cmd.Parameters.Add("@ShipToID", SqlDbType.NVarChar, 100);
							cmd.Parameters.Add("@ShipNumber", SqlDbType.NVarChar, 10);
							cmd.Parameters.Add("@ETD", SqlDbType.DateTimeOffset);

							cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;
							cmd.Parameters["@OriginStationIATAID"].Value = getTransactionSR.OriginStationIATAID;
							cmd.Parameters["@ShipToID"].Value = getTransactionSR.ShipToID;
							cmd.Parameters["@ShipNumber"].Value = getTransactionSR.DestinationSerialNumber1;
							cmd.Parameters["@ETD"].Value = getTransactionSR.ETD;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}
				case GetTransactionRequest.SITE_DOCUMENTNUMBER:
					{
						using (SqlCommand cmd = new SqlCommand())
						{
							cmd.CommandText = "SELECT tblTransactions.TransID, tblTransactions.DocumentNumber " +
								"FROM tblTransactions " +
								"WHERE (tblTransactions.SiteGuid = @SiteGuid " +
								"AND tblTransactions.DocumentNumber = @DocumentNumber) " +
								"OR tblTransactions.TransID = @TransID " +
								"AND (tblTransactions.DeleteFlag = 0 OR tblTransactions.DeleteFlag IS NULL)";

							cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
							cmd.Parameters.Add("@DocumentNumber", SqlDbType.NVarChar, 30);
							cmd.Parameters.Add("@TransID", SqlDbType.NVarChar, 64);

							cmd.Parameters["@SiteGuid"].Value = new Guid(getTransactionSR.Site);
							cmd.Parameters["@DocumentNumber"].Value = getTransactionSR.DocumentNumber;
							cmd.Parameters["@TransID"].Value = getTransactionSR.TransId;

							dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
						}

						getTransactionDO.TransactionDataSet = dataSet;

						return getTransactionDO;
					}
				case GetTransactionRequest.ALIAS_ROW_VERSION:
					{
						return GetTransactionsByAliasAndRowVersion(getTransactionSR);
					}

				default:
					break;
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandText = "usp_TransactionList " +
						"@AliasName," +
						"@TransTypeID," +
						"@BeginDate," +
						"@EndDate," +
						"@ManagerID," +
						"@OwnerID ," +
						"@ShipperID," +
						"@BillToID," +
						"@ShipToID," +
						"@CarrierID," +
						"@DocumentNumber," +
						"@TransactionStatus," +
						"''," +
						"@LoginSiteGuid," +
						"@SiteGuid," +
						"@UserGuid";

				cmd.Parameters.Add("@AliasName", SqlDbType.NVarChar, 32);
				cmd.Parameters.Add("@TransTypeID", SqlDbType.SmallInt);
				cmd.Parameters.Add("@BeginDate", SqlDbType.Date);
				cmd.Parameters.Add("@EndDate", SqlDbType.Date);
				cmd.Parameters.Add("@ManagerID", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@OwnerID", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@ShipperID", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@BillToID", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@ShipToID", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@CarrierID", SqlDbType.NVarChar, 100);
				cmd.Parameters.Add("@DocumentNumber", SqlDbType.NVarChar, 30);
				cmd.Parameters.Add("@TransactionStatus", SqlDbType.NVarChar, 2);
				cmd.Parameters.Add("@LoginSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@AliasName"].Value = AliasName;
				cmd.Parameters["@TransTypeID"].Value = TransTypeID;
				cmd.Parameters["@BeginDate"].Value = BeginDate.Date;
				cmd.Parameters["@EndDate"].Value = EndDate.Date;
				cmd.Parameters["@ManagerID"].Value = ManagerID;
				cmd.Parameters["@OwnerID"].Value = OwnerID;
				cmd.Parameters["@ShipperID"].Value = ShipperID;
				cmd.Parameters["@BillToID"].Value = BillToID;
				cmd.Parameters["@ShipToID"].Value = ShipToID;
				cmd.Parameters["@CarrierID"].Value = carrierID;
				cmd.Parameters["@DocumentNumber"].Value = DocumentNumber;
				cmd.Parameters["@TransactionStatus"].Value = TransactionStatus;
				cmd.Parameters["@LoginSiteGuid"].Value = getTransactionSR.Security.LoginSiteGuid;
				cmd.Parameters["@SiteGuid"].Value = getTransactionSR.Security.SiteGuid;

				if (getTransactionSR.Security.UserGuid == Guid.Empty)
				{
					cmd.Parameters["@UserGuid"].Value = DBNull.Value;
				}
				else
				{
					cmd.Parameters["@UserGuid"].Value = getTransactionSR.Security.UserGuid;
				}


				dataSet = dal.GetDataSet(cmd, getTransactionSR.Security);
			}

			getTransactionDO.TransactionDataSet = dataSet;

			return getTransactionDO;
		}



		/// <summary>
		/// This method will retrieve transactions in format for csv export with transaction notes, users data  and line items
		/// original usage for Port Buffalo Niagara BOLExport
		/// </summary>
		/// <param name="getTransactionSR"></param>
		/// <returns></returns>
		private static GetTransactionDO GetTransactionsByAliasAndRowVersion(GetTransactionSR getTransactionSR)
		{

			if (getTransactionSR.Security == null)
			{
				throw new ArgumentException("Invalid security.");
			}

			GetTransactionDO getTransactionDO = new GetTransactionDO();
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_TransactionsExportByAliasAndRowVersion";

				cmd.Parameters.Add("@AliasNames", SqlDbType.NVarChar, 200);
				cmd.Parameters.Add("@BOLExportServiceLastRowVersion", SqlDbType.Timestamp);
				cmd.Parameters.Add("@ConvertUnits", SqlDbType.Bit);
				cmd.Parameters.Add("@AllowedTransactionStatuses", SqlDbType.NVarChar, 200);
				cmd.Parameters.Add("@ExportInterfaceName", SqlDbType.NVarChar, 150);

				cmd.Parameters["@AliasNames"].Value = getTransactionSR.AliasName;


				if (getTransactionSR.RowVersion != null)
				{
					cmd.Parameters["@BOLExportServiceLastRowVersion"].Value = getTransactionSR.RowVersion;
				}
				else
				{
					cmd.Parameters["@BOLExportServiceLastRowVersion"].Value = DBNull.Value;
				}

				if (getTransactionSR.ConvertToSiteUnits)
				{

					cmd.Parameters["@ConvertUnits"].Value = 1;
				}
				else
				{
					cmd.Parameters["@ConvertUnits"].Value = 0;
				}

				if (getTransactionSR.TransStatuses != null && getTransactionSR.TransStatuses.Any())
				{
					cmd.Parameters["@AllowedTransactionStatuses"].Value = string.Join(",", getTransactionSR.TransStatuses.Select(x => ((int)x).ToString()));
				}

				cmd.Parameters["@ExportInterfaceName"].Value = getTransactionSR.InterfaceName;

				ConsolidatedDAClass dal = new ConsolidatedDAClass();
				getTransactionDO.TransactionDataSet = dal.GetDataSet(cmd, getTransactionSR.Security);

			}
			return getTransactionDO;
		}

		/// <summary>
		/// This method will retrieve either a shipment or receipt type transaction ID based
		/// on the site, transaction type ID, status, and reference ID.  This method is used
		/// by the save transaction processor to find a corresponding transaction.
		/// </summary>
		/// <param name="getTransSR"></param>
		/// <returns></returns>
		private GetTransactionDO GetTransaction_GetTransactionShipmentOrReceipt(GetTransactionSR getTransSR)
		{
			GetTransactionDO getTransDO = new GetTransactionDO();
			using (SqlCommand sqlCommand = new SqlCommand())
			{
				string sql = "SELECT TransID FROM tblTransactions" +
							 " WHERE LookupTransTypeIndex = @LookupTransTypeIndex" +
							 " AND LookupTransactionStatusIndex <> @Status" +
							 " AND ShipmentNumber = @ShipmentNumber" +
							 " AND (DeleteFlag = NULL OR DeleteFlag = 0)";

				sqlCommand.CommandText = sql;

				SqlParameter parameter = new SqlParameter("@Status", SqlDbType.Int)
				{
					Value = (int)getTransSR.TransStatus
				};
				sqlCommand.Parameters.Add(parameter);

				parameter = new SqlParameter("@LookupTransTypeIndex", SqlDbType.SmallInt)
				{
					Value = (int)getTransSR.TransTypeID
				};
				sqlCommand.Parameters.Add(parameter);

				parameter = new SqlParameter("@ShipmentNumber", SqlDbType.NVarChar, 30)
				{
					Value = getTransSR.ShipmentNumber
				};
				sqlCommand.Parameters.Add(parameter);

				// Most of the time (receipts) will look for a corresponding transaction that
				// has a null reference ID.
				if (string.IsNullOrEmpty(getTransSR.ReferenceID) == true)
				{
					sql += " AND (TransReferenceID = NULL OR TransReferenceID = '')";
				}
				else
				{
					sql += " AND TransReferenceID = @ReferenceID";
					parameter = new SqlParameter("@ReferenceID", SqlDbType.NVarChar, 64)
					{
						Value = getTransSR.ReferenceID
					};
					sqlCommand.Parameters.Add(parameter);
				}

				ConsolidatedDAClass dal = new ConsolidatedDAClass();
				DataSet dataSet = dal.GetDataSet(sqlCommand, getTransSR.Security);
				getTransDO.TransactionDataSet = dataSet;

				return getTransDO;
			}
		}

		private GetTransactionDO GetTransaction_getTransactionsByCustomQuery(GetTransactionSR getTransactionSR)
		{
			DataSet dataSet;
			GetTransactionDO getTransactionDO = new GetTransactionDO();

			try
			{
				using (SqlCommand command = new SqlCommand())
				{
					command.CommandText = getTransactionSR.CustomQuery;
					command.CommandType = CommandType.Text;

					ConsolidatedDAClass dal = new ConsolidatedDAClass();
					dataSet = dal.GetDataSet(command, getTransactionSR.Security);
					getTransactionDO.TransactionDataSet = dataSet;
				}
			}
			catch (AccountingDAException e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("GetTransactionProcessor - " + e.Message, FMEventLogEntryType.Error);
				throw;
			}
			catch (InvalidCastException e)
			{
				FMEventLog eventLog = new FMEventLog();
				eventLog.WriteEntry("GetTransactionProcessor - Bad interface query command - " + e.Message, FMEventLogEntryType.Error);
				throw;
			}

			return getTransactionDO;
		}

		private GetTransactionDO getTransactionsByAliasTransDateTimeReceivingEquipmentUserData131404(GetTransactionSR getTransactionSR)
		{
			using (SqlCommand command = new SqlCommand())
			{
				DataSet dataSet;
				GetTransactionDO getTransactionDO = new GetTransactionDO();

				try
				{
					string strSelect = "SELECT t.* FROM tblTransactions t " +
											"INNER JOIN tblTransactionUserData u on t.TransactionGuid = u.TransactionGuid " +
											"WHERE t.AliasName = @alias AND t.TransDateTime = @transDateTime and t.DeleteFlag = 0 " +
											"AND ISNULL(t.IssuePoint,'') = ISNULL(@issPt,'') AND ISNULL(t.IssuePointNumber,'') = ISNULL(@issPtNum,'') " +
											"AND (ISNULL(u.UserData4,'') = ISNULL(@serial,'') OR ISNULL(u.UserData4,'') = '00' OR ISNULL(u.UserData4,'') = '0000') " +
											"AND t.SiteGuid = @SiteGuid";


					if (!string.IsNullOrEmpty(getTransactionSR.ReceivingEquipment))
					{
						strSelect += " AND t.DestinationRegistrationID1 = @receivingEquipment ";
						command.Parameters.AddWithValue("@receivingEquipment", getTransactionSR.ReceivingEquipment);
					}

					if (!string.IsNullOrEmpty(getTransactionSR.CardNumber))
					{
						strSelect += " AND t.CardNumber = @cardNumber";
						command.Parameters.AddWithValue("@cardNumber", getTransactionSR.CardNumber);
					}

					command.CommandText = strSelect;
					command.CommandType = CommandType.Text;

					command.Parameters.AddWithValue("@alias", getTransactionSR.AliasName);
					command.Parameters.AddWithValue("@transDateTime", getTransactionSR.TransactionDateTime);
					command.Parameters.AddWithValue("@receivingEquipment", getTransactionSR.ReceivingEquipment);
					command.Parameters.AddWithValue("@issPt", getTransactionSR.IssPt);
					command.Parameters.AddWithValue("@issPtNum", getTransactionSR.IssPtNum);
					command.Parameters.AddWithValue("@serial", getTransactionSR.Serial);
					command.Parameters.AddWithValue("@SiteGuid", getTransactionSR.CurrentSiteGuid);

					ConsolidatedDAClass dal = new ConsolidatedDAClass();
					dataSet = dal.GetDataSet(command, getTransactionSR.Security);
					getTransactionDO.TransactionDataSet = dataSet;
				}
				catch (Exception e)
				{
					FMEventLog eventLog = new FMEventLog();
					eventLog.WriteEntry("GetTransactionProcessor - " + e.Message, FMEventLogEntryType.Error);
					throw;
				}

				return getTransactionDO;
			}
		}

		/// <summary>
		/// The get transaction type ID and alias name.
		/// </summary>
		/// <param name="getTransactionSr">
		/// The get transaction service request.
		/// </param>
		/// <returns>
		/// The <see cref="GetTransactionDO"/>.
		/// </returns>
		/// <exception cref="ArgumentException">Null argument exception.
		/// </exception>
		private GetTransactionDO GetTransTypeIdAndAliasName(GetTransactionSR getTransactionSr)
		{
			if (getTransactionSr == null)
			{
				throw new ArgumentException("Must have a GetTransactionSR.");
			}

			if (getTransactionSr.Security == null)
			{
				throw new ArgumentException("Invalid security.");
			}

			if (string.IsNullOrEmpty(getTransactionSr.TransId))
			{
				throw new ArgumentException("Invalid transaction ID.");
			}

			var command = new SqlCommand();
			var getTransactionDO = new GetTransactionDO();

			try
			{
				command.CommandText = "SELECT t.LookupTransTypeIndex AS TransTypeID, t.AliasName, t.TransactionAliasGuid As AliasGuid " +
											 "FROM tblTransactions t " +
											 "WHERE t.TransID = @TransID ";

				command.CommandType = CommandType.Text;

				var parm = new SqlParameter("@TransID", SqlDbType.NVarChar, 64) { Value = getTransactionSr.TransId };
				command.Parameters.Add(parm);

				var consolidatedDa = new ConsolidatedDAClass();
				DataSet dataSet = consolidatedDa.GetDataSet(command, getTransactionSr.Security);
				getTransactionDO.TransactionDataSet = dataSet;
			}
			catch (Exception e)
			{
				var eventLog = new EventLog("Application", ".", "FuelsManager");
				eventLog.WriteEntry("GetTransactionProcessor - " + e.Message, EventLogEntryType.Error);
				throw;
			}

			return getTransactionDO;
		}
		#endregion
	}

}
