using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionErrorDO : DataObject
	{
		#region Private Attributes
		private string aliasName;
		private string inventoryDate;
		private int errorStatus;
		#endregion

		#region Constructors
		public TransactionErrorDO ( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This properties gets and sets the alias name.
		/// </summary>
		[DataMember]
		public string AliasName
		{
			get { return this.aliasName; }
			set { this.aliasName = value; }
		}
		/// <summary>
		/// This properties gets and sets the inventory date.
		/// </summary>
		[DataMember]
		public string InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
		}
		/// <summary>
		/// This properties gets and sets the error status.
		/// </summary>
		[DataMember]
		public int ErrorStatus
		{
			get { return this.errorStatus; }
			set { this.errorStatus = value; }
		}
		#endregion

		#region Override Methods
		override public string getSelectCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getUpdateCommand ( )
		{
			return null;
		}
		#endregion
	}
}
