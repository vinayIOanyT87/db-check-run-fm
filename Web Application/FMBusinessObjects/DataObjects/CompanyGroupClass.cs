using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class CompanyGroupCollectionClass : List<CompanyGroupClass> { }

	/// <summary>
	/// Summary description for CompanyGroupClass.
	/// </summary>
	[DataContract]
   [Serializable]
	public class CompanyGroupClass : BaseDataObject
	{
		[DataMember]
		public STRING_TYPE Type { get; set; }

		[DataMember]
		public CompanyMapCollectionClass AssignedCompanyCollection { get; set; }

		[DataMember]
		public ProductMapCollectionClass AuthorizedProductCollection { get; set; }

		public override string ID { get { return _ID; } set { SetString("CompanyGroup ID", 30, value, ref _ID); } }

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.COMPANY_GROUP;
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

		public CompanyGroupClass()
		{
			Initialize();
		}

		private void Initialize()
		{
			Type = STRING_TYPE.COMPANY_GROUP;
			AssignedCompanyCollection = new CompanyMapCollectionClass();
			AuthorizedProductCollection = new ProductMapCollectionClass();
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
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

			_IdentityGuid = DataObject.getValue<Guid>(Row["ApplicationStringGuid"], Guid.Empty);
			Type = DataObject.getValue<STRING_TYPE>(Row["LookupApplicationStringTypeIndex"], STRING_TYPE.COMPANY_GROUP);
			_ID = DataObject.getValue<string>(Row["ID"], "");
			_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
		}

		public void InsertSQL(SqlCommand cmd)
		{

			cmd.CommandText = "INSERT INTO tblApplicationString " +
				"(LookupApplicationStringTypeIndex," +
				"ID," +
				"SiteGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"ApplicationStringGuid"+
				") VALUES (" +
				"@LookupApplicationStringTypeIndex," +
				"@ID," +
				"@SiteGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@ApplicationStringGuid"+
				") ";

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)Type;
			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@ApplicationStringGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblApplicationString " +
			  "SET ID = @ID," +
			  "SiteGuid = @SiteGuid," +
			  "UpdatedDate = @UpdatedDate," +
			  "UpdatedBy = @UpdatedBy " +
			  "WHERE ApplicationStringGuid = @IdentityGuid AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)Type;
			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblApplicationString WHERE ApplicationStringGuid = @IdentityGuid AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)Type;
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblApplicationString.* FROM tblApplicationString " +
				SQLUpdateLock(bInTransaction) + " WHERE ApplicationStringGuid = @IdentityGuid";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
		}

		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblApplicationString.*" +
				" FROM tblApplicationString " + SQLUpdateLock(bInTransaction) +
				" WHERE " + this.AppendSiteWhereClause(cmd, security, "tblApplicationString", "ApplicationStringGuid") +
				" AND ID = @ID" +
				" AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex";

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);

			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)this.Type;
			cmd.Parameters["@ID"].Value = this.ID;
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblApplicationString.*" +
				" FROM tblApplicationString" +
				" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblApplicationString", "ApplicationStringGuid") +
				" AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex" +
				" ORDER BY ID";

			cmd.Parameters.Add("@LookupApplicationStringTypeIndex", SqlDbType.Int);

			cmd.Parameters["@LookupApplicationStringTypeIndex"].Value = (int)this.Type;
		}
	}
}
