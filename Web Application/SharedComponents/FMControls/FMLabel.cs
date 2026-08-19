// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMLabel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMLabel.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
    using System;
    using System.ComponentModel;
    using System.Web.UI.WebControls;
    using FMBusinessObjects.UtilityObjects;

    /// <summary>
    /// Label control tailored for FuelsManager.
    /// </summary>
    public class FMLabel : Label
    {
        #region Private Attributes

        /// <summary>
        /// A value indicating whether the label should use data dictionary translation.
        /// Be sure to set the initial value here, the DefaultValue(true) attribute on the 
        /// </summary>
        private bool useDataDictionary = true;

        #endregion Private Attributes

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the label should use data dictionary translation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use data dictionary] (default); otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        public bool UseDataDictionary
        {
            get { return this.useDataDictionary; }
            set { this.useDataDictionary = value; }
        }

        public bool IsServiceDisrupted { get; set; } = false;
        #endregion Public Properties

        #region Methods

        /// <summary>
        /// Initialization routine override.
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
        protected void PagePreRender(object sender, EventArgs e)
        {
            if (this.IsServiceDisrupted)
            {
                return;
            }
            if (this.DesignMode == false)
            {
                if ((this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"]) && this.useDataDictionary)
                {
                    if (this.Page.Session["SiteGuid"] == null)
                    {
                        return;
                    }

                    var siteGuid = (Guid)this.Page.Session["SiteGuid"];


                    if (this.Text.Length != 0)
                    {
                        this.Text = this.Text.Trim();
                        if (this.Text[this.Text.Length - 1] == ':')
                        {
                            this.Text = this.Text.Remove(this.Text.Length - 1, 1);
                            this.Text = this.GetDataDictionaryValueByKey(siteGuid, this.Text) + ":";
                        }
                        else
                        {
                            this.Text = this.GetDataDictionaryValueByKey(siteGuid, this.Text);
                        }
                    }

                    if (this.ToolTip.Length != 0)
                    {
                        this.ToolTip = this.GetDataDictionaryValueByKey(siteGuid, this.ToolTip);
                    }

                }
                else
                {
                    // Remove translation group identifier
                    this.Text = this.Text.Substring(this.Text.IndexOf("|", StringComparison.Ordinal) + 1);
                }
            }
        }

        private string GetDataDictionaryValueByKey(Guid siteGuid, string key)
        {
            try
            {
                return DataDictionarySingleton.Get(siteGuid, key);
            }
            catch
            {
                return key;
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