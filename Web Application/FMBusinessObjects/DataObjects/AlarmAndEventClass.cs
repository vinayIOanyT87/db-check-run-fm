using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	public class AlarmAndEventCollectionClass : List<AlarmAndEventClass> { }

	[DataContract]
	[Serializable]
	public class AlarmAndEventClass : BaseDataObject
	{
		[DataMember]
		public string Source { get; set; }

		[DataMember]
		public bool Alarm { get; set; }

		[DataMember]
		public Guid CategoryGuid { get; set; }

		[DataMember]
		public Guid PriorityGuid { get; set; }

		[DataMember]
		public string CategoryID { get; set; }

		[DataMember]
		public string PriorityID { get; set; }

		[DataMember]
		public bool Enabled { get; set; }

      [DataMember]
      public EmailTemplateClass EmailTemplate{ get; set; }


      string SelectClause = "SELECT tblAlarmAndEvents.*," +
								 "(SELECT ID FROM tblApplicationString WHERE tblApplicationString.ApplicationStringGuid = tblAlarmAndEvents.CategoryGuid) AS CategoryID," +
								 "(SELECT ID FROM tblAlarmPriorities WHERE tblAlarmPriorities.AlarmPriorityGuid = tblAlarmAndEvents.PriorityGuid) AS PriorityID";

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.ALARM_AND_EVENT; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public AlarmAndEventClass()
		{
			Initialize();
		}

		private void Initialize()
		{
			Source = "";
			Alarm = false;
			CategoryGuid = Guid.Empty;
			PriorityGuid = Guid.Empty;
			CategoryID = "{None}";
			PriorityID = "{None}";
			Enabled = true;
			EmailTemplate = new EmailTemplateClass();
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
		}

		/// <summary>
		/// Populates the AlarmAndEvent attributes with values from the passed dataset
		/// </summary>
		/// <param name="Set">The dataset from which values will be populated</param>
		public void Load(DataSet Set)
		{
			if (Set == null)
				throw new ArgumentNullException("Set");

			Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				_SiteGuid = Guid.Empty;
				return;
			}

			DataRow Row = Table.Rows[0];

			_IdentityGuid = DataObject.getValue<Guid>(Row["AlarmAndEventGuid"], Guid.Empty);
			Source = DataObject.getValue<string>(Row["Source"], "");
			Alarm = DataObject.getValue<bool>(Row["Alarm"], false);
			ID = DataObject.getValue<string>(Row["ID"], "");
			CategoryGuid = DataObject.getValue<Guid>(Row["CategoryGuid"], Guid.Empty);
			PriorityGuid = DataObject.getValue<Guid>(Row["PriorityGuid"], Guid.Empty);
			_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
			_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			Enabled = DataObject.getValue<bool>(Row["Enabled"], true);
			// selected column aliases
			CategoryID = DataObject.getValue<string>(Row["CategoryID"], "{None}");
			PriorityID = DataObject.getValue<string>(Row["PriorityID"], "{None}");
		}


		/// <summary>
		/// Generates the SQL command that will insert the AlarmAndEvent record into the DB
		/// </summary>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblAlarmAndEvents " +
				"(Source," +
				"Alarm," +
				"ID," +
				"CategoryGuid," +
				"PriorityGuid," +
				"SiteGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"Enabled," +
				" AlarmAndEventGuid "+
				") VALUES (" +
				"@Source," +
				"@Alarm," +
				"@ID," +
				"@CategoryGuid," +
				"@PriorityGuid," +
				"@SiteGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@Enabled," +
				"@AlarmAndEventGuid)";

			cmd.Parameters.Add("@Source", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@Alarm", SqlDbType.Bit);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@CategoryGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PriorityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@Enabled", SqlDbType.Bit);
			cmd.Parameters.Add("@AlarmAndEventGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@Source"].Value = Source;

			if (Alarm)
			{
				cmd.Parameters["@Alarm"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Alarm"].Value = 0;
			}

			cmd.Parameters["@ID"].Value = ID;

			if (CategoryGuid == Guid.Empty)
			{
				cmd.Parameters["@CategoryGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@CategoryGuid"].Value = CategoryGuid;
			}

			if (PriorityGuid == Guid.Empty)
			{
				cmd.Parameters["@PriorityGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@PriorityGuid"].Value = PriorityGuid;
			}

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;

			if (Enabled)
			{
				cmd.Parameters["@Enabled"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Enabled"].Value = 0;
			}

			cmd.Parameters["@AlarmAndEventGuid"].Value = _IdentityGuid;
		}

		public void RowCountSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT COUNT(*) FROM tblAlarmAndEventLog";
		}

		/// <summary>
		/// Generates the SQL that will save changes to the AlarmAndEvent object to the DB
		/// </summary>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblAlarmAndEvents " +
				"SET SiteGuid = @SiteGuid, " +
				"CategoryGuid = @CategoryGuid, " +
				"PriorityGuid = @PriorityGuid, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy, " +
				"Enabled = @Enabled " +
				"WHERE AlarmAndEventGuid = @AlarmAndEventGuid";

			cmd.Parameters.Add("@CategoryGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@PriorityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@Enabled", SqlDbType.Bit);
			cmd.Parameters.Add("@AlarmAndEventGuid", SqlDbType.UniqueIdentifier);

			if (CategoryGuid == Guid.Empty)
			{
				cmd.Parameters["@CategoryGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@CategoryGuid"].Value = CategoryGuid;
			}

			if (PriorityGuid == Guid.Empty)
			{
				cmd.Parameters["@PriorityGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@PriorityGuid"].Value = PriorityGuid;
			}

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;

			if (Enabled)
			{
				cmd.Parameters["@Enabled"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Enabled"].Value = 0;
			}

			cmd.Parameters["@AlarmAndEventGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblAlarmAndEvents WHERE AlarmAndEventGuid = @AlarmAndEventGuid";

			cmd.Parameters.Add("@AlarmAndEventGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AlarmAndEventGuid"].Value = IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAlarmAndEvents" +
				" WHERE AlarmAndEventGuid = @AlarmAndEventGuid";

			cmd.Parameters.Add("@AlarmAndEventGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AlarmAndEventGuid"].Value = IdentityGuid;
		}

		public void SelectBySourceAndIDSQL(SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAlarmAndEvents " +
				" WHERE Source = @Source" +
				" AND ID = @ID" +
				" AND (SiteGuid = @SiteGuid" +
				" OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityAlarmAndEventToSite" +
				" WHERE MapToSiteGuid = @SiteGuid))";

			cmd.Parameters.Add("@Source", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 120);

			cmd.Parameters["@Source"].Value = Source;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@ID"].Value = ID;
		}

		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
				" FROM tblAlarmAndEvents" +
				" WHERE (SiteGuid = @SiteGuid" +
				" OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityAlarmAndEventToSite" +
				" WHERE MapToSiteGuid = @SiteGuid))" +
				" ORDER BY Source, ID";

			cmd.Parameters.Add("@Source", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@Source"].Value = Source;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
		}

		public void EnumerateBySourceAndTypeSQL(SqlCommand cmd, string Type)
		{
			string AlarmClause = "";

			if (Type == "Alarms" || Type == "Events")
			{
				AlarmClause = " AND tblAlarmAndEvents.Alarm = @Alarm";
			}

			cmd.CommandText = SelectClause +
				" FROM tblAlarmAndEvents" +
				" WHERE tblAlarmAndEvents.Source = @Source" +
				AlarmClause +
				" AND (SiteGuid = @SiteGuid" +
				" OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityAlarmAndEventToSite" +
				" WHERE MapToSiteGuid = @SiteGuid))" +
				" ORDER BY Source, ID";

			cmd.Parameters.Add("@Source", SqlDbType.NVarChar, 120);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			if (Type == "Alarms" || Type == "Events")
			{
				cmd.Parameters.Add("@Alarm", SqlDbType.Bit);

				if (Type == "Alarms")
				{
					cmd.Parameters["@Alarm"].Value = 1;
				}
				else if (Type == "Events")
				{
					cmd.Parameters["@Alarm"].Value = 0;
				}
			}

			cmd.Parameters["@Source"].Value = Source;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
		}

		public void EnumerateSourcesSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT DISTINCT Source FROM tblAlarmAndEvents" +
			  " WHERE (SiteGuid = @SiteGuid" +
			  " OR SiteGuid = (SELECT OwnerSiteGuid FROM map.tblEntityAlarmAndEventToSite " +
			  " WHERE MapToSiteGuid = @SiteGuid))" +
			  " ORDER BY Source";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
		}
	}
}
