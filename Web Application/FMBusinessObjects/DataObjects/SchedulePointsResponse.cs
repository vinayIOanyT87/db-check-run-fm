
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public enum SchedulePointsStatus
    {
        Good,
        MaxRowVersionCheckFailed,
        Bad,
        InvalidSecurity,
        NoPointsAssigned
    };

    [Serializable]
    public class SchedulePointsResponse
    {
        [DataMember]
        public PointChecksumCollection PointCheckSums;

        [DataMember]
        public SchedulePointsStatus Status;
    }

    [Serializable]
    [CollectionDataContract]
    public class PointChecksumCollection : List<PointChecksum>
    {
    }

    [Serializable]
    public class PointChecksum
    {
        [DataMember]
        public Guid PointGuid;

        [DataMember]
        public long MaxRowVersion;

    }
}
