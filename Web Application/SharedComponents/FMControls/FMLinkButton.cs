/******************************************************************************
	FILE NAME:		FMLinkButton.aspx.cs
	PURPOSE:		Implementation of FMEditLinkButton, FMDeleteLinkButton,
					FMSelectLinkButton, FMUpdateLinkButton, FMCancelLinkButton,
					FMAddSubLineItemLinkButton, FMViewAssociatedTxLinkButton,
					FMAddAssociatedTxLinkButton

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2007-03-14	Richard Panachida	For performance issues, created the data dictionary object
										one time and stored into the session. For each original text
										seeking translation, add the key and translated value to session (CSI 4305).
 
		2009-06-24	I.Orndorff			- Added ShowDeleted property.
												- Modified ImageFile property to select image based on new ShowDeleted
												  property. This addresses task 4128. 

		2009-10-21  C. Knight           - Make AlternateText overridable - Bug 4622
*******************************************************************************/
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FMBusinessObjects.UtilityObjects;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{

	public class FMLinkButton : System.Web.UI.WebControls.LinkButton
	{
		protected bool bUseDataDictionary = true;
		protected bool bShowDeleted = false;
		protected Guid SiteGuid = Guid.Empty;

		protected string ImageFile_Enabled = null;
		protected string ImageFile_Disabled = null;
		protected string Deleted_ImageFile_Enabled = null;
		protected string Deleted_ImageFile_Disabled = null;
		protected string alternateText = "";

		public const string DATA_DICTIONARY_KEY = "DataDictionaryKey";
		public const string LINK_KEY = "LinkKey";
		public int Border { get; set; }

		public FMLinkButton ( )
		{
			Border = 0;
		}

		protected void Page_Load ( object sender, System.EventArgs e )
		{
			if (Page.Session["UseDataDictionary"] == null || (bool) Page.Session["UseDataDictionary"])
			{
				if (Page.Session["SiteGuid"] == null)
				{
					return;
				}

				bUseDataDictionary = true;
				SiteGuid = (Guid)Page.Session["SiteGuid"];
			}
			else
			{
				bUseDataDictionary = false;
			}

			SetText ( Enabled );
		}

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
		protected override void OnInit( EventArgs e )
		{
			this.InitializeComponent();
			base.OnInit ( e );
			this.ToolTip = this.CommandName + " button";

			if ( this.DesignMode == false
				&& Page.Session["SiteGuid"] != null 
				&& ( Page.Session["UseDataDictionary"] == null || (bool) Page.Session["UseDataDictionary"] ) )
			{
				this.bUseDataDictionary = true;
				this.SiteGuid = (Guid)Page.Session["SiteGuid"];
			}
			else
			{
				this.bUseDataDictionary = false;
			}
		}

		private void InitializeComponent ( )
		{
			this.Load += new EventHandler ( this.Page_Load );
		}

		/// <summary>
		/// This method will translate the orginal text to a data dictionary text. If there is
		/// no data dictionary text, then the original text is returned. In addition, for performance
		/// the new translated text is stored in the session.
		/// </summary>
		/// <param name="originalText"></param>
		/// <returns></returns>
		public string GetTranslatedText ( string originalText )
		{
			string linkKey = FMLinkButton.LINK_KEY + originalText;
			string translatedText = originalText;

			try
			{
				if (bUseDataDictionary == true)
				{
					if (Page.Session[linkKey] == null)
					{

						translatedText = DataDictionarySingleton.Get(SiteGuid, originalText);

						Page.Session.Add ( linkKey, translatedText );
					}
					else
					{
						translatedText = (string) Page.Session[linkKey];
					}
				}
			}
			catch
			{
			}

			return translatedText;
		}

		override public bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				base.Enabled = value;

				// Make sure the control has been initialized before doing this
				if (this.Page != null)
				{
					SetText ( value );
				}
			}
		}

		protected void SetText ( bool bEnabled )
		{
			string imageFile = ImageFile;
			string altText = GetTranslatedText ( AlternateText );

			if (!this.Enabled)
			{
				altText += " disabled ";
				if (this is FMControls.FMDeleteLinkButton)
				{
					// find the onclick attribute
					if(this.Attributes.Count > 0)
					{
						this.Attributes.Remove("onClick");
					}
				}
			}

			this.Controls.Clear();
			//Text = "<img src=../FMWebApp/Images/" + imageFile + " style='border:solid " + this.Border + "pt black'   " + " align=absmiddle alt='" + altText + "'>";
			Image img = new Image();
			img.ImageUrl = "../FMWebApp/Images/" + imageFile;
			img.Style.Add("border", "solid " + this.Border + "pt black");
			img.Attributes.Add("align", "absmiddle");
			img.Attributes.Add("alt", altText);

			this.Controls.Add(img);
		}

		protected string ImageFile
		{
			get
			{
				// Show different image for delete items (25-Jun-2009 IGO)
				if (ShowDeleted)
				{
					return ( Enabled ) ? Deleted_ImageFile_Enabled : Deleted_ImageFile_Disabled;
				}
				else
				{
					return ( Enabled ) ? ImageFile_Enabled : ImageFile_Disabled;
				}
			}
		}

		virtual public string AlternateText
		{
			get { return this.alternateText; }
			set { this.alternateText = value; }
		}

		public bool ShowDeleted
		{
			get { return this.bShowDeleted; }
			set
			{
				this.bShowDeleted = value;
				SetText ( Enabled );
			}
		}
	}
}
