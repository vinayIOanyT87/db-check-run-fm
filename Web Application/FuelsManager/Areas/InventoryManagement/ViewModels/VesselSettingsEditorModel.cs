namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.DataObjects.CodedVariables;
	using Areas.Controllers;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class VesselSettingsEditorModel
	{
		public string PointId { get; set; }

		public Guid PointGuid { get; set; }

		public EngineeringUnit LevelUnit { get; set; }

		public EngineeringUnit VolumeUnit { get; set; }

		public EngineeringUnit TemperatureUnit { get; set; }

		public string PointPropertyId { get; set; }

		public Guid PointPropertyGuid { get; set; }

		public Vessel VesselSettings { get; set; }

		public string TankInstallationDate { get; set; }

		public bool IsTemplatePoint { get; set; }

		public string CSTManufactureDate { get; set; }

		public string CSTCommissionDate { get; set; }

		public VesselSettingsEditorModel()
		{
			this.VesselSettings = new Vessel();
		}


		public VesselSettingsEditorModel(bool isTemplatePoint, BasePoint basePoint, string pointPropertyId, Guid pointPropertyGuid, Vessel vesselSettings)
		{
			this.IsTemplatePoint = isTemplatePoint;
			this.PointId = basePoint.ID;
			this.PointGuid = basePoint.IdentityGuid;
			this.LevelUnit = basePoint.LevelUnit;
			this.VolumeUnit = basePoint.VolumeUnit;
			this.TemperatureUnit = basePoint.TemperatureUnit;
			this.PointPropertyId = pointPropertyId;
			this.PointPropertyGuid = pointPropertyGuid;
			this.VesselSettings = vesselSettings;
			this.TankInstallationDate = vesselSettings.TankInstallationDateString;
			this.CSTManufactureDate = vesselSettings.CSTManufactureDateString;
			this.CSTCommissionDate = vesselSettings.CSTCommissionDateString;
		}
	}
}