namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ImproperAdditizationFG.
	/// </summary>
	public class ImproperAdditizationFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		private bool dataChanged;

		public ImproperAdditizationFG()
		{
		}

		public bool DataChanged { get { return dataChanged; } }
		public override string FieldID { get { return "LineItem ImproperAdditization"; } }
		public override bool Editable { get { return true; } }

		#region ILineItemField implementations
		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.ImproperAdditization == null)
			{
				return false;
			}
			return lineItem.ImproperAdditization.Value;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			if (inLineItem.ImproperAdditization == null)
			{
				return bool.FalseString;
			}

			return inLineItem.ImproperAdditization.Value.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue is Boolean)
			{
				inLineItem.ImproperAdditization = newValue as bool?;
			}
			else if (newValue is string)
			{
				lineItem.ImproperAdditization = new bool?(bool.Parse((string)newValue));
			}
			else
			{
				lineItem.ImproperAdditization = false;
			}

			foreach (SubLineItemDO subLine in inLineItem.SubLineItems)
			{
				if (subLine.ProductType != ProductClass.ProductTypeID(ProductType.AdditiveProduct))
				{
					continue;
				}

				subLine.ImproperAdditization = inLineItem.ImproperAdditization.Value;
			}

			dataChanged = true;

			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField implementations
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			if (inSublineItem.ImproperAdditization == null)
			{
				return false;
			}
			return inSublineItem.ImproperAdditization.Value;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			if (inSublineItem.ImproperAdditization == null)
			{
				return bool.FalseString;
			}

			return inSublineItem.ImproperAdditization.Value.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			if (newValue is Boolean)
			{
				inSublineItem.ImproperAdditization = (bool)newValue;
			}
			else if (newValue is string)
			{
				sublineItem.ImproperAdditization = new bool?(bool.Parse((string)newValue));
			}
			else
			{
				inSublineItem.ImproperAdditization = false;
			}

			if (inSublineItem.ProductType != ProductClass.ProductTypeID(ProductType.AdditiveProduct) &&
				inSublineItem.ImproperAdditization == null && inSublineItem.ImproperAdditization.Value)
			{
				inSublineItem.ImproperAdditization = false;
			}

			if (inSublineItem.ImproperAdditization.Value)
			{
				lineItem.ImproperAdditization = true;

				// Have to do the remaining additive subline items as well.  If one is improper, all are improper
				foreach (SubLineItemDO sliDO in lineItem.SubLineItems)
				{
					if (sliDO != null)
					{
						if (sliDO.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
						{
							sliDO.ImproperAdditization = true;
						}
					}
				}
			}

			dataChanged = true;

			OnFieldChanged();
		}
		#endregion
	}
}
