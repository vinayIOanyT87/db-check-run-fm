using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADOFMSImport.DataObjects.Interfaces;
using ADOFMSImport.Validators;

namespace ADOFMSImport.DataObjects
{
   public class CSVObject : DataObject, IDataObject
   {
      #region Attributes
      // a hash table representing the CSV file with:
      // keys - column names
      // values - ArrayList of objects
      protected Hashtable m_ds = new Hashtable();
      // an array containing the column orders
      protected ArrayList m_columnOrder = new ArrayList();
      // an array containing the column types
      protected ArrayList m_columnType = new ArrayList();
      // a table that maps to sub-class column types to CSV file column names
      protected Hashtable m_columnMap = new Hashtable();

      protected Defaults m_defaults;
      #endregion // Attributes

      #region Constants
      protected const string ERROR_SUPPLIED_MORE_COLUMNS = "supplied more or less values than there are in columns";
      protected const string ERROR_TYPE_MISMATCH = "supplied value for column does not match specified column type";
      protected const string ERROR_COLUMN_NOT_FOUND = "requested column was not found";

      public const string COLUMN_FMTRANSNAME = "FM TRANS NAME";
      public const string COLUMN_FMTRANSTYPE = "FM TRANS TYPE";
      public const string COLUMN_SITE = "SITE";
      #endregion

      #region IDataObject members
      public override void Reset()
      {
         m_ds = new Hashtable();
         m_columnOrder = new ArrayList();
      }

      public override DataObject CopyFrom(DataObject a_copy)
      {
         if (a_copy.GetType() != typeof(CSVObject))
         {
            throw new Exception("CSVObject CopyTo() expected type CSVObject but got " + a_copy.GetType().ToString());
         }

         CSVObject copy = a_copy as CSVObject;

         Hashtable dsCopy = copy.GetDataSet();
         foreach (string key in dsCopy.Keys)
         {
            m_ds[key] = dsCopy[key];
         }

         m_columnOrder = new ArrayList(copy.GetColumnOrders());
         m_columnType = new ArrayList(copy.GetColumnTypes());

         return this;
      }
      #endregion // IDataObject members

      #region Construction
      public CSVObject(Defaults a_defaults)
         : base()
      {
         m_defaults = a_defaults;
      }

      public CSVObject(CSVObject a_copy)
      {
         this.CopyFrom(a_copy);
      }
      #endregion // Construction

      public void AddColumn(string a_columnName, Type a_columnType)
      {
         m_ds[a_columnName.ToUpper()] = new ArrayList();
         m_columnOrder.Add(a_columnName.ToUpper());
         m_columnType.Add(a_columnType);
      }

      #region Properties
      public ICollection ColumnNames
      {
         get
         {
            return m_ds.Keys;
         }
      }

      public int Count
      {
         get
         {
            int count = 0;

            if (m_ds.Count > 0)
            {
               ArrayList list = null;
               foreach (string column in m_ds.Keys)
               {
                  list = m_ds[column.ToUpper()] as ArrayList;
                  break;
               }
               if (list != null)
                  count = list.Count;
            }

            return count;
         }
      }
      #endregion // Properties

      #region Internals
      internal Hashtable GetDataSet()
      {
         return m_ds;
      }

      internal ArrayList GetColumnOrders()
      {
         return m_columnOrder;
      }

      internal ArrayList GetColumnTypes()
      {
         return m_columnType;
      }
      #endregion // Internals

      #region Overridables
      public virtual bool IsAcceptableRow(object[] row)
      {
         throw new NotImplementedException();
      }

      public virtual object GetRowValue(int a_column, int a_row)
      {
         object result = null;
         string columnName = GetColumnName(a_column);
         if (!string.IsNullOrEmpty(columnName))
         {
            result = GetRowValue(columnName, a_row);
         }

         return result;
      }

      public virtual string GetColumnName(int a_column)
      {
         string columnName = null;

         if (m_columnMap.Contains(a_column))
         {
            columnName = m_columnMap[a_column].ToString();
         }

         return columnName;
      }

      public virtual int GetColumnOrder(string a_columnName)
      {
         return m_columnOrder.IndexOf(a_columnName.ToUpper());
      }

      public virtual void AddVRow(params object[] args)
      {
         object[] rowObjects = new object[args.Length];

         for (int i = 0; i < args.Length; ++i)
         {
            rowObjects[i] = args[i];
         }

         AddRow(rowObjects);
      }

      public virtual void AddRow(object[] args)
      {
         // verify row length is the same as the number of columns
         if (args.Length != m_ds.Count)
         {
            throw new Exception(ERROR_SUPPLIED_MORE_COLUMNS);
         }

         // verify types are consistent
         for (int i = 0; i < args.Length; ++i)
         {
            if (args[i].GetType() != (System.Type) m_columnType[i])
            {
               throw new Exception(ERROR_TYPE_MISMATCH);
            }
         }

         if (IsAcceptableRow(args))
         {
            // add the row to our ds
            for (int i = 0; i < args.Length; ++i)
            {
               string columnName = m_columnOrder[i] as string;
               ArrayList columnValue = m_ds[columnName] as ArrayList;
               columnValue.Add(args[i]);
            }
         }
         else
         {
            LoggerManager.LogProgress(m_defaults.LoggerKey, "Could add row, format mismatch: " + args.ToString());
         }
      }

      public virtual object[] GetRow(int a_row)
      {
         object[] result = new object[m_columnOrder.Count];

         int index = -1;
         foreach (string key in m_columnOrder)
         {
            if (m_ds.Contains(key))
            {
               ArrayList columnValues = m_ds[key] as ArrayList;
               if (columnValues != null)
               {
                  object value = null;
                  try
                  {
                     value = columnValues[a_row];
                  }
                  catch (Exception)
                  {
                     value = null;
                  }
                  result[++index] = value;
               }
            }
         }

         return result;
      }

      public virtual object GetRowValue(string a_columnName, int a_row)
      {
         object result = null;

         if (m_ds.Contains(a_columnName.ToUpper()))
         {
            ArrayList columnValue = m_ds[a_columnName.ToUpper()] as ArrayList;
            result = columnValue[a_row];
         }
         else
            throw new Exception(ERROR_COLUMN_NOT_FOUND);

         return result;
      }
      #endregion // Overridables
   }
}
