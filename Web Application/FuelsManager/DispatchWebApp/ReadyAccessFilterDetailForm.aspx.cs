// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadyAccessFilterDetailForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind implementation for ready access filter detail form
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.DispatchWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	/// Code behind implementation for ready access filter detail form
	/// </summary>
	public partial class ReadyAccessFilterDetailForm : FMFormBase
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
		}

		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void Page_Init( object sender, EventArgs e )
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
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}
	}
}