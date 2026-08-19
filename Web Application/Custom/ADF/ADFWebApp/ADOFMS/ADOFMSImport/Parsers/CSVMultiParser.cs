using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ADOFMSImport.Parsers.Interfaces;
using ADOFMSImport.DataObjects;
using ADOFMSImport.Validators;

namespace ADOFMSImport.Parsers
{
   public class CSVMultiParser : Parser, IParser
   {
      #region Construction
      public CSVMultiParser(params CSVObject[] a_objects)
         : base(a_objects)
      {
      }
      #endregion // Construction

      #region IReader members
      public override void Read(string a_fileName)
      {
         foreach (CSVObject csv in m_dest)
         {
            csv.Reset();
         }

         // validate file first
         CSVFileValidator validator = new CSVFileValidator(a_fileName);
         bool ok = validator.Validate();
         if (!ok)
         {
            // throw all critical errors
            throw new Exception(validator.GetErrorMessage());
         }
         else
         {
            // no errors
            using (StreamReader reader = new StreamReader(a_fileName))
            {
               // read first line for column names
               string line = reader.ReadLine();
               if (line != null)
               {
                  string[] columnNames = line.Split(',');
                  // create each column in the CSV data object
                  foreach (string columnName in columnNames)
                  {
                     foreach (CSVObject csv in m_dest)
                     {
                        csv.AddColumn(columnName.Trim(), typeof(string));
                     }
                  }
               }

               // read until EOL
               while ((line = reader.ReadLine()) != null)
               {
                  string[] rowValues = line.Split(',');

                  object[] input = new object[rowValues.Length];
                  for (int i = 0; i < rowValues.Length; ++i)
                  {
                     input[i] = rowValues[i].Trim();
                  }

                  if (input.Length > 0)
                  {
                     foreach (CSVObject csv in m_dest)
                     {
                        if (csv.IsAcceptableRow(input))
                        {
                           csv.AddRow(input);
                        }
                     }
                  }
               }
            }
         }
      }
      #endregion // IReader members
   }
}
