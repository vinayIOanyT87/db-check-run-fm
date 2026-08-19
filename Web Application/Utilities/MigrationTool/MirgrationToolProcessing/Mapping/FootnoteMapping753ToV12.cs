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

    public class FootnoteMapping753ToV12 : FootnoteMappingBase
    {
        #region data members
        private MigrationDatabaseDAClass migrationDA;
        private Guid targetSiteGuid;
        private Guid sourceSiteGuid;
        private Dictionary<string, Guid> productList;
        private Dictionary<string, Guid> additiveProfileList;
        private Dictionary<string, Guid> companyShipperList;
        private Dictionary<string, Guid> companyShipToList;
        private Dictionary<string, Guid> applicationStringList;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public FootnoteMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for footnote application string.
        /// </summary>
        /// <param name="inFootnoteDo">The footnote data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(FootnoteBaseDo inFootnoteDo, MigrationDatabaseDAClass migrationDA)
        {
            this.migrationDA = migrationDA;

            base.MessageFlag = false;
            base.Message = string.Empty;

            var applicationStrFootnoteDoList = new List<ApplicationString753ToV12Do>();
            var footnoteDoList = new List<Footnote753ToV12Do>();
            var footnoteDo = inFootnoteDo as Footnote753ToV12Do;
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
                footnoteDo.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = this.migrationDA.GetDataSet(command);
                sourceSiteIndex = footnoteDo.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            // Get Source site info
            this.sourceSiteGuid = Guid.Empty;

            try
            {
                this.sourceSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(base.SecurityHndlr.Security, base.SourceSiteId));
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Retrieving source site GUID for ID '" + base.SourceSiteId + "'. " + ex.Message;
                return;
            }

            if (this.sourceSiteGuid == Guid.Empty)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site GUID is not found.";
                return;
            }

            // Get Target site info
            this.targetSiteGuid = Guid.Empty;

            try
            {
                this.targetSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(base.SecurityHndlr.Security, base.TargetSiteId));
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Retrieving target site GUID for ID '" + base.TargetSiteId + "'. " + ex.Message;
                return;
            }

            if (this.targetSiteGuid == Guid.Empty)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Target Site GUID is not found.";
                return;
            }

            // Get the source Footnotes
            using (var command = new SqlCommand())
            {
                footnoteDo.EnumerateFootnotesSql(command);
                sourceDataSet = this.migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Footnote Application String map found in the 7.5.3 " + footnoteDo.SourceDbName + " database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newfootnodeDo = new Footnote753ToV12Do();
                newfootnodeDo.Load(row);
                footnoteDoList.Add(newfootnodeDo);
            }

            if (footnoteDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Footnote Application String map found in the 7.5.3 " + footnoteDo.SourceDbName + " database.";
                return;
            }

            // Get all the source footnote application strings
            using (var command = new SqlCommand())
            {
                var applicationStrDo = new ApplicationString753ToV12Do(footnoteDo.SourceDbName, footnoteDo.TargetDbName);
                applicationStrDo.EnumerateSourceApplicationStringFootnoteSql(command, sourceSiteIndex.Value, STRING_TYPE.FOOT_NOTE);
                sourceDataSet = this.migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Footnote Application String found in the 7.5.3 " + footnoteDo.SourceDbName + " database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newApplicationStrDo = new ApplicationString753ToV12Do();
                newApplicationStrDo.Load(row);
                applicationStrFootnoteDoList.Add(newApplicationStrDo);
            }

            this.productList            = new Dictionary<string, Guid>();
            this.additiveProfileList    = new Dictionary<string, Guid>();
            this.companyShipperList     = new Dictionary<string, Guid>();
            this.companyShipToList      = new Dictionary<string, Guid>();
            this.applicationStringList  = new Dictionary<string, Guid>();

            this.MapFootnotes(footnoteDoList, applicationStrFootnoteDoList);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source footnote application string to the target.
        /// </summary>
        /// <param name="sourceFootnoteDoList">The list of source footnote application strings.</param>
        private void MapFootnotes(List<Footnote753ToV12Do> sourceFootnoteDoList, List<ApplicationString753ToV12Do> sourceApplicationStrDoList)
        {
            // Set the target site for the migration.
            base.SecurityHndlr.Security.SiteGuid = this.targetSiteGuid;

            var entityAssignmentProcessor = new EntityService.EntityAssignmentProcessor(this.SecurityHndlr);

            // Get the list of target equipment types to be used to check if already exists.
            var targetFootnoteList = FMChannelHelper.MakeCall<IFootNotes, FootNoteCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security));

            int insertCount = 0;

            foreach(ApplicationStringBaseDo sourceApplicationStrDo in sourceApplicationStrDoList)
            {
                var messageList = new List<string>();

                List<Footnote753ToV12Do> foundFootNotes = sourceFootnoteDoList.FindAll(
                                    x => x.ApplicationStringId == sourceApplicationStrDo.ID && x.ApplicationStringType == sourceApplicationStrDo.Type);

                if(foundFootNotes == null || foundFootNotes.Count == 0)
                {
                    continue;
                }

                FootNoteClass existingFootnote = this.FootnoteExists(sourceApplicationStrDo.ID, ref targetFootnoteList);

                if(existingFootnote != null)
                {
                    // During entity assignment the source site will be i.e. Citgo and
                    // the target site will be i.e. Chattanooga. It is reversed from the migration.
                    Guid sourceEntitySiteGuid = this.targetSiteGuid;
                    Guid targetEntitySiteGuid = this.sourceSiteGuid;

                    // Entity assign the footnote to the target site.
                    string entityMessage = " For Footnote ID: '" + sourceApplicationStrDo.ID + "' to the target Site: " + this.SourceSiteId + ".";
                    base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                        , existingFootnote.IdentityGuid
                                                        , sourceEntitySiteGuid
                                                        , targetEntitySiteGuid
                                                        , typeof(IFootNotes).GUID
                                                        , ENTITY_TYPE.FOOTNOTE
                                                        , entityMessage);

                    continue;
                }

                var targetFootnoteDo = new FootNoteClass
                {
                    ID          = sourceApplicationStrDo.ID,
                    EntityType  = ENTITY_TYPE.FOOTNOTE,
                    SiteGuid    = this.targetSiteGuid,
                    SiteID      = base.TargetSiteId
                };

                if(sourceApplicationStrDo.StartDate != null)
                {
                    targetFootnoteDo.StartDate = sourceApplicationStrDo.StartDate.Value;
                }

                if (sourceApplicationStrDo.EndDate != null)
                {
                    targetFootnoteDo.EndDate = sourceApplicationStrDo.EndDate.Value;
                }

                foreach (Footnote753ToV12Do foundFootNote in foundFootNotes)
                {
                    var targetFootnoteApplicationStringMapDo = new ApplicationStringMapClass
                    {
                        Sequence                = foundFootNote.Sequence,
                        Type                    = (STRING_MAP_TYPE)foundFootNote.Type,
                        ApplicationStringGuid   = Guid.Empty,
                        CreatedBy               = "Migration Tool",
                        UpdatedBy               = "Migration Tool",
                        CreatedDate             = DateTimeOffset.Now,
                        UpdatedDate             = DateTimeOffset.Now
                    };

                    switch (targetFootnoteApplicationStringMapDo.Type)
                    {
                        case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT:
                            Guid productGuid = this.GetProductGuid(foundFootNote.ProductId);

                            if(productGuid != Guid.Empty)
                            {
                                targetFootnoteApplicationStringMapDo.AssignedToGuid = productGuid;
                                targetFootnoteApplicationStringMapDo.AssignedToID = foundFootNote.ProductId;
                                targetFootnoteDo.FootNoteProductMapCollection.Add(targetFootnoteApplicationStringMapDo);
                                messageList.Add(this.AddFootnoteMessage(foundFootNote.ProductId, STRING_MAP_TYPE.FOOT_NOTE_PRODUCT));
                            }
                            break;
                        case STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE:
                            Guid additiveGuid = this.GetAdditiveProfileGuid(foundFootNote.AdditiveProfileId);

                            if (additiveGuid != Guid.Empty)
                            {
                                targetFootnoteApplicationStringMapDo.AssignedToGuid = additiveGuid;
                                targetFootnoteApplicationStringMapDo.AssignedToID = foundFootNote.AdditiveProfileId;
                                targetFootnoteDo.FootNoteAdditiveProfileMapCollection.Add(targetFootnoteApplicationStringMapDo);
                                messageList.Add(this.AddFootnoteMessage(foundFootNote.AdditiveProfileId, STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE));
                            }
                            break;
                        case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER:
                            Guid shipperGuid = this.GetCompanyGuid(foundFootNote.CompanyShipperId, COMPANY_ROLE.SHIPPER);

                            if (shipperGuid != Guid.Empty)
                            {
                                targetFootnoteApplicationStringMapDo.AssignedToGuid = shipperGuid;
                                targetFootnoteApplicationStringMapDo.AssignedToID = foundFootNote.CompanyShipperId;
                                targetFootnoteDo.FootNoteShipperMapCollection.Add(targetFootnoteApplicationStringMapDo);
                                messageList.Add(this.AddFootnoteMessage(foundFootNote.CompanyShipperId, STRING_MAP_TYPE.FOOT_NOTE_SHIPPER));
                            }
                            break;
                        case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO:
                            Guid shipToGuid = this.GetCompanyGuid(foundFootNote.CompanyShipToId, COMPANY_ROLE.CUSTOMER_SHIPTO);

                            if (shipToGuid != Guid.Empty)
                            {
                                targetFootnoteApplicationStringMapDo.AssignedToGuid = shipToGuid;
                                targetFootnoteApplicationStringMapDo.AssignedToID = foundFootNote.CompanyShipToId;
                                targetFootnoteDo.FootNoteShipToMapCollection.Add(targetFootnoteApplicationStringMapDo);
                                messageList.Add(this.AddFootnoteMessage(foundFootNote.CompanyShipToId, STRING_MAP_TYPE.FOOT_NOTE_SHIPTO));
                            }
                            break;
                        case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE:
                            targetFootnoteApplicationStringMapDo.AssignedToGuid = Guid.Empty;
                            targetFootnoteApplicationStringMapDo.AssignedToID = foundFootNote.CompanyShipToState;
                            targetFootnoteDo.FootNoteShipToStateMapCollection.Add(targetFootnoteApplicationStringMapDo);
                            messageList.Add(this.AddFootnoteMessage(foundFootNote.CompanyShipToState, STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE));
                            break;
                    }
                }

                try
                {
                    var targetFootnoteGuid = FMChannelHelper.MakeCall<IFootNotes, Guid>(x => x.Add(base.SecurityHndlr.Security, targetFootnoteDo));

                    foreach(string footnoteMessage in messageList)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + footnoteMessage;
                        insertCount++;
                    }

                    // During entity assignment the source site will be i.e. Citgo and
                    // the target site will be i.e. Chattanooga. It is reversed from the migration.
                    Guid sourceEntitySiteGuid = this.targetSiteGuid;
                    Guid targetEntitySiteGuid = this.sourceSiteGuid;

                    // Entity assign the footnote to the target site.
                    string entityMessage = " For Footnote ID: '" + sourceApplicationStrDo.ID + "' to the target Site: " + this.SourceSiteId + ".";
                    base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                        , targetFootnoteGuid
                                                        , sourceEntitySiteGuid
                                                        , targetEntitySiteGuid
                                                        , typeof(IFootNotes).GUID
                                                        , ENTITY_TYPE.FOOTNOTE
                                                        , entityMessage);
                }
                catch (Exception ex)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Error: Adding Foothnotes with ID: '" + sourceApplicationStrDo.ID + "' to the target DB. " + ex.Message;

                    base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;
                }
            }

            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " Footnote items.";
            }
        }

        /// <summary>
        /// This method is a helper to find if an existing footnote already exists at the
        /// target database.
        /// </summary>
        /// <param name="soureFootnoteId">The footnote ID to search on.</param>
        /// <param name="targetFootNoteList">The list of footnotes at the target site.</param>
        /// <returns>Returns null if the footnote does not exist, otherwise it returns the footnote class.</returns>
        private FootNoteClass FootnoteExists(string soureFootnoteId, ref FootNoteCollectionClass targetFootNoteList)
        {
            if (targetFootNoteList == null || targetFootNoteList.Count <= 0)
            {
                return null;
            }

            FootNoteClass targetFootnote = targetFootNoteList.Find(x => x.ID.ToUpper() == soureFootnoteId.ToUpper());

            if (targetFootnote == null)
            {
                return null;
            }

            return targetFootnote;
        }

        /// <summary>
        /// This method retrieves the target product Guid that matches the product ID.
        /// </summary>
        /// <param name="productId">The source product ID used to retrieve the product GUID.</param>
        /// <returns>Return the product Guid or empty Guid if not found.</returns>
        private Guid GetProductGuid(string productId)
        {
            Guid masterGuid = Guid.Empty;

            if(string.IsNullOrEmpty(productId))
            {
                return Guid.Empty;
            }

            if(this.productList.ContainsKey(productId))
            {
                return this.productList[productId];
            }

            try
            {
                var productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, productId));

                if(productGuid == null || productGuid == Guid.Empty)
                {
                    return Guid.Empty;
                }

                masterGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.SecurityHndlr.Security, productGuid));

                if (masterGuid == null || masterGuid == Guid.Empty)
                {
                    return Guid.Empty;
                }

                this.productList.Add(productId, masterGuid);
            }
            catch(Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Footnotes could retrieve product with ID: " + productId + ". " + ex.Message;
            }

            return masterGuid;
        }

        /// <summary>
        /// This method retrieves the target additive profle Guid that matches the additive profile ID.
        /// </summary>
        /// <param name="additiveProfileId">The source additive profile ID used to retrieve the additive profile GUID.</param>
        /// <returns>Return the additive profile Guid or empty Guid if not found.</returns>
        private Guid GetAdditiveProfileGuid(string additiveProfileId)
        {
            Guid additiveProfileGuid = Guid.Empty;

            if (string.IsNullOrEmpty(additiveProfileId))
            {
                return Guid.Empty;
            }

            if (this.additiveProfileList.ContainsKey(additiveProfileId))
            {
                return this.additiveProfileList[additiveProfileId];
            }

            try
            {
                additiveProfileGuid = FMChannelHelper.MakeCall<IAdditiveProfiles, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, additiveProfileId));

                if (additiveProfileGuid == null || additiveProfileGuid == Guid.Empty)
                {
                    return Guid.Empty;
                }

                this.additiveProfileList.Add(additiveProfileId, additiveProfileGuid);
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Footnotes could retrieve additive profile with ID: " + additiveProfileId + ". " + ex.Message;
            }

            return additiveProfileGuid;
        }

        /// <summary>
        /// This method retrieves the target company Guid that matches the company ID and role.
        /// </summary>
        /// <param name="companyId">The source company ID used to retrieve the company GUID.</param>
        /// <param name="companyRole">The source company role used to retrieve the company GUID.</param>
        /// <returns>Return the company Guid or empty Guid if not found.</returns>
        private Guid GetCompanyGuid(string companyId, COMPANY_ROLE companyRole)
        {
            Guid masterGuid = Guid.Empty;

            if (string.IsNullOrEmpty(companyId))
            {
                return Guid.Empty;
            }

            if(companyRole == COMPANY_ROLE.SHIPPER)
            {
                if (this.companyShipperList.ContainsKey(companyId))
                {
                    return this.companyShipperList[companyId];
                }
            }

            if (companyRole == COMPANY_ROLE.CUSTOMER_SHIPTO)
            {
                if (this.companyShipToList.ContainsKey(companyId))
                {
                    return this.companyShipToList[companyId];
                }
            }

            try
            {
                var companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, companyId));
                var company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.SecurityHndlr.Security, companyGuid, true, false));

                if (company == null || company.MasterRecordGuid == Guid.Empty)
                {
                    return Guid.Empty;
                }

                if(company.HasRole(companyRole) && companyRole == COMPANY_ROLE.SHIPPER)
                {
                    this.companyShipperList.Add(companyId, company.MasterRecordGuid);
                    masterGuid = company.MasterRecordGuid;
                }

                if (company.HasRole(companyRole) && companyRole == COMPANY_ROLE.CUSTOMER_SHIPTO)
                {
                    this.companyShipToList.Add(companyId, company.MasterRecordGuid);
                    masterGuid = company.MasterRecordGuid;
                }
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Footnotes could retrieve company Guid with ID: " + companyId + ". " + ex.Message;
            }

            return masterGuid;
        }

        /// <summary>
        /// This method retrieves the target application string Guid that matches the ID.
        /// </summary>
        /// <param name="applicationStringId">The source application string ID used to retrieve the GUID.</param>
        /// <returns>Return the application Guid or empty Guid if not found.</returns>
        private Guid GetApplicationStringGuid(string applicationStringId)
        {
            Guid applicationStringGuid = Guid.Empty;

            if (string.IsNullOrEmpty(applicationStringId))
            {
                return Guid.Empty;
            }

            if (this.applicationStringList.ContainsKey(applicationStringId))
            {
                return this.applicationStringList[applicationStringId];
            }

            try
            {
                applicationStringGuid = FMChannelHelper.MakeCall<IApplicationStrings, Guid>
                                                        (x => x.GetIdentityGuid(this.SecurityHndlr.Security, STRING_TYPE.FOOT_NOTE, applicationStringId));

                if (applicationStringGuid == null || applicationStringGuid == Guid.Empty)
                {
                    return Guid.Empty;
                }

                this.applicationStringList.Add(applicationStringId, applicationStringGuid);
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Footnotes could retrieve application string with ID: " + applicationStringId + ". " + ex.Message;
            }

            return applicationStringGuid;
        }

        /// <summary>
        /// This method will add a message for which footnote is being added.
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <param name="footnoteMapType">The footnote type.</param>
        private string AddFootnoteMessage(string id, STRING_MAP_TYPE footnoteMapType)
        {
            string message = string.Empty;

            switch(footnoteMapType)
            {
                case STRING_MAP_TYPE.FOOT_NOTE_ADDITIVE_PROFILE:
                    message = Environment.NewLine + "Info: Adding Additive Profile footnote for ID: " + id + ".";
                    break;
                case STRING_MAP_TYPE.FOOT_NOTE_PRODUCT:
                    message = Environment.NewLine + "Info: Adding Product footnote for ID: " + id + ".";
                    break;
                case STRING_MAP_TYPE.FOOT_NOTE_SHIPPER:
                    message = Environment.NewLine + "Info: Adding Shipper footnote for ID: " + id + ".";
                    break;
                case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO:
                    message = Environment.NewLine + "Info: Adding Ship-To footnote for ID: " + id + ".";
                    break;
                case STRING_MAP_TYPE.FOOT_NOTE_SHIPTO_STATE:
                    message = Environment.NewLine + "Info: Adding State footnote for ID: " + id + ".";
                    break;
            }

            return message;
        }
        #endregion
    }
}
