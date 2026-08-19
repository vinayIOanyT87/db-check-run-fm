namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TestSetsClass : ITestSets, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TestSetsClass()
		{
		}

		private void Validate(TestSetClass testSet)
		{
			if (testSet.ID.Trim().Length == 0)
			{
				throw (new Exception("Name Required"));
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TestSetClass testSet)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testSet == null)
			{
				throw new ArgumentNullException("testSet");
			}

			if (!security.HasRight(RIGHT.MODIFY_TEST_ITEMS))
			{
				throw new FMInsufficientRightsException();
			} 

			if (GetIdentityGuid(security, testSet.ID) != Guid.Empty)
			{
				throw new Exception("TestSet Exists.");
			}

			Validate(testSet);

			testSet.CreatedDate = DateTimeOffset.Now;
			testSet.CreatedBy = security.UserID;
			testSet.UpdatedDate = testSet.CreatedDate;
			testSet.UpdatedBy = security.UserID;
			testSet.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				testSet.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var testToTestSetMaps = new TestToTestSetMapsClass();

			for (int inxNew = 0; inxNew < testSet.testCollection.Count; inxNew++)
			{
				Guid newTestGuid = testSet.testCollection.ElementAt(inxNew).IdentityGuid;

				//Add new mapping.
				var testToTestSetMap = new TestToTestSetMapClass
				                       {
					                       TestDefinitionGuid = newTestGuid,
					                       TestSetDefinitionGuid = testSet.IdentityGuid
				                       };

				testToTestSetMaps.Add(security, testToTestSetMap);
			}

			// Create Entity to Site Map
			var entityToSiteMaps = new EntityToSiteMaps();
			var entityToSiteMap = new EntityToSiteMapClass(testSet);
			entityToSiteMaps.Add(security, entityToSiteMap, GetType().GUID);

			return testSet.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TestSetClass testSet)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testSet == null)
			{
				throw new ArgumentNullException("testSet");
			}

			Validate(testSet);

			TestSetClass oldTestSet = Get(security, testSet.IdentityGuid);

			if (oldTestSet.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("Test Set Not Found"));
			}

			testSet.UpdatedDate = DateTimeOffset.Now;
			testSet.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				testSet.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var testToTestSetMaps = new TestToTestSetMapsClass();
			TestToTestSetMapCollectionClass oldTestToTestSetMapCollection = testToTestSetMaps.EnumerateByTestSetGuid(security, testSet.IdentityGuid);

			for (int inxNew = 0; inxNew < testSet.testCollection.Count; inxNew++)
			{
				bool found = false;
				Guid newTestGuid = testSet.testCollection.ElementAt(inxNew).IdentityGuid;

				for (int inxOld = 0; inxOld < oldTestToTestSetMapCollection.Count; inxOld++)
				{
					if (oldTestToTestSetMapCollection.ElementAt(inxOld).TestDefinitionGuid == newTestGuid)
					{
						//Keep old mapping. Removed from collection here so that when purging is done in the next loop it doesn't get purged.
						oldTestToTestSetMapCollection.RemoveAt(inxOld);
						found = true;
						break;
					}
				}

				if (!found)
				{
					//Add new mapping.
					var testToTestSetMap = new TestToTestSetMapClass
					                       {
						                       TestDefinitionGuid = newTestGuid,
						                       TestSetDefinitionGuid = testSet.IdentityGuid
					                       };

					testToTestSetMaps.Add(security, testToTestSetMap);
				}
			}

			//Remove mappings that are no longer used.
			for (int inxOld = 0; inxOld < oldTestToTestSetMapCollection.Count; inxOld++)
			{
				testToTestSetMaps.Purge(security, oldTestToTestSetMapCollection.ElementAt(inxOld).TestDefinitionToTestSetDefinitionGuid);
			}

			var entityToSiteMaps = new EntityToSiteMaps();
			EntityToSiteMapCollectionClass entityToSiteMapCollection = entityToSiteMaps.EnumerateByTypeIDAndGuid(security, testSet.EntityType, testSet.IdentityGuid);

			if (testSet.SiteGuid != oldTestSet.SiteGuid)
			{
				// Purge from EntityToSiteMap
				foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
				{
					entityToSiteMaps.Purge(security, entityToSiteMap);
				}

				// Create Entity to Site Map
				var newEntityToSiteMap = new EntityToSiteMapClass(testSet);
				entityToSiteMaps.Add(security, newEntityToSiteMap, GetType().GUID);
			}
		}

		public TestSetClass Get(SecurityClass security, Guid testSetGuid)
		{
			return this.GetByIncludeTests(security, testSetGuid, false);
		}

		public TestSetClass GetByIncludeTests(SecurityClass security, Guid testSetGuid, bool includeTests)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var testSet = new TestSetClass { IdentityGuid = testSetGuid };

			if (testSetGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					testSet.SelectSQL(cmd, ContextUtil.IsInTransaction);
					testSet.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}

				if (includeTests)
				{
					var tests = new TestsClass();
					testSet.testCollection = tests.EnumerateByTestSetGuid(security, testSetGuid);
				}
			}

			return testSet;
		}

		public Guid GetIdentityGuid(SecurityClass security, string id)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			//   CheckSecurity(security);

			if (id == "{All}" || id == "{Unassigned}" || id == "{None}")
			{
				return Guid.Empty;
			}

			var testSet = new TestSetClass();
			testSet.ID = id;

			using (var cmd = new SqlCommand())
			{
				testSet.SelectByIDSQL(cmd, security, ContextUtil.IsInTransaction);
				testSet.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}
			
			return testSet.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid testSetGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			TestSetClass testSet = Get(security, testSetGuid);
			if (testSet.IdentityGuid == Guid.Empty)
			{
				return;
			}

            // Purge from EntityToSiteMap
            var entityToSiteMaps = new EntityToSiteMaps();
            EntityToSiteMapCollectionClass entityToSiteMapCollection =
										entityToSiteMaps.EnumerateByTypeIDAndGuid(security, testSet.EntityType, testSetGuid);

            foreach (EntityToSiteMapClass entityToSiteMap in entityToSiteMapCollection)
            {
                entityToSiteMaps.Purge(security, entityToSiteMap);
            }

			using (var cmd = new SqlCommand())
			{
				testSet.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, testSet);
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

		public TestSetCollectionClass Enumerate(SecurityClass security, string filter, string order)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TEST_ITEMS)
				&& !security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
				&& !security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			var testSet = new TestSetClass();

			using (var cmd = new SqlCommand())
			{
				testSet.EnumerateSQL(cmd, security, filter, order);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testSetCollection = new TestSetCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testSet = new TestSetClass();
					testSet.Load(set);
					testSetCollection.Add(testSet);
					table.Rows.RemoveAt(0);
				}

				return testSetCollection;
			}
		}
	}
}
