
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Web.SessionState;


[assembly: TagPrefix ( "FMControls", "FMControls" )]

namespace FMControls
{
	public class FMDownLinkButton : FMLinkButton
	{
		public FMDownLinkButton ( )
		{
			CommandName			= "Down";
			ID					= "DownButton";
			CausesValidation	= false;
			ImageFile_Enabled	= "down.gif";
			ImageFile_Disabled	= "down_un.gif";
			alternateText		= "Move this item down";
			Border				= 0;
		}

	}

}
