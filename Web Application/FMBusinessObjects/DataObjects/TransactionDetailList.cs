using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Specialized;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionDetailList
	{
		#region Public data members
		public const string TransactionDetailListKey = "TransactionDetailList";
		#endregion

		#region Private data members
		[DataMember]
		private StringCollection transactionIDList;
		[DataMember]
		private int currentIndex;
        [DataMember]
        private string returnURL;
        [DataMember]
        private Guid? selectedTransactionGuid;
	    [DataMember]
	    private string selectedTransactionAliasID;
        [DataMember]
		private TransactionDO newTransaction;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transaction Detail List class.
		/// </summary>
		public TransactionDetailList ( )
		{
			this.TransactionIDList = new StringCollection ( );
		}
		#endregion

		#region Properties
		
		public StringCollection TransactionIDList
		{
			get { return this.transactionIDList; }
			set { this.transactionIDList = value; }
		}

		public int CurrentIndex
		{
			get { return this.currentIndex; }
			set { this.currentIndex = value; }
		}

        public string ReturnURL
        {
            get { return this.returnURL; }
            set { this.returnURL = value; }
        }
        public Guid? SelectedTransactionGuid
        {
            get { return this.selectedTransactionGuid; }
            set { this.selectedTransactionGuid = value; }
        }

	    public string SelectedTransactionAliasID
	    {
	        get { return this.selectedTransactionAliasID; }
	        set { this.selectedTransactionAliasID = value; }
	    }

        public TransactionDO NewTransaction
		{
			get { return this.newTransaction; }
			set { this.newTransaction = value; }
		}
		#endregion
	}
}
