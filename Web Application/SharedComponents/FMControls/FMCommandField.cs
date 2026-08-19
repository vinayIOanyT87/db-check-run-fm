// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMCommandField.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMCommandField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	/// <summary>
	/// Base class for a FuelsManager tailored command field.
	/// </summary>
	public abstract class FMCommandField : CommandField
	{
		#region Constants and Fields

		/// <summary>
		/// Internal use variable indicating whether the control is enabled.
		/// </summary>
		private bool enabled;

		/// <summary>
		/// Location of the central image directory
		/// </summary>
		protected const string ImageDirectory = "..\\FMWebApp\\images";

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMCommandField"/> class.
		/// </summary>
		public FMCommandField()
		{
			this.Enabled = true;
			this.ButtonType = ButtonType.Image;
			this.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
			this.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
			this.ItemStyle.VerticalAlign = VerticalAlign.Middle;
			this.HeaderStyle.Width = Unit.Pixel( 25 );
			this.ItemStyle.Width = Unit.Pixel( 25 );
			this.SiteGuid = Guid.Empty;
			this.UseDataDictionary = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a value indicating whether the control is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}

			set
			{
				this.enabled = value;
				this.BuildAndSetTheImageFileName();

				if (this.Cell != null)
				{
					this.Cell.Enabled = this.enabled;

					foreach (Control control in this.Cell.Controls)
					{
						var button = control as IButtonControl;
						var image = control as Image;
						if (image != null && button != null && button.CommandName.Length > 0)
						{
							if (button.CommandName == "Delete")
							{
								image.ImageUrl = this.DeleteImageUrl;
							}
							else if (button.CommandName == "Cancel")
							{
								image.ImageUrl = this.CancelImageUrl;
							}
							else if (button.CommandName == "Edit")
							{
								image.ImageUrl = this.EditImageUrl;
							}
							else if (button.CommandName == "Insert")
							{
								image.ImageUrl = this.InsertImageUrl;
							}
							else if (button.CommandName == "New")
							{
								image.ImageUrl = this.NewImageUrl;
							}
							else if (button.CommandName == "Select")
							{
								image.ImageUrl = this.SelectImageUrl;
							}
							else if (button.CommandName == "Update")
							{
								image.ImageUrl = this.UpdateImageUrl;
							}

							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets header text for the field control.
		/// </summary>
		public override string HeaderText
		{
			get
			{
				return base.HeaderText;
			}

			set
			{
				base.HeaderText = value;
				this.BuildAndSetTheImageFileName();
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether to use data dictionary.
		/// </summary>
		protected bool UseDataDictionary { get; set; }

		/// <summary>
		/// Gets or sets alternate text to place on the image of this control.
		/// </summary>
		protected abstract string AlternateText { get; set; }

		/// <summary>
		/// Gets image file to use for displaying the control as disabled.
		/// </summary>
		protected abstract string ImageFileDisabled { get; }

		/// <summary>
		/// Gets image file to use for displaying the control as enabled.
		/// </summary>
		protected abstract string ImageFileEnabled { get; }

		/// <summary>
		/// Gets or sets the site GUID.
		/// </summary>
		protected Guid SiteGuid { get; set; }

		/// <summary>
		/// Gets or sets Cell.
		/// </summary>
		protected DataControlFieldCell Cell { get; set; }

		/// <summary>
		/// Gets the name of the proper image file to use to display the control.
		/// </summary>
		protected string ImageFile
		{
			get
			{
				return this.Enabled ? this.ImageFileEnabled : this.ImageFileDisabled;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Override used to save the cell for this control.
		/// </summary>
		/// <param name="fieldCell">
		/// The cell.
		/// </param>
		/// <param name="cellType">
		/// The cell type.
		/// </param>
		/// <param name="rowState">
		/// The row state.
		/// </param>
		/// <param name="rowIndex">
		/// The row index.
		/// </param>
		public override void InitializeCell(
			DataControlFieldCell fieldCell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
		{
			base.InitializeCell(fieldCell, cellType, rowState, rowIndex);
			this.Cell = fieldCell;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Builds the image file name for the control.
		/// </summary>
		protected void BuildAndSetTheImageFileName()
		{
			string imageFile = ImageDirectory + "\\" + this.ImageFile;
			this.SetImageFileName(imageFile);
		}

		/// <summary>
		/// Sets the name of the image file.
		/// </summary>
		/// <param name="imageFile">The image file name.</param>
		protected abstract void SetImageFileName(string imageFile);

		#endregion
	}
}