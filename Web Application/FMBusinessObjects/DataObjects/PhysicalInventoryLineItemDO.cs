using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class PhysicalInventoryLineItemDO
	{
		#region Attributes
		[DataMember]
		protected DateTime inventoryDate;
		[DataMember]
		protected double netQuantity;
		[DataMember]
		protected double grossQuantity;
		[DataMember]
		protected double massQuantity;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// This is the default constructor for the physical inventory line item
		/// data object class.
		/// </summary>
		public PhysicalInventoryLineItemDO()
		{
		}
		#endregion

		#region Properties
		public DateTime InventoryDate
		{
			get { return inventoryDate; }
			set { inventoryDate = value; }
		}

		public double NetQuantity
		{
			get { return netQuantity; }
			set { netQuantity = value; }
		}

		public double GrossQuantity
		{
			get { return grossQuantity; }
			set { grossQuantity = value; }
		}

		public double MassQuantity
		{
			get { return massQuantity; }
			set { massQuantity = value; }
		}
		#endregion Properties
	}
}
