using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionListDO : DataObject
	{
		#region Attributes
		private DataSet transactionDataSet;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction list data object class.
		/// </summary>
		public TransactionListDO ( )
		{
			init ( );
		}

		/// <summary>
		/// This constructor initializes the transaction list data object based
		/// on a data set.
		/// </summary>
		/// <param name="dataSet"></param>
		public TransactionListDO ( DataSet dataSet )
		{
			init ( );
			load ( dataSet );
		}
		#endregion

		#region Properties
		[DataMember]
		public DataSet TransactionDataSet
		{
			get { return this.transactionDataSet; }
			set { this.transactionDataSet = value; }
		}
		#endregion

		#region Public Methods
		private void init ( )
		{
		}


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
			return null;
		}


		#endregion
	}
}
