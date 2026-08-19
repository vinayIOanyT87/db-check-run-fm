// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDataObject.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Extension methods for BaseDataOBject and related class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.IO;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using FMBusinessObjects.Attributes;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.UtilityObjects;
	using System.Runtime.InteropServices;


	/// <summary>
	/// Extension methods for BaseDataOBject and related class
	/// </summary>
	public static class BaseDataObjectExtension
	{
		/// <summary>
		/// Test whether an object with the given Guid is in the list or not
		/// </summary>
		/// <param name="self"></param>
		/// <returns>True if empty and false otherwise</returns>
		public static bool ContainsGuid<BaseDataObjectType>(this List<BaseDataObjectType> self, Guid targetGuid)
				where BaseDataObjectType : BaseDataObject
		{
				return self.Exists(dataObject => dataObject.IdentityGuid == targetGuid);

		}

		/// <summary>
		/// Find the object with the given Guid in the given list
		/// </summary>
		/// <param name="self"></param>
		/// <returns>True if empty and false otherwise</returns>
		public static BaseDataObjectType FindByGuid<BaseDataObjectType>(this List<BaseDataObjectType> self, Guid targetGuid)
				where BaseDataObjectType : BaseDataObject
		{
				return self.SingleOrDefault<BaseDataObjectType>(dataObject => dataObject.IdentityGuid == targetGuid);

		}

	}

	[Serializable]
	[DataContract]
	public abstract class BaseDataObject : BaseObjectClass
	{
		public const string ADMIN = "Admin";

		private static readonly CachedPropertyDictionary CachedProperties = new CachedPropertyDictionary();

		private static readonly object CachedDataObjectLock = new object();

		[DataMember]
		protected Guid _IdentityGuid;
		[XmlIgnore]
		public Guid IdentityGuid { get { return this._IdentityGuid; } set { this._IdentityGuid = value; } }

		[DataMember]
		protected string _ID;
		[XmlIgnore]
		[FMPersistedField]
		virtual public string ID { get { return this._ID; } set { this._ID = value; } }

		[DataMember]
		protected DateTimeOffset _CreatedDate;
		[XmlIgnore]
		[FMPersistedField(AddOnly = true)]
		public DateTimeOffset CreatedDate { get { return this._CreatedDate; } set { this._CreatedDate = value; } }

		[DataMember]
		protected string _CreatedBy;
		[XmlIgnore]
		[FMPersistedField(AddOnly = true, DefaultValue = ADMIN)]
		public string CreatedBy { get { return this._CreatedBy; } set { this._CreatedBy = value; } }

		[DataMember]
		protected DateTimeOffset _UpdatedDate;
		[XmlIgnore]
		[FMPersistedField]
		public DateTimeOffset UpdatedDate { get { return this._UpdatedDate; } set { this._UpdatedDate = value; } }

		[DataMember]
		protected string _UpdatedBy;

		[XmlIgnore]
		[FMPersistedField(DefaultValue = ADMIN)]
		public string UpdatedBy { get { return this._UpdatedBy; } set { this._UpdatedBy = value; } }

		[DataMember]
		protected Guid _SiteGuid;

		[XmlIgnore]
		[FMPersistedField]
		virtual public Guid SiteGuid { get { return this._SiteGuid; } set { this._SiteGuid = value; } }

		[DataMember]
		protected string _SiteID;
		[XmlIgnore]
		virtual public string SiteID { get { return this._SiteID; } set { this._SiteID = value; } }

		[DataMember]
		protected bool _Deleted;
		[XmlIgnore]
		public bool Deleted { get { return this._Deleted; } set { this._Deleted = value; } }

		[DataMember]
		[XmlIgnore]
		[FMPersistedField(AlternateName = "_RowVersion", ReadOnly = true)]
		public Byte[] RowVersion { get; set; }

		[DataMember]
		[XmlIgnore]
		[FMPersistedField(AlternateName = "_ClusterIdx", ReadOnly = true)]
		virtual public long ClusteredIndex { get; set; }

		public static readonly Guid DUMMY_GUID = Guid.Empty;

		[XmlIgnore]
		public virtual ENTITY_TYPE EntityType { get { return ENTITY_TYPE.UNKNOWN; } set { ;} }

		[XmlIgnore]
		public virtual ENTITY_TYPE ParentEntityType { get { return ENTITY_TYPE.UNKNOWN; } set { ;} }

		static protected string SQLUpdateLock(bool bInTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
		    return string.Empty;
		}

	    protected BaseDataObject()
		{
		}

	    protected BaseDataObject(BaseDataObject baseDataObject)
		{
				this._ID = string.IsNullOrEmpty(baseDataObject.ID) ? string.Empty : string.Copy(baseDataObject.ID);
				this._IdentityGuid = baseDataObject.IdentityGuid;
				this._CreatedDate = baseDataObject.CreatedDate;
				this._CreatedBy = string.IsNullOrEmpty(baseDataObject.CreatedBy) ? string.Empty : string.Copy(baseDataObject.CreatedBy);
				this._UpdatedDate = baseDataObject.UpdatedDate;
				this._UpdatedBy = string.IsNullOrEmpty(baseDataObject.UpdatedBy) ? string.Empty : string.Copy(baseDataObject.UpdatedBy);
				this._SiteID = string.IsNullOrEmpty(baseDataObject.SiteID) ? string.Empty : string.Copy(baseDataObject.SiteID);
				this._SiteGuid = baseDataObject.SiteGuid;
				this._Deleted = baseDataObject.Deleted;
		}

		public virtual void Reset()
		{
				this._IdentityGuid = Guid.Empty;
				this._SiteGuid = Guid.Empty;
				this._ID = string.Empty;
				this._SiteID = string.Empty;
				this._CreatedDate = DateTimeOffset.Now;
				this._CreatedBy = ADMIN;
				this._UpdatedDate = this.CreatedDate;
				this._UpdatedBy = ADMIN;
				this._Deleted = false;
		}

		public string SiteFromJoinClause(SecurityClass security, string entityTable, string entityIndexColumn)
		{
				string SQL =
					String.Format(
						" FROM {0} A, {1} B WHERE A.{2}=B.{2} AND B.[SiteGuid]='{3}'",
						entityTable,
						EntityToSiteMapClass.GetMappingTableName(this.EntityType),
						entityIndexColumn,
						security.SiteGuid);

				return SQL;
		}

		// Don't use this one anymore, use the new one!!!
		virtual public string SiteWhereClause(SecurityClass security, string entityTable, string entityGuidColumn)
		{
				var SQL = " (" + entityTable + "." + entityGuidColumn + " IN (SELECT " + entityGuidColumn + " FROM " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + " WITH(NOLOCK) WHERE SiteGuid = '" + security.SiteGuid.ToString() + "'))";
				return SQL;
		}

		/// <summary>
		/// Same function as above except add to and existing command instead
		/// This will append SQL to the end.  Parameter name used will be @ + entityGuidColumn
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		/// <param name="entityTable"></param>
		/// <param name="entityGuidColumn"></param>
		virtual public string AppendSiteWhereClause(SqlCommand cmd, SecurityClass security, string entityTable, string entityGuidColumn)
		{
				const string ParamNameSiteguid1 = "@SiteGuid1";
				const SqlDbType ParamTypeSiteguid = SqlDbType.UniqueIdentifier;
				var sql = " (" + entityTable + "." + entityGuidColumn +
								" IN (SELECT " + entityGuidColumn +
								" FROM " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) +
								" WITH(NOLOCK) "
								+ DataObject.AddParameter(cmd, "WHERE", "SiteGuid", "=", ParamNameSiteguid1, ParamTypeSiteguid, security.SiteGuid) +
								"))";
				return sql;
		}

		/// <summary>
		/// Same function as AppendSiteWhereClause except it will not repeat @SiteGuid1, 2, 3
		/// This allows AppendSiteWhereClauseParameters to be called on the same SQLCommand object 
		/// multiple times.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		/// <param name="entityTable"></param>
		/// <param name="entityGuidColumn"></param>
		virtual public string AppendSiteWhereClauseParameters(SqlCommand cmd, SecurityClass security, string entityTable, string entityGuidColumn)
		{
				int NextParamater = cmd.Parameters.Count;
				string SQL = "";
				string PARAM_NAME_SITEGUID1 = "@SiteGuid" + NextParamater;
				SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;

				SQL = " (" + entityTable + "." + entityGuidColumn +
				" IN (SELECT " + entityGuidColumn +
				" FROM " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) +
				" WITH(NOLOCK) "
				+ DataObject.AddParameter(cmd, "WHERE", "SiteGuid", "=", PARAM_NAME_SITEGUID1, PARAM_TYPE_SITEGUID, security.SiteGuid) +
				"))";
				return SQL;
		}

		public void SetString(string propertyName, int limit, string value, ref string property)
		{
			if (string.IsNullOrEmpty(value))
			{
				property = string.Empty;
				return;
			}

            string interimValue = value.Trim();

			if (interimValue.Length > limit)
			{
				throw new Exception("[" + propertyName + "], [maximum length of] " + limit + " [exceeded]");
			}

			property = interimValue;
		}

		public void SetDate(string PropertyName, string Value, ref Date Property)
		{
				try
				{
					if (!String.IsNullOrEmpty(Value))
						Property.Value = TimeConverter.ToDate(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Parse(Value, Property.Format, DateTimeStyles.None), Property.StandardName));
					else
						Property.Value = DateTimeOffset.MinValue;
				}
				catch
				{
					throw new Exception("[" + PropertyName + "], [invalid date format]");
				}
		}

		public void SetDateAndTime(string PropertyName, string Value, ref DateAndTime Property)
		{
				try
				{
					if (!String.IsNullOrEmpty(Value))
						Property.Value = DateTimeOffset.Parse(Value, Property.Format);
					else
						Property.Value = DateTimeOffset.MinValue;
				}
				catch
				{
					throw new Exception("[" + PropertyName + "], [invalid date time format]");
				}
		}

		public void SetTime(string PropertyName, string Value, ref Time Property)
		{
				try
				{
					if (!String.IsNullOrEmpty(Value))
						Property.Value = DateTimeOffset.Parse(Value, Property.Format);
					else
						Property.Value = DateTimeOffset.MinValue;
				}
				catch
				{
					throw new Exception("[" + PropertyName + "], [invalid time format]");
				}
		}

		public void SetDouble(string PropertyName, string Value, ref double Property)
		{
				try
				{
					Property = Convert.ToDouble(Value);
				}
				catch
				{
					throw new Exception("[" + PropertyName + "], [invalid format]");
				}
		}

		public void SetByte(string PropertyName, string Value, ref byte Property)
		{
				try
				{
					Property = Convert.ToByte(Value);
				}
				catch
				{
					throw new Exception("[" + PropertyName + "], [invalid format]");
				}
		}

		public void SetInt(string PropertyName, string Value, ref int Property)
		{
				try
				{
					Property = Convert.ToInt32(Value);
				}
				catch
				{
					throw new Exception("[" + PropertyName + "], [invalid format]");
				}
		}

		public void SetSIDouble(string PropertyName, string Value, ref SIDouble Property)
		{
				try
				{
					Property.Value = Convert.ToDouble(Value, Property.Format);
				}
				catch (Exception e)
				{
					string msg = "[" + PropertyName + "] " + e.Message;
					throw new Exception(msg);
				}
		}

		public string GetSIDouble(string PropertyName, SIDouble Property)
		{
				try
				{
					return Property.ToString();
				}
				catch (Exception e)
				{
					string msg = "[" + PropertyName + "] " + e.Message;
					throw new Exception(msg);
				}

		}

		public void SetSIDifferential(string PropertyName, string Value, ref SIDifferential Property)
		{
				try
				{
					Property.Value = Convert.ToDouble(Value, Property.Format);
				}
				catch (Exception e)
				{
					string msg = "[" + PropertyName + "] " + e.Message;
					throw new Exception(msg);
				}
		}

		public string SetSIDifferential(string PropertyName, SIDifferential Property)
		{
				try
				{
					return Property.ToString();
				}
				catch (Exception e)
				{
					string msg = "[" + PropertyName + "] " + e.Message;
					throw new Exception(msg);
				}

		}

		public void SetDecimal(string PropertyName, string Value, ref FMDecimal Property)
		{
				try
				{
					Property.Value = Convert.ToDecimal(Value, Property.Format);
				}
				catch
				{
					throw new Exception("[" + PropertyName + "], [invalid decimal format]");
				}
		}

		public virtual void Load(Object O)
		{
				if (O == null)
				{
					throw new ArgumentNullException("Object");
				}

				if (typeof(XmlNode).IsInstanceOfType(O))
				{
					XmlNode Node = (XmlNode)O;
					PropertyInfo[] Properties = this.GetType().GetProperties();

					foreach (PropertyInfo Property in Properties)
					{
						XmlIgnoreAttribute xmlIgnoreAttribute = System.Attribute.GetCustomAttribute(Property, typeof(XmlIgnoreAttribute)) as XmlIgnoreAttribute;

						if (xmlIgnoreAttribute != null)
						{
								continue;
						}

						if (!Property.CanWrite)
						{
								continue;
						}

						XmlAttribute Attribute = Node.Attributes[Property.Name];

						if (Attribute != null)
						{
								if (Property.PropertyType.BaseType == typeof(Enum))
								{
									Property.SetValue(this, Enum.Parse(Property.PropertyType, Attribute.Value, true), null);
								}
								else if (Property.PropertyType == typeof(int))
								{
									if (!string.IsNullOrEmpty(Attribute.Value))
									{
										Property.SetValue(this, Convert.ChangeType(Attribute.Value, Property.PropertyType), null);
									}
								}
								else
								{
									Property.SetValue(this, Convert.ChangeType(Attribute.Value, Property.PropertyType), null);
								}
						}
					}

					return;
				}
		}

		public virtual void Store(object O)
		{
				if (O == null)
					throw new ArgumentNullException("Object");

				if (typeof(XmlNode).IsInstanceOfType(O))
				{
					XmlNode Node = (XmlNode)O;

					PropertyInfo[] Properties = this.GetType().GetProperties();

					foreach (PropertyInfo Property in Properties)
					{
						XmlIgnoreAttribute xmlIgnoreAttribute = System.Attribute.GetCustomAttribute(Property, typeof(XmlIgnoreAttribute)) as XmlIgnoreAttribute;
						if (xmlIgnoreAttribute != null)
								continue;

						if (!Property.CanWrite)
								continue;

						if (Property.GetValue(this, null) == null)
								continue;

						XmlAttribute Attribute = Node.OwnerDocument.CreateAttribute(Property.Name);
						Attribute.Value = Property.GetValue(this, null).ToString();
						Node.Attributes.Append(Attribute);
					}
				}

				else
					throw new Exception("Store Error - Invalid Object Type : " + O.GetType().ToString());

		}

		public void AutoGenerateInsertProcSQL(SqlCommand cmd, string procedureName)
		{
				this.AutoGenerateProcCall(cmd, procedureName, addOnly: true);
		}

		public void AutoGenerateModifyProcSQL(SqlCommand cmd, string procedureName)
		{
				this.AutoGenerateProcCall(cmd, procedureName, addOnly: false);
		}

		private void AutoGenerateProcCall(SqlCommand cmd, string procedureName, bool addOnly)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = procedureName;

			cmd.Parameters.Clear();

			PropertyInfo[] properties = this.GetType().GetProperties();

			var columnNames = new List<string>();

			foreach (PropertyInfo property in properties)
			{
				var value = property.GetValue(this);


				if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					if (property.GetValue(this, null) == null)
					{
							continue;
					}
				}

				var column = property.Name;

				var fieldAttribute = property.GetCustomAttributes(typeof(FMPersistedField), true).FirstOrDefault() as FMPersistedField;

				if (fieldAttribute != null)
				{
					// Skip if a readonly field
					if (fieldAttribute.ReadOnly)
					{
						continue;
					}

					// Only continue if addOnly matches 
					if (addOnly.Equals(false) && fieldAttribute.AddOnly)
					{
						continue;
					}

					if (String.IsNullOrEmpty(fieldAttribute.AlternateName) == false)
					{
						column = fieldAttribute.AlternateName;
					}

					columnNames.Add(column);

					if (property.PropertyType.IsEnum)
					{
						if (fieldAttribute.LiteralEnum)
						{
							cmd.Parameters.AddWithValue("@" + column, property.GetValue(this).ToString());
						}
						else
						{
							cmd.Parameters.AddWithValue("@" + column, (int) property.GetValue(this));
						}
					}
					else if (property.PropertyType == typeof(bool))
					{
						cmd.Parameters.AddWithValue("@" + column, (bool)property.GetValue(this) ? 1 : 0);
					}
					else if (property.PropertyType == typeof(object))
					{
						cmd.Parameters.Add("@" + column, SqlDbType.Variant).Value = property.GetValue(this);
					}
					else
					{
						cmd.Parameters.AddWithValue("@" + column, property.GetValue(this));
					}
				}
			}

			if (columnNames.Count == 0)
			{
				throw new Exception("No columns marked as persisted columns.");
			}
		}

		public void AutoLoad(DataRow row)
		{
				AutoLoad(this, row);
		}

		static public T AutoLoad<T>(DataRow row) where T : BaseDataObject, new()
		{
				var dataObject = new T();
				AutoLoad(dataObject, row, AutoLoadMismatchBehavior.Exception);
				return dataObject;
		}

		static public byte [] ObjectToByteArray(object o)
		{
			if (o != null)
			{
				if (o is byte)
					return BitConverter.GetBytes((byte)o);
				else if (o is bool)
					return BitConverter.GetBytes((bool)o);
				else if (o is short)
					return BitConverter.GetBytes((short)o);
				else if (o is int)
					return BitConverter.GetBytes((int)o);
				else if (o is long)
					return BitConverter.GetBytes((long)o);
				else if (o is char)
					return BitConverter.GetBytes((char)o);
				else if (o is ushort)
					return BitConverter.GetBytes((ushort)o);
				else if (o is uint)
					return BitConverter.GetBytes((uint)o);
				else if (o is ulong)
					return BitConverter.GetBytes((ulong)o);
				else if (o is float)
					return BitConverter.GetBytes((float)o);
				else if (o is double)
					return BitConverter.GetBytes((double)o);
				else if (o is string
				&& (o as string) != null)
				{
					byte[] bytes = new byte[(o as string).Length * sizeof(char)];
					System.Buffer.BlockCopy((o as string).ToCharArray(), 0, bytes, 0, bytes.Length);
					return bytes;
				}
				else if (o is DateTime)
					return BitConverter.GetBytes(((DateTime)o).Ticks);
				else
					return null;
			}
			else
			{
				return null;
			}

		}

		static public object ByteArrayToObject(byte [] data, VarEnum dataType)
		{
			if (data != null)
			{
				if (dataType == VarEnum.VT_I1)
					return BitConverter.ToChar(data, 0);
				else if (dataType == VarEnum.VT_BOOL)
					return BitConverter.ToBoolean(data, 0);
				else if (dataType == VarEnum.VT_I2)
					return BitConverter.ToUInt16(data, 0);
				else if (dataType == VarEnum.VT_UI2)
					return BitConverter.ToUInt16(data, 0);
				else if (dataType == VarEnum.VT_I4)
					return BitConverter.ToInt32(data, 0);
				else if (dataType == VarEnum.VT_UI4)
					return BitConverter.ToUInt32(data, 0);
				else if (dataType == VarEnum.VT_I8)
					return BitConverter.ToInt64(data, 0);
				else if (dataType == VarEnum.VT_UI8)
					return BitConverter.ToUInt64(data, 0);
				else if (dataType == VarEnum.VT_R4)
					return BitConverter.ToSingle(data, 0);
				else if (dataType == VarEnum.VT_R8)
					return BitConverter.ToDouble(data, 0);
				else if (dataType == VarEnum.VT_LPWSTR)
				{
					char[] chars = new char[data.Length / sizeof(char)];
					System.Buffer.BlockCopy(data, 0, chars, 0, data.Length);
					return new string(chars);
				}
				else if (dataType == VarEnum.VT_DATE)
					return new DateTime(BitConverter.ToInt64(data, 0));
			}

			return null;
		}

		public static byte[] SerializeObject(object o)
		{
			if(o == null)
			{
				return null;
			}

			var serializationStream = new MemoryStream();
			var formatter = new BinaryFormatter();
			formatter.Serialize(serializationStream, o);
			return serializationStream.ToArray();
		}

		public static object DeserializeObject(byte [] data)
		{
			if(data == null)
			{
				return null;
			}

			var deserializationStream = new MemoryStream(data);
			var formatter = new BinaryFormatter();
			return formatter.Deserialize(deserializationStream);
		}

		/// <summary>
		/// Automatically loads properties in the child object based on properties marked with the
		/// FMPersistedField attribute.
		/// </summary>
		/// <param name="dataObject">The data object to populate.</param>
		/// <param name="row">The data row to use to populate the object.</param>
		/// <param name="mismatchBehavior">Exception: Throw an exception if a field from the row (DataRow) cannot be matched with a field from the dataObject. Ignore: Ignore row fields that cannot be matched with a field from the dataObject. </param>
		static public void AutoLoad( object dataObject, DataRow row, AutoLoadMismatchBehavior mismatchBehavior = AutoLoadMismatchBehavior.Exception )
		{
			Dictionary<string, CachedPropertyInfo> properties = GetDataObjectPersistedProperties( dataObject );
			DataColumnCollection rowColumns = row.Table.Columns;

			foreach ( DataColumn rowColumn in rowColumns )
			{
				var columnName = rowColumn.ColumnName;

				CachedPropertyInfo cachedProperty;
				if ( properties.TryGetValue( columnName, out cachedProperty ) )
				{
					PropertyInfo property = cachedProperty.PropertyInfo;

					// If the row is null, use the pre-defined default value; otherwise, just use the C# default value untouched.
					if ( row.IsNull( columnName ) )
					{
						if ( cachedProperty.FMPersistedField.DefaultValue != null )
						{
							property.SetValue( dataObject, cachedProperty.FMPersistedField.DefaultValue );
						}
					}
					else
					{
						if (property.PropertyType.IsEnum)
						{
							var fieldAttribute = cachedProperty.FMPersistedField;

							if (fieldAttribute != null)
							{
								if (fieldAttribute.LiteralEnum)
								{
									property.SetValue(dataObject, Enum.Parse(property.PropertyType, row[columnName] as String));
								}
								else
								{
									property.SetValue(dataObject, row[columnName]);
								}
							}
							else
							{
								property.SetValue(dataObject, row[columnName]);
							}
						}
						else
						{
							property.SetValue(dataObject, row[columnName]);
						}
					}
				}
				else if ( mismatchBehavior != AutoLoadMismatchBehavior.Ignore )
				{
					throw new Exception( "AutoLoad data column not found: " + columnName );
				}
			}
		}
		
		/// <summary>
		/// Retrieves the list of properties for an Object that are marked with the FMPersistedField attribute and returns 
		/// it in a dictionary, where the key corresponds to the FMPersistedField.AlternateName if found, otherwise
		/// to the property name itself.
		/// The dictionary allows fast lookup of the properties of a data object against its applicable FMPersistedField 
		/// (AlternateName or regular property name).
		/// </summary>
		/// <param name="dataObject">The data object whose persisted properties are to be queried.</param>
		/// <returns></returns>
		static private Dictionary<string, CachedPropertyInfo> GetDataObjectPersistedProperties( object dataObject )
		{
			Dictionary<string, CachedPropertyInfo> result;

			lock ( CachedDataObjectLock )
			{
				if (CachedProperties.TryGetValue(dataObject.GetType(), out result) == false)
				{
					result = CacheDataObjectProperties(dataObject.GetType());
				}
			}

			return result;
		}

		/// <summary>
		/// Caches property data about FMPersistedField annotated properties.  Can be used to "warm-up" data types.
		/// </summary>
		/// <param name="dataType">The object type to cache.</param>
		/// <returns>The property information cached.</returns>
		static private Dictionary<string, CachedPropertyInfo> CacheDataObjectProperties( Type dataType )
		{
			PropertyInfo[] properties = dataType.GetProperties();
			var result = new Dictionary<string, CachedPropertyInfo>( properties.Count() );

			foreach ( PropertyInfo property in properties )
			{
				var fieldAttribute = property.GetCustomAttributes( typeof( FMPersistedField ), inherit: true ).FirstOrDefault() as FMPersistedField;

				// Only look at properties with the FMDatabaseField attribute
				if ( fieldAttribute != null )
				{
					var propertyColumnName = property.Name;

					if ( String.IsNullOrEmpty( fieldAttribute.AlternateName ) == false )
					{
						propertyColumnName = fieldAttribute.AlternateName;
					}

					var propInfo = new CachedPropertyInfo
					{
						PropertyInfo = property,
						FMPersistedField = fieldAttribute
					};

					result.Add( propertyColumnName, propInfo );
				}
			}

			CachedProperties[dataType] = result;

			return result;
		}

		/// <summary>
		/// Sets the Site, CreatedBy, CreatedDate, UpdatedBy, and UpdatedDate fields for object creation.
		/// </summary>
		/// <param name="security">Current security object.</param>
		public void SetCreationStamp(SecurityClass security)
		{
				this.CreatedBy = security.UserID;
				this.CreatedDate = DateTimeOffset.Now;
				this.UpdatedBy = security.UserID;
				this.UpdatedDate = this.CreatedDate;
				this.SiteGuid = security.SiteGuid;
		}

		/// <summary>
		/// Sets the UpdatedBy, and UpdatedDate fields for object modification.
		/// </summary>
		/// <param name="security">Current security object.</param>
		public void SetModifyStamp(SecurityClass security)
		{
				this.UpdatedBy = security.UserID;
				this.UpdatedDate = DateTimeOffset.Now;
		}

		/// <summary>
		/// Rows the version to string.
		/// </summary>
		/// <param name="rowVersion">The row version.</param>
		/// <returns></returns>
		public static Int64 RowVersionToInt64( byte[] rowVersion )
		{
			if (rowVersion == null || rowVersion.Length == 0)
			{
				return 0;
			}

			Debug.Assert(rowVersion.Length == 8);

			long value;

			try
			{
				Array.Reverse( rowVersion );
				value = BitConverter.ToInt64( rowVersion, 0 );
			}
			finally
			{
				Array.Reverse( rowVersion );
			}

			return value;
		}

		public static byte[] Int64ToRowVersion( Int64 rowVersionInt )
		{
			byte[] result = BitConverter.GetBytes( rowVersionInt );

			Array.Reverse(result);
			
			return result;
		}

		public static void GenerateGuidListTable(SqlCommand cmd, List<Guid> guidList)
		{
			var guidTable = new DataTable();
			guidTable.Columns.Add("Guid", typeof(Guid));
			foreach (var rowGuid in guidList)
			{
				var row = guidTable.NewRow();
				row[0] = rowGuid;

				guidTable.Rows.Add(row);
			}

			SqlParameter tableValuedParameter = cmd.Parameters.Add("@GuidTable", SqlDbType.Structured);
			tableValuedParameter.Value = guidTable;
			tableValuedParameter.TypeName = "dbo.GuidListType";
		}

		protected void BaseClone(BaseDataObject o)
		{
			o.RowVersion = new byte[this.RowVersion.Length];
			for (var i = 0; i < this.RowVersion.Length; i++)
			{
				o.RowVersion[i] = this.RowVersion[i];
			}
		}
	}

	[DataContract]
	[Serializable]
	public class FMBaseDataObjectWithUserData : BaseDataObject
	{
		// Additional Data
		[DataMember]
		[XmlIgnore]
		public UserDataClass UserData;

        public FMBaseDataObjectWithUserData()
            : base()
        {
        }

        public FMBaseDataObjectWithUserData(FMBaseDataObjectWithUserData fmbaseDataObjectWithUserData)
	        : base((BaseDataObject)fmbaseDataObjectWithUserData)
	    {
	    }

        protected bool UpdateFieldName(QueryWriterField userField, UserDataFieldCollectionClass userDataFieldCollection)
        {
            foreach (UserDataFieldClass Field in userDataFieldCollection)
            {
                if (Field.DbName.EndsWith(userField.FieldName))
                {
                    userField.DisplayName = Field.DisplayName;
                    return true;
                }
            }

            return false;
        }

	}
}
