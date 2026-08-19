namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;

	using FMCore;

   [Serializable]
    public class QueryWriterTopicCollection : List<QueryWriterTopic> { }

	
	[AttributeUsage(AttributeTargets.Class)]
	[DebuggerDisplay("DisplayName={DisplayName}")]
	[DataContract]
	[Serializable]
	public class QueryWriterTopic : Attribute
	{
		[DataMember]
		private string associateTopicTypeName;
		public Type AssociatedTopicType
		{
			get
			{
				if (string.IsNullOrEmpty(associateTopicTypeName))
				{
					return null;
				}

				return Type.GetType(associateTopicTypeName);
			}
			set
			{
				associateTopicTypeName = value.FullName;
			}
		}

		[DataMember]
		public QueryWriterAliasGuidCollection AliasGuids { get; set; }

		[DataMember]
		public Guid? TransTypeAliasID { get; set; }

		[DataMember]
		public QueryWriterFieldCollection Fields { get; set; }

		[DataMember]
		public string PostQueryAliasName { get; set; }

		[DataMember]
		public bool UseDataDictionary { get; set; }

		[DataMember]
		public bool SupportsArchiveQuery { get; set; }

		[DataMember]
		protected string displayName = string.Empty;
		public string DisplayName
		{
			get
			{
				return displayName;
			}
		}

		[DataMember]
		protected string objectType = null;
		public Type ObjectType
		{
			get
			{
				if (objectType != null)
				{
					Type returnType = Type.GetType(objectType);
					return returnType;
				}

				return null;
			}
		}

		public QueryWriterTopic(Type objectType, string displayName)
		{
			this.objectType = objectType.AssemblyQualifiedName;
			this.displayName = displayName;
			this.UseDataDictionary = true;
			this.SupportsArchiveQuery = false;
		}

		public QueryWriterTopic(Type objectType, string displayName, Type assocTopicType, string postQueryAliasName)
		{
			this.objectType = objectType.AssemblyQualifiedName;
			this.displayName = displayName;
			this.AssociatedTopicType = assocTopicType;
			this.PostQueryAliasName = postQueryAliasName;
			this.SupportsArchiveQuery = false;
		}

		public QueryWriterFieldCollection GetFields(SecurityClass Security, bool filterFields)
		{
			if (Fields == null)
			{
				this.Fields = GetFieldsInternal(Security, filterFields);
			}

			List<QueryWriterField> secondaryFields = this.GetSecondaryFieldsFromObjectProperties(Fields);
			List<string> primaryFieldNames = new List<string>();
			foreach (QueryWriterField field in Fields)
			{
				primaryFieldNames.Add(field.DBFieldName);
			}

			List<QueryWriterField> fieldsToAdd = new List<QueryWriterField>();

			foreach (QueryWriterField field in secondaryFields)
			{
				if (primaryFieldNames.Contains(field.DBFieldName) == false)
				{
					fieldsToAdd.Add(field);
				}
			}

			Fields.AddRange(fieldsToAdd.ToList());

			return this.Fields;
		}

		protected QueryWriterFieldCollection GetFieldsInternal(SecurityClass Security, bool filterFields)
		{
			QueryWriterFieldCollection Fields = new QueryWriterFieldCollection();

			PropertyInfo[] Properties = ObjectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (PropertyInfo Property in Properties)
			{
				foreach (QueryWriterField Field in Property.GetCustomAttributes(typeof(QueryWriterField), false))
				{
					Field.FieldName = Property.Name;
					Field.SetFieldType(Property.PropertyType.AssemblyQualifiedName);
					Field.Topic = this;

					Fields.Add(Field);
				}
			}

			// Also get any associated topics (like Line Items to Transactions)
			if (AssociatedTopicType != null)
			{
				PropertyInfo[] AssociatedProperties = AssociatedTopicType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				foreach (PropertyInfo Property in AssociatedProperties)
				{
					foreach (QueryWriterField Field in Property.GetCustomAttributes(typeof(QueryWriterField), false))
					{
						Field.FieldName = Property.Name;
						Field.SetFieldType(Property.PropertyType.AssemblyQualifiedName);
						Field.Topic = this;

						Fields.Add(Field);
					}
				}
			}

			if (filterFields)
			{
				MethodInfo sqlMethod = this.ObjectType.GetMethod("QueryAliasFields");

				if (sqlMethod != null)
				{
					object mainObject = Activator.CreateInstance(this.ObjectType);

					object[] parameters = { Security, Fields };

					if (sqlMethod.GetParameters().Count() == 3)
					{
						parameters = new object[] { Security, Fields, this.TransTypeAliasID };
					}

					Fields = (QueryWriterFieldCollection)sqlMethod.Invoke(mainObject, parameters);
				}
			}

			return Fields;
		}

		private List<QueryWriterField> GetSecondaryFieldsFromObjectProperties(QueryWriterFieldCollection Fields)
		{
			PropertyInfo[] Properties =
			    ObjectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			// build a list of virtual field names
			List<string> virtualFieldNames =
			    Fields.Where(x => string.IsNullOrEmpty(x.SecondaryDBFieldName) == false)
				   .Select(x => x.SecondaryDBFieldName)
				   .ToList();

			List<QueryWriterField> virtualFields = new List<QueryWriterField>();

			foreach (PropertyInfo Property in Properties)
			{
				// add a new field for each virtual field
				foreach (QueryWriterField Field in
				    Property.GetCustomAttributes(typeof(QueryWriterField), false)
					   .Where(x => virtualFieldNames.Contains(((QueryWriterField)x).DBFieldName)))
				{
					foreach (QueryWriterField f in Fields)
					{
						if (f.SecondaryDBFieldName == Field.DBFieldName)
						{
							Field.FieldName = Property.Name;
							Field.DisplayName = "~" + Property.Name;
							Field.SetFieldType(Property.PropertyType.AssemblyQualifiedName);
							Field.Topic = this;
							//Field.IsVirtualField = true;

							virtualFields.Add(Field);
						}
					}
				}
			}
			return virtualFields;
		}

		/// <summary>
		/// This method returns a collection of QueryWriterTopicSecurity attributes if any are applied to the 
		/// ObjectType of this QueryWriterTopic.  If none are applied, it means that security rights should
		/// not be considered in the availability of the topic.
		/// </summary>
		/// <returns>a QueryWriterTopicSecurityCollection object</returns>
		public QueryWriterTopicSecurityCollection GetSecurityRights()
		{
			var topicSecurityCollection = new QueryWriterTopicSecurityCollection();

			foreach (QueryWriterTopicSecurity topicSecurity in this.ObjectType.GetCustomAttributes(typeof(QueryWriterTopicSecurity), true))
			{
				topicSecurityCollection.Add(topicSecurity);
			}

			return topicSecurityCollection;
		}

		public QueryWriterField FindFieldByDbName(SecurityClass Security, string dbFieldName, bool filterFields)
		{
			QueryWriterFieldCollection fields = GetFields(Security, filterFields);

			foreach (QueryWriterField field in fields)
			{
				if (field.DBFieldName.Equals(dbFieldName))
				{
					return field;
				}
			}

			return null;
		}

		public QueryWriterField FindFieldByID(SecurityClass Security, string ID, bool filterFields)
		{
			QueryWriterFieldCollection fields = this.GetFields(Security, filterFields);

			foreach (QueryWriterField field in fields)
			{
				if (field.ID.Equals(ID))
				{
					return field;
				}
			}

			return null;
		}

		static public QueryWriterTopic Get(SecurityClass Security, string objectType, string assemblies)
		{
			var topics = Enumerate(Security, assemblies);


			var Topic = (from T in topics
					   where T.ObjectType.ToString() == objectType
					   select T)
				    .DefaultIfEmpty(null)
				    .FirstOrDefault();

			//the below section is used to load old queries that were not migrated in the xml to use the new client class
			if (Topic == null)
			{
				try
				{
					var classToCheckType = Type.GetType(objectType);

					if (classToCheckType != null)
					{

						Topic = (from T in topics
							    where T.ObjectType.IsSubclassOf(classToCheckType)
							    select T)
				    .DefaultIfEmpty(null)
				    .FirstOrDefault();
					}
				}
				catch
				{
					Topic = null;
				}
			}

			return Topic as QueryWriterTopic;
		}



		private static QueryWriterTopicCollection cachedTypeCollection = null;

		static public QueryWriterTopicCollection Enumerate(SecurityClass Security, string assemblies)
		{
			CheckSecurity(Security);

			if (cachedTypeCollection == null)
			{
				LoadCache(Security, assemblies);
			}

			QueryWriterTopicCollection queryTypeCollection = new QueryWriterTopicCollection();

			foreach (QueryWriterTopic queryWriterTopic in cachedTypeCollection)
			{
				QueryWriterTopicSecurityCollection securityCollection = queryWriterTopic.GetSecurityRights();

				if (securityCollection.HasRights(Security))
				{
					queryTypeCollection.Add(queryWriterTopic);
				}
			}

			return queryTypeCollection;
		}


		private static void LoadCache(SecurityClass Security, string assemblies)
		{
			cachedTypeCollection = new QueryWriterTopicCollection();

			try
			{
				//string assemblyPath = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(Security, QUERYWRITER_SETTING));

				// Parse the list of assemblies
				char[] separator = { ';' };
				string[] sssemList = assemblies.Split(separator, StringSplitOptions.RemoveEmptyEntries);

				// Go through all the assemblies
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				foreach (string assemblyName in sssemList)
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
									string message = "Assembly Load Error in Query Topic Load Cache. " + ex.Message;
									FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
								}
							}

							if (dll != null)
								AssemblyDictionary.Add(assemblyName.ToLower(), dll);
						}
						else
						{
							dll = AssemblyDictionary.Get(assemblyName.ToLower());
						}

						if (dll == null)
						{
							continue;
						}

						Type[] types;

						try
						{
							types = dll.GetTypes();

							foreach (Type Module in types)
							{
								GetQueryTypes(Module, cachedTypeCollection);
							}
						}
						catch
						{
							continue;
						}

					}
					catch
					{

					}
				}
			}
			catch (ReflectionTypeLoadException reflectionException)
			{
				throw new ApplicationException(BuildLoadExceptionMessage(reflectionException));
			}
		}

		private static void GetQueryTypes(Type Module, QueryWriterTopicCollection queryTypeCollection)
		{
			foreach (QueryWriterTopic queryWriterTopic in Module.GetCustomAttributes(typeof(QueryWriterTopic), true))
			{
				if (queryTypeCollection.Count(x => queryWriterTopic.ObjectType.IsAssignableFrom(x.ObjectType)) > 0)
				{
					continue; //don't want parent classes added since there is a subclass already added that overrides the parent class
				}

				queryTypeCollection.RemoveAll(x => queryWriterTopic.ObjectType.IsSubclassOf(x.ObjectType)); //remove parent class since this one will override

				queryTypeCollection.Add(queryWriterTopic);
			}
		}

		static private string BuildLoadExceptionMessage(ReflectionTypeLoadException reflectionException)
		{
			if (reflectionException == null)
			{
				throw new ArgumentNullException();
			}

			string Message = reflectionException.Message;

			foreach (Exception except in reflectionException.LoaderExceptions)
			{
				Message += "\n" + "===========" + "\n" + except.Message;
			}

			return Message;

		}

		static private void CheckSecurity(SecurityClass Security)
		{
		}

	}

	[Serializable]
    public class QueryWriterTopicSecurityCollection : List<QueryWriterTopicSecurity>
    {
        /// <summary>
        /// This method will return true if the collection is empty or if the security
        /// object passed contains one of the rights in this collection.
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public bool HasRights(SecurityClass security)
        {
            if (this.Count == 0 || security == null)
            {
                return true;
            }

            foreach (QueryWriterTopicSecurity topicSecurity in this)
            {
                if (security.HasRight(topicSecurity.SecurityRight))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class QueryWriterTopicSecurity : Attribute
    {
        public RIGHT SecurityRight { get; set; }

        public QueryWriterTopicSecurity(RIGHT securityRight)
        {
            SecurityRight = securityRight;
        }
    }

    [Serializable]
    [CollectionDataContract]
    public class QueryWriterAliasGuidCollection : List<QueryWriterAliasGuid>
    {

    }

    [XMLObject(NodeName = "QueryWriterAliasGuid")]
    [DataContract]
    [Serializable]
    public class QueryWriterAliasGuid : IEquatable<QueryWriterAliasGuid>
    {
        [XMLProperty]
        [DataMember(EmitDefaultValue = false)]
        public Guid AliasGuid { get; set; }

        public QueryWriterAliasGuid(string guid)
        {
            Guid tmp;

            if (Guid.TryParse(guid, out tmp))
            {
                AliasGuid = tmp;
            }
        }

        public QueryWriterAliasGuid(Guid guid)
        {
            AliasGuid = guid;
        }

        public override string ToString()
        {
            return AliasGuid.ToString();
        }

        public bool Equals(QueryWriterAliasGuid obj)
        {
            //this function is needed for checking if it exists in a list
            return this.AliasGuid.Equals(obj.AliasGuid);
        }

        public override int GetHashCode()
        {
            return AliasGuid.GetHashCode();
        }
    }

    [Serializable]
    [CollectionDataContract]
    public class QueryWriterFieldCollection : List<QueryWriterField>
    {
        public QueryWriterFieldCollection() : base() { }

        public QueryWriterFieldCollection(QueryWriterFieldCollection Fields)
           : base()
        {
            foreach (QueryWriterField Field in Fields)
            {
                Add(new QueryWriterField(Field));
            }
        }

        public QueryWriterField Get(string ID)
        {
            var field = (from F in this
                         where F.ID == ID
                         select F)
                     .DefaultIfEmpty(null)
                     .FirstOrDefault();

            return field as QueryWriterField;

        }

        public QueryWriterField DBGet(string DBID)
        {
            var field = (from F in this
                         where F.DBFieldName == DBID
                         select F)
                     .DefaultIfEmpty(null)
                     .FirstOrDefault();

            return field as QueryWriterField;
        }

        public void Swap(int Index1, int Index2)
        {
            QueryWriterField Object = this[Index1];
            this[Index1] = this[Index2];
            this[Index2] = Object;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    [DebuggerDisplay("Name = {ID}")]
    [XMLObject(NodeName = "QueryWriterField")]
    [DataContract]
    [Serializable]
    public class QueryWriterField : Attribute, IComparable
    {
        /// <summary>
        /// Flag indicating whether the field should be included in the select statement
        /// generated.  If false, Query Writer will assume the data object will generate
        /// the field in its processing of the select statement.
        /// </summary>
        [XMLProperty]
        [DataMember]
        public bool GenerateSelect { get; set; }

        [XMLProperty]
        [DataMember]
        public string DBFieldName { get; set; }

        [XMLProperty]
        [DataMember]
        public string SecondaryDBFieldName { get; set; }

        [XMLProperty]
        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        private string fieldTypeName;

        [XMLProperty]
        public Type FieldType
        {
            get
            {
                return Type.GetType(fieldTypeName);
            }
            set
            {
                this.fieldTypeName = value.AssemblyQualifiedName;
            }
        }

        [DataMember]
        protected string _fieldName = string.Empty;
        public string FieldName
        {
            get
            {
                return _fieldName;
            }
            set
            {
                if (DBFieldName.DefaultIfNull(string.Empty).Equals(string.Empty))
                {
                    DBFieldName = value;
                }

                _fieldName = value;

            }
        }

        [XMLProperty]
        public string ID
        {
            get
            {
                return Topic.ObjectType.ToString() + "/" + FieldName;
            }
        }

        [DataMember]
        public QueryWriterTopic Topic { get; set; }

        public void SetFieldType(string fullName)
        {
            fieldTypeName = fullName;
        }

        public string EnumFieldName
        { get { return this.DBFieldName + "-ENUMVALUE"; } }

        public QueryWriterField(string displayName, string dbFieldName, string secondaryDBFieldName = "")
        {
            Reset();
            DisplayName = displayName;
            DBFieldName = dbFieldName;
            SecondaryDBFieldName = secondaryDBFieldName;
    }

        public QueryWriterField(string displayName)
        {
            Reset();
            DisplayName = displayName;
        }

        public QueryWriterField(string displayName, bool bGenerateSelect)
        {
            Reset();
            DisplayName = displayName;
            GenerateSelect = bGenerateSelect;
        }

        public QueryWriterField(string displayName, string dbFieldName, bool bGenerateSelect)
        {
            Reset();
            DisplayName = displayName;
            DBFieldName = dbFieldName;
            GenerateSelect = bGenerateSelect;
        }

        public QueryWriterField(QueryWriterField Field)
        {
            Reset();
            DisplayName = Field.DisplayName;
            GenerateSelect = Field.GenerateSelect;
            DBFieldName = Field.DBFieldName;
            SecondaryDBFieldName = Field.SecondaryDBFieldName;
            FieldType = Field.FieldType;
            FieldName = Field.FieldName;
            Topic = Field.Topic;
        }

        public QueryWriterField()
        {
            Reset();
        }

        private void Reset()
        {
            DisplayName = string.Empty;
            GenerateSelect = true;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(QueryWriterField))
            {
                QueryWriterField Field = (QueryWriterField)obj;
                return this.DisplayName.CompareTo(Field.DisplayName);
            }

            throw new ArgumentException("Object is not a QueryWriterField");
        }

        public bool IsDateOnlyType()
        {
            if (FieldType == typeof(Date))
            {
                return true;
            }

            return false;
        }

        public bool IsDateType()
        {
            if (FieldType == typeof(System.DateTimeOffset?)
               || FieldType == typeof(System.DateTimeOffset)
               || FieldType == typeof(System.DateTime?)
               || FieldType == typeof(System.DateTime)
               || FieldType == typeof(DateAndTime)
               || FieldType == typeof(FMBusinessObjects.DataObjects.Date))
            {
                return true;
            }

            return false;
        }
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    [DefaultProperty("NodeName")]
    public class XMLObject : Attribute
    {
        public string NodeName { get; set; }

        public XMLObject()
        {
            this.NodeName = string.Empty;
        }
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    [DefaultProperty("NodeName")]
    public class XMLProperty : Attribute
    {
        public string NodeName { get; set; }

        public XMLProperty()
        {
            NodeName = string.Empty;
        }
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    [DefaultProperty("NodeName")]
    public class XMLCollection : Attribute
    {
        public string NodeName { get; set; }

        public XMLCollection()
        {
            NodeName = string.Empty;
        }

    }

    // import export attribute class
    [AttributeUsage(AttributeTargets.Field |
           AttributeTargets.Property,
           AllowMultiple = true)]
    public class EntityImportExportAttribute : Attribute, IComparable
    {
        public string ColumnName { get; set; }

        public int ColumnWidth { get; set; }

        public string MemberName { get; set; }

        public int Order { get; set; }

        public string Value { get; set; }

        public string XMLColumnName
        {
            get
            {
                return this.ColumnName;
            }

            set
            {
                this.ColumnName = value;
            }
        }

        public int XMLColumnWidth
        {
            get
            {
                return ColumnWidth;
            }
            set
            {
                ColumnWidth = value;
            }
        }

        public string XMLMemberName
        {
            get
            {
                return MemberName;
            }
            set
            {
                MemberName = value;
            }
        }

        public int XMLOrder
        {
            get
            {
                return this.Order;
            }

            set
            {
                this.Order = value;
            }
        }

        public EntityImportExportAttribute(string columnAndMemberName, int columnwidth)
        {
            this.Reset();
            this.ColumnName = columnAndMemberName;
            this.ColumnWidth = columnwidth;
            this.MemberName = columnAndMemberName;
        }

        public EntityImportExportAttribute(string columnname, int columnwidth, string membername)
        {
            this.Reset();
            this.ColumnName = columnname;
            this.ColumnWidth = columnwidth;
            this.MemberName = membername;
        }

        public EntityImportExportAttribute(string columnname, int columnwidth, string membername, string value)
        {
            this.Reset();
            this.ColumnName = columnname;
            this.ColumnWidth = columnwidth;
            this.MemberName = membername;
            this.Value = value;
        }

        public EntityImportExportAttribute(string columnname, int columnwidth, string membername, int order)
        {
            this.Reset();
            this.ColumnName = columnname;
            this.ColumnWidth = columnwidth;
            this.MemberName = membername;
            this.Order = order;
        }

        public EntityImportExportAttribute()
        {
            this.Reset();
        }

        private void Reset()
        {
            this.ColumnName = string.Empty;
            this.ColumnWidth = 0;
            this.MemberName = string.Empty;
            this.Order = 0;
            this.Value = string.Empty;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(EntityImportExportAttribute))
            {
                var field = (EntityImportExportAttribute)obj;
                return this.Order.CompareTo(field.Order);
            }

            throw new ArgumentException("Object Compare Failed");
        }
    }

    // import export XML worksheet attribute class
    [AttributeUsage(AttributeTargets.Class |
           AttributeTargets.Field |
           AttributeTargets.Property,
           AllowMultiple = false)]
    public class EntityImportExportWorksheetAttribute : Attribute//, IComparable
    {
        public string WorksheetName { get; set; }

        public string RootId { get; set; }


        // Type of the items contained in the list
        public Type TypeOfListItem { get; set; }

        public string XMLWorksheetName
        {
            get
            {
                return WorksheetName;
            }
            set
            {
                WorksheetName = value;
            }
        }

        public EntityImportExportWorksheetAttribute(string worksheetname)
        {
            Reset();
            WorksheetName = worksheetname;
        }

        public EntityImportExportWorksheetAttribute(string worksheetname, Type typeOfListItem)
        {
            Reset();
            WorksheetName = worksheetname;
            TypeOfListItem = typeOfListItem;
        }


        public EntityImportExportWorksheetAttribute(string worksheetname, string rootId)
        {
            Reset();
            WorksheetName = worksheetname;
            RootId = rootId;
        }


        public EntityImportExportWorksheetAttribute()
        {
            Reset();
        }

        private void Reset()
        {
            this.WorksheetName = string.Empty;
            this.TypeOfListItem = null;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() == typeof(QueryWriterField))
            {
                var field = (EntityImportExportWorksheetAttribute)obj;
                return string.Compare(this.WorksheetName, field.WorksheetName, StringComparison.Ordinal);
            }

            throw new ArgumentException("Object Compare Failed");
        }
    }
}
