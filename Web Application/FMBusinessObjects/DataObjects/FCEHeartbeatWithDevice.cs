using FMBusinessObjects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    [DataContract]
    [Serializable]
    public class FCEDeviceWithLastHeartbeat : BaseDataObject
    {
        [FMPersistedField]
        [DataMember]
        public Guid FCEDeviceGuid
        {
            get
            {
                return this.IdentityGuid;
            }
            set
            {
                this.IdentityGuid = value;
            }
        }
        [FMPersistedField]
        [DataMember]
        public string ImeiNumber { get; set; }

        [FMPersistedField]
        [DataMember]
        public int MsgType { get; set; }
        [FMPersistedField]
        [DataMember]
        public DateTimeOffset Timestamp { get; set; }
        [FMPersistedField]
        [DataMember]
        public Byte[] BinaryData { get; set; }
        [FMPersistedField]
        [DataMember]
        public string EdgeData { get; set; }
        [FMPersistedField]
        [DataMember]
        public int Heartbeat { get; set; }
        [FMPersistedField]
        [DataMember]
        public bool HeartbeatTimeoutProcessed { get; set; }

    }
}
