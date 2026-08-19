// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlacklistImportFileReader.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	This class is the standard black list import file reader which can be initialized with any parsing class that implements the 
//	IBlacklistImportFileParser interface.  The parsing class will be given a reference to a IBlacklistImportRecordHandler callback method.
//	The parsing library should call the callback method for each import record that could be mapped to a blacklist import record.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.UtilityObjects
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

	/// <summary>
	/// Class BlacklistImportFileReader.
	/// </summary>
	public class BlacklistImportFileReader : IBlacklistImportRecordHandler
	{
		/// <summary>
		/// The blacklist import file parser
		/// </summary>
		private readonly IBlacklistImportFileParser blacklistImportFileParser = null;

		/// <summary>
		/// The blacklist import record collection
		/// </summary>
		public List<GasboyBlacklistImportRecord> BlacklistImportRecords { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BlacklistImportFileReader" /> class.
		/// </summary>
		/// <param name="parser">The parser.</param>
		public BlacklistImportFileReader(IBlacklistImportFileParser parser)
		{
			this.blacklistImportFileParser = parser;
			this.BlacklistImportRecords = new List<GasboyBlacklistImportRecord>();
		}

		/// <summary>
		/// Parses the import file.
		/// </summary>
		/// <param name="importFile">The import file.</param>
		/// <returns>The number of items that were parsed.</returns>
		/// <exception cref="System.ArgumentNullException">importFile;@Import file must be specified.</exception>
		public int ParseImportFile(string importFile)
		{
			if (string.IsNullOrEmpty(importFile))
			{
				throw new ArgumentNullException("importFile", @"Import file must be specified.");
			}

			if (null != this.blacklistImportFileParser)
			{
				this.blacklistImportFileParser.ParseImportFile(importFile, this.ProcessImportRecord);
			}

			return this.BlacklistImportRecords.Count;
		}

		/// <summary>
		/// Processes the import record.
		/// </summary>
		/// <param name="importRecord">The import record.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void ProcessImportRecord(GasboyBlacklistImportRecord importRecord)
		{
			this.BlacklistImportRecords.Add(importRecord);
		}
	}
}
