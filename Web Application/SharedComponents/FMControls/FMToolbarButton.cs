// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMToolbarButton.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Toolbar button generated on the fly. First developed for use in Dispatch.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System.Globalization;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	/// <summary>
	/// Toolbar button generated on the fly. First developed for use in Dispatch.
	/// </summary>
	public class FMToolbarButton : WebControl
	{
		#region Public Properties

		/// <summary>
		/// Initializes a new instance of the <see cref="FMToolbarButton"/> class.
		/// </summary>
		public FMToolbarButton(string img, string text, string id, string cssClass, string onClick, short tabIndex)
		{
			this.ID = id;
			this.CssClass = cssClass;

			this.MainControl = new HtmlGenericControl("li");

			this.SpanControl = new HtmlGenericControl("span");
			this.SpanControl.Attributes.Add("class", cssClass);
			this.SpanControl.Attributes.Add("onclick", onClick);
			this.SpanControl.Attributes.Add("ID", id);
			this.SpanControl.Attributes.Add("tabindex", tabIndex.ToString(CultureInfo.InvariantCulture));
			this.MainControl.Controls.Add(this.SpanControl);

			if (img != null)
			{
				this.ImageControl = new HtmlGenericControl("img");
				this.ImageControl.Attributes.Add("src", img);
				this.ImageControl.Attributes.Add("style", "vertical-align:bottom");
				this.ImageControl.Attributes.Add("alt", text);
				this.SpanControl.Controls.Add(this.ImageControl);
			}

			this.LabelControl = new HtmlGenericControl("label");
			this.LabelControl.InnerText = text;
			this.SpanControl.Controls.Add(this.LabelControl);
		}

		/// <summary>
		/// Gets the span control.
		/// </summary>
		protected HtmlGenericControl SpanControl { get; private set; }

		/// <summary>
		/// Gets the image control.
		/// </summary>
		protected HtmlGenericControl ImageControl { get; private set; }

		/// <summary>
		/// Gets the main control of this toolbar button.
		/// </summary>
		protected HtmlGenericControl MainControl { get; private set; }

		/// <summary>
		/// Gets the label control.
		/// </summary>
		protected HtmlGenericControl LabelControl { get; private set; }

		/// <summary>
		/// Renders the control to the specified HTML writer.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			// Write the control to the client page
			this.MainControl.RenderControl(writer);
		}

		#endregion
	}
}