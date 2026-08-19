using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	[KnownType ( typeof ( LedgerLineItemCollection ) )]
	public class LedgerDO : DataObject
	{
		#region Attributes
		[DataMember] private LedgerLineItemCollection lineItemsList;
		private const int EMPTY_STRING = 0;
		#endregion

		#region Contructor
		public LedgerDO ( )
		{
			this.init ( );
		}

		/// <summary>
		/// This is the deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		//public LedgerDO ( System.Runtime.Serialization.SerializationInfo info,
		//   System.Runtime.Serialization.StreamingContext context )
		//{
		//}

		public LedgerDO ( System.Data.DataSet dataSet )
		{
			init ( );
			base.load ( dataSet );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This properties gets and sets the collection of ledger line item
		/// data objects.  The data is used to populate the ledger page.
		/// </summary>
		public LedgerLineItemCollection LedgerLineItems
		{
			get { return this.lineItemsList; }
			set { this.lineItemsList = value; }
		}
		#endregion

		#region Override Public Methods
		override public string getUpdateCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getSelectCommand ( )
		{
			int deleted = 0;
			return "SELECT * FROM tblTransactions WHERE DeleteFlag = " + deleted;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This methods initializes the Ledger DO object.
		/// </summary>
		private void init ( )
		{
			this.lineItemsList = new LedgerLineItemCollection ( );
		}
		#endregion
	}
}
