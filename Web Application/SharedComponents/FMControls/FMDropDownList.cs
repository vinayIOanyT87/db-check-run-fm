// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDropDownList.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMDropDownList.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Drop down list tailored for FuelsManager
	/// </summary>
	public class FMDropDownList : DropDownList
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMDropDownList"/> class.
		/// </summary>
		public FMDropDownList()
		{
			this.Sort = true;
			this.Translate = true;
		}

		#region Constants and Fields

		/// <summary>
		/// Gets or sets a value indicating whether to Sort.
		/// </summary>
		public bool Sort { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the control should automatically translate
		/// the items in the dropdown.
		/// </summary>
		public bool Translate { get; set; }

		#endregion

		#region Public Methods and Operators
		/// <summary>
		/// Selects an item by looking for the specified text.
		/// </summary>
		/// <param name="text">
		/// The text.
		/// </param>
		/// <returns>
		/// True, if the text is found; otherwise, false.
		/// </returns>
		public bool SelectByText(string text)
		{
			ListItem li = this.Items.FindByText(text);

			if (li != null && li.Value != null)
			{
				this.SelectedValue = li.Value;
				return true;
			}

			return false;
		}
		#endregion

		#region Methods

		/// <summary>
		/// The initialization override for the component.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Page load event for the component.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected void PageLoad(object sender, EventArgs e)
		{
			if (this.DesignMode == false && !this.Page.IsPostBack && this.Translate)
			{
				if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
				{
					if (this.Page.Session["SiteGuid"] == null)
					{
						return;
					}

					var siteGuid = (Guid)this.Page.Session["SiteGuid"];


					foreach (ListItem item in this.Items)
					{
						item.Text = this.GetDataDictionaryValueByKey(siteGuid, item.Text);
					}

					if (this.ToolTip.Length != 0)
					{
						this.ToolTip = GetDataDictionaryValueByKey(siteGuid, this.ToolTip);
					}

				}
				else
				{
					// Remove translation group identifier
					foreach (ListItem item in this.Items)
					{
						item.Text = item.Text.Substring(item.Text.IndexOf("|", StringComparison.Ordinal) + 1);
					}

					if (this.ToolTip.Length != 0)
					{
						this.ToolTip = this.ToolTip.Substring(this.ToolTip.IndexOf("|", StringComparison.Ordinal) + 1);
					}
				}
			}
		}

		protected string GetDataDictionaryValueByKey(Guid siteGuid, string p)
		{
            return DataDictionarySingleton.Get(siteGuid, p);
        }


		/// <summary>
		/// Renders the control on the specified writer.
		/// </summary>
		/// <param name="output">
		/// The output writer to use.
		/// </param>
		protected override void RenderContents(HtmlTextWriter output)
		{
			var outputItems = new ListItemCollection();
			ListItem selectedItem = null;

			int selectedIndex = 0;

			var specialItems = new List<ListItem>();

			foreach (ListItem item in this.Items)
			{
				if (selectedIndex == this.SelectedIndex)
				{
					selectedItem = item;
				}

				if (!this.Sort)
				{
					outputItems.Add(item);
				}
				else
				{
					bool inserted = false;

					// Keep bracketed items separate until later so they can be at the beginning of the list
					if (string.IsNullOrEmpty(item.Text) == false && item.Text[0] == '{')
					{
						specialItems.Add(item);
						inserted = true;
					}

					if (!inserted)
					{
						foreach (ListItem existingItem in outputItems)
						{
							if (string.CompareOrdinal(existingItem.Text, item.Text) > 0)
							{
								int insertIndex = outputItems.IndexOf(existingItem);
								outputItems.Insert(insertIndex, item);
								inserted = true;
								break;
							}
						}
					}

					if (!inserted)
					{
						outputItems.Add(item);
					}
				}

				selectedIndex++;
			}

			// Put all the special items at the beginning of the list.  Expect them to be in the correct sort order
			int insertLocation = 0;
			foreach (var item in specialItems)
			{
				outputItems.Insert(insertLocation++, item);
			}

			selectedIndex = 0;
			foreach (ListItem item in outputItems)
			{
				output.WriteBeginTag("option");

				if (item != null && item.Equals(selectedItem))
				{
					output.WriteAttribute("selected", "selected");
				}

				Debug.Assert(item != null, "item != null");
				output.WriteAttribute("Value", item.Value);
				item.Attributes.Render(output);
				output.Write(HtmlTextWriter.TagRightChar);
				output.Write(HttpUtility.HtmlEncode(item.Text));
				output.WriteEndTag("option");
				output.WriteLine();
				selectedIndex++;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
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

		/// <summary>
		/// Initialization routine for the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += this.PageLoad;
		}

		#endregion
	}
}