namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security;
	using System.ServiceModel;
    using System.Security.Cryptography;

	using Crypt;
	using Crypt.Interfaces;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;

	using FMBusinessServices.DataAccessLayer;

	[SecuritySafeCritical]
	[ServiceBehavior( TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted )]
	public class Pictures : FMServiceBase, IPictures
	{
		private readonly ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public Guid Add( SecurityClass security, Picture picture)
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

            if (picture == null)
            {
                throw new ArgumentNullException("picture");
            }

		    
            //Check for Image Existence by Hash
            if (!String.IsNullOrEmpty(picture.ImageHash))
		    {
		        Guid pictureGuid = this.GetPictureGuidByImageHash(security, picture.ImageHash);
		        if (pictureGuid != Guid.Empty)
		        {
		            return pictureGuid;
		        }
		    }
		    else
		    {
               //No Picture Data should be stored with an empty or null ImageHash.
               //Picture object automatically sets the Hash when the ImageStream property is set.
               throw new Exception("Empty Picture.ImageHash is not allowed.");
		    }

			// TODO: Check security rights

			picture.SetCreationStamp( security );

			using ( var cmd = new SqlCommand() )
			{
				picture.SetCreationStamp( security );
				picture.AutoGenerateInsertProcSQL( cmd, "gsp_PicturesInsertByPK" );
				cmd.Parameters["@PictureGuid"].Direction = ParameterDirection.InputOutput;
				this.consolidatedDA.ExecuteQuery( security, cmd );
				picture.IdentityGuid = new Guid( cmd.Parameters["@PictureGuid"].Value.ToString() );
			}
		    return picture.IdentityGuid;
		}

		[OperationBehavior( TransactionScopeRequired = true, TransactionAutoComplete = true )]
		public void Purge( SecurityClass security, Guid pictureGuid )
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

            if (pictureGuid == null)
            {
                throw new ArgumentNullException("pictureGuid");
            }

            // TODO: Check security rights

            var drawing = this.Get( security, pictureGuid );
			if ( drawing.IdentityGuid == Guid.Empty )
			{
				throw new Exception( "Picture not found." );
			}

			// Delete point
			using ( var cmd = new SqlCommand() )
			{
				cmd.CommandText = "gsp_PicturesDeleteByRowGuid";
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@PictureGuid", pictureGuid );
				this.consolidatedDA.ExecuteQuery( security, cmd );
			}
		}

		public Picture Get( SecurityClass security, Guid pictureGuid )
		{
			if ( security == null )
			{
				//throw new ArgumentNullException( "security" );
			}

            if (pictureGuid == null)
            {
                throw new ArgumentNullException("pictureGuid");
            }

            // TODO: Check security rights

            DataSet set = null;
			var picture = new Picture() { PictureGuid = pictureGuid };

			using ( var cmd = new SqlCommand() )
			{
				picture.SelectSQL( cmd, pictureGuid );
				set = consolidatedDA.GetDataSet( cmd, security );
			}

			DataTable table = set.Tables[0];
			picture = new Picture();
			if ( table.Rows.Count > 0 )
			{
			    picture.IgnoreHashCalculation = true;
				picture.AutoLoad( table.Rows[0] );
			    picture.IgnoreHashCalculation = false;
			}

			return picture;
		}

        public Guid GetPictureGuidByImageHash(SecurityClass security, string imageHash)
        {
            if (security == null)
            {
                //throw new ArgumentNullException( "security" );
            }
            if (imageHash == null)
            {
                throw new ArgumentNullException("imageHash");
            }

            // TODO: Check security rights

            DataSet set = null;
            
            using (var cmd = new SqlCommand())
            {
                Picture.SelectPictureGuidByImageHashAndSiteGuidSQL(cmd, imageHash, security.SiteGuid);
                set = consolidatedDA.GetDataSet(cmd, security);
            }

            DataTable table = set.Tables[0];
            Guid returnValue = Guid.Empty;
            if (table.Rows.Count > 0)
            {
                if (table.Rows[0]["PictureGuid"] != null)
                {
                    returnValue = (Guid)table.Rows[0]["PictureGuid"];
                }
            }

            return returnValue;
        }

        public PictureCollection Enumerate(SecurityClass security)
		{
			if ( security == null )
			{
				throw new ArgumentNullException( "security" );
			}

			// TODO: Check security rights

			DataSet set = null;

			using ( var cmd = new SqlCommand() )
			{
				Picture.EnumerateSQL( cmd, security );
				set = consolidatedDA.GetDataSet( cmd, security );
			}

			DataTable table = set.Tables[0];
			var pictures = new PictureCollection();

			foreach ( DataRow row in table.Rows )
			{
				var picture = new Picture();
                picture.IgnoreHashCalculation = true;
                picture.AutoLoad(row);
                picture.IgnoreHashCalculation = false;
                pictures.Add( picture );
			}

			return pictures;
		}
	}
}
