using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for PersonRoleMapsClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class PersonRoleMapsClass : IPersonRoleMaps
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public PersonRoleMapsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, PersonRoleMapClass PersonRoleMap)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (PersonRoleMap == null)
				throw new ArgumentNullException("PersonRoleMap");

			if (!security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			DataSet Set = null;
			using (SqlCommand cmd = PersonRoleMap.SelectSQL(ContextUtil.IsInTransaction))
			{
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			if (Set.Tables[0].Rows.Count != 0)
				return;

			PersonRoleMap.SiteGuid = security.SiteGuid;
			PersonRoleMap.CreatedDate = DateTimeOffset.Now;
			PersonRoleMap.CreatedBy = security.UserID;
			using (SqlCommand cmd = PersonRoleMap.InsertSQL_)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid targetGuid, PERSON_ROLE Role)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			PersonRoleMapClass PersonRoleMap = new PersonRoleMapClass();
			PersonRoleMap.PersonGuid = targetGuid;
			PersonRoleMap.Role = Role;

			using (SqlCommand cmd = PersonRoleMap.PurgeSQL)
			{
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		public PersonRoleMapCollectionClass EnumerateByPerson(SecurityClass security, Guid targetGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_PERSONNEL_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
			&& !security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD)
			&& !security.HasRight(RIGHT.VIEW_TEST_ITEMS)
			&& !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS)
			&& !security.HasRight(RIGHT.MODIFY_PERSON_TRAINING)
			&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
			&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			&& !security.HasRight(RIGHT.VIEW_DISPATCH)
            && !security.HasRight(RIGHT.CREATE_ORDERS)
            && !security.HasRight(RIGHT.VIEW_ORDERS)
            && !security.HasRight(RIGHT.MODIFY_ORDERS)
            && !security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
            && !security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
            && !security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
            && !security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
			&& !security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
			&& !security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
				throw new FMInsufficientRightsException();

			PersonRoleMapClass PersonRoleMap = new PersonRoleMapClass();
			PersonRoleMap.PersonGuid = targetGuid;
			PersonRoleMap.SiteGuid = security.SiteGuid;

			DataSet Set = null;
			using (SqlCommand cmd = PersonRoleMap.EnumerateByPersonSQL)
			{
				Set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			PersonRoleMapCollectionClass PersonRoleMapCollection = new PersonRoleMapCollectionClass();

			DataTable Table = Set.Tables[0];
			while (Table.Rows.Count != 0)
			{
				PersonRoleMap = new PersonRoleMapClass();
				PersonRoleMap.Load(Set);
				PersonRoleMapCollection.Add(PersonRoleMap);
				Table.Rows.RemoveAt(0);
			}

			return PersonRoleMapCollection;
		}
	}
}
