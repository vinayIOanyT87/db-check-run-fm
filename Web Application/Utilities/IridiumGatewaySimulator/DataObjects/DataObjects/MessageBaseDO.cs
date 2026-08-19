namespace DataObjects.DataObjects
{
	using System;
	using System.Xml.Serialization;

	[Serializable, XmlRoot("MessageBaseDO")]
	public class MessageBaseDO
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor
		/// </summary>
		public MessageBaseDO()
		{
			this.Init();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
		}
		#endregion
	}
}
