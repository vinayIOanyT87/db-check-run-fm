namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TestEquipmentResultsClass : ITestEquipmentResults, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TestEquipmentResultsClass()
		{
		}

		private void Validate(TestEquipmentResultClass testEquipmentResult)
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TestEquipmentResultClass testEquipmentResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testEquipmentResult == null)
			{
				throw new ArgumentNullException("testEquipmentResult");
			}

			if (!security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(testEquipmentResult);

			testEquipmentResult.CreatedDate = DateTimeOffset.Now;
			testEquipmentResult.CreatedBy = security.UserID;
			testEquipmentResult.UpdatedDate = testEquipmentResult.CreatedDate;
			testEquipmentResult.UpdatedBy = security.UserID;
			testEquipmentResult.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				testEquipmentResult.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return testEquipmentResult.IdentityGuid;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TestEquipmentResultClass testEquipmentResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testEquipmentResult == null)
			{
				throw new ArgumentNullException("testEquipmentResult");
			}

			this.Validate(testEquipmentResult);

			TestEquipmentResultClass oldTestEquipmentResult = Get(security, testEquipmentResult.IdentityGuid);

			if (oldTestEquipmentResult.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("TestEquipmentResult Not Found"));
			}

			testEquipmentResult.UpdatedDate = DateTimeOffset.Now;
			testEquipmentResult.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				testEquipmentResult.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public TestEquipmentResultClass Get(SecurityClass security, Guid testEquipmentResultGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var testEquipmentResult = new TestEquipmentResultClass { IdentityGuid = testEquipmentResultGuid };

			if (testEquipmentResultGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					testEquipmentResult.SelectSQL(cmd, ContextUtil.IsInTransaction);
					testEquipmentResult.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return testEquipmentResult;
		}


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid testEquipmentResultGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			TestEquipmentResultClass testEquipmentResult = Get(security, testEquipmentResultGuid);

			if (testEquipmentResult.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (var cmd = new SqlCommand())
			{
				testEquipmentResult.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
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


		public TestEquipmentResultCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			var testEquipmentResult = new TestEquipmentResultClass();

			using (var cmd = new SqlCommand())
			{
				testEquipmentResult.EnumerateSQL(cmd, security);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testEquipmentResultCollection = new TestEquipmentResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testEquipmentResult = new TestEquipmentResultClass();
					testEquipmentResult.Load(set);
					testEquipmentResultCollection.Add(testEquipmentResult);
					table.Rows.RemoveAt(0);
				}

				return testEquipmentResultCollection;
			}
		}

		public TestEquipmentResultCollectionClass EnumerateByTestSetEquipmentResultGuid(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			var testEquipmentResult = new TestEquipmentResultClass();

			using (var cmd = new SqlCommand())
			{
				testEquipmentResult.EnumerateByTestSetEquipmentResultGuidSQL(cmd, security, identityGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testEquipmentResultCollection = new TestEquipmentResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testEquipmentResult = new TestEquipmentResultClass();
					testEquipmentResult.Load(set);
					testEquipmentResultCollection.Add(testEquipmentResult);
					table.Rows.RemoveAt(0);
				}

				return testEquipmentResultCollection;
			}
		}
	}
}
