namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Summary description for SchedulesClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class SchedulesClass : ISchedules
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public SchedulesClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, ScheduleClass schedule)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (schedule == null)
			{
				throw new ArgumentNullException("schedule");
			}

			Guid scheduleGuid = GetIdentityGuid(security,
										schedule.EntityGuid,
										schedule);

			if (!scheduleGuid.IsEmpty())
			{
				throw new Exception("Schedule Exists");
			}

			schedule.SiteGuid = security.SiteGuid;
			schedule.CreatedDate = DateTimeOffset.Now;
			schedule.CreatedBy = security.UserID;
			schedule.UpdatedDate = schedule.CreatedDate;
			schedule.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				schedule.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			schedule.IdentityGuid = GetIdentityGuid(security,
											schedule.EntityGuid,
											schedule);

			return schedule.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, ScheduleClass schedule)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (schedule == null)
			{
				throw new ArgumentNullException("schedule");
			}


			// Verify Schedule does not exist
			Guid scheduleGuid = GetIdentityGuid(security,
										schedule.EntityGuid,
										schedule);
			if (scheduleGuid.IsNotEmptyAndNotEqualTo(schedule.IdentityGuid))
			{
				throw (new Exception("Schedule Exists"));
			}

			ScheduleClass oldSchedule = Get(security, schedule.IdentityGuid, schedule.Type);

			if (oldSchedule.IdentityGuid.IsEmpty())
			{
				throw (new Exception("Schedule Not Found"));
			}

			// Set Format in Time members because Get cannot
			oldSchedule.OpeningTime.Format = schedule.OpeningTime.Format;
			oldSchedule.ClosingTime.Format = schedule.ClosingTime.Format;
			oldSchedule.EndOfDayTime.Format = schedule.EndOfDayTime.Format;

			schedule.UpdatedDate = DateTimeOffset.Now;
			schedule.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				schedule.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PurgeByIdentityGuid(SecurityClass security, Guid targetGuid, SCHEDULE_TYPE scheduleType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			ScheduleClass schedule = Get(security, targetGuid, scheduleType);

			Purge(security, schedule);
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, ScheduleClass schedule)
		{
			// Check security
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// Check Schedule object
			if (schedule == null)
			{
				throw new ArgumentNullException("schedule");
			}

			// Purge the object
			using (var cmd = new SqlCommand())
			{
				schedule.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public ScheduleClass Get(SecurityClass security, Guid targetGuid, SCHEDULE_TYPE scheduleType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var schedule = new ScheduleClass { IdentityGuid = targetGuid, Type = scheduleType };

			using (var cmd = new SqlCommand())
			{
				schedule.SelectSQL(cmd, ContextUtil.IsInTransaction);
				schedule.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return schedule;
		}

		internal Guid GetIdentityGuid(SecurityClass security, Guid targetEntityGuid, ScheduleClass srcSchedule)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var newSchedule = new ScheduleClass
			                  {
				                  EntityGuid = targetEntityGuid,
				                  Type = srcSchedule.Type,
				                  Day = srcSchedule.Day,
				                  HolidayDate = srcSchedule.HolidayDate
			                  };

			using (var cmd = new SqlCommand())
			{
				newSchedule.SelectByEntityGuidTypeAndDaySQL(cmd, ContextUtil.IsInTransaction);
				newSchedule.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return newSchedule.IdentityGuid;
		}

		/// <summary>
		/// The enumerate company access type.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="targetSiteGuid">
		/// The target Site globally unique identifier.
		/// </param>
		/// <returns>
		/// The<see cref="ScheduleCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Security is null
		/// </exception>
		public ScheduleCollectionClass EnumerateCompanyAccessType(SecurityClass security, Guid targetSiteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			using (var cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "map.usp_GetCompanyScheduleAccessBySite ";
				cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);

				var scheduleCollection = new ScheduleCollectionClass();

				var sites = new SitesClass();
				var site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

				var table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					var schedule = new ScheduleClass(site.GetDateTimeFormatInfo()) { Type = SCHEDULE_TYPE.COMPANY_ACCESS_TYPE };
					schedule.Load(set);
					scheduleCollection.Add(schedule);
					table.Rows.RemoveAt(0);
				}

				return scheduleCollection;
			}
		}

		public ScheduleCollectionClass EnumerateByEntityGuidAndType(SecurityClass security, Guid entityGuid, SCHEDULE_TYPE scheduleType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var schedule = new ScheduleClass
			{
				SiteGuid = security.SiteGuid,
				EntityGuid = entityGuid,
				Type = scheduleType
			};

			using (var cmd = new SqlCommand())
			{
				schedule.EnumerateByEntityGuidAndTypeSQL(cmd);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var scheduleCollection = new ScheduleCollectionClass();

				var sites = new SitesClass();
				SiteClass site = sites.GetByMemberAndProcessVariables(security, security.SiteGuid, false, false);

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					schedule = new ScheduleClass(site.GetDateTimeFormatInfo()) { Type = scheduleType };
					schedule.Load(set);
					scheduleCollection.Add(schedule);
					table.Rows.RemoveAt(0);
				}

				return scheduleCollection;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security,
									Guid guid,
									ScheduleCollectionClass newScheduleCollection,
									ScheduleCollectionClass existingScheduleCollection)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			// Return if the collections are equal.
			if (this.CompareCollections(newScheduleCollection, existingScheduleCollection))
			{
				return;
			}

			ScheduleCollectionClass addCollection;
			ScheduleCollectionClass modifyCollection;
			ScheduleCollectionClass deleteCollection;

			this.BuildAddModifyDeleteList(	newScheduleCollection,
											existingScheduleCollection,
											out addCollection,
											out modifyCollection,
											out deleteCollection);

			if (addCollection.Count > 0)
			{
				foreach (ScheduleClass newSchedule in addCollection)
				{
					newSchedule.EntityGuid = guid;
					newSchedule.IdentityGuid = this.Add(security, newSchedule);
				}
			}

			if (deleteCollection.Count > 0)
			{
				foreach (ScheduleClass existingSchedule in deleteCollection)
				{
					this.Purge(security, existingSchedule);
					existingScheduleCollection.Remove(existingSchedule);
				}
			}

			if (modifyCollection.Count > 0)
			{
				foreach (ScheduleClass schedule in modifyCollection)
				{
					this.Modify(security, schedule);
				}
			}
		}

		/// <summary>
		/// This method will create an add, modify, and delete collection of
		/// schedules.
		/// </summary>
		/// <param name="newScheduleCollection">The new schedule collection.</param>
		/// <param name="existingScheduleCollection">The existing schedule collection.</param>
		/// <param name="addCollection">Return a collection of schedules to be added.</param>
		/// <param name="modifyCollection">Return a collection of schedules to be modified.</param>
		/// <param name="deleteCollection">Return a collection of schedules to be deleted.</param>
		private void BuildAddModifyDeleteList(	ScheduleCollectionClass newScheduleCollection,
												ScheduleCollectionClass existingScheduleCollection,
												out ScheduleCollectionClass addCollection,
												out ScheduleCollectionClass modifyCollection,
												out ScheduleCollectionClass deleteCollection)
		{
			addCollection	 = new ScheduleCollectionClass();
			deleteCollection = new ScheduleCollectionClass();
			modifyCollection = new ScheduleCollectionClass();

			// Do nothing if the collections are empty.
			if ((newScheduleCollection == null || newScheduleCollection.Count == 0)
				&& (existingScheduleCollection == null || existingScheduleCollection.Count == 0))
			{
				return;
			}

			// If the new schedule collection is empty, then create a 
			// delete list if the GUIDs are not empty.
			if ((newScheduleCollection == null || newScheduleCollection.Count == 0) 
				&& existingScheduleCollection != null 
				&& existingScheduleCollection.Count > 0)
			{
				foreach (ScheduleClass schedule in existingScheduleCollection)
				{
					if (schedule.IdentityGuid != Guid.Empty)
					{
						deleteCollection.Add(schedule);
					}
				}
			}
			
			// If the existing schedule collection is empty, then create an
			// add collection.
			if ((existingScheduleCollection == null || existingScheduleCollection.Count == 0) 
				&& newScheduleCollection != null 
				&& newScheduleCollection.Count > 0)
			{
				foreach (ScheduleClass schedule in newScheduleCollection)
				{
					addCollection.Add(schedule);
				}
			}

			// If both collection exist, then create an add, potential modify, and
			// delete collections.
			if (newScheduleCollection != null 
				&& newScheduleCollection.Count > 0
				&& existingScheduleCollection != null 
				&& existingScheduleCollection.Count > 0)
			{
				foreach (ScheduleClass newSchedule in newScheduleCollection)
				{
					var existingSchedule = existingScheduleCollection.Find(x => x.IdentityGuid == newSchedule.IdentityGuid);

                    // An existingSchedule.IdentityGuid of the empty Guid indicates that there was no schedule entry in the database,
                    // so we need to create a new entry.
					if (existingSchedule == null || existingSchedule.IdentityGuid == Guid.Empty)
					{
						addCollection.Add(newSchedule);
					}
					else
					{
						if (existingSchedule.Day != newSchedule.Day
							|| existingSchedule.Enabled != newSchedule.Enabled
							|| existingSchedule.OpeningTime.Value != newSchedule.OpeningTime.Value
							|| existingSchedule.ClosingTime.Value != newSchedule.ClosingTime.Value
							|| existingSchedule.EndOfDayEnabled != newSchedule.EndOfDayEnabled
							|| existingSchedule.EndOfDayTime.Value != newSchedule.EndOfDayTime.Value)
						{
							modifyCollection.Add(newSchedule);
						}
					}
				}

				foreach (ScheduleClass oldSchedule in existingScheduleCollection)
				{
					var newSchedule = newScheduleCollection.Find(x => x.IdentityGuid == oldSchedule.IdentityGuid);

					if (newSchedule == null)
					{
						deleteCollection.Add(oldSchedule);
					}
				}
			}
		}

		/// <summary>
		/// This method will compare each item in the list to see if they are
		/// equal along with the item count and GUIDs.  If not, the false is
		/// returned. 
		/// </summary>
		/// <param name="newScheduleCollection">New schedule collection.</param>
		/// <param name="existingScheduleCollection">Old schedule collection.</param>
		/// <returns>Returns true if equal, otherwise returns false.</returns>
		private bool CompareCollections(ScheduleCollectionClass newScheduleCollection,
										ScheduleCollectionClass existingScheduleCollection)
		{
			if (newScheduleCollection == null || existingScheduleCollection == null)
			{
				return false;
			}

			if (newScheduleCollection.Count != existingScheduleCollection.Count)
			{
				return false;
			}

			foreach(ScheduleClass newSchedule in newScheduleCollection)
			{
				var existingSchedule = existingScheduleCollection.Find(x => x.IdentityGuid == newSchedule.IdentityGuid);

				if (existingSchedule == null)
				{
					return false;
				}

				if (newSchedule.Day != existingSchedule.Day
					|| newSchedule.HolidayDate != existingSchedule.HolidayDate
					|| newSchedule.Enabled != existingSchedule.Enabled
					|| !newSchedule.OpeningTime.Equals(existingSchedule.OpeningTime)
					|| !newSchedule.ClosingTime.Equals(existingSchedule.ClosingTime)
					|| newSchedule.EndOfDayEnabled != existingSchedule.EndOfDayEnabled
					|| !newSchedule.EndOfDayTime.Equals(existingSchedule.EndOfDayTime))
				{
					return false;
				}
			}

			return true;
		}
	}
}
