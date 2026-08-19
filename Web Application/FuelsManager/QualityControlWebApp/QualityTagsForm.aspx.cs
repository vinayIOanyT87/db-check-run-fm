// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QualityTagsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.QualityControlWebApp
{
	using System;
	using System.Data;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	using FuelsManager.FMWebApp;

	/// <summary>
	///     Summary description for QualityTagsForm.
	/// </summary>
	public partial class QualityTagsForm : FMFormBaseAjax, IEntityDiscovery
	{
		#region Constants and Fields

		protected QualityTagCollectionClass QualityTagCollection;

		private const string QualityTagFindString = "QualityTagFindString";
		private const string SortDirection = "QualityTagSortDirection";
		private const string SortExpression = "QualityTagSortExpression";

		private string searchString;

		#endregion

		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IQualityTags);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.QUALITY_TAG;
			}
		}

		#endregion

		#region Public Methods and Operators

		public string[] EnumerateSeverityNames()
		{
			return Enum.GetNames(typeof(QUALITY_SEVERITY_LEVELS));
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			QualityTagCollectionClass qualityTagCollection =
				FMChannelHelper.MakeCall<IQualityTags, QualityTagCollectionClass>(
					tags => tags.Enumerate(security, null, null, false));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (QualityTagClass qualityTag in qualityTagCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == qualityTag.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != qualityTag.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != qualityTag.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(qualityTag);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IQualityTags, Guid>(tags => tags.GetIdentityGuid(security, id));
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			FMChannelHelper.MakeCall<IQualityTags>(
				qualityTags =>
					{
						QualityTagClass qualityTag = qualityTags.Get(security, guid);
						qualityTag.SiteGuid = siteGuid;
						qualityTags.Modify(security, qualityTag);
					});
		}

		#endregion

		#region Methods

        /// <summary>
        /// Enable or disable controls on the screen
        /// </summary>
        /// <param name="enable">True to enable, false to disable</param>
	    private void EnableControls(bool enable)
	    {
            this.AddButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) && enable;
            this.AddButton2.Enabled = this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD) && enable;
	        this.FindBtn.Enabled = enable;
	        this.ShowAllButton.Enabled = enable;
            this.QualityTagsFormPageSizeDropDown.Enabled = enable;
	    }

		protected void FindBtnOnClick(object sender, EventArgs e)
		{
			if (this.FindTextBox == null || (this.FindTextBox.Text.Length < 1))
			{
				this.searchString = null;
				this.Session.Remove(QualityTagFindString);
			}
			else
			{
				this.searchString = this.FindTextBox.Text.ToUpper();
				this.FindTextBox.Text = this.searchString;
				this.Session.Add(QualityTagFindString, this.searchString);
			}

			// Update the page with the new contents.
			this.QualityTagsDataGrid.PageIndex = 0;
			this.UpdateView();
		}

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
                    this.EnableControls(true);

					if (this.Session[SortExpression] == null)
					{
						this.Session[SortExpression] = "Name";
					}

					if (this.Session[SortDirection] == null)
					{
						this.Session[SortDirection] = "DESC";
					}

					if (this.Session["QualityTagsDataGrid.PageIndex"] == null)
					{
						this.QualityTagsDataGrid.PageIndex = 0;
					}
					else
					{
						this.QualityTagsDataGrid.PageIndex = (int)this.Session["QualityTagsDataGrid.PageIndex"];
					}

					this.UpdateView();
				}
				else
				{
					if (this.Session["QualityTagCollection"] == null)
					{
						this.UpdateView();
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void QualityTagsDataGridPageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			if (this.QualityTagsDataGrid.EditIndex > -1)
			{
				return;
			}

			this.QualityTagsDataGrid.PageIndex = e.NewPageIndex;
			this.Session["QualityTagsDataGrid.PageIndex"] = e.NewPageIndex;
			this.UpdateView();
		}

		protected void QualityTagsDataGridRowCancelEdit(object sender, GridViewCancelEditEventArgs e)
		{
			this.QualityTagsDataGrid.EditIndex = -1;
			this.Session.Remove("QualityTagCollection");
		    this.EnableControls(true);
			this.UpdateView();
		}

		protected void QualityTagsDataGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					var dataKey = this.QualityTagsDataGrid.DataKeys[e.Row.RowIndex];
					if (dataKey != null)
					{
						Guid siteGuid = Guid.Parse((string)dataKey["SiteGuid"]);

						TableCell cell = e.Row.Cells[6];//bds
						var fcell = (DataControlFieldCell)cell;
						var editCommandField = (FMCommandField)fcell.ContainingField;
						editCommandField.Enabled = siteGuid == this.Security.SiteGuid;
						var deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
						deleteButton.Enabled = siteGuid == this.Security.SiteGuid;

						if (e.Row.RowIndex == this.QualityTagsDataGrid.EditIndex)
						{
							var index = (int)dataKey["Index"];
							this.QualityTagCollection = this.Session["QualityTagCollection"] as QualityTagCollectionClass;
							var qualityTagCollectionClass = this.QualityTagCollection;
							if (qualityTagCollectionClass != null)
							{
								QualityTagClass tag = qualityTagCollectionClass[index];
								Control ctrl = e.Row.Cells[4].Controls[1];//bds
								var severityDropDownList = ctrl as DropDownList;
								if (severityDropDownList != null)
								{
									severityDropDownList.SelectedIndex = (int)tag.Severity;
								}
							}
						}
						else if (this.QualityTagsDataGrid.EditIndex > -1)
						{
							editCommandField.Enabled = false;
							deleteButton.Enabled = false;
							e.Row.Enabled = false;
						}

						if (!this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD))
						{
							editCommandField.Enabled = false;
							deleteButton.Enabled = false;
							e.Row.Enabled = false;
						}
					}
				}
			}
				// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}
		}

		protected void QualityTagsDataGridRowDelete(object sender, GridViewDeleteEventArgs e)
		{
			try
			{

				int index = e.RowIndex;
				this.QualityTagsDataGrid.SelectedIndex = -1;

				if (index >= this.QualityTagsDataGrid.DataKeys.Count)
				{
					return;
				}

				DataKey dataKey = this.QualityTagsDataGrid.DataKeys[index];
				if (dataKey != null)
				{
					index = (int)dataKey["Index"];
				}

				var qualityTagCollection = (QualityTagCollectionClass)this.Session["QualityTagCollection"];
				this.Session.Remove("QualityTagCollection");
				if (index < qualityTagCollection.Count)
				{
					QualityTagClass qualityTag = qualityTagCollection[index];
					if (qualityTag.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<IQualityTags>(tags => tags.Purge(this.Security, qualityTag.IdentityGuid));
					}
				}

				this.EnableControls(true);

				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
				e.Cancel = true;
			}
		}

		protected void QualityTagsDataGridRowEdit(object sender, GridViewEditEventArgs e)
		{
			this.QualityTagsDataGrid.EditIndex = e.NewEditIndex;
			this.GridViewDataBind();
            this.EnableControls(false);
		}

		protected void QualityTagsDataGridRowUpdate(object sender, GridViewUpdateEventArgs e)
		{
            this.EnableControls(true);

			try
			{
				this.QualityTagCollection = this.Session["QualityTagCollection"] as QualityTagCollectionClass;
				var qualityTagCollectionClass = this.QualityTagCollection;
				var nameTextBox = QualityTagsDataGrid.Rows[e.RowIndex].Cells[3].Controls[0] as TextBox;//bds
				var severityDropDownList = QualityTagsDataGrid.Rows[e.RowIndex].Cells[4].Controls[1] as DropDownList;//bds
				var activeCheckBox = QualityTagsDataGrid.Rows[e.RowIndex].Cells[5].Controls[1] as CheckBox;//bds

				var dataKey = QualityTagsDataGrid.DataKeys[e.RowIndex];
				if (dataKey != null && qualityTagCollectionClass != null && nameTextBox != null && severityDropDownList != null
				    && activeCheckBox != null)
				{
					var index = (int)dataKey["Index"];
					QualityTagClass tag = qualityTagCollectionClass[index];

					if (nameTextBox.Text.Trim().Length == 0)
					{
						throw new Exception("Name required.");
					}

					tag.ID = nameTextBox.Text.Trim();

					tag.Active = activeCheckBox.Checked;
					tag.Severity =
						(QUALITY_SEVERITY_LEVELS)Enum.Parse(typeof(QUALITY_SEVERITY_LEVELS), severityDropDownList.SelectedItem.Text);
					tag.SiteGuid = this.Security.SiteGuid;

					if (tag.IdentityGuid == Guid.Empty)
					{
						FMChannelHelper.MakeCall<IQualityTags>(tags => tags.Add(this.Security, tag));
					}
					else
					{
						FMChannelHelper.MakeCall<IQualityTags>(tags => tags.Modify(this.Security, tag));
					}
				}

				this.QualityTagsDataGrid.EditIndex = -1;
				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
				e.Cancel = true;
			}
		}

		protected void QualityTagsDataGridSort(object sender, GridViewSortEventArgs e)
		{
            // do not allow sorting while an item is in edit
		    if (this.QualityTagsDataGrid.EditIndex >= 0)
		    {
		        return;
		    }

			var sortExpression = this.Session[SortExpression] as string;
			var sortDirection = this.Session[SortDirection] as string;

			if (e.SortExpression != sortExpression)
			{
				this.Session[SortDirection] = "DESC";
			}
			else
			{
				if (sortDirection == "DESC")
				{
					this.Session[SortDirection] = "ASC";
				}
				else
				{
					this.Session[SortDirection] = "DESC";
				}
			}

			this.Session[SortExpression] = e.SortExpression;
			this.UpdateView();
		}

		// *************************************************************************************************
		// This method is called when the find button is pressed. It will retrieve data from the find
		// text box and set the search string. If there is no data, then the search string is set to null.
		// *************************************************************************************************

		// **************************************************************************************************
		// This method is called when the show all button is pressed. It will set the search string to null
		// indicating that we do not want to use the filter on finding companies.  In addition, the find
		// text box is cleared.
		// **************************************************************************************************
		protected void ShowAllBtnOnClick(object sender, EventArgs e)
		{
			this.Session.Remove(QualityTagFindString);
			this.searchString = null;
			this.FindTextBox.Text = string.Empty;
			this.QualityTagsDataGrid.PageIndex = 0;
			this.UpdateView();
		}

		private void AddButtonCommand(object sender, CommandEventArgs e)
		{
			this.EnumerateQualityTags();
			var tag = new QualityTagClass { SiteGuid = this.Security.SiteGuid };
			this.QualityTagCollection.Add(tag);

			this.QualityTagsDataGrid.EditIndex = this.QualityTagCollection.Count - 1;
			this.QualityTagsDataGrid.PageIndex = (this.QualityTagCollection.Count - 1) / this.QualityTagsDataGrid.PageSize;
			this.QualityTagsDataGrid.EditIndex = (this.QualityTagCollection.Count - 1) % this.QualityTagsDataGrid.PageSize;

			this.GridViewDataBind();

            this.EnableControls(false);
		}

		private void EnumerateQualityTags()
		{
			// Determine if the user entered in a filter to narrow the equip list. If so,
			// then call the method in equipments that will use the filter. Otherwise, use the
			// original method to get equipments.
			this.QualityTagCollection =
				FMChannelHelper.MakeCall<IQualityTags, QualityTagCollectionClass>(
					tags =>
					tags.Enumerate(
						this.Security, 
						this.searchString, 
						this.Session[SortExpression] as string + " " + this.Session[SortDirection], 
						false));

			this.Session["QualityTagCollection"] = this.QualityTagCollection;
		}

		private void GridViewDataBind()
		{
			var qualityTagDataTable = new DataTable();
			this.QualityTagCollection = this.Session["QualityTagCollection"] as QualityTagCollectionClass;

			qualityTagDataTable.Columns.Add("SiteGuid", typeof(string));
			qualityTagDataTable.Columns.Add("Index", typeof(Int32));
			qualityTagDataTable.Columns.Add("Name", typeof(string));
			qualityTagDataTable.Columns.Add("Severity", typeof(String));
			qualityTagDataTable.Columns.Add("Active", typeof(bool));
			int i = 0;
			var qualityTagCollectionClass = this.QualityTagCollection;
			if (qualityTagCollectionClass != null)
			{
				foreach (QualityTagClass qualityTag in qualityTagCollectionClass)
				{
					DataRow qualityTagDataRow = qualityTagDataTable.NewRow();

					qualityTagDataRow["SiteGuid"] = qualityTag.SiteGuid.ToString();
					qualityTagDataRow["Index"] = i++;
					qualityTagDataRow["Name"] = qualityTag.ID;
					qualityTagDataRow["Severity"] = Enum.GetName(typeof(QUALITY_SEVERITY_LEVELS), qualityTag.Severity);
					qualityTagDataRow["Active"] = qualityTag.Active;

					qualityTagDataTable.Rows.Add(qualityTagDataRow);
				}
			}
			var qualityTagDataView = new DataView(qualityTagDataTable);
			this.QualityTagsFormPageSizeDropDown.SetPageSize(this.QualityTagsDataGrid, qualityTagDataView.Count);

			this.QualityTagsDataGrid.DataSource = qualityTagDataView;
			this.QualityTagsDataGrid.DataBind();
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton.Command += this.AddButtonCommand;
			this.AddButton2.Command += this.AddButtonCommand;

			this.QualityTagsDataGrid.RowDeleting += this.QualityTagsDataGridRowDelete;
			this.QualityTagsDataGrid.RowUpdating += this.QualityTagsDataGridRowUpdate;
			this.QualityTagsDataGrid.RowEditing += this.QualityTagsDataGridRowEdit;
			this.QualityTagsDataGrid.RowCancelingEdit += this.QualityTagsDataGridRowCancelEdit;
			this.QualityTagsDataGrid.Sorting += this.QualityTagsDataGridSort;
			this.QualityTagsDataGrid.RowDataBound += this.QualityTagsDataGridRowDataBound;
			this.QualityTagsDataGrid.PageIndexChanging += this.QualityTagsDataGridPageIndexChanging;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			// Locate the previous search string from the session. Set the set
			// string if found.
			if (this.Session[QualityTagFindString] != null)
			{
				this.searchString = this.Session[QualityTagFindString] as string;
			}

			this.EnumerateQualityTags();
			this.GridViewDataBind();
		}

		#endregion
	}
}