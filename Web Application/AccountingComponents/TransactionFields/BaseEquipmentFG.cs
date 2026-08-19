/// <summary>
/// File name:	BaseEquipmentFG.cs
/// Purpose:	The purpose of this module is to define the base equipment class. This class
///				is an abstract class defining what should be implemented by the derived classes.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Thomas Beckum
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	--------------------------------------------
///		2007-02-23		Richard Panachida		The SetEquipment method would not save the equipement
///												value if it was typed into the field manually and the
///												equipment did not exist in the database (CSI 2804).
///      2009-08-14     A. Coker             WI5265 - Replacing references to Equipment type enum by 
///                                          Equipment Type class.
/// </summary>

namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	public abstract class BaseEquipmentFG : EquipmentTextButtonGenerator
	{
		internal BaseEquipmentFG(bool destination, byte eqNumber)
			: base(destination, eqNumber)
		{
		}

		protected override void SpecializeControl(WebControl control)
		{
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				updatePanel.ID = updatePanel.ID + this.eqNumber;
			}
		}

		protected string GetDataText ( EquipmentDO equipmentDO )
		{
			return equipmentDO.RegistrationID;
		}
	}
}
