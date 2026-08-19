namespace FMBusinessServices.ServiceClasses
{

	using System.Collections.Generic;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ChannelFactories;

	using DataAccessLayer;
	using InternalInterfaces;
	using System;
	using Cassandra;

	[SecuritySafeCritical]
	public class CassandraAdministration : FMServiceBase, ICassandraAdministration
	{
		private static readonly ICassandraUtility CassandraInstance = new CassandraUtilityDAO();

		//Updates Cassandra Users when the configuration setting is changed.
		public bool CassandraUserUpdate(SecurityClass security, string[] credentials)
		{
			//PointTagArchiveDatabase.Initialize(security);
			return CassandraInstance.CreateOrModifyCassandraUser(security, credentials);
		}
	}
}