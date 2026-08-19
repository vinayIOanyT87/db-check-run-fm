/******************************************************************************

	FILE NAME:		SiteLeakDetectionPage.ascx.cs


	PURPOSE:			Implementation of SiteLeakDetectionPage


	COMMENTS:

		Copyright (C) Varec, Inc.  All rights reserved.

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	P Reynolds


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------


*******************************************************************************/

namespace FuelsManager.FMWebApp
{
	using FMBusinessObjects.DataObjects;

	using System;
	using System.Web.UI.WebControls;

	/// <summary>
	///  Leak Detection Module set site setttings
	/// </summary>
	public partial class SiteLeakDetectionPage : FMUserControlBase
	{
		private const int MinNumberQuietTimeSamples = 1;
		private const int MaxNumberQuietTimeSamples = 10;
		private const int MinQuietTimeFactor = 1;
		private const int MaxQuietTimeFactor = 10;

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					InitializeOptions();
				}
				// Need to UpdateData on each post back becuse if Site is Login Site
				// other controls reformat based upon new settings
				else
					this.UpdateData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void InitializeOptions()
		{
			SiteClass site = (SiteClass)this.Session["Site"];

			NumberQuietTimeSamplesTextBox.Text = site.LeakDetectionMinQuietSamples.ToString();

			QuietTimeFactorTextBox.Text = site.LeakDetectionQuietTimeFactor.ToString();

			MinimumTotalQuietTimeTextbox.Text = site.LeakDetectionMinQuietTime.ToString();

			UseMinimumIssueWaitDropDownList.Items.Add(new ListItem(true.ToString(), true.ToString()));
			UseMinimumIssueWaitDropDownList.Items.Add(new ListItem(false.ToString(), false.ToString()));
			UseMinimumIssueWaitDropDownList.SelectedValue = site.LeakDetectionUseMinWait.ToString();

			NumberQuietTimeSamplesTextBox.Enabled = false;
			QuietTimeFactorTextBox.Enabled = false;
			MinimumTotalQuietTimeTextbox.Enabled = false;
			UseMinimumIssueWaitDropDownList.Enabled = false;
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public void UpdateData()
		{
			SiteClass site = (SiteClass)this.Session["Site"];

#if ContinuousAndRealTimeLeakDetectionImplemented
			site.LeakDetectionMinQuietSamples = GetInt(NumberQuietTimeSamplesTextBox.Text, NumberQuietTimeSamplesLabel.Text);
			site.LeakDetectionQuietTimeFactor = GetInt(QuietTimeFactorTextBox.Text, QuietTimeFactorLabel.Text);
			site.LeakDetectionMinQuietTime = GetInt(MinimumTotalQuietTimeTextbox.Text, MinimumTotalQuietTimeLabel.Text);
			site.LeakDetectionUseMinWait = GetBool(UseMinimumIssueWaitDropDownList.SelectedValue, UseMinimumIssueWaitLabel.Text);
#endif // ContinuousAndRealTimeLeakDetectionImplemented
		}

		private int GetInt(string textToParse, string name)
		{
			int value;
			if (int.TryParse(textToParse, out value))
			{
				return value;
			}
			throw new Exception("[" + name + "] is not a number");
		}

		private bool GetBool(string textToParse, string name)
		{
			bool value;
			if (bool.TryParse(textToParse, out value))
			{
				return value;
			}
			throw new Exception("[" + name + "] has invalid format");
		}
	}
}
