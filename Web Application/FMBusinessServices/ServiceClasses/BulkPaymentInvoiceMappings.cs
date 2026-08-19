// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkPaymentInvoiceMappings.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the BulkPaymentInvoiceMappingsClass type.
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
	/// The bulk payment invoice mappings class.
	/// </summary>
	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class BulkPaymentInvoiceMappingsClass : IDependency, IBulkPaymentInvoiceMappings
	{
		#region Protected data members
		/// <summary>
		/// The consolidated data layer.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDa;
		#endregion 

		#region Construction
		/// <summary>
		/// Initializes a new instance of the <see cref="BulkPaymentInvoiceMappingsClass"/> class.
		/// </summary>
		public BulkPaymentInvoiceMappingsClass()
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}
		#endregion 

		#region Database interaction wrappers
		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="invoiceMapping">
		/// The invoice mapping.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, BulkPaymentInvoiceMappingClass invoiceMapping)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (invoiceMapping == null)
			{
				throw new ArgumentNullException("invoiceMapping");
			}

			// add the data which user shouldn't have access to
			invoiceMapping.CreatedBy = security.UserID;
			invoiceMapping.CreatedDate = DateTimeOffset.Now;
			invoiceMapping.UpdatedBy = invoiceMapping.CreatedBy;
			invoiceMapping.UpdatedDate = invoiceMapping.CreatedDate;

			using (var cmd = new SqlCommand())
			{
				invoiceMapping.InsertSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// The update.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="invoiceMapping">
		/// The invoice mapping.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Update(SecurityClass security, BulkPaymentInvoiceMappingClass invoiceMapping)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (invoiceMapping == null)
			{
				throw new ArgumentNullException("invoiceMapping");
			}

			// check that the user has the rights to perform this action
			//    if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
			//    {
			//        throw new Exception("User does not have the right to modify payment data");
			//    }

			// add the data which user shouldn't have access to
			invoiceMapping.UpdatedBy = security.UserID;
			invoiceMapping.UpdatedDate = DateTimeOffset.Now;

			using (var cmd = new SqlCommand())
			{
				invoiceMapping.UpdateSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}

		/// <summary>
		/// The remove.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="invoiceMapping">
		/// The invoice mapping.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Null argument exception.
		/// </exception>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Remove(SecurityClass security, BulkPaymentInvoiceMappingClass invoiceMapping)
		{
			// Validate the security and price list (aka standing offer) objects
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (invoiceMapping == null)
			{
				throw new ArgumentNullException("invoiceMapping");
			}

			using (var cmd = new SqlCommand())
			{
				invoiceMapping.PurgeSQL(cmd);
				this.consolidatedDa.ExecuteQuery(security, cmd);
			}
		}
		#endregion

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
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentInvoiceMappingCollectionClass"/>.
		/// </returns>
		public BulkPaymentInvoiceMappingCollectionClass Enumerate(SecurityClass security)
		{
			BulkPaymentInvoiceMappingCollectionClass bulkPaymentInvoiceMappingCollection;

			using (var cmd = new SqlCommand())
			{
				BulkPaymentInvoiceMappingClass.EnumerateSQL(cmd);
				bulkPaymentInvoiceMappingCollection = this.EnumerateEx(security, cmd);
			}

			return bulkPaymentInvoiceMappingCollection;
		}

		/// <summary>
		/// The enumerate by invoice transaction ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="transId">
		/// The trans id.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentInvoiceMappingClass"/>.
		/// </returns>
		public BulkPaymentInvoiceMappingClass EnumerateByInvoiceTransID(SecurityClass security, string transId)
		{
			BulkPaymentInvoiceMappingClass result = null;

			using (var cmd = new SqlCommand())
			{
				BulkPaymentInvoiceMappingClass.SelectByInvoiceTransID(cmd, transId);
				BulkPaymentInvoiceMappingCollectionClass collection = this.EnumerateEx(security, cmd);

				if (collection.Count > 0)
				{
					result = collection.Item(0);
				}
			}

			return result;
		}

		/// <summary>
		/// The enumerate by bulk payment ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="guid">
		/// The GUID.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentInvoiceMappingCollectionClass"/>.
		/// </returns>
		public BulkPaymentInvoiceMappingCollectionClass EnumerateByBulkPaymentID(SecurityClass security, Guid guid)
		{
			DataSet rs;

			// now get the wac using the index
			using (var cmd = new SqlCommand())
			{
				BulkPaymentInvoiceMappingClass.SelectByID(cmd, guid);
				rs = this.consolidatedDa.GetDataSet(cmd, security);
			}

			DataTable rtable = rs.Tables[0];

			var result = new BulkPaymentInvoiceMappingCollectionClass();

			// check if we have results
			if (0 != rtable.Rows.Count)
			{
				foreach (DataRow row in rtable.Rows)
				{
					var invoice = new BulkPaymentInvoiceMappingClass();
					invoice.Load(row);
					result.Add(invoice);
				}
			}

			return result;
		}

		/// <summary>
		/// The enumerate extra.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <returns>
		/// The <see cref="BulkPaymentInvoiceMappingCollectionClass"/>.
		/// </returns>
		protected BulkPaymentInvoiceMappingCollectionClass EnumerateEx(SecurityClass security, SqlCommand cmd)
		{
			DataSet ds = this.consolidatedDa.GetDataSet(cmd, security);
			var collection = new BulkPaymentInvoiceMappingCollectionClass();

			// go through our results and add it to our collection
			DataTable dt = ds.Tables[0];
			foreach (DataRow row in dt.Rows)
			{
				var invoice = new BulkPaymentInvoiceMappingClass();
				invoice.Load(row);
				collection.Add(invoice);
			}

			return collection;
		}
		#endregion 
	}
}
