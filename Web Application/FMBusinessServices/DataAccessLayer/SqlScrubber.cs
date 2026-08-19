namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;

	/// <summary>
    /// Class for containing functions for sanitizing SQL
    /// </summary>
    internal class SqlScrubber
    {
		public static bool IsSqlOk(SqlCommand cmd)
		{
			return IsSqlOk(cmd.CommandText) && AreParametersOk(cmd);
		}

		private static bool AreParametersOk(SqlCommand cmd)
		{
            if(cmd == null)
            {
                return false;
            }
			foreach (SqlParameter parameter in cmd.Parameters)
			{
				if (parameter.DbType == DbType.String
					&& parameter.Value != null 
					&& IsParameterOk( parameter.Value.ToString() ) == false )
				{
					return false;
				}
			}

			return true;
		}

		private static bool IsParameterOk(string sql)
		{
			if(sql == null)
            {
                return true;
            }
			if ( sql.ToLower().Contains( "<script" ) || sql.ToLower().Contains( "</script>" ) )
			{
				return false;
			}
			
			return true;
		}

		/// <summary>
        /// Tests if SQL string is safe to execute
        /// </summary>
        /// <param name="sql">SQL text to test</param>
        /// <returns>
        /// true:  sql contains no dangerous sql constructs
        /// false:  sql is potentially dangerous injection
        /// </returns>
        /// <remarks>
        /// Sql is currently considered dangerous if it has:
        ///     At least on semicolon
        ///     an odd number of single quotes
        ///     a comment marker outside of a delimited string
        /// </remarks>
        public static bool IsSqlOk(string sql)
        {
            sql = sql.Trim();

            if(sql == null)
            {
                return false;
            }
            // Start by checking for semicolons but allow to be terminated by semi-colons
            if (sql.Contains(';') && !sql.EndsWith(";"))
            {
                return false;
            }

				if (sql.ToLower().Contains("<script") || sql.ToLower().Contains("</script>"))
				{
					 return false;
				}
            // Now, split sql into substrings delimited by single quotes.
            // Append a space first because the logic below might not work right
            // if the last character is a single quote.
            String[] sqlPieces = (sql + " ").Split(new char[] { '\'' }, StringSplitOptions.None);

            // Number of pieces  will equal the number of single quotes + 1.
            // Therefore, an even number of pieces means an odd number of single quotes
            if ((sqlPieces.Length % 2) == 0)
            {
                return false;
            }

            // Now, check pieces for comment delimiters
            // check all pieces for multi-line comment markers
            // only check every other (0,2,4,...) for single line marker (--)
            for (int index = 0; index < sqlPieces.Length; index++)
            {
                if (sqlPieces[index].Contains("/*"))
                {
                    return false;
                }

                if (sqlPieces[index].Contains("*/"))
                {
                    return false;
                }

                if (((index % 2) == 0) && (sqlPieces[index].Contains("--")))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
