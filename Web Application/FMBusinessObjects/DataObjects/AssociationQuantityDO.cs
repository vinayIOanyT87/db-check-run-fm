using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class AssociationQuantityDO : CustomResultDO
	{
		#region Properties
		[DataMember]
		public double TotalQuantity
		{
			get;
			set;
		}

		[DataMember]
		public double ProductPrice
		{
			get;
			set;
		}

		[DataMember]
		public double Excise
		{
			get;
			set;
		}

		[DataMember]
		public double GST
		{
			set;
			get;
		}

		[DataMember]
		public double MarkUp
		{
			get;
			set;
		}
		#endregion

		#region Construction
		/// <summary>
		/// This is the default constructor for the Association Quantity data object class.
		/// </summary>
		public AssociationQuantityDO ( )
			: base ( )
		{
			this.TotalQuantity	= 0.0;
			this.ProductPrice	= 0.0;
			this.Excise			= 0.0;
			this.GST			= 0.0;
			this.MarkUp			= 0.0;
		}
		#endregion
	}
}
