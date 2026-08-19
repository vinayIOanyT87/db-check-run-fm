namespace FMPointService.PointExecution
{

	internal class MovementControlScript
	{
		public const string StandardMovementControl = @"
			namespace FMPointService.PointExecution
			{
				using System;
				using System.Collections.Generic;
				using Microsoft.ClearScript.V8;
				using FMBusinessObjects.DataObjects;
				using MovementControl;

				public class StandardMovementControl : PointTemplateLogic
				{
					#region Private data members
					// Tags
					private PointTag Initiate;
					private PointTag Stop;
					private PointTag MovementIdentity;


					// Modules
					private FMMovementControl MovementControl;

					private const string InitiateTagGuid = ""BB7C5A17-E4B7-4ED1-95FD-3B6B82017415"";
					private const string StopTagGuid = ""8884B142-42EA-45A0-B2CF-87E5E4D8119E"";
					private const string MovementIdentityTagGuid = ""728F8010-A5F5-4500-A35F-4D919DBB4F73"";

					#endregion

					#region Constructors
					/// <summary>
					/// This is the default constructor for the standard movement control object.
					/// </summary>
					/// <param name=""point"">The point that contains the tags.</param>
					public StandardMovementControl(Point point, Dictionary<Guid, ModuleToPointTemplateMap> moduleInstances, Dictionary<Guid, string> moduleLogicScript) : base(point)
					{
						// Initialize Tag References
						this.Initiate = this.GetTag(InitiateTagGuid); 
						this.Stop = this.GetTag(StopTagGuid); 
						this.MovementIdentity = this.GetTag(MovementIdentityTagGuid); 

						// Instantiate Modules
						this.MovementControl = new FMMovementControl();
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

						//  Basic Calculation Sequence for Movement Control
						this.MovementControl.MovementControlCalculation(	this.Initiate
																							, this.Stop
																							, this.MovementIdentity); 

					}
					#endregion
				}
			}
		";
	}
}






