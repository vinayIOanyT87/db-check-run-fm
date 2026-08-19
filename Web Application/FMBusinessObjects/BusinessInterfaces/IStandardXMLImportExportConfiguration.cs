using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Text;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IStandardXMLImportExportConfiguration
	{
		[OperationContract]
		ImportFilter GetConfiguration( SecurityClass security, string name );

		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void SaveConfiguration( SecurityClass security, ImportFilter filter );
	}
}
