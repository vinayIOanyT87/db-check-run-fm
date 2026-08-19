// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrderEntrySplash.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for order entry splash.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.OrderEntryWebApp
{
	using System;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Code behind for order entry splash
	/// </summary>
	public partial class OrderEntrySplash : FMFormBase
	{
		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Put user code to initialize the page here
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		#endregion
	}
}