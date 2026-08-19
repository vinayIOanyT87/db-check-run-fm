namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The sites notes page allows a user to enter notes for a Site
	/// </summary>
	public partial class SiteNotesPage : FMUserControlBase
	{
		/// <summary>
		/// Loads information from the Session Site object and updates the controls on the page with the data
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">unused</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				SiteClass site = (SiteClass)this.Session["Site"];

				if (!this.Page.IsPostBack)
				{
					if (site.Note != null && !string.IsNullOrEmpty(site.Note.Note))
					{
						this.NoteText.Text = site.Note.Note;
					}
				}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the updating the Site data object with the information on the 
		/// Site notes page.
		/// </summary>
		public void UpdateData()
		{
			SiteClass site = (SiteClass)this.Session["Site"];

			// If a notes object exists, update it; otherwise, we need to create one
			NoteClass Note = site.Note;

			if (Note != null)
			{
				Note.Note = this.NoteText.Text;
			}
			else
			{
				site.Note = new NoteClass(this.NoteText.Text);
			}
		}
	}
}