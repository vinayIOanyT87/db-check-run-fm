///=============================================================================================
/// File name:	FMGridView.cs
/// 
/// Purpose:   The purpose of the FMGridView is to encapsulate functionality in creating
///            summary grids. 
///				
/// Comments:  Copyright (C) Varec, Inc. Norcross, GA, USA, 2009
///				This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec.
///				
/// Modification History:
///   Date:          By:                  Reason:
///   ----------     -------------------- ---------------------------------------------------
///
///

using System;
using System.Collections;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using FMBusinessObjects.UtilityObjects;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{

	public class FMGridView : FMGroupingGridView
	{
		public bool ShowFooterWhenEmpty { get; set; }
		public bool AutoDetermineWidth { get; set; }

		protected bool _FixedHeaders = false;
		protected const string DATA_DICTIONARY_KEY = "DataDictionaryKey";

		public bool FixedHeaders
		{
			get { return _FixedHeaders; }
			set
			{
				_FixedHeaders = value;

				if (FixedHeaders)
				{
					HeaderStyle.CssClass = "GVFixedHeader";
					PagerStyle.CssClass = "GVFixedFooter2";
					FooterStyle.CssClass = "GVFixedFooter2";
				}
				else
				{
					HeaderStyle.CssClass = "tablecolhead";
					PagerStyle.CssClass = "pgr";
					PagerStyle.HorizontalAlign = HorizontalAlign.Center;
				}
			}
		}

		public FMGridView()
		{
			ShowHeaderWhenEmpty = true;
			ShowFooterWhenEmpty = false;
			AutoDetermineWidth = false;

			UseAccessibleHeader = true;

			AutoGenerateColumns = false;
			BorderStyle = BorderStyle.Solid;
			BackColor = Color.White;
			GridLines = GridLines.Vertical;
			BorderWidth = Unit.Pixel(1);
			CssClass = "tabletext";
			AllowPaging = true;
			CellPadding = 3;
			PageSize = 10;
			ShowFooter = false;
			ShowHeader = true;

			HeaderStyle.ForeColor = Color.White;
			HeaderStyle.Font.Bold = true;
			HeaderStyle.CssClass = "tablecolhead";
			HeaderStyle.BackColor = FMColor.HeaderBlue;
			HeaderStyle.Height = Unit.Pixel(12);

			FooterStyle.ForeColor = Color.Black;
			FooterStyle.BackColor = FMColor.HeaderBlue;

			RowStyle.ForeColor = Color.Black;
			RowStyle.BackColor = FMColor.RowGray;
			RowStyle.CssClass = "tabletext";

			AlternatingRowStyle.BackColor = FMColor.AlternateRowGray;
			AlternatingRowStyle.CssClass = "tabletext";

			PagerStyle.CssClass = "pgr";
			PagerStyle.BackColor = FMColor.HeaderBlue;
			PagerStyle.HorizontalAlign = HorizontalAlign.Center;

			SelectedRowStyle.Font.Bold = true;
			SelectedRowStyle.ForeColor = Color.White;
			SelectedRowStyle.BackColor = FMColor.SelectedRowColor;
			SelectedRowStyle.CssClass = "GVSelectedRow";

			PagerSettings.Mode = PagerButtons.Numeric;

			if (FixedHeaders)
			{
				HeaderStyle.CssClass = "GVFixedHeader";
				PagerStyle.CssClass = "GVFixedFooter2";
				FooterStyle.CssClass = "GVFixedFooter2";
			}

			EmptyDataText = "No records found";
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			PageIndexChanging += new GridViewPageEventHandler(FMGridView_PageIndexChanging);
			RowEditing += new GridViewEditEventHandler(FMGridView_RowEditing);
			RowDeleting += new GridViewDeleteEventHandler(FMGridView_RowDeleting);
			RowDeleted += new GridViewDeletedEventHandler(FMGridView_RowDeleted);
			PreRender += new EventHandler(FMGridView_PreRender);
			Sorting += new GridViewSortEventHandler(FMGridView_Sorting);
			Sorted += new EventHandler(FMGridView_Sorted);
			DataBinding += new EventHandler(FMGridView_DataBinding);
		}

		private string GetDataDictionaryValueByKey(Guid siteGuid, string p)
		{
			return DataDictionarySingleton.Get(siteGuid, p);
		}

		protected void FMGridView_DataBinding(object sender, EventArgs e)
		{
			if (Page.IsPostBack == false)
			{
				// Try catch is necessary for designer
				try
				{
					if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
					{
						Guid SiteGuid = (Guid)Page.Session["SiteGuid"];

						// Apply the data dictionary to the column headers if the dictionary
						// exists.
						foreach (DataControlField field in Columns)
						{
							field.HeaderText = this.GetDataDictionaryValueByKey(SiteGuid, field.HeaderText);
						}
					}
					else
					{
						// Remove the all characters with the exception of the column name.
						foreach (DataControlField field in Columns)
						{
							field.HeaderText = field.HeaderText.Substring(field.HeaderText.IndexOf("|") + 1);
						}
					}
				}
				catch
				{
				}
			}
		}

		void FMGridView_Sorted(object sender, EventArgs e)
		{

		}

		void FMGridView_Sorting(object sender, GridViewSortEventArgs e)
		{

		}

		void FMGridView_PreRender(object sender, EventArgs e)
		{
			GridView gv = (GridView)sender;
			GridViewRow gvr = (GridViewRow)gv.BottomPagerRow;
			if (gvr != null)
			{
				gvr.Visible = true;
			}
		}

		void FMGridView_RowDeleted(object sender, GridViewDeletedEventArgs e)
		{

		}

		void FMGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			if (EditIndex > -1)
			{
				e.Cancel = true;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			RenderPanelBegin(writer);
			base.Render(writer);
			RenderPanelEnd(writer);
		}

		public void RenderPanelBegin(HtmlTextWriter writer)
		{
			if (FixedHeaders)
			{
				if (AutoDetermineWidth)
				{
					if (Rows.Count < 12)
					{
						writer.Write("<div id=\"pnlContainer\" style=\"width:100%;overflow:auto\">");
					}
					else
					{
						writer.Write("<div id=\"pnlContainer\" style=\"height:{0};width:100%;overflow:auto\">", Height);
					}
				}
				else
				{
					// Add 20 pixels to allow for the vertical scroll bar of the <div>
					Unit newWidth = new Unit(Width.Value + 20);

					if (Rows.Count < 12)
					{
						Height = new Unit(0, UnitType.Pixel);
						writer.Write("<div id=\"pnlContainer\" style=\"width:{0};overflow:auto;\">", newWidth);
					}
					else
					{
						writer.Write("<div id=\"pnlContainer\" style=\"height:{0};width:{1};overflow:auto;\">", Height, newWidth);
					}
				}
			}
		}

		public void RenderPanelEnd(HtmlTextWriter writer)
		{
			if (FixedHeaders)
			{
				writer.Write("</div>");
			}

		}

		void FMGridView_RowEditing(object sender, GridViewEditEventArgs e)
		{
		}

		void FMGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			// Do not allow page changes when the grid is in edit mode
			if (EditIndex > -1)
			{
				e.Cancel = true;
			}
			else
			{
				PageIndex = e.NewPageIndex;
				DataBind();
			}

		}

		protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
		{
			if (DesignMode)
			{
				return base.CreateChildControls(dataSource, dataBinding);
			}

			int rows = base.CreateChildControls(dataSource, dataBinding);

			//  no data rows created, create empty table if enabled
			if (rows == 0 && (this.ShowFooterWhenEmpty || this.ShowHeaderWhenEmpty))
			{
				//  create the table
				Table table = this.CreateChildTable();

				DataControlField[] fields;
				if (this.AutoGenerateColumns)
				{
					PagedDataSource source = new PagedDataSource();
					source.DataSource = dataSource;

					System.Collections.ICollection autoGeneratedColumns = this.CreateColumns(source, true);
					fields = new DataControlField[autoGeneratedColumns.Count];
					autoGeneratedColumns.CopyTo(fields, 0);
				}
				else
				{
					fields = new DataControlField[this.Columns.Count];
					this.Columns.CopyTo(fields, 0);
				}

				if (this.ShowHeaderWhenEmpty)
				{
					//  create a new header row
					GridViewRow headerRow = base.CreateRow(-1, -1, DataControlRowType.Header, DataControlRowState.Normal);
					this.InitializeRow(headerRow, fields);

					//  add the header row to the table
					table.Rows.Add(headerRow);
				}

				//  create the empty row
				GridViewRow emptyRow = new GridViewRow(-1, -1, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
				TableCell cell = new TableCell { ColumnSpan = fields.Length, Width = Unit.Percentage(100) };

				//  respect the precedence order if both EmptyDataTemplate
				//  and EmptyDataText are both supplied ...
				if (this.EmptyDataTemplate != null)
				{
					this.EmptyDataTemplate.InstantiateIn(cell);
				}
				else if (!string.IsNullOrEmpty(this.EmptyDataText))
				{
					cell.Controls.Add(new LiteralControl(EmptyDataText));
				}

				emptyRow.Cells.Add(cell);
				table.Rows.Add(emptyRow);

				if (this.ShowFooterWhenEmpty)
				{
					//  create footer row
					GridViewRow footerRow = base.CreateRow(-1, -1, DataControlRowType.Footer, DataControlRowState.Normal);
					this.InitializeRow(footerRow, fields);

					//  add the footer to the table
					table.Rows.Add(footerRow);
				}

				this.Controls.Clear();
				this.Controls.Add(table);
			}

			return rows;
		}

		public static void CreateSortIndicator(object sender, GridViewRowEventArgs eventArg, string currentSortExpression, string currentSortDirection)
		{
			FMGridView me = sender as FMGridView;

			if (me != null && eventArg.Row.RowType == DataControlRowType.Header)
			{
				if (currentSortExpression != null)
				{
					int index = 0;

					foreach (DataControlField currentColumn in me.Columns)
					{
						if (currentColumn.SortExpression == currentSortExpression)
						{
							TableCell cell = eventArg.Row.Cells[index];
							Label sortedLabel = new Label();
							sortedLabel.Font.Name = "webdings";
							sortedLabel.Font.Size = FontUnit.XSmall;

							if ((string.IsNullOrEmpty(currentSortDirection)) || (currentSortDirection == "ASC"))
							{
								sortedLabel.Text = "6";
							}
							else
							{
								sortedLabel.Text = "5";
							}

							cell.Controls.Add(sortedLabel);
							break;
						}

						index++;
					}
				}
			}
		}
	}

	/// <summary>
	/// This makes every row of the grid view in edit mode.
	/// </summary>
	public class FMBulkEditGridView : FMGridView
	{
		protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState)
		{
			return base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState | DataControlRowState.Edit);
		}
	}
}
