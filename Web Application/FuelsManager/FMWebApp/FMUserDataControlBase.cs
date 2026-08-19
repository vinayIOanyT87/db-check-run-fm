namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class FMUserDataControlBase : FMUserControlBase
	{
		#region Constants and Fields
		protected List<UserDataFieldDoubleColumns> UserDataFieldDoubleColumnList;
		protected bool DoubleColumns;
        protected UserDataFieldCollectionClass userDataFieldCollection;


        protected const int CellColumnIndex1 = 2;
		protected const int CellColumnIndex2 = 6;
		#endregion

		#region Properties

		protected virtual ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.UNKNOWN;
			}
		}

		protected virtual Table Table
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region Methods

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				((FMFormBase)this.Page).GetSecurity();

				this.userDataFieldCollection =
					FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
						x => x.EnumerateByEntityType(this.Security, this.EntityType, Guid.Empty, false, false));

				bool makeNewRow = true;
				this.DoubleColumns = false;
				TableRow row = null;
				this.UserDataFieldDoubleColumnList = new List<UserDataFieldDoubleColumns>();

				if (this.userDataFieldCollection.Count > 12)
				{
					this.DoubleColumns = true;
				}

				int userDataCount = 0;

				foreach (var fieldClass in this.userDataFieldCollection)
				{
					userDataCount++;
					var userDataField = (UserDataFieldClass)fieldClass;

					if (this.DoubleColumns)
					{
						if (makeNewRow)
						{
							row = new TableRow();
							this.Table.Rows.Add(row);
							makeNewRow = false;
							var userDataFieldDoubleColumn = new UserDataFieldDoubleColumns
							                                {
								                                Column1UserDataField = userDataField,
								                                UserDataFieldName1 = "UserData" + userDataCount
							                                };
							this.UserDataFieldDoubleColumnList.Add(userDataFieldDoubleColumn);
						}
						else
						{
							makeNewRow = true;
							int lastIndex = this.UserDataFieldDoubleColumnList.Count - 1;
							UserDataFieldDoubleColumns userDataFieldDoubleColumn = this.UserDataFieldDoubleColumnList[lastIndex];
							userDataFieldDoubleColumn.Column2UserDataField = userDataField;
							userDataFieldDoubleColumn.UserDataFieldName2 = "UserData" + (userDataCount + 1);
						}
					}
					else
					{
						row = new TableRow();
						this.Table.Rows.Add(row);
						var userDataFieldDoubleColumn = new UserDataFieldDoubleColumns
						                                {
							                                Column1UserDataField = userDataField,
							                                UserDataFieldName1 = "UserData" + userDataCount
						                                };
						this.UserDataFieldDoubleColumnList.Add(userDataFieldDoubleColumn);
					}

					var labelCell = new TableCell { Width = new Unit("2in"), Height = new Unit("32px") };
					row.Cells.Add(labelCell);
					
					var userDataLabel = new Label
						                {
							                Text = userDataField.DisplayName + ":",
							                CssClass = "formfieldtitle",
							                ForeColor = Color.Black
						                };

					labelCell.Controls.Add(userDataLabel);
					var requiredCell = new TableCell { Width = new Unit("20px"), Height = new Unit("32px") };

					if (userDataField.FieldRequired)
					{
						requiredCell.Text = "*";
						requiredCell.Style.Add("color", "red");
					}

					row.Cells.Add(requiredCell);


					var valueCell = new TableCell { Width = new Unit("5in"), Height = new Unit("32px") };
					row.Cells.Add(valueCell);

					if (userDataField.UserDataType == USER_DATA_TYPE.TEXT)
					{
						var valueTextBox = new TextBox
							                {
								                CssClass = "formfield",
								                ID = "UserData" + userDataField.Number,
								                Width = new Unit("5in"),
								                MaxLength = 60,
																ToolTip = userDataField.DisplayName,
							                };
						valueCell.Controls.Add(valueTextBox);
					}
					else
					{
						var valueDropDownList = new DropDownList
							                    {
								                    CssClass = "formfield",
								                    ID = "UserData" + userDataField.Number,
								                    Width = new Unit("5in"),
																		ToolTip = userDataField.DisplayName,
							                    };
						valueCell.Controls.Add(valueDropDownList);
						foreach (UserDataListValueClass userDataListValue in userDataField.UserDataListValueCollection)
						{
							var item = new ListItem(userDataListValue.ID, userDataListValue.ID);
							valueDropDownList.Items.Add(item);
						}
					}

					var spacerCell = new TableCell { Width = new Unit("20px"), Height = new Unit("32px"), Text = "  " };
					row.Cells.Add(spacerCell);

				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion
	}

	public class UserDataFieldDoubleColumns
	{
		private UserDataFieldClass column1UserDataField;
		private UserDataFieldClass column2UserDataField;
		private string userDataFieldName1;
		private string userDataFieldName2;

		public UserDataFieldDoubleColumns()
		{
			this.column1UserDataField = null;
			this.column2UserDataField = null;
		}

		public UserDataFieldClass Column1UserDataField
		{
			get { return this.column1UserDataField; }
			set { this.column1UserDataField = value; }
		}

		public UserDataFieldClass Column2UserDataField
		{
			get { return this.column2UserDataField; }
			set { this.column2UserDataField = value; }
		}

		public string UserDataFieldName1
		{
			get { return this.userDataFieldName1; }
			set { this.userDataFieldName1 = value; }
		}

		public string UserDataFieldName2
		{
			get { return this.userDataFieldName2; }
			set { this.userDataFieldName2 = value; }
		}
	}
}