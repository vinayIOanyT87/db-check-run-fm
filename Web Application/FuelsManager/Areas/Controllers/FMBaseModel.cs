using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Areas.Controllers
{
    [Serializable]
    public class FMBaseModel
    {         
        public ResponseMessageClass Results = new ResponseMessageClass();

        public static string GetResults(object o)
        {
            if (o != null)
            {
                if (o.GetType().IsSubclassOf(typeof(FMBaseModel)))
                {
                    var scrSer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    return scrSer.Serialize(((FMBaseModel)o).Results);
                }
            }

            return "null";
        }
    }


}
