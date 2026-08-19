// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMTime.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Handles time display and entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.ComponentModel;
	using System.Globalization;
	using System.Text;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Handles time display and entry.
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:FMTime runat=server></{0}:FmTime>")]
	public class FMTime : WebControl
	{
		#region Constants and Fields

		protected TextBox AMPMTextBox = new TextBox();

		protected TextBox HourTextBox = new TextBox();

		protected TextBox MinuteTextBox = new TextBox();

		public TimeSpan? Offset;

		protected TextBox SecondTextBox = new TextBox();

		protected Label Separator1Label = new Label();

		protected Label Separator2Label = new Label();

		protected Label Separator3Label = new Label();

		private static string javascriptBasicFunctions = @"
		<script type='text/javascript'>
		<!--

		// This function will updated the transaction status on the transaction detail
		// page if the schedule datetime is changed.
		function fmtime_textbox_onchange(textboxID)
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

		function fmtime_textbox_onkeydown(textboxID, timepattern, amSymbol, pmSymbol)
		{
			var textbox = document.getElementById(textboxID);
			var valueInt=parseInt(textbox.value,10);

			if(textboxID.indexOf(' Hour') != -1)
			{
				// Down
				if(event.keyCode == '40')
				{
					if (isNaN(valueInt))
					{
						if (timepattern.indexOf('tt') != -1)
						{
							valueInt = 13;
						}
						else
						{
							valueInt = 1;
						}
					}

					valueInt--;
					if (timepattern.indexOf('tt') != -1)
					{
						if (valueInt < 1)
						{
							valueInt = 12;
						}
					}
					else
					{
						if (valueInt < 0)
						{
							valueInt = 23;
						}
					}

					if (((timepattern.indexOf('HH') != -1) || (timepattern.indexOf('hh') != -1)) && (valueInt < 10))
					{
						textbox.value = '0' + valueInt.toString(10);
					}
					else
					{
						textbox.value = valueInt.toString(10);
					}

					fmtime_textbox_onchange(textboxID);
				}

				// Up
				else if(event.keyCode == '38')
				{
					if(isNaN(valueInt))
					{
						if(timepattern.indexOf('tt') != -1)
							valueInt=0;
						else
							valueInt=22;
					}

					valueInt++;
					if(timepattern.indexOf('tt') != -1)
					{
						if(valueInt > 12)
							valueInt=1;
					}
					else
					{
						if(valueInt > 23)
							valueInt=0;
					}

					if (((timepattern.indexOf('HH') != -1) || (timepattern.indexOf('hh') != -1)) && (valueInt < 10))
					{
						textbox.value = '0' + valueInt.toString(10);
					}
					else
					{
						textbox.value = valueInt.toString(10);
					}

					fmtime_textbox_onchange(textboxID);
				}
			}

			else if ((textboxID.indexOf(' Minute') != -1) || (textboxID.indexOf(' Second') != -1))
			{
				// Down
				if(event.keyCode == '40')
				{
					if (isNaN(valueInt))
					{
						valueInt = 1;
					}

					valueInt--;
					if (valueInt < 0)
					{
						valueInt = 59;
					}

					if (valueInt < 10)
					{
						textbox.value = '0' + valueInt.toString(10);
					}
					else
					{
						textbox.value = valueInt.toString(10);
					}

					fmtime_textbox_onchange(textboxID);
				}

				// Up
				else if(event.keyCode == '38')
				{
					if (isNaN(valueInt))
					{
						valueInt = 58;
					}

					valueInt++;
					if (valueInt > 59)
					{
						valueInt = 0;
					}

					if (valueInt < 10)
					{
						textbox.value = '0' + valueInt.toString(10);
					}
					else
					{
						textbox.value = valueInt.toString(10);
					}

					fmtime_textbox_onchange(textboxID);
				}
			}
			else if (textboxID.indexOf(' AM/PM') != -1)
			{
				// Down
				if(event.keyCode == '40')
				{
					if (textbox.value == amSymbol)
					{
						textbox.value = pmSymbol;
					}
					else if (textbox.value == pmSymbol)
					{
						textbox.value = amSymbol;
					}
					else
					{
						textbox.value = amSymbol;
					}

					fmtime_textbox_onchange(textboxID);
				}
				// Up
				else if(event.keyCode == '38')
				{
					if (textbox.value == amSymbol)
					{
						textbox.value = pmSymbol;
					}
					else if (textbox.value == pmSymbol)
					{
						textbox.value = amSymbol;
					}
					else
					{
						textbox.value = amSymbol;
					}

					fmtime_textbox_onchange(textboxID);
				}
			}
		}
		//-->
		</script>";

		private string text = string.Empty;

		#endregion

		#region Public Properties

		public string AMSymbol
		{
			get
			{
				return this.TimeFormatInfo.AMDesignator;
			}
		}

		public override string CssClass
		{
			get
			{
				return base.CssClass;
			}

			set
			{
				base.CssClass = value;
				this.HourTextBox.CssClass = value;
				this.Separator1Label.CssClass = value;
				this.MinuteTextBox.CssClass = value;
				this.Separator2Label.CssClass = value;
				this.SecondTextBox.CssClass = value;
				this.Separator3Label.CssClass = value;
				this.AMPMTextBox.CssClass = value;
			}
		}

		public string PMSymbol
		{
			get
			{
				return this.TimeFormatInfo.PMDesignator;
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		public string Text
		{
			get
			{
				var sb = new StringBuilder();

				if (this.HourTextBox.Text == string.Empty || this.MinuteTextBox.Text == string.Empty)
				{
					return string.Empty;
				}

				if (this.TimePattern == "h:mm:ss tt" || this.TimePattern == "hh:mm:ss tt")
				{
					sb.Append(this.HourTextBox.Text);
					sb.Append(this.TimeSeparator);
					sb.Append(this.MinuteTextBox.Text);
					sb.Append(this.TimeSeparator);
					sb.Append(this.SecondTextBox.Text);
					sb.Append(" ");
					sb.Append(this.AMPMTextBox.Text);
				}
				else if (this.TimePattern == "h:mm tt" || this.TimePattern == "hh:mm tt")
				{
					sb.Append(this.HourTextBox.Text);
					sb.Append(this.TimeSeparator);
					sb.Append(this.MinuteTextBox.Text);
					sb.Append(" ");
					sb.Append(this.AMPMTextBox.Text);
				}
				else if (this.TimePattern == "H:mm:ss" || this.TimePattern == "HH:mm:ss")
				{
					sb.Append(this.HourTextBox.Text);
					sb.Append(this.TimeSeparator);
					sb.Append(this.MinuteTextBox.Text);
					sb.Append(this.TimeSeparator);
					sb.Append(this.SecondTextBox.Text);
				}
				else if (this.TimePattern == "H:mm" || this.TimePattern == "HH:mm")
				{
					sb.Append(this.HourTextBox.Text);
					sb.Append(this.TimeSeparator);
					sb.Append(this.MinuteTextBox.Text);
				}

				if (this.Offset != null && this.Offset.Value != TimeSpan.Zero)
				{
					sb.Append(" ");

					if (this.Offset.Value.TotalSeconds > 0)
					{
						sb.Append("+");
					}
					else if (this.Offset.Value.TotalSeconds < 0)
					{
						sb.Append("-");
					}
					//using abs because timezones with partial hours break it - would out put -09:-30 for example
					sb.Append(Math.Abs(this.Offset.Value.Hours).ToString("D2"));
					sb.Append(this.TimeSeparator);
					sb.Append(Math.Abs(this.Offset.Value.Minutes).ToString("D2"));
				}

				this.text = sb.ToString();
				return this.text;
			}

			set
			{
				this.text = value;

				try
				{
					var currentTime = DateTimeOffset.Parse(this.text, this.TimeFormatInfo);

					this.MinuteTextBox.Text = currentTime.Minute.ToString("D02");
					this.SecondTextBox.Text = currentTime.Second.ToString("D02");
					this.Offset = currentTime.Offset;

					if (currentTime.Hour < 12)
					{
						this.AMPMTextBox.Text = this.AMSymbol;
					}
					else
					{
						this.AMPMTextBox.Text = this.PMSymbol;
					}

					if (this.TimePattern == "h:mm:ss tt" || this.TimePattern == "h:mm tt")
					{
						if (currentTime.Hour == 0)
						{
							this.HourTextBox.Text = "12";
						}
						else if (currentTime.Hour < 13)
						{
							this.HourTextBox.Text = currentTime.Hour.ToString();
						}
						else
						{
							this.HourTextBox.Text = (currentTime.Hour - 12).ToString();
						}
					}
					else if (this.TimePattern == "hh:mm:ss tt" || this.TimePattern == "hh:mm tt")
					{
						if (currentTime.Hour == 0)
						{
							this.HourTextBox.Text = "12";
						}
						else if (currentTime.Hour < 13)
						{
							this.HourTextBox.Text = currentTime.Hour.ToString("D02");
						}
						else
						{
							this.HourTextBox.Text = (currentTime.Hour - 12).ToString("D02");
						}
					}
					else if (this.TimePattern == "H:mm:ss" || this.TimePattern == "H:mm")
					{
						this.HourTextBox.Text = currentTime.Hour.ToString();
					}
					else if (this.TimePattern == "HH:mm:ss" || this.TimePattern == "HH:mm")
					{
						this.HourTextBox.Text = currentTime.Hour.ToString("D02");
					}
				}
				catch
				{
					this.HourTextBox.Text = string.Empty;
					this.MinuteTextBox.Text = string.Empty;
					this.SecondTextBox.Text = string.Empty;
					this.AMPMTextBox.Text = string.Empty;
					this.text = string.Empty;
				}
			}
		}

		public DateTimeFormatInfo TimeFormatInfo
		{
			get
			{
				DateTimeFormatInfo formatInfo;

				try
				{
					formatInfo = this.ViewState["TimeFormatInfo"] as DateTimeFormatInfo;

					if (formatInfo == null)
					{
						if (this.Page.Session["Token"] != null)
						{
							if (this.Page.Session["Security"] == null)
							{
								throw new ArgumentNullException("Security");
							}

							var security = this.Page.Session["Security"] as SecurityClass;
							if (security == null)
							{
								throw new ArgumentNullException("Security");
							}

							var site =
								FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid, false, false, false));

							formatInfo = site.GetDateTimeFormatInfo();
							this.ViewState["TimeFormatInfo"] = formatInfo;
						}

						if (formatInfo == null)
						{
							formatInfo = new DateTimeFormatInfo();
							formatInfo.AMDesignator = DateTimeFormatInfo.CurrentInfo.AMDesignator;
							formatInfo.PMDesignator = DateTimeFormatInfo.CurrentInfo.PMDesignator;
							formatInfo.ShortTimePattern = DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
							formatInfo.TimeSeparator = DateTimeFormatInfo.CurrentInfo.TimeSeparator;
							this.ViewState["TimeFormatInfo"] = formatInfo;
						}
					}
				}
				catch
				{
					formatInfo = new DateTimeFormatInfo();
					formatInfo.AMDesignator = DateTimeFormatInfo.CurrentInfo.AMDesignator;
					formatInfo.PMDesignator = DateTimeFormatInfo.CurrentInfo.PMDesignator;
					formatInfo.ShortTimePattern = DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
					formatInfo.TimeSeparator = DateTimeFormatInfo.CurrentInfo.TimeSeparator;
					this.ViewState["TimeFormatInfo"] = formatInfo;
				}

				return formatInfo;
			}

			set
			{
				this.ViewState["TimeFormatInfo"] = value;
			}
		}

		public string TimePattern
		{
			get
			{
				return this.TimeFormatInfo.ShortTimePattern;
			}
		}

		public string TimeSeparator
		{
			get
			{
				return this.TimeFormatInfo.TimeSeparator;
			}
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			this.HourTextBox.ID = this.ID + " Hour";
			this.Separator1Label.ID = this.ID + " Separator1";
			this.Separator1Label.TabIndex = -1;
			this.MinuteTextBox.ID = this.ID + " Minute";
			this.Separator2Label.ID = this.ID + " Separator2";
			this.Separator2Label.TabIndex = -1;
			this.SecondTextBox.ID = this.ID + " Second";
			this.Separator3Label.ID = this.ID + " Separator3";
			this.Separator3Label.TabIndex = -1;
			this.AMPMTextBox.ID = this.ID + " AM/PM";

			this.Controls.Add(this.HourTextBox);
			this.Controls.Add(this.Separator1Label);
			this.Controls.Add(this.MinuteTextBox);
			this.Controls.Add(this.Separator2Label);
			this.Controls.Add(this.SecondTextBox);
			this.Controls.Add(this.AMPMTextBox);

			this.InitializeComponent();
			base.OnInit(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			try
			{
				this.Separator1Label.Text = "&nbsp;" + this.TimeSeparator + "&nbsp;";
				this.Separator2Label.Text = "&nbsp;" + this.TimeSeparator + "&nbsp;";
				this.Separator3Label.Text = "&nbsp;&nbsp;";

				this.MinuteTextBox.Width = new Unit(20, UnitType.Pixel);
				this.SecondTextBox.Width = new Unit(20, UnitType.Pixel);
				this.HourTextBox.Width = new Unit(20, UnitType.Pixel);
				this.Separator1Label.Width = new Unit(9, UnitType.Pixel);
				this.Separator2Label.Width = new Unit(9, UnitType.Pixel);
				this.Separator3Label.Width = new Unit(6, UnitType.Pixel);
				this.AMPMTextBox.Width = new Unit(25, UnitType.Pixel);

				this.HourTextBox.MaxLength = 2;
				this.MinuteTextBox.MaxLength = 2;
				this.SecondTextBox.MaxLength = 2;
				this.AMPMTextBox.MaxLength = 2;
			}
			catch
			{
			}

			// register the javascript specific to this instance of the fmtime
			if (ScriptManager.GetCurrent(this.Page) != null)
			{
				ScriptManager.RegisterClientScriptBlock(
					this.Page, 
					this.GetType(), 
					"fmtime_basic_functions", 
					javascriptBasicFunctions, 
					false);
			}
			else
			{
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "fmtime_basic_functions", javascriptBasicFunctions);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// The following is to get the inner controls to have
				// the tab index originally set for the outer span control
				// and to have the span control to not be in the tab order
				foreach (WebControl control in this.Controls)
				{
					if (!(control is Label) && this.TabIndex != -1)
					{
						control.TabIndex = this.TabIndex;
					}
				}

				this.TabIndex = -1;
			}
			catch
			{
			}
		}

		/// <summary>
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output">
		/// The HTML writer to write out to 
		/// </param>
		protected override void Render(HtmlTextWriter output)
		{
			this.RenderBeginTag(output);

			this.HourTextBox.Attributes.Add(
				"onkeydown", 
				"return fmtime_textbox_onkeydown('" + this.ClientID + " Hour" + "','" + this.TimePattern + "','" + this.AMSymbol
				+ "','" + this.PMSymbol + "');");
			this.MinuteTextBox.Attributes.Add(
				"onkeydown", 
				"return fmtime_textbox_onkeydown('" + this.ClientID + " Minute" + "','" + this.TimePattern + "','" + this.AMSymbol
				+ "','" + this.PMSymbol + "');");
			this.SecondTextBox.Attributes.Add(
				"onkeydown", 
				"return fmtime_textbox_onkeydown('" + this.ClientID + " Second" + "','" + this.TimePattern + "','" + this.AMSymbol
				+ "','" + this.PMSymbol + "');");
			this.AMPMTextBox.Attributes.Add(
				"onkeydown", 
				"return fmtime_textbox_onkeydown('" + this.ClientID + " AM/PM" + "','" + this.TimePattern + "','" + this.AMSymbol
				+ "','" + this.PMSymbol + "');");
			this.HourTextBox.Attributes.Add("onchange", "return fmtime_textbox_onchange('" + this.ClientID + "');");
			this.MinuteTextBox.Attributes.Add("onchange", "return fmtime_textbox_onchange('" + this.ClientID + "');");
			this.SecondTextBox.Attributes.Add("onchange", "return fmtime_textbox_onchange('" + this.ClientID + "');");
			this.AMPMTextBox.Attributes.Add("onchange", "return fmtime_textbox_onchange('" + this.ClientID + "');");

			if (this.ToolTip == null)
			{
				this.ToolTip = string.Empty;
			}
			this.HourTextBox.Attributes.Add("alt", this.ToolTip + " Hours");
			this.HourTextBox.ToolTip = this.ToolTip + " Hours";
			this.MinuteTextBox.Attributes.Add("alt", this.ToolTip + " Minutes");
			this.MinuteTextBox.ToolTip = this.ToolTip + " Minutes";
			this.SecondTextBox.Attributes.Add("alt", this.ToolTip + " Seconds");
			this.SecondTextBox.ToolTip = this.ToolTip + " Seconds";
			this.AMPMTextBox.Attributes.Add("alt", this.ToolTip + " AM or PM");
			this.AMPMTextBox.ToolTip = this.ToolTip + " AM or PM";

			if (this.TimePattern == "h:mm:ss tt" || this.TimePattern == "hh:mm:ss tt")
			{
				this.HourTextBox.RenderControl(output);
				this.Separator1Label.RenderControl(output);
				this.MinuteTextBox.RenderControl(output);
				this.Separator2Label.RenderControl(output);
				this.SecondTextBox.RenderControl(output);
				this.Separator3Label.RenderControl(output);
				this.AMPMTextBox.RenderControl(output);
			}
			else if (this.TimePattern == "h:mm tt" || this.TimePattern == "hh:mm tt")
			{
				this.HourTextBox.RenderControl(output);
				this.Separator1Label.RenderControl(output);
				this.MinuteTextBox.RenderControl(output);
				this.Separator3Label.RenderControl(output);
				this.AMPMTextBox.RenderControl(output);
			}
			else if (this.TimePattern == "H:mm:ss" || this.TimePattern == "HH:mm:ss")
			{
				this.HourTextBox.RenderControl(output);
				this.Separator1Label.RenderControl(output);
				this.MinuteTextBox.RenderControl(output);
				this.Separator2Label.RenderControl(output);
				this.SecondTextBox.RenderControl(output);
			}
			else if (this.TimePattern == "H:mm" || this.TimePattern == "HH:mm")
			{
				this.HourTextBox.RenderControl(output);
				this.Separator1Label.RenderControl(output);
				this.MinuteTextBox.RenderControl(output);
			}

			this.RenderEndTag(output);
		}

		private void InitializeComponent()
		{
			this.Load += this.Page_Load;
		}

		#endregion
	}
}