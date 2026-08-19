using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using FMBusinessObjects.Interfaces;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class DataTransmission : BaseDataObject, IAlarmAndEventDiscovery
	{
		public DataTransmission()
		{
			m_SiteID = "";
			m_UserID = "";
		}

		public DataTransmission(string strSiteID, string strUserID)
		{
			m_SiteID = strSiteID;
			m_UserID = strUserID;
		}

		public DataTransmission(string strSiteID, string strUserID, string fromDate)
		{
			m_SiteID = strSiteID;
			m_UserID = strUserID;
			m_fromDate = fromDate;
		}

		#region Data Members
		[DataMember]
		public string m_SiteID;
		[DataMember]
		public string m_UserID;

		private string m_fromDate = "";

		static string TransmissionImportKey = "Data Transmission Import";
		static string TransmissionExportKey = "Data Transmission Export";
		static string TransmissionExportReProcessKey = "Data Transmission Export Re-process";

		public static AlarmAndEventDescriptorClass TransmissionImportEventDescriptor = new AlarmAndEventDescriptorClass(false, DataSynchronization, TransmissionImportKey);
		public static AlarmAndEventDescriptorClass TransmissionExportEventDescriptor = new AlarmAndEventDescriptorClass(false, DataSynchronization, TransmissionExportKey);
		public static AlarmAndEventDescriptorClass TransmissionExportReProcessEventDescriptor = new AlarmAndEventDescriptorClass(false, DataSynchronization, TransmissionExportReProcessKey);
		#endregion

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.DATA_TRANSMISSION; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		#region IAlarmAndEventDiscovery Members
		public AlarmAndEventDescriptorClass[] AlarmAndEvents
		{

			get
			{
				AlarmAndEventDescriptorClass[] Descriptors = { TransmissionImportEventDescriptor, TransmissionExportEventDescriptor, TransmissionExportReProcessEventDescriptor };
				return Descriptors;
			}

		}
		#endregion

		public AlarmAndEventLogClass TransmissionImportEventLog
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(TransmissionImportEventDescriptor);
				AlarmAndEventLog.AssociatedData = "UserID: " + m_UserID + ", Site ID:" + m_SiteID;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransmissionExportEventLog
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(TransmissionExportEventDescriptor);
				AlarmAndEventLog.AssociatedData = "UserID: " + m_UserID + ", Site ID:" + m_SiteID;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass TransmissionExportReProcessEventLog
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(TransmissionExportReProcessEventDescriptor);
				AlarmAndEventLog.AssociatedData = string.Format("Date: {0}, UserID: {1}, Site ID: {2}", m_fromDate, m_UserID, m_SiteID);
				return AlarmAndEventLog;
			}
		}
	}
}
