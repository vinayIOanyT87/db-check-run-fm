using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ADOFMSImport.DataObjects;

namespace ADOFMSImport.Transformers.Interfaces
{
   public interface ITransformer
   {
      bool Transform(CSVObject a_csv);
      Type GetTransformingType();
   }
}
