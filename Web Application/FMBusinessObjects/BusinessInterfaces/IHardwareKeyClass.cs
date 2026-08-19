using System;
using System.ServiceModel;


namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IHardwareKey
	{
		[OperationContract]
		bool IsDefenseKey();

		[OperationContract]
		bool IsDescKey();

		[OperationContract]
		bool IsADFKey();

		[OperationContract]
		bool IsMODKey();

		[OperationContract]
		bool IsTFMDKey();

		[OperationContract]
		bool IsDescEnterpriseKey();

		[OperationContract]
		bool IsDescProfessionalKey();

		[OperationContract]
		bool IsNspaEnterpriseKey();

		[OperationContract]
		bool IsNspaProfessionalKey();

		[OperationContract]
		bool IsAviationProduct();

		[OperationContract]
		bool IsAnOrderEntryKey();

		[OperationContract]
		bool IsMovementKey();

		[OperationContract]
		bool IsLeakDetectionKey();

		[OperationContract]
		uint GetOptionsCell();

        [OperationContract]
        ushort GetUseNewLicenseFile();

        [OperationContract]
		ushort GetProgramVersion();

		[OperationContract]
		ushort GetProgramVersionLIN();

		[OperationContract]
		ushort GetWord1ValueLIN();

		[OperationContract]
		ushort GetWord2ValueLIN();

		[OperationContract]
		void ReadHardwareKey();

		[OperationContract]
		void CheckVersion();

		[OperationContract]
        ushort CheckActivatedLicenceVersion();

        [OperationContract]
		uint GetSpecialKeyCodes();

		[OperationContract]
		bool IsMultipleSiteKey();

		[OperationContract]
		uint GetOPCAllowedFunctions();

        [OperationContract]
        bool IsEnterpriseKey();

        [OperationContract]
        bool IsTacFuelsKey();

      [OperationContract]
      bool IsDataAnalyticsKey();

      [OperationContract]
      bool IsDatawarehouseKey();

      [OperationContract]
        bool GetLicenseExpired();

		[OperationContract]
		long GetLicenseDaysLeftToExpire();

        [OperationContract]
        DateTime GetLicenseExpirationDate();
    }
}
