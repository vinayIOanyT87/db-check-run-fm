using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	public class TransactionTypesDO : DataObject
	{
		#region Attributes
		private long transTypeId;
		private string site;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the transaction types data object class.
		/// </summary>
		public TransactionTypesDO ( )
		{
			this.init ( );
		}
		#endregion

		#region Properties
		[DataMember]
		public long TransactionTypeID
		{
			get { return this.transTypeId; }
			set { this.transTypeId = value; }
		}

		[DataMember]
		public string Site
		{
			get { return this.site; }
			set { this.site = value; }
		}
		#endregion

		#region Methods
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
			return null;
		}

		private void init ( )
		{
		}
		#endregion
	}
}
