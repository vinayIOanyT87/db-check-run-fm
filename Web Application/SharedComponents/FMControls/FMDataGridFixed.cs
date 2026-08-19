// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDataGridFixed.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMDataGridFixed type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	/// <summary>
	/// Fixed data grid that does not allow paging.
	/// </summary>
	public class FMDataGridFixed : FMDataGridFixedBase
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets a value that indicates whether paging is enabled.
		/// </summary>
		/// <returns>true if paging is enabled; otherwise, false. The default value is false.</returns>
		public override bool AllowPaging
		{
			get
			{
				return false;
			}

			set
			{
			}
		}

		#endregion
	}
}