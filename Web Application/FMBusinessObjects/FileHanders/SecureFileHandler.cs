// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecureFileHandler.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SecureFileHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.FileHanders
{
	using System;
	using System.Diagnostics;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The secure file handler.
	/// </summary>
	public class SecureFileHandler : FileHanderBase
	{
		#region Private Members
		byte[] secureSessionKey;
		byte[] secureData;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SecureFileHandler"/> class.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		public SecureFileHandler(SecurityClass inSecurity) : base(inSecurity)
		{
			this.Initialize( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the secure session key.
		/// </summary>
		public byte[] SecureSessionKey
		{
			get { return this.secureSessionKey; }
			set { this.secureSessionKey = value; }
		}

		/// <summary>
		/// Gets or sets the secure data.
		/// </summary>
		public byte[] SecureData
		{
			get { return this.secureData; }
			set { this.secureData = value; }
		}
		#endregion

		#region Override Methods

		#region Public Overrides
		/// <summary>
		/// This method saves a string to the specified file.
		/// </summary>
		/// <param name="document">
		/// The document.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public override bool Save(string document)
		{
			if ( this.fileName == string.Empty )
			{
				return false;
			}

			this.OpenFile(FILE_ACCESS.WRITE);
			this.writer.Write(document.ToCharArray( ));
			this.CloseFile( );

			return true;
		}

		/// <summary>
		/// This method saves a byte stream to the specified file.
		/// </summary>
		/// <param name="stream">
		/// The stream.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public override bool Save(byte[ ] stream)
		{
			if ( this.fileName == string.Empty )
			{
				return false;
			}

			this.OpenFile(FILE_ACCESS.WRITE);
			this.writer.Write(stream, 0, stream.Length);
			this.CloseFile( );

			return true;
		}

		#endregion

		#region Protected Overrides
		/// <summary>
		/// This method sets the directory from the retrieved information in the registry.
		/// If there is not registry setting or the registry is not found, then the default
		/// is set to ".\\".
		/// </summary>
		protected override void SetDirectory( )
		{
			try
			{
				const string EnterpriseFilePathSecure = "Enterprise_FilePathSecure";

				ConfigurationSettingDOClass configSetting =
					FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, EnterpriseFilePathSecure));

				this.directory = configSetting.SettingValue.TrimEnd("\\".ToCharArray( ));
			}
			catch
			{
				this.eventLogging.LogEvent(
					"SecureFileHandler.SetDirectory: Could not find path registry entries, setting to default.",
					EventLogEntryType.Information);
				this.directory = ".";
			}
		}

		/// <summary>
		/// This method will create a secure file name.
		/// </summary>
		protected override void CreateFileName( )
		{
			this.fileName = this.filePrefix + "_Enterprise_" +
								 DateTime.UtcNow.ToString("yyyyMMdd_hhmmss") +
								 ".vcef";
		}
		#endregion

		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the Secure File Handler class to its initial state.
		/// </summary>
		private void Initialize( )
		{
			this.SetDirectory( );
			this.CreateFileName( );
		}
		#endregion

		#region Public Members

		/// <summary>
		/// This method saves the accounting, TAV, and symmetric key as a package. It wraps all the
		/// data and encrypts it asymmetric with a public key.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		/// <exception cref="Exception">Save package exception.
		/// </exception>
		public bool SavePackage( )
		{
			if ( this.secureSessionKey.Length == 0 )
			{
				this.eventLogging.LogEvent("SecureFileHandler.SavePackage: Secure key not provided", EventLogEntryType.Error);
				throw new Exception("Unable to save secure file package.  Session key not provided");
			}

			if ( this.secureData.Length == 0 )
			{
				this.eventLogging.LogEvent("SecureFileHandler.SavePackage: Data not provided", EventLogEntryType.Error);
				throw new Exception("Unable to save secure file package.  Data not provided");
			}

			try
			{
				this.eventLogging.LogEvent(
					"SecureFileHandler.SavePackage: Start parsing stream and saving.", EventLogEntryType.Information);

				byte[] buffer =
					{
						0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
						0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
					};

				this.OpenFile(FILE_ACCESS.WRITE);
				int headerlen = 200;

				// Write position of first byte of secureSessionKey
				writer.Write(0);
				headerlen -= sizeof(int);

				// Write length of secureSessionKey
				writer.Write(this.secureSessionKey.Length);
				headerlen -= sizeof(int);

				// Write postion of first byte in secure data
				writer.Write(this.secureSessionKey.Length + buffer.Length);
				headerlen -= sizeof(int);

				//Write length of secure data
				writer.Write(secureData.Length);
				headerlen -= sizeof(int);

				// Write remaining bytes of header record.
				for (int i = 0; i < headerlen; i++)
				{
					writer.Write((byte)0);
				}

				// Write secureSessionKey
				writer.Write(this.secureSessionKey, 0, this.secureSessionKey.Length);

				// Write buffer
				writer.Write(buffer, 0, buffer.Length);

				// Write SecureData
				writer.Write(this.secureData, 0, this.secureData.Length);

				// Write buffer
				writer.Write(buffer, 0, buffer.Length);

				writer.Close();
				return true;
			}
			catch ( Exception e )
			{
				this.eventLogging.LogEvent("SecureFileHandler.SavePackage: " + e.Message, EventLogEntryType.Error);
				throw new Exception("Unable to save secure file package.\nReason: " + e.Message);
			}
		}

		/// <summary>
		/// This method will read a package stream, parse it into components.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		/// <exception cref="Exception">Read package exception.
		/// </exception>
		public bool ReadPackage( )
		{
			try
			{
				this.eventLogging.LogEvent("SecureFileHandler.ReadPackage: Start reading package.", EventLogEntryType.Information);

				this.OpenFile(FILE_ACCESS.READ);
				int headerlen = 200;
				var buffer = new byte[30];

				// Eric Simmons - 06-11-2007
				// Determine if file has been opened by a text editor like
				// Microsoft Notepad.  Most of the bytes in the header should be
				// 0x00 but if more than 30 bytes are equal to 0x20 then the
				// file has been opened and corrupted by Microsoft Notepad.
				byte[] headerbuffer = reader.ReadBytes(200);
				int numberof20Hex = 0;

				for ( int i = 0; i < headerbuffer.Length; i++ )
				{
					if ( 0x20 == headerbuffer[i] )
					{
						numberof20Hex++;
					}
				}

				if ( numberof20Hex >= 30 )
				{
					this.eventLogging.LogEvent("SecureFileHandler.ReadPackage: Unable to read package", EventLogEntryType.Error);
					throw new Exception("Unable to read package.  The file may be corrupt by a user opening and resaving the file in Microsoft Notepad.");
				}


				reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);

				// reader position of first byte of secureSessionKey
				int sessionKeyPos = this.reader.ReadInt32( );

				if ( sessionKeyPos != 0 )
				{
					this.eventLogging.LogEvent("SecureFileHandler.ReadPackage: Unable to read package", EventLogEntryType.Error);
					throw new Exception("Unable to read package.  The file may be corrupt");
				}

				headerlen -= sizeof(int);

				// read length of secureSessionKey
				int sessionKeyLen = this.reader.ReadInt32( );
				headerlen -= sizeof(int);

				// read postion of first byte in secure data
				this.reader.ReadInt32( );
				headerlen -= sizeof(int);

				// read length of secure data
				int dataLen = this.reader.ReadInt32( );
				headerlen -= sizeof(int);
				reader.ReadBytes(headerlen);

				// read secureSessionKey
				this.secureSessionKey = reader.ReadBytes(sessionKeyLen);

				// read Buffer
				buffer = reader.ReadBytes(buffer.Length);

				// read Secure Data
				this.secureData = reader.ReadBytes(dataLen);

				// read Buffer
				buffer = reader.ReadBytes(buffer.Length);
				reader.Close( );

				return true;
			}
			catch ( Exception e )
			{
				if ( this.reader != null )
				{
					reader.Close( );
				}

				this.eventLogging.LogEvent("SecureFileHandler.ReadPackage: " + e.Message, EventLogEntryType.Error);
				throw new Exception("Unable to read secure file package.\nReason: " + e.Message);
			}
		}
		#endregion
	}
}
