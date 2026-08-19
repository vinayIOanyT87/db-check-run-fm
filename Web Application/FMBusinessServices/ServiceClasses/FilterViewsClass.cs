using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Xml;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class FilterViewsClass : IDependency, IFilterViews
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion // Protected data members

		#region Constructors
		public FilterViewsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion // Constructors

		#region Database interaction wrappers
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, FilterViewClass filter)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("Security  not found");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("Missing method parametre");
			}

			// add the data which user shouldn't have access to
			filter.CreatedBy = security.UserID;
			filter.CreatedDate = DateTimeOffset.Now;
			filter.UpdatedBy = filter.CreatedBy;
			filter.UpdatedDate = filter.CreatedDate;

			using (SqlCommand cmd = new SqlCommand())
			{
				filter.InsertSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Update(SecurityClass security, FilterViewClass filter)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("Security  not found");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("Missing method parametre");
			}

			// add the data which user shouldn't have access to
			filter.UpdatedBy = security.UserID;
			filter.UpdatedDate = DateTimeOffset.Now;

			using (SqlCommand cmd = new SqlCommand())
			{
				filter.UpdateSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, FilterViewClass filter)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("Security  not found");
			}
			if (filter == null)
			{
				throw new ArgumentNullException("Missing method parametre");
			}

			using (SqlCommand cmd = new SqlCommand())
			{
				filter.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public FilterViewClass GetByIdentityGuid(SecurityClass security, Guid filterViewGuid)
		{
			// now get the wac using the guid
			DataSet rs = null;

			using (SqlCommand cmd = new SqlCommand())
			{
				FilterViewClass.SelectByIdentityGuid(cmd, filterViewGuid);
				rs = this.consolidatedDA.GetDataSet(cmd, security);
			}

			DataTable rtable = rs.Tables[0];

			// check that we have results (that we should)
			if (0 == rtable.Rows.Count)
			{
				throw new Exception("No results found");
			}

			FilterViewClass result = new FilterViewClass();
			result.Load(rtable.Rows[0]);

			return result;
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

		/// <param name="inObject"></param>
		void IDependency.Purge(SecurityClass security, BaseDataObject inObject)
		{
			// not needed
		}
		#endregion // Handle dependencies

		#region Enumerators
		public FilterViewsCollectionClass Enumerate(SecurityClass security)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				FilterViewClass.EnumerateSQL(cmd);
				return this.EnumerateEx(security, cmd);
			}
		}

		public FilterViewsCollectionClass EnumerateByTransTypeID(SecurityClass security, TransactionTypes type)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				FilterViewClass.EnumerateByTransTypeID(cmd, type);
				return this.EnumerateEx(security, cmd);
			}
		}

		protected FilterViewsCollectionClass EnumerateEx(SecurityClass security, SqlCommand cmd)
		{
			DataSet ds = consolidatedDA.GetDataSet(cmd, security);

			FilterViewsCollectionClass collection = new FilterViewsCollectionClass();

			// go through our results and add it to our collection
			DataTable dt = ds.Tables[0];
			foreach (DataRow row in dt.Rows)
			{
				FilterViewClass filterView = new FilterViewClass();
				filterView.Load(row);
				collection.Add(filterView);
			}

			return collection;
		}
		#endregion // Enumerators
	}
}