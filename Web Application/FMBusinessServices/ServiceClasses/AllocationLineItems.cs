using System;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	/// <summary>
	/// Summary description for AllocationLineItemsClass.
	/// </summary>
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class AllocationLineItemsClass : IAllocationLineItems
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();


		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, AllocationLineItemClass allocationLineItem)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (allocationLineItem == null)
				throw new ArgumentNullException("allocationLineItem");

			if (!security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
				throw new FMInsufficientRightsException();

			if (GetIdentityGuid(security,
				allocationLineItem.AllocationGuid,
				allocationLineItem.Type,
				allocationLineItem.AssignedGuid,
				allocationLineItem.ResetPeriod) != Guid.Empty)
				throw (new Exception("Item Exists"));

			allocationLineItem.SiteGuid = security.SiteGuid;
			allocationLineItem.CreatedDate = DateTimeOffset.Now;
			allocationLineItem.CreatedBy = security.UserID;
			allocationLineItem.UpdatedDate = allocationLineItem.CreatedDate;
			allocationLineItem.UpdatedBy = security.UserID;
			allocationLineItem.IdentityGuid = Guid.NewGuid();

			using (SqlCommand cmd = new SqlCommand())
			{
				allocationLineItem.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			return allocationLineItem.IdentityGuid;
		}

		public Guid GetIdentityGuid(SecurityClass security,
									Guid allocationGuid,
									ALLOCATION_TYPE type,
									Guid assignedGuid,
									ALLOCATION_RESET_PERIOD resetPeriod)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS)
			&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
				throw new FMInsufficientRightsException();

			var site = new SiteClass();
			var allocationLineItem = new AllocationLineItemClass(site);
			allocationLineItem.AllocationGuid = allocationGuid;
			allocationLineItem.Type = type;
			allocationLineItem.ResetPeriod = resetPeriod;
			allocationLineItem.AssignedGuid = assignedGuid;
			allocationLineItem.SiteGuid = security.SiteGuid;
			DataSet set;

			using (var cmd = new SqlCommand())
			{
				allocationLineItem.SelectIdentityGuidSQL(cmd, ContextUtil.IsInTransaction);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

		    if (set.Tables.Count == 1 && set.Tables[0].Rows.Count == 1)
		    {
		        return (Guid)set.Tables[0].Rows[0][0];
		    }

		    return Guid.Empty;
		}

	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
	    public void Modify(SecurityClass security, AllocationLineItemClass allocationLineItem)
	    {
	        if (security == null)
	        {

	            throw new ArgumentNullException("security");
	        }

	        if (allocationLineItem == null)
	        {
	            throw new ArgumentNullException("allocationLineItem");
	        }


	        if (!security.HasRight(RIGHT.MODIFY_ALLOCATIONS)) throw new FMInsufficientRightsException();

	        Guid identityGuid = GetIdentityGuid(
	            security,
	            allocationLineItem.AllocationGuid,
	            allocationLineItem.Type,
	            allocationLineItem.AssignedGuid,
	            allocationLineItem.ResetPeriod);

	        if (identityGuid != Guid.Empty && identityGuid != allocationLineItem.IdentityGuid) throw (new Exception("ItemExists"));

	        AllocationLineItemClass oldAllocationLineItem = Get(security, allocationLineItem.IdentityGuid);
	        if (oldAllocationLineItem.IdentityGuid == Guid.Empty) throw (new Exception("Item Not Found"));

	        allocationLineItem.UpdatedBy = security.UserID;
	        allocationLineItem.UpdatedDate = DateTimeOffset.Now;

	        using (var cmd = new SqlCommand())
	        {
	            allocationLineItem.UpdateSQL(cmd);
	            ConsolidatedDA.ExecuteQuery(security, cmd);
	        }
	    }


	    [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid allocationLineItemGuid)
		{
	        if (security == null)
	        {
	            throw new ArgumentNullException("security");
	        }

	        if (!security.HasRight(RIGHT.MODIFY_ALLOCATIONS) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
	            && !security.HasRight(RIGHT.MODIFY_PRODUCTS) && !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
	        {
	            throw new FMInsufficientRightsException();
	        }

	        AllocationLineItemClass allocationLineItem = Get(security, allocationLineItemGuid);
	        if (allocationLineItem.IdentityGuid == Guid.Empty)
	        {
	            throw (new Exception("Item Not Found"));
	        }

	        using (var cmd = new SqlCommand())
			{
				allocationLineItem.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

		}

		public AllocationLineItemCollectionClass EnumerateByAllocationGuid(SecurityClass security,
																									Guid allocationGuid)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS)
			&& !security.HasRight(RIGHT.MODIFY_ALLOCATIONS)
			&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
			&& !security.HasRight(RIGHT.MODIFY_DISPATCH)
			&& !security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA))
				throw new FMInsufficientRightsException();

			var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			var allocationLineItem = new AllocationLineItemClass(site);
			allocationLineItem.AllocationGuid = allocationGuid;
			allocationLineItem.SiteGuid = security.SiteGuid;

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				allocationLineItem.EnumerateByAllocationGuidSQL(cmd);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			var allocationCollection = new AllocationLineItemCollectionClass();

			DataTable table = set.Tables[0];
			while (table.Rows.Count != 0)
			{
				allocationLineItem = new AllocationLineItemClass(site);
				allocationLineItem.Load(set);
				allocationCollection.Add(allocationLineItem);
				table.Rows.RemoveAt(0);
			}

			return allocationCollection;

		}

	    public AllocationLineItemClass Get(SecurityClass security, Guid identityGuid)
	    {
	        if (security == null)
	        {
	            throw new ArgumentNullException("security");
	        }

	        if (!security.HasRight(RIGHT.VIEW_ALLOCATIONS) && !security.HasRight(RIGHT.MODIFY_ALLOCATIONS))
	        {
	            throw new FMInsufficientRightsException();
	        }

	        var sites = new SitesClass();
			SiteClass site = sites.GetUsingGuid(security, security.SiteGuid);

			var allocationLineItem = new AllocationLineItemClass(site);
			allocationLineItem.IdentityGuid = identityGuid;

			using (var cmd = new SqlCommand())
			{
				allocationLineItem.SelectSQL(cmd, ContextUtil.IsInTransaction);
				allocationLineItem.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return allocationLineItem;
		}

		public double GetAmountLoaded(
			SecurityClass security,
			string allocationID,
			Guid itemGuid,
			ALLOCATION_TYPE allocationType,
			ALLOCATION_RESET_PERIOD resetPeriod,
			int resetMultiple,
			DateTimeOffset resetDate,
			DateTimeOffset lastAllocationResetDate,
			DateTimeOffset expirationDate,
			STATION_TYPE stationType,
			string transactionID)
		{
			return this.GetAmountLoaded(security,
													allocationID,
													itemGuid,
													allocationType,
													resetPeriod,
													resetMultiple,
													resetDate,
													lastAllocationResetDate,
													expirationDate,
													security.SiteGuid,
										stationType,
										transactionID);
		}


		public double GetAmountLoaded(SecurityClass security,
												string allocationID,
												Guid itemGuid,
												ALLOCATION_TYPE allocationType,
												ALLOCATION_RESET_PERIOD resetPeriod,
												int resetMultiple,
												DateTimeOffset resetDate,
												DateTimeOffset lastAllocationResetDate,
												DateTimeOffset expirationDate,
												Guid siteGuid,
									STATION_TYPE stationType,
									string transactionID)
		{
			double loaded = 0.0;

			string[] names = this.ParseNames(allocationID);

			string managerID = "";
			string ownerID = "";
			string shipperID = "";
			string billToID = "";
			string shipToID = "";

			if (names != null && names.Length > 0)
			{
				managerID = names[0];
				if (names.Length > 1)
				{
					ownerID = names[1];
					if (names.Length > 2)
					{
						shipperID = names[2];
						if (names.Length > 3)
						{
							billToID = names[3];
							if (names.Length > 4)
								shipToID = names[4];
						}
					}
				}
			}

			DateTimeOffset ending;
			DateTimeOffset beginning = ending = resetDate;

			switch (resetPeriod)
			{
				case ALLOCATION_RESET_PERIOD.DAY_RESET_PERIOD:
					ending = ending.AddDays(resetMultiple);
					break;
				case ALLOCATION_RESET_PERIOD.WEEK_RESET_PERIOD:
					int maxCheck = Int32.MaxValue / 7;
					if (resetMultiple >= maxCheck)
					{
						throw new ApplicationException(string.Format("Reset value must be less than {0}.", maxCheck));
					}
					ending = ending.AddDays(resetMultiple * 7);
					break;
				case ALLOCATION_RESET_PERIOD.MONTH_RESET_PERIOD:
					ending = ending.AddMonths(resetMultiple);
					break;
				case ALLOCATION_RESET_PERIOD.YEAR_RESET_PERIOD:
					ending = ending.AddYears(resetMultiple);
					break;
				default:
					ending = expirationDate;
					break;
			}


			if (ending > expirationDate)
				ending = expirationDate;

			if (beginning < lastAllocationResetDate)
				beginning = lastAllocationResetDate;

			var allocationLineItem = new AllocationLineItemClass();

			DataSet set;

			using (var cmd = new SqlCommand())
			{
				allocationLineItem.AmountLoadedSQL(cmd,
																beginning,
																ending,
																managerID,
																ownerID,
																shipperID,
																billToID,
																shipToID,
																itemGuid,
																allocationType,
																stationType,
																transactionID,
																siteGuid);

				set = ConsolidatedDA.GetDataSet(cmd, security);
			}
			loaded = (set.Tables[0].Rows[0].IsNull(0)) ? 0.0 : (double)set.Tables[0].Rows[0][0];

			return loaded;
		}


		private string[] ParseNames(string allocationID)
		{
			const string Delimeters = "`!@#$%^&*()_+=1234567890";

			// look for a delimeter we can safely use
			for (int nLoop = 0; nLoop < Delimeters.Length; ++nLoop)
			{
				string delimeter = Delimeters.Substring(nLoop, 1);

				// If we find a delimeter we can use...
				if (allocationID.IndexOf(delimeter) == -1)
				{
					// Replace the standard separator and then split the string up with the new delimeter.
					return allocationID.Replace("->", delimeter).Split(delimeter[0]);
				}

			}

			return null;

		}

	}

}
