///***************************************************************************
/// Module Name:  ConfigStringArray.cs
/// Author:       Bryan Ponnwitz
/// Copyright (c) Varec, Inc. 2016 All rights reserved.
///***************************************************************************

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FMExportService.Config
{
	[Serializable()]
	[DebuggerStepThrough()]
	[DesignerCategory("code")]
	public sealed class ConfigStringArray {
		
		public ConfigArrayOfString ArrayOfString {
			get;
			set;
		}

	}
}
