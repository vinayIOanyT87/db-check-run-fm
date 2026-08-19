// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityClass.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the SecurityClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.IdentityModel.Claims;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Security.Cryptography;
	using System.Text;
	using System.Xml.Serialization;

	/// <summary>
	/// The security class.
	/// </summary>
	[DataContract]
	[Serializable]
	public class SecurityClass : BaseDataObject
	{
		/// <summary>
		/// This dictionary defines the rights that imply other rights.  For example: having Modify Company Data
		/// implies that a user has View Company Data rights.  
		/// Another way to read this is "key right is implied by value right".
		/// </summary>
		public static readonly Dictionary<RIGHT, RIGHT> AssociatedRights = new Dictionary<RIGHT, RIGHT>
			{
				{ RIGHT.VIEW_ALLOCATIONS, RIGHT.MODIFY_ALLOCATIONS },
				{ RIGHT.VIEW_COMPANY_DATA, RIGHT.MODIFY_COMPANY_DATA },
				{ RIGHT.VIEW_EQUIPMENT_DATA, RIGHT.MODIFY_EQUIPMENT_DATA },
				{ RIGHT.VIEW_FUEL_CARD_DATA, RIGHT.MODIFY_FUEL_CARD_DATA },
				{ RIGHT.VIEW_LOAD_RACK_DATA, RIGHT.MODIFY_LOAD_RACK_DATA },
				{ RIGHT.VIEW_PERSONNEL_DATA, RIGHT.MODIFY_PERSONNEL_DATA },
				{ RIGHT.VIEW_PIDX_PROFILES, RIGHT.MODIFY_PIDX_PROFILES },
				{ RIGHT.VIEW_PRODUCTS, RIGHT.MODIFY_PRODUCTS },
				{ RIGHT.VIEW_QUERIES, RIGHT.MODIFY_QUERIES },
				{ RIGHT.VIEW_REPORTS, RIGHT.MODIFY_REPORTS },
				{ RIGHT.VIEW_SITES_AND_SITE_GROUPS, RIGHT.MODIFY_SITES_AND_SITE_GROUPS },
				{ RIGHT.VIEW_STANDING_OFFERS, RIGHT.MODIFY_STANDING_OFFERS },
				{ RIGHT.VIEW_TICKETING_DATA, RIGHT.MODIFY_TICKETING_DATA },
				{ RIGHT.VIEW_TRANSACTION_ALIASES, RIGHT.MODIFY_TRANSACTION_ALIASES },
				{ RIGHT.VIEW_TRANSACTION_DATA, RIGHT.MODIFY_TRANSACTION_DATA },
				{ RIGHT.VIEW_USER_GROUPS, RIGHT.MODIFY_USER_GROUPS },
				{ RIGHT.VIEW_USERS, RIGHT.MODIFY_USERS },
				{ RIGHT.VIEW_CLOSEOUT_DATA, RIGHT.PERFORM_CLOSEOUT },
				{ RIGHT.VIEW_QUALITY_TESTS, RIGHT.MODIFY_QUALITY_TESTS },
				{ RIGHT.VIEW_TEST_ITEMS, RIGHT.MODIFY_TEST_ITEMS},
				{ RIGHT.VIEW_QUALITYTAG_LOGS, RIGHT.MODIFY_QUALITYTAG_LOGS },
				{ RIGHT.VIEW_QUALITYTAG_RECORD, RIGHT.ADD_QUALITYTAG_RECORD },
				{ RIGHT.VIEW_DISPATCH, RIGHT.MODIFY_DISPATCH },
				{ RIGHT.VIEW_INCOMING_TRUCK_DATA, RIGHT.MODIFY_INCOMING_TRUCK_DATA },
				{ RIGHT.VIEW_TANK_DATA, RIGHT.MODIFY_TANK_DATA },
				{ RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION, RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION },
				{ RIGHT.VIEW_ENTITY_ASSIGNMENTS, RIGHT.MODIFY_ENTITY_ASSIGNMENTS },
				{ RIGHT.VIEW_FIELD_LEVEL_CONTROL_CONFIGURATION, RIGHT.MODIFY_FIELD_LEVEL_CONTROL_CONFIGURATION },
				{ RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS, RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS },
				{ RIGHT.VIEW_SYNC_CONFIG_SERVER_SETTINGS, RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS },
				{ RIGHT.VIEW_SYNC_CONFIG_SITE_SETTINGS, RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS },
				{ RIGHT.VIEW_SYNC_CONFLICT_STATUS, RIGHT.MODIFY_SYNC_CONFLICT_STATUS },
				{ RIGHT.VIEW_ORDERS, RIGHT.MODIFY_ORDERS},
				{ RIGHT.VIEW_SUPPLY_ORDERS, RIGHT.MODIFY_SUPPLY_ORDERS},
				{ RIGHT.IRS_EXSTARS_MANAGER, RIGHT.CREATE_IRS_EXSTARS_REPORT },
				{ RIGHT.CREATE_IRS_EXSTARS_REPORT, RIGHT.VIEW_IRS_EXSTARS_REPORT },
				{ RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION, RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION},
            { RIGHT.VIEW_ONLY_SITE_CLOSEOUT_TIME, RIGHT.MODIFY_SITE_CLOSEOUT_TIME },
				{ RIGHT.OPERATE_VIEW_POINT_GROUPS, RIGHT.OPERATE_MODIFY_POINT_GROUPS },
         };

		/// <summary>
		/// The undefined right text.
		/// </summary>
		public const string UndefinedRightText = "Undefined Right";
		public const int CSRFTokenLength = 16;

		[DataMember]
		public int UserIndex { get; set; }

		[DataMember]
		public Guid UserGuid { get; set; }

		[DataMember]
		public Guid LoginSiteGuid { get; set; }

		[DataMember]
		public string UserID { get; set; }

		[DataMember]
		public Guid Token { get; set; }

		[DataMember]
		public string Password { get; set; }

		[DataMember]
		public string LoginSiteID { get; set; }

		[DataMember]
		public bool ClientCertLogOn { get; set; }

		[DataMember]
		private bool[] RightsArray { get; set; }

		[DataMember]
		public string ASPSessionID { get; set; }

		[DataMember]
		public string ClientDomain { get; set; }

		[DataMember]
		public string ClientUserName { get; set; }

		[DataMember]
		public string ClientIpAddress { get; set; }

		[DataMember]
		public string WebServerIpAddress { get; set; }


		[DataMember]
		public string Workstation { get; set; }

		[DataMember]
		public Dictionary<string, TransactionTypes> ModifyTransactionSecurityRights { get; set; }

		[DataMember]
		public Dictionary<string, TransactionTypes> ViewTransactionSecurityRights { get; set; }

		[DataMember]
		public bool EnableChangeTracking { get; set; }

		[DataMember]
		public bool EnableChangeLogging { get; set; }

		[DataMember]
		public bool UseDataDictionary { get; set; }

		[DataMember]
		protected string csrfToken;

		[DataMember]
		public bool ActiveDirectoryUser { get; set; }

		[DataMember]
		public bool ForcePasswordUpdate { get; set; }

		[DataMember]
		public bool SkipSessionTimeUpdate { get; set; }

		/// <summary>
		/// Gets the size of the rights array.
		/// </summary>
		/// <value>
		/// The size of the rights array.
		/// </value>
		public int RightsArraySize
		{
			get
			{
				return (int)Enum.GetValues(typeof(RIGHT)).Cast<RIGHT>().Max() + 1;
			}
		}

		/// <summary>
		/// Sets the rights array based on the passed collection.
		/// </summary>
		/// <value>
		/// The rights collection to use for setting the array.
		/// </value>
		public RightCollectionClass RightCollection
		{
			set
			{
				// Clear the array
				this.RightsArray = new bool[this.RightsArraySize];

				foreach (RIGHT right in value)
				{
					this.AddRight(right);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityClass"/> class.
		/// </summary>
		public SecurityClass()
		{
			this.RightsArray = new bool[this.RightsArraySize];

			this.EnableChangeTracking = true;
			this.UseDataDictionary = true;
		}


		/// <summary>
		/// Generates a random looking CSRFToken from original CSRF token.
		/// If input csrfToken string is empty or null, a new random CSRF token is generated as an original
		/// and then XORed with a random mask. Random mask is then appended to resulting string,
		/// which is then returned as a randomized CSRF Token with the original hidden within it.
		/// In order to provide CSRFToken that looks different for each request, the original
		/// CSRF token is XORed with a random mask, and the mask is appended to the resulting
		/// XORed string.The original CSRFToken is obtained from XORing appended mask (right half) with the
		/// left half of the input string csrfToken. 
		/// 
		/// All this randomized looking CSRF token is for BREACH attack protection. Every request
		/// will look like it is submitting a different CSRF token.
		/// </summary>
		/// <param name="csrfToken"></param>
		/// <returns></returns>
		static public string GenerateCSRFToken(string csrfToken)
		{
			var csrfTokenSb = new StringBuilder(string.Empty);
			byte[] unmaskedCSRFBytes = new byte[CSRFTokenLength];

			byte[] maskBytes = null;

			if (string.IsNullOrEmpty(csrfToken))
			{
				//A new original CSRF Token is created.
				var rnd = new RNGCryptoServiceProvider();
				rnd.GetBytes(unmaskedCSRFBytes);

			}
			else
			{
				//The original CSRF token is obtained by getting the mask, which is the right half of
				//the input string, and XORing it with the left half of the input string.
				//The resulting left half is the original CSRFT token.
				const int randomizedCSRFTokenLength = CSRFTokenLength * 2;
				for (int i = 0; i < CSRFTokenLength; i++)
				{
					byte b = byte.Parse(csrfToken.Substring(i << 1, 2), NumberStyles.HexNumber);
					byte m = byte.Parse(csrfToken.Substring(randomizedCSRFTokenLength + (i << 1), 2), NumberStyles.HexNumber);
					unmaskedCSRFBytes[i] = (byte)(b ^ m);
				}

			}

			//Generate a new mask to create a new randomized CSRF token from the original.
			//Original will be hidden within the randomize CSRF token 
			maskBytes = new byte[CSRFTokenLength];
			var mask = new RNGCryptoServiceProvider();
			mask.GetBytes(maskBytes);

			//XOR original CSRF token with mask
			for (int i = 0; i < CSRFTokenLength; i++)
			{
				byte b = (byte)(unmaskedCSRFBytes[i] ^ maskBytes[i]);
				csrfTokenSb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}
			//Append the mask to resulting XORed string
			foreach (byte b in maskBytes)
			{
				csrfTokenSb.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}

			csrfToken = csrfTokenSb.ToString();

			return csrfToken;
		}


		/// <summary>
		/// Gets or sets the CSRF token.
		/// </summary>
		public string CSRFToken
		{
			get
			{
				this.csrfToken = GenerateCSRFToken(this.csrfToken);
				return this.csrfToken;
			}

			set
			{
				this.csrfToken = value;
			}
		}

		public string CSRFTokenWithParamName
		{
			get
			{
				return "CSRFToken=" + CSRFToken;
			}
		}

		/// <summary>
		/// Compares two randomized CSRF tokens to see if their original CSRF tokens match.
		/// 
		/// </summary>
		/// <param name="csrfToken1"></param>
		/// <param name="csrfToken2"></param>
		/// <returns></returns>
		static public bool ValidatedCSRFToken(string csrfToken1, string csrfToken2)
		{
			if (string.IsNullOrEmpty(csrfToken1) || string.IsNullOrEmpty(csrfToken2))
				return false;

			const int randomizedCSRFTokenLength = CSRFTokenLength * 2;
			int csrfToken1Length = csrfToken1.Length >> 2; //in bytes and original token size without the mask appended
			int csrfToken2Length = csrfToken2.Length >> 2; //in bytes and original token size without the mask appended

			if (csrfToken1Length != CSRFTokenLength || csrfToken2Length != CSRFTokenLength)
				return false;

			for (int i = 0; i < CSRFTokenLength; i++)
			{
				byte b0 = byte.Parse(csrfToken2.Substring(i << 1, 2), NumberStyles.HexNumber);
				byte m0 = byte.Parse(csrfToken2.Substring(randomizedCSRFTokenLength + (i << 1), 2), NumberStyles.HexNumber);
				byte b1 = byte.Parse(csrfToken1.Substring(i << 1, 2), NumberStyles.HexNumber);
				byte m1 = byte.Parse(csrfToken1.Substring(randomizedCSRFTokenLength + (i << 1), 2), NumberStyles.HexNumber);
				if ((b0 ^ m0) != (b1 ^ m1))
					return false;
			}


			return true;

		}

		/// <summary>
		/// Creates a SQL insert statement used to map a SQL Server process ID to a Fuels Manager Session.
		/// </summary>
		/// <param name="security">
		/// The security context that contains the Session GUID
		/// </param>
		/// <returns>
		/// A <see cref="string"/> that represents the SQL Insert statement.
		/// </returns>
		public static string CreateInsertSqlSessionCommandString(SecurityClass security)
		{
			string insertPreamble = "IF EXISTS (SELECT 1 FROM [dbo].[tblSessions] WHERE SessionGuid = '"
											+ security.Token.ToString() + "')" + Environment.NewLine;

			insertPreamble += "BEGIN" + Environment.NewLine;
			insertPreamble += "INSERT INTO map.tblSessionToSQLProcess (SessionGuid, SqlServerSessionID) VALUES ('"
									+ security.Token.ToString() + "', @@SPID); "
									+ Environment.NewLine;
			insertPreamble += "DECLARE @ID bigint; " + Environment.NewLine;
			insertPreamble += "SELECT @ID = CONVERT(bigint, SCOPE_IDENTITY());" + Environment.NewLine;
			insertPreamble += "IF (@ID IS NULL)" + Environment.NewLine;
			insertPreamble += "BEGIN" + Environment.NewLine;
			insertPreamble += "RAISERROR('Table tblSessionToSQLProcess does not have an identity column.',16,1); " + Environment.NewLine;
			insertPreamble += "END" + Environment.NewLine;
			insertPreamble += "ELSE" + Environment.NewLine;
			insertPreamble += "BEGIN" + Environment.NewLine;
			insertPreamble += "SELECT @ID 'SessionToSQLProcessIndex'; " + Environment.NewLine;
			insertPreamble += "END" + Environment.NewLine;

			insertPreamble += "END" + Environment.NewLine;

			return insertPreamble;
		}
		/// Creates a SQL insert statement used to map a SQL Server process ID to a Fuels Manager Session.
		/// </summary>
		/// <param name="security">
		/// The security context that contains the Session GUID
		/// </param>
		/// <returns>
		/// A <see cref="string"/> that represents the SQL Insert statement.
		/// </returns>
		public static SqlCommand CreateInsertSqlSessionCommand(SecurityClass security)
		{
			SqlCommand cmd = new SqlCommand();
			string commandString = "IF EXISTS (SELECT 1 FROM [dbo].[tblSessions] WHERE SessionGuid = @sessionGuid)" + Environment.NewLine;
			commandString += "BEGIN" + Environment.NewLine;
			commandString += "INSERT INTO map.tblSessionToSQLProcess (SessionGuid, SqlServerSessionID) VALUES (@sessionGuid, @@SPID); "
										+ Environment.NewLine;
			commandString += "DECLARE @ID bigint; " + Environment.NewLine;
			commandString += "SELECT @ID = CONVERT(bigint, SCOPE_IDENTITY());" + Environment.NewLine;
			commandString += "IF (@ID IS NULL)" + Environment.NewLine;
			commandString += "BEGIN" + Environment.NewLine;
			commandString += "RAISERROR('Table tblSessionToSQLProcess does not have an identity column.',16,1); " + Environment.NewLine;
			commandString += "END" + Environment.NewLine;
			commandString += "ELSE" + Environment.NewLine;
			commandString += "BEGIN" + Environment.NewLine;
			commandString += "SELECT @ID 'SessionToSQLProcessIndex'; " + Environment.NewLine;
			commandString += "END" + Environment.NewLine;

			commandString += "END" + Environment.NewLine;

         cmd.Parameters.Add(new SqlParameter("@sessionGuid", security.Token));

			cmd.CommandText = commandString;

			return cmd;
		}



		/// <summary>
		/// Creates a SQL delete statement used to remove a previously created mapping record between a SQL Server process ID and a Fuels Manager Session.
		/// </summary>
		/// <param name="index">
		/// The index of the mapping record that should be removed.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> that represents the SQL Delete statement.
		/// </returns>
		public static string CreateDeleteSqlSessionCommandString(long index)
		{
			string deletePostamble = string.Format("DELETE FROM map.tblSessionToSQLProcess WHERE SessionToSQLProcessIndex = {0}", index);
			return deletePostamble;
		}

		/// <summary>
		/// Creates a SQL delete statement used to remove a previously created mapping record between a SQL Server process ID and a Fuels Manager Session.
		/// </summary>
		/// <param name="index">
		/// The index of the mapping record that should be removed.
		/// </param>
		/// <returns>
		/// A <see cref="string"/> that represents the SQL Delete statement.
		/// </returns>
		public static SqlCommand CreateDeleteSqlSessionCommand(long index)
		{
			var cmd = new SqlCommand();

			string deletePostamble = "DELETE FROM map.tblSessionToSQLProcess WHERE SessionToSQLProcessIndex = @sessionIndexSessionToSQLProcessIndex";

			cmd.CommandText = deletePostamble;
			cmd.Parameters.Add(new SqlParameter("@sessionIndexSessionToSQLProcessIndex", index));

			return cmd;
		}

		/// <summary>
		/// Creates a SQL delete statement used to remove a previously created mapping record between a SQL Server process ID and
		/// a Fuels Manager Session for the current @@SPID
		/// </summary>
		/// <returns>
		/// A <see cref="string"/> that represents the SQL Delete statement.
		/// </returns>
		public static string CreateDeleteCurrentSqlSessionCommandString()
		{
			string deletePostamble = "DELETE FROM map.tblSessionToSQLProcess WHERE SqlServerSessionID = @@SPID";
			return deletePostamble;
		}

		/// <summary>
		/// The clone.
		/// </summary>
		/// <returns>
		/// The <see cref="SecurityClass"/>.
		/// </returns>
		public SecurityClass Clone()
		{
			var newObject = (SecurityClass)this.MemberwiseClone();
			newObject.CloneRights(this);

			return newObject;
		}

		/// <summary>
		/// Checks to see if the security object contains the specified right or 
		/// possibly an associated right that implies the specified right.
		/// </summary>
		/// <param name="right">The right to find.</param>
		/// <param name="checkAssociated">Set to <c>true</c> to authorize a check of associated rights as well.</param>
		/// <returns><c>True</c> if the right is found.</returns>
		public bool HasRight(RIGHT right, bool checkAssociated = true)
		{
			if (this.RightsArray[(int)right])
			{
				return true;
			}

			// if we are here then the user does not have this right but we need to 
			// check to see if the right is assumed modify right assumes view.
			if (checkAssociated)
			{
				return this.CheckAssociatedRight(right);
			}

			return false;
		}

		/// <summary>
		/// Clones the rights array into this class.
		/// </summary>
		/// <param name="security">The security.</param>
		public void CloneRights(SecurityClass security)
		{
			for (var index = 0; index < this.RightsArraySize; ++index)
			{
				this.RightsArray[index] = security.RightsArray[index];
			}
		}

		/// <summary>
		/// This method checks for rights that are associated with the given right.  For example, a Modify right 
		/// implies a View right of the same type.
		/// </summary>
		/// <param name="right">The right to check.</param>
		/// <returns>True if the specified right is implied by the presence of an associated right.</returns>
		private bool CheckAssociatedRight(RIGHT right)
		{
			RIGHT associatedRight;

			switch (right)
			{
				case RIGHT.VIEW_ALLOCATIONS:
					associatedRight = RIGHT.MODIFY_ALLOCATIONS;
					break;
				case RIGHT.VIEW_COMPANY_DATA:
					associatedRight = RIGHT.MODIFY_COMPANY_DATA;
					break;
				case RIGHT.VIEW_EQUIPMENT_DATA:
					associatedRight = RIGHT.MODIFY_EQUIPMENT_DATA;
					break;

				// TODO: Temporary commented out so that QA does not test financial configuration features.
				//case RIGHT.VIEW_FINANCIAL_DATA:
				//	AssociatedRight = RIGHT.MODIFY_FINANCIAL_DATA;
				//	break;

				case RIGHT.VIEW_FUEL_CARD_DATA:
					associatedRight = RIGHT.MODIFY_FUEL_CARD_DATA;
					break;
				case RIGHT.VIEW_LOAD_RACK_DATA:
					associatedRight = RIGHT.MODIFY_LOAD_RACK_DATA;
					break;
				case RIGHT.VIEW_PERSONNEL_DATA:
					associatedRight = RIGHT.MODIFY_PERSONNEL_DATA;
					break;
				case RIGHT.VIEW_PIDX_PROFILES:
					associatedRight = RIGHT.MODIFY_PIDX_PROFILES;
					break;
				case RIGHT.VIEW_PRODUCTS:
					associatedRight = RIGHT.MODIFY_PRODUCTS;
					break;
				case RIGHT.VIEW_QUERIES:
					associatedRight = RIGHT.MODIFY_QUERIES;
					break;
				case RIGHT.VIEW_REPORTS:
					associatedRight = RIGHT.MODIFY_REPORTS;
					break;
				case RIGHT.VIEW_SITES_AND_SITE_GROUPS:
					associatedRight = RIGHT.MODIFY_SITES_AND_SITE_GROUPS;
					break;
				case RIGHT.VIEW_STANDING_OFFERS:
					associatedRight = RIGHT.MODIFY_STANDING_OFFERS;
					break;
				case RIGHT.VIEW_TICKETING_DATA:
					associatedRight = RIGHT.MODIFY_TICKETING_DATA;
					break;
				case RIGHT.VIEW_TRANSACTION_ALIASES:
					associatedRight = RIGHT.MODIFY_TRANSACTION_ALIASES;
					break;
				case RIGHT.VIEW_TRANSACTION_DATA:
					associatedRight = RIGHT.MODIFY_TRANSACTION_DATA;
					break;
				case RIGHT.VIEW_USER_GROUPS:
					associatedRight = RIGHT.MODIFY_USER_GROUPS;
					break;
				case RIGHT.VIEW_USERS:
					associatedRight = RIGHT.MODIFY_USERS;
					break;
				case RIGHT.VIEW_CLOSEOUT_DATA:
					associatedRight = RIGHT.PERFORM_CLOSEOUT;
					break;
				case RIGHT.VIEW_QUALITY_TESTS:
					associatedRight = RIGHT.MODIFY_QUALITY_TESTS;
					break;
				case RIGHT.VIEW_TEST_ITEMS:
					associatedRight = RIGHT.MODIFY_TEST_ITEMS;
					break;
				case RIGHT.VIEW_QUALITYTAG_LOGS:
					associatedRight = RIGHT.MODIFY_QUALITYTAG_LOGS;
					break;
				case RIGHT.VIEW_QUALITYTAG_RECORD:
					associatedRight = RIGHT.ADD_QUALITYTAG_RECORD;
					break;
				case RIGHT.VIEW_DISPATCH:
					associatedRight = RIGHT.MODIFY_DISPATCH;
					break;
				case RIGHT.VIEW_INCOMING_TRUCK_DATA:
					associatedRight = RIGHT.MODIFY_INCOMING_TRUCK_DATA;
					break;
				case RIGHT.VIEW_TANK_DATA:
					associatedRight = RIGHT.MODIFY_TANK_DATA;
					break;
				case RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION:
					associatedRight = RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION;
					break;

				// TODO: Temporary commented out so that QA does not test financial configuration features. 
				//case RIGHT.VIEW_MOBILE_DEVICE_PROFILES:
				//	associatedRight = RIGHT.MODIFY_MOBILE_DEVICE_PROFILES;
				//	break;

				case RIGHT.VIEW_ENTITY_ASSIGNMENTS:
					associatedRight = RIGHT.MODIFY_ENTITY_ASSIGNMENTS;
					break;
				case RIGHT.VIEW_FIELD_LEVEL_CONTROL_CONFIGURATION:
					associatedRight = RIGHT.MODIFY_FIELD_LEVEL_CONTROL_CONFIGURATION;
					break;

				// TODO: Temporary commented out so that QA does not test financial configuration features.
				//case RIGHT.VIEW_MOBILE_DEVICES:
				//	associatedRight = RIGHT.MODIFY_MOBILE_DEVICES;
				//	break;
				case RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS:
					associatedRight = RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS;
					break;
				case RIGHT.VIEW_SYNC_CONFIG_SERVER_SETTINGS:
					associatedRight = RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS;
					break;
				case RIGHT.VIEW_SYNC_CONFIG_SITE_SETTINGS:
					associatedRight = RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS;
					break;
				case RIGHT.VIEW_SYNC_CONFLICT_STATUS:
					associatedRight = RIGHT.MODIFY_SYNC_CONFLICT_STATUS;
					break;

				case RIGHT.CREATE_IRS_EXSTARS_REPORT:
					associatedRight = RIGHT.VIEW_IRS_EXSTARS_REPORT;
					break;

				case RIGHT.VIEW_POINT_TEMPLATES:
					associatedRight = RIGHT.MODIFY_POINT_TEMPLATES;
					break;

				// Inventory Management Associations
				case RIGHT.VIEW_PICTURE_SUMMARY:
					associatedRight = RIGHT.MODIFY_PICTURE_SUMMARY;
					break;
				case RIGHT.VIEW_POINT_CATEGORIES:
					associatedRight = RIGHT.MODIFY_POINT_CATEGORIES;
					break;
				case RIGHT.VIEW_MODULE_LIBRARY:
					associatedRight = RIGHT.MODIFY_MODULE_LIBRARY;
					break;
				case RIGHT.VIEW_POINTS:
					associatedRight = RIGHT.MODIFY_POINTS;
					break;
				case RIGHT.VIEW_POINT_COMMANDSTATUS_LIST:
					associatedRight = RIGHT.MODIFY_POINT_COMMANDSTATUS_LIST;
					break;
				case RIGHT.OPERATE_VIEW_TRENDS:
					associatedRight = RIGHT.OPERATE_MODIFY_TRENDS;
					break;
				case RIGHT.VIEW_POINT_ACCESS_GROUP:
					associatedRight = RIGHT.MODIFY_POINT_ACCESS_GROUP;
					break;
				case RIGHT.VIEW_POINT_TYPES:
					associatedRight = RIGHT.MODIFY_POINT_TYPES;
					break;
				case RIGHT.OPERATE_VIEW_POINT_GROUPS:
					associatedRight = RIGHT.OPERATE_MODIFY_POINT_GROUPS;
					break;
            case RIGHT.VIEW_ONLY_SITE_CLOSEOUT_TIME:
               associatedRight = RIGHT.MODIFY_SITE_CLOSEOUT_TIME;
               break;
            case RIGHT.VIEW_FCEE_DATA:
               associatedRight = RIGHT.MODIFY_FCEE_DATA;
               break;
            default:
					return false;
			}

			return this.HasRight(associatedRight, checkAssociated: false);
		}

		/// <summary>
		/// The right id.
		/// </summary>
		/// <param name="right">
		/// The right.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string RightID(RIGHT right)
		{
			switch (right)
			{
				case RIGHT.VIEW_USERS: return "View Users";
				case RIGHT.VIEW_USER_GROUPS: return "View User Groups";
				case RIGHT.MODIFY_USERS: return "Modify Users";
				case RIGHT.MODIFY_USER_GROUPS: return "Modify User Groups";
				case RIGHT.IMPORT_CONFIGURATION_DATA: return "Import Configuration Data";
				case RIGHT.EXPORT_CONFIGURATION_DATA: return "Export Configuration Data";
				case RIGHT.VIEW_INSTALLED_MODULES_STATUS: return "View Installed Modules Status";
				case RIGHT.VIEW_SITES_AND_SITE_GROUPS: return "View Sites and Site Groups";
				case RIGHT.MODIFY_SITES_AND_SITE_GROUPS: return "Modify Sites and Site Groups";
				case RIGHT.VIEW_COMPANY_DATA: return "View Company Data";
				case RIGHT.MODIFY_COMPANY_DATA: return "Modify Company Data";
				case RIGHT.VIEW_PRODUCTS: return "View Product Data";
				case RIGHT.MODIFY_PRODUCTS: return "Modify Product Data";
				case RIGHT.VIEW_ALLOCATIONS: return "View Allocation Data";
				case RIGHT.MODIFY_ALLOCATIONS: return "Modify Allocation Data";
				case RIGHT.VIEW_EQUIPMENT_DATA: return "View Equipment Data";
				case RIGHT.MODIFY_EQUIPMENT_DATA: return "Modify Equipment Data";
				case RIGHT.VIEW_PERSONNEL_DATA: return "View Personnel Data";
				case RIGHT.MODIFY_PERSONNEL_DATA: return "Modify Personnel Data";
				case RIGHT.VIEW_TRANSACTION_DATA: return "View Transaction Data";
				case RIGHT.MODIFY_TRANSACTION_DATA: return "Modify Transaction Data";
				case RIGHT.VIEW_TRANSACTION_ALIASES: return "View Transaction Aliases";
				case RIGHT.MODIFY_TRANSACTION_ALIASES: return "Modify Transaction Aliases";
				case RIGHT.PERFORM_CLOSEOUT: return "Perform Closeout";
				case RIGHT.CONFIGURE_ACCOUNTING: return "Configure Accounting";
				case RIGHT.VIEW_LOAD_RACK_DATA: return "View Load Rack Data";
				case RIGHT.MODIFY_LOAD_RACK_DATA: return "Modify Load Rack Data";
				case RIGHT.VIEW_INVENTORY_DATA: return "View Inventory Data";
				case RIGHT.VIEW_REPORTS: return "View Reports";
				case RIGHT.MODIFY_REPORTS: return "Modify Reports";
				case RIGHT.CONFIGURE_IMPORT_EXPORT: return "Configure Import Export";
				case RIGHT.EXECUTE_IMPORT_EXPORT: return "Execute Import Export";
				case RIGHT.VIEW_TICKETING_DATA: return "View Ticketing Data";
				case RIGHT.MODIFY_TICKETING_DATA: return "Modify Ticketing Data";
				case RIGHT.VIEW_QUERIES: return "View Queries";
				case RIGHT.MODIFY_QUERIES: return "Modify Queries";
				case RIGHT.MODIFY_SYSTEM_SETTINGS: return "Modify System Settings";
				case RIGHT.PERFORM_REVERSE_TRANSACTION: return "Perform Reverse Transaction";
				case RIGHT.VIEW_STANDING_OFFERS: return "View Price List";
				case RIGHT.MODIFY_STANDING_OFFERS: return "Modify Price List";
				case RIGHT.VIEW_GRAPHICS: return "View Graphics";
				case RIGHT.VIEW_PIDX_PROFILES: return "View PIDX Profiles";
				case RIGHT.MODIFY_PIDX_PROFILES: return "Modify PIDX Profiles";
				case RIGHT.ENABLEDISABLE_STATIONS: return "Enable-Disable Stations";
				case RIGHT.CONFIGURE_TRAINING: return "Configure Training Items";
				case RIGHT.CONFIGURE_QUALIFICATIONS: return "Configure Qualification Items";
				case RIGHT.CONFIGURE_LICENSES: return "Configure Licenses";
				case RIGHT.VIEW_FUEL_CARD_DATA: return "View Fuel Card";
				case RIGHT.MODIFY_FUEL_CARD_DATA: return "Modify Fuel Card";
				//case RIGHT.VIEW_FINANCIAL_DATA: return "View Financial Data";		TODO: Temporary commented out so that QA does not test financial configuration features.
				//case RIGHT.MODIFY_FINANCIAL_DATA: return "Modify Financial Data";	TODO: Temporary commented out so that QA does not test financial configuration features.
				//case RIGHT.PRIVILEGED_FINANCIAL: return "Privileged Financial";	TODO: Temporary commented out so that QA does not test financial configuration features.
				case RIGHT.INTERFACE_IMPORT: return "Interface Import";
				case RIGHT.BACKUP_DATABASE: return "Backup Database";
				case RIGHT.TOGGLE_DATA_DICTIONARY: return "Toggle Data Dictionary";
				case RIGHT.VIEW_AUDIT_LOGS: return "View Audit Logs";
				case RIGHT.VIEW_ALARM_EVENT_LOGS: return "View Alarm Event Logs";
				case RIGHT.VIEW_CLOSEOUT_DATA: return "View Closeout Data";
				case RIGHT.CONFIGURE_QUERIES: return "Configure Queries";
				case RIGHT.CONFIGURE_RESERVE_LEVEL: return "Configure Reserve Levels";
				case RIGHT.UNDELETE_TRANSACTION_DATA: return "Undelete Transaction Data";
				case RIGHT.VIEW_BILLS_OF_LADING: return "View Bills Of Lading";
				case RIGHT.EXECUTE_QUALITY_TESTS: return "Execute Quality Tests";
				case RIGHT.MODIFY_QUALITY_TESTS: return "Modify Quality Tests";
				case RIGHT.VIEW_QUALITY_TESTS: return "View Quality Tests";
				case RIGHT.MODIFY_TEST_ITEMS: return "Modify Test Items";
				case RIGHT.VIEW_TEST_ITEMS: return "View Test Items";
				case RIGHT.ADD_MAINTENANCE_RECORD: return "Add Maintenance Record";
				case RIGHT.VIEW_MAINTENANCE_RECORD: return "View Maintenance Record";
				case RIGHT.MODIFY_MAINTENANCE_RECORD: return "Modify Maintenance Record";
				case RIGHT.VIEW_TRAINING_QUALIFICATIONS: return "View Personnel Training-Qualifications";
				case RIGHT.MODIFY_PERSON_QUALIFICATIONS: return "Modify Personnel Qualifications";
				case RIGHT.MODIFY_PERSON_TRAINING: return "Modify Personnel Training";
				case RIGHT.MODIFY_TRAINING_QUAL_HISTORY: return "Modify Training-Qualification History";
				case RIGHT.VIEW_TRAINING_QUAL_HISTORY: return "View Training-Qualification History";
				case RIGHT.MODIFY_APPOINTMENTS: return "Modify Appointments";
				case RIGHT.VIEW_APPOINTMENTS: return "View Appointments";
				case RIGHT.ADD_QUALITYTAG_RECORD: return "Add Quality Tag Record";
				case RIGHT.VIEW_QUALITYTAG_RECORD: return "View Quality Tag Record";
				case RIGHT.VIEW_QUALITYTAG_LOGS: return "View Quality Tag Logs";
				case RIGHT.MODIFY_QUALITYTAG_RECORD: return "Modify Quality Tag Record";
				case RIGHT.MODIFY_QUALITYTAG_LOGS: return "Modify Quality Tag Logs";
				case RIGHT.VIEW_DISPATCH: return "View Dispatch";
				case RIGHT.VIEW_DATABASE_AUDIT_LOG: return "View Database Audit Log";
				case RIGHT.MODIFY_DATABASE_AUDIT_LOG: return "Modify Database Audit Log";
				case RIGHT.MODIFY_DISPATCH: return "Modify Dispatch";
				case RIGHT.CONFIGURE_DISPATCH_VALIDATIONS: return "Configure Dispatch Validations";
				case RIGHT.OVERRIDE_WAC: return "Override WAC";
				case RIGHT.VIEW_WAC_HISTORY: return "View WAC History";
				case RIGHT.MODIFY_INVOICE_QUERIES: return "Modify Invoice Queries";
				case RIGHT.ACCESS_MFCS: return "Access MFCS";
				case RIGHT.ACCESS_ARTS: return "Access ARTS";
				case RIGHT.BASE_EXPORT: return "Base Export";
				case RIGHT.ENTERPRISE_EXPORT: return "Enterprise Export";
				case RIGHT.IMPORT_ENTITIES: return "Import Entities";
				case RIGHT.EXPORT_ENTITIES: return "Export Entities";
				case RIGHT.IMPORT_ENTERPRISE_DATA: return "Import Enterprise Data";
				case RIGHT.EXPORT_ENTERPRISE_DATA: return "Export Enterprise Data";
				case RIGHT.ALLOW_SINGLE_SITE_GROUP_SELECT: return "Allow Enterprise Configuration";
				case RIGHT.CONFIGURE_FOOTNOTES: return "Configure Footnotes";
				case RIGHT.EOM_APPROVAL_ACCOUNTABLE: return "EOM Approval - Accountable";
				case RIGHT.EOM_APPROVAL_APPROVING: return "EOM Approval - Approving";
				case RIGHT.VIEW_INCOMING_TRUCK_DATA: return "View Incoming Truck Data";
				case RIGHT.MODIFY_INCOMING_TRUCK_DATA: return "Modify Incoming Truck Data";
				case RIGHT.ACCESS_ACCOUNTING_OPERATIONS: return "Access Accounting Operations";
				case RIGHT.ACCESS_ACCOUNTING_LEDGER: return "Access Accounting Ledger";
				case RIGHT.ACCESS_ONLINE_HELP: return "Access Online Help";
				case RIGHT.ACCESS_ONLINE_TUTORIALS: return "Access Online Tutorials";
				case RIGHT.ACCESS_ONLINE_ADMIN_MANUAL: return "Access Online Admin Help";
				case RIGHT.ACCESS_ONLINE_ADMIN_TUTORIAL: return "Access Online Admin Tutorials";
				case RIGHT.BASE_EXPORT_MANUAL: return "Base Export Manual";
				case RIGHT.RAPS_IMPORT: return "RAPS Import";
				case RIGHT.MODIFY_ERROR_TRANSACTION: return "Modify Error Transaction";
				case RIGHT.SEND_TO_EBS: return "Send Transactions to EBS";
				case RIGHT.MODIFY_TANK_DATA: return "Modify Tank Data";
				case RIGHT.VIEW_TANK_DATA: return "View Tank Data";
				case RIGHT.EXPORT_INFLIGHT_TRANSACTIONS: return "Export Inflight Transactions";
				case RIGHT.MODIFY_SUSPENDED_TRANSACTIONS: return "Modify Suspended Transactions";
				case RIGHT.MODIFY_CONFIGURATION_SETTINGS: return "Modify Configuration Settings";
				case RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION: return "View Auto Distribution Configuration";
				case RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION: return "Modify Auto Distribution Configuration";
				case RIGHT.PERFORM_AUTO_DISTRIBUTION: return "Perform Auto Distribution";
				case RIGHT.MODIFY_METERS: return "Modify Meters";
				case RIGHT.VIEW_METERS: return "View Meters";
				case RIGHT.VIEW_METER_RECONCILIATION: return "View Meter Reconciliation";
				case RIGHT.VIEW_SERVICE_REQUEST_MESSAGING_CONFIGURATION: return "View Service Request Messaging Configuration";
				case RIGHT.MODIFY_SERVICE_REQUEST_MESSAGING_CONFIGURATION: return "Modify Service Request Messaging Configuration";
				// case RIGHT.VIEW_MOBILE_DEVICE_PROFILES: return "View Mobile Device Profiles"; TODO: Temporary commented out so that QA does not test financial configuration features.
				// case RIGHT.MODIFY_MOBILE_DEVICE_PROFILES: return "Modify Mobile Device Profiles"; TODO: Temporary commented out so that QA does not test financial configuration features.
				case RIGHT.ACCESS_COOGEE_TANK_ADJ: return "Access Variance Distribution";
				case RIGHT.VIEW_ENTITY_ASSIGNMENTS: return "View Entity Assignments";
				case RIGHT.MODIFY_ENTITY_ASSIGNMENTS: return "Modify Entity Assignments";
				case RIGHT.VIEW_FIELD_LEVEL_CONTROL_CONFIGURATION: return "View Field Level Control Configuration";
				case RIGHT.MODIFY_FIELD_LEVEL_CONTROL_CONFIGURATION: return "Modify Field Level Control Configuration";
				// case RIGHT.VIEW_MOBILE_DEVICES: return "View Mobile Devices"; TODO: Temporary commented out so that QA does not test financial configuration features.
				// case RIGHT.MODIFY_MOBILE_DEVICES: return "Modify Mobile Devices"; TODO: Temporary commented out so that QA does not test financial configuration features.
				// case RIGHT.LOGIN_FROM_MOBILE_DEVICE: return "Login from Mobile Device"; TODO: Temporary commented out so that QA does not test financial configuration features.
				case RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS: return "View Client Synchronization Configuration";
				case RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS: return "Modify Client Synchronization Configuration";
				case RIGHT.VIEW_SYNC_CONFIG_SERVER_SETTINGS: return "View Server Synchronization Configuration";
				case RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS: return "Modify Server Synchronization Configuration";
				case RIGHT.VIEW_SYNC_CONFIG_SITE_SETTINGS: return "View Site Synchronization Settings";
				case RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS: return "Modify Site Synchronization Settings";
				case RIGHT.VIEW_SYNC_CONFLICT_STATUS: return "View Conflict Status";
				case RIGHT.MODIFY_SYNC_CONFLICT_STATUS: return "Modify Conflict Status";
				case RIGHT.PERFORM_SYNCHRONIZATION: return "Perform a Synchronization Request";
				case RIGHT.MIGRATION_PERFORM_IMPORT_EXPORT:
					return "Perform data migration import and export.";
				case RIGHT.CONFIGURE_AVIATION_EXPORT: return "Configure Aviation Export";
				case RIGHT.VIEW_FUEL_CARD_LIMIT:
					return "View Fuel Card Limits";
				case RIGHT.MODIFY_FUEL_CARD_LIMIT:
					return "Modify Fuel Card Limits";
				case RIGHT.VIEW_INVENTORY_RECONCILIATION:
					return "View Inventory Reconciliation";
				case RIGHT.IMPORT_NATO_FUELCARD:
					return "Import NATO Fuel Card";
				case RIGHT.CREATE_IRS_EXSTARS_REPORT:
					return "Create IRS ExSTARS Report";
				case RIGHT.VIEW_IRS_EXSTARS_REPORT:
					return "View IRS ExSTARS Report";
				case RIGHT.IRS_EXSTARS_MANAGER:
					return "IRS EXSTARS MANAGER";
				case RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION:
					return "View External Stations";

				case RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION:
					return "Modify External Stations";
				case RIGHT.MODIFY_UNOBTAINABLE:
					return "Modify Unobtainable Quantity";
				case RIGHT.CONFIGURE_LOCATIONS:
					return "Configure Locations";
				case RIGHT.VIEW_MOVEMENT:
					return "View Movement Calendar";
				case RIGHT.CONFIGURE_WEB_LINKS:
					return "Configure Web Links";
				case RIGHT.CONFIGURE_DLA_TEST:
					return "Configure DLA Test";
				case RIGHT.VIEW_DATA_ANALYTICS:
					return "View Data Analytics";
				case RIGHT.VIEW_MAPS:
					return "View Maps";
				case RIGHT.VIEW_MAP_CONFIGURATION:
					return "View Map Configuration";
				case RIGHT.MODIFY_MAP_CONFIGURATION:
					return "Modify Map Configuration";
				case RIGHT.VIEW_ASSET_TRACKING_DEVICES:
					return "View Asset Tracking Devices";
				case RIGHT.MODIFY_ASSET_TRACKING_DEVICES:
					return "Modify Asset Tracking Devices";
				case RIGHT.VIEW_ICON_CONFIGURATION:
					return "View Icon Configuration";
				case RIGHT.MODIFY_ICON_CONFIGURATION:
					return "Modify Icon Configuration";
				case RIGHT.MAP_INITIATE_INVESTIGATION:
					return "Map Initiate Investigation";
				case RIGHT.MAP_COMPLETE_INVESTIGATION:
					return "Map Complete Investigation";
				case RIGHT.CREATE_ORDERS:
					return "Create Orders";
				case RIGHT.VIEW_ORDERS:
					return "View Orders";
				case RIGHT.MODIFY_ORDERS:
					return "Modify Orders";
				case RIGHT.CREATE_SUPPLY_ORDERS:
					return "Create Supply Orders";
				case RIGHT.VIEW_SUPPLY_ORDERS:
					return "View Supply Orders";
				case RIGHT.MODIFY_SUPPLY_ORDERS:
					return "Modify Supply Orders";
				case RIGHT.IMPORT_TRANSACTION:
					return "Import Transactions";
				case RIGHT.VIEW_POINT_TEMPLATES:
					return "View Point Templates";
				case RIGHT.MODIFY_POINT_TEMPLATES:
					return "Modify Point Templates";
				case RIGHT.ACKNOWLEDGE_ALL_ALARMS:
					return "Acknowledge All Alarms";
				case RIGHT.ACKNOWLEDGE_WITH_COMMENTS:
					return "Acknowledge All Alarms with Comments";
				case RIGHT.ACCESS_DRAW:
					return "Access Draw";
				case RIGHT.VIEW_PICTURE_SUMMARY:
					return "View Picture Summary";
				case RIGHT.MODIFY_PICTURE_SUMMARY:
					return "Modify Picture Summary";
				case RIGHT.VIEW_POINT_CATEGORIES:
					return "View Point Categories";
				case RIGHT.MODIFY_POINT_CATEGORIES:
					return "Modify Point Categories";
				case RIGHT.VIEW_POINT_ACCESS_GROUP:
					return "View Point Access Group";
				case RIGHT.MODIFY_POINT_ACCESS_GROUP:
					return "Modify Point Access Group";
				case RIGHT.VIEW_POINT_TYPES:
					return "View Point Types";
				case RIGHT.MODIFY_POINT_TYPES:
					return "Modify Point Types";
				//case RIGHT.VIEW_POINT_TEMPLATES:
				//	return "View Point Templates";
				//case RIGHT.MODIFY_POINT_TEMPLATES:
				//	return "Modify Point Templates";
				case RIGHT.VIEW_MODULE_LIBRARY:
					return "View Module Library";
				case RIGHT.MODIFY_MODULE_LIBRARY:
					return "Modify Module Library";
				case RIGHT.VIEW_POINTS:
					return "View Points";
				case RIGHT.MODIFY_POINTS:
					return "Modify Points";
				case RIGHT.ENABLE_POINTS:
					return "Enable Points";
				case RIGHT.DISABLE_POINTS:
					return "Disable Points";
				case RIGHT.ENABLE_ALARMS_ON_POINTS:
					return "Enable Alarms on Points";
				case RIGHT.DISABLE_ALARMS_ON_POINTS:
					return "Disable Alarms on Points";
				case RIGHT.VIEW_POINT_COMMANDSTATUS_LIST:
					return "View Point Command-Status List";
				case RIGHT.MODIFY_POINT_COMMANDSTATUS_LIST:
					return "Modify Point Command-Status List";
				case RIGHT.VIEW_OPERATE_ONLY:
					return "View Operate Only";
				case RIGHT.OPERATE_CREATE_PUBLIC_POINT_GROUPS:
					return "Operate Create Public Point Groups";
				case RIGHT.OPERATE_CREATE_SHARED_POINT_GROUPS:
					return "Operate Create Shared Point Groups";
				case RIGHT.OPERATE_MODIFY_PUBLIC_POINT_GROUPS:
					return "Operate Modify Public Point Groups";
				case RIGHT.OPERATE_MODIFY_SHARED_POINT_GROUPS:
					return "Operate Modify Shared Point Groups";
				case RIGHT.OPERATE_USE_POINT_CALCULATOR:
					return "Operate Use Point Calculator";
				case RIGHT.ACCESS_TAG_VIEWER:
					return "Access Tag Viewer";
				case RIGHT.OPERATE_VIEW_ALARM_SUMMARY:
					return "Operate View Alarm Summary";
				case RIGHT.OPERATE_VIEW_ALARM_HISTORY:
					return "Operate View Alarm History";
				case RIGHT.OPERATE_VIEW_TRENDS:
					return "Operate View Trends";
				case RIGHT.OPERATE_MODIFY_TRENDS:
					return "Operate Modify Trends";
				case RIGHT.OPERATE_VIEW_IM_REPORTS:
					return "Operate View Inventory Management Reports";
				case RIGHT.SILENCE_ALARMS:
					return "Silence Alarms";
				case RIGHT.COPY_POINT_TEMPLATES:
					return "Copy Point Templates";
				case RIGHT.OPERATE_VIEW_POINT_GROUPS:
					return "Operate View Point Groups";
				case RIGHT.OPERATE_MODIFY_POINT_GROUPS:
					return "Operate Modify Point Groups";
				case RIGHT.ENABLE_ALARMS_ON_POINT_TEMPLATES:
					return "Enable Alarms on Point Templates";
				case RIGHT.DISABLE_ALARMS_ON_POINT_TEMPLATES:
					return "Disable Alarms on Point Templates";
				case RIGHT.OPERATE_VIEW_POINTS:
					return "Operate View Points";
				case RIGHT.OPERATE_VIEW_GRAPHICS:
					return "Operate View Graphics";
				case RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION:
					return "Remote Enterprise Entity Configuration";
				case RIGHT.ACCESS_SYNC_DASHBOARD: return "Access Sync Dashboard";
				case RIGHT.ACCESS_ADMIN_DASHBOARD: return "Access Admin Dashboard";
				case RIGHT.IMPORT_IM_TANK_DATA: return "Import IM Tank Data";
				case RIGHT.ACCESS_RECONCILIATION_VIEWS: return "Access Reconciliation Views";
				case RIGHT.ACCESS_TRANSACTION_DETAIL_CLIN: return "Access Transaction Detail CLIN";
				case RIGHT.PRODUCT_AUTHORIZATION_AND_CONTROL: return "Product Authorization and Control";
				case RIGHT.PRODUCT_AUTHORIZATION_OVERRIDE: return "Product Authorization Override";
				case RIGHT.MODIFY_EPOS_MANUAL_TRANSACTION: return "Allow EPoS Transaction Manual Review";
				case RIGHT.IMPORT_EPOS_TRANSACTION_FILE: return "Import EPoS Transaction File";
				case RIGHT.OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY: return "Operate Create Public Movement Summary";
				case RIGHT.OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY: return "Operate Create Shared Movement Summary";
				case RIGHT.OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY: return "Operate Modify Public Movement Summary";
				case RIGHT.OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY: return "Operate Modify Shared Movement Summary";
				case RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY: return "Operate Modify Movement Summary";
				case RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY: return "Operate View Movement Summary";
				case RIGHT.OPERATE_MODIFY_MOVEMENT_HISTORY: return "Operate Modify Movement History";
				case RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY: return "Operate View Movement History";
				case RIGHT.VIEW_FCEE_DATA: return "View FCEE Data";
				case RIGHT.MODIFY_FCEE_DATA: return "Modify FCEE Data";
				case RIGHT.AVIATION_MODIFY_PUSH_VENDOR_TRANS: return "Aviation Modify Push Vendor Transactions";
				case RIGHT.AVIATION_VIEW_PUSH_VENDOR_TRANS: return "Aviation View Push Vendor Transactions";
				case RIGHT.AVIATION_MODIFY_PULL_VENDOR_TRANS: return "Aviation Modify Pull Vendor Transactions";
				case RIGHT.AVIATION_VIEW_PULL_VENDOR_TRANS: return "Aviation View Pull Vendor Transactions";
				case RIGHT.AVIATION_MODIFY_TANK_FARM_VENDOR: return "Aviation Modify Tank Farm Vendor";
				case RIGHT.AVIATION_VIEW_TANK_FARM_VENDOR: return "Aviation View Tank Farm Vendor";
				case RIGHT.MOBILE_LAUNCH: return "Launch Mobile Dispatch App";
				case RIGHT.MOBILE_VIEW_CONFIGURATION: return "View Mobile Dispatch Configuration";
				case RIGHT.MOBILE_MODIFY_CONFIGURATION: return "Modify Mobile Dispatch Configuration";
				case RIGHT.MOBILE_ROOT_MENU_DISPLAY: return "Display Mobile Root Menu";
				case RIGHT.OPERATE_PERFORM_LEAK_DETECTION: return "Operate Perform Leak Detection";
				case RIGHT.ROLLING_STOCK_IMPORT: return "Rolling Stock Import";
				case RIGHT.ALLOW_ACK_FM_LICENSE_EXPIRATION_WARNING: return "Acknowledge License Expiration Warning";
				case RIGHT.CONFIGURE_NOTIFY_ALARMS_ON_POINTS:  return "Configure Notify Alarms on Points";
				case RIGHT.VIEW_OPERATE_STATISTICS:  return "View Operate Statistics";
				case RIGHT.OPERATE_VIEW_UNPUBLISHED: return "Operate View Unpublished";
				case RIGHT.OPERATE_VIEW_POINT_HISTORY: return "Operate View Point History";
            case RIGHT.MODIFY_SITE_CLOSEOUT_TIME: return "Modify Site Closeout Time";
            case RIGHT.VIEW_ONLY_SITE_CLOSEOUT_TIME: return "View Only Site Closeout";
            case RIGHT.OPERATE_ADMINISTER_POINT_GROUP: return "Operate Administer Point Group";
            case RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY: return "Operate Administer Movement Summary";

            default:
					return UndefinedRightText;
			}
		}

		/// <summary>
		/// The get core product security rights.
		/// </summary>
		/// <returns>
		/// The <see cref="RightCollectionClass"/>.
		/// </returns>
		public static RightCollectionClass GetCoreProductSecurityRights()
		{
			var rights = new RightCollectionClass();

			//================================================================
			// Add all core product security rights.  
			// Only add core product rights to this rights collection
			//================================================================
			// Use ISecurityDiscovery for optional security rights
			//================================================================
			rights.Add(RIGHT.CONFIGURE_ACCOUNTING);
			rights.Add(RIGHT.CONFIGURE_IMPORT_EXPORT);
			rights.Add(RIGHT.CONFIGURE_LICENSES);
			rights.Add(RIGHT.CONFIGURE_QUALIFICATIONS);
			rights.Add(RIGHT.CONFIGURE_TRAINING);
			rights.Add(RIGHT.CREATE_ORDERS);
			// WCG : Temporarily Added Modify View Orders and Supply Orders until figure out why they are not here already
			rights.Add(RIGHT.MODIFY_ORDERS);
			rights.Add(RIGHT.VIEW_ORDERS);
			rights.Add(RIGHT.CREATE_SUPPLY_ORDERS);
			rights.Add(RIGHT.MODIFY_SUPPLY_ORDERS);
			rights.Add(RIGHT.VIEW_SUPPLY_ORDERS);
			rights.Add(RIGHT.ENABLEDISABLE_STATIONS);
			rights.Add(RIGHT.EXECUTE_IMPORT_EXPORT);
			rights.Add(RIGHT.INTERFACE_IMPORT);
			rights.Add(RIGHT.BACKUP_DATABASE);
			rights.Add(RIGHT.MODIFY_ALLOCATIONS);
			rights.Add(RIGHT.MODIFY_COMPANY_DATA);
			rights.Add(RIGHT.MODIFY_EQUIPMENT_DATA);
			// Rights.Add ( RIGHT.MODIFY_FINANCIAL_DATA ); TODO: Temporary commented out so that QA does not test financial configuration features.
			rights.Add(RIGHT.MODIFY_FUEL_CARD_DATA);
			rights.Add(RIGHT.MODIFY_LOAD_RACK_DATA);
			rights.Add(RIGHT.MODIFY_PERSONNEL_DATA);
			rights.Add(RIGHT.MODIFY_PIDX_PROFILES);
			rights.Add(RIGHT.MODIFY_PRODUCTS);
			rights.Add(RIGHT.MODIFY_QUERIES);
			rights.Add(RIGHT.MODIFY_REPORTS);
			rights.Add(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
			rights.Add(RIGHT.MODIFY_STANDING_OFFERS);
			rights.Add(RIGHT.MODIFY_SYSTEM_SETTINGS);
			rights.Add(RIGHT.MODIFY_TICKETING_DATA);
			rights.Add(RIGHT.MODIFY_TRANSACTION_ALIASES);
			rights.Add(RIGHT.MODIFY_TRANSACTION_DATA);
			rights.Add(RIGHT.MODIFY_USER_GROUPS);
			rights.Add(RIGHT.IMPORT_CONFIGURATION_DATA);
			rights.Add(RIGHT.EXPORT_CONFIGURATION_DATA);
			rights.Add(RIGHT.MODIFY_USERS);
			rights.Add(RIGHT.PERFORM_CLOSEOUT);
			rights.Add(RIGHT.PERFORM_REVERSE_TRANSACTION);
			// Rights.Add ( RIGHT.PRIVILEGED_FINANCIAL ); TODO: Temporary commented out so that QA does not test financial configuration features.
			rights.Add(RIGHT.VIEW_ALLOCATIONS);
			rights.Add(RIGHT.VIEW_COMPANY_DATA);
			rights.Add(RIGHT.VIEW_EQUIPMENT_DATA);
			// Rights.Add ( RIGHT.VIEW_FINANCIAL_DATA ); TODO: Temporary commented out so that QA does not test financial configuration features.
			rights.Add(RIGHT.VIEW_FUEL_CARD_DATA);
			rights.Add(RIGHT.VIEW_GRAPHICS);
			rights.Add(RIGHT.VIEW_INSTALLED_MODULES_STATUS);
			rights.Add(RIGHT.VIEW_INVENTORY_DATA);
			rights.Add(RIGHT.VIEW_LOAD_RACK_DATA);
			rights.Add(RIGHT.VIEW_PERSONNEL_DATA);
			rights.Add(RIGHT.VIEW_PIDX_PROFILES);
			rights.Add(RIGHT.VIEW_PRODUCTS);
			rights.Add(RIGHT.VIEW_QUERIES);
			rights.Add(RIGHT.VIEW_REPORTS);
			rights.Add(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
			rights.Add(RIGHT.VIEW_STANDING_OFFERS);
			rights.Add(RIGHT.VIEW_TICKETING_DATA);
			rights.Add(RIGHT.VIEW_TRANSACTION_ALIASES);
			rights.Add(RIGHT.VIEW_TRANSACTION_DATA);
			rights.Add(RIGHT.VIEW_USER_GROUPS);
			rights.Add(RIGHT.VIEW_USERS);
			rights.Add(RIGHT.TOGGLE_DATA_DICTIONARY);
			rights.Add(RIGHT.VIEW_AUDIT_LOGS);
			rights.Add(RIGHT.VIEW_ALARM_EVENT_LOGS);
			rights.Add(RIGHT.VIEW_CLOSEOUT_DATA);
			rights.Add(RIGHT.CONFIGURE_QUERIES);
			rights.Add(RIGHT.CONFIGURE_RESERVE_LEVEL);
			rights.Add(RIGHT.UNDELETE_TRANSACTION_DATA);
			rights.Add(RIGHT.VIEW_BILLS_OF_LADING);
			rights.Add(RIGHT.EXECUTE_QUALITY_TESTS);
			rights.Add(RIGHT.MODIFY_QUALITY_TESTS);
			rights.Add(RIGHT.VIEW_QUALITY_TESTS);
			rights.Add(RIGHT.MODIFY_TEST_ITEMS);
			rights.Add(RIGHT.VIEW_TEST_ITEMS);
			rights.Add(RIGHT.ADD_MAINTENANCE_RECORD);
			rights.Add(RIGHT.VIEW_MAINTENANCE_RECORD);
			rights.Add(RIGHT.MODIFY_MAINTENANCE_RECORD);
			rights.Add(RIGHT.VIEW_TRAINING_QUALIFICATIONS);
			rights.Add(RIGHT.MODIFY_PERSON_QUALIFICATIONS);
			rights.Add(RIGHT.MODIFY_PERSON_TRAINING);
			rights.Add(RIGHT.MODIFY_TRAINING_QUAL_HISTORY);
			rights.Add(RIGHT.VIEW_TRAINING_QUAL_HISTORY);
			rights.Add(RIGHT.VIEW_APPOINTMENTS);
			rights.Add(RIGHT.MODIFY_APPOINTMENTS);
			rights.Add(RIGHT.ADD_QUALITYTAG_RECORD);
			rights.Add(RIGHT.VIEW_QUALITYTAG_RECORD);
			rights.Add(RIGHT.VIEW_QUALITYTAG_LOGS);
			rights.Add(RIGHT.MODIFY_QUALITYTAG_RECORD);
			rights.Add(RIGHT.MODIFY_QUALITYTAG_LOGS);
			rights.Add(RIGHT.VIEW_DISPATCH);
			// rights.Add ( RIGHT.MODIFY_DATABASE_AUDIT_LOG ); TODO: Commented out as Database Audit Log functionality is removed from system
			// rights.Add ( RIGHT.VIEW_DATABASE_AUDIT_LOG ); TODO: Commented out as Database Audit Log functionality is removed from system
			rights.Add(RIGHT.MODIFY_DISPATCH);
			rights.Add(RIGHT.CONFIGURE_DISPATCH_VALIDATIONS);
			rights.Add(RIGHT.BASE_EXPORT);
			rights.Add(RIGHT.ENTERPRISE_EXPORT);
			rights.Add(RIGHT.IMPORT_ENTITIES);
			rights.Add(RIGHT.EXPORT_ENTITIES);
			rights.Add(RIGHT.IMPORT_ENTERPRISE_DATA);
			rights.Add(RIGHT.EXPORT_ENTERPRISE_DATA);
			// WCG 5/22/2014 : Removing this right from the system.  However the code will remain in case in the future
			//					a decision is made to support this capability.
			//			rights.Add ( RIGHT.ALLOW_SINGLE_SITE_GROUP_SELECT );
			rights.Add(RIGHT.CONFIGURE_FOOTNOTES);

			// TODO: End of month approval is a TFMD feature and is out of scope for Cirrus
			// rights.Add ( RIGHT.EOM_APPROVAL_ACCOUNTABLE ); 
			// rights.Add ( RIGHT.EOM_APPROVAL_APPROVING );
			rights.Add(RIGHT.VIEW_INCOMING_TRUCK_DATA);
			rights.Add(RIGHT.MODIFY_INCOMING_TRUCK_DATA);
			rights.Add(RIGHT.ACCESS_ACCOUNTING_OPERATIONS);
			rights.Add(RIGHT.ACCESS_ACCOUNTING_LEDGER);
			rights.Add(RIGHT.ACCESS_ONLINE_HELP);
			rights.Add(RIGHT.ACCESS_ONLINE_TUTORIALS);
			rights.Add(RIGHT.ACCESS_ONLINE_ADMIN_MANUAL);
			rights.Add(RIGHT.ACCESS_ONLINE_ADMIN_TUTORIAL);
			rights.Add(RIGHT.MODIFY_CONFIGURATION_SETTINGS);
			rights.Add(RIGHT.VIEW_TANK_DATA);
			rights.Add(RIGHT.MODIFY_TANK_DATA);
			rights.Add(RIGHT.VIEW_AUTO_DISTRIBUTION_CONFIGURATION);
			rights.Add(RIGHT.MODIFY_AUTO_DISTRIBUTION_CONFIGURATION);
			rights.Add(RIGHT.PERFORM_AUTO_DISTRIBUTION);
			rights.Add(RIGHT.VIEW_METERS);
			rights.Add(RIGHT.MODIFY_METERS);
			rights.Add(RIGHT.VIEW_METER_RECONCILIATION);
			// rights.Add (RIGHT.VIEW_MOBILE_DEVICE_PROFILES); TODO: Temporary commented out so that QA does not test financial configuration features.
			// rights.Add (RIGHT.MODIFY_MOBILE_DEVICE_PROFILES); TODO: Temporary commented out so that QA does not test financial configuration features.
			rights.Add(RIGHT.VIEW_ENTITY_ASSIGNMENTS);
			rights.Add(RIGHT.MODIFY_ENTITY_ASSIGNMENTS);
			rights.Add(RIGHT.VIEW_FIELD_LEVEL_CONTROL_CONFIGURATION);
			rights.Add(RIGHT.MODIFY_FIELD_LEVEL_CONTROL_CONFIGURATION);
			// rights.Add (RIGHT.VIEW_MOBILE_DEVICES); TODO: Temporary commented out so that QA does not test financial configuration features.
			// rights.Add (RIGHT.MODIFY_MOBILE_DEVICES); TODO: Temporary commented out so that QA does not test financial configuration features.
			// rights.Add (RIGHT.LOGIN_FROM_MOBILE_DEVICE); TODO: Temporary commented out so that QA does not test financial configuration features.
			rights.Add(RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS);
			rights.Add(RIGHT.MODIFY_SYNC_CONFIG_CLIENT_SETTINGS);
			rights.Add(RIGHT.VIEW_SYNC_CONFIG_SERVER_SETTINGS);
			rights.Add(RIGHT.MODIFY_SYNC_CONFIG_SERVER_SETTINGS);
			rights.Add(RIGHT.VIEW_SYNC_CONFIG_SITE_SETTINGS);
			rights.Add(RIGHT.MODIFY_SYNC_CONFIG_SITE_SETTINGS);
			rights.Add(RIGHT.MODIFY_SYNC_CONFLICT_STATUS);
			rights.Add(RIGHT.VIEW_SYNC_CONFLICT_STATUS);
			rights.Add(RIGHT.PERFORM_SYNCHRONIZATION);
			rights.Add(RIGHT.MIGRATION_PERFORM_IMPORT_EXPORT);
			rights.Add(RIGHT.CONFIGURE_AVIATION_EXPORT);
			rights.Add(RIGHT.VIEW_FUEL_CARD_LIMIT);
			rights.Add(RIGHT.MODIFY_FUEL_CARD_LIMIT);
			rights.Add(RIGHT.VIEW_INVENTORY_RECONCILIATION);
			rights.Add(RIGHT.CREATE_IRS_EXSTARS_REPORT);
			rights.Add(RIGHT.VIEW_IRS_EXSTARS_REPORT);
			rights.Add(RIGHT.VIEW_MAPS);
			rights.Add(RIGHT.VIEW_POINT_TEMPLATES);
			rights.Add(RIGHT.MODIFY_POINT_TEMPLATES);
			rights.Add(RIGHT.ACKNOWLEDGE_ALL_ALARMS);
			rights.Add(RIGHT.ACKNOWLEDGE_WITH_COMMENTS);
			rights.Add(RIGHT.ACCESS_DRAW);
			rights.Add(RIGHT.VIEW_PICTURE_SUMMARY);
			rights.Add(RIGHT.MODIFY_PICTURE_SUMMARY);
			rights.Add(RIGHT.VIEW_POINT_CATEGORIES);
			rights.Add(RIGHT.MODIFY_POINT_CATEGORIES);
			rights.Add(RIGHT.VIEW_POINT_ACCESS_GROUP);
			rights.Add(RIGHT.MODIFY_POINT_ACCESS_GROUP);
			rights.Add(RIGHT.VIEW_POINT_TYPES);
			rights.Add(RIGHT.MODIFY_POINT_TYPES);
			rights.Add(RIGHT.VIEW_MODULE_LIBRARY);
			rights.Add(RIGHT.MODIFY_MODULE_LIBRARY);
			rights.Add(RIGHT.VIEW_POINTS);
			rights.Add(RIGHT.MODIFY_POINTS);
			rights.Add(RIGHT.ENABLE_POINTS);
			rights.Add(RIGHT.DISABLE_POINTS);
			rights.Add(RIGHT.ENABLE_ALARMS_ON_POINTS);
			rights.Add(RIGHT.DISABLE_ALARMS_ON_POINTS);
			rights.Add(RIGHT.VIEW_POINT_COMMANDSTATUS_LIST);
			rights.Add(RIGHT.MODIFY_POINT_COMMANDSTATUS_LIST);
			rights.Add(RIGHT.VIEW_OPERATE_ONLY);
			rights.Add(RIGHT.OPERATE_CREATE_PUBLIC_POINT_GROUPS);
			rights.Add(RIGHT.OPERATE_CREATE_SHARED_POINT_GROUPS);
			rights.Add(RIGHT.OPERATE_MODIFY_PUBLIC_POINT_GROUPS);
			rights.Add(RIGHT.OPERATE_MODIFY_SHARED_POINT_GROUPS);
			rights.Add(RIGHT.OPERATE_USE_POINT_CALCULATOR);
			rights.Add(RIGHT.ACCESS_TAG_VIEWER);
			rights.Add(RIGHT.OPERATE_VIEW_ALARM_SUMMARY);
			rights.Add(RIGHT.OPERATE_VIEW_ALARM_HISTORY);
			rights.Add(RIGHT.OPERATE_VIEW_TRENDS);
			//rights.Add(RIGHT.CREATE_TREND);
			rights.Add(RIGHT.OPERATE_MODIFY_TRENDS);
			rights.Add(RIGHT.OPERATE_VIEW_IM_REPORTS);
			rights.Add(RIGHT.SILENCE_ALARMS);
			rights.Add(RIGHT.COPY_POINT_TEMPLATES);
			rights.Add(RIGHT.OPERATE_VIEW_POINT_GROUPS);
			rights.Add(RIGHT.OPERATE_MODIFY_POINT_GROUPS);
			rights.Add(RIGHT.ENABLE_ALARMS_ON_POINT_TEMPLATES);
			rights.Add(RIGHT.DISABLE_ALARMS_ON_POINT_TEMPLATES);
			rights.Add(RIGHT.OPERATE_VIEW_POINTS);
			rights.Add(RIGHT.OPERATE_VIEW_GRAPHICS);
			rights.Add(RIGHT.REMOTE_ENTERPRISE_ENTITY_CONFIGURATION);

			rights.Add(RIGHT.IRS_EXSTARS_MANAGER);
			rights.Add(RIGHT.VIEW_DATA_ANALYTICS);
			rights.Add(RIGHT.VIEW_MAP_CONFIGURATION);
			rights.Add(RIGHT.MODIFY_MAP_CONFIGURATION);
			rights.Add(RIGHT.VIEW_ASSET_TRACKING_DEVICES);
			rights.Add(RIGHT.MODIFY_ASSET_TRACKING_DEVICES);
			rights.Add(RIGHT.VIEW_ICON_CONFIGURATION);
			rights.Add(RIGHT.MODIFY_ICON_CONFIGURATION);
			rights.Add(RIGHT.MAP_INITIATE_INVESTIGATION);
			rights.Add(RIGHT.MAP_COMPLETE_INVESTIGATION);
			rights.Add(RIGHT.IMPORT_TRANSACTION);
			//			rights.Add(RIGHT.ACCESS_SYNC_DASHBOARD);
			rights.Add(RIGHT.ACCESS_ADMIN_DASHBOARD);

			// Add Movement Summary rights
			rights.Add(RIGHT.OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY);
			rights.Add(RIGHT.OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY);
			rights.Add(RIGHT.OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY);
			rights.Add(RIGHT.OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY);
			rights.Add(RIGHT.OPERATE_MODIFY_MOVEMENT_SUMMARY);
			rights.Add(RIGHT.OPERATE_VIEW_MOVEMENT_SUMMARY);

			// Add Movement History rights

			rights.Add(RIGHT.OPERATE_MODIFY_MOVEMENT_HISTORY);
			rights.Add(RIGHT.OPERATE_VIEW_MOVEMENT_HISTORY);

			// Add FCEE rights
			rights.Add(RIGHT.VIEW_FCEE_DATA);
			rights.Add(RIGHT.MODIFY_FCEE_DATA);

			// Add aviation rights
			rights.Add(RIGHT.AVIATION_MODIFY_PUSH_VENDOR_TRANS);
			rights.Add(RIGHT.AVIATION_VIEW_PUSH_VENDOR_TRANS);
			rights.Add(RIGHT.AVIATION_MODIFY_PULL_VENDOR_TRANS);
			rights.Add(RIGHT.AVIATION_VIEW_PULL_VENDOR_TRANS);
			rights.Add(RIGHT.AVIATION_MODIFY_TANK_FARM_VENDOR);
			rights.Add(RIGHT.AVIATION_VIEW_TANK_FARM_VENDOR);

			rights.Add(RIGHT.MOBILE_LAUNCH);
			rights.Add(RIGHT.MOBILE_VIEW_CONFIGURATION);
			rights.Add(RIGHT.MOBILE_MODIFY_CONFIGURATION);
			rights.Add(RIGHT.MOBILE_ROOT_MENU_DISPLAY);

			rights.Add(RIGHT.OPERATE_PERFORM_LEAK_DETECTION);
			rights.Add(RIGHT.ROLLING_STOCK_IMPORT);

			//License expiration warning acknowledgement right
			rights.Add(RIGHT.ALLOW_ACK_FM_LICENSE_EXPIRATION_WARNING);

			rights.Add(RIGHT.CONFIGURE_NOTIFY_ALARMS_ON_POINTS);
			rights.Add(RIGHT.VIEW_OPERATE_STATISTICS);
            rights.Add(RIGHT.OPERATE_VIEW_UNPUBLISHED);
            rights.Add(RIGHT.OPERATE_VIEW_POINT_HISTORY);
         rights.Add(RIGHT.MODIFY_SITE_CLOSEOUT_TIME);
         rights.Add(RIGHT.VIEW_ONLY_SITE_CLOSEOUT_TIME);
         rights.Add(RIGHT.OPERATE_ADMINISTER_POINT_GROUP);
			rights.Add(RIGHT.OPERATE_ADMINISTER_MOVEMENT_SUMMARY);

         return rights;
		}

		/// <summary>
		/// This method purpose is to add a right to the right's array.  It was 
		/// originally used for the "fasapi.cpp" since the managed C++ cannot get the linkage
		/// enumeration, but is now the primary method for accessing the rights array outside
		/// the class.
		/// </summary>
		/// <param name="newRight"></param>
		public void AddRight(RIGHT newRight)
		{
			// Protect the array from being forced beyond it's length.
			if ((int)newRight + 1 > this.RightsArray.Length)
			{
				return;
			}
			this.RightsArray[(int)newRight] = true;
		}

		/// <summary>
		/// Removes the specified right.
		/// </summary>
		/// <param name="removeRight">The right to remove.</param>
		public void RemoveRight(RIGHT removeRight)
		{
			this.RightsArray[(int)removeRight] = false;
		}

		/// <summary>
		/// The has modify transaction right by alias name.
		/// </summary>
		/// <param name="aliasName">
		/// The alias name.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HasModifyTransactionRightByAliasName(string aliasName)
		{
			return this.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && this.ModifyTransactionSecurityRights.ContainsKey(aliasName.ToUpper());
		}

		/// <summary>
		/// The has view transaction right by alias name.
		/// </summary>
		/// <param name="aliasName">
		/// The alias name.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HasViewTransactionRightByAliasName(string aliasName)
		{
			if (this.HasModifyTransactionRightByAliasName(aliasName))
			{
				return true;
			}

			return (this.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) || this.HasRight(RIGHT.VIEW_TRANSACTION_DATA))
					&& this.ViewTransactionSecurityRights.ContainsKey(aliasName.ToUpper());
		}

		/// <summary>
		/// The has modify transaction right by trans type id.
		/// </summary>
		/// <param name="TransTypeID">
		/// The trans type id.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HasModifyTransactionRightByTransTypeID(TransactionTypes TransTypeID)
		{
			return this.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) &&
			this.ModifyTransactionSecurityRights.ContainsValue(TransTypeID);
		}

		/// <summary>
		/// The has view transaction right by trans type id.
		/// </summary>
		/// <param name="transTypeId">
		/// The trans type id.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HasViewTransactionRightByTransTypeID(TransactionTypes transTypeId)
		{
			if (this.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) && this.HasModifyTransactionRightByTransTypeID(transTypeId))
			{
				return true;
			}

			return (this.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) || this.HasRight(RIGHT.VIEW_TRANSACTION_DATA))
					&& this.ViewTransactionSecurityRights.ContainsValue(transTypeId);
		}
	}

	// Modifications on enum RIGHT may require new INSERT/UPDATE on table lookup.tblRights
	// For example, if you add a new right it must be added to lookup.tblRights
	// IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightName = 'MODIFY_CONFIGURATION_SETTINGS')
	// BEGIN
	//	INSERT INTO lookup.tblRight (RightIndex, RightCode, RightName)
	//	VALUES (124, 'MODIFY_CONFIGURATION_SETTINGS', 'MODIFY_CONFIGURATION_SETTINGS')
	// END
	[XmlType(Namespace = "urn:FMSecurityRight")]
	[XmlRoot(Namespace = "urn:FMSecurityRight")]
	public enum RIGHT
	{
		VIEW_USERS = 0,
		VIEW_USER_GROUPS = 1,
		MODIFY_USERS = 2,
		MODIFY_USER_GROUPS = 3,
		IMPORT_CONFIGURATION_DATA = 4,
		EXPORT_CONFIGURATION_DATA = 5,
		PERFORM_PRODUCT_UPDATE = 6,
		VIEW_INSTALLED_MODULES_STATUS = 7,
		VIEW_SITES_AND_SITE_GROUPS = 8,
		MODIFY_SITES_AND_SITE_GROUPS = 9,
		VIEW_COMPANY_DATA = 10,
		MODIFY_COMPANY_DATA = 11,
		VIEW_PRODUCTS = 12,
		MODIFY_PRODUCTS = 13,
		VIEW_ALLOCATIONS = 14,
		MODIFY_ALLOCATIONS = 15,
		VIEW_EQUIPMENT_DATA = 16,
		MODIFY_EQUIPMENT_DATA = 17,
		VIEW_PERSONNEL_DATA = 18,
		MODIFY_PERSONNEL_DATA = 19,
		VIEW_TRANSACTION_DATA = 20,
		MODIFY_TRANSACTION_DATA = 21,
		VIEW_TRANSACTION_ALIASES = 22,
		MODIFY_TRANSACTION_ALIASES = 23,
		PERFORM_CLOSEOUT = 24,
		CONFIGURE_ACCOUNTING = 25,
		VIEW_LOAD_RACK_DATA = 26,
		MODIFY_LOAD_RACK_DATA = 27,
		VIEW_INVENTORY_DATA = 28,
		VIEW_REPORTS = 29,
		MODIFY_REPORTS = 30,
		CONFIGURE_IMPORT_EXPORT = 31,
		EXECUTE_IMPORT_EXPORT = 32,
		VIEW_TICKETING_DATA = 33,
		MODIFY_TICKETING_DATA = 34,
		MODIFY_ORDERS = 35,
		VIEW_ORDERS = 36,
		CREATE_ORDERS = 37,
		VIEW_QUERIES = 38,
		MODIFY_QUERIES = 39,
		MODIFY_SYSTEM_SETTINGS = 40,
		PERFORM_REVERSE_TRANSACTION = 41,
		VIEW_STANDING_OFFERS = 42,
		MODIFY_STANDING_OFFERS = 43,
		VIEW_GRAPHICS = 44,
		VIEW_PIDX_PROFILES = 45,      //vthompson CSI 5773
		MODIFY_PIDX_PROFILES = 46,
		ENABLEDISABLE_STATIONS = 47,
		CONFIGURE_TRAINING = 48,
		CONFIGURE_QUALIFICATIONS = 49,
		CONFIGURE_LICENSES = 50,
		MODIFY_FUEL_CARD_DATA = 51,
		VIEW_FUEL_CARD_DATA = 52,
		MODIFY_FINANCIAL_DATA = 53,
		VIEW_FINANCIAL_DATA = 54,
		PRIVILEGED_FINANCIAL = 55,
		INTERFACE_IMPORT = 56,
		BACKUP_DATABASE = 57,
		//		MODIFY_PAYMENT_DATA 				=  58,
		//		VIEW_RECOVERY_DATA 				=  59,
		//		MODIFY_RECOVERY_DATA 			=  60,
		VIEW_SUPPLY_ORDERS = 61,
		CREATE_SUPPLY_ORDERS = 62,
		MODIFY_SUPPLY_ORDERS = 63,
		TOGGLE_DATA_DICTIONARY = 64,
		//		CREATE_ADJUSTMENT 				=  65,
		//		MODIFY_ADJUSTMENT 				=  66,
		VIEW_AUDIT_LOGS = 67,
		VIEW_ALARM_EVENT_LOGS = 68,
		VIEW_CLOSEOUT_DATA = 69,
		CONFIGURE_QUERIES = 70,
		CONFIGURE_RESERVE_LEVEL = 71,
		UNDELETE_TRANSACTION_DATA = 72,
		VIEW_BILLS_OF_LADING = 73,
		EXECUTE_QUALITY_TESTS = 74,
		MODIFY_QUALITY_TESTS = 75,
		VIEW_QUALITY_TESTS = 76,
		MODIFY_TEST_ITEMS = 77,
		VIEW_TEST_ITEMS = 78,
		ADD_MAINTENANCE_RECORD = 79,
		VIEW_MAINTENANCE_RECORD = 80,
		UNDEFINED_81 = 81,   //Place holder. Required for serialization to work.
		VIEW_TRAINING_QUALIFICATIONS = 82,
		MODIFY_PERSON_QUALIFICATIONS = 83,
		MODIFY_PERSON_TRAINING = 84,
		UNDEFINED_85 = 85,   //Place holder. Required for serialization to work.
		VIEW_TRAINING_QUAL_HISTORY = 86,
		MODIFY_TRAINING_QUAL_HISTORY = 87,
		MODIFY_APPOINTMENTS = 88,
		VIEW_APPOINTMENTS = 89,
		VIEW_DISPATCH = 90,
		ADD_QUALITYTAG_RECORD = 91,
		VIEW_QUALITYTAG_RECORD = 92,
		VIEW_QUALITYTAG_LOGS = 93,
		VIEW_DATABASE_AUDIT_LOG = 94,
		MODIFY_DATABASE_AUDIT_LOG = 95,
		MODIFY_MAINTENANCE_RECORD = 96,
		UNDEFINED_97 = 97,   //Place holder. Required for serialization to work.
		MODIFY_QUALITYTAG_RECORD = 98,
		MODIFY_QUALITYTAG_LOGS = 99,
		MODIFY_DISPATCH = 100,
		VIEW_WAC_HISTORY = 101,
		OVERRIDE_WAC = 102,
		MODIFY_INVOICE_QUERIES = 103,
		ACCESS_MFCS = 104,
		BASE_EXPORT = 105,
		ENTERPRISE_EXPORT = 106,
		ACCESS_ARTS = 107,
		IMPORT_ENTITIES = 108,
		EXPORT_ENTITIES = 109,
		IMPORT_ENTERPRISE_DATA = 110,
		EXPORT_ENTERPRISE_DATA = 111,
		ALLOW_SINGLE_SITE_GROUP_SELECT = 112,     // To allow site combo to be visible on single site system
		CONFIGURE_FOOTNOTES = 113,
		EOM_APPROVAL_ACCOUNTABLE = 114,
		EOM_APPROVAL_APPROVING = 115,
		VIEW_INCOMING_TRUCK_DATA = 116,
		MODIFY_INCOMING_TRUCK_DATA = 117,
		ACCESS_ACCOUNTING_OPERATIONS = 118,
		ACCESS_ACCOUNTING_LEDGER = 119,
		ACCESS_ONLINE_HELP = 120,
		ACCESS_ONLINE_TUTORIALS = 121,
		ACCESS_ONLINE_ADMIN_MANUAL = 122,
		ACCESS_ONLINE_ADMIN_TUTORIAL = 123,
		BASE_EXPORT_MANUAL = 124,
		RAPS_IMPORT = 125,
		MODIFY_ERROR_TRANSACTION = 126,
		SEND_TO_EBS = 127,
		VIEW_TANK_DATA = 128,
		MODIFY_TANK_DATA = 129,
		EXPORT_INFLIGHT_TRANSACTIONS = 130,
		MODIFY_SUSPENDED_TRANSACTIONS = 131,
		MODIFY_CONFIGURATION_SETTINGS = 132,
		VIEW_AUTO_DISTRIBUTION_CONFIGURATION = 133,
		MODIFY_AUTO_DISTRIBUTION_CONFIGURATION = 134,
		PERFORM_AUTO_DISTRIBUTION = 135,
		MODIFY_METERS = 136,
		VIEW_METERS = 137,
		VIEW_METER_RECONCILIATION = 138,
		VIEW_SERVICE_REQUEST_MESSAGING_CONFIGURATION = 139,
		MODIFY_SERVICE_REQUEST_MESSAGING_CONFIGURATION = 140,
		VIEW_MOBILE_DEVICE_PROFILES = 141,
		MODIFY_MOBILE_DEVICE_PROFILES = 142,
		ACCESS_COOGEE_TANK_ADJ = 143,
		VIEW_ENTITY_ASSIGNMENTS = 144,
		MODIFY_ENTITY_ASSIGNMENTS = 145,
		VIEW_FIELD_LEVEL_CONTROL_CONFIGURATION = 146,
		MODIFY_FIELD_LEVEL_CONTROL_CONFIGURATION = 147,
		VIEW_MOBILE_DEVICES = 148,
		MODIFY_MOBILE_DEVICES = 149,
		LOGIN_FROM_MOBILE_DEVICE = 150,
		CONFIGURE_DISPATCH_VALIDATIONS = 151,

		// The following are created in 8.0.1 for ADF.
		// These are place holders so data migration would be easier later.
		// Please don't renumber them
		VIEW_PRODUCT_SITE_LIMITS = 152,
		MODIFY_PRODUCT_SITE_LIMITS = 153,
		VIEW_PRICE_THRESHOLDS = 154,
		MODIFY_PRICE_THRESHOLDS = 155,
		VIEW_JOB_QUEUE = 156,
		MODIFY_JOB_QUEUE = 157,
		VIEW_ARCHIVING = 158,
		MODIFY_ARCHIVING = 159,
		VIEW_ALL_ARCHIVING = 160,
		MODIFY_ALL_ARCHIVING = 161,
		PERFORM_FORMAT_CONFIGURATION = 162,
		// End of ADF enums

		VIEW_SYNC_CONFIG_CLIENT_SETTINGS = 163,
		MODIFY_SYNC_CONFIG_CLIENT_SETTINGS = 164,
		VIEW_SYNC_CONFIG_SERVER_SETTINGS = 165,
		MODIFY_SYNC_CONFIG_SERVER_SETTINGS = 166,
		VIEW_SYNC_CONFIG_SITE_SETTINGS = 167,
		MODIFY_SYNC_CONFIG_SITE_SETTINGS = 168,

		MODIFY_SYNC_CONFLICT_STATUS = 169,
		PERFORM_SYNCHRONIZATION = 170,
		VIEW_SYNC_CONFLICT_STATUS = 171,

		MIGRATION_PERFORM_IMPORT_EXPORT = 172,

		CONFIGURE_AVIATION_EXPORT = 173,
		VIEW_FUEL_CARD_LIMIT = 174,
		MODIFY_FUEL_CARD_LIMIT = 175,

		VIEW_INVENTORY_RECONCILIATION = 176,
		IMPORT_NATO_FUELCARD = 177,

		CREATE_IRS_EXSTARS_REPORT = 178,
		VIEW_IRS_EXSTARS_REPORT = 179,

		IRS_EXSTARS_MANAGER = 180,
		VIEW_AUTOMATED_FUEL_SERVICE_STATION = 181,
		MODIFY_AUTOMATED_FUEL_SERVICE_STATION = 182,

		// BSME added rights (Were: 132 - 135)
		MODIFY_UNOBTAINABLE = 183,
		CONFIGURE_LOCATIONS = 184,
		VIEW_MOVEMENT = 185,
		CONFIGURE_WEB_LINKS = 186,
		CONFIGURE_DLA_TEST = 187,

		ACCESS_SYNC_DASHBOARD = 188,
		ACCESS_ADMIN_DASHBOARD = 189,
		IMPORT_IM_TANK_DATA = 190,
		ACCESS_RECONCILIATION_VIEWS = 191,
		ACCESS_TRANSACTION_DETAIL_CLIN = 192,
		PRODUCT_AUTHORIZATION_AND_CONTROL = 193,
		PRODUCT_AUTHORIZATION_OVERRIDE = 194,
		VIEW_DATA_ANALYTICS = 195,

		// Map rights
		VIEW_MAPS = 196,
		VIEW_MAP_CONFIGURATION = 197,
		MODIFY_MAP_CONFIGURATION = 198,
		VIEW_ASSET_TRACKING_DEVICES = 199,
		MODIFY_ASSET_TRACKING_DEVICES = 200,
		VIEW_ICON_CONFIGURATION = 201,
		MODIFY_ICON_CONFIGURATION = 202,
		MAP_INITIATE_INVESTIGATION = 203,
		MAP_COMPLETE_INVESTIGATION = 204,
		//This right will allow users to edit and upload transactions on pages 
		//like intoPlane and standard transaction import interface
		IMPORT_TRANSACTION = 205,

		MODIFY_EPOS_MANUAL_TRANSACTION = 206,
		IMPORT_EPOS_TRANSACTION_FILE = 207,




		// FM10 IM rights
		VIEW_POINT_TEMPLATES = 300,
		MODIFY_POINT_TEMPLATES = 301,

		ACKNOWLEDGE_ALL_ALARMS = 302,
		ACKNOWLEDGE_WITH_COMMENTS = 303,
		ACCESS_DRAW = 304,
		VIEW_PICTURE_SUMMARY = 305,
		MODIFY_PICTURE_SUMMARY = 306,
		VIEW_POINT_CATEGORIES = 307,
		MODIFY_POINT_CATEGORIES = 308,
		VIEW_POINT_ACCESS_GROUP = 309,
		MODIFY_POINT_ACCESS_GROUP = 310,
		VIEW_POINT_TYPES = 311,
		MODIFY_POINT_TYPES = 312,
		VIEW_MODULE_LIBRARY = 313,
		MODIFY_MODULE_LIBRARY = 314,
		VIEW_POINTS = 315,
		MODIFY_POINTS = 316,
		ENABLE_POINTS = 317,
		DISABLE_POINTS = 318,
		ENABLE_ALARMS_ON_POINTS = 319,
		DISABLE_ALARMS_ON_POINTS = 320,
		VIEW_POINT_COMMANDSTATUS_LIST = 321,
		MODIFY_POINT_COMMANDSTATUS_LIST = 322,
		VIEW_OPERATE_ONLY = 323,
		OPERATE_CREATE_PUBLIC_POINT_GROUPS = 324,
		OPERATE_CREATE_SHARED_POINT_GROUPS = 325,
		OPERATE_MODIFY_PUBLIC_POINT_GROUPS = 326,
		OPERATE_MODIFY_SHARED_POINT_GROUPS = 327,
		OPERATE_USE_POINT_CALCULATOR = 328,
		ACCESS_TAG_VIEWER = 329,
		OPERATE_VIEW_ALARM_SUMMARY = 330,
		OPERATE_VIEW_ALARM_HISTORY = 331,
		OPERATE_VIEW_TRENDS = 332,
		OPERATE_MODIFY_TRENDS = 333,
		OPERATE_VIEW_IM_REPORTS = 334,
		SILENCE_ALARMS = 335,
		COPY_POINT_TEMPLATES = 336,
		OPERATE_VIEW_POINT_GROUPS = 337,
		OPERATE_MODIFY_POINT_GROUPS = 338,
		ENABLE_ALARMS_ON_POINT_TEMPLATES = 339,
		DISABLE_ALARMS_ON_POINT_TEMPLATES = 340,
		OPERATE_VIEW_POINTS = 341,
		OPERATE_VIEW_GRAPHICS = 342,

		REMOTE_ENTERPRISE_ENTITY_CONFIGURATION = 343,

		// Movement Summary permissions
		OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY = 344,
		OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY = 345,
		OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY = 346,
		OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY = 347,
		OPERATE_MODIFY_MOVEMENT_SUMMARY = 348,
		OPERATE_VIEW_MOVEMENT_SUMMARY = 349,

		// FCEE permissions
		VIEW_FCEE_DATA = 350,
		MODIFY_FCEE_DATA = 351,

		// Aviation permissions
		AVIATION_MODIFY_PUSH_VENDOR_TRANS = 352,
		AVIATION_VIEW_PUSH_VENDOR_TRANS = 353,
		AVIATION_MODIFY_PULL_VENDOR_TRANS = 354,
		AVIATION_VIEW_PULL_VENDOR_TRANS = 355,
		AVIATION_MODIFY_TANK_FARM_VENDOR = 356,
		AVIATION_VIEW_TANK_FARM_VENDOR = 357,

		// Mobile menu
		MOBILE_LAUNCH = 361,
		MOBILE_VIEW_CONFIGURATION = 362,
		MOBILE_MODIFY_CONFIGURATION = 363,
		MOBILE_ROOT_MENU_DISPLAY = 364,

		OPERATE_PERFORM_LEAK_DETECTION = 365,

		// Movement History permissions
		OPERATE_MODIFY_MOVEMENT_HISTORY = 366,
		OPERATE_VIEW_MOVEMENT_HISTORY = 367,

		//	Rolling Stock Import
		ROLLING_STOCK_IMPORT = 368,

		//Right to acknowledge License
		ALLOW_ACK_FM_LICENSE_EXPIRATION_WARNING = 369,


		CONFIGURE_NOTIFY_ALARMS_ON_POINTS = 370,

		VIEW_OPERATE_STATISTICS = 371,

		OPERATE_VIEW_UNPUBLISHED = 372,

        OPERATE_VIEW_POINT_HISTORY = 373,

      MODIFY_SITE_CLOSEOUT_TIME = 374,
      VIEW_ONLY_SITE_CLOSEOUT_TIME = 375,

		OPERATE_ADMINISTER_POINT_GROUP = 376,
		OPERATE_ADMINISTER_MOVEMENT_SUMMARY = 377
      // Modifications on enum RIGHT may require new INSERT/UPDATE on table lookup.tblRights
      // For example, if you add a new right it must be added to lookup.tblRights
      // IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightName = 'MODIFY_CONFIGURATION_SETTINGS')
      // BEGIN
      //	INSERT INTO lookup.tblRight (RightIndex, RightCode, RightName)
      //	VALUES (124, 'MODIFY_CONFIGURATION_SETTINGS', 'MODIFY_CONFIGURATION_SETTINGS')
      // END
   }
}