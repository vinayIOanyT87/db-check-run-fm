namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    public partial class StationRequiredTestsandInspectionsPage : QualificationPageBase
	{
		StationClass station;
		protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.EQUIPMENT_TEST_AND_INSPECTION;

	    protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.EQUIPMENT_TEST_AND_INSPECTION_TO_STATION;

	    protected override DataGrid MapGrid => this.QualificationsDataGrid;

	    protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass maps = this.station.ReqTestsandInspectionsCollection;
				return maps;
			}
			set
			{
				this.station.ReqTestsandInspectionsCollection = value;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.station = (StationClass)this.Session["Station"];
			/*	System.Globalization.DateTimeFormatInfo d = ViewState["DATE_FORMAT"] as System.Globalization.DateTimeFormatInfo;
				if (d != null)
				{
					_dateFormat = d;
				}*/
				if (this.Page.IsPostBack == false)
				{
				/*	if (d == null && this.Security != null)
					{
						int siteIndex = (this.Station.SiteIndex == 0) ? this.Security.SiteIndex : this.Station.SiteIndex;
						SitesClass sites = new SitesClass();
						SiteClass site = sites.Get(this.Security, siteIndex);
						if (site != null)
						{
							d = site.GetDateTimeFormatInfo();
						}
					}
					if (d != null)
					{
						_dateFormat = d;
					}
					ViewState["DATE_FORMAT"] = _dateFormat;*/

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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
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