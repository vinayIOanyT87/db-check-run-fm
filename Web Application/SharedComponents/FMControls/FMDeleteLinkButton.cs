// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDeleteLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMDeleteLinkButton type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.UI;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	using System;
	using System.Web;

	/// <summary>
	/// Delete link button for FuelsMnaager
	/// </summary>
	public sealed class FMDeleteLinkButton : FMLinkButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDeleteLinkButton"/> class.
		/// </summary>
		public FMDeleteLinkButton()
		{
			this.CommandName = "Delete";
			this.ID = "DeleteButton";
			this.ImageFile_Enabled = "Delete.gif";
			this.ImageFile_Disabled = "Delete_un.gif";
			this.alternateText = "Delete this item";
			this.ConfirmationText = "Are you sure you want to delete?";
		}
		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets ConfirmationText.
		/// </summary>
		public string ConfirmationText { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Page load event handler.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The event args.
		/// </param>
		private void FMDeleteLinkButtonLoad(object sender, EventArgs e)
		{
		    if (this.Enabled)
		    {
				string confirmText = HttpUtility.JavaScriptStringEncode(this.GetTranslatedText(this.ConfirmationText));
                this.Attributes.Add("onClick", "return confirm(\"" + confirmText + "\");");
		    }
		}

		/// <summary>
		/// Initialization override for the component.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.InitializeComponent();
		}

	    /// <summary>
		/// Initialization routine for the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += this.FMDeleteLinkButtonLoad;
		}

        /// <summary>
        /// In newer versions of the framework disabled controls don't always get the "disabled" attribute
        /// this causes the javascript which detects the button being disabled when determining to display the "Are you sure you want to delete?" prompt 
        /// to not work correctly. Overriding the SupportsDisabledAttribute allows us to still set the disabled attribute
        /// even when the controlRenderingCompatibilityVersion setting in web.config is greater than 3.5
        /// </summary>
	    public override bool SupportsDisabledAttribute
	    {
	        get
	        {
	            return true;
	        }
	    }

		#endregion
	}
}