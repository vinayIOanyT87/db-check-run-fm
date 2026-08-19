using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
	internal class MapDBInfoClass
	{
		/// <summary>
		/// This is a map helper data container
		/// Let's say we assign a Qualification to a Person.
		/// Qualification is the "Assigned".  The person is the "Assignee"
		/// </summary>
		const string MAPTABLE_PREFIX = "map";
		public MapDBInfoClass(string newEntityTypeID, string newMapTableName, string newAssigneedGuidColumnName, string newAssigneeTableName) :
			this(newEntityTypeID, MAPTABLE_PREFIX + "." + newMapTableName, string.Empty, newAssigneedGuidColumnName, newAssigneeTableName, newAssigneedGuidColumnName, false)
		{
		}
		public MapDBInfoClass(string newEntityTypeID, string newMapTableName, string newAssigneedGuidColumnName, string newAssigneeTableName, bool newSupportSiteGuid) :
			this(newEntityTypeID, MAPTABLE_PREFIX + "." + newMapTableName, string.Empty, newAssigneedGuidColumnName, newAssigneeTableName, newAssigneedGuidColumnName, newSupportSiteGuid)
		{
		}
		public MapDBInfoClass(string newEntityTypeID, string newMapTableName, string newAssignedGuidColumnName,
			string newAssigneedGuidColumnName, string newAssigneeTableName, string newAssigneeTablePrimaryKeyColumnName, bool newSupportSiteGuid)
		{
			EntityTypeID = newEntityTypeID;
			MapTableName = newMapTableName;
			AssignedGuidColumnName = newAssignedGuidColumnName;
			AssigneeGuidColumnName = newAssigneedGuidColumnName;
			AssigneeTableName = newAssigneeTableName;
			AssigneeTablePrimaryKeyColumnName = newAssigneeTablePrimaryKeyColumnName;
			SupportsSiteGuid = newSupportSiteGuid;
		}

		public string EntityTypeID { get; set; }

		// e.g. tblQualificationPersonLicenseToPerson
		public string MapTableName { get; set; }
		// default QualificationGuid
		public string AssignedGuidColumnName { get; set; }
		// e.g. PersonnelGuid on the map table
		public string AssigneeGuidColumnName { get; set; }
		// e.g. tblPersonnel
		public string AssigneeTableName { get; set; }
		// e.g. PersonnelGuid (The Assignee Guid column in the Assignee table.  Default same is AssignedGuidColumnName, column in the Map table.
		public string AssigneeTablePrimaryKeyColumnName { get; set; }

		public bool SupportsSiteGuid { get; set; }

		public string MapTablePrimaryKeyColumnName
		{
			get
			{
				return MapTableName.Replace(MAPTABLE_PREFIX + ".tbl", "") + "Guid";
			}
		}

	}
}
