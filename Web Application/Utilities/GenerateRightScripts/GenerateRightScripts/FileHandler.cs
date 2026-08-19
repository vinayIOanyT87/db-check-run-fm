using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace GenerateRightScripts
{
    using Microsoft.Office.Interop.Excel;

    public class FileHandler
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public FileHandler()
        {
            this.Init();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method will read and the excel SS and return an object with a list
        /// of the items.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<RightsClass> ReadInputFile(string fileName)
        {
            Application xlApp       = new Application();
            Workbook xlWorkbook     = xlApp.Workbooks.Open(fileName, 0, true, 5, "", "", true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Worksheet xlWorksheet   = (Worksheet)xlWorkbook.Worksheets.Item[1];
            Range xlRange           = xlWorksheet.UsedRange;
            int rowCount            = xlRange.Rows.Count;
            int columnCount         = xlRange.Columns.Count;

            List<RightsClass> rightsList = new List<RightsClass>();

            if (rowCount == 0 || columnCount == 0)
            {
                return rightsList;
            }

            try
            {
                // Header is on row 1.
                for (int nextRow = 2; nextRow <= rowCount; nextRow++)
                {
                    var rightsObj = new RightsClass();

                    for (int nextColumn = 1; nextColumn <= columnCount; nextColumn++)
                    {
                        if (nextColumn == 1)
                        {
                            rightsObj.RightIndexStr = ((double)(xlRange.Cells[nextRow, nextColumn] as Range).Value2).ToString();
                        }

                        if (nextColumn == 2)
                        {
                            rightsObj.RightCode = (string)(xlRange.Cells[nextRow, nextColumn] as Range).Value2;
                        }

                        if (nextColumn == 3)
                        {
                            rightsObj.RightDescription = (string)(xlRange.Cells[nextRow, nextColumn] as Range).Value2;
                        }
                    }

                    rightsList.Add(rightsObj);
                }
            }
            catch (Exception)
            {
                // ignore
                xlWorkbook.Close(true, null, null);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorksheet);
                Marshal.ReleaseComObject(xlWorkbook);
                Marshal.ReleaseComObject(xlApp);

                return rightsList;
            }           

            xlWorkbook.Close(true, null, null);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorksheet);
            Marshal.ReleaseComObject(xlWorkbook);
            Marshal.ReleaseComObject(xlApp);

            return rightsList;
        }

        /// <summary>
        /// This method will write the update SQL to a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="updateList"></param>
        public void WriteOutput(string fileName, List<string> updateList )
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                foreach (string updateSql in updateList)
                {
                    file.WriteLine(updateSql);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will set the object to its initial state.
        /// </summary>
        private void Init()
        {
            
        }
        #endregion
    }
}
