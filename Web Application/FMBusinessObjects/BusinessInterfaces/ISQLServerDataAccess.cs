namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
    using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface ISQLServerArchiveDataAccess
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
        void AddArchiveData(SecurityClass security, DataSet archiveData);

        [OperationContract]
        DataSet ReadArchiveRecord(SecurityClass security, DateTime startTime, DateTime endTime, string nodeID);
    }
}
