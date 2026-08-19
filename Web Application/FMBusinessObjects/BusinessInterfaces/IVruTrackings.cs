namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;

    using DataObjects;

    [ServiceContract]
    public interface IVruTrackings
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        Guid Add(SecurityClass security, VruTrackingClass vcu);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Modify(SecurityClass security, VruTrackingClass vcu);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateResetDate(SecurityClass security, Guid vruTrackingGuid);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void Purge(SecurityClass security, Guid vruTrackingGuid);

        [OperationContract]
        VruTrackingClass Get(SecurityClass security, Guid vruTrackingGuid);

        [OperationContract]
        bool IsThresholdExceeded(SecurityClass security);

        [OperationContract]
        VRUTrackingCollectionClass Enumerate(SecurityClass security);

        [OperationContract]
        VRUTrackingCollectionClass EnumerateThresholdsExceeded(SecurityClass security);

        [OperationContract]
        void CalculateRunningTotals(SecurityClass security);

        [OperationContract]
        void AggregateDailyBolTotals(SecurityClass security);

        [OperationContract]
        int? GetAutoCalculationInterval(SecurityClass security);
    }
}
