// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonTrainingPage.ascx.cs" company="Varec, Inc.">
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

	public partial class PersonTrainingPage : QualificationPageBase
	{
		#region Properties

		protected override DataGrid MapGrid
		{
			get
			{
				return this.TrainingDataGrid;
			}
		}

		protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				return this.Person.TrainingCollection;
			}

			set
			{
				this.Person.TrainingCollection = value;
			}
		}

		protected override QUALIFICATION_MAP_TYPE PageQualificationMapType
		{
			get
			{
				return QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;
			}
		}

		protected override QUALIFICATION_TYPE PageQualificationType
		{
			get
			{
				return QUALIFICATION_TYPE.PERSON_TRAINING;
			}
		}

		protected PersonClass Person
		{
			get
			{
				return ((PersonForm)this.Page).Person;
			}
		}

		#endregion

		#region Methods

		protected override void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var personForm = (PersonForm)this.Page;
			personForm.EnableControls(enable);
		}

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					this.UpdateTrainingView();

					if (!this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA) && !this.Security.HasRight(RIGHT.MODIFY_PERSON_TRAINING))
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
			this.TrainingDataGrid.EditCommand += this.TrainingDataGridEditCommand;
			this.TrainingDataGrid.PageIndexChanged += this.TrainingDataGridPageIndexChanged;
			this.TrainingDataGrid.CancelCommand += this.TrainingDataGridCancelCommand;
			this.TrainingDataGrid.UpdateCommand += this.TrainingDataGridNoDueDateEditUpdateCommand;
			this.TrainingDataGrid.DeleteCommand += this.TrainingDataGridDeleteCommand;
			this.TrainingDataGrid.ItemDataBound += this.QualificationsDataGridItemDataBound;
			this.AddButton.Command += this.AddButtonTrainingCommand;
		}

		#endregion
	}
}