using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FMControls
{
	public enum CustomTextBoxType : short
	{
		INVOICE_QUERY,
		BULK_PAYMENT_INVOICES
	}

	public class FMCustomTextBox : FMTextBoxButtonControl
	{
		protected CustomTextBoxType m_type;

		#region Constructors
		public FMCustomTextBox ( )
		{
			m_type = CustomTextBoxType.INVOICE_QUERY; // default
		}

		public FMCustomTextBox ( CustomTextBoxType a_type )
		{
			m_type = a_type;
		}
		#endregion // Constructors

		#region Overrides
		protected override void Page_Load ( object sender, EventArgs e )
		{
			try
			{
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		protected override void Render ( HtmlTextWriter writer )
		{
			writer.WriteBeginTag ( "input" );
			writer.WriteAttribute ( "name", UniqueID );
			writer.WriteAttribute ( "type", "text" );
			writer.WriteAttribute ( "value", base.Text );
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
			writer.WriteAttribute ( "title", ToolTip );
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
			{
				writer.WriteAttribute ( "disabled", "disabled" );
			}

			keys = Style.Keys.GetEnumerator ( );

			style = "padding:0;width: 20px; height:20px";
			while (keys.MoveNext ( ))
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

			if (CustomTextBoxType.INVOICE_QUERY == m_type)
			{
				writer.WriteAttribute ( "onclick", "InvoiceQuerySelect('" + UniqueID + "')" );
			}
			else if (CustomTextBoxType.BULK_PAYMENT_INVOICES == m_type)
			{
				writer.WriteAttribute ( "onclick", "BulkPaymentInvoiceSelect('" + UniqueID + "')" );
			}
			else
			{
				writer.WriteAttribute ( "onclick", "javascript:alert('FMCustomTextBox: Invalid onclick action specified')" );
			}

			writer.WriteAttribute ( "type", "button" );
			writer.WriteAttribute ( "value", "..." );
			writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
		}
		#endregion // Overrides
	}
}
