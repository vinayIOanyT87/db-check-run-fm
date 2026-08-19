// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMAddSubLineItemLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMAddSubLineItemLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// Wrapper class for an the Add Sub Line Item Link button
	/// </summary>
	public sealed class FMAddSubLineItemLinkButton : FMLinkButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMAddSubLineItemLinkButton"/> class.
		/// </summary>
		public FMAddSubLineItemLinkButton()
		{
			this.CommandName = "AddSubLineItem";
			this.ID = "AddSubLineItemButton";
			this.ImageFile_Enabled = "Select.gif";
			this.ImageFile_Disabled = "Select_un.gif";
			this.alternateText = "Add Subline-item";
		}

		#endregion
	}
}