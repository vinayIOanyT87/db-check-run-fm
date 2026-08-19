using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Web;
using System.Security;
using System.Runtime.Serialization;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class GatesClass : IDependency, IGates
	{
		private ConsolidatedDAClass consolidatedDA;

		public GatesClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		private void Validate(GateClass Gate)
		{
			if (Gate.ID == "")
			{
				throw (new Exception("ID Required"));
			}

			if (Gate.ID == "{None}" || Gate.ID == "{Unassigned}" || Gate.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + Gate.ID);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, GateClass Gate)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Gate == null)
			{
				throw new ArgumentNullException("Gate");
			}

			this.Validate(Gate);

			if (!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			if (GetIdentityGuid(security, Gate.ID) != Guid.Empty)
			{
				throw (new Exception("Loading Location Exists"));
			}

			Gate.SiteGuid = security.SiteGuid;
			Gate.CreatedDate = DateTimeOffset.Now;
			Gate.CreatedBy = security.UserID;
			Gate.UpdatedDate = Gate.CreatedDate;
			Gate.UpdatedBy = security.UserID;
			Gate.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				Gate.InsertSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

			return Gate.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, GateClass Gate)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (Gate == null)
			{
				throw new ArgumentNullException("Gate");
			}

			this.Validate(Gate);

			if (!security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			Guid identityGuid = GetIdentityGuid(security, Gate.ID);

			if (identityGuid != Guid.Empty && identityGuid != Gate.IdentityGuid)
			{
				throw (new Exception("Loading Location Exists"));
			}

			GateClass OldGate = Get(security, Gate.IdentityGuid);

			if (OldGate.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Loading Location Not Found"));
			}

			Gate.UpdatedDate = DateTimeOffset.Now;
			Gate.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				Gate.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public GateClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_TICKETING_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			GateClass Gate = new GateClass();
			Gate.IdentityGuid = identityGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				Gate.SelectSQL(cmd, ContextUtil.IsInTransaction);
				Gate.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return Gate;
		}

		public Guid GetIdentityGuid(SecurityClass security, string ID)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			GateClass Gate = new GateClass();
			Gate.SiteGuid = security.SiteGuid;
			Gate.ID = ID;

			using (SqlCommand cmd = new SqlCommand())
			{
				Gate.SelectByIDSQL(cmd, ContextUtil.IsInTransaction);
				Gate.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return Gate.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.MODIFY_TICKETING_DATA) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			GateClass Gate = Get(security, identityGuid);

			if (Gate.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Loading Location Not Found"));
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				Gate.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public GateCollectionClass Enumerate(SecurityClass security)
		{
			return this.EnumerateBySite(security, security.SiteGuid);
		}

		public GateCollectionClass EnumerateBySite(SecurityClass security, Guid siteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TICKETING_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TICKETING_DATA)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA)
				&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
			{
				throw new FMInsufficientRightsException();
			}

			GateClass Gate = new GateClass();
			Gate.SiteGuid = security.SiteGuid;
			DataSet Set = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				Gate.EnumerateSQL(cmd);
				Set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			GateCollectionClass GateCollection = new GateCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				Gate = new GateClass();
				Gate.Load(Set);
				GateCollection.Add(Gate);
				Table.Rows.RemoveAt(0);
			}

			return GateCollection;
		}

		#region Dependency methods
		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
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
				throw new ArgumentNullException("Security");
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
				throw new ArgumentNullException("Security");
			}

			if (Object == null)
			{
				throw new ArgumentNullException("Object");
			}

			// Purge Gates
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				GateCollectionClass GateCollection = Enumerate(security);

				foreach (GateClass Gate in GateCollection)
				{
					this.Purge(security, Gate.IdentityGuid);
				}
			}
		}
		#endregion
	}
}