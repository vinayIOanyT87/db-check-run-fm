namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using DataObjects;
	using Cassandra.Mapping;

	public class CassandraArchiveTableMappings : Mappings
	{
		public CassandraArchiveTableMappings()
		{
			For<ArchiveDataElement>()
				.TableName("ValueArchiveData")
				.Column(u => u.PointValueGuid,cm => cm.WithName("A"))
				.Column(u => u.PropertyID, cm => cm.WithName("B"))
				.Column(u => u.Partition, cm => cm.WithName("C"))
				.Column(u => u.Value, cm => cm.WithName("D"))
				.Column(u => u.ValueOpcStatus, cm => cm.WithName("E"))
				.Column(u => u.ValueTimeStamp, cm => cm.WithName("F"))
				.Column(u => u.ArchiveRecordType, cm => cm.WithName("G"))
				.Column(u => u.DataType, cm => cm.WithName("H"))
				.Column(u => u.EngineeringUnitsIndex, cm => cm.WithName("I"))
				.Column(u => u.AlarmPriorityGuid, cm => cm.WithName("J"))
				.Column(u => u.Acknowledged, cm => cm.WithName("K"))
				.Column(u => u.AlarmState, cm => cm.WithName("L"))
				.Column(u => u.AlarmOrStatusChanged, cm => cm.WithName("M"))
				.Column(u => u.RecordTimeStamp, cm => cm.WithName("N"))
				.Column(u => u.QualityString, cm => cm.WithName("O"))
				.Column(u => u.SiteGuid, cm => cm.Ignore())
				.PartitionKey(u => u.PointValueGuid, u => u.PropertyID, u => u.Partition)
				.ClusteringKey(u => u.ValueTimeStamp, SortOrder.Ascending);

			For<AlarmDataElement>()
				.TableName("AlarmArchiveData")
				.Column(u => u.PointValueGuid, cm => cm.WithName("A"))
				.Column(u => u.PropertyID, cm => cm.WithName("B"))
				.Column(u => u.Value, cm => cm.WithName("D"))
				.Column(u => u.ValueOpcStatus, cm => cm.WithName("E"))
				.Column(u => u.ValueTimeStamp, cm => cm.WithName("F"))
				.Column(u => u.ArchiveRecordType, cm => cm.WithName("G"))
				.Column(u => u.DataType, cm => cm.WithName("H"))
				.Column(u => u.EngineeringUnitsIndex, cm => cm.WithName("I"))
				.Column(u => u.AlarmPriorityGuid, cm => cm.WithName("J"))
				.Column(u => u.Acknowledged, cm => cm.WithName("K"))
				.Column(u => u.AlarmState, cm => cm.WithName("L"))
				.Column(u => u.AlarmOrStatusChanged, cm => cm.WithName("M"))
				.PartitionKey(u => u.PointValueGuid, u => u.PropertyID)
				.ClusteringKey(u => u.ValueTimeStamp, SortOrder.Ascending);

			this.For<SynchronizationElement>().TableName("SynchronizationData").PartitionKey("TableName", "SiteGuid");

		}
	}
}
