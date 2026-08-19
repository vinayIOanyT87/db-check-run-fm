using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransmitTranListDO : DataObject
	{
		#region Attributes

		[DataMember]
		private string xsdpath;

		[DataMember]
		private DataSet dsHeaders;
		[DataMember]
		private DataSet dsLineItems;
		[DataMember]
		private DataSet dsNotes;
		[DataMember]
		private DataSet dsSignature;
		[DataMember]
		private DataSet dsSubLineItems;
		[DataMember]
		private DataSet dsUserData;
		[DataMember]
		private DataSet dsWeightReadings;
		[DataMember]
		private DataSet dsTransportLineItems;

		//Header Tags
		[DataMember]
		private string headerStartTag;
		[DataMember]
		private string headerEndTag;

		//UserData Tags
		[DataMember]
		private string userDataStartTag;
		[DataMember]
		private string userDataEndTag;

		//Line Items Tags
		[DataMember]
		private string lineItemStartTag;
		[DataMember]
		private string lineItemEndTag;

		//Sub Line Items Tags
		[DataMember]
		private string subLineItemStartTag;
		[DataMember]
		private string subLineItemEndTag;

		//Notes Tags
		[DataMember]
		private string notesStartTag;
		[DataMember]
		private string notesEndTag;

		//Weight Readings Tags
		[DataMember]
		private string weightReadingsStartTag;
		[DataMember]
		private string weightReadingsEndTag;

		//Transport Line Item Tags
		[DataMember]
		private string transportLineItemStartTag;
		[DataMember]
		private string transportLineItemEndTag;

		#endregion

		#region Constructor
		public TransmitTranListDO()
		{
			init();
		}
		#endregion

		#region Properties

		public string XSDPath
		{
			get { return xsdpath; }
			set { xsdpath = value.TrimEnd("\\".ToCharArray()); }
		}

		public DataSet Headers
		{
			get { return dsHeaders; }
			set { dsHeaders = value; }
		}

		public DataSet LineItems
		{
			get { return dsLineItems; }
			set { dsLineItems = value; }
		}

		public DataSet Notes
		{
			get { return dsNotes; }
			set { dsNotes = value; }
		}

		public DataSet Signature
		{
			get { return dsSignature; }
			set { dsSignature = value; }
		}

		public DataSet SubLineItems
		{
			get { return dsSubLineItems; }
			set { dsSubLineItems = value; }
		}

		public DataSet UserData
		{
			get { return dsUserData; }
			set { dsUserData = value; }
		}

		public DataSet WeightReadings
		{
			get { return dsWeightReadings; }
			set { dsWeightReadings = value; }
		}

		public DataSet TransportLineItems
		{
			get { return this.dsTransportLineItems; }
			set { this.dsTransportLineItems = value; }
		}

		public bool NoData
		{
			get
			{
				return (dsHeaders == null ||
						dsHeaders.Tables[0] == null ||
						dsHeaders.Tables[0].Rows == null ||
						dsHeaders.Tables[0].Rows.Count == 0);
			}
		}
		#endregion

		#region Public Methods
		public void init()
		{
			dsHeaders = null;
			dsLineItems = null;
			dsNotes = null;
			dsSubLineItems = null;
			dsUserData = null;
			dsWeightReadings = null;
			this.headerStartTag = "<HEADER_IDENTIFICATION_ELEMENT_CA1E8920-2A36-489C-A873-AD1468338210>";
			this.headerEndTag = "</HEADER_IDENTIFICATION_ELEMENT_CA1E8920-2A36-489C-A873-AD1468338210>";

			this.userDataStartTag = "<USERDATA_IDENTIFICATION_ELEMENT_ECE725E1-9805-4604-B392-D5EF807D20DE>";
			this.userDataEndTag = "</USERDATA_IDENTIFICATION_ELEMENT_ECE725E1-9805-4604-B392-D5EF807D20DE>";

			this.lineItemStartTag = "<LINEITEM_IDENTIFICATION_ELEMENT_17770547-0AF3-40c2-A698-8C06882668B4>";
			this.lineItemEndTag = "</LINEITEM_IDENTIFICATION_ELEMENT_17770547-0AF3-40c2-A698-8C06882668B4>";

			this.subLineItemStartTag = "<SUBLINEITEM_IDENTIFICATION_ELEMENT_6A11714A-D72F-45e1-B294-320A33DF25C2>";
			this.subLineItemEndTag = "</SUBLINEITEM_IDENTIFICATION_ELEMENT_6A11714A-D72F-45e1-B294-320A33DF25C2>";

			this.notesStartTag = "<NOTES_IDENTIFICATION_ELEMENT_1EA41534-1BBB-4ac7-820D-1F3FE7478C3F>";
			this.notesEndTag = "</NOTES_IDENTIFICATION_ELEMENT_1EA41534-1BBB-4ac7-820D-1F3FE7478C3F>";

			this.weightReadingsStartTag = "<WEIGHTREADINGS_IDENTIFICATION_ELEMENT_FAC6A5FD-6E8E-4667-83EE-A6F3E007B743>";
			this.weightReadingsEndTag = "</WEIGHTREADINGS_IDENTIFICATION_ELEMENT_FAC6A5FD-6E8E-4667-83EE-A6F3E007B743>";

			this.transportLineItemStartTag = "<TRANSPORTLINEITEM_IDENTIFICATION_ELEMENT_B04EB73C-568B-4468-91B1-6DD1B70CB4D9>";
			this.transportLineItemEndTag = "</TRANSPORTLINEITEM_IDENTIFICATION_ELEMENT_B04EB73C-568B-4468-91B1-6DD1B70CB4D9>";
		}
		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return null;
		}

		private string StripTimeZoneInfoFromDateTimes(string xml, DataSet ds)
		{
			if (ds == null ||
				ds.Tables == null ||
				ds.Tables[0] == null ||
				ds.Tables[0].Rows == null ||
				ds.Tables[0].Rows.Count == 0)
			{
				return "";
			}

			DataColumnCollection cols = ds.Tables[0].Columns;
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList list = doc.SelectNodes("//Table");
			foreach (XmlNode trans in list)
			{
				foreach (XmlNode field in trans.ChildNodes)
				{

					if (cols.Contains(field.Name) &&
						System.Type.GetTypeCode(cols[field.Name].DataType) == System.TypeCode.DateTime)
					{
						string val = field.InnerText;
						int len = val.LastIndexOf(".");
						val = val.Substring(0, len);
						field.InnerText = val;
					}
				}
			}

			return doc.OuterXml;
		}
		public string GetDataSetAsXML()
		{
			string results = "";

			//Append Header Data Set XML
			results += headerStartTag;
			if (dsHeaders != null)
			{
				results += StripTimeZoneInfoFromDateTimes(dsHeaders.GetXml(), dsHeaders);
			}
			results += headerEndTag;

			//Append UserData Data Set XML
			results += userDataStartTag;
			if (dsUserData != null)
			{
				results += StripTimeZoneInfoFromDateTimes(dsUserData.GetXml(), dsUserData);
			}
			results += userDataEndTag;

			//Append LineItem Data Set XML
			results += lineItemStartTag;
			if (dsLineItems != null)
			{
				results += StripTimeZoneInfoFromDateTimes(dsLineItems.GetXml(), dsLineItems);
			}
			results += lineItemEndTag;

			//Append SubLineItem Data Set XML
			results += subLineItemStartTag;
			if (dsSubLineItems != null)
			{
				results += StripTimeZoneInfoFromDateTimes(dsSubLineItems.GetXml(), dsSubLineItems);
			}
			results += subLineItemEndTag;

			//Append Notes Data Set XML
			results += notesStartTag;
			if (dsNotes != null)
			{
				results += StripTimeZoneInfoFromDateTimes(dsNotes.GetXml(), dsNotes);
			}
			results += notesEndTag;

			//Append Notes Data Set XML
			results += weightReadingsStartTag;
			if (dsWeightReadings != null)
			{
				results += StripTimeZoneInfoFromDateTimes(dsWeightReadings.GetXml(), dsWeightReadings);
			}
			results += weightReadingsEndTag;

			//Append TransportLineItem Data Set XML
			results += this.transportLineItemStartTag;
			if (this.dsTransportLineItems != null)
			{
				results += this.StripTimeZoneInfoFromDateTimes(this.dsTransportLineItems.GetXml(), this.dsTransportLineItems);
			}
			results += this.transportLineItemEndTag;

			return results;
		}

		public void LoadDataSetsFromXML(string xml)
		{
			string datasetxml = "";
			int startindex = 0;
			int endindex = 0;
			//Header XML Data
			startindex = xml.IndexOf(headerStartTag) + headerStartTag.Length;
			endindex = xml.IndexOf(headerEndTag, startindex);
			datasetxml = xml.Substring(startindex, endindex - startindex).Trim();
			if (dsHeaders == null)
				dsHeaders = new DataSet();
			dsHeaders.Clear();
			dsHeaders.ReadXmlSchema(xsdpath + "\\tblTransactions.xsd");
			LoadDataSet(datasetxml, dsHeaders);


			//UserData XML 
			startindex = xml.IndexOf(userDataStartTag) + userDataStartTag.Length;
			endindex = xml.IndexOf(userDataEndTag, startindex);
			datasetxml = xml.Substring(startindex, endindex - startindex).Trim();
			if (dsUserData == null)
				dsUserData = new DataSet();
			dsUserData.Clear();
			dsUserData.ReadXmlSchema(xsdpath + "\\tblTransactionUserData.xsd");
			LoadDataSet(datasetxml, dsUserData);


			//Line Items XML 
			startindex = xml.IndexOf(lineItemStartTag) + lineItemStartTag.Length;
			endindex = xml.IndexOf(lineItemEndTag, startindex);
			datasetxml = xml.Substring(startindex, endindex - startindex).Trim();
			if (dsLineItems == null)
				dsLineItems = new DataSet();
			dsLineItems.Clear();
			dsLineItems.ReadXmlSchema(xsdpath + "\\tblTransactionLineItems.xsd");
			LoadDataSet(datasetxml, dsLineItems);


			//Sub Line Items XML 
			startindex = xml.IndexOf(subLineItemStartTag) + subLineItemStartTag.Length;
			endindex = xml.IndexOf(subLineItemEndTag, startindex);
			datasetxml = xml.Substring(startindex, endindex - startindex).Trim();
			if (dsSubLineItems == null)
				dsSubLineItems = new DataSet();
			dsSubLineItems.Clear();
			dsSubLineItems.ReadXmlSchema(xsdpath + "\\tblTransactionSubLineItems.xsd");
			LoadDataSet(datasetxml, dsSubLineItems);

			//Notes Items XML 
			startindex = xml.IndexOf(notesStartTag) + notesStartTag.Length;
			endindex = xml.IndexOf(notesEndTag, startindex);
			datasetxml = xml.Substring(startindex, endindex - startindex).Trim();
			if (dsNotes == null)
				dsNotes = new DataSet();
			dsNotes.Clear();
			dsNotes.ReadXmlSchema(xsdpath + "\\tblTransactionNotes.xsd");
			LoadDataSet(datasetxml, dsNotes);


			//Weight Reading Items XML 
			startindex = xml.IndexOf(weightReadingsStartTag) + weightReadingsStartTag.Length;
			endindex = xml.IndexOf(weightReadingsEndTag, startindex);
			datasetxml = xml.Substring(startindex, endindex - startindex).Trim();
			if (dsWeightReadings == null)
				dsWeightReadings = new DataSet();
			dsWeightReadings.Clear();
			dsWeightReadings.ReadXmlSchema(xsdpath + "\\tblTransactionWeightReadings.xsd");
			LoadDataSet(datasetxml, dsWeightReadings);

			//Transport Line Items XML 
			startindex = xml.IndexOf(this.transportLineItemStartTag) + this.transportLineItemStartTag.Length;
			endindex = xml.IndexOf(this.transportLineItemEndTag, startindex);
			datasetxml = xml.Substring(startindex, endindex - startindex).Trim();

			if (this.dsTransportLineItems == null)
			{
				this.dsTransportLineItems = new DataSet();
			}

			this.dsTransportLineItems.Clear();
			this.dsTransportLineItems.ReadXmlSchema(xsdpath + "\\tblTransactionTransportLineItems.xsd");
			this.LoadDataSet(datasetxml, this.dsTransportLineItems);
		}

		public object GetMaximumSequenceNumber()
		{
			if (dsHeaders == null || dsHeaders.Tables.Count == 0 || dsHeaders.Tables[0].Rows.Count == 0)
				return null;
			Int64 maxvalue = 0;
			foreach (DataRow row in dsHeaders.Tables[0].Rows)
			{
				Int64 value = DataObject.getValue<Int64>(row["TransVersion"], 0);
				maxvalue = (value > maxvalue) ? value : maxvalue;
			}
			return maxvalue;
		}
		#endregion

		#region Private Functions
		private void LoadDataSet(string xml, DataSet ds)
		{
			if (xml.Length == 0)
				return;
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNodeList nodes = doc.SelectNodes("//Table");
			foreach (XmlNode node in nodes)
			{
				DataRow row = ds.Tables[0].NewRow();
				foreach (XmlNode childnode in node.ChildNodes)
				{
					if (ds.Tables[0].Columns.Contains(childnode.Name))
					{
						if (childnode.InnerText.Length == 0 &&
							System.Type.GetTypeCode(ds.Tables[0].Columns[childnode.Name].DataType) != System.TypeCode.String)
							continue;
						switch (System.Type.GetTypeCode(ds.Tables[0].Columns[childnode.Name].DataType))
						{
							case System.TypeCode.Boolean:
								row[childnode.Name] = System.Boolean.Parse(childnode.InnerText);
								break;

							case System.TypeCode.String:
								row[childnode.Name] = childnode.InnerText;
								break;

							case System.TypeCode.DateTime:
								row[childnode.Name] = System.DateTime.Parse(childnode.InnerText);
								break;

							case System.TypeCode.Double:
								row[childnode.Name] = System.Double.Parse(childnode.InnerText);
								break;

							case System.TypeCode.Single:
								row[childnode.Name] = System.Single.Parse(childnode.InnerText);
								break;

							case System.TypeCode.Int16:
								row[childnode.Name] = System.Int16.Parse(childnode.InnerText);
								break;

							case System.TypeCode.Int32:
								row[childnode.Name] = System.Int32.Parse(childnode.InnerText);
								break;

							case System.TypeCode.Int64:
								row[childnode.Name] = System.Int64.Parse(childnode.InnerText);
								break;

							case System.TypeCode.Decimal:
								row[childnode.Name] = System.Decimal.Parse(childnode.InnerText);
								break;

							default:
								break;
						}
					}
				}
				ds.Tables[0].Rows.Add(row);
			}
		}
		#endregion
	}
}
