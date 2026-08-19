using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class EquipmentListSR : AccountingServiceRequest
	{
		#region Public enumeration
		public enum EquipmentListButtons { CLOSE, MODIFY, DELETE, NEXT_PAGE };
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the equipment list service request class.
		/// </summary>
		public EquipmentListSR ( )
		{
		}
		#endregion

		#region Properties

/* A. Hush 2/6/2012 These properties are never referenced, so I can't tell if they are
 * being used properly per the new DB schema

		[DataMember]
		public long TransactionId
		{
			get;
			set;
		}

		[DataMember]
		public long EquipmentId
		{
			get;
			set;
		}
 */

		[DataMember]
		public EquipmentListButtons ButtonSelected
		{
			get;
			set;
		}

		[DataMember]
		public string Tolerance
		{
			get;
			set;
		}
		#endregion
	}
}
