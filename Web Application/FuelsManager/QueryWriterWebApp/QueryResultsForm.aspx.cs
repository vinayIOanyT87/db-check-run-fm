// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryResultsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The query results form code behind.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.QueryWriterWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Reflection;
	using System.Text;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using AjaxControlToolkit;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;
	using FMCore;

	using FuelsManager.FMWebApp;


	/// <summary>
	/// The query results form code behind.
	/// </summary>
	public partial class QueryResultsForm : FMFormBaseAjax
	{
		#region Constants and Fields

		/// <summary>
		/// The query results additional info.
		/// </summary>
		public static string QueryResultsAdditionalInfo = "QueryWriter_Additional_Info";

		/// <summary>
		/// The query.
		/// </summary>
		public QueryClass Query;

		/// <summary>
		/// The query results data table.
		/// </summary>
		public static string QueryResultsDataTable = "QueryWriter_Results_DT";

		/// <summary>
		/// The query results sortdirection.
		/// </summary>
		protected static string QueryResultsSortdirection = "QueryWriter_Results_SortDir";

		/// <summary>
		/// The query results sortexpression.
		/// </summary>
		protected static string QueryResultsSortexpression = "QueryWriter_Results_Sort";

		/// <summary>
		/// The date time info.
		/// </summary>
		protected DateTimeFormatInfo DateTimeInfo;

		/// <summary>
		/// Set of bound column names in the query results grid view
		/// </summary>
		protected HashSet<string> boundColumnNames = new HashSet<string>();

		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether is return from edit.
		/// </summary>
		protected bool IsReturnFromEdit
		{
			get
			{
				return this.Request.GetQueryOrFormValue("Mode").DefaultIfNull(string.Empty).Equals("Returning");
			}
		}
		#endregion

		#region Public Methods and Operators
		/// <summary>
		/// The create new field.
		/// </summary>
		/// <param name="field">
		/// The field.
		/// </param>
		/// <param name="dateTimeInfo">
		/// The date time info.
		/// </param>
		/// <returns>
		/// The <see cref="BoundField"/>.
		/// </returns>
		public static BoundField CreateNewField(QueryWriterField field, DateTimeFormatInfo dateTimeInfo)
		{
			var newField = new BoundField();

			if (field.FieldType.BaseType == typeof(Enum))
			{
				newField.DataField = field.EnumFieldName;
			}
			else
			{
				newField.DataField = field.DBFieldName;
			}

			newField.HeaderText = field.DisplayName;
			newField.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
			newField.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			newField.SortExpression = field.DBFieldName;

			if (field.IsDateType())
			{
				string shortDatePattern = dateTimeInfo.ShortDatePattern.Replace("/", dateTimeInfo.DateSeparator);
				string longTimePattern = dateTimeInfo.LongTimePattern.Replace(":", dateTimeInfo.TimeSeparator);
				newField.DataFormatString = "{0:" + shortDatePattern + "   " + longTimePattern + "}";
			}
			else if (field.IsDateOnlyType())
			{
				string shortDatePattern = dateTimeInfo.ShortDatePattern.Replace("/", dateTimeInfo.DateSeparator);
				newField.DataFormatString = "{0:" + shortDatePattern + "}";
			}

			return newField;
		}

		/// <summary>
		/// The format additional information.
		/// </summary>
		/// <param name="table">
		/// The table.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string FormatAdditionalInformation(DataTable table)
		{
			DataRow row = table.Rows[0];

			var displayValue = new StringBuilder();
			displayValue.Append("<table  role=\"presentation\" aria-label=\"additional info layout\">");

			foreach (DataColumn column in table.Columns)
			{
				string name = column.ColumnName.SplitIntoWords();

				displayValue.Append(
					string.Format(
						"<tr><td class=\"formfield\">{0}:</td><td class=\"formfield\"  style=\"text-align:right\">{1}</td></tr>",
						name,
						row[column.ColumnName]));
			}

			displayValue.Append("</table>");

			return displayValue.ToString();
		}
		#endregion

		#region Methods
		/// <summary>
		/// The are groups defined.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		protected bool AreGroupsDefined()
		{
			return this.Query.DataGroups.Count > 0;
		}

		/// <summary>
		///     This method is responsible for generating the Filter Controls on the results page
		///     based on the query definition.
		/// </summary>
		protected void GenerateFilterControls()
		{
			bool newRowFlag = true;

			HtmlTableRow row = null;

			int count = 0;

			foreach (QueryFilterGroupClass filterGroup in this.Query.FilterGroups)
			{
				// If the filter group is marked as a filter
				if (filterGroup.Filter)
				{
					++count;

					QueryWriterField field = this.Query.Topic.FindFieldByID(this.Security, filterGroup.FilterID, false);

					if (field == null)
					{
						throw new ApplicationException("Result filter field not found.");
					}

					HtmlTableCell cell;
					if (newRowFlag)
					{
						row = new HtmlTableRow();
						this.FitlerControlsTable.Rows.Add(row);
					}
					else
					{
						// add a spacing cell
						cell = new HtmlTableCell { Width = "150px", InnerHtml = "&nbsp;" };
						row.Cells.Add(cell);
					}

					newRowFlag ^= true;

					// Add the label field
					cell = new HtmlTableCell { Width = "150px" };
					cell.Controls.Add(new FMLabel { CssClass = "formfieldtitle", Text = field.DisplayName + ": " });
					row.Cells.Add(cell);

					// Add the text entry field
					// If the field is a date, we need two text boxes for begin and end
					if (field.IsDateType())
					{
						this.AddDateFilterControls(filterGroup, row, count);
					}
					else
					{
						cell = new HtmlTableCell { ColSpan = 2 };

						var newBox = new TextBox { ID = "Value1" + count.ToString(CultureInfo.InvariantCulture), Width = Unit.Pixel(250) };

						if (this.IsPostBack == false)
						{
							newBox.Text = filterGroup.SaveValue1 ?? filterGroup.DefaultValue1;
						}

						cell.Controls.Add(newBox);
						row.Cells.Add(cell);
					}
				}
			}

			// If there were no filters, we need to remove the filter row and extra separation line
			if (count == 0)
			{
				this.HeaderTable.Rows.Remove(this.FilterControlsRow);
				this.HeaderTable.Rows.Remove(this.ExtraLineRow);
			}
		}

		/// <summary>
		/// The get page filters.
		/// </summary>
		/// <returns>
		/// The <see cref="QueryCriteriaPhraseCollection"/>.
		/// </returns>
		protected QueryCriteriaPhraseCollection GetPageFilters()
		{
			var criterion = new QueryCriteriaPhraseCollection();

			int count = 1;

			foreach (QueryFilterGroupClass filterGroup in this.Query.FilterGroups)
			{
				if (filterGroup.Filter)
				{
					QueryWriterField field = this.Query.Topic.FindFieldByID(this.Security, filterGroup.FilterID, false);

					// Get the control
					var value1 = (TextBox)this.FitlerControlsTable.FindControl("Value1" + count.ToString(CultureInfo.InvariantCulture));
					string actualValue = value1.Text;

					string actualValue2 = string.Empty;
					if (field.IsDateType())
					{
						var value2 = (TextBox)this.FitlerControlsTable.FindControl("Value2" + count.ToString(CultureInfo.InvariantCulture));
						actualValue2 = value2.Text.DefaultIfNull(string.Empty);
					}

					filterGroup.SaveValue1 = actualValue;
					filterGroup.SaveValue2 = actualValue2;

					if (actualValue.NotEquals(string.Empty))
					{
						var criteria = new QueryCriteriaPhrase { Field = field };

						if (field.IsDateType() && actualValue2.NotEquals(string.Empty))
						{
							criteria.Operator = QueryOperator.Between;
							criteria.Value2 = actualValue2;
						}
						else
						{
							criteria.Operator = QueryOperator.Equals;
						}

						criteria.Value = actualValue;
						criteria.Conjunction = QueryAndOr.AND;

						criterion.Add(criteria);
					}

					++count;
				}
			}

			return criterion;
		}

		/// <summary>
		/// The on initialize.
		/// </summary>
		/// <param name="e">
		/// The e.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponent();
		}

		/// <summary>
		/// The page size drop down selected index changed.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		/// <summary>
		/// The page initialize.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				SiteClass currentSite =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

				this.DateTimeInfo = currentSite.GetDateTimeFormatInfo();

				if (IsPostBack == false)
				{
					string queryGuid = Request.QueryString["id"];
					Guid parsedGuid;
					if (string.IsNullOrEmpty(queryGuid) == false && Guid.TryParse(queryGuid, out parsedGuid))
					{

						QueryClass newQuery = FMChannelHelper.MakeCall<IQueries, QueryClass>(x => x.Get(Security, parsedGuid));
						if (newQuery != null && newQuery.QueryStorageGuid != Guid.Empty)
						{
							newQuery.QueryCalledFromMenu = true;
							Session.Add(QueryDefinitionForm.QuerywriterQueryObject, newQuery);
						}
					}
				}

				// Disable the Query Definition Link if the user does not have the modify right
				if (this.Security.HasRight(RIGHT.MODIFY_QUERIES) == false)
				{
					this.QueryDefinitionLink.Enabled = false;
				}

				// Get the query object out of session
				this.Query = (QueryClass)this.Session[QueryDefinitionForm.QuerywriterQueryObject];

				if (this.IsPostBack == false)
				{
					this.Session.Remove(QueryResultsSortexpression);
					this.Session.Remove(QueryResultsSortdirection);
					this.Session.Remove(QueryResultsDataTable);
					this.Session.Remove(QueryResultsAdditionalInfo);
					this.SetGridTemplate();
				}

				this.GenerateFilterControls();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The page load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.IsPostBack == false)
				{
					// If we are returing from edit, we need to refresh the data results
					if (this.IsReturnFromEdit)
					{
						this.Session.Remove(QueryResultsDataTable);
					}

					this.UpdateTitle();
					this.SetInitialPageSize();
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The query definition link click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void QueryDefinitionLinkClick(object sender, EventArgs e)
		{
			try
			{
				// Put the query object in the expected session variable
				this.Session[ManageQueriesForm.ManageQueriesObject] = this.Query;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("QueryDefinitionForm.aspx?Mode=Edit");
		}

		/// <summary>
		/// The refresh button click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void RefreshButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove(QueryResultsDataTable);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The results grid page index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void ResultsGridPageIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.UpdateView();
//				this.UpdateRecordDisplayMessage();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The results grid row command.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		/// <exception cref="NullReferenceException">
		/// Query topic reference is null.
		/// </exception>
		protected void ResultsGridRowCommand(object sender, GridViewCommandEventArgs e)
		{
			string urlValue = string.Empty;

			try
			{
				if (e.CommandName == "Edit")
				{
					int index = Convert.ToInt32(e.CommandArgument);

					var literalControl = (Literal)this.ResultsGrid.Rows[index].FindControl("EntityGuid");
					string entityGuid = literalControl.Text;

					// Get an object of the correct type
					object topicObject = Activator.CreateInstance(this.Query.Topic.ObjectType);
					MethodInfo sqlMethod = this.Query.Topic.ObjectType.GetMethod("DetailPageReference");

					if (sqlMethod == null)
					{
						throw new NullReferenceException("Query topic object detail page reference not found.");
					}

					var detailUrl = (string)sqlMethod.Invoke(topicObject, null);

					urlValue = "..\\" + detailUrl + "?QueryEdit=" + entityGuid;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			// Do the redirect outside the try/catch or a "Thread is being aborted" exception is logged in the event log.
			if (string.IsNullOrEmpty(urlValue) == false)
			{
				this.Redirect(urlValue);
			}
		}

		/// <summary>
		/// The results grid sorting.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		/// <exception cref="ApplicationException">
		/// Sorting not allow when query groups.
		/// </exception>
		protected void ResultsGridSorting(object sender, GridViewSortEventArgs e)
		{
			try
			{
				if (this.AreGroupsDefined())
				{
					e.Cancel = true;
					throw new ApplicationException("Sorting not allowed when query groups are defined");
				}
				
				this.SetSortExpression(e.SortExpression);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The set grid template.
		/// </summary>
		protected void SetGridTemplate()
		{
			if (this.AreGroupsDefined())
			{
				this.ResultsGrid.AllowSorting = false;
			}

			if (this.Query.IncludeLineNumbers)
			{
				// Show the Line Number column, which should be the first one.
				this.ResultsGrid.Columns[3].Visible = true;
			}

			if (this.Query.HasGroups)
			{
				this.ResultsGrid.AlternatingRowStyle.BackColor = FMColor.RowGray;
				this.ResultsGrid.BorderColor = Color.Black;
				this.ResultsGrid.GridLines = GridLines.Both;

				// Add grid column
				QueryWriterField field = this.Query.DataGroups[0];
				this.ResultsGrid.Columns.Add(CreateNewField(field, this.DateTimeInfo));
				this.ResultsGrid.GroupingDepth = this.Query.DataGroups.Count;

				if (this.Query.DataGroups.Count > 1)
				{
					this.ResultsGrid.Columns.Add(CreateNewField(this.Query.DataGroups[1], this.DateTimeInfo));
				}

				if (this.Query.DataGroups.Count > 2)
				{
					this.ResultsGrid.Columns.Add(CreateNewField(this.Query.DataGroups[2], this.DateTimeInfo));
				}
			}

			// Loop through the Query fields and add the columns
			this.boundColumnNames.Clear();
			foreach (QueryWriterField field in this.Query.Fields)
			{
				BoundField boundField = QueryResultsForm.CreateNewField(field, this.DateTimeInfo);
				boundColumnNames.Add(boundField.DataField);
				this.ResultsGrid.Columns.Add(boundField);
			}
		}

		/// <summary>
		/// The set initial page size.
		/// </summary>
		protected void SetInitialPageSize()
		{
			this.PageSizeDropDown.SetSelectionValue(this.Query.InitialPageSize.DefaultIfNull("10"));
		}

		/// <summary>
		/// The set sort expression.
		/// </summary>
		/// <param name="expression">
		/// The expression.
		/// </param>
		protected void SetSortExpression(string expression)
		{
			var lastDirection = (string)this.Session[QueryResultsSortdirection];

			string sortDirection = lastDirection.DefaultIfNull("DESC") == "DESC" ? "ASC" : "DESC";

			string lastSort = ((string)this.Session[QueryResultsSortexpression]).DefaultIfNull(string.Empty);

			// Default to ASC when a different column is selected
			if ((lastSort.Length < 4) || lastSort.Substring(0, lastSort.Length - 4).NotEquals(expression))
			{
				sortDirection = "ASC";
			}

			this.Session[QueryResultsSortexpression] = expression + " " + sortDirection;
			this.Session[QueryResultsSortdirection] = sortDirection;
		}

		/// <summary>
		/// The update record display message.
		/// </summary>
		protected void UpdateRecordDisplayMessage()
		{
			int startingValue = (this.ResultsGrid.PageIndex * this.ResultsGrid.PageSize) + 1;
			int endingValue = startingValue + this.ResultsGrid.PageSize - 1;

			var table = (DataTable)this.Session[QueryResultsDataTable];

			if (endingValue > table.Rows.Count)
			{
				endingValue = table.Rows.Count;
			}

			string messageTemplate = "Found {0} record(s) that match the query criteria.";
			if (table.Rows.Count > 0)
			{
				messageTemplate += "  Displaying {1} - {2}";
			}

			this.RecordsMessageLabel.Text = string.Format(messageTemplate, table.Rows.Count, startingValue, endingValue);
		}

		/// <summary>
		/// The update title.
		/// </summary>
		protected void UpdateTitle()
		{
			string title = this.Query.Title;

			if (string.IsNullOrEmpty(title))
			{
				title = "Query Results";
			}

			this.TitleLabel.Text = title;
		}

		/// <summary>
		/// The update view.
		/// </summary>
		protected void UpdateView()
		{
			try
			{
				// Do we need to refresh the data source or is one stored in session for us?
				DataTable table;

				if (this.Session[QueryResultsDataTable] != null)
				{
					table = (DataTable)this.Session[QueryResultsDataTable];
				}
				else
				{
					this.DisposeResultsTable( );

               QueryCriteriaPhraseCollection filters = this.GetPageFilters();

               DataSet ds = FMChannelHelper.MakeCall<IQueries, DataSet>(queries => queries.GetQueryResults(this.Security, this.Query, filters));
               //boundColumnNames is null and needs to be populated
               this.boundColumnNames.Clear();
               foreach (QueryWriterField field in this.Query.Fields)
               {
                  BoundField boundField = QueryResultsForm.CreateNewField(field, this.DateTimeInfo);
                  boundColumnNames.Add(boundField.DataField);
               }
               table = this.RemoveDuplicateRows(ds.Tables[0]);

               this.Session[QueryResultsDataTable] = table;

					if (ds.Tables.Count > 1)
					{
						this.Session[QueryResultsAdditionalInfo] = ds.Tables[1];
					}
				}

				this.PageSizeDropDown.SetPageSize(this.ResultsGrid, table.Rows.Count);

				var newView = new DataView(table);

				var sortExpression = (string)this.Session[QueryResultsSortexpression];
				if (sortExpression.DefaultIfNull(string.Empty).NotEquals(string.Empty))
				{
					newView.Sort = sortExpression;
				}

				// Set the date time display format for the grid columns.
				this.SetColumnFormatsForDates(newView);

				this.ResultsGrid.DataSource = newView;
				this.ResultsGrid.DataBind();

				// Set record display message
				this.UpdateRecordDisplayMessage();

				// Display additional information
				this.PopulateAdditionalInformation();
			}
			catch (OutOfMemoryException except)
			{
				this.DisposeResultsTable();
                var table = new DataTable("QueryResultsTable");
				table.Columns.Add(new DataColumn(QueryClass.LINE_NUMBER, typeof(Int32)));

				this.PageSizeDropDown.SetPageSize(this.ResultsGrid, table.Rows.Count);
				var sortExpression = (string)this.Session[QueryResultsSortexpression];

				var newView = new DataView(table);

				if (sortExpression.DefaultIfNull(string.Empty).NotEquals(string.Empty))
				{
					newView.Sort = sortExpression;
				}

				this.ResultsGrid.DataSource = new DataView(table);
				this.ResultsGrid.DataBind();
				this.Session[QueryResultsDataTable] = table;
				this.UpdateRecordDisplayMessage();

				LogErrorMessage(except.Message);

				this.ErrorHandler("FuelsManager", "Query results too large.  Please narrow results using filter criteria.");
			}
			catch (Exception except)
			{
				var table = new DataTable("QueryResultsTable");
				table.Columns.Add(new DataColumn(QueryClass.LINE_NUMBER, typeof(Int32)));
				this.PageSizeDropDown.SetPageSize(this.ResultsGrid, table.Rows.Count);
				var sortExpression = (string)this.Session[QueryResultsSortexpression];
				var newView = new DataView(table);
				if (sortExpression.DefaultIfNull(string.Empty).NotEquals(string.Empty))
				{
					newView.Sort = sortExpression;
				}
				this.ResultsGrid.DataSource = new DataView(table);
				this.ResultsGrid.DataBind();
                this.Session[QueryResultsDataTable] = table;
				this.RecordsMessageLabel.Text = "";
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The add date filter controls.
		/// </summary>
		/// <param name="filterGroup">
		/// The filter group.
		/// </param>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <param name="count">
		/// The count.
		/// </param>
		private void AddDateFilterControls(QueryFilterGroupClass filterGroup, HtmlTableRow row, int count)
		{
			var cell = new HtmlTableCell();

			var newBox = new TextBox { ID = "Value1" + count.ToString(CultureInfo.InvariantCulture), CssClass = "formfield", Width = Unit.Pixel(120) };

			if (this.IsPostBack == false)
			{
				newBox.Text = filterGroup.SaveValue1 ?? filterGroup.DefaultValue1;
			}

			cell.Controls.Add(newBox);
			row.Cells.Add(cell);

			var watermark = new TextBoxWatermarkExtender
				{
					ID = "IBEW1-" + count.ToString(CultureInfo.InvariantCulture),
					TargetControlID = "Value1" + count.ToString(CultureInfo.InvariantCulture),
					WatermarkText = string.Format("Begin ({0})", this.DateTimeInfo.ShortDatePattern),
					WatermarkCssClass = "watermarked"
				};

			cell.Controls.Add(watermark);

			cell = new HtmlTableCell();

			newBox = new TextBox { ID = "Value2" + count.ToString(CultureInfo.InvariantCulture), CssClass = "formfield", Width = Unit.Pixel(120) };

			if (this.IsPostBack == false)
			{
				newBox.Text = filterGroup.SaveValue2 ?? filterGroup.DefaultValue2;
			}

			cell.Controls.Add(newBox);
			row.Cells.Add(cell);

			watermark = new TextBoxWatermarkExtender
				{
					ID = "IBEW2-" + count.ToString(CultureInfo.InvariantCulture),
					TargetControlID = "Value2" + count.ToString(CultureInfo.InvariantCulture),
					WatermarkText = string.Format("End ({0})", this.DateTimeInfo.ShortDatePattern),
					WatermarkCssClass = "watermarked"
				};

			cell.Controls.Add(watermark);
		}

		/// <summary>
		/// The dispose results table.
		/// </summary>
		private void DisposeResultsTable()
		{
			var table = (DataTable)this.Session[QueryResultsDataTable];
			if (table != null)
			{
				this.Session.Remove(QueryResultsDataTable);
				table.Dispose();
			}

			table = (DataTable)this.Session[QueryResultsAdditionalInfo];
			if (table != null)
			{
				this.Session.Remove(QueryResultsAdditionalInfo);
				table.Dispose();
			}
		}

		/// <summary>
		/// The initialize component.
		/// </summary>
		private void InitializeComponent()
		{
			this.ResultsGrid.PageIndexChanged += this.ResultsGridPageIndexChanged;
			this.ResultsGrid.RowCommand += this.ResultsGridRowCommand;
			this.ResultsGrid.Sorting += this.ResultsGridSorting;
			this.ResultsGrid.RowDataBound += this.ResultsGridRowDataBound;
		}

		/// <summary>
		/// The populate additional information.
		/// </summary>
		private void PopulateAdditionalInformation()
		{
			if (this.Session[QueryResultsAdditionalInfo] != null)
			{
				var table = (DataTable)this.Session[QueryResultsAdditionalInfo];

				if (table.Rows.Count > 0)
				{
					this.AdditionalInformation.InnerHtml = FormatAdditionalInformation(table);
				}
			}
			else
			{
				this.AdditionalInformation.InnerText = string.Empty;
			}
		}

		/// <summary>
		/// The results grid row data bound.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void ResultsGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					var row = (DataRowView)e.Row.DataItem;
					if (row != null)
					{
						var rowType = (string)row[QueryClass.ROW_TYPE];

						if (rowType.Equals(QueryRowType.Total.ToString()))
						{
							e.Row.BackColor = FMColor.TotalRowColor;

							var edit = (FMEditLinkButton)e.Row.FindControl("EditButton");
							if (edit != null)
							{
								edit.Visible = false;
							}

							var totalLabel = (FMLabel)e.Row.FindControl("TotalText");
							if (totalLabel != null)
							{
								totalLabel.Visible = true;
								totalLabel.Text = "Total";
							}
						}
						else if (rowType.Equals(QueryRowType.Subtotal.ToString()))
						{
							e.Row.BackColor = FMColor.SubTotalRowColor;

							var edit = (FMEditLinkButton)e.Row.FindControl("EditButton");
							if (edit != null)
							{
								edit.Visible = false;
							}

							var totalLabel = (FMLabel)e.Row.FindControl("TotalText");
							if (totalLabel != null)
							{
								totalLabel.Visible = true;
								totalLabel.Text = "Sub";
							}
						}
						else
						{
							var lineLabel = (Label)e.Row.FindControl("QueryNameLabel");
							if (lineLabel != null)
							{
								lineLabel.Text = ((this.ResultsGrid.PageIndex * this.ResultsGrid.PageSize) + e.Row.RowIndex + 1).ToString(CultureInfo.InvariantCulture);
							}

							var edit = (FMEditLinkButton)e.Row.FindControl("EditButton");
							if (edit != null)
							{
								edit.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
							}
						}

						var literalControl = (Literal)e.Row.FindControl("EntityGuid");
						if (literalControl != null)
						{
							Guid entityGuid;

							if (Guid.TryParse(literalControl.Text, out entityGuid) == false)
							{
								// Some query topics will prepend the entity guid with a letter.
								// For example, the equipment maintenance log will prepend the guid with T for tanks and E for equipment.
								// If the entity guid is not a guid, we must consider the possibility that it is prepended with a letter.
								if (literalControl.Text.Length >= 2)
								{
									Guid.TryParse(literalControl.Text.Substring(1), out entityGuid);
								}	
							}

							var edit = (FMEditLinkButton)e.Row.FindControl("EditButton");
							if (edit != null)
							{
								if (this.Query.QueryOnArchiveData == true)
								{
									edit.Enabled = false;
								}
								else
								{
									edit.Enabled = entityGuid != Guid.Empty
									               || (this.Query.Topic.ObjectType.Equals(typeof(TransactionDO)) && literalControl.Text.Length > 0);
								}
							}
						}
					}
				}
				else if (e.Row.RowType == DataControlRowType.Header)
				{
                    // for transactions only, we want to overwrite if the datadictionary changed the display name 
                    // because these are defined on the transaction alias and should not be datadictionaried
					if (this.Query.Topic.ObjectType.Equals(typeof(TransactionDO)))
					{
						foreach (TableCell cell in e.Row.Cells)
						{
							BoundField field = ((DataControlFieldCell)cell).ContainingField as BoundField;
                            if (field != null) //should only be autogenerated column
                            {
                                var queryWriterField = this.Query.Fields.FindLast(x => x.DBFieldName == field.DataField);
                                if (queryWriterField != null)
                                {
                                    field.HeaderText = queryWriterField.DisplayName;
                                    cell.Text = queryWriterField.DisplayName;
                                }
                            }
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will set the date time offset columns to the correct
		/// display format.
		/// </summary>
		/// <param name="view">The data view with the grid information.</param>
		private void SetColumnFormatsForDates(DataView view)
		{
			if (view.Table == null)
			{
				return;
			}

			var dateTimePattern = "{0:" + this.DateTimeInfo.ShortDatePattern + " " + this.DateTimeInfo.ShortTimePattern + "}";
			var datePattern = "{0:" + this.DateTimeInfo.ShortDatePattern + "}";

			foreach (DataColumn column in view.Table.Columns)
			{
				if (column.DataType.Name == "DateTimeOffset")
				{
					foreach (var gridColumn in this.ResultsGrid.Columns)
					{
						if (gridColumn is BoundField)
						{
							if ((gridColumn as BoundField).DataField == column.ColumnName)
							{
								(gridColumn as BoundField).DataFormatString = dateTimePattern;
								break;
							}
						}
					}
				}
				else if (column.DataType.Name == "DateTime" || column.ColumnName.Contains("InventoryDate"))
				{
					foreach (var gridColumn in this.ResultsGrid.Columns)
					{
						if (gridColumn is BoundField)
						{
							if ((gridColumn as BoundField).DataField == column.ColumnName)
							{
								(gridColumn as BoundField).DataFormatString = datePattern;
								break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Remove duplicate rows from the query results data table based on the bound columns.
		/// To determine if a row is a duplicate first concatenate all the bound fields into a
		/// single string.  Then attempt to add the string to the computed hash set of unique rows.
		/// If the bound field string is added to the set of unique rows then it is a unique row
		/// otherwise it is a duplicate row.
		/// </summary>
		/// <param name="table">The query results data table</param>
		/// <returns>The query results data table with duplicate rows removed</returns>
		private DataTable RemoveDuplicateRows(DataTable table)
		{
			if (table == null)
			{
				return new DataTable();
			}

			HashSet<string> uniqueRows = new HashSet<string>();
			DataTable uniqueTable = table.Clone();

			foreach (DataRow row in table.Rows)
			{
				string boundFields = string.Empty;
				foreach (string dataField in this.boundColumnNames)
				{
					boundFields += row[dataField];
				}

				bool rowAdded = uniqueRows.Add(boundFields);

				if (rowAdded)
				{
					uniqueTable.ImportRow(row);
				}
			}

			return uniqueTable;
		}
		#endregion
	}
}