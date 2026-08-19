namespace LedgerCore
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	public class LRProductDO
	{
		public enum PRODUCT_TYPE
		{
			ComponentProduct	= 0,
			BlendProduct		= 1,
			AdditiveProduct		= 2,
			AdditizedProduct	= 3,
			MaxProduct			= 4
		};

		#region Private data members
		private string productID;
		private Guid productGuid;
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

		public double VarianceTolerance { get; set; }

		private const int MaxRoundValue = 2147483647;
		private const double ConvertValue = 1;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Product Data Object class
		/// </summary>
		public LRProductDO()
		{
			this.Init();
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
		/// This property sets and gets the Product Guid data member.
		/// </summary>
		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
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
			get { return this.aviationProduct; }
			set { this.aviationProduct = value; }
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
		/// This method will retrieve product information such as the conversion factor and volume
		/// decimal places based on the product guid.
		/// </summary>
		public void RetrieveProductInfo(LedgerConnection ledgerConnection, Guid siteGuid)
		{
			using (var command = new SqlCommand())
			{
				command.CommandText =	"SELECT "
										+ "p.ProductID, "
										+ "p.ProductGuid, "
										+ "p.LookupProductTypeIndex, " 
										+ "VolumeDecimalPlaces, VolumeUnitIndex, "
										+ "MassDecimalPlaces, "
										+ "MassUnitIndex, " 
										+ "VarianceTolerance," 
										+ "dbo.udf_ConvertFromSIUnits(@ConvertValue, VolumeUnitIndex, @RoundFactor) AS VolumeFactor, " 
										+ "dbo.udf_ConvertFromSIUnits(@ConvertValue, MassUnitIndex, @RoundFactor) AS MassFactor, " 
										+ "p.VolumePackageSize, "
										+ "p.MassPackageSize, "
										+ "p.LoadByWeight, "
										+ "p.UserData1 " 
										+ "FROM tblProducts p Inner Join [erv].[udf_GetProductRecordVersions](@SiteGuid) rp " 
										+ "On p.ProductGuid = rp.ProductGuid " 
										+ "Where rp.MasterRecordGuid = @ProductGuid";

				command.Parameters.Add("@ConvertValue", SqlDbType.Float);
				command.Parameters.Add("@RoundFactor", SqlDbType.Int);
				command.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
				command.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

				command.Parameters["@ConvertValue"].Value = ConvertValue;
				command.Parameters["@RoundFactor"].Value = MaxRoundValue;
				command.Parameters["@ProductGuid"].Value = this.productGuid;
				command.Parameters["@SiteGuid"].Value = siteGuid;

				DataSet dataSet = ledgerConnection.GetDataSet(command);

				// Load the retrieved product information.
				this.LoadProductInfo(dataSet);
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will load the retrieved product information such as the conversion factor and volume
		/// decimal.
		/// </summary>
		/// <param name="dataSet"></param>
		public void LoadProductInfo(DataSet dataSet)
		{
			if ((dataSet != null) && (dataSet.Tables.Count > 0))
			{
				DataTable table = dataSet.Tables[0];
				DataRow row = null;

				if (table.Rows.Count > 0)
				{
					row = table.Rows[0];

					this.productID				= (row.IsNull("ProductID")) ? string.Empty : (string)row["ProductID"];
					this.productGuid			= (row.IsNull("ProductGuid")) ? Guid.Empty : (Guid)(row["ProductGuid"]);
					this.productType			= (PRODUCT_TYPE)((row.IsNull("LookupProductTypeIndex")) ? 0 : Convert.ToInt32(row["LookupProductTypeIndex"]));
					this.volumeDecimalPlaces	= (row.IsNull("VolumeDecimalPlaces")) ? 0 : Convert.ToInt32(row["VolumeDecimalPlaces"]);
					this.volumeUnitIndex		= (row.IsNull("VolumeUnitIndex")) ? -1 : Convert.ToInt32(row["VolumeUnitIndex"]);
					this.volumeConversionFactor = (row.IsNull("VolumeFactor")) ? 1.0 : (double)row["VolumeFactor"];
					this.volumePackageSize		= (row.IsNull("VolumePackageSize")) ? 0.0 : (double)(row["VolumePackageSize"]);
					this.massDecimalPlaces		= (row.IsNull("MassDecimalPlaces")) ? 0 : Convert.ToInt32(row["MassDecimalPlaces"]);
					this.massUnitIndex			= (row.IsNull("MassUnitIndex")) ? -1 : Convert.ToInt32(row["MassUnitIndex"]);
					this.massConversionFactor	= (row.IsNull("MassFactor")) ? 1.0 : (double)row["MassFactor"];
					this.massPackageSize		= (row.IsNull("MassPackageSize")) ? 0.0 : (double)(row["MassPackageSize"]);
					this.loadByWeight			= (row.IsNull("LoadByWeight")) ? false : Convert.ToBoolean(row["LoadByWeight"]);
					this.VarianceTolerance		= row.IsNull("VarianceTolerance") ? 0.0 : (double)row["VarianceTolerance"];

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

					if (row.IsNull("UserData1"))
					{
						this.aviationProduct = false;
					}
					else
					{
						if (string.Equals("Yes", (string)row["UserData1"], StringComparison.InvariantCultureIgnoreCase))
						{
							this.aviationProduct = true;
						}
						else
						{
							this.aviationProduct = false;
						}
					}
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the product data object to its initial state.
		/// </summary>
		private void Init()
		{
			this.productID							= string.Empty;
			this.productGuid						= Guid.Empty;
			this.productType						= PRODUCT_TYPE.ComponentProduct;
			this.volumeDecimalPlaces				= 0;
			this.volumeUnitIndex					= 1;
			this.volumeConversionFactor				= 1;
			this.useProductVolumeConversionFactor	= false;
			this.volumePackageSize					= 0;
			this.massDecimalPlaces					= 0;
			this.massUnitIndex						= 1;
			this.massConversionFactor				= 1;
			this.useProductMassConversionFactor		= false;
			this.massPackageSize					= 0;
			this.loadByWeight						= false;
		}
		#endregion
	}
}
