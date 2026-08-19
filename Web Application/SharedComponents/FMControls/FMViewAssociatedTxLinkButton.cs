// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMViewAssociatedTxLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMViewAssociatedTxLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// View associated transactions link button class
	/// </summary>
	public class FMViewAssociatedTxLinkButton : FMLinkButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initialization overrride method for component.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit( System.EventArgs e )
		{
			base.OnInit( e );

			this.ID = "TxViewBtn";
			this.CommandName = "TxViewBtn";
			this.ImageFile_Enabled = "FMICO_search_16.gif";
			this.ImageFile_Disabled = "FMICO_search_16g.gif";
			this.alternateText = "View Associated Transactions";
		}

		#endregion
	}
}