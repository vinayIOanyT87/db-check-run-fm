//******************************************************************************
//	FILE NAME:		LineItemMassPackageSizeFG.cs
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
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	/// <summary>
	/// Summary description for LineItemMassPackageSizeFG.
	/// </summary>
	public class LineItemMassPackageSizeFG : TextFieldGenerator, ILineItemField, ISublineItemField
	{
		public LineItemMassPackageSizeFG()
		{
			bFieldRequired = false;
			virtualField = true;
		}

		public override string FieldID
		{
			get
			{
				return "LineItem MassPackageSize";
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

		protected override void SpecializeControl(WebControl control)
		{
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.ReadOnly = true;
					textBox.Enabled = false;
					textBox.BackColor = this.VarecBkgrndReadOnlyGray;
				}
			}
		}

		#region ILineItemField Members
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.ProductGuid == Guid.Empty)
			{
				return string.Empty;
			}

			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
														x =>
														x.GetByProductAuthorizedCompanies(this.transContext.security, inLineItem.ProductGuid, false)
												);

			return product.MassPackageSize;
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
		}

		public void SetNewValue(LineItemDO inLineItem)
		{
			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByID(this.transContext.security, inLineItem.Product)
																);

			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

				if (textBox != null)
				{
					textBox.Text = product.MassPackageSize;
				}
			}
		}
		#endregion

		#region ISublineItemField Members
		object ISublineItemField.GetDataValue(SubLineItemDO inSublineItem)
		{
			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByID(this.transContext.security, inSublineItem.Product)
																);

			return product.MassPackageSize;
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
		}
		#endregion
	}
}
