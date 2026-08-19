namespace TransactionFields
{
	using System.Web.UI;
	using System.Collections;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for AGRGridGenerator.
	/// </summary>
	public class AGRGridGenerator
	{
		private DataGrid grid;
		private ArrayList columnList;
		private TransactionContext transContext;
		private TransactionDO trans;
		private TransactionFieldGenerator fieldGenerator;
		private Logger logger;

		public AGRGridGenerator ( DataGrid grid, TransactionContext transContext, TransactionDO trans, TransactionFieldGenerator fieldGenerator )
		{
			this.grid = grid;
			this.transContext = transContext;
			this.trans = trans;
			this.fieldGenerator = fieldGenerator;
			this.logger = new Logger ( "Accounting" );
		}

		public void Generate ( ) { Generate ( true ); }
		public void Generate ( bool bind )
		{
			columnList = new ArrayList ( );
			foreach (FieldClass field in transContext.aliasClass.DisplayOrder ( TRANSACTION_SECTION_TYPE.WEIGHT_READINGS ))
			{
				AddColumn ( field );
			}

			grid.ItemDataBound += this.grid_ItemDataBound;

			if (bind)
			{
				Bind ( );
			}
		}

		public void Bind ( )
		{
			var gridItemList = new ArrayList ( );

			foreach (WeightReadingDO agr in trans.WeightReadings)
			{
				gridItemList.Add ( agr );
			}

			grid.DataSource = gridItemList;
			grid.DataBind ( );
		}


		/// <summary>
		/// This method will remove all the Transaction Weight Readings 
		/// from the collection.
		/// </summary>
		public void ClearTransWeightReadings()
		{
			if (this.trans.WeightReadings != null)
			{
				this.trans.WeightReadings.Clear();
			}
		}

		protected void AddColumn ( FieldClass field )
		{
			if (( field.ID == "CloseoutDate" ))
			{
				this.logger.Warn ( "AGRGridGenerator.AddColumn(" + field.ID + ") : Invalid field." );
				return;
			}

			string id = field.ID;

			FieldGenerator colGenerator = fieldGenerator.GetFieldGenerator ( "AGR " + id );

			if (colGenerator == null)
			{
				logger.Warn ( "AGRGridGenerator.AddColumn() : " +
					"No GaugeReadingColumnGenerator found for field \"" + id + "\"." );
				return;
			}

			columnList.Add ( colGenerator );

			var col = new TemplateColumn();
			grid.Columns.Add(col);
			col.HeaderStyle.CssClass = grid.HeaderStyle.CssClass;
			col.HeaderText = "<table><tr class='" + grid.HeaderStyle.CssClass + "'><td>" + field.DisplayName + "</td>";
			col.ItemStyle.Wrap = false;

			// This will be one last check to see if the field is required.
			// There are lots of places that set the required property (such as Net and Gross fields)
			// so only change the required property if the required field is false
			if (!colGenerator.Required)
			{
				colGenerator.bFieldRequired = field.FieldRequired;
			}

			if (colGenerator.Required)
			{
				col.HeaderText += "<td style='color:red'>*</td>";
			}

			col.HeaderText += "</tr></table>";
		}

		private void grid_ItemDataBound ( object sender, DataGridItemEventArgs e )
		{
			if (e.Item.DataItem != null)
			{
				for (int cellIndex = 2; cellIndex < e.Item.Cells.Count; ++cellIndex)
				{
					TableCell cell = e.Item.Cells[cellIndex];
					int columnIndex = cellIndex - 2;
					var colGenerator = columnList[columnIndex] as FieldGenerator;

					if (( colGenerator is IWeightReadingField ) == false)
					{
						continue;
					}

					bool editable = this.grid.EditItemIndex == e.Item.ItemIndex;

					cell.ID = colGenerator.FieldID + " " + e.Item.ItemIndex;
					colGenerator.GenerateField ( cell, this.trans, this.transContext, editable, e.Item.ItemIndex );

					foreach (Control control in cell.Controls)
					{
						var webControl = control as WebControl;

						if (webControl != null)
						{
							webControl.CssClass = "tabletext";
						}

						var htmlControl = control as HtmlControl;
						if (htmlControl != null)
						{ 
							htmlControl.Attributes.Add ( "class", "tabletext" ); 
						}
					}
				}
			}
		}
	}
}
