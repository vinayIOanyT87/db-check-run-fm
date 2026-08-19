namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for LineItemVCF_FG.
	/// </summary>
	public class LineItemVcfFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemVcfFG()
		{
		}

		public override string FieldID
		{
			get
			{
				return "LineItem Vcf";
			}
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 6.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return base.GetFieldLength(FieldID, 6);
			}
		}

		public override ENumericType NumericType
		{
			get
			{
				return ENumericType.Double;
			}
		}

		public override SITE_VARIABLE_TYPE UnitType
		{
			get
			{
				return SITE_VARIABLE_TYPE.VCF;
			}
		}

		override public string GetFormattedValue()
		{
			object dataValue = GetDataValue();

			if ((dataValue == null) || dataValue.Equals(string.Empty))
			{
				return string.Empty;
			}

			var formatInfo = this.fieldGenerator.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VCF);

/*			Disabling the precision selection as it is forcing a change detection when rounding is disabled in the VCF settings
			if ((double) dataValue >= 1)
			{
				formatInfo.NumberDecimalDigits = 4;
				return (Convert.ToDouble(dataValue).ToString("N4", formatInfo));
			}
*/
			formatInfo.NumberDecimalDigits = 5;
			return (Convert.ToDouble(dataValue).ToString("N5", formatInfo));
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			this.ManualValueFlag = inLineItem.Quantity.VcfManualValueFlag;

			if (inLineItem.VCF == null)
			{
				return null;
			}

			return inLineItem.VCF.Value;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (GetDataValue(inLineItem) != null)
			{
				return GetDataValue(inLineItem).ToString();
			}
			
			return null;
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if ((newValue == null) || (newValue.Equals(string.Empty)))
			{
				inLineItem.VCF = null;
			}
			else
			{
				inLineItem.VCF = (double) newValue;
			}

			inLineItem.Quantity.IsVcfDirty = true;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			this.ManualValueFlag = inSublineItem.Quantity.VcfManualValueFlag;

			if (inSublineItem.VCF == null)
			{
				return null;
			}
			
			return inSublineItem.VCF.Value;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}
			
			return null;
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem,
			object newValue)
		{
			if ((newValue == null) || (newValue.Equals(string.Empty)))
			{
				inSublineItem.VCF = null;
			}
			else
			{
				inSublineItem.VCF = (double) newValue;
			}

			inSublineItem.Quantity.IsVcfDirty = true;
			OnFieldChanged();
		}
		#endregion
	}
}
