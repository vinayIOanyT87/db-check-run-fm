// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountFileHandler.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AccountFileHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.FileHanders
{
	using System;
	using System.Diagnostics;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using Microsoft.Win32;

	/// <summary>
	/// The account file handler.
	/// </summary>
	public class AccountFileHandler : FileHanderBase
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AccountFileHandler"/> class.
		/// </summary>
		/// <param name="inSecurity">
		/// The in security.
		/// </param>
		public AccountFileHandler(SecurityClass inSecurity)
			: base(inSecurity)
		{
			this.Initialize( );
		}
		#endregion

		#region Override Methods

		#region Public Overrides
		/// <summary>
		/// This method saves a string document to the specified file.
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

			this.OpenFile(FILE_ACCESS.WRITE);
			this.SetDirectory( );
			this.writer.Write(stream);
			this.CloseFile( );

			return true;
		}
		#endregion

		#region Protected Overrides
		/// <summary>
		/// This method will set the directory by retrieving the settings in the
		/// registry. If an error occurs, the default directory is set to ".\\".
		/// </summary>
		protected override void SetDirectory( )
		{
			try
			{
				const string EnterpriseFilePathAccounting = "Enterprise_FilePathAccounting";

				ConfigurationSettingDOClass configSetting =
					FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, EnterpriseFilePathAccounting));


				this.directory = configSetting.SettingValue.TrimEnd("\\".ToCharArray( ));
			}
			catch
			{
				this.eventLogging.LogEvent(
					"AccountFileHandler.SetDirectory: Could not find path registry entries, setting to default.",
					EventLogEntryType.Information);
				this.directory = ".\\";
			}
		}

		/// <summary>
		/// This method will create the file name per the standards spelled out in the
		/// requirements for accounting data on the server express.
		/// </summary>
		protected override void CreateFileName( )
		{
			this.fileName = this.filePrefix + "_Accounting_" +
								 DateTime.Now.ToString("yyyyMMdd_HHmmss") +
								 ".txt";
		}
		#endregion

		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the accounting File Handler objects to its
		/// initial state.
		/// </summary>
		private void Initialize( )
		{
			this.SetDirectory( );
			this.CreateFileName( );
		}
		#endregion
	}
}
