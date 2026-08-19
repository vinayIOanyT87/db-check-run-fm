
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.InternalClasses;
	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class TransactionAliasFieldsClass : ITransactionAliasFields
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public TransactionAliasFieldsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, TransactionAliasFieldClass transactionAliasField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (transactionAliasField == null)
			{
				throw new ArgumentNullException("transactionAliasField");
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			transactionAliasField.CreatedDate = DateTimeOffset.Now;
			transactionAliasField.CreatedBy = security.UserID;
			transactionAliasField.UpdatedDate = transactionAliasField.CreatedDate;
			transactionAliasField.UpdatedBy = security.UserID;
			transactionAliasField.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				transactionAliasField.InsertSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Insert(security, transactionAliasField, false);

			return transactionAliasField.IdentityGuid;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, TransactionAliasFieldClass transactionAliasField)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (transactionAliasField == null)
			{
				throw new ArgumentNullException("transactionAliasField");
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			TransactionAliasFieldClass oldTransactionAliasField = Get(security, transactionAliasField.IdentityGuid);

			if (oldTransactionAliasField.IdentityGuid == Guid.Empty)
			{
				throw (new Exception("TransactionAliasField Not Found"));
			}

			transactionAliasField.UpdatedDate = DateTimeOffset.Now;
			transactionAliasField.UpdatedBy = security.UserID;

			using (var cmd = new SqlCommand())
			{
				transactionAliasField.UpdateSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			TransactionAliasFieldClass transactionAliasField = Get(security, identityGuid);

			if (transactionAliasField.IdentityGuid == Guid.Empty)
			{
				return;
			}

			var dependencies = new DependenciesClass(security);
			dependencies.Purge(security, transactionAliasField);

			using (var cmd = new SqlCommand())
			{
				transactionAliasField.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}

		public TransactionAliasFieldClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) 
				&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
			    && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				throw new FMInsufficientRightsException();
			}

			var transactionAliasField = new TransactionAliasFieldClass { IdentityGuid = identityGuid };

			using (var cmd = new SqlCommand())
			{
				transactionAliasField.SelectSQL(cmd, ContextUtil.IsInTransaction);
				transactionAliasField.Load(ConsolidatedDA.GetDataSet(cmd, security));

				return transactionAliasField;
			}
		}


        /// <summary>
        /// Extended database attributes for all the fields defined for transactions and sub nodes
        /// </summary>
        /// <param name="userSecurity"></param>
        /// <returns></returns>
        public IEnumerable<TransactionAliasFieldExtendedAttributes> GetColumnDefinitionsForTransactions(SecurityClass userSecurity)
        {
            if (userSecurity == null)
            {
                throw new ArgumentNullException("security");
            }

            if (!userSecurity.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES))
            {
                throw new FMInsufficientRightsException();
            }

            var results = new List<TransactionAliasFieldExtendedAttributes>();
            using (var command = new SqlCommand())
            {
                TransactionAliasFieldExtendedAttributes.EnumerateSQL(command);
                using (var dbResults = this.ConsolidatedDA.GetDataTable(command, userSecurity))
                {
                    foreach (DataRow entry in dbResults.Rows)
                    {
                        var tempFieldAttributes = new TransactionAliasFieldExtendedAttributes();
                        tempFieldAttributes.Load(entry);
                        results.Add(tempFieldAttributes);
                    }
                }
                return results;
            }
        }

        /// <summary>
        /// Gets a list of TransactionAliasFieldClass objects from the database given the specified parameters.
        /// </summary>
        /// <param name="security">The security object</param>
        /// <param name="transactionAliasGuid">The asscoiated transaction alias identity Guid</param>
        /// <param name="type">The type of transaction field to retrieve</param>
        /// <param name="dispatchFields">If true then retrieve dispatch transaction fields</param>
        /// <param name="byUser">If true then retrieve fields associated with the current user</param>
        /// <returns>The specified list of TransactionAliasFieldClass objects</returns>
        public TransactionAliasFieldCollectionClass Enumerate(
															SecurityClass security,
															Guid transactionAliasGuid,
															TransactionFieldType type,
															bool dispatchFields,
															bool byUser)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var transactionAliasField = new TransactionAliasFieldClass
				{
					TransactionAliasGuid = transactionAliasGuid,
					Type = type,
					DispatchField = dispatchFields
				};

			using (var cmd = new SqlCommand())
			{
				transactionAliasField.EnumerateSQL(cmd, security, byUser, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(cmd, security);
				var transactionAliasFieldCollection = new TransactionAliasFieldCollectionClass();

				DataTable table = set.Tables[0];

				while (table.Rows.Count != 0)
				{
					transactionAliasField = new TransactionAliasFieldClass();
					transactionAliasField.Load(set);

					// Exclude field if true.
					if (this.ExcludeField(transactionAliasField).Equals(false))
					{
						transactionAliasFieldCollection.Add(transactionAliasField);
					}

					table.Rows.RemoveAt(0);
				}

				return transactionAliasFieldCollection;
			}
		}

		/// <summary>
		/// The enumerate by alias ID.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="aliasGuid">
		/// The alias GUID.
		/// </param>
		/// <param name="byUser">
		/// The by user.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasFieldCollectionClass"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// </exception>
		public TransactionAliasFieldCollectionClass EnumerateByAliasGuid(SecurityClass security, Guid aliasGuid, bool byUser)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var transactionAliasField = new TransactionAliasFieldClass
			{
				TransactionAliasGuid = aliasGuid
			};

			var transactionAliasFieldCollection = new TransactionAliasFieldCollectionClass();

			using (var command = new SqlCommand())
			{
				transactionAliasField.EnumerateByAliasIdSql(command, security, byUser, ContextUtil.IsInTransaction);
				DataSet set = this.ConsolidatedDA.GetDataSet(command, security);

				DataTable table = set.Tables[0];
				while (table.Rows.Count != 0)
				{
					transactionAliasField = new TransactionAliasFieldClass();
					transactionAliasField.Load(set);
					transactionAliasFieldCollection.Add(transactionAliasField);
					table.Rows.RemoveAt(0);
				}
			}

			return transactionAliasFieldCollection;
		}

		/// <summary>
		/// This method will return the list of alias fields to display.
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="fieldType">Contains the transaction alias field type.</param>
		/// <param name="transType">Contains the transaction type.</param>
		/// <returns>Returns a string array of alias field names.</returns>
		public List<string> EnumerateFields(SecurityClass security, TransactionFieldType fieldType, TransactionTypes transType)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES) && !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES))
			{
				throw new FMInsufficientRightsException();
			}

			var transactionAliasField = new TransactionAliasFieldClass
			{
				Type = fieldType
			};

			using (var cmd = new SqlCommand())
			{
				transactionAliasField.EnumerateFieldsSQL(cmd, transType);
				DataSet set = ConsolidatedDA.GetDataSet(cmd, security);

				DataTable table = set.Tables[0];

				//var fields = new string[table.Rows.Count];
				var fields = new List<string>();

				foreach (DataRow row in table.Rows)
				{
					var fieldName = DataObject.getValue<string>(row["name"], "");

					if (fieldType == TransactionFieldType.LineItem)
					{
						if (fieldName == "CloseoutDate")
						{
							fieldName = "ItemCloseoutDate";
						}
						else if (fieldName == "RequestedDateTime")
						{
							fieldName = "ItemRequestedDateTime";
						}
						else if (fieldName == "DispatchedDateTime")
						{
							fieldName = "ItemDispatchedDateTime";
						}

						// Exclude the following fields.
						if (fieldName.ToUpper().Equals("CREATEDBY").Equals(false) &&
							fieldName.ToUpper().Equals("CREATEDDATE").Equals(false) &&
							fieldName.ToUpper().Equals("UPDATEDBY").Equals(false) &&
							fieldName.ToUpper().Equals("UPDATEDDATE").Equals(false))
						{
							// fields.Add(fieldName); TODO: Uncomment this line of code when financial is to be added back.

							// TODO: Temporary commented out so that QA does not test financial configuration features.
							// TODO: Remove this IF code block when financial is to be added back.
							if (fieldName.ToUpper().Equals("CURRENCYGUID").Equals(false) &&
								fieldName.ToUpper().Equals("EXCHANGERATE").Equals(false) &&
								fieldName.ToUpper().Equals("NONDOMESTICPRICE").Equals(false) &&
								fieldName.ToUpper().Equals("PRODUCTPRICE").Equals(false) &&
								fieldName.ToUpper().Equals("TAX1").Equals(false) &&
								fieldName.ToUpper().Equals("TAX2").Equals(false) &&
								fieldName.ToUpper().Equals("TAX3").Equals(false) &&
								fieldName.ToUpper().Equals("TAX4").Equals(false) &&
								fieldName.ToUpper().Equals("TAX5").Equals(false) &&
								fieldName.ToUpper().Equals("TOTALVALUE").Equals(false))
							{
								fields.Add(fieldName);
							}
						}

					}
					else if (fieldType == TransactionFieldType.WeightReading)
					{
						// Exclude the following fields.
						if (fieldName.ToUpper().Equals("FUELSMANAGERVERSIONNUMBER").Equals(false) &&
							fieldName.ToUpper().Equals("HISTORICALFLAG").Equals(false) &&
							fieldName.ToUpper().Equals("SOURCEVERSIONNUMBER").Equals(false) &&
							fieldName.ToUpper().Equals("CREATEDBY").Equals(false) &&
							fieldName.ToUpper().Equals("CREATEDDATE").Equals(false) &&
							fieldName.ToUpper().Equals("UPDATEDBY").Equals(false) &&
							fieldName.ToUpper().Equals("UPDATEDDATE").Equals(false))
						{
							fields.Add(fieldName);
						}
					}
					else if (fieldType == TransactionFieldType.Note)
					{
						// Exclude the following fields.
						if (fieldName.ToUpper().Equals("CREATEDBY").Equals(false) &&
							fieldName.ToUpper().Equals("CREATEDDATE").Equals(false) &&
							fieldName.ToUpper().Equals("UPDATEDBY").Equals(false) &&
							fieldName.ToUpper().Equals("UPDATEDDATE").Equals(false))
						{
							fields.Add(fieldName);
						}
					}
					else if (fieldType == TransactionFieldType.TransportInfo)
					{
						// Exclude the following fields.
						if (fieldName.ToUpper().Equals("CREATEDBY").Equals(false) &&
							fieldName.ToUpper().Equals("CREATEDDATE").Equals(false) &&
							fieldName.ToUpper().Equals("UPDATEDBY").Equals(false) &&
							fieldName.ToUpper().Equals("UPDATEDDATE").Equals(false))
						{
							fields.Add(fieldName);
						}
					}
					else
					{
						fields.Add(fieldName);
					}
				}

				// Add fields with special handling
				fields = this.AddConjoinedFields(fields, fieldType, transType);
				fields = this.AddVirtualFields(fields, fieldType);
				fields = this.AddCustomFields(fields, fieldType);

				return fields;
			}
		}

		/// <summary>
		/// This method will add custom fields to the list of fields.
		/// </summary>
		/// <param name="fields">Original list of fields</param>
		/// <param name="type">Transaction field type.</param>
		/// <returns>Field list with custom fields added.</returns>
		protected List<string> AddCustomFields(List<string> fields, TransactionFieldType type)
		{
			// failsafe
			var hardwareKey = new HardwareKeyClass();

			if (hardwareKey.IsADFKey() == false)
			{
				return fields;
			}

			var customFields = new List<string>();

			switch (type)
			{
				case TransactionFieldType.Transaction:
					customFields.Add("InvoiceQuery");
					customFields.Add("ADFTransactionDateTime");
					customFields.Add("BulkPaymentNumber");
					customFields.Add("AssocTxControl");
					customFields.Add("TotalOnCost");
					customFields.Add("ROSupplier");
					customFields.Add("TotalForeignPrice");
					break;

				case TransactionFieldType.LineItem:
					customFields.Add("CustomProductsLabel");
					customFields.Add("SelectedQuality");
					customFields.Add("DeliveryLocationLabel");
					customFields.Add("ParentUserData03");
					customFields.Add("ParentDocumentNumber");
					customFields.Add("ParentReceiptNumber");
					customFields.Add("ParentFuelOrderNumber");
					customFields.Add("BaseCost");
					customFields.Add("TotalForeignPrice");
					customFields.Add("CurrencyUnitLabel");
					customFields.Add("TotalOnCost");
					customFields.Add("OnCost");
					break;
			}

			foreach (string field in fields)
			{
				customFields.Add(field);
			}

			customFields.Sort();
			return customFields;
		}

		/// <summary>
		/// This method will append conjoined field names to the field list.
		/// </summary>
		/// <param name="fields">Original list of fields</param>
		/// <param name="type">Transaction field type.</param>
		/// <param name="transType">Transaction type.</param>
		/// <returns>Field list with conjoined fields added.</returns>
		private List<string> AddConjoinedFields(List<string> fields, TransactionFieldType type, TransactionTypes transType)
		{
			if (type == TransactionFieldType.Transaction)
			{
				var conjoinedFields = new List<string>();

				if (transType == TransactionTypes.T11_ConsumerTransfer)
				{
					conjoinedFields.Add("FromBillToID");
					conjoinedFields.Add("FromShipToID");
					conjoinedFields.Add("ToBillToID");
					conjoinedFields.Add("ToShipToID");
				}
				else if (transType == TransactionTypes.T13_OwnerTransfer)
				{
					conjoinedFields.Add("FromManagerID");
					conjoinedFields.Add("FromOwnerID");
					conjoinedFields.Add("FromCarrierID");
					conjoinedFields.Add("ToManagerID");
					conjoinedFields.Add("ToOwnerID");
					conjoinedFields.Add("ToCarrierID");
				}

				foreach (string field in fields)
				{
					conjoinedFields.Add(field);
				}

				conjoinedFields.Sort();
				return conjoinedFields;
			}

			if (type == TransactionFieldType.LineItem)
			{
				var conjoinedFields = new List<string>();

				if (transType == TransactionTypes.T15_PrimaryRegrade
				|| transType == TransactionTypes.T16_SecondaryRegrade)
				{
					conjoinedFields.Add("FromProduct");
					conjoinedFields.Add("ToProduct");
					conjoinedFields.Add("FromStorageLocationID");
					conjoinedFields.Add("ToStorageLocationID");
				}

				if (transType == TransactionTypes.T23_StorageTransfer ||
					transType == TransactionTypes.T13_OwnerTransfer)
				{
					conjoinedFields.Add("FromStorageLocationID");
					conjoinedFields.Add("ToStorageLocationID");
				}

				foreach (string field in fields)
				{
					conjoinedFields.Add(field);
				}

				conjoinedFields.Sort();
				return conjoinedFields;
			}

			return fields;
		}

		/// <summary>
		/// This method will add the virtual fields to the field list.
		/// </summary>
		/// <param name="fields">Original list of fields</param>
		/// <param name="type">Transaction field type.</param>
		/// <returns>Field list with virtual fields added.</returns>
		private List<string> AddVirtualFields(List<string> fields, TransactionFieldType type)
		{
			// Add virtual fields to the transaction header fields
			// WARNING - If you add fields to this list, you need to mark their field generator as a virtual field
			if (type == TransactionFieldType.Transaction)
			{
				var virtualFields = TransactionAliasFieldClass.VirtualFields(type);

				foreach (string field in virtualFields)
				{
					fields.Add(field);
				}

				fields.Sort();
			}

			// WARNING - If you add fields to this list, you need to mark their field generator as a virtual field
			if (type == TransactionFieldType.LineItem)
			{
				var virtualFields = TransactionAliasFieldClass.VirtualFields(type);

				foreach (string field in virtualFields)
				{
					fields.Add(field);
				}

				fields.Sort();
			}

			return fields;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void ModifyCollection(
									SecurityClass security,
									Guid transactionAliasGuid,
									string transactionAliasName,
									TransactionAliasFieldCollectionClass newFieldCollection,
									TransactionAliasFieldCollectionClass oldFieldCollection)
		{
			if (newFieldCollection != null)
			{
				foreach (var fieldClass in newFieldCollection)
				{
					var newField = (TransactionAliasFieldClass)fieldClass;
					newField.TransactionAliasGuid = transactionAliasGuid;
					newField.AliasName = transactionAliasName;

					if (oldFieldCollection != null)
					{
						int collectionIndex = 0;
						foreach (var fieldClass1 in oldFieldCollection)
						{
							var oldField = (TransactionAliasFieldClass)fieldClass1;
							if (oldField.Type == newField.Type
								&& oldField.DbName == newField.DbName
								&& oldField.DispatchField == newField.DispatchField
								&& oldField.IdentityGuid != Guid.Empty)
							{
								if (oldField.DisplayOrder != newField.DisplayOrder
									|| oldField.DisplayName != newField.DisplayName
									|| oldField.FieldRequired != newField.FieldRequired
									|| oldField.UserGroupGuid != newField.UserGroupGuid
									|| oldField.ClearOnNew != newField.ClearOnNew
									|| oldField.DefaultValue != newField.DefaultValue
									|| oldField.ReadOnly != newField.ReadOnly
									|| oldField.Visibility != newField.Visibility
									)
								{
									newField.IdentityGuid = oldField.IdentityGuid;
									this.Modify(security, newField);
								}

								break;
							}

							collectionIndex++;
						}

						if (collectionIndex < oldFieldCollection.Count)
						{
							oldFieldCollection.Remove(collectionIndex);
						}
						else
						{
							this.Add(security, newField);
						}
					}
					else
					{
						this.Add(security, newField);
					}
				}
			}

			if (oldFieldCollection != null)
			{
				foreach (var fieldClass in oldFieldCollection)
				{
					var oldField = (TransactionAliasFieldClass)fieldClass;
					this.Purge(security, oldField.IdentityGuid);
				}
			}
		}

		/// <summary>
		/// This method will return true if a field is to be excluded
		/// </summary>
		/// <param name="aliasField">Transaction alias field object.</param>
		/// <returns>Return true if excluded. Otherwise, false.</returns>
		private bool ExcludeField(TransactionAliasFieldClass aliasField)
		{
			bool excludeField = false;

			switch (aliasField.Type)
			{
				case TransactionFieldType.LineItem:
					if (aliasField.DbName.ToUpper().Equals("CREATEDDATE") ||
						aliasField.DbName.ToUpper().Equals("CREATEDBY") ||
						aliasField.DbName.ToUpper().Equals("UPDATEDDATE") ||
						aliasField.DbName.ToUpper().Equals("UPDATEDBY"))
					{
						excludeField = true;
					}

					// TODO: Temporary commented out so that QA does not test financial configuration features.
					if (aliasField.IsFinancialField)
					{
						excludeField = true;
					}

					// TODO: Temporary commented out so that QA does not test financial configuration features.
					// TODO: Remove this IF code block when financial is to be added back.
					if (aliasField.DbName.ToUpper().Equals("CURRENCYGUID") ||
						aliasField.DbName.ToUpper().Equals("EXCHANGERATE") ||
						aliasField.DbName.ToUpper().Equals("NONDOMESTICPRICE") ||
						aliasField.DbName.ToUpper().Equals("PRODUCTPRICE") ||
						aliasField.DbName.ToUpper().Equals("TAX1") ||
						aliasField.DbName.ToUpper().Equals("TAX2") ||
						aliasField.DbName.ToUpper().Equals("TAX3") ||
						aliasField.DbName.ToUpper().Equals("TAX4") ||
						aliasField.DbName.ToUpper().Equals("TAX5") ||
						aliasField.DbName.ToUpper().Equals("TOTALVALUE"))
					{
						excludeField = true;
					}
					break;

				case TransactionFieldType.WeightReading:
					if (aliasField.DbName.ToUpper().Equals("FUELSMANAGERVERSIONNUMBER") ||
						aliasField.DbName.ToUpper().Equals("HISTORICALFLAG") ||
						aliasField.DbName.ToUpper().Equals("SOURCEVERSIONNUMBER") ||
						aliasField.DbName.ToUpper().Equals("CREATEDDATE") ||
						aliasField.DbName.ToUpper().Equals("CREATEDBY") ||
						aliasField.DbName.ToUpper().Equals("UPDATEDDATE") ||
						aliasField.DbName.ToUpper().Equals("UPDATEDBY"))
					{
						excludeField = true;
					}
					break;

				case TransactionFieldType.Note:
					if (aliasField.DbName.ToUpper().Equals("CREATEDDATE") ||
						aliasField.DbName.ToUpper().Equals("CREATEDBY") ||
						aliasField.DbName.ToUpper().Equals("UPDATEDDATE") ||
						aliasField.DbName.ToUpper().Equals("UPDATEDBY"))
					{
						excludeField = true;
					}
					break;

				case TransactionFieldType.TransportInfo:
					if (aliasField.DbName.ToUpper().Equals("CREATEDDATE") ||
						aliasField.DbName.ToUpper().Equals("CREATEDBY") ||
						aliasField.DbName.ToUpper().Equals("UPDATEDDATE") ||
						aliasField.DbName.ToUpper().Equals("UPDATEDBY"))
					{
						excludeField = true;
					}
					break;

				default:
					// TODO: Temporary commented out so that QA does not test financial configuration features.
					if (aliasField.IsFinancialField)
					{
						excludeField = true;
					}
					break;
			}

			return excludeField;
		}


		/// <summary>
		/// Queries the alias fields.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="fields">The fields.</param>
		/// <returns>A collection of aliased fields.</returns>
		[SecurityCritical]
		public QueryWriterFieldCollection EnumerateQueryAliasFields(SecurityClass security, QueryWriterFieldCollection fields, QueryWriterAliasGuidCollection aliasGuids)
		{
			// Start by blanking all the field display values
			foreach (QueryWriterField field in fields)
			{
				field.DisplayName = string.Empty;
			}

			GroupsClass group = new GroupsClass();
			var groupCollection = group.EnumerateByUser(security, security.UserGuid);

			var aliasFields = new List<FieldCollectionClass>();
			var fullAliases = new TransactionAliasCollectionClass();

			if (aliasGuids != null && aliasGuids.Count > 0)
			{
				foreach (QueryWriterAliasGuid aliasGuid in aliasGuids)
				{
					AddMainAliasFields(security, groupCollection, aliasFields, aliasGuid.AliasGuid, fullAliases);
				}
			}
			else
			{
				AddMainAliasFields(security, groupCollection, aliasFields, Guid.Empty, fullAliases);
			}


			// JS20100812 WI-16687 remove censor fields from being selectable
			TransactionAliasFieldCollectionClass censorByRight = this.GetCensorFieldsByRights(security, fullAliases);
			foreach (FieldCollectionClass fieldCollection in aliasFields)
			{
				if (fieldCollection.GetType() == typeof(TransactionAliasFieldCollectionClass))
				{
					var col = fieldCollection as TransactionAliasFieldCollectionClass;
					foreach (TransactionAliasFieldClass censorField in censorByRight)
					{
						this.RemoveCensorField(col, censorField);
					}
				}
			}

			var newCollection = new QueryWriterFieldCollection();
			Dictionary<string, string> dictionary = BuildFieldDictionary(aliasFields);

			foreach (var field in fields)
			{
				this.SetDisplayNames(dictionary, field);
				if (string.IsNullOrEmpty(field.DisplayName) == false)
				{
					newCollection.Add(field);
				}
				else if (field.FieldName.Equals("TransID"))
				{
					// WI-16495 Add Transaction ID to the list because it is so useful but will
					// never show up because it is not contained within a real control
					field.DisplayName = "Transaction ID";
					newCollection.Add(field);
				}
				else if (field.FieldName.Equals("Notes"))
				{
					// WI-17158 Add Notes to the list because it's on every transaction but not technically
					// a configurable transaction alias field either
					field.DisplayName = "Notes";
					newCollection.Add(field);
				}
				else if (field.FieldName.Equals("Status"))
				{
					// WI-78968 Add Transaction Status to the list because we want this field but it is
					// not configurable in transaction alias either
					if (field.DBFieldName == "tblTransactionLineItems.LookupTransactionStatusIndex")
						field.DisplayName = "Line Item Status";
					else
						field.DisplayName = "Transaction Status";
					newCollection.Add(field);
				}
				else if (field.FieldName.Equals("CreatedDate"))
				{
					field.DisplayName = "Created Date";
					newCollection.Add(field);
				}
			}

			return newCollection;
		}


		protected Dictionary<string, string> BuildFieldDictionary(List<FieldCollectionClass> AliasFields)
		{
			Dictionary<string, string> aliasDictionary = new Dictionary<string, string>();
			foreach (FieldCollectionClass AliasFieldCollection in AliasFields)
			{
				foreach (FieldClass AliasField in AliasFieldCollection)
				{
					string dbName = AliasField.DbName;

					if (AliasField.GetType().Equals(typeof(TransactionAliasFieldClass)))
					{
						var fieldType = (AliasField as TransactionAliasFieldClass).Type;

						if (fieldType == TransactionFieldType.Transaction)
						{
							dbName = "tblTransactions." + dbName;
						}
						else if (fieldType == TransactionFieldType.LineItem)
						{
							dbName = "tblTransactionLineItems." + dbName;
						}
						else if (fieldType == TransactionFieldType.Note)
						{
							dbName = "tblTransactionNotes." + dbName;
						}
						else if (fieldType == TransactionFieldType.TransportInfo)
						{
							dbName = "tblTransactionTransportLineItems." + dbName;
						}
						else if (fieldType == TransactionFieldType.WeightReading)
						{
							dbName = "tblTransactionWeightReadings." + dbName;
						}
						else
						{
							dbName = "tblExportInterfaceResult." + dbName;
						}
					}
					else if (AliasField.GetType().Equals(typeof(UserDataFieldClass)))
					{
						dbName = string.Format("UserData{0}", (AliasField as UserDataFieldClass).Number + 1);
						if ((AliasField as UserDataFieldClass).UserDataEntityType == ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM)
						{
							dbName = "tblTransactionLineItemUserData." + dbName;
						}
						else
						{
							dbName = "tblTransactionUserData." + dbName;
						}
					}

					dbName = dbName.ToUpper();

					string dictValue = "";
					if (!aliasDictionary.TryGetValue(dbName, out dictValue))
					{
						dictValue = AliasField.DisplayName;
						aliasDictionary.Add(dbName, dictValue);
					}
					else
					{
						// Only add name if it isn't already in the list
						string testValue = AliasField.DisplayName;
						if (testValue[testValue.Length - 1].Equals(':'))
						{
							testValue = testValue.Substring(0, testValue.Length - 1);
						}

						if (dictValue.Contains(testValue) == false)
						{
							dictValue += string.IsNullOrEmpty(dictValue) ? string.Empty : "/";
							dictValue += testValue;
							aliasDictionary[dbName] = dictValue;
						}
					}
				}
			}
			return aliasDictionary;
		}

		/// <summary>
		/// The get censor fields by rights.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasFieldCollectionClass"/>.
		/// </returns>
		protected TransactionAliasFieldCollectionClass GetCensorFieldsByRights(SecurityClass security, TransactionAliasCollectionClass fullAliasCollection)
		{
			var censorFields = new TransactionAliasFieldCollectionClass();

			// JS20100812 WI-16687 filter out financial fields if user do not have the right
			if (!security.HasRight(RIGHT.VIEW_FINANCIAL_DATA))
			{
				censorFields = ProcessCensorFields(security, fullAliasCollection);
			}

			return censorFields;
		}

		private void RemoveCensorField(TransactionAliasFieldCollectionClass col, TransactionAliasFieldClass censorField)
		{
			for (int index = 0; index < col.Count; ++index)
			{
				if (col.Item(index).DbName.Equals(censorField.DbName))
				{
					col.RemoveAt(index);
					break;
				}
			}
		}


		/// <summary>
		/// Adds the alias collection.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="aliasFields">The tx alias fields.</param>
		/// <param name="aliases">The aliases.</param>
		/// <param name="groupCollection">The group collection.</param>
		/// <param name="aliasFieldList">The alias fields.</param>
		/// <param name="aliasCollection">The alias collection.</param>
		private void AddAliasCollection(SecurityClass security, GroupCollectionClass groupCollection, List<FieldCollectionClass> aliasFieldList, TransactionAliasCollectionClass aliasCollection, TransactionAliasCollectionClass fullAliasCollection)
		{

			var aliases = new TransactionAliasesClass();
			var aliasFields = new TransactionAliasFieldsClass();

			foreach (var transactionAlias in aliasCollection)
			{
				TransactionAliasClass fullAlias = aliases.Get(security, transactionAlias.IdentityGuid, false);

				fullAliasCollection.Add(fullAlias);

				// Only include aliases assigned for the current user
				if (fullAlias.GroupTransactionAliasMapCollection.Count == 0 || this.UserHasAccessToGroup(fullAlias, groupCollection))
				{
					//TransactionAliasFieldCollectionClass transactionFieldCollection = aliasFields.Enumerate(security, transactionAlias.IdentityGuid, TRANSACTION_FIELD_TYPE.TRANSACTION, false, false);
					//TransactionAliasFieldCollectionClass lineItemFieldCollection = aliasFields.Enumerate(security, transactionAlias.IdentityGuid, TRANSACTION_FIELD_TYPE.LINE_ITEM, false, false);
					//TransactionAliasFieldCollectionClass exportResultFieldCollection = aliasFields.Enumerate(security, transactionAlias.IdentityGuid, TRANSACTION_FIELD_TYPE.EXPORT_RESULT, false, false);
					//TransactionAliasFieldCollectionClass transportLineItemFieldCollection = aliasFields.Enumerate(security, transactionAlias.IdentityGuid, TRANSACTION_FIELD_TYPE.TRANSPORT_INFO, false, false);

					aliasFieldList.Add(fullAlias.TransactionFieldCollection);
					aliasFieldList.Add(fullAlias.LineItemFieldCollection);
					aliasFieldList.Add(fullAlias.ExportResultDetailFieldCollection);
					aliasFieldList.Add(fullAlias.TransportLineItemFieldCollection);

					aliasFieldList.Add(fullAlias.NoteFieldCollection);
					aliasFieldList.Add(fullAlias.WeightReadingFieldCollection);

					aliasFieldList.Add(fullAlias.UserDataFieldCollection);

					// JS20101001 WI-18005 add the line item user data fields
					aliasFieldList.Add(fullAlias.LineItemUserDataFieldCollection);
				}
			}
		}


		/// <summary>
		/// Adds the main alias fields.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="aliases">The aliases.</param>
		/// <param name="groupCollection">The group collection.</param>
		/// <param name="aliasFields">The alias fields.</param>
		/// <param name="fullAliasCollection">the fully loaded alias used later for censoring fields</param>
		private void AddMainAliasFields(SecurityClass security, GroupCollectionClass groupCollection, List<FieldCollectionClass> aliasFields, Guid aliasGuid, TransactionAliasCollectionClass fullAliasCollection)
		{
			TransactionAliasCollectionClass aliasCollection;

			var aliases = new TransactionAliasesClass();

			if (aliasGuid != Guid.Empty)
			{
				aliasCollection = new TransactionAliasCollectionClass();
				aliasCollection.Add(new TransactionAliasClass() { IdentityGuid = aliasGuid, SiteGuid = security.SiteGuid });//only using guid here since it gets fully loaded later
			}
			else
			{
				aliasCollection = aliases.Enumerate(security);
			}

			AddAliasCollection(security, groupCollection, aliasFields, aliasCollection, fullAliasCollection);
		}

		private bool UserHasAccessToGroup(TransactionAliasClass alias, GroupCollectionClass groupcollection)
		{
			foreach (GroupTransactionAliasMapClass group in alias.GroupTransactionAliasMapCollection)
			{
				foreach (GroupClass userGroup in groupcollection)
				{
					if (userGroup.IdentityGuid == group.GroupGuid)
					{
						return true;
					}
				}
			}

			return false;
		}


		protected void SetDisplayNames(Dictionary<string, string> AliasFields, QueryWriterField Field)
		{
			string toRet;

			if (AliasFields.TryGetValue(Field.DBFieldName.ToUpper(), out toRet))
			{
				Field.DisplayName = toRet;
			}
		}

		/// <summary>
		/// The process censor fields.
		/// </summary>
		/// <param name="aliases">
		/// The aliases.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="isAdf">
		/// The is ADF.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasFieldCollectionClass"/>.
		/// </returns>
		private TransactionAliasFieldCollectionClass ProcessCensorFields(SecurityClass security, TransactionAliasCollectionClass fullAliasCollection)
		{
			var censorFields = new TransactionAliasFieldCollectionClass();
			var aliases = new TransactionAliasesClass();
			var key = new HardwareKeyClass();

			var isAdf = key.IsADFKey();

			//            TransactionAliasCollectionClass aliasCollection = aliases.Enumerate(security);

			foreach (TransactionAliasClass alias in fullAliasCollection)
			{
				TransactionAliasClass fullAlias = alias; // aliases.Get(security, alias.IdentityGuid, false);
				foreach (TransactionAliasFieldClass field in fullAlias.TransactionFieldCollection)
				{
					if (field.IsFinancialField)
					{
						censorFields.Add(field);
					}
				}

				foreach (TransactionAliasFieldClass field in fullAlias.LineItemFieldCollection)
				{
					// ADF only, also remove number fields for sales and issues
					if (isAdf &&
					(field.ID.ToUpper().Equals("NUMBER01") ||
					 field.ID.ToUpper().Equals("NUMBER02") ||
					 field.ID.ToUpper().Equals("NUMBER03") ||
					 field.ID.ToUpper().Equals("NUMBER04") ||
					 field.ID.ToUpper().Equals("NUMBER05") ||
					 field.ID.ToUpper().Equals("NUMBER06")) &&
					(fullAlias.ID.ToUpper().Contains("SALE") ||
					 fullAlias.ID.ToUpper().Contains("ISSUE"))
						)
					{
						censorFields.Add(field);
					}
					else if (field.IsFinancialField)
					{
						censorFields.Add(field);
					}
				}
			}

			return censorFields;
		}
	}
}
