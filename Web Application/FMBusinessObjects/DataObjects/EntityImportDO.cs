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
	public class EntityImportDO
	{
		#region Private data members
		private bool importCompanies;
		private bool importEquipment;
		private bool importPersonnel;
		private bool importProducts;
		private bool importStandingOffers;
		private bool importFuelCard;
		private bool importIATACodes;
		private bool importEquipmentTypes;
		private EntityImportExportException impExpException;
		#endregion

		#region Constructors
		public EntityImportDO ()
		{
			this.importCompanies		= false;
			this.importEquipment		= false;
			this.importPersonnel		= false;
			this.importProducts			= false;
			this.importStandingOffers	= false;
			this.importFuelCard			= false;
			this.importIATACodes		= false;
			this.importEquipmentTypes	= false;
			this.impExpException		= new EntityImportExportException ( null, EntityImportExportException.EXCEPTION_TYPES.NONE );
		}
		#endregion

		#region Properties
		[DataMember]
		public bool ImportCompanies
		{
			get { return this.importCompanies; }
			set { this.importCompanies = value; }
		}

		[DataMember]
		public bool ImportEquipment
		{
			get { return this.importEquipment; }
			set { this.importEquipment = value; }
		}

		[DataMember]
		public bool ImportPersonnel
		{
			get { return this.importPersonnel; }
			set { this.importPersonnel = value; }
		}

		[DataMember]
		public bool ImportProducts
		{
			get { return this.importProducts; }
			set { this.importProducts = value; }
		}

		[DataMember]
		public bool ImportStandingOffers
		{
			get { return this.importStandingOffers; }
			set { this.importStandingOffers = value; }
		}

		[DataMember]
		public bool ImportFuelCard
		{
			get { return this.importFuelCard; }
			set { this.importFuelCard = value; }
		}

		[DataMember]
		public bool ImportIATACodes
		{
			get { return this.importIATACodes; }
			set { this.importIATACodes = value; }
		}

		[DataMember]
		public bool ImportEquipmentTypes
		{
			get { return this.importEquipmentTypes; }
			set { this.importEquipmentTypes = value; }
		}

		[DataMember]
		public EntityImportExportException ImportException
		{
			get { return this.impExpException; }
			private set { this.impExpException = value; }
		}
		#endregion
	}
}
