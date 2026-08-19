using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class GetTransactionTypeDO : DataObject
	{
		#region Private data members
		[DataMember]
		private TransactionTypes transType;
		[DataMember]
		private string documentNumber = "";
		[DataMember]
		private string transID = "";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Get Transaction Type data object class.
		/// </summary>
		public GetTransactionTypeDO ( )
		{
		}
		#endregion

		#region Public override methods
		public override string getDeleteCommand ( )
		{
			return null;
		}


		public override string getInsertCommand ( )
		{
			return null;
		}


		public override string getSelectCommand ( )
		{
			return null;
		}


		public override string getUpdateCommand ( )
		{
			return null;
		}
		#endregion

		#region Properties

		public TransactionTypes TransType
		{
			get { return this.transType; }
			set { this.transType = value; }
		}

		public string DocumentNumber
		{
			get { return this.documentNumber; }
			set { this.documentNumber = value; }
		}

		public string TransID
		{
			get { return this.transID; }
			set { this.transID = value; }
		}
		#endregion
	}
}
