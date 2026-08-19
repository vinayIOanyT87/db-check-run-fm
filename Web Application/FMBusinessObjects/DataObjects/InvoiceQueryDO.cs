using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[System.Serializable]
	[XmlRoot ( "Transaction" )]
	[XmlType ( "Transaction" )]
	[DataContract]
	public class InvoiceQueryDO : DataObject
	{
		#region Attributes
		[DataMember]
		protected InvoiceQueryClass m_query = new InvoiceQueryClass ( );
		#endregion // Attributes

		#region Constructors
		public InvoiceQueryDO ( )
		{
			this.InvoiceQueryGuid = Guid.Empty;
			this.Description = " ";
		}
		#endregion // Constructors

		#region Properties

		public Guid InvoiceQueryGuid
		{
			get { return m_query.IdentityGuid; }
			set { m_query.IdentityGuid = value; }
		}

		public string Description
		{
			get { return m_query.Description; }
			set { m_query.Description = value; }
		}

		public string CreatedBy
		{
			get { return m_query.CreatedBy; }
			set { m_query.CreatedBy = value; }
		}

		public DateTimeOffset CreatedDate
		{
			get { return m_query.CreatedDate; }
			set { m_query.CreatedDate = value; }
		}

		public string UpdatedBy
		{
			get { return m_query.UpdatedBy; }
			set { m_query.UpdatedBy = value; }
		}

		public DateTimeOffset UpdatedDate
		{
			get { return m_query.UpdatedDate; }
			set { m_query.UpdatedDate = value; }
		}
		#endregion // Properties

		#region Overrides
		public override string getInsertCommand ( )
		{
			return null;
		}

		public override string getDeleteCommand ( )
		{
			return null;
		}

		public override string getSelectCommand ( )
		{
			return null;
		}

		public override string getUpdateCommand ( )
		{
			return null;
		}
		#endregion // Overrides
	}
}
