/// <summary>
///   File name:	LedgerLineItemCollection.cs
///   Purpose:	   The purpose of the Ledger Line Item collection is to contain
///               all the ledger line items. This class inherits form BaseCollections.
///				   
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				   
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///   Date:			   By:						   Reason:
///   ----------		--------------------	   ----------------------------------
///   yyyy-mm-dd     Coder's name            Change reason
///   
/// </summary>
using System;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class LedgerLineItemCollection : BaseCollections
{
   #region Attributes
   private int siteIndex;
   private int productIndex;
	private int tankIndex;
   #endregion

   #region Constructors
   /// <summary>
   /// This is the default constructor for the Ledger Line Item
   /// Collection class.
   /// </summary>
   public LedgerLineItemCollection()
   {
   }
   #endregion

   #region Public Properties
   /// <summary>
   /// This property gets and sets the Site Index data
   /// member.
   /// </summary>
   public int SiteIndex
   {
      get { return this.siteIndex; }
      set { this.siteIndex = value; }
   }

   /// <summary>
   /// This property gets and sets the Product Index data
   /// member.
   /// </summary>
   public int ProductIndex
   {
      get { return this.productIndex; }
      set { this.productIndex = value; }
   }

	/// <summary>
	/// This property gets and sets the Tank Index data
	/// member.
	/// </summary>
	public int TankIndex
	{
		get { return this.tankIndex; }
		set { this.tankIndex = value; }
	}

   #endregion
}
