// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonQualificationsPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.TrainingWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public partial class PersonQualificationsPage : QualificationPageBase
	{
		#region Properties

		protected override DataGrid MapGrid
		{
			get
			{
				return this.QualificationsDataGrid;
			}
		}

		protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				return this.Person.QualificationCollection;
			}

			set
			{
				this.Person.QualificationCollection = value;
			}
		}

		protected override QUALIFICATION_MAP_TYPE PageQualificationMapType
		{
			get
			{
				return QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON;
			}
		}

		protected override QUALIFICATION_TYPE PageQualificationType
		{
			get
			{
				return QUALIFICATION_TYPE.PERSON_QUALIFICATION;
			}
		}

		protected PersonClass Person
		{
			get
			{
				return ( (PersonForm) this.Page ).Person;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// This method overrides and implements the base class enable controls.
		/// </summary>
		/// <param name="enable">boolean indicating whether to enable the controls.
		/// </param>
		protected override void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var personForm = (PersonForm) this.Page;
			personForm.EnableControls(enable);
		}

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">
		/// The <see cref="EventArgs"/> instance containing the event data.
		/// </param>
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
				if (this.Page.IsPostBack == false)
				{
					this.UpdateQualificationsView();

					if (!this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
					    && !this.Security.HasRight(RIGHT.MODIFY_PERSON_QUALIFICATIONS))
					{
						this.AddButton.Enabled = false;
					}
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
			this.QualificationsDataGrid.EditCommand += this.QualificationsDataGridEditCommand;
			this.QualificationsDataGrid.PageIndexChanged += this.QualificationsDataGridPageIndexChanged;
			this.QualificationsDataGrid.CancelCommand += this.QualificationsDataGridCancelCommand;
			this.QualificationsDataGrid.UpdateCommand += this.QualificationsDataGridNoDueDateEditUpdateCommand;
			this.QualificationsDataGrid.DeleteCommand += this.QualificationsDataGridDeleteCommand;
			this.QualificationsDataGrid.ItemDataBound += this.QualificationsDataGridItemDataBound;
			this.AddButton.Command += this.AddButtonCommand;
		}

		#endregion
	}
}