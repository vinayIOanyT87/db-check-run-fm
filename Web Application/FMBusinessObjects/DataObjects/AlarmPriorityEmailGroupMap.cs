using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   public class AlarmPriorityEmailGroupMapClass : BaseDataObject
{
		#region Private data members
		[DataMember]
		private Guid emailGroupGuid;
		[DataMember]
		private Guid alarmPriorityGuid;
		#endregion

		#region Constructor
		public AlarmPriorityEmailGroupMapClass ( )
		{
			Initialize();
		}
		#endregion

		#region Properties

		public Guid EmailGroupGuid
		{
			get { return this.emailGroupGuid; }
			set { this.emailGroupGuid = value; }
		}

		public Guid AlarmPriorityGuid
		{
			get { return this.alarmPriorityGuid; }
			set { this.alarmPriorityGuid = value; }
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblAlarmPriorityToEmailGroup " +
				"(EmailGroupGuid," +
				"AlarmPriorityGuid," +
				"CreatedDate," +
				"CreatedBy " +
				") VALUES (" +
				"@EmailGroupGuid," +
				"@AlarmPriorityGuid," +
				"@CreatedDate," +
				"@CreatedBy" +
				")";

			cmd.Parameters.Add("@AlarmPriorityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EmailGroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);

			cmd.Parameters["@AlarmPriorityGuid"].Value = AlarmPriorityGuid;
			cmd.Parameters["@EmailGroupGuid"].Value = EmailGroupGuid;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
		}

		public void PurgeSQL (SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblAlarmPriorityToEmailGroup WHERE EmailGroupGuid = @EmailGroupGuid AND AlarmPriorityGuid = @AlarmPriorityGuid";

			cmd.Parameters.Add("@AlarmPriorityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EmailGroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AlarmPriorityGuid"].Value = AlarmPriorityGuid;
			cmd.Parameters["@EmailGroupGuid"].Value = EmailGroupGuid;
		}

		#endregion

		private void Initialize()
		{
			this.emailGroupGuid = Guid.Empty;
			this.alarmPriorityGuid = Guid.Empty;
		}
		
		#region Public and internal methods
		public override void Reset ( )
		{
			base.Reset ( );
			Initialize();
		}

		public void Load ( DataSet Set )
		{
			if (Set == null)
			{
				throw new ArgumentNullException ( "Set" );
			}

			this.Reset ( );
			DataTable Table = Set.Tables[0];

			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

			this.emailGroupGuid = DataObject.getValue<Guid>(Row["EmailGroupGuid"], Guid.Empty);
			this.alarmPriorityGuid = DataObject.getValue<Guid>(Row["AlarmPriorityGuid"], Guid.Empty);
			base._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			base._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM map.tblAlarmPriorityToEmailGroup " + SQLUpdateLock(bInTransaction) +
				" WHERE AlarmPriorityGuid = @AlarmPriorityGuid AND EmailGroupGuid = @EmailGroupGuid";

			cmd.Parameters.Add("@AlarmPriorityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@EmailGroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AlarmPriorityGuid"].Value = AlarmPriorityGuid;
			cmd.Parameters["@EmailGroupGuid"].Value = EmailGroupGuid;
		}

		#endregion
	}
}
