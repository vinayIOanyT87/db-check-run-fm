/// <summary>
///   File name:	ProductDO.cs
///   Purpose:	   The purpose of this class is to contain Product information.
///               It contains the SQL to retrieve the current product.
///				
///   Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA,
///				   2000.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///	2010-02-26		W.Gray		   		Correction to support Additive Volume
///
/// </summary>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data;

public class ProductDO
{
	public enum PRODUCT_TYPE
	{
		COMPONENT_PRODUCT = 0,
		BLEND_PRODUCT = 1,
		ADDITIVE_PRODUCT = 2,
		ADDITIZED_PRODUCT = 3,
		MAX_PRODUCT = 4
	};

	#region Private data members
	private string productID;
	private int productIndex;
	private PRODUCT_TYPE productType;
	private int volumeDecimalPlaces;
	private int volumeUnitIndex;
	private double volumeConversionFactor;
	private bool useProductVolumeConversionFactor;
	private double volumePackageSize;
	private int massDecimalPlaces;
	private int massUnitIndex;
	private bool aviationProduct;
	private double massConversionFactor;
	private bool useProductMassConversionFactor;
	private double massPackageSize;
	private bool loadByWeight;

	private const int MaxRoundValue = 2147483647;
	private const double ConvertValue = 1;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default constructor for the Product Data Object class
	/// </summary>
	public ProductDO()
	{
		this.productID = "";
		this.productIndex = 0;
		this.productType = PRODUCT_TYPE.COMPONENT_PRODUCT;
		this.volumeDecimalPlaces = 0;
		this.volumeUnitIndex = 1;
		this.volumeConversionFactor = 1;
		this.useProductVolumeConversionFactor = false;
		this.volumePackageSize = 0;
		this.massDecimalPlaces = 0;
		this.massUnitIndex = 1;
		this.massConversionFactor = 1;
		this.useProductMassConversionFactor = false;
		this.massPackageSize = 0;
		this.loadByWeight = false;
	}
	#endregion

	#region Properties
	/// <summary>
	/// This property sets and gets the Product ID data member.
	/// </summary>
	public string ProductID
	{
		get { return this.productID; }
		set
		{
			this.productID = value;
			if (this.productID == null)
			{
				this.productID = "";
			}
		}
	}

	/// <summary>
	/// This property sets and gets the Product Index data member.
	/// </summary>
	public int ProductIndex
	{
		get { return this.productIndex; }
		set { this.productIndex = value; }
	}

	/// <summary>
	/// This property sets and gets the Product Type data member.
	/// </summary>
	public PRODUCT_TYPE ProductType
	{
		get { return this.productType; }
		set { this.productType = value; }
	}

	/// <summary>
	/// This property gets the Site Volume Unit Index. The default is
	/// one (SI units).
	/// </summary>
	public int VolumeUnitIndex
	{
		get { return this.volumeUnitIndex; }
	}

	/// <summary>
	/// This property gets the Site Volume Decimal Places. The default
	/// is zero.
	/// </summary>
	public int VolumeDecimalPlaces
	{
		get { return this.volumeDecimalPlaces; }
	}

	/// <summary>
	/// This property will get the Conversion Factor from SI units.
	/// </summary>
	public double VolumeConversionFactor
	{
		get { return this.volumeConversionFactor; }
	}

	/// <summary>
	/// This property will return true if the product has a product volume unit index
	/// that is non-zero.
	/// </summary>
	public bool UseProductVolumeConversionFactor
	{
		get { return this.useProductVolumeConversionFactor; }
	}

	/// <summary>
	/// This property will return the volumePackageSize
	/// </summary>
	public double VolumePackageSize
	{
		get { return this.volumePackageSize; }
	}

	/// <summary>
	/// This property gets the Site Mass Unit Index. The default is
	/// one (SI units).
	/// </summary>
	public int MassUnitIndex
	{
		get { return this.massUnitIndex; }
	}

	/// <summary>
	/// This property gets the Site Mass Decimal Places. The default
	/// is zero.
	/// </summary>
	public int MassDecimalPlaces
	{
		get { return this.massDecimalPlaces; }
	}

	public bool AviationProduct
	{
		get { return aviationProduct; }
		set { aviationProduct = value; }
	}

	/// <summary>
	/// This property will get the Conversion Factor from SI units.
	/// </summary>
	public double MassConversionFactor
	{
		get { return this.massConversionFactor; }
	}

	/// <summary>
	/// This property will return true if the product has a product mass unit index
	/// that is non-zero.
	/// </summary>
	public bool UseProductMassConversionFactor
	{
		get { return this.useProductMassConversionFactor; }
	}

	/// <summary>
	/// This property will return the massPackageSize
	/// </summary>
	public double MassPackageSize
	{
		get { return this.massPackageSize; }
	}

	/// <summary>
	/// This property will return the loadByWeight
	/// </summary>
	public bool LoadByWeight
	{
		get { return this.loadByWeight; }
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// This method will determine if the entity to site map exists for a site.
	/// </summary>
	public bool IsProductAssigned(SqlConnection connection, int siteIndex)
	{
		DataSet dataSet = new DataSet();
		string sql = "SELECT * FROM tblEntityToSiteMap"
						+ " WHERE TypeID = 'Products'"
						+ " AND [Index] = @ProductIndex"
						+ " AND [SiteIndex] = @SiteIndex";

		SqlCommand command = new SqlCommand(sql, connection);

		command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
		command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);

		command.Parameters["@SiteIndex"].Value = siteIndex;
		command.Parameters["@ProductIndex"].Value = this.productIndex;

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		return (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0) ? true : false;
	}

	/// <summary>
	/// This method will retrieve product information such as the conversion factor and volume
	/// decimal places based on the product index.
	/// </summary>
	public void RetrieveProductInfo(SqlConnection a_connection)
	{
		DataSet dataSet = new DataSet();
		string sql = "SELECT p.ProductID, p.ProductIndex, p.ProductType, " +
						 "VolumeDecimalPlaces, VolumeUnitIndex, " +
						 "MassDecimalPlaces, MassUnitIndex, " +
						 "dbo.udf_ConvertFromSIUnits(@ConvertValue, VolumeUnitIndex, @RoundFactor) AS VolumeFactor, " +
						 "dbo.udf_ConvertFromSIUnits(@ConvertValue, MassUnitIndex, @RoundFactor) AS MassFactor, " +
						 "p.VolumePackageSize, p.MassPackageSize, p.LoadByWeight, p.UserData1 " +
						 "FROM tblProducts p WHERE p.ProductIndex = @ProductIndex";

		SqlCommand command = new SqlCommand(sql, a_connection);

		command.Parameters.Add("@ConvertValue", System.Data.SqlDbType.Float);
		command.Parameters.Add("@RoundFactor", System.Data.SqlDbType.Int);
		command.Parameters.Add("@ProductIndex", System.Data.SqlDbType.Int);

		command.Parameters["@ConvertValue"].Value = ProductDO.ConvertValue;
		command.Parameters["@RoundFactor"].Value = ProductDO.MaxRoundValue;
		command.Parameters["@ProductIndex"].Value = this.productIndex;

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		// Load the retrieved product information.
		this.LoadProductInfo(dataSet);
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// This method will load the retrieved product information such as the conversion factor and volume
	/// decimal.
	/// </summary>
	/// <param name="dataSet"></param>
	private void LoadProductInfo(DataSet dataSet)
	{
		if ((dataSet != null) && (dataSet.Tables.Count > 0))
		{
			DataTable table = dataSet.Tables[0];
			DataRow row = null;

			if (table.Rows.Count > 0)
			{
				row = table.Rows[0];

				this.productID = (row.IsNull("ProductID")) ? "" : (string)row["ProductID"];
				this.productIndex = (row.IsNull("ProductIndex")) ? 0 : Convert.ToInt32(row["ProductIndex"]);
				this.productType = (PRODUCT_TYPE)((row.IsNull("ProductType")) ? 0 : Convert.ToInt32(row["ProductType"]));
				this.volumeDecimalPlaces = (row.IsNull("VolumeDecimalPlaces")) ? 0 : Convert.ToInt32(row["VolumeDecimalPlaces"]);
				this.volumeUnitIndex = (row.IsNull("VolumeUnitIndex")) ? -1 : Convert.ToInt32(row["VolumeUnitIndex"]);
				this.volumeConversionFactor = (row.IsNull("VolumeFactor")) ? 1.0 : (double)row["VolumeFactor"];
				this.volumePackageSize = (row.IsNull("VolumePackageSize")) ? 0.0 : (double)(row["VolumePackageSize"]);
				this.massDecimalPlaces = (row.IsNull("MassDecimalPlaces")) ? 0 : Convert.ToInt32(row["MassDecimalPlaces"]);
				this.massUnitIndex = (row.IsNull("MassUnitIndex")) ? -1 : Convert.ToInt32(row["MassUnitIndex"]);
				this.massConversionFactor = (row.IsNull("MassFactor")) ? 1.0 : (double)row["MassFactor"];
				this.massPackageSize = (row.IsNull("MassPackageSize")) ? 0.0 : (double)(row["MassPackageSize"]);
				this.loadByWeight = (row.IsNull("LoadByWeight")) ? false : Convert.ToBoolean(row["LoadByWeight"]);
				// If the volume unit index is non-zero, then that means the ledger
				// should use the product convertion factor and not the site's convertion
				// factor.
				if (this.volumeUnitIndex > 0)
				{
					this.useProductVolumeConversionFactor = true;
				}

				// If the mass unit index is non-zero, then that means the ledger
				// should use the product convertion factor and not the site's convertion
				// factor.
				if (this.massUnitIndex > 0)
				{
					this.useProductMassConversionFactor = true;
				}

				if (row.IsNull("UserData1") == true)
				{
					this.aviationProduct = false;
				}
				else
				{
					if (string.Equals("Yes", (string)row["UserData1"], StringComparison.InvariantCultureIgnoreCase) == true)
					{
						aviationProduct = true;
					}
					else
					{
						aviationProduct = false;
					}
				}
			}
		}
	}
	#endregion
}
