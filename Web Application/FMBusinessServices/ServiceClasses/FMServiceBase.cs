namespace FMBusinessServices.ServiceClasses
{
	using System;

	using FMBusinessObjects.DataObjects;

	public class FMServiceBase
	{
		protected void ValidateUserData(SecurityClass security, FMBaseDataObjectWithUserData entity)
		{
			var userDataFields = new UserDataFieldsClass();

			UserDataFieldCollectionClass userDataFieldCollection = userDataFields.EnumerateByEntityType(
				security, entity.EntityType, Guid.Empty, false, false );

			foreach ( UserDataFieldClass userDataField in userDataFieldCollection )
			{
				if ( userDataField.FieldRequired && string.IsNullOrEmpty( entity.UserData[userDataField.Number] ) )
				{
					var dictionary = new DataDictionariesClass();
					var message = dictionary.Get(security.SiteGuid, "User data field is required") + ": " + userDataField.DisplayName;
					throw new Exception(message);
				}
			}
		}
	}
}