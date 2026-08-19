namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///		Summary description for SiteUserDataPage.
	/// </summary>
	public partial class SiteUserDataPage : FMUserDataControlBase
	{

		protected override Table Table
		{
			get
			{
				return this.UserDataTable;
			}
		}

		protected override ENTITY_TYPE EntityType
		{
			get
			{
				SiteClass site = new SiteClass();
				return site.EntityType;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				SiteClass site = (SiteClass) this.Session["Site"];

				if (!this.Page.IsPostBack)
				{
					int index = 0;

					foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
					{
						UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

						if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
						{
							TextBox valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							valueTextBox.Text = site.UserData[userDataField1.Number];
						}
						else
						{
							DropDownList valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							ListItem item = valueDropDownList.Items.FindByText(site.UserData[userDataField1.Number]);

							if (item == null)
							{
								valueDropDownList.Items.Add(
									new ListItem(site.UserData[userDataField1.Number], site.UserData[userDataField1.Number]));
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
								TextBox valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								valueTextBox.Text = site.UserData[userDataField2.Number];
							}
							else
							{
								DropDownList valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								ListItem item = valueDropDownList.Items.FindByText(site.UserData[userDataField2.Number]);

								if (item == null)
								{
									valueDropDownList.Items.Add(
										new ListItem(site.UserData[userDataField2.Number], site.UserData[userDataField2.Number]));
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		public void UpdateData()
		{
			SiteClass site = (SiteClass) this.Session["Site"];

			int index = 0;

			foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
			{
				UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

				if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
				{
					TextBox valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					site.UserData[userDataField1.Number] = valueTextBox.Text;
				}
				else
				{
					DropDownList valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					site.UserData[userDataField1.Number] = valueDropDownList.SelectedValue;
				}

				if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
				{
					UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

					if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
					{
						TextBox valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						site.UserData[userDataField2.Number] = valueTextBox.Text;
					}
					else
					{
						DropDownList valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						site.UserData[userDataField2.Number] = valueDropDownList.SelectedValue;
					}
				}

				index++;
			}
		}
	}
}
