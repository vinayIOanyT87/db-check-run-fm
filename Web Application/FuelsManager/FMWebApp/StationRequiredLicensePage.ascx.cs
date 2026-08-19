namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    public partial class StationRequiredLicensePage : QualificationPageBase
	{
		StationClass station;

		protected override DataGrid MapGrid => this.LicenseDataGrid;

        protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.PERSON_LICENSE;

        protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_STATION;

        protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass maps = this.station.ReqLicenseCollection;
				return maps;
			}
			set
			{
				this.station.ReqLicenseCollection = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.station = (StationClass)this.Session["Station"];
				if (!this.Page.IsPostBack)
				{
				    this.UpdateQualificationsView();
					if (!this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA)) this.AddButton.Enabled = false;
				}
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}

		override protected void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				this.AddButton.Enabled = enable;
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.LicenseDataGrid.EditCommand += new DataGridCommandEventHandler(this.QualificationsDataGridEditCommand);
			this.LicenseDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.QualificationsDataGridPageIndexChanged);
			this.LicenseDataGrid.CancelCommand += new DataGridCommandEventHandler(this.QualificationsDataGridCancelCommand);
			this.LicenseDataGrid.UpdateCommand += new DataGridCommandEventHandler(this.QualificationsDataGridUpdateCommand);
			this.LicenseDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.QualificationsDataGridDeleteCommand);
			this.LicenseDataGrid.ItemDataBound += new DataGridItemEventHandler(this.QualificationsDataGridItemDataBound);
			this.AddButton.Command += new CommandEventHandler(this.AddButtonCommand);

		}
		#endregion
	}
}