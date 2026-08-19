// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GateFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   GateFG.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TransactionFields
{
	using System;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using System.Collections.Specialized;

	/// <summary>
	/// Summary description for Gate.
	/// </summary>
	public class GateFG : DropDownGenerator, IHeaderField
	{

		public GateFG()
		{
			// This is a virtual field because standard field skips all Guids
			virtualField = true;
		}

		public override string FieldID
		{ 
			get 
			{
				return "GateID"; 
			} 
		}

		public object GetDataValue(TransactionDO transaction)
		{
			return transaction.GateID;
		}

		public string GetDataText(TransactionDO transaction)
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		public void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.GateID = newValue as string;
			transaction.GateGuid = FMChannelHelper.MakeCall<IGates, Guid>(
																	 x =>
																	 x.GetIdentityGuid(transContext.security, transaction.GateID)
																);
			OnFieldChanged();
		}

		public override HybridDictionary GetEntries()
		{
			GateCollectionClass gateList =
				FMChannelHelper.MakeCall<IGates, GateCollectionClass>(
						x =>
						x.Enumerate(transContext.security));
			HybridDictionary listEntries = new HybridDictionary(gateList.Count, false);
			foreach (GateClass Gate in gateList)
			{
				listEntries.Add(Gate.ID, Gate.ID);
			}
			
			return listEntries;
		}
	}
}