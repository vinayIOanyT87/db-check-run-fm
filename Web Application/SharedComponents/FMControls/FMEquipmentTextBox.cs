// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMEquipmentTextBox.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMEquipmentTextBox.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Collections;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Customization code for text box for equipment selection.
	/// </summary>
	public class FMEquipmentTextBox : FMTextBoxButtonControl
	{
		#region Methods

		/// <summary>
		/// This is an abstract method to enforce the derived classes to implement the
		/// page load.
		/// </summary>
		/// <param name="sender">Object that sent the message.</param>
		/// <param name="e">Event arguments</param>
		protected override void Page_Load(object sender, EventArgs e)
		{
			this.buttonTitle = "Equipment select button";
			base.Page_Load(sender, e);

			try
			{
				if ( this.Page.IsPostBack == false )
				{
					if (!string.IsNullOrEmpty(this.Text) && this.Text != "{All}" && this.Text != "{Unassigned}")
					{
						if (this.Page.Session["Security"] == null)
						{
							throw new ArgumentNullException("Security");
						}

						var security = this.Page.Session["Security"] as SecurityClass;

						if (security == null)
						{
							return;
						}

						var equipment =
							FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
								x => x.Get(security, x.GetIdentityGuid(security, this.Text)));

						this.ToolTip = equipment.EquipmentToolTip;
					}
					else
					{
						this.ToolTip = string.Empty;
					}
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Renders the <see cref="T:System.Web.UI.WebControls.TextBox"/> control to the specified <see cref="T:System.Web.UI.HtmlTextWriter"/> object.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> that receives the rendered output.</param>
		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteBeginTag("input");
			writer.WriteAttribute("name", this.UniqueID);
			writer.WriteAttribute("type", "text");
			writer.WriteAttribute("value", HttpUtility.HtmlEncode(this.Text));
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
				var key = (string)keys.Current;
				style += ";" + key + ": " + this.Style[key];
			}

			writer.WriteAttribute("style", style);

			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			writer.Write(writer.NewLine);

			// Add the Select button
			writer.WriteBeginTag("input");
			writer.WriteAttribute("class", "formfieldtitle");
			writer.WriteAttribute("title", this.buttonTitle);

			// JS20100809 WI-14889 allow the read-only of this control to trigger
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

			writer.WriteAttribute("onclick", "EquipmentSelect('" + this.UniqueID + "')");
			writer.WriteAttribute("type", "button");
			writer.WriteAttribute("value", "...");
			writer.WriteAttribute("id", UniqueID + " Select Button");

			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
		}

		#endregion
	}
}