///***************************************************************************
/// Module Name:  ConfigStringArraySetting.cs
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
	[XmlRoot("setting", IsNullable = false)]
	public sealed class ConfigStringArraySetting {

		[XmlAttribute("name")]
		public string Name {
			get;
			set;
		}

		[XmlAttribute("serializeAs")]
		public string SerializeAs {
			get {
				return "Xml";
			}
			set {
				// Do Nothing
			}
		}

		[XmlElement("value")]
		public ConfigStringArray Value {
			get;
			set;
		}

	}
}
