
using System;
using System.Web.UI;
using FMBusinessObjects.UtilityObjects;

[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	/// <summary>
	/// Summary description for FMRadionButton
	/// </summary>
	public class FMRadioButton : System.Web.UI.WebControls.RadioButton
	{
		protected void Page_Load ( object sender, System.EventArgs e )
		{
			if (!Page.IsPostBack)
			{
				try
				{
					if (Page.Session["UseDataDictionary"] == null || (bool) Page.Session["UseDataDictionary"])
					{
						if (Page.Session["SiteGuid"] == null)
						{
							return;
						}

						Guid SiteGuid = (Guid)Page.Session["SiteGuid"];

						

							if (Text.Length != 0)
							{
								if (Text[Text.Length - 1] == ':')
								{
									Text = Text.Remove(Text.Length - 1, 1);
									Text = this.GetDataDictionaryValueByKey(SiteGuid, Text) + ":";
								}
								else
								{
									Text = this.GetDataDictionaryValueByKey(SiteGuid, Text);
								}
							}

							if (ToolTip.Length != 0)
							{
								ToolTip = this.GetDataDictionaryValueByKey(SiteGuid, ToolTip);
							}
					}
				}
				catch
				{
				}
			}
		}

		protected string GetDataDictionaryValueByKey(Guid siteGuid, string p)
		{
			return DataDictionarySingleton.Get(siteGuid, p);
		}

		override protected void OnInit ( EventArgs e )
		{
			InitializeComponent ( );
			base.OnInit ( e );
		}

		private void InitializeComponent ( )
		{
			this.Load += new System.EventHandler ( this.Page_Load );

		}
	}
}
