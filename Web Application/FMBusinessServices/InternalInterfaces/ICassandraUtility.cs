namespace FMBusinessServices.InternalInterfaces
{
	using FMBusinessObjects.DataObjects;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;

	internal interface ICassandraUtility
	{
		void InitializeWithCredentials(SecurityClass security, string username, string password);
		bool CreateOrModifyCassandraUser(SecurityClass security, string[] credentials);
	}
}