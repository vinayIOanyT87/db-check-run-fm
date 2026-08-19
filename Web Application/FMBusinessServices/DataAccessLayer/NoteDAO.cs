namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.InternalClasses;

	internal static class NoteDAO
	{
		internal static SqlCommand UpdateSQL(this NoteClass note)
		{
			const string SQL = "UPDATE tblNotes SET " + "Note = @Note," + "UpdatedDate = @UpdatedDate," + "UpdatedBy = @UpdatedBy "
								+ "WHERE NoteGuid = @NoteGuid";

			var cmd = new SqlCommand( SQL );

			cmd.Parameters.Add( "@NoteGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters.Add( "@UpdatedDate", SqlDbType.DateTimeOffset );
			cmd.Parameters.Add( "@Note", SqlDbType.NVarChar, 2000 );
			cmd.Parameters.Add( "@UpdatedBy", SqlDbType.NVarChar, 100 );

			cmd.Parameters["@Note"].Value = note.Note;
			cmd.Parameters["@UpdatedDate"].Value = note.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = note.UpdatedBy;
			cmd.Parameters["@NoteGuid"].Value = note.IdentityGuid;

			return cmd;
		}

		internal static void InsertSQL( this NoteClass note, SqlCommand cmd )
		{
			cmd.CommandText = "INSERT INTO tblNotes (" + "Note," + "CreatedDate," + "CreatedBy," + "UpdatedDate," + "UpdatedBy,"
							  + "NoteGuid" + ") VALUES (" + "@Note," + "@CreatedDate," + "@CreatedBy," + "@UpdatedDate,"
							  + "@UpdatedBy," + "@NoteQuid" + ")";

			cmd.Parameters.Add( "@Note", SqlDbType.NVarChar, 2000 );
			cmd.Parameters.Add( "@CreatedBy", SqlDbType.NVarChar, 100 );
			cmd.Parameters.Add( "@CreatedDate", SqlDbType.DateTimeOffset );
			cmd.Parameters.Add( "@UpdatedBy", SqlDbType.NVarChar, 100 );
			cmd.Parameters.Add( "@UpdatedDate", SqlDbType.DateTimeOffset );
			cmd.Parameters.Add( "@NoteQuid", SqlDbType.UniqueIdentifier );

			cmd.Parameters["@Note"].Value = note.Note;
			cmd.Parameters["@CreatedBy"].Value = note.CreatedBy;
			cmd.Parameters["@CreatedDate"].Value = note.CreatedDate;
			cmd.Parameters["@UpdatedBy"].Value = note.UpdatedBy;
			cmd.Parameters["@UpdatedDate"].Value = note.UpdatedDate;
			cmd.Parameters["@NoteQuid"].Value = note.IdentityGuid;
		}

		internal static void PurgeSQL( this NoteClass note, SqlCommand cmd )
		{
			cmd.CommandText = "DELETE FROM tblNotes WHERE NoteGuid = @NoteGuid";

			cmd.Parameters.Add( "@NoteGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters["@NoteGuid"].Value = note.IdentityGuid;
		}

		internal static void SelectSQL( this NoteClass note, SqlCommand cmd, bool bInTransaction )
		{
			cmd.CommandText = "SELECT tblNotes.* FROM tblNotes " + BaseDAO.SQLUpdateLock( bInTransaction ) + " WHERE  NoteGuid = @NoteGuid";
			cmd.Parameters.Add( "@NoteGuid", SqlDbType.UniqueIdentifier );
			cmd.Parameters["@NoteGuid"].Value = note.IdentityGuid;
		}

		internal static void LoadDataSet( this NoteClass note, DataSet set )
		{
			if ( set == null )
			{
				throw new ArgumentNullException( "set" );
			}

			note.Reset();

			DataTable table = set.Tables[0];
			if ( table.Rows.Count == 0 )
			{
				return;
			}

			DataRow row = table.Rows[0];

			note.IdentityGuid = DataObject.getValue( row["NoteGuid"], Guid.Empty );

			note.Note = table.Columns.Contains("Note") ? DataObject.getValue(row["Note"], string.Empty) : String.Empty;

			note.CreatedDate = DataObject.getValue(row, "CreatedDate", DateTimeOffset.Now);
			note.CreatedBy = DataObject.getValue(row, "CreatedBy", BaseDataObject.ADMIN);
			note.UpdatedDate = DataObject.getValue(row, "UpdatedDate", DateTimeOffset.Now);
			note.UpdatedBy = DataObject.getValue( row["UpdatedBy"], BaseDataObject.ADMIN );
		}

		/// <summary>
		/// This method adds the specified object to the database.
		/// </summary>
		/// <param name="note">The note class to add.</param>
		/// <param name="security">A valid FuelsManager security object.</param>
		internal static void Add(this NoteClass note, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				note.InsertSQL( cmd );
				consolidatedDA.ExecuteQuery( security, cmd );
			}
		}

		internal static void Get(this NoteClass note, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				note.SelectSQL( cmd, ContextUtil.IsInTransaction );
				note.LoadDataSet( consolidatedDA.GetDataSet( cmd, security ) );
			}
		}

		internal static void Modify(this NoteClass note, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			consolidatedDA.ExecuteQuery( security, note.UpdateSQL() );
		}

		internal static void Purge(this NoteClass note, SecurityClass security)
		{
			var consolidatedDA = new ConsolidatedDAClass();

			using ( var cmd = new SqlCommand() )
			{
				note.PurgeSQL( cmd );
				consolidatedDA.ExecuteQuery( security, cmd );
			}
		}
	}
}
