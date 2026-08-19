namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///	Summary description for EquipmentTypeReqTrainingPage.
	/// </summary>
	public partial class EquipmentTypeReqTrainingPage : QualificationPageBase
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
			get{return QUALIFICATION_TYPE.PERSON_TRAINING;}
		}

		protected override QUALIFICATION_MAP_TYPE PageQualificationMapType
		{
			get { return QUALIFICATION_MAP_TYPE.PERSON_TRAINING_TO_EQUIPMENT_TYPE; }
		}

		protected override DataGrid MapGrid
		{
			get{return this.TrainingDataGrid;}
		}

		protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass Maps=this.EquipmentType.ReqTrainingCollection;
				return Maps;
			}
			set
			{
				this.EquipmentType.ReqTrainingCollection=value;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if ( this.Page.IsPostBack == false)
				{
					base.UpdateTrainingView();

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
			this.TrainingDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.TrainingDataGridEditCommand);
			this.TrainingDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.TrainingDataGridPageIndexChanged);
			this.TrainingDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.TrainingDataGridCancelCommand);
			this.TrainingDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.TrainingDataGridUpdateCommand);
			this.TrainingDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.TrainingDataGridDeleteCommand);
			this.TrainingDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.QualificationsDataGridItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonTrainingCommand);

		}
		#endregion
	}
}
