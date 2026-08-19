using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ADOFMSImport.DataObjects;
using ADOFMSImport.Validators.Interfaces;

namespace ADOFMSImport.Validators
{
   public class CSVFileValidator : FileValidator, IValidator
   {
      #region Constants
      protected const string ERROR_MALFORMED = "malformed input CSV file, number of columns on each line is not consistent";
      #endregion // Constants

      public CSVFileValidator(string a_csvFile)
         : base(a_csvFile)
      {
      }

      #region IValidator members
      public override bool Validate()
      {
         bool result = true;

         try
         {
            using (StreamReader reader = new StreamReader(m_fileName))
            {
               // some validation parametres:
               int nCommaPerLine = 0;
               int rowIndex = 0;
               bool firstLineDone = false;

               string line = null;
               while ((line = reader.ReadLine()) != null)
               {
                  if (nCommaPerLine >= 0)
                  {
                     int commaCount = 0;
                     for (int i = 0; i < line.Length; ++i)
                     {
                        if (line[i] == ',')
                        {
                           ++commaCount;
                        }
                     }

                     if (!firstLineDone)
                     {
                        nCommaPerLine = commaCount;
                        firstLineDone = true;
                     }

                     if (commaCount != nCommaPerLine)
                     {
                        m_errorMessage = "Number of columns on row " + rowIndex.ToString() +
                           " does not match other columns. Expected " + (nCommaPerLine + 1) + " columns, found " + (commaCount + 1);
                        result = false;
                        break;
                     }
                  }

                  ++rowIndex;
               }
            }
         }
         catch (Exception e)
         {
            m_errorMessage = e.Message;
         }

         return result;
      }

      public override string GetErrorMessage()
      {
         string result = base.GetErrorMessage();

         base.ClearErrorMessage();

         return result;
      }
      #endregion // IValidator members
   }
}
