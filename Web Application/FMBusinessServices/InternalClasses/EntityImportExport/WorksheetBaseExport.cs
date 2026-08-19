using System;
using System.Collections;
using System.Text;
using System.Xml;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Interfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;
using System.Collections.Generic;

namespace EntityImportExport
{
    using FMBusinessServices.InternalClasses.EntityImportExport;

    public abstract class WorksheetBaseExport : WorksheetBase
    {
        #region Protected data members
        protected XmlNode tableNode;
        protected SecurityClass security;
        protected SiteClass site;
        #endregion

        #region Private data members
        private XmlDocument excelDoc;
        private string worksheetXml;
        private string excelXmlUrl;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor for the Worksheet base class.
        /// </summary>
        /// <param name="wrkshtName"></param>
        public WorksheetBaseExport(string wrkshtName)
           : base(wrkshtName)
        {
            this.security = new SecurityClass();
            this.excelXmlUrl = "http://schemas.microsoft.com/office/excel/2003/xml";
            this.excelDoc = new XmlDocument();
            this.CreateWorksheetNodes();
        }
        #endregion

        #region Properties
        public string ExcelXmlUrl
        {
            get { return this.excelXmlUrl; }
        }

        public string WorksheetXML
        {
            get
            {
                this.RemoveXmlnsUrl();
                return this.worksheetXml;
            }
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

        public SiteInfoDO SiteInfo
        {
            get;
            set;
        }

        public List<EntityImportExportAttribute> RootAttributeList { get; set; }

        #endregion

        #region Private methods
        /// <summary>
        /// This method creates the worksheet and table nodes for any of the worksheets.
        /// </summary>
        private void CreateWorksheetNodes()
        {
            this.worksheetNode = (XmlNode)this.excelDoc.CreateNode(XmlNodeType.Element, "Worksheet", null);
            XmlAttribute attribute = this.worksheetNode.OwnerDocument.CreateAttribute("ss", "Name", this.ExcelXmlUrl);
            attribute.Value = base.WorksheetName;
            this.worksheetNode.Attributes.Append(attribute);

            this.tableNode = (XmlNode)this.worksheetNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Table", null);
            this.worksheetNode.AppendChild(this.tableNode);
        }

        /// <summary>
        /// This method will remove the xmlns URL from the attributes of all the nodes. With the URL in
        /// the attribute, excel will not work.
        /// </summary>
        private void RemoveXmlnsUrl()
        {
            this.worksheetXml = this.worksheetNode.OuterXml;
            this.worksheetXml = this.worksheetXml.Replace("xmlns:ss=\"http://schemas.microsoft.com/office/excel/2003/xml\"", "");
        }
        #endregion

        #region Abstract methods
        public abstract void CreateWorksheet(object obj);
        #endregion

        #region Protected methods
        /// <summary>
        /// This method will return the Site ID for a given site guid.
        /// </summary>
        /// <param name="siteGuid"></param>
        /// <returns></returns>
        protected string GetSiteID(Guid siteGuid)
        {
            if (SiteInfo == null)
            {
                throw new ArgumentNullException("SitesInfo");
            }

            string siteID = SiteInfo.GetSiteID(siteGuid);

            return siteID;

        }
        #endregion
    }
}
