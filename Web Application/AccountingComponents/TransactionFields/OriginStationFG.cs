using System;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for OriginStationFG.
	/// </summary>
	public class OriginStationFG : RouteStationGenerator, IHeaderField
	{
		public OriginStationFG()
		{

		}

		public override string FieldID
		{ get { return "OriginStationIATAID"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteInfo.OriginStationIATAID;
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
			transaction.RouteInfo.OriginStationIATAID = newValue as string;

			if (string.IsNullOrEmpty(transaction.RouteInfo.OriginStationIATAID))
				transaction.RouteInfo.OriginStationIATAGuid = Guid.Empty;
			else
			{
				transaction.RouteInfo.OriginStationIATAGuid = FMChannelHelper.MakeCall<IIATACodes, Guid>(
																	 x =>
																	 x.GetIdentityGuid(transContext.security, transaction.RouteInfo.OriginStationIATAID)
																);
			}

			OnFieldChanged();
		}
	}
}
