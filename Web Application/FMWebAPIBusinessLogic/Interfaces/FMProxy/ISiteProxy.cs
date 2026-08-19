using System;
using System.Collections.Generic;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface ISiteProxy
    {
        Guid Add(SiteClass site, string userID, string password);
        bool CheckCurrentPassword(UserClass user, string passwordText);
        Guid CreateDefaultSingleSite(SiteClass site);
        Guid CreateDefaultSingleSiteByLoginID(SiteClass site, string databaseLogOnId);
        SiteCollectionClass Enumerate(SecurityClass security);
        SiteCollectionClass EnumerateByCandidateChildrenSites(Guid siteGuid);
        SiteCollectionClass EnumerateByChildSite(Guid siteGuid);
        SiteCollectionClass EnumerateByChildSiteForUser(Guid siteGuid);
        SiteCollectionClass EnumerateByParentSite(Guid siteGuid);
        SiteCollectionClass EnumerateByParentSiteCurrentUserAssigned(Guid siteGuid);
        SiteCollectionClass EnumerateByServiceLogin(string serviceLogin);
        SiteCollectionClass EnumerateBySiteGroup(bool siteGroup);
        SiteSelectList EnumerateForSiteSelect(Guid parentSiteGuid);
        List<SiteClass> EnumerateIndexIdGroupFlag(SecurityClass security);
        SiteCollectionClass EnumerateLimitSiteMemberByParentSite(Guid siteGuid);
        List<SiteClass> EnumerateReportDirectories(SecurityClass security);
        SiteCollectionClass EnumerateSitesByAssignedUser(Guid siteGuid, Guid userGuid);
        SiteCollectionClass EnumerateSitesInfo(SecurityClass security);
        SiteSyncList EnumerateSiteSynchronizationListBySiteSQL(Guid SiteGuid);
        SiteClass Get(Guid identityGuid, bool getMemberSites, bool getSchedulesAndProcessVariables, bool bGetAssociatedAliases);
        SiteClass GetBasic(Guid identityGuid);
        SiteClass GetByID(string SiteID, bool skipReset);
        SiteClass GetByMemberAndProcessVariables(Guid identityGuid, bool getMemberSites, bool getSchedulesAndProcessVariables);
        Guid GetIdentityGuid(string siteID);
        string GetIDNoRefresh(Guid siteGuid);
        string GetNextDocumentNumber(DOCUMENT_TYPE Type, Guid siteGuid);
        List<string> GetNextDocumentNumbers(DOCUMENT_TYPE type, Guid siteGuid, int numberDesired);
        List<string> GetNextDocumentNumbers(List<DOCUMENT_TYPE> documentTypes, Guid siteGuid, int numberDesired);
        string GetNextInvoiceNumber(SecurityClass security);
        string GetReportDirectory(string ReportPath);
        SecurityClass GetSecurity(string token);
        int GetSiteCount(SecurityClass security);
        int GetSiteCountByServiceLogin(string serviceLogin);
        SiteClass GetUsingGuid(Guid identityGuid);
        void InitializeNextDocumentInvoiceNumbers(string siteId);
        bool IsSiteGroup(Guid identityGuid);
        string Login(out bool ChangePassword, out int DaysUntilExpiration, out SecurityClass security, SecurityLoginRequest sr);
        SecurityLoginResponse Login2(SecurityLoginRequest sr);
        void Logout(SecurityClass security);
        void LogoutToken(string token);
        void Modify(DATA_TYPE Type, SiteClass Site, bool updateDocumentNumbers);
        void ModifySecurity(string Token, SecurityClass security);
        void Purge(Guid identityGuid);
        void RefreshTransactionSecurityRightsCache(ref SecurityClass security);
    }
}