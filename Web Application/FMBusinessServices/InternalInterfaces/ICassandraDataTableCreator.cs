namespace FMBusinessServices.InternalInterfaces
{
	using Cassandra;

	internal interface ICassandraDataTablesCreator
	{
		/// <summary>
		///	Creates the archive data table(s)
		/// </summary>
		void CreateArchiveTables( ISession session, Cassandra.ConsistencyLevel consistencyLevel);
		void CreateAandETables(ISession session, Cassandra.ConsistencyLevel consistencyLevel);
	}
}
