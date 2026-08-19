using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for UserDataListValuesClass.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class UserDataListValuesClass : IUserDataListValues
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public UserDataListValuesClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, UserDataListValueClass UserDataListValue, ENTITY_TYPE userDataFieldEntityType)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (UserDataListValue == null)
				throw new ArgumentNullException("UserDataListValue");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			UserDataListValue.CreatedDate = DateTimeOffset.Now;
			UserDataListValue.CreatedBy = security.UserID;
			UserDataListValue.UpdatedDate = UserDataListValue.CreatedDate;
			UserDataListValue.UpdatedBy = security.UserID;
			UserDataListValue.UserDataFieldEntityType = userDataFieldEntityType;

			using (SqlCommand cmd = new SqlCommand())
			{
				UserDataListValue.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid UserDataFieldGuid, string Value, ENTITY_TYPE userDataFieldEntityType)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			UserDataListValueClass UserDataListValue = new UserDataListValueClass();
			UserDataListValue.UserDataFieldGuid = UserDataFieldGuid;
			UserDataListValue.ID = Value;
			UserDataListValue.UserDataFieldEntityType = userDataFieldEntityType;

			using (SqlCommand cmd = new SqlCommand())
			{
				UserDataListValue.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		public UserDataListValueCollectionClass Enumerate(SecurityClass security, Guid UserDataFieldGuid, ENTITY_TYPE userDataFieldEntityType)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			UserDataListValueClass UserDataListValue = new UserDataListValueClass();
			UserDataListValue.UserDataFieldGuid = UserDataFieldGuid;
			UserDataListValue.UserDataFieldEntityType = userDataFieldEntityType;

			using (SqlCommand cmd = new SqlCommand())
			{
				UserDataListValue.EnumerateSQL(cmd, ContextUtil.IsInTransaction);
				DataSet Set = ConsolidatedDA.GetDataSet(cmd, security);
				UserDataListValueCollectionClass UserDataListValueCollection = new UserDataListValueCollectionClass();

				DataTable Table = Set.Tables[0];
				while (Table.Rows.Count != 0)
				{
					UserDataListValue = new UserDataListValueClass();
					UserDataListValue.UserDataFieldEntityType = userDataFieldEntityType;
					UserDataListValue.Load(Set);
					UserDataListValueCollection.Add(UserDataListValue);
					Table.Rows.RemoveAt(0);
				}

				return UserDataListValueCollection;
			}
		}
	}
}
