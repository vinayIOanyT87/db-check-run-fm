// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FTPConnectionClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FTPConnectionClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The ftp connection class.
	/// </summary>
	[Serializable]
	public class FTPConnectionClass
	{
		/// <summary>
		/// The FMExport service event logger.
		/// </summary>
		private readonly FMExportServiceLogger logger;

		/// <summary>
		/// Initializes a new instance of the FTPConnectionClass class.
		/// </summary>
		public FTPConnectionClass()
		{
			this.logger = FMExportServiceLogger.Instance;
			this.Server = string.Empty;
			this.User = string.Empty;
			this.Password = string.Empty;
			this.DebugMode = false;
			this.EnableSSL = false;
			this.UsePassiveMode = false;
		}

		/// <summary>
		/// Gets or sets the Server.
		/// </summary>
		public string Server
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the User.
		/// </summary>
		public string User
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Password.
		/// </summary>
		public string Password
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether in debug mode.
		/// </summary>
		public bool DebugMode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable SSL.
		/// </summary>
		public bool EnableSSL
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use passive transfer mode.
		/// </summary>
		public bool UsePassiveMode
		{
			get;
			set;
		}

		/// <summary>
		/// Method to upload the specified file to the specified FTP Server
		/// </summary>
		/// <param name="filename">file full name to be uploaded</param>
		/// <returns>True if successful</returns>
		private bool Upload(string filename)
		{
			if (this.Server == string.Empty
			|| this.User == string.Empty
			|| this.Password == string.Empty)
			{
				this.logger.LogWarning("Unable to upload file \"" + filename + "\" because there are missing FTP credentials.  Check the configuration for this file export.", 1010);
				return false;
			}

			var fileInf = new FileInfo(filename);
			string uri = "ftp://" + this.Server + "/" + fileInf.Name;

			// Create FtpWebRequest object from the Uri provided
			FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(uri));

			// Provide the WebPermission Credintials
			reqFTP.Credentials = new NetworkCredential(this.User, this.Password);

			// By default KeepAlive is true, where the control connection is not closed
			// after a command is executed.
			reqFTP.KeepAlive = false;

			// Specify the command to be executed.
			reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

			// Notify the Server about the size of the uploaded file
			reqFTP.ContentLength = fileInf.Length;

			// Specify passive mode
			reqFTP.UsePassive = this.UsePassiveMode;

			// Specify SSL
			reqFTP.EnableSsl = this.EnableSSL;
			if (reqFTP.EnableSsl)
			{
				ServicePointManager.ServerCertificateValidationCallback = this.ValidateServerCertificate;
			}

			// The buffer size is set to 2kb
			const int BuffLength = 2048;
			byte[] buff = new byte[BuffLength];
			
			try {
				// Opens a file stream (System.IO.FileStream) to read the file to be uploaded
				using (FileStream fs = fileInf.OpenRead()) {
					// Stream to which the file to be upload is written
					using (Stream strm = reqFTP.GetRequestStream()) {
						// Read from the file stream 2kb at a time
						int contentLen = fs.Read(buff, 0, BuffLength);

						// Till Stream content ends
						while (contentLen != 0) {
							// Write Content from the file stream to the FTP Upload Stream
							strm.Write(buff, 0, contentLen);
							contentLen = fs.Read(buff, 0, BuffLength);
						}
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				// Write message to event log
				this.logger.LogError("Error uploading file " + fileInf.Name + ":\n" + ex.ToString(), 1003);

				if (this.DebugMode)
				{
					this.logger.LogError("StackTrace:\n" + ex.StackTrace, 1004);
				}

				return false;
			}
		}

		/// <summary>
		/// Validates server certificate.
		/// </summary>
		/// <param name="sender">The sender</param>
		/// <param name="certificate">The certificate</param>
		/// <param name="chain">The chain</param>
		/// <param name="sslPolicyErrors">The SSL policy errors</param>
		/// <returns>True if server certificate is valid</returns>
		private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			// Allow use of self signed certificate in debug mode
			if (!this.DebugMode && (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors))
			{
				return false;
			}

			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				System.Security.Policy.Zone z = 
					System.Security.Policy.Zone.CreateFromUrl(((FtpWebRequest)sender).RequestUri.ToString());
				if (z.SecurityZone == System.Security.SecurityZone.Intranet 
				|| z.SecurityZone == System.Security.SecurityZone.MyComputer)
				{
					return true;
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Uploads files from the upload staging folder to the specified FTP Server
		/// </summary>
		/// <param name="request">The export request object</param>
		public void UploadFiles(ExportRequestClass request)
		{
			try
			{
				if (this.DebugMode)
				{
					this.logger.LogInfo(request.SendingCompanyCode + " FTP thread started.");
				}

                string assemblyPath = FMConvert.GetAssemblyDirectory();

				// Check if the export directory exists
                var dir = new DirectoryInfo(assemblyPath + request.UploadStagingFolder);
				if (!dir.Exists)
				{
					this.logger.LogWarning("Staging Folder: " + dir.FullName + " does not exist", 1005);
					return;
				}

				// Get the files
				FileInfo[] files = dir.GetFiles("*.*");
				foreach (FileInfo fi in files)
				{
					// Check if File is accessible
					try
					{
						FileStream stream = File.Open(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.None);
						stream.Close();
					}
					catch (IOException ex)
					{
						this.logger.LogWarning(ex.ToString(), 1008);
						continue;
					}

					// Upload and archive the file
					if (this.Upload(fi.FullName))
					{
						this.logger.LogInfo(fi.Name + " was successfully sent to FTP Server (" + this.Server + ") at " + DateTime.UtcNow.ToString("r"), 777);

						fi.Delete();
					}
				}
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex.ToString(), 1006);

				if (this.DebugMode)
				{
					this.logger.LogError("StackTrace: \n" + ex.StackTrace, 1007);
				}
			}

			if (this.DebugMode)
			{
				this.logger.LogInfo(request.SendingCompanyCode + " FTP thread stopped.", 1008);
			}
		}
	}
}
