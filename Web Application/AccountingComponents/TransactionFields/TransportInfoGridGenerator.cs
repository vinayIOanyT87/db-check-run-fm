namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.DataObjects;

	public class TransportInfoGridGenerator
	{
		#region Protected data member
		protected DataGrid grid;
		protected ArrayList columnList;
		protected TransactionContext transContext;
		protected TransactionDO trans;
		protected TransactionFieldGenerator fieldGenerator;
		protected Logger logger;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transport info grid generator class.
		/// </summary>
		/// <param name="grid"></param>
		/// <param name="transContext"></param>
		/// <param name="trans"></param>
		/// <param name="fieldGenerator"></param>
		public TransportInfoGridGenerator(DataGrid grid,
										  TransactionContext transContext,
										  TransactionDO trans,
										  TransactionFieldGenerator fieldGenerator)
		{
			this.grid = grid;
			this.transContext = transContext;
			this.trans = trans;
			this.fieldGenerator = fieldGenerator;
			this.logger = new Logger("Accounting");
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will generate the grid based on the columns
		/// configured.
		/// </summary>
		public void Generate()
		{
			this.Generate(true);
		}

		/// <summary>
		/// This method will generate the grid based on the columns
		/// configured.
		/// </summary>
		/// <param name="bindGrid"></param>
		public void Generate(bool bindGrid)
		{
			this.columnList = new ArrayList();

			foreach (FieldClass field in transContext.aliasClass.DisplayOrder(TRANSACTION_SECTION_TYPE.TRANPORT_INFO))
			{
				this.AddColumn(field);
			}

			this.grid.ItemDataBound += this.grid_ItemDataBound;

			if (bindGrid)
			{
				this.Bind();
			}
		}

		/// <summary>
		/// This method will bind the data to the grid.
		/// </summary>
		public void Bind()
		{
			var gridItemList = new ArrayList();

			foreach (TransportLineItemDO transportLineItemDO in trans.TransportInfoList)
			{
				gridItemList.Add(transportLineItemDO);
			}

			this.grid.DataSource = gridItemList;
			this.grid.DataBind();
		}

		/// <summary>
		/// This method will bind the data to the grid.
		/// </summary>
		public void BindSpecial(TransactionDO inTrans)
		{
			this.trans = inTrans;
			this.Bind();
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// This method will add a column to the grid.
		/// </summary>
		/// <param name="field"></param>
		protected void AddColumn(FieldClass field)
		{
			string id = field.ID;

			FieldGenerator colGenerator = fieldGenerator.GetFieldGenerator("TransportLineItem " + id);

			if (colGenerator == null)
			{
				logger.Warn("TransportInfoGridGenerator.AddColumn() : " +
							"No TransportInfoColumnGenerator found for field \"" + id + "\".");
				return;
			}

			this.columnList.Add(colGenerator);

			var column = new TemplateColumn();
			this.grid.Columns.Add(column);

			column.HeaderStyle.CssClass = this.grid.HeaderStyle.CssClass;
			column.HeaderText = "<table><tr class='" + this.grid.HeaderStyle.CssClass + "'><td>" + field.DisplayName + "</td>";
			column.ItemStyle.Wrap = false;

			if (colGenerator.Required == false)
			{
				colGenerator.bFieldRequired = field.FieldRequired;
			}

			if (colGenerator.Required)
			{
				column.HeaderText += "<td style='color:red'>*</td>";
			}

			column.HeaderText += "</tr></table>";
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method hands the item data bound to set the cells to their
		/// initial settings.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void grid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (this.grid.EditItemIndex > -1)
			{
				var deleteBtn = (LinkButton) e.Item.FindControl("DeleteButton3");

				if (deleteBtn != null)
				{
					deleteBtn.Enabled = false;
				}
			}

			if (e.Item.DataItem != null)
			{
				for (int cellIndex = 2; cellIndex < e.Item.Cells.Count; ++cellIndex)
				{
					TableCell cell = e.Item.Cells[cellIndex];
					int columnIndex = cellIndex - 2;
					var colGenerator = columnList[columnIndex] as FieldGenerator;

					if ((colGenerator is ITransportLineItemField) == false)
					{
						continue;
					}

					bool editable = this.grid.EditItemIndex == e.Item.ItemIndex;

					cell.ID = colGenerator.FieldID + " " + e.Item.ItemIndex;
					colGenerator.GenerateField(cell, this.trans, this.transContext, editable, e.Item.ItemIndex);

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
							htmlControl.Attributes.Add("class", "tabletext");
						}
					}
				}
			}
		}
		#endregion
	}
}
