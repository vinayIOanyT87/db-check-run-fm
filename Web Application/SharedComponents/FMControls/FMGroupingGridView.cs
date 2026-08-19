using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace FMControls
{
	/// <summary>
	/// This class provides basic grouping capability to the GridView control. 
	/// </summary>
	public class FMGroupingGridView : GridView
   {
	   //Index of column that will be assigned the row scope.
	   protected List<int> rowScopeIndex = new List<int>();
	   protected string[] headerTexts = null;

      /// <summary>
      /// Gets or sets the offset to the first column that should be grouped when 
      /// a GroupingDepth is set.
      /// </summary>
      public int GroupColumnOffset { get; set; }
	  public List<int> RowScopeIndex { get { return rowScopeIndex; } }

      public FMGroupingGridView ()
      {
         GroupColumnOffset = 0;
		 this.UseAccessibleHeader = true;
      }

      protected Table InnerTable
      {
         get
         {
            if (false == this.HasControls())
               return null;

            return (Table)this.Controls[0];
         }
      }

      public int GroupingDepth
      {
         get
         {
            object val = this.ViewState["GroupingDepth"];
            if (null == val)
            {
               return 0;
            }

            return (int)val;
         }
         set
         {
            if (value < 0)
               throw new ArgumentOutOfRangeException( "value", "value must be greater than or equal to zero" );

            this.ViewState["GroupingDepth"] = value;

         }
      }

      protected override void OnInit ( EventArgs e )
      {
         base.OnInit( e );
         InitializeComponent();
      }

      private void InitializeComponent ()
      {
         RowDataBound += new GridViewRowEventHandler( FMGroupingGridView_RowDataBound );
      }


      void FMGroupingGridView_RowDataBound ( object sender, GridViewRowEventArgs e )
      {
         if (GroupingDepth > 0)
         {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
               foreach (TableCell tc in e.Row.Cells)
               {
                  tc.Attributes["style"] = "border-color:black";
               }
            }

         }
		 if (e.Row.RowType == DataControlRowType.DataRow)
		 {
			 ;
		 }
      }

      protected override void OnDataBound ( EventArgs e )
      {
         base.OnDataBound( e );
         this.SpanCellsRecursive( 0, 0, this.Rows.Count );
		 if (this.BottomPagerRow != null && BottomPagerRow.Controls.Count > 0 && BottomPagerRow.Controls[0].Controls.Count > 0)
		 {
			 Table pagerTable = (Table)this.BottomPagerRow.Controls[0].Controls[0];
			 if (pagerTable != null)
			 {
				pagerTable.Attributes.Add("role", "presentation");
				pagerTable.Attributes.Add("aria-label", "Page indexes");
			 }
		 }
		 AddRowScope();
	  }

      private void SpanCellsRecursive ( int columnIndex, int startRowIndex, int endRowIndex )
      {
         if (columnIndex >= this.GroupingDepth || (GroupColumnOffset + columnIndex) >= this.Columns.Count)
            return;

         TableCell groupStartCell = null;
         int groupStartRowIndex = startRowIndex;

         for (int i = startRowIndex; i < endRowIndex; i++)
         {
            TableCell currentCell = this.Rows[i].Cells[GroupColumnOffset + columnIndex];

            bool isNewGroup = (null == groupStartCell) || (0 != String.CompareOrdinal( currentCell.Text, groupStartCell.Text ));

            if (isNewGroup)
            {
               if (null != groupStartCell)
               {
                  SpanCellsRecursive( columnIndex + 1, groupStartRowIndex, i );
               }

               groupStartCell = currentCell;
               groupStartCell.RowSpan = 1;
               groupStartRowIndex = i;
            }
            else
            {
               currentCell.Visible = false;
               groupStartCell.RowSpan += 1;
            }
         }

         SpanCellsRecursive( columnIndex + 1, groupStartRowIndex, endRowIndex );

      }

		/// <summary>
		/// Adds ALT and Tooltips to controls.
		/// </summary>
		/// <param name="c">Control recursively searched for any that has attyributes such as ALT, ToolTip, Title, and etc.</param>
		/// <param name="t">Test that is assigned to control attributes such as ALT, ToolTip, Title, and etc.</param>
	  virtual protected void AddAccessibility(Control c, string txt)
	  {
		  const int maxLength = 96;
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

		protected override void OnRowDataBound(GridViewRowEventArgs e)
		{

			base.OnRowDataBound(e);

			if (e.Row.RowType == DataControlRowType.Header)
			{
				int i = 0;
				headerTexts = new string[e.Row.Cells.Count];
				foreach (TableCell tc in e.Row.Cells)
				{
					headerTexts[i++] = tc.Text;
				}
				rowScopeIndex.Clear();

				//Get header column names and find index of the column that will represent the row scope.
				if (!string.IsNullOrWhiteSpace(this.RowHeaderColumn))
				{
					string[] rowScopes = this.RowHeaderColumn.Split(',');

					i = 0;

					foreach (TableCell tc in e.Row.Cells)
					{
						if (tc.Controls.Count > 0)
						{
							if (tc.Controls[0] is LinkButton)
							{
								LinkButton ctl = tc.Controls[0] as LinkButton;
								headerTexts[i] = ctl.Text;
							}
							else if (tc.Controls.Count > 1 && tc.Controls[1] is Label)
							{
								Label ctl = tc.Controls[1] as Label;
								headerTexts[i] = ctl.Text;
							}
						}
			
						i++;
					}

					for (int k = 0; k < headerTexts.Length; k++)
					{
						if (rowScopes.Any(rowScope => headerTexts[k] == rowScope))
						{
							rowScopeIndex.Add(k);
						}
					}
				}

			}

		}

	   /// <summary>
	   /// Adds row scope attribute to each row, and sets the alt and titles of controls such as edit and delete buttons, etc.
	   /// </summary>
		void AddRowScope()
		{
			if (this.InnerTable != null && this.InnerTable.Rows.Count > 1 )
			{
				int i = 0;
				foreach (GridViewRow row in this.Rows)
				{
					if (row.RowType == DataControlRowType.DataRow)
					{
						if (rowScopeIndex.Count > 0)
						{
							foreach (int rsi in rowScopeIndex)
							{

								row.Cells[rsi].Attributes["scope"] = "row";
							}
							i = 0;

							foreach (TableCell c in row.Cells)
							{
								if (!rowScopeIndex.Contains(i))
								{
									//Columns that are not column scoped. Create tooltip, alt, etc with following format:
									//Header of column that has scope + row cell value of column that has scope + name of column not scoped + cell value of column not scoped.
									string txt = string.Empty;
									string headerText = "";

									if (headerTexts != null && headerTexts.Length > i)
									{
										string cellText = FMBaseDataGrid.getCellText(row.Cells[i]);
										if (cellText.Trim().StartsWith(headerTexts[i]) )
										{
											headerText = FMBaseDataGrid.getCellText(row.Cells[i]);
										}
										else
										{
											headerText = headerTexts[i] + " " + FMBaseDataGrid.getCellText(row.Cells[i]);
										}
									}
									foreach (int rsi in rowScopeIndex)
									{

										txt = FMBaseDataGrid.getCellText(row.Cells[rsi]);

										if (string.IsNullOrWhiteSpace(txt))
										{
											txt = string.Format("item {0}", row.RowIndex + 1);
										}
										else if (headerTexts != null && headerTexts.Length > rsi)
										{
											txt = headerTexts[rsi] + " " + txt;
										}

										headerText += " for " + txt;
									}
									headerText = headerText.Replace("<br/>", " ");
									AddAccessibility(this.InnerTable.Rows[row.RowIndex + 1].Controls[i], headerText);	
								}
								i++;
							}
						}
						else if (headerTexts != null)
						{
							i = 0;
							foreach (TableCell c in row.Cells)
							{
								if (headerTexts.Length > i)
								{
									string headerText = headerTexts[i] + " " + string.Format("item {0}", row.RowIndex + 1);
									AddAccessibility(this.InnerTable.Rows[row.RowIndex + 1].Controls[i], headerText);

								}
								i++;
							}
						}
					}
				}

			}
		}


		protected override void Render(HtmlTextWriter writer)
		{
			//This will change <td> elements that have scope="row" to <th> elements. 
			writer = new GridHtmlTextWriter(writer);

			base.Render(writer);
		}
	}
}
