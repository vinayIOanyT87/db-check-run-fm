using FMBusinessObjects.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
    public class FCEEMappingWithDevice : FCEEMapping
    {
        [DataMember]
        [FMPersistedField]
        public string FriendlyName { get; set; }

        [DataMember]
        [FMPersistedField]
        public string ImeiNumber { get; set; }

        public new void EnumerateBySiteGuidSQL(SqlCommand cmd, Guid siteGuid)
        {
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "SELECT s.SiteGuid, p.ID AS PointID, p.PointGuid, fm.FCEEMappingGuid, fm.MsgType, fm.[Index], fm.[Device], fm.[TagSelection], d.FCEDeviceGuid, d.FriendlyName, d.ImeiNumber FROM [dbo].[tblFCEEMapping] fm"
                                       + " LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = fm.PointGuid"
                                       + " LEFT JOIN [dbo].[tblSites] s ON s.SiteGuid = p.SiteGuid"
                                       + " LEFT JOIN [dbo].[tblFCEDevice] d ON d.FCEDeviceGuid = fm.FCEDeviceGuid"
                                       + " WHERE s.SiteGuid = @siteGuid";
            cmd.Parameters.AddWithValue("@siteGuid", siteGuid);
        }

    }
}
