using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Interfaces.FMBusinessLogic
{
    public interface ICurrentRequestContext
    {
        SecurityClass GetCurrentSecurityContext();
        void SetCurrentSecurityContext(SecurityClass toSet);
        SiteClass GetCurrentSite();
        void SetCurrentSite(SiteClass toSet);
    }
}
