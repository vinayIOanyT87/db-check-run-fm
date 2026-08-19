using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[XmlType("TransportLineItem")]
   [Serializable]
   [DataContract]
	public class TransportLineItemDO : DataObject
	{
		#region Constants
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion

		#region Private data members

		[DataMember]
		private string transportOrderNumber;
		[DataMember]
		private string locationName;
		[DataMember]
		private string address1;
		[DataMember]
		private string address2;
		[DataMember]
		private string city;
		[DataMember]
		private string state;
		[DataMember]
		private string zip;
		[DataMember]
		private string pocName;
		[DataMember]
		private string pocPhone;
		[DataMember]
		private long transVersion;
		[DataMember]
		private Guid transactionGuid;
		[DataMember]
		private string createdBy;
		[DataMember]
		private string updatedBy;
		[DataMember]
		private DateTimeOffset? createdDate;
		[DataMember]
		private DateTimeOffset? updatedDate;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Routing Line Item DO class.
		/// </summary>
		public TransportLineItemDO()
		{
			this.Init();
		}
		#endregion

		#region Properties

		[DataMember]
		[XmlIgnore]
		public Guid TransactionTransportLineItemGuid { get; set; }

		[DataMember]
		[XmlIgnore]
		public Guid ConjoinedTransactionTransportLineItemGuid { get; set; }

		[QueryWriterField("TransportOrderNumber", "tblTransactionTransportLineItems.TransportOrderNumber")]
		public string TransportOrderNumber
		{
			get { return this.transportOrderNumber; }
			set { this.transportOrderNumber = value; }
		}

		[QueryWriterField("LocationName", "tblTransactionTransportLineItems.LocationName")]
		public string LocationName
		{
			get { return this.locationName; }
			set { this.locationName = value; }
		}

		[QueryWriterField("Address1", "tblTransactionTransportLineItems.Address1")]
		public string Address1
		{
			get { return this.address1; }
			set { this.address1 = value; }
		}

		[QueryWriterField("Address2", "tblTransactionTransportLineItems.Address2")]
		public string Address2
		{
			get { return this.address2; }
			set { this.address2 = value; }
		}

		[QueryWriterField("City", "tblTransactionTransportLineItems.City")]
		public string City
		{
			get { return this.city; }
			set { this.city = value; }
		}

		[QueryWriterField("State", "tblTransactionTransportLineItems.State")]
		public string State
		{
			get { return this.state; }
			set { this.state = value; }
		}

		[QueryWriterField("Zip", "tblTransactionTransportLineItems.Zip")]
		public string Zip
		{
			get { return this.zip; }
			set { this.zip = value; }
		}

		[QueryWriterField("POCName", "tblTransactionTransportLineItems.POCName")]
		public string POCName
		{
			get { return this.pocName; }
			set { this.pocName = value; }
		}

		[QueryWriterField("POCPhone", "tblTransactionTransportLineItems.POCPhone")]
		public string POCPhone
		{
			get { return this.pocPhone; }
			set { this.pocPhone = value; }
		}

		public long TransVersion
		{
			get { return this.transVersion; }
			set { this.transVersion = value; }
		}

		[XmlIgnore]
		public Guid TransactionGuid
		{
			get { return this.transactionGuid; }
			set { this.transactionGuid = value; }
		}

		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		[XmlElement("CreatedDateString")]
		public string CreatedDateString
		{
			get
			{
				return this.createdDate == null ? string.Empty : ((DateTimeOffset)this.createdDate).ToString(TimeFormat);
			}

			set
			{
				this.createdDate = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[XmlIgnore]
		public DateTimeOffset? CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		[XmlElement("UpdatedDateString")]
		public string UpdatedDateString
		{
			get
			{
				return this.updatedDate == null ? string.Empty : ((DateTimeOffset)this.updatedDate).ToString(TimeFormat);
			}

			set
			{
				this.updatedDate = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[XmlIgnore]
		public DateTimeOffset? UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

		#endregion Properties

		#region Public methods
		/// <summary>
		/// This method loads the transport line item object with the contents
		/// from the database.
		/// </summary>
		/// <param name="row"></param>
		public void Load(DataRow row)
		{
			this.TransactionTransportLineItemGuid = DataObject.getValue<Guid>(row["TransactionTransportLineItemGuid"], Guid.Empty);
			this.TransactionGuid = DataObject.getValue<Guid>(row["TransactionGuid"], Guid.Empty);
			this.transportOrderNumber = DataObject.getValue<string>(row["TransportOrderNumber"], "");
			this.transVersion = DataObject.getValue<long>(row["TransVersion"], -1);

			this.locationName = DataObject.getValue<string>(row["LocationName"], "");
			this.address1 = DataObject.getValue<string>(row["Address1"], "");
			this.address2 = DataObject.getValue<string>(row["Address2"], "");
			this.city = DataObject.getValue<string>(row["City"], "");
			this.state = DataObject.getValue<string>(row["State"], "");
			this.zip = DataObject.getValue<string>(row["Zip"], "");
			this.pocName = DataObject.getValue<string>(row["POCName"], "");
			this.pocPhone = DataObject.getValue<string>(row["POCPhone"], "");
			this.createdBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
			this.updatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
			this.createdDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this.updatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this.createdDate.Value);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.TransactionTransportLineItemGuid = Guid.Empty;
			this.transportOrderNumber = "";
			this.locationName = "";
			this.address1 = "";
			this.address2 = "";
			this.city = "";
			this.state = "";
			this.zip = "";
			this.pocName = "";
			this.pocPhone = "";
			this.transVersion = -1;
			this.transactionGuid = Guid.Empty;
			this.createdBy = "";
			this.updatedBy = "";
			this.createdDate = null;
			this.updatedDate = null;
		}
		#endregion

		#region Overrides
		public override string getSelectCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getDeleteCommand()
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
