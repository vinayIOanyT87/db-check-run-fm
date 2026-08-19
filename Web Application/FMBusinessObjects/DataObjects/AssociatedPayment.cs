/// <summary>
///   File name:	AssociatedPayment.cs
///   Purpose:	   The purpose of the FESS Associated Payment data object is to contain a single row
///               of data from the Transaction Line Item table for a payment. It inherits from 
///               ConsolidatedDataObjects base class.
///            
///   Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				   2005.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				
///   Author(s):	Richard R. Panachida
///   Version:	7.5.0.19  Current version
///	
///   Modification History:
///   Date:			   By:						Reason:
///   ----------		--------------------	----------------------------------
///	yyyy/mm/dd		developers name      change reason
///	
/// </summary>
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    using FMCore;

    #region FESS Associated Payment Class
    [DataContract]
   [Serializable]
   public class AssociatedPaymentClass : BaseDataObject
	{
		#region Public data members
		public const string ENTITY_TYPE_ID = "FESS Associated Payment";
		#endregion

		#region Private Data Members
		[DataMember]
		private string transID;
		[DataMember]
		private Guid transactionLineItemGuid;
		[DataMember]
		private bool selectedFlag;
		[DataMember]
		private string orderNumber;
		[DataMember]
		private string accountNumber;
		[DataMember]
		private string invoiceNumber;
		[DataMember]
		private string supplierID;
		[DataMember]
		private string productID;

		private const string ERR_MSG_001 = "No data found";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the FESS Associated Payment class.
		/// </summary>
		public AssociatedPaymentClass ( )
		{
			Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the Order Number of type string.
		/// </summary>
		public string OrderNumber
		{
			get { return this.orderNumber; }
			set { this.orderNumber = value; }
		}

		/// <summary>
		/// This property sets and gets the Account Number of type string.
		/// </summary>
		public string AccountNumber
		{
			get { return this.accountNumber; }
			set { this.accountNumber = value; }
		}

		/// <summary>
		/// This property sets and gets the Invoice Number of type string.
		/// </summary>
		public string InvoiceNumber
		{
			get { return this.invoiceNumber; }
			set { this.invoiceNumber = value; }
		}

		/// <summary>
		/// This property sets and gets the Supplier ID of type string.
		/// </summary>
		public string SupplierID
		{
			get { return this.supplierID; }
			set { this.supplierID = value; }
		}

		/// <summary>
		/// This property sets and gets the Product ID of type string.
		/// </summary>
		public string ProductID
		{
			get { return this.productID; }
			set { this.productID = value; }
		}

		/// <summary>
		/// This property sets and gets the transaction ID of type string.
		/// </summary>
		public string TransID
		{
			get { return this.transID; }
			set { this.transID = value; }
		}

		/// <summary>
		/// This property sets and gets the transaction line item GUID
		/// </summary>
		public Guid TransactionLineItemGuid
		{
			get { return this.transactionLineItemGuid; }
			set { this.transactionLineItemGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the Selected Flag of type boolean.
		/// </summary>
		public bool SelectedFlag
		{
			get { return this.selectedFlag; }
			set { this.selectedFlag = value; }
		}

		#endregion

		#region Operators
		/// <summary>
		/// Equals
		/// </summary>
		public override bool Equals ( object equalsObject )
		{
			if (equalsObject != null && equalsObject is AssociatedPaymentClass)
			{
				AssociatedPaymentClass associatedPayment = equalsObject as AssociatedPaymentClass;

				return ( ( this.TransID != null )
						&& ( this.TransID == associatedPayment.TransID )
						&& ( this.TransactionLineItemGuid == associatedPayment.TransactionLineItemGuid ) );

			}

			return false;
		}

		public override int GetHashCode ( )
		{
			return this.ToString ( ).GetHashCode ( );
		}

		/// <summary>
		/// == operator
		/// </summary>
		public static bool operator == ( AssociatedPaymentClass associatedPayment1, AssociatedPaymentClass associatedPayment2 )
		{
			try
			{
				return associatedPayment1.Equals ( associatedPayment2 );
			}
			catch
			{
				return ( (object) associatedPayment2 ) == null;
			}

		}

		/// <summary>
		/// Handle the boolean operator.
		/// </summary>
		/// <param name="associatedPayment1"></param>
		/// <param name="associatedPayment2"></param>
		/// <returns></returns>
		public static bool operator != ( AssociatedPaymentClass associatedPayment1, AssociatedPaymentClass associatedPayment2 )
		{
			return !( associatedPayment1 == associatedPayment2 );
		}
		#endregion

		private void Initialize()
		{

			this.transID			= "";
			this.transactionLineItemGuid	= Guid.Empty;
			this.orderNumber		= "";
			this.accountNumber		= "";
			this.invoiceNumber		= "";
			this.supplierID			= "";
			this.productID			= "";
			this.selectedFlag		= false;
		}

		#region Public methods
		/// <summary>
		/// This method resets the object to its initial state.
		/// </summary>
		public override void Reset ( )
		{
			base.Reset ( );
			Initialize();
		}

		/// <summary>
		/// This method loads the object with the information from the 
		/// database.
		/// </summary>
		/// <param name="Set"></param>
		/// 
		public void Load ( DataRow row )
		{
			if (row == null)
			{
				throw new ArgumentNullException ( AssociatedPaymentClass.ERR_MSG_001 );
			}

			this.Reset ( );
			this.transactionLineItemGuid = DataObject.getValue<Guid>(row["TransactionLineItemGuid"], Guid.Empty);
			this.transID			= DataObject.getValue<string>(row["TransID"], "");
			this.orderNumber		= DataObject.getValue<string>(row["OrderNumber"], "");
			this.accountNumber	= DataObject.getValue<string>(row["AccountNumber"], "");
			this.invoiceNumber	= DataObject.getValue<string>(row["InvoiceNumber"], "");
			this.supplierID		= DataObject.getValue<string>(row["SupplierID"], "");
			this.productID			= DataObject.getValue<string>(row["Product"], "");
			this.selectedFlag		= DataObject.getValue<bool>(row["SelectedFlag"], false);
		}

		/// <summary>
		/// This method loads the object using a dataset.  It will only load one row.
		/// </summary>
		/// <param name="dataSet"></param>
		public void Load ( DataSet dataSet )
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException ( AssociatedPaymentClass.ERR_MSG_001 );
			}

			this.Reset ( );

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			this.Load ( table.Rows[0] );
		}

		/// <summary>
		/// This method will create a Select Command that retrieves all the unassociated (to rebates)
		/// invoice payment type transactions.
		/// </summary>
		/// <param name="sqlCommand"></param>
		public void SelectUnassociatedSQL ( SqlCommand sqlCommand )
		{
			sqlCommand.Parameters.Clear ( );
			int transTypeID = (int) TransactionTypes.T21_AccountPayableInvoice;

			string sql = "SELECT t.TransID, " +
						 "l.TransactionLineItemGuid, " +
						 "t.DocumentNumber AS OrderNumber, " +
						 "ul.UserData13 AS AccountNumber, " +
						 "l.InvoiceNumber, " +
						 "l.Product, " +
						 "0 AS SelectedFlag, " +
						 "t.SupplierID " +
						 "FROM tblTransactionLineItems l LEFT OUTER JOIN tblTransactions t ON l.TransactionGuid = t.TransactionGuid " +
						 "LEFT OUTER JOIN tblTransactionLineItemUserData ul ON l.TransactionLineItemGuid = ul.TransactionLineItemGuid " +
						 "WHERE t.LookupTransTypeIndex = @LookupTransTypeIndex" +
						 " AND (l.TransactionLineItemGuid NOT IN (SELECT TransactionLineItemGuid FROM tblRebateInvoiceMapping) " +
						 " AND t.TransID NOT IN (SELECT TransID FROM tblRebateInvoiceMapping))";

			sqlCommand.CommandText = sql;

			sqlCommand.Parameters.Add ( "@LookupTransTypeIndex", System.Data.SqlDbType.Int );
			sqlCommand.Parameters["@LookupTransTypeIndex"].Value = transTypeID;
		}

		/// <summary>
		/// This method will create a Select Command that retrieves all the unassociated (to rebates)
		/// invoice payment type transactions using a find filter.
		/// </summary>
		/// <param name="sqlCommand"></param>
		public void SelectUnassociatedSQL ( SqlCommand sqlCommand, string findStr )
		{
			// If the find string empty then get all.
			if (( findStr == null ) || ( findStr.Trim().Length <= 0 ))
			{
				this.SelectUnassociatedSQL ( sqlCommand );
			}
			else
			{
				findStr = findStr.Trim();

				if (findStr.Length > 50)
				{
					findStr = findStr.Substring ( 0, 50 );
				}

				findStr = "%" + FuelsManagerExtensions.EscapeLikeClauseCharacters(findStr) + "%";
				sqlCommand.Parameters.Clear ( );
				int transTypeID = (int) TransactionTypes.T21_AccountPayableInvoice;

				string sql = "SELECT t.TransID, " +
							 "l.TransactionLineItemGuid, " +
							 "t.DocumentNumber AS OrderNumber, " +
							 "ul.UserData13 AS AccountNumber, " +
							 "l.InvoiceNumber, " +
							 "l.Product, " +
							 "0 AS SelectedFlag, " +
							 "t.SupplierID " +
							 "FROM tblTransactionLineItems l LEFT OUTER JOIN tblTransactions t ON l.TransactionGuid = t.TransactionGuid " +
							 "LEFT OUTER JOIN tblTransactionLineItemUserData ul ON l.TransactionLineItemGuid = ul.TransactionLineItemGuid " +
							 "WHERE t.LookupTransTypeIndex = @LookupTransTypeIndex " +
							 " AND (UPPER(t.DocumentNumber) LIKE @FindStr " +
									"OR UPPER(ul.UserData13) LIKE @FindStr " +
									"OR UPPER(l.InvoiceNumber) LIKE @FindStr " +
									"OR UPPER(t.SupplierID) LIKE @FindStr " +
									"OR UPPER(l.Product) LIKE @FindStr " +
									")" +
							 " AND (l.TransactionLineItemGuid NOT IN (SELECT TransactionLineItemGuid FROM tblRebateInvoiceMapping) " +
							 " AND t.TransID NOT IN (SELECT TransID FROM tblRebateInvoiceMapping))";

				sqlCommand.CommandText = sql;

				sqlCommand.Parameters.Add ( "@LookupTransTypeIndex", System.Data.SqlDbType.Int );
				sqlCommand.Parameters.Add ( "@FindStr", System.Data.SqlDbType.NVarChar, 102 );

				sqlCommand.Parameters["@LookupTransTypeIndex"].Value = transTypeID;
				sqlCommand.Parameters["@FindStr"].Value = findStr;
			}
		}
		#endregion
	}
	#endregion

	#region Associated Payment Collection Class
   [Serializable]
   [CollectionDataContract]
	public class AssociatedPaymentCollectionClass : CollectionBase
	{
		#region Private data members
		private const string ERR_MSG_001 = "Invalid Index";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Associated Payment Collection Class.
		/// </summary>
		public AssociatedPaymentCollectionClass ( )
		{
		}
		#endregion

		/// <summary>
		/// This method adds a FESS Associated Payment object to the collection.
		/// </summary>
		/// <param name="fessInvoiceMapping"></param>
		public void Add ( AssociatedPaymentClass associatedPayment )
		{
			List.Add ( associatedPayment );
		}

		/// <summary>
		/// This method removes a FESS Associated Payment Object from the collection given
		/// an index.
		/// </summary>
		/// <param name="index"></param>
		public void Remove ( int index )
		{
			if (index > Count - 1 || index < 0)
			{
				throw ( new Exception ( AssociatedPaymentCollectionClass.ERR_MSG_001 ) );
			}
			else
			{
				List.RemoveAt ( index );
			}
		}

		/// <summary>
		/// This method removes a FESS Associated Payment Object from the collection given
		/// a invoice mapping object.
		/// </summary>
		/// <param name="associatedPayment"></param>
		public void Remove ( AssociatedPaymentClass associatedPayment )
		{
			int index = 0;

			foreach (AssociatedPaymentClass associatedPaymentItem in List)
			{
				if (( associatedPaymentItem.TransactionLineItemGuid == associatedPayment.TransactionLineItemGuid )
				   && ( associatedPaymentItem.TransID != null )
				   && ( associatedPaymentItem.TransID.Length > 0 )
				   && ( associatedPaymentItem.TransID == associatedPayment.TransID ))
				{
					List.RemoveAt ( index );
					return;
				}

				index++;
			}
		}

		/// <summary>
		/// This method will return a FESS Associated Payment Object for a given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public AssociatedPaymentClass Item ( int index )
		{
			return (AssociatedPaymentClass) List[index];
		}

		/// <summary>
		/// This method perform an insert into the collection for a given
		/// index and FESS Associated Payment object combination.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="associatedPayment"></param>
		public void Insert ( int index, AssociatedPaymentClass associatedPayment )
		{
			List.Insert ( index, associatedPayment );
		}
	}
	#endregion
}
