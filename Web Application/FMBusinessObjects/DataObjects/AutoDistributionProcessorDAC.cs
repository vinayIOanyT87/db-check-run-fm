///***************************************************************************
/// Module Name:  AutoDistributionProcessorDAC
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    using FMCore;

    /// <summary>
    /// This is just a data container for passing information from AutoDistributionProcessor to AutoDistributionProcessorDAC
    /// </summary>
    public class AutoDistributionThruputSqlInfo
	{

		public Guid RuleGuid { get; set; }
		public Guid ManagerGuid { get; set; }
		public Guid ProductGuid { get; set; }

		public DateTimeOffset StartDate { get; set; }
		public DateTimeOffset EndDate { get; set; }

		public double MassConversionFactor { get; set; }
		public double MassDecimalPlaces { get; set; }

		public double VolumeConversionFactor { get; set; }
		public double VolumeDecimalPlaces { get; set; }

	}

	/// <summary>
	/// This is used by the AutoDistributionProcessor to call the database to calculate the thruput
	/// </summary>
	public class AutoDistributionProcessorDAC
	{
		private static string SqlCalculateThruput = "dbo.usp_AutoDistributionRuleCalculateThruput";

		/// <summary>
		/// Prepares a SqlCommand to calculate thruput.
		/// </summary>
		/// <param name="cmd">SqlCommand to be prepared</param>
		/// <param name="mySecurity">Security object used by FM</param>
		/// <param name="sqlInfo">sql parameter values</param>
		public static void PrepareThruputSqlCommand(SqlCommand cmd, SecurityClass mySecurity, AutoDistributionThruputSqlInfo sqlInfo)
		{
            cmd.ThrowIfNull("cmd");
            mySecurity.ThrowIfNull("mySecurity");

			cmd.CommandText = SqlCalculateThruput;
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@AutoDistributionRuleGuid", sqlInfo.RuleGuid);

			cmd.Parameters.AddWithValue("@SelectedSiteGuid", mySecurity.SiteGuid);
			cmd.Parameters.AddWithValue("@LoginSiteGuid", mySecurity.LoginSiteGuid);

			cmd.Parameters.AddWithValue("@ManagerGuid", sqlInfo.ManagerGuid);
			cmd.Parameters.AddWithValue("@ProductGuid", sqlInfo.ProductGuid);

			cmd.Parameters.AddWithValue("@StartDate", sqlInfo.StartDate);
			cmd.Parameters.AddWithValue("@EndDate", sqlInfo.EndDate);

			cmd.Parameters.AddWithValue("@VolumeConversionFactor", sqlInfo.VolumeConversionFactor);
			cmd.Parameters.AddWithValue("@VolumeDecimalPlaces", sqlInfo.VolumeDecimalPlaces);

			cmd.Parameters.AddWithValue("@MassConversionFactor", sqlInfo.MassConversionFactor);
			cmd.Parameters.AddWithValue("@MassDecimalPlaces", sqlInfo.MassDecimalPlaces);
		}
	}
}
