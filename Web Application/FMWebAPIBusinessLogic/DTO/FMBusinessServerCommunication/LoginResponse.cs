using FMBusinessObjects.DataObjects;
using System.Collections.Generic;

namespace FMWebAPIBusinessLogic.DTO.FMBusinessServerCommunication
{
    public class LoginResponse
    {
        public bool LoginSuccess { get; set; }
        public bool MustChangePassword { get; set; }
        public int DaysUntilPasswordExpires { get; set; }
        public SecurityClass SecurityProperties { get; set; }
        public SiteClass CurrentSite { get; set; }
        public IEnumerable<TransactionAliasClass> Transactions { get; set; }
    }
}
