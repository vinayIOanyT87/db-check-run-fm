// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMCheckBox.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for FMCheckBox.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMControls
{
	using System;
	using System.Web.UI.WebControls;
	using System.Web.UI;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Checkbox control tailored for FuelsManager
	/// </summary>
	public class FMCheckBox : CheckBox
	{
		#region Methods

		/// <summary>
		/// Page load event for the component.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The event args.
		/// </param>
		protected void FMCheckBoxLoad(object sender, EventArgs e)
		{
			if (this.DesignMode == false && !this.Page.IsPostBack)
			{
				if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
				{
					if (this.Page.Session["SiteGuid"] == null)
					{
						return;
					}

					var siteGuid = (Guid)this.Page.Session["SiteGuid"];

					if (this.Text.Length != 0)
					{
						if (this.Text[this.Text.Length - 1] == ':')
						{
							this.Text = this.Text.Remove(this.Text.Length - 1, 1);
							this.Text = this.GetDataDictionaryValueByKey(siteGuid, this.Text) + ":";
						}
						else
						{
							this.Text = GetDataDictionaryValueByKey(siteGuid, this.Text);
						}
					}

					if (this.ToolTip.Length != 0)
					{
						this.ToolTip = GetDataDictionaryValueByKey(siteGuid, this.ToolTip);
					}
				}
				else
				{
					// Remove translation group identifier
					this.Text = this.Text.Substring(this.Text.IndexOf("|", StringComparison.Ordinal) + 1);

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
		/// Initialization routine for the component.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += this.FMCheckBoxLoad;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (String.IsNullOrEmpty(this.ToolTip))
			{
				/**/
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
/**/
				if (this.ToolTip == string.Empty)
				{
					if (this.Text != string.Empty)
					{
						this.ToolTip = this.Text;
					}
					else //finally, if none of these are defined then look for an associated label. If label not found use static text "checkbox" 
					{
						Label l = FindAssociatedLabel(this.Parent) ;
						if (l == null)
							this.ToolTip = "Checkbox";
						else
						{
							string txt = l.Text.Trim();
							if (txt.EndsWith(":"))
							{
								txt = txt.Substring(0, txt.Length - 1);
							}
							this.ToolTip = txt;
						}
					}
				}

			}
			this.InputAttributes["alt"] = this.ToolTip;
			this.InputAttributes["aria-checked"] = this.Checked.ToString().ToLower();
			this.InputAttributes["role"] = "checkbox";

		}

		/// <summary>
		///	Looks for a label that is associated with this checkbox. 
		///
		/// </summary>
		/// <param name="p">parent control</param>
		/// <returns></returns>
		protected Label FindAssociatedLabel(Control p) 
		{
			if (p == null)
				return null;

			foreach (Control c in p.Controls)
			{
				if (c is Label)
				{
					Label l = c as Label;
					if (l.AssociatedControlID == this.ID)
					{
						return l;
					}
				}
			}
			
			return FindAssociatedLabel(p.Parent);
		}

		#endregion
	}
}