using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class MessageCollectionClass : List<MessageClass> { }

	public enum MessageFrequencyType
	{
		Always = 0,
		OncePerDay = 1,
		Once = 2,
		MaxType = 3
	};


	public enum MessageLocationType
	{
		Gate = 0,
		LoadRack = 1,
		GateAndLoadRack = 2,
		MaxType = 3
	};

	/// <summary>
	/// Summary description for MessageClass.
	/// </summary>
   [Serializable]
   [DataContract]
	public class MessageClass : BaseDataObject, IAlarmAndEventDiscovery
	{
		#region public data members
		[DataMember]
		public MessageLocationType _LocationType;
		[DataMember]
		public MessageFrequencyType _FrequencyType;
		[DataMember]
		public Guid CompanyGuid;
		[DataMember]
		public Guid PersonnelGuid;
		[DataMember]
		public string _CompanyID;
		[DataMember]
		public string _CompanyName;
		[DataMember]
		public string _CompanyAddress;
		[DataMember]
		public string _CompanyCity;
		[DataMember]
		public string _CompanyState;
		[DataMember]
		public string _PersonID;
		[DataMember]
		public string _PersonFirstName;
		[DataMember]
		public string _PersonMiddleName;
		[DataMember]
		public string _PersonLastName;
		#endregion public data members

		#region static data members
		static string MessageLogKey = "Message Log";
		static readonly AlarmAndEventDescriptorClass MessageLogEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, MessageLogKey);
		#endregion static data members

		#region public properties
		public override string ID { get { return this._ID; } set {
		    this.SetString("Message", 120, value, ref this._ID); } }

		public string LocationType => LocationTypeID(this._LocationType);

	    public string FrequencyType => FrequencyTypeID(this._FrequencyType);

	    public string CompanyID { get { return this._CompanyID; } set {
		    this._CompanyID = value; } }

		public string CompanyName { get { return this._CompanyName; } set {
		    this._CompanyName = value; } }

		public string CompanyAddress { get { return this._CompanyAddress; } set {
		    this._CompanyAddress = value; } }

		public string CompanyCity { get { return this._CompanyCity; } set {
		    this._CompanyCity = value; } }

		public string CompanyState { get { return this._CompanyState; } set {
		    this._CompanyState = value; } }

		public string PersonID { get { return this._PersonID; } set {
		    this._PersonID = value; } }

		public string PersonFirstName { get { return this._PersonFirstName; } set {
		    this._PersonFirstName = value; } }

		public string PersonMiddlName { get { return this._PersonMiddleName; } set {
		    this._PersonMiddleName = value; } }

		public string PersonLastName { get { return this._PersonLastName; } set {
		    this._PersonLastName = value; } }
		#endregion public properties

		//string SelectClause = "SELECT a.*,C.ID AS CompanyID,C.Name AS CompanyName,C.Address1 AS CompanyAddress,C.City AS CompanyCity," +
		//	"C.State AS CompanyState,P.PersonID,P.FirstName AS PersonFirstName,P.MiddleName AS PersonMiddleName,P.LastName AS PersonLastName ";

		protected string BuildSelectClauseNotInTransaction(string whereClause)
		{
			string localWhereClause = (whereClause.Length > 0) ? " AND " + whereClause : "";
			string ret = "Select a.ID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.CompanyGuid, a.LookupFrequencyTypeIndex, a.LookupLocationTypeIndex, a.MessageGuid, a.PersonnelGuid, a.SiteGuid, C.ID AS CompanyID, C.Name AS CompanyName, C.Address1 AS CompanyAddress, C.City AS CompanyCity, C.State AS CompanyState, P.PersonID, P.FirstName AS PersonFirstName, P.MiddleName AS PersonMiddleName, P.LastName AS PersonLastName " +
			" from tblMessages a " +
			" LEFT JOIN tblCompanies C " +
			" ON a.CompanyGuid = C._MasterRecordGuid " +
			" INNER JOIN [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid) rc " +
			" ON rc.CompanyGuid = C.CompanyGuid  " +
			" LEFT JOIN tblPersonnel P " +
			" ON a.PersonnelGuid = P._MasterRecordGuid " +
			" INNER JOIN [erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) rp " +
			" ON rp.PersonnelGuid = P.PersonnelGuid " +
			" WHERE a.SiteGuid = @TargetSiteGuid " + localWhereClause +
			" Union " +
			" Select a.ID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.CompanyGuid, a.LookupFrequencyTypeIndex, a.LookupLocationTypeIndex, a.MessageGuid, a.PersonnelGuid, a.SiteGuid, C.ID AS CompanyID, C.Name AS CompanyName, C.Address1 AS CompanyAddress, C.City AS CompanyCity, C.State AS CompanyState, P.PersonID, P.FirstName AS PersonFirstName, P.MiddleName AS PersonMiddleName, P.LastName AS PersonLastName " +
			" from tblMessages a " +
			" LEFT JOIN tblCompanies C " +
			" ON a.CompanyGuid = C.CompanyGuid " +
			" LEFT JOIN tblPersonnel P " +
			" ON a.PersonnelGuid = P.PersonnelGuid " +
			" WHERE a.SiteGuid = @TargetSiteGuid AND a.CompanyGuid IS NULL and a.PersonnelGuid IS NULL " + localWhereClause +
			" Union " +
			" Select a.ID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.CompanyGuid, a.LookupFrequencyTypeIndex, a.LookupLocationTypeIndex, a.MessageGuid, a.PersonnelGuid, a.SiteGuid, C.ID AS CompanyID, C.Name AS CompanyName, C.Address1 AS CompanyAddress, C.City AS CompanyCity, C.State AS CompanyState, P.PersonID, P.FirstName AS PersonFirstName, P.MiddleName AS PersonMiddleName, P.LastName AS PersonLastName " +
			" from tblMessages a " +
			" LEFT JOIN tblCompanies C " +
			" ON a.CompanyGuid = C._MasterRecordGuid " +
			" LEFT JOIN tblPersonnel P " +
			" ON a.PersonnelGuid = P._MasterRecordGuid " +
			" INNER JOIN [erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) rp " +
			" ON rp.PersonnelGuid = P.PersonnelGuid " +
			" WHERE a.SiteGuid = @TargetSiteGuid AND a.CompanyGuid IS NULL " + localWhereClause +
			" Union " +
			" Select a.ID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.CompanyGuid, a.LookupFrequencyTypeIndex, a.LookupLocationTypeIndex, a.MessageGuid, a.PersonnelGuid, a.SiteGuid, C.ID AS CompanyID, C.Name AS CompanyName, C.Address1 AS CompanyAddress, C.City AS CompanyCity, C.State AS CompanyState, P.PersonID, P.FirstName AS PersonFirstName, P.MiddleName AS PersonMiddleName, P.LastName AS PersonLastName " +
			" from tblMessages a " +
			" LEFT JOIN tblCompanies C " +
			" ON a.CompanyGuid = C._MasterRecordGuid " +
			" INNER JOIN [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid) rc " +
			" ON rc.CompanyGuid = C.CompanyGuid  " +
			" LEFT JOIN tblPersonnel P " +
			" ON a.PersonnelGuid = P._MasterRecordGuid " +
			" WHERE a.SiteGuid = @TargetSiteGuid AND a.PersonnelGuid IS NULL " + localWhereClause;
			return ret;
		}

		protected string BuildSelectClauseInTransaction(string whereClause)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
            string localWhereClause = (whereClause.Length > 0) ? " AND " + whereClause : "";
			string ret = "Select a.ID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.CompanyGuid, a.LookupFrequencyTypeIndex, a.LookupLocationTypeIndex, a.MessageGuid, a.PersonnelGuid, a.SiteGuid, C.ID AS CompanyID, C.Name AS CompanyName, C.Address1 AS CompanyAddress, C.City AS CompanyCity, C.State AS CompanyState, P.PersonID, P.FirstName AS PersonFirstName, P.MiddleName AS PersonMiddleName, P.LastName AS PersonLastName " +
			" from tblMessages a " +
			" LEFT JOIN tblCompanies C " + 
			" ON a.CompanyGuid = C._MasterRecordGuid " +
			" INNER JOIN [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid) rc " +
			" ON rc.CompanyGuid = C.CompanyGuid  " +
			" LEFT JOIN tblPersonnel P " + 
			" ON a.PersonnelGuid = P._MasterRecordGuid " +
			" INNER JOIN [erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) rp " +
			" ON rp.PersonnelGuid = P.PersonnelGuid " +
			" WHERE a.SiteGuid = @TargetSiteGuid " + localWhereClause +
			" Union " +
			" Select a.ID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.CompanyGuid, a.LookupFrequencyTypeIndex, a.LookupLocationTypeIndex, a.MessageGuid, a.PersonnelGuid, a.SiteGuid, C.ID AS CompanyID, C.Name AS CompanyName, C.Address1 AS CompanyAddress, C.City AS CompanyCity, C.State AS CompanyState, P.PersonID, P.FirstName AS PersonFirstName, P.MiddleName AS PersonMiddleName, P.LastName AS PersonLastName " +
			" from tblMessages a " +
			" LEFT JOIN tblCompanies C " + 
			" ON a.CompanyGuid = C.CompanyGuid " +
			" LEFT JOIN tblPersonnel P " + 
			" ON a.PersonnelGuid = P.PersonnelGuid " +
			" WHERE a.SiteGuid = @TargetSiteGuid AND a.CompanyGuid IS NULL and a.PersonnelGuid IS NULL " + localWhereClause +
			" Union " +
			" Select a.ID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.CompanyGuid, a.LookupFrequencyTypeIndex, a.LookupLocationTypeIndex, a.MessageGuid, a.PersonnelGuid, a.SiteGuid, C.ID AS CompanyID, C.Name AS CompanyName, C.Address1 AS CompanyAddress, C.City AS CompanyCity, C.State AS CompanyState, P.PersonID, P.FirstName AS PersonFirstName, P.MiddleName AS PersonMiddleName, P.LastName AS PersonLastName " +
			" from tblMessages a " +
			" LEFT JOIN tblCompanies C " + 
			" ON a.CompanyGuid = C._MasterRecordGuid " +
            " LEFT JOIN tblPersonnel P " + 
			" ON a.PersonnelGuid = P._MasterRecordGuid " +
			" INNER JOIN [erv].[udf_GetPersonnelRecordVersions] (@TargetSiteGuid) rp " +
			" ON rp.PersonnelGuid = P.PersonnelGuid " +
			" WHERE a.SiteGuid = @TargetSiteGuid AND a.CompanyGuid IS NULL " + localWhereClause +
			" Union " +
			" Select a.ID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.CompanyGuid, a.LookupFrequencyTypeIndex, a.LookupLocationTypeIndex, a.MessageGuid, a.PersonnelGuid, a.SiteGuid, C.ID AS CompanyID, C.Name AS CompanyName, C.Address1 AS CompanyAddress, C.City AS CompanyCity, C.State AS CompanyState, P.PersonID, P.FirstName AS PersonFirstName, P.MiddleName AS PersonMiddleName, P.LastName AS PersonLastName " +
			" from tblMessages a " +
            " LEFT JOIN tblCompanies C " + 
			" ON a.CompanyGuid = C._MasterRecordGuid " +
			" INNER JOIN [erv].[udf_GetCompanyRecordVersions] (@TargetSiteGuid) rc " +
			" ON rc.CompanyGuid = C.CompanyGuid  " +
            " LEFT JOIN tblPersonnel P " + 
			" ON a.PersonnelGuid = P._MasterRecordGuid " +
			" WHERE a.SiteGuid = @TargetSiteGuid AND a.PersonnelGuid IS NULL "  + localWhereClause;
			return ret;
		}

		#region Constructors
		public MessageClass()
		{
		    this.Reset();
		}
		#endregion Constructors

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType => ENTITY_TYPE.MESSAGE;

	    [XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;


		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] descriptors ={	MessageLogEventDescriptor
																	};

				return descriptors;
			}
		}

		public AlarmAndEventLogClass MessageLogEvent(string companyID, string personID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(MessageLogEventDescriptor)
		                                             {
		                                                 AssociatedData = companyID + ", " + personID + ", " + this.ID
		                                             };
		    return alarmAndEventLog;
		}

		public override void Reset()
		{
			base.Reset();
		    this._LocationType = MessageLocationType.GateAndLoadRack;
		    this._FrequencyType = MessageFrequencyType.Once;
		    this.CompanyGuid = Guid.Empty;
		    this.PersonnelGuid = Guid.Empty;
		    this._CompanyID = "";
		    this._CompanyName = "";
		    this._CompanyCity = "";
		    this._CompanyState = "";
		    this._PersonID = "";
		    this._PersonFirstName = "";
		    this._PersonMiddleName = "";
		    this._PersonLastName = "";
		}

		public static string LocationTypeID(MessageLocationType type)
		{
			switch (type)
			{
				case MessageLocationType.Gate:
					return "Gate";
				case MessageLocationType.GateAndLoadRack:
					return "Gate & Rack";
				case MessageLocationType.LoadRack:
					return "Rack";
				default:
					return "Undefined";
			}
		}

		public static string FrequencyTypeID(MessageFrequencyType type)
		{
			switch (type)
			{
				case MessageFrequencyType.Always:
					return "Always";
				case MessageFrequencyType.Once:
					return "Once";
				case MessageFrequencyType.OncePerDay:
					return "Once Per Day";
				default:
					return "Undefined";
			}
		}

		[DataMember]
		public string CompanyToolTip
		{
			get
			{
				string toolTip = "";
				if (this._CompanyName != "")
					toolTip = this._CompanyName;
				if (this._CompanyAddress != "")
					toolTip += ", " + this._CompanyAddress;
				if (this._CompanyCity != "")
					toolTip += ", " + this._CompanyCity;
				if (this._CompanyState != "")
					toolTip += ", " + this._CompanyState;
				return toolTip;
			}
			private set { ;}
		}

		[DataMember]
		public string PersonToolTip
		{
			get
			{
				string toolTip = "";
				if (this._PersonFirstName != "")
					toolTip = this._PersonFirstName;
				if (this._PersonMiddleName != "")
					toolTip += " " + this._PersonMiddleName;
				if (this._PersonLastName != "")
					toolTip += " " + this._PersonLastName;
				return toolTip;
			}
			private set { ;}
		}

		public void Load(DataSet set)
		{
			if (set == null)
				throw new ArgumentNullException(nameof(set));

		    this.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
				return;

			DataRow row = table.Rows[0];

		    this._IdentityGuid = DataObject.getValue<Guid>(row["MessageGuid"], Guid.Empty);
		    this._SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
		    this._ID = DataObject.getValue<string>(row["ID"], "");
		    this._LocationType = DataObject.getValue<MessageLocationType>(row["LookupLocationTypeIndex"], MessageLocationType.GateAndLoadRack);
		    this._FrequencyType = DataObject.getValue<MessageFrequencyType>(row["LookupFrequencyTypeIndex"], MessageFrequencyType.Once);
		    this.CompanyGuid = DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty);
		    this.PersonnelGuid = DataObject.getValue<Guid>(row["PersonnelGuid"], Guid.Empty);
		    this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
		    this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
		    this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
		    this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
		    this._CompanyID = DataObject.getValue<string>(row["CompanyID"], "{All}");
		    this._CompanyName = DataObject.getValue<string>(row["CompanyName"], "");
		    this._CompanyAddress = DataObject.getValue<string>(row["CompanyAddress"], "");
		    this._CompanyCity = DataObject.getValue<string>(row["CompanyCity"], "");
		    this._CompanyState = DataObject.getValue<string>(row["CompanyState"], "");
		    this._PersonID = DataObject.getValue<string>(row["PersonID"], "{All}");
		    this._PersonFirstName = DataObject.getValue<string>(row["PersonFirstName"], "");
		    this._PersonMiddleName = DataObject.getValue<string>(row["PersonMiddleName"], "");
		    this._PersonLastName = DataObject.getValue<string>(row["PersonLastName"], "");
		}


		#region SqlCommands with Parameters

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblMessages ( " +
					" SiteGuid, " +
					" ID, " +
					" LookupLocationTypeIndex, " +
					" LookupFrequencyTypeIndex, " +
					" CompanyGuid, " +
					" PersonnelGuid, " +
					" CreatedDate, " +
					" CreatedBy, " +
					" UpdatedDate, " +
					" UpdatedBy, " +
					" MessageGuid" +
					") VALUES (" +
					"@SiteGuid, " +
					"@ID, " +
					"@LookupLocationTypeIndex, " +
					"@LookupFrequencyTypeIndex, " +
					"@CompanyGuid, " +
					"@PersonnelGuid, " +
					"@CreatedDate, " +
					"@CreatedBy, " +
					"@UpdatedDate, " +
					"@UpdatedBy, " +
					"@MessageGuid)";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@LookupLocationTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LookupFrequencyTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PersonnelGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@LookupLocationTypeIndex"].Value = this._LocationType;
			cmd.Parameters["@LookupFrequencyTypeIndex"].Value = this._FrequencyType;
			
			if (this.CompanyGuid != Guid.Empty)
				{cmd.Parameters["@CompanyGuid"].Value = this.CompanyGuid;}
			else
				{cmd.Parameters["@CompanyGuid"].Value = DBNull.Value;}
	
			if (this.PersonnelGuid != Guid.Empty )
				{cmd.Parameters["@PersonnelGuid"].Value = this.PersonnelGuid;}
			else
				{cmd.Parameters["@PersonnelGuid"].Value = DBNull.Value;}

			cmd.Parameters["@CreatedDate"].Value = this._CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this._CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
			cmd.Parameters["@MessageGuid"].Value = this._IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText= "UPDATE tblMessages " +
				"SET ID = @ID, " +
				"LookupLocationTypeIndex = @LookupLocationTypeIndex, " + 
				"LookupFrequencyTypeIndex = @LookupFrequencyTypeIndex, " + 
				"CompanyGuid = @CompanyGuid, " + 
				"PersonnelGuid = @PersonnelGuid, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy " +
				" WHERE MessageGuid = @MessageGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@LookupLocationTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@LookupFrequencyTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PersonnelGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@LookupLocationTypeIndex"].Value = this._LocationType;
			cmd.Parameters["@LookupFrequencyTypeIndex"].Value = this._FrequencyType;

			if (this.CompanyGuid != Guid.Empty)
			{ cmd.Parameters["@CompanyGuid"].Value = this.CompanyGuid; }
			else
			{ cmd.Parameters["@CompanyGuid"].Value = DBNull.Value; }

			if (this.PersonnelGuid != Guid.Empty)
			{ cmd.Parameters["@PersonnelGuid"].Value = this.PersonnelGuid; }
			else
			{ cmd.Parameters["@PersonnelGuid"].Value = DBNull.Value; }

			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
			cmd.Parameters["@MessageGuid"].Value = this._IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblMessages WHERE MessageGuid = @MessageGuid";
			cmd.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MessageGuid"].Value = this._IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			string whereClause = "a.MessageGuid = @MessageGuid";
			if (bInTransaction)
			{
				cmd.CommandText = this.BuildSelectClauseInTransaction(whereClause);
			}
			else
			{
				cmd.CommandText = this.BuildSelectClauseNotInTransaction(whereClause);
			}
			cmd.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MessageGuid"].Value = this._IdentityGuid;
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = this._SiteGuid;

		}

		public void SelectByIDAndGuidsSQL(SqlCommand cmd, bool bInTransaction)
		{
			string whereClause = "a.ID = @ID";
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 120);
			cmd.Parameters["@ID"].Value = this.ID;
			if (this.CompanyGuid != Guid.Empty)
			{
				whereClause += " AND a.CompanyGuid = @CompanyGuid ";
				cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CompanyGuid"].Value = this.CompanyGuid;
			}
			if (this.PersonnelGuid != Guid.Empty)
			{
				whereClause += " AND a.PersonnelGuid = @PersonnelGuid ";
				cmd.Parameters.Add("@PersonnelGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@PersonnelGuid"].Value = this.PersonnelGuid;
			}
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = this._SiteGuid;

			if (bInTransaction)
			{
				cmd.CommandText = this.BuildSelectClauseInTransaction(whereClause);
			}
			else
			{
				cmd.CommandText = this.BuildSelectClauseNotInTransaction(whereClause);
			}

		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			string localSelectCaluse = this.BuildSelectClauseNotInTransaction("");

			cmd.CommandText = localSelectCaluse +
				" ORDER BY C.ID,P.PersonID,a.ID";

			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		public void EnumerateByCompanySQL(SqlCommand cmd, SecurityClass security)
		{
			string whereClause = (this.CompanyGuid != Guid.Empty) ? "a.CompanyGuid = @CompanyGuid" : "";
			string localSelectClause = this.BuildSelectClauseNotInTransaction(whereClause);

			cmd.CommandText = localSelectClause +
				" ORDER BY C.ID,P.PersonID,a.ID";

			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
			cmd.Parameters["@CompanyGuid"].Value = this.CompanyGuid; 
		}

		public void EnumerateByGuidsSQL(SqlCommand cmd, SecurityClass security)
		{

			string whereClause = (this.CompanyGuid != Guid.Empty) ? "(a.CompanyGuid = @CompanyGuid or a.CompanyGuid IS NULL)" : "";
			if (this.PersonnelGuid != Guid.Empty)
			{
				if (whereClause.Length > 0)
				{
					whereClause += " AND ";
				}
				whereClause += "(a.PersonnelGuid = @PersonnelGuid or a.PersonnelGuid IS NULL)";
			}
			string localSelectClause = this.BuildSelectClauseNotInTransaction(whereClause);

			cmd.CommandText = localSelectClause +
				" ORDER BY C.ID,P.PersonID,a.ID";


			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;

			if (this.CompanyGuid != Guid.Empty)
				{cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@CompanyGuid"].Value = this.CompanyGuid;}
			
			if (this.PersonnelGuid != Guid.Empty)
				{cmd.Parameters.Add("@PersonnelGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@PersonnelGuid"].Value = this.PersonnelGuid;}
		}
		#endregion
	}
}
