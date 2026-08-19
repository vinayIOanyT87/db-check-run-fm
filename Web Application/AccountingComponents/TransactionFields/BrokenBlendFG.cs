namespace TransactionFields
{
	using System;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ImproperAdditizationFG.
	/// </summary>
	public class BrokenBlendFG : CheckBoxGenerator, ILineItemField, ISublineItemField
	{
		private bool dataChanged;

		public BrokenBlendFG()
		{
		}

		public bool DataChanged { get { return dataChanged; } }
		public override string FieldID { get { return "LineItem BrokenBlend"; } }
		public override bool Editable { get { return true; } }

		#region ILineItemField implementations
		public object GetDataValue(LineItemDO lineItem)
		{
			if (lineItem.BrokenBlend == null)
			{
				return false;
			}
			return lineItem.BrokenBlend.Value;
		}

		public string GetDataText(LineItemDO lineItem)
		{
			if (lineItem.BrokenBlend == null)
			{
				return bool.FalseString;
			}
			return lineItem.BrokenBlend.Value.ToString();
		}

		public void SetDataValue(LineItemDO lineItem, object newValue)
		{
			if (newValue is Boolean)
			{
				lineItem.BrokenBlend = (bool)newValue;
			}
			else if (newValue is string)
			{
				lineItem.BrokenBlend = bool.Parse(newValue as string);
			}
			else
			{
				lineItem.BrokenBlend = false;
			}

			foreach (SubLineItemDO subLine in lineItem.SubLineItems)
			{
				if (subLine.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct))
				{
					continue;
				}

				subLine.BrokenBlend = lineItem.BrokenBlend.Value;
			}

			dataChanged = true;
			OnFieldChanged();
		}
		#endregion

		#region ISublineItemField implementations
		object ISublineItemField.GetDataValue(SubLineItemDO sublineItem)
		{
			if (sublineItem.BrokenBlend == null)
			{
				return false;
			}
			return sublineItem.BrokenBlend.Value;
		}

		string ISublineItemField.GetDataText(SubLineItemDO sublineItem)
		{
			if (sublineItem.BrokenBlend == null)
			{
				return bool.FalseString;
			}
			return sublineItem.BrokenBlend.Value.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO sublineItem, object newValue)
		{
			if (newValue is Boolean)
			{
				sublineItem.BrokenBlend = (bool)newValue;
			}
			else if (newValue is string)
			{
				sublineItem.BrokenBlend = bool.Parse(newValue as string);
			}
			else
			{
				sublineItem.BrokenBlend = false;
			}

			if (sublineItem.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct) &&
				(sublineItem.BrokenBlend != null && sublineItem.BrokenBlend.Value == true))
			{
				sublineItem.BrokenBlend = false;
			}

			if (sublineItem.BrokenBlend.Value)
			{
				lineItem.BrokenBlend = true;
			}

			if (sublineItem.BrokenBlend.Value)
			{
				lineItem.BrokenBlend = true;

				// Have to do the remaining additive subline items as well.  If one is improper, all are improper
				foreach (SubLineItemDO sliDO in lineItem.SubLineItems)
				{
					if (sliDO != null)
					{
						if (sliDO.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
						{
							sliDO.BrokenBlend = true;
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
