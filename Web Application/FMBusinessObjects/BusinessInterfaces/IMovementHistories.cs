namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface IMovementHistories
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, MovementData movementData);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void AddByList(SecurityClass security, List<MovementHistoryDO> movementHistoryDoList);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass security, MovementHistoryDO movementHistoryDo);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteByMovementName(SecurityClass security, Guid movementHistoryGuid, string movementName);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateComment(SecurityClass security, Guid movementHistoryGuid, string comment, string commentUserId, DateTime commentDateTime);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateHandgaugeFromHistory(SecurityClass security, MovementHistoryDO movementHistoryDo, bool updateFinalRecord);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateNodeDataToFinalRecord(SecurityClass security, MovementHistoryDO movementHistoryDo);

        [OperationContract]
        List<MovementHistoryDO> GetMovementByMovementName(SecurityClass security, string movementName);

        [OperationContract]
        List<MovementHistoryDO> GetAllMovementsBySiteGuid(SecurityClass security
                                                        , Guid siteGuid
                                                        , DateTime startTime
                                                        , DateTime endTime
                                                        , bool autoGauge
                                                        , bool handGauge
                                                        , bool midnightRecord
                                                        , string orderColumnName
                                                        , string orderDirection);

        [OperationContract]
        List<MovementHistoryDO> GetMovementsByInitialLoadRequest(SecurityClass security, Guid siteGuid, int initialLoadCount);

        [OperationContract]
        MovementHistoryDO GetMovementRecordByGuid(SecurityClass security, Guid movementHistoryGuid);

		  [OperationContract]
		  void PrintMovementTicket(SecurityClass security, Guid movementHistoryGuid, bool automatic);
   
         [OperationContract]
         void ArchiveMovementTicket(SecurityClass security, Guid movementHistoryGuid, string movementID = "");
   }
}
