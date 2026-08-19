// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlLogPrintFriendlyForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// The query printer friendly results.
	/// </summary>
	public partial class ControlLogPrintFriendlyForm : FMFormBaseAjax
	{
		#region Constants and Fields
		/// <summary>
		/// The site format info.
		/// </summary>
		private DateTimeFormatInfo siteFormatInfo;
		#endregion

		#region Methods
		/// <summary>
		/// The page_ init.
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

				SiteClass currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 sites => sites.Get(this.Security, this.Security.LoginSiteGuid, false, false, false));

				this.siteFormatInfo = currentSite.GetDateTimeFormatInfo();
				this.UpdateView();
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
		}

		/// <summary>
		/// The set local footer.
		/// </summary>
		protected void SetLocalFooter()
		{
			this.LocalFooter.Text = "Unclassified / For Official Use Only";
		}

		/// <summary>
		/// The set local header.
		/// </summary>
		protected void SetLocalHeader()
		{
			this.LocalHeader.Text = "Unclassified / For Official Use Only";
		}

		/// <summary>
		/// The set title.
		/// </summary>
		protected void SetTitle()
		{
			this.ControllerLogTitle.Text = "Controller Logs";
			this.Page.Title = this.ControllerLogTitle.Text;
		}

		/// <summary>
		/// The update view.
		/// </summary>
		protected void UpdateView()
		{
			// Set the Query local header
			this.SetLocalHeader();

			// Set the title
			this.SetTitle();

			// Show the results body
			this.GenerateTable();

			// Set the Query local footer
			this.SetLocalFooter();
		}

		/// <summary>
		/// This method will generate a table of for the controller log information to
		/// be printed.
		/// </summary>
		private void GenerateTable()
		{
			var tableRow = new TableRow();

			var tableCell = new TableCell();
			tableCell.Controls.Add(new LiteralControl("Date Time"));
			tableRow.Cells.Add(tableCell);

			tableCell = new TableCell();
			tableCell.Controls.Add(new LiteralControl("Controller"));
			tableRow.Cells.Add(tableCell);

			tableCell = new TableCell();
			tableCell.Controls.Add(new LiteralControl("Memo"));
			tableRow.Cells.Add(tableCell);

			this.ControllerLogPrintTable.Rows.Add(tableRow);

			var controllerLogList = this.Page.Session[ControlLogForm.DataListSessionKey] as List<ControllerLogClass>;

			if (controllerLogList != null && controllerLogList.Count > 0)
			{
				foreach(ControllerLogClass controllerLog in controllerLogList)
				{
					tableRow = new TableRow();

					tableCell = new TableCell();
					tableCell.Controls.Add(new LiteralControl(controllerLog.EventTime));
					tableRow.Cells.Add(tableCell);

					tableCell = new TableCell();
					tableCell.Controls.Add(new LiteralControl(controllerLog.Controller));
					tableRow.Cells.Add(tableCell);

					tableCell = new TableCell();
					tableCell.Controls.Add(new LiteralControl(controllerLog.Memo));
					tableRow.Cells.Add(tableCell);

					this.ControllerLogPrintTable.Rows.Add(tableRow);
				}
			}
		}
		#endregion
	}
}