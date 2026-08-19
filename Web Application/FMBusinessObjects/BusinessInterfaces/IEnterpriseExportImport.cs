using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;


namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IEnterpriseExportImport
	{
		[OperationContract]
		string WriteStreamToFile ( SecurityClass security, string eventLogSource, MemoryStream stream, string strDirPathToWriteTo );

		[OperationContract]
		void WriteToEventLogs(SecurityClass security, string eventLogSource, string strMessage, EventLogEntryType eventLogEntryType);

		[OperationContract]
		EnterpriseExportImportDO ReadSettings ( SecurityClass security, string eventLogSource );
	}
}
