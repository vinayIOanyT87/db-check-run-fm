namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.ServiceClasses;

	internal static class ExStarsSiteConfigDAO
	{
		internal static void GetExStarsConfigSql(
			this ExStarsSiteConfigClass config,
			SqlCommand cmd,
			Guid managerCompanyGuid,
			Guid siteGuid)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.gsp_ExStarsSiteConfigSelect";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@ManagerCompanyGuid", managerCompanyGuid);
			cmd.CommandTimeout = 120;
			
		}

		internal static void GetExStarsConfigSql(
			this ExStarsSiteConfigClass config,
			SqlCommand cmd)
		{
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
@"	SELECT TOP 1 [SiteGuid]
	,[ManagerCompanyGuid]
	,InterchangeSenderId
	,[ApplicationSendersCode]
	,[AuthorizationCode]
	,[FeinCode]
	,[SecurityCode]
	,[InfoProviderName]
	,[AbbreviatedProviderName]
	,[GroupControlNumber]
	,[IRS_637Registration]
	,[TerminalControlNumber]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
FROM [dbo].[tblExStarsSiteConfig]";
			cmd.CommandTimeout = 120;

		}

	}
}