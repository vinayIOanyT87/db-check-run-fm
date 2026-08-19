// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMUpLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMUpLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	using System;

	/// <summary>
	/// Uplink button 
	/// </summary>
	public class FMUpLinkButton : FMLinkButton
	{
		#region Methods

		/// <summary>
		/// Initialization overrride method for component.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit( EventArgs e )
		{
			base.OnInit(e);

			this.CommandName = "Up";
			this.ID = "UpButton";
			this.CausesValidation = false;
			this.ImageFile_Enabled = "up.gif";
			this.ImageFile_Disabled = "up_un.gif";
			this.alternateText = "Move this item up";
			this.Border = 0;
		}

		#endregion
	}
}