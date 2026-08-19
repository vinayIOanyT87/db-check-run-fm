// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMConfirmationButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMConfirmationButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web;
using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]
namespace FMControls
{
	using System;

	/// <summary>
	/// Confirmation button for FuelsManager
	/// </summary>
	public class FMConfirmationButton : FMButton
	{
		#region Constants and Fields

		/// <summary>
		/// Gets or sets confirmation text.
		/// </summary>
		public string ConfirmationText { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Event handler for page load initialization.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The event arguments.
		/// </param>
		protected void FMConfirmationButtonLoad(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.ConfirmationText) == false)
			{
				string confirmText = HttpUtility.JavaScriptStringEncode(this.GetTranslationText(this.ConfirmationText));

				this.Attributes.Add("onClick", "if(disabled)return false; return confirm(\"" + confirmText + "\");");
			}
		}

		/// <summary>
		/// Initialization override for component.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Initialization routine for this component.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += this.FMConfirmationButtonLoad;
		}

		#endregion
	}
}