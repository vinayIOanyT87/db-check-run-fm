// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedgerViewsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for ledger views form
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System;
	using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;

	using FMControls;

	/// <summary>
	/// Code behind for ledger views form
	/// </summary>
	public partial class LedgerViewsForm : AccountingWebFormView
	{

		#region Methods

		/// <summary>
		/// Handles the RowCommand event of the LedgerViewGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="CommandEventArgs" /> instance containing the event data.</param>
		protected void LedgerViewGridRowCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("Edit", StringComparison.OrdinalIgnoreCase))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.LedgerViewGrid.Rows[rowIndex];

					var identityGuidLabel = (Label)row.FindControl("IdentityGuidLabel");
					Guid columnGuid = Guid.Parse(identityGuidLabel.Text);

					ListViewClass view =
						FMChannelHelper.MakeCall<IListViews, ListViewClass>(x => x.Get(this.security, LISTVIEW_TYPE.STANDARD, columnGuid));

					this.Session[PageSessionKeyConstants.LEDGER_VIEW_OBJECT] = view;

					this.Redirect("LedgerViewForm.aspx");
				}
				else if (e.CommandName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);
					GridViewRow row = this.LedgerViewGrid.Rows[rowIndex];

					var identityGuidLabel = (Label)row.FindControl("IdentityGuidLabel");
					Guid columnGuid = Guid.Parse(identityGuidLabel.Text);

					// See if the one that was just deleted was the one that was currently selected on the Ledger screen.
					// If so, clear the current LedgerViewSelection session variable.
					if (this.Session[PageSessionKeyConstants.LEDGER_VIEW_SELECTION] != null)
					{
						var selectedGuid = (Guid)this.Session[PageSessionKeyConstants.LEDGER_VIEW_SELECTION];

						if (selectedGuid != Guid.Empty)
						{
							if (columnGuid == selectedGuid)
							{
								this.Session[PageSessionKeyConstants.LEDGER_VIEW_SELECTION] = null;
							}
						}
					}

					var timer = new StopWatch(StopWatch.Appnames.Accounting, "LedgerViewsForm.RowCommand - views.Purge()");
					FMChannelHelper.MakeCall<IListViews>(x => x.Purge(this.security, LISTVIEW_TYPE.STANDARD, columnGuid));

					timer.Stop();

					timer.Start("LedgerViewsForm.RowCommand - UpdateView()");
					this.UpdateView();
					timer.Stop();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowDataBound event of the LedgerViewGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="GridViewRowEventArgs" /> instance containing the event data.</param>
		protected void LedgerViewGridRowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					var timer = new StopWatch(StopWatch.Appnames.Accounting, "LedgerViewsForm - RowDataBound");
					timer.Info("LedgerViewsForm - RowDataBound");

					var view = (ListViewClass)e.Row.DataItem;

					var deleteButton = (FMDeleteLinkButton)e.Row.FindControl("DeleteButton");
					if (deleteButton != null)
					{
						deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
						deleteButton.Enabled = view.SiteGuid == this.security.SiteGuid;
					}

					var editButton = (FMEditLinkButton)e.Row.FindControl("EditButton");
					if (editButton != null)
					{
						editButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			this.Initialize();
			this.InitializeControls();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.UpdateView();
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
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected void UpdateView()
		{
			var timer = new StopWatch(StopWatch.Appnames.Accounting, "LedgerViewsForm - UpdateView - EnumerateByTypeAndTypeGuid");

			ListViewCollectionClass viewsCollection =
				FMChannelHelper.MakeCall<IListViews, ListViewCollectionClass>(
					x =>
					x.EnumerateByTypeAndTypeGuid(
						this.security, LISTVIEW_TYPE.STANDARD, ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER)));
			
			timer.Stop();

			timer.Start("LedgerViewsForm - UpdateView - DataBind");
			this.LedgerViewGrid.DataSource = viewsCollection;
			this.LedgerViewGrid.DataBind();
			timer.Stop();
		}

		/// <summary>
		/// Handles the Click event of the AddButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void AddButtonClick(object sender, EventArgs e)
		{
			var view = new ListViewClass
				{
					Type = LISTVIEW_TYPE.STANDARD,
					TypeGuid = ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER),
					ListViewStandardType = LISTVIEW_STANDARD_TYPE.LEDGER
				};

			this.Session[PageSessionKeyConstants.LEDGER_VIEW_OBJECT] = view;
			
			this.Redirect("LedgerViewForm.aspx");
		}

		public void PopupAlert(string message)
		{
			string alertString = "<script type=\"text/javascript\">\r\n<!--\r\n";
			alertString += "alert(\"" + HttpUtility.JavaScriptStringEncode(message) + "\");";
			alertString += "\r\n--></script>";

			ScriptManager.RegisterClientScriptBlock(
				this.Page,
				this.GetType(),
				"LedgerView",
				alertString,
				false);
		}

		/// <summary>
		/// Handles the Click event of the Create Default View button control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void CreateDefaultLedgerViewButtonClick(object sender, EventArgs e)
		{
			try
			{
				string Msg = FMChannelHelper.MakeCall<IListViews, string>(x => x.CreateDefaultLedgerView(this.security));
				PopupAlert(Msg);
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Initializes the controls.
		/// </summary>
		private void InitializeControls()
		{
			this.AddButton.Click += this.AddButtonClick;
			this.AddButton2.Click += this.AddButtonClick;
			this.CreateDefaultLedgerViewButton.Click += this.CreateDefaultLedgerViewButtonClick;
			this.LedgerViewGrid.RowCommand += this.LedgerViewGridRowCommand;
			this.LedgerViewGrid.RowDataBound += this.LedgerViewGridRowDataBound;
		}

		#endregion
	}
}