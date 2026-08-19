
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAnimationDrawingMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AnimationToDrawingMapClass animationToDrawing);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AnimationToDrawingMapClass animationToDrawing);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid animationToDrawingGuid);

		[OperationContract]
		AnimationToDrawingMapClass Get(SecurityClass security, Guid animationToDrawingGuid);

		[OperationContract]
		Dictionary<Guid, AnimationToDrawingMapClass> EnumerateByAnimationToDrawingGuids(SecurityClass security, List<Guid> animationToDrawingGuidList);

		[OperationContract]
		Dictionary<Guid, AnimationToDrawingMapClass> EnumerateByAnimationGuids(SecurityClass security, List<Guid> animationGuidList);

		[OperationContract]
		Dictionary<Guid, AnimationToDrawingMapClass> EnumerateByDrawingGuids(SecurityClass security, List<Guid> drawingGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModifyAnimationToDrawingMap(SecurityClass security, List<AnimationToDrawingMapClass> animationToDrawingList, bool enableAdd, bool enableModify, bool enableDelete);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAnimationToDrawingMaps(SecurityClass security, List<Guid> animationToDrawingGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAnimationToDrawingMapsByDrawingGuidList(SecurityClass security, List<Guid> drawingGuidList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAnimationToDrawingMapsByAnimationGuidList(SecurityClass security, List<Guid> animationGuidList);
	}
}
