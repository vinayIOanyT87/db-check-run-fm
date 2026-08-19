// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FCRC_UserDataPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.FuelCardWebApp
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public partial class FCRC_UserDataPage : FMUserDataControlBase
	{
		#region Properties
		protected override ENTITY_TYPE EntityType
		{
			get
			{
				var fuelCard = new FuelCardClass();
				return fuelCard.EntityType;
			}
		}

		protected override Table Table
		{
			get
			{
				return this.UserDataTable;
			}
		}
		#endregion

		#region Public Methods and Operators
		public void UpdateData()
		{
			var fuelCard = ((FCRC_DetailForm) this.Page).FuelCard;

			int index = 0;

			foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
			{
				UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

				if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
				{
					var valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					fuelCard.UserData[userDataField1.Number] = valueTextBox.Text;
				}
				else
				{
					var valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
					fuelCard.UserData[userDataField1.Number] = valueDropDownList.SelectedValue;
				}

				if (this.DoubleColumns && userDataFieldDoubleColumn.Column2UserDataField != null)
				{
					UserDataFieldClass userDataField2 = userDataFieldDoubleColumn.Column2UserDataField;

					if (userDataField2.UserDataType == USER_DATA_TYPE.TEXT)
					{
						var valueTextBox = (TextBox) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						fuelCard.UserData[userDataField2.Number] = valueTextBox.Text;
					}
					else
					{
						var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
						fuelCard.UserData[userDataField2.Number] = valueDropDownList.SelectedValue;
					}
				}

				index++;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					var fuelCard = ((FCRC_DetailForm)this.Page).FuelCard;

					int index = 0;

					foreach (UserDataFieldDoubleColumns userDataFieldDoubleColumn in this.UserDataFieldDoubleColumnList)
					{
						UserDataFieldClass userDataField1 = userDataFieldDoubleColumn.Column1UserDataField;

						if (userDataField1.UserDataType == USER_DATA_TYPE.TEXT)
						{
							var valueTextBox = (TextBox)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							valueTextBox.Text = fuelCard.UserData[userDataField1.Number];
						}
						else
						{
							var valueDropDownList = (DropDownList)this.UserDataTable.Rows[index].Cells[CellColumnIndex1].Controls[0];
							ListItem item = valueDropDownList.Items.FindByText(fuelCard.UserData[userDataField1.Number]);

							if (item == null)
							{
								valueDropDownList.Items.Add(
									new ListItem(fuelCard.UserData[userDataField1.Number], fuelCard.UserData[userDataField1.Number]));
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
								valueTextBox.Text = fuelCard.UserData[userDataField2.Number];
							}
							else
							{
								var valueDropDownList = (DropDownList) this.UserDataTable.Rows[index].Cells[CellColumnIndex2].Controls[0];
								ListItem item = valueDropDownList.Items.FindByText(fuelCard.UserData[userDataField2.Number]);

								if (item == null)
								{
									valueDropDownList.Items.Add(
										new ListItem(fuelCard.UserData[userDataField2.Number], fuelCard.UserData[userDataField2.Number]));
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
		#endregion
	}
}