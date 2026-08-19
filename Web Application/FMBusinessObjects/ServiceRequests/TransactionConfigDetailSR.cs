using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class TransactionConfigDetailSR : AccountingServiceRequest
	{
		#region Public enumeration
		public enum TransactionConfigDetailTab { STANDARD, CUSTOM, PRODUCT };
		public enum TransactionCongigDetailButton
		{
			RESET, DEFAULT, NEW_CUSTOM, NEW_VALUE, APPLY_VALUE, EDIT_VALUE, DELETE_VALUE,
			CUSTOM_SELECTED, OK
		};
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the transaction configuration detail 
		/// service request class.
		/// </summary>
		public TransactionConfigDetailSR ( )
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public TransactionCongigDetailButton ActiveButton
		{
			get;
			set;
		}

		[DataMember]
		public TransactionConfigDetailTab Tab
		{
			get;
			set;
		}

		[DataMember]
		public string AliasName
		{
			get;
			set;
		}

		[DataMember]
		public string TransactionType
		{
			get;
			set;
		}

		[DataMember]
		public string TransactionDescription
		{
			get;
			set;
		}

		[DataMember]
		public bool UseTransactionControlNumber
		{
			get;
			set;
		}

		[DataMember]
		public DataObject DataObject
		{
			get;
			set;
		}
		#endregion
	}
}
