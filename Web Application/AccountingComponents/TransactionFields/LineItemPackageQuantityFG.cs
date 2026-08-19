//******************************************************************************
//	FILE NAME:		LineItemPackageQuantityFG.cs
//	PURPOSE:		This class inherits from NumericTextFieldGenerator and sets
//					the Mass value.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Sijuan Jiang
//	VERSION:		1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		
//*******************************************************************************       

namespace TransactionFields
{
	using System;
	using System.Globalization;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	/// <summary>
	/// Summary description for LineItemPackageQuantityFG.
	/// </summary>
	public class LineItemPackageQuantityFG : NumericTextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemPackageQuantityFG()
		{
			bFieldRequired = false;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem PackageQuantity";
			}
		}

		public override bool Required
		{
			get
			{
				return bFieldRequired;
			}
		}

		protected override short MaxColumns
		{
			get
			{
				return 10;
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
				return SITE_VARIABLE_TYPE.PACKAGE;
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			this.ManualValueFlag = lineItem.Quantity.PackageManualValueFlag;

			if (inLineItem.ProductGuid == Guid.Empty)
			{
				return null;
			}

			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
														x =>
														x.GetByProductAuthorizedCompanies(this.transContext.security, inLineItem.ProductGuid, false)
																);

			if (!product.LoadByWeight)
			{
				if (product._VolumePackageSize.Value != 0)
				{
					return Math.Round(
						inLineItem.Quantity.NetInventoryChange / product._VolumePackageSize.Value,
						product.VolumeDecimalPlaces,
						MidpointRounding.AwayFromZero);
				}

				this.bFieldEditible = false;

				if (product._MassPackageSize.Value != 0)
				{
					return Math.Round(
						inLineItem.Quantity.MassInventoryChange / product._MassPackageSize.Value,
						product.MassDecimalPlaces,
						MidpointRounding.AwayFromZero);
				}

				return null;
			}

			if (product._MassPackageSize.Value != 0)
			{
				return Math.Round(
					inLineItem.Quantity.MassInventoryChange / product._MassPackageSize.Value,
					product.MassDecimalPlaces,
					MidpointRounding.AwayFromZero);
			}

			this.bFieldEditible = false;

			if (product._VolumePackageSize.Value != 0)
			{
				return Math.Round(
					inLineItem.Quantity.NetInventoryChange / product._VolumePackageSize.Value,
					product.VolumeDecimalPlaces,
					MidpointRounding.AwayFromZero);
			}

			return null;
		}

		public string GetDataText(LineItemDO inLineItem)
		{
			object obj = GetDataValue(inLineItem);

			if (obj == null)
			{
				return null;
			}
			
			return obj.ToString();
		}

		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.Quantity.NullablePackage = null;
			}
			else
			{
				inLineItem.Quantity.NullablePackage = (double) newValue;
			}

			inLineItem.Quantity.IsPackageDirty = true;
			OnFieldChanged();
		}

		override public string GetFormattedValue()
		{
			object dataValue = GetDataValue();

			if (dataValue == null)
			{
				return string.Empty;
			}

			return DataObject.getDouble(dataValue).ToString(CultureInfo.InvariantCulture);
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			this.ManualValueFlag = sublineItem.Quantity.PackageManualValueFlag;

			if (inSublineItem.ProductGuid == Guid.Empty)
			{
				return null;
			}

			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
														 x =>
														 x.GetByProductAuthorizedCompanies(this.transContext.security, lineItem.ProductGuid, false)
													);

			if (!product.LoadByWeight)
			{
				if (product._VolumePackageSize.Value != 0)
				{
					return Math.Round(inSublineItem.Quantity.NetInventoryChange / product._VolumePackageSize.Value, product.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
				}

				this.bFieldEditible = false;

				if (product._MassPackageSize.Value != 0)
				{
					return Math.Round(inSublineItem.Quantity.MassInventoryChange / product._MassPackageSize.Value, product.MassDecimalPlaces, MidpointRounding.AwayFromZero);
				}
				
				return null;
			}

			if (product._MassPackageSize.Value != 0)
			{
				return Math.Round(inSublineItem.Quantity.MassInventoryChange / product._MassPackageSize.Value, product.MassDecimalPlaces, MidpointRounding.AwayFromZero);
			}
			
			this.bFieldEditible = false;

			if (product._VolumePackageSize.Value != 0)
			{
				return Math.Round(inSublineItem.Quantity.NetInventoryChange / product._VolumePackageSize.Value, product.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
			}
			
			return null;
		}

		string ISublineItemField.GetDataText(SubLineItemDO inSublineItem)
		{
			object obj = ((ISublineItemField) this).GetDataValue(inSublineItem);

			if (obj == null)
			{
				return null;
			}

			return obj.ToString();
		}

		void ISublineItemField.SetDataValue(SubLineItemDO inSublineItem, object newValue)
		{
			if (newValue == null)
			{
				return;
			}

			if (inSublineItem.Quantity.PackageInventoryChange != (double) newValue)
			{
				inSublineItem.Quantity.PackageInventoryChange = (double) newValue;
				inSublineItem.Quantity.IsPackageDirty = true;
			}

			OnFieldChanged();
		}
		#endregion
	}
}
