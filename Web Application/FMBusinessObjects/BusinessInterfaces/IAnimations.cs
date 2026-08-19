
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IAnimations
	{

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, AnimationClass animation);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, AnimationClass animation);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid animationGuid);

		[OperationContract]
		AnimationClass Get(SecurityClass security, Guid animationGuid);

		[OperationContract]
		Dictionary<Guid, AnimationClass> EnumerateByAnimationGuids(SecurityClass security, List<Guid> animationGuidList);

		[OperationContract]
		Dictionary<Guid, AnimationClass> EnumerateAnimationsBySiteGuid(SecurityClass security, Guid siteGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModifyAnimations(SecurityClass security, List<AnimationClass> animationList, bool enableAdd, bool enableModify);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteAnimations(SecurityClass security, List<Guid> animationGuidList);

	}
}
