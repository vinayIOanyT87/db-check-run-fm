// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataResultClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DataResultClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// The data result class.
	/// </summary>
	[Serializable]
	public class DataResultClass
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataResultClass"/> class.
		/// </summary>
		public DataResultClass()
		{
			this.Xml = new SortedList<string, string>();
			this.TransCount = new SortedList<string, int>();
			this.LargestRowVersion = 0;
			this.TotalNumberOfRecords = 0;
			this.UseRawResultFileName = false;
		}

		/// <summary>
		/// Gets or sets a value indicating whether use raw result file name.
		/// </summary>
		public bool UseRawResultFileName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the largest row version.
		/// </summary>
		public long LargestRowVersion
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the xml.
		/// </summary>
		public SortedList<string, string> Xml
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the trans count.
		/// </summary>
		public SortedList<string, int> TransCount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the total number of records.
		/// </summary>
		public int TotalNumberOfRecords
		{
			get;
			set;
		}

		/// <summary>
		/// Gets transaction count value associated with the specified key.
		/// </summary>
		/// <param name="key"> The key </param>
		/// <returns> The transaction count value </returns>
		public int GetTransCountValue(string key)
		{
			return (!this.TransCount.ContainsKey(key)) ? 0 : this.TransCount[key];
		}
	}
}
