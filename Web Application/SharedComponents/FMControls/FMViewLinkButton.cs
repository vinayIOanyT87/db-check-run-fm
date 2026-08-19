// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMViewLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMViewLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// View link button tailored for FuelsManager
	/// </summary>
	public sealed class FMViewLinkButton : FMLinkButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMViewLinkButton"/> class.
		/// </summary>
		public FMViewLinkButton()
		{
			this.CommandName = "View";
			this.ID = "ViewButton";
			this.CausesValidation = false;
			this.ImageFile_Enabled = "FMICO_Search_16.gif";
			this.ImageFile_Disabled = "FMICO_Search_16g.gif";
			this.alternateText = "View this item";
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets AlternateText.
		/// </summary>
		public override string AlternateText
		{
			get
			{
				return this.alternateText;
			}

			set
			{
				this.alternateText = value;
			}
		}

		#endregion
	}
}