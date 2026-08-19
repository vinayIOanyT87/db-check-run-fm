using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	#region Invoice Query Collection Class
   [Serializable]
   [CollectionDataContract]
	[KnownType(typeof(InvoiceQueryClass))]
	public class InvoiceQueryCollectionClass : CollectionBase
	{
		public void Add(InvoiceQueryClass a_wac)
		{
			List.Add(a_wac);
		}

		public void Remove(int index)
		{
			if (index > Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				List.RemoveAt(index);
			}
		}

		public void Remove(InvoiceQueryClass a_wac)
		{
			int index = 0;
			foreach (InvoiceQueryClass Item in List)
			{
				if (Item.IdentityGuid == a_wac.IdentityGuid)
				{
					List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public InvoiceQueryClass Item(int Index)
		{
			return (InvoiceQueryClass)List[Index];
		}
	}
	#endregion

	#region Invoice Query Class
	[DataContract]
   [Serializable]
	public class InvoiceQueryClass : BaseDataObject
	{
		[DataMember]
		public string Description
		{
			get;
			set;
		}

		public void Load(DataSet a_ds)
		{
			if (a_ds == null)
			{
				throw new ArgumentNullException("Set");
			}

			this.Reset();

			DataTable table = a_ds.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			this.Load(row);
		}

		#region Enumerators
		static public string EnumerateSQL()
		{
			string sql = "SELECT * FROM tblInvoiceQueries";

			return sql;
		}

		//static public string EnumerateByIndex ( int a_queryIndex )
		//{
		//   string sql = InvoiceQueryClass.EnumerateSql ( );

		//   sql += " WHERE queryIndex = " + a_queryIndex.ToString ( );

		//   return sql;
		//}

		//static public string EnumerateByKeyword ( string a_word )
		//{
		//   string sql = InvoiceQueryClass.EnumerateSql ( );

		//   sql += " WHERE description LIKE '%" + a_word + "%'";

		//   return sql;
		//}
		#endregion // Enumerators

		#region Sql Command with Parameters

		static public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblInvoiceQueries";
		}

		static public void EnumerateByIdentityGuid(SqlCommand cmd, Guid invoiceQueryGuid)
		{
			cmd.CommandText = "SELECT * FROM tblInvoiceQueries WHERE InvoiceQueryGuid = @InvoiceQueryGuid";
			cmd.Parameters.Add("@InvoiceQueryGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@InvoiceQueryGuid"].Value = invoiceQueryGuid;
		}

		static public void EnumerateByKeyword(SqlCommand cmd, string a_word)
		{
			cmd.CommandText += "SELECT * FROM tblInvoiceQueries WHERE description LIKE @description";
			cmd.Parameters.Add("@description", SqlDbType.NVarChar, 512);
			cmd.Parameters["@description"].Value = "%" + a_word + "%";
		}
		#endregion

		public void Load(DataRow a_row)
		{
			IdentityGuid = DataObject.getValue<Guid>(a_row["InvoiceQueryGuid"], Guid.Empty);
			Description = DataObject.getValue<string>(a_row["Description"], "");
			CreatedBy = DataObject.getValue<string>(a_row["CreatedBy"], ADMIN);
			CreatedDate = DataObject.getValue<DateTimeOffset>(a_row["CreatedDate"], DateTimeOffset.Now);
			UpdatedBy = DataObject.getValue<string>(a_row["UpdatedBy"], ADMIN);
			UpdatedDate = DataObject.getValue<DateTimeOffset>(a_row["UpdatedDate"], CreatedDate);
		}

		#region Overrides
		public override void Load(object o)
		{
			base.Load(o);
		}

		public override void Reset()
		{
			base.Reset();

			this.Description = " ";
		}
		#endregion // Overrides
	}
	#endregion
}
