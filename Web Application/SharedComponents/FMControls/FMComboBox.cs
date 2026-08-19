// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Varec, Inc." file="FMComboBox.cs">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   A customization of the AjaxControlToolkit ComboBox control.
//   Ensures that there is always a selected item, the first one
//   by default.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace FMControls
{
	using System;
	using System.Web.UI.WebControls;

	using AjaxControlToolkit;

	/// <summary>
	///   A customization of the AjaxControlToolkit ComboBox control.
	///   Ensures that there is always a selected item, the first one
	///   by default.
	/// </summary>
	public class FMComboBox : ComboBox
	{
		#region Public Properties

		/// <summary>
		/// Gets the hidden field CNTRL.
		/// </summary>
		public HiddenField HiddenFieldCntrl
		{
			get
			{
				return this.HiddenFieldControl;
			}
		}

		// Replaces the Items.Clear() method - use this instead.

		/// <summary>
		///   Gets or sets the index of the selected item. If getting and no item is
		///   selected, then selects the first one.
		/// </summary>
		public override int SelectedIndex
		{
			get
			{
				// We want the first item selected when we start
				if (base.SelectedIndex < 0 && this.Items.Count > 0)
				{
					base.SelectedIndex = 0;
				}

				return base.SelectedIndex;
			}

			set
			{
			//	if (base.Items.Count > value)
				{
					base.SelectedIndex = value;
				}
			}
		}


		/// <summary>
		///   Gets the selected item. If no item is selected, it selects the first one.
		/// </summary>
		public override ListItem SelectedItem
		{
			get
			{
				// We want the first item selected when we start
				if (base.SelectedIndex < 0 && this.Items.Count > 0)
				{
					base.SelectedIndex = 0;
				}

				return base.SelectedItem;
			}
		}

		/// <summary>
		///   Gets or sets the value of the selected item. If getting and no item is
		///   selected, it selects the first one.
		/// </summary>
		public override string SelectedValue
		{
			get
			{
				// We want the first item selected when we start
				if (base.SelectedIndex < 0 && this.Items.Count > 0)
				{
					base.SelectedIndex = 0;
				}

				return base.SelectedValue;
			}

			set
			{
				base.SelectedValue = value;
			}
		}

		/// <summary>
		/// Gets the text box CNTRL.
		/// </summary>
		public TextBox TextBoxCntrl
		{
			get
			{
				return this.TextBoxControl;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			this.Items.Clear();

			// This next line seems unnecessary, but without it, it will throw on the
			// next data bind
			this.SelectedIndex = -1;

			this.HiddenFieldControl.Value = "0";
		}

		/// <summary>
		/// Selects the item in the combobox that has the sent text.
		/// Also return selected index or -1 on failure.
		/// </summary>
		/// <param name="itemText">The s item text.</param>
		/// <returns>The selected index if successful; otherwise, -1</returns>
		public int SelectByText(string itemText)
		{
			if (string.IsNullOrEmpty(itemText))
			{
				return -1;
			}

			var item = this.Items.FindByText(itemText);

			if (item == null)
			{
				return -1;
			}

			this.SelectedIndex = this.Items.IndexOf( item );

			return this.SelectedIndex;
		}


		#endregion

		#region Methods
		protected void Add508ComplianceAttributes()
		{
			System.Web.UI.Control c = this.Parent;

			if (c == null)
				return;

			while (c != null)
			{
				string str = c.ID;
				//Go up the chain of controls until finding control with ID containing "FieldValue". This works
				//if fmcontrols used within a grid.
				if (!string.IsNullOrEmpty(str) && str.Contains("FieldValue"))
				{
					//Look for control representing the label for this control. It should have an ID that contains FieldLabel. 
					string label = str.Replace("FieldValue", "FieldLabel");
					c = c.Parent.FindControl(label);
					if (c != null)
					{
						if (c.Controls.Count > 0)
						{
							c = c.Controls[0];
						}
						else
						{
							if (c is TableCell)
							{
								TableCell tc = c as TableCell;

								tc.Attributes.Add("role", "presentation");

								string txt = tc.Text;
								if (txt.Substring(0, 3) == "<a ")
								{
									int p0 = txt.IndexOf(" id=\"", StringComparison.OrdinalIgnoreCase);
									if (p0 > 1)
									{
										int p1 = txt.IndexOf("\"", p0 + 6, StringComparison.OrdinalIgnoreCase);
										if (p1 > p0 + 6)
										{
											string id = txt.Substring(p0 + 5, p1 - p0 - 5);
											this.TextBoxControl.Attributes.Add("aria-labelledby", id);

										}
									}
									p0 = txt.IndexOf(">", StringComparison.OrdinalIgnoreCase);
									if (p0 > 1)
									{
										int p1 = txt.IndexOf("</", p0 + 2, StringComparison.OrdinalIgnoreCase);
										if (p1 > p0 + 2)
										{
											string id = txt.Substring(p0 + 1, p1 - p0 - 1);
											this.Attributes["alt"] = id;
											tc.Attributes.Add("aria-label", "Label for " + id);
										}
									}
								}
								else
								{

								}
							}

						}
					}
					break;
				}
				c = c.Parent;
			}


			string alt = this.Attributes["alt"];
			if (string.IsNullOrEmpty(alt))
				alt = this.ID;
			base.ComboTable.Attributes.Add("aria-label", alt);
			base.ComboTable.Attributes.Add("role", "presentation");
			if (this.TextBoxCntrl != null)
			{
				this.TextBoxCntrl.Attributes["alt"] = alt;
			}


		}

		////public override void RenderBeginTag(System.Web.UI.HtmlTextWriter writer)
		////{


		////	base.RenderBeginTag(writer);
		////}
 
		////protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
		////{

		////	//var x = this.TagName;
		////	//if (x == "div")
		////	{
		////		var e = this.Attributes["onchange"];
		////		if (!string.IsNullOrEmpty(e))
		////		{
		////			this.Attributes.Remove("onchange");
		////			this.Attributes.Add("onclick", e);
		////		}
		////	}
		////	base.AddAttributesToRender(writer);
		
		////}

		//protected override void Render(System.Web.UI.HtmlTextWriter writer)
		//{
		//	var x = new RewriteFormHtmlTextWriter(writer);
		//	base.Render(x);
		//}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			this.ButtonControl.ToolTip = this.ToolTip;

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

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			Add508ComplianceAttributes();

		}

		/// <summary>
		/// Raises the <see cref="OnLoad"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			// There seems to be a bug with the combo box - if it's disabled when the page loads,
			// The box is greyed out and clicking the arrow next to the box does nothing.
			// However, if there's a postback, the box stays greyed out but clicking the arrow will display
			// the items in the list. This seemingly unnecessary line is here to make sure a disabled 
			// box stays truly disabled
			if (!this.Enabled)
			{
				this.Enabled = false;
			}

			// We want the first item selected when we start
			if (!this.Page.IsPostBack && this.SelectedItem == null && this.Items.Count > 0)
			{
				this.SelectedIndex = 0;
			}
		}

		#endregion
	}


	public class RewriteFormHtmlTextWriter : System.Web.UI.HtmlTextWriter
	{

		public RewriteFormHtmlTextWriter(System.Web.UI.HtmlTextWriter writer)
			: base(writer)
		{
			this.InnerWriter = writer.InnerWriter;
		}
		public RewriteFormHtmlTextWriter(System.IO.TextWriter writer)
			: base(writer)
		{
			base.InnerWriter = writer;
		}
		public override void AddAttribute(string name, string value)
		{
			if (name == "onchange"
				&& this.TagName == "div"
				&& this.IsValidFormAttribute(name))
			{

				name = "onclick";
				//value = "alert('a');" + value;
			} 
			base.AddAttribute(name, value);
		}
		public override void AddAttribute(string name, string value, bool fEndode)
		{
			if (name == "onchange"
				&& this.TagName == "div"
				&& this.IsValidFormAttribute(name))
			{

				name = "onclick";
				//value = "alert('b');" + value;
			}
			base.AddAttribute(name, value, fEndode);
		}
		protected override void AddAttribute(string name, string value, System.Web.UI.HtmlTextWriterAttribute key)
		{
			if (name == "onchange"
				&& this.TagName == "div"
				&& this.IsValidFormAttribute(name))
			{

				name = "onclick";
				//value = "alert('c');" + value;
				key = System.Web.UI.HtmlTextWriterAttribute.Onclick;
			}

			base.AddAttribute(name, value, key);
		}
		public override void AddAttribute(System.Web.UI.HtmlTextWriterAttribute key, string value)
		{
			if (key == System.Web.UI.HtmlTextWriterAttribute.Onchange)
			{

				key = System.Web.UI.HtmlTextWriterAttribute.Onclick;
				//value = "alert('d');" + value;
			} 
			base.AddAttribute(key, value);
		}
		public override void AddAttribute(System.Web.UI.HtmlTextWriterAttribute key, string value, bool fEncode)
		{
			if (key == System.Web.UI.HtmlTextWriterAttribute.Onchange
				&& this.TagName == "div")
			{

				key = System.Web.UI.HtmlTextWriterAttribute.Onclick;
			//	value = "alert('e');" + value;
			}
			base.AddAttribute(key, value, fEncode);
		}
		public override void RenderBeginTag(System.Web.UI.HtmlTextWriterTag tagKey)
		{
			if (tagKey == System.Web.UI.HtmlTextWriterTag.Ul)
			{
				;// this.AddAttribute("onclick", "alert('aa')");
			}
			else if (tagKey == System.Web.UI.HtmlTextWriterTag.Input)
			{
				//this.AddAttribute("title", "hello");
			}			
			base.RenderBeginTag(tagKey);

		}
	 
	}

}