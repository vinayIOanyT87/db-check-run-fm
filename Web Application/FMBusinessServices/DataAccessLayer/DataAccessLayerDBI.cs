// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataAccessLayerDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Extension methods for SyncDBI and related class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.DataAccessLayer
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.ServiceClasses;

    /// <summary>
    /// Extension methods for DataAccessLayerDBI and related class
    /// </summary>
    public static class DataAccessLayerDBIExtension
    {
        /// <summary>
        /// If the value is null, a DBNull.Value is return. Typically passing in a value of null to a command parameter will result in
        /// the parameter being exclude from the underlying command execution.  To avoid this, a DBNull.Value should be returned which will
        /// result in a NULL being sent to the database.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="dbi">
        /// The dbi.
        /// </param>
        /// <param name="value">
        /// Value to set
        /// </param>
        /// <returns>
        /// DBNull.Value if the input value is null; otherwise the original value is returned.
        /// </returns>
        public static object SetOptionalValue<T>(this DataAccessLayerDBI dbi, T value)
            where T : class
        {
            object retValue = value;

            if (value == null)
            {
                retValue = DBNull.Value;
            }

            return retValue;
        }

        /// <summary>
        /// If the value is null, a DBNull.Value is return. Typically passing in a value of null to a command parameter will result in
        /// the parameter being exclude from the underlying command execution.  To avoid this, a DBNull.Value should be returned which will
        /// result in a NULL being sent to the database.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="dbi">
        /// The dbi.
        /// </param>
        /// <param name="value">
        /// Value to set
        /// </param>
        /// <returns>
        /// DBNull.Value if the input value is null; otherwise the original value is returned.
        /// </returns>
        public static object SetOptionalValue<T>(this DataAccessLayerDBI dbi, Nullable<T> value)
            where T : struct
        {
            return value.HasValue ? value.Value : (object)DBNull.Value;
        }

        /// <summary>
        /// If the value is null, a DBNull.Value is return. Typically passing in a value of null to a command parameter will result in
        /// the parameter being exclude from the underlying command execution.  To avoid this, a DBNull.Value should be returned which will
        /// result in a NULL being sent to the database.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="dbi">
        /// The dbi.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// DBNull.Value if the input value is null; otherwise the original value is returned.
        /// </returns>
        public static T GetOutputValue<T>(this DataAccessLayerDBI dbi, SqlParameter parameter)
            where T : class
        {
            T retValue = default(T);

            if (!dbi.IsParameterNull(parameter))
            {
                retValue = (T)parameter.Value;
            }

            return retValue;
        }

        /// <summary>
        /// If the value is null, a DBNull.Value is return. Typically passing in a value of null to a command parameter will result in
        /// the parameter being exclude from the underlying command execution.  To avoid this, a DBNull.Value should be returned which will
        /// result in a NULL being sent to the database.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="dbi">
        /// The dbi.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="defaultValue">
        /// The default Value.
        /// </param>
        /// <returns>
        /// DBNull.Value if the input value is null; otherwise the original value is returned.
        /// </returns>
        public static T? GetOutputValue<T>(this DataAccessLayerDBI dbi, SqlParameter parameter, T defaultValue)
            where T : struct
        {
            T retValue = default(T);

            if (!dbi.IsParameterNull(parameter))
            {
                retValue = (T)parameter.Value;
            }

            return retValue;
        }

        /// <summary>
        /// The is parameter null.
        /// </summary>
        /// <param name="dbi">
        /// The dbi.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsParameterNull(this DataAccessLayerDBI dbi, SqlParameter parameter)
        {
            return (null == parameter) || (parameter.Value == null || parameter.SqlValue == DBNull.Value);
        }
    }

    /// <summary>
    /// Summary description for DataAccessLayerDBI.
    /// </summary>
    public abstract class DataAccessLayerDBI : IDisposable
    {
        #region Attributes

        /// <summary>
        /// The _ disposed.
        /// </summary>
        private bool _Disposed = false;

        /// <summary>
        /// The _ consolidated da.
        /// </summary>
        private ConsolidatedDAClass _ConsolidatedDA = new ConsolidatedDAClass();

        #endregion Attributes

        #region Properties

        /// <summary>
        /// Gets the user.
        /// </summary>
        protected string User { get; private set; }

        /// <summary>
        /// Gets or sets the created date time.
        /// </summary>
        protected DateTimeOffset CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        protected string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the updated date time.
        /// </summary>
        protected DateTimeOffset UpdatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        protected string UpdatedBy { get; set; }

        /// <summary>
        /// Gets the consolidated da.
        /// </summary>
        protected ConsolidatedDAClass ConsolidatedDA
        {
            get { return this._ConsolidatedDA; }
        }
        #endregion Properties

        #region Constructors / Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccessLayerDBI"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        protected DataAccessLayerDBI(string user)
        {
            this.User = user;

            this.CreatedBy = user;
            this.UpdatedBy = user;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DataAccessLayerDBI"/> class. 
        /// </summary>
        ~DataAccessLayerDBI()
        {
            this.Dispose(false);
        }
        #endregion Constructors / Destructors

        #region Initialization
        #endregion Initialization

        #region Abstract members
        abstract protected SqlCommand PrepareSelectStatement();
        abstract protected SqlCommand PrepareInsertStatement();
        abstract protected SqlCommand PrepareUpdateStatement();
        abstract protected SqlCommand PrepareDeleteStatement();
        #endregion Abstract members

        #region Public Static Methods

        /// <summary>
        /// The get server node id.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static Guid GetServerNodeID(SecurityClass security)
        {
            ConfigurationSettingsClass configurationSettings = new ConfigurationSettingsClass();

            string serverNodeId = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid);
            string serverNodeName = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName);
            if (string.IsNullOrEmpty(serverNodeId))
            {
                serverNodeId = Guid.NewGuid().ToString();
                configurationSettings.Modify(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid, serverNodeId);
                configurationSettings.Modify(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName, Environment.MachineName);
            }

            else if (string.IsNullOrEmpty(serverNodeName))
            {
                configurationSettings.Modify(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName, Environment.MachineName);
            }

            else if (serverNodeName != Environment.MachineName)
            {
                serverNodeId = Guid.NewGuid().ToString();
                configurationSettings.Modify(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid, serverNodeId);
                configurationSettings.Modify(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName, Environment.MachineName);
            }

            return new Guid(serverNodeId);
        }
        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// The create default sql connection.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlConnection"/>.
        /// </returns>
        internal static SqlConnection CreateDefaultConnection()
        {
            string connectionString = ConsolidatedDAClass.ConnectionString;

            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// The create connection using specified connection string.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlConnection"/>.
        /// </returns>
        internal static SqlConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Gets the value of the specified named output parameter
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal static object GetOutParameter(string parameter, IDbCommand command)
        {
            bool flag;
            return DataAccessLayerDBI.GetOutParameter(parameter, command, out flag);
        }

        /// <summary>
        /// Gets the value of the specified named output parameter
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="found">
        /// The found.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal static object GetOutParameter(string parameter, IDbCommand command, out bool found)
        {
            found = true;
            DbParameter retParameter = DataAccessLayerDBI.GetParameter(command, parameter);
            if (retParameter != null)
            {
                return retParameter.Value;
            }
            found = false;
            return null;
        }

        /// <summary>
        /// Locates a parameter in the specified Sql Command
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameterName">
        /// The parameter name.
        /// </param>
        /// <returns>
        /// The <see cref="DbParameter"/>.
        /// </returns>
        internal static DbParameter GetParameter(IDbCommand command, string parameterName)
        {
            if (command != null)
            {
                if (command.Parameters.Contains("@" + parameterName))
                {
                    return (DbParameter)command.Parameters["@" + parameterName];
                }
                if (command.Parameters.Contains(":" + parameterName))
                {
                    return (DbParameter)command.Parameters[":" + parameterName];
                }
                if (command.Parameters.Contains(parameterName))
                {
                    return (DbParameter)command.Parameters[parameterName];
                }
            }
            return null;
        }

        /// <summary>
        /// The serialize rowversion value.
        /// </summary>
        /// <param name="dbRowVersion">
        /// The RowVersion value from the database
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal static byte[] SerializeRowVersion(object dbRowVersion)
        {
            using (var serializationStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(serializationStream, dbRowVersion);
                return serializationStream.ToArray();
            }
        }

        /// <summary>
        /// The deserialize row version byte array
        /// </summary>
        /// <param name="rowVersionBytes">
        /// The anchor.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        internal static object DeserializeRowVersion(byte[] rowVersionBytes)
        {
            using (var serializationStream = new MemoryStream(rowVersionBytes))
            {
                return new BinaryFormatter().Deserialize(serializationStream);
            }
        }

        internal static long ConvertRowVersion(byte[] rowVersion)
        {
            var swappedBytes = new byte[8];
            if (rowVersion.Length == 8)
            {
                swappedBytes[0] = rowVersion[7];
                swappedBytes[1] = rowVersion[6];
                swappedBytes[2] = rowVersion[5];
                swappedBytes[3] = rowVersion[4];
                swappedBytes[4] = rowVersion[3];
                swappedBytes[5] = rowVersion[2];
                swappedBytes[6] = rowVersion[1];
                swappedBytes[7] = rowVersion[0];

            }
            else if (rowVersion.Length == 4)
            {
                swappedBytes[0] = rowVersion[3];
                swappedBytes[1] = rowVersion[2];
                swappedBytes[2] = rowVersion[1];
                swappedBytes[3] = rowVersion[0];
                swappedBytes[4] = 0;
                swappedBytes[5] = 0;
                swappedBytes[6] = 0;
                swappedBytes[7] = 0;
            }
            else
            {
                throw new Exception("DataAccessLayerDBI: ConvertRowVersion invalid length.");
            }
            return BitConverter.ToInt64(swappedBytes, 0);
        }

        #endregion Internal Methods

        #region Virtual Methods

        /// <summary>
        /// The prepare upsert statement.
        /// </summary>
        /// <returns>
        /// The <see cref="SqlCommand"/>.
        /// </returns>
        protected virtual SqlCommand PrepareUpsertStatement()
        {
            return null;
        }

        #endregion Virtual Methods

        #region IDisposable Interface Implementation
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this._Disposed == false)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                }

                // Note disposing has been done.
                _Disposed = true;
            }
        }
        #endregion IDisposable Interface Implementation
    }
}
