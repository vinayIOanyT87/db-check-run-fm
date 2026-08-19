///***************************************************************************
/// Module Name:  MeterReconciliationProcessor.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// The meter reconciliation processor class provides the ability to retrieve meter reconciliation 
	/// summary and detail information from the database
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MeterReconciliationProcessorClass : IMeterReconciliationProcessor
	{
		/// <summary>
		/// Used for database access
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA;

		/// <summary>
		/// Construct a MeterReconciliationProcessor
		/// </summary>
		public MeterReconciliationProcessorClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		/// <summary>
		/// Retrieve the data necessary to populate the Meter Reconciliation Summary screen's grid
		/// </summary>
		/// <param name="sr">The meter reconciliation service request which contains the search parameters</param>
		/// <returns>A collection containing the results</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public List<MeterReconciliationSummaryData> GetSummary(MeterReconciliationSR sr)
		{
			//make sure there is a meter closeout transaction alias
			this.CheckForMeterCloseout(sr);

			List<MeterReconciliationSummaryData> meterReconciliationSummaryCollection = new List<MeterReconciliationSummaryData>();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_MeterReconciliationSelectSummaryInformation";

				cmd.Parameters.Add("@InventoryDate", SqlDbType.Date);
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@AssetGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@CarrierCompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@InOutOfTolerance", SqlDbType.Bit);
				cmd.Parameters.Add("@ToleranceValue", SqlDbType.Float);
				cmd.Parameters.Add("@ToleranceIsPercent", SqlDbType.Bit);

				cmd.Parameters["@InventoryDate"].Value = sr.InventoryDate.Date;
				cmd.Parameters["@SiteGuid"].Value = sr.CurrentSiteGuid;

				cmd.Parameters["@AssetGuid"].Value = sr.AssetGuid != Guid.Empty ? sr.AssetGuid : (object)DBNull.Value;

				cmd.Parameters["@MeterGuid"].Value = sr.MeterGuid != Guid.Empty ? sr.MeterGuid : (object)DBNull.Value;

				cmd.Parameters["@ManagerCompanyGuid"].Value = sr.ManagerCompanyGuid != Guid.Empty ? sr.ManagerCompanyGuid : (object)DBNull.Value;

				cmd.Parameters["@ProductGuid"].Value = sr.ProductGuid != Guid.Empty ? sr.ProductGuid : (object)DBNull.Value;

				cmd.Parameters["@CarrierCompanyGuid"].Value = sr.CarrierCompanyGuid != Guid.Empty ? sr.CarrierCompanyGuid : (object)DBNull.Value;

				cmd.Parameters["@InOutOfTolerance"].Value = sr.InOutOfTolerance != null ? sr.InOutOfTolerance == true ? 1 : 0 : (object)DBNull.Value;

				cmd.Parameters["@ToleranceValue"].Value = sr.ToleranceValue;

				cmd.Parameters["@ToleranceIsPercent"].Value = (sr.ToleranceIsPercent ? 1 : 0);
	
				DataSet set = this.consolidatedDA.GetDataSet(cmd, sr.Security);

				this.LoadSummaryData(meterReconciliationSummaryCollection, set);

				return meterReconciliationSummaryCollection;
			}
		}

		/// <summary>
		/// Get the data necessary to populate the meter reconciliation detail screen, which consists of 
		/// transactions which used a particular meter
		/// </summary>
		/// <param name="sr">A service request holding search parameters</param>
		/// <returns>A collection containing the results</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public List<MeterReconciliationDetailData> GetDetail(MeterReconciliationSR sr)
		{
			List<MeterReconciliationDetailData> meterReconciliationDetailCollection = new List<MeterReconciliationDetailData>();

			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "usp_MeterReconciliationSelectDetailInformation";

				cmd.Parameters.Add("@InventoryDate", SqlDbType.Date);
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@InventoryDate"].Value = sr.InventoryDate.Date;
				cmd.Parameters["@SiteGuid"].Value = sr.CurrentSiteGuid;

				cmd.Parameters["@MeterGuid"].Value = sr.MeterGuid != Guid.Empty ? sr.MeterGuid : (object)DBNull.Value;

				DataSet set = this.consolidatedDA.GetDataSet(cmd, sr.Security);

				this.LoadDetailData(meterReconciliationDetailCollection, set);

				return meterReconciliationDetailCollection;
			}
		}

		/// <summary>
		/// Read the summary data results from a data set and populate the input collection
		/// </summary>
		/// <param name="meterReconciliationSummaryData">A collection to populate with results</param>
		/// <param name="dataSet">A dataset holding results</param>
		private void LoadSummaryData(List<MeterReconciliationSummaryData> meterReconciliationSummaryData, DataSet dataSet)
		{
			if (dataSet != null)
			{
				if ((dataSet.Tables != null) && (dataSet.Tables.Count > 0))
				{
					DataTable table = dataSet.Tables[0];

					foreach (DataRow row in table.Rows)
					{
						string assetID = DataObject.getValue<string>(row["AssetID"], string.Empty);

						if (assetID != string.Empty)
						{
							MeterReconciliationSummaryData summaryData = new MeterReconciliationSummaryData {
								MeterGuid = DataObject.getValue<Guid>(row["MeterGuid"], Guid.Empty),
								MeterID = DataObject.getValue<string>(row["MeterID"], string.Empty),
								AssetGuid = DataObject.getValue<Guid>(row["AssetGuid"], Guid.Empty),
								AssetID = assetID,
								RotatesBackwardsFlag = DataObject.getValue<bool>(row["RotatesBackwardsFlag"], false),
								MeterStart = DataObject.getValue<double>(row["MeterStart"], 0),
								MeterStop = DataObject.getValue<double>(row["MeterStop"], 0),
								MeterTotal = DataObject.getValue<double>(row["MeterTotal"], 0),
								TransactionMeterTotal = DataObject.getValue<double>(row["TransactionMeterTotal"], 0),
								TransactionVolumeTotal = DataObject.getValue<double>(row["TransactionVolumeTotal"], 0),
								Product = DataObject.getValue<string>(row["Product"], string.Empty),
								Carrier = DataObject.getValue<string>(row["Carrier"], string.Empty),
								CurrentCloseoutTransactionID = DataObject.getValue<string>(row["CurrentCloseoutTransactionID"], string.Empty),
								MoreThanOneCloseoutFlag = DataObject.getValue<bool>(row["MoreThanOneCloseoutFlag"], false),
								NoCurrentCloseoutFlag = DataObject.getValue<bool>(row["NoCurrentCloseoutFlag"], false),
								NoPreviousCloseoutFlag = DataObject.getValue<bool>(row["NoPreviousCloseoutFlag"], false),
								MeterVariance = DataObject.getValue<double>(row["MeterVariance"], 0),
								VolumeVariance = DataObject.getValue<double>(row["VolumeVariance"], 0),
								TransactionGuid = DataObject.getValue<Guid>(row["CurrentCloseoutTransactionGuid"], Guid.Empty)
							};

							meterReconciliationSummaryData.Add(summaryData);
						}
					}
				}
			}
		}

		/// <summary>
		/// Read the detail data results from a data set and populate the input collection
		/// </summary>
		/// <param name="meterReconciliationDetailCollection">A collection to populate with results</param>
		/// <param name="dataSet">A dataset holding results</param>
		private void LoadDetailData(List<MeterReconciliationDetailData> meterReconciliationDetailCollection, DataSet dataSet)
		{
			if (dataSet != null)
			{
				if ((dataSet.Tables != null) && (dataSet.Tables.Count > 0))
				{
					DataTable table = dataSet.Tables[0];

					double ? lastMeterStop = null;

					foreach (DataRow row in table.Rows)
					{
						MeterReconciliationDetailData detailData = new MeterReconciliationDetailData
						{
							TransactionID = DataObject.getValue<string>(row["TransID"], string.Empty),
							MeterStart = DataObject.getValue<double>(row["MeterStart"], 0),
							MeterStop = DataObject.getValue<double>(row["MeterStop"], 0),
							MeterTotal = DataObject.getValue<double>(row["MeterTotal"], 0),
							StationID = DataObject.getValue<string>(row["StationID"], string.Empty),
							Product = DataObject.getValue<string>(row["Product"], string.Empty),
							Carrier = DataObject.getValue<string>(row["Carrier"], string.Empty),
							TransactionAlias = DataObject.getValue<string>(row["TransactionAlias"], string.Empty),
							FlightNumber = DataObject.getValue<string>(row["FlightNumber"], string.Empty),
							TicketNumber = DataObject.getValue<string>(row["TicketNumber"], string.Empty),
							RotatesBackwardsFlag = DataObject.getValue<bool>(row["RotatesBackwardsFlag"], false),
							NumberOfDigits = DataObject.getValue<byte>(row["NumberOfDigits"], 0),
							TransactionGuid = DataObject.getValue<Guid>(row["TransactionGuid"], Guid.Empty),
							GrossVolume = DataObject.getValue<double>(row["GrossVolume"], 0)
						};
						detailData.MeterSkip = this.CalculateMeterSkip(lastMeterStop, detailData.MeterStart, detailData.RotatesBackwardsFlag, detailData.NumberOfDigits);

						meterReconciliationDetailCollection.Add(detailData);

						lastMeterStop = detailData.MeterStop;
					}
				}
			}
		}

		/// <summary>
		/// Calculate the meter skip based on the last meter stop and the current meter start
		/// </summary>
		/// <param name="lastMeterStop">the meter stop of the previous transaction which used the meter</param>
		/// <param name="meterStart">the meter start of the current transaction that we're calculating the meter skip for</param>
		/// <param name="rotatesBackwardsFlag">Whether or not the meter rotates backwards</param>
		/// <param name="numberOfDigits">the number of digits the meter has. We need this to detect rollover</param>
		/// <returns>The meter skip, which is the difference between the current meter start and the last meter stop</returns>
		private double CalculateMeterSkip(double ? lastMeterStop, double meterStart, bool rotatesBackwardsFlag, int numberOfDigits)
		{
			//if this is the first transaction of the period which used the meter, the meter skip is zero.
			if (lastMeterStop == null)
			{
				return 0;
			}

			double beginValue;
			double endValue;

			if (rotatesBackwardsFlag)
			{
				beginValue = meterStart;
				endValue = lastMeterStop.GetValueOrDefault();
			}
			else
			{
				beginValue = lastMeterStop.GetValueOrDefault();
				endValue = meterStart;
			}

			// Did the meter rollover? 
			if (beginValue > endValue)
			{
				// If so, calculate the maximum meter value and use it to calculate the skip
				string maxValue = new string('9', numberOfDigits);

				double maxMeterValue = double.Parse(maxValue);

				return Math.Abs(endValue + maxMeterValue - beginValue + 1);
			}
			else
			{
				return Math.Abs(endValue - beginValue);
			}		
		}

		/// <summary>
		/// Check to make sure that a type 12 transaction is configured as the meter closeout transaction alias. 
		/// Throw a user-friendly error message if one is not found, or if more than one is found
		/// </summary>
		/// <param name="sr">The service request object which contains security information we need</param>
		private void CheckForMeterCloseout(MeterReconciliationSR sr)
		{
			TransactionAliasClass transactionAlias = new TransactionAliasClass();

			int transactionAliasCount = 0;
			using (SqlCommand cmd = new SqlCommand())
			{
				transactionAlias.GetMeterCloseoutAliasCountSQL(cmd, sr.Security);
				DataSet set = this.consolidatedDA.GetDataSet(cmd, sr.Security);

				if(set != null && set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0 && set.Tables[0].Rows[0].ItemArray.Length > 0)
				{
					DataRow row = set.Tables[0].Rows[0];
					transactionAliasCount = (int)row[0];
				}
			}

			if (transactionAliasCount == 0)
			{
				throw new Exception("A meter closeout Transaction Alias was not detected. You must configure a Transaction Alias as a meter closeout to use Meter Reconciliation");
			}
			else if (transactionAliasCount > 1)
			{
				throw new Exception("More than one meter closeout Transaction Alias was detected. There must be only one meter closeout Transaction Alias configured to use Meter Reconciliation");
			}
		}
	}
}