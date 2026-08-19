namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPictures
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		Guid Add( SecurityClass security, Picture picture );

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void Purge( SecurityClass security, Guid pictureGuid );

		[OperationContract]
		Picture Get( SecurityClass security, Guid pictureGuid);

		[OperationContract]
		PictureCollection Enumerate(SecurityClass security);

	    [OperationContract]
	    Guid GetPictureGuidByImageHash(SecurityClass security, string imageHash);

	}
}
