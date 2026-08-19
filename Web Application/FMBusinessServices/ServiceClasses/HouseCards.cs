namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.UtilityObjects;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class HouseCardsClass : IDependency, IHouseCards
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public HouseCardsClass()
		{
		}

		private void Validate(HouseCardClass houseCard)
		{
			if (string.IsNullOrEmpty(houseCard.ID))
			{
				throw (new Exception("ID Required"));
			}

			if (houseCard.ID == "{None}" || houseCard.ID == "{Unassigned}" || houseCard.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + houseCard.ID);
			}

			if (string.IsNullOrEmpty(houseCard.Number))
			{
				throw (new Exception("Number Required"));
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, HouseCardClass houseCard)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (houseCard == null)
			{
				throw new ArgumentNullException("houseCard");
			}

			if (!security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(houseCard);

			if (!GetIdentityGuidByID(security, houseCard.ID).IsEmpty())
			{
				throw (new Exception("HouseCard Exists"));
			}

			houseCard.SiteGuid = security.SiteGuid;
			houseCard.CreatedDate = DateTimeOffset.Now;
			houseCard.CreatedBy = security.UserID;
			houseCard.UpdatedDate = houseCard.CreatedDate;
			houseCard.UpdatedBy = security.UserID;
			houseCard.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				houseCard.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
			
			return houseCard.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, HouseCardClass houseCard)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (houseCard == null)
			{
				throw new ArgumentNullException("houseCard");
			}

			if (!security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(houseCard);

			Guid oldHouseCardGuid = GetIdentityGuidByID(security, houseCard.ID);
			
			if (oldHouseCardGuid.IsNotEmptyAndNotEqualTo(houseCard.IdentityGuid))
			{
				throw (new Exception("HouseCard Exists"));
			}

			HouseCardClass oldHouseCard = Get(security, houseCard.IdentityGuid);
			
			if (oldHouseCard.IdentityGuid.IsEmpty())
			{
				throw (new Exception("HouseCard Not Found"));
			}

			houseCard.UpdatedDate = DateTimeOffset.Now;
			houseCard.UpdatedBy = security.UserID;
			
			using (SqlCommand cmd = houseCard.UpdateSQL)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			if (houseCard.DriverGuid != oldHouseCard.DriverGuid)
			{
				var alarmAndEventLogs = new AlarmAndEventLogsClass();

				if (!houseCard.DriverGuid.IsEmpty())
				{
					alarmAndEventLogs.Add(security, houseCard.Assigned(houseCard.DriverID));
				}
				else
				{
					alarmAndEventLogs.Add(security, houseCard.Unassigned(oldHouseCard.DriverID));
				}
			}
		}

		public HouseCardClass Get(SecurityClass security, Guid targetHouseCardGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
			    && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var houseCard = new HouseCardClass { IdentityGuid = targetHouseCardGuid };

			using (SqlCommand cmd = houseCard.SelectSQL(ContextUtil.IsInTransaction))
			{
				houseCard.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return houseCard;
		}

		public Guid GetIdentityGuidByID(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
			    && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var houseCard = new HouseCardClass { ID = id, SiteGuid = security.SiteGuid };

			using (SqlCommand cmd = houseCard.SelectByIDSQL(ContextUtil.IsInTransaction))
			{
				houseCard.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return houseCard.IdentityGuid;
		}


		public Guid GetIdentityGuidByNumber(SecurityClass security, string number)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var houseCard = new HouseCardClass { Number = number, SiteGuid = security.SiteGuid };

			using (SqlCommand cmd = houseCard.SelectByNumberSQL)
			{
				houseCard.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return houseCard.IdentityGuid;
		}

		public Guid GetIdentityGuidByDriverGuid(SecurityClass security, Guid targetDriverGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			var houseCard = new HouseCardClass { DriverGuid = targetDriverGuid };

			using (SqlCommand cmd = houseCard.SelectByDriverGuidSQL)
			{
				houseCard.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return houseCard.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid targetHouseCardGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			HouseCardClass houseCard = Get(security, targetHouseCardGuid);

			if (houseCard.IdentityGuid.IsEmpty())
			{
				throw (new Exception("HouseCard Not Found"));
			}

			using (SqlCommand cmd = houseCard.PurgeSQL)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public HouseCardCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
			    && !security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) 
				&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			    && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var houseCard = new HouseCardClass { SiteGuid = security.SiteGuid };
			DataSet set;

			using (SqlCommand cmd = houseCard.EnumerateSQL)
			{
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			var houseCardCollection = new HouseCardCollectionClass();
			DataTable table = set.Tables[0];

			while (table.Rows.Count != 0)
			{
				houseCard = new HouseCardClass();
				houseCard.Load(set);
				houseCardCollection.Add(houseCard);
				table.Rows.RemoveAt(0);
			}

			return houseCardCollection;
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			// Purge HouseCards
			if (Object is SiteClass)
			{
				HouseCardCollectionClass houseCardCollection = Enumerate(security);

				foreach (HouseCardClass houseCard in houseCardCollection)
				{
					Purge(security, houseCard.IdentityGuid);
				}
			}
		}
	}
}
