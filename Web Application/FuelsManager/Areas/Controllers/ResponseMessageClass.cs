using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Areas.Controllers
{
    [Serializable]
    public class ResponseMessageClass
    {
        public Dictionary<string, List<string>> ErrorMessage { get; set; }
        public Dictionary<string, List<string>> SuccessMessage { get; set; }
        public object Data { get; set; }


        public int SuccessMessagesTimeoutMillisecond;

        public int ErrorMessagesTimeoutMillisecond;

        public ResponseMessageClass()
        {
            this.ErrorMessage = new Dictionary<string, List<string>>();
            this.SuccessMessage = new Dictionary<string, List<string>>();
            SuccessMessagesTimeoutMillisecond = 0;
            ErrorMessagesTimeoutMillisecond = 0;
            this.Data = null;
        }

    }
}
