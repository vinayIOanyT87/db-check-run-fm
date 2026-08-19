namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessServices.DataAccessLayer;

	public class ExStarsBrokerTransferLoader : FMServiceBase
	{
		public ExStarsBrokerTransferListClass TransactionList { get; protected set; }

		#region Constants and Fields

		private readonly ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();

		private readonly Guid? selectedOwnerGuid = null;

		private readonly ExStarsSiteConfigExpanded config;

		#endregion

		public ExStarsBrokerTransferLoader(ExStarsSiteConfigExpanded config, bool useFromToOwner) : this(config, useFromToOwner, null) { }

		public ExStarsBrokerTransferLoader(ExStarsSiteConfigExpanded config, bool useFromToOwner, Guid? selectedOwnerGuid)
		{			
			// If a user has the authority to view ExSTARS reports, it is presumed they have the right to see the
			// company data on that report.
			if (!config.Security.HasRight(RIGHT.VIEW_IRS_EXSTARS_REPORT))
			{
				throw new FMInsufficientRightsException();
			}

			this.config = config;

			this.TransactionList = new ExStarsBrokerTransferListClass();

			var consolidatedDA = new ConsolidatedDAClass();
			using (var cmd = new SqlCommand("[dbo].[gsp_ExStarsTransBrokerXferSelect]"))
			{
				try
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SiteGuid", config.Site.SiteGuid);
					cmd.Parameters.AddWithValue("@ManagerCompanyGuid", config.ManagerCompanyGuid);
					cmd.Parameters.AddWithValue("@ToOwnerCompanyGuid", this.selectedOwnerGuid);
					cmd.Parameters.AddWithValue("@StartDate", config.StartTransactionDateTime);
					cmd.Parameters.AddWithValue("@EndDate", config.EndTransactionDateTime);
					DateTime startDateForCurrentManager = ExStarsConstants.BeginningOfDateTime;
					if (config.ReportType == ReportTypeEnum.IncomingManager)
					{
						// TBD:startDateForCurrentManager = date submitted previously
					}

					cmd.Parameters.AddWithValue("@UpdatedSince", startDateForCurrentManager);
					cmd.Parameters.AddWithValue("@UseFromToOwners", useFromToOwner);

					DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, config.Security);

					if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
					{
						return;
					}
					DataTable table = dataSet.Tables[0];
					LoadResults(table, useFromToOwner, this.TransactionList);
				}
				catch (Exception e)
				{
					throw new ExStarsSqlException(e, "SQL error: {0}", cmd.CommandText);
				}
			}
		}

		private void LoadResults(DataTable table, bool useFromToOwner, ExStarsBrokerTransferListClass brokerTransactionsList)
		{
			foreach (DataRow row in table.Rows)
			{
				ExStarsBrokerTransferClass brokerTrx = new ExStarsBrokerTransferClass(useFromToOwner);
				brokerTrx.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
				brokerTrx.DebitTransGuid = DataObject.getValue(row["DebitTransGuid"], Guid.Empty);
				brokerTrx.CreditTransGuid = DataObject.getValue(row["CreditTransGuid"], Guid.Empty);
				brokerTrx.ProductId = DataObject.getValue(row["ProductId"], "");
				brokerTrx.ReportYear = DataObject.getValue(row["ReportYear"], 0);
				brokerTrx.ReportMonth = DataObject.getValue(row["ReportMonth"], 0);
				brokerTrx.ReportDay = DataObject.getValue(row["ReportDay"], 0);
				brokerTrx.DocumentNumber = DataObject.getValue(row["DocumentNumber"], "");
				brokerTrx.ProductGuid = DataObject.getValue(row["ProductGuid"], Guid.Empty);
				brokerTrx.ManagerCompanyGuid = DataObject.getValue(row["ManagerCompanyGuid"], Guid.Empty);
				brokerTrx.CarrierCompanyGuid = DataObject.getValue(row["CarrierCompanyGuid"], Guid.Empty);
				brokerTrx.ShipperCompanyGuid = DataObject.getValue(row["ShipperCompanyGuid"], Guid.Empty);
				brokerTrx.FromOwnerCompanyGuid = DataObject.getValue(row["FromOwnerCompanyGuid"], Guid.Empty);
				brokerTrx.ToOwnerCompanyGuid = DataObject.getValue(row["ToOwnerCompanyGuid"], Guid.Empty);
				brokerTrx.SupplierCompanyGuid = DataObject.getValue(row["SupplierCompanyGuid"], Guid.Empty);
				brokerTrx.ManagerID = DataObject.getValue(row["ManagerID"], "");
				brokerTrx.CarrierCompanyId = DataObject.getValue(row["CarrierCompanyId"], "");
				brokerTrx.ShipperCompanyId = DataObject.getValue(row["ShipperCompanyId"], "");
				brokerTrx.FromOwnerId = DataObject.getValue(row["FromOwnerId"], "");
				brokerTrx.ToOwnerId = DataObject.getValue(row["ToOwnerId"], "");
				brokerTrx.FromOwnerCode = DataObject.getValue(row["FromOwnerCode"], "");
				brokerTrx.ToOwnerCode = DataObject.getValue(row["ToOwnerCode"], "");
				brokerTrx.ManagerState = DataObject.getValue(row["ManagerState"], "");
				brokerTrx.SupplierId = DataObject.getValue(row["SupplierId"], "");
				brokerTrx.ManagerFederalId = DataObject.getValue(row["ManagerFederalId"], "");
				brokerTrx.NetVolume = DataObject.getValue(row["NetQuantity"], 0.0);
				brokerTrx.GrossVolume = DataObject.getValue(row["GrossQuantity"], 0.0);
				brokerTrx.EquipmentType = DataObject.getValue(row["EquipmentType"], "");
				brokerTrx.EquipmentRegistrationId = DataObject.getValue(row["EquipmentRegistrationId"], "");
				brokerTrx.EquipmentSerialNumber = DataObject.getValue(row["EquipmentSerialNumber"], "");
				Validate(brokerTrx);

				brokerTransactionsList.Add(brokerTrx);
			}		
		}

		public void Validate(ExStarsBrokerTransferClass brokerTrx)
		{
			if (string.IsNullOrEmpty(brokerTrx.EquipmentType)
			    || !config.IrsModeCodeKeyExists(brokerTrx.EquipmentType))
			{
				throw new ExStarsBusinessException("Equipment Type is missing for ticket (document number) \"{0}\" is not configured in table tblConfigurationSetting", brokerTrx.DocumentNumber);
			}

		}




	}
}