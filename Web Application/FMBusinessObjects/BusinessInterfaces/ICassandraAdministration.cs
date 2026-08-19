namespace FMBusinessObjects.BusinessInterfaces
{

	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ServiceModel;

	using DataObjects;
	using Cassandra;

	[InheritedExport]
	[ServiceContract]
	public interface ICassandraAdministration
	{

		[OperationContract]
		bool CassandraUserUpdate(SecurityClass security, string[] credentials);
	}
}
