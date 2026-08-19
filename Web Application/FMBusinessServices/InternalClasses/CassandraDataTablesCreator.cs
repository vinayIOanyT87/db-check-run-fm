namespace FMBusinessServices.InternalClasses
{
	using Cassandra.Data.Linq;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.InternalInterfaces;

	using FMCore;

	internal class CassandraDataTablesCreator : ICassandraDataTablesCreator
	{
		/// <summary>
		///	Creates the archive data table(s)
		/// </summary>
		public void CreateArchiveTables( Cassandra.ISession session, Cassandra.ConsistencyLevel consistencyLevel)
		{
			session.ThrowIfNull("session");

			var valueArchiveDataTable = new Table<ArchiveDataElement>( session );
			valueArchiveDataTable.SetConsistencyLevel(consistencyLevel);
			valueArchiveDataTable.CreateIfNotExists();

			var alarmArchiveDataTable = new Table<AlarmDataElement>(session);
			alarmArchiveDataTable.SetConsistencyLevel(consistencyLevel);
			alarmArchiveDataTable.CreateIfNotExists();

			var synchronizationTable = new Table<SynchronizationElement>(session);
			synchronizationTable.CreateIfNotExists();
		}
		public void CreateAandETables(Cassandra.ISession session, Cassandra.ConsistencyLevel consistencyLevel)
		{
			session.ThrowIfNull("session");

			var archiveDataTable = new Table<AandEDataElement>(session);
			archiveDataTable.SetConsistencyLevel(consistencyLevel);
			archiveDataTable.CreateIfNotExists();

			var synchronizationTable = new Table<AlarmAndEventSynchronizationElement>(session);
			synchronizationTable.CreateIfNotExists();
		}
	}
}