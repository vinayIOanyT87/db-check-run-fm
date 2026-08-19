using CqlSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMPointTagArchive.Core
{
	[CqlTable("TankChangePartitionKeyClass")]
	public class TankChangePartitionKeyClass
	{
		// PointValueGuid which is either the PointTagGuid or the PointPropertyGuid
		[CqlColumn("a", Order = 0)]
		[CqlKey]
		public Guid A { get; set; } //only on CqlKey attribute used, so this will be the partition key

		// PropertyID which is string.Empty for Tag and PropertyID for Exposed Settings
		[CqlColumn("b", Order = 1)]
		[CqlKey]
		public string B { get; set; }

		// Year * 100 + Month partition.
		[CqlColumn("c", Order = 2)]
		[CqlKey]
		public int C { get; set; }
	}

}
