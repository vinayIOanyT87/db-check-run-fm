
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using FMBusinessObjects.UtilityObjects;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// Summary description for FMPageSizeDropDown.
	/// </summary>
	public class FMPageSizeDropDown : System.Web.UI.WebControls.DropDownList
	{
		protected bool bUseDataDictionary = true;
		protected Guid SiteGuid = Guid.Empty;

		public string StringPrefix = "";

		public FMPageSizeDropDown()
		{
			this.CssClass = "formfield";
			this.AutoPostBack = true;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (DesignMode == false && Page.IsPostBack == false)
			{
				try
				{
					SiteGuid = (Guid)Page.Session["SiteGuid"];
				}
				catch { }
			}
		}

		public void SetPageSize(DataGrid Grid, int max)
		{
			if (Grid != null)
			{
				int newPageSize = Grid.PageSize;

				try
				{
					newPageSize = Convert.ToInt32(this.SelectedValue);
				}
				catch { }

				// Only make changes if the page size is different than the current
				// setting in the grid.
				if (newPageSize != Grid.PageSize)
				{

					if (newPageSize <= 0)
					{
						// Needs to be one more than the max so inline additions will work
						newPageSize = max + 1;
					}

					Grid.PageSize = newPageSize;


					Page.Session[this.ID + "PageSize"] = this.SelectedValue;

				}
				// Check the page number to make sure it is still valid
				if (max == 0)
				{
					Grid.CurrentPageIndex = 0;
				}
				else
				{
					int maxPage = (max - 1) / newPageSize;

					if (maxPage < Grid.CurrentPageIndex)
					{
						Grid.CurrentPageIndex = maxPage;
					}
				}

			}

		}

		public void SetPageSize(GridView Grid, int max)
		{
			if (Grid != null)
			{
				int newPageSize = Grid.PageSize;

				try
				{
					newPageSize = Convert.ToInt32(this.SelectedValue);
				}
				catch { }

				// Only make changes if the page size is different than the current
				// setting in the grid.
				if (newPageSize != Grid.PageSize)
				{

					if (newPageSize <= 0)
					{
						// Needs to be one more than the max so inline additions will work
						newPageSize = max + 1;
					}

					Grid.PageSize = newPageSize;


					Page.Session[this.ID + "PageSize"] = this.SelectedValue;

				}
				// Check the page number to make sure it is still valid
				if (max == 0)
				{
					Grid.PageIndex = 0;
				}
				else
				{
					int maxPage = (max - 1) / newPageSize;

					if (maxPage < Grid.PageIndex)
					{
						Grid.PageIndex = maxPage;
					}
				}

			}

		}

		private void AddItem(string inText, int value)
		{
			ListItem item = CreateItem(inText, value);
			Items.Add(item);
		}

		private ListItem CreateItem(string inText, int value)
		{
			var item = new ListItem(GetTranslatedText(inText), value.ToString());
			return item;
		}

		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);

			if (DesignMode == false)
			{
				if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
				{
					bUseDataDictionary = true;

					try
					{
						SiteGuid = (Guid)Page.Session["SiteGuid"];
					}
					catch { }
				}
				else
				{
					bUseDataDictionary = false;
				}
			}
			if (string.IsNullOrEmpty(this.Attributes["alt"]))
			{
				this.Attributes["alt"] = "Page Size";
			}

			AddSelectionItems();
			SetDefaultSelection();
		}


		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(Page_Load);
		}

		public void SetSelectionValue(string value)
		{
			switch (value)
			{
				case "10":
					this.SelectedIndex = 0;
					break;

				case "25":
					this.SelectedIndex = 1;
					break;

				case "50":
					this.SelectedIndex = 2;
					break;

                case "100":
                    this.SelectedIndex = 3;
                    break;

                case "1500":
                    this.SelectedIndex = 4;
                    break;

                default:
					this.SelectedIndex = 5;
					break;
			}
		}

		private void AddSelectionItems()
		{
			if (StringPrefix != "")
			{
				StringPrefix += "|";
			}

			AddItem(StringPrefix + "Show 10", 10);
			AddItem(StringPrefix + "Show 25", 25);
			AddItem(StringPrefix + "Show 50", 50);
            AddItem(StringPrefix + "Show 100", 100);
            AddItem(StringPrefix + "Show 1500", 1500);
            AddItem(StringPrefix + "Show All", 0);
		}

		public void SetLimit(int limit)
		{
			return;
			//Items.RemoveAt(4);

			//ListItem newItem = CreateItem(string.Format("Show {0}", limit), limit);

			//for (int index = 0; index < Items.Count; ++index)
			//{
			//	ListItem item = Items[index];
			//	int value = Convert.ToInt32(item.Value);
			//	if (value > limit)
			//	{
			//		Items.Insert(index, newItem);
			//		SetDefaultSelection();
			//		return;
			//	}
			//}

			//Items.Add(newItem);
			//SetDefaultSelection();
		}

		private void SetDefaultSelection()
		{
			int defaultSelection = 10;

			if (DesignMode == false)
			{
				if (Page != null
					&& Page.Session != null
					&& this.ID != null
					&& Page.Session[this.ID + "PageSize"] != null)
				{
					defaultSelection = Convert.ToInt32(Page.Session[this.ID + "PageSize"]);
				}
			}

			this.SelectedValue = defaultSelection.ToString();
		}

		public string GetTranslatedText(string originalText)
		{
			string translatedText = originalText;

			if (DesignMode == false && bUseDataDictionary)
			{
				translatedText = DataDictionarySingleton.Get(SiteGuid, originalText);
			}

			return translatedText;
		}
	}
}
