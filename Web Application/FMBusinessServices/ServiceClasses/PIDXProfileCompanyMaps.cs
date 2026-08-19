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
	public class PIDXProfileCompanyMapsClass : IDependency, IPIDXProfileCompanyMaps
	{
		private ConsolidatedDAClass consolidatedDA;

		public PIDXProfileCompanyMapsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, PIDXProfileCompanyMapClass PIDXProfileCompanyMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (PIDXProfileCompanyMap == null)
			{
				throw new ArgumentNullException("PIDXProfileCompanyMap");
			}


			PIDXProfileCompanyMap.SiteGuid = security.SiteGuid;
			PIDXProfileCompanyMap.CreatedDate = DateTimeOffset.Now;
			PIDXProfileCompanyMap.CreatedBy = security.UserID;
			PIDXProfileCompanyMap.UpdatedDate = PIDXProfileCompanyMap.CreatedDate;
			PIDXProfileCompanyMap.UpdatedBy = security.UserID;

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfileCompanyMap.IdentityGuid = Guid.NewGuid();
				PIDXProfileCompanyMap.InsertSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, PIDXProfileCompanyMapClass PIDXProfileCompanyMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (PIDXProfileCompanyMap == null)
			{
				throw new ArgumentNullException("PIDXProfileCompanyMap");
			}

			PIDXProfileCompanyMapClass OldPIDXProfileCompanyMap = Get(security, PIDXProfileCompanyMap.PIDXProfileGuid, PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid);

			PIDXProfileCompanyMap.CreatedBy = security.UserID;
			PIDXProfileCompanyMap.UpdatedDate = PIDXProfileCompanyMap.CreatedDate;

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfileCompanyMap.UpdateSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid pidxProfileGuid, Guid companyPersonnelToShipToBillToGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			PIDXProfileCompanyMapClass PIDXProfileCompanyMap = new PIDXProfileCompanyMapClass();
			PIDXProfileCompanyMap.SiteGuid = security.SiteGuid;
			PIDXProfileCompanyMap.PIDXProfileGuid = pidxProfileGuid;
			PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid = companyPersonnelToShipToBillToGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfileCompanyMap.PurgeSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public PIDXProfileCompanyMapClass Get(SecurityClass security, Guid pidxProfileGuid, Guid companyPersonnelToShipToBillToGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			PIDXProfileCompanyMapClass PIDXProfileCompanyMap = new PIDXProfileCompanyMapClass();
			PIDXProfileCompanyMap.SiteGuid = security.SiteGuid;
			PIDXProfileCompanyMap.PIDXProfileGuid = pidxProfileGuid;
			PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid = companyPersonnelToShipToBillToGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfileCompanyMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
				PIDXProfileCompanyMap.Load(consolidatedDA.GetDataSet(cmd, security));
			}

			return PIDXProfileCompanyMap;
		}

		public PIDXProfileCompanyMapCollectionClass EnumerateByPIDXProfileGuid(SecurityClass security, Guid pidxProfileGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			PIDXProfileCompanyMapClass PIDXProfileCompanyMap = new PIDXProfileCompanyMapClass();
			PIDXProfileCompanyMap.PIDXProfileGuid = pidxProfileGuid;
			PIDXProfileCompanyMap.SiteGuid = security.SiteGuid;

			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfileCompanyMap.EnumerateByPIDXProfileGuidSQL(cmd, ContextUtil.IsInTransaction);
				Set = consolidatedDA.GetDataSet(cmd, security);
			}



			PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = new PIDXProfileCompanyMapCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				PIDXProfileCompanyMap = new PIDXProfileCompanyMapClass();
				PIDXProfileCompanyMap.Load(Set);
				PIDXProfileCompanyMapCollection.Add(PIDXProfileCompanyMap);
				Table.Rows.RemoveAt(0);
			}

			return PIDXProfileCompanyMapCollection;
		}

		public PIDXProfileCompanyMapCollectionClass EnumerateSiteAndCompanyPersonnelToShipToBillToGuid(SecurityClass security, Guid companyPersonnelToShipToBillToGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			PIDXProfileCompanyMapClass PIDXProfileCompanyMap = new PIDXProfileCompanyMapClass();

			PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid = companyPersonnelToShipToBillToGuid;
			PIDXProfileCompanyMap.SiteGuid = security.SiteGuid;

			DataSet Set;
			using (SqlCommand cmd = new SqlCommand())
			{
				PIDXProfileCompanyMap.EnumerateBySiteAndCompanyPersonnelToShipToBillToGuidSQL(cmd);
				Set = consolidatedDA.GetDataSet(cmd, security);
			}



			PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection = new PIDXProfileCompanyMapCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				PIDXProfileCompanyMap = new PIDXProfileCompanyMapClass();
				PIDXProfileCompanyMap.Load(Set);
				PIDXProfileCompanyMapCollection.Add(PIDXProfileCompanyMap);
				Table.Rows.RemoveAt(0);
			}

			return PIDXProfileCompanyMapCollection;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(SecurityClass security,
										Guid pidxProfileGuid,
										PIDXProfileCompanyMapCollectionClass NewPIDXProfileCompanyMapCollection,
										PIDXProfileCompanyMapCollectionClass ExistingPIDXProfileCompanyMapCollection)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (NewPIDXProfileCompanyMapCollection != null)
			{
				foreach (PIDXProfileCompanyMapClass PIDXProfileCompanyMap in NewPIDXProfileCompanyMapCollection)
				{
					PIDXProfileCompanyMap.PIDXProfileGuid = pidxProfileGuid;

					if (ExistingPIDXProfileCompanyMapCollection != null)
					{
						int Item = 0;
						foreach (PIDXProfileCompanyMapClass ExistingPIDXProfileCompanyMap in ExistingPIDXProfileCompanyMapCollection)
						{
							if (ExistingPIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid == PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid)
							{
								if (ExistingPIDXProfileCompanyMap.SellerID != PIDXProfileCompanyMap.SellerID
								|| ExistingPIDXProfileCompanyMap.ShipperID != PIDXProfileCompanyMap.ShipperID
								|| ExistingPIDXProfileCompanyMap.ConsigneeNumber != PIDXProfileCompanyMap.ConsigneeNumber
								|| ExistingPIDXProfileCompanyMap.DenialOverride != PIDXProfileCompanyMap.DenialOverride
								|| ExistingPIDXProfileCompanyMap.UnavailableOverride != PIDXProfileCompanyMap.UnavailableOverride)
									Modify(security, PIDXProfileCompanyMap);

								break;
							}
							Item++;
						}

						if (Item == ExistingPIDXProfileCompanyMapCollection.Count)
							Add(security, PIDXProfileCompanyMap);
						else
							ExistingPIDXProfileCompanyMapCollection.Remove(Item);
					}
					else
						Add(security, PIDXProfileCompanyMap);
				}
			}

			if (ExistingPIDXProfileCompanyMapCollection != null)
			{
				foreach (PIDXProfileCompanyMapClass PIDXProfileCompanyMap in ExistingPIDXProfileCompanyMapCollection)
				{
					Purge(security, PIDXProfileCompanyMap.PIDXProfileGuid, PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid);
				}
			}
		}

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

			if (typeof(CompanyMapClass).IsInstanceOfType(Object))
			{
				CompanyMapClass CompanyMap = (CompanyMapClass)Object;
				if (CompanyMap.Type != COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP)
				{
					return;
				}

				PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection;
				PIDXProfileCompanyMapCollection = EnumerateSiteAndCompanyPersonnelToShipToBillToGuid(security, CompanyMap.IdentityGuid);

				foreach (PIDXProfileCompanyMapClass PIDXProfileCompanyMap in PIDXProfileCompanyMapCollection)
				{
					Purge(security, PIDXProfileCompanyMap.PIDXProfileGuid, PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid);
				}

				return;
			}

			if (typeof(PIDXProfileClass).IsInstanceOfType(Object))
			{
				PIDXProfileClass PIDXProfile = (PIDXProfileClass)Object;
				PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection;
				PIDXProfileCompanyMapCollection = EnumerateByPIDXProfileGuid(security, PIDXProfile.IdentityGuid);

				foreach (PIDXProfileCompanyMapClass PIDXProfileCompanyMap in PIDXProfileCompanyMapCollection)
				{
					Purge(security, PIDXProfileCompanyMap.PIDXProfileGuid, PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid);
				}

				return;
			}
		}
	}
}
