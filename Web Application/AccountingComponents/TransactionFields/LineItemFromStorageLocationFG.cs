/*****************************************************************************
LineItemFromStorageLocationFG

Original Author: Van Thompson
Revisions: See source control comments

(C) Copyright 2008 by Varec, Inc.  All rights reserved.

Revision History
Date:		By:					Reason:

//*****************************************************************************/
using FMBusinessObjects.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionFields
{
	public class LineItemFromStorageLocationFG : LineItemStorageLocationFG
	{
		public override string FieldID
		{
			get
			{
				return "LineItem FromStorageLocationID";
			}
		}

		public override bool Required
		{
			get
			{
				return base.Required;
			}
		}
	}
}
