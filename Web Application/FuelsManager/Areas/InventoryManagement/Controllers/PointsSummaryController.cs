namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.ServiceModel;
	using System.Web.Mvc;

	using Areas.Controllers;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using ViewModels;

	public class PointsSummaryController : FMBaseControllerEx
	{
		#region Private members
		private enum Buttons { None, Add };
		#endregion

		#region Action methods
		/// <summary>
		/// This method will handle the form post.
		/// </summary>
		/// <param name="postedModel">The posted model.</param>
		/// <returns></returns>
		public ActionResult PointsSummaryView(PointsSummaryModel postedModel)
		{
			Buttons buttonPressed = this.GetButtonPressed();

			if (buttonPressed == Buttons.Add)
			{
				// Redirect to the Add Points page.
				return this.RedirectToAction("PointsAddView", "PointsAdd");
			}

			PointsSummaryModel newModel = this.GetPointsSummaryModel();
			return this.View(newModel);
		}

		/// <summary>
		/// This method will handle the delete action.
		/// </summary>
		/// <param name="id">The ID of the item to delete.</param>
		/// <returns>Returns the view.</returns>
		public ActionResult Delete(string id)
		{
            try
            {
                var pointTemplateGuid = new Guid(id);
                var fceeMappingList = FMChannelHelper.MakeCall<IFCEEServiceManager, Dictionary<Guid, FCEEMapping>>(x => x.EnumerateByPointGuid(this.Security, pointTemplateGuid)).Values.ToList();
                bool mappingExists = fceeMappingList.Count > 0;
				//this.AddSuccess("Success.");
                return this.JsonWithErrorMessages(mappingExists, JsonRequestBehavior.AllowGet);

            }
            catch (CommunicationException e)
            {
                this.ErrorHandler(e);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);

            }
            catch (Exception except)
            {
                this.OnError(except);
                return this.JsonWithErrorMessages(null, JsonRequestBehavior.AllowGet);
            }
 		}
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                var pointTemplateGuid = new Guid(id);
                FMChannelHelper.MakeCall<IPoints>(x => x.Purge(this.Security, pointTemplateGuid));
                return this.RedirectToAction("PointsSummaryView", "PointsSummary");
            }
            catch (CommunicationException e)
            {
                this.ErrorHandler(e);
                PointsSummaryModel newModel = this.GetPointsSummaryModel();
                return this.View("PointsSummaryView", newModel);

            }
            catch (Exception except)
            {
                this.OnError(except);
            }

            return this.RedirectToAction("PointsSummaryView", "PointsSummary");

        }
     
        #endregion

        #region Private methods
        /// <summary>
        /// This method will get a collection of points for the summary view.
        /// </summary>
        /// <returns>Returns a collection of points within the summary model.</returns>
        private PointsSummaryModel GetPointsSummaryModel()
		{
			var model = new PointsSummaryModel();

			try
			{
				// Populate the model
				model.Points =
					FMChannelHelper.MakeCall<IPoints, List<Point>>(
								x => x.EnumerateForSummaryWithCategories(this.Security, this.Security.SiteGuid, includeDictionaries: false));
				model.DeleteEnabled = this.Security.HasRight(RIGHT.MODIFY_POINTS);

				List<string> types = model.Points.Select(x => x.PointType).Distinct().ToList();
				types.Sort();

				model.PointTypeslist = new List<KeyValuePair<string, string>>();
				model.PointTypeslist.Add(new KeyValuePair<string, string>(String.Empty, "{All}"));
				foreach (var type in types)
                {
					model.PointTypeslist.Add(new KeyValuePair<string, string>(type, type));
				}

				var categories = model.Points.SelectMany(x => x.PointCategoryCollection.Select(c=>c.ID)).Distinct().ToList();
				categories.Sort();
				model.PointCategoriesList = new List<KeyValuePair<string, string>>();
				model.PointCategoriesList.Add(new KeyValuePair<string, string>( String.Empty, "{All}"));
				foreach (var category in categories)
                {
					model.PointCategoriesList.Add(new KeyValuePair<string, string>(category, category));
				}
			}
			catch (Exception except)
			{
				this.OnError(except);
			}

			return model;
		}

		/// <summary>
		/// This method determine which button was pressed if any.
		/// </summary>
		/// <returns>Return the button pressed enumeration.</returns>
		private Buttons GetButtonPressed()
		{
			const string AddButtonTop		= "addButton1";
			const string AddBottomButton	= "addButton2";

			string addTopButtonPressed = this.Request.Params.AllKeys.FirstOrDefault(x => x.StartsWith(AddButtonTop));
			string addBottomButtonPressed = this.Request.Params.AllKeys.FirstOrDefault(x => x.StartsWith(AddBottomButton));

			if (string.IsNullOrEmpty(addTopButtonPressed) == false || string.IsNullOrEmpty(addBottomButtonPressed) == false)
			{
				return Buttons.Add;
			}

			return Buttons.None;
		}
		#endregion
	}
}