///***************************************************************************
/// Module Name:  Configuration.cs
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
	[XmlRoot(ElementName = "configuration", Namespace = "", IsNullable = false)]
	public sealed class Configuration {
		
		[XmlElement("applicationSettings")]
		public ConfigAppSettings AppSettings {
			get;
			set;
		}

	}

}
