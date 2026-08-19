namespace FMBusinessServices.ServiceClasses
{

    using System;
    using System.ServiceModel;
    using System.Data;
    using System.Data.SqlClient;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.BusinessInterfaces;

    using FMBusinessServices.DataAccessLayer;
    using FMBusinessObjects.Exceptions;
    using FMBusinessServices.InternalClasses;

	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class WeightedAverageCostsClass : IWeightedAverageCosts, IDependency
	{
		#region Private data members
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion // Protected data members

		#region Error messages

	    private const string Msg002 = "Security is null";
		private const string Msg003 = "WAC is null";
		private const string Msg005 = "Latest WAC was not found";
		#endregion // Error messages

		#region Constructors
		public WeightedAverageCostsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion // Constructors

		#region Database interaction wrappers
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        
		public void Add(SecurityClass security, WeightedAverageCostClass wac)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException(Msg002);
			}
			if (wac == null)
			{
				throw new ArgumentNullException(Msg003);
			}

			// check that the user has the rights to perform this action
			if (!security.HasRight(RIGHT.VIEW_WAC_HISTORY))
			{
				throw new FMInsufficientRightsException();
			}

			// add the data which user shouldn't have access to
			wac.CreatedBy = security.UserID;
			wac.CreatedDate = DateTimeOffset.Now;
			using (SqlCommand cmd = new SqlCommand())
			{
				wac.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public WeightedAverageCostClass GetByIdentityGuid(SecurityClass security, Guid weightedAverageCostGuid)
		{
			// now get the wac using the index
			using (SqlCommand cmd = new SqlCommand())
			{
				WeightedAverageCostClass.SelectByIdentityGuid(cmd, weightedAverageCostGuid);
				DataSet rs = this.consolidatedDA.GetDataSet(cmd, security);

				DataTable rtable = rs.Tables[0];

				// check that we have results (that we should)
				if (0 == rtable.Rows.Count)
				{
					throw new Exception(Msg005);
				}

				WeightedAverageCostClass result = new WeightedAverageCostClass();
				result.Load(rtable.Rows[0]);

				return result;
			}
		}

		public WeightedAverageCostClass GetLatest(SecurityClass security, Guid siteGuid, Guid productGuid)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				WeightedAverageCostClass.SelectIdentityGuidBySiteProduct(cmd, security, siteGuid, productGuid);

				// execute the query to get all the IDs
				DataSet rs = this.consolidatedDA.GetDataSet(cmd, security);

				DataTable rtable = rs.Tables[0];

				// find the max
				if (0 == rtable.Rows.Count)
				{
					return null;
				}

				// first row should be max
				Guid maxGuid = (Guid)rtable.Rows[0][0];

				WeightedAverageCostClass result = this.GetByIdentityGuid(security, maxGuid);

				return result;
			}
		}
		#endregion // Database interaction wrappers

		#region Handle dependencies
		void IDependency.Insert(SecurityClass security, BaseDataObject inObject, bool preOperation)
		{
			// not needed
		}

		void IDependency.Update(SecurityClass security, BaseDataObject inObject)
		{
			// not needed
		}


		/// <param name="security"></param>
		/// <param name="inObject"></param>
		void IDependency.Purge(SecurityClass security, BaseDataObject inObject)
		{
			// NO!
		}
		#endregion // Handle dependencies

		#region Enumerators
		public WeightedAverageCostCollectionClass Enumerate(SecurityClass security)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				WeightedAverageCostClass.EnumerateSQL(cmd);
				return this.EnumerateEx(security, cmd);
			}
		}

		public WeightedAverageCostCollectionClass EnumerateBySiteProductDate(SecurityClass security, Guid siteGuid, Guid productGuid, DateTimeOffset startDate, DateTimeOffset endDate)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				WeightedAverageCostClass.EnumerateSQLBySiteDateProduct(cmd, security, siteGuid, productGuid, startDate, endDate);
				return this.EnumerateEx(security, cmd);
			}
		}

		protected WeightedAverageCostCollectionClass EnumerateEx(SecurityClass security, SqlCommand cmd)
		{
			// CheckSecurity(security); // TBC

			DataSet ds = consolidatedDA.GetDataSet(cmd, security);
			WeightedAverageCostCollectionClass collection = new WeightedAverageCostCollectionClass();

			// go through our results and add it to our collection
			DataTable dt = ds.Tables[0];
			foreach (DataRow row in dt.Rows)
			{
				WeightedAverageCostClass wac = new WeightedAverageCostClass();
				wac.Load(row);
				collection.Add(wac);
			}

			return collection;
		}
		#endregion // Enumerators
	}
}
