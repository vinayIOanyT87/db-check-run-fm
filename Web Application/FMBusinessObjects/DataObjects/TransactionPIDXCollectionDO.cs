using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionPIDXCollectionDO : DataObject
	{
		#region Private data members
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction PIDX data object collection.
		/// </summary>
		public TransactionPIDXCollectionDO()
		{
			this.TransactionPIDXDOList = new ArrayList();
		}
		#endregion

		#region Properties
		[DataMember]
		public ArrayList TransactionPIDXDOList { get; private set; }
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will add a transaction PIDX data object to the collection.
		/// </summary>
		/// <param name="transPidxDO"></param>
		public void Add(TransactionPIDXDO transPidxDO)
		{
			this.TransactionPIDXDOList.Add(transPidxDO);
		}

		/// <summary>
		/// This method will clear the transaction PIDX data object collection.
		/// </summary>
		public void Clear()
		{
			this.TransactionPIDXDOList.Clear();
		}
		#endregion

		#region Abstract implementation
		public override string getSelectCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion
	}
}
