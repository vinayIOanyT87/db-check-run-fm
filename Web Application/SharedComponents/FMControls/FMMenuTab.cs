
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.UtilityObjects;

namespace FMControls
{
	/// <summary>
	/// FMMenuTab is a subclass of asp:Menu and used as a replacement for TabStrip.  Includes
	/// DataDictionary support.
	/// </summary>
	public class FMMenuTab : Menu
	{
		/// <summary>
		/// Index of the desired default menu item selection on initial load of the control
		/// </summary>
		public int SelectionDefault { get; set; }

		/// <summary>
		/// Used to associate a MultiViewControl with this FMMenuTab control instance
		/// </summary>
		public string MultiViewID { get; set; }

		public FMMenuTab ( )
		{
			SelectionDefault = 0;
			Orientation = Orientation.Horizontal;
			ItemWrap = true;
			StaticEnableDefaultPopOutImage = false;
			StaticMenuItemStyle.CssClass = "FMStaticMenuItemStyle";
			StaticHoverStyle.CssClass = "FMStaticHoverStyle";
			StaticSelectedStyle.CssClass = "FMStaticSelectedStyle";
		}
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			writer = new FMMenuItemHtmlTextWriter(writer);
			base.Render(writer);
		}

		protected override void OnInit ( EventArgs e )
		{
			base.OnInit ( e );
			InitializeComponents ( );
		}

		protected void InitializeComponents ( )
		{
			Load += new EventHandler ( FMMenu_Load );
			MenuItemClick += new MenuEventHandler ( FMMenuTab_MenuItemClick );
		}

		void FMMenuTab_MenuItemClick ( object sender, MenuEventArgs e )
		{
			MultiView multiViewControl = (MultiView) Page.FindControl ( MultiViewID );

			if (multiViewControl != null)
			{
				try
				{
					int viewIndex = Convert.ToInt32 ( e.Item.Value );
					multiViewControl.ActiveViewIndex = viewIndex;
				}
				catch (FormatException)
				{
					// Ignore format except.  It means the values were not set for the menu items with
					// view indexes.  Let everything else bubble up.
				}
			}

		}

		protected void FMMenu_Load ( object sender, EventArgs e )
		{
			if (Page.IsPostBack == false)
			{
				if (Items.Count > SelectionDefault)
				{
					Items[SelectionDefault].Selected = true;
				}

				ApplyDataDictionary ( );
			}
		}

		private string GetDataDictionaryValueByKey(Guid siteGuid, string p)
		{
			return DataDictionarySingleton.Get(siteGuid, p);
		}

		protected void ApplyDataDictionary ( )
		{
			try
			{
				if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
				{
					if (Page.Session["SiteGuid"] == null)
					{
						return;
					}

					Guid SiteGuid = (Guid)Page.Session["SiteGuid"];

					foreach (MenuItem item in Items)
					{
						item.Text = this.GetDataDictionaryValueByKey(SiteGuid, item.Text);

						if (item.ToolTip.Length != 0)
						{
							item.ToolTip = this.GetDataDictionaryValueByKey(SiteGuid, item.ToolTip);
						}
					}

				}
				else
				{
					foreach (MenuItem item in Items)
					{
						item.Text = item.Text.Substring(item.Text.IndexOf("|") + 1);

						if (item.ToolTip.Length != 0)
						{
							item.ToolTip = item.ToolTip.Substring(item.ToolTip.IndexOf("|" + 1));
						}
					}
				}
			}
			catch
			{
			}
		}


		private class FMMenuItemHtmlTextWriter : System.Web.UI.HtmlTextWriter
		{
			public FMMenuItemHtmlTextWriter(System.Web.UI.HtmlTextWriter writer) : base(writer)
			{
				;
			}
			protected override bool OnTagRender(string name, System.Web.UI.HtmlTextWriterTag key)
			{

				if (key == System.Web.UI.HtmlTextWriterTag.Table)
				{
					this.AddAttribute("role", "presentation");
					this.AddAttribute("aria-label", "Menu layout");

				}
				return base.OnTagRender(name, key);
			}
		}
	}
}
