/******************************************************************************
	FILE NAME:		RegionalSettingsForm.aspx.cs
	PURPOSE:		Implementation of RegionalSettingsForm

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-07-12	Richard Panachida	Fixed the date format problem when the date format
												contains a "-" mircosoft will not allow the separator
												to change. It must be a "/" (CSI 3044)
 
		2006-11-14	Richard Panachida	Fixed the issue of the short date pattern not being set
												correctly (CSI 3385).

		2007-05-21  W.Gray            Revised to remove ShortDatePattern and TimePattern from
												session when changed (CSI 4635)
 
		2009-02-20  G.Kendall         WI#1662 - Allow editing at each site level.  Also include
												site name for clarity.
 
		2009-02-25  A. Coker          Updated code to refuse blank or empty Separator settings. Also, refuse
												same Date and Time Separators.
*******************************************************************************/

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Sockets;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	///		Summary description for RegionalSettingsForm.
	/// </summary>
	public partial class RegionalSettingsForm : FMFormBase, IMenuDiscovery
	{
		protected new SiteClass Site;
		protected const string Msg1 = "Separator must not be empty or blank.";
		protected const string Msg2 = "Date Separator and Time Separator must be different.";

		protected void PopulateDecimalSymbolDropDownList(SiteClass site)
		{
			string[] decimalSymbols = { ".", "," };
			int index = 0;
			foreach (string decimalSymbol in decimalSymbols)
			{
				ListItem decimalSymbolItem = new ListItem(decimalSymbol, index.ToString());
				this.DecimalSymbolDropDownList.Items.Add(decimalSymbolItem);
				if (decimalSymbol == site.NumberDecimalSeparator)
					this.DecimalSymbolDropDownList.SelectedIndex = this.DecimalSymbolDropDownList.Items.Count - 1;
				index++;
			}
		}

		protected void PopulateDigitGroupingSymbolDropDownList(SiteClass site)
		{
			string[] digitGroupingSymbols = { ".", "," };
			int index = 0;
			foreach (string digitGroupingSymbol in digitGroupingSymbols)
			{
				ListItem groupSymbolItem = new ListItem(digitGroupingSymbol, index.ToString());
				this.DigitGroupingSymbolDropDownList.Items.Add(groupSymbolItem);
				if (digitGroupingSymbol == site.NumberGroupSeparator)
					this.DigitGroupingSymbolDropDownList.SelectedIndex = this.DigitGroupingSymbolDropDownList.Items.Count - 1;
				index++;
			}
		}

		protected void PopulateDigitGroupingDropDownList(SiteClass site)
		{
			this.DigitGroupingDropDownList.Items.Clear();

			string[] digitGroupings = new string[3];

			digitGroupings[0] = "123456789";
			if (site.NumberGroupSeparator == ",")
			{
				digitGroupings[1] = "123,456,789";
				digitGroupings[2] = "12,34,56,789";
			}
			else
			{
				digitGroupings[1] = "123.456.789";
				digitGroupings[2] = "12.34.56.789";
			}

			int index = 0;
			foreach (string digitGrouping in digitGroupings)
			{
				ListItem groupingItem = new ListItem(digitGrouping, index.ToString());
				this.DigitGroupingDropDownList.Items.Add(groupingItem);
				if (index == (int)site.NumberGroupSizesType)
					this.DigitGroupingDropDownList.SelectedIndex = this.DigitGroupingDropDownList.Items.Count - 1;
				index++;
			}
		}

		private void PopulateTimePatternDropDownList(SiteClass site)
		{
			this.TimePatternDropDownList.Items.Clear();
			string[] timePatterns ={"h:mm:ss tt",
											"hh:mm:ss tt",
											"h:mm tt",
											"hh:mm tt",
											"H:mm:ss",
											"HH:mm:ss",
											"H:mm",
											"HH:mm"};

			int index = 0;
			foreach (string timePattern in timePatterns)
			{
				ListItem timePatternItem = new ListItem(timePattern, index.ToString());
				this.TimePatternDropDownList.Items.Add(timePatternItem);
				if (timePattern == site.TimePattern)
					this.TimePatternDropDownList.SelectedIndex = this.TimePatternDropDownList.Items.Count - 1;
				index++;
			}
		}

		/// <summary>
		/// This method will create the short date pattern entries for the dropdown list.
		/// </summary>
		/// <param name="site"></param>
		private void PopulateShortDatePatternDropDownList(SiteClass site)
		{
			this.ShortDatePatternDropDownList.Items.Clear();
			string[] datePatterns = {	"M/d/yyyy",
												"M/d/yy",
												"MM/dd/yy",
												"MM/dd/yyyy",
												"yy/MM/dd",
												"yyyy-MM-dd",
												"dd-MMM-yy",
												"dd-MM-yy"
											 };

			int index = 0;
			foreach (string datePattern in datePatterns)
			{
				ListItem datePatternItem = new ListItem(datePattern, index.ToString());
				this.ShortDatePatternDropDownList.Items.Add(datePatternItem);

				// Ensure that the patterns match by using the same delimiters.
				string sitePattern = site.ShortDatePattern.Replace("/", "-");
				string currentPattern = datePattern.Replace("/", "-");

				if (currentPattern == sitePattern)
				{
					this.ShortDatePatternDropDownList.SelectedIndex = this.ShortDatePatternDropDownList.Items.Count - 1;
				}

				index++;
			}
		}

		private void PopulateLongDatePatternDropDownList(SiteClass site)
		{
			this.LongDatePatternDropDownList.Items.Clear();
			string[] datePatterns ={"dddd, MMMM dd,yyyy",
											"MMMM dd, yyyy",
											"dddd, dd MMMM, yyyy",
											"dd MMMM, yyyy"
										  };

			int index = 0;
			foreach (string datePattern in datePatterns)
			{
				ListItem datePatternItem = new ListItem(datePattern, index.ToString());
				this.LongDatePatternDropDownList.Items.Add(datePatternItem);
				if (datePattern == site.LongDatePattern)
					this.LongDatePatternDropDownList.SelectedIndex = this.LongDatePatternDropDownList.Items.Count - 1;
				index++;
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.Site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, getMemberSites: true, getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true)
																);
				if (!this.Page.IsPostBack)
				{
					this.ConfigurationLabel.Text = this.GetTranslatedText("Regional Settings Configuration") + " - " + this.Site.ID;

					this.PopulateDecimalSymbolDropDownList(this.Site);
					this.PopulateDigitGroupingSymbolDropDownList(this.Site);
					this.PopulateDigitGroupingDropDownList(this.Site);
					this.ListSeparatorTextBox.Text = this.Site.ListSeparator;
					this.UpdateSampleNumberFormats(this.Site);
					this.PopulateTimePatternDropDownList(this.Site);
					this.TimeSeparatorTextBox.Text = this.Site.TimeSeparator;
					this.AMSymbolTextBox.Text = this.Site.AMSymbol;
					this.PMSymbolTextBox.Text = this.Site.PMSymbol;
					this.UpdateSampleTimeFormat(this.Site);
					this.PopulateShortDatePatternDropDownList(this.Site);
					this.DateSeparatorTextBox.Text = this.Site.DateSeparator;
					this.UpdateSampleShortDateFormat(this.Site);
					this.PopulateLongDatePatternDropDownList(this.Site);
					this.UpdateSampleLongDateFormat(this.Site);
					this.FourDigitCalendarEndYearTextBox.Text = this.Site.TwoDigitCalendarEndYear.ToString("D4");

					if (this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) == false)
					{
						this.DigitGroupingSymbolDropDownList.Enabled = false;
						this.DigitGroupingDropDownList.Enabled = false;
						this.DecimalSymbolDropDownList.Enabled = false;
						this.ListSeparatorTextBox.Enabled = false;
						this.TimePatternDropDownList.Enabled = false;
						this.TimeSeparatorTextBox.Enabled = false;
						this.AMSymbolTextBox.Enabled = false;
						this.PMSymbolTextBox.Enabled = false;
						this.DateSeparatorTextBox.Enabled = false;
						this.FourDigitCalendarEndYearTextBox.Enabled = false;
						this.ShortDatePatternDropDownList.Enabled = false;
						this.LongDatePatternDropDownList.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void DecimalSymbolDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Site.NumberDecimalSeparator = this.DecimalSymbolDropDownList.SelectedItem.Text;

				if (this.Site.NumberDecimalSeparator == ".")
				{
					this.Site.NumberGroupSeparator = ",";
					this.DigitGroupingSymbolDropDownList.SelectedIndex = 1;
				}
				else
				{
					this.Site.NumberGroupSeparator = ".";
					this.DigitGroupingSymbolDropDownList.SelectedIndex = 0;
				}

				this.PopulateDigitGroupingDropDownList(this.Site);

				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
				this.UpdateSampleNumberFormats(this.Site);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DigitGroupingSymbolDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Site.NumberGroupSeparator = this.DigitGroupingSymbolDropDownList.SelectedItem.Text;

				if (this.Site.NumberGroupSeparator == ".")
				{
					this.Site.NumberDecimalSeparator = ",";
					this.DecimalSymbolDropDownList.SelectedIndex = 1;
				}
				else
				{
					this.Site.NumberDecimalSeparator = ".";
					this.DecimalSymbolDropDownList.SelectedIndex = 0;
				}

				this.PopulateDigitGroupingDropDownList(this.Site);

				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);

				this.UpdateSampleNumberFormats(this.Site);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DigitGroupingDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Site.NumberGroupSizesType = (NUMBER_GROUP_SIZES_TYPE)Convert.ToInt32(this.DigitGroupingDropDownList.SelectedItem.Value);
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
				this.UpdateSampleNumberFormats(this.Site);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


		private void UpdateSampleNumberFormats(SiteClass site)
		{
			NumberFormatInfo format = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);

			format.NumberDecimalDigits = 2;
			SIDouble integerSampleValue = new SIDouble(EngineeringUnit.FmvMeter3, format, 0) { Value = 123456789.0 };
			this.SampleNumberFormat1TextBox.Text = integerSampleValue.ToString();

			format.NumberDecimalDigits = 3;
			SIDouble decimalSampleValue = new SIDouble(EngineeringUnit.FmvMeter3, format, 0) { Value = .123 };
			this.SampleNumberFormat2TextBox.Text = decimalSampleValue.ToString();
		}

		protected void ListSeparatorTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Site.ListSeparator = this.ListSeparatorTextBox.Text;
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TimePatternDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				bool changeFromToMilitary = (this.Site.TimePattern.IndexOf("H", StringComparison.Ordinal) == -1
				&& this.TimePatternDropDownList.SelectedItem.Text.IndexOf("H", StringComparison.Ordinal) != -1)
				|| (this.Site.TimePattern.IndexOf("H", StringComparison.Ordinal) != -1
				&& this.TimePatternDropDownList.SelectedItem.Text.IndexOf("H", StringComparison.Ordinal) == -1);

				this.Site.TimePattern = this.TimePatternDropDownList.SelectedItem.Text;
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
				if (changeFromToMilitary && UsingLoadRack)
				{
					try
					{
						ILoadRackManager loadRackManager = this.GetLoadRackManager();
						loadRackManager.Modify(this.Security, typeof(SiteClass), this.Site.IdentityGuid);
					}
					catch (SocketException socketExcept)
					{
						if (socketExcept.ErrorCode != 10061)
							throw;
					}
				}


				this.UpdateSampleTimeFormat(this.Site);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TimeSeparatorTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.TimeSeparatorTextBox.Text.Trim().Length == 0)
				{
					this.TimeSeparatorTextBox.Text = this.Site.TimeSeparator;
					throw new Exception(Msg1);
				}
				if (this.DateSeparatorTextBox.Text.Trim() == this.TimeSeparatorTextBox.Text.Trim())
				{
					this.TimeSeparatorTextBox.Text = this.Site.TimeSeparator;
					throw new Exception(Msg2);
				}

				if (this.TimeSeparatorTextBox.Text.Equals(";"))
				{
					throw new ApplicationException("Semi-colon not supported for separator");
				}

				this.Site.TimeSeparator = this.TimeSeparatorTextBox.Text;
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
				this.UpdateSampleTimeFormat(this.Site);
			}
			catch (ApplicationException except)
			{
				this.ErrorHandler(except);
				this.TimeSeparatorTextBox.Focus();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void AmSymbolTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Site.AMSymbol = this.AMSymbolTextBox.Text;
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
				this.UpdateSampleTimeFormat(this.Site);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PmSymbolTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Site.PMSymbol = this.PMSymbolTextBox.Text;
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
				this.UpdateSampleTimeFormat(this.Site);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateSampleTimeFormat(SiteClass site)
		{
			this.Session.Remove("TimePatern");
			this.SampleTimeFormatTextBox.Text = DateTimeOffset.Now.ToString("t", site.GetDateTimeFormatInfo());
		}

		protected void ShortDatePatternDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.UpdateShortDateFormat();

				this.UpdateSampleShortDateFormat(this.Site);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UpdateShortDateFormat()
		{
			try
			{
				string dateFormatStr = this.ShortDatePatternDropDownList.SelectedItem.Text;
				// update the short date format with the selected date seperator
				if (dateFormatStr.IndexOf("-", StringComparison.Ordinal) > 0 && this.Site.DateSeparator.Length == 1)
					this.Site.ShortDatePattern = dateFormatStr.Replace("-", this.Site.DateSeparator);
				else if (dateFormatStr.IndexOf("/", StringComparison.Ordinal) > 0 && this.Site.DateSeparator.Length == 1)
					this.Site.ShortDatePattern = dateFormatStr.Replace("/", this.Site.DateSeparator);
				else
					this.Site.ShortDatePattern = dateFormatStr;

				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void DateSeparatorTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.DateSeparatorTextBox.Text.Trim().Length == 0)
				{
					this.DateSeparatorTextBox.Text = this.Site.DateSeparator;
					throw new Exception(Msg1);
				}
				if (this.DateSeparatorTextBox.Text.Trim() == this.TimeSeparatorTextBox.Text.Trim())
				{
					this.DateSeparatorTextBox.Text = this.Site.DateSeparator;
					throw new Exception(Msg2);
				}

				if (this.DateSeparatorTextBox.Text.Equals(";"))
				{
					throw new ApplicationException("Semi-colon not supported for separator");
				}

				this.Site.DateSeparator = this.DateSeparatorTextBox.Text;
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
				this.UpdateShortDateFormat();
				this.UpdateSampleShortDateFormat(this.Site);
			}
			catch (ApplicationException except)
			{
				this.ErrorHandler(except);
				this.DateSeparatorTextBox.Focus();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateSampleShortDateFormat(SiteClass site)
		{
			this.Session.Remove("ShortDatePattern");
			this.SampleShortDateTextBox.Text = DateTimeOffset.Now.ToString("d", site.GetDateTimeFormatInfo());
		}


		protected void LongDatePatternDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Site.LongDatePattern = this.LongDatePatternDropDownList.SelectedItem.Text;
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
				this.UpdateSampleLongDateFormat(this.Site);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateSampleLongDateFormat(SiteClass site)
		{
			this.SampleLongDateTextBox.Text = DateTimeOffset.Now.ToString("D", site.GetDateTimeFormatInfo());
		}

		protected void FourDigitCalendarEndYearTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				if (this.FourDigitCalendarEndYearTextBox.Text.Trim().Length != 4)
				{
					this.FourDigitCalendarEndYearTextBox.Focus();
					throw new Exception("4 digit number required.");
				}
				this.Site.TwoDigitCalendarEndYear = Convert.ToInt32(this.FourDigitCalendarEndYearTextBox.Text);
				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Site, updateDocumentNumbers: true)
																);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                    return null;
            }
            List<FMMenuItem> items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
				&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				return null;

			items.Add(new FMMenuItem {
				MenuItemType = FMMenuItemType.ADMIN_SYSTEM_REGIONAL_SETTINGS,
				RootMenuName = "Administration",
				CategoryName = "System",
				ItemName = "Regional Settings",
				NavigateUrl = "RegionalSettingsForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			});

			return items;

		}
	}
}
