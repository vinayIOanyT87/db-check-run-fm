// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="QueryDefinitionAdvanced.ascx.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the QueryDefinitionAdvanced type.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.QueryWriterWebApp
{
    using System;
    using System.Data;
    using System.Web.UI.WebControls;

    using AjaxControlToolkit;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;


    using FMControls;
    using FMCore;
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// Code behind for advanced query definition tab.
    /// </summary>
    public partial class QueryDefinitionAdvanced : QueryPageBase
	{
		#region Constants and Fields

		/// <summary>
		/// Query Writer Data View session key
		/// </summary>
		private const string QuerywriterDataView = "QueryWriter.QueryAdvancedDef.DataView";

		/// <summary>
		/// Gets or sets the query object we are working with.
		/// </summary>
		protected QueryClass Query { get; set; }

		/// <summary>
		/// Gets or sets the topic.
		/// </summary>
		/// <value>
		/// The topic.
		/// </value>
		protected QueryWriterTopic Topic { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Creates the default data table.
		/// </summary>
		/// <returns>A default data table</returns>
		public DataTable CreateDefaultDataTable()
		{
			var table = new DataTable("FMQueryAdvancedTable");

			Type stringType = Type.GetType("System.String");
			if ( stringType == null )
			{
				throw new ApplicationException("Could not find system.string type.");
			}

			Type booleanType = Type.GetType("System.Boolean");
			if ( booleanType == null )
			{
				throw new ApplicationException( "Could not find system.boolean type." );
			}

			table.Columns.Add(new DataColumn("FieldID", stringType));
			table.Columns.Add(new DataColumn("Field", stringType));
			table.Columns.Add(new DataColumn("Filter", booleanType));
			table.Columns.Add(new DataColumn("Value", stringType));
			table.Columns.Add(new DataColumn("Value2", stringType));
			table.Columns.Add(new DataColumn("DateType", booleanType));
			table.Columns.Add(new DataColumn("DBFieldName", stringType));

			if (this.Topic != null)
			{
				QueryWriterFieldCollection fields = this.Topic.GetFields(this.Security, true);
				fields.Sort();

                // build a list of the virtual field names
			    List<string> virtualFieldNames =
			        fields.Where(x => string.IsNullOrEmpty(x.SecondaryDBFieldName) == false)
			            .Select(x => x.SecondaryDBFieldName)
			            .ToList();

                // create a new row for each non-virtual field
			    foreach (QueryWriterField field in fields.Where(x => virtualFieldNames.Contains(x.DBFieldName) == false))
				{
					DataRow row = table.NewRow();

					row["FieldID"] = field.ID;
					row["Field"] = field.DisplayName;
					row["Filter"] = false;
					row["Value"] = string.Empty;
					row["Value2"] = string.Empty;
					row["DateType"] = field.IsDateType();
					row["DBFieldName"] = field.DBFieldName;


					table.Rows.Add(row);
				}
			}

			return table;
		}

		/// <summary>
		/// Loads the data table from session.
		/// </summary>
		/// <returns>The saved data table or a new one if one does not exist.</returns>
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

		/// <summary>
		/// Sets the group drop downs.
		/// </summary>
		public void SetGroupDropDowns()
		{
			this.Group1DropDown.Translate = this.Topic.UseDataDictionary;
			this.Group2DropDown.Translate = this.Topic.UseDataDictionary;
			this.Group3DropDown.Translate = this.Topic.UseDataDictionary;

			this.Group1DropDown.Items.Clear();
			this.Group1DropDown.Items.Add(new ListItem(string.Empty, string.Empty));

			this.Group2DropDown.Items.Clear();
			this.Group2DropDown.Items.Add(new ListItem(string.Empty, string.Empty));

			this.Group3DropDown.Items.Clear();
			this.Group3DropDown.Items.Add(new ListItem(string.Empty, string.Empty));

			// Load the first group
			foreach (QueryWriterField field in this.Query.Topic.GetFields(this.Security, true))
			{
				this.Group1DropDown.Items.Add(new ListItem(field.DisplayName, field.ID));
				this.Group2DropDown.Items.Add(new ListItem(field.DisplayName, field.ID));
				this.Group3DropDown.Items.Add(new ListItem(field.DisplayName, field.ID));
			}

			// Load the first group 
			this.Group1DropDown.SelectedValue = (this.Query.DataGroups.Count > 0) ? this.Query.DataGroups[0].ID : string.Empty;

			// Only load the second group if one is selected in the first one
			this.Group2DropDown.Enabled = this.Query.DataGroups.Count > 0;
			this.Group2DropDown.SelectedValue = (this.Query.DataGroups.Count > 1) ? this.Query.DataGroups[1].ID : string.Empty;

			// Only load the third group if one is selected in the second one
			this.Group3DropDown.Enabled = this.Query.DataGroups.Count > 1;
			this.Group3DropDown.SelectedValue = (this.Query.DataGroups.Count > 2) ? this.Query.DataGroups[2].ID : string.Empty;
		}

		/// <summary>
		/// Updates the control states.
		/// </summary>
		public void UpdateControlStates()
		{
			var topic = (QueryWriterTopic)this.Session[QueryDefinitionBasic.QuerywriterQueryTopic];

			this.ArchiveQueryCheckBox.Visible = topic.SupportsArchiveQuery;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		public void UpdateView()
		{
			this.Topic = (QueryWriterTopic)this.Session[QueryDefinitionBasic.QuerywriterQueryTopic];
			this.Query = (QueryClass)this.Session[QueryDefinitionForm.QuerywriterQueryObject];

			DataTable table = this.CreateDefaultDataTable();

			foreach (QueryFilterGroupClass filterGroup in this.Query.FilterGroups)
			{
				bool foundByDbFieldName = false;

				// match up the filter group to a table entry
				foreach (DataRow row in table.Rows)
				{
					if (filterGroup.DbFieldName == (string)row["DBFieldName"])
					{
						row["Filter"] = filterGroup.Filter;
						row["Value"] = filterGroup.DefaultValue1;
						row["Value2"] = filterGroup.DefaultValue2;
						foundByDbFieldName = true;
						break;
					}
				}

				if (foundByDbFieldName == false)
				{
					foreach (DataRow row in table.Rows)
					{
						if (filterGroup.FilterID == (string)row["FieldID"])
						{
							row["Filter"] = filterGroup.Filter;
							row["Value"] = filterGroup.DefaultValue1;
							row["Value2"] = filterGroup.DefaultValue2;
							break;
						}
					}
				}
			}

			this.Session.Add(QuerywriterDataView, table);

			this.GroupFilterGrid.DataSource = new DataView(table);
			this.GroupFilterGrid.DataBind();
		}

		#endregion


		#region Methods

		/// <summary>
		/// Checks for the specified group in the collection
		/// </summary>
		/// <param name="groupCollection">The group collection.</param>
		/// <param name="checkGroup">The check group.</param>
		/// <returns>True if the group is in the collection.</returns>
		protected static bool ExistsInGroupCollection(GroupCollectionClass groupCollection, GroupClass checkGroup)
		{
			foreach (GroupClass group in groupCollection)
			{
				if (group.ID == checkGroup.ID)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Binds the table to data grid.
		/// </summary>
		/// <param name="dataTable">The data table.</param>
		protected void BindTableToDataGrid(DataTable dataTable)
		{
			var dv = new DataView(dataTable);
			this.GroupFilterGrid.DataSource = dv;
			this.GroupFilterGrid.DataBind();
		}

		/// <summary>
		/// Reloads the specified group drop down.
		/// </summary>
		/// <param name="dropDown">The drop down.</param>
		protected void ReloadGroupDropDown(FMDropDownList dropDown)
		{
			dropDown.Items.Clear();
			dropDown.Items.Add( new ListItem( string.Empty, string.Empty ) );

			foreach ( QueryWriterField field in this.Query.Topic.GetFields( this.Security, true ) )
			{
				dropDown.Items.Add( new ListItem( field.DisplayName, field.ID ) );
			}
		}

		/// <summary>
		/// Enables the data group controls.
		/// </summary>
		protected void EnableDataGroupControls()
		{
			if (this.Group2DropDown.SelectedValue == string.Empty && this.Query.DataGroups.Count > 1)
			{
				this.Query.DataGroups.RemoveRange(1, this.Query.DataGroups.Count - 1 );
			}

			if ( this.Group1DropDown.SelectedValue == string.Empty && this.Query.DataGroups.Count > 0 )
			{
				this.Query.DataGroups.RemoveRange( 0, this.Query.DataGroups.Count - 1 );
			}

			this.Group2DropDown.Enabled = this.Query.DataGroups.Count > 0;
			this.Group3DropDown.Enabled = this.Query.DataGroups.Count > 1;

			// Remove the item selected from the list
			if ( this.Group2DropDown.Enabled )
			{
				this.RemoveDropDownItem( this.Group1DropDown, this.Group2DropDown );
			}

			if ( this.Group3DropDown.Enabled )
			{
				this.RemoveDropDownItem( this.Group1DropDown, this.Group3DropDown );
				this.RemoveDropDownItem( this.Group2DropDown, this.Group3DropDown );
			}
		}

		/// <summary>
		/// Removes the drop down item.
		/// </summary>
		/// <param name="dropDown1">The drop down with a selected value.</param>
		/// <param name="dropDown2">The drop down from which to remove the item.</param>
		private void RemoveDropDownItem( FMDropDownList dropDown1, FMDropDownList dropDown2 )
		{
			var item = dropDown1.SelectedValue;

			if (item != string.Empty)
			{
				var foundItem = dropDown2.Items.FindByValue(item);

				if (foundItem != null)
				{
					dropDown2.Items.Remove(foundItem);
				}
			}
		}

		/// <summary>
		/// Group1s the drop down selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Group1DropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				// If the blank item is selected, clear all items
				if (this.Group1DropDown.SelectedValue.Equals(string.Empty))
				{
					this.Query.DataGroups.Clear();

					this.Group2DropDown.SelectedValue = string.Empty;
					this.Group2DropDown.Enabled = false;

					this.Group3DropDown.SelectedValue = string.Empty;
					this.Group3DropDown.Enabled = false;
				}
				else
				{
					QueryWriterField field = this.Query.Topic.FindFieldByID(this.Security, this.Group1DropDown.SelectedValue, true);
					if (this.Query.DataGroups.Count > 0)
					{
						this.Query.DataGroups[0] = field;
					}
					else
					{
						this.Query.DataGroups.Add(field);
					}

					this.ReloadGroupDropDown( this.Group2DropDown );
					this.ReloadGroupDropDown( this.Group3DropDown );
					this.EnableDataGroupControls();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Group2s the drop down selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Group2DropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				// If the blank item is selected, clear all items
				if (this.Group2DropDown.SelectedValue.Equals(string.Empty))
				{
					QueryWriterField field = null;

					if (this.Query.DataGroups.Count > 0)
					{
						field = this.Query.DataGroups[0];
					}

					this.Query.DataGroups.Clear();

					if (field != null)
					{
						this.Query.DataGroups.Add(field);
					}

					this.Group3DropDown.SelectedValue = string.Empty;
					this.Group3DropDown.Enabled = false;
				}
				else
				{
					QueryWriterField field = this.Query.Topic.FindFieldByID(this.Security, this.Group2DropDown.SelectedValue, true);
					if (this.Query.DataGroups.Count > 1)
					{
						this.Query.DataGroups[1] = field;
					}
					else
					{
						this.Query.DataGroups.Add(field);
					}

					this.ReloadGroupDropDown( this.Group3DropDown );
					this.EnableDataGroupControls();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Group3s the drop down selected index changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Group3DropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				// If the blank item is selected, clear all items
				if (this.Group3DropDown.SelectedValue.Equals(string.Empty))
				{
					QueryWriterField field1 = null;
					QueryWriterField field2 = null;

					if (this.Query.DataGroups.Count > 0)
					{
						field1 = this.Query.DataGroups[0];
					}

					if (this.Query.DataGroups.Count > 1)
					{
						field2 = this.Query.DataGroups[1];
					}

					this.Query.DataGroups.Clear();

					if (field1 != null)
					{
						this.Query.DataGroups.Add(field1);
					}

					if (field2 != null)
					{
						this.Query.DataGroups.Add(field2);
					}
				}
				else
				{
					QueryWriterField field = this.Query.Topic.FindFieldByID(this.Security, this.Group3DropDown.SelectedValue, true);
					if (this.Query.DataGroups.Count > 2)
					{
						this.Query.DataGroups[2] = field;
					}
					else
					{
						this.Query.DataGroups.Add(field);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		protected void InitializeComponent()
		{
			this.AssignButton.Click += this.AssignButtonClick;
			this.RemoveButton.Click += this.RemoveButtonClick;
			this.PageSizeDropDown.SelectedIndexChanged += this.PageSizeDropDownSelectedIndexChanged;
			this.TitleTextBox.TextChanged += this.TitleTextBoxTextChanged;
			this.HeaderTextBox.TextChanged += this.HeaderTextBoxTextChanged;
			this.FooterTextBox.TextChanged += this.FooterTextBoxTextChanged;
			this.TotalAllFields.CheckedChanged += this.TotalAllFieldsCheckedChanged;
			this.LineNumbersCheckBox.CheckedChanged += this.LineNumbersCheckBoxCheckedChanged;
			this.SummaryOnly.CheckedChanged += this.SummaryOnlyCheckedChanged;
			this.GroupFilterGrid.RowDataBound += this.GroupFilterGridRowDataBound;
			this.ArchiveQueryCheckBox.CheckedChanged += this.ArchiveQueryCheckBoxCheckedChanged;
			this.MenuLocationTextBox.TextChanged += MenuLocationTextBoxTextChanged;
			this.PreventDeletionCheckBox.CheckedChanged += PreventDeletionCheckBoxCheckedChanged;
		}

		void PreventDeletionCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				Query.SystemQuery = PreventDeletionCheckBox.Checked;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		void MenuLocationTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				string originalPath = MenuLocationTextBox.Text;
				string newPath = originalPath;

				while (newPath.Contains("//"))
				{
					newPath = newPath.Replace("//", "/");
				}

				while (newPath.StartsWith("/"))
				{
					newPath = newPath.Remove(0, 1);
				}

				while (newPath.EndsWith("/"))
				{
					newPath = newPath.Remove(newPath.Length - 1, 1);
				}

				if (newPath.StartsWith("FuelsManager/"))
				{
					newPath = newPath.Remove(0, 13);
				}

				if (string.IsNullOrEmpty(newPath) == false)
				{
					QueryClass existingQuery = FMChannelHelper.MakeCall<IQueries, QueryClass>(x => x.GetByNodePath(Security, newPath));

					if (existingQuery != null && existingQuery.QueryStorageGuid != Guid.Empty
					    && existingQuery.QueryStorageGuid != Query.QueryStorageGuid)
					{
						throw new ApplicationException(string.Format("Menu Location has already been used."));
					}
				}

				MenuLocationTextBox.Text = newPath;
				Query.NavNodePath = MenuLocationTextBox.Text;
			}
			catch (Exception execpt)
			{
				this.ErrorHandler(execpt);
			}
		}

		/// <summary>
		/// Loads the settings.
		/// </summary>
		protected void LoadSettings()
		{
			try
			{
				this.TitleTextBox.Text = this.Query.Title;
				this.HeaderTextBox.Text = this.Query.Header;
				this.FooterTextBox.Text = this.Query.Footer;
				this.PageSizeDropDown.SelectedValue = this.Query.InitialPageSize;
				this.TotalAllFields.Checked = this.Query.TotalAllFields;

				if (this.TotalAllFields.Checked)
				{
					this.SummaryOnly.Enabled = true;
					this.SummaryOnly.Checked = this.Query.ShowSummaryLinesOnly;
				}
				else
				{
					this.SummaryOnly.Checked = false;
					this.SummaryOnly.Enabled = false;
					this.Query.ShowSummaryLinesOnly = false;
				}
				
				this.LineNumbersCheckBox.Checked = this.Query.IncludeLineNumbers;
				this.ArchiveQueryCheckBox.Checked = this.Query.QueryOnArchiveData;
				this.MenuLocationTextBox.Text = this.Query.NavNodePath;
				this.PreventDeletionCheckBox.Checked = this.Query.SystemQuery;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Loads the user groups.
		/// </summary>
		protected void LoadUserGroups()
		{
			GroupCollectionClass groupCollection =
				FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(groups => groups.Enumerate(this.Security));

			GroupCollectionClass qgm =
			 FMChannelHelper.MakeCall<IQueryGroupMaps, GroupCollectionClass>(x => x.EnumerateAssignedGroups(this.Security, this.Query.IdentityGuid));

			this.UnassignedGroupsListBox.Items.Clear();
			this.AssignedGroupsListBox.Items.Clear();

			// Check that all the user groups in the Query are in the main group enumeration from 
			// the system.  If not, go ahead and remove them.
			for (int index = 0; index < this.Query.AssignedGroups.Count;)
			{
				GroupClass group = this.Query.AssignedGroups[index];

				//Remove from assigned user groups if user group is not assigned to
				//current site or user group is not assigned to query
				if (ExistsInGroupCollection(groupCollection, group) == false || ExistsInGroupCollection(qgm, group) == false)
				{
					this.Query.AssignedGroups.RemoveAt(index);
				}
				else
				{
					++index;
				}

			}

			// Load up the assigned groups list
			this.AssignedGroupsListBox.DataSource = this.Query.AssignedGroups;
			this.AssignedGroupsListBox.DataTextField = "ID";
			this.AssignedGroupsListBox.DataValueField = "IdentityGuid";
			this.AssignedGroupsListBox.DataBind();

			// Load up the unassigned groups list
			var unassignedSource = new GroupCollectionClass();
			foreach (GroupClass group in groupCollection)
			{
				if (ExistsInGroupCollection(this.Query.AssignedGroups, group) == false)
				{
					unassignedSource.Add(group);
				}
			}

			this.UnassignedGroupsListBox.DataSource = unassignedSource;
			this.UnassignedGroupsListBox.DataTextField = "ID";
			this.UnassignedGroupsListBox.DataValueField = "IdentityGuid";
			this.UnassignedGroupsListBox.DataBind();
		}

		/// <summary>
		/// Raises the <see cref="OnInit"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponent();
		}

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				if (this.IsPostBack || this.Request.GetQueryOrFormValue("Mode").DefaultIfNull(string.Empty).Equals("New"))
				{
					this.BindTableToDataGrid(this.LoadDataTableFromSession());
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Query = (QueryClass)this.Session[QueryDefinitionForm.QuerywriterQueryObject];
				this.Topic = (QueryWriterTopic)this.Session[QueryDefinitionBasic.QuerywriterQueryTopic];

				// This could mean that the user has modify query rights but no rights to any topics
				if (this.Topic == null)
				{
					return;
				}

				if (this.IsPostBack == false)
				{
					if (this.Request.GetQueryOrFormValue("Mode").DefaultIfNull(string.Empty).Equals("Edit"))
					{
						this.UpdateView();
					}

					this.LoadSettings();
					this.LoadUserGroups();
					this.SetGroupDropDowns();
				}

				this.UpdateDataTableFromDataGrid();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the CheckedChanged event of the ArchiveQueryCheckBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ArchiveQueryCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.Query.QueryOnArchiveData = this.ArchiveQueryCheckBox.Checked;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the AssignButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void AssignButtonClick(object sender, EventArgs e)
		{
			try
			{
				FMChannelHelper.MakeCall<IGroups>(this.AddAssignment);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Adds the assignment.
		/// </summary>
		/// <param name="groups">The groups proxy.</param>
		private void AddAssignment(IGroups groups)
		{
			ListItem item;
			while ((item = this.UnassignedGroupsListBox.SelectedItem) != null)
			{
				this.UnassignedGroupsListBox.Items.Remove(item);
				item.Selected = false;

				Guid identityGuid = Guid.Parse(item.Value);

				GroupClass group = groups.Get(this.Security, identityGuid);

				this.Query.AssignedGroups.Add(@group);

				this.AssignedGroupsListBox.Items.Add(item);
			}
		}

		/// <summary>
		/// Handles the TextChanged event of the FooterTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void FooterTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Query.Footer = this.FooterTextBox.Text;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowDataBound event of the GroupFilterGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
		private void GroupFilterGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					DataRow row = this.LoadDataTableFromSession().Rows[e.Row.RowIndex];

					// If the row is a date type, we need to show two value text boxes and put a 
					// hint object in them to explain what they are.
					if ((bool)row["DateType"])
					{
						var value = (TextBox)e.Row.FindControl("ValueTextBox");
						var value1 = (TextBox)e.Row.FindControl("ValueTextBox1");
						var value2 = (TextBox)e.Row.FindControl("ValueTextBox2");

						if (value != null && value2 != null)
						{
							value.Visible = false;
							value1.Visible = true;
							value2.Visible = true;
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
		/// Handles the TextChanged event of the HeaderTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void HeaderTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Query.Header = this.HeaderTextBox.Text;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the CheckedChanged event of the LineNumbersCheckBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void LineNumbersCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.Query.IncludeLineNumbers = this.LineNumbersCheckBox.Checked;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the PageSizeDropDown control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void PageSizeDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Query.InitialPageSize = this.PageSizeDropDown.SelectedValue;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the RemoveButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void RemoveButtonClick(object sender, EventArgs e)
		{
			try
			{
				ListItem selectedItem;
				while ((selectedItem = this.AssignedGroupsListBox.SelectedItem) != null)
				{
					this.AssignedGroupsListBox.Items.Remove(selectedItem);
					selectedItem.Selected = false;

					this.RemoveGroupFromQuery(selectedItem.Text);

					this.UnassignedGroupsListBox.Items.Add(selectedItem);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Removes the group from query.
		/// </summary>
		/// <param name="groupId">The group ID.</param>
		private void RemoveGroupFromQuery(string groupId)
		{
			foreach (GroupClass group in this.Query.AssignedGroups)
			{
				if (group.ID == groupId)
				{
					this.Query.AssignedGroups.Remove(group);
					break;
				}
			}
		}

		/// <summary>
		/// Handles the CheckedChanged event of the SummaryOnly control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void SummaryOnlyCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.Query.ShowSummaryLinesOnly = this.SummaryOnly.Checked;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the TextChanged event of the TitleTextBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void TitleTextBoxTextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Query.Title = this.TitleTextBox.Text;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the CheckedChanged event of the TotalAllFields control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void TotalAllFieldsCheckedChanged(object sender, EventArgs e)
		{
			try
			{
				this.Query.TotalAllFields = this.TotalAllFields.Checked;

				if (this.TotalAllFields.Checked)
				{
					this.SummaryOnly.Enabled = true;
				}
				else
				{
					this.SummaryOnly.Checked = false;
					this.SummaryOnly.Enabled = false;
					this.Query.ShowSummaryLinesOnly = false;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Updates the data table from data grid.
		/// </summary>
		private void UpdateDataTableFromDataGrid()
		{
			DataTable dt = this.LoadDataTableFromSession();
			int index = 0;

			this.Query.FilterGroups.Clear();

			foreach (GridViewRow row in this.GroupFilterGrid.Rows)
			{
				DataRow dataRow = dt.Rows[index];

				var filter = new QueryFilterGroupClass
					{
						FilterID = dataRow["FieldID"].ToString(),
						DbFieldName = dataRow["DBFieldName"].ToString()
					};


				var filterCheck = (FMCheckBox)row.FindControl("FilterCheckBox");
				if (filterCheck != null)
				{
					dataRow["Filter"] = filterCheck.Checked;
					filter.Filter = filterCheck.Checked;
				}

				if ((bool)dataRow["DateType"])
				{
					var valueText1 = (TextBox)row.FindControl("ValueTextBox1");
					if (valueText1 != null)
					{
						var extender = (TextBoxWatermarkExtender)row.FindControl("IBWE1");
						if (extender == null || valueText1.Text.NotEquals(extender.WatermarkText))
						{
							dataRow["Value"] = valueText1.Text;
							filter.DefaultValue1 = valueText1.Text;
						}
					}

					var valueText2 = (TextBox)row.FindControl("ValueTextBox2");
					if (valueText2 != null)
					{
						var extender = (TextBoxWatermarkExtender)row.FindControl("IBWE2");
						if (extender == null || valueText2.Text.NotEquals(extender.WatermarkText))
						{
							dataRow["Value2"] = valueText2.Text;
							filter.DefaultValue2 = valueText2.Text;
						}
					}
				}
				else
				{
					var valueText = (TextBox)row.FindControl("ValueTextBox");
					if (valueText != null)
					{
						dataRow["Value"] = valueText.Text;
						filter.DefaultValue1 = valueText.Text;
					}
				}

				if (filter.Filter)
				{
					Query.FilterGroups.Add(filter);
				}

				++index;
			}
		}
		#endregion
	}
}