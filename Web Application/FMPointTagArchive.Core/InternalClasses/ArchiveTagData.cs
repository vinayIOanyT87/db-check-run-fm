namespace FMPointTagArchive.Core.InternalClasses
{
    using System;

	public enum PointValueType
	{
		Tag = 0,
		Setting = 1,
		Point = 2,
		All = 3
	}


	internal class ArchiveTagData
    {
        public ArchiveTagData()
        {
            this.PointTagGuid = Guid.Empty;	
			this.PointGuid = Guid.Empty;
			this.EngrUnitIndex = 0;
			this.Value = null;
            this.ValueTimeStamp = DateTimeOffset.MinValue;
			this.QualityString = string.Empty;
			this.DataType = string.Empty;
			this.PropertyID = string.Empty;
			this.Enabled = 0;
		}

        public Guid PointTagGuid { get; set; }

		public Guid PointGuid { get; set; }

		public int EngrUnitIndex { get; set; }

		public object Value { get; set; }

        public DateTimeOffset ValueTimeStamp { get; set; }
		public string QualityString { get; set; }
		public string DataType { get; set; }
		public string PropertyID { get; set; }
		public int Enabled { get; set; }
	}

	internal class PointValueIdentifier
	{
		public Guid IdentityGuid { get; set; }
		public PointValueType PointValueType { get; set; }
		public string PropertyID { get; set; }

	}

	internal class PointValueAccess
	{
		public bool View { get; set; }
		public bool Modify { get; set; }
		public bool ExceedRange { get; set; }
		public bool Override { get; set; }

		public PointValueAccess()
		{
			View = true;
			Modify = true;
			ExceedRange = true;
			Override = true;
		}
	}

}
