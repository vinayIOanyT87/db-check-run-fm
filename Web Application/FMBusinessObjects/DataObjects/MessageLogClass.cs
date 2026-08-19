using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	public class MessageLogCollectionClass : List<MessageLogClass> { }

   [Serializable]
   [DataContract]
	public class MessageLogClass : BaseDataObject
	{
		#region public data members
		[DataMember]
		public Guid MessageGuid;
		[DataMember]
		public Guid CompanyGuid;
		[DataMember]
		public Guid PersonnelGuid;
		#endregion public data members


		#region Constructors
		public MessageLogClass()
		{
			Reset();
		}
		#endregion Constructors

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.MESSAGE_LOG;
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

		public override void Reset()
		{
			base.Reset();
			MessageGuid = Guid.Empty;
			CompanyGuid = Guid.Empty;
			PersonnelGuid = Guid.Empty;
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

			MessageGuid = DataObject.getValue<Guid>(Row["MessageGuid"], Guid.Empty);
			CompanyGuid = DataObject.getValue<Guid>(Row["CompanyGuid"], Guid.Empty);
			PersonnelGuid = DataObject.getValue<Guid>(Row["PersonnelGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
		}


		#region SqlCommand w/ Parameters

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblMessageLog (" +
				" MessageGuid, " +
				" CompanyGuid, " +
				" PersonnelGuid, " +
				" CreatedDate, " +
				" CreatedBy, " +
				" MessageLogGuid" +
				") VALUES (" +
				" @MessageGuid, " +
				" @CompanyGuid, " +
				" @PersonnelGuid, " +
				" @CreatedDate, " +
				" @CreatedBy, " +
				" @MessageLogGuid)";

			cmd.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PersonnelGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@MessageLogGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MessageGuid"].Value = MessageGuid;
			cmd.Parameters["@CompanyGuid"].Value = CompanyGuid;
			cmd.Parameters["@PersonnelGuid"].Value = PersonnelGuid;
			cmd.Parameters["@CreatedDate"].Value = _CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = _CreatedBy;
			cmd.Parameters["@MessageLogGuid"].Value = _IdentityGuid;

		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblMessageLog" +
					" WHERE MessageGuid = @MessageGuid";

			cmd.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MessageGuid"].Value = MessageGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT *" +
					" FROM tblMessageLog " + SQLUpdateLock(bInTransaction) +
					" WHERE MessageGuid = @MessageGuid" +
					" AND CompanyGuid = @CompanyGuid" +
					" AND PersonnelGuid = @PersonnelGuid";

			cmd.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PersonnelGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@MessageGuid"].Value = MessageGuid;
			cmd.Parameters["@CompanyGuid"].Value = CompanyGuid;
			cmd.Parameters["@PersonnelGuid"].Value = PersonnelGuid;
		}

		public void SelectTodaySQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * " +
				" FROM tblMessageLog " +
				" WHERE MessageGuid = @MessageGuid " +
				" AND CompanyGuid = @CompanyGuid " +
				" AND PersonnelGuid = @PersonnelGuid " +
				" AND CreatedDate = @CreatedDate";

			cmd.Parameters.Add("@MessageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PersonnelGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);

			cmd.Parameters["@MessageGuid"].Value = MessageGuid;
			cmd.Parameters["@CompanyGuid"].Value = CompanyGuid;
			cmd.Parameters["@PersonnelGuid"].Value = PersonnelGuid;
			cmd.Parameters["@CreatedDate"].Value = DateTimeOffset.Now;
		}
		#endregion

	}
}
