namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AssetTrackingDetails : IAssetTrackingDetails
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AssetTrackingDetails()
		{		
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will add an asset tracking detail record to the database.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDetail">The asset tracking detail record to save.</param>
		/// <returns>Returns the new asset tracking detail GUID.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AssetTrackingDetailClass assetTrackingDetail)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDetail == null)
			{
				throw new ArgumentNullException("assetTrackingDetail");
			}

			if (!security.HasRight(RIGHT.VIEW_MAPS))
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				assetTrackingDetail.InsertSql(sqlCommand, security);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}

			if (assetTrackingDetail.PayloadValues != null && assetTrackingDetail.PayloadValues.Count > 0)
			{
				using (var sqlCommand = new SqlCommand())
				{
					foreach (AssetTrackingPayloadClass payload in assetTrackingDetail.PayloadValues)
					{
						payload.AssetTrackingDetailGuid = assetTrackingDetail.AssetTrackingDetailGuid;
						payload.InsertSql(sqlCommand, security);
						this.consolidatedDA.ExecuteQuery(security, sqlCommand);
					}
				}
			}

			if (assetTrackingDetail.TrackingTanks != null && assetTrackingDetail.TrackingTanks.Count > 0)
			{
				using (var sqlCommand = new SqlCommand())
				{
					foreach (AssetTrackingTankClass tank in assetTrackingDetail.TrackingTanks)
					{
						tank.AssetTrackingDetailGuid = assetTrackingDetail.AssetTrackingDetailGuid;
						tank.InsertSql(sqlCommand, security);
						this.consolidatedDA.ExecuteQuery(security, sqlCommand);
					}
				}
			}

			return assetTrackingDetail.AssetTrackingDetailGuid;
		}

		/// <summary>
		/// This method will update an asset tracking detail record to the database.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDetail">The asset tracking detail record to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Update(SecurityClass security, AssetTrackingDetailClass assetTrackingDetail)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDetail == null)
			{
				throw new ArgumentNullException("assetTrackingDetail");
			}

			if (!security.HasRight(RIGHT.VIEW_MAPS))
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				assetTrackingDetail.UpdateSql(sqlCommand, security);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}

			if (assetTrackingDetail.PayloadValues != null && assetTrackingDetail.PayloadValues.Count > 0)
			{
				using (var sqlCommand = new SqlCommand())
				{
					var deletePayload = new AssetTrackingPayloadClass();
					deletePayload.DeleteByAssetTrackingDetail(sqlCommand, assetTrackingDetail.AssetTrackingDetailGuid);
					this.consolidatedDA.ExecuteQuery(security, sqlCommand);

					foreach (AssetTrackingPayloadClass payload in assetTrackingDetail.PayloadValues)
					{
						payload.UpdateSql(sqlCommand, security);
						this.consolidatedDA.ExecuteQuery(security, sqlCommand);
					}
				}
			}

			if (assetTrackingDetail.TrackingTanks != null && assetTrackingDetail.TrackingTanks.Count > 0)
			{
				using (var sqlCommand = new SqlCommand())
				{
					var deleteTank = new AssetTrackingTankClass();
					deleteTank.DeleteByAssetTrackingDetail(sqlCommand, assetTrackingDetail.AssetTrackingDetailGuid);
					this.consolidatedDA.ExecuteQuery(security, sqlCommand);

					foreach (AssetTrackingTankClass tank in assetTrackingDetail.TrackingTanks)
					{
						tank.UpdateSql(sqlCommand, security);
						this.consolidatedDA.ExecuteQuery(security, sqlCommand);
					}
				}
			}
		}

		/// <summary>
		/// This method will update the asset tracking detail records to the investigate state
		/// based on a list a asset tracking GUIDs.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingGuidList">The list of asset tracking GUIDs to update.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateRecordsToInvestigateState(SecurityClass security, List<string> assetTrackingGuidList )
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingGuidList == null)
			{
				throw new ArgumentNullException("assetTrackingGuidList");
			}

			if (!security.HasRight(RIGHT.MAP_INITIATE_INVESTIGATION))
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDetail = new AssetTrackingDetailClass();
				assetTrackingDetail.UpdateRecordsToInvestigateStateSql(sqlCommand, security, assetTrackingGuidList);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will update the asset tracking detail records to the investigate complete state
		/// based on a list a asset tracking GUIDs.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="deviceId">The device ID to filter on.</param>
		/// <param name="completeState">Complete state is either completed failed or completed passed.</param>
		/// <param name="remarks">The investigation remarks.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateRecordsToInvestigateCompleteState(SecurityClass security, 
															string deviceId,
															AssetTrackingDetailClass.MessageStates completeState,
															string remarks)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MAP_COMPLETE_INVESTIGATION))
			{
				throw new FMInsufficientRightsException();
			}

			// This start date is the date 60 days in the past.
			var currentDateTime = DateTime.Now;
			var startDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);
			startDate = startDate.AddDays(-60);

			using (var sqlCommand = new SqlCommand())
			{
				var completeInvestigateDateTime = DateTime.Now;
				var assetTrackingDetail = new AssetTrackingDetailClass();

				assetTrackingDetail.UpdateRecordsToInvestigateCompleteStateSql(sqlCommand, security, startDate, deviceId, completeState, completeInvestigateDateTime);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);

				sqlCommand.Parameters.Clear();
				assetTrackingDetail.UpdateRemarksOnInvestigateCompleteSql(sqlCommand, security, startDate, deviceId, remarks, completeInvestigateDateTime);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}

		/// <summary>
		/// This method will update the remarks for a given asset tracking detail.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDetailGuid">The asset tracking detail used to update the remarks.</param>
		/// <param name="remarks">The remarks to update.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void UpdateRemarks(SecurityClass security, Guid assetTrackingDetailGuid, string remarks)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDetailGuid == Guid.Empty)
			{
				throw new ArgumentNullException("assetTrackingDetailGuid");
			}

			if (!security.HasRight(RIGHT.MAP_COMPLETE_INVESTIGATION))
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDetail = new AssetTrackingDetailClass();
				assetTrackingDetail.UpdateRemarksSql(sqlCommand, security, assetTrackingDetailGuid, remarks);
				this.consolidatedDA.ExecuteQuery(security, sqlCommand);
			}
		}


		/// <summary>
		/// This method will get the asset tracking detail record.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="assetTrackingDetailGuid">The record to retrieve.</param>
		/// <returns>Returns the asset tracking detail record or null if not found.</returns>
		public AssetTrackingDetailClass Get(SecurityClass security, Guid assetTrackingDetailGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (assetTrackingDetailGuid == Guid.Empty)
			{
				throw new ArgumentNullException("assetTrackingDetailGuid");
			}

			if (!security.HasRight(RIGHT.VIEW_MAPS))
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDetail = new AssetTrackingDetailClass();
				assetTrackingDetail.GetSql(sqlCommand, assetTrackingDetailGuid);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				assetTrackingDetail.Load(row);
				assetTrackingDetail.PayloadValues = this.GetAssociatedPayload(assetTrackingDetailGuid, security);
				assetTrackingDetail.TrackingTanks = this.GetAssociatedTanks(assetTrackingDetailGuid, security);

				return assetTrackingDetail;
			}
		}

		/// <summary>
		/// This method will get a list of asset tracking detail records.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="endDate">The ending date range.</param>
		/// <param name="deviceId">The asset tracking device filter</param>
		/// <param name="startDate">The starting date range.</param>
		/// <param name="topOne">Whether to only get the top one record.</param>
		/// <returns>Return a list of asset tracking detail records.</returns>
		public List<AssetTrackingDetailClass> GetByFilters(SecurityClass security, DateTime startDate, DateTime endDate, string deviceId, bool topOne)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_MAPS))
			{
				throw new FMInsufficientRightsException();
			}

			var assetTrackingDetailList = new List<AssetTrackingDetailClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDetail = new AssetTrackingDetailClass();
				assetTrackingDetail.GetByFilterSql(sqlCommand, deviceId, topOne);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				string previousDeviceId = string.Empty;

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingDetail = new AssetTrackingDetailClass();
					assetTrackingDetail.Load(row);

					assetTrackingDetail.MarkerType = AssetTrackingDetailClass.MarkerTypes.Crumb;

					if (assetTrackingDetail.AssetTrackingDeviceId.Equals(previousDeviceId) == false)
					{
						assetTrackingDetail.MarkerType = AssetTrackingDetailClass.MarkerTypes.Marker;
					}

					previousDeviceId = assetTrackingDetail.AssetTrackingDeviceId;

					if (assetTrackingDetail.MarkerType == AssetTrackingDetailClass.MarkerTypes.Marker)
					{
						assetTrackingDetail.PayloadValues = this.GetAssociatedPayload(assetTrackingDetail.AssetTrackingDetailGuid, security);
						assetTrackingDetail.TrackingTanks = this.GetAssociatedTanks(assetTrackingDetail.AssetTrackingDetailGuid, security);
						assetTrackingDetailList.Add(assetTrackingDetail);
					}
					else if (assetTrackingDetail.AssetSessionDateTime >= startDate
							&& assetTrackingDetail.AssetSessionDateTime <= endDate)
					{
						assetTrackingDetail.PayloadValues = this.GetAssociatedPayload(assetTrackingDetail.AssetTrackingDetailGuid, security);
						assetTrackingDetail.TrackingTanks = this.GetAssociatedTanks(assetTrackingDetail.AssetTrackingDetailGuid, security);
						assetTrackingDetailList.Add(assetTrackingDetail);
					}
				}

				return assetTrackingDetailList;
			}
		}

		/// <summary>
		/// This method will get a list of asset tracking details based on a device.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="device">The deivce that is associated to the detail record.</param>
		/// <returns>Returns the list of asset tracking details.</returns>
		public List<AssetTrackingDetailClass> GetByDeviceAndMostCurrent(SecurityClass security, AssetTrackingDeviceClass device)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (device == null)
			{
				throw new ArgumentNullException("device");
			}

			if (security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			var assetTrackingDetailList = new List<AssetTrackingDetailClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDetail = new AssetTrackingDetailClass();
				assetTrackingDetail.GetByDeviceAndMostCurrentSql(sqlCommand, device.DeviceId);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingDetail = new AssetTrackingDetailClass();
					assetTrackingDetail.Load(row);

					assetTrackingDetailList.Add(assetTrackingDetail);
				}
			}

			return assetTrackingDetailList;
		}

		/// <summary>
		/// This method will retrieve the last 60 days by a given device.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="deviceId">The device ID used to retrieve the detail data.</param>
		/// <param name="startDate">The starting date which will be in the past.</param>
		/// <returns>Returns a collection of asset tracking details.</returns>
		public bool FoundInvestigateStates(SecurityClass security, string deviceId, DateTime startDate)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(deviceId))
			{
				throw new ArgumentNullException("deviceId");
			}

			if (security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDetail = new AssetTrackingDetailClass();
				assetTrackingDetail.FoundInvestigateStatesSql(sqlCommand, deviceId, startDate);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return false;
				}

				DataRow row = dataSet.Tables[0].Rows[0];
				int numberFound = row.IsNull("NumberFound") ? 0 : (int)row["NumberFound"];

				if (numberFound > 0)
				{
					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// This method will retrieve the last 60 days by a given device.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="deviceId">The device ID used to retrieve the detail data.</param>
		/// <param name="startDate">The starting date which will be in the past.</param>
		/// <param name="filterStartingDate">This the start date in the past.</param>
		/// <param name="filterEndingDate">This is the ending date which is the most current date.</param>
		/// <param name="topOne">Only get the most recent record.</param>
		/// <returns>Returns a collection of asset tracking details.</returns>
		public List<AssetTrackingDetailClass> GetLast60DaysByDevice(SecurityClass security, 
																	string deviceId, 
																	DateTime startDate, 
																	DateTime filterStartingDate, 
																	DateTime filterEndingDate, 
																	bool topOne)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(deviceId))
			{
				throw new ArgumentNullException("deviceId");
			}

			if (security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			var assetTrackingDetailList = new List<AssetTrackingDetailClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingDetail = new AssetTrackingDetailClass();
				assetTrackingDetail.GetLast60DaysByDeviceListSql(sqlCommand, deviceId, startDate);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				bool foundOneContaminate = false;

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingDetail = new AssetTrackingDetailClass();
					assetTrackingDetail.Load(row);

					var foundContaminate = this.SetItemState(assetTrackingDetail, foundOneContaminate);
					if (foundContaminate && foundOneContaminate == false)
					{
						foundOneContaminate = true;
					}		

					// Filter based on a filter date range.
					if (topOne || (assetTrackingDetail.AssetSessionDateTime >= filterStartingDate && assetTrackingDetail.AssetSessionDateTime <= filterEndingDate))
					{
						// Since we started from oldest to most current date, we want to make the list in descending order.
						assetTrackingDetailList.Insert(0, assetTrackingDetail);
					}
				}

				// Top one means only to get the most current record.
				if (topOne && assetTrackingDetailList.Count > 1)
				{
					var itemsToRemove = assetTrackingDetailList.Count - 1;
					assetTrackingDetailList.RemoveRange(1, itemsToRemove);
				}

				this.AssociatedWrdcuTanks(assetTrackingDetailList, security);
				return assetTrackingDetailList;
			}
		}

		/// <summary>
		/// This method will get a list of asset tracking details based on a list of devices.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="devices">The list of deivces to use in the query.</param>
		/// <param name="filterStartingDate">The filter starting date</param>
		/// <param name="filterEndingDate">The filter ending date</param>
		/// <param name="topOne">Used to get the top one record if true.</param>
		/// <returns>Returns the list of asset tracking details.</returns>
		public List<AssetTrackingDetailClass> GetByDeviceList(SecurityClass security,
																List<AssetTrackingDeviceClass> devices,
																DateTime filterStartingDate,
																DateTime filterEndingDate,
																bool topOne)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (devices == null)
			{
				throw new ArgumentNullException("devices");
			}

			if (security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			if (devices.Count == 0)
			{
				return null;
			}

			var assetTrackingDetailList = new List<AssetTrackingDetailClass>();
			var detailGroupList = new List<AssetTrackingDetailClass>();

			using (var sqlCommand = new SqlCommand())
			{
				// This start date is the date 60 days in the past. That is were we want to start retrieving
				// the data.
				var currentDateTime = DateTime.Now;
				var startDate = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, 0, 0, 0);
				startDate = startDate.AddDays(-60);

				var assetTrackingDetail = new AssetTrackingDetailClass();
				assetTrackingDetail.GetByDeviceListSql(sqlCommand, devices, startDate);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				string previousDeviceId = string.Empty;
				AssetTrackingDetailClass.MessageStates forceState = AssetTrackingDetailClass.MessageStates.None;

				foreach (DataRow row in dataSet.Tables[0].Rows)
				{
					assetTrackingDetail = new AssetTrackingDetailClass();
					assetTrackingDetail.Load(row);

					if (assetTrackingDetail.AssetTrackingDeviceId.Equals(previousDeviceId) == false)
					{
						forceState = AssetTrackingDetailClass.MessageStates.None;

						this.AppendToAssetTrackingDetail(security, detailGroupList, assetTrackingDetailList, topOne, filterStartingDate, filterEndingDate);
					}

					forceState = this.SetTruckItemState(assetTrackingDetail, forceState);
					previousDeviceId = assetTrackingDetail.AssetTrackingDeviceId;

					// Since we started from oldest to most current date, we want to make the list in descending order.
					detailGroupList.Insert(0, assetTrackingDetail);
				}

				// This means there was only one device or it is the last
				// device in the data collection.
				if (detailGroupList.Count > 0)
				{
					this.AppendToAssetTrackingDetail(security, detailGroupList, assetTrackingDetailList, topOne, filterStartingDate, filterEndingDate);
				}

				this.AssociatedWrdcuTanks(assetTrackingDetailList, security);
				return assetTrackingDetailList;
			}
		}

		/// <summary>
		/// This method will append an asset tracking detail group to the asset tracking detail list that
		/// contains all the details.  It also gets the associated tank information for a detail.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="detailGroupList">The asset tracking detail group list.</param>
		/// <param name="assetTrackingDetailList">The asset tracking detail total list.</param>
		/// <param name="topOne">Flag to only get the top one out of the group.</param>
		/// <param name="filterStartingDate">The starting date to filter on.</param>
		/// <param name="filterEndingDate">The ending date to filter on.</param>
		private void AppendToAssetTrackingDetail(SecurityClass security, 
												List<AssetTrackingDetailClass> detailGroupList, 
												List<AssetTrackingDetailClass> assetTrackingDetailList,
												bool topOne,
												DateTime filterStartingDate,
												DateTime filterEndingDate)
		{
			if (detailGroupList.Count == 0)
			{
				return;
			}

			bool firstItem = true;

			// Top one means only to get the most current record.
			if (topOne && detailGroupList.Count > 1)
			{
				var itemsToRemove = detailGroupList.Count - 1;
				detailGroupList.RemoveRange(1, itemsToRemove);
			}

			foreach (var assetTrackingDetail in detailGroupList)
			{
				assetTrackingDetail.MarkerType = AssetTrackingDetailClass.MarkerTypes.Crumb;

				if (firstItem)
				{
					assetTrackingDetail.MarkerType = AssetTrackingDetailClass.MarkerTypes.Marker;
					firstItem = false;
				}

				// Filter based on a filter date range.
				if (assetTrackingDetail.MarkerType == AssetTrackingDetailClass.MarkerTypes.Marker
					|| topOne
					|| (assetTrackingDetail.AssetSessionDateTime >= filterStartingDate && assetTrackingDetail.AssetSessionDateTime <= filterEndingDate))
				{
					assetTrackingDetailList.Add(assetTrackingDetail);
				}
			}

			// Clear asset tracking detail group list for the next group.
			detailGroupList.Clear();
		}

		/// <summary>
		/// This method will retrieve a WRDCU tank data based on the asset tracking detail GUID.  It will return all the tanks.
		/// </summary>
		/// <param name="assetTrackingDetailGuid">The tracking detail GUID used to retrieve the WRDCU tanks.</param>
		/// <param name="security">The security object.</param>
		/// <returns>Returns list of WRDCU tank data objects.</returns>
		public List<AssetTrackingTankClass> GetWrdcuTanks(Guid assetTrackingDetailGuid, SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var wrdcuTankList = new List<AssetTrackingTankClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingTank = new AssetTrackingTankClass();
				assetTrackingTank.GetAllTanksByAssetTrackingDetailSql(sqlCommand, assetTrackingDetailGuid);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				DataRowCollection rows = dataSet.Tables[0].Rows;

				foreach (DataRow row in rows)
				{
					assetTrackingTank = new AssetTrackingTankClass();
					assetTrackingTank.Load(row);
					wrdcuTankList.Add(assetTrackingTank);
				}

				if (wrdcuTankList.Count > 0)
				{
					return wrdcuTankList;
				}
			}

			return null;
		}

		/// <summary>
		/// This method will retrieve all the asset tracking tanks associated to the
		/// asset tracking detail record.
		/// </summary>
		/// <param name="assetTrackingDeviceId">The asset tracking detail tanks to retrieve.</param>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a collection of assoicated tanks.</returns>
		public List<AssetTrackingTankClass> GetPreviousDetailTanks(string assetTrackingDeviceId, SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (string.IsNullOrEmpty(assetTrackingDeviceId))
			{
				throw new ArgumentNullException("assetTrackingDeviceId");
			}

			if (security.HasRight(RIGHT.VIEW_MAPS) == false)
			{
				throw new FMInsufficientRightsException();
			}

			var tanks = new List<AssetTrackingTankClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingTank = new AssetTrackingTankClass();
				assetTrackingTank.GetPreviousDetailTanksSql(sqlCommand, assetTrackingDeviceId);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				DataRowCollection rows = dataSet.Tables[0].Rows;

				foreach (DataRow row in rows)
				{
					assetTrackingTank = new AssetTrackingTankClass();
					assetTrackingTank.Load(row);

					tanks.Add(assetTrackingTank);
				}

				return tanks;
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will retrieve all the asset tracking payload associated to the
		/// asset tracking detail record.
		/// </summary>
		/// <param name="assetTrackingDetailGuid">The asset tracking detail to retrieve.</param>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a collection of assoicated payloads.</returns>
		private List<AssetTrackingPayloadClass> GetAssociatedPayload(Guid assetTrackingDetailGuid, SecurityClass security)
		{
			var payloads = new List<AssetTrackingPayloadClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingPayload = new AssetTrackingPayloadClass();
				assetTrackingPayload.GetAllBasedOnAssetTrackingDetailSql(sqlCommand, assetTrackingDetailGuid);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				DataRowCollection rows = dataSet.Tables[0].Rows;

				foreach (DataRow row in rows)
				{
					assetTrackingPayload = new AssetTrackingPayloadClass();
					assetTrackingPayload.Load(row);

					payloads.Add(assetTrackingPayload);
				}

				return payloads;
			}
		}

		/// <summary>
		/// This method will retrieve all the asset tracking tanks associated to the
		/// asset tracking detail record.
		/// </summary>
		/// <param name="assetTrackingDetailGuid">The asset tracking detail to retrieve.</param>
		/// <param name="security">The security object.</param>
		/// <returns>Returns a collection of assoicated tanks.</returns>
		private List<AssetTrackingTankClass> GetAssociatedTanks(Guid assetTrackingDetailGuid, SecurityClass security)
		{
			var tanks = new List<AssetTrackingTankClass>();

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingTank = new AssetTrackingTankClass();
				assetTrackingTank.GetAllTanksByAssetTrackingDetailSql(sqlCommand, assetTrackingDetailGuid);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return null;
				}

				DataRowCollection rows = dataSet.Tables[0].Rows;

				foreach (DataRow row in rows)
				{
					assetTrackingTank = new AssetTrackingTankClass();
					assetTrackingTank.Load(row);

					tanks.Add(assetTrackingTank);
				}

				return tanks;
			}
		}

		/// <summary>
		/// This method will set the Contamination state on the message. If the previous state is not
		/// investigate, failed, or passed and the contaminate flag is set or a previous message
		/// had a contamination, then the state is set.
		/// </summary>
		/// <param name="detail">The assert tracking detail to check.</param>
		/// <param name="foundOneContaminate">True if any previous messages were contaminated.</param>
		/// <returns>Returns the force contaminate state flag.</returns>
		private bool SetItemState(AssetTrackingDetailClass detail, bool foundOneContaminate)
		{
			if (detail.Contaminated
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.Investigate
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.InvestigateCompletedFailed
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.InvestigateCompletedPassed)
			{
				detail.MessageState = AssetTrackingDetailClass.MessageStates.Contaminated;
				return true;
			}

			if (detail.Contaminated == false 
				&& foundOneContaminate
				&& (detail.MessageState == AssetTrackingDetailClass.MessageStates.None || detail.MessageState == AssetTrackingDetailClass.MessageStates.Contaminated))
			{
				detail.MessageState = AssetTrackingDetailClass.MessageStates.Contaminated;
				return true;
			}

			if (detail.Contaminated == false
				&& foundOneContaminate
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.Investigate
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.InvestigateCompletedFailed
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.InvestigateCompletedPassed)
			{
				detail.MessageState = AssetTrackingDetailClass.MessageStates.Contaminated;
				return true;
			}

			return false;
		}

		/// <summary>
		/// This method will set the Investigate state or Contaminate state.  The Investigate state is a 
		/// higher priority than the contaminate state.
		/// </summary>
		/// <param name="detail">The assert tracking detail to check.</param>
		/// <param name="forceState">The force states are Investigate and Contaminate</param>
		/// <returns>Returns the force state.</returns>
		private AssetTrackingDetailClass.MessageStates SetTruckItemState(AssetTrackingDetailClass detail, AssetTrackingDetailClass.MessageStates forceState)
		{
			if ((forceState == AssetTrackingDetailClass.MessageStates.Investigate || forceState == AssetTrackingDetailClass.MessageStates.Contaminated)
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.Investigate)
			{
				detail.MessageState = forceState;
				return forceState;
			}

			if (detail.MessageState == AssetTrackingDetailClass.MessageStates.Investigate)
			{
				return AssetTrackingDetailClass.MessageStates.Investigate;
			}

			if (detail.Contaminated
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.Investigate
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.InvestigateCompletedFailed
				&& detail.MessageState != AssetTrackingDetailClass.MessageStates.InvestigateCompletedPassed)
			{
				detail.MessageState = AssetTrackingDetailClass.MessageStates.Contaminated;

				// Return force contaminate state to be be true.
				return AssetTrackingDetailClass.MessageStates.Contaminated;
			}

			return detail.MessageState;
		}

		/// <summary>
		/// This method will retrieve a WRDCU tank data based on the asset tracking detail GUID.  It will return all the tanks.
		/// </summary>
		/// <param name="detailRecordList">The tracking detail GUID used to retrieve the WRDCU tanks.</param>
		/// <param name="security">The security object.</param>
		private void AssociatedWrdcuTanks(List<AssetTrackingDetailClass> detailRecordList, SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (detailRecordList.Count == 0)
			{
				return;
			}

			using (var sqlCommand = new SqlCommand())
			{
				var assetTrackingTank = new AssetTrackingTankClass();
				assetTrackingTank.GetAssociatedWrdcuTanksSql(sqlCommand, detailRecordList);

				DataSet dataSet = this.consolidatedDA.GetDataSet(sqlCommand, security);

				if (dataSet == null || dataSet.Tables.Count < 1 || dataSet.Tables[0].Rows.Count < 1)
				{
					return;
				}

				DataRowCollection rows = dataSet.Tables[0].Rows;

				foreach (DataRow row in rows)
				{
					assetTrackingTank = new AssetTrackingTankClass();
					assetTrackingTank.Load(row);

					AssetTrackingDetailClass detailRecord = detailRecordList.Find(x => x.AssetTrackingDetailGuid == assetTrackingTank.AssetTrackingDetailGuid);
					
					if (detailRecord != null && detailRecord.PayloadType == AssetTrackingDetailClass.PayloadTypes.Wrdcu)
					{
						if (detailRecord.TrackingTanks == null)
						{
							detailRecord.TrackingTanks = new List<AssetTrackingTankClass>();
						}

						detailRecord.TrackingTanks.Add(assetTrackingTank);
					}
				}
			}
		}
		#endregion
	}
}
