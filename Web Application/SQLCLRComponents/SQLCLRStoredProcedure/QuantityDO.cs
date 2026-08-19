/// <summary>
///   File name:  QuantityDO.cs
///   Purpose:	   The purpoase of this class is to contain quantity volumes and 
///               pricing for one day and one alias entry.
///				   
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				   
///	Author(s):	Richard R. Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///   Date:			   By:						   Reason:
///   ----------		--------------------	   ----------------------------------
///   yyyy-mm-dd     Coder's name            Reason for change
/// </summary>
using System;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class QuantityDO
{
   #region Attributes
   private double gross;
   private double net;
	private double mass;
	private double package;
   private double grossPrice;
   private double netPrice;
	private double massPrice;
   private double number01;
   private double number02;
   private double number03;
   private double number04;
   private double number05;
   private double number06;
   private bool   affectsInventory;
   private string moniker;
   private bool   isFillerQuantity;
   private bool   transErrorFlag;
   private bool   isAggregateQuantity;
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
   public QuantityDO(double gross, double net, double mass)
   {
      this.Init();

      this.gross = gross;
      this.net   = net;
		this.mass  = mass;
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
	public QuantityDO(double gross,double net,double mass,double package,double grossPrice,double netPrice,double massPrice)
   {
      this.Init();

      this.gross      = gross;
      this.net        = net;
		this.mass		 = mass;
		this.package	 = package;
      this.grossPrice = grossPrice;
      this.netPrice   = netPrice;
		this.massPrice  = massPrice;
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
   public QuantityDO(double gross, 
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
                   double number06)
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
   /// <param name="transErrorFlag"></param>
   public QuantityDO(double gross,
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
                double number06,
                bool   transErrorFlag)
   {
      this.Init();

      this.gross          = gross;
      this.net            = net;
		this.mass			  = mass;
		this.package		  = package;
      this.grossPrice     = grossPrice;
      this.netPrice       = netPrice;
		this.massPrice      = massPrice;
      this.number01       = number01;
      this.number02       = number02;
      this.number03       = number03;
      this.number04       = number04;
      this.number05       = number05;
      this.number06       = number06;
      this.transErrorFlag = transErrorFlag;
   }
   #endregion

   #region Properties
   /// <summary>
   /// This property returns True if the quantity is associated to an aggregate
   /// column. Otherwise, it returns False.
   /// </summary>
   public bool IsAggregateQuantity
   {
      get { return this.isAggregateQuantity; }
      set { this.isAggregateQuantity = value; }
   }

   /// <summary>
   /// This property returns True if the key (Inventory Date & Alias Name) has
   /// an error associated with the original transaction. Otherwise, it returns
   /// false.
   /// </summary>
   public bool TransErrorFlag
   {
      get { return this.transErrorFlag; }
   }

   /// <summary>
   /// This property returns True if the quantity is used as a filler on the ledger. This
   /// means that no quantity was retrieved from the database. Otherwise, it will return
   /// false, meaning that it is not a filler.
   /// </summary>
   public bool IsFillerQuantity
   {
      get { return this.isFillerQuantity; }
      set { this.isFillerQuantity = value; }
   }

   /// <summary>
   /// Property that set and returns the Number 01 volume value as an unsigned floating point number.
   /// </summary>
   public double Number01
   {
      get { return Math.Abs(this.number01); }
      set { this.number01 = value; }
   }

   /// <summary>
   /// Property that returns the Number 01 Volume value as a floating point number.
   /// </summary>
   public double Number01Change
   {
      get { return this.number01; }
      set { this.number01 = value; }
   }

   /// <summary>
   /// Property that set and returns the Number 02 volume value as an unsigned floating point number.
   /// </summary>
   public double Number02
   {
      get { return Math.Abs(this.number02); }
      set { this.number02 = value; }
   }

   /// <summary>
   /// Property that returns the Number 02 Volume value as a floating point number.
   /// </summary>
   public double Number02Change
   {
      get { return this.number02; }
      set { this.number02 = value; }
   }

   /// <summary>
   /// Property that set and returns the Number 03 volume value as an unsigned floating point number.
   /// </summary>
   public double Number03
   {
      get { return Math.Abs(this.number03); }
      set { this.number03 = value; }
   }

   /// <summary>
   /// Property that returns the Number 03 Volume value as a floating point number.
   /// </summary>
   public double Number03Change
   {
      get { return this.number03; }
      set { this.number03 = value; }
   }

   /// <summary>
   /// Property that set and returns the Number 04 volume value as an unsigned floating point number.
   /// </summary>
   public double Number04
   {
      get { return Math.Abs(this.number04); }
      set { this.number04 = value; }
   }

   /// <summary>
   /// Property that returns the Number 04 Volume value as a floating point number.
   /// </summary>
   public double Number04Change
   {
      get { return this.number04; }
      set { this.number04 = value; }
   }

   /// <summary>
   /// Property that set and returns the Number 05 volume value as an unsigned floating point number.
   /// </summary>
   public double Number05
   {
      get { return Math.Abs(this.number05); }
      set { this.number05 = value; }
   }

   /// <summary>
   /// Property that returns the Number 05 Volume value as a floating point number.
   /// </summary>
   public double Number05Change
   {
      get { return this.number05; }
      set { this.number05 = value; }
   }

   /// <summary>
   /// Property that set and returns the Number 06 volume value as an unsigned floating point number.
   /// </summary>
   public double Number06
   {
      get { return Math.Abs(this.number06); }
      set { this.number06 = value; }
   }

   /// <summary>
   /// Property that returns the Number 06 Volume value as a floating point number.
   /// </summary>
   public double Number06Change
   {
      get { return this.number06; }
      set { this.number06 = value; }
   }

   /// <summary>
   /// Property that set and returns the Gross Volume value as a signed floating point number.
   /// </summary>
   public double Gross
   {
      get { return Math.Abs(gross); }
      set { gross = value; }
   }

	/// <summary>
	/// Property that set and returns the Net Volume value as a signed floating point number.
	/// </summary>
	public double Net
	{
		get { return Math.Abs(net); }
		set { net = value; }
	}

	/// <summary>
	/// Property that set and returns the Mass value as a signed floating point number.
	/// </summary>
	public double Mass
	{
		get { return Math.Abs(mass); }
		set { mass = value; }
	}

	/// <summary>
	/// Property that set and returns the Package value as a signed floating point number.
	/// </summary>
	public double Package
	{
		get { return Math.Abs(package); }
		set { package = value; }
	}


	/// <summary>
   /// Property that returns the Gross Volume value as a positive floating point number.
   /// </summary>
   public double GrossInventoryChange
   {
      get
      {
         return gross;
      }
      set { gross = value; }
   }

	/// <summary>
	/// Property that set and returns the Net Volume value as a positive floating point number.
	/// </summary>
	public double NetInventoryChange
	{
		get
		{
			return net;
		}
		set { net = value; }
	}

	/// <summary>
	/// Property that set and returns the Mass value as a positive floating point number.
	/// </summary>
	public double MassInventoryChange
	{
		get
		{
			return mass;
		}
		set { mass = value; }
	}

	/// <summary>
	/// Property that set and returns the Package value as a positive floating point number.
	/// </summary>
	public double PackageInventoryChange
	{
		get
		{
			return package;
		}
		set { package = value; }
	}



   /// <summary>
   /// Property that set and returns the Gross Price value as a positive floating point number.
   /// </summary>
   public double GrossPrice
   {
      get { return Math.Abs(this.grossPrice); }
      set { this.grossPrice = value; }
   }

   /// <summary>
   /// Property that set and returns the Gross Price value as a signed floating point number.
   /// </summary>
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
      get { return Math.Abs(this.netPrice); }
      set { this.netPrice = value; }
   }

   /// <summary>
   /// Property that set and returns the Net Price value as a signed floating point number.
   /// </summary>
   public double NetPriceInventoryChange
   {
      get { return this.netPrice; }
      set { this.netPrice = value; }
   }

	/// <summary>
	/// Property that set and returns the Mass Price value as a positive floating point number.
	/// </summary>
	public double MassPrice
	{
		get { return Math.Abs(this.massPrice); }
		set { this.massPrice = value; }
	}

	/// <summary>
	/// Property that set and returns the Mass Price value as a signed floating point number.
	/// </summary>
	public double MassPriceInventoryChange
	{
		get { return this.massPrice; }
		set { this.massPrice = value; }
	}



   /// <summary>
   /// This property will return the Moniker data member.  The default moniker
   /// value is an empty string.
   /// </summary>
   public string Moniker
   {
      get { return this.moniker; }
   }

   /// <summary>
   /// This property will return true if the volume/price affects inventory.
   /// Otherwise, it returns false.
   /// </summary>
   public bool AffectsInventory
   {
      get { return this.affectsInventory; }
      set { this.affectsInventory = value; }
   }

   #endregion

   #region Public methods
   /// <summary>
   /// This method will append a new moniker to the existing moniker associcated
   /// with this volume.
   /// </summary>
   /// <param name="inMoniker"></param>
   public void AppendMoniker(string inMoniker)
   {
      if ((inMoniker != null) && (inMoniker.Length > 0))
      {
         // Incoming moniker can only be one character.
         if (inMoniker.Length > 1)
         {
            this.moniker += inMoniker.Substring(0, 1);
         }
         else
         {
            this.moniker += inMoniker;
         }
      }
   }

   /// <summary>
   /// This method will combine monikers when ledgers are combined for the 
   /// final ledger.
   /// </summary>
   /// <param name="inMoniker"></param>
   public void CombineMonikers(string inMoniker)
   {
      if ((inMoniker != null) && (inMoniker.Length > 0))
      {
         CharEnumerator charEnumerator = inMoniker.GetEnumerator();

         while (charEnumerator.MoveNext() == true)
         {
            string aCharacter = charEnumerator.Current.ToString();

            if (this.moniker.Contains(aCharacter) == false)
            {
               this.moniker += aCharacter;
            }
         }
      }
   }

   /// <summary>
   /// This method will OR the error flag for an unique key of
   /// Inventory Date and Alias Name.
   /// </summary>
   /// <param name="inErrorFlag"></param>
   public void OrErrorFlag(bool inErrorFlag)
   {
      this.transErrorFlag = this.transErrorFlag | inErrorFlag;
   }
   #endregion

   #region Private methods
   /// <summary>
   /// This method will initialize the Volume data object to its initial state.
   /// </summary>
   private void Init()
   {
      this.gross                 = 0.0;
      this.net                   = 0.0;
		this.mass						= 0.0;
		this.package					= 0.0;
      this.grossPrice            = 0.0;
      this.netPrice              = 0.0;
		this.massPrice					= 0.0;
      this.number01              = 0.0;
      this.number02              = 0.0;
      this.number03              = 0.0;
      this.number04              = 0.0;
      this.number05              = 0.0;
      this.number06              = 0.0;
      this.moniker               = "";
      this.isFillerQuantity      = true;
      this.transErrorFlag        = false;
      this.isAggregateQuantity   = false;
   }
   #endregion
}