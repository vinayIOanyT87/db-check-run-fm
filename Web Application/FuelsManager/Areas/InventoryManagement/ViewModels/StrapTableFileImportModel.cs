using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
    using FMBusinessObjects.DataObjects;
    using System.Web.Mvc;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable]
    public class StrapTableFileImportModel
    {
        public Guid SiteGuid;
        public string logText;
        public EngineeringUnit LevelUnit { get; set; }
        public int LevelDecimalPlaces { get; set; }
        public EngineeringUnit VolumeUnit { get; set; }
        public int VolumeDecimalPlaces { get; set; }
        public EngineeringUnit DensityUnit { get; set; }
        public int DensityDecimalPlaces { get; set; }
        public EngineeringUnit TemperatureUnit { get; set; }
        public int TemperatureDecimalPlaces { get; set; }
        public EngineeringUnit MassUnit { get; set; }
        public int MassDecimalPlaces { get; set; }
        public bool IsTemplatePoint { get; set; }
        public string PointId;

        public StrapTableFileImportModel(Guid id, string log)
        {
            this.SiteGuid = id;
            this.logText = log;
        }

        public StrapTableFileImportModel(Guid id)
        {
            this.SiteGuid = id;
        }

        public StrapTableFileImportModel(BasePoint basePoint, Guid siteGuid, string logText) : this(siteGuid, logText)
        {
            LevelUnit = basePoint.LevelUnit;
            LevelDecimalPlaces = 9;
            VolumeUnit = basePoint.VolumeUnit;
            VolumeDecimalPlaces = 9;
            DensityUnit = basePoint.DensityUnit;
            DensityDecimalPlaces = 9;
            TemperatureUnit = basePoint.TemperatureUnit;
            TemperatureDecimalPlaces = 9;
            MassUnit = basePoint.MassUnit;
            MassDecimalPlaces = 9;
        }
    }
}