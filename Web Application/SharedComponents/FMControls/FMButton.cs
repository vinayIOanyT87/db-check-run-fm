// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMButton.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Button control tailored for FuelsManager.
	/// </summary>
	public class FMButton : Button
	{
        public bool IsServiceDisrupted { get; set; } = false;
        #region Methods

        /// <summary>
        /// Gets the translated text for the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to translate.
        /// </param>
        /// <returns>
        /// Translated text.
        /// </returns>
        protected string GetTranslationText(string key)
		{
			string value = key;

			try
			{
				if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
				{
					if (this.Page.Session["SiteGuid"] != null)
					{
						var siteGuid = (Guid)this.Page.Session["SiteGuid"];

                        value =  DataDictionarySingleton.Get(siteGuid, key);
					}
				}
			}
			// ReSharper disable EmptyGeneralCatchClause
			catch (Exception)
			// ReSharper restore EmptyGeneralCatchClause
			{
			}

			return value;
		}

		/// <summary>
		/// Component initialization routine.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);

			if (string.IsNullOrEmpty(this.Style["padding-left"]))
			{
				this.Style["padding-left"] = "3px";
			}

			if (string.IsNullOrEmpty(this.Style["padding-right"]))
			{
				this.Style["padding-right"] = "3px";
			}
		}

		/// <summary>
		/// Called on page load for the component.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected void PagePreRender(object sender, EventArgs e)
		{
            if (this.IsServiceDisrupted)
            {
                return;
            }
            if (this.DesignMode == false && !this.Page.IsPostBack)
			{
				if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
				{
					this.Text = this.GetTranslationText(this.Text);

					if (this.ToolTip.Length > 0)
					{
						this.ToolTip = this.GetTranslationText(this.ToolTip);
					}
				}
				else
				{
					// Remove translation group identifier
					this.Text = this.Text.Substring(this.Text.IndexOf("|", StringComparison.Ordinal) + 1);

					if (this.ToolTip.Length != 0)
					{
						this.ToolTip = this.ToolTip.Substring(this.ToolTip.IndexOf("|", StringComparison.Ordinal) + 1);
					}
				}
			}
		}

		/// <summary>
		/// Initialization routine for the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.PreRender += this.PagePreRender;
		}
		#endregion
	}
}