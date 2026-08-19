using System;

using FM7Accounting;
using FMCommon;

using XMLImport;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for ImportProcessor.
	/// </summary>
	public class ImportProcessor
	{
		#region Attributes
		AccountingSecurity accountingSecurity;
		#endregion Attributes

		public ImportProcessor()
		{
		}

		public ImportValidationResults Import(AccountingSecurity accountingSecurity,
			ImportFilter filter, string site, System.IO.Stream stream)
		{
			this.accountingSecurity = accountingSecurity;
			XMLImportProcessor importer = new XMLImportProcessor();
			return importer.Import(accountingSecurity, /*filter,*/ site, stream, filter);
		}
	}
}
