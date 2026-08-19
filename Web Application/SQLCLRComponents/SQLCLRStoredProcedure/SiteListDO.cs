/// <summary>
///   File name:	SiteListDO.cs
///   Purpose:	   The purpose of this class is to contain a list of Site Data object classes.
///               It contains the SQL to retrieve the current site and its children.
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

#region SiteListDO Class
public class SiteListDO
{
	#region Private data members
	private Hashtable siteListHsh;
	private string retrieveSitesSQL;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default constructor for the Site List class.
	/// </summary>
	public SiteListDO()
	{
		this.Init();
	}
	#endregion

	#region Properties
	/// <summary>
	/// This property will return the list of sites.
	/// </summary>
	public Hashtable SiteList
	{
		get { return this.siteListHsh; }
	}
	#endregion

	#region Public methods
	/// <summary>
	/// This method will retrieve the list of sites if the given site index is
	/// a site group. It will exluded itself from the list.
	/// </summary>
	/// <param name="siteIndex"></param>
	public void RetrieveSiteList(SqlConnection a_connection, int siteIndex)
	{
		DataSet dataSet = new DataSet();

		SqlCommand command = new SqlCommand(this.retrieveSitesSQL, a_connection);

		command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);
		command.Parameters["@SiteIndex"].Value = siteIndex;

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		// Load the results.
		this.LoadSiteList(dataSet);
	}

	/// <summary>
	/// This method will add a Site data object to the list if it does
	/// not exist.
	/// </summary>
	/// <param name="siteDO"></param>
	public void AddSiteToList(SiteDO siteDO)
	{
		if ((siteDO != null) && (string.IsNullOrEmpty(siteDO.SiteName) == false))
		{
			if (this.siteListHsh.Contains(siteDO.SiteName) == false)
			{
				this.siteListHsh.Add(siteDO.SiteName, siteDO);
			}
		}
	}
	#endregion

	#region Private methods
	/// <summary>
	/// This method will load the list of sites if the given site index is
	/// a site group. It will excluded the current site group.
	/// </summary>
	/// <param name="dataSet"></param>
	private void LoadSiteList(DataSet dataSet)
	{
		this.siteListHsh.Clear();

		if ((dataSet != null) && (dataSet.Tables.Count > 0))
		{
			DataTable table = dataSet.Tables[0];
			DataRow row = null;

			if (table.Rows.Count > 0)
			{
				for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
				{
					row = table.Rows[rowIndex];

					string site = (row.IsNull("ID")) ? "" : (string)row["ID"];
					string siteIndexStr = (row.IsNull("SiteIndex")) ? "0" : row["SiteIndex"].ToString();
					int siteIndex = Convert.ToInt32(siteIndexStr);

					if (this.siteListHsh.Contains(site) == false)
					{
						SiteDO siteDO = new SiteDO();
						siteDO.SiteName = site;
						siteDO.SiteIndex = siteIndex;
						this.siteListHsh.Add(site, siteDO);
					}
				}
			}
		}
	}

	/// <summary>
	/// This method will initialize the Site List object to its intial state.
	/// </summary>
	private void Init()
	{
		this.siteListHsh = new Hashtable();
		this.retrieveSitesSQL = "EXEC usp_SiteList @SiteGuid";
	}
	#endregion
}
#endregion


#region SiteDO class
public class SiteDO
{
	#region Private data members
	private string siteName;
	private int siteIndex;
	private bool siteGroupFlag;
	private int volumeDecimalPlaces;
	private int volumeUnitIndex;
	private double volumeConversionFactor;
	private int additiveVolumeDecimalPlaces;
	private int additiveVolumeUnitIndex;
	private double additiveVolumeConversionFactor;
	private int massDecimalPlaces;
	private int massUnitIndex;
	private double massConversionFactor;
	private bool singleOwner;
	private bool inhibitSiteLedgerRollup;

	public const int MaxRoundValue = 2147483647;
	public const double ConvertValue = 1;
	#endregion

	#region Constructors
	/// <summary>
	/// This is the default constructor for the Site Data Object class
	/// </summary>
	public SiteDO()
	{
		this.siteName = "";
		this.siteIndex = 0;
		this.siteGroupFlag = false;
		this.volumeDecimalPlaces = 0;
		this.volumeUnitIndex = 1;
		this.volumeConversionFactor = 1;
		this.additiveVolumeDecimalPlaces = 0;
		this.additiveVolumeUnitIndex = 1;
		this.additiveVolumeConversionFactor = 1;
		this.massDecimalPlaces = 0;
		this.massUnitIndex = 1;
		this.massConversionFactor = 1;
		this.singleOwner = true;
		this.inhibitSiteLedgerRollup = false;
	}
	#endregion

	#region Properties
	/// <summary>
	/// This property sets and gets the Site Name data member.
	/// </summary>
	public string SiteName
	{
		get { return this.siteName; }
		set
		{
			this.siteName = value;
			if (this.siteName == null)
			{
				this.siteName = "";
			}
		}
	}

	/// <summary>
	/// This property sets and gets the Site Index data member.
	/// </summary>
	public int SiteIndex
	{
		get { return this.siteIndex; }
		set { this.siteIndex = value; }
	}

	/// <summary>
	/// This property gets the Site Group Flag data member.
	/// True means that the site is a site group.
	/// </summary>
	public bool SiteGroupFlag
	{
		get { return this.siteGroupFlag; }
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
	/// This property will get the Volume Conversion Factor from SI units.
	/// </summary>
	public double VolumeConversionFactor
	{
		get { return this.volumeConversionFactor; }
	}

	/// <summary>
	/// This property gets the Site AdditiveVolume Unit Index. The default is
	/// one (SI units).
	/// </summary>
	public int AdditiveVolumeUnitIndex
	{
		get { return this.additiveVolumeUnitIndex; }
	}

	/// <summary>
	/// This property gets the Site Additive Volume Decimal Places. The default
	/// is zero.
	/// </summary>
	public int AdditiveVolumeDecimalPlaces
	{
		get { return this.additiveVolumeDecimalPlaces; }
	}

	/// <summary>
	/// This property will get the Additive Volume Conversion Factor from SI units.
	/// </summary>
	public double AdditiveVolumeConversionFactor
	{
		get { return this.additiveVolumeConversionFactor; }
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
	/// This property gets the Site Additive Volume Decimal Places. The default
	/// is zero.
	/// </summary>
	public int MassDecimalPlaces
	{
		get { return this.massDecimalPlaces; }
	}

	/// <summary>
	/// This property will get the Additive Volume Conversion Factor from SI units.
	/// </summary>
	public double MassConversionFactor
	{
		get { return this.massConversionFactor; }
	}


	/// <summary>
	/// This property will return true if the site is a single owner site.
	/// </summary>
	public bool SingleOwner
	{
		get { return this.singleOwner; }
	}

	/// <summary>
	/// This property will return true if the site is prohibited from 
	/// being rolled up in the ledger.  Otherwise, it return false (default).
	/// </summary>
	public bool InhibitSiteLedgerRollup
	{
		get { return this.inhibitSiteLedgerRollup; }
		set { this.inhibitSiteLedgerRollup = value; }
	}
	#endregion

	#region Public Methods
	/// <summary>
	/// This method will retrieve site information such as the conversion factor and volume
	/// decimal places based on the site index.
	/// </summary>
	/// <param name="siteIndex"></param>
	public void RetrieveSiteInfo(SqlConnection a_connection, int siteIndex)
	{
		DataSet dataSet = new DataSet();
		string sql = "SELECT [ID] AS SiteName, SiteIndex, SiteGroupFlag, " +
							 " VolumeDecimalPlaces, VolumeUnitIndex, " +
									 " dbo.udf_ConvertFromSIUnits(@ConvertValue, VolumeUnitIndex, @RoundFactor) AS VolumeFactor, " +
							 " AdditiveVolumeDecimalPlaces, AdditiveVolumeUnitIndex, " +
									 " dbo.udf_ConvertFromSIUnits(@ConvertValue, AdditiveVolumeUnitIndex, @RoundFactor) AS AdditiveVolumeFactor, " +
							 " EnforceSingleOwner, InhibitSiteLedgerRollup " +
							 " FROM tblSites WHERE SiteIndex = @SiteIndex";

		SqlCommand command = new SqlCommand(sql, a_connection);

		command.Parameters.Add("@ConvertValue", System.Data.SqlDbType.Float);
		command.Parameters.Add("@RoundFactor", System.Data.SqlDbType.Int);
		command.Parameters.Add("@SiteIndex", System.Data.SqlDbType.Int);

		command.Parameters["@ConvertValue"].Value = SiteDO.ConvertValue;
		command.Parameters["@RoundFactor"].Value = SiteDO.MaxRoundValue;
		command.Parameters["@SiteIndex"].Value = siteIndex;

		command.Prepare();

		SqlDataAdapter adapter = new SqlDataAdapter(command);
		adapter.Fill(dataSet);

		// Load the retrieved site information.
		this.LoadSiteInfo(dataSet);
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// This method will load the retrieved site information such as the conversion factor and volume
	/// decimal.
	/// </summary>
	/// <param name="dataSet"></param>
	public void LoadSiteInfo(DataSet dataSet)
	{
		if ((dataSet != null) && (dataSet.Tables.Count > 0))
		{
			DataTable table = dataSet.Tables[0];
			DataRow row = null;

			if (table.Rows.Count > 0)
			{
				row = table.Rows[0];

				this.siteName = (row.IsNull("SiteName")) ? "" : (string)row["SiteName"];
				string siteIndexStr = (row.IsNull("SiteIndex")) ? "0" : row["SiteIndex"].ToString();
				this.siteIndex = Convert.ToInt32(siteIndexStr);
				this.siteGroupFlag = (row.IsNull("SiteGroupFlag")) ? false : (bool)row["SiteGroupFlag"];
				this.volumeDecimalPlaces = (row.IsNull("VolumeDecimalPlaces")) ? 0 : Convert.ToInt32(row["VolumeDecimalPlaces"]);
				this.volumeUnitIndex = (row.IsNull("VolumeUnitIndex")) ? 1 : Convert.ToInt32(row["VolumeUnitIndex"]);
				this.volumeConversionFactor = (row.IsNull("VolumeFactor")) ? 1.0 : (double)row["VolumeFactor"];
				this.additiveVolumeDecimalPlaces = (row.IsNull("AdditiveVolumeDecimalPlaces")) ? 0 : Convert.ToInt32(row["AdditiveVolumeDecimalPlaces"]);
				this.additiveVolumeUnitIndex = (row.IsNull("AdditiveVolumeUnitIndex")) ? 1 : Convert.ToInt32(row["AdditiveVolumeUnitIndex"]);
				this.additiveVolumeConversionFactor = (row.IsNull("AdditiveVolumeFactor")) ? 1.0 : (double)row["AdditiveVolumeFactor"];
				this.singleOwner = (row.IsNull("EnforceSingleOwner")) ? false : (bool)row["EnforceSingleOwner"];
				this.inhibitSiteLedgerRollup = (row.IsNull("InhibitSiteLedgerRollup")) ? false : (bool)row["InhibitSiteLedgerRollup"];
			}
		}
	}
	#endregion
}
#endregion
