namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IDrawings
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		Drawing Add( SecurityClass security, Drawing drawing );

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void Modify( SecurityClass security, Drawing drawing );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid drawingGuid);

		[OperationContract]
		List<DrawingName> EnumerateAvailableDrawingNames(SecurityClass security);

		[OperationContract]
		Dictionary<Guid, DrawingName> EnumerateByDrawingGuids(SecurityClass security, List<Guid> drawingGuidList);

		[OperationContract]
		List<DrawingName> EnumerateAvailableDrawingNamesByPanelType(SecurityClass security, List<PANELTYPE> panelTypes);

		[OperationContract]
		List<DrawingName> EnumerateAvailableDrawingNamesByPointTemplate(SecurityClass security, Guid pointTemplateGuid);

		[OperationContract]
		List<DrawingName> EnumerateAvailableDrawingNamesByPublished(SecurityClass security);

		[OperationContract]
		Drawing Get(SecurityClass security, Guid drawingGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string ID);
	}
}
