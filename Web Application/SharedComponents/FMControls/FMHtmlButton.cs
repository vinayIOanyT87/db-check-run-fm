using System;
using System.Web.UI;
using FMBusinessObjects.UtilityObjects;

[assembly: TagPrefix("FMControls", "FMControls")]
namespace FMControls
{
	public class FMHtmlButton : System.Web.UI.HtmlControls.HtmlButton
	{

		private string rawText = string.Empty;

		public string Text
		{
			set
			{
				rawText = value;
			}
		}

		// Override the OnLoad method to convert the raw text to
		// html for underlining the access key
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.InnerHtml = ParseAccessKey(rawText);
			this.Attributes.Add("type","button");
		}

		private string ParseAccessKey(string value)
		{
			value = GetTranslationText(value);

			int ampersandIndex = value.IndexOf('&');
			string newValue = value;

			if (ampersandIndex >= 0 && ampersandIndex < value.Length - 1)
			{
				newValue = value.Substring(0, ampersandIndex);

				string accessKey = value.Substring(ampersandIndex + 1, 1);

				this.SetAttribute("AccessKey", accessKey);

				newValue += "<u>" + accessKey + "</u>";

				if (ampersandIndex + 2 < value.Length)
				{
					newValue += value.Substring(ampersandIndex + 2);
				}
			}

			return newValue;
		}

		private string GetTranslationText(string key)
		{
			string value = key;

			try
			{
				if (Page != null && Page.Session != null)
				{
					if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
					{
						if (Page.Session["SiteGuid"] != null)
						{
							if (string.IsNullOrEmpty(key) == false)
							{
								Guid siteGuid = (Guid)Page.Session["SiteGuid"];
								if ((key.StartsWith("&") == true) && (key.Length > 1))
								{
									string realKey = key.Substring(1);

									return "&" + DataDictionarySingleton.Get(siteGuid, realKey);
								}

								return DataDictionarySingleton.Get(siteGuid, key);
							}						
						}
					}
				}
			}
			catch
			{
			}

			return value;
		}

		public bool Enabled
		{
			get { return !Disabled; }
			set { Disabled = !value; }
		}

	}
}
