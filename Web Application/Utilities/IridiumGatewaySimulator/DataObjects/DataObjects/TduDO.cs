namespace DataObjects.DataObjects
{
	public class TduDO
	{
		#region Constructor
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public TduDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		public string TankId { get; set; }
		public string VolumeStr { get; set; }
		public string TemperatureStr { get; set; }
		public string PressureStr { get; set; }
		public string RowIdStr { get; set; }

		public int RowId
		{
			get
			{
				return int.Parse(this.RowIdStr);
			}
		}

		public double? Volume
		{
			get
			{
				if (string.IsNullOrEmpty(this.VolumeStr))
				{
					return null;
				}

				return double.Parse(this.VolumeStr);
			}
		}

		public double? Temperature
		{
			get
			{
				if (string.IsNullOrEmpty(this.TemperatureStr))
				{
					return null;
				}

				return double.Parse(this.TemperatureStr);
			}
		}

		public double? Pressure
		{
			get
			{
				if (string.IsNullOrEmpty(this.PressureStr))
				{
					return null;
				}

				return double.Parse(this.PressureStr);
			}
		}
		#endregion

		#region Private methods
		private void Init()
		{
			this.TankId			= string.Empty;
			this.VolumeStr		= string.Empty;
			this.TemperatureStr = string.Empty;
			this.PressureStr	= string.Empty;
			this.RowIdStr		= "0";
		}
		#endregion

	}
}
