// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcelImport.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ExcelImport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EntityImportExport
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Web;
    using System.Xml;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using System.Collections.Generic;
    using System.ServiceModel.Channels;

    /// <summary>
    /// The excel import.
    /// </summary>
    public class ExcelImport
    {
        #region Private data members
        private XMLImportDocumentCollectionClass xmldocumentcollection;
        private XmlDocument entityDoc;
        private SecurityClass security;
        private SiteClass site;
        private bool importCompanies;
        private bool importEquipment;
        private bool importPersonnel;
        private bool importProducts;
        private bool importStandingOffers;
        private bool importFuelCard;
        private bool importIATACodes;
        private bool importEquipmentTypes;
        private bool importAssignments;
        private bool importPoints;
        private bool importPointTemplates;
        private bool importPointCategories;
        private bool importPointTypes;
        private bool importPointTags;
        private bool includeStrapTables;
        private XmlNamespaceManager nameSpaceManager;
        private ImportExportException impExpException;
        private DateTime startTime = DateTime.Now;
        private int sessionTimeout = HttpContext.Current.Session.Timeout;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelImport"/> class. 
        /// This is the default constructor for the excel import class.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        public ExcelImport(SecurityClass security, SiteClass site)
        {
            this.xmldocumentcollection = new XMLImportDocumentCollectionClass();
            this.Initialize(security, site, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelImport"/> class. 
        /// This constructor will set the entity document data member.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <param name="entityDoc">
        /// Entity document.
        /// </param>
        public ExcelImport(SecurityClass security, SiteClass site, XmlDocument entityDoc)
        {
            this.xmldocumentcollection = new XMLImportDocumentCollectionClass();
            this.Initialize(security, site, entityDoc);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the site info.
        /// </summary>
        public SiteInfoDO SiteInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether import companies.
        /// </summary>
        public bool ImportCompanies
        {
            get { return this.importCompanies; }
            set { this.importCompanies = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import equipment.
        /// </summary>
        public bool ImportEquipment
        {
            get { return this.importEquipment; }
            set { this.importEquipment = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import personnel.
        /// </summary>
        public bool ImportPersonnel
        {
            get { return this.importPersonnel; }
            set { this.importPersonnel = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import products.
        /// </summary>
        public bool ImportProducts
        {
            get { return this.importProducts; }
            set { this.importProducts = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import standing offers.
        /// </summary>
        public bool ImportStandingOffers
        {
            get { return this.importStandingOffers; }
            set { this.importStandingOffers = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import fuel card.
        /// </summary>
        public bool ImportFuelCard
        {
            get { return this.importFuelCard; }
            set { this.importFuelCard = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import IATA codes.
        /// </summary>
        public bool ImportIATACodes
        {
            get { return this.importIATACodes; }
            set { this.importIATACodes = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import equipment types.
        /// </summary>
        public bool ImportEquipmentTypes
        {
            get { return this.importEquipmentTypes; }
            set { this.importEquipmentTypes = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import assignments.
        /// </summary>
        public bool ImportAssignments
        {
            get { return this.importAssignments; }
            set { this.importAssignments = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import points.
        /// </summary>
        public bool ImportPoints
        {
            get { return this.importPoints; }
            set { this.importPoints = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import point templates.
        /// </summary>
        public bool ImportPointTemplates
        {
            get { return this.importPointTemplates; }
            set { this.importPointTemplates = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import point categories.
        /// </summary>
        public bool ImportPointCategories
        {
            get { return this.importPointCategories; }
            set { this.importPointCategories = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import point templates.
        /// </summary>
        public bool ImportPointTypes
        {
            get { return this.importPointTypes; }
            set { this.importPointTypes = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether import point tags.
        /// </summary>
        public bool ImportPointTags
        {
            get { return this.importPointTags; }
            set { this.importPointTags = value; }
        }
        /// <summary>
        /// Gets the import exception.
        /// </summary>
        public ImportExportException ImportException
        {
            get { return this.impExpException; }
        }
		  public bool IncludeStrapTables
		  {
		      get { return this.includeStrapTables; }
			   set { this.includeStrapTables = value; }
		  }
		#endregion

		#region Public methods
		/// <summary>
		/// This method starts the import process for all worksheets.
		/// </summary>
		public void StartImport()
        {
            try
            {
                if (this.importProducts)
                {
                    var product = new ProductClass(this.site);

                    this.ImportSelectedObjectFromExcel("PRODUCTID*", null, product, typeof(IProducts));

                    this.xmldocumentcollection.Clear();
                }

                if (this.importEquipmentTypes)
                {
                    var equipmenttype = new EquipmentTypeClass(this.site);

                    this.ImportSelectedObjectFromExcel("TYPECLASSID*", null, equipmenttype, typeof(IEquipmentTypes));

                    this.xmldocumentcollection.Clear();
                }

                if (this.importEquipment)
                {
                    var equipment = new EquipmentClass(this.site);

                    this.ImportSelectedObjectFromExcel("EQUIPMENTID*", null, equipment, typeof(IEquipments));

                    this.xmldocumentcollection.Clear();
                }

                if (this.importPersonnel)
                {
                    var person = new PersonClass(this.site);

                    this.ImportSelectedObjectFromExcel("PERSONID*", null, person, typeof(IPersonnel));

                    this.xmldocumentcollection.Clear();
                }

                if (this.importStandingOffers)
                {
                    var standingoffer = new StandingOfferClass();

                    this.ImportSelectedObjectFromExcel("STANDINGOFFERID*", null, standingoffer, typeof(IStandingOffers));

                    this.xmldocumentcollection.Clear();
                }

                if (this.importFuelCard)
                {
                    var fuelcard = new FuelCardClass();

                    this.ImportSelectedObjectFromExcel("FUELCARDID*", null, fuelcard, typeof(IFuelCards));

                    this.xmldocumentcollection.Clear();
                }

                if (this.ImportIATACodes)
                {
                    var iatacode = new IATACodeClass();

                    this.ImportSelectedObjectFromExcel("IATACODEID*", null, iatacode, typeof(IIATACodes));

                    this.xmldocumentcollection.Clear();
                }

                if (this.importCompanies)
                {
                    var company = new CompanyClass(this.site);

                    this.ImportSelectedObjectFromExcel("COMPANYID*", null, company, typeof(ICompanies));

                    this.xmldocumentcollection.Clear();
                }

                if (this.importAssignments)
                {
                    var entityToSiteMap = new EntityToSiteMapClass();

                    this.ImportSelectedObjectFromExcel("ASSIGNEDID*", null, entityToSiteMap, typeof(IEntityToSiteMaps));

                    this.xmldocumentcollection.Clear();
                }

                if (this.importPointTemplates)
                {
                    var pointTemplate = new PointTemplate();
                    var module = new FMBusinessObjects.DataObjects.Module();
                    var alarmCategory = new ApplicationStringClass();
                    var alarmPriority = new AlarmPriorityClass();

                    List<EntityImportExportAttribute> rootAttributeListPts = new List<EntityImportExportAttribute>();
                    List<EntityImportExportAttribute> rootAttributeListModules = new List<EntityImportExportAttribute>();
                    List<EntityImportExportAttribute> rootAttributeListAlarmCategories = new List<EntityImportExportAttribute>();
                    List<EntityImportExportAttribute> rootAttributeListAlarmPriorities = new List<EntityImportExportAttribute>();

                    rootAttributeListPts.Add(new EntityImportExportAttribute("SITE*", 100, "SiteGuid", this.security.SiteID));
                    rootAttributeListModules.Add(new EntityImportExportAttribute("SITE*", 100, "SiteGuid", this.security.SiteID));
                    rootAttributeListAlarmCategories.Add(new EntityImportExportAttribute("SITE*", 100, "SiteGuid", this.security.SiteID));
                    rootAttributeListAlarmPriorities.Add(new EntityImportExportAttribute("SITE*", 100, "SiteGuid", this.security.SiteID));

                    EntityImportExportWorksheetAttribute entityImportExportWorksheetAttributePts = new EntityImportExportWorksheetAttribute("POINTTEMPLATES", "POINTTEMPLATEID*");
                    EntityImportExportWorksheetAttribute entityImportExportWorksheetAttributeModules = new EntityImportExportWorksheetAttribute("MODULES", "MODULEID*");
                    EntityImportExportWorksheetAttribute entityImportExportWorksheetAttributeAlarmCategories = new EntityImportExportWorksheetAttribute("POINTTEMPLATEAPPSTRINGS", "ID*");
                    EntityImportExportWorksheetAttribute entityImportExportWorksheetAttributeAlarmPriorities = new EntityImportExportWorksheetAttribute("ALARMPRIORITIES", "ALARMPRIORITYID*");

                    //this.CreateWorkSheet(entityImportExportWorksheetAttribute, rootAttributeList, typeof(ProductClass));
                    //this.CreateWorkSheet(entityImportExportWorksheetAttributeNumTwo, rootAttributeList, typeof(ModuleData));
                
                    this.CreateWorkSheet(entityImportExportWorksheetAttributePts, rootAttributeListPts, typeof(PointTemplate));
                    this.CreateWorkSheet(entityImportExportWorksheetAttributeModules, rootAttributeListModules, typeof(FMBusinessObjects.DataObjects.Module));
                    this.CreateWorkSheet(entityImportExportWorksheetAttributeAlarmCategories, rootAttributeListAlarmCategories, typeof(ApplicationStringClass));
                    this.CreateWorkSheet(entityImportExportWorksheetAttributeAlarmPriorities, rootAttributeListAlarmPriorities, typeof(AlarmPriorityClass));

                    this.ImportSelectedObjectWithDictionaryRelationshipsFromExcel(entityImportExportWorksheetAttributeModules, module);
                    this.ImportSelectedObjectWithDictionaryRelationshipsFromExcel(entityImportExportWorksheetAttributeAlarmCategories, alarmCategory);
                    this.ImportSelectedObjectWithDictionaryRelationshipsFromExcel(entityImportExportWorksheetAttributeAlarmPriorities, alarmPriority);
                    this.ImportSelectedObjectWithDictionaryRelationshipsFromExcel(entityImportExportWorksheetAttributePts, pointTemplate);

                    this.xmldocumentcollection.Clear();
                }

                if (this.importPoints)
                {
                    var point = new Point();

                    List<EntityImportExportAttribute> rootAttributeList = new List<EntityImportExportAttribute>();
                    rootAttributeList.Add(new EntityImportExportAttribute("SITE*", 100, "SiteGuid", this.security.SiteID));
                    EntityImportExportWorksheetAttribute entityImportExportWorksheetAttribute = new EntityImportExportWorksheetAttribute("POINTS", "POINTID*");

                    this.CreateWorkSheet(entityImportExportWorksheetAttribute, rootAttributeList, typeof(ProductClass));

                    this.ImportSelectedObjectWithDictionaryRelationshipsFromExcel(entityImportExportWorksheetAttribute, point);

                    this.xmldocumentcollection.Clear();
                }

                if (this.importPointTags)
                {
                    var point = new Point();

                    List<EntityImportExportAttribute> rootAttributeList = new List<EntityImportExportAttribute>();
                    rootAttributeList.Add(new EntityImportExportAttribute("SITE*", 100, "SiteGuid", this.security.SiteID));
                    EntityImportExportWorksheetAttribute entityImportExportWorksheetAttribute = new EntityImportExportWorksheetAttribute("POINTS", "POINTID*");

                    this.CreateWorkSheet(entityImportExportWorksheetAttribute, rootAttributeList, typeof(ProductClass));

                    this.ImportSelectedObjectWithDictionaryRelationshipsFromExcel(entityImportExportWorksheetAttribute, point);

                    this.xmldocumentcollection.Clear();
                }

                if (this.importPointCategories || this.importPointTypes)
                {
                    var applicationString = new ApplicationStringClass();

                    this.ImportSelectedObjectFromExcel("ID*", null, applicationString, typeof(IApplicationStrings));

                    this.xmldocumentcollection.Clear();
                }

            }
            finally
            {
                if (HttpContext.Current.Session.Timeout != sessionTimeout)
                {
                    HttpContext.Current.Session.Timeout = sessionTimeout;
                }
            }

        }
        #endregion

        #region Private methods

        private WSImportObject CreateWorkSheet(
           EntityImportExportWorksheetAttribute entityImportExportWorksheetAttribute,
           List<EntityImportExportAttribute> rootAttributeList,
           Type objectType)
        {
            WSImportObject worksheet = new WSImportObject(entityImportExportWorksheetAttribute.WorksheetName);

            worksheet.Site = this.site;
            worksheet.Security = this.security;
            worksheet.ImportException = this.impExpException;
            //worksheet.SiteInfo = this.SiteInfo;
            worksheet.RootAttributeList = new List<EntityImportExportAttribute>();
            foreach (var rootAttribute in rootAttributeList)
            {
                worksheet.RootAttributeList.Add(rootAttribute);
            }

            worksheet.RootAttributeList.Add(new EntityImportExportAttribute(entityImportExportWorksheetAttribute.RootId, 0));

            worksheet.CreateMemberHashTable(objectType);

            xmldocumentcollection.Add(worksheet);

            return worksheet;
        }




        public void ImportExcelRow(string rootID,
                                   string rootValue,
                                   XmlNodeList worksheetList,
                                   WSImportObject worksheet,
                                   object obj)
        {
            MemberInfo[] members = obj.GetType().GetMembers();

            foreach (MemberInfo member in members)
            {
                if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
                {
                    continue;
                }

                var worksheetAttributes = member.GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];
                EntityImportExportWorksheetAttribute collectionWorksheetAttribute = null;

                if (worksheetAttributes != null && worksheetAttributes.Length > 0)
                {
                    collectionWorksheetAttribute = worksheetAttributes[0];
                }

                var collectionImportExportAttributes = member.GetCustomAttributes(typeof(EntityImportExportAttribute), false) as EntityImportExportAttribute[];

                if (collectionWorksheetAttribute != null)
                {
                    object collection = worksheet.GetMemberValue(member, obj);
                    MethodInfo methodInfo = collection.GetType().GetMethod("Add");

                    if (methodInfo == null)
                    {
                        continue;
                    }

                    ConstructorInfo constructorInfo = null;
                    ParameterInfo[] parameterInfoArray = null;

                    parameterInfoArray = methodInfo.GetParameters();

                    IList list = null;
                    IDictionary dictionary = null;

                    if (collection is IList)
                    {
                        list = collection as IList;
                        list.Clear();

                        if (parameterInfoArray == null || parameterInfoArray.Length != 1)
                        {
                            continue;
                        }

                        // Test for Constructor that takes a SiteClass parameter
                        constructorInfo = parameterInfoArray[0].ParameterType.GetConstructor(new[] { typeof(SiteClass) });

                        if (constructorInfo == null || constructorInfo.IsPrivate)
                        {
                            constructorInfo = parameterInfoArray[1].ParameterType.GetConstructor(new Type[] { });
                        }
                    }

                    else if (collection is IDictionary)
                    {
                        dictionary = collection as IDictionary;
                        dictionary.Clear();

                        if (parameterInfoArray == null || parameterInfoArray.Length != 2)
                        {
                            continue;
                        }

                        // Test for Constructor that takes a SiteClass parameter
                        constructorInfo = parameterInfoArray[1].ParameterType.GetConstructor(new[] { typeof(SiteClass) });

                        if (constructorInfo == null || constructorInfo.IsPrivate)
                        {
                            constructorInfo = parameterInfoArray[1].ParameterType.GetConstructor(new Type[] { });
                        }
                    }

                    if (constructorInfo == null || constructorInfo.IsPrivate)
                    {
                        continue;
                    }


                    WSImportObject colworksheet = this.xmldocumentcollection.find(collectionWorksheetAttribute.WorksheetName);

                    worksheet.RootAttributeList[worksheet.RootAttributeList.Count - 1].Value = rootValue;

                    if (colworksheet == null)
                    {
                        colworksheet = this.CreateWorkSheet(collectionWorksheetAttribute, worksheet.RootAttributeList, constructorInfo.DeclaringType);

                        foreach (XmlNode colworksheetNode in worksheetList)
                        {
                            string colworksheetName = colworksheetNode.Attributes.Item(0).Value.ToUpper();

                            if (!colworksheetName.Equals((collectionWorksheetAttribute.WorksheetName.ToUpper())))
                            {
                                continue;
                            }

                            colworksheet.WorksheetNode = colworksheetNode;
                            colworksheet.NameSpaceManager = this.nameSpaceManager;
                            break;
                        }
                    }

                    colworksheet.ParseSheet();

                    while (colworksheet == null || colworksheet.RecordRows.Count > 0)
                    {
                        object collectionObject;

                        if (constructorInfo.GetParameters().Length == 0)
                        {
                            collectionObject = constructorInfo.Invoke(new object[] { });
                        }
                        else
                        {
                            collectionObject = constructorInfo.Invoke(new object[] { this.site });
                        }

                        this.ImportExcelRow(collectionWorksheetAttribute.RootId, colworksheet.GetRootData(collectionWorksheetAttribute.RootId), worksheetList, colworksheet, collectionObject);

                        if (list != null)
                        {
                            list.Add(collectionObject);
                        }
                        else if (dictionary != null)
                        {
                            if (parameterInfoArray[0].ParameterType == typeof(System.String))
                            {
                                dictionary.Add((collectionObject as BaseDataObject).IdentityGuid.ToString(), collectionObject);
                            }
                            else
                            {
                                dictionary.Add((collectionObject as BaseDataObject).IdentityGuid, collectionObject);
                            }
                        }

                        colworksheet.RecordRows.RemoveAt(0);
                    }

                    colworksheet.RootAttributeList[colworksheet.RootAttributeList.Count - 1].Value = string.Empty;
                }

                else
                {
                    if (collectionImportExportAttributes == null || collectionImportExportAttributes.Length == 0)
                    {
                        continue;
                    }


                    worksheet.ImportExcelMemberData(
                                            worksheet.RecordRows[0] as Hashtable,
                                            member,
                                            collectionImportExportAttributes,
                                            obj);
                }
            }
        }




        /// <summary>
        /// The import selected object from excel.
        /// </summary>
        /// <param name="rootId">
        /// The root ID.
        /// </param>
        /// <param name="worksheetAttribute">
        /// The worksheet attribute.
        /// </param>
        /// <param name="dataEntityObject">
        /// The data entity object.
        /// </param>
        /// <param name="businessEntityObject">
        /// The business entity object.
        /// </param>
        /// <exception cref="NullReferenceException">
        /// Null exception.
        /// </exception>
        /// <exception cref="FMInvalidEntityImportFileFormatException">
        /// Invalid entity import file.
        /// </exception>
        private void ImportSelectedObjectWithDictionaryRelationshipsFromExcel(
           EntityImportExportWorksheetAttribute worksheetAttribute,
           object dataEntityObject)
        {

            WSImportObject worksheet = this.xmldocumentcollection.find(worksheetAttribute.WorksheetName);

            if (this.entityDoc == null)
            {
                throw new NullReferenceException("entityDoc");
            }

            if (this.security == null)
            {
                throw new NullReferenceException("security");
            }

            if (this.site == null)
            {
                throw new NullReferenceException("site");
            }

            XmlNodeList worksheetList = this.entityDoc.SelectNodes("/ss:Workbook/ss:Worksheet", this.nameSpaceManager);
            XmlNodeList stylelist = this.entityDoc.SelectNodes("/ss:Workbook/ss:Styles/ss:Style", this.nameSpaceManager);
            worksheet.NumberFormatList.Clear();

            if (stylelist != null)
            {
                foreach (XmlNode styleNode in stylelist)
                {
                    if (styleNode.Attributes != null)
                    {
                        XmlAttribute attribId = styleNode.Attributes["ss:ID"];
                        if (attribId == null)
                        {
                            continue;
                        }
                    }

                    if (styleNode.Attributes != null)
                    {
                        string id = styleNode.Attributes["ss:ID"].Value;

                        var xmlNodeList = styleNode.SelectNodes("ss:NumberFormat", this.nameSpaceManager);

                        if (xmlNodeList != null)
                        {
                            XmlNode node = xmlNodeList.Item(0);
                            if (node == null)
                            {
                                continue;
                            }

                            if (node.Attributes != null)
                            {
                                XmlAttribute attrib = node.Attributes["ss:Format"];

                                if (attrib == null)
                                {
                                    continue;
                                }

                                worksheet.NumberFormatList.Add(id, attrib.Value);
                            }
                        }
                    }
                }
            }

            if (worksheetList == null || worksheetList.Count == 0)
            {
                throw new FMInvalidEntityImportFileFormatException();
            }

            foreach (XmlNode worksheetNode in worksheetList)
            {
                if (worksheetNode.Attributes != null)
                {
                    string worksheetName = worksheetNode.Attributes.Item(0).Value.ToUpper();

                    if (!worksheetName.Equals(worksheetAttribute.WorksheetName.ToUpper()))
                    {
                        continue;
                    }
                }

                worksheet.WorksheetNode = worksheetNode;
                worksheet.NameSpaceManager = this.nameSpaceManager;
                break;
            }

            worksheet.ParseSheet();
            worksheet.RootAttributeList[0].Value = worksheet.GetRootData(worksheet.RootAttributeList[0].ColumnName);
            string rootValue = worksheet.GetRootData(worksheetAttribute.RootId);
            MemberInfo[] members = dataEntityObject.GetType().GetMembers();

            while (!string.IsNullOrEmpty(rootValue))
            {
                try
                {
                    this.ImportExcelRow(worksheetAttribute.RootId, rootValue, worksheetList, worksheet, dataEntityObject);
                }
                catch (Exception e)
                {
                    if (worksheet.RecordRows.Count > 0)
                    {
                        worksheet.RecordRows.RemoveAt(0);
                    }

                    rootValue = worksheet.GetRootData(worksheetAttribute.RootId);

                    this.ImportException.AppendMessage(e.Message + " Skipping row.", ImportExportException.EXCEPTION_TYPES.ERROR);
                    continue;
                }
                // Within 30 seconds of executionTimeout, increase the timeout
                if (DateTime.Now - startTime > new TimeSpan(0, 0, HttpContext.Current.Server.ScriptTimeout - 30))
                {
                    HttpContext.Current.Server.ScriptTimeout += 30;
                }

                // Within 1 minute of Session.Timeout, increase the timeout
                if (DateTime.Now - startTime > new TimeSpan(0, HttpContext.Current.Session.Timeout - 1, 0))
                {
                    HttpContext.Current.Session.Timeout += 1;
                    FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(this.security));
                }


                if (dataEntityObject is Point)
                {
                    try
                    {
                        if (this.ImportPointTags)
                        {
                            FMChannelHelper.MakeCall<IPoints>(x => x.ModifyTagsOnly(this.security, (Point)dataEntityObject));
                            Point point = (Point)dataEntityObject;
                            this.ImportException.AppendMessage("Point: " + point.ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);
                        }
                        else
                        {
							      Point point = (Point)dataEntityObject;

                           if (includeStrapTables == false)
                           {
                              Guid strapTablePropertyGuid = Guid.Empty;
                              foreach (var kvp in point.Properties)
                              {
                                 if (kvp.Value.ID.ToUpper() == "STRAP TABLE")
                                 {
                                    strapTablePropertyGuid = kvp.Key;
                                    break;
                                 }
                              }

                              if (strapTablePropertyGuid != Guid.Empty)
                              {
                                 point.Properties.Remove(strapTablePropertyGuid);
                              }
                           }

							      FMChannelHelper.MakeCall<IPoints>(x => x.Import(this.security, point));
                           this.ImportException.AppendMessage("Point: " + point.ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);
                        }
                    }
                    catch (Exception e)
                    {
                        this.ImportException.AppendMessage(e.Message + ". Skipping row. ", ImportExportException.EXCEPTION_TYPES.ERROR);

                        if (worksheet.RecordRows.Count > 0) //consume the next row
                        {
                            worksheet.RecordRows.RemoveAt(0);
                        }

                        rootValue = worksheet.GetRootData(worksheetAttribute.RootId);
                        continue;
                    }
                }
                else if (dataEntityObject is PointTemplate)
                {
                    try
                    {
                        FMChannelHelper.MakeCall<IPointTemplates>(x => x.Import(this.security, (PointTemplate)dataEntityObject));
                        PointTemplate pointTemplate = (PointTemplate)dataEntityObject;
                        this.ImportException.AppendMessage("Point Template: " + pointTemplate.ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);
                    }
                    catch (Exception e)
                    {
                        this.ImportException.AppendMessage(e.Message + ". Skipping row. ", ImportExportException.EXCEPTION_TYPES.ERROR);

                        if (worksheet.RecordRows.Count > 0) //consume the next row
                        {
                            worksheet.RecordRows.RemoveAt(0);
                        }

                        rootValue = worksheet.GetRootData(worksheetAttribute.RootId);
                        continue;
                    }
                }
                else if (dataEntityObject is FMBusinessObjects.DataObjects.Module)
                {
                    try
                    {
                        string moduleImportResultText = FMChannelHelper.MakeCall<IModules, string>(x => x.Import(this.security, (FMBusinessObjects.DataObjects.Module)dataEntityObject));
                        FMBusinessObjects.DataObjects.Module module = (FMBusinessObjects.DataObjects.Module)dataEntityObject;
                        this.ImportException.AppendMessage("Module: " + moduleImportResultText, ImportExportException.EXCEPTION_TYPES.INFO);
                    }
                    catch (Exception e)
                    {
                        this.ImportException.AppendMessage(e.Message + ". Skipping row. ", ImportExportException.EXCEPTION_TYPES.ERROR);

                        if (worksheet.RecordRows.Count > 0) //consume the next row
                        {
                            worksheet.RecordRows.RemoveAt(0);
                        }

                        rootValue = worksheet.GetRootData(worksheetAttribute.RootId);
                        continue;
                    }
                }

                else if (dataEntityObject is FMBusinessObjects.DataObjects.ApplicationStringClass)
                {
                    try
                    {
                        // udpate the import to have two way guid parameter so we use the incoming guid
                         FMChannelHelper.MakeCall<IApplicationStrings>(x => x.Import(this.security, (ApplicationStringClass)dataEntityObject));
                        ApplicationStringClass appString = (ApplicationStringClass)dataEntityObject;
                        this.ImportException.AppendMessage("Application String: " + appString.ID + " was imported successfully", ImportExportException.EXCEPTION_TYPES.INFO);
                    }
                    catch (Exception e)
                    {
                        this.ImportException.AppendMessage(e.Message + ". Skipping row. ", ImportExportException.EXCEPTION_TYPES.ERROR);

                        if (worksheet.RecordRows.Count > 0) //consume the next row
                        {
                            worksheet.RecordRows.RemoveAt(0);
                        }

                        rootValue = worksheet.GetRootData(worksheetAttribute.RootId);
                        continue;
                    }
                }

                else if (dataEntityObject is FMBusinessObjects.DataObjects.AlarmPriorityClass)
                {
                    try
                    {
                        //create import function
                        Guid alarmPriorityImportResultGuid = FMChannelHelper.MakeCall<IAlarmPriorities, Guid>(x => x.Import(this.security, (FMBusinessObjects.DataObjects.AlarmPriorityClass)dataEntityObject));
                        //this.ImportException.AppendMessage("Module: " + moduleImportResultText, ImportExportException.EXCEPTION_TYPES.INFO);
                        FMBusinessObjects.DataObjects.AlarmPriorityClass alarmPriority = (FMBusinessObjects.DataObjects.AlarmPriorityClass)dataEntityObject;
                        this.ImportException.AppendMessage("AlarmPriority: " + alarmPriority.ID + " was imported successfully" , ImportExportException.EXCEPTION_TYPES.INFO);
                    }
                    catch (Exception e)
                    {
                        this.ImportException.AppendMessage(e.Message + ". Skipping row. ", ImportExportException.EXCEPTION_TYPES.ERROR);

                        if (worksheet.RecordRows.Count > 0) //consume the next row
                        {
                            worksheet.RecordRows.RemoveAt(0);
                        }

                        rootValue = worksheet.GetRootData(worksheetAttribute.RootId);
                        continue;
                    }
                }


                // reset the values in the object so we can reuse it
                if (dataEntityObject is Point)
                {
                    ((Point)dataEntityObject).Reset();
                }
                else if (dataEntityObject is PointTemplate)
                {
                    ((PointTemplate)dataEntityObject).Reset();
                }

                if (worksheet.RecordRows.Count > 0)
                {
                    worksheet.RecordRows.RemoveAt(0);
                }

                rootValue = worksheet.GetRootData(worksheetAttribute.RootId);
            }
        }

        /// <summary>
        /// The import selected object from excel.
        /// </summary>
        /// <param name="rootId">
        /// The root ID.
        /// </param>
        /// <param name="worksheetAttribute">
        /// The worksheet attribute.
        /// </param>
        /// <param name="dataEntityObject">
        /// The data entity object.
        /// </param>
        /// <param name="businessEntityInterface">
        /// The business entity object.
        /// </param>
        /// <exception cref="NullReferenceException">
        /// Null exception.
        /// </exception>
        /// <exception cref="FMInvalidEntityImportFileFormatException">
        /// Invalid entity import file.
        /// </exception>
        private void ImportSelectedObjectFromExcel(
           string rootId,
           EntityImportExportWorksheetAttribute worksheetAttribute,
           object dataEntityObject,
           Type businessEntityInterface)
        {
            string xmlworksheetname;

            if (worksheetAttribute == null)
            {
                var worksheetAttributes =
                   dataEntityObject.GetType().GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];

                if (worksheetAttributes == null || worksheetAttributes.Length == 0)
                {
                    return;
                }

                xmlworksheetname = worksheetAttributes[0].WorksheetName;
            }
            else
            {
                xmlworksheetname = worksheetAttribute.WorksheetName;
            }

            WSImportObject worksheet = this.xmldocumentcollection.find(xmlworksheetname);

            if (worksheet == null)
            {
                worksheet = new WSImportObject(xmlworksheetname)
                {
                    Site = this.site,
                    Security = this.security,
                    ImportException = this.impExpException,
                    //SiteInfo = this.SiteInfo
                };

                this.xmldocumentcollection.Add(worksheet);
            }

            if (this.entityDoc == null)
            {
                throw new NullReferenceException("entityDoc");
            }

            if (this.security == null)
            {
                throw new NullReferenceException("security");
            }

            if (this.site == null)
            {
                throw new NullReferenceException("site");
            }

            XmlNodeList worksheetList = this.entityDoc.SelectNodes("/ss:Workbook/ss:Worksheet", this.nameSpaceManager);
            XmlNodeList stylelist = this.entityDoc.SelectNodes("/ss:Workbook/ss:Styles/ss:Style", this.nameSpaceManager);
            worksheet.NumberFormatList.Clear();

            if (stylelist != null)
            {
                foreach (XmlNode styleNode in stylelist)
                {
                    if (styleNode.Attributes != null)
                    {
                        XmlAttribute attribId = styleNode.Attributes["ss:ID"];
                        if (attribId == null)
                        {
                            continue;
                        }
                    }

                    if (styleNode.Attributes != null)
                    {
                        string id = styleNode.Attributes["ss:ID"].Value;

                        var xmlNodeList = styleNode.SelectNodes("ss:NumberFormat", this.nameSpaceManager);

                        XmlNode node = xmlNodeList?.Item(0);

                        if (node?.Attributes != null)
                        {
                            XmlAttribute attrib = node.Attributes["ss:Format"];

                            if (attrib == null)
                            {
                                continue;
                            }

                            worksheet.NumberFormatList.Add(id, attrib.Value);
                        }
                    }
                }
            }

            if (worksheetList == null || worksheetList.Count == 0)
            {
                throw new FMInvalidEntityImportFileFormatException();
            }

            foreach (XmlNode worksheetNode in worksheetList)
            {
                if (worksheetNode.Attributes != null)
                {
                    string worksheetName = worksheetNode.Attributes.Item(0).Value.ToUpper();

                    if (!worksheetName.Equals(xmlworksheetname.ToUpper()))
                    {
                        continue;
                    }
                }

                worksheet.WorksheetNode = worksheetNode;
                worksheet.NameSpaceManager = this.nameSpaceManager;
                break;
            }

            worksheet.ParseSheet(null, null);
            string rootValue = worksheet.GetRootData(rootId);
            MemberInfo[] members = dataEntityObject.GetType().GetMembers();

            while (rootValue != string.Empty && rootValue.Length > 0)
            {
                foreach (MemberInfo member in members)
                {
                    if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
                    {
                        continue;
                    }

                    var worksheetAttributes =
                       member.GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];
                    EntityImportExportWorksheetAttribute collectionWorksheetAttribute = null;

                    if (worksheetAttributes != null && worksheetAttributes.Length > 0)
                    {
                        collectionWorksheetAttribute = worksheetAttributes[0];
                    }

                    var collectionImportExportAttributes =
                       member.GetCustomAttributes(typeof(EntityImportExportAttribute), false) as EntityImportExportAttribute[];

                    if (collectionImportExportAttributes == null || collectionImportExportAttributes.Length == 0)
                    {
                        continue;
                    }

                    if (collectionWorksheetAttribute != null)
                    {
                        object collection = worksheet.GetMemberValue(member, dataEntityObject);

                        MethodInfo methodInfo = collection?.GetType().GetMethod("Add");

                        ParameterInfo[] parameterInfoArray = methodInfo?.GetParameters();

                        if (parameterInfoArray == null || parameterInfoArray.Length != 1)
                        {
                            continue;
                        }


                        // Get the Type of items that will be added to the list
                        // parameterInfoArray[0].ParameterType may be an abstract type so we allow overide from collectionWorksheetAttribute
                        Type listType = collectionWorksheetAttribute.TypeOfListItem ?? parameterInfoArray[0].ParameterType;

                        // Test for Constructor that takes a SiteClass parameter
                        ConstructorInfo constructorInfo = listType.GetConstructor(new[] { typeof(SiteClass) });

                        if (constructorInfo == null || constructorInfo.IsPrivate)
                        {
                            constructorInfo = listType.GetConstructor(new Type[] { });
                        }

                        if (constructorInfo == null || constructorInfo.IsPrivate)
                        {
                            continue;
                        }

                        var list = collection as IList;
                        list.Clear();

                        string colxmlworksheetname = collectionWorksheetAttribute.WorksheetName;
                        WSImportObject colworksheet = this.xmldocumentcollection.find(colxmlworksheetname);

                        if (colworksheet == null)
                        {
                            colworksheet = new WSImportObject(collectionWorksheetAttribute.WorksheetName)
                            {
                                Site = this.site,
                                Security = this.security,
                                ImportException = this.impExpException,
                                //SiteInfo = this.SiteInfo
                            };

                            this.xmldocumentcollection.Add(colworksheet);
                        }

                        foreach (XmlNode colworksheetNode in worksheetList)
                        {
                            string colworksheetName = colworksheetNode.Attributes.Item(0).Value.ToUpper();

                            if (!colworksheetName.Equals(colxmlworksheetname.ToUpper()))
                            {
                                continue;
                            }

                            colworksheet.WorksheetNode = colworksheetNode;
                            colworksheet.NameSpaceManager = this.nameSpaceManager;
                            break;
                        }

                        colworksheet.ParseSheet(rootId, rootValue);

                        while (colworksheet.RecordRows.Count > 0)
                        {
                            object collectionObject;

                            if (constructorInfo.GetParameters().Length == 0)
                            {
                                collectionObject = constructorInfo.Invoke(new object[] { });
                            }
                            else
                            {
                                collectionObject = constructorInfo.Invoke(new object[] { this.site });
                            }

                            try
                            {
                                colworksheet.ImportExcelRow(colworksheet.RecordRows[0] as Hashtable, collectionObject, collectionImportExportAttributes);
                                list.Add(collectionObject);
                            }
                            catch (CompanyRoleMapCollectionException e)
                            { 
                                var comp = dataEntityObject as CompanyClass;
                                var message = "Company "+comp.ID + ": " + e.Message;
                                AddAlarmAndEventLog(message); 
                                ImportException.AppendMessage(message, ImportExportException.EXCEPTION_TYPES.WARNING);
                            }
                            colworksheet.RecordRows.RemoveAt(0);
                        }
                    }
                    else
                    {
                        worksheet.ImportExcelMemberData(
                                                worksheet.RecordRows[0] as Hashtable,
                                                member,
                                                collectionImportExportAttributes,
                                                dataEntityObject);
                    }
                }

                // Within 30 seconds of executionTimeout, increase the timeout
                if (DateTime.Now - this.startTime > new TimeSpan(0, 0, HttpContext.Current.Server.ScriptTimeout - 30))
                {
                    HttpContext.Current.Server.ScriptTimeout += 30;
                }

                // Within 1 minute of Session.Timeout, increase the timeout
                if (DateTime.Now - this.startTime > new TimeSpan(0, HttpContext.Current.Session.Timeout - 1, 0))
                {
                    HttpContext.Current.Session.Timeout += 1;
                    FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(this.security));
                }

                try { 
                if (businessEntityInterface == typeof(ICompanies))
                {
                    this.ValidateSiteMatches(((CompanyClass)dataEntityObject).SiteID, ((CompanyClass)dataEntityObject).SiteGuid);
                    FMChannelHelper.MakeCall<ICompanies>(x => x.Import(this.security, (CompanyClass)dataEntityObject));
                    this.ImportException.AppendMessage("Company: " + ((CompanyClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);
                }
                else
                {
                    if (businessEntityInterface == typeof(IEquipments))
                    {
                        this.ValidateSiteMatches(((EquipmentClass)dataEntityObject).SiteID, ((EquipmentClass)dataEntityObject).SiteGuid);
                        FMChannelHelper.MakeCall<IEquipments>(x => x.Import(this.security, (EquipmentClass)dataEntityObject));
                            this.ImportException.AppendMessage("Equipment: " + ((EquipmentClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);

                        }
                        else
                    {
                        if (businessEntityInterface == typeof(IPersonnel))
                        {
                            this.ValidateSiteMatches(((PersonClass)dataEntityObject).SiteID, ((PersonClass)dataEntityObject).SiteGuid);
                            FMChannelHelper.MakeCall<IPersonnel>(x => x.Import(this.security, (PersonClass)dataEntityObject));
                                this.ImportException.AppendMessage("Personnel: " + ((PersonClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);

                            }
                            else
                        {
                            if (businessEntityInterface == typeof(IProducts))
                            {
                                this.ValidateSiteMatches(((ProductClass)dataEntityObject).SiteID, ((ProductClass)dataEntityObject).SiteGuid);
                                FMChannelHelper.MakeCall<IProducts>(x => x.Import(this.security, (ProductClass)dataEntityObject));
                                    this.ImportException.AppendMessage("Product: " + ((ProductClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);

                                }
                                else
                            {
                                if (businessEntityInterface == typeof(IStandingOffers))
                                {
                                    FMChannelHelper.MakeCall<IStandingOffers>(x => x.ImportWithStandingOffer(this.security, (StandingOfferClass)dataEntityObject));
                                        this.ImportException.AppendMessage("Standing Offer: " + ((StandingOfferClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);

                                    }
                                    else
                                {
                                    if (businessEntityInterface == typeof(IFuelCards))
                                    {
                                        FMChannelHelper.MakeCall<IFuelCards>(x => x.Import(this.security, (FuelCardClass)dataEntityObject));
                                            this.ImportException.AppendMessage("Fuel Card: " + ((FuelCardClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);

                                        }
                                        else
                                    {
                                        if (businessEntityInterface == typeof(IIATACodes))
                                        {
                                            FMChannelHelper.MakeCall<IIATACodes>(x => x.Import(this.security, (IATACodeClass)dataEntityObject));
                                                this.ImportException.AppendMessage("IIATA Code: " + ((IATACodeClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);

                                            }
                                            else
                                        {
                                            if (businessEntityInterface == typeof(IEquipmentTypes))
                                            {
                                                this.ValidateSiteMatches(((EquipmentTypeClass)dataEntityObject).SiteID, ((EquipmentTypeClass)dataEntityObject).SiteGuid);
                                                FMChannelHelper.MakeCall<IEquipmentTypes>(x => x.Import(this.security, (EquipmentTypeClass)dataEntityObject));
                                                    this.ImportException.AppendMessage("Equipment Types: " + ((EquipmentTypeClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);

                                                }
                                                else
                                                if (businessEntityInterface == typeof(IEntityToSiteMaps))
                                                {
                                                FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Import(this.security, (EntityToSiteMapClass)dataEntityObject));
                                                    this.ImportException.AppendMessage("Entity To Site Map: " + ((EntityToSiteMapClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);

                                                }
                                                else if (businessEntityInterface == typeof(IApplicationStrings))
                                                { 
                                                FMChannelHelper.MakeCall<IApplicationStrings>(x => x.Import(this.security, (ApplicationStringClass)dataEntityObject));
                                                this.ImportException.AppendMessage("Application String: " + ((ApplicationStringClass)dataEntityObject).ID + " was imported successfully.", ImportExportException.EXCEPTION_TYPES.INFO);
                                                }

                                            }
                                        }
                                }
                            }
                        }
                    }
                }
                }
                catch (Exception e)
                {
                    this.ImportException.AppendMessage(e.Message + ". Skipping row. ", ImportExportException.EXCEPTION_TYPES.ERROR);
                }

                   // reset the values in the object so we can reuse it
                   // Reset is a virtual method in the base class with overrides in all of the derived classes;
                   // no need for explicit calls
                   (dataEntityObject as BaseDataObject)?.Reset();

                if (worksheet.RecordRows.Count > 0)
                {
                    worksheet.RecordRows.RemoveAt(0);
                }

                rootValue = worksheet.GetRootData(rootId);
            }
        }

        /// <summary>
        /// This method checks to ensure that the Site ID in the entity
        /// matches the current Site ID. An exception is thrown if not.
        /// </summary>
        /// <param name="siteId">
        /// The site ID.
        /// </param>
        /// <param name="siteGuid">
        /// The site GUID.
        /// </param>
        /// <exception cref="Exception">
        /// Invalid Site ID.
        /// </exception>
        private void ValidateSiteMatches(string siteId, Guid siteGuid)
        {
            if (string.IsNullOrEmpty(siteId))
            {
                if (siteGuid == Guid.Empty)
                {
                    throw new Exception("Invalid Site ID.");
                }

                if (this.site.SiteGuid != siteGuid)
                {
                    throw new Exception("Site ID does not match the current site.");
                }
            }
            else if (this.site.SiteID.Equals(siteId) == false)
            {
                throw new Exception("Site ID does not match the current site.");
            }
        }

        /// <summary>
        /// This method initialize the excel import object to its initial state.
        /// </summary>
        /// <param name="inSecurity">Security object.</param>
        /// <param name="inSite">Site object.</param>
        /// <param name="inEntityDoc">Entity document.</param>
        private void Initialize(SecurityClass inSecurity, SiteClass inSite, XmlDocument inEntityDoc)
        {
            this.security = inSecurity;
            this.site = inSite;
            this.importCompanies = false;
            this.importEquipment = false;
            this.importPersonnel = false;
            this.importProducts = false;
            this.importStandingOffers = false;
            this.importFuelCard = false;
            this.importIATACodes = false;
            this.ImportAssignments = false;
            this.ImportPoints = false;
            this.ImportPointTemplates = false;
			   this.includeStrapTables = false;
			   this.impExpException = new ImportExportException(null, ImportExportException.EXCEPTION_TYPES.NONE);




            //var sitesInfoClient = new FMChannelFactory<ISitesInfo>();
            //ISitesInfo sitesInfo = sitesInfoClient.CreateProxy();
            //this.SiteInfo = sitesInfo.RefreshSiteInfo(inSecurity);
            this.SiteInfo = FMChannelHelper.MakeCall<ISitesInfo, SiteInfoDO>(x => x.RefreshSiteInfo(inSecurity));

            if (inEntityDoc == null)
            {
                this.entityDoc = new XmlDocument();
            }
            else
            {
                this.entityDoc = inEntityDoc;
                this.nameSpaceManager = new XmlNamespaceManager(this.entityDoc.NameTable);

                this.nameSpaceManager.AddNamespace(string.Empty, "urn:schemas-microsoft-com:office:spreadsheet");
                this.nameSpaceManager.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
                this.nameSpaceManager.AddNamespace("o", "urn:schemas-microsoft-com:office:office");
                this.nameSpaceManager.AddNamespace("html", "http://www.w3.org/TR/REC-html40");
                this.nameSpaceManager.AddNamespace("x", "urn:schemas-microsoft-com:office:excel");
            }
        }

        private void AddAlarmAndEventLog(string message)
        {
            AlarmAndEventLogClass eventNotification = new AlarmAndEventLogClass();
            eventNotification.Source = "Entity Import";
            eventNotification.AssociatedData = $"Entity Import {message}";
            eventNotification.ID = "Entity Import";
            FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventChannel =>
            {
                alarmAndEventChannel.Add(security, eventNotification);
            });
        }
        #endregion
    }
}
