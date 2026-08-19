
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FMControls;

	/// <summary>
	/// Summary description for EquipmentUserData.
	/// </summary>
	public partial class EquipmentUserDataPage : FMUserDataControlBase
	{
		protected FMLabel Label20;
		protected TextBox PersonIDTextbox;
		protected FMLabel Label21;
		protected DropDownList PersonNameDropDownList;

		protected EquipmentClass Equipment => ((EquipmentForm) this.Page).Equipment;

	    protected override Table Table => this.UserDataTable;

        protected override ENTITY_TYPE EntityType
		{
			get
			{
				var equipment = new EquipmentClass();
				return equipment.EntityType;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					int index = 0;

					foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
					{
						UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

						if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
						{
							var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							valueTextBox.Text = this.Equipment.UserData[userDataField1.Number];
						}
						else
						{
							var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							ListItem item = valueDropDownList.Items.FindByText(this.Equipment.UserData[userDataField1.Number]);

							if (item == null)
							{
								valueDropDownList.Items.Add(
									new ListItem(this.Equipment.UserData[userDataField1.Number], this.Equipment.UserData[userDataField1.Number]));
								valueDropDownList.SelectedIndex = valueDropDownList.Items.Count - 1;
							}
							else
							{
								valueDropDownList.SelectedIndex = valueDropDownList.Items.IndexOf(item);
							}
						}

						if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
						{
							UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

							if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
							{
								var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								valueTextBox.Text = this.Equipment.UserData[userDataField2.Number];
							}
							else
							{
								var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								ListItem item = valueDropDownList.Items.FindByText(this.Equipment.UserData[userDataField2.Number]);

								if (item == null)
								{
									valueDropDownList.Items.Add(
										new ListItem(this.Equipment.UserData[userDataField2.Number], this.Equipment.UserData[userDataField2.Number]));
									valueDropDownList.SelectedIndex = valueDropDownList.Items.Count - 1;
								}
								else
								{
									valueDropDownList.SelectedIndex = valueDropDownList.Items.IndexOf(item);
								}
							}
						}

						index++;
					}
					this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion


		public void UpdateData()
		{
			if (this.Equipment != null)
			{
				List<string> versionSpecificFields = ((EquipmentForm) this.Page).VersionSpecificFields;
				bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
				bool dontPerformRvCheck = (this.Equipment.IdentityGuid.Equals(Guid.Empty)
										  || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid)));

				int index = 0;

				foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
				{
					UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

					if (dontPerformRvCheck 
						|| (versionSpecificFields != null
                        && versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1)))
					{
						if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
						{
							var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							this.Equipment.UserData[userDataField1.Number] = valueTextBox.Text;
						}
						else
						{
							var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							this.Equipment.UserData[userDataField1.Number] = valueDropDownList.SelectedValue;
						}
					}

					if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
					{
						UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

						if (dontPerformRvCheck
							|| (versionSpecificFields != null
                            && versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2)))
						{
							if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
							{
								var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								this.Equipment.UserData[userDataField2.Number] = valueTextBox.Text;
							}
							else
							{
								var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								this.Equipment.UserData[userDataField2.Number] = valueDropDownList.SelectedValue;
							}
						}
					}

					index++;
				}
			}
		}


		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
			List<string> versionSpecificFields = ((EquipmentForm) this.Page).VersionSpecificFields;

			if (this.Equipment.IdentityGuid.Equals(Guid.Empty)
			    || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid))
			    || (versionSpecificFields == null))
			{
				return;
			}

			int index = 0;

			foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
			{
				UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

				if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
				{
					var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					valueTextBox.Enabled = (valueTextBox.Enabled 
											&& versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1));
				}
				else
				{
					var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					valueDropDownList.Enabled = (valueDropDownList.Enabled 
												&& versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1));
				}

				if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
				{
					UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

					if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
					{
						var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						valueTextBox.Enabled = (valueTextBox.Enabled
												&& versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2));
					}
					else
					{
						var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						valueDropDownList.Enabled = (valueDropDownList.Enabled
													&& versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2));
					}
				}

				index++;
			}
		}
	}
}

