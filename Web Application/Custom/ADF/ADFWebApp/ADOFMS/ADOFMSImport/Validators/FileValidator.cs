using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ADOFMSImport.DataObjects;
using ADOFMSImport.Validators.Interfaces;

namespace ADOFMSImport.Validators
{
   public class FileValidator : IValidator
   {
      #region Attributes
      protected string m_fileName = null;
      protected string m_errorMessage = null;
      #endregion // Attributes

      public FileValidator(string a_fileName)
      {
         m_fileName = a_fileName;
      }

      #region IValidator members
      public virtual bool Validate()
      {
         throw new NotImplementedException(MethodBase.GetCurrentMethod().ToString());
      }

      public virtual string GetErrorMessage()
      {
         string result = "";
         if (m_errorMessage != null)
            result = m_errorMessage;

         return result;
      }
      #endregion IValidator members

      public virtual bool HasErrorMessage()
      {
         return (m_errorMessage != null);
      }

      protected virtual void ClearErrorMessage()
      {
         m_errorMessage = null;
      }
   }
}
