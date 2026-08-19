// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDataGridFixedPaging.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMDataGridFixedPaging type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	using System.Drawing;

	/// <summary>
	/// Fixed paging data grid for FuelsManager
	/// </summary>
	public sealed class FMDataGridFixedPaging : FMDataGridFixedBase
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataGridFixedPaging"/> class.
		/// </summary>
		public FMDataGridFixedPaging()
		{
			this.PagerStyle.ForeColor = Color.Black;
			this.PagerStyle.BackColor = FMColor.HeaderBlue;
			this.PagerStyle.CssClass = "GVFixedFooter";
		}

		/// <summary>
		/// Gets or sets a value that indicates whether paging is enabled.
		/// </summary>
		/// <returns>True, since paging should always be enabled for this control</returns>
		public override bool AllowPaging
		{
			get
			{
				return true;
			}

			set
			{
			}
		}

		#endregion
	}
}