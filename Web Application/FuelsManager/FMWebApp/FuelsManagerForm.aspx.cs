// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FuelsManagerForm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;


    /// <summary>
    /// Code behind for FuelsManagerForm.
    /// </summary>
    public partial class FuelsManagerForm : FMFormBase
	{
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

				string newText = "FuelsManager";

				if (base.useDataDictionary)
				{ 
					newText = GetDataDictionaryValueByKey(this.Security.SiteGuid, "FuelsManager"); 
				}

				this.TitleLabel.Text = newText + " - " + this.Security.SiteID;

				this.lnkHelpOverview.Text = newText + " Overview";
				this.lnkHelpFMInterface.Text = "Understanding the " + newText + " Interface";

         }
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

      /// <summary>
      /// Show Help Using FM Interface page
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
      protected void LnkHelpFMInterfaceClick(object sender, EventArgs e)
		{
			this.ucFMMenuBar.OpenHelpPage("Understanding_the_FuelsManager_Interface.htm");
		}

		/// <summary>
		/// Show Help Overview page
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void LnkHelpOverviewClick(object sender, EventArgs e)
		{
			this.ucFMMenuBar.OpenHelpPage("Overview.htm");
		}

		/// <summary>
		///   Required method for Designer support - do not modify
		///   the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		#endregion
	}
}