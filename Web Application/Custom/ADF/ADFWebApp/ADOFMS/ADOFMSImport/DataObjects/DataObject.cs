using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using ADOFMSImport.DataObjects.Interfaces;
using ADOFMSImport.Validators;

namespace ADOFMSImport.DataObjects
{
   public class DataObject : IDataObject, IDisposable
   {
      #region IDataObject members
      public virtual void Reset()
      {
         throw new NotImplementedException(MethodBase.GetCurrentMethod().ToString());
      }

      public virtual DataObject CopyFrom(DataObject a_copy)
      {
         throw new NotImplementedException(MethodBase.GetCurrentMethod().ToString());
      }
      #endregion // IDataObject members

      #region IDisposable members
      public void Dispose()
      {
      }
      #endregion
   }
}
