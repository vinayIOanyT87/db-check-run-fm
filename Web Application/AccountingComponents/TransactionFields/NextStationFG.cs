using System;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for NextStationFG.
	/// </summary>
	public class NextStationFG : RouteStationGenerator, IHeaderField
	{
		public NextStationFG()
		{

		}

		public override string FieldID
		{ get { return "NextStationIATAID"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteInfo.NextStationIATAID;
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
			transaction.RouteInfo.NextStationIATAID = newValue as string;

			if (string.IsNullOrEmpty(transaction.RouteInfo.NextStationIATAID))
				transaction.RouteInfo.NextStationIATAGuid = Guid.Empty;
			else
			{
				transaction.RouteInfo.NextStationIATAGuid = FMChannelHelper.MakeCall<IIATACodes, Guid>(
																	 x =>
																	 x.GetIdentityGuid(transContext.security, transaction.RouteInfo.NextStationIATAID)
																);
			}

			OnFieldChanged();
		}
	}
}
