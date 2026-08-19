using FMBusinessObjects.DataObjects;
using System;
using System.Web.UI.WebControls;

namespace FuelsManager.FMWebApp
{
    public partial class StationRequiredEquipmentTagAndLicensePage : QualificationPageBase
    {
        StationClass station;
        protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE;

        protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_STATION;

        protected override DataGrid MapGrid => this.EquipmentTagAndLicensesDataGrid;

        protected override QualificationMapCollectionClass PageMaps
        {
            get
            {
                QualificationMapCollectionClass maps = this.station.ReqEquipmentTagAndLicenseCollection;
                return maps;
            }
            set
            {
                this.station.ReqEquipmentTagAndLicenseCollection = value;
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

                    if (this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) == false)
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
            if (this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
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
            this.EquipmentTagAndLicensesDataGrid.EditCommand += new DataGridCommandEventHandler(this.QualificationsDataGridEditCommand);
            this.EquipmentTagAndLicensesDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.QualificationsDataGridPageIndexChanged);
            this.EquipmentTagAndLicensesDataGrid.CancelCommand += new DataGridCommandEventHandler(this.QualificationsDataGridCancelCommand);
            this.EquipmentTagAndLicensesDataGrid.UpdateCommand += new DataGridCommandEventHandler(this.QualificationsDataGridUpdateCommand);
            this.EquipmentTagAndLicensesDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.QualificationsDataGridDeleteCommand);
            this.EquipmentTagAndLicensesDataGrid.ItemDataBound += new DataGridItemEventHandler(this.QualificationsDataGridItemDataBound);
            this.AddButton.Command += new CommandEventHandler(this.AddButtonCommand);
        }
        #endregion
    }
}