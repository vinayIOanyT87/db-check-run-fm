namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;

	public class FCEETupleMapping
    {
		public string SiteID { get; set; }

		public Guid SiteGuid { get; set; }

		public string PointID { get; set; }

		public Guid PointGuid { get; set; }

		public long RowVersion { get; set; }
		public int? TagSelection {  get; set; }

		public List<PointValueIdentifier> PointValueIdentifierList { get; set; }

		public FCEETupleMapping(Tuple<string, Guid, string, Guid, long, int?> mapping)
		{
			SiteID = mapping.Item1;
			SiteGuid = mapping.Item2;
			PointID = mapping.Item3;
			PointGuid = mapping.Item4;
			RowVersion = mapping.Item5;
			TagSelection = mapping.Item6;
			
			PointValueIdentifierList = new List<PointValueIdentifier>();
		}
	}
}
