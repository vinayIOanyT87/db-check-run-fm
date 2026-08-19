namespace FMPointService.PointExecution
{

	internal class MovementScript
	{

		public const string StandardMovement = @"
			namespace FMPointService.PointExecution
			{
				using System;
				using System.Collections.Generic;
				using Microsoft.ClearScript.V8;
				using FMBusinessObjects.DataObjects;
				using RateModules;
				using Movement;

				public class StandardMovement : PointTemplateLogic
				{
					#region Private data members
					// Tags
					private PointTag Command;
					private PointTag PercentDeviation;
					private PointTag Deviation;
					private PointTag DeviationHighAlarm;
					private PointTag DeviationHighAlarmLimit;
					private PointTag DeviationLowAlarm;
					private PointTag DeviationLowAlarmLimit;
					private PointTag Status;
					private PointTag TransferStartTime;
					private PointTag TransferStopTime;
					private PointTag InitiationCount;
					private PointTag MovementHistoryWrittenTime;
					private PointTag TransferredGOV;
					private PointTag TransferredNSV;
					private PointTag TransferTimeRemaining;
					private PointTag StartIdentity;
					private PointTag StopIdentity;
					private PointTag MovementDiscreteAlarm;

					// Properties
					private PointProperty MovementSettingsProperty;
					private PointProperty MovementDataProperty;

					// Modules
					private FMMovement Movement;


					private const string CommandTagGuid = ""45F4AF52-126A-4836-A336-6CDE6D611E3B"";
					private const string PercentDeviationTagGuid = ""A95E83BA-CDD9-43C5-81BC-3DCF8145FFA0"";
					private const string DeviationHighAlarmTagGuid = ""E8C66F94-0808-4C47-9F25-28670D050B3D"";
					private const string DeviationHighAlarmLimitTagGuid = ""20868E89-6330-4E90-A17B-C9E886BE7DCC"";
					private const string DeviationLowAlarmTagGuid = ""3F87E87E-6747-4B79-8E8D-6B6732D8375D"";
					private const string DeviationLowAlarmLimitTagGuid = ""6D186506-76BC-4AD5-A7DC-1DCC97A3B0C4"";
					private const string StatusTagGuid = ""065DA402-8A0C-4CDB-B64F-83B7B4C0D3ED"";
					private const string TransferStartTimeTagGuid = ""1113B77F-E421-4086-B535-5C7CF3D16922"";
					private const string TransferStopTimeTagGuid = ""955D3D56-B476-4B9A-9C8E-88A5B0D139A8"";
					private const string InitiationCountTagGuid = ""4DCDC163-E055-417F-9016-9BB1913E730C"";
					private const string MovementHistoryWrittenTimeTagGuid = ""D15E46AB-741D-4533-AEB9-95C1E98C9689"";
					private const string TransferredGOVTagGuid = ""5CAA7F26-9A2A-4E67-A8F3-694BF5E2EF6B"";
					private const string TransferredNSVTagGuid = ""F021C476-325D-4CF8-A59F-95B6B136A483"";
					private const string DeviationTagGuid = ""9A8866D0-FE08-456B-B494-7ED408863960"";
					private const string TransferTimeRemainingGuid = ""009F1EB8-4EE2-4B8F-AB75-7A066C1FECA0"";
					private const string StartIdentityGuid = ""64BE1F86-A923-4752-9902-D5BFF4711EC1"";
					private const string StopIdentityGuid = ""CA09FCDF-81FF-4C52-8A1E-4B1867D0DF3F"";
					private const string MovementDiscreteAlarmGuid = ""322E377C-1995-4AFE-A1EA-AEAAC02D5C85"";

					// Setting PointTemplateGuids
					private const string MovementSettingsPointTemplatePropertyGuid = ""FC861EC7-89C7-4430-ABE2-7CAA8B9FBEC1"";
					private const string MovementDataPointTemplatePropertyGuid = ""5C760DDA-DCD6-4EF0-BE8A-AEDEDFA7A3EC"";



					#endregion


					#region Constructors
					/// <summary>
					/// This is the default constructor for the standard tank object.
					/// </summary>
					/// <param name=""point"">The point that contains the tags.</param>
					public StandardMovement(Point point, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Dictionary<Guid, string> moduleLogicScript) : base(point)
					{
						// Initialize Tag References
						this.Command = this.GetTag(CommandTagGuid); 
						this.PercentDeviation = this.GetTag(PercentDeviationTagGuid); 
						this.DeviationHighAlarm = this.GetTag(DeviationHighAlarmTagGuid); 
						this.DeviationHighAlarmLimit = this.GetTag(DeviationHighAlarmLimitTagGuid);
						this.DeviationLowAlarm = this.GetTag(DeviationLowAlarmTagGuid);
						this.DeviationLowAlarmLimit = this.GetTag(DeviationLowAlarmLimitTagGuid);
						this.Status = this.GetTag(StatusTagGuid);
						this.TransferStartTime = this.GetTag(TransferStartTimeTagGuid);
						this.TransferStopTime = this.GetTag(TransferStopTimeTagGuid);
						this.InitiationCount = this.GetTag(InitiationCountTagGuid);
						this.MovementHistoryWrittenTime = this.GetTag(MovementHistoryWrittenTimeTagGuid);
						this.TransferredGOV = this.GetTag(TransferredGOVTagGuid);
						this.TransferredNSV = this.GetTag(TransferredNSVTagGuid);
						this.Deviation = this.GetTag(DeviationTagGuid); 
						this.TransferTimeRemaining = this.GetTag(TransferTimeRemainingGuid);
						this.StartIdentity = this.GetTag(StartIdentityGuid);
						this.StopIdentity = this.GetTag(StopIdentityGuid);
						this.MovementDiscreteAlarm = this.GetTag(MovementDiscreteAlarmGuid);
	
						// Initialize Property References
						this.MovementSettingsProperty = this.GetProperty(MovementSettingsPointTemplatePropertyGuid);
						this.MovementDataProperty = this.GetProperty(MovementDataPointTemplatePropertyGuid);

						// Instantiate Modules
						this.Movement = new FMMovement();

						// Set Module References
						this.Movement.SetPointTag = this.SetPointTag;
						this.Movement.SetPointProperty = this.SetPointProperty;

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
						this.Movement.MovementModuleSettings = this.MovementSettingsProperty.Value as MovementModuleSettings;
						this.Movement.MovementData = this.MovementDataProperty.Value as MovementData;

						//  Basic Calculation Sequence for Movement
						this.Movement.MovementCalculation(	this.PercentDeviation
																		, this.Command
																		, this.Status
																		, this.TransferStartTime
																		, this.TransferStopTime
																		, this.InitiationCount
																		, this.MovementHistoryWrittenTime
																		, this.TransferredGOV
																		, this.TransferredNSV
																		, this.TransferTimeRemaining
																		, this.StartIdentity
																		, this.StopIdentity
																		, this.MovementDiscreteAlarm); 

					}
					#endregion
				}
			}
		";
	}
}
