// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadFileInfo.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the DownloadFileInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ADC.Nspa.General
{
	using System.Net;

	/// <summary>
	/// File info for the download file.
	/// </summary>
	public class DownloadFileInfo
	{
		/// <summary>
		/// The block size of file read to calculate MD5.  It seems to matter.
		/// </summary>
		public const int BlockSize = 8 * 1024;

		/// <summary>
		/// Custom http code return when server experiences any errors.
		/// </summary>
		public const HttpStatusCode CustomHttpErrorStatusCode = (HttpStatusCode)550;

		public string FileId { get; set; }

		/// <summary>
		/// Gets or sets the file hash. (MD5 with the BlockSize constant)
		/// </summary>
		/// <value>
		/// The file hash.
		/// </value>
		public byte[] FileHash { get; set; }

	}
}
