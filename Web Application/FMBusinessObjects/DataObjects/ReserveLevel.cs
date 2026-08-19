/// FILE NAME:	ReserveLevel.cs
/// PURPOSE:	ReserveLevelClass
/// 
/// COMMENTS:	The Reserve Level Class handles the business logic to add, modify,
///				and delete the reserve level configuration.
/// 
/// Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
/// This file shall not be copied or reproduced in any form without
/// the express written consent of Endress+HaGate.
/// 
/// AUTHOR(S):	Richard Panachida
/// VERSION:	1.0.0  Current version
/// 
/// MODIFICATION HISTORY:
/// Date:		By:				Reason:
/// ----------	----------		-------------------------------------------
/// 20-Mar-09	B. Schaal		Corrected property implementation
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Xml.Serialization;
using System.Collections;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
	#region ReserveLevel Collection class
   [Serializable]
   [CollectionDataContract]
	public class ReserveLevelCollectionClass : List<ReserveLevelClass>
	{
		/// <summary>
		/// This method will remove the item from the list
		/// that matches the one given.
		/// </summary>
		/// <param name="reserveLevel"></param>
		public void RemoveByIndex(ReserveLevelClass reserveLevel)
		{
			int index = 0;

			foreach (ReserveLevelClass item in this)
			{
				if (item.IdentityGuid == reserveLevel.IdentityGuid)
				{
					this.RemoveAt(index);
					return;
				}

				index++;
			}
		}
	}
	#endregion

	#region ReserveLevel Class
   [Serializable]
   [DataContract]
	public class ReserveLevelClass : BaseDataObject
	{
		#region Private data members
		[DataMember] private Guid productGuid;
		[DataMember] private string productID;
		[DataMember] private double minimumLevel;
		[DataMember] private double warningLevel;

		[DataMember] private bool auditLog = false;
		private const string TABLE_NAME = "tblReserveLevels";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the reserve level class.
		/// </summary>
		public ReserveLevelClass ( )
		{
			this.Reset ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property gets and sets the Audit Log flag. Returns true
		/// if the audit log is to be used.
		/// </summary>
		public bool AuditLog
		{
			get { return this.auditLog; }
			set { this.auditLog = value; }
		}

		/// <summary>
		/// This property gets and sets the product guid data member.
		/// </summary>
		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		public string ProductID
		{
			get { return this.productID; }
			set { this.productID = value; }
		}

		/// <summary>
		/// This property gets and sets the minimum level data member.
		/// </summary>
		public double MinimumLevel
		{
			get { return this.minimumLevel; }
			set { this.minimumLevel = value; }
		}

		/// <summary>
		/// This property gets and sets the warning level data member.
		/// </summary>
		public double WarningLevel
		{
			get { return this.warningLevel; }
			set { this.warningLevel = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method loads the object with the information from the 
		/// database.
		/// </summary>
		/// <param name="Set"></param>
		public void Load ( DataSet dataSet )
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException ( "Set" );
			}

			this.Reset ( );

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			base.IdentityGuid = DataObject.getValue<Guid>(row["ReserveLevelGuid"], Guid.Empty);
			base.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this.productGuid = DataObject.getValue<Guid>(row["ProductGuid"], Guid.Empty);
			this.productID = DataObject.getValue<string>(row["ProductID"], "");
			this.MinimumLevel = DataObject.getValue<double>(row["MinimumLevel"], 0.0);
			this.WarningLevel = DataObject.getValue<double>(row["WarningLevel"], 0.0);

		}

		#endregion

		#region SQL with Parameters

		/// <summary>
		/// This method will return an insert SqlCommand for inserting one row.
		/// </summary>
		/// <returns></returns>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = " INSERT INTO " + ReserveLevelClass.TABLE_NAME +
					" (ProductGuid, SiteGuid, MinimumLevel, WarningLevel, ReserveLevelGuid) " +
					" VALUES (" +
					" @ProductGuid, " +
					" @SiteGuid, " +
					" @MinimumLevel, " +
					" @WarningLevel, " +
					" @ReserveLevelGuid) ";
					
				cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters.Add("@MinimumLevel", SqlDbType.Float);
				cmd.Parameters.Add("@WarningLevel", SqlDbType.Float);
				cmd.Parameters.Add("@ReserveLevelGuid", SqlDbType.UniqueIdentifier);

				cmd.Parameters["@ProductGuid"].Value = this.productGuid;
				cmd.Parameters["@SiteGuid"].Value = base.SiteGuid;
				cmd.Parameters["@MinimumLevel"].Value = this.minimumLevel;
				cmd.Parameters["@WarningLevel"].Value = this.warningLevel;
				cmd.Parameters["@ReserveLevelGuid"].Value = this._IdentityGuid;
		}

		/// <summary>
		/// This method will return an update SqlCommand for updating one row.
		/// </summary>
		/// <returns></returns>
		public void UpdateSQL(SqlCommand cmd)
		{
			string update = "UPDATE " + ReserveLevelClass.TABLE_NAME;
			string setValue = " SET ProductGuid = @ProductGuid, " +
							  " SiteGuid = @SiteGuid, " + 
							  " MinimumLevel = @MinimumLevel, " +
							  " WarningLevel = @WarningLevel ";
			string where = " WHERE ReserveLevelGuid = @ReserveLevelGuid";

			cmd.CommandText = update + setValue + where;

			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@MinimumLevel", SqlDbType.Float);
			cmd.Parameters.Add("@WarningLevel", SqlDbType.Float);
			cmd.Parameters.Add("@ReserveLevelGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ProductGuid"].Value = this.productGuid;
			cmd.Parameters["@SiteGuid"].Value = base.SiteGuid;
			cmd.Parameters["@MinimumLevel"].Value = this.minimumLevel;
			cmd.Parameters["@WarningLevel"].Value = this.warningLevel;
			cmd.Parameters["@ReserveLevelGuid"].Value = base.IdentityGuid;
		}

		/// <summary>
		/// This method will return a purge SQL cmd for deleting a single
		/// reserve level item.
		/// </summary>
		/// <returns></returns>
		public void PurgeSQL(SqlCommand cmd)
		{
			string delete = "DELETE FROM " + ReserveLevelClass.TABLE_NAME + " ";
			string where = "WHERE ReserveLevelGuid = @ReserveLevelGuid";
			cmd.CommandText =  delete + where;

			cmd.Parameters.Add("@ReserveLevelGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ReserveLevelGuid"].Value = base.IdentityGuid;			
		}

		/// <summary>
		/// This method will return a SQL Command that returns one reserve level
		/// item.
		/// </summary>
		/// <returns></returns>
		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			string select = "SELECT TOP 1 rs.*, tblProducts.ProductID ";
			string from = " FROM " + ReserveLevelClass.TABLE_NAME + " rs INNER JOIN tblProducts " + SQLUpdateLock(bInTransaction) +
							" ON rs.ProductGuid = tblProducts.ProductGuid ";
			string where = " WHERE ReserveLevelGuid = @ReserveLevelGuid"; 

			cmd.CommandText = select + from + where;

			cmd.Parameters.Add("@ReserveLevelGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@ReserveLevelGuid"].Value = base.IdentityGuid;		
		}

		/// <summary>
		/// This property returns the a Sql Command object with SELECT by ID query.
		/// </summary>
		public void SelectByProductIDSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblReserveLevels.*, ProductID FROM tblReserveLevels INNER JOIN tblProducts " + SQLUpdateLock(bInTransaction) +
				" ON tblReserveLevels.ProductGuid = tblProducts.ProductGuid " +
				" WHERE ProductID = @ProductID " +
				" AND tblReserveLevels.SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@ProductID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ProductID"].Value = this.ProductID;		
			cmd.Parameters["@SiteGuid"].Value = base.SiteGuid;		
		}

		/// <summary>
		/// This method will return a SqlCommand object with the enumeration SQL to retrieve all the reserve level
		/// records based on the security context.
		/// </summary>
		/// <param name="Security"></param>
		/// <returns></returns>
		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblReserveLevels.*,  tblProducts.productID FROM " +
				"tblReserveLevels INNER JOIN tblProducts ON tblReserveLevels.ProductGuid = tblProducts.ProductGuid " +
				" WHERE tblReserveLevels.SiteGuid = @SiteGuid";
			
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;		
		}

		#endregion

		#region Override Methods
		public override void Reset ( )
		{
			base.Reset ( );
			this.ProductID = "";
			this.ProductGuid = Guid.Empty;
			this.MinimumLevel = 0.0;
			this.WarningLevel = 0.0;
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.RESERVE_LEVEL; }
			set { ; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}
		#endregion
	}
	#endregion
}
