namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessServices.DataAccessLayer;

	[SecuritySafeCritical]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class OperateScreenConfigurations : FMServiceBase, IOperateScreenConfigurations
	{
		private const int MaxOperateScreenSettingsToPurge = 32;
		private const string OperatorScreenID = "Operator";
		private const string ScreenWindowNamePrefix = "Screen";

		public OperateScreenConfiguration GetBySiteUserClientIpAddress(SecurityClass security, Guid siteGuid, Guid userGuid, string clientIpAddress)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var consolidatedDA = new ConsolidatedDAClass();
			using (var cmd = new SqlCommand())
			{
				OperateScreenConfiguration.GetBySiteUserClientIpAddressSQL(cmd, siteGuid, userGuid, clientIpAddress);
				var dataSet = consolidatedDA.GetDataSet(cmd, security);
				if (dataSet == null || dataSet.Tables.Count <= 0 || dataSet.Tables[0].Rows.Count <= 0)
				{
					return null;
				}

				var configuration = new OperateScreenConfiguration();
				configuration.AutoLoad(dataSet.Tables[0].Rows[0]);
				return configuration;
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void SetScreenMask(SecurityClass security, Guid siteGuid, Guid userGuid, string clientIpAddress, long screenMask)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var consolidatedDA = new ConsolidatedDAClass();
			var previousScreenMask = GetExistingScreenMask(security, consolidatedDA, siteGuid, userGuid, clientIpAddress);
			var normalizedScreenMask = NormalizeScreenMask(screenMask);
			var configuration = new OperateScreenConfiguration(security)
			{
				OperateScreenConfigurationGuid = Guid.NewGuid(),
				SiteGuid = siteGuid,
				UserGuid = userGuid,
				ClientIpAddress = clientIpAddress ?? string.Empty,
				ScreenMask = normalizedScreenMask
			};
			configuration.SetCreationStamp(security);

			using (var cmd = new SqlCommand())
			{
				configuration.GetUpsertSQL(cmd);
				consolidatedDA.ExecuteQuery(security, cmd);
			}

			if (previousScreenMask.HasValue)
			{
				PurgeRemovedScreenViewStateSettings(security, consolidatedDA, siteGuid, userGuid, configuration.ClientIpAddress, previousScreenMask.Value, normalizedScreenMask);
			}
		}

		private static long NormalizeScreenMask(long screenMask)
		{
			return (screenMask < 1L ? 1L : screenMask) | 1L;
		}

		private static long? GetExistingScreenMask(SecurityClass security, ConsolidatedDAClass consolidatedDA, Guid siteGuid, Guid userGuid, string clientIpAddress)
		{
			using (var cmd = new SqlCommand())
			{
				OperateScreenConfiguration.GetBySiteUserClientIpAddressSQL(cmd, siteGuid, userGuid, clientIpAddress);
				var dataSet = consolidatedDA.GetDataSet(cmd, security);
				if (dataSet == null || dataSet.Tables.Count <= 0 || dataSet.Tables[0].Rows.Count <= 0)
				{
					return null;
				}

				var configuration = new OperateScreenConfiguration();
				configuration.AutoLoad(dataSet.Tables[0].Rows[0]);
				return NormalizeScreenMask(configuration.ScreenMask);
			}
		}

		private static void PurgeRemovedScreenViewStateSettings(SecurityClass security, ConsolidatedDAClass consolidatedDA, Guid siteGuid, Guid userGuid, string clientIpAddress, long previousScreenMask, long newScreenMask)
		{
			var removedScreenMask = NormalizeScreenMask(previousScreenMask) & ~NormalizeScreenMask(newScreenMask);
			for (var screenNumber = 2; screenNumber <= MaxOperateScreenSettingsToPurge; screenNumber++)
			{
				var screenBit = 1L << (screenNumber - 1);
				if ((removedScreenMask & screenBit) == 0)
				{
					continue;
				}

				using (var cmd = new SqlCommand())
				{
					UserViewStateSetting.GetPurgeBySiteAndUserAndWindowNameAndViewIDSQL(
						cmd,
						siteGuid,
						userGuid,
						clientIpAddress,
						ScreenWindowNamePrefix + screenNumber,
						OperatorScreenID);
					consolidatedDA.ExecuteQuery(security, cmd);
				}
			}
		}
	}
}
