namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Xml.Serialization;
    using System.Runtime.Serialization;


    [XmlType("SyncRecordConflictCount")]
    [DataContract]
    [Serializable]

    public class SyncRecordConflictCountDO
    {
        #region Properties
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public DateTimeOffset OldestDate { get; set; }
        #endregion Properties
    }
}
