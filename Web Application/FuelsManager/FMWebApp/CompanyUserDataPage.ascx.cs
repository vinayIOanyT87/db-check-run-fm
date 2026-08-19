namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for CompanyUserDataPage.
	/// </summary>
	public partial class CompanyUserDataPage : FMUserDataControlBase
	{
		protected CompanyClass Company
		{
			get
			{
				return ( (CompanyForm) this.Page ).Company;
			}
		}

		protected override Table Table
		{
			get { return this.UserDataTable; }
		}

		protected override ENTITY_TYPE EntityType
		{
			get
			{
				CompanyClass company = new CompanyClass();
				return company.EntityType;
			}
		}

		protected void Page_Load ( object sender, EventArgs e )
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
							TextBox valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							valueTextBox.Text = this.Company.UserData[userDataField1.Number];
							valueTextBox.ToolTip = "User data field";
						}
						else
						{
							DropDownList valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							ListItem item = valueDropDownList.Items.FindByText(this.Company.UserData[userDataField1.Number]);
							valueDropDownList.ToolTip = "User data field";

							if (item == null)
							{
								valueDropDownList.Items.Add(
									new ListItem(this.Company.UserData[userDataField1.Number], this.Company.UserData[userDataField1.Number]));
								valueDropDownList.SelectedIndex = valueDropDownList.Items.Count - 1;
							}
							else
							{
								valueDropDownList.SelectedIndex = valueDropDownList.Items.IndexOf ( item );
							}
						}

						if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
						{
							UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

							if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
							{
								TextBox valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								valueTextBox.Text = this.Company.UserData[userDataField2.Number];
							}
							else
							{
								DropDownList valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								ListItem item = valueDropDownList.Items.FindByText(this.Company.UserData[userDataField2.Number]);

								if (item == null)
								{
									valueDropDownList.Items.Add(
										new ListItem(this.Company.UserData[userDataField2.Number], this.Company.UserData[userDataField2.Number]));
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
				this.ErrorHandler ( except );
			}
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

		}
		#endregion

		public void UpdateData ( )
		{
			if (this.Company != null)
			{
				System.Collections.Generic.List<string> versionSpecificFields = ((CompanyForm)this.Page).VersionSpecificFields;
				bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
				bool dontPerformRvCheck = ((this.Company.IdentityGuid.Equals(Guid.Empty)
										  || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))));

				int index = 0;

				foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
				{
					UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

					if (dontPerformRvCheck 
						|| ((versionSpecificFields != null)
                            && versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1)))
					{
						if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
						{
							TextBox valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							this.Company.UserData[userDataField1.Number] = valueTextBox.Text;
						}
						else
						{
							DropDownList valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							this.Company.UserData[userDataField1.Number] = valueDropDownList.SelectedValue;
						}
					}

					if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
					{
						UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

						if (dontPerformRvCheck
							|| ((versionSpecificFields != null)
                                && versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2)))
						{
							if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
							{
								TextBox valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								this.Company.UserData[userDataField2.Number] = valueTextBox.Text;
							}
							else
							{
								DropDownList valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								this.Company.UserData[userDataField2.Number] = valueDropDownList.SelectedValue;
							}
						}
					}

					index++;
				}
			}
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            System.Collections.Generic.List<string> versionSpecificFields = ((CompanyForm)this.Page).VersionSpecificFields;

            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))
                 || (versionSpecificFields == null)))
            {
                return;
            }

            int index = 0;

			foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
			{
				UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

				if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
                {
                    TextBox valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
                    valueTextBox.Enabled = (valueTextBox.Enabled 
											&& versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1));
                }
                else
                {
                    DropDownList valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
                    valueDropDownList.Enabled = (valueDropDownList.Enabled 
												&& versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1));
                }

				if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
				{
					UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

					if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
					{
						TextBox valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						valueTextBox.Enabled = (valueTextBox.Enabled 
												&& versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2));
					}
					else
					{
						DropDownList valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						valueDropDownList.Enabled = (valueDropDownList.Enabled 
													&& versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2));
					}
				}

                index++;
            }
        }
	}
}
