// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyCrossReference.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyCrossReference type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.ServiceClasses
{
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	/// <summary>
	/// The company cross reference mapping.
	/// </summary>
	public class CompanyCrossReference : ICompanyCrossReference
	{
		/// <summary>
		/// The consolidated data access layer.
		/// </summary>
		private ConsolidatedDAClass consolidatedDa;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompanyCrossReference"/> class.
		/// </summary>
		public CompanyCrossReference( )
		{
			this.consolidatedDa = new ConsolidatedDAClass();
		}

		/// <summary>
		/// This method will return the Key Name from "tblCompanyCrossReference"
		/// based on the Reference Name and the Reference Type (1 = Navy, 2 = Others).
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="referenceName">
		/// The reference name.
		/// </param>
		/// <param name="referenceType">
		/// The reference type index.
		/// </param>
		/// <returns>
		/// Key Name <see cref="string"/>.
		/// </returns>
		public string GetKeyName(SecurityClass security, string referenceName, CompanyCrossReferenceDO.CrossReferenceTypes referenceType)
		{
			string returnKeyName = string.Empty;

			using ( var sqlCommand = new SqlCommand( ) )
			{
				const string Sql = "SELECT KeyName " +
								   "FROM tblCompanyCrossReference ccr LEFT OUTER JOIN lookup.tblCompanyCrossReferenceType ccrt " +
								   "ON ccr.ReferenceTypeIndex = ccrt.CompanyCrossReferenceTypeIndex " +
								   "WHERE ReferenceName = @ReferenceName AND ccrt.CompanyCrossReferenceTypeIndex = @ReferenceTypeIndex ";

				sqlCommand.CommandText = Sql;

				var parm = new SqlParameter("@ReferenceName", SqlDbType.NVarChar, 100) { Value = referenceName };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@ReferenceTypeIndex", SqlDbType.Int) { Value = (int) referenceType };
				sqlCommand.Parameters.Add(parm);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
				{
					DataTable table = dataSet.Tables[0];

					if ( table.Rows.Count > 0 )
					{
						DataRow row = table.Rows[0];
						returnKeyName = row.IsNull("KeyName") ? string.Empty : (string) row["KeyName"];

						if ( string.IsNullOrEmpty(returnKeyName) )
						{
							System.Diagnostics.Trace.WriteLine("Key Name is null. ");
						}
					}
				}
			}

			return returnKeyName;
		}

		/// <summary>
		/// This method will return the Reference Name from "tblCompanyCrossReference"
		/// based on the Key Name and the Reference Type (1 = Navy, 2 = Others).
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="keyName">
		/// The key name.
		/// </param>
		/// <param name="referenceType">
		/// The Reference type index.
		/// </param>
		/// <returns>
		/// Reference Name <see cref="string"/>.
		/// </returns>
		public string GetReferenceName(SecurityClass security, string keyName, CompanyCrossReferenceDO.CrossReferenceTypes referenceType)
		{
			string returnReferenceName = string.Empty;

			using ( var sqlCommand = new SqlCommand( ) )
			{
				const string Sql = "SELECT ReferenceName " +
								   "FROM tblCompanyCrossReference ccr LEFT OUTER JOIN lookup.tblCompanyCrossReferenceType ccrt " +
								   "ON ccr.ReferenceTypeIndex = ccrt.CompanyCrossReferenceTypeIndex " +
								   "WHERE ccr.KeyName = @KeyName AND ccrt.CompanyCrossReferenceTypeIndex = @ReferenceTypeIndex ";

				sqlCommand.CommandText = Sql;

				var parm = new SqlParameter("@KeyName", SqlDbType.NVarChar, 100) { Value = keyName };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@ReferenceTypeIndex", SqlDbType.Int) { Value = (int) referenceType };
				sqlCommand.Parameters.Add(parm);

				DataSet dataSet = this.consolidatedDa.GetDataSet(sqlCommand, security);

				if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
				{
					DataTable table = dataSet.Tables[0];

					if ( table.Rows.Count > 0 )
					{
						DataRow row = table.Rows[0];
						returnReferenceName = row.IsNull("ReferenceName") ? string.Empty : (string) row["ReferenceName"];

						if ( string.IsNullOrEmpty(returnReferenceName) )
						{
							System.Diagnostics.Trace.WriteLine("Reference Name is null. ");
						}
					}
				}
			}

			return returnReferenceName;
		}
	}
}