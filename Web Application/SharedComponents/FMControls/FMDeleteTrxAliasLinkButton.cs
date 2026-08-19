// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDeleteTrxAliasLinkButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Delete confirmation button tailored for transaction aliases.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Web;

	/// <summary>
	/// Delete confirmation button tailored for transaction aliases.
	/// </summary>
	public sealed class FMDeleteTrxAliasLinkButton : FMLinkButton
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDeleteTrxAliasLinkButton"/> class.
		/// </summary>
		public FMDeleteTrxAliasLinkButton()
		{
			this.CommandName = "Delete";
			this.ID = "DeleteButton";
			this.ImageFile_Enabled = "Delete.gif";
			this.ImageFile_Disabled = "Delete_un.gif";
			this.alternateText = "Delete this item";
		}

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
		private void FMDeleteTrxAliasLinkButtonLoad(object sender, EventArgs e)
		{
			string confirmText = HttpUtility.JavaScriptStringEncode(this.GetTranslatedText("Transaction records could be orphaned and cause ledger calculation errors. Are you sure you want to delete?"));
			this.Attributes.Add("onClick", "return confirm(\"" + confirmText + "\");");
		}

		/// <summary>
		/// Initialization override for the component.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit( EventArgs e )
		{
			base.OnInit(e);
			this.InitializeComponent();
		}

		/// <summary>
		/// Initialization routine for the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += this.FMDeleteTrxAliasLinkButtonLoad;
		}

		#endregion
	}
}