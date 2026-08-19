using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using FMDepedencyManager;
using Unity;
using XMLImport;
using FMCore;
using FMWebAPIBusinessLogic;

namespace AccountingImportExport
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            FMServiceLocator.Container = new UnityContainer();
            FMServiceLocator.Container.RegisterFMCoreServices();
            FMServiceLocator.Container.RegisterFuelManagerWebAPIBusinessServices();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}