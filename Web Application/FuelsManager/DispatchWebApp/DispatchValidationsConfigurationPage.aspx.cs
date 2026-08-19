// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchValidationsConfigurationPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchValidationsConfigurationPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	/// Partial definition of the DispatchValidationsConfigurationPage class.  Provides functionality for the
	/// Dispatch Validations Configuration web page.
	/// </summary>
	public partial class DispatchValidationsConfigurationPage : FMFormBase
	{
		/// <summary>
		/// Executes when the page is loaded.  Disables command buttons if security requirements are not satisfied.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!Page.IsPostBack)
				{
					// Retrieve the current Dispatch Configuration from the database
					var dispatchConfig = new DispatchConfigurationClass();

					bool entityAssigned = false;
					FMChannelHelper.MakeCall<IDispatchConfigurations>(
						dispatchConfigs =>
						{
							Guid dispatchConfigGuid = dispatchConfigs.GetIdentityGuidBySiteIdAndAssigned(
								Security, Security.SiteGuid, DispatchConfigurationClass.DefaultId, true, out entityAssigned);

							if (dispatchConfigGuid != Guid.Empty)
							{
								dispatchConfig = dispatchConfigs.Get(this.Security, dispatchConfigGuid);
							}
						});

					if (entityAssigned || !Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.EnableControls(false);
					}

					this.chkQuantityNotZero.Checked = dispatchConfig.QuantityNotZeroCheck;
					this.chkExactlyOneManager.Checked = dispatchConfig.ExactlyOneManagerCheck;
					this.chkExactlyOneOwner.Checked = dispatchConfig.ExactlyOneOwnerCheck;
					this.chkDispatchFuelAdditiveFlag.Checked = dispatchConfig.DispatchFuelAdditiveFlagCheck;
					this.chkFastLogFuelAdditiveFlag.Checked = dispatchConfig.FastLogFuelAdditiveFlagCheck;
					this.chkFillstandVolumeWithinTolerance.Checked = dispatchConfig.FillstandVolumeWithinToleranceCheck;
					this.chkReturnToBulkVolumeWithinTolerance.Checked = dispatchConfig.ReturnToBulkVolumeWithinToleranceCheck;
					this.chkRecirculationVolumesGreaterThanZero.Checked = dispatchConfig.RecirculationVolumesGreaterThanZeroCheck;
					this.chkOperatorIsIn.Checked = dispatchConfig.OperatorIsInCheck;
					this.chkOperatorNotAssigned.Checked = dispatchConfig.OperatorNotAssignedCheck;
					this.chkOperatorHasRequiredTraining.Checked = dispatchConfig.OperatorHasRequiredTrainingCheck;
					this.chkOperatorTrainingNotExpired.Checked = dispatchConfig.OperatorTrainingNotExpiredCheck;
					this.chkOperatorNotLockedOut.Checked = dispatchConfig.OperatorNotLockedOutCheck;
					this.chkOperatorHasRequiredQualifications.Checked = dispatchConfig.OperatorHasRequiredQualificationsCheck;
					this.chkOperatorQualificationsNotExpired.Checked = dispatchConfig.OperatorQualificationsNotExpiredCheck;
					this.chkDefuelStatusCheck.Checked = dispatchConfig.DefuelStatusCheck;
					this.chkRefuelStatusCheck.Checked = dispatchConfig.RefuelStatusCheck;
					this.chkEquipmentFuelGrade.Checked = dispatchConfig.EquipmentFuelGradeCheck;
					this.chkEquipmentNotLockedOut.Checked = dispatchConfig.EquipmentNotLockedOutCheck;
					this.chkEquipmentNotAssigned.Checked = dispatchConfig.EquipmentNotAssignedCheck;
					this.chkEquipmentInService.Checked = dispatchConfig.EquipmentInServiceCheck;
					this.chkTagLicenseNotExpired.Checked = dispatchConfig.TagLicenseNotExpiredCheck;
					this.chkTestInspectionNotExpired.Checked = dispatchConfig.TestInspectionNotExpiredCheck;
					this.chkQualityControlCheckupDate.Checked = dispatchConfig.QualityControlCheckupDateCheck;
					this.chkCautionQualityTagCheck.Checked = dispatchConfig.CautionQualityTagCheck;
					this.chkWarningQualityTagCheck.Checked = dispatchConfig.WarningQualityTagCheck;
					this.chkDangerQualityTagCheck.Checked = dispatchConfig.DangerQualityTagCheck;
					this.chkEquipmentRequired.Checked = dispatchConfig.EquipmentRequired;
					this.chkPersonnelRequired.Checked = dispatchConfig.PersonnelRequired;

					this.Session["DispatchConfiguration"] = dispatchConfig;
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		/// <summary>
		/// Enables or disables all the data entry controls.
		/// </summary>
		/// <param name="enable">If true controls are enables otherwise they are disabled.</param>
		private void EnableControls(bool enable)
		{
			this.chkQuantityNotZero.Enabled = enable;
			this.chkExactlyOneManager.Enabled = enable;
			this.chkExactlyOneOwner.Enabled = enable;
			this.chkFastLogFuelAdditiveFlag.Enabled = enable;
			this.chkFillstandVolumeWithinTolerance.Enabled = enable;
			this.chkReturnToBulkVolumeWithinTolerance.Enabled = enable;
			this.chkDispatchFuelAdditiveFlag.Enabled = enable;
			this.chkOperatorIsIn.Enabled = enable;
			this.chkRecirculationVolumesGreaterThanZero.Enabled = enable;
			this.chkOperatorNotAssigned.Enabled = enable;
			this.chkOperatorHasRequiredTraining.Enabled = enable;
			this.chkOperatorTrainingNotExpired.Enabled = enable;
			this.chkOperatorNotLockedOut.Enabled = enable;
			this.chkOperatorHasRequiredQualifications.Enabled = enable;
			this.chkOperatorQualificationsNotExpired.Enabled = enable;
			this.chkDefuelStatusCheck.Enabled = enable;
			this.chkRefuelStatusCheck.Enabled = enable;
			this.chkEquipmentFuelGrade.Enabled = enable;
			this.chkEquipmentNotLockedOut.Enabled = enable;
			this.chkEquipmentNotAssigned.Enabled = enable;
			this.chkEquipmentInService.Enabled = enable;
			this.chkTagLicenseNotExpired.Enabled = enable;
			this.chkTestInspectionNotExpired.Enabled = enable;
			this.chkQualityControlCheckupDate.Enabled = enable;
			this.chkCautionQualityTagCheck.Enabled = enable;
			this.chkWarningQualityTagCheck.Enabled = enable;
			this.chkDangerQualityTagCheck.Enabled = enable;
			this.chkEquipmentRequired.Enabled = enable;
			this.chkPersonnelRequired.Enabled = enable;
			this.checkAllButton.Enabled = enable;
			this.clearAllButton.Enabled = enable;
			this.applyButton.Enabled = enable && this.Security.HasRight(RIGHT.CONFIGURE_DISPATCH_VALIDATIONS);
		}

		/// <summary>
		/// Sets all the check box buttons to the specified state.
		/// </summary>
		/// <param name="checkState">The check state</param>
		private void SetAllCheckButtons(bool checkState)
		{
			this.chkQuantityNotZero.Checked = checkState;
			this.chkExactlyOneManager.Checked = checkState;
			this.chkExactlyOneOwner.Checked = checkState;
			this.chkFastLogFuelAdditiveFlag.Checked = checkState;
			this.chkFillstandVolumeWithinTolerance.Checked = checkState;
			this.chkReturnToBulkVolumeWithinTolerance.Checked = checkState;
			this.chkDispatchFuelAdditiveFlag.Checked = checkState;
			this.chkOperatorIsIn.Checked = checkState;
			this.chkRecirculationVolumesGreaterThanZero.Checked = checkState;
			this.chkOperatorNotAssigned.Checked = checkState;
			this.chkOperatorHasRequiredTraining.Checked = checkState;
			this.chkOperatorTrainingNotExpired.Checked = checkState;
			this.chkOperatorNotLockedOut.Checked = checkState;
			this.chkOperatorHasRequiredQualifications.Checked = checkState;
			this.chkOperatorQualificationsNotExpired.Checked = checkState;
			this.chkDefuelStatusCheck.Checked = checkState;
			this.chkRefuelStatusCheck.Checked = checkState;
			this.chkEquipmentFuelGrade.Checked = checkState;
			this.chkEquipmentNotLockedOut.Checked = checkState;
			this.chkEquipmentNotAssigned.Checked = checkState;
			this.chkEquipmentInService.Checked = checkState;
			this.chkTagLicenseNotExpired.Checked = checkState;
			this.chkTestInspectionNotExpired.Checked = checkState;
			this.chkQualityControlCheckupDate.Checked = checkState;
			this.chkCautionQualityTagCheck.Checked = checkState;
			this.chkWarningQualityTagCheck.Checked = checkState;
			this.chkDangerQualityTagCheck.Checked = checkState;
			this.chkEquipmentRequired.Checked = checkState;
			this.chkPersonnelRequired.Checked = checkState;
		}

		/// <summary>
		/// Sets all the check buttons to checked.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void CheckAllButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				this.SetAllCheckButtons(true);
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		/// <summary>
		/// Sets all the check buttons to unchecked.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void ClearAllButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				this.SetAllCheckButtons(false);
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		/// <summary>
		/// Saves the current dispatch validations configuration to the database.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void ApplyButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				var dispatchConfig = (DispatchConfigurationClass)Session["DispatchConfiguration"];

				dispatchConfig.QuantityNotZeroCheck = this.chkQuantityNotZero.Checked;
				dispatchConfig.ExactlyOneManagerCheck = this.chkExactlyOneManager.Checked;
				dispatchConfig.ExactlyOneOwnerCheck = this.chkExactlyOneOwner.Checked;
				dispatchConfig.DispatchFuelAdditiveFlagCheck = this.chkDispatchFuelAdditiveFlag.Checked;
				dispatchConfig.FastLogFuelAdditiveFlagCheck = this.chkFastLogFuelAdditiveFlag.Checked;
				dispatchConfig.FillstandVolumeWithinToleranceCheck = this.chkFillstandVolumeWithinTolerance.Checked;
				dispatchConfig.ReturnToBulkVolumeWithinToleranceCheck = this.chkReturnToBulkVolumeWithinTolerance.Checked;
				dispatchConfig.RecirculationVolumesGreaterThanZeroCheck = this.chkRecirculationVolumesGreaterThanZero.Checked;
				dispatchConfig.OperatorIsInCheck = this.chkOperatorIsIn.Checked;
				dispatchConfig.OperatorNotAssignedCheck = this.chkOperatorNotAssigned.Checked;
				dispatchConfig.OperatorHasRequiredTrainingCheck = this.chkOperatorHasRequiredTraining.Checked;
				dispatchConfig.OperatorTrainingNotExpiredCheck = this.chkOperatorTrainingNotExpired.Checked;
				dispatchConfig.OperatorNotLockedOutCheck = this.chkOperatorNotLockedOut.Checked;
				dispatchConfig.OperatorHasRequiredQualificationsCheck = this.chkOperatorHasRequiredQualifications.Checked;
				dispatchConfig.OperatorQualificationsNotExpiredCheck = this.chkOperatorQualificationsNotExpired.Checked;
				dispatchConfig.DefuelStatusCheck = this.chkDefuelStatusCheck.Checked;
				dispatchConfig.RefuelStatusCheck = this.chkRefuelStatusCheck.Checked;
				dispatchConfig.EquipmentFuelGradeCheck = this.chkEquipmentFuelGrade.Checked;
				dispatchConfig.EquipmentNotLockedOutCheck = this.chkEquipmentNotLockedOut.Checked;
				dispatchConfig.EquipmentNotAssignedCheck = this.chkEquipmentNotAssigned.Checked;
				dispatchConfig.EquipmentInServiceCheck = this.chkEquipmentInService.Checked;
				dispatchConfig.TagLicenseNotExpiredCheck = this.chkTagLicenseNotExpired.Checked;
				dispatchConfig.TestInspectionNotExpiredCheck = this.chkTestInspectionNotExpired.Checked;
				dispatchConfig.QualityControlCheckupDateCheck = this.chkQualityControlCheckupDate.Checked;
				dispatchConfig.CautionQualityTagCheck = this.chkCautionQualityTagCheck.Checked;
				dispatchConfig.WarningQualityTagCheck = this.chkWarningQualityTagCheck.Checked;
				dispatchConfig.DangerQualityTagCheck = this.chkDangerQualityTagCheck.Checked;
				dispatchConfig.EquipmentRequired = this.chkEquipmentRequired.Checked;
				dispatchConfig.PersonnelRequired = this.chkPersonnelRequired.Checked;

				// Add a new configuration if the current one does not exist in the database
				FMChannelHelper.MakeCall<IDispatchConfigurations>(
					dispatchConfigs =>
					{
						if (dispatchConfig.IdentityGuid == Guid.Empty)
						{
							dispatchConfig.IdentityGuid = dispatchConfigs.Add(this.Security, dispatchConfig);
						}
						else
						{
							dispatchConfigs.Modify(this.Security, dispatchConfig);
						}

						this.Session["DispatchConfiguration"] = dispatchConfigs.Get(this.Security, dispatchConfig.IdentityGuid);
					});
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}
	}
}
