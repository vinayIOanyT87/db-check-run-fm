using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ReportSvr2005;
using FMCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuelsManager.Areas.FCEE.ViewModels
{
	[Serializable]
	public class FCEDeviceEditorModel
	{
		public FCEDevice fceDevice { get; set; }

		[Required]
		[RegularExpression("[0-9]{15,15}", ErrorMessage = "ImeiNumber must be a 15 digit number.")]
		[MinLength(15, ErrorMessage = "ImeiNumber must be a 15 digit number.")]
		[MaxLength(15, ErrorMessage = "ImeiNumber must be a 15 digit number.")]
		public string ImeiNumber
		{
			get { return this.fceDevice.ImeiNumber; }
			set { this.fceDevice.ImeiNumber = value.Trim(); }
		}
		[StringLength(30, ErrorMessage = "Friendly Name length can't be more than 30.")]
		[RegularExpression("[0-9a-zA-Z_\\-\\s']{0,30}", ErrorMessage = "Allowed characters for Friendly Name are alphanumeric values, space, -, _, and apostrophe.")]
		public string FriendlyName
		{
			get { return this.fceDevice.FriendlyName; }
			set { this.fceDevice.FriendlyName = value.Trim(); }
		}

		[Range(0.0, 1440.0)]
		public int MinTime
		{
			get { return this.fceDevice?.MinTime ?? 0; }
			set { this.fceDevice.MinTime = value; }
		}

		[Range(0.0, 1440.0)]
		public int MaxTime
		{
			get { return this.fceDevice?.MaxTime ?? 0; }
			set { this.fceDevice.MaxTime = value; }
		}

		[Range(-999999.0, 999999.0)]
		public double LevelDeadband
		{
			get { return this.fceDevice?.LevelDeadband ?? 0; }
			set { this.fceDevice.LevelDeadband = value; }
		}

		[Range(-999999.0, 999999.0)]
		public double TempDeadband
		{
			get { return this.fceDevice?.TempDeadband ?? 0; }
			set { this.fceDevice.TempDeadband = value; }
		}

		[Range(0.0, 60.0)]
		public int Heartbeat
		{
			get { return this.fceDevice?.Heartbeat ?? 0; }
			set { this.fceDevice.Heartbeat = value; }
		}
		[Range(1.0, 16.0)]
		public short TLStanks
		{
			get { return this.fceDevice?.TLStanks ?? 1; }
			set { this.fceDevice.TLStanks = value; }
		}
		[Range(0.0, 16.0)]
		public short ModbusMap
		{
			get { return this.fceDevice?.ModbusMap ?? 0; }
			set { this.fceDevice.ModbusMap = value; }
		}
		[Range(0.0, 1439.0)]
		public int MidnightOffset
		{
			get { return this.fceDevice?.MidnightOffset ?? 0; }
			set { this.fceDevice.MidnightOffset = value; }
		}
		[Range(-999999.0, 999999.0)]
		public double ShortDeadband
		{
			get { return this.fceDevice?.ShortDeadband ?? 0; }
			set { this.fceDevice.ShortDeadband = value; }
		}
		[Range(0.0, 1439.0)]
		public int ShortTime
		{
			get { return this.fceDevice?.ShortTime ?? 0; }
			set { this.fceDevice.ShortTime = value; }
		}
		[Range(-999999.0, 999999.0)]
		public double LongDeadband
		{
			get { return this.fceDevice?.LongDeadband ?? 0; }
			set { this.fceDevice.LongDeadband = value; }
		}
		[Range(0.0, 1439.0)]
		public int LongTime
		{
			get { return this.fceDevice?.LongTime ?? 0; }
			set { this.fceDevice.LongTime = value; }
		}

		public FCEDeviceEditorModel()
		{

		}
		public FCEDeviceEditorModel(FCEDevice fceDevice)
		{
			this.fceDevice = fceDevice;
			this.fceDevice.ImeiNumber = this.fceDevice.ImeiNumber.DefaultIfNull(string.Empty).TrimEnd();
			this.fceDevice.FriendlyName = this.fceDevice.FriendlyName.DefaultIfNull(string.Empty).TrimEnd();
		}

		public List<string> GetScalerTypeList
		{
			get
			{
				List<string> scalerTypeList = new List<string>(96);

				for (int index = 0; index < 96; index++)
				{
					if ((this.fceDevice.ScalerConfiguration[index / 8] & (byte)(1 << index % 8)) == 0)
					{
						continue;
					}

					if ((this.fceDevice.ScalerType[index / 8] & (byte)(1 << index % 8)) == 0)
					{
						scalerTypeList.Add(index.ToString("D2") + "   Temperature");
					}
					else
					{
						scalerTypeList.Add(index.ToString("D2") + "   Level");
					}
				}

				return scalerTypeList;
			}
		}
	}
}