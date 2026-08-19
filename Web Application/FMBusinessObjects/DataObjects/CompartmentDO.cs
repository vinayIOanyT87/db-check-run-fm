using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class CompartmentDO
	{
		#region Protected data members
		protected string location;
		protected double quantity;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Compartment Data Object class.
		/// </summary>
		public CompartmentDO ( )
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public string Location
		{
			get { return location; }
			set { location = value; }
		}

		[DataMember]
		public double Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}
		#endregion
	}
}
