namespace FMBusinessObjects.Constants
{
   using System;

   public static class SystemDataPointVirtualPoint
   {
		// Virtual Point well known Guids for System Data Point
		public static readonly Guid PointTemplateTypeGuid = new Guid("{2ddeb3e0-545c-444b-b1bf-9cab048f21b7}");

		public static readonly Guid PointTemplateGuid	= new Guid("{19070B53-0D5B-4327-80B6-997A7F2DACB0}");
		public static readonly Guid PointGuid				= new Guid("{DE80C5F1-A0F7-4292-92F2-B68FDC6A0425}");
		public static readonly Guid TagSiteDataGuid		= new Guid("{C3BC3BA2-026D-468E-A833-4031E70B0E60}");
		public static readonly Guid TagUserDataGuid		= new Guid("{DB8A725D-8C31-45B1-BFC9-439BEC87B30C}");
		public static readonly Guid TagDateTimeDataGuid = new Guid("{089EDE41-6B8E-4E3B-B498-22DA5B79F2F2}");
        public static readonly Guid TaglicenseExpiryDataGuid = new Guid("{54838E9B-55E8-483A-AD59-DF27691BB97A}");

        public static readonly string PointTemplateId	= "System Data Point - Point Template";
		public static readonly string PointId			= "System Data Point - Point";
		public static readonly string TagSiteDataId		= "Site Info";
		public static readonly string TagUserDataId		= "User Info";
		public static readonly string TagDateTimeDataId = "Date/Time Info";
        public static readonly string TagLicenseExpiryDataId = "License Expiry Info";

    }
}
