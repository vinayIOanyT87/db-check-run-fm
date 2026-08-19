using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	[Serializable]
	public class ExStarsDateTimeSegment : ExStarsSegment
	{
		public ExStarsDateTimeSegment(string qualifier, int year, int month, int day)
			: this(qualifier, new DateTime(year, month, day)) { }

		public ExStarsDateTimeSegment(string qualifier, DateTime date)
			: base("DTM", "Date/Time Reference")
		{
			this.AddElement(1, "Date/Time Qualifier", "", EnumExStarsElementTypes.ID, 3, 3, qualifier);
			this.AddElement(2, "Date","",EnumExStarsElementTypes.DT,8,8,date.ToString("yyyyMMdd"));
		}
	}
}
