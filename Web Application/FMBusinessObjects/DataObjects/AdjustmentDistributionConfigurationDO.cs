/// <summary>
/// File name:	AdjustmentDistributionConfigurationDO.cs
/// Purpose:	The purpose of this class is to contain the adjustment distribution
///				configuration data.  It has methods that return SQL to retrieve, insert,
///				and update.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;


namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   public class AdjustmentDistributionConfigurationDO : DataObject
	{
		#region public attributes
		public enum AdjustmentDistributionMethods { ALLOCATION, MANUAL, THROUGHPUT };
		#endregion

		#region Private attributes
		[DataMember]
		private AdjustmentDistributionMethods adjustmentMethod;
		[DataMember]
		private Guid siteGuid;
		[DataMember]
		private int method;
		[DataMember]
		private bool consortiumFlag;
		[DataMember]
		private string createdBy;
		[DataMember]
		private string updatedBy;
		[DataMember]
		private System.DateTimeOffset createdDate;
		[DataMember]
		private System.DateTimeOffset updatedDate;

/* A. Hush 2/28/2012 -- No longer using integer PKs. Waiting to hear if this class is still needed.
		[DataMember]
		private ArrayList assignedAliasIDs;
		[DataMember]
		private int aliasID1;
		[DataMember]
		private int aliasID2;
		[DataMember]
		private int aliasID3;
		[DataMember]
		private int aliasID4;
		[DataMember]
		private int aliasID5;
		[DataMember]
		private int aliasID6;
		[DataMember]
		private int aliasID7;
		[DataMember]
		private int aliasID8;
		[DataMember]
		private int aliasID9;
		[DataMember]
		private int aliasID10;
		[DataMember]
		private int aliasID11;
		[DataMember]
		private int aliasID12;
		[DataMember]
		private int aliasID13;
		[DataMember]
		private int aliasID14;
		[DataMember]
		private int aliasID15;
		[DataMember]
		private int aliasID16;
 */
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the adjustment distribution configuration
		/// data object.
		/// </summary>
		public AdjustmentDistributionConfigurationDO ( )
		{
			this.Init ( );
		}
		#endregion

		#region properties
		/// <summary>
		/// This property sets and gets the adjustment method for distribution. It also sets
		/// the method attribution that is saved to the database.
		/// </summary>
		public AdjustmentDistributionMethods AdjustmentMethod
		{
			get { return this.adjustmentMethod; }
			set
			{
				this.adjustmentMethod = value;

				switch (value)
				{
					case AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.ALLOCATION:
						this.method = 1;
						break;
					case AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.MANUAL:
						this.method = 2;
						break;
					case AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.THROUGHPUT:
						this.method = 3;
						break;
					default:
						this.method = 1;
						break;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the consortium flag attribute.
		/// </summary>
		public bool ConsortiumFlag
		{
			get { return this.consortiumFlag; }
			set { this.consortiumFlag = value; }
		}

		/// <summary>
		/// This property sets and gets the created by attribute.
		/// </summary>
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// This property sets and gets the updated by attribute.
		/// </summary>
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		/// <summary>
		/// This property sets and gets the created date attribute.
		/// </summary>
		public System.DateTimeOffset CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		/// <summary>
		/// This property sets and gets the updated date attribute.
		/// </summary>
		public System.DateTimeOffset UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

/* A. Hush 2/28/2012 -- No longer using integer PKs. Waiting to hear if this class is still needed.
		public int AliasID1
		{
			get { return this.aliasID1; }
			set { this.aliasID1 = value; }
		}
 */
		#endregion

		#region SQL methods
		/// <summary>
		/// This method return a SqlCommand that will retrieve the adjustment distribution 
		/// configuration settings for a given site.
		/// </summary>
		/// <param name="siteGuid"></param>
		/// <returns></returns>
		public SqlCommand RetrieveAdjustmentConfigurationSQL ( Guid siteGuid )
		{
			throw new NotImplementedException();
/* A. Hush 2/28/2012 -- No longer using integer PKs. Waiting to hear if this class is still needed.
			const string PARAM_NAME_SITEGUID = "@SiteGuid";
			const SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;


			string select = "SELECT SiteGuid, Method, ConsortiumFlag, CreatedBy, UpdatedBy, CreatedDate, UpdatedDate, " +
							"AliasID1, AliasID2, AliasID3, AliasID4, AliasID5, AliasID6, AliasID7, AliasID8, AliasID9, " +
							"AliasID10, AliasID11, AliasID12, AliasID13, AliasID14, AliasID15, AliasID16 ";
			string from = "FROM tblAdjustmentDistributionConfig ";
			SqlCommand cmd = new SqlCommand();
			string where = AddParameter(cmd, "WHERE", "SiteGuid", "=", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, siteGuid);
			cmd.CommandText = select + from + where;
			
			return cmd;
 */
		}

		// The following is not used.  When used, need to SQLCommandize and SQLParameterize.
		///// <summary>
		///// This method will return a SQL string that inserts a new configuration into the database.
		///// </summary>
		///// <param name="siteIndex"></param>
		///// <returns></returns>
		//public string InsertAdjustmentConfigurationSQL ( int siteIndex )
		//{
		//   string insert = "INSERT INTO tblAdjustmentDistributionConfig (SiteIndex, Method, ConsortiumFlag, " +
		//               "CreatedBy, AliasID1, AliasID2, AliasID3, AliasID4, AliasID5, AliasID6, AliasID7, " +
		//               "AliasID8, AliasID9, liasID10, AliasID11, AliasID12, AliasID13, AliasID14, AliasID15, " +
		//               "AliasID16, CreatedDate) ";
		//   string values = "VALUES (" + siteIndex + ", " + this.method + ", " + System.Convert.ToInt32 ( this.consortiumFlag ) +
		//               ", '" + this.createdBy + "', " + this.aliasID1 + ", " + this.aliasID2 + ", " +
		//               this.aliasID3 + ", " + this.aliasID4 + ", " + this.aliasID5 + ", " + this.aliasID6 + ", " +
		//               this.aliasID7 + ", " + this.aliasID8 + ", " + this.aliasID9 + ", " + this.aliasID10 + ", " +
		//               this.aliasID11 + ", " + this.aliasID12 + ", " + this.aliasID13 + ", " + this.aliasID14 + ", " +
		//      //Eric Simmons - (11/23/2007)
		//      //Updated Call of ToString() to ToString("s") to resolve CSI#5381
		//               this.aliasID15 + ", " + this.aliasID16 + ", '" + this.createdDate.ToString ( "s" ) + "') ";

		//   return ( insert + values );
		//}

		// The following is not used.  When used, need to SQLCommandize and SQLParameterize.
		///// <summary>
		///// This method will return a SQL string that updates an existing configuration into the database.
		///// </summary>
		///// <param name="siteIndex"></param>
		///// <returns></returns>
		//public string UpdateAdjustmentConfigurationSQL ( int siteIndex )
		//{
		//   string update = "UPDATE tblAdjustmentDistributionConfig ";
		//   string setValues = "SET Method = " + this.method + ", " +
		//                  "ConsortiumFlag = " + System.Convert.ToInt32 ( this.consortiumFlag ) + ", " +
		//                  "UpdatedBy = '" + this.updatedBy + "', " +
		//      //Eric Simmons - (11/23/2007)
		//      //Updated Call of ToString() to ToString("s") to resolve CSI#5381
		//                  "UpdatedDate = '" + this.updatedDate.ToString ( "s" ) + "', " +
		//                  "AliasID1 = " + this.aliasID1 + ", " +
		//                  "AliasID2 = " + this.aliasID2 + ", " +
		//                  "AliasID3 = " + this.aliasID3 + ", " +
		//                  "AliasID4 = " + this.aliasID4 + ", " +
		//                  "AliasID5 = " + this.aliasID5 + ", " +
		//                  "AliasID6 = " + this.aliasID6 + ", " +
		//                  "AliasID7 = " + this.aliasID7 + ", " +
		//                  "AliasID8 = " + this.aliasID8 + ", " +
		//                  "AliasID9 = " + this.aliasID9 + ", " +
		//                  "AliasID10 = " + this.aliasID10 + ", " +
		//                  "AliasID11 = " + this.aliasID11 + ", " +
		//                  "AliasID12 = " + this.aliasID12 + ", " +
		//                  "AliasID13 = " + this.aliasID13 + ", " +
		//                  "AliasID14 = " + this.aliasID14 + ", " +
		//                  "AliasID15 = " + this.aliasID15 + ", " +
		//                  "AliasID16 = " + this.aliasID16 + " ";
		//   string where = "WHERE SiteIndex = " + siteIndex;

		//   return ( update + setValues + where );
		//}
		#endregion

		#region SQL Load Methods
		/// <summary>
		/// This method will load the adjustment distribution configuration settings from the
		/// database.
		/// </summary>
		/// <param name="dataSet"></param>
		public void LoadAdjustmentConfigurationSQL ( System.Data.DataSet dataSet )
		{
			throw new NotImplementedException();
/* A. Hush 2/28/2012 -- No longer using integer PKs. Waiting to hear if this class is still needed.
			if (dataSet != null)
			{
				this.assignedAliasIDs = new ArrayList ( );
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					this.siteGuid = DataObject.getValue<Guid>(row[0], Guid.Empty);
					this.method = DataObject.getInt ( row[1] );
					this.consortiumFlag = DataObject.getBool ( row[2] );
					this.createdBy = DataObject.getString ( row[3] );
					this.updatedBy = DataObject.getString ( row[4] );
					this.aliasID1 = DataObject.getInt ( row[7] );
					this.aliasID2 = DataObject.getInt ( row[8] );
					this.aliasID3 = DataObject.getInt ( row[9] );
					this.aliasID4 = DataObject.getInt ( row[10] );
					this.aliasID5 = DataObject.getInt ( row[11] );
					this.aliasID6 = DataObject.getInt ( row[12] );
					this.aliasID7 = DataObject.getInt ( row[13] );
					this.aliasID8 = DataObject.getInt ( row[14] );
					this.aliasID9 = DataObject.getInt ( row[15] );
					this.aliasID10 = DataObject.getInt ( row[16] );
					this.aliasID11 = DataObject.getInt ( row[17] );
					this.aliasID12 = DataObject.getInt ( row[18] );
					this.aliasID13 = DataObject.getInt ( row[19] );
					this.aliasID14 = DataObject.getInt ( row[20] );
					this.aliasID15 = DataObject.getInt ( row[21] );
					this.aliasID16 = DataObject.getInt ( row[22] );

					if (this.aliasID1 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[7] ) );

					if (this.aliasID2 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[8] ) );

					if (this.aliasID3 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[9] ) );

					if (this.aliasID4 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[10] ) );

					if (this.aliasID5 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[11] ) );

					if (this.aliasID6 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[12] ) );

					if (this.aliasID7 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[13] ) );

					if (this.aliasID8 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[14] ) );

					if (this.aliasID9 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[15] ) );

					if (this.aliasID10 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[16] ) );

					if (this.aliasID11 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[17] ) );

					if (this.aliasID12 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[18] ) );

					if (this.aliasID13 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[19] ) );

					if (this.aliasID14 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[20] ) );

					if (this.aliasID15 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[21] ) );

					if (this.aliasID16 != -99)
						this.assignedAliasIDs.Add ( DataObject.getInt ( row[22] ) );

					switch (this.method)
					{
						case 1:
							this.adjustmentMethod = AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.ALLOCATION;
							break;

						case 2:
							this.adjustmentMethod = AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.MANUAL;
							break;

						case 3:
							this.adjustmentMethod = AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.THROUGHPUT;
							break;

						default:
							this.adjustmentMethod = AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.ALLOCATION;
							break;
					}

					// Determine if the created and updated dates are not null. If not, then
					// set the attributes.
					if (DataObject.isNull ( row[5] ) == false)
						this.createdDate = DataObject.getDateTime ( row[5] );

					if (DataObject.isNull ( row[6] ) == false)
						this.updatedDate = DataObject.getDateTime ( row[6] );
				}
			}
 */
		}
		#endregion

		#region private methods
		/// <summary>
		/// This method initializes the object to its initial state.  It is called by the constructor.
		/// </summary>
		private void Init ( )
		{
			this.adjustmentMethod = AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.MANUAL;
			this.siteGuid = Guid.Empty;
			this.consortiumFlag = false;
/* A. Hush 2/28/2012 -- No longer using integer PKs. Waiting to hear if this class is still needed.
			this.assignedAliasIDs = null;
			this.aliasID1 = -99;
			this.aliasID2 = -99;
			this.aliasID3 = -99;
			this.aliasID4 = -99;
			this.aliasID5 = -99;
			this.aliasID6 = -99;
			this.aliasID7 = -99;
			this.aliasID8 = -99;
			this.aliasID9 = -99;
			this.aliasID10 = -99;
			this.aliasID11 = -99;
			this.aliasID12 = -99;
			this.aliasID13 = -99;
			this.aliasID14 = -99;
			this.aliasID15 = -99;
			this.aliasID16 = -99;
*/
		}
		#endregion

		#region Override Methods
		override public string getUpdateCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getSelectCommand ( )
		{
			return null;
		}
		#endregion
	}
}

