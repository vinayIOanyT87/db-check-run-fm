namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using FMBusinessObjects.UtilityObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public enum ALLOCATION_TYPE
	{
		PRODUCT_ALLOCATION = 0,
		PRODUCT_GROUP_ALLOCATION = 1,
		ALL_PRODUCTS_ALLOCATION = 2,
		MAX_ALLOCATION_TYPE = 3
	};
	public enum ALLOCATION_RESET_PERIOD
	{
		DAY_RESET_PERIOD = 0,
		WEEK_RESET_PERIOD = 1,
		MONTH_RESET_PERIOD = 2,
		YEAR_RESET_PERIOD = 3,
		MAX_RESET_PERIOD = 4
	};
	public enum ALLOCATION_RESET_METHOD
	{
		REPEAT_METHOD = 0,
		BALANCE_FORWARD_METHOD = 1,
		NEXT_LIMIT_METHOD = 2,
		NEXT_PLUS_BALANCE_FORWARD_METHOD = 3,
		BOOK_MINUS_UNAVAILABLE_METHOD = 4,
		MAX_ALLOCATION_METHOD = 5
	};

	/// <summary>
	/// Summary description for AllocationLineItem.
	/// </summary>
	[Serializable()]
	[CollectionDataContract]
	[KnownType(typeof(AllocationLineItemClass))]
	public class AllocationLineItemCollectionClass : CollectionBase
	{
		public void Add(AllocationLineItemClass AllocationLineItem)
		{
		    this.List.Add(AllocationLineItem);
		}

		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
			    this.List.RemoveAt(index);
			}
		}

		public void Remove(AllocationLineItemClass AllocationLineItem)
		{
			int index = 0;
			foreach (AllocationLineItemClass Item in this.List)
			{
				if (Item.IdentityGuid == AllocationLineItem.IdentityGuid)
				{
				    this.List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public AllocationLineItemClass Item(int Index)
		{
			return (AllocationLineItemClass)this.List[Index];
		}
	}

	/// <summary>
	/// Summary description for AllocationLineItemClass.
	/// </summary>
	[Serializable()]
	[DataContract]
	public class AllocationLineItemClass : BaseDataObject, IAlarmAndEventDiscovery
	{
		[DataMember]
		public Guid AllocationGuid;
		[DataMember]
		public ALLOCATION_TYPE Type;
		[DataMember]
		public Guid AssignedGuid;
		[DataMember]
		public SIDouble Limit;
		[DataMember]
		public SIDouble Loaded;					// Computed Value
		[DataMember]
		public SIDouble Next;
		[DataMember]
		public ALLOCATION_RESET_PERIOD ResetPeriod;
		[DataMember]
		public int _ResetMultiple;
		[DataMember]
		public ALLOCATION_RESET_METHOD _ResetMethod;
		[DataMember]
		public Date ResetDate;

		// Linked Items
		[DataMember]
		public string AssignedID;

		static string AllocationWarningKey = "Allocation Warning";
		public static AlarmAndEventDescriptorClass AllocationWarningAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, AllocationWarningKey);
		static string AllocationDenialKey = "Allocation Denial";
		public static AlarmAndEventDescriptorClass AllocationDenialAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, AllocationDenialKey);
		static string BlendComponentAllocationWarningKey = "Blend Component Allocation Warning";
		public static AlarmAndEventDescriptorClass BlendComponentAllocationWarningAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, BlendComponentAllocationWarningKey);
		static string BlendComponentAllocationDenialKey = "Blend Component Allocation Denial";
		public static AlarmAndEventDescriptorClass BlendComponentAllocationDenialAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, BlendComponentAllocationDenialKey);

		public override string ID { get { return this.AssignedID; } set {
		    this._ID = value; } }

		public int ResetMultiple
		{
			get { return this._ResetMultiple; }
			set
			{
				if (value == 0)
					throw new Exception("Reset Multiple must not be zero");
			    this._ResetMultiple = value;
			}
		}

		public ALLOCATION_RESET_METHOD ResetMethod
		{
			get { return this._ResetMethod; }
			set
			{
				if (value == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD) this.ResetPeriod = ALLOCATION_RESET_PERIOD.MAX_RESET_PERIOD;

			    this._ResetMethod = value;
			}
		}

		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] Descriptors ={	AllocationWarningAlarmDescriptor,
																		AllocationDenialAlarmDescriptor,
																		BlendComponentAllocationWarningAlarmDescriptor,
																		BlendComponentAllocationDenialAlarmDescriptor
																	};
				return Descriptors;
			}
		}



		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.ALLOCATION_LINE_ITEM;
			}
		}

		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.ALLOCATION_GROUP;
			}
		}

		public static string GetAssignedColumnName(ALLOCATION_TYPE allocationType)
		{
			switch (allocationType)
			{
				case ALLOCATION_TYPE.PRODUCT_ALLOCATION:
					return "AssignedProductGuid";
				case ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION:
					return "AssignedApplicationStringGuid";
				default:
					return "Unknown";
			}
		}

		string SelectClause = "SELECT tblAllocationLineItems.*," +
									"((SELECT ProductID FROM tblProducts WHERE tblAllocationLineItems.LookupAllocationTypeIndex = 0 AND tblAllocationLineItems.AssignedProductGuid = tblProducts.ProductGuid) UNION " +
									"(SELECT ID FROM tblApplicationString WHERE tblAllocationLineItems.LookupAllocationTypeIndex = 1 AND tblAllocationLineItems.AssignedApplicationStringGuid = tblApplicationString.ApplicationStringGuid)) AS AssignedID ";


		public AlarmAndEventLogClass AllocationWarningAlarm(string HierarchyID, string ProductID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(AllocationWarningAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = HierarchyID + " : " + ProductID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass AllocationDenialAlarm(string HierarchyID, string ProductID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(AllocationDenialAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = HierarchyID + " : " + ProductID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass BlendComponentAllocationWarningAlarm(string hierarchyID, string productID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(BlendComponentAllocationWarningAlarmDescriptor)
		                                             {
		                                                 AssociatedData = hierarchyID + " : " + productID
		                                             };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass BlendComponentAllocationDenialAlarm(string hierarchyID, string productID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(BlendComponentAllocationDenialAlarmDescriptor)
		                                             {
		                                                 AssociatedData = hierarchyID + " : " + productID
		                                             };
		    return alarmAndEventLog;
		}


		public AllocationLineItemClass()
		{
		    this.Limit = new SIDouble(EngineeringUnit.FmvMeter3, NumberFormatInfo.CurrentInfo, 0);
		    this.Loaded = new SIDouble(EngineeringUnit.FmvMeter3, NumberFormatInfo.CurrentInfo, 0);
		    this.Next = new SIDouble(EngineeringUnit.FmvMeter3, NumberFormatInfo.CurrentInfo, 0);
		    this.ResetDate = new Date();

		    this.Initialize();
		}

		public AllocationLineItemClass(SiteClass Site)
		{
		    this.Limit = new SIDouble(Site.VolumeUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 0);
		    this.Loaded = new SIDouble(Site.VolumeUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 0);
		    this.Next = new SIDouble(Site.VolumeUnits, Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 0);
		    this.ResetDate = new Date(Site);

		    this.Initialize();
		}

		public static string TypeID(ALLOCATION_TYPE AllocType)
		{
			switch (AllocType)
			{
				case ALLOCATION_TYPE.PRODUCT_ALLOCATION:
					return "Product";
				case ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION:
					return "Group";
				case ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION:
					return "All Products";
				default:
					return "Undefined";
			}
		}

		public static string ResetPeriodID(ALLOCATION_RESET_PERIOD ResetPeriod)
		{
			switch (ResetPeriod)
			{
				case ALLOCATION_RESET_PERIOD.DAY_RESET_PERIOD:
					return "Day";
				case ALLOCATION_RESET_PERIOD.WEEK_RESET_PERIOD:
					return "Week";
				case ALLOCATION_RESET_PERIOD.MONTH_RESET_PERIOD:
					return "Month";
				case ALLOCATION_RESET_PERIOD.YEAR_RESET_PERIOD:
					return "Year";
				default:
					return "Undefined";
			}
		}

		public static string ResetMethodID(ALLOCATION_RESET_METHOD ResetMethod)
		{
			switch (ResetMethod)
			{
				case ALLOCATION_RESET_METHOD.REPEAT_METHOD:
					return "Repeat";
				case ALLOCATION_RESET_METHOD.BALANCE_FORWARD_METHOD:
					return "Balance";
				case ALLOCATION_RESET_METHOD.NEXT_LIMIT_METHOD:
					return "Next";
				case ALLOCATION_RESET_METHOD.NEXT_PLUS_BALANCE_FORWARD_METHOD:
					return "Next+Balance";
				case ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD:
					return "Book-Unavailable";
				default:
					return "Undefined";
			}
		}

		private void Initialize()
		{
		    this.IdentityGuid = Guid.Empty;
		    this.AllocationGuid = Guid.Empty;
		    this.Type = ALLOCATION_TYPE.PRODUCT_ALLOCATION;
		    this.AssignedGuid = Guid.Empty;
		    this.Limit.Value = 0.0;
		    this.Loaded.Value = 0.0;
		    this.Next.Value = 0.0;
		    this.ResetPeriod = ALLOCATION_RESET_PERIOD.DAY_RESET_PERIOD;
		    this.ResetMultiple = 1;
		    this.ResetMethod = ALLOCATION_RESET_METHOD.REPEAT_METHOD;
		    this.ResetDate.Value = TimeConverter.Today(this.ResetDate.StandardName);
		    this.AssignedID = "";
		}

		public override void Reset()
		{
			base.Reset();
		    this.Initialize();
		}

		public bool SetResetDate(DateTimeOffset EffectiveDate, DateTimeOffset ExpirationDate, DateTimeOffset today)
		{
			// Reset is performed externally
			if (this.ResetMethod == ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD)
				return false;

			TimeSpan Delta = new TimeSpan(0);
			DateTimeOffset NewResetDate = DateTimeOffset.Now;

			if (today < ExpirationDate)
			{
				if (today > EffectiveDate)
					Delta = today - EffectiveDate;
			}
			else
				Delta = ExpirationDate - EffectiveDate;

			switch (this.ResetPeriod)
			{
				case ALLOCATION_RESET_PERIOD.DAY_RESET_PERIOD:
					NewResetDate = EffectiveDate.AddDays(((int)Delta.Days / this.ResetMultiple) * this.ResetMultiple);
					break;
				case ALLOCATION_RESET_PERIOD.WEEK_RESET_PERIOD:
					NewResetDate = EffectiveDate.AddDays(((int)(Delta.Days + (int)EffectiveDate.DayOfWeek) / (this.ResetMultiple * 7)) * this.ResetMultiple * 7 - (int)EffectiveDate.DayOfWeek);
					break;
				case ALLOCATION_RESET_PERIOD.MONTH_RESET_PERIOD:
					NewResetDate = new DateTimeOffset(EffectiveDate.Year, EffectiveDate.Month, 1, 0, 0, 0, EffectiveDate.Offset);
					while (NewResetDate < ExpirationDate && NewResetDate < today.AddMonths(-this.ResetMultiple))
						NewResetDate = NewResetDate.AddMonths(this.ResetMultiple);
					break;
				case ALLOCATION_RESET_PERIOD.YEAR_RESET_PERIOD:
					NewResetDate = new DateTimeOffset(EffectiveDate.Year, 1, 1, 0, 0, 0, EffectiveDate.Offset);
					while (NewResetDate < ExpirationDate && NewResetDate < today.AddYears(-this.ResetMultiple))
						NewResetDate = NewResetDate.AddYears(this.ResetMultiple);
					break;
				default:
					NewResetDate = this.ResetDate.Value;
					break;
			}

			if (this.ResetDate.Value == NewResetDate)
				return false;

		    this.ResetDate.Value = NewResetDate;

			switch (this.ResetMethod)
			{
				case ALLOCATION_RESET_METHOD.REPEAT_METHOD:
					break;
				case ALLOCATION_RESET_METHOD.BALANCE_FORWARD_METHOD:
			        this.Limit.SIValue = this.Limit.SIValue - this.Loaded.SIValue;
					break;
				case ALLOCATION_RESET_METHOD.NEXT_LIMIT_METHOD:
			        this.Limit.SIValue = this.Next.SIValue;
					break;
				case ALLOCATION_RESET_METHOD.NEXT_PLUS_BALANCE_FORWARD_METHOD:
			        this.Limit.SIValue = this.Next.SIValue + this.Limit.SIValue - this.Loaded.SIValue;
					break;
				case ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD:
			        this.Limit.SIValue = 0;
					break;
			}

		    this.Loaded.Value = 0.0;

			return true;
		}


		public void Load(DataSet Set)
		{
			if (Set == null)
				throw new ArgumentNullException("Set");

		    this.Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

		    this.IdentityGuid = DataObject.getValue<Guid>(Row["AllocationLineItemGuid"], Guid.Empty);
		    this.AllocationGuid = DataObject.getValue<Guid>(Row["AllocationGuid"], Guid.Empty);
		    this.Type = DataObject.getValue<ALLOCATION_TYPE>(Row["LookupAllocationTypeIndex"], ALLOCATION_TYPE.PRODUCT_ALLOCATION);

            // If this line item is assigned to all products, the AssignedGuid should be empty.
		    if (this.Type == ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION)
		    {
		        this.AssignedGuid = Guid.Empty;
		    }
		    else
		    {
		        this.AssignedGuid = DataObject.getValue<Guid>(Row[GetAssignedColumnName(this.Type)], Guid.Empty);
		    }

		    this.Limit.SIValue = DataObject.getValue<double>(Row["Limit"], 0.0);
		    this.Next.SIValue = DataObject.getValue<double>(Row["Next"], 0.0);
		    this.ResetPeriod = DataObject.getValue<ALLOCATION_RESET_PERIOD>(Row["LookupResetPeriodIndex"], ALLOCATION_RESET_PERIOD.DAY_RESET_PERIOD);
		    this.ResetMultiple = DataObject.getValue<int>(Row["ResetMultiple"], 1);
		    this.ResetMethod = DataObject.getValue<ALLOCATION_RESET_METHOD>(Row["LookupResetMethodIndex"], ALLOCATION_RESET_METHOD.REPEAT_METHOD);
		    this.ResetDate.Value = DataObject.getValue<DateTimeOffset>(Row["ResetDate"], TimeConverter.Today(this.ResetDate.StandardName));
		    this.CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
		    this.CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
		    this.UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this.CreatedDate);
		    this.UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			// selected column alias
		    this.AssignedID = DataObject.getValue<string>(Row["AssignedID"], "");
		}

		public void InsertSQL(SqlCommand cmd)
		{

			cmd.CommandText = "INSERT INTO tblAllocationLineItems (" +
				"AllocationGuid," +
				"LookupAllocationTypeIndex," +
				(this.Type != ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION ? GetAssignedColumnName(this.Type) + "," : string.Empty) +
				"Limit," +
				"Next," +
				"LookupResetPeriodIndex," +
				"ResetMultiple," +
				"LookupResetMethodIndex," +
				"ResetDate," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"AllocationLineItemGuid"+
				") VALUES (" +
				"@AllocationGuid," +
				"@LookupAllocationTypeIndex," +
                (this.Type != ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION ? "@AssignedGuid," : string.Empty) + 
				"@Limit," +
				"@Next," +
				"@LookupResetPeriodIndex," +
				"@ResetMultiple," +
				"@LookupResetMethodIndex," +
				"@ResetDate," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@AllocationLineItemGuid" +
				") ";

            cmd.Parameters.Add("@AllocationGuid", SqlDbType.UniqueIdentifier).Value = this.AllocationGuid;
            cmd.Parameters.Add("@LookupAllocationTypeIndex", SqlDbType.Int).Value = (int)this.Type;

		    if (this.Type != ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION)
		    {
		        cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier).Value = this.AssignedGuid;
		    }

		    cmd.Parameters.Add("@Limit", SqlDbType.Float).Value = this.Limit.SIValue;
            cmd.Parameters.Add("@Next", SqlDbType.Float).Value = this.Next.SIValue;
            cmd.Parameters.Add("@LookupResetPeriodIndex", SqlDbType.Int).Value = (int)this.ResetPeriod;
            cmd.Parameters.Add("@ResetMultiple", SqlDbType.Int).Value = this.ResetMultiple;
            cmd.Parameters.Add("@LookupResetMethodIndex", SqlDbType.Int).Value = (int)this.ResetMethod;
            cmd.Parameters.Add("@ResetDate", SqlDbType.DateTimeOffset).Value = this.ResetDate.TimeString;
            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset).Value = this.CreatedDate;
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100).Value = this.CreatedBy;
            cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset).Value = this.UpdatedDate;
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = this.UpdatedBy;
            cmd.Parameters.Add("@AllocationLineItemGuid", SqlDbType.UniqueIdentifier).Value = this._IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblAllocationLineItems " +
					"SET LookupAllocationTypeIndex = @LookupAllocationTypeIndex,";

            // Blank out any existing AssignedGuids if the line item type is "All Products"
		    if (this.Type == ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION)
		    {
		        cmd.CommandText += "AssignedApplicationStringGuid = @AssignedGuid, AssignedProductGuid = @AssignedGuid,";
		    }
		    else
		    {  
                cmd.CommandText += GetAssignedColumnName(this.Type) + " = @AssignedGuid,";	        
		    }
		  
			cmd.CommandText += "Limit = @Limit," +
					"Next = @Next," +
					"LookupResetPeriodIndex = @LookupResetPeriodIndex," +
					"ResetMultiple = @ResetMultiple," +
					"LookupResetMethodIndex = @LookupResetMethodIndex," +
					"ResetDate = @ResetDate," +
					"UpdatedDate = @UpdatedDate," +
					"UpdatedBy = @UpdatedBy " +
					"WHERE AllocationLineItemGuid = @AllocationLineItemGuid";

            cmd.Parameters.Add("@LookupAllocationTypeIndex", SqlDbType.Int).Value = (int)this.Type;
            cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier).Value = this.AssignedGuid == Guid.Empty ? DBNull.Value : (object)this.AssignedGuid;
            cmd.Parameters.Add("@Limit", SqlDbType.Float).Value = this.Limit.SIValue;
            cmd.Parameters.Add("@Next", SqlDbType.Float).Value = this.Next.SIValue;
            cmd.Parameters.Add("@LookupResetPeriodIndex", SqlDbType.Int).Value = (int)this.ResetPeriod;
            cmd.Parameters.Add("@ResetMultiple", SqlDbType.Int).Value = this.ResetMultiple;
            cmd.Parameters.Add("@LookupResetMethodIndex", SqlDbType.Int).Value = (int)this.ResetMethod;
            cmd.Parameters.Add("@ResetDate", SqlDbType.DateTimeOffset).Value = this.ResetDate.TimeString;
            cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset).Value = this.UpdatedDate;
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = this.UpdatedBy;
            cmd.Parameters.Add("@AllocationLineItemGuid", SqlDbType.UniqueIdentifier).Value = this._IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblAllocationLineItems " +
				"WHERE AllocationLineItemGuid = @AllocationLineItemGuid";

			cmd.Parameters.Add("@AllocationLineItemGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AllocationLineItemGuid"].Value = this._IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblAllocationLineItems " + SQLUpdateLock(bInTransaction) +
				" WHERE AllocationLineItemGuid = @AllocationLineItemGuid";

			cmd.Parameters.Add("@AllocationLineItemGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AllocationLineItemGuid"].Value = this._IdentityGuid;
		}

		public void SelectIdentityGuidSQL(SqlCommand cmd, bool bInTransaction)
		{
		    cmd.CommandText = "SELECT AllocationLineItemGuid" + " FROM tblAllocationLineItems "
		                      + SQLUpdateLock(bInTransaction) + " WHERE AllocationGuid = @AllocationGuid"
		                      + " AND LookupAllocationTypeIndex = @LookupAllocationTypeIndex"
		                      + " AND LookupResetPeriodIndex = @LookupResetPeriodIndex";

		    if (this.Type != ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION)
		    {
                cmd.CommandText += " AND " + GetAssignedColumnName(this.Type) + " = @AssignedGuid";
		    }

			cmd.Parameters.Add("@AllocationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LookupAllocationTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LookupResetPeriodIndex", SqlDbType.Int);

			cmd.Parameters["@AllocationGuid"].Value = this.AllocationGuid;
			cmd.Parameters["@LookupAllocationTypeIndex"].Value = (int)this.Type;
			cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
			cmd.Parameters["@LookupResetPeriodIndex"].Value = (int)this.ResetPeriod;
		}

		public void SelectByAllocationAndAssignedGuidsSQL(SqlCommand cmd)
		{
		    cmd.CommandText = this.SelectClause 
                    + " FROM tblAllocationLineItems " 
                    + " WHERE AllocationGuid = @AllocationGuid"
                    + " AND LookupAllocationTypeIndex = @LookupAllocationTypeIndex";

            if (this.Type != ALLOCATION_TYPE.ALL_PRODUCTS_ALLOCATION)
            {
                cmd.CommandText += " AND " + GetAssignedColumnName(this.Type) + " = @AssignedGuid";
            }

			cmd.Parameters.Add("@AllocationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LookupAllocationTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@AllocationGuid"].Value = this.AllocationGuid;
			cmd.Parameters["@LookupAllocationTypeIndex"].Value = (int)this.Type;
			cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
		}

		public void EnumerateByAllocationGuidSQL(SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause +
					" FROM tblAllocationLineItems " +
					" WHERE AllocationGuid = @AllocationGuid";

			cmd.Parameters.Add("@AllocationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AllocationGuid"].Value = this.AllocationGuid;
		}

		public void AmountLoadedSQL(
			SqlCommand sqlCommand,
			DateTimeOffset Beginning,
			DateTimeOffset Ending,
			string ManagerID,
			string OwnerID,
			string ShipperID,
			string BillToID,
			string ShipToID,
			Guid itemGuid,
			ALLOCATION_TYPE AllocationType,
			STATION_TYPE StationType,
			string TransactionID,
			Guid siteGuid)
		{
			sqlCommand.CommandTimeout = 0;
			sqlCommand.CommandType = CommandType.StoredProcedure;
			sqlCommand.CommandText = "usp_AllocationAmountLoaded";
			sqlCommand.Parameters.Add("@BeginDate", SqlDbType.Date);
			sqlCommand.Parameters.Add("@EndDate", SqlDbType.Date);
			sqlCommand.Parameters.Add("@ManagerID", SqlDbType.NVarChar, 100);
			sqlCommand.Parameters.Add("@OwnerID", SqlDbType.NVarChar, 100);
			sqlCommand.Parameters.Add("@ShipperID", SqlDbType.NVarChar, 100);
			sqlCommand.Parameters.Add("@BillToID", SqlDbType.NVarChar, 100);
			sqlCommand.Parameters.Add("@ShipToID", SqlDbType.NVarChar, 100);
			sqlCommand.Parameters.Add("@ItemGuid", SqlDbType.UniqueIdentifier);
			sqlCommand.Parameters.Add("@AllocationType", SqlDbType.TinyInt);
			sqlCommand.Parameters.Add("@StationType", SqlDbType.TinyInt);
			sqlCommand.Parameters.Add("@TransActionID", SqlDbType.NVarChar, 50);
			sqlCommand.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			sqlCommand.Parameters["@BeginDate"].Value = Beginning.Date;
			sqlCommand.Parameters["@EndDate"].Value = Ending.Date;
			sqlCommand.Parameters["@ManagerID"].Value = ManagerID;
			sqlCommand.Parameters["@OwnerID"].Value = OwnerID;
			sqlCommand.Parameters["@ShipperID"].Value = ShipperID;
			sqlCommand.Parameters["@BillToID"].Value = BillToID;
			sqlCommand.Parameters["@ShipToID"].Value = ShipToID;
			sqlCommand.Parameters["@ItemGuid"].Value = itemGuid;
			sqlCommand.Parameters["@AllocationType"].Value = (int)AllocationType;
			sqlCommand.Parameters["@StationType"].Value = (int)StationType;
			sqlCommand.Parameters["@TransActionID"].Value = TransactionID;
			sqlCommand.Parameters["@SiteGuid"].Value = siteGuid;

		}
	}
}
