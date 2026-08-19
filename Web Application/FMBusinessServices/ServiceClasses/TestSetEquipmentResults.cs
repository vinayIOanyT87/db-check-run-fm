namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TestSetEquipmentResultsClass : ITestSetEquipmentResults, IDependency
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();
		public TestSetEquipmentResultsClass()
		{
		}

		private void Validate(TestSetEquipmentResultClass testSetEquipmentResult)
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TestSetEquipmentResultClass testSetEquipmentResult)
		{

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testSetEquipmentResult == null)
			{
				throw new ArgumentNullException("testSetEquipmentResult");
			}

			if (!security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS) && !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			this.Validate(testSetEquipmentResult);

			testSetEquipmentResult.SiteGuid = security.SiteGuid;
			testSetEquipmentResult.CreatedDate = DateTimeOffset.Now;
			testSetEquipmentResult.CreatedBy = security.UserID;
			testSetEquipmentResult.UpdatedDate = testSetEquipmentResult.CreatedDate;
			testSetEquipmentResult.UpdatedBy = security.UserID;
			testSetEquipmentResult.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				testSetEquipmentResult.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			// add test results releated to the test set results
			foreach (TestEquipmentResultClass testEquipmentResult in testSetEquipmentResult.TestEquipmentResultCollection)
			{
				testEquipmentResult.TestSetEquipmentResultGuid = testSetEquipmentResult.IdentityGuid;
				var testEquipmentResults = new TestEquipmentResultsClass();
				testEquipmentResults.Add(security, testEquipmentResult);
			}

			return testSetEquipmentResult.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TestSetEquipmentResultClass testSetEquipmentResult)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (testSetEquipmentResult == null)
			{
				throw new ArgumentNullException("testSetEquipmentResult");
			}

			this.Validate(testSetEquipmentResult);
			TestSetEquipmentResultClass oldTestSetEquipmentResult = Get(security, testSetEquipmentResult.IdentityGuid);

			if (oldTestSetEquipmentResult.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("TestSetEquipmentResult Not Found"));
			}

			testSetEquipmentResult.UpdatedDate = DateTimeOffset.Now;
			testSetEquipmentResult.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				testSetEquipmentResult.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var testEquipmentResults = new TestEquipmentResultsClass();
			TestEquipmentResultCollectionClass oldTestEquipmentResultCollection = testEquipmentResults.EnumerateByTestSetEquipmentResultGuid(security, testSetEquipmentResult.IdentityGuid);

			// Delete all old test results related to test set result
			foreach (TestEquipmentResultClass oldTestEquipmentResult in oldTestEquipmentResultCollection)
			{
				testEquipmentResults.Purge(security, oldTestEquipmentResult.TestEquipmentResultGuid);
			}

			// Add all new test results
			foreach (TestEquipmentResultClass newTestEquipmentResult in testSetEquipmentResult.TestEquipmentResultCollection)
			{
				newTestEquipmentResult.TestSetEquipmentResultGuid = testSetEquipmentResult.IdentityGuid;
				testEquipmentResults.Add(security, newTestEquipmentResult);
			}
		}

		public TestSetEquipmentResultClass Get(SecurityClass security, Guid testSetEquipmentResultGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var testSetEquipmentResult = new TestSetEquipmentResultClass { IdentityGuid = testSetEquipmentResultGuid };

			if (testSetEquipmentResultGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					testSetEquipmentResult.SelectSQL(cmd, ContextUtil.IsInTransaction);
					testSetEquipmentResult.Load(ConsolidatedDA.GetDataSet(cmd, security));
				}
			}

			// get the site name from the guid stored in the database
			var sites = new SitesClass();
			SiteClass site = sites.GetByMemberAndProcessVariables(security, testSetEquipmentResult.SiteGuid, false, false);
			testSetEquipmentResult.SiteID = site.ID;

			var testEquipmentResults = new TestEquipmentResultsClass();
			testSetEquipmentResult.TestEquipmentResultCollection = testEquipmentResults.EnumerateByTestSetEquipmentResultGuid(security, testSetEquipmentResultGuid);

			return testSetEquipmentResult;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid testSetEquipmentResultGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			TestSetEquipmentResultClass testSetEquipmentResult = Get(security, testSetEquipmentResultGuid);

			if (testSetEquipmentResult.IdentityGuid == Guid.Empty)
			{
				return;
			}

			// Purge the related test first
			var testEquipmentResults = new TestEquipmentResultsClass();
			TestEquipmentResultCollectionClass testEquipmentResultCollection = testEquipmentResults.EnumerateByTestSetEquipmentResultGuid(security, testSetEquipmentResultGuid);
			
			foreach (TestEquipmentResultClass testEquipmentResult in testEquipmentResultCollection)
			{
				testEquipmentResults.Purge(security, testEquipmentResult.TestEquipmentResultGuid);
			}

			using (var cmd = new SqlCommand())
			{
				testSetEquipmentResult.PurgeSQL(cmd);
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

		public TestSetEquipmentResultCollectionClass Enumerate(SecurityClass security, DateTimeOffset startDate, DateTimeOffset endDate)
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

			var testSetEquipmentResult = new TestSetEquipmentResultClass();

			using (var cmd = new SqlCommand())
			{
				testSetEquipmentResult.EnumerateSQL(cmd, security, startDate, endDate);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testSetEquipmentResultCollection = new TestSetEquipmentResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testSetEquipmentResult = new TestSetEquipmentResultClass();
					testSetEquipmentResult.Load(set);

					testSetEquipmentResultCollection.Add(testSetEquipmentResult);
					table.Rows.RemoveAt(0);
				}

				return testSetEquipmentResultCollection;
			}
		}

		public TestSetEquipmentResultCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();

			var testSetEquipmentResult = new TestSetEquipmentResultClass();

			using (var cmd = new SqlCommand())
			{
				testSetEquipmentResult.EnumerateSQL(cmd, security);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testSetEquipmentResultCollection = new TestSetEquipmentResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testSetEquipmentResult = new TestSetEquipmentResultClass();
					testSetEquipmentResult.Load(set);

					// get the site name from the guid stored in the database
					SiteClass site = sites.GetByMemberAndProcessVariables(security, testSetEquipmentResult.SiteGuid, false, false);
					testSetEquipmentResult.SiteID = site.ID;
					testSetEquipmentResultCollection.Add(testSetEquipmentResult);
					table.Rows.RemoveAt(0);
				}

				return testSetEquipmentResultCollection;
			}
		}

		public TestSetEquipmentResultCollectionClass EnumerateByEquipmentGuid(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.EXECUTE_QUALITY_TESTS)
				&& !security.HasRight(RIGHT.MODIFY_QUALITY_TESTS))
			{
				throw new FMInsufficientRightsException();
			}

			var sites = new SitesClass();
			var testSetEquipmentResult = new TestSetEquipmentResultClass();
			
			using (var cmd = new SqlCommand())
			{
				testSetEquipmentResult.EnumerateByEquipmentGuidSQL(cmd, security, identityGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				var testSetEquipmentResultCollection = new TestSetEquipmentResultCollectionClass();
				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					testSetEquipmentResult = new TestSetEquipmentResultClass();
					testSetEquipmentResult.Load(set);

					// get the site name from the guid stored in the database
					SiteClass site = sites.GetByMemberAndProcessVariables(security, testSetEquipmentResult.SiteGuid, false, false);
					testSetEquipmentResult.SiteID = site.ID;
					testSetEquipmentResultCollection.Add(testSetEquipmentResult);
					table.Rows.RemoveAt(0);
				}

				return testSetEquipmentResultCollection;
			}
		}


		public TestSetEquipmentResultClass GetPreviousSampleNumber(SecurityClass security)
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

			var TestSetEquipmentResult = new TestSetEquipmentResultClass();

			using (var cmd = new SqlCommand())
			{
				TestSetEquipmentResult.GetPreviousSampleNumberSQL(cmd, ContextUtil.IsInTransaction, security.SiteGuid);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				DataTable table = set.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					TestSetEquipmentResult.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
					TestSetEquipmentResult.ResultTimeStamp = DataObject.getValue<DateTimeOffset>(row["ResultTimeStamp"], DateTimeOffset.Now);
					TestSetEquipmentResult.SampleNumber = DataObject.getValue<int>(row["SampleNumber"], 0);
					TestSetEquipmentResult.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				}

				return TestSetEquipmentResult;
			}
		}

		public bool FindDuplicateSampleNumber(SecurityClass security, int sampleNumber, Guid testSetEquipmentResultGuid)
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

			var testSetEquipmentResult = new TestSetEquipmentResultClass();
			
			using (var cmd = new SqlCommand())
			{
				testSetEquipmentResult.FindDuplicateSampleNumberSQL(cmd, ContextUtil.IsInTransaction, security.SiteGuid, sampleNumber);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				DataTable table = set.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];

					testSetEquipmentResult.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
					testSetEquipmentResult.SampleNumber = DataObject.getValue<int>(row["SampleNumber"], 0);
					testSetEquipmentResult.TestSetEquipmentResultGuid = DataObject.getValue<Guid>(row["ResultGuid"], Guid.Empty);
					var asset = DataObject.getValue<string>(row["Asset"], "");

					// if the result guid and the asset are the same it is not a duplicate
					if (testSetEquipmentResult.TestSetEquipmentResultGuid == testSetEquipmentResultGuid && asset == "Equip")
					{
						return false;
					}

					// otherwise, if test set result is not the same return a duplicate found
					return true;
				}

				return false;
			}
		}

		public string DetailPageReference()
		{


			var security = new SecurityClass { SiteGuid = Guids.SiteAdminGuid };
			security.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
			security.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
			security.AddRight(RIGHT.VIEW_USERS);
			security.AddRight(RIGHT.MODIFY_USERS);
			security.AddRight(RIGHT.VIEW_USER_GROUPS);
			security.AddRight(RIGHT.MODIFY_USER_GROUPS);
			security.AddRight(RIGHT.VIEW_EQUIPMENT_DATA);
			security.AddRight(RIGHT.MODIFY_EQUIPMENT_DATA);

			string serviceLogin =
				FMChannelHelper.MakeCall<IDBAccess, string>(dbAccessChannel => dbAccessChannel.ServiceLogin(security));

			security.UserID = serviceLogin;

			string testSetResultFormUrl =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(security, "TestSetResultFormURL"));

			if (string.IsNullOrEmpty(testSetResultFormUrl))
			{
				testSetResultFormUrl = "QualityControlWebApp\\TestSetResultForm.aspx";
			}

			return testSetResultFormUrl;
		}
	}
}
