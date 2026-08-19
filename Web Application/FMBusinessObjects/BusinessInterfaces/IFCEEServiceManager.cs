
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	 using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
	using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

	[ServiceContract]

	public interface IFCEEServiceManager
	{
        [OperationContract]
        [FaultContractAttribute(typeof(FMAlarmAndEventLogException))]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void PurgeOldRecords(SecurityClass security);

        [OperationContract]
		Guid? Add(SecurityClass security, FCEEMapping fceeMapping);

		[OperationContract]
		void Modify(SecurityClass security, FCEEMapping fceeMapping);

		[OperationContract]
		Guid? GetFCEEToPointGuid(SecurityClass security, string imeiNumber, int msgType, int index);

		[OperationContract]
		Tuple<string, Guid, string, Guid, long, int?> GetMapping(SecurityClass security, string imeiNumber, int msgType, int index, int? device = null);

		[OperationContract]
		void Purge(SecurityClass security, Guid fceeMappingGuid);

		[OperationContract]
		void AddMessage(SecurityClass security, string imeiNumber, DateTimeOffset timeStamp, int msgType, int index, byte? device, byte[] binaryData, string edgeData, bool validity);

        [OperationContract]
        Dictionary<Guid, Tuple<string, Guid, string, Guid, long>> EnumerateBySiteGuid(SecurityClass security, Guid siteGuid);

        [OperationContract]
        Dictionary<Guid, FCEEMapping> EnumerateBySiteGuid2(SecurityClass security, Guid siteGuid);

		[OperationContract]
		Dictionary<Guid, FCEEMappingWithDevice> EnumerateBySiteGuidWithDevice(SecurityClass security, Guid siteGuid);

		[OperationContract]
        Dictionary<Guid, FCEEMapping> EnumerateByPointGuid(SecurityClass security, Guid pointGuid);

        [OperationContract]
        FCEEMapping Get(SecurityClass security, Guid mappingGuid);
        [OperationContract]
        void UpdateFCEEMappings(SecurityClass security, List<FCEEMapping> fceeMappings);
        [OperationContract]
        List<FCEEMessage> EnumerateMessages(SecurityClass security, string startDate, string endDate);

        [OperationContract]

        /// <summary>
        /// When a FCEE Value override/force is removed
        /// If the Heartbeat is good (or the value is from the past 24 hours), set the value to the last known good value
        /// Otherwise, set the value to null with data quality unknown
        /// </summary>
        /// <param name="security">Contains Security Information</param>
        /// <param name="pointValue">The FCEE point value that needs to have its value and quality updated from force removal</param>
        bool Refresh(SecurityClass security, PointValue pointValue);

        [OperationContract]
        Tuple<bool, int, byte[]> ProcessRequestHandler(SecurityClass security, bool pointStatusProcessing, MemoryStream memoryStream, string contentType, string httpMethod);
        [OperationContract]
        void ProcessFceHeartbeats(SecurityClass security);
    }
}
