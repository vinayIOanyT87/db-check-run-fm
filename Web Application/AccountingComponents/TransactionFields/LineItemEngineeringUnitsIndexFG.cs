 #pragma warning disable 1587
/// <summary>
/// File name:	LineItemEngineeringUnitsIndexFG.cs
/// Purpose:	
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	--------------------------------------------
///		2007-09-18		I.Orndorff				7.3.0.0 - Added new transaction type (T18_SupplyOrder). 
/// </summary>
#pragma warning restore 1587
namespace TransactionFields
{
    using System;
    using System.Collections.Specialized;
    using System.Security;

    using FMBusinessObjects.DataObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Summary description for LineItemEngineeringUnitsIndexFG.
	/// </summary>
	public class LineItemEngineeringUnitsIndexFG : DropDownGenerator, ILineItemField
	{
		public LineItemEngineeringUnitsIndexFG()
		{
		}

		override public bool Editable
		{
			get
			{
				return ((this.trans.TransTypeID == TransactionTypes.T17_Order) ||
						(this.trans.TransTypeID == TransactionTypes.T18_SupplyOrder));
			}
		}

		public override string FieldID
		{
			get
			{
				return "LineItem EngineeringUnitsIndex";
			}
		}

		protected override string SelectText
		{
			get
			{
				return "Select a unit";
			}
		}

		public override bool Required => false;

        [SecurityCritical]
		public override HybridDictionary GetEntries()
		{
			var listEntries = new HybridDictionary(false);

			string abbreviation;

			// Add volume
			for (EngineeringUnit index = EngineeringUnit.FmvCm3; index <= EngineeringUnit.FmvKl; ++index)
			{
				abbreviation = EngineeringUnits.GetUnitAbbreviation(index);

				listEntries.Add(abbreviation, index.ToString());
			}

			// Add mass
			for (EngineeringUnit index = EngineeringUnit.FmmGram; index <= EngineeringUnit.FmmMlbs; ++index)
			{
				abbreviation = EngineeringUnits.GetUnitAbbreviation(index);

				listEntries.Add(abbreviation, index.ToString());
			}

			return listEntries;
		}

		[SecurityCritical]
		virtual public object GetDataValue(LineItemDO inLineItem)
		{
			string value = string.Empty;

			try
			{
				value = EngineeringUnits.GetUnitAbbreviation(inLineItem.EngineeringUnitsIndex);
			}
			catch
			{
			}

			return value;
		}


		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		virtual public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.EngineeringUnitsIndex = EngineeringUnit.FmvCm3;
			}
			else
			{
				inLineItem.EngineeringUnitsIndex = (EngineeringUnit) Enum.Parse(typeof(EngineeringUnit), newValue.ToString());
			}

			OnFieldChanged();
		}
	}
}
