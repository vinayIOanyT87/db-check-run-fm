namespace MovementControl
{
	using System;
	using System.Linq;
	using System.Diagnostics;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.Constants;


	using FMBusinessObjects.DataObjects.CodedVariables;
	using FMPointCommon;
	using System.Collections.Generic;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.BusinessInterfaces;
	using Opc.Ua;

	public class FMMovementControl : FuelsManagerModule, IFuelsManagerModule
	{
		private bool? currentInitiate;

		private bool? currentStop;

		private SecurityClass security = new SecurityClass() { UserID = "FMPointService" };


		public void MovementControlCalculation(PointTag Initiate, PointTag Stop, PointTag MovementIdentity)
		{
			CalculateMovementInitiate(Initiate, MovementIdentity);

			CalculateMovementStop(Stop, MovementIdentity);
		}

		private void CalculateMovementInitiate(PointTag Initiate, PointTag MovementIdentity)
		{
			if (!IsValueGood(Initiate)
			|| !IsValueGood(MovementIdentity))
			{
				return;
			}

			Guid movementIdentityGuid;

			if (!(MovementIdentity.Value is String)
			|| !Guid.TryParse(MovementIdentity.Value as String, out movementIdentityGuid))
			{
				return;
			}


			if (Initiate.Value is Boolean
			&& (Boolean)Initiate.Value != currentInitiate)
			{
				currentInitiate = (bool)Initiate.Value;

				if ((Boolean)Initiate.Value)
				{


					var movementWellKnownTagGuidList = new Guid[] {
						Guids.MovementStatusGuid,
						Guids.MovementInitiateIdentityGuid
					};

					var pointGuidList = new List<Guid>
					{
						movementIdentityGuid
					};

					var movementPointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.security, pointGuidList, movementWellKnownTagGuidList.ToList()));
					var movementPointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(security, movementPointValueIdentifierList, false));

					if (movementPointValueList[0].Value is MovementStatus
					&& (MovementStatus)movementPointValueList[0].Value == MovementStatus.Inactive)
					{
						movementPointValueList.RemoveAt(0);

						movementPointValueList[0].Value = MovementIdentity.Value;
						movementPointValueList[0].Status = StatusCodes.Good;
						movementPointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
						movementPointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

						FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(security, movementPointValueList, false));
					}
				}
			}
		}

		private void CalculateMovementStop(PointTag Stop, PointTag MovementIdentity)
		{
			if (!IsValueGood(Stop)
			|| !IsValueGood(MovementIdentity))
			{
				return;
			}

			Guid movementIdentityGuid;

			if (!(MovementIdentity.Value is String)
			|| !Guid.TryParse(MovementIdentity.Value as String, out movementIdentityGuid))
			{
				return;
			}



			if (Stop.Value is Boolean
			&& (Boolean)Stop.Value != currentStop)
			{
				currentStop = (bool)Stop.Value;

				if ((Boolean)Stop.Value)
				{

					var movementWellKnownTagGuidList = new Guid[] {
						Guids.MovementStatusGuid,
						Guids.MovementStopIdentityGuid
					};

					var pointGuidList = new List<Guid>
					{
						movementIdentityGuid
					};

					var movementPointValueIdentifierList = FMChannelHelper.MakeCall<IPointTags, List<PointValueIdentifier>>(x => x.EnumeratePointValueIdentifersByPointAndTagLists(this.security, pointGuidList, movementWellKnownTagGuidList.ToList()));
					var movementPointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(x => x.GetPointValueData(security, movementPointValueIdentifierList, false));

					if (movementPointValueList[0].Value is MovementStatus
					&& (MovementStatus)movementPointValueList[0].Value != MovementStatus.Inactive
					&& (MovementStatus)movementPointValueList[0].Value != MovementStatus.Disabled)
					{
						movementPointValueList.RemoveAt(0);

						movementPointValueList[0].Value = MovementIdentity.Value;
						movementPointValueList[0].Status = StatusCodes.Good;
						movementPointValueList[0].ServerTimeStamp = DateTimeOffset.UtcNow;
						movementPointValueList[0].SourceTimeStamp = DateTimeOffset.UtcNow;

						FMChannelHelper.MakeCall<IPointServiceManager>(x => x.SetPointValueData(security, movementPointValueList, false));
					}
				}
			}
		}


		public ModuleInputOutputCollection GetInputOutputCollection(string calculationName)
		{
			var properties = new ModuleInputOutputCollection { };
			return properties;
		}
	}
}
