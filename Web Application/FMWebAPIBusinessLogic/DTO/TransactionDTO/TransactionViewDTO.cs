using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.DTO.TransactionDTO
{
    public class TransactionViewDTO : TransactionInSimplifiedFormatDTO
    {
        public bool CanBeReversed { get; set; }
        public bool CanBeEdited { get; set; }
        public string ReversalType { get; set; }
    }
}
