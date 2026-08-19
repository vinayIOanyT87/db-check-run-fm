namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	// ReSharper disable InconsistentNaming
	public class ExStarsUniversalConfig
	{
		public string DunsNumber { get; protected set; }
		public string InterchangeControlVersion { get; protected set; }
		public string GS03_ApplicationReceiversCode { get; protected set; }
		public string ISA12_InterchangeControlVersion { get; protected set; }
		public string GS08_FuncGrpHdrVerReleaseIndustryIdCode { get; protected set; }
		public string ISA05Qualifier { get; protected set; }
		public int EnableDebugFeatures { get; protected set; }

		private readonly ConfigurationSettingsClass fmBusServicesConfigSettings;

		protected SecurityClass Security;

		public ExStarsUniversalConfig(SecurityClass security)
		{
			this.Security = security;
			this.fmBusServicesConfigSettings = new ConfigurationSettingsClass();
			this.DunsNumber =						this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_DunsNumber_ISA08);
			this.InterchangeControlVersion =		this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_InterchangeControlVersion_ISA12);
			this.GS03_ApplicationReceiversCode =	this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_ApplicationReceiversCode_GS03);
			this.GS08_FuncGrpHdrVerReleaseIndustryIdCode = this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_FuncGrpHdrVerReleaseIndIdCode_GS08);
			this.ISA12_InterchangeControlVersion =	this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_InterchangeControlVersion_ISA12);
			this.ISA05Qualifier =					this.GetString( ConfigurationSettingDOClass.Key_IrsExStars_ISA05Qualifier);
			this.EnableDebugFeatures =				int.Parse( this.GetString(ConfigurationSettingDOClass.Key_IrsExStars_EnableDebugFeatures));
		}

		protected string GetString(string key)
		{
			ConfigurationSettingDOClass keyedValue = this.fmBusServicesConfigSettings.GetByKey(this.Security, key);
			if (keyedValue == null || string.IsNullOrEmpty(keyedValue.SettingValue))
			{
				throw new ExStarsBusinessException("Database table dbo.tblConfigurationSetting has no setting for {0}", key);
			}

			return keyedValue.SettingValue;
		}
	}
}