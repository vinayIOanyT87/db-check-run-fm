/******************************************************************************
	FILE NAME:		FMFuelCardTextBox.cs
	PURPOSE:		Implementation of: FMFuelCardTextBox

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	Richard Panachida
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		01/26/2009	W.Gray				7.4.6.0 - Revised to not request authorized companies on
												FuelCards.Get
		
*******************************************************************************/

using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace FMControls
{
    using System.Web;

    public class FMFuelCardTextBox : FMTextBoxButtonControl
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor the FMFuelCardTextBox base class
		/// </summary>
		public FMFuelCardTextBox ( )
		{
		}
		#endregion

		/// <summary>
		/// This method will perform the page load by getting the FuelCard information and
		/// setting the tooltip.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		override protected void Page_Load ( object sender, System.EventArgs e )
		{
			this.buttonTitle = "Fuel card select button";
			base.Page_Load(sender, e);

			try
			{
				if (Text != null &&
					 Text != "" &&
					 Text != "{All}" &&
					 Text != "{Unassigned}")
				{
					if (Page.Session["Security"] == null)
					{
						throw new ArgumentNullException ( "Security" );
					}

					SecurityClass security = Page.Session["Security"] as SecurityClass;

					if (security == null)
					{
						return;
					}

					Guid identityGuid = FMChannelHelper.MakeCall<IFuelCards, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, Text)
																);

					FuelCardClass FuelCard = FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(
																	 x =>
																	 x.Get(security, identityGuid, false)
																);
				}
				else
				{
					ToolTip = "Fuel Card";
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// This method will render the text box and button control. It overrides the 
		/// web control.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render ( HtmlTextWriter writer )
		{
			writer.WriteBeginTag ( "input" );
			writer.WriteAttribute ( "name", UniqueID );
			writer.WriteAttribute ( "type", "text" );
			writer.WriteAttribute ( "value", HttpUtility.HtmlEncode(base.Text) );
			writer.WriteAttribute ( "readonly", "readonly" );

			if (AutoPostBack)
			{
				writer.WriteAttribute ( "onchange", "__doPostBack('" + UniqueID + "','')" );
			}

			if (!Enabled)
			{
				writer.WriteAttribute ( "disabled", "disabled" );
			}

			writer.WriteAttribute ( "id", UniqueID );
			writer.WriteAttribute ( "tabindex", "-1" );
			writer.WriteAttribute ( "title", HttpUtility.HtmlEncode(ToolTip) );
			writer.WriteAttribute ( "class", CssClass );
			IEnumerator keys = Style.Keys.GetEnumerator ( );

			string style = "background:#DDDDDD;width:" + (Width.Value - 5) + "px";
			while (keys.MoveNext ( ))
			{
				string key = (string) keys.Current;
				style += ";" + key + ": " + Style[key];
			}

			writer.WriteAttribute ( "style", style );
			writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
			writer.Write ( writer.NewLine );

			// Add the Select button
			writer.WriteBeginTag ( "input" );
			writer.WriteAttribute ( "class", "formfieldtitle" );
			writer.WriteAttribute("title", this.buttonTitle);

			// JS20100809 WI-14889 allow the read-only of this control to trigger
			// the disable behaviour of the button, leaving text readable
			if (!Enabled || ReadOnly)
			{
				writer.WriteAttribute ( "disabled", "disabled" );
			}

			keys = Style.Keys.GetEnumerator ( );

			style = "padding:0;width: 20px; height:20px";
			while (keys.MoveNext())
			{
				string key = ((string)keys.Current).ToLower();

				if (key == "height")
				{
					continue;
				}

				if (key == "left")
				{
					style += ";" + key + ": " + (Unit.Parse(Style[key]).Value + Width.Value + 5) + "px";
				}
				else
				{
					style += ";" + key + ": " + Style[key];
				}
			}

			writer.WriteAttribute ( "style", style );
			writer.WriteAttribute ( "onclick", "FuelCardSelect('" + UniqueID + "')" );
			writer.WriteAttribute ( "type", "button" );
			writer.WriteAttribute ( "value", "..." );
			writer.WriteAttribute("id", UniqueID + " Select Button");
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
		}
	}
}
