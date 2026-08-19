namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    public partial class StationRequiredQualificationPage : QualificationPageBase
	{
		StationClass station;
		protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.PERSON_QUALIFICATION;

	    protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_STATION;

	    protected override DataGrid MapGrid => this.QualificationsDataGrid;

        protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass maps = this.station.ReqQualificationsCollection;
				return maps;
			}
			set
			{
				this.station.ReqQualificationsCollection = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.station = (StationClass)this.Session["Station"];
				if (this.Page.IsPostBack == false)
				{
					this.UpdateQualificationsView();

					if (this.Security.HasRight ( RIGHT.MODIFY_LOAD_RACK_DATA ) == false)
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

		override protected void EnableControls(bool enable)
		{
			if (this.Security.HasRight ( RIGHT.MODIFY_LOAD_RACK_DATA ))
			{
				this.AddButton.Enabled = enable;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.QualificationsDataGrid.EditCommand += new DataGridCommandEventHandler(this.QualificationsDataGridEditCommand);
			this.QualificationsDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.QualificationsDataGridPageIndexChanged);
			this.QualificationsDataGrid.CancelCommand += new DataGridCommandEventHandler(this.QualificationsDataGridCancelCommand);
			this.QualificationsDataGrid.UpdateCommand += new DataGridCommandEventHandler(this.QualificationsDataGridUpdateCommand);
			this.QualificationsDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.QualificationsDataGridDeleteCommand);
			this.QualificationsDataGrid.ItemDataBound += new DataGridItemEventHandler(this.QualificationsDataGridItemDataBound);
			this.AddButton.Command += new CommandEventHandler(this.AddButtonCommand);

		}
		#endregion
	}
}