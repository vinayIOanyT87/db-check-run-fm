using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class ListViewSR : AccountingServiceRequest
	{
		#region Attributes
		[DataMember] private LISTVIEW_TYPE _Type;
		[DataMember] private Guid _TypeGuid;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the list view service request class.
		/// </summary>
		public ListViewSR()
		{
			ListViewGuid = Guid.Empty;
		}
		#endregion

		#region Properties

		public LISTVIEW_TYPE Type
		{
			get { return _Type; }
			set { _Type = value; }
		}

		public Guid TypeGuid
		{
			get { return _TypeGuid; }
			set { _TypeGuid = value; }
		}

		[DataMember]
		public Guid ListViewGuid
		{
			get;
			set;
		}
		#endregion
	}
}
