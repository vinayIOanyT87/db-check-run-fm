using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   public class AdjustmentOwnerRecord
	{
		#region Private Attributes
		[DataMember]
		private string ownerName;
		[DataMember]
		private double grossValue;
		[DataMember]
		private double netValue;
		[DataMember]
		private double massValue;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the adjustment owner record object.
		/// </summary>
		public AdjustmentOwnerRecord ( )
		{
			this.Init ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property gets and sets the owner name attribute.
		/// </summary>
		public string OwnerName
		{
			get { return this.ownerName; }
			set { this.ownerName = value; }
		}

		/// <summary>
		/// This property gets and sets the gross value attribute.
		/// </summary>
		public double GrossValue
		{
			get { return this.grossValue; }
			set { this.grossValue = value; }
		}

		/// <summary>
		/// This property gets and sets the net value attribute.
		/// </summary>
		public double NetValue
		{
			get { return this.netValue; }
			set { this.netValue = value; }
		}

		/// <summary>
		/// This property gets and sets the mass value attribute.
		/// </summary>
		public double MassValue
		{
			get { return this.massValue; }
			set { this.massValue = value; }
		}
		#endregion

		#region private methods
		/// <summary>
		/// This method initializes the adjustment owner record object to its
		/// initial starting state.
		/// </summary>
		private void Init ( )
		{
			this.ownerName = "";
			this.grossValue = 0.0;
			this.netValue = 0.0;
			this.massValue = 0.0;
		}
		#endregion
	}
}
