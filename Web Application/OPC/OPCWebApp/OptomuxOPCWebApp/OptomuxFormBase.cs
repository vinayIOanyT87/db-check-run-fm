/******************************************************************************
	FILE NAME:		OptomuxFormBase.cs

	PURPOSE:			OptomuxFormBase

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

using System;
using System.Web.UI.WebControls;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FuelsManager.FMWebApp;

namespace OPCWebApp.OptomuxOPCWebApp
{
	/// <summary>
	/// Summary description for OptomuxFormBase.
	/// </summary>
	public class OsdpFormBase : FMAutoSubmitFormBase
	{
		protected void UpdateDeleteButton( LinkButton deleteButton )
		{
			if(deleteButton != null)
			{
				deleteButton.Enabled = this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
			}

		}

		protected string GetDictionaryText( string key )
		{
			string altText = key;

			if(this.Session["UseDataDictionary"] == null || (bool) this.Session["UseDataDictionary"])
			{
			    altText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.Security.SiteGuid, key));
			}
			else
			{
				// If the data dictionary is not available or is turned off, we need to strip off the
				// compartmental text at the beginning of the key value
				int index = key.IndexOf( "|", StringComparison.Ordinal );

				if ( index >= 0 )
				{
					altText = altText.Substring( index+1 );
				}

			}

			return altText;

		}

	}

}
