namespace FMBusinessObjects.DataObjects
{
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.UtilityObjects;
	
	[SuppressMessage ( "Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix" ), System.Serializable]
	[DataContract]
   	public class AccountingDataDictionary
	{
		#region Attributes
	
		private const int EMPTY_STRING = 0;
		[DataMember]
		private SecurityClass security;
		[DataMember]
		private bool useDataDictionary;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Data Dictionary class.
		/// It will make a call to the database via the accounting services
		/// to retreive the data dictionary data.
		/// </summary>
		/// <param name="accountingClient"></param>
		public AccountingDataDictionary ( SecurityClass security, bool useDataDictionary )
		{
			this.security = security;
			this.useDataDictionary = useDataDictionary;
		}
		#endregion


		#region Public Methods
		/// <summary>
		/// This method returns the Global data dictionary value.  It wraps the shared
		/// components data dictionary to facilate a simpler interface.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string getNameFromGlobalDictionary ( string key )
		{
			if (!useDataDictionary)
			{
				return key;
			}

			return DataDictionarySingleton.Get(security.LoginSiteGuid, key);
		}

		#endregion

		#region Properties
		/// <summary>
		/// This property gets the use data dictionary value.
		/// </summary>
		public bool UseDataDictionary
		{
			get { return this.useDataDictionary; }
			private set { this.useDataDictionary = value; }
		}
		#endregion
	}
}
