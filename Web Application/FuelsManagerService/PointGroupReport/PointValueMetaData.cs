using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FuelsManagerService.PointGroupReport
{
    public class PointValueMetaData
    {
        public Guid IdentityGuid { get; set; }
        public object PointValueType { get; set; }
        public string PropertyID { get; set; }
        public object UtcTicks { get; set; }
        public Guid PointGuid { get; set; }
        public Guid PointTagGuid { get; set; }
        public string ID { get; set; }
        public object Units { get; set; }
        public object Maximum { get; set; }
        public object Minimum { get; set; }
        public object DecimalPlaces { get; set; }
        public object EngineeringUnitsType { get; set; }
        public object InhibitOverride { get; set; }
        public object WellKnownIdentityGuid { get; set; }
        public object InputOutputType { get; set; }
        public object Status { get; set; }
        public object CommunicationsFailure { get; set; }
    }
}