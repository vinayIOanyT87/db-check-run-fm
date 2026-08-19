///***************************************************************************
/// Module Name:  FMMeterTextBox.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMControls
{
	using System;
	using System.Collections;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Allows a user to select a meter and displays the ID of the selected meter
	/// </summary>
	public class FMMeterTextBox : FMTextBoxButtonControl
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public FMMeterTextBox()
		{
		}

		/// <summary>
		/// When the page loads, check the user's security
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">not used</param>
		protected override void Page_Load(object sender, System.EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.Text)
				&& this.Text != "{All}")
			{
				if (this.Page.Session["Security"] == null)
				{
					throw new ArgumentNullException("Security");
				}

				SecurityClass security = this.Page.Session["Security"] as SecurityClass;

				if (security == null)
				{
					return;
				}
			}
			else
			{
				this.ToolTip = "Meter";
			}
		}

		/// <summary>
		/// Create a text box with a button, and have the button call the javascript function to select a meter.
		/// Code borrowed from the other select text boxes
		/// </summary>
		/// <param name="writer">used to write html to create the controls</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteBeginTag("input");
			writer.WriteAttribute("name", this.UniqueID);
			writer.WriteAttribute("type", "text");
			writer.WriteAttribute("value", HttpUtility.HtmlEncode(base.Text));
			writer.WriteAttribute("readonly", "readonly");

			if (this.AutoPostBack)
			{
				writer.WriteAttribute("onchange", "__doPostBack('" + this.UniqueID + "','')");
			}

			if (!this.Enabled)
			{
				writer.WriteAttribute("disabled", "disabled");
			}

			writer.WriteAttribute("id", this.UniqueID);
			writer.WriteAttribute("tabindex", "-1");
			writer.WriteAttribute("title", HttpUtility.HtmlEncode(this.ToolTip));
			writer.WriteAttribute("class", this.CssClass);

			IEnumerator keys = this.Style.Keys.GetEnumerator();

			string style = "background:#DDDDDD;width:" + (this.Width.Value - 5) + "px";

			while (keys.MoveNext())
			{
				string key = keys.Current.ToString();
				style += ";" + key + ": " + this.Style[key];
			}

			writer.WriteAttribute("style", style);

			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			writer.Write(writer.NewLine);

			// Add the Select button
			writer.WriteBeginTag("input");
			writer.WriteAttribute("class", "formfieldtitle");

			// Allow the read-only of this control to trigger
			// the disable behaviour of the button, leaving text readable
			if (!this.Enabled || this.ReadOnly)
			{
				writer.WriteAttribute("disabled", "disabled");
			}

			keys = this.Style.Keys.GetEnumerator();

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
					style += ";" + key + ": " + (Unit.Parse(this.Style[key]).Value + this.Width.Value + 5) + "px";
				}
				else
				{
					style += ";" + key + ": " + this.Style[key];
				}
			}

			writer.WriteAttribute("style", style);
			writer.WriteAttribute("onclick", "MeterSelect('" + this.UniqueID + "', '')");
			writer.WriteAttribute("type", "button");
			writer.WriteAttribute("value", "...");
			writer.WriteAttribute("id", UniqueID + " Select Button");

			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
		}
	}
}
