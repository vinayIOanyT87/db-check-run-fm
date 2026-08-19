// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Notes.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for NotessClass.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;
	using FMBusinessServices.InternalClasses;

	/// <summary>
	/// Service class for notes access.
	/// </summary>
	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class Notes : INotes
	{
		#region Constants and Fields
		/// <summary>
		/// Common database access object.
		/// </summary>
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();
		#endregion

		#region Public Methods and Operators
		/// <summary>
		/// Adds the specified note object.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="note">The note object to add.</param>
		/// <returns>The identity Guid of the newly added note record.</returns>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public Guid Add(SecurityClass security, NoteClass note)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (note == null)
			{
				throw new ArgumentNullException("note");
			}

			note.CreatedDate = DateTimeOffset.Now;
			note.CreatedBy = security.UserID;
			note.UpdatedDate = note.CreatedDate;
			note.UpdatedBy = security.UserID;
			note.IdentityGuid = Guid.NewGuid();

			using (var cmd = new SqlCommand())
			{
				note.InsertSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

			return note.IdentityGuid;
		}

		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The identity GUID of the note to get.</param>
		/// <returns>The request note object or null if not found.</returns>
		public NoteClass Get(SecurityClass security, Guid identityGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if ( identityGuid == null )
			{
				throw new ArgumentNullException( "identityGuid" );
			}

			var note = new NoteClass { IdentityGuid = identityGuid };

			if (identityGuid != Guid.Empty)
			{
				using (var cmd = new SqlCommand())
				{
					note.SelectSQL(cmd, ContextUtil.IsInTransaction);
					note.LoadDataSet(this.consolidatedDA.GetDataSet(cmd, security));
				}
			}

			return note;
		}

		/// <summary>
		/// Modifies the specified security.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="note">The note to save.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Modify(SecurityClass security, NoteClass note)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (note == null)
			{
				throw new ArgumentNullException("note");
			}

			var oldNote = this.Get(security, note.IdentityGuid);

			if (oldNote.IdentityGuid == Guid.Empty)
			{
				throw new Exception("Note Not Found");
			}

			note.UpdatedDate = DateTimeOffset.Now;
			note.UpdatedBy = security.UserID;
			this.consolidatedDA.ExecuteQuery(security, note.UpdateSQL());
		}

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="noteGuid">The identity GUID of the note to purge.</param>
		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Purge(SecurityClass security, Guid noteGuid)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			NoteClass note = this.Get(security, noteGuid);
			if (note.IdentityGuid == Guid.Empty)
			{
				return;
			}

			using (var cmd = new SqlCommand())
			{
				note.PurgeSQL(cmd);
				this.consolidatedDA.ExecuteQuery(security, cmd);
			}
		}
		#endregion
	}
}