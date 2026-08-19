using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class FuelOrderAssociatedTxListDO : DataObject
	{
		#region Private data members
		private BaseCollections fuelOrderAssociatedTx;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the fuel order associated transaction 
		/// list data object class.
		/// </summary>
		public FuelOrderAssociatedTxListDO ( )
		{
			this.fuelOrderAssociatedTx = new BaseCollections ( );
		}
		#endregion

		#region Properties
		[DataMember]
		public BaseCollections FuelOrderAssociatedTx
		{
			get { return this.fuelOrderAssociatedTx; }
			private set { this.fuelOrderAssociatedTx = value; }
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
