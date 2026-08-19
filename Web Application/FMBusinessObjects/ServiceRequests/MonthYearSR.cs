using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Interfaces;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class MonthYearSR : AccountingServiceRequest
    {
        #region Enumerations
        public enum MonthYearDetermiationType { Standard, EndOfMonth}
        #endregion

        #region Private data members
        [DataMember] bool useDataDictionary;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the month year service request class.
		/// </summary>
		public MonthYearSR ( )
		{
			this.useDataDictionary = false;
            this.DeterminationType = MonthYearDetermiationType.Standard;
		}

		public MonthYearSR(bool useDictionary)
		{
			this.useDataDictionary = useDictionary;
		}
		#endregion

		#region Properties
		public bool UseDataDictionary
		{
			get { return this.useDataDictionary; }
			set { this.useDataDictionary = value; }
		}

		[DataMember]
        public MonthYearDetermiationType DeterminationType 
		{ 
			get; 
			set; 
		}
		#endregion

		#region Public methods
		
		#endregion
	}
}
