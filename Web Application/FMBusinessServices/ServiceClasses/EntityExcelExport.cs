using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Collections;
using System.Reflection;
using System.ServiceModel;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;
using FMBusinessServices.InternalClasses.EntityImportExport;

namespace FMBusinessServices.ServiceClasses
{
	public class EntityExcelExportClass : IEntityExcelExport
	{
		#region Private data members
		private XMLExportDocumentCollectionClass xmldocumentcollection = new XMLExportDocumentCollectionClass();
		private SecurityClass security;
		private SiteClass site;
		private string headerStart;
		private string headerEnd;
		private EntityExportDO exportDO;

		private const string EXPORT_MSG_COMPANIES_SUCCESS = "Done exporting Companies.";
		private const string EXPORT_MSG_EQUIPMENT_SUCCESS = "Done exporting Equipment.";
		private const string EXPORT_MSG_PERSONNEL_SUCCESS = "Done exporting Personnel.";
		private const string EXPORT_MSG_PRODUCTS_SUCCESS = "Done exporting Products.";
		private const string EXPORT_MSG_STANDING_OFFERS_SUCCESS = "Done exporting Price List.";
		private const string EXPORT_MSG_FUEL_CARD_SUCCESS = "Done exporting Fuel Cards.";
		private const string EXPORT_MSG_IATA_CODES_SUCCESS = "Done exporting Delivery Locations.";
		private const string EXPORT_MSG_EQUIPMENT_TYPES_SUCCESS = "Done exporting Equipment Types.";

		public SiteInfoDO SiteInfo { get; set; }

		#endregion

		#region Constructors

		public EntityExcelExportClass()
		{
		}

		/// <summary>
		/// This is the default constructor for the excel export class.
		/// </summary>
		public EntityExcelExportClass(SecurityClass security, SiteClass site)
		{
			this.Initialize(security, site);
		}
		#endregion

		#region Public methods
		public void StartExport(SecurityClass security, SiteClass site)
		{
			this.Initialize(security, site);
			Export();
		}

		protected void Export()
		{
			// Build company export if requested.
			if (this.exportDO.ExportCompanies == true)
			{
				CompaniesClass companies = new CompaniesClass();
				EntityImportExportAttribute rootAttribute = null;

				this.CreateWorkSheets("COMPANYID*", ref rootAttribute, typeof(CompanyClass));

				foreach (CompanyClass company in companies.EnumerateExt(this.security, false, true))
				{
					if (company.SiteGuid == security.SiteGuid)
					{
						this.AddObjectToWorkSheets("COMPANYID*", ref rootAttribute, null, null, companies.Get(this.security, company.IdentityGuid));
					}
				}

				foreach (WSExportObject worksheet in this.xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				this.xmldocumentcollection.Clear();
			}

			// Build equipment export if requested.
			if (this.exportDO.ExportEquipment == true)
			{
				EquipmentsClass equipments = new EquipmentsClass();
				EntityImportExportAttribute rootAttribute = null;

				this.CreateWorkSheets("EQUIPMENTID*", ref rootAttribute, typeof(EquipmentClass));

				foreach (EquipmentInfo equipmentInfo in equipments.EnumerateInfo(this.security))
				{
					if (equipmentInfo.siteGuid == security.SiteGuid)
					{
						this.AddObjectToWorkSheets("EQUIPMENTID*", ref rootAttribute, null, null, equipments.Get(this.security, equipmentInfo.identityGuid));
					}
				}

				foreach (WSExportObject worksheet in this.xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				this.xmldocumentcollection.Clear();
			}

			// Build Personnel export if requested.
			if (this.exportDO.ExportPersonnel == true)
			{
				PersonnelClass personnel = new PersonnelClass();
				EntityImportExportAttribute rootAttribute = null;

				this.CreateWorkSheets("PERSONID*", ref rootAttribute, typeof(PersonClass));

				foreach (PersonClass person in personnel.Enumerate(this.security))
				{
					if (person.SiteGuid == security.SiteGuid)
					{
						this.AddObjectToWorkSheets("PERSONID*", ref rootAttribute, null, null, personnel.Get(this.security, person.IdentityGuid));
					}
				}

				foreach (WSExportObject worksheet in this.xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				this.xmldocumentcollection.Clear();
			}

			// Build Products export if requested.
			if (this.exportDO.ExportProducts == true)
			{
				ProductsClass products = new ProductsClass();
				EntityImportExportAttribute rootAttribute = null;

				this.CreateWorkSheets("PRODUCTID*", ref rootAttribute, typeof(ProductClass));

				foreach (ProductClass product in products.Enumerate(this.security))
				{
					if (product.SiteGuid == security.SiteGuid)
					{
						this.AddObjectToWorkSheets("PRODUCTID*", ref rootAttribute, null, null, products.Get(this.security, product.IdentityGuid));
					}
				}

				foreach (WSExportObject worksheet in this.xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				this.xmldocumentcollection.Clear();
			}

			// Build Standing Offers (aka Price List) export if requested.
			if (this.exportDO.ExportStandingOffers == true)
			{
				StandingOffersClass standingOffers = new StandingOffersClass();
				EntityImportExportAttribute rootAttribute = null;

				this.CreateWorkSheets("STANDINGOFFERID*", ref rootAttribute, typeof(StandingOfferClass));

				foreach (StandingOfferClass standingOffer in standingOffers.Enumerate(this.security))
				{
					if (standingOffer.SiteGuid == security.SiteGuid)
					{
						this.AddObjectToWorkSheets("STANDINGOFFERID*", ref rootAttribute, null, null, standingOffers.Get(this.security, standingOffer.IdentityGuid));
					}
				}

				foreach (WSExportObject worksheet in this.xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				this.xmldocumentcollection.Clear();
			}

			// Build Fuel Common Request export if requested.
			if (this.exportDO.ExportFuelCard == true)
			{
				FuelCardsClass fuelCards = new FuelCardsClass();
				EntityImportExportAttribute rootAttribute = null;

				this.CreateWorkSheets("FUELCARDID*", ref rootAttribute, typeof(FuelCardClass));

				foreach (FuelCardClass fuelCard in fuelCards.EnumerateFuelCards(this.security))
				{
					if (fuelCard.SiteGuid == security.SiteGuid)
					{
						this.AddObjectToWorkSheets("FUELCARDID*", ref rootAttribute, null, null, fuelCards.Get(this.security, fuelCard.IdentityGuid, true));
					}
				}

				foreach (WSExportObject worksheet in this.xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				this.xmldocumentcollection.Clear();
			}

			// Build IATA codes export if requested.
			if (this.exportDO.ExportIATACodes == true)
			{
				IATACodesClass iataCodes = new IATACodesClass();
				EntityImportExportAttribute rootAttribute = null;

				this.CreateWorkSheets("IATACODEID*", ref rootAttribute, typeof(IATACodeClass));

				foreach (IATACodeClass iataCode in iataCodes.Enumerate(this.security))
				{
					if (iataCode.SiteGuid == security.SiteGuid)
					{
						this.AddObjectToWorkSheets("IATACODEID*", ref rootAttribute, null, null, iataCodes.Get(this.security, iataCode.IdentityGuid));
					}
				}

				foreach (WSExportObject worksheet in this.xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				this.xmldocumentcollection.Clear();
			}

			// Build Equipment Types export if requested.
			if (this.exportDO.ExportEquipmentTypes == true)
			{
				EquipmentTypesClass equipmentTypes = new EquipmentTypesClass();
				EntityImportExportAttribute rootAttribute = null;

				this.CreateWorkSheets("TYPECLASSID*", ref rootAttribute, typeof(EquipmentTypeClass));

				foreach (EquipmentTypeClass equipmentType in equipmentTypes.Enumerate(this.security, null, null))
				{
					if (equipmentType.SiteGuid == security.SiteGuid)
					{
						this.AddObjectToWorkSheets("TYPECLASSID*", ref rootAttribute, null, null, equipmentTypes.Get(this.security, equipmentType.IdentityGuid));
					}
				}

				foreach (WSExportObject worksheet in this.xmldocumentcollection)
				{
					this.AddWorksheet(worksheet.WorksheetXML);
				}

				this.xmldocumentcollection.Clear();
			}

			// Finish
			this.exportDO.ExcelXMLDocument = this.exportDO.ExcelXMLDocument + this.headerEnd;
		}
		#endregion

		#region Private methods
		private void Initialize(SecurityClass security, SiteClass site)
		{
			this.security = security;
			this.site = site;
			this.exportDO = new EntityExportDO();

			this.exportDO.ExcelXMLDocument = "";
			this.CreateExcelXmlHeaderStart();
			this.CreateExcelXmlHeaderEnd();
			this.exportDO.ExcelXMLDocument = this.headerStart;

			SitesInfoClass sitesInfo = new SitesInfoClass();
			this.SiteInfo = sitesInfo.RefreshSiteInfo(security);

		}

		/// <summary>
		/// This method will add a worksheet to the Excel XML document.
		/// </summary>
		/// <param name="worksheet"></param>
		private void AddWorksheet(string worksheet)
		{
			if ((worksheet != null) && (worksheet.Length > 0))
			{
				this.exportDO.ExcelXMLDocument = this.exportDO.ExcelXMLDocument + worksheet;
			}
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

		private void CreateWorkSheets(string rootID, ref EntityImportExportAttribute rootAttribute, Type objectType)
		{
			EntityImportExportWorksheetAttribute[] worksheetAttributes = objectType.GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];

			if (worksheetAttributes == null || worksheetAttributes.Length == 0)
			{
				return;
			}

			string xmlworksheetname = worksheetAttributes[0].WorksheetName;
			WSExportObject worksheet = null;

			worksheet = new WSExportObject(xmlworksheetname);
			worksheet.Site = this.site;
			worksheet.Security = this.security;
			worksheet.ImportException = this.exportDO.ImportException;
			worksheet.SiteInfo = this.SiteInfo;

			EntityImportExportAttribute[] importExportAttributes = worksheet.GetImportExportAttributes(objectType);

			worksheet.CreateHeaderandWidthData(rootID, ref rootAttribute, importExportAttributes);

			if (rootAttribute == null)
			{
				throw new Exception("Root Attribute Not Found in Import Export Attributes");
			}

			this.xmldocumentcollection.Add(worksheet);

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
				worksheet.ImportException = this.exportDO.ImportException;
				worksheet.SiteInfo = this.SiteInfo;

				worksheet.CreateHeaderandWidthData(rootID, ref rootAttribute, collectionImportExportAttributes);

				if (rootAttribute == null)
				{
					throw new Exception("Root Attribute Not Found in Import Export Attributes");
				}

				this.xmldocumentcollection.Add(worksheet);
			}
		}

		private void AddObjectToWorkSheets(string rootID,
											ref EntityImportExportAttribute rootAttribute,
											string rootValue,
											EntityImportExportWorksheetAttribute worksheetAttribute,
											object o)
		{
			string xmlworksheetname = "";
			WSExportObject worksheet = null;

			if (worksheetAttribute == null)
			{
				EntityImportExportWorksheetAttribute[] worksheetAttributes = o.GetType().GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];

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

			worksheet = this.xmldocumentcollection.find(xmlworksheetname);

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

				EntityImportExportWorksheetAttribute[] worksheetAttributes = member.GetCustomAttributes(typeof(EntityImportExportWorksheetAttribute), false) as EntityImportExportWorksheetAttribute[];
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

				IEnumerable enumerable = value as IEnumerable;
				IEnumerator enumerator = enumerable.GetEnumerator();

				xmlworksheetname = collectionWorksheetAttribute.WorksheetName;
				worksheet = this.xmldocumentcollection.find(xmlworksheetname);

				if (worksheet == null)
				{
					throw new Exception("WorkSheet " + xmlworksheetname + " not found");
				}

				while (enumerator.MoveNext())
				{
					worksheet.CreaterRowData(rootAttribute, ref rootValue, enumerator.Current);
				}
			}
		}
		#endregion
	}
}