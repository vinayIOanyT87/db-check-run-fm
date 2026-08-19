// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMViewCommandField.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMViewCommandField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	/// <summary>
	/// View command field.
	/// </summary>
	public sealed class FMViewCommandField : FMCommandField
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMViewCommandField"/> class.
		/// </summary>
		public FMViewCommandField()
		{
			this.HeaderText = "View";
			this.ShowInsertButton = true;
			this.AlternateText = "View";
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
				return this.NewText;
			}

			set
			{
				this.NewText = value;
			}
		}

		/// <summary>
		/// Gets the image file to use for displaying the control as disabled.
		/// </summary>
		protected override string ImageFileDisabled
		{
			get
			{
				return "FMICO_Search_16g.gif";
			}
		}

		/// <summary>
		/// Gets the image file to use for displaying the control as enabled.
		/// </summary>
		protected override string ImageFileEnabled
		{
			get
			{
				return "FMICO_Search_16.gif";
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
			this.NewImageUrl = imageFile;
		}

		#endregion
	}
}