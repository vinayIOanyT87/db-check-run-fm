namespace FMBusinessObjects.UtilityObjects
{
	using System;

	using DataObjects;
	using Cassandra.Mapping;

	public class CassandraAandETableMappings : Mappings
	{
		public CassandraAandETableMappings()
		{
			For<AandEDataElement>()
				.TableName("ArchiveData")
				.Column(u => u.PointDescription, cm => cm.WithName("a"))
				.Column(u => u.AlarmOrTagGuid, cm => cm.WithName("b"))
				.Column(u => u.AlarmTestGuid, cm => cm.WithName("c"))
				.Column(u => u.Point, cm => cm.WithName("d"))
				.Column(u => u.Site, cm => cm.WithName("e"))
				.Column(u => u.SiteGuid, cm => cm.WithName("f"))
				.Column(u => u.DateAndTime, cm => cm.WithName("g"))
				.Column(u => u.AlarmState, cm => cm.WithName("h"))
				.Column(u => u.PointType, cm => cm.WithName("i"))
				.Column(u => u.Variable, cm => cm.WithName("j"))
				.Column(u => u.Value, cm => cm.WithName("k"))
				.Column(u => u.Units, cm => cm.WithName("l"))
				.Column(u => u.Priority, cm => cm.WithName("m"))
				.Column(u => u.Action, cm => cm.WithName("n"))
				.Column(u => u.User, cm => cm.WithName("o"))
				.Column(u => u.Comments, cm => cm.WithName("p"))
				.Column(u => u.RecordType, cm => cm.WithName("q"))
				.Column(u => u.RecordGuid, cm => cm.WithName("r"))
				.Column(u => u.CommentUser, cm => cm.WithName("s"))
				.Column(u => u.CommentDateTime, cm => cm.WithName("t"))
				.Column(u => u.Partition, cm => cm.WithName("u"))
				.PartitionKey(u => u.SiteGuid, u => u.Partition)
				.ClusteringKey(new Tuple<string, SortOrder>[] { new Tuple<string, SortOrder>("g", SortOrder.Ascending), new Tuple<string, SortOrder>("r", SortOrder.Ascending) });

			this.For<AlarmAndEventSynchronizationElement>().TableName("SynchronizationData").PartitionKey("TableName", "SiteGuid");

		}
	}
}
