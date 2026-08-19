namespace FuelsManager.FMWebApp
{
    using System;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    public partial class UserUserDataPage : FMUserDataControlBase
	{


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
							var valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							valueTextBox.Text = this.User.UserData[userDataField1.Number];
						}
						else
						{
							var valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							ListItem item = valueDropDownList.Items.FindByText(this.User.UserData[userDataField1.Number]);

							if (item == null)
							{
								valueDropDownList.Items.Add(
									new ListItem(this.User.UserData[userDataField1.Number], this.User.UserData[userDataField1.Number]));
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
								var valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								valueTextBox.Text = this.User.UserData[userDataField2.Number];
							}
							else
							{
								var valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								ListItem item = valueDropDownList.Items.FindByText(this.User.UserData[userDataField2.Number]);

								if (item == null)
								{
									valueDropDownList.Items.Add(
										new ListItem(this.User.UserData[userDataField2.Number], this.User.UserData[userDataField2.Number]));
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
					//this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected UserClass User => ((UserForm)this.Page).FMUser;

	    protected override Table Table => this.UserDataTable;

	    protected override ENTITY_TYPE EntityType
		{
			get
			{
				var user = new UserClass();
				return user.EntityType;
			}
		}


		public void UpdateData()
		{
			if (this.User != null)
			{

				int index = 0;

				foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
				{
					UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

					if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
					{
						var valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
						this.User.UserData[userDataField1.Number] = valueTextBox.Text;
					}
					else
					{
						var valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
						this.User.UserData[userDataField1.Number] = valueDropDownList.SelectedValue;
					}


					if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
					{
						UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

						if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
						{
							var valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
							this.User.UserData[userDataField2.Number] = valueTextBox.Text;
						}
						else
						{
							var valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
							this.User.UserData[userDataField2.Number] = valueDropDownList.SelectedValue;
						}

					}

					index++;
				}
			}
		}
	}
}
