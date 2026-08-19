namespace FMPointService.PointExecution
{

	internal class StandardTankScript
	{

		public const string StandardTank = @"
			namespace FMPointService.PointExecution
			{
				using System;
				using System.Collections.Generic;
				using System.Linq;
				using Microsoft.ClearScript.V8;
				using FMBusinessObjects.DataObjects;
				using VCF;
				using Quantities;
				using StrapTables;
				using ShellCorrection;
				using FloatingRoofCorrection;
				using RateModules;
				using TankCommands;
				using TankTransfer;
				using AvailableAndRemainingVolume;
				using CustomModule;
				using StandardTankCalculator;

				public class StandardTank : PointTemplateLogic
				{
					#region Private data members
					// Tags
					private PointTag LevelProduct;
					private PointTag LevelWater;
					private PointTag LevelSolids;
					private PointTag VolumeStrapProduct;
					private PointTag VolumeStrapWater;
					private PointTag TemperatureProduct;
					private PointTag TemperatureDensity;
					private PointTag TemperatureVapor;
					private PointTag DensityObserved;
					private PointTag DensityStandard;
					private PointTag PressureVapor;
					private PointTag VolumeCorrectionforTemperature;
					private PointTag VolumeCorrectionforPressure;
					private PointTag VolumeCorrectionforTemperatureandPressure;
					private PointTag VolumeCorrectionFactor;
					private PointTag VolumeCorrectionFactorUnrounded;
					private PointTag APICorrectionError;
					private PointTag GaugeCommand;
					private PointTag VolumeGrossObserved;
					private PointTag VolumeRoofCorrection;
					private PointTag VolumeBottom;
					private PointTag PercentBSW;
					private PointTag TankShellCorrection;
					private PointTag VolumeNetStandard;
					private PointTag VolumeNetStandardUnrounded;
					private PointTag Mass;
					private PointTag VolumeStrapSolids;
					private PointTag DensityObservedinAir;
					private PointTag DensityStandardinAir;
					private PointTag WeightGrossStandard;
					private PointTag WeightNetStandard;
					private PointTag VolumeGrossStandard;
					private PointTag VolumeWater;
					private PointTag VolumeBSW;
					private PointTag VolumeTotalCalculated;
					private PointTag TemperatureAmbient;
					private PointTag RoofCriticalZone;
					private PointTag LevelLowLimit;
					private PointTag LevelLoLoLimit;
					private PointTag LevelProductMinOpLimit;
					private PointTag LevelHighLimit;
					private PointTag LevelHiHiLimit;
					private PointTag LevelProductMaxOpLimit;
					private PointTag VolumeTotalObservedRate;
					private PointTag VolumeNetStandardRate;
					private PointTag LevelProductRate;
					private PointTag VolumeGrossObservedRate;
					private PointTag TankCommand;
					private PointTag TankStatus;
					private PointTag LevelProductStop;
					private PointTag LevelProductMovement;
					private PointTag TankModeDiscreteAlarm;
					private PointTag TransferMode;
					private PointTag TransferStatus;
					private PointTag TransferTarget;
					private PointTag TransferStartLevelProduct;
					private PointTag TransferStartGOV;
					private PointTag TransferStartNSV;
					private PointTag TransferStartVolumeWater;
					private PointTag TransferTimeRemaining;
					private PointTag TransferTimeCompletion;
					private PointTag TransferredGOV;
					private PointTag TransferredNSV;
					private PointTag TransferredVolumeWater;
					private PointTag TankTransferDiscreteAlarm;
					private PointTag VolumeGOVAvailable;
					private PointTag VolumeGOVRemaining;
					private PointTag VolumeNSVAvailable;
					private PointTag VolumeNSVRemaining;
					private PointTag TransferStartVolume;
					private PointTag TransferredVolume;
					private PointTag TransferStartTime;
					private PointTag TransferStopTime;
					private PointTag TransferLevelTarget;
					private PointTag TransferVolumeTarget;

					// added for vapor calculations
					private PointTag DensityVapor;
					private PointTag VolumeVaporNet;
					private PointTag MassVapor;

					// added for gauge density calculations
					private PointTag DensityProductGauge;

					// Properties
					private PointProperty StrapTableProperty;
					private PointProperty VcfSettingsProperty;
					private PointProperty VesselProperty;
					private PointProperty QuantitySettingsProperty;
					private PointProperty LevelRateSettingsProperty;
					private PointProperty TotalObservedRateSettingsProperty;
					private PointProperty NetStandardRateSettingsProperty;
					private PointProperty GrossObservedRateSettingsProperty;
					private PointProperty TankCommandSettingsProperty;
					private PointProperty TransferSettingsProperty;

					// Modules
					private FMVcf Vcf;
					private FMQuantities Quantities;
					private FMStrapTable StrapTable;
					private FMShellCorrection ShellCorrection;
					private FMFloatingRoofCorrection RoofCorrection;
					private FMRateModule RateModuleLevel;
					private FMRateModule RateModuleTotalObserved;
					private FMRateModule RateModuleNetStandard;
					private FMRateModule RateModuleGrossObserved;
					private FMTankCommands TankCommands;
					private FMTankTransfer TankTransfer;
					private FMAvailableAndRemainingVolume AvailableAndRemainingVolume;
					private FMStandardTankCalculator StandardTankCalculator;

					// Tag PointTemplateGuids
					private const string LevelTemplateTagGuid								= ""9EAB1A9F-2AA2-4EC9-AC60-7231345A974A"";
					private const string WaterLevelTemplateTagGuid						= ""257B0B99-B1F0-4FD2-BC76-348AEE522A90"";
					private const string SolidsLevelTemplateTagGuid						= ""1C9EA4B3-5460-450d-8971-D97CD0E43280"";
					private const string StrapVolumeTemplateTagGuid						= ""DCB0D63D-5F0A-4AB9-B454-7F124063ED47"";
					private const string WaterStrapVolumeTemplateTagGuid				= ""D5C390E3-EE24-41D8-96C0-43E5DFFF8CC5"";
					private const string TemperatureProductTemplateTagGuid			= ""8624008F-D28C-496d-8578-7227E329E493"";
					private const string TemperatureDensityTemplateTagGuid			= ""D350FEAB-229A-4808-A7A1-76E552501B47"";
					private const string TemperatureVaporTemplateTagGuid				= ""C58C9B1C-6471-474F-A2B4-6D9D2A5B5E7B"";
					private const string DensityProductObservedTemplateTagGuid		= ""5C6F08CC-1CEF-4AF6-B25C-CD12F3C82FB7"";
					private const string DensityProductStandardTemplateTagGuid		= ""8F82ABFB-8ED8-4A4B-9424-672C8E74752A"";
					private const string PressureVaporTemplateTagGuid					= ""A3D5835E-5F79-4110-8BA9-B868949E6EB9"";
					private const string VolCorForTempTemplateTagGuid					= ""9127882D-A34D-4465-A338-DB9BC7CF5D02"";
					private const string VolCorForPressTemplateTagGuid					= ""46C7073B-546C-4D31-B1BF-642DF6CC74AC"";
					private const string VolCorForPressTempTemplateTagGuid			= ""58ADEE26-8DEC-47D8-BB44-870DC5D8CDDF"";
					private const string VolumeCorrectionFactorTemplateTagGuid		= ""72269896-29D3-4082-856D-812E8BD90319"";
					private const string VolumeCorrectionFactorUnroundedTemplateTagGuid = ""D83854E0-41F4-474A-BDF4-21D2172065A4"";
					private const string APICorrectionErrorTemplateTagGuid			= ""70EC0770-89B6-4EF1-847D-C97EC459E988"";
					private const string GaugeCommandTagGuid								= ""9CC55B3E-F7FA-46DF-98B0-A5085B45821B"";
					private const string GrossObservedVolumeTemplateTagGuid			= ""2A80E1CE-933F-44bd-B0ED-C3C2861CE89D"";
					private const string RoofCorrectionTemplateTagGuid					= ""9173AD63-65C1-4c43-BB5A-054FFEABF1FE"";
					private const string BottomVolumeTemplateTagGuid					= ""C48D535A-2082-4c7b-AC91-93D17D89ADD8"";
					private const string PercentBSWTemplateTagGuid						= ""D723F705-01F6-4c46-92FF-BF46AB9A4C62"";
					private const string TankShellCorrectionTemplateTagGuid			= ""526A0F15-EF35-45bc-8F3D-CD2DF565A649"";
					private const string NetStandardVolumeTemplateTagGuid				= ""9A467CD0-6F77-4541-BEF3-C2C7E8879F05"";
					private const string NetStandardVolumeUnroundedTemplateTagGuid	= ""34CF4D6F-B832-4C8D-82F4-9AE591CA2740"";
					private const string MassTemplateTagGuid								= ""D4BD1CE3-D5E4-45b0-98A0-F8E5240E2A64"";
					private const string SolidsVolumeTemplateTagGuid					= ""78100E99-0E39-4170-BB01-19F752F0D929"";
					private const string DensityInAirTemplateTagGuid					= ""7F820D6E-D913-4258-8218-D0A68A8C4590"";
					private const string StdDensityInAirTemplateTagGuid				= ""78B4749F-F02B-489c-9B0C-46C2F9E02116"";
					private const string GrossStdWeightTemplateTagGuid					= ""F640ABC2-F3B7-471a-834A-D231ACE7DC2A"";
					private const string NetStdWeightTemplateTagGuid					= ""EF176082-14E0-4a30-8FCC-53465F70E897"";
					private const string GrossStdVolumeTemplateTagGuid					= ""76699AB2-813B-45f9-9F85-3DE09F22D6DD"";
					private const string BSWVolumeTemplateTagGuid						= ""13EE61B5-5420-4CC1-9A6C-56DB3022423E"";
					private const string TotalCalculatedVolumeTemplateTagGuid		= ""0434B936-4328-4BCE-A4DE-9BEFC2307437"";
					private const string TemperatureAmbientTemplateTagGuid			= ""AF72ED89-FA23-446D-A551-16A915C8E0E9"";
					private const string CritizalZoneTemplateTagGuid					= ""DBF87C9C-65DB-41fe-AAB0-37129382866F"";
					private const string LevelLowLimitTemplateTagGuid					= ""a591c864-0421-4cb9-adf8-3d95598c02d2"";
					private const string LevelLoLoLimitTemplateTagGuid					= ""6e346a70-2ae0-4697-aab7-6e0ba21ad5b2"";
					private const string LevelProductMinOpLimitTemplateTagGuid				= ""79c8b5a4-30e7-4a4c-81be-38f82eb50bf3"";
					private const string LevelHighLimitTemplateTagGuid					= ""72711441-6dfb-4aff-b398-d5fcfa51f6d7"";
					private const string LevelHiHiLimitTemplateTagGuid					= ""6d127899-ac8f-4acc-bb16-8f1325980d70"";
					private const string LevelProductMaxOpLimitTemplateTagGuid				= ""11469d43-5c8b-492e-b166-272abfe7976a"";
					private const string VolumeTotalObservedRateTagGuid				= ""F7B61B07-7364-4DBF-82C8-6EDC1E7C6E21"";
					private const string VolumeNetStandardRateTagGuid					= ""2D928F24-9B54-40D5-B00B-D19D87069D74"";
					private const string LevelProductRateTagGuid							= ""70ADFA50-CCCA-4ABC-8055-5889F4433E26"";
					private const string VolumeGrossObservedRateTagGuid				= ""66BDC1EB-98F8-4179-B54F-E9CBCA9D8DE0"";
					private const string TankCommandTagGuid								= ""99164989-A1DB-4C83-A834-01396B8D589E"";
					private const string TankStatusTagGuid									= ""EDD65B84-474F-4CD9-B169-42517668338C"";
					private const string LevelProductStopTagGuid							= ""B9E03854-EDDC-4165-8A11-6FD56B79E988"";
					private const string LevelProductMovementTagGuid					= ""A7B30ADC-FBF2-4F35-94A6-ED75B9E5E062"";
					private const string TankModeDiscreteAlarmTagGuid					= ""3E0B375C-F090-430A-BC16-A4C9883D0F13"";
					private const string TransferModeTemplateTagGuid					= ""B24E6A16-AB76-4980-A648-07A724D84A74"";
					private const string TransferStatusTemplateTagGuid					= ""7A451A61-E2E6-480E-AE85-609E7BC2A57F"";
					private const string TransferTargetSetPointTemplateTagGuid		= ""CF589E1E-5BBE-49F5-8381-3160709E2889"";
					private const string TransferLevelStartTemplateTagGuid			= ""12F92CBC-BEA9-472D-87B2-34D2A838647C"";
					private const string TransferGOVStartTemplateTagGuid				= ""DC7BFBCA-4F68-46EB-92B4-3921B6E13019"";
					private const string TransferNSVStartTemplateTagGuid				= ""8678EA00-E579-4A04-9DC4-F655731DCE3C"";
					private const string TransferVolumeWaterStartTemplateTagGuid	= ""3D8C66F7-E4B7-4CD4-91B1-65C8FE050EBD"";
					private const string TransferTimeRemainingTemplateTagGuid		= ""37A589E6-4230-4A9F-8044-AA66C7BED7A7"";
					private const string TransferTimeCompletionTemplateTagGuid		= ""C34BC94B-F8E8-41ED-80E4-5FEB10094785"";
					private const string TransferredGOVTemplateTagGuid					= ""EE65D3E3-818F-4304-846C-AB711471185C"";
					private const string TransferredNSVTemplateTagGuid					= ""B7043057-E2C9-4BD3-848C-04FD0CD6E0A7"";
					private const string TransferredVolumeWaterTemplateTagGuid		= ""06DA0DAF-1227-4E3B-85CA-689A95101060"";
					private const string TankTransferDiscreteAlarmTemplateTagGuid	= ""C2400FDD-55BB-4E43-ABB5-7D87B510513A"";
					private const string VolumeGOVAvailableTagGuid						= ""EA90B57C-E223-4041-95C0-8BC15A097755"";
					private const string VolumeGOVRemainingTagGuid						= ""ABAFCCD1-1480-47A4-81AF-2D86413DD27D"";
					private const string VolumeNSVAvailableTagGuid						= ""816A00C8-FCD7-4D79-A17C-BD8C8F0AD2DC"";
					private const string VolumeNSVRemainingTagGuid						= ""8F35B5D4-CA96-47FA-8A76-AD99326B9D19"";
					private const string TransferStartVolumeTemplateTagGuid			= ""F270261C-C376-4617-98FA-37C67A4C1019"";
					private const string TransferredVolumeTemplateTagGuid				= ""B9361FF1-4D5F-44C8-AA36-8302F26E2BEE"";
					private const string TransferStartTimeTemplateTagGuid				= ""F60377C7-DD21-4ECA-B22C-BE2F6950C85E"";
					private const string TransferStopTimeTemplateTagGuid				= ""349FAEA0-766A-4C20-BF1F-7CB85E7BE1FC"";
					private const string TransferLevelTargetTagGuid						= ""64710B23-C502-4513-8C7C-8B7812ABA684"";
					private const string TransferVolumeTargetTagGuid					= ""86A99CD8-0235-4CC5-BDA4-F5E4CBD15894"";

					// added for vapor calculations
					private const string DensityVaporTemplateTagGuid				= ""9BFEBBE8-BF75-430b-8F79-0DE0AA6DD430"";
					private const string VolumeVaporNetTemplateTagGuid				= ""AE0EF9EC-9D71-4e57-929E-4ED668026383"";
					private const string MassVaporTemplateTagGuid					= ""0B5FB637-24BF-428a-9152-E327719F6B5E"";

					// added for gauge density
					private const string DensityProductGaugeProductTemplateTagGuid		= ""07A95F08-2794-480e-92BD-0FF62CD8F7F2"";

					// Setting PointTemplateGuids
					private const string StrapTablePointTemplatePropertyGuid						= ""F48F45A0-80B4-4CEC-8DC1-4C49B1B72169"";
					private const string VesselPointTemplatePropertyGuid							= ""96FDC24A-2E74-4a3a-A20A-033659207A39"";
					private const string VcfSettingsPointTemplatePropertyGuid					= ""8529fc7f-7d00-4344-a968-58273c7ee6d7"";
					private const string QuantitySettingsPointTemplatePropertyGuid				= ""E51C7A94-96CD-4E2B-AD1D-12E62F916998"";
					private const string LevelRateSettingsPointTemplatePropertyGuid			= ""15443351-1677-43DE-8775-911D0885175B"";
					private const string TotalObservedRateSettingsPointTemplatePropertyGuid = ""7FCD093E-3D52-46AA-8F4B-D68E21328423"";
					private const string NetStandardRateSettingsPointTemplatePropertyGuid	= ""41EEBEF8-7020-47F2-A95A-3E084F6CE3DC"";
					private const string GrossObservedRateSettingsPointTemplatePropertyGuid = ""90046419-3720-43BB-BDA6-BD74EE2A87E3"";
					private const string TankCommandSettingsPointTemplatePropertyGuid			= ""5150A1F0-6E7B-480B-9AE2-F113669B1955"";
					private const string TankTransferSettingsPointTemplatePropertyGuid		= ""B2DA91E2-CD00-44FA-AB8D-DDA06A12B7BA"";

					#endregion

					#region Constructors
					/// <summary>
					/// This is the default constructor for the standard tank object.
					/// </summary>
					/// <param name=""point"">The point that contains the tags.</param>
					public StandardTank(Point point, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Dictionary<Guid, string> moduleLogicScript) : base(point)
					{

						// Initialize Tag References
						this.LevelProduct													= this.GetTag(LevelTemplateTagGuid);
						this.LevelWater													= this.GetTag(WaterLevelTemplateTagGuid);
						this.LevelSolids													= this.GetTag(SolidsLevelTemplateTagGuid);
						this.VolumeStrapProduct											= this.GetTag(StrapVolumeTemplateTagGuid);
						this.VolumeStrapWater											= this.GetTag(WaterStrapVolumeTemplateTagGuid);
						this.TemperatureProduct											= this.GetTag(TemperatureProductTemplateTagGuid);
						this.TemperatureDensity											= this.GetTag(TemperatureDensityTemplateTagGuid);
						this.TemperatureVapor											= this.GetTag(TemperatureVaporTemplateTagGuid);
						this.DensityObserved												= this.GetTag(DensityProductObservedTemplateTagGuid);
						this.DensityStandard												= this.GetTag(DensityProductStandardTemplateTagGuid);
						this.PressureVapor												= this.GetTag(PressureVaporTemplateTagGuid);
						this.VolumeCorrectionforTemperature					= this.GetTag(VolCorForTempTemplateTagGuid);
						this.VolumeCorrectionforPressure						= this.GetTag(VolCorForPressTemplateTagGuid);
						this.VolumeCorrectionforTemperatureandPressure	= this.GetTag(VolCorForPressTempTemplateTagGuid);
						this.VolumeCorrectionFactor									= this.GetTag(VolumeCorrectionFactorTemplateTagGuid);
						this.VolumeCorrectionFactorUnrounded						= this.GetTag(VolumeCorrectionFactorUnroundedTemplateTagGuid);
						this.APICorrectionError											= this.GetTag(APICorrectionErrorTemplateTagGuid);
						this.GaugeCommand													= this.GetTag(GaugeCommandTagGuid);
						this.VolumeGrossObserved										= this.GetTag(GrossObservedVolumeTemplateTagGuid);
						this.VolumeRoofCorrection										= this.GetTag(RoofCorrectionTemplateTagGuid);
						this.VolumeBottom													= this.GetTag(BottomVolumeTemplateTagGuid);
						this.PercentBSW													= this.GetTag(PercentBSWTemplateTagGuid);
						this.TankShellCorrection										= this.GetTag(TankShellCorrectionTemplateTagGuid);
						this.VolumeNetStandard											= this.GetTag(NetStandardVolumeTemplateTagGuid);
						this.VolumeNetStandardUnrounded								= this.GetTag(NetStandardVolumeUnroundedTemplateTagGuid);
						this.Mass															= this.GetTag(MassTemplateTagGuid);
						this.VolumeStrapSolids											= this.GetTag(SolidsVolumeTemplateTagGuid);
						this.DensityObservedinAir										= this.GetTag(DensityInAirTemplateTagGuid);
						this.DensityStandardinAir										= this.GetTag(StdDensityInAirTemplateTagGuid);
						this.WeightGrossStandard										= this.GetTag(GrossStdWeightTemplateTagGuid);
						this.WeightNetStandard											= this.GetTag(NetStdWeightTemplateTagGuid);
						this.VolumeGrossStandard										= this.GetTag(GrossStdVolumeTemplateTagGuid);
						this.VolumeBSW														= this.GetTag(BSWVolumeTemplateTagGuid);
						this.VolumeTotalCalculated										= this.GetTag(TotalCalculatedVolumeTemplateTagGuid);
						this.TemperatureAmbient											= this.GetTag(TemperatureAmbientTemplateTagGuid);
						this.RoofCriticalZone													= this.GetTag(CritizalZoneTemplateTagGuid);
						this.LevelLowLimit												= this.GetTag(LevelLowLimitTemplateTagGuid);
						this.LevelLoLoLimit												= this.GetTag(LevelLoLoLimitTemplateTagGuid);
						this.LevelProductMinOpLimit												= this.GetTag(LevelProductMinOpLimitTemplateTagGuid);
						this.LevelHighLimit												= this.GetTag(LevelHighLimitTemplateTagGuid);
						this.LevelHiHiLimit												= this.GetTag(LevelHiHiLimitTemplateTagGuid);
						this.LevelProductMaxOpLimit												= this.GetTag(LevelProductMaxOpLimitTemplateTagGuid);
						this.VolumeTotalObservedRate									= this.GetTag(VolumeTotalObservedRateTagGuid);
						this.VolumeNetStandardRate										= this.GetTag(VolumeNetStandardRateTagGuid);
						this.LevelProductRate											= this.GetTag(LevelProductRateTagGuid);
						this.VolumeGrossObservedRate									= this.GetTag(VolumeGrossObservedRateTagGuid);
						this.TankCommand													= this.GetTag(TankCommandTagGuid);
						this.TankStatus													= this.GetTag(TankStatusTagGuid);
						this.LevelProductStop											= this.GetTag(LevelProductStopTagGuid);
						this.LevelProductMovement										= this.GetTag(LevelProductMovementTagGuid);
						this.TankModeDiscreteAlarm										= this.GetTag(TankModeDiscreteAlarmTagGuid);

						this.TransferMode													= this.GetTag(TransferModeTemplateTagGuid);
						this.TransferStatus												= this.GetTag(TransferStatusTemplateTagGuid);
						this.TransferTarget												= this.GetTag(TransferTargetSetPointTemplateTagGuid);
						this.TransferStartLevelProduct								= this.GetTag(TransferLevelStartTemplateTagGuid);
						this.TransferStartGOV											= this.GetTag(TransferGOVStartTemplateTagGuid);
						this.TransferStartNSV											= this.GetTag(TransferNSVStartTemplateTagGuid);
						this.TransferStartVolumeWater									= this.GetTag(TransferVolumeWaterStartTemplateTagGuid);
						this.TransferStartVolume										= this.GetTag(TransferStartVolumeTemplateTagGuid);
						this.TransferTimeRemaining										= this.GetTag(TransferTimeRemainingTemplateTagGuid);
						this.TransferTimeCompletion									= this.GetTag(TransferTimeCompletionTemplateTagGuid);
						this.TransferredGOV												= this.GetTag(TransferredGOVTemplateTagGuid);
						this.TransferredNSV												= this.GetTag(TransferredNSVTemplateTagGuid);
						this.TransferredVolumeWater									= this.GetTag(TransferredVolumeWaterTemplateTagGuid);
						this.TransferredVolume											= this.GetTag(TransferredVolumeTemplateTagGuid);
						this.TankTransferDiscreteAlarm								= this.GetTag(TankTransferDiscreteAlarmTemplateTagGuid);
						this.TransferStartTime											= this.GetTag(TransferStartTimeTemplateTagGuid);
						this.TransferStopTime											= this.GetTag(TransferStopTimeTemplateTagGuid);
						this.TransferLevelTarget										= this.GetTag(TransferLevelTargetTagGuid);
						this.TransferVolumeTarget										= this.GetTag(TransferVolumeTargetTagGuid);

						// added for vapor calculations
						this.DensityVapor													=this.GetTag(DensityVaporTemplateTagGuid);
						this.VolumeVaporNet												=this.GetTag(VolumeVaporNetTemplateTagGuid);
						this.MassVapor														=this.GetTag(MassVaporTemplateTagGuid);

						this.VolumeGOVAvailable											= this.GetTag(VolumeGOVAvailableTagGuid);
						this.VolumeGOVRemaining											= this.GetTag(VolumeGOVRemainingTagGuid);
						this.VolumeNSVAvailable											= this.GetTag(VolumeNSVAvailableTagGuid);
						this.VolumeNSVRemaining											= this.GetTag(VolumeNSVRemainingTagGuid);

						// added for gauge density calculations
						this.DensityProductGauge										= this.GetTag(DensityProductGaugeProductTemplateTagGuid);

						// Initialize Property References
						this.StrapTableProperty							= this.GetProperty(StrapTablePointTemplatePropertyGuid);
						this.VcfSettingsProperty						= this.GetProperty(VcfSettingsPointTemplatePropertyGuid);
						this.VesselProperty								= this.GetProperty(VesselPointTemplatePropertyGuid);
						this.QuantitySettingsProperty					= this.GetProperty(QuantitySettingsPointTemplatePropertyGuid);
						this.LevelRateSettingsProperty				= this.GetProperty(LevelRateSettingsPointTemplatePropertyGuid); ;
						this.TotalObservedRateSettingsProperty		= this.GetProperty(TotalObservedRateSettingsPointTemplatePropertyGuid); 
						this.NetStandardRateSettingsProperty		= this.GetProperty(NetStandardRateSettingsPointTemplatePropertyGuid);
						this.GrossObservedRateSettingsProperty		= this.GetProperty(GrossObservedRateSettingsPointTemplatePropertyGuid);
						this.TankCommandSettingsProperty				= this.GetProperty(TankCommandSettingsPointTemplatePropertyGuid);
						this.TransferSettingsProperty					= this.GetProperty(TankTransferSettingsPointTemplatePropertyGuid);

						// Instantiate Modules
						this.Vcf											= new FMVcf();
						this.Quantities								= new FMQuantities();
						this.StrapTable								= new FMStrapTable();
						this.ShellCorrection							= new FMShellCorrection();
						this.RoofCorrection							= new FMFloatingRoofCorrection();
						this.RateModuleLevel							= new FMRateModule();
						this.RateModuleTotalObserved				= new FMRateModule();
						this.RateModuleNetStandard					= new FMRateModule();
						this.RateModuleGrossObserved				= new FMRateModule();
						this.TankCommands								= new FMTankCommands();
						this.TankTransfer								= new FMTankTransfer();
						this.AvailableAndRemainingVolume			= new FMAvailableAndRemainingVolume();
						this.StandardTankCalculator				= new FMStandardTankCalculator();

						// Set Module References
						this.ShellCorrection.GetPoint				= this.GetPoint;

						this.TankTransfer.StrapTable = this.StrapTable;
						this.TankTransfer.RoofCorrection = this.RoofCorrection;
						this.TankTransfer.Quantities = this.Quantities;
						this.TankTransfer.SetPointTag = this.SetPointTag;
						this.TankTransfer.SetPointProperty = this.SetPointProperty;

						this.AvailableAndRemainingVolume.StrapTable = this.StrapTable;
						this.AvailableAndRemainingVolume.RoofCorrection = this.RoofCorrection;
						this.AvailableAndRemainingVolume.Quantities = this.Quantities;

						this.StandardTankCalculator.StrapTable = this.StrapTable;
						this.StandardTankCalculator.Quantities = this.Quantities;
						this.StandardTankCalculator.VolumeCorrection = this.Vcf;
						this.StandardTankCalculator.ShellCorrection = this.ShellCorrection;
						this.StandardTankCalculator.FloatingRoofCorrection = this.RoofCorrection;
						this.StandardTankCalculator.AvailableAndRemainingVolume = this.AvailableAndRemainingVolume;
					}
					#endregion

		
					#region Public methods
					/// <summary>
					/// This method overrides the Execute base class to initialize tags and settings.
					/// </summary>
					public override void Execute(V8ScriptEngine v8Engine, PointTemplateLogic.CalculationType calculationType, PointCalculatorData pointCalculatorData)
					{
						if(this.InitializationFailed)
						{
							return;
						}

						// Apply Module Settings
						this.StrapTable.StrapTable					= this.StrapTableProperty.Value as StrapTable;
						this.StrapTable.Vessel						= this.VesselProperty.Value as Vessel;
						this.Vcf.VcfSettings						= this.VcfSettingsProperty.Value as VcfModuleSettings;
						this.ShellCorrection.VcfSettings			= this.VcfSettingsProperty.Value as VcfModuleSettings;
						this.ShellCorrection.Vessel					= this.VesselProperty.Value as Vessel;
						this.ShellCorrection.StrapTable				= this.StrapTableProperty.Value as StrapTable;
						this.ShellCorrection.QuantitySettings		= this.QuantitySettingsProperty.Value as QuantityModuleSettings;
						this.Quantities.QuantitySettings			= this.QuantitySettingsProperty.Value as QuantityModuleSettings;
						this.Quantities.VcfSettings					= this.VcfSettingsProperty.Value as VcfModuleSettings;
						this.Quantities.Vessel						= this.VesselProperty.Value as Vessel;
						this.RoofCorrection.StrapTable				= this.StrapTableProperty.Value as StrapTable;
						this.RoofCorrection.QuantitySettings		= this.QuantitySettingsProperty.Value as QuantityModuleSettings;
						this.RateModuleLevel.Settings				= this.LevelRateSettingsProperty.Value as RateModuleSettings;
						this.RateModuleTotalObserved.Settings		= this.TotalObservedRateSettingsProperty.Value as RateModuleSettings;
						this.RateModuleNetStandard.Settings			= this.NetStandardRateSettingsProperty.Value as RateModuleSettings;
						this.RateModuleGrossObserved.Settings		= this.GrossObservedRateSettingsProperty.Value as RateModuleSettings;
						this.TankCommands.TankCommandSettings		= this.TankCommandSettingsProperty.Value as TankCommandModuleSettings;
						this.TankTransfer.TankTransferSettings			= this.TransferSettingsProperty.Value as TankTransferModuleSettings;


						if(calculationType == PointTemplateLogic.CalculationType.Calculator)
						{
							if(pointCalculatorData != null)
							{
								var pointLogicTags = new List<PointTag>();
								pointLogicTags.Add(this.VolumeCorrectionFactorUnrounded);
								pointLogicTags.Add(this.VolumeNetStandardUnrounded);
								pointLogicTags.Add(this.VolumeCorrectionforTemperature);
								pointLogicTags.Add(this.VolumeCorrectionforPressure);
								pointLogicTags.Add(this.VolumeCorrectionforTemperatureandPressure);
								pointLogicTags.Add(this.APICorrectionError);
								pointLogicTags.Add(this.DensityObservedinAir);
								pointLogicTags.Add(this.DensityStandardinAir);
								pointLogicTags.Add(this.RoofCriticalZone);
								pointLogicTags.Add(this.VolumeBSW);
								pointLogicTags.Add(this.VolumeVaporNet);
								pointLogicTags.Add(this.LevelProductMinOpLimit);
								pointLogicTags.Add(this.LevelProductMaxOpLimit);
								pointLogicTags.Add(this.DensityProductGauge);


								// call the standardtankcalculator module
								this.StandardTankCalculator.TankCalculatorCalculation(pointCalculatorData, pointLogicTags);
							}
							else
							{
								// call the standardtankcalculator module using mapped in values
								this.StandardTankCalculator.TankCalculatorCalculation(this.LevelProduct,
																					this.LevelWater, 
																					this.LevelSolids,
																					this.VolumeStrapProduct,
																					this.VolumeStrapWater,
																					this.VolumeStrapSolids,
																					this.TemperatureProduct, 
																					this.TemperatureDensity,
																					this.TemperatureVapor,
																					this.TemperatureAmbient,
																					this.PressureVapor,
																					this.PercentBSW,
																					this.DensityStandard,
																					this.DensityObserved,
																					this.DensityVapor,
																					this.TankShellCorrection,
																					this.VolumeCorrectionFactor,
																					this.VolumeBottom,
																					this.VolumeGrossObserved,
																					this.VolumeGOVAvailable,
																					this.VolumeGOVRemaining,
																					this.VolumeNetStandard,
																					this.VolumeNSVAvailable,
																					this.VolumeNSVRemaining,
																					this.VolumeRoofCorrection,
																					this.Mass,
																					this.MassVapor,
																					this.VolumeCorrectionforTemperature,
																					this.VolumeCorrectionforPressure,
																					this.VolumeCorrectionforTemperatureandPressure,
																					this.APICorrectionError,
																					this.DensityObservedinAir,
																					this.DensityStandardinAir,
																					this.RoofCriticalZone,
																					this.WeightGrossStandard,
																					this.WeightNetStandard,
																					this.VolumeGrossStandard,
																					this.VolumeBSW,
																					this.VolumeTotalCalculated,
																					this.VolumeVaporNet,
																					this.LevelProductMinOpLimit,
																					this.LevelProductMaxOpLimit,
																					this.DensityProductGauge,
																					this.VolumeCorrectionFactorUnrounded,
																					this.VolumeNetStandardUnrounded
								);
							}
						}
						else
						{

							//  Basic Calculation Sequence for Level Product
							this.StrapTable.StrapCalculation(this.LevelProduct, this.LevelWater, this.LevelSolids, this.VolumeStrapProduct, this.VolumeStrapWater, this.VolumeStrapSolids);

							this.Vcf.VcfCalculation(this.TemperatureProduct, this.TemperatureDensity, this.TemperatureVapor, this.DensityStandard, this.DensityObserved, this.PressureVapor,
													this.VolumeCorrectionforTemperature, this.VolumeCorrectionforPressure, this.VolumeCorrectionforTemperatureandPressure,
													this.VolumeCorrectionFactor, this.VolumeCorrectionFactorUnrounded, this.APICorrectionError, this.DensityObservedinAir, this.DensityStandardinAir,this.DensityProductGauge);

							this.ShellCorrection.ShellCorrectionCalculation(this.TemperatureAmbient, this.TemperatureProduct, this.TankShellCorrection);

							this.RoofCorrection.FloatingRoofCorrectionCalculation(this.TemperatureProduct, this.DensityObserved, this.DensityObservedinAir, this.Mass, this.LevelProduct, this.VolumeCorrectionFactor, this.RoofCriticalZone, this.VolumeRoofCorrection);

							this.Quantities.QuantityCalculation(this.VolumeRoofCorrection, this.VolumeStrapProduct, this.VolumeStrapWater, this.VolumeStrapSolids,
																this.PercentBSW, this.VolumeCorrectionFactor, this.VolumeCorrectionFactorUnrounded, this.DensityStandard,
																this.DensityObserved, this.TankShellCorrection, this.DensityObservedinAir, this.DensityStandardinAir, this.DensityVapor,
																this.TemperatureVapor, this.PressureVapor, this.VolumeBottom, this.VolumeGrossObserved, this.VolumeNetStandard,
																this.VolumeNetStandardUnrounded, this.Mass, this.WeightGrossStandard, this.VolumeGrossStandard, this.WeightNetStandard,
																this.VolumeBSW, this.VolumeTotalCalculated, this.VolumeVaporNet, this.MassVapor);

							this.RateModuleLevel.RateCalculation(this.LevelProduct, this.LevelProductRate);
							this.RateModuleTotalObserved.RateCalculation(this.VolumeStrapProduct, this.VolumeTotalObservedRate);
							this.RateModuleNetStandard.RateCalculation(this.VolumeNetStandard, this.VolumeNetStandardRate);
							this.RateModuleGrossObserved.RateCalculation(this.VolumeGrossObserved, this.VolumeGrossObservedRate);

							this.AvailableAndRemainingVolume.AvailableAndRemainingVolumeCalculation(
											this.LevelProductMinOpLimit, this.LevelProductMaxOpLimit,
											this.VolumeStrapProduct, this.VolumeRoofCorrection, this.TemperatureProduct, this.DensityObserved, this.DensityStandardinAir, this.Mass, this.VolumeCorrectionFactor, this.VolumeBottom, this.RoofCriticalZone,
											this.VolumeStrapWater, this.VolumeStrapSolids, this.PercentBSW, this.TankShellCorrection, this.VolumeGrossObserved, this.VolumeNetStandard,
											this.VolumeGOVAvailable, this.VolumeNSVAvailable, this.VolumeGOVRemaining, this.VolumeNSVRemaining);

							this.TankTransfer.TransferCalculation(this.LevelProduct, this.VolumeGrossObserved, this.LevelProductRate, this.VolumeGrossObservedRate, this.VolumeNetStandardRate,
																		this.VolumeNetStandard, this.VolumeStrapWater, this.TransferMode, this.TransferStatus, this.TransferTarget,
																		this.TemperatureProduct, this.DensityObserved, this.DensityObservedinAir, this.Mass,
																		this.VolumeCorrectionFactor, this.VolumeBottom, this.TankShellCorrection, this.PercentBSW,
																		this.VolumeGOVAvailable, this.VolumeGOVRemaining, this.VolumeNSVAvailable, this.VolumeNSVRemaining,
																		TransferStartLevelProduct, this.TransferStartGOV, this.TransferStartNSV, this.TransferStartVolumeWater, this.TransferStartVolume, this.TransferTimeRemaining,
																		this.TransferTimeCompletion, this.TransferredGOV, TransferredNSV, TransferredVolumeWater, TransferredVolume,
																		this.TankTransferDiscreteAlarm, TankCommand, this.LevelProductMaxOpLimit, this.LevelProductMinOpLimit, this.TransferStartTime, this.TransferStopTime,
																		this.TransferLevelTarget, TransferVolumeTarget);


							this.TankCommands.TankCommandCalculation(this.TankCommand, this.VolumeTotalObservedRate, this.LevelProduct,
																		  this.TankStatus, this.TankModeDiscreteAlarm, this.LevelProductStop, this.LevelProductMovement);
						}

					}
					#endregion
				}
			}
		";
	}
}
