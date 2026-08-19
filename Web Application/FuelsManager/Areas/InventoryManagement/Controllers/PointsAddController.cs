namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.Linq;

	using Areas.Controllers;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using ViewModels;
   using FuelsManager.FMWebApp;
   using System.Web.UI;

   public class PointsAddController : FMBaseControllerEx
	{
		private enum Buttons { None, Create, Cancel };

		#region Action methods
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PointsAddView(PointsDetailModel model)
		{
			Buttons buttonPressed = this.GetButtonPressed();

			if (buttonPressed == Buttons.Cancel)
			{
				return this.RedirectToAction("PointsSummaryView", "PointsSummary");
			}

			if (buttonPressed == Buttons.Create)
			{
				ViewResult vr = null;

            try
				{
               vr = View(model);					
					
					if (string.IsNullOrEmpty(model.ID))
					{
						throw new Exception("ID cannot be blank.");
					}


					if (model.Results.ErrorMessage.Values.Count > 0)
					{
						string errMessage = "Input validation error in PointsAddView.";
						foreach (var errKeys in model.Results.ErrorMessage.Keys)
						{
							foreach (var errMsg in model.Results.ErrorMessage[errKeys])
							{
                        errMessage += string.Format("{0}{1} {2}", Environment.NewLine , errKeys, errMsg);

							}
						}
						FMFormBase.LogErrorMessage(errMessage);
                  AddTemplates(model);
                  return vr;
					}

					FMChannelHelper.MakeCall<IPoints>(
						x => x.CreatePoints(this.Security, model.ID, model.NumberToCreate, model.TemplateSelection));
				}
				catch (Exception except)
				{
					this.OnError(except);
               AddTemplates(model);
               return vr;
            }
         }

			return this.RedirectToAction("PointsSummaryView", "PointsSummary");
        }

		[HttpGet]
		public ActionResult PointsAddView()
		{
			var model = new PointsDetailModel();

			AddTemplates(model);

			return this.View(model);
		}
		#endregion

		#region Private methods
		public void AddTemplates(PointsDetailModel model)
		{
         try
         {
            bool isMovementKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMovementKey());

            if (isMovementKey)
            {
               model.Templates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(
                           x => x.EnumerateByType(this.Security, null));
            }
            else
            {
               // If the movement key is not enabled, then remove the movement templates from the list.
               model.Templates = new PointTemplateCollection();

               var pointTemplates = FMChannelHelper.MakeCall<IPointTemplates, PointTemplateCollection>(
                           x => x.EnumerateByType(this.Security, null));

               foreach (PointTemplate pointTemplate in pointTemplates)
               {
                  if (pointTemplate.ID.ToUpper() == "STANDARD MOVEMENT" || pointTemplate.ID.ToUpper() == "STANDARD MOVEMENT NODE VOL")
                  {
                     continue;
                  }

                  model.Templates.Add(pointTemplate);
               }
            }
         }
         catch (Exception except)
         {
            this.OnError(except);
         }
      }

      /// <summary>
      /// This method determine which button was pressed if any.
      /// </summary>
      /// <returns>Return the button pressed enumeration.</returns>
      private Buttons GetButtonPressed()
		{
			const string CancelButton = "cancelButton";
			const string CreateButton = "createButton";

			string cancelButtonPressed = this.Request.Params.AllKeys.FirstOrDefault(x => x.StartsWith(CancelButton));
			string createButtonPressed = this.Request.Params.AllKeys.FirstOrDefault(x => x.StartsWith(CreateButton));

			if (string.IsNullOrEmpty(createButtonPressed) == false)
			{
				return Buttons.Create;
			}

			if (string.IsNullOrEmpty(cancelButtonPressed) == false)
			{
				return Buttons.Cancel;
			}

			return Buttons.None;
		}
		#endregion
	}
}