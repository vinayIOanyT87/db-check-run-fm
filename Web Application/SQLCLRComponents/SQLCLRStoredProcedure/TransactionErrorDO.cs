/// <summary>
/// File name:	TransactionErrorDO.cs
/// Purpose:	The purpoase of this class is to transaction error status with
///				the associated alias name and inventory date.
///				
/// Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
/// Author(s):	Richard R. Panachida
/// Version:	1.0.0  Current version
///	
/// Modification History:
/// Date:			By:						Reason:
/// ----------		--------------------	----------------------------------
///		
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TransactionErrorDO
{
   #region Private Attributes
   private string aliasName;
   private string inventoryDate;
   private int    errorStatus;
   #endregion

   #region Constructors
   /// <summary>
   /// This is the default constructor for the Transaction Error Data Object class.
   /// </summary>
   public TransactionErrorDO()
   {
   }
   #endregion

   #region Properties
   /// <summary>
   /// This properties gets and sets the alias name.
   /// </summary>
   public string AliasName
   {
      get { return this.aliasName; }
      set { this.aliasName = value; }
   }
   /// <summary>
   /// This properties gets and sets the inventory date.
   /// </summary>
   public string InventoryDate
   {
      get { return this.inventoryDate; }
      set { this.inventoryDate = value; }
   }
   /// <summary>
   /// This properties gets and sets the error status.
   /// </summary>
   public int ErrorStatus
   {
      get { return this.errorStatus; }
      set { this.errorStatus = value; }
   }
   #endregion
}
