using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   [KnownType(typeof(AssociatedTxDO))]
	public class AssociatedTxListDO : DataObject
	{
		#region Private data member
		[DataMember]
		private DataSet associatedTransactions;
		[DataMember]
		private DataSet availableTransactions;
		[DataMember]
		private BaseCollections associatedTransIDs = new BaseCollections ( );
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Associated Tx List Data object class.
		/// </summary>
		public AssociatedTxListDO ( )
		{
			this.associatedTransactions = new DataSet ( );
			this.availableTransactions  = new DataSet ( );
		}
		#endregion

		#region Properties

		public DataSet AssociatedTransactions
		{
			get { return this.associatedTransactions; }
			set { this.associatedTransactions = value; }
		}

		public DataSet AvailableTransactions
		{
			get { return this.availableTransactions; }
			set { this.availableTransactions = value; }
		}

		public BaseCollections AssociatedTransIDs
		{
			get { return this.associatedTransIDs; }
			private set { this.associatedTransIDs = value; }
		}
		#endregion

		#region Override public methods
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
	}
}
