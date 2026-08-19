/******************************************************************************
	FILE NAME:		FMCompartmentTextBox.cs
	PURPOSE:		Implementation of: FMCompartmentTextBox

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
*******************************************************************************/
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace FMControls
{
	/// <summary>
	/// Summary description for FMCompartmentTextBox.
	/// </summary>
	public class FMCompartmentTextBox : FMTextBoxButtonControl
	{
		public FMCompartmentTextBox ( )
		{
		}

		override protected void Page_Load ( object sender, System.EventArgs e )
		{
			try
			{
				ToolTip = "";
			}
			catch
			{
			}
		}

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
			writer.WriteAttribute ( "tabindex", TabIndex.ToString ( ) );
			writer.WriteAttribute ( "title", HttpUtility.HtmlEncode(ToolTip));
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
			// JS20100809 WI-14889 allow the read-only of this control to trigger
			// the disable behaviour of the button, leaving text readable
			if (!Enabled || ReadOnly)
				writer.WriteAttribute ( "disabled", "disabled" );

			keys = Style.Keys.GetEnumerator ( );

			style = "padding:0;width: 20px; height:20px";
			while (keys.MoveNext ( ))
			{
				string key = ((string)keys.Current).ToLower();
				if (key == "height")
					continue;

				if (key == "left")
					style += ";" + key + ": " + (Unit.Parse(Style[key]).Value + Width.Value + 5) + "px";
				else
					style += ";" + key + ": " + Style[key];
			}
			writer.WriteAttribute ( "style", style );

			writer.WriteAttribute ( "onclick", "CompartmentSelect('" + UniqueID + "')" );
			writer.WriteAttribute ( "type", "button" );
			writer.WriteAttribute ( "value", "..." );
			writer.WriteAttribute("id", UniqueID + " Select Button");

			writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
		}
	}
}
