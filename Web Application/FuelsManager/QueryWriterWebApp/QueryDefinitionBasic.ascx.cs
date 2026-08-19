// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryDefinitionBasic.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for basic query definition page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.QueryWriterWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;
    using FMCore;

	public partial class QueryDefinitionBasic : QueryPageBase
	{
		#region Constants and Fields

		public const string QuerywriterQueryTopic = "QueryWriter.QueryBasicDef.QueryTopic";

		protected const string QuerywriterDataView = "QueryWriter.QueryBasicDef.DataView";

		protected static QueryClass Query;

		protected QueryWriterTopic Topic;

		private const string QueryWriterExportButtonMode = "QueryWriter.QueryBasicDef.ExportButtonMode";
		private const string ConfirmProcessScript = @"
			<script type='text/javascript'>
			<!--
			
			ConfirmProcess();
			//-->
			</script>
			";

		#endregion

		#region Public Methods and Operators

		public DataTable CreateDefaultDataTable()
		{
			var table = new DataTable( "FMQueryBasicTable" );

			var stringType = Type.GetType( "System.String" );
			if (stringType == null)
			{
				throw new ApplicationException("Could not obtain object for system.string type.");
			}

			table.Columns.Add(new DataColumn("Field", stringType));
			table.Columns.Add(new DataColumn("Operator", stringType));
			table.Columns.Add(new DataColumn("Value", stringType));
			table.Columns.Add(new DataColumn("AndOr", stringType));
			table.Columns.Add(new DataColumn("Type", stringType));

			return table;
		}

		public DataTable LoadDataTableFromSession()
		{
			DataTable dataTable;

			if (null == this.Session[QuerywriterDataView])
			{
				dataTable = this.CreateDefaultDataTable();
				this.Session.Add(QuerywriterDataView, dataTable);
			}
			else
			{
				dataTable = (DataTable)this.Session[QuerywriterDataView];
			}

			return dataTable;
		}

		public void OperatorListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.UpdateView();
		}

		#endregion


		#region Methods

		protected static ListItem NewFieldListItem(QueryWriterField field)
		{
			// JS20100923 WI-17997 need to use DB field name because field.ID can duplicate between
			// transactions and line items, for example, Flag05
			var newItem = new ListItem(field.DisplayName, field.DBFieldName);
			newItem.Attributes.Add("title", field.DisplayName);
			return newItem;
		}

		protected static void RemoveFromSelected(ListItem selectedItem)
		{
			foreach (QueryWriterField field in Query.Fields)
			{
				if (field.DBFieldName == selectedItem.Value)
				{
					Query.Fields.Remove(field);
					break;
				}
			}
		}

		protected void BindTableToDataGrid(DataTable dataTable)
		{
			var dv = new DataView(dataTable);
			this.QueryCriteriaGrid.DataSource = dv;
			this.QueryCriteriaGrid.DataBind();
		}

		protected void DeleteCriteria(GridViewCommandEventArgs e)
		{
			int index = Convert.ToInt32(e.CommandArgument);

			DataTable table = this.LoadDataTableFromSession();
			DataRow row = table.Rows[index];

			var rowType = (string)row["Type"];
			table.Rows.RemoveAt(index);

			if (rowType == QueryCriteriaType.StartGroup.ToString())
			{
				while ((table.Rows[index]["Type"] as String) != QueryCriteriaType.EndGroup.ToString())
				{
					++index;
				}

				table.Rows.RemoveAt(index);
			}
			else if (rowType == QueryCriteriaType.EndGroup.ToString())
			{
				while ((table.Rows[index - 1]["Type"] as String) != QueryCriteriaType.StartGroup.ToString())
				{
					--index;
				}
				table.Rows.RemoveAt(index - 1);
			}

			this.BindTableToDataGrid(table);
			this.UpdateDataTableFromDataGrid();
		}

		/// <summary>
		///     During Criteria Grid row created processing this method is used to display
		///     the row as a group moniker.  It shows the Group label and hides the other data
		///     controls on the row.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="displayText"></param>
		protected void DisplayGroupRow(GridViewRowEventArgs e, string displayText)
		{
			var comboBox = (FMDropDownList)e.Row.FindControl("FieldList");
			if (comboBox != null)
			{
				comboBox.Visible = false;
			}

			var groupLabel = (FMLabel)e.Row.FindControl("GroupLabel");
			if (groupLabel != null)
			{
				groupLabel.Visible = true;
				groupLabel.Text = displayText;
			}

			var dropDown = (FMDropDownList)e.Row.FindControl("OperatorList");
			if (dropDown != null)
			{
				dropDown.Visible = false;
			}

			Control valueBox = e.Row.FindControl("ValueTextBox");
			if (valueBox != null)
			{
				valueBox.Visible = false;
			}

			dropDown = (FMDropDownList)e.Row.FindControl("AndOrDropDown");
			if (dropDown != null)
			{
				dropDown.Visible = false;
			}
		}

		protected void HiddenButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.SaveProcessing(true);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void InitializeComponent()
		{
			this.QueryCriteriaGrid.RowCommand += this.QueryCriteriaGridRowCommand;
			this.QueryCriteriaGrid.RowDataBound += this.QueryCriteriaGridRowDataBound;
			this.SubmitButton.Click += this.SubmitButtonClick;
			this.QueryTypeDropDown.SelectedIndexChanged += this.QueryTypeDropDownSelectedIndexChanged;
			this.AssignButton.Click += this.AssignButtonClick;
			this.RemoveButton.Click += this.RemoveButtonClick;
			this.MoveUpButton.Click += this.MoveUpButtonClick;

			this.MoveDownButton.Click += this.MoveDownButtonClick;
			this.ManageQueriesButton.Click += this.ManageQueriesButtonClick;
			this.AddPhraseButton.Click += this.AddPhraseButtonClick;
			this.AddPhraseGroupButton.Click += this.AddPhraseGroupButtonClick;
			this.ApplyButton.Click += this.ApplyButtonClick;

			this.ApplyButton.OnClientClick = String.Format("fnClickOK('{0}','{1}')", this.ApplyButton.UniqueID, string.Empty);
		}

		protected void LoadAndOrList(GridViewRowEventArgs e)
		{
			var dropDown = (DropDownList)e.Row.FindControl("AndOrDropDown");
			if (dropDown != null)
			{
				DataTable dt = this.LoadDataTableFromSession();
				DataRow row = dt.Rows[e.Row.RowIndex];

				var view = (DataView)this.QueryCriteriaGrid.DataSource;
				if (view != null)
				{
					if (view.Table.Rows.Count == e.Row.RowIndex + 1)
					{
						dropDown.Visible = false;
					}
					else
					{
						// If this is the next to last item and the next one is a group end
						// we do not want to display this control
						if (view.Table.Rows.Count == e.Row.RowIndex + 2)
						{
							DataRow nextRow = dt.Rows[e.Row.RowIndex + 1];
							if (nextRow["Type"].ToString() == QueryCriteriaType.EndGroup.ToString())
							{
								dropDown.Visible = false;
							}
						}
					}
				}

				if (row["AndOr"] is DBNull)
				{
					row["AndOr"] = dropDown.Items[0].Value;
				}

				dropDown.SelectedValue = (string)row["AndOr"];
			}
		}

		protected void LoadAvailableFields()
		{
			this.AvailableFieldsList.Items.Clear();

			// Load the available fields list box with the fields that are left
			QueryWriterFieldCollection fieldCollection = this.Topic.GetFields(this.Security, true);

			fieldCollection.Sort();

			foreach (QueryWriterField field in fieldCollection)
			{
                // add fields to available fields list
				if (Query.Fields.Get(field.ID) == null)
				{
					this.AvailableFieldsList.Items.Add(NewFieldListItem(field));
				}
			}
		}

		/// <summary>
		/// During Criteria Grid row created processing this method is used to load
		/// the Field List drop down.
		/// </summary>
		/// <param name="e">The <see cref="GridViewRowEventArgs"/> instance containing the event data.</param>
		protected void LoadFieldList(GridViewRowEventArgs e)
		{
			if (this.Topic != null)
			{
				QueryWriterFieldCollection fields = this.Topic.GetFields(this.Security, true);

			    List<string> virtualFieldNames =
			        fields.Where(x => string.IsNullOrEmpty(x.SecondaryDBFieldName) == false)
			            .Select(x => x.SecondaryDBFieldName)
			            .ToList();

			    fields.RemoveAll(x => virtualFieldNames.Contains(x.DBFieldName));

				var dropDown = (FMDropDownList)e.Row.FindControl("FieldList");
				if (dropDown != null)
				{
					dropDown.Translate = Topic.UseDataDictionary;
					fields.Sort();
					dropDown.DataSource = fields;
					dropDown.DataTextField = "DisplayName";

					// JS20100923 WI-17997 need to use DB field name because field.ID can duplicate between
					// transactions and line items, for example, Flag05
					dropDown.DataValueField = "DBFieldName";
					dropDown.DataBind();

					DataTable dt = this.LoadDataTableFromSession();
					DataRow row = dt.Rows[e.Row.RowIndex];

					if (row["Field"] is DBNull)
					{
						row["Field"] = dropDown.Items[0].Value;
					}

					dropDown.SelectedValue = (string)row["Field"];
				}
			}
		}

		protected void AddTransactionAliasFieldToTransactionsFieldCollection(ref QueryWriterFieldCollection fieldCollection)
		{

			if (this.QueryTypeDropDown.SelectedItem.Text.Trim().ToUpper().Equals("TRANSACTIONS"))
			{
				Boolean bAliasNameFoundInFieldList = false;
				foreach (QueryWriterField fd in fieldCollection)
				{
					if ((fd.DBFieldName == "tblTransactions.AliasName") && (fd.DisplayName == "Type"))
					{
						bAliasNameFoundInFieldList = true;
					}
				}

				if (bAliasNameFoundInFieldList == false)
				{
					const string DbFieldName = "tblTransactions.AliasName";
					const string DisplayName = "Type";
					const bool BGenerateSelect = true;
					var field = new QueryWriterField(DisplayName, DbFieldName, BGenerateSelect) { Topic = this.Topic };

					fieldCollection.Add(field);
				}
			}
		}

		/// <summary>
		///     During Criteria Grid row created processing this method is used to load
		///     the Field List drop down.
		/// </summary>
		/// <param name="e"></param>
		protected void LoadOperatorList(GridViewRowEventArgs e)
		{
			var dropDown = (DropDownList)e.Row.FindControl("OperatorList");
			if (dropDown != null)
			{
				DataTable dt = this.LoadDataTableFromSession();
				DataRow row = dt.Rows[e.Row.RowIndex];

				if (row["Operator"] is DBNull)
				{
					row["Operator"] = dropDown.Items[0].Value;
				}

				dropDown.SelectedValue = (string)row["Operator"];

				var valueTextBox = (TextBox)e.Row.FindControl("ValueTextBox");
				if (valueTextBox != null)
				{
					if (Enum.IsDefined(typeof(QueryOperator), dropDown.SelectedValue))
					{
						var selectedType = (QueryOperator)Enum.Parse(typeof(QueryOperator), dropDown.SelectedValue);
						if (selectedType == QueryOperator.NotNullOrEmpty || selectedType == QueryOperator.NullOrEmpty)
						{
							valueTextBox.Visible = false;
						}
						else
						{
							valueTextBox.Visible = true;
						}
					}
				}
			}
		}

		protected void LoadQueryType()
		{
			this.QueryTypeDropDown.Items.Clear();

			QueryWriterTopicCollection topics = FMChannelHelper.MakeCall<IQueryWriterTopics, QueryWriterTopicCollection>(x => x.Enumerate(this.Security));

			if (topics.Count == 0)
			{
				throw new FMNoQueryTypesAvailableException();
			}

			this.QueryTypeDropDown.DataSource = topics;
			this.QueryTypeDropDown.DataTextField = "DisplayName";
			this.QueryTypeDropDown.DataValueField = "ObjectType";

			if (Query.Topic != null)
			{
				if (FMChannelHelper.MakeCall<IQueryWriterTopics, QueryWriterTopic>(x => x.Get(this.Security, Query.Topic.ObjectType.ToString())) == null)
				{
					topics.Add(Query.Topic);
				}
			}

			this.QueryTypeDropDown.DataBind();

			// Set the query type from the query object
			if (Query.Topic != null)
			{
				this.QueryTypeDropDown.SelectedValue = Query.Topic.ObjectType.ToString();
			}
			else
			{
				// If no query topic selected, try to pick transactions
				// Otherwise, just pick the first one in the list
				if (this.QueryTypeDropDown.SelectByText("Transactions") == false && this.QueryTypeDropDown.Items.Count > 0)
				{
					this.QueryTypeDropDown.SelectedIndex = 0;
				}
			}

			this.QueryTypeDropDownSelectedIndexChanged("INIT", null);
		}

		protected void LoadSelectedFields()
		{
			this.SelectedFieldsList.Items.Clear();

			foreach (QueryWriterField field in Query.Fields)
			{
				this.SelectedFieldsList.Items.Add(NewFieldListItem(field));
			}
		}

		protected void LoadValue(GridViewRowEventArgs e)
		{
			var valueTextBox = (TextBox)e.Row.FindControl("ValueTextBox");
			if (valueTextBox != null)
			{
				DataRow row = this.LoadDataTableFromSession().Rows[e.Row.RowIndex];

				if (row["Value"] is DBNull)
				{
					valueTextBox.Text = string.Empty;
				}
				else
				{
					valueTextBox.Text = (string)row["Value"];
				}
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponent();
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			this.Topic = (QueryWriterTopic)this.Session[QuerywriterQueryTopic];

			if (this.IsPostBack == false)
			{
				this.Session.Remove(QuerywriterDataView);

				// Trying to determine if the export button should be enabled or disabled
				// based on the if the query has been saved. If the mode is "NEW", then the query
				// has not been saved. If it is "EDIT", then it has been saved with the except if
				// coming from the QueryResultsForm page.  In that we want to ignore.
				string mode = this.Request.GetQueryOrFormValue("Mode");
				string referrer = this.Request.UrlReferrer == null ? string.Empty : this.Request.UrlReferrer.AbsoluteUri;

				// Maintain export button mode in session if coming from the QueryResultsForm page.
				// Otherwise, reset.
				if (referrer.Contains("QueryResultsForm.aspx") == false)
				{
					this.Session.Remove(QueryWriterExportButtonMode);
				}

				if (string.IsNullOrEmpty(mode) == false && referrer.Contains("QueryResultsForm.aspx") == false)
				{
					if (mode.ToUpper().Equals("NEW"))
					{
						this.Session.Add(QueryWriterExportButtonMode, "DISABLE");
					}
					else if(mode.ToUpper().Equals("EDIT"))
					{
						this.Session.Add(QueryWriterExportButtonMode, "ENABLE");
					}
				}
			}

			if (this.IsPostBack || this.Request.GetQueryOrFormValue("Mode").DefaultIfNull(string.Empty).Equals("New"))
			{
				this.BindTableToDataGrid(this.LoadDataTableFromSession());
			}

			// Cannot change type of existing query
			if (this.Request.GetQueryOrFormValue("Mode").DefaultIfNull(string.Empty).Equals("Edit"))
			{
				this.QueryTypeDropDown.Enabled = false;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				Query = (QueryClass)this.Session[QueryDefinitionForm.QuerywriterQueryObject];
				this.Topic = (QueryWriterTopic)this.Session[QuerywriterQueryTopic];

				if (this.IsPostBack == false)
				{
					if (this.Request.GetQueryOrFormValue("Mode").DefaultIfNull(string.Empty).Equals("Edit"))
					{
						this.UpdateView();
					}

					this.LoadQueryType();

					// Set the initial name and description if appropriate
					this.NameTextBox.Text = Query.QueryName.DefaultIfNull(string.Empty);
					this.DescriptionTextBox.Text = Query.QueryDescription.DefaultIfNull(string.Empty);
				}

				// Set focus to the NameTextBox when the embedded modal save dialog is displayed
				var sb = new StringBuilder();
				sb.Append("<script type=\"text/javascript\">\n");
				sb.Append("Sys.Application.add_load(modalSetup);\n");
				sb.Append("function modalSetup() {\n");
				sb.Append(string.Format("var modalPopup = $find('{0}');\n", this.ModalPopupExtender1.BehaviorID));
				sb.Append("if ( modalPopup != null )");
				sb.Append("modalPopup.add_shown(SetFocusOnControl); }\n");
				sb.Append("function SetFocusOnControl() {\n");
				sb.Append(string.Format("var textBox1 = $get('{0}');\n", this.NameTextBox.ClientID));
				sb.Append("if ( textBox1 != null )\n");
				sb.Append("textBox1.focus();}\n");
				sb.Append("</script>\n");
				this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "Startup", sb.ToString());

				this.UpdateDataTableFromDataGrid();
				this.SetExportButtonMode();
			}
			catch (FMNoQueryTypesAvailableException except)
			{
				this.ErrorHandler(except);
				this.DisableButtons();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Disables the controls on the form.
		/// </summary>
		private void DisableButtons()
		{
			this.FMDropDownListTransactionAliasTypes.Enabled = false;
			this.AssignButton.Enabled = false;
			this.RemoveButton.Enabled = false;
			this.MoveDownButton.Enabled = false;
			this.MoveUpButton.Enabled = false;
			this.AddPhraseButton.Enabled = false;
			this.AddPhraseGroupButton.Enabled = false;
			this.SubmitButton.Enabled = false;
			this.SaveButton.Enabled = false;
			this.ExportButton.Enabled = false;
			this.ManageQueriesButton.Enabled = false;
		}

		/// <summary>
		/// This method will set the export button mode to enabled or disabled
		/// based on the query being saved. The export button should be
		/// disabled if the query has not been saved.
		/// </summary>
		private void SetExportButtonMode()
		{
			var exportButtonMode = this.Session[QueryWriterExportButtonMode] as string;

			if (string.IsNullOrEmpty(exportButtonMode) == false && exportButtonMode.ToUpper().Equals("DISABLE"))
			{
				this.ExportButton.Enabled = false;
			}

			if (string.IsNullOrEmpty(exportButtonMode) == false && exportButtonMode.ToUpper().Equals("ENABLE"))
			{
				this.ExportButton.Enabled = true;
			}
		}

		protected void ReloadFields()
		{
			this.PopulateDropDownListWithTransactionTypes();

			if ( string.IsNullOrEmpty( this.QueryTypeDropDown.SelectedItem.Text ) == false )
			{
				if ( this.QueryTypeDropDown.SelectedItem.Text.Trim().ToUpper().Equals( "TRANSACTIONS" )
				&& this.FMDropDownListTransactionAliasTypes.SelectedIndex != -1)
				{

					var aliasGuids = new QueryWriterAliasGuidCollection();
					foreach (ListItem item in FMDropDownListTransactionAliasTypes.Items)
					{
						if (item.Selected)
						{
							aliasGuids.Add(new QueryWriterAliasGuid(item.Value));
						}
					}
					// Must set the transTypeAliasID if a transaction because it is used to filter the transaction alias field names.
					this.Topic.AliasGuids = aliasGuids;
				}
			}

			this.LoadSelectedFields();
			this.LoadAvailableFields();
		}

		///<summary>
		/// PopulateDropDownListWithTransactionTypes 
		/// ///</summary>
		///<remarks>
		///Used to populate the a transaction alias drop down.  
		///</remarks>
		protected void PopulateDropDownListWithTransactionTypes()
		{
			if ( string.IsNullOrEmpty( this.QueryTypeDropDown.SelectedItem.Text ) )
			{
				this.FMDropDownListTransactionAliasTypes.Items.Clear();
				this.FMDropDownListTransactionAliasTypes.Enabled = false;
				this.FMDropDownListTransactionAliasTypes.Visible = false;
				this.FMLabelTransactionType.Enabled = false;
				this.FMLabelTransactionType.Visible = false;
				return;
			}

			if ( this.QueryTypeDropDown.SelectedItem.Text.Trim().ToUpper().Equals( "TRANSACTIONS" ) )
			{
				this.FMDropDownListTransactionAliasTypes.Enabled = true;
				this.FMDropDownListTransactionAliasTypes.Visible = true;
				this.FMLabelTransactionType.Enabled = true;
				this.FMLabelTransactionType.Visible = true;
				this.FMDropDownListTransactionAliasTypes.Items.Clear();

				var aliasCollection =
					FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
						aliases => aliases.EnumerateNamesOnly(Security, true));

				if ( aliasCollection.Count > 0 )
				{

					this.Topic.AliasGuids = Query.TransactionAliasGuids;

					var item = new ListItem
						                {
							                Value = Guid.Empty.ToString(), 
											Text = "-ALL-"
						                };

					this.FMDropDownListTransactionAliasTypes.Items.Add( item );

					foreach ( TransactionAliasNameClass ta in aliasCollection )
					{
						item = new ListItem
							       {
								       Value = ta.IdentityGuid.ToString(), 
									   Text = ta.AliasName
							       };

						this.FMDropDownListTransactionAliasTypes.Items.Add( item );

						if ( Query.TransactionAliasGuids.Contains(new QueryWriterAliasGuid(item.Value)) )
						{
							item.Selected = true;
						}
					}
				}

				DataTable dataTable = this.CreateDefaultDataTable();

				foreach ( QueryCriteriaPhrase phrase in Query.Criterion )
				{
					DataRow row = dataTable.NewRow();

					if ( phrase.Type == QueryCriteriaType.Phrase )
					{
						row["Field"] = phrase.Field.DBFieldName;
						row["Operator"] = phrase.Operator;
						row["Value"] = phrase.Value;
						row["AndOr"] = phrase.Conjunction;
					}
				}

				// Cannot change TransactionAliasTypes of existing query if this is the edit mode. 
				if ( Request.GetQueryOrFormValue("Mode").DefaultIfNull( string.Empty ).Equals( "Edit" ) )
				{
					if ( this.FMDropDownListTransactionAliasTypes.Visible )
					{
						this.FMDropDownListTransactionAliasTypes.Enabled = false;
					}
				}
			}
			else 
			{
				// Not transactions
				this.FMDropDownListTransactionAliasTypes.Items.Clear();
				this.FMDropDownListTransactionAliasTypes.Enabled = false;
				this.FMDropDownListTransactionAliasTypes.Visible = false;
				this.FMLabelTransactionType.Enabled = false;
				this.FMLabelTransactionType.Visible = false;
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected void UpdateView()
		{
			DataTable dataTable = this.CreateDefaultDataTable();

			foreach (QueryCriteriaPhrase phrase in Query.Criterion)
			{
				DataRow row = dataTable.NewRow();

				if (phrase.Type == QueryCriteriaType.Phrase)
				{
					row["Field"] = phrase.Field.DBFieldName;
					row["Operator"] = phrase.Operator;
					row["Value"] = phrase.Value;
					row["AndOr"] = phrase.Conjunction;
				}

				row["Type"] = phrase.Type.ToString();

				dataTable.Rows.Add(row);
			}

			this.Session.Add(QuerywriterDataView, dataTable);

			this.BindTableToDataGrid(dataTable);
		}

		private void AddPhraseButtonClick(object sender, EventArgs e)
		{
			try
			{
				DataTable table = this.LoadDataTableFromSession();
				DataRow row = table.NewRow();
				row["Type"] = QueryCriteriaType.Phrase.ToString();

				if (this.QueryCriteriaGrid.SelectedIndex == -1)
				{
					table.Rows.Add(row);
				}
				else
				{
					table.Rows.InsertAt(row, this.QueryCriteriaGrid.SelectedIndex);
				}

				this.BindTableToDataGrid(table);
				this.UpdateDataTableFromDataGrid();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddPhraseGroupButtonClick(object sender, EventArgs e)
		{
			try
			{
				DataTable table = this.LoadDataTableFromSession();

				DataRow row = table.NewRow();
				row["Type"] = QueryCriteriaType.StartGroup.ToString();
				table.Rows.Add(row);

				row = table.NewRow();
				row["Type"] = QueryCriteriaType.EndGroup.ToString();
				table.Rows.Add(row);

				this.BindTableToDataGrid(table);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ApplyButtonClick(object sender, EventArgs e)
		{
			try
			{
				// User must have Modify Queries right to save new queries
				if (this.Security.HasRight(RIGHT.MODIFY_QUERIES) == false)
				{
					throw new FMInsufficientRightsException();
				}

				// The Name must not be empty
				if (string.IsNullOrEmpty(this.NameTextBox.Text))
				{
					throw new ApplicationException("Name cannot be blank");
				}

				// If the query already exists, check to see if this user can overwrite the existing query. 
				// A user can only overwrite an existing query if the query to be overwritten is the same query (i.e., a regular modify).
				QueryClass existingQuery = FMChannelHelper.MakeCall<IQueries, QueryClass>(
						queries => queries.GetByQueryName(this.Security, this.NameTextBox.Text));

				QueryClass query = Query;

				if (existingQuery.IdentityGuid != Guid.Empty && existingQuery.IdentityGuid != query.IdentityGuid)
				{
					throw new ApplicationException("The name " + this.NameTextBox.Text + " is already in use by a different query.");
				}
				
				if (existingQuery.IdentityGuid != Guid.Empty)
				{
					// Query exists, make sure the user wants to overwrite the Query before saving it
					ScriptManager.RegisterStartupScript(
						this.UpdatePanelStep4, typeof(UpdatePanel), "ConfirmProcessScript_Key", ConfirmProcessScript, false);
				}
				else
				{
					this.SaveProcessing(false);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AssignButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem availableItem;
				while ((availableItem = this.AvailableFieldsList.SelectedItem) != null)
				{
					this.AvailableFieldsList.Items.Remove(availableItem);
					availableItem.Selected = false;

					Query.Fields.Add(this.Topic.FindFieldByDbName(this.Security, availableItem.Value, true));

					this.SelectedFieldsList.Items.Add(availableItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Manages the queries button click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void ManageQueriesButtonClick(object sender, EventArgs e)
		{
			this.Redirect("ManageQueriesForm.aspx");
		}

		/// <summary>
		/// Moves down button click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void MoveDownButtonClick(object sender, EventArgs e)
		{
			try
			{
				int index = this.SelectedFieldsList.SelectedIndex;
				if (-1 == index)
				{
					return;
				}

				if (index < this.SelectedFieldsList.Items.Count - 1)
				{
					Query.Fields.Swap(index + 1, index);
					this.SelectedFieldsList.Swap(index + 1, index);
					this.SelectedFieldsList.SelectedIndex = index + 1;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void MoveUpButtonClick(object sender, EventArgs e)
		{
			try
			{
				int index = this.SelectedFieldsList.SelectedIndex;

				if (index > 0)
				{
					Query.Fields.Swap(index, index - 1);
					this.SelectedFieldsList.Swap(index, index - 1);
					this.SelectedFieldsList.SelectedIndex = index - 1;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void QueryCriteriaGridRowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName == "Delete")
				{
					this.DeleteCriteria(e);
				}
				else if (e.CommandName == "Select")
				{
					this.BindTableToDataGrid(this.LoadDataTableFromSession());
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void QueryCriteriaGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowIndex != -1)
				{
					var deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
					if (deleteButton != null)
					{
						deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
					}

					DataTable table = this.LoadDataTableFromSession();
					if (e.Row.RowIndex < table.Rows.Count)
					{
						DataRow row = this.LoadDataTableFromSession().Rows[e.Row.RowIndex];

						// Is the row a group start or end moniker?
						if ((row["Type"] as string) == QueryCriteriaType.StartGroup.ToString())
						{
							this.DisplayGroupRow(e, "(Start Group)");
						}
						else if ((row["Type"] as string) == QueryCriteriaType.EndGroup.ToString())
						{
							this.DisplayGroupRow(e, "(End Group)");
						}
						else
						{
							this.LoadFieldList(e);
							this.LoadOperatorList(e);
							this.LoadValue(e);
							this.LoadAndOrList(e);
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void QueryTypeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			// Reset the query class object if we are not in init mode
			try
			{
				if (sender.ToString().Equals("INIT") == false || this.Request.GetQueryOrFormValue("Mode").Equals("New"))
				{
					this.Topic = FMChannelHelper.MakeCall<IQueryWriterTopics, QueryWriterTopic>(x => x.Get(this.Security, this.QueryTypeDropDown.SelectedValue));

					Query = FMChannelHelper.MakeCall<IQueries, QueryClass>(queries => queries.NewQuery(this.Security, this.Topic));

					this.Session[QueryDefinitionForm.QuerywriterQueryObject] = Query;

					// Clear the criteria grid
					DataTable table = this.LoadDataTableFromSession();
					table.Rows.Clear();
					this.BindTableToDataGrid(table);
				}

				this.Topic = Query.Topic ?? FMChannelHelper.MakeCall<IQueryWriterTopics, QueryWriterTopic>(x => x.Get(this.Security, this.QueryTypeDropDown.SelectedValue));

				this.SelectedFieldsList.UseDataDictionary = this.Topic.UseDataDictionary;
				this.AvailableFieldsList.UseDataDictionary = this.Topic.UseDataDictionary;

				this.Session[QuerywriterQueryTopic] = this.Topic;

				// Reset the selected/available fields list boxes
				this.ReloadFields();

				// Reset the values on the Advanced page as well
				((QueryDefinitionForm)this.Page).QueryDefinitionAdvancedPage.UpdateView();
				((QueryDefinitionForm)this.Page).QueryDefinitionAdvancedPage.SetGroupDropDowns();
				((QueryDefinitionForm)this.Page).QueryDefinitionAdvancedPage.UpdateControlStates();

                // If the user is querying transactions, add a step to the criteria advising them of fields to add to the criteria.
                // This is being done due to the poor performance of transaction queries in a database with a lot of transactions.
                this.Step3FListItem.Visible = this.QueryTypeDropDown != null && this.QueryTypeDropDown.SelectedItem != null 
                    && this.QueryTypeDropDown.SelectedItem.Text.Equals("Transactions", StringComparison.OrdinalIgnoreCase);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// FMDropDownListTransactionAliasTypes_SelectedIndexChanged  is the event function that is called to populate the tranactions alias drop down.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		/// <remarks>
		/// Get the Transaction alias id and set the TransTypeAliasID field in the topic
		/// Loads the selected fields and the available fields.
		/// </remarks>
		protected void FMDropDownListTransactionAliasTypesSelectedIndexChanged( object sender, EventArgs e )
		{
			bool hasAll = false;
			var aliasGuids = new QueryWriterAliasGuidCollection();

			if (FMDropDownListTransactionAliasTypes.SelectedIndex == 0)
			{
				hasAll = true;
			}

			foreach (ListItem item in FMDropDownListTransactionAliasTypes.Items)
			{
				if (item.Selected)
				{
					if (hasAll && item.Value != Guid.Empty.ToString())
					{
						item.Selected = false;
						continue;
					}
					
					aliasGuids.Add(new QueryWriterAliasGuid(item.Value));
				}
			}


			// get all the fields based on the the transaction type or alias and populate the AvailableFieldsList. 
			this.Topic.AliasGuids = aliasGuids;
			Query.TransactionAliasGuids = aliasGuids;

			// force reload
			this.Topic.Fields = null; 

			// make sure all selected fields are still available
			QueryWriterFieldCollection fieldCollection = this.Topic.GetFields( Security, true );

			for ( int i = Query.Fields.Count - 1; i >= 0; i-- )
			{
				QueryWriterField field;
				if ( ( field = fieldCollection.DBGet( Query.Fields[i].DBFieldName ) ) == null )
				{
					Query.Fields.RemoveAt( i );
				}
				else
				{
					Query.Fields[i].DisplayName = field.DisplayName;
				}
			}

			this.LoadSelectedFields();
			this.LoadAvailableFields();

			Query.Topic.Fields = fieldCollection;

			// All can have multiple criteria for type (Alias Name)
			if ( this.FMDropDownListTransactionAliasTypes.SelectedItem.Text != "-ALL-" )
			{
				this.RemoveNonAvailableCriteria();
			}
			else
			{
				// Reset the criteria dropdown text to reflect the fields display text
				this.BindTableToDataGrid( this.LoadDataTableFromSession() ); 
			}
		}

		private void RemoveButtonClick( object sender, EventArgs e )
		{
			try
			{
				ListItem selectedItem;
				while ((selectedItem = this.SelectedFieldsList.SelectedItem) != null)
				{
					this.SelectedFieldsList.Items.Remove(selectedItem);
					selectedItem.Selected = false;

					this.AvailableFieldsList.Items.Add(selectedItem);
					RemoveFromSelected(selectedItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles saving query without checking for existence of the query.  Assumes checking has already been done.
		/// </summary>
		/// <param name="queryExists">if set to <c>true</c> indicates query should be updated not created new.</param>
		private void SaveProcessing(bool queryExists)
		{
			var query = (QueryClass)this.Session[QueryDefinitionForm.QuerywriterQueryObject];

			query.QueryName = this.NameTextBox.Text;
			query.SiteGuid = this.Security.SiteGuid;
			query.QueryDescription = this.DescriptionTextBox.Text;

			if (queryExists)
			{
				FMChannelHelper.MakeCall<IQueries>(queries => queries.Modify(this.Security, query));
			}
			else
			{
				query.OwnerUserGuid = this.Security.UserGuid;
				query.IdentityGuid = FMChannelHelper.MakeCall<IQueries, Guid>(queries => queries.Add(this.Security, query));
			}

			this.Session[QueryDefinitionForm.QuerywriterQueryObject] = query;
			this.Session.Add(QueryWriterExportButtonMode, "ENABLE");
			this.SetExportButtonMode();
		}

		/// <summary>
		/// Submits the button click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void SubmitButtonClick(object sender, EventArgs e)
		{
			this.Redirect("QueryResultsForm.aspx");
		}

		/// <summary>
		/// Updates the data table from data grid.
		/// </summary>
		private void UpdateDataTableFromDataGrid()
		{
			DataTable dt = this.LoadDataTableFromSession();
			int index = 0;

			Query.Criterion.Clear();

			foreach (GridViewRow row in this.QueryCriteriaGrid.Rows)
			{
				DataRow dataRow = dt.Rows[index];

				var phrase = new QueryCriteriaPhrase();
				Query.Criterion.Add(phrase);

				var fieldList = (FMDropDownList)row.FindControl("FieldList");
				if (fieldList != null)
				{
					dataRow["Field"] = fieldList.SelectedValue;
					phrase.Field = this.Topic.FindFieldByDbName(this.Security, fieldList.SelectedValue, false);
				}

				var operatorList = (FMDropDownList)row.FindControl("OperatorList");
				if (operatorList != null)
				{
					dataRow["Operator"] = operatorList.SelectedValue;
					phrase.Operator = (QueryOperator)Enum.Parse(typeof(QueryOperator), operatorList.SelectedValue);
				}

				var valueTextBox = (TextBox)row.FindControl("ValueTextBox");
				if (valueTextBox != null)
				{
					dataRow["Value"] = valueTextBox.Text;
					phrase.Value = valueTextBox.Text;
				}

				var andOrList = (FMDropDownList)row.FindControl("AndOrDropDown");
				if (andOrList != null)
				{
					dataRow["AndOr"] = andOrList.SelectedValue;
					phrase.Conjunction = (QueryAndOr)Enum.Parse(typeof(QueryAndOr), andOrList.SelectedValue);
				}

				phrase.Type = (QueryCriteriaType)Enum.Parse(typeof(QueryCriteriaType), (string)dataRow["Type"]);

				phrase.Topic = Query.Topic;

				++index;
			}
		}

		protected void RemoveNonAvailableCriteria()
		{

			DataTable table = LoadDataTableFromSession();
			bool didRemove = false;

			if ( table.Rows.Count > 0 )
			{
				// Load the available fields list box with the fields that are left
				QueryWriterFieldCollection fieldCollection = this.Topic.GetFields( Security, true );

				for ( int i = table.Rows.Count - 1; i >= 0; i-- )
				{
					DataRow currentRow = table.Rows[i];

					// Is the row a group start or end moniker?
					if ( currentRow["Type"] as string != QueryCriteriaType.StartGroup.ToString()
						 && currentRow["Type"] as string != QueryCriteriaType.EndGroup.ToString() )
					{
						string strField = currentRow["Field"].ToString();

						if ( fieldCollection.DBGet( strField ) == null )
						{
							table.Rows.RemoveAt( i );
							didRemove = true;
						}
					}
				}

				if ( didRemove )
				{
					this.BindTableToDataGrid( table );
					this.UpdateDataTableFromDataGrid();
				}
			}
		}

		#endregion
	}
}