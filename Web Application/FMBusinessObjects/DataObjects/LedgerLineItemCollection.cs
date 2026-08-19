using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [CollectionDataContract]
	[KnownType ( typeof ( LedgerLineItemDO ) )]
	public class LedgerLineItemCollection : BaseCollections
	{
		#region Attributes
		[DataMember] private double grossAvgUnitPrice;
		[DataMember] private double netAvgUnitPrice;
		[DataMember] private double massAvgUnitPrice;
		#endregion

		#region Constructors
		public LedgerLineItemCollection ( )
		{
		}
		#endregion

		#region Public Properties
		public double GrossAverageUnitPrice
		{
			get { return this.grossAvgUnitPrice; }
			set { this.grossAvgUnitPrice = value; }
		}

		public double NetAverageUnitPrice
		{
			get { return this.netAvgUnitPrice; }
			set { this.netAvgUnitPrice = value; }
		}

		public double MassAverageUnitPrice
		{
			get { return this.massAvgUnitPrice; }
			set { this.massAvgUnitPrice = value; }
		}
		#endregion
	}
}
