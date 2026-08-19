using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Interfaces;

using FMPointCommon;

namespace LeakDetection
{
	public class FMLeakDetection : FuelsManagerModule, IFuelsManagerModule
	{
		public LeakDetectionSettings LeakDetectionSettings { get; set; }

		public void LeakDetectionCalculation(PointTag VolumeNetStandardUnrounded,
													PointTag VolumeCorrectionFactorUnrounded,
													PointTag PressureBottom,
													PointTag LeakRate,
													PointTag LeakRateHighAlarm,
													PointTag LeakRateHighLimit,
													PointTag LeakDetectionDiscreteAlarm,
													PointTag LeakDetectionDataInsufficientLimit,
													PointTag LeakDetectionAlarm,
													PointTag LeakDetectionDataLastRunTime,
													PointTag VolumeNetStandard,
													PointTag LevelProduct,
													PointTag LevelWater,
													PointTag TemperatureProduct
													)
		{
			_ = VolumeNetStandardUnrounded;
			_ = VolumeCorrectionFactorUnrounded;
			_ = PressureBottom;
			_ = LeakRate;
			_ = LeakRateHighAlarm;
			_ = LeakRateHighLimit;
			_ = LeakDetectionDiscreteAlarm;
			_ = LeakDetectionDataInsufficientLimit;
			_ = LeakDetectionAlarm;
			_ = LeakDetectionDataLastRunTime;
			_ = VolumeNetStandard;
			_ = LevelProduct;
			_ = LevelWater;
			_ = TemperatureProduct;
		}

		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection { };
			return properties;
		}
	}
}
