using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMBusinessObjects.Exceptions;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class EntityExportDO
	{
		#region Private data members
		private bool exportCompanies;
		private bool exportEquipment;
		private bool exportPersonnel;
		private bool exportProducts;
		private bool exportStandingOffers;
		private bool exportFuelCard;
		private bool exportIATACodes;
		private bool exportEquipmentTypes;
		private string excelXml;
		private EntityImportExportException impExpException;
		#endregion

		#region Constructors
		public EntityExportDO ( )
		{
			this.exportCompanies		= false;
			this.exportEquipment		= false;
			this.exportPersonnel		= false;
			this.exportProducts			= false;
			this.exportStandingOffers	= false;
			this.exportFuelCard			= false;
			this.exportIATACodes		= false;
			this.exportEquipmentTypes	= false;
			this.excelXml				= "";
			this.impExpException		= new EntityImportExportException ( null, EntityImportExportException.EXCEPTION_TYPES.NONE );
		}
		#endregion

		#region Properties
		[DataMember]
		public string ExcelXMLDocument
		{
			get { return this.excelXml; }
			set { this.excelXml = value; }
		}

		[DataMember]
		public bool ExportCompanies
		{
			get { return this.exportCompanies; }
			set { this.exportCompanies = value; }
		}

		[DataMember]
		public bool ExportEquipment
		{
			get { return this.exportEquipment; }
			set { this.exportEquipment = value; }
		}

		[DataMember]
		public bool ExportPersonnel
		{
			get { return this.exportPersonnel; }
			set { this.exportPersonnel = value; }
		}

		[DataMember]
		public bool ExportProducts
		{
			get { return this.exportProducts; }
			set { this.exportProducts = value; }
		}

		[DataMember]
		public bool ExportStandingOffers
		{
			get { return this.exportStandingOffers; }
			set { this.exportStandingOffers = value; }
		}

		[DataMember]
		public bool ExportFuelCard
		{
			get { return this.exportFuelCard; }
			set { this.exportFuelCard = value; }
		}

		[DataMember]
		public bool ExportIATACodes
		{
			get { return this.exportIATACodes; }
			set { this.exportIATACodes = value; }
		}

		[DataMember]
		public bool ExportEquipmentTypes
		{
			get { return this.exportEquipmentTypes; }
			set { this.exportEquipmentTypes = value; }
		}

		[DataMember]
		public EntityImportExportException ImportException
		{
			get { return this.impExpException; }
		}
		#endregion
	}
}
