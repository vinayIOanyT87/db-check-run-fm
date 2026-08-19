namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///		Summary description for EquipmentTypeReqQualificationsPage.
	/// </summary>
	public partial class EquipmentTypeReqQualificationsPage : QualificationPageBase
	{

		protected EquipmentTypeClass EquipmentType
		{
			get
			{
				return ((EquipmentTypeDetailsForm) this.Page).EquipmentType;
			}
		}

		protected override QUALIFICATION_TYPE PageQualificationType
		{
			get { return QUALIFICATION_TYPE.PERSON_QUALIFICATION; }
		}

		protected override QUALIFICATION_MAP_TYPE PageQualificationMapType
		{
			get { return QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_EQUIPMENT_TYPE; }
		}

		protected override DataGrid MapGrid
		{
			get{return this.QualificationsDataGrid;}
		}

		protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass Maps=this.EquipmentType.ReqQualificationsCollection;
				return Maps;
			}
			set
			{
				this.EquipmentType.ReqQualificationsCollection=value;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					base.UpdateQualificationsView();

					if (this.Security.HasRight ( RIGHT.MODIFY_EQUIPMENT_DATA ) == false)
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
			if (this.Security.HasRight ( RIGHT.MODIFY_EQUIPMENT_DATA ) == true)
			{
				this.AddButton.Enabled = enable;
			}

			// Call the main form to disable buttons and tabs.
			EquipmentTypeDetailsForm equipmentTypeDetailsForm = (EquipmentTypeDetailsForm) this.Page;
			equipmentTypeDetailsForm.EnableControls(enable);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.QualificationsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridEditCommand);
			this.QualificationsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.QualificationsDataGridPageIndexChanged);
			this.QualificationsDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridCancelCommand);
			this.QualificationsDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridUpdateCommand);
			this.QualificationsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridDeleteCommand);
			this.QualificationsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.QualificationsDataGridItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);

		}
		#endregion
	}
}
