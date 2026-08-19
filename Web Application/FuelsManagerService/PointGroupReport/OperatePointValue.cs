using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FuelsManagerService.PointGroupReport
{
    // Copied from namespace FuelsManager.Areas.InventoryManagement.ViewModels.OperateModel
    public class OperatePointValue
    {
        public Guid SiteGuid { get; set; }
        public string SiteID { get; set; }
        public Guid PointValueIdentifier_IdentityGuid { get; set; }

        public PointValueType PointValueIdentifier_PointValueType { get; set; }

        public string PointValueIdentifier_PropertyID { get; set; }

        public string PointValueIdentifier_UtcTicks { get; set; }

        public Guid IdentityGuid { get; set; }

        public PointValueType PointValueType { get; set; }

        public string PropertyID { get; set; }

        public Guid PointGuid { get; set; }

        public string PointID { get; set; }

        public object Value { get; set; }

        public string ValueTypeString { get; set; }

        public long Status { get; set; }

        public string ID { get; set; }

        public DateTimeOffset ServerTimeStamp { get; set; }

        public EngineeringUnit Units { get; set; }

        public int DecimalPlaces { get; set; }

        public double Maximum { get; set; }

        public double Minimum { get; set; }

        public string QualityAbbreviation { get; set; }

        public EngineeringUnitType EngineeringUnitsType { get; set; }

        public bool Acknowledged { get; set; }

        public Guid AlarmPriorityGuid { get; set; }

        public string AlarmState { get; set; }

        public string ProductColor { get; set; }

        public string PatternColor { get; set; }

        public int PatternNumber { get; set; }

        public bool HasProductGraphicInfo { get; set; }

        public List<AlarmLimitValue> AlarmLimits { get; set; }

        public PointValueAccess Access { get; set; }

        public PointTemplateTag.PointTagInputOutputType InputOutputType { get; set; }

        public bool InhibitOverride { get; set; }

        public bool CommunicationsFailure { get; set; }

        public OperatePointValue()
        {
            this.AlarmLimits = null;
        }

        public OperatePointValue(PointValue p)
        {
            this.SiteGuid = p.PointValueIdentifier.SiteGuid;
            this.SiteID = p.SiteID;
            this.PointValueIdentifier_IdentityGuid = p.PointValueIdentifier.IdentityGuid;
            this.PointValueIdentifier_PointValueType = p.PointValueIdentifier.PointValueType;
            this.PointValueIdentifier_PropertyID = p.PointValueIdentifier.PropertyID;
            this.PointValueIdentifier_UtcTicks = p.ServerTimeStamp.UtcTicks.ToString();
            this.IdentityGuid = p.PointValueIdentifier.IdentityGuid;
            this.PointValueType = p.PointValueIdentifier.PointValueType;
            this.PropertyID = p.PointValueIdentifier.PropertyID;
            this.PointGuid = p.PointGuid;
            this.PointID = p.PointID;
            this.Value = p.Value;
            this.ValueTypeString = p.ValueTypeString;
            this.Status = p.Status;
            this.ID = (p.PointValueIdentifier.PointValueType == PointValueType.Point ? p.PointValueIdentifier.PropertyID : p.ID);
            this.ServerTimeStamp = p.ServerTimeStamp;
            this.Units = p.Units;
            this.DecimalPlaces = p.DecimalPlaces;
            this.Maximum = p.Maximum;
            this.Minimum = p.Minimum;
            this.QualityAbbreviation = p.QualityAbbreviation;
            this.EngineeringUnitsType = p.EngineeringUnitsType;
            this.Acknowledged = p.Acknowledged;
            this.AlarmPriorityGuid = p.AlarmPriorityGuid;
            this.AlarmState = p.AlarmState;
            this.ProductColor = p.ProductColor;
            this.PatternColor = p.PatternColor;
            this.PatternNumber = p.PatternNumber;
            this.HasProductGraphicInfo = p.HasProductGraphicInfo;
            this.Access = p.Access;
            this.InputOutputType = p.InputOutputType;
            this.InhibitOverride = p.InhibitOverride;
            this.AlarmLimits = null;
            this.CommunicationsFailure = false;
            if (p.AlarmLimitList != null && p.AlarmLimitList.Count > 0)
            {
                this.AlarmLimits = p.AlarmLimitList;
            }
        }
    }
}