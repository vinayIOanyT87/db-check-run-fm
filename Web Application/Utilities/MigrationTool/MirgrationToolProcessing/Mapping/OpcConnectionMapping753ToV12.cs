namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class OpcConnectionMapping753ToV12 : OpcConnectiongMappingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public OpcConnectionMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for OPC connections.
        /// </summary>
        /// <param name="opcConnectionDo">The OPC connection data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(OpcConnectionBaseDo opcConnectionDo, MigrationDatabaseDAClass migrationDA)
        {
            base.MessageFlag = false;
            base.Message = string.Empty;

            var opcConnectionDoList = new List<OpcConnection753ToV12Do>();
            OpcConnection753ToV12Do opcConnection = opcConnectionDo as OpcConnection753ToV12Do;
            DataSet sourceDataSet = null;

            if (string.IsNullOrEmpty(base.SourceSiteId) || string.IsNullOrEmpty(base.TargetSiteId))
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site ID or Target Site ID is null.";
                return;
            }

            using (var command = new SqlCommand())
            {
                opcConnection.EnumerateOpcConnectionSql(command);
                sourceDataSet = migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No OPC Connections found in the 7.5.3 " + opcConnection.SourceDbName + " database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newOpcConnection = new OpcConnection753ToV12Do();
                newOpcConnection.Load(row);
                opcConnectionDoList.Add(newOpcConnection);
            }

            if (opcConnectionDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No OPC Connections found in the 7.5.3 " + opcConnection.SourceDbName + " database.";
                return;
            }

            this.MapOpcConnections(opcConnectionDoList);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source OPC Connection to the target.
        /// </summary>
        /// <param name="opcConnectionList">The list of source OPC Connections.</param>
        private void MapOpcConnections(List<OpcConnection753ToV12Do> opcConnectionList)
        {
            // For OPC Connection, the target site is set to Site Admin.
            Guid targetSiteGuid = this.SecurityHndlr.SiteAdminGuid;

            // Set the target site for the migration.
            base.SecurityHndlr.Security.SiteGuid = targetSiteGuid;

            int insertCount = 0;

            foreach (OpcConnection753ToV12Do sourceOpcConnectionDo in opcConnectionList)
            {
                var targetOpcConnectionDo = new OPCConnectionClass
                {
                    ProgID          = sourceOpcConnectionDo.ProgId,
                    URL             = sourceOpcConnectionDo.Url,
                    IdentityGuid    = Guid.Empty,
                    CreatedBy       = "Migration Tool",
                    UpdatedBy       = "Migration Tool",
                    CreatedDate     = DateTimeOffset.Now,
                    UpdatedDate     = DateTimeOffset.Now
                };

                try
                {
                    Guid targetOpcConnectionGuid = FMChannelHelper.MakeCall<IOPCConnections, Guid>(x => x.Add(base.SecurityHndlr.Security, targetOpcConnectionDo));
                    targetOpcConnectionDo.IdentityGuid = targetOpcConnectionGuid;
                    insertCount++;
                }
                catch (Exception ex)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Adding OPC Connection for ProgID '" + sourceOpcConnectionDo.ProgId + "' to the target DB. " + ex.Message;
                }
            }

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " OPC Connection items.";
            }
        }
        #endregion
    }
}
