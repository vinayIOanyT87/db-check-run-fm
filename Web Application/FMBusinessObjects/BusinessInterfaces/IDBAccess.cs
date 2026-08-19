namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;
	using System.Collections.Generic;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
	public interface IDBAccess
	{
		[OperationContract]
		string GetDBPassword ( string unmangledPassword );

		[OperationContract]
		string ServiceLogin( SecurityClass security );

		[OperationContract]
		VersionInfo GetVersion();

        [OperationContract]
        Guid SiteAdminGuid();

		[OperationContract]
		Dictionary<string, ForeignKeyDO> EnumerateForeignKeys(SecurityClass security, string schema, string tableName);
	}
}
