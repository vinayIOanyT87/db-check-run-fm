using FMBusinessObjects.DataObjects;
using FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic;
using System;

namespace FMWebAPIBusinessLogic.Services.FMBusinessLogic
{
    public class CurrentRequestContext : ICurrentRequestContext
    {
        private SecurityClass _currentSecurity;
        private SiteClass _currentSite;
        public SecurityClass GetCurrentSecurityContext()
        {
            if (_currentSecurity == null)
            {
                throw new ApplicationException("Please set current security context before trying to access it");
            }
            return _currentSecurity;
        }

        public SiteClass GetCurrentSite()
        {
            if (_currentSite == null)
            {
                throw new ApplicationException("Please set current user site context before trying to access it");
            }
            return _currentSite;
        }

        public void SetCurrentSecurityContext(SecurityClass toSet)
        {
            this._currentSecurity = toSet;
        }

        public void SetCurrentSite(SiteClass toSet)
        {
            this._currentSite = toSet;
        }
    }
}
