using FMBusinessObjects.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FMBusinessObjects.DataObjects
{
    public enum EDGEMESSAGETYPE : int
    {
        Invalid = 0x0000,
        Heartbeat = 0x0001,
        SoftwareVersion = 0x0002,
        DeviceStatus = 0x0003,
        Enraf854TankGauge = 0x0004,
        Enraf854TankGaugeDensity = 0x0005,
        ModbusIntegerRegisterBlock = 0x0006,
        GenericScalingPoint = 0x0007,
        ITTBarton3500ATG = 0x0008,
        VeederRootTLS350 = 0x0009,
        VeederRootSystemStatus = 0x000A,
        VeederRootLeakTest = 0x000B,
        VeederRootSystemAlarms = 0x000C,
        VeederRootInventoryReport = 0x000D,
        VeederRootInTankStatusReport = 0x000E,
        VeederRootLiquidSensorStatusReport = 0x000F,
        ModbusInventory = 0x0010,
        ModbusDensityAndAlarm = 0x0011,
        ModbusFacilityStatus = 0x0012,
        ModbusStorage = 0x0013,
        CommandStatus = 0x0014,
        WAGOPLC = 0x0015,
        ForwardedInvalid = 0x8000,
        ForwardedHeartbeat = 0x8001,
        ForwardedSoftwareVersion = 0x8002,
        ForwardedDeviceStatus = 0x8003,
        ForwardedEnraf854TankGauge = 0x8004,
        ForwardedEnraf854TankGaugeDensity = 0x8005,
        ForwardedModbusIntegerRegisterBlock = 0x8006,
        ForwardedGenericScalingPoint = 0x8007,
        ForwardedITTBarton3500ATG = 0x8008,
        ForwardedVeederRootTLS350 = 0x8009,
        ForwardedVeederRootSystemStatus = 0x800A,
        ForwardedVeederRootLeakTest = 0x0800B,
        ForwardedVeederRootSystemAlarms = 0x800C,
        ForwardedVeederRootInventoryReport = 0x800D,
        ForwardedVeederRootInTankStatusReport = 0x800E,
        ForwardedVeederRootLiquidSensorStatusReport = 0x800F,
        ForwardedModbusInventory = 0x8010,
        ForwardedModbusDensityAndAlarm = 0x8011,
        ForwardedModbusFacilityStatus = 0x8012,
        ForwardedModbusStorage = 0x8013,
        ForwardedCommandStatus = 0x8014,
        ForwardedWAGOPLC = 0x8015
    }
    public enum TAGSELECTIONTYPE : int
    {
        [Display(Name = "")]
        None = 0,
        [Display(Name="Level Product")]
        LevelProduct =1,
        [Display(Name="Temperature Product")]
        TemperatureProduct =2,
        [Display(Name="Level Water")]
        LevelWater=3
    }
    public class FCEEMapping : BaseDataObject
    {
        [DataMember]
        [FMPersistedField]
        public Guid? FCEDeviceGuid { get; set; }

        [DataMember]
        [FMPersistedField]
        public EDGEMESSAGETYPE MsgType { get; set; }

        [DataMember]
        [FMPersistedField]
        public int Index { get; set; }

        [DataMember]
        [FMPersistedField]
        public int? Device { get; set; }

        [DataMember]
        [FMPersistedField]
        public TAGSELECTIONTYPE TagSelection { get; set; }

        [DataMember]
        [FMPersistedField]
        public Guid? PointGuid { get; set; }

        [FMPersistedField]
        public Guid FCEEMappingGuid
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

        [DataMember]
        [FMPersistedField]
        public string PointID { get; set; }


        public void EnumerateByFCEEMappingGuidSQL(SqlCommand cmd, Guid mappingGuid)
        {
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "SELECT s.SiteGuid, p.ID AS PointID, p.PointGuid, fm.FCEEMappingGuid, fm.MsgType, fm.[Index], fm.[Device], fm.[TagSelection], d.FCEDeviceGuid FROM [dbo].[tblFCEEMapping] fm"
                                       + " LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = fm.PointGuid"
                                       + " LEFT JOIN [dbo].[tblSites] s ON s.SiteGuid = p.SiteGuid"
                                       + " LEFT JOIN [dbo].[tblFCEDevice] d ON d.FCEDeviceGuid = fm.FCEDeviceGuid"
                                       + " WHERE fm.FCEEMappingGuid = @mappingGuid";
            cmd.Parameters.AddWithValue("@mappingGuid", mappingGuid);
        }

        public void EnumerateBySiteGuidSQL(SqlCommand cmd, Guid siteGuid)
        {
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "SELECT s.SiteGuid, p.ID AS PointID, p.PointGuid, fm.FCEEMappingGuid, fm.MsgType, fm.[Index], fm.[Device], fm.[TagSelection], d.FCEDeviceGuid FROM [dbo].[tblFCEEMapping] fm"
                                       + " LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = fm.PointGuid"
                                       + " LEFT JOIN [dbo].[tblSites] s ON s.SiteGuid = p.SiteGuid"
                                       + " LEFT JOIN [dbo].[tblFCEDevice] d ON d.FCEDeviceGuid = fm.FCEDeviceGuid"
                                       + " WHERE s.SiteGuid = @siteGuid";
            cmd.Parameters.AddWithValue("@siteGuid", siteGuid);
        }
        public void EnumerateByPointGuidSQL(SqlCommand cmd, Guid pointGuid)
        {
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "SELECT s.SiteGuid, p.ID AS PointID, p.PointGuid, fm.FCEEMappingGuid, fm.MsgType, fm.[Index], fm.[Device], fm.[TagSelection], d.FCEDeviceGuid FROM [dbo].[tblFCEEMapping] fm"
                                       + " LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = fm.PointGuid"
                                       + " LEFT JOIN [dbo].[tblSites] s ON s.SiteGuid = p.SiteGuid"
                                       + " LEFT JOIN [dbo].[tblFCEDevice] d ON d.FCEDeviceGuid = fm.FCEDeviceGuid"
                                       + " WHERE p.PointGuid = @pointGuid";
            cmd.Parameters.AddWithValue("@pointGuid", pointGuid);
        }
        public static string GetTagSelectionTypeTagName(TAGSELECTIONTYPE t)
        {
            string tagName = string.Empty;
            MemberInfo[]info = t.GetType().GetMember(t.ToString());
            if (info.Length > 0)
            {
                var x= info[0].GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (x != null) 
                { 
                    tagName = x.Name; 
                }
   
            }
            return tagName;
        }
    }
}
