// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FCRC_EquipmentPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FuelCardWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public partial class FCRC_EquipmentPage : FuelCardPageBase
	{
		#region Properties
		private string JavascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Assign and Unassign Button values according to Data Dictionary
					var AssignButton=document.getElementById('FCRC_EquipmentPage_AssignButton');
					if(AssignButton != null)
						AssignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Assign") + @"';
					var UnassignButton=document.getElementById('FCRC_EquipmentPage_UnassignButton');
					if(UnassignButton != null)
						UnassignButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Unassign") + @"';
				//-->
				</script>
				";
				return script;
			}
		}
		#endregion

		#region Public Methods and Operators
		public void AssignedEquipmentDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				var idLabel = (Label)e.Item.FindControl("IDLabel");

				var equipmentCollection = this.AssignedEquipmentDataGrid.DataSource as EquipmentCollectionClass;
				if (equipmentCollection != null)
				{
					EquipmentClass equipment = equipmentCollection[e.Item.DataSetIndex];

					idLabel.Text = equipment.ID;
					idLabel.ToolTip = equipment.EquipmentToolTip;
				}
			}
		}

		public void AssignedEquipmentDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.AssignedEquipmentDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.AssignedEquipmentDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		public void UpdateData()
		{
		}
		#endregion

		#region Methods
		protected void AssignEntitiesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] ds = this.AssignEntitiesTextBox.Text.Split('|');
				this.AssignEntitiesTextBox.Text = "";

				FMChannelHelper.MakeCall<IEquipments>(
					equipments =>
						{
							foreach (string id in ds)
							{
								if (id == "|")
								{
									continue;
								}

								EquipmentClass equipment = equipments.Get(this.Security, equipments.GetIdentityGuid(this.Security, id));
								this.FuelCard.EquipmentCollection.Add(equipment);
							}
						});

				this.FuelCard.EquipmentCollection.Sort();

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					for (var type = EQUIPMENT_TYPE.TRAILER_TYPE; type <= EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; type++)
					{
						ListItem item;

						if (type == EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
						{
							item = new ListItem("{All}", ((int)type).ToString(CultureInfo.InvariantCulture));
						}
						else
						{
							item = new ListItem(EquipmentTypeClass.TypeID(type), ((int)type).ToString(CultureInfo.InvariantCulture));
						}

						this.TypeDropDownList.Items.Add(item);
					}

					this.TypeDropDownList.SelectedIndex = this.TypeDropDownList.Items.Count - 1;
					this.UpdateView();
				}

				this.Page.ClientScript.RegisterStartupScript(
					this.GetType(), "CompanyCarrierPageScriptBlock", this.JavascriptStartup);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		protected void UnassignEntitiesTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				string[] ds = this.UnassignEntitiesTextBox.Text.Split('|');
				this.UnassignEntitiesTextBox.Text = "";

				foreach (string id in ds)
				{
					if (id == "|")
					{
						continue;
					}

					int index = 0;

					foreach (EquipmentClass equipment in this.FuelCard.EquipmentCollection)
					{
						if (equipment.ID == id)
						{
							this.FuelCard.EquipmentCollection.RemoveAt(index);
							break;
						}

						index++;
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void UpdateView()
		{
			if (this.TypeDropDownList.SelectedValue == "")
			{
				return;
			}

			var type = (EQUIPMENT_TYPE)Convert.ToInt32(this.TypeDropDownList.SelectedValue);

			if (type == EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
			{
				this.AssignedEquipmentDataGrid.DataSource = this.FuelCard.EquipmentCollection;
			}

			else
			{
				var equipmentCollection = new EquipmentCollectionClass();
				
				foreach (EquipmentClass equipment in this.FuelCard.EquipmentCollection)
				{
					if (equipment.Type != type)
					{
						continue;
					}

					equipmentCollection.Add(equipment);
				}

				this.AssignedEquipmentDataGrid.DataSource = equipmentCollection;
			}

			var equipmentClasses = this.AssignedEquipmentDataGrid.DataSource as List<EquipmentClass>;

			if (equipmentClasses != null)
			{
				int count = equipmentClasses.Count;

				if ((count - 1) / this.AssignedEquipmentDataGrid.PageSize < this.AssignedEquipmentDataGrid.CurrentPageIndex)
				{
					this.AssignedEquipmentDataGrid.CurrentPageIndex = (count - 1) / this.AssignedEquipmentDataGrid.PageSize;
				}
			}

			this.AssignedEquipmentDataGrid.DataBind();
		}
		#endregion
	}
}