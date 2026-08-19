// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceDataDictionaryKeys.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This class is used to pull out all the column names and create data dictionary keys from the database
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Reflection;
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// This class is used to pull out all the column names and create data dictionary keys
	/// </summary>
	[ServiceBehavior]
	public class ServiceDataDictionaryKeys : IServiceDataDictionaryKeys
	{
		#region Private data members
		/// <summary>
		/// The consolidatedDA is the DAL component to query the database.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDataDictionaryKeys"/> class.
		/// </summary>
		public ServiceDataDictionaryKeys()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion
		/// <summary>
		/// Gets the data dictionary keys representing the table column names in the database.
		/// </summary>
		/// <param name="security">The security token for logged in user.</param>
		/// <returns> string array of data dictionary keys</returns>
		/// <exception cref="System.ArgumentNullException">Security token was null</exception>
		/// <exception cref="System.Exception">Error retrieving Column Name Data Dictionary Keys:  + ex.Message</exception>
		public string[] GetKeys(SecurityClass security)
		{
			if (security == null)
			{
				const string SecurityStr = "Security";
				throw new ArgumentNullException(SecurityStr);
			}

			var ret = this.GetAssemblyKeys(security);

			try
			{
				using (var cmd = new SqlCommand())
				{
					cmd.CommandText = "select Value from vw_DatabaseDictionaryKeyValue";
					var dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables.Count > 0))
					{
						var dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							ret.AddRange(from DataRow row in dataTable.Rows select DataObject.getValue(row["Value"], string.Empty));
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving Column Name Data Dictionary Keys: " + ex.Message);
			}

			return ret.ToArray();
		}

		/// <summary>
		/// This method retrieves all the dictionary keys from all the assemblies and searches for any value
		///    change from the database. It then builds a table of keys and values for the criterion.  The criterion
		///    can be a letter or the search string.
		/// </summary>
		/// <param name="security">
		/// security context for call
		/// </param>
		/// <returns>
		/// The dictionary keys.
		/// </returns>
		private List<string> GetAssemblyKeys(SecurityClass security)
		{
			var dll = Assembly.GetExecutingAssembly();
			var types = dll.GetTypes();
			var ret = new List<string>();

			// Determine if the search string contains a value, if so, then
			// we want to use the search string as a criterion and not a letter.
			foreach (var keys in (from module in types where module.IsClass let dataDictionaryInterface = module.GetInterface("FMBusinessObjects.DataObjects.IDataDictionary") where dataDictionaryInterface != null select Activator.CreateInstance(module) into engine select engine).OfType<IDataDictionary>().Select(dataDictionary => dataDictionary.Keys(security)).Where(keys => keys != null))
			{
				ret.AddRange(keys);
			}

			return ret;
		}
	}
}