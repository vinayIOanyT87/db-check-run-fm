///***************************************************************************
/// Module Name:  MeterAssetClass.cs
/// Author:       Ryan Hill
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	/// <summary>
	/// This class represents something a meter is assigned to, which can be a piece of equipment, a tank, or a load arm 
	/// </summary>
	[Serializable]
	[DataContract]
	public class MeterAssetClass : BaseDataObject
	{
		/// <summary>
		/// The types of assets to which meters can belong
		/// </summary>
		public enum METER_ASSET_TYPE
		{
			Undefined = 0,
			Equipment = 1,
			Tank = 2,
			LoadArm = 3
		};

		/// <summary>
		/// The type of asset (tank, load arm, or equipment) 
		/// </summary>
		[DataMember]
		public METER_ASSET_TYPE AssetType { get; set; }

		/// <summary>
		/// Get a user-friendly string representation of the asset
		/// </summary>
		public string AssetTypeID
		{
			get
			{
				if (this.AssetType != METER_ASSET_TYPE.Undefined)
				{
					return EntityToSiteMapClass.GetEntityTypeID(this.EntityType);
				}
				else
				{
					return string.Empty;
				}
			}
		}

		/// <summary>
		/// Return the entity type. The entity type corresponds to the asset type.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				if (this.AssetType == METER_ASSET_TYPE.Equipment)
				{
					return ENTITY_TYPE.EQUIPMENT;
				}
				else if (this.AssetType == METER_ASSET_TYPE.Tank)
				{
					return ENTITY_TYPE.TANK;
				}
				else if (this.AssetType == METER_ASSET_TYPE.LoadArm)
				{
					return ENTITY_TYPE.LOAD_ARM;
				}

				return ENTITY_TYPE.UNKNOWN;
			}
		}

		/// <summary>
		/// Return the parent entity type of a meter asset.
		/// Meter assets don't have a parent, so we return NONE.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		/// <summary>
		/// Blanks out the data in the meter asset object
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			this.AssetType = METER_ASSET_TYPE.Undefined;
		}

		/// <summary>
		/// Read a meter asset object from a DataSet
		/// </summary>
		/// <param name="set">A DataSet to read meter asset information from</param>
		/// <returns>true if loading the asset information was successful</returns>
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

			this._IdentityGuid = DataObject.getValue<Guid>(row["AssetGuid"], Guid.Empty);
			this._ID = DataObject.getValue<string>(row["AssetID"], string.Empty);
			this.AssetType = DataObject.getValue<METER_ASSET_TYPE>(row["AssetType"], METER_ASSET_TYPE.Undefined);
			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);

			return true;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to read all the meter assets for the specified site
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate</param>
		public void Enumerate(SqlCommand cmd)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterAssetSelect";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
		}

		/// <summary>
		/// Set up a SqlCommand object with the information necessary to list all assets in the system which have
		/// and ID partially matching the provided parameter
		/// </summary>
		/// <param name="cmd">A SqlCommand object to populate</param>
		/// <param name="assetIDFilterValue">A value to filter the returned assets on. Filtering is done on the ID</param>
		public void EnumerateAndFilter(SqlCommand cmd, string assetIDFilterValue)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "usp_MeterAssetSelect";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;

			cmd.Parameters.Add("@AssetIDFilterValue", SqlDbType.NVarChar, 30);
			cmd.Parameters["@AssetIDFilterValue"].Value = assetIDFilterValue;
		}
	}
}
