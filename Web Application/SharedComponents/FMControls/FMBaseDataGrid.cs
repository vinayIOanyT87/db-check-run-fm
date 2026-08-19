// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMBaseDataGrid.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMBaseDataGrid type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Base data grid object
	/// </summary>
	public class FMBaseDataGrid : DataGrid
	{

		protected List<int> rowScopeIndex = new List<int>();
		public virtual string RowHeaderColumn { get; set; }
		public List<int> RowScopeIndex { get { return rowScopeIndex; } }

		#region Public Methods and Operators

		/// <summary>
		/// The purpose of this event handler is to implement a workaround/fix to a 
		/// bug introduced in the DataGrid in .NET 2.0 where the column span value of 
		/// the automatically generated pager row is rendered incorrectly despite having 
		/// the correct value recorded. This causes a problem for implementations, like 
		/// using FuelsManager's ListView, where columns are dynamically generated for the grid. 
		/// This fix takes advantage of the HTML rule where when duplicate tags exist for an 
		/// element, the first one is honored and any others are ignored.
		/// </summary>
		/// <param name="sender">
		/// Sender object passed by event caller. 
		/// </param>
		/// <param name="e">
		/// Event arguments object passed by event caller. 
		/// </param>
		public void DataGridItemCreatedPagerFix(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Pager)
			{
				if (e.Item.Cells.Count > 0)
				{
					int columnSpan = e.Item.Cells[0].ColumnSpan;
					e.Item.Cells[0].Attributes.Add("colspan", columnSpan.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init"/> event for the <see cref="T:System.Web.UI.WebControls.BaseDataList"/> control.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			this.UseAccessibleHeader = true;
			base.OnInit(e);
			this.Attributes["role"] = "application";
			// Add override event
			this.ItemCreated += this.DataGridItemCreatedPagerFix;
		}
		protected override void OnItemCreated(DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				rowScopeIndex.Clear();
				if (!string.IsNullOrWhiteSpace(this.RowHeaderColumn))
				{
					string[] rowScopes = this.RowHeaderColumn.Split(',');

					foreach (string rs in rowScopes)
					{
						int i = 0;
						string rowScope = rs;// this.GetDataDictionaryValueByKey(rs);
						foreach (TableHeaderCell c in e.Item.Cells)
						{
							if (c.Text == rowScope)
							{
								rowScopeIndex.Add(i);
								break;
							}
							else
							{
								Control control = ControlWithText(c.Controls, rowScope);
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
			else if (e.Item.ItemType == ListItemType.Item
					|| e.Item.ItemType == ListItemType.AlternatingItem
					|| e.Item.ItemType == ListItemType.SelectedItem
					|| e.Item.ItemType == ListItemType.EditItem)
			{

				
				if (rowScopeIndex.Count > 0)
				{
					foreach (int inx in rowScopeIndex)
					{
						if (e.Item.Cells[inx].Attributes["scope"] == null)
							e.Item.Cells[inx].Attributes["scope"] = "row";
					}

				}

	
			}
		
			base.OnItemCreated(e);
		}
		static public string getCellText(Control control)
		{
			string txt = "";

			if (control is TableCell)
			{
				txt = ((TableCell)control).Text;
			}

			if (string.IsNullOrWhiteSpace(txt))
			{
				if (control is Label)
				{
					txt = ((Label)control).Text;

				}
				else if (control is TextBox)
				{
					txt = ((TextBox)control).Text;

				}
				else if (control is DropDownList)
				{
					if (((DropDownList)control).SelectedItem == null)
					{
						txt = ((DropDownList)control).Text;
					}
					else
					{
						txt = ((DropDownList)control).SelectedItem.Text;
					}

				}
				else if (control is CheckBox)
				{
					txt = (((CheckBox)control).Checked ? "Checked" : "Unchecked");
				}
				else if (control is IButtonControl)
				{
					txt = ((IButtonControl)control).Text;
				}
				else if (control is Button)
				{
					txt = ((Button)control).Text;
				}
				else if (control is LinkButton)
				{
					txt = ((LinkButton)control).Text;
				}
				else if (control is Image)
				{
					txt = ((Image)control).AlternateText;
				} 
				if (string.IsNullOrWhiteSpace(txt))
				{
					foreach (Control child in control.Controls)
					{
						txt = getCellText(child);
						if (!string.IsNullOrWhiteSpace(txt))
						{
							return txt;
						}
					}
				}

			}
			
			return txt;
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
					string txt = getCellText(c);
					//If cell contains data (and does not have row scope), create a title or alt for each cell that will start with cell's column name 
					//concatenated with text identifying the row that the cell belongs in.
					string t = headerTexts[k] + " " + txt + " for " + headerText;
					AddAccessibility(c, t.Replace("&nbsp;", " "));

				}
				k++;

			}
		}

		string[] headerTexts = null;
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
						string txt = getCellText(c); 
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

		static public void AddAccessibility(Control c, string txt)
		{
			const int maxLength = 128;
			string t = txt;
			if (t.Length > maxLength)
			{
				t = txt.Substring(0, maxLength - 4) + " ...";
			}

			if (c.GetType().GetProperty("Title") != null)
			{
				c.GetType().InvokeMember("Title", System.Reflection.BindingFlags.SetProperty, null, c, new object[1] { t });
			}
			if (c.GetType().GetProperty("ToolTip") != null)
			{
				c.GetType().InvokeMember("ToolTip", System.Reflection.BindingFlags.SetProperty, null, c, new object[1] { t });
			}
			if (c.GetType().GetProperty("AlternateText") != null)
			{
				c.GetType().InvokeMember("AlternateText", System.Reflection.BindingFlags.SetProperty, null, c, new object[1] { t });
			}
			if (c.GetType().GetProperty("Alt") != null)
			{
				c.GetType().InvokeMember("Alt", System.Reflection.BindingFlags.SetProperty, null, c, new object[1] { t });
			}

			foreach (Control x in c.Controls)
			{
				AddAccessibility(x, t);
			}
		}

		public static Control ControlWithText(ControlCollection controls, string text)
		{
			Control control = null;
			foreach (Control c in controls)
			{
				if (c.GetType().GetProperty("Text") != null)
				{
					object o = c.GetType().InvokeMember("Text", System.Reflection.BindingFlags.GetProperty, null, c, null);
					if (o is string)
					{
						string s = o as string;
						if (s == text)
						{
							return c;
						}
					}
				}
				control = ControlWithText(c.Controls, text);
				if (control != null)
				{
					return control;
				}
			}

			return control;
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer = new GridHtmlTextWriter(writer);

			base.Render(writer);
		}
		#endregion
	}

	class GridHtmlTextWriter : HtmlTextWriter
	{

		public GridHtmlTextWriter(TextWriter w)
			: base(w)
		{
			;
		}

		public GridHtmlTextWriter(TextWriter w, string t)
			: base(w, t)
		{
			;
		}

		public override void RenderBeginTag(HtmlTextWriterTag tagKey)
		{
			string attrVal;

			if (tagKey == System.Web.UI.HtmlTextWriterTag.Td && this.IsAttributeDefined(HtmlTextWriterAttribute.Scope, out attrVal))
			{
				if (attrVal == "row")
				{
					tagKey = System.Web.UI.HtmlTextWriterTag.Th;
					this.TagKey = tagKey;
					this.TagName = "Th";
					this.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "left");
				}

			}

			base.RenderBeginTag(tagKey);
		}


	}
}