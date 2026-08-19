using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	 // Modifications on enum PERSON_ROLE may require update on table lookup.tblPersonnelRole
	public enum PERSON_ROLE
	{
		LOADER_ROLE = 0,
		SUPERVISOR_ROLE = 1,
        OFFLOADER_ROLE = 2,
		MAX_PERSON_ROLE = 3
	};

   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(PersonRoleMapClass))]
	public class PersonRoleMapCollectionClass : List<PersonRoleMapClass> { }

   [Serializable]
   [DataContract]
	public class PersonRoleMapClass : BaseDataObject
	{
		[DataMember]
		public Guid PersonGuid { get; set; }

		[DataMember]
		public PERSON_ROLE Role { get; set; }

		public override void Reset ( )
		{
			base.Reset ( );
			PersonGuid = Guid.Empty;
		}

		public void Load ( DataSet Set )
		{
			if (Set == null)
				throw new ArgumentNullException ( "Set" );

			Reset ( );

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
				return;

			DataRow Row = Table.Rows[0];

			PersonGuid = DataObject.getValue<Guid>(Row["PersonnelGuid"], Guid.Empty);
			Role = DataObject.getValue<PERSON_ROLE>(Row["LookupPersonnelRoleIndex"], PERSON_ROLE.LOADER_ROLE);
			CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], BaseDataObject.ADMIN);
		}

		[EntityImportExportAttribute("ID*", 110, "ID")]
		public new string ID
		{
			get { return RoleID(this.Role); }
			set
			{
				if (value == "Loader")
					this.Role = PERSON_ROLE.LOADER_ROLE;
				else if (value == "Supervisor")
					this.Role = PERSON_ROLE.SUPERVISOR_ROLE;
                else if (value == "Offloader")
                    this.Role = PERSON_ROLE.OFFLOADER_ROLE;
                else if (value == "{All}")
					this.Role = PERSON_ROLE.MAX_PERSON_ROLE;
			}
		}

		static public string RoleID ( PERSON_ROLE Role )
		{
			switch (Role)
			{
				case PERSON_ROLE.LOADER_ROLE:
					return "Loader";
				case PERSON_ROLE.SUPERVISOR_ROLE:
					return "Supervisor";
                case PERSON_ROLE.OFFLOADER_ROLE:
                    return "Offloader";
                case PERSON_ROLE.MAX_PERSON_ROLE:
					return "{All}";
				default:
					return "Undefined";
			}
		}
		public SqlCommand InsertSQL_
		{
			get
			{
				const string PARAM_NAME_PERSONGUID = "@PersonGuid";
				const SqlDbType PARAM_TYPE_PERSONGUID = SqlDbType.UniqueIdentifier;
				const string PARAM_NAME_ROLEINDEX = "@Role";
				const SqlDbType PARAM_TYPE_ROLEINDEX = SqlDbType.Int;
				const string PARAM_NAME_CREATEDDATE = "@CreateDate";
				const SqlDbType PARAM_TYPE_CREATEDDATE = SqlDbType.DateTimeOffset;
				const string PARAM_NAME_CREATEDBY = "@CreateBy";
				const SqlDbType PARAM_TYPE_CREATEDBY = SqlDbType.NVarChar;
				const int PARAM_SIZE_CREATEDBY = 100;



				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "INSERT INTO map.tblPersonnelToRole " +
						"(PersonnelGuid," +
						"LookupPersonnelRoleIndex," +
						"CreatedDate," +
						"CreatedBy" +
						") VALUES (" +
						DataObject.AddParameter(cmd, string.Empty, PARAM_NAME_PERSONGUID, PARAM_TYPE_PERSONGUID, PersonGuid) +
						DataObject.AddParameter(cmd, ", ", PARAM_NAME_ROLEINDEX, PARAM_TYPE_ROLEINDEX, (int)Role) +
						DataObject.AddParameter(cmd, ", ", PARAM_NAME_CREATEDDATE, PARAM_TYPE_CREATEDDATE, CreatedDate) +
						DataObject.AddParameter(cmd, ", ", PARAM_NAME_CREATEDBY, PARAM_TYPE_CREATEDBY, PARAM_SIZE_CREATEDBY, CreatedBy) +
						")";

						
				return cmd;
			}
		}

		public SqlCommand PurgeSQL
		{
			get
			{				
				const string PARAM_NAME_PERSONGUID = "@PersonGuid";
				const SqlDbType PARAM_TYPE_PERSONGUID = SqlDbType.UniqueIdentifier;
				const string PARAM_NAME_ROLEINDEX = "@Role";
				const SqlDbType PARAM_TYPE_ROLEINDEX = SqlDbType.Int;

				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "DELETE FROM map.tblPersonnelToRole WHERE " +
											DataObject.AddParameter(cmd, false, "PersonnelGuid", PARAM_NAME_PERSONGUID, PARAM_TYPE_PERSONGUID, PersonGuid)+
											DataObject.AddParameter(cmd, true, "LookupPersonnelRoleIndex", PARAM_NAME_ROLEINDEX, PARAM_TYPE_ROLEINDEX, (int) Role);
				return cmd;
			}
		}

		public SqlCommand SelectSQL ( bool bInTransaction )
		{
			const string PARAM_NAME_PERSONGUID = "@PersonGuid";
			const SqlDbType PARAM_TYPE_PERSONGUID = SqlDbType.UniqueIdentifier;
			const string PARAM_NAME_ROLEINDEX = "@Role";
			const SqlDbType PARAM_TYPE_ROLEINDEX = SqlDbType.Int;

			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = "SELECT * FROM map.tblPersonnelToRole " + SQLUpdateLock(bInTransaction) + " WHERE " +
										DataObject.AddParameter(cmd, false, "PersonnelGuid", PARAM_NAME_PERSONGUID, PARAM_TYPE_PERSONGUID, PersonGuid) +
										DataObject.AddParameter(cmd, true, "LookupPersonnelRoleIndex", PARAM_NAME_ROLEINDEX, PARAM_TYPE_ROLEINDEX, (int)Role);

			return cmd;
		}

		public SqlCommand EnumerateByPersonSQL
		{
			get
			{
				const string PARAM_NAME_PERSONGUID = "@PersonGuid";
				const SqlDbType PARAM_TYPE_PERSONGUID = SqlDbType.UniqueIdentifier;

				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "SELECT * FROM map.tblPersonnelToRole WHERE " +
										DataObject.AddParameter(cmd, false, "PersonnelGuid", PARAM_NAME_PERSONGUID, PARAM_TYPE_PERSONGUID, PersonGuid);
				return cmd;
			}
		}

	}

}
