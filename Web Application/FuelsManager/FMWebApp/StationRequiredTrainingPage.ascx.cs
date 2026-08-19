namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    public partial class StationRequiredTrainingPage : QualificationPageBase
	{
		StationClass station;

		protected override DataGrid MapGrid => this.TrainingDataGrid;

	    protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.PERSON_TRAINING;

	    protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_STATION;

	    protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass maps = this.station.ReqTrainingCollection;
				return maps;
			}
			set
			{
				this.station.ReqTrainingCollection = value;
			}
		}

		protected void Page_Load ( object sender, EventArgs e )
		{
			try
			{
				this.station = (StationClass)this.Session["Station"];
				if (this.Page.IsPostBack == false)
				{
					this.UpdateTrainingView ( );

					if (this.Security.HasRight ( RIGHT.MODIFY_LOAD_RACK_DATA ) == false)
					{
					    this.AddButton.Enabled = false;
					}
				}
			}
			catch (Exception except)
			{
			    this.ErrorHandler ( except );
			}
		}

		override protected void EnableControls ( bool enable )
		{
			if (this.Security.HasRight ( RIGHT.MODIFY_LOAD_RACK_DATA ))
			{
				this.AddButton.Enabled = enable;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit ( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent ( );
			base.OnInit ( e );
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ( )
		{
			this.TrainingDataGrid.EditCommand += new DataGridCommandEventHandler( this.TrainingDataGridEditCommand );
			this.TrainingDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler( this.TrainingDataGridPageIndexChanged );
			this.TrainingDataGrid.CancelCommand += new DataGridCommandEventHandler( this.TrainingDataGridCancelCommand );
			this.TrainingDataGrid.UpdateCommand += new DataGridCommandEventHandler( this.TrainingDataGridUpdateCommand );
			this.TrainingDataGrid.DeleteCommand += new DataGridCommandEventHandler( this.TrainingDataGridDeleteCommand );
			this.TrainingDataGrid.ItemDataBound += new DataGridItemEventHandler( this.QualificationsDataGridItemDataBound );
			this.AddButton.Command += new CommandEventHandler( this.AddButtonTrainingCommand );

		}
		#endregion
	}
}