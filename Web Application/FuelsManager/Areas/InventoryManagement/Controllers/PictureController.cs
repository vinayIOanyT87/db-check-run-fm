namespace FuelsManager.Areas.InventoryManagement.Controllers
{
	using System;
	using System.IO;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.Areas.Controllers;
	using FuelsManager.Areas.InventoryManagement.ViewModels;

	public class PictureController : FMBaseController
    {
      [HttpGet]
		public ActionResult PictureSummary(string id)
        {
	        var model = new PictureSummaryModel
	                    {
		                    DeleteEnabled = true,
		                    Pictures =
			                    FMChannelHelper.MakeCall<IPictures, PictureCollection>(
				                    x => x.Enumerate(this.Security))
	                    };
				model.ReadOnly = !this.Security.HasRight(RIGHT.MODIFY_PICTURE_SUMMARY);
	        this.Session[PictureSummaryModel.SessionKey] = model;

	        return this.View(model);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PictureSummary(PictureSummaryModel model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					model = this.Session[PictureSummaryModel.SessionKey] as PictureSummaryModel;
					return this.View(model);
				}

				// Read the file
				var picture = new Picture
				              {
					              ImageStream = new byte[model.File.InputStream.Length],
								  ID = Path.GetFileName(model.File.FileName),
								  Description = "Uploaded file",
                                  ContentType = model.File.ContentType
				              };

				model.File.InputStream.Read(picture.ImageStream, 0, picture.ImageStream.Length);

				// Save it in the database
				FMChannelHelper.MakeCall<IPictures>(x => x.Add(this.Security, picture));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return this.View(model);
			}

			return this.RedirectToAction("PictureSummary");
		}

		public ActionResult Delete( string id )
		{
			var model = this.Session[PictureSummaryModel.SessionKey] as PictureSummaryModel;

			try
			{
				var pictureGuid = new Guid( id );
				FMChannelHelper.MakeCall<IPictures>( x => x.Purge( this.Security, pictureGuid ) );
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
				return this.View( "PictureSummary", model );
			}

			return this.RedirectToAction( "PictureSummary" );
		}
    }
}
