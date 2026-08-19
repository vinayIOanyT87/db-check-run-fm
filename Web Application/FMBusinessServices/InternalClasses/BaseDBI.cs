// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDBI.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Data.SqlClient;

	using FMBusinessServices.DataAccessLayer;

	public abstract class BaseDBI : IDisposable
	{
		#region Constants and Fields

		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		protected string createdBy;

		protected DateTimeOffset createdDateTime;

		protected SqlCommand deleteCmd;

		protected SqlCommand deleteRemainingCmd;

		protected SqlCommand insertCmd;

		protected DateTimeOffset saveTime;

		protected SqlCommand selectCmd;

		protected SqlCommand updateCmd;

		protected string updatedBy;

		protected DateTimeOffset updatedDateTime;

		protected string user;

		private bool disposed;

		#endregion Constants and Fields

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseDBI" /> class.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="saveTime">The save time.</param>
		protected BaseDBI(string user, DateTimeOffset saveTime)
		{
			this.InitializeStatements();
			this.user = user;
			this.saveTime = saveTime;

			this.createdBy = user;
			this.updatedBy = user;
			this.createdDateTime = saveTime;
			this.updatedDateTime = saveTime;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="BaseDBI" /> class.
		/// </summary>
		~BaseDBI()
		{
			this.Dispose(false);
		}

		#endregion Constructors and Destructors

		#region Public Methods and Operators

        /// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);

			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		#endregion Public Methods and Operations

        #region Public Static Methods

        /// <summary>
        /// If the value is null, a DBNull.Value is return. Typically passing in a value of null to a command parameter will result in
        /// the parameter being exclude from the underlying command execution.  To avoid this, a DBNull.Value should be returned which will
        /// result in a NULL being sent to the database.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="value">
        /// Value to set
        /// </param>
        /// <returns>
        /// DBNull.Value if the input value is null; otherwise the original value is returned.
        /// </returns>
        public static object SetOptionalValue<T>(T value)
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
        /// <param name="value">
        /// Value to set
        /// </param>
        /// <returns>
        /// DBNull.Value if the input value is null; otherwise the original value is returned.
        /// </returns>
        public static object SetOptionalValue<T>(Nullable<T> value)
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
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// DBNull.Value if the input value is null; otherwise the original value is returned.
        /// </returns>
        public static T GetOutputValue<T>(SqlParameter parameter)
            where T : class
        {
            T retValue = default(T);

            if (!BaseDBI.IsParameterNull(parameter))
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
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="defaultValue">
        /// The default Value.
        /// </param>
        /// <returns>
        /// DBNull.Value if the input value is null; otherwise the original value is returned.
        /// </returns>
        public static T? GetOutputValue<T>(SqlParameter parameter, T defaultValue)
            where T : struct
        {
            T retValue = default(T);

            if (!BaseDBI.IsParameterNull(parameter))
            {
                retValue = (T)parameter.Value;
            }

            return retValue;
        }

        /// <summary>
        /// The is parameter null.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsParameterNull(SqlParameter parameter)
        {
            return (null == parameter) || (parameter.Value == null || parameter.SqlValue == DBNull.Value);
        }

        #endregion Public Static Methods

        #region Methods

        /// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (this.disposed == false)
			{
				// If disposing equals true, dispose all managed
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
					if (this.selectCmd != null)
					{
						this.selectCmd.Dispose();
					}

					if (this.insertCmd != null)
					{
						this.insertCmd.Dispose();
					}

					if (this.deleteCmd != null)
					{
						this.deleteCmd.Dispose();
					}

					if (this.deleteRemainingCmd != null)
					{
						this.deleteRemainingCmd.Dispose();
					}

					if (this.updateCmd != null)
					{
						this.updateCmd.Dispose();
					}
				}

				// Note disposing has been done.
				this.disposed = true;
			}
		}

		protected abstract void PrepareDeleteRemainingStatement();

		protected abstract void PrepareDeleteStatement();

		protected abstract void PrepareInsertStatement();

		protected abstract void PrepareSelectStatement();

		protected virtual void PrepareUpdateStatement()
		{
		}

		/// <summary>
		/// Initializes the SQL command objects used by this class.
		/// </summary>
		private void InitializeStatements()
		{
			this.insertCmd = new SqlCommand();
			this.selectCmd = new SqlCommand();
			this.deleteCmd = new SqlCommand();
			this.deleteRemainingCmd = new SqlCommand();
			this.updateCmd = new SqlCommand();

			this.PrepareSelectStatement();
			this.PrepareInsertStatement();
			this.PrepareDeleteStatement();
			this.PrepareDeleteRemainingStatement();
			this.PrepareUpdateStatement();
		}

		#endregion Methods
	}
}