namespace FuelsManager.Areas.Config.Models
{
	using System.ComponentModel.DataAnnotations;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Config.ViewModels;

	public class ConfigurationSettingsModel
	{
		[StringLength(50)]
		public string FindText { get; set; }

		public bool DeleteEnabled { get; set; }

		public ConfigurationSettingDOCollectionClass Settings { get; set; }

		public ConfigurationSettingsModel()
		{
			this.Reset();
		}

		public ConfigurationSettingsModel(ConfigurationSettingsFilterContext context)
		{
			this.Reset();

			if (context != null)
			{
				this.FindText = context.FindText;
			}
		}

		private void Reset()
		{
			this.DeleteEnabled = false;
		}
	}
}
