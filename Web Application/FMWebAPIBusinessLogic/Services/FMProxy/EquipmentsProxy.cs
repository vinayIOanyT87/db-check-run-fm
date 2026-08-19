using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Data;
using System.Diagnostics;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class EquipmentsProxy : IEquipmentsProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;

        public EquipmentsProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid Add(EquipmentClass equipment)
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass Enumerate(bool hideHiddenEquipmentRecords = false)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
                    service => service.Enumerate(currentSecurity, hideHiddenEquipmentRecords));
                timer.Stop();
                _logger.Debug($"{nameof(this.Enumerate)}Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public EquipmentCollectionClass EnumerateByCompany(Guid companyGuid, SiteClass site, bool hideHiddenEquipmentRecords = false)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
                    service => service.EnumerateByCompany(currentSecurity, companyGuid, hideHiddenEquipmentRecords));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public EquipmentCollectionClass EnumerateByCompanyAndLocalize(Guid companyGuid, bool localize, bool hideHiddenEquipmentRecords = false)
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateByCompanyAndSite(Guid companyGuid, SiteClass site, bool hideHiddenEquipmentRecords = false)
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateByFuelCard(Guid fuelCardGuid, bool hideHiddenEquipmentRecords = false)
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateByManagedFillstand()
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateBySource()
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateByTypeAndFilterAndProduct(EQUIPMENT_TYPE type, string filter, Guid productGuid, bool excludeNonEditableCompanyGuid, bool excludeNonEditableFuelCardGuid, bool hideHiddenEquipmentRecords = false)
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateByTypeAndProduct(EQUIPMENT_TYPE type, Guid productGuid, bool excludeNonEditableCompanyGuid, bool excludeNonEditableFuelCardGuid, bool hideHiddenEquipmentRecords = false)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(EQUIPMENT_TYPE[] types, object companyGuid, object fuelCardGuid, object productGuid, object secondaryStorage)
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateDataSet(bool managedEquipmentOnly, bool secondaryStorageOnly, Guid equipmentTypeGuid, EQUIPMENT_TYPE equipmentType, string translatedUnassigned, string filter, bool isDefense, bool hideHiddenEquipmentRecords = false)
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateExt(Guid equipmentTypeGuid, bool managedEquipmentOnly = false, bool secondaryStorageOnly = false, EQUIPMENT_TYPE equipmentType = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE, string filter = null, int limit = 0, bool hideHiddenEquipmentRecords = false)
        {
            throw new NotImplementedException();
        }

        public EquipmentInfo[] EnumerateInfo()
        {
            throw new NotImplementedException();
        }

        public EquipmentInfo[] EnumerateInfoByTypesCompanyFuelCardProductAndSecondaryStorage(EQUIPMENT_TYPE[] types, object companyGuid, object fuelCardGuid, object productIndexGuid, object secondaryStorage, bool hideHiddenEquipmentRecords = false)
        {
            throw new NotImplementedException();
        }

        public EquipmentInfo[] EnumerateInfoUndelegated()
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateManagedEquipment()
        {
            throw new NotImplementedException();
        }

        public EquipmentCollectionClass EnumerateManagedEquipmentWithoutQualityTag()
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateUpdateVersions()
        {
            throw new NotImplementedException();
        }

        public DataSet EnumerateUpdateVersionsForOpc()
        {
            throw new NotImplementedException();
        }

        public EquipmentClass Get(Guid equipmentGuid)
        {
            throw new NotImplementedException();
        }

        public EquipmentClass GetBasicInfo(Guid equipmentGuid, Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public EquipmentClass GetBySite(Guid equipmentGuid, SiteClass site)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdentityGuid(string id)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IEquipments, Guid>(
                    service => service.GetIdentityGuid(currentSecurity, id));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public Guid GetIdentityGuidByCardNumberAndEquipmentID(Guid companyGuid, string truckCardNumber)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdentityGuidByCompanyGuidAndEquipmentID(Guid companyGuid, string companyEquipmentID)
        {
            throw new NotImplementedException();
        }

        public string GetLatestRowVersionBySource()
        {
            throw new NotImplementedException();
        }

        public Guid GetMasterRecordGuid(string id)
        {
            throw new NotImplementedException();
        }

        public void Import(EquipmentClass equipment)
        {
            throw new NotImplementedException();
        }

        public void Modify(EquipmentClass equipment)
        {
            throw new NotImplementedException();
        }

        public void Purge(Guid equipmentGuid)
        {
            throw new NotImplementedException();
        }

        public EquipmentClass GetByMeterGuid(Guid meterGuid)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
                    service => service.GetByMeterGuid(currentSecurity, meterGuid));
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }
    }
}
