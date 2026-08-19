// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMTankTextBox.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <author>Richard Panachida</author>
// <summary>
//   Defines the FMTankTextBox type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Collections;
	using System.Globalization;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Provides customization of a textbox control to better support tank selection.
	/// </summary>
	public class FMTankTextBox : FMTextBoxButtonControl
	{
		/// <summary>
		/// This method will perform the page load by getting the tank information and
		/// setting the tooltip.
		/// </summary>
		/// <param name="sender">The sending object for the event.</param>
		/// <param name="e">The system event args paramenter.</param>
		protected override void Page_Load( object sender, EventArgs e )
		{
			if ( this.DesignMode == false
				&& string.IsNullOrEmpty(this.Text) == false
				&& (this.Text != "{All}") 
				&& ( this.Text != "{Unassigned}" ))
			{
				var security = (SecurityClass) Page.Session["Security"];

				Guid tankGuid = FMChannelHelper.MakeCall<ITanks, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, Text)
																);

				TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
																	 x =>
																	 x.Get(security, tankGuid)
																);

				this.ToolTip = tank.ProductID + ", " + tank.ManagerID;
			}
			else
			{
				this.ToolTip = string.Empty;
			}
		}

		/// <summary>
		/// This method will render the text box and button control. It overrides the 
		/// web control.
		/// </summary>
		/// <param name="writer">The writer being used to render the control - passed by event sender</param>
		protected override void Render ( HtmlTextWriter writer )
		{
			writer.WriteBeginTag ( "input" );
			writer.WriteAttribute ( "name", this.UniqueID );
			writer.WriteAttribute ( "type", "text" );
			writer.WriteAttribute ( "value", HttpUtility.HtmlEncode(this.Text));
			writer.WriteAttribute ( "readonly", "readonly" );

			if (this.AutoPostBack)
			{
				writer.WriteAttribute ( "onchange", "__doPostBack('" + this.UniqueID + "','')" );
			}

			if (this.Enabled == false)
			{
				writer.WriteAttribute ( "disabled", "disabled" );
			}

			writer.WriteAttribute ( "id", this.UniqueID );
			writer.WriteAttribute ( "tabindex", this.TabIndex.ToString(CultureInfo.InvariantCulture) );
			writer.WriteAttribute ( "title", HttpUtility.HtmlEncode(this.ToolTip));
			writer.WriteAttribute ( "class", this.CssClass );
			IEnumerator keys = Style.Keys.GetEnumerator();

			string style = "background:#DDDDDD;width:" + (Width.Value - 5) + "px";
			while (keys.MoveNext ( ))
			{
				var key = (string) keys.Current;
				style += ";" + key + ": " + this.Style[key];
			}

			writer.WriteAttribute ( "style", style );
			writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
			writer.Write ( writer.NewLine );

			// Add the Select button
			writer.WriteBeginTag ( "input" );
			writer.WriteAttribute ( "class", "formfieldtitle" );

			// JS20100809 WI-14889 allow the read-only of this control to trigger
			// the disable behaviour of the button, leaving text readable
			if (this.Enabled == false || this.ReadOnly)
			{
				writer.WriteAttribute ( "disabled", "disabled" );
			}

			keys = Style.Keys.GetEnumerator();

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
					style += ";" + key + ": " + this.Style[key];
				}
			}

			writer.WriteAttribute ( "style", style );
			writer.WriteAttribute ( "onclick", "TankSelect('" + this.UniqueID + "')" );
			writer.WriteAttribute ( "type", "button" );
			writer.WriteAttribute ( "value", "..." );
			writer.WriteAttribute("id", UniqueID + " Select Button");
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
		}
	}
}
