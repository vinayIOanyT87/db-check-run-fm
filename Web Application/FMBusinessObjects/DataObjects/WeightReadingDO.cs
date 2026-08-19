using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;


namespace FMBusinessObjects.DataObjects
{
	[XmlType("WeightReading")]
	[Serializable]
	[DataContract]
	public class WeightReadingDO : DataObject
	{
		#region Protected data members
		[DataMember]
		protected string compartmentName;
		[DataMember]
		protected double? beginQty;
		[DataMember]
		protected double? requestedQty;
		[DataMember]
		protected double? finalQty;
		#endregion

		#region Properties

		public string CompartmentName
		{
			get { return compartmentName; }
			set { compartmentName = value; }
		}

		public double? BeginQuantity
		{
			get { return beginQty; }
			set { beginQty = value; }
		}

		public double? RequestedQuantity
		{
			get { return requestedQty; }
			set { requestedQty = value; }
		}

		public double? FinalQuantity
		{
			get { return finalQty; }
			set { finalQty = value; }
		}

		[DataMember]
		public bool? VolumetricTopOffFlag { get; set; }

		[DataMember]
		public int FuelsManagerVersionNumber { get; set; }

		[DataMember]
		public int ? SourceVersionNumber { get; set; }

		[DataMember]
		public bool HistoricalFlag { get; set; }

		#endregion Properties

		public WeightReadingDO()
		{
			this.compartmentName = "";
			this.beginQty = 0.0;
			this.finalQty = 0.0;
			this.requestedQty = 0.0;
			this.FuelsManagerVersionNumber = 0;
			this.SourceVersionNumber = null;
			this.HistoricalFlag = false;
			this.VolumetricTopOffFlag = false;
		}

		#region Overrides
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getDeleteCommand()
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
		#endregion Overrides
	}
}
