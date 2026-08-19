// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelCardLimitLineItem.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Defines maximum quantities allowed to be disbursed for a fuel card in a particular period of time.
// A Fuel Card Limit is associated with zero to many fuel cards. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines the permitted periods for a fuel card limit
    /// </summary>
    public enum FuelCardLimitPeriod
    {
        Day = 0,
        Week = 1,
        Month = 2,
        Year = 3,
        Transactional = 4
    }

    /// <summary>
    /// Defines the type of fuel card limit line item.
    /// A line item can be associated with a product, a product group, or all products
    /// </summary>
    public enum FuelCardLimitLineItemType
    {
        Product = 0,
        ProductGroup = 1,
        AllProducts = 2
    }

    /// <summary>
    /// Defines maximum quantities allowed to be disbursed for a fuel card in a particular period of time.
    /// A Fuel Card Limit is associated with zero to many fuel cards. 
    /// </summary>
    [Serializable]
    [DataContract]
    public class FuelCardLimitLineItem : BaseDataObject
    {
        /// <summary>
        /// Create a Fuel Card Limit Line Item and use the volume units and decimal places for the site provided to initialize the record.
        /// </summary>
        /// <param name="site"></param>
        public FuelCardLimitLineItem(SiteClass site)
        {
            this.Limit = new SIDouble(site.VolumeUnits, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 0);
        }

        /// <summary>
        /// Identifies the fuel card limit this line item is associated with
        /// </summary>
        [DataMember]
        public Guid FuelCardLimitGuid { get; set; }

        /// <summary>
        /// The amount of fuel that is permitted to be disbursed to the associated fuel cards.
        /// The value is stored in the database in International Units (SI)
        /// </summary>
        [DataMember]
        public SIDouble Limit { get; set; }

        /// <summary>
        /// The time period to which this limit applies (day, month, week, etc.) 
        /// </summary>
        [DataMember]
        public FuelCardLimitPeriod Period { get; set; }

        /// <summary>
        /// A value indicating whether this line item is associated with a product, a product group, or all products
        /// </summary>
        public FuelCardLimitLineItemType LineItemType
        {
            get
            {
                if (this.ProductGuid != Guid.Empty)
                {
                    return FuelCardLimitLineItemType.Product;
                }
                else if (this.ProductGroupApplicationStringGuid != Guid.Empty)
                {
                    return FuelCardLimitLineItemType.ProductGroup;
                }
                else
                {
                    return FuelCardLimitLineItemType.AllProducts;
                }
            }
        }

        /// <summary>
        /// Identifies the product that this limit applies to.
        /// This will be empty if the limit is associated with a product group or all products
        /// </summary>
        [DataMember]
        public Guid ProductGuid { get; set; }

        /// <summary>
        /// Identifies the product group that this limit applies to.
        /// This will be empty if the limit is associated with a product or all products
        /// </summary>
        [DataMember]
        public Guid ProductGroupApplicationStringGuid { get; set; }

        /// <summary>
        /// The ID of the product or product group this limit is assigned to.
        /// Can also be "all products"
        /// </summary>
        [DataMember]
        public string AssignedProductGroupOrProductID { get; set; }

        /// <summary>
        /// Blanks out the data in the Fuel Card Limit Line Item
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            this.FuelCardLimitGuid = Guid.Empty;
            this.Limit.SIValue = 0;
            this.Period = FuelCardLimitPeriod.Day;
            this.ProductGuid = Guid.Empty;
            this.ProductGroupApplicationStringGuid = Guid.Empty;
            this.AssignedProductGroupOrProductID = string.Empty;
        }

        /// <summary>
        /// Read a Fuel Card Limit Line Item object from a DataSet
        /// </summary>
        /// <param name="set">A DataSet to read meter information from</param>
        /// <returns>True if loading the meter from the data set was successful</returns>
        public bool Load(DataSet set)
        {
            if (set == null)
            {
                throw new ArgumentNullException("set");
            }

            this.Reset();

            DataTable table = set.Tables[0];
            if (table.Rows.Count == 0)
            {
                return false;
            }

            DataRow row = table.Rows[0];

            this.IdentityGuid = DataObject.getValue(row["FuelCardLimitLineItemGuid"], Guid.Empty);
            this.FuelCardLimitGuid = DataObject.getValue(row["FuelCardLimitGuid"], Guid.Empty);
            this.Limit.SIValue = DataObject.getValue(row["Limit"], default(double));
            this.Period = DataObject.getValue(row["Period"], FuelCardLimitPeriod.Day);
            this.ProductGuid = DataObject.getValue(row["ProductGuid"], Guid.Empty);
            this.ProductGroupApplicationStringGuid = DataObject.getValue(row["ProductGroupApplicationStringGuid"], Guid.Empty);
            this.AssignedProductGroupOrProductID = DataObject.getValue(row["AssignedProductGroupOrProductID"], string.Empty);
            this.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
            this.CreatedBy = DataObject.getValue(row["CreatedBy"], ADMIN);
            this.UpdatedDate = DataObject.getValue(row["UpdatedDate"], this._CreatedDate);
            this.UpdatedBy = DataObject.getValue(row["UpdatedBy"], ADMIN);

            return true;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to insert a fuel card limit line item into the database 
        /// </summary>
        /// <param name="cmd">a SqlCommand object to populate</param>
        public void InsertSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitLineItemInsert";

            cmd.Parameters.Add("@FuelCardLimitLineItemGuid", SqlDbType.UniqueIdentifier).Value = this.IdentityGuid;
            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = this.FuelCardLimitGuid;
            cmd.Parameters.Add("@CreatedUpdatedBy", SqlDbType.NVarChar, 100).Value = this.CreatedBy;

            this.AddCommonInsertUpdateParameters(cmd);
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to update a fuel card limit line item in the database
        /// </summary>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void UpdateSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitLineItemUpdate";

            cmd.Parameters.Add("@FuelCardLimitLineItemGuid", SqlDbType.UniqueIdentifier).Value = this.IdentityGuid;
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100).Value = this.UpdatedBy;

            this.AddCommonInsertUpdateParameters(cmd);
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to delete a fuel card limit line item record in the database
        /// </summary>
        /// <param name="cmd">a SqlCommand object to populate</param>
        public void PurgeSQL(SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitLineItemDelete";

            cmd.Parameters.Add("@FuelCardLimitLineItemGuid", SqlDbType.UniqueIdentifier).Value = this.IdentityGuid;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read a fuel card limit line item record from the database by its primary key
        /// </summary>
        /// <param name="security"></param>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void SelectSQL(SecurityClass security, SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitLineItemGet";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
            cmd.Parameters.Add("@FuelCardLimitLineItemGuid", SqlDbType.UniqueIdentifier).Value = this.IdentityGuid;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read a fuel card limit line item record from the database by its natural key
        /// (the product or product group it is assigned to and the period)
        /// </summary>
        /// <param name="security"></param>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void SelectByAssignmentAndPeriodSQL(SecurityClass security, SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitLineItemGet";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = this.FuelCardLimitGuid;

            if (this.ProductGuid != Guid.Empty)
            {
                cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = this.ProductGuid;
            }
            else if (this.ProductGroupApplicationStringGuid != Guid.Empty)
            {
                cmd.Parameters.Add("@ProductGroupApplicationStringGuid", SqlDbType.UniqueIdentifier).Value = this.ProductGroupApplicationStringGuid;
            }

            cmd.Parameters.Add("@Period", SqlDbType.Int).Value = (int)this.Period;
        }

        /// <summary>
        /// Set up a SqlCommand object with the information necessary to read all line items for a particular fuel card limit
        /// </summary>
        /// <param name="security"></param>
        /// <param name="cmd">A SqlCommand object to populate</param>
        public void EnumerateSQL(SecurityClass security, SqlCommand cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardLimitLineItemGet";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = this.FuelCardLimitGuid;
        }

        /// <summary>
        /// Get a user friendly string for a line item type enumeration value so that we can display it.
        /// For example, change "AllProducts" to "All Products"
        /// </summary>
        /// <param name="lineItemType">The line item type enumeration value to convert</param>
        /// <returns>A user friendly string for a line item type enumeration value</returns>
        public static string GetUserFriendlyLineItemTypeEnumString(FuelCardLimitLineItemType lineItemType)
        {
            switch (lineItemType)
            {
                case FuelCardLimitLineItemType.AllProducts:
                    {
                        return "All Products";
                    }
                case FuelCardLimitLineItemType.ProductGroup:
                    {
                        return "Product Group";
                    }
                default:
                    {
                        return lineItemType.ToString();
                    }
            }
        }

        /// <summary>
        /// Get a user friendly string for the line item type enumeration value defined for this limit.
        /// Used for display purposes.
        /// </summary>
        /// <returns>A user friendly string for the line item's line item type enumeration value</returns>
        public string UserFriendlyLineItemType
        {
            get
            {
                return GetUserFriendlyLineItemTypeEnumString(this.LineItemType);
            }
        }

        /// <summary>
        /// Add parameters that are used by both the insert and update stored procedures
        /// </summary>
        /// <param name="cmd">A SqlCommand to add parameters to</param>
        private void AddCommonInsertUpdateParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@Limit", SqlDbType.Float).Value = this.Limit.SIValue;
            cmd.Parameters.Add("@Period", SqlDbType.Int).Value = this.Period;
            cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier).Value = this.ProductGuid == Guid.Empty ? DBNull.Value : (object)this.ProductGuid;
            cmd.Parameters.Add("@ProductGroupApplicationStringGuid", SqlDbType.UniqueIdentifier).Value = this.ProductGroupApplicationStringGuid == Guid.Empty ? DBNull.Value : (object)this.ProductGroupApplicationStringGuid;
        }
    }
}
