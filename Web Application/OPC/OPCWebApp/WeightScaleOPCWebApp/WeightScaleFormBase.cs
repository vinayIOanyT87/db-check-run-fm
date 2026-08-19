/******************************************************************************
	FILE NAME:		WeightScaleFormBase.cs

	PURPOSE:			WeightScaleFormBase

	COMMENTS:

		Copyright (C) Leidos - Varec, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the expressed written consent of Leidos - Varec.

	AUTHOR(S):	Kendall

	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using System.Web.UI.WebControls;
using FuelsManager.FMWebApp;

namespace WeightScaleOPCWebApp
{
   /// <summary>
   /// Summary description for WeightScaleFormBase.
   /// </summary>
   public class WeightScaleFormBase : FMAutoSubmitFormBase
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

		protected string GetDictionaryText( string Key )
		{
			string altText = Key;

			if(Session["UseDataDictionary"] == null || (bool) Session["UseDataDictionary"])
			{
					 altText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(Security.SiteGuid, altText)
																);
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
