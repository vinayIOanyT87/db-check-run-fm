using System.IO;
using System.Net;

namespace FMExportService
{
	interface IWebServiceSender {

		string WebServiceSenderName { get; }

		void Send(Stream File, string WebServiceEndPoint);

		void Send(Stream File, string WebServiceEndPoint, NetworkCredential Credentials);

	}
}
