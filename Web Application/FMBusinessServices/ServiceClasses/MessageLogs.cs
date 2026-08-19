using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Web;
using System.Data.SqlClient;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;

using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	using FMBusinessObjects.Exceptions;

	[SecuritySafeCriticalAttribute]
	[ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class MessageLogsClass : IMessageLogs
	{
		internal ConsolidatedDAClass ConsolidatedDA = new ConsolidatedDAClass();

		public MessageLogsClass()
		{
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void Add(SecurityClass security, MessageLogClass MessageLog)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (MessageLog == null)
				throw new ArgumentNullException("MessageLog");

			if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			MessagesClass Messages = new MessagesClass();
			MessageClass Message = Messages.Get(security, MessageLog.MessageGuid);
			CompaniesClass Companies = new CompaniesClass();
			CompanyClass Company = Companies.Get(security, MessageLog.CompanyGuid);
			PersonnelClass Personnel = new PersonnelClass();
			PersonClass Person = Personnel.Get(security, MessageLog.PersonnelGuid);

			AlarmAndEventLogsClass AlarmAndEvents = new AlarmAndEventLogsClass();
			AlarmAndEvents.Add(security, Message.MessageLogEvent(Company.ID, Person.ID));

			if (Message._FrequencyType == MessageFrequencyType.Once)
			{
				bool bPurge = true;

				if (Message.CompanyGuid == Guid.Empty
				|| Message.PersonnelGuid == Guid.Empty)
				{
                    // Check Loaders first
					PersonCollectionClass Drivers = Personnel.EnumerateByRole(security, PERSON_ROLE.LOADER_ROLE);
					foreach (PersonClass Driver in Drivers)
					{
						if (Message.CompanyGuid != Guid.Empty
						&& Driver.CompanyGuid != Message.CompanyGuid)
							continue;

						if (Driver.MasterRecordGuid == MessageLog.PersonnelGuid)
							continue;

						MessageLogClass DriverLog = Get(security, Message.IdentityGuid, Driver.CompanyGuid, Driver.MasterRecordGuid);
						if (DriverLog.MessageGuid == Guid.Empty)
						{
							bPurge = false;
							break;
						}
					}

                    // Now check Offloaders
                    Drivers = Personnel.EnumerateByRole(security, PERSON_ROLE.OFFLOADER_ROLE);
                    foreach (PersonClass Driver in Drivers)
                    {
                        if (Message.CompanyGuid != Guid.Empty
                        && Driver.CompanyGuid != Message.CompanyGuid)
                            continue;

                        if (Driver.MasterRecordGuid == MessageLog.PersonnelGuid)
                            continue;

                        MessageLogClass DriverLog = Get(security, Message.IdentityGuid, Driver.CompanyGuid, Driver.MasterRecordGuid);
                        if (DriverLog.MessageGuid == Guid.Empty)
                        {
                            bPurge = false;
                            break;
                        }
                    }

                    // All drivers have been issued the message
                    if (bPurge == true)
					{
						Messages.Purge(security, MessageLog.MessageGuid);
						MessageLog = null;
					}
				}

				else
				{
					Messages.Purge(security, MessageLog.MessageGuid);
					MessageLog = null;
				}
			}

			if (MessageLog != null)
			{
				MessageLog.CreatedDate = DateTimeOffset.Now;
				MessageLog.CreatedBy = security.UserID;

				using (SqlCommand cmd = new SqlCommand())
				{
					MessageLog.IdentityGuid = Guid.NewGuid();
					MessageLog.InsertSQL(cmd);
					ConsolidatedDA.GetDataSet(cmd, security);
				}
			}
		}




		public MessageLogClass Get(SecurityClass security,
											Guid messageGuid,
											Guid companyGuid,
											Guid personnelGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			MessageLogClass MessageLog = new MessageLogClass();
			MessageLog.MessageGuid = messageGuid;
			MessageLog.CompanyGuid = companyGuid;
			MessageLog.PersonnelGuid = personnelGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				MessageLog.SelectSQL(cmd, ContextUtil.IsInTransaction);
				MessageLog.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}
			return MessageLog;
		}

		public MessageLogClass GetToday(SecurityClass security,
											Guid messageGuid,
											Guid companyGuid,
											Guid personnelGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA)
			&& !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			MessageLogClass MessageLog = new MessageLogClass();
			MessageLog.MessageGuid = messageGuid;
			MessageLog.CompanyGuid = companyGuid;
			MessageLog.PersonnelGuid = personnelGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				MessageLog.SelectTodaySQL(cmd);
				MessageLog.Load(ConsolidatedDA.GetDataSet(cmd, security));
			}

			return MessageLog;
		}

		public void Purge(SecurityClass security, Guid messageGuid)
		{
			if (security == null)
				throw new ArgumentNullException("Security");

			if (!security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
				throw new FMInsufficientRightsException();

			MessageLogClass MessageLog = new MessageLogClass();
			MessageLog.MessageGuid = messageGuid;

			using (SqlCommand cmd = new SqlCommand())
			{
				MessageLog.PurgeSQL(cmd);
				ConsolidatedDA.ExecuteQuery(security, cmd);
			}
		}
	}

}
