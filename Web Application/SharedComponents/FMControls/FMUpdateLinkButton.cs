// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMUpdateLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMUpdateLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	using System;

	/// <summary>
	/// Update link button
	/// </summary>
	public class FMUpdateLinkButton : FMLinkButton
	{
		#region Methods

		/// <summary>
		/// Initialization overrride method for component.
		/// </summary>
		/// <param name="e">
		/// The event args. 
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			this.CommandName = "Update";
			this.ID = "UpdateButton";
			this.ImageFile_Enabled = "Update.gif";
			this.ImageFile_Disabled = "Update_un.gif";
			this.alternateText = "Save changes";

			base.OnInit(e);
		}

		#endregion
	}
}