using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADOFMSImport.DataObjects.Interfaces
{
   public interface IDataObject
   {
      void Reset();
      DataObject CopyFrom(DataObject a_copy);
   }
}
