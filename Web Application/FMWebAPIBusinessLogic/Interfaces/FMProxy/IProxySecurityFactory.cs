using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface IProxySecurityFactory
    {
        SecurityClass GetSecurity();
    }
}
