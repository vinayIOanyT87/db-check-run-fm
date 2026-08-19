// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMRadioButtonList.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMRadioButtonList.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Radio button list tailored for FuelsManager
	/// </summary>
	public class FMRadioButtonList : RadioButtonList
	{
		#region Methods

		/// <summary>
		/// Initialization override for the component.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
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
		protected void PageLoad(object sender, EventArgs e)
		{
			if (this.DesignMode == false && !this.Page.IsPostBack)
			{
				if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
				{
					if (this.Page.Session["SiteGuid"] == null)
					{
						return;
					}

					var siteGuid = (Guid)this.Page.Session["SiteGuid"];

					
							foreach (ListItem item in this.Items)
							{
								item.Text = this.GetDataDictionaryValueByKey(siteGuid, item.Text);
							}
				}
				else
				{
					// Remove translation group identifier
					foreach (ListItem item in this.Items)
					{
						item.Text = item.Text.Substring(item.Text.IndexOf("|", StringComparison.Ordinal) + 1);
					}
				}
			}
		}

		private string GetDataDictionaryValueByKey(Guid siteGuid, string p)
		{
			return DataDictionarySingleton.Get(siteGuid, p);
		}

		/// <summary>
		/// Initialization routine for the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += this.PageLoad;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (string.IsNullOrEmpty(Attributes["role"]))
			{
				Attributes.Add("role", "radiogroup");
			}
		}

		#endregion
	}
}