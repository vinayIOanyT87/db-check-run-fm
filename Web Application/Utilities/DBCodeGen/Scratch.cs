using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;
using System.Data;

namespace FuelsManagerCodeGen
{
	/// <summary>
	/// This class is not used.  T4 editor does not really have a very good context-sensitive help.
	/// It is easier to write it in here and copy over to the template.
	/// </summary>
	public class Scratch
	{
		public string DatabaseName { get; set; }
		public string FullTableName { get; set; }
		//public virtual TableInfo MyTable { get; set; }
		protected int _indentCount = 0;

		private string TrimWordGuid(string src)
		{
			string retValue = src;
			if (src.Length > 4)
			{
				int newLength = src.Length - 4;
				if (src.Substring(newLength).ToUpper().Equals("GUID"))
				{
					retValue = src.Substring(0, newLength);
				}
			}
			return retValue;
		}
		protected Column FindPrimaryKeyColumn(Table smoTable)
		{
			Column retValue = null;
			foreach (Column column in smoTable.Columns)
			{
				if (column.InPrimaryKey || column.IsForeignKey)
				{
					retValue = column;
					break;
				}
			}
			
			return retValue;
		}
		protected bool FindInStringArray(string[] list, string target)
		{
			bool found = false;
			for (int idx =0; idx<list.Length; idx++)
			{
				if (list[idx].Equals(target))
				{
					found = true;
					break;
				}
			}
			return found;
		}
		protected Table GetSqlSmoTable(string FullTableName)
		{
			Server server = new Server();
			Database smoDatabase = new Database(server, DatabaseName);
			
			Table smoTable = new Table(smoDatabase, FullTableName);
			foreach (Column column in smoTable.Columns)
			{
				string dataType = column.DataType.ToString().ToUpper();
				string dataLength = string.Empty;				
				switch (column.DataType.SqlDataType)
				{
					case SqlDataType.Char:
					case SqlDataType.VarChar:
					case SqlDataType.NChar:
					case SqlDataType.NVarChar:
					case SqlDataType.Timestamp:
						dataLength = string.Format("({0})", column.DataType.MaximumLength);
						break;
				}
			}
			smoTable.Refresh();
			return smoTable;
		}

		protected Table GetSqlSmoTable()
		{
			HashSet<string> objectList = new HashSet<string>();
		
			return null;
		}
		private List<string> WriteInsertParameters()
		{

			List<string> retValue = new List<string>();
			Table mySmoTable = GetSqlSmoTable();
			DataTable foreignKeyTable = mySmoTable.EnumForeignKeys();
			foreach (DataRow row in foreignKeyTable.Rows)
			{
				string schemaName = row.ItemArray[0].ToString();
				string referencingTableName = row.ItemArray[1].ToString();
				Table referencingTable = new Table(null, schemaName, referencingTableName);
				foreach (ForeignKey foreignKeyInfo in referencingTable.ForeignKeys)
				{
					if (foreignKeyInfo.ReferencedTable == mySmoTable.Name)
					{
						string entityName = referencingTableName;
						string columnName = foreignKeyInfo.Columns[0].Name;
						if (entityName.ToUpper().StartsWith("TBL"))
						{
							entityName = entityName.Substring(3);
						}
						if (entityName.ToUpper().StartsWith("ENTITY"))
						{
							entityName = entityName.Substring(6);
						}
						string spName = string.Format("[{0}].[usp_{1}DeleteBy{2}]", schemaName, entityName, columnName);
						retValue.Add(spName);
					}
				}
			}
			return retValue;
		}
		
		public static void Test()
		{
			

		}
	}
}

