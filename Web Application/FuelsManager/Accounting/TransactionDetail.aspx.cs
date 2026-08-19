// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionDetail.aspx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the TransactionDetail type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Drawing;
	using System.Globalization;
	using System.Net.Sockets;
	using System.Reflection;
	using System.Security;
	using System.ServiceModel;
	using System.ServiceProcess;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	using System.Linq;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using FMCore;

	using FMWebApp;

	using TransactionFields;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using Varec.CommonComponents.VolumeCorrection;

    /// <summary>
    ///	Summary description for TransactionDetail.
    /// </summary>
    public partial class TransactionDetailBase : AccountingAutoSubmitWebFormViewAjax
	{
		#region Constants and Fields
		public const string AliasKey = "AliasName";
		public const string ModeKey = "TransactionDetailMode";
		public const string ReturnPageKey = "TransactionDetailReturnPage";
		public const string SessionLineItemObject = "TransactionDetail.LineItemObject";
		public OrderAssociatedTxContext OrderContext;
		public SupplyOrderAssociatedTxContext SupplyOrderContext;

		protected const string CombineTransKey = "TransactionDetailCombineTransID";
		protected const string MeterReadingsKey = "TransactionDetailMeterReadings";
		protected const string OriginalTransKey = "TransactionDetailOriginalTransaction";
		protected const string SessionLineItemAdded = "TransactionDetail.NewLineItem";
		protected const string TransKey = "TransactionDetailTransaction";
		protected FMButton AddTxButton;
		protected TransactionAliasClass AliasObject;
		protected TextBox NotesTextBox;
		protected string OrderProduct = string.Empty;
		protected string OrderProductCode = string.Empty;
		protected Guid OrderProductGuid = Guid.Empty;
		protected string OrderReferenceID = string.Empty;
		protected Panel Panel1;
		protected FMButton ViewTxButton;
		protected AccountingSite AccountingSite;
		protected AGRGridGenerator AgrGridGenerator;
		protected string CustomScriptName = string.Empty;
		protected LineItemGridGenerator LineItemGridGenerator;
		protected Logger Logger;
		protected bool NoSaveErrors = true;
		protected bool RetrieveDataFromPageFlag = true;
		protected DateTimeOffset StartTime;
		protected TransactionDO Trans;
		// ReSharper disable once InconsistentNaming
		protected string transAlias;
		protected TransactionContext TransContext;
		protected TransactionFieldGenerator TransactionFieldGenerator;
		protected TransportInfoGridGenerator TransportInfoGridGenerator;
		protected bool IsAdfKey;
		protected virtual bool AllowCrossSiteTranactions
		{
			get
			{
				return false;
			}
		}

		private const string CustomClientScriptsJs = "CustomClientScriptsJS";
		private const string CustomClientScriptName = "CUSTOM_CLIENT_SCRIPT_NAME";
		private const string MissingCurrentyUnitMsg = "Missing Currency Unit selection field.";
		private const string MissingNonDomesticPriceMsg = "Missing Non-Domestic Price field.";
		private const string MissingSelectionInCurrentUnitMsg = "Missing a selection in Currency Units field.";
		private const string MissingValueInNonDomesticPriceMsg = "Missing a value in Non-Domestic Price field.";
		private const string SessionGaugeReadingGridInAddMode = "GaugeReadingGridInAddMode";
		private const string SessionSublineItemObject = "TransactionDetail.SubLineItemObject";

		private static string javascriptEnvokeBolSummary = @"
			<script type='text/javascript'>
			<!--
				showModalDialogFrame({
					url: '../LRWebApp/BillOfLadingsForm.aspx?Select=true&',
					width: 955,
					height: 560,
					doPostBackAfterCloseCallback: false,
					onClose: function ()
					{
						if (this.returnValue != null)
						{
							__mydoPostBack( 'COMBINE_TRANSACTION', this.returnValue[0] );	
						}
					}
				});

			-->
			</script>
			";

		private string orderTxReferenceID = string.Empty;

		private bool bTransIDBeingLoaded;
		#endregion

		#region Enums
		// ReSharper disable once InconsistentNaming
		protected enum SaveTypes { SAVE, NewButtonSave };
		#endregion

		#region Properties
		internal string TransAlias
		{
			get { return this.transAlias; }
			set { this.transAlias = value; }
		}

		protected virtual bool IsTransactionEditable
		{
			get
			{
				// make sure the user has permissions to the alias
				var hasPermission = FMChannelHelper.MakeCall<ITransactionAliases, bool>(
						x => x.UserHasModifyPermissions(this.security, this.Trans.TransactionAliasGuid));

				if (!hasPermission)
				{

					return false;
				}

				// If this is an Order type transaction, check the order security rights
				if ((this.Trans.TransTypeID == TransactionTypes.T17_Order)
					|| (this.Trans.TransTypeID == TransactionTypes.T18_SupplyOrder))
				{
					// If the order has been saved, only allow modify rights to change it
					return this.security.HasModifyTransactionRightByAliasName(this.Trans.Alias);
				}

				// Return not editable (false) if there is a closeout date. 
				if (this.Trans.CloseoutDate != null)
				{
					return false;
				}

				if (this.Trans.PartialCloseout)
				{
					return false;
				}

				if (this.Trans.SiteGuid == Guid.Empty)
				{
					return false;
				}

				if (this.Trans.Status == TransactionStatus.Posted || this.Trans.Status == TransactionStatus.Pending)
				{
					return false;
				}

				if (this.Trans.ReversalType == TransactionDO.ReversalWithUpdate
					|| this.Trans.ReversalType == TransactionDO.UpdateOriginal || this.Trans.ReversalType == TransactionDO.Original
					|| this.Trans.ReversalType == TransactionDO.Reversal)
				{
					return false;
				}

				if (this.bTransIDBeingLoaded)
				{
					return false;
				}

				//Check for user's permission to edit transactions.
				if (!this.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA))
				{
					return false;
				}

				return true;
			}
		}
		#endregion

		#region Public Methods and Operators


		/// <summary>
		///	Override to distinguish how the page is being used
		/// </summary>
		/// <returns>Key for lookup into tblHelpMapping</returns>
		public override string GetHelpContextKey()
		{
			if (this.Trans != null)
			{
				return base.GetHelpContextKey() + "|" + this.Trans.TransTypeID;
			}

			return base.GetHelpContextKey();
		}
		public override List<string> GetHelpContextKeys()
		{
			if (this.Trans != null)
			{
				return new List<string>() { base.GetHelpContextKey() + "|" + this.Trans.TransTypeID.ToString() };
			}
			else
			{
				return new List<string> { base.GetHelpContextKey() };
			}
		}
		public virtual void QuantityReceivedFGAddButtonClick(object sender)
		{
			this.TransferToNewTx(this.Trans.LineItems[0], sender, 0);
		}

		public virtual void QuantityReceivedFGViewButtonClick(object sender)
		{
			this.TransferToViewing(this.Trans.LineItems[0]);
		}

		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public void CalculateQty(LineItemDO lineItem)
		{
			if (lineItem == null)
			{
				return;
			}

			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(this.security, lineItem.Product));
			if (product == null)
			{
				return;
			}

			byte volumeDecimalPlaces = (this.TransContext.aliasClass.VolumeUnits != 0) ? this.TransContext.aliasClass._VolumeDecimalPlaces : this.AccountingSite.CurrentSite._VolumeDecimalPlaces;
			byte additiveDecimalPlaces = (this.TransContext.aliasClass.AdditiveVolumeUnits != 0) ? this.TransContext.aliasClass._AdditiveVolumeDecimalPlaces : this.AccountingSite.CurrentSite._AdditiveVolumeDecimalPlaces;
			byte massDecimalPlaces = (this.TransContext.aliasClass.MassUnits != 0) ? this.TransContext.aliasClass._MassDecimalPlaces : this.AccountingSite.CurrentSite._MassDecimalPlaces;

			if (product.LoadByWeight)
			{
				if (lineItem.Quantity.MassManualValueFlag == true
					&& lineItem.Quantity.IsMassDirty
					&& lineItem.Quantity.IsPackageDirty
					&& lineItem.Quantity.PackageManualValueFlag == true)
				{
					throw new Exception("Package quantity and Mass quantity cannot be entered at same time.");
				}
			}
			else
			{
				if (lineItem.Quantity.NetManualValueFlag == true
					&& lineItem.Quantity.IsNetDirty
					&& lineItem.Quantity.IsPackageDirty
					&& lineItem.Quantity.PackageManualValueFlag == true)
				{
					throw new Exception("Package quantity and Net quantity cannot be entered at same time.");
				}
			}

			double vcf = 1.0;
			int dirty = 0;

			if (lineItem.Quantity.IsGrossDirty)
			{
				if (lineItem.Quantity.NullableGross == null)
				{
					lineItem.Quantity.GrossManualValueFlag = false;
				}
				else
				{
					lineItem.Quantity.GrossManualValueFlag = true;
				}
				dirty++;
			}

			if (lineItem.Quantity.IsNetDirty)
			{
				if (lineItem.Quantity.NullableNet == null)
				{
					lineItem.Quantity.NetManualValueFlag = false;
				}
				else
				{
					lineItem.Quantity.NetManualValueFlag = true;
				}
				dirty++;
			}

			if (lineItem.Quantity.IsMassDirty)
			{
				if (lineItem.Quantity.NullableMass == null)
				{
					lineItem.Quantity.MassManualValueFlag = false;
				}
				else
				{
					lineItem.Quantity.MassManualValueFlag = true;
				}
				dirty++;
			}

			if (lineItem.Quantity.IsVcfDirty)
			{
				if (lineItem.VCF == null)
				{
					lineItem.Quantity.VcfManualValueFlag = false;
				}
				else
				{
					lineItem.Quantity.VcfManualValueFlag = true;
				}
			}

			if (lineItem.Quantity.IsPackageDirty)
			{
				if (lineItem.Quantity.NullablePackage == null)
				{
					lineItem.Quantity.PackageManualValueFlag = null;
				}

				else
				{
					lineItem.Quantity.PackageManualValueFlag = true;
				}
				dirty++;
			}

			double dSiDensity;
			if (lineItem.Density == null || lineItem.Density.Value == 0.0)
			{
				dSiDensity = product._StandardDensity.SIValue;
				lineItem.Density = this.ConvertUnits(dSiDensity, EngineeringUnit.FmdKgM3, lineItem.DensityUnits);
			}
			else
			{
				dSiDensity = this.ConvertUnits(lineItem.Density.Value, lineItem.DensityUnits, EngineeringUnit.FmdKgM3);
			}

			try
			{
				if (lineItem.Quantity.VcfManualValueFlag == false || lineItem.Quantity.VcfManualValueFlag == null)
				{
					if (lineItem.Temperature != null
					&& lineItem.Density != null
					&& product != null
					&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE
					&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE_1980)
					{
						double standardDensity = this.ConvertUnits(
							lineItem.Density.Value,
							lineItem.DensityUnits,
							this.TransContext.accountingSite.CurrentSite.DensityUnits);

						Vcf volumeCorrection = new Vcf();

						volumeCorrection.VcfSettings = product._VcfModuleSettings.GetCommonComponentVcfModuleSettings(product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits);

						vcf = volumeCorrection.VcfCalculation((ECorrectionTypeMajor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
																(ECorrectionTypeMinor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
																(lineItem.Temperature.HasValue) ? lineItem.Temperature.Value : 0.0,
																lineItem.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : lineItem.TemperatureUnits,
																product._VcfModuleSettings.BaseTemperature.Value,
																product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
																standardDensity,
																product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.DensityUnits : product.DensityUnits,
																(lineItem.Pressure.HasValue) ? lineItem.Pressure.Value : 0.0,
																product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits,
																0.0,
																product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
																0.0,
																product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits,
																new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
					}
					else
					{
						vcf = 1.0;
					}

					// ReSharper disable once CompareOfFloatsByEqualityOperator
					if (lineItem.VCF == null || lineItem.VCF.Value != vcf)
					{
						lineItem.VCF = vcf;
						lineItem.Quantity.IsVcfDirty = true;
					}
				}
			}
			catch (Exception e)
			{
				throw new Exception("CalculateVCF Error : " + e.Message);
			}

			bool grossChanged = false;
			bool netChanged = false;
			bool massChanged = false;
			bool packageChanged = false;

			if (lineItem.VCF != null)
			{
				vcf = lineItem.VCF.Value;
			}

			int iManual = 0;
			if (lineItem.Quantity.GrossManualValueFlag == true)
			{
				iManual++;
			}

			if (lineItem.Quantity.NetManualValueFlag == true)
			{
				iManual++;
			}

			if (lineItem.Quantity.MassManualValueFlag == true)
			{
				iManual++;
			}

			if ((iManual >= 2 && dirty >= 2)
				|| (iManual == 0
					&& (lineItem.Quantity.PackageManualValueFlag == false || lineItem.Quantity.PackageManualValueFlag == null)
					&& dirty != 0)) // no calculation or clear					
			{
				if (!lineItem.Quantity.IsGrossDirty) lineItem.Quantity.NullableGross = null;
				if (!lineItem.Quantity.IsNetDirty) lineItem.Quantity.NullableNet = null;
				if (!lineItem.Quantity.IsMassDirty) lineItem.Quantity.NullableMass = null;
				if (!lineItem.Quantity.IsPackageDirty) lineItem.Quantity.NullablePackage = null;
			}
			else if (iManual >= 1 && dirty >= 2
						&& (lineItem.Quantity.IsPackageDirty && lineItem.Quantity.PackageManualValueFlag == true))
			{
				if (!lineItem.Quantity.IsGrossDirty) lineItem.Quantity.NullableGross = null;
				if (!lineItem.Quantity.IsNetDirty) lineItem.Quantity.NullableNet = null;
				if (!lineItem.Quantity.IsMassDirty) lineItem.Quantity.NullableMass = null;

				// Calculate Net from Package
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
				{
					lineItem.Quantity.NetInventoryChange = lineItem.Quantity.PackageInventoryChange * product._VolumePackageSize.Value;
					netChanged = true;
					lineItem.Quantity.NetManualValueFlag = false;
				}

				// Calculate Mass from Package
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
				{
					lineItem.Quantity.MassInventoryChange = lineItem.Quantity.PackageInventoryChange * product._MassPackageSize.Value;
					massChanged = true;
					lineItem.Quantity.MassManualValueFlag = false;
				}
			}
			else
			{
				double dSiMassQuantity;
				double dSiVolume;

				if (lineItem.Quantity.IsGrossDirty && lineItem.Quantity.GrossManualValueFlag == true)
				{
					// Calculate Net from Gross
					lineItem.Quantity.NetInventoryChange = lineItem.Quantity.GrossInventoryChange * vcf;
					netChanged = true;
					lineItem.Quantity.NetManualValueFlag = false;

					// Calculate Mass from Net
					dSiVolume = this.ConvertUnits(lineItem.Quantity.NetInventoryChange,
													lineItem.VolumeUnits,
													EngineeringUnit.FmvMeter3);

					// round volume to nearest 0.001
					dSiVolume = Math.Round(dSiVolume, 3, MidpointRounding.AwayFromZero);

					dSiMassQuantity = dSiVolume * dSiDensity;
					lineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																				EngineeringUnit.FmmKg,
																				lineItem.MassUnits);
					massChanged = true;
					lineItem.Quantity.MassManualValueFlag = false;

					if (product._MassPackageSize.Value != 0.0 && product.LoadByWeight)
					{
						// Calculate Package from Mass
						lineItem.Quantity.PackageInventoryChange = lineItem.Quantity.MassInventoryChange
																				/ product._MassPackageSize.Value;
						packageChanged = true;
						lineItem.Quantity.PackageManualValueFlag = false;
					}
					else if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
					{
						// Calculate Package from Net
						lineItem.Quantity.PackageInventoryChange = lineItem.Quantity.NetInventoryChange
																				/ product._VolumePackageSize.Value;
						packageChanged = true;
						lineItem.Quantity.PackageManualValueFlag = false;
					}

					// enforce correct rounding of net quantity
					lineItem.Quantity.NetInventoryChange = lineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct) ?
																			Math.Round(lineItem.Quantity.NetInventoryChange, additiveDecimalPlaces) :
																			Math.Round(lineItem.Quantity.NetInventoryChange, volumeDecimalPlaces);
				}

				else if (lineItem.Quantity.IsNetDirty && lineItem.Quantity.NetManualValueFlag == true)
				{
					// Calculate Gross from Net
					lineItem.Quantity.GrossInventoryChange = lineItem.Quantity.NetInventoryChange / vcf;
					grossChanged = true;
					lineItem.Quantity.GrossManualValueFlag = false;

					// Calculate Mass from Net
					dSiVolume = this.ConvertUnits(lineItem.Quantity.NetInventoryChange,
													lineItem.VolumeUnits,
													EngineeringUnit.FmvMeter3);

					// round volume to nearest 0.001
					dSiVolume = Math.Round(dSiVolume, 3, MidpointRounding.AwayFromZero);

					dSiMassQuantity = dSiVolume * dSiDensity;
					lineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																				EngineeringUnit.FmmKg,
																				lineItem.MassUnits);
					massChanged = true;
					lineItem.Quantity.MassManualValueFlag = false;

					// Calculate Package
					// ReSharper disable once CompareOfFloatsByEqualityOperator
					if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
					{
						lineItem.Quantity.PackageInventoryChange = lineItem.Quantity.MassInventoryChange
																				/ product._MassPackageSize.Value;
						packageChanged = true;
						lineItem.Quantity.MassManualValueFlag = false;
					}

					// Calculate Package from Net
					else if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
					{
						lineItem.Quantity.PackageInventoryChange = lineItem.Quantity.NetInventoryChange
																				/ product._VolumePackageSize.Value;
						packageChanged = true;
						lineItem.Quantity.MassManualValueFlag = false;
					}


					// enforce correct rounding of gross quantity
					lineItem.Quantity.GrossInventoryChange = lineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct) ?
																			Math.Round(lineItem.Quantity.GrossInventoryChange, additiveDecimalPlaces) :
																			Math.Round(lineItem.Quantity.GrossInventoryChange, volumeDecimalPlaces);
				}
				else if (lineItem.Quantity.IsMassDirty && lineItem.Quantity.MassManualValueFlag == true)
				{
					// Calculate Net from Mass
					dSiMassQuantity = this.ConvertUnits(lineItem.Quantity.MassInventoryChange,
														lineItem.MassUnits,
														EngineeringUnit.FmmKg);
					dSiVolume = dSiMassQuantity / dSiDensity;

					lineItem.Quantity.NetInventoryChange = this.ConvertUnits(dSiVolume,
																			EngineeringUnit.FmvMeter3,
																			lineItem.VolumeUnits);
					netChanged = true;
					lineItem.Quantity.NetManualValueFlag = false;

					// Calculate Gross from Net
					lineItem.Quantity.GrossInventoryChange = lineItem.Quantity.NetInventoryChange / vcf;
					grossChanged = true;
					lineItem.Quantity.GrossManualValueFlag = false;

					// Calculate Package
					if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
					{
						lineItem.Quantity.PackageInventoryChange = lineItem.Quantity.MassInventoryChange
																				/ product._MassPackageSize.Value;
						packageChanged = true;
						lineItem.Quantity.PackageManualValueFlag = false;
					}
					// Calculate Package from Net
					else if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
					{
						lineItem.Quantity.PackageInventoryChange = lineItem.Quantity.NetInventoryChange
																				/ product._VolumePackageSize.Value;
						packageChanged = true;
						lineItem.Quantity.PackageManualValueFlag = false;
					}

					// enforce correct rounding of net quantity
					lineItem.Quantity.NetInventoryChange = lineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct) ?
																			Math.Round(lineItem.Quantity.NetInventoryChange, additiveDecimalPlaces) :
																			Math.Round(lineItem.Quantity.NetInventoryChange, volumeDecimalPlaces);
					// enforce correct rounding of gross quantity
					lineItem.Quantity.GrossInventoryChange = lineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct) ?
																			Math.Round(lineItem.Quantity.GrossInventoryChange, additiveDecimalPlaces) :
																			Math.Round(lineItem.Quantity.GrossInventoryChange, volumeDecimalPlaces);
				}

				else if (lineItem.Quantity.IsPackageDirty && lineItem.Quantity.PackageManualValueFlag == true)
				{
					// Calculate Net from Package
					if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
					{
						lineItem.Quantity.NetInventoryChange = lineItem.Quantity.PackageInventoryChange
																			* product._VolumePackageSize.Value;
						netChanged = true;
						lineItem.Quantity.NetManualValueFlag = false;
					}

					// Calculate Mass from Package
					if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
					{
						lineItem.Quantity.MassInventoryChange = lineItem.Quantity.PackageInventoryChange * product._MassPackageSize.Value;
						massChanged = true;
						lineItem.Quantity.MassManualValueFlag = false;
					}

					// Calculate Net from Mass
					if (massChanged && !netChanged)
					{
						dSiMassQuantity = this.ConvertUnits(lineItem.Quantity.MassInventoryChange,
															lineItem.MassUnits,
															EngineeringUnit.FmmKg);
						dSiVolume = dSiMassQuantity / dSiDensity;

						lineItem.Quantity.NetInventoryChange = this.ConvertUnits(dSiVolume,
																				EngineeringUnit.FmmKg,
																				lineItem.MassUnits);
						netChanged = true;
						lineItem.Quantity.NetManualValueFlag = false;
					}

					// Calculate Mass from Net
					if (netChanged && !massChanged)
					{
						dSiVolume = this.ConvertUnits(lineItem.Quantity.NetInventoryChange,
													lineItem.VolumeUnits,
													EngineeringUnit.FmvMeter3);
						dSiMassQuantity = dSiVolume * dSiDensity;

						lineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																					EngineeringUnit.FmmKg,
																					lineItem.MassUnits);
						massChanged = true;
						lineItem.Quantity.MassManualValueFlag = false;
					}

					// Calculate Gross from Net
					if (netChanged)
					{
						lineItem.Quantity.GrossInventoryChange = lineItem.Quantity.NetInventoryChange / vcf;
						lineItem.Quantity.GrossManualValueFlag = false;
					}
				}
				else if (lineItem.Quantity.IsVcfDirty)
				{
					if (lineItem.Quantity.GrossManualValueFlag == true)
					{
						lineItem.Quantity.NetInventoryChange = lineItem.Quantity.GrossInventoryChange * vcf;
						netChanged = true;
					}
					else
					{
						lineItem.Quantity.GrossInventoryChange = lineItem.Quantity.NetInventoryChange / vcf;
					}
				}
			}

			// If Order Entry, update the remaining value
			if (this.Trans.TransTypeID == TransactionTypes.T17_Order)
			{
				lineItem.NetQuantityRemaining = lineItem.Quantity.NetInventoryChange - lineItem.NetQuantityReceived;
				lineItem.MassQuantityRemaining = lineItem.Quantity.MassInventoryChange - lineItem.MassQuantityReceived;
			}

			// Update Supply order value remaining and total value
			if (null != lineItem.ProductPrice)
			{
				if (product.LoadByWeight)
				{
					lineItem.ValueRemaining = lineItem.MassQuantityRemaining * lineItem.ProductPrice.Value;
					lineItem.TotalValue = lineItem.Quantity.MassInventoryChange * lineItem.ProductPrice.Value;
				}
				else
				{
					lineItem.ValueRemaining = lineItem.NetQuantityRemaining * lineItem.ProductPrice.Value;
					lineItem.TotalValue = lineItem.Quantity.NetInventoryChange * lineItem.ProductPrice.Value;
				}
			}


			if (!this.TransContext.aliasClass.MultipleLineItems)
			{
				if (grossChanged)
				{
					TransactionAliasFieldClass grossField =
						this.TransContext.aliasClass.LineItemFieldCollection.Find("GrossQuantity");

					if (grossField != null)
					{
						var grossFG =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem GrossQuantity") as LineItemGrossQuantityFG;
						grossFG.SetNewValue();
					}
				}

				if (netChanged)
				{
					TransactionAliasFieldClass netField = this.TransContext.aliasClass.LineItemFieldCollection.Find("NetQuantity");

					if (netField != null)
					{
						var netFG =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem NetQuantity") as LineItemNetQuantityFG;
						netFG.SetNewValue();
					}

					TransactionAliasFieldClass netRemainingField =
						this.TransContext.aliasClass.LineItemFieldCollection.Find("NetQuantityRemaining");

					if (netRemainingField != null)
					{
						var netremainingFG =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem NetQuantityRemaining") as LineItemNetQuantityRemainingFG;

						netremainingFG?.SetNewValue();
					}
				}

				if (massChanged)
				{
					TransactionAliasFieldClass massField = this.TransContext.aliasClass.LineItemFieldCollection.Find(
						"MassQuantity");
					if (massField != null)
					{
						var massFG =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem MassQuantity") as LineItemMassQuantityFG;
						massFG.SetNewValue();
					}

					TransactionAliasFieldClass massRemainingField =
						this.TransContext.aliasClass.LineItemFieldCollection.Find("MassQuantityRemaining");

					if (massRemainingField != null)
					{
						var massremainingFG =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem MassQuantityRemaining") as LineItemMassQuantityRemainingFG;
						massremainingFG?.SetNewValue();
					}
				}

				if (packageChanged)
				{
					TransactionAliasFieldClass packageField =
						this.TransContext.aliasClass.LineItemFieldCollection.Find("PackageQuantity");

					if (packageField != null)
					{
						var packageFG =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem PackageQuantity") as LineItemPackageQuantityFG;

						packageFG?.SetNewValue();
					}
				}

				TransactionAliasFieldClass grossmanualField =
					this.TransContext.aliasClass.LineItemFieldCollection.Find("GrossManualValueFlag");

				if (grossmanualField != null)
				{
					var grossmanualFG =
						this.TransactionFieldGenerator.GetFieldGenerator("LineItem GrossManualValueFlag") as
						LineItemGrossManualValueFlagFG;
					grossmanualFG?.SetNewValue(lineItem.Quantity.GrossManualValueFlag);
				}

				TransactionAliasFieldClass netmanualField =
					this.TransContext.aliasClass.LineItemFieldCollection.Find("NetManualValueFlag");

				if (netmanualField != null)
				{
					var netmanualFG =
						this.TransactionFieldGenerator.GetFieldGenerator("LineItem NetManualValueFlag") as LineItemNetManualValueFlagFG;

					netmanualFG?.SetNewValue(lineItem.Quantity.NetManualValueFlag);
				}

				TransactionAliasFieldClass massmanualField =
					this.TransContext.aliasClass.LineItemFieldCollection.Find("MassManualValueFlag");

				if (massmanualField != null)
				{
					var massmanualFG =
						this.TransactionFieldGenerator.GetFieldGenerator("LineItem MassManualValueFlag") as LineItemMassManualValueFlagFG;

					massmanualFG?.SetNewValue(lineItem.Quantity.MassManualValueFlag);
				}

				TransactionAliasFieldClass vcfmanualField =
					this.TransContext.aliasClass.LineItemFieldCollection.Find("VcfManualValueFlag");

				if (vcfmanualField != null)
				{
					var vcfmanualFG =
						this.TransactionFieldGenerator.GetFieldGenerator("LineItem VcfManualValueFlag") as LineItemVcfManualValueFlagFG;

					vcfmanualFG?.SetNewValue(lineItem.Quantity.VcfManualValueFlag);
				}
			}
		}

		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public void CalculateQty(SubLineItemDO subLineItem)
		{
			if (subLineItem == null)
			{
				return;
			}

			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(this.security, subLineItem.Product));

			if (product == null)
			{
				return;
			}

			if (product.LoadByWeight)
			{
				if (subLineItem.Quantity.MassManualValueFlag == true
					&& subLineItem.Quantity.IsMassDirty
					&& subLineItem.Quantity.IsPackageDirty
					&& subLineItem.Quantity.PackageManualValueFlag == true)
				{
					throw new Exception("Package quantity and Mass quantity cannot be entered at same time.");
				}
			}
			else
			{
				if (subLineItem.Quantity.NetManualValueFlag == true
					&& subLineItem.Quantity.IsNetDirty
					&& subLineItem.Quantity.IsPackageDirty
					&& subLineItem.Quantity.PackageManualValueFlag == true)
				{
					throw new Exception("Package quantity and Net quantity cannot be entered at same time.");
				}
			}

			double dSiDensity;
			double vcf = 1.0;
			int dirty = 0;

			if (subLineItem.Quantity.IsGrossDirty)
			{
				if (subLineItem.Quantity.NullableGross == null) subLineItem.Quantity.GrossManualValueFlag = null;
				else subLineItem.Quantity.GrossManualValueFlag = true;
				dirty++;
			}

			if (subLineItem.Quantity.IsNetDirty)
			{
				if (subLineItem.Quantity.NullableNet == null) subLineItem.Quantity.NetManualValueFlag = null;
				else subLineItem.Quantity.NetManualValueFlag = true;
				dirty++;
			}

			if (subLineItem.Quantity.IsMassDirty)
			{
				if (subLineItem.Quantity.NullableMass == null) subLineItem.Quantity.MassManualValueFlag = null;
				else subLineItem.Quantity.MassManualValueFlag = true;
				dirty++;
			}

			if (subLineItem.Quantity.IsVcfDirty)
			{
				if (subLineItem.VCF == null) subLineItem.Quantity.VcfManualValueFlag = null;
				else subLineItem.Quantity.VcfManualValueFlag = true;
			}

			if (subLineItem.Quantity.IsPackageDirty)
			{
				if (subLineItem.Quantity.NullablePackage == null) subLineItem.Quantity.PackageManualValueFlag = null;
				else subLineItem.Quantity.PackageManualValueFlag = true;
				dirty++;
			}

			if (subLineItem.Density == null || subLineItem.Density.Value == 0.0)
			{
				dSiDensity = product._StandardDensity.SIValue;
				subLineItem.Density = this.ConvertUnits(dSiDensity,
														EngineeringUnit.FmdKgM3,
														subLineItem.DensityUnits);
			}
			else
			{
				dSiDensity = this.ConvertUnits(subLineItem.Density.Value,
												subLineItem.DensityUnits,
												EngineeringUnit.FmdKgM3);
			}

			// round std denisty to nearest 0.1
			dSiDensity = Math.Round(dSiDensity, 1, MidpointRounding.AwayFromZero);

			try
			{
				if (subLineItem.Quantity.VcfManualValueFlag == false || subLineItem.Quantity.VcfManualValueFlag == null)
				{
					if (subLineItem.Temperature != null
					&& subLineItem.Density != null
					&& product != null
					&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE
					&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE_1980)
					{
						double standardDensity = this.ConvertUnits(
							subLineItem.Density.Value,
							subLineItem.DensityUnits,
							this.TransContext.accountingSite.CurrentSite.DensityUnits);

						Vcf VolumeCorrection = new Vcf();

						VolumeCorrection.VcfSettings = product._VcfModuleSettings.GetCommonComponentVcfModuleSettings(product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits);

						vcf = VolumeCorrection.VcfCalculation((ECorrectionTypeMajor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
																(ECorrectionTypeMinor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
																(subLineItem.Temperature.HasValue) ? subLineItem.Temperature.Value : 0.0,
																subLineItem.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : subLineItem.TemperatureUnits,
																product._VcfModuleSettings.BaseTemperature.Value,
																product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
																standardDensity,
																product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.DensityUnits : product.DensityUnits,
																(subLineItem.Pressure.HasValue) ? subLineItem.Pressure.Value : 0.0,
																product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits,
																0.0,
																product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
																0.0,
																product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits,
																new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
					}
					else
					{
						vcf = 1.0;
					}

					if (subLineItem.VCF == null || subLineItem.VCF.Value != vcf)
					{
						subLineItem.VCF = vcf;
						subLineItem.Quantity.IsVcfDirty = true;
					}
				}
			}
			catch (Exception e)
			{
				throw new Exception("CalculateVCF Error : " + e.Message);
			}

			bool netChanged = false;
			bool massChanged = false;

			if (subLineItem.VCF != null)
			{
				vcf = subLineItem.VCF.Value;
			}

			int iManual = 0;
			if (subLineItem.Quantity.GrossManualValueFlag == true) iManual++;
			if (subLineItem.Quantity.NetManualValueFlag == true) iManual++;
			if (subLineItem.Quantity.MassManualValueFlag == true) iManual++;

			if ((iManual >= 2 && dirty >= 2)
				|| (iManual == 0
					&& (subLineItem.Quantity.PackageManualValueFlag == false
							|| subLineItem.Quantity.PackageManualValueFlag == null) && dirty != 0)) // no calculation or clear					
			{
				if (!subLineItem.Quantity.IsGrossDirty) subLineItem.Quantity.NullableGross = null;
				if (!subLineItem.Quantity.IsNetDirty) subLineItem.Quantity.NullableNet = null;
				if (!subLineItem.Quantity.IsMassDirty) subLineItem.Quantity.NullableMass = null;
				if (!subLineItem.Quantity.IsPackageDirty) subLineItem.Quantity.NullablePackage = null;
			}
			else if (iManual >= 1 && dirty >= 2
						&& (subLineItem.Quantity.IsPackageDirty && subLineItem.Quantity.PackageManualValueFlag == true))
			{
				if (!subLineItem.Quantity.IsGrossDirty) subLineItem.Quantity.NullableGross = null;
				if (!subLineItem.Quantity.IsNetDirty) subLineItem.Quantity.NullableNet = null;
				if (!subLineItem.Quantity.IsMassDirty) subLineItem.Quantity.NullableMass = null;

				// Calculate Net from Package
				if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
				{
					subLineItem.Quantity.NetInventoryChange = subLineItem.Quantity.PackageInventoryChange
																			* product._VolumePackageSize.Value;
					subLineItem.Quantity.NetManualValueFlag = false;
				}

				// Calculate Mass from Package
				if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
				{
					subLineItem.Quantity.MassInventoryChange = subLineItem.Quantity.PackageInventoryChange
																			* product._MassPackageSize.Value;
					subLineItem.Quantity.MassManualValueFlag = false;
				}
			}
			else
			{
				double dSiMassQuantity;
				double dSiVolume;

				if (subLineItem.Quantity.GrossManualValueFlag == true)
				{
					// Calculate Net from Gross
					subLineItem.Quantity.NetInventoryChange = subLineItem.Quantity.GrossInventoryChange * vcf;
					subLineItem.Quantity.NetManualValueFlag = false;

					// Calculate Mass from Net
					dSiVolume = this.ConvertUnits(subLineItem.Quantity.NetInventoryChange,
													subLineItem.VolumeUnits,
													EngineeringUnit.FmvMeter3);

					// round volume to nearest 0.001
					dSiVolume = Math.Round(dSiVolume, 3, MidpointRounding.AwayFromZero);

					dSiMassQuantity = dSiVolume * dSiDensity;
					subLineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																					EngineeringUnit.FmmKg,
																					subLineItem.MassUnits);
					subLineItem.Quantity.MassManualValueFlag = false;

					// Calculate Package from Mass
					if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
					{
						subLineItem.Quantity.PackageInventoryChange = subLineItem.Quantity.MassInventoryChange
																					/ product._MassPackageSize.Value;
						subLineItem.Quantity.PackageManualValueFlag = false;
					}
					// Calculate Package from Net
					else if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
					{
						subLineItem.Quantity.PackageInventoryChange = subLineItem.Quantity.NetInventoryChange
																					/ product._VolumePackageSize.Value;
						subLineItem.Quantity.PackageManualValueFlag = false;
					}
				}

				else if (subLineItem.Quantity.NetManualValueFlag == true)
				{
					// Calculate Gross from Net
					subLineItem.Quantity.GrossInventoryChange = subLineItem.Quantity.NetInventoryChange / vcf;
					subLineItem.Quantity.GrossManualValueFlag = false;

					// Calculate Mass from Net
					dSiVolume = this.ConvertUnits(subLineItem.Quantity.NetInventoryChange,
													subLineItem.VolumeUnits,
													EngineeringUnit.FmvMeter3);

					// round volume to nearest 0.001
					dSiVolume = Math.Round(dSiVolume, 3, MidpointRounding.AwayFromZero);

					dSiMassQuantity = dSiVolume * dSiDensity;
					subLineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																				EngineeringUnit.FmmKg,
																				subLineItem.MassUnits);
					subLineItem.Quantity.MassManualValueFlag = false;

					// Calculate Package
					if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
					{
						subLineItem.Quantity.PackageInventoryChange = subLineItem.Quantity.MassInventoryChange
																					/ product._MassPackageSize.Value;
						subLineItem.Quantity.MassManualValueFlag = false;
					}

					// Calculate Package from Net
					else if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
					{
						subLineItem.Quantity.PackageInventoryChange = subLineItem.Quantity.NetInventoryChange
																					/ product._VolumePackageSize.Value;
						subLineItem.Quantity.MassManualValueFlag = false;
					}
				}
				else if (subLineItem.Quantity.MassManualValueFlag == true)
				{
					// Calculate Net from Mass
					dSiMassQuantity = this.ConvertUnits(subLineItem.Quantity.MassInventoryChange,
														subLineItem.MassUnits,
														EngineeringUnit.FmmKg);
					dSiVolume = dSiMassQuantity / dSiDensity;
					subLineItem.Quantity.NetInventoryChange = this.ConvertUnits(dSiVolume,
																				EngineeringUnit.FmvMeter3,
																				subLineItem.VolumeUnits);
					subLineItem.Quantity.NetManualValueFlag = false;

					// Calculate Gross from Net
					subLineItem.Quantity.GrossInventoryChange = subLineItem.Quantity.NetInventoryChange / vcf;
					subLineItem.Quantity.GrossManualValueFlag = false;

					// Calculate Package
					if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
					{
						subLineItem.Quantity.PackageInventoryChange = subLineItem.Quantity.MassInventoryChange
																					/ product._MassPackageSize.Value;
						subLineItem.Quantity.PackageManualValueFlag = false;
					}
					// Calculate Package from Net
					else if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
					{
						subLineItem.Quantity.PackageInventoryChange = subLineItem.Quantity.NetInventoryChange
																					/ product._VolumePackageSize.Value;
						subLineItem.Quantity.PackageManualValueFlag = false;
					}
				}

				else if (subLineItem.Quantity.PackageManualValueFlag == true)
				{
					// Calculate Net from Package
					if (product._VolumePackageSize.Value != 0 && !product.LoadByWeight)
					{
						subLineItem.Quantity.NetInventoryChange = subLineItem.Quantity.PackageInventoryChange
																				* product._VolumePackageSize.Value;
						netChanged = true;
						subLineItem.Quantity.NetManualValueFlag = false;
					}

					// Calculate Mass from Package
					if (product._MassPackageSize.Value != 0 && product.LoadByWeight)
					{
						subLineItem.Quantity.MassInventoryChange = subLineItem.Quantity.PackageInventoryChange
																				* product._MassPackageSize.Value;
						massChanged = true;
						subLineItem.Quantity.MassManualValueFlag = false;
					}

					// Calculate Net from Mass
					if (massChanged && !netChanged)
					{
						dSiMassQuantity = this.ConvertUnits(subLineItem.Quantity.MassInventoryChange,
															subLineItem.MassUnits,
															EngineeringUnit.FmmKg);
						dSiVolume = dSiMassQuantity / dSiDensity;
						subLineItem.Quantity.NetInventoryChange = this.ConvertUnits(dSiVolume,
																					EngineeringUnit.FmmKg,
																					subLineItem.MassUnits);
						netChanged = true;
						subLineItem.Quantity.NetManualValueFlag = false;
					}

					// Calculate Mass from Net
					if (netChanged && !massChanged)
					{
						dSiVolume = this.ConvertUnits(subLineItem.Quantity.NetInventoryChange,
														subLineItem.VolumeUnits,
														EngineeringUnit.FmvMeter3);
						dSiMassQuantity = dSiVolume * dSiDensity;
						subLineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																					EngineeringUnit.FmmKg,
																					subLineItem.MassUnits);
						subLineItem.Quantity.MassManualValueFlag = false;
					}

					// Calculate Gross from Net
					if (netChanged)
					{
						subLineItem.Quantity.GrossInventoryChange = subLineItem.Quantity.NetInventoryChange / vcf;
						subLineItem.Quantity.GrossManualValueFlag = false;
					}
				}

				else if (subLineItem.Quantity.IsVcfDirty)
				{
					if (subLineItem.Quantity.NetManualValueFlag != true)
					{
						subLineItem.Quantity.NetInventoryChange = subLineItem.Quantity.GrossInventoryChange * vcf;
					}
					else if (subLineItem.Quantity.GrossManualValueFlag != true)
					{
						subLineItem.Quantity.GrossInventoryChange = subLineItem.Quantity.NetInventoryChange / vcf;
					}
				}
			}
		}


		public void CalculateDeliveredQty(LineItemDO lineItem)
		{
			lineItem.DeliveredGrossInventoryChange = lineItem.GrossInventoryChange;
			lineItem.DeliveredNetInventoryChange = lineItem.NetInventoryChange;
			foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
			{
				subLineItem.Quantity.DeliveredGrossInventoryChange = subLineItem.Quantity.GrossInventoryChange;
				subLineItem.Quantity.DeliveredNetInventoryChange = subLineItem.Quantity.NetInventoryChange;
			}

			if ((lineItem.LoadingLocationStationGuid.IsEmpty()) || (lineItem.ArmNumber == null))
			{
				return;
			}
			if ((lineItem.SubLineItems == null) || (lineItem.SubLineItems.Count == 0))
			{
				return;
			}
			var ethanolSubLineItems = lineItem.SubLineItems.Where(x => x.IsEthanol).ToList();
			if ((ethanolSubLineItems == null) || ethanolSubLineItems.Count == 0)
			{
				return;
			}

			StationClass station = FMChannelHelper.MakeCall<IStations, StationClass>(x => x.Get(this.security, lineItem.LoadingLocationStationGuid));
			if (station.EthanolExcess == false)
			{
				return;
			}

			LoadArmClass targetLoadArm = null;
			foreach (LoadArmClass loadArm in station.LoadArmCollection)
			{
				if (((loadArm.BayAStationGuid == station.IdentityGuid) && (loadArm.BayAArmNumber == lineItem.ArmNumber))
					|| ((loadArm.BayBStationGuid == station.IdentityGuid) && (loadArm.BayBArmNumber == lineItem.ArmNumber)))
				{
					targetLoadArm = loadArm;
					break;
				}
			}
			if (targetLoadArm == null)
			{
				return;
			}

			EthanolExpansionScenarioHelper ee = new EthanolExpansionScenarioHelper();
			if (ee.isEEScenario2(lineItem, this.security))
			{
                Dictionary<Guid, double> bobBlendPercentagesByProductGuid = new Dictionary<Guid, double>();
				bobBlendPercentagesByProductGuid = ee.getBobBlendPercentages(lineItem, this.security);
				lineItem.UpdateDeliveredQuantities(station.EthanolExcess, targetLoadArm, false, bobBlendPercentagesByProductGuid);
				return;
			}
			lineItem.UpdateDeliveredQuantities(station.EthanolExcess, targetLoadArm, false);
		}

		/// <summary>
		///	Checks that all necessary fields are properly entered.
		///	This validation should run through only when either currency unit or non-domestic price fields are
		///	configured.
		/// </summary>
		/// <returns></returns>
		public string ValidateCurrencyFields(DataGridItem item)
		{
			if (this.TransContext.Currencies == null)
			{
				return null;
			}

			//Eric Simmons
			//09-12-2008 Modified this section after talk with Ali and Van on the phone at 2:45 PM.
			//If the tranasction is defined as a multiple line item tranaction and the item is null
			if (this.TransContext.aliasClass.MultipleLineItems && item == null)
			{
				return null;
			}

			HtmlSelect currencyUnitSelect = null;
			TextBox nonDomesticPriceTextBox = null;

			//Eric Simmons
			if (this.TransContext.aliasClass.MultipleLineItems)
			{
				if (item != null)
				{
					currencyUnitSelect = item.FindControl("TransactionFields.LineItemCurrencyUnitFG") as HtmlSelect;
					nonDomesticPriceTextBox = item.FindControl("TransactionFields.LineItemNonDomesticPriceFG") as TextBox;
				}
			}
			else
			{
				if (this.FieldTable != null)
				{
					currencyUnitSelect = this.FieldTable.FindControl("TransactionFields.LineItemCurrencyUnitFG") as HtmlSelect;
					nonDomesticPriceTextBox = this.FieldTable.FindControl("TransactionFields.LineItemNonDomesticPriceFG") as TextBox;
				}
			}

			//
			// If neither currency unit or non-domestic price fields are configured, then just exit.
			//
			if (currencyUnitSelect == null && nonDomesticPriceTextBox == null)
			{
				return null;
			}
			if (currencyUnitSelect == null)
			{
				return MissingCurrentyUnitMsg;
			}

			if ((FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey())
				|| this.Trans.Alias.ToUpper().Equals("FUEL ORDER")))
			{
				if (nonDomesticPriceTextBox == null)
				{
					return MissingNonDomesticPriceMsg;
				}

				if (currencyUnitSelect.SelectedIndex > 0
					&& string.IsNullOrEmpty(nonDomesticPriceTextBox.Text))
				{
					return MissingValueInNonDomesticPriceMsg;
				}

				if (currencyUnitSelect.SelectedIndex == 0
					&& string.IsNullOrEmpty(nonDomesticPriceTextBox.Text))
				{
					return MissingSelectionInCurrentUnitMsg;
				}
			}

			return null;
		}
		#endregion

		#region Methods
		public static void CheckForAndDisplayWarningMessages(
																SaveTransactionsResultDO resultDO,
																Guid currentSiteGuid,
																AccountingWebFormView currentPage,
																ILogger logger)
		{
			if (resultDO.Results.Count > 0) // if there are items here these are warnings. Any errors would of caused an exception
			{
				bool bFound = false;
				string msg = DataDictionarySingleton.Get(currentSiteGuid, "Save transaction warnings");

				string alertMsg = msg;

				foreach (TransactionValidationResult result in resultDO.Results)
				{
					foreach (string error in result.WarningList)
					{
						msg += "\n\r" + error;
						alertMsg += "\n" + error;
						bFound = true;
					}
				}

				if (bFound)
				{
					string alertString = "<script type=\"text/javascript\">\r\n<!--\r\nalert(\""
												+ HttpUtility.JavaScriptStringEncode(alertMsg) + "\");\r\n-->\r\n</script>";

					ScriptManager.RegisterClientScriptBlock(
						currentPage.Page,
						currentPage.GetType(),
						"SaveTransactionWarnings",
						alertString,
						false);

					msg = "TransactionDetail.Save() : \n\r" + msg;
					logger?.Warn(msg);
				}
			}
		}

		protected virtual void AgrNewButtonClick(object sender, EventArgs e)
		{
			if (this.RetrieveDataFromPage() == false)
			{
				return;
			}

			this.Trans.WeightReadings.Add(new WeightReadingDO());
			this.GaugeReadingsDataGrid.SelectedIndex = this.GaugeReadingsDataGrid.Items.Count;
			this.GaugeReadingsDataGrid.EditItemIndex = this.GaugeReadingsDataGrid.Items.Count;
			this.AgrGridGenerator.Bind();
			this.EnableFieldTable(false, false);
			this.DisableButtonsForEditing();
			this.Session.Add(SessionGaugeReadingGridInAddMode, "TRUE");
		}

		/// <summary>
		///	Aggregates values quantity, excise amount, GST amount, markup amount,
		///	total value, and "total price with tax" and assigns them to either their
		///	respective fields in line item or control.
		///	Parameter strLineItemIndex is the index to the line item contained in transaction line items.
		///	Parameter setControls -	if set to true, it will set related control fields
		///	if set to false, it will set related line item fields.
		/// </summary>
		/// <param name="itemIndex"></param>
		/// <param name="setControls"></param>
		protected virtual void AggregateAssociatedTxValues(int itemIndex, bool setControls)
		{
			try
			{
				if (this.TransContext.aliasClass.AggregateAssociatedTransactions
					&& this.TransContext.aliasClass.AssociatedAliases.Count > 0)
				{
					LineItemDO lineItem = this.Trans.LineItems[itemIndex];

					lineItem.Tax1 = null;
					lineItem.Tax2 = null;
					lineItem.Tax3 = null;

					// Create and populate the request object
					List<AssociatedTxDO> associatedTransactions = lineItem.AssociatedTransactions;
					double qty = 0;
					double qtyReceived = 0;
					double excise = 0;
					double markup = 0;
					double gst = 0;
					double totalValue = 0;
					double totalPriceWithTax = 0;
					double valueRemaining = 0;
					double quantityRemaining = 0;
					bool assocTransIsDemand = false;

					this.GetAssociatedTransactionAggregates(lineItem);

					foreach (AssociatedTxDO assocTx in associatedTransactions)
					{
						if (assocTx.Associated == 1)
						{
							if (this.TransContext.aliasClass.TransTypeID == TransactionTypes.T18_SupplyOrder)
							{
								if (assocTx.TransTypeID == TransactionTypes.T9_Request)
								{
									qty += assocTx.GrossQuantity;
									assocTransIsDemand = true;
								}
								qtyReceived += assocTx.GrossQuantityReceived;
							}
							else
							{
								qty += assocTx.GrossQuantity;
							}

							excise += assocTx.Excise;
							gst += assocTx.GST;
							markup += assocTx.Markup;
							totalValue += assocTx.TotalValue;
							totalPriceWithTax += assocTx.TotalPriceWithTax;
						}
					}

					qty = this.AccountingSite.ConvertFromSi(Math.Abs(qty), AccountingSite.ConversionUnits.VOLUME);
					qtyReceived = this.AccountingSite.ConvertFromSi(Math.Abs(qtyReceived), AccountingSite.ConversionUnits.VOLUME);
					totalValue = totalPriceWithTax - gst;

					if (!setControls)
					{
						if (TransactionTypes.T18_SupplyOrder == this.Trans.TransTypeID)
						{
							if (lineItem.AssociatedTransactions.Count > 0 && assocTransIsDemand)
							{
								//Update gross quantity with Demand transaction quantities, if associated transactions are 
								//Demand type.
								lineItem.Quantity.Gross = qty;
							}

							quantityRemaining = lineItem.Quantity.Gross - qtyReceived;
							lineItem.GrossQuantityReceived = qtyReceived;
						}
						else if (TransactionTypes.T21_AccountPayableInvoice == this.Trans.TransTypeID
									|| TransactionTypes.T22_AccountReceivableInvoice == this.Trans.TransTypeID)
						{
							if (lineItem.AssociatedTransactions.Count > 0)
							{
								//Update gross quantity with associated transactions quantities. This should be applied
								//only to Payment and Recovery transactions.
								lineItem.Quantity.GrossInventoryChange = qty;
							}
							if (lineItem.AlternativeUnits != null && (lineItem.AlternativeUnits.Value) > 0)
							{
								if (lineItem.AlternativeGrossVolume == null)
								{
									lineItem.AlternativeGrossVolume = 0;
								}

								lineItem.AlternativeGrossVolume = this.ConvertUnits(
									lineItem.Quantity.GrossInventoryChange,
									lineItem.VolumeUnits,
									(EngineeringUnit)lineItem.AlternativeUnits.Value);
							}
						}
						if (lineItem.ProductPrice != null)
						{
							valueRemaining = quantityRemaining * lineItem.ProductPrice.Value;
						}

						lineItem.Tax1 = excise;
						lineItem.Tax2 = gst;
						lineItem.Tax3 = markup;
						lineItem.TotalValue = totalValue;
						lineItem.TotalPriceWithTax = totalPriceWithTax;
						lineItem.GrossQuantityRemaining = quantityRemaining;
						lineItem.ValueRemaining = valueRemaining;
					}
					else if (this.LineItemDataGrid.Items.Count > itemIndex)
					{
						var htmlSelect =
							this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemAlternativeUnitsFG") as HtmlSelect;

						TextBox textBox;
						if (TransactionTypes.T18_SupplyOrder == this.Trans.TransTypeID)
						{
							textBox =
								this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemGrossQuantityFG") as TextBox;
							if (textBox != null)
							{
								if (assocTransIsDemand)
								{
									textBox.Text = qty.ToString(CultureInfo.CurrentCulture);
								}
								try
								{
									qty = Convert.ToDouble(textBox.Text);
								}
								catch
								{
									qty = 0.0;
								}
							}

							textBox = this.FieldTable.FindControl("TransactionFields.LineItemAlternativeGrossVolumeFG") as TextBox;
							//If transaction is an aggregate type then alternate volume needs to be based on gross quantity
							if (textBox != null && assocTransIsDemand && htmlSelect != null && htmlSelect.SelectedIndex > 0)
							{
								textBox.Text =
									this.ConvertUnits(qty, lineItem.VolumeUnits, (EngineeringUnit)Convert.ToInt32(htmlSelect.Value)).ToString("N");
							}
							textBox =
								this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemGrossQuantityReceivedFG") as
								TextBox;
							if (textBox != null)
							{
								textBox.Text = qtyReceived.ToString("N");
							}
							quantityRemaining = qty - qtyReceived;
							textBox =
								this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemProductPriceFG") as TextBox;
							try
							{
								if (textBox != null)
								{
									valueRemaining = quantityRemaining * Convert.ToDouble(textBox.Text);
								}
								else
								{
									valueRemaining = 0.0;
								}
							}
							catch
							{
								valueRemaining = 0.0;
							}
						}
						else
						{
							if ((TransactionTypes.T21_AccountPayableInvoice == this.Trans.TransTypeID
								|| TransactionTypes.T22_AccountReceivableInvoice == this.Trans.TransTypeID))
							{
								//Update gross quantity with associated transactions quantities. This should be applied
								//only to Payment and Recovery transactions.
								textBox =
									this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemGrossQuantityFG") as TextBox;
								if (textBox != null)
								{
									textBox.Text = qty.ToString("N");
								}
								textBox =
									this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemAlternativeGrossVolumeFG") as
									TextBox;
								//If transaction is an aggregate type then alternate volume needs to be based on gross quantity
								if (textBox != null && htmlSelect != null && htmlSelect.SelectedIndex > 0)
								{
									textBox.Text =
										this.ConvertUnits(qty, lineItem.VolumeUnits, (EngineeringUnit)Convert.ToInt32(htmlSelect.Value))
											.ToString("N");
								}
							}
						}

						textBox =
							this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemTotalPriceWithTaxFG") as TextBox;
						if (textBox != null)
						{
							textBox.Text = totalPriceWithTax.ToString("N");
						}
						textBox = this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemTotalValueFG") as TextBox;
						if (textBox != null)
						{
							textBox.Text = totalValue.ToString("N");
						}
						textBox = this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemTax1FG") as TextBox;
						if (textBox != null)
						{
							textBox.Text = excise.ToString("N");
						}
						textBox = this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemTax2FG") as TextBox;
						if (textBox != null)
						{
							textBox.Text = gst.ToString("N");
						}
						textBox = this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemTax3FG") as TextBox;
						if (textBox != null)
						{
							textBox.Text = markup.ToString("N");
						}
						textBox =
							this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemValueRemainingFG") as TextBox;
						if (textBox != null)
						{
							textBox.Text = valueRemaining.ToString("N");
						}
						textBox =
							this.LineItemDataGrid.Items[itemIndex].FindControl("TransactionFields.LineItemGrossQuantityRemainingFG") as
							TextBox;
						if (textBox != null)
						{
							textBox.Text = quantityRemaining.ToString("N");
						}
					}
				}
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch
			{
			}
		}


		protected virtual TransactionFieldGenerator CreateNewFieldGenerator()
		{
			var transFieldGenerator = new TransactionFieldGenerator(
				this.TransContext,
				this.FieldTable,
				this.Trans,
				this.AccountingSite,
				this.Page)
			{ GlossaryFileName = this.GetGlossaryFileName() };


			return transFieldGenerator;
		}

		protected virtual string GetGlossaryFileName()
		{
			// Load help mappings if need be
			HelpMappingDictionary helpDictionary;

			if (this.Session["HelpMappingDictionary"] == null)
			{
				helpDictionary = FMChannelHelper.MakeCall<IHelpMappings, HelpMappingDictionary>(helpMappingsChannel => helpMappingsChannel.GetDictionary(this.security));
			}
			else
			{
				helpDictionary = (HelpMappingDictionary)this.Session["HelpMappingDictionary"];
			}

			// Get help page for this context
			FMMenuData menuData;
			if (this.Session[PageSessionKeyConstants.FM_MENU_DATA] == null)
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	x =>
																	x.GetUsingGuid(this.security, this.security.SiteGuid)
																);
				List<KeyValuePair<string, Exception>> exceptions;
				menuData = FMMenuEngine.LoadMenuData(this.security, site.SiteGroup, this.useDataDictionary, out exceptions);
				this.HandleMenuLoadErrors(exceptions);
			}
			else
			{
				menuData = this.Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;
			}

			// Get help page for this context
			string helpContextKey = helpDictionary.GetHelpPage(this.GetHelpContextKey());

			if (menuData != null)
			{
				helpContextKey = menuData.GetHelpUrl(this.useDataDictionary) + "/" + helpContextKey.Replace(".htm", "_Fields_Description.htm");
			}
			else
			{
				helpContextKey = string.Empty;
			}

			return helpContextKey;
		}

		/// <summary>
		/// Use the Page's ErrorHandler method to display errors that occurred while loading
		/// menu data
		/// </summary>
		/// <param name="exceptions">Collection of information about exceptions</param>
		protected void HandleMenuLoadErrors(List<KeyValuePair<string, Exception>> exceptions)
		{
			// There are several situations in which non-fatal exceptions can be recorded
			if (exceptions != null)
			{
				foreach (KeyValuePair<string, Exception> exInfo in exceptions)
				{
					var value = exInfo.Value as ReflectionTypeLoadException;
					if (value != null)
					{
						this.ErrorHandler(exInfo.Key, this.BuildLoadExceptionMessage(value));
					}
					else if (exInfo.Key == string.Empty)
					{
						this.ErrorHandler(exInfo.Value);
					}
					else
					{
						this.ErrorHandler(exInfo.Key, exInfo.Value);
					}
				}
			}
		}

		/// <summary>
		/// Format the output for a ReflectionTypeLoadException
		/// </summary>
		/// <param name="reflectionException">The ReflectionTypeLoadException</param>
		/// <returns>Formatted error information</returns>
		private string BuildLoadExceptionMessage(ReflectionTypeLoadException reflectionException)
		{
			if (reflectionException == null)
			{
				throw new ArgumentNullException();
			}

			string message = reflectionException.Message;

			foreach (Exception except in reflectionException.LoaderExceptions)
			{
				message += "\n" + except.Message;
			}

			return message;

		}

		protected virtual void BindControls()
		{
			this.TransIDLabel.Text = this.Trans.TransID;

			// the transactionFieldGenerator is used to populated the passed table control
			// with transaction controls
			this.TransactionFieldGenerator = this.CreateNewFieldGenerator();
			this.TransactionFieldGenerator.GlossaryFileName = this.GetGlossaryFileName();

			if (this.TransContext.aliasClass.AggregateAssociatedTransactions
				&& this.TransContext.aliasClass.AssociatedAliases.Count > 0)
			{
				//Override excise, gst, markup, total value and total price with tax values
				//with their respective aggregated values.
				for (int i = 0; i < this.Trans.LineItems.Count; i++)
				{
					this.AggregateAssociatedTxValues(i, false);
				}
			}

			// See if this transaction is related to an order.  If so, add a line item based on
			// the details of the order.
			if (!string.IsNullOrEmpty(this.OrderReferenceID))
			{
				if (this.Trans.LineItems.Count == 0)
				{
					this.Trans.LineItems.Add(new LineItemDO());

					this.Trans.LineItems[0].Product = this.OrderProduct;
					this.Trans.LineItems[0].ProductCode = this.OrderProductCode;
					this.Trans.LineItems[0].ProductGuid = this.OrderProductGuid;

					if (this.Trans.ShipToCompanyGuid != Guid.Empty)
					{
						CompanyClass shipTo =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.security, this.Trans.ShipToCompanyGuid));
						string specialInstructions;
						Guid prodMapGuid;
						PRODUCT_MAP_TYPE mapType;

						this.GetSpecialInstructions(shipTo, this.OrderProductGuid, out specialInstructions, out prodMapGuid, out mapType);
						this.Trans.LineItems[0].SpecialInstructionsNote = specialInstructions;
						this.Trans.LineItems[0].SpecialInstructionsNoteGuid = prodMapGuid;
						this.Trans.LineItems[0].SpecialInstructionsNoteProductMapType = mapType;
					}
				}
			}

			// If the alias does not allow multiple line items hide the line item
			// datagrid and the new line item button
			if (this.TransContext.aliasClass.MultipleLineItems == false)
			{
				this.LineItemDataGrid.Visible = false;
				this.NewLineItemButton.Visible = false;

				if (this.Trans.LineItems.Count == 0 && this.TransContext.aliasClass.LineItemFieldCollection.Count > 0)
				{
					var lineItem = new LineItemDO { Quantity = { NullableGross = null, NullableNet = null } };
					this.Trans.LineItems.Add(lineItem);
				}
			}

			if (this.TransContext.aliasClass.MultipleWeightReadings == false)
			{
				this.GaugeReadingsDataGrid.Visible = false;
				this.NewAGRButton.Visible = false;

				if (this.Trans.WeightReadings.Count == 0 && this.TransContext.aliasClass.WeightReadingFieldCollection.Count > 0)
				{
					this.Trans.WeightReadings.Add(new WeightReadingDO());
				}
			}

			if (this.TransContext.aliasClass.MultipleTransportLineItems == false)
			{
				this.TransportDataGrid.Visible = false;
				this.NewTransportButton.Visible = false;

				if (this.Trans.TransportInfoList.Count == 0
					&& this.TransContext.aliasClass.TransportLineItemFieldCollection.Count > 0)
				{
					this.Trans.TransportInfoList.Add(new TransportLineItemDO());
				}
			}

			this.TransactionFieldGenerator.BindControls();

			if (this.TransContext.aliasClass.MultipleWeightReadings)
			{
				this.AgrGridGenerator = new AGRGridGenerator(
															this.GaugeReadingsDataGrid,
															this.TransContext,
															this.Trans,
															this.TransactionFieldGenerator);

				this.AgrGridGenerator.Generate(false);
				this.GaugeReadingsDataGrid.EditCommand += this.GaugeReadingsDataGridEditCommand;
				this.GaugeReadingsDataGrid.UpdateCommand += this.GaugeReadingsDataGridUpdateCommand;
				this.GaugeReadingsDataGrid.CancelCommand += this.GaugeReadingsDataGridCancelCommand;
				this.GaugeReadingsDataGrid.DeleteCommand += this.GaugeReadingsDataGridDeleteCommand;
				this.GaugeReadingsDataGrid.SelectedIndexChanged += this.GaugeReadingsDataGridSelectedIndexChanged;

				// Do not display the Weight Reading Grid if the only two columns are
				// Edit and Delete.
				if (this.GaugeReadingsDataGrid.Columns.Count <= 2)
				{
					this.GaugeReadingsDataGrid.Visible = false;
					this.NewAGRButton.Visible = false;
				}
			}

			if (this.TransContext.aliasClass.MultipleTransportLineItems)
			{
				this.TransportInfoGridGenerator = new TransportInfoGridGenerator(
																				this.TransportDataGrid,
																				this.TransContext,
																				this.Trans,
																				this.TransactionFieldGenerator);

				this.TransportInfoGridGenerator.Generate(false);
				this.TransportDataGrid.EditCommand += this.TransportDataGridEditCommand;
				this.TransportDataGrid.UpdateCommand += this.TransportDataGridUpdateCommand;
				this.TransportDataGrid.CancelCommand += this.TransportDataGridCancelCommand;
				this.TransportDataGrid.DeleteCommand += this.TransportDataGridDeleteCommand;

				// Do not display the Transport Grid if the only two columns are
				// Edit and Delete.
				if (this.TransportDataGrid.Columns.Count <= 2)
				{
					this.TransportDataGrid.Visible = false;
					this.NewTransportButton.Visible = false;
				}
			}

			if (this.TransContext.aliasClass.MultipleLineItems)
			{
				this.LineItemGridGenerator = new LineItemGridGenerator(
																		this.LineItemDataGrid,
																		this.TransContext,
																		this.Trans,
																		this.TransactionFieldGenerator,
																		this.security);
				this.LineItemGridGenerator.Generate(false);

				this.LineItemDataGrid.ItemCommand += this.LineItemDataGridItemCommand;
				this.LineItemDataGrid.EditCommand += this.LineItemDataGridEditCommand;
				this.LineItemDataGrid.UpdateCommand += this.LineItemDataGridUpdateCommand;
				this.LineItemDataGrid.CancelCommand += this.LineItemDataGridCancelCommand;
				this.LineItemDataGrid.DeleteCommand += this.LineItemDataGridDeleteCommand;
				this.LineItemDataGrid.SelectedIndexChanged += this.LineItemDataGridSelectedIndexChanged;
			}
		}


		/// <summary>
		///	This method uses the standard density, temperature or VCF to calculate volumes.
		/// </summary>
		/// <param name="item"></param>
		[SecurityCritical]
		protected virtual void CalculateVolumes(LineItemDO lineItem)
		{
			var brokenBlendFG = this.TransactionFieldGenerator.GetFieldGenerator("LineItem BrokenBlend") as BrokenBlendFG;

			if (brokenBlendFG != null && brokenBlendFG.DataChanged)
			{
				return;
			}

			var improperAdditizationFG = this.TransactionFieldGenerator.GetFieldGenerator("LineItem ImproperAdditization") as ImproperAdditizationFG;

			if (improperAdditizationFG != null && improperAdditizationFG.DataChanged)
			{
				return;
			}


			if (lineItem.ProductGuid != Guid.Empty)
			{
				ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByProductAuthorizedCompanies(this.security, lineItem.ProductGuid, false));

				if (lineItem.Temperature == null || lineItem.Temperature.Value == 0.0)
				{
					lineItem.Temperature = product._VcfModuleSettings.BaseTemperature.Value;
				}

				this.CalculateQty(lineItem);

				lineItem.BrokenBlend = false;
				lineItem.ImproperAdditization = false;



				AdditiveProfileClass additiveProfile = null;
				if (lineItem.AdditiveProfileGuid != Guid.Empty)
				{
					additiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(x => x.Get(this.security, lineItem.AdditiveProfileGuid));
				}

				var multipleBlendedExternalComponents = new ArrayList();
				var conventionalComponents = new ArrayList();
				double totalComponentBlendPercentage = 0;

				// Determine components with multiple lines
				// These will indicated external components that
				// were blended with multiple components of the blend
				int itemIndex = 0;
				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
					{
						ProductMapClass component = product.ComponentCollection.Find(x => x.AssignedGuid == subLineItem.ProductGuid);

						if (component != null)
						{
							if ((itemIndex < lineItem.SubLineItems.Count - 1
									&& subLineItem.ProductGuid == lineItem.SubLineItems[itemIndex + 1].ProductGuid)
									|| (itemIndex > 0
										&& subLineItem.ProductGuid == lineItem.SubLineItems[itemIndex - 1].ProductGuid))
							{
								multipleBlendedExternalComponents.Add(subLineItem);
							}
							else
							{
								totalComponentBlendPercentage += component.BlendPercentage;
								conventionalComponents.Add(subLineItem);
							}
						}
					}

					itemIndex++;
				}

				itemIndex = 0;
				foreach (SubLineItemDO subLineItem in multipleBlendedExternalComponents)
				{
					ProductMapClass externalComponentMap = product.ComponentCollection.Find(x => x.AssignedGuid == subLineItem.ProductGuid);

					var componentSubLineItem = conventionalComponents[itemIndex] as SubLineItemDO;

					ProductMapClass componentMap = product.ComponentCollection.Find(x => componentSubLineItem != null && x.AssignedGuid == componentSubLineItem.ProductGuid);

					// Component Temmperature is Line Item Temperature unless set by load rack or user
					if (subLineItem.Temperature == null)
					{
						subLineItem.Temperature = lineItem.Temperature;
					}

					// Component Standard Density is product Density by load rack or user
					double dSiDensity = 0.0;
					if (subLineItem.Density == null)
					{
						ProductClass component = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(this.security, subLineItem.Product));
						if (component != null)
						{
							dSiDensity = component._StandardDensity.SIValue;
							subLineItem.Density = this.ConvertUnits(dSiDensity, EngineeringUnit.FmdKgM3, subLineItem.DensityUnits);
						}
					}
					else
					{
						dSiDensity = this.ConvertUnits(subLineItem.Density.Value, subLineItem.DensityUnits, EngineeringUnit.FmdKgM3);
					}

					this.CalculateVCF(subLineItem);

					subLineItem.BrokenBlend = false;

					double externalComponentBlendPercentage = externalComponentMap.BlendPercentage * componentMap.BlendPercentage
																			/ (100 * totalComponentBlendPercentage);

					if (lineItem.PresetAmount != null)
					{
						subLineItem.PresetAmount =
							Math.Round(
									externalComponentBlendPercentage
									* this.ConvertUnits(lineItem.PresetAmount.Value, lineItem.VolumeUnits, subLineItem.VolumeUnits),
									subLineItem.VolumeDecimalPlaces,
									MidpointRounding.AwayFromZero);
					}

					if (subLineItem.Quantity.GrossManualValueFlag != true)
					{
						subLineItem.Quantity.GrossInventoryChange =
							Math.Round(
								externalComponentBlendPercentage
								* this.ConvertUnits(lineItem.Quantity.GrossInventoryChange, lineItem.VolumeUnits, subLineItem.VolumeUnits),
								subLineItem.VolumeDecimalPlaces,
								MidpointRounding.AwayFromZero);
					}

					if (subLineItem.Quantity.NetManualValueFlag != true)
					{
						subLineItem.Quantity.NetInventoryChange = subLineItem.Quantity.GrossInventoryChange * subLineItem.VCF.Value;
					}

					if (subLineItem.Quantity.MassManualValueFlag != true)
					{
						double dSiVolume = this.ConvertUnits(subLineItem.Quantity.NetInventoryChange,
														subLineItem.VolumeUnits,
														EngineeringUnit.FmvMeter3);

						// round volume to nearest 0.001
						dSiVolume = Math.Round(dSiVolume, 3, MidpointRounding.AwayFromZero);

						double dSiMassQuantity = dSiVolume * dSiDensity;
						subLineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																					EngineeringUnit.FmmKg,
																					lineItem.MassUnits);
					}

					itemIndex++;

					if (itemIndex == conventionalComponents.Count)
					{
						itemIndex = 0;
					}
				}

				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
					{
						ProductMapClass componentMap = product.ComponentCollection.Find(x => x.AssignedGuid == subLineItem.ProductGuid);

						if (componentMap != null)
						{
							if (multipleBlendedExternalComponents.Contains(subLineItem))
							{
								continue;
							}

							// Component Temmperature is Line Item Temperature unless set by load rack or user
							if (subLineItem.Temperature == null)
							{
								subLineItem.Temperature = lineItem.Temperature;
							}

							// Component Standard Density is product Density by load rack or user
							double dSiDensity = 0.0;
							if (subLineItem.Density == null)
							{
								ProductClass component = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(this.security, subLineItem.Product));
								if (component != null)
								{
									dSiDensity = component._StandardDensity.SIValue;
									subLineItem.Density = this.ConvertUnits(dSiDensity, EngineeringUnit.FmdKgM3, subLineItem.DensityUnits);
								}
							}
							else
							{
								dSiDensity = this.ConvertUnits(subLineItem.Density.Value, subLineItem.DensityUnits, EngineeringUnit.FmdKgM3);
							}

							this.CalculateVCF(subLineItem);

							subLineItem.BrokenBlend = false;

							double componentBlendPercentage = componentMap.BlendPercentage / 100;

							if (lineItem.PresetAmount != null)
							{
								subLineItem.PresetAmount =
									Math.Round(
										componentBlendPercentage
										* this.ConvertUnits(lineItem.PresetAmount.Value, lineItem.VolumeUnits, subLineItem.VolumeUnits),
										subLineItem.VolumeDecimalPlaces,
										MidpointRounding.AwayFromZero);
							}

							if (subLineItem.Quantity.GrossManualValueFlag != true)
							{
								subLineItem.Quantity.GrossInventoryChange =
										Math.Round(
											componentBlendPercentage
											* this.ConvertUnits(lineItem.Quantity.GrossInventoryChange, lineItem.VolumeUnits, subLineItem.VolumeUnits),
											subLineItem.VolumeDecimalPlaces,
											MidpointRounding.AwayFromZero);
							}

							if (subLineItem.Quantity.NetManualValueFlag != true)
							{
								subLineItem.Quantity.NetInventoryChange = subLineItem.Quantity.GrossInventoryChange * subLineItem.VCF.Value;
							}

							if (subLineItem.Quantity.MassManualValueFlag != true)
							{
								double dSiVolume = this.ConvertUnits(subLineItem.Quantity.NetInventoryChange,
																subLineItem.VolumeUnits,
																EngineeringUnit.FmvMeter3);

								// round volume to nearest 0.001
								dSiVolume = Math.Round(dSiVolume, 3, MidpointRounding.AwayFromZero);

								double dSiMassQuantity = dSiVolume * dSiDensity;
								subLineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																							EngineeringUnit.FmmKg,
																							lineItem.MassUnits);
							}
						}
					}

					else if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
					{
						//the line item may or may not have an additive profile, so check to make sure it does before accessing it
						ProductMapClass additiveMap = additiveProfile?.AdditiveCollection.Find(x => x.AssignedGuid == subLineItem.ProductGuid);
						if (additiveMap != null)
						{
							double cycleVolume = this.ConvertUnits(
									additiveMap._AdditiveCycleVolume.SIValue,
									EngineeringUnit.FmvMeter3,
									subLineItem.VolumeUnits);

							double rate = this.ConvertUnits(
									additiveMap._AdditiveRate.SIValue,
									EngineeringUnit.FmvMeter3,
									lineItem.VolumeUnits);

							// Additive Temmperature is Line Item Temperature unless set by load rack or user
							if (subLineItem.Temperature == null)
							{
								subLineItem.Temperature = lineItem.Temperature;
							}

							// Additive Standard Density is product Density by load rack or user
							double dSiDensity = 0.0;
							if (subLineItem.Density == null)
							{
								ProductClass additive = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByID(this.security, subLineItem.Product));
								if (additive != null)
								{
									dSiDensity = additive._StandardDensity.SIValue;
									subLineItem.Density = this.ConvertUnits(dSiDensity, EngineeringUnit.FmdKgM3, subLineItem.DensityUnits);
								}
							}
							else
							{
								dSiDensity = this.ConvertUnits(subLineItem.Density.Value, subLineItem.DensityUnits, EngineeringUnit.FmdKgM3);
							}

							this.CalculateVCF(subLineItem);

							subLineItem.ImproperAdditization = false;

							if (lineItem.PresetAmount != null)
							{
								subLineItem.PresetAmount = Math.Round(
									((int)(lineItem.PresetAmount.Value / rate)) * cycleVolume,
									subLineItem.VolumeDecimalPlaces,
									MidpointRounding.AwayFromZero);
							}

							if (subLineItem.Quantity.GrossManualValueFlag != true)
							{
								subLineItem.Quantity.GrossInventoryChange =
										Math.Round(
											((int)(lineItem.Quantity.GrossInventoryChange / rate)) * cycleVolume,
											subLineItem.VolumeDecimalPlaces,
											MidpointRounding.AwayFromZero);
							}

							if (subLineItem.Quantity.NetManualValueFlag != true)
							{
								subLineItem.Quantity.NetInventoryChange = subLineItem.Quantity.GrossInventoryChange * subLineItem.VCF.Value;
							}

							if (subLineItem.Quantity.MassManualValueFlag != true)
							{

								double dSiVolume = this.ConvertUnits(subLineItem.Quantity.NetInventoryChange,
																subLineItem.VolumeUnits,
																EngineeringUnit.FmvMeter3);

								// round volume to nearest 0.001
								dSiVolume = Math.Round(dSiVolume, 3, MidpointRounding.AwayFromZero);

								double dSiMassQuantity = dSiVolume * dSiDensity;
								subLineItem.Quantity.MassInventoryChange = this.ConvertUnits(dSiMassQuantity,
																							EngineeringUnit.FmmKg,
																							lineItem.MassUnits);
							}
						}
					}
				}

				// Now check that the sum of the sublineitem volumes match up to the lineitem volume and set the lineItem Net and Mass
				if (product.ProductType == ProductType.BlendProduct)
				{
					double grossSum = 0.0;
					double netSum = 0.0;
					double presetSum = 0.0;
					double massSum = 0.0;
					SubLineItemDO smallestSubLine = null;

					if (lineItem.SubLineItems.Count > 0)
					{
						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
							{
								grossSum += Math.Round(subLineItem.Quantity.GrossInventoryChange, subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
								netSum += Math.Round(subLineItem.Quantity.NetInventoryChange, subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
								massSum += Math.Round(subLineItem.Quantity.MassInventoryChange, subLineItem.MassDecimalPlaces, MidpointRounding.AwayFromZero);
								presetSum += Math.Round((subLineItem.PresetAmount != null) ? subLineItem.PresetAmount.Value : 0.0, subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
								if (smallestSubLine == null || smallestSubLine.Quantity.Gross > subLineItem.Quantity.Gross)
								{
									smallestSubLine = subLineItem;
								}
							}

							lineItem.Quantity.Net = netSum;
							lineItem.Quantity.Mass = massSum;
						}

						// If quantities don't match by 1, add the difference to the smallest line item to make it match, per D. Blagg/J. Ussery (bug 35920)
						// If they differ by more than one, something other than rounding is in play; do no fixup.
						// Equation below allows for floating point error
						if (smallestSubLine != null)
						{
							if (Math.Abs(Math.Abs(grossSum - lineItem.Quantity.GrossInventoryChange) - Math.Pow(10.0, -1.0 * lineItem.VolumeDecimalPlaces)) < 0.0000001)
							{
								smallestSubLine.Quantity.GrossInventoryChange -= grossSum - lineItem.Quantity.GrossInventoryChange;
							}

							if (Math.Abs(Math.Abs(netSum - lineItem.Quantity.NetInventoryChange) - Math.Pow(10.0, -1.0 * lineItem.VolumeDecimalPlaces)) < 0.0000001)
							{
								smallestSubLine.Quantity.NetInventoryChange -= netSum - lineItem.Quantity.NetInventoryChange;
							}

							if (Math.Abs(Math.Abs(massSum - lineItem.Quantity.MassInventoryChange) - Math.Pow(10.0, -1.0 * lineItem.MassDecimalPlaces)) < 0.0000001)
							{
								smallestSubLine.Quantity.MassInventoryChange -= massSum - lineItem.Quantity.MassInventoryChange;
							}

							if (lineItem.PresetAmount != null
								&& (Math.Abs(Math.Abs(presetSum - lineItem.PresetAmount.Value) - Math.Pow(10.0, -1.0 * lineItem.VolumeDecimalPlaces)) < 0.0000001))
							{
								if (smallestSubLine.PresetAmount != null)
								{
									smallestSubLine.PresetAmount -= presetSum - lineItem.PresetAmount.Value;
								}
							}
						}

						// Determine temperature for the batch
						bool allComponentsHaveItem = true;
						double averagedItem = 0.0;

						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
							{
								continue;
							}

							if (subLineItem.Temperature == null
							|| subLineItem.Quantity == null
							|| lineItem.Quantity.NetInventoryChange == 0)
							{
								allComponentsHaveItem = false;
								break;
							}

							averagedItem += this.ConvertUnits(subLineItem.Temperature.Value,
																		subLineItem.TemperatureUnits,
																		lineItem.TemperatureUnits) * (this.ConvertUnits(subLineItem.Quantity.NetInventoryChange,
																																		subLineItem.VolumeUnits,
																																		lineItem.VolumeUnits) / lineItem.Quantity.NetInventoryChange);
						}

						EthanolExpansionScenarioHelper ee = new EthanolExpansionScenarioHelper();
						if (allComponentsHaveItem
							&& lineItem.SubLineItems.Count > 0
							&& !ee.isEEScenario2(lineItem, this.security))
						{
							lineItem.Temperature = averagedItem;
						}

						// Determine density for the batch
						averagedItem = 0.0;
						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
							{
								continue;
							}

							if (subLineItem.Density == null
							|| subLineItem.Quantity == null
							|| lineItem.Quantity.NetInventoryChange == 0)
							{
								allComponentsHaveItem = false;
								break;
							}

							averagedItem += this.ConvertUnits(subLineItem.Density.Value, subLineItem.DensityUnits, lineItem.DensityUnits)
															* (this.ConvertUnits(subLineItem.Quantity.NetInventoryChange,
																						subLineItem.VolumeUnits,
																						lineItem.VolumeUnits) / lineItem.Quantity.NetInventoryChange);
						}

						if (allComponentsHaveItem && averagedItem != 0.0)
						{
                            lineItem.Density = averagedItem;
							lineItem.Quantity.VcfManualValueFlag = false;
							this.CalculateVCF(lineItem);
						}
					}
				}
			}
		}


		[SecurityCritical]
		protected virtual void CalculateVolumes(LineItemDO parentLineItem, SubLineItemDO subLineItem)
		{
			this.CalculateQty(subLineItem);


			if (parentLineItem.ProductGuid != Guid.Empty)
			{
				ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByProductAuthorizedCompanies(this.security, parentLineItem.ProductGuid, false));

				if (parentLineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct))
				{
					double totalGrossVolume = 0;
					double totalNetVolume = 0;
					double totalTemp = 0;
					double totalMassQuantity = 0;
					double totalPackageQuantity = 0;

					// Iterate through the sub-line items and total the net volumes
					// and temperatures.  If the temperatures are all null then
					// do not set anything for the parent line item temp
					bool isSubTempNull = true;

					foreach (SubLineItemDO subLine in parentLineItem.SubLineItems)
					{
						if (subLine == null)
						{
							continue;
						}

						if (subLine.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
						{
							continue;
						}

						if (subLine.Quantity != null)
						{
							totalGrossVolume += this.ConvertUnits(
								subLine.Quantity.GrossInventoryChange,
								subLine.VolumeUnits,
								parentLineItem.VolumeUnits);

							totalNetVolume += this.ConvertUnits(
								subLine.Quantity.NetInventoryChange,
								subLine.VolumeUnits,
								parentLineItem.VolumeUnits);

							totalMassQuantity += this.ConvertUnits(subLine.Quantity.Mass, subLine.MassUnits, parentLineItem.MassUnits);
							totalPackageQuantity += subLine.Quantity.Package;

							if (subLine.Temperature != null)
							{
								totalTemp += (this.ConvertUnits(
									subLine.Temperature.Value,
									subLine.TemperatureUnits,
									parentLineItem.TemperatureUnits)
												* this.ConvertUnits(
													subLine.Quantity.GrossInventoryChange,
													subLine.VolumeUnits,
													parentLineItem.VolumeUnits));
								isSubTempNull = false;
							}
						}
					}

					// Set the line item's volume
					parentLineItem.Quantity.GrossManualValueFlag = false;
					parentLineItem.Quantity.GrossInventoryChange = totalGrossVolume;
					parentLineItem.Quantity.NetManualValueFlag = false;
					parentLineItem.Quantity.NetInventoryChange = totalNetVolume;
					parentLineItem.Quantity.MassManualValueFlag = false;
					parentLineItem.Quantity.MassInventoryChange = totalMassQuantity;
					parentLineItem.Quantity.PackageManualValueFlag = false;
					parentLineItem.Quantity.PackageInventoryChange = totalPackageQuantity;

					double weightedAverageTemp = totalTemp / totalGrossVolume;

					if (!isSubTempNull)
					{
						parentLineItem.Temperature = weightedAverageTemp;
					}
					else
					{
						parentLineItem.Temperature = null;
					}

					// Now calculate the density
					// This was copied then modified from StationManager.RollUpSplashBlendTotals()
					if (parentLineItem.Quantity.NetInventoryChange != 0.0)
					{
						double lineItemDensity = 0;

						// As the sub-line items are iterated through...
						// if any of the sub-line item densities are null there's
						// no good way to roll-up the density to the parent so
						// check for it
						bool isSubDensityNull = true;

						foreach (SubLineItemDO subLine in parentLineItem.SubLineItems)
						{
							if (subLine == null)
							{
								continue;
							}

							if (subLine.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
							{
								continue;
							}

							if (subLine.Density != null)
							{
								isSubDensityNull = false;
							}
							else
							{
								continue;
							}

							if (subLine.Quantity != null)
							{
								lineItemDensity += (this.ConvertUnits(subLine.Density.Value, subLine.DensityUnits, parentLineItem.DensityUnits)
														* this.ConvertUnits(
															subLine.Quantity.NetInventoryChange,
															subLine.VolumeUnits,
															parentLineItem.VolumeUnits) / parentLineItem.Quantity.NetInventoryChange);
							}
						}

						if (!isSubDensityNull)
						{
							parentLineItem.Density = lineItemDensity;
						}
					}

					//calcuate LineItem VCF for blend after calculating the (average) density and temperature based on subLineItems
					//this function includes temperature and density nullable checking, if one is null, VCF value will not be updated
					parentLineItem.Quantity.VcfManualValueFlag = false;
					this.CalculateVCF(parentLineItem);

					// Determine if Blend Components are in tolerance
					bool brokenBlend = false;

					foreach (SubLineItemDO subLine in parentLineItem.SubLineItems)
					{
						if (subLine == null)
						{
							continue;
						}

						if (subLine.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct))
						{
							continue;
						}

						ProductMapClass component = product.ComponentCollection.Find(x => x.AssignedGuid == subLine.ProductGuid);
						if (component == null)
						{
							continue;
						}

						// Total the Gross & Net volume for the component because for external components
						// there may be multiple subline items
						double grossVolume = 0;
						double netVolume = 0;

						foreach (SubLineItemDO secondSubLineItem in parentLineItem.SubLineItems)
						{
							if (subLine.ProductGuid == secondSubLineItem.ProductGuid)
							{
								if (secondSubLineItem.Quantity != null)
								{
									grossVolume += this.ConvertUnits(
										secondSubLineItem.Quantity.Gross,
										secondSubLineItem.VolumeUnits,
										parentLineItem.VolumeUnits);
									netVolume += this.ConvertUnits(
										secondSubLineItem.Quantity.Net,
										secondSubLineItem.VolumeUnits,
										parentLineItem.VolumeUnits);
								}
							}
						}

						if (this.TransContext.accountingSite.CurrentSite.LoadByNet)
						{
							double requiredAmount = (parentLineItem.Quantity.Net * component.BlendPercentage) / 100.0;

							if (Math.Abs(requiredAmount - netVolume) / requiredAmount > (double)product._ComponentTolerance.Value / 100.0)
							{
								brokenBlend = true;
								break;
							}
						}
						else
						{
							double requiredAmount = parentLineItem.Quantity.Gross * component.BlendPercentage / 100.0;

							if (Math.Abs(requiredAmount - grossVolume) / requiredAmount > (double)product._ComponentTolerance.Value / 100.0)
							{
								brokenBlend = true;
								break;
							}
						}
					}

					parentLineItem.BrokenBlend = brokenBlend;

					foreach (SubLineItemDO subLine in parentLineItem.SubLineItems)
					{
						if (subLine == null)
						{
							continue;
						}

						if (subLine.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct))
						{
							continue;
						}

						subLine.BrokenBlend = parentLineItem.BrokenBlend;
					}
				}
			}

			// Determine if Additives are in tolerance
			bool improperlyAdditized = false;

			if (parentLineItem.AdditiveProfileGuid != Guid.Empty)
			{
				AdditiveProfileClass additiveProfile =
					FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
						x => x.Get(this.security, parentLineItem.AdditiveProfileGuid));

				foreach (SubLineItemDO subLine in parentLineItem.SubLineItems)
				{
					if (subLine == null)
					{
						continue;
					}

					if (subLine.ProductType != ProductClass.ProductTypeID(ProductType.AdditiveProduct))
					{
						continue;
					}

					ProductMapClass additive = additiveProfile.AdditiveCollection.Find(x => x.IdentityGuid == subLine.ProductGuid);
					if (additive == null)
					{
						continue;
					}

					double cycleVolume = this.ConvertUnits(
						additive._AdditiveCycleVolume.SIValue,
						EngineeringUnit.FmvMeter3,
						subLine.VolumeUnits);
					double rate = this.ConvertUnits(
						additive._AdditiveRate.SIValue,
						EngineeringUnit.FmvMeter3,
						parentLineItem.VolumeUnits);

					double requiredAmount = parentLineItem.Quantity.Net / rate * cycleVolume;

					if (Math.Abs(requiredAmount - subLine.Quantity.Net) / requiredAmount > additive.Tolerance / 100.0)
					{
						subLine.ImproperAdditization = true;
						improperlyAdditized = true;
					}
					else
					{
						subLine.ImproperAdditization = false;
					}
				}

				parentLineItem.ImproperAdditization = improperlyAdditized;
			}
		}

		protected void CalculateVCF(LineItemDO lineItem)
		{
			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByProductAuthorizedCompanies(this.security, lineItem.ProductGuid, false));

			if (lineItem.Quantity.VcfManualValueFlag == false || lineItem.Quantity.VcfManualValueFlag == null)
			{
				if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct))
				{
					if(lineItem.Quantity.GrossInventoryChange != 0.0)
					{
						lineItem.VCF = Math.Round(lineItem.Quantity.NetInventoryChange / lineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);
					}
					else
					{
						lineItem.VCF = 1.0;
					}
				}
				else
				{
					if (lineItem.Temperature != null
					&& lineItem.Density != null
					&& product != null
					&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE
					&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE_1980)
					{
						double standardDensity = this.ConvertUnits(lineItem.Density.Value, lineItem.DensityUnits, this.TransContext.accountingSite.CurrentSite.DensityUnits);

						Vcf VolumeCorrection = new Vcf();

						VolumeCorrection.VcfSettings = product._VcfModuleSettings.GetCommonComponentVcfModuleSettings(product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits);

						lineItem.VCF = VolumeCorrection.VcfCalculation((ECorrectionTypeMajor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
											(ECorrectionTypeMinor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
											(lineItem.Temperature.HasValue) ? lineItem.Temperature.Value : 0.0,
											lineItem.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : lineItem.TemperatureUnits,
											product._VcfModuleSettings.BaseTemperature.Value,
											product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
											standardDensity,
											product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.DensityUnits : product.DensityUnits,
											(lineItem.Pressure.HasValue) ? lineItem.Pressure.Value : 0.0,
											product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits,
											0.0,
											product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
											0.0,
											product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits,
											new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
					}
					else
					{
						lineItem.VCF = 1;
					}
				}
			}
		}


		protected void CalculateVCF(SubLineItemDO subLineItem)
		{
			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetByProductAuthorizedCompanies(this.security, subLineItem.ProductGuid, false));

			if (subLineItem.Quantity.VcfManualValueFlag == false || subLineItem.Quantity.VcfManualValueFlag == null)
			{
				if (subLineItem.Temperature != null
				&& subLineItem.Density != null
				&& product != null
				&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE
				&& product._VcfModuleSettings.CorrectionMethodType != ECorrectionTypeMajor.CORR_NONE_1980)
				{
					double standardDensity = this.ConvertUnits(subLineItem.Density.Value, subLineItem.DensityUnits, this.TransContext.accountingSite.CurrentSite.DensityUnits);

					Vcf VolumeCorrection = new Vcf();

					VolumeCorrection.VcfSettings = product._VcfModuleSettings.GetCommonComponentVcfModuleSettings(product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits);

					subLineItem.VCF = VolumeCorrection.VcfCalculation((ECorrectionTypeMajor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
										(ECorrectionTypeMinor)Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
										(subLineItem.Temperature.HasValue) ? subLineItem.Temperature.Value : 0.0,
										subLineItem.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : subLineItem.TemperatureUnits,
										product._VcfModuleSettings.BaseTemperature.Value,
										product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
										standardDensity,
										product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.DensityUnits : product.DensityUnits,
										(subLineItem.Pressure.HasValue) ? subLineItem.Pressure.Value : 0.0,
										product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits,
										0.0,
										product.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.TemperatureUnits : product.TemperatureUnits,
										0.0,
										product.PressureUnits == EngineeringUnit.FmSiteUnits ? this.AccountingSite.CurrentSite.PressureUnits : product.PressureUnits,
										new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
				}
				else
				{
					subLineItem.VCF = 1.0;
				}
			}
		}



		/// <summary>
		/// The check for and display warning messages internal.
		/// </summary>
		/// <param name="resultDO">
		/// The result do.
		/// </param>
		protected void CheckForAndDisplayWarningMessagesInternal(SaveTransactionsResultDO resultDO)
		{
			CheckForAndDisplayWarningMessages(resultDO, this.AccountingSite.CurrentSiteGuid, this, this.Logger);
		}

		/// <summary>
		/// This method will close the transaction detail page and transfer to the URL in the
		/// transaction list context.  If the transaction list context does not exist, then
		/// one is created and a new URL is built returning to the transaction list page.
		/// </summary>
		protected virtual void Close()
		{
			string returnPage = "../FMWebApp/FuelsManagerForm.aspx";

			try
			{
				var associatedTxContext = this.Session["AssociatedTxContext"] as AssociatedTxContext;
				var transDetailList = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList;
				var orderContext = this.Session["OrderAssociatedTxContext"] as OrderAssociatedTxContext;
				var supplyOrderContext = this.Session["SupplyOrderAssociatedTxContext"] as SupplyOrderAssociatedTxContext;

				if (this.Request.GetQueryOrFormValue("QueryEditItem").DefaultIfNull(string.Empty).Equals(string.Empty) == false)
				{
					returnPage = "..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning";
				}
				else if (this.Request.GetQueryOrFormValue("MeterReconciliationDetail").DefaultIfNull(string.Empty).Equals(string.Empty) == false)
				{
					returnPage = "../Accounting/MeterReconciliationDetail.aspx?Returning=true";
				}
				else if (associatedTxContext != null)
				{
					returnPage = "../Accounting/AssociatedTxSummary.aspx";
				}
				else if (orderContext != null)
				{
					returnPage = "..\\OrderEntryWebApp\\OrderAssociatedTxSummary.aspx";
				}
				else if (supplyOrderContext != null)
				{
					returnPage = "..\\SupplyOrderWebApp\\SupplyOrderAssociatedTxSummary.aspx";
				}
				else if ((transDetailList != null) && (string.IsNullOrEmpty(transDetailList.ReturnURL) == false))
				{
					returnPage = transDetailList.ReturnURL;

					if (returnPage.IndexOf("Column=" + this.Trans.Alias, StringComparison.Ordinal) == -1)
					{
						int inx = returnPage.IndexOf("Column=", StringComparison.Ordinal);

						if (inx > 0)
						{
							int inx2 = returnPage.IndexOf("&", inx + 1, StringComparison.Ordinal);

							if (inx2 > inx + 1)
							{
								returnPage = returnPage.Substring(0, inx + 7) + this.Trans.Alias + returnPage.Substring(inx2);
							}
							else
							{
								returnPage = returnPage.Substring(0, inx + 7) + Uri.EscapeDataString(this.Trans.Alias);
							}
						}
						else
						{
							returnPage += (returnPage.IndexOf('?') > -1) ? "&" : "?";
							returnPage += "Column=" + this.Trans.Alias;
						}
					}

					this.Session.Remove(TransactionDetailList.TransactionDetailListKey);
				}
				else
				{
					this.UpdateTransDetailList();

					// Escape the alias name for any URL special characters (i.e. & ' / ? ! # $ * + , : ; = @ [ ])
					string columnName = Uri.EscapeDataString(this.Trans.Alias);

					// Build URL for transferring to the transaction list page.
					int row = this.Trans.InventoryDate.Day - 1;
					returnPage = "../Accounting/TransactionList.aspx?Row=";
					returnPage = returnPage + row + "&Column=" + columnName;

					if (this.Trans.TransTypeID == TransactionTypes.T17_Order)
					{
						returnPage = "..\\OrderEntryWebApp\\OrderSummary.aspx";
					}
					else if (this.Trans.TransTypeID == TransactionTypes.T18_SupplyOrder)
					{
						returnPage = "..\\SupplyOrderWebApp\\SupplyOrderSummary.aspx";
					}
					else if (this.Trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
					{
						this.Page.Session.Add("InvoiceSummaryType", "21");
						returnPage = "..\\InvoiceWebApp\\InvoiceSummary.aspx";
					}
					else if (this.Trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
					{
						this.Page.Session.Add("InvoiceSummaryType", "22");
						returnPage = "..\\InvoiceWebApp\\InvoiceSummary.aspx";
					}
					else
					{
						var transactionListContext = this.Session["TransactionListContext"] as TransactionListContext
															?? new TransactionListContext();

						if (transactionListContext.TransactionListReturnURL == string.Empty)
						{
							transactionListContext.Site = this.security.SiteID;
							transactionListContext.Month = this.Trans.InventoryDate.ToString("MMM yyyy", this.TransContext.accountingSite.CurrentSite.GetDateTimeFormatInfo());

							if (!string.IsNullOrEmpty(this.Trans.ManagerID))
							{
								transactionListContext.Manager = this.Trans.ManagerID;
							}

							if (this.Trans.LineItems.Count > 0)
							{
								transactionListContext.Product = this.Trans.LineItems[0].Product;
							}

							if (!string.IsNullOrEmpty(transactionListContext.Product))
							{
								transactionListContext.TransactionListReturnURL = transactionListContext.ReturnURL;

								if (!string.IsNullOrEmpty(this.Trans.OwnerID))
								{
									transactionListContext.Owner = this.Trans.OwnerID;
								}
								this.Session["TransactionListContext"] = transactionListContext;
							}
						}
					}
				}

				this.Reset();
				this.Session.Remove("allAssociatedTransactionsBeforeTransactionEdit");

				this.Redirect(returnPage);
				this.Context.ApplicationInstance.CompleteRequest();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			this.Redirect(returnPage);
		}

		/// <summary>
		/// This method handles the Close button being pressed event. The page is redirected
		/// to the calling page.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected virtual void CloseButtonClick(object sender, EventArgs e)
		{
			try
			{
				if (this.TransContext != null)
				{
					// Retrieve data from the page in order to populate the manager and owner fields.
					if (this.TransContext.aliasClass.TransactionFieldCollection.Find("ManagerID") != null)
					{
						var managerFG = this.TransactionFieldGenerator.GetFieldGenerator("ManagerID") as ManagerFG;
						// This isn't pretty.  The field generator should be able to retrieve the value directly from the protected cell
						// member but to big a change at this time.
						string managerID =
							managerFG?.GetNewValue(this.FindControl(managerFG.GetType().ToString()).Parent as WebControl) as string;

						if (!string.IsNullOrEmpty(managerID) && managerID != this.Trans.ManagerID)
						{
							this.Trans.ManagerID = managerID;
							this.Trans.ManagerCompanyGuid =
								FMChannelHelper.MakeCall<ICompanies, Guid>(
										x => x.GetMasterRecordGuid(this.TransContext.security, this.Trans.ManagerID));
						}
					}

					if (this.TransContext.aliasClass.TransactionFieldCollection.Find("OwnerID") != null)
					{
						var ownerFG = this.TransactionFieldGenerator.GetFieldGenerator("OwnerID") as OwnerFG;
						var ownerID =
							ownerFG?.GetNewValue(this.FindControl(ownerFG.GetType().ToString()).Parent as WebControl) as string;

						if (!string.IsNullOrEmpty(ownerID) && ownerID != this.Trans.OwnerID)
						{
							this.Trans.OwnerID = ownerID;
							this.Trans.OwnerCompanyGuid =
								FMChannelHelper.MakeCall<ICompanies, Guid>(
										x => x.GetMasterRecordGuid(this.TransContext.security, this.Trans.OwnerID));
						}
					}
				}

				this.Close();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method handles the Combine button being pressed event. The
		/// intent is to allow the user to select a BOL for combination
		/// alias and print the report.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected virtual void CombineBtnClick(object sender, EventArgs e)
		{
			this.NoSaveErrors = true;

			if (this.SaveProcessing(sender))
			{
				// Write script to request BOL Summary
				ScriptManager.RegisterClientScriptBlock(this.Page,
														this.GetType(),
														"transactiondetail_envoke_BOLSummary",
														javascriptEnvokeBolSummary,
														false);
			}
		}

		/// <summary>
		/// The construct.
		/// </summary>
		protected override void Construct()
		{
			this.StartTime = DateTimeOffset.Now;
			base.Construct();
			this.Logger = new Logger("Accounting");
			this.Logger.Debug("TransactionDetail.Construct()");
		}

		/// <summary>
		/// This method will convert the Alternate Gross volume and populate the gross quantity
		/// volume.
		/// </summary>
		/// <param name="lineItem">
		/// The line item.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		protected virtual bool ConvertAlternateVolumeToGrossVolume(LineItemDO lineItem)
		{
			if (lineItem == null)
			{
				return true;
			}

			if ((lineItem.AlternativeUnits != null) && (lineItem.AlternativeUnits.Value > 0))
			{
				var grossVolumeTextBox = this.FieldTable.FindControl("TransactionFields.LineItemGrossQuantityFG") as TextBox;

				if (lineItem.Quantity != null && lineItem.AlternativeGrossVolume == null)
				{
					// Set the alternate gross using gross value conversion if the gross field already has a value but the alternate gross doesn't.
					lineItem.AlternativeGrossVolume = 0;

					if (TransactionTypes.T21_AccountPayableInvoice == this.Trans.TransTypeID
						|| TransactionTypes.T22_AccountReceivableInvoice == this.Trans.TransTypeID)
					{
						lineItem.AlternativeGrossVolume = this.ConvertUnits(
							lineItem.Quantity.GrossInventoryChange,
							lineItem.VolumeUnits,
							(EngineeringUnit)lineItem.AlternativeUnits.Value);
					}
					else
					{
						lineItem.AlternativeGrossVolume = this.ConvertUnits(
							lineItem.Quantity.Gross,
							lineItem.VolumeUnits,
							(EngineeringUnit)lineItem.AlternativeUnits.Value);
					}
				}

				if (lineItem.AlternativeGrossVolume == null)
				{
					// If transaction is an aggregate type then alternate volume needs to be based on gross quantity
					this.HandleFieldError(new Exception("Field AlternativeGrossVolume must be assigned a value."));
					return false;
				}

				if (lineItem.Quantity != null)
				{
					lineItem.Quantity.Gross = this.ConvertUnits(lineItem.AlternativeGrossVolume.Value,
																(EngineeringUnit)lineItem.AlternativeUnits.Value,
																lineItem.VolumeUnits);

					if (grossVolumeTextBox != null)
					{
						grossVolumeTextBox.Text = lineItem.Quantity.Gross.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			else
			{
				lineItem.AlternativeGrossVolume = null;
			}

			return true;
		}

		[SecurityCritical]
		protected double ConvertUnits(double source, EngineeringUnit sourceUnits, EngineeringUnit resultUnits)
		{
			// Use the accounting site conversion functions to convert
			double result = 0;

			EngineeringUnits.Convert(source, sourceUnits, ref result, resultUnits, 0);

			return result;
		}

		protected virtual void Delete()
		{
			// Different action if the transaction is already deleted. (24-Jun-2009 IGO)
			if (this.Trans.DeleteFlag)
			{
				this.Trans.DeleteFlag = false;

				foreach (LineItemDO lineItem in this.Trans.LineItems)
				{
					lineItem.DeleteFlag = false;
					foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
					{
						subLineItem.DeleteFlag = false;
					}
				}
			}
			else
			{
				this.Trans.DeleteFlag = true;

				foreach (LineItemDO lineItem in this.Trans.LineItems)
				{
					lineItem.DeleteFlag = true;
					//Remove Associations
					lineItem.AssociatedTransactions.Clear();

					foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
					{
						subLineItem.DeleteFlag = true;
					}
				}
			}

			// Call the save method
			if (this.Save())
			{
				this.Close();
			}
			else
			{
				if (this.Trans.DeleteFlag)
				{
					this.Trans.DeleteFlag = false;
					foreach (LineItemDO lineItem in this.Trans.LineItems)
					{
						lineItem.DeleteFlag = false;
						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							subLineItem.DeleteFlag = false;
						}
					}
				}
				else
				{
					this.Trans.DeleteFlag = true;
					foreach (LineItemDO lineItem in this.Trans.LineItems)
					{
						lineItem.DeleteFlag = true;
						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							subLineItem.DeleteFlag = true;
						}
					}
				}
			}

			this.Session.Remove("allAssociatedTransactionsBeforeTransactionEdit");
		}

		protected virtual void DeleteButtonClick(object sender, EventArgs e)
		{
			var sr = new AssociatedTxSR
			{
				RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions,
				TransID = this.Trans.TransID,
				Security = this.security
			};

			sr.RequestType = AssociatedTxSR.RequestTypes.GetAssociatedParentTransactions;
			sr.TransID = this.Trans.TransID;
			sr.Security = this.security;

			AssociatedTxListDO txList = FMChannelHelper.MakeCall<IAssociatedTxProcessor, AssociatedTxListDO>(x => x.Process(sr));

			// Here txList.AssociatedTransactions contain parent transactions to which this transaction is associated.
			if (txList.AssociatedTransactions.Tables.Count > 0 && txList.AssociatedTransactions.Tables[0].Rows.Count > 0)
			{
				// Get user confirmation if the transaction is associated to other transactions.
				const string Message = "Transaction is associated to other transactions.\nClick OK to continue with delete.";

				ScriptManager.RegisterStartupScript(
													this,
													this.GetType(),
													"DELETE_CONFIRMATION",
													"if (confirm('" + HttpUtility.JavaScriptStringEncode(Message)
													+ "')) {__mydoPostBack('DELETE_CONFIRMATION', 'OK');}",
													true);

				return;
			}

			this.Delete();
		}

		protected virtual void DisableButtonsForEditingCustomized()
		{

		}

		/// <summary>
		///	This method will determine if the transaction buttons are viewable and
		///	either disable or enable the ones that are.
		/// </summary>
		protected void DisableButtonsForEditing()
		{
			this.ReverseButton.Enabled = false;
			this.ReverseUpdateButton.Enabled = false;
			this.NewButton.Enabled = false;
			this.NewLineItemButton.Enabled = false;
			this.SaveButton.Enabled = false;
			this.CloseButton.Enabled = false;
			this.DeleteButton.Enabled = false;
			this.ViewPrintableBtn.Enabled = false;
			this.NextButton.Enabled = false;
			this.PreviousButton.Enabled = false;

			if (this.NewTransportButton.Visible)
			{
				this.NewTransportButton.Enabled = false;
			}

			if (this.NewAGRButton.Visible)
			{
				this.NewAGRButton.Enabled = false;
			}

			DisableButtonsForEditingCustomized();
		}

		protected virtual void EnableFieldTable(bool enable, bool ignoreCloseoutStatus)
		{
			// Ignore closeout is currently redundant because a requirement was changed and this is no
			// longer required, but may become useful again down the track.
			if (ignoreCloseoutStatus)
			{
				// temporarily force no closeout because post CCP-042 we do not disable fields if disabled
				bool closeoutStatus = this.Trans.PartialCloseout;
				this.Trans.PartialCloseout = false;

				enable = enable && this.IsTransactionEditable;

				this.Trans.PartialCloseout = closeoutStatus;
			}
			else
			{
				enable = enable && this.IsTransactionEditable && (this.Trans.PartialCloseout == false);
			}

			foreach (TableRow row in this.FieldTable.Rows)
			{
				foreach (TableCell cell in row.Cells)
				{
					if (!cell.ID.StartsWith("FieldValue"))
					{
						continue;
					}

					string key = cell.ID.Replace("FieldValue ", string.Empty);

					FieldGenerator fieldGenerator = this.TransactionFieldGenerator.GetFieldGenerator(key);

					foreach (Control control in cell.Controls)
					{
						var updatePanel = control as UpdatePanel;

						if (updatePanel == null)
						{
							continue;
						}

						var webControl = updatePanel.ContentTemplateContainer.Controls[0] as WebControl;
						if (webControl != null)
						{
							if (enable)
							{
								var textBox = webControl as TextBox;
								if (textBox != null)
								{
									textBox.Enabled = true;
									textBox.ReadOnly = (fieldGenerator != null) && !fieldGenerator.Editable;

									if (textBox.ReadOnly)
									{
										textBox.BackColor = Color.LightGray;
									}
									else
									{
										textBox.BackColor = Color.White;
									}
								}
								else
								{
									webControl.Enabled = (fieldGenerator == null) || fieldGenerator.Editable;
								}
							}
							else
							{
								webControl.Enabled = false;
							}

							continue;
						}

						var htmlControl = updatePanel.ContentTemplateContainer.Controls[0] as HtmlControl;
						if (htmlControl != null)
						{
							if (enable)
							{
								htmlControl.Disabled = (fieldGenerator != null) && !fieldGenerator.Editable;
							}
							else
							{
								htmlControl.Disabled = true;
							}
						}
					}
				}
			}

			this.UpdatePanel1.Update();
		}

		protected virtual void EnterKeyButtonClick(object sender, EventArgs e)
		{
			this.Logger.Debug("Enter key pressed.");
			switch (this.EnterKeySource.Value)
			{
				case "AGR":
					if (this.GaugeReadingsDataGrid.EditItemIndex != -1)
					{
						this.RetrieveGaugeReading(this.GaugeReadingsDataGrid.Items[this.GaugeReadingsDataGrid.EditItemIndex]);
						this.GaugeReadingsDataGrid.EditItemIndex = -1;
						return;
					}
					break;
				case "LineItem":
					if (this.LineItemDataGrid.EditItemIndex != -1)
					{
						bool bNoLineItemErrors = this.RetrieveLineItem(this.LineItemDataGrid.Items[this.LineItemDataGrid.EditItemIndex]);
						if (bNoLineItemErrors)
						{
							this.LineItemDataGrid.EditItemIndex = -1;
							return;
						}
					}
					break;
				case "TransportLineItem":
					if (this.TransportDataGrid.EditItemIndex != -1)
					{
						this.RetrieveTransportLineItems(this.TransportDataGrid.Items[this.TransportDataGrid.EditItemIndex]);
						this.TransportDataGrid.EditItemIndex = -1;
					}
					break;
			}

			switch (this.TransContext.mode)
			{
				// JS20100803 WI-16549 added extra logic, for example, shouldn't simulate new button
				// click if it is not enabled.
				case TransactionContext.Mode.Add:
					if (this.NewButton.Enabled)
					{
						this.NewButtonClick(this.NewButton, null);
					}
					else if (this.SaveButton.Enabled)
					{
						this.SaveButtonClick(this.SaveButton, null);
					}
					else
					{
						this.RetrieveDataFromPage();
						this.Close();
					}

					break;
				case TransactionContext.Mode.Edit:
					if (this.SaveButton.Enabled)
					{
						this.SaveButtonClick(this.SaveButton, null);
					}
					else
					{
						this.RetrieveDataFromPage();
						this.Close();
					}

					break;
				case TransactionContext.Mode.View:
					this.RetrieveDataFromPage();
					this.Close();

					break;
			}
		}

		/// <summary>
		///	This method handles the Gauge Readings Data Grid cancel command event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected virtual void GaugeReadingsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			if (this.Session[SessionGaugeReadingGridInAddMode] != null)
			{
				this.Trans.WeightReadings.RemoveAt(this.GaugeReadingsDataGrid.EditItemIndex);
				this.Session.Remove(SessionGaugeReadingGridInAddMode);
			}
			this.GaugeReadingsDataGrid.EditItemIndex = -1;
			this.EnableFieldTable(true, false);
			this.NewAGRButton.Enabled = true;

			if (this.NewLineItemButton.Visible)
			{
				this.NewLineItemButton.Enabled = true;
			}

			if (this.NewTransportButton.Visible)
			{
				this.NewTransportButton.Enabled = true;
			}

			// Set the buttons back to the previous settings prior to the line
			// item edit.
			this.AgrGridGenerator.Bind();
			this.SetButtons();
		}

		/// <summary>
		///	This method handles the Gauge Readings Data Grid delete command event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected virtual void GaugeReadingsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			this.Trans.WeightReadings.RemoveAt(e.Item.ItemIndex);

			if (e.Item.ItemIndex == this.GaugeReadingsDataGrid.EditItemIndex)
			{
				this.GaugeReadingsDataGrid.EditItemIndex = -1;
				this.EnableFieldTable(true, false);
				this.NewAGRButton.Enabled = true;

				if (this.NewLineItemButton.Visible)
				{
					this.NewLineItemButton.Enabled = true;
				}

				if (this.NewTransportButton.Visible)
				{
					this.NewTransportButton.Enabled = true;
				}
			}

			this.AgrGridGenerator.Bind();
		}

		protected virtual void GaugeReadingsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.RetrieveDataFromPage();
			this.GaugeReadingsDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.GaugeReadingsDataGrid.SelectedIndex = e.Item.ItemIndex;
			this.AgrGridGenerator.Bind();
			this.EnableFieldTable(false, false);

			this.DisableButtonsForEditing();
		}

		protected virtual void GaugeReadingsDataGridSelectedIndexChanged(object sender, EventArgs e)
		{
			var dataGridItemEventArgs = e as DataGridItemEventArgs;

			if (dataGridItemEventArgs != null)
			{
				this.GaugeReadingsDataGrid.SelectedIndex = dataGridItemEventArgs.Item.ItemIndex;
			}
			this.AgrGridGenerator.Bind();
			this.DisableButtonsForEditing();
		}

		/// <summary>
		///	This method handles the Gauge Reading Data Grid update command.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected virtual void GaugeReadingsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			if (this.Session[SessionGaugeReadingGridInAddMode] != null)
			{
				this.Session.Remove(SessionGaugeReadingGridInAddMode);
			}
			this.RetrieveGaugeReading(e.Item);
			this.GaugeReadingsDataGrid.EditItemIndex = -1;
			this.AgrGridGenerator.Bind();
			this.EnableFieldTable(true, false);
			this.NewAGRButton.Enabled = true;

			if (this.NewLineItemButton.Visible)
			{
				this.NewLineItemButton.Enabled = true;
			}

			if (this.NewTransportButton.Visible)
			{
				this.NewTransportButton.Enabled = true;
			}

			// Set the buttons back to the previous settings prior to the line
			// item edit.
			this.SetButtons();
		}

		protected virtual void GaugeReadingsItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				return;
			}

			this.SetGaugeReadingsDeleteAndEditButtonState(e);
		}

		/// <summary>
		///	Retrieves specified lineitem's associated transactions' aggregated field values
		///	quantity, excise amount, gst amount, markup amount, total value, and
		///	"total price with tax".
		/// </summary>
		/// <param name="lineItem"></param>
		protected void GetAssociatedTransactionAggregates(LineItemDO lineItem)
		{
			string product = null;

			if (this.LineItemDataGrid.EditItemIndex > -1)
			{
				// Editing line item. Retrieve product from control since it will have the most recent product selection.
				var productTextBox =
					this.LineItemDataGrid.Items[this.LineItemDataGrid.EditItemIndex].FindControl("TransactionFields.LineItemProductFG")
					as TextBox;
				if (productTextBox != null)
				{
					product = productTextBox.Text;
				}

				var locationTextBox =
					this.LineItemDataGrid.Items[this.LineItemDataGrid.EditItemIndex].FindControl(
						"TransactionFields.LineItemDeliveryLocationFG") as TextBox;

				if (locationTextBox != null)
				{
					lineItem.DeliveryLocation = locationTextBox.Text;
				}
			}
			else
			{
				// Not editing line item. Get product from line item.
				product = lineItem.Product;
			}

			if (string.IsNullOrEmpty(product))
			{
				return;
			}

			var sr = new AssociatedTxSR
			{
				RequestType = AssociatedTxSR.RequestTypes.GetAssociatedTransactionDetails,
				TransID = this.Trans.TransID,
				TransactionLineItemGuid = lineItem.TransactionLineItemGuid,
				CurrentSiteGuid = this.AccountingSite.CurrentSiteGuid
			};

			var lineItemGuidToAssocTx = new Hashtable(lineItem.AssociatedTransactions.Count);

			// get the associated transaction ids and line item ids
			foreach (AssociatedTxDO assocTx in lineItem.AssociatedTransactions)
			{
				if (assocTx.Associated == 1)
				{
					sr.AssociatedTransactionIDs.Add(assocTx);
					lineItemGuidToAssocTx.Add(assocTx.TransactionLineItemGuid.ToString(), assocTx);
				}
			}

			sr.TransactionAliasGuid = this.Trans.TransactionAliasGuid;
			sr.Security = this.security;

			// Retrieve the list of associated transactions
			AssociatedTxListDO txList = FMChannelHelper.MakeCall<IAssociatedTxProcessor, AssociatedTxListDO>(x => x.Process(sr));

			// Iterate through the dataset and create a BaseCollections object that
			// can be used by ListViews
			if (txList.AssociatedTransactions.Tables.Count > 0)
			{
				foreach (DataRow dr in txList.AssociatedTransactions.Tables[0].Rows)
				{
					AssociatedTxDO populatedAssociatedTx = this.PopulateAssociatedTxDO(dr, AssociatedTxSR.RequestTypes.GetAssociatedTransactionDetails);
					var associatedTx = lineItemGuidToAssocTx[populatedAssociatedTx.TransactionLineItemGuid.ToString()] as AssociatedTxDO;

					if (associatedTx != null)
					{
						if (associatedTx.Associated == 0)
						{
							continue;
						}

						associatedTx.TransTypeID = populatedAssociatedTx.TransTypeID;
						associatedTx.GrossQuantityReceived = populatedAssociatedTx.GrossQuantityReceived;
						associatedTx.GrossQuantity = populatedAssociatedTx.GrossQuantity;
						associatedTx.Excise = populatedAssociatedTx.Excise;
						associatedTx.GST = populatedAssociatedTx.GST;
						associatedTx.Markup = populatedAssociatedTx.Markup;
						associatedTx.TotalPriceWithTax = populatedAssociatedTx.TotalPriceWithTax;
						associatedTx.TotalValue = associatedTx.TotalPriceWithTax - populatedAssociatedTx.GST;
					}
				}
			}
		}

		protected DateTime GetCurrentInventoryDate()
		{
			var inventoryDateSR = new InventoryDateSR { Security = this.security, CurrentSiteGuid = this.security.SiteGuid };

			InventoryDateDO inventoryDateDO =
				FMChannelHelper.MakeCall<IInventoryDateProcessor, InventoryDateDO>(x => x.Process(inventoryDateSR));

			return inventoryDateDO.InventoryDate;
		}

		protected void GetItemIndices(DataGridItem item, out int lineItemIndex, out int sublineItemIndex)
		{
			char[] separatorList = { '.' };
			string[] stringList = item.ID.Split(separatorList);
			lineItemIndex = int.Parse(stringList[0]);
			sublineItemIndex = int.Parse(stringList[1]);
		}

		protected string GetSpecificEquipmentID(string lineItemEq)
		{
			if (!string.IsNullOrEmpty(lineItemEq))
			{
				return lineItemEq;
			}
			{
				return null;
			}
		}

		protected void HandleFieldError(Exception e)
		{
			string message = e.Message;

			if (message.StartsWith("RT_ERR_001"))
			{
				int colonIdx = message.IndexOf(":", StringComparison.Ordinal);
				string fieldName = message.Substring(colonIdx + 1);

				message = GetDataDictionaryValueByKey(this.AccountingSite.CurrentSiteGuid, "The following fields are required:");
				message = message + " " + fieldName + "!";
			}

			if (message.StartsWith("RT_ERR_002"))
			{
				message = GetDataDictionaryValueByKey(this.AccountingSite.CurrentSiteGuid, "Must select product") + "!";
			}

			var fmLogger = new Logger("Accounting");
			fmLogger.Error("TransactionDetail - " + message);

			string alertString = "<script type=\"text/javascript\">\r\n<!--\r\n";

			if (this.CustomScriptName.Length > 0)
			{
				alertString += "TxDetailOnload();\r\n";
			}
			// Strip detailed debug exception info from message displayed to user
			if (message.StartsWith(TransactionFieldGenerator.RetrieveExceptionPrefix))
			{
				int messageStart = message.IndexOf(TransactionFieldGenerator.RetrieveExceptionDelimiter, StringComparison.Ordinal) +
																TransactionFieldGenerator.RetrieveExceptionDelimiter.Length;
				message = message.Substring(messageStart);
			}

			alertString += "alert(" + HttpUtility.JavaScriptStringEncode(message, true) + ");" + "\r\n--></script>";
			ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "FieldError", alertString, false);
			this.UpdatePanel1.Update();
			this.Session.Add("TransactionDetailFieldError", true);
		}

		/// <summary>
		/// Gets the initial product name for a new transaction data object
		/// </summary>
		protected virtual string GetInitialProductName()
		{
			return this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] as string;
		}

		/// <summary>
		///	This method will initialize a new transaction data object and set it to its initial
		///	state.
		/// </summary>
		protected virtual void InitTransaction()
		{
			TransactionAliasClass alias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
													x => x.Get(this.security, x.GetIdentityGuid(this.security, this.transAlias), true));

			this.AliasObject = alias;

			switch (alias.TransTypeID)
			{
				case TransactionTypes.T11_ConsumerTransfer:
					if (this.Trans == null)
					{
						this.Trans = new ConsumerTransferDO();
					}
					else
					{
						this.Trans.init();
					}

					break;

				case TransactionTypes.T13_OwnerTransfer:
					if (this.Trans == null)
					{
						this.Trans = new OwnerTransferDO();
					}
					else
					{
						this.Trans.init();
					}

					break;

				case TransactionTypes.T15_PrimaryRegrade:
				case TransactionTypes.T16_SecondaryRegrade:
					if (this.Trans == null)
					{
						this.Trans = new RegradeDO();
					}
					else
					{
						this.Trans.init();
					}

					break;
				case TransactionTypes.T23_StorageTransfer:
					if (this.Trans == null)
					{
						this.Trans = new StorageTransferDO();
					}
					else
					{
						this.Trans.init();
					}

					break;
				default:
					if (this.Trans == null)
					{
						this.Trans = new TransactionDO();
					}
					else
					{
						this.Trans.init();
					}

					break;
			}

			this.Trans.TransID = FuelsManagerId.NewId();
			this.Trans.Site = this.AccountingSite.CurrentSiteName;
			this.Trans.SiteGuid = this.AccountingSite.CurrentSiteGuid;

			this.Trans.Alias = this.transAlias;
			this.Trans.TransactionAliasGuid = alias.MasterRecordGuid;
			this.Trans.TransTypeID = alias.TransTypeID;
			this.Trans.SubmittedToAccounting = true;
			this.Trans.OriginApplication = TransactionOrigin.Accounting;

			this.Trans.InventoryDate = this.GetCurrentInventoryDate();
			DateTimeOffset siteTimeNow = TimeConverter.Now(this.AccountingSite.CurrentSite);
			this.Trans.TransactionDateTime = siteTimeNow;
			this.Trans.UpdatedBy = this.security.UserID;
			this.Trans.UpdatedDate = siteTimeNow;
			this.Trans.Date03 = siteTimeNow;

			this.Trans.ShipmentNumber = string.Empty;

			if (this.Trans.TransTypeID == TransactionTypes.T17_Order)
			{
				this.Trans.Status = TransactionStatus.Requested;

				// If the AutoComplete field is included, default it to true
				foreach (var fieldClass in alias.TransactionFieldCollection)
				{
					var field = (TransactionAliasFieldClass)fieldClass;
					if (field.ID.ToUpper() == "AUTOCOMPLETE")
					{
						this.Trans.AutoComplete = true;
						break;
					}
				}
			}

			if (this.Request.GetQueryOrFormValue("Manager") != null)
			{
				this.Trans.ManagerID = this.Request.GetQueryOrFormValue("Manager");
				CompanyClass manager =
					FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
						x => x.Get(this.security, x.GetIdentityGuid(this.security, this.Trans.ManagerID), false));

				if (manager.MasterRecordGuid != Guid.Empty)
				{
					this.Trans.ManagerCode = manager.Code;
					this.Trans.ManagerCompanyGuid = manager.MasterRecordGuid;
				}
			}

			else
			{
				CompanyCollectionClass managers =
					FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
						x => x.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(this.security, new[] { COMPANY_ROLE.MANAGER }));

				if (managers.Count == 1)
				{
					this.Trans.ManagerID = managers[0].ID;
					this.Trans.ManagerCode = managers[0].Code;
					this.Trans.ManagerCompanyGuid = managers[0].MasterRecordGuid;
				}
			}

			if (this.Trans.TransTypeID != TransactionTypes.T14_PhysicalInventory)
			{
				if (this.Request.GetQueryOrFormValue("Owner") != null)
				{
					this.Trans.OwnerID = this.Request.GetQueryOrFormValue("Owner");
					CompanyClass owner =
						FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
							x => x.Get(this.security, x.GetIdentityGuid(this.security, this.Trans.OwnerID), false));

					if (owner.MasterRecordGuid != Guid.Empty)
					{
						this.Trans.OwnerCode = owner.Code;
						this.Trans.OwnerCompanyGuid = owner.MasterRecordGuid;
					}
				}
				else
				{
					CompanyCollectionClass owners =
						FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
							x => x.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(this.security, new[] { COMPANY_ROLE.OWNER }));
					if (owners.Count == 1)
					{
						this.Trans.OwnerID = owners[0].ID;
						this.Trans.OwnerCode = owners[0].Code;
						this.Trans.OwnerCompanyGuid = owners[0].MasterRecordGuid;
					}
				}
			}

			if (this.Request.GetQueryOrFormValue("InventoryDate") != null)
			{
				this.Trans.InventoryDate =
					DateTime.Parse(this.AccountingSite.UnformatDate(this.Request.GetQueryOrFormValue("InventoryDate")));
			}

			var unitsHelper = new UnitsHelperClass(this.security, this.AccountingSite.CurrentSite, alias, null);
			unitsHelper.SetUnits(this.Trans, 0);

			if ((alias.MultipleLineItems == false) && (alias.LineItemFieldCollection.Count != 0))
			{
				LineItemDO lineItem;
				switch (this.Trans.TransTypeID)
				{
					// When multiple line item is false we must create a Regrade Line Item DO instead
					// of a line item DO.  
					case TransactionTypes.T15_PrimaryRegrade:
					case TransactionTypes.T16_SecondaryRegrade:
						var regradeLineItemDO = new RegradeLineItemDO();
						unitsHelper.SetUnits(regradeLineItemDO, ProductType.MaxProduct, null);
						this.Trans.LineItems.Add(regradeLineItemDO);
						break;

					case TransactionTypes.T23_StorageTransfer:
					case TransactionTypes.T13_OwnerTransfer:
						{
							var transferLineItemDO = new StorageTransferLineItemDO();
							unitsHelper.SetUnits(transferLineItemDO, ProductType.MaxProduct, null);
							this.Trans.LineItems.Add(transferLineItemDO);
							break;
						}

					default:
						lineItem = new LineItemDO();
						unitsHelper.SetUnits(lineItem, ProductType.MaxProduct, null);
						this.Trans.LineItems.Add(lineItem);
						break;
				}

				lineItem = (this.Trans.LineItems[0]);
				lineItem.Quantity.NullableGross = null;
				lineItem.Quantity.NullableNet = null;

				string productName = this.Request.GetQueryOrFormValue("Product");

				if (string.IsNullOrEmpty(productName))
				{
					productName = this.GetInitialProductName();
				}

				if (!string.IsNullOrEmpty(productName))
				{
					ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
						x => x.GetByID(this.security, productName));

					unitsHelper.Product = product;

					if (product.IdentityGuid != Guid.Empty)
					{
						lineItem.Product = productName;
						lineItem.ProductCode = product.Code;
						lineItem.ProductType = ProductClass.ProductTypeID(product.ProductType);
						lineItem.ProductGuid = product.MasterRecordGuid;
						lineItem.MassPackageSize = product._MassPackageSize.Value;
						lineItem.VolumePackageSize = product._VolumePackageSize.Value;
					}

					unitsHelper.SetUnits(lineItem, product.ProductType, product);
				}
			}

			// Set the default status if one is indicated in the Transaction Alias
			if (alias.LookupDefaultStatusIndex != -1)
			{
				this.Trans.Status = (TransactionStatus)alias.LookupDefaultStatusIndex;

				foreach (LineItemDO line in this.Trans.LineItems)
				{
					line.Status = (TransactionStatus)alias.LookupDefaultStatusIndex;
				}
			}
		}

		/// <summary>
		/// This method handles the cancel line item event. If the line item is a new line item
		/// and has not been saved, then the cancel event will remove the line item.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected virtual void LineItemDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			int lineItemIndex;
			int sublineItemIndex;
			this.GetItemIndices(e.Item, out lineItemIndex, out sublineItemIndex);

			if (this.Session[SessionLineItemAdded] != null)
			{
				var newItemIndex = (int)this.Session[SessionLineItemAdded];

				if (newItemIndex == e.Item.ItemIndex)
				{
					this.Session.Remove(SessionLineItemAdded);
					this.Session.Remove(SessionSublineItemObject);

					if (sublineItemIndex > -1)
					{
						LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];
						lineItem.SubLineItems.RemoveAt(sublineItemIndex);
					}
					else
					{
						this.Trans.LineItems.RemoveAt(lineItemIndex);
					}
				}
			}
			else if (this.TransContext.aliasClass.AssociatedAliases.Count > 0)
			{
				// Reset associated transactions back to the original list that was before the edit.
				var lineItem = this.Session[SessionLineItemObject] as LineItemDO;
				var associatedTransactionsBeforeEdit = this.Session["associatedTransactionsBeforeEdit"] as BaseCollections;

				if (associatedTransactionsBeforeEdit != null)
				{
					if (lineItem != null)
					{
						lineItem.AssociatedTransactions.Clear();

						foreach (AssociatedTxDO atx in associatedTransactionsBeforeEdit)
						{
							lineItem.AssociatedTransactions.Add(atx);
						}
					}

					this.AggregateAssociatedTxValues(lineItemIndex, false);
				}
			}
			else if (this.Session[SessionSublineItemObject] != null)
			{
				// Remove newly added sub line item from the grid.
				if (sublineItemIndex > -1)
				{
					LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];
					lineItem.SubLineItems.RemoveAt(sublineItemIndex);
				}

				this.Session.Remove(SessionSublineItemObject);
			}

			this.LineItemDataGrid.EditItemIndex = -1;
			this.LineItemDataGrid.SelectedIndex = -1;

			this.EnableFieldTable(true, false);
			this.NewLineItemButton.Enabled = true;

			if (this.NewAGRButton.Visible)
			{
				this.NewAGRButton.Enabled = true;
			}

			if (this.NewTransportButton.Visible)
			{
				this.NewTransportButton.Enabled = true;
			}

			// Reset line item to its original value before the grid row edit
			var originalLineItem = this.Session[SessionLineItemObject] as LineItemDO;
			if (originalLineItem != null && this.Trans.LineItems.Count > lineItemIndex)
			{
				this.Trans.LineItems[lineItemIndex] = originalLineItem;
			}
			this.LineItemGridGenerator.Bind();

			// If this is an alias with associated aliases disable the associate transaction link button
			if (this.TransContext.aliasClass.AssociatedAliases.Count > 0)
			{
				// Get the associate control
				var button = (FMViewAssociatedTxLinkButton)e.Item.FindControl("FMViewAssociatedTxLinkButton1");

				if (button != null)
				{
					button.Enabled = true;
				}
			}

			this.Session.Remove("associatedTransactionsBeforeEdit");
			this.Session.Remove(SessionLineItemObject);

			// Set the buttons back to the previous settings prior to the line
			// item edit.
			this.SetButtons();
		}

		/// <summary>
		/// The line item data grid delete command.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected virtual void LineItemDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			int lineItemIndex;
			int sublineItemIndex;

			this.GetItemIndices(e.Item, out lineItemIndex, out sublineItemIndex);
			if (sublineItemIndex > -1)
			{
            LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];
				lineItem.SubLineItems.RemoveAt(sublineItemIndex);
            this.UpdateLineItemVolumeAndQuantity(e.Item);
         }
         else
			{
				this.Trans.LineItems.RemoveAt(lineItemIndex);
			}

			if (e.Item.ItemIndex == this.LineItemDataGrid.EditItemIndex)
			{
				this.Session.Remove(SessionLineItemObject);
				this.Session.Remove(SessionSublineItemObject);
				this.LineItemDataGrid.EditItemIndex = -1;
				this.EnableFieldTable(true, false);
				this.NewLineItemButton.Enabled = true;

				if (this.NewAGRButton.Visible)
				{
					this.NewAGRButton.Enabled = true;
				}

				if (this.NewTransportButton.Visible)
				{
					this.NewTransportButton.Enabled = true;
				}
			}

         this.LineItemGridGenerator.Bind();
         this.LineItemDataGrid.SelectedIndex = -1;

         this.SetButtons();

			// Update the header's virtual fields
			this.PopulateHeaderVirtualFields();
		}

		protected virtual void LineItemDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.RetrieveDataFromPage();
			this.LineItemDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.LineItemDataGrid.SelectedIndex = e.Item.ItemIndex;
			this.LineItemGridGenerator.Bind();
			this.EnableFieldTable(false, false);

			// Get the Current LineItem for use by various Select Forms
			int index = 0;
			int lineItemIndex = 0;

			while (true)
			{
				LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];
				if (index == e.Item.DataSetIndex)
				{
					this.Session[SessionLineItemObject] = lineItem;
					break;
				}

				index++;
				lineItemIndex++;

				if (index + lineItem.SubLineItems.Count > e.Item.DataSetIndex)
				{
					break;
				}

				index += lineItem.SubLineItems.Count;
			}

			// If this alias has other associated aliases disable the view associated trans button
			if (this.TransContext.aliasClass.AssociatedAliases.Count > 0)
			{
				var button = (FMViewAssociatedTxLinkButton)e.Item.FindControl("FMViewAssociatedTxLinkButton2");

				if (button != null)
				{
					button.Enabled = false;
				}
			}
			else
			{
				// We want the Add Associated Transaction button to be disabled during edit
				var addButton = e.Item.FindControl("lbAddAssociatedTx2") as LinkButton;

				if (addButton != null)
				{
					addButton.Enabled = false;
				}
			}

			this.Session.Remove("associatedTransactionsBeforeEdit");

			// Disable all buttons for line item edit processing.
			this.DisableButtonsForEditing();
		}

		protected virtual void LineItemDataGridItemCommand(object source, DataGridCommandEventArgs e)
		{
			this.Logger.Debug("TransactionDetail.LineItemDataGrid_ItemCommand(" + e.CommandName + ")");

			// Get the current line item
			int lineItemIndex;
			int sublineItemIndex;
			this.GetItemIndices(e.Item, out lineItemIndex, out sublineItemIndex);
			LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];

			// Is this the View link?
			if (e.CommandName == "TxViewBtn")
			{
				// Redirect to a transaction listing
				this.TransferToViewing(lineItem);
				return;
			}

			// Is this the Add link?
			if (e.CommandName == "TxAddBtn")
			{
				if (this.IsTransactionEditable)
				{
					this.TransferToNewTx(lineItem, source, lineItemIndex);
				}
				else
				{
					this.ErrorHandler(new Exception("Transaction not editable."));
				}

				return;
			}

			// Now make sure we don't process anything but the AddSubLineItem button from here on in
			if (e.CommandName != "AddSubLineItem")
			{
				return;
			}

			if (this.CheckEditable() == false)
			{
				return;
			}

			// Do our default processing
			var subLineItem = new SubLineItemDO();
			lineItem.SubLineItems.Add(subLineItem);
			this.Session[SessionLineItemObject] = lineItem;
			this.Session[SessionSublineItemObject] = subLineItem;

			//Make the new last sub line item of the line item editable.
			this.LineItemDataGrid.EditItemIndex = e.Item.ItemIndex + lineItem.SubLineItems.Count;
			this.LineItemGridGenerator.Bind();

			this.EnableFieldTable(false, false);
			this.DisableButtonsForEditing();
		}

		protected virtual void LineItemDataGridSelectedIndexChanged(object sender, EventArgs e)
		{
			var dataGridItemEventArgs = e as DataGridItemEventArgs;

			if (dataGridItemEventArgs != null)
			{
				this.LineItemDataGrid.SelectedIndex = dataGridItemEventArgs.Item.ItemIndex;
				this.LineItemGridGenerator.Bind();
			}
		}

		/// <summary>
		///	This method will handle the line item update event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected virtual void LineItemDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			string errorMsg = this.ValidateCurrencyFields(e.Item);

			if (errorMsg != null)
			{
				this.HandleFieldError(new RetrieveException(errorMsg));
			}
			else
			{
				bool bNoLineItemErrors = this.RetrieveLineItem(e.Item);

				if (bNoLineItemErrors)
				{
					this.LineItemDataGrid.EditItemIndex = -1;
					this.LineItemGridGenerator.Bind();

					this.EnableFieldTable(true, false);
					this.NewLineItemButton.Enabled = true;

					if (this.NewAGRButton.Visible)
					{
						this.NewAGRButton.Enabled = true;
					}

					if (this.NewTransportButton.Visible)
					{
						this.NewTransportButton.Enabled = true;
					}

					// update the header's virtual fields
					this.PopulateHeaderVirtualFields();

					// If the transaction alias has other aliases associated disable the select
					// transactions button
					if (this.TransContext.aliasClass.AssociatedAliases.Count > 0)
					{
						var button = (FMViewAssociatedTxLinkButton)e.Item.FindControl("FMViewAssociatedTxLinkButton1");

						if (button != null)
						{
							button.Enabled = true;
						}
					}

					if (this.Session[SessionLineItemAdded] != null)
					{
						var newItemIndex = (int)this.Session[SessionLineItemAdded];

						if (newItemIndex == e.Item.ItemIndex)
						{
							this.Session.Remove(SessionLineItemAdded);
							this.Session.Remove(SessionLineItemObject);
							this.Session.Remove(SessionSublineItemObject);
						}
					}
				}
			}

			this.Session.Remove("associatedTransactionsBeforeEdit");
			this.Session.Remove(TransactionDetailBase.SessionLineItemObject);

			// Set the buttons back to the previous settings prior to the line
			// item edit.
			this.SetButtons();
		}

		/// <summary>
		/// The line-item item data bound.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected virtual void LineItemItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				return;
			}

			this.SetLineItemDeleteAndEditButtonState(e);
			this.SetAddSubLineItemButtonState(e);

			// Only Orders have Add & View buttons
			if ((this.Trans.TransTypeID != TransactionTypes.T17_Order)
				&& (this.Trans.TransTypeID != TransactionTypes.T18_SupplyOrder)
				&& (this.TransContext.aliasClass.MultipleLineItems == false
					|| this.TransContext.aliasClass.AssociatedAliases.Count == 0))
			{
				return;
			}

			// Requirements changed to allow one or more transaction aliases to be associated
			// with another transaction alias.  This change occured after the associate invoice
			// transaction functionality was completed.  For now the associate invoice functionality
			// will be kept.
			// Show or hide the add transaction button based on whether or not
			// the trans type is an invoice
			var addTransactionNonEditMode = e.Item.FindControl("lbAddAssociatedTx1") as FMAddAssociatedTxLinkButton;
			var addTransactionEditMode = e.Item.FindControl("lbAddAssociatedTx2") as FMAddAssociatedTxLinkButton;

			var assocTxElipseBtnEditMode = e.Item.FindControl("btnAddAssocTx2") as FMElipseButton;

			if (this.TransContext.aliasClass.AssociatedTransactionAliasGuid != Guid.Empty)
			{
				// This transaction alias allows transactions to be associated
				if (assocTxElipseBtnEditMode != null)
				{
					assocTxElipseBtnEditMode.Visible = true;
				}

				// We only hide these for an invoice transaction type
				if (this.TransContext.aliasClass.TransTypeID == TransactionTypes.T21_AccountPayableInvoice ||
					 this.TransContext.aliasClass.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
				{
					if (addTransactionNonEditMode != null)
					{
						addTransactionNonEditMode.Visible = false;
					}

					if (addTransactionEditMode != null)
					{
						addTransactionEditMode.Visible = false;
					}
				}

				bool aggregate = this.TransContext.aliasClass.AggregateAssociatedTransactions;

				// Populate the ellipse button's data1 and data 2 properties with the line
				// items associated transactions
				// the item might be a sub-line item, in which case we aren't interested in it.
				if (e.Item.DataItem is LineItemDO)
				{
					// e.Item.ItemIndex is zero based and includes sub line items, so if there are two line items and the first has a sub line item 
					// and this fires for the second line item e.Item.ItemIndex will be 2, when really we want it to be 1.
					// use the GetItemIndices method to get the correct value.

					this.GetItemIndices(e.Item, out int lineItemIndex, out _);

					if (this.Trans.LineItems.Count > lineItemIndex)
					{
						LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];

						foreach (AssociatedTxDO associatedTxDo in lineItem.AssociatedTransactions)
						{
							var assocLineItem = associatedTxDo;

							if (assocTxElipseBtnEditMode != null)
							{
								assocTxElipseBtnEditMode.Data += assocLineItem.TransID + "|";
								assocTxElipseBtnEditMode.Data2 += assocLineItem.TransactionLineItemGuid + "|";
							}
						}

						if (assocTxElipseBtnEditMode != null)
						{
							assocTxElipseBtnEditMode.OnClick = "AssociateTx(" + e.Item.ItemIndex + ", "
																		+ aggregate.ToString().ToLower() + ")";
						}
					}
				}
			}
			else
			{
				if (assocTxElipseBtnEditMode != null)
				{
					assocTxElipseBtnEditMode.Visible = false;
				}

				if (addTransactionNonEditMode != null)
				{
					addTransactionNonEditMode.Visible = true;
				}

				if (addTransactionEditMode != null)
				{
					addTransactionEditMode.Visible = true;
				}
			}

			this.SetAddTxButtonState(e);
			this.SetViewTxButtonState(e);
		}

		/// <summary>
		///	This method loads a transaction based on the transaction ID. It will return
		///	a TransactionDO object if successful.
		/// </summary>
		/// <param name="transID"></param>
		/// <returns></returns>
		protected virtual TransactionDO LoadTransaction(string transID)
		{
			var sr = new TransactionSR { Security = this.security, TransID = transID, AllowCrossSiteTransactions = this.AllowCrossSiteTranactions };

			try
			{
				var timer3 = new StopWatch(StopWatch.Appnames.Accounting, "Checking Terminal Automation Service Status");
				var serviceController = new ServiceController("FuelsManager Terminal Automation");

				if (!this.WasServiceFound(serviceController))
				{
					serviceController = null;
				}

				timer3.Stop();

				// vthompson 10-27-2008
				// Changed this to only call the load rack if the site is not a site group
				if (this.AccountingSite.CurrentSite.SiteGroup
					|| serviceController == null
					|| serviceController.Status != ServiceControllerStatus.Running)
				{
					sr.AccountingSite = this.AccountingSite;

					TransactionDO localTrans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));

					this.bTransIDBeingLoaded = false;
					return localTrans;
				}

				if (UsingLoadRack)
				{
					ILoadRackManager loadRackManager = this.GetLoadRackManager();
					var timer = new StopWatch(StopWatch.Appnames.Accounting, "Actual Load Rack load call to BLL");
					TransactionDO localTrans = loadRackManager.AccountingRequest(sr);
					timer.Stop();

					this.bTransIDBeingLoaded = true;
					return localTrans;
				}
				// alternateLoad
			}
			catch (SocketException)
			{
				// Originally the exception message was checked to determine if connectivity to the load rack service
				// failed.  The message changed in .NET 2.0 so this design was changed.
				// bschaal 1/07/09
				// the above comment is incorrect. The change made here prevents the ability to edit a bol when the loadrack service is running.
				// .NET 2.0 has nothing to do with this. This code has been changed back

				// alternateLoad
			}
			catch (Exception except)
			{
				if (!except.Message.Contains("No connection could be made because the target machine actively refused it")
					&& !except.Message.Contains("Not Loading") && !except.Message.Contains("Requested Service not found"))
				{
					throw;
				}

				sr.AccountingSite = this.AccountingSite;

				TransactionDO localTrans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));

				this.bTransIDBeingLoaded = false;
				return localTrans;
			}
			// alternateLoad
			sr.AccountingSite = this.AccountingSite;
			var timer2 = new StopWatch(StopWatch.Appnames.Accounting, "Actual load call to BLL");

			TransactionDO trans2 = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));

			timer2.Stop();
			this.bTransIDBeingLoaded = false;
			return trans2;
		}

		/// <summary>
		///	This method handles the processing for the new button being pressed.
		/// </summary>
		protected void NewButtonProcess()
		{
			this.Session.Remove(TransactionDetailList.TransactionDetailListKey);
			this.Session.Remove("AssociatedTxContext");
			this.Session.Remove("allAssociatedTransactionsBeforeTransactionEdit");

			//clear the grids
			this.Trans.LineItems.Clear();

			if (this.LineItemGridGenerator != null)
			{
				this.LineItemGridGenerator.ClearTransLineItems();
				this.LineItemGridGenerator.Bind();
			}

			this.Trans.WeightReadings.Clear();

			if (this.AgrGridGenerator != null)
			{
				this.AgrGridGenerator.ClearTransWeightReadings();
				this.AgrGridGenerator.Bind();
			}

			this.Trans.TransportInfoList.Clear();

			this.TransportInfoGridGenerator?.Bind();

			//Create new transaction.
			this.InitTransaction();
			this.TransContext.mode = TransactionContext.Mode.Add;

			this.Session[ModeKey] = this.TransContext.mode;
			this.Session[TransKey] = this.Trans;

			this.SetButtons();
			this.RegenerateControls(true);
		}

		/// <summary>
		///	This method handles the processing for the new button being pressed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void NewButtonClick(object sender, EventArgs e)
		{
			this.NoSaveErrors = true;

			if (this.IsTransactionEditable && !this.RetrieveDataFromPage() || !this.Save())
			{
				return;
			}

			this.Request.Cookies.Remove("ActiveElement");
			this.NewButtonProcess();
		}

		/// <summary>
		///	This method handles the new line item Add button. It will create a new
		///	line item in the grid along with a LineItemDO object.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void NewLineItemButtonClick(object sender, EventArgs e)
		{
			if (this.RetrieveDataFromPage() == false)
			{
				return;
			}

			LineItemDO lineItem;
			if ((this.Trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade)
				|| (this.Trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade))
			{
				lineItem = new RegradeLineItemDO();
			}
			else if (this.Trans.TransTypeID == TransactionTypes.T23_StorageTransfer ||
						this.Trans.TransTypeID == TransactionTypes.T13_OwnerTransfer)
			{
				lineItem = new StorageTransferLineItemDO();
			}
			else
			{
				lineItem = new LineItemDO();
			}

			lineItem.Quantity.NullableGross = null;
			lineItem.Quantity.NullableNet = null;
			lineItem.Quantity.NullableMass = null;
			lineItem.Quantity.NullablePackage = null;

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
											x => x.Get(this.security, this.Trans.SiteGuid, false, false, false));

			// Initialize the line item units.
			var unitsHelper = new UnitsHelperClass(this.security, site, this.AliasObject, product: null);
			unitsHelper.SetUnits(lineItem, ProductType.MaxProduct, null);

			if (!string.IsNullOrEmpty(this.OrderReferenceID))
			{
				// Set the product
				lineItem.Product = this.OrderProduct;
				lineItem.ProductCode = this.OrderProductCode;
				lineItem.ProductGuid = this.OrderProductGuid;

				if (this.OrderProductGuid != Guid.Empty)
				{
					ProductClass orderProduct =
						FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(this.security, this.OrderProductGuid));
					unitsHelper.SetUnits(lineItem, orderProduct.ProductType, orderProduct);
				}
			}

			if ((this.Trans.TransTypeID == TransactionTypes.T17_Order)
				|| (this.Trans.TransTypeID == TransactionTypes.T18_SupplyOrder))
			{
				lineItem.Status = this.Trans.Status;
			}
			else
			{
				if (this.TransContext != null && this.TransContext.DefaultStatus != -1)
				{
					lineItem.Status = (TransactionStatus)this.TransContext.DefaultStatus;
				}
			}

			// For an AR invoice auto-populate the line item's invoice number
			if (this.Trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				lineItem.InvoiceNumber = FMChannelHelper.MakeCall<ISites, string>(x => x.GetNextInvoiceNumber(this.security));
			}

			this.Session[SessionLineItemObject] = lineItem;

			this.Trans.LineItems.Add(lineItem);

			this.LineItemDataGrid.SelectedIndex = this.LineItemDataGrid.Items.Count;
			this.LineItemDataGrid.EditItemIndex = this.LineItemDataGrid.Items.Count;
			this.LineItemGridGenerator.Bind();
			this.EnableFieldTable(false, false);

			// Disable all buttons for line item edit processing.
			this.DisableButtonsForEditing();

			this.Session.Add(SessionLineItemAdded, this.LineItemDataGrid.EditItemIndex);
		}

		protected virtual void NextButtonClick(object sender, EventArgs e)
		{
			var list = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList;
			if (list != null)
			{
				list.CurrentIndex++;
			}

			// Read the TransactionDetail URL from the Web.config file (08-Jul-2009 IGO)
			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];
			string url = "../" + transactionDetailUrl + "?" + AliasKey + "=" + this.Trans.Alias;

			this.Redirect(url);
			this.Context.ApplicationInstance.CompleteRequest();
		}

		protected override void OnInitComplete(EventArgs e)
		{
			base.OnInitComplete(e);
		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}
		protected override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
		}
		protected override void LoadControlState(object savedState)
		{
			base.LoadControlState(savedState);
		}
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
		}
		protected override object LoadPageStateFromPersistenceMedium()
		{
			return base.LoadPageStateFromPersistenceMedium();
		}

		protected override void OnInit(EventArgs e)
		{
			if (this.Session["Security"] == null)
			{
				this.ErrorHandler(new FMSessionInvalidException());
			}

			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			this.LineItemDataGrid.ItemDataBound += this.LineItemItemDataBound;
			this.GaugeReadingsDataGrid.ItemDataBound += this.GaugeReadingsItemDataBound;
			this.TransportDataGrid.ItemDataBound += this.TransportItemDataBound;

			// Retrieve and store the custom client script name.
			this.GetCustomClientScriptName();

			base.OnInit(e);
			this.IsAdfKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey());
			this.Logger.Debug("TransactionDetail.OnInit()");
		}

		protected override void OnSaveStateComplete(EventArgs e)
		{
			base.OnSaveStateComplete(e);
		}
		protected override void OnPreRenderComplete(EventArgs e)
		{
			base.OnPreRenderComplete(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			// fix the css links for app-rooted paths
            cssFuelsManager.Href = ResolveUrl("~/CSS/FuelsManager.css");
            cssJQueryUi.Href = ResolveUrl("~/Javascripts/jquery-ui-1.10.3.custom/css/ui-lightness/jquery-ui-1.10.3.custom.css");
            cssDispatchUi.Href = ResolveUrl("~/DispatchWebApp/css/jquery-ui-1.8.17.custom.css");

			// fix the script links for app-rooted paths
            RegisterHeaderScript("~/Javascripts/TransactionDetail_min.js");
            RegisterHeaderScript("~/Javascripts/autocomplete.js");
            RegisterHeaderScript("~/Javascripts/jquery-ui-1.10.3.custom/js/jquery-1.9.1.js");
            RegisterHeaderScript("~/Javascripts/jquery-ui-1.10.3.custom/js/jquery-ui-1.10.3.custom.js");
            RegisterHeaderScript("~/Javascripts/TransactionDetail2.js");
            RegisterHeaderScript("~/Javascripts/modalpopup.js");
            RegisterHeaderScript("~/Javascripts/CSRFToken.js");
            RegisterHeaderScript("~/Javascripts/json2.js");
        }

        protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
		}

		protected override void OnUnload(EventArgs e)
		{
			base.OnUnload(e);

			this.Logger.Debug("TransactionDetail.Unload()");
			TimeSpan span = DateTimeOffset.Now - this.StartTime;
			this.Logger.Debug("TransactionDetail completed in " + span.TotalMilliseconds + " ms.");
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.Response.Buffer = true;

				if (!this.IsPostBack)
				{
					this.Session.Remove("TransactionDetail.AccountingSite");
					this.Session.Remove("TransactionDetail.TransactionContext");
				}

				this.Initialize();
				if (this.security == null)
				{
					throw new FMSessionInvalidException();
				}


				// Are we coming from the Query Writer?
				if (this.Request.GetQueryOrFormValue("QueryEdit").DefaultIfNull(string.Empty).Equals(string.Empty) == false)
				{
					string transID = this.Request.GetQueryOrFormValue("QueryEdit");

					string modeValue = "Mode=" + ((this.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)) ? "Edit" : "View");

					this.Redirect("transactiondetail.aspx?" + modeValue + "&TransID=" + transID + "&QueryEditItem=True");
					this.Context.ApplicationInstance.CompleteRequest();
				}
				else
				{
					if (this.HasViewRights())
					{
						// Check and prepare if this is an Order associated transaction
						this.PrepForAssociatedOrderTxIfNecessary();

						var timer = new StopWatch(StopWatch.Appnames.Accounting, "RegeneratedControls()");
						this.RegenerateControls(false);
						timer.Stop();

						if (this.AliasObject != null && this.AliasObject.UseTransactionDetailWithLayout)
						{
							var newUrl = this.Request.RawUrl.Replace("TransactionDetail.aspx", "TransactionDetailV2.aspx");
							this.Context.Response.Redirect(newUrl);
							return;
						}

					}
					else
					{
						var ex = new FMInsufficientRightsException();
						this.ErrorHandler(ex);
						this.Context.ApplicationInstance.CompleteRequest();
					}
				}
			}
			catch (FMSessionInvalidException ex)
			{
				this.ErrorHandler(ex);
			}
			catch (Exception except)
			{
				string msg = except.Message;

				// Ignore the exception that the thread is being aborted. It is in response
				// to the response being terminated.
				if (msg.StartsWith("Thread was being aborted.") == false)
				{
					this.ErrorHandler(except);
				}

				this.Context.ApplicationInstance.CompleteRequest();
			}
		}

		/// <summary>
		///	This method handles the loading of the transaction detail page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.LineItemGridGenerator?.Bind();

				this.AgrGridGenerator?.Bind();

				this.TransportInfoGridGenerator?.Bind();

				// Register the Transaction Detail java script file with the page.
				if (this.Page.ClientScript.IsClientScriptBlockRegistered("TransactionDetailJS") == false)
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						"TransactionDetailJS",
						"<SCRIPT type='text/JavaScript' src='../Javascripts/TransactionDetail_min.js'></SCRIPT>", false);
				}

				if ((this.Page.ClientScript.IsClientScriptBlockRegistered(CustomClientScriptsJs) == false)
					&& this.CustomScriptName.Length > 0)
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						CustomClientScriptsJs,
						"<SCRIPT type='text/JavaScript' src='" + this.CustomScriptName + "'></SCRIPT>", false);
					// Register the page onload event for custom scripts
					this.CreateAnOnloadPageEventForCustomScripts();
				}

				if (this.Page.IsPostBack == false)
				{
					this.Request.Cookies.Remove("ActiveElement");

					this.Session.Remove(CombineTransKey);

					this.Session.Remove(SessionGaugeReadingGridInAddMode);

					if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("RETURNING")))
					{
						this.Session.Remove(SessionLineItemObject);
						this.Session.Remove(SessionSublineItemObject);
					}

					//For a "Sale" type transaction, setup Tail Number select box's "onchange" event handler
					//that would populate billing information when user makes a selection.
					if (this.Trans != null && this.Trans.Alias.StartsWith("Sale"))
					{
						this.SetTailNumberControlCallBack(this.FieldTable);
					}
				}
				else
				{
					if (this.Request.GetQueryOrFormValue("__MYEVENTTARGET") == "TAIL_NUMBER_CHANGED"
						|| this.Request.GetQueryOrFormValue("__MYEVENTTARGET") == "TAIL_NUMBER_CHANGE_APPROVED")
					{
						this.PopulateBillingFromFuelCard(this.Request.GetQueryOrFormValue("__MYEVENTARGUMENT"));
						this.RefreshSpecialInstructions();
					}
					else if (this.Request.GetQueryOrFormValue("__MYEVENTTARGET") == "DELETE_CONFIRMATION")
					{
						string val = this.Request.GetQueryOrFormValue("__MYEVENTARGUMENT");
						if (val.ToUpper() == "OK")
						{
							this.Delete();
						}
					}
					else if (this.Request.GetQueryOrFormValue("__MYEVENTTARGET") == "COMBINE_TRANSACTION")
					{
						this.CombineTransaction(this.Request.GetQueryOrFormValue("__MYEVENTARGUMENT"));
					}
				}

				this.SetButtons();
			}
			catch (Exception except)
			{
				string msg = except.Message;

				// Ignore the exception that the thread is being aborted. It is in response
				// to the response being terminated.
				if (msg.StartsWith("Thread was being aborted.") == false)
				{
					this.ErrorHandler(except);
				}

				// Stop so we can see the error now; otherwise, there will most likely be
				// other errors as a result of this one and the user will only see the 
				// follow-up errors
				this.Context.ApplicationInstance.CompleteRequest();
			}
		}

		protected virtual void Page_PreRender(object sender, EventArgs e)
		{
			if ((this.LineItemDataGrid.EditItemIndex == -1) && (this.TransportDataGrid.EditItemIndex == -1)
				&& (this.GaugeReadingsDataGrid.EditItemIndex == -1))
			{
				this.EnableFieldTable(true, false);
			}

			//Write out client side scripts that declare java script variables representing
			//the currency fields.
			if (this.Session[LineItemCurrencyUnitFG.CLIENT_SIDE_SCRIPT_LINEITEM_CURRENCY_UNIT] != null)
			{
				if (this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemCurrencyUnitFG.CLIENT_SIDE_KEY_LINEITEM_CURRENCY_UNIT,
						this.Session[LineItemCurrencyUnitFG.CLIENT_SIDE_SCRIPT_LINEITEM_CURRENCY_UNIT] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemCurrencyUnitFG.CLIENT_SIDE_KEY_LINEITEM_CURRENCY_UNIT,
						this.Session[LineItemCurrencyUnitFG.CLIENT_SIDE_SCRIPT_LINEITEM_CURRENCY_UNIT] as string);
				}
			}

			if (this.Session[LineItemProductPriceFG.CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCTPRICE] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemProductPriceFG.CLIENT_SIDE_KEY_LINEITEM_PRODUCTPRICE,
						this.Session[LineItemProductPriceFG.CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCTPRICE] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemProductPriceFG.CLIENT_SIDE_KEY_LINEITEM_PRODUCTPRICE,
						this.Session[LineItemProductPriceFG.CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCTPRICE] as string);
				}
			}

			if (this.Session[LineItemNonDomesticPriceFG.CLIENT_SIDE_SCRIPT_LINEITEM_NONDOMESTIC_PRICE] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemNonDomesticPriceFG.CLIENT_SIDE_KEY_LINEITEM_NONDOMESTIC_PRICE,
						this.Session[LineItemNonDomesticPriceFG.CLIENT_SIDE_SCRIPT_LINEITEM_NONDOMESTIC_PRICE] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemNonDomesticPriceFG.CLIENT_SIDE_KEY_LINEITEM_NONDOMESTIC_PRICE,
						this.Session[LineItemNonDomesticPriceFG.CLIENT_SIDE_SCRIPT_LINEITEM_NONDOMESTIC_PRICE] as string);
				}
			}

			if (this.Session[LineItemExchangeRateFG.CLIENT_SIDE_SCRIPT_LINEITEM_EXCHANGE_RATE] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemExchangeRateFG.CLIENT_SIDE_KEY_LINEITEM_EXCHANGE_RATE,
						this.Session[LineItemExchangeRateFG.CLIENT_SIDE_SCRIPT_LINEITEM_EXCHANGE_RATE] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemExchangeRateFG.CLIENT_SIDE_KEY_LINEITEM_EXCHANGE_RATE,
						this.Session[LineItemExchangeRateFG.CLIENT_SIDE_SCRIPT_LINEITEM_EXCHANGE_RATE] as string);
				}
			}

			if (this.Session[LineItemTax1FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX1] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemTax1FG.CLIENT_SIDE_KEY_LINEITEM_TAX1,
						this.Session[LineItemTax1FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX1] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemTax1FG.CLIENT_SIDE_KEY_LINEITEM_TAX1,
						this.Session[LineItemTax1FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX1] as string);
				}
			}

			if (this.Session[LineItemTax2FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX2] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemTax2FG.CLIENT_SIDE_KEY_LINEITEM_TAX2,
						this.Session[LineItemTax2FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX2] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemTax2FG.CLIENT_SIDE_KEY_LINEITEM_TAX2,
						this.Session[LineItemTax2FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX2] as string);
				}
			}

			if (this.Session[LineItemTax3FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX3] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemTax3FG.CLIENT_SIDE_KEY_LINEITEM_TAX3,
						this.Session[LineItemTax3FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX3] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemTax3FG.CLIENT_SIDE_KEY_LINEITEM_TAX3,
						this.Session[LineItemTax3FG.CLIENT_SIDE_SCRIPT_LINEITEM_TAX3] as string);
				}
			}

			if (this.Session[LineItemFlag04FG.CLIENT_SIDE_SCRIPT_LINEITEM_FLAG04] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemFlag04FG.CLIENT_SIDE_KEY_LINEITEM_FLAG04,
						this.Session[LineItemFlag04FG.CLIENT_SIDE_SCRIPT_LINEITEM_FLAG04] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemFlag04FG.CLIENT_SIDE_KEY_LINEITEM_FLAG04,
						this.Session[LineItemFlag04FG.CLIENT_SIDE_SCRIPT_LINEITEM_FLAG04] as string);
				}
			}

			if (this.Session[LineItemFlag05FG.CLIENT_SIDE_SCRIPT_LINEITEM_FLAG05] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemFlag05FG.CLIENT_SIDE_KEY_LINEITEM_FLAG05,
						this.Session[LineItemFlag05FG.CLIENT_SIDE_SCRIPT_LINEITEM_FLAG05] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemFlag05FG.CLIENT_SIDE_KEY_LINEITEM_FLAG05,
						this.Session[LineItemFlag05FG.CLIENT_SIDE_SCRIPT_LINEITEM_FLAG05] as string);
				}
			}
			if (this.Session[LineItemAlternativeUnitsFG.CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEUNITS] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemAlternativeUnitsFG.CLIENT_SIDE_KEY_LINEITEM_ALTERNATIVEUNITS,
						this.Session[LineItemAlternativeUnitsFG.CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEUNITS] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemAlternativeUnitsFG.CLIENT_SIDE_KEY_LINEITEM_ALTERNATIVEUNITS,
						this.Session[LineItemAlternativeUnitsFG.CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEUNITS] as string);
				}
			}

			if (this.Session[LineItemGrossQuantityFG.CLIENT_SIDE_SCRIPT_LINEITEM_GROSSQUANTITY] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemGrossQuantityFG.CLIENT_SIDE_KEY_LINEITEM_GROSSQUANTITY,
						this.Session[LineItemGrossQuantityFG.CLIENT_SIDE_SCRIPT_LINEITEM_GROSSQUANTITY] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemGrossQuantityFG.CLIENT_SIDE_KEY_LINEITEM_GROSSQUANTITY,
						this.Session[LineItemGrossQuantityFG.CLIENT_SIDE_SCRIPT_LINEITEM_GROSSQUANTITY] as string);
				}
			}

			if (this.Session[LineItemNetQuantityFG.CLIENT_SIDE_SCRIPT_LINEITEM_NET_QUANTITY] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemNetQuantityFG.CLIENT_SIDE_KEY_LINEITEM_NET_QUANTITY,
						this.Session[LineItemNetQuantityFG.CLIENT_SIDE_SCRIPT_LINEITEM_NET_QUANTITY] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemNetQuantityFG.CLIENT_SIDE_KEY_LINEITEM_NET_QUANTITY,
						this.Session[LineItemNetQuantityFG.CLIENT_SIDE_SCRIPT_LINEITEM_NET_QUANTITY] as string);
				}
			}

			if (this.Session[LineItemNumber01FG.CLIENT_SIDE_SCRIPT_LINEITEM_NUMBER01] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemNumber01FG.CLIENT_SIDE_KEY_LINEITEM_NUMBER01,
						this.Session[LineItemNumber01FG.CLIENT_SIDE_SCRIPT_LINEITEM_NUMBER01] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemNumber01FG.CLIENT_SIDE_KEY_LINEITEM_NUMBER01,
						this.Session[LineItemNumber01FG.CLIENT_SIDE_SCRIPT_LINEITEM_NUMBER01] as string);
				}
			}

			if (this.Session[LineItemAlternativeGrossVolumeFG.CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEGROSSVOLUME] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemAlternativeGrossVolumeFG.CLIENT_SIDE_KEY_LINEITEM_ALTERNATIVEGROSSVOLUME,
						this.Session[LineItemAlternativeGrossVolumeFG.CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEGROSSVOLUME] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemAlternativeGrossVolumeFG.CLIENT_SIDE_KEY_LINEITEM_ALTERNATIVEGROSSVOLUME,
						this.Session[LineItemAlternativeGrossVolumeFG.CLIENT_SIDE_SCRIPT_LINEITEM_ALTERNATIVEGROSSVOLUME] as string);
				}
			}

			if (this.Session[LineItemTotalValueFG.CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_VALUE] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemTotalValueFG.CLIENT_SIDE_KEY_LINEITEM_TOTAL_VALUE,
						this.Session[LineItemTotalValueFG.CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_VALUE] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemTotalValueFG.CLIENT_SIDE_KEY_LINEITEM_TOTAL_VALUE,
						this.Session[LineItemTotalValueFG.CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_VALUE] as string);
				}
			}

			if (this.Session[LineItemTotalPriceWithTaxFG.CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_PRICE_TAX] != null)
			{
				if (this.TransContext != null && this.TransContext.aliasClass.MultipleLineItems)
				{
					ScriptManager.RegisterClientScriptBlock(
						this.Page,
						this.GetType(),
						LineItemTotalPriceWithTaxFG.CLIENT_SIDE_KEY_LINEITEM_TOTAL_PRICE_TAX,
						this.Session[LineItemTotalPriceWithTaxFG.CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_PRICE_TAX] as string,
						false);
				}
				else
				{
					this.ClientScript.RegisterStartupScript(
						this.GetType(),
						LineItemTotalPriceWithTaxFG.CLIENT_SIDE_KEY_LINEITEM_TOTAL_PRICE_TAX,
						this.Session[LineItemTotalPriceWithTaxFG.CLIENT_SIDE_SCRIPT_LINEITEM_TOTAL_PRICE_TAX] as string);
				}
			}

			if (this.Session[LineItemLoadingLocationFG.ClientSideScriptLineitemLoadinglocationFG] != null)
			{
				ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(),
																	LineItemLoadingLocationFG.ClientSideKeyLineitemLoadinglocationFG, this.Session[LineItemLoadingLocationFG.ClientSideScriptLineitemLoadinglocationFG] as string,
																	false);
			}

			if (this.Session[ShipToFG.CLIENT_SIDE_SCRIPT_SHIPTO] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					ShipToFG.CLIENT_SIDE_KEY_SHIPTO,
					this.Session[ShipToFG.CLIENT_SIDE_SCRIPT_SHIPTO] as string);
			}

			if (this.Session[BillToFG.CLIENT_SIDE_SCRIPT_BILLTO_FG] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					BillToFG.CLIENT_SIDE_KEY_BILLTO_FG,
					this.Session[BillToFG.CLIENT_SIDE_SCRIPT_BILLTO_FG] as string);
			}

			if (this.Session[Flag01FG.ClientSideScriptFlag01] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					Flag01FG.ClientSideKeyFlag01,
					this.Session[Flag01FG.ClientSideScriptFlag01] as string);
			}

			if (this.Session[ManagerFG.CLIENT_SIDE_SCRIPT_MANAGER_FG] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					ManagerFG.CLIENT_SIDE_KEY_MANAGER_FG,
					this.Session[ManagerFG.CLIENT_SIDE_SCRIPT_MANAGER_FG] as string);
			}

			if (this.Session[CarrierFG.CLIENT_SIDE_SCRIPT_CARRIER_FG] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					CarrierFG.CLIENT_SIDE_KEY_CARRIER_FG,
					this.Session[CarrierFG.CLIENT_SIDE_SCRIPT_CARRIER_FG] as string);
			}

			if (this.Session[SupplierFG.CLIENT_SIDE_SCRIPT_SUPPLIER_FG] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					SupplierFG.CLIENT_SIDE_KEY_SUPPLIER_FG,
					this.Session[SupplierFG.CLIENT_SIDE_SCRIPT_SUPPLIER_FG] as string);
			}

			if (this.Session[ShipperFG.CLIENT_SIDE_SCRIPT_SHIPPER_FG] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					ShipperFG.CLIENT_SIDE_KEY_SHIPPER_FG,
					this.Session[ShipperFG.CLIENT_SIDE_SCRIPT_SHIPPER_FG] as string);
			}

			if (this.Session[CardNumberFG.CLIENT_SIDE_SCRIPT_CARD_NUMBER] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					CardNumberFG.CLIENT_SIDE_KEY_CARD_NUMBER,
					this.Session[CardNumberFG.CLIENT_SIDE_SCRIPT_CARD_NUMBER] as string);
			}

			if (this.Session[UserDataTextFG.CLIENT_SIDE_SCRIPT_USER_DATA] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					UserDataTextFG.CLIENT_SIDE_KEY_USER_DATA,
					this.Session[UserDataTextFG.CLIENT_SIDE_SCRIPT_USER_DATA] as string);
			}

			if (this.Session[UserDataListFG.CLIENT_SIDE_SCRIPT_USERDATA_LIST] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					UserDataListFG.CLIENT_SIDE_KEY_USERDATA_LIST,
					this.Session[UserDataListFG.CLIENT_SIDE_SCRIPT_USERDATA_LIST] as string);
			}

			if (this.Session[TransactionStatusFG.CLIENT_SIDE_SCRIPT_TRANS_STATUS] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					TransactionStatusFG.CLIENT_SIDE_KEY_TRANS_STATUS,
					this.Session[TransactionStatusFG.CLIENT_SIDE_SCRIPT_TRANS_STATUS] as string);
			}

			if (this.Session[LineItemProductFG.CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCT_FG] != null)
			{
				ScriptManager.RegisterClientScriptBlock(
					this.Page,
					this.GetType(),
					LineItemProductFG.CLIENT_SIDE_KEY_LINEITEM_PRODUCT_FG,
					this.Session[LineItemProductFG.CLIENT_SIDE_SCRIPT_LINEITEM_PRODUCT_FG] as string,
					false);
			}

			if (this.Session[Flag05FG.CLIENT_SIDE_SCRIPT_FLAG05] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					Flag05FG.CLIENT_SIDE_KEY_FLAG05,
					this.Session[Flag05FG.CLIENT_SIDE_SCRIPT_FLAG05] as string);
			}

			if (this.Session[Flag06FG.CLIENT_SIDE_SCRIPT_FLAG06] != null)
			{
				this.ClientScript.RegisterStartupScript(
					this.GetType(),
					Flag06FG.CLIENT_SIDE_KEY_FLAG06,
					this.Session[Flag06FG.CLIENT_SIDE_SCRIPT_FLAG06] as string);
			}

			// Register the page onload event for custom scripts
			this.CreateAnOnloadPageEventForCustomScripts();

			if (this.Request.GetQueryOrFormValue("__MYEVENTTARGET") == "ASSOCIATIONS_CHANGED")
			{
				try
				{
					this.AggregateAssociatedTxValues(Convert.ToInt32(this.Request.GetQueryOrFormValue("__MYEVENTARGUMENT")), true);
				}
				catch
				{
					// ignored
				}
			}

			if (this.LineItemDataGrid.EditItemIndex > -1)
			{
				if (this.TransContext != null
						&& this.TransContext.aliasClass.AggregateAssociatedTransactions
					&& this.TransContext.aliasClass.AssociatedAliases.Count > 0)
				{
					LineItemDO lineItem = this.Trans.LineItems[this.LineItemDataGrid.EditItemIndex];
					// Create and populate the request object
					List<AssociatedTxDO> associatedTransactions = lineItem.AssociatedTransactions;

					foreach (AssociatedTxDO assocTx in associatedTransactions)
					{
						if (assocTx.Associated == 1)
						{
							// A Coker 03/14/2009 
							// Moved it here from LineItemGridGenerator so that text box retains its intented value.
							// vthompson 9/30/2008
							// If the alias is set to aggregate associated transactions then
							// gross quantity is only editable if no transactions are associated
							var textBox =
								this.LineItemDataGrid.Items[this.LineItemDataGrid.EditItemIndex].FindControl(
									"TransactionFields.LineItemGrossQuantityFG") as TextBox;

							if (!string.IsNullOrEmpty(textBox?.Text) && !(FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey())
																										&& (this.Trans.TransTypeID == TransactionTypes.T8_Receipt
																											|| this.Trans.Alias.ToUpper().Contains("FUEL ORDER"))))
							{
								// JS20100510 should not apply to receipts for ADF
								textBox.ReadOnly = true;
								textBox.BackColor = Color.LightGray;
							}
							var select =
								this.LineItemDataGrid.Items[this.LineItemDataGrid.EditItemIndex].FindControl(
									"TransactionFields.LineItemDeliveryLocationFG") as HtmlSelect;
							if (select != null && select.SelectedIndex > 0)
							{
								select.Disabled = true;
							}
							break;
						}
					}
				}
			}

			if (this.Request.Cookies["ActiveElement"] == null)
			{
				string firstEnabledFieldControlID = null;

				foreach (TableRow row in this.FieldTable.Rows)
				{
					foreach (TableCell cell in row.Cells)
					{
						if (!cell.ID.StartsWith("FieldValue"))
						{
							continue;
						}

						foreach (Control control in cell.Controls)
						{
							var updatePanel = control as UpdatePanel;
							if (updatePanel == null)
							{
								continue;
							}

							var webControl = updatePanel.ContentTemplateContainer.Controls[0] as WebControl;

							if (webControl != null)
							{
								var textBox = webControl as TextBox;
								if (textBox != null)
								{
									if (textBox.ReadOnly)
									{
										continue;
									}

									if (textBox.Enabled == false)
									{
										continue;
									}
								}

								else
								{
									if (webControl.Enabled == false)
									{
										continue;
									}
								}

								if (webControl is FMComboBox)
								{
									firstEnabledFieldControlID = control.ID + "__TextBox";
								}
								else if (typeof(FMDate) != control.GetType())
								{
									firstEnabledFieldControlID = control.ID;
								}
								else
								{
									continue;
								}
								break;
							}

							var htmlControl = updatePanel.ContentTemplateContainer.Controls[0] as HtmlControl;

							if (htmlControl != null)
							{
								if (htmlControl.Disabled)
								{
									continue;
								}

								firstEnabledFieldControlID = control.ID;
								break;
							}
						}

						if (firstEnabledFieldControlID != null)
						{
							break;
						}
					}

					if (firstEnabledFieldControlID != null)
					{
						break;
					}
				}

				if (firstEnabledFieldControlID != null)
				{
					this.Request.Cookies.Add(new HttpCookie("ActiveElement", firstEnabledFieldControlID));
				}
			}

			string activeElement = null;

			if (this.Request.Cookies["ActiveElement"] != null)
			{
				activeElement = this.Request.Cookies["ActiveElement"].Value;
				this.Response.Cookies.Add(new HttpCookie("ActiveElement", this.Request.Cookies["ActiveElement"].Value));
			}

			if (this.IsPostBack)
			{
				const string RenableUpdatePanel = "<script type=\"text/javascript\">\r\n"
												+ " var updatePanelDiv = document.getElementById('UpdatePanel1');"
												+ " if (updatePanelDiv != null) "
												+ "	updatePanelDiv.disabled = false;"
												+ "</script>\r\n";

				ScriptManager.RegisterStartupScript(this.Page, this.GetType(),
															"RENABLEPANEL",
															RenableUpdatePanel,
															false);
			}

			if (!string.IsNullOrEmpty(activeElement))
			{
				string setFocus = "<script type=\"text/javascript\">\r\n" + "$(document).ready(function() {\r\n"
									+ "	var activeElement=document.getElementById(\"" + activeElement + "\");\r\n"
									+ " if(activeElement != null)\r\n"
									+ "	{\r\n"
									+ "		if(activeElement.style.visibility == \"hidden\")\r\n"
									+ "			activeElement.style.visibility=\"visible\";\r\n"
									+ "		activeElement.focus();\r\n"
									+ "		if(!activeElement.activeElement)\r\n"
									+ "		{\r\n"
									+ "			try{\r\n"
									+ "				activeElement.setActive();\r\n"
									+ "			}catch(e){}\r\n"
									+ "			try{\r\n"
									+ "				activeElement.select();"
									+ "			}catch(e){}\r\n"
									+ "		}\r\n"
									+ "	}\r\n"
									+ "});\r\n </script>\r\n";
				ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SETFOCUS", setFocus, false);
			}


		}

		protected virtual void PerformDrawdown()
		{
			if (this.TransContext.aliasClass.TransTypeID != TransactionTypes.T18_SupplyOrder
				&& this.TransContext.aliasClass.TransTypeID != TransactionTypes.T9_Request)
			{
				return;
			}

			foreach (LineItemDO lineItem in this.Trans.LineItems)
			{
				var sr = new DrawdownSR { Alias = this.TransContext.aliasClass, LineItem = lineItem, Security = this.security };

				DrawdownDO drawdown = FMChannelHelper.MakeCall<IDrawdownProcessor, DrawdownDO>(x => x.Process(sr));

				if (drawdown.QuantityLimitExceeded)
				{
					var ex = new ApplicationException("The budget quantity for " + lineItem.Product + " " + "has been exceeded.");
					this.HandleFieldError(ex);
					break;
				}

				if (drawdown.QuantityToleranceExceeded)
				{
					var ex = new ApplicationException("The tolerance quantity for " + lineItem.Product + " " + "has been exceeded.");
					this.HandleFieldError(ex);
					break;
				}

				if (drawdown.ValueLimitExceeded)
				{
					var ex = new ApplicationException("The budget value for " + lineItem.Product + " " + "has been exceeded.");
					this.HandleFieldError(ex);
					break;
				}

				if (drawdown.ValueToleranceExceeded)
				{
					var ex = new ApplicationException("The tolerance value for " + lineItem.Product + " " + "has been exceeded.");
					this.HandleFieldError(ex);
					break;
				}
			}
		}

		/// <summary>
		///	When updating or adding line items the header virtual fields are not updating.
		///	They update fine when loaded but not when line item changes are made.  This will
		///	update the virtual fields based on line item changes
		/// </summary>
		protected void PopulateHeaderVirtualFields()
		{
			// Find each virtual field and update it's value
			// Total Price
			var textBox = this.FieldTable.FindControl("TransactionFields.TotalPriceAmountFG") as TextBox;
			if (textBox != null)
			{
				textBox.Text = this.Trans.TotalPrice.ToString("N");
			}

			textBox = this.FieldTable.FindControl("TransactionFields.TotalPriceWithTaxFG") as TextBox;
			if (textBox != null)
			{
				textBox.Text = this.Trans.TotalPriceWithTax.ToString("N");
			}

			textBox = this.FieldTable.FindControl("TransactionFields.TotalExciseFG") as TextBox;
			if (textBox != null)
			{
				textBox.Text = this.Trans.TotalExcise.ToString("N");
			}

			textBox = this.FieldTable.FindControl("TransactionFields.TotalGSTFG") as TextBox;
			if (textBox != null)
			{
				textBox.Text = this.Trans.TotalGST.ToString("N");
			}

			textBox = this.FieldTable.FindControl("TransactionFields.TotalMarkupFG") as TextBox;
			if (textBox != null)
			{
				textBox.Text = this.Trans.TotalMarkup.ToString("N");
			}

			textBox = this.FieldTable.FindControl("TransactionFields.TotalGrossQuantityFG") as TextBox;
			if (textBox != null)
			{
				textBox.Text = this.Trans.TotalGrossQuantity.ToString("N", this.TransContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
			}

			textBox = this.FieldTable.FindControl("TransactionFields.TotalNetQuantityFG") as TextBox;
			if (textBox != null)
			{
				textBox.Text = this.Trans.TotalNetQuantity.ToString("N", this.TransContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
			}

			textBox = this.FieldTable.FindControl("TransactionFields.TotalMassQuantityFG") as TextBox;
			if (textBox != null)
			{
				textBox.Text = this.Trans.TotalMassQuantity.ToString("N", this.TransContext.accountingSite.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));
			}
		}

		protected virtual void PreviousButtonClick(object sender, EventArgs e)
		{
			var list = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList;

			if (list != null)
			{
				list.CurrentIndex--;
			}

			// Read the TransactionDetail URL from the Web.config file (08-Jul-2009 IGO)
			string transactionDetailUrl = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];
			string url = "../" + transactionDetailUrl + "?" + AliasKey + "=" + this.Trans.Alias;
			this.Redirect(url);
			this.Context.ApplicationInstance.CompleteRequest();
		}

		protected virtual void RegenerateControls(bool reload)
		{
			if (reload)
			{
				this.UpdatePanel1.Update();
			}

			var timer = new StopWatch(StopWatch.Appnames.Accounting, "RegenerateControls() - loadSiteInfo");
			timer.Debug("RegenerateControls() - START");

			if (this.Session["TransactionDetail.AccountingSite"] == null)
			{

				this.AccountingSite =
					FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
						x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

				this.Session["TransactionDetail.AccountingSite"] = this.AccountingSite;
			}
			else
			{
				this.AccountingSite = this.Session["TransactionDetail.AccountingSite"] as AccountingSite;
			}

			this.transAlias = this.Request.GetQueryOrFormValue("TransAlias");

			if (string.IsNullOrEmpty(this.transAlias) == false)
			{
				this.transAlias = this.transAlias.RemoveSemicolonAndTextAfter();
			}

			var mode = TransactionContext.Mode.View;

			timer.Stop();

			if (!this.IsPostBack && string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("KEEPASSOCCONTEXT")))
			{
				this.Session.Remove("AssociatedTxContext");
			}

			bool newlyLoadedTransaction = false;

			if (!this.IsPostBack && this.Request.GetQueryOrFormValue("RETURNING") != null)
			{
				//This block gets executed after Associated Transactions page is closed.
				var associatedTxContext = this.Session["AssociatedTxContext"] as AssociatedTxContext;

				if (associatedTxContext != null)
				{
					this.Trans = associatedTxContext.transaction;

					int editItemIndex = Convert.ToInt32(associatedTxContext.EditItemIndex);
					this.LineItemDataGrid.EditItemIndex = editItemIndex;

					if (editItemIndex > -1 && this.Trans != null)
					{
						this.Session[SessionLineItemObject] = this.Trans.LineItems[editItemIndex];
						if (associatedTxContext.TransactionLineItemGuid == Guid.Empty.ToString())
						{
							this.Session[SessionLineItemAdded] = editItemIndex;
						}
					}
					else
					{
						this.Session.Remove(SessionLineItemObject);
					}

					if (associatedTxContext.mode == "Add")
					{
						mode = TransactionContext.Mode.Add;
					}
					else if (associatedTxContext.mode == "Edit")
					{
						mode = TransactionContext.Mode.Edit;
					}
					else
					{
						mode = TransactionContext.Mode.View;
					}

					this.Session[ModeKey] = mode;
					this.Session[TransKey] = this.Trans;
					this.Session[TransactionDetailList.TransactionDetailListKey] = associatedTxContext.DetailList;
					this.Session["allAssociatedTransactionsBeforeTransactionEdit"] =
											associatedTxContext.allAssociatedTransactionsBeforeTransactionEdit;
					this.Session["associatedTransactionsBeforeEdit"] = associatedTxContext.associatedTransactionsBeforeEdit;

					if (associatedTxContext.previousAssociatedTxContext == null)
					{
						this.Session.Remove("AssociatedTxContext");
					}
					else
					{
						this.Session["AssociatedTxContext"] = associatedTxContext.previousAssociatedTxContext;
					}
				}
			}
			else if (!this.IsPostBack)
			{
				string transMode = this.Request.GetQueryOrFormValue(ModeKey);
				string transID = this.Request.GetQueryOrFormValue("TransID");
				bool bNewTransAction = false;

				if (string.IsNullOrEmpty(transID))
				{
					if ((string.IsNullOrEmpty(transMode)) || (!transMode.Equals("ADD")))
					{
						var transList = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList;

						if (transList != null)
						{
							if (transList.TransactionIDList.Count > 0)
							{
								transID = transList.TransactionIDList[transList.CurrentIndex];
							}
							else
							{
								this.Trans = transList.NewTransaction;
								mode = TransactionContext.Mode.Add;
								bNewTransAction = true;
							}
						}
					}
				}

				if (!bNewTransAction || this.Trans == null)
				{
					if (transID != null)
					{
						this.GetTransaction(transID);
						newlyLoadedTransaction = true;
					}
					else
					{
						timer.ActionName = "RegenerateControls() - InitTransaction";
						timer.Start();
						this.InitTransaction();
						timer.Stop();

						mode = TransactionContext.Mode.Add;
					}
				}
				this.Session.Remove("allAssociatedTransactionsBeforeTransactionEdit");
			}
			else
			{
				this.Trans = this.Session[TransKey] as TransactionDO;
				this.SetVolumeSigns(this.Trans, true);
				object modeObject = this.Session[ModeKey];
				if (modeObject == null)
				{
					mode = TransactionContext.Mode.View;
				}
				else
				{
					mode = (TransactionContext.Mode)modeObject;
				}
			}

			if (string.IsNullOrEmpty(this.transAlias))
			{
				var transactionDO = this.Trans;

				if (transactionDO != null)
				{
					this.transAlias = transactionDO.Alias;
				}
			}

			this.lblPageTitle.Text = this.transAlias;

			if (this.Session["UseDataDictionary"] != null)
			{
				this.useDataDictionary = (bool)this.Session["UseDataDictionary"];
			}

			if (this.Session["TransactionDetail.TransactionContext"] == null)
			{
				this.TransContext = new TransactionContext(this.security, this.AccountingSite, this.transAlias, mode, this.useDataDictionary);
				this.TransContext.GetTransactionContext(this.AliasObject);
				this.Session["TransactionDetail.TransactionContext"] = this.TransContext;
			}
			else
			{
				this.TransContext = this.Session["TransactionDetail.TransactionContext"] as TransactionContext;
			}

			timer.Start("RegenerateControls() - GetTransactionContext()");
			var transactionContext = this.TransContext;

			if (transactionContext != null)
			{
				transactionContext.GetTransactionContext(this.AliasObject);
				timer.Stop();

				if (newlyLoadedTransaction)
				{
					if (this.IsTransactionEditable)
					{
						mode = TransactionContext.Mode.Edit;
					}
					else
					{
						mode = TransactionContext.Mode.View;
					}
				}

				transactionContext.mode = mode;

				this.Session.Add(ModeKey, mode);
				this.Session.Add(TransKey, this.Trans);

				// this control is hidden on the web app. this is necessary to control the auto populate capability
				this.LimitSelectionsBasedOnHierarchy.Value = transactionContext.aliasClass.LimitSelectionsBasedOnHierarchy
					? "true"
					: "false";

				// if we are setup for hierarchy support check the boxes for additional auto add items
				if (!this.IsPostBack)
				{
					this.AutoPopulateHierarchalData();

					if (this.Request.GetQueryOrFormValue("RETURNING") != null || newlyLoadedTransaction)
					{
						if (transactionContext.aliasClass.AggregateAssociatedTransactions
							&& transactionContext.aliasClass.AssociatedAliases.Count > 0)
						{
							for (int i = 0; i < this.Trans.LineItems.Count; ++i)
							{
								//if aggregate, fields such as quantity, excise, etc need to be re-calculated based on associated transactions.
								this.AggregateAssociatedTxValues(i, false);
							}
						}
					}
				}

				// Remove all the controls from the table control
				this.FieldTable.Controls.Clear();

				transactionContext.reload = reload;

				// Remove the items from the DataGrid control
				foreach (DataGridItem item in this.LineItemDataGrid.Items)
				{
					for (int i = 2; i < item.Cells.Count - 2; ++i)
					{
						item.Cells.RemoveAt(i);
					}
				}

				// Remove the colums from the line item datagrid control
				while (this.LineItemDataGrid.Columns.Count > 4)
				{
					if (this.LineItemDataGrid.Columns[2].HeaderText.ToUpper().Equals("TRANSACTIONS") ||
						this.LineItemDataGrid.Columns[2].HeaderText.ToUpper().Equals("DELETE"))
					{
						continue;
					}
					else
					{
						this.LineItemDataGrid.Columns.RemoveAt(2);
					}
				}

				// Remove the items from the gauge reading datagrid control
				foreach (DataGridItem item in this.GaugeReadingsDataGrid.Items)
				{
					for (int i = 2; i < item.Cells.Count; ++i)
					{
						item.Cells.RemoveAt(i);
					}
				}

				// Remove the columns from the gauge reading datagrid control
				while (this.GaugeReadingsDataGrid.Columns.Count > 2)
				{
					this.GaugeReadingsDataGrid.Columns.RemoveAt(2);
				}

				// Remove the items from the transport line item datagrid control
				foreach (DataGridItem item in this.TransportDataGrid.Items)
				{
					for (int nextCell = 2; nextCell < item.Cells.Count; ++nextCell)
					{
						item.Cells.RemoveAt(nextCell);
					}
				}

				// Remove the columns from the transport line item datagrid control
				while (this.TransportDataGrid.Columns.Count > 2)
				{
					this.TransportDataGrid.Columns.RemoveAt(2);
				}

				// vthompson 7/25/2008
				// Disable sub-items for selected Transaction Types
				Debug.Assert(this.Trans != null);

				bool hideTransactionColumn = false;

				if (this.Trans.TransTypeID == TransactionTypes.T17_Order
					|| this.Trans.TransTypeID == TransactionTypes.T18_SupplyOrder
					|| this.Trans.TransTypeID == TransactionTypes.T14_PhysicalInventory
					|| this.Trans.TransTypeID == TransactionTypes.T15_PrimaryRegrade
					|| this.Trans.TransTypeID == TransactionTypes.T16_SecondaryRegrade
					|| this.Trans.TransTypeID == TransactionTypes.T1_PrimaryAdjustment
					|| this.Trans.TransTypeID == TransactionTypes.T2_SecondaryAdjustment
					|| this.Trans.TransTypeID == TransactionTypes.T9_Request
					|| this.Trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice
					|| this.Trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice
					|| this.Trans.TransTypeID == TransactionTypes.T23_StorageTransfer
					|| this.Trans.TransTypeID == TransactionTypes.T13_OwnerTransfer)
				{
					// Do not show the Sub Item Column
					this.LineItemDataGrid.Columns[0].Visible = false;

					// Do not show the Transactions column
					if (((this.Trans.TransTypeID != TransactionTypes.T17_Order)
						&& (this.Trans.TransTypeID != TransactionTypes.T18_SupplyOrder))
						&& transactionContext.aliasClass.AssociatedAliases.Count == 0)
					{
						hideTransactionColumn = true;
					}
				}
				else
				{
					// Do not show the Transactions column
					hideTransactionColumn = true;
				}

				if (hideTransactionColumn == true)
				{
					// find the location of the TRANSACTIONS column
					int sublineitemColumnLocation = -1;
					int loopNumber = 0;
					foreach (DataGridColumn column in this.LineItemDataGrid.Columns)
					{
						if (column.HeaderText.ToUpper().Equals("TRANSACTIONS"))
						{
							sublineitemColumnLocation = loopNumber;
							break;
						}
						++loopNumber;
					}
					if (sublineitemColumnLocation > -1)
					{
						this.LineItemDataGrid.Columns[sublineitemColumnLocation].Visible = false;
					}
				}

				if (!string.IsNullOrEmpty(this.OrderReferenceID))
				{
					this.PrepAssociatedTransaction();
				}

				// Apply the data dictionary to the non dynamic column headers such as
				// Edit, Delete, and Add-sublineitems.
				this.ApplyDictionaryToNonDynamicColumnHeaders();

				if (this.Request.GetQueryOrFormValue("__MYEVENTTARGET") == "SHIPTO_REFRESH")
				{
					this.RefreshSpecialInstructions();
				}

				var timer3 = new StopWatch(StopWatch.Appnames.Accounting, "BindControls()");
				this.BindControls();
				timer3.Stop();

				this.TransIDLabel.Text = this.Trans.TransID;

				var customFields = new FMCustomFieldStatesClass();
				customFields.SetTransactionFieldStates(this.security, this);

				if (newlyLoadedTransaction)
				{
					var updatedByDate = this.FieldTable.FindControl("TransactionFields.UpdatedDateFG DateTime") as FMDateTime;

					if (updatedByDate != null && this.Trans != null)
					{
						DateTimeOffset siteTime = TimeConverter.ToSiteTime(this.AccountingSite.CurrentSite, this.Trans.UpdatedDate);
						string convertedDate = this.AccountingSite.FormatDateTime(siteTime);
						updatedByDate.Text = convertedDate;
					}
				}

				transactionContext.reload = false;
			}
		}

		protected void Reset()
		{
			this.Session.Remove(ModeKey);
			this.Session.Remove(ReturnPageKey);
			this.Session.Remove(TransactionDetailList.TransactionDetailListKey);
		}

		/// <summary>
		///	This method will retrieve the data from the transaction detail page and
		///	updated the TransactionDO object with the new values.  Once the new values
		///	are updated the price calculator can update the calculations.
		/// </summary>
		/// <returns></returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		protected virtual bool RetrieveDataFromPage()
		{
			var timer = new StopWatch(StopWatch.Appnames.Accounting, "TransactionDetail() - RetrieveDataFromPage()");

			const bool NoErrors = true;
			CurrencyClass currency = null;

			try
			{
				// Save the original values prior to getting the new values from
				// the page. This is used for the PriceCalculator.
				var origLineItems = new List<LineItemDO>();
				foreach (LineItemDO lineItemDO in this.Trans.LineItems)
				{
					var newLineItemDO = new LineItemDO(lineItemDO);
					origLineItems.Add(newLineItemDO);
				}

				// JS20100722 Changed rounding restoration code to here to prevent onload pricing from being calculated incorrectly
				// JS20100714 Rounding error, remember the pre-rounded value
				var oldU23Array = new object[this.Trans.LineItems.Count];
				var unroundedPriceList = new ArrayList();

				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey())
					&& !this.Trans.Alias.ToUpper().Equals("COMMERCIAL")
					&& !this.Trans.Alias.ToUpper().Equals("DIRECT FUEL PURCHASE"))
				{
					for (int i = 0; i < this.Trans.LineItems.Count; ++i)
					{
						double? liProductPrice = (this.Trans.LineItems[i]).ProductPrice;
						oldU23Array[i] = null;

						if (liProductPrice == null)
						{
							unroundedPriceList.Add(null);
						}
						else if (liProductPrice.Value == 0.0)
						{
							unroundedPriceList.Add(null);
						}
						else
						{
							oldU23Array[i] = (this.Trans.LineItems[i]).UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_23];
							unroundedPriceList.Add(liProductPrice.Value);
							(this.Trans.LineItems[i]).UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_23] =
														liProductPrice.Value.ToString(CultureInfo.InvariantCulture);
						}
					}
				}

				this.TransactionFieldGenerator.Retrieve();

				// JS20100714 Restore precision to fuel price
				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey())
					&& !this.Trans.Alias.ToUpper().Equals("COMMERCIAL")
					&& !this.Trans.Alias.ToUpper().Equals("DIRECT FUEL PURCHASE"))
				{
					for (int i = 0; i < this.Trans.LineItems.Count; ++i)
					{
						// check if requires restore
						var price = unroundedPriceList[i] as double?;

						if (price != null)
						{
							(this.Trans.LineItems[i]).UserData[BaseTransactionLineItemDO.USER_DATA_LINE_ITEM_KEY_23] = oldU23Array[i].ToString();
							(this.Trans.LineItems[i]).ProductPrice = price;
						}
					}
				}

				if (this.TransContext.Currencies != null)
				{
					currency = new CurrencyClass(this.security) { InventoryDate = this.Trans.InventoryDate };
				}

				foreach (LineItemDO lineItemDO in this.Trans.LineItems)
				{
					this.SetLineItemCurrencyFields(currency, lineItemDO);
				}

				if (this.TransContext.aliasClass.MultipleLineItems == false && this.Trans.LineItems.Count > 0)
				{
					LineItemDO lineItem = this.Trans.LineItems[0];

					// If the Alternate Volume field is populate, then convert the value and
					// populate the Gross Volume field.
					if (this.ConvertAlternateVolumeToGrossVolume(lineItem) == false)
					{
						return false;
					}
				}

				// JS20100920 Calculate at a different point, price calculator needs the net volume for WAC
				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey())
					&& this.TransContext.aliasClass.MultipleLineItems == false && this.Trans.LineItems.Count > 0)
				{
					this.CalculateVolumes(this.Trans.LineItems[0]);
				}
				FMChannelHelper.MakeCall<IPriceCalculatorInvoker, TransactionDO>(
					x => x.CalculateWithLineItems(this.security, this.Trans, origLineItems));

				if (this.TransContext.aliasClass.AggregateAssociatedTransactions
					&& this.TransContext.aliasClass.AssociatedAliases.Count > 0)
				{
					//Override excise, gst, markup, total value and total price with tax values
					//with their respective aggregated values.
					for (int i = 0; i < this.Trans.LineItems.Count; i++)
					{
						this.AggregateAssociatedTxValues(i, false);
					}
				}

				//If not using the Line Item Grid, we must calculate VCF and Net now,
				//since it would not be set in RetrieveLineItem.
				if (this.TransContext.aliasClass.MultipleLineItems == false && this.Trans.LineItems.Count > 0)
				{
					LineItemDO lineItem = this.Trans.LineItems[0];

					var txtTotalValue = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemTotalValueFG");
					if (txtTotalValue != null)
					{
						FieldGenerator lineItemTotalValue = this.TransactionFieldGenerator.GetFieldGenerator("LineItem TotalValue");

						if (lineItemTotalValue != null)
						{
							txtTotalValue.Text = lineItemTotalValue.GetFormattedValue();
						}
					}

					var txtTotalPriceWithTax = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemTotalPriceWithTaxFG");
					if (txtTotalPriceWithTax != null)
					{
						FieldGenerator lineItemTotalPriceWithTax =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem TotalPriceWithTax");

						if (lineItemTotalPriceWithTax != null)
						{
							txtTotalPriceWithTax.Text = lineItemTotalPriceWithTax.GetFormattedValue();
						}
					}

					this.SetLineItemCurrencyFields(currency, lineItem);
					var txtPrice = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemProductPriceFG");
					if (txtPrice != null)
					{
						FieldGenerator lineItemProductPrice = this.TransactionFieldGenerator.GetFieldGenerator("LineItem ProductPrice");
						if (lineItemProductPrice != null)
						{
							txtPrice.Text = lineItemProductPrice.GetFormattedValue();
						}
					}
					var txtExchangeRate = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemExchangeRateFG");
					if (txtExchangeRate != null)
					{
						FieldGenerator lineItemExchangeRateFG = this.TransactionFieldGenerator.GetFieldGenerator("LineItem ExchangeRate");
						if (lineItemExchangeRateFG != null)
						{
							txtExchangeRate.Text = lineItemExchangeRateFG.GetFormattedValue();
						}
					}

					var txtTotalGrossQuantity = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemGrossQuantityFG");
					if (txtTotalGrossQuantity != null)
					{
						FieldGenerator lineItemGrossQuantity = this.TransactionFieldGenerator.GetFieldGenerator("LineItem GrossQuantity");

						if (lineItemGrossQuantity != null)
						{
							txtTotalGrossQuantity.Text = lineItemGrossQuantity.GetFormattedValue();
						}
					}

					var txtTotalNetQuantity = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemNetQuantityFG");
					if (txtTotalNetQuantity != null)
					{
						FieldGenerator lineItemNetQuantity = this.TransactionFieldGenerator.GetFieldGenerator("LineItem NetQuantity");

						if (lineItemNetQuantity != null)
						{
							txtTotalNetQuantity.Text = lineItemNetQuantity.GetFormattedValue();
						}
					}
					var txtMassQuantity = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemMassQuantityFG");
					if (txtMassQuantity != null)
					{
						FieldGenerator lineItemMassQuantity = this.TransactionFieldGenerator.GetFieldGenerator("LineItem MassQuantity");

						if (lineItemMassQuantity != null)
						{
							txtMassQuantity.Text = lineItemMassQuantity.GetFormattedValue();
						}
					}
					// JS20100701 WI-15697 Ensures the actuals are updated
					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) && this.Trans.Alias.ToUpper().Contains("SALE"))
					{
						var numberFields = new ArrayList { "Number01", "Number02", "Number03", "Number04", "Number05", "Number06" };

						foreach (string fieldName in numberFields)
						{
							var txtNumberField = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItem" + fieldName + "FG");

							if (txtNumberField != null)
							{
								FieldGenerator txtNumberGenerator = this.TransactionFieldGenerator.GetFieldGenerator("LineItem " + fieldName);
								if (txtNumberGenerator != null)
								{
									txtNumberField.Text = txtNumberGenerator.GetFormattedValue();
								}
							}
						}
					}

					for (int i = 1; i <= 24; i++)
					{
						string fieldID = "TransactionFields.UserDataTextFG" + BaseTransactionLineItemDO.UserDataLineItemKeyPrefix + i;
						Control control = this.FieldTable.FindControl(fieldID);

						if (control is TextBox)
						{
							var textBox = control as TextBox;
							lineItem.UserData[BaseTransactionLineItemDO.UserDataLineItemKeyPrefix + i] = textBox.Text;
							continue;
						}

						fieldID = "TransactionFields.UserDataListFG" + BaseTransactionLineItemDO.UserDataLineItemKeyPrefix + i;
						control = this.FieldTable.FindControl(fieldID);

						if (control is HtmlSelect)
						{
							var listBox = control as HtmlSelect;
							lineItem.UserData[BaseTransactionLineItemDO.UserDataLineItemKeyPrefix + i] = listBox.Items[listBox.SelectedIndex].Value;
						}
						else if (control is FMComboBox)
						{
							var comboBox = control as FMComboBox;
							lineItem.UserData[BaseTransactionLineItemDO.UserDataLineItemKeyPrefix + i] = comboBox.Items[comboBox.SelectedIndex].Value;
						}
					}

					// JS20100920 Calculate at a different point, price calculator needs the net volume for WAC
					if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()) == false)
					{
						this.CalculateVolumes(lineItem);
					}

					// Clear dirty flag for items 
					lineItem.Quantity.IsGrossDirty = false;
					lineItem.Quantity.IsNetDirty = false;
					lineItem.Quantity.IsMassDirty = false;
					lineItem.Quantity.IsPackageDirty = false;
					lineItem.Quantity.IsVcfDirty = false;

					foreach (SubLineItemDO subLine in lineItem.SubLineItems)
					{
						subLine.Quantity.IsGrossDirty = false;
						subLine.Quantity.IsNetDirty = false;
						subLine.Quantity.IsMassDirty = false;
						subLine.Quantity.IsPackageDirty = false;
						subLine.Quantity.IsVcfDirty = false;
					}

					//????? not sure of its exact purpose. It used to be Net rather than Gross.
					var grossVolumeTextBox = this.FieldTable.FindControl("TransactionFields.LineItemGrossQuantityFG") as TextBox;

					if (grossVolumeTextBox != null)
					{
						FieldGenerator grossFieldGenerator = this.TransactionFieldGenerator.GetFieldGenerator("LineItem GrossQuantity");
						grossVolumeTextBox.Text = grossFieldGenerator.GetFormattedValue();
					}

					if (TransactionTypes.T18_SupplyOrder == this.Trans.TransTypeID && lineItem.ProductPrice != null)
					{
						lineItem.ValueRemaining = lineItem.GrossQuantityRemaining * lineItem.ProductPrice.Value;
					}

					// also figure out fuel price based on Standing Offers (aka Price List) (14-Nov-2007 IGO)
					if (TransactionTypes.T1_PrimaryAdjustment == this.Trans.TransTypeID
						|| TransactionTypes.T2_SecondaryAdjustment == this.Trans.TransTypeID
						|| TransactionTypes.T3_PrimaryDefuel == this.Trans.TransTypeID
						|| TransactionTypes.T4_SecondaryDefuel == this.Trans.TransTypeID
						|| TransactionTypes.T5_PrimaryDisbursement == this.Trans.TransTypeID
						|| TransactionTypes.T6_SecondaryDisbursement == this.Trans.TransTypeID
						|| TransactionTypes.T8_Receipt == this.Trans.TransTypeID
						|| TransactionTypes.T9_Request == this.Trans.TransTypeID
						|| TransactionTypes.T11_ConsumerTransfer == this.Trans.TransTypeID
						|| TransactionTypes.T12_InventoryNotAffected == this.Trans.TransTypeID
						|| TransactionTypes.T13_OwnerTransfer == this.Trans.TransTypeID
						|| TransactionTypes.T18_SupplyOrder == this.Trans.TransTypeID
						|| TransactionTypes.T15_PrimaryRegrade == this.Trans.TransTypeID
						|| TransactionTypes.T25_Shipment == this.Trans.TransTypeID)
					{
						var productPriceTextBox = this.FieldTable.FindControl("TransactionFields.LineItemProductPriceFG") as TextBox;
						FieldGenerator productPriceFieldGenerator =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem ProductPrice");

						if (null != productPriceTextBox)
						{
							productPriceTextBox.Text = productPriceFieldGenerator.GetFormattedValue();
						}
					}

					// For physical inventory use average unit price instead of price list price (aka standing offer price)
					if (this.Trans.TransTypeID == TransactionTypes.T14_PhysicalInventory)
					{
						var productPriceTextBox = this.FieldTable.FindControl("TransactionFields.LineItemProductPriceFG") as TextBox;
						FieldGenerator productPriceFieldGenerator =
							this.TransactionFieldGenerator.GetFieldGenerator("LineItem ProductPrice");

						if (null != productPriceTextBox)
						{
							productPriceTextBox.Text = productPriceFieldGenerator.GetFormattedValue();
						}
					}

					// Populate the Alternative Gross and Alternative Net fields
					var txtAltGross = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemAlternativeGrossVolumeFG");
					FieldGenerator altGrossFG = this.TransactionFieldGenerator.GetFieldGenerator("LineItem AlternativeGrossVolume");

					if (txtAltGross != null)
					{
						txtAltGross.Text = altGrossFG.GetFormattedValue();
					}

					var txtAltNet = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemAlternativeNetVolumeFG");
					FieldGenerator altNetFG = this.TransactionFieldGenerator.GetFieldGenerator("LineItem AlternativeNetVolume");

					if (txtAltNet != null)
					{
						txtAltNet.Text = altNetFG.GetFormattedValue();
					}

					// Set the excise tax value
					if ((this.Trans.TransTypeID == TransactionTypes.T8_Receipt)
						|| (this.Trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement)
						|| (this.Trans.TransTypeID == TransactionTypes.T25_Shipment)
						|| (this.Trans.TransTypeID == TransactionTypes.T12_InventoryNotAffected))
					{
						// The field must be manually populated when in single line item mode
						var txtExcise = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemTax1FG");
						FieldGenerator exciseFG = this.TransactionFieldGenerator.GetFieldGenerator("LineItem Tax1");

						if (txtExcise != null)
						{
							txtExcise.Text = exciseFG.GetFormattedValue();
						}

						var txtGst = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemTax2FG");
						FieldGenerator gstFG = this.TransactionFieldGenerator.GetFieldGenerator("LineItem Tax2");

						if (txtGst != null)
						{
							txtGst.Text = gstFG.GetFormattedValue();
						}

						var txtMarkup = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemTax3FG");
						FieldGenerator markuoFG = this.TransactionFieldGenerator.GetFieldGenerator("LineItem Tax3");

						if (txtMarkup != null)
						{
							txtMarkup.Text = markuoFG.GetFormattedValue();
						}
					}
				}

				return NoErrors;
			}
			catch (FMStandingOfferException soException)
			{
				this.HandleFieldError(soException);
				return soException.ContinueOn;
			}
			catch (Exception e)
			{
				this.HandleFieldError(e);
				return false;
			}
			finally
			{
				timer.Stop();
			}
		}

		protected virtual bool RetrieveLineItem(DataGridItem item)
		{
			try
			{
				bool noLineItemErrors = true;

				int lineItemIndex;
				int sublineItemIndex;
				this.GetItemIndices(item, out lineItemIndex, out sublineItemIndex);

				// Save the original values prior to getting the new values from
				// the page. This is used for the PriceCalculator.
				var origLineItems = new List<LineItemDO>();
				foreach (LineItemDO lineItemDO in this.Trans.LineItems)
				{
					var newLineItemDO = new LineItemDO(lineItemDO);
					origLineItems.Add(newLineItemDO);
				}

				foreach (WebControl control in item.Controls)
				{
					if (string.IsNullOrEmpty(control.ID))
					{
						continue;
					}

					//Remove the row index from the end of the ID to find out which field it is.
					//Be careful, "LineItem SourceEquipmentModel1 5" is the 5th row of SourceEquipmentModel1.
					//If we remove the fieldKey.Trim() line and put a space in trimCharacters, the 1 gets removed.
					//So do it in 2 steps.
					char[] trimCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
					string fieldKey = control.ID.TrimEnd(trimCharacters);
					fieldKey = fieldKey.Trim();

					FieldGenerator field = null;
					// Check to see if this is line item user data
					if (fieldKey.StartsWith(TransactionDO.UserDataLineItemKeyPrefix))
					{
						// Mandatory user data fields on the line items aren't triggering empty field validation
						foreach (FieldClass fieldClass in this.TransContext.aliasClass.DisplayOrder(TRANSACTION_SECTION_TYPE.LINE_ITEMS))
						{
							if (fieldClass.ID == fieldKey)
							{
								var userField = fieldClass as UserDataFieldClass;

								switch (userField.UserDataType)
								{
									case USER_DATA_TYPE.TEXT:
										field = new LineItemUserDataTextFG(fieldKey) { Required = fieldClass.FieldRequired };
										break;
									case USER_DATA_TYPE.LIST:
										field = new LineItemUserDataListFG(fieldKey) { Required = fieldClass.FieldRequired };
										break;
								}
								break;
							}
						}
					}
					else
					{
						field = this.TransactionFieldGenerator.GetFieldGenerator(fieldKey);
					}

					if ((field is ILineItemField) || (field is ISublineItemField))
					{
						try
						{
                     if (control.Controls.Count > 0)
							{

								if ((control.Controls[0] as UpdatePanel)?.ContentTemplateContainer.Controls[0] is HtmlSelect)
								{
									var selectList = (control.Controls[0] as UpdatePanel).ContentTemplateContainer.Controls[0] as HtmlSelect;
									string value = this.Request.Form[selectList.Name];

									if (value != null)
									{
										foreach (ListItem selectItem in selectList.Items)
										{
											if (selectItem.Value == value)
											{
												selectList.SelectedIndex = selectList.Items.IndexOf(selectItem);
												break;
											}
										}
									}
								}
								else if (control.Controls[0] is DropDownList)
								{
									var selectList = control.Controls[0] as DropDownList;
									string value = selectList.SelectedValue;

									if (value != null)
									{
										foreach (ListItem selectItem in selectList.Items)
										{
											if (selectItem.Value == value)
											{
												selectList.SelectedIndex = selectList.Items.IndexOf(selectItem);
												break;
											}
										}
									}
								}
							}

							field.DisplayName = control.ID;
							field.Retrieve(control, this.Trans, this.TransContext, lineItemIndex, sublineItemIndex);
						}
						catch (Exception e)
						{
							noLineItemErrors = false;
							this.HandleFieldError(e);
						}
						continue;
					}

					this.Logger.Error("TransactionDetail.RetrieveLineItem(item) : Field " + fieldKey + " not found.");
				}

				if (noLineItemErrors)
				{
					CurrencyClass currency = null;
					LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];
					SubLineItemDO sublineItem = null;

					if (this.TransContext.Currencies != null)
					{
						currency = new CurrencyClass(this.security) { InventoryDate = this.Trans.InventoryDate };
					}

					this.SetLineItemCurrencyFields(currency, lineItem);

					// If the Alternate Volume field is populate, then convert the value and
					// populate the Gross Volume field.
					if (this.ConvertAlternateVolumeToGrossVolume(lineItem) == false)
					{
						return false;
					}

					try
					{
						FMChannelHelper.MakeCall<IPriceCalculatorInvoker>(
							x => x.CalculateWithLineItems(this.security, this.Trans, origLineItems));
					}
					catch (FMStandingOfferException soException)
					{
						// Standing Offer exception is due to not finding a standing offer (aka price list price) or
						// getting the most recent one. In the former case, we do not want
						// to continue (false). In the other case, we do (true).
						noLineItemErrors = soException.ContinueOn;
						this.HandleFieldError(new Exception(soException.Message));
					}
					catch (Exception ex)
					{
						noLineItemErrors = false;
						this.HandleFieldError(ex);
					}

					// If there are associated transactions, and current transaction is aggregate type, then override 
					// quantity, excise amount, GST amount, markup amount, total value, and "total price with tax"
					// fields with their respective aggregate amounts based on associated transactions.
					if (this.TransContext.aliasClass.AggregateAssociatedTransactions
						&& this.TransContext.aliasClass.AssociatedAliases.Count > 0)
					{
						foreach (AssociatedTxDO atx in lineItem.AssociatedTransactions)
						{
							if (atx.Associated == 1)
							{
								//At least one associated transaction exists. Assign aggregated values.
								this.AggregateAssociatedTxValues(lineItemIndex, false);
								break;
							}
						}
					}

					if (sublineItemIndex > -1)
					{
						sublineItem = lineItem.SubLineItems[sublineItemIndex];
					}

					if (sublineItem == null)
					{
						this.CalculateVolumes(lineItem);

						lineItem.Quantity.IsGrossDirty = false;
						lineItem.Quantity.IsNetDirty = false;
						lineItem.Quantity.IsMassDirty = false;
						lineItem.Quantity.IsPackageDirty = false;
						lineItem.Quantity.IsVcfDirty = false;
					}
					else
					{
						this.CalculateVolumes(lineItem, sublineItem);

						sublineItem.Quantity.IsGrossDirty = false;
						sublineItem.Quantity.IsNetDirty = false;
						sublineItem.Quantity.IsMassDirty = false;
						sublineItem.Quantity.IsPackageDirty = false;
						sublineItem.Quantity.IsVcfDirty = false;
					}

					CalculateDeliveredQty(lineItem);

					// SpecialInstructions
					if (this.Trans.ShipToCompanyGuid != Guid.Empty)
					{
						CompanyClass shipTo =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.security, this.Trans.ShipToCompanyGuid));

						if (lineItem != null && lineItem.ProductGuid != Guid.Empty)
						{
							string specialInstructions;
							Guid prodMapGuid;
							PRODUCT_MAP_TYPE mapType;
							this.GetSpecialInstructions(shipTo, lineItem.ProductGuid, out specialInstructions, out prodMapGuid, out mapType);
							lineItem.SpecialInstructionsNote = specialInstructions;
							lineItem.SpecialInstructionsNoteGuid = prodMapGuid;
							lineItem.SpecialInstructionsNoteProductMapType = mapType;
						}

						if (sublineItem != null && sublineItem.ProductGuid != Guid.Empty)
						{
							string specialInstructions;
							Guid prodMapGuid;
							PRODUCT_MAP_TYPE mapType;
							this.GetSpecialInstructions(shipTo, sublineItem.ProductGuid, out specialInstructions, out prodMapGuid, out mapType);
							sublineItem.SpecialInstructionsNote = specialInstructions;
							sublineItem.SpecialInstructionsNoteGuid = prodMapGuid;
							sublineItem.SpecialInstructionsNoteProductMapType = mapType;
						}
					}

					if (TransactionTypes.T18_SupplyOrder == this.Trans.TransTypeID && lineItem.ProductPrice != null)
					{
						lineItem.ValueRemaining = lineItem.GrossQuantityRemaining * lineItem.ProductPrice.Value;
					}
				}

				return noLineItemErrors;
			}
			catch (Exception e)
			{
				this.HandleFieldError(e);
				return false;
			}
		}
      protected virtual bool UpdateLineItemVolumeAndQuantity(DataGridItem item)
      {
         try
         {
            bool noLineItemErrors = true;

            int lineItemIndex;
            int sublineItemIndex;
            this.GetItemIndices(item, out lineItemIndex, out sublineItemIndex);
            LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];

            this.CalculateVolumes(lineItem, (SubLineItemDO) null);

            CalculateDeliveredQty(lineItem);

            if (TransactionTypes.T18_SupplyOrder == this.Trans.TransTypeID && lineItem.ProductPrice != null)
            {
               lineItem.ValueRemaining = lineItem.GrossQuantityRemaining * lineItem.ProductPrice.Value;
            }
            return noLineItemErrors;
         }
         catch (Exception e)
         {
            this.HandleFieldError(e);
            return false;
         }
      }
 
		/// <summary>
		/// This method handles the reverse button click event.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected virtual void ReverseButtonClick(object sender, EventArgs e)
		{
			this.NoSaveErrors = true;

			if (this.IsTransactionEditable && !this.SaveProcessing(sender))
			{
				return;
			}

			this.ReverseProcessing();
		}

		/// <summary>
		/// The reverse processing.
		/// </summary>
		protected virtual void ReverseProcessing()
		{
			this.Trans.Status = TransactionStatus.Completed;
			this.Trans.ReversalType = TransactionDO.Reversal;
			this.Trans.ReversedTransID = this.Trans.TransID;
			this.Trans.ConjoinReversedTransID = this.Trans.ConjoinedTransID;
			this.Trans.TransID = FuelsManagerId.NewId();
			this.Trans.TransactionGuid = Guid.Empty;
			this.Trans.ConjoinedTransactionGuid = Guid.Empty;
			this.Trans.TransactionNoteGuid = Guid.Empty;
			this.Trans.ConjoinedNotesGuid = Guid.Empty;
			this.Trans.TransactionSignatureGuid = Guid.Empty;
			this.Trans.ConjoinedSignatureGuid = Guid.Empty;
			this.Trans.TransactionUserDataGuid = Guid.Empty;
			this.Trans.ConjoinedUserDataGuid = Guid.Empty;

			if (string.IsNullOrEmpty(this.Trans.ConjoinedTransID) == false)
			{
				this.Trans.ConjoinedTransID = FuelsManagerId.NewId();
			}

			// check the configuration for reverse transaction date mode (25-Jun-2009 IGO)
			var genConfigSr = new GeneralConfigSR
			{
				Security = this.security,
				Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION
			};

			GeneralConfigDO genConfigDO =
				FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(x => x.Get(genConfigSr));

			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
			{
				// For ADF Projects, reversals can NEVER be backdated
				this.Trans.TransactionDateTime = TimeConverter.Now(this.AccountingSite.CurrentSite);
				this.Trans.InventoryDate = TimeConverter.ToDate(this.Trans.TransactionDateTime.Value).Date;
			}
			else
			{
				// Always default to current date
				this.Trans.InventoryDate = genConfigDO.ReverseTransactionDateMode == "Original" ?
												this.Trans.InventoryDate : this.GetCurrentInventoryDate();
			}

			this.Trans.CloseoutDate = null;
			this.Trans.PartialCloseout = false;

			this.SetVolumeSigns(this.Trans, false);

			// Reverse the quantities for both the line items and any
			// sub-line items.
			foreach (LineItemDO lineItem in this.Trans.LineItems)
			{
				lineItem.TransactionLineItemGuid = Guid.Empty;
				lineItem.ConjoinedTransactionLineItemGuid = Guid.Empty;
				lineItem.TransactionLineItemUserDataGuid = Guid.Empty;
				lineItem.ConjoinedTransactionLineItemUserDataGuid = Guid.Empty;
				lineItem.Quantity.GrossInventoryChange *= -1;
				lineItem.Quantity.NetInventoryChange *= -1;
				lineItem.Quantity.MassInventoryChange *= -1;
				lineItem.Quantity.PackageInventoryChange *= -1;
				lineItem.CloseoutDate = null;

				foreach (SubLineItemDO sublineItem in lineItem.SubLineItems)
				{
					sublineItem.TransactionSubLineItemGuid = Guid.Empty;
					sublineItem.ConjoinedTransactionSubLineItemGuid = Guid.Empty;
					sublineItem.Quantity.GrossInventoryChange *= -1;
					sublineItem.Quantity.NetInventoryChange *= -1;
					sublineItem.Quantity.MassInventoryChange *= -1;
					sublineItem.Quantity.PackageInventoryChange *= -1;
					sublineItem.CloseoutDate = null;
				}
			}

			foreach (TransportLineItemDO transportLineItem in this.Trans.TransportInfoList)
			{
				transportLineItem.TransactionTransportLineItemGuid = Guid.Empty;
				transportLineItem.ConjoinedTransactionTransportLineItemGuid = Guid.Empty;
			}

			if (this.Trans.TransPIDXCollection != null)
			{
				foreach (TransactionPIDXDO transactionPidxDo in this.Trans.TransPIDXCollection)
				{
					transactionPidxDo.SentFlag = false;
					transactionPidxDo.AuthorizationNumber = string.Empty;
				}
			}

			this.Save();
			this.SetVolumeSigns(this.Trans, true);
			this.SetButtons();
			this.RegenerateControls(true);

			if (this.TransContext.aliasClass.MultipleLineItems)
			{
				this.LineItemGridGenerator.Bind();
			}

			if (this.TransContext.aliasClass.MultipleWeightReadings)
			{
				this.AgrGridGenerator.Bind();
			}

			if (this.TransContext.aliasClass.MultipleTransportLineItems)
			{
				this.TransportInfoGridGenerator.Bind();
			}

			// Update the UpdatedBy and UpdatedDate fields with the current values.
			this.LastUpdatedFields();
		}

		/// <summary>
		/// This method handles the reverse update button click event.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected virtual void ReverseUpdateButtonClick(object sender, EventArgs e)
		{
			this.NoSaveErrors = true;

			if (this.IsTransactionEditable && !this.SaveProcessing(sender))
			{
				return;
			}

			this.ReverseUpdateProcessing();
		}

		/// <summary>
		/// The reverse update processing.
		/// </summary>
		protected virtual void ReverseUpdateProcessing()
		{
			this.TransContext.mode = TransactionContext.Mode.Add;
			this.Session[ModeKey] = this.TransContext.mode;

			this.Trans.Status = TransactionStatus.Completed;
			this.Trans.ReversalType = TransactionDO.Update;
			this.Trans.ReversedTransID = this.Trans.TransID;
			this.Trans.ConjoinReversedTransID = this.Trans.ConjoinedTransID;
			this.Trans.TransID = FuelsManagerId.NewId();
			this.Trans.LinkedDocumentNumber = null;
			this.Trans.TransactionGuid = Guid.Empty;
			this.Trans.ConjoinedTransactionGuid = Guid.Empty;
			this.Trans.TransactionNoteGuid = Guid.Empty;
			this.Trans.ConjoinedNotesGuid = Guid.Empty;
			this.Trans.TransactionSignatureGuid = Guid.Empty;
			this.Trans.ConjoinedSignatureGuid = Guid.Empty;
			this.Trans.TransactionUserDataGuid = Guid.Empty;
			this.Trans.ConjoinedUserDataGuid = Guid.Empty;

			if (string.IsNullOrEmpty(this.Trans.ConjoinedTransID) == false)
			{
				this.Trans.ConjoinedTransID = FuelsManagerId.NewId();
			}

			// check the configuration for reverse transaction date mode (25-Jun-2009 IGO)
			var genConfigSr = new GeneralConfigSR
			{
				Security = this.security,
				Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION
			};

			GeneralConfigDO genConfigDO =
				FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(x => x.Get(genConfigSr));

			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
			{
				// For ADF Projects, reversals can NEVER be backdated
				this.Trans.TransactionDateTime = TimeConverter.Now(this.AccountingSite.CurrentSite);
				this.Trans.InventoryDate = TimeConverter.ToDate(this.Trans.TransactionDateTime.Value).Date;
			}
			else
			{
				if (genConfigDO.ReverseTransactionDateMode == "Original")
				{
					this.Trans.InventoryDate = this.Trans.InventoryDate;
				}
				else
				{
					// always default to current date
					this.Trans.InventoryDate = this.GetCurrentInventoryDate();
				}
			}

			this.Trans.CloseoutDate = null;
			this.Trans.PartialCloseout = false;

			foreach (LineItemDO lineItemDO in this.Trans.LineItems)
			{
				lineItemDO.CloseoutDate = null;
				lineItemDO.TransactionLineItemGuid = Guid.Empty;
				lineItemDO.ConjoinedTransactionLineItemGuid = Guid.Empty;
				lineItemDO.TransactionLineItemUserDataGuid = Guid.Empty;
				lineItemDO.ConjoinedTransactionLineItemUserDataGuid = Guid.Empty;

				foreach (SubLineItemDO subLineItemDO in lineItemDO.SubLineItems)
				{
					subLineItemDO.CloseoutDate = null;
					subLineItemDO.TransactionSubLineItemGuid = Guid.Empty;
					subLineItemDO.ConjoinedTransactionSubLineItemGuid = Guid.Empty;
				}
			}


			foreach (TransportLineItemDO transportLineItem in this.Trans.TransportInfoList)
			{
				transportLineItem.TransactionTransportLineItemGuid = Guid.Empty;
				transportLineItem.ConjoinedTransactionTransportLineItemGuid = Guid.Empty;
			}

			// Just clear the entire PIDX collection; Save() will recreate it properly.
			this.Trans.TransPIDXCollection = null;
			this.SetButtons();
			this.RegenerateControls(true);

			this.LineItemGridGenerator?.Bind();

			this.AgrGridGenerator?.Bind();

			this.TransportInfoGridGenerator?.Bind();

			// Update the UpdatedBy and UpdatedDate fields with the current values.
			this.LastUpdatedFields();
		}

		/// <summary>
		///	This method handles the process of saving a transaction to the database. It calls the
		///	accounting BLL with the data to be saved.
		/// </summary>
		/// <returns></returns>
		protected virtual bool Save()
		{
			var timer = new StopWatch(StopWatch.Appnames.Accounting, "Save() - ValidateCurrencyFields()");
			bool saveSuccessful = false;
			string errorMsg = this.ValidateCurrencyFields(null);
			timer.Stop();

			this.Trans.UpdatedBy = this.security.UserID;
			this.Trans.UpdatedDate = DateTimeOffset.Now;

			// Set null GrossManualValueFlag to true so that initial Quantity value of 0 does not appear greyed out
			foreach (LineItemDO lineItem in this.Trans.LineItems)
			{
				if (lineItem.Quantity?.NullableGross != null && !lineItem.Quantity.GrossManualValueFlag.HasValue)
				{
					lineItem.Quantity.GrossManualValueFlag = true;
				}
			}

			if (errorMsg != null)
			{
				this.HandleFieldError(new RetrieveException(errorMsg));
				return false;
			}

			try
			{
				// Enforce the system lock dates here rather than in IsTransactionEditable
				if (this.Trans.SiteGuid != Guid.Empty)
				{
					const bool GetSchedulesFlag = false;
					const bool GetMemberSites = false;
					const bool GetAssociatedAliases = false;

					SiteClass site;
					if (this.Trans.SiteGuid == this.AccountingSite.CurrentSiteGuid)
					{
						site = this.AccountingSite.CurrentSite;
					}
					else if (this.Trans.SiteGuid == this.AccountingSite.LoginSite.IdentityGuid)
					{
						site = this.AccountingSite.LoginSite;
					}
					else
					{
						site = FMChannelHelper.MakeCall<ISites, SiteClass>(
												x =>
									x.Get(this.security, this.Trans.SiteGuid, GetMemberSites, GetSchedulesFlag, GetAssociatedAliases)
							);
					}

					// CSI 5825 - The inventory date must be after the administrative lock date.
					if (this.Trans.InventoryDate <= site._AdministrativeLockDate.Value)
					{
						throw new Exception("Inventory date must be after the Administrative Lock Date.");
					}

					// The user must not have the Perform Closeout and the Configure Accounting
					// right for the operational lock date to take effect. If the user has either
					// one or the other, then the operational lock date check is ignored.
					if ((this.security.HasRight(RIGHT.CONFIGURE_ACCOUNTING) == false)
						&& (this.security.HasRight(RIGHT.PERFORM_CLOSEOUT) == false)
						&& (this.Trans.InventoryDate <= site._OperationalLockDate.Value))
					{
						throw new Exception("Inventory date must be after the Operational Lock Date.");
					}
				}
			}
			catch (Exception e)
			{
				string msg = "<script type=\"text/javascript\">\r\n<!--\r\nalert(\"" + HttpUtility.JavaScriptStringEncode(e.Message)
								+ "\");\r\n-->\r\n</script>";
				ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SaveTransactionFailure", msg, false);
				this.Logger.Error(msg);
				return false;
			}

			if (this.IsOrderAssociatedTransaction(this.Trans))
			{
				if (string.IsNullOrEmpty(this.Trans.TransRefID))
				{
					this.Trans.TransRefID = this.orderTxReferenceID;
				}
				else if (string.IsNullOrEmpty(this.orderTxReferenceID))
				{
					this.orderTxReferenceID = this.Trans.TransRefID;
				}

				// We need to link up the line items to the associated Order line items
				this.LinkUpOrderLineItems();
			}

			// Add new PIDX record for BOL transaction	
			if ((this.Trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
				|| this.Trans.TransTypeID == TransactionTypes.T25_Shipment) && this.Trans.TransPIDXCollection == null)
			{
				Guid loadIDCompanyMapGuid = Guid.Empty;

				if (this.Trans.ShipToCompanyGuid != Guid.Empty && this.Trans.BillToCompanyGuid != Guid.Empty)
				{
					CompanyMapCollectionClass shipToBillToMaps =
						FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
							x =>
							x.EnumerateByAssignedGuidAndType(this.security, this.Trans.ShipToCompanyGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP));

					foreach (CompanyMapClass shipToBillToMap in shipToBillToMaps)
					{
						CompanyMapClass billToShipperMap =
							FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
								x => x.Get(this.security, shipToBillToMap.AssignedToGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP));

						if (billToShipperMap.AssignedGuid == this.Trans.BillToCompanyGuid)
						{
							CompanyMapCollectionClass loadIDToShipToMaps =
								FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
									x =>
									x.EnumerateByAssignedToGuidAndType(
										this.security, shipToBillToMap.IdentityGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP));

							foreach (CompanyMapClass loadIDToShipToMap in loadIDToShipToMaps)
							{
								// {All} Drivers
								if (loadIDToShipToMap.AssignedGuid == Guid.Empty)
								{
									this.Trans.LoadID = loadIDToShipToMap.MapID;
									loadIDCompanyMapGuid = loadIDToShipToMap.IdentityGuid;
									break;
								}
								// Driver Specific LoadID
								if (this.Trans.OperatorPersonnelGuid != Guid.Empty
									&& this.Trans.OperatorPersonnelGuid == loadIDToShipToMap.AssignedGuid)
								{
									this.Trans.LoadID = loadIDToShipToMap.MapID;
									loadIDCompanyMapGuid = loadIDToShipToMap.IdentityGuid;
									break;
								}
							}
						}
					}
				}

				if (loadIDCompanyMapGuid != Guid.Empty)
				{
					this.Trans.TransPIDXCollection = new List<TransactionPIDXDO>();

					PIDXProfileCollectionClass pidxProfileCollection =
						FMChannelHelper.MakeCall<IPIDXProfiles, PIDXProfileCollectionClass>(x => x.Enumerate(this.security));

					PIDXProfileCompanyMapCollectionClass pidxProfileCompanyMapCollection =
						FMChannelHelper.MakeCall<IPIDXProfileCompanyMaps, PIDXProfileCompanyMapCollectionClass>(
							x => x.EnumerateSiteAndCompanyPersonnelToShipToBillToGuid(this.security, loadIDCompanyMapGuid));

					foreach (PIDXProfileCompanyMapClass pidxProfileCompanyMap in pidxProfileCompanyMapCollection)
					{
						PIDXProfileClass pidxProfile = pidxProfileCollection.Find(pidxProfileCompanyMap.PIDXProfileGuid);

						if (pidxProfile == null || !pidxProfile.Enabled)
						{
							continue;
						}

						var transactionPidxDo = new TransactionPIDXDO
						{
							PIDXProfileGuid = pidxProfileCompanyMap.PIDXProfileGuid,
							CompanyPersonnelToShipToBillToGuid =
																pidxProfileCompanyMap.CompanyPersonnelToShipToBillToGuid,
							BOLVersion = (int)pidxProfile.Version
						};

						this.Trans.TransPIDXCollection.Add(transactionPidxDo);
					}
				}
			}

			// Update the PIDX collection if it already exists for this transaction.
			if (null != this.Trans.TransPIDXCollection)
			{
				bool brokenBlend = false;

				foreach (LineItemDO lineItem in this.Trans.LineItems)
				{
					if (lineItem.BrokenBlend != null && lineItem.BrokenBlend.Value)
					{
						brokenBlend = true;
						break;
					}
				}

				foreach (TransactionPIDXDO transPidxDO in this.Trans.TransPIDXCollection)
				{
					transPidxDO.BrokenBlend = brokenBlend;
				}
			}

			// Correct volume signs for saving to database.
			this.SetVolumeSigns(this.Trans, false);

			// If SCAC is empty, Carrier is not empty, and Carrier has a SCAC, then store that SCAC in the transaction
			if (string.IsNullOrEmpty(this.Trans.SCACCode))
			{
				if (this.Trans.CarrierCompanyGuid != Guid.Empty)
				{
					CompanyClass carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.security, this.Trans.CarrierCompanyGuid));
					//companies.Get(this.security, (int)this.trans.CarrierIndex.Value);
					if (carrier != null)
					{
						this.Trans.SCACCode = carrier.SCACCode;
					}
				}
			}

			// If driver ID number (card/shortcard number) is empty, fill it from the operator record (if present)
			if (string.IsNullOrEmpty(this.Trans.DriverIDNumber))
			{
				if (this.Trans.OperatorPersonnelGuid != Guid.Empty)
				{
					var driver = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(personnel => personnel.Get(this.security, this.Trans.OperatorPersonnelGuid));
					if (driver != null)
					{
						this.Trans.DriverIDNumber = this.AccountingSite.CurrentSite.UseShortCardNumber
													? driver.ShortCardNumber
													: driver.CardNumber;
					}
				}
			}

			this.SetLineItemEquipment();

			var sr = new SaveTransactionsSR
			{
				Security = this.security,
				CurrentSiteGuid = this.security.SiteGuid,
				AccountingSite = this.AccountingSite
			};

			// Used during the transaction validation process in the save transaction processor.
			this.Trans.PermitNonReferenceData = this.TransContext.aliasClass.PermitNonReferenceData;

			sr.Transactions.Add(this.Trans);

			timer.ActionName = "Save() - Combine Transaction Processing";
			timer.Start();

			// Test for Combine Transaction
			if (this.Session[CombineTransKey] != null)
			{
				TransactionDO combineTrans = this.LoadTransaction(this.Session[CombineTransKey] as string);

				combineTrans.Status = TransactionStatus.Completed;
				combineTrans.ReversalType = TransactionDO.Reversal;
				combineTrans.ReversedTransID = combineTrans.TransID;
				combineTrans.ConjoinReversedTransID = combineTrans.ConjoinedTransID;

				// Clearing the guids so they are not copied when the new
				// transaction (the reversal of the source) is created 
				combineTrans.TransID = FuelsManagerId.NewId();
				combineTrans.TransactionGuid = Guid.NewGuid();
				combineTrans.TransactionUserDataGuid = Guid.NewGuid();
				combineTrans.TransactionSignatureGuid = Guid.NewGuid();
				combineTrans.TransactionNoteGuid = Guid.NewGuid();

				foreach (LineItemDO lineItem in combineTrans.LineItems)
				{
					lineItem.TransactionLineItemGuid = Guid.NewGuid();
					lineItem.TransactionLineItemUserDataGuid = Guid.NewGuid();

					foreach (var subLineItem in lineItem.SubLineItems)
					{
						subLineItem.TransactionSubLineItemGuid = Guid.NewGuid();
					}
				}

				foreach (var transportLineItem in combineTrans.TransportInfoList)
				{
					transportLineItem.TransactionTransportLineItemGuid = Guid.NewGuid();
				}

				if (!string.IsNullOrEmpty(combineTrans.ConjoinedTransID))
				{
					combineTrans.ConjoinedTransID = FuelsManagerId.NewId();
				}

				// check the configuration for reverse transaction date mode (25-Jun-2009 IGO)
				var genConfigSR = new GeneralConfigSR
				{
					Security = this.security,
					Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION
				};

				GeneralConfigDO genConfigDO =
					FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(x => x.Get(genConfigSR));

				if (genConfigDO.ReverseTransactionDateMode == "Original")
				{
					combineTrans.InventoryDate = combineTrans.InventoryDate;
				}
				else
				{
					combineTrans.InventoryDate = this.GetCurrentInventoryDate();
				}

				combineTrans.CloseoutDate = null;
				combineTrans.PartialCloseout = false;

				// Reverse the quantities for both the line items and any
				// sub-line items.
				foreach (LineItemDO lineItem in combineTrans.LineItems)
				{
					lineItem.Quantity.GrossInventoryChange *= -1;
					lineItem.Quantity.NetInventoryChange *= -1;
					lineItem.Quantity.MassInventoryChange *= -1;
					lineItem.CloseoutDate = null;

					foreach (SubLineItemDO sublineItem in lineItem.SubLineItems)
					{
						sublineItem.Quantity.GrossInventoryChange *= -1;
						sublineItem.Quantity.NetInventoryChange *= -1;
						sublineItem.Quantity.MassInventoryChange *= -1;
						sublineItem.CloseoutDate = null;
					}
				}

				if (combineTrans.TransPIDXCollection != null)
				{
					foreach (TransactionPIDXDO transactionPidxDo in combineTrans.TransPIDXCollection)
					{
						transactionPidxDo.SentFlag = false;
						transactionPidxDo.AuthorizationNumber = string.Empty;
					}
				}

				//SetVolumeSigns(combineTrans, false);

				sr.Transactions.Add(combineTrans);
			}

			timer.Stop();
			timer.Start("Save() - Actual save call processing");
			try
			{
				// First attempt to save the Transaction through Load Rack Manager
				try
				{
					var serviceController = new ServiceController("FuelsManager Terminal Automation");
					if (!this.WasServiceFound(serviceController))
					{
						serviceController = null;
					}

					// vthompson 10-27-2008
					// Changed this to only call the load rack if the site is not a site group
					if (this.AccountingSite.CurrentSite.SiteGroup || serviceController == null
						|| serviceController.Status != ServiceControllerStatus.Running
						|| ((this.Trans.TransTypeID != TransactionTypes.T3_PrimaryDefuel
								&& this.Trans.TransTypeID != TransactionTypes.T5_PrimaryDisbursement
								&& this.Trans.TransTypeID != TransactionTypes.T25_Shipment)
							|| (this.Trans.Status != TransactionStatus.InProgress && this.Trans.Status != TransactionStatus.LoadPending)))
					{
						SaveTransactionsResultDO resultDO =
							FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(x => x.SaveTransactions(sr));

						this.CheckForAndDisplayWarningMessagesInternal(resultDO);
						saveSuccessful = true;
					}
					else
					{
						if (UsingLoadRack)
						{
							ILoadRackManager loadRackManager = this.GetLoadRackManager();
							SaveTransactionsResultDO resultDO = loadRackManager.AccountingRequest(sr);
							this.CheckForAndDisplayWarningMessagesInternal(resultDO);
							saveSuccessful = true;
						}
						// alternateLoad
					}
				}
				catch (SocketException)
				{
					// vthompson 10/15/2008
					// Originally the exception message was checked to determine if connectivity to the load rack service
					// failed.  The message changed in .NET 2.0 so this design was changed.

					// alternateLoad
					SaveTransactionsResultDO resultDo2 =
						FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(x => x.SaveTransactions(sr));

					this.CheckForAndDisplayWarningMessagesInternal(resultDo2);
					saveSuccessful = true;
				}
			}
			catch (FaultException<SaveTransactionsException> e)
			{
				// Data dictionary the error message.
				string msg = GetDataDictionaryValueByKey(this.AccountingSite.CurrentSiteGuid, "Save transaction failed");
				msg = msg + "! ";
				string alertMsg = msg;

				foreach (TransactionValidationResult result in e.Detail.Results)
				{
					foreach (string error in result.ErrorList)
					{
						msg += "\n\r" + error;
						alertMsg += "\n" + error;
					}
				}

				string alertString = "<script type=\"text/javascript\">\r\n<!--\r\nalert(\""
											+ HttpUtility.JavaScriptStringEncode(alertMsg) + "\");\r\n-->\r\n</script>";

				ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SaveTransactionFailed", alertString, false);

				msg = "TransactionDetail.Save() : \n\r" + msg;
				this.Logger.Error(msg);
			}
			catch (Exception e)
			{
				this.ErrorHandler(e);
			}
			finally
			{
				timer.Stop();
			}

			// Convert the volume signs back for displaying.
			this.SetVolumeSigns(this.Trans, true);

			//Save meter readings for the most specific equipment for each line item and subline item.
			if (saveSuccessful && (this.TransContext.aliasClass.MeterCloseout == false))
			{
				string eqID = null;
				var meterReadings = this.Session[MeterReadingsKey] as Hashtable ?? new Hashtable();
				foreach (LineItemDO lineItem in this.Trans.LineItems)
				{
					if (lineItem.MeterReading.MeterStop != null)
					{
						if (EquipmentMeterMap.GetMeterEquipment(this.Trans.TransTypeID) == EquipmentMeterMap.MeterEquipment.Destination)
						{
							eqID = this.GetSpecificEquipmentID(lineItem.DestinationEQ.RegistrationID);
						}
						else if (EquipmentMeterMap.GetMeterEquipment(this.Trans.TransTypeID) == EquipmentMeterMap.MeterEquipment.Source)
						{
							eqID = this.GetSpecificEquipmentID(lineItem.SourceEQ.RegistrationID);
						}

						if (eqID != null)
						{
							if (meterReadings.ContainsKey(eqID))
							{
								meterReadings.Remove(eqID);
							}
							meterReadings.Add(eqID, lineItem.MeterReading.MeterStop.Value);
						}
					}
				}

				this.Session.Add(MeterReadingsKey, meterReadings);
			}

			if (saveSuccessful)
			{
				timer.Start("Save() - Loading transaction back");
				this.Trans = this.LoadTransaction(this.Trans.TransID);
				this.Session[TransKey] = this.Trans;
				this.Session.Remove("allAssociatedTransactionsBeforeTransactionEdit");
				this.Session.Remove(CombineTransKey);
				timer.Stop();

				// There are 3 quanity fields (gross, net, mass), density, and VCF which are previously
				// calculated. This call will populate the calculated fields when the application
				// is configured for the non-multi line item mode.
				this.SetCalulatedQuantitiesWhenInSingleLineMode();

				// Update the UpdatedBy and UpdatedDate fields with the current values.
				this.LastUpdatedFields();

				// Set the LEDGER_PRODUCT_SELECTION and TransactionListContext Session keys if the product has changed
				var productName = this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] as string;
				var transactionListContext = this.Session["TransactionListContext"] as TransactionListContext;

				foreach (LineItemDO lineItem in this.Trans.LineItems)
				{
					if (!string.IsNullOrEmpty(lineItem.Product))
					{
						if (productName != lineItem.Product)
						{
							this.Session[PageSessionKeyConstants.LEDGER_PRODUCT_SELECTION] = lineItem.Product;
						}
						if (transactionListContext != null && transactionListContext.Product != lineItem.Product)
						{
							transactionListContext.Product = lineItem.Product;
							this.Session["TransactionListContext"] = transactionListContext;
						}
						break;
					}
				}

				if (this.Trans.Status == TransactionStatus.Completed || this.Trans.Status == TransactionStatus.Posted)
				{
					switch (this.Trans.TransTypeID)
					{
						case TransactionTypes.T5_PrimaryDisbursement:
							this.RenderAndArchiveReportInPDF();
							break;

						case TransactionTypes.T8_Receipt:
							try
							{
								ILoadRackManager loadRackManager = this.GetLoadRackManager();
								foreach (LineItemDO lineItem in this.Trans.LineItems)
								{
									loadRackManager.ResetOwnerAllocationsForSingleProduct(this.security, lineItem.Product);
								}
							}
							catch (SocketException except)
							{
								// Error code 10061 means the Loadrack manager is not installed. 
								// Therefore, ignore the exception. See method
								// FMFormBase.GetLoadRackManager().
								if (except.ErrorCode != 10061)
								{
									this.ErrorHandler(except);
								}
							}

							break;
					}
				}
				//disable the BOL Number/DocumentNumber textbox after saving
				var documentNumberTextBox = this.FindControl("TransactionFields.DocumentNumberFG") as TextBox;
				if (documentNumberTextBox != null)
				{
					documentNumberTextBox.Enabled = false;
				}
			}

			return saveSuccessful;
		}

		private void RenderAndArchiveReportInPDF()
		{
			try
			{
				if (this.Trans.SiteGuid != Guid.Empty)
				{
					const bool GetSchedulesFlag = false;
					const bool GetMemberSites = false;
					const bool GetAssociatedAliases = false;

					SiteClass site =
								FMChannelHelper.MakeCall<ISites, SiteClass>(
									x => x.Get(this.security, this.Trans.SiteGuid, GetMemberSites, GetSchedulesFlag, GetAssociatedAliases));

					if (!site.EnableBOLPDFArchiving) return;


					EventLog eventLog = new EventLog("Application", ".", "FuelsManager");
					//reveral section
					string reversal = this.Trans.ReversalType;
					if (this.Trans.ReversalType == TransactionDO.None)
					{
						reversal = "None";
					}

					//parameters section
					FMBusinessObjects.ReportSvr2005.ParameterValue[] parameterValues = new FMBusinessObjects.ReportSvr2005.ParameterValue[3];

					parameterValues[0] = new FMBusinessObjects.ReportSvr2005.ParameterValue { Name = "TransID", Value = this.Trans.TransID };
					parameterValues[1] = new FMBusinessObjects.ReportSvr2005.ParameterValue
					{
						Name = "SiteGuid",
						Value = site.SiteGuid.ToString()
					};
					parameterValues[2] = new FMBusinessObjects.ReportSvr2005.ParameterValue { Name = "FromStation", Value = "True" };

					ReportServicePrintService printService =
							new ReportServicePrintService(eventLog)
							{
								BOLPDFArchivingPath = site.BOLPDFArchivingPath,
								BOLPDFArchivingFileName = site.Number + "." + this.Trans.DocumentNumber + "." + DateTime.Now.ToString("yyyyMMdd.HHmmss") + "." + reversal + ".pdf",
								ReportName = site.ReportDirectory + "/" + this.TransContext.aliasClass.AssociatedReport,
								ParameterValues = parameterValues,
								Security = this.security
							};

					printService.ArchiveReport();

					//Now retrieve original or originalupdate tranaction
					if ((this.Trans.ReversalType != TransactionDO.Reversal && this.Trans.ReversalType != TransactionDO.Update)
					|| string.IsNullOrEmpty(this.Trans.ReversedTransID))
					{
						return;
					}

					TransactionDO original = this.LoadTransaction(this.Trans.ReversedTransID);
					parameterValues[0] = new FMBusinessObjects.ReportSvr2005.ParameterValue { Name = "TransID", Value = original.TransID };
					printService.BOLPDFArchivingFileName = site.Number + "." + original.DocumentNumber + "." + DateTime.Now.ToString("yyyyMMdd.HHmmss") + "." + original.ReversalType + ".pdf";
					printService.ArchiveReport();

					if (this.Trans.ReversalType != TransactionDO.Update || string.IsNullOrEmpty(this.Trans.ReversedTransID))
					{
						return;
					}

					var getTransactionSR = new GetTransactionSR
					{
						Security = this.security,
						Request = GetTransactionRequest.SITE_TYPEID_REVERSEDTRANSID,
						ReversedTransID = this.Trans.ReversedTransID,
						TransTypeID = this.Trans.TransTypeID
					};

					string reversalTransID = null;
					string reversalTransType = null;
					GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(x => x.Process(getTransactionSR));
					if (getTransactionDO != null && getTransactionDO.TransactionDataSet != null
							&& getTransactionDO.TransactionDataSet.Tables.Count == 1)
					{
						foreach (DataRow row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
						{
							// Select the Reversal associated with the update
							if ((row["ReversalType"] != null
									&& (row["ReversalType"] as string == TransactionDO.Reversal
										|| row["ReversalType"] as string == TransactionDO.ReversalWithUpdate))
							&& row["ReversedTransID"] != null
							&& row["ReversedTransID"] as string == this.Trans.ReversedTransID)
							{
								reversalTransID = row["TransID"] as string;
								reversalTransType = row["ReversalType"] as string;
								break;
							}
						}
					}
					if (!string.IsNullOrEmpty(reversalTransID))
					{
						parameterValues[0] = new FMBusinessObjects.ReportSvr2005.ParameterValue { Name = "TransID", Value = reversalTransID };
						printService.BOLPDFArchivingFileName = site.Number + "." + this.Trans.DocumentNumber + "." + DateTime.Now.ToString("yyyyMMdd.HHmmss") + "." + reversalTransType + ".pdf";
						printService.ArchiveReport();
					}

				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

		}

		/// <summary>
		///	This method handles the save button event (pressing the save button).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void SaveButtonClick(object sender, EventArgs e)
		{
			this.NoSaveErrors = true;

			if (this.Session["TransactionDetailFieldError"] != null)
			{
				this.UpdatePanel1.Update();
				this.Session.Remove("TransactionDetailFieldError");
			}

			if (this.SaveProcessing(sender))
			{
				this.PerformDrawdown();
				this.SaveButton.Enabled = false;
				this.RefreshSystemFields();
			}
		}


		/// <summary>
		/// Refresh the display value of the fields that are not set by the user, but by the service layer and/or the database.
		/// </summary>
		private void RefreshSystemFields()
		{
			if (this.Trans == null)
			{
				return;
			}

			var createdDateFg = (CreatedDateFG)this.TransactionFieldGenerator.GetFieldGenerator("CreatedDate");

			createdDateFg?.SetDisplayValue(this.Trans.CreatedDate);

			var createdByFg = (CreatedByFG)this.TransactionFieldGenerator.GetFieldGenerator("CreatedBy");

			createdByFg?.SetDisplayValue(this.Trans.CreatedBy);

			DocumentNumberFG fg = (DocumentNumberFG)this.TransactionFieldGenerator.GetFieldGenerator("DocumentNumber");
			if (fg != null)
			{
				try
				{
					fg.SetDisplayValue(this.Trans.DocumentNumber);
				}
				catch (NullReferenceException)
				{
					// This will happen if the Type 5 transaction alias does not include Document Number for display.
					// Reasonably possible; just eat the exception.
				}
			}
		}

		/// <summary>
		///	This method will orchestrate the retrieving of the data from the page and
		///	saving it to the database.
		///	///
		/// </summary>
		/// <param name="sender"></param>
		/// <returns></returns>
		protected virtual bool SaveProcessing(object sender)
		{
			var timer = new StopWatch(StopWatch.Appnames.Accounting, "SaveProcessing() - RetrieveDataFromPage()");

			// When overriding this method we sometimes do not want to retrieve
			// from the page since it is already done.
			if (this.RetrieveDataFromPageFlag == true)
			{
				// Retrieve all the data from the page.
				this.NoSaveErrors = this.RetrieveDataFromPage();
				timer.Stop();
			}

			// Save the data to the database
			if (this.NoSaveErrors)
			{
				timer.ActionName = "SaveProcessing() - Save()";
				timer.Start();
				bool successfulSave = this.Save();
				timer.Stop();

				if (successfulSave == true)
				{
					this.TransContext.mode = TransactionContext.Mode.Edit;
					this.Session.Add(TransactionDetailBase.ModeKey, this.TransContext.mode);
					this.SetButtons();
				}
				else
				{
					return false;
				}
			}
			return this.NoSaveErrors;
		}

		protected virtual void SetButtonsCustomized(bool isEditable)
		{

		}

		/// <summary>
		///	This method will set the buttons on the page to a predetermined state.
		/// </summary>
		protected void SetButtons()
		{
			// If the page is not in edit mode for a line item, then set the buttons to their normal
			// state. If they are in edit mode, then set the buttons to the edit mode state.
			if (this.InEditMode() == false && this.Trans != null)
			{
				bool editable = this.IsTransactionEditable;
				bool hasEditableLineItem = false;

				foreach (LineItemDO lineItem in this.Trans.LineItems)
				{
					if (lineItem == null)
					{
						continue;
					}
					hasEditableLineItem |= this.IsLineItemEditable(lineItem);
				}
				this.SetNextPrevious();
				this.SetSaveButton(editable | hasEditableLineItem);
				this.SetReverseButton(editable | hasEditableLineItem);
				this.SetReverseUpdateButton(editable | hasEditableLineItem);
				this.SetDeleteButton(editable | hasEditableLineItem);
				this.SetNewButton(editable | hasEditableLineItem);
				this.SetViewPrintable();
				this.SetCloseButton();
				this.SetCombineButton();
				this.SetButtonsCustomized(editable | hasEditableLineItem);
			}
			else
			{
				this.DisableButtonsForEditing();
			}

			if (this.MainButtonPanel != null)
			{
				this.MainButtonPanel.Update();
			}
		}

		protected virtual void SetLineItemCurrencyFields(CurrencyClass currency, LineItemDO lineItem)
		{
			if (lineItem.CurrencyGuid == Guid.Empty)
			{
				lineItem.ExchangeRate = null;
				return;
			}

			Guid currencyGuid = lineItem.CurrencyGuid;
			if (currencyGuid == Guid.Empty)
			{
				lineItem.ExchangeRate = null;
				return;
			}

			if (currency != null)
			{
				foreach (CurrencyDO currencyDO in this.TransContext.Currencies)
				{
					if (currencyDO.IdentityGuid == lineItem.CurrencyGuid)
					{
						currency.CurrencyGuid = currencyDO.IdentityGuid;

						if (lineItem.NonDomesticPrice == null)
						{
							currency.NonDomesticPrice = 0.0;
						}
						else
						{
							currency.NonDomesticPrice = lineItem.NonDomesticPrice.Value;
						}

						lineItem.ProductPrice = currency.Price;

						lineItem.ExchangeRate = currency.ExchangeRate;

						return;
					}
				}
			}
		}

		/// <summary>
		///	This method sets the state of the New button based on the line
		///	items having the required fields populated.
		/// </summary>
		protected virtual void SetNewButton(bool editable)
		{
			if (this.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false)
			{
				this.NewButton.Enabled = false;
				return;
			}

			if (editable == false)
			{
				this.NewButton.Enabled = false;
			}
			else
			{
				if (this.Trans.DeleteFlag)
				{
					this.NewButton.Enabled = false;
				}
				else
				{
					// Make sure that there are line items before 
					// allowing the save button to be enabled.
					if (this.Trans.LineItems.Count > 0 || this.TransContext.aliasClass.LineItemFieldCollection.Count == 0)
					{
						this.NewButton.Enabled = true;

						// If the Tranport data grid is also configured, check to ensure
						// it has line items prior to setting the apply button.
						if (this.TransportDataGrid.Visible)
						{
							if (this.Trans.TransportInfoList.Count <= 0)
							{
								this.NewButton.Enabled = false;
							}
						}
					}
					else
					{
						this.NewButton.Enabled = false;
					}
				}
			}

			if (this.Trans.TransTypeID == TransactionTypes.T1_PrimaryAdjustment)
			{
				this.NewButton.Enabled = this.SaveButton.Enabled && this.security.HasModifyTransactionRightByAliasName(this.Trans.Alias);
			}

			if ((editable == false) || (this.TransContext.aliasClass.MultipleLineItems == false))
			{
				this.NewLineItemButton.Visible = false;
			}
			else
			{
				this.NewLineItemButton.Visible = true;
			}

			if ((editable == false) || (this.TransContext.aliasClass.MultipleWeightReadings == false))
			{
				this.NewAGRButton.Visible = false;
			}
			else
			{
				// Do not display the Weight Reading add button if the only two columns are
				// Edit and Delete.
				if (this.GaugeReadingsDataGrid != null && this.GaugeReadingsDataGrid.Columns.Count <= 2)
				{
					this.NewAGRButton.Visible = false;
				}
				else
				{
					this.NewAGRButton.Visible = true;
				}
			}

			if ((editable == false) || (this.TransContext.aliasClass.MultipleTransportLineItems == false))
			{
				this.NewTransportButton.Visible = false;
			}
			else
			{
				if (this.TransportDataGrid.Columns.Count > 2)
				{
					this.NewTransportButton.Visible = true;
				}
			}
		}

		/// <summary>
		/// The set reverse button.
		/// </summary>
		/// <param name="editable">
		/// The editable.
		/// </param>
		protected virtual void SetReverseButton(bool editable)
		{
			this.ReverseButton.Enabled = true;

			if (editable)
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			if (this.Trans.DeleteFlag)
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			if (this.TransContext.mode == TransactionContext.Mode.Add || this.bTransIDBeingLoaded)
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			// No reverse for Orders
			if ((this.Trans.TransTypeID == TransactionTypes.T17_Order)
				|| (this.Trans.TransTypeID == TransactionTypes.T18_SupplyOrder))
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			if (this.Trans.ReversalType != TransactionDO.None
				&& this.Trans.ReversalType != TransactionDO.Update
				&& !string.IsNullOrEmpty(this.Trans.ReversalType))
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			// check the configuration for reverse transaction date mode if the transaction is closed (26-Jun-2009 IGO)
			if (this.Trans.CloseoutDate != null || this.Trans.PartialCloseout)
			{
				var genConfigSr = new GeneralConfigSR
				{
					Security = this.security,
					Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION
				};

				GeneralConfigDO genConfigDO =
					FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(x => x.Get(genConfigSr));

				// disable the button, reversals cannot be applied in closed periods (26-Jun-2009 IGO)
				if (genConfigDO.ReverseTransactionDateMode == "Original"
					|| (this.Trans.CloseoutDate != null
						&& this.Trans.CloseoutDate == TimeConverter.Today(this.AccountingSite.CurrentSite).Date))
				{
					this.ReverseButton.Enabled = false;
				}
			}

			if (this.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false)
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			if (this.security.HasRight(RIGHT.PERFORM_REVERSE_TRANSACTION) == false)
			{
				this.ReverseButton.Enabled = false;
				return;
			}

			if (this.Trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				if (!this.security.HasModifyTransactionRightByAliasName(this.Trans.Alias))
				{
					this.ReverseButton.Enabled = false;
					return;
				}
			}

			if (this.Trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				if (!this.security.HasModifyTransactionRightByAliasName(this.Trans.Alias))
				{
					this.ReverseButton.Enabled = false;
				}
			}
		}

		/// <summary>
		/// The set reverse update button.
		/// </summary>
		/// <param name="editable">
		/// The editable.
		/// </param>
		protected virtual void SetReverseUpdateButton(bool editable)
		{
			this.ReverseUpdateButton.Enabled = true;

			if (editable)
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			if (this.Trans.DeleteFlag)
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			if (this.TransContext.mode == TransactionContext.Mode.Add || this.bTransIDBeingLoaded)
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			// No reverse for Orders
			if ((this.Trans.TransTypeID == TransactionTypes.T17_Order)
				|| (this.Trans.TransTypeID == TransactionTypes.T18_SupplyOrder))
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			if (this.Trans.ReversalType != TransactionDO.None
				&& this.Trans.ReversalType != TransactionDO.Update
				&& !string.IsNullOrEmpty(this.Trans.ReversalType))
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			// check the configuration for reverse transaction date mode if the transaction is closed (26-Jun-2009 IGO)
			if (this.Trans.CloseoutDate != null || this.Trans.PartialCloseout)
			{
				var genConfigSr = new GeneralConfigSR
				{
					Security = this.security,
					Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION
				};

				GeneralConfigDO genConfigDO =
					FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(x => x.Get(genConfigSr));

				// disable the button, reversals cannot be applied in closed periods (26-Jun-2009 IGO)
				// disable the button, reversals cannot be applied in closed periods (26-Jun-2009 IGO)
				if (genConfigDO.ReverseTransactionDateMode == "Original"
					|| (this.Trans.CloseoutDate != null
						&& this.Trans.CloseoutDate == TimeConverter.Today(this.AccountingSite.CurrentSite).Date))
				{
					this.ReverseUpdateButton.Enabled = false;
				}
			}

			if (this.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false)
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			if (this.security.HasRight(RIGHT.PERFORM_REVERSE_TRANSACTION) == false)
			{
				this.ReverseUpdateButton.Enabled = false;
				return;
			}

			if (this.Trans.TransTypeID == TransactionTypes.T21_AccountPayableInvoice)
			{
				if (!this.security.HasModifyTransactionRightByAliasName(this.Trans.Alias))
				{
					this.ReverseUpdateButton.Enabled = false;
					return;
				}
			}

			if (this.Trans.TransTypeID == TransactionTypes.T22_AccountReceivableInvoice)
			{
				if (!this.security.HasModifyTransactionRightByAliasName(this.Trans.Alias))
				{
					this.ReverseUpdateButton.Enabled = false;
				}
			}
		}

		/// <summary>
		///	This method sets the state of the Apply (save) button based on the line
		///	items having the required fields populated.
		/// </summary>
		protected virtual void SetSaveButton(bool editable)
		{
			if ((editable == false) && (this.TransContext.mode != TransactionContext.Mode.Add))
			{
				this.SaveButton.Enabled = false;
				return;
			}

			if (this.Trans.ReversalType == TransactionDO.Reversal || this.Trans.ReversalType == TransactionDO.ReversalWithUpdate)
			{
				this.SaveButton.Enabled = false;
				return;
			}

			if (this.Trans.TransTypeID == TransactionTypes.T17_Order)
			{
				if (((this.TransContext.mode == TransactionContext.Mode.Add && this.security.HasRight(RIGHT.CREATE_ORDERS))
						|| (this.TransContext.mode == TransactionContext.Mode.Edit && this.security.HasRight(RIGHT.MODIFY_ORDERS)))
						&& this.security.HasModifyTransactionRightByAliasName(this.Trans.Alias))
				{
					this.SaveButton.Enabled = true;
				}
				else
				{
					this.SaveButton.Enabled = false;
				}
				return;
			}

			//  Make sure the user has right to modify transaction data to save
			if (this.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == false)
			{
				this.SaveButton.Enabled = false;
				return;
			}

			// Different action if the transaction is already deleted. (24-Jun-2009 IGO)
			if (this.Trans.DeleteFlag)
			{
				this.SaveButton.Enabled = false;
			}
			else
			{
				// Make sure that there are line items before 
				// allowing the save button to be enabled. (13-Feb-2009 IGO)
				if (this.Trans.LineItems.Count > 0 || this.TransContext.aliasClass.LineItemFieldCollection.Count == 0)
				{
					this.SaveButton.Enabled = true;
				}
				else
				{
					this.SaveButton.Enabled = false;
				}
			}
		}

		protected virtual void SetViewPrintable()
		{
			if (this.TransContext.mode == TransactionContext.Mode.Add)
			{
				this.ViewPrintableBtn.Enabled = false;
			}
			else
			{
				bool enabled = (!string.IsNullOrEmpty(this.TransContext.aliasClass.AssociatedReport)
									&& this.TransContext.aliasClass.AssociatedReport != "{None}");

				enabled = enabled && this.Trans.LineItems.Count > 0;
				this.ViewPrintableBtn.Enabled = enabled;
			}
		}

		protected virtual void SetVolumeSigns(TransactionDO transactionDo, bool forDisplay)
		{
			transactionDo.SetVolumeSigns(forDisplay);
		}

		protected virtual void TransferToViewing(LineItemDO lineItem)
		{
			this.Session.Remove("OrderAssociatedTxContext");
			this.Session.Remove("SupplyOrderAssociatedTxContext");

			if (lineItem == null)
			{
				return;
			}

			if (this.TransContext.aliasClass.AssociatedAliases.Count > 0)
			{
				var associatedTxContext = new AssociatedTxContext
				{
					TransactionLineItemGuid = lineItem.TransactionLineItemGuid.ToString(),
					ReturnURL = "TransactionDetail.aspx?KEEPASSOCCONTEXT=true&RETURNING=true&TransID=" + this.Trans.TransID,
					OrderNumber = this.Trans.DocumentNumber,
					CustomerOrderNumber = this.Trans.PONumber,
					LineNumber = lineItem.LineNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
					Product = lineItem.Product,
					transaction = this.Trans,
					EditItemIndex = this.LineItemDataGrid.EditItemIndex.ToString(CultureInfo.InvariantCulture),
					allAssociatedTransactionsBeforeTransactionEdit = this.Session["allAssociatedTransactionsBeforeTransactionEdit"]
																					as BaseCollections,
					associatedTransactionsBeforeEdit = this.Session["associatedTransactionsBeforeEdit"] as BaseCollections
				};

				if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
				{
					associatedTxContext.ReturnURL = "../ADFWebApp/" + associatedTxContext.ReturnURL;
				}

				if (this.TransContext.mode == TransactionContext.Mode.Add)
				{
					associatedTxContext.mode = "Add";
				}
				else if (this.TransContext.mode == TransactionContext.Mode.Edit)
				{
					associatedTxContext.mode = "Edit";
				}
				else
				{
					associatedTxContext.mode = "View";
				}

				associatedTxContext.DetailList = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList;
				var prevContext = (AssociatedTxContext)this.Session["AssociatedTxContext"];

				if (prevContext != null)
				{
					associatedTxContext.previousAssociatedTxContext = prevContext;
				}

				//Put the object into session and transfer to the TransactionLlist
				this.Session["AssociatedTxContext"] = associatedTxContext;

				this.Session.Remove(TransactionDetailList.TransactionDetailListKey);
				this.Session.Remove(ModeKey);
				this.Session.Remove(TransKey);
				this.Session.Remove("allAssociatedTransactionsBeforeTransactionEdit");

				this.Redirect("..\\Accounting\\AssociatedTxSummary.aspx");
				this.Context.ApplicationInstance.CompleteRequest();
			}
			else
			{
				if (TransactionTypes.T17_Order == this.Trans.TransTypeID)
				{
					var associatedTxContext = new OrderAssociatedTxContext
					{
						TransactionLineItemGuid = lineItem.TransactionLineItemGuid,
						ReturnURL = this.Request.RawUrl,
						OrderNumber = this.Trans.DocumentNumber,
						CustomerOrderNumber = this.Trans.PONumber,
						LineNumber = lineItem.LineNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
						Product = lineItem.Product,
						transaction = this.Trans,
						DetailList = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList
					};

					var transactionDateTime = this.Trans.TransactionDateTime;

					if (transactionDateTime != null)
					{
						associatedTxContext.TransDate = this.AccountingSite.FormatDate(transactionDateTime.Value);
					}

					//Put the object into session and transfer to the TransactionLlist
					this.Session["OrderAssociatedTxContext"] = associatedTxContext;
					this.Redirect("..\\OrderEntryWebApp\\OrderAssociatedTxSummary.aspx");
					this.Context.ApplicationInstance.CompleteRequest();
				}
				else if (TransactionTypes.T18_SupplyOrder == this.Trans.TransTypeID)
				{
					var supplyContext = new SupplyOrderAssociatedTxContext
					{
						TransactionLineItemGuid = lineItem.TransactionLineItemGuid,
						ReturnURL = this.Request.RawUrl,
						OrderNumber = this.Trans.DocumentNumber,
						CustomerOrderNumber = this.Trans.PONumber,
						LineNumber = lineItem.LineNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
						Product = lineItem.Product,
						transaction = this.Trans,
						DetailList = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList
					};

					var transactionDateTime = this.Trans.TransactionDateTime;

					if (transactionDateTime != null)
					{
						supplyContext.TransDate = this.AccountingSite.FormatDate(transactionDateTime.Value);
					}

					//Put the object into session and transfer to the TransactionLlist
					this.Session["SupplyOrderAssociatedTxContext"] = supplyContext;
					this.Redirect("..\\SupplyOrderWebApp\\SupplyOrderAssociatedTxSummary.aspx");
					this.Context.ApplicationInstance.CompleteRequest();
				}
			}
		}

		/// <summary>
		///	This method will handle the Cancel Command event for the transport info grid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected virtual void TransportDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			this.TransportDataGrid.EditItemIndex = -1;
			this.TransportDataGrid.SelectedIndex = -1;
			this.TransportInfoGridGenerator.Bind();

			this.EnableFieldTable(true, false);
			this.NewTransportButton.Enabled = true;

			if (this.NewLineItemButton.Visible)
			{
				this.NewLineItemButton.Enabled = true;
			}

			if (this.NewAGRButton.Visible)
			{
				this.NewAGRButton.Enabled = true;
			}

			// Set the buttons back to the previous settings prior to the line
			// item edit.
			this.SetButtons();
		}

		/// <summary>
		///	This method handles the Gauge Readings Data Grid delete command event.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected virtual void TransportDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			this.Trans.TransportInfoList.RemoveAt(e.Item.ItemIndex);

			if (e.Item.ItemIndex == this.TransportDataGrid.EditItemIndex)
			{
				this.TransportDataGrid.EditItemIndex = -1;
				this.EnableFieldTable(true, false);
				this.NewTransportButton.Enabled = true;

				if (this.NewLineItemButton.Visible)
				{
					this.NewLineItemButton.Enabled = true;
				}

				if (this.NewAGRButton.Visible)
				{
					this.NewAGRButton.Enabled = true;
				}
			}

			this.TransportInfoGridGenerator.Bind();
		}

		/// <summary>
		///	This method will handle the Edit Command event for the transport info grid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected virtual void TransportDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.RetrieveDataFromPage();
			this.TransportDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.TransportDataGrid.SelectedIndex = e.Item.ItemIndex;
			this.TransportInfoGridGenerator.Bind();

			this.EnableFieldTable(false, false);
			this.DisableButtonsForEditing();
		}

		/// <summary>
		///	This method will handle the Update Command event for the transport info grid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected virtual void TransportDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.RetrieveTransportLineItems(e.Item);

				this.TransportDataGrid.EditItemIndex = -1;
				this.TransportInfoGridGenerator.Bind();
				this.EnableFieldTable(true, false);
				this.NewTransportButton.Enabled = true;

				if (this.NewLineItemButton.Visible)
				{
					this.NewLineItemButton.Enabled = true;
				}

				if (this.NewAGRButton.Visible)
				{
					this.NewAGRButton.Enabled = true;
				}

				// Set the buttons back to the previous settings prior to the line
				// item edit.
				this.SetButtons();
			}
			catch (Exception ex)
			{
				this.HandleFieldError(ex);
			}
		}

		/// <summary>
		///	This method will handle the new button click event for the transport grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void TransportInfoNewButtonClick(object sender, EventArgs e)
		{
			if (this.RetrieveDataFromPage() == false)
			{
				return;
			}

			this.Trans.TransportInfoList.Add(new TransportLineItemDO());
			this.TransportDataGrid.SelectedIndex = this.TransportDataGrid.Items.Count;
			this.TransportDataGrid.EditItemIndex = this.TransportDataGrid.Items.Count;
			this.TransportInfoGridGenerator.Bind();
			this.EnableFieldTable(false, false);
			this.DisableButtonsForEditing();
		}

		protected virtual void TransportItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex == -1)
			{
				return;
			}

			this.SetTransportItemDeleteAndEditButtonState(e);
		}

		/// <summary>
		///	This method will build a transaction list context for transferring to the Transaction List page.
		///	When a user adds a transaction from the left tree view the return page should be the
		///	transaction list page and not the accounting splash page. Therefore, the transaction list
		///	page requires a transaction list context in the session.
		/// </summary>
		/// <returns></returns>
		protected void UpdateTransDetailList()
		{
			TransactionListContext transListContext;

			if (this.Session["TransactionListContext"] == null)
			{
				transListContext = new TransactionListContext();
			}
			else
			{
				transListContext = (TransactionListContext)this.Session["TransactionListContext"];
			}

			// Update the transaction list context with the manager, owner, and product.
			if (!string.IsNullOrEmpty(this.Trans.ManagerID))
			{
				transListContext.Manager = this.Trans.ManagerID;
			}

			transListContext.Month = DateEfficacy.ConvertToMonthAndYear(this.Trans.InventoryDate);
			if (!string.IsNullOrEmpty(this.Trans.OwnerID))
			{
				transListContext.Owner = this.Trans.OwnerID;
			}

			transListContext.ReturnURL = "Ledger.aspx";
			transListContext.Site = this.Trans.Site;

			List<LineItemDO> lineItems = this.Trans.LineItems;

			// Get the first product in the line items. If the product or product code
			// fields are empty, then set the product in the context to blank.
			if ((lineItems == null) || (lineItems.Count < 1))
			{
				if (string.IsNullOrEmpty(transListContext.TransactionListReturnURL))
				{
					transListContext.Product = string.Empty;
				}
			}
			else
			{
				LineItemDO lineItem = lineItems[0];

				if (string.IsNullOrEmpty(lineItem.Product))
				{
					if (string.IsNullOrEmpty(lineItem.ProductCode))
					{
						transListContext.Product = string.Empty;
					}
					else
					{
						transListContext.Product = lineItem.ProductCode;
					}
				}
				else
				{
					bool hasProduct = this.TransListHasProduct(lineItems, transListContext.Product);

					if (hasProduct == false)
					{
						transListContext.Product = lineItem.Product;
					}
				}
			}

			this.Session.Add("TransactionListContext", transListContext);
		}

		/// <summary>
		///	This method handles the Print Viewable button being pressed event. The
		///	intent is to determine the report that is associated with the current
		///	alias and print the report.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void ViewPrintableBtnClick(object sender, EventArgs e)
		{
			string bolRptType = ((int)ReportTypesClass.ReportTypes.BOL_RPT).ToString(CultureInfo.InvariantCulture);

			//string rptURL = "../FMReporting/ReportLandingPage.aspx?ReportType=" + bolRptType;
			string rptUrl = "../FMReportWebMain/PopupReportLandingPage.aspx?ReportType=" + bolRptType;

			string reportName = this.TransContext.aliasClass.AssociatedReport.Replace(" ", "+");
			rptUrl = rptUrl + "&ReportName=" + reportName;
			rptUrl = rptUrl + "&SiteGuidStr=" + this.Trans.SiteGuid;
			rptUrl = rptUrl + "&TransID=" + this.Trans.TransID;
			rptUrl += "&" + this.security.CSRFTokenWithParamName;

			string javascriptPopupReport = "<script type='text/javascript'>\n<!-- \n" + "window.open('" + rptUrl + "', "
													+ "'Reports', "
													+ "'status=0, toolbar=0, menubar=1, resizable=1, scrollbars=1, height=950, width=850'"
													+ "); \n" + "-->\n</script>";

			this.Response.Cookies.Add(new HttpCookie("Token", this.Session["Token"] as string));
			ScriptManager.RegisterClientScriptBlock(
				this.Page, this.GetType(), "RPT_POPUP_NEW_BROWSER", javascriptPopupReport, false);
		}

		/// <summary>
		///	Tests whether a ServiceController object can be successfully used by seeing if
		///	accessing a property throws an InvalidOperationException. The DebuggerHidden
		///	attribute is set so that the debugger won't break if InvalidOperationException is thrown
		/// </summary>
		/// <param name="serviceController">object to test</param>
		/// <returns>True if property was successfully accessed</returns>
		//[DebuggerHidden]
		protected bool WasServiceFound(ServiceController serviceController)
		{
			if (serviceController == null)
			{
				return false;
			}

			try
			{
				// checking the service controller status is the important part here.
				// That the variable isn't used is intended.
				// ReSharper disable once UnusedVariable
				var status = serviceController.Status;
				return true;
			}
			catch (InvalidOperationException)
			{
				// This means the service was not found installed on the system
				return false;
			}
		}

		/// <summary>
		///	This method will dictionary the line item grid non dynamic header columns. All
		///	the other column headers are dynamic and do not need to be dictionaried.
		/// </summary>
		private void ApplyDictionaryToNonDynamicColumnHeaders()
		{
			foreach (DataGridColumn column in this.LineItemDataGrid.Columns)
			{
				if (column.HeaderText.ToUpper().Equals("EDIT") || column.HeaderText.ToUpper().Equals("DELETE")
					|| column.HeaderText.ToUpper().Equals("ADD SUBLINE-ITEM"))
				{
					string newText = GetDataDictionaryValueByKey(this.AccountingSite.CurrentSiteGuid, column.HeaderText);
					column.HeaderText = newText;
				}
			}
		}


		/*
				private void ProcessEquipment(IEquipments equipments, EquipmentDO[] equipmentDOArray, byte eqNumber, bool destination)
				{
					foreach (EquipmentDO equipmentDO in equipmentDOArray)
					{
						eqNumber++;

						if (eqNumber == 4)
						{
							eqNumber = 1;
							destination = false;
						}

						if ((equipmentDO.RegistrationID == null) || (equipmentDO.RegistrationID == string.Empty)
							|| (equipmentDO.EquipmentGuid == Guid.Empty))
						{
							continue;
						}

						Guid testGuid = equipments.GetIdentityGuid(this.security, equipmentDO.RegistrationID);
						if (testGuid != Guid.Empty)
						{
							equipmentDO.EquipmentGuid = testGuid;
							continue;
						}

						var equipment = new EquipmentClass();
						equipment.ID = equipmentDO.RegistrationID;
						equipment.Model = equipmentDO.EquipmentModel;
						equipment.Xref = equipmentDO.EquipmentRefID;

						if (!this.TransContext.aliasClass.MultipleLineItems && this.Trans.LineItems.Count == 1)
						{
							equipment.ProductID = (this.Trans.LineItems[0]).Product;
							if ((this.Trans.LineItems[0]).ProductGuid != Guid.Empty)
							{
								equipment.ProductGuid = (this.Trans.LineItems[0]).ProductGuid;
							}
						}

						if (this.Trans.FuelCardGuid != Guid.Empty
							&& (((this.Trans.TransTypeID == TransactionTypes.T3_PrimaryDefuel
									|| this.Trans.TransTypeID == TransactionTypes.T4_SecondaryDefuel) && !destination)
								|| (((this.Trans.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
										|| this.Trans.TransTypeID == TransactionTypes.T6_SecondaryDisbursement) && destination))))
						{
							equipment.FuelCardGuid = this.Trans.FuelCardGuid;
							equipment.FuelCardID = this.Trans.FuelCardID;
						}

						equipmentDO.EquipmentGuid = equipments.Add(this.security, equipment);
					}
				}
		*/

		private void AutoPopulateHierarchalData()
		{
			// this routine will auto populate as much of the selections that it can. If there is only one selection available
			// then set that value for the drop down box following the hierarchy.
			if (!this.TransContext.aliasClass.LimitSelectionsBasedOnHierarchy)
			{
				return;
			}

			// get the manager string from the text box
			if (string.IsNullOrEmpty(this.Trans.ManagerID))
			{
				return;
			}

			// check if there is only one owner configured
			CompanyCollectionClass companyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																		x =>
																		x.EnumerateHierarchialCustomerFromRole(this.security,
																												COMPANY_ROLE.OWNER,
																												this.Trans.ManagerID,
																												string.Empty,
																												string.Empty,
																												string.Empty,
																												string.Empty));

			if (companyCollection.Count == 1)
			{
				// populate the owners box with the entry
				this.Trans.OwnerID = companyCollection[0].ID;
				this.Trans.OwnerCode = companyCollection[0].Code;
				this.Trans.OwnerCompanyGuid = companyCollection[0].MasterRecordGuid;

				// check the shipper
				companyCollection =
					FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
						x =>
						x.EnumerateHierarchialCustomerFromRole(
							this.security, COMPANY_ROLE.SHIPPER, this.Trans.ManagerID, this.Trans.OwnerID, string.Empty, string.Empty, string.Empty));

				if (companyCollection.Count == 1)
				{
					this.Trans.ShipperID = companyCollection[0].ID;
					this.Trans.ShipperCode = companyCollection[0].Code;
					this.Trans.ShipperCompanyGuid = companyCollection[0].MasterRecordGuid;

					// check the bill to
					companyCollection =
						FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
							x =>
							x.EnumerateHierarchialCustomerFromRole(
								this.security,
								COMPANY_ROLE.CUSTOMER_BILLTO,
								this.Trans.ManagerID,
								this.Trans.OwnerID,
								this.Trans.ShipperID,
								string.Empty,
								string.Empty));

					if (companyCollection.Count == 1)
					{
						this.Trans.BillToID = companyCollection[0].ID;
						this.Trans.BillToCode = companyCollection[0].Code;
						this.Trans.BillToCompanyGuid = companyCollection[0].MasterRecordGuid;

						// check the ship to
						companyCollection =
							FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
								x =>
								x.EnumerateHierarchialCustomerFromRole(
									this.security,
									COMPANY_ROLE.CUSTOMER_SHIPTO,
									this.Trans.ManagerID,
									this.Trans.OwnerID,
									this.Trans.ShipperID,
									this.Trans.BillToID,
									string.Empty));

						if (companyCollection.Count == 1)
						{
							this.Trans.ShipToID = companyCollection[0].ID;
							this.Trans.ShipToCode = companyCollection[0].Code;
							this.Trans.ShipToCompanyGuid = companyCollection[0].MasterRecordGuid;
						}
					}
				}
			}
		}

		private bool CheckEditable()
		{
			if (this.IsTransactionEditable == false)
			{
				this.ErrorHandler(new Exception("Transaction not editable."));
				return false;
			}

			return true;
		}

		/// <summary>
		///	This method will prepare for combination of transaction
		/// </summary>
		private void CombineTransaction(string transID)
		{
			TransactionDO combineTrans = this.LoadTransaction(transID);

			/// <summary>
			/// Check to ensure that we don't combine 
			/// from a broken blend transaction.
			/// </summary>
			foreach (LineItemDO lineItemDO in combineTrans.LineItems)
			{
				if (lineItemDO.BrokenBlend.Value)
				{
					const string ErrMsg = "Cannot combine BOLs. Source BOL contains a broken blend.";
					throw new ApplicationException(ErrMsg);
				}
			}

			this.SetVolumeSigns(this.Trans, false);

			// Assign new guids before adding the lineitem(s) so the guid(s) in the target transaction 
			// doesn't match what's in the source transaction.
			foreach (LineItemDO lineItemDO in combineTrans.LineItems)
			{
				lineItemDO.TransactionLineItemGuid = Guid.NewGuid();
				lineItemDO.TransactionLineItemUserDataGuid = Guid.NewGuid();

				foreach (var subLineItem in lineItemDO.SubLineItems)
				{
					subLineItem.TransactionSubLineItemGuid = Guid.NewGuid();
				}

				this.Trans.LineItems.Add(lineItemDO);
			}

			if (this.TransContext.aliasClass.MultipleWeightReadings)
			{
				foreach (WeightReadingDO weightReadingDO in combineTrans.WeightReadings)
				{
					this.Trans.WeightReadings.Add(weightReadingDO);
				}
			}
			else
			{
				if (combineTrans.WeightReadings.Count == 1 && this.Trans.WeightReadings.Count == 1)
				{
					WeightReadingDO sourceDO = combineTrans.WeightReadings[0];
					WeightReadingDO destinationDO = this.Trans.WeightReadings[0];

					if (destinationDO != null)
					{
						if (sourceDO != null)
						{
							destinationDO.BeginQuantity += sourceDO.BeginQuantity;
							destinationDO.FinalQuantity += sourceDO.FinalQuantity;
							destinationDO.RequestedQuantity += sourceDO.RequestedQuantity;
						}
						else
						{
							throw new ApplicationException("Bad source weight reading.");
						}
					}
					else
					{
						throw new ApplicationException("Bad Destination weight reading");
					}
				}
			}
			this.SetVolumeSigns(this.Trans, true);
			this.Session[CombineTransKey] = transID;

			/// <summary>
			/// Prepare as reverse update
			/// </summary>
			if (this.Trans.CloseoutDate != null || this.Trans.Status == TransactionStatus.Posted)
			{
				this.TransContext.mode = TransactionContext.Mode.Add;
				this.Session[ModeKey] = this.TransContext.mode;
				this.Trans.Status = TransactionStatus.Completed;
				this.Trans.ReversalType = TransactionDO.Update;
				this.Trans.ReversedTransID = this.Trans.TransID;
				this.Trans.ConjoinReversedTransID = this.Trans.ConjoinedTransID;

				// Assigning new guids to be used on the new transaction (the reversal of the source) 
				this.Trans.TransID = FuelsManagerId.NewId();
				this.Trans.TransactionGuid = Guid.NewGuid();
				this.Trans.TransactionUserDataGuid = Guid.NewGuid();
				this.Trans.TransactionSignatureGuid = Guid.NewGuid();
				this.Trans.TransactionNoteGuid = Guid.NewGuid();

				foreach (var item in this.Trans.LineItems)
				{
					item.TransactionLineItemGuid = Guid.NewGuid();
					item.TransactionLineItemUserDataGuid = Guid.NewGuid();

					foreach (var subLineItem in item.SubLineItems)
					{
						subLineItem.TransactionSubLineItemGuid = Guid.NewGuid();
					}
				}

				foreach (var transportLineItem in this.Trans.TransportInfoList)
				{
					transportLineItem.TransactionTransportLineItemGuid = Guid.NewGuid();
				}

				if (!string.IsNullOrEmpty(this.Trans.ConjoinedTransID))
				{
					this.Trans.ConjoinedTransID = FuelsManagerId.NewId();
				}

				/// <summary>
				/// check the configuration for reverse transaction 
				/// date mode (25-Jun-2009 IGO).
				/// </summary>
				var genConfigSR = new GeneralConfigSR
				{
					Security = this.security,
					Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION
				};

				GeneralConfigDO genConfigDO =
					FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(x => x.Get(genConfigSR));

				if (genConfigDO.ReverseTransactionDateMode == "Original")
				{
					this.Trans.InventoryDate = this.Trans.InventoryDate;
				}
				/// <summary>
				/// always default to current date
				/// </summary>
				else
				{
					this.Trans.InventoryDate = this.GetCurrentInventoryDate();
				}

				this.Trans.CloseoutDate = null;
				this.Trans.PartialCloseout = false;

				foreach (LineItemDO lineItemDO in this.Trans.LineItems)
				{
					lineItemDO.CloseoutDate = null;

					foreach (SubLineItemDO subLineItemDO in lineItemDO.SubLineItems)
					{
						subLineItemDO.CloseoutDate = null;
					}
				}
			}

			if (this.Trans.TransPIDXCollection != null)
			{
				foreach (TransactionPIDXDO transactionPidxDo in this.Trans.TransPIDXCollection)
				{
					transactionPidxDo.SentFlag = false;
					transactionPidxDo.AuthorizationNumber = string.Empty;
				}
			}

			this.SetButtons();
			this.EnableFieldTable(true, false);

			if (this.TransContext.aliasClass.MultipleLineItems)
			{
				this.LineItemGridGenerator.Bind();
				this.NewLineItemButton.Enabled = true;
			}

			if (this.TransContext.aliasClass.MultipleWeightReadings)
			{
				this.AgrGridGenerator.Bind();
				this.NewAGRButton.Enabled = true;
			}

			if (this.TransContext.aliasClass.MultipleTransportLineItems)
			{
				this.TransportInfoGridGenerator.Bind();

				if (this.TransportDataGrid.Columns.Count > 2)
				{
					this.NewTransportButton.Enabled = true;
				}
			}

			this.TransIDLabel.Text = this.Trans.TransID;

			var transIDTextBox = this.FindControl("TransactionFields.TransID_FG") as TextBox;

			if (transIDTextBox != null)
			{
				transIDTextBox.Text = this.Trans.TransID;
			}

			var reversedTransIDTextBox = this.FindControl("TransactionFields.ReversedTransID_FG") as TextBox;

			if (reversedTransIDTextBox != null)
			{
				reversedTransIDTextBox.Text = this.Trans.ReversedTransID;
			}

			var conjoinedTransIDTextBox = this.FindControl("TransactionFields.ConjoinedTransID_FG") as TextBox;

			if (conjoinedTransIDTextBox != null)
			{
				conjoinedTransIDTextBox.Text = this.Trans.ConjoinedTransID;
			}

			var reversalTypeTextBox = this.FindControl("TransactionFields.ReversalTypeFG") as TextBox;

			if (reversalTypeTextBox != null)
			{
				reversalTypeTextBox.Text = this.Trans.ReversalType;
			}

			var statusSelect = this.FindControl("TransactionFields.TransactionStatusFG") as HtmlSelect;

			if (statusSelect != null)
			{
				string completedString = Enum.GetName(typeof(TransactionStatus), TransactionStatus.Completed);

				if (this.TransContext.useDataDictonary)
				{
					var s = completedString;
					completedString = GetDataDictionaryValueByKey(this.TransContext.accountingSite.CurrentSiteGuid, s);
				}

				int index = 0;

				foreach (ListItem item in statusSelect.Items)
				{
					if (item.Text == completedString)
					{
						break;
					}

					index++;
				}

				statusSelect.SelectedIndex = index;
			}

			var inventoryDate = this.FindControl("TransactionFields.InventoryDateFG Date") as FMDate;
			if (inventoryDate != null)
			{
				inventoryDate.Text = this.TransContext.accountingSite.FormatDate(this.Trans.InventoryDate);
			}

			var closeoutDate = this.FindControl("TransactionFields.CloseoutDateFG Date") as FMDate;
			if (closeoutDate != null)
			{
				closeoutDate.Text = string.Empty;
			}

			var transDateTime = this.FindControl("TransactionFields.TransactionDateTimeFG DateTime") as FMDateTime;
			if (transDateTime != null)
			{
				var transactionDateTime = this.Trans.TransactionDateTime;

				if (transactionDateTime != null)
				{
					transDateTime.Text = this.TransContext.accountingSite.FormatDateTime(transactionDateTime.Value);
				}
			}
		}

		/// <summary>
		///	The purpose of this method is to register an onload event to invoke custom
		///	client scripting if the custom script file is present. The custom script file
		///	must have the TxDetailOnlad() function present.
		/// </summary>
		private void CreateAnOnloadPageEventForCustomScripts()
		{
			// Only register a page onload event if the custom script is
			// present.
			if (this.CustomScriptName.Length > 0)
			{
				const string Onload = @"<script type=""text/javascript"">
										var onLoadFunction = function(){TxDetailOnload();};
										Sys.Application.add_load(onLoadFunction);
										</script>";
				this.ClientScript.RegisterStartupScript(this.GetType(), "ONLOAD123", Onload, false);
			}
		}

		private LineItemDO FindMatchingLineItem(TransactionDO orderTx, string product)
		{
			// Since Orders are limited to only one lineitem per product, we just need to
			// find the one that matches

			foreach (LineItemDO lineItem in orderTx.LineItems)
			{
				if (lineItem.Product == product)
				{
					return lineItem;
				}
			}

			return null;
		}

		/// <summary>
		///	This method will return the custom client script name to be used
		///	for custom client side scripting.
		/// </summary>
		private void GetCustomClientScriptName()
		{
			if (this.Session[CustomClientScriptName] != null)
			{
				this.CustomScriptName = this.Session[CustomClientScriptName] as string;
			}

			if (string.IsNullOrEmpty(this.CustomScriptName))
			{
				this.CustomScriptName = ConfigurationManager.AppSettings["CustomClientScriptName_01"];

				if (string.IsNullOrEmpty(this.CustomScriptName))
				{
					this.CustomScriptName = string.Empty;
				}

				this.Session.Add(CustomClientScriptName, this.CustomScriptName);
			}
		}

		/// <summary>
		///	Returns the parent line item of the passed sub-line item
		/// </summary>
		/// <param name="subLine">The sub-line item whose parent is being found</param>
		/// <returns>A line item if one is found.  Null if the parent is not found</returns>
		private LineItemDO GetParentLineItem(SubLineItemDO subLine)
		{
			LineItemDO lineItem = null;

			foreach (LineItemDO lineItemDo in this.Trans.LineItems)
			{
				if (lineItemDo.SubLineItems.IndexOf(subLine) >= 0)
				{
					lineItem = lineItemDo;
					break;
				}
			}

			return lineItem;
		}

		private void GetSpecialInstructions(CompanyClass shipTo,
											Guid productGuid,
											out string specialInstruction,
											out Guid productMapGuid,
											out PRODUCT_MAP_TYPE productMapType)
		{
			foreach (ProductMapClass productMap in shipTo.AuthorizedProductCollection)
			{
				if (productMap.AssignedGuid == productGuid)
				{
					specialInstruction = productMap.SpecialInstructions;
					productMapGuid = productMap.IdentityGuid;
					productMapType = productMap.Type;

					return;
				}
			}
			specialInstruction = string.Empty;
			productMapGuid = Guid.Empty;
			productMapType = PRODUCT_MAP_TYPE.UNDEFINED_MAP;
		}

		/// <summary>
		///	This method will retrieve the transaction from the database for the
		///	selected transaction.
		/// </summary>
		/// <param name="transID"></param>
		private void GetTransaction(string transID)
		{
			var timer = new StopWatch(StopWatch.Appnames.Accounting, "TransactionDetail.GetTransaction()");

			//TransactionDO origTrans = this.LoadTransaction(transID);
			this.Trans = this.LoadTransaction(transID);

			//Show positive volumes for Issues and such.
			//Correct volume signs
			this.SetVolumeSigns(this.Trans, true);
			//this.SetVolumeSigns(origTrans, false);

			if (this.IsOrderAssociatedTransaction(this.Trans))
			{
				this.orderTxReferenceID = this.Trans.TransRefID;

				foreach (LineItemDO lineItem in this.Trans.LineItems)
				{
					if (lineItem.OrderReferenceTransactionLineItemGuid != Guid.Empty)
					{
						this.OrderReferenceID = lineItem.OrderReferenceTransactionLineItemGuid.ToString();
						break;
					}
				}

				this.OrderContext = this.Session["OrderAssociatedTxContext"] as OrderAssociatedTxContext;
				this.SupplyOrderContext = this.Session["SupplyOrderAssociatedTxContext"] as SupplyOrderAssociatedTxContext;
			}

			this.Session[TransKey] = this.Trans;
			//Session[TransactionDetailBase.OriginalTransKey] = origTrans;

			timer.Stop();
		}

		/// <summary>
		///	This method will return true if the user has permission to view transactions.
		///	Otherwise, it will return false.
		/// </summary>
		/// <returns></returns>
		private bool HasViewRights()
		{
			return this.security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
					|| this.security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
					|| this.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
					|| this.security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
					|| this.security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
					|| this.security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
					|| this.security.HasRight(RIGHT.VIEW_ORDERS)
					|| this.security.HasRight(RIGHT.MODIFY_ORDERS)
					|| this.security.HasRight(RIGHT.CREATE_ORDERS);
		}

		/// <summary>
		///	This method will return false if the Gauge Readings Data Grid and the Line Item Data
		///	Grids are not in edit mode. If either are in edit mode, then it will return true.
		/// </summary>
		/// <returns></returns>
		private bool InEditMode()
		{
			return (this.GaugeReadingsDataGrid != null && this.GaugeReadingsDataGrid.EditItemIndex != -1)
								|| (this.LineItemDataGrid != null && this.LineItemDataGrid.EditItemIndex != -1)
								|| (this.TransportDataGrid != null && this.TransportDataGrid.EditItemIndex != -1);
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private bool IsLineItemEditable(LineItemDO lineItem)
		{
			// A line item is editable only when the entire transaction is editable or when it is a non-closed line item
			// that is part of a transaction that has been partially closed out.
			if (this.IsTransactionEditable == false || (this.Trans.PartialCloseout && lineItem.CloseoutDate != null))
			{
				return false;
			}

			return true;
		}

		private bool IsOrderAssociatedTransaction(TransactionDO transaction)
		{
			if (this.TransContext != null && this.TransContext.mode == TransactionContext.Mode.Add)
			{
				if (!string.IsNullOrEmpty(this.orderTxReferenceID))
				{
					return true;
				}
			}

			if (!string.IsNullOrEmpty(this.orderTxReferenceID) || transaction.TransRefID != string.Empty)
			{
				if (transaction.LineItems.Count > 0)
				{
					foreach (LineItemDO lineItem in transaction.LineItems)
					{
						if (lineItem.OrderReferenceTransactionLineItemGuid != Guid.Empty)
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		///	This method will update the UpdatedBy and UpdatedDate fields with the current
		///	user ID and date.
		/// </summary>
		private void LastUpdatedFields()
		{
			var updatedByTextBox = this.FieldTable.FindControl("TransactionFields.UpdatedByFG") as TextBox;
			var updatedByDate = this.FieldTable.FindControl("TransactionFields.UpdatedDateFG DateTime") as FMDateTime;

			if (updatedByTextBox != null)
			{
				updatedByTextBox.Text = this.security.UserID;
			}

			if (updatedByDate != null)
			{
				DateTimeOffset siteTime = TimeConverter.Now(this.AccountingSite.CurrentSite);
				string convertedDate = this.AccountingSite.FormatDateTime(siteTime);
				updatedByDate.Text = convertedDate;
			}
		}

		private void LinkUpOrderLineItems()
		{
			// Look up the order we are associating with this transaction
			TransactionDO orderTx = this.LoadTransaction(this.orderTxReferenceID);

			if (orderTx != null)
			{
				foreach (LineItemDO lineItem in this.Trans.LineItems)
				{
					LineItemDO orderLineItem = this.FindMatchingLineItem(orderTx, lineItem.Product);

					if (orderLineItem != null)
					{
						lineItem.OrderReferenceTransactionLineItemGuid = orderLineItem.TransactionLineItemGuid;
					}
				}
			}
		}

		/// <summary>
		///	Populates associated transaction
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="requestType"></param>
		/// <returns></returns>
		private AssociatedTxDO PopulateAssociatedTxDO(DataRow dr, AssociatedTxSR.RequestTypes requestType)
		{
			var txDo = new AssociatedTxDO();

			if (requestType == AssociatedTxSR.RequestTypes.GetAssociatedTransactionDetails)
			{
				txDo.TransactionLineItemGuid = DataObject.getValue(dr["TransactionLineItemGuid"], Guid.Empty);
				txDo.TransTypeID = DataObject.getValue(dr["LookupTransTypeIndex"], TransactionTypes.TransactionType_None);
				txDo.GrossQuantity = DataObject.getValue(dr["GrossQuantity"], 0.0);
				txDo.GrossQuantityReceived = DataObject.getValue(dr["GrossQuantityReceived"], 0.0);
				txDo.Excise = DataObject.getValue(dr["Excise"], 0.0);
				txDo.GST = DataObject.getValue(dr["GST"], 0.0);
				txDo.Markup = DataObject.getValue(dr["Markup"], 0.0);
				txDo.TotalValue = DataObject.getValue(dr["TotalValue"], 0.0);
				txDo.TotalPriceWithTax = DataObject.getValue(dr["TotalPriceWithTax"], 0.0);
			}
			else
			{
				txDo.Load(dr);
			}

			return txDo;
		}

		private void PopulateBillingFromFuelCard(string tailNumber)
		{
			if (string.IsNullOrEmpty(tailNumber))
			{
				return;
			}

			EquipmentClass equipment =
				FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
					x => x.Get(this.security, x.GetIdentityGuid(this.security, tailNumber)));

			if (equipment.FuelCardGuid == Guid.Empty)
			{
				return;
			}

			//Get Billing information from FuelCard
			FuelCardClass fuelCard =
				FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(x => x.Get(this.security, equipment.FuelCardGuid, false));

			bool foundInactiveFuelCard = false;

			if (FuelCardClass.Statuses.ACTIVE == fuelCard.Status)
			{
				if (fuelCard.StatusModifiedDate.AddMonths(fuelCard.InactivityPeriod) < DateTimeOffset.Now)
				{
					foundInactiveFuelCard = true;
					fuelCard.Status = FuelCardClass.Statuses.INACTIVE;

					fuelCard.StatusModifiedDate = DateTimeOffset.Now;
					fuelCard.StatusModifiedBy = this.security.UserID;
					FMChannelHelper.MakeCall<IFuelCards>(x => x.Modify(this.security, fuelCard));
				}
				else
				{
					if (this.Request.GetQueryOrFormValue("__MYEVENTTARGET") == "TAIL_NUMBER_CHANGED")
					{
						var fmManagerControlbase = this.FieldTable.FindControl("TransactionFields.ManagerFG") as TextBox;
						var fmOwnerControlbase = this.FieldTable.FindControl("TransactionFields.OwnerFG") as TextBox;
						var fmShipperControlbase = this.FieldTable.FindControl("TransactionFields.ShipperFG") as TextBox;
						var fmBillToControlbase = this.FieldTable.FindControl("TransactionFields.BilltoFG") as TextBox;
						var fmShipToControlbase = this.FieldTable.FindControl("TransactionFields.ShiptoFG") as TextBox;

						if ((fmManagerControlbase != null && fuelCard.ManagerID != fmManagerControlbase.Text)
							|| (fmOwnerControlbase != null && fuelCard.OwnerID != fmOwnerControlbase.Text)
							|| (fmShipperControlbase != null && fuelCard.ShipperID != fmShipperControlbase.Text)
							|| (fmBillToControlbase != null && fuelCard.BillToID != fmBillToControlbase.Text)
							|| (fmShipToControlbase != null && fuelCard.ShipToID != fmShipToControlbase.Text))
						{
							//Fuel card exists and user values are different than fuel card values. 
							//Ask user if fuel card values should be used.
							const string Message = "A fuel card exists for selected equipment.\nTo override field values with values from the existing fuel card click OK.";

							this.Page.ClientScript.RegisterStartupScript(
								this.GetType(),
								"FUELCARD_EXISTS",
								"<script type=\"text/javascript\">\r\n<!--\r\nif (confirm(\"" + HttpUtility.JavaScriptStringEncode(Message)
								+ "\")) __mydoPostBack('TAIL_NUMBER_CHANGE_APPROVED', '" + tailNumber + "');\r\n-->\r\n</script>");
							return;
						}

						if (fmManagerControlbase != null)
						{
							fmManagerControlbase.Text = fuelCard.ManagerID;
						}

						if (fmOwnerControlbase != null)
						{
							fmOwnerControlbase.Text = fuelCard.OwnerID;
						}

						if (fmShipperControlbase != null)
						{
							fmShipperControlbase.Text = fuelCard.ShipperID;
						}

						if (fmBillToControlbase != null)
						{
							fmBillToControlbase.Text = fuelCard.BillToID;
						}

						if (fmShipToControlbase != null)
						{
							fmShipToControlbase.Text = fuelCard.ShipToID;
						}

						fuelCard.StatusModifiedDate = DateTimeOffset.Now;
						fuelCard.StatusModifiedBy = this.security.UserID;
						FMChannelHelper.MakeCall<IFuelCards>(x => x.Modify(this.security, fuelCard));

						return;
					}

					if (FuelCardClass.Statuses.INACTIVE == fuelCard.Status)
					{
						foundInactiveFuelCard = true;
					}
				}

				if (foundInactiveFuelCard)
				{
					throw new Exception("Fuel Card has exceeded its configured inactivity period.");
				}
			}
		}


		private void PrepAssociatedTransaction()
		{
			this.Trans.AssociatedOrderTx = true;
			this.Trans.AssociatedOrderProduct = this.OrderProduct;

			if (this.OrderContext != null)
			{
				this.Trans.OwnerID = this.OrderContext.transaction.OwnerID;
				this.Trans.OwnerCode = this.OrderContext.transaction.OwnerCode;
				this.Trans.OwnerCompanyGuid = this.OrderContext.transaction.OwnerCompanyGuid;
				this.Trans.ManagerID = this.OrderContext.transaction.ManagerID;
				this.Trans.ManagerCode = this.OrderContext.transaction.ManagerCode;
				this.Trans.ManagerCompanyGuid = this.OrderContext.transaction.ManagerCompanyGuid;
				this.Trans.BillToID = this.OrderContext.transaction.BillToID;
				this.Trans.BillToCode = this.OrderContext.transaction.BillToCode;
				this.Trans.BillToCompanyGuid = this.OrderContext.transaction.BillToCompanyGuid;
				this.Trans.ShipToID = this.OrderContext.transaction.ShipToID;
				this.Trans.ShipToCode = this.OrderContext.transaction.ShipToCode;
				this.Trans.ShipToCompanyGuid = this.OrderContext.transaction.ShipToCompanyGuid;
				this.Trans.ShipperID = this.OrderContext.transaction.ShipperID;
				this.Trans.ShipperCode = this.OrderContext.transaction.ShipperCode;
				this.Trans.ShipperCompanyGuid = this.OrderContext.transaction.ShipperCompanyGuid;
				this.Trans.CarrierID = this.OrderContext.transaction.CarrierID;
				this.Trans.CarrierCode = this.OrderContext.transaction.CarrierCode;
				this.Trans.CarrierCompanyGuid = this.OrderContext.transaction.CarrierCompanyGuid;
				this.Trans.SCACCode = this.OrderContext.transaction.SCACCode;
				this.Trans.PONumber = this.OrderContext.transaction.PONumber;
			}

			else if (this.SupplyOrderContext != null)
			{
				this.Trans.OwnerID = this.SupplyOrderContext.transaction.OwnerID;
				this.Trans.OwnerCode = this.SupplyOrderContext.transaction.OwnerCode;
				this.Trans.OwnerCompanyGuid = this.SupplyOrderContext.transaction.OwnerCompanyGuid;
				this.Trans.ManagerID = this.SupplyOrderContext.transaction.ManagerID;
				this.Trans.ManagerCode = this.SupplyOrderContext.transaction.ManagerCode;
				this.Trans.ManagerCompanyGuid = this.SupplyOrderContext.transaction.ManagerCompanyGuid;
				this.Trans.ShipperID = this.SupplyOrderContext.transaction.ShipperID;
				this.Trans.ShipperCode = this.SupplyOrderContext.transaction.ShipperCode;
				this.Trans.ShipperCompanyGuid = this.SupplyOrderContext.transaction.ShipperCompanyGuid;
				this.Trans.SupplierID = this.SupplyOrderContext.transaction.SupplierID;
				this.Trans.SupplierCode = this.SupplyOrderContext.transaction.SupplierCode;
				this.Trans.SupplierCompanyGuid = this.SupplyOrderContext.transaction.SupplierCompanyGuid;
				this.Trans.PONumber = this.SupplyOrderContext.transaction.PONumber;
			}
		}

		private void PrepForAssociatedOrderTxIfNecessary()
		{
			// Is this an Order referenced transaction?
			this.OrderReferenceID = this.Request.GetQueryOrFormValue("OrderRef");
			this.OrderProduct = this.Request.GetQueryOrFormValue("OrderProduct");
			this.orderTxReferenceID = this.Request.GetQueryOrFormValue("OrderTxRef");

			if (!string.IsNullOrEmpty(this.OrderProduct))
			{
				// Get the product Guid
				this.OrderProductGuid =
					FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(this.security, this.OrderProduct));

				if (this.OrderProductGuid != Guid.Empty)
				{
					ProductClass product =
						FMChannelHelper.MakeCall<IProducts, ProductClass>(
							x => x.GetByProductAuthorizedCompanies(this.security, this.OrderProductGuid, false));
					this.OrderProductCode = product.Code;
				}
			}

			this.OrderContext = this.Session["OrderAssociatedTxContext"] as OrderAssociatedTxContext;
			this.SupplyOrderContext = this.Session["SupplyOrderAssociatedTxContext"] as SupplyOrderAssociatedTxContext;
		}

		private void RefreshSpecialInstructions()
		{
			if (this.Trans.LineItems.Count > 0)
			{
				string companyID = this.Request.GetQueryOrFormValue("__MYEVENTARGUMENT");
				CompanyClass shipTo = null;

				if (!string.IsNullOrEmpty(companyID))
				{
					shipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
											x => x.Get(this.security, x.GetIdentityGuid(this.security, companyID)));
				}

				if (shipTo != null)
				{
					foreach (LineItemDO lineItem in this.Trans.LineItems)
					{
						if (lineItem.ProductGuid != Guid.Empty)
						{
							string specialInstructions;
							Guid prodMapGuid;
							PRODUCT_MAP_TYPE mapType;
							this.GetSpecialInstructions(shipTo, lineItem.ProductGuid, out specialInstructions, out prodMapGuid, out mapType);
							lineItem.SpecialInstructionsNote = specialInstructions;
							lineItem.SpecialInstructionsNoteGuid = prodMapGuid;
							lineItem.SpecialInstructionsNoteProductMapType = mapType;
						}

						foreach (SubLineItemDO sublineItem in lineItem.SubLineItems)
						{
							if (sublineItem.ProductGuid != Guid.Empty)
							{
								string specialInstructions;
								Guid prodMapGuid;
								PRODUCT_MAP_TYPE mapType;
								this.GetSpecialInstructions(shipTo, sublineItem.ProductGuid, out specialInstructions, out prodMapGuid, out mapType);
								sublineItem.SpecialInstructionsNote = specialInstructions;
								sublineItem.SpecialInstructionsNoteGuid = prodMapGuid;
								sublineItem.SpecialInstructionsNoteProductMapType = mapType;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will retrieve the Gauge Readings from the 
		/// page.
		/// </summary>
		/// <param name="item"></param>
		private void RetrieveGaugeReading(DataGridItem item)
		{
			foreach (WebControl control in item.Controls)
			{
				if (string.IsNullOrEmpty(control.ID))
				{
					continue;
				}

				// Remove the row index from the end of the ID to find out which field it is.
				// Be careful, "LineItem SourceEquipmentModel1 5" is the 5th row of SourceEquipmentModel1.
				// If we remove the fieldKey.Trim() line and put a space in trimCharacters, the 1 gets removed.
				// So do it in 2 steps.
				char[] trimCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
				string fieldKey = control.ID.TrimEnd(trimCharacters);
				fieldKey = fieldKey.Trim();
				FieldGenerator field = this.TransactionFieldGenerator.GetFieldGenerator(fieldKey);

				if (field is IWeightReadingField)
				{
					try
					{
						field.Retrieve(control, this.Trans, this.TransContext, item.ItemIndex);
						continue;
					}
					catch (Exception exception)
					{
						this.ErrorHandler(exception);
					}
				}

				this.Logger.Error("TransactionDetail.RetrieveWeightReading(item) : Field " + fieldKey + " not found.");
			}
		}

		/// <summary>
		///	This method will retrieve transport line items from the controls.
		/// </summary>
		/// <param name="item"></param>
		private void RetrieveTransportLineItems(DataGridItem item)
		{
			foreach (WebControl control in item.Controls)
			{
				if (string.IsNullOrEmpty(control.ID))
				{
					continue;
				}

				// Remove the row index from the end of the ID to find out which field it is.
				// Be careful, "LineItem SourceEquipmentModel1 5" is the 5th row of SourceEquipmentModel1.
				// If we remove the fieldKey.Trim() line and put a space in trimCharacters, the 1 gets removed.
				// So do it in 2 steps.
				char[] trimCharacters = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
				string fieldKey = control.ID.TrimEnd(trimCharacters);
				fieldKey = fieldKey.Trim();

				FieldGenerator field = this.TransactionFieldGenerator.GetFieldGenerator(fieldKey);

				if (field is ITransportLineItemField)
				{
					field.Retrieve(control, this.Trans, this.TransContext, item.ItemIndex);
					continue;
				}

				this.Logger.Error("TransactionDetail.RetrieveTransportLineItems(item) : Field " + fieldKey + " not found.");
			}
		}

		/// <summary>
		/// The set add sub-line item button state.
		/// </summary>
		/// <param name="e">
		/// The e.
		/// </param>
		private void SetAddSubLineItemButtonState(DataGridItemEventArgs e)
		{
			var addSubLine = e.Item.FindControl("AddSubLineItemButton") as LinkButton;

			if (addSubLine != null)
			{
				int lineItemIndex, sublineItemIndex;

				this.GetItemIndices(e.Item, out lineItemIndex, out sublineItemIndex);

				addSubLine.Visible = sublineItemIndex < 0;
			}
		}

		/// <summary>
		/// The set add associated transaction button state.
		/// </summary>
		/// <param name="e">
		/// The e.
		/// </param>
		private void SetAddTxButtonState(DataGridItemEventArgs e)
		{
			if (e.Item.FindControl("lbAddAssociatedTx1") is LinkButton addButton)
			{
				if (this.AccountingSite.CurrentSite.SiteGroup || this.TransContext.aliasClass.AssociatedTransactionAliasGuid == Guid.Empty)
				{
					addButton.Enabled = false;
				}
			}
			else
			{
				addButton = e.Item.FindControl("lbAddAssociatedTx2") as LinkButton;

				if (addButton != null)
				{
					if (this.AccountingSite.CurrentSite.SiteGroup || this.TransContext.aliasClass.AssociatedTransactionAliasGuid == Guid.Empty)
					{
						addButton.Enabled = false;
					}
				}
			}
		}

		private void SetCloseButton()
		{
			this.CloseButton.Enabled = true;
		}

		private void SetCombineButton()
		{
			// Different action if the transaction is already deleted. (24-Jun-2009 IGO)
			if (this.Trans.DeleteFlag
					|| this.Session[CombineTransKey] != null
				|| this.Trans.ReversalType == TransactionDO.ReversalWithUpdate
				|| this.Trans.ReversalType == TransactionDO.UpdateOriginal
					|| this.Trans.ReversalType == TransactionDO.Original
				|| this.Trans.ReversalType == TransactionDO.Reversal
					|| this.bTransIDBeingLoaded
				|| this.Trans.TransTypeID != TransactionTypes.T5_PrimaryDisbursement
				|| !this.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
				|| FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsTFMDKey()))
			{
				this.CombineBtn.Enabled = false;
			}
			else
			{
				this.CombineBtn.Enabled = true;
			}
		}

		protected virtual void SetDeleteButton(bool editable)
		{
			if (this.Trans.DeleteFlag)
			{
				this.DeleteButton.ConfirmationText = "This transaction is deleted.\nClick OK to continue with the undelete.";

				string undeletestring = "Undelete";
				undeletestring = this.GetTranslatedText(undeletestring);
				this.DeleteButton.Text = undeletestring;
			}

			if (!editable
				&& !this.Trans.DeleteFlag
				&& this.Trans.ReversalType != TransactionDO.Reversal)
			{
				this.DeleteButton.Enabled = false;
				return;
			}

			// Posted or Pending transactions may not be deleted
			if ((this.Trans.Status == TransactionStatus.Pending || this.Trans.Status == TransactionStatus.Posted)
				&& !this.Trans.DeleteFlag)
			{
				this.DeleteButton.Enabled = false;
				return;
			}


			if (this.TransContext.mode == TransactionContext.Mode.Add)
			{
				this.DeleteButton.Enabled = false;
				return;
			}

			if (this.TransContext.mode != TransactionContext.Mode.Edit
				&& !this.Trans.DeleteFlag
				&& this.Trans.ReversalType != TransactionDO.Reversal)
			{
				this.DeleteButton.Enabled = false;
				return;
			}

			if (this.Trans.DeleteFlag)
			{
				if (this.security.HasRight(RIGHT.UNDELETE_TRANSACTION_DATA))
				{
					this.DeleteButton.Enabled = true;
				}

				else
				{
					this.DeleteButton.Enabled = false;
				}
				return;
			}

			this.DeleteButton.Enabled = true;
		}

		private void SetGaugeReadingsDeleteAndEditButtonState(DataGridItemEventArgs e)
		{
			if (!this.IsTransactionEditable)
			{
				var deleteBtn = (LinkButton)e.Item.FindControl("DeleteButton2");

				if (deleteBtn != null)
				{
					deleteBtn.Enabled = false;
				}

				var editBtn = (LinkButton)e.Item.FindControl("EditButton2");

				if (editBtn != null)
				{
					editBtn.Enabled = false;
				}
			}
			else
			{
				// if we are in edit mode disable the delete and edit buttons
				if (this.GaugeReadingsDataGrid.EditItemIndex > -1)
				{
					var editBtn = (LinkButton)e.Item.FindControl("EditButton2");

					if (editBtn != null)
					{
						editBtn.Enabled = false;
					}

					var deleteBtn = (LinkButton)e.Item.FindControl("DeleteButton2");

					if (deleteBtn != null)
					{
						deleteBtn.Enabled = false;
					}
				}
			}
		}

		/// <summary>
		///	This method handles the line item delete and edit button state. It is called
		///	by the Item_Bound method. If the transaction line item collation is empty,
		///	the method will do nothing.
		/// </summary>
		/// <param name="e"></param>
		private void SetLineItemDeleteAndEditButtonState(DataGridItemEventArgs e)
		{
			int lineItemIndex, sublineItemIndex;

			// If there are no line items, then just return.
			if (this.Trans.LineItems.Count == 0)
			{
				return;
			}

			this.GetItemIndices(e.Item, out lineItemIndex, out sublineItemIndex);

			LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];

			if (this.IsLineItemEditable(lineItem) == false)
			{
				var addSubItemBtn = (LinkButton)e.Item.FindControl("AddSubLineItemButton");

				if (addSubItemBtn != null)
				{
					addSubItemBtn.Enabled = false;
				}

				var editBtn = (LinkButton)e.Item.FindControl("EditButton");

				if (editBtn != null)
				{
					editBtn.Enabled = false;
				}

				var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");

				if (deleteButton != null)
				{
					deleteButton.Enabled = false;
				}
			}
			else
			{
				if (lineItem.AdditiveProfileGuid == Guid.Empty
					&& (lineItem.ProductGuid == Guid.Empty
						|| lineItem.ProductType != ProductClass.ProductTypeID(ProductType.BlendProduct)))
				{
					var addSubItemBtn = (LinkButton)e.Item.FindControl("AddSubLineItemButton");

					if (addSubItemBtn != null)
					{
						addSubItemBtn.Enabled = false;
					}
				}
				if (this.LineItemDataGrid.EditItemIndex > -1 && e.Item.ItemIndex != this.LineItemDataGrid.EditItemIndex)
				{
					var editBtn = (LinkButton)e.Item.FindControl("EditButton");

					if (editBtn != null)
					{
						editBtn.Enabled = false;
					}
				}

				//disable the update and cancel buttons so they can only be clicked once
				if (this.LineItemDataGrid.EditItemIndex > -1 && e.Item.ItemIndex == this.LineItemDataGrid.EditItemIndex)
				{
					var updateBtn = (LinkButton)e.Item.FindControl("UpdateButton");
					var cancelBtn = (LinkButton)e.Item.FindControl("CancelButton");

					string cancelJquery = "";
					string updateJquery = "";

					if (cancelBtn != null)
					{
						cancelJquery = "$('[id=\"" + cancelBtn.ClientID
											+ "\"]').attr('onclick','').click(function(e){e.preventDefault();});";
					}

					if (updateBtn != null)
					{
						updateJquery = "$('[id=\"" + updateBtn.ClientID
										+ "\"]').attr('onclick','').click(function(e){e.preventDefault();});";
					}

					updateBtn?.Attributes.Add("onClick", updateJquery + cancelJquery);

					cancelBtn?.Attributes.Add("onClick", updateJquery + cancelJquery);
				}
			}

			if (this.LineItemDataGrid.EditItemIndex > -1)
			{
				var deleteBtn = (LinkButton)e.Item.FindControl("DeleteButton");

				if (deleteBtn != null)
				{
					deleteBtn.Enabled = false;
				}
			}
		}

		/// <summary>
		///	This method handles the process of setting the Line Item Equipment to the header equipment
		///	when the Line Item Source or Destination equipment is not configured in the transaction alias
		/// </summary>
		/// <returns></returns>
		private void SetLineItemEquipment()
		{
			if (this.TransContext.aliasClass.LineItemFieldCollection.Find("DestinationRegistrationID") == null)
			{
				EquipmentDO equipmentDO = null;
				if (this.TransContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID1") != null
					&& !string.IsNullOrEmpty(this.Trans.DestinationEQ1.RegistrationID)
					&& EquipmentTypeClass.HasCompartments(EquipmentTypeClass.Type(this.Trans.DestinationEQ1.EquipmentType)))
				{
					equipmentDO = this.Trans.DestinationEQ1;
				}
				else if (this.TransContext.aliasClass.TransactionFieldCollection.Find("DestinationRegistrationID2") != null
							&& !string.IsNullOrEmpty(this.Trans.DestinationEQ2.RegistrationID)
							&& EquipmentTypeClass.HasCompartments(EquipmentTypeClass.Type(this.Trans.DestinationEQ2.EquipmentType)))
				{
					equipmentDO = this.Trans.DestinationEQ2;
				}

				if (equipmentDO != null)
				{
					foreach (LineItemDO lineItemDO in this.Trans.LineItems)
					{
						lineItemDO.DestinationEQ.RegistrationID = equipmentDO.RegistrationID;
						lineItemDO.DestinationEQ.SerialNumber = equipmentDO.SerialNumber;
						lineItemDO.DestinationEQ.EquipmentModel = equipmentDO.EquipmentModel;
						lineItemDO.DestinationEQ.EquipmentType = equipmentDO.EquipmentType;

						if (equipmentDO.EquipmentGuid != Guid.Empty)
						{
							lineItemDO.DestinationEQ.EquipmentGuid = equipmentDO.EquipmentGuid;
						}
						else
						{
							lineItemDO.DestinationEQ.EquipmentGuid = Guid.Empty;
						}
					}
				}
			}

			if (this.TransContext.aliasClass.LineItemFieldCollection.Find("SourceRegistrationID") == null)
			{
				EquipmentDO equipmentDO = null;
				if (this.TransContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID1") != null
					&& !string.IsNullOrEmpty(this.Trans.SourceEQ1.RegistrationID)
					&& EquipmentTypeClass.HasCompartments(EquipmentTypeClass.Type(this.Trans.SourceEQ1.EquipmentType)))
				{
					equipmentDO = this.Trans.SourceEQ1;
				}
				else if (this.TransContext.aliasClass.TransactionFieldCollection.Find("SourceRegistrationID2") != null
							&& !string.IsNullOrEmpty(this.Trans.SourceEQ2.RegistrationID)
							&& EquipmentTypeClass.HasCompartments(EquipmentTypeClass.Type(this.Trans.SourceEQ2.EquipmentType)))
				{
					equipmentDO = this.Trans.SourceEQ2;
				}

				if (equipmentDO != null)
				{
					foreach (LineItemDO lineItemDO in this.Trans.LineItems)
					{
						lineItemDO.SourceEQ.RegistrationID = equipmentDO.RegistrationID;
						lineItemDO.SourceEQ.SerialNumber = equipmentDO.SerialNumber;
						lineItemDO.SourceEQ.EquipmentModel = equipmentDO.EquipmentModel;
						lineItemDO.SourceEQ.EquipmentType = equipmentDO.EquipmentType;
						lineItemDO.SourceEQ.EquipmentType = equipmentDO.EquipmentType;

						if (equipmentDO.EquipmentGuid != Guid.Empty)
						{
							lineItemDO.SourceEQ.EquipmentGuid = equipmentDO.EquipmentGuid;
						}
						else
						{
							lineItemDO.SourceEQ.EquipmentGuid = Guid.Empty;
						}
					}
				}
			}
		}

		private void SetNextPrevious()
		{
			var list = this.Session[TransactionDetailList.TransactionDetailListKey] as TransactionDetailList;

			this.PreviousButton.Visible = false;
			this.NextButton.Visible = false;

			if (list == null)
			{
				return;
			}
			if (this.TransContext.mode == TransactionContext.Mode.Add)
			{
				return;
			}
			if (list.CurrentIndex > 0)
			{
				this.PreviousButton.Visible = true;
				this.PreviousButton.Enabled = true;
			}
			if (list.CurrentIndex < list.TransactionIDList.Count - 1)
			{
				this.NextButton.Visible = true;
				this.NextButton.Enabled = true;
			}
		}

		private bool SetTailNumberControlCallBack(Control inControl)
		{
			if (inControl == null)
			{
				return false;
			}

			ControlCollection controls = inControl.Controls;

			foreach (Control control in controls)
			{
				if (control.HasControls())
				{
					bool done = this.SetTailNumberControlCallBack(control);
					if (done)
					{
						return true;
					}
				}
				else
				{
					var tableCell = control as TableCell;

					if (tableCell != null)
					{
						var cell = tableCell;

						if (cell.Text == "Tail Number" && cell.ClientID.StartsWith("FieldLabel "))
						{
							string controlName = "TransactionFields.UserDataListFG" + cell.ClientID.Substring(11);
							var htmlSelect = (HtmlSelect)this.FieldTable.FindControl(controlName);
							htmlSelect?.Attributes.Add("onchange", "__mydoPostBack('TAIL_NUMBER_CHANGED', options[selectedIndex].value)");
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		///	This method handles the line item delete and edit button state. It is called
		///	by the Item_Bound method. If the transaction line item collation is empty,
		///	the method will do nothing.
		/// </summary>
		/// <param name="e"></param>
		private void SetTransportItemDeleteAndEditButtonState(DataGridItemEventArgs e)
		{
			// If there are no transport items, then just return.
			if (this.Trans.TransportInfoList.Count == 0)
			{
				return;
			}

			if (!this.IsTransactionEditable)
			{
				var editBtn = (LinkButton)e.Item.FindControl("EditButton3");

				if (editBtn != null)
				{
					editBtn.Enabled = false;
				}

				var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton3");

				if (deleteButton != null)
				{
					deleteButton.Enabled = false;
				}
			}
		}

		private void SetViewTxButtonState(DataGridItemEventArgs e)
		{
			var txViewBtn = (LinkButton)e.Item.FindControl("FMViewAssociatedTxLinkButton1");

			if (txViewBtn != null)
			{
				// The View button is only available when a transaction has been saved.  
				bool viewEnabled = this.Trans.LineItems.Count > 0;

				if (viewEnabled)
				{
					int lineItemIndex, sublineItemIndex;
					this.GetItemIndices(e.Item, out lineItemIndex, out sublineItemIndex);

					LineItemDO lineItem = this.Trans.LineItems[lineItemIndex];

					if (lineItem == null || this.LineItemDataGrid.EditItemIndex > -1)
					{
						viewEnabled = false;
					}
					else
					{
						if (string.IsNullOrEmpty(this.TransContext.aliasClass.AssociatedAlias))
						{
							viewEnabled = false;
						}
						else if (this.TransContext.aliasClass.AssociatedAlias.Equals("{None}"))
						{
							viewEnabled = false;
						}

						viewEnabled = (lineItem.AssociatedTransactions.Count > 0) || viewEnabled;
					}
				}

				txViewBtn.Enabled = viewEnabled;
			}
		}

		/// <summary>
		///	This method will return True if the transaction list product is one of the
		///	products in the Line Item list. It will return False otherwise.
		/// </summary>
		/// <param name="lineItems">Line items to check</param>
		/// <param name="currentProduct">Product to look for</param>
		/// <returns>True if the product appears in the transaction already</returns>
		private bool TransListHasProduct(List<LineItemDO> lineItems, string currentProduct)
		{
			bool hasProduct = false;

			if (!string.IsNullOrEmpty(currentProduct))
			{
				foreach (LineItemDO lineItemDO in lineItems)
				{
					if (!string.IsNullOrEmpty(lineItemDO.Product))
					{
						if (currentProduct.Equals(lineItemDO.Product))
						{
							hasProduct = true;
							break;
						}
					}
				}
			}

			return hasProduct;
		}

		/// <summary>
		/// Transfers to new tx.
		/// </summary>
		/// <param name="lineItem">The line item.</param>
		/// <param name="sender">The sender.</param>
		/// <exception cref="System.Exception">There is no associated alias set for this Order alias type.  Cannot create new transaction.</exception>
		private void TransferToNewTx(LineItemDO lineItem, object sender, int index)
		{
			try
			{
				this.NoSaveErrors = true;

				if (this.IsTransactionEditable && !this.SaveProcessing(sender))
				{
					return;
				}

				if (lineItem.TransactionLineItemGuid == Guid.Empty)
				{
					lineItem = Trans.LineItems[index];
				}

				if (string.IsNullOrEmpty(this.TransContext.aliasClass.AssociatedAlias))
				{
					// There is no alias associated with this order type for transaction creation.
					throw new Exception("There is no associated alias set for this Order alias type.  Cannot create new transaction.");
				}

				// Save default information 
				string redirect = "transactiondetail.aspx?" + ModeKey + "=ADD&TransAlias="
										+ this.TransContext.aliasClass.AssociatedAlias;

				if (TransactionTypes.T17_Order == this.Trans.TransTypeID)
				{
					var detailList = new TransactionDetailList();
					detailList.CurrentIndex = 0;
					detailList.TransactionIDList.Add(this.Trans.TransID);

					var context = new OrderAssociatedTxContext
					{
						TransactionLineItemGuid = lineItem.TransactionLineItemGuid,
						ReturnURL = String.Format("{0}?CSRFToken={1}", this.Request.Path, this.Request.QueryString.Get("CSRFToken")),
						OrderNumber = this.Trans.DocumentNumber,
						CustomerOrderNumber = this.Trans.PONumber,
						LineNumber = lineItem.LineNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
						Product = lineItem.Product,
						transaction = this.Trans,
						DetailList = detailList
					};

					this.Session["OrderAssociatedTxContext"] = context;

					redirect += "&OrderRef=" + lineItem.TransactionLineItemGuid;
					redirect += "&OrderProduct=" + lineItem.Product;
					redirect += "&OrderTxRef=" + this.Trans.TransID;
				}
				else if (TransactionTypes.T18_SupplyOrder == this.Trans.TransTypeID)
				{
					var detailList = new TransactionDetailList();
					detailList.CurrentIndex = 0;
					detailList.TransactionIDList.Add(this.Trans.TransID);

					var supplyContext = new SupplyOrderAssociatedTxContext
					{
						TransactionLineItemGuid = lineItem.TransactionLineItemGuid,
						ReturnURL = String.Format("{0}?CSRFToken={1}", this.Request.Path, this.Request.QueryString.Get("CSRFToken")),
						OrderNumber = this.Trans.DocumentNumber,
						CustomerOrderNumber = this.Trans.PONumber,
						LineNumber = lineItem.LineNumber?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
						Product = lineItem.Product,
						transaction = this.Trans,
						DetailList = detailList
					};
					this.Session["SupplyOrderAssociatedTxContext"] = supplyContext;

					redirect += "&OrderRef=" + lineItem.TransactionLineItemGuid;
					redirect += "&OrderProduct=" + lineItem.Product;
					redirect += "&OrderTxRef=" + this.Trans.TransID;
				}

				this.Redirect(redirect);
				this.Context.ApplicationInstance.CompleteRequest();
			}
			catch (Exception exception)
			{
				this.ErrorHandler(exception);
			}
		}

		/// <summary>
		/// This method updates the quantity fields (gross, net, mass), temperature, density, and VCF for a
		/// non-multi line item. These fields are calculated and and need update the UI after an apply.
		/// </summary>
		private void SetCalulatedQuantitiesWhenInSingleLineMode()
		{
			if (this.TransContext.aliasClass.MultipleLineItems == false && this.Trans.LineItems.Count > 0)
			{
				var txtGrossQuantity = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemGrossQuantityFG");

				if (txtGrossQuantity != null)
				{
					FieldGenerator lineItemGrossQuantity = this.TransactionFieldGenerator.GetFieldGenerator("LineItem GrossQuantity");

					if (lineItemGrossQuantity != null)
					{
						lineItemGrossQuantity.SetDataValue(this.Trans.LineItems[0].Quantity.GrossInventoryChange);
						txtGrossQuantity.Text = lineItemGrossQuantity.GetFormattedValue();
					}
				}

				var txtNetQuantity = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemNetQuantityFG");

				if (txtNetQuantity != null)
				{
					FieldGenerator lineItemNetQuantity = this.TransactionFieldGenerator.GetFieldGenerator("LineItem NetQuantity");

					if (lineItemNetQuantity != null)
					{
						lineItemNetQuantity.SetDataValue(this.Trans.LineItems[0].Quantity.NetInventoryChange);
						txtNetQuantity.Text = lineItemNetQuantity.GetFormattedValue();
					}
				}

				var txtMassQuantity = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemMassQuantityFG");

				if (txtMassQuantity != null)
				{
					FieldGenerator lineItemMassQuantity = this.TransactionFieldGenerator.GetFieldGenerator("LineItem MassQuantity");

					if (lineItemMassQuantity != null)
					{
						lineItemMassQuantity.SetDataValue(this.Trans.LineItems[0].Quantity.MassInventoryChange);
						txtMassQuantity.Text = lineItemMassQuantity.GetFormattedValue();
					}
				}

				var txtPackage = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemPackageQuantityFG");

				if (txtPackage != null)
				{
					FieldGenerator lineItemPackage = this.TransactionFieldGenerator.GetFieldGenerator("LineItem PackageQuantity");

					if (lineItemPackage != null)
					{
						lineItemPackage.SetDataValue(this.Trans.LineItems[0].Quantity.Package);
						txtPackage.Text = lineItemPackage.GetFormattedValue();
					}
				}

				var txtDensity = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemDensityFG");

				if (txtDensity != null)
				{
					FieldGenerator lineItemDensity = this.TransactionFieldGenerator.GetFieldGenerator("LineItem Density");

					if (lineItemDensity != null)
					{
						lineItemDensity.SetDataValue(this.Trans.LineItems[0].Density);
						txtDensity.Text = lineItemDensity.GetFormattedValue();
					}
				}

				var txtVcf = (TextBox)this.FieldTable.FindControl("TransactionFields.LineItemVCF_FG");

				if (txtVcf != null)
				{
					FieldGenerator lineItemVcf = this.TransactionFieldGenerator.GetFieldGenerator("LineItem Vcf");

					if (lineItemVcf != null)
					{
						lineItemVcf.SetDataValue(this.Trans.LineItems[0].VCF);
						txtVcf.Text = lineItemVcf.GetFormattedValue();
					}
				}
			}

		}

        private void RegisterHeaderScript(string virtualPath)
        {
            var script = new System.Web.UI.HtmlControls.HtmlGenericControl("script");
            script.Attributes["type"] = "text/javascript";
            script.Attributes["src"] = ResolveUrl(virtualPath);
            Header.Controls.Add(script);
        }
		#endregion
	}
}
