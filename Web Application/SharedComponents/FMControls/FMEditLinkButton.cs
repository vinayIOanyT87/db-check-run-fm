/// <SUMMARY>
/// File name:	FMEditLinkButton.cs
/// Purpose:	
///				
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///					2000.  This file shall not be copied or reproduced in any form 
///					without the express written consent of Endress+Hauser.
///				
///	Author(s):	
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	-------------------------------------------
///		2009-06-24	I.Orndorff				- Added image files for Deleted_ImageFile_Enabled and
///													  Deleted_ImageFile_Disabled. This addresses 
///													  task 4128.
///													  
///     2009-10-21  C. Knight               - Override AlternateText to have text for undelete command - 
///                                             Bug 4622
/// </SUMMARY>

using System.Web.UI;


[assembly: TagPrefix("FMControls", "FMControls")]

namespace FMControls
{
	public class FMEditLinkButton : FMLinkButton
	{
		string alternateDeletedText;

		public FMEditLinkButton ( )
		{
			CommandName					= "Edit";
			ID							= "EditButton";
			CausesValidation			= false;
			ImageFile_Enabled			= "Edit.gif";
			ImageFile_Disabled			= "Edit_un.gif";
			Deleted_ImageFile_Enabled	= "Edit_deleted.gif";
			Deleted_ImageFile_Disabled	= "Edit_deleted_un.gif";
			ToolTip					= "Edit button";//this item";
			alternateText				= "Edit button";//this item";
			alternateDeletedText		= "Undelete button";//this item";
		}

		override public string AlternateText
		{
			get
			{
				if (ShowDeleted)
				{
					return this.alternateDeletedText;
				}
				else
				{
					return this.alternateText;
				}
			}
			set { this.alternateText = value; }
		}
	}

}
