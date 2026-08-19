using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	[SecuritySafeCriticalAttribute]
	public class InvoiceQueriesClass : IDependency, IInvoiceQueries
	{
		#region Protected data members
		private ConsolidatedDAClass consolidatedDA;
		#endregion // Protected data members

		#region Constructors
		public InvoiceQueriesClass ( ) 
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
		}
		#endregion // Constructors

		#region Handle dependencies
		void IDependency.Insert ( SecurityClass security, BaseDataObject inObject, bool preOperation )
		{
			// not needed
		}

		void IDependency.Update ( SecurityClass security, BaseDataObject inObject )
		{
			// not needed
		}

		/// <param name="inObject"></param>
		void IDependency.Purge ( SecurityClass security, BaseDataObject inObject )
		{
			// not needed
		}
		#endregion // Handle dependencies

		#region Databasing
		public InvoiceQueryClass GetByIdentityGuid ( SecurityClass security, Guid invoiceQueryGuid )
		{
			DataSet rs;
			using (SqlCommand cmd = new SqlCommand())
			{
				InvoiceQueryClass.EnumerateByIdentityGuid(cmd, invoiceQueryGuid);
				rs = consolidatedDA.GetDataSet(cmd, security);
			}


			DataTable rtable = rs.Tables[0];

			if (0 == rtable.Rows.Count)
			{
				throw new Exception("No results for query " + invoiceQueryGuid.ToString());
			}

			InvoiceQueryClass result = new InvoiceQueryClass ( );
			result.Load ( rtable.Rows[0] );

			return result;
		}

		public InvoiceQueryCollectionClass Enumerate ( SecurityClass security )
		{
			//string sql = InvoiceQueryClass.EnumerateSql ( );
			//return this.EnumerateEx ( security, sql );
			using (SqlCommand cmd = new SqlCommand())
			{
				InvoiceQueryClass.EnumerateSQL(cmd);
				return this.EnumerateExCmd(security, cmd);
			}
		}

		public InvoiceQueryCollectionClass EnumerateByKeyword ( SecurityClass security, string keyword )
		{
			//string sql = InvoiceQueryClass.EnumerateByKeyword ( keyword );

			using (SqlCommand cmd = new SqlCommand())
			{
				InvoiceQueryClass.EnumerateByKeyword(cmd, keyword);
				return this.EnumerateExCmd(security, cmd);
			}
		}


		//moddified EnumerateEx takes SqlCommand argument instead of SQL string variable.
		//and uses the new consolidatedDA.GetDataSet
		protected InvoiceQueryCollectionClass EnumerateExCmd(SecurityClass security, SqlCommand cmd)
		{	
			DataSet ds;
			
			ds = consolidatedDA.GetDataSet(cmd, security);

			InvoiceQueryCollectionClass collection = new InvoiceQueryCollectionClass();

			DataTable dt = ds.Tables[0];
			foreach (DataRow row in dt.Rows)
			{
				InvoiceQueryClass wac = new InvoiceQueryClass();
				wac.Load(row);
				collection.Add(wac);
			}

			return collection;
		}

		#endregion // Databasing
	}
}