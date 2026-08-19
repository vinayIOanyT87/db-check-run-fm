namespace FuelsManager
{
	using System;
	using System.Web;
	using System.IO;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;

	/// <summary>
    /// Request handler to return pictures stored in FuelsManager database
    /// </summary>
	public class DisplayImage : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
	        try
	        {
		        SecurityClass security = null;

				context.Response.Clear();

				// Did the request include a picture guid?
				if ( context.Request.QueryString["PictureGuid"] != null )
				{
					var pictureGuid = new Guid( context.Request.QueryString["PictureGuid"] );

					var picture = FMChannelHelper.MakeCall<IPictures, Picture>( x => x.Get( security, pictureGuid ) );
                    context.Response.ContentType = picture.ContentType;
                    var headerValue = context.Request.Headers["If-Modified-Since"];

                    var pictureUpdate = new DateTime(picture.UpdatedDate.Year,
                        picture.UpdatedDate.Month,
                        picture.UpdatedDate.Day,
                        picture.UpdatedDate.Hour,
                        picture.UpdatedDate.Minute,
                        picture.UpdatedDate.Second);


                    if (headerValue != null)
                    {
                        var dt = new DateTime();
                        if (DateTime.TryParse(headerValue, out dt))
                        {
                            var modifiedSince = dt.ToLocalTime();
                            if (modifiedSince >= pictureUpdate)
                            {
                                context.Response.StatusCode = 304;
                                //context.Response.Status = "Image not modified";
                                return;
                            }
                        }
                        
                    }
                    
                    using (var memoryStream = new MemoryStream(picture.ImageStream, false))
				    {
				        using (var mainImageStream = System.Drawing.Image.FromStream(memoryStream))
				        {
				            //Attempt to Resize Image if parameters are specified.
				            int height = mainImageStream.Height;
				            int width = mainImageStream.Width;

				            if (context.Request.QueryString["Height"] != null)
				            {
				                int.TryParse(context.Request.QueryString["Height"], out height);
				            }
				            if (context.Request.QueryString["Width"] != null)
				            {
				                int.TryParse(context.Request.QueryString["Width"], out width);
				            }
                            using (var bmpImageStream = new System.Drawing.Bitmap(mainImageStream, new System.Drawing.Size(width, height)))
                            {
                                bmpImageStream.Save(context.Response.OutputStream, mainImageStream.RawFormat);
                            }
				           
				        }
				    }
				    // .. and set last modified in the date format specified in the HTTP rfc.
                    context.Response.AddHeader("Last-Modified", picture.UpdatedDate.ToUniversalTime().ToString("R"));
                }
	        }
	        catch (Exception except)
	        {
		        var logger = new Logger("FuelsManager");
				logger.Error(except.ToString());
	        }
        }

		public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
