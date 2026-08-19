using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class DrawdownDO : DataObject
	{
		#region Private data members
		[DataMember]
		private bool noSupplyOrderInHierarchy = false;
		[DataMember]
		private bool noSupplyOrderChildren = false;
		[DataMember]
		private bool quantityToleranceExceeded = false;
		[DataMember]
		private bool quantityLimitExceeded = false;
		[DataMember]
		private bool valueToleranceExceeded = false;
		[DataMember]
		private bool valueLimitExceeded = false;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Drawdown data object class.
		/// </summary>
		public DrawdownDO()
		{
		}
		#endregion

		#region Properties
		public bool NoSupplyOrderInHierarchy
		{
			get { return this.noSupplyOrderInHierarchy; }
			set { this.noSupplyOrderInHierarchy = value; }
		}

		public bool NoSupplyOrderChildren
		{
			get { return this.noSupplyOrderChildren; }
			set { this.noSupplyOrderChildren = value; }
		}

		public bool QuantityToleranceExceeded
		{
			get { return this.quantityToleranceExceeded; }
			set { this.quantityToleranceExceeded = value; }
		}

		public bool QuantityLimitExceeded
		{
			get { return this.quantityLimitExceeded; }
			set { this.quantityLimitExceeded = value; }
		}

		public bool ValueToleranceExceeded
		{
			get { return this.valueToleranceExceeded; }
			set { this.valueToleranceExceeded = value; }
		}

		public bool ValueLimitExceeded
		{
			get { return this.valueLimitExceeded; }
			set { this.valueLimitExceeded = value; }
		}
		#endregion

		#region Public override methods
		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getSelectCommand()
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
