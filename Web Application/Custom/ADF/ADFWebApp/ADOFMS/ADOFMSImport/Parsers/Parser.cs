using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using ADOFMSImport.Parsers.Interfaces;
using ADOFMSImport.DataObjects;

namespace ADOFMSImport.Parsers
{
   public class Parser : IParser, IDisposable
   {
      #region Attributes
      protected string m_fileName = null;
      protected DataObject[] m_dest = null;
      #endregion Attributes

      #region Construction
      public Parser(DataObject a_dest)
      {
         m_dest = new DataObject[1];
         m_dest[0] = new DataObject().CopyFrom(a_dest);
      }

      public Parser(DataObject[] a_dest)
      {
         m_dest = a_dest;
      }
      #endregion // Construction

      #region IReader members
      public virtual void Read(string a_fileName)
      {
         throw new NotImplementedException(MethodBase.GetCurrentMethod().ToString());
      }
      #endregion // IReader members

      #region IDisposable members
      public void Dispose()
      {
         foreach (DataObject obj in m_dest)
         {
            obj.Dispose();
         }

         m_dest = null;
      }
      #endregion // IDisposable members

      #region Overridables
      public virtual DataObject GetDataObject()
      {
         DataObject result = null;
         if (m_dest.Length > 0)
            result = m_dest[0];

         return result;
      }

      public virtual int GetDataObjectCount()
      {
         return m_dest.Length;
      }

      public virtual DataObject GetDataObject(int a_index)
      {
         return m_dest[a_index];
      }
      #endregion // Overridables
   }
}
