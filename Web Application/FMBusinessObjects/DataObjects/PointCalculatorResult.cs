using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	using Opc.Ua;
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class PointCalculatorResult
	{
		[DataMember]
		public Guid RunGuid { get; set; }
		[DataMember]
		public string PointId { get; set; }
		[DataMember]
		public Guid PointGuid { get; set; }
		[DataMember]
		public string SiteId { get; set; }
		[DataMember]
		public Guid SiteGuid { get; set; }
		[DataMember]
		public string UserId { get; set; }
		[DataMember]
		public Guid UserGuid { get; set; }
		[DataMember]
		public Guid Token { get; set; }
		[DataMember]
		public string CalculationMode { get; set; }
		[DataMember]
		public List<PointCalculatorTagValue> TagValues { get; set; }
	}
}
