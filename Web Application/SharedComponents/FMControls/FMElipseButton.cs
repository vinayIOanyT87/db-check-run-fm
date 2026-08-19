/*
 * Revision History
 * Date:		By:				Reason:
 * 08-22-2008	V. Thompson		Made the buttom dimensions match the company select button dimensions
 * 
 * 09-04-2008	V. Thompson		Added 2 additional data fields
 * */

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace FMControls
{
	/// <summary>
	/// Displays an elipse button intended to be used as a trigger for client-side events
	/// </summary>
	public class FMElipseButton : System.Web.UI.WebControls.WebControl
	{
		private string itemIndex;
		private string onClick;
		private string data;
		private string data2;
		private string data3;

//		private string cssClass;

		public FMElipseButton()
		{
			this.Load += new EventHandler(FMElipseButton_Load);
		}

		/// <summary>
		/// The unique identifier for an object related to the elipse button
		/// </summary>
		public string ItemIndex
		{
			get { return itemIndex; }
			set { itemIndex = value; }
		}

		/// <summary>
		/// The client-side script fuction that will handle the elipse button click
		/// </summary>
		public string OnClick
		{
			get { return onClick; }
			set { onClick = value; }
		}

		/// <summary>
		/// Any string data related to the elipse button click that needs to be captured
		/// </summary>
		public string Data
		{
			get { return data; }
			set { data = value; }
		}

		public string Data2
		{
			get { return data2; }
			set { data2 = value; }
		}

		public string Data3
		{
			get { return data3; }
			set { data3 = value; }
		}

//		public string CssClass
//		{
//			get { return cssClass; }
//			set { cssClass = value; }
//		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteBeginTag("input");
			writer.WriteAttribute("name", this.UniqueID);
			writer.WriteAttribute("id", this.UniqueID);
			writer.WriteAttribute("type", "button");
			writer.WriteAttribute("value", "...");
			writer.WriteAttribute("class", this.CssClass);
			writer.WriteAttribute("style", "width: 20px; height: 20px;");
			writer.WriteAttribute("title", this.ToolTip);
			writer.WriteAttribute("onclick", onClick);
			if (!this.Enabled)
				writer.Write(" DISABLED ");
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);

			// Add a hidden field that will contain the data for the control
			writer.WriteBeginTag("input");
			writer.WriteAttribute("name", "hid" + this.UniqueID);
			writer.WriteAttribute("id", "hid" + this.UniqueID);
			writer.WriteAttribute("type", "hidden");
			writer.WriteAttribute("value", data);
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);

			// Add hidden fields for data2 and data3
			writer.WriteBeginTag("input");
			writer.WriteAttribute("name", "hidData2" + this.UniqueID);
			writer.WriteAttribute("id", "hidData2" + this.UniqueID);
			writer.WriteAttribute("type", "hidden");
			writer.WriteAttribute("value", data2);
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);

			// Data 3
			writer.WriteBeginTag("input");
			writer.WriteAttribute("name", "hidData3" + this.UniqueID);
			writer.WriteAttribute("id", "hidData3" + this.UniqueID);
			writer.WriteAttribute("type", "hidden");
			writer.WriteAttribute("value", data3);
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
		}

		private void FMElipseButton_Load(object sender, EventArgs e)
		{
			if (Page.IsPostBack)
			{
				this.data = Page.Request.Form["hid" + this.UniqueID];
				this.data2 = Page.Request.Form["hidData2" + this.UniqueID];
				this.data3 = Page.Request.Form["hidData3" + this.UniqueID];
			}
		}
	}
}
