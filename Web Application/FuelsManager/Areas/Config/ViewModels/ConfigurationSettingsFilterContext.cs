namespace FuelsManager.Areas.Config.ViewModels
{
	using System;

	using FuelsManager.Areas.Config.Models;

	[Serializable]
	public class ConfigurationSettingsFilterContext
	{
		public const string SessionKey = "ConfigurationSettingsFilterContext";

		public string FindText { get; set; }

		public ConfigurationSettingsFilterContext()
		{
			this.FindText = string.Empty;
		}

		public ConfigurationSettingsFilterContext(ConfigurationSettingsModel model)
		{
			if (model != null)
			{
				this.FindText = model.FindText;
			}
		}
	}
}
