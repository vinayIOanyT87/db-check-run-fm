using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	#region Email Group Collection Class
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(EmailGroupClass))]
	public class EmailGroupCollectionClass : CollectionBase
	{

		public void Add(EmailGroupClass EmailGroup)
		{
			List.Add(EmailGroup);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				List.RemoveAt(index);
			}
		}

		public void Remove(EmailGroupClass EmailGroup)
		{
			int index = 0;
			foreach (EmailGroupClass Item in List)
			{
				if (Item.IdentityGuid == EmailGroup.IdentityGuid)
				{
					List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public EmailGroupClass Item(int Index)
		{
			return (EmailGroupClass)List[Index];
		}
	}
	#endregion

	/// <summary>
	/// Summary description for EmailGroupClass.
	/// </summary>
	[DataContract]
   [Serializable]
	public class EmailGroupClass : BaseDataObject
	{
		#region Private data members
		[DataMember]
		private bool alwaysEnabled;
		[DataMember]
		private Time startTime;
		[DataMember]
		private Time endTime;
		[DataMember]
		private bool categoriesAndPriorities;
		[DataMember]
		private ApplicationStringMapCollectionClass categoryCollection;
		[DataMember]
		private AlarmPriorityCollectionClass priorityCollection;
		[DataMember]
		private ApplicationStringMapCollectionClass emailAddressCollection;
		#endregion

		#region Constructors
		public EmailGroupClass()
		{
			this.startTime = new Time();
			this.endTime = new Time();
			this.Reset();
		}

		/*public EmailGroupClass(SiteClass site)
		{
			this.startTime = new Time(site);
			this.endTime = new Time(site);
			this.Reset();
		}*/
		#endregion

		#region Properties

		public bool AlwaysEnabled
		{
			get { return this.alwaysEnabled; }
			set { this.alwaysEnabled = value; }
		}

		public Time StartTime
		{
			get { return this.startTime; }
			set { this.startTime = value; }
		}

		public Time EndTime
		{
			get { return this.endTime; }
			set { this.endTime = value; }
		}

		public bool CategoriesAndPriorities
		{
			get { return this.categoriesAndPriorities; }
			set { this.categoriesAndPriorities = value; }
		}

		public ApplicationStringMapCollectionClass CategoryCollection
		{
			get { return this.categoryCollection; }
			set { this.categoryCollection = value; }
		}

		public AlarmPriorityCollectionClass PriorityCollection
		{
			get { return this.priorityCollection; }
			set { this.priorityCollection = value; }
		}

		public ApplicationStringMapCollectionClass EmailAddressCollection
		{
			get { return this.emailAddressCollection; }
			set { this.emailAddressCollection = value; }
		}

		public override string ID
		{
			get { return _ID; }
			set { SetString("Group Name", 80, value, ref _ID); }
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblEmailGroups " +
				"(ID," +
				"AlwaysEnabled," +
				"StartTime," +
				"EndTime," +
				"CategoriesAndPriorities," +
				"SiteGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"EmailGroupGuid"+ 
				") VALUES (" +
				"@ID," +
				"@AlwaysEnabled," +
				"@StartTime," +
				"@EndTime," +
				"@CategoriesAndPriorities," +
				"@SiteGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@EmailGroupGuid)";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 80);
			cmd.Parameters.Add("@AlwaysEnabled", SqlDbType.Bit);
			cmd.Parameters.Add("@StartTime", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndTime", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CategoriesAndPriorities", SqlDbType.Bit);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@EmailGroupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = ID;

			if (AlwaysEnabled)
			{
				cmd.Parameters["@AlwaysEnabled"].Value = 1;
			}
			else
			{
				cmd.Parameters["@AlwaysEnabled"].Value = 0;
			}

			cmd.Parameters["@StartTime"].Value = TimeConverter.ToFMTime(startTime.Value);
			cmd.Parameters["@EndTime"].Value = TimeConverter.ToFMTime(endTime.Value);

			if (CategoriesAndPriorities)
			{
				cmd.Parameters["@CategoriesAndPriorities"].Value = 1;
			}
			else
			{
				cmd.Parameters["@CategoriesAndPriorities"].Value = 0;
			}

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@CreatedDate"].Value = CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@EmailGroupGuid"].Value = _IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{

			cmd.CommandText = "UPDATE tblEmailGroups " +
				"SET ID = @ID," +
				"AlwaysEnabled = @AlwaysEnabled," +
				"StartTime = @StartTime," +
				"EndTime = @EndTime," +
				"CategoriesAndPriorities = @CategoriesAndPriorities," +
				"SiteGuid = @SiteGuid," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE EmailGroupGuid = @EmailGroupGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 80);
			cmd.Parameters.Add("@AlwaysEnabled", SqlDbType.Bit);
			cmd.Parameters.Add("@StartTime", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndTime", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CategoriesAndPriorities", SqlDbType.Bit);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@EmailGroupGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = ID;

			if (AlwaysEnabled)
			{
				cmd.Parameters["@AlwaysEnabled"].Value = 1;
			}
			else
			{
				cmd.Parameters["@AlwaysEnabled"].Value = 0;
			}

			cmd.Parameters["@StartTime"].Value = TimeConverter.ToFMTime(startTime.Value);
			cmd.Parameters["@EndTime"].Value = TimeConverter.ToFMTime(endTime.Value);

			if (CategoriesAndPriorities)
			{
				cmd.Parameters["@CategoriesAndPriorities"].Value = 1;
			}
			else
			{
				cmd.Parameters["@CategoriesAndPriorities"].Value = 0;
			}

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;
			cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@EmailGroupGuid"].Value = IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblEmailGroups WHERE EmailGroupGuid = @EmailGroupGuid";
			cmd.Parameters.Add("@EmailGroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@EmailGroupGuid"].Value = IdentityGuid;
		}

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.EMAIL_GROUP; }
			set { ; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#endregion

		#region Public and Internal methods
		public override void Reset()
		{
			base.Reset();

			base.ID = "";
			this.alwaysEnabled = true;
			this.startTime.Value = TimeConverter.DefaultFMStartTime;
			this.endTime.Value = TimeConverter.DefaultFMEndTime;
			this.categoriesAndPriorities = true;
			this.categoryCollection = new ApplicationStringMapCollectionClass();
			this.priorityCollection = new AlarmPriorityCollectionClass();
			this.emailAddressCollection = new ApplicationStringMapCollectionClass();
		}

		public override void Load(Object o)
		{
			this.Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				base._IdentityGuid = DataObject.getValue<Guid>(Row["EmailGroupGuid"], Guid.Empty);
				base.ID = DataObject.getValue<string>(Row["ID"], "");
				this.alwaysEnabled = DataObject.getValue<bool>(Row["AlwaysEnabled"], true);
				this.startTime.Value = DataObject.getValue<DateTimeOffset>(Row["StartTime"], TimeConverter.DefaultFMStartTime);
				this.endTime.Value = DataObject.getValue<DateTimeOffset>(Row["EndTime"], TimeConverter.DefaultFMEndTime);
				this.categoriesAndPriorities = DataObject.getValue<bool>(Row["CategoriesAndPriorities"], true);
				base._SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				base._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				base._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				base._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], _CreatedDate);
				base._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			}
			else if (typeof(EmailGroupClass).IsInstanceOfType(o))
			{
				EmailGroupClass emailGroup = (EmailGroupClass)o;

				base._IdentityGuid = emailGroup.IdentityGuid;
				this.alwaysEnabled = emailGroup.AlwaysEnabled;
				this.startTime = emailGroup.StartTime;
				this.endTime = emailGroup.EndTime;
				this.categoriesAndPriorities = emailGroup.CategoriesAndPriorities;
				base._SiteGuid = emailGroup.SiteGuid;
				base._CreatedDate = emailGroup.CreatedDate;
				base._CreatedBy = emailGroup.CreatedBy;
				base._UpdatedDate = emailGroup.UpdatedDate;
				base._UpdatedBy = emailGroup.UpdatedBy;

				foreach (ApplicationStringMapClass Category in emailGroup.CategoryCollection)
				{
					ApplicationStringMapClass newCategory = new ApplicationStringMapClass();
					newCategory.Load(Category);
					CategoryCollection.Add(newCategory);
				}

				foreach (AlarmPriorityClass priority in emailGroup.PriorityCollection)
				{
					AlarmPriorityClass newPriority = new AlarmPriorityClass();
					newPriority.Load(priority);
					PriorityCollection.Add(newPriority);
				}

				foreach (ApplicationStringMapClass emailAddress in emailGroup.EmailAddressCollection)
				{
					ApplicationStringMapClass newEmailAddress = new ApplicationStringMapClass();
					newEmailAddress.Load(emailAddress);
					emailAddressCollection.Add(newEmailAddress);
				}
			}
			else
			{
				base.Load(o);
			}
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblEmailGroups " + SQLUpdateLock(bInTransaction) + " WHERE EmailGroupGuid = @EmailGroupGuid";
			cmd.Parameters.Add("@EmailGroupGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@EmailGroupGuid"].Value = IdentityGuid;
		}

		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblEmailGroups " + SQLUpdateLock(bInTransaction) +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblEmailGroups", "EmailGroupGuid") +
				" AND ID = @ID";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 80);
			cmd.Parameters["@ID"].Value = ID;
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblEmailGroups.*" +
					" FROM tblEmailGroups" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblEmailGroups", "EmailGroupGuid") +
					" ORDER BY ID";
		}

		public void EnumerateByAlarmPrioritySQL(SqlCommand cmd, Guid alarmPriorityGuid)
		{
			cmd.CommandText = "SELECT tblEmailGroups.* FROM tblEmailGroups, map.tblAlarmPriorityToEmailGroup " +
				" WHERE map.tblAlarmPriorityToEmailGroup.EmailGroupGuid = tblEmailGroups.EmailGroupGuid " +
				" AND map.tblAlarmPriorityToEmailGroup.AlarmPriorityGuid = @AlarmPriorityGuid" +
				" ORDER BY ID";

			cmd.Parameters.Add("@AlarmPriorityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AlarmPriorityGuid"].Value = alarmPriorityGuid;
		}

		#endregion
	}
}
