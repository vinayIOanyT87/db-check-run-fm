using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMCore.Interfaces;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FMWebAPIBusinessLogic.Services.FMProxy
{
    public class SiteProxy : ISiteProxy
    {
        ICurrentRequestContext _requestContext;
        IFMCustomLogger _logger;
        public SiteProxy(ICurrentRequestContext requestContext,
            IFMCustomLogger logger)
        {
            this._requestContext = requestContext;
            this._logger = logger;
        }

        public Guid Add(SiteClass site, string userID, string password)
        {
            throw new NotImplementedException();
        }

        public bool CheckCurrentPassword(UserClass user, string passwordText)
        {
            throw new NotImplementedException();
        }

        public Guid CreateDefaultSingleSite(SiteClass site)
        {
            throw new NotImplementedException();
        }

        public Guid CreateDefaultSingleSiteByLoginID(SiteClass site, string databaseLogOnId)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass Enumerate(SecurityClass security)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateByCandidateChildrenSites(Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateByChildSite(Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateByChildSiteForUser(Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateByParentSite(Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateByParentSiteCurrentUserAssigned(Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateByServiceLogin(string serviceLogin)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateBySiteGroup(bool siteGroup)
        {
            throw new NotImplementedException();
        }

        public SiteSelectList EnumerateForSiteSelect(Guid parentSiteGuid)
        {
            throw new NotImplementedException();
        }

        public List<SiteClass> EnumerateIndexIdGroupFlag(SecurityClass security)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateLimitSiteMemberByParentSite(Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public List<SiteClass> EnumerateReportDirectories(SecurityClass security)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateSitesByAssignedUser(Guid siteGuid, Guid userGuid)
        {
            throw new NotImplementedException();
        }

        public SiteCollectionClass EnumerateSitesInfo(SecurityClass security)
        {
            throw new NotImplementedException();
        }

        public SiteSyncList EnumerateSiteSynchronizationListBySiteSQL(Guid SiteGuid)
        {
            throw new NotImplementedException();
        }

        public SiteClass Get(Guid identityGuid, bool getMemberSites, bool getSchedulesAndProcessVariables, bool bGetAssociatedAliases)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ISites, SiteClass>(services =>
                    services.Get(currentSecurity, identityGuid, getMemberSites, getSchedulesAndProcessVariables, bGetAssociatedAliases)
                );
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

        public SiteClass GetBasic(Guid identityGuid)
        {
            throw new NotImplementedException();
        }

        public SiteClass GetByID(string SiteID, bool skipReset)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var currentSecurity = this._requestContext.GetCurrentSecurityContext();
                var result = FMChannelHelper.MakeCall<ISites, SiteClass>(services =>
                    services.GetByID(currentSecurity, SiteID, skipReset)
                );
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

        public SiteClass GetByMemberAndProcessVariables(Guid identityGuid, bool getMemberSites, bool getSchedulesAndProcessVariables)
        {
            throw new NotImplementedException();
        }

        public Guid GetIdentityGuid(string siteID)
        {
            throw new NotImplementedException();
        }

        public string GetIDNoRefresh(Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public string GetNextDocumentNumber(DOCUMENT_TYPE Type, Guid siteGuid)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNextDocumentNumbers(DOCUMENT_TYPE type, Guid siteGuid, int numberDesired)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNextDocumentNumbers(List<DOCUMENT_TYPE> documentTypes, Guid siteGuid, int numberDesired)
        {
            throw new NotImplementedException();
        }

        public string GetNextInvoiceNumber(SecurityClass security)
        {
            throw new NotImplementedException();
        }

        public string GetReportDirectory(string ReportPath)
        {
            throw new NotImplementedException();
        }

        public SecurityClass GetSecurity(string token)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                var result = FMChannelHelper.MakeCall<ISites, SecurityClass>(services =>
                    services.GetSecurity(token)
                );
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

        public int GetSiteCount(SecurityClass security)
        {
            throw new NotImplementedException();
        }

        public int GetSiteCountByServiceLogin(string serviceLogin)
        {
            throw new NotImplementedException();
        }

        public SiteClass GetUsingGuid(Guid identityGuid)
        {
            throw new NotImplementedException();
        }

        public void InitializeNextDocumentInvoiceNumbers(string siteId)
        {
            throw new NotImplementedException();
        }

        public bool IsSiteGroup(Guid identityGuid)
        {
            throw new NotImplementedException();
        }

        public string Login(out bool ChangePassword, out int DaysUntilExpiration, out SecurityClass security, SecurityLoginRequest sr)
        {
            try
            {
                var timer = Stopwatch.StartNew();
                bool _changePassword = false;
                int _daysUntilExpiration = 0;
                SecurityClass _security = null;
                var result = FMChannelHelper.MakeCall<ISites, string>(
                    service =>
                        service.Login(out _changePassword, out _daysUntilExpiration, out _security, sr)
                    );
                ChangePassword = _changePassword;
                DaysUntilExpiration = _daysUntilExpiration;
                security = _security;
                timer.Stop();
                _logger.Debug($"Took {timer.ElapsedMilliseconds}ms");

                return result;
            }
            catch(Exception e)
            {
                _logger.Error(e, "something failed");
                throw;
            }
        }

        public SecurityLoginResponse Login2(SecurityLoginRequest sr)
        {
            throw new NotImplementedException();
        }

        public void Logout(SecurityClass security)
        {
            throw new NotImplementedException();
        }

        public void LogoutToken(string token)
        {
            throw new NotImplementedException();
        }

        public void Modify(DATA_TYPE Type, SiteClass Site, bool updateDocumentNumbers)
        {
            throw new NotImplementedException();
        }

        public void ModifySecurity(string Token, SecurityClass security)
        {
            throw new NotImplementedException();
        }

        public void Purge(Guid identityGuid)
        {
            throw new NotImplementedException();
        }

        public void RefreshTransactionSecurityRightsCache(ref SecurityClass security)
        {
            throw new NotImplementedException();
        }
    }
}
