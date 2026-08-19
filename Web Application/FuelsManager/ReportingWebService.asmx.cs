using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace FuelsManager
{
    /// <summary>
    /// Summary description for ReportingWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org")]
    [System.Web.Script.Services.ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ReportingWebService : System.Web.Services.WebService
    {
        [WebMethod]
        public List<string> GetReportParameters(string securityToken, string P1, string P2, string P3, string P4, string P5, string P6, string P7, string P8, string P9)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            AddParameter(parameters, P1);
            AddParameter(parameters, P2);
            AddParameter(parameters, P3);
            AddParameter(parameters, P4);
            AddParameter(parameters, P5);
            AddParameter(parameters, P6);
            AddParameter(parameters, P7);
            AddParameter(parameters, P8);
            AddParameter(parameters, P9);

            SecurityClass security = null;
            try
            {
                security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
            }
            finally
            {
                if (security == null || security.IdentityGuid == Guid.Empty)
                {
                    EventLog.WriteEntry("FuelsManager", "Session timed out or invalid");
                }
            }
            return FMChannelHelper.MakeCall<IReportingRequest, List<string>>(
                reportingRequests => reportingRequests.GetReportParameters(security, parameters));
        }
        [WebMethod]
        [SoapDocumentMethod(ParameterStyle = SoapParameterStyle.Wrapped)]
        public DataSet GetReportData(string securityToken, string P1, string P2, string P3, string P4, string P5, string P6, string P7, string P8, string P9)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            AddParameter(parameters, P1);
            AddParameter(parameters, P2);
            AddParameter(parameters, P3);
            AddParameter(parameters, P4);
            AddParameter(parameters, P5);
            AddParameter(parameters, P6);
            AddParameter(parameters, P7);
            AddParameter(parameters, P8);
            AddParameter(parameters, P9);

            SecurityClass security = null;
            try
            {
                // try to get the security class from the token
                security = FMChannelHelper.MakeCall<ISites, SecurityClass>(sites => sites.GetSecurity(securityToken));
            }
            catch
            {
            }

            // if the token is not valid, try to login as the specified userId 
            if (security == null)
            {
                // get the service login id
                security = new SecurityClass();
                string serviceLogin = FMChannelHelper.MakeCall<IDBAccess, string>(x => x.ServiceLogin(security));
                security.UserID = serviceLogin;

                // get the userId from the parameters
                string userId = GetRequiredParameter(parameters, "UserId");

                // get the siteId from the parameters
                string siteId = GetRequiredParameter(parameters, "SiteId");

                string siteGuidStr = GetRequiredParameter(parameters, "SiteGuid");
                var siteGuid = new Guid(siteGuidStr);

                // setup default rights so we can get the user
                var userRights = new RightCollectionClass();
                userRights = new RightCollectionClass();
                userRights.Add(RIGHT.VIEW_USERS);
                userRights.Add(RIGHT.MODIFY_USERS);
			    userRights.Add(RIGHT.IMPORT_ENTERPRISE_DATA);
                security.RightCollection = userRights;
                security.UserID = userId;
                security.SiteID = siteId;
                security.SiteGuid = siteGuid;

                var user = FMChannelHelper.MakeCall<IUsers, UserClass>(users => users.GetByID(security, userId));

                // set the login requrest properties
                var loginRequest = new SecurityLoginRequest();
                loginRequest.UserID = user.ID;
                loginRequest.Password = user.Password;
                loginRequest.SiteID = siteId;
                loginRequest.CACEnabled = false;
                loginRequest.TimeOut = 25;

                // after this call, the security object should be for a valid session.
				SecurityLoginResponse loginResponse = FMChannelHelper.MakeCall<ISites, SecurityLoginResponse>(
                    x => x.Login2(loginRequest));

	            string result = loginResponse.Result;

                if (result != null)
                {
                    throw new System.Security.SecurityException("User \"" + user.ID + "\" " + result);
                }
            }

            return FMChannelHelper.MakeCall<IReportingRequest, DataSet>(
                reportingRequests => reportingRequests.ProcessReport(security, parameters));
        }

        private void AddParameter(Dictionary<string, string> parameters, string nameValuePair)
        {
            if (string.IsNullOrEmpty(nameValuePair))
            {
                return;
            }

            string[] parts = nameValuePair.Split('=');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Parameter is invalid: " + nameValuePair);
            }

            string name = parts[0];
            string value = parts[1];

            parameters.Add(name, value);
        }

        protected string GetRequiredParameter(Dictionary<string, string> parameters, string name)
        {
            if (parameters.ContainsKey(name))
            {
                return parameters[name];
            }
            else
            {
                throw new ArgumentException("Report Parameter " + name + " is missing.");
            }
        }

        protected string GetOptionalParameter(Dictionary<string, string> parameters, string name)
        {
            return GetOptionalParameter(parameters, name, string.Empty);
        }

        protected string GetOptionalParameter(Dictionary<string, string> parameters, string name, string defaultValue)
        {
            if (parameters.ContainsKey(name))
            {
                return parameters[name];
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
