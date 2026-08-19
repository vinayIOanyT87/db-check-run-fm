// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileHanderBase.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FileHanderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.FileHanders
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// The file hander base.
	/// </summary>
	public abstract class FileHanderBase
	{
		#region Protected Attributes
		protected SecurityClass Security;
		protected string directory;
		protected string fileName;
		protected string filePrefix;
		protected BinaryReader reader;
		protected BinaryWriter writer;
		protected bool allowSettingofAbosulteFilePath;
		protected EventLogging eventLogging;
		protected enum FILE_ACCESS { READ, WRITE, NONE };
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FileHanderBase"/> class.
		/// </summary>
		/// <param name="inSecurity">
		/// The Security.
		/// </param>
		protected FileHanderBase(SecurityClass inSecurity)
		{
			this.Security = inSecurity;
			this.Initialize( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether allow setting of absolute file path.
		/// </summary>
		public bool AllowSettingofAbsoluteFilePath
		{
			get
			{
				return this.allowSettingofAbosulteFilePath;
			}

			set
			{
				this.allowSettingofAbosulteFilePath = value;
			}
		}

		/// <summary>
		/// Gets or sets the file path.
		/// </summary>
		public string FilePath
		{
			get
			{
				return this.directory + "\\" + this.fileName;
			}

			set
			{
				if ( this.allowSettingofAbosulteFilePath )
				{
					int index = value.LastIndexOf("\\", StringComparison.Ordinal);
					this.fileName = value.Substring(index + 1);
					this.directory = value.Substring(0, index);
				}
			}
		}

		/// <summary>
		/// Gets or sets the file name.
		/// </summary>
		/// <exception cref="Exception">Invalid file name exception.
		/// </exception>
		public string FileName
		{
			get
			{
				return this.fileName;
			}

			set
			{
				if ( value.IndexOfAny("\\|/".ToCharArray( )) != -1 )
				{
					throw new Exception("Invalid File Name");
				}

				this.fileName = value.TrimEnd("\\".ToCharArray( ));
			}
		}

		/// <summary>
		/// Gets or sets the file prefix.
		/// </summary>
		/// <exception cref="Exception">Invalid prefix exception.
		/// </exception>
		public string FilePrefix
		{
			get
			{
				return this.filePrefix;
			}

			set
			{
				if ( value.IndexOfAny("\\|/".ToCharArray( )) != -1 )
				{
					throw new Exception("Invalid File Prefix");
				}

				this.filePrefix = value.TrimEnd("\\".ToCharArray( ));
				this.fileName = this.filePrefix + this.fileName;
			}
		}

		/// <summary>
		/// Gets or sets the directory.
		/// </summary>
		public string Directory
		{
			get { return this.directory; }
			set { this.directory = value; }
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// The save.
		/// </summary>
		/// <param name="document">
		/// The document.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public abstract bool Save(string document);

		/// <summary>
		/// The save.
		/// </summary>
		/// <param name="stream">
		/// The stream.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public abstract bool Save(byte[] stream);

		/// <summary>
		/// The set directory.
		/// </summary>
		protected abstract void SetDirectory( );

		/// <summary>
		/// The create file name.
		/// </summary>
		protected abstract void CreateFileName( );
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the File Handler Base object to its initial
		/// state.
		/// </summary>
		private void Initialize( )
		{
			this.directory		= ".\\";
			this.fileName		= string.Empty;
			this.reader			= null;
			this.writer			= null;
			this.eventLogging	= new EventLogging( );
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// This method close a specified file.
		/// </summary>
		protected void CloseFile( )
		{
			if ( this.reader != null )
			{
				this.reader.Close( );
				this.reader = null;
			}

			if ( this.writer != null )
			{
				this.writer.Close( );
				this.writer = null;
			}
		}

		/// <summary>
		/// This method opens a specified file with a given access. It will throw an error if it
		/// cannot.
		/// </summary>
		/// <param name="access">
		/// The access.
		/// </param>
		/// <exception cref="Exception">File access exception.
		/// </exception>
		protected void OpenFile(FILE_ACCESS access)
		{
			if ( access == FILE_ACCESS.NONE )
			{
				this.eventLogging.LogEvent("FileHandlerBase.OpenFile: Wrong file type NONE.", EventLogEntryType.Error);
				throw new Exception("File Access of NONE not supported for this call.");
			}

			try
			{
				if ( this.reader != null )
				{
					this.reader.Close( );
					this.reader = null;
				}

				if ( this.writer != null )
				{
					this.writer.Close( );
					this.writer = null;
				}

				if ( access == FILE_ACCESS.READ )
				{
					this.reader = new BinaryReader(File.Open(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.None));
				}
				else
				{
					this.writer = new BinaryWriter(File.Open(this.FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
				}
			}
			catch ( Exception e )
			{
				this.eventLogging.LogEvent("FileHandlerBase.OpenFile: " + e.Message, EventLogEntryType.Error);
				throw new Exception("Unable to open file " + this.FilePath + " for reading\nReason: " + e.Message);
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method reads a specified text file. It will throw an error if it
		/// cannot.
		/// </summary>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		/// <exception cref="Exception">Read file exception.
		/// </exception>
		public string ReadTextFile( )
		{
			byte[] byteresult;
			string stringresult;

			try
			{
				this.OpenFile(FILE_ACCESS.READ);
				byteresult = this.reader.ReadBytes((int) this.reader.BaseStream.Length);
				stringresult = ASCIIEncoding.ASCII.GetString(byteresult, 0, byteresult.Length);

				this.reader.Close( );
				this.reader = null;
			}
			catch ( Exception e )
			{
				this.eventLogging.LogEvent("FileHandlerBase.ReadTextFile: Cannot read file " + this.FilePath +
													 ". Reason: " + e.Message, EventLogEntryType.Error);
				throw new Exception("Unable to read file " + this.FilePath + "\nReason: " + e.Message);
			}
			finally
			{
				if ( this.reader != null )
				{
					this.reader.Close( );
				}

				this.reader = null;
			}

			return stringresult;
		}

		/// <summary>
		/// This method reads a specified binary file. It will throw an error if it
		/// cannot.
		/// </summary>
		/// <returns>
		/// The <see cref="byte"/>.
		/// </returns>
		/// <exception cref="Exception">Read access exception.
		/// </exception>
		public byte[] ReadBinaryFile( )
		{
			byte[] byteresult;

			try
			{
				this.OpenFile(FILE_ACCESS.READ);
				byteresult = this.reader.ReadBytes((int) this.reader.BaseStream.Length);
				this.reader.Close( );
				this.reader = null;
			}
			catch ( Exception e )
			{
				this.eventLogging.LogEvent("FileHandlerBase.ReadBinaryFile: Unable to read file " + this.FilePath +
													 ". Reason: " + e.Message, EventLogEntryType.Error);
				throw new Exception("Unable to read file " + this.FilePath + "\nReason: " + e.Message);
			}
			finally
			{
				if ( this.reader != null )
				{
					this.reader.Close( );
				}

				this.reader = null;
			}

			return byteresult;
		}

		/// <summary>
		/// This method deletes the specified file. It will throw an error if it
		/// cannot.
		/// </summary>
		public void DeleteFile( )
		{
			try
			{
				this.CloseFile( );

				if ( File.Exists(this.FilePath) )
				{
					File.Delete(this.FilePath);
				}
			}
			catch ( Exception e )
			{
				this.eventLogging.LogEvent("FileHandlerBase.DeleteFile: Unable to delete file " + this.FilePath +
													 ". Reason: " + e.Message, EventLogEntryType.Error);
				throw new Exception("Unable to delete file.\nReason: " + e.Message);
			}
		}
		#endregion
	}
}
