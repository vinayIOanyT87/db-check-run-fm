namespace FMPointService.PointExecution
{

	internal class StandardNodeScript
	{

		public const string StandardNode = @"
			namespace FMPointService.PointExecution
			{
				using System;
				using System.Collections.Generic;
				using Microsoft.ClearScript.V8;
				using FMBusinessObjects.DataObjects;
				using NodeTransfer;

				public class StandardNode : PointTemplateLogic
				{
					#region Private data members
					// Tags
					private PointTag	TransferMode;
					private PointTag	TransferStartTime;
					private PointTag	TransferStatus;
					private PointTag	TransferStopTime;

					// Properties

					// Modules
					private NodeTransfer.FMNodeTransfer NodeTransfer;

					// Tag PointTemplateGuids
					private const string StandardVolumeTransferModeTagGuid				= ""DA354855-777E-47C8-B932-4BD8C1F4BA8C"";
					private const string StandardVolumeTransferStatusTagGuid				= ""F55A77AE-EB91-4FAA-B5F0-6BE8F780D741"";
					private const string StandardVolumeTransferStartTimeTagGuid			= ""C4ADC176-6E56-46B3-A332-248C8A79B2BC"";
					private const string StandardVolumeTransferStopTimeTagGuid			= ""E93A7E6F-9C32-4FA7-B706-726CF4063C45"";


					// Setting PointTemplateGuids

					#endregion

					#region Constructors
					/// <summary>
					/// This is the default constructor for the standard tank object.
					/// </summary>
					/// <param name=""point"">The point that contains the tags.</param>
					public StandardNode(Point point, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Dictionary<Guid, string> moduleLogicScript) : base(point)
					{
						// Initialize Tag References
						this.TransferMode											= base.GetTag(StandardVolumeTransferModeTagGuid);
						this.TransferStartTime									= base.GetTag(StandardVolumeTransferStartTimeTagGuid);
						this.TransferStatus										= base.GetTag(StandardVolumeTransferStatusTagGuid);
						this.TransferStopTime									= base.GetTag(StandardVolumeTransferStopTimeTagGuid);


						// Initialize Property References

						// Instantiate Modules
						this.NodeTransfer					= new NodeTransfer.FMNodeTransfer();

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


						this.NodeTransfer.TransferCalculation(
							this.TransferMode,
							this.TransferStatus,
							this.TransferStartTime,
							this.TransferStopTime);
					}
					#endregion
				}
			}
		";
	}
}
