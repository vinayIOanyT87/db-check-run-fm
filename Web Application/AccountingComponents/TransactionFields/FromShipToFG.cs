//*****************************************************************************************************************
//  FILE NAME:		FromShipToFG.cs
//	PURPOSE:		This class inherits from the ShipToFG class. It is used during
//					consumer transfers.
//
//	COMMENTS:
//		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Endress+Hauser.
//
//	AUTHOR(S):	Richard Panachida
//	VERSION:	1.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:		By:					Reason:
//		----------	-----------------	-------------------------------------------
//		2006-11-02	Richard Panachida	Corrected the defect for the missing ToShipTo (CSI 3575).
//*****************************************************************************************************************
namespace TransactionFields
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;

	/// <summary>
	/// Summary description for FromShipToFG.
	/// </summary>
	public class FromShipToFG : ShipToFG
	{
		public FromShipToFG()
		{
			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
			{
				companySubRole = ADF_SUBROLE;
			}
		}

		public override string FieldID
		{
			get
			{
				return "FromShipToID";
			}
		}
	}
}
