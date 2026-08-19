namespace TransactionFields
{
	using System;
	using System.Globalization;
	using System.Web.UI;
	using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    class LineItemMeterTotalFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Line Item Meter Total field generator.
		/// </summary>
		public LineItemMeterTotalFG()
		{
			virtualField = true;
		}
		#endregion

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
				return "LineItem MeterTotal";
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

		/// <summary>
		/// This property will return true if the field is editable.
		/// Otherwise, it will return false.
		/// </summary>
		public override bool Editable
		{
			get
			{
				if (transContext == null || transContext.aliasClass == null
				    || transContext.aliasClass.LineItemFieldCollection.Find("MeterStart") == null
				    || transContext.aliasClass.LineItemFieldCollection.Find("MeterStop") == null)
				{
					bFieldEditible = true;
				}
				else
				{
					bFieldEditible = false;
				}

				return bFieldEditible;
			}
			set
			{
				bFieldEditible = value;
			}
		}

		public override void Generate(bool editable)
		{
			base.Generate(editable);
			var meterStartFG = fieldGenerator.GetFieldGenerator("LineItem MeterStart") as LineItemMeterStartFG;
			var meterStopFG = fieldGenerator.GetFieldGenerator("LineItem MeterStop") as LineItemMeterStopFG;

			if (meterStartFG != null)
			{
				meterStartFG.FieldChanged += this.OnMeterChanged;
			}

			if (meterStopFG != null)
			{
				meterStopFG.FieldChanged += this.OnMeterChanged;
			}
		}

		protected void OnMeterChanged(FieldGenerator meterFG)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.Text = this.GetFormattedValue();
				}
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.MeterReading.MeterStart == null || inLineItem.MeterReading.MeterStop == null)
			{
				return null;
			}

			if (inLineItem.MeterReading.MeterStop.Value > inLineItem.MeterReading.MeterStart.Value)
			{
				return inLineItem.MeterReading.MeterStop.Value - inLineItem.MeterReading.MeterStart.Value;
			}

			double rollOver		= inLineItem.MeterReading.MeterStart.Value;
			string rollOverText = Convert.ToInt32(rollOver).ToString(CultureInfo.InvariantCulture);
			rollOverText		= "".PadRight(rollOverText.Length, '9');
			rollOver			= Convert.ToDouble(rollOverText) + 1;

			return inLineItem.MeterReading.MeterStop.Value + (rollOver - inLineItem.MeterReading.MeterStart.Value);
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
			if (bFieldEditible)
			{
				if (newValue == null)
				{
					inLineItem.MeterReading.MeterStart = null;
					inLineItem.MeterReading.MeterStop = null;
				}
				else
				{
					inLineItem.MeterReading.MeterStart = 0;
					inLineItem.MeterReading.MeterStop = (double) newValue;
				}

				OnFieldChanged();
			}
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			if (inSublineItem.MeterReading.MeterStart == null || inSublineItem.MeterReading.MeterStop == null)
			{
				return null;
			}

			if (inSublineItem.MeterReading.MeterStop.Value > inSublineItem.MeterReading.MeterStart.Value)
			{
				return inSublineItem.MeterReading.MeterStop.Value - inSublineItem.MeterReading.MeterStart.Value;
			}

			double rollOver		= inSublineItem.MeterReading.MeterStart.Value;
			string rollOverText = Convert.ToInt32(rollOver).ToString(CultureInfo.InvariantCulture);
			rollOverText		= "".PadRight(rollOverText.Length, '9');
			rollOver			= Convert.ToDouble(rollOverText) + 1;

			return inSublineItem.MeterReading.MeterStop.Value + (rollOver - inSublineItem.MeterReading.MeterStart.Value);
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
			if (bFieldEditible)
			{
				if (newValue == null)
				{
					inSublineItem.MeterReading.MeterStart = null;
					inSublineItem.MeterReading.MeterStop = null;
				}
				else
				{
					inSublineItem.MeterReading.MeterStart = 0;
					inSublineItem.MeterReading.MeterStop = (double) newValue;
				}

				OnFieldChanged();
			}
		}
		#endregion
	}
}
