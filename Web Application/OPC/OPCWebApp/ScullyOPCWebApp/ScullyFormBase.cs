/******************************************************************************
	FILE NAME:		ScullyFormBase.cs

	PURPOSE:			ScullyFormBase

	COMMENTS:

		Copyright (C) SAIC - Varec, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of SAIC - Varec.

	AUTHOR(S):	S. Jiang

	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using System.Web.UI.WebControls;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

using FuelsManager.FMWebApp;

namespace OPCWebApp.ScullyOPCWebApp
{
   /// <summary>
   /// Summary description for ScullyFormBase.
   /// </summary>
   public class ScullyFormBase : FMAutoSubmitFormBase
	{
		protected void UpdateDeleteButton( LinkButton DeleteButton )
		{
			if(DeleteButton != null)
			{
				DeleteButton.Enabled = Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
			}

		}

		protected virtual void UpdateView()
		{
		}

		protected string GetDictionaryText(string Key)
		{
			string altText = Key;

            if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
            {
                altText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.Security.SiteGuid, Key));
            }
            else
			{
				// If the data dictionary is not available or is turned off, we need to strip off the
				// compartmental text at the beginning of the key value
				int Index = Key.IndexOf( "|" );

				if ( Index >= 0 )
				{
					altText = altText.Substring( Index+1 );
				}

			}

			return altText;

		}

	}

}
