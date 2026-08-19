namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Text;

	using Crypt;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FMBusinessServices.DataAccessLayer;

	using FMCore;


	/// <summary>
	/// Class for FuelsManager Defense Password generation and service account names
	/// </summary>
	/// <remarks>
	/// This class is used to generate the passwords used by FuelsManager Defense
	/// Currently, it only generates the Database Password based on the database id
	/// Future expansion will includ the application Password encryption as well
	/// </remarks>
	// ReSharper disable once InconsistentNaming
	public sealed class DBAccess : IDBAccess
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		#region Attributes
		// ReSharper disable once InconsistentNaming
		private const string serviceLogin = "FMDService";
		private const string adminLogin = "administrator|SiteAdmin";
		private const string SaLogin = "sa";
		  #endregion

		#region Constructor
		/// <summary>
		/// Constructor.  Currently does nothing
		/// </summary>
		/// <remarks>
		/// Constructor is currently private as this class should not be
		/// instantiated.  Only static methods are used.  If this changes,
		/// make the constructor public
		/// </remarks>
		private DBAccess()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Standard, limited access account used for validating user
		/// passwords or conducting other operations where a user context is
		/// not available.  This should never be used if it can be determined
		/// which user is performing the access.
		/// </summary>
		internal static string ServiceLoginAccess => serviceLogin;

		  public string ServiceLogin ( SecurityClass security )
		{
			return ServiceLoginAccess;
		}

		/// <summary>
		/// Privileged account, the usage of which should be limited to tasks requiring
		/// a higher level of access, such as audit log reporting.
		/// </summary>
		public static string AdminLogin => adminLogin;

		  /// <summary>
		/// Name of the SA account for SQL Server.  To be used only in the most rare of cases.
		/// </summary>
		public static string SALogin => SaLogin;

		  #endregion

		#region Members

		public string GetDBPassword ( string unmangledPassword )
		{
			return GetDBPasswordAccess( unmangledPassword );
		}

		  /// <summary>
		  /// generates a FuelsManager Defense database Password for a user id
		  /// </summary>
		  /// <returns>Generated database Password</returns>
		  static internal string GetDBPasswordAccess( string unmangledPassword )
		{
			// Algorithm is to take a SHA-1 hash of the bytes of the ASCII representation
			// of the user ID followed by the bytes of the salt "{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}"
			ASCIIEncoding encoding = new ASCIIEncoding();

			// Split out for obfuscation purposes
			// Probably something more thorough required later

			//Eric Simmons
			//08-10-2007
			//Updated to ensure that UserID is always uppercase.
			//resolves CSI #5049
			StringBuilder newData = new StringBuilder( unmangledPassword.ToUpper() );
			newData.Append( '{' );
			newData.Append( '0' );
			newData.Append( '1' );
			newData.Append( 'A' );
			newData.Append( 'F' );
			newData.Append( 'E' );
			newData.Append( 'B' );
			newData.Append( 'D' );
			newData.Append( '3' );
			newData.Append( '-' );
			newData.Append( '7' );
			newData.Append( '8' );
			newData.Append( 'C' );
			newData.Append( 'D' );
			newData.Append( '-' );
			newData.Append( '4' );
			newData.Append( 'B' );
			newData.Append( '1' );
			newData.Append( '5' );
			newData.Append( '-' );
			newData.Append( 'A' );
			newData.Append( 'B' );
			newData.Append( '9' );
			newData.Append( 'B' );
			newData.Append( '-' );
			newData.Append( 'F' );
			newData.Append( '4' );
			newData.Append( 'A' );
			newData.Append( 'A' );
			newData.Append( '1' );
			newData.Append( 'C' );
			newData.Append( '0' );
			newData.Append( 'E' );
			newData.Append( '2' );
			newData.Append( 'D' );
			newData.Append( '9' );
			newData.Append( 'B' );
			newData.Append( '}' );
			byte[] userIDBytes = encoding.GetBytes( newData.ToString() );
			//byte[]	saltBytes = encoding.GetBytes("{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}");

			SHAChecksum shaHelper = new SHAChecksum();
			byte[] pwdBytes = shaHelper.Checksum(userIDBytes);

			newData.Length = 0;
			foreach (byte pwdByte in pwdBytes)
			{
				newData.Append( pwdByte.ToString( "x2" ) ); // x indicates hexidecimal integer, 2 (the precision) is
				// the minimum number of digits.  Output will be zero
				// padded on the left as necessary
			}

			// Mangle some of the characters so that we will have more complex-looking passwords.  Go with:
			// a,c,e stay lowercase
			// b,d,f coerced to uppercase
			// 4,8 coerced to shifted form ($,*)
			// other digits stay same
			// Note that this actually adds no entropy to the passwords, it just makes them extremely likely to
			// meet group policy requirements.
			string passwordString = newData.ToString();
			passwordString = passwordString.Replace( 'b', 'B' );
			passwordString = passwordString.Replace( 'd', 'D' );
			passwordString = passwordString.Replace( 'f', 'F' );
			passwordString = passwordString.Replace( '4', '$' );
			passwordString = passwordString.Replace( '8', '*' );

			return passwordString;
		}

		/// <summary>
		/// Tests the passed-in id to verify that it is one of the recognized service
		/// ids.  
		/// </summary>
		/// <param name="testId">Id to test</param>
		/// <returns>
		/// true : <paramref name="testId"/> matches one of the service ids.
		/// false : <paramref name="testId"/> does not match any of the service ids.
		/// </returns>
		static public bool IsValidServiceLogin( string testId )
		{
			bool ret = false;
			switch (testId)
			{
				case serviceLogin:
				case adminLogin:
				case SaLogin:
					ret = true;
					break;
				default:
					ret = false;
					break;
			}

			return ret;
		}

		public VersionInfo GetVersion()
		{
			return ConsolidatedDAClass.GetVersion();
		}

		public Guid SiteAdminGuid() => Guids.SiteAdminGuid;

		public Dictionary<string, ForeignKeyDO> EnumerateForeignKeys(SecurityClass security, string schema, string tableName)
		{
			security.ThrowIfNull("security");

			if (string.IsNullOrEmpty(schema))
			{
				throw new Exception("schema");
			}

			if (string.IsNullOrEmpty(tableName))
			{
				throw new Exception("tableName");
			}

			Dictionary<string, ForeignKeyDO> foreignKeyDictionary = new Dictionary<string, ForeignKeyDO>();

			DataSet set;

			using (SqlCommand cmd = new SqlCommand())
			{
				ForeignKeyDO.EnumerateSQL(cmd, schema, tableName);
				set = ConsolidatedDA.GetDataSet(cmd, security);
			}

			DataTable table = set.Tables[0];
			foreach(DataRow row in table.Rows)
			{
				var foreignKey = new ForeignKeyDO();
				foreignKey.Load(row);
				foreignKeyDictionary.Add(foreignKey.ColumnName, foreignKey);
			}

			return foreignKeyDictionary;
		}

		#endregion
	}
	
}
