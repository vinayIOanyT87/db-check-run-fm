
namespace FMEnterpriseManagementProxyServices.ServiceClasses
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.ServiceRequests;

    using FMEnterpriseManagementBusinessObjects.BusinessInterfaces;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
	public class ClientEnterpriseManagementService : IClientEnterpriseManagementService
	{
		public void ReadHardwareKey()
		{
			FMChannelHelper.MakeCall<IHardwareKey>(
				x =>
				{
					x.ReadHardwareKey();

					// check that the dispatch option is enabled in the key
					if ((x.GetOptionsCell() & 0x1000) == 0)
					{
						throw new Exception("Dispatch Not Authorized For This Computer");
					}
				});
		}

		public void IsDefenseKey()
		{
			//FMChannelFactory<IHardwareKey>.RefreshConfiguration();
			FMChannelHelper.MakeCall<IHardwareKey>(x => x.IsDefenseKey());
		}

		public SecurityLoginResponse Login(SecurityLoginRequest sr)
		{
			SecurityLoginResponse loginResult =
				FMChannelHelper.MakeCall<ISites, SecurityLoginResponse>(
					x => x.Login2(sr));
			return loginResult;
		}

		public void PingSession(SecurityClass security)
		{
			FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(security));

		}

	    void IClientEnterpriseManagementService.Logout(SecurityClass security)
	    {
	        FMChannelHelper.MakeCall<ISites>(x => x.Logout(security));
	    }

	    public EquipmentCollectionClass EnumerateEquipment(SecurityClass security)
	    {
	        throw new NotImplementedException();
	    }

	    public DataSet EnumerateEquipmentDataSet(
	        SecurityClass security,
	        bool managedEquipmentOnly,
	        bool secondaryStorageOnly,
	        Guid equipmentTypeGuid,
	        EQUIPMENT_TYPE equipmentType,
	        string translatedUnassigned,
	        string filter,
	        bool isDefense,
	        bool hideHiddenEquipmentRecords = false,
			int limit = 1500)
	    {
	        if (!security.HasRight(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION))
	        {
	            throw new FMInsufficientRightsException();
	        }

	        return
	            FMChannelHelper.MakeCall<IEquipments, DataSet>(
	                x =>
	                    x.EnumerateDataSet(
	                        security,
	                        managedEquipmentOnly,
	                        secondaryStorageOnly,
	                        equipmentTypeGuid,
	                        equipmentType,
	                        translatedUnassigned,
	                        filter,
                            isDefense,
                            hideHiddenEquipmentRecords,
							limit));
	    }

        public PersonCollectionClass EnumeratePersonnel(SecurityClass security)
	    {
	        throw new NotImplementedException();
	    }

	    public PersonCollectionClass EnumeratePersonnelByRoleAndFilter(
	        SecurityClass security,
	        PERSON_ROLE role,
	        string filterString,
	        string orderBy,
	        bool hideHiddenPersonnel)
	    {
            if (!security.HasRight(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION))
            {
                throw new FMInsufficientRightsException();
            }

            return
                FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
                    x =>
                        x.EnumerateByRoleAndFilter(
                            security,
                            role,
                            filterString,
                            orderBy,
                            hideHiddenPersonnel));
        }

        public void RequestEnterpriseEquipmentAssignment(
	        SecurityClass enterpriseSecurity,
	        Guid assignedToSiteGuid,
	        Guid equipmentMasterGuid)
	    {
            if (!enterpriseSecurity.HasRight(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION))
            {
                throw new FMInsufficientRightsException();
            }

            // Equipment could potentially have a collection of compartments; these will need to be assigned as well.
            // Collect into a list of equipment
	        List<EquipmentClass> equipmentList = new List<EquipmentClass>
	                                             {
	                                                 FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(enterpriseSecurity, equipmentMasterGuid))
	                                             };

	        // Now add compartments
	        foreach (EquipmentClass compartment in equipmentList[0].CompartmentCollection)
	        {
	            equipmentList.Add(compartment);
	        }

	        var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(enterpriseSecurity, assignedToSiteGuid));

	        foreach (EquipmentClass equipment in equipmentList)
	        {
	            var entityToSiteMap = new EntityToSiteMapClass
	                                  {
	                                      TypeID = ENTITY_TYPE.EQUIPMENT,
	                                      ID = equipment.ID,
	                                      IdentityGuid = equipment.MasterRecordGuid,
	                                      SiteID = site.ID,
	                                      SiteGuid = assignedToSiteGuid,
	                                      AssignedFromSiteId = enterpriseSecurity.LoginSiteID,
	                                      AssignedFromSiteGuid = enterpriseSecurity.LoginSiteGuid,
	                                      IsAssigned = true
	                                  };

	            FMChannelHelper.MakeCall<IEntityToSiteMaps>(
	                x => x.Add(enterpriseSecurity, entityToSiteMap, typeof(IEquipments).GUID));
	        }
	    }

        public void RequestEnterprisePersonAssignment(
            SecurityClass enterpriseSecurity,
            Guid assignedToSiteGuid,
            Guid personMasterGuid)
	    {
            if (!enterpriseSecurity.HasRight(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION))
            {
                throw new FMInsufficientRightsException();
            }

            // Get Person entity
            PersonClass person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(enterpriseSecurity, personMasterGuid));

            var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(enterpriseSecurity, assignedToSiteGuid));

                var entityToSiteMap = new EntityToSiteMapClass
                {
                    TypeID = ENTITY_TYPE.PERSONNEL,
                    ID = person.ID,
                    IdentityGuid = person.MasterRecordGuid,
                    SiteID = site.ID,
                    SiteGuid = assignedToSiteGuid,
                    AssignedFromSiteId = enterpriseSecurity.LoginSiteID,
                    AssignedFromSiteGuid = enterpriseSecurity.LoginSiteGuid,
                    IsAssigned = true
                };

                FMChannelHelper.MakeCall<IEntityToSiteMaps>(
                    x => x.Add(enterpriseSecurity, entityToSiteMap, typeof(IPersonnel).GUID));
        }

        public EquipmentClass GetEquipment(SecurityClass enterpriseSecurity, Guid equipmentGuid)
		{
            if (!enterpriseSecurity.HasRight(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION))
            {
                throw new FMInsufficientRightsException();
            }

            var equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(enterpriseSecurity, equipmentGuid));
			return equipment;
		}

        public Guid GetEquipmentMasterGuid(SecurityClass enterpriseSecurity, string equipmentId)
        {
            if (!enterpriseSecurity.HasRight(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION))
            {
                throw new FMInsufficientRightsException();
            }

            var equipment = FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.GetMasterRecordGuid(enterpriseSecurity, equipmentId));
            return equipment;
        }

        public PersonClass GetPerson(SecurityClass enterpriseSecurity, Guid personMasterGuid)
		{
            if (!enterpriseSecurity.HasRight(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION))
            {
                throw new FMInsufficientRightsException();
            }

            var person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(enterpriseSecurity, personMasterGuid));
            return person;
        }

	    public Guid GetPersonMasterGuid(SecurityClass enterpriseSecurity, string personId)
	    {
            if (!enterpriseSecurity.HasRight(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION))
            {
                throw new FMInsufficientRightsException();
            }

            var person = FMChannelHelper.MakeCall<IPersonnel, Guid>(x => x.GetMasterRecordGuid(enterpriseSecurity, personId));
            return person;
        }

        public string Logout(SecurityClass security)
		{
			var alertSetting = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_AlertSessionLogoutEnabled));
			FMChannelHelper.MakeCall<ISites>(x => x.Logout(security));
			return alertSetting;

		}
	}
}
