/* ***************************************************************************
	FILE NAME:	FMDate.cs
	PURPOSE:	Implementation of: FMDate

	COMMENTS:
	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
	This file shall not be copied or reproduced in any form without
	the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
	Date:	By:			Reason:
	----------	-----------------	-------------------------------------------
	2007-03-08	Richard Panachida	Fixed the problem with a blank date or a date with invalid
					values. In addition, set the calendar background color to white
					to ensure any controls underneath are hidden. Fixed all the different
					regional issues and the spinning.

	2007-04-05	W.GRay		Changed back to support null date (CSI 4326)
	2007-09-21	Richard Panachida	Added a DateTime property that will return the date in the System.DateTime
					object type.
	2007-10-31	Richard Panachida	Corrected year text box to accept the correct digits (CSI 5327).
	
	2007-11-23  E. Simmons		Updated DateTime Property to resolve CSI #5382.  The FMDate.text
										attribute was blank.  This attribute is only set when a call
										is made to either the GET or SET function of the FMDate.Text property.
	2007-12-04	Richard Panachida	Added a JS function to handle updating the transaction status if the date time
											control on the transaction detail page is the scheduled date time and
											the transactions status that is selected is "Requested" (CSI 5209). In addition,
											if the calendar display button is pressed, update the status.
	2008-10-02	E. Simmons		Added Set Operation to DateTime Property to support CSI #6153.
	2008-11-10  A. Coker            Changed date and time formats to honor regional settings. (Task ID 189)
	2009-10-20  A. Coker       WI 7639 - Use z-index if one provided in style attribute.
	2009-10-24	W.Gray			Revised Text Property to not throw if value is "".  Any other value that doesn't
										parse will throw.  WI 8661
*******************************************************************************/

namespace FMControls
{
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Summary description for FMDate.
	/// </summary>
	[DefaultProperty("Text"),
	ToolboxData("<{0}:FMDate runat=server></{0}:FmDate>")]
	public class FMDate : WebControl
	{
		#region Private Attributes
		private string text = "";
		private string[] MonthAbrv = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
		private DateTimeOffset currentDate;
		#endregion

		#region Protected Attributes
		protected TextBox MonthTextBox = new TextBox();
		protected Label Separator1Label = new Label();
		protected TextBox DayTextBox = new TextBox();
		protected Label Separator2Label = new Label();
		protected TextBox YearTextBox = new TextBox();
		protected FMCalendarSetLinkButton SetButton = new FMCalendarSetLinkButton();
		protected System.Web.UI.WebControls.Calendar calendar = new System.Web.UI.WebControls.Calendar();

		protected bool bUseDataDictionary = false;

		protected Guid SiteGuid = Guid.Empty;

		#endregion

		#region Contructors
		/// <summary>
		/// This is the default constructor for class FMDate.
		/// </summary>
		public FMDate()
		{
			IsStandard = true;
		}
		#endregion

		#region Properties

		/// <summary>
		/// Standard Date is used in the transaction.  It also includes a big chunk of javascript.
		/// This is set so the javascript is not included unless it's needed.
		/// Also, it provides callback for the page to customize event handling.
		/// </summary>
		public bool IsStandard { get; set; }

		public override string CssClass
		{
			get
			{
				return base.CssClass;
			}
			set
			{
				base.CssClass = value;
				MonthTextBox.CssClass = value;
				Separator1Label.CssClass = value;
				DayTextBox.CssClass = value;
				Separator2Label.CssClass = value;
				YearTextBox.CssClass = value;
				SetButton.CssClass = value;
				calendar.CssClass = value;
			}
		}


		public System.Web.UI.WebControls.Calendar Calendar
		{
			get
			{
				return calendar;
			}
		}

		private void SetTextBoxControls()
		{
			// since the regional options are applied we need to reformat the expected string with the characters we are looking for
			// DateSeparator

			

			string shortDatePattern = ShortDatePattern.Replace(DateSeparator, "/");

			if (shortDatePattern.Contains("yyyy"))
			{
				YearTextBox.MaxLength = 4;
			}
			else
			{
				YearTextBox.MaxLength = 2;
			}

			if (shortDatePattern == "M/d/yyyy")
			{
				MonthTextBox.Text = currentDate.Month.ToString();
				DayTextBox.Text = currentDate.Day.ToString();
				YearTextBox.Text = currentDate.Year.ToString("D04");

				// Reset the text property to a valid date.
				this.text = this.MonthTextBox.Text + "/" + this.DayTextBox.Text + "/" + this.YearTextBox.Text;
			}
			else if (shortDatePattern == "M/d/yy")
			{
				MonthTextBox.Text = currentDate.Month.ToString();
				DayTextBox.Text = currentDate.Day.ToString();
				YearTextBox.Text = (currentDate.Year % 100).ToString("D02");

				// Reset the text property to a valid date.
				this.text = this.MonthTextBox.Text + "/" + this.DayTextBox.Text + "/" + this.YearTextBox.Text;
			}

			else if (shortDatePattern == "MM/dd/yy")
			{
				MonthTextBox.Text = currentDate.Month.ToString("D02");
				DayTextBox.Text = currentDate.Day.ToString("D02");
				YearTextBox.Text = (currentDate.Year % 100).ToString("D02");

				// Reset the text property to a valid date.
				this.text = this.MonthTextBox.Text + "/" + this.DayTextBox.Text + "/" + this.YearTextBox.Text;
			}

			else if (shortDatePattern == "MM/dd/yyyy")
			{
				MonthTextBox.Text = currentDate.Month.ToString("D02");
				DayTextBox.Text = currentDate.Day.ToString("D02");
				YearTextBox.Text = currentDate.Year.ToString("D04");

				// Reset the text property to a valid date.
				this.text = this.MonthTextBox.Text + "/" + this.DayTextBox.Text + "/" + this.YearTextBox.Text;
			}

			else if ((shortDatePattern == "yy/MM/dd") || (shortDatePattern == "dd/MM/yy"))
			{
				MonthTextBox.Text = currentDate.Month.ToString("D02");
				DayTextBox.Text = currentDate.Day.ToString("D02");
				YearTextBox.Text = (currentDate.Year % 100).ToString("D02");

				if (shortDatePattern == "yy/MM/dd")
				{
					// Reset the text property to a valid date.
					this.text = this.YearTextBox.Text + "/" + this.MonthTextBox.Text + "/" + this.DayTextBox.Text;
				}
				else
				{
					// Reset the text property to a valid date.
					this.text = this.DayTextBox.Text + "/" + this.MonthTextBox.Text + "/" + this.YearTextBox.Text;
				}
			}

			else if (shortDatePattern == "yyyy/MM/dd")
			{
				MonthTextBox.Text = currentDate.Month.ToString("D02");
				DayTextBox.Text = currentDate.Day.ToString("D02");
				YearTextBox.Text = (currentDate.Year).ToString("D04");

				// Reset the text property to a valid date.
				this.text = this.YearTextBox.Text + "/" + this.MonthTextBox.Text + "/" + this.DayTextBox.Text;
			}

			else if (shortDatePattern == "dd/MMM/yy")
			{
				MonthTextBox.Text = MonthAbrv[currentDate.Month - 1];
				DayTextBox.Text = currentDate.Day.ToString("D02");
				YearTextBox.Text = (currentDate.Year % 100).ToString("D02");

				// Reset the text property to a valid date.
				this.text = this.DayTextBox.Text + "/" + this.MonthTextBox.Text + "/" + this.YearTextBox.Text;
			}
		}

		public DateTime DateTimeValue
		{
			get
			{
				DateTimeOffset dt = CurrentValue;
				return  new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
			}
		}

		/// <summary>
		/// This property will return the date that the text fields are
		/// set to. It will return the today's date if an exception happens.
		/// </summary>
		public DateTimeOffset CurrentValue
		{
			get
			{
				if (Page.Session["Security"] == null)
				{
					throw new ArgumentNullException("Security");
				}
				var security = Page.Session["Security"] as SecurityClass;

				if (security == null)
				{
					throw new ArgumentNullException("Security");
				}

				var currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																x =>
																x.Get(security, security.SiteGuid, false, false, false));

				try
				{

					currentDate = new DateTimeOffset(DateTime.Parse(this.Text, FormatInfo, DateTimeStyles.None).Date, TimeConverter.Today(currentSite).Offset);
					
				}
				catch
				{
					try
					{                                                                
						currentDate = TimeConverter.Today(currentSite).Date;
					}
					catch
					{
						currentDate = TimeConverter.Today().Date;
					}
				}

				return currentDate;
			}

			set
			{
				try
				{
					currentDate = value.Date;
					SetTextBoxControls();
				}
				catch (Exception)
				{
					MonthTextBox.Text = "";
					DayTextBox.Text = "";
					YearTextBox.Text = "";
					this.text = "";
				}
			}
		}

		public DateTimeFormatInfo FormatInfo
		{
			get
			{
				DateTimeFormatInfo formatInfo = null;

				try
				{
					formatInfo = ViewState["FormatInfo"] as DateTimeFormatInfo;
					
					//Use site regional settings if formatting attributes are not set
					if (formatInfo == null)
					{
						if (!DesignMode && Page.Session["Token"] != null)
						{
							if (Page.Session["Security"] == null)
							{
								throw new ArgumentNullException("Security");
							}

							var security = Page.Session["Security"] as SecurityClass;

							if (security == null) throw new ArgumentNullException("Security");

							SiteClass currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(security, security.SiteGuid, false, false, false)
																);


							formatInfo = currentSite.GetDateTimeFormatInfo();
							ViewState["FormatInfo"] = formatInfo;
						}

						if (formatInfo == null)
						{
							if (DateTimeFormatInfo.CurrentInfo != null)
							{
								formatInfo = new DateTimeFormatInfo
								             {
									             ShortDatePattern = DateTimeFormatInfo.CurrentInfo.ShortDatePattern,
									             DateSeparator = DateTimeFormatInfo.CurrentInfo.DateSeparator
								             };
							}

							ViewState["FormatInfo"] = formatInfo;
						}
					}
				}
				catch
				{
					if (DateTimeFormatInfo.CurrentInfo != null)
					{
						formatInfo = new DateTimeFormatInfo
						             {
							             ShortDatePattern = DateTimeFormatInfo.CurrentInfo.ShortDatePattern,
							             DateSeparator = DateTimeFormatInfo.CurrentInfo.DateSeparator
						             };
					}

					ViewState["FormatInfo"] = formatInfo;
				}

				return formatInfo;
			}

			set
			{
				ViewState["FormatInfo"] = value;
			}
		}

		public string ShortDatePattern
		{
			get
			{

				return FormatInfo.ShortDatePattern;
			}
		}

		public string DateSeparator
		{
			get
			{
				return FormatInfo.DateSeparator;

			}
		}

		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public string Text
		{
			get
			{
				if (MonthTextBox.Text == ""
				|| DayTextBox.Text == ""
				|| YearTextBox.Text == "")
					return "";

				string shortDatePattern = ShortDatePattern.Replace(DateSeparator, "/");

				if (shortDatePattern == "M/d/yyyy"
				|| shortDatePattern == "M/d/yy"
				|| shortDatePattern == "MM/dd/yy"
				|| shortDatePattern == "MM/dd/yyyy")
				{
					text = MonthTextBox.Text +
					DateSeparator +
					DayTextBox.Text +
					DateSeparator +
					YearTextBox.Text;

				}

				else if (shortDatePattern == "yy/MM/dd"
				|| shortDatePattern == "yyyy/MM/dd")
				{
					text = YearTextBox.Text +
					DateSeparator +
					MonthTextBox.Text +
					DateSeparator +
					DayTextBox.Text;

				}


				else if (shortDatePattern == "dd/MMM/yy"
				|| shortDatePattern == "dd/MM/yy")
				{
					text = DayTextBox.Text +
					DateSeparator +
					MonthTextBox.Text +
					DateSeparator +
					YearTextBox.Text;
				}

				return this.text;
			}

			set
			{
				this.text = value;
				DateTimeOffset tempDate;

				if (DateTimeOffset.TryParse(this.text, FormatInfo, DateTimeStyles.None, out tempDate))
				{
					currentDate = tempDate.Date;
					SetTextBoxControls();
				}
				else
				{
					MonthTextBox.Text = "";
					DayTextBox.Text = "";
					YearTextBox.Text = "";

					if (this.text != "")
					{
						this.text = "";
						throw new Exception("Invalid date.");
					}
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// The following is to get the inner controls to have
				// the tab index originally set for the outer span control
				// and to have the span control to not be in the tab order
				foreach (WebControl control in Controls)
				{
					if ((!typeof(Label).IsInstanceOfType(control)) && (TabIndex != -1))
					{
						control.TabIndex = TabIndex;
					}
				}

				TabIndex = -1;

				MonthTextBox.MaxLength = 2;
				DayTextBox.MaxLength = 2;
				YearTextBox.MaxLength = 2;

				MonthTextBox.Width = new Unit(20, UnitType.Pixel);
				DayTextBox.Width = new Unit(20, UnitType.Pixel);
				YearTextBox.Width = new Unit(20, UnitType.Pixel);
				Calendar.Width = new Unit(110, UnitType.Pixel);
				Separator1Label.Width = new Unit(9, UnitType.Pixel);
				Separator2Label.Width = new Unit(9, UnitType.Pixel);
				Calendar.BackColor = System.Drawing.Color.White;

				if (calendar.Visible)
				{
					calendar_SelectionChanged(null, null);
				}
				else if (this.Attributes["originalZIndex"] != null)
				{
					this.Style["Z-INDEX"] = calendar.Attributes["originalZIndex"];
					calendar.Style["Z-INDEX"] = calendar.Attributes["originalZIndex"];
				}
			}
			catch
			{
			}
		}

		override protected void OnInit(EventArgs e)
		{
			if (!DesignMode)
			{				
				if (Page.Session["SiteGuid"] != null
				&& (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"]))
				{
					bUseDataDictionary = true;
					SiteGuid = (Guid) Page.Session["SiteGuid"];
				}
				else
				{
					bUseDataDictionary = false;
				}
			}
			else
			{
				bUseDataDictionary = false;
			}

			if (string.IsNullOrWhiteSpace(this.Style["Z-INDEX"]))
			{
				this.Style["Z-INDEX"] = "200";
			}
			string zIndex = this.Style["Z-INDEX"];

			if (string.IsNullOrWhiteSpace(calendar.Style["Z-INDEX"]))
			{
				calendar.Style["Z-INDEX"] = zIndex;
			}


			if (calendar.Attributes["originalZIndex"] == null)
			{
				calendar.Attributes["originalZIndex"] = zIndex;
			}
		
			MonthTextBox.ID = ID + " Month";
			Separator1Label.ID = ID + " Separator1";
			Separator1Label.TabIndex = -1;
			DayTextBox.ID = ID + " Day";
			Separator2Label.ID = ID + " Separator2";
			Separator2Label.TabIndex = -1;
			YearTextBox.ID = ID + " Year";
			SetButton.ID = ID + " SetButton";
			SetButton.Style.Add("Left", "2px");
			calendar.Visible = false;
			calendar.ID = ID + " Calendar";

			if (this.ToolTip == null)
			{
				this.ToolTip = string.Empty;
			}
			 
			string setButtonText = "Set";


			try
			{
				if (this.bUseDataDictionary)
				{

                    setButtonText = DataDictionarySingleton.Get(SiteGuid, setButtonText);
                }
            }
			catch
			{
			}
			if (this.ToolTip == null)
			{
				this.ToolTip = string.Empty;
			}
			SetButton.Text = this.ToolTip + " " + setButtonText;
			SetButton.CommandName = SetButton.Text;

			Controls.Add(MonthTextBox);
			Controls.Add(Separator1Label);
			Controls.Add(DayTextBox);
			Controls.Add(Separator2Label);
			Controls.Add(YearTextBox);
			Controls.Add(SetButton);
			Controls.Add(calendar);
			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.SetButton.Command += this.SetButton_Command;
			this.calendar.SelectionChanged += this.calendar_SelectionChanged;
			this.calendar.VisibleMonthChanged += this.calendar_MonthChanged;
			this.Load += this.Page_Load;			
		}

		protected override void OnPreRender(EventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(ShortDatePattern) || string.IsNullOrEmpty(DateSeparator))
				{
					return;
				}

				Separator1Label.Text = "&nbsp;" + DateSeparator + "&nbsp;";
				Separator2Label.Text = "&nbsp;" + DateSeparator + "&nbsp;";

				string shortDatePattern = ShortDatePattern.Replace(DateSeparator, "/");

				if (shortDatePattern == "M/d/yyyy"
				|| shortDatePattern == "MM/dd/yyyy"
				|| shortDatePattern == "yyyy/MM/dd")
				{
					YearTextBox.MaxLength = 4;
					YearTextBox.Width = new Unit(35, UnitType.Pixel);
				}
				else if (shortDatePattern == "dd/MMM/yy")
				{
					MonthTextBox.MaxLength = 3;
					MonthTextBox.Width = new Unit(30, UnitType.Pixel);
				}
			}
			catch
			{
			}

			string pageScript = javascriptBasicFunctions;

			if (IsStandard ==false)
			{
				pageScript = javascriptNonStandardFunctions;
			}

			// register the javascript specific to this instance of the fmdate
			if (ScriptManager.GetCurrent(Page) != null)
			{
				ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "fmdate_basic_functions", pageScript, false);
			}
			else
			{ 
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "fmdate_basic_functions", pageScript);
			}

		}


		/// <summary>
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			CalendarHtmlTextWriter calendarHtmlTextWriter = new CalendarHtmlTextWriter(output);

			string functionNameSuffix = IsStandard ? string.Empty : "Custom";

			this.MonthTextBox.Attributes.Add("onkeydown", "return fmdate_textbox_onkeydown" + functionNameSuffix + "('" + ClientID + " Month" + "','" + ShortDatePattern + "');");
			this.DayTextBox.Attributes.Add("onkeydown", "return fmdate_textbox_onkeydown" + functionNameSuffix + "('" + ClientID + " Day" + "','" + ShortDatePattern + "');");
			this.YearTextBox.Attributes.Add("onkeydown", "return fmdate_textbox_onkeydown" + functionNameSuffix + "('" + ClientID + " Year" + "','" + ShortDatePattern + "');");
			this.YearTextBox.Attributes.Add("onchange", "return fmdate_textbox_onChange" + functionNameSuffix + "('" + ClientID + "');");
			this.MonthTextBox.Attributes.Add("onchange", "return fmdate_textbox_onChange" + functionNameSuffix + "('" + ClientID + "');");
			this.DayTextBox.Attributes.Add("onchange", "return fmdate_textbox_onChange" + functionNameSuffix + "('" + ClientID + "');");
			this.SetButton.Attributes.Add("onclick", "return fmdate_setbutton_click" + functionNameSuffix + "('" + ClientID + "');");

			if (this.ToolTip == null)
			{
				this.ToolTip = string.Empty;
			}
			this.MonthTextBox.Attributes.Add("alt", this.ToolTip + " Month");
			this.MonthTextBox.ToolTip = this.ToolTip + " Month";
			this.DayTextBox.Attributes.Add("alt", this.ToolTip + " Day");
			this.DayTextBox.ToolTip = this.ToolTip + " Day";
			this.YearTextBox.Attributes.Add("alt", this.ToolTip + " Year");
			this.YearTextBox.ToolTip = this.ToolTip + " Year";

			RenderBeginTag(output);

			string shortDatePattern = ShortDatePattern.Replace(DateSeparator, "/");

			if (shortDatePattern == "M/d/yyyy"
			|| shortDatePattern == "M/d/yy"
			|| shortDatePattern == "MM/dd/yy"
			|| shortDatePattern == "MM/dd/yyyy")
			{
				MonthTextBox.RenderControl(output);
				Separator1Label.RenderControl(output);
				DayTextBox.RenderControl(output);
				Separator2Label.RenderControl(output);
				YearTextBox.RenderControl(output);
				SetButton.RenderControl(output);
				calendar.RenderControl(calendarHtmlTextWriter);
			}

			else if (shortDatePattern == "yy/MM/dd"
			|| shortDatePattern == "yyyy/MM/dd")
			{
				YearTextBox.RenderControl(output);
				Separator2Label.RenderControl(output);
				MonthTextBox.RenderControl(output);
				Separator1Label.RenderControl(output);
				DayTextBox.RenderControl(output);
				SetButton.RenderControl(output);
				calendar.RenderControl(calendarHtmlTextWriter);
			}


			else if (shortDatePattern == "dd/MMM/yy"
			|| shortDatePattern == "dd/MM/yy")
			{
				DayTextBox.RenderControl(output);
				Separator2Label.RenderControl(output);
				MonthTextBox.RenderControl(output);
				Separator1Label.RenderControl(output);
				YearTextBox.RenderControl(output);
				SetButton.RenderControl(output);
				calendar.RenderControl(calendarHtmlTextWriter);
			}

			RenderEndTag(output);
		}

		private void SetButton_Command(object sender, CommandEventArgs e)
		{
			DayTextBox.Visible = false;
			Separator2Label.Visible = false;
			MonthTextBox.Visible = false;
			Separator1Label.Visible = false;
			YearTextBox.Visible = false;
			SetButton.Visible = false;
			calendar.Visible = true;

			int zIndex = 0;
			if (Int32.TryParse(calendar.Style["Z-INDEX"], out zIndex))
			{
				zIndex += 200;
				calendar.Style["Z-INDEX"] = zIndex.ToString();
			}
			else
			{
				calendar.Style["Z-INDEX"] = "311";
			}
			this.Style["Z-INDEX"] = calendar.Style["Z-INDEX"];

			// No exceptions are explicity thrown in any of the try blocks because each one has
			// a catch all block that disregards any exceptions.  The reasoning is that selecting
			// a date should not throw an exception.  Any invalid state that might cause an exception
			// will be handled at a higher level of execution.
			try
			{
				calendar.SelectedDate = DateTime.Parse(Text, FormatInfo);
			}
			catch
			{
				try
				{
					var security = Page.Session["Security"] as SecurityClass;
					var currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 security != null ? x.Get(security, security.SiteGuid, false, false, false) : null
																);

					calendar.SelectedDate = TimeConverter.Today(currentSite).Date;
				}
				catch
				{
					calendar.SelectedDate = DateTime.Today;
				}

				// When Set Occurs on FMDate that is part of FMDateTime force the Time
				if (typeof(FMDateTime).IsInstanceOfType(Parent))
				{
					var fmDateTime = Parent as FMDateTime;

					try
					{
						if (fmDateTime != null)
						{
							fmDateTime.Text = this.calendar.SelectedDate.ToString(this.FormatInfo);
						}
					}
					catch
					{
					}
				}
			}

			calendar.VisibleDate = calendar.SelectedDate;
		}

		private void calendar_SelectionChanged(object sender, EventArgs e)
		{
			DayTextBox.Visible = true;
			Separator2Label.Visible = true;
			MonthTextBox.Visible = true;
			Separator1Label.Visible = true;
			YearTextBox.Visible = true;
			SetButton.Visible = true;
			calendar.Visible = false;
			if (calendar.Attributes["originalZIndex"] == null)
			{
				this.Style["Z-INDEX"] = calendar.Style["Z-INDEX"];
			}
			else
			{
				this.Style["Z-INDEX"] = calendar.Attributes["originalZIndex"];

			}
			Text = calendar.SelectedDate.ToString(FormatInfo);
		}

		private void calendar_MonthChanged(object sender, MonthChangedEventArgs e)
		{
			DayTextBox.Visible = false;
			Separator2Label.Visible = false;
			MonthTextBox.Visible = false;
			Separator1Label.Visible = false;
			YearTextBox.Visible = false;
			SetButton.Visible = false;
			calendar.Visible = true;
			int zIndex = 0;
			if (Int32.TryParse(calendar.Style["Z-INDEX"], out zIndex))
			{
				zIndex += 200;
				calendar.Style["Z-INDEX"] = zIndex.ToString();
			}
			else
			{
				calendar.Style["Z-INDEX"] = "311";
			}
			this.Style["Z-INDEX"] = calendar.Style["Z-INDEX"];

		}

		private static string javascriptBasicFunctions = @"
		<script type='text/javascript'>
		<!--

		// This function will updated the transaction status on the transaction detail
		// page if the schedule datetime calendar display button is pressed.
		function fmdate_setbutton_click(setButtonID)
		{
		if (setButtonID.indexOf('TransactionFields.ScheduledDateFG DateTime Date') != -1)
		{
			var orderStatus = document.getElementById('TransactionFields.TransactionStatusFG');
			if (orderStatus != null)
			{
			var options = orderStatus.options;

			if ((options != null) && (options.length > 0))
			{
				var selectedIndex = orderStatus.selectedIndex;
				var selectedOption = options[selectedIndex];

				if ((selectedOption != null) && (selectedOption.value == 'Requested'))
				{
				for (nextOption = 0; nextOption < options.length; nextOption++)
				{
					if (options[nextOption].value == 'Scheduled')
					{
					orderStatus.selectedIndex = nextOption;
					break;
					}
				}
				}
			}
			}
		}
		}

		// This function will updated the transaction status on the transaction detail
		// page if the schedule datetime is changed.
		function fmdate_textbox_onChange(textboxID)
		{
		if (textboxID.indexOf('TransactionFields.ScheduledDateFG') != -1)
		{
			var orderStatus = document.getElementById('TransactionFields.TransactionStatusFG');
			if (orderStatus != null)
			{
			var options = orderStatus.options;

			if ((options != null) && (options.length > 0))
			{
				var selectedIndex = orderStatus.selectedIndex;
				var selectedOption = options[selectedIndex];

				if ((selectedOption != null) && (selectedOption.value == 'Requested'))
				{
				for (nextOption = 0; nextOption < options.length; nextOption++)
				{
					if (options[nextOption].value == 'Scheduled')
					{
					orderStatus.selectedIndex = nextOption;
					break;
					}
				}
				}
			}
			}
		}
		}

		function fmdate_textbox_onkeydown(textboxID, datepattern)
		{
		var textbox  = document.getElementById(textboxID);
		var valueInt = parseInt(textbox.value, 10);

		if(textboxID.indexOf(' Month') != -1)
		{
			var MonthAbrv = new Array('Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec');

			// Down
			if(event.keyCode == '40')
			{
			if(datepattern.indexOf('MMM') != -1)
			{
				var Index;
				for(Index = 0; Index < 12; Index++)
				{
				if (textbox.value == MonthAbrv[Index])
				{
					Index++;
					break;
				}
				}

				if (Index > 11)
				{
				Index = 0;
				}
				textbox.value = MonthAbrv[Index];
			}
			else
			{
				if (isNaN(valueInt))
				{
				valueInt = 13;
				}

				valueInt--;

				if (valueInt == 0)
				{
				valueInt = 12;
				}

				if ((datepattern.indexOf('MM') != -1) && (valueInt < 10))
				{
				textbox.value = '0' + valueInt.toString(10);
				}
				else
				{
				textbox.value = valueInt.toString(10);
				}
			}

			fmdate_textbox_onChange(textboxID);
			}

			// Up
			else if(event.keyCode == '38')
			{
			if (datepattern.indexOf('MMM') != -1)
			{
				var Index;
				for (Index = 11; Index > -1; Index--)
				{
				if (textbox.value == MonthAbrv[Index])
				{
					Index--;
					break;
				}
				}

				if (Index < 0)
				{
				Index = 11;
				}

				textbox.value = MonthAbrv[Index];
			}
			else
			{
				if (isNaN(valueInt))
				{
				valueInt = 0;
				}

				valueInt++;

				if (valueInt > 12)
				{
				valueInt = 1;
				}

				if ((datepattern.indexOf('MM') != -1) && (valueInt < 10))
				{
				textbox.value = '0' + valueInt.toString(10);
				}
				else
				{
				textbox.value = valueInt.toString(10);
				}
			}

			fmdate_textbox_onChange(textboxID);
			}
		}

		else if(textboxID.indexOf(' Day') != -1)
		{
			// Down
			if(event.keyCode == '40')
			{
			if (isNaN(valueInt))
			{
				valueInt = 32;
			}

			valueInt--;
			if (valueInt <= 0)
			{
				valueInt = 31;
			}

			if ((datepattern.indexOf('dd') != -1) && (valueInt < 10))
			{
				textbox.value = '0' + (valueInt.toString(10));
			}
			else
			{
				textbox.value = valueInt.toString(10);
			}

			fmdate_textbox_onChange(textboxID);
			}

			// Up
			else if(event.keyCode == '38')
			{
			if (isNaN(valueInt))
			{
				valueInt = 0;
			}

			valueInt++;
			if (valueInt >= 32)
			{
				valueInt = 1;
			}

			if ((datepattern.indexOf('dd') != -1) && (valueInt < 10))
			{
				textbox.value = '0' + (valueInt.toString(10));
			}
			else
			{
				textbox.value = valueInt.toString(10);
			}

			fmdate_textbox_onChange(textboxID);
			}
		}

		else if(textboxID.indexOf(' Year') != -1)
		{
			if (isNaN(valueInt))
			{
			var d = new Date();
			if (datepattern.indexOf('yyyy') != -1)
			{
				textbox.value = d.getFullYear();
			}
			else
			{
				textbox.value = d.getFullYear();
				textbox.value = textbox.value.substring(2, 2);
			}

			fmdate_textbox_onChange(textboxID);
			}

			// Down
			else
			{
			if (event.keyCode == '40')
			{
				valueInt--;
				if (valueInt < 0)
				{
				valueInt = 99;
				}

				if (valueInt < 10)
				{
				textbox.value = '0' + valueInt.toString(10);
				}
				else
				{
				textbox.value = valueInt.toString(10);
				}

				fmdate_textbox_onChange(textboxID);
			}

			// Up
			else if (event.keyCode == '38')
			{
				valueInt++;
				if (valueInt < 1900 && valueInt > 99)
				{
				valueInt = 0;
				}

				if(valueInt < 10)
				{
				textbox.value = '0' + valueInt.toString(10);
				}
				else
				{
				textbox.value = valueInt.toString(10);
				}

				fmdate_textbox_onChange(textboxID);
			}
			}
		}
		}
		//-->
		</script>
		";

		private static string javascriptNonStandardFunctions = @"
		<script type='text/javascript'>
		<!--

		function fmdate_setbutton_clickCustom(setButtonID)
		{
			if (window.fmdate_setbutton_click)	
			{
				fmdate_setbutton_click(setButtonID)
			}
		}

		function fmdate_textbox_onChangeCustom(textboxID)
		{
			if (window.fmdate_textbox_onChange)	
			{
				fmdate_textbox_onChange(textboxID)
			}
		}

		function fmdate_textbox_onkeydownCustom(textboxID, datepattern)
		{
			if (window.fmdate_textbox_onkeydown)	
			{
				fmdate_textbox_onkeydown(textboxID, datepattern)
			}
		}
		//-->
		</script>
		";


		private class CalendarHtmlTextWriter : HtmlTextWriter
		{
			public CalendarHtmlTextWriter(System.IO.TextWriter writer)
				: base(writer)
			{

			}

			public CalendarHtmlTextWriter(System.IO.TextWriter writer, string tabString)
				: base(writer, tabString)
			{

			}


			public override void RenderBeginTag(HtmlTextWriterTag tagKey)
			{
				if (tagKey == HtmlTextWriterTag.Table)
				{
					string val = "";
					if (!this.IsAttributeDefined(HtmlTextWriterAttribute.Title, out val))
					{
						val = "Calendar";
					}
					this.AddAttribute("aria-label", val);
					if (this.IsAttributeDefined(HtmlTextWriterAttribute.Border, out val))
					{
						if (val == "0")
						{
							this.AddAttribute("role", "presentation");
						}

					}
					else
					{
						this.AddAttribute("role", "presentation");

					}
				}
				base.RenderBeginTag(tagKey);
			}
		}

	}
}
