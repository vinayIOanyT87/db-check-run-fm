// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMCalendarSetLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMCalendarSetLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// Calendar set link button control
	/// </summary>
	public sealed class FMCalendarSetLinkButton : FMLinkButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMCalendarSetLinkButton"/> class.
		/// </summary>
		public FMCalendarSetLinkButton()
		{
			this.CommandName = "Set";
			this.ID = "SetButton";
			this.CausesValidation = false;
			this.ImageFile_Enabled = "edititem.gif";
			this.ImageFile_Disabled = "edititem_un.gif";
			this.alternateText = "Show Calendar";
		}

		#endregion
	}
}