using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace FuelsManagerUploadService
{
    /// <summary>
    /// Summary description for FuelsManagerUpload
    /// </summary>
    [WebService(Namespace = "http://varec.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class FuelsManagerUpload : System.Web.Services.WebService
    {

        [WebMethod]
        public bool UploadToServer(string UploadData)
        {
            return true;
        }

        [WebMethod]
        public string DownloadFromServer()
        {
            return "ERROR FILE";
        }
    }
}
