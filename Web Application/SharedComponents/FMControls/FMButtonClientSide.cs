// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMButtonClientSide.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMButtonClientSide.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.UtilityObjects;


	public class FMButtonClientSide : Button
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the FM Button Client Side control.
		/// </summary>
		public FMButtonClientSide ( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the translated text for the specified key.
		/// </summary>
		/// <param name="key">
		/// The key to translate.
		/// </param>
		/// <returns>
		/// Translated text.
		/// </returns>
		protected string GetTranslationText(string key)
		{
			string value = key;

			try
			{
				if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
				{
					if (this.Page.Session["SiteGuid"] != null)
					{
						var siteGuid = (Guid)this.Page.Session["SiteGuid"];

						value = DataDictionarySingleton.Get(siteGuid, key);
					}
				}
			}
			// ReSharper disable EmptyGeneralCatchClause
			catch (Exception)
			// ReSharper restore EmptyGeneralCatchClause
			{
			}

			return value;
		}


		/// <summary>
		/// This property will return the data dictionary value of the button label or set it.
		/// </summary>
		public string ButtonText
		{
			get { return this.DataDictionaryText ( ); }
			set { this.Text = value; }
		}
		#endregion

		/// <summary>
		/// This method overrides the render of the ASP button and renders an
		/// HTML button that the onclick is invoked on the client side function.
		/// The client side function must be named "FMButtonClientSideEvent()".
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render ( HtmlTextWriter writer )
		{
			string style = string.Empty;

			// Add the Select button
			writer.WriteBeginTag ( "input" );
			writer.WriteAttribute ( "class", "formfieldtitle" );

			if ( Enabled == false )
			{
				writer.WriteAttribute ( "disabled", "disabled" );
			}

			Unit widthUnit  = base.Width;
			Unit heightUnit = base.Height;

			if ( widthUnit.IsEmpty == true )
			{
				style = "width: 60px; ";
			}
			else
			{
				style = "width: " + widthUnit.ToString ( ) + "; ";
			}

			if ( heightUnit.IsEmpty == true )
			{
				style = style + "height: 24px";
			}
			else
			{
				style = style + "height: " + heightUnit.ToString ( );
			}

			if ( string.IsNullOrEmpty ( style ) == false )
			{
				writer.WriteAttribute ( "style", style );
			}

			writer.WriteAttribute ( "onclick", "FMButtonClientSideEvent()" );
			writer.WriteAttribute ( "type", "button" );
			writer.WriteAttribute ( "value", this.Text );
			writer.WriteAttribute ( "id", base.ID );
			writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
		}

		/// <summary>
		/// This method will return the button text or a data dictionary version of the 
		/// text.
		/// </summary>
		/// <returns></returns>
		private string DataDictionaryText ( )
		{
			try
			{
				if(Page.Session [ "UseDataDictionary" ] == null || ( bool ) Page.Session [ "UseDataDictionary" ])
				{
					this.GetTranslationText(this.Text);
				}
				else
				{
					return this.Text;
				}
			}
			catch
			{
				return this.Text;
			}

			return this.Text;
		}
	}
}
