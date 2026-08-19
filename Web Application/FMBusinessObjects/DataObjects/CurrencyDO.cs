/// <summary>
/// File name:	CurrencyDO.cs
/// Purpose:	To contain and load currency data.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000. This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
///	Author(s):	Van Thompson
///	Version:	1.0.0 Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		yyyy-mm-dd		Developer's name		Reason for the changes
///		
///</summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	#region Currency DO Collection Class
   [Serializable]
   [CollectionDataContract]
	public class CurrencyDOCollectionClass : List<CurrencyDO> { }
	#endregion

	#region Currency DO Class
	[DataContract]
   [Serializable]
	[KnownType(typeof(CurrencyLineItemDOCollectionClass))]
	[KnownType(typeof(CurrencyLineItemDO))]
	public class CurrencyDO : BaseDataObject
	{
		#region Protected data members
		[DataMember] protected int lookupCurrencyUnitIndex;
		[DataMember] protected string country;
		[DataMember] protected string unitDisplayName;
		[DataMember] protected bool displayFlag;
		[DataMember] protected CurrencyLineItemDOCollectionClass currencyLineItems;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Currency Data Object class.
		/// </summary>
		public CurrencyDO()
		{
			currencyLineItems = new CurrencyLineItemDOCollectionClass();
		}
		#endregion

		#region Properties
		public int LookupCurrencyUnitIndex
		{
			get { return lookupCurrencyUnitIndex; }
			set { lookupCurrencyUnitIndex = value; }
		}

		public string Country
		{
			get { return country; }
			set { country = value; }
		}

		public string UnitDisplayName
		{
			get { return unitDisplayName; }
			set { unitDisplayName = value; }
		}

		public bool DisplayFlag
		{
			get { return displayFlag; }
			set { displayFlag = value; }
		}

		public CurrencyLineItemDOCollectionClass LineItems
		{
			get { return currencyLineItems; }
			set { currencyLineItems = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Populates the Currency object with data from a database
		/// </summary>
		/// <param name="dr">Contains the data used to populate the Currency object</param>
		public void Populate(DataRow dr)
		{
			base._IdentityGuid = DataObject.getValue<Guid>(dr["CurrencyGuid"], Guid.Empty);
			this.lookupCurrencyUnitIndex = DataObject.getValue<int>(dr["LookupCurrencyUnitIndex"], 0);
			this.country = DataObject.getValue<string>(dr["Country"], "");
			this.unitDisplayName = DataObject.getValue<string>(dr["UnitDisplayName"], "");
			this.displayFlag = DataObject.getValue<bool>(dr["DisplayFlag"], false);
			base._SiteGuid = DataObject.getValue<Guid>(dr["SiteGuid"], Guid.Empty);
			base._CreatedDate = DataObject.getValue<DateTimeOffset>(dr["CreatedDate"], DateTimeOffset.Now);
			base._CreatedBy = DataObject.getValue<string>(dr["CreatedBy"], ADMIN);
			base._UpdatedDate = DataObject.getValue<DateTimeOffset>(dr["UpdatedDate"], _CreatedDate);
			base._UpdatedBy = DataObject.getValue<string>(dr["UpdatedBy"], ADMIN);
		}
		#endregion

		#region Public SQL methods
		/// <summary>
		/// Returns the configured currencies for the passed site guid
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="siteGuid">The guid of a site</param>
		public void SelectForSite(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = ""
				+ "  SELECT CurrencyGuid, "
				+ "			LookupCurrencyUnitIndex, "
				+ "			Country, "
				+ "			UnitDisplayName, "
				+ "			DisplayFlag, "
				+ "			SiteGuid, "
				+ "			CreatedBy, "
				+ "			CreatedDate, "
				+ "			UpdatedBy, "
				+ "			UpdatedDate "
				+ "	FROM tblCurrencies "
				+ "	WHERE SiteGuid = @SiteGuid "
				+ " ORDER BY UnitDisplayName ";

			SqlParameter guidParm = cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			guidParm.Value = siteGuid;
		}

		/// <summary>
		/// Returns the configured currencies for the passed currency guid
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="currencyGuid">The guid of a currency record</param>
		public void SelectLineItemsForCurrency(SqlCommand cmd, Guid currencyGuid)
		{
			cmd.CommandText = "SELECT CurrencyLineItemGuid, CurrencyGuid, Date, Rate, CreatedBy, " +
						 "CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM  tblCurrencyLineItems " +
						 "WHERE CurrencyGuid = @CurrencyGuid " +
						  "ORDER BY Date DESC";

			// Prepare the statement
			cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = currencyGuid;
		}

		/// <summary>
		/// Retrieves all currencies from the database
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <remarks>Originally currencies were created by site.  This changed to
		/// currencies being global which prompted the creation of this method.</remarks>
		public void SelectCurrencies(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT " +
						 "CurrencyGuid, LookupCurrencyUnitIndex, Country, UnitDisplayName, DisplayFlag, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM tblCurrencies " +
						 "ORDER BY UnitDisplayName";
		}

		/// <summary>
		/// Retrieves the currency units such as dollars, marks, etc.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		public void SelectCurrencyUnits(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT " +
						 "CurrencyUnitIndex, CurrencyUnitName " +
						 "FROM lookup.tblCurrencyUnit " +
						 "ORDER BY CurrencyUnitName";
		}

		/// <summary>
		/// This method will return the sql command that retrieves currency data
		/// based on a currency guid.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="currencyGuid"></param>
		public void Select(SqlCommand cmd, Guid currencyGuid)
		{
			cmd.CommandText = "SELECT CurrencyGuid, LookupCurrencyUnitIndex, Country, UnitDisplayName, DisplayFlag, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM tblCurrencies " +
						 "WHERE CurrencyGuid = @CurrencyGuid";

			SqlParameter guidParm = cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			guidParm.Value = currencyGuid;
		}

		/// <summary>
		/// This method will return one row that matches the Unit Index.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="lookupCurrencyUnitIndex"></param>
		public void SelectByUnitIndex(SqlCommand cmd, int lookupCurrencyUnitIndex)
		{
			cmd.CommandText = "SELECT CurrencyGuid, LookupCurrencyUnitIndex, Country, UnitDisplayName, DisplayFlag, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate " +
						 "FROM tblCurrencies " +
						 "WHERE LookupCurrencyUnitIndex = @LookupCurrencyUnitIndex";

			SqlParameter indexParm = cmd.Parameters.Add("@LookupCurrencyUnitIndex", SqlDbType.Int);
			indexParm.Value = lookupCurrencyUnitIndex;
		}

		/// <summary>
		/// This method will return a SQL command that updates the currency
		/// table tblCurrencies.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="currency"></param>
		/// <param name="userID"></param>
		public void Update(SqlCommand cmd, CurrencyDO currency, string userID)
		{
			// First, update the currency object
			cmd.CommandText = "UPDATE tblCurrencies " +
						 "SET " +
						 "LookupCurrencyUnitIndex = @LookupCurrencyUnitIndex, " +
						 "Country = @country, " +
						 "UnitDisplayName = @displayName, " +
						 "DisplayFlag = @displayFlag, " +
						 "SiteGuid = @SiteGuid, " +
						 "CreatedBy = @createdBy, " +
						 "CreatedDate = @createdDate, " +
						 "UpdatedBy = @updatedBy, " +
						 "UpdatedDate = @updatedDate " +
						 "WHERE CurrencyGuid = @CurrencyGuid";

			// Add parameters
			int i = 0;
			cmd.Parameters.Add("@LookupCurrencyUnitIndex", SqlDbType.Int);
			cmd.Parameters[i++].Value = currency.LookupCurrencyUnitIndex;
			cmd.Parameters.Add("@country", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = currency.Country;
			cmd.Parameters.Add("@displayName", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = currency.UnitDisplayName;
			cmd.Parameters.Add("@displayFlag", SqlDbType.Bit);
			cmd.Parameters[i++].Value = currency.DisplayFlag;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = currency.SiteGuid;
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = currency.IdentityGuid;
		}

		/// <summary>
		/// This method will return a sql command to update the currency
		/// line items.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="lineItem"></param>
		/// <param name="userID"></param>
		public void UpdateLineItem(SqlCommand cmd, CurrencyLineItemDO lineItem, string userID)
		{
			cmd.CommandText = "UPDATE tblCurrencyLineItems " +
						 "SET " +
						 "CurrencyGuid = @CurrencyGuid, " +
						 "Date = @effectiveDate, " +
						 "Rate = @rate, " +
						 "UpdatedBy = @updatedBy, " +
						 "UpdatedDate = @updatedDate " +
						 "WHERE CurrencyLineItemGuid = @CurrencyLineItemGuid";

			int i = 0;
			cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = lineItem.CurrencyGuid;
			cmd.Parameters.Add("@effectiveDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = lineItem.EffectiveDate;
			cmd.Parameters.Add("@rate", SqlDbType.Float);
			cmd.Parameters[i++].Value = lineItem.Rate;
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@CurrencyLineItemGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = lineItem.IdentityGuid;
		}

		/// <summary>
		/// This method will return a sql command to insert a currency
		/// line item.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="lineItem"></param>
		/// <param name="userID"></param>
		public void InsertLineItem(SqlCommand cmd, CurrencyLineItemDO lineItem, string userID)
		{
			// The SQL statement inserts the new item then selects the new items
			// autogenerated ID
			cmd.CommandText = "INSERT INTO tblCurrencyLineItems (" +
							"CurrencyGuid, Date, Rate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, CurrencyLineItemGuid) " +
							"VALUES (" +
							"@CurrencyGuid, @date, @rate, @createdBy, @createdDate, @updatedBy, @updatedDate, @CurrencyLineItemGuid)";

			// Prepare the command
			int i = 0;
			cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = lineItem.CurrencyGuid;
			cmd.Parameters.Add("@date", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = lineItem.EffectiveDate;
			cmd.Parameters.Add("@rate", SqlDbType.Float);
			cmd.Parameters[i++].Value = lineItem.Rate;
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@CurrencyLineItemGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = lineItem.IdentityGuid;
		}

		/// <summary>
		/// This method will return a sql command to insert a currency.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="currency"></param>
		/// <param name="userID"></param>
		public void Insert(SqlCommand cmd, CurrencyDO currency, string userID)
		{
			cmd.CommandText = "INSERT INTO tblCurrencies (" +
						 "LookupCurrencyUnitIndex, Country, UnitDisplayName, DisplayFlag, SiteGuid, " +
						 "CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, CurrencyGuid) " +
						 "VALUES (" +
						 "@LookupCurrencyUnitIndex, @country, @displayName, @displayFlag, @SiteGuid, " +
						 "@createdBy, @createdDate, @updatedBy, @updatedDate, @CurrencyGuid)";

			// Prepare the command
			int i = 0;
			cmd.Parameters.Add("@LookupCurrencyUnitIndex", SqlDbType.Int);
			cmd.Parameters[i++].Value = currency.LookupCurrencyUnitIndex;
			cmd.Parameters.Add("@country", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = currency.Country;
			cmd.Parameters.Add("@displayName", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = currency.UnitDisplayName;
			cmd.Parameters.Add("@displayFlag", SqlDbType.Bit);
			cmd.Parameters[i++].Value = currency.DisplayFlag;
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = currency.SiteGuid;
			cmd.Parameters.Add("@createdBy", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@createdDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@updatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters[i++].Value = userID;
			cmd.Parameters.Add("@updatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters[i++].Value = DateTimeOffset.Now;
			cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[i++].Value = currency.IdentityGuid;
		}

		/// <summary>
		/// Populates a SqlCommand object to checks to see if a Currency with the passed
		/// Currency's UnitDisplayName exists in the database
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="currency">The Currency to check</param>
		/// <param name="inTransaction">True if in DB transaction</param>
		public void Exists(SqlCommand cmd, CurrencyDO currency, bool inTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
            cmd.CommandText = "SELECT CurrencyGuid " +
						 "FROM tblCurrencies " + 
						 "WHERE UnitDisplayName = @displayName";

			cmd.Parameters.Add("@displayName", SqlDbType.NVarChar, 50);
			cmd.Parameters[0].Value = currency.UnitDisplayName;
		}

		/// <summary>
		/// Removes all line items but the ones in the list.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="indexListNotToDelete"></param>
		/// <param name="currencyGuid"></param>
		public void DeleteAllCurrencyLineItemsBut(SqlCommand cmd, string indexListNotToDelete, Guid currencyGuid)
		{
			cmd.CommandText = "DELETE tblCurrencyLineItems " +
						 "WHERE  CurrencyLineItemGuid NOT IN (" + indexListNotToDelete + ") " +
						 "AND CurrencyGuid = @CurrencyGuid";

			cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = currencyGuid;
		}

		/// <summary>
		/// Removes all line items for a Currency from the database
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="currencyGuid">The guid of the Currency for which line items will be removed</param>
		public void DeleteCurrencyLineItems(SqlCommand cmd, Guid currencyGuid)
		{
			cmd.CommandText = "DELETE tblCurrencyLineItems " +
						 "WHERE CurrencyGuid = @CurrencyGuid";

			cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = currencyGuid;
		}

		/// <summary>
		/// Removes the passed Currency from the database.
		/// </summary>
		/// <param name="cmd">SQL command object</param>
		/// <param name="currencyGuid">The guid of the Currency to remove.</param>
		public void Delete(SqlCommand cmd, Guid currencyGuid)
		{
			cmd.CommandText = "DELETE tblCurrencies " +
						 "WHERE  CurrencyGuid = @CurrencyGuid";

			// Prepare the command to delete the currency
			cmd.Parameters.Add("@CurrencyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters[0].Value = currencyGuid;
		}
		#endregion
	}
	#endregion
}
