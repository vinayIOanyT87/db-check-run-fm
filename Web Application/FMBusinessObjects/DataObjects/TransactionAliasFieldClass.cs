namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    #region Public enumeration
	public enum TransactionFieldType //: byte
	{
		Transaction = 1,
		LineItem = 2,
		WeightReading = 3,
		Note = 4,
		TransportInfo = 5,
		ExportResult = 6,
		TransactionFieldTypeMax = 7
	}

	public enum TransactionFieldVisibility 
	{
		Always = 0,
		GroundProducts = 1,
		AviationProducts = 2,
	}

	#endregion

	#region Transaction Alias Field Collection Class
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(TransactionAliasFieldClass))]
	public class TransactionAliasFieldCollectionClass : FieldCollectionClass
	{
		public TransactionAliasFieldClass Find(string dbName)
		{
			foreach (TransactionAliasFieldClass transactionAliasField in this.List)
			{
				if (dbName == transactionAliasField.DbName)
				{
					return transactionAliasField;
				}
			}

			return null;
		}

		public TransactionAliasFieldClass this[int index]
		{
			get
			{
				return (TransactionAliasFieldClass)this.List[index];
			}
			set
			{
			    this.List[index] = value;
			}
		}

	}
	#endregion

	/// <summary>
	/// Summary description for TransactionAliasFieldClass.
	/// </summary>
	[DebuggerDisplay("TransactionAliasFieldClass {ID},DisplayName={_DisplayName}")]
	[Serializable]
	[DataContract]
	public class TransactionAliasFieldClass : FieldClass
	{
		#region Protected data members
		[DataMember]
		private bool defaultAssigned;

		static readonly string[] TransactionVirtualFields = {
					        //"TotalPriceAmount", TODO: Temporary commented out so that QA does not test financial configuration features.
					        //"TotalPriceWithTax", TODO: Temporary commented out so that QA does not test financial configuration features.
					        //"TotalExcise", TODO: Temporary commented out so that QA does not test financial configuration features.
					        //"TotalGST", TODO: Temporary commented out so that QA does not test financial configuration features.
					        //"TotalMarkup", TODO: Temporary commented out so that QA does not test financial configuration features.
					        "TotalGrossQuantity",
					        "TotalNetQuantity",
					        "TotalMassQuantity",
					        "VolumeUnit",
					        "LevelUnit",
					        "TemperatureUnit",
					        "DensityUnit",
					        "MassUnit",
					        "FlowUnit",
					        "PressureUnit",
					        "ResponseTime",
					        "FuelTime",
					        AutoDistributionReasonCodeClass.TransactionFieldID,
							"ProcessingSite"
				        };

		static readonly string[] LineItemVirtualFields = {
					        "GrossQuantityReceived",
					        "GrossQuantityRemaining",
					        "NetQuantityReceived",
					        "NetQuantityRemaining",
					        "MassQuantityReceived",
					        "MassQuantityRemaining",
					        "SpecialInstructions",
					        //"TotalValue", TODO: Temporarily commented out so that QA does not test financial configuration features.
					        "ValueRemaining",
					        //"TotalPriceWithTax", TODO: Temporary commented out so that QA does not test financial configuration features.
					        "MeterTotal",
					        "VolumeUnit",
					        "LevelUnit",
					        "TemperatureUnit",
					        "DensityUnit",
					        "MassUnit",
					        "FlowUnit",
					        "PressureUnit",
					        "MassPackageSize",
					        "VolumePackageSize",
					        "PackageQuantity"
				        };
		#endregion

		#region Constructors
		public TransactionAliasFieldClass()
		{
			this.Reset();
		}

		public TransactionAliasFieldClass(TransactionFieldType type, int displayOrder, string dbName, string displayName)
		{
			this.Reset();
			this.Type = type;
			this.DisplayOrder = displayOrder;
			this.DbName = dbName;
			this.DisplayName = displayName;
		}

		public TransactionAliasFieldClass(
			TransactionFieldType type,
			int displayOrder,
			string dbName,
			string displayName,
			bool defaultAssigned)
		{
			this.Reset();
			this.Type = type;
			this.DisplayOrder = displayOrder;
			this.DbName = dbName;
			this.DisplayName = displayName;
			this.DefaultAssigned = defaultAssigned;
		}

		public TransactionAliasFieldClass(
			TransactionFieldType type,
			int displayOrder,
			string dbName,
			string displayName,
			bool defaultAssigned,
			bool isVirtualField)
		{
			this.Reset();
			this.Type = type;
			this.DisplayOrder = displayOrder;
			this.DbName = dbName;
			this.DisplayName = displayName;
			this.DefaultAssigned = defaultAssigned;
			this.VirtualField = isVirtualField;
		}

		#endregion

		#region Properties
		string SelectClause => "SELECT tblTransactionAliasFields.*," +
		                       "tblTransactionAliases.AliasName," +
		                       "(Select GroupID from tblGroups WHERE tblGroups.GroupGuid = tblTransactionAliasFields.UserGroupGuid) AS UserGroupID ";

	    public override string ID
		{
			get
			{
				return this._DbName;
			}
			set
			{
				this._DbName = value;
			}
		}

        [DataMember]
		public TransactionFieldType Type { get; set; }

		public bool DefaultAssigned
		{
			get
			{
				return this.defaultAssigned;
			}
			set
			{
				this.defaultAssigned = value;
			}
		}

		[XmlIgnore]
		public override ENTITY_TYPE EntityType => ENTITY_TYPE.TRANSACTION_ALIAS_FIELD;

	    [XmlIgnore]
		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

	    public bool IsFinancialField
		{
			get
			{
				if (this.Type == TransactionFieldType.Transaction)
				{
					if (this.ID == "TotalExcise"
					 || this.ID == "TotalGST"
					 || this.ID == "TotalMarkup"
					 || this.ID == "TotalPriceAmount"
					 || this.ID == "TotalPriceWithTax"
						)
					{
						return true;
					}
				}
				else if (this.Type == TransactionFieldType.LineItem)
				{
					if (this.ID == "CurrencyGuid"
					 || this.ID == "ExchangeRate"
					 || this.ID == "NonDomesticPrice"
					 || this.ID == "ProductPrice"
					 || this.ID == "Tax1"
					 || this.ID == "Tax2"
					 || this.ID == "Tax3"
					 || this.ID == "TotalPriceWithTax"
					 || this.ID == "TotalValue"
						)
					{
						return true;
					}
				}

				return false;
			}
		}
		#endregion

		#region Methods

		public static bool IsVirtual(string field, TransactionFieldType type)
		{
			var virtualFields = VirtualFields(type);

			foreach (var virtualField in virtualFields)
			{
				if (field.Equals(virtualField))
				{
					return true;
				}
			}

			return false;

		}

		public static string[] VirtualFields(TransactionFieldType type)
		{
			if (type == TransactionFieldType.Transaction)
			{
				return TransactionVirtualFields;
			}

			if (type == TransactionFieldType.LineItem)
			{
				return LineItemVirtualFields;
			}

			return new string[0];
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTransactionAliasFields " +
					"(TransactionAliasGuid," +
					"LookupTransactionFieldTypeIndex," +
					"DbName," +
					"DisplayOrder," +
					"DisplayName," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy, " +
					"Required, " +
					"Virtual," +
					"DispatchField," +
					"ClearOnNew," +
					"ReadOnly," +
					"Visibility," +
					"DefaultValueType," +
					"DefaultValue," +
					"UserGroupGuid," +
					"TransactionAliasFieldGuid) " +
					"VALUES (" +
					"@TransactionAliasGuid," +
					"@LookupTransactionFieldTypeIndex," +
					"@DbName," +
					"@DisplayOrder," +
					"@DisplayName," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy, " +
					"@Required, " +
					"@Virtual," +
					"@DispatchField," +
					"@ClearOnNew," +
					"@ReadOnly," +
					"@Visibility," +
					"@DefaultValueType," +
					"@DefaultValue," +
					"@UserGroupGuid," +
					"@TransactionAliasFieldGuid)";

			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.TransactionAliasGuid);
			cmd.Parameters.AddWithValue("@LookupTransactionFieldTypeIndex", (int) this.Type);
			cmd.Parameters.AddWithValue("@DbName", this.DbName);
			cmd.Parameters.AddWithValue("@DisplayOrder", this.DisplayOrder);
			cmd.Parameters.AddWithValue("@DisplayName", this.DisplayName);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@Required", this.FieldRequired);
			cmd.Parameters.AddWithValue("@Virtual", this.VirtualField);
			cmd.Parameters.AddWithValue("@DispatchField", this.DispatchField);
			cmd.Parameters.AddWithValue("@ClearOnNew", this.ClearOnNew);
			cmd.Parameters.AddWithValue("@ReadOnly", this.ReadOnly);
			cmd.Parameters.AddWithValue("@Visibility", (int)this.Visibility);
			if (this.DefaultValueXml == null) {
				cmd.Parameters.AddWithValue("@DefaultValueType", DBNull.Value);
				cmd.Parameters.Add("@DefaultValue", SqlDbType.Xml);
				cmd.Parameters["@DefaultValue"].Value = DBNull.Value;
            } else {
				cmd.Parameters.AddWithValue("@DefaultValueType", this.DefaultValueTypeString);
				cmd.Parameters.Add("@DefaultValue", SqlDbType.Xml);
				cmd.Parameters["@DefaultValue"].Value = this.DefaultValueXml;
			}


			if (this.UserGroupGuid == Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@UserGroupGuid", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@UserGroupGuid", this.UserGroupGuid);
			}

			cmd.Parameters.AddWithValue("@TransactionAliasFieldGuid", this._IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTransactionAliasFields " +
						"SET DisplayOrder = @DisplayOrder," +
						"DBName = @DBName," +
						"DisplayName = @DisplayName," +
						"UpdatedDate = @UpdatedDate," +
						"UpdatedBy = @UpdatedBy," +
						"Required = @Required," +
						"Virtual = @Virtual," +
						"DispatchField = @DispatchField," +
						"ClearOnNew = @ClearOnNew," +
						"UserGroupGuid = @UserGroupGuid," +
						"ReadOnly = @ReadOnly," +
						"Visibility = @Visibility," +
						"DefaultValueType = @DefaultValueType," +
						"DefaultValue = @DefaultValue" +
						" WHERE TransactionAliasFieldGuid = @TransactionAliasFieldGuid";

			cmd.Parameters.AddWithValue("@DisplayOrder", this.DisplayOrder);
			cmd.Parameters.AddWithValue("@DbName", this.DbName);
			cmd.Parameters.AddWithValue("@DisplayName", this.DisplayName);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@Required", this.FieldRequired);
			cmd.Parameters.AddWithValue("@Virtual", this.VirtualField);
			cmd.Parameters.AddWithValue("@DispatchField", this.DispatchField);
			cmd.Parameters.AddWithValue("@ClearOnNew", this.ClearOnNew);
			cmd.Parameters.AddWithValue("@ReadOnly", this.ReadOnly);
			cmd.Parameters.AddWithValue("@Visibility", (int)this.Visibility);
			if (this.DefaultValueXml == null)
			{
				cmd.Parameters.AddWithValue("@DefaultValueType", DBNull.Value);
				cmd.Parameters.Add("@DefaultValue", SqlDbType.Xml);
				cmd.Parameters["@DefaultValue"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters.AddWithValue("@DefaultValueType", this.DefaultValueTypeString);
				cmd.Parameters.Add("@DefaultValue", SqlDbType.Xml);
				cmd.Parameters["@DefaultValue"].Value = this.DefaultValueXml;
			}

			if (this.UserGroupGuid == Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@UserGroupGuid", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@UserGroupGuid", this.UserGroupGuid);
			}

			cmd.Parameters.AddWithValue("@TransactionAliasFieldGuid", this._IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTransactionAliasFields" +
					" WHERE TransactionAliasFieldGuid = @TransactionAliasFieldGuid";

			cmd.Parameters.AddWithValue("@TransactionAliasFieldGuid", this._IdentityGuid);
		}
		#endregion

		#region Public methods
		public static string TransactionFieldTypeID(TransactionFieldType type)
		{
			switch (type)
			{
				case TransactionFieldType.Transaction:
					return "Transaction";
				case TransactionFieldType.LineItem:
					return "Line Item";
				case TransactionFieldType.WeightReading:
					return "Weight Reading";
				case TransactionFieldType.Note:
					return "Note";
				case TransactionFieldType.TransportInfo:
					return "Transport Line Item";
				case TransactionFieldType.ExportResult:
					return "Export Result";
				default:
					return "Undefined";
			}
		}

		public override sealed void Reset()
		{
			base.Reset();
			this.Type = TransactionFieldType.TransactionFieldTypeMax;
			this.DefaultAssigned = false;
		}

		public void Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			this.Reset();

			DataTable table = set.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			this._IdentityGuid			= DataObject.getValue<Guid>(row["TransactionAliasFieldGuid"], Guid.Empty);
			this.TransactionAliasGuid	= DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
			this.Type					= DataObject.getValue<TransactionFieldType>(row["LookupTransactionFieldTypeIndex"], TransactionFieldType.TransactionFieldTypeMax);
			this.DbName					= DataObject.getValue<string>(row["DbName"], string.Empty);
			this.DisplayOrder			= DataObject.getValue<int>(row["DisplayOrder"], 0);
			this.DisplayName			= DataObject.getValue<string>(row["DisplayName"], string.Empty);
			this._CreatedDate			= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy				= DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this._UpdatedDate			= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy				= DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			this.AliasName				= DataObject.getValue<string>(row["AliasName"], string.Empty);
			this.FieldRequired			= DataObject.getValue<bool>(row["Required"], false);
			this.VirtualField			= DataObject.getValue<bool>(row["Virtual"], false);
			this.DispatchField			= DataObject.getValue<bool>(row["DispatchField"], false);
			this.ClearOnNew				= DataObject.getValue<bool>(row["ClearOnNew"], false);
			this.UserGroupGuid			= DataObject.getValue<Guid>(row["UserGroupGuid"], Guid.Empty);
			this.UserGroupID			= DataObject.getValue<string>(row["UserGroupID"], string.Empty);
			this.ReadOnly				= DataObject.getValue<bool>(row["ReadOnly"], false);
			this.Visibility				= DataObject.getValue<TransactionFieldVisibility>(row["Visibility"], TransactionFieldVisibility.Always);
			this.DefaultValueTypeString	= DataObject.getValue<string>(row["DefaultValueType"], "System.String");
			this.DefaultValueXml		= DataObject.getValue<string>(row["DefaultValue"], string.Empty);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblTransactionAliasFields, tblTransactionAliases " + SQLUpdateLock(bInTransaction) +
				" WHERE tblTransactionAliasFields.TransactionAliasGuid = tblTransactionAliases.TransactionAliasGuid" +
				" AND tblTransactionAliasFields.[TransactionAliasFieldGuid] = @TransactionAliasFieldGuid";

			cmd.Parameters.AddWithValue("@TransactionAliasFieldGuid", this._IdentityGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, bool byUser, bool bInTransaction)
		{
			string byUserWhereClause = string.Empty;

			if (byUser)
			{
				byUserWhereClause =
					" AND (UserGroupGuid IS NULL OR UserGroupGuid IN (SELECT GroupGuid FROM map.tblUserToGroup WHERE UserGuid = @UserGuid AND SiteGuid = @SiteGuid))";
			}

			cmd.CommandText = this.SelectClause +
				" FROM tblTransactionAliasFields, tblTransactionAliases " + SQLUpdateLock(bInTransaction) +
				" WHERE tblTransactionAliasFields.TransactionAliasGuid = tblTransactionAliases.TransactionAliasGuid" +
					" AND tblTransactionAliasFields.TransactionAliasGuid = @TransactionAliasGuid " +
					" AND LookupTransactionFieldTypeIndex = @LookupTransactionFieldTypeIndex " +
					" AND tblTransactionAliasFields.DispatchField = @DispatchField" +
					byUserWhereClause +
				" ORDER BY DbName";

			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.TransactionAliasGuid);
			cmd.Parameters.AddWithValue("@LookupTransactionFieldTypeIndex", (int) this.Type);
			cmd.Parameters.AddWithValue("@DispatchField", this.DispatchField);

			if (byUser)
			{
				cmd.Parameters.AddWithValue("@UserGuid", security.UserGuid);
				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			}
		}

		/// <summary>
		/// The enumerate by alias ID SQL.
		/// </summary>
		/// <param name="command">The SQL command to be populated.</param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="byUser">
		/// The by user.
		/// </param>
		/// <param name="inTransaction">
		/// The transaction.
		/// </param>
		public void EnumerateByAliasIdSql(SqlCommand command, SecurityClass security, bool byUser, bool inTransaction)
		{
			string byUserWhereClause = string.Empty;

			if (byUser)
			{
				byUserWhereClause = 
					" AND (UserGroupGuid IS NULL OR UserGroupGuid IN (SELECT GroupGuid FROM map.tblUserToGroup WHERE UserGuid = @UserGuid AND SiteGuid = @SiteGuid))";

			}

			command.CommandText = this.SelectClause +
						 " FROM tblTransactionAliasFields, tblTransactionAliases " + SQLUpdateLock(inTransaction) +
						 " WHERE tblTransactionAliasFields.TransactionAliasGuid = tblTransactionAliases.TransactionAliasGuid" +
						 " AND tblTransactionAliasFields.TransactionAliasGuid = @TransactionAliasGuid " +
						 byUserWhereClause +
						 " ORDER BY DbName";

			command.Parameters.AddWithValue("@TransactionAliasGuid", this.TransactionAliasGuid);

			if (byUser)
			{
				command.Parameters.AddWithValue("@UserGuid", security.UserGuid);
				command.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			}
		}

		public void EnumerateFieldsSQL(SqlCommand cmd, TransactionTypes transType)
		{
			string table;

			switch (this.Type)
			{
				case TransactionFieldType.Transaction:
					table = "tblTransactions";
					break;
				case TransactionFieldType.LineItem:
					table = "tblTransactionLineItems";
					break;
				case TransactionFieldType.WeightReading:
					table = "tblTransactionWeightReadings";
					break;
				case TransactionFieldType.Note:
					table = "tblTransactionNotes";
					break;
				case TransactionFieldType.TransportInfo:
					table = "tblTransactionTransportLineItems";
					break;
				case TransactionFieldType.ExportResult:
                    table = "tblExportResultDetails";
                    break;
				default:
					throw new Exception("Invalid Transaction Field Type");
			}

			string sql = 
@"declare @tableid int 
select @tableid = object_id('dbo.' + @Table)
SELECT name FROM syscolumns
WHERE id = @tableid
AND object_schema_name(id) = 'dbo'
AND name <> 'SubType'
AND name <> 'TransVersion'
AND name <> 'TransLineItemID'
AND name <> 'TransactionLineItemGuid'
AND name <> 'TransSubLineItemID'
AND name <> 'TransactionSubLineItemGuid'
AND name <> 'SequenceID'
AND name <> 'TransactionVersion'
AND name <> 'PartialCloseout'
AND name <> 'ShipToCode'
AND name <> 'SupplierCode'
AND name <> 'ShipperCode'
AND name <> 'OwnerCode'
AND name <> 'ManagerCode'
AND name <> 'CarrierCode'
AND name <> 'BillToCode'
AND name <> 'TransactionInventoryDate'
AND name <> 'OrderLineReferenceID'
AND name <> 'OrderReferenceTransactionLineItemGuid'
AND name <> '_RowVersion'
AND name <> '_ClusterIdx'";

			// Exclude all fields except Error, Interface01 - Interface08 from the
			// list of configurable fields.
			if (this.Type == TransactionFieldType.ExportResult)
			{
				const string FieldExclusion = " AND name <> 'Index'" +
				                              " AND name <> 'ExportResultIndex'" +
				                              " AND name <> 'RecordID'" +
				                              " AND name <> 'Fail'" +
				                              " AND name <> 'TransVersion'" +
				                              " AND name <> 'CreatedBy'" +
				                              " AND name <> 'UpdatedBy'" +
				                              " AND name <> 'CreatedDate'" +
				                              " AND name <> 'UpdatedDate'";
				sql = sql + FieldExclusion;
			}

			if (transType == TransactionTypes.T11_ConsumerTransfer)
			{
				sql += " AND name <> 'BillToID'" +
								" AND name <> 'ShipToID'";
			}
			else if (transType == TransactionTypes.T13_OwnerTransfer)
			{
				sql += " AND name <> 'ManagerID'" +
								" AND name <> 'OwnerID'" +
								" AND name <> 'CarrierID'";
			}
			else if (transType == TransactionTypes.T15_PrimaryRegrade || transType == TransactionTypes.T16_SecondaryRegrade)
			{
				sql += " AND name <> 'Product'";
			}
			else if (transType == TransactionTypes.T23_StorageTransfer)
			{
				sql += " AND name <> 'StorageLocationID'";
			}

			sql += " AND (object_name(id) <> 'tblTransactionLineItems' OR (name NOT LIKE('%SerialNumber%')" +
					" AND name NOT LIKE('%EquipmentType%')" +
					" AND name NOT LIKE('%EquipmentModel%')))" +
					" AND name NOT LIKE('%CompanyEquipmentID%')" +
					" AND name <> 'ProductType'" +
					" AND name <> 'ProductCode'" +
					" AND name <> 'CustomerProductName'" +
					" AND name <> 'CustomerProductCode'" +
					" AND ((name NOT LIKE('%Index%') OR name LIKE 'Lookup%Index'))" +
					" AND name NOT LIKE('%Guid%')" +
					" AND (name <> 'TransID' OR object_name(id) = 'tblTransactions')" +
					" ORDER BY name";

			cmd.CommandText = sql;
			cmd.Parameters.AddWithValue("@Table", table);
		}
		#endregion
	}
}