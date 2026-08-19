namespace MirgrationToolProcessing
{
    using BusinessObjects.MigrationObjects;
    using MigrationToolBusinessObjects;
    using MigrationToolDataAccessLayer;
    using MirgrationToolProcessing.Mapping;

    public class StationProcessing753ToV12 : StationProcessingBase
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public StationProcessing753ToV12(DbConfigurationDO dbConfigurationDo)
            : base(dbConfigurationDo)
        {
            base.migrationDA = new MigrationDatabaseDAClass { ConnectionString = base.DbConfigDo.SourceConnectionString };
            base.migrationTargetDA = new MigrationDatabaseDAClass { ConnectionString = base.DbConfigDo.TargetConnectionString };
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method starts the migration of the stations data.
        /// </summary>
        public override void MigrationProcess()
        {
            this.MigrateStations();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method migrates the stations.
        /// </summary>
        private void MigrateStations()
        {
            var sourceStationDo = new Stations753ToV12Do(base.SourceDbName, base.TargetDbName);
            var stationsMapping = new StationMapping753ToV12
            {
                SecurityHndlr = base.SecurityHndlr,
                SourceSiteId = base.SourceSiteId,
                TargetSiteId = base.TargetSiteId
            };

            stationsMapping.PerformMapping(sourceStationDo, base.migrationDA, base.migrationTargetDA);

            if (stationsMapping.MessageFlag)
            {
                base.MessageFlag = true;
                base.Message = base.Message + stationsMapping.Message;
            }
        }
        #endregion
    }
}
