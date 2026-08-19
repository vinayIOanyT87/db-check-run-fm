namespace FuelsManager.Areas.AssetTrackingArea.Controllers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.AssetTrackingArea.ViewModels;
	using FuelsManager.Areas.Controllers;

	public class AssetIconSelectionController : FMBaseController
	{
        // GET: AssetTrackingArea/AssetIconConfiguration
        public ActionResult IconSelection(AssetIconSelectionModel postedModel)
        {
	        if (postedModel == null)
	        {
		        var newIconConfigurationModel = new AssetIconSelectionModel();
				this.UpdateModel(newIconConfigurationModel);

		        return this.View(newIconConfigurationModel);
	        }

			this.UpdateModel(postedModel);
	        return this.View(postedModel);
        }

		#region Private methods
		/// <summary>
		/// This method will update the icon configuration model.
		/// </summary>
		/// <param name="iconSelectionModel"></param>
		private void UpdateModel(AssetIconSelectionModel iconSelectionModel)
		{
			string iconPath;
			var iconModelList = new List<IconModel>();
			List<string> fileNameList = this.GetIcons(out iconPath, iconSelectionModel.FindText);

			if (fileNameList.Count > 0)
			{
				int iconKey = 0;

				foreach (string fileName in fileNameList)
				{
					var iconModel = new IconModel
					                {
						                IconKey = iconKey,
										IconFileName = fileName,
										IconImage = iconPath + "/" + fileName
					                };

					iconModelList.Add(iconModel);
					iconKey++;
				}
			}

			iconSelectionModel.IconModelList = iconModelList;
		}

		/// <summary>
		/// This method will get a list of icon file names with the "png"
		/// file extension.
		/// </summary>
		/// <returns>Returns a list of file names in alphabetical order.</returns>
		private List<string> GetIcons(out string iconPath, string findStr)
		{
			const string IconPathKey = "GeoTrackingMapIconPath";
			var configSettingDo		 = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, IconPathKey));
			iconPath				 = "~/Areas/images/AssetMapImages/MapIcons";

			if (configSettingDo != null && string.IsNullOrEmpty(configSettingDo.SettingValue) == false)
			{
				iconPath = configSettingDo.SettingValue;
			}

			string root = this.Server.MapPath(iconPath);
			string[] iconPathFileNames = Directory.GetFiles(root, "*.png");

			var iconFileList = new List<string>();

			if (iconPathFileNames.Length > 0)
			{
				foreach (string pathFileName in iconPathFileNames)
				{
					if (string.IsNullOrEmpty(findStr))
					{
						string fileName = this.GetFileNameOnly(pathFileName);
						iconFileList.Add(fileName);
					}
					else if (pathFileName.ToUpper().Contains(findStr.ToUpper()))
					{
						string fileName = this.GetFileNameOnly(pathFileName);
						iconFileList.Add(fileName);
					}
				}
			}

			// Sort alphabetically.
			iconFileList.Sort();

			return iconFileList;
		}

		/// <summary>
		/// This method will return the file name only.
		/// </summary>
		/// <param name="pathAndFileName">Path and File Name string.</param>
		/// <returns>Returns file name only.</returns>
		private string GetFileNameOnly(string pathAndFileName)
		{
			string[] split = pathAndFileName.Split('\\');

			if (split.Length == 0)
			{
				return string.Empty;
			}

			string fileName = split[split.Length - 1];
			return fileName;
		}
		#endregion
	}
}