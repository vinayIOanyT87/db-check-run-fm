namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	public partial class EquipmentTypeAircraftTanksPage : FMUserControlBase
	{
		protected EquipmentTypeClass EquipmentType
		{
			get
			{
				return ((EquipmentTypeDetailsForm)this.Page).EquipmentType;
			}
		}
		
		protected void ApplyDataDictionary()
		{
			DataGridColumnCollection columns = this.TanksDataGrid.Columns;
			for (int i = 0; i < columns.Count; i++)
			{
				string newText = this.GetTranslatedText(columns[i].HeaderText);
				columns[i].HeaderText = newText;
			}
		}

		private void UpdateTanksView()
		{
			this.TanksDataGrid.DataSource = this.EnumerateTanks();
			this.TanksDataGrid.DataBind();
			this.ApplyDataDictionary();
		}

		private ICollection EnumerateTanks()
		{
			DataTable tankDataTable = new DataTable();
			tankDataTable.Columns.Add("Index", typeof(int));
			tankDataTable.Columns.Add("Alias", typeof(string));
			tankDataTable.Columns.Add("CustomerTankID", typeof(string));
			tankDataTable.Columns.Add("Description", typeof(string));
			tankDataTable.Columns.Add("Capacity", typeof(string));
			tankDataTable.Columns.Add("Position", typeof(int));
			tankDataTable.Columns.Add("Location", typeof(string));
			tankDataTable.Columns.Add("GuiOrder", typeof(int));

			foreach (AirplaneTankClass tank in this.EquipmentType.TankCollection)
			{
				DataRow tankDataRow = tankDataTable.NewRow();
				tankDataRow["Index"] = tankDataTable.Rows.Count;
				tankDataRow["Alias"] = tank.Alias;
				tankDataRow["CustomerTankID"] = tank.CustomerTankID;
				tankDataRow["Description"] = tank.Description;
				tankDataRow["Capacity"] = tank.Capacity;
				tankDataRow["Position"] = tank.Position;
				tankDataRow["Location"] = this.GetTranslatedText(tank.Location);
				tankDataRow["GuiOrder"] = tank.GuiOrder;

				tankDataTable.Rows.Add(tankDataRow);
			}

			DataView tankDataView = new DataView(tankDataTable);
			return tankDataView;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			try
			{
				if (!this.Page.IsPostBack)
				{
					// CSI 3754 - Need to call view update only on non-postback; otherwise, the values in the controls
					// get overwritten before they can be read for saving.
					this.UpdateTanksView();

					if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
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

		/// <summary>
		/// This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				this.AddButton.Enabled = enable;
			}
		}

		protected void AddButton_Click(object sender, EventArgs e)
		{
			AirplaneTankCollectionClass tanks = this.EquipmentType.TankCollection;
			AirplaneTankClass tank = new AirplaneTankClass(this.EquipmentType.GetAirplaneTankCapacityUnit(), this.EquipmentType.GetAirplaneTankCapacityDecimalPlaces());
			tank.ParentGuid = this.EquipmentType.IdentityGuid;
			tank.ID = this.EquipmentType.ID + "_Tank_" + (tanks.Count + 1).ToString();
			tank.IdentityGuid = Guid.Empty;

			tanks.Add(tank);
			this.TanksDataGrid.CurrentPageIndex = (tanks.Count - 1) / this.TanksDataGrid.PageSize;
			this.TanksDataGrid.EditItemIndex = (tanks.Count - 1) % this.TanksDataGrid.PageSize;

			this.EnableControls(false);
			this.UpdateTanksView();
		}

		protected void TanksDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			this.TanksDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateTanksView();
		}

		protected void TanksDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			// disable edit and delete button if no security rights
			LinkButton editButton = (LinkButton)e.Item.FindControl("EditButton");
			LinkButton deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

			if ((editButton != null) && (deleteButton != null))
			{
				if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
				{
					editButton.Enabled = false;
					deleteButton.Enabled = false;
				}
			}

			if (e.Item.ItemType == ListItemType.EditItem) 
			{
				DropDownList dropDownList1 = (DropDownList)e.Item.FindControl("LocationDropDownList"); 
				DataRowView dataItem1 = (DataRowView)e.Item.DataItem; 
				dropDownList1.SelectedValue = (string)dataItem1.Row["Location"]; 
			} 
		}

		protected void TanksDataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				AirplaneTankCollectionClass tanks = this.EquipmentType.TankCollection;
				AirplaneTankClass tank = tanks[Convert.ToInt32(indexLabel.Text)];

				if (tank.IdentityGuid == Guid.Empty)
				{
					tanks.RemoveAt(Convert.ToInt32(indexLabel.Text));

					if ((this.TanksDataGrid.Items.Count == 1) && (this.TanksDataGrid.CurrentPageIndex > 0))
					{
						this.TanksDataGrid.CurrentPageIndex--;
					}
				}

				this.TanksDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateTanksView();
			}
		}

		protected void TanksDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				AirplaneTankCollectionClass tanks = this.EquipmentType.TankCollection;

				if (this.TanksDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					this.TanksDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (this.TanksDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					this.TanksDataGrid.EditItemIndex--;
				}

				tanks.RemoveAt(Convert.ToInt32(indexLabel.Text));

				if (this.TanksDataGrid.Items.Count == 1 && this.TanksDataGrid.CurrentPageIndex > 0)
				{
					this.TanksDataGrid.CurrentPageIndex--;
				}

				this.UpdateTanksView();
			}
		}

		protected void TanksDataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				Label indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					int indLbl = Convert.ToInt32(indexLabel.Text);
					AirplaneTankCollectionClass tanks = this.EquipmentType.TankCollection;
					AirplaneTankClass tank = tanks[Convert.ToInt32(indexLabel.Text)];
					TextBox aliasTextBox = (TextBox)e.Item.FindControl("AliasTextBox");
					TextBox customerTankIDTextBox = (TextBox)e.Item.FindControl("CustomerTankIDTextBox");

					if (aliasTextBox.Text.Trim().Length == 0)
					{
						throw new ApplicationException("The value for Alias is required.");
					}

					for (int i = 0; i < tanks.Count; i++)
					{
						if (i != indLbl)
						{
							if (string.Compare(tanks[i].Alias, aliasTextBox.Text, StringComparison.InvariantCultureIgnoreCase) == 0)
							{
								throw new ApplicationException("The value for Alias must be unique.");
							}

							if ( string.Compare( tanks[i].CustomerTankID, customerTankIDTextBox.Text, StringComparison.InvariantCultureIgnoreCase ) == 0 )
							{
								throw new ApplicationException("The value for Customer Tank ID must be unique.");
							}
						}
					}

					tank.Alias = aliasTextBox.Text;
					tank.CustomerTankID = customerTankIDTextBox.Text;

					TextBox descriptionTextBox = (TextBox)e.Item.FindControl("DescriptionTextBox");
					tank.Description = descriptionTextBox.Text;

					var capacityTextBox = (TextBox)e.Item.FindControl("CapacityTextBox");
					double capacity;
					if ( double.TryParse( capacityTextBox.Text, out capacity ) )
					{
						tank.Capacity = capacity.ToString( CultureInfo.InvariantCulture );
					}
					else
					{
						throw new ApplicationException( "Capacity must be a numeric value." );
					}

					int position;
					TextBox positionTextBox = (TextBox)e.Item.FindControl("PositionTextBox");
					if (int.TryParse(positionTextBox.Text, out position))
					{
						tank.Position = position;
					}
					else
					{
						throw new Exception("Position must be a numeric value.");
					}

					DropDownList locationDropDownList = (DropDownList)e.Item.FindControl("LocationDropDownList");
					tank.LocationIndex = (EQUIPMENT_TYPE_LOCATION)locationDropDownList.SelectedIndex;

					int guiOrder;
					TextBox guiOrderTextBox = (TextBox)e.Item.FindControl("GuiOrderTextBox");
					if (int.TryParse(guiOrderTextBox.Text, out guiOrder))
					{
						tank.GuiOrder = guiOrder;
					}
					else
					{
						throw new Exception("Order must be a numeric value.");
					}
					
					this.TanksDataGrid.EditItemIndex = -1;

					// If the user updates an existing Tank we don't want it to disappear if they press the cancel button. 
					// The cancel button will remove any Tank with an empty IdentityGuid.
					// Tanks that are not yet in the database will have an empty IdentityGuid
					// To solve this issue, we set the Tank's identity guid to a new guid. There's no need to worry about changing the IdentityGuid of a Tank because
					// all Tanks are deleted and re-entered when the equipment record is modified.
					tank.IdentityGuid = Guid.NewGuid();

					this.EnableControls(true);
					this.UpdateTanksView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TanksDataGrid_EdiCommand(object source, DataGridCommandEventArgs e)
		{
			this.TanksDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.EnableControls(false);
			this.UpdateTanksView();
		}

		public ICollection PopulateLocationDropDownList()
		{
			DataTable locationTable = new DataTable();
			locationTable.Columns.Add("LocationField", typeof(string));

			for (int i = 0; i < (int)EQUIPMENT_TYPE_LOCATION.MAX_EQUIPMENT_TYPE_LOCATION; i++)
			{
				DataRow locationDataRow = locationTable.NewRow();
				locationDataRow["LocationField"] = this.GetTranslatedText(AirplaneTankClass.TypeLocation((EQUIPMENT_TYPE_LOCATION)i));
				locationTable.Rows.Add(locationDataRow);
			}

			DataView locationView = new DataView(locationTable);
			return locationView;
		}

		public void SetReadOnly()
		{
			DisableControls();
		}
	}
}