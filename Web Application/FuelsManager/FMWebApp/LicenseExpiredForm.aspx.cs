using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FuelsManager.Areas.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FuelsManager.FMWebApp
{
    public partial class LicenseExpiredForm : FMFormBase
    {
        public string licenseStatusText = string.Empty;
        public string AppName = "FuelsManager";
        protected void Page_Load(object sender, EventArgs e)
        {
            SecurityClass security = Session["Security"] as SecurityClass;
            if (security != null)
            {
                DateTime expirationDate = (System.DateTime) Session["LicenseExpirationDate"] ;
                SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                                                    x =>
                                                                                    x.GetUsingGuid(security, security.SiteGuid)
                                                                               );
                var dateTimeFormat = site.GetDateTimeFormatInfo();
                licenseStatusText = expirationDate.ToString("d", dateTimeFormat);
            }
            string fromAppSettingAppName = ConfigurationManager.AppSettings["LoginPageWelcomeTitle"];
            
            if (string.IsNullOrWhiteSpace(fromAppSettingAppName) == false)
            {
                AppName = fromAppSettingAppName;
            }
        }
    }
}