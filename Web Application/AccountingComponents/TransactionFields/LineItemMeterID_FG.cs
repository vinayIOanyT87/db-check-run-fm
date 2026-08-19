namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// A transaction field control which allows a user to select a meter
	/// </summary>
	public class LineItemMeterIDFG : MeterIDTextButtonGenerator, ILineItemField, ISublineItemField
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public LineItemMeterIDFG()
		{
		}

		/// <summary>
		/// Get the field ID. This is used when generating the field based on the transaction alias configuration
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem MeterID";
			}
		}

		/// <summary>
		/// If this control is a combo box, this determines the maximum length of any input text
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength(FieldID, FIELD_LENGTH);
			}
		}

		#region ILineItemField Members
		/// <summary>
		/// Returns the line item's meter ID
		/// </summary>
		/// <param name="inLineItem">The line item to get the meter ID for</param>
		/// <returns>The line item's meter ID</returns>
		public object GetDataValue(LineItemDO inLineItem)
		{
			return inLineItem.MeterID;
		}

		/// <summary>
		/// Get the data in text format
		/// </summary>
		/// <param name="inLineItem">the line item to get the data for</param>
		/// <returns>The line item's meter ID</returns>
		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}

			return null;
		}

		/// <summary>
		/// Set the meter data on the Line item
		/// </summary>
		/// <param name="inLineItem">the Line item to set the meter data for</param>
		/// <param name="newValue">the meter ID</param>
		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			var meterID = newValue as string;

			MeterClass meter = GetMeterObject(meterID);

			if (meter == null)
			{
				inLineItem.MeterID = meterID;
				inLineItem.MeterGuid = Guid.Empty;
			}
			else
			{
				inLineItem.MeterID = meter.ID;
				inLineItem.MeterGuid = meter.IdentityGuid;
			}

			this.SetMeter();
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		/// <summary>
		/// Returns the sub line item's meter ID
		/// </summary>
		/// <param name="inSublineItem">The sub line item to get the meter ID for</param>
		/// <returns>The sub line item's meter ID</returns>
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			return inSublineItem.MeterID;
		}

		/// <summary>
		/// Set the meter data on the sub line item
		/// </summary>
		/// <param name="inSublineItem">the sub line item to set the meter data for</param>
		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}

			return null;
		}

		/// <summary>
		/// Set the meter data on the sublineitem
		/// </summary>
		/// <param name="inSublineItem">the sublineitem to set the meter data for</param>
		/// <param name="newValue">the meter ID</param>
		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			var meterID = newValue as string;

			MeterClass meter = GetMeterObject(meterID);

			if (meter == null)
			{
				inSublineItem.MeterID = meterID;
				inSublineItem.MeterGuid = Guid.Empty;
			}
			else
			{
				inSublineItem.MeterID = meter.ID;
				inSublineItem.MeterGuid = meter.IdentityGuid;
			}

			SetMeter();
			OnFieldChanged();
		}
		#endregion
	}
}
