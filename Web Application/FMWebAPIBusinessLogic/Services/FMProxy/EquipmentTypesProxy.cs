using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Varec.CommonComponents.EngineeringUnitsLibrary;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class EquipmentTypesProxy : IEquipmentTypesProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;
        public EquipmentTypesProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }
        public Guid Add(EquipmentTypeClass equipmentType)
        {
            throw new NotImplementedException();
        }

        public EquipmentTypeCollectionClass Enumerate(string filter, string order)
        {
            throw new NotImplementedException();
        }

        public AirplaneTankCollectionClass EnumerateByEquipmentType(Guid equipmentTypeGuid, EngineeringUnit capacityUnits, int capacityDecimalPlaces)
        {
            throw new NotImplementedException();
        }

        public AirplaneTankCollectionClass EnumerateByEquipmentType(Guid equipmentTypeGuid)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateDataSet(string filter, string order)
        {
            throw new NotImplementedException();
        }

        public EquipmentTypeClass Get(Guid equipmentTypeGuid)
        {
            try
            {
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
                    service => service.Get(currentSecurity, equipmentTypeGuid));
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error getting EquipmentType from EquipmentTypesProxy");
                throw;
            }
        }

        public EquipmentTypeClass Get(Guid equipmentTypeGuid, SiteClass site)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdentityGuid(string id)
        {
            throw new NotImplementedException();
        }

        public void Import(EquipmentTypeClass equipmentType)
        {
            throw new NotImplementedException();
        }

        public void Modify(EquipmentTypeClass equipmentType)
        {
            throw new NotImplementedException();
        }

        public void Purge(Guid equipmentTypeGuid)
        {
            throw new NotImplementedException();
        }
    }
}
