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
    public partial class AboutDatawarehouseLicense : FMFormBase
    {
        public string licenseStatusText = string.Empty;
        public string AppName = "FuelsManager";
      private const string NoDWNoDA = @"
         <div class=""formfieldtitle"" style=""position:relative; margin-top:50px; margin-left:80px"">
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
				<p>
					<span>Your license for Enterprise {0} does not include support for Data Analytics.</span>
				</p>
				<p  style='margin-bottom:0in;line-height:normal'>
					<span>To activate Data Analytics, please contact Varec Sales.</span>
				</p>	
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
				<p  style='margin-bottom:0in;line-height:normal'>
					<span>Thank you for being a valued customer of <span>{0}</span>!</span>
				</p>
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
			</div>	";
      private const string NoDA = @"
         <div class=""formfieldtitle"" style=""position:relative; margin-top:50px; margin-left:80px"">
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
				<p>
					<span>Your license for Enterprise {0} does not include the Data Analytics option.</span>
				</p>
				<p  style='margin-bottom:0in;line-height:normal'>
					<span>To activate the Data Analytics option, please contact Varec Sales.</span>
				</p>	
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
				<p  style='margin-bottom:0in;line-height:normal'>
					<span>Thank you for being a valued customer of <span>{0}</span>!</span>
				</p>
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
				<p  style='margin-bottom:0in;line-height:normal'><span>&nbsp</span></p>
			</div>	";
      public string LicenseMessage = string.Empty;

      protected void Page_Load(object sender, EventArgs e)
        {
         if (IsPostBack == false)
         {
            string fromAppSettingAppName = ConfigurationManager.AppSettings["LoginPageWelcomeTitle"];

            if (string.IsNullOrWhiteSpace(fromAppSettingAppName) == false)
            {
               AppName = fromAppSettingAppName;
            }
            bool isDataAnalyticsKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDataAnalyticsKey());
            bool isDataWarehouseKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDatawarehouseKey());
            if (isDataAnalyticsKey && isDataWarehouseKey) {
               return;
            }
				LicenseMessage = string.Format((!isDataAnalyticsKey && !isDataWarehouseKey ? NoDWNoDA : NoDA), AppName);


         }
      }
    }
}