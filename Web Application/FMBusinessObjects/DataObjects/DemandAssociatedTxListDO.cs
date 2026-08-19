using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class DemandAssociatedTxListDO : DataObject
	{
		#region Private data members
		[DataMember]
		private BaseCollections demandAssociatedTrans;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor to the Demand Associated Transaction List data object.
		/// </summary>
		public DemandAssociatedTxListDO ( )
		{
			this.demandAssociatedTrans = new BaseCollections ( );
		}
		#endregion

		#region Properties
		public BaseCollections DemandAssociatedTrans
		{
			get { return this.demandAssociatedTrans; }
			private set { this.demandAssociatedTrans = value; }
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
	}
}
