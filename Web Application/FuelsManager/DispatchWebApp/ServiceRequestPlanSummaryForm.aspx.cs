// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRequestPlanSummaryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for Service Request Plan Summary page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	/// Code behind for Service Request Plan Summary page.
	/// </summary>
	public partial class ServiceRequestPlanSummaryForm : FMFormBase
	{
		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit( EventArgs e )
		{
			base.OnInit( e );
			this.InitializeComponent();
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton1.Click += this.HandleAddButtonClickedEvent;
			this.AddButton2.Click += this.HandleAddButtonClickedEvent;
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
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					this.RefreshGridData();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the RowCommand event of the QueryGrid control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		protected void PlanGridRowCommand( object sender, CommandEventArgs e )
		{
			try
			{
				throw new NotImplementedException();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		/// Updates the grid data.
		/// </summary>
		protected void RefreshGridData()
		{
			// TODO: This is just sample data.  Implement proper data lookup with business objects and business service class
			var sampleData = new List<object>();
			for ( var index = 0; index < 12; ++index )
			{
				sampleData.Add(
					new
					{
						FilterName = string.Format( "Service Request Plan {0}", index.ToString( CultureInfo.InvariantCulture ) ),
						FilterDescription = "This is the service request plan description."
					});
			}

			this.ServiceRequestPlanGrid.DataSource = sampleData;
			this.ServiceRequestPlanGrid.DataBind();
		}

		/// <summary>
		/// Handles the add button clicked event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		private void HandleAddButtonClickedEvent( object sender, EventArgs e )
		{
			try
			{
				this.Redirect( "ServiceReqquestPlanDetailForm.aspx?Mode=Add");
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}
	}
}
