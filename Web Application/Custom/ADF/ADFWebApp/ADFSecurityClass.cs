using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FMBusinessObjects.Interfaces;
using FMBusinessObjects.DataObjects;

namespace ADFWebApp
{
    public class ADFSecurityClass : ISecurityDiscovery
    {
        public RightCollectionClass GetSecurityRights(SecurityClass security, uint options, uint specialKeyCodes)
        {
            RightCollectionClass Rights = new RightCollectionClass();

            Rights.Add(RIGHT.VIEW_WAC_HISTORY);
            Rights.Add(RIGHT.OVERRIDE_WAC);
            Rights.Add(RIGHT.MODIFY_INVOICE_QUERIES);

            return Rights;
        }
    }
}
