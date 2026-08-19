using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   public class AliasAssignmentDO
	{
		#region Attributes
		[DataMember]
		protected string assignedSite;
		[DataMember]
		protected Guid transactionAliasGuid;
		[DataMember]
		protected string aliasName;
		[DataMember]
		protected string aliasCustomName;
		#endregion Attributes

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Alias Assignment data object.
		/// </summary>
		public AliasAssignmentDO ( )
		{

		}
		#endregion

		#region Properties

		public string AssignedSite
		{
			get { return this.assignedSite; }
			set { this.assignedSite = value; }
		}

		public Guid TransactionAliasGuid
		{
			get { return this.transactionAliasGuid; }
			set { this.transactionAliasGuid = value; }
		}

		public string AliasName
		{
			get { return this.aliasName; }
			set { this.aliasName = value; }
		}

		public string AliasCustomName
		{
			get { return this.aliasCustomName; }
			set { this.aliasCustomName = value; }
		}
		#endregion Properties
	}
}
