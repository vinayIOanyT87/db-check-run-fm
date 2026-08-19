using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.DTO.TransactionDTO
{
    public class TransactionAliasFieldPlacementDTO
    {
        public Guid TransactionAliasGuid { get; set; }
        public string PlacementInformation { get; set; }
    }
}
