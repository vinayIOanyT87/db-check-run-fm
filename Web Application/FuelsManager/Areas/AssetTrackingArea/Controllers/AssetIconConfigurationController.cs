namespace FuelsManager.Areas.AssetTrackingArea.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.AssetTrackingArea.ViewModels;
	using FuelsManager.Areas.Controllers;

	public class AssetIconConfigurationController : FMBaseController
	{
		#region Private memembers
		private enum Buttons { None, New, Ok, Cancel };
		#endregion

		#region Action methods
		// GET: AssetTrackingArea/AssetIconConfiguration
		public ActionResult IconConfiguration(AssetIconConfigurationModel postedModel)
		{
			bool usePostedModel = true;
			AssetIconConfigurationModel iconConfigModel = null;

			try
			{
				if (postedModel == null)
				{
					usePostedModel = false;

					// Once we have entity assignment, then the guid will be coming
					// from the summary page.
					iconConfigModel = this.GetIconConfiguration(Guid.Empty);
					iconConfigModel.IsEditable = this.IsEditable();

					return this.View(iconConfigModel);
				}

				if (postedModel.PostFromPopup)
				{
					this.ModelState.Clear();
					postedModel.PostFromPopup = false;
					postedModel.IsEditable = this.IsEditable();
					return this.View(postedModel);
				}

				Buttons buttonAction = this.WhichButtonWasPressed();

				if (buttonAction == Buttons.Ok)
				{
					if (string.IsNullOrEmpty(postedModel.IconConfigurationId))
					{
						postedModel.IsEditable = this.IsEditable();

						this.ModelState.AddModelError("Error", "Icon ID is required.");
						return this.View(postedModel);
					}

					string errorMsg;
					bool duplicateIcon = this.DuplicateIcons(postedModel, out errorMsg);

					if (duplicateIcon)
					{
						postedModel.IsEditable = this.IsEditable();

						this.ModelState.AddModelError("Error", errorMsg);
						return this.View(postedModel);
					}

					if (postedModel.AssetTrackingIconConfigurationGuid == Guid.Empty)
					{
						this.InsertRecordToDatabase(postedModel);
					}
					else
					{
						this.UpdateRecordToDatabase(postedModel);
					}

                    this.ModelState.Clear();
                    this.ViewBag.Message = "Successfully saved";
					postedModel.IsEditable = this.IsEditable();
					return this.View(postedModel);
				}

				if (buttonAction == Buttons.New)
				{
					if (string.IsNullOrEmpty(postedModel.IconConfigurationId))
					{
						postedModel.IsEditable = this.IsEditable();

						this.ModelState.AddModelError("Error", "Icon ID is required.");
						return this.View(postedModel);
					}

					string errorMsg;
					bool duplicateIcon = this.DuplicateIcons(postedModel, out errorMsg);

					if (duplicateIcon)
					{
						postedModel.IsEditable = this.IsEditable();

						this.ModelState.AddModelError("Error", errorMsg);
						return this.View(postedModel);
					}

					if (postedModel.AssetTrackingIconConfigurationGuid == Guid.Empty)
					{
						this.InsertRecordToDatabase(postedModel);
					}
					else
					{
						this.UpdateRecordToDatabase(postedModel);
					}

					usePostedModel = false;
					this.ModelState.Clear();
					iconConfigModel = new AssetIconConfigurationModel { IsEditable = this.IsEditable() };

					return this.View(iconConfigModel);
				}

				if (buttonAction == Buttons.Cancel)
				{
					return this.View(postedModel);
				}

				// Once we have entity assignment, then the guid will be coming
				// from the summary page.
				iconConfigModel = this.GetIconConfiguration(Guid.Empty);
				iconConfigModel.IsEditable = this.IsEditable();

				return this.View(iconConfigModel);
			}
			catch (Exception ex)
			{
				this.ModelState.AddModelError("Error", "Error: " + ex.Message);

				if (usePostedModel)
				{
					return this.View(postedModel);
				}

				return this.View(iconConfigModel);
			}
		}

		/// <summary>
		/// This method will handle the window modal dialog event. It will redirect to the 
		/// icon configuration page.
		/// </summary>
		/// <param name="isIconPopup"></param>
		/// <returns></returns>
		[HttpGet]
		[RequireRouteValues(new[] { "isIconPopup" })]
		public ActionResult IconConfiguration(string isIconPopup)
		{
			if (string.IsNullOrEmpty(isIconPopup) == false && isIconPopup.Equals("True"))
			{
				return this.RedirectToAction("IconSelection", "AssetIconSelection");
			}

			// This should never happen.
			return this.View(new AssetIconConfigurationModel { IsEditable = this.IsEditable() });
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will check to see if a duplication icon was selected.
		/// </summary>
		/// <param name="iconConfigModel">The icon configuration model</param>
		/// <param name="errMsg">Error message if a duplication is found.</param>
		/// <returns>Returns true if a duplication was found.</returns>
		private bool DuplicateIcons(AssetIconConfigurationModel iconConfigModel, out string errMsg)
		{
			errMsg = "Cannot have selected duplicate icons.";
			const string SelectIcon = "SelectIcon.png";

			if (iconConfigModel.EquipmentIconName != SelectIcon)
			{
				if (iconConfigModel.EquipmentIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.EquipmentVarianceIconName != SelectIcon)
			{
				if (iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentVarianceIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.EquipmentInvestigationIconName != SelectIcon)
			{
				if (iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentInvestigationIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.EquipmentCompleteInvestigationFailedIconName != SelectIcon)
			{
				if (iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.EquipmentCompleteInvestigationPassedIconName != SelectIcon)
			{
				if (iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.EquipmentCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.TankIconName != SelectIcon)
			{
				if (iconConfigModel.TankIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.TankIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.FacilityIconName != SelectIcon)
			{
				if (iconConfigModel.FacilityIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.FacilityIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.DeliveryLocationIconName != SelectIcon)
			{
				if (iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.DeliveryLocationIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.BreadcrumbIconName != SelectIcon)
			{
				if (iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.BreadcrumbIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.BreadcrumbVarianceIconName != SelectIcon)
			{
				if (iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.BreadcrumbVarianceIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.BreadcrumbInvestigationIconName != SelectIcon)
			{
				if (iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.BreadcrumbInvestigationIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName != SelectIcon)
			{
				if (iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName != SelectIcon)
			{
				if (iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.MapPinIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName))
				{
					return true;
				}
			}

			if (iconConfigModel.MapPinIconName != SelectIcon)
			{
				if (iconConfigModel.MapPinIconName.Equals(iconConfigModel.EquipmentIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.TankIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.FacilityIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.DeliveryLocationIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.BreadcrumbIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.EquipmentVarianceIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.EquipmentInvestigationIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationFailedIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.EquipmentCompleteInvestigationPassedIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.BreadcrumbInvestigationIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.BreadcrumbVarianceIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationFailedIconName)
					|| iconConfigModel.MapPinIconName.Equals(iconConfigModel.BreadcrumbCompleteInvestigationPassedIconName))
				{
					return true;
				}
			}

			errMsg = string.Empty;
			return false;
		}

		/// <summary>
		/// This method will return true if the user has modify rights.
		/// </summary>
		/// <returns>Return true if editable.</returns>
		private bool IsEditable()
		{
			return this.Security.HasRight(RIGHT.MODIFY_ICON_CONFIGURATION);
		}

		/// <summary>
		/// This method determine which button was pressed if any.
		/// </summary>
		/// <returns>Return the button pressed enumeration.</returns>
		private Buttons WhichButtonWasPressed()
		{
			string buttonPressed = this.Request.Params.AllKeys.FirstOrDefault(
								x => x.StartsWith("NewIconConfigBtn")
								|| x.StartsWith("OkIconConfigBtn")
								|| x.StartsWith("CancelIconConfigBtn"));

			if (string.IsNullOrEmpty(buttonPressed))
			{
				return Buttons.None;
			}

			if (buttonPressed.Equals("NewIconConfigBtn"))
			{
				return Buttons.New;
			}

			if (buttonPressed.Equals("OkIconConfigBtn"))
			{
				return Buttons.Ok;
			}

			if (buttonPressed.Equals("CancelIconConfigBtn"))
			{
				return Buttons.Cancel;
			}

			return Buttons.None;
		}
		/// <summary>
		/// This method will get the icon configuration data from the database based on the icon
		/// configuration GUID.
		/// </summary>
		/// <param name="iconConfigGuid">The GUID used to get the icon configuration data.</param>
		/// <returns>Returns a populated model.</returns>
		private AssetIconConfigurationModel GetIconConfiguration(Guid iconConfigGuid)
		{
			const string IconPathKey = "GeoTrackingMapIconPath";
			AssetTrackingIconConfigurationClass iconConfiguration = null;

			// Once we have Icon Configuration as entity assignable, then this
			// IF statement can be removed.
			if (iconConfigGuid == Guid.Empty)
			{
				var iconConfigurationList = 
							FMChannelHelper.MakeCall<IAssetTrackingIconConfigurations, List<AssetTrackingIconConfigurationClass>>(
																											x => x.Enumerate(this.Security));

				if (iconConfigurationList != null && iconConfigurationList.Count > 0)
				{
					iconConfiguration = iconConfigurationList[0];
				}
			}
			else
			{
				iconConfiguration =
							FMChannelHelper.MakeCall<IAssetTrackingIconConfigurations, AssetTrackingIconConfigurationClass>(
														x => x.Get(this.Security, iconConfigGuid));
			}

			if (iconConfiguration == null)
			{
				iconConfiguration = new AssetTrackingIconConfigurationClass();
			}

			var configSettingDo = FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(x => x.GetByKey(this.Security, IconPathKey));
			string iconPath = "~/Areas/images/AssetMapImages/MapIcons/";

			if (configSettingDo != null && string.IsNullOrEmpty(configSettingDo.SettingValue) == false)
			{
				const string Slash = "/";
				iconPath = configSettingDo.SettingValue;
				string lastChar = iconPath.Substring(iconPath.Length - 1, 1);

				if (lastChar.Equals(Slash) == false)
				{
					iconPath = iconPath + Slash;
				}
			}

			string equipmentIconName				= string.IsNullOrEmpty(iconConfiguration.EquipmentIconName) ? "SelectIcon.png" : iconConfiguration.EquipmentIconName;
			string equipmentVarianceIconName		= string.IsNullOrEmpty(iconConfiguration.EquipmentVarianceIconName) ? "SelectIcon.png" : iconConfiguration.EquipmentVarianceIconName;
			string equipmentInvestigationIconName	= string.IsNullOrEmpty(iconConfiguration.EquipmentInvestigationIconName) ? "SelectIcon.png" : iconConfiguration.EquipmentInvestigationIconName;
			string tankIconName						= string.IsNullOrEmpty(iconConfiguration.TankIconName) ? "SelectIcon.png" : iconConfiguration.TankIconName;
			string facilityIconName					= string.IsNullOrEmpty(iconConfiguration.FacilityIconName) ? "SelectIcon.png" : iconConfiguration.FacilityIconName;
			string deliveryLocationIconName			= string.IsNullOrEmpty(iconConfiguration.DeliveryLocationIconName) ? "SelectIcon.png" : iconConfiguration.DeliveryLocationIconName;
			string breadcrumbIconName				= string.IsNullOrEmpty(iconConfiguration.BreadcrumbIconName) ? "SelectIcon.png" : iconConfiguration.BreadcrumbIconName;
			string breadcrumbVarianceIconName		= string.IsNullOrEmpty(iconConfiguration.BreadcrumbVarianceIconName) ? "SelectIcon.png" : iconConfiguration.BreadcrumbVarianceIconName;
			string breadcrumbInvestigationIconName	= string.IsNullOrEmpty(iconConfiguration.BreadcrumbInvestigationIconName) ? "SelectIcon.png" : iconConfiguration.BreadcrumbInvestigationIconName;
			string mapPinIconName					= string.IsNullOrEmpty(iconConfiguration.MapPinIconName) ? "SelectIcon.png" : iconConfiguration.MapPinIconName;

			string equipmentCompleteInvestigationFailedIconName = string.IsNullOrEmpty(iconConfiguration.EquipmentCompleteInvestigationFailedIconName) ? "SelectIcon.png" : iconConfiguration.EquipmentCompleteInvestigationFailedIconName;
			string equipmentCompleteInvestigationPassedIconName = string.IsNullOrEmpty(iconConfiguration.EquipmentCompleteInvestigationPassedIconName) ? "SelectIcon.png" : iconConfiguration.EquipmentCompleteInvestigationPassedIconName;
			string breadcrumbCompleteInvestigationFailedIconName = string.IsNullOrEmpty(iconConfiguration.BreadcrumbCompleteInvestigationFailedIconName) ? "SelectIcon.png" : iconConfiguration.BreadcrumbCompleteInvestigationFailedIconName;
			string breadcrumbCompleteInvestigationPassedIconName = string.IsNullOrEmpty(iconConfiguration.BreadcrumbCompleteInvestigationPassedIconName) ? "SelectIcon.png" : iconConfiguration.BreadcrumbCompleteInvestigationPassedIconName;

			var iconConfigModel = new AssetIconConfigurationModel
			{
				AssetTrackingIconConfigurationGuid				= iconConfiguration.AssetTrackingIconConfigurationGuid,
				IconConfigurationId								= iconConfiguration.IconConfigurationId,
				EquipmentIconName								= equipmentIconName,
				EquipmentVarianceIconName						= equipmentVarianceIconName,
				EquipmentInvestigationIconName					= equipmentInvestigationIconName,
				EquipmentCompleteInvestigationFailedIconName	= equipmentCompleteInvestigationFailedIconName,
				EquipmentCompleteInvestigationPassedIconName	= equipmentCompleteInvestigationPassedIconName,
				TankIconName									= tankIconName,
				FacilityIconName								= facilityIconName,
				DeliveryLocationIconName						= deliveryLocationIconName,
				BreadcrumbIconName								= breadcrumbIconName,
				BreadcrumbVarianceIconName						= breadcrumbVarianceIconName,
				BreadcrumbInvestigationIconName					= breadcrumbInvestigationIconName,
				BreadcrumbCompleteInvestigationFailedIconName	= breadcrumbCompleteInvestigationFailedIconName,
				BreadcrumbCompleteInvestigationPassedIconName	= breadcrumbCompleteInvestigationPassedIconName,
				MapPinIconName									= mapPinIconName,
				IconPath										= iconPath,
				SiteGuid										= iconConfiguration.SiteGuid
			};

			return iconConfigModel;
		}

		/// <summary>
		/// This method will create an asset Icon configuration record.
		/// </summary>
		/// <param name="postedModel">The data to save.</param>
		private void InsertRecordToDatabase(AssetIconConfigurationModel postedModel)
		{
			if (postedModel == null)
			{
				throw new ArgumentException("Model is null.");
			}

			if (string.IsNullOrEmpty(postedModel.IconConfigurationId))
			{
				throw new Exception("Icon ID is required.");
			}

			var iconConfig = new AssetTrackingIconConfigurationClass()
			{
				IconConfigurationId								= postedModel.IconConfigurationId.Trim(),
				EquipmentIconName								= postedModel.EquipmentIconName,
				EquipmentVarianceIconName						= postedModel.EquipmentVarianceIconName,
				EquipmentInvestigationIconName					= postedModel.EquipmentInvestigationIconName,
				EquipmentCompleteInvestigationFailedIconName	= postedModel.EquipmentCompleteInvestigationFailedIconName,
				EquipmentCompleteInvestigationPassedIconName	= postedModel.EquipmentCompleteInvestigationPassedIconName,
				FacilityIconName								= postedModel.FacilityIconName,
				DeliveryLocationIconName						= postedModel.DeliveryLocationIconName,
				BreadcrumbIconName								= postedModel.BreadcrumbIconName,
				BreadcrumbVarianceIconName						= postedModel.BreadcrumbVarianceIconName,
				BreadcrumbInvestigationIconName					= postedModel.BreadcrumbInvestigationIconName,
				BreadcrumbCompleteInvestigationFailedIconName	= postedModel.BreadcrumbCompleteInvestigationFailedIconName,
				BreadcrumbCompleteInvestigationPassedIconName	= postedModel.BreadcrumbCompleteInvestigationPassedIconName,
				TankIconName									= postedModel.TankIconName,
				MapPinIconName									= postedModel.MapPinIconName,
				SiteGuid										= postedModel.SiteGuid
			};

			postedModel.AssetTrackingIconConfigurationGuid =
					FMChannelHelper.MakeCall<IAssetTrackingIconConfigurations, Guid>(x => x.Add(this.Security, iconConfig));
		}

		/// <summary>
		/// This method will update an asset tracking icon configuration record.
		/// </summary>
		/// <param name="postedModel">The data to save.</param>
		private void UpdateRecordToDatabase(AssetIconConfigurationModel postedModel)
		{
			if (postedModel == null)
			{
				throw new ArgumentException("Model is null.");
			}

			if (string.IsNullOrEmpty(postedModel.IconConfigurationId))
			{
				throw new Exception("Icon ID is required.");
			}

			var iconConfig = new AssetTrackingIconConfigurationClass()
			{
				AssetTrackingIconConfigurationGuid				= postedModel.AssetTrackingIconConfigurationGuid,
				IconConfigurationId								= postedModel.IconConfigurationId.Trim(),
				EquipmentIconName								= postedModel.EquipmentIconName,
				EquipmentVarianceIconName						= postedModel.EquipmentVarianceIconName,
				EquipmentInvestigationIconName					= postedModel.EquipmentInvestigationIconName,
				EquipmentCompleteInvestigationFailedIconName	= postedModel.EquipmentCompleteInvestigationFailedIconName,
				EquipmentCompleteInvestigationPassedIconName	= postedModel.EquipmentCompleteInvestigationPassedIconName,
				FacilityIconName								= postedModel.FacilityIconName,
				DeliveryLocationIconName						= postedModel.DeliveryLocationIconName,
				BreadcrumbIconName								= postedModel.BreadcrumbIconName,
				BreadcrumbVarianceIconName						= postedModel.BreadcrumbVarianceIconName,
				BreadcrumbInvestigationIconName					= postedModel.BreadcrumbInvestigationIconName,
				BreadcrumbCompleteInvestigationFailedIconName	= postedModel.BreadcrumbCompleteInvestigationFailedIconName,
				BreadcrumbCompleteInvestigationPassedIconName	= postedModel.BreadcrumbCompleteInvestigationPassedIconName,
				TankIconName									= postedModel.TankIconName,
				MapPinIconName									= postedModel.MapPinIconName
			};

			FMChannelHelper.MakeCall<IAssetTrackingIconConfigurations>(x => x.Modify(this.Security, iconConfig));
		}
		#endregion
	}
}