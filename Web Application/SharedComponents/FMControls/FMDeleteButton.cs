// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDeleteButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMDeleteButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// Wrapper class for presenting a Delete button with confirmation text.
	/// </summary>
	public sealed class FMDeleteButton : FMConfirmationButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDeleteButton"/> class.
		/// </summary>
		public FMDeleteButton()
		{
			this.ConfirmationText = "Are you sure you want to delete?";
		}

		#endregion
	}
}