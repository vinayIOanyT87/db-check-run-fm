// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMSelectCommandField.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMSelectCommandField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	/// <summary>
	/// Select command field
	/// </summary>
	public sealed class FMSelectCommandField : FMCommandField
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMSelectCommandField"/> class.
		/// </summary>
		public FMSelectCommandField()
		{
			this.HeaderText = "Select";
			this.ShowSelectButton = true;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets alternate text to place on the image of this control.
		/// </summary>
		protected override string AlternateText
		{
			get
			{
				return this.SelectText;
			}

			set
			{
				this.SelectText = value;
			}
		}

		/// <summary>
		/// Gets the image file to use for displaying the control as disabled.
		/// </summary>
		protected override string ImageFileDisabled
		{
			get
			{
				return "select_un.gif";
			}
		}

		/// <summary>
		/// Gets the image file to use for displaying the control as enabled.
		/// </summary>
		protected override string ImageFileEnabled
		{
			get
			{
				return "select.gif";
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the name of the image file.
		/// </summary>
		/// <param name="imageFile">The image file name.</param>
		protected override void SetImageFileName( string imageFile )
		{
			this.SelectImageUrl = imageFile;
		}

		#endregion
	}
}