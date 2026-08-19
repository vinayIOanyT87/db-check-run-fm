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

    public class ProductMapMapping753ToV12 : ProductMapMappingBase
    {
        #region Data Member
        private MigrationDatabaseDAClass migrationDA;
        private List<ProductMap753ToV12Do> sourceProductMapDoList;
        private Dictionary<string, ProductClass> productIdGuidList;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ProductMapMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the retrieval of the product maps. It saves the the source product map
        /// collection to be used to retreive based on the load arm.
        /// </summary>
        /// <param name="productMapBaseDo">The product map data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void RetrieveAllMapping(ProductMapBaseDo productMapBaseDo, MigrationDatabaseDAClass migrationDA)
        {
            this.migrationDA = migrationDA;

            base.MessageFlag = false;
            base.Message = string.Empty;

            this.sourceProductMapDoList = new List<ProductMap753ToV12Do>();
            ProductMap753ToV12Do sourceProductMapDo = productMapBaseDo as ProductMap753ToV12Do;
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
                sourceProductMapDo.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = this.migrationDA.GetDataSet(command);
                sourceSiteIndex = sourceProductMapDo.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Get source Product Map
            using (var command = new SqlCommand())
            {
                sourceProductMapDo.EnumerateSourceProductMapSql(command);
                sourceDataSet = this.migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Product Maps found in the 7.5.3 " + sourceProductMapDo.SourceDbName + " database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newProductMapDo = new ProductMap753ToV12Do();
                newProductMapDo.Load(row);
                this.sourceProductMapDoList.Add(newProductMapDo);
            }

            if (sourceProductMapDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Product Maps found in the 7.5.3 " + sourceProductMapDo.SourceDbName + " database.";
                return;
            }
        }

        /// <summary>
        /// This method return a collection of product maps for a given load arm.
        /// </summary>
        /// <param name="sourceLoadArmDo">The load arm source data object to get the product map.</param>
        /// <param name="sourceProcessVariableDoList">The source process variable list.</param>
        /// <param name="targetOpcConnectionDoList">The target OPC connection list.</param>
        /// <returns>Returns the product map collection for an load arm.</returns>
        public override ProductMapCollectionClass GetProductMapCollection(LoadArm753ToV12Do sourceLoadArmDo
                                                                        , PRODUCT_MAP_TYPE productMapType
                                                                        , ProcessVariableMapping753ToV12 processVariableMap
                                                                        , List<UNIT_TYPE> unitTypeList
                                                                        , bool productMapProcessVariable)
        {
            var targetProductMapCollection = new ProductMapCollectionClass();

            // The product map type enumeration is different between v7.5.3 and v12.
            // v12 has an additional enumeration at 14.
            int sourceProductMapType = this.GetSourceProductMapType(productMapType);

            List<ProductMap753ToV12Do> foundProductMapDoList = 
                                            this.sourceProductMapDoList.FindAll(x => x.AssignedToIndex == sourceLoadArmDo.Index
                                            && x.Type == sourceProductMapType);

            if(foundProductMapDoList == null || foundProductMapDoList.Count == 0)
            {
                return targetProductMapCollection;
            }

            // For load arms, the target site is going to be the same as the source site.
            Guid targetSiteGuid = Guid.Empty;

            try
            {
                // For load arms, the target site is going to be the same as the source site.
                targetSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(base.SecurityHndlr.Security, base.SourceSiteId));
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Retrieving target site GUID for ID '" + base.SourceSiteId + "'. " + ex.Message;
                return targetProductMapCollection;
            }

            if (targetSiteGuid == Guid.Empty)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Target Site GUID is not found for Target Site ID: " + base.SourceSiteId;
                return targetProductMapCollection;
            }

            // Set the target site for the migration, which is the same as the source site ID.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.GetSiteGuidById(base.SourceSiteId);

            var targetAdditiveProfileList = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileCollectionClass>
                                                                        (x => x.Enumerate(this.SecurityHndlr.Security));

            var targetTankList = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security, false));
            var targetTankGroupList = FMChannelHelper.MakeCall<ITankGroups, TankGroupCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security));

            int insertCount = 0;

            foreach (ProductMap753ToV12Do foundProductMapDo in foundProductMapDoList)
            {
                string productMapTypeStr = ((PRODUCT_MAP_TYPE)foundProductMapDo.Type).ToString();

                // The product map type enumeration is different between v7.5.3 and v12.
                // v12 has an additional enumeration at 14.
                PRODUCT_MAP_TYPE targetProductMapType = this.GetTargetProductMapType(foundProductMapDo.Type);

                var targetProductMapDo = new ProductMapClass
                {
                    Type                        = targetProductMapType,
                    Sequence                    = foundProductMapDo.Sequence,
                    BlendPercentage             = foundProductMapDo.BlendPercentage,
                    AdditiveRate                = foundProductMapDo.AdditiveRate.ToString(),
                    Ratio                       = foundProductMapDo.Ratio,
                    AdditiveCycleVolume         = foundProductMapDo.AdditiveCycleVolume.ToString(),
                    Tolerance                   = foundProductMapDo.Tolerance,
                    PresetNumber                = foundProductMapDo.PresetNumber,
                    AdditiveProfileGuid         = this.GetAdditiveProfileGuid(foundProductMapDo, ref targetAdditiveProfileList),
                    TankOrGroupGuid             = this.GetTankOrTankGroupGuid(foundProductMapDo, ref targetTankList, ref targetTankGroupList),
                    MeterID                     = foundProductMapDo.MeterId,
                    ShipToProductID             = foundProductMapDo.ShipToProductId,
                    ShipToProductCode           = foundProductMapDo.ShipToProductCode,
                    ShipToLoadRackDisplayText   = foundProductMapDo.ShipToLoadRackDisplayText,
                    //SpecialInstructions         = foundProductMapDo.SpecialInstructionIndex,
                    UnavailableInventoryGross   = foundProductMapDo.UnavailableInventoryGross == null ? "0.0" : foundProductMapDo.UnavailableInventoryGross.Value.ToString(),
                    UnavailableInventoryNet     = foundProductMapDo.UnavailableInventoryNet == null ? "0.0" : foundProductMapDo.UnavailableInventoryNet.Value.ToString(),
                    DesiredTreatRate            = foundProductMapDo.DesiredTreatRate == null ? 0.0 : foundProductMapDo.DesiredTreatRate.Value,
                    EnableRecipe                = foundProductMapDo.EnableRecipe
                };

                ProductClass product = this.GetProduct(foundProductMapDo.AssignedProductId);

                // Must have product Guid or the add station will fail.
                if(product == null)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Error: Product '" + foundProductMapDo.AssignedProductId + "' not found for product map associated to Station '" 
                                        + sourceLoadArmDo.StationId + "' and Load Arm > " + sourceLoadArmDo.LoadArmMessageId
                                        +". Station migration will fail.";
                }
                else
                {
                    targetProductMapDo.AssignedGuid = product.MasterRecordGuid;
                    targetProductMapDo.AssignedProductType = product.ProductType;
                }

                if (string.IsNullOrEmpty(foundProductMapDo.MeterId) == false)
                {
                    var meterGuid = this.GetMeterGuid(foundProductMapDo.MeterId);

                    if(meterGuid == Guid.Empty)
                    {
                        MeterClass newMeter = this.CreateMeter(foundProductMapDo.MeterId, targetSiteGuid);

                        targetProductMapDo.Meter = newMeter;
                        targetProductMapDo.MeterID = newMeter.ID;
                    }
                }

                // Only get the process variables if flag is true.
                if (productMapProcessVariable)
                {
                    // Get the process variables for the target product map.
                    ProcessVariableCollectionClass targetProcessVariableCollection =
                                            processVariableMap.GetTargetProcessVariables(foundProductMapDo.Index, PROCESS_VARIABLE_TYPE.MAX_PV, unitTypeList);

                    if (targetProcessVariableCollection != null && targetProcessVariableCollection.Count > 0)
                    {
                        targetProductMapDo.ProcessVariableCollection = targetProcessVariableCollection;
                    }
                }

                targetProductMapCollection.Add(targetProductMapDo);
                insertCount++;
            }

            base.MessageFlag = true;
            base.Message = base.Message + Environment.NewLine
                                + "Info: Added " + insertCount + " product map items for Station ID: " + sourceLoadArmDo.StationId 
                                + " and Load Arm > " + sourceLoadArmDo.LoadArmMessageId;

            return targetProductMapCollection;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will return an additive profile GUID that matches the source additive profile ID.
        /// </summary>
        /// <param name="sourceProductMapDo">The product map data object.</param>
        /// <param name="targetAdditiveProfileList">The target additive profile list.</param>
        /// <returns>Returns the additive profile Guid or an empty Guid if not found.</returns>
        private Guid GetAdditiveProfileGuid(ProductMap753ToV12Do sourceProductMapDo, ref AdditiveProfileCollectionClass targetAdditiveProfileList)
        {
            if (targetAdditiveProfileList == null || targetAdditiveProfileList.Count == 0)
            {
                return Guid.Empty;
            }

            if (string.IsNullOrEmpty(sourceProductMapDo.AdditiveProfileId))
            {
                return Guid.Empty;
            }

            var targetAdditiveProfileEnumerator = targetAdditiveProfileList.GetEnumerator();

            while(targetAdditiveProfileEnumerator.MoveNext())
            {
                var targetAdditiveProfile = targetAdditiveProfileEnumerator.Current as AdditiveProfileClass;
                if(targetAdditiveProfile.ID == sourceProductMapDo.AdditiveProfileId)
                {
                    return targetAdditiveProfile.IdentityGuid;
                }
            }

            return Guid.Empty;

        }

        /// <summary>
        /// This method will get the tank or tank group Guid based on the source tank ID.
        /// </summary>
        /// <param name="sourceProductMapDo">The source product map object.</param>
        /// <param name="targetTankList">The target tank list.</param>
        /// <param name="targetTankGroupList">The target tank group list.</param>
        /// <returns>Returns the tank or tank group Guid. Returns emptyp Guid if not found.</returns>
        private Guid GetTankOrTankGroupGuid(ProductMap753ToV12Do sourceProductMapDo, ref TankCollectionClass targetTankList, ref TankGroupCollectionClass targetTankGroupList)
        {
            if((targetTankList == null || targetTankList.Count == 0)
                && (targetTankGroupList == null || targetTankGroupList.Count == 0))
            {
                return Guid.Empty;
            }

            if (targetTankList != null && targetTankList.Count > 0)
            {
                TankClass targetTank = targetTankList.Find(x => x.ID == sourceProductMapDo.TankId);

                if(targetTank != null)
                {
                    return targetTank.IdentityGuid;
                }
            }

            if (targetTankGroupList != null && targetTankGroupList.Count > 0)
            {
                TankGroupClass targetGroupTank = targetTankGroupList.Find(x => x.ID == sourceProductMapDo.TankId);

                if (targetGroupTank != null)
                {
                    return targetGroupTank.IdentityGuid;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// This is a helper method to get the product guid.
        /// </summary>
        /// <param name="productId">The product ID to search on.</param>
        /// <returns>Return the product guid.</returns>
        private ProductClass GetProduct(string productId)
        {
            if(this.productIdGuidList == null)
            {
                this.productIdGuidList = new Dictionary<string, ProductClass>();
            }

            if(string.IsNullOrEmpty(productId))
            {
                return null;
            }

            if(this.productIdGuidList.ContainsKey(productId) == false)
            {
                var product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(base.SecurityHndlr.Security, productId));

                if(product == null)
                {
                    return null;
                }

                this.productIdGuidList.Add(productId, product);
                return product;
            }

            return this.productIdGuidList[productId];
        }

        /// <summary>
        /// This method will check for an existing meter based on the meterID.
        /// </summary>
        /// <param name="meterId">The meter ID used to find an existing meter.</param>
        /// <returns>Returns the meter GUID if found, otherwise it returns an empty Guid.</returns>
        private Guid GetMeterGuid(string meterId)
        {
            if(string.IsNullOrEmpty(meterId))
            {
                return Guid.Empty;
            }

            var meterGuid = FMChannelHelper.MakeCall<IMeters, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, meterId));

            if(meterGuid == null || meterGuid == Guid.Empty)
            {
                return Guid.Empty;
            }

            return meterGuid;
        }

        /// <summary>
        /// This is a helper class to create a meter object.
        /// </summary>
        /// <param name="meterId">The meter ID used to create the object.</param>
        /// <param name="targetSiteGuid">The target site Guid</param>
        /// <returns>Return a Meter Class.</returns>
        private MeterClass CreateMeter(string meterId, Guid targetSiteGuid)
        {
            var meter = new MeterClass
            {
                ID                      = meterId,
                IdentityGuid            = Guid.Empty,
                SiteGuid                = targetSiteGuid,
                NumberOfDigits          = 8,
                RotatesBackwardsFlag    = false,
                ReceiptMeterFlag        = false,
                CreatedBy               = "Migration Tool",
                CreatedDate             = DateTime.Now,
                UpdatedBy               = "Migration Tool",
                UpdatedDate             = DateTime.Now
            };

            return meter;
        }

        /// <summary>
        /// This method will get the target product map type since the enumeration are different
        /// starting at source product map type 14. V12 has an additional product map type after enumeration
        /// 13.  Therefore, add one to the source product map type.
        /// </summary>
        /// <param name="sourceProductMapType">The source product map type.</param>
        /// <returns>Returns the correct product map type.</returns>
        private PRODUCT_MAP_TYPE GetTargetProductMapType(int sourceProductMapType)
        {
            // Product map type enumeration is the same between 7.5.3 and v12 up
            // until enumeration 14.
            if (sourceProductMapType <= 13)
            {
                return (PRODUCT_MAP_TYPE)sourceProductMapType;
            }

            int newProductMapTypeIndex = sourceProductMapType + 1;

            // Max product map is 18 in 7.5.3
            if(newProductMapTypeIndex >= 18)
            {
                return PRODUCT_MAP_TYPE.MAX_MAP;
            }

            return (PRODUCT_MAP_TYPE)newProductMapTypeIndex;
        }

        /// <summary>
        /// This method will get the source product map type since the enumeration are different
        /// starting at source product map type 14. V12 has an additional product map type after enumeration
        /// 13.  Therefore, add one to the source product map type.
        /// </summary>
        /// <param name="targetProductMapType">The target product map type.</param>
        /// <returns>Returns the correct product map type.</returns>
        private int GetSourceProductMapType(PRODUCT_MAP_TYPE targetProductMapType)
        {
            // Product map type enumeration is the same between 7.5.3 and v12 up
            // until enumeration 14.
            if ((int)targetProductMapType <= 13)
            {
                return (int)targetProductMapType;
            }

            int newProductMapTypeIndex = (int)targetProductMapType - 1;

            // Max product map is 18 in 7.5.3
            if (newProductMapTypeIndex > 18)
            {
                return 18;
            }

            return newProductMapTypeIndex;
        }
        #endregion
    }
}
