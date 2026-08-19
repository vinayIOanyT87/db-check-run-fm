///***************************************************************************
/// Module Name:	HelpMappings
/// Author:			Andy Hush
/// Copyright (c) Varec, Inc. All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using System.Data.SqlClient;
using System.Data;

namespace FMBusinessServices.ServiceClasses
{
	/// <summary>
	/// Service class to allow enumeration of HelpMappingClass objects
	/// from tblHelpMapping
	/// </summary>
	public class HelpMappingsClass : IHelpMappings
	{
		#region Internal Fields

		/// <summary>
		/// For database access
		/// </summary>
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		public HelpMappingsClass()
		{
		}

		/// <summary>
		/// Retrieve a Dictionary of all the help mappings
		/// </summary>
		/// <param name="security">Security object</param>
		/// <returns>Dictionary of help mappings</returns>
		public HelpMappingDictionary GetDictionary(SecurityClass security)
		{
			if (security == null)
				throw new ArgumentNullException("security");

			var helpMapping = new HelpMappingClass();
			using (SqlCommand cmd = new SqlCommand())
			{
				helpMapping.EnumerateSQL(cmd, ContextUtil.IsInTransaction);

				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);
				var helpMappingDictionary = new HelpMappingDictionary();

				DataTable table = set.Tables[0];
				foreach (DataRow row in table.Rows)
				{
					helpMapping = new HelpMappingClass();
					helpMapping.Load(row);
					if (!helpMappingDictionary.ContainsKey(helpMapping.HelpContextKey))
					{
						helpMappingDictionary.Add(helpMapping.HelpContextKey, helpMapping.HelpPage);
					}
				}

				return helpMappingDictionary;
			}
		}
	}
}