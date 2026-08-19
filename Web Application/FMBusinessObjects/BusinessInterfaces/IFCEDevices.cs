
namespace FMBusinessObjects.BusinessInterfaces
{
	 using System;
	 using System.Collections.Generic;
	 using System.ServiceModel;
	 using FMBusinessObjects.DataObjects;

	 [ServiceContract]
	 public interface IFCEDevices
	 {
		  [OperationContract]
		  Guid? Add(SecurityClass security, FCEDevice fceDevice);

		  [OperationContract]
		  FCEDevice Get(SecurityClass security, Guid fceDeviceGuid);

        [OperationContract]
        FCEDevice GetbyIMEI(SecurityClass security, string IMEI);

        [OperationContract]
		  void Modify(SecurityClass security, FCEDevice fceDevice);

		  [OperationContract]
		  void Purge(SecurityClass security, Guid fceDeviceGuid);


		  [OperationContract]
		  List<FCEDevice> EnumerateBySiteGuid(SecurityClass security, Guid siteGuid);
	 }
}
