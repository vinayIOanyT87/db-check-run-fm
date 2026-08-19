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
    using System.Runtime.InteropServices;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class TankMapping753ToV12 : TankMappingBase
    {
        #region Data Member
        private MigrationDatabaseDAClass migrationDA;
        private MigrationDatabaseDAClass migrationTargetDA;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public TankMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for tanks.
        /// </summary>
        /// <param name="tankBaseDo">The tank data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        /// <param name="migrationTargetDA">The Migration data access object for target DB.</param>
        public override void PerformMapping(TankBaseDo tankBaseDo, MigrationDatabaseDAClass migrationDA, MigrationDatabaseDAClass migrationTargetDA)
        {
            this.migrationDA = migrationDA;
            this.migrationTargetDA = migrationTargetDA;

            base.MessageFlag = false;
            base.Message = string.Empty;

            var sourceTankDoList = new List<Tank753ToV12Do>();
            Tank753ToV12Do sourceTankDo = tankBaseDo as Tank753ToV12Do;
            DataSet sourceDataSet = null;

            if (string.IsNullOrEmpty(base.SourceSiteId) || string.IsNullOrEmpty(base.TargetSiteId))
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site ID or Target Site ID is null.";
                return;
            }

            int? sourceSiteIndex = null;

            using (var command = new SqlCommand())
            {
                sourceTankDo.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = this.migrationDA.GetDataSet(command);
                sourceSiteIndex = sourceTankDo.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Get source Tanks
            using (var command = new SqlCommand())
            {
                sourceTankDo.EnumerateTanksSql(command, sourceSiteIndex.Value);
                sourceDataSet = this.migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Tanks found in the 7.5.3 " + sourceTankDo.SourceDbName + " database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newTankDo = new Tank753ToV12Do();
                newTankDo.Load(row);
                sourceTankDoList.Add(newTankDo);
            }

            if (sourceTankDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Tanks found in the 7.5.3 " + sourceTankDo.SourceDbName + " database.";
                return;
            }
            this.MapTanks(sourceTankDoList);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source tanks to the target.
        /// </summary>
        /// <param name="sourceTankDoList">The list of source tanks.</param>
        private void MapTanks(List<Tank753ToV12Do> sourceTankDoList)
        {
            // For tanks, the target site is going to be the same as the source site.
            Guid targetSiteGuid = Guid.Empty;

            try
            {
                // For tanks, the target site is going to be the same as the source site.
                targetSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(base.SecurityHndlr.Security, base.SourceSiteId));
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Retrieving target site GUID for ID '" + base.SourceSiteId + "'. " + ex.Message;
                return;
            }

            if (targetSiteGuid == Guid.Empty)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Target Site GUID is not found for Target Site ID: " + base.SourceSiteId;
                return;
            }

            // Set the target site for the migration, which is the same as the source site ID.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.GetSiteGuidById(base.SourceSiteId);

            // Get the list of target tanks to be used to check if already exists.
            var targetTankList = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(x => x.Enumerate(base.SecurityHndlr.Security, false));
            int insertCount = 0;

            foreach (Tank753ToV12Do sourceTankDo in sourceTankDoList)
            {
                bool tankExist = this.TankExists(sourceTankDo.TankId, ref targetTankList);

                if (tankExist == false)
                {
                    Guid targetManagerGuid = base.FindCompany(sourceTankDo.ManagerId);
                    Guid targetProductGuid = base.FindProduct(sourceTankDo.ProductId);

                    var targetTankDo = new TankClass
                    {
                        ID          = sourceTankDo.TankId,
                        SiteGuid    = targetSiteGuid,
                        SiteID      = base.SourceSiteId,
                        ProductGuid = Guid.Empty,
                        VesselType  = (VESSEL_TYPE) sourceTankDo.VesselTypeIndex,
                        ManagerGuid = Guid.Empty
                    };

                    if (targetManagerGuid == Guid.Empty)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Info: Could find Manager ID '" + sourceTankDo.ManagerId + "' in target database.";
                    }
                    else
                    {
                        targetTankDo.ManagerGuid = targetManagerGuid;
                    }

                    if (targetProductGuid == Guid.Empty)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Info: Could find Product ID '" + sourceTankDo.ProductId + "' in target database.";
                    }
                    else
                    {
                        targetTankDo.ProductGuid = targetProductGuid;
                    }

                    try
                    {
                        ProcessVariableCollectionClass processVariableList = this.GetProcessVariable(sourceTankDo);

                        if(processVariableList.Count == 0)
                        {
                            base.MessageFlag = true;
                            base.Message = base.Message + Environment.NewLine
                                                + "Info: Could find process variables for Tank ID '" + sourceTankDo.TankId + "' to the target DB.";
                        }
                        else
                        {
                            targetTankDo.ProcessVariableCollection = processVariableList;
                        }
                    }
                    catch(Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Error: Retrieving process variables for Tank ID '" + sourceTankDo.TankId + "' to the target DB. " + ex.Message;
                    }

                    // Migration the tank
                    try
                    {
                        Guid targetTankGuid = FMChannelHelper.MakeCall<ITanks, Guid>(x => x.Add(base.SecurityHndlr.Security, targetTankDo));
                        targetTankDo.IdentityGuid = targetTankGuid;
                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Error: Adding Tank ID '" + sourceTankDo.TankId + "' to the target DB. " + ex.Message;
                    }
                }
                else
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Tank ID '" + sourceTankDo.TankId + "' already exists at Target site '"
                                        + base.TargetSiteId + "'.";
                }
            }

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " Tank items.";
            }

            // Reset the site guid to SiteAdmin.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;
        }

        /// <summary>
        /// This method is a helper to find if an existing tank already exists at the
        /// target database.
        /// </summary>
        /// <param name="sourceTankId">The source tank ID</param>
        /// <param name="targetTankList">The target tank list.</param>
        /// <returns>Return false if the tank does not exist at the target DB. Otherwise, returns true.</returns>
        private bool TankExists(string sourceTankId, ref TankCollectionClass targetTankList)
        {
            if (targetTankList == null || targetTankList.Count <= 0)
            {
                return false;
            }

            TankClass targetTank = targetTankList.Find(x => x.ID == sourceTankId);

            if (targetTank == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method will retrieve the process variables for a tank.
        /// </summary>
        /// <param name="tankDo">The tank data object.</param>
        /// <returns>Returns a collection of process variables.</returns>
        private ProcessVariableCollectionClass GetProcessVariable(Tank753ToV12Do tankDo)
        {
            var processVariableCollection = new ProcessVariableCollectionClass();

            using(var command = new SqlCommand())
            {
                var processVariablesDo = new ProcessVariables753ToV12Do(tankDo.SourceDbName, tankDo.TargetDbName);
                processVariablesDo.EnumerateProcessVariableSql(command, tankDo.TankIndex);
                DataSet sourceDataSet = this.migrationDA.GetDataSet(command);

                if(sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
                {
                    return processVariableCollection;
                }

                foreach(DataRow row in sourceDataSet.Tables[0].Rows)
                {
                    var newSourceProcessVariable = new ProcessVariables753ToV12Do();
                    newSourceProcessVariable.Load(row);

                    var targetProcessVariable = new ProcessVariableClass
                    {
                        ProcessVariableType = (PROCESS_VARIABLE_TYPE)newSourceProcessVariable.ProcessVariableType,
                        InstanceNumber      = newSourceProcessVariable.InstanceNumber,
                        DataType            = newSourceProcessVariable.DataType == null ? VarEnum.VT_UNKNOWN : (VarEnum)newSourceProcessVariable.DataType,
                        ServerUnits         = newSourceProcessVariable.ServerEngineeringUnitsIndex == null ? EngineeringUnit.FmvUsGal : (EngineeringUnit)newSourceProcessVariable.ServerEngineeringUnitsIndex,
                        OPCQuality          = newSourceProcessVariable.Quality == null ? (short)0 : (short)newSourceProcessVariable.Quality,
                        SIValue             = newSourceProcessVariable.SIValue,
                        DateTimeStamp       = newSourceProcessVariable.DateTimeStamp == null ? DateTime.Now : (DateTime)newSourceProcessVariable.DateTimeStamp,
                        siMaximum           = newSourceProcessVariable.Maximum,
                        siMinimum           = newSourceProcessVariable.Minimum,
                        DataTypeEnabled     = newSourceProcessVariable.DataTypeEnabled,
                        Input               = newSourceProcessVariable.Input,
                        InputEnabled        = newSourceProcessVariable.InputEnabled
                    };

                    //var messageCollection = FMChannelHelper.MakeCall<IMessages, MessageCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security));

                    //if(messageCollection != null)
                    //{
                    //    MessageClass messageDo = messageCollection.Find(x => x.ID == newSourceProcessVariable.MessageID);

                    //    if(messageDo != null)
                    //    {
                    //        targetProcessVariable.MessageID = newSourceProcessVariable.MessageID;
                    //        targetProcessVariable.IdentityGuid = messageDo.IdentityGuid;
                    //    }
                    //}

                    processVariableCollection.Add(targetProcessVariable);
                }
            }

            return processVariableCollection;
        }
        #endregion
    }
}
