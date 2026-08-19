using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[System.Serializable]
	[XmlRoot("Transaction")]
	[XmlType("Transaction")]
	[DataContract]
	public class WeightedAverageCostDO : DataObject
	{
		#region Constructors
		public WeightedAverageCostDO()
		{
			this.SiteGuid = Guid.Empty;
			this.ProductGuid = Guid.Empty;
			this.WacValue = -1;
			this.WeightedAverageCostGuid = Guid.Empty;
			this.IsManualOverride = true;
			this.Source = "SYSTEM";
			this.Notes = "";
			this.CreatedDate = DateTimeOffset.Now;
			this.CreatedBy = "SYSTEM";
			this.UpdatedDate = this.CreatedDate;
			this.UpdatedBy = this.CreatedBy;
			this.InventoryDate = null;
		}
		#endregion // Constructors

		#region Properties
		[DataMember]
		public Guid SiteGuid { get; set; }
		[DataMember]
		public Guid ProductGuid { get; set; }
		[DataMember]
		public double WacValue { get; set; }
		[DataMember]
		public bool IsManualOverride { get; set; }
		[DataMember]
		public string Source { get; set; }
		[DataMember]
		public string Notes { get; set; }
		[DataMember]
		public Guid WeightedAverageCostGuid { get; set; }
		[DataMember]
		public DateTimeOffset CreatedDate { get; set; }
		[DataMember]
		public string CreatedBy { get; set; }
		[DataMember]
		public DateTimeOffset UpdatedDate { get; set; }
		[DataMember]
		public string UpdatedBy { get; set; }
		[DataMember]
		public DateTime? InventoryDate { get; set; }
		#endregion // Properties

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
		#endregion // Overrides
	}
}
