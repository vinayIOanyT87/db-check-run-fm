using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class OrderAssociatedTxLineItemDO : OrderListLineItemDO
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the order associated
		/// transaction line item data object class.
		/// </summary>
		public OrderAssociatedTxLineItemDO ( )
		{
		}
		#endregion

		#region Override methods
		public override string getSelectCommand ( )
		{
			return null;
		}

		public override string getInsertCommand ( )
		{
			return null;
		}

		public override string getUpdateCommand ( )
		{
			return null;
		}

		public override string getDeleteCommand ( )
		{
			return null;
		}
		#endregion
	}
}
