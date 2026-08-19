///***************************************************************************
/// Module Name:  TankMeterAssignmentPage.aspx.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FMControls;

	/// <summary>
	/// This page allows a user to view, add, edit, and delete meters assigned to a particular tank
	/// </summary>
	public partial class TankMeterAssignmentPage : FMUserControlBase
	{
		/// <summary>
		/// Override OnInit to wire up some events
		/// </summary>
		/// <param name="e">not used</param>
		protected override void OnInit(EventArgs e)
		{
			try
			{
				this.AddButton.Click += new EventHandler(this.AddButton_Click);
				this.MeterGrid.RowUpdating += new GridViewUpdateEventHandler(this.MeterGrid_RowUpdating);
				this.MeterGrid.RowCancelingEdit += new GridViewCancelEditEventHandler(this.MeterGrid_RowCancelingEdit);
				this.MeterGrid.RowEditing += new GridViewEditEventHandler(this.MeterGrid_RowEditing);
				this.MeterGrid.RowDataBound += new GridViewRowEventHandler(this.MeterGrid_RowDataBound);
				this.MeterGrid.RowCommand += new GridViewCommandEventHandler(this.MeterGrid_RowCommand);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Perform processing when the page is loaded
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.BindData();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when the user cancels editing a meter.
		/// If the meter is one that the user has not yet saved, it is removed from the list.
		/// Otherwise, we just switch the grid out of edit mode
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">identifies the row the cancel occurred on</param>
		private void MeterGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			try
			{
				this.EnableControls(true);

				TankClass tank = this.Session["Tank"] as TankClass;

				this.MeterGrid.EditIndex = -1;

				if (tank != null)
				{
					// Get the object we have associated with the row
					MeterClass meter = tank.Meters[e.RowIndex];

					//if the meter is a new one, cancel should remove it from the list rather than just cancelling the edit
					if (string.IsNullOrEmpty(meter.ID))
					{
						tank.Meters.RemoveAt(e.RowIndex);
					}
				}

				this.BindData();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when the user saves updates to a row. We save the data they entered in the meter collection
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">the row the user saved</param>
		private void MeterGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			try
			{
				// Get the row
				GridViewRow row = this.MeterGrid.Rows[e.RowIndex];

				TankClass tank = this.Session["Tank"] as TankClass;

				if (tank != null)
				{
					// Get the object we have associated with the row
					MeterClass meter = tank.Meters[e.RowIndex];

					if (meter != null)
					{
						// save the data the user entered
						string meterID = ((FMTextBox)row.Cells[1].Controls[1]).Text;//bds

						if (string.IsNullOrEmpty(meterID))
						{
							throw new ApplicationException("Meter ID is required");
						}

						meter.ID = meterID;

						string numberOfDigitsText = ((FMTextBox)row.Cells[2].Controls[1]).Text;//bds

						meter.NumberOfDigits = MeterClass.ValidateNumberOfDigits(numberOfDigitsText);
	
						meter.RotatesBackwardsFlag = ((FMCheckBox)row.Cells[3].Controls[1]).Checked;//bds
						meter.ReceiptMeterFlag = ((FMCheckBox)row.Cells[4].Controls[1]).Checked;//bds
					}
				}

				this.EnableControls(true);

				// Reset the edit index
				this.MeterGrid.EditIndex = -1;

				// Bind data to the grid control
				this.BindData();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Bind the meters contained within the tank to the grid on the screen.
		/// </summary>
		private void BindData()
		{
			this.MeterGrid.DataSource = this.EnumerateMeters();
			this.MeterGrid.DataBind();
		}

		/// <summary>
		/// Fires when the user presses the delete button and deletes the meter from the grid
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">Identifies the row the user pressed delete on</param>
		private void MeterGrid_RowCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);

					TankClass tank = this.Session["Tank"] as TankClass;

					if (tank != null && tank.Meters != null)
					{
						tank.Meters.RemoveAt(rowIndex);
					}
                    
                    this.EnableControls(true);
				    this.MeterGrid.EditIndex = -1;
					this.BindData();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when the user presses the add button to add a new meter to the tank
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		private void AddButton_Click(object sender, EventArgs e)
		{
			try
			{
				MeterClass meter = new MeterClass();

				TankClass tank = this.Session["Tank"] as TankClass;

				if (tank != null)
				{
					tank.Meters.Add(meter);

					this.EnableControls(false);

					// The newly added row in the grid should be in edit.
					this.MeterGrid.EditIndex = tank.Meters.Count - 1;
				}

				this.BindData();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Enable or disable controls on the screen.
		/// </summary>
		/// <param name="enable"> True to enable, false to disable.</param>
		private void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
		}

		/// <summary>
		/// Fires when a row is edited. Disables the other controls on the screen, and sets the row to edit mode.
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">the row being edited</param>
		private void MeterGrid_RowEditing(object sender, GridViewEditEventArgs e)
		{
			try
			{
				this.EnableControls(false);
				this.MeterGrid.EditIndex = e.NewEditIndex;
				this.BindData();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when a row is bound to the grid. Wires up the grid row's delete button
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		private void MeterGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					FMDeleteLinkButton deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");

					if (deleteButton != null)
					{
						deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);

                        // Disable the delete button if the row is not the row in edit so users can't edit one row and then delete another
                        if (this.MeterGrid.EditIndex != -1 && this.MeterGrid.EditIndex != e.Row.RowIndex)
                        {
                            deleteButton.Enabled = false;
                        }
                        else
                        {
                            deleteButton.Enabled = true;
                        }
					}

                    // Disable the edit button if the row is not the row in edit so users can't edit one row and then edit another
				    if (e.Row.Cells.Count > 0 && e.Row.Cells[0] is DataControlFieldCell && ((DataControlFieldCell)e.Row.Cells[0]).ContainingField is FMEditCommandField)
				    {
                        FMEditCommandField editButton = ((DataControlFieldCell)e.Row.Cells[0]).ContainingField as FMEditCommandField;

                        if (editButton != null)
				        {
                            if (this.MeterGrid.EditIndex != -1 && this.MeterGrid.EditIndex != e.Row.RowIndex)
				            {
                                editButton.Enabled = false;
				            }
                            else
                            {
                                editButton.Enabled = true;
                            }
                        }                  
				    }
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// List all of the meters assigned to this tank.
		/// This is used to Bind Data to the meter assignment grid.
		/// </summary>
		/// <returns> The meters assigned to this tank in a DataView </returns>
		private ICollection EnumerateMeters()
		{
			DataView metersDataView = null;

			try
			{
				DataTable metersDataTable = new DataTable();

				metersDataTable.Columns.Add("Index", typeof(Int32));
				metersDataTable.Columns.Add("MeterID", typeof(string));
				metersDataTable.Columns.Add("NumberOfDigits", typeof(Int32));
				metersDataTable.Columns.Add("RotatesBackwardsFlag", typeof(bool));
				metersDataTable.Columns.Add("ReceiptMeterFlag", typeof(bool));

				string AddText = this.GetTranslatedText("Add");
				string EditText = this.GetTranslatedText("Edit");

				int Item = 0;

				TankClass tank = this.Session["Tank"] as TankClass;

				if (tank != null)
				{
					DataRow meterDataRow;

					foreach (MeterClass meter in tank.Meters)
					{
						meterDataRow = metersDataTable.NewRow();

						meterDataRow["Index"] = Item;
						meterDataRow["MeterID"] = meter.ID;
						meterDataRow["NumberOfDigits"] = meter.NumberOfDigits;
						meterDataRow["RotatesBackwardsFlag"] = meter.RotatesBackwardsFlag;
						meterDataRow["ReceiptMeterFlag"] = meter.ReceiptMeterFlag;

						metersDataTable.Rows.Add(meterDataRow);

						Item++;
					}
				}

				metersDataView = new DataView(metersDataTable);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return metersDataView;					
		}

	}
}