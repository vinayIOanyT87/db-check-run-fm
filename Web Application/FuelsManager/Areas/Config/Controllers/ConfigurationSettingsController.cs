namespace FuelsManager.Areas.Config.Controllers
{
	using System;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.AssetTrackingArea.ViewModels;
	using FuelsManager.Areas.Config.Models;
	using FuelsManager.Areas.Config.ViewModels;
	using FuelsManager.Areas.Controllers;

	using Newtonsoft.Json;

    public class ConfigurationSettingsController : FMBaseController
    {

        /// <summary>
        /// This method will return the model as a string representation.
        /// </summary>
        /// <param name="model">The configuration setting model to be serialize.</param>
        /// <returns>The serialized string version of the model.</returns>
        [NonAction]
        public static string SerializeModel(ConfigurationSettingDOClass model)
        {
            return JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// This method will deserialize the model string into an object.
        /// </summary>
        /// <param name="modelStr">The string version of the model.</param>
        /// <returns>Returns the model as an object.</returns>
        [NonAction]
        public static ConfigurationSettingDOClass DeserializeModel(string modelStr)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var model = JsonConvert.DeserializeObject<ConfigurationSettingDOClass>(modelStr, jsonSerializerSettings);

            return model;
        }

        [HttpGet]
        public ActionResult ConfigurationSettingsIndex()
		{
			var context = this.Session[ConfigurationSettingsFilterContext.SessionKey] as ConfigurationSettingsFilterContext;
			var model = new ConfigurationSettingsModel(context);

			try
			{
				model.Settings = this.GetSettings(model.FindText);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ConfigurationSettingsIndex( ConfigurationSettingsModel model )
		{
			try
			{
				if (ModelState.IsValid)
				{
					var context = new ConfigurationSettingsFilterContext(model);
					this.Session[ConfigurationSettingsFilterContext.SessionKey] = context;

					model.Settings = this.GetSettings(model.FindText);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		[NonAction]
		private ConfigurationSettingDOCollectionClass GetSettings( string findText )
		{
			var settings =
				FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOCollectionClass>(
					x => x.Enumerate( this.Security ) );

			if ( string.IsNullOrEmpty( findText ) == false )
			{
				findText = findText.ToLower();

				for ( var index = settings.Count - 1; index >= 0; --index )
				{
					var setting = settings.Item( index );

					if ( setting.SettingValue.ToLower().Contains( findText ) == false 
						&& setting.SettingKey.ToLower().Contains( findText ) == false )
					{
						settings.RemoveAt( index );
					}
				}
			}

			return settings;
		}

		[HttpGet]
		public ActionResult ConfigurationSettingsDetail( string id )
		{
			try
			{
				var settingGuid = new Guid( id );

				var settings = this.GetSettings( null );

				foreach (ConfigurationSettingDOClass setting in settings)
				{
					if (setting.ConfigurationSettingGuid == settingGuid)
					{
						return this.View(setting);
					}
				}

				throw new Exception("Setting not found.");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.RedirectToAction("ConfigurationSettingsIndex");
		}

		[HttpPost]
		public ActionResult UpdateConfigurationSettingsDetail(string modelStr)
		{
         var resultData = new ConfigSettingsResultDataClass();

         try
			{
			    if (string.IsNullOrEmpty(modelStr) == false)
			    {
			        var model = DeserializeModel(modelStr);
			        var settingValue = model.SettingValue ?? string.Empty;
				    FMChannelHelper.MakeCall<IConfigurationSettings>(
                                        x => x.ModifyWithEncryption(this.Security, model.SettingKey, settingValue, model.KeyType));

			        resultData.ConfigSettingsDetailModel = model;
			    }
				 ClearCachedConfigurationSettings();
         }
			catch (Exception except)
			{
			    resultData.ErrorFlag = true;
			    resultData.ErrorMessage = "Error updating configuration settings: " + except.Message;
				return this.Json(resultData);
         }

         return this.Json(resultData);
      }
		private void ClearCachedConfigurationSettings()
		{
			AppDomain.CurrentDomain.SetData("IsFdsIM", null);

		}		
	}

    #region Result data class
    [Serializable]
    public class ConfigSettingsResultDataClass
    {
        public bool ErrorFlag;
        public string ErrorMessage;
        public ConfigurationSettingDOClass ConfigSettingsDetailModel;

        public ConfigSettingsResultDataClass()
        {
            this.ErrorFlag = false;
            this.ErrorMessage = string.Empty;
            this.ConfigSettingsDetailModel = new ConfigurationSettingDOClass();
        }
    }
    #endregion
}
