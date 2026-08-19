using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IEntityExcelImport
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
      void StartImport(SecurityClass securityParam, SiteClass siteParam, string entityDocXmlString);
	}
}
