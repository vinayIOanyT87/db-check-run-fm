namespace FMPointService.PointExecution
{

	internal class StandardTduScript
	{

		public const string StandardTdu = @"
			namespace FMPointService.PointExecution
			{
				using System;
				using System.Collections.Generic;
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
				public class TDU : PointTemplateLogic
				{
					#region Private data members
					// Tags

					private PointTag	NotePad;
					private PointTag	SearchHartCommand;
					private PointTag	SearchHartTkNum;
					private PointTag	TDUFirmware;
					private PointTag	TDUStatus;
					private PointTag	TDUTemp;
					private PointTag	TDUVoltage;
					private PointTag	TrainComm;
					private PointTag	TrCurrIndex;
					private PointTag	TrDirection;
					private PointTag	TrEndVolume;
					private PointTag	TrInitialVol;
					private PointTag	TrNextVol;
					private PointTag	TrPressure;
					private PointTag	TrRelaxVol;
					private PointTag	TrStatus;
					private PointTag	TrTemp;
					private PointTag	TrTkNumber;
					private PointTag	TrVolume;

					// Properties

					// Modules

					// Tag PointTemplateGuids

					private const string NotePadTagGuid					= ""B7208B3A-5233-7226-F694-B9EC10DD4CB8"";
					private const string SearchHartCommandTagGuid	= ""34B93CBA-001C-34B4-002D-0D2A4020FF80"";
					private const string SearchHartTkNumTagGuid		= ""FD100787-1D4D-B2B9-2553-A4BD08864ADD"";
					private const string TDUFirmwareTagGuid			= ""1170CCE1-93BF-4CF0-BD7E-4F542583336B"";
					private const string TDUStatusTagGuid				= ""D59C5E63-0447-8BAC-70F0-E5719DEBD5CB"";
					private const string TDUTempTagGuid					= ""892CABCF-14CF-4AC2-243E-9145CFE5A998"";
					private const string TDUVoltageTagGuid				= ""894CCDE6-E173-9CCA-EA42-B92AA2A1CD21"";
					private const string TrainCommTagGuid				= ""8817CA5F-5360-78CD-36FA-E27F150A4DB0"";
					private const string TrCurrIndexTagGuid			= ""BF851720-EA89-63CF-9C37-FDBAE622D663"";
					private const string TrDirectionTagGuid			= ""8A6FC66E-9C9C-8CDD-6BBA-A98FD5A34577"";
					private const string TrEndVolumeTagGuid			= ""7A89ABCE-3792-DEC8-2244-3E057E224396"";
					private const string TrInitialVolTagGuid			= ""015AA21D-5011-BF49-1E7B-B0505D03E942"";
					private const string TrNextVolTagGuid				= ""A4DC706F-8606-2A2D-AF6C-4EEB3541C8BF"";
					private const string TrPressureTagGuid				= ""850514A2-3577-12CF-2895-2A14CC2A422E"";
					private const string TrRelaxVolTagGuid				= ""53933321-77A3-743E-59D0-868C04754B17"";
					private const string TrStatusTagGuid				= ""D59C5E63-0447-8BAC-70F0-E5719DEBD5CB"";
					private const string TrTempTagGuid					= ""892CABCF-14CF-4AC2-243E-9145CFE5A998"";
					private const string TrTkNumberTagGuid				= ""5E0FFB68-E900-B798-75C2-927D6042978A"";
					private const string TrVolumeTagGuid				= ""8DD84845-13E8-EBB1-B134-73BA8BA2B25B"";

					// Setting PointTemplateGuids

					#endregion

					#region Constructors
					/// <summary>
					/// This is the default constructor for the standard tdu object.
					/// </summary>
					/// <param name=""point"">The point that contains the tags.</param>
					public TDU(Point point, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Dictionary<Guid, string> moduleLogicScript) : base(point)
					{
						// Initialize Tag References

						this.NotePad								= base.GetTag(NotePadTagGuid);
						this.SearchHartCommand					= base.GetTag(SearchHartCommandTagGuid);
						this.SearchHartTkNum						= base.GetTag(SearchHartTkNumTagGuid);
						this.TDUFirmware							= base.GetTag(TDUFirmwareTagGuid);
						this.TDUStatus								= base.GetTag(TDUStatusTagGuid);
						this.TDUTemp								= base.GetTag(TDUTempTagGuid);
						this.TDUVoltage							= base.GetTag(TDUVoltageTagGuid);
						this.TrainComm								= base.GetTag(TrainCommTagGuid);
						this.TrCurrIndex							= base.GetTag(TrCurrIndexTagGuid);
						this.TrDirection							= base.GetTag(TrDirectionTagGuid);
						this.TrEndVolume							= base.GetTag(TrEndVolumeTagGuid);
						this.TrInitialVol							= base.GetTag(TrInitialVolTagGuid);
						this.TrNextVol								= base.GetTag(TrNextVolTagGuid);
						this.TrPressure							= base.GetTag(TrPressureTagGuid);
						this.TrRelaxVol							= base.GetTag(TrRelaxVolTagGuid);
						this.TrStatus								= base.GetTag(TrStatusTagGuid);
						this.TrTemp									= base.GetTag(TrTempTagGuid);
						this.TrTkNumber							= base.GetTag(TrTkNumberTagGuid);
						this.TrVolume								= base.GetTag(TrVolumeTagGuid);

						// Initialize Property References

						// Instantiate Modules

						// Set Module References

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
					}

					#endregion
				}
			}
		";
	}
}
