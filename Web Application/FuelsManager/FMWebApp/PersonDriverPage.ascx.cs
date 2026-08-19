// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonDriverPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for PersonDriverPage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Code behind for the person driver tab page.
	/// </summary>
	public partial class PersonDriverPage : PersonPageBase
	{
		#region Public Methods and Operators

		/// <summary>
		/// Updates the data.
		/// </summary>
		public void UpdateData()
		{
			if (this.Person != null)
			{
				if (!this.Person.HasRole(PERSON_ROLE.LOADER_ROLE) && !this.Person.HasRole(PERSON_ROLE.OFFLOADER_ROLE))
				{
					return;
				}

				this.Person.Status =
					(PersonClass.STATUS)
					Convert.ToInt32((this.StatusDownList.SelectedValue == string.Empty) ? "0" : this.StatusDownList.SelectedValue);
				this.Person.AssignedEquipmentID = this.AssignedEquipmentTextBox.Text;
			}
		}
		#endregion

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
				if (this.Person == null || (!this.Person.HasRole(PERSON_ROLE.LOADER_ROLE) && !this.Person.HasRole(PERSON_ROLE.OFFLOADER_ROLE)))
				{
					return;
				}

				if (!this.Page.IsPostBack)
				{
					this.AssignedEquipmentTextBox.Text = this.Person.AssignedEquipmentID;

					string[] statusName = Enum.GetNames(typeof(PersonClass.STATUS));
					for (int i = 0; i < statusName.GetLength(0); i++)
					{
						this.StatusDownList.Items.Add(new ListItem(statusName[i], i.ToString(CultureInfo.InvariantCulture)));
					}

					this.StatusDownList.SelectedIndex = (int)this.Person.Status;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
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