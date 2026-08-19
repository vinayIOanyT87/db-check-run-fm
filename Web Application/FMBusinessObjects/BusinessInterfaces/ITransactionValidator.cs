using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface ITransactionValidator
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		bool ValidateSite ( SecurityClass security, TransactionDO trans );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		bool ValidateTransactionAlias ( SecurityClass security, TransactionDO trans );

		[OperationContract]
		List<CloseoutDO> GetCloseoutDates(
			SecurityClass securityParameter,
			string siteID,
			Guid siteGuid,
			Guid managerGuid,
			List<Guid> productList);

		[OperationContract]
		GeneralConfigDO GetForcedCloseout(SecurityClass securityParameter, string siteID, Guid siteGuid);

		[OperationContract]
		string ValidateInventoryDate(
			string productId,
			string site,
			DateTime inventoryDate,
			DateTime? closeoutDate,
			List<CloseoutDO> fromManagerCloseoutListParam,
            List<CloseoutDO> toManagerCloseoutListParam,
            GeneralConfigDO generalConfiguration);

		/// <summary>
		/// Validate the provided transaction using the provided security credentials
		/// </summary>
		/// <param name="securityParam">Contains security information to be used when validating the transaction</param>
		/// <param name="inTrans">
		/// The transaction to validate
		/// </param>
		/// <returns>
		/// The <see cref="TransactionValidationResult"/>.
		/// </returns>
		[OperationContract]
		TransactionValidationResult ValidateTransaction(SecurityClass securityParam, TransactionDO inTrans);
	}
}
