// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReleaseToAccountingForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReleaseToAccountingForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	public partial class ReleaseToAccountingForm : FMFormBase
	{

		/// <summary>
		/// Show a message to the user on top of the form displaying the specified text.
		/// </summary>
		/// <param name="alertMessage">The message to show</param>
		public void ShowAlert(string alertMessage)
		{
			this.ClientScript.RegisterClientScriptBlock(this.GetType(), 
														"AlertScript", 
														"showAlertDialog('" + HttpUtility.JavaScriptStringEncode(alertMessage) + "');", 
														true);
		}

		/// <summary>
		/// Executes when the page is loaded.  Disables the command 
		/// buttons if security requirements are not satisfied.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
					{
						this.EnableControls(false);
					}

					this.closeButton.Attributes.Add("onclick", "return window.close();");


					SetCurrentSystemTime(true);

				}

				if (this.applyButton.Enabled)
				{
					this.applyButton.Focus();
				}
				else
				{
					this.closeButton.Focus();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#region Form Button Control Events

		protected void CurrentDateButtonOnClick(object sender, EventArgs e)
		{
			this.SetCurrentSystemTime(false);
		}

		/// <summary>
		/// Apply changes and display error message or operation result message.
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Not used</param>
		protected void ApplyButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				DateTime lockOutDateTime;
				bool validValue = DateTime.TryParse(this.lockOutDateInput.Value, out lockOutDateTime);

				// Set the default date time offset to EDST.
				// Should never use this.
				var timeSpanOffset = new TimeSpan(-4, 0, 0);

				if (string.IsNullOrEmpty(this.TimeOffsetField.Value) == false)
				{
					string strOffset = this.TimeOffsetField.Value.Trim();
					int offSet;

					if (int.TryParse(strOffset, out offSet))
					{
						// The offset is in minutes. Convert to hours.
						int hours = offSet / 60;
						timeSpanOffset = new TimeSpan(hours, 0, 0);
					}
				}

				DateTimeOffset lockOutDate;

				if (validValue == false)
				{
					lockOutDate = DateTimeOffset.Now;
					this.lockOutDateInput.Value = lockOutDate.LocalDateTime.ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					lockOutDate = new DateTimeOffset(lockOutDateTime, timeSpanOffset);					
				}

				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(this.Security, this.Security.SiteGuid));
				DateTimeOffset convertedLockOutDate = TimeConverter.ToSiteTime(site, lockOutDate);

				Dictionary<string, string> results = FMChannelHelper.MakeCall<IDispatchRequests, Dictionary<string, string>>(
					dispatchRequests => dispatchRequests.ReleaseToAccounting(this.Security, convertedLockOutDate));

				if (results.ContainsKey("OK"))
				{
					this.ShowAlert(results["OK"]);
				}
				
				if (results.ContainsKey("Failed"))
				{
					this.ShowAlert(results["Failed"]);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}


		private void SetCurrentSystemTime(bool setMidnight)
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.GetBasic(this.Security, this.Security.SiteGuid));
			DateTimeOffset time = TimeConverter.ToSiteTime(site, DateTimeOffset.Now);

			if (setMidnight)
			{
				time = new DateTimeOffset(time.Year, time.Month, time.Day, 23, 59, 59, time.Offset);
			}

			this.lockOutDateInput.Value = time.DateTime.ToString(CultureInfo.InvariantCulture);
			this.TimeOffsetField.Value = time.Offset.TotalMinutes.ToString(CultureInfo.InvariantCulture);

			string newDateDeclaration = "new Date(" + time.Year + "," + (time.Month - 1) + "," + time.Day + "," + time.Hour + ","
			                            + time.Minute + "," + time.Second + ",0)";

			Page.ClientScript.RegisterStartupScript(
				this.GetType(),
				"settime",
				"<script language='javascript'>initLockOutDateControl(" + newDateDeclaration + ")</script>");
		}


		/// <summary>
		/// Enables or disables the command buttons.
		/// </summary>
		/// <param name="enable">If true controls are enabled otherwise they are disabled.</param>
		private void EnableControls(bool enable)
		{
			this.useCurrentDateButton.Enabled = enable;
			this.applyButton.Enabled = enable;
		}
		#endregion
	}
}