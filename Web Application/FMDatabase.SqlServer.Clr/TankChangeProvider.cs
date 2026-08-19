using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankChangeProvider.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace FMDatabase.SqlServer.Clr
{
	using System;
	using System.Data.SqlTypes;
	using System.Security.Cryptography;


	using Microsoft.SqlServer.Server;

	using FMDatabase.SqlServer.Clr.Interfaces;
	using FMDatabase.SqlServer.Clr.UtilityClasses;

	using FMPasswordEncryptDecrypt;
	using FMPasswordEncryptDecrypt.Crypt;

	using FMPointTagArchive.Core;
	using FMPointTagArchive.Core.Interfaces;
	using FMPointTagArchive.Core.Interfaces.ServiceRequests;
	public class TankChangeProvider
	{
		private readonly ITankChangeProcessor TankChangeProcessor = new TankChangeProcessor();

		private readonly IDataSetTransmitter DataSetTransmitter = new DataSetTransmitter();

		private static readonly TankChangeProvider Provider = new TankChangeProvider();

		[SqlProcedure]
		public static void usp_TankChange(SqlGuid SiteGuid, string SiteID, DateTimeOffset BeginDate, DateTimeOffset EndDate, int SelectedType, SqlGuid UserGuid, bool useSmallFieldNames, string refDataTableAsXML, string cassandraConfiguration, string cassandraUsername, string cassandraPassword)
		{
			ValidateArguments(SiteGuid, SiteID, refDataTableAsXML, cassandraConfiguration.ToString(), cassandraUsername.ToString(), cassandraPassword.ToString());

			// here we will set the date based on the passed in flag
			SetDateBasedOnSelection(SelectedType, ref BeginDate, ref EndDate);

			var provider = new TankChangeProvider();
			provider.Generate(SiteGuid.Value, SiteID, BeginDate, EndDate, UserGuid.Value, useSmallFieldNames, refDataTableAsXML, cassandraConfiguration.ToString(), cassandraUsername.ToString(), cassandraPassword.ToString());
		}

		private static void ValidateArguments(SqlGuid SiteGuid, string SiteID, string refDataTableAsXML, string cassandraConfiguration, string username, string password)
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

			if (string.IsNullOrEmpty(username))
			{
				throw new ArgumentNullException("cassandraUsername");
			}

			if (string.IsNullOrEmpty(password))
			{
				throw new ArgumentNullException("cassandraPassword");
			}
		}

		private static void SetDateBasedOnSelection(int SelectedType, ref DateTimeOffset BeginDate, ref DateTimeOffset EndDate)
		{
			DateTimeOffset currentTime = DateTimeOffset.Now;
			bool exitLoop = false;
			int lastValue = 0;

			currentTime = currentTime.UtcDateTime.AddHours(-currentTime.UtcDateTime.Hour);
			currentTime = currentTime.UtcDateTime.AddMinutes(-currentTime.UtcDateTime.Minute);
			currentTime = currentTime.UtcDateTime.AddSeconds(-currentTime.UtcDateTime.Second);
			if (SelectedType == 2)	// last full day
			{
				// set the bigin date to 2 days ago and the end date to yesterday
				BeginDate = currentTime.AddDays(-2);
				EndDate = currentTime.AddDays(-1);
				EndDate = EndDate.AddMinutes(-1);
				return;
			}
			else if (SelectedType == 3)	// last full week
			{
				// decrement by one day so we have our start
				BeginDate = currentTime.AddDays(-1);
				EndDate = currentTime.AddDays(-1);
				// week is saturday through sunday decrement end until we get to saturday
				exitLoop = false;
				while (!exitLoop)
				{
					if (EndDate.DayOfWeek == DayOfWeek.Sunday)
					{
						BeginDate = EndDate.AddDays(-7);
						break;
					}
					EndDate = EndDate.AddDays(-1);
				}
				EndDate = EndDate.AddMinutes(-1);
				return;
			}
			else if (SelectedType == 4)	// last full month
			{
				BeginDate = currentTime.AddMonths(-1);
				EndDate = currentTime.AddMonths(-1);
				// decrement until the month changes
				exitLoop = false;
				lastValue = BeginDate.Month;
				while (!exitLoop)
				{
					if(lastValue != BeginDate.Month)
					{
						BeginDate = BeginDate.AddDays(1);
						break;
					}
					BeginDate = BeginDate.AddDays(-1);
				}

				exitLoop = false;
				lastValue = EndDate.Month;
				while (!exitLoop)
				{
					if (lastValue != EndDate.Month)
					{
						//EndDate = EndDate.AddDays(-1);
						break;
					}
					EndDate = EndDate.AddDays(1);
				}
				EndDate = EndDate.AddMinutes(-1);
				return;
			}
			else if (SelectedType == 5)  // change to day
			{
				// set the bigin date to 2 days ago and the end date to yesterday
				BeginDate = currentTime;
				EndDate = currentTime.AddDays(1);
				return;
			}
			else if (SelectedType == 6) // change to week
			{
				// decrement by one day so we have our start
				EndDate = currentTime.AddDays(1);
				BeginDate = currentTime.AddDays(-1);
				// week is saturday through sunday decrement end until we get to sunday
				exitLoop = false;
				while (!exitLoop)
				{
					if (BeginDate.DayOfWeek == DayOfWeek.Sunday)
					{
						//BeginDate = EndDate.AddDays(-7);
						break;
					}
					BeginDate = BeginDate.AddDays(-1);
				}
				return;
			}
			else if (SelectedType == 7) // change to month
			{
				BeginDate = currentTime.AddDays(-currentTime.Day);
				BeginDate = BeginDate.AddDays(1);
				EndDate = currentTime.AddDays(1);
				return;
			}
			else
			{
				return;
			}
		}

		private void Generate(Guid siteGuid, string siteID, DateTimeOffset reportDate, DateTimeOffset EndDate, Guid UserGuid, bool useSmallFieldNames, string refDataTableAsXML, string cassandraConfiguration, string cassandraUsername, string cassandraPassword)
		{


			string hex = BitConverter.ToString((Convert.FromBase64String(cassandraPassword)));
			hex = hex.Replace("-", "");
			string decrypted = PasswordEncrytpDecrpt.Decrypt(hex, new Guid("00000000-0000-0000-0000-000000000001"));

			var request = new TankChangeProcessorSR
			{
				SiteGuid = siteGuid,
				SiteID = siteID,
				BeginDate = reportDate,
				EndDate = EndDate,
				UseSmallFieldNames = useSmallFieldNames,
				refDataTableAsXML = refDataTableAsXML,
				CassandraConfiguration = cassandraConfiguration,
				UserGuid = UserGuid,
CassandraUsername = cassandraUsername,
CassandraPassword = decrypted
			};

			var dataSet = this.TankChangeProcessor.Process(request);

			// Transmit the dataset
			this.DataSetTransmitter.Transmit(dataSet);
		}
	}
}
