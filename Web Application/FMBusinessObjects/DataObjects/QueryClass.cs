namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml.Linq;
	using System.Xml.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using ChannelFactories;

	using FMCore;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using FMBusinessObjects.Constants;

	public enum QueryRowType
	{
		DataRow,
		Subtotal,
		Total
	}

	[Serializable]
	[CollectionDataContract]
	public class QueryCollectionClass : List<QueryClass>
	{
		public void AddDistinct(SecurityClass security, QueryClass query)
		{
			var existingQuery = this.Find(delegate (QueryClass q) { return q.QueryName.Equals(query.QueryName); });
			if (existingQuery == null)
			{
				this.Add(query);
			}
			else if (query.OwnerUserGuid == security.UserGuid && existingQuery.OwnerUserGuid != query.OwnerUserGuid)
			{
				this.Remove(existingQuery);
				this.Add(query);
			}
		}
	}

	[Serializable()]
	[XMLObject(NodeName = "FuelsManager.Query")]
	[DataContract]
	public class QueryClass : BaseDataObject
	{
		public const string ROW_TYPE = "Internal__RowType";
		public const string LINE_NUMBER = "Internal__LineNumber";
		public const string GROUP_LINE_NUMBER = "Internal__GroupLineNumber";

		private List<Assembly> fmBusinessObjects;
		private Assembly convertUnits;
		private string queryAssemblies;

		[DataMember]
		public QueryWriterTopic Topic { get; set; }

		[XMLProperty]
		public Type TopicIDType
		{
			get
			{
				return this.Topic.ObjectType;
			}
		}

		[XMLCollection(NodeName = "Fields")]
		[DataMember]
		public QueryWriterFieldCollection Fields { get; set; }

		[XMLCollection(NodeName = "Criterion")]
		[DataMember]
		public QueryCriteriaPhraseCollection Criterion { get; set; }

		[XMLCollection(NodeName = "AssignedGroups")]
		[DataMember]
		public GroupCollectionClass AssignedGroups { get; set; }

		[XMLCollection(NodeName = "FilterGroups")]
		[DataMember]
		public QueryFilterGroupCollection FilterGroups { get; set; }

		[XMLCollection(NodeName = "DataGroups")]
		[DataMember]
		public QueryWriterFieldCollection DataGroups { get; set; }

		[DataMember]
		public Guid OwnerUserGuid { get; set; }

		[XMLCollection(NodeName = "TransactionAliasGuids")]
		[DataMember]
		public QueryWriterAliasGuidCollection TransactionAliasGuids { get; set; }

		[XMLProperty]
		[DataMember]
		public string Title { get; set; }

		[XMLProperty]
		[DataMember]
		public string Header { get; set; }

		[XMLProperty]
		[DataMember]
		public string Footer { get; set; }

		[XMLProperty]
		[DataMember]
		public string InitialPageSize { get; set; }

		[XMLProperty]
		[DataMember]
		public bool TotalAllFields { get; set; }

		[XMLProperty]
		[DataMember]
		public bool IncludeLineNumbers { get; set; }

		[XMLProperty]
		[DataMember]
		public bool ShowSummaryLinesOnly { get; set; }

		[XMLProperty]
		[DataMember]
		public bool QueryOnArchiveData { get; set; }

		[XMLProperty]
		[DataMember]
		public string QueryName { get; set; }

		[XMLProperty]
		[DataMember]
		public string QueryDescription { get; set; }

		[XMLProperty]
		[DataMember]
		public string NavNodePath { get; set; }

		[XMLProperty]
		[DataMember]
		public bool SystemQuery { get; set; }

		[XMLProperty]
		[DataMember]
		public bool QueryCalledFromMenu { get; set; }

		[XMLProperty]
		[DataMember]
		public Guid QueryStorageGuid
		{
			get
			{
				return this._IdentityGuid;
			}
			set
			{
				this._IdentityGuid = value;
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public QueryClass()
		{
			this.Reset();
		}

		public override void Reset()
		{
			base.Reset();

			this.Fields = new QueryWriterFieldCollection();
			this.Criterion = new QueryCriteriaPhraseCollection();
			this.Title = "";
			this.Header = "";
			this.Footer = "";
			this.InitialPageSize = "10";
			this.TotalAllFields = false;
			this.IncludeLineNumbers = false;
			this.ShowSummaryLinesOnly = false;
			this.QueryOnArchiveData = false;
			this.AssignedGroups = new GroupCollectionClass();
			this.FilterGroups = new QueryFilterGroupCollection();
			this.DataGroups = new QueryWriterFieldCollection();
			this.OwnerUserGuid = Guid.Empty;
			this.NavNodePath = string.Empty;
			this.SystemQuery = false;
			this.TransactionAliasGuids = new QueryWriterAliasGuidCollection();
		}

		public void Load(SecurityClass Security, object O, bool bQuickLoad, string queryAssemblies)
		{
			if (Security == null)
			{
				throw new ArgumentNullException("Security");
			}

			if (typeof(DataSet).IsInstanceOfType(O))
			{
				DataSet Set = (DataSet)O;

				this.Reset();

				DataTable Table = Set.Tables[0];
				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow Row = Table.Rows[0];

				if (bQuickLoad == false)
				{
					this.ReadXML(Security, DataObject.getValue<string>(Row["QueryXML"], ""), queryAssemblies);
				}

				this._IdentityGuid = DataObject.getValue<Guid>(Row["QueryStorageGuid"], Guid.Empty);
				this.QueryName = DataObject.getValue<string>(Row["QueryName"], "");
				this.QueryDescription = DataObject.getValue<string>(Row["QueryDescription"], "");
				this.NavNodePath = DataObject.getValue<string>(Row["NavNodePath"], "");
				this.SystemQuery = DataObject.getValue<bool>(Row["SystemQuery"], false);
				this._SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
				this._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
				this._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
				this._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this._CreatedDate);
				this._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
				this.OwnerUserGuid = DataObject.getValue<Guid>(Row["OwnerUserGuid"], Guid.Empty);

			}
			else
			{
				base.Load(O);
			}

		}


		private static string Prescrub(string p)
		{
			string result = p.Replace("&amp;", "**ampersand**");
			return result;
		}

		public XElement GetXML()
		{
			object[] classAttributes = this.GetType().GetCustomAttributes(typeof(XMLObject), false);

			XMLObject classAttribute = classAttributes.DefaultIfEmpty(null).FirstOrDefault() as XMLObject;

			XElement mainElement = new XElement("FuelsManager.Queries");

			if (classAttribute != null)
			{
				XElement queryNode = new XElement(classAttribute.NodeName);
				mainElement.Add(queryNode);
				this.GetPropertyXML(this, queryNode);
			}

			return mainElement;

		}

		private void GetPropertyXML(object mainObject, XElement mainElement)
		{
			PropertyInfo[] Properties = mainObject.GetType().GetProperties();

			foreach (PropertyInfo Property in Properties)
			{
				foreach (XMLProperty xmlProperty in Property.GetCustomAttributes(typeof(XMLProperty), false))
				{
					string name = (xmlProperty.NodeName.Equals("")) ? Property.Name : xmlProperty.NodeName;

					object mainValue = Property.GetValue(mainObject, null);
					string value = "";
					if (mainValue != null)
					{
						value = mainValue.ToString();
					}

					if (value.Equals("") == false)
					{
						mainElement.Add(new XElement(name, value));
					}
				}

				foreach (XMLCollection xmlCollection in Property.GetCustomAttributes(typeof(XMLCollection), false))
				{
					XElement outerElement = new XElement(xmlCollection.NodeName);
					mainElement.Add(outerElement);

					IList aList = (IList)Property.GetValue(mainObject, null);

					this.AddElements(Property, outerElement, aList);
				}

			}

		}

		private void AddElements(PropertyInfo Property, XElement outerElement, IList aList)
		{
			if (aList != null)
			{
				foreach (object Element in aList)
				{
					object[] classAttributes = Element.GetType().GetCustomAttributes(typeof(XMLObject), false);

					XMLObject classAttribute = classAttributes.DefaultIfEmpty(null).FirstOrDefault() as XMLObject;

					XElement mainElement = new XElement(classAttribute.NodeName);

					outerElement.Add(mainElement);

					this.GetPropertyXML(Element, mainElement);
				}

			}

		}

		public void ReadXML(SecurityClass security, string xmlStream, string queryAssemblies)
		{
			// First undo any pre-scrubbing
			xmlStream = xmlStream.Replace("**ampersand**", "&amp;");

			XDocument document = XDocument.Parse(xmlStream);

			var newQuery = (from F in document.Descendants("FuelsManager.Query")
								 where true
								 select F)
								.DefaultIfEmpty(null)
								.FirstOrDefault();

			// Load assemblies for reflection information - doing the load with the use of a self lookup
			// AssemblyName object allows us to get the product version without having to remember to 
			// change it each time the assembly version changes in the code.
			//var name = new AssemblyName(this.GetType().Assembly.FullName) { Name = "FMBusinessObjects" };

			//// e6190a5bf6c69d61 - DANGER: search and replace will not work.  Actual usage is broken into bytes
			//name.SetPublicKeyToken(new byte[] { 0xe6, 0x19, 0x0a, 0x5b, 0xf6, 0xc6, 0x9d, 0x61 });
			this.fmBusinessObjects = GetQueryAssemblies(queryAssemblies);

			this.queryAssemblies = queryAssemblies;

			var convertName = new AssemblyName(typeof(EngineeringUnit).Assembly.FullName);

			if (!AssemblyDictionary.ContainsKey(convertName.ToString().ToLower()))
			{
				try
				{
					this.convertUnits = Assembly.Load(convertName);
				}
				catch
				{
					this.convertUnits = null;
				}

				if (this.convertUnits != null)
					AssemblyDictionary.Add(convertName.ToString().ToLower(), this.convertUnits);
			}
			else
			{
				this.convertUnits = AssemblyDictionary.Get(convertName.ToString().ToLower());
			}

			// Read main properties
			this.ReadMainProperties(newQuery);

			// Read Fields
			this.ReadFields(newQuery);

			// Read Criteria
			this.ReadCriteria(security, newQuery);

			// Read Groups
			this.ReadGroups(newQuery);

			// Read FilterGroups
			this.ReadFilterGroups(newQuery);

			// Read DataGroups
			this.ReadDataGroups(newQuery);

		}

		private List<Assembly> GetQueryAssemblies(string assemblyPath)
		{
			// Parse the list of assemblies
			char[] separator = { ';' };
			string[] assemList = assemblyPath.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			List<Assembly> toRet = new List<Assembly>();

			// Go through all the assemblies
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			foreach (string assemblyName in assemList)
			{
				try
				{
					Assembly dll = null;
					if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
					{
						try
						{
							dll = Assembly.LoadFrom(baseDirectory + "\\bin\\" + assemblyName);
						}
						catch
						{
							try
							{
								dll = Assembly.Load(assemblyName);
							}
							catch (Exception ex)
							{
								string message = "Assembly Load Error in Query Load. " + ex.Message;
								FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
							}
						}

						if (dll != null)
						{
							AssemblyDictionary.Add(assemblyName.ToLower(), dll);
						}
					}
					else
					{
						dll = AssemblyDictionary.Get(assemblyName.ToLower());
					}

					if (dll != null)
					{
						toRet.Add(dll);
					}
				}
				catch
				{

				}
			}

			return toRet;
		}

		private void ReadMainProperties(XElement xElement)
		{
			this.Title = ((string)xElement.Element("Title")).DefaultIfNull(string.Empty);

			this.Header = ((string)xElement.Element("Header")).DefaultIfNull(string.Empty);

			this.Footer = ((string)xElement.Element("Footer")).DefaultIfNull(string.Empty);

			this.QueryName = ((string)xElement.Element("QueryName")).DefaultIfNull(string.Empty);

			this.QueryDescription = ((string)xElement.Element("QueryDescription")).DefaultIfNull(string.Empty);

			this.InitialPageSize = ((string)xElement.Element("InitialPageSize")).DefaultIfNull(string.Empty);

			this.TotalAllFields = Boolean.Parse(((string)xElement.Element("TotalAllFields")).DefaultIfNull("false"));

			this.IncludeLineNumbers = Boolean.Parse(((string)xElement.Element("IncludeLineNumbers")).DefaultIfNull("false"));

			this.ShowSummaryLinesOnly = Boolean.Parse(((string)xElement.Element("ShowSummaryLinesOnly")).DefaultIfNull("false"));

			this.QueryOnArchiveData = Boolean.Parse(((string)xElement.Element("QueryOnArchiveData")).DefaultIfNull("false"));

			string topicIDType = ((string)xElement.Element("TopicIDType")).DefaultIfNull(string.Empty);
			this.Topic = QueryWriterTopic.Get(null, topicIDType, queryAssemblies);

			this.ReadTransAliases(xElement);
		}


		private void ReadTransAliases(XElement xElement)
		{
			XElement aliasesElement = xElement.Element("TransactionAliasGuids");

			this.TransactionAliasGuids = new QueryWriterAliasGuidCollection();

			if (aliasesElement == null)
				return;

			// Get the field nodes
			var queryAliases = from F in aliasesElement.Descendants("QueryWriterAliasGuid")
									 select new
									 {
										 AliasGuid = (string)F.Element("AliasGuid")
									 };


			foreach (var alias in queryAliases)
			{
				Guid parsedGuid;

				if (Guid.TryParse(alias.AliasGuid, out parsedGuid))
				{
					var newAlias = new QueryWriterAliasGuid(parsedGuid);

					this.TransactionAliasGuids.Add(newAlias);
				}
			}

		}


		private Type GetFieldType(string fieldType)
		{
			Type returnValue = Type.GetType(fieldType);
			if (returnValue == null)
			{
				foreach (Assembly a in fmBusinessObjects)
				{
					returnValue = a.GetType(fieldType);
					if (returnValue != null)
					{
						break;
					}
				}

				if (returnValue == null)
				{
					returnValue = this.convertUnits.GetType(fieldType);
				}
			}

			if (returnValue == null)
			{
				throw new ApplicationException("Unable to load query object type.");
			}

			return returnValue;

		}

		private void ReadFields(XElement xElement)
		{
			XElement fieldsElement = xElement.Element("Fields");

			// Get the field nodes
			var queryFields = from F in fieldsElement.Descendants("QueryWriterField")
									select new
									{
										DisplayName = (string)F.Element("DisplayName"),
										ID = (string)F.Element("ID"),
										FieldType = (string)F.Element("FieldType"),
										DBFieldName = (string)F.Element("DBFieldName"),
										SecondaryDBFieldName = (string)F.Element("SecondaryDBFieldName"),
										GenerateSelect = (bool)F.Element("GenerateSelect"),
										FieldName = ((string)F.Element("ID")).Split(new char[] { '/' })[1]
									};

			this.Fields = new QueryWriterFieldCollection();

			foreach (var Field in queryFields)
			{
				var newField = new QueryWriterField
				{
					DisplayName = Field.DisplayName,
					FieldName = Field.FieldName,
					DBFieldName = Field.DBFieldName,
					SecondaryDBFieldName = Field.SecondaryDBFieldName,
					GenerateSelect = Field.GenerateSelect,
					FieldType = this.GetFieldType(Field.FieldType),
					Topic = this.Topic
				};

				this.Fields.Add(newField);
			}

		}


		private void ReadGroups(XElement xElement)
		{
			var Groups = from G in xElement.Descendants("Group")
							 select new GroupClass
							 {
								 ID = ((string)G.Element("ID")).DefaultIfNull(string.Empty)
							 };

			this.AssignedGroups = new GroupCollectionClass();

			foreach (GroupClass Group in Groups)
			{
				this.AssignedGroups.Add(Group);
			}

		}

		private void ReadCriteria(SecurityClass security, XElement xElement)
		{
			var newCriterion = from C in xElement.Descendants("QueryCriteriaPhrase")
									 select new
									 {
										 Value = ((string)C.Element("Value")).DefaultIfNull(string.Empty),
										 Type = (QueryCriteriaType)Enum.Parse(typeof(QueryCriteriaType), ((string)C.Element("Type")).DefaultIfNull("Phrase")),
										 Operator = (QueryOperator)Enum.Parse(typeof(QueryOperator), ((string)C.Element("Operator")).DefaultIfNull("Equals")),
										 Conjunction = (QueryAndOr)Enum.Parse(typeof(QueryAndOr), ((string)C.Element("Conjunction")).DefaultIfNull("AND")),
										 TopicObjectType = this.GetFieldType(((string)C.Element("TopicObjectType")).DefaultIfNull(string.Empty)),
										 FieldName = ((string)C.Element("FieldName")).DefaultIfNull(string.Empty),
										 DBFieldName = ((string)C.Element("DBFieldName")).DefaultIfNull(string.Empty)
									 };

			this.Criterion = new QueryCriteriaPhraseCollection();

			foreach (var criteriaObject in newCriterion)
			{
				var criteria = new QueryCriteriaPhrase
				{
					Value = criteriaObject.Value,
					Type = criteriaObject.Type,
					Operator = criteriaObject.Operator,
					Conjunction = criteriaObject.Conjunction,
					//TopicObjectType	= criteriaObject.TopicObjectType,
					Topic = QueryWriterTopic.Get(null, criteriaObject.TopicObjectType.ToString(), queryAssemblies)
				};

				// Use DBFieldName if it exists to avoid ambiguity between duplicate transaction and line item field names
				if (!string.IsNullOrEmpty(criteriaObject.DBFieldName))
				{
					criteria.Field = criteria.Topic.FindFieldByDbName(security, criteriaObject.DBFieldName, false);
				}
				else
				{
					var field = new QueryWriterField { Topic = criteria.Topic, FieldName = criteriaObject.FieldName };
					criteria.Field = criteria.Topic.FindFieldByID(security, field.ID, true);
				}

				this.Criterion.Add(criteria);
			}
		}

		private void ReadFilterGroups(XElement xElement)
		{
			var filterGroups = from FG in xElement.Descendants("QueryFilterGroup")
									 select new QueryFilterGroupClass
									 {
										 FilterID = ((string)FG.Element("FilterID")).DefaultIfNull(string.Empty),
										 Filter = Boolean.Parse(((string)FG.Element("Filter")).DefaultIfNull(string.Empty)),
										 DefaultValue1 = ((string)FG.Element("DefaultValue1")).DefaultIfNull(string.Empty),
										 DefaultValue2 = ((string)FG.Element("DefaultValue2")).DefaultIfNull(string.Empty),
										 DbFieldName = ((string)FG.Element("DBFieldName")).DefaultIfNull(string.Empty)
									 };

			this.FilterGroups = new QueryFilterGroupCollection();

			foreach (QueryFilterGroupClass filter in filterGroups)
			{
				this.FilterGroups.Add(filter);
			}
		}

		private void ReadDataGroups(XElement xElement)
		{
			// Find the DataGroups collection
			XElement dataGroupsElement = xElement.Element("DataGroups");

			if (dataGroupsElement != null)
			{
				var dataGroups = from F in dataGroupsElement.Descendants("QueryWriterField")
									  select new QueryWriterField
									  {
										  DisplayName = (string)F.Element("DisplayName"),
										  FieldName = ((string)F.Element("ID")).Split(new char[] { '/' })[1],
										  FieldType = this.GetFieldType((string)F.Element("FieldType")),
										  DBFieldName = (string)F.Element("DBFieldName"),
										  Topic = QueryWriterTopic.Get(null, ((string)F.Element("ID")).Split(new char[] { '/' })[0], queryAssemblies)
									  };

				this.DataGroups = new QueryWriterFieldCollection();

				foreach (QueryWriterField field in dataGroups)
				{
					this.DataGroups.Add(field);
				}
			}
		}

		public string SelectStatement(SecurityClass security)
		{
			string sql = "SELECT 0 as " + LINE_NUMBER;

			sql += ",'" + QueryRowType.DataRow + "' as " + ROW_TYPE;

			var includedFields = new QueryWriterFieldCollection();

			if (this.HasGroups)
			{
				sql += this.AddFieldToQueryStatement(includedFields, this.DataGroups[0]);

				if (this.DataGroups.Count > 1)
				{
					sql += this.AddFieldToQueryStatement(includedFields, this.DataGroups[1]);
				}

				if (this.DataGroups.Count > 2)
				{
					sql += this.AddFieldToQueryStatement(includedFields, this.DataGroups[2]);
				}

			}

			// build a list of virtual field names
			List<string> virtualFieldNames =
				 Fields.Where(x => string.IsNullOrEmpty(x.SecondaryDBFieldName) == false)
					  .Select(x => x.SecondaryDBFieldName)
					  .ToList();

			// only add non-virtual fields to SQL
			foreach (QueryWriterField Field in Fields.Where(x => virtualFieldNames.Contains(x.DBFieldName) == false))
			{
				sql += AddFieldToQueryStatement(includedFields, Field);
			}

			// Add in the filter fields in case they are not already part of the query statement
			foreach (QueryCriteriaPhrase phrase in this.Criterion)
			{
				if (phrase.Type == QueryCriteriaType.Phrase)
				{
					sql += this.AddFieldToQueryStatement(includedFields, phrase.Field);
				}
			}

			// Makes sure the fields for the user filters are included in the select statement
			foreach (QueryFilterGroupClass filterGroup in this.FilterGroups)
			{
				if (filterGroup.Filter)
				{
					QueryWriterField field = this.Topic.FindFieldByID(security, filterGroup.FilterID, false);
					if (field != null)
					{
						sql += this.AddFieldToQueryStatement(includedFields, field);
					}
				}
			}

			sql += " ";

			return sql;
		}

		private string AddFieldToQueryStatement(QueryWriterFieldCollection includedFields, QueryWriterField field)
		{
			if (field.GenerateSelect && includedFields.DBGet(field.DBFieldName) == null)
			{
				includedFields.Add(field);
				return "," + this.ProcessFieldName(field.DBFieldName);
			}

			return string.Empty;
		}

		public static bool IsDataRow(DataRow row)
		{
			return QueryRowType.DataRow.ToString().Equals(row[ROW_TYPE].ToString());
		}

		public string DataGroupStatement
		{
			get
			{
				if (this.DataGroups.Count == 0)
				{
					return String.Empty;
				}

				string sql = " ORDER BY ";

				int count = 1;
				foreach (QueryWriterField field in this.DataGroups)
				{
					string fieldName = field.DBFieldName.ToUpper();
					int nIndex = fieldName.IndexOf(" AS ");
					if (nIndex >= 0)
					{
						fieldName = fieldName.Substring(0, nIndex);
					}

					if (string.IsNullOrEmpty(this.Topic.PostQueryAliasName) == false)
					{
						fieldName = this.Topic.PostQueryAliasName + "." + fieldName;
					}

					sql += this.ProcessFieldNameWithoutAS(fieldName);

					if (count < this.DataGroups.Count)
					{
						sql += ",";
					}

					++count;
				}

				return sql;
			}
		}

		public string ProcessFieldNameWithoutAS(string fieldName)
		{
			int periodIndex = fieldName.IndexOf(".");

			if (periodIndex >= 0)
			{
				return fieldName.Substring(0, periodIndex) + ".[" + fieldName.Substring(periodIndex + 1) + "]";
			}

			return "[" + fieldName + "]";
		}

		protected string ProcessFieldName(string fieldName)
		{
			int periodIndex = fieldName.IndexOf(".");

			if (periodIndex >= 0)
			{
				return fieldName.Substring(0, periodIndex) + ".[" + fieldName.Substring(periodIndex + 1) + "] as '" + fieldName + "'";
			}

			return "[" + fieldName + "]";
		}

		public bool HasGroups { get { return this.DataGroups.Count > 0; } }

		public static void ApplyDataDictionary(SecurityClass Security, QueryWriterFieldCollection fieldCollection)
		{
			FMChannelHelper.MakeCall<IDataDictionariesClass>(
				dictionaries =>
					{
						foreach (QueryWriterField field in fieldCollection)
						{
							string translation = dictionaries.Get(Security.SiteGuid, field.DisplayName);
							if (string.IsNullOrEmpty(translation) == false)
							{
								field.DisplayName = translation;
							}
						}
					});
		}

		#region SQL Command with Parameters

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblQueryStorage " +
					"(SiteGuid," +
					"QueryName," +
					"QueryDescription," +
					"QueryXML," +
					"NavNodePath," +
					"SystemQuery," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"OwnerUserGuid," +
					"QueryStorageGuid)" +
					" VALUES (" +
					"@SiteGuid," +
					"@QueryName," +
					"@QueryDescription," +
					"@QueryXML," +
					"@NavNodePath," +
					"@SystemQuery," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@OwnerUserGuid," +
					"@QueryStorageGuid)";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@QueryName", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@QueryDescription", SqlDbType.NVarChar, 500);
			cmd.Parameters.Add("@QueryXML", SqlDbType.Text);
			cmd.Parameters.Add("@NavNodePath", SqlDbType.NVarChar, 255);
			cmd.Parameters.Add("@SystemQuery", SqlDbType.Bit);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@OwnerUserGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@QueryStorageGuid", SqlDbType.UniqueIdentifier);


			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
			cmd.Parameters["@QueryName"].Value = this.QueryName;
			cmd.Parameters["@QueryDescription"].Value = this.QueryDescription;
			cmd.Parameters["@QueryXML"].Value = Prescrub(this.GetXML().ToString());
			cmd.Parameters["@NavNodePath"].Value = this.NavNodePath;
			cmd.Parameters["@SystemQuery"].Value = this.SystemQuery ? 1 : 0; ;
			cmd.Parameters["@CreatedDate"].Value = this._CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this._CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
			cmd.Parameters["@OwnerUserGuid"].Value = this.OwnerUserGuid;
			cmd.Parameters["@QueryStorageGuid"].Value = this._IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblQueryStorage " +
				 " SET SiteGuid = @SiteGuid, " +
				 " QueryName = @QueryName, " +
				 " QueryDescription = @QueryDescription, " +
				 " QueryXML = @QueryXML, " +
				 " UpdatedDate = @UpdatedDate, " +
				 " UpdatedBy = @UpdatedBy, " +
				 " OwnerUserGuid = @OwnerUserGuid, " +
				 " NavNodePath = @NavNodePath, " +
				 " SystemQuery = @SystemQuery " +
				 " WHERE QueryStorageGuid = @QueryStorageGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@QueryName", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@QueryDescription", SqlDbType.NVarChar, 500);
			cmd.Parameters.Add("@QueryXML", SqlDbType.Text);
			cmd.Parameters.Add("@NavNodePath", SqlDbType.NVarChar, 255);
			cmd.Parameters.Add("@SystemQuery", SqlDbType.Bit);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@OwnerUserGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@QueryStorageGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;
			cmd.Parameters["@QueryName"].Value = this.QueryName;
			cmd.Parameters["@QueryDescription"].Value = this.QueryDescription;
			cmd.Parameters["@QueryXML"].Value = Prescrub(this.GetXML().ToString());
			cmd.Parameters["@NavNodePath"].Value = this.NavNodePath;
			cmd.Parameters["@SystemQuery"].Value = this.SystemQuery ? 1 : 0;
			cmd.Parameters["@UpdatedDate"].Value = this._UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
			cmd.Parameters["@OwnerUserGuid"].Value = this.OwnerUserGuid;
			cmd.Parameters["@QueryStorageGuid"].Value = this.IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblQueryStorage" +
				 " WHERE QueryStorageGuid = @QueryStorageGuid";

			cmd.Parameters.Add("@QueryStorageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@QueryStorageGuid"].Value = this.IdentityGuid;
		}

		public void EnumerateForUserPurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT tblQueryStorage.*" +
				" FROM tblQueryStorage" +
				" WHERE tblQueryStorage.OwnerUserGuid = @OwnerUserGuid ";

			cmd.Parameters.Add("@OwnerUserGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@OwnerUserGuid"].Value = this.OwnerUserGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblQueryStorage " + SQLUpdateLock(bInTransaction) +
			  " WHERE QueryStorageGuid = @QueryStorageGuid";

			cmd.Parameters.Add("@QueryStorageGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@QueryStorageGuid"].Value = this.IdentityGuid;
		}

		public void SelectByNameSQL(SqlCommand cmd, SecurityClass Security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblQueryStorage " + SQLUpdateLock(bInTransaction) +
			  " WHERE [QueryName] = @QueryName " +
			  " AND [SiteGuid] = @SiteGuid ";

			cmd.Parameters.Add("@QueryName", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@QueryName"].Value = this.QueryName;
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;

			if (Security.HasRight(RIGHT.CONFIGURE_QUERIES) == false)
			{
				cmd.CommandText += " AND OwnerUserGuid = @OwnerUserGuid";
				cmd.Parameters.Add("@OwnerUserGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@OwnerUserGuid"].Value = Security.UserGuid;
			}
		}


		public void SelectByNodePathSQL(SqlCommand cmd, SecurityClass Security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT * FROM tblQueryStorage " + SQLUpdateLock(bInTransaction) +
			  " WHERE [NavNodePath] = @NavNodePath " +
			  " AND [SiteGuid] = @SiteGuid ";

			cmd.Parameters.Add("@NavNodePath", SqlDbType.NVarChar, 255);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@NavNodePath"].Value = this.NavNodePath;
			cmd.Parameters["@SiteGuid"].Value = this._SiteGuid;

			if (Security.HasRight(RIGHT.CONFIGURE_QUERIES) == false)
			{
				cmd.CommandText += " AND OwnerUserGuid = @OwnerUserGuid";
				cmd.Parameters.Add("@OwnerUserGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@OwnerUserGuid"].Value = Security.UserGuid;
			}
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass Security)
		{
			cmd.CommandText = "SELECT tblQueryStorage.*,QGM.*" +
				" FROM tblQueryStorage" +
				 " LEFT OUTER JOIN map.tblQueryStorageToGroup QGM ON QGM.QueryStorageGuid = tblQueryStorage.QueryStorageGuid" +
				" LEFT OUTER JOIN map.tblUserToGroup G ON QGM.GroupGuid = G.GroupGuid AND G.UserGuid = @UserGuid AND G.SiteGuid = tblQueryStorage.SiteGuid" +
				" WHERE tblQueryStorage.SiteGuid = @SiteGuid";

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@UserGuid"].Value = Security.UserGuid;
			cmd.Parameters["@SiteGuid"].Value = Security.SiteGuid;

			if (Security.HasRight(RIGHT.CONFIGURE_QUERIES) == false)
			{
				cmd.CommandText += " AND (OwnerUserGuid = @OwnerUserGuid OR (QGM.QueryStorageGuid IS NOT NULL AND G.UserGuid = @OwnerUserGuid))";
				cmd.Parameters.Add("@OwnerUserGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@OwnerUserGuid"].Value = Security.UserGuid;
			}
			cmd.CommandText += " ORDER BY QueryName";
		}
		#endregion
	}
}
