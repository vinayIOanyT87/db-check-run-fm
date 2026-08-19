// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataObject.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DataObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;

    /// <summary>
    /// Abstract base class for all data objects.
    /// </summary>
    [DataContract]
    [Serializable]
    [KnownType(typeof(BaseTransactionDO))]
    [KnownType(typeof(TransactionDO))]
    [XmlType( Namespace = "http://varec.com/fmbusinessobjects", TypeName = "DataObject" )]
    public abstract class DataObject
    {
        #region private data members
        /// <summary>
        /// The serialized dataset.
        /// </summary>
        [DataMember] private byte[] serializedDataset;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public virtual ENTITY_TYPE EntityType
        {
            get { return ENTITY_TYPE.UNKNOWN; } 
        }
        #endregion

        #region Public Methods and Operators
        /// <summary>
        /// Adds a new output Guid SqlParameter using the given name. Used by Insert to get the new Guid back
        /// </summary>
        /// <param name="cmd">
        /// The SQLCommand object to modify.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name to which to add the guid value.
        /// </param>
        public static void AddGuidOutputParameter(SqlCommand cmd, string parameterName)
        {
            var guidParameter = new SqlParameter(parameterName, SqlDbType.UniqueIdentifier);
            guidParameter.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(guidParameter);
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type and value to the given SqlCommand This also returns the sql with the format prefix fieldName operatorString AliasName
        /// </summary>
        /// <param name="cmd">The SQLCommand object to modify.
        /// </param>
        /// <param name="prefix">
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <param name="set2DBNullIfEmpty">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddGuidParameter(SqlCommand cmd, string prefix, string paramName, Guid newValue, bool set2DBNullIfEmpty = false)
        {
            cmd.Parameters.Add(NewGuidParameter(paramName, newValue, set2DBNullIfEmpty));
            return " " + prefix + " " + paramName + " ";
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type and value to the given SqlCommand
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="paramName">
        /// The param Name.
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="newValue">
        /// </param>
        public static void AddParameter(SqlCommand cmd, string paramName, SqlDbType paramType, object newValue)
        {
            cmd.Parameters.Add(NewParameter(paramName, paramType, newValue));
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type, size and value to the given SqlCommand
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="paramName">
        /// The param Name.
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="newValue">
        /// </param>
        public static void AddParameter(SqlCommand cmd, string paramName, SqlDbType paramType, int size, object newValue)
        {
            cmd.Parameters.Add(NewParameter(paramName, paramType, size, newValue));
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type and value to the given SqlCommand This also returns the sql with the format [AND] fieldName = AliasName
        /// </summary>
        /// <param name="cmd">The SQLCommand object to modify.
        /// </param>
        /// <param name="prefixWithAnd">
        /// indicates whether the SQL should add "AND" as prefix 
        /// </param>
        /// <param name="fieldName">
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddParameter(
            SqlCommand cmd, bool prefixWithAnd, string fieldName, string paramName, object newValue)
        {
            return AddParameter(cmd, (prefixWithAnd ? " AND " : string.Empty) + fieldName + "=", paramName, newValue);
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type and value to the given SqlCommand This also returns the sql with the format [AND] fieldName = AliasName
        /// </summary>
        /// <param name="cmd">The SQLCommand object to modify.
        /// </param>
        /// <param name="prefixWithAnd">
        /// indicates whether the SQL should add "AND" as prefix 
        /// </param>
        /// <param name="fieldName">
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddParameter(
            SqlCommand cmd, bool prefixWithAnd, string fieldName, string paramName, SqlDbType paramType, object newValue)
        {
            return AddParameter(cmd, prefixWithAnd ? "AND" : string.Empty, fieldName, "=", paramName, paramType, newValue);
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type, size and value to the given SqlCommand This also returns the sql with the format [AND] fieldName = AliasName
        /// </summary>
        /// <param name="cmd">
        /// </param>
        /// <param name="prefixWithAnd">
        /// indicates whether the SQL should add "AND" as prefix 
        /// </param>
        /// <param name="fieldName">
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="paramSize">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddParameter(
            SqlCommand cmd, 
            bool prefixWithAnd, 
            string fieldName, 
            string paramName, 
            SqlDbType paramType, 
            int paramSize, 
            object newValue)
        {
            return AddParameter(
                cmd, prefixWithAnd ? "AND" : string.Empty, fieldName, "=", paramName, paramType, paramSize, newValue);
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type and value to the given SqlCommand This also returns the sql with the format prefix fieldName operatorString paramName
        /// </summary>
        /// <param name="cmd">
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="fieldName">
        /// </param>
        /// <param name="operatorString">
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddParameter(
            SqlCommand cmd, 
            string prefix, 
            string fieldName, 
            string operatorString, 
            string paramName, 
            SqlDbType paramType, 
            object newValue)
        {
            return AddParameter(
                cmd, string.Format("{0} {1} {2}", prefix, fieldName, operatorString), paramName, paramType, newValue);
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type, size and value to the given SqlCommand This also returns the sql with the format prefix fieldName operatorString paramName
        /// </summary>
        /// <param name="cmd">
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="fieldName">
        /// </param>
        /// <param name="operatorString">
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="paramSize">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddParameter(
            SqlCommand cmd, 
            string prefix, 
            string fieldName, 
            string operatorString, 
            string paramName, 
            SqlDbType paramType, 
            int paramSize, 
            object newValue)
        {
            return AddParameter(
                cmd, string.Format("{0} {1} {2}", prefix, fieldName, operatorString), paramName, paramType, paramSize, newValue);
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type and value to the given SqlCommand This also returns the sql with the format prefix paramName
        /// </summary>
        /// <param name="cmd">
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddParameter(
            SqlCommand cmd, string prefix, string paramName, SqlDbType paramType, object newValue)
        {
            cmd.Parameters.Add(NewParameter(paramName, paramType, newValue));
            return " " + prefix + " " + paramName + " ";
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type and value to the given SqlCommand This also returns the sql with the format prefix paramName
        /// </summary>
        /// <param name="cmd">
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddParameter(SqlCommand cmd, string prefix, string paramName, object newValue)
        {
            cmd.Parameters.AddWithValue(paramName, newValue);
            return " " + prefix + " " + paramName + " ";
        }

        /// <summary>
        /// This method adds a new SqlParameter using the given name, type and value to the given SqlCommand This also returns the sql with the format prefix fieldName operatorString AliasName
        /// </summary>
        /// <param name="cmd">
        /// </param>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <param name="paramName">
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="paramSize">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string AddParameter(
            SqlCommand cmd, string prefix, string paramName, SqlDbType paramType, int paramSize, object newValue)
        {
            cmd.Parameters.Add(NewParameter(paramName, paramType, paramSize, newValue));
            return " " + prefix + " " + paramName + " ";
        }

        /// <summary>
        /// This method creates a new SqlParameter using the given name and value
        /// </summary>
        /// <param name="paramName">
        /// The param Name.
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <param name="set2DBNullIfEmpty">
        /// The set 2 DB Null If Empty.
        /// </param>
        /// <returns>
        /// The System.Data.SqlClient.SqlParameter.
        /// </returns>
        public static SqlParameter NewGuidParameter(string paramName, Guid newValue, bool set2DBNullIfEmpty = false)
        {
            var param = new SqlParameter(paramName, SqlDbType.UniqueIdentifier);
            if (set2DBNullIfEmpty && newValue == Guid.Empty)
            {
                param.Value = DBNull.Value;
            }
            else
            {
                param.Value = newValue;
            }

            return param;
        }

        /// <summary>
        /// This method creates a new SqlParameter using the given name, type and value
        /// </summary>
        /// <param name="paramName">
        /// The param Name.
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.Data.SqlClient.SqlParameter.
        /// </returns>
        public static SqlParameter NewParameter(string paramName, SqlDbType paramType, object newValue)
        {
            var param = new SqlParameter(paramName, paramType);
            param.Value = newValue;
            return param;
        }

        /// <summary>
        /// This method creates a new SqlParameter using the given name, type, size and value
        /// </summary>
        /// <param name="paramName">
        /// The param Name.
        /// </param>
        /// <param name="paramType">
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        /// <param name="newValue">
        /// </param>
        /// <returns>
        /// The System.Data.SqlClient.SqlParameter.
        /// </returns>
        public static SqlParameter NewParameter(string paramName, SqlDbType paramType, int size, object newValue)
        {
            var param = new SqlParameter(paramName, paramType, size);
            param.Value = newValue;
            return param;
        }

        /// <summary>
        /// This method will determine if the row has a null value. If so, then returns a boolean row.
        /// </summary>
        /// <param name="row">
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public static bool getBool(object row)
        {
            return (!isNull(row)) && (bool)row;
        }

        public static char getChar(object row)
        {
            if (isNull(row) || row.ToString().Length == 0)
            {
                return '\0';
            }

            return row.ToString()[0];
        }

        /// <summary>
        /// This method will determine if the row has a null value. If so, then a double doubleing point zero is returned. Else, the double doubleing point is returned.
        /// </summary>
        /// <param name="row">
        /// </param>
        /// <returns>
        /// The System.Double.
        /// </returns>
        public static double getDouble(object row)
        {
            if (isNull(row))
            {
                return 0.0;
            }

            return (double)row;
        }

        /// <summary>
        /// This method will determine if the row has a null value. If so, then a double point zero is returned. Else, the double point is returned.
        /// </summary>
        /// <param name="row">
        /// </param>
        /// <returns>
        /// The System.Double.
        /// </returns>
        public static double getFloat(object row)
        {
            if (isNull(row))
            {
                return 0.0F;
            }

            return (float)row;
        }

        /// <summary>
        /// Returns the Guid from the specified column if it exists in the data set.
        /// Otherwise it returns Guid.Empty;
        /// </summary>
        /// <param name="row">The DataRow to search.</param>
        /// <param name="columnName">The column name to search for.</param>
        /// <returns>Returns the Guid from the specified column if it exists in the data set; otherwise, Guid.Empty.</returns>
        public static Guid getGuid(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                return getGuid(row[columnName]);
            }

            return Guid.Empty;
        }

        public static Guid getGuid(object columnValue)
        {
            if (isNull(columnValue))
            {
                return Guid.Empty;
            }

            return (Guid)columnValue;
        }

        /// <summary>
        /// This method will determine if the row has a null value. If so, then an integer zero is returned. Else, the actual value is returned.
        /// </summary>
        /// <param name="row">
        /// </param>
        /// <returns>
        /// The System.Int32.
        /// </returns>
        public static int getInt(object row)
        {
            if (isNull(row))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(row);
            }
        }

        public static long getLong(object row)
        {
            if (isNull(row))
            {
                return 0;
            }

            return (long)row;
        }

        public static bool? getOptionalBool(object row)
        {
            if (isNull(row))
            {
                return null;
            }

            return (bool)row;
        }

        public static DateTimeOffset? getOptionalDateTimeOffset(object row)
        {
            if (isNull(row))
            {
                return null;
            }

            return (DateTimeOffset)row;
        }

        public static double? getOptionalDouble(object row)
        {
            if (isNull(row))
            {
                return null;
            }

            return (double)row;
        }

        public static int? getOptionalInt(object row)
        {
            if (isNull(row))
            {
                return null;
            }

            return Convert.ToInt32(row);
        }

        public static byte[] getOptionalVarBinary(object row)
        {
            if (isNull(row))
            {
                return null;
            }

            return (byte[])row;
        }

        /// <summary>
        /// This method will determine if the row has a null value. If so, then the method will return an empty string.
        /// </summary>
        /// <param name="row">
        /// The row to check.
        /// </param>
        /// <returns>
        /// The object as a string or String.Empty if the row is null.
        /// </returns>
        public static string getString(object row)
        {
            if (isNull(row))
                return null;

            return (string)row;
        }

        /// <summary>
        /// Returns the columnValue as an object of the specified type. If the columnValue is null then returns the specified default value.
        /// </summary>
        /// <typeparam name="T">
        /// type to convert columnValue to 
        /// </typeparam>
        /// <param name="columnValue">
        /// value to convert 
        /// </param>
        /// <param name="defaultValue">
        /// default value if columnValue is null 
        /// </param>
        /// <returns>
        /// The T.
        /// </returns>
        public static T getValue<T>(object columnValue, T defaultValue)
        {
            if (isNull(columnValue))
            {
                return defaultValue;
            }

            if (typeof(T).IsEnum && columnValue is string)
            {
                return (T) Enum.Parse(typeof(T), columnValue.ToString());
            }

            return (T)columnValue;
        }

        public static T getValue<T>(DataRow row, string columnName, T defaultValue)
        {
            if (row.Table.Columns.Contains(columnName) == false)
            {
                return defaultValue;
            }

            return getValue(row[columnName], defaultValue);
        }

        /// <summary>
        /// This method will decrypt a value stored in a varbinary column using the standard encryption/decryption routine.
        /// </summary>
        /// <typeparam name="T">
        /// type to convert columnValue to 
        /// </typeparam>
        /// <param name="columnValue">
        /// value to convert 
        /// </param>
        /// <param name="saltValue">
        /// salt value required in order to decrypt the data correctly
        /// </param>
        /// <param name="defaultValue">
        /// default value if columnValue is null 
        /// </param>
        public static T GetEncryptedValue<T>(object columnValue, Guid saltValue, T defaultValue)
        {
            if (isNull(columnValue))
            {
                return defaultValue;
            }

            return (T)(DataObject.getValue(columnValue == DBNull.Value ? (object)defaultValue : UserClass.decode((byte[])columnValue, saltValue), (object)defaultValue));
        }

        /// <summary>
        /// This method will serialize the dataset object to be used later for comparison.
        /// </summary>
        /// <param name="dataset">
        /// The dataset.
        /// </param>
        protected void SerializeData ( DataSet dataset )
        {
            if ( dataset != null )
            {
                var binFormatter = new BinaryFormatter ( );
                var memoryStream = new MemoryStream ( );

                binFormatter.Serialize ( memoryStream, dataset );
                this.serializedDataset = memoryStream.ToArray ( );
            }
        }

        /// <summary>
        /// Same function as above except add to and existing command instead
        /// This will append SQL to the end.  Parameter name used will be @ + entityGuidColumn
        /// </summary>
        /// <param name="cmd">SQL Command
        /// </param>
        /// <param name="security">Security object
        /// </param>
        /// <param name="entityTable">entity table name
        /// </param>
        /// <param name="entityGuidColumn">GUID column of the entity
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        protected virtual string AppendSiteWhereClause(SqlCommand cmd, SecurityClass security, string entityTable, string entityGuidColumn)
        {
            const string ParmNameSiteGuid = "@SiteGuid1";
            const SqlDbType ParmTypeSiteGuid = SqlDbType.UniqueIdentifier;

            string sql = " (" + entityTable + "." + entityGuidColumn +
                        " IN (SELECT " + entityGuidColumn +
                        " FROM " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) +
                        " WITH(NOLOCK) "
                        + AddParameter(cmd, "WHERE", "SiteGuid", "=", ParmNameSiteGuid, ParmTypeSiteGuid, security.SiteGuid) +
                        "))";

            return sql;
        }

        /// <summary>
        /// The append site where clause.
        /// </summary>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="entityTable">
        /// The entity table.
        /// </param>
        /// <param name="entityGuidColumn">
        /// The entity guid column.
        /// </param>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        protected virtual string AppendSiteWhereClause(SqlCommand cmd, SecurityClass security, string entityTable, string entityGuidColumn, ENTITY_TYPE entityType)
        {
            const string ParmNameSiteGuid = "@SiteGuid1";
            const SqlDbType ParmTypeSiteGuid = SqlDbType.UniqueIdentifier;

            string sql = " (" + entityTable + "." + entityGuidColumn +
                        " IN (SELECT " + entityGuidColumn +
                        " FROM " + EntityToSiteMapClass.GetMappingTableName(entityType) +
                        " WITH(NOLOCK) "
                        + AddParameter(cmd, "WHERE", "SiteGuid", "=", ParmNameSiteGuid, ParmTypeSiteGuid, security.SiteGuid) +
                        "))";

            return sql;
        }

        /// <summary>
        /// This method de-serializes the internal dataset and converts it back to an 
        /// object.
        /// </summary>
        /// <returns>Returns a deserialized object.
        /// /// </returns>
        protected object DeserializeData ( )
        {
            if ( this.serializedDataset == null )
            {
                return null;
            }

            var memoryStream = new MemoryStream ( );
            var binFormmatter = new BinaryFormatter ( );

            memoryStream.Write ( this.serializedDataset, 0, this.serializedDataset.Length );
            memoryStream.Seek ( 0, SeekOrigin.Begin );
            object deserializedObj = ( object ) binFormmatter.Deserialize ( memoryStream );

            if ( deserializedObj == null )
            {
                throw new Exception ( "Compare object is null.  Cannot build update statement." );
            }

            return deserializedObj;
        }

        /// <summary>
        /// This method will compare the current property values with the old values. If there
        /// are changes the name of the property is added to a change list.
        /// </summary>
        /// <param name="currentObj">
        /// The current Obj.
        /// </param>
        /// <param name="oldObj">
        /// The old Obj.
        /// </param>
        /// <returns>
        /// A change list that contains the properties that have changed.
        /// </returns>
        protected List<string> GetChangedColumns ( DataObject currentObj, DataObject oldObj )
        {
            var changedProperties = new List<string>( );

            PropertyInfo[] currentProperties = currentObj.GetType ( ).GetProperties ( );
            PropertyInfo[] oldProperties = oldObj.GetType ( ).GetProperties ( );

            foreach ( PropertyInfo currentProperty in currentProperties )
            {
                // Ignore the created/updated by/Date properties. They are handed in the derived
                // data objects.
                if ( currentProperty.Name.Equals("CreatedBy") || currentProperty.Name.Equals("CreatedDate") ||
                     currentProperty.Name.Equals("UpdatedBy") || currentProperty.Name.Equals("UpdatedDate") )
                {
                    continue;
                }

                foreach ( PropertyInfo oldProperty in oldProperties )
                {
                    if ( currentProperty.Name.Equals ( oldProperty.Name ) )
                    {
                        var currentValue = currentProperty.GetValue ( currentObj, null );
                        var oldValue = oldProperty.GetValue ( oldObj, null );

                        if ( currentValue == null && oldValue != null )
                        {
                            changedProperties.Add(currentProperty.Name);
                        }
                        
                        if ( currentValue != null && currentValue.Equals ( oldValue ) == false )
                        {
                            changedProperties.Add ( currentProperty.Name );
                        }

                        break;
                    }
                }
            }

            return changedProperties;
        }

        /// <summary>
        /// Tests if the object is null.
        /// </summary>
        /// <param name="o">
        /// The object to test.
        /// </param>
        /// <returns>
        /// Boolean indicating if object is null.
        /// </returns>
        public static bool isNull(object o)
        {
            return o == DBNull.Value;
        }

        public virtual void GetDeleteCommand(SqlCommand cmd)
        {
        }

        public virtual void GetInsertCommand(SqlCommand cmd)
        {
        }

        public virtual void GetSelectCommand(SqlCommand cmd)
        {
        }

        public virtual void GetUpdateCommand(SqlCommand cmd)
        {
        }

        public abstract string getDeleteCommand();

        public abstract string getInsertCommand();

        public abstract string getSelectCommand();

        public abstract string getUpdateCommand();

        public void load(DataSet dataSet)
        {
        }

        #endregion
    }
}