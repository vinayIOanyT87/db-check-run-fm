// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoteClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	[DataContract]
	[Serializable]
	public class NoteClass : BaseDataObject
	{
		[DataMember]
		private string note;

		// Audit logging has been removed from the main program.
		//public bool AuditLog = false;

		#region Constructors and Destructors

		public NoteClass()
		{
			this.Reset();
		}

		public NoteClass( string value )
		{
			this.Reset();
			this.note = value;
		}

		#endregion

		protected bool Equals( NoteClass other )
		{
			return string.Equals(this.note, other.note);
		}

		public override int GetHashCode()
		{
			return (this.note != null ? this.note.GetHashCode() : 0);
		}

		#region Public Properties

		public string AuditNote { get { return (this.note.Length > 1000) ? this.note.Substring(0, 1000) : this.note; } }

		[XmlIgnore]
		public override ENTITY_TYPE EntityType { get { return ENTITY_TYPE.NOTE; } }

		public string Note { get { return this.note; } set { this.SetString("Note", 2000, value, ref this.note); } }

		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType { get { return ENTITY_TYPE.NONE; } }

		#endregion

		#region Public Methods and Operators

		public override bool Equals(object obj)
		{
			var noteClass = obj as NoteClass;
			if (noteClass != null && noteClass.Note == this.note && noteClass.IdentityGuid == this._IdentityGuid)
			{
				return true;
			}

			return false;
		}

		public override sealed void Reset()
		{
			base.Reset();
			this.Note = string.Empty;
		}

		public override string ToString()
		{
			return this.note;
		}

		#endregion

	}
}
