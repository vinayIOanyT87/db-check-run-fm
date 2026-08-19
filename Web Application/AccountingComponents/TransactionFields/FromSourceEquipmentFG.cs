/*****************************************************************************
FromSourceEquipmentFG

Original Author: Van Thompson
Revisions: See source control comments

(C) Copyright 2008 by Varec, Inc.  All rights reserved.

Revision History
Date:		By:					Reason:

//*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionFields
{
	public class FromSourceEquipmentFG : SourceEquipmentFG
	{
		public FromSourceEquipmentFG(byte equipmentNumber) : base(equipmentNumber) { }
		public override string FieldID
		{
			get
			{
				return "FromSourceEquipment" + eqNumber;
			}
		}

		public override bool Required
		{
			get
			{
				return true;
			}
		}
	}
}
