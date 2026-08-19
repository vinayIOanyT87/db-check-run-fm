// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Extension methods for SyncDBI and related class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses.SyncClasses
{
	using System;
	using System.Data;
	using System.Data.Common;
	using System.Data.SqlClient;
	using System.IO;
	using System.Linq;
	using System.Runtime.Serialization.Formatters.Binary;

	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses.SyncClasses;
	using FMBusinessServices.InternalClasses.SyncClasses.Client;
	using FMBusinessServices.ServiceClasses;

	/// <summary>
	/// Extension methods for SyncDBI and related class
	/// </summary>
	public static class SyncDBIExtension
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
		public static object SetOptionalValue<T>(this SyncDBI dbi, T value)
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
		public static object SetOptionalValue<T>(this SyncDBI dbi, Nullable<T> value)
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
		public static T GetOutputValue<T>(this SyncDBI dbi, SqlParameter parameter)
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
		public static T? GetOutputValue<T>(this SyncDBI dbi, SqlParameter parameter, T defaultValue)
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
		public static bool IsParameterNull(this SyncDBI dbi, SqlParameter parameter)
		{
			return (null == parameter) || (parameter.Value == null || parameter.SqlValue == DBNull.Value);
		}
	}

	/// <summary>
	/// Summary description for SyncDBI.
	/// </summary>
	public abstract class SyncDBI : IDisposable
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
		internal ConsolidatedDAClass ConsolidatedDA
		{
			get { return this._ConsolidatedDA; }
		}
		#endregion Properties

		#region Constructors / Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SyncDBI"/> class.
		/// </summary>
		/// <param name="user">
		/// The user.
		/// </param>
		protected SyncDBI(string user)
		{
			this.User = user;

			this.CreatedBy = user;
			this.UpdatedBy = user;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="SyncDBI"/> class. 
		/// </summary>
		~SyncDBI()
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

			// Load the server data store ID
			string serverNodeId = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid);

			// If it hasn't been set yet and we're at this stage, go ahead and initialize it.
			if (string.IsNullOrEmpty(serverNodeId))
			{
				serverNodeId = Guid.NewGuid().ToString();
				configurationSettings.Modify(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeGuid, serverNodeId);
			}

			return new Guid(serverNodeId);
		}

		/// <summary>
		/// Returns the synchronization data source name
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GetServerNodeName(SecurityClass security)
		{
			ConfigurationSettingsClass configurationSettings = new ConfigurationSettingsClass();

			// Load the server data store Name
			string serverNodeName = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName);

			// If it hasn't been set yet and we're at this stage, go ahead and initialize it.
			if (string.IsNullOrEmpty(serverNodeName))
			{
				Random rand = new Random();

				serverNodeName = string.Format("{0}_{1}", Environment.MachineName, rand.Next(1000, 9999));

				configurationSettings.Modify(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncNodeName, serverNodeName);
			}

			return serverNodeName;
		}

		/// <summary>
		/// Gets the selected synchronization profile from the database.  Defaulting it to {Complete} if it hasn't been configured yet.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GetSelectedSynchronizationProfile(SecurityClass security)
		{
			ConfigurationSettingsClass configurationSettings = new ConfigurationSettingsClass();

			string syncProfileID = configurationSettings.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncProfileID);
			if (string.IsNullOrEmpty(syncProfileID))
			{
				syncProfileID = SyncConstants.DEFAULT_PROFILE_COMPLETE;
				configurationSettings.Modify(security, ConfigurationSettingDOClass.Key_InstallDetails_SyncProfileID, syncProfileID);
			}

			return syncProfileID;
		}

		/// <summary>
		/// The update session with synchronization node.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="remoteNodeGuid">
		/// The remote node guid.
		/// </param>
		public static void UpdateSessionWithSynchronizationNode(SecurityClass security, Guid remoteNodeGuid)
		{
			using (SqlConnection conn = SyncDBI.CreateServerConnection())
			{
				conn.Open();

				using (SqlCommand updateSessionCommand = conn.CreateCommand())
				{
					string updateSessionWithNode = "UPDATE tblSessions WITH(ROWLOCK) SET SynchronizationNodeGuid = @SynchronizationNodeGuid WHERE SessionGuid = @sessionGuid; ";

					updateSessionCommand.CommandType = CommandType.Text;
					updateSessionCommand.CommandText = updateSessionWithNode;
					updateSessionCommand.Parameters.AddWithValue("@SynchronizationNodeGuid", remoteNodeGuid);
					updateSessionCommand.Parameters.AddWithValue("@sessionGuid", security.Token);
					updateSessionCommand.ExecuteNonQuery();
				}
			}
		}

		public static void ResetUploadOnlySynchronizationScopes(SecurityClass security, string profileID)
		{
			using (SqlConnection conn = SyncDBI.CreateServerConnection())
			{
				conn.Open();

				using (SqlCommand sqlCommand = conn.CreateCommand())
				{
					sqlCommand.CommandTimeout = 1800;
					const string syncResetAnchorProc = "[sync].[usp_SyncSetUploadOnlyAnchorsAfterInitialSync]";

					sqlCommand.CommandType = CommandType.StoredProcedure;
					sqlCommand.CommandText = syncResetAnchorProc;
					sqlCommand.Parameters.AddWithValue("@ID", profileID);
					sqlCommand.ExecuteNonQuery();
				}
			}
		}

		public static void ReIndexAllTablesInSyncProfile(SecurityClass security)
		{
			using (SqlConnection conn = SyncDBI.CreateServerConnection())
			{
				conn.Open();

				using (SqlCommand reindexCommand = conn.CreateCommand())
				{
					reindexCommand.CommandTimeout = 1800;
					const string reIndexProc = "maint.usp_ReindexDatabase_UpdateStats";

					reindexCommand.CommandType = CommandType.StoredProcedure;
					reindexCommand.CommandText = reIndexProc;
					reindexCommand.ExecuteNonQuery();
				}
			}
		}

		public static long GetMaxSyncAnchor()
		{
			object anchorVal = null;

			using (SqlConnection conn = SyncDBI.CreateClientConnection())
			{
				conn.Open();

				IDbCommand cmd = SyncProviderHelperFM.CreateMaxAnchorCommand();

				if (SyncDBI.GetParameter(cmd, SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER) != null)
				{
					// parameter mode
					cmd.Connection = conn;
					cmd.ExecuteNonQuery();
					anchorVal =
						SqlExpressClientSyncProvider.GetSyncObjectOutParameter(
							SyncParamsFM.SYNC_NEW_RECEIVED_ANCHOR_PARAMETER,
							cmd);
				}
				else
				{
					// assume scalar mode
					anchorVal = cmd.ExecuteScalar();
				}
			}

			if ((anchorVal == null) || (anchorVal == System.DBNull.Value))
			{
				return 0;
			}
			else
			{
				return (long)anchorVal;
			}
		}
		#endregion Public Methods

		#region Internal Methods

		/// <summary>
		/// The create client connection.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlConnection"/>.
		/// </returns>
		internal static SqlConnection CreateClientConnection()
		{
			string connectionString = ConsolidatedDAClass.ConnectionString;

			return new SqlConnection(connectionString);
		}

		/// <summary>
		/// The create server connection.
		/// </summary>
		/// <returns>
		/// The <see cref="SqlConnection"/>.
		/// </returns>
		internal static SqlConnection CreateServerConnection()
		{
			string connectionString = ConsolidatedDAClass.ConnectionString;

			return new SqlConnection(connectionString);
		}

		/// <summary>
		/// duplicate of the function found in synchronization utility
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
		internal static object GetSyncObjectOutParameter(string parameter, IDbCommand command)
		{
			bool flag;
			return SyncDBI.GetSyncObjectOutParameter(parameter, command, out flag);
		}

		/// <summary>
		/// duplicate of the function found in synchronization utility
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
		internal static object GetSyncObjectOutParameter(string parameter, IDbCommand command, out bool found)
		{
			found = true;
			DbParameter parameter2 = SyncDBI.GetParameter(command, parameter);
			if (parameter2 != null)
			{
				return parameter2.Value;
			}
			found = false;
			return null;
		}

		/// <summary>
		/// duplicate of the function found in synchronization utility
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
		/// The serialize anchor value.
		/// </summary>
		/// <param name="anchorVal">
		/// The anchor val.
		/// </param>
		/// <returns>
		/// The <see>
		///         <cref>byte[]</cref>
		///     </see>
		///     .
		/// </returns>
		internal static byte[] SerializeAnchorValue(object anchorVal)
		{
			using (MemoryStream serializationStream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(serializationStream, anchorVal);
				return serializationStream.ToArray();
			}
		}

		/// <summary>
		/// The deserialize anchor value.
		/// </summary>
		/// <param name="anchor">
		/// The anchor.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		internal static object DeserializeAnchorValue(byte[] anchor)
		{
			using (MemoryStream serializationStream = new MemoryStream(anchor))
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
				throw new Exception("SyncDBI: ConvertRowVersion invalid length.");
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
