// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMEditCommandField.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMEditCommandField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Web.UI.WebControls;

	/// <summary>
	/// Edit command field.
	/// </summary>
	public sealed class FMEditCommandField : FMCommandField
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMEditCommandField"/> class.
		/// </summary>
		public FMEditCommandField()
		{
			this.HeaderText = "Edit";
			this.ShowEditButton = true;
			this.ItemStyle.Width = Unit.Pixel(50);
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
				return this.EditText;
			}

			set
			{
				this.EditText = value;
			}
		}

		/// <summary>
		/// Gets the image file to use for displaying the control as disabled.
		/// </summary>
		protected override string ImageFileDisabled
		{
			get
			{
				return "edit_un.gif";
			}
		}

		/// <summary>
		/// Gets the image file to use for displaying the control as enabled.
		/// </summary>
		protected override string ImageFileEnabled
		{
			get
			{
				return "edit.gif";
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
			const string ImageDir = ImageDirectory;

			this.EditImageUrl = imageFile;
			this.UpdateImageUrl = string.Format("{0}\\" + "update.gif", ImageDir);
			this.CancelImageUrl = string.Format("{0}\\" + "cancel.gif", ImageDir);
		}

		#endregion
	}
}