namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using FMBusinessObjects.DataObjects;

    public partial class SiteOpcUaPage : FMUserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SiteClass site = (SiteClass)this.Session["Site"];

                if (!this.Page.IsPostBack)
                {
                    this.ServerEndPointTextBox.Text = site.ServerEndPoint;
                    this.LoadSecurityModes(site);
                    this.LoadSecurityPolicies(site);
                    this.LoadMessageEncoding(site);
                    this.LoadUserIdentityMethods(site);
                    this.UserNameOrCertificateTextBox.Text = (site.UserIdentityMethod == "Certificate") ? site.UserCertificatePath : site.UserId;
                    this.PasswordTextBox.Attributes["value"] = site.UserPassword;
                }
                else
                {
                    site.SecurityMode = this.SecurityModeDropDownList.SelectedValue;
                    site.SecurityPolicy = this.SecurityPolicyDropDownList.SelectedValue;
                    this.LoadSecurityPolicies(site);
                    site.UserIdentityMethod = this.UserIdentityMethodDropDownList.SelectedValue;
                    this.PasswordTextBox.Attributes["value"] = this.PasswordTextBox.Text;
                }

                site.ServerEndPoint = this.ServerEndPointTextBox.Text;
                this.ServerEndPointTextBox.ToolTip = site.ServerEndPoint;
                this.UserIdLabel.Text = (site.UserIdentityMethod == "Certificate") ? "Certificate Path" : "User Name";
                this.UserNameOrCertificateTextBox.ToolTip = this.UserIdLabel.Text;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        public void UpdateData()
        {
            SiteClass site = (SiteClass)this.Session["Site"];
            if (!site.Enterprise)
            {
                site.MessageEncoding = this.MessageEncodingDropDownList.SelectedValue;

                if (site.UserIdentityMethod == "Certificate")
                {
                    site.UserId = "";
                    site.UserCertificatePath = this.UserNameOrCertificateTextBox.Text;
                }
                else
                {
                    site.UserId = this.UserNameOrCertificateTextBox.Text;
                    site.UserCertificatePath = "";
                }

                site.UserPassword = this.PasswordTextBox.Text;
            }
         }


        private void LoadSecurityModes(SiteClass site)
        {
            string[] securityModes = { "None", "Sign", "SignAndEncrypt" };

            foreach (var securityMode in securityModes)
            {
                var item = new ListItem(securityMode, securityMode);
                this.SecurityModeDropDownList.Items.Add(item);

                if (securityMode.Equals(site.SecurityMode))
                {
                    this.SecurityModeDropDownList.SelectedIndex = this.SecurityModeDropDownList.Items.Count - 1;
                }
            }
        }

        private void LoadSecurityPolicies(SiteClass site)
        {
            string[] securityPolicies = { "None", "Basic256", "Basic128Rsa15", "Basic256Sha256","Aes_128_Sha256_RsaOaep", "Aes256_Sha256_RsaPss" };

            this.SecurityPolicyDropDownList.Items.Clear();

            foreach (var securityPolicy in securityPolicies)
            {
                if(site.SecurityMode == "None"
                && securityPolicy != "None")
                {
                    continue;
                }

                if (site.SecurityMode != "None"
                && securityPolicy == "None")
                {
                    continue;
                }

                var item = new ListItem(securityPolicy, securityPolicy);

                this.SecurityPolicyDropDownList.Items.Add(item);

                if (securityPolicy.Equals(site.SecurityPolicy))
                {
                    this.SecurityPolicyDropDownList.SelectedIndex = this.SecurityPolicyDropDownList.Items.Count - 1;
                }
            }
        }

        private void LoadMessageEncoding(SiteClass site)
        {
            string[] messageEncodings = { "Binary", "Xml" };

            foreach (var messageEncoding in messageEncodings)
            {
                var item = new ListItem(messageEncoding, messageEncoding);
                this.MessageEncodingDropDownList.Items.Add(item);

                if (messageEncoding.Equals(site.MessageEncoding))
                {
                    this.MessageEncodingDropDownList.SelectedIndex = this.MessageEncodingDropDownList.Items.Count - 1;
                }
            }
        }

        private void LoadUserIdentityMethods(SiteClass site)
        {
            string[] userIdentityMethods = { "Anonymous", "UserName", "Certificate" };

            foreach (var userIdentityMethod in userIdentityMethods)
            {
                var item = new ListItem(userIdentityMethod, userIdentityMethod);
                this.UserIdentityMethodDropDownList.Items.Add(item);

                if (userIdentityMethod.Equals(site.UserIdentityMethod))
                {
                    this.UserIdentityMethodDropDownList.SelectedIndex = this.UserIdentityMethodDropDownList.Items.Count - 1;
                }
            }
        }
    }
}