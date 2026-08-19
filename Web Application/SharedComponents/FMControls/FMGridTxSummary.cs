/// <SUMMARY>
/// File name:	FMGridTxSummary.cs
/// Purpose:	The purpose of the FMGridTxSummary is to encapsulate functionality in creating
///				transaction summary grids. The class will utilize ListViews to build the columns
///				and use the DataView object to contain the data. It is derived from FMGrid.
///				
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
/// </SUMMARY>
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using FMBusinessObjects.DataObjects;

[assembly: TagPrefix("FMControls", "FMControls")]
namespace FMControls
{
	public class FMGridTxSummary : FMGrid
	{
		#region Private Attributes
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the FMGrid transaction base summary.
		/// </summary>
		public FMGridTxSummary()
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will create the columns for the grid. It uses the ListView classes
		/// to get the columns to create and will create the edit type columns if requested.
		/// </summary>
		/// <param name="listViewType"></param>
		/// <param name="aliasGuid"></param>
		/// <param name="productName"></param>
		public override void InitializeGridColumns(LISTVIEW_TYPE listViewType, Guid aliasGuid, string productName)
		{
			// Remove all the columns from the grid with the exception of the ones
			// specified if present (edit, delete, select).
			this.RemoveColumns();

			// Get the list view objects for the given list view and alias types.
			ListViewClass listView = base.GetListViews(listViewType, aliasGuid);

			if (listView != null)
			{
				bool firstTime = true;
				bool transIDPresent = false;

				// Get the product type.
				ProductType productType = base.GetProductType(productName);

				foreach (ListViewFieldClass listViewField in listView.ListViewFieldCollection)
				{
					if ((listViewField.DataPath == "ManagerID") ||
						(listViewField.DataPath == "OwnerID") ||
						(listViewField.DataPath == "ShipperID") ||
						(listViewField.DataPath == "BillToID") ||
						(listViewField.DataPath == "ShipToID") ||
						(listViewField.DataPath == "SupplierID") ||
						(listViewField.DataPath == "CarrierID"))
					{
						TemplateColumn column = new TemplateColumn();

						column.HeaderText = listViewField.ID;
						column.SortExpression = listViewField.DataPath;
						column.ItemTemplate = new TemplateLabelClass(listViewField.DataPath);
						this.Columns.Add(column);

						// Setup the tool tip keys.
						base.SetToolTipKeys(listViewField.DataPath);
					}
					else
					{
						System.Web.UI.WebControls.BoundColumn column = new System.Web.UI.WebControls.BoundColumn();

						column.HeaderText = listViewField.ID;
						column.DataField = listViewField.DataPath;
						column.SortExpression = listViewField.DataPath;

						if ((column.DataField == "GrossQuantity") ||
							(column.DataField == "NetQuantity") ||
							(column.DataField == "MassQuantity") ||
							(column.DataField == "LineFill") ||
							(column.DataField == "BottomVolume") ||
							(column.DataField == "NetCapacity") ||
							(column.DataField == "ReceiptVariance") ||
							(column.DataField == "LoadRackVariance"))
						{
							if (productType == ProductType.AdditiveProduct)
							{
								string DecimalFormat = new string('0', base.loginSite._AdditiveVolumeDecimalPlaces);
								column.DataFormatString = "{0:#,0." + DecimalFormat + ";(#,0." + DecimalFormat + ")}";
							}
							else
							{
								string DecimalFormat = new string('0', base.loginSite._VolumeDecimalPlaces);
								column.DataFormatString = "{0:#,0." + DecimalFormat + ";(#,0." + DecimalFormat + ")}";
							}
						}
						else if (column.DataField == "Temperature" || column.DataField == "FreezePoint")
						{
							string DecimalFormat = new string('0', base.loginSite._TemperatureDecimalPlaces);
							column.DataFormatString = "{0:#,0." + DecimalFormat + ";(#,0." + DecimalFormat + ")}";
						}
						else if (column.DataField == "Density")
						{
							string DecimalFormat = new string('0', base.loginSite._DensityDecimalPlaces);
							column.DataFormatString = "{0:#,0." + DecimalFormat + ";(#,0." + DecimalFormat + ")}";
						}
						else if (column.DataField == "DifferentialPressure")
						{
							string DecimalFormat = new string('0', base.loginSite._PressureDecimalPlaces);
							column.DataFormatString = "{0:#,0." + DecimalFormat + ";(#,0." + DecimalFormat + ")}";
						}
						else if (column.DataField == "InventoryDate"
						|| column.DataField == "EffectiveDate"
						|| column.DataField == "ExpirationDate")
						{
							column.DataFormatString = "{0:d}";
						}

						// Since the transaction ID is needed for edit and delete, we need
						// to check if it exist in the list of fields.
						if (listViewField.DataPath.ToUpper().Equals("TRANSID") == true)
						{
							transIDPresent = true;
						}

						// Add the column to the grid.
						this.Columns.Add(column);
					}

					// Set the sort expression to the first column and
					// also the direction.
					if (firstTime == true)
					{
						base.SetSortColumn(listViewField.DataPath);
						firstTime = false;
					}
				}

				// If the transaction ID was not present, then we need to create
				// a column that will contain the ID for editing and deleting.
				if (transIDPresent == false)
				{
					System.Web.UI.WebControls.BoundColumn column = new System.Web.UI.WebControls.BoundColumn();
					column.HeaderText = "TransID";
					column.DataField = "TransID";
					column.Visible = false;
					this.Columns.Add(column);
				}
			}
		}

		/// <summary>
		/// This method handles the data grid item data bound event. This event is called when
		/// the items are getting bound to the grid. During this process the tool tips for the
		/// company role columns are created.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public override void DataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
            base.DataGrid_ItemDataBound(sender, e);

			if (e.Item.ItemIndex >= 0)
			{
				var view = (DataView)this.DataSource;

				// Update the the EditLinkButton if ShowDeletedEnabled and the row contains a deleted item (25-Jun-2009 IGO)
				DataRow datarow = view.Table.Rows[e.Item.ItemIndex];
				bool deleteflag = DataObject.getValue(datarow["DeleteFlag"], false);

				if (deleteflag)
				{
					var editButton = e.Item.FindControl("FMEditLinkButton1") as FMEditLinkButton;

					if (editButton != null)
					{
						editButton.ShowDeleted = true;
					}
				}
			}
		}
		#endregion
	}
}
