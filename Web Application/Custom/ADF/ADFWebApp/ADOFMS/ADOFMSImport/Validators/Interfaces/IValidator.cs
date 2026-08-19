using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADOFMSImport.Validators.Interfaces
{
   public interface IValidator
   {
      bool Validate();
      string GetErrorMessage();
   }
}
