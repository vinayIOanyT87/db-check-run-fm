// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IATACodeGeneralPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IATACodeGeneralPage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
    using System;
    using System.Globalization;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;

    /// <summary>
    /// Summary description for IATACodeGeneralPage.
    /// </summary>
    public partial class IATACodeGeneralPage : IATACodePageBase
	{
		#region Protected data members
		protected System.Web.UI.WebControls.TextBox IDTextbox;
		#endregion

		public DateTimeFormatInfo DateFormat = DateTimeFormatInfo.CurrentInfo;

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.ViewState["DATE_FORMAT"] != null)
				{
				    this.DateFormat = this.ViewState["DATE_FORMAT"] as DateTimeFormatInfo;
				}
				else
				{
					if (this.Security != null)
					{
						SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
							sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, bGetAssociatedAliases: false));

						if (site != null)
						{
							DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();

							if (dateTimeFormatInfo != null)
							{
							    this.DateFormat = dateTimeFormatInfo;
							}
						}
					}

				    this.ViewState["DATE_FORMAT"] = this.DateFormat;
				}

				this.DataBind();

				if (!this.Page.IsPostBack)
				{
				    this.IdentifierTextbox.Text = this.IATACode.ID;
				    this.CountryTextbox.Text    = this.IATACode.Country;
				    this.NameTextBox.Text       = this.IATACode.Name;
				    this.TimeZoneTextbox.Text   = this.IATACode.TimeZone;
				    this.LatitudeTextbox.Text   = this.IATACode.LatitudeStr;
				    this.LongitudeTextbox.Text  = this.IATACode.LongitudeStr;
				    this.ZoomTextbox.Text       = this.IATACode.ZoomStr;
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
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new EventHandler(this.Page_Load);
		}
		#endregion

		public void UpdateData()
		{
		    this.IATACode.ID            = this.IdentifierTextbox.Text;
		    this.IATACode.Country       = this.CountryTextbox.Text;
		    this.IATACode.Name          = this.NameTextBox.Text;
		    this.IATACode.TimeZone      = this.TimeZoneTextbox.Text;
		    this.IATACode.LatitudeStr   = this.LatitudeTextbox.Text;
		    this.IATACode.LongitudeStr  = this.LongitudeTextbox.Text;
		    this.IATACode.ZoomStr       = this.ZoomTextbox.Text;
		}
	}
}
