using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	public class FootNoteCollectionClass : List<FootNoteClass> { }

	[DataContract]
   [Serializable]
	public class FootNoteClass : BaseDataObject
	{
		[DataMember]
		public ApplicationStringMapCollectionClass FootNoteShipToMapCollection;
		[DataMember]
		public ApplicationStringMapCollectionClass FootNoteShipperMapCollection;
		[DataMember]
		public ApplicationStringMapCollectionClass FootNoteShipToStateMapCollection;
		[DataMember]
		public ApplicationStringMapCollectionClass FootNoteProductMapCollection;
        [DataMember]
        public ApplicationStringMapCollectionClass FootNoteAdditiveProfileMapCollection;

        [DataMember]
        public DateTime?  StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }

        public override string ID { get { return _ID; } set { SetString("Footnote", 250, value, ref _ID); } }
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.FOOTNOTE;
			}

			set
			{
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


		public FootNoteClass()
		{
			Reset();
		}

		public override void Reset()
		{
			base.Reset();
			FootNoteShipToMapCollection = new ApplicationStringMapCollectionClass();
			FootNoteShipperMapCollection = new ApplicationStringMapCollectionClass();
			FootNoteShipToStateMapCollection = new ApplicationStringMapCollectionClass();
			FootNoteProductMapCollection = new ApplicationStringMapCollectionClass();
            FootNoteAdditiveProfileMapCollection = new ApplicationStringMapCollectionClass();
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
			_ID = DataObject.getValue<string>(Row["ID"], "");
			_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
            StartDate = Row["StartDate"] as DateTime?;
            EndDate = Row["EndDate"] as DateTime?;
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
                "StartDate," +
                "EndDate," +
                "ApplicationStringGuid" +
				") VALUES (" +
				"@Type," +
				"@ID," +
				"@SiteGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
                "@StartDate," +
                "@EndDate," +
                "@ApplicationStringGuid" +
				") ";

			cmd.Parameters.Add("@Type", SqlDbType.Int);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmd.Parameters.Add("@EndDate", SqlDbType.DateTime);
            cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);            

            cmd.Parameters["@Type"].Value = (int)STRING_TYPE.FOOT_NOTE;
			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;

            if (StartDate.HasValue == true)
            {
                cmd.Parameters["@StartDate"].Value = StartDate.Value;
            }
            else
            {
                cmd.Parameters["@StartDate"].Value = DBNull.Value;
            }

            if (EndDate.HasValue == true)
            {
                cmd.Parameters["@EndDate"].Value = EndDate.Value;
            }
            else
            {
                cmd.Parameters["@EndDate"].Value = DBNull.Value;
            }

            cmd.Parameters["@ApplicationStringGuid"].Value = _IdentityGuid;  
        }

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE tblApplicationString " +
				"SET ID = @ID," +
				"SiteGuid = @SiteGuid," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy, " +
                "StartDate = @StartDate, " +
                "EndDate = @EndDate " +
                "WHERE ApplicationStringGuid = @ApplicationStringGuid AND LookupApplicationStringTypeIndex = @Type";

			cmd.Parameters.Add("@Type", SqlDbType.Int);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@StartDate", SqlDbType.DateTime);
            cmd.Parameters.Add("@EndDate", SqlDbType.DateTime);
            cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@Type"].Value = (int)STRING_TYPE.FOOT_NOTE;
			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
            if (StartDate.HasValue == true)
            {
                cmd.Parameters["@StartDate"].Value = StartDate.Value;
            }
            else
            {
                cmd.Parameters["@StartDate"].Value = DBNull.Value;
            }

            if (EndDate.HasValue == true)
            {
                cmd.Parameters["@EndDate"].Value = EndDate.Value;
            }
            else
            {
                cmd.Parameters["@EndDate"].Value = DBNull.Value;
            }
            cmd.Parameters["@ApplicationStringGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblApplicationString WHERE ApplicationStringGuid = @ApplicationStringGuid AND LookupApplicationStringTypeIndex = @Type";

			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Type", SqlDbType.Int);

			cmd.Parameters["@ApplicationStringGuid"].Value = IdentityGuid;
			cmd.Parameters["@Type"].Value = (int)STRING_TYPE.FOOT_NOTE;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblApplicationString.* FROM tblApplicationString " + SQLUpdateLock(bInTransaction) + " WHERE ApplicationStringGuid = @ApplicationStringGuid";

			cmd.Parameters.Add("@ApplicationStringGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ApplicationStringGuid"].Value = IdentityGuid;
		}

		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblApplicationString.*" +
				" FROM tblApplicationString " + SQLUpdateLock(bInTransaction) +
				" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblApplicationString", "ApplicationStringGuid") +
				" AND ID = @ID" +
				" AND LookupApplicationStringTypeIndex = @Type";

			cmd.Parameters.Add("@Type", SqlDbType.Int);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 250);

			cmd.Parameters["@Type"].Value = (int)STRING_TYPE.FOOT_NOTE;
			cmd.Parameters["@ID"].Value = ID;
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblApplicationString.*" +
				" FROM tblApplicationString" +
				" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblApplicationString", "ApplicationStringGuid") +
				" AND LookupApplicationStringTypeIndex = @Type" +
				" ORDER BY ID";

			cmd.Parameters.Add("@Type", SqlDbType.Int);

			cmd.Parameters["@Type"].Value = (int)STRING_TYPE.FOOT_NOTE;
		}
	}
}
