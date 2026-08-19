namespace FMControls
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	public class FMAutoComplete : TextBox
	{
		public string FieldKey { get; set; }
		public bool ClientAutoPost { get; set; }
		public string CallbackAddress { get; set; }
		public string LineItemNumber { get; set; }
		public string DependentFieldId { get; set; }

		protected override void Render(HtmlTextWriter writer)
		{
			writer.AddAttribute("class", "ui-widget");
			writer.AddAttribute("autoCompleteFieldKey", this.FieldKey);
			writer.AddAttribute("callBack", this.CallbackAddress);
			writer.AddAttribute("dependField", this.DependentFieldId);

			if (string.IsNullOrEmpty(this.LineItemNumber) == false)
			{
				writer.AddAttribute("lineItem", this.LineItemNumber);
			}

			if (this.ClientAutoPost)
			{
				this.Attributes["autoPost"] = "true";
			}
			else
			{
				this.Attributes["autoPost"] = "false";
			}

			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			base.Render(writer);

			// End of DIV
			writer.RenderEndTag();
		}
	}
}
