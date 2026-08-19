namespace FMBusinessObjects.Attributes
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Designates a field as a database field for auto-generation of SQL 
	/// statements in FMBusinessObjects.
	/// </summary>
	[DataContract]
	[Serializable]
	public class FMPersistedField : Attribute
	{
		[DataMember]
		public string AlternateName { get; set; }

		[DataMember]
		public bool AddOnly { get; set; }

		[DataMember]
		public bool ReadOnly { get; set; }

		[DataMember]
		public bool LiteralEnum { get; set; }

		[DataMember]
		public object DefaultValue { get; set; }

		public FMPersistedField()
		{
			this.Init();
		}

		public FMPersistedField(string alternateName)
		{
			this.Init();

			this.AlternateName = alternateName;
		}

		private void Init()
		{
			this.AddOnly = false;
			this.ReadOnly = false;
			this.LiteralEnum = false;
		}
	}
}
