using FMBusinessObjects.DataObjects;

namespace FMBusinessServices.InternalInterfaces
{
	internal interface ICassandraConnectionConfig
	{
        string[] GetContactPoints(SecurityClass security);
		int GetReplicationFactor(SecurityClass security);
		string GetConsistencyLevel(SecurityClass security);

		string[] GetCredentials(SecurityClass security);
	}
}
