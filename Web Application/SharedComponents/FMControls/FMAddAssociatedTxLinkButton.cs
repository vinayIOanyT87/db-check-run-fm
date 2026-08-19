// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMAddAssociatedTxLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMAddAssociatedTxLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// Wrapper class for an the Add Associated Transaction Link button
	/// </summary>
	public sealed class FMAddAssociatedTxLinkButton : FMLinkButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMAddAssociatedTxLinkButton"/> class.
		/// </summary>
		public FMAddAssociatedTxLinkButton()
		{
			this.CommandName = "TxAddBtn";
			this.ID = "TxAddBtn";
			this.ImageFile_Enabled = "FMICO_add_16.gif";
			this.ImageFile_Disabled = "FMICO_add_16g.gif";
			this.alternateText = "Add Associated Transaction";
		}

		#endregion
	}
}