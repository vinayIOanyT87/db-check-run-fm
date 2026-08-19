using System;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for PreviousStationFG.
	/// </summary>
	public class PreviousStationFG : RouteStationGenerator, IHeaderField
	{
		public PreviousStationFG()
		{
			
		}

		public override string FieldID
		{ get { return "PreviousStationIATAID"; } } 

		#region IHeaderField Members

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteInfo.PreviousStationIATAID;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}
			else
			{
				return null;
			}
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.RouteInfo.PreviousStationIATAID = newValue as string;

			if(string.IsNullOrEmpty(transaction.RouteInfo.PreviousStationIATAID))
				transaction.RouteInfo.PreviousStationIATAGuid=Guid.Empty;
			else
			{
				transaction.RouteInfo.PreviousStationIATAGuid = FMChannelHelper.MakeCall<IIATACodes, Guid>(
																	 x =>
																	 x.GetIdentityGuid(transContext.security, transaction.RouteInfo.PreviousStationIATAID)
																);
			}

			OnFieldChanged();
		}

		#endregion


	}
}
