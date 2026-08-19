// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyNotesPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for CompanyNotesPage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Code behind for CompanyNotesPage.
	/// </summary>
	public partial class CompanyNotesPage : CompanyPageBase
	{
		#region Public Methods and Operators

		/// <summary>
		/// Updates the data.
		/// </summary>
		public void UpdateData()
		{
			this.Company.Note = this.NoteText.Text;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
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
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					if (this.Company.Note != string.Empty)
					{
						this.NoteText.Text = this.Company.Note;
					}
                    SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}



        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            System.Collections.Generic.List<string> versionSpecificFields = ((CompanyForm)Page).VersionSpecificFields;
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))))
            {
                return;
            }
            if (!NoteText.ReadOnly)
            {
                if ((versionSpecificFields == null) || !versionSpecificFields.Contains("Note"))
                    this.NoteText.ReadOnly = true;
            }
        }


		#endregion
	}
}