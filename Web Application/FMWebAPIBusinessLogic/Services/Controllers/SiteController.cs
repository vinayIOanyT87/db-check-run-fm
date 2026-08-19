using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMWebAPIBusinessLogic.DTO;
using FMWebAPIBusinessLogic.DTO.FMBusinessServerCommunication;
using FMWebAPIBusinessLogic.Interfaces.Controllers;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using FMWebAPIBusinessLogic.Interfaces.FMProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FMWebAPIBusinessLogic.Services.Controllers
{
    public class SiteController : ISiteController
    {
        private readonly ISiteProxy _siteProxy;
        private readonly ITransactionAliasesProxy _transactionAliasesProxy;
        private readonly ICurrentRequestContext _currentUserSecurity;
        private readonly FMWebAPIConfiguration _config;

        public SiteController(ISiteProxy siteProxy,
            ITransactionAliasesProxy transactionAliasesProxy,
            FMWebAPIConfiguration config,
            ICurrentRequestContext currentUserSecurity)
        {
            _siteProxy = siteProxy;
            _config = config;
            _transactionAliasesProxy = transactionAliasesProxy;
            _currentUserSecurity = currentUserSecurity;
        }

        public bool CheckToken(string token)
        {
            bool valid = false;

            try
            {
                var securityResponse = _siteProxy.GetSecurity(token);
                this._currentUserSecurity.SetCurrentSecurityContext(securityResponse);
                var currentSite = _siteProxy.Get(securityResponse.SiteGuid, false, false, false);
                this._currentUserSecurity.SetCurrentSite(currentSite);
                valid = true;
            }

            catch (Exception e)
            {
                valid = false;
            }

            return valid;
        }

        public LoginResponse Login(string username, string password, string siteName)
        {
            SecurityClass fmSecurity = null;
            bool changePassword = false;
            int daysUntilExpiration = 0;

            var request = new SecurityLoginRequest()
            {
                CACEnabled = false,
                UserID = username,
                Password = password,
                SiteID = siteName,
                TimeOut = _config.FuelsManagerSessionTimeoutMinutes
            };

            var response = _siteProxy.Login(out changePassword, out daysUntilExpiration, out fmSecurity, request);

            // For invalid logins, the return value is set to an error message which starts with "User" or "Login Failed".
            if ((response != null) && (response.StartsWith("User") || response.ToUpper().StartsWith("LOGIN FAILED")))
            {
                return new LoginResponse()
                {
                    LoginSuccess = false,
                    SecurityProperties = null,
                    MustChangePassword = false,
                    DaysUntilPasswordExpires = 0
                };
            }
            _currentUserSecurity.SetCurrentSecurityContext(fmSecurity);

            var site = _siteProxy.Get(fmSecurity.SiteGuid, false, false, false);
            var transactions = this._transactionAliasesProxy.Enumerate().ToList();
            //var transactionAlliases = _transactionAliasesEndpoint.Enumerate(fmSecurity).ToList();
            //transactionAlliases = StripOutAliasesThatTheUserHasNoRightsTo(fmSecurity, transactionAlliases);

            return new LoginResponse()
            {
                LoginSuccess = true,
                SecurityProperties = fmSecurity,
                MustChangePassword = changePassword,
                DaysUntilPasswordExpires = daysUntilExpiration,
                CurrentSite = site,
                Transactions = transactions
            };
        }

        public LoginResponse Login(string token)
        {
            var security = _currentUserSecurity.GetCurrentSecurityContext();
            var site = this._siteProxy.Get(security.SiteGuid, false, false, false);
            var transactions = this._transactionAliasesProxy.Enumerate().ToList();
            return new LoginResponse()
            {
                LoginSuccess = true,
                SecurityProperties = security,
                MustChangePassword = false,
                DaysUntilPasswordExpires = 10000,
                CurrentSite = site,
                Transactions = transactions
            };
        }

        private List<TransactionAliasClass> StripOutAliasesThatTheUserHasNoRightsTo(SecurityClass security, List<TransactionAliasClass> aliase)
        {
            var result = new List<TransactionAliasClass>();
            //get the names of all the aliases the user has rights to modify
            var userRights = security.ModifyTransactionSecurityRights.Select(x => x.Key);
            //filter the current aliases we got back grom the server and return it if any of them match
            result = aliase.Where(x => userRights.Any(y => x.ID == y)).ToList();
            return result;
        }
    }
}
