namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    /// <summary>
	/// Summary description for LineItemMeterStartFG.
	/// </summary>
	public class LineItemMeterStartFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemMeterStartFG()
		{
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 10.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return base.GetFieldLength(FieldID, 10);
			}
		}

		public override string FieldID
		{
			get
			{
				return "LineItem MeterStart";
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
				string additiveTypeID = ProductClass.ProductTypeID( ProductType.AdditiveProduct );
				string productID = "";
				string productType = "";

				if ( this.sublineItem != null 
					&& this.sublineItem.Product != null )
				{
					productID = this.sublineItem.Product;
					productType = this.sublineItem.ProductType;
				}
				
				else if ( this.lineItem != null 
					&& this.lineItem.Product != null )
				{
					productID = this.lineItem.Product;
					productType = this.lineItem.ProductType;
				}

				if (string.IsNullOrEmpty(productType))
				{
					if (!string.IsNullOrEmpty(productID))
					{
						ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByID(this.transContext.security, productID)
																);

						productType = ProductClass.ProductTypeID(product.ProductType);
					}
				}

				if (productType == additiveTypeID)
				{
					return SITE_VARIABLE_TYPE.ADDITIVE_VOLUME;
				}

				return SITE_VARIABLE_TYPE.VOLUME;
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.MeterReading.MeterStart == null)
			{
				return null;
			}

			return inLineItem.MeterReading.MeterStart.Value;
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
			if (newValue == null)
			{
				inLineItem.MeterReading.MeterStart = null;
			}
			else
			{
				inLineItem.MeterReading.MeterStart = (double) newValue;
			}

			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			if (inSublineItem.MeterReading.MeterStart == null)
			{
				return null;
			}

			return inSublineItem.MeterReading.MeterStart.Value;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (((ISublineItemField) this).GetDataValue(inSublineItem) != null)
			{
				return ((ISublineItemField) this).GetDataValue(inSublineItem).ToString();
			}

			return null;
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			if (newValue == null)
			{
				inSublineItem.MeterReading.MeterStart = null;
			}
			else
			{
				inSublineItem.MeterReading.MeterStart = (double) newValue;
			}

			OnFieldChanged();
		}
		#endregion

		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox == null)
				{
					return;
				}

				string meterStartID = control.Controls[0].ClientID;
				string meterStopID = meterStartID.Replace("Start", "Stop");
				string meterTotalID = meterStopID.Replace("Stop", "Total");
				string grossID = meterStartID.Replace("MeterStart", "GrossQuantity");

				textBox.Attributes.Add("onChange",
					"javascript:MeterStartStopChange(\"" + meterStartID + "\", \""
					+ meterStopID + "\", \"" + grossID + "\", \"" + meterTotalID + "\", \""
					+ this.transContext.accountingSite.CurrentSite.NumberDecimalSeparator + "\", \""
					+ this.transContext.accountingSite.CurrentSite.VolumeDecimalPlaces + "\");");
			}
		}
	}
}
