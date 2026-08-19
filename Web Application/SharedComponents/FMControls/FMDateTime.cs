/******************************************************************************
	FILE NAME:		FMDate.cs
	PURPOSE:		Implementation of: FMDateTime

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version



	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		2007-11-14	Richard Panachida	Added a new property that will return the DateTime object
										from the FMDate object.
		2008-11-10  A. Coker            Changed date and time formats to honor regional settings. (Task ID 189)
*******************************************************************************/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Globalization;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace FMControls
{
	/// <summary>
	/// Summary description for FMDateTime.
	/// </summary>
	[DefaultProperty("Text"),
	ToolboxData("<{0}:FMDateTime runat=server></{0}:FmDateTime>")]
	public class FMDateTime : System.Web.UI.WebControls.WebControl
	{
		protected FMDate fmDate = new FMDate();
		protected Label SeparatorLabel = new Label();
		protected FMTime fmTime = new FMTime();

		public FMDateTime()
		{
		}


		#region Properties
		/// <summary>
		/// Gets or sets the format information.
		/// </summary>
		/// <value>
		/// The format information.
		/// </value>
		public DateTimeFormatInfo FormatInfo
		{
			get { return fmDate.FormatInfo; }
			set
			{
				fmTime.TimeFormatInfo = value;
				fmDate.FormatInfo = value;
			}
		}


		/// <summary>
		/// This property will return the date that the text field is set to.
		/// </summary>
		public DateTimeOffset CurrentValue
		{
			get
			{
				string DateTimeText = Text;
				DateTimeFormatInfo formatInfo = fmDate.FormatInfo;
				if (formatInfo == null)
				{
					formatInfo = new DateTimeFormatInfo();
					formatInfo.AMDesignator = DateTimeFormatInfo.CurrentInfo.AMDesignator;
					formatInfo.PMDesignator = DateTimeFormatInfo.CurrentInfo.PMDesignator;
					formatInfo.ShortTimePattern = DateTimeFormatInfo.CurrentInfo.ShortTimePattern;
					formatInfo.AMDesignator = DateTimeFormatInfo.CurrentInfo.AMDesignator;
					formatInfo.AMDesignator = DateTimeFormatInfo.CurrentInfo.AMDesignator;
					formatInfo.ShortDatePattern = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
					formatInfo.DateSeparator = DateTimeFormatInfo.CurrentInfo.DateSeparator;
				}
				else
				{
					formatInfo = fmDate.FormatInfo;
				}

				fmTime.Offset = null;

				if (this.Page.Session["Security"] == null)
				{
					throw new ArgumentNullException("Security");
				}

				var security = this.Page.Session["Security"] as SecurityClass;
				if (security == null)
				{
					throw new ArgumentNullException("Security");
				}

				var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.SiteGuid, false, false, false));

				var dateTimeOffset = DateTimeOffset.Parse(this.Text, formatInfo);
				var siteTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(site.TimeZone);
				var offset = siteTimeZoneInfo.GetUtcOffset(dateTimeOffset.DateTime);
				this.fmTime.Offset = offset;

				return DateTimeOffset.Parse(Text, formatInfo);
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
				fmDate.CssClass = value;
				SeparatorLabel.CssClass = value;
				fmTime.CssClass = value;
			}
		}


		public System.Web.UI.WebControls.Calendar Calendar
		{
			get
			{
				return fmDate.Calendar;
			}
		}


		[Bindable(true),
		Category("Appearance"),
		DefaultValue("")]
		public string Text
		{
			get
			{
				if (fmDate.Text != "")
				{
					if (fmTime.Text == "")
						fmTime.Text = "12:00:00 AM";

					return fmDate.Text + " " + fmTime.Text;
				}
				else
					return "";
			}

			set
			{
				int DelimiterIndex = value.IndexOf(" ");
				if (DelimiterIndex >= 0)
				{
					fmDate.Text = value.Substring(0, DelimiterIndex);
					fmTime.Text = value.Substring(DelimiterIndex + 1);
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			SeparatorLabel.Text = "&nbsp;&nbsp;&nbsp;";

			// The following is to get the inner controls to have
			// the tab index originally set for the outer span control
			// and to have the span control to not be in the tab order
			foreach (WebControl Control in Controls)
			{
				if (!typeof(Label).IsInstanceOfType(Control) && TabIndex != -1)
				{
					Control.TabIndex = TabIndex;
				}
			}
			TabIndex = -1;
		}

		override protected void OnInit(EventArgs e)
		{
			if (string.IsNullOrEmpty(this.ToolTip))
			{
				this.ToolTip = "Date and Time";
			}
			if (string.IsNullOrWhiteSpace(this.Style["Z-INDEX"]))
			{
				this.Style["Z-INDEX"] = "200";
			}
			string zIndex = this.Style["Z-INDEX"];

			if (string.IsNullOrWhiteSpace(fmDate.Style["Z-INDEX"]))
			{
				fmDate.Style["Z-INDEX"] = zIndex;
			}			fmDate.ID = ID + " Date";
			fmDate.ToolTip = this.ToolTip;
			SeparatorLabel.ID = ID + " Separator";
			SeparatorLabel.TabIndex = -1;
			fmTime.ID = ID + " Time";
			fmTime.ToolTip = this.ToolTip;
			Controls.Add(fmDate);
			Controls.Add(SeparatorLabel);
			Controls.Add(fmTime);



			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}

		protected override void OnPreRender(EventArgs e)
		{

			base.OnPreRender(e);
			if (fmDate.Style["Z-INDEX"] != null)
			{
				this.Style["Z-INDEX"] = fmDate.Style["Z-INDEX"];
			}
		}

		/// <summary>
		/// Render this control to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void Render(HtmlTextWriter output)
		{
			RenderBeginTag(output);
			fmDate.RenderControl(output);
			SeparatorLabel.RenderControl(output);
			fmTime.RenderControl(output);
			RenderEndTag(output);
		}
	}
}
