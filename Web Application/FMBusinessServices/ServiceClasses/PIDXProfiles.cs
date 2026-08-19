using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Xml;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PIDXProfilesClass : IDependency, IPIDXProfiles
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		public PIDXProfilesClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Private methods
		private void Validate(PIDXProfileClass PIDXProfile)
		{
			if (PIDXProfile.ID == "")
			{
				throw (new Exception("ID Required"));
			}

			if (PIDXProfile.ID == "{None}" || PIDXProfile.ID == "{Unassigned}" || PIDXProfile.ID == "{All}")
			{
				throw new Exception("ID is reserved key word " + PIDXProfile.ID);
			}
		}
		#endregion

		#region Public methods
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, PIDXProfileClass PIDXProfile)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (PIDXProfile == null)
			{
				throw new ArgumentNullException("PIDXProfile");
			}

			if (!security.HasRight(RIGHT.MODIFY_PIDX_PROFILES))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(PIDXProfile);

			if (GetIdentityGuid(security, PIDXProfile.ID) != Guid.Empty)
			{
				throw (new Exception("PIDXProfile Exists"));
			}

			PIDXProfile.SiteGuid = security.SiteGuid;
			PIDXProfile.CreatedDate = DateTimeOffset.Now;
			PIDXProfile.CreatedBy = security.UserID;
			PIDXProfile.UpdatedDate = PIDXProfile.CreatedDate;
			PIDXProfile.UpdatedBy = security.UserID;

			PIDXProfile.IdentityGuid = Guid.Empty;

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfile.IdentityGuid = Guid.NewGuid();
				PIDXProfile.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			PIDXProfileCompanyMapsClass PIDXProfileCompanyMaps = new PIDXProfileCompanyMapsClass();
			PIDXProfileCompanyMaps.ModifyCollection(security, PIDXProfile.IdentityGuid, PIDXProfile.PIDXProfileCompanyMapCollection, null);

			// TODO: Temporary commented out so that QA does not test change queue features.
			//ChangeQueueRecordsClass.ProcessChangeQueueRecords(security,
			//													ChangeQueueEventType.Add,
			//													PIDXProfile.IdentityGuid,
			//													PIDXProfile.ID,
			//													ChangeQueueRecordType.PIDXProfiles);

			return PIDXProfile.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PIDXProfileClass PIDXProfile)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (PIDXProfile == null)
			{
				throw new ArgumentNullException("PIDXProfile");
			}

			if (!security.HasRight(RIGHT.MODIFY_PIDX_PROFILES))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(PIDXProfile);

			Guid identityGuid = GetIdentityGuid(security, PIDXProfile.ID);

			if (identityGuid != Guid.Empty && identityGuid != PIDXProfile.IdentityGuid)
			{
				throw (new Exception("PIDXProfile Exists"));
			}

			PIDXProfileClass OldPIDXProfile = Get(security, PIDXProfile.IdentityGuid, true);

			if (OldPIDXProfile.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("PIDXProfile Not Found"));
			}

			PIDXProfile.UpdatedDate = DateTimeOffset.Now;
			PIDXProfile.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfile.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			PIDXProfileCompanyMapsClass PIDXProfileCompanyMaps = new PIDXProfileCompanyMapsClass();
			PIDXProfileCompanyMaps.ModifyCollection(security,
														PIDXProfile.IdentityGuid,
														PIDXProfile.PIDXProfileCompanyMapCollection,
														OldPIDXProfile.PIDXProfileCompanyMapCollection);

			// TODO: Temporary commented out so that QA does not test change queue features.
			// ChangeQueueRecordsClass.ProcessChangeQueueRecords(security, ChangeQueueEventType.Modify, PIDXProfile.IdentityGuid, PIDXProfile.ID, ChangeQueueRecordType.PIDXProfiles);
		}

		public PIDXProfileClass Get(SecurityClass security, Guid identityGuid, bool GetMaps)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (!security.HasRight(RIGHT.MODIFY_PIDX_PROFILES) && !security.HasRight(RIGHT.VIEW_PIDX_PROFILES))
			{
				throw new FMInsufficientRightsException();
			}

			PIDXProfileClass PIDXProfile = new PIDXProfileClass();
			PIDXProfile.IdentityGuid = identityGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfile.SelectSQL(cmd, ContextUtil.IsInTransaction);
				PIDXProfile.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			if (GetMaps)
			{
				PIDXProfileCompanyMapsClass PIDXProfileCompanyMaps = new PIDXProfileCompanyMapsClass();
				PIDXProfile.PIDXProfileCompanyMapCollection = PIDXProfileCompanyMaps.EnumerateByPIDXProfileGuid(security, PIDXProfile.IdentityGuid);
			}

			return PIDXProfile;
		}

		public Guid GetIdentityGuid(SecurityClass security, string ID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			PIDXProfileClass PIDXProfile = new PIDXProfileClass();
			PIDXProfile.ID = ID;
			PIDXProfile.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfile.SelectByIDSQL(cmd, ContextUtil.IsInTransaction);
				PIDXProfile.Load(this.consolidatedDA.GetDataSet(cmd, security));
			}

			return PIDXProfile.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pidxProfileGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			PIDXProfileClass PIDXProfile = Get(security, pidxProfileGuid, true);

			if (PIDXProfile.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("PIDXProfile Not Found"));
			}

			if (!security.HasRight(RIGHT.MODIFY_PIDX_PROFILES))
			{
				throw new FMInsufficientRightsException();
			}

			DependenciesClass Dependencies = new DependenciesClass(security);
			Dependencies.Purge(security, PIDXProfile);

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfile.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}

			// TODO: Temporary commented out so that QA does not test change queue features.
			// ChangeQueueRecordsClass.ProcessChangeQueueRecords(security, ChangeQueueEventType.Purge, PIDXProfile.IdentityGuid, PIDXProfile.ID, ChangeQueueRecordType.PIDXProfiles);
		}

		public PIDXProfileCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			PIDXProfileClass PIDXProfile = new PIDXProfileClass();
			PIDXProfile.SiteGuid = security.SiteGuid;

			PIDXProfileCollectionClass PIDXProfileCollection = new PIDXProfileCollectionClass();

			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfile.EnumerateSQL(cmd);
				Set = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				PIDXProfile = new PIDXProfileClass();
				PIDXProfile.Load(Set);
				PIDXProfileCollection.Add(PIDXProfile);
				Table.Rows.RemoveAt(0);
			}

			return PIDXProfileCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Import(SecurityClass Security, PIDXProfileClass pidxProfile)
		{
			if (Security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (pidxProfile == null)
			{
				throw new ArgumentNullException("pidxProfile");
			}

			SecurityClass security = Security.Clone();
			CompanyMapsClass companyMaps = new CompanyMapsClass();

			try
			{
				// Get this early since we may need to use it
				pidxProfile.IdentityGuid = GetIdentityGuid(security, pidxProfile.ID);

				// If the entity exists and is not owned by this site, do not update it.
				if (pidxProfile.IdentityGuid != Guid.Empty && pidxProfile.SiteGuid != security.SiteGuid)
				{
					return;
				}

				// Modify User Group Assignments
				foreach (PIDXProfileCompanyMapClass pidxCompanyMap in pidxProfile.PIDXProfileCompanyMapCollection)
				{
					Guid identityGuid = companyMaps.GetIdentityGuidByMapID(security, pidxCompanyMap.ShipToID);
					if (identityGuid == Guid.Empty)
					{
						throw (new Exception("CompanyMap: " + pidxCompanyMap.ShipToID + " Not Found For Site Id: " + security.SiteID));
					}

					pidxCompanyMap.CompanyPersonnelToShipToBillToGuid = identityGuid;
					pidxCompanyMap.PIDXProfileGuid = pidxProfile.IdentityGuid;
					pidxCompanyMap.SiteGuid = security.SiteGuid;
				}

				if (pidxProfile.IdentityGuid == Guid.Empty)
				{
					Add(security, pidxProfile);
				}
				else
				{
					Modify(security, pidxProfile);
				}
			}
			catch (Exception except)
			{
				throw new ApplicationException("[PIDX Profile Import Error ID] : " + pidxProfile.ID + ", " + except.Message);
			}
		}
		#endregion

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

			// Purge PIDXProfiles
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				PIDXProfileCollectionClass PIDXProfileCollection = Enumerate(security);

				foreach (PIDXProfileClass PIDXProfile in PIDXProfileCollection)
				{
					this.Purge(security, PIDXProfile.IdentityGuid);
				}
			}
		}
		#endregion
	}
}
