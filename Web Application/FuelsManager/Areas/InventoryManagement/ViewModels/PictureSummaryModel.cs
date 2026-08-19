namespace FuelsManager.Areas.InventoryManagement.ViewModels
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.Web;
	using System.Xml.Serialization;
	using FMBusinessObjects.DataObjects;

	[Serializable]
	public class PictureSummaryModel
	{
		public static string SessionKey = "PictureSummaryModelKey";

		public bool ReadOnly;

		[NonSerialized]
		private HttpPostedFileBase file;

		[Required]
		[XmlIgnore]
		public HttpPostedFileBase File
		{
			get
			{
				return this.file;
			}
			set
			{
				this.file = value;
			}
		}

		public PictureCollection Pictures { get; set; }

		public bool DeleteEnabled { get; set; }
	}

	[Serializable]
	public class PointDetailPictures
	{
		public Guid PictureGuid { get; set; }
		public string ID { get; set; }
		public byte[] ImageStream { get; set; }
		public string Description { get; set; }
		public string ContentType { get; set; }
		public bool IsSystemImage { get; set; }
		public string ImageHash { get; set; }
	}
}
