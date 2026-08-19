namespace FMPointService.PointExecution
{

	internal class StandardVolumeScript
	{

		public const string StandardVolume = @"
			namespace FMPointService.PointExecution
			{
				using System;
				using System.Collections.Generic;
				using Microsoft.ClearScript.V8;
				using FMBusinessObjects.DataObjects;
				using RateModules;
				using VolumeTransfer;
				using TotalizerCalculation;
				using VCF;

				public class StandardVolume : PointTemplateLogic
				{
					#region Private data members
					// Tags
					private PointTag	TransferDiscreteAlarm;
					private PointTag	TransferMode;
					private PointTag	TransferStartGOV;
					private PointTag	TransferStartNSV;
					private PointTag	TransferStartTime;
					private PointTag	TransferStartVolume;
					private PointTag	TransferStatus;
					private PointTag	TransferStopTime;
					private PointTag	TransferTarget;
					private PointTag	TransferTimeCompletion;
					private PointTag	TransferTimeRemaining;
					private PointTag	TransferredGOV;
					private PointTag	TransferredNSV;
					private PointTag	TransferredVolume;
					private PointTag	TransferVolumeTarget;
					private PointTag	VolumeGrossObserved;
					private PointTag	VolumeGrossObservedRate;
					private PointTag	VolumeNetStandard;
					private PointTag	VolumeNetStandardRate;

					private PointTag	TemperatureProduct;
					private PointTag	TemperatureDensity;
					private PointTag	TemperatureVapor;
					private PointTag	DensityObserved;
					private PointTag	DensityStandard;
					private PointTag	PressureVapor;
					private PointTag	VolumeCorrectionFactorForTemperature;
					private PointTag	VolumeCorrectionFactorForPressure;
					private PointTag	VolumeCorrectionFactorForPressureAndTemperature;
					private PointTag	VolumeCorrectionFactor;
					private PointTag	VolumeCorrectionFactorUnrounded;
					private PointTag	APICorrectionError;
					private PointTag	DensityObservedInAir;
					private PointTag	DensityStandardInAir;
					private PointTag	DensityGauge;

					private PointTag	StandardVolumeReset;
					private PointTag	StandardVolumeFlowFilter;
					private PointTag	PulseMeterNumberOfRollOvers;
					private PointTag	PulseMeterLastValue;
					private PointTag	PulseMeterCurrentValue;
					private PointTag	PulseMeterVolumePerPulse;
					private PointTag	PulseMeterRollOverAmount;
					private PointTag	PulseMeterLastReadWasRollOver;

					// Properties
					private PointProperty	GrossObservedRateSettingsProperty;
					private PointProperty	NetStandardRateSettingsProperty;
					private PointProperty	VolumeTransferSettingsProperty;
					private PointProperty	VcfSettingsProperty;

					// Modules
					private FMRateModule RateModuleGrossObserved;
					private FMRateModule RateModuleNetStandard;
					private VolumeTransfer.FMVolumeTransfer VolumeTransfer;
					private TotalizerCalculation.FMTotalizer FMTotalizer;
					private FMVcf Vcf;

					// Tag PointTemplateGuids
					private const string StandardVolumeVolumeGrossObservedTagGuid		= ""ADC283D1-A9D0-42E2-B69F-D867B91C9B80"";
					private const string StandardVolumeVolumeNetStandardTagGuid			= ""7A1DB105-DB02-4968-B17D-7D78B445AED9"";
					private const string StandardVolumeVolumeGrossObservedRateTagGuid	= ""F663C1BA-0B0A-460C-830F-C86070D7BB42"";
					private const string StandardVolumeVolumeNetStandardRateTagGuid		= ""85CA2C60-3B5E-4FA2-938A-2541AC207EC3"";
					private const string StandardVolumeTransferModeTagGuid				= ""9F5CB5A7-1C07-477A-A53D-E69A625DC7E1"";
					private const string StandardVolumeTransferStatusTagGuid			= ""BA9621C5-D9FB-41B6-885E-A9451D54ABA1"";
					private const string StandardVolumeTransferTargetTagGuid			= ""29DA71C4-1A51-42EE-8DFB-C781BE5C7B2E"";
					private const string StandardVolumeTransferredGOVTagGuid			= ""06BF336D-97E8-4AE6-A092-332731ACDF76"";
					private const string StandardVolumeTransferredNSVTagGuid			= ""BB42BC34-E90A-4067-B89E-FFFF9547F0E0"";
					private const string StandardVolumeTransferredVolumeTagGuid			= ""945926ED-C944-4D5F-A6B6-22A64A660B70"";
					private const string StandardVolumeTransferStartGOVTagGuid			= ""30A2BB23-DB8E-445A-BE09-32F7F1132F25"";
					private const string StandardVolumeTransferStartNSVTagGuid			= ""A8FDE285-C978-407D-A994-797AADF58C13"";
					private const string StandardVolumeTransferStartVolumeTagGuid		= ""88F438F1-F4D2-4048-9C4B-D8A3B693C6DF"";
					private const string StandardVolumeTransferTimeRemainingTagGuid		= ""85BD69C2-E54E-45AF-AB06-ACC17C1E8F76"";
					private const string StandardVolumeTransferTimeCompletionTagGuid	= ""4F24D174-D533-4CBB-9654-D7D6DA332AFC"";
					private const string StandardVolumeVolumeTransferDescreteAlarmTagGuid  = ""7595F7E2-1490-4D22-BC43-68D09A3DCFDF"";
					private const string StandardVolumeTransferVolumeTargetTagGuid		= ""902EF38D-5516-4BAB-951A-A0E0394D3DFC"";
					private const string StandardVolumeAdvisoryAlarmTagGuid				= ""B39E729D-D26F-40EA-BD7E-8283E61E599F"";
					private const string StandardVolumeTargetAlarmTagGuid				= ""D3738551-BBB1-48A2-83F5-05BA5B4DFA15"";
					private const string StandardVolumeReverseFlowAlarmTagGuid			= ""76C46A63-5C6B-4FE7-A69A-F95E8C5D3379"";
					private const string StandardVolumeAdvisoryAlarmLimitTagGuid		= ""E3CEB56C-2EB3-4D3A-966D-A1117B2993A7"";
					private const string StandardVolumeTargetAlarmLimitTagGuid			= ""7C692270-E28D-450F-9BFF-00C50F439031"";
					private const string StandardVolumeReverseFlowAlarmLImitTagGuid		= ""4924CCAD-0580-4956-8458-DDC82E061F0A"";
					private const string StandardVolumeMovementIdTagGuid				= ""72AB75AC-878D-4E8D-BC5A-B4AEE0BB4778"";
					private const string StandardVolumeDensityProductObservedTagGuid	= ""75A69330-C70F-4BCC-B390-5C1905927D6E"";
					private const string StandardVolumeDensityProductStandardTagGuid	= ""7AB31EB0-F82F-4E36-9DCB-5A125B1AD1D1"";
					private const string StandardVolumeMassTagGuid						= ""F1304099-C72D-452B-A857-9177F9DB4094"";
					private const string StandardVolumeTemperatureProductTagGuid		= ""2A572C1B-3670-4D69-8AB2-303BBD4640A1"";
					private const string StandardVolumeTransferStartTimeTagGuid			= ""9C1B1656-8C94-4E25-8862-E6A3B1E8A902"";
					private const string StandardVolumeTransferStopTimeTagGuid			= ""D4E548CA-40DA-4B79-A21A-C4D2EEC6C737"";

					private const string TemperatureDensityTemplateTagGuid				= ""C6D3FE2B-B301-4E1F-ABA1-4E67BB214F99"";
					private const string TemperatureVaporTemplateTagGuid				= ""4CE264BE-116F-4C7D-A0D2-B8CB2ECE7747"";
					private const string DensityProductObservedTemplateTagGuid			= ""75A69330-C70F-4BCC-B390-5C1905927D6E"";
					private const string DensityProductStandardTemplateTagGuid			= ""7AB31EB0-F82F-4E36-9DCB-5A125B1AD1D1"";
					private const string PressureVaporTemplateTagGuid					= ""E959D2BC-83FA-4B41-A195-4D2029B69710"";
					private const string VolCorForTempTemplateTagGuid					= ""EB7441A8-32B0-4116-A976-90A18B55FE13"";
					private const string VolCorForPressTemplateTagGuid					= ""3ECE1AAA-B613-4EE2-A320-B3FA5A314A95"";
					private const string VolCorForPressTempTemplateTagGuid				= ""D4DA43C5-4437-4213-A0F8-3CE11D415AB7"";
					private const string VolumeCorrectionFactorTemplateTagGuid			= ""81E007E0-F325-438C-BBC4-B7667173EC6E"";
					private const string VolumeCorrectionFactorUnroundedTemplateTagGuid = ""4F02A4CF-A9C3-4B14-94D6-BAF528B62737"";
					private const string APICorrectionErrorTemplateTagGuid				= ""4C8FDBE5-6617-4B17-B239-138559EE0DB3"";
					private const string DensityInAirTemplateTagGuid					= ""15BD13CE-1B09-4E0B-8E72-8BC50A77CE46"";
					private const string StdDensityInAirTemplateTagGuid					= ""0FA5B3B2-ADBF-41BF-AE36-960F24C41273"";
					private const string DensityGaugeProductTemplateTagGuid				= ""C96CA69D-FB49-4EEE-A1D5-E0377BDDE643"";

					private const string StandardVolumeResetTagGuid						= ""1A82134A-98B9-43C9-86C2-EFA232BDEB19"";
					private const string StandardVolumeFlowFilterTagGuid				= ""0FDEECCA-BBC1-4A68-8879-9611294A1CDC"";
					private const string PulseMeterNumberOfRollOversTagGuid				= ""9CE844BE-4A53-4E31-BDE1-95EDE4D7D1C0"";
					private const string PulseMeterLastValueTagGuid						= ""1D4C53CD-41E2-49E3-8AAF-7A6A247A779A"";
					private const string PulseMeterCurrentValueTagGuid					= ""C878E5A7-3EDF-470B-94F2-415D66A9A3A2"";
					private const string PulseMeterVolumePerPulseTagGuid				= ""C9454F0E-A573-4D37-94FF-9C7E8BDD8DEE"";
					private const string PulseMeterRollOverAmountTagGuid				= ""6A5EC2CA-E2C9-4BDA-AD07-0B56B2372DA5"";
					private const string PulseMeterLastReadWasRollOverTagGuid			= ""1385627C-5BD4-4381-95F3-50D8C0C44507"";

					// Setting PointTemplateGuids
					private const string GrossObservedRateSettingsPointTemplatePropertyGuid = ""A668316B-2963-41F9-AF8D-162EE2F5D255"";
					private const string NetStandardRateSettingsPointTemplatePropertyGuid	= ""B7A7C15F-18A9-4653-81F3-BD35AE33F44F"";
					private const string VolumeTransferSettingsPointTemplatePropertyGuid	= ""2060F1CA-2E48-4C94-84FE-82988FFDA0A4"";
					private const string VcfSettingsPointTemplatePropertyGuid				= ""2B1CFEAD-A4DC-40E0-A53B-19005075463B"";

					#endregion

					#region Constructors
					/// <summary>
					/// This is the default constructor for the standard tank object.
					/// </summary>
					/// <param name=""point"">The point that contains the tags.</param>
					public StandardVolume(Point point, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Dictionary<Guid, string> moduleLogicScript) : base(point)
					{
						// Initialize Tag References
						this.TransferDiscreteAlarm								= base.GetTag(StandardVolumeVolumeTransferDescreteAlarmTagGuid);
						this.TransferMode											= base.GetTag(StandardVolumeTransferModeTagGuid);
						this.TransferStartGOV									= base.GetTag(StandardVolumeTransferStartGOVTagGuid);
						this.TransferStartNSV									= base.GetTag(StandardVolumeTransferStartNSVTagGuid);
						this.TransferStartTime									= base.GetTag(StandardVolumeTransferStartTimeTagGuid);
						this.TransferStartVolume								= base.GetTag(StandardVolumeTransferStartVolumeTagGuid);
						this.TransferStatus										= base.GetTag(StandardVolumeTransferStatusTagGuid);
						this.TransferStopTime									= base.GetTag(StandardVolumeTransferStopTimeTagGuid);
						this.TransferTarget										= base.GetTag(StandardVolumeTransferTargetTagGuid);
						this.TransferTimeCompletion							= base.GetTag(StandardVolumeTransferTimeCompletionTagGuid);
						this.TransferTimeRemaining								= base.GetTag(StandardVolumeTransferTimeRemainingTagGuid);
						this.TransferredGOV										= base.GetTag(StandardVolumeTransferredGOVTagGuid);
						this.TransferredNSV										= base.GetTag(StandardVolumeTransferredNSVTagGuid);
						this.TransferredVolume									= base.GetTag(StandardVolumeTransferredVolumeTagGuid);
						this.TransferVolumeTarget								= base.GetTag(StandardVolumeTransferVolumeTargetTagGuid);

						this.VolumeGrossObserved								= base.GetTag(StandardVolumeVolumeGrossObservedTagGuid);
						this.VolumeGrossObservedRate							= base.GetTag(StandardVolumeVolumeGrossObservedRateTagGuid);
						this.VolumeNetStandard									= base.GetTag(StandardVolumeVolumeNetStandardTagGuid);
						this.VolumeNetStandardRate								= base.GetTag(StandardVolumeVolumeNetStandardRateTagGuid);

						this.TemperatureProduct									= base.GetTag(StandardVolumeTemperatureProductTagGuid);
						this.TemperatureDensity									= this.GetTag(TemperatureDensityTemplateTagGuid);
						this.TemperatureVapor									= this.GetTag(TemperatureVaporTemplateTagGuid);
						this.DensityObserved									= this.GetTag(DensityProductObservedTemplateTagGuid);
						this.DensityStandard									= this.GetTag(DensityProductStandardTemplateTagGuid);
						this.PressureVapor										= this.GetTag(PressureVaporTemplateTagGuid);
						this.VolumeCorrectionFactorForTemperature				= this.GetTag(VolCorForTempTemplateTagGuid);
						this.VolumeCorrectionFactorForPressure					= this.GetTag(VolCorForPressTemplateTagGuid);
						this.VolumeCorrectionFactorForPressureAndTemperature	= this.GetTag(VolCorForPressTempTemplateTagGuid);
						this.VolumeCorrectionFactor								= this.GetTag(VolumeCorrectionFactorTemplateTagGuid);
						this.VolumeCorrectionFactorUnrounded					= this.GetTag(VolumeCorrectionFactorUnroundedTemplateTagGuid);
						this.DensityGauge										= this.GetTag(DensityGaugeProductTemplateTagGuid);
						this.APICorrectionError									= this.GetTag(APICorrectionErrorTemplateTagGuid);
						this.DensityObservedInAir								= this.GetTag(DensityInAirTemplateTagGuid);
						this.DensityStandardInAir								= this.GetTag(StdDensityInAirTemplateTagGuid);

						this.StandardVolumeReset								= base.GetTag(StandardVolumeResetTagGuid);
						this.StandardVolumeFlowFilter							= base.GetTag(StandardVolumeFlowFilterTagGuid);
						this.PulseMeterNumberOfRollOvers						= base.GetTag(PulseMeterNumberOfRollOversTagGuid);
						this.PulseMeterLastValue								= base.GetTag(PulseMeterLastValueTagGuid);
						this.PulseMeterCurrentValue								= base.GetTag(PulseMeterCurrentValueTagGuid);
						this.PulseMeterVolumePerPulse							= base.GetTag(PulseMeterVolumePerPulseTagGuid);
						this.PulseMeterRollOverAmount							= base.GetTag(PulseMeterRollOverAmountTagGuid);
						this.PulseMeterLastReadWasRollOver						= base.GetTag(PulseMeterLastReadWasRollOverTagGuid);

						// Initialize Property References
						this.GrossObservedRateSettingsProperty	= this.GetProperty(GrossObservedRateSettingsPointTemplatePropertyGuid);
						this.NetStandardRateSettingsProperty	= this.GetProperty(NetStandardRateSettingsPointTemplatePropertyGuid);
						this.VolumeTransferSettingsProperty		= base.GetProperty(VolumeTransferSettingsPointTemplatePropertyGuid);
						this.VcfSettingsProperty				= this.GetProperty(VcfSettingsPointTemplatePropertyGuid);

						// Instantiate Modules
						this.RateModuleGrossObserved	= new FMRateModule();
						this.RateModuleNetStandard		= new FMRateModule();
						this.VolumeTransfer				= new VolumeTransfer.FMVolumeTransfer();
						this.Vcf						= new FMVcf();	
						this.FMTotalizer				= new FMTotalizer();

						// Set Module References
						this.VolumeTransfer.SetPointTag = this.SetPointTag;
						this.VolumeTransfer.SetPointProperty = this.SetPointProperty;

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
						this.RateModuleGrossObserved.Settings		= this.GrossObservedRateSettingsProperty.Value as RateModuleSettings;
						this.RateModuleNetStandard.Settings			= this.NetStandardRateSettingsProperty.Value as RateModuleSettings;
						this.VolumeTransfer.VolumeTransferSettings	= this.VolumeTransferSettingsProperty.Value as VolumeTransferModuleSettings;
						this.Vcf.VcfSettings						= this.VcfSettingsProperty.Value as VcfModuleSettings;

						this.Vcf.VcfCalculation(this.TemperatureProduct, this.TemperatureDensity, this.TemperatureVapor, this.DensityStandard, this.DensityObserved, this.PressureVapor,
												this.VolumeCorrectionFactorForTemperature, this.VolumeCorrectionFactorForPressure, this.VolumeCorrectionFactorForPressureAndTemperature,
												this.VolumeCorrectionFactor, this.VolumeCorrectionFactorUnrounded, this.APICorrectionError, this.DensityObservedInAir, this.DensityStandardInAir,this.DensityGauge);

						this.FMTotalizer.TotalizerCalculation(
								this.VolumeGrossObserved,
								this.VolumeNetStandard,
								this.PulseMeterNumberOfRollOvers,
								this.PulseMeterLastValue,
								this.PulseMeterCurrentValue,
								this.PulseMeterVolumePerPulse,
								this.PulseMeterRollOverAmount,
								this.PulseMeterLastReadWasRollOver,
								this.VolumeCorrectionFactor);

						//  Basic Calculation Sequence for Movement Node
						this.RateModuleGrossObserved.RateCalculation(this.VolumeGrossObserved, this.VolumeGrossObservedRate);
						this.RateModuleNetStandard.RateCalculation(this.VolumeNetStandard, this.VolumeNetStandardRate);

						this.VolumeTransfer.TransferCalculation(
							this.VolumeGrossObserved,
							this.VolumeGrossObservedRate,
							this.VolumeNetStandard,
							this.VolumeNetStandardRate,
							this.TransferMode,
							this.TransferStatus,
							this.TransferTarget,
							this.TransferStartGOV,
							this.TransferStartNSV,
							this.TransferStartVolume,
							this.TransferTimeRemaining,
							this.TransferTimeCompletion,
							this.TransferredGOV,
							this.TransferredNSV,
							this.TransferredVolume,
							this.TransferDiscreteAlarm,
							this.TransferStartTime,
							this.TransferStopTime,
							this.TransferVolumeTarget);
					}
					#endregion
				}
			}
		";
	}
}
