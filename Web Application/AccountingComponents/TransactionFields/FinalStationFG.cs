using System;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

namespace TransactionFields
{
	/// <summary>
	/// Summary description for FinalStationFG.
	/// </summary>
	public class FinalStationFG : RouteStationGenerator, IHeaderField
	{
		public FinalStationFG()
		{

		}

		public override string FieldID
		{ get { return "FinalStationIATAID"; } }

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.RouteInfo.FinalStationIATAID;
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
			transaction.RouteInfo.FinalStationIATAID = newValue as string;

			if (string.IsNullOrEmpty(transaction.RouteInfo.FinalStationIATAID))
				transaction.RouteInfo.FinalStationIATAGuid = Guid.Empty;
			else
			{
				transaction.RouteInfo.FinalStationIATAGuid = FMChannelHelper.MakeCall<IIATACodes, Guid>(
																	 x =>
																	 x.GetIdentityGuid(transContext.security, transaction.RouteInfo.FinalStationIATAID)
																);
			}

			OnFieldChanged();
		}
	}
}
