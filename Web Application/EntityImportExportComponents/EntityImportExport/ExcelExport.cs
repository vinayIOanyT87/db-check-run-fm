/******************************************************************************
	FILE NAME:		ExcelExport.cs
	PURPOSE:		Excel Export

	COMMENTS:
		Copyright (C) Varec, Inc. Norcross, GA, USA, 2008
		This file shall not be copied or reproduced in any form without
		the express written consent of Varec.

	AUTHOR(S):		Richard Panachida
	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:				By:					Reason:
		------------	-----------------	-------------------------------------------
		2008-04-14		B. Nelson			Added product blend components export.

		2008-04-16		I.Orndorff			- Remove unused "Description" from the 
													  personnelmap. Adjusted range to
													  compensate for one less column.

		2008-04-16		B. Nelson			- Removed personnel schema and map data.
		2008-04-24		B. Nelson			- Added CompanyCollection and CompanyObjectList
													  to company export for improved performance.
		2008-08-25		A. Coker				- Added standing offer entity export. 
		2008-10-25		A. Coker				- Added fuel common request entity export. 
		2008-11-18		A. Coker				- Added turnover period entity export. 
		2009-03-26		I.Orndorff			- Added "ExportIATACodes()" and "BuildIATACodesExport()". 
													  This addresses change request 2434. 										  
											  
		2008-06-05		I.Orndorff			- Modified "BuildProductsExport()" to include
													 WSExportProductCustomers.	This fixes CSI #5842.
		2008-12-04		I.Orndorff			- Modified "BuildPersonnelExport()" to include 
													 WSExportPersonnelAccessSchedules. This fixes 
													 CSI #5913.
 
	  2009-09-22     A. Coker          - WI 5824 : Added new fields to export.

*******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace EntityImportExport
{
	public class ExcelExport
	{
		#region Private data members
		private XMLExportDocumentCollectionClass xmldocumentcollection = new XMLExportDocumentCollectionClass();
		private SecurityClass security;
		private SiteClass site;
		private ImportExportException impExpException;
		private bool exportCompanies;
		private bool exportEquipment;
		private bool exportPersonnel;
		private bool exportProducts;
		private bool exportStandingOffers;
		private bool exportFuelCard;
		private bool exportIATACodes;
		private bool exportEquipmentTypes;
		private bool exportAssignments;
		private bool exportPoints;
		private bool exportPointTemplates;
		private bool exportPointCategories;
		private bool exportPointTypes;
		private bool exportPointTags;
		private string headerStart;
		private string headerEnd;
		private string excelXml;
		private bool includeStrapTables;
		private List<KeyValuePair<string, string>> rootIDList;

		private const string EXPORT_MSG_COMPANIES_SUCCESS = "Done exporting Companies.";
		private const string EXPORT_MSG_EQUIPMENT_SUCCESS = "Done exporting Equipment.";
		private const string EXPORT_MSG_PERSONNEL_SUCCESS = "Done exporting Personnel.";
		private const string EXPORT_MSG_PRODUCTS_SUCCESS = "Done exporting Products.";
		private const string EXPORT_MSG_STANDING_OFFERS_SUCCESS = "Done exporting Price List.";
		private const string EXPORT_MSG_FUEL_CARD_SUCCESS = "Done exporting Fuel Cards.";
		private const string EXPORT_MSG_IATA_CODES_SUCCESS = "Done exporting Delivery Locations.";
		private const string EXPORT_MSG_EQUIPMENT_TYPES_SUCCESS = "Done exporting Equipment Types.";
		private const string EXPORT_MSG_ASSIGNMENTS_SUCCESS = "Done exporting Assignments.";
		private const string EXPORT_MSG_POINTS_SUCCESS = "Done exporting Points.";
		private const string EXPORT_MSG_POINTTEMPLATES_SUCCESS = "Done exporting Point Templates.";
		private const string EXPORT_MSG_POINT_CATEGORIES_SUCCESS = "Done exporting Point Categories.";
		private const string EXPORT_MSG_POINT_TYPES_SUCCESS = "Done exporting Point Types.";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the excel export class.
		/// </summary>
		public ExcelExport(SecurityClass security, SiteClass site)
		{
			this.Initialize(security, site);
		}
		#endregion

		#region Properties
		public SiteInfoDO SiteInfo
		{
			get;
			set;
		}

		public string ExcelXMLDocument
		{
			get { return this.excelXml; }
		}

		public bool ExportCompanies
		{
			get { return this.exportCompanies; }
			set { this.exportCompanies = value; }
		}

		public bool ExportEquipment
		{
			get { return this.exportEquipment; }
			set { this.exportEquipment = value; }
		}

		public bool ExportPersonnel
		{
			get { return this.exportPersonnel; }
			set { this.exportPersonnel = value; }
		}

		public bool ExportProducts
		{
			get { return this.exportProducts; }
			set { this.exportProducts = value; }
		}

		public bool ExportStandingOffers
		{
			get { return this.exportStandingOffers; }
			set { this.exportStandingOffers = value; }
		}

		public bool ExportFuelCard
		{
			get { return this.exportFuelCard; }
			set { this.exportFuelCard = value; }
		}

		public bool ExportIATACodes
		{
			get { return this.exportIATACodes; }
			set { this.exportIATACodes = value; }
		}

		public bool ExportEquipmentTypes
		{
			get { return this.exportEquipmentTypes; }
			set { this.exportEquipmentTypes = value; }
		}

		public bool ExportAssignments
		{
			get { return this.exportAssignments; }
			set { this.exportAssignments = value; }
		}

		public bool ExportPoints
		{
			get { return this.exportPoints; }
			set { this.exportPoints = value; }
		}
		public bool ExportPointTemplates
		{
			get { return this.exportPointTemplates; }
			set { this.exportPointTemplates = value; }
		}
		public bool ExportPointCategories
		{
			get { return this.exportPointCategories; }
			set { this.exportPointCategories = value; }
		}
		public bool ExportPointTypes
		{
			get { return this.exportPointTypes; }
			set { this.exportPointTypes = value; }
		}

		  public bool ExportPointTags
		  { get { return this.exportPointTags; }
				set { this.exportPointTags = value; }
		  }

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
		/// This method will add a worksheet to the Excel XML document.
		/// </summary>
		/// <param name="worksheet"></param>
		public void AddWorksheet(string worksheet)
		{
			if ((worksheet != null) && (worksheet.Length > 0))
			{
				this.excelXml = this.excelXml + worksheet;
			}
		}

		public void Export()
		{
			// Build company export if requested.
			if (this.exportCompanies == true)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("COMPANYID*", ref rootAttribute, typeof(CompanyClass));

				foreach (CompanyClass company in FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(x => x.EnumerateExtPrime(this.security, false, true, true)))
				{
					if (company.SiteGuid != this.security.SiteGuid)
					{
						continue;
					}

					AddObjectToWorkSheets("COMPANYID*", ref rootAttribute, null, null, company);
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build equipment export if requested.
			if (this.exportEquipment == true)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("EQUIPMENTID*", ref rootAttribute, typeof(EquipmentClass));

				foreach (EquipmentInfo equipmentInfo in FMChannelHelper.MakeCall<IEquipments, EquipmentInfo[]>(x => x.EnumerateInfo(this.security)))
				{
					if (equipmentInfo.siteGuid != this.security.SiteGuid)
					{
						continue;
					}

					AddObjectToWorkSheets("EQUIPMENTID*", ref rootAttribute, null, null, FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.security, equipmentInfo.identityGuid)));
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build Personnel export if requested.
			if (this.exportPersonnel == true)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("PERSONID*", ref rootAttribute, typeof(PersonClass));

				foreach (PersonClass person in FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(x => x.Enumerate(this.security)))
				{
					if (person.SiteGuid != this.security.SiteGuid)
					{
						continue;
					}

					AddObjectToWorkSheets("PERSONID*", ref rootAttribute, null, null, FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.security, person.IdentityGuid)));
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build Products export if requested.
			if (this.exportProducts == true)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("PRODUCTID*", ref rootAttribute, typeof(ProductClass));

				foreach (ProductClass product in FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.security)))
				{
					if (product.SiteGuid != this.security.SiteGuid)
					{
						continue;
					}

					AddObjectToWorkSheets("PRODUCTID*", ref rootAttribute, null, null, FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(this.security, product.IdentityGuid)));
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build Standing Offers (aka Price List) export if requested.
			if (this.exportStandingOffers)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("STANDINGOFFERID*", ref rootAttribute, typeof(StandingOfferClass));

				foreach (StandingOfferClass standingOffer in FMChannelHelper.MakeCall<IStandingOffers, StandingOfferCollectionClass>(x => x.Enumerate(this.security)))
				{
					if (standingOffer.SiteGuid != this.security.SiteGuid)
					{
						continue;
					}

					AddObjectToWorkSheets("STANDINGOFFERID*", ref rootAttribute, null, null, FMChannelHelper.MakeCall<IStandingOffers, StandingOfferClass>(x => x.Get(this.security, standingOffer.IdentityGuid)));
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build Fuel Common Request export if requested.
			if (this.exportFuelCard)
			{
				EntityImportExportAttribute rootAttribute = null;

				FuelCardCollectionClass fuelCardCollection =
							FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(x => x.EnumerateFuelCardsForEntityExport(this.security));

				CreateWorkSheets("FUELCARDID*", ref rootAttribute, typeof(FuelCardClass));

				foreach (FuelCardClass fuelCard in fuelCardCollection)
				{
					if (fuelCard.SiteGuid != this.security.SiteGuid)
					{
						continue;
					}

					AddObjectToWorkSheets("FUELCARDID*", ref rootAttribute, null, null, fuelCard);
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build IATA codes export if requested.
			if (this.exportIATACodes)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("IATACODEID*", ref rootAttribute, typeof(IATACodeClass));

				foreach (IATACodeClass iataCode in FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(x => x.Enumerate(this.security)))
				{
					if (iataCode.SiteGuid != this.security.SiteGuid)
					{
						continue;
					}

					AddObjectToWorkSheets("IATACODEID*", ref rootAttribute, null, null, FMChannelHelper.MakeCall<IIATACodes, IATACodeClass>(x => x.Get(this.security, iataCode.IdentityGuid)));
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build Equipment Types export if requested.
			if (this.ExportEquipmentTypes)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("TYPECLASSID*", ref rootAttribute, typeof(EquipmentTypeClass));

				foreach (EquipmentTypeClass equipmentType in FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeCollectionClass>(x => x.Enumerate(this.security, null, null)))
				{
					if (equipmentType.SiteGuid != this.security.SiteGuid)
					{
						continue;
					}

					AddObjectToWorkSheets("TYPECLASSID*", ref rootAttribute, null, null, FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(x => x.Get(this.security, equipmentType.IdentityGuid)));
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build Assignements export if requested.
			if (this.exportAssignments)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("ASSIGNEDID*", ref rootAttribute, typeof(EntityToSiteMapClass));
				ENTITY_TYPE[] entityTypes = { ENTITY_TYPE.EQUIPMENT,
												ENTITY_TYPE.FUEL_CARD,
												ENTITY_TYPE.PRODUCT,
												ENTITY_TYPE.EQUIPMENT_TYPE,
												ENTITY_TYPE.COMPANY,
												ENTITY_TYPE.PERSONNEL,
												ENTITY_TYPE.STANDING_OFFER,
												ENTITY_TYPE.IATA_CODE
											 };

				foreach (ENTITY_TYPE entityType in entityTypes)
				{
					foreach (EntityToSiteMapClass entityToSiteMap in FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapCollectionClass>(x => x.EnumerateEntityMapsByAssignedFromSiteGuid(this.security, entityType, this.security.SiteGuid)))
					{
						AddObjectToWorkSheets("ASSIGNEDID*", ref rootAttribute, null, null, entityToSiteMap);
					}
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Build Equipment Types export if requested.
			if (this.exportPoints)
			{
				PointCollection points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateBySite(this.security, this.security.SiteGuid));
				List<EntityImportExportAttribute> rootAttributeList = new List<EntityImportExportAttribute>();
				rootAttributeList.Add(new EntityImportExportAttribute("SITE*", 100, "SiteGuid", this.security.SiteID));
				EntityImportExportWorksheetAttribute entityImportExportWorksheetAttribute = new EntityImportExportWorksheetAttribute("POINTS", "POINTID*");

				this.CreateWorkSheetWithDictionaryRelationship(entityImportExportWorksheetAttribute, rootAttributeList, typeof(Point));

				foreach (Point point in points)
				{
					AddObjectToWorkSheet(entityImportExportWorksheetAttribute, point);
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			if (this.exportPointTemplates)
			{
            // create lists for supporting objects: Modules, Alarm categories, and Alarm priorities
				List <FMBusinessObjects.DataObjects.Module> ListofModules = new List<FMBusinessObjects.DataObjects.Module>();
            Dictionary <Guid, ApplicationStringClass> ListofAlarmCategories = new Dictionary<Guid, ApplicationStringClass>();
            Dictionary <Guid, AlarmPriorityClass> ListofAlarmPriorities = new Dictionary<Guid, AlarmPriorityClass>();

				PointTemplateCollection pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(x => x.EnumerateByType(this.security, null));
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

            this.CreateWorkSheetWithDictionaryRelationship(entityImportExportWorksheetAttributePts, rootAttributeListPts, typeof(PointTemplate));
				this.CreateWorkSheetWithDictionaryRelationship(entityImportExportWorksheetAttributeModules, rootAttributeListModules, typeof(FMBusinessObjects.DataObjects.Module));
            this.CreateWorkSheetWithDictionaryRelationship(entityImportExportWorksheetAttributeAlarmCategories, rootAttributeListAlarmCategories, typeof(ApplicationStringClass));
            this.CreateWorkSheetWithDictionaryRelationship(entityImportExportWorksheetAttributeAlarmPriorities, rootAttributeListAlarmPriorities, typeof(AlarmPriorityClass));

            foreach (PointTemplate pointTemplate in pointTemplates)
				{
					if(pointTemplate.Standard)
					{
						continue;
					}

               // populate list of modules
					foreach (var module in pointTemplate.Modules)
					{
						if (module.Value.Standard == false && !ListofModules.Any(i=>i.IdentityGuid == module.Key))
						{
							ListofModules.Add(module.Value);
						}
					}

					// populate dictionary of alarm categories
					foreach (var ptTag in pointTemplate.Tags)
					{
						foreach (var alarmTemplate in ptTag.Value.AlarmTemplates)
						{
                     if (!ListofAlarmCategories.ContainsKey(alarmTemplate.Value.AlarmCategoryApplicationStringGuid))
                     {
								// don't export the default alarm category
								if ( alarmTemplate.Value.AlarmCategoryApplicationStringGuid != new Guid("{512ab266-b3b8-4a29-b8d9-594795cf63ed}"))
								{
									ListofAlarmCategories.Add(alarmTemplate.Value.AlarmCategoryApplicationStringGuid, FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(x => x.Get(this.security, alarmTemplate.Value.AlarmCategoryApplicationStringGuid)));

								}

							}

							// populate dictionary of alarm priorities
							foreach (var alarmTestTemplate in alarmTemplate.Value.AlarmTestTemplates)
							{
								if (!ListofAlarmPriorities.ContainsKey(alarmTestTemplate.Value.AlarmPriorityGuid))
								{
									// do not include the default alarm priorities in the export
									if (alarmTestTemplate.Value.AlarmPriorityGuid != new Guid("{aa9e557c-a652-4caf-9bca-2bcb9ab5b104}") // HiHiLoLoAlarmPriority
											&& alarmTestTemplate.Value.AlarmPriorityGuid != new Guid("{BA35E686-5CCE-402D-982B-18D45958CCB6}") //HighLowAlarmPriority
										&& alarmTestTemplate.Value.AlarmPriorityGuid != new Guid("{402A7722-062B-42F6-B6A5-E6180E2BA2B8}") // MaxMinOperatingAlarmPriority
										&& alarmTestTemplate.Value.AlarmPriorityGuid != new Guid("{5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f}")) // NormalUnacknowledgedAlarmPriority
									{ 
										ListofAlarmPriorities.Add(alarmTestTemplate.Value.AlarmPriorityGuid, FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityClass>(x => x.Get(this.security, alarmTestTemplate.Value.AlarmPriorityGuid)));
									}
								}
								if (!ListofAlarmPriorities.ContainsKey(alarmTestTemplate.Value.NormalUnacknowledgedAlarmPriorityGuid))
								{
									// do not include the default alarm priorities in the export
									if (alarmTestTemplate.Value.NormalUnacknowledgedAlarmPriorityGuid != new Guid("{aa9e557c-a652-4caf-9bca-2bcb9ab5b104}") // HiHiLoLoAlarmPriority
											&& alarmTestTemplate.Value.NormalUnacknowledgedAlarmPriorityGuid != new Guid("{BA35E686-5CCE-402D-982B-18D45958CCB6}") //HighLowAlarmPriority
										&& alarmTestTemplate.Value.NormalUnacknowledgedAlarmPriorityGuid != new Guid("{402A7722-062B-42F6-B6A5-E6180E2BA2B8}") // MaxMinOperatingAlarmPriority
										&& alarmTestTemplate.Value.NormalUnacknowledgedAlarmPriorityGuid != new Guid("{5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f}")) // NormalUnacknowledgedAlarmPriority
									{
										ListofAlarmPriorities.Add(alarmTestTemplate.Value.NormalUnacknowledgedAlarmPriorityGuid, FMChannelHelper.MakeCall<IAlarmPriorities, AlarmPriorityClass>(x => x.Get(this.security, alarmTestTemplate.Value.NormalUnacknowledgedAlarmPriorityGuid)));
									}
								}
							}
						}
					}

					if (!(pointTemplate.IdentityGuid == FMBusinessObjects.Constants.Guids.VerticalTankTemplateGuid)) // cannot export the standard tank template
					{
							AddObjectToWorkSheet(entityImportExportWorksheetAttributePts, pointTemplate);
					}
				}

				ListofModules = ListofModules.Distinct().ToList();

				foreach (var module in ListofModules)
				{
					AddObjectToWorkSheet(entityImportExportWorksheetAttributeModules, module);
				}

				foreach (var alarmCategory in ListofAlarmCategories)
				{
					AddObjectToWorkSheet(entityImportExportWorksheetAttributeAlarmCategories, alarmCategory.Value);
				}

				foreach (var alarmPriority in ListofAlarmPriorities)
				{
					AddObjectToWorkSheet(entityImportExportWorksheetAttributeAlarmPriorities, alarmPriority.Value);
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

         if (this.ExportPointTags)
         {
            PointCollection points = FMChannelHelper.MakeCall<IPoints, PointCollection>(x => x.EnumerateBySite(this.security, this.security.SiteGuid));
            List<EntityImportExportAttribute> rootAttributeList = new List<EntityImportExportAttribute>();
            rootAttributeList.Add(new EntityImportExportAttribute("SITE*", 100, "SiteGuid", this.security.SiteID));
            EntityImportExportWorksheetAttribute entityImportExportWorksheetAttribute = new EntityImportExportWorksheetAttribute("POINTS", "POINTID*");

            this.CreateWorkSheetWithDictionaryRelationship(entityImportExportWorksheetAttribute, rootAttributeList, typeof(Point));

            foreach (Point point in points)
            {
					point.Properties.Clear(); //only want the tags for this export
					foreach (PointTag tag in point.Tags.Values)
					{
						tag.Alarms.Clear();
					}
               AddObjectToWorkSheet(entityImportExportWorksheetAttribute, point);
            }

            foreach (WSExportObject worksheet in xmldocumentcollection)
            {
               this.AddWorksheet(worksheet.WorksheetXML);
            }

            xmldocumentcollection.Clear();
         }
         if (this.exportPointCategories)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("ID*", ref rootAttribute, typeof(ApplicationStringClass));

				foreach (ApplicationStringClass pointCategory in FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.security, STRING_TYPE.POINT_CATEGORY)))
				{
					AddObjectToWorkSheets("ID*", ref rootAttribute, null, null, pointCategory);
				}

				if (this.exportPointTypes)
				{
					foreach (ApplicationStringClass pointCategory in FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.security, STRING_TYPE.POINT_TEMPLATE_TYPE)))
					{
						AddObjectToWorkSheets("ID*", ref rootAttribute, null, null, pointCategory);
					}
				}

            foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			else if (this.exportPointTypes)
			{
				EntityImportExportAttribute rootAttribute = null;

				CreateWorkSheets("ID*", ref rootAttribute, typeof(ApplicationStringClass));

				foreach (ApplicationStringClass pointCategory in FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(x => x.EnumerateByType(this.security, STRING_TYPE.POINT_TEMPLATE_TYPE)))
				{
					AddObjectToWorkSheets("ID*", ref rootAttribute, null, null, pointCategory);
				}

				foreach (WSExportObject worksheet in xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				xmldocumentcollection.Clear();
			}

			// Finish
			this.excelXml = this.excelXml + this.headerEnd;
		}

		#endregion

		#region Private methods
		private void Initialize(SecurityClass security, SiteClass site)
		{
			this.security = security;
			this.site = site;
			this.exportCompanies = false;
			this.exportEquipment = false;
			this.exportPersonnel = false;
			this.exportProducts = false;
			this.exportStandingOffers = false;
			this.exportFuelCard = false;
			this.exportIATACodes = false;
			this.exportEquipmentTypes = false;
			this.includeStrapTables = false;
			this.impExpException = new ImportExportException(null, ImportExportException.EXCEPTION_TYPES.NONE);

			this.excelXml = "";
			this.CreateExcelXmlHeaderStart();
			this.CreateExcelXmlHeaderEnd();
			this.excelXml = this.headerStart;
			this.rootIDList = new List<KeyValuePair<string, string>>();
			this.SiteInfo = FMChannelHelper.MakeCall<ISitesInfo, SiteInfoDO>(siteInfoChannel => siteInfoChannel.RefreshSiteInfo(security));

		}

		/// <summary>
		/// This method creates the excel XML header that will contain all the 
		/// worksheets.
		/// </summary>
		private void CreateExcelXmlHeaderStart()
		{
			this.headerStart =
				"<?xml version=\"1.0\"?>" +
				"<?mso-application progid=\"Excel.Sheet\"?>" +
				"<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" " +
				"xmlns:o=\"urn:schemas-microsoft-com:office:office\" " +
				"xmlns:x=\"urn:schemas-microsoft-com:office:excel\" " +
				"xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" " +
				"xmlns:html=\"http://www.w3.org/TR/REC-html40\" " +
				"xmlns:x2=\"http://schemas.microsoft.com/office/excel/2003/xml\">" +
				"<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">" +
				"<WindowHeight>16095</WindowHeight>" +
				"<WindowWidth>20955</WindowWidth>" +
				"<WindowTopX>360</WindowTopX>" +
				"<WindowTopY>75</WindowTopY>" +
				"<ProtectStructure>False</ProtectStructure>" +
				"<ProtectWindows>False</ProtectWindows>" +
				"<FutureVer>11</FutureVer>" +
				"</ExcelWorkbook>" +
				"<Styles>" +
				"<Style ss:ID=\"Default\" ss:Name=\"Normal\"><Alignment ss:Vertical=\"Bottom\"/><Borders/><Font/><Interior/><NumberFormat/><Protection/></Style>" +
				"<Style ss:ID=\"s21\"><Font ss:Bold=\"1\"/></Style>" +
				"<Style ss:ID=\"s22\"><NumberFormat ss:Format=\"Medium Time\"/></Style>" +
				"<Style ss:ID=\"s23\"><NumberFormat ss:Format=\"Short Date\"/></Style>" +
				"<Style ss:ID=\"s24\"><NumberFormat ss:Format=\"Fixed\"/></Style>" +
				"<Style ss:ID=\"s25\"><NumberFormat ss:Format=\"0\"/></Style>" +
				"</Styles>";
		}

		/// <summary>
		/// This method creates the excel XML header end that will wrap all the 
		/// worksheets.
		/// </summary>
		private void CreateExcelXmlHeaderEnd()
		{
			this.headerEnd = "</Workbook>";
		}

		private void CreateWorkSheetWithDictionaryRelationship(
			EntityImportExportWorksheetAttribute entityImportExportWorksheetAttribute,
			List<EntityImportExportAttribute> rootAttributeList,
			Type objectType)
		{
			WSExportObject worksheet = new WSExportObject(entityImportExportWorksheetAttribute.WorksheetName);

			worksheet.Site = this.site;
			worksheet.Security = this.security;
			worksheet.ImportException = this.impExpException;
			worksheet.SiteInfo = this.SiteInfo;
			worksheet.RootAttributeList = new List<EntityImportExportAttribute>();
			foreach (var rootAttribute in rootAttributeList)
			{
				worksheet.RootAttributeList.Add(rootAttribute);
			}

			EntityImportExportAttribute[] importExportAttributes = worksheet.GetImportExportAttributes(objectType);

			worksheet.CreateHeaderandWidthData(entityImportExportWorksheetAttribute.RootId, importExportAttributes);

			worksheet.CreateMemberHashTable(objectType);

			xmldocumentcollection.Add(worksheet);
		}

		private void CreateWorkSheets(
 string rootID,
 ref EntityImportExportAttribute rootAttribute,
 Type objectType
)
		{
			EntityImportExportWorksheetAttribute[] worksheetAttributes = objectType.GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];
			if (worksheetAttributes == null
			|| worksheetAttributes.Length == 0)
				return;

			string xmlworksheetname = worksheetAttributes[0].WorksheetName;

			WSExportObject worksheet = null;

			worksheet = new WSExportObject(xmlworksheetname);

			worksheet.Site = this.site;
			worksheet.Security = this.security;
			worksheet.ImportException = this.impExpException;
			worksheet.SiteInfo = this.SiteInfo;

			EntityImportExportAttribute[] importExportAttributes = worksheet.GetImportExportAttributes(objectType);

			worksheet.CreateHeaderandWidthData(rootID, ref rootAttribute, importExportAttributes);

			if (rootAttribute == null)
			{
				throw new Exception("Root Attribute Not Found in Import Export Attributes");
			}

			xmldocumentcollection.Add(worksheet);

			// Create sub worksheets for collections
			MemberInfo[] members = objectType.GetMembers();
			foreach (MemberInfo member in members)
			{
				if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
				{
					continue;
				}

				worksheetAttributes = member.GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];
				EntityImportExportWorksheetAttribute collectionWorksheetAttribute = null;

				if (worksheetAttributes != null && worksheetAttributes.Length > 0)
				{
					collectionWorksheetAttribute = worksheetAttributes[0];
				}

				if (collectionWorksheetAttribute == null)
				{
					continue;
				}

				// Export Attributes may be declared on the collection else derive them from the 
				EntityImportExportAttribute[] collectionImportExportAttributes = member.GetCustomAttributes(typeof(EntityImportExportAttribute), false) as EntityImportExportAttribute[];


				if (collectionImportExportAttributes == null || collectionImportExportAttributes.Length == 0)
				{
					continue;
				}

				xmlworksheetname = collectionWorksheetAttribute.WorksheetName;

				Array.Sort(collectionImportExportAttributes);

				worksheet = new WSExportObject(xmlworksheetname);

				worksheet.Site = this.site;
				worksheet.Security = this.security;
				worksheet.ImportException = this.impExpException;
				worksheet.SiteInfo = this.SiteInfo;

				worksheet.CreateHeaderandWidthData(rootID, ref rootAttribute, collectionImportExportAttributes);

				if (rootAttribute == null)
				{
					throw new Exception("Root Attribute Not Found in Import Export Attributes");
				}

				xmldocumentcollection.Add(worksheet);
			}
		}

		private void AddObjectToWorkSheets(string rootID,
											 ref EntityImportExportAttribute rootAttribute,
											 string rootValue,
											 EntityImportExportWorksheetAttribute worksheetAttribute,
											 object o)
		{
			string xmlworksheetname;

			if (worksheetAttribute == null)
			{
				var worksheetAttributes = o.GetType().GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];

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

			WSExportObject worksheet = this.xmldocumentcollection.find(xmlworksheetname);

			if (worksheet == null)
			{
				throw new Exception("WorkSheet " + xmlworksheetname + " not found");
			}

			// set the row data
			worksheet.CreaterRowData(rootAttribute, ref rootValue, o);

			MemberInfo[] members = o.GetType().GetMembers();
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

				if (collectionWorksheetAttribute == null)
				{
					continue;
				}

				object value = worksheet.GetMemberValue(member, o);

				if (!worksheet.IsEnumerable(value))
				{
					continue;
				}

				var enumerable = value as IEnumerable;

				if (enumerable != null)
				{
					IEnumerator enumerator = enumerable.GetEnumerator();

					xmlworksheetname = collectionWorksheetAttribute.WorksheetName;
					worksheet = this.xmldocumentcollection.find(xmlworksheetname);

					if (worksheet == null)
					{
						throw new Exception("WorkSheet " + xmlworksheetname + " not found");
					}

					while (enumerator.MoveNext())
					{
						if (enumerator.Current != null)
						{
							worksheet.CreaterRowData(rootAttribute, ref rootValue, enumerator.Current);
						}
					}
				}
			}
		}

		private void AddObjectToWorkSheet(EntityImportExportWorksheetAttribute worksheetAttribute,
														object o)
		{
			WSExportObject worksheet = this.xmldocumentcollection.find(worksheetAttribute.WorksheetName);
			
			if (worksheet == null)
			{
				throw new Exception("WorkSheet " + worksheetAttribute.WorksheetName + " not found");
			}

			var rootAttributeList = worksheet.RootAttributeList;

			worksheet.CreateRowData(o, IncludeStrapTables);

			// process enumerations with EntityImportExportWoorksheetAttribute
			MemberInfo[] members = o.GetType().GetMembers();
			foreach (MemberInfo member in members)
			{
				if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
				{
					continue;
				}

				var worksheetAttributes = member.GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];

				if (worksheetAttributes == null || worksheetAttributes.Length == 0)
				{
					continue;
				}

				object value = worksheet.GetMemberValue(member, o);

				if (!worksheet.IsEnumerable(value))
				{
					continue;
				}

				IEnumerable enumerable = null;
				IEnumerator enumerator = null;

				if (value is IEnumerable)
				{
					if (value is IDictionary)
					{
						var dictionary = value as IDictionary;
						enumerator = dictionary.Values.GetEnumerator();
					}
					else
					{
						enumerable = value as IEnumerable;
						enumerator = enumerable.GetEnumerator();
					}
				}

				if (enumerator != null)
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current != null)
						{
							var subworksheet = this.xmldocumentcollection.find(worksheetAttributes[0].WorksheetName);
							if (subworksheet == null)
							{
								this.CreateWorkSheetWithDictionaryRelationship(worksheetAttributes[0], rootAttributeList, enumerator.Current.GetType());
							}

							this.AddObjectToWorkSheet(worksheetAttributes[0], enumerator.Current);
						}
					}
				}
			}
		}
		#endregion
	}
}
