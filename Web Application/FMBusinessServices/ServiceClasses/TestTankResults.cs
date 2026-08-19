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
	public class TestTankResultsClass : ITestTankResults, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TestTankResultsClass()
		{
		}


		private void Validate(TestTankResultClass testTankResult)
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TestTankResultClass testTankResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testTankResult == null)
			{
				throw new ArgumentNullException("testTankResult");
			}

			if (!security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			Validate(testTankResult);

			testTankResult.CreatedDate = DateTimeOffset.Now;
			testTankResult.CreatedBy = security.UserID;
			testTankResult.UpdatedDate = testTankResult.CreatedDate;
			testTankResult.UpdatedBy = security.UserID;
			testTankResult.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				testTankResult.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
				return testTankResult.IdentityGuid;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TestTankResultClass testTankResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testTankResult == null)
			{
				throw new ArgumentNullException("testTankResult");
			}

			Validate(testTankResult);

			TestTankResultClass oldTestTankResult = Get(security, testTankResult.IdentityGuid);

			if (oldTestTankResult.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("TestTankResult Not Found"));
			}

			testTankResult.UpdatedDate = DateTimeOffset.Now;
			testTankResult.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				testTankResult.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public TestTankResultClass Get(SecurityClass security, Guid testTankResultGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			var testTankResult = new TestTankResultClass();
			testTankResult.IdentityGuid = testTankResultGuid;

			if (testTankResultGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					testTankResult.SelectSQL(cmd, ContextUtil.IsInTransaction);
					testTankResult.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return testTankResult;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid testTankResultGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			TestTankResultClass testTankResult = Get(security, testTankResultGuid);

			if (testTankResult.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (var cmd = new SqlCommand())
			{
				testTankResult.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		#region IDependency methods
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
		#endregion IDependency methods


		public TestTankResultCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			var testTankResult = new TestTankResultClass();
			
			using (var cmd = new SqlCommand())
			{
				testTankResult.EnumerateSQL(cmd, security);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testTankResultCollection = new TestTankResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testTankResult = new TestTankResultClass();
					testTankResult.Load(set);
					testTankResultCollection.Add(testTankResult);
					table.Rows.RemoveAt(0);
				}

				return testTankResultCollection;
			}
		}


		public TestTankResultCollectionClass EnumerateByTestSetTankResultGuid(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			var testTankResult = new TestTankResultClass();

			using (var cmd = new SqlCommand())
			{
				testTankResult.EnumerateByTestSetTankResultGuidSQL(cmd, security, identityGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testTankResultCollection = new TestTankResultCollectionClass();
				DataTable table = set.Tables[0];
				
				while (table.Rows.Count != 0)
				{
					testTankResult = new TestTankResultClass();
					testTankResult.Load(set);
					testTankResultCollection.Add(testTankResult);
					table.Rows.RemoveAt(0);
				}

				return testTankResultCollection;
			}
		}
	}
}
