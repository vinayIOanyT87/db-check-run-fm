using System;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;

using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;

namespace FMBusinessServices.ServiceClasses
{
	public class AssociatedPaymentsClass : IAssociatedPayments
	{
		#region Private data members
		private const string ErrMsg001 = "Security argument null";

		private readonly ConsolidatedDAClass consolidatedDA;
		private enum SecurityType { Modify, View, Either };
		private AssociatedPaymentCollectionClass unAssociatedPaymentCollection;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the FESS Associated Payments Class.
		/// </summary>
		public AssociatedPaymentsClass ( )
		{
			this.consolidatedDA = new ConsolidatedDAClass ( );
			this.Reset ( );
		}
		#endregion

		#region Public methods

		/// <summary>
		/// This method will return a DataSet of all the associated and unassociated payments. If there
		/// is not data, then the DataView will be null.
		/// </summary>
		/// <returns></returns>
		/// <param name="security"></param>
		/// <param name="findStr"></param>
		/// <param name="dataSet"></param>
		public void GetPaymentListByFindString ( SecurityClass security, string findStr, DataSet dataSet )
		{
			if ( string.IsNullOrEmpty(findStr) )
			{
				findStr = null;
			}

			this.RetrievePaymentList ( security, findStr, dataSet );
		}

		/// <summary>
		/// This method will return a DataSet of all the associated and unassociated payments. If there
		/// is not data, then the DataView will be null.
		/// </summary>
		/// <returns></returns>
		/// <param name="security"></param>
		/// <param name="dataSet"></param>
		public void GetPaymentList ( SecurityClass security, DataSet dataSet )
		{
			this.RetrievePaymentList ( security, null, dataSet );
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Reset ( )
		{
			this.unAssociatedPaymentCollection = new AssociatedPaymentCollectionClass ( );
		}

		/// <summary>
		/// This method retrieve all the Associated and Unassociated Payments and load
		/// up two collections.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="findStr"></param>
		private void RetreivePayments ( SecurityClass security, string findStr )
		{
			this.CheckSecurity ( security, SecurityType.Either );

			var associatedPayment = new AssociatedPaymentClass ( );

			using (var sqlCommand = new SqlCommand())
			{
				associatedPayment.SelectUnassociatedSQL( sqlCommand, findStr );
				DataSet dataSetUnassociated = this.consolidatedDA.GetDataSet( sqlCommand, security );

				// Load all the unassociated payments
				if (dataSetUnassociated != null)
				{
					DataTable table = dataSetUnassociated.Tables[0];

					if (table.Rows.Count > 0)
					{
						foreach (DataRow row in table.Rows)
						{
							associatedPayment = new AssociatedPaymentClass();
							associatedPayment.Load( row );
							this.unAssociatedPaymentCollection.Add( associatedPayment );
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will return a data set of the payments that can be associated
		/// to a rebate number.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="findStr"></param>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		private void RetrievePaymentList ( SecurityClass security, string findStr, DataSet dataSet )
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException( "dataSet" );
			}

			DataTable table = null;

			this.RetreivePayments ( security, findStr );

			if (this.unAssociatedPaymentCollection.Count > 0)
			{
				try
				{
					table = new DataTable( "Payments" );

					table.Columns.Add( "Select", Type.GetType( "System.Boolean" ) );
					table.Columns.Add( "TransID", Type.GetType( "System.String" ) );
					table.Columns.Add( "LineItemGuid", Type.GetType( "System.Guid" ) );
					table.Columns.Add( "OrderNumber", Type.GetType( "System.String" ) );
					table.Columns.Add( "AccountNumber", Type.GetType( "System.String" ) );
					table.Columns.Add( "InvoiceNumber", Type.GetType( "System.String" ) );
					table.Columns.Add( "Supplier", Type.GetType( "System.String" ) );
					table.Columns.Add( "FuelType", Type.GetType( "System.String" ) );

					dataSet.Tables.Add( table );

					foreach (AssociatedPaymentClass associatedPayment in this.unAssociatedPaymentCollection)
					{
						DataRow row = table.NewRow();
						row["Select"] = associatedPayment.SelectedFlag;
						row["TransID"] = associatedPayment.TransID;
						row["LineItemGuid"] = associatedPayment.TransactionLineItemGuid;
						row["OrderNumber"] = associatedPayment.OrderNumber;
						row["AccountNumber"] = associatedPayment.AccountNumber;
						row["InvoiceNumber"] = associatedPayment.InvoiceNumber;
						row["Supplier"] = associatedPayment.SupplierID;
						row["FuelType"] = associatedPayment.ProductID;

						table.Rows.Add( row );
					}
				}
				catch (Exception)
				{
					if (table != null)
					{
						table.Dispose();
					}

					throw;
				}
			}
		}

		/// <summary>
		/// This method will check to see if the user has the modify
		/// Financial data right.
		/// </summary>
		/// <param name="security"></param>
		/// <param name="securityType"></param>
		private void CheckSecurity ( SecurityClass security, SecurityType securityType )
		{
			if (security == null)
			{
				throw new ArgumentNullException ( ErrMsg001 );
			}

			switch (securityType)
			{
				case SecurityType.Modify:
					{
						if (security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA ) == false)
						{
							throw new FMInsufficientRightsException();
						}
						break;
					}
				case SecurityType.View:
					{
						if (security.HasRight ( RIGHT.VIEW_FINANCIAL_DATA ) == false)
						{
							throw new FMInsufficientRightsException();
						}
						break;
					}
				case SecurityType.Either:
					{
						if (( security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA ) == false )
						   && ( security.HasRight ( RIGHT.VIEW_FINANCIAL_DATA ) == false ))
						{
							throw new FMInsufficientRightsException();
						}
						break;
					}
				default:
				{
					throw new FMInsufficientRightsException();
				}
			}

			if (security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA ) == false)
			{
				throw new FMInsufficientRightsException(); 
			}
		}
		#endregion
	}
}