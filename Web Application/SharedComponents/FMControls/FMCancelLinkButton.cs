// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMCancelLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMCancelLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// Cancel link button control.
	/// </summary>
	public sealed class FMCancelLinkButton : FMLinkButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMCancelLinkButton"/> class.
		/// </summary>
		public FMCancelLinkButton()
		{
			this.CommandName = "Cancel";
			this.ID = "CancelButton";
			this.ImageFile_Enabled = "Cancel.gif";
			this.ImageFile_Disabled = "Cancel_un.gif";
			this.alternateText = "Cancel editing";
		}

		#endregion
	}
}