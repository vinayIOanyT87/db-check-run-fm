// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentTankInventoryProvider.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace FMDatabase.SqlServer.Clr
{
    using System;
    using System.Data.SqlTypes;


    using Microsoft.SqlServer.Server;

    using FMDatabase.SqlServer.Clr.Interfaces;
    using FMDatabase.SqlServer.Clr.UtilityClasses;

    using FMPasswordEncryptDecrypt;

    using FMPointTagArchive.Core;
    using FMPointTagArchive.Core.Interfaces;
    using FMPointTagArchive.Core.Interfaces.ServiceRequests;

	public class CurrentTankInventoryProvider
    {
        private readonly ICurrentTankInventoryProcessor CurrentTankInventoryProcessor = new CurrentTankInventoryProcessor();

        private readonly IDataSetTransmitter DataSetTransmitter = new DataSetTransmitter();

        private static readonly CurrentTankInventoryProvider Provider = new CurrentTankInventoryProvider();

        [SqlProcedure]
        public static void usp_CurrentTankInventory(SqlGuid SiteGuid, string SiteID, DateTimeOffset BeginDate, bool useSmallFieldNames, bool useDateOnly, SqlGuid UserGuid, string refDataTableAsXML, string cassandraConfiguration, string cassandraUsername, string cassandraPassword)
        {
            ValidateArguments(SiteGuid, SiteID, refDataTableAsXML, cassandraConfiguration, cassandraUsername, cassandraPassword);

			var provider = new CurrentTankInventoryProvider();
            provider.Generate(SiteGuid.Value, SiteID,  BeginDate, UserGuid.Value, useSmallFieldNames, refDataTableAsXML, cassandraConfiguration, cassandraUsername,cassandraPassword);
        }

        private static void ValidateArguments(SqlGuid SiteGuid, string SiteID, string refDataTableAsXML, string cassandraConfiguration, string cassandraUsername, string cassandraPassword)
        {
            if (SiteGuid.IsNull)
            {
                throw new ArgumentNullException("SiteGuid");
            }

            if (SiteGuid.Value == Guid.Empty)
            {
                throw new ArgumentException("SiteGuid is invalid.");
            }

            if (string.IsNullOrEmpty(SiteID))
            {
                throw new ArgumentNullException("SiteID");
            }

            if (string.IsNullOrEmpty(refDataTableAsXML))
            {
                throw new ArgumentNullException("refDataTableAsXML");
            }

            if (string.IsNullOrEmpty(cassandraConfiguration))
            {
                throw new ArgumentNullException("cassandraConfiguration");
            }

				if (string.IsNullOrEmpty(cassandraUsername))
				{
					throw new ArgumentNullException("cassandraUsername");
				}

				if (string.IsNullOrEmpty(cassandraPassword))
				{
					throw new ArgumentNullException("cassandraPassword");
				}

		}

        private void Generate(Guid siteGuid, string siteID, DateTimeOffset reportDate, Guid UserGuid, bool useSmallFieldNames, string refDataTableAsXML, string cassandraConfiguration, string cassandraUsername, string cassandraPassword)
        {
			string hex = BitConverter.ToString((Convert.FromBase64String(cassandraPassword)));
			hex = hex.Replace("-", "");
			string decrypted = PasswordEncrytpDecrpt.Decrypt(hex, new Guid("00000000-0000-0000-0000-000000000001"));

			var request = new CurrentTankInventoryProcessorSR
            {
                SiteGuid = siteGuid,
                SiteID = siteID,
                BeginDate = reportDate,
				UserGuid = UserGuid,
				UseSmallFieldNames = useSmallFieldNames,
                refDataTableAsXML = refDataTableAsXML,
                CassandraConfiguration = cassandraConfiguration,
					CassandraUsername = cassandraUsername,
CassandraPassword = decrypted

			};

            var dataSet = this.CurrentTankInventoryProcessor.Process(request);

            // Transmit the dataset
            this.DataSetTransmitter.Transmit(dataSet);
        }
    }
}
