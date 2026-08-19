using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
    [Serializable]
    public class ResponseMessage
    {
        public Dictionary<string, List<string>> ErrorMessage { get; set; }
        public Dictionary<string, List<string>> SuccessMessage { get; set; }
        public object Data { get; set; }

        public ResponseMessage()
        {
            this.ErrorMessage = new Dictionary<string, List<string>>();
            this.SuccessMessage = new Dictionary<string, List<string>>();
            this.Data = null;
        }

    }
}
