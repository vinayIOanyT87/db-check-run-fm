/// <summary>
/// Represents the different send method for file transfers using the FM Export Service.
/// </summary>
/// <remarks>
/// TFS #72786 - FM Export Service Web Service Plug-ins
/// Bryan Ponnwitz - 4/18/2017
/// </remarks>

namespace FMBusinessObjects.Constants
{
	public enum FileSendMethodEnum {
		/// <summary>
		/// Do not send the file, only write it to disk.
		/// </summary>
		None = 0,

		/// <summary>
		/// Transfer the file using the FTP protocol.
		/// </summary>
		FTP = 1,

		/// <summary>
		/// Transfer the file using the FTPS protocol.
		/// Note: SFTP is not supported.
		/// </summary>
		FTPS = 2,

		/// <summary>
		/// Transfer the file using a custom web service snap-in.
		/// </summary>
		WebService = 3
	}
}
