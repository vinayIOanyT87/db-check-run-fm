using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class SaveInvoiceQuerySR : AccountingServiceRequest
	{
		#region Protected data members
		[DataMember]
		protected List<InvoiceQueryDO> m_queryList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the save invoice query service
		/// request class.
		/// </summary>
		/// <param name="a_security"></param>
		public SaveInvoiceQuerySR(SecurityClass a_security)
			: base()
		{
			this.InvoiceQueries = new List<InvoiceQueryDO>();
			base.Security = a_security;
		}
		#endregion

		#region Properties

		public List<InvoiceQueryDO> InvoiceQueries
		{
			get { return m_queryList; }
			set { m_queryList = value; }
		}
		#endregion // Properties
	}
}
