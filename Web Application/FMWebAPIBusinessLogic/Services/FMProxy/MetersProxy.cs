using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class MetersProxy : IMetersProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;

        public MetersProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid Add(MeterClass meter)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MeterClass> Enumerate()
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IMeters, List<MeterClass>>(
                    service => service.Enumerate(currentSecurity));
                timer.Stop();
                _logger.Debug($"{nameof(this.Enumerate)} Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public IEnumerable<MeterClass> EnumerateAndFilter(string meterIDFilterValue)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MeterAssetClass> EnumerateAssets()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MeterAssetClass> EnumerateAssetsAndFilter(string assetIDFilterValue)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IMeters, List<MeterAssetClass>>(
                    service => service.EnumerateAssetsAndFilter(currentSecurity, assetIDFilterValue));
                timer.Stop();
                _logger.Debug($"{nameof(this.EnumerateByAssetGuid)} Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error getting meters by asset guid through proxy call.");
                throw;
            }
        }

        public IEnumerable<MeterClass> EnumerateByAssetGuid(Guid assetGuid)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IMeters, List<MeterClass> > (
                    service => service.EnumerateByAssetGuid(currentSecurity, assetGuid));
                timer.Stop();
                _logger.Debug($"{nameof(this.EnumerateByAssetGuid)} Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error getting meters by asset guid through proxy call.");
                throw;
            }
        }

        public IEnumerable<MeterClass> EnumerateByAssetGuidAndFilter(Guid assetGuid, string meterIDFilterValue)
        {
            throw new NotImplementedException();
        }

        public MeterClass Get(Guid identityGuid)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IMeters, MeterClass>(
                    service => service.Get(currentSecurity, identityGuid));
                timer.Stop();
                _logger.Debug($"{nameof(this.Get)} Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error getting meter identity guid through proxy call.");
                throw;
            }
        }

        public Guid GetIdentityGuid(string id)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IMeters, Guid>(
                    service => service.GetIdentityGuid(currentSecurity, id));
                timer.Stop();
                _logger.Debug($"{nameof(this.GetIdentityGuid)} Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error getting meter identity guid through proxy call.");
                throw;
            }
        }

        public List<string> GetMeterIdsByAssetGuids(List<EquipmentClass> assets)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<IMeters, List<string>>(
                    service => service.GetMeterIdsByAssetGuids(currentSecurity, assets));
                timer.Stop();
                _logger.Debug($"{nameof(this.GetIdentityGuid)} Took {timer.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error getting meter IDs through proxy call.");
                throw;
            }
        }
    }
}
