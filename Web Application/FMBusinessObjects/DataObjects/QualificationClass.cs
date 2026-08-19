using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	public enum QUALIFICATION_TYPE
	{
		COMPANY_CERTIFICATE_AND_PERMIT = 0,
		EQUIPMENT_TEST_AND_INSPECTION = 1,
		EQUIPMENT_TAG_AND_LICENSE = 2,
		PERSON_QUALIFICATION = 3,
		PERSON_LICENSE = 4,
		PERSON_TRAINING = 5,
		MAX_QUALIFICATION_TYPE = 6
	};

   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(QualificationClass))]
	public class QualificationCollectionClass : List<QualificationClass> { }

	/// <summary>
	/// Summary description for QualificationClass.
	/// </summary>
   [Serializable]
   [DataContract]
	public class QualificationClass : BaseDataObject
	{
		[DataMember]
		public QUALIFICATION_TYPE Type;
		[DataMember]
		public string _Description;
		[DataMember]
		public int _Duration = 0;
		[DataMember]
		public int _Reoccurrence = 0;

		public override string ID { get { return _ID; } set { SetString("ID", 30, value, ref _ID); } }
		public string Description { get { return _Description; } set { SetString("Description", 255, value, ref _Description); } }
		public int Duration { get { return _Duration; } set { _Duration = value; } }
		public int Reoccurrence { get { return _Reoccurrence; } set { _Reoccurrence = value; } }


		#region SQL Command constants
		const string PARAM_NAME_ID = "@ID";
		const string PARAM_NAME_ID_WHERE = "@WhereID";
		const SqlDbType PARAM_TYPE_ID = SqlDbType.NVarChar;
		const int PARAM_SIZE_ID = 80;
		const string PARAM_NAME_DESCRIPTION = "@Description";
		const SqlDbType PARAM_TYPE_DESCRIPTION = SqlDbType.NVarChar;
		const int PARAM_SIZE_DESCRIPTION = 255;
		const string PARAM_NAME_SITEGUID = "@SiteGuid";
		const SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_DURATION = "@Duration";
		const SqlDbType PARAM_TYPE_DURATION = SqlDbType.Int;
		const string PARAM_NAME_REOCCURRENCE = "@Reoccurrence";
		const SqlDbType PARAM_TYPE_REOCCURRENCE = SqlDbType.Int;
		const string PARAM_NAME_UPDATEDDATE = "@UpdatedDate";
		const SqlDbType PARAM_TYPE_UPDATEDDATE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_UPDATEDBY = "@UpdatedBy";
		const SqlDbType PARAM_TYPE_UPDATEDBY = SqlDbType.NVarChar;
		const int PARAM_SIZE_UPDATEDBY = 100;
		const string PARAM_NAME_TYPE_WHERE = "@WhereType";
		const SqlDbType PARAM_TYPE_TYPE = SqlDbType.Int;
		#endregion

		public QualificationClass()
		{
			Reset();
		}

		public override ENTITY_TYPE EntityType
		{
			get
			{
				switch (Type)
				{
					case QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT:
						return ENTITY_TYPE.QUALIFICATION_COMPANY_CERTIFICATE_AND_PERMIT;
					case QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION:
						return ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TEST_AND_INSPECTION;
					case QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE:
						return ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE;
					case QUALIFICATION_TYPE.PERSON_QUALIFICATION:
						return ENTITY_TYPE.QUALIFICATION_PERSON_QUALIFICATION;
					case QUALIFICATION_TYPE.PERSON_LICENSE:
						return ENTITY_TYPE.QUALIFICATION_PERSON_LICENSE;
					case QUALIFICATION_TYPE.PERSON_TRAINING:
						return ENTITY_TYPE.QUALIFICATION_PERSON_TRAINING;
					default:
						return ENTITY_TYPE.NONE;
				}
			}
			set { }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public static string TypeID(QUALIFICATION_TYPE Type)
		{
			switch (Type)
			{
				case QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT:
					return "Certificate and Permit";
				case QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION:
					return "Test and Inspection";
				case QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE:
					return "Tag and License";
				case QUALIFICATION_TYPE.PERSON_QUALIFICATION:
					return "Qualification";
				case QUALIFICATION_TYPE.PERSON_LICENSE:
					return "License";
				case QUALIFICATION_TYPE.PERSON_TRAINING:
					return "Training";
				default:
					return "Undefined";
			}
		}

		public override void Reset()
		{
			base.Reset();
			Type = QUALIFICATION_TYPE.MAX_QUALIFICATION_TYPE;
			Description = "";
			Duration = 0;
			Reoccurrence = 0;
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

			IdentityGuid = DataObject.getValue<Guid>(Row["QualificationGuid"], Guid.Empty);
			SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			Type = DataObject.getValue<QUALIFICATION_TYPE>(Row["LookupQualificationTypeIndex"], QUALIFICATION_TYPE.MAX_QUALIFICATION_TYPE);
			_ID = DataObject.getValue<string>(Row["ID"], "");
			_Description = DataObject.getValue<string>(Row["Description"], "");
			_Duration = DataObject.getValue<int>(Row["Duration"], 0);
			_Reoccurrence = DataObject.getValue<int>(Row["Reoccurrence"], 0);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			const string PARAM_NAME_CREATEDDATE = "@CreatedDate";
			const SqlDbType PARAM_TYPE_CREATEDDATE = SqlDbType.DateTimeOffset;
			const string PARAM_NAME_CREATEDBY = "@CreatedBy";
			const SqlDbType PARAM_TYPE_CREATEDBY = SqlDbType.NVarChar;
			const int PARAM_SIZE_CREATEDBY = 100;
			const string PARAM_NAME_LOOKUPTUPE = "@lookupType";
			const SqlDbType PARAM_TYPE_LOOKUPTYPE = SqlDbType.Int;

			cmd.CommandText = "INSERT INTO tblQualifications " +
				"(" +
				"ID," +
				"Description," +
				"SiteGuid," +
				"Duration," +
				"Reoccurrence," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"LookupQualificationTypeIndex," +
				"QualificationGuid" +
				") VALUES (" +
					DataObject.AddParameter(cmd, string.Empty, PARAM_NAME_ID, PARAM_TYPE_ID, PARAM_SIZE_ID, _ID) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_DESCRIPTION, PARAM_TYPE_DESCRIPTION, PARAM_SIZE_DESCRIPTION, _Description) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, _SiteGuid) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_DURATION, PARAM_TYPE_DURATION, _Duration) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_REOCCURRENCE, PARAM_TYPE_REOCCURRENCE, _Reoccurrence) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_CREATEDDATE, PARAM_TYPE_CREATEDDATE, _CreatedDate) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_CREATEDBY, PARAM_TYPE_CREATEDBY, PARAM_SIZE_CREATEDBY, _CreatedBy) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_UPDATEDDATE, PARAM_TYPE_UPDATEDDATE, _UpdatedDate) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_UPDATEDBY, PARAM_TYPE_UPDATEDBY, PARAM_SIZE_UPDATEDBY, _UpdatedBy) +
					DataObject.AddParameter(cmd, ",", PARAM_NAME_LOOKUPTUPE, PARAM_TYPE_LOOKUPTYPE, (int)this.Type) +
					DataObject.AddParameter(cmd, ",", "@QualificationGuid", SqlDbType.UniqueIdentifier, _IdentityGuid) +
				")";

		}

		private static string AddWhereGuid(SqlCommand cmd, Guid targetGuid)
		{
			return " WHERE " + DataObject.AddParameter(cmd, false, "QualificationGuid", "@WhereQualificationGuid", SqlDbType.UniqueIdentifier, targetGuid);
		}

		public SqlCommand UpdateSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "UPDATE tblQualifications " +
					"SET " +
					DataObject.AddParameter(cmd, false, "ID", PARAM_NAME_ID, PARAM_TYPE_ID, PARAM_SIZE_ID, _ID) + "," +
					DataObject.AddParameter(cmd, false, "Description", PARAM_NAME_DESCRIPTION, PARAM_TYPE_DESCRIPTION, PARAM_SIZE_DESCRIPTION, _Description) + "," +
					DataObject.AddParameter(cmd, false, "SiteGuid", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, _SiteGuid) + "," +
					DataObject.AddParameter(cmd, false, "Duration", PARAM_NAME_DURATION, PARAM_TYPE_DURATION, _Duration) + "," +
					DataObject.AddParameter(cmd, false, "Reoccurrence", PARAM_NAME_REOCCURRENCE, PARAM_TYPE_REOCCURRENCE, _Reoccurrence) + "," +
					DataObject.AddParameter(cmd, false, "UpdatedDate", PARAM_NAME_UPDATEDDATE, PARAM_TYPE_UPDATEDDATE, _UpdatedDate) + "," +
					DataObject.AddParameter(cmd, false, "UpdatedBy", PARAM_NAME_UPDATEDBY, PARAM_TYPE_UPDATEDBY, PARAM_SIZE_UPDATEDBY, _UpdatedBy) +
					AddWhereGuid(cmd, _IdentityGuid);

				return cmd;
			}
		}


		public SqlCommand PurgeSQL
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "DELETE FROM tblQualifications " + AddWhereGuid(cmd, _IdentityGuid);
				return cmd;
			}
		}


		public SqlCommand SelectSQL(bool bInTransaction)
		{

			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT * FROM tblQualifications " + SQLUpdateLock(bInTransaction) + AddWhereGuid(cmd, _IdentityGuid);
			return cmd;
		}

		public SqlCommand SelectByIDAndTypeSQL(SecurityClass security, bool bInTransaction)
		{

			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT * FROM tblQualifications " + SQLUpdateLock(bInTransaction) +
					" WHERE" + AppendSiteWhereClause(cmd, security, "tblQualifications", "QualificationGuid") +
					DataObject.AddParameter(cmd, true, "ID", PARAM_NAME_ID_WHERE, PARAM_TYPE_ID, PARAM_SIZE_ID, _ID) +
					DataObject.AddParameter(cmd, true, "LookupQualificationTypeIndex", PARAM_NAME_TYPE_WHERE, PARAM_TYPE_TYPE, (int)Type);

			return cmd;
		}


		public SqlCommand EnumerateByTypeSQL(SecurityClass security)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT * FROM tblQualifications" +
					" WHERE" + AppendSiteWhereClause(cmd, security, "tblQualifications", "QualificationGuid") +
					DataObject.AddParameter(cmd, true, "LookupQualificationTypeIndex", PARAM_NAME_TYPE_WHERE, PARAM_TYPE_TYPE, (int)Type);
			;

			return cmd;
		}
	}
}
