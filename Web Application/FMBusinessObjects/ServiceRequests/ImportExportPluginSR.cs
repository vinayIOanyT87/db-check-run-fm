using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class ImportExportPluginSR : AccountingServiceRequest
	{
		public ImportExportPluginSR ( )
		{
		}
	}
}
