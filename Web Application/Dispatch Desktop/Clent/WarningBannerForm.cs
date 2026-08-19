// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WarningBannerForm.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Code behind for warning banner.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dispatch
{
	using System;

	/// <summary>
	/// Code behind for warning banner.
	/// </summary>
	public partial class WarningBannerForm : FMBaseForm
	{
		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="WarningBannerForm" /> class.
		/// </summary>
		public WarningBannerForm()
		{
			this.InitializeComponent();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Handles the Click event of the AcceptButton control.
		/// </summary>
		/// <param name="sender">
		/// The source of the event.
		/// </param>
		/// <param name="e">
		/// The <see cref="EventArgs"/> instance containing the event data.
		/// </param>
		private void AcceptButtonClick(object sender, EventArgs e)
		{
			try
			{
				this.Close();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion
	}
}