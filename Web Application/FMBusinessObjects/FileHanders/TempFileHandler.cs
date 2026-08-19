// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TempFileHandler.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TempFileHandler type.
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
	/// The temp file handler.
	/// </summary>
	public class TempFileHandler : FileHanderBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TempFileHandler"/> class.
		/// </summary>
		/// <param name="inSecurity">
		/// The Security.
		/// </param>
		public TempFileHandler(SecurityClass inSecurity) : base(inSecurity)
		{
			this.Initialize( );
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
			this.SetDirectory( );
			writer.Write(document.ToCharArray( ));
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
		public override bool Save(byte[] stream)
		{
			if ( this.fileName == string.Empty )
			{
				return false;
			}

			this.SetDirectory();
			this.OpenFile(FILE_ACCESS.WRITE);
			writer.Write(stream);
			this.CloseFile( );

			return true;
		}
		#endregion

		#region Protected Overrides
		/// <summary>
		/// This method sets the directory path by retrieving it from the registry. If it cannot
		/// find the path, then it sets the directory to ".\\".
		/// </summary>
		protected override void SetDirectory( )
		{
			// Already has value.
			if ( string.IsNullOrEmpty(this.directory) == false )
			{
				return;
			}

			try
			{
				const string EnterpriseFilePathsTempFilePath = "Enterprise_FilePathsTempFilePath";

				ConfigurationSettingDOClass configSetting =
					FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, EnterpriseFilePathsTempFilePath));

				this.directory = configSetting.SettingValue.TrimEnd("\\".ToCharArray( ));
			}
			catch
			{
				this.eventLogging.LogEvent(
					"TempFileHandler.SetDirectory: Could not find path registry entries, setting to default.",
					EventLogEntryType.Information);
				this.directory = ".\\";
			}
		}

		/// <summary>
		/// This method implements the CreateFileName method.
		/// </summary>
		protected override void CreateFileName( )
		{
			this.fileName = Guid.NewGuid( ).ToString("N") + ".tmp";
		}
		#endregion

		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the Temp File Handler object to its initial state.
		/// </summary>
		private void Initialize( )
		{
			this.SetDirectory( );
			this.CreateFileName( );
		}
		#endregion
	}


}
