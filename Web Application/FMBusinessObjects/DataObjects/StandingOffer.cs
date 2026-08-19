/// <summary>
///	FILE NAME: StandingOffer.cs
///	PURPOSE:   StandingOfferClass
///
///	COMMENTS:
///		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Endress+Hauser.
///
///	AUTHOR(S):	W. Gray
///	VERSION:	   1.0.0  Current version
///
///	MODIFICATION HISTORY:
///		Date:		   By:					Reason:
///		----------	-----------------	-------------------------------------------
///		2007-11-14	Richard Panachida	Added a new method to return the standing offer index
///										      for a product and current period combination.
///		2008-08-25  A. Coker			   Added destination location.
///      2009-06-18  A. Coker          Fixed Defect 3970. Modified GetIndexSQL(SecurityClass security, 
///                                    int supplierIndex, int productIndex, int locationIndex, string currentPeriod)
///                                    to allow supplierIndex to be optional (non-positive value will exclude supplierindex from query) .
///      2009-06-18  A. Coker          Fixed defect 4137. Modified GetIndexSQL. If can't find a standing offer with an effective date range
///                                    satisfying the inventory date, get the most recent and not the future.
/// 
///      2009-06-24  A. Coker          Fixed defect 4166 - Get standing offer with lowest price, given other conditions are the same.
///
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;

using FMBusinessObjects.Constants;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
	#region Standing Offer Collection Class
   [Serializable]
   [CollectionDataContract]
	public class StandingOfferCollectionClass : List<StandingOfferClass> { }
	#endregion

	#region Standing Offer class

	/// <summary>
	/// Data Object class for Standing Offers, which are now known as Price List.
	/// </summary>
   [Serializable]
   [DataContract]
	[EntityImportExportWorksheetAttribute("STANDINGOFFERS")]
	public class StandingOfferClass : BaseDataObject
	{
		#region Public data members
		public const string LOCATION_NONE = "None";
		#endregion

		#region Protected Data Members
		[DataMember]
		protected string _SupplierID;
		[DataMember]
		protected string _ProductID;
		[DataMember]
		protected string _LocationID;
		[DataMember]
		protected string _LocationName;
		[DataMember]
		protected Guid _SupplierGuid;
		[DataMember]
		protected Guid _ProductGuid;
		[DataMember]
		protected Guid _LocationGuid;
		[DataMember]
		protected double _StandingOfferPrice;
		[DataMember]
		protected int _LowerBound;
		[DataMember]
		protected int _UpperBound;
		[DataMember]
		protected string _ReferenceNumber;
		[DataMember]
		protected DateTimeOffset _EffectiveDate;
		[DataMember]
		protected DateTimeOffset _ExpirationDate;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the price list entry (aka standing offer) class.
		/// </summary>
		public StandingOfferClass()
		{
			this.Reset();

			base.ID = "unassigned";
		}
		#endregion

		#region Properties
		[EntityImportExportAttribute("SITE*", 105, "SITEGUID")]
		new public Guid SiteGuid { get { return _SiteGuid; } set { _SiteGuid = value; } }

		[EntityImportExportAttribute("STANDINGOFFERID*", 100)]
		new public Guid IdentityGuid
		{
			get { return base._IdentityGuid; }
			set { base._IdentityGuid = value; }
		}

		// This here to prevent ID from exporting during entity export
		new public string ID
		{
			get { return base._ID; }
			set { base._ID = value; }
		}

		/// <summary>
		/// This property sets and gets the Supplier ID of type string.
		/// </summary>
		[EntityImportExportAttribute("SUPPLIER", 195, "SupplierID")]
		public string SupplierID
		{
			get { return this._SupplierID; }
			set { this._SupplierID = value; }
		}

		/// <summary>
		/// This property sets and gets the Product ID of type string.
		/// </summary>
		[EntityImportExportAttribute("FUELTYPE", 70, "ProductID")]
		public string ProductID
		{
			get { return this._ProductID; }
			set { this._ProductID = value; }
		}

		/// <summary>
		/// This property sets and gets the Location ID of type string.
		/// </summary>
		[EntityImportExportAttribute("LOCATION", 195, "LocationID")]
		public string LocationID
		{
			get { return this._LocationID; }
			set { this._LocationID = value; }
		}

		/// <summary>
		/// This property sets and gets the Location Name of type string.
		/// </summary>
		public string LocationName
		{
			get { return this._LocationName; }
			set { this._LocationName = value; }
		}
		/// <summary>
		/// This property sets and gets the Supplier Guid.
		/// </summary>
		public Guid SupplierGuid
		{
			get { return this._SupplierGuid; }
			set { this._SupplierGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the Product Guid.
		/// </summary>
		public Guid ProductGuid
		{
			get { return this._ProductGuid; }
			set { this._ProductGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the Location Guid.
		/// </summary>
		public Guid LocationGuid
		{
			get { return this._LocationGuid; }
			set { this._LocationGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the Standing Offer Price (aka Price List Price) of type float.
		/// </summary>
		[EntityImportExportAttribute("STANDINGOFFERPRICE", 190, "StandingOfferPrice")]
		public double StandingOfferPrice
		{
			get { return this._StandingOfferPrice; }
			set { this._StandingOfferPrice = value; }
		}

		/// <summary>
		/// This property sets and gets the Effective Date of type DateTimeOffset.
		/// </summary>
		[EntityImportExportAttribute("EFFECTIVEDATE", 90)]
		public DateTimeOffset EffectiveDate
		{
			get { return this._EffectiveDate; }
			set { this._EffectiveDate = value; }
		}

		/// <summary>
		/// This property sets and gets the Expiration Date of type DateTimeOffset.
		/// </summary>
		[EntityImportExportAttribute("EXPIRATIONDATE", 90)]
		public DateTimeOffset ExpirationDate
		{
			get { return this._ExpirationDate; }
			set { this._ExpirationDate = value; }
		}

		/// <summary>
		/// This property sets and gets the Lower Bound data member value.
		/// </summary>
		[EntityImportExportAttribute("LOWERBOUND", 190)]
		public int LowerBound
		{
			get { return this._LowerBound; }
			set { this._LowerBound = value; }
		}

		/// <summary>
		/// This property sets and gets the Upper Bound data member value.
		/// </summary>
		[EntityImportExportAttribute("UPPERBOUND", 190)]
		public int UpperBound
		{
			get { return this._UpperBound; }
			set { this._UpperBound = value; }
		}

		/// <summary>
		/// This property sets and gets the Reference Number data member value.
		/// </summary>
		[EntityImportExportAttribute("REFERENCENUMBER", 195)]
		public string ReferenceNumber
		{
			get { return this._ReferenceNumber; }
			set { this._ReferenceNumber = value; }
		}

		public string StandingOfferID
		{
			get { /*CreateNewID();*/ return base.ID; }
		}


		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.STANDING_OFFER; }
		}

		/// <summary>
		/// This property returns the Parent Entity Type for this object. It is used for
		/// auditing. 
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#endregion

		#region Public methods
		/// <summary>
		/// This method resets the object to its initial state.
		/// </summary>
		public override void Reset()
		{
			base.Reset();

			this._SupplierID = "";
			this._ExpirationDate = TimeConverter.Today();
			this._EffectiveDate = TimeConverter.Today();
			this._ProductID = "";
			this._LocationID = "";
			this._LocationName = LOCATION_NONE;
			this._StandingOfferPrice = 0.0;
			this._ProductGuid = Guid.Empty;
			this._SupplierGuid = Guid.Empty;
			this._LocationGuid = Guid.Empty;
			this._UpperBound = 0;
			this._LowerBound = 0;
			this._ReferenceNumber = "";
		}

		/// <summary>
		/// This method will pad the date value with a zero and return a 
		/// two digit number as a string.
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		private string Pad(int num)
		{
			string outStr = num.ToString();

			if (num < 10)
			{
				outStr = "0" + num.ToString();
			}

			return outStr;
		}

		/// <summary>
		/// This method loads the object with the information from the 
		/// database.
		/// </summary>
		/// <param name="Set"></param>
		public void Load(DataSet dataSet)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException("Set");
			}

			this.Reset();

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			base._IdentityGuid = DataObject.getValue<Guid>(row["StandingOfferGuid"], Guid.Empty);
			base._SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			base._ID = DataObject.getValue<string>(row["ID"], "");
			this._SupplierGuid = DataObject.getValue<Guid>(row["SupplierCompanyGuid"], Guid.Empty);
			this._ProductGuid = DataObject.getValue<Guid>(row["ProductGuid"], Guid.Empty);
			this._LocationGuid = DataObject.getValue<Guid>(row["LocationIATAGuid"], Guid.Empty);
			this._EffectiveDate = DataObject.getValue<DateTimeOffset>(row["EffectiveDate"], TimeConverter.Today());
			this._ExpirationDate = DataObject.getValue<DateTimeOffset>(row["ExpirationDate"], TimeConverter.Today());
			base._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			base._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			base._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			base._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], _CreatedDate);
			this._UpperBound = DataObject.getValue<int>(row["UpperBound"], 0);
			this._LowerBound = DataObject.getValue<int>(row["LowerBound"], 0);
			this._SupplierID = DataObject.getValue<string>(row["SupplierID"], "");
			this._ProductID = DataObject.getValue<string>(row["ProductID"], "");
			this._LocationID = DataObject.getValue<string>(row["LocationID"], "");
			this._LocationName = DataObject.getValue<string>(row["LocationName"], LOCATION_NONE);
			this._StandingOfferPrice = DataObject.getValue<double>(row["StandingOfferPrice"], 0.0);
			this._ReferenceNumber = DataObject.getValue<string>(row["ReferenceNumber"], "");
		}

		/// <summary>
		/// This method will return true if the query found an overlapping price list entry (aka standing offer).
		/// Otherwise it will return false.  The default is false.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public bool LoadOverlap(DataSet dataSet)
		{
			bool isOverlap = false;

			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					DataRow row = table.Rows[0];
					Guid standingOfferGuid = DataObject.getValue<Guid>(row["StandingOfferGuid"], Guid.Empty);

					if (standingOfferGuid != Guid.Empty)
					{
						isOverlap = true;
					}
				}
			}

			return isOverlap;
		}

		/// <summary>
		/// This method will return a SQL statement that gets the most current price list entry (aka standing offer) guid
		/// or one within the current period.  It ignores the lower and upper boundary.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		/// <param name="supplierGuid"></param>
		/// <param name="productGuid"></param>
		/// <param name="locationGuid"></param>
		/// <param name="currentPeriod"></param>
		/// <returns></returns>
		public void GetIdentityGuidSQL(SqlCommand cmd,
												SecurityClass security,
												Guid supplierGuid,
												Guid productGuid,
												Guid locationGuid,
												DateTimeOffset? currentPeriod)
		{
			bool mostRecent = false;
			double? quantity = null;

			this.GetIdentityGuidSQL(cmd, security, supplierGuid, productGuid, locationGuid, currentPeriod, quantity, mostRecent);
		}

		/// <summary>
		/// This method will return a SQL statement that gets the most current price list entry (aka standing offer) guid
		/// or one within the current period.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		/// <param name="supplierGuid"></param>
		/// <param name="productGuid"></param>
		/// <param name="locationGuid"></param>
		/// <param name="currentPeriod"></param>
		/// <param name="quantity"></param>
		/// <param name="mostRecent"></param>
		/// <returns></returns>
		public void GetIdentityGuidSQL(SqlCommand cmd,
												SecurityClass security,
												Guid supplierGuid,
												Guid productGuid,
												Guid locationGuid,
												DateTimeOffset? currentPeriod,
												double? quantity,
												bool mostRecent)
		{
			string select = "SELECT TOP(1) StandingOfferGuid, EffectiveDate " +
							"FROM tblStandingOffers " +
							"WHERE " + SiteWhereClause(security, "tblStandingOffers", "StandingOfferGuid") +
							" AND ProductGuid = @ProductGuid";

			cmd.Parameters.AddWithValue("@ProductGuid", productGuid);

			if (supplierGuid != Guid.Empty)
			{
				select += " AND SupplierCompanyGuid = @SupplierGuid";
				cmd.Parameters.AddWithValue("@SupplierGuid", supplierGuid);
			}

			if (locationGuid != Guid.Empty)
			{
				select += " AND LocationIATAGuid = @LocationGuid";
				cmd.Parameters.AddWithValue("@LocationGuid", locationGuid);
			}

			if (quantity != null)
			{
				select += " AND @Quantity >= LowerBound AND @Quantity <= UpperBound";
				cmd.Parameters.AddWithValue("@Quantity", quantity.Value);
			}

			if (currentPeriod == null)
			{
				cmd.CommandText = select;
				return;
			}

			if (mostRecent == false)
			{
				select = select + " AND @CurrentPeriod >= EffectiveDate AND @CurrentPeriod <= ExpirationDate";
				cmd.Parameters.AddWithValue("@CurrentPeriod", currentPeriod);
			}
			else
			{
				//This select will be used in case there are no Standing Offers (aka Price List) found with an effective date range inclusive of the
				//inventory date. Then it will search for most recent price list entry (aka standing offer) that satisfy given conditions.
				select = select + " AND @CurrentPeriod >= EffectiveDate";
				cmd.Parameters.AddWithValue("@CurrentPeriod", currentPeriod);
			}

			select = select + " ORDER BY EffectiveDate DESC, StandingOfferPrice ASC";

			string sql =
				"BEGIN " +
					"CREATE TABLE #TEMP (StandingOfferGuid uniqueidentifier, EffectiveDate DateTimeOffset) " +
					"INSERT INTO #TEMP " + select + " " +
					"SELECT * FROM #TEMP " +
					"DROP TABLE #TEMP " +
				"END";


			cmd.CommandText = sql;
		}

		/// <summary>
		/// This method will return a sql statement that gets the most current price list entry (aka standing offer) guid or
		/// one within the current period for a given product.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		/// <param name="productGuid"></param>
		/// <param name="currentPeriod"></param>
		/// <returns></returns>
		public void GetIdentityGuidSQL(SqlCommand cmd, SecurityClass security, Guid productGuid, DateTimeOffset? currentPeriod)
		{
			GetIdentityGuidSQL(cmd, security, Guid.Empty, productGuid, Guid.Empty, currentPeriod);
		}

		/// <summary>
		/// This method will return the enumeration SQL to retrieve all the price list entry (aka standing offer)
		/// records based on the security context.
		/// </summary>
		/// <param name="Security"></param>
		/// <returns></returns>
		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT * FROM vw_StandingOffers" +
				  " WHERE" + SiteWhereClause(security, "vw_StandingOffers", "StandingOfferGuid") +
				  " ORDER BY vw_StandingOffers.EffectiveDate DESC, SupplierID";
		}

		/// <summary>
		/// This method will return the enumeration SQL to retrieve all the Standing Offers (aka Price List)
		/// that match the suppler, product, effective date, and site group. 
		/// </summary>
		/// <param name="security"></param>
		/// <param name="filterList"></param>
		/// <returns></returns>
		public void EnumerateSQLWithFilter(SqlCommand cmd, SecurityClass security, StandingOfferFilterClass filterList, int limit)
		{
			string sql = "";
			string whereFilter = "";

			if (filterList.SupplierGuid != Guid.Empty)
			{
				whereFilter = whereFilter + " AND vw_StandingOffers.SupplierCompanyGuid = @SupplierGuid";
				cmd.Parameters.AddWithValue("@SupplierGuid", filterList.SupplierGuid);
			}

			if (filterList.ProductGuid != Guid.Empty)
			{
				whereFilter = whereFilter + " AND vw_StandingOffers.ProductGuid = @ProductGuid";
				cmd.Parameters.AddWithValue("@ProductGuid", filterList.ProductGuid);
			}

			if (filterList.LocationGuid != Guids.AllFilterGuid)
			{
				if (filterList.LocationGuid == Guid.Empty)
				{
					whereFilter = whereFilter + " AND vw_StandingOffers.LocationIATAGuid IS NULL";
				}
				else
				{
					whereFilter = whereFilter + " AND vw_StandingOffers.LocationIATAGuid = @LocationGuid";
					cmd.Parameters.AddWithValue("@LocationGuid", filterList.LocationGuid);
				}
			}

			if ((filterList.EffectiveStartDate != null) && (filterList.EffectiveEndDate != null))
			{
				whereFilter = whereFilter + " AND vw_StandingOffers.EffectiveDate >= @EffectiveStartDate" +
													" AND vw_StandingOffers.EffectiveDate <= @EffectiveEndDate";

				// Truncate time portion of these date-times
				cmd.Parameters.AddWithValue("@EffectiveStartDate", TimeConverter.ToDate(filterList.EffectiveStartDate.Value));
				cmd.Parameters.AddWithValue("@EffectiveEndDate", TimeConverter.ToDate(filterList.EffectiveEndDate.Value));
			}

			if (filterList.ReferenceNumber != null)
			{
				whereFilter = whereFilter + " AND vw_StandingOffers.ReferenceNumber = UPPER(@ReferenceNumber)";
				cmd.Parameters.AddWithValue("@ReferenceNumber", filterList.ReferenceNumber);
			}

			if (limit > 0)
			{
				sql = "SELECT TOP " + limit.ToString();
			}
			else
			{
				sql = "SELECT";
			}

			sql += " * FROM vw_StandingOffers" +
					" WHERE" + SiteWhereClause(security, "vw_StandingOffers", "StandingOfferGuid") + whereFilter +
					" ORDER BY vw_StandingOffers.EffectiveDate DESC, SupplierID";

			cmd.CommandText = sql;
		}
		#endregion

		#region SQL properties
		/// <summary>
		/// This property returns an insert SQL to insert a new price list entry (aka standing offer) record in the database.
		/// </summary>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblStandingOffers" +
				"(" +
					"SiteGuid," +
					"SupplierCompanyGuid," +
					"EffectiveDate," +
					"ExpirationDate," +
					"ProductGuid," +
					"LocationIATAGuid," +
					"StandingOfferPrice," +
					"LowerBound," +
					"UpperBound," +
					"ReferenceNumber," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"StandingOfferGuid" +
				") VALUES (" +
					"@SiteGuid," +
					"@SupplierGuid," +
					"@EffectiveDate," +
					"@ExpirationDate," +
					"@ProductGuid," +
					"@LocationGuid," +
					"@StandingOfferPrice," +
					"@LowerBound," +
					"@UpperBound," +
					"@ReferenceNumber," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@StandingOfferGuid"+
				")";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@SupplierGuid", _SupplierGuid);
			// Truncate time portion of these date-times
			cmd.Parameters.AddWithValue("@EffectiveDate", TimeConverter.ToDate(_EffectiveDate));
			cmd.Parameters.AddWithValue("@ExpirationDate", TimeConverter.ToDate(_ExpirationDate));
			cmd.Parameters.AddWithValue("@ProductGuid", _ProductGuid);
			if (_LocationGuid != Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@LocationGuid", _LocationGuid);
			}
			else
			{
				cmd.Parameters.AddWithValue("@LocationGuid", DBNull.Value);
			}
			cmd.Parameters.AddWithValue("@StandingOfferPrice", _StandingOfferPrice);
			cmd.Parameters.AddWithValue("@LowerBound", _LowerBound);
			cmd.Parameters.AddWithValue("@UpperBound", _UpperBound);
			cmd.Parameters.AddWithValue("@ReferenceNumber", _ReferenceNumber);
			cmd.Parameters.AddWithValue("@CreatedDate", _CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", _CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@ID", base._ID);
			cmd.Parameters.AddWithValue("@StandingOfferGuid", _IdentityGuid);
		}

		/// <summary>
		/// This property returns an update SQL used to update an existing price list entry (aka standing offer) record.
		/// </summary>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblStandingOffers SET " +
					"SiteGuid = @SiteGuid," +
					"SupplierCompanyGuid = @SupplierGuid," +
					"EffectiveDate = @EffectiveDate," +
					"ExpirationDate = @ExpirationDate," +
					"ProductGuid = @ProductGuid," +
					"LocationIATAGuid = @LocationGuid," +
					"StandingOfferPrice = @StandingOfferPrice," +
					"LowerBound = @LowerBound," +
					"UpperBound = @UpperBound," +
					"ReferenceNumber = @ReferenceNumber," +
					"UpdatedDate = @UpdatedDate," +
					"UpdatedBy = @UpdatedBy " +
					"WHERE StandingOfferGuid = @StandingOfferGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@SupplierGuid", _SupplierGuid);
			// Truncate time portion of these date-times
			cmd.Parameters.AddWithValue("@EffectiveDate", TimeConverter.ToDate(_EffectiveDate));
			cmd.Parameters.AddWithValue("@ExpirationDate", TimeConverter.ToDate(_ExpirationDate));
			cmd.Parameters.AddWithValue("@ProductGuid", _ProductGuid);
			if (_LocationGuid != Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@LocationGuid", _LocationGuid);
			}
			else
			{
				cmd.Parameters.AddWithValue("@LocationGuid", DBNull.Value);
			}
			cmd.Parameters.AddWithValue("@StandingOfferPrice", _StandingOfferPrice);
			cmd.Parameters.AddWithValue("@LowerBound", _LowerBound);
			cmd.Parameters.AddWithValue("@UpperBound", _UpperBound);
			cmd.Parameters.AddWithValue("@ReferenceNumber", _ReferenceNumber);
			cmd.Parameters.AddWithValue("@UpdatedDate", _UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", _UpdatedBy);
			cmd.Parameters.AddWithValue("@StandingOfferGuid", _IdentityGuid);
		}

		/// <summary>
		/// This property returns the SQL string to delete a price list entry (aka standing offer) record from the database.
		/// </summary>
		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblStandingOffers WHERE StandingOfferGuid = @StandingOfferGuid";
			cmd.Parameters.AddWithValue("@StandingOfferGuid", _IdentityGuid);
		}

		/// <summary>
		/// This property returns the SQL select by identity guid string.
		/// </summary>
		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM vw_StandingOffers " + SQLUpdateLock(bInTransaction) + " WHERE StandingOfferGuid = @StandingOfferGuid";
			cmd.Parameters.AddWithValue("@StandingOfferGuid", _IdentityGuid);
		}

		/// <summary>
		/// This property returns the SQL select by ID string.
		/// </summary>
		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM vw_StandingOffers " + SQLUpdateLock(bInTransaction) +
					" WHERE" + SiteWhereClause(security, "vw_StandingOffers", "StandingOfferGuid") +
					" AND ID = @ID";

			cmd.Parameters.AddWithValue("@ID", base._ID);
		}

		/// <summary>
		/// This method will return an SQL string that retrieves a price list entry (aka standing offer) that matches
		/// the supplier, product, effective date, expiration date, and the lower bound is overlapping.
		/// </summary>
		/// <param name="bInTransaction"></param>
		/// <returns></returns>
		public void SelectOverlapSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT StandingOfferGuid FROM vw_StandingOffers " + BaseDataObject.SQLUpdateLock(bInTransaction) +
									" WHERE SupplierCompanyGuid = @SupplierGuid" +
									" AND ProductGuid = @ProductGuid" +
									" AND EffectiveDate = @EffectiveDate" +
									" AND ExpirationDate = @ExpirationDate" +
									" AND ((@LowerBound > LowerBound AND @LowerBound < UpperBound) " +
									" OR (@upperBound > LowerBound AND @upperBound < UpperBound) " +
									" OR (@LowerBound <= LowerBound AND @upperBound >= UpperBound)) " +
									" AND StandingOfferGuid <> @StandingOfferGuid";

			cmd.Parameters.AddWithValue("@SupplierGuid", _SupplierGuid);
			cmd.Parameters.AddWithValue("@ProductGuid", _ProductGuid);
			// Truncate time portion of these date-times
			cmd.Parameters.AddWithValue("@EffectiveDate", TimeConverter.ToDate(_EffectiveDate));
			cmd.Parameters.AddWithValue("@ExpirationDate", TimeConverter.ToDate(_ExpirationDate));
			cmd.Parameters.AddWithValue("@LowerBound", _LowerBound);
			cmd.Parameters.AddWithValue("@UpperBound", _UpperBound);
			cmd.Parameters.AddWithValue("@StandingOfferGuid", _IdentityGuid);
		}

		public void BuildIDSQLUsingGuids(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT dbo.udf_buildStandingOfferID(" +
					"@SiteGuid," +
					"@SupplierGuid," +
					"@ProductGuid," +
					"@LocationGuid," +
					"@EffectiveDate," +
					"@ExpirationDate," +
					"@LowerBound," +
					"@UpperBound" +
				") AS ID";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@SupplierGuid", _SupplierGuid);
			cmd.Parameters.AddWithValue("@ProductGuid", _ProductGuid);
			cmd.Parameters.AddWithValue("@LocationGuid", _LocationGuid);
			// Truncate time portion of these date-times
			cmd.Parameters.AddWithValue("@EffectiveDate", TimeConverter.ToDate(_EffectiveDate));
			cmd.Parameters.AddWithValue("@ExpirationDate", TimeConverter.ToDate(_ExpirationDate));
			cmd.Parameters.AddWithValue("@LowerBound", _LowerBound);
			cmd.Parameters.AddWithValue("@UpperBound", _UpperBound);
		}

		public void BuildIDSQLUsingIDs(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT dbo.udf_buildStandingOfferID(" +
				"@SiteGuid," +
				"(SELECT tblCompanies_ID.MasterRecordGuid FROM (select * from tblCompanies where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@SiteGuid))) tblCompanies_ID WHERE tblCompanies_ID.ID = @SupplierID)," +
				"(SELECT _MasterRecordGuid FROM tblProducts p WITH(NOLOCK) inner join [erv].[udf_GetProductRecordVersions](@SiteGuid) rp on p.ProductGuid = rp.ProductGuid WHERE p.ProductID = @ProductID) AS ProductGuid," +
				"(SELECT IATAGuid    FROM tblIATA      WHERE IATAID    = @LocationID)," +
				"@EffectiveDate," +
				"@ExpirationDate," +
				"@LowerBound," +
				"@UpperBound" +
				") AS ID";

			cmd.Parameters.AddWithValue("@SiteGuid", _SiteGuid);
			cmd.Parameters.AddWithValue("@SupplierID", _SupplierID);
			cmd.Parameters.AddWithValue("@ProductID", _ProductID);
			cmd.Parameters.AddWithValue("@LocationID", _LocationID);
			// Truncate time portion of these date-times
			cmd.Parameters.AddWithValue("@EffectiveDate", TimeConverter.ToDate(_EffectiveDate));
			cmd.Parameters.AddWithValue("@ExpirationDate", TimeConverter.ToDate(_ExpirationDate));
			cmd.Parameters.AddWithValue("@LowerBound", _LowerBound);
			cmd.Parameters.AddWithValue("@UpperBound", _UpperBound);
		}

		#endregion
	}
	#endregion

	#region Standing offer filter class
   [Serializable]
   [DataContract]
	public class StandingOfferFilterClass
	{
		#region Private data members
		[DataMember]
		private Guid supplierGuid;
		[DataMember]
		private Guid productGuid;
		[DataMember]
		private Guid locationGuid;
		[DataMember]
		private DateTimeOffset? effectiveStartDate;
		[DataMember]
		private DateTimeOffset? effectiveEndDate;
		[DataMember]
		private string referenceNumber;
		#endregion

		#region Constructors
		/// <summary>
		/// This the default constructor for the standing offer (aka price list) filter class.
		/// </summary>
		public StandingOfferFilterClass()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public Guid SupplierGuid
		{
			get { return this.supplierGuid; }
			set { this.supplierGuid = value; }
		}

		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		public Guid LocationGuid
		{
			get { return this.locationGuid; }
			set { this.locationGuid = value; }
		}

		public DateTimeOffset? EffectiveStartDate
		{
			get { return this.effectiveStartDate; }
			set { this.effectiveStartDate = value; }
		}

		public DateTimeOffset? EffectiveEndDate
		{
			get { return this.effectiveEndDate; }
			set { this.effectiveEndDate = value; }
		}

		public string ReferenceNumber
		{
			get { return this.referenceNumber; }
			set { this.referenceNumber = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.supplierGuid = Guid.Empty;
			this.productGuid = Guid.Empty;
			this.locationGuid = Guid.Empty;
			this.effectiveEndDate = null;
			this.effectiveStartDate = null;
			this.referenceNumber = null;
		}
		#endregion
	}
	#endregion
}
