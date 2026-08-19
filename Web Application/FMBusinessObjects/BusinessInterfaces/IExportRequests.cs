// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExportRequests.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IExportRequests interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Runtime.Serialization;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for ExportRequests.  Provides a database interface for the
	/// ExportRequestClass type and the custom aviation export interfaces.
	/// </summary>
	[ServiceContract]
	public interface IExportRequests
	{
		/// <summary>
		/// Gets a list of ExportRequestClass objects from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <returns>The specified list of ExportRequestClass objects</returns>
		[OperationContract]
		List<ExportRequestClass> GetRequests(SecurityClass security);

		/// <summary>
		/// Gets a table of in-memory data from the database.  Executes the
		/// specified SQL command and returns the resultant DataTable.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The serializable SQL Command</param>
		/// <returns>The DataTable containing the results of the specified SQL command</returns>
		[OperationContract]
		DataTable GetDataTable(SecurityClass security, SerializableSqlCommand cmd);

		/// <summary>
		/// Executes the specified SQL command and returns the number of rows affected.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The serializable SQL Command</param>
		/// <returns>The number of rows affected</returns>
		[OperationContract]
		int ExecuteQuery(SecurityClass security, SerializableSqlCommand cmd);

		/// <summary>
		/// Executes the specified SQL command and returns the first column of the first row
		/// in the result set returned by the query. Additional columns or rows are ignored.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The serializable SQL Command</param>
		/// <returns>The first column of the first row of the results of the specified SQL command</returns>
		[OperationContract]
		object ExecuteScalar(SecurityClass security, SerializableSqlCommand cmd);

		/// <summary>
		/// Adds an ExportRequestClass object to the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="exportRequest">The object to add to the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, ExportRequestClass exportRequest);

		/// <summary>
		///  Modifies an existing ExportRequestClass object in the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="exportRequest">The object to modify in the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Update(SecurityClass security, ExportRequestClass exportRequest);

		/// <summary>
		/// Deletes an existing ExportRequestClass object from the database.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="identityGuid">Identifies the object to delete in the database</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Delete(SecurityClass security, Guid identityGuid);

		/// <summary>
		/// Gets an existing ExportRequestClass object from the database given the ID.
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="id">The ID of the ExportRequestClass object</param>
		/// <returns>The specified ExportRequestClass object</returns>
		[OperationContract]
		ExportRequestClass GetRequestById(SecurityClass security, string id);

		/// <summary>
		/// Gets an existing ExportRequestClass object from the database given the Identity Guid (ExportRequestGuid)
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="identityGuid">The identity guid identifying the ExportRequestClass record</param>
		/// <returns>The specified ExportRequestClass record</returns>
		[OperationContract]
		ExportRequestClass Get(SecurityClass security, Guid identityGuid);
	}

	/// <summary>
	/// The serializable SQL parameter.
	/// </summary>
	[DataContract]
	[KnownType(typeof(DateTimeOffset))]
	public class SerializableSqlParameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableSqlParameter"/> class.
		/// </summary>
		public SerializableSqlParameter()
		{
			this.Name = string.Empty;
			this.DbType = SqlDbType.BigInt;
			this.Value = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableSqlParameter"/> class.
		/// </summary>
		/// <param name="name">The parameter name</param>
		/// <param name="value">The parameter value</param>
		public SerializableSqlParameter(string name, object value)
		{
			this.Name = name;
			this.Value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableSqlParameter"/> class.
		/// </summary>
		/// <param name="name">The parameter name</param>
		/// <param name="type">The parameter type</param>
		/// <param name="value">The parameter value</param>
		public SerializableSqlParameter(string name, SqlDbType type, object value)
		{
			this.Name = name;
			this.DbType = type;
			this.Value = value;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the SQL DB type.
		/// </summary>
		[DataMember]
		public SqlDbType DbType { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		[DataMember]
		public object Value { get; set; }
	}

	/// <summary>
	/// The serializable SQL command.
	/// </summary>
	[DataContract]
	public class SerializableSqlCommand
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableSqlCommand"/> class.
		/// </summary>
		public SerializableSqlCommand()
		{
			this.CmdText = string.Empty;
			this.CmdType = CommandType.Text;
			this.Parameters = new List<SerializableSqlParameter>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableSqlCommand"/> class.
		/// </summary>
		/// <param name="commandText">The command text</param>
		public SerializableSqlCommand(string commandText)
		{
			this.CmdText = commandText;
			this.CmdType = CommandType.Text;
			this.Parameters = new List<SerializableSqlParameter>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableSqlCommand"/> class.
		/// </summary>
		/// <param name="commandText">The command text</param>
		/// <param name="commandType">The command type</param>
		public SerializableSqlCommand(string commandText, CommandType commandType)
		{
			this.CmdText = commandText;
			this.CmdType = commandType;
			this.Parameters = new List<SerializableSqlParameter>();
		}

		/// <summary>
		/// Gets or sets the command text.
		/// </summary>
		[DataMember]
		public string CmdText { get; set; }

		/// <summary>
		/// Gets or sets the command text.
		/// </summary>
		[DataMember]
		public CommandType CmdType { get; set; }

		/// <summary>
		/// Gets or sets the parameters.
		/// </summary>
		[DataMember]
		public List<SerializableSqlParameter> Parameters { get; set; }
	}
}
