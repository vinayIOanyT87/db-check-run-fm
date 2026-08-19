using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class ImportTypeDO
	{
		#region Protected data members
		protected string name;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the import type data
		/// object class.
		/// </summary>
		public ImportTypeDO ( )
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		#endregion
	}
}
