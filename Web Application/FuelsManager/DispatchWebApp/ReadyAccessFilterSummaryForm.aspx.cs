// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadyAccessFilterSummaryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for Ready Access FIlter Summary Form
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
	/// Code behind for Ready Access FIlter Summary Form
	/// </summary>
	public partial class ReadyAccessFilterSummaryForm : FMFormBase
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
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if ( this.IsPostBack == false )
				{
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Load( object sender, EventArgs e )
		{
			try
			{
				if ( this.IsPostBack == false )
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
		protected void FilterGridRowCommand(object sender, CommandEventArgs e)
		{
			try
			{
				throw new NotImplementedException();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
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
						FilterName = string.Format( "Filter {0}", index.ToString( CultureInfo.InvariantCulture ) ),
						FilterDescription = "This is the filter description."
					});
			}

			this.FilterGrid.DataSource = sampleData;
			this.FilterGrid.DataBind();
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
				this.Redirect("ReadyAccessFilterDetailForm.aspx?Mode=Add");
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		/// Handles the SelectedIndexChanged event of the PageSizeDropDown control.
		/// </summary>
		/// <param name="source">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void PageSizeDropDownSelectedIndexChanged( object source, EventArgs e )
		{
			try
			{
				this.UpdateView();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
		}

		protected void FindAllBtn_OnClick(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		protected void FindBtn_OnClick(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
