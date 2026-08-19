///***************************************************************************
/// Module Name:  ReasonCodeFG
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

using System;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using System.Collections.Specialized;


namespace TransactionFields
{
	/// <summary>
	/// Summary description for ReasonCode.
	/// </summary>
	public class ReasonCodeFG : DropDownGenerator, IHeaderField
	{
        /// <summary>
        ///     This property will returned either a figured data length or the
        ///     default length of 20.
        /// </summary>
        protected override short MaxColumns
        {
            get
            {
                return this.GetFieldLength(this.FieldID, 50);
            }
        }

		public ReasonCodeFG()
		{
			// This is a virtual field because standard field skips all Guids
			virtualField = true;
		}

		public override string FieldID
		{ 
			get 
			{
				return AutoDistributionReasonCodeClass.TransactionFieldID; 
			} 
		}

		public object GetDataValue(TransactionDO trans)
		{
			// This has to return string, otherwise, DropDownGenerator won't pick up the value.
			string retValue = null;
			if (trans.ReasonCodeGuid != Guid.Empty)
			{
				retValue = trans.ReasonCodeGuid.ToString();
			}
			return retValue;
		}

		public string GetDataText(TransactionDO trans)
		{
			return GetDataValue(trans) as string;
		}

		public void SetDataValue(TransactionDO trans, object newValue)
		{

			string newString = newValue as string;
			Guid newReasonCodeGuid;

			if (string.IsNullOrEmpty(newString) ||
				Guid.TryParse(newString, out newReasonCodeGuid) == false)
			{
				newReasonCodeGuid = Guid.Empty;
			}

			trans.ReasonCodeGuid = newReasonCodeGuid;

			OnFieldChanged();
		}

		public override HybridDictionary GetEntries()
		{
			AutoDistributionReasonCodeCollectionClass reasonCodeList =
				FMChannelHelper.MakeCall<IAutoDistributionReasonCodes, AutoDistributionReasonCodeCollectionClass>(
						x =>
						x.Enumerate(transContext.security)
				);
			HybridDictionary listEntries = new HybridDictionary(reasonCodeList.Count, false);
			foreach (AutoDistributionReasonCodeClass reasonCode in reasonCodeList)
			{
				string displayText = string.Format("{0} - {1}", reasonCode.ID, reasonCode.Description);
				listEntries.Add(displayText, reasonCode.IdentityGuid.ToString());
			}
			
			return listEntries;
		}

	}
}
