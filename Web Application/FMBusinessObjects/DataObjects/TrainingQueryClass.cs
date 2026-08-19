using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FMBusinessObjects.DataObjects
{
	[QueryWriterTopic(typeof(TrainingQueryClass), "Training", SupportsArchiveQuery = false)]
	[QueryWriterTopicSecurity(RIGHT.VIEW_TRAINING_QUALIFICATIONS)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_PERSON_QUALIFICATIONS)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_PERSON_TRAINING)]
	public class TrainingQueryClass
	{
		[QueryWriterField("Last Name")]
		protected string LastName { get; set; }

		[QueryWriterField("First Name")]
		protected string FirstName { get; set; }

		[QueryWriterField("ID")]
		protected string ID { get; set; }

		[QueryWriterField("Date Completed")]
		protected DateTimeOffset DateCompleted { get; set; }

		[QueryWriterField("Expiration Date")]
		protected DateTimeOffset ExpirationDate { get; set; }

		[QueryWriterField("Instructor")]
		protected string Instructor { get; set; }

		[QueryWriterField("Rating")]
		protected string Rating { get; set; }

		[QueryWriterField("Sequence Number")]
		protected string Sequence { get; set; }

		[QueryWriterField("Type")]
		protected string Type { get; set; }

		[QueryWriterField("Created By")]
		protected string CreatedBy { get; set; }

		[QueryWriterField("Created Date")]
		protected DateTimeOffset CreatedDate { get; set; }

		[QueryWriterField("Updated By")]
		protected string UpdatedBy { get; set; }

		[QueryWriterField("Updated Date")]
		protected DateTimeOffset UpdatedDate { get; set; }

		public void QueryWriterSQL(SqlCommand cmd, SecurityClass Security, string selectClause)
		{
			cmd.CommandText = selectClause + ", " +
					"EntityGuid " +
					"FROM (SELECT  " +
						"P.[PersonnelGuid] as 'EntityGuid', " +
						"P.LastName, " +
						"P.FirstName, " +
						"P.Department, " +
						"R.ID, " +
						"Q.DateCompleted, " +
						"Q.ExpirationDate, " +
						"Q.Instructor, " +
						"Q.Rating, " +
						"Q.Sequence, " +
						"[Type] = 'Qualification', " +
						"Q.CreatedBy, " +
						"Q.CreatedDate, " +
						"Q.UpdatedBy, " +
						"Q.UpdatedDate " +
					"from map.tblQualificationPersonQualificationToPerson Q  " +
						"join tblPersonnel P on Q.PersonnelGuid = P.PersonnelGuid " +
						"join tblQualifications R on R.QualificationGuid = Q.QualificationGuid " +
					"where  " +
						"R.SiteGuid = @SiteGuid " +
					" UNION " +
					" SELECT " +
						"P.[PersonnelGuid] as 'EntityGuid', " +
						"P.LastName, " +
						"P.FirstName, " +
						"P.Department, " +
						"R.ID, " +
						"Q.DateCompleted, " +
						"Q.ExpirationDate, " +
						"Q.Instructor, " +
						"Q.Rating, " +
						"Q.Sequence, " +
						"[Type] =  'Training', " +
						"Q.CreatedBy, " +
						"Q.CreatedDate, " +
						"Q.UpdatedBy, " +
						"Q.UpdatedDate " +
					"from map.tblQualificationPersonTrainingToPerson Q  " +
						"join tblPersonnel P on Q.PersonnelGuid = P.PersonnelGuid " +
						"join tblQualifications R on R.QualificationGuid = Q.QualificationGuid " +
					"where  " +
						"R.SiteGuid = @SiteGuid " +
					") tblResults WHERE 1=1";

			cmd.Parameters.AddWithValue("@SiteGuid", Security.SiteGuid);
		}

		public void QueryWriterPostProcess(SecurityClass security, DataSet set)
		{
			CensorFieldsIfNecessary(security, set);
		}

		private void CensorFieldsIfNecessary(SecurityClass security, DataSet set)
		{
			if (security.HasRight(RIGHT.VIEW_TRAINING_QUAL_HISTORY) == false
				&& security.HasRight(RIGHT.MODIFY_TRAINING_QUAL_HISTORY) == false)
			{
				set.Tables[0].Rows.Clear();
			}
		}

		public string DetailPageReference()
		{
			return "FMWebApp\\PersonForm.aspx";
		}

	}
}
