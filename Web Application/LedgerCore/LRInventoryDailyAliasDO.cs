namespace LedgerCore
{
	using System;

	class LRInventoryDailyAliasDO
	{
		#region Private data member
		private string aliasName;
		private double grossQuantity;
		private double netQuantity;
		private double massQuantity;
		private double packageQuantity;
		private double grossPrice;
		private double netPrice;
		private double massPrice;
		private double number01;
		private double number02;
		private double number03;
		private double number04;
		private double number05;
		private double number06;
		private string site;
		private int transTypeID;
		private string inventoryDateStr;
		private DateTime inventoryDate;
		private readonly double volumeConversionFactor;
		private readonly double massConversionFactor;
		private readonly double currencyFactor;
		private readonly int volumeDecimalPlaces;
		private readonly int massDecimalPlaces;
		private readonly int currencyDecimalPlaces;
		private readonly double volumePackageSize;
		private readonly double massPackageSize;
		private readonly bool loadByWeight;
		private bool errorFlag;
		private string reversalType;
		private Int64 maxTransVersion;
		private enum IgnoreQuantityRounding { IgnoreRounding, UseRounding };
		private bool reversalFlag;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Inventory Daily Alias data object.
		/// </summary>
		public LRInventoryDailyAliasDO(	double volumeConversionFactor, 
										double massConversionFactor, 
										double currencyFactor,
										int volumeDecimalPlaces, 
										int massDecimalPlaces, 
										int currencyDecimalPlaces,
										double volumePackageSize, 
										double massPackageSize, 
										bool loadByWeight)
		{
			this.Initialize();
			this.volumeConversionFactor = volumeConversionFactor;
			this.massConversionFactor	= massConversionFactor;
			this.currencyFactor			= currencyFactor;
			this.volumeDecimalPlaces	= volumeDecimalPlaces;
			this.massDecimalPlaces		= massDecimalPlaces;
			this.currencyDecimalPlaces	= currencyDecimalPlaces;
			this.volumePackageSize		= volumePackageSize;
			this.massPackageSize		= massPackageSize;
			this.loadByWeight			= loadByWeight;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will set and get the alias name data member.
		/// </summary>
		public string AliasName
		{
			get { return this.aliasName; }
			set { this.aliasName = value; }
		}

		/// <summary>
		/// This property will set and get the site data member.
		/// </summary>
		public string Site
		{
			get { return this.site; }
			set { this.site = value; }
		}

		/// <summary>
		/// This property will get the gross quantity data member.
		/// </summary>
		public double GrossQuantity
		{
			get { return this.grossQuantity; }
		}

		/// <summary>
		/// This property will get the net quantity data member.
		/// </summary>
		public double NetQuantity
		{
			get { return this.netQuantity; }
		}

		/// <summary>
		/// This property will get the mass quantity data member.
		/// </summary>
		public double MassQuantity
		{
			get { return this.massQuantity; }
		}

		/// <summary>
		/// This property will get the package quantity data member.
		/// </summary>
		public double PackageQuantity
		{
			get { return this.packageQuantity; }
		}


		/// <summary>
		/// This property will get the gross price data member.
		/// </summary>
		public double GrossPrice
		{
			get { return this.grossPrice; }
		}


		/// <summary>
		/// This property will get the net price data member.
		/// </summary>
		public double NetPrice
		{
			get { return this.netPrice; }
		}


		/// <summary>
		/// This property will get the mass price data member.
		/// </summary>
		public double MassPrice
		{
			get { return this.massPrice; }
		}

		/// <summary>
		/// This property will set and get the transaction type ID member.
		/// </summary>
		public int TransTypeID
		{
			get { return this.transTypeID; }
			set { this.transTypeID = value; }
		}

		/// <summary>
		/// This property will set and get the transaction reversalType member.
		/// </summary>
		public string ReversalType
		{
			get { return this.reversalType; }
			set { this.reversalType = value; }
		}


		/// <summary>
		/// This property will set and get the transaction maxTransVersion member.
		/// </summary>
		public Int64 MaxTransVersion
		{
			get { return this.maxTransVersion; }
			set { this.maxTransVersion = value; }
		}

		/// <summary>
		/// This property will set and get the inventory date string data member.
		/// </summary>
		public string InventoryDateStr
		{
			get { return this.inventoryDateStr; }
			set
			{
				this.inventoryDateStr = value;

				if (string.IsNullOrEmpty(this.inventoryDateStr) == false)
				{
					char[] separatorList = { '/' };
					string[] stringList = this.inventoryDateStr.Split(separatorList);

					int year = Convert.ToInt32(stringList[0]);
					int month = Convert.ToInt32(stringList[1]);
					int day = Convert.ToInt32(stringList[2]);

					this.inventoryDate = new DateTime(year, month, day, 00, 00, 00);
				}

			}
		}

		/// <summary>
		/// This property will return the inventory date as a Date Time object.
		/// </summary>
		public DateTime InventoryDate
		{
			get { return this.inventoryDate; }
		}

		/// <summary>
		/// This property will return true if the transaction has the error
		/// flag set.
		/// </summary>
		public bool ErrorFlag
		{
			get { return this.errorFlag; }
		}

		public bool ReversalFlag
		{
			get { return this.reversalFlag; }
		}

		/// <summary>
		/// This property will get the number 01 data member.
		/// </summary>
		public double Number01
		{
			get { return this.number01; }
		}

		/// <summary>
		/// This property will get the number 02 data member.
		/// </summary>
		public double Number02
		{
			get { return this.number02; }
		}

		/// <summary>
		/// This property will get the number 03 data member.
		/// </summary>
		public double Number03
		{
			get { return this.number03; }
		}

		/// <summary>
		/// This property will get the number 04 data member.
		/// </summary>
		public double Number04
		{
			get { return this.number04; }
		}

		/// <summary>
		/// This property will get the number 05 data member.
		/// </summary>
		public double Number05
		{
			get { return this.number05; }
		}

		/// <summary>
		/// This property will get the number 06 data member.
		/// </summary>
		public double Number06
		{
			get { return this.number06; }
		}
		#endregion

		#region Public methods

		/// <summary>
		/// This method will sum the gross quantity to the existing
		/// data member.
		/// </summary>
		/// <param name="gross"></param>
		public void SumGross(double gross)
		{
			this.grossQuantity += this.GetVolumeQuantity(gross, IgnoreQuantityRounding.UseRounding);
		}

		/// <summary>
		/// This method will sum the net quantity to the existing
		/// data member.
		/// </summary>
		/// <param name="net"></param>
		public void SumNet(double net)
		{
			double convertedNet = this.GetVolumeQuantity(net, IgnoreQuantityRounding.UseRounding);
			this.netQuantity += convertedNet;

			if (!this.loadByWeight && this.volumePackageSize != 0)
			{
				this.packageQuantity += convertedNet / this.volumePackageSize;
			}
		}

		/// <summary>
		/// This method will sum the mass quantity to the existing
		/// data member.
		/// </summary>
		/// <param name="mass"></param>
		public void SumMass(double mass)
		{
			double convertedMass = this.GetMassQuantity(mass, IgnoreQuantityRounding.UseRounding);
			this.massQuantity += convertedMass;

			if (this.loadByWeight && this.massPackageSize != 0)
			{
				this.packageQuantity += convertedMass / this.massPackageSize;
			}
		}

		/// <summary>
		/// This method will sum the gross price weighted by the quantity to the existing
		/// data member.
		/// </summary>
		/// <param name="price"></param>
		/// <param name="gross"></param>
		public void SumGrossPrice(double price, double gross)
		{
			// Conversion the currency and then round.
			double newPrice = price * this.currencyFactor;

			if (this.currencyDecimalPlaces >= 0)
			{
				newPrice = Math.Round(newPrice, this.currencyDecimalPlaces, MidpointRounding.AwayFromZero);
			}

			// Sum the prices weighted by the quantity. Use the quantity that is not rounded.
			this.grossPrice += (newPrice * this.GetVolumeQuantity(gross, IgnoreQuantityRounding.IgnoreRounding));
		}

		/// <summary>
		/// This method will sum the net price weighted by the quantity to the existing
		/// data member.
		/// </summary>
		/// <param name="price"></param>
		/// <param name="net"></param>
		public void SumNetPrice(double price, double net)
		{
			// Conversion the currency and then round.
			double newPrice = price * this.currencyFactor;

			if (this.currencyDecimalPlaces >= 0)
			{
				newPrice = Math.Round(newPrice, this.currencyDecimalPlaces, MidpointRounding.AwayFromZero);
			}

			// Sum the prices weighted by the quantity. Use the quantity that is not rounded.
			this.netPrice += (newPrice * this.GetVolumeQuantity(net, IgnoreQuantityRounding.IgnoreRounding));
		}

		/// <summary>
		/// This method will sum the mass price weighted by the quantity to the existing
		/// data member.
		/// </summary>
		/// <param name="price"></param>
		/// <param name="mass"></param>
		public void SumMassPrice(double price, double mass)
		{
			// Conversion the currency and then round.
			double newPrice = price * this.currencyFactor;

			if (this.currencyDecimalPlaces >= 0)
			{
				newPrice = Math.Round(newPrice, this.currencyDecimalPlaces, MidpointRounding.AwayFromZero);
			}

			// Sum the prices weighted by the quantity. Use the quantity that is not rounded.
			this.massPrice += (newPrice * this.GetMassQuantity(mass, IgnoreQuantityRounding.IgnoreRounding));
		}



		/// <summary>
		/// This method will sum the Number fields weighted by the quantity to the existing
		/// data member.
		/// </summary>
		/// <param name="numberFieldValue"></param>
		/// <param name="whichNumberField"></param>
		public void SumNumberField(double numberFieldValue, int whichNumberField)
		{
			double newQuantity = numberFieldValue;

			// Perform the conversion prior to rounding
			newQuantity = Math.Round(newQuantity, this.volumeDecimalPlaces, MidpointRounding.AwayFromZero);

			switch (whichNumberField)
			{
				case 1:
					this.number01 += newQuantity;
					break;
				case 2:
					this.number02 += newQuantity;
					break;
				case 3:
					this.number03 += newQuantity;
					break;
				case 4:
					this.number04 += newQuantity;
					break;
				case 5:
					this.number05 += newQuantity;
					break;
				case 6:
					this.number06 += newQuantity;
					break;
			}
		}

		/// <summary>
		/// This method will OR the error flag for an unique key of
		/// Inventory Date and Alias Name.
		/// </summary>
		/// <param name="inErrorFlag"></param>
		public void OrErrorFlag(bool inErrorFlag)
		{
			this.errorFlag = this.errorFlag | inErrorFlag;
		}

		public void OrReversalFlag(bool inReversalFlag)
		{
			this.reversalFlag = this.reversalFlag | inReversalFlag;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.aliasName			= string.Empty;
			this.grossQuantity		= 0.0;
			this.netQuantity		= 0.0;
			this.massQuantity		= 0.0;
			this.grossPrice			= 0.0;
			this.netPrice			= 0.0;
			this.massPrice			= 0.0;
			this.site				= string.Empty;
			this.transTypeID		= 0;
			this.inventoryDateStr	= string.Empty;
			this.number01			= 0.0;
			this.number02			= 0.0;
			this.number03			= 0.0;
			this.number04			= 0.0;
			this.number05			= 0.0;
			this.number06			= 0.0;
			this.errorFlag			= false;
			this.reversalFlag		= false;
			this.maxTransVersion	= 0;
		}

		/// <summary>
		/// This method will divide the quantity in half if the transaction is
		/// a specific type of transfer transaction. In addition, a conversion
		/// factor is applied along with rounding.
		/// </summary>
		/// <param name="quantity">The quantity to be converted and rounded.</param>
		/// <param name="quantityRounding">Identifies whether to round the quantity or not.</param>
		/// <returns></returns>
		private double GetVolumeQuantity(double quantity, IgnoreQuantityRounding quantityRounding)
		{
			double newQuantity = quantity;

			if (this.transTypeID == 11 || this.transTypeID == 23)
			{
				if (this.reversalType == "R" || this.reversalType == "RU")
				{
					if (quantity > 0)
					{
						newQuantity = 0.0;
					}
				}
				else if (quantity < 0)
				{
					newQuantity = 0.0;
				}
			}

			// Perform the conversion prior to rounding
			newQuantity = newQuantity * this.volumeConversionFactor;

			if (IgnoreQuantityRounding.UseRounding == quantityRounding)
			{
				newQuantity = Math.Round(newQuantity, this.volumeDecimalPlaces, MidpointRounding.AwayFromZero);
			}

			return newQuantity;
		}

		/// <summary>
		/// This method will divide the quantity in half if the transaction is
		/// a specific type of transfer transaction. In addition, a conversion
		/// factor is applied along with rounding.
		/// </summary>
		/// <param name="quantity">The quantity to be converted and rounded.</param>
		/// <param name="quantityRounding">Identifies whether to round the quantity or not.</param>
		/// <returns></returns>
		private double GetMassQuantity(double quantity, IgnoreQuantityRounding quantityRounding)
		{
			double newQuantity = quantity;

			if (this.transTypeID == 11 || this.transTypeID == 23)
			{
				if (this.reversalType == "R" || this.reversalType == "RU")
				{
					if (quantity > 0)
					{
						newQuantity = 0.0;
					}
				}
				else if (quantity < 0)
				{
					newQuantity = 0.0;
				}
			}

			// Perform the conversion prior to rounding
			newQuantity = newQuantity * this.massConversionFactor;

			if (IgnoreQuantityRounding.UseRounding == quantityRounding)
			{
				newQuantity = Math.Round(newQuantity, this.massDecimalPlaces, MidpointRounding.AwayFromZero);
			}

			return newQuantity;
		}

		/// <summary>
		/// This method will divide the quantity in half if the transaction is
		/// a specific type of transfer transaction. In addition, a conversion
		/// factor is applied along with rounding.
		/// </summary>
		/// <param name="quantity">The quantity to be converted and rounded.</param>
		/// <param name="quantityRounding">Identifies whether to round the quantity or not.</param>
		/// <returns></returns>
		private double GetPackageQuantity(double quantity, IgnoreQuantityRounding quantityRounding)
		{
			double newQuantity = quantity;

			if (this.transTypeID == 11 || this.transTypeID == 23)
			{
				if (this.reversalType == "R" || this.reversalType == "RU")
				{
					if (quantity > 0)
					{
						newQuantity = 0.0;
					}
				}
				else if (quantity < 0)
				{
					newQuantity = 0.0;
				}
			}

			// Perform the conversion prior to rounding
			newQuantity = newQuantity * this.massConversionFactor;

			if (IgnoreQuantityRounding.UseRounding == quantityRounding)
			{
				newQuantity = Math.Round(newQuantity, this.massDecimalPlaces, MidpointRounding.AwayFromZero);
			}

			return newQuantity;
		}
		#endregion
	}
}