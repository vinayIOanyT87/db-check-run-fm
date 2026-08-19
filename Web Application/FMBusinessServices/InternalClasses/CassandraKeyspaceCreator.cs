namespace FMBusinessServices.InternalClasses
{
	using System.Collections.Generic;

	using Cassandra;

	using FMBusinessObjects.UtilityObjects;

	using FMBusinessServices.InternalInterfaces;

	using FMCore;

	public class CassandraKeyspaceCreator : ICassandraKeyspaceCreator
	{
		//public static string KeyspaceName = "FMArchive_Data";

		/// <summary>
		/// Creates the KeySpace in Cassandra
		/// </summary>
		public void CreateKeySpaceIfNotExists( ISession session, int ReplicationFactor, string KeyspaceName)
		{
			session.ThrowIfNull("session");

			// this will create the keyspace if it does not exist. Keyspace is roughly the same as the database
			var replication = new Dictionary<string, string>
			{
				{"class", "SimpleStrategy"},
				{"replication_factor", ReplicationFactor.ToString()}
			};

			// ReSharper disable once RedundantArgumentDefaultValue
			session.CreateKeyspaceIfNotExists( KeyspaceName, replication, durableWrites: true );
			session.ChangeKeyspace( KeyspaceName );
		}
	}
}
