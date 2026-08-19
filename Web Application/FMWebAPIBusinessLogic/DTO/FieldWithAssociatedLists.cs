using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMWebAPIBusinessLogic.DTO
{

    public class FieldWithAssociatedList
    {
        public FieldWithAssociatedList()
        {
            this.Options = new List<string>();
        }
        public string FieldName { get; set; }
        public IEnumerable<string> Options { get; set; }
    }
}
