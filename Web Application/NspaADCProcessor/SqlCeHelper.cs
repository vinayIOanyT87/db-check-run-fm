// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlCeHelper.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the SqlCeHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nspa
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Data.SqlServerCe;
	using System.IO;
	using System.Reflection;
	using System.Security.Cryptography;
	using System.Text;

	/// <summary>
	/// Helper class to handle all Sql Ce related functions.
	/// </summary>
	public class SqlCeHelper
	{
		private SqlCeConnection connectionInternal = null;

		private Guid id = Guid.NewGuid();

		private Guid siteGuid;

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCeHelper"/> class.
		/// </summary>
		/// <param name="newSiteGuid">The new site unique identifier.</param>
		internal SqlCeHelper(Guid newSiteGuid)
		{
			this.siteGuid = newSiteGuid;

			// calling this to bypass this error:
			// System.NotSupportedException: SQL Server Compact is not intended for ASP.NET development.
			AppDomain.CurrentDomain.SetData("SQLServerCompactEditionUnderWebHosting", true);
			
			CreateDb(this.GetDbPath());
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <returns></returns>
		private SqlCeConnection GetConnection()
		{
			var connection = this.connectionInternal;
			if (connection == null)
			{
				var connectionString = GetConnectionString();
			    try
			    {
                    connection = new SqlCeConnection(connectionString);
                    connection.Open();
                }
			    catch (Exception)
			    {
			        var engine = new SqlCeEngine(connectionString);
                    engine.CreateDatabase();
                    connection = new SqlCeConnection(connectionString);
                    connection.Open();
                }
				this.connectionInternal = connection;
			}
			return connection;
		}

		/// <summary>
		/// Creates the database.
		/// </summary>
		/// <param name="dbPath">The database path.</param>
		/// <param name="emptyDbPath">The empty database path.</param>
		private void CreateDb(string dbPath)
		{
			if (File.Exists(dbPath))
			{
				File.Delete(dbPath);
			}

			var dbResourceName = "Nspa.Nspa_Mobile_Reference.sdf";

			using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(dbResourceName))
			{
				if (resourceStream == null)
				{
					throw new Exception("Internal error : no empty database found.");
				}
				using (var fileStream = new FileStream(dbPath, FileMode.Create))
				{
					resourceStream.CopyTo(fileStream);
				}
			}
		}

		/// <summary>
		/// Gets the identifier string.
		/// </summary>
		/// <value>
		/// The identifier string.
		/// </value>
		public string IdString
		{
			get
			{
				var idString = this.id.ToString("N").ToUpper();
				return idString;
			}
		}

		/// <summary>
		/// Gets the working folder.
		/// </summary>
		/// <returns></returns>
		private static string GetWorkingFolder()
		{
			var folderName = Path.GetTempPath();
			return folderName;
		}

		/// <summary>
		/// Gets the name of the database file.
		/// </summary>
		/// <param name="fileId">The file identifier.</param>
		/// <returns></returns>
		public static string GetDbFileName(string fileId)
		{
			var dbName = string.Format("Nspa{0}.sdf", fileId);			
			return dbName;
		}

		/// <summary>
		/// Gets the database path static.
		/// </summary>
		/// <param name="fileId">The file identifier.</param>
		/// <returns></returns>
		public static string GetDbPathStatic(string fileId)
		{
			var dbName = GetDbFileName(fileId);
			var dbPath = Path.Combine(GetWorkingFolder(), dbName);
			return dbPath;
		}

		/// <summary>
		/// Gets the database path.
		/// </summary>
		/// <returns></returns>
		private string GetDbPath()
		{
			var dbPath = GetDbPathStatic(this.IdString);
			return dbPath;
		}

		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <returns></returns>
		private string GetConnectionString()
		{
			var dbPath = this.GetDbPath();
			var password = GetSomeText();
			var connectionString = string.Format("Data Source={0};Password={1};encryption mode=engine default", dbPath, password);
			return connectionString;
		}

		/// <summary>
		/// Cleanups this instance.
		/// </summary>
		internal void Cleanup()
		{
			if (this.connectionInternal != null)
			{
                // call dispose, which will close the connection if necessary
                // but will also free up any unmanaged resources
                this.connectionInternal.Dispose();
				this.connectionInternal = null;
			}
		}

		/// <summary>
		/// Saves the table.
		/// </summary>
		/// <param name="newDataTable">The new data table.</param>
		internal void SaveTable(DataTable newDataTable)
		{
			var connection = this.GetConnection();
			var sql = "Select * FROM " + newDataTable.TableName;
		    using (var dataAdapter = new SqlCeDataAdapter(sql, connection))
		    {
		        try
		        {
		            PrepareInsertCommand(dataAdapter, newDataTable, this.siteGuid);
		            dataAdapter.InsertCommand.Connection = connection;
		            dataAdapter.Update(newDataTable);
		        }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("FK_"))
                    {
                        return;
                    }
                    throw;
                }
                finally
                {
                    if (null != dataAdapter.InsertCommand)
                    {
                        dataAdapter.InsertCommand.Dispose();
                    }
                }
            }
		}

		#region static
		//private static string GenerateCreateTableSql(DataTable newDataTable)
		//{
		//	var buffer = new StringBuilder();
		//	buffer.AppendLine(string.Format("CREATE TABLE [{0}]", newDataTable.TableName));
		//	buffer.AppendLine("(");

		//	var allColumns = newDataTable.Columns;
		//	var columnCount = allColumns.Count;
		//	for (var index = 0; index < columnCount; index++)
		//	{
		//		var currentColumn = allColumns[index];
		//		var columnName = currentColumn.ColumnName;
		//		var columnType = GetColumnTypeString(currentColumn);
		//		buffer.Append(string.Format("[{0}] {1}", columnName, columnType));
		//		var separator = (index == columnCount - 1) ? "" : ",";
		//		buffer.AppendLine(separator);
		//	}
		//	buffer.AppendLine(")");
		//	return buffer.ToString();
		//}


		/// <summary>
		/// Determines whether [has site unique identifier] [the specified table name].
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns></returns>
		private static bool HasSiteGuid(string tableName)
		{
			var hasSiteGuid = true;
			var tableList = new[]
			                {
				                "tblEquipmentTypeClass", "tblProductToTransactionAliasExclusion", "tblSites",
				                "tblUserDataListValueTransactionAlias",
			                };
			foreach (var tempTableName in tableList)
			{
				if (string.Equals(tableName, tempTableName, StringComparison.InvariantCultureIgnoreCase))
				{
					hasSiteGuid = false;
					break;
				}
			}
			return hasSiteGuid;
		}

		/// <summary>
		/// Prepares the insert command.
		/// </summary>
		/// <param name="dataAdapter">The data adapter.</param>
		/// <param name="newDataTable">The new data table.</param>
		/// <param name="siteGuid">The site unique identifier.</param>
		private static void PrepareInsertCommand(SqlCeDataAdapter dataAdapter, DataTable newDataTable, Guid siteGuid)
		{
			var newCommand = new SqlCeCommand();
			var fieldNames = new StringBuilder();
			var values = new StringBuilder();

			var allColumns = newDataTable.Columns;
			var columnCount = allColumns.Count;
			var parameters = newCommand.Parameters;
			for (var index = 0; index < columnCount; index++)
			{
				var currentColumn = allColumns[index];
				var columnName = currentColumn.ColumnName;
				var parameterName = "@" + columnName;
				var columnType = GetSqlType(currentColumn.DataType);
				fieldNames.Append(string.Format("[{0}]", columnName));
				values.Append(string.Format("{0}", parameterName));
				var separator = (index == columnCount - 1) ? "" : ",";
				fieldNames.Append(separator);
				values.Append(separator);
				var fieldSize = GetFieldSize(currentColumn);
				var newParameter = new SqlCeParameter(parameterName, columnType, fieldSize, columnName);
				parameters.Add(newParameter);
			}

			if (HasSiteGuid(newDataTable.TableName))
			{
				fieldNames.Append(",[SiteGuid]");
				values.AppendFormat(",'{0}'", siteGuid);
			}

			var sql = string.Format(
				"INSERT INTO [{0}] ({1}) VALUES ({2})",
				newDataTable.TableName,
				fieldNames.ToString(),
				values.ToString());
			newCommand.CommandText = sql;

			dataAdapter.InsertCommand = newCommand;
		}


		/// <summary>
		/// Gets the size of the field.
		/// </summary>
		/// <param name="theColumn">The column.</param>
		/// <returns></returns>
		public static int GetFieldSize(DataColumn theColumn)
		{
			var type = theColumn.DataType;
			var dataLength = theColumn.MaxLength;
			if (type == typeof(string))
			{
				if (dataLength <= 0)
				{
					dataLength = 100;
				}
			}
			if (type == typeof(byte[]))
			{
				if (dataLength <= 0)
				{
					dataLength = 100;
				}
			}
			else
			{
				if (dataLength <= 0)
				{
					dataLength = 0;
				}
			}
			return dataLength;
		}

		/// <summary>
		/// Get the equivalent SQL data type of the given type.
		/// </summary>
		/// <param name="type">Type to get the SQL type equivalent of</param>
		public static SqlDbType GetSqlType(Type type)
		{
			if (type == typeof(string))
				return SqlDbType.NVarChar;
			if (type == typeof(byte[]))
				return SqlDbType.VarBinary;
			if (type == typeof(UInt64))
				return SqlDbType.BigInt;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = Nullable.GetUnderlyingType(type);

			var param = new SqlParameter("", Activator.CreateInstance(type));
			return param.SqlDbType;
		}

		/// <summary>
		/// Gets the column type string.
		/// </summary>
		/// <param name="theColumn">The column.</param>
		/// <returns></returns>
		//private static string GetColumnTypeString(DataColumn theColumn)
		//{
		//	string dataTypeString;

		//	var dbType = GetSqlType(theColumn.DataType);
		//	var fieldSize = GetFieldSize(theColumn);
								
		//	if (fieldSize>0)
		//	{
		//		dataTypeString = string.Format("NVARCHAR({0})", fieldSize);
		//	}
		//	else
		//	{
		//		dataTypeString = dbType.ToString();
		//	}
		//	return dataTypeString;
		//}


		/// <summary>
		/// generates a database text
		/// </summary>
		/// <param name="userId">User Id for which to generate a database Password</param>
		/// <returns>Generated database Password</returns>
		private static string GetSomeText()
		{
			return GetSomeText("Flightline");
		}

		/// <summary>
		/// generates a database text
		/// </summary>
		/// <param name="source">input for the text</param>
		/// <returns>Generated database text</returns>
		private static string GetSomeText(string source)
		{
			// Algorithm is to take a SHA-1 hash of the bytes of the ASCII representation
			// of the user ID followed by the bytes of the salt "{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}"
			ASCIIEncoding encoding = new ASCIIEncoding();
			SHA1 sha = new SHA1CryptoServiceProvider();

			// Split out for obfuscation purposes
			// Probably something more thorough required later

			//Eric Simmons
			//08-10-2007
			//Updated to ensure that UserID is always uppercase.
			//resolves CSI #5049
			StringBuilder newData = new StringBuilder(source.ToUpper());
			newData.Append('{');
			newData.Append('0');
			newData.Append('1');
			newData.Append('A');
			newData.Append('F');
			newData.Append('E');
			newData.Append('B');
			newData.Append('D');
			newData.Append('3');
			newData.Append('-');
			newData.Append('7');
			newData.Append('8');
			newData.Append('C');
			newData.Append('D');
			newData.Append('-');
			newData.Append('4');
			newData.Append('B');
			newData.Append('1');
			newData.Append('5');
			newData.Append('-');
			newData.Append('A');
			newData.Append('B');
			newData.Append('9');
			newData.Append('B');
			newData.Append('-');
			newData.Append('F');
			newData.Append('4');
			newData.Append('A');
			newData.Append('A');
			newData.Append('1');
			newData.Append('C');
			newData.Append('0');
			newData.Append('E');
			newData.Append('2');
			newData.Append('D');
			newData.Append('9');
			newData.Append('B');
			newData.Append('}');
			byte[] userIDBytes = encoding.GetBytes(newData.ToString());
			//byte[]	saltBytes = encoding.GetBytes("{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}");

			byte[] pwdBytes = sha.ComputeHash(userIDBytes);

			newData.Length = 0;
			foreach (byte pwdByte in pwdBytes)
			{
				newData.Append(pwdByte.ToString("x2")); // x indicates hexidecimal integer, 2 (the precision) is
				// the minimum number of digits.  Output will be zero
				// padded on the left as necessary
			}

			// Mangle some of the characters so that we will have more complex-looking text.  Go with:
			// a,c,e stay lowercase
			// b,d,f coerced to uppercase
			// 4,8 coerced to shifted form ($,*)
			// other digits stay same
			// Note that this actually adds no entropy to the text, it just makes them extremely likely to
			// meet group policy requirements.
			string finalString = newData.ToString();
			finalString = finalString.Replace('b', 'B');
			finalString = finalString.Replace('d', 'D');
			finalString = finalString.Replace('f', 'F');
			finalString = finalString.Replace('4', '$');
			finalString = finalString.Replace('8', '*');

			return finalString;
		}
		
		#endregion

		internal byte[] GetFileHash()
		{
			// I think that this has to match the handheld side to be correct.
			// This can't be big to avoid memory issue on HH side.
			const int BlockSize = 8 * 1024;
			var dbPath = this.GetDbPath();
			using (var md5Hasher = MD5.Create())
			{
				using (var dbStream = File.OpenRead(dbPath))
				{

					var fileBuffer = new byte[BlockSize];
					var hashBuffer = new byte[BlockSize];
					var emptyBuffer = new byte[0];
					int len;
					while ((len = dbStream.Read(fileBuffer, 0, fileBuffer.Length)) > 0)
					{
						md5Hasher.TransformBlock(hashBuffer, 0, len, fileBuffer, 0);
					}
					md5Hasher.TransformFinalBlock(emptyBuffer, 0, 0);
					var newHash = md5Hasher.Hash;
					return newHash;
				
				}
			}
		}
	}
}
