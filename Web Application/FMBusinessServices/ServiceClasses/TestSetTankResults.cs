namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TestSetTankResultsClass : ITestSetTankResults, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TestSetTankResultsClass()
		{
		}

		private void Validate(TestSetTankResultClass testSetTankResult)
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TestSetTankResultClass testSetTankResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testSetTankResult == null)
			{
				throw new ArgumentNullException("testSetTankResult");
			}

			if (!security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) &&
				 !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
				throw new FMInsufficientRightsException();

			Validate(testSetTankResult);

			testSetTankResult.SiteGuid = security.SiteGuid;
			testSetTankResult.CreatedDate = DateTimeOffset.Now;
			testSetTankResult.CreatedBy = security.UserID;
			testSetTankResult.UpdatedDate = testSetTankResult.CreatedDate;
			testSetTankResult.UpdatedBy = security.UserID;
			testSetTankResult.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				testSetTankResult.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);

				// add test results releated to the test set results
				foreach (TestTankResultClass testTankResult in testSetTankResult.TestTankResultCollection)
				{
					testTankResult.TestSetTankResultGuid = testSetTankResult.IdentityGuid;
					var testTankResults = new TestTankResultsClass();
					testTankResults.Add(security, testTankResult);
				}

				return testSetTankResult.IdentityGuid;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TestSetTankResultClass testSetTankResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testSetTankResult == null)
			{
				throw new ArgumentNullException("testSetTankResult");
			}

			this.Validate(testSetTankResult);

			TestSetTankResultClass oldTestSetTankResult = Get(security, testSetTankResult.IdentityGuid);

			if (oldTestSetTankResult.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("TestSetTankResult Not Found"));
			}

			testSetTankResult.UpdatedDate = DateTimeOffset.Now;
			testSetTankResult.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				testSetTankResult.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var testTankResults = new TestTankResultsClass();
			TestTankResultCollectionClass oldTestTankResultCollection = testTankResults.EnumerateByTestSetTankResultGuid(security, testSetTankResult.IdentityGuid);

			// Delete all old test results related to test set result
			foreach (TestTankResultClass oldTestTankResult in oldTestTankResultCollection)
			{
				testTankResults.Purge(security, oldTestTankResult.TestTankResultGuid);
			}

			// Add all new test results
			foreach (TestTankResultClass newTestTankResult in testSetTankResult.TestTankResultCollection)
			{
				newTestTankResult.TestSetTankResultGuid = testSetTankResult.IdentityGuid;
				testTankResults.Add(security, newTestTankResult);
			}
		}

		public TestSetTankResultClass Get(SecurityClass security, Guid testSetTankResultGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			var testSetTankResult = new TestSetTankResultClass { IdentityGuid = testSetTankResultGuid };

			if (testSetTankResultGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					testSetTankResult.SelectSQL(cmd, ContextUtil.IsInTransaction);
					testSetTankResult.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			// get the site name from the guid stored in the database
			var sites = new SitesClass();
			SiteClass Site;
			Site = sites.GetByMemberAndProcessVariables(security, testSetTankResult.SiteGuid, false, false);
			testSetTankResult.SiteID = Site.ID;

			var testTankResults = new TestTankResultsClass();
			testSetTankResult.TestTankResultCollection = testTankResults.EnumerateByTestSetTankResultGuid(security, testSetTankResultGuid);

			return testSetTankResult;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid testSetTankResultGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			TestSetTankResultClass testSetTankResult = Get(security, testSetTankResultGuid);

			if (testSetTankResult.IdentityGuid == Guid.Empty)
			{
				return;
			}

			// Purge the related test first
			var testTankResults = new TestTankResultsClass();
			TestTankResultCollectionClass testTankResultCollection = testTankResults.EnumerateByTestSetTankResultGuid(security, testSetTankResultGuid);

			foreach (TestTankResultClass testTankResult in testTankResultCollection)
			{
				testTankResults.Purge(security, testTankResult.TestTankResultGuid);
			}

			using (var cmd = new SqlCommand())
			{
				testSetTankResult.PurgeSQL(cmd);
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

		public TestSetTankResultCollectionClass EnumerateByDates(SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (startDate == null)
			{
				throw new ArgumentNullException("startDate");
			}

			if (endDate == null)
			{
				throw new ArgumentNullException("endDate");
			}

			if (!security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			var testSetTankResult = new TestSetTankResultClass();

			using (SqlCommand cmd = new SqlCommand())
			{
				testSetTankResult.EnumerateSQL(cmd, security, startDate, endDate);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testSetTankResultCollection = new TestSetTankResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testSetTankResult = new TestSetTankResultClass();
					testSetTankResult.Load(set);

					testSetTankResultCollection.Add(testSetTankResult);
					table.Rows.RemoveAt(0);
				}

				return testSetTankResultCollection;
			}
		}

		public TestSetTankResultCollectionClass Enumerate(SecurityClass security)
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

			var sites = new SitesClass();
			var testSetTankResult = new TestSetTankResultClass();

			using (var cmd = new SqlCommand())
			{
				testSetTankResult.EnumerateSQL(cmd, security);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testSetTankResultCollection = new TestSetTankResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testSetTankResult = new TestSetTankResultClass();
					testSetTankResult.Load(set);

					// get the site name from the guid stored in the database
					SiteClass site = sites.GetByMemberAndProcessVariables(security, testSetTankResult.SiteGuid, false, false);
					testSetTankResult.SiteID = site.ID;
					testSetTankResultCollection.Add(testSetTankResult);
					table.Rows.RemoveAt(0);
				}

				return testSetTankResultCollection;
			}
		}

		public TestSetTankResultCollectionClass EnumerateByTankGuid(SecurityClass security, Guid identityGuid)
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

			var sites = new SitesClass();
			var testSetTankResult = new TestSetTankResultClass();

			using (var cmd = new SqlCommand())
			{
				testSetTankResult.EnumerateByTankGuidSQL(cmd, security, identityGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testSetTankResultCollection = new TestSetTankResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testSetTankResult = new TestSetTankResultClass();
					testSetTankResult.Load(set);

					// get the site name from the guid stored in the database
					SiteClass site = sites.GetByMemberAndProcessVariables(security, testSetTankResult.SiteGuid, false, false);
					testSetTankResult.SiteID = site.ID;
					testSetTankResultCollection.Add(testSetTankResult);
					table.Rows.RemoveAt(0);
				}

				return testSetTankResultCollection;
			}
		}

		public TestSetTankResultClass GetPreviousSampleNumber(SecurityClass security)
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

			var testSetTankResult = new TestSetTankResultClass();

			using (var cmd = new SqlCommand())
			{
				testSetTankResult.GetPreviousSampleNumberSQL(cmd, ContextUtil.IsInTransaction, security.SiteGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				DataTable table = set.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					testSetTankResult.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
					testSetTankResult.ResultTimeStamp = DataObject.getValue<DateTimeOffset>(row["ResultTimeStamp"], DateTimeOffset.Now);
					testSetTankResult.SampleNumber = DataObject.getValue<int>(row["SampleNumber"], 0);
					testSetTankResult.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				}

				return testSetTankResult;
			}
		}

		public bool FindDuplicateSampleNumber(SecurityClass security, int sampleNumber, Guid testSetTankResultGuid)
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

			var testSetTankResult = new TestSetTankResultClass();

			using (var cmd = new SqlCommand())
			{
				testSetTankResult.FindDuplicateSampleNumberSQL(cmd, ContextUtil.IsInTransaction, security.SiteGuid, sampleNumber);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				DataTable table = set.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					testSetTankResult.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
					testSetTankResult.SampleNumber = DataObject.getValue<int>(row["SampleNumber"], 0);
					testSetTankResult.TestSetTankResultGuid = DataObject.getValue<Guid>(row["ResultGuid"], Guid.Empty);
					var asset = DataObject.getValue<string>(row["Asset"], "");

					// if the result guid and the asset are the same it is not a duplicate
					if (testSetTankResult.TestSetTankResultGuid == testSetTankResultGuid && asset == "Tank")
					{
						return false;
					}

					// otherwise, if test set result is not the same return a duplcate found
					return true;
				}
			}

			return false;
		}
	}
}
