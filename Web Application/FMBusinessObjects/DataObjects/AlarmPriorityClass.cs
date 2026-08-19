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
	public class AlarmPriorityCollectionClass : List<AlarmPriorityClass> { }

	/// <summary>
	/// Summary description for AlarmPriorityClass.
	/// </summary>
	[DataContract]
   [Serializable]
   public class AlarmPriorityClass : BaseDataObject
	{
		public static string BackgroundSteadyDefaultColor = "000000";

		public static string BackGroundAlternateDefaultColor = "FF00FF";

		public static string TextSteadyDefaultColor = "FF00FF";

		public static string TextAlternateDefaultColor = "696969";

        [EntityImportExportAttribute("ALARMPRIORITYGUID", 200, "ALARMPRIORITYGUID")]
        public Guid AlarmPriorityGuid
        {
            get
            {
                return this.IdentityGuid;
            }

            set
            {
                this.IdentityGuid = value;
            }
        }

        [EntityImportExportAttribute("BACKGROUNDSTEADY", 100, "BACKGROUNDSTEADY")]
      [DataMember]
		public string BackgroundSteady { get; set; }

      [EntityImportExportAttribute("BACKGROUNDALTERNATE", 100, "BACKGROUNDALTERNATE")]
      [DataMember]
		public string BackgroundAlternate { get; set; }

      [EntityImportExportAttribute("TEXTSTEADY", 100, "TEXTSTEADY")]
      [DataMember]
		public string TextSteady { get; set; }

      [EntityImportExportAttribute("TEXTALTERNATE", 100, "TEXTALTERNATE")]
      [DataMember]
		public string TextAlternate { get; set; }

      [DataMember]
		public string _SoundFile;

      [EntityImportExportAttribute("PRIORITY", 100, "PRIORITY")]
      [DataMember]
		public byte? Priority { get; set; }

      [EntityImportExportAttribute("ALARMPRIORITYID*", 100, "ALARMPRIORITYID")]
      public override string ID { get { return _ID; } set { SetString("Priority Name", 32, value, ref _ID); } }

      [EntityImportExportAttribute("SOUNDFILE", 100, "SOUNDFILE")]
      public string SoundFile { get { return _SoundFile; } set { SetString("Sound File", 50, value, ref _SoundFile); } }

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.ALARM_PRIORITY; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public AlarmPriorityClass()
		{
			Initialize();
		}

		private void Initialize()
		{
			BackgroundSteady = BackgroundSteadyDefaultColor;
			BackgroundAlternate = BackGroundAlternateDefaultColor;
			TextSteady = TextSteadyDefaultColor;
			TextAlternate = TextAlternateDefaultColor;
			SoundFile = "";
			Priority = null;
		}

		public override void Reset()
		{
			base.Reset();
			Initialize();
		}

		public override void Load(Object o)
		{
			Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];
				if (Table.Rows.Count == 0)
					return;

				DataRow Row = Table.Rows[0];

				_IdentityGuid = DataObject.getValue<Guid>(Row["AlarmPriorityGuid"], Guid.Empty);
				_ID = DataObject.getValue<string>(Row["ID"], "");
				BackgroundSteady = DataObject.getValue<string>(Row["BackgroundSteady"], BackgroundSteadyDefaultColor);
				BackgroundAlternate = DataObject.getValue<string>(Row["BackgroundAlternate"], BackGroundAlternateDefaultColor);
				TextSteady = DataObject.getValue<string>(Row["TextSteady"], TextSteadyDefaultColor);
				TextAlternate = DataObject.getValue<string>(Row["TextAlternate"], TextAlternateDefaultColor);
				SoundFile = DataObject.getValue<string>(Row["SoundFile"], "");
				_SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				Priority = DataObject.getValue<byte?>(Row["Priority"], Priority);
				_CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				_CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				_UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
				_UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			}

			else if (typeof(AlarmPriorityClass).IsInstanceOfType(o))
			{
				AlarmPriorityClass AlarmPriority = (AlarmPriorityClass)o;
				_IdentityGuid = AlarmPriority.IdentityGuid;
				ID = AlarmPriority.ID;
				BackgroundSteady = AlarmPriority.BackgroundSteady;
				BackgroundAlternate = AlarmPriority.BackgroundAlternate;
				TextSteady = AlarmPriority.TextSteady;
				TextAlternate = AlarmPriority.TextAlternate;
				SoundFile = AlarmPriority.SoundFile;
				_SiteGuid = AlarmPriority.SiteGuid;
				Priority = AlarmPriority.Priority;
				_CreatedDate = AlarmPriority.CreatedDate;
				_CreatedBy = AlarmPriority.CreatedBy;
				_UpdatedDate = AlarmPriority.UpdatedDate;
				_UpdatedBy = AlarmPriority.UpdatedBy;
			}
			else
				base.Load(o);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblAlarmPriorities " +
					"(ID," +
					"BackgroundSteady," +
					"BackgroundAlternate," +
					"TextSteady," +
					"TextAlternate," +
					"SoundFile," +
					"SiteGuid," +
               "Priority," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"AlarmPriorityGuid"+
					") VALUES (" +
					"@ID," +
					"@BackgroundSteady," +
					"@BackgroundAlternate," +
					"@TextSteady," +
					"@TextAlternate," +
					"@SoundFile," +
					"@SiteGuid," +
               "@Priority," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@AlarmPriorityGuid)";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 32);
			cmd.Parameters.Add("@BackgroundSteady", SqlDbType.NVarChar);
			cmd.Parameters.Add("@BackgroundAlternate", SqlDbType.NVarChar);
			cmd.Parameters.Add("@TextSteady", SqlDbType.NVarChar);
			cmd.Parameters.Add("@TextAlternate", SqlDbType.NVarChar);
			cmd.Parameters.Add("@SoundFile", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Priority", SqlDbType.TinyInt);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@AlarmPriorityGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@BackgroundSteady"].Value = BackgroundSteady;
			cmd.Parameters["@BackgroundAlternate"].Value = BackgroundAlternate;
			cmd.Parameters["@TextSteady"].Value = TextSteady;
			cmd.Parameters["@TextAlternate"].Value = TextAlternate;
			cmd.Parameters["@SoundFile"].Value = SoundFile;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			if (Priority.HasValue)
			{
				cmd.Parameters["@Priority"].Value = Priority;
			}
			else
			{
				cmd.Parameters["@Priority"].Value = DBNull.Value;
			}
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@AlarmPriorityGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE tblAlarmPriorities " +
				"SET ID = @ID," +
				"BackgroundSteady = @BackgroundSteady," +
				"BackgroundAlternate = @BackgroundAlternate," +
				"TextSteady = @TextSteady," +
				"TextAlternate = @TextAlternate," +
				"SoundFile = @SoundFile," +
				"SiteGuid = @SiteGuid," +
            "Priority = @Priority," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE AlarmPriorityGuid = @AlarmPriorityGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 32);
			cmd.Parameters.Add("@BackgroundSteady", SqlDbType.NVarChar);
			cmd.Parameters.Add("@BackgroundAlternate", SqlDbType.NVarChar);
			cmd.Parameters.Add("@TextSteady", SqlDbType.NVarChar);
			cmd.Parameters.Add("@TextAlternate", SqlDbType.NVarChar);
			cmd.Parameters.Add("@SoundFile", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Priority", SqlDbType.TinyInt);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@AlarmPriorityGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@BackgroundSteady"].Value = BackgroundSteady;
			cmd.Parameters["@BackgroundAlternate"].Value = BackgroundAlternate;
			cmd.Parameters["@TextSteady"].Value = TextSteady;
			cmd.Parameters["@TextAlternate"].Value = TextAlternate;
			cmd.Parameters["@SoundFile"].Value = SoundFile;
			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			if (Priority.HasValue)
			{
				cmd.Parameters["@Priority"].Value = Priority;
			}
			else
			{
				cmd.Parameters["@Priority"].Value = DBNull.Value;
			}
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@AlarmPriorityGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblalarmPriorities WHERE AlarmPriorityGuid = @AlarmPriorityGuid";
			cmd.Parameters.Add("@AlarmPriorityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AlarmPriorityGuid"].Value = IdentityGuid;
		}

		public static void SelectListSQL(SqlCommand cmd, bool bInTransaction, List<Guid> alarmPriorityList)
		{
			cmd.CommandText = "SELECT * FROM tblAlarmPriorities ap" + SQLUpdateLock(bInTransaction)
			                                                        + " INNER JOIN @GuidTable gtbl ON gtbl.Guid = ap.AlarmPriorityGuid";
			GenerateGuidListTable(cmd, alarmPriorityList);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblAlarmPriorities " + SQLUpdateLock(bInTransaction) + "WHERE AlarmPriorityGuid = @AlarmPriorityGuid";
			cmd.Parameters.Add("@AlarmPriorityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AlarmPriorityGuid"].Value = IdentityGuid;
		}

		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblAlarmPriorities " + SQLUpdateLock(bInTransaction) +
				" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblAlarmPriorities", "AlarmPriorityGuid") +
				" AND ID = @ID";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 32);
			cmd.Parameters["@ID"].Value = ID;
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT * FROM tblAlarmPriorities" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblAlarmPriorities", "AlarmPriorityGuid") +
					" ORDER BY ID";
		}

		public void EnumerateByEmailGroupSQL(SqlCommand cmd, Guid GroupGuid, bool bInTransaction)
		{

			cmd.CommandText = "SELECT tblAlarmPriorities.* FROM tblAlarmPriorities, map.tblAlarmPriorityToEmailGroup " + SQLUpdateLock(bInTransaction) +
					" WHERE  map.tblAlarmPriorityToEmailGroup.AlarmPriorityGuid = tblAlarmPriorities.AlarmPriorityGuid " +
					" AND map.tblAlarmPriorityToEmailGroup.EmailGroupGuid = @GroupGuid" +
					" ORDER BY ID";

			cmd.Parameters.Add("@GroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@GroupGuid"].Value = GroupGuid;

		}
	}
}
