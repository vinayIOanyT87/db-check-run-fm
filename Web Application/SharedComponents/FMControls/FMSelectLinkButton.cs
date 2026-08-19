// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMSelectLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMSelectLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// Select link button
	/// </summary>
	public class FMSelectLinkButton : FMLinkButton
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

			this.CommandName = "Select";
			this.ID = "SelectButton";
			this.ImageFile_Enabled = "Select.gif";
			this.ImageFile_Disabled = "Select_un.gif";
			this.alternateText = "Select this item";
		}

		#endregion
	}
}