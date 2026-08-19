using System.IO;

namespace FMExportService
{
	/// <summary>
	/// This interface defines how a FM Export Service web service plug-in will behave.
	/// </summary>
	public interface IWebServicePlugin {

		/// <summary>
		/// This is a human readable-name that will uniquely identify the plug-in.
		/// </summary>
		string WebServicePluginID { get; }

		/// <summary>
		/// This method must be called before accessing the Send() method. The string should be built using the same format as a database connection string:
		/// param1=value1;param2=value2
		/// The parameters and values are unique to every web service plug-in, allowing for custom parameters to be passed to a plug-in when run.
		/// </summary>
		/// <param name="ConfigurationString">A delimited configuration string.</param>
		void SetConfiguration(string ConfigurationString);

		/// <summary>
		/// Sends a file to a web service.
		/// </summary>
		/// <param name="FileName">The path of the file to send.</param>
		void Send(string FileName);

		/// <summary>
		/// Sends a data stream to a web service.
		/// </summary>
		/// <param name="File">The stream of data to send.</param>
		void Send(Stream File);

	}
}
