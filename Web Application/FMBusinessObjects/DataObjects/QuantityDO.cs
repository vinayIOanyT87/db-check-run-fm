using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[XmlRoot( "Quantity" )]
	[XmlType( "Quantity" )]
	[Serializable]
	[DataContract]
	public class QuantityDO
	{
		#region Attributes
		[DataMember] private double? gross;
		[DataMember] private double? net;
		[DataMember] private double? deliveredgross;
		[DataMember] private double? deliverednet;
		[DataMember] private double? mass;
		[DataMember] private double? package;
		[DataMember] private bool isgrossdirty;
		[DataMember] private bool isnetdirty;
		[DataMember] private bool isdeliveredgrossdirty;
		[DataMember] private bool isdeliverednetdirty;
		[DataMember] private bool ismassdirty;
		[DataMember] private bool ispackagedirty;
		[DataMember] private bool isvcfdirty;
		[DataMember] private double grossPrice;
		[DataMember] private double netPrice;
		[DataMember] private double massPrice;
		[DataMember] private bool affectsInventory;
		[DataMember] private bool badGrossQualityLogged;
		[DataMember] private bool badNetQualityLogged;
		[DataMember] private bool badMassQualityLogged;
		[DataMember] private double number01;
		[DataMember] private double number02;
		[DataMember] private double number03;
		[DataMember] private double number04;
		[DataMember] private double number05;
		[DataMember] private double number06;
		[DataMember] private string moniker;
		[DataMember] private bool? grossManualValueFlag;
		[DataMember] private bool? netManualValueFlag;
		[DataMember] private bool? deliveredGrossManualValueFlag;
		[DataMember] private bool? deliveredNetManualValueFlag;
		[DataMember] private bool? massManualValueFlag;
		[DataMember] private bool? packageManualValueFlag;
		[DataMember] private bool? vcfManualValueFlag;
		#endregion

		#region Constructors
		/// <summary>
		/// Default constructor for class QuantityDO.
		/// </summary>
		public QuantityDO()
		{
			this.Init();
		}

		/// <summary>
		/// Constructor to initialize the QuantityDO class with the its
		/// properties.
		/// </summary>
		/// <param name="gross"></param>
		/// <param name="net"></param>
		/// <param name="mass"></param>
		/// <param name="package"></param>
		public QuantityDO( double gross, double net, double mass, double package)
		{
			this.Init();
			this.gross = gross;
			this.net   = net;
			this.mass  = mass;
			this.package = package;
		}

		/// <summary>
		/// Constructor to initialize the QuantityDO class with the its
		/// properties.
		/// </summary>
		/// <param name="gross"></param>
		/// <param name="net"></param>
		/// <param name="mass"></param>
		/// <param name="package"></param>
		/// <param name="grossPrice"></param>
		/// <param name="netPrice"></param>
		/// <param name="massPrice"></param>
		public QuantityDO( double gross, double net, double mass, double package, double grossPrice, double netPrice, double massPrice )
		{
			this.Init();
			this.gross      = gross;
			this.net        = net;
			this.mass		 = mass;
			this.package	 = package;
			this.grossPrice = grossPrice;
			this.netPrice   = netPrice;
			this.massPrice	 = massPrice;
		}

		/// <summary>
		/// Constructor to initialize the QuantityDO class with the its
		/// properties.
		/// </summary>
		/// <param name="gross"></param>
		/// <param name="net"></param>
		/// <param name="mass"></param>
		/// <param name="package"></param>
		/// <param name="grossPrice"></param>
		/// <param name="netPrice"></param>
		/// <param name="massPrice"></param>
		/// <param name="number01"></param>
		/// <param name="number02"></param>
		/// <param name="number03"></param>
		/// <param name="number04"></param>
		/// <param name="number05"></param>
		/// <param name="number06"></param>
		public QuantityDO( double gross,
								double net,
								double mass,
								double package,
								double grossPrice,
								double netPrice,
								double massPrice,
								double number01,
								double number02,
								double number03,
								double number04,
								double number05,
								double number06 )
		{
			this.Init();
			this.gross                 = gross;
			this.net                   = net;
			this.mass						= mass;
			this.package					= package;
			this.grossPrice            = grossPrice;
			this.netPrice              = netPrice;
			this.massPrice					= massPrice;
			this.number01              = number01;
			this.number02              = number02;
			this.number03              = number03;
			this.number04              = number04;
			this.number05              = number05;
			this.number06              = number06;
		}
          
      public QuantityDO(QuantityDO quantityDO)
      {
			if (quantityDO == null)
			{
			throw new ArgumentNullException(nameof(quantityDO));
			}

			this.gross = quantityDO.gross;
			this.net = quantityDO.net;
			this.deliveredgross = quantityDO.deliveredgross;
			this.deliverednet = quantityDO.deliverednet;
			this.mass = quantityDO.mass;
			this.package = quantityDO.package;
			this.isgrossdirty=quantityDO.isgrossdirty;
			this.isnetdirty = quantityDO.isnetdirty;
			this.ismassdirty = quantityDO.ismassdirty;
			this.ispackagedirty = quantityDO.ispackagedirty;
			this.isvcfdirty = quantityDO.isvcfdirty;
			this.grossPrice = quantityDO.grossPrice;
			this.netPrice = quantityDO.netPrice;
			this.massPrice = quantityDO.massPrice;
			this.affectsInventory = quantityDO.affectsInventory;
			this.badGrossQualityLogged = quantityDO.badGrossQualityLogged;
			this.badNetQualityLogged = quantityDO.badNetQualityLogged;
			this.badMassQualityLogged = quantityDO.badMassQualityLogged;
			this.number01 = quantityDO.number01;
			this.number02 = quantityDO.number02;
			this.number03 = quantityDO.number03;
			this.number04 = quantityDO.number04;
			this.number05 = quantityDO.number05;
			this.number06 = quantityDO.number06;
			this.moniker = quantityDO.moniker;
			this.netManualValueFlag = quantityDO.netManualValueFlag;
			this.massManualValueFlag = quantityDO.massManualValueFlag;
			this.grossManualValueFlag = quantityDO.grossManualValueFlag;
			this.packageManualValueFlag = quantityDO.packageManualValueFlag;
			this.vcfManualValueFlag = quantityDO.vcfManualValueFlag;
			this.deliveredGrossManualValueFlag = quantityDO.deliveredGrossManualValueFlag;
			this.deliveredNetManualValueFlag = quantityDO.deliveredNetManualValueFlag;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Property that set and returns the moniker value associated to this volume.
		/// The default is an empty string.
		/// </summary>
		public string Moniker
		{
			get { return this.moniker; }
			set
			{
					if (value == null)
					{
						this.moniker = "";
					}
					else
					{
						this.moniker = value;
					}
			}
		}

		/// <summary>
		/// Property that set and returns the Number 01 volume value as an unsigned floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double Number01
		{
			get { return Math.Abs( this.number01 ); }
			set { this.number01 = value; }
		}

		/// <summary>
		/// Property that returns the Number 01 Volume value as a floating point number.
		/// </summary>
		[XmlElement( ElementName = "Number01" )]
		public double Number01Change
		{
			get { return this.number01; }
			set { this.number01 = value; }
		}

		/// <summary>
		/// Property that set and returns the Number 02 volume value as an unsigned floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double Number02
		{
			get { return Math.Abs( this.number02 ); }
			set { this.number02 = value; }
		}

		/// <summary>
		/// Property that returns the Number 02 Volume value as a floating point number.
		/// </summary>
		[XmlElement( ElementName = "Number02" )]
		public double Number02Change
		{
			get { return this.number02; }
			set { this.number02 = value; }
		}

		/// <summary>
		/// Property that set and returns the Number 03 volume value as an unsigned floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double Number03
		{
			get { return Math.Abs( this.number03 ); }
			set { this.number03 = value; }
		}

		/// <summary>
		/// Property that returns the Number 03 Volume value as a floating point number.
		/// </summary>
		[XmlElement( ElementName = "Number03" )]
		public double Number03Change
		{
			get { return this.number03; }
			set { this.number03 = value; }
		}

		/// <summary>
		/// Property that set and returns the Number 04 volume value as an unsigned floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double Number04
		{
			get { return Math.Abs( this.number04 ); }
			set { this.number04 = value; }
		}

		/// <summary>
		/// Property that returns the Number 04 Volume value as a floating point number.
		/// </summary>
		[XmlElement( ElementName = "Number04" )]
		public double Number04Change
		{
			get { return this.number04; }
			set { this.number04 = value; }
		}

		/// <summary>
		/// Property that set and returns the Number 05 volume value as an unsigned floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double Number05
		{
			get { return Math.Abs( this.number05 ); }
			set { this.number05 = value; }
		}

		/// <summary>
		/// Property that returns the Number 05 Volume value as a floating point number.
		/// </summary>
		[XmlElement( ElementName = "Number05" )]
		public double Number05Change
		{
			get { return this.number05; }
			set { this.number05 = value; }
		}

		/// <summary>
		/// Property that set and returns the Number 06 volume value as an unsigned floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double Number06
		{
			get { return Math.Abs( this.number06 ); }
			set { this.number06 = value; }
		}

		/// <summary>
		/// Property that returns the Number 06 Volume value as a floating point number.
		/// </summary>
		[XmlElement( ElementName = "Number06" )]
		public double Number06Change
		{
			get { return this.number06; }
			set { this.number06 = value; }
		}

		/// <summary>
		/// Property that set and returns the Gross Volume value as a positive floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double Gross
		{
			get { return (gross == null) ? 0.0 : Math.Abs( (double)gross ); }
			set { gross = value; }
		}


		/// <summary>
		/// Property that set and returns the Gross Volume value.
		/// </summary>
		[XmlIgnoreAttribute]
		public double? NullableGross
		{
			get { return gross; }
			set { gross = value; }
		}

		/// <summary>
		/// Property that returns the Gross Volume value as a signed floating point number.
		/// </summary>
		[XmlElement( ElementName="Gross" )]
		public double GrossInventoryChange
		{
			get
			{
					return (gross == null) ? 0.0 : (double)gross;
			}
			set { gross = value; }
		}

		/// <summary>
		/// Property that set and returns the Gross Volume value as a positive floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double DeliveredGross
		{
			get { return (deliveredgross == null) ? 0.0 : Math.Abs((double)deliveredgross); }
			set { deliveredgross = value; }
		}


		/// <summary>
		/// Property that set and returns the Gross Volume value.
		/// </summary>
		[XmlIgnoreAttribute]
		public double? NullableDeliveredGross
		{
			get { return deliveredgross; }
			set { deliveredgross = value; }
		}

		/// <summary>
		/// Property that returns the Delivered Gross Volume value as a signed floating point number.
		/// </summary>
		[XmlElement(ElementName = "Delivered Gross")]
		public double DeliveredGrossInventoryChange
		{
			get
			{
				return (deliveredgross == null) ? 0.0 : (double)deliveredgross;
			}
			set { deliveredgross = value; }
		}



		[XmlIgnoreAttribute]
		public double Mass
		{
			get { return (mass == null) ? 0.0 : Math.Abs( (double)mass ); }
			set { mass = value; }
		}

		[XmlIgnoreAttribute]
		public double? NullableMass
		{
			get { return mass; }
			set { mass = value; }
		}

		/// <summary>
		/// Property that returns the Mass value as a signed floating point number.
		/// </summary>
		[XmlElement( ElementName="Mass" )]
		public double MassInventoryChange
		{
			get
			{
					return (mass == null) ? 0.0 : (double)mass;
			}
			set { mass = value; }
		}


		[XmlIgnoreAttribute]
		public double Package
		{
			get { return (package == null) ? 0.0 : Math.Abs( (double)package ); }
			set { package = value; }
		}

		[XmlIgnoreAttribute]
		public double? NullablePackage
		{
			get { return package; }
			set { package = value; }
		}

		/// <summary>
		/// Property that returns the Package value as a signed floating point number.
		/// </summary>
		[XmlElement( ElementName="Package" )]
		public double PackageInventoryChange
		{
			get
			{
					return (package == null) ? 0.0 : (double)package;
			}
			set { package = value; }
		}



		/// <summary>
		/// Property that set and returns the Net Volume value as a positive floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double Net
		{
			get { return (net == null) ? 0.0 : Math.Abs( (double)net ); }
			set { net = value; }
		}



		/// <summary>
		/// Property that set and returns the Net Volume value as a positive floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double? NullableNet
		{
			get { return net; }
			set { net = value; }
		}

		/// <summary>
		/// Property that set and returns the Net Volume value as a signed floating point number.
		/// </summary>
		[XmlElement( ElementName="Net" )]
		public double NetInventoryChange
		{
			get
			{
					return (net == null) ? 0.0 : (double)net;
			}
			set { net = value; }
		}

		/// <summary>
		/// Property that set and returns the Delivered Net Volume value as a positive floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double DeliveredNet
		{
			get { return (deliverednet == null) ? 0.0 : Math.Abs((double)deliverednet); }
			set { deliverednet = value; }
		}


		/// <summary>
		/// Property that set and returns the Delivered Net Volume value.
		/// </summary>
		[XmlIgnoreAttribute]
		public double? NullableDeliveredNet
		{
			get { return deliverednet; }
			set { deliverednet = value; }
		}

		/// <summary>
		/// Property that returns the Delivered Net Volume value as a signed floating point number.
		/// </summary>
		[XmlElement(ElementName = "Delivered Net")]
		public double DeliveredNetInventoryChange
		{
			get
			{
				return (deliverednet == null) ? 0.0 : (double)deliverednet;
			}
			set { deliverednet = value; }
		}



		/// <summary>
		/// Property that set and returns the Is Gross Dirty value as a boolean.
		/// </summary>
		[XmlIgnoreAttribute]
		public bool IsGrossDirty
		{
			get { return this.isgrossdirty; }
			set { this.isgrossdirty = value; }
		}

		/// <summary>
		/// Property that set and returns the Is Net Dirty value as a boolean.
		/// </summary>
		[XmlIgnoreAttribute]
		public bool IsNetDirty
		{
			get { return this.isnetdirty; }
			set { this.isnetdirty = value; }
		}

		/// <summary>
		/// Property that set and returns the Is Delivered Gross Dirty value as a boolean.
		/// </summary>
		[XmlIgnoreAttribute]
		public bool IsDeliveredGrossDirty
		{
			get { return this.isdeliveredgrossdirty; }
			set { this.isdeliveredgrossdirty = value; }
		}

		/// <summary>
		/// Property that set and returns the Is Delivered Net Dirty value as a boolean.
		/// </summary>
		[XmlIgnoreAttribute]
		public bool IsDeliveredNetDirty
		{
			get { return this.isdeliverednetdirty; }
			set { this.isdeliverednetdirty = value; }
		}


		/// <summary>
		/// Property that set and returns the Is Mass Dirty value as a boolean.
		/// </summary>
		[XmlIgnoreAttribute]
		public bool IsMassDirty
		{
			get { return this.ismassdirty; }
			set { this.ismassdirty = value; }
		}

		[XmlIgnoreAttribute]
		public bool IsPackageDirty
		{
			get { return this.ispackagedirty; }
			set { this.ispackagedirty = value; }
		}

		[XmlIgnoreAttribute]
		public bool IsVcfDirty
		{
			get { return this.isvcfdirty; }
			set { this.isvcfdirty = value; }
		}

		/// <summary>
		/// Property that set and returns the Gross Price value as a positive floating point number.
		/// </summary>
		public double GrossPrice
		{
			get { return Math.Abs( this.grossPrice ); }
			set { this.grossPrice = value; }
		}

		/// <summary>
		/// Property that set and returns the Gross Price value as a signed floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double GrossPriceInventoryChange
		{
			get { return this.grossPrice; }
			set { this.grossPrice = value; }
		}

		/// <summary>
		/// Property that set and returns the Net Price value as a positive floating point number.
		/// </summary>
		public double NetPrice
		{
			get { return Math.Abs( this.netPrice ); }
			set { this.netPrice = value; }
		}

		/// <summary>
		/// Property that set and returns the Net Price value as a signed floating point number.
		/// </summary>
		[XmlIgnoreAttribute]
		public double NetPriceInventoryChange
		{
			get { return this.netPrice; }
			set { this.netPrice = value; }
		}

		/// <summary>
		/// Gets or sets the mass price.
		/// </summary>
		/// <value>
		/// The mass price.
		/// </value>
		public double MassPrice
		{
			get { return Math.Abs( this.massPrice ); }
			set { this.massPrice = value; }
		}

		/// <summary>
		/// Gets or sets the mass price inventory change.
		/// </summary>
		/// <value>
		/// The mass price inventory change.
		/// </value>
		[XmlIgnoreAttribute]
		public double MassPriceInventoryChange
		{
			get { return this.massPrice; }
			set { this.massPrice = value; }
		}


		/// <summary>
		/// Gets or sets a value indicating whether [affects inventory].
		/// </summary>
		/// <value>
		///   <c>true</c> if [affects inventory]; otherwise, <c>false</c>.
		/// </value>
		[XmlIgnoreAttribute]
		public bool AffectsInventory
		{
			get { return this.affectsInventory; }
			set { this.affectsInventory = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [bad gross quality logged].
		/// </summary>
		/// <value>
		/// <c>true</c> if [bad gross quality logged]; otherwise, <c>false</c>.
		/// </value>
		[XmlIgnoreAttribute]
		public bool BadGrossQualityLogged
		{
			get { return this.badGrossQualityLogged; }
			set { this.badGrossQualityLogged = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [bad net quality logged].
		/// </summary>
		/// <value>
		/// <c>true</c> if [bad net quality logged]; otherwise, <c>false</c>.
		/// </value>
		[XmlIgnoreAttribute]
		public bool BadNetQualityLogged
		{
			get { return this.badNetQualityLogged; }
			set { this.badNetQualityLogged = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether [bad mass quality logged].
		/// </summary>
		/// <value>
		/// <c>true</c> if [bad mass quality logged]; otherwise, <c>false</c>.
		/// </value>
		[XmlIgnoreAttribute]
		public bool BadMassQualityLogged
		{
			get { return this.badMassQualityLogged; }
			set { this.badMassQualityLogged = value; }
		}


		/// <summary>
		/// Gets or sets the gross manual value flag.
		/// </summary>
		/// <value>
		/// The gross manual value flag.
		/// </value>
		public bool? GrossManualValueFlag
		{
			get { return this.grossManualValueFlag; }
			set { this.grossManualValueFlag = value; }
		}

		/// <summary>
		/// Gets or sets the net manual value flag.
		/// </summary>
		/// <value>
		/// The net manual value flag.
		/// </value>
		public bool? NetManualValueFlag
		{
			get { return this.netManualValueFlag; }
			set { this.netManualValueFlag = value; }
		}

		/// <summary>
		/// Gets or sets the delivered gross manual value flag.
		/// </summary>
		/// <value>
		/// The delivered gross manual value flag.
		/// </value>
		public bool? DeliveredGrossManualValueFlag
		{
			get { return this.deliveredGrossManualValueFlag; }
			set { this.deliveredGrossManualValueFlag = value; }
		}

		/// <summary>
		/// Gets or sets the delivered net manual value flag.
		/// </summary>
		/// <value>
		/// The delivered net manual value flag.
		/// </value>
		public bool? DeliveredNetManualValueFlag
		{
			get { return this.deliveredNetManualValueFlag; }
			set { this.deliveredNetManualValueFlag = value; }
		}

		/// <summary>
		/// Gets or sets the mass manual value flag.
		/// </summary>
		/// <value>
		/// The mass manual value flag.
		/// </value>
		public bool? MassManualValueFlag
		{
			get { return this.massManualValueFlag; }
			set { this.massManualValueFlag = value; }
		}
		/// <summary>
		/// Gets or sets the package manual value flag.
		/// </summary>
		/// <value>
		/// The package manual value flag.
		/// </value>
		public bool? PackageManualValueFlag
		{
			get { return this.packageManualValueFlag; }
			set { this.packageManualValueFlag = value; }
		}

		/// <summary>
		/// Gets or sets the VCF manual value flag.
		/// </summary>
		/// <value>
		/// The VCF manual value flag.
		/// </value>
		public bool? VcfManualValueFlag
		{
			get { return this.vcfManualValueFlag; }
			set { this.vcfManualValueFlag = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the Volume data object to its initial state.
		/// </summary>
		private void Init()
		{
			this.gross					= null;
			this.net						= null;
			this.deliveredgross		= null;
			this.deliverednet			= null;
			this.mass					= null;
			this.isgrossdirty			= false;
			this.isnetdirty			= false;
			this.isdeliveredgrossdirty = false;
			this.isdeliverednetdirty = false;
			this.ismassdirty			= false;
			this.ispackagedirty     = false;
			this.isvcfdirty			= false;
			this.grossPrice			= 0.0;
			this.netPrice				= 0.0;
			this.massPrice				= 0.0;
			this.badGrossQualityLogged	= false;
			this.badNetQualityLogged	= false;
			this.badMassQualityLogged	= false;
			this.number01				= 0.0;
			this.number02				= 0.0;
			this.number03				= 0.0;
			this.number04				= 0.0;
			this.number05				= 0.0;
			this.number06				= 0.0;
			this.moniker				= "";
			this.grossManualValueFlag = null;
			this.netManualValueFlag = null;
			this.massManualValueFlag    = null;
			this.vcfManualValueFlag     = null;
			this.deliveredGrossManualValueFlag = null;
			this.deliveredNetManualValueFlag = null;
			this.packageManualValueFlag = null;
		}
		#endregion
	}
}
