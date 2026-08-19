// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountingWebFormView.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AccountingWebFormView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMWebApp;

    public class AccountingWebFormView : FMFormBase
	{
		#region Constants and Fields
		protected SecurityClass security;
		private Guid currentSiteGuid;
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the current site guid that was retrieved from the cookie.  The default is site admin.
		/// </summary>
		public Guid CurrentSiteGuid
		{
			get { return this.currentSiteGuid; }
			set { this.currentSiteGuid = value; }
		}
		#endregion

		#region Public Methods and Operators
		public void DisplayErrorPage()
		{
			this.Redirect("/Error.aspx");
		}

		/// <summary>
		/// This method will return true if the hardware key is defense.
		/// </summary>
		/// <returns>Returns turn if defense key.</returns>
		public bool IsBsme
		{
			get
			{
				return FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDefenseKey())
					   || FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescEnterpriseKey())
					   || FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey())
					   || FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescProfessionalKey());
			}
		}

		/// <summary>
		/// This method will initialize the security and data dictionary classes.
		/// </summary>
		public void Initialize()
		{
			try
			{
				this.security = this.Session["Security"] as SecurityClass;
				if (this.security == null)
				{
					// this should only happen on a session timeout where the security object has been deleted
					throw new FMSessionInvalidException();
				}

				this.Session.Add("SiteGuid", this.security.SiteGuid);
				this.currentSiteGuid = this.security.SiteGuid;
			}
			catch (COMException ex)
			{
				this.ErrorHandler(ex);
			}
			catch (FMSessionInvalidException except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Overrides the OnInit event
		/// </summary>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);

			base.OnInit(e);
		}
		#endregion
	}

	// New class.
	public class AccountingAutoSubmitWebFormView : AccountingWebFormView
	{
		#region Methods
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		private void AccountingAutoSubmitWebFormViewLoad(object sender, EventArgs e)
		{
			if (this.IsPostBack == false)
			{
				// This is a simpler, more reliable, cross-browser way of setting the
				// default button that avoids writing javascript
				if (this.FindControl("OK") is Button)
				{
					this.Form.DefaultButton = "OK";
				}
				else if (this.FindControl("OKButton") is Button)
				{
					this.Form.DefaultButton = "OKButton";
				}
				else if (this.FindControl("FindBtn") is Button)
				{
					this.Form.DefaultButton = "FindBtn";
				}
				else if (this.FindControl("RefreshButton") is Button)
				{
					this.Form.DefaultButton = "RefreshButton";
				}
				else if (this.FindControl("AddButton") is Button)
				{
					this.Form.DefaultButton = "AddButton";
				}
				else if (this.FindControl("SaveButton") is Button)
				{
					this.Form.DefaultButton = "SaveButton";
				}
				else if (this.FindControl("CloseButton") is Button)
				{
					this.Form.DefaultButton = "CloseButton";
				}
			}
		}

		private void InitializeComponent()
		{
			this.Load += this.AccountingAutoSubmitWebFormViewLoad;
		}
		#endregion
	}

	// New class.
	public class AccountingAutoSubmitWebFormViewAjax : AccountingAutoSubmitWebFormView
	{
		#region Properties
		private UpdatePanel ActiveUpdatePanel
		{
			get
			{
				try
				{
					ScriptManager currentScriptManager = ScriptManager.GetCurrent(this);

					if (currentScriptManager != null)
					{
						Control activeControl = currentScriptManager.FindControl(currentScriptManager.AsyncPostBackSourceElementID);

						while (activeControl?.Parent != null)
						{
							if (activeControl.Parent.GetType() == typeof(UpdatePanel))
							{
								return (UpdatePanel)activeControl.Parent;
							}

							activeControl = activeControl.Parent;
						}
					}

					return null;
				}
				catch
				{
					return null;
				}
			}
		}
		#endregion

		#region Methods
		protected override void RedirectAfterSessionTimeout()
		{
			UpdatePanel activeUpdatePanel = this.ActiveUpdatePanel;

			if (activeUpdatePanel != null)
			{
				var messageGuid = Guid.NewGuid();
				const string SMessage = "window.top.location=\"../FMWebApp/LogoutForm.aspx\";";

				ScriptManager.RegisterStartupScript(
					activeUpdatePanel, activeUpdatePanel.GetType(), messageGuid.ToString(), SMessage, true);
			}
			else
			{
				this.Response.Flush();
				base.RedirectAfterSessionTimeout();
			}
		}

		protected override void RenderErrorMessage(string errorMessage)
		{
			UpdatePanel activeUpdatePanel = this.ActiveUpdatePanel;

			if (activeUpdatePanel != null)
			{
				Guid gMessage = Guid.NewGuid();
				string sMessage = "alert(\"" + HttpUtility.JavaScriptStringEncode(errorMessage) + "\");";

				ScriptManager.RegisterStartupScript(
					activeUpdatePanel, activeUpdatePanel.GetType(), gMessage.ToString(), sMessage, true);
			}
			else
			{
				base.RenderErrorMessage(errorMessage);
			}
		}
		#endregion
	}
}