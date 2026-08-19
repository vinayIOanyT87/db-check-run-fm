using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Xml;

using FMBusinessObjects.Exceptions;
using FMBusinessObjects.DataObjects;
using FMBusinessServices.ServiceClasses;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessServices.InternalClasses.EntityImportExport
{
    using FMCore;

    public abstract class WorksheetBaseImport : WorksheetBase
    {
        #region Protected data members
        protected CompanyClass company;
        protected EquipmentClass equipment;
        protected PersonRoleMapClass personRoleMap;
        protected QualificationClass personQualification;
        protected QualificationClass personLicense;
        protected ProductClass product;
        protected SecurityClass security;
        protected SiteClass site;
        protected ArrayList headerColumns;
        protected ArrayList recordRows = new ArrayList();
        protected XmlNamespaceManager nsMgr;
        protected bool firstRow;
        #endregion

        public Hashtable NumberFormatList = new Hashtable(128);

        #region Constructors
        /// <summary>
        /// This is the default constructor for the Worksheet base class.
        /// </summary>
        /// <param name="wrkshtName"></param>
        public WorksheetBaseImport(string wrkshtName)
           : base(WorksheetBase.WORKSHEET_COMPANIES)
        {
            this.security = new SecurityClass();
            base.EntityImportExportException.ClearMessages();
            this.headerColumns = new ArrayList(128);
            this.NumberFormatList = new Hashtable(128);
        }
        #endregion

        #region properties
        public SiteInfoDO SiteInfo
        {
            get;
            set;
        }

        public XmlNamespaceManager NameSpaceManager
        {
            get { return this.nsMgr; }
            set { this.nsMgr = value; }
        }

        public XmlNode WorksheetNode
        {
            get { return base.worksheetNode; }
            set { base.worksheetNode = value; }
        }

        public SecurityClass Security
        {
            get { return this.security; }
            set { this.security = value; }
        }

        public SiteClass Site
        {
            get { return this.site; }
            set { this.site = value; }
        }

        public ArrayList RecordRows
        {
            get { return this.recordRows; }
        }

        #endregion

        #region Abstract methods
        public abstract void ParseWorksheet();
        #endregion

        #region Protected methods
        /// <summary>
        /// This method will parse the row that contains the column names and build
        /// an array list of those name. The list will be used for matching up with
        /// the data rows.
        /// </summary>
        /// <param name="headerRow"></param>
        protected void ParseWorksheetHeaderCell(XmlNode headerCell)
        {
            if (headerCell == null)
            {
                base.EntityImportExportException.AppendMessage(EntityImportExportException.IMPORT_MSG_008,
                                               EntityImportExportException.EXCEPTION_TYPES.CRITICAL);
                throw base.EntityImportExportException;
            }

            string columnName = headerCell.InnerText;

            if ((columnName == null) || (columnName.Length <= 0))
            {
                base.EntityImportExportException.AppendMessage(EntityImportExportException.IMPORT_MSG_008,
                                                EntityImportExportException.EXCEPTION_TYPES.CRITICAL);
                throw base.EntityImportExportException;
            }

            if (this.headerColumns.Contains(columnName.ToUpper()) == true)
            {
                base.EntityImportExportException.AppendMessage(EntityImportExportException.IMPORT_MSG_009,
                                                EntityImportExportException.EXCEPTION_TYPES.CRITICAL);
                throw base.EntityImportExportException;
            }

            this.headerColumns.Add(columnName.ToUpper());
        }

        /// <summary>
        /// This method will parse each Row and build an array of
        /// that contains column name / data hash tables.
        /// </summary>
        /// <param name="rowNode"></param>
        public void ParseSheet(string rootID, string rootValue)
        {
            XmlNodeList XMLRowList = base.worksheetNode.SelectNodes("ss:Table/ss:Row", this.nsMgr);

            if (XMLRowList == null
            || XMLRowList.Count == 0)
            {
                base.EntityImportExportException.AppendMessage(EntityImportExportException.IMPORT_MSG_001 + this.WorksheetName,
                                                         EntityImportExportException.EXCEPTION_TYPES.ERROR);
                throw base.EntityImportExportException;
            }

            recordRows = new ArrayList();
            firstRow = true;

            foreach (XmlNode rowNode in XMLRowList)
            {
                if (!firstRow
                || headerColumns.Count == 0)
                    ParseRowNode(rowNode, rootID, rootValue);

                firstRow = false;
            }
        }


        /// <summary>
        /// This method will parse each Row element and build an array of
        /// that contains column name / data hash tables.
        /// </summary>
        /// <param name="rowNode"></param>
        protected void ParseRowNode(XmlNode rowNode, string rootID, string rootValue)
        {
            int columnCount = 0;

            if (rowNode == null)
            {
                base.EntityImportExportException.AppendMessage(EntityImportExportException.IMPORT_MSG_008,
                                               EntityImportExportException.EXCEPTION_TYPES.CRITICAL);
                throw base.EntityImportExportException;
            }

            XmlNodeList cellList = rowNode.SelectNodes("ss:Cell", this.nsMgr);

            if (cellList == null
            || cellList.Count == 0)
                throw new NullReferenceException("No Cells in Sheet Row " + this.WorksheetName);

            XmlNodeList dataList;

            Hashtable columnNameDataList = new Hashtable(512);

            foreach (XmlNode cellNode in cellList)
            {
                // Look for cell ss:Index attribute
                if (cellNode.Attributes["ss:Index"] != null)
                {
                    // Get the column number for this data and skip over the unused columns
                    int idx = int.Parse(cellNode.Attributes["ss:Index"].InnerText);
                    while (columnCount < idx - 1)
                    {
                        columnNameDataList.Add(this.headerColumns[columnCount++], "");
                    }
                }

                dataList = cellNode.SelectNodes("ss:Data", this.nsMgr);
                XmlNode dataNode = dataList.Item(0);

                // The first row contains the column names for all the rows. Build
                // an array list of those column name for later use.
                if (this.firstRow == true)
                {
                    this.ParseWorksheetHeaderCell(dataNode);
                }
                else
                {
                    if (columnCount < this.headerColumns.Count)
                    {
                        if (dataNode == null)
                        {
                            columnNameDataList.Add(this.headerColumns[columnCount], "");
                        }
                        else
                        {
                            string sData = dataNode.InnerText;

                            string headerName = headerColumns[columnCount].ToString();

                            if (rootValue != null && headerName.Equals(rootID) && sData.NotEquals(rootValue))
                            {
                                return;
                            }

                            XmlAttribute datatype = dataNode.Attributes["ss:Type"];
                            if (datatype != null && datatype.Value == "Number")
                            {
                                XmlAttribute styleID = cellNode.Attributes["ss:StyleID"];
                                if (NumberFormatList.ContainsKey(styleID.Value))
                                {
                                    string numberFormat = NumberFormatList[styleID.Value].ToString();
                                    if (!standardformatType.Contains(numberFormat))
                                    {
                                        string format = "{0:";
                                        format += numberFormat;
                                        format += "}";
                                        sData = string.Format(format, Convert.ToInt64(dataNode.InnerText));
                                    }
                                }
                            }

                            columnNameDataList.Add(this.headerColumns[columnCount], sData);
                        }

                        columnCount++;
                    }
                    else
                    {
                        // Error: data and header count differ.
                        base.EntityImportExportException.AppendMessage(EntityImportExportException.IMPORT_MSG_006,
                                                        EntityImportExportException.EXCEPTION_TYPES.CRITICAL);
                        throw base.EntityImportExportException;
                    }
                }
            }

            if (this.firstRow == false)
            {
                this.recordRows.Add(columnNameDataList);
            }
        }

        /// <summary>
        /// This method will return the Site Guid for a given Site ID.
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        protected Guid GetSiteGuid(string siteID)
        {
            if (SiteInfo == null)
            {
                throw new ArgumentNullException("SiteInfoDO");
            }

            Guid siteGuid = SiteInfo.GetSiteGuid(siteID);

            return siteGuid;
        }
        #endregion
    }
}