// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDataGrid.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMDataGrid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;
	using System.Web.UI;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Support class for a FuelsManager data grid.
	/// </summary>
	public class FMDataGrid : DataGrid
	{
		#region Properties

		protected string[] headerTexts = null;
		protected Guid siteGuid = Guid.Empty;
		protected List<int> rowScopeIndex = new List<int>();

		public bool UseDataDictionary { get; set; }
		public virtual string RowHeaderColumn { get; set; }
		public List<int> RowScopeIndex { get { return rowScopeIndex; } }

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the FM Data Grid class.
		/// </summary>
		public FMDataGrid()
		{
			this.UseDataDictionary = true;
			this.UseAccessibleHeader = true;

		}
		#endregion

		//private void HTMLEncode(DataGridItemEventArgs e)
		//{
		//	if (e.Item.ItemType == ListItemType.AlternatingItem ||
		//		e.Item.ItemType == ListItemType.EditItem ||
		//		e.Item.ItemType == ListItemType.Item ||
		//		e.Item.ItemType == ListItemType.SelectedItem)
		//	{
		//		var rowView = e.Item.DataItem as DataRowView;

		//		if (rowView != null)
		//		{
		//			int l = rowView.Row.ItemArray.Length;

		//			for (int i = 0; i < l; i++)
		//			{
		//				var rowViewStr = rowView.Row[i] as string;

		//				if (!string.IsNullOrEmpty(rowViewStr))
		//				{
		//					rowViewStr = HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(rowViewStr));

		//					if (rowView.Row.Table.Columns[i].MaxLength < rowViewStr.Length)
		//					{
		//						try
		//						{
		//							if (rowView.Row.Table.Columns[i].MaxLength > -1)
		//							{
		//								rowView.Row.Table.Columns[i].MaxLength = rowViewStr.Length;
		//							}
		//						}
		//						catch (Exception)
		//						{
		//							rowView.Row.Table.Columns[i].MaxLength = -1;
		//						}

		//					}

		//					rowView.Row[i] = rowViewStr;
		//				}
		//			}
		//		}
		//	}
		//}


		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.WebControls.DataGrid.ItemCreated" /> event. This allows you to provide a custom handler for the event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Web.UI.WebControls.DataGridItemEventArgs" /> that contains event data.</param>
		protected override void OnItemCreated(DataGridItemEventArgs e)
		{
			//this.HTMLEncode(e);

			if (e.Item.ItemType == ListItemType.Pager)
			{
				if (e.Item.Cells.Count > 0)
				{
					int columnSpan = e.Item.Cells[0].ColumnSpan;
					e.Item.Cells[0].Attributes.Add("colspan", columnSpan.ToString(CultureInfo.InvariantCulture));
				}
			}
			else if (e.Item.ItemType == ListItemType.Header)
			{
				rowScopeIndex.Clear();
				if (!string.IsNullOrWhiteSpace(this.RowHeaderColumn))
				{
					string[] rowScopes = this.RowHeaderColumn.Split(',');

					foreach (string rs in rowScopes)
					{
						int i = 0;
						string rowScope = this.GetDataDictionaryValueByKey(rs);
						foreach (TableHeaderCell c in e.Item.Cells)
						{
							if (c.Text == rowScope)
							{
								rowScopeIndex.Add(i);
								break;
							}
							else
							{
								Control control = FMBaseDataGrid.ControlWithText(c.Controls, rowScope);
								if (control != null)
								{
									rowScopeIndex.Add(i);
									break;
								}
							}
							i++;
						}
					}
				}
			}
			else if (rowScopeIndex.Count > 0
				&& (e.Item.ItemType == ListItemType.Item
				|| e.Item.ItemType == ListItemType.AlternatingItem))
			{
				foreach (int inx in rowScopeIndex)
				{
					if (e.Item.Cells[inx].Attributes["scope"] == null)
						e.Item.Cells[inx].Attributes["scope"] = "row";
				}
			}
			base.OnItemCreated(e);
		}


		protected override void OnItemDataBound(DataGridItemEventArgs e)
		{
			base.OnItemDataBound(e);

			if (e.Item.ItemType == ListItemType.Header)
			{
				if (e.Item.Cells.Count > 0)
					headerTexts = new string[e.Item.Cells.Count];

				int i = 0;
				foreach (TableCell c in e.Item.Cells)
				{
					if (this.Columns.Count <= i || string.IsNullOrWhiteSpace(this.Columns[i].HeaderText))
					{
						string txt = FMBaseDataGrid.getCellText(c);
						headerTexts[i++] = txt;
					}
					else
					{
						headerTexts[i] = this.Columns[i++].HeaderText;
					}

				}

			}
			else
			if (e.Item.ItemType == ListItemType.Item
				|| e.Item.ItemType == ListItemType.AlternatingItem
				|| e.Item.ItemType == ListItemType.SelectedItem)
			{
				AssignAccessibilityAttributesToItem(e.Item);
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event for the <see cref="T:System.Web.UI.WebControls.BaseDataList"/> control.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
			this.Attributes["role"] = "application";
			if (this.Page.Session["SiteGuid"] == null)
			{
				return;
			}

			siteGuid = (Guid)this.Page.Session["SiteGuid"];
		}
		protected void AssignAccessibilityAttributesToItem(DataGridItem e)
		{
			//Indicate which row data cell belongs in.
			string headerText = "";

			int k = 0;
			//Row a data cell belongs in is identified with the names of the columns that have row scope concatenated 
			//with content of the current row cells under those columns.
			foreach (int inx in rowScopeIndex)
			{
				string txt = (inx < e.Cells.Count) ? FMBaseDataGrid.getCellText(e.Cells[inx]) : "";
				headerText += " " + headerTexts[inx] + " " + txt;
			}
			//In case no column with row scope specified, use "item #" to identify the row that the data cell belongs in. # is the row number.
			if (string.IsNullOrWhiteSpace(headerText.Trim()))
			{
				headerText = " item " + (e.ItemIndex + 1).ToString();
			}

			foreach (TableCell c in e.Cells)
			{

				if (!rowScopeIndex.Contains(k))
				{
					string txt = FMBaseDataGrid.getCellText(c);
					//If cell contains data (and does not have row scope), create a title or alt for each cell that will start with cell's column name 
					//concatenated with text identifying the row that the cell belongs in.
					string t = headerTexts[k] + " " + txt + " for " + headerText;
					string atxt = t.Replace(@"&nbsp;", " ");
					FMBaseDataGrid.AddAccessibility(c, atxt);//t.Replace("&nbsp;", " "));

				}
				k++;

			}
		}

		/// <summary>
		/// Called during the data grid data binding.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void FMDataGridDataBinding(object sender, EventArgs e)
		{
			if (this.DesignMode == false)
			{
				if (!this.Page.IsPostBack)
				{
					if (this.UseDataDictionary &&
						(this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"]))
					{
						foreach (DataGridColumn column in this.Columns)
						{
							if (!column.Visible)
							{
								continue;
							}

							column.HeaderText = this.GetDataDictionaryValueByKey(column.HeaderText);
						}
					}
					else
					{
						foreach (DataGridColumn column in this.Columns)
						{
							column.HeaderText = column.HeaderText.Substring(column.HeaderText.IndexOf("|", StringComparison.Ordinal) + 1);
						}
					}
				}

				// Check for no records and hide the pager
				var view = this.DataSource as DataView;
				if (view != null && view.Count == 0)
				{
					this.PagerStyle.Visible = false;
				}
				else
				{
					this.PagerStyle.Visible = true;
				}
			}
		}

		protected string GetDataDictionaryValueByKey(string p)
		{
			return DataDictionarySingleton.Get(siteGuid, p);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer = new GridHtmlTextWriter(writer);

			base.Render(writer);
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.DataBinding += this.FMDataGridDataBinding;
		}
	}
}