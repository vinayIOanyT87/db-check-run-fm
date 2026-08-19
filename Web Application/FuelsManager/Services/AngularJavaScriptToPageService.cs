using FMBusinessObjects.DataObjects;
using FuelsManager.FMWebApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace FuelsManager.Services
{
    public class AngularJavaScriptToPageService
    {
        public AngularJavaScriptToPageService()
        {

        }

        public void DynamicallyLoadJSAndCSSOntoCurrentPage(Page currentPage, SecurityClass security)
        {
            //TODO: look for a form tag that we can add scripts too, otherwise throw an unsupported exception
            var asdf = currentPage.Items;
            var buildNumber = Assembly.GetExecutingAssembly().GetName().Version.Build;
            var baseUrl = String.Format("{0}://{1}{2}", currentPage.Request.Url.Scheme,
                currentPage.Request.Url.Authority,
                currentPage.ResolveUrl("~/AngularAppBinaries/"));
            var binaryPath = currentPage.Server.MapPath("~/AngularAppBinaries");
            string[] files = GetDirectoryContents(currentPage, binaryPath);
            var jsFiles = files.Where(x => Path.GetExtension(x).ToLower() == ".js");
            foreach (var jsFile in jsFiles)
            {
                var fileName = Path.GetFileName(jsFile);
                currentPage.Page.ClientScript.RegisterClientScriptInclude(fileName, PadVersionToFile(baseUrl, fileName, buildNumber));
            }
            var cssFiles = files.Where(x => Path.GetExtension(x).ToLower() == ".css");
            foreach (var cssFile in cssFiles)
            {
                var fileName = Path.GetFileName(cssFile);
                HtmlLink link = new HtmlLink();
                link.Href = PadVersionToFile(baseUrl, fileName, buildNumber);
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                currentPage.Page.Header.Controls.Add(link);
            }

            //inject the auth token and bootstrap everything
            currentPage.Page.ClientScript.RegisterClientScriptBlock(currentPage.Page.GetType(), "AuthToken",
                $"window.currentAuthenticationToken='{(security == null ? "" : security.Token.ToString())}';",
                true);
            currentPage.Page.ClientScript.RegisterClientScriptBlock(currentPage.Page.GetType(), "WebAPILocation",
                $"window.serverUrl='{(string.Format("{0}://{1}{2}", currentPage.Request.Url.Scheme, currentPage.Request.Url.Authority, "/FMWebAPI/api"))}';",
                true);
            currentPage.Page.ClientScript.RegisterClientScriptBlock(currentPage.Page.GetType(), "pingTimeout",
                $"window.pingTimeout = 10;",
                true);
            currentPage.Page.ClientScript.RegisterClientScriptBlock(currentPage.Page.GetType(), "InitAuthToken",
                @"    
function bootstrap() {
    if (!window['siteService']) {
        setTimeout(function () {
            console.log('wait just a bit longer for ie 11');
            bootstrap();
        }, 200);
    } else {
        siteService.setAuthenicationToken(window.currentAuthenticationToken).subscribe();
    }
}

document.addEventListener('DOMContentLoaded', function (event) {
    bootstrap();
});
        ", true);
        }

        internal void DynamicallyLoadJSAndCSSOntoCurrentPage(UserControl userControl, SecurityClass securityClass)
        {
            this.DynamicallyLoadJSAndCSSOntoCurrentPage(userControl.Page, securityClass);
        }

        private string PadVersionToFile(string baseUrl, string fileName, int versionBuildNumber)
        {
            return String.Format("{0}{1}?V={2}", baseUrl, fileName, versionBuildNumber);
        }

        private string[] GetDirectoryContents(Page currentPage, string binaryPath)
        {
            string[] contents = currentPage.Page.Cache["AngularDirectoryContents"] as string[];
            if (contents == null)
            {
                contents = Directory.GetFiles(binaryPath);
                currentPage.Page.Cache.Insert("AngularDirectoryContents", contents);
            }
            return contents;
        }
    }
}