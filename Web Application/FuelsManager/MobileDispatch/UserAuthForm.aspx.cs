// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JFQCUserAuthForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the JFQCUserAuthForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Wingware
{
	using System;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using System.Web.UI;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMCore;
    using FuelsManager.FMWebApp;

    /// <summary>
    /// Allows a user to choose a product
    /// </summary>
    public partial class JFQCUserAuthForm : FMFormBase 
	{
		#region Methods
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			base.OnInit(e);
		}

		protected  void Page_Load(object sender, EventArgs e)
		{
        }
		protected async void Timer_Tick(object sender, EventArgs e)
		{
			Timer.Enabled = false;
			await GetJFQCWebsitePreAuthenticatedURL();
		}

		private async Task GetJFQCWebsitePreAuthenticatedURL()
        {
			string baseUrl = "https://api.jetfuelqc.com";
			string integration_uuid;
			string api_user;
			string api_password;
			string requested_url = "https://www.jetfuelqc.com/tickets";
			string integrationUrl;
			JFQCUserAuthenticationPayload payload;
			JFQCUserAuthenticationResp response;
            try
            {
                this.GetSecurity();

				//var integrationDO = FMChannelHelper.MakeCall<IWWIntegrationClass, WWIntegrationDO>(x => x.GetForSite(this.Security, new Guid("CBD3BB9D-ACBD-4EB0-91BB-86EC1DC2903F")));
				var integrationDO = FMChannelHelper.MakeCall<IWWIntegrationClass, WWIntegrationDO>(x => x.GetByIntegrationGuid(this.Security, new Guid("C2198D51-D964-11ED-88DA-020E416B1C13")));
				var user = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.GetByID(this.Security, this.Security.UserID));

                if (integrationDO != null || true)
                {
					integration_uuid = (integrationDO != null) ? integrationDO.IntegrationGuid.ToString() : new Guid("C2198D51-D964-11ED-88DA-020E416B1C13").ToString();
					api_user = (integrationDO != null) ? integrationDO.API_Username : "varec_test";
					api_password = (integrationDO != null) ?  integrationDO.API_Password : "7R4HV17Z0HGQgS9iSBDDD3uWJbSTgv1P";
					integrationUrl = "api/integration/" + integration_uuid + "/authenticate";

					string givenName = this.Security.UserID;

					if(user != null && user.Name.Trim().Length > 0)
                    {
						givenName = user.Name;
                    }

					payload = new JFQCUserAuthenticationPayload
					{
						username = this.Security.UserID,
						station = (integrationDO != null) ? integrationDO.StationIATACode : "DAL",
						vendor = (integrationDO != null) ? integrationDO.Vendor : "MZ1",
						name = givenName,
						requested_url = (integrationDO != null) ? integrationDO.RequestedURL : requested_url,
					};

					var client = GetHttpClient();
					client.BaseAddress = new Uri((integrationDO != null) ? integrationDO.BaseURL : baseUrl);
                    WebApi web = new WebApi(client)
                    {
                        UserName = api_user,
                        Password = api_password
                    };

                    try
					{
						response = await web.GetAuthenticatedURLAsync<JFQCUserAuthenticationResp>(integrationUrl, payload);

						if (response != null)
						{

							if (Convert.ToBoolean(response.success))
							{
								ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open Window", "setTimeout(function(){window.open('" + response.presigned_link + "','_newtab')}, 100);", true);

								resultSuccess.Visible = true;

								ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Close Window", "setTimeout(closeWindow, 5000);", true);
							}
							else
                            {
								lblError.Text = response.message;
								lblErrorCaption.Visible = true;
								lblError.Visible = true;
							}
						}
					}
					catch (HttpRequestException httpReqEx)
					{
						resultFailed.Visible = true;
						lblError.Text = httpReqEx.InnerException.Message.ToString();
						lblErrorCaption.Visible = true;
						lblError.Visible = true;
						this.ErrorHandler(httpReqEx);
					}

				}
				else
                {
					lblError.Text = "No integration exists for a given station & vendor combination.";
					lblErrorCaption.Visible = true;
					lblError.Visible = true;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		public HttpClient GetHttpClient()
		{
			HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            handler.SslProtocols = SslProtocols.Tls12;
            HttpClient client = new HttpClient(handler);
			client.DefaultRequestHeaders.Add("Accept", "application/json");
			return client;
		}

		#endregion
	}

	public class JFQCUserAuthenticationPayload
	{
		public string username;
		public string station;
		public string vendor;
		public string name;
		public string requested_url;
	}

	public class JFQCUserAuthenticationResp
	{
		public string success { get; set; } = "false";
		public string message { get; set; } = "Unauthorized";
		public string presigned_link { get; set; } = string.Empty;
	}

}