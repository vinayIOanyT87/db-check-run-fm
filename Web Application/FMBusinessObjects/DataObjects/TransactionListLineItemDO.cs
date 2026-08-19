using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionListLineItemDO : DataObject
	{
		#region Attributes
		[DataMember] private ArrayList quantityDOList = null;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the transaction list line item
		/// data object class.
		/// </summary>
		public TransactionListLineItemDO ( )
		{
		}
		#endregion

		#region Methods
		public void AddLineItemColumn ( QuantityDO quantityDO )
		{
			this.quantityDOList.Add ( quantityDO );
		}

		public void RemoveLineItemColumnAt ( int index )
		{
			this.quantityDOList.RemoveAt ( index );
		}

		public QuantityDO getLineItemColumn ( int index )
		{
			return (QuantityDO) this.quantityDOList[index];
		}

		override public string getSelectCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}
		override public string getUpdateCommand ( )
		{
			return null;
		}
		override public string getDeleteCommand ( )
		{
			return null; ;
		}
		#endregion
	}
}
