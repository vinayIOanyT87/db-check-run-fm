namespace FMBusinessObjects.DataObjects
{
	using System;
    using System.Data;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
    using System.Runtime.ExceptionServices;

	using FMBusinessObjects.Attributes;
    using Crypt;
	[Serializable]
	public class PictureCollection : List<Picture>
	{
	}

	[Serializable]
	[DataContract]
	public class Picture : BaseDataObject
	{
	    private byte[] imageStream;
	    private string imageHash;

	    public Picture()
	    {
	        this.Description = "";
	        this.ContentType= "";
	        this.imageHash = "";
	        this.IsSystemImage = false;
	        this.imageStream = null;
	        this.IgnoreHashCalculation = false;
	    }

        public bool IgnoreHashCalculation { get; set; }
        /// <summary>
        /// Public property used by other calling entities to get and set the image stream.  The imageHash attribute is 
        /// automatically set to ensure that the image stream byte array and it's corresponding hash value remain in sync
        /// </summary>
        [DataMember]
        [FMPersistedField]
        public byte[] ImageStream {
		    get
		    {
		        return this.imageStream;
		    } 
		    set
		    {
		        if (value == null || value.Length == 0)
		        {
		            this.imageStream = null;
		            this.imageHash = "";
		            return;
		        }
		        this.imageStream = value;
                //Compute the SHA1 hash value against the base64 representation of the imageStream property.
                //base64 representation is used because the HTML image control used in the client computes the exact 
                //hash using the sha1.js library but the hash is against the base64 representation of the image.
		        if (!this.IgnoreHashCalculation)
		        {
		            SHAChecksum checkSum = new SHAChecksum();
		            this.imageHash = checkSum.HashString(Convert.ToBase64String(value));
		        }
		    }
		}

		[DataMember]
		[FMPersistedField]
		public string Description { get; set; }


        [DataMember]
        [FMPersistedField]
        public string ContentType { get; set; }

        [DataMember]
        [FMPersistedField]
        public bool IsSystemImage { get; set; }

        /// <summary>
        /// SHA1 Hash of the ImageStream byte array
        /// </summary>
        [DataMember]
        [FMPersistedField]
        public string ImageHash
	    {
	        get
	        {
	           return this.imageHash;
	        }
            private set
            {
                this.imageHash = value;
            }
	    }

	    [FMPersistedField]
		public Guid PictureGuid
		{
			get
			{
				return this.IdentityGuid;
			}
			set
			{
				this.IdentityGuid = value;
			}
		}

		public void SelectSQL( SqlCommand cmd, Guid pictureGuid )
		{
			cmd.CommandText = "SELECT * FROM tblPictures WHERE PictureGuid = @PictureGuid";
			cmd.Parameters.AddWithValue( "@PictureGuid", pictureGuid );
		}

		static public void EnumerateSQL( SqlCommand cmd, SecurityClass security )
		{
			// we don't have entity assignment yet so we want to retrieve any image loaded at the site or enterprise (System Images)
			cmd.CommandText = "SELECT PictureGuid,ID,IsSystemImage,ImageHash,Description FROM tblPictures WHERE SiteGuid IN ( @SiteGuid, '00000000-0000-0000-0000-000000000001' ) ORDER BY [ID]";
			cmd.Parameters.AddWithValue( "@SiteGuid", security.SiteGuid );
		}

		static public void SelectPictureGuidByImageHashAndSiteGuidSQL(SqlCommand cmd, string imageHash, Guid siteGuid)
		{
			cmd.CommandText = "SELECT PictureGuid FROM tblPictures WHERE ImageHash = @ImageHash AND SiteGuid = @SiteGuid";
			cmd.Parameters.AddWithValue("@ImageHash", imageHash);
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}
	}
}
