namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ProductUserDataPage.
	/// </summary>
	public partial class ProductUserDataPage : FMUserDataControlBase
	{
		public delegate bool UserDataFieldUpdateDataEventHandler(ProductUserDataPage page, UserDataFieldClass userDataField, string value);
		public delegate bool UserDataFieldPageLoadEventHandler(ProductUserDataPage page, UserDataFieldClass userDataField, WebControl control);
		private UserDataFieldUpdateDataEventHandler userDataFieldUpdateDataEventHandler;
		private UserDataFieldPageLoadEventHandler userDataFieldPageLoadEventHandler;

		protected ProductClass Product
		{
			get
			{
				return ((ProductForm) this.Page).Product;
			}
		}

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
				var newProduct = new ProductClass();
				return newProduct.EntityType;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.InitializeUserDataControls();
				}

				this.CallRICEPageLoadEventHandlers();

				this.SetFieldAccessibilityForChildRecordVersion();

				foreach (TableRow row in this.UserDataTable.Rows)
				{
					foreach (TableCell c in row.Cells)
					{
						if (c.Controls.Count > 0)
						{
							WebControl wc = (WebControl)c.Controls[0];
							if (wc != null && (wc is TextBox || wc is DropDownList))
							{
								wc.TabIndex = 1;
							}
						}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		this.userDataFieldUpdateDataEventHandler += UserDataUpdateData;
		this.userDataFieldPageLoadEventHandler += UserDataPageLoad;
		UserDataFieldUpdateDataEventHandler otherUserDataFieldUpdateDataEventHandler = AppDomain.CurrentDomain.GetData("ProductUserDataPage.UpdateData") as UserDataFieldUpdateDataEventHandler;
		UserDataFieldPageLoadEventHandler otherUserDataFieldPageLoadEventHandler = AppDomain.CurrentDomain.GetData("ProductUserDataPage.PageLoad") as UserDataFieldPageLoadEventHandler;
		if (otherUserDataFieldUpdateDataEventHandler != null)
		{
			this.userDataFieldUpdateDataEventHandler += otherUserDataFieldUpdateDataEventHandler;
		}
		if (otherUserDataFieldPageLoadEventHandler != null)
		{
			this.userDataFieldPageLoadEventHandler += otherUserDataFieldPageLoadEventHandler;
		}
	}
		#endregion

		static private bool UserDataUpdateData(ProductUserDataPage page, UserDataFieldClass userDataField, string value)
		{
			return true;
		}

		static private bool UserDataPageLoad(ProductUserDataPage page, UserDataFieldClass userDataField, WebControl control)
		{
			return true;
		}

		public void UpdateData()
		{
			if (this.Product != null)
			{
				List<string> versionSpecificFields = ((ProductForm) this.Page).VersionSpecificFields;
				bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
				bool dontPerformRvCheck = ((this.Product.IdentityGuid.Equals(Guid.Empty)
										  || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))));
				bool isBsme = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());

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
							var valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							if (this.userDataFieldUpdateDataEventHandler(this, userDataField1, valueTextBox.Text))
							{
								this.Product.UserData[userDataField1.Number] = valueTextBox.Text;
							}
						}
						else
						{
							var valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							if (this.userDataFieldUpdateDataEventHandler(this, userDataField1, valueDropDownList.SelectedValue))
							{
								this.Product.UserData[userDataField1.Number] = valueDropDownList.SelectedValue;
							}
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
								var valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								if (this.userDataFieldUpdateDataEventHandler(this, userDataField1, valueTextBox.Text))
								{
									this.Product.UserData[userDataField2.Number] = valueTextBox.Text;
								}
							}
							else
							{
								var valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								if (this.userDataFieldUpdateDataEventHandler(this, userDataField1, valueDropDownList.SelectedValue))
								{
									this.Product.UserData[userDataField2.Number] = valueDropDownList.SelectedValue;
								}
							}
						}
					}

					index++;
				}
			}
		}


		private void InitializeUserDataControls()
		{
			int index = 0;

			foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
			{
				WebControl control = this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0] as WebControl;

				if (control != null)
				{
					UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

					if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
					{
						var valueTextBox = (TextBox)control;
						valueTextBox.Text = this.Product.UserData[userDataField1.Number];
					}
					else if (control is DropDownList)
					{
						var valueDropDownList = (DropDownList)control;
						ListItem item = valueDropDownList.Items.FindByText(this.Product.UserData[userDataField1.Number]);

						if (item == null)
						{
							valueDropDownList.Items.Add(
								new ListItem(this.Product.UserData[userDataField1.Number], this.Product.UserData[userDataField1.Number]));
							valueDropDownList.SelectedIndex = valueDropDownList.Items.Count - 1;
						}
						else
						{
							valueDropDownList.SelectedIndex = valueDropDownList.Items.IndexOf(item);
						}

					}
				}

				if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
				{
					control = this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0] as WebControl;
					if (control != null)
					{
						UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

						if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
						{
							var valueTextBox = (TextBox)control;
							valueTextBox.Text = this.Product.UserData[userDataField2.Number];
						}
						else if (control is DropDownList)
						{
							var valueDropDownList = (DropDownList)control;
							ListItem item = valueDropDownList.Items.FindByText(this.Product.UserData[userDataField2.Number]);

							if (item == null)
							{
								valueDropDownList.Items.Add(
									new ListItem(this.Product.UserData[userDataField2.Number], this.Product.UserData[userDataField2.Number]));
								valueDropDownList.SelectedIndex = valueDropDownList.Items.Count - 1;
							}
							else
							{
								valueDropDownList.SelectedIndex = valueDropDownList.Items.IndexOf(item);
							}
						}
					}
				}

				index++;
			}
		}
		private void CallRICEPageLoadEventHandlers()
		{
			int index = 0;

			foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
			{
				WebControl control = this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0] as WebControl;

				if (control != null)
				{
					UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;
					this.userDataFieldPageLoadEventHandler(this, userDataField1, control);
				}

				if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
				{
					control = this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0] as WebControl;
					if (control != null)
					{
						UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;
						this.userDataFieldPageLoadEventHandler(this, userDataField2, control);
					}
				}

				index++;
			}
		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
			List<string> versionSpecificFields = ((ProductForm) this.Page).VersionSpecificFields;

			if ((this.Product.IdentityGuid.Equals(Guid.Empty)
				 || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))
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
					var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					valueTextBox.Enabled = (valueTextBox.Enabled && versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1));
				}
				else
				{
					var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					valueDropDownList.Enabled = (valueDropDownList.Enabled && versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName1));
				}

				if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
				{
					UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

					if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
					{
						var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						valueTextBox.Enabled = (valueTextBox.Enabled && versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2));
					}
					else
					{
						var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						valueDropDownList.Enabled = (valueDropDownList.Enabled && versionSpecificFields.Contains(userDataFieldDoubleColumn.UserDataFieldName2));
					}
				}

				index++;
			}
		}
	}
}
