// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="FMTextBox.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  Generally this class is not needed unless you are using multiline mode. This class 
//	overcomes a defect in the rendering of textareas to the browser in .NET where the MaxLength attribute 
//	is not rendered. This attribute works in conjunction with the MaxLength.HTC file to limit text in 
//	multiline text boxes. Singleline will work fine with the default rendering.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Globalization;
    using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMCore;

	/// <summary>
	/// Generally this class is not needed unless you are using multiline mode. This class overcomes 
	/// a defect in the rendering of textareas to the browser in .NET where the MaxLength attribute is 
	/// not rendered. This attribute works in conjunction with the MaxLength.HTC file to limit text 
	/// in multiline text boxes. Singleline will work fine with the default rendering.
	/// </summary>
	public class FMTextBox : TextBox
	{
		#region Methods

		/// <summary>
		/// Override method for rendering the control.
		/// </summary>
		/// <param name="writer">
		/// The writer.
		/// </param>
		protected override void Render(HtmlTextWriter writer)
		{
			if (this.TextMode == TextBoxMode.MultiLine)
			{
				writer.AddAttribute("MaxLength", this.MaxLength.ToString(CultureInfo.InvariantCulture));
			}
            // Overriding base TextBox render as MultiLine adds new line at start of textarea
            RenderBeginTag(writer);
            if (TextMode == TextBoxMode.MultiLine)
            {
                HttpUtility.HtmlEncode(Text, writer);
            }
            RenderEndTag(writer);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (this.TextMode == TextBoxMode.MultiLine && MaxLength <= 0)
			{
				var maxLength = System.Configuration.ConfigurationManager.AppSettings["TextBoxDefaultMaxLength"].DefaultIfNullOrEmpty("4096");

				int tmpTextBoxDefaultMaxLength = 4096;

				if (!int.TryParse(maxLength, out tmpTextBoxDefaultMaxLength))
				{
					tmpTextBoxDefaultMaxLength = 4096;
				}

				MaxLength = tmpTextBoxDefaultMaxLength;
			}

			if (String.IsNullOrEmpty(this.ToolTip))
			{
				GridViewRow grView = this.NamingContainer as GridViewRow;

				if (grView != null)
				{
					GridView gr = grView.NamingContainer as GridView;

					if (gr != null)
					{
						var t = this.Parent as DataControlFieldCell;

						if (t != null)
						{
							this.ToolTip = t.ContainingField.HeaderText;
						}
					}
				}
				else
				{
					DataGridItem dgi = this.NamingContainer as DataGridItem;
					if (dgi != null)
					{
						DataGrid gr = dgi.NamingContainer as DataGrid;

						if (gr != null)
						{
							var tc = this.Parent as TableCell;

							if (tc != null)
							{
								int inx = dgi.Cells.GetCellIndex(tc);
								if (inx > -1)
								{
									string ht = gr.Columns[inx].HeaderText;
									if (!string.IsNullOrEmpty(ht))
									{
										this.ToolTip = ht;
									}
								}
							}
						}
					}
				}
			}
		}

		#endregion
	}
}