// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Common.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Common type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMCore
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;

	/// <summary>
	/// Class for consolidating code extensions.
	/// </summary>
	public class Common
	{
		/// <summary>
		/// Takes a collection of values and creates SqlParameters and a comma-delimited
		/// list for use in a SQL IN clause.
		/// </summary>
		/// <param name="paramCollection">Collection to which the new parameters will be
		/// appended</param>
		/// <param name="paramValues">The values for the parameters</param>
		/// <param name="baseParamName">The base name to use, e.g., "@Prm" for this
		/// would cause parameters to be created with names "@Prm1", "@Prm2", etc.</param>
		/// <param name="sqlDbType">Type of the parameters</param>
		/// <returns>Comma-delimited list of the parameters, e.g., "@Prm1, @Prm2, @Prm3"</returns>
		public static string ConstructSqlParametersFromCollection(SqlParameterCollection paramCollection, IEnumerable paramValues, string baseParamName, SqlDbType sqlDbType)
		{
			return ConstructSqlParametersFromCollection(paramCollection, paramValues, baseParamName, sqlDbType, -1);
		}

		/// <summary>
		/// Takes a collection of values and creates SqlParameters and a comma-delimited
		/// list for use in a SQL IN clause.
		/// </summary>
		/// <param name="paramCollection">Collection to which the new parameters will be
		/// appended</param>
		/// <param name="paramValues">The values for the parameters</param>
		/// <param name="baseParamName">The base name to use, e.g., "@Prm" for this
		/// would cause parameters to be created with names "@Prm1", "@Prm2", etc.</param>
		/// <param name="sqlDbType">Type of the parameters</param>
		/// <param name="size">Length of the parameters</param>
		/// <returns>Comma-delimited list of the parameters, e.g., "@Prm1, @Prm2, @Prm3"</returns>
		public static string ConstructSqlParametersFromCollection(SqlParameterCollection paramCollection, IEnumerable paramValues, string baseParamName, SqlDbType sqlDbType, int size)
		{
			System.Text.StringBuilder paramList = new System.Text.StringBuilder();
			int paramNumber = 0;

			if (!baseParamName.StartsWith( "@" ))
			{
				baseParamName = "@" + baseParamName;
			}

			foreach (object value in paramValues)
			{
				paramNumber++;
				string paramName = baseParamName + paramNumber.ToString(CultureInfo.InvariantCulture);

				paramList.Append(paramName + ",");

				SqlParameter param;
				if (size > 0)
				{
					param = paramCollection.Add( paramName, sqlDbType, size );
				}
				else
				{
					param = paramCollection.Add(paramName, sqlDbType);
				}

				if (value == null)
				{
					param.Value = DBNull.Value;
				}
				else
				{
					param.Value = value;
				}
			}

			if (paramList.Length > 0)
			{
				return paramList.ToString().TrimEnd( ',' );
			}
			
			return string.Empty;
		}

		/// <summary>
		/// This method will remove percent signs and escape any and all "'". It will return the 
		/// modified string or the if no change, then the original string.
		/// </summary>
		/// <param name="inStr">
		/// The base string object.
		/// </param>
		/// <returns>
		/// The escape like clause characters.
		/// </returns>
		public static string EscapeLikeClauseCharacters( string inStr )
		{
			string outStr = inStr;

			if (outStr.IndexOf('%') >= 0)
			{
				outStr = outStr.Replace("%", string.Empty);
			}

			if (outStr.IndexOf('\'') >= 0)
			{
				outStr = outStr.Replace("'", "''");
			}

			return outStr;
		}

		/// <summary>
		/// generic method to check for null parameter
		/// </summary>
		/// <typeparam name="TObjectType">The type of the object being extended.</typeparam>
		/// <param name="parameter">The base object for the extension.</param>
		public static void ValidateObject<TObjectType>( TObjectType parameter ) where TObjectType : class
		{
			if (parameter == null)
			{
				throw new ArgumentException("Parameter cannot be null.");
			}
		}
	}
}
