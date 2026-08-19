using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.UtilityObjects;

using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for Accessibilities.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AccessibilitiesClass : IDependency, IAccessibilities
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();


		private void Validate(SecurityClass security, AccessibilityClass Accessibility)
		{
			;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AccessibilityClass Accessibility)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Accessibility == null)
			{
				throw new ArgumentNullException("Accessibility");
			}

			Validate(security, Accessibility);

			Accessibility.CreatedDate = DateTimeOffset.Now;
			Accessibility.CreatedBy = security.UserID;
			Accessibility.UpdatedDate = Accessibility.CreatedDate;
			Accessibility.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				Accessibility.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return Accessibility.IdentityGuid;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, AccessibilityClass Accessibility)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (Accessibility == null)
			{
				throw new ArgumentNullException("Accessibility");
			}

			if (!security.HasRight(RIGHT.MODIFY_USERS) && security.UserGuid != Accessibility.UserGuid)
			{
				throw new FMInsufficientRightsException();
			}

			Validate(security, Accessibility);

			Accessibility.UpdatedDate = DateTimeOffset.Now;
			Accessibility.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				Accessibility.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}


		public AccessibilityClass Get(SecurityClass security, Guid userGuid, string settingKey)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USERS)
				&& !security.HasRight(RIGHT.MODIFY_USERS)
				&& security.UserGuid != userGuid)
			{
				throw new FMInsufficientRightsException();
			}

			var Accessibility = new AccessibilityClass(userGuid);
			Accessibility.SettingKey = settingKey;

			using (var cmd = new SqlCommand())
			{
				Accessibility.SelectSQL(cmd);
				Accessibility.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}


			return Accessibility;
		}

		public AccessibilityCollectionClass Enumerate(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_USERS)
				&& !security.HasRight(RIGHT.MODIFY_USERS)
				&& security.UserGuid != userGuid)
			{
				throw new FMInsufficientRightsException();
			}

			var Accessibility = new AccessibilityClass(userGuid);

			DataSet set;
			using (var cmd = new SqlCommand())
			{
				Accessibility.EnumerateSQL(cmd, security);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			DataTable table = set.Tables[0];

			var AccessibilityCollection = new AccessibilityCollectionClass();


			foreach (DataRow row in table.Rows)
			{
				var a = new AccessibilityClass();
				a.IdentityGuid = row.Field<Guid>("AccessibilityConfigurationSettingGuid");
				a.AccessibilityGuid = row.Field<Guid>("AccessibilityGuid");
				a.SettingKey = row.Field<string>("SettingKey");
				a.SettingValue = row.Field<string>("SettingValue");
				a.ValueRange = row.Field<string>("ValueRange");
				a.ValueType = row.Field<string>("ValueType");
				a.UserGuid = row.Field<Guid>("UserGuid");
				a.DisplayName = row.Field<string>("DisplayName");
				a.Description = row.Field<string>("Description");
				AccessibilityCollection.Add(a);

			}

			return AccessibilityCollection;
		}

		public void PurgeByUser(SecurityClass security, Guid userGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (userGuid == Guid.Empty)
			{
				throw new ArgumentNullException("userGuid");
			}

			var accessibility = new AccessibilityClass();
			accessibility.UserGuid = userGuid;

			using (var cmd = new SqlCommand())
			{
				accessibility.PurgeByUserSQL(cmd);
				this.ConsolidatedDA.ExecuteQuery(security, cmd);
			}
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


		}
	}
}
