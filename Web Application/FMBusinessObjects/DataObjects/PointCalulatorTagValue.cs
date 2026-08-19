using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	[DataContract]
	[Serializable]
	public class PointCalculatorTagValue : IEquatable<PointCalculatorTagValue>
	{
		[DataMember]
		public string Tagname { get; set; }

		[DataMember]
		public string Units { get; set; }

		[DataMember]
		public string Acronym { get; set; }

		[DataMember]
		public string BeginValue { get; set; }

		[DataMember]
		public string EndValue { get; set; }

		[DataMember]
		public string DiffValue { get; set; }

		[DataMember]
		public int DisplayOrder { get; set; }

		public PointCalculatorTagValue(string tagName, string beginValue, string endValue, int order, string units = "none", string acronym = "", string diffValue = "")
		{ 
			Tagname = tagName;
			Units = units;
			Acronym = acronym;
			BeginValue = beginValue;
			EndValue = endValue;
			DiffValue = diffValue;
			DisplayOrder = order;
		}

		public bool Equals(PointCalculatorTagValue tagValue)
		{
			if (tagValue == null)
			{
				return false;
			}

			return tagValue.Tagname == this.Tagname;

		}
	}
}
