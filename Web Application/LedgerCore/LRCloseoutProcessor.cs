namespace LedgerCore
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;

    // ReSharper disable once InconsistentNaming
	public class LRCloseoutProcessor
	{
		#region Private data members
		private readonly LRDateConverter dateConverter;
		private double massFactor;
		private double volumeFactor;
		private int volumePrecision;
		private int massPrecision;

		private readonly LedgerConnection ledgerConnection;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Closeout class.
		/// </summary>
		public LRCloseoutProcessor(LedgerConnection inLedgerConnection)
		{
			this.ledgerConnection = inLedgerConnection;
			this.dateConverter = new LRDateConverter();
			this.Reset();
		}
		#endregion

		#region Properties
		public double VolumeFactor
		{
			get { return this.volumeFactor; }
			set { this.volumeFactor = value; }
		}

		public int VolumePrecision
		{
			get { return this.volumePrecision; }
			set { this.volumePrecision = value; }
		}

		public double MassFactor
		{
			get { return this.massFactor; }
			set { this.massFactor = value; }
		}

		public int MassPrecision
		{
			get { return this.massPrecision; }
			set { this.massPrecision = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		public void Reset()
		{
			this.massFactor			= 1.0;
			this.volumeFactor		= 1.0;
			this.massPrecision		= 2;
			this.volumePrecision	= 2;
		}

		/// <summary>
		/// This method will return an Owner Closeout DO based on the manager, owner, product, 
		/// and site.
		/// </summary>
		/// <returns></returns>
		public List<LROwnerCloseoutDO> RetrieveOwnerCloseoutRecord(	List<LRSiteDO> siteList,
																	int nonSiteGroupCount,
																	DateTime ledgerStartDate,
																	Guid managerGuid,
																	Guid ownerGuid,
																	Guid productGuid)
		{
			var ownerCloseoutList = new List<LROwnerCloseoutDO>();

			var ownerCloseoutDO = new LROwnerCloseoutDO
			                      {
				                      ManagerGuid = managerGuid,
				                      OwnerGuid = ownerGuid,
				                      ProductGuid = productGuid
			                      };

			using (var command = new SqlCommand())
			{
				DateTimeOffset startDate = this.dateConverter.GetDateWithCorrectTimePortion(ledgerStartDate, LRDateConverter.TimeTypes.Start);

				ownerCloseoutDO.GetCurrentOwnerCloseoutSelectSQL(command, siteList, startDate);
				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						ownerCloseoutDO = new LROwnerCloseoutDO(ledgerStartDate)
						                  {
							                  VolumeFactor = this.volumeFactor,
							                  VolumePrecision = this.volumePrecision,
							                  MassFactor = this.massFactor,
							                  MassPrecision = this.massPrecision
						                  };

						ownerCloseoutDO.LoadCurrentOwnerCloseout(row);
						ownerCloseoutList.Add(ownerCloseoutDO);
					}
				}
			}

			return ownerCloseoutList;
		}

		/// <summary>
		/// This method will return an Owner Closeout DO based on the manager, owner, product, 
		/// and site.
		/// </summary>
		/// <returns></returns>
		public List<LROwnerCloseoutDO> RetrieveOwnerCloseoutRecordSingleSite(	List<LRSiteDO> siteList,
																				DateTime ledgerStartDate,
																				Guid managerGuid,
																				Guid ownerGuid,
																				Guid productGuid)
		{
			var ownerCloseoutList = new List<LROwnerCloseoutDO>();

			var ownerCloseoutDO = new LROwnerCloseoutDO
			                      {
				                      ManagerGuid = managerGuid,
				                      OwnerGuid = ownerGuid,
				                      ProductGuid = productGuid
			                      };

			using (var command = new SqlCommand())
			{
				DateTimeOffset startDate = this.dateConverter.GetDateWithCorrectTimePortion(ledgerStartDate, LRDateConverter.TimeTypes.Start);

				ownerCloseoutDO.GetCurrentOwnerCloseoutSingleSiteSelectSQL(command, startDate);
				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						ownerCloseoutDO = new LROwnerCloseoutDO(ledgerStartDate)
						                  {
							                  VolumeFactor = this.volumeFactor,
							                  VolumePrecision = this.volumePrecision,
							                  MassFactor = this.massFactor,
							                  MassPrecision = this.massPrecision
						                  };

						ownerCloseoutDO.LoadCurrentOwnerCloseout(row);
						ownerCloseoutDO.SiteGuid = siteList[0].SiteGuid;
						ownerCloseoutDO.SiteName = siteList[0].SiteName;
						ownerCloseoutList.Add(ownerCloseoutDO);
					}
				}
			}

			return ownerCloseoutList;
		}

		/// <summary>
		/// This method will retrieve the most recent owner closeout date based on the manager, 
		/// owner, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <returns></returns>
		public List<LROwnerCloseoutDO> RetrieveMostRecentOwnerCloseoutDate(	List<LRSiteDO> siteList,
																			int nonSiteGroupCount,
																			Guid managerGuid,
																			Guid ownerGuid,
																			Guid productGuid,
																			DateTime beginDate)
		{
			var ownerCloseoutList = new List<LROwnerCloseoutDO>();

			var ownerCloseoutDO = new LROwnerCloseoutDO
			                      {
				                      ManagerGuid = managerGuid,
				                      OwnerGuid = managerGuid,
				                      ProductGuid = productGuid
			                      };

			using (var command = new SqlCommand())
			{
				DateTimeOffset ledgerStartDate = this.dateConverter.GetDateWithCorrectTimePortion(beginDate, LRDateConverter.TimeTypes.Start);

				ownerCloseoutDO.GetLatestCloseoutDateSelectSQL(command, siteList, ledgerStartDate);
				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						ownerCloseoutDO = new LROwnerCloseoutDO();
						ownerCloseoutDO.LoadLatestCloseoutDate(row);
						ownerCloseoutList.Add(ownerCloseoutDO);
					}
				}
			}

			return ownerCloseoutList;
		}

		/// <summary>
		/// This method will retrieve the most recent owner closeout date based on the manager, 
		/// owner, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <returns></returns>
		public List<LROwnerCloseoutDO> RetrieveMostRecentOwnerCloseoutDateSingleSite(List<LRSiteDO> siteList,
																					Guid managerGuid,
																					Guid ownerGuid,
																					Guid productGuid,
																					DateTime beginDate)
		{
			var ownerCloseoutList = new List<LROwnerCloseoutDO>();
			var ownerCloseoutDO = new LROwnerCloseoutDO
			                      {
				                      ManagerGuid = managerGuid,
				                      OwnerGuid = ownerGuid,
				                      ProductGuid = productGuid,
									  SiteGuid = siteList[0].SiteGuid
			                      };

			using (var command = new SqlCommand())
			{
				DateTimeOffset ledgerStartDate = this.dateConverter.GetDateWithCorrectTimePortion(beginDate, LRDateConverter.TimeTypes.Start);
				ownerCloseoutDO.GetLatestCloseoutDateSingleSiteSelectSQL(command, ledgerStartDate);

				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						ownerCloseoutDO = new LROwnerCloseoutDO();
						ownerCloseoutDO.LoadLatestCloseoutDateSingleSite(row);

						ownerCloseoutDO.SiteName = siteList[0].SiteName;
						ownerCloseoutDO.SiteGuid = siteList[0].SiteGuid;

						ownerCloseoutList.Add(ownerCloseoutDO);
					}
				}
			}

			return ownerCloseoutList;
		}

		/// <summary>
		/// This method will return the Closeout DO with the most recent closeout date based on the
		/// manager, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <returns></returns>
		public List<LRCloseoutDO> RetrieveMostRecentCloseoutDate(List<LRSiteDO> siteList,
																int nonSiteGroupCount,
																Guid managerGuid,
																Guid productGuid,
																DateTime beginDate)
		{
			var closeoutList = new List<LRCloseoutDO>();

			using (var command = new SqlCommand())
			{
				var closeoutDO = new LRCloseoutDO
				                 {
					                 ManagerGuid = managerGuid, 
									 ProductGuid = productGuid, 
									 SiteGuid = siteList[0].SiteGuid
				                 };

				DateTimeOffset ledgerStartDate = this.dateConverter.GetDateWithCorrectTimePortion(beginDate, LRDateConverter.TimeTypes.Start);

				closeoutDO.GetLatestCloseoutDateSelectSQL(command, siteList, ledgerStartDate);
				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						closeoutDO = new LRCloseoutDO();
						closeoutDO.LoadLatestCloseoutDate(row);

						closeoutList.Add(closeoutDO);
					}
				}
			}

			return closeoutList;
		}

		/// <summary>
		/// This method will return the Closeout DO with the most recent closeout date based on the
		/// manager, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <returns></returns>
		public List<LRCloseoutDO> RetrieveMostRecentCloseoutDateSingleSite(	List<LRSiteDO> siteList,
																			Guid managerGuid,
																			Guid productGuid,
																			DateTime beginDate)
		{
			var closeoutList = new List<LRCloseoutDO>();

			using (var command = new SqlCommand())
			{
				var closeoutDO = new LRCloseoutDO
				                 {
					                 ManagerGuid = managerGuid, 
									 ProductGuid = productGuid,
									 SiteGuid = siteList[0].SiteGuid
				                 };

				DateTimeOffset ledgerStartDate = this.dateConverter.GetDateWithCorrectTimePortion(beginDate, LRDateConverter.TimeTypes.Start);

				closeoutDO.GetLatestCloseoutDateSingleSiteSelectSQL(command, ledgerStartDate);
				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						closeoutDO = new LRCloseoutDO();
						closeoutDO.LoadLatestCloseoutDateSingleSite(row);

						// Need to add the site info since the query is not retrieving it.
						closeoutDO.SiteID = siteList[0].SiteName;
						closeoutDO.SiteGuid = siteList[0].SiteGuid;

						closeoutList.Add(closeoutDO);
					}
				}
			}

			return closeoutList;
		}

		/// <summary>
		/// This method will return the Closeout DO with the most recent closeout date based on the
		/// manager, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <returns></returns>
		public List<LRCloseoutDO> RetrieveMostRecentBrokenBlendDate(List<LRSiteDO> siteList,
																	int nonSiteGroupCount,
																	Guid managerGuid,
																	Guid productGuid,
																	DateTime endDate)
		{
			var closeoutList = new List<LRCloseoutDO>();

			using (var command = new SqlCommand())
			{
				var closeoutDO = new LRCloseoutDO
				                 {
					                 ManagerGuid = managerGuid, 
									 ProductGuid = productGuid,
				                 };

				DateTimeOffset ledgerEndDate = this.dateConverter.GetDateWithCorrectTimePortion(endDate, LRDateConverter.TimeTypes.End);
				DateTimeOffset lastCloseoutDate = this.dateConverter.GetDateWithCorrectTimePortion(closeoutDO.CloseoutDate, 
																							 LRDateConverter.TimeTypes.Start);

				closeoutDO.LastCloseoutDate = lastCloseoutDate;
				closeoutDO.GetBrokenBlendDateSelectSQL(command, siteList, ledgerEndDate);

				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						closeoutDO = new LRCloseoutDO();
						closeoutDO.LoadBrokenBlendDate(row);
						closeoutList.Add(closeoutDO);
					}
				}
			}

			return closeoutList;
		}

		/// <summary>
		/// This method will return the Closeout DO with the most recent closeout date based on the
		/// manager, product, site, and the closeout that is great than or equal to the ledger
		/// start date.
		/// </summary>
		/// <returns></returns>
		public List<LRCloseoutDO> RetrieveMostRecentBrokenBlendDateSingleSite(	List<LRSiteDO> siteList,
																				Guid managerGuid,
																				Guid productGuid,
																				DateTime endDate)
		{
			var closeoutList = new List<LRCloseoutDO>();

			using (var command = new SqlCommand())
			{
				var closeoutDO = new LRCloseoutDO
				                 {
					                 ManagerGuid = managerGuid, 
									 ProductGuid = productGuid,
									 SiteGuid = siteList[0].SiteGuid
				                 };

				DateTimeOffset ledgerEndDate = this.dateConverter.GetDateWithCorrectTimePortion(endDate, LRDateConverter.TimeTypes.End);
				DateTimeOffset lastCloseoutDate = this.dateConverter.GetDateWithCorrectTimePortion(closeoutDO.CloseoutDate, 
																							 LRDateConverter.TimeTypes.Start);

				closeoutDO.LastCloseoutDate = lastCloseoutDate;
				closeoutDO.GetBrokenBlendDateSingleSiteSelectSQL(command, ledgerEndDate);

				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						closeoutDO = new LRCloseoutDO();
						closeoutDO.LoadBrokenBlendDateSingleSite(row);

						closeoutDO.SiteGuid = siteList[0].SiteGuid;
						closeoutDO.SiteID = siteList[0].SiteName;

						closeoutList.Add(closeoutDO);
					}
				}

				return closeoutList;
			}
		}
		#endregion
	}
}