// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PIDXProfileForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for PIDXProfileForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Defines code behind for PIDX profile detail form
	/// </summary>
	public partial class PIDXProfileForm : FMFormBase
	{
		#region Public Methods and Operators

		/// <summary>
		/// Enables the controls on the page.
		/// </summary>
		/// <param name="enable">if set to <c>true</c> [enable].</param>
		public void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_PIDX_PROFILES))
{
				this.OK.Enabled = enable;
			}

			this.Cancel.Enabled = enable;
			this.tcPIDXProfileTabs.HeaderEnabled = enable;
		}

	/// <summary>
		/// Updates the data.
	/// </summary>
		public void UpdateData()
	{
			this.PIDXProfileGeneralPage.UpdateData();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					PIDXProfileClass pidxProfile;

					// Get IdentityGuid
					if (this.Session["IdentityGuid"] != null)
					{
						// Get PIDXProfile
						var identityGuid = this.Session["IdentityGuid"] as string;

						if ( string.IsNullOrEmpty(identityGuid) )
						{
							throw new ApplicationException("Error: did not find identify guid.");
					}

						pidxProfile = FMChannelHelper.MakeCall<IPIDXProfiles, PIDXProfileClass>(
																	 x =>
																	 x.Get(this.Security, Guid.Parse(identityGuid), true)
																);
					}
					else
					{
						pidxProfile = new PIDXProfileClass();
					}

					this.Session["PIDXProfile"] = pidxProfile;

					if (!this.Security.HasRight(RIGHT.MODIFY_PIDX_PROFILES))
					{
						// vthompson CSI 5773
						this.OK.Enabled = false;
				}
				}
				else
				{
					if (this.Session["PIDXProfile"] == null)
					{
						throw new Exception("PIDXProfile not in Session");
				}
			}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		/// <summary>
		/// Handles the Command event of the Cancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.Session.Remove("PIDXProfile");
			this.Redirect("PIDXProfilesForm.aspx");
		}

		/// <summary>
		/// Initialize events for controls.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
		}

		/// <summary>
		/// Handles the Command event of the OK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.UpdateData();

				var pidxProfile = this.Session["PIDXProfile"] as PIDXProfileClass;

				if (pidxProfile != null && pidxProfile.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IPIDXProfiles>(
																	 x =>
																	 x.Modify(this.Security, pidxProfile)
																);
				}
				else
				{
					FMChannelHelper.MakeCall<IPIDXProfiles, Guid>(
																	 x =>
																	 x.Add(this.Security, pidxProfile)
																);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Session.Remove("PIDXProfile");
			this.Redirect("PIDXProfilesForm.aspx");
			}

		#endregion
}
}
