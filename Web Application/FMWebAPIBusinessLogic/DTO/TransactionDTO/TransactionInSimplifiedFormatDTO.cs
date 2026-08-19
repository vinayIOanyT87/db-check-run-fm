using System.Collections.Generic;

namespace FMWebAPIBusinessLogic.DTO.TransactionDTO
{

    public class TransactionInSimplifiedFormatDTO
    {
        public TransactionInSimplifiedFormatDTO()
        {
            this.TransactionPropertyValuePairs = new Dictionary<string, string>();
        }
        public Dictionary<string, string> TransactionPropertyValuePairs { get; set; }
        public string TransactionAliasGuid { get; set; }
    }

}