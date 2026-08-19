namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	using FMBusinessObjects.UtilityObjects;

	[DataContract(Namespace = "")]
	[Serializable]
	public class RateModuleSettings
	{
		#region Public Properties
		[DataMember(Order = 0)]
		public string Deadband { get; set; }
		[DataMember(Order = 1)]
		public int StaleTimePeriodInSeconds { get; set; }
		[DataMember(Order = 2)]
		public string FlowCalculationType { get; set; }
		[DataMember(Order = 3)]
		public int AveragingNumberSamples { get; set; }
		[DataMember(Order = 4)]
		public int AveragingSampleTimeSeconds { get; set; }
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public RateModuleSettings()
		{
			this.Init();
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method is a utility to get the serialize XML string for this object.
		/// It is used in the put in the database script "Script.StandardTank.sql".
		/// </summary>
		/// <returns>Returns the XML string of this object.</returns>
		public string GetXmlOfThisObject()
		{
			var stringwriter = new System.IO.StringWriter();
			var serializer = CachingXmlSerializerFactory.Create(this.GetType());
			serializer.Serialize(stringwriter, this);

			return stringwriter.ToString();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the object to its initial class.
		/// </summary>
		private void Init()
		{
			this.Deadband = "0";
			this.StaleTimePeriodInSeconds = 0;
			this.FlowCalculationType = "Averaging";
			this.AveragingNumberSamples = 4;
			this.AveragingSampleTimeSeconds = 30;
		}
		#endregion
	}
}
