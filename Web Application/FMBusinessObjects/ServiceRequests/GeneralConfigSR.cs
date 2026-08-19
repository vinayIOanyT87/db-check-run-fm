using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	[KnownType(typeof(GeneralConfigDO))]
	public class GeneralConfigSR : AccountingServiceRequest
	{
		#region Public Attributes
		public enum GeneralConfigurationRequests { SAVE_CONFIGURATION, GET_CONFIGURATION, GET_CONFIGURATION_EXCLUDE_ALIASES, PURGE, NONE };
		public enum GeneralConfigAdjustMethod { MANUAL = 0, ALLOCATION = 1, THROUGHPUT = 2 };
		[DataMember] public Guid SiteGuid;
		#endregion

		#region private attributes
		[DataMember] private GeneralConfigDO generalConfigDO;
		[DataMember] private GeneralConfigurationRequests request;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the general configuration service request class.
		/// </summary>
		public GeneralConfigSR()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will set and get the general configuration data object attribute.
		/// </summary>
		public GeneralConfigDO GeneralConfigurationDO
		{
			get { return this.generalConfigDO; }
			set { this.generalConfigDO = value; }
		}

		/// <summary>
		/// This property will set and get the configuration request attribute.
		/// </summary>
		public GeneralConfigurationRequests Request
		{
			get { return this.request; }
			set { this.request = value; }
		}
		#endregion

		#region private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.request = GeneralConfigSR.GeneralConfigurationRequests.NONE;
			this.generalConfigDO = new GeneralConfigDO();
			this.SiteGuid = Guid.Empty;
		}
		#endregion
	}
}
