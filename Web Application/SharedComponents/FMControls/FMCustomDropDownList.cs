// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMCustomDropDownList.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   FuelsManager custom drop down list class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	/// <summary>
	/// FuelsManager custom drop down list class
	/// </summary>
	public class FMCustomDropDownList : DropDownList
	{
		/// <summary>
		/// Renders the items in the ListControl control.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter</param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			foreach (ListItem item in this.Items)
			{
				writer.WriteBeginTag("option");

				if (item.Selected)
				{
					writer.WriteAttribute("selected", "selected", false);
				}

				writer.WriteAttribute("value", item.Value, true);

				if (item.Value.Equals("-1"))
				{
					writer.WriteAttribute("disabled", "true", false);
				}
				else if (item.Attributes["GroupColor"] != null)
				{
					// Color is value equipvalent to standard MediumBlue but shorter text.
					writer.WriteAttribute( "style", item.Attributes["GroupColor"], false );
				}

				writer.Write(HtmlTextWriter.TagRightChar);
				writer.Write(item.Text);
				writer.WriteEndTag("option");
				writer.WriteLine();
			}
		}
	}
}