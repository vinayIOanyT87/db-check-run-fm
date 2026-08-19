// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkPayments.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the BulkPaymentsClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// The bulk payments class.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class BulkPaymentsClass : IDependency, IBulkPayments
	{
		#region Protected data members
		/// <summary>
		/// The consolidated data layer.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;
		#endregion // Protected data members

		#region Construction
		/// <summary>
		/// Initializes a new instance of the <see cref="BulkPaymentsClass"/> class.
		/// </summary>
		public BulkPaymentsClass()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion // Construction

		#region Database interaction wrappers

		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="payment">
		/// The payment.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, BulkPaymentClass payment)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (payment == null)
			{
				throw new ArgumentNullException("payment");
			}

			// add the data which user shouldn't have access to
			payment.CreatedBy = security.UserID;
			payment.CreatedDate = DateTimeOffset.Now;
			payment.UpdatedBy = payment.CreatedBy;
			payment.UpdatedDate = payment.CreatedDate;

			using (var cmd = new SqlCommand())
			{
				payment.InsertSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// The update.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="payment">
		/// The payment.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Update(SecurityClass security, BulkPaymentClass payment)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (payment == null)
			{
				throw new ArgumentNullException("payment");
			}

			// add the data which user shouldn't have access to
			payment.UpdatedBy = security.UserID;
			payment.UpdatedDate = DateTimeOffset.Now;

			using (var cmd = new SqlCommand())
			{
				payment.UpdateSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// The remove.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="payment">
		/// The payment.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Remove(SecurityClass security, BulkPaymentClass payment)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (payment == null)
			{
				throw new ArgumentNullException("payment");
			}

			using (var cmd = new SqlCommand())
			{
				payment.PurgeSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// The get by ID.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="guid">
		/// The GUID.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentClass"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// Retrieve bulk payment exception.
		/// </exception>
		public BulkPaymentClass GetByID(SecurityClass inSecurity, Guid guid)
		{
			// now get the wac using the IdentityGuid
			DataSet rs;

			using (var cmd = new SqlCommand())
			{
				BulkPaymentClass.SelectByID(cmd, guid);
				rs = this.consolidatedDa.GetDataSet(cmd, inSecurity);
			}

			DataTable rtable = rs.Tables[0];

			// check that we have results (that we should)
			if (0 == rtable.Rows.Count)
			{
				throw new Exception("No results");
			}

			var result = new BulkPaymentClass();
			result.Load(rtable.Rows[0]);

			return result;
		}
		#endregion // Database interaction wrappers

		#region Handle dependencies
		/// <summary>
		/// The insert.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="inObject">
		/// The in object.
		/// </param>
		/// <param name="preOperation">
		/// The pre operation.
		/// </param>
		void IDependency.Insert(SecurityClass security, BaseDataObject inObject, bool preOperation)
		{
			// will be done manually
		}

		/// <summary>
		/// The update.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="inObject">
		/// The in object.
		/// </param>
		void IDependency.Update(SecurityClass security, BaseDataObject inObject)
		{
			// will be done manually
		}

		/// <summary>
		/// The purge.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="inObject">
		/// The in object.
		/// </param>
		void IDependency.Purge(SecurityClass security, BaseDataObject inObject)
		{
			// will be done manually
		}
		#endregion // Handle dependencies

		#region Enumerators
		/// <summary>
		/// The enumerate.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentCollectionClass"/>.
		/// </returns>
		public BulkPaymentCollectionClass Enumerate(SecurityClass inSecurity)
		{
			using (var cmd = new SqlCommand())
			{
				BulkPaymentClass.EnumerateSQL(cmd);
				return this.EnumerateEx(inSecurity, cmd);
			}
		}

		/// <summary>
		/// The enumerate by filter.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="inFilter">
		/// The in filter.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentCollectionClass"/>.
		/// </returns>
		public BulkPaymentCollectionClass EnumerateByFilter(SecurityClass inSecurity, BulkPaymentFilter inFilter)
		{
			using (var cmd = new SqlCommand())
			{
				BulkPaymentClass.EnumerateSQLByFilter(cmd, inFilter);
				return this.EnumerateEx(inSecurity, cmd);
			}
		}

		/// <summary>
		/// The enumerate invoice mapping.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="inCollection">
		/// The in collection.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentCollectionClass"/>.
		/// </returns>
		public BulkPaymentCollectionClass EnumerateInvoiceMapping(SecurityClass inSecurity, BulkPaymentCollectionClass inCollection)
		{
			// just a helper class, the core of the code is in BulkPaymentClass EnumerateInvoiceMapping(BulkPaymentClass)
			var result = new BulkPaymentCollectionClass();

			// need to enumerate all the invoice mapping for the bulk payments in the collection
			foreach (BulkPaymentClass payment in inCollection)
			{
				result.Add(this.EnumerateInvoiceMappingByPayment(inSecurity, payment));
			}

			return result;
		}

		/// <summary>
		/// The enumerate invoice mapping by payment.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="inPayment">
		/// The in payment.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentClass"/>.
		/// </returns>
		public BulkPaymentClass EnumerateInvoiceMappingByPayment(SecurityClass inSecurity, BulkPaymentClass inPayment)
		{
			var invoiceMappings = new BulkPaymentInvoiceMappingsClass();

			BulkPaymentClass result = inPayment;
			result.Mapping = invoiceMappings.EnumerateByBulkPaymentID(inSecurity, inPayment.BulkPaymentID);

			return result;
		}

		/// <summary>
		/// The enumerate extra.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		/// <param name="cmd">
		/// The cmd.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentCollectionClass"/>.
		/// </returns>
		protected BulkPaymentCollectionClass EnumerateEx(SecurityClass inSecurity, SqlCommand cmd)
		{
			DataSet ds = this.consolidatedDa.GetDataSet(cmd, inSecurity);
			var collection = new BulkPaymentCollectionClass();

			// go through our results and add it to our collection
			DataTable dt = ds.Tables[0];
			foreach (DataRow row in dt.Rows)
			{
				var payment = new BulkPaymentClass();
				payment.Load(row);
				collection.Add(payment);
			}

			return collection;
		}
		#endregion
	}
}
