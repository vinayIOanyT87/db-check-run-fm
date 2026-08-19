using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class SaveWeightedAverageCostsSR : AccountingServiceRequest
	{
		[DataMember]
		protected List<WeightedAverageCostDO> m_wacList;

		public SaveWeightedAverageCostsSR ( SecurityClass a_security ) : base ( )
		{
			this.WeightedAverageCosts = new List<WeightedAverageCostDO> ( );

			base.Security = a_security;
		}

		#region Properties
		
		public List<WeightedAverageCostDO> WeightedAverageCosts
		{
			get { return m_wacList; }
			set { m_wacList = value; }
		}
		#endregion // Properties
	}
}
