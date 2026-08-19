
namespace FMControls
{
	using System;
	using System.Configuration;
	using System.Web.UI.WebControls;

	public class FMPageFadeImage : Panel
	{
		public FMPageFadeImage()
		{
			this.CssClass = "pageFadeImage";
			if (this.DesignMode == false)
			{
				this.Style["background-image"] = "url('" + ConfigurationManager.AppSettings["PageFadeImage"].Replace("\\", "/")
										   + "')";
			}
			else
			{
				this.Style["background-image"] = "url('../FMWebApp/images/Page_Fade_7.jpg')";
			}
		}
	}
}
