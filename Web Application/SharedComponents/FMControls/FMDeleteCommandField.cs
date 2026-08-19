// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDeleteCommandField.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMDeleteCommandField type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web;

	/// <summary>
	/// Delete command field
	/// </summary>
	public sealed class FMDeleteCommandField : FMCommandField
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDeleteCommandField"/> class.
		/// </summary>
		public FMDeleteCommandField()
		{
			this.HeaderText = "Delete";
			this.ShowDeleteButton = true;
			this.ItemStyle.Width = Unit.Pixel(35);
			this.DeleteConfirmationText = "Are you sure you want to delete?";
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets delete confirmation prompt text.
		/// </summary>
		public string DeleteConfirmationText { get; set; }

		/// <summary>
		/// Gets or sets alternate text to place on the image of this control.
		/// </summary>
		protected override string AlternateText
		{
			get
			{
				return this.DeleteText;
			}

			set
			{
				this.DeleteText = value;
			}
		}

		/// <summary>
		/// Gets image file to use for displaying the control as disabled.
		/// </summary>
		protected override string ImageFileDisabled
		{
			get
			{
				return "delete_un.gif";
			}
		}

		/// <summary>
		/// Gets image file to use for displaying the control as enabled.
		/// </summary>
		protected override string ImageFileEnabled
		{
			get
			{
				return "delete.gif";
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Used to handle setting important information on parts of the control during initialization.
		/// </summary>
		/// <param name="fieldCell">
		/// The field cell.
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
			if (!string.IsNullOrEmpty(this.DeleteConfirmationText) && this.ShowDeleteButton)
			{
				foreach (Control control in fieldCell.Controls)
				{
					var button = control as IButtonControl;
					if (button != null && button.CommandName == "Delete")
					{
						// Add delete confirmation
						((WebControl)control).Attributes.Add(
							"onclick", string.Format("if (!confirm(\"{0}\")) return false;", HttpUtility.JavaScriptStringEncode(this.DeleteConfirmationText)));
					}
				}
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
			this.DeleteImageUrl = imageFile;
		}

		#endregion
	}
}