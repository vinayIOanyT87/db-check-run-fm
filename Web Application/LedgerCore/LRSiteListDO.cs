namespace LedgerCore
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;

	#region SiteListDO Class
	public class LRSiteListDO
	{
		#region Private data members
		private Hashtable siteListHsh;
		private DateTime defaultBeginDate;
		private readonly LedgerConnection ledgerConnection;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Site List class.
		/// </summary>
		public LRSiteListDO(DateTime inDefaultBeginDate, LedgerConnection inLedgerConnection)
		{
			this.ledgerConnection = inLedgerConnection;
			this.Init(inDefaultBeginDate);
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the list of sites.
		/// </summary>
		public Hashtable SiteList
		{
			get { return this.siteListHsh; }
		}

		/// <summary>
		/// This property will get the default begin date that is
		/// used to start retrieving data.
		/// </summary>
		public DateTime DefaultBeginDate
		{
			get { return this.defaultBeginDate; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will retrieve the list of sites if the given site guid is
		/// a site group. It will exluded itself from the list.
		/// </summary>
		/// <param name="siteId">The site ID to retrieve data from.</param>
		public void RetrieveSiteList(string siteId)
		{
			using (var command = new SqlCommand())
			{
				command.CommandType = CommandType.Text;
				command.CommandText =	"SELECT "
										+ "B.ID, "
										+ "B.SiteGuid, "
										+ "B.VolumeDecimalPlaces, "
										+ "VolumeUnitIndex, "
										+ "dbo.udf_ConvertFromSIUnits(@ConvertValue, B.VolumeUnitIndex, @RoundFactor) AS VolumeFactor, "
										+ "B.AdditiveVolumeDecimalPlaces, "
										+ "B.AdditiveVolumeUnitIndex, "
										+ "dbo.udf_ConvertFromSIUnits(@ConvertValue, B.AdditiveVolumeUnitIndex, @RoundFactor) AS AdditiveVolumeFactor, "
										+ "B.MassDecimalPlaces, " 
										+ "B.MassUnitIndex, "
										+ "dbo.udf_ConvertFromSIUnits(@ConvertValue, B.MassUnitIndex, @RoundFactor) AS MassFactor, "
										+ "B.EnforceSingleOwner, "
										+ "B.SiteGroupFlag, "
										+ "InhibitSiteLedgerRollup "
										+ "FROM [dbo].[udf_GetSiteToSiteHierarchyListForSiteID](@SiteID,0,0,1,0,0,0) A "
										+ "LEFT JOIN tblSites B ON A.SiteGuid = B.SiteGuid AND (B.InhibitSiteLedgerRollup = CAST(0 as BIT) OR B.ID = @SiteID) ";


				command.Parameters.Add("@SiteID", SqlDbType.NVarChar, 30).Value = siteId;
				command.Parameters.Add("@ConvertValue", SqlDbType.Float).Value = LRSiteDO.ConvertValue;
				command.Parameters.Add("@RoundFactor", SqlDbType.Int).Value = LRSiteDO.MaxRoundValue;

				DataSet dataSet = this.ledgerConnection.GetDataSet(command);

				// Load the results.
				this.LoadSiteList(dataSet);
			}
		}

		/// <summary>
		/// This method will add a Site data object to the list if it does
		/// not exist.
		/// </summary>
		/// <param name="siteDO"></param>
		public void AddSiteToList(LRSiteDO siteDO)
		{
			if ((siteDO != null) && (string.IsNullOrEmpty(siteDO.SiteName) == false))
			{
				if (this.siteListHsh.Contains(siteDO.SiteName) == false)
				{
					this.siteListHsh.Add(siteDO.SiteName, siteDO);
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will load the list of sites if the given site guid is
		/// a site group. It will excluded the current site group.
		/// </summary>
		/// <param name="dataSet"></param>
		private void LoadSiteList(DataSet dataSet)
		{
			this.siteListHsh.Clear();

			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				var table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
					{
						var row = table.Rows[rowIndex];

						string siteName							= (row.IsNull("ID")) ? string.Empty : (string)row["ID"];
						int volumeDecimalPlaces					= (row.IsNull("VolumeDecimalPlaces")) ? 0 : Convert.ToInt32(row["VolumeDecimalPlaces"]);
						int volumeUnitIndex						= (row.IsNull("VolumeUnitIndex")) ? 1 : Convert.ToInt32(row["VolumeUnitIndex"]);
						double volumeConversionFactor			= (row.IsNull("VolumeFactor")) ? 1.0 : (double)row["VolumeFactor"];
						int additiveVolumeDecimalPlaces			= (row.IsNull("AdditiveVolumeDecimalPlaces")) ? 0 : Convert.ToInt32(row["AdditiveVolumeDecimalPlaces"]);
						int additiveVolumeUnitIndex				= (row.IsNull("AdditiveVolumeUnitIndex")) ? 1 : Convert.ToInt32(row["AdditiveVolumeUnitIndex"]);
						double additiveVolumeConversionFactor	= (row.IsNull("AdditiveVolumeFactor")) ? 1.0 : (double)row["AdditiveVolumeFactor"];
						int massDecimalPlaces					= (row.IsNull("MassDecimalPlaces")) ? 0 : Convert.ToInt32(row["MassDecimalPlaces"]);
						int massUnitIndex						= (row.IsNull("MassUnitIndex")) ? 1 : Convert.ToInt32(row["MassUnitIndex"]);
						double massConversionFactor				= (row.IsNull("MassFactor")) ? 1.0 : (double)row["MassFactor"];
						bool singleOwner						= (row.IsNull("EnforceSingleOwner")) ? false : (bool)row["EnforceSingleOwner"];
						bool siteGroupFlag						= (row.IsNull("SiteGroupFlag")) ? false : (bool)row["SiteGroupFlag"];
						string siteGuidStr						= (row.IsNull("SiteGuid")) ? Guid.Empty.ToString() : row["SiteGuid"].ToString();

						if (this.siteListHsh.Contains(siteName) == false)
						{
							var siteDO = new LRSiteDO(this.defaultBeginDate)
							             {
								             SiteName						= siteName,
								             SiteGuid						= Guid.Parse(siteGuidStr),
								             VolumeDecimalPlaces			= volumeDecimalPlaces,
								             VolumeUnitIndex				= volumeUnitIndex,
								             VolumeConversionFactor			= volumeConversionFactor,
								             AdditiveVolumeDecimalPlaces	= additiveVolumeDecimalPlaces,
								             AdditiveVolumeUnitIndex		= additiveVolumeUnitIndex,
								             AdditiveVolumeConversionFactor = additiveVolumeConversionFactor,
								             MassDecimalPlaces				= massDecimalPlaces,
								             MassUnitIndex					= massUnitIndex,
								             MassConversionFactor			= massConversionFactor,
								             SingleOwner					= singleOwner,
								             SiteGroupFlag					= siteGroupFlag
							             };

							this.siteListHsh.Add(siteName, siteDO);
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will initialize the Site List object to its intial state.
		/// </summary>
		private void Init(DateTime inDefaultBeginDate)
		{
			this.siteListHsh = new Hashtable();
			this.defaultBeginDate = inDefaultBeginDate;
		}
		#endregion
	}
	#endregion

	#region SiteDO class
	public class LRSiteDO
	{
		#region Public data members
		public const int MaxRoundValue = 2147483647;
		public const double ConvertValue = 1;
		#endregion

		#region Private data members
		private string			siteName;
		private Guid			siteGuid;
		private bool			siteGroupFlag;
		private int				volumeDecimalPlaces;
		private int				volumeUnitIndex;
		private double			volumeConversionFactor;
		private int				additiveVolumeDecimalPlaces;
		private int				additiveVolumeUnitIndex;
		private double			additiveVolumeConversionFactor;
		private int				massDecimalPlaces;
		private int				massUnitIndex;
		private double			massConversionFactor;
		private bool			singleOwner;
		private bool			inhibitSiteLedgerRollup;
		private bool			hasPhysicalInventory;
		private DateTime		physicalInvDateForLedgerStart;
		private bool			physicalOnLastDay;  // For BSME only
		private DateTime		startDate;
		private bool			foundOnwerCloseoutRecord;
		private LRQuantityDO	initialBookInventory;
		private DateTime		closeOutDateForLedgerStart;
		private DateTime		ownerCloseOutDateForLedgerStart;
		private DateTime		ledgerBrokenBlendStatusDate;
		private DateTime		ledgerCloseoutStatusDate;
		private string			ledgerBrokenBlendStatusDateStr;
		private string			ledgerCloseoutStatusDateStr;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Site Data Object class
		/// </summary>
		public LRSiteDO()
		{
		}

		/// <summary>
		/// This constructor sets the begin date.
		/// </summary>
		/// <param name="beginDate"></param>
		public LRSiteDO(DateTime? beginDate)
		{
			this.Init(beginDate);
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the Site Name data member.
		/// </summary>
		public string SiteName
		{
			get { return this.siteName; }
			set
			{
				this.siteName = value;
				if (string.IsNullOrEmpty(this.siteName))
				{
					this.siteName = string.Empty;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the Site Guid data member.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		/// <summary>
		/// This property gets the Site Group Flag data member.
		/// True means that the site is a site group.
		/// </summary>
		public bool SiteGroupFlag
		{
			get { return this.siteGroupFlag; }
            set { this.siteGroupFlag = value; }
		}

		/// <summary>
		/// This property gets the Site Volume Unit Index. The default is
		/// one (SI units).
		/// </summary>
		public int VolumeUnitIndex
		{
			get { return this.volumeUnitIndex; }
			set { this.volumeUnitIndex = value; }
		}

		/// <summary>
		/// This property gets the Site Volume Decimal Places. The default
		/// is zero.
		/// </summary>
		public int VolumeDecimalPlaces
		{
			get { return this.volumeDecimalPlaces; }
			set { this.volumeDecimalPlaces = value; }
		}

		/// <summary>
		/// This property will get the Volume Conversion Factor from SI units.
		/// </summary>
		public double VolumeConversionFactor
		{
			get { return this.volumeConversionFactor; }
			set { this.volumeConversionFactor = value; }
		}

		/// <summary>
		/// This property gets the Site AdditiveVolume Unit Index. The default is
		/// one (SI units).
		/// </summary>
		public int AdditiveVolumeUnitIndex
		{
			get { return this.additiveVolumeUnitIndex; }
			set { this.additiveVolumeUnitIndex = value; }
		}

		/// <summary>
		/// This property gets the Site Additive Volume Decimal Places. The default
		/// is zero.
		/// </summary>
		public int AdditiveVolumeDecimalPlaces
		{
			get { return this.additiveVolumeDecimalPlaces; }
			set { this.additiveVolumeDecimalPlaces = value; }
		}

		/// <summary>
		/// This property will get the Additive Volume Conversion Factor from SI units.
		/// </summary>
		public double AdditiveVolumeConversionFactor
		{
			get { return this.additiveVolumeConversionFactor; }
			set { this.additiveVolumeConversionFactor = value; }
		}

		/// <summary>
		/// This property gets the Site Mass Unit Index. The default is
		/// one (SI units).
		/// </summary>
		public int MassUnitIndex
		{
			get { return this.massUnitIndex; }
			set { this.massUnitIndex = value; }
		}

		/// <summary>
		/// This property gets the Site Additive Volume Decimal Places. The default
		/// is zero.
		/// </summary>
		public int MassDecimalPlaces
		{
			get { return this.massDecimalPlaces; }
			set { this.massDecimalPlaces = value; }
		}

		/// <summary>
		/// This property will get the Additive Volume Conversion Factor from SI units.
		/// </summary>
		public double MassConversionFactor
		{
			get { return this.massConversionFactor; }
			set { this.massConversionFactor = value; }
		}


		/// <summary>
		/// This property will return true if the site is a single owner site.
		/// </summary>
		public bool SingleOwner
		{
			get { return this.singleOwner; }
			set { this.singleOwner = value; }
		}

		/// <summary>
		/// This property will return true if the site has a physical inventory
		/// record. It will return false otherwise.
		/// </summary>
		public bool HasPhysicalInventory
		{
			get { return this.hasPhysicalInventory; }
			set { this.hasPhysicalInventory = value; }
		}

		/// <summary>
		/// This property will return the physical inventory date if a physical
		/// inventory was found.  It will return 1901-01-01 as a default.
		/// </summary>
		public DateTime PhysicalInvDateForLedgerStart
		{
			get { return this.physicalInvDateForLedgerStart; }
			set { this.physicalInvDateForLedgerStart = value; }
		}

		/// <summary>
		/// This property contains a flag that indicates whether
		/// the physical inventory was ound on the last day of the
		/// month.  This is BSME specific.
		/// </summary>
		public bool PhysicalOnLastDay
		{
			get { return this.physicalOnLastDay; }
			set { this.physicalOnLastDay = value; }
		}

		/// <summary>
		/// This property contains the start date for gathering ledger
		/// data for this site.
		/// </summary>
		public DateTime StartDate
		{
			get { return this.startDate; }
			set { this.startDate = value; }
		}

		/// <summary>
		/// This property contains a flag that indicates whether
		/// a owner closeout record was found. True = found, False =
		/// not found.
		/// </summary>
		public bool FoundOnwerCloseoutRecord
		{
			get { return this.foundOnwerCloseoutRecord; }
			set { this.foundOnwerCloseoutRecord = value; }
		}

		/// <summary>
		/// This property contains the initial book inventory
		/// for this site.
		/// </summary>
		public LRQuantityDO InitialBookInventory
		{
			get { return this.initialBookInventory; }
			set { this.initialBookInventory = value; }
		}

		/// <summary>
		/// This property contains the closeout date from the
		/// owner closeout record.
		/// </summary>
		public DateTime CloseoutDateForLedgerStart
		{
			get { return this.closeOutDateForLedgerStart; }
			set { this.closeOutDateForLedgerStart = value; }
		}

		/// <summary>
		/// This property contains the owner closeout date from the
		/// owner closeout record.
		/// </summary>
		public DateTime OwnerCloseoutDateForLedgerStart
		{
			get { return this.ownerCloseOutDateForLedgerStart; }
			set { this.ownerCloseOutDateForLedgerStart = value; }
		}

		/// <summary>
		/// This property contains the broken blend date from the
		/// transaction sub-line item record to set the ledger status.
		/// </summary>
		public DateTime LedgerBrokenBlendStatusDate
		{
			get { return this.ledgerBrokenBlendStatusDate; }
			set { this.ledgerBrokenBlendStatusDate = value; }
		}

		/// <summary>
		/// This property contains the ledger broken blend date
		/// to set the ledger status (in string format).
		/// </summary>
		public String LedgerBrokenBlendStatusDateStr
		{
			get { return this.ledgerBrokenBlendStatusDateStr; }
			set { this.ledgerBrokenBlendStatusDateStr = value; }
		}

		/// <summary>
		/// This property contains the ledger closeout date or owner closeout
		/// date to set the ledger status.
		/// </summary>
		public DateTime LedgerCloseoutStatusDate
		{
			get { return this.ledgerCloseoutStatusDate; }
			set { this.ledgerCloseoutStatusDate = value; }
		}

		/// <summary>
		/// This property contains the ledger closeout date or owner closeout
		/// date to set the ledger status (in string format).
		/// </summary>
		public String LedgerCloseoutStatusDateStr
		{
			get { return this.ledgerCloseoutStatusDateStr; }
			set { this.ledgerCloseoutStatusDateStr = value; }
		}

		/// <summary>
		/// Gets and sets the inhibit Site Ledger Rollup flag. True indicates
		/// the ledger for this site is not rolled up.
		/// </summary>
		public bool InhibitSiteLedgerRollup
		{
			get { return this.inhibitSiteLedgerRollup; }
			set { this.inhibitSiteLedgerRollup = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will retrieve site information such as the conversion factor and volume
		/// decimal places based on the site guid.
		/// </summary>
		/// <param name="inSiteGuid">The site unique identifier.</param>
		/// <param name="ledgerConnection"></param>
		public void RetrieveSiteInfo(Guid inSiteGuid, LedgerConnection ledgerConnection)
		{
			const string SQL =	"SELECT ID, "
								+ "SiteGuid, "
								+ "SiteGroupFlag, "
								+ "VolumeDecimalPlaces, "
								+ "VolumeUnitIndex, "
								+ "dbo.udf_ConvertFromSIUnits(@ConvertValue, VolumeUnitIndex, @RoundFactor) AS VolumeFactor, "
								+ "AdditiveVolumeDecimalPlaces, "
								+ "AdditiveVolumeUnitIndex, "
								+ "dbo.udf_ConvertFromSIUnits(@ConvertValue, AdditiveVolumeUnitIndex, @RoundFactor) AS AdditiveVolumeFactor, "
								+ "MassDecimalPlaces, "
								+ "MassUnitIndex, "
								+ "dbo.udf_ConvertFromSIUnits(@ConvertValue, MassUnitIndex, @RoundFactor) AS MassFactor, "
								+ "EnforceSingleOwner, "
								+ "InhibitSiteLedgerRollup "
								+ "FROM tblSites WHERE SiteGuid = @SiteGuid";

			using (var command = new SqlCommand(SQL))
			{
				command.Parameters.Add("@ConvertValue", SqlDbType.Float).Value = ConvertValue;
				command.Parameters.Add("@RoundFactor", SqlDbType.Int).Value = MaxRoundValue;
				command.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = inSiteGuid;

				DataSet dataSet = ledgerConnection.GetDataSet(command);

				// Load the retrieved site information.
				this.LoadSiteInfo(dataSet);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will load the retrieved site information such as the conversion factor and volume
		/// decimal.
		/// </summary>
		/// <param name="dataSet"></param>
		public void LoadSiteInfo(DataSet dataSet)
		{
			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					this.siteName						= (row.IsNull("ID")) ? string.Empty : (string)row["ID"];
					string siteGuidStr					= (row.IsNull("SiteGuid")) ? Guid.Empty.ToString() : row["SiteGuid"].ToString();
					this.siteGuid						= Guid.Parse(siteGuidStr);
					this.siteGroupFlag					= (!row.IsNull("SiteGroupFlag")) && (bool)row["SiteGroupFlag"];
					this.volumeDecimalPlaces			= (row.IsNull("VolumeDecimalPlaces")) ? 0 : Convert.ToInt32(row["VolumeDecimalPlaces"]);
					this.volumeUnitIndex				= (row.IsNull("VolumeUnitIndex")) ? 1 : Convert.ToInt32(row["VolumeUnitIndex"]);
					this.volumeConversionFactor			= (row.IsNull("VolumeFactor")) ? 1.0 : (double)row["VolumeFactor"];
					this.additiveVolumeDecimalPlaces	= (row.IsNull("AdditiveVolumeDecimalPlaces")) ? 0 : Convert.ToInt32(row["AdditiveVolumeDecimalPlaces"]);
					this.additiveVolumeUnitIndex		= (row.IsNull("AdditiveVolumeUnitIndex")) ? 1 : Convert.ToInt32(row["AdditiveVolumeUnitIndex"]);
					this.additiveVolumeConversionFactor = (row.IsNull("AdditiveVolumeFactor")) ? 1.0 : (double)row["AdditiveVolumeFactor"];
					this.massDecimalPlaces				= (row.IsNull("MassDecimalPlaces")) ? 0 : Convert.ToInt32(row["MassDecimalPlaces"]);
					this.massUnitIndex					= (row.IsNull("MassUnitIndex")) ? 1 : Convert.ToInt32(row["MassUnitIndex"]);
					this.massConversionFactor			= (row.IsNull("MassFactor")) ? 1.0 : (double)row["MassFactor"];
					this.singleOwner					= (!row.IsNull("EnforceSingleOwner")) && (bool)row["EnforceSingleOwner"];
					this.inhibitSiteLedgerRollup		= (!row.IsNull("InhibitSiteLedgerRollup")) && (bool) row["InhibitSiteLedgerRollup"];
				}
			}
		}

		/// <summary>
		/// This method will reset the object.
		/// </summary>
		/// <param name="beginDate"></param>
		public void Reset(DateTime? beginDate)
		{
			this.Init(beginDate);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initial the Site data object to its initial state.
		/// </summary>
		/// <param name="beginDate">The beginning date to start the search.</param>
		private void Init(DateTime? beginDate)
		{
			this.siteName						= string.Empty;
			this.siteGuid						= Guid.Empty;
			this.siteGroupFlag					= false;
			this.volumeDecimalPlaces			= 0;
			this.volumeUnitIndex				= 1;
			this.volumeConversionFactor			= 1;
			this.additiveVolumeDecimalPlaces	= 0;
			this.additiveVolumeUnitIndex		= 1;
			this.additiveVolumeConversionFactor = 1;
			this.massDecimalPlaces				= 0;
			this.massUnitIndex					= 1;
			this.massConversionFactor			= 1;
			this.singleOwner					= true;
			this.singleOwner					= true;
			this.inhibitSiteLedgerRollup		= false;
			this.hasPhysicalInventory			= false;
			this.physicalOnLastDay				= false;
			this.foundOnwerCloseoutRecord		= false;
			this.initialBookInventory			= new LRQuantityDO();
			this.ledgerCloseoutStatusDateStr	= string.Empty;
			this.ledgerBrokenBlendStatusDateStr = string.Empty;

			if (beginDate == null)
			{
				this.physicalInvDateForLedgerStart		= new DateTime(1901, 01, 01, 00, 00, 00);
				this.startDate							= new DateTime(1901, 01, 01, 00, 00, 00);
				this.closeOutDateForLedgerStart			= new DateTime(1901, 01, 01, 00, 00, 00);
				this.ownerCloseOutDateForLedgerStart	= new DateTime(1901, 01, 01, 00, 00, 00);
				this.ledgerBrokenBlendStatusDate		= new DateTime(1901, 01, 01, 00, 00, 00);
				this.ledgerCloseoutStatusDate			= new DateTime(1901, 01, 01, 00, 00, 00);
			}
			else
			{
				this.physicalInvDateForLedgerStart		= beginDate.Value.AddDays(-1);
				this.startDate							= beginDate.Value.AddDays(-1);
				this.closeOutDateForLedgerStart			= beginDate.Value.AddDays(-1);
				this.ownerCloseOutDateForLedgerStart	= beginDate.Value.AddDays(-1);
				this.ledgerBrokenBlendStatusDate		= beginDate.Value.AddDays(-1);
				this.ledgerCloseoutStatusDate			= beginDate.Value.AddDays(-1);
			}
		}
		#endregion
	}
	#endregion
}