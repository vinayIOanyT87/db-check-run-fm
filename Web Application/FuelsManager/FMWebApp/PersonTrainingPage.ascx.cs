namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FMControls;

	public partial class PersonTrainingPage : QualificationPageBase
	{

		protected PersonClass Person => ( (PersonForm) this.Page ).Person;

	    protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.PERSON_TRAINING;

	    protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_PERSON;

	    protected override DataGrid MapGrid => this.TrainingDataGrid;

        protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass maps = this.Person.TrainingCollection;
				return maps;
			}
			set
			{
				this.Person.TrainingCollection = value;
			}
		}
		protected List<string> VersionSpecificFields => ((PersonForm)this.Page).VersionSpecificFields;

	    override protected void QualificationsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			base.QualificationsDataGridItemDataBound(sender, e);
			var editButton = (LinkButton)e.Item.FindControl("Fmedittrainlinkbutton");

			if (editButton != null)
			{
				bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
				if (this.Person.IdentityGuid.Equals(Guid.Empty)
					  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
				{
					return;
				}

				editButton.Enabled = (editButton.Enabled && (this.VersionSpecificFields != null)
                                                  && this.VersionSpecificFields.Contains("Training"));
			}

			var deleteButton = (LinkButton)e.Item.FindControl("Fmdeletetrainlinkbutton");

			if (deleteButton != null)
			{
				bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
				if (this.Person.IdentityGuid.Equals(Guid.Empty)
					  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
				{
					return;
				}

				deleteButton.Enabled = (deleteButton.Enabled && (this.VersionSpecificFields != null)
                                                  && this.VersionSpecificFields.Contains("Training"));
			}
		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
			if (this.Person.IdentityGuid.Equals(Guid.Empty)
				  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
			{
				return;
			}

			this.AddButton.Enabled = (this.AddButton.Enabled && (this.VersionSpecificFields != null)
                                  && this.VersionSpecificFields.Contains("Training"));
		}


		protected void Page_Load ( object sender, EventArgs e )
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					this.UpdateTrainingView ( );

					if (!this.Security.HasRight ( RIGHT.MODIFY_PERSONNEL_DATA ) ||
						!this.Security.HasRight ( RIGHT.MODIFY_PERSON_TRAINING ))
					{
						this.AddButton.Enabled = false;
					}

					this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler ( except );
			}
		}

		override protected void EnableControls ( bool enable )
		{
			this.AddButton.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var personForm = (PersonForm) this.Page;
			personForm.EnableControls ( enable );
		}

		#region Web Form Designer generated code
		override protected void OnInit ( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent ( );
			base.OnInit ( e );
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ( )
		{
			this.TrainingDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler ( this.TrainingDataGridEditCommand );
			this.TrainingDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler ( this.TrainingDataGridPageIndexChanged );
			this.TrainingDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler ( this.TrainingDataGridCancelCommand );
			this.TrainingDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler ( this.TrainingDataGridNoDueDateEditUpdateCommand );
			this.TrainingDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler ( this.TrainingDataGridDeleteCommand );
			this.TrainingDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler ( this.QualificationsDataGridItemDataBound );
			this.TrainingDataGrid.ItemDataBound += new DataGridItemEventHandler ( this.LocalTrainingDataGrid_ItemDataBound );
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler ( this.AddButtonTrainingCommand );

		}

		void LocalTrainingDataGrid_ItemDataBound ( object sender, DataGridItemEventArgs e )
		{
			try
			{
				if (this.Security.HasRight ( RIGHT.MODIFY_PERSON_TRAINING ) == false
				   && e.Item.ItemIndex != -1)
				{
					FMDeleteLinkButton deleteButton = (FMDeleteLinkButton) e.Item.FindControl ( "DeleteButton" );
					if (deleteButton != null)
					{
						deleteButton.Enabled = false;
					}

					FMEditLinkButton editButton = (FMEditLinkButton) e.Item.FindControl ( "EditButton" );
					if (editButton != null)
					{
						editButton.Enabled = false;
					}

				}
			}
			catch (Exception except)
			{
				this.ErrorHandler ( except );
			}
		}
		#endregion
	}
}
