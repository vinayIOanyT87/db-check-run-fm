namespace FMBusinessServices.InternalInterfaces
{
	using Cassandra;

	internal interface ICassandraKeyspaceCreator
	{
		/// <summary>
		/// Creates the KeySpace in Cassandra
		/// </summary>
		void CreateKeySpaceIfNotExists( ISession session, int ReplicationFactor, string KeyspaceName);
	}
}
