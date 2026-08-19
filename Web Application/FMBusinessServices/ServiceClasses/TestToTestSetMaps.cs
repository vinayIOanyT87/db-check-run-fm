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
	public class TestToTestSetMapsClass : ITestToTestSetMaps, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TestToTestSetMapsClass()
		{
		}

		private void Validate(TestToTestSetMapClass TestToTestSetMap)
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TestToTestSetMapClass testToTestSetMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testToTestSetMap == null)
			{
				throw new ArgumentNullException("testToTestSetMap");
			}

			Validate(testToTestSetMap);

			testToTestSetMap.CreatedDate = DateTimeOffset.Now;
			testToTestSetMap.CreatedBy = security.UserID;
			testToTestSetMap.UpdatedDate = testToTestSetMap.CreatedDate;
			testToTestSetMap.UpdatedBy = security.UserID;
			testToTestSetMap.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				testToTestSetMap.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
				return testToTestSetMap.IdentityGuid;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TestToTestSetMapClass testToTestSetMap)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testToTestSetMap == null)
			{
				throw new ArgumentNullException("testToTestSetMap");
			}

			Validate(testToTestSetMap);

			TestToTestSetMapClass oldTestToTestSetMap = Get(security, testToTestSetMap.IdentityGuid);
			if (oldTestToTestSetMap.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("TestToTestSetMap Not Found"));
			}

			testToTestSetMap.UpdatedDate = DateTimeOffset.Now;
			testToTestSetMap.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				testToTestSetMap.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public TestToTestSetMapClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var testToTestSetMap = new TestToTestSetMapClass();
			testToTestSetMap.IdentityGuid = identityGuid;

			if (identityGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					testToTestSetMap.SelectSQL(cmd, ContextUtil.IsInTransaction);
					testToTestSetMap.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return testToTestSetMap;
		}

		public TestToTestSetMapClass GetByDefinition(SecurityClass security, Guid testDefinitionGuid, Guid testSetDefinitionGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var testToTestSetMap = new TestToTestSetMapClass();

			if (testSetDefinitionGuid != Guid.Empty && testDefinitionGuid != Guid.Empty)
			{
				testToTestSetMap.TestSetDefinitionGuid = testSetDefinitionGuid;
				testToTestSetMap.TestDefinitionGuid = testDefinitionGuid;

				using (var cmd = new SqlCommand())
				{
					testToTestSetMap.SelectByTestAndTestSetGuidSQL(cmd, ContextUtil.IsInTransaction);
					testToTestSetMap.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return testToTestSetMap;
		}

		public Guid GetIdentityGuid(SecurityClass security, Guid testDefinitionGuid, Guid testSetDefinitionGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var testToTestSetMap = new TestToTestSetMapClass();

			if (testSetDefinitionGuid != Guid.Empty && testDefinitionGuid != Guid.Empty)
			{
				testToTestSetMap.TestSetDefinitionGuid = testSetDefinitionGuid;
				testToTestSetMap.TestDefinitionGuid = testDefinitionGuid;

				using (var cmd = new SqlCommand())
				{
					testToTestSetMap.SelectByTestAndTestSetGuidSQL(cmd, ContextUtil.IsInTransaction);
					testToTestSetMap.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			return testToTestSetMap.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid testDefinitionToTestSetDefinitionGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			TestToTestSetMapClass testToTestSetMap = Get(security, testDefinitionToTestSetDefinitionGuid);
			
			if (testToTestSetMap.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (var cmd = new SqlCommand())
			{
				testToTestSetMap.PurgeSQL(cmd);
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


			if (Object is TestClass)
			{
				var test = (TestClass) Object;
				TestToTestSetMapCollectionClass testToTestSetMapCollection = this.EnumerateByTestGuid(security, test.IdentityGuid);
				var testToTestSetMaps = new TestToTestSetMapsClass();

				foreach (TestToTestSetMapClass testToTestSetMap in testToTestSetMapCollection)
				{
					testToTestSetMaps.Purge(security, testToTestSetMap.IdentityGuid);
				}

			}
			else if (Object is TestSetClass)
			{
				var testSet = (TestSetClass) Object;
				TestToTestSetMapCollectionClass testToTestSetMapCollection = this.EnumerateByTestSetGuid(security, testSet.IdentityGuid);
				var testToTestSetMaps = new TestToTestSetMapsClass();

				foreach (TestToTestSetMapClass testToTestSetMap in testToTestSetMapCollection)
				{
					testToTestSetMaps.Purge(security, testToTestSetMap.IdentityGuid);
				}
			}
		}

		public TestToTestSetMapCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TEST_ITEMS) && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS))
			{
				throw new FMInsufficientRightsException();
			}

			var testToTestSetMap = new TestToTestSetMapClass();

			using (var cmd = new SqlCommand())
			{
				testToTestSetMap.EnumerateSQL(cmd, security);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testToTestSetMapCollection = new TestToTestSetMapCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testToTestSetMap = new TestToTestSetMapClass();
					testToTestSetMap.Load(set);
					testToTestSetMapCollection.Add(testToTestSetMap);
					table.Rows.RemoveAt(0);
				}

				return testToTestSetMapCollection;
			}
		}

		public TestToTestSetMapCollectionClass EnumerateByTestSetGuid(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TEST_ITEMS) && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS))
			{
				throw new FMInsufficientRightsException();
			}

			var testToTestSetMap = new TestToTestSetMapClass();
			
			using (var cmd = new SqlCommand())
			{
				testToTestSetMap.EnumerateByTestSetGuidSQL(cmd, security, identityGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testToTestSetMapCollection = new TestToTestSetMapCollectionClass();
				DataTable table = set.Tables[0];
				
				while (table.Rows.Count != 0)
				{
					testToTestSetMap = new TestToTestSetMapClass();
					testToTestSetMap.Load(set);
					testToTestSetMapCollection.Add(testToTestSetMap);
					table.Rows.RemoveAt(0);
				}

				return testToTestSetMapCollection;
			}
		}

		public TestToTestSetMapCollectionClass EnumerateByTestGuid(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TEST_ITEMS) && !security.HasRight(RIGHT.MODIFY_TEST_ITEMS))
			{
				throw new FMInsufficientRightsException();
			}

			var testToTestSetMap = new TestToTestSetMapClass();
			
			using (var cmd = new SqlCommand())
			{
				testToTestSetMap.EnumerateByTestGuidSQL(cmd, security, identityGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testToTestSetMapCollection = new TestToTestSetMapCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testToTestSetMap = new TestToTestSetMapClass();
					testToTestSetMap.Load(set);
					testToTestSetMapCollection.Add(testToTestSetMap);
					table.Rows.RemoveAt(0);
				}

				return testToTestSetMapCollection;
			}
		}
	}
}
