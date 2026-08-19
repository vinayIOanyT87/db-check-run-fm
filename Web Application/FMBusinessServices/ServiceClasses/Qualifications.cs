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
using FMBusinessObjects.UtilityObjects;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for QualificationsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class QualificationsClass : IDependency, IQualifications
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public QualificationsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, QualificationClass Qualification)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Qualification == null)
				throw new ArgumentNullException("Qualification");

			if ((Qualification.Type == QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			|| ((Qualification.Type == QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE
			|| Qualification.Type == QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION)
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			|| ((Qualification.Type == QUALIFICATION_TYPE.PERSON_LICENSE
			|| Qualification.Type == QUALIFICATION_TYPE.PERSON_QUALIFICATION)
			&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			if (!GetIdentityGuid(security, Qualification.Type, Qualification.ID).IsEmpty())
			{
				switch (Qualification.Type)
				{
					case QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT:
						throw (new Exception("Certificate Exists"));
					case QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE:
						throw (new Exception("Tag Exists"));
					case QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION:
						throw (new Exception("Inspection Exists"));
					case QUALIFICATION_TYPE.PERSON_LICENSE:
						throw (new Exception("License Exists"));
					case QUALIFICATION_TYPE.PERSON_QUALIFICATION:
					default:
						throw (new Exception("Qualification Exists"));
				}
			}

			Qualification.SiteGuid = security.SiteGuid;
			Qualification.CreatedDate = DateTimeOffset.Now;
			Qualification.CreatedBy = security.UserID;
			Qualification.UpdatedDate = Qualification.CreatedDate;
			Qualification.UpdatedBy = security.UserID;
			Qualification.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				Qualification.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
			// Create Entity to Site Map
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(Qualification);
			EntityToSiteMaps.Add(security, EntityToSiteMap, GetType().GUID);

			return Qualification.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, QualificationClass qualification)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (qualification == null)
				throw new ArgumentNullException("Qualification");

			if ((qualification.Type == QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			|| ((qualification.Type == QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE
			|| qualification.Type == QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION)
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			|| ((qualification.Type == QUALIFICATION_TYPE.PERSON_LICENSE
			|| qualification.Type == QUALIFICATION_TYPE.PERSON_QUALIFICATION)
			&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			Guid qualificationGuid = GetIdentityGuid(security, qualification.Type, qualification.ID);
			if (qualificationGuid.IsNotEmptyAndNotEqualTo(qualification.IdentityGuid))
			{
				switch (qualification.Type)
				{
					case QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT:
						throw (new Exception("Certificate Exists"));
					case QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE:
						throw (new Exception("Tag Exists"));
					case QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION:
						throw (new Exception("Inspection Exists"));
					case QUALIFICATION_TYPE.PERSON_LICENSE:
						throw (new Exception("License Exists"));
					case QUALIFICATION_TYPE.PERSON_QUALIFICATION:
					default:
						throw (new Exception("Qualification Exists"));
				}
			}

			QualificationClass oldQualification = Get(security, qualification.IdentityGuid);

			if (oldQualification.IdentityGuid.IsEmpty())
			{
				throw (new Exception("Qualification Not Found"));
			}

			qualification.UpdatedDate = DateTimeOffset.Now;
			qualification.UpdatedBy = security.UserID;
			using (SqlCommand cmd = qualification.UpdateSQL)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, qualification.EntityType, qualification.IdentityGuid);

			if (qualification.SiteGuid != qualification.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
				{
					EntityToSiteMap.ID = qualification.ID;
					EntityToSiteMaps.Purge(security, EntityToSiteMap);
				}

				// Create Entity to Site Map
				EntityToSiteMapClass NewEntityToSiteMap = new EntityToSiteMapClass(qualification);
				EntityToSiteMaps.Add(security, NewEntityToSiteMap, GetType().GUID);
			}
		}

		public QualificationClass Get(SecurityClass security, Guid targetGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			QualificationClass Qualification = new QualificationClass();
			Qualification.IdentityGuid = targetGuid;
			using (SqlCommand cmd = Qualification.SelectSQL(ContextUtil.IsInTransaction))
			{
				Qualification.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			if ((Qualification.Type == QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT
			&& !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			|| ((Qualification.Type == QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE
			|| Qualification.Type == QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION)
			&& !security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			|| ((Qualification.Type == QUALIFICATION_TYPE.PERSON_LICENSE
			|| Qualification.Type == QUALIFICATION_TYPE.PERSON_QUALIFICATION)
			&& !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
			&& !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING)
			&& !security.HasRight(RIGHT.VIEW_PERSONNEL_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			return Qualification;
		}

		public Guid GetIdentityGuid(SecurityClass security, QUALIFICATION_TYPE Type, string ID)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			QualificationClass Qualification = new QualificationClass();
			Qualification.Type = Type;
			Qualification.ID = ID;
			Qualification.SiteGuid = security.SiteGuid;

			using (SqlCommand cmd = Qualification.SelectByIDAndTypeSQL(security, ContextUtil.IsInTransaction))
			{
				Qualification.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return Qualification.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid targetGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			QualificationClass Qualification = Get(security, targetGuid);
			if (Qualification.IdentityGuid.IsEmpty())
			{
				throw (new Exception("Qualification Not Found"));
			}

			if ((Qualification.Type == QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			|| ((Qualification.Type == QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE
			|| Qualification.Type == QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION)
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			|| ((Qualification.Type == QUALIFICATION_TYPE.PERSON_LICENSE
			|| Qualification.Type == QUALIFICATION_TYPE.PERSON_QUALIFICATION)
			&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			DependenciesClass Dependencies = new DependenciesClass(security);
			Dependencies.Purge(security, Qualification);

			// Purge from EntityToSiteMap
			EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, Qualification.EntityType, targetGuid);
			foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
			{
				EntityToSiteMap.ID = Qualification.ID;
				EntityToSiteMaps.Purge(security, EntityToSiteMap);
			}

			using (SqlCommand cmd = Qualification.PurgeSQL)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		public QualificationCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			QualificationClass Qualification = new QualificationClass();
			Qualification.SiteGuid = security.SiteGuid;

			QUALIFICATION_TYPE[] Types ={	QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT,
													QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION,
													QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE,
													QUALIFICATION_TYPE.PERSON_QUALIFICATION,
													QUALIFICATION_TYPE.PERSON_LICENSE,
													QUALIFICATION_TYPE.PERSON_TRAINING
												};

			QualificationCollectionClass QualificationCollection = new QualificationCollectionClass();

			foreach (QUALIFICATION_TYPE Type in Types)
			{
				Qualification.Type = Type;
				DataSet Set = null;
				using (SqlCommand cmd = Qualification.EnumerateByTypeSQL(security))
				{
					Set = ConsolidatedDA.GetDataSet(cmd, security);
				}

				DataTable Table = Set.Tables[0];
				while (Table.Rows.Count != 0)
				{
					Qualification = new QualificationClass();
					Qualification.Load(Set);
					QualificationCollection.Add(Qualification);
					Table.Rows.RemoveAt(0);
				}
			}
			return QualificationCollection;
		}

		public QualificationCollectionClass EnumerateByType(SecurityClass security, QUALIFICATION_TYPE Type)
		{
			if (security == null)
				throw new ArgumentNullException("Security");


			if ((Type == QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT
			&& !security.HasRight(RIGHT.VIEW_COMPANY_DATA)
			&& !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			|| ((Type == QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE
			|| Type == QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION)
			&& !security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			|| ((Type == QUALIFICATION_TYPE.PERSON_LICENSE
			|| Type == QUALIFICATION_TYPE.PERSON_QUALIFICATION)
			&& !security.HasRight(RIGHT.VIEW_PERSONNEL_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
			&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT))
			&& !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			&& !security.HasRight(RIGHT.VIEW_TRAINING_QUALIFICATIONS)
			&& !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
			&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			&& !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING)
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS))
				throw new FMInsufficientRightsException();


			QualificationClass Qualification = new QualificationClass();
			Qualification.Type = Type;
			Qualification.SiteGuid = security.SiteGuid;
			DataSet Set = null;
			using (SqlCommand cmd = Qualification.EnumerateByTypeSQL(security))
			{
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			QualificationCollectionClass QualificationCollection = new QualificationCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				Qualification = new QualificationClass();
				Qualification.Load(Set);
				QualificationCollection.Add(Qualification);
				Table.Rows.RemoveAt(0);
			}

			return QualificationCollection;
		}

		void IDependency.Insert(SecurityClass security, BaseDataObject Object, bool preOperation)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			if (preOperation && typeof(EntityToSiteMapClass).IsInstanceOfType(Object))
			{
				EntityToSiteMapClass EntityToSiteMap = (EntityToSiteMapClass)Object;

				if ( EntityToSiteMap.TypeID != ENTITY_TYPE.QUALIFICATION_MAP )
				{
					return;
				}

				QualificationClass Qualification = new QualificationClass();
				Qualification.EntityType = EntityToSiteMap.TypeID;
				if ((Qualification.Type != QUALIFICATION_TYPE.MAX_QUALIFICATION_TYPE) &&
					(!GetIdentityGuid(security, Qualification.Type, EntityToSiteMap.ID).IsEmpty()))
				{
					switch (Qualification.Type)
					{
						case QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT:
							throw (new Exception("Certificate Exists - " + EntityToSiteMap.ID));
						case QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE:
							throw (new Exception("Tag Exists - " + EntityToSiteMap.ID));
						case QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION:
							throw (new Exception("Inspection Exists - " + EntityToSiteMap.ID));
						case QUALIFICATION_TYPE.PERSON_LICENSE:
							throw (new Exception("License Exists - " + EntityToSiteMap.ID));
						case QUALIFICATION_TYPE.PERSON_QUALIFICATION:
						default:
							throw (new Exception("Qualification Exists - " + EntityToSiteMap.ID));
					}
				}
			}
		}

		void IDependency.Update(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				QualificationCollectionClass QualificationCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
				foreach (QualificationClass Qualification in QualificationCollection)
				{
					if (Site.SiteGuid == Qualification.SiteGuid)
					{
						EntityToSiteMapCollectionClass EntityToSiteMapCollection = EntityToSiteMaps.EnumerateByTypeIDAndGuid(security, Qualification.EntityType, Qualification.IdentityGuid);
						foreach (EntityToSiteMapClass EntityToSiteMap in EntityToSiteMapCollection)
						{
							if (EntityToSiteMap.SiteGuid != Site.SiteGuid)
							{
								EntityToSiteMap.ID = Qualification.ID;
								EntityToSiteMaps.Purge(security, EntityToSiteMap);
							}
						}
					}
				}
			}
		}

		void IDependency.Purge(SecurityClass security, BaseDataObject Object)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (Object == null)
				throw new ArgumentNullException("Object");

			// Purge Qualifications
			if (typeof(SiteClass).IsInstanceOfType(Object))
			{
				SiteClass Site = (SiteClass)Object;
				QualificationCollectionClass QualificationCollection = Enumerate(security);
				EntityToSiteMaps EntityToSiteMaps = new EntityToSiteMaps();
				foreach (QualificationClass Qualification in QualificationCollection)
				{
					if (Site.SiteGuid == Qualification.SiteGuid)
						Purge(security, Qualification.IdentityGuid);
					else
					{
						EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(Qualification);
						EntityToSiteMap.SiteGuid = Site.SiteGuid;
						EntityToSiteMaps.Purge(security, EntityToSiteMap);
					}
				}
			}
		}
	}
}
