// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryDefault.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the QueryDefaultClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	/// <summary>
	/// Class for holding query default values.
	/// </summary>
   [Serializable]
   [DataContract]
	public class QueryDefaultClass : BaseDataObject
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryDefaultClass"/> class.
		/// </summary>
		public QueryDefaultClass()
		{
			this.Reset();
		}

		#endregion

		#region Public Properties

		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.QUERY_DEFAULT;
			}

			set
			{
				;
			}
		}

		/// <summary>
		/// Gets or sets the footer.
		/// </summary>
		/// <value>
		/// The footer text.
		/// </value>
		[DataMember]
		public string Footer { get; set; }

		/// <summary>
		/// Gets or sets the header.
		/// </summary>
		/// <value>
		/// The header text.
		/// </value>
		[DataMember]
		public string Header { get; set; }

		/// <summary>
		/// Gets the type of the parent entity.
		/// </summary>
		/// <value>
		/// The type of the parent entity.
		/// </value>
		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		#endregion

		#region Public Methods and Operators

		public void EnumerateBySiteSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblQueryDefaults WHERE SiteGuid = @SiteGuid";
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
		}

		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblQueryDefaults Q"
			                  + " LEFT OUTER JOIN map.tblEntityQuerySettingToSite E ON E.SiteGuid = @SiteGuid "
			                  + " WHERE Q.SiteGuid = @SiteGuid OR E.MapToSiteGuid IS NOT NULL";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblQueryDefaults (SiteGuid, Header, Footer, CreatedDate,"
							  + "CreatedBy, UpdatedDate, UpdatedBy, QueryDefaultGuid) VALUES ("
			                  + "@SiteGuid, @Header, @Footer, @CreatedDate, @CreatedBy, @UpdatedDate,"
							  + "@UpdatedBy, @QueryDefaultGuid)";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Header", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@Footer", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@QueryDefaultGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
			cmd.Parameters["@Header"].Value = this.Header;
			cmd.Parameters["@Footer"].Value = this.Footer;
			cmd.Parameters["@CreatedDate"].Value = this._CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this._CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
			cmd.Parameters["@QueryDefaultGuid"].Value = this._IdentityGuid;
		}

		/// <summary>
		/// Loads the specified object into this data object.
		/// </summary>
		/// <param name="o">The object containing the information with which to load this object.</param>
		public override void Load(object o)
		{
			if (o is DataSet)
			{
				var set = (DataSet)o;

				this.Reset();

				DataTable table = set.Tables[0];
				if (table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				this._IdentityGuid = DataObject.getValue(row["QueryDefaultGuid"], Guid.Empty);
				this._SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);
				this.Header = DataObject.getValue(row["Header"], "FuelsManager Query Results");
				this.Footer = DataObject.getValue(row["Footer"], "Confidential");
				this._CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
				this._CreatedBy = DataObject.getValue( row["CreatedBy"], ADMIN );
				this._UpdatedDate = DataObject.getValue(row["UpdatedDate"], this._CreatedDate);
				this._UpdatedBy = DataObject.getValue( row["UpdatedBy"], ADMIN );
			}
			else
			{
				base.Load(o);
			}
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblQueryDefaults WHERE QueryDefaultGuid = @QueryDefaultGuid";
			cmd.Parameters.Add("@QueryDefaultGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@QueryDefaultGuid"].Value = this.IdentityGuid;
		}

		public override void Reset()
		{
			base.Reset();

			this.Header = "FuelsManager Query Results";
			this.Footer = "Confidential";
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblQueryDefaults " + SQLUpdateLock(bInTransaction)
			                  + " WHERE QueryDefaultGuid = @QueryDefaultGuid";

			cmd.Parameters.Add("@QueryDefaultGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@QueryDefaultGuid"].Value = this.IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblQueryDefaults SET " + " SiteGuid = @SiteGuid, " + " Header = @Header, "
			                  + " Footer = @Footer, " + " UpdatedDate = @UpdatedDate, " + " UpdatedBy = @UpdatedBy "
			                  + " WHERE QueryDefaultGuid = @QueryDefaultGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Header", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@Footer", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@QueryDefaultGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
			cmd.Parameters["@Header"].Value = this.Header;
			cmd.Parameters["@Footer"].Value = this.Footer;
			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
			cmd.Parameters["@QueryDefaultGuid"].Value = this.IdentityGuid;
		}

		#endregion
	}
}