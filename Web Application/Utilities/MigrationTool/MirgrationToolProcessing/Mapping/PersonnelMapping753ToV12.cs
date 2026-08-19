namespace MirgrationToolProcessing.Mapping
{
    using BusinessObjects.DataObjects;
    using BusinessObjects.MigrationObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using MigrationToolDataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class PersonnelMapping753ToV12 : PersonnelMappingBase
    {
        #region Data members
        private List<QualificationMapsBaseDo> qualificationMapsList;
        private QualificationCollectionClass targetQualificationCollection;
        private List<PersonCompanyMap753ToV12Do> sourcePersonCompanyMapList;
        private List<Schedule753ToV12Do> sourceSchedulePersonnelList;
        private MigrationDatabaseDAClass migrationDA;
        private Guid targetSiteGuid;
        private SiteClass targetSite;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public PersonnelMapping753ToV12()
        {
            base.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method implements the Perform Mapping for qualification.
        /// </summary>
        /// <param name="qualificationDo">The qualification data object to map.</param>
        /// <param name="migrationDA">The Migration data access object.</param>
        public override void PerformMapping(PersonnelBaseDo personnelDo, MigrationDatabaseDAClass migrationDA)
        {
            this.migrationDA = migrationDA;
            base.MessageFlag = false;
            base.Message = string.Empty;

            var personnelDoList = new List<Personnel753ToV12Do>();
            var personnel = personnelDo as Personnel753ToV12Do;
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
                personnel.GetSiteIndexByIdSql(command, base.SourceSiteId);
                DataSet dataSet = migrationDA.GetDataSet(command);
                sourceSiteIndex = personnel.GetSiteIndex(dataSet);
            }

            if (sourceSiteIndex == null)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Source Site Index is not found.";
                return;
            }

            this.targetSiteGuid = Guid.Empty;

            try
            {
                this.targetSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetByID(base.SecurityHndlr.Security, base.TargetSiteId, true));
                this.targetSiteGuid = targetSite.SiteGuid;
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

            using (var command = new SqlCommand())
            {
                personnel.EnumeratePersonnelSql(command, sourceSiteIndex.Value);
                sourceDataSet = migrationDA.GetDataSet(command);
            }

            if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Personnel found in the 7.5.3 " + personnelDo.SourceDbName + " database.";
                return;
            }

            foreach (DataRow row in sourceDataSet.Tables[0].Rows)
            {
                var newPersonnel = new Personnel753ToV12Do(personnel.SourceDbName, personnel.TargetDbName);
                newPersonnel.Load(row);
                personnelDoList.Add(newPersonnel);
            }

            if (personnelDoList.Count == 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: No Personnel found in the 7.5.3 " + personnelDo.SourceDbName + " database.";
                return;
            }

            // Get the list of source DB qualification maps.
            var sourceDbQualificationMapping = new SourceDbQualificationMapping753ToV12();
            sourceDbQualificationMapping.GetSourceQualificationMaps(this.migrationDA);
            this.qualificationMapsList = sourceDbQualificationMapping.QualificationMapsBaseList;

            try
            {
                // Get the target qualification list.
                this.GetTargetQualifications();
            }
            catch(Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Error: Could not get the target Qualification for site: " + base.TargetSiteId + ". " + ex.Message;
                return;
            }

            // Get the personnel to company map.
            sourceDataSet = null;

            try
            {
                this.sourcePersonCompanyMapList = new List<PersonCompanyMap753ToV12Do>();

                using (SqlCommand command = new SqlCommand())
                {
                    var sourcePersonCompanyMapDo = new PersonCompanyMap753ToV12Do(personnel.SourceDbName, personnel.TargetDbName);
                    sourcePersonCompanyMapDo.EnumeratePersonCompanyMapSql(command, sourceSiteIndex.Value);

                    sourceDataSet = migrationDA.GetDataSet(command);
                }

                if(sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Could not find any source Person/Command mapping for site: " + base.SourceSiteId + ".";
                }
                else
                {
                    foreach(DataRow row in sourceDataSet.Tables[0].Rows)
                    {
                        var newSourcePersonCompanyMapDo = new PersonCompanyMap753ToV12Do();
                        newSourcePersonCompanyMapDo.Load(row);
                        this.sourcePersonCompanyMapList.Add(newSourcePersonCompanyMapDo);
                    }
                }
            }
            catch (Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: Could not get the source Person/Command mapping for site: " + base.SourceSiteId + ". " + ex.Message;
            }

            // Get the personnel schedule
            sourceDataSet = null;
            try
            {
                this.sourceSchedulePersonnelList = new List<Schedule753ToV12Do>();

                using (SqlCommand command = new SqlCommand())
                {
                    const int ScheduleTypePersonnel = 3;
                    var sourceScheduleDo = new Schedule753ToV12Do(personnel.SourceDbName, personnel.TargetDbName);
                    sourceScheduleDo.EnumerateScheduleSql(command, ScheduleTypePersonnel);

                    sourceDataSet = migrationDA.GetDataSet(command);
                }

                if (sourceDataSet == null || sourceDataSet.Tables.Count == 0 || sourceDataSet.Tables[0].Rows.Count == 0)
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Could not find any source personnel schedule for site: " + base.SourceSiteId + ".";
                }
                else
                {
                    foreach (DataRow row in sourceDataSet.Tables[0].Rows)
                    {
                        var newSourceScheduleDo = new Schedule753ToV12Do();
                        newSourceScheduleDo.Load(row);
                        this.sourceSchedulePersonnelList.Add(newSourceScheduleDo);
                    }
                }
            }
            catch(Exception ex)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Warning: Could not get the source personnel schedule for site: " + base.SourceSiteId + ". " + ex.Message;
            }

            this.MapPersonnel(personnelDoList);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method maps the source personnel to the target.
        /// </summary>
        /// <param name="personnelDoList">The source personnel data object list.</param>
        private void MapPersonnel(List<Personnel753ToV12Do> personnelDoList)
        {
            base.EquipmentList = new List<EquipmentClass>();
            var entityAssignmentProcessor = new EntityService.EntityAssignmentProcessor(this.SecurityHndlr);

            // Note, the source site for entity assignment is the target site ID and the target entity assignment is the source site ID.
            var sourceEntitySiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, this.TargetSiteId));
            var targetEntitySiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetIdentityGuid(this.SecurityHndlr.Security, this.SourceSiteId));

            // Set the target site for the migration.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.GetSiteGuidById(base.TargetSiteId);

            // Get the list of target personnel to be used to check if already exists.
            var targetPersonList = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(x => x.Enumerate(base.SecurityHndlr.Security, false));
            int insertCount = 0;

            foreach (Personnel753ToV12Do sourcePersonnelDo in personnelDoList)
            {
                PersonClass existingPerson = this.PersonExists(sourcePersonnelDo.PersonId, ref targetPersonList);

                if (existingPerson == null)
                {
                    base.UpdateEquipmentList(sourcePersonnelDo.EquipmentId, this.targetSiteGuid);

                    PersonClass personDo = this.PopulateTargetPerson(sourcePersonnelDo);
                    personDo.SiteGuid = this.targetSiteGuid;
                    personDo.SiteID = base.TargetSiteId;

                    EquipmentClass equipment = base.EquipmentList.Find(x => x.ID.ToUpper() == sourcePersonnelDo.EquipmentId.ToUpper());

                    if (equipment != null)
                    {
                        personDo.AssignedEquipmentGuid = equipment.IdentityGuid;
                        personDo.AssignedEquipmentID = sourcePersonnelDo.EquipmentId;
                    }

                    // Get the qualification from the source database and map it for a given
                    // person ID.
                    QualificationMapCollectionClass qualMapCollection = this.MapPersonQualifications(sourcePersonnelDo, personDo
                                                                                                , QualificationMapsBaseDo.QualificationMapTypes.PERSON_QUALIFICATION_TO_PERSON);
                    personDo.QualificationCollection = qualMapCollection;

                    qualMapCollection = this.MapPersonQualifications(sourcePersonnelDo, personDo, QualificationMapsBaseDo.QualificationMapTypes.PERSON_LICENSE_TO_PERSON);
                    personDo.LicenseCollection = qualMapCollection;

                    qualMapCollection = this.MapPersonQualifications(sourcePersonnelDo, personDo, QualificationMapsBaseDo.QualificationMapTypes.PERSON_TRAINING_TO_PERSON);
                    personDo.TrainingCollection = qualMapCollection;

                    try
                    {
                        PersonRoleMapCollectionClass  sourcePersonRoleMapList = this.MapPersonRoles(sourcePersonnelDo);

                        if (sourcePersonRoleMapList.Count == 0)
                        {
                            base.MessageFlag = true;
                            base.Message = base.Message + Environment.NewLine
                                                + "Error: Cannot find person role for ID: " + sourcePersonnelDo.PersonId + ". ";
                            continue;
                        }

                        personDo.RoleCollection = sourcePersonRoleMapList;
                    }
                    catch(Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Error: Retrieving person role for ID: " + sourcePersonnelDo.PersonId + ". " + ex.Message;
                        continue;
                    }

                    // Map the companies that the person is assigned based on the source information.
                    try
                    {
                        personDo.AssignedCompaniesCollection = this.GetTargetCompanyMapCollection(sourcePersonnelDo);
                    }
                    catch(Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Warning: Retrieving company map collection for person ID: " + sourcePersonnelDo.PersonId + ". " + ex.Message;
                    }

                    ScheduleCollectionClass targetPersonScheduleCollection = this.GetPersonSchedule(sourcePersonnelDo);

                    if(targetPersonScheduleCollection.Count == 0)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Info: No schedule for person ID: " + sourcePersonnelDo.PersonId + ".";
                    }
                    else
                    {
                        personDo.AccessScheduleCollection = targetPersonScheduleCollection;
                    }

                    // Add the person to the database via FM business services.
                    try
                    {
                        Guid targetPersonGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(x => x.Add(base.SecurityHndlr.Security, personDo));

                        // Entity assign the person to the target site.
                        string entityMessage = " For Person ID: '" + sourcePersonnelDo.PersonId + "' to the target Site: " + this.SourceSiteId + ".";
                        base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                            , targetPersonGuid
                                                            , sourceEntitySiteGuid
                                                            , targetEntitySiteGuid
                                                            , typeof(IPersonnel).GUID
                                                            , ENTITY_TYPE.PERSONNEL
                                                            , entityMessage);

                        insertCount++;
                    }
                    catch (Exception ex)
                    {
                        base.MessageFlag = true;
                        base.Message = base.Message + Environment.NewLine
                                            + "Info: Adding person ID '" + sourcePersonnelDo.PersonId + "' to the target DB. " + ex.Message;
                    }
                }
                else
                {
                    base.MessageFlag = true;
                    base.Message = base.Message + Environment.NewLine
                                        + "Info: Person ID '" + sourcePersonnelDo.PersonId + "' already exists at Target site '"
                                        + base.TargetSiteId + "'.";

                    // Entity assign the person to the target site.
                    string entityMessage = " For Person ID: '" + sourcePersonnelDo.PersonId + "' to the target Site: " + this.SourceSiteId + ".";
                    base.PerformEntityAssignmentHelper(entityAssignmentProcessor
                                                        , existingPerson.IdentityGuid
                                                        , sourceEntitySiteGuid
                                                        , targetEntitySiteGuid
                                                        , typeof(IPersonnel).GUID
                                                        , ENTITY_TYPE.PERSONNEL
                                                        , entityMessage);
                }
            }

            if (insertCount > 0)
            {
                base.MessageFlag = true;
                base.Message = base.Message + Environment.NewLine
                                    + "Info: Successfully migrated " + insertCount + " Person items.";
            }

            // Reset the site guid to SiteAdmin.
            base.SecurityHndlr.Security.SiteGuid = base.SecurityHndlr.SiteAdminGuid;
        }

        /// <summary>
        /// This method populates the person class.
        /// </summary>
        /// <param name="sourcePersonnelDo">The source person DO.</param>
        /// <returns>Returns the person class</returns>
        private PersonClass PopulateTargetPerson(Personnel753ToV12Do sourcePersonnelDo)
        {
            UserDataClass userData = new UserDataClass();
            userData[0] = sourcePersonnelDo.UserData1;
            userData[1] = sourcePersonnelDo.UserData2;
            userData[2] = sourcePersonnelDo.UserData3;
            userData[3] = sourcePersonnelDo.UserData4;
            userData[4] = sourcePersonnelDo.UserData5;
            userData[5] = sourcePersonnelDo.UserData6;
            userData[6] = sourcePersonnelDo.UserData7;
            userData[7] = sourcePersonnelDo.UserData8;
            userData[8] = sourcePersonnelDo.UserData9;
            userData[9] = sourcePersonnelDo.UserData10;
            userData[10] = sourcePersonnelDo.UserData11;
            userData[11] = sourcePersonnelDo.UserData12;
            userData[12] = sourcePersonnelDo.UserData13;
            userData[13] = sourcePersonnelDo.UserData14;
            userData[14] = sourcePersonnelDo.UserData15;
            userData[15] = sourcePersonnelDo.UserData16;
            userData[16] = sourcePersonnelDo.UserData17;
            userData[17] = sourcePersonnelDo.UserData18;
            userData[18] = sourcePersonnelDo.UserData19;
            userData[19] = sourcePersonnelDo.UserData20;
            userData[20] = sourcePersonnelDo.UserData21;
            userData[21] = sourcePersonnelDo.UserData22;
            userData[22] = sourcePersonnelDo.UserData23;
            userData[23] = sourcePersonnelDo.UserData24;

            var targetPersonDo = new PersonClass
            {
                ID                          = sourcePersonnelDo.PersonId
                , UserData                  = userData
                , CardNumber                = sourcePersonnelDo.CardNumber
                , UserGuid                  = Guid.Empty
                , UserID                    = string.Empty
                , FirstName                 = sourcePersonnelDo.FirstName
                , MiddleName                = sourcePersonnelDo.MiddleName
                , LastName                  = sourcePersonnelDo.LastName
                , Title                     = sourcePersonnelDo.Title
                , Department                = sourcePersonnelDo.Department
                , Address1                  = sourcePersonnelDo.Address1
                , Address2                  = sourcePersonnelDo.Address2
                , City                      = sourcePersonnelDo.City
                , State                     = sourcePersonnelDo.State
                , Zip                       = sourcePersonnelDo.Zip
                , Country                   = sourcePersonnelDo.Country
                , Phone1                    = sourcePersonnelDo.Phone1
                , Phone2                    = sourcePersonnelDo.Phone2
                , AssignmentDate            = sourcePersonnelDo.AssignmentDate.ToString()
                , SupervisionDate           = sourcePersonnelDo.SupervisionDate.ToString()
                , SSAN                      = sourcePersonnelDo.SSAN
                , BirthDate                 = sourcePersonnelDo.BirthDate.ToString()
                , PayRate                   = sourcePersonnelDo.PayRate == null ? 0 : sourcePersonnelDo.PayRate.Value
                , LaborRate1                = sourcePersonnelDo.LaborRate1 == null ? 0 : sourcePersonnelDo.LaborRate1.Value
                , LaborRate2                = sourcePersonnelDo.LaborRate2 == null ? 0 : sourcePersonnelDo.LaborRate2.Value
                , LaborRate3                = sourcePersonnelDo.LaborRate3 == null ? 0 : sourcePersonnelDo.LaborRate3.Value
                , LaborRate4                = sourcePersonnelDo.LaborRate4 == null ? 0 : sourcePersonnelDo.LaborRate4.Value
                , Status                    = sourcePersonnelDo.Status == null ? PersonClass.STATUS.In : (PersonClass.STATUS)sourcePersonnelDo.Status
                , Email                     = sourcePersonnelDo.Email
                , ResponsibleOfficer        = sourcePersonnelDo.ResponsibleOfficer
                , Shift                     = (sourcePersonnelDo.Shift == null ? (short)0 : sourcePersonnelDo.Shift.Value)
                , PINNumber                 = sourcePersonnelDo.PinNumber
                , PINRequired               = sourcePersonnelDo.PinRequired
                , LockedOut                 = sourcePersonnelDo.LockedOut
                , LockedOutReason           = sourcePersonnelDo.LockedOutReason
                , LockedOutDate             = sourcePersonnelDo.LockedOutDate == null ? string.Empty : sourcePersonnelDo.LockedOutDate.Value.ToString()
                , LastActivityDate          = sourcePersonnelDo.LastActivityDate == null ? string.Empty : sourcePersonnelDo.LastActivityDate.Value.ToString()
                , CardedIn                  = sourcePersonnelDo.CardedIn
                , ShortCardNumber           = sourcePersonnelDo.ShortCardNumber
                , OnFileSignature           = sourcePersonnelDo.OnFileSignature
                , InhibitInactivityLockout  = sourcePersonnelDo.InhibitInactivityLockout
                , CreatedBy                 = "Migration Tool"
                , UpdatedBy                 = "Migration Tool"
                , CreatedDate               = DateTimeOffset.Now
                , UpdatedDate               = DateTimeOffset.Now
            };

            return targetPersonDo;
        }

        /// <summary>
        /// This method is a helper to find if an existing person already exists at the
        /// target database.
        /// </summary>
        /// <param name="sourcePersonId">The source person ID</param>
        /// <param name="targetPersonList">The target person list.</param>
        /// <returns>Return null if the person does not exist at the target DB. Otherwise, returns the person class.</returns>
        private PersonClass PersonExists(string sourcePersonId, ref PersonCollectionClass targetPersonList)
        {
            if (targetPersonList == null || targetPersonList.Count <= 0)
            {
                return null;
            }

            PersonClass targetPerson = targetPersonList.Find(x => x.ID.ToUpper() == sourcePersonId.ToUpper());

            if (targetPerson == null)
            {
                return null;
            }

            return targetPerson;
        }

        /// <summary>
        /// This method will map the Person Qualifications, License, training  Maps.
        /// </summary>
        /// <param name="sourcePersonnelDo">The source personnel data object.</param>
        /// <param name="personDo">The target person data object.</param>
        /// <param name="mapType">The personnel map type.</param>
        private QualificationMapCollectionClass MapPersonQualifications(Personnel753ToV12Do sourcePersonnelDo
                                                                        , PersonClass personDo
                                                                        , QualificationMapsBaseDo.QualificationMapTypes mapType)
        {
            int personMapType = (int)mapType;

            QualificationMapCollectionClass personQualificationCollection = new QualificationMapCollectionClass();

            if (this.qualificationMapsList.Count == 0)
            {
                return personQualificationCollection;
            }

            List<QualificationMapsBaseDo> qualificationMapsBaseList = 
                                            this.qualificationMapsList.FindAll(x => x.Index == sourcePersonnelDo.PersonIndex 
                                            && x.Type == personMapType);

            if(qualificationMapsBaseList == null || qualificationMapsBaseList.Count == 0)
            {
                return personQualificationCollection;
            }

            foreach(QualificationMapsBaseDo qualificationMapsBaseDo in qualificationMapsBaseList)
            {
                Guid targetQualificationGuid = Guid.Empty;

                QualificationClass targetQualification =
                     this.targetQualificationCollection.Find(x => x.ID == qualificationMapsBaseDo.QualificationId);
                
                if(targetQualification != null)
                {
                    targetQualificationGuid = targetQualification.IdentityGuid;
                }

                var qualificationMaps753ToV12Do = (QualificationMaps753ToV12Do)qualificationMapsBaseDo;
                QualificationMapClass targetQualificationMap = new QualificationMapClass
                {
                    ID = qualificationMaps753ToV12Do.Id
                    , Number = qualificationMaps753ToV12Do.Id
                    , Type = (QUALIFICATION_MAP_TYPE)qualificationMaps753ToV12Do.Type
                    , Rating = qualificationMaps753ToV12Do.Rating
                    , HistoricalRecord = qualificationMaps753ToV12Do.HistoricalRecord
                    , Instructor = qualificationMaps753ToV12Do.Instructor
                    , SiteGuid = personDo.SiteGuid
                    , SiteID = personDo.SiteID
                    , AssignedGuid = targetQualificationGuid
                };

                if(qualificationMaps753ToV12Do.DateCompleted != null)
                {
                    var newDate = new Date { Value = qualificationMaps753ToV12Do.DateCompleted.Value };
                    targetQualificationMap.DateCompleted = newDate;
                }

                if (qualificationMaps753ToV12Do.DateDue != null)
                {
                    var newDate = new Date { Value = qualificationMaps753ToV12Do.DateDue.Value };
                    targetQualificationMap.DateDue = newDate;
                }

                if (qualificationMaps753ToV12Do.ExpirationDate != null)
                {
                    var newDate = new Date { Value = qualificationMaps753ToV12Do.ExpirationDate.Value };
                    targetQualificationMap.ExpirationDate = newDate;
                }

                personQualificationCollection.Add(targetQualificationMap);
            }

            return personQualificationCollection;
        }

        /// <summary>
        /// This method will populate the roles for a given person.
        /// </summary>
        /// <param name="sourcePersonDo">The source person data object.</param>
        /// <returns>Returns a list of person roles.</returns>
        private PersonRoleMapCollectionClass MapPersonRoles(Personnel753ToV12Do sourcePersonDo)
        {
            var targetPersonRoleMapList = new PersonRoleMapCollectionClass();

            if(sourcePersonDo.PersonIndex == -99)
            {
                return targetPersonRoleMapList;
            }

            using(SqlCommand command = new SqlCommand())
            {
                var sourcePersonRoleMapBaseDo = new PersonRoleMapBaseDo(sourcePersonDo.SourceDbName, null);
                sourcePersonRoleMapBaseDo.GetPersonMapByIndexSql(command, sourcePersonDo.PersonIndex.Value);

                DataSet dataSet = this.migrationDA.GetDataSet(command);

                if(dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
                {
                    return targetPersonRoleMapList;
                }

                foreach(DataRow row in dataSet.Tables[0].Rows)
                {
                    sourcePersonRoleMapBaseDo = new PersonRoleMapBaseDo();
                    sourcePersonRoleMapBaseDo.Load(row);

                    var personRoleMap = new PersonRoleMapClass
                    {
                        Role = (PERSON_ROLE)sourcePersonRoleMapBaseDo.Role
                        , IdentityGuid = Guid.Empty
                        , PersonGuid = Guid.Empty
                        , SiteGuid = this.targetSiteGuid
                        , SiteID = base.TargetSiteId
                    };

                    targetPersonRoleMapList.Add(personRoleMap);
                }
            }

            return targetPersonRoleMapList;
        }

        /// <summary>
        /// This method will return a company map collection for a given person.
        /// </summary>
        /// <param name="sourcePersonnelDo">The source personnel data object.</param>
        /// <returns>Return a company map collection.</returns>
        private CompanyMapCollectionClass GetTargetCompanyMapCollection(Personnel753ToV12Do sourcePersonnelDo)
        {
            CompanyMapCollectionClass targetCompanyMapCollection = new CompanyMapCollectionClass();

            if(sourcePersonnelDo == null)
            {
                return targetCompanyMapCollection;
            }

            List<PersonCompanyMap753ToV12Do> sourcePersonCompanyMapList = 
                                                    this.sourcePersonCompanyMapList.FindAll(x => x.PersonIndex == sourcePersonnelDo.PersonIndex);

            if(sourcePersonCompanyMapList == null || sourcePersonCompanyMapList.Count == 0)
            {
                return targetCompanyMapCollection;
            }

            foreach(PersonCompanyMap753ToV12Do sourcePersonCompanyMap in sourcePersonCompanyMapList)
            {
                base.UpdateCompanyList(sourcePersonCompanyMap.CompanyId, this.targetSiteGuid);
                CompanyClass company = base.CompanyList.Find(x => x.ID.ToUpper() == sourcePersonCompanyMap.CompanyId.ToUpper());

                if(company == null)
                {
                    continue;
                }

                var assignedDriver                  = CompanyMapClass.CreateCompanyMap(COMPANY_MAP_TYPE.PERSON_ASSIGNED_COMPANY);
                assignedDriver.AssignedGuid         = company.IdentityGuid;
                assignedDriver.AssignedID           = company.ID;
                assignedDriver.AssignedToGuid       = Guid.Empty;
                assignedDriver.AssignedToID         = sourcePersonCompanyMap.PersonId;
                assignedDriver.AssignedToFirstName  = sourcePersonCompanyMap.FirstName;
                assignedDriver.AssignedToMiddleName = sourcePersonCompanyMap.MiddleName;
                assignedDriver.AssignedToLastName   = sourcePersonCompanyMap.LastName;

                targetCompanyMapCollection.Add(assignedDriver);
            }

            return targetCompanyMapCollection;
        }

        /// <summary>
        /// This method will retrieve the target qualification data.
        /// </summary>
        private void GetTargetQualifications()
        {
            this.SecurityHndlr.Security.SiteGuid = this.targetSiteGuid;
            this.SecurityHndlr.Security.SiteID = base.TargetSiteId;

            this.targetQualificationCollection = 
                            FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(x => x.Enumerate(this.SecurityHndlr.Security));

            this.SecurityHndlr.Security.SiteGuid = this.SecurityHndlr.SiteAdminGuid;
            this.SecurityHndlr.Security.SiteID = "SiteAdmin";
        }

        /// <summary>
        /// This method will get the schedule for a given person.
        /// </summary>
        /// <param name="sourcePersonnelDo">The source person data.</param>
        /// <returns>Return a collection of target schedules.</returns>
        private ScheduleCollectionClass GetPersonSchedule(Personnel753ToV12Do sourcePersonnelDo)
        {
            var targetPersonScheduleCollection = new ScheduleCollectionClass();
            List<Schedule753ToV12Do> sourcePersonScheduleList = this.sourceSchedulePersonnelList.FindAll(x => x.EntityIndex == sourcePersonnelDo.PersonIndex);

            if(sourcePersonScheduleList == null || sourcePersonScheduleList.Count == 0)
            {
                return targetPersonScheduleCollection;
            }

            foreach(Schedule753ToV12Do sourcePersonSchedule in sourcePersonScheduleList)
            {
                var targetSchedule = new ScheduleClass
                {
                    Type                = (SCHEDULE_TYPE)sourcePersonSchedule.Type,
                    Day                 = sourcePersonSchedule.Day,
                    Enabled             = sourcePersonSchedule.Enabled,
                    EndOfDayEnabled     = sourcePersonSchedule.EndOfDayEnabled
                };

                targetSchedule.OpeningTime  = new Time(this.targetSite) { Value = sourcePersonSchedule.OpeningTime };
                targetSchedule.ClosingTime  = new Time(this.targetSite) { Value = sourcePersonSchedule.ClosingTime };
                targetSchedule.EndOfDayTime = new Time(this.targetSite) { Value = sourcePersonSchedule.EndOfDayTime };

                targetPersonScheduleCollection.Add(targetSchedule);
            }

            return targetPersonScheduleCollection;
        }
        #endregion
    }
}
