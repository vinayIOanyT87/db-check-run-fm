using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.ChannelFactories;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Summary description for AlarmAndEventsClass.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AlarmAndEventsClass : IDependency, IAlarmAndEvents
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public AlarmAndEventsClass()
		{
		}

		protected void Validate(AlarmAndEventClass AlarmAndEvent)
		{
			if (AlarmAndEvent.ID.Length == 0)
				throw (new Exception("ID Required"));

			if (AlarmAndEvent.ID == "{None}"
			|| AlarmAndEvent.ID == "{Unassigned}"
			|| AlarmAndEvent.ID == "{All}")
				throw new Exception("ID is reserved key word " + AlarmAndEvent.ID);

			return;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AlarmAndEventClass alarmAndEvent)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (alarmAndEvent == null)
				throw new ArgumentNullException("alarmAndEvent");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();


			if (GetIdentityGuid(security, alarmAndEvent.Source, alarmAndEvent.ID) != Guid.Empty)
				throw (new Exception("Alarm and Event Exists"));

			Validate(alarmAndEvent);

			alarmAndEvent.SiteGuid = security.SiteGuid;
			alarmAndEvent.CreatedDate = DateTimeOffset.Now;
			alarmAndEvent.CreatedBy = security.UserID;
			alarmAndEvent.UpdatedDate = alarmAndEvent.CreatedDate;
			alarmAndEvent.UpdatedBy = security.UserID;
			alarmAndEvent.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				alarmAndEvent.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
			if (alarmAndEvent.EmailTemplate != null)
			{

            if (alarmAndEvent.EmailTemplate.IdentityGuid == Guid.Empty)
				{
               alarmAndEvent.EmailTemplate.IdentityGuid= Guid.NewGuid();

            }
				EmailTemplatesClass emailTemplatesClass = new EmailTemplatesClass();
            emailTemplatesClass.Add(security, alarmAndEvent.EmailTemplate);

				using (SqlCommand cmd = new SqlCommand())
				{
						EmailTemplateToAlarmAndEventMapClass emailTemplateToAlarmAndEventMap = new EmailTemplateToAlarmAndEventMapClass()
						{
							EmailTemplateGuid = alarmAndEvent.EmailTemplate.IdentityGuid,
							AlarmAndEventGuid = alarmAndEvent.IdentityGuid,
                     IdentityGuid = Guid.NewGuid()
                  };

						emailTemplateToAlarmAndEventMap.InsertSQL(cmd);
						ConsolidatedDA.ExecuteQuery(security, cmd);

				}
			}
			return alarmAndEvent.IdentityGuid;

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AlarmAndEventClass alarmAndEvent)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (alarmAndEvent == null)
				throw new ArgumentNullException("alarmAndEvent");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			Validate(alarmAndEvent);

			Guid identityGuid = GetIdentityGuid(security, alarmAndEvent.Source, alarmAndEvent.ID);

			if (identityGuid != Guid.Empty
				&& identityGuid != alarmAndEvent.IdentityGuid)
				throw (new Exception("Alarm and Event with Specified Source and ID Exists"));

			if (identityGuid == Guid.Empty)
				throw (new Exception("Alarm and Event Not Found"));

			alarmAndEvent.UpdatedDate = DateTimeOffset.Now;
			alarmAndEvent.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				alarmAndEvent.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

         if (alarmAndEvent.EmailTemplate != null)
         {

            EmailTemplatesClass emailTemplatesClass = new EmailTemplatesClass();
            if (alarmAndEvent.EmailTemplate.IdentityGuid == Guid.Empty)
            {

					emailTemplatesClass.Add(security, alarmAndEvent.EmailTemplate);
					using (SqlCommand cmd = new SqlCommand())
					{
						EmailTemplateToAlarmAndEventMapClass emailTemplateToAlarmAndEventMap = new EmailTemplateToAlarmAndEventMapClass()
						{
							EmailTemplateGuid = alarmAndEvent.EmailTemplate.IdentityGuid,
							AlarmAndEventGuid = alarmAndEvent.IdentityGuid,
							IdentityGuid = Guid.NewGuid()
						};

						emailTemplateToAlarmAndEventMap.InsertSQL(cmd);
						ConsolidatedDA.ExecuteQuery(security, cmd);

					}           
				
				}
				else
				{
					emailTemplatesClass.Modify(security, alarmAndEvent.EmailTemplate);

            }
         }
      }

		/// <summary>
		/// Retrieve the alarm and event record with the corresponding source and id
		/// </summary>
		/// <param name="security">Contains Security information</param>
		/// <param name="source">The source, e.g. System, Load Rack, or Transactions</param>
		/// <param name="alarmAndEventID">The ID, e.g. User Logged In</param>
		/// <returns>The alarm and event class corresponding to the provided source and id. If no record is found the alarm and event record
		/// will not have an identity guid.</returns>
		public AlarmAndEventClass Get(SecurityClass security, string source, string alarmAndEventID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			string alarmEventId = alarmAndEventID;

			if (string.IsNullOrEmpty(alarmAndEventID))
			{
				alarmEventId = string.Empty;
			}

			var alarmAndEvent = new AlarmAndEventClass
				                                   {
					                                   Source = source,
													   ID = alarmEventId,
					                                   SiteGuid = security.SiteGuid
				                                   };

			using (var cmd = new SqlCommand())
			{
				alarmAndEvent.SelectBySourceAndIDSQL(cmd);
				alarmAndEvent.Load(this.ConsolidatedDA.GetDataSet(cmd, security));
			}
         using (var cmd = new SqlCommand())
         {
            alarmAndEvent.EmailTemplate.SelectSQL(cmd, alarmAndEvent.IdentityGuid, true);
            alarmAndEvent.EmailTemplate.Load(ConsolidatedDA.GetDataSet(cmd, security));
         }

			return alarmAndEvent;
		}

		public AlarmAndEventClass Get(SecurityClass security, Guid alarmAndEventGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			AlarmAndEventClass alarmAndEvent = new AlarmAndEventClass();
			alarmAndEvent.IdentityGuid = alarmAndEventGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				alarmAndEvent.SelectSQL(cmd);
				alarmAndEvent.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}
         using (var cmd = new SqlCommand())
         {
            alarmAndEvent.EmailTemplate.SelectSQL(cmd, alarmAndEvent.IdentityGuid, true);
				alarmAndEvent.EmailTemplate.Load(ConsolidatedDA.GetDataSet(cmd, security));
         }

			return alarmAndEvent;
		}

		public Guid GetIdentityGuid(SecurityClass security, string source, string alarmAndEventID)
		{
			return this.Get(security, source, alarmAndEventID).IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid alarmAndEventGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			AlarmAndEventClass alarmAndEvent = Get(security, alarmAndEventGuid);
			if (alarmAndEvent.IdentityGuid == Guid.Empty)
				throw (new Exception("Alarm and Event Not Found"));

			using (SqlCommand cmd = new SqlCommand())
			{
				alarmAndEvent.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public AlarmAndEventCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			AlarmAndEventClass alarmAndEvent = new AlarmAndEventClass();
			alarmAndEvent.SiteGuid = security.SiteGuid;

			DataSet Set = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				alarmAndEvent.EnumerateSQL(cmd);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			AlarmAndEventCollectionClass alarmAndEventCollection = new AlarmAndEventCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				alarmAndEvent = new AlarmAndEventClass();
				alarmAndEvent.Load(Set);
				alarmAndEventCollection.Add(alarmAndEvent);
				Table.Rows.RemoveAt(0);
			}

			return alarmAndEventCollection;
		}

		public AlarmAndEventCollectionClass EnumerateBySourceAndType(SecurityClass security, string source, string type)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			AlarmAndEventClass alarmAndEvent = new AlarmAndEventClass();
			alarmAndEvent.Source = source;
			alarmAndEvent.SiteGuid = security.SiteGuid;

			DataSet Set = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				alarmAndEvent.EnumerateBySourceAndTypeSQL(cmd, type);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			AlarmAndEventCollectionClass alarmAndEventCollection = new AlarmAndEventCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				alarmAndEvent = new AlarmAndEventClass();
				alarmAndEvent.Load(Set);
				alarmAndEventCollection.Add(alarmAndEvent);
				Table.Rows.RemoveAt(0);
			}

			return alarmAndEventCollection;
		}

		public string[] EnumerateSources(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				throw new FMInsufficientRightsException();

			AlarmAndEventClass alarmAndEvent = new AlarmAndEventClass();
			alarmAndEvent.SiteGuid = security.SiteGuid;

			DataSet Set = null;
			using (SqlCommand cmd = new SqlCommand())
			{
				alarmAndEvent.EnumerateSourcesSQL(cmd);
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable Table = Set.Tables[0];
			string[] Modules; Modules = new string[Table.Rows.Count];

			for (int Item = 0; Item < Table.Rows.Count; Item++)
			{
				DataRow Row = Table.Rows[Item];
				Modules[Item] = (string)Row["Source"];
			}

			return Modules;
		}

		/// <summary>
		/// This method checks the number of rows in the table and throws an exception
		/// when the number of rows reaches <thresholdPercentage /> of the CapacityLimitInRows setting.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="capacityLimitInRows"></param>
		/// <param name="thresholdPercentage"></param>
		public void CheckLogSize(SecurityClass security, int capacityLimitInRows, int thresholdPercentage)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			AlarmAndEventClass alarmAndEvent = new AlarmAndEventClass();

			int numberOfRows = 0;
			using (SqlCommand cmd = new SqlCommand())
			{
				alarmAndEvent.RowCountSQL(cmd);
				numberOfRows = (int)ConsolidatedDA.ExecuteQuery(security, cmd, ConsolidatedDAClass.Uniquifier).Tables[0].Rows[0][0];
			}

			if (numberOfRows > 0)
			{
				int currentPercentage = Convert.ToInt32(((double)numberOfRows / (double)capacityLimitInRows) * (double)100.0);

				if (currentPercentage >= thresholdPercentage)
				{
					throw new FMRowCountThresholdException("Alarm and Event Log", currentPercentage.ToString());
				}

			}

		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			if (typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
			{
				EntityToSiteMapClass EntityToSiteMap = (EntityToSiteMapClass)Object;

				if (preOperation && EntityToSiteMap.TypeID == ENTITY_TYPE.ALARM_AND_EVENT)
				{
					EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
					EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndSiteGuid(security, EntityToSiteMap.TypeID, security.SiteGuid);
					if (EntityToSiteMapCollection.Count != 0)
						throw (new Exception("Alarm and Event Exists - " + EntityToSiteMap.ID));
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				AlarmAndEventClass AlarmAndEvent = new AlarmAndEventClass();
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
				EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, AlarmAndEvent.EntityType, Site.IdentityGuid);
				foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
					EntityToSiteMaps.Purge(security, EntityToSiteMap);
			}

		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			// Purge AlarmAndEvents
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				AlarmAndEventCollectionClass AlarmAndEventCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
				foreach (AlarmAndEventClass AlarmAndEvent in AlarmAndEventCollection)
				{
					if (Site.SiteGuid == AlarmAndEvent.SiteGuid)
						Purge(security, AlarmAndEvent.IdentityGuid);
					else
					{
						EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass();
						EntityToSiteMap.TypeID = AlarmAndEvent.EntityType;
						EntityToSiteMap.SiteGuid = Site.SiteGuid;
						EntityToSiteMap.IdentityGuid = AlarmAndEvent.IdentityGuid;
						EntityToSiteMaps.Purge(security, EntityToSiteMap);
					}
				}
			}

			else if (typeof(ApplicationStringClass).IsInstanceOfType(Object))
			{
				ApplicationStringClass ApplicationString = (ApplicationStringClass)Object;
				if (ApplicationString.Type == STRING_TYPE.ALARM_EVENT_CATEGORY)
				{
					AlarmAndEventCollectionClass AlarmAndEventCollection = Enumerate(security);
					foreach (AlarmAndEventClass AlarmAndEvent in AlarmAndEventCollection)
					{
						if (AlarmAndEvent.CategoryGuid == ApplicationString.IdentityGuid)
						{
							AlarmAndEvent.CategoryGuid = Guid.Empty;
							Modify(security, AlarmAndEvent);
						}
					}
				}
			}

			else if (typeof(AlarmPriorityClass).IsInstanceOfType(Object))
			{
				AlarmPriorityClass AlarmPriority = (AlarmPriorityClass)Object;
				AlarmAndEventCollectionClass AlarmAndEventCollection = Enumerate(security);
				foreach (AlarmAndEventClass AlarmAndEvent in AlarmAndEventCollection)
				{
					if (AlarmAndEvent.CategoryGuid == AlarmPriority.IdentityGuid)
					{
						AlarmAndEvent.PriorityGuid = Guid.Empty;
						Modify(security, AlarmAndEvent);
					}
				}
			}
		}
	}
}
