namespace FMBusinessServices.ServiceClasses
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.Transactions;
    using System.Xml;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;

    using FMBusinessServices.InternalClasses.EntityImportExport;

    [ServiceBehavior(TransactionIsolationLevel = IsolationLevel.ReadCommitted)]
	public class EntityExcelImportClass : IEntityExcelImport
	{
		#region Private data members
		private readonly XMLImportDocumentCollectionClass xmldocumentcollection = new XMLImportDocumentCollectionClass ( );
		private XmlDocument entityDoc;
		private SecurityClass security;
		private SiteClass site;
		private XmlNamespaceManager nsMgr;
		private EntityImportDO importDO;

		//private const string ImportMsgCompaniesSuccess = "Done importing Companies.";
		//private const string ImportMsgEquipmentSuccess = "Done importing Equipment.";
		//private const string ImportMsgPersonnelSuccess = "Done importing Personnel.";
		//private const string ImportMsgProductsSuccess = "Done importing Products.";
		//private const string ImportMsgStandingOffersSuccess = "Done importing Price List.";
		//private const string ImportMsgFuelCardSuccess = "Done importing Fuel Cards.";
		//private const string ImportMsgIataCodesSuccess = "Done importing Delivery Locations.";
		//private const string ImportMsgEquipmentTypesSuccess = "Done importing Equipment Types.";
		#endregion

        public SiteInfoDO SiteInfo 
		{ 
			get; 
			set; 
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void StartImport(SecurityClass securityParam, SiteClass siteParam, string entityDocXmlString)
		{
		    if (string.IsNullOrEmpty(entityDocXmlString))
		    {
		        return;
		    }

			XmlDocument localEntityDoc = new XmlDocument();

			using (StringReader reader = new StringReader(entityDocXmlString))
			{
				localEntityDoc.Load(reader);
				this.Initialize(securityParam, siteParam, localEntityDoc);
				this.StartImport();
			}
		}

		/// <summary>
		/// This method starts the import process for all worksheets.
		/// </summary>
		protected void StartImport()
		{
			if (this.importDO.ImportProducts)
			{
				ProductClass product = new ProductClass (this.site);

				ProductsClass products = new ProductsClass ( );

				EntityImportExportAttribute rootAttribute = null;

			    this.ImportSelectedObjectFromExcel ( "PRODUCTID*", ref rootAttribute, null, null, null, product, products );

			    this.xmldocumentcollection.Clear ( );
			}

			if (this.importDO.ImportEquipmentTypes)
			{
				EquipmentTypeClass equipmenttype = new EquipmentTypeClass (this.site );

				EquipmentTypesClass equipmenttypes = new EquipmentTypesClass ( );

				EntityImportExportAttribute rootAttribute = null;

			    this.ImportSelectedObjectFromExcel ( "TYPECLASSID*", ref rootAttribute, null, null, null, equipmenttype, equipmenttypes );

			    this.xmldocumentcollection.Clear ( );
			}

			if (this.importDO.ImportEquipment)
			{
				EquipmentClass equipment = new EquipmentClass (this.site );

				EquipmentsClass equipments = new EquipmentsClass ( );

				EntityImportExportAttribute rootAttribute = null;

			    this.ImportSelectedObjectFromExcel ( "EQUIPMENTID*", ref rootAttribute, null, null, null, equipment, equipments );

			    this.xmldocumentcollection.Clear ( );
			}

			if (this.importDO.ImportPersonnel)
			{
				PersonClass person = new PersonClass (this.site );

				PersonnelClass personnel = new PersonnelClass ( );

				EntityImportExportAttribute rootAttribute = null;

			    this.ImportSelectedObjectFromExcel ( "PERSONID*", ref rootAttribute, null, null, null, person, personnel );

			    this.xmldocumentcollection.Clear ( );
			}


			if (this.importDO.ImportStandingOffers)
			{
				StandingOfferClass standingoffer = new StandingOfferClass ( );

				StandingOffersClass standingoffers = new StandingOffersClass ( );

				EntityImportExportAttribute rootAttribute = null;

			    this.ImportSelectedObjectFromExcel ( "STANDINGOFFERID*", ref rootAttribute, null, null, null, standingoffer, standingoffers );

			    this.xmldocumentcollection.Clear ( );
			}

			if (this.importDO.ImportFuelCard)
			{
				FuelCardClass fuelcard = new FuelCardClass ( );

				FuelCardsClass fuelcards = new FuelCardsClass ( );

				EntityImportExportAttribute rootAttribute = null;

			    this.ImportSelectedObjectFromExcel ( "FUELCARDID*", ref rootAttribute, null, null, null, fuelcard, fuelcards );

			    this.xmldocumentcollection.Clear ( );
			}

			if (this.importDO.ImportIATACodes)
			{
				IATACodeClass iatacode = new IATACodeClass ( );

				IATACodesClass iatacodes = new IATACodesClass ( );

				EntityImportExportAttribute rootAttribute = null;

			    this.ImportSelectedObjectFromExcel ( "IATACODEID*", ref rootAttribute, null, null, null, iatacode, iatacodes );

			    this.xmldocumentcollection.Clear ( );
			}


			if (this.importDO.ImportCompanies)
			{
				CompanyClass company = new CompanyClass (this.site );

				CompaniesClass companies = new CompaniesClass ( );

				EntityImportExportAttribute rootAttribute = null;

			    this.ImportSelectedObjectFromExcel ( "COMPANYID*", ref rootAttribute, null, null, null, company, companies );

			    this.xmldocumentcollection.Clear ( );
			}

		}
		/// <summary>
		/// This method will start the import process for the Standing Offers (aka Price List) worksheets:
		/// </summary>
		[SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId="entityDoc" )]
		private void ImportSelectedObjectFromExcel ( string rootID,
		    // ReSharper disable once UnusedParameter.Local
													ref EntityImportExportAttribute rootAttribute,
		    // ReSharper disable once RedundantAssignment
													string rootValue,
		    // ReSharper disable once UnusedParameter.Local
													EntityImportExportAttribute[] importExportAttributes,
													EntityImportExportWorksheetAttribute worksheetAttribute,
													object dataObj,
													object u )
		{

			string xmlworksheetname;

		    if (worksheetAttribute == null)
			{
				EntityImportExportWorksheetAttribute[] worksheetAttributes = dataObj.GetType ( ).GetCustomAttributes ( typeof ( EntityImportExportWorksheetAttribute ), false ) as EntityImportExportWorksheetAttribute[];

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

			var worksheet = this.xmldocumentcollection.find ( xmlworksheetname );

			if (worksheet == null)
			{
			    worksheet = new WSImportObject(xmlworksheetname)
			                {
			                    Site = this.site,
			                    Security = this.security,
			                    ImportException = this.importDO.ImportException,
			                    SiteInfo = this.SiteInfo
			                };

			    this.xmldocumentcollection.Add ( worksheet );
			}

			if (this.entityDoc == null)
			{
				throw new NullReferenceException ( "entityDoc" );
			}

			if (this.security == null)
			{
				throw new NullReferenceException ( "security" );
			}

			if (this.site == null)
			{
				throw new NullReferenceException ( "site" );
			}

			XmlNodeList worksheetList = this.entityDoc.SelectNodes ( "/ss:Workbook/ss:Worksheet", this.nsMgr );

			XmlNodeList stylelist = this.entityDoc.SelectNodes ( "/ss:Workbook/ss:Styles/ss:Style", this.nsMgr );
			worksheet.NumberFormatList.Clear ( );

		    if (stylelist != null)
		    {
		        foreach (XmlNode styleNode in stylelist)
		        {
		            XmlAttribute attribID = styleNode.Attributes?["ss:ID"];
		            if (attribID == null)
		            {
		                continue;
		            }

		            string id = styleNode.Attributes["ss:ID"].Value;

		            XmlNode node = styleNode.SelectNodes ( "ss:NumberFormat", this.nsMgr )?.Item ( 0 );

		            XmlAttribute attrib = node?.Attributes?["ss:Format"];
		            if (attrib == null)
		            {
		                continue;
		            }

		            worksheet.NumberFormatList.Add ( id, attrib.Value );
		        }
		    }

		    if (worksheetList == null || worksheetList.Count == 0)
			{
				throw new FMInvalidEntityImportFileFormatException ( );
			}

			foreach (XmlNode worksheetNode in worksheetList)
			{
				string worksheetName = worksheetNode.Attributes?.Item ( 0 ).Value.ToUpper ( );

				if (!worksheetName.Equals ( xmlworksheetname.ToUpper ( ) ))
				{
					continue;
				}

				worksheet.WorksheetNode = worksheetNode;
				worksheet.NameSpaceManager = this.nsMgr;
				break;
			}

			worksheet.ParseSheet ( null, null );

			rootValue = worksheet.GetRootData ( rootID );

			MemberInfo[] members = dataObj.GetType ( ).GetMembers ( );

			while (string.IsNullOrEmpty(rootValue) == false && rootValue.Length > 0)
			{
				foreach (MemberInfo member in members)
				{
					if (member.MemberType != MemberTypes.Property && member.MemberType != MemberTypes.Field)
					{
						continue;
					}

					EntityImportExportWorksheetAttribute[] worksheetAttributes = member.GetCustomAttributes ( typeof ( EntityImportExportWorksheetAttribute ), false ) as EntityImportExportWorksheetAttribute[];
					EntityImportExportWorksheetAttribute collectionWorksheetAttribute = null;

					if (worksheetAttributes != null && worksheetAttributes.Length > 0)
					{
						collectionWorksheetAttribute = worksheetAttributes[0];
					}

					EntityImportExportAttribute[] collectionImportExportAttributes = member.GetCustomAttributes ( typeof ( EntityImportExportAttribute ), false ) as EntityImportExportAttribute[];

					if (collectionImportExportAttributes == null || collectionImportExportAttributes.Length == 0)
					{
						continue;
					}

					if (collectionWorksheetAttribute != null)
					{
						object collection = worksheet.GetMemberValue ( member, dataObj );

						MethodInfo methodInfo = collection.GetType ( ).GetMethod ( "Add" );
						if (methodInfo == null)
						{
							continue;
						}

					    ParameterInfo[] parameterInfoArray = methodInfo?.GetParameters ( );
						if (parameterInfoArray == null || parameterInfoArray.Length != 1)
						{
							continue;
						}

						// Test for Constructor that takes a SiteClass parameter
						ConstructorInfo constructorInfo = parameterInfoArray[0].ParameterType.GetConstructor ( new[] { typeof ( SiteClass ) } );

						if (constructorInfo == null || constructorInfo.IsPrivate)
						{
							constructorInfo = parameterInfoArray[0].ParameterType.GetConstructor ( new Type[] { } );
						}

						if (constructorInfo == null || constructorInfo.IsPrivate)
						{
							continue;
						}

						IList list = collection as IList;
						list.Clear ( );

					    var colxmlworksheetname = collectionWorksheetAttribute.WorksheetName;
						var colworksheet = this.xmldocumentcollection.find ( colxmlworksheetname );

						if (colworksheet == null)
						{
						    colworksheet = new WSImportObject(collectionWorksheetAttribute.WorksheetName)
						                   {
						                       Site = this.site,
						                       Security = this.security,
						                       ImportException = this.importDO.ImportException,
						                       SiteInfo = this.SiteInfo
						                   };

						    this.xmldocumentcollection.Add ( colworksheet );
						}

						foreach (XmlNode colworksheetNode in worksheetList)
						{
							string colworksheetName = colworksheetNode.Attributes.Item ( 0 ).Value.ToUpper ( );

							if (!colworksheetName.Equals ( colxmlworksheetname.ToUpper ( ) ))
							{
								continue;
							}

							colworksheet.WorksheetNode = colworksheetNode;
							colworksheet.NameSpaceManager = this.nsMgr;
							break;
						}

						colworksheet.ParseSheet ( rootID, rootValue );

						while (colworksheet.RecordRows.Count > 0)
						{
							object collectionObject;

							if (constructorInfo.GetParameters ( ).Length == 0)
							{
								collectionObject = constructorInfo.Invoke ( new object[] { } );
							}
							else
							{
								collectionObject = constructorInfo.Invoke ( new object[] { this.site } );
							}

							colworksheet.ImportExcelRow ( colworksheet.RecordRows[0] as Hashtable,
														collectionObject,
														collectionImportExportAttributes );

							list.Add ( collectionObject );
							colworksheet.RecordRows.RemoveAt ( 0 );
						}
					}
					else
					{
						worksheet.ImportExcelMemberData (	worksheet.RecordRows[0] as Hashtable,
															member,
															collectionImportExportAttributes,
															dataObj );
					}
				}

				if (u.GetType ( ) == typeof ( CompaniesClass ))
					( (CompaniesClass) u ).Import (this.security, (CompanyClass) dataObj );
				else if (u.GetType ( ) == typeof ( EquipmentsClass ))
					( (EquipmentsClass) u ).Import (this.security, (EquipmentClass) dataObj );
				else if (u.GetType ( ) == typeof ( PersonnelClass ))
					( (PersonnelClass) u ).Import (this.security, (PersonClass) dataObj );
				else if (u.GetType ( ) == typeof ( ProductsClass ))
					( (ProductsClass) u ).Import (this.security, (ProductClass) dataObj );
				else if (u.GetType ( ) == typeof ( StandingOffersClass ))
					( (StandingOffersClass) u ).ImportWithStandingOffer (this.security, (StandingOfferClass) dataObj );
				else if (u.GetType ( ) == typeof ( FuelCardsClass ))
					( (FuelCardsClass) u ).Import (this.security, (FuelCardClass) dataObj );
				else if (u.GetType ( ) == typeof ( IATACodesClass ))
					( (IATACodesClass) u ).Import (this.security, (IATACodeClass) dataObj );
				else if (u.GetType ( ) == typeof ( EquipmentTypesClass ))
					( (EquipmentTypesClass) u ).Import (this.security, (EquipmentTypeClass) dataObj );

				// reset the values in the object so we can reuse it
				if (u.GetType ( ) == typeof ( CompaniesClass ))
					( (CompanyClass) dataObj ).Reset ( );
				else if (u.GetType ( ) == typeof ( EquipmentsClass ))
					( (EquipmentClass) dataObj ).Reset ( );
				else if (u.GetType ( ) == typeof ( PersonnelClass ))
					( (PersonClass) dataObj ).Reset ( );
				else if (u.GetType ( ) == typeof ( ProductsClass ))
					( (ProductClass) dataObj ).Reset ( );
				else if (u.GetType ( ) == typeof ( StandingOffersClass ))
					( (StandingOfferClass) dataObj ).Reset ( );
				else if (u.GetType ( ) == typeof ( FuelCardsClass ))
					( (FuelCardClass) dataObj ).Reset ( );
				else if (u.GetType ( ) == typeof ( IATACodesClass ))
					( (IATACodeClass) dataObj ).Reset ( );
				else if (u.GetType ( ) == typeof ( EquipmentTypesClass ))
					( (EquipmentTypeClass) dataObj ).Reset ( );

				if (worksheet.RecordRows.Count > 0)
				{
					worksheet.RecordRows.RemoveAt ( 0 );
				}

				rootValue = worksheet.GetRootData ( rootID );
			}
		}


		/// <summary>
		/// This method initialize the excel import object to its initial state.
		/// </summary>
		/// <param name="securityParam"></param>
		/// <param name="siteParam"></param>
		/// <param name="entityDocParam"></param>
		private void Initialize ( SecurityClass securityParam, SiteClass siteParam, XmlDocument entityDocParam )
		{
			this.security = securityParam;
			this.site = siteParam;
			this.importDO = new EntityImportDO ( );

			SitesInfoClass sitesInfo = new SitesInfoClass ( );
			this.SiteInfo = sitesInfo.RefreshSiteInfo ( securityParam );

			if (entityDocParam == null)
			{
				this.entityDoc = new XmlDocument ( );
			}
			else
			{
				this.entityDoc = entityDocParam;
				this.nsMgr = new XmlNamespaceManager ( this.entityDoc.NameTable );

			    this.nsMgr.AddNamespace ( "", "urn:schemas-microsoft-com:office:spreadsheet" );
			    this.nsMgr.AddNamespace ( "ss", "urn:schemas-microsoft-com:office:spreadsheet" );
			    this.nsMgr.AddNamespace ( "o", "urn:schemas-microsoft-com:office:office" );
			    this.nsMgr.AddNamespace ( "html", "http://www.w3.org/TR/REC-html40" );
			    this.nsMgr.AddNamespace ( "x", "urn:schemas-microsoft-com:office:excel" );
			}
		}
	}
}