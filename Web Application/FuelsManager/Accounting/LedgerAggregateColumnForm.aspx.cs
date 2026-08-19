// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedgerAggregateColumnForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Accounting
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	public partial class LedgerAggregateColumnForm : AccountingWebFormView
	{
		#region Constants and Fields

		private LedgerAggregateColumnClass Column;

		private bool Editable = true;

		#endregion

		#region Public Methods and Operators

		public void AliasGrid_RowCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);
					this.Column.Aliases.RemoveAt(rowIndex);

					this.BindData();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			this.Initialize();
			base.OnInit(e);
			this.InitializeComponents();
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the PageSizeDropDown control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.BindData();
		}

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.NullReferenceException">Expected session to contain column object.</exception>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.Column = this.Session[PageSessionKeyConstants.LEDGER_AGGREGATE_COLUMN_OBJECT] as LedgerAggregateColumnClass;
				if (this.Column == null)
				{
					throw new NullReferenceException("Expected session to contain column object.");
				}

				if (this.Column.IdentityGuid != Guid.Empty && this.Column.SiteGuid != this.security.SiteGuid)
				{
					this.EnableControls(false);
					this.Editable = false;
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
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (this.IsPostBack == false)
				{
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the AddButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void AddButtonClick(object sender, EventArgs e)
		{
			try
			{
				var columnMap = new LedgerAggregateColumnMapClass { LedgerAggregateColumnGuid = this.Column.IdentityGuid };
				this.Column.Aliases.Add(columnMap);

				this.EnableControls(false);

				this.AliasGrid.EditIndex = this.Column.Aliases.Count - 1;
				this.BindData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowCancelingEdit event of the AliasGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="GridViewCancelEditEventArgs" /> instance containing the event data.</param>
		private void AliasGridRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			try
			{
				this.EnableControls(true);
				this.AliasGrid.EditIndex = -1;

				// Get the object we have associated with the row
				LedgerAggregateColumnMapClass columnMap = this.Column.Aliases[e.RowIndex];

				// Remove the line if this was a new line that is being cancelled
				if (columnMap.TransactionAliasGuid == Guid.Empty)
				{
					this.Column.Aliases.RemoveAt(e.RowIndex);
				}

				this.BindData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowDataBound event of the AliasGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="GridViewRowEventArgs" /> instance containing the event data.</param>
		private void AliasGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					if (this.Editable == false)
					{
						this.DisableEditDelete(e.Row);
					}

					var deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
					if (deleteButton != null)
					{
						deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);

						if (this.AliasGrid.EditIndex >= 0)
						{
							deleteButton.Enabled = false;
						}
					}

					var aliasDropDownList = (FMDropDownList)e.Row.FindControl("AliasDropDown");
					if (aliasDropDownList != null)
					{
						TransactionAliasNameCollectionClass aliasCollection =
							FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
								x => x.EnumerateNamesOnly(this.security, false));

						var currentMap = (LedgerAggregateColumnMapClass)e.Row.DataItem;

						for (int idx = 0; idx < this.Column.Aliases.Count; ++idx)
						{
							LedgerAggregateColumnMapClass map = this.Column.Aliases[idx];

							if (map.TransactionAliasGuid != currentMap.TransactionAliasGuid)
							{
								TransactionAliasNameClass aliasName = aliasCollection.Find(x => x.AliasName == map.AliasName);
								if (aliasName != null)
								{
									aliasCollection.Remove(aliasName);
								}
							}
						}

						aliasDropDownList.DataTextField = "AliasName";
                        aliasDropDownList.DataValueField = "MasterRecordGuid";
						aliasDropDownList.DataSource = aliasCollection;
						aliasDropDownList.DataBind();

						LedgerAggregateColumnMapClass columnMap = this.Column.Aliases[e.Row.RowIndex];
						aliasDropDownList.SelectByText(columnMap.AliasName);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowEditing event of the AliasGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="GridViewEditEventArgs" /> instance containing the event data.</param>
		private void AliasGridRowEditing(object sender, GridViewEditEventArgs e)
		{
			try
			{
				this.EnableControls(false);
				this.AliasGrid.EditIndex = e.NewEditIndex;
				this.BindData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowUpdating event of the AliasGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="GridViewUpdateEventArgs" /> instance containing the event data.</param>
		private void AliasGridRowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			try
			{
				// Get the row
				GridViewRow row = this.AliasGrid.Rows[e.RowIndex];

				// Get the object we have associated with the row
				LedgerAggregateColumnMapClass columnMap = this.Column.Aliases[e.RowIndex];

				// Pull the data from the controls on the grid
				columnMap.LedgerAggregateColumnGuid = this.Column.IdentityGuid;
				columnMap.TransactionAliasGuid = Guid.Parse(((FMDropDownList)row.Cells[1].Controls[1]).SelectedValue);//bds
				columnMap.AliasName = ((FMDropDownList)row.Cells[1].Controls[1]).SelectedItem.Text;//bds

				// Get the optional symbol
				string symbol = ((TextBox)row.Cells[2].Controls[1]).Text;//bds
				columnMap.Symbol = (symbol.Length > 0) ? symbol.Substring(0, 1) : string.Empty;

				this.EnableControls(true);

				// Reset the edit index
				this.AliasGrid.EditIndex = -1;

				// Bind data to the grid control
				this.BindData();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Binds the data.
		/// </summary>
		private void BindData()
		{
			this.AliasGrid.DataSource = this.Column.Aliases;
			this.AliasGrid.DataBind();
		}

		/// <summary>
		/// Handles the Click event of the CancelButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void CancelButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Redirect("LedgerAggregateColumnsForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Disables the edit delete.
		/// </summary>
		/// <param name="gridViewRow">The grid view row.</param>
		private void DisableEditDelete(GridViewRow gridViewRow)
		{
			var cell = (DataControlFieldCell)gridViewRow.Cells[3];//bds

			var commandField = cell.ContainingField as FMDeleteCommandField;

			if (commandField != null)
			{
				commandField.Enabled = false;
			}

			cell = (DataControlFieldCell)gridViewRow.Cells[0];

			var editField = cell.ContainingField as FMEditCommandField;

			if ( editField != null )
			{
				editField.Enabled = false;
			}
		}

		/// <summary>
		/// Enables the controls.
		/// </summary>
		/// <param name="enable">if set to <c>true</c> [b enable].</param>
		private void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;
			this.NameTextBox.Enabled = enable;
			this.FieldList.Enabled = enable;
			this.NewButton.Enabled = enable;
			this.OKButton.Enabled = enable;
			this.CustomFunctionTextBox.Enabled = enable;
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the FieldList control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void FieldListSelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.CustomFunctionLabel.Visible = this.FieldList.SelectedValue.Equals("CustomFunction");
				this.CustomFunctionTextBox.Visible = this.CustomFunctionLabel.Visible;

				if (this.CustomFunctionLabel.Visible == false)
				{
					this.CustomFunctionTextBox.Text = string.Empty;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Initializes the components.
		/// </summary>
		private void InitializeComponents()
		{
			this.CancelButton.Click += this.CancelButtonClick;
			this.AddButton.Click += this.AddButtonClick;
			this.AddButton2.Click += this.AddButtonClick;
			this.NewButton.Click += this.NewButtonClick;
			this.OKButton.Click += this.OkButtonClick;

			this.AliasGrid.RowEditing += this.AliasGridRowEditing;
			this.AliasGrid.RowCommand += this.AliasGrid_RowCommand;
			this.AliasGrid.RowCancelingEdit += this.AliasGridRowCancelingEdit;
			this.AliasGrid.RowDataBound += this.AliasGridRowDataBound;
			this.AliasGrid.RowUpdating += this.AliasGridRowUpdating;
			this.FieldList.SelectedIndexChanged += this.FieldListSelectedIndexChanged;
		}

		/// <summary>
		/// Handles the Click event of the NewButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void NewButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Save();
				this.Session[PageSessionKeyConstants.LEDGER_AGGREGATE_COLUMN_OBJECT] = new LedgerAggregateColumnClass();
				
				this.Redirect("LedgerAggregateColumnForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Click event of the OKButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void OkButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Save();
				
				this.Redirect("LedgerAggregateColumnsForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		private void Save()
		{
			// Retrieve data from non-grid controls
			this.Column.ID = this.NameTextBox.Text;

			this.Column.AggregateField =
				(LedgerAggregateColumnClass.AggregateType)
				Enum.Parse(typeof(LedgerAggregateColumnClass.AggregateType), this.FieldList.SelectedValue);

			this.Column.CustomFunctionName = this.CustomFunctionTextBox.Text;

			if (this.Column.IdentityGuid == Guid.Empty)
			{
				FMChannelHelper.MakeCall<ILedgerAggregateColumns, Guid>(x => x.Add(this.security, this.Column));
			}
			else
			{
				FMChannelHelper.MakeCall<ILedgerAggregateColumns>(x => x.Modify(this.security, this.Column));
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			this.NameTextBox.Text = this.Column.ID;

			// Set the title label with a key field from the bound object appended.
			if (this.Column != null)
			{
				this.TitleLabel.Text = this.GetTitleLabelText(this.TitleLabel.Text, this.Column.ID);
			}

			this.CustomFunctionTextBox.Text = this.Column.CustomFunctionName;
			this.FieldList.SelectedValue = this.Column.AggregateField.ToString();
			this.FieldListSelectedIndexChanged(null, null);
			this.BindData();
		}

		#endregion
	}
}