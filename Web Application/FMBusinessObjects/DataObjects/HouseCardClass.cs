using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
//using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using FMBusinessObjects.Interfaces;
using FMBusinessObjects.UtilityObjects;


namespace FMBusinessObjects.DataObjects
{

   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(HouseCardClass))]
	public class HouseCardCollectionClass : List<HouseCardClass> { }

	[DataContract]
   [Serializable]
	public class HouseCardClass : BaseDataObject, IAlarmAndEventDiscovery
	{
		#region constants
		const string PARAM_NAME_SITEGUID = "@SiteGuid";
		const string PARAM_NAME_SITEGUID_WHERE = "@WhereSiteGuid";
		const SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_HOUSECARDGUID = "@HouseCardGuid";
		const SqlDbType PARAM_TYPE_HOUSECARDGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_DRIVERGUID = "@DriverGuid";
		const SqlDbType PARAM_TYPE_DRIVERGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_ID = "@ID";
		const string PARAM_NAME_ID_WHERE = "@WhereID";
		const SqlDbType PARAM_TYPE_ID = SqlDbType.NVarChar;
		const int PARAM_SIZE_ID = 100;
		const string PARAM_NAME_NUMBER = "@NUMBER";
		const SqlDbType PARAM_TYPE_NUMBER = SqlDbType.NVarChar;
		const int PARAM_SIZE_NUMBER = 100;
		const string PARAM_NAME_CREATEDDATE = "@CreatedDate";
		const SqlDbType PARAM_TYPE_CREATEDDATE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_CREATEDBY = "@CreatedBy";
		const SqlDbType PARAM_TYPE_CREATEDBY = SqlDbType.NVarChar;
		const int PARAM_SIZE_CREATEDBY = 100;
		const string PARAM_NAME_UPDATEDDATE = "@UpdatedDate";
		const SqlDbType PARAM_TYPE_UPDATEDDATE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_UPDATEDBY = "@UpdatedBy";
		const SqlDbType PARAM_TYPE_UPDATEDBY = SqlDbType.NVarChar;
		const int PARAM_SIZE_UPDATEDBY = 100;
		#endregion constants
		#region Private data members
		[DataMember]
		private string _Number;
		[DataMember]
		private Guid _DriverGuid;
		private string SelectClause = "Select tblHouseCards.*," +
									  "(Select PersonID FROM tblPersonnel WHERE tblPersonnel.PersonnelGuid = tblHouseCards.DriverPersonnelGuid) AS DriverID ";
		#endregion Private data members

		#region static data memebers
		static string AssignedKey = "House Card Assigned";
		static string UnassignedKey = "House Card Unassigned";
		static AlarmAndEventDescriptorClass AssignedEventDescriptor = new AlarmAndEventDescriptorClass(false, SystemKey, AssignedKey);
		static AlarmAndEventDescriptorClass UnassignedEventDescriptor = new AlarmAndEventDescriptorClass(false, SystemKey, UnassignedKey);
		#endregion static data memebers

		#region Public data members

		[DataMember]
		public string DriverID;

		#endregion Public data members


		#region Public properties
		public override string ID { get { return _ID; } set { SetString("ID", 50, value, ref _ID); } }
		public string Number { get { return _Number; } set { SetString("Number", 50, value, ref _Number); } }
		public Guid DriverGuid { get { return _DriverGuid; } set { _DriverGuid = value; } }
		#endregion Public properties


		#region Constructors
		public HouseCardClass()
		{
			Reset();
		}
		#endregion Constructors



		#region Alarm and event descriptor
		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] Descriptors ={	AssignedEventDescriptor,
																		UnassignedEventDescriptor
																	};
				return Descriptors;
			}
		}
		#endregion Alarm and event descriptor

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.HOUSE_CARD;
			}

		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		public AlarmAndEventLogClass Assigned(string DriverID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(AssignedEventDescriptor);
			AlarmAndEventLog.AssociatedData = "Card : " + ID + ",Driver : " + DriverID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass Unassigned(string DriverID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(UnassignedEventDescriptor);
			AlarmAndEventLog.AssociatedData = "Card : " + ID + ",Driver : " + DriverID;
			return AlarmAndEventLog;
		}

		public override void Reset()
		{
			base.Reset();
			_Number = "";
			_DriverGuid = Guid.Empty;
			DriverID = "{None}";
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
				throw new ArgumentNullException("Set");

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
				return;

			DataRow Row = Table.Rows[0];

			_IdentityGuid = DataObject.getValue<Guid>(Row["HouseCardGuid"], Guid.Empty);
			_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			_ID = DataObject.getValue<string>(Row["ID"], "");
			_Number = DataObject.getValue<string>(Row["Number"], "");
			_DriverGuid = DataObject.getValue<Guid>(Row["DriverPersonnelGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			DriverID = DataObject.getValue<string>(Row["DriverID"], "{None}");
		}

		public void InsertSQL(SqlCommand cmd)
		{

			cmd.CommandText = "INSERT INTO tblHouseCards " +
				"(SiteGuid," +
				"ID," +
				"Number," +
				"DriverPersonnelGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"HouseCardGuid" +
				") VALUES (" +
					DataObject.AddParameter(cmd, string.Empty, PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, _SiteGuid) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_ID, PARAM_TYPE_ID, _ID) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_NUMBER, PARAM_TYPE_NUMBER, _Number) +
					DataObject.AddGuidParameter(cmd, ",", PARAM_NAME_DRIVERGUID, _DriverGuid, true) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_CREATEDDATE, PARAM_TYPE_CREATEDDATE, _CreatedDate) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_CREATEDBY, PARAM_TYPE_CREATEDBY, PARAM_SIZE_CREATEDBY, _CreatedBy) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_UPDATEDDATE, PARAM_TYPE_UPDATEDDATE, _UpdatedDate) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_UPDATEDBY, PARAM_TYPE_UPDATEDBY, PARAM_SIZE_UPDATEDBY, _UpdatedBy) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_HOUSECARDGUID, PARAM_TYPE_HOUSECARDGUID, _IdentityGuid) +
				")";
		}

		public SqlCommand UpdateSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "UPDATE tblHouseCards " +
					  "SET " +
						DataObject.AddParameter(cmd, false, "[ID]", PARAM_NAME_ID, PARAM_TYPE_ID, _ID) + "," +
						DataObject.AddParameter(cmd, false, "Number", PARAM_NAME_NUMBER, PARAM_TYPE_NUMBER, _Number) + "," +
						DataObject.AddGuidParameter(cmd, "DriverPersonnelGuid = ", PARAM_NAME_DRIVERGUID, _DriverGuid, true) + "," +
						DataObject.AddParameter(cmd, false, "UpdatedDate", PARAM_NAME_UPDATEDDATE, PARAM_TYPE_UPDATEDDATE, _UpdatedDate) + "," +
						DataObject.AddParameter(cmd, false, "UpdatedBy", PARAM_NAME_UPDATEDBY, PARAM_TYPE_UPDATEDBY, PARAM_SIZE_UPDATEDBY, _UpdatedBy) +
					  "WHERE " +
					  DataObject.AddParameter(cmd, false, "HouseCardGuid", PARAM_NAME_HOUSECARDGUID, PARAM_TYPE_HOUSECARDGUID, _IdentityGuid);

				return cmd;
			}
		}

		public SqlCommand PurgeSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "DELETE FROM tblHouseCards WHERE " +
										DataObject.AddParameter(cmd, false, "HouseCardGuid", PARAM_NAME_HOUSECARDGUID, PARAM_TYPE_HOUSECARDGUID, _IdentityGuid);
				return cmd;
			}
		}

		public SqlCommand SelectSQL(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = SelectClause +
				  " FROM tblHouseCards " + SQLUpdateLock(bInTransaction) +
				  " WHERE " +
				  DataObject.AddParameter(cmd, false, "HouseCardGuid", PARAM_NAME_HOUSECARDGUID, PARAM_TYPE_HOUSECARDGUID, _IdentityGuid);

			return cmd;
		}

		public SqlCommand SelectByIDSQL(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = SelectClause +
				  " FROM tblHouseCards " + SQLUpdateLock(bInTransaction) +
					" WHERE " +
					DataObject.AddParameter(cmd, false, "SiteGuid", PARAM_NAME_SITEGUID_WHERE, PARAM_TYPE_SITEGUID, _SiteGuid) +
					DataObject.AddParameter(cmd, true, "[ID]", PARAM_NAME_ID_WHERE, PARAM_TYPE_ID, _ID);

			return cmd;
		}

		public SqlCommand SelectByNumberSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = SelectClause +
					" FROM tblHouseCards" +
					" WHERE " +
					DataObject.AddParameter(cmd, false, "SiteGuid", PARAM_NAME_SITEGUID_WHERE, PARAM_TYPE_SITEGUID, _SiteGuid) +
					DataObject.AddParameter(cmd, true, "Number", PARAM_NAME_NUMBER, PARAM_TYPE_NUMBER, _Number);

				return cmd;
			}
		}

		public SqlCommand SelectByDriverGuidSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = SelectClause +
					" FROM tblHouseCards" +
					" WHERE " +
					DataObject.AddParameter(cmd, false, "DriverPersonnelGuid", PARAM_NAME_DRIVERGUID, PARAM_TYPE_DRIVERGUID, _DriverGuid);

				return cmd;
			}
		}

		public SqlCommand EnumerateSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = SelectClause +
						" FROM tblHouseCards" +
						" WHERE " +
						DataObject.AddParameter(cmd, false, "SiteGuid", PARAM_NAME_SITEGUID_WHERE, PARAM_TYPE_SITEGUID, _SiteGuid) +
						" ORDER BY ID";

				return cmd;
			}
		}

	}
}
