namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessServices.DataAccessLayer;

	public class ExStarsTransactionLoader 
	{
		public ExStarsTransactionListClass TransactionList { get; protected set; }

		#region Constants and Fields
		private readonly ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
		private readonly ExStarsSiteConfigExpanded config;
		#endregion

		public ExStarsTransactionLoader(ExStarsSiteConfigExpanded config, EnumExStarsTrxType exStarsTrxType)
		{
			// If a user has the authority to view ExSTARS reports, it is presumed they have the right to see the
			// company data on that report.
			if (!config.Security.HasRight(RIGHT.VIEW_IRS_EXSTARS_REPORT))
			{
				throw new FMInsufficientRightsException();
			}
			this.config = config;

			this.TransactionList = new ExStarsTransactionListClass();

			const string StoredProcedureExStarsTransSelect = "[dbo].[gsp_ExStarsTransSelect]";
			using (var cmd = new SqlCommand(StoredProcedureExStarsTransSelect))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SiteGuid", config.Site.SiteGuid);
				cmd.Parameters.AddWithValue("@ManagerCompanyGuid", config.ManagerCompanyGuid);
				cmd.Parameters.AddWithValue("@StartDate", config.StartTransactionDateTime);
				cmd.Parameters.AddWithValue("@EndDate", config.EndTransactionDateTime);
				cmd.Parameters.AddWithValue("@Alias", ExStarsConstants.ToString(exStarsTrxType));
				DateTime startDateForCurrentManager = ExStarsConstants.BeginningOfDateTime;
				if (config.ReportType == ReportTypeEnum.IncomingManager)
				{
					// TBD:startDateForCurrentManager = date submitted previously
				}

				cmd.Parameters.AddWithValue("@UpdatedSince", startDateForCurrentManager);

				DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, config.Security);

				if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
				{
					return;
				}
				DataTable table = dataSet.Tables[0];
				table.TableName = "gsp_ExStarsTransSelect";
				table.TableName = StoredProcedureExStarsTransSelect;
				LoadResults(table, this.TransactionList);
			}
		}

		private string SetDefault(string input, string defaultValue)
		{
			if (! string.IsNullOrEmpty(input))
			{
				return input;
			}
			return defaultValue;
		}

		private void LoadResults(DataTable table, ExStarsTransactionListClass transactionList)
		{
			foreach (DataRow row in table.Rows)
			{
				ExStarsTransactionClass transaction = new ExStarsTransactionClass();

				transaction.TransactionGuid = DataObject.getValue(row["TransactionGuid"], Guid.Empty);
				transaction.SubType = DataObject.getValue(row["SubType"], "");
				transaction.AliasName = DataObject.getValue(row["AliasName"], "");
				transaction.TransId = DataObject.getValue(row["TransId"], "");
				transaction.AviationFuelFlag = DataObject.getValue(row["AviationFuelFlag"], false);
				transaction.GroundFuel = DataObject.getValue(row["GroundFuel"], false);
				transaction.TaxCode = DataObject.getValue(row["TaxCode"], "");
				transaction.ProductId = DataObject.getValue(row["ProductId"], "");
				transaction.ReportYear = DataObject.getValue(row["ReportYear"], 0);
				transaction.ReportMonth = DataObject.getValue(row["ReportMonth"], 0);
				transaction.ReportDay = DataObject.getValue(row["ReportDay"], 0);
				transaction.BillOfLadingNumber = DataObject.getValue(row["DocumentNumber"], "");

				transaction.ProductGuid = DataObject.getValue(row["ProductGuid"], Guid.Empty);
				transaction.ManagerCompanyGuid = DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty);
				transaction.CarrierCompanyGuid = DataObject.getValue(row["CarrierCompanyGuid"], Guid.Empty);
				transaction.ShipperCompanyGuid = DataObject.getValue(row["ShipperCompanyGuid"], Guid.Empty);
				transaction.OwnerCompanyGuid = DataObject.getValue(row["OwnerCompanyGuid"], Guid.Empty);
				transaction.SupplierCompanyGuid = DataObject.getValue(row["SupplierCompanyGuid"], Guid.Empty);
				transaction.ShipToCompanyGuid = DataObject.getValue(row["ShipToCompanyGuid"], Guid.Empty);

				transaction.ManagerId = DataObject.getValue(row["ManagerID"], "");
				transaction.CarrierCompanyId = DataObject.getValue(row["CarrierCompanyId"], "");
				transaction.ShipperCompanyId = DataObject.getValue(row["ShipperCompanyId"], "");
				transaction.OwnerId = DataObject.getValue(row["OwnerId"], "");
				transaction.SupplierId = DataObject.getValue(row["SupplierId"], "");
				transaction.ShipToId = DataObject.getValue(row["ShipToID"], "");

				transaction.ManagerFederalId = DataObject.getValue(row["ManagerFederalId"], "");
				transaction.SupplierFederalId = DataObject.getValue(row["SupplierFederalId"], "");
				transaction.ShipToFederalId = DataObject.getValue(row["ShipToFederalId"], "");
				transaction.ShipToState = DataObject.getValue(row["ShipToState"], "");

				transaction.ManagerFederalId = SetDefault(transaction.ManagerFederalId, "??NOTSET??");
				transaction.SupplierFederalId =			  SetDefault(transaction.SupplierFederalId, "??NOTSET??");
				transaction.ShipToFederalId =			  SetDefault(transaction.ShipToFederalId, "??NOTSET??");
				transaction.ShipToState = SetDefault(transaction.ShipToState, "??");

				transaction.Userdata4 = DataObject.getValue(row["Userdata4"], "");
				transaction.Userdata10 = DataObject.getValue(row["Userdata10"], "");

				transaction.GrossVolume = Math.Round(
					DataObject.getValue(row["GrossQuantity"], 0.0)
					, this.config.SiteVolumePrecision
					, MidpointRounding.AwayFromZero);
										
				transaction.NetVolume = Math.Round(
					DataObject.getValue(row["NetQuantity"], 0.0)
					, this.config.SiteVolumePrecision
					, MidpointRounding.AwayFromZero);

				transaction.SrcEquipmentType = DataObject.getValue(row["SrcEquipmentType"], "");
				transaction.SrcEquipmentRegistrationId = DataObject.getValue(row["SrcEquipmentRegistrationId"], "");
				transaction.SrcEquipmentSerialNumber = DataObject.getValue(row["SrcEquipmentSerialNumber"], "");

				transaction.DestEquipmentType = DataObject.getValue(row["DestEquipmentType"], "");
				transaction.DestEquipmentRegistrationId = DataObject.getValue(row["DestEquipmentRegistrationId"], "");
				transaction.DestEquipmentSerialNumber = DataObject.getValue(row["DestEquipmentSerialNumber"], "");

				if (transaction.AliasName == ExStarsTransactionClass.TransferTransaction)
				{
					// transfers only change ownership, and do not move phyiscal product, therefore there is notn equipment involved
				}	
				else if (transaction.AliasIs(EnumExStarsTrxType.Adjustment))
				{
					// do nothing
				}
				else if (transaction.AliasIs(EnumExStarsTrxType.Defuel))
				{
					transaction.EquipmentType = transaction.DestEquipmentType;
					transaction.EquipmentRegistrationId = transaction.DestEquipmentRegistrationId;
					transaction.EquipmentSerialNumber = transaction.DestEquipmentSerialNumber;					
				}
				else if (transaction.AliasIs(EnumExStarsTrxType.Receipt)) 
				{
                    //UserData4 for Legacy Aviation transactions contains the IRS
                    //Shipping Mode Code.  Otherwise we need to derive it from the
                    //Equipment type of the shipping vehicle if we can.
                    if (!string.IsNullOrEmpty(transaction.Userdata4))
                    {
                        transaction.EquipmentType = transaction.Userdata4.ToUpper();
                    }
                    else
                    {
					transaction.EquipmentType = string.IsNullOrEmpty( transaction.SrcEquipmentType)
					?"pipeline"
					: transaction.SrcEquipmentType;
                    }

                    //We need to set this to something other than PIPELINE TEST if the registration ID is
                    //empty.  Eric Simmons will discuss this with Paul.
					transaction.EquipmentRegistrationId = string.IsNullOrEmpty(transaction.SrcEquipmentRegistrationId)
					? "PIPELINE TEST"
					: transaction.SrcEquipmentRegistrationId;
					transaction.EquipmentSerialNumber = transaction.SrcEquipmentSerialNumber;
				}
				else //  issue, bulk issue
				{
					transaction.EquipmentType = transaction.SrcEquipmentType;
					transaction.EquipmentRegistrationId = transaction.SrcEquipmentRegistrationId;
					transaction.EquipmentSerialNumber = transaction.SrcEquipmentSerialNumber;
				}

				transaction.Userdata4 = "";
				transaction.Userdata10 = "";
				try
				{
					transaction.Userdata4 = DataObject.getValue(row["Userdata4"], "");
					transaction.Userdata10 = DataObject.getValue(row["Userdata10"], "");
				}
				catch (Exception)
				{
					// do nothing
				}

				if (transaction.AliasIs(EnumExStarsTrxType.Adjustment))
				{
					transaction.IrsTransportMode = transaction.Userdata10;
				}
				else if (string.IsNullOrEmpty(transaction.EquipmentType)
					&& transaction.AliasIs(EnumExStarsTrxType.BulkIssue))
				{
					transaction.EquipmentType = ExStarsTransactionClass.Truck;
				}
				try
				{
					transaction.IrsTransportMode = transaction.AviationFuelFlag
															  ? this.config.IrsModeCode(transaction.EquipmentType)
															  : ExStarsConstants.TFS06_DeliveryVehicle_GSE;
				}
				catch (Exception)
				{
					this.config.AppendError(ExStarsErrorSource.Transaction, "Transaction '{0}' does not have a valid equipment type", transaction.TransId);
					transaction.IrsTransportMode = "J ";
					// throw new ApplicationException(string.Format("Transaction '{0}' does not have a valid equipment type", transaction.TransId));
				}
				// C_ExSTARS_X12_Schedule_Detail::IRS_DeliveryVehicleType() ~2269

				transaction.TestIfValid();
				transactionList.Add(transaction);
			}
		}




	}
}