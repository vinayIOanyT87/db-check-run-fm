// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMListBox.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMListBox
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// List box tailored for FuelsManager.
	/// </summary>
	public class FMListBox : ListBox
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMListBox"/> class.
		/// </summary>
		public FMListBox()
		{
			this.UseDataDictionary = true;
			this.Sort = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the selection count.
		/// </summary>
		public int SelectionCount
		{
			get
			{
				int count = 0;

				foreach (ListItem item in this.Items)
				{
					if (item.Selected)
					{
						count++;
					}
				}

				return count;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to sort.
		/// </summary>
		public bool Sort { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether UseDataDictionary.
		/// </summary>
		public bool UseDataDictionary { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Swaps two integers with some safety tests.
		/// </summary>
		/// <param name="index1">
		/// The index 1.
		/// </param>
		/// <param name="index2">
		/// The index 2.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Thrown if either parameter is invalid.
		/// </exception>
		public void Swap(int index1, int index2)
		{
			if (index1 >= this.Items.Count || index1 < 0)
			{
				throw new ArgumentException("Invalid value for Index1");
			}

			if (index2 >= this.Items.Count || index2 < 0)
			{
				throw new ArgumentException("Invalid value for Index2");
			}

			if (index1 != index2)
			{
				ListItem temp = this.Items[index1];
				this.Items.RemoveAt(index1);
				this.Items.Insert(index1, this.Items[index2]);

				this.Items.RemoveAt(index2);
				this.Items.Insert(index2, temp);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Initialization routine override for the component.
		/// </summary>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected override void OnInit( EventArgs e )
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
		protected void PageLoad( object sender, EventArgs e )
		{
			if (this.DesignMode == false && !this.Page.IsPostBack)
			{
				if (this.UseDataDictionary
				    && (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"]))
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
								this.ToolTip = this.GetDataDictionaryValueByKey(siteGuid, this.ToolTip);
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
		/// Renders the contents.
		/// </summary>
		/// <param name="output">The output writer to use.</param>
		protected override void RenderContents(HtmlTextWriter output)
		{
			var outputItems = new ListItemCollection();

			int index = 0;
			foreach (ListItem item in this.Items)
			{
				if (index == this.SelectedIndex)
				{
				}

				if (!this.Sort)
				{
					outputItems.Add(item);
				}
				else
				{
					bool inserted = false;
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

					if (!inserted)
					{
						outputItems.Add(item);
					}
				}

				index++;
			}

			index = 0;
			foreach (ListItem item in outputItems)
			{
				output.WriteBeginTag("option");

				if (item.Selected)
				{
					output.WriteAttribute("selected", "selected");
				}

				if (item.Enabled == false)
				{
					output.WriteAttribute("enabled", "false");
					output.WriteAttribute("disabled", "disabled");
				}

				output.WriteAttribute("Value", item.Value);
				item.Attributes.Render(output);
				output.Write(HtmlTextWriter.TagRightChar);
				output.Write(HttpUtility.HtmlEncode(item.Text));
				output.WriteEndTag("option");
				output.WriteLine();
				index++;
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